using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactwebAdmin.Controllers
{
    public class CancelRequitionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly HttpClient _httpClient;
        private readonly ICancelRequition _ICancelRequition;
        private readonly ILogger<CancelRequitionController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ConnectionStringService _connectionStringService;
        private const string BaseUrl = "http://bhashsms.com/api/sendmsgutil.php";

        public CancelRequitionController(ILogger<CancelRequitionController> logger, IDataLogic iDataLogic, ICancelRequition iCancelRequition, IWebHostEnvironment iWebHostEnvironment, ConnectionStringService connectionStringService, HttpClient httpClient)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICancelRequition = iCancelRequition;
            _IWebHostEnvironment = iWebHostEnvironment;
            _connectionStringService = connectionStringService;
            _httpClient = httpClient;
        }
        [Route("{controller}/CancelRequition")]
        [HttpGet]
        public IActionResult CancelRequition(int ID)
        {
            var model = new CancelRequitionModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.FromDate = HttpContext.Session.GetString("FromDate");
            return View("CancelRequition", model);
        }


    }
}
