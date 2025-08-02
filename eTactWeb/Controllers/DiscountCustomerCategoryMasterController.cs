using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class DiscountCustomerCategoryMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IControlPlan _IControlPlan { get; }
        private readonly ILogger<DiscountCustomerCategoryMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public DiscountCustomerCategoryMasterController(ILogger<DiscountCustomerCategoryMasterController> logger, IDataLogic iDataLogic, IControlPlan iControlPlan, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IControlPlan = iControlPlan;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
    }
}
