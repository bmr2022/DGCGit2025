using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Net;

namespace eTactWeb.Controllers
{
    public class ProcessMasterController : Controller
    {
        private readonly IProcessMaster _IProcessMaster;
        private readonly IDataLogic _IDataLogic;

        public ProcessMasterController(IProcessMaster iProcessMaster, IDataLogic iDataLogic)
        {
            _IProcessMaster = iProcessMaster;
            _IDataLogic = iDataLogic;
        }

        public async Task<IActionResult> DashBoard()
        {
            var model = new ProcessMasterModel();
            model.Mode = "Dashboard";
            model = await _IProcessMaster.GetDashboardData(model).ConfigureAwait(true);
            model.ProcessMasterList = model.ProcessMasterList.DistinctBy(x => x.Process_Id).ToList();
            return View(model);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IProcessMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetSearchData(string StageCode, string StageShortName)
        {
            var model = new ProcessMasterModel();
            model.Mode = "Search";
            model = await _IProcessMaster.GetSearchData(model, StageCode, StageShortName).ConfigureAwait(true);
            model.ProcessMasterList = model.ProcessMasterList?.DistinctBy(x => x.Process_Id).ToList() ?? new List<ProcessMasterModel>();
            return PartialView("_ProcessMasterDashboard", model);
        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            var IsDelete = _IDataLogic.IsDelete(ID, "ProcessId");

            if (IsDelete == 0)
            {
                var Result = await _IProcessMaster.DeleteByID(ID).ConfigureAwait(true);

                if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
                {
                    ViewBag.isSuccess = true;
                    TempData["410"] = "410";
                }
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["423"] = "423";
            }

            return RedirectToAction(nameof(DashBoard));
        }
        public async Task<IActionResult> Form(int ID, string Mode)
        {
            var model = new ProcessMasterModel();
            model = await _IProcessMaster.GetByID(ID).ConfigureAwait(true);
            model.Mode = Mode;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(ProcessMasterModel model)
        {
            ModelState.Remove("TxPageName");
            if (1 == 1) 
            {
                model.Mode = model.ID == 0 ? "Insert" : "Update";
                model.CreatedBy = Constants.UserID;
                model.CC = HttpContext.Session.GetString("UID");
                model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                var Result = await _IProcessMaster.SaveProcessMasterMaster(model).ConfigureAwait(true);

                if (Result == null)
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Something Went Wrong, Please Try Again while saveing Process Master.";
                }
                else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                    return RedirectToAction(nameof(DashBoard));
                }
                else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.Ambiguous)
                {
                    ViewBag.isSuccess = false;
                    TempData["300"] = "300";
                }
                else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                {
                    ViewBag.isSuccess = true;
                    TempData["202"] = "202";
                    return RedirectToAction(nameof(DashBoard));
                }
                return RedirectToAction(nameof(Form), new { ID = 0 });
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["Message"] = "Form Validation Error.";
                return RedirectToAction(nameof(Form), new { ID = 0 });
            }
        }
    }
}
