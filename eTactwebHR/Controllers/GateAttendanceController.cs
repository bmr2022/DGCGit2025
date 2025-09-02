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

            model.DeptList = await IDataLogic.GetDropDownList("FILLDepartment", "HRSPGateAttendanceMainDetail");
            model.DesignationList = await IDataLogic.GetDropDownList("FILLDocumentList", "HRSPGateAttendanceMainDetail");
            return model;
        }
        public async Task<IActionResult> LoadAttendanceGrid(string DayOrMonthType, DateTime Attdate)
        {
            // Get list from DB based on date
            //var list = new List<GateAttendanceModel>();
            var list = await IGateAttendance.GetManualAttendance(DayOrMonthType, Attdate).ConfigureAwait(false);

            return PartialView("_GateAttendanceGrid", list);
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
    }
}
