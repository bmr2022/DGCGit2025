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
        public async Task<IActionResult> GateAttendance(int ID, int YearCode, string Mode, string? AttendanceEntryMethodType, string FromDate = "", string ToDate = "", string DashboardType = "", string Searchbox = "")
        {
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
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.GateAttYearCode = YearCode;
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
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
            //MainModel.DashboardTypeBack = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "";
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
                        if(!string.IsNullOrEmpty(model.DayOrMonthType) && string.Equals(model.DayOrMonthType, "monthly", StringComparison.OrdinalIgnoreCase))
                        {
                            DateTime fromDate = new DateTime(MainModel.GateAttYearCode, 1, 1);
                            DateTime toDate = new DateTime(MainModel.GateAttYearCode, 12, 31);
                            MainModel.NFromDate = CommonFunc.ParseFormattedDate(fromDate.ToString("dd/MM/yyyy"));
                            MainModel.NToDate = CommonFunc.ParseFormattedDate(toDate.ToString("dd/MM/yyyy"));    
                        }
                        else 
                        {
                            model.NFromDate = model.strEmpAttDate;
                            model.NToDate = model.strEmpAttDate;
                        }
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

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

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
        public async Task<IActionResult> LoadAttendanceGrid(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode, int EmpCatId)
        {
            // Get list from DB based on date
            GateAttendanceModel model = new GateAttendanceModel();
            model = await IGateAttendance.GetManualAttendance(DayOrMonthType, Attdate, AttMonth, YearCode).ConfigureAwait(false);
            //model.ShiftList = new List<TextValue>();
            if (string.Equals(DayOrMonthType, "monthly", StringComparison.OrdinalIgnoreCase))
            {
                Attdate = new DateTime(YearCode, AttMonth, 1);
                model.intEmpAttMonth = AttMonth;
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
            
            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("GateAttendance", serializedGateAttendance);
            return PartialView("_GateManualAttendance", model);
        }
        private static DataSet GetItemDetailTable(GateAttendanceModel itemDetailList, string Mode, int? EntryID, int? YearCode)
        {
            DataSet DS = new();
            DataTable Table = new();

            int daysInMonth = 1;
            Table.Columns.Add("GateAttEntryId", typeof(int));
            Table.Columns.Add("GateAttYearCode", typeof(int));
            Table.Columns.Add("EmpId", typeof(int));
            Table.Columns.Add("EmpAttYear", typeof(int));
            if (itemDetailList != null && string.Equals(itemDetailList.DayOrMonthType, "Monthly", StringComparison.OrdinalIgnoreCase))
            {
                if (YearCode > 0 && itemDetailList.intEmpAttMonth > 0) {
                    daysInMonth = DateTime.DaysInMonth(Convert.ToInt32(YearCode), Convert.ToInt32(itemDetailList.intEmpAttMonth));
                    daysInMonth = 31;
                }
                if (itemDetailList.DayHeaders == null || itemDetailList.DayHeaders.Count == 0 || itemDetailList.DayHeaders.Count != (31*4))
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
            else
            {
                itemDetailList.DayHeaders.Add($"AttendStatus");
                itemDetailList.DayHeaders.Add($"AttInTime");
                itemDetailList.DayHeaders.Add($"AttOutTime");
                itemDetailList.DayHeaders.Add($"TotalNoOfHours");
                Table.Columns.Add($"AttendStatus", typeof(string));
                Table.Columns.Add($"AttInTime", typeof(DateTime));
                Table.Columns.Add($"AttOutTime", typeof(DateTime));
                Table.Columns.Add($"TotalNoOfHours", typeof(decimal));
            }
            Table.Columns.Add("AttShiftId", typeof(int));
            Table.Columns.Add("CC", typeof(string));
            Table.Columns.Add("CategoryCode", typeof(int));
            Table.Columns.Add("EmpAttTime", typeof(DateTime));
            Table.Columns.Add("ActualEmpShift", typeof(int));
            Table.Columns.Add("ActualEmpInTime", typeof(DateTime));
            Table.Columns.Add("ActualEmpOutTime", typeof(DateTime));
            Table.Columns.Add("OvertTime", typeof(decimal));

            foreach (GateAttendanceModel Item in itemDetailList?.GateAttDetailsList ?? new List<GateAttendanceModel>())
            {
                var currentDt = CommonFunc.ParseSafeDate(DateTime.Now.ToString());
                List<object> rowValues = new List<object>
                {
                    (itemDetailList.GateAttEntryId > 0 ? itemDetailList.GateAttEntryId : EntryID) ?? 0,
                    (itemDetailList.GateAttYearCode > 0 ? itemDetailList.GateAttYearCode : YearCode) ?? 0,
                    Item.EmpId,
                    Item.EmpAttYear ?? Item.GateAttYearCode
                };

                if (itemDetailList.DayHeaders != null)
                {
                    foreach (var header in itemDetailList.DayHeaders)
                    {
                        if (!Item.Attendance.ContainsKey(header) && header.Contains("AttendStatus"))
                        { rowValues.Add(string.Empty); }
                        else if (!Item.Attendance.ContainsKey(header) && header.Contains("TotalNoOfHours"))
                        { rowValues.Add(0); } //Math.Round(Item.Attendance[header], 2, MidpointRounding.AwayFromZero)
                        else if (Item.Attendance != null && Item.Attendance.ContainsKey(header) && (header.Contains("InTime") || header.Contains("OutTime")))
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
                                day = itemDetailList.EmpAttDate != null  ? CommonFunc.ParseDate(Convert.ToDateTime(itemDetailList.EmpAttDate).Date.ToString()).Day : 1;
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
                        else if (header.Contains("InTime") || header.Contains("OutTime"))
                        {
                            rowValues.Add((object?)null ?? DBNull.Value);
                        }
                        else
                        { rowValues.Add(string.Empty); }
                    }
                }

                rowValues.Add(Item.ActualEmpShiftId ?? 0);
                rowValues.Add(itemDetailList.CC ?? string.Empty);
                rowValues.Add(itemDetailList.EmpCategoryId);
                rowValues.Add(currentDt);
                rowValues.Add(itemDetailList.ActualEmpShiftId ?? 0);
                rowValues.Add(currentDt);
                rowValues.Add(currentDt);
                rowValues.Add(0);

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
        public async Task<IActionResult> DashBoard()
        {
            HttpContext.Session.Remove("GateAttendance");
            var _List = new List<TextValue>();
            var MainModel = await IGateAttendance.GetDashBoardData(null);
            MainModel.FromDate = new DateTime(DateTime.Today.Year, 1, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 12, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in December this year

            return View(MainModel);
        }
        public async Task<IActionResult> GetSearchData(GateAttDashBoard model, int pageNumber = 1, int pageSize = 25, string SearchBox = "")
        {
            model.Mode = "SEARCH";
            model = await IGateAttendance.GetDashBoardData(model);
            //model.DashboardType = "Summary";
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

            _MemoryCache.Set("KeyGateAttList", modelList, cacheEntryOptions);
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
    }
}
