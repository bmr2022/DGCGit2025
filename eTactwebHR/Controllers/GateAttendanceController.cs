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
        [HttpPost]
        public async Task<IActionResult> GateAttendance([FromBody] GateAttendanceModel model)
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

                var cc = stat.CurrentEntryCount;
                var pp = stat.CurrentEstimatedSize;

                ModelState.Clear();

                if (MainModel.GateAttDetailsList != null && MainModel.GateAttDetailsList.Count > 0)
                {
                    //DS = GetItemDetailTable(MainModel.ItemDetailGrid, model.Mode, MainModel.EntryID, MainModel.YearCode);
                    //ItemDetailDT = DS.Tables[0];
                    //model.ItemDetailGrid = MainModel.ItemDetailGrid;

                    //isError = false;
                    //if (MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Any())
                    //{
                    //    var hasDupes = MainModel.ItemDetailGrid.GroupBy(x => new { x.ItemCode, x.docTypeId, x.Description })
                    //   .Where(x => x.Skip(1).Any()).Any();
                    //    if (hasDupes)
                    //    {
                    //        isError = true;
                    //        ErrList.Add("ItemDetailGrid", "Document Type + ItemCode + Description In ItemDetails can not be Duplicate...!");
                    //    }
                    //}
                }
                else
                {
                    ErrList.Add("ItemDetailGrid", "Item Details Cannot Be Blank..!");
                }

                if (model.CreatedBy == 0)
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
                        //Result = await IDirectPurchaseBill.SaveDirectPurchaseBILL(ItemDetailDT, TaxDetailDT, TDSDetailDT, model, DrCrDetailDT, AdjDetailDT);
                    }

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("GateAttendance");
                        }
                        else if (Result.StatusText == "Inserted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("GateAttendance");
                        }
                        else if (Result.StatusText == "Updated Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("GateAttendance");
                            return RedirectToAction(nameof(GateAttendance));
                        }
                        else if (Result.StatusText == "Deleted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["410"] = "410";
                            HttpContext.Session.Remove("GateAttendance");
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
                                return View("GateAttendance", model);
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
                                return View("GateAttendance", model);
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
                                return RedirectToAction(nameof(GateAttendance));
                            }
                            else
                            {
                                TempData["ErrorMessage"] = Result.StatusText;
                                HttpContext.Session.Remove("GateAttendance");
                                return View("GateAttendance", model);
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

                    return RedirectToAction(nameof(GateAttendance));

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
                return View("Error", ResponseResult);
            }
            model = await BindModels(model);
            return View("GateAttendance", model);

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
