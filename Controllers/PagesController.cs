using Microsoft.AspNetCore.Mvc;

namespace pcinx_api.Controllers;

public class PagesController : Controller
{
    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        return PhysicalFile(
            Path.Combine(Directory.GetCurrentDirectory(), "index.html"),
            "text/html"
        );
    }

    [HttpGet]
    [Route("/index.html")]
    public IActionResult IndexHtml()
    {
        return Index();
    }

    [HttpGet]
    [Route("/pecas.html")]
    public IActionResult Pecas()
    {
        return PhysicalFile(
            Path.Combine(Directory.GetCurrentDirectory(), "pecas.html"),
            "text/html"
        );
    }

    [HttpGet]
    [Route("/montagem.html")]
    public IActionResult Montagem()
    {
        return PhysicalFile(
            Path.Combine(Directory.GetCurrentDirectory(), "montagem.html"),
            "text/html"
        );
    }

    [HttpGet]
    [Route("/Montagem_Salva.html")]
    public IActionResult MontagemSalva()
    {
        return PhysicalFile(
            Path.Combine(Directory.GetCurrentDirectory(), "Montagem_Salva.html"),
            "text/html"
        );
    }
}

