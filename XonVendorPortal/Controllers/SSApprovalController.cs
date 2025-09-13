using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class SSApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ISSApproval _ISSApproval;
        private readonly ILogger<PSApprovalController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public SSApprovalController(ILogger<PSApprovalController> logger, IDataLogic iDataLogic, ISSApproval iSSApproval, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISSApproval = iSSApproval;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        public IActionResult SSApproval(string type = "")
        {
            var model = new SSApprovalModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.ApprovalType = type;
            return View("SSApproval", model);
        }
        public async Task<IActionResult> GetListOfApprovedSS(string FromDate, string ToDate, string ApprovalType, string SONO, string SchNo, string VendorName)
        {
            var model = new SSApprovalModel();
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            string Flag = "";
            if (ApprovalType == "listofApp")
                Flag = "APPROVEDSSSUMM";
            if (ApprovalType == "listofUnApp")
                Flag = "UNAPPROVEDSSSUMM";
            if (ApprovalType == "listofAppAmd")
                Flag = "APPROVEDAMMSSSUMM";
            if (ApprovalType == "listofUnAppAmd")
                Flag = "UNAPPROVEDAMMSSSUMM";

            var Result = await _ISSApproval.GetSSData(Flag, FromDate, ToDate, ApprovalType, SONO, SchNo, VendorName, EmpID, UID);

            if (Result.StatusText == "Error")
            {

            }
            else
            {
                DataTable sourceTable = Result.Result.Tables[0];

                var DT = sourceTable.DefaultView.ToTable(true, "EntryID", "SONO", "AccountCode", "CustomerName", "DeliveryAddress",
                "SchApproved", "SODate", "SchNo", "SchDate", "SchYear", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");

                model.SSApprovalGrid = CommonFunc.DataTableToList<SSApprovalDetail>(DT, "SaleSchedule");
            }
            return PartialView("_SSApprovalGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowSSDetail(int ID, int YC, string SchNo, string TypeOfApproval, string VendorName, string SONO)
        {
            HttpContext.Session.Remove("KeySSApprovalDetail");
            var MainModel = new List<SSApprovalDetail>();
            MainModel = await _ISSApproval.ShowSSDetail(ID, YC, SchNo).ConfigureAwait(true);
            return View(MainModel);
        }

        [HttpGet]
        public async Task<IActionResult> SaveApproval(int EntryId, int YC, string SchNo, string type, string SONO, string VendorName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _ISSApproval.SaveApproval(EntryId, YC, SchNo, type, EmpID);
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
                    return RedirectToAction("SSApproval", new { type = type });
                }
            }
            var model = new PSApprovalModel();
            return RedirectToAction("SSApproval", new { type = type, SONO = SONO, VendorName = VendorName });
        }
    }
}