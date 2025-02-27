using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class CityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
