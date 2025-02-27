using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class SOAmendmentController : Controller
    {
        private static readonly Action<ILogger, string, Exception> _loggerMessage = LoggerMessage.Define<string>(LogLevel.Error, eventId: new EventId(id: 0, name: "ERROR"), formatString: "{Message}");
        private readonly EncryptDecrypt _EncryptDecrypt;
        private readonly IDataLogic _IDataLogic;
        private readonly ISaleOrder _ISaleOrder;
        private readonly ITaxModule _ITaxModule;
        private readonly ILogger _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly LoggerInfo loggerInfo;

        public SOAmendmentController(ILogger<SOAmendmentController> logger, IDataLogic iDataLogic, ISaleOrder iSaleOrder, ITaxModule iTaxModule, IMemoryCache imemoryCache, EncryptDecrypt encryptDecrypt, LoggerInfo loggerInfo)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleOrder = iSaleOrder;
            _ITaxModule = iTaxModule;
            _MemoryCache = imemoryCache;
            _EncryptDecrypt = encryptDecrypt;
            this.loggerInfo = loggerInfo;
            Culture = new CultureInfo("en-GB");
        }

        public CultureInfo Culture { get; set; }

        [HttpGet]
        public async Task<SaleOrderModel> BindModels(SaleOrderModel model)
        {
            if (model == null)
            {
                var CurrentDate = DateTime.Today.ToString("dd/MM/yyyy", Culture);

                model = new SaleOrderModel
                {
                    YearCode = Constants.FinincialYear,
                    EntryID = _IDataLogic.GetEntryID("SaleOrderMain", Constants.FinincialYear, "SOEntryid","Soyearcode"),
                    EntryDate = CurrentDate,
                    WEF = CurrentDate,
                    QDate = CurrentDate,
                    SODate = CurrentDate,
                    AmmEffDate = CurrentDate,
                    SOCloseDate = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy", Culture),
                    EntryTime = DateTime.Now.ToString("HH:m:ss tt", Culture),
                    SOConfirmDate = CurrentDate,
                    SODeliveryDate = CurrentDate,
                    SOFor = "For Saleorder"
                };
                model.SONo = model.EntryID;
            }

            model.SOForList = await _IDataLogic.GetDropDownList("SOFOR", "SP_GetDropDownList").ConfigureAwait(false);
            model.QuotList = await _IDataLogic.GetDropDownList("QUOTDATA", "SP_GetDropDownList").ConfigureAwait(false);
            model.BranchList = await _IDataLogic.GetDropDownList("BRANCH", "SP_GetDropDownList").ConfigureAwait(false);
            model.SOTypeList = await _IDataLogic.GetDropDownList("SOTYPE", "SP_GetDropDownList").ConfigureAwait(false);
            model.QuotYearList = await _IDataLogic.GetDropDownList("QUOTYEAR", "SP_GetDropDownList").ConfigureAwait(false);
            model.CurrencyList = await _IDataLogic.GetDropDownList("CURRENCY", "SP_GetDropDownList").ConfigureAwait(false);
            model.StoreList = await _IDataLogic.GetDropDownList("Store_Master", "SP_GetDropDownList").ConfigureAwait(false);
            model.SaleDocTypeList = await _IDataLogic.GetDropDownList("SALEDOC", "SP_GetDropDownList").ConfigureAwait(false);
            model.PreparedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList").ConfigureAwait(false);
            model.AccountList = await _IDataLogic.GetDropDownList("PARTYNAMELIST", "F", "SP_GetDropDownList").ConfigureAwait(false);
            model.ResponsibleSalesPersonList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList").ConfigureAwait(false);
            model.PartCodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList").ConfigureAwait(false);
            model.ItemNameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList").ConfigureAwait(false);

            return model;
        }

        [HttpGet, Route("{controller}/Index")]
        public async Task<IActionResult> SOAmendment(string Mode, string EID)
        {
            // _logger.Log(LogLevel.Information, "\n \n ********* SOAmendment ********* \n " +
            // JsonConvert.SerializeObject(loggerInfo) + "\n");

            _loggerMessage(this._logger, "SOAmendment", null);

            var model = new SaleOrderModel();

            if (!string.IsNullOrEmpty(Mode) && !string.IsNullOrEmpty(EID))
            {
                int.TryParse(_EncryptDecrypt.Decrypt(EID), out var DID);

                model = await _ISaleOrder.GetViewByID(DID, 2022, "").ConfigureAwait(false);
                model = await BindModels(model).ConfigureAwait(false);
                model.Mode = Mode;
                model.ID = DID;

                if (model.ItemDetailGrid?.Count != 0 && model.ItemDetailGrid != null)
                {
                    HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
                }

                if (model.TaxDetailGridd != null)
                {
                    MemoryCacheEntryOptions cacheEntryOptions = new()
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                        SlidingExpiration = TimeSpan.FromMinutes(60),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd, cacheEntryOptions);
                }
            }
            else
            {
                model = await BindModels(null).ConfigureAwait(false);
                HttpContext.Session.Remove("ItemList");
                HttpContext.Session.Remove("TaxGrid");
                _MemoryCache.Remove("KeyTaxGrid");
            }

            //var model = new SaleOrderModel

            //{
            //    DeliveryScheduleList = new List<DeliverySchedule>{new()},

            // AccountList = new List<TextValue> { new() },

            // BranchList = new List<TextValue> { new() },

            // CurrencyList = new List<TextValue> { new() },

            // FreightByList = new List<SelectListItem> { new() },

            // ItemNameList = new List<TextValue> { new() },

            // OrderTypeList = new List<SelectListItem> { new() },

            // PartCodeList = new List<TextValue> { new() },

            // PreparedByList = new List<TextValue> { new() },

            // QuotList = new List<TextValue> { new() },

            // QuotYearList = new List<TextValue> { new() },

            // ResponsibleSalesPersonList = new List<TextValue> { new() },

            // SaleDocTypeList = new List<TextValue> { new() },

            // SOForList = new List<TextValue> { new() },

            // SOTypeList = new List<TextValue> { new() },

            // StoreList = new List<TextValue> { new() },

            // TaxList = new List<SelectListItem> { new() },

            // TransportModeList = new List<SelectListItem> { new() },

            // UnitRateList = new List<SelectListItem> { new() }

            //};

            return View(model);
        }


        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _ISaleOrder.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost, Route("{controller}/SaveForm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SOAmendment(SaleOrderModel model)
        {
            _loggerMessage(this._logger, "Post - SOAmendment - ModelValues : " + JsonConvert.SerializeObject(model), null);

            if (!string.IsNullOrEmpty(model?.Mode) && model.Mode == "I")
            {
                model.Mode = model?.Mode == "U" ? "Update" : "Insert";
                model.CreatedBy = Constants.UserID;

                var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);
                _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

                if (model.TaxDetailGridd != null)
                {
                    MemoryCacheEntryOptions cacheEntryOptions = new()
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                        SlidingExpiration = TimeSpan.FromMinutes(60),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd, cacheEntryOptions);
                }
            }
            else
            {
                model = await BindModels(null).ConfigureAwait(false);
                HttpContext.Session.Remove("ItemList");
                HttpContext.Session.Remove("TaxGrid");
                _MemoryCache.Remove("KeyTaxGrid");
            }

            return View(model);
        }

        [HttpGet, Route("{controller}/AmendmentList")]
        public async Task<IActionResult> SOAmendmentList()
        {
            HttpContext.Session.Remove("ItemList");
            HttpContext.Session.Remove("TaxGrid");
            _MemoryCache.Remove("KeyTaxGrid");
            var _List = new List<TextValue>();
            string EndDate = HttpContext.Session.GetString("ToDate");
            var model = await _ISaleOrder.GetDashboardData(EndDate).ConfigureAwait(true);

            foreach (var item in model.SODashboard)
            {
                item.EID = _EncryptDecrypt.Encrypt(item.EntryID.ToString(new CultureInfo("en-GB")));
                TextValue _SONo = new()
                {
                    Text = item.SONo.ToString(Culture),
                    Value = item.SONo.ToString(Culture),
                };
                _List.Add(_SONo);
            }
            model.SONoList = _List;

            model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy", Culture).Replace("-", "/", 0); // 1st Feb this year
            model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy", Culture).Replace("-", "/", 0);//.AddDays(-1); // Last day in January next year

            return View(model);
        }
    }
}