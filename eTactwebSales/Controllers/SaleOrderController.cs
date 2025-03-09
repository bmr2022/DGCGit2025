using System.Data;
using System.Diagnostics;
using System.Xml;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers;

[Authorize]
public class SaleOrderController : Controller
{
    private readonly IDataLogic _IDataLogic;
    private readonly ISaleOrder _ISaleOrder;
    private readonly ITaxModule _ITaxModule;
    private readonly ILogger _logger;
    private readonly IMemoryCache _MemoryCache;
    private readonly IItemMaster itemMaster;

    public SaleOrderController(ILogger<SaleOrderController> logger, IDataLogic iDataLogic, ISaleOrder iSaleOrder, ITaxModule iTaxModule, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IItemMaster itemMaster, EncryptDecrypt encryptDecrypt, LoggerInfo loggerInfo)
    {
        _logger = logger;
        _IDataLogic = iDataLogic;
        _ISaleOrder = iSaleOrder;
        _ITaxModule = iTaxModule;
        _MemoryCache = iMemoryCache;
        _IWebHostEnvironment = iWebHostEnvironment;
        this.itemMaster = itemMaster;
        _EncryptDecrypt = encryptDecrypt;
        LoggerInfo = loggerInfo;
    }

    private EncryptDecrypt _EncryptDecrypt { get; }
    private IWebHostEnvironment _IWebHostEnvironment { get; }
    private LoggerInfo LoggerInfo { get; }

    public PartialViewResult AddSchedule(DeliverySchedule model)
    {
        var MainModel = new SaleOrderModel();
        var ScheduleList = new List<DeliverySchedule>();
        var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

        foreach (ItemDetail item in ItemDetailList)
        {
            if (item.PartCode == model.DPartCode)
            {
                if (item.DeliveryScheduleList == null)
                {
                    ScheduleList.Add(
                        new DeliverySchedule()
                        {
                            SRNo = 1,
                            DPartCode = model.DPartCode,
                            ItemName = model.ItemName,
                            AltQty = model.AltQty,
                            Date = Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"),
                            Days = model.Days,
                            Qty = model.Qty,
                            TotalQty = model.Qty,
                            Remarks = model.Remarks,
                        });
                    item.DeliveryScheduleList = ScheduleList;
                    MainModel.DPartCode = model.DPartCode;
                    MainModel.ItemDetailGrid = ItemDetailList;
                    MainModel.DeliveryScheduleList = ScheduleList;
                    HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
                else
                {
                    item.DeliveryScheduleList.Add(
                        new DeliverySchedule()
                        {
                            SRNo = item.DeliveryScheduleList.Count + 1,
                            DPartCode = model.DPartCode,
                            AltQty = model.AltQty,
                            Date = Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"),
                            Days = model.Days,
                            Qty = model.Qty,
                            TotalQty = item.DeliveryScheduleList.Sum(x => x.Qty) + model.Qty,
                            Remarks = model.Remarks,
                        });
                    MainModel.DPartCode = model.DPartCode;
                    MainModel.ItemDetailGrid = ItemDetailList;
                    MainModel.DeliveryScheduleList = item.DeliveryScheduleList;
                    HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
            }
        }



        //var MainModel = new OrderMainModel();
        //var _List = new List<DeliverySchedule>();

        //if (HttpContext.Session.GetString("Schedule") == null)
        //{
        //    _List.Add(new DeliverySchedule()
        //    {
        //        SRNo = 1,
        //        AltQty = model.AltQty,
        //        Date = Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"),
        //        Days = model.Days,
        //        Qty = model.Qty,
        //        TotalQty = model.Qty,
        //        Remarks = model.Remarks,
        //    });

        // MainModel.DeliveryScheduleList = _List;

        //    HttpContext.Session.SetString("Schedule", JsonConvert.SerializeObject(MainModel.DeliveryScheduleList));
        //}
        //else
        //{
        //    _List = JsonConvert.DeserializeObject<List<DeliverySchedule>>(HttpContext.Session.GetString("Schedule"));
        //    //_List.ForEach(x => x.TotalQty = _List.Sum(x => x.Qty));

        // _List.Add(new DeliverySchedule() { SRNo = _List.Count + 1, AltQty = model.AltQty, Date =
        // Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"), Days = model.Days, Qty =
        // model.Qty, TotalQty = _List.Sum(x => x.Qty) + model.Qty, Remarks = model.Remarks, });

        // MainModel.DeliveryScheduleList = _List;

        //    HttpContext.Session.SetString("Schedule", JsonConvert.SerializeObject(MainModel.DeliveryScheduleList));
        //}

        return PartialView("_SODeliveryGrid", MainModel);
    }

    public async Task<JsonResult> GetServerDate()
    {
        try
        {
            DateTime time = DateTime.Now;
            string format = "MMM ddd d HH:mm yyyy";
            string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var dt = time.ToString(format);
            return Json(formattedDate);
            //string apiUrl = "https://worldtimeapi.org/api/ip";

            //using (HttpClient client = new HttpClient())
            //{
            //    HttpResponseMessage response = await client.GetAsync(apiUrl);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        string content = await response.Content.ReadAsStringAsync();
            //        JObject jsonObj = JObject.Parse(content);

            //        string datetimestring = (string)jsonObj["datetime"];
            //        var formattedDateTime = datetimestring.Split(" ")[0];

            //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            //        return Json(formattedDate);
            //    }
            //    else
            //    {
            //        string errorContent = await response.Content.ReadAsStringAsync();
            //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
            //    }
            //}
        }
        catch (HttpRequestException ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine($"HttpRequestException: {ex.Message}");
            return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
        }
        catch (Exception ex)
        {
            // Log any other unexpected exceptions
            Console.WriteLine($"Unexpected Exception: {ex.Message}");
            return Json(new { error = "An unexpected error occurred: " + ex.Message });
        }
    }

    public PartialViewResult AddMultiBuyers(SaleOrderBillToShipTo model)
    {
        var MainModel = new SaleOrderModel();
        var BillToShipToList = new List<SaleOrderBillToShipTo>();
        var BillShipGrid = new List<SaleOrderBillToShipTo>();
        _MemoryCache.TryGetValue("KeySaleBillToShipTo", out BillShipGrid);
        //_MemoryCache.TryGetValue("KeySaleBillToShipTo",out BillToShipToList);
        //var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? String.Empty);
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        if (BillShipGrid == null)
        {
            BillToShipToList.Add(
                    new SaleOrderBillToShipTo()
                    {
                        SeqNo = 1,
                        MainCustomerId = model.MainCustomerId,
                        BillToAccountCode = model.BillToAccountCode,
                        BillToAccountName = model.BillToAccountName,
                        BuyerAddress = model.BuyerAddress,
                        ShiptoAccountCode = model.ShiptoAccountCode,
                        ShiptoAccountName = model.ShiptoAccountName,
                        ShipToAddress = model.ShipToAddress,

                    });
            MainModel.SaleOrderBillToShipTo = BillToShipToList;
            _MemoryCache.Set("KeySaleBillToShipTo", MainModel.SaleOrderBillToShipTo, cacheEntryOptions);

        }
        else
        {
            var billCount = BillShipGrid.Count;
            BillShipGrid.Add(
                new SaleOrderBillToShipTo()
                {
                    SeqNo = billCount + 1,
                    MainCustomerId = model.MainCustomerId,
                    BillToAccountCode = model.BillToAccountCode,
                    BillToAccountName = model.BillToAccountName,
                    BuyerAddress = model.BuyerAddress,
                    ShiptoAccountCode = model.ShiptoAccountCode,
                    ShiptoAccountName = model.ShiptoAccountName,
                    ShipToAddress = model.ShipToAddress,
                });
            MainModel.SaleOrderBillToShipTo = BillShipGrid;
            _MemoryCache.Set("KeySaleBillToShipTo", MainModel.SaleOrderBillToShipTo, cacheEntryOptions);
        }
        _MemoryCache.TryGetValue("KeySaleBillToShipTo", out List<SaleOrderBillToShipTo> SaleOrderBillToShipToGrid);

        return PartialView("_SOMultiBuyerGrid", MainModel);
    }
    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await _ISaleOrder.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> NewEntryId(int YearCode)
    {
        var JSON = await _ISaleOrder.NewEntryId(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> NewAmmEntryId(int YearCode)
    {
        var JSON = await _ISaleOrder.NewAmmEntryId(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<SaleOrderModel> BindModels(SaleOrderModel model)
    {
        if (model == null)
        {
            model = new SaleOrderModel
            {
                YearCode = Constants.FinincialYear,
                EntryID = _IDataLogic.GetEntryID("SaleOrderMain", Constants.FinincialYear, "SOEntryID", "SOyearcode"),
                SONo = _IDataLogic.GetEntryID("SaleOrderMain", Constants.FinincialYear, "SOEntryID","SOyearcode"),
                EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                WEF = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                QDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                SODate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                AmmEffDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                SOCloseDate = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy").Replace("-", "/"),
                EntryTime = DateTime.Now.ToString("HH:m:ss tt"),
                SOConfirmDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                SODeliveryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                SOFor = "For Saleorder"
            };
        }

        model.SOForList = await _IDataLogic.GetDropDownList("SOFOR", "SP_GetDropDownList");
        model.QuotList = await _IDataLogic.GetDropDownList("QUOTDATA", "SP_GetDropDownList");
        //model.BranchList = await _IDataLogic.GetDropDownList("BRANCH", "SP_GetDropDownList");
        model.SOTypeList = await _IDataLogic.GetDropDownList("SOTYPE", "SP_GetDropDownList");
        model.QuotYearList = await _IDataLogic.GetDropDownList("QUOTYEAR", "SP_GetDropDownList");
        model.CurrencyList = await _IDataLogic.GetDropDownList("CURRENCY", "SP_GetDropDownList");
        model.StoreList = await _IDataLogic.GetDropDownList("Store_Master", "SP_GetDropDownList");
        model.SaleDocTypeList = await _IDataLogic.GetDropDownList("SALEDOC", "SP_GetDropDownList");
        model.PreparedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");
        model.AccountList = await _IDataLogic.GetDropDownList("PARTYNAMELIST", "F", "SP_GetDropDownList");
        model.ResponsibleSalesPersonList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");
        model.PartCodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.ItemNameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
        model.Branch = HttpContext.Session.GetString("Branch");
        model.FinFromDate = HttpContext.Session.GetString("FromDate");
        model.FinToDate = HttpContext.Session.GetString("ToDate");
        model.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        model.PreparedByName = HttpContext.Session.GetString("EmpName");
        //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        if (model.Mode == "SOA")
        {
            model.AmmEffDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
        }

        return model;
    }

    public async Task<IActionResult> Dashboard()
    {
        HttpContext.Session.Remove("ItemList");
        HttpContext.Session.Remove("TaxGrid");
        _MemoryCache.Remove("KeyTaxGrid");

        var _List = new List<TextValue>();
        string EndDate = HttpContext.Session.GetString("ToDate");
        var model = await _ISaleOrder.GetDashboardData(EndDate);

        foreach (SaleOrderDashboard item in model.SODashboard)
        {
            TextValue _SONo = new()
            {
                Text = item.SONo.ToString(),
                Value = item.SONo.ToString(),
            };
            _List.Add(_SONo);
        }
        model.BranchList = await _IDataLogic.GetDropDownList("BRANCH", "SP_GetDropDownList");
        model.SONoList = _List;
        model.CC = HttpContext.Session.GetString("Branch");
        model.Year = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/"); // 1st Feb this year
        model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");//.AddDays(-1); // Last day in January next year

        return View(model);
    }

    public async Task<IActionResult> DeleteByID(int ID, int YC) 
    {
        var Result = await _ISaleOrder.DeleteByID(ID, YC, "DELETEBYID");

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Locked)
        {
            ViewBag.isSuccess = true;
            TempData["423"] = "423";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction(nameof(Dashboard));
    }

    public PartialViewResult DeleteDeliveryRow(string SRNo, string DPC)
    {
        SaleOrderModel MainModel = new();
        var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

        int Indx = Convert.ToInt32(SRNo) - 1;
        int PCode = Convert.ToInt32(DPC);

        List<ItemDetail> DeliveryList = ItemDetailList.Where(x => x.PartCode == PCode).ToList();
        List<DeliverySchedule> ScheduleList = DeliveryList
            .SelectMany(x => x.DeliveryScheduleList)
            .ToList();
        ScheduleList.RemoveAt(Indx);

        Indx = 0;
        foreach (DeliverySchedule item in ScheduleList)
        {
            Indx++;
            item.SRNo = Indx;
            item.TotalQty = ScheduleList.Sum(x => x.Qty);
            MainModel.DPartCode = item.DPartCode;
        }

        foreach (ItemDetail ID in ItemDetailList)
        {
            if (ID.PartCode == PCode && ID.DeliveryScheduleList != null)
            {
                foreach (DeliverySchedule item in ID.DeliveryScheduleList)
                {
                    if (item.DPartCode == PCode)
                    {
                        ID.DeliveryScheduleList = ScheduleList;
                        break;
                    }
                }
            }
        }

        MainModel.ItemDetailGrid = ItemDetailList;
        HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));


        return PartialView("_SODeliveryGrid", MainModel);
    }

    public PartialViewResult DeleteBillToShipToRow(string SeqNo)
    {
        var MainModel = new SaleOrderModel();
        _MemoryCache.TryGetValue("KeySaleBillToShipTo", out List<SaleOrderBillToShipTo> SaleBillGrid);
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (SaleBillGrid != null && SaleBillGrid.Count > 0)
        {
            SaleBillGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (var item in SaleBillGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.SaleOrderBillToShipTo = SaleBillGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeySaleBillToShipTo", MainModel.SaleOrderBillToShipTo, cacheEntryOptions);
            _MemoryCache.TryGetValue("KeySaleBillToShipTo", out List<SaleOrderBillToShipTo> SaleBillGridDemo);

            if (SaleBillGrid.Count == 0)
            {
                _MemoryCache.Remove("KeySaleBillToShipTo");
            }
        }
        return PartialView("_SOMultiBuyerGrid", MainModel);
    }

    public IActionResult DeleteItemRow(string SeqNo)
    {
        bool exists = false;
        var model = new SaleOrderModel();
        _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (HttpContext.Session.GetString("ItemList") != null)
        {
            model.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList"));

            var itemfound = model.ItemDetailGrid.FirstOrDefault(item => item.SeqNo == Convert.ToInt32(SeqNo)).PartCode;

            var ItmPartCode = (from item in model.ItemDetailGrid
                               where item.SeqNo == Convert.ToInt32(SeqNo)
                               select item.PartCode).FirstOrDefault();

            if (TaxGrid != null)
            {
                exists = TaxGrid.Any(x => x.TxPartCode == ItmPartCode);
            }

            if (exists)
            {
                return StatusCode(207, "Duplicate");
                //return Problem();
            }

            model.ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (ItemDetail item in model.ItemDetailGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            model.ItemNetAmount = model.ItemDetailGrid.Sum(x => x.Amount);
            if (model.ItemDetailGrid.Count <= 0)
            {
                HttpContext.Session.Remove("ItemList");
                _MemoryCache.Remove("ItemList");
            }
            else
            {
                HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
            }
        }
        return PartialView("_SaleItemGrid", model);
    }

    public IActionResult EditItemRow(SaleOrderModel model)
    {
        bool exists = false;
        object Result = string.Empty;

        int Indx = Convert.ToInt32(model.SeqNo) - 1;

        if (HttpContext.Session.GetString("ItemList") != null)
        {
            _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            model.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList"));

            var ItmPartCode = model.ItemDetailGrid.FirstOrDefault(item => item.SeqNo == Convert.ToInt32(model.SeqNo)).PartCode;

            if (TaxGrid != null)
            {
                exists = TaxGrid.Any(x => x.TxPartCode == ItmPartCode);
            }

            if (exists)
            {
                return StatusCode(207, "Duplicate");
            }
            Result = model.ItemDetailGrid.Where(m => m.SeqNo == model.SeqNo).ToList();
            model.ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;
            foreach (ItemDetail item in model.ItemDetailGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }

            if (model.ItemDetailGrid.Count > 0)
            {
                HttpContext.Session.SetString
                (
                    "ItemList",
                    JsonConvert.SerializeObject(model.ItemDetailGrid)
                );
            }
            else
            {
                HttpContext.Session.Remove("ItemList");
            }
        }

        return Json(JsonConvert.SerializeObject(Result));
    }
    public JsonResult ResetGridItems()
    {
        HttpContext.Session.Remove("ItemList");
        _MemoryCache.Remove("KeyTaxGrid");

        var MainModel = new SaleOrderModel();
        List<TaxModel> taxList = new List<TaxModel>();

        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
        MainModel.Branch = HttpContext.Session.GetString("Branch");

        _MemoryCache.Set("ItemList", MainModel, DateTimeOffset.Now.AddMinutes(60));
        _MemoryCache.Set("KeyTaxGrid", taxList, DateTimeOffset.Now.AddMinutes(60));
        HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel));
        _MemoryCache.TryGetValue("ItemList", out MainModel);
        _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

        return new(StatusCodes.Status200OK);
    }

    // GET: SaleOrderController/GetAddress
    public async Task<JsonResult> GetAddress(string Code)
    {
        ResponseResult JsonString = await _ISaleOrder.GetAddress(Code);
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonString);
    }
    public async Task<JsonResult> GetClearTxGrid(string Code)
    {
        _MemoryCache.Remove("KeyTaxGrid");
        ResponseResult JsonString = await _ISaleOrder.GetAddress(Code);
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonString);
    }
    public async Task<JsonResult> GetClearItemGrid(string Code)
    {
        _MemoryCache.Remove("ItemList");
        _MemoryCache.Remove("KeyTaxGrid");
        ResponseResult JsonString = await _ISaleOrder.GetAddress(Code);
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonString);
    }
    public async Task<JsonResult> GetFillCurrency(string CTRL)
    {
        ResponseResult JsonString = await _ISaleOrder.GetFillCurrency(CTRL);
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        var stringJson = JsonConvert.SerializeObject(JsonString);
        return Json(stringJson);
    }

    public async Task<IActionResult> GetAmmSearchData(SaleOrderDashboard model)
    {
        model = await _ISaleOrder.GetAmmSearchData(model);
        model.Mode = "Pending";
        return PartialView("_SOAmmListGrid", model);
    }
    public async Task<IActionResult> GetUpdAmmData(SaleOrderDashboard model)
    {
        model = await _ISaleOrder.GetUpdAmmData(model);
        model.Mode = "U";
        return PartialView("_SOAmmListGrid", model);
    }

    public async Task<IActionResult> GetCurrencyDetail(string Currency)
    {
        string CurrentDate = DateTime.Today.ToString("dd/MMM/yyyy").Replace("-", "/");
        ResponseResult Result = await _ISaleOrder.GetCurrencyDetail(CurrentDate, Currency);
        return Json(JsonConvert.SerializeObject(Result));
    }

    public async Task<JsonResult> GetItemPartCode(string Code)
    {
        ResponseResult JsonString = await _ISaleOrder.GetItemPartCode(Code, "GetItemPartCode");
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonConvert.SerializeObject(JsonString));
    }

    public async Task<IActionResult> GetItemPartList(string TF)
    {
        dynamic PartCodeList = null;
        dynamic ItemNameList = null;
        // with 3 param
        if (TF == "F")
        {
            PartCodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
            ItemNameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
        }
        else
        {
            PartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
            ItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");
        }
        string PartCode = JsonConvert.SerializeObject(PartCodeList);
        string ItemName = JsonConvert.SerializeObject(ItemNameList);

        return Json(new { PartCode, ItemName });
    }

    // GET: SaleOrderController/GetPartyList
    public async Task<JsonResult> GetPartyList(string Check)
    {
        var JSON = await _IDataLogic.GetDropDownList("PARTYNAMELIST", Check, "SP_GetDropDownList");
        _logger.LogError(JsonConvert.SerializeObject(JSON));
        return Json(JSON);
    }

    public async Task<JsonResult> GetQuotData(string Code)
    {
        var JsonString = await _ISaleOrder.GetQuotData(Code, "QUOTDATA");
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonConvert.SerializeObject(JsonString));
    }

    public async Task<IActionResult> GetSearchData(SaleOrderDashboard model)
    {
        model = await _ISaleOrder.GetSearchData(model);
        return PartialView("_SODashboardGrid", model);
    }

    public async Task<IActionResult> GetSOAmmCompletedSearchData(SaleOrderDashboard model)
    {
        model = await _ISaleOrder.GetSOAmmCompletedSearchData(model);
        model.Mode = "Completed";
        return PartialView("_SOAmmListGrid", model);
    }

    //public JsonResult GetTaxPartItem()
    //{
    //    List<TextValue> PartCode = new();
    //    List<TextValue> ItemCode = new();

    // if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ItemList"))) { List<ItemDetail>
    // model = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList"));

    // if (model != null && model.Count > 0) { foreach (ItemDetail item in model) { PartCode.Add(new
    // TextValue { Text = item.PartText, Value = item.PartCode.ToString() });

    // ItemCode.Add(new TextValue { Text = item.ItemText, Value = item.ItemCode.ToString() }); } } }

    //    _logger.LogError(JsonConvert.SerializeObject(PartCode));
    //    return Json(new { PartCode, ItemCode });
    //}

    public async Task<IActionResult> ItemGrid(SaleOrderModel model)
    {
        var _List = new List<ItemDetail>();
        var MainModel = new SaleOrderModel();
        _MemoryCache.TryGetValue("ItemList", out MainModel);
        _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
        if (MainModel?.ItemDetailGrid == null)
        {
            _List.Add(
                new ItemDetail
                {
                    SeqNo = 1,
                    PartCode = model.PartCode,
                    PartText = model.PartText,
                    ItemCode = model.ItemCode,
                    ItemText = model.ItemText,
                    HSNNo = model.HSNNo,
                    Qty = model.Qty,
                    Unit = model.Unit,
                    Rate = model.Rate,
                    OtherRateCurr = model.OtherRateCurr,
                    StoreName = model.StoreName,
                    StockQty = model.StockQty,
                    UnitRate = model.UnitRate,
                    DiscPer = model.DiscPer,
                    DiscRs = model.DiscRs,
                    AltQty = model.AltQty,
                    AltUnit = model.AltUnit,
                    Amount = model.Amount,
                    TolLimit = model.TolLimit,
                    AmendmentNo = model.AmendmentNo,
                    AmendmentDate = model.AmendmentDate,
                    AmendmentReason = model.AmendmentReason,
                    Color = model.Color,
                    Rejper = model.Rejper,
                    Excessper = model.Excessper,
                    Description = model.Description,
                    Remark = model.Remark,
                    ProjQty1 = model.ProjQty1,
                    ProjQty2 = model.ProjQty2,
                });

            model.ItemDetailGrid = _List;
            model.ItemNetAmount = decimal.Parse(_List.Sum(x => x.Amount).ToString("#.#0"));
            HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
            _MemoryCache.Set("ItemList", model.ItemDetailGrid);
        }
        else
        {
            //_MemoryCache.TryGetValue("KeyTaxGrid", out IList<TaxModel> TaxGrid);


            var storedItemList =  HttpContext.Session.GetString("ItemList");//MainModel?.ItemDetailGrid;//
            if (storedItemList != null)
            {
                model.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(storedItemList);
            }

            bool TF = model.ItemDetailGrid.Any(x => x.ItemCode == model.ItemCode);
            //bool TF = MainModel.ItemDetailGrid.Any(x => x.ItemCode == model.ItemCode);

            if (TF == false)
            {
                model.ItemDetailGrid.Add(
                    new ItemDetail
                    {
                        SeqNo = model.ItemDetailGrid.Count + 1,
                        PartCode = model.PartCode,
                        PartText = model.PartText,
                        ItemCode = model.ItemCode,
                        ItemText = model.ItemText,
                        HSNNo = model.HSNNo,
                        Qty = model.Qty,
                        Unit = model.Unit,
                        Rate = model.Rate,
                        UnitRate = model.UnitRate,
                        DiscPer = model.DiscPer,
                        DiscRs = model.DiscRs,
                        AltQty = model.AltQty,
                        AltUnit = model.AltUnit,
                        Amount = model.Amount,
                        TolLimit = model.TolLimit,
                        AmendmentNo = model.AmendmentNo,
                        AmendmentDate = model.AmendmentDate,
                        AmendmentReason = model.AmendmentReason,
                        Color = model.Color,
                        Rejper = model.Rejper,
                        Excessper = model.Excessper,
                        Description = model.Description,
                        Remark = model.Remark,
                        ProjQty1 = model.ProjQty1,
                        ProjQty2 = model.ProjQty2,
                    });

                model.ItemNetAmount = decimal.Round(model.ItemDetailGrid.Sum(x => x.Amount), 2);
                //HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
            }
            else
            {
                return StatusCode(207, "Duplicate");
            }

        }
        HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
        _MemoryCache.Set("ItemList", model);
        _MemoryCache.TryGetValue("ItemList", out model);
        return PartialView("_SaleItemGrid", model);
    }

    //private readonly ICacheProvider _cacheProvider;
    // GET: SaleOrderController/OrderDetail
    //[Obsolete]
    public async Task<JsonResult> GetTotalStock(int store, int Itemcode)
    {
        var JSON = await _ISaleOrder.GetTotalStockList(store, Itemcode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAllowMultiBuyerProp()
    {
        var JSON = await _ISaleOrder.GetAllowMultiBuyerProp();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetCurrency(string Currency)
    {
        var JSON = await _ISaleOrder.GetCurrency(Currency);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GetAltQty(int ItemCode, float UnitQty, float ALtQty)
    {
        var JSON = await _ISaleOrder.GetAltQty(ItemCode, UnitQty, ALtQty);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetLockedYear(int YearCode)
    {
        var JSON = await _ISaleOrder.GetLockedYear(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }



    [Route("{controller}/Index")]
    public async Task<ActionResult> OrderDetail(string Mode, int ID, int YC)
    {
       // string ipaddress = IPAddress.IPv6Loopback.ToString();
        var model = new SaleOrderModel();

        //var webReport = new WebReport();
        //var mssqlDataConnection = new MsSqlDataConnection();
        //mssqlDataConnection.ConnectionString = _IDataLogic.GetDBConnection();
        //webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);

        ////webReport.EnableMargins = true;
        ////webReport.ShowExports = true;

        //webReport.Report.Load(Path.Combine(_IWebHostEnvironment.ContentRootPath, "REPORT", "ItemMasterReport.frx"));

        //webReport.Report.Load(Path.Combine(_IWebHostEnvironment.WebRootPath, "Reports", "ItemMasterReport2.frx"));

        //var categories = GetTable<Category>(_northwindContext.Categories, "Categories");
        //webReport.Report.RegisterData(categories, "Categories");

        ////var DTReport = itemMaster.GetDashBoardData("", "", "", "", "", "").Result.ToArray();
        ////webReport.Report.RegisterData(DTReport, "Categories");

        ////webReport.Report.Prepare();
        ////FastReport.Export.Html.HTMLExport export = new FastReport.Export.Html.HTMLExport();
        ////webReport.Report.Export(export, "result.pdf");

        //return PartialView("_ViewReport", webReport);
        model.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        model.PreparedByName = HttpContext.Session.GetString("EmpName");
        model.FinFromDate = HttpContext.Session.GetString("FromDate");
        if (Mode != "SOA" && Mode != "SAU")
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        else
        {

            model.YearCode = YC;
            model.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }
        model.FinToDate = HttpContext.Session.GetString("ToDate");
        model.Branch = HttpContext.Session.GetString("Branch");

        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U" || Mode == "SOA" || Mode == "SAU"))
        {
            model = await _ISaleOrder.GetViewByID(ID, YC, Mode == "SOA" ? "SOA" : "VIEWBYID").ConfigureAwait(true);

            model.Mode = Mode;
            model = await BindModels(model);

            model.ID = ID;
            string EID = _EncryptDecrypt.Encrypt(model.EntryID.ToString());
            string DID = _EncryptDecrypt.Decrypt(EID);
            model.EID = EID;

            if (model.ItemDetailGrid?.Count != 0 && model.ItemDetailGrid != null)
            {
                HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
                _MemoryCache.Set("ItemList", model);
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
            if (model.SaleOrderBillToShipTo != null)
            {
                MemoryCacheEntryOptions cacheEntryOptions = new()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                    SlidingExpiration = TimeSpan.FromMinutes(60),
                    Size = 1024,
                };

                _MemoryCache.Set("KeySaleBillToShipTo", model.SaleOrderBillToShipTo, cacheEntryOptions);
            }
        }
        else
        {
            model = await BindModels(null);
            HttpContext.Session.Remove("ItemList");
            _MemoryCache.Remove("ItemList");
            HttpContext.Session.Remove("TaxGrid");
            _MemoryCache.Remove("KeyTaxGrid");
            _MemoryCache.Remove("KeySaleBillToShipTo");
        }

        if (Mode != "SOA" && Mode != "SAU")
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        else
        {

            model.YearCode = YC;
            model.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }
        return View(model);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [Route("{controller}/Index")]
    public async Task<ActionResult> OrderDetail(SaleOrderModel model)
    {
        try
        {
            bool isError = true;
            DataSet DS = new();
            DataTable ItemDetailDT = null;
            DataTable TaxDetailDT = null;
            DataTable MultiBuyersDT = null;
            ResponseResult Result = new();
            DataTable DelieveryScheduleDT = null;
            Dictionary<string, string> ErrList = new();
            _MemoryCache.TryGetValue("KeySaleBillToShipTo", out List<SaleOrderBillToShipTo> SaleOrderBillToShipToGrid);


            var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

            _logger.LogInformation("ItemDetailList session Data done", DateTime.UtcNow);

            _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            var MainModel = new SaleOrderModel();
            //_MemoryCache.TryGetValue("ItemList", out List<ItemDetail> ItemDetailList);

            ModelState.Clear();

            //var ItemDetailList = MainModel.ItemDetailGrid;
            _logger.LogInformation("TaxGrid session Data done", DateTime.UtcNow);

            if (ItemDetailList != null && ItemDetailList.Count > 0)
            {
                foreach (var item in ItemDetailList)
                {
                    item.Mode = model.Mode;
                }
                DS = GetItemDetailTable(ItemDetailList);
                ItemDetailDT = DS.Tables[0];
                DelieveryScheduleDT = DS.Tables[1];
                model.ItemDetailGrid = ItemDetailList;
                isError = false;
            }
            else
            {
                ErrList.Add("ItemDetailGrid", "Item Details Cannot Be Blank..!");
            }

            _logger.LogInformation("GetItemDetailTable Data done", DateTime.UtcNow);

            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                TaxDetailDT = GetTaxDetailTable(TaxGrid);
                model.ItemDetailGrid = ItemDetailList;
            }
            if (SaleOrderBillToShipToGrid != null)
            {
                MultiBuyersDT = GetMultiBuyers(SaleOrderBillToShipToGrid);

            }
            if (model.PreparedBy == 0)
            {
                ErrList.Add("PreparedBy", "Please Select Prepared By From List..!");
            }

            _logger.LogInformation("MultiBuyers done", DateTime.UtcNow);

            if (!isError)
            {
                if (ItemDetailDT.Rows.Count > 0 || TaxDetailDT.Rows.Count > 0)
                {
                    if (model.Mode != "U" && model.Mode != "SOA" && model.Mode != "SSA")
                    {
                        model.Mode = "Insert";
                    }
                    else if (model.Mode == "U")
                    {
                        model.Mode = "Update";
                    }
                    else
                    {
                        model.Mode = model.Mode;
                    }
                    //model.Mode = model.Mode == "U" ? "Update" : "Insert";
                    model.CreatedBy = Constants.UserID;
                    Result = await _ISaleOrder.SaveSaleOrder(ItemDetailDT, DelieveryScheduleDT, TaxDetailDT, MultiBuyersDT, model);
                }
                _logger.LogInformation("Save SaleOrder Data done", DateTime.UtcNow);
                if (Result != null)
                {
                    var stringResponse = JsonConvert.SerializeObject(Result);
                    if (stringResponse.Contains("Constraint"))
                    {

                    }
                    else
                    {
                        if (model.TypeOfSave != "PS")
                        {
                            var saleOrderModel = new SaleOrderModel();
                            saleOrderModel = await BindModels(null);
                            HttpContext.Session.Remove("ItemList");
                            _MemoryCache.Remove("ItemList");
                            HttpContext.Session.Remove("TaxGrid");
                            _MemoryCache.Remove("KeyTaxGrid");
                            _MemoryCache.Remove("KeySaleBillToShipTo");
                            if (Result.StatusCode == HttpStatusCode.InternalServerError)
                            {
                                ViewBag.isSuccess = false;
                                var input = "";
                                input = Result.StatusText;
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    int messageStartIndex = index + "#ERROR_MESSAGE".Length; // Remove the extra space and colon
                                    string errorMessage = input.Substring(messageStartIndex).Trim();
                                    int maxLength = 100;
                                    int wrapLength = Math.Min(maxLength, errorMessage.Length);
                                    TempData["ErrorMessage"] = errorMessage.Substring(0, wrapLength);
                                }
                                else
                                {
                                    TempData["500"] = "500";
                                }

                            }
                            else if (model.Mode == "Update")
                            {
                                ViewBag.isSuccess = true;
                                TempData["202"] = "202";
                            }
                            else
                            {
                                ViewBag.isSuccess = true;
                                TempData["200"] = "200";
                            }
                             return RedirectToAction("OrderDetail", new { ID = 0, YC = 0, Mode = "" });

                        }
                        else
                        {
                            dynamic jsonObj = JsonConvert.DeserializeObject(stringResponse);
                            if (jsonObj.Result != null && jsonObj.Result.Count > 0)
                            {
                                int resultValue = jsonObj.Result[0].Result;
                                int YearCodeVal = jsonObj.Result[0].YearCode;
                                return RedirectToAction("OrderDetail", new { ID = resultValue, YC = YearCodeVal, Mode = "U" });
                            }
                            else
                            {
                                ErrList.Add("ItemDetailGrid", "Something went Wrong");
                            }
                        }
                    }
                }
                else
                {

                }
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
            _logger.LogError("\n \n" + ex, ex.Message, model);
        }
        return View(model);
    }

    public IActionResult RefreshSchedule(int PCode, string Typ)
    {
        var MainModel = new SaleOrderModel();
        Dictionary<string, string> SchVal = new();

        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ItemList")))
        {
            var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

            var DeliveryList = ItemDetailList?.FirstOrDefault(x => x.PartCode == PCode);

            if (DeliveryList != null)
            {
                SchVal.Add("Qty", DeliveryList.Qty.ToString());
                SchVal.Add("AltQty", DeliveryList.AltQty.ToString());
                SchVal.Add("Remark", DeliveryList.Remark == null ? DeliveryList.Remark : DeliveryList.Remark.ToString());
            }

            if (DeliveryList != null && DeliveryList.DeliveryScheduleList != null)
            {
                MainModel.DPartCode = PCode;
                MainModel.ItemDetailGrid = ItemDetailList;
            }
            else
            {
                MainModel.DPartCode = PCode;
            }
        }

        if (Typ == "SchVal")
        {
            return Json(new { SchVal });
        }
        return PartialView("_SODeliveryGrid", MainModel);
    }

    //public JsonResult ResetGridItems()
    //{
    //    HttpContext.Session.Remove("ItemList");

    //    return new(StatusCodes.Status200OK);
    //}

    [HttpPost, Route("{controller}/SOAmendment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SOAmendment(SaleOrderModel model)
    {
        try
        {
            bool isError = true;
            DataSet DS = new();
            DataTable ItemDetailDT = null;
            DataTable TaxDetailDT = null;
            DataTable MultiBuyersDT = null;
            ResponseResult Result = new();
            DataTable DelieveryScheduleDT = null;
            Dictionary<string, string> ErrList = new();

            var AmmStatus = await _ISaleOrder.GetAmmStatus(model.EntryID, model.YearCode).ConfigureAwait(true);

            var ItemDetailList = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

            _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

            ModelState.Clear();

            if (ItemDetailList != null && ItemDetailList.Count > 0)
            {
                DS = GetItemDetailTable(ItemDetailList);
                ItemDetailDT = DS.Tables[0];
                DelieveryScheduleDT = DS.Tables[1];
                model.ItemDetailGrid = ItemDetailList;
                isError = false;
            }
            else
            {
                ErrList.Add("ItemDetail", "Item Details Cannot Be Blank..!");
                isError = true;
            }

            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                TaxDetailDT = GetTaxDetailTable(TaxGrid);
                model.ItemDetailGrid = ItemDetailList;
            }

            if (model.PreparedBy == 0)
            {
                ErrList.Add("Prepared By", "Please Select Prepared By From List..!");
                isError = true;
            }

            if (AmmStatus == null)
            {
                ErrList.Add("Amm Status", "Amendment not possible...!");
                isError = true;
            }

            if (!isError)
            {
                if (ItemDetailDT.Rows.Count > 0 || TaxDetailDT.Rows.Count > 0)
                {
                    model.CreatedBy = Constants.UserID;
                    Result = await _ISaleOrder.SaveSaleOrder(ItemDetailDT, DelieveryScheduleDT, TaxDetailDT, MultiBuyersDT, model);
                }

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                    }
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    return RedirectToAction(nameof(SOAmendmentList));
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
            else
            {
                model = await BindModels(model);

                foreach (KeyValuePair<string, string> Err in ErrList)
                {
                    ModelState.AddModelError(Err.Key, Err.Value);
                }

                return View("OrderDetail", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error In SO Amendment");
            _logger.LogError("\n \n" + ex, ex.Message, model);
        }

        return RedirectToAction(nameof(OrderDetail));
    }

    [HttpGet, Route("/AmendmentList")]
    public async Task<IActionResult> SOAmendmentList()
    {
        HttpContext.Session.Remove("ItemList");
        HttpContext.Session.Remove("TaxGrid");
        _MemoryCache.Remove("KeyTaxGrid");
        var _List = new List<TextValue>();
        var model = await _ISaleOrder.GetAmmDashboardData().ConfigureAwait(true);

        foreach (var item in model.SODashboard)
        {
            item.EID = _EncryptDecrypt.Encrypt(item.EntryID.ToString(new CultureInfo("en-GB")));
            TextValue _SONo = new()
            {
                Text = item.SONo.ToString(new CultureInfo("en-IN")),
                Value = item.SONo.ToString(new CultureInfo("en-IN")),
            };
            _List.Add(_SONo);
        }
        model.SONoList = _List;
        model.Mode = "Pending";
        model.CC = HttpContext.Session.GetString("Branch");
        model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy", new CultureInfo("en-GB")); // 1st Feb this year
        model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy", new CultureInfo("en-GB"));//.AddDays(-1); // Last day in January next year

        return View(model);
    }

    [HttpGet, Route("/AmendmentCompleted")]
    public async Task<IActionResult> SOAmmCompleted()
    {
        var _List = new List<TextValue>();
        HttpContext.Session.Remove("ItemList");
        HttpContext.Session.Remove("TaxGrid");
        _MemoryCache.Remove("KeyTaxGrid");

        var model = await _ISaleOrder.GetSOAmmCompleted().ConfigureAwait(true);
        model.Mode = "Completed";

        foreach (var item in model.SODashboard)
        {
            item.EID = _EncryptDecrypt.Encrypt(item.EntryID.ToString(new CultureInfo("en-GB")));
            TextValue _SONo = new()
            {
                Text = item.SONo.ToString(new CultureInfo("en-IN")),
                Value = item.SONo.ToString(new CultureInfo("en-IN")),
            };
            _List.Add(_SONo);
        }
        model.SONoList = _List;

        model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy", new CultureInfo("en-GB")); // 1st Feb this year
        model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy", new CultureInfo("en-GB"));//.AddDays(-1); // Last day in January next year

        return View(model);
    }

    public async Task<ActionResult> ViewSOCompleted(string Mode, int ID, int YC)
    {
        var model = new SaleOrderModel();

        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "SOC"))
        {
            model = await _ISaleOrder.GetViewSOCcompletedByID(ID, YC, "VIEWSOCOMPLETEDBYID").ConfigureAwait(true);

            model.Mode = Mode;
            model = await BindModels(model);

            model.ID = ID;
            string EID = _EncryptDecrypt.Encrypt(model.EntryID.ToString());
            string DID = _EncryptDecrypt.Decrypt(EID);

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
            model = await BindModels(null);
            HttpContext.Session.Remove("ItemList");
            HttpContext.Session.Remove("TaxGrid");
            _MemoryCache.Remove("KeyTaxGrid");
        }

        return View("OrderDetail", model);
    }

    private static DataTable GetDeliveryTable(ItemDetail itemDetail, ref DataTable TblSch)
    {
        DateTime Dt = new DateTime();
        foreach (var Item in itemDetail.DeliveryScheduleList)
        {
            if (string.IsNullOrEmpty(Item.Date))
            {
                return default;
            }
            if (DateTime.TryParseExact(Item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deliveryDt))
            {
                deliveryDt = DateTime.ParseExact(Item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                deliveryDt = DateTime.Parse(Item.Date);
            }

            TblSch.Rows.Add(
            new object[]
            {
                itemDetail.ItemCode,
                Item.Qty,
                Item.AltQty,
                Item.Days,
                deliveryDt == default ? string.Empty : deliveryDt,
                Item.Remarks,
            });
        }

        return TblSch;
    }

    private static DataTable GetMultiBuyers(List<SaleOrderBillToShipTo> SaleOrderDetail)
    {
        DataTable TblSch = new();
        if (SaleOrderDetail.Count > 0 || SaleOrderDetail != null)
        {
            TblSch.Columns.Add("SeqNo", typeof(int));
            TblSch.Columns.Add("EntryId", typeof(int));
            TblSch.Columns.Add("Yearcode", typeof(int));
            TblSch.Columns.Add("MainCustomerId", typeof(int));
            TblSch.Columns.Add("BillToAccountCode", typeof(int));
            TblSch.Columns.Add("BuyerAddress", typeof(string));
            TblSch.Columns.Add("ShiptoAccountCode", typeof(int));
            TblSch.Columns.Add("ShipToAddress", typeof(string));

            foreach (var Item in SaleOrderDetail)
            {
                TblSch.Rows.Add(
                new object[]
                {
                    Item.SeqNo,
                    0,
                    2023,
                Item.MainCustomerId,
                Item.BillToAccountCode,
                Item.BuyerAddress,
                Item.ShiptoAccountCode,
                Item.ShipToAddress,

                });
            }
        }

        return TblSch;
    }

    private static DataSet GetItemDetailTable(List<ItemDetail> itemDetailList)
    {
        DataSet DS = new();
        DataTable Table = new();

        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("HSNNo", typeof(int));
        Table.Columns.Add("Qty", typeof(float));
        Table.Columns.Add("Unit", typeof(string));
        Table.Columns.Add("AltQty", typeof(float));
        Table.Columns.Add("AltUnit", typeof(string));
        Table.Columns.Add("Rate", typeof(float));
        Table.Columns.Add("OtherRateCurr", typeof(float));
        Table.Columns.Add("UnitRate", typeof(string));
        Table.Columns.Add("DiscPer", typeof(float));
        Table.Columns.Add("DiscRs", typeof(float));
        Table.Columns.Add("Amount", typeof(float));
        Table.Columns.Add("TolLimit", typeof(float));
        Table.Columns.Add("Description", typeof(string));
        Table.Columns.Add("Remark", typeof(string));
        Table.Columns.Add("StoreName", typeof(string));
        Table.Columns.Add("StockQty", typeof(int));
        Table.Columns.Add("AmendmentNo", typeof(string));
        Table.Columns.Add("AmendmentDate", typeof(DateTime));
        Table.Columns.Add("AmendmentReason", typeof(string));
        Table.Columns.Add("Color", typeof(string));
        Table.Columns.Add("Rejper", typeof(float));
        Table.Columns.Add("Excessper", typeof(float));
        Table.Columns.Add("ProjQty1", typeof(float));
        Table.Columns.Add("ProjQty2", typeof(float));

        DataTable TblSch = new();

        TblSch.Columns.Add("ItemCode", typeof(int));
        TblSch.Columns.Add("Qty", typeof(float));
        TblSch.Columns.Add("AltQty", typeof(float));
        TblSch.Columns.Add("Days", typeof(int));
        TblSch.Columns.Add("Date", typeof(string));
        TblSch.Columns.Add("Remarks", typeof(string));

        foreach (ItemDetail Item in itemDetailList)
        {
            Table.Rows.Add(
                new object[]
                {
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.HSNNo,
                    Item.Qty,
                    Item.Unit,
                    Item.AltQty,
                    Item.AltUnit,
                    Item.Rate,
                    Item.OtherRateCurr,
                    Item.UnitRate,
                    Item.DiscPer,
                    Item.DiscRs,
                    Item.Amount,
                    Item.TolLimit,
                    Item.Description,
                    Item.Remark,
                    Item.StoreName,
                    Item.StockQty,
                    Item.AmendmentNo,
                    Item.AmendmentDate == null ? "" : ParseFormattedDate(Item.AmendmentDate),
                    Item.AmendmentReason,
                    Item.Color,
                    Item.Rejper,
                    Item.Excessper,
                    Item.ProjQty1,
                    Item.ProjQty2
                });

            if (Item.DeliveryScheduleList != null && Item.DeliveryScheduleList.Count > 0)
            {
                GetDeliveryTable(Item, ref TblSch);
            }

        }

        DS.Tables.Add(Table);
        DS.Tables.Add(TblSch);
        return DS;
    }

    private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
    {
        DataTable Table = new();
        Table.Columns.Add("TxSeqNo", typeof(int));
        Table.Columns.Add("TxType", typeof(string));
        Table.Columns.Add("TxItemCode", typeof(int));
        Table.Columns.Add("TxTaxType", typeof(int));
        Table.Columns.Add("TxAccountCode", typeof(int));
        Table.Columns.Add("TxPercentg", typeof(float));
        Table.Columns.Add("TxAdInTxable", typeof(string));
        Table.Columns.Add("TxRoundOff", typeof(string));
        Table.Columns.Add("TxAmount", typeof(float));
        Table.Columns.Add("TxRefundable", typeof(string));
        Table.Columns.Add("TxOnExp", typeof(float));
        Table.Columns.Add("TxRemark", typeof(string));

        foreach (TaxModel Item in TaxDetailList)
        {
            Table.Rows.Add(
                new object[]
                {
                    Item.TxSeqNo,
                    Item.TxType,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    Item.TxAdInTxable,
                    Item.TxRoundOff,
                    Item.TxAmount,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark,
                });
        }

        return Table;
    }

    //public IActionResult Report()
    //{
    //    var webReport = new WebReport();
    //    var mssqlDataConnection = new MsSqlDataConnection();
    //    mssqlDataConnection.ConnectionString = _IDataLogic.GetDBConnection();
    //    webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
    //    webReport.Report.Load(Path.Combine(_IWebHostEnvironment.ContentRootPath, "reports", "ItemMasterReport.frx"));
    //    var categories = GetTable<Category>(_northwindContext.Categories, "Categories");
    //    webReport.Report.RegisterData(categories, "Categories");
    //    return View(webReport);
    //}

    [HttpPost]
    public IActionResult UploadExcel()
    {
        var excelFile = Request.Form.Files[0];
        string Currency = Request.Form.Where(x => x.Key == "Currency").FirstOrDefault().Value;
        var GetExhange = GetCurrency(Currency);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<ItemDetail> data = new List<ItemDetail>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var itemCode = _ISaleOrder.GetItemCode(worksheet.Cells[row, 3].Value.ToString());
                var partcode = 0;
                var itemCodeValue = 0;
                if (itemCode.Result.Result != null)
                {
                    partcode = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                    itemCodeValue = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                }
                if (partcode == 0)
                {
                    return Json("Partcode not available");
                }

                JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 10].Value) * AltRateToken;

                data.Add(new ItemDetail()
                {
                    SeqNo = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                    PartCode = partcode,
                    ItemCode = itemCodeValue,
                    PartText = worksheet.Cells[row, 3].Value.ToString(),
                    ItemText = worksheet.Cells[row, 4].Value.ToString(),
                    HSNNo = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString()),
                    Qty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
                    Unit = worksheet.Cells[row, 7].Value.ToString(),
                    AltQty = Convert.ToDecimal(worksheet.Cells[row, 8].Value.ToString()),
                    AltUnit = worksheet.Cells[row, 9].Value == null ? "" : worksheet.Cells[row, 9].Value.ToString(),
                    Rate = Convert.ToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                    OtherRateCurr = RateInOther,
                    UnitRate = worksheet.Cells[row, 12].Value.ToString(),
                    DiscPer = Convert.ToDecimal(worksheet.Cells[row, 13].Value.ToString()),
                    DiscRs = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                    Amount = Convert.ToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                    TolLimit = Convert.ToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                    Description = worksheet.Cells[row, 17].Value == null ? "" : worksheet.Cells[row, 17].Value.ToString(),
                    Remark = worksheet.Cells[row, 18].Value == null ? "" : worksheet.Cells[row, 18].Value.ToString(),
                    AmendmentNo = worksheet.Cells[row, 19].Value == null ? "" : worksheet.Cells[row, 19].Value.ToString(),
                    AmendmentDate = worksheet.Cells[row, 20].Value.ToString(),
                    AmendmentReason = worksheet.Cells[row, 21].Value == null ? "" : worksheet.Cells[row, 21].Value.ToString(),
                    Color = worksheet.Cells[row, 22].Value == null ? "" : worksheet.Cells[row, 22].Value.ToString(),
                    Rejper = Convert.ToDecimal(worksheet.Cells[row, 23].Value) == null ? 0 : Convert.ToDecimal(worksheet.Cells[row, 23].Value),
                    Excessper = Convert.ToInt32(worksheet.Cells[row, 24].Value) == null ? 0 : Convert.ToInt32(worksheet.Cells[row, 24].Value),
                    ProjQty1 = Convert.ToDecimal(worksheet.Cells[row, 25].Value.ToString()),
                    ProjQty2 = Convert.ToDecimal(worksheet.Cells[row, 26].Value.ToString()),
                });
            }
        }


        var MainModel = new SaleOrderModel();
        var POItemGrid = new List<ItemDetail>();
        var POGrid = new List<ItemDetail>();
        var SSGrid = new List<ItemDetail>();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        var seqNo = 0;

        _MemoryCache.Remove("ItemList");

        foreach (var item in data)
        {
            _MemoryCache.TryGetValue("ItemList", out SaleOrderModel Model);

            if (item != null)
            {

                if (Model == null)
                {
                    item.SeqNo += seqNo + 1;
                    POItemGrid.Add(item);
                    seqNo++;
                }
                else
                {
                    if (Model.ItemDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        item.SeqNo = Model.ItemDetailGrid.Count + 1;
                        POItemGrid = Model.ItemDetailGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(POItemGrid);
                        POItemGrid.Add(item);
                    }
                }
                MainModel.ItemDetailGrid = POItemGrid;

                _MemoryCache.Set("ItemList", MainModel, cacheEntryOptions);
                HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
            }
        }
        _MemoryCache.TryGetValue("ItemList", out SaleOrderModel MainModel1);

        return PartialView("_SaleItemGrid", MainModel);
    }

}
