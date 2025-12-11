using Microsoft.AspNetCore.Mvc;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using System.Net;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing.Drawing2D;
using System.Runtime.Caching;
using eTactwebHR.Models;
using System.Xml.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Drawing.BarCodes;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Ocsp;

namespace eTactwebHR.Controllers
{
    public class GateAttendanceController : Controller
    {
        private readonly IMemoryCacheService _iMemoryCacheService;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public ILogger<GateAttendanceModel> _Logger { get; set; }
        public CultureInfo CI { get; private set; }
        public EncryptDecrypt EncryptDecrypt { get; private set; }
        public IDataLogic IDataLogic { get; private set; }
        public IGateAttendance IGateAttendance { get; set; }

        public GateAttendanceController(IGateAttendance iGateAttendance, IDataLogic iDataLogic, ILogger<GateAttendanceModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, IMemoryCache iMemoryCache)
        {
            _iMemoryCacheService = iMemoryCacheService;
            IGateAttendance = iGateAttendance;
            IDataLogic = iDataLogic;
            _Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            CI = new CultureInfo("en-GB");
            _IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
            _MemoryCache = iMemoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GateAttendance(int ID, int YearCode, string Mode, string? AttendanceEntryMethodType, string FromDate = "", string ToDate = "", string DashboardType = "", string DashDepartment = "", string DashCategory = "", string DashDesignation = "", string DashEmployee = "",string DashAttendStatus = "", string Searchbox = "", bool IsIncreament = false)
        {
            GateAttendanceModel tMainModel = new();
            if (IsIncreament)
            {
                string GateAttendanceJson = HttpContext.Session.GetString("tempGateAttendance");
                if (!string.IsNullOrEmpty(GateAttendanceJson))
                {
                    tMainModel = JsonConvert.DeserializeObject<GateAttendanceModel>(GateAttendanceJson);
                    if (tMainModel != null)
                    {
                        if (string.Equals(tMainModel.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
                        {
                            tMainModel.EmpAttDate = (!string.IsNullOrEmpty(tMainModel.strEmpAttDate) ? CommonFunc.ParseDate(tMainModel.strEmpAttDate)  : (tMainModel.EmpAttDate ?? DateTime.Now)).AddDays(1);
                        }
                        else if (string.Equals(tMainModel.DayOrMonthType, "monthly", StringComparison.OrdinalIgnoreCase))
                        {
                            tMainModel.EmpAttDate = new DateTime(tMainModel.GateAttYearCode, tMainModel.intEmpAttMonth ?? 1, 1, 0, 0, 0).AddMonths(1);
                            tMainModel.intEmpAttMonth = tMainModel.EmpAttDate.HasValue ? tMainModel.EmpAttDate.Value.Month : 1;
                        }
                        tMainModel.strEmpAttDate = CommonFunc.ParseFormattedDate(tMainModel.EmpAttDate.Value.ToString("dd/MM/yyyy"));

                        if (tMainModel.Mode == "U" || tMainModel.Mode == "V")
                        {
                            int nextId = tMainModel.ID + 1;
                            var nextModel = await IGateAttendance.GetViewByID(nextId, YearCode).ConfigureAwait(false);

                            if (nextModel != null && nextModel.ID > 0)
                            {
                                tMainModel = nextModel;
                                tMainModel.Mode = tMainModel.Mode; 
                                tMainModel.strEmpAttDate = CommonFunc.ParseFormattedDate(tMainModel.EmpAttDate?.ToString("dd/MM/yyyy"));
                            }
                            else
                            {
                                tMainModel.ID = 0;
                                tMainModel.Mode = "INSERT";
                            }
                        }
                        else if (tMainModel.Mode == "INSERT")
                        {
                            tMainModel.ID = 0;
                            tMainModel.Mode = "INSERT";
                        }

                        tMainModel.GateAttYearCode = YearCode;
                        tMainModel = await BindModels(tMainModel).ConfigureAwait(false);
                        tMainModel.HolidayList = (!string.IsNullOrEmpty(tMainModel.EmpCategoryId) && tMainModel.EmpAttDate != null)
                            ? GetHolidayList(Convert.ToInt32(tMainModel.EmpCategoryId), tMainModel.EmpAttDate ?? new DateTime(tMainModel.GateAttYearCode, 1, 1), YearCode)?.HolidayList ?? new List<GateAttendanceHolidayModel>()
                            : null;

                        ViewBag.DeptList = await IDataLogic.GetDropDownList("FillDepartment", "HRSPGateAttendanceMainDetail");
                        ViewBag.DesigList = await IDataLogic.GetDropDownList("FillDesignation", "HRSPGateAttendanceMainDetail");
                        ViewBag.EmployeeList = await IDataLogic.GetDropDownList("FillEmployee", "HRSPGateAttendanceMainDetail");
                        tMainModel.IsIncreamented = true;
                        tMainModel.EmpCategoryId =  string.IsNullOrEmpty(tMainModel.EmpCategoryId) ? tMainModel.CategoryCode : tMainModel.EmpCategoryId;
                        tMainModel.strEmpAttMonth = (tMainModel.intEmpAttMonth != null && tMainModel.intEmpAttMonth > 0) ? new DateTime(Convert.ToInt32(YearCode), Convert.ToInt32(tMainModel.intEmpAttMonth), 1).ToString("MMM") : tMainModel.strEmpAttMonth;
                        tMainModel.GateAttDetailsList = new List<GateAttendanceModel>();
                        string serializedGrid = JsonConvert.SerializeObject(tMainModel);
                        HttpContext.Session.SetString("GateAttendance", serializedGrid);
                        HttpContext.Session.Remove("tempGateAttendance");

                        return View(tMainModel);
                    }
                }
            }
            HttpContext.Session.Remove("GateAttendance");
            var MainModel = new GateAttendanceModel();
            MainModel.GateAttYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.GateAttEntryDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
            MainModel.strEmpAttDate = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
            DateTime fromDate = new DateTime(MainModel.GateAttYearCode, 1, 1);
            DateTime toDate = new DateTime(MainModel.GateAttYearCode, 12, 31);
            MainModel.NFromDate = CommonFunc.ParseFormattedDate(fromDate.ToString("dd/MM/yyyy"));
            MainModel.NToDate = CommonFunc.ParseFormattedDate(toDate.ToString("dd/MM/yyyy"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.CreatedByName = GetEmpByMachineName();
            MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.UpdatedByName = GetEmpByMachineName();
            MainModel.Branch = HttpContext.Session.GetString("Branch");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IGateAttendance.GetViewByID(ID, YearCode).ConfigureAwait(false);
                MainModel.strEmpAttDate = (MainModel.EmpAttDate != null) ? CommonFunc.ParseFormattedDate(MainModel.EmpAttDate?.Date.ToString("dd/MM/yyyy")) : CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.GateAttYearCode = YearCode;
                fromDate = new DateTime(MainModel.GateAttYearCode, 1, 1);
                toDate = new DateTime(MainModel.GateAttYearCode, 12, 31);
                MainModel.NFromDate = CommonFunc.ParseFormattedDate(fromDate.ToString("dd/MM/yyyy"));
                MainModel.NToDate = CommonFunc.ParseFormattedDate(toDate.ToString("dd/MM/yyyy"));
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.intEmpAttMonth = string.IsNullOrWhiteSpace(MainModel.strEmpAttMonth) ? 0 : new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }.Select((m, i) => new { m, i }).FirstOrDefault(x => MainModel.strEmpAttMonth.StartsWith(x.m, StringComparison.OrdinalIgnoreCase))?.i + 1 ?? 0;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.HolidayList = (!string.IsNullOrEmpty(MainModel.EmpCategoryId) && MainModel.EmpAttDate != null) ? GetHolidayList(Convert.ToInt32(MainModel.EmpCategoryId), MainModel.EmpAttDate ?? new DateTime(MainModel.GateAttYearCode, 1, 1), YearCode)?.HolidayList ?? new List<GateAttendanceHolidayModel>() : null;
                if (string.Equals(MainModel.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime? currentDate = MainModel.EmpAttDate;
                    string currentDay = currentDate?.DayOfWeek.ToString() ?? "";
                    var holiday = MainModel.HolidayList?.FirstOrDefault(h => h.HolidayYear == MainModel.GateAttYearCode && h.HolidayEffFrom >= currentDate && h.HolidayEffTill <= currentDate);
                    bool isWeekoff = MainModel.HolidayList?.Any(a => a.HolidayEffFrom == null && a.HolidayEffTill == null && a.DayName.Equals(currentDay, StringComparison.OrdinalIgnoreCase)) ?? false;
                    bool isHoliday = !isWeekoff && holiday != null;
                    string cssClass = isWeekoff ? "weekoff-time" : (isHoliday ? "holiday-time" : "");
                    bool allowEdit = (holiday != null && isHoliday && string.Equals(holiday?.AllowedCompOff, "YES", StringComparison.OrdinalIgnoreCase)) || !isHoliday;
                    if (isWeekoff)
                        allowEdit = false;

                    ViewBag.IsWeekoff = isWeekoff;
                    ViewBag.IsHoliday = isHoliday;
                    ViewBag.AllowEdit = allowEdit;
                    ViewBag.CssClass = cssClass;
                }
                ViewBag.DeptList = await IDataLogic.GetDropDownList("FillDepartment", "HRSPGateAttendanceMainDetail");
                ViewBag.DesigList = await IDataLogic.GetDropDownList("FillDesignation", "HRSPGateAttendanceMainDetail");
                //ViewBag.ShiftList = await IDataLogic.GetDropDownList("FillShift", "HRSPGateAttendanceMainDetail");
                ViewBag.EmployeeList = await IDataLogic.GetDropDownList("FillEmployee", "HRSPGateAttendanceMainDetail");

                string serializedGrid = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("GateAttendance", serializedGrid);
                //var taxGrid = MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd;
                //string serializedKeyTaxGrid = JsonConvert.SerializeObject(taxGrid);
                //HttpContext.Session.SetString("KeyTaxGrid", serializedKeyTaxGrid);
            }
            else
            {
                if (string.IsNullOrEmpty(Mode) && ID == 0)
                {
                    MainModel = await BindModels(MainModel).ConfigureAwait(false);
                    MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
                    MainModel.Mode = "INSERT";
                }
                else
                {
                    MainModel = await BindModels(MainModel).ConfigureAwait(false);
                    MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
                    MainModel.Mode = Mode;
                }
            }

            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByName = GetEmpByMachineName();
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = GetEmpByMachineName();
                MainModel.UpdatedOn = DateTime.Now;
            }

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            //MainModel.AttendanceEntryMethodTypeBack = AttendanceEntryMethodType != null && AttendanceEntryMethodType != "0" && AttendanceEntryMethodType != "undefined" ? AttendanceEntryMethodType : "";
            MainModel.DashboardTypeBack = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "";
            MainModel.DashAttendStatusBack = DashAttendStatus != null && DashAttendStatus != "0" && DashAttendStatus != "undefined" ? DashAttendStatus : "";
            MainModel.DashDepartmentBack = DashDepartment != null && DashDepartment != "0" && DashDepartment != "undefined" ? DashDepartment : "";
            MainModel.DashCategoryBack = DashCategory != null && DashCategory != "0" && DashCategory != "undefined" ? DashCategory : "";
            MainModel.DashDesignationBack = DashDesignation != null && DashDesignation != "0" && DashDesignation != "undefined" ? DashDesignation : "";
            MainModel.DashEmployeeBack = DashEmployee != null && DashEmployee != "0" && DashEmployee != "undefined" ? DashEmployee : "";
            MainModel.GlobalSearchBack = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";

            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("GateAttendance", serializedGateAttendance);

            return View(MainModel);
        }
        [HttpPost]
        public async Task<IActionResult> GateAttendance(GateAttendanceModel model)
        {
            try
            {
                bool isError = true;
                DataSet DS = new();
                DataTable ItemDetailDT = null;
                ResponseResult Result = new();
                Dictionary<string, string> ErrList = new();
                string modePOA = "data";
                var stat = new MemoryCacheStatistics();

                //var selectedRows = model.GateAttDetailsList.Where(e => selectedEmpIds.Contains(e.EmpId)).ToList();
                // 1. Get GateAttendance
                string GateAttendanceJson = HttpContext.Session.GetString("GateAttendance");
                GateAttendanceModel MainModel = string.IsNullOrEmpty(GateAttendanceJson)
                    ? new GateAttendanceModel()
                    : JsonConvert.DeserializeObject<GateAttendanceModel>(GateAttendanceJson);

                SetEmptyOrNullFields<GateAttendanceModel>(model, MainModel);
                string serializedGateAttendance = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("GateAttendance", serializedGateAttendance);


                ModelState.Clear();

                if (model.GateAttDetailsList != null && model.GateAttDetailsList.Count > 0)
                {
                    DS = GetItemDetailTable(model, model.Mode, model.GateAttEntryId, model.GateAttYearCode);
                    ItemDetailDT = DS.Tables[0];
                    MainModel.GateAttDetailsList = model.GateAttDetailsList;

                    isError = false;
                    if (model.GateAttDetailsList != null && model.GateAttDetailsList.Any())
                    {
                        var hasDupes = model.GateAttDetailsList.GroupBy(x => new { x.EmpId, x.EmpCategoryId })
                       .Where(x => x.Skip(1).Any()).Any();
                        if (hasDupes)
                        {
                            isError = true;
                            ErrList.Add("GateAttDetailsList", "ItemDetails can not be Duplicate...!");
                        }
                    }
                }
                else
                {
                    ErrList.Add("GateAttDetailsList", "Select At least one employee attandance..!");
                }
                if (model != null && model.CreatedBy == 0)
                {
                    ErrList.Add("CreatedBy", "Please Select Created By From List..!");
                }

                if (!isError)
                {
                    if (ItemDetailDT.Rows.Count > 0)
                    {
                        if (model.Mode == "U")
                        {
                            model.Mode = "UPDATE";
                        }
                        else if (model.Mode == "C")
                        {
                            model.Mode = "COPY";
                        }
                        else
                        {
                            model.Mode = "INSERT";
                        }
                        //model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";

                        model.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model.FinToDate = HttpContext.Session.GetString("ToDate");
                        model.GateAttYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        model.Branch = HttpContext.Session.GetString("Branch");
                        model.EntryByMachineName = HttpContext.Session.GetString("EmpName");
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                        string serializedtempGateAttendance = JsonConvert.SerializeObject(model);
                        HttpContext.Session.SetString("tempGateAttendance", serializedtempGateAttendance); // this seted before NFromDateChange because It will sets up at on screen attandance date as per earlier date
                        if (!string.IsNullOrEmpty(model.DayOrMonthType) && string.Equals(model.DayOrMonthType, "monthly", StringComparison.OrdinalIgnoreCase))
                        {
                            DateTime fromDate = new DateTime(model.GateAttYearCode, 1, 1);
                            DateTime toDate = new DateTime(model.GateAttYearCode, 12, 31);
                            model.NFromDate = CommonFunc.ParseFormattedDate(fromDate.ToString("dd/MM/yyyy"));
                            model.NToDate = CommonFunc.ParseFormattedDate(toDate.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            model.NFromDate = model.strEmpAttDate;
                            model.NToDate = model.strEmpAttDate;
                        }
                        model.EntryByMachineName = HttpContext.Session.GetString("ClientMachineName");
                        model.IPAddress = HttpContext.Session.GetString("ClientIP");
                        Result = await IGateAttendance.SaveGateAtt(model, ItemDetailDT);
                    }
                    string message = string.Empty;
                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";

                            HttpContext.Session.Remove("GateAttendance");
                            message = "Success!";
                        }
                        else if (Result.StatusText == "Inserted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("GateAttendance");
                            message = "Attendance inserted successfully!";
                        }
                        else if (Result.StatusText == "Updated Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("GateAttendance");
                            //return RedirectToAction(nameof(GateAttendance));
                            message = "Attendance updated successfully!";
                            return Json(new { isSuccess = true, message = message });
                        }
                        else if (Result.StatusText == "Deleted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["410"] = "410";
                            HttpContext.Session.Remove("GateAttendance");
                            message = "Deleted Successfully";
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.ToString(); //.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                TempData["2627"] = "2627";
                                _Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModels(model);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.GateAttYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.Branch = HttpContext.Session.GetString("Branch");
                                model2.CreatedByName = HttpContext.Session.GetString("EmpName");
                                model2.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model2.UpdatedByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                //return View("GateAttendance", model);
                                message = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                return Json(new { isSuccess = false, message = message });
                            }
                            else
                            {
                                ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                TempData["500"] = "500";
                                model = await BindModels(model);
                                //model.FinFromDate = HttpContext.Session.GetString("FromDate");
                                //model.FinToDate = HttpContext.Session.GetString("ToDate");
                                model.GateAttYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                //model.Branch = HttpContext.Session.GetString("Branch");
                                //model.PreparedByName = HttpContext.Session.GetString("EmpName");
                                //model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                //return View("GateAttendance", model);
                                message = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                return Json(new { isSuccess = false, message = message });
                            }
                        }
                        else
                        {
                            model = await BindModels(model);
                            //model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                            //model.FinFromDate = HttpContext.Session.GetString("FromDate");
                            //model.FinToDate = HttpContext.Session.GetString("ToDate");
                            //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            //model.Branch = HttpContext.Session.GetString("Branch");
                            //model.PreparedByName = HttpContext.Session.GetString("EmpName");
                            //model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            if (Result.StatusText.Contains("success") && (Result.StatusCode == HttpStatusCode.OK || Result.StatusCode == HttpStatusCode.Accepted))
                            {
                                ViewBag.isSuccess = true;
                                TempData["202"] = "202";
                                HttpContext.Session.Remove("GateAttendance");
                                //return RedirectToAction(nameof(GateAttendance));
                                message = Result.StatusText;
                                return Json(new { isSuccess = true, message = message });
                            }
                            else
                            {
                                TempData["ErrorMessage"] = Result.StatusText;
                                HttpContext.Session.Remove("GateAttendance");
                                //return View("GateAttendance", model);
                                message = Result.StatusText;
                                return Json(new { isSuccess = false, message = message });
                            }
                        }
                    }
                    var model1 = await BindModels(model);
                    //model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                    //model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                    //model1.FinToDate = HttpContext.Session.GetString("ToDate");
                    //model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    //model1.Branch = HttpContext.Session.GetString("Branch");
                    //model1.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    //model1.PreparedByName = HttpContext.Session.GetString("EmpName");
                    //model1.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //model1.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    //return RedirectToAction(nameof(GateAttendance));
                    message = (!string.IsNullOrEmpty(message)) ? message : Result.StatusCode + "Occurred while saving data";
                    return Json(new { isSuccess = true, message = message });
                }
                else
                {
                    model = await BindModels(model);
                    foreach (KeyValuePair<string, string> Err in ErrList)
                    {
                        ModelState.AddModelError(Err.Key, Err.Value);
                    }
                    var message = string.Join(", ", ErrList?.Select(a => a.Key + ":- " + a.Value).ToList());
                    return Json(new { isSuccess = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                _Logger.LogError("\n \n" + ex, ex.Message, model);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                ViewBag.ResponseResult = ResponseResult;
                //return View("Error", ResponseResult);
                var message = "Occurred while saving data" + ex.Message;
                return Json(new { isSuccess = true, message = message });
            }
            model = await BindModels(model);
            //return View("GateAttendance", model);
            return Json(new { isSuccess = true, message = string.Empty });
        }
        public static void SetEmptyOrNullFields<T>(T target, T source)
        {
            if (target == null || source == null) return;

            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var excludedProps = new HashSet<string> { "AttendanceType", "AttendanceEntryMethodType", "DayOrMonthType", "strEmpAttDate", "strEmpAttMonth", "GateAttEntryDay", "GateAttEntryOffDay", "EmployeeCode", "EmpId", "CategoryCode", "EmpCategoryId" };
            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;
                if (excludedProps.Contains(prop.Name))
                    continue; // skip these fields completely  

                var targetValue = prop.GetValue(target);
                var sourceValue = prop.GetValue(source);

                // If property type is class (not string), handle recursively
                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                {
                    if (targetValue == null && sourceValue != null)
                    {
                        prop.SetValue(target, sourceValue);
                    }
                    else if (targetValue != null && sourceValue != null)
                    {
                        // recursion for inner model
                        SetEmptyOrNullFields(targetValue, sourceValue);
                    }
                }
                else
                {
                    bool isDefault = targetValue == null ||
                                     (prop.PropertyType.IsValueType && targetValue.Equals(Activator.CreateInstance(prop.PropertyType))) ||
                                     (prop.PropertyType == typeof(string) && string.IsNullOrEmpty((string)targetValue));

                    if (isDefault && sourceValue != null)
                    {
                        prop.SetValue(target, sourceValue);
                    }
                }
            }
        }
        public async Task<GateAttendanceModel> BindModels(GateAttendanceModel model)
        {
            CommonFunc.LogException<GateAttendanceModel>.LogInfo(_Logger, "********** Gate Attendance BindModels *************");

            _Logger.LogInformation("********** Binding Model *************");
            var oDataSet = new DataSet();
            var SqlParams = new List<KeyValuePair<string, string>>();
            model.DeptList = new List<TextValue>();
            model.DesignationList = new List<TextValue>();
            model.EmployeeList = new List<TextValue>();
            model.ShiftList = new List<TextValue>();
            model.CategoryList = new List<TextValue>();

            model.DeptList = await IDataLogic.GetDropDownList("FillDepartment", "HRSPGateAttendanceMainDetail");
            model.DesignationList = await IDataLogic.GetDropDownList("FillDesignation", "HRSPGateAttendanceMainDetail");
            model.EmployeeList = await IDataLogic.GetDropDownList("FillEmployee", "HRSPGateAttendanceMainDetail");
            model.CategoryList = await IDataLogic.GetDropDownList("FillCategory", "HRSPGateAttendanceMainDetail");
            //foreach (var emp in model.EmployeeList)
            //{
            //    if (!string.IsNullOrWhiteSpace(emp.Text) && emp.Text.StartsWith("--->"))
            //    {
            //        emp.Text = emp.Value;   // updates the list item itself
            //    }
            //}
            return model;
        }
        public async Task<IActionResult> LoadAttendanceGrid(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode, int EmpCatId, int EmpId, bool IsManual = false)
        {
            // Get list from DB based on date
            GateAttendanceModel model = new GateAttendanceModel();
            model = await IGateAttendance.GetManualAttendance(DayOrMonthType, Attdate, AttMonth, YearCode, EmpCatId, EmpId, IsManual).ConfigureAwait(false);
            //model.ShiftList = new List<TextValue>();
            if (string.Equals(DayOrMonthType, "monthly", StringComparison.OrdinalIgnoreCase))
            {
                Attdate = new DateTime(YearCode, AttMonth, 1);
                model.intEmpAttMonth = AttMonth;
            }
            else
            {
                if (Attdate != null)
                {
                    model.EmpAttDate = Attdate;
                    model.strEmpAttDate = Attdate.ToString("dd/MM/yyyy");
                }
            }
            model.HolidayList = GetHolidayList(EmpCatId, Attdate, YearCode)?.HolidayList ?? new List<GateAttendanceHolidayModel>();
            ViewBag.DeptList = await IDataLogic.GetDropDownList("FillDepartment", "HRSPGateAttendanceMainDetail");
            ViewBag.DesigList = await IDataLogic.GetDropDownList("FillDesignation", "HRSPGateAttendanceMainDetail");
            //ViewBag.ShiftList = await IDataLogic.GetDropDownList("FillShift", "HRSPGateAttendanceMainDetail");
            ViewBag.EmployeeList = await IDataLogic.GetDropDownList("FillEmployee", "HRSPGateAttendanceMainDetail");

            model.DayOrMonthType = DayOrMonthType;
            //ViewBag.DeptList = model.DeptList;
            //ViewBag.DesigList = model.DesignationList;
            //ViewBag.EmployeeList = model.EmployeeList;

            string GateAttendanceJson = HttpContext.Session.GetString("GateAttendance");
            GateAttendanceModel MainModel = string.IsNullOrEmpty(GateAttendanceJson)
                ? new GateAttendanceModel()
                : JsonConvert.DeserializeObject<GateAttendanceModel>(GateAttendanceJson);
            SetEmptyOrNullFields<GateAttendanceModel>(MainModel, model);
            if (string.Equals(DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
            {
                if (Attdate != null)
                {
                    model.EmpAttDate = Attdate;
                    model.strEmpAttDate = Attdate.ToString("dd/MM/yyyy");
                    DateTime? currentDate = model.EmpAttDate;
                    string currentDay = currentDate?.DayOfWeek.ToString() ?? "";

                    var holiday = model.HolidayList?.FirstOrDefault(h => h.HolidayYear == model.GateAttYearCode && h.HolidayEffFrom >= currentDate && h.HolidayEffTill <= currentDate);

                    bool isWeekoff = model.HolidayList?.Any(a => a.HolidayEffFrom == null && a.HolidayEffTill == null && a.DayName.Equals(currentDay, StringComparison.OrdinalIgnoreCase)) ?? false;

                    bool isHoliday = !isWeekoff && holiday != null;

                    string cssClass = isWeekoff ? "weekoff-time" : (isHoliday ? "holiday-time" : "");

                    bool allowEdit = (holiday != null && isHoliday &&
                        string.Equals(holiday?.AllowedCompOff, "YES", StringComparison.OrdinalIgnoreCase))
                        || !isHoliday;

                    if (isWeekoff)
                        allowEdit = false;

                    // Assign results to model
                    ViewBag.IsWeekoff = isWeekoff;
                    ViewBag.IsHoliday = isHoliday;
                    ViewBag.AllowEdit = allowEdit;
                    ViewBag.CssClass = cssClass;
                }
            }
            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("GateAttendance", serializedGateAttendance);
            return PartialView("_GateManualAttendance", model);
        }
        [HttpPost]
        public JsonResult CheckHolidayWeekoff(DateTime date, int empCatId, int yearCode)
        {
            var holidayData = GetHolidayList(empCatId, date, yearCode);
            bool isWeekoff = holidayData?.HolidayList?.Any(a => a.HolidayEffFrom == null && a.HolidayEffTill == null && a.DayName.Equals(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase))  ?? false;
            bool isHoliday = holidayData?.HolidayList?.Any(a => a.HolidayEffFrom != null && a.HolidayEffTill != null && a.HolidayEffFrom <= date && a.HolidayEffTill >= date) ?? false;

            return Json(new
            {
                isHoliday = isHoliday,
                isWeekoff = isWeekoff,
                HolidayName = holidayData?.HolidayList?.FirstOrDefault(h => h.HolidayEffFrom <= date  && h.HolidayEffTill >= date)?.HolidayName ?? ""
            });
        }
        private static DataSet GetItemDetailTable(GateAttendanceModel itemDetailList, string Mode, int? EntryID, int? YearCode)
        {
            DataSet DS = new();
            DataTable Table = new();
            if (itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
            {
                Table.Columns.Add("GateAttEntryId", typeof(int));
                Table.Columns.Add("GateAttYearCode", typeof(int));
                Table.Columns.Add("EmpAttYear", typeof(int));
                Table.Columns.Add("CardOrBiometricId", typeof(string));
                if (itemDetailList.DayHeaders == null || itemDetailList.DayHeaders.Count == 0 || itemDetailList.DayHeaders.Count != 4)
                {
                    itemDetailList.DayHeaders = new List<string>();
                    var intday = itemDetailList.strEmpAttDate != null ? CommonFunc.ParseDate(itemDetailList.strEmpAttDate).Day : 1;
                    itemDetailList.DayHeaders.Add($"AttendStatus{intday}");
                    itemDetailList.DayHeaders.Add($"EmpId");
                    itemDetailList.DayHeaders.Add($"AttInTime{intday}");
                    itemDetailList.DayHeaders.Add($"AttOutTime{intday}");
                    itemDetailList.DayHeaders.Add($"TotalNoOfHours{intday}");
                }
                Table.Columns.Add("AttendStatus", typeof(string));
                Table.Columns.Add("EmpId", typeof(int));
                Table.Columns.Add("AttInTime", typeof(DateTime));
                Table.Columns.Add("AttOutTime", typeof(DateTime));
                Table.Columns.Add("TotalNoOfHours", typeof(decimal));
                Table.Columns.Add("LateEntry", typeof(string));
                Table.Columns.Add("EarlyExit", typeof(string));
                Table.Columns.Add("LeaveTypeId", typeof(int));
                Table.Columns.Add("AttShiftId", typeof(int));
                Table.Columns.Add("ApproveByDept", typeof(string));
                Table.Columns.Add("DeptApprovalDate", typeof(DateTime));
                Table.Columns.Add("DeptApprovalEmpId", typeof(int));
                Table.Columns.Add("ApproveByHR", typeof(string));
                Table.Columns.Add("HRApprovalDate", typeof(DateTime));
                Table.Columns.Add("HRApprovalEmpId", typeof(int));
                Table.Columns.Add("CC", typeof(string));
                Table.Columns.Add("CategoryCode", typeof(int));
                Table.Columns.Add("ActualEmpShift", typeof(int));
                Table.Columns.Add("ActualShiftInTime", typeof(DateTime));
                Table.Columns.Add("ActualShiftOutTime", typeof(DateTime));
                Table.Columns.Add("Overtime", typeof(decimal));
                Table.Columns.Add("DesigId", typeof(int));
                Table.Columns.Add("DeptId", typeof(int));
            }
            else
            {
                int daysInMonth = 1;
                Table.Columns.Add("GateAttEntryId", typeof(int));
                Table.Columns.Add("GateAttYearCode", typeof(int));
                Table.Columns.Add("EmpId", typeof(int));
                Table.Columns.Add("EmpAttYear", typeof(int));
                if (itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "Monthly", StringComparison.OrdinalIgnoreCase))
                {
                    if (YearCode > 0 && itemDetailList.intEmpAttMonth > 0)
                    {
                        daysInMonth = DateTime.DaysInMonth(Convert.ToInt32(YearCode), Convert.ToInt32(itemDetailList.intEmpAttMonth));
                        daysInMonth = 31;
                    }
                    if (itemDetailList.DayHeaders == null || itemDetailList.DayHeaders.Count == 0 || itemDetailList.DayHeaders.Count != (31 * 4))
                    {
                        itemDetailList.DayHeaders = new List<string>();
                        itemDetailList.strEmpAttMonth = new DateTime(Convert.ToInt32(YearCode), Convert.ToInt32(itemDetailList.intEmpAttMonth), 1).ToString("MMM");
                        for (int d = 1; d <= daysInMonth; d++)
                        {
                            itemDetailList.DayHeaders.Add($"AttendStatus{d}");
                            itemDetailList.DayHeaders.Add($"AttInTime{d}");
                            itemDetailList.DayHeaders.Add($"AttOutTime{d}");
                            itemDetailList.DayHeaders.Add($"TotalNoOfHours{d}");
                        }
                    }
                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        Table.Columns.Add($"AttendStatus{d}", typeof(string));
                        Table.Columns.Add($"AttInTime{d}", typeof(DateTime));
                        Table.Columns.Add($"AttOutTime{d}", typeof(DateTime));
                        Table.Columns.Add($"TotalNoOfHours{d}", typeof(decimal));
                    }
                }
                Table.Columns.Add("AttShiftId", typeof(int));
                Table.Columns.Add("DesigId", typeof(int));
                Table.Columns.Add("DeptId", typeof(int));
                Table.Columns.Add("CC", typeof(string));
                Table.Columns.Add("CategoryCode", typeof(int));
                Table.Columns.Add("EmpAttTime", typeof(DateTime));
                Table.Columns.Add("ActualEmpShift", typeof(int));
                Table.Columns.Add("ActualEmpInTime", typeof(DateTime));
                Table.Columns.Add("ActualEmpOutTime", typeof(DateTime));
                Table.Columns.Add("OvertTime", typeof(decimal));
            }
            foreach (GateAttendanceModel Item in itemDetailList?.GateAttDetailsList ?? new List<GateAttendanceModel>())
            {
                var currentDt = CommonFunc.ParseSafeDate(DateTime.Now.ToString());
                List<object> rowValues = new List<object>();
                {
                    rowValues.Add((itemDetailList.GateAttEntryId > 0 ? itemDetailList.GateAttEntryId : EntryID) ?? 0);
                    rowValues.Add((itemDetailList.GateAttYearCode > 0 ? itemDetailList.GateAttYearCode : YearCode) ?? 0);
                    if (itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
                    {
                        rowValues.Add(Item.EmpAttYear ?? Item.GateAttYearCode);
                        rowValues.Add(Item.CardOrBiometricId ?? string.Empty);
                    }
                    else
                    {
                        rowValues.Add(Item.EmpId);
                        rowValues.Add(Item.EmpAttYear ?? Item.GateAttYearCode);
                    }
                }

                if (itemDetailList.DayHeaders != null)
                {
                    foreach (var header in itemDetailList.DayHeaders)
                    {
                        if (!Item.Attendance.ContainsKey(header) && header.Contains("AttendStatus"))
                        { rowValues.Add(string.Empty); }
                        else if (Item.EmpId > 0 && header.Contains("EmpId") && itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
                        { rowValues.Add(Item.EmpId); }
                        else if (!Item.Attendance.ContainsKey(header) && header.Contains("TotalNoOfHours"))
                        { rowValues.Add(0); } //Math.Round(Item.Attendance[header], 2, MidpointRounding.AwayFromZero)
                        else if (Item.Attendance != null && Item.Attendance.ContainsKey(header) && (header.Contains("InTime") || header.Contains("OutTime") || header.Contains("AttInTime") || header.Contains("AttOutTime") || header.Contains("AttendStatus") || header.Contains("TotalNoOfHour")))
                        {
                            if (header.Contains("AttendStatus") || header.Contains("TotalNoOfHour"))
                            {
                                rowValues.Add(Item.Attendance[header] ?? string.Empty);
                            }
                            else if (header.Contains("TotalNoOfHour"))
                            {
                                rowValues.Add(Item.Attendance[header] != null ? Convert.ToDecimal(Item.Attendance[header]) : 0);
                            }
                            else
                            {
                                var time = CommonFunc.ParseSafeTime(Item.Attendance[header]);

                                int year = itemDetailList.GateAttYearCode;
                                int month = itemDetailList.intEmpAttMonth != null && itemDetailList.intEmpAttMonth > 0 ? Convert.ToInt32(itemDetailList.intEmpAttMonth) : 0;
                                int day = 1;

                                if (string.Equals(itemDetailList.DayOrMonthType, "Monthly", StringComparison.OrdinalIgnoreCase))
                                {
                                    var dayPart = new string(header.Where(char.IsDigit).ToArray());
                                    if (int.TryParse(dayPart, out int parsedDay))
                                        day = parsedDay;
                                }
                                else
                                {
                                    day = itemDetailList.strEmpAttDate != null ? CommonFunc.ParseDate(itemDetailList.strEmpAttDate).Day : 1;
                                    month = itemDetailList.strEmpAttDate != null ? CommonFunc.ParseDate(itemDetailList.strEmpAttDate).Month : 1;
                                }
                                if (time == null)
                                {
                                    rowValues.Add((object?)null ?? DBNull.Value);
                                    continue;
                                }
                                else
                                {
                                    var timeOnly = (TimeOnly)time;
                                    var combinedDateTime = new DateTime(year, month, day, timeOnly.Hour, timeOnly.Minute, timeOnly.Second);
                                    rowValues.Add(combinedDateTime);
                                }
                            }
                        }
                        else if (header.Contains("InTime") || header.Contains("OutTime"))
                        {
                            rowValues.Add((object?)null ?? DBNull.Value);
                        }
                        else
                        { rowValues.Add(string.Empty); }
                    }
                }

                if (itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "daily", StringComparison.OrdinalIgnoreCase))
                {
                    rowValues.Add(Item.LateEntry ?? string.Empty);
                    rowValues.Add(Item.EarlyExit ?? string.Empty);
                    rowValues.Add(Item.LeaveTypeId ?? 0);
                    rowValues.Add(Item.ActualEmpShiftId ?? 0);
                    rowValues.Add(Item.ApproveByDept);
                    rowValues.Add((Item.DeptApprovaldate != null) ? CommonFunc.ParseSafeDate(Item.DeptApprovaldate.ToString() ?? currentDt.ToString("MM/dd/yyyy")) : null);
                    rowValues.Add(Item.DeptApprovalEmpId ?? 0);
                    rowValues.Add(Item.ApproveByHR);
                    rowValues.Add((Item.HRApprovaldate != null) ? CommonFunc.ParseSafeDate(Item.HRApprovaldate.ToString() ?? currentDt.ToString("MM/dd/yyyy")) : null);
                    rowValues.Add(Item.HRApprovalEmpId ?? 0);
                    rowValues.Add(!string.IsNullOrEmpty(itemDetailList.CC) ? itemDetailList.CC : (!string.IsNullOrEmpty(itemDetailList.Branch) ? itemDetailList.Branch : string.Empty));
                    rowValues.Add(itemDetailList.CategoryId != null ? Convert.ToInt32(itemDetailList.CategoryId) : 1);
                    rowValues.Add(itemDetailList.ActualEmpShiftId ?? 0);
                    rowValues.Add(currentDt);
                    rowValues.Add(currentDt);
                    rowValues.Add(0);
                    rowValues.Add(Item.DesignationEntryId ?? 0);
                    rowValues.Add(Item.DeptId ?? 0);
                }
                else
                {
                    rowValues.Add(Item.ActualEmpShiftId ?? 0);
                    rowValues.Add(Item.DesignationEntryId ?? 0);
                    rowValues.Add(Item.DeptId ?? 0);
                    rowValues.Add(!string.IsNullOrEmpty(itemDetailList.CC) ? itemDetailList.CC : (!string.IsNullOrEmpty(itemDetailList.Branch) ? itemDetailList.Branch : string.Empty));
                    rowValues.Add(itemDetailList.EmpCategoryId);
                    rowValues.Add(currentDt);
                    rowValues.Add(itemDetailList.ActualEmpShiftId ?? 0);
                    rowValues.Add(currentDt);
                    rowValues.Add(currentDt);
                    rowValues.Add(0);
                }
                Table.Rows.Add(rowValues.ToArray());
            }

            DS.Tables.Add(Table);
            return DS;
        }
        public GateAttendanceModel GetHolidayList(int EmpCatId, DateTime Attdate, int YearCode)
        {
            GateAttendanceModel model = new GateAttendanceModel();
            model = IGateAttendance.GetHolidayList(EmpCatId, Attdate, YearCode);
            return model;
        }
        public string GetEmpByMachineName()
        {
            try
            {
                string empname = string.Empty;
                empname = HttpContext.Session.GetString("EmpName").ToString();
                if (string.IsNullOrEmpty(empname)) { empname = Environment.UserDomainName; }
                return empname;
            }
            catch
            {
                return "";
            }
        }
        public async Task<IActionResult> DashBoard(string FromDate = "", string ToDate = "", string DashboardType = "", string DashDepartment = "", string DashCategory = "", string DashDesignation = "", string DashEmployee = "", string DashAttendStatus = "", string Searchbox = "", string Flag = "True")
        {
            HttpContext.Session.Remove("GateAttendance");
            var _List = new List<TextValue>();
            var MainModel = await IGateAttendance.GetDashBoardData(null);

            PBDashBoard model = new PBDashBoard();
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime today = DateTime.Now;
            var commonparams = new Dictionary<string, object>()
        {
            { "@fromdate", firstDayOfMonth },
            { "@Todate", today }
        };
            MainModel = await BindDashboardList(MainModel, commonparams);
            MainModel.FromDate = new DateTime(DateTime.Today.Year, 1, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 12, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in December this year
            if (Flag != "True")
            {
                MainModel.FromDate = FromDate;
                MainModel.ToDate = ToDate;
                MainModel.DashboardType = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "0";
                MainModel.DashAttendStatus = DashAttendStatus != null && DashAttendStatus != "0" && DashAttendStatus != "undefined" ? DashAttendStatus : "0";
                MainModel.DashDepartment = DashDepartment != null && DashDepartment != "0" && DashDepartment != "undefined" ? DashDepartment : "0";
                MainModel.DashCategory = DashCategory != null && DashCategory != "0" && DashCategory != "undefined" ? DashCategory : "0";
                MainModel.DashDesignation = DashDesignation != null && DashDesignation != "0" && DashDesignation != "undefined" ? DashDesignation : "0";
                MainModel.DashEmployee = DashEmployee != null && DashEmployee != "0" && DashEmployee != "undefined" ? DashEmployee : "0";
                MainModel.Searchbox = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";
            }
            return View(MainModel);
        }
        public async Task<GateAttDashBoard> BindDashboardList(GateAttDashBoard MainModel, Dictionary<string, object> commonparams)
        {
            Dictionary<string, object> Build(string flag)
            {
                var p = new Dictionary<string, object>() { { "@flag", flag } };

                foreach (var kv in commonparams)
                    if (kv.Key != "@flag") p[kv.Key] = kv.Value;

                return p;
            }
            var dashdep = Build("DashboardFillDepartment");
            var dashcat = Build("DashboardFillCategory");
            var dashdesg = Build("DashboardFillDesignation");
            var dashemp = Build("DashboardFillEmployee");
            MainModel.DashDepartmentList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPGateAttendanceMainDetail", dashdep, false, false);
            MainModel.DashCategoryList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPGateAttendanceMainDetail", dashcat, false, true);
            MainModel.DashDesignationList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPGateAttendanceMainDetail", dashdesg, false, false);
            MainModel.DashEmployeeList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPGateAttendanceMainDetail", dashemp, false, false);

            return MainModel;
        }
        public async Task<IActionResult> GetSearchData(GateAttDashBoard model, int pageNumber = 1, int pageSize = 25, string SearchBox = "")
        {
            model.Mode = "SEARCH";
            model.DashboardType = !string.IsNullOrEmpty(model.DashboardType) ? model.DashboardType : "SUMMARY";
            var dbtype = model.DashboardType;
            model = await IGateAttendance.GetDashBoardData(model);
            model.DashboardType = dbtype;
            var modelList = model?.GateAttDashboard ?? new List<GateAttDashBoard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.GateAttDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<GateAttDashBoard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }
                model.TotalRecords = filteredResults.Count;
                model.GateAttDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set($"KeyGateAttList_{model.DashboardType}", modelList, cacheEntryOptions);
            return PartialView("_DashBoardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "SUMMARY", int pageNumber = 1, int pageSize = 25)
        {
            GateAttDashBoard model = new GateAttDashBoard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_DashBoardGrid", new List<GateAttDashBoard>());
            }
            string cacheKey = $"KeyGateAttList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<GateAttDashBoard> Dashboard) || Dashboard == null)
            {
                return PartialView("_DashBoardGrid", new List<GateAttDashBoard>());
            }

            List<GateAttDashBoard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = Dashboard.ToList();
            }
            else
            {
                filteredResults = Dashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = Dashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.GateAttDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.DashboardType = dashboardType;
            return PartialView("_DashBoardGrid", model);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await IGateAttendance.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryId(int YearCode)
        {
            var JSON = await IGateAttendance.FillEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DeleteByID(int ID, int YC, string PurchVoucherNo, string InvNo = "", bool? IsDetail = false)
        {
            int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string EntryByMachineName = HttpContext.Session.GetString("EmpName");
            string cc = HttpContext.Session.GetString("Branch");
            DateTime EntryDate = DateTime.Today;
            var Result = await IGateAttendance.DeleteByID(ID, YC, "DELETEBYID", EntryBy, EntryByMachineName, cc, EntryDate);

            var rslt = string.Empty;
            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                rslt = "true";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Locked)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
                rslt = "true";
            }
            else if ((Result.StatusText == "Deleted Successfully" || Result.StatusText == "deleted Successfully") && (Result.StatusCode == HttpStatusCode.Accepted || Result.StatusCode == HttpStatusCode.OK))
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                rslt = "true";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
                rslt = "false";
            }
            return Json(new { success = rslt, message = Result.StatusText });

            //return Json(rslt);
            //return RedirectToAction(nameof(DashBoard));
        }
    }
}
