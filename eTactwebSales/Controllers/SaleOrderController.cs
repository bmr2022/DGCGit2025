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
using System.Reflection;
using FastReport.Web;

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
    public WebReport webReport;


    public SaleOrderController(ILogger<SaleOrderController> logger, IDataLogic iDataLogic, ISaleOrder iSaleOrder, ITaxModule iTaxModule, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IItemMaster itemMaster, EncryptDecrypt encryptDecrypt, LoggerInfo loggerInfo, IConfiguration configuration)
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
        _iconfiguration = configuration;
    }

	private EncryptDecrypt _EncryptDecrypt { get; }
	private IWebHostEnvironment _IWebHostEnvironment { get; }
    public IMemoryCache IMemoryCache { get; }
    private readonly IConfiguration _iconfiguration;
    private LoggerInfo LoggerInfo { get; }


    public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string SONO = "", string ShowOnlyAmendItem = "", int AmmNo = 0)
    {

        string my_connection_string;
        string contentRootPath = _IWebHostEnvironment.ContentRootPath;
        string webRootPath = _IWebHostEnvironment.WebRootPath;
        webReport = new WebReport();
        
        ViewBag.EntryId = EntryId;
        ViewBag.YearCode = YearCode;
        ViewBag.SONO = SONO;
        ViewBag.ShowOnlyAmendItem = ShowOnlyAmendItem;
        ViewBag.AmmNo = AmmNo;
        
        
            webReport.Report.Load(webRootPath + "\\SOReport.frx"); // default report

        
        
        webReport.Report.SetParameterValue("entryparam", EntryId);
        webReport.Report.SetParameterValue("yearparam", YearCode);
        webReport.Report.SetParameterValue("ShowOnlyAmendItemparam", ShowOnlyAmendItem);
        webReport.Report.SetParameterValue("AmmNo", AmmNo);
        my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
        webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
        webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
        webReport.Report.SetParameterValue("MyParameter", my_connection_string);
        webReport.Report.Refresh();
        return View(webReport);
    }

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
			var time = CommonFunc.ParseFormattedDate(DateTime.Now.ToString());
			//string format = "MMM ddd d HH:mm yyyy";
			//string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

			//var dt = time.ToString(format);
			return Json(time);
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
    public async Task<JsonResult> GetFormRightsAmm()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await _ISaleOrder.GetFormRightsAmm(userID);
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
        var ammEffDate = model?.AmmEffDate;
        if (model == null)
		{
			model = new SaleOrderModel
			{
				YearCode = Constants.FinincialYear,
				EntryID = _IDataLogic.GetEntryID("SaleOrderMain", Constants.FinincialYear, "SOEntryID", "SOyearcode"),
				SONo = _IDataLogic.GetEntryID("SaleOrderMain", Constants.FinincialYear, "SOEntryID", "SOyearcode"),
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

		if (model.Mode != "U")
		{
			model.AccountList = await _IDataLogic.GetDropDownList("PARTYNAMELIST", "F", "SP_GetDropDownList");
		}
		else
		{
			model.AccountList = await _IDataLogic.GetDropDownList("PARTYNAMELIST", "T", "SP_GetDropDownList");

		}
		model.ResponsibleSalesPersonList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");
		model.PartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
		model.ItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");
		model.Branch = HttpContext.Session.GetString("Branch");
		model.FinFromDate = HttpContext.Session.GetString("FromDate");
		model.FinToDate = HttpContext.Session.GetString("ToDate");
		model.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
		model.PreparedByName = HttpContext.Session.GetString("EmpName");
		//model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
		if (model.Mode == "SOA")
		{
			model.AmmEffDate = ammEffDate;
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
		model.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy");
        model.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

        return View(model);
	}


    public async Task<IActionResult> ShowGroupWiseItems(int Group_Code,int AccountCode)
    {
        var model = new SaleOrderModel();
        model = await _ISaleOrder.ShowGroupWiseItems(Group_Code, AccountCode);

        
            return PartialView("_SaleOrderGroupWiseItems", model);
        
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

		if (_MemoryCache.TryGetValue("ItemList", out List<ItemDetail> ItemDetailGrid) != null)
		{
			model.ItemDetailGrid = ItemDetailGrid;

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
			//if (model.ItemDetailGrid.Count <= 0)
			//{
			//	HttpContext.Session.Remove("ItemList");
			//	_MemoryCache.Remove("ItemList");
			//}
			//else
			//{
			//	HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
			//}
		}
		return PartialView("_SaleItemGrid", model);
	}

	public IActionResult EditItemRow(SaleOrderModel model)
	{
		bool exists = false;
		object Result = string.Empty;

		int Indx = Convert.ToInt32(model.SeqNo) - 1;

		if (_MemoryCache.TryGetValue("ItemList" , out List<ItemDetail> ItemDetailGrid) != null)
		{
			_MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
			model.ItemDetailGrid = ItemDetailGrid;

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

			//if (model.ItemDetailGrid.Count > 0)
			//{
			//	HttpContext.Session.SetString
			//	(
			//		"ItemList",
			//		JsonConvert.SerializeObject(model.ItemDetailGrid)
			//	);
			//}
			//else
			//{
			//	HttpContext.Session.Remove("ItemList");
			//}
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
		HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
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

    public async Task<JsonResult> getdiscCategoryName(int Group_Code, int AccountCode)
    {
        ResponseResult JsonString = await _ISaleOrder.getdiscCategoryName(Group_Code, AccountCode);
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
        //	ResponseResult JsonString = await _ISaleOrder.GetItemPartCode(Code, "GetItemPartCode");
        //.,mnbvcx.LogError(JsonConvert.SerializeObject(JsonString));
        ResponseResult JsonString = await _ISaleOrder.GetItemPartCode(Code, "GetItemPartCode");
        _logger.LogError(JsonConvert.SerializeObject(JsonString));
        return Json(JsonConvert.SerializeObject(JsonString));
	}

    public async Task<JsonResult> GetItemGroup()
    {
        var JSON = await _ISaleOrder.GetItemGroup();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GETGROUPWISEITEM(int Group_Code)
    {
        var JSON = await _ISaleOrder.GETGROUPWISEITEM( Group_Code);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
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

    public IActionResult AddMultipleItemDetail(List<ItemDetail> model)
    {
        try
        {
            var MainModel = new SaleOrderModel();
            var StockGrid = new List<ItemDetail>();
            var StockAdjustGrid = new List<ItemDetail>();

            var SeqNo = 1;
            foreach (var item in model)
            {
                string modelJson = HttpContext.Session.GetString("ItemList");
                //IList<ItemDetail> ItemDetail = new List<ItemDetail>();
                //if (!string.IsNullOrEmpty(modelJson))
                //{
                //    ItemDetail = JsonConvert.DeserializeObject<List<ItemDetail>>(modelJson);
                //}

                _MemoryCache.TryGetValue("ItemList", out List<ItemDetail> ItemDetail);


                if (model != null)
                {
                    if (ItemDetail == null)
                    {
                        item.SeqNo = SeqNo++;
                        StockGrid.Add(item);
                    }
                    else
                    {


						if (ItemDetail.Any(x => x.PartCode == item.PartCode && x.ItemCode == item.ItemCode))
						{
							return StatusCode(207, "Duplicate");
						}


						item.SeqNo = ItemDetail.Count + 1;
                        StockGrid = ItemDetail.Where(x => x != null).ToList();
                        StockAdjustGrid.AddRange(StockGrid);
                        StockGrid.Add(item);
                    }
                    MainModel.ItemDetailGrid = StockGrid;
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("ItemList", MainModel.ItemDetailGrid, cacheEntryOptions);
                    //HttpContext.Session.SetString("KeyStockMultiBatchAdjustGrid", JsonConvert.SerializeObject(MainModel.StockAdjustModelGrid));
                    HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
            }


            return PartialView("_SaleItemGrid", MainModel);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task<IActionResult> ItemGrid(ItemDetail model)
	{
		if (model.Mode == "SOA")
		{
			return PartialView("_SaleItemGrid", model);
		}
		else
		{
			var MainModel = new SaleOrderModel();
			var _List = new List<ItemDetail>();
			var SSGrid = new List<ItemDetail>();
			_MemoryCache.TryGetValue("ItemList", out List<ItemDetail> ItemDetailGrid);
			_MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
			if (model != null)
			{
				if (ItemDetailGrid == null)
				{
					model.SeqNo = 1;
					_List.Add(model);
				}
				else
				{
					if (ItemDetailGrid.Any(x => x.PartCode == model.PartCode && x.ItemCode == model.ItemCode))
					{
						return StatusCode(207, "Duplicate");
					}
					else
					{
						model.SeqNo = ItemDetailGrid.Count + 1;	
						_List = ItemDetailGrid.Where(x=>x!=null).ToList();
						SSGrid.AddRange(_List);
						_List.Add(model);	
					}
				}
				MainModel.ItemDetailGrid = _List;

				MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddMinutes(60),
					SlidingExpiration = TimeSpan.FromMinutes(55),
					Size = 1024,
				};

				_MemoryCache.Set("ItemList", MainModel.ItemDetailGrid, cacheEntryOptions);
				HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
			}
			else
			{
				ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
			}
			return PartialView("_SaleItemGrid", MainModel);
		}
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

	public async Task<JsonResult> CheckOrderNo(int year, int accountcode, int entryid, string custorderno)
	{
		var JSON = await _ISaleOrder.CheckOrderNo( year,  accountcode,  entryid,  custorderno);
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
	public async Task<ActionResult> OrderDetail(string Mode, int ID, int YC,string fromDateBack = "", string summaryDetail = "", string toDateBack = "",string customerNameBack = "",string customerOrderNoBack="",string branchBack = "",string sonoBack = "",string orderTypeBack = "",string soTypeBack = "",string partCodeBack = "",string itemNameBack = "")
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
		
		model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
		model.CreatedByName = HttpContext.Session.GetString("EmpName");

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
				_MemoryCache.Set("ItemList", model.ItemDetailGrid);
				HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(model.ItemDetailGrid));
				//_MemoryCache.Set("ItemList", model);
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
			//HttpContext.Session.Remove("ItemList");
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

        model.FromDateBack = fromDateBack;
        model.ToDateBack = toDateBack;
		model.DashboardTypeBack = summaryDetail;
        model.CustomerNameBack = customerNameBack;
        model.CustomerOrderNoBack = customerOrderNoBack;
        model.BranchBack = branchBack;
        model.SONOBack = sonoBack;
        model.OrderTypeBack = orderTypeBack;
        model.SoTypeBack = soTypeBack;
        model.PartCodeBack = partCodeBack;
        model.ItemNameBack = itemNameBack;

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
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                        model.CreatedByName = HttpContext.Session.GetString("EmpName");
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

                                    string formattedMessage = errorMessage.Substring(0, wrapLength).Replace("\n", "<br>");

                                    TempData["ErrorMessage"] = formattedMessage;
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

							if (model.Mode == "SOA") {
								return RedirectToAction("SOAmendmentList");
							}
							else
							{
                                return RedirectToAction("OrderDetail", new { ID = 0, YC = 0, Mode = "" });
                            }

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
		Table.Columns.Add("AmendmentDate", typeof(string)); // datetime
		Table.Columns.Add("AmendmentReason", typeof(string));
		Table.Columns.Add("Color", typeof(string));
		Table.Columns.Add("Rejper", typeof(float));
		Table.Columns.Add("Excessper", typeof(float));
		Table.Columns.Add("ProjQty1", typeof(float));
		Table.Columns.Add("ProjQty2", typeof(float));
		Table.Columns.Add("deliverydate", typeof(string)); // datetime
        Table.Columns.Add("CustomerSaleOrder", typeof(string));
        Table.Columns.Add("CustomerLocation", typeof(string));
        Table.Columns.Add("ItemModel", typeof(string));
        Table.Columns.Add("CustItemCategory", typeof(string));	
        Table.Columns.Add("ItemGroupCode", typeof(int));	

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
					Convert.ToDecimal(Item.Qty.ToString("F4")),
					Item.Unit?.Trim() ?? string.Empty,
                    Convert.ToDecimal(Item.AltQty.ToString("F4")),
					Item.AltUnit?.Trim() ?? string.Empty,
                    Convert.ToDecimal(Item.Rate.ToString("F4")),
                    Convert.ToDecimal(Item.OtherRateCurr.ToString("F4")),
					Item.UnitRate?.Trim() ?? string.Empty,
					Item.DiscPer,
					Item.DiscRs,
                    Convert.ToDecimal(Item.Amount.ToString("F4")),
					Item.TolLimit,
					Item.Description ?? string.Empty,
					Item.Remark ?? string.Empty,
					Item.StoreName ?? string.Empty,
                    Convert.ToDecimal(Item.StockQty.ToString("F4")),
					Item.AmendmentNo ?? string.Empty,
					Item.AmendmentDate == null ? "" : ParseFormattedDate(Item.AmendmentDate),
					Item.AmendmentReason ?? string.Empty,
					Item.Color ?? string.Empty,
					Item.Rejper,
					Item.Excessper,
					Item.ProjQty1,
					Item.ProjQty2,
					Item.DeliveryDate == null ? "" : ParseFormattedDate(Item.DeliveryDate),
					Item.CustomerSaleOrder == null ? "" : Item.CustomerSaleOrder,
                    Item.CustomerLocation == null ? "" : Item.CustomerLocation,
                    Item.ItemModel == null ? "" : Item.ItemModel,
                    Item.CustItemCategory == null ? "" : Item.CustItemCategory,
                    Item.Group_Code == null ? 0 : Item.Group_Code,
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
	public async Task<IActionResult> UploadExcel()
	{
		try
		{
			IFormFile ExcelFile = Request.Form.Files.FirstOrDefault();
			if (ExcelFile == null || ExcelFile.Length == 0)
			{
				return BadRequest("Invalid file. Please upload a valid Excel file.");
			}

			string validPartCodesString = Request.Form["validPartCodes"];
			var validPartCodes = new HashSet<string>(validPartCodesString.Split(','), StringComparer.OrdinalIgnoreCase);

			string path = Path.Combine(this._IWebHostEnvironment.WebRootPath, "Uploads");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			string fileName = Path.GetFileName(ExcelFile.FileName);
			string filePath = Path.Combine(path, fileName);
			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				await ExcelFile.CopyToAsync(stream);
			}

			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var SaleGridList = new List<ItemDetail>();
			var MainModel = new SaleOrderModel();
			var errors = new List<string>(); // List to collect validation errors

			using (var package = new ExcelPackage(new FileInfo(filePath)))
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
				if (worksheet == null)
				{
					return BadRequest("Uploaded file does not contain any worksheet.");
				}

				var rowCount = worksheet.Dimension.Rows;
				var itemList = new List<ItemDetail>();
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
					if (isRowEmpty) continue;

					var partCode = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();
					var validateUnit = (worksheet.Cells[row, 2].Value ?? string.Empty).ToString().Trim();

					// **Validate Unit**
                    if (validateUnit.Length > 3)
                    {
                        errors.Add($"Invalid Unit at row {row}: {validateUnit}");
                        continue;
                    }

                    // **Validate PartCode**
                    if (!validPartCodes.Contains(partCode))
					{
						errors.Add($"Invalid PartCode at row {row}: {partCode}");
						continue;
					}

					// **Fetch Item Details from Database**
					var response = await _ISaleOrder.GetExcelData(partCode);

					if (response?.Result is not DataTable itemData || itemData.Rows.Count == 0)
					{
						errors.Add($"No data found for PartCode '{partCode}' at row {row}.");
						continue;
					}

					string hsnNo = itemData.Rows[0]["HSNNo"].ToString();
					string unit = itemData.Rows[0]["Unit"].ToString();
					string altUnit = itemData.Rows[0]["AltUnit"].ToString();
					string itemName = itemData.Rows[0]["Item_Name"].ToString();
					int itemCode = Convert.ToInt32(itemData.Rows[0]["Item_Code"]);


                    string soType = Request.Form["SOType"];
                    string soEntryId = Request.Form["SOEntryId"];
                    string soYearCode = Request.Form["SOYearCode"];
                    string wef = Request.Form["WEF"];
                    string soCloseDate = Request.Form["soCloseDate"];

					itemList.Add(new ItemDetail()
					{
					    SOEntryId = Convert.ToInt32(soEntryId),
						SOYearCode = Convert.ToInt32(soYearCode),
						ItemCode = Convert.ToInt32(itemData.Rows[0]["Item_Code"]),
						PartText = partCode,
                        CustomerSaleOrder = worksheet.Cells[row, 10].Value?.ToString() ?? ""
                    });

					bool isSOTypeClose = soType.Equals("Close", StringComparison.OrdinalIgnoreCase);

					// **Quantity and Rate Validation**
					decimal qty = isSOTypeClose
						? decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out decimal tempQty) ? tempQty : 0
						: 0;

					decimal rate = decimal.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out decimal tempRate) ? tempRate : 0;
					if(rate == 0)
					{
                        errors.Add($"Enter rate more then 0 at row {row}: {partCode}");
                    }

					if (isSOTypeClose && qty <= 0)
					{
						errors.Add($"Qty should be greater than 0 at row {row} ");
						continue; // Skip processing this row
					}

					// **Delivery Date Validation**
					string deliveryDateStr = worksheet.Cells[row, 5].Value?.ToString();

                    if (isSOTypeClose && deliveryDateStr == "")
					{
                        errors.Add($"DeliveryDate is manadatory {row} ");
                        continue;
                    }

                        deliveryDateStr = ParseFormattedDate(deliveryDateStr);
					DateTime? deliveryDate = null;

                    deliveryDate = DateTime.Parse(deliveryDateStr);
                    DateTime wefDate = DateTime.Parse(wef);
                    DateTime soClose = DateTime.Parse(soCloseDate);

                    if (deliveryDate < wefDate || deliveryDate > soClose)
                    {
                        errors.Add($"DeliveryDate must between of wefDate and socloseDate at Row - {row} ");
                        continue;
                    }

                    if (DateTime.TryParse(deliveryDateStr, out DateTime tempDeliveryDate))
					{
						if (tempDeliveryDate < DateTime.Today)
						{
							errors.Add($"Delivery Date at row {row} must be greater than today ({DateTime.Today:dd/MMM/yyyy}).");
						}
						else
						{
							deliveryDate = tempDeliveryDate;
						}
					}

					// **Calculate Amount (Qty * Rate)**
					decimal amount = qty * rate;

					// **Add to SaleGridList**
					SaleGridList.Add(new ItemDetail
					{
						SeqNo = SaleGridList.Count + 1,
						PartCode = itemCode,
						PartText = partCode,
						ItemCode = itemCode,
						ItemText = itemName,
						HSNNo = int.TryParse(hsnNo, out int tempHSN) ? tempHSN : 0, // Fetched from DB
						Qty = qty,
						Unit = unit, // Fetched from DB
						DeliveryDate = deliveryDate?.ToString("dd/MMM/yyyy") ?? "", // Store in required format
						AltQty = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out decimal tempAltQty) ? tempAltQty : 0,
						AltUnit = altUnit, // Fetched from DB
						Rate = rate,
						OtherRateCurr =rate,
						StoreName = worksheet.Cells[row, 17].Value?.ToString() ?? "",
						StockQty = decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out decimal tempStockqty) ? tempStockqty : 0,
						UnitRate = worksheet.Cells[row, 3].Value?.ToString() ?? "",
						DiscPer = decimal.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out decimal tempDiscPer) ? tempDiscPer : 0,
						DiscRs = decimal.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out decimal tempDiscRs) ? tempDiscRs : 0,
						Amount = amount, // Auto-calculated value
						TolLimit = decimal.TryParse(worksheet.Cells[row, 14].Value?.ToString(), out decimal tempTolLimit) ? tempTolLimit : 0,
						Description = worksheet.Cells[row, 15].Value?.ToString() ?? "",
						Remark = worksheet.Cells[row, 16].Value?.ToString() ?? "",
						AmendmentNo = worksheet.Cells[row, 19].Value?.ToString() ?? "",
						AmendmentDate = DateTime.TryParse(worksheet.Cells[row, 20].Value?.ToString(), out DateTime tempAmendDate)
							? tempAmendDate.ToString("yyyy-MM-dd")
							: DateTime.Now.ToString("yyyy-MM-dd"), // Defaults to today if empty
						AmendmentReason = worksheet.Cells[row, 21].Value?.ToString() ?? "",
						Color = worksheet.Cells[row, 22].Value?.ToString() ?? "",
						Rejper = decimal.TryParse(worksheet.Cells[row, 23].Value?.ToString(), out decimal tempRejper) ? tempRejper : 0,
						Excessper = int.TryParse(worksheet.Cells[row, 24].Value?.ToString(), out int tempExcessper) ? tempExcessper : 0,
						ProjQty1 = decimal.TryParse(worksheet.Cells[row, 25].Value?.ToString(), out decimal tempProjQty1) ? tempProjQty1 : 0,
						ProjQty2 = decimal.TryParse(worksheet.Cells[row, 26].Value?.ToString(), out decimal tempProjQty2) ? tempProjQty2 : 0,
                        CustomerSaleOrder = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                        CustomerLocation = worksheet.Cells[row, 11].Value?.ToString() ?? "",
                        ItemModel = worksheet.Cells[row, 12].Value?.ToString() ?? "",
                        CustItemCategory = worksheet.Cells[row, 13].Value?.ToString() ?? "",
                        
                    });
				}

                var duplicateItems = SaleGridList
     .GroupBy(x => new { x.PartText, x.CustomerSaleOrder })
     .Where(g => g.Count() > 1)
     .Select(g => $"[PartCode: {g.Key.PartText}, OrderNo: {g.Key.CustomerSaleOrder}]")
     .ToList();

                if (duplicateItems.Any())
                {
                    var duplicateErrorMsg = "Duplicate PartCode + CustomerOrderNo found:\n" + string.Join("\n", duplicateItems);
                    return BadRequest(string.Join("\n", duplicateErrorMsg));
                }

                if (errors.Count > 0)
				{
					return BadRequest( string.Join("\n", errors));
				}
			}

			MainModel.ItemDetailGrid = SaleGridList;
			HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(SaleGridList));
			return PartialView("_SaleItemGrid", MainModel);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while processing the Excel file.");
			return StatusCode(500, "An internal server error occurred. Please check the file format.");
		}
	}

}
