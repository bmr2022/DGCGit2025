using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eTactWebChatBOT.Models;

namespace eTactWebChatBOT.Controllers;

public class WrenController : Controller
{
    private readonly ILogger<WrenController> _logger;

    public WrenController(ILogger<WrenController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
