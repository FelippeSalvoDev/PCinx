using pcinx_api.Models;

namespace pcinx_api.Services;

public class CompatibilityService : ICompatibilityService
{
    public List<Message> Validate(List<Part> selectedParts)
    {
        var msgs = new List<Message>();
        Part? cpu = selectedParts.FirstOrDefault(p => p.Category.Equals("CPU", StringComparison.OrdinalIgnoreCase));
        Part? mobo = selectedParts.FirstOrDefault(p => p.Category.Equals("Motherboard", StringComparison.OrdinalIgnoreCase));
        Part? gpu = selectedParts.FirstOrDefault(p => p.Category.Equals("GPU", StringComparison.OrdinalIgnoreCase));
        Part? ram = selectedParts.FirstOrDefault(p => p.Category.Equals("RAM", StringComparison.OrdinalIgnoreCase));
        Part? psu = selectedParts.FirstOrDefault(p => p.Category.Equals("PSU", StringComparison.OrdinalIgnoreCase));
        Part? storage = selectedParts.FirstOrDefault(p => p.Category.Equals("Storage", StringComparison.OrdinalIgnoreCase));

        string? CpuAttr(string key) => cpu?.Attributes?.GetValueOrDefault(key);
        string? MoboAttr(string key) => mobo?.Attributes?.GetValueOrDefault(key);
        string? RamAttr(string key) => ram?.Attributes?.GetValueOrDefault(key);
        string? GpuAttr(string key) => gpu?.Attributes?.GetValueOrDefault(key);
        string? PsuAttr(string key) => psu?.Attributes?.GetValueOrDefault(key);

        // Verificação de peças essenciais (agora como ERRO)
        var requiredCats = new[] { "CPU", "Motherboard", "RAM", "Storage", "PSU" };
        foreach (var rc in requiredCats)
        {
            if (!selectedParts.Any(p => p.Category.Equals(rc, StringComparison.OrdinalIgnoreCase)))
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

