using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static FastReport.Utils.HtmlTextRenderer;
using static Grpc.Core.Metadata;

namespace eTactWeb.Controllers
{
    public class CompanyDetailController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICompanyDetail _ICompanyDetail { get; }

        private readonly ILogger<CompanyDetailController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CompanyDetailController(ILogger<CompanyDetailController> logger, IDataLogic iDataLogic, ICompanyDetail iCompanyDetail, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICompanyDetail = iCompanyDetail;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CompanyDetail(int ID, string Mode
            , int MainItemCode,
    int AlternateItemCode, string DispName, string Com_Name, string WebSite, string OfficeAdd1,
    string OfficeAdd2, string PinCode, string Email, string Phone, string Mobile, string StateCode,
    string StateName, string Commodity, string? Start_Date, string? End_Date, string PhoneF,
    string Division, string OrgType, string PANNo, string TDSAccount, string Range, string TDSRange,
    string GSTNO, string PFApplicable, string PFNO, string Registration_No, string VENDOR_CODE,
    string? SoftwareStartDate, string Prefix, string ContactPersonSales, string ContacPersonPurchase,
    string ContactPersonQC, string ContactPersonAccounts, string Country, string LUTNO, string? LUTDATE,
    string UDYMANO
            )
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new CompanyDetailModel();
            //MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            //MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            //MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            //MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            //MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            //MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
            //MainModel.EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _ICompanyDetail.GetViewByID(ID).ConfigureAwait(false);
               
                MainModel.Mode = Mode; 
                MainModel.ID = ID;
                MainModel.EntryID = ID;
                MainModel.DispName = DispName;
                MainModel.Com_Name = Com_Name;
                MainModel.WebSite = WebSite;
                MainModel.OfficeAdd1 = OfficeAdd1;
                MainModel.OfficeAdd2 = OfficeAdd2;
                MainModel.PinCode = PinCode;
                MainModel.Email = Email;
                MainModel.Phone = Phone;
                MainModel.Mobile = Mobile;
                MainModel.StateCode = StateCode;
                MainModel.StateName = StateName;
                MainModel.Commodity = Commodity;
                MainModel.Start_Date = Start_Date;
                MainModel.End_Date = End_Date;
                MainModel.PhoneF = PhoneF;
                MainModel.Division = Division;
                MainModel.OrgType = OrgType;
                MainModel.PANNo = PANNo;
                MainModel.TDSAccount = TDSAccount;
                MainModel.Range = Range;
                MainModel.TDSRange = TDSRange;
                MainModel.GSTNO = GSTNO;
                MainModel.PFApplicable = PFApplicable;
                MainModel.PFNO = PFNO;
                MainModel.Registration_No = Registration_No;
                MainModel.VENDOR_CODE = VENDOR_CODE;
                MainModel.SoftwareStartDate = SoftwareStartDate;
                MainModel.Prefix = Prefix;
                MainModel.ContactPersonSales = ContactPersonSales;
                MainModel.ContacPersonPurchase = ContacPersonPurchase;
                MainModel.ContactPersonQC = ContactPersonQC;
                MainModel.ContactPersonAccounts = ContactPersonAccounts;
                MainModel.Country = Country;
                MainModel.LUTNO = LUTNO;
                MainModel.LUTDATE = LUTDATE;
                MainModel.UDYMANO = UDYMANO;

                if (Mode == "U")
                {
                    //MainModel.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //MainModel.LastUpdatedBy = HttpContext.Session.GetString("EmpName");
                    //MainModel.UpdationDate = DateTime.Now.ToString();
                  

                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
                
            }

            // If not in "Update" mode, bind new model data
            else
            {
                // MainModel = await BindModels(MainModel);
            }

            // When updating the record, make sure to capture updated info

            return View(MainModel); // Pass the model with old data to the view
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> CompanyDetail(CompanyDetailModel model)
        {
            try
            {
                var GIGrid = new DataTable();
               // _MemoryCache.TryGetValue("AlternateItemMasterGrid", out List<AlternateItemMasterGridModel> AlternateItemMasterGrid);


                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
               
                var Result = await _ICompanyDetail.SaveCompanyDetail(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove("CompanyGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(CompanyDetailDashboard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<CompanyDetailController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CompanyDetailDashboard()
        {
            try
            {
                var model = new CompanyDetailModel();
                var result = await _ICompanyDetail.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                //model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");
                //model.ToDate = new DateTime(now.Year + 1, 3, 31).ToString("dd/MM/yyyy");


                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetDetailData()
        {
            //model.Mode = "Search";
            var model = new CompanyDetailModel();
            model = await _ICompanyDetail.GetDashboardDetailData();
            return PartialView("_CompanyDetailDashBoardGrid", model);
        }
    }
}

