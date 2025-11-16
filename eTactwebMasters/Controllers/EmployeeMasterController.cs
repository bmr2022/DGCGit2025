using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class EmployeeMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IEmployeeMaster _IEmployeeMaster;

        public EmployeeMasterController(IDataLogic iDataLogic, IEmployeeMaster iEmployeeMaster)
        {
            _IDataLogic = iDataLogic;
            _IEmployeeMaster = iEmployeeMaster;
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
                model.ApprovalDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.ActualEntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                model.Branch = HttpContext.Session.GetString("Branch");
                model.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                model.ActualEntrybyId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                //model.EntryByEmpName = HttpContext.Session.GetString("EmpName");
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

        //public async Task<IActionResult> DashBoard()
        //{
        //    var model = new EmployeeMasterModel();
        //    model.Mode = "Dashboard";
        //    model = await _IEmployeeMaster.GetDashboardData(model);
        //    //model.EmployeeMasterList = model.EmployeeMasterList.DistinctBy(x => x.EmpId).ToList();

        //    return View(model);

        //}
        public async Task<IActionResult> DashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new EmployeeMasterModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            Dictionary<int, string> monthNames = new Dictionary<int, string>
            {
                {1, "Jan"}, {2, "Feb"}, {3, "Mar"}, {4, "Apr"}, {5, "May"}, {6, "Jun"},
                {7, "Jul"}, {8, "Aug"}, {9, "Sep"}, {10, "Oct"}, {11, "Nov"}, {12, "Dec"}
            };

            model.FromDate = $"{firstDayOfMonth.Day}/{monthNames[firstDayOfMonth.Month]}/{firstDayOfMonth.Year}";
            model.ToDate = $"{now.Day}/{monthNames[now.Month]}/{now.Year}";
            
            //model.ReportType = "SUMMARY";
            var Result = await _IEmployeeMaster.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.EmployeeMasterGrid = CommonFunc.DataTableToList<EmployeeMasterModel>(dt, "EmployeeMasterDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        {
            //model.Mode = "Search";
            var model = new EmployeeMasterModel();
            model = await _IEmployeeMaster.GetDashboardDetailData();

            return PartialView("_EmployeeMasterDashboardSummary", model);

        }
        //public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        //{
        //    try
        //    {
        //        var model = await _IEmployeeMaster.GetDashboardDetailData();
        //        return PartialView("_EmployeeMasterDashboardSummary", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 500; // required for AJAX error
        //        return Json(new { Message = ex.Message });
        //    }
        //}

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
        public async Task<JsonResult> FILLAllowanceMode()
        {
            var JSON = await _IEmployeeMaster.FILLAllowanceMode();
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
        public async Task<JsonResult> GetWorkLocation()
        {
            var JSON = await _IEmployeeMaster.GetWorkLocation();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetRefThrough()
        {
            var JSON = await _IEmployeeMaster.GetRefThrough();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeMaster(EmployeeMasterModel model)
        {
            model.Mode = model.Mode != "U" ? "SAVE" : "UPDATE";
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid");
            string jsonAllDed = HttpContext.Session.GetString("KeyEmployeeMasterGrid_AllDed");
            string jsonEdu = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Edu");
            string jsonExp = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Exp");
            string jsonNJob = HttpContext.Session.GetString("KeyEmployeeMasterGrid_NJob");
            var listAllDed = !string.IsNullOrEmpty(jsonAllDed) ?
       JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(jsonAllDed) :
       new List<EmployeeMasterModel>();

            var listEdu = !string.IsNullOrEmpty(jsonEdu) ?
                JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(jsonEdu) :
                new List<EmployeeMasterModel>();

            var listExp = !string.IsNullOrEmpty(jsonExp) ?
                JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(jsonExp) :
                new List<EmployeeMasterModel>();

            var listNJob = !string.IsNullOrEmpty(jsonNJob) ?
                JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(jsonNJob) :
                new List<EmployeeMasterModel>();
            List<EmployeeMasterModel> EmployeeMasterDetail = new List<EmployeeMasterModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                EmployeeMasterDetail = JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(modelJson);
            }
            var DtAllDed = GetDtAllDedTable(listAllDed, model);
            var DtEdu = GetDtEduTable(listEdu, model);
            var dtexp = GetdtexpTable(listExp, model);
            //var dtNjob = GetdtexpTable(EmployeeMasterDetail);
            var dtNjob = GetdtNjobTable(listNJob, model);
            var Result = await _IEmployeeMaster.SaveEmployeeMaster(model, DtAllDed, DtEdu, dtexp, dtNjob);

            if (Result == null)
            {
                ViewBag.isSuccess = false;
                TempData["Message"] = "Something Went Wrong, Please Try Again.";
                
            }
            else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.isSuccess = true;
                TempData["200"] = "200";
               
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
               
            }
            if (Result.StatusText == "Error")
            {
                ViewBag.isSuccess = false;
                var input = "";
                if (Result?.Result != null)
                {
                    if (Result.Result is string str)
                    {
                        input = str;
                    }
                    else
                    {
                        input = JsonConvert.SerializeObject(Result.Result);
                    }

                    TempData["ErrorMessage"] = input;
                }
                else
                {
                    TempData["500"] = "500";
                }

            }
            return RedirectToAction(nameof(EmployeeMaster), new { ID = 0 });

        }
        private static DataTable GetDtAllDedTable(IList<EmployeeMasterModel> DetailList, EmployeeMasterModel model)
        {
            try
            {
                var DtAllDed = new DataTable();

                DtAllDed.Columns.Add("EmpId", typeof(long));
                DtAllDed.Columns.Add("EmpCode", typeof(string));
                DtAllDed.Columns.Add("SeqNo", typeof(long));
                DtAllDed.Columns.Add("SalHeadEntryId", typeof(long));
                DtAllDed.Columns.Add("Mode", typeof(string));
                DtAllDed.Columns.Add("Percentage", typeof(decimal));
                DtAllDed.Columns.Add("Amount", typeof(decimal));
                DtAllDed.Columns.Add("IncDecType", typeof(string));
                DtAllDed.Columns.Add("PartofPaySlip", typeof(string));
                DtAllDed.Columns.Add("PercentageOfSalaryHeadID", typeof(long));


                foreach (var Item in DetailList)
                {

                    DtAllDed.Rows.Add(
                        new object[]
                        {
                         model.EmpId == 0 ? 0 : model.EmpId,
                model.EmpCode ?? string.Empty,
                Item.SrNo == 0 ? 0 : Item.SrNo,
                Item.SalaryHeadId == 0 ? 0 : Item.SalaryHeadId,
                Item.AllowanceMode ?? string.Empty,
                Item.Percent == 0 ? 0 : Item.Percent,
                Item.AllowanceAmount == 0 ? 0 : Item.AllowanceAmount,
                Item.AllowanceType ?? string.Empty,
                Item.PartyPay ?? string.Empty,
                Item.Percent == 0 ? 0 : Item.Percent
                    });
                }
                DtAllDed.Dispose();
                return DtAllDed;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetDtEduTable(IList<EmployeeMasterModel> DetailList, EmployeeMasterModel model)
        {
            try
            {
                var DtEdu = new DataTable();
                DtEdu.Columns.Add("EmpId", typeof(long));
                DtEdu.Columns.Add("EmpCode", typeof(string));
                DtEdu.Columns.Add("SeqNo", typeof(long));
                DtEdu.Columns.Add("Qualification", typeof(string));
                DtEdu.Columns.Add("Univercity", typeof(string));
                DtEdu.Columns.Add("Percentage", typeof(decimal));
                DtEdu.Columns.Add("PassoutYear", typeof(long));
                DtEdu.Columns.Add("Remarks", typeof(string));


                foreach (var Item in DetailList)
                {

                    DtEdu.Rows.Add(
                        new object[]
                        {
                        model.EmpId == 0 ? 0 : model.EmpId,                     // long
                model.EmpCode ?? string.Empty,                         // string
                Item.SrNo == 0 ? 0 : Item.SrNo,                       // int
                Item.Qualification ?? string.Empty,                   // string
                Item.Univercity_Sch ?? string.Empty,                  // string
                Item.Per is null ? 0 : Item.Per,                      // decimal?
                Item.InYear == 0 ? 0 : Item.InYear,                   // int
                Item.Remark ?? string.Empty

                    });
                }
                DtEdu.Dispose();
                return DtEdu;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetdtexpTable(IList<EmployeeMasterModel> DetailList, EmployeeMasterModel model)
        {
            try
            {
                var dtExp = new DataTable();
                dtExp.Columns.Add("EmpId", typeof(long));
                dtExp.Columns.Add("EmpCode", typeof(string));
                dtExp.Columns.Add("SeqNo", typeof(long));
                dtExp.Columns.Add("CompanyName", typeof(string));
                dtExp.Columns.Add("FromDate", typeof(DateTime));
                dtExp.Columns.Add("ToDate", typeof(DateTime));
                dtExp.Columns.Add("Designation", typeof(string));
                dtExp.Columns.Add("NetSalaryAmt", typeof(decimal));
                dtExp.Columns.Add("GrossSalary", typeof(decimal));
                dtExp.Columns.Add("Country", typeof(string));
                dtExp.Columns.Add("City", typeof(string));
                dtExp.Columns.Add("ContactPersonname", typeof(string));
                dtExp.Columns.Add("ContactPersonNumber", typeof(string));
                dtExp.Columns.Add("HRPersonName", typeof(string));
                dtExp.Columns.Add("HRContactNo", typeof(string));
                dtExp.Columns.Add("Remarks", typeof(string));



                foreach (var Item in DetailList)
                {

                    dtExp.Rows.Add(
                        new object[]
                        {
                        model.EmpId == 0 ? 0 : model.EmpId,                         // long
                model.EmpCode ?? string.Empty,                             // string
                Item.SrNo == 0 ? 0 : Item.SrNo,                           // int
                Item.CompanyName ?? string.Empty,                         // string
                Item.CFromDate ,                           // string (or DateTime.ToString())
                Item.CToDate,                             // string (or DateTime.ToString())
                Item.Designation ?? string.Empty,                         // string
                Item.Salary is 0 ? 0 : Item.Salary,                    // decimal?
                Item.GrossSalary is 0 ? 0 : Item.GrossSalary,          // decimal?
                Item.Country ?? string.Empty,                             // string
                Item.City ?? string.Empty,                                // string
                Item.ContactPersonname ?? string.Empty,                   // string
                Item.ContactPersonNumber ?? string.Empty,                 // string
                Item.HRPersonName ?? string.Empty,                        // string
                Item.HRContactNo ?? string.Empty,                         // string
                Item.ExeRemark ?? string.Empty

                    });
                }
                dtExp.Dispose();
                return dtExp;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetdtNjobTable(IList<EmployeeMasterModel> DetailList, EmployeeMasterModel model)
        {
            try
            {
                var dtNjob = new DataTable();
                dtNjob.Columns.Add("EmpId", typeof(long));
                dtNjob.Columns.Add("EmpCode", typeof(string));
                dtNjob.Columns.Add("SeqNo", typeof(long));
                dtNjob.Columns.Add("NatureOfjob", typeof(string));

                foreach (var Item in DetailList)
                {

                    dtNjob.Rows.Add(
                        new object[]
                        {
                         model.EmpId == 0 ? 0 : model.EmpId,           // long
                model.EmpCode ?? string.Empty,               // string
                Item.SrNo == 0 ? 0 : Item.SrNo,             // int
                Item.NatureOfDuties ?? string.Empty

                    });
                }
                dtNjob.Dispose();
                return dtNjob;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> GetSearchData(string EmpCode, string ReportType)
        {
            var model = new EmployeeMasterModel();
            model.Mode = "Search";
            //model = await _IEmployeeMaster.GetSearchData(model, EmpCode, ReportType).ConfigureAwait(true);
            model.EmployeeMasterList = model.EmployeeMasterList?.DistinctBy(x => x.EmpId).ToList() ?? new List<EmployeeMasterModel>();
            if (ReportType == "DashBoardSummary")
            {
                return PartialView("_EmployeeMasterDashboardSummary", model);
            }
            if (ReportType == "DashBoardDetail")
            {
                return PartialView("_EmployeeMasterDashboardDetail", model);
            }
            return null;
        }

        public async Task<IActionResult> DeleteByID(int ID, string EmpName, int ActualEntrybyId, string EntryByMachineName)
        {
            //var IsDelete = _IDataLogic.IsDelete(ID, "Emp_Id",  ActualEntrybyId,  EntryByMachineName);

                var Result = await _IEmployeeMaster.DeleteByID(ID, EmpName,  ActualEntrybyId,  EntryByMachineName).ConfigureAwait(true);

                if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
                {
                    ViewBag.isSuccess = true;
                    TempData["410"] = "410";
                }
                else if (Result.StatusCode == HttpStatusCode.BadRequest)  
                {
                    TempData["ErrorMessage"] = Result.StatusText;     
                    ViewBag.isSuccess = false;
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_AllDed");
                IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }

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

                    MainModel.AllowanceDeductionList = OrderGrid;

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_AllDed", JsonConvert.SerializeObject(MainModel.AllowanceDeductionList));
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_AllDed");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_AllDed");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_AllDed");
                List<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(modelJson);
                }
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

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_AllDed", JsonConvert.SerializeObject(MainModel.EmployeeMasterGrid));
                }
            }

            return PartialView("_EmployeeAllowanceGrid", MainModel);
        }
        public IActionResult AddToGridDataEduction(EmployeeMasterModel model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Edu");
                IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }

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

                    MainModel.EducationList = OrderGrid;

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_Edu", JsonConvert.SerializeObject(MainModel.EducationList));
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Edu");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Edu");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Edu");
                List<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(modelJson);
                }
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

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_Edu", JsonConvert.SerializeObject(MainModel.EmployeeMasterGrid));
                }
            }

            return PartialView("_EducationalQualificationGrid", MainModel);
        }
        public IActionResult AddToGridDataExperiance(EmployeeMasterModel model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Exp");
                IList<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }

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

                    MainModel.ExperienceList = OrderGrid;

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_Exp", JsonConvert.SerializeObject(MainModel.ExperienceList));
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Exp");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Exp");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<IList<EmployeeMasterModel>>(modelJson);
                }
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
                string modelJson = HttpContext.Session.GetString("KeyEmployeeMasterGrid_Exp");
                List<EmployeeMasterModel> EmployeeMasterGrid = new List<EmployeeMasterModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    EmployeeMasterGrid = JsonConvert.DeserializeObject<List<EmployeeMasterModel>>(modelJson);
                }
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

                    HttpContext.Session.SetString("KeyEmployeeMasterGrid_Exp", JsonConvert.SerializeObject(MainModel.EmployeeMasterGrid));
                }
            }
            return PartialView("_EmployeeMasterExperianceGrid", MainModel);
        }
    }
}
