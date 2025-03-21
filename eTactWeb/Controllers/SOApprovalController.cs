using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
namespace eTactWeb.Controllers
{
    public class SOApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ISOApproval _ISOApproval;
        private readonly ILogger<SOApprovalController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public SOApprovalController(ILogger<SOApprovalController> logger, IDataLogic iDataLogic, ISOApproval iSOApproval, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISOApproval = iSOApproval;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SOApproval(string type = "")
        {
            var model = new SOApprovalModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.ApprovalType = type;
            return View("SOApproval", model);
        }
        [HttpGet]
        public async Task<IActionResult> ShowSODetail(int ID, int YC, string SONo, string CustOrderNo, string TypeOfApproval, string SONum, string CustOrderNum, string VendorNm)
        {
            var MainModel = new List<SoApprovalDetail>();
            MainModel = await _ISOApproval.ShowSODetail(ID, YC, SONo).ConfigureAwait(true);
            return View(MainModel);
        }
        public async Task<JsonResult> GetUnApprovedSO(string SONO, string AccountName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOApproval.GetProcData("UNAPPROVEDSOSUMM", UID, EmpID, SONO, AccountName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnApprovedAmmSO(string SONO, string AccountName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOApproval.GetProcData("UNAPPROVEDAMMSOSUMM", UID, EmpID, SONO, AccountName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetApprovedAmmSO(string SONO, string AccountName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOApproval.GetProcData("APPROVEDAMMSOSUMM", UID, EmpID, SONO, AccountName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetApprovedSO(string SONO, string AccountName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOApproval.GetProcData("APPROVEDSOSUMM", UID, EmpID, SONO, AccountName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string SONO, string AccountName, string CustOrderNo)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOApproval.GetSearchData(FromDate, ToDate, ApprovalType, UID, EmpID, SONO, AccountName, CustOrderNo).ConfigureAwait(true);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> SaveApproval(int EntryId, int YC, string SONo, string CustOrderNo, string type, string SONum, string CustOrderNum, string VendorNm)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _ISOApproval.SaveApproval(EntryId, YC, SONo, CustOrderNo, type, EmpID).ConfigureAwait(true);
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


                    var model1 = new SOApprovalModel();
                    ViewBag.isSuccess = true;
                    if (string.IsNullOrEmpty(message))
                        TempData["200"] = "200";
                    else
                    {
                        TempData["302"] = message;
                    }
                    //return RedirectToAction("SOApproval", new { type = type, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm });
                    return Json(new { redirectUrl = Url.Action("SOApproval", new { type = type,YC = YC , CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm }) });
                }
            }
            var model = new SOApprovalModel();
            //return RedirectToAction("SOApproval", new { type = type, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm });
            return Json(new { redirectUrl = Url.Action("SOApproval", new { type = type, YC = YC, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm }) });
        }

    }
}