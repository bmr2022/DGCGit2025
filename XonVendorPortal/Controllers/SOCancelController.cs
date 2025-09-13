using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class SOCancelController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ISOCancel _ISOCancel;
        private readonly ILogger<SOCancelController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public SOCancelController(ILogger<SOCancelController> logger, IDataLogic iDataLogic, ISOCancel iSOCancel, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISOCancel = iSOCancel;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SOCancel(string type = "")
        {
            var model = new SOCancelModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.CancelationType = type;
            return View("SOCancel", model);
        }

        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string CancelType, string SONO, string AccountName, string CustOrderNo)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOCancel.GetSearchData(FromDate, ToDate, CancelType, UID, EmpID, SONO, AccountName, CustOrderNo).ConfigureAwait(true);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowSODetail(int ID, int YC, string SONo, string CustOrderNo, string TypeOfApproval, string SONum, string CustOrderNum, string VendorNm)
        {
            var MainModel = new List<SoCancelDetail>();
            MainModel = await _ISOCancel.ShowSODetail(ID, YC, SONo).ConfigureAwait(true);
            return View(MainModel);
        }


        [HttpGet]
        public async Task<IActionResult> SaveActivation(int EntryId, int YC, string SONo, string CustOrderNo, string type, string SONum, string CustOrderNum, string VendorNm)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _ISOCancel.SaveActivation(EntryId, YC, SONo, CustOrderNo, type, EmpID).ConfigureAwait(true);
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
                    return Json(new { redirectUrl = Url.Action("SOCancel", new { type = type, YC = YC, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm }) });
                }
            }
            var model = new SOApprovalModel();
            //return RedirectToAction("SOApproval", new { type = type, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm });
            return Json(new { redirectUrl = Url.Action("SOCancel", new { type = type, YC = YC, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm }) });
        }

    }
}
