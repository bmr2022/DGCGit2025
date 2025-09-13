using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class PSApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPSApproval _IPSApproval;
        private readonly ILogger<PSApprovalController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PSApprovalController(ILogger<PSApprovalController> logger, IDataLogic iDataLogic, IPSApproval iPSApproval, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPSApproval = iPSApproval;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        public IActionResult PSApproval(string type="")
        {
            var model = new PSApprovalModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ApprovalType= type;
            return View("PSApproval", model);
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetListOfApprovedPS(string FromDate, string ToDate, string ApprovalType, string PONO,string SchNo, string VendorName)
        {
            var model = new PSApprovalModel();
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            string Flag = "";
            if (ApprovalType == "listofApp")
                Flag = "APPROVEDPSSUMM";
            if (ApprovalType == "listofUnApp")
                Flag = "UNAPPROVEDPSSUMM";
            if (ApprovalType == "listofAppAmd")
                Flag = "APPROVEDAMMPSSUMM";
            if (ApprovalType == "listofUnAppAmd")
                Flag = "UNAPPROVEDAMMPSSUMM";

            var Result = await _IPSApproval.GetPSData(Flag, FromDate, ToDate, ApprovalType, PONO, SchNo, VendorName, EmpID, UID);
            
            var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "PONO", "AccountCode", "VendorName", "DeliveryAddress",
       "SchApproved", "PODate", "SchNo", "SchDate", "SchYear", "POYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");

            model.PSAppDetailGrid = CommonFunc.DataTableToList<PSApprovalDetail>(DT, "PODASHBOARD");

            return PartialView("_PSApprovalGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowPSDetail(int ID, int YC, string SchNo,string TypeOfApproval,string ShowOnlyAmendItem)
        {
            HttpContext.Session.Remove("KeyPSApprovalDetail");
            var MainModel = new List<PSApprovalDetail>();
            MainModel = await _IPSApproval.ShowPSDetail(ID, YC, SchNo, ShowOnlyAmendItem).ConfigureAwait(true);
            return View(MainModel);
        }

        [HttpGet]
        public async Task<IActionResult> SaveApproval(int EntryId, int YC, string SchNo, string type)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _IPSApproval.SaveApproval(EntryId, YC, SchNo, type, EmpID);
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
                    return RedirectToAction("PSApproval", new { type = type });
                }
            }
            var model = new PSApprovalModel();
            return RedirectToAction("PSApproval", new { type = type});
        }

    }
}
