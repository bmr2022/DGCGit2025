using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eTactWeb.Controllers
{
    public class POApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPOApproval _IPOApproval;
        private readonly ILogger<POApprovalController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public POApprovalController(ILogger<POApprovalController> logger, IDataLogic iDataLogic, IPOApproval iPOApproval, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPOApproval = iPOApproval;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public IActionResult POApproval()
        {
            var model = new POApprovalModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.FromDate = HttpContext.Session.GetString("FromDate");

            return View("POApproval", model);
        }
        public async Task<JsonResult> GetInitialData()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDFIRSTLVLPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFinalApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDFINALAPPPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmmApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDAmendmentPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmmUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDAmendmentPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFinalUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDFinallevelPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFirstLvlUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDFirstlevelPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowedAction()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IPOApproval.GetAllowedAction("GetAllowedAction", EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string PONO, string VendorName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetSearchData(FromDate, ToDate, ApprovalType,PONO,VendorName,EmpID,UID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowPODetail(int ID, int YC, string PONo, string TypeOfApproval)
        {
            HttpContext.Session.Remove("KeyPoApprovalDetail");
            var MainModel = new List<POApprovalDetail>();
            MainModel = await _IPOApproval.ShowPODetail(ID, YC, PONo, TypeOfApproval).ConfigureAwait(true);
            //string JsonString = JsonConvert.SerializeObject(JSON);
            return View(MainModel);
        }

        [HttpGet]
        public async Task<IActionResult> SaveApproval(int EntryId, int YC, string PONO, string type)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _IPOApproval.SaveApproval(EntryId, YC, PONO, type, EmpID);
            if (Result != null)
            {
                if (Result.StatusText == "Success")
                {
                    var status = "";
                    var message = "";

                    var ds = (DataSet)Result.Result;
                    if (ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            status = Convert.ToString(dt.Rows[0][0]);
                            if (dt.Columns.Count > 1)
                                message = dt.Rows[0][1].ToString();
                        }
                    }


                    var model1 = new POApprovalModel();
                    ViewBag.isSuccess = true;
                    if (string.IsNullOrEmpty(message))
                        TempData["200"] = "200";
                    else
                    {
                        TempData["302"] = message;
                    }
                    //return View("POApproval", model1);
                    return RedirectToAction("POApproval");
                }
            }
            var model = new POApprovalModel();
            //return View("POApproval", model);
          return RedirectToAction("POApproval");
        }

    }
}
