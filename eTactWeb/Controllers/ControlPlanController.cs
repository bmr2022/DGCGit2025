using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactWeb.Controllers
{
    public class ControlPlanController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IControlPlan _IControlPlan { get; }
        private readonly ILogger<ControlPlanController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ControlPlanController(ILogger<ControlPlanController> logger, IDataLogic iDataLogic, IControlPlan iControlPlan, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IControlPlan = iControlPlan;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ControlPlan(int AccountCode, int ID, string Mode, string Requisitionby, int CanSaleBillReqYearcode, string CanRequisitionNo, string CustomerName, int SaleBillEntryId, string SaleBillNo, int SaleBillYearCode, string SaleBillDate, int BillAmt, int INVNetAmt, string ReasonOfCancel, int Approvedby, string CC, int uid, string Canceled, string VoucherType, string ApprovalDate, string CancelDate, string MachineName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new ControlPlanModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            // MainModel.ActualEntryDate = HttpContext.Session.GetString("EntryDate");


            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                
                //MainModel = await _ICancelSaleBillrequisition.GetViewByID(CanRequisitionNo, CanSaleBillReqYearcode).ConfigureAwait(false);
                MainModel.Mode = Mode; 
               
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            return View(MainModel);
        }
    }
}
