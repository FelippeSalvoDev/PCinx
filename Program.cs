using pcinx_api.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Adicionar controllers
builder.Services.AddControllers();

// Registrar serviços
builder.Services.AddSingleton<IPartService, PartService>();
builder.Services.AddSingleton<ICompatibilityService, CompatibilityService>();
builder.Services.AddSingleton<IBuildService, BuildService>();

// CORS para permitir o front rodando local 
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();

// Habilitar arquivos estáticos (CSS, JS, imagens) da raiz do projeto
var staticFileOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Directory.GetCurrentDirectory()),
    RequestPath = ""
};
app.UseStaticFiles(staticFileOptions);

app.UseCors();
app.UseRouting();
app.MapControllers();

app.Run();
