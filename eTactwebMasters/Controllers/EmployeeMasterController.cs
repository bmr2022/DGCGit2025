using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace eTactWeb.Controllers
{
    public class EmployeeMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IEmployeeMaster _IEmployeeMaster;
        private readonly IMemoryCache _MemoryCache;

        public EmployeeMasterController(IDataLogic iDataLogic, IEmployeeMaster iEmployeeMaster, IMemoryCache memoryCache)
        {
            _IDataLogic = iDataLogic;
            _IEmployeeMaster = iEmployeeMaster;
            _MemoryCache = memoryCache;
        }

        public async Task<IActionResult> EmployeeMaster(int ID, string Mode)
        {

            var model = new EmployeeMasterModel();
            if (ID == 0)
            {
                model.EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.DOB = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.DateOfJoining = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.DateOfResignation = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.Branch = HttpContext.Session.GetString("Branch");
                model.Mode = Mode;
                model.Active = "Y";

            }
            else
            {
                model = await _IEmployeeMaster.GetByID(ID);
                model.Mode = Mode;

                model.EntryDate = FormatDate(model.EntryDate);
                model.DOB = FormatDate(model.DOB);
                model.DateOfJoining = FormatDate(model.DateOfJoining);
                model.DateOfResignation = FormatDate(model.DateOfResignation);
            }

            model.DesignationList = await _IDataLogic.GetDropDownList("FillDesignation", "HREmployeeMaster");
            model.DepartmentList = await _IDataLogic.GetDropDownList("FillDepartment", "HREmployeeMaster");
            model.CategoryList = await _IDataLogic.GetDropDownList("FillCategory", "HREmployeeMaster");
            model.ShiftList = await _IDataLogic.GetDropDownList("Shift", "HREmployeeMaster");

            return View(model);
        }

        public async Task<IActionResult> DashBoard()
        {
            var model = new EmployeeMasterModel();
            model.Mode = "Dashboard";
            model = await _IEmployeeMaster.GetDashboardData(model);
            model.EmployeeMasterList = model.EmployeeMasterList.DistinctBy(x => x.EmpId).ToList();

            return View(model);

        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IEmployeeMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetEmpIdandEmpCode(string designation, string department)
        {
            var JSON = await _IEmployeeMaster.GetEmpIdandEmpCode(designation, department);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(EmployeeMasterModel model)
        {
            ModelState.Remove("TxPageName");
            if (ModelState.IsValid)
            {
                model.Mode = model.Mode != "U" ? "SAVE" : "UPDATE";
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                var Result = await _IEmployeeMaster.SaveEmployeeMaster(model);

                if (Result == null)
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Something Went Wrong, Please Try Again.";
                    return RedirectToAction(nameof(DashBoard));
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

        public async Task<IActionResult> GetSearchData(string EmpCode, string Name)
        {
            var model = new EmployeeMasterModel();
            model.Mode = "Search";
            model = await _IEmployeeMaster.GetSearchData(model, EmpCode).ConfigureAwait(true);
            model.EmployeeMasterList = model.EmployeeMasterList?.DistinctBy(x => x.EmpId).ToList() ?? new List<EmployeeMasterModel>();
            return PartialView("_EmployeeMasterDashboard", model);
        }

        public async Task<IActionResult> DeleteByID(int ID, string EmpName)
        {
            var IsDelete = _IDataLogic.IsDelete(ID, "Emp_Id");

            if (IsDelete == 0)
            {
                var Result = await _IEmployeeMaster.DeleteByID(ID, EmpName).ConfigureAwait(true);

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
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }
        }

        private string FormatDate(string date)
        {
            return DateTime.TryParse(date, out DateTime parsedDate) ? parsedDate.ToString("dd/MM/yyyy") : date;
        }
    }
}
