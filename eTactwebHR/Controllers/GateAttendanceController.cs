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
        public async Task<IActionResult> GateAttendance(int ID, int YearCode, string Mode)
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
                //MainModel = await IGateAttendance.GetViewByID(ID, YearCode, "ViewByID").ConfigureAwait(false);
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

            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("GateAttendance", serializedGateAttendance);

            return View(MainModel);
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
            return PartialView("_GateManualAttendance", model);
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
