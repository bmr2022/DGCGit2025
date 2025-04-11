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
        public async Task<JsonResult> GetSalaryHead()
        {
            var JSON = await _IEmployeeMaster.GetSalaryHead();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSalaryMode(int SalaryHeadId)
        {
            var JSON = await _IEmployeeMaster.GetSalaryMode(SalaryHeadId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetJobDepartMent()
        {
            var JSON = await _IEmployeeMaster.GetJobDepartMent();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
      
        public async Task<JsonResult> GetJobDesignation()
        {
            var JSON = await _IEmployeeMaster.GetJobDesignation();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
      
        public async Task<JsonResult> GetJobShift()
        {
            var JSON = await _IEmployeeMaster.GetJobShift();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEmployeeType()
        {
            var JSON = await _IEmployeeMaster.GetEmployeeType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetReportingMg()
        {
            var JSON = await _IEmployeeMaster.GetReportingMg();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
      

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeMaster(EmployeeMasterModel model)
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
                return RedirectToAction(nameof(EmployeeMaster), new { ID = 0 });
            
        }

        public async Task<IActionResult> GetSearchData(string EmpCode, string ReportType)
        {
            var model = new EmployeeMasterModel();
            model.Mode = "Search";
            model = await _IEmployeeMaster.GetSearchData(model, EmpCode, ReportType).ConfigureAwait(true);
            model.EmployeeMasterList = model.EmployeeMasterList?.DistinctBy(x => x.EmpId).ToList() ?? new List<EmployeeMasterModel>();
            if (ReportType== "DashBoardSummary")
            {
                return PartialView("_EmployeeMasterDashboardSummary", model);
            }
            if (ReportType== "DashBoardDetail")
            {
                return PartialView("_EmployeeMasterDashboardDetail", model);
            }
            return null;
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

        public IActionResult AddToGridData(EmployeeMasterModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterGrid", out IList<EmployeeMasterModel> EmployeeMasterGrid);

                var MainModel = new EmployeeMasterModel();
                var WorkOrderPGrid = new List<EmployeeMasterModel>();
                var OrderGrid = new List<EmployeeMasterModel>();
                var ssGrid = new List<EmployeeMasterModel>();

                if (model != null)
                {
                    if (EmployeeMasterGrid == null)
                    {
                        model.SrNo = 1;
                        OrderGrid.Add(model);
                    }
                    else
                    {
                        if (EmployeeMasterGrid.Any(x => (x.SalaryHead == model.SalaryHead)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //count = WorkOrderProcessGrid.Count();
                            model.SrNo = EmployeeMasterGrid.Count + 1;
                            OrderGrid = EmployeeMasterGrid.Where(x => x != null).ToList();
                            ssGrid.AddRange(OrderGrid);
                            OrderGrid.Add(model);

                        }

                    }

                    MainModel.EmployeeMasterGrid = OrderGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                }
                return PartialView("_EmployeeAllowanceGrid", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SrNO, string Mode)
        {
            IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterGrid", out EmployeeMasterGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterGrid", out EmployeeMasterGrid);
            }
            IEnumerable<EmployeeMasterModel> SSBreakdownGrid = EmployeeMasterGrid;
            if (EmployeeMasterGrid != null)
            {
                SSBreakdownGrid = EmployeeMasterGrid.Where(x => x.SrNo == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SrNO, string Mode)
        {
            var MainModel = new EmployeeMasterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterGrid", out List<EmployeeMasterModel> EmployeeMasterGrid);
                int Indx = SrNO - 1;

                if (EmployeeMasterGrid != null && EmployeeMasterGrid.Count > 0)
                {
                    EmployeeMasterGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in EmployeeMasterGrid)
                    {
                        Indx++;
                        item.SrNo = Indx;
                    }
                    MainModel.EmployeeMasterGrid = EmployeeMasterGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
            }

            return PartialView("_EmployeeAllowanceGrid", MainModel);
        }
        public IActionResult AddToGridDataEduction(EmployeeMasterModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterEductionGrid", out IList<EmployeeMasterModel> EmployeeMasterGrid);

                var MainModel = new EmployeeMasterModel();
                var WorkOrderPGrid = new List<EmployeeMasterModel>();
                var OrderGrid = new List<EmployeeMasterModel>();
                var ssGrid = new List<EmployeeMasterModel>();

                if (model != null)
                {
                    if (EmployeeMasterGrid == null)
                    {
                        model.SrNo = 1;
                        OrderGrid.Add(model);
                    }
                    else
                    {
                        if (EmployeeMasterGrid.Any(x => (x.SrNo == model.SrNo)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //count = WorkOrderProcessGrid.Count();
                            model.SrNo = EmployeeMasterGrid.Count + 1;
                            OrderGrid = EmployeeMasterGrid.Where(x => x != null).ToList();
                            ssGrid.AddRange(OrderGrid);
                            OrderGrid.Add(model);

                        }

                    }

                    MainModel.EmployeeMasterGrid = OrderGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterEductionGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                }
                return PartialView("_EducationalQualificationGrid", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditEductionItemRow(int SrNO, string Mode)
        {
            IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterEductionGrid", out EmployeeMasterGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterEductionGrid", out EmployeeMasterGrid);
            }
            IEnumerable<EmployeeMasterModel> SSBreakdownGrid = EmployeeMasterGrid;
            if (EmployeeMasterGrid != null)
            {
                SSBreakdownGrid = EmployeeMasterGrid.Where(x => x.SrNo == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteEductionItemRow(int SrNO, string Mode)
        {
            var MainModel = new EmployeeMasterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterEductionGrid", out List<EmployeeMasterModel> EmployeeMasterGrid);
                int Indx = SrNO - 1;

                if (EmployeeMasterGrid != null && EmployeeMasterGrid.Count > 0)
                {
                    EmployeeMasterGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in EmployeeMasterGrid)
                    {
                        Indx++;
                        item.SrNo = Indx;
                    }
                    MainModel.EmployeeMasterGrid = EmployeeMasterGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterEductionGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
            }

            return PartialView("_EducationalQualificationGrid", MainModel);
        }
        public IActionResult AddToGridDataExperiance(EmployeeMasterModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterExperianceGrid", out IList<EmployeeMasterModel> EmployeeMasterGrid);

                var MainModel = new EmployeeMasterModel();
                var WorkOrderPGrid = new List<EmployeeMasterModel>();
                var OrderGrid = new List<EmployeeMasterModel>();
                var ssGrid = new List<EmployeeMasterModel>();

                if (model != null)
                {
                    if (EmployeeMasterGrid == null)
                    {
                        model.SrNo = 1;
                        OrderGrid.Add(model);
                    }
                    else
                    {
                        if (EmployeeMasterGrid.Any(x => (x.SrNo == model.SrNo)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //count = WorkOrderProcessGrid.Count();
                            model.SrNo = EmployeeMasterGrid.Count + 1;
                            OrderGrid = EmployeeMasterGrid.Where(x => x != null).ToList();
                            ssGrid.AddRange(OrderGrid);
                            OrderGrid.Add(model);

                        }

                    }

                    MainModel.EmployeeMasterGrid = OrderGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterExperianceGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                }
                return PartialView("_EmployeeMasterExperianceGrid", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditExperianceItemRow(int SrNO, string Mode)
        {
            IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterExperianceGrid", out EmployeeMasterGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterExperianceGrid", out EmployeeMasterGrid);
            }
            IEnumerable<EmployeeMasterModel> SSBreakdownGrid = EmployeeMasterGrid;
            if (EmployeeMasterGrid != null)
            {
                SSBreakdownGrid = EmployeeMasterGrid.Where(x => x.SrNo == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteExperianceItemRow(int SrNO, string Mode)
        {
            var MainModel = new EmployeeMasterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyEmployeeMasterExperianceGrid", out List<EmployeeMasterModel> EmployeeMasterGrid);
                int Indx = SrNO - 1;

                if (EmployeeMasterGrid != null && EmployeeMasterGrid.Count > 0)
                {
                    EmployeeMasterGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in EmployeeMasterGrid)
                    {
                        Indx++;
                        item.SrNo = Indx;
                    }
                    MainModel.EmployeeMasterGrid = EmployeeMasterGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyEmployeeMasterExperianceGrid", MainModel.EmployeeMasterGrid, cacheEntryOptions);
                }
            }

            return PartialView("_EmployeeMasterExperianceGrid", MainModel);
        }
    }
}
