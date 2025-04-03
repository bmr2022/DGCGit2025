using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.DotNet.MSIdentity.Shared;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace eTactWeb.Controllers;

[Authorize]
public class SaleScheduleController : Controller
{
    public SaleScheduleController(ISaleSchedule iSaleSchedule, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<SaleOrderController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
    {
        ISaleSchedule = iSaleSchedule;
        IDataLogic = iDataLogic;
        IMemoryCache = iMemoryCache;
        Logger = logger;
        EncryptDecrypt = encryptDecrypt;
        IWebHostEnvironment = iWebHostEnvironment;
    }

    public IDataLogic IDataLogic { get; }
    public IMemoryCache IMemoryCache { get; }
    public ISaleSchedule ISaleSchedule { get; }
    public IWebHostEnvironment IWebHostEnvironment { get; }
    public ILogger<SaleOrderController> Logger { get; }
    private EncryptDecrypt EncryptDecrypt { get; }

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

        //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

    }

    public IActionResult AddScheduleDetail(SaleScheduleGrid model)
    {
        try
        {
            IMemoryCache.TryGetValue("KeySaleScheduleGrid", out IList<SaleScheduleGrid> SaleScheduleGrid);

            var MainModel = new SaleSubScheduleModel();
            var SaleGrid = new List<SaleScheduleGrid>();
            var SSGrid = new List<SaleScheduleGrid>();
            model.DeliveryDate = ParseDate(model.DeliveryDate).ToString();

            if (model != null)
            {
                if (SaleScheduleGrid == null)
                {
                    model.SeqNo = 1;
                    SaleGrid.Add(model);
                }
                else
                {
                    if (SaleScheduleGrid.Any(x => x.ItemCode == model.ItemCode && ParseDate(x.DeliveryDate) == ParseDate(model.DeliveryDate)))
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        model.SeqNo = SaleScheduleGrid.Count + 1;
                        SaleGrid = SaleScheduleGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(SaleGrid);
                        SaleGrid.Add(model);
                    }
                }

                MainModel.SaleScheduleList = SaleGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);
            }
            else
            {
                ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
            }

            return PartialView("_SaleScheduleGrid", MainModel);
        }
        catch (Exception ex)
        {
            throw ex;
        }
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

    public async Task<JsonResult> EditItemRows(int SeqNo)
    {
        var MainModel = new SaleSubScheduleModel();
        IMemoryCache.TryGetValue("KeySaleScheduleGrid", out List<SaleScheduleGrid> SaleScheduleGrid);
        var SSGrid = SaleScheduleGrid.Where(x => x.SeqNo == SeqNo);
        string JsonString = JsonConvert.SerializeObject(SSGrid);
        return Json(JsonString);
    }

    public IActionResult AddMonthlyEntryData(List<SaleScheduleGrid> model)
    {
        try
        {

            var MainModel = new SaleSubScheduleModel();
            var SaleGrid = new List<SaleScheduleGrid>();
            var SSGrid = new List<SaleScheduleGrid>();


            var SeqNo = 0;
            foreach (var item in model)
            {
                IMemoryCache.TryGetValue("KeySaleScheduleGrid", out IList<SaleScheduleGrid> SaleScheduleGrid);

                if (model != null)
                {
                    item.DeliveryDate = ParseDate(item.DeliveryDate).ToString();
                    if (SaleScheduleGrid == null)
                    {
                        item.SeqNo += SeqNo + 1;
                        SaleGrid.Add(item);
                    }
                    else
                    {
                        if (SaleScheduleGrid.Any(x => x.ItemCode == item.ItemCode && ParseDate(x.DeliveryDate) == ParseDate(item.DeliveryDate)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            item.SeqNo = SaleScheduleGrid.Count + 1;
                            SaleGrid = SaleScheduleGrid.Where(x => x != null).ToList();
                            SSGrid.AddRange(SaleGrid);
                            SaleGrid.Add(item);
                        }
                    }
                    MainModel.SaleScheduleList = SaleGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);

                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
            }


            return PartialView("_SaleScheduleGrid", MainModel);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public IActionResult ClearGrid()
    {
        IMemoryCache.Remove("KeySaleScheduleGrid");
        var MainModel = new SaleSubScheduleModel();
        return PartialView("_SaleScheduleGrid", MainModel);
    }

    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await ISaleSchedule.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetFormRightsAmen()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await ISaleSchedule.GetFormRightsAmen(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<IActionResult> Dashboard()
    {
        IMemoryCache.Remove("KeySaleScheduleGrid");
        var model = new SSDashboard();
        var Result = await ISaleSchedule.GetDashboardData();

        if (Result != null)
        {
            var _List = new List<TextValue>();
            DataSet DS = Result.Result;

            //var DSLst = (from r in DS.Tables[0].AsEnumerable()
            //select r["EntryID"]).Distinct().ToList();

            //var results = from p in DS.Tables[0].AsEnumerable()
            //select p["SONO"];

            //var pp = DS.Tables[0].AsEnumerable().Distinct(System.Data.DataRowComparer.Default).ToList();

            //var distinctRows = (from DataRow dRow in DS.Tables[0].Rows
            //                    select dRow["EntryID"]).Distinct();

            //var CC = DS.Tables[0].AsEnumerable().Select(m => m.Table.Columns["EntryID"]);

            //var distinctValues = DS.Tables[0].AsEnumerable()
            //            .Select(row => new
            //            {
            //                EntryID = row.Field<string>("EntryID"),
            //                SONO = row.Field<string>("SONO")
            //            })
            //            .Distinct();

            var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryID", "EntryDate", "SONO", "AccountCode", "CustomerName", "CustomerOrderNo", "DeliveryAddress",
            "SchApproved", "SchAmendApprove", "SODate", "SchNo", "SchDate", "SchYear", "CreatedBYName", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "SOCloseDate",
            "SchCompleted", "SchClosed", "SchAmendNo", "CreatedBy", "CreatedOn", "ApprovedBy", "ModeOFTransport", "TentetiveConfirm",
            "OrderPriority", "CC", "UpdatedByName", "UpdatedOn", "EntryByMachineName");

            model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(DT, "SaleSchedule");

            foreach (var row in DS.Tables[0].AsEnumerable())
            {
                _List.Add(new TextValue()
                {
                    Text = row["SONO"].ToString(),
                    Value = row["SONO"].ToString()
                });
            }
            //var dd = _List.Select(x => x.Value).Distinct();
            var _SONOList = _List.DistinctBy(x => x.Value).ToList();
            model.SONOList = _SONOList;
            _List = new List<TextValue>();

            model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/"); // 1st Feb this year
            model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");//.AddDays(-1); // Last day in January next year
            model.SummaryDetail = "Summary";
        }

        return View(model);
    }

    public async Task<IActionResult> DeleteByID(int ID, int YC)
    {
        var Result = await ISaleSchedule.DeleteByID(ID, YC).ConfigureAwait(false);

        if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult DeleteItemRow(int SeqNo)
    {
        var MainModel = new SaleSubScheduleModel();
        IMemoryCache.TryGetValue("KeySaleScheduleGrid", out List<SaleScheduleGrid> SaleScheduleGrid);
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (SaleScheduleGrid != null && SaleScheduleGrid.Count > 0)
        {
            SaleScheduleGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (var item in SaleScheduleGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.SaleScheduleList = SaleScheduleGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);
        }
        return PartialView("_SaleScheduleGrid", MainModel);
    }

    public async Task<IActionResult> GetSearchData(SSDashboard model)
    {
        var Result = await ISaleSchedule.GetSearchData(model);

        if (model.SummaryDetail == "Summary")
        {
            var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "EntryDate", "SONO", "AccountCode", "CustomerName", "CustomerOrderNo", "DeliveryAddress",
                "SchApproved", "SchAmendApprove", "SODate", "SchNo", "SchDate", "SchYear", "CreatedBYName", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "SOCloseDate",
                "SchCompleted", "SchClosed", "SchAmendNo", "CreatedBy", "CreatedOn", "ApprovedBy", "ModeOFTransport", "TentetiveConfirm",
                "OrderPriority", "CC", "UpdatedByName", "UpdatedOn", "EntryByMachineName");

            model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(DT, "SaleSchedule");
        }
        else
        {
            var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "EntryDate", "SONO", "AccountCode", "CustomerName", "CustomerOrderNo", "DeliveryAddress",
             "SchApproved", "SchAmendApprove", "SODate", "SchNo", "SchDate", "SchYear", "CreatedBYName", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "SOCloseDate",
             "SchCompleted", "SchClosed", "SchAmendNo", "CreatedBy", "CreatedOn", "ApprovedBy", "ModeOFTransport", "TentetiveConfirm",
             "OrderPriority", "CC", "UpdatedByName", "UpdatedOn", "EntryByMachineName", "ItemCode", "ItemName", "PartCode", "Unit", "SchQty",
             "PendQty", "AltUnit", "AltPendQty", "Rate", "RateInOthCurr", "DeliveryDate", "ItemSize", "ItemColor", "OtherDetail", "Remarks");

            model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(DT, "SaleScheduleDetail");
        }

        return PartialView("_SSDashboardGrid", model);
    }

    public async Task<IActionResult> GetSODATA(int AccountCode, int SONO, int Year)
    {
        var JSONString = await ISaleSchedule.GetSODATA(AccountCode, SONO, Year);
        return Json(JSONString);
    }

    public async Task<IActionResult> GetSOItem(int AccountCode, int SONO, int Year, int ItemCode)
    {
        var JSONString = await ISaleSchedule.GetSOItem(AccountCode, SONO, Year, ItemCode);

        //var Dlist = JsonConvert.DeserializeObject<Dictionary<object, object>>(JSONString);
        //JObject json = JObject.Parse(JSONString);
        //object obj = JsonConvert.DeserializeObject(JSONString, typeof(object));
        //JToken jToken = (JToken)json;

        //object value = "";
        //Dlist.TryGetValue(key: "Result", out value);

        return Json(JSONString);
    }

    public async Task<IActionResult> GetSONO(int AccountCode)
    {
        var JSONString = await ISaleSchedule.GetSONO(AccountCode);
        return Json(JSONString);
    }

    public async Task<IActionResult> GetSOYear(int AccountCode, int SONO)
    {
        var JSONString = await ISaleSchedule.GetSOYear(AccountCode, SONO);
        return Json(JSONString);
    }

    public async Task<JsonResult> NewEntryId(int YearCode)
    {
        var JSON = await ISaleSchedule.NewEntryId(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> NewAmmEntryId()
    {
        var JSON = await ISaleSchedule.NewAmmEntryId();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAltUnitQty(int ItemCode, float AltSchQty, float UnitQty)
    {
        var JSON = await ISaleSchedule.GetAltUnitQty(ItemCode, AltSchQty, UnitQty);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    // GET: SaleScheduleController/SaleScheduleForm
    [Route("{controller}/Index")]
    public async Task<ActionResult> SaleSchedule(int ID, string Mode, int YC)
    {
        Logger.LogInformation("\n \n ********** Page Sale Schedule ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
        TempData.Clear();
        var MainModel = new SaleSubScheduleModel();
        IMemoryCache.Remove("KeySaleScheduleGrid");
        //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        if (Mode != "SSA")
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        else
        {
            MainModel.YearCode = YC;
            MainModel.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }
        MainModel.CC = HttpContext.Session.GetString("Branch");
        if (Mode != "U")
        {
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedOn = DateTime.Now;
        }

        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U" || Mode == "SSA"))
        {
            MainModel = await ISaleSchedule.GetViewByID(ID, YC, Mode);
            MainModel.Mode = Mode;
            MainModel.ID = ID;
            MainModel = await BindModel(MainModel);

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);
        }
        else
        {
            MainModel = await BindModel(MainModel);
        }
        return View(MainModel);
    }

    public async Task<IActionResult> GetAmmSearchData(SSDashboard model)
    {
        model = await ISaleSchedule.GetAmmSearchData(model);
        model.Mode = "Pending";
        return PartialView("_SSAmmListGrid", model);
    }

    public async Task<IActionResult> GetUpdAmmData(SSDashboard model)
    {
        model = await ISaleSchedule.GetUpdAmmData(model);
        model.Mode = "U";
        return PartialView("_SSAmmListGrid", model);
    }
    public async Task<IActionResult> GetAmmCompSearchData(SSDashboard model)
    {
        model.Mode = "SSAMMCOMPLETED";
        var Result = await ISaleSchedule.GetSearchData(model);

        var DT = Result.Result.DefaultView.ToTable(true, "EntryID", "SONO", "AccountCode", "CustomerName", "DeliveryAddress",
                         "SchApproved", "SODate", "SchNo", "SchDate", "SchYear", "SOYearCode", "CustomerOrderNo", "SOCloseDate", "SchCompleted", "SchAmendNo", "CreatedBYName",
                        "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");

        model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(DT, "SaleScheduleComp");
        model.Mode = "Completed";

        return PartialView("_SSAmmListGrid", model);
    }

    public async Task<ActionResult> ViewSSCompleted(string Mode, int ID, int YC, int SONO)
    {
        var model = new SaleSubScheduleModel();

        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "SSC"))
        {
            model = await ISaleSchedule.GetViewSSCcompletedByID(ID, YC, SONO, Mode).ConfigureAwait(true);

            model.Mode = Mode;
            model = await BindModel(model);

            model.ID = ID;

            if (model.SaleScheduleList?.Count != 0 && model.SaleScheduleList != null)
            {
                IMemoryCache.Set("KeySaleScheduleGrid", model.SaleScheduleList);
            }
        }
        else
        {
            model = await BindModel(null);
            IMemoryCache.Remove("ItemList");
        }

        return View("SaleSchedule", model);
    }

    [HttpGet, Route("{controller}/SSAmendmentList")]
    public async Task<IActionResult> SSAmendmentList()
    {
        IMemoryCache.Remove("KeySaleScheduleGrid");
        var model = new SSDashboard();
        string toDt = HttpContext.Session.GetString("ToDate");
        var Result = await ISaleSchedule.GetAmmDashboardData(toDt);
        if (Result != null)
        {
            var _List = new List<TextValue>();
            DataSet DS = Result.Result;

            var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryID", "EntryDate", "SONO", "AccountCode", "CustomerName", "CustomerOrderNo", "DeliveryAddress",
            "SchApproved", "SchAmendApprove", "SODate", "SchNo", "SchDate", "SchYear", "CreatedBYName", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "SOCloseDate",
            "SchCompleted", "SchClosed", "SchAmendNo", "CreatedBy", "CreatedOn", "ApprovedBy", "ModeOFTransport", "TentetiveConfirm",
            "OrderPriority", "CC", "UpdatedByName", "UpdatedOn", "EntryByMachineName");


            model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(DT, "SaleSchedule");

            foreach (var row in DS.Tables[0].AsEnumerable())
            {
                _List.Add(new TextValue()
                {
                    Text = row["SONO"].ToString(),
                    Value = row["SONO"].ToString()
                });
            }
            //var dd = _List.Select(x => x.Value).Distinct();
            var _SONOList = _List.DistinctBy(x => x.Value).ToList();
            model.SONOList = _SONOList;
            _List = new List<TextValue>();
            model.Mode = "Pending";

            model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/"); // 1st Feb this year
            model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");//.AddDays(-1); // Last day in January next year
        }

        return View(model);
    }

    [HttpPost, Route("{controller}/SSAmendment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SSAmendment(SaleSubScheduleModel model)
    {
        try
        {
            bool isError = true;
            DataSet DS = new();
            DataTable ItemDetailDT = null;
            ResponseResult Result = new();
            Dictionary<string, string> ErrList = new();

            var AmmStatus = await ISaleSchedule.GetAmmStatus(model.EntryID, model.YearCode).ConfigureAwait(true);
            var DTSSGrid = new DataTable();
            IMemoryCache.TryGetValue("KeySaleScheduleGrid", out IList<SaleScheduleGrid> SaleScheduleGrid);

            ModelState.Clear();


            if (SaleScheduleGrid == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("SaleScheduleGrid", "Sale Schedule Grid Should Have Atlease 1 Item...!");
                model = await BindModel(model);
                return View("SaleSchedule", model);
            }
            else
            {
                //  model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                foreach (var items in SaleScheduleGrid)
                {
                    items.Mode = model.Mode;
                }

                if (model.Mode == "UPDATE")
                {
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    DTSSGrid = GetDetailTable(SaleScheduleGrid);
                }
                else
                {
                    DTSSGrid = GetDetailTable(SaleScheduleGrid);
                }

                if (AmmStatus == null)
                {
                    ErrList.Add("Amm Status", "Amendment not possible...!");
                    isError = true;
                }
                if (SaleScheduleGrid.Count > 0)
                {
                    model.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    Result = await ISaleSchedule.SaveSaleSchedule(model, DTSSGrid);
                }
                if (!isError)
                {
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
                        return RedirectToAction(nameof(SSAmendmentList));
                    }
                    else
                    {
                        model = await BindModel(model);

                        foreach (KeyValuePair<string, string> Err in ErrList)
                        {
                            ModelState.AddModelError(Err.Key, Err.Value);
                        }
                    }
                }
                else
                {
                    model = await BindModel(model);

                    foreach (KeyValuePair<string, string> Err in ErrList)
                    {
                        ModelState.AddModelError(Err.Key, Err.Value);
                    }

                    return View("SaleSchedule", model);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogInformation("Error In SO Amendment");
            Logger.LogError("\n \n" + ex, ex.Message, model);
        }

        return RedirectToAction(nameof(SSAmmCompleted));
    }

    public async Task<JsonResult> GetallData(int ID, int YC)
    {
        var JSON = await ISaleSchedule.GetAllData(ID, YC);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    // POST: SaleScheduleController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{controller}/Index")]
    public async Task<IActionResult> SaleSchedule(SaleSubScheduleModel model)
    {
        try
        {
            //ViewBag.isSuccess = false;
            var DTSSGrid = new DataTable();
            IMemoryCache.TryGetValue("KeySaleScheduleGrid", out IList<SaleScheduleGrid> SaleScheduleGrid);

            if (SaleScheduleGrid == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("SaleScheduleGrid", "Sale Schedule Grid Should Have Atlease 1 Item...!");
                model = await BindModel(model);
                return View("SaleSchedule", model);
            }
            else
            {
                //model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";
                if (model.Mode == "U")
                {
                    model.Mode = "UPDATE";
                }
                else if (model.Mode == "SSA")
                {
                    model.Mode = "SSA";
                }
                else
                {
                    model.Mode = "INSERT";
                }
                // model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                foreach (var items in SaleScheduleGrid)
                {
                    items.Mode = model.Mode;
                }

                if (model.Mode == "UPDATE")
                {
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    DTSSGrid = GetDetailTable(SaleScheduleGrid);
                }
                else
                {
                    DTSSGrid = GetDetailTable(SaleScheduleGrid);
                }

                //DTSSGrid = CommonFunc.ConvertListToTable(SaleScheduleGrid);
                //DTSSGrid.Columns.Remove("ID");
                //DTSSGrid.Columns.Remove("Mode");
                //DTSSGrid.Columns.Remove("SeqNo");
                //DTSSGrid.Columns.Remove("Active");
                //DTSSGrid.Columns.Remove("Browser");
                //DTSSGrid.Columns.Remove("ItemName");
                //DTSSGrid.Columns.Remove("PartCode");
                //DTSSGrid.Columns.Remove("CreatedBy");
                //DTSSGrid.Columns.Remove("CreatedOn");
                //DTSSGrid.Columns.Remove("IPAddress");
                //DTSSGrid.Columns.Remove("UpdatedBy");
                //DTSSGrid.Columns.Remove("UpdatedOn");

                var Result = await ISaleSchedule.SaveSaleSchedule(model, DTSSGrid);

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
                            var MainModel = new SaleSubScheduleModel();
                            IMemoryCache.Remove("KeySaleScheduleGrid");
                            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            MainModel.CC = HttpContext.Session.GetString("Branch");
                            MainModel = await BindModel(MainModel);
                            if (model.Mode == "UPDATE")
                            {
                                ViewBag.isSuccess = true;
                                TempData["202"] = "202";
                                return RedirectToAction("SaleSchedule");
                            }
                            else if (model.Mode == "SSA")
                            {
                                return RedirectToAction(nameof(SSAmendmentList));

                            }
                            else
                            {
                                ViewBag.isSuccess = true;
                                TempData["200"] = "200";
                                //var MainModel1 = new SaleSubScheduleModel();
                                //MainModel1 = await BindModel(MainModel1);
                                //return View(MainModel1);
                                return RedirectToAction("SaleSchedule");
                            }


                            //  return RedirectToAction(nameof(Dashboard));
                        }
                        else
                        {
                            dynamic jsonObj = JsonConvert.DeserializeObject(stringResponse);
                            if (jsonObj.Result != null && jsonObj.Result.Count > 0)
                            {
                                int resultValue = jsonObj.Result[0].Result;
                                int YearCodeVal = jsonObj.Result[0].YearCode;
                                return RedirectToAction("SaleSchedule", new { ID = resultValue, YC = YearCodeVal, Mode = "U" });
                            }
                            else
                            {
                                TempData["500"] = "500";
                                //return RedirectToAction(nameof(Dashboard));
                                //ModelState.AddModelError("ItemDetailGrid", "Something went Wrong");
                            }
                        }
                    }
                }
                else
                {

                }
            }
            // return View(model);
        }
        catch (Exception ex)
        {
            LogException<SaleOrderController>.WriteException(Logger, ex);

            //Logger.Log(LogLevel.Information, "\n \n ********** Log ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogInformation("\n \n ********** LogInformation ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogCritical("\n \n ********** LogCritical ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogDebug("\n \n ********** LogDebug ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogTrace("\n \n ********** LogTrace ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            //Logger.LogWarning("\n \n ********** LogWarning ********** \n " + JsonConvert.SerializeObject(ex) + "\n");

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult UploadExcel()
    {
        IFormFile ExcelFile = Request.Form.Files[0];
        String ErrMsg = String.Empty;
        var ExcelDup = new List<string>();
        var SaleGrid = new SaleScheduleGrid();
        var MainModel = new SaleSubScheduleModel();
        var SaleGridList = new List<SaleScheduleGrid>();
        var Error = new Dictionary<string, string>();

        if (ExcelFile != null)
        {
            //Create a Folder.
            string path = Path.Combine(this.IWebHostEnvironment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Save the uploaded Excel file.
            string fileName = Path.GetFileName(ExcelFile.FileName);
            string filePath = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                ExcelFile.CopyTo(stream);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return BadRequest("Uploaded file does not contain any worksheet.");
                }
                else
                {
                    var AC = Convert.ToInt32(Request.Form.Where(x => x.Key == "AC").FirstOrDefault().Value, System.Globalization.CultureInfo.InvariantCulture);
                    var SONO = Convert.ToInt32(Request.Form.Where(x => x.Key == "SONO").FirstOrDefault().Value, System.Globalization.CultureInfo.InvariantCulture);
                    //var Year = Convert.ToInt32(Request.Form.Where(x => x.Key == "Year").FirstOrDefault().Value, new CultureInfo("en-IN"));
                    var yearString = Request.Form.FirstOrDefault(x => x.Key == "Year").Value;

                    var Year = 2025;//Convert.ToInt32(yearString, new CultureInfo("en-IN"));
                   
                    var FromDate = Convert.ToDateTime(Request.Form.Where(x => x.Key == "FromDate").FirstOrDefault().Value).ToString("dd/MM/yyyy");
                    var TillDate = Convert.ToDateTime(Request.Form.Where(x => x.Key == "TillDate").FirstOrDefault().Value).ToString("dd/MM/yyyy");
                    var Action = Request.Form.Where(x => x.Key == "Action").FirstOrDefault().Value;

                    var JSONString = ISaleSchedule.GetSOItem(AC, SONO, Year, 0).GetAwaiter().GetResult();

                    var ItemList = JsonConvert.DeserializeObject<Root>(JSONString);

                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {

                        bool isRowEmpty = true;
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            if (!string.IsNullOrEmpty((worksheet.Cells[row, col].Value ?? string.Empty).ToString().Trim()))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }

                        if (isRowEmpty)
                        {
                            // Row is empty, you can handle this case accordingly
                            ErrMsg = "Row : " + row + " is empty.";
                        }
                        else
                        {
                            var ExlPC = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();
                            var ExlDD = (worksheet.Cells[row, 3].Value ?? string.Empty).ToString().Trim();
                            var ExlDelDate = (worksheet.Cells[row, 4].Value ?? string.Empty).ToString().Trim();
                            var ExIC = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.ItemCode).FirstOrDefault();

                            DateTime DelDate;
                            var isDate = DateTime.TryParse(ExlDD, out DelDate);

                            var isValidPartCode = ItemList.Result.Where(x => x.PartCode == ExlPC).Any();

                            //var isValidDate = (Convert.ToDateTime(ExlDD).CompareTo(Convert.ToDateTime(FromDate)) > 0) && (Convert.ToDateTime(ExlDD).CompareTo(Convert.ToDateTime(TillDate)) < 1);

                            //var isValidDate = (DelDate.CompareTo(Convert.ToDateTime(FromDate)) > 0) && (DelDate.CompareTo(Convert.ToDateTime(TillDate)) < 1);

                            //if (!isValidPartCode && !isValidDate)
                            //{
                            //    ErrMsg = "Row : " + row + " has invalid Partcode & DeliveryDate.";
                            //}
                            //else if (!isValidPartCode && isValidDate)
                            //{
                            //    ErrMsg = "Row : " + row + " has invalid Partcode.";
                            //}
                            //else if (isValidPartCode && !isValidDate)
                            //{
                            //    ErrMsg = "Row : " + row + " has invalid DeliveryDate.";
                            //}
                            //else if (ExcelDup.Contains(ExlPC))
                            //{
                            //    ErrMsg = "Row : " + row + " has Duplicate PartCode.";
                            //}
                            //else
                            //{
                            //    ExcelDup.Add(ExlPC);
                            //}

                            //var DelDateValidate = SaleGridList.Where(x => x.DeliveryDate == ExlDelDate).Any();
                            var EffFromDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
                            var EffTillDate = DateTime.ParseExact(TillDate, "dd/MM/yyyy", null);

                            if (!isValidPartCode)
                            {
                                //  ErrorMsg = "PartCode does not Exists";
                                ErrMsg = "PartCode does not Exists";
                            }
                            else if ((Convert.ToDateTime(ExlDelDate) >= EffFromDate && Convert.ToDateTime(ExlDelDate) <= EffTillDate))
                            {
                                ErrMsg = "Row : " + row + "has OutofRange DeliveryDate.";
                            }
                            else
                            {
                                ExcelDup.Add(ExlPC);
                            }

                            if (!isValidPartCode /*|| !isValidDate*/ || !string.IsNullOrEmpty(ErrMsg))
                            {
                                Error.Add(row.ToString(), ErrMsg);
                                //TempData["500"] = "500";

                            }
                            else if (SaleGridList.Any(x => x.ItemCode == ExIC && ParseDate(x.DeliveryDate) == ParseDate(ExlDelDate)))
                            {
                                ErrMsg = "Row : " + row + " has Duplicate Items.";
                            }
                            else
                            {
                                SaleGridList.Add(new SaleScheduleGrid
                                {
                                    SeqNo = SaleGridList.Count + 1,
                                    ItemCode = ExIC,
                                    ItemName = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.ItemName).FirstOrDefault(),
                                    PartCode = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim(),
                                    SchQty = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                                    PendQty = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                                    Unit = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.Unit).FirstOrDefault(),
                                    AltUnit = ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.AltUnit).FirstOrDefault(),
                                    Rate = Convert.ToDecimal(ItemList.Result.Where(c => c.PartCode == ExlPC).Select(x => x.Rate).FirstOrDefault()),
                                    DeliveryDate = ParseDate(ExlDelDate).ToString(),
                                    AltSchQty = (worksheet.Cells[row, 5].Value ?? string.Empty).ToString().Trim() == "" ? 0 : Convert.ToDecimal((worksheet.Cells[row, 5].Value ?? string.Empty).ToString().Trim()),
                                    ItemSize = (worksheet.Cells[row, 6].Value ?? string.Empty).ToString().Trim(),
                                    ItemColor = (worksheet.Cells[row, 7].Value ?? string.Empty).ToString().Trim(),
                                    Remarks = (worksheet.Cells[row, 8].Value ?? string.Empty).ToString().Trim(),
                                    OtherDetail = (worksheet.Cells[row, 9].Value ?? string.Empty).ToString().Trim(),
                                });
                            }

                        }
                    }
                }
            }

            ////Read the connection string for the Excel file.
            //string conString = this.Configuration.GetConnectionString("ExcelConString");
            //DataTable dt = new DataTable();
            //conString = string.Format(conString, filePath);

            //using (OleDbConnection connExcel = new OleDbConnection(conString))
            //{
            //    using (OleDbCommand cmdExcel = new OleDbCommand())
            //    {
            //        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
            //        {
            //            cmdExcel.Connection = connExcel;

            // //Get the name of First Sheet. connExcel.Open(); DataTable dtExcelSchema;
            // dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null); string
            // sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString(); connExcel.Close();

            //            //Read Data from First Sheet.
            //            connExcel.Open();
            //            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
            //            odaExcel.SelectCommand = cmdExcel;
            //            odaExcel.Fill(dt);
            //            connExcel.Close();
            //        }
            //    }
            //}

            ////Insert the Data read from the Excel file to Database Table.
            //conString = this.Configuration.GetConnectionString("DBConnection");
            //using (SqlConnection con = new SqlConnection(conString))
            //{
            //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
            //    {
            //        //Set the database table name.
            //        sqlBulkCopy.DestinationTableName = "dbo.customersDB";

            // //[OPTIONAL]: Map the Excel columns with that of the database table.
            // sqlBulkCopy.ColumnMappings.Add("CustomerId", "customer_id");
            // sqlBulkCopy.ColumnMappings.Add("FirstName", "first_name");
            // sqlBulkCopy.ColumnMappings.Add("LastName", "last_name");
            // sqlBulkCopy.ColumnMappings.Add("Phone", "phone");
            // sqlBulkCopy.ColumnMappings.Add("Email", "email");
            // sqlBulkCopy.ColumnMappings.Add("Street", "street");
            // sqlBulkCopy.ColumnMappings.Add("City", "city");
            // sqlBulkCopy.ColumnMappings.Add("State", "state");
            // sqlBulkCopy.ColumnMappings.Add("Zip", "zip_code");

            //        con.Open();
            //        sqlBulkCopy.WriteToServer(dt);
            //        con.Close();
            //    }
            //}
        }

        if (Error.Count > 0)
        {
            return StatusCode(207, Error);
        }
        else
        {
            MainModel.SaleScheduleList = SaleGridList;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);

            return PartialView("_SaleScheduleGrid", MainModel);
        }

        return View();
    }

    [HttpPost]
    [Route("{controller}/UploadMonthlyExcel")]
    public IActionResult UploadMonthlyExcel()
    {
        IFormFile ExcelFile = Request.Form.Files[0];
        String ErrMsg = String.Empty;
        var ExcelDup = new List<string>();
        var SaleGrid = new SaleScheduleGrid();
        var MainModel = new SaleSubScheduleModel();
        var SaleGridList = new List<SaleScheduleGrid>();
        var Error = new Dictionary<string, string>();

        if (ExcelFile != null)
        {
            // Create a Folder.
            string path = Path.Combine(this.IWebHostEnvironment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Save the uploaded Excel file.
            string fileName = Path.GetFileName(ExcelFile.FileName);
            string filePath = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                ExcelFile.CopyTo(stream);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(filePath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    return BadRequest("Invalid Excel File.");
                }

                var rowCount = worksheet.Dimension.Rows;

                // Dictionary to store column mappings
                Dictionary<string, int> columnMapping = new Dictionary<string, int>();

                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    string header = (worksheet.Cells[1, col].Value ?? string.Empty).ToString().Trim();

                    if (!string.IsNullOrEmpty(header))
                    {
                        // Check if the header is a number
                        if (int.TryParse(header, out _))
                        {
                            header = "Exl" + header; // Replace numeric headers with "Exl" + original number
                        }

                        columnMapping[header] = col; // Store the modified header name and column index
                    }
                }


                // Initialize DataTable
                DataTable dt = new DataTable();

                // Add columns dynamically from headers
                foreach (var header in columnMapping.Keys)
                {
                    dt.Columns.Add(header);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    bool isRowEmpty = true;
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        if (!string.IsNullOrEmpty((worksheet.Cells[row, col].Value ?? string.Empty).ToString().Trim()))
                        {
                            isRowEmpty = false;
                            break;
                        }
                    }

                    if (isRowEmpty)
                    {
                        ErrMsg = "Row : " + row + " is empty.";
                        continue;
                    }

                    var ExlPC = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();

                    int AC = Convert.ToInt32(Request.Form["AC"]);
                    int SONO = Convert.ToInt32(Request.Form["SONO"]);
                    int Year = Convert.ToInt32(Request.Form["Year"], new CultureInfo("en-IN"));
                    string FromDate = CommonFunc.ParseFormattedDate(Request.Form["FromDate"]);
                    //Convert.ToDateTime(Request.Form["FromDate"]).ToString("dd/MM/yyyy");
                    string TillDate = CommonFunc.ParseFormattedDate(Request.Form["TillDate"]);
                        //Convert.ToDateTime(Request.Form["TillDate"]).ToString("dd/MM/yyyy");

                    var JSONString = ISaleSchedule.GetSOItem(AC, SONO, Year, 0).GetAwaiter().GetResult();
                    var ItemList = JsonConvert.DeserializeObject<Root>(JSONString);
                    var ItemDetails = ItemList?.Result?.FirstOrDefault(c => c.PartCode == ExlPC);
                    if (ItemDetails == null)
                    {
                        ErrMsg = $"PartCode does not exist.{ExlPC}";
                        Error.Add(row.ToString(), ErrMsg);
                        continue;
                    }

                    // Create a new row
                    DataRow dr = dt.NewRow();
                    dr["Part Code"] = ExlPC;

                    foreach (var kvp in columnMapping)
                    {
                        var value = worksheet.Cells[row, kvp.Value].Value ?? 0;
                        dr[kvp.Key] = value;
                    }

                    dt.Rows.Add(dr);

                    // Add to Sale Grid List
                    SaleGridList.Add(new SaleScheduleGrid
                    {
                        SeqNo = SaleGridList.Count + 1,
                        ItemCode = ItemDetails.ItemCode,
                        ItemName = ItemDetails.ItemName,
                        PartCode = ExlPC,
                        Unit = ItemDetails.Unit,
                        AltUnit = ItemDetails.AltUnit,
                        Rate = Convert.ToDecimal(ItemDetails.Rate),
                    });
                }

                if (Error.Count > 0)
                {
                    return StatusCode(207, Error);
                }
                else
                {
                    // Convert DataTable to a List of Dictionary for JSON serialization
                    var dtList = dt.AsEnumerable().Select(row =>
                        dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])).ToList();

                    // Create an object containing both DataTable data and SaleGridList
                    var responseObject = new
                    {
                        SaleGridList = SaleGridList,
                        DataTableData = dtList
                    };

                    // Convert to JSON
                    var jsonString = JsonConvert.SerializeObject(responseObject, Formatting.Indented);

                    return Json(jsonString);
                }
            }
        }

        return BadRequest("No file uploaded.");
    }

    private bool IsRowEmpty(ExcelWorksheet worksheet, int row, int totalCols)
    {
        for (int col = 1; col <= totalCols; col++)
        {
            if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Value?.ToString().Trim()))
            {
                return false;
            }
        }
        return true;
    }

    // Safely convert cell value to integer
    private int ConvertToInt(object value)
    {
        return value == null || string.IsNullOrWhiteSpace(value.ToString()) ? 0 : Convert.ToInt32(value);
    }

    // Safely convert cell value to decimal
    private decimal ConvertToDecimal(object value)
    {
        return value == null || string.IsNullOrWhiteSpace(value.ToString()) ? 0 : Convert.ToDecimal(value);
    }
    private static DataTable GetDetailTable(IList<SaleScheduleGrid> DetailList)
    {
        var DTSSGrid = new DataTable();

        DTSSGrid.Columns.Add("ItemCode", typeof(int));
        DTSSGrid.Columns.Add("Unit", typeof(string));
        DTSSGrid.Columns.Add("SchQty", typeof(decimal));
        DTSSGrid.Columns.Add("PendQty", typeof(decimal));
        DTSSGrid.Columns.Add("AltUnit", typeof(string));
        DTSSGrid.Columns.Add("AltSchQty", typeof(decimal));
        DTSSGrid.Columns.Add("AltPendQty", typeof(decimal));
        DTSSGrid.Columns.Add("Rate", typeof(decimal));
        DTSSGrid.Columns.Add("RateInOthCurr", typeof(decimal));
        DTSSGrid.Columns.Add("DeliveryDate", typeof(string));
        DTSSGrid.Columns.Add("ItemSize", typeof(string));
        DTSSGrid.Columns.Add("ItemColor", typeof(string));
        DTSSGrid.Columns.Add("OtherDetail", typeof(string));
        DTSSGrid.Columns.Add("Remarks", typeof(string));

        var DelDate = "";
        //DateTime DeliveryDt = new DateTime();
        foreach (var Item in DetailList)
        {
            if (string.IsNullOrEmpty(Item.DeliveryDate))
            {
                return default;
            }
            // String deliveryDt;
            if (DateTime.TryParseExact(Item.DeliveryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deliveryDt))
            {
                //  deliveryDt = ConvertToDesiredFormat(Item.DeliveryDate);
                deliveryDt = DateTime.ParseExact(Item.DeliveryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DelDate = deliveryDt.ToString("dd/MMM/yyyy");
                //  DelDate = deliveryDt;
            }
            else
            {
                deliveryDt = DateTime.Parse(Item.DeliveryDate);
                DelDate = deliveryDt.ToString("dd/MMM/yyyy");
            }
            DTSSGrid.Rows.Add(
                new object[]
                {
                    Item.ItemCode,
                    Item.Unit,
                    Item.SchQty,
                    Item.PendQty,
                    Item.AltUnit,
                    Item.AltSchQty,
                    Item.AltPendQty,
                    Item.Rate,
                    Item.RateInOthCurr,
                    DelDate == default ? string.Empty : DelDate,
                    Item.ItemSize,
                    Item.ItemColor,
                    Item.OtherDetail,
                    Item.Remarks,
                });
        }
        DTSSGrid.Dispose();
        return DTSSGrid;
    }

    private async Task<SaleSubScheduleModel> BindModel(SaleSubScheduleModel model)
    {
        var oDataSet = new DataSet();
        var _List = new List<TextValue>();
        oDataSet = await ISaleSchedule.BindAllDropDown();

        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in oDataSet.Tables[0].Rows)
            {
                _List.Add(new TextValue
                {
                    Value = row["Com_Name"].ToString(),
                    Text = row["Com_Name"].ToString()
                });
            }
            model.BranchList = _List;
            _List = new List<TextValue>();

            foreach (DataRow row in oDataSet.Tables[1].Rows)
            {
                _List.Add(new TextValue
                {
                    Value = row["AccountCode"].ToString(),
                    Text = row["AccountName"].ToString()
                });
            }
            model.AccountList = _List;
            _List = new List<TextValue>();

            foreach (DataRow row in oDataSet.Tables[2].Rows)
            {
                _List.Add(new TextValue
                {
                    Value = row["CustOrderNo"].ToString(),
                    Text = row["CustOrderNo"].ToString()
                });
            }
            model.CustomerOrderList = _List;
            _List = new List<TextValue>();
        }

        if (model.Mode == null && model.ID == 0)
        {
            //model.YearCode = Constants.FinincialYear;
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.EntryID = IDataLogic.GetEntryID("SaleScheduleMain", Constants.FinincialYear, "SaleSchEntryID", "Saleyearcode");
            //model.ScheduleNo = IDataLogic.GetEntryID("SaleScheduleMain", Constants.FinincialYear, "Entry_ID");
            model.EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            model.SchAmendmentDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            model.ScheduleDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            model.SchEffFromDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            model.SchEffTillDate = DateTime.Today.AddMonths(1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.CC = HttpContext.Session.GetString("Branch");
        }
        return model;
    }
    public async Task<JsonResult> FillCustomer(string SchEffFromDate)
    {
        var JSON = await ISaleSchedule.FillCustomer(SchEffFromDate);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    [HttpGet, Route("/SSAmendmentCompleted")]
    public async Task<IActionResult> SSAmmCompleted()
    {
        var _List = new List<TextValue>();
        IMemoryCache.Remove("KeySaleScheduleGrid");
        string toDt = HttpContext.Session.GetString("ToDate");
        var model = await ISaleSchedule.GetSOAmmCompleted(toDt).ConfigureAwait(true);
        model.Mode = "Completed";

        foreach (var item in model.SSDashboard)
        {
            TextValue _SONo = new()
            {
                Text = item.SONO.ToString(new CultureInfo("en-IN")),
                Value = item.SONO.ToString(new CultureInfo("en-IN")),
            };
            if (_List != null)
            {
                if (_List.Exists(x => x.Value == _SONo.Value))
                {
                    //do nothing
                }
                else
                {
                    _List.Add(_SONo);

                }
            }
            else
            {

                _List.Add(_SONo);
            }
        }
        model.SONoList = _List;

        model.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy", new CultureInfo("en-GB")); // 1st Feb this year
        model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy", new CultureInfo("en-GB"));//.AddDays(-1); // Last day in January next year

        return View(model);
    }

    public async Task<JsonResult> GetCurrency(string Flag)
    {
        var JSON = await ISaleSchedule.GetCurrency(Flag);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GetExchangeRate(string Currency)
    {
        var JSON = await ISaleSchedule.GetExchangeRate(Currency);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpPost]
    public IActionResult UploadExcelSaleSchedule()
    {
        var excelFile = Request.Form.Files[0];
        var Flag = Request.Form.Where(x => x.Key == "Flag").FirstOrDefault().Value;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<SaleScheduleGrid> data = new List<SaleScheduleGrid>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var itemCode = ISaleSchedule.GetItemCode(worksheet.Cells[row, 3].Value.ToString().Split(" ")[0]);
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
                decimal AltPendQTyValue = 0;
                decimal RateInOther = 0;
                var GetCurrencyval = GetCurrency(Flag);

                JObject AltPendQTy = JObject.Parse(GetCurrencyval.Result.Value.ToString());
                JToken recqtyToken = AltPendQTy["Result"]["Table"][0]["RECQTY"];
                decimal recqtyValue = recqtyToken.Value<decimal>();
                AltPendQTyValue = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()) - recqtyValue;
                if (AltPendQTyValue < 0)
                {
                    return Json("Not Done");
                }
                JObject AltCurrency = JObject.Parse(GetCurrencyval.Result.Value.ToString());
                JToken CurrencyToken = AltCurrency["Result"]["Table"][0]["Currency"];
                string Currency = CurrencyToken.Value<string>();
                if (Currency == null)
                {
                    //do nothing
                }
                else
                {
                    var GetExhange = GetExchangeRate(Currency);
                    JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                    decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                    RateInOther = Convert.ToDecimal(worksheet.Cells[row, 11].Value) * AltRateToken;
                }
                data.Add(new SaleScheduleGrid()
                {
                    SeqNo = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                    ItemCode = partcode,
                    ItemName = worksheet.Cells[row, 4].Value.ToString(),
                    PartCode = worksheet.Cells[row, 3].Value.ToString().Split(" ")[0],
                    Unit = worksheet.Cells[row, 5].Value.ToString(),
                    SchQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value) == null ? 0 : Convert.ToDecimal(worksheet.Cells[row, 6].Value),
                    PendQty = Convert.ToDecimal(AltPendQTyValue.ToString("F2")),
                    AltUnit = worksheet.Cells[row, 8].Value == null ? "" : worksheet.Cells[row, 8].Value.ToString(),
                    AltSchQty = Convert.ToDecimal(worksheet.Cells[row, 9].Value.ToString()),
                    AltPendQty = Convert.ToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                    Rate = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                    RateInOthCurr = RateInOther,
                    DeliveryDate = worksheet.Cells[row, 13].Value.ToString(),
                    ItemSize = worksheet.Cells[row, 14].Value == null ? "" : worksheet.Cells[row, 14].Value.ToString(),
                    ItemColor = worksheet.Cells[row, 15].Value == null ? "" : worksheet.Cells[row, 15].Value.ToString(),
                    OtherDetail = worksheet.Cells[row, 16].Value == null ? "" : worksheet.Cells[row, 16].Value.ToString(),
                    Remarks = worksheet.Cells[row, 17].Value == null ? "" : worksheet.Cells[row, 17].Value.ToString(),
                });

            }
        }

        var MainModel = new SaleSubScheduleModel();
        var saleScheduleGrid = new List<SaleScheduleGrid>();
        var POGrid = new List<SaleScheduleGrid>();
        var SSGrid = new List<SaleScheduleGrid>();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        var seqNo = 0;

        IMemoryCache.Remove("KeySaleScheduleGrid");

        foreach (var item in data)
        {
            IMemoryCache.TryGetValue("KeySaleScheduleGrid", out IList<SaleScheduleGrid> SaleScheduleGrid);

            if (item != null)
            {

                if (SaleScheduleGrid == null)
                {
                    item.SeqNo += seqNo + 1;
                    saleScheduleGrid.Add(item);
                    seqNo++;
                }
                else
                {
                    if (SaleScheduleGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        item.SeqNo = SaleScheduleGrid.Count + 1;
                        saleScheduleGrid = SaleScheduleGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(saleScheduleGrid);
                        saleScheduleGrid.Add(item);
                    }
                }
                MainModel.SaleScheduleList = saleScheduleGrid;

                IMemoryCache.Set("ItemList", MainModel, cacheEntryOptions);
                IMemoryCache.Set("KeySaleScheduleGrid", MainModel.SaleScheduleList, cacheEntryOptions);
                HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.SaleScheduleList));
            }
        }
        IMemoryCache.TryGetValue("ItemList", out SaleOrderModel MainModel1);

        return PartialView("_SaleScheduleGrid", MainModel);
    }
}