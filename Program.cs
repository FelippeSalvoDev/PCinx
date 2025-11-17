using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORS para permitir o front rodando local 
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
app.UseCors();

// Carregar dados
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "parts.json");
var json = await File.ReadAllTextAsync(dataPath);
var parts = JsonSerializer.Deserialize<List<Part>>(json) ?? new();

app.MapGet("/", () => Results.Ok(new { name = "PCinx API", parts = parts.Count }));

app.MapGet("/api/parts", (string? category) =>
{
    var q = parts.AsEnumerable();
    if (!string.IsNullOrWhiteSpace(category))
        q = q.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(q);
});

app.MapPost("/api/build/validate", (BuildRequest req) =>
{
    var selected = parts.Where(p => req.PartIds.Contains(p.Id)).ToList();
    var messages = Compatibility.Validate(selected);
    return Results.Ok(new { messages });
});

app.MapPost("/api/build/save", (BuildRequest req) =>
{
    var selected = parts.Where(p => req.PartIds.Contains(p.Id)).ToList();
    var code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    var outObj = new { code, createdAt = DateTimeOffset.UtcNow, parts = selected };
    var outDir = Path.Combine(AppContext.BaseDirectory, "Data", "builds");
    Directory.CreateDirectory(outDir);
    File.WriteAllText(Path.Combine(outDir, $"{code}.json"),
        JsonSerializer.Serialize(outObj, new JsonSerializerOptions { WriteIndented = true }));
    return Results.Ok(new { code });
});

app.Run();

record BuildRequest(List<string> PartIds);

record Part
{
    public string Id { get; init; } = default!;
    public string Category { get; init; } = default!; // CPU, Motherboard, GPU, RAM, Storage, PSU, Case
    public string Name { get; init; } = default!;
    public string? Brand { get; init; }
    public decimal Price { get; init; }
    public Dictionary<string, string>? Attributes { get; init; }
}

record Message
{
    [JsonPropertyName("level")] public string Level { get; init; } = "info"; // info|warning|error
    [JsonPropertyName("text")] public string Text { get; init; } = "";
}

static class Compatibility
{
    public static List<Message> Validate(List<Part> selected)
{
    var msgs = new List<Message>();
    Part? cpu = selected.FirstOrDefault(p => p.Category.Equals("CPU", StringComparison.OrdinalIgnoreCase));
    Part? mobo = selected.FirstOrDefault(p => p.Category.Equals("Motherboard", StringComparison.OrdinalIgnoreCase));
    Part? gpu = selected.FirstOrDefault(p => p.Category.Equals("GPU", StringComparison.OrdinalIgnoreCase));
    Part? ram = selected.FirstOrDefault(p => p.Category.Equals("RAM", StringComparison.OrdinalIgnoreCase));
    Part? psu = selected.FirstOrDefault(p => p.Category.Equals("PSU", StringComparison.OrdinalIgnoreCase));
    Part? storage = selected.FirstOrDefault(p => p.Category.Equals("Storage", StringComparison.OrdinalIgnoreCase));

    string? CpuAttr(string key) => cpu?.Attributes?.GetValueOrDefault(key);
    string? MoboAttr(string key) => mobo?.Attributes?.GetValueOrDefault(key);
    string? RamAttr(string key) => ram?.Attributes?.GetValueOrDefault(key);
    string? GpuAttr(string key) => gpu?.Attributes?.GetValueOrDefault(key);
    string? PsuAttr(string key) => psu?.Attributes?.GetValueOrDefault(key);

    // Verificação de peças essenciais (agora como ERRO)
    var requiredCats = new[] { "CPU", "Motherboard", "RAM", "Storage", "PSU" };
    foreach (var rc in requiredCats)
    {
        if (!selected.Any(p => p.Category.Equals(rc, StringComparison.OrdinalIgnoreCase)))
        {
            msgs.Add(new Message { Level = "error", Text = $"ERRO: Componente essencial ausente - {rc}." });
        }
    }

    // CPU x Placa-mãe - socket
    if (cpu != null && mobo != null)
    {
        var cpuSocket = CpuAttr("Socket");
        var moboSocket = MoboAttr("Socket");
        
        if (string.IsNullOrEmpty(cpuSocket))
            msgs.Add(new Message { Level = "error", Text = "ERRO: Não foi possível verificar o socket do CPU." });
        else if (string.IsNullOrEmpty(moboSocket))
            msgs.Add(new Message { Level = "error", Text = "ERRO: Não foi possível verificar o socket da placa-mãe." });
        else if (!cpuSocket.Equals(moboSocket, StringComparison.OrdinalIgnoreCase))
            msgs.Add(new Message { Level = "error", Text = $"ERRO: Socket incompatível - CPU {cpuSocket} ≠ Placa-mãe {moboSocket}" });
    }

    // RAM x Placa-mãe - tipo
    if (ram != null && mobo != null)
    {
        var ramType = RamAttr("Type");
        var moboRamType = MoboAttr("MemoryType");
        
        if (!string.IsNullOrEmpty(ramType) && !string.IsNullOrEmpty(moboRamType))
        {
            if (!ramType.Equals(moboRamType, StringComparison.OrdinalIgnoreCase))
                msgs.Add(new Message { Level = "error", Text = $"ERRO: Tipo de RAM incompatível - {ramType} não suportado pela placa-mãe ({moboRamType})" });
        }
    }

    // Verificação de potência da fonte
    int cpuTdp = int.TryParse(CpuAttr("TdpW"), out var ctdp) ? ctdp : 65;
    int gpuTdp = int.TryParse(GpuAttr("TdpW"), out var gtdp) ? gtdp : 0;
    int headroom = 150; // Margem de segurança aumentada
    int required = cpuTdp + gpuTdp + headroom;

    if (psu != null)
    {
        int psuW = int.TryParse(PsuAttr("WattageW"), out var wattage) ? wattage : 0;
        if (psuW < required)
            msgs.Add(new Message { Level = "error", Text = $"ERRO: Fonte subdimensionada - Necessário {required}W (selecionado: {psuW}W)" });
    }

    // Só mostra mensagem de sucesso se não houver ERROS
    if (!msgs.Any(m => m.Level == "error"))
    {
        if (msgs.Any()) // Se tiver apenas avisos/info
        {
            msgs.Insert(0, new Message { Level = "warning", Text = "⚠️ Montagem válida com observações:" });
        }
        else // Se estiver tudo perfeito
        {
            msgs.Add(new Message { Level = "ok", Text = "✅ Compatibilidade perfeita!" });
        }
    }

    return msgs;
}
}
