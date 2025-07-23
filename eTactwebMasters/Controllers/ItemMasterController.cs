using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Runtime.Caching;
using System.Xml.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using DataTable = System.Data.DataTable;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Drawing.Printing;
using ClosedXML.Excel;
using DinkToPdf;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO.Packaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Controllers;

[Authorize]
public class ItemMasterController : Controller
{
    private readonly EncryptDecrypt _EncryptDecrypt;
    private readonly IDataLogic _IDataLogic;
    private readonly IItemMaster _IItemMaster;
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private readonly ILogger<ItemMasterController> _logger;
    public ItemMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IItemMaster iItemMaster, ILogger<ItemMasterController> logger)
    {
        _IDataLogic = iDataLogic;
        _IItemMaster = iItemMaster;
        _IWebHostEnvironment = iWebHostEnvironment;
        _EncryptDecrypt = encryptDecrypt;
        _logger = logger;
    }
    [HttpPost]
    public JsonResult AutoComplete(string ColumnName, string prefix)
    {
        IList<Common.TextValue> iList = _IDataLogic.AutoComplete("Item_Master", ColumnName, "", "", 0, 0);
        var Result = (from item in iList where item.Text.Contains(prefix) select new { item.Text })
            .Distinct()
            .ToList();

        return Json(Result);
    }
    
    public async Task<IActionResult> Dashboard(string Item_Name, string PartCode, string ParentCode, string ItemType, string HsnNo, string Flag, string Package, string OldPartCode, string SerialNo, string VoltageVlue, string UniversalPartCode = "", int pageNumber = 1, int pageSize = 50)
    {
        ItemMasterModel model = new ItemMasterModel
        {
            ParentGroupList = await _IDataLogic.GetDropDownList("Itemgroup_master", "SP_GetDropDownList"),
            ItemTypeList = await _IDataLogic.GetDropDownList("Item_Master_Type", "SP_GetDropDownList")
        };
        Item_Name = string.IsNullOrEmpty(Item_Name) ? "" : Item_Name.Trim();
        PartCode = string.IsNullOrEmpty(PartCode) ? "" : PartCode.Trim();
        ItemType = ItemType == "0" || ItemType == null ? null : ItemType;
        ParentCode = ParentCode == "0" || ParentCode == null ? null : ParentCode;
        HsnNo = HsnNo == "0" || HsnNo == null ? null : HsnNo;

        var allData = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HsnNo, UniversalPartCode, Flag);

        HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(allData));

        model.TotalRecords = allData.Count();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        model.MasterList = allData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        model.Item_Name = Item_Name;
        model.PartCode = PartCode;
        model.ParentName = ParentCode;
        model.ItemTypeName = ItemType;
        model.VoltageVlue = VoltageVlue;
        model.SerialNo = SerialNo;
        model.OldPartCode = OldPartCode;
        model.Package = Package;
        model.HSNNO = HsnNo == null ? 0 : Convert.ToInt32(HsnNo);

        // return View(model);
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_IMGrid", model); // Your partial grid view
        }

        return View(model);
    }
    public async Task<IActionResult> GetSearchData(string Item_Name, string PartCode, string ParentCode, string ItemType, string HsnNo, string UniversalPartCode, string Flag, int pageNumber = 1, int pageSize = 50)
    {
        ItemMasterModel model = new ItemMasterModel();
        model.SwitchAll = "false";
        Item_Name = string.IsNullOrEmpty(Item_Name) ? "" : Item_Name.Trim();
        PartCode = string.IsNullOrEmpty(PartCode) ? "" : PartCode.Trim();
        ItemType = ItemType == "0" || ItemType == null ? null : ItemType;
        ParentCode = ParentCode == "0" || ParentCode == null ? null : ParentCode;
        HsnNo = HsnNo == "0" || HsnNo == null ? null : HsnNo;
        model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HsnNo, UniversalPartCode, "Search");
        var allData = model.MasterList;
        HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(allData));

        model.TotalRecords = allData.Count();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        model.MasterList = allData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return PartialView("_IMGrid", model);
    }
    public async Task<IActionResult> GetAllColumns(string Item_Name, string PartCode, string ParentCode, string ItemType, string HsnNo, string UniversalPartCode, string Flag, int pageNumber = 1, int pageSize = 50)
    {
        ItemMasterModel model = new ItemMasterModel();
        model.SwitchAll = "true";
        Item_Name = string.IsNullOrEmpty(Item_Name) ? "" : Item_Name.Trim();
        PartCode = string.IsNullOrEmpty(PartCode) ? "" : PartCode.Trim();
        ItemType = ItemType == "0" || ItemType == null ? null : ItemType;
        ParentCode = ParentCode == "0" || ParentCode == null ? null : ParentCode;
        HsnNo = HsnNo == "0" || HsnNo == null ? null : HsnNo;
        model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HsnNo, UniversalPartCode, "Search");
        var allData = model.MasterList;
        HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(allData));

        model.TotalRecords = allData.Count();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        model.MasterList = allData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return PartialView("_IMGridAllColumns", model);
    }
    public IActionResult ExportItemMasterToExcel(bool showAll)
    {
        string modelListJson = HttpContext.Session.GetString("KeyItemListSearch");

        List<ItemMasterModel> modelList = new List<ItemMasterModel>();
        if (!string.IsNullOrEmpty(modelListJson))
        {
            modelList = JsonConvert.DeserializeObject<List<ItemMasterModel>>(modelListJson);
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Item Master");

        // Choose export method based on 'showAll'
        if (showAll)
        {
            EXPORT_AllColumnsGrid(worksheet, modelList);
        }
        else
        {
            EXPORT_SearchGrid(worksheet, modelList);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ItemMasterReport.xlsx"
        );
    }
    public IActionResult ExportSelectedItemToExcel(string flag)
    {
        string modelListJson = HttpContext.Session.GetString("KeyItemListSearch");
            
        List<ItemMasterModel> modelList = new List<ItemMasterModel>();
        if (!string.IsNullOrEmpty(modelListJson))
        {
            modelList = JsonConvert.DeserializeObject<List<ItemMasterModel>>(modelListJson);
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Item Master");

        int row = 1;
        int col = 1;

        // Common headers
        worksheet.Cell(row, col++).Value = "Sr";
        worksheet.Cell(row, col++).Value = "Item_Code";
        worksheet.Cell(row, col++).Value = "PartCode";
        worksheet.Cell(row, col++).Value = "Item_Name";

        // Add selected column(s)
        if (flag == "hsncode")
        {
            worksheet.Cell(row, col).Value = "HSNCODE";
        }
        else if (flag == "price")
        {
            worksheet.Cell(row, col++).Value = "SalePrice";
            worksheet.Cell(row, col++).Value = "PurchasePrice";
            worksheet.Cell(row, col).Value = "CostPrice";
        }
        else if (flag == "store")
        {
            worksheet.Cell(row, col++).Value = "StoreName";
        }
        else if (flag == "workcenter")
        {
            worksheet.Cell(row, col++).Value = "WorkCenter";
        }
        else if (flag == "minmaxlevel")
        {
            worksheet.Cell(row, col++).Value = "MaximumLevel";
            worksheet.Cell(row, col).Value = "MinimumLevel";
        }
        else
        {
            // If flag is empty or unrecognized, export all additional columns
            worksheet.Cell(row, col++).Value = "HSNCODE";
            worksheet.Cell(row, col++).Value = "WorkCenter";
            worksheet.Cell(row, col++).Value = "WorkCenterId";
            worksheet.Cell(row, col++).Value = "Store";
            worksheet.Cell(row, col++).Value = "SalePrice";
            worksheet.Cell(row, col++).Value = "PurchasePrice";
            worksheet.Cell(row, col++).Value = "CostPrice";
            worksheet.Cell(row, col++).Value = "MaximumLevel";
            worksheet.Cell(row, col).Value = "MinimumLevel";
        }

        // Write data rows
        for (int i = 0; i < modelList.Count; i++)
        {
            int r = i + 2;
            int c = 1;

            worksheet.Cell(r, c++).Value = i + 1;
            worksheet.Cell(r, c++).Value = modelList[i].Item_Code;
            worksheet.Cell(r, c++).Value = modelList[i].PartCode;
            worksheet.Cell(r, c++).Value = modelList[i].Item_Name;

            if (flag == "hsncode")
            {
                worksheet.Cell(r, c).Value = modelList[i].HSNNO;
            }
            else if (flag == "price")
            {
                worksheet.Cell(r, c++).Value = modelList[i].SalePrice;
                worksheet.Cell(r, c++).Value = modelList[i].PurchasePrice;
                worksheet.Cell(r, c).Value = modelList[i].CostPrice;
            }
            else if (flag == "store")
            {
                worksheet.Cell(r, c).Value = modelList[i].StoreName;
            }
            else if (flag == "workcenter")
            {
                worksheet.Cell(r, c).Value = modelList[i].ProdWorkCenterDescription;
               // worksheet.Cell(r, c).Value = modelList[i].ProdInWorkcenter;
            }
            else if (flag == "minmaxlevel")
            {
                worksheet.Cell(r, c++).Value = modelList[i].MaximumLevel;
                worksheet.Cell(r, c).Value = modelList[i].MinimumLevel;
            }
            else
            {
                // Export all if no flag
                worksheet.Cell(r, c++).Value = modelList[i].HSNNO;
                worksheet.Cell(r, c++).Value = modelList[i].ProdWorkCenterDescription;
                worksheet.Cell(r, c++).Value = modelList[i].ProdInWorkcenter;
                worksheet.Cell(r, c++).Value = modelList[i].Store;
                worksheet.Cell(r, c++).Value = modelList[i].SalePrice;
                worksheet.Cell(r, c++).Value = modelList[i].PurchasePrice;
                worksheet.Cell(r, c++).Value = modelList[i].CostPrice;
                worksheet.Cell(r, c++).Value = modelList[i].MaximumLevel;
                worksheet.Cell(r, c).Value = modelList[i].MinimumLevel;
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ItemMasterReport.xlsx"
        );
    }

    private void EXPORT_HSNGrid(IXLWorksheet sheet, IList<ItemMasterModel> list)
    {
        string[] headers = {
                "#Sr","Item Code", "Part Code", "Item Name","HSN Code"
            };



        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        int row = 2, srNo = 1;
        foreach (var item in list)
        {
            sheet.Cell(row, 1).Value = srNo++;
            sheet.Cell(row, 2).Value = item.Item_Code;
            sheet.Cell(row, 3).Value = item.PartCode;
            sheet.Cell(row, 4).Value = item.Item_Name;
            sheet.Cell(row, 5).Value = item.HSNNO;
           
            row++;
        }
    }

    private void EXPORT_SearchGrid(IXLWorksheet sheet, IList<ItemMasterModel> list)
    {
        string[] headers = {
                "#Sr","Item Code", "Part Code", "Item Name", "Item Group", "Item Category", "Unit", "Alt.Unit", "HSN Code",
        "Minimum Level", "Maximum Level", "Std Packing", "Prod In Workcenter", "Prod Inhouse JW",
        "Batch No", "Entry Date", "Created By", "Created On", "Updated By", "Updated On",
        "Old PartCode", "Voltage Value", "Serial No", "Package"
            };



        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        int row = 2, srNo = 1;
        foreach (var item in list)
        {
            sheet.Cell(row, 1).Value = srNo++;
            sheet.Cell(row, 2).Value = item.Item_Code;
            sheet.Cell(row, 3).Value = item.PartCode;
            sheet.Cell(row, 4).Value = item.Item_Name;
            sheet.Cell(row, 5).Value = item.ItemGroup;
            sheet.Cell(row, 6).Value = item.TypeName;
            sheet.Cell(row, 7).Value = item.Unit;
            sheet.Cell(row, 8).Value = item.AlternateUnit;
            sheet.Cell(row, 9).Value = item.HSNNO;
            sheet.Cell(row, 10).Value = item.MinimumLevel;
            sheet.Cell(row, 11).Value = item.MaximumLevel;
            sheet.Cell(row, 12).Value = item.StdPacking;
            sheet.Cell(row, 13).Value = item.ProdWorkCenterDescription;
            sheet.Cell(row, 14).Value = item.ProdInhouseJW;
            sheet.Cell(row, 15).Value = item.BatchNO;
            sheet.Cell(row, 16).Value = item.EntryDate;
            sheet.Cell(row, 17).Value = item.CreatedByName;
            sheet.Cell(row, 18).Value = item.CreatedOn?.ToString("yyyy-MM-dd");
            sheet.Cell(row, 19).Value = item.UpdatedByName;
            sheet.Cell(row, 20).Value = item.UpdatedOn?.ToString("yyyy-MM-dd");
            sheet.Cell(row, 21).Value = item.SerialNo;
            sheet.Cell(row, 22).Value = item.VoltageVlue;
            sheet.Cell(row, 23).Value = item.OldPartCode;
            sheet.Cell(row, 24).Value = item.Package;

            row++;
        }
    }

    private void EXPORT_AllColumnsGrid(IXLWorksheet sheet, IList<ItemMasterModel> list)
    {
        string[] headers = {
        "#Sr", "Item_Code", "PartCode", "Item_Name", "ItemGroup", "LeadTime", "Unit", "SalePrice", "PurchasePrice",
        "CostPrice", "WastagePercent", "WtSingleItem", "NoOfPcs", "QcReq",
        "ItemType", "ImageURL", "DrawingNo", "MinimumLevel", "MaximumLevel",
        "ReorderLevel", "YearCode", "AlternateUnit", "RackID", "BinNo", "ItemSize",
        "Colour", "NeedPO", "StdPacking", "PackingType", "ModelNo", "YearlyConsumedQty",
        "DispItemName", "PurchaseAccountcode", "SaleAccountcode", "MinLevelDays", "MaxLevelDays",
        "DailyRequirment", "Stockable", "WipStockable", "Store", "ProductLifeInus", "ItemDesc", "MaxWipStock",
        "NeedSo", "BomRequired", "HSNNO", "Universal Part Code", "Universal Description", "WorkCenterDescription",
        "ProdInhouseJW", "BatchNO", "VoltageValue", "SerialNo", "OldPartCode", "Package",
        "IsCustJWAdjMandatory", "Active", "JobWorkItem", "ItemServAssets"
    };

        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        int row = 2;
        int srNo = 1;

        foreach (var item in list)
        {
            int col = 1;
            sheet.Cell(row, col++).Value = srNo++;
            sheet.Cell(row, col++).Value = item.Item_Code;
            sheet.Cell(row, col++).Value = item.PartCode;
            sheet.Cell(row, col++).Value = item.Item_Name;
            sheet.Cell(row, col++).Value = item.ItemGroup;
            sheet.Cell(row, col++).Value = item.LeadTime;
            sheet.Cell(row, col++).Value = item.Unit;
            sheet.Cell(row, col++).Value = item.SalePrice;
            sheet.Cell(row, col++).Value = item.PurchasePrice;
            sheet.Cell(row, col++).Value = item.CostPrice;
            sheet.Cell(row, col++).Value = item.WastagePercent;
            sheet.Cell(row, col++).Value = item.WtSingleItem;
            sheet.Cell(row, col++).Value = item.NoOfPcs;
            sheet.Cell(row, col++).Value = item.QcReq;
            sheet.Cell(row, col++).Value = item.TypeName;
            sheet.Cell(row, col++).Value = item.ImageURL;
            sheet.Cell(row, col++).Value = item.DrawingNo;
            sheet.Cell(row, col++).Value = item.MinimumLevel;
            sheet.Cell(row, col++).Value = item.MaximumLevel;
            sheet.Cell(row, col++).Value = item.ReorderLevel;
            sheet.Cell(row, col++).Value = item.YearCode;
            sheet.Cell(row, col++).Value = item.AlternateUnit;
            sheet.Cell(row, col++).Value = item.RackID;
            sheet.Cell(row, col++).Value = item.BinNo;
            sheet.Cell(row, col++).Value = item.ItemSize;
            sheet.Cell(row, col++).Value = item.Colour;
            sheet.Cell(row, col++).Value = item.NeedPO;
            sheet.Cell(row, col++).Value = item.StdPacking;
            sheet.Cell(row, col++).Value = item.PackingType;
            sheet.Cell(row, col++).Value = item.ModelNo;
            sheet.Cell(row, col++).Value = item.YearlyConsumedQty;
            sheet.Cell(row, col++).Value = item.DispItemName;
            sheet.Cell(row, col++).Value = item.PurchaseAccountcode;
            sheet.Cell(row, col++).Value = item.SaleAccountcode;
            sheet.Cell(row, col++).Value = item.MinLevelDays;
            sheet.Cell(row, col++).Value = item.MaxLevelDays;
            sheet.Cell(row, col++).Value = item.DailyRequirment;
            sheet.Cell(row, col++).Value = item.Stockable;
            sheet.Cell(row, col++).Value = item.WipStockable;
            sheet.Cell(row, col++).Value = item.StoreName;
            sheet.Cell(row, col++).Value = item.ProductLifeInus;
            sheet.Cell(row, col++).Value = item.ItemDesc;
            sheet.Cell(row, col++).Value = item.MaxWipStock;
            sheet.Cell(row, col++).Value = item.NeedSo;
            sheet.Cell(row, col++).Value = item.BomRequired;
            sheet.Cell(row, col++).Value = item.HSNNO;
            sheet.Cell(row, col++).Value = item.UniversalPartCode;
            sheet.Cell(row, col++).Value = item.UniversalDescription;
            sheet.Cell(row, col++).Value = item.ProdWorkCenterDescription;
            sheet.Cell(row, col++).Value = item.ProdInhouseJW;
            sheet.Cell(row, col++).Value = item.BatchNO;
            sheet.Cell(row, col++).Value = item.VoltageVlue;
            sheet.Cell(row, col++).Value = item.SerialNo;
            sheet.Cell(row, col++).Value = item.OldPartCode;
            sheet.Cell(row, col++).Value = item.Package;
            sheet.Cell(row, col++).Value = item.IsCustJWAdjMandatory;
            sheet.Cell(row, col++).Value = item.Active;
            sheet.Cell(row, col++).Value = item.JobWorkItem;
            sheet.Cell(row, col++).Value = item.ItemServAssets;

            row++;
        }
    }


    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await _IItemMaster.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetPartCode(int ParentCode, int ItemType)
    {
        var JSON = await _IItemMaster.GetPartCode(ParentCode, ItemType);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetItemCategory(string ItemServAssets)
    {
        var JSON = await _IItemMaster.GetItemCategory(ItemServAssets);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetItemGroup(string ItemServAssets)
    {
        var JSON = await _IItemMaster.GetItemGroup(ItemServAssets);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetProdInWorkcenter()
    {
        var JSON = await _IItemMaster.GetProdInWorkcenter();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    [HttpGet]
    public IActionResult GlobalSearch(string searchString, string ShowAll, int pageNumber = 1, int pageSize = 50)
    {
        ItemMasterModel model = new ItemMasterModel();

        // Get session data
        string jsonString = HttpContext.Session.GetString("KeyItemListSearch");

        // Deserialize it into the actual list
        IList<ItemMasterModel> itemViewModel = new List<ItemMasterModel>();
        if (!string.IsNullOrEmpty(jsonString))
        {
            itemViewModel = JsonConvert.DeserializeObject<IList<ItemMasterModel>>(jsonString);
        }

        // If no search input, return empty result
        if (ShowAll == "true")
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_IMGridAllColumns", new List<ItemMasterModel>());
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_IMGrid", new List<ItemMasterModel>());
            }

        }

        // If still null or empty after session read, return empty
        if (ShowAll == "true")
        {
            if (itemViewModel == null || !itemViewModel.Any())
            {
                return PartialView("_IMGridAllColumns", new List<ItemMasterModel>());
            }
        }
        else
        {
            if (itemViewModel == null || !itemViewModel.Any())
            {
                return PartialView("_IMGrid", new List<ItemMasterModel>());
            }
        }

        // Perform search on all string properties
        var filteredResults = itemViewModel
            .Where(i => i.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.GetValue(i)?.ToString())
                .Any(value => !string.IsNullOrEmpty(value) &&
                              value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        // Fill model for paged result
        model.TotalRecords = filteredResults.Count;
        model.MasterList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        if (ShowAll == "true")
        {
            return PartialView("_IMGridAllColumns", model);
        }
        else
        {
            return PartialView("_IMGrid", model);
        }
        return null;
    }
    public async Task<IActionResult> DeleteItemByID(int ID, string ParentName, string POServType, string HSNNO, string PartCode, string ItemName)
    {
        var Result = await _IItemMaster.DeleteItemByID(ID);

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.OK)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else if (Result.StatusText == "Failure" || Result.StatusCode == HttpStatusCode.Locked)
        {
            ViewBag.isSuccess = false;
            TempData["423"] = "423";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }
        return RedirectToAction("Dashboard", new { Item_Name = ItemName, PartCode = PartCode, ParentCode = ParentName, ItemType = POServType, HsnNo = HSNNO, Flag = "" });
    }
    public async Task<IActionResult> GetData()
    {
        ItemMasterList iList = new ItemMasterList
        {
            ItemMaster = await _IItemMaster.GetAllItemMaster("Get"),
            UnitList = await _IDataLogic.GetDropDownList("Unit_Master", "SP_GetDropDownList"),
            AltUnitList = await _IDataLogic.GetDropDownList("Unit_Master", "SP_GetDropDownList"),
            ParentGroupList = await _IDataLogic.GetDropDownList("Itemgroup_master", "SP_GetDropDownList"),
            ItemTypeList = await _IDataLogic.GetDropDownList("Item_Master_Type", "SP_GetDropDownList"),
            PurchaseAccList = await _IDataLogic.GetDropDownList("Account_Head_Master_PA", "SP_GetDropDownList"),
            SaleAccList = await _IDataLogic.GetDropDownList("Account_Head_Master_SA", "SP_GetDropDownList"),
            StoreList = await _IDataLogic.GetDropDownList("Store_Master", "SP_GetDropDownList"),
            EmpList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList")
        };

        IList<ItemMasterModel> model = await _IItemMaster.GetAllItemMaster("Get");

        return View();
    }
    [HttpPost]
    public JsonResult isDuplicate(string ColName, string Colval)
    {
        if (!string.IsNullOrEmpty(ColName))
        {
            if (ColName == "Item Name")
            {
                ColName = "Item_Name";
            }
            else if (ColName == "Part Code")
            {
                ColName = "PartCode";
            }
        }
        var Result = _IDataLogic.isDuplicate(Colval, ColName, "Item_Master");
        return Json(Result);
    }
    [HttpGet]
    public async Task<IActionResult> ItemDetail(int ID, string Mode)
    {
        ItemMasterModel model = new ItemMasterModel();
        HttpContext.Session.Remove("KeyItemList");

        if (ID == 0)
        {
            model.EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
            model.LastUpdatedDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");

            model.BomRequired = "N";
            model.JobWorkItem = "N";
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.Mode = null;
        }
        else
        {
            model = await _IItemMaster.GetItemMasterByID(ID);
            model.Mode = Mode;
        }

        model.UnitList = await _IDataLogic.GetDropDownList("Unit_Master", "SP_GetDropDownList");
        model.AltUnitList = await _IDataLogic.GetDropDownList("Unit_Master", "SP_GetDropDownList");
        model.ParentGroupList = await _IDataLogic.GetDropDownList("Itemgroup_master", "SP_GetDropDownList");
        model.ItemTypeList = await _IDataLogic.GetDropDownList("Item_Master_Type", "SP_GetDropDownList");
        model.PurchaseAccList = await _IDataLogic.GetDropDownList("Account_Head_Master_PA", "SP_GetDropDownList");
        model.SaleAccList = await _IDataLogic.GetDropDownList("Account_Head_Master_SA", "SP_GetDropDownList");
        model.StoreList = await _IDataLogic.GetDropDownList("Store_Master", "SP_GetDropDownList");
        model.EmpList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");
        model.FeatureOption = _IItemMaster.GetFeatureOption();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        model.FinFromDate = HttpContext.Session.GetString("FromDate");
        model.FinToDate = HttpContext.Session.GetString("ToDate");
        model.CC = HttpContext.Session.GetString("Branch");
        if (Mode != "U")
        {
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.CreatedByName = HttpContext.Session.GetString("EmpName");
            model.CreatedOn = DateTime.Now;
        }
        else if (Mode == "U")
        {
            //model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //model.UpdatedByName = HttpContext.Session.GetString("EmpName");
            //model.UpdatedOn = DateTime.Now;
        }
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ItemDetail(ItemMasterModel model)
    {
        model.Mode = model.Item_Code == 0 ? "Insert" : "Update";

        if (model.Mode == "Update")
        {
            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            if (model.UploadImage != null)
            {
                var getPathById = await _IItemMaster.GetItemMasterByID(model.Item_Code);
                string ImagePath = Path.Combine(_IWebHostEnvironment.WebRootPath + getPathById.ImageURL);

                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
            }
            if (model.ItemImage != null)
            {
                var getPathById = await _IItemMaster.GetItemMasterByID(model.Item_Code);
                string ImagePath = Path.Combine(_IWebHostEnvironment.WebRootPath + getPathById.ItemImageURL);

                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
            }
        }

        if (model.UploadImage != null)
        {
            //string ImagePath = "UploadedImages/";
            string ImagePath = "Uploads/Item/";

            if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
            {
                Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
            }

            ImagePath += Guid.NewGuid().ToString() + "_" + model.UploadImage.FileName;
            model.ImageURL = "/" + ImagePath;
            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
            //await model.UploadImage.CopyToAsync(new FileStream(ServerPath, FileMode.Create));
            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
            {
                await model.UploadImage.CopyToAsync(FileStream);
            }

            //string ServerPath = _IWebHostEnvironment.WebRootPath;
            //string FileName = Path.GetFileNameWithoutExtension(model.UploadImage.FileName);
            //string Extension = Path.GetExtension(model.UploadImage.FileName);
            //FileName = DateTime.Now.ToString("dd_MM_yy_ffff") + "_" + FileName + Extension;
            //string FullPath = Path.Combine(ServerPath + "/UploadedImages/", FileName);

            //using (var FileStream = new FileStream(FullPath, FileMode.Create))
            //{
            //    await model.UploadImage.CopyToAsync(FileStream);
            //}
        }
        if (model.ItemImage != null)
        {
            //string ImagePath = "UploadedImages/";
            string ImagePath = "Uploads/ItemImage/";

            if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
            {
                Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
            }

            ImagePath += Guid.NewGuid().ToString() + "_" + model.ItemImage.FileName;
            model.ItemImageURL = "/" + ImagePath;
            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
            //await model.UploadImage.CopyToAsync(new FileStream(ServerPath, FileMode.Create));
            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
            {
                await model.ItemImage.CopyToAsync(FileStream);
            }

        }
        //else
        //{
        //    ModelState.Remove("UploadImage");
        //    ModelState.Remove("CC");
        //    ModelState.Remove("UID");
        //    ModelState.Remove("Mode");
        //    ModelState.Remove("Act");
        //    ModelState.Remove("EmpList");
        //}

        //if (ModelState.IsValid)
        //{
        //model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

        var Result = await _IItemMaster.SaveData(model).ConfigureAwait(true);

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
        if (Result.StatusText == "Success")
        {
            return RedirectToAction(nameof(Dashboard));
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
            return RedirectToAction(nameof(ItemDetail), new { ID = 0 });
        }

        //}
        //else
        //{
        //    ViewBag.isSuccess = false;
        //    TempData["500"] = "500";
        //    return RedirectToAction(nameof(ItemDetail), new { ID = 0 });
        //}
    }
    public ActionResult ImportItems()
    {
        ItemMasterModel model = new ItemMasterModel();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

        return View(model);
    }

    public async Task<IActionResult> ImportandUpdateItems(
     string? Item_Name = "",
     string? PartCode = "",
     string? ParentCode = "",
     string? ItemType = "",
     string? HSNNO = "",
     string? UniversalPartCode = "",
     string? Flag = "")
    {
        var model = new ItemMasterModel();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

        model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HSNNO, UniversalPartCode, Flag);

        HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(model.MasterList));

        return View(model);
    }


    [HttpPost]
    public IActionResult UpdateUploadExcel(IFormFile excelFile)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<ImportItemViewModel> data = new List<ImportItemViewModel>();

            using (var stream = excelFile.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var totalColumns = worksheet.Dimension.Columns;
                var headersMap = new Dictionary<string, int>();

                for (int col = 1; col <= totalColumns; col++)
                {
                    var header = worksheet.Cells[1, col].Text?.Trim();
                    if (!string.IsNullOrEmpty(header) && !headersMap.ContainsKey(header))
                        headersMap[header] = col;
                }
                var dupeItemNameFeatureOpt = _IItemMaster.GetFeatureOption();
                var UnitList = _IItemMaster.GetUnitList();
                
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    //var cellValue = worksheet.Cells[row, 2].Value;
                    //if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    //    break;
                    var partCode = worksheet.Cells[row, headersMap["PartCode"]].Text?.Trim();
                    var ItemName = worksheet.Cells[row, headersMap["Item_Name"]].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(partCode)) break;
                    var itemGroup = worksheet.Cells[row, headersMap["ItemGroup"]].Text?.Trim();
                    var itemType = worksheet.Cells[row, headersMap["ItemType"]].Text?.Trim();
                    var workCenter = worksheet.Cells[row, headersMap["WorkCenterDescription"]].Text?.Trim();
                    var unit = worksheet.Cells[row, headersMap["Unit"]].Text?.Trim();
                    var Store = worksheet.Cells[row, headersMap["Store"]].Text?.Trim();
                    var duplicatePartCode = _IDataLogic.isDuplicate(partCode, "PartCode", "Item_Master");
                    var duplicateItemName = _IDataLogic.isDuplicate(ItemName, "Item_Name", "Item_Master");

                    var itemGroupCode = _IItemMaster.GetItemGroupCode(itemGroup);
                    var itemCatCode = _IItemMaster.GetItemCatCode(itemType);
                    var WorkCenterId = _IItemMaster.GetWorkCenterId(workCenter);
                    var StoreIdResult = _IItemMaster.GetStoreCode(Store);
                    var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 0 ? "Y" : "N";
                    var ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : (Convert.ToInt32(duplicateItemName.Result) > 0 ? "Y" : "N");
                    var ItemServAssets = worksheet.Cells[row, headersMap["ItemServAssets"]].Text?.Trim();
                    var ItemGroupList = _IItemMaster.GetItemGroup(ItemServAssets);
                    var ItemCategoryList = _IItemMaster.GetItemCategory(ItemServAssets);
                    
                    //var itemGroupCode = _IItemMaster.GetItemGroupCode(worksheet.Cells[row, 6].Value?.ToString());
                    //var itemCatCode = _IItemMaster.GetItemCatCode(worksheet.Cells[row, 20].Value?.ToString());
                    //var WorkCenterId = _IItemMaster.GetWorkCenterId(worksheet.Cells[row, 56].Value?.ToString());

                    //var duplicatePartCode = _IDataLogic.isDuplicate(worksheet.Cells[row, 3].Value?.ToString(), "PartCode", "Item_Master");
                    //var duplicateItemName = _IDataLogic.isDuplicate(worksheet.Cells[row, 4].Value?.ToString(), "Item_Name", "Item_Master");

                    //var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 0 ? "Y" : "N";
                    //var ItemNameExists = Convert.ToInt32(duplicateItemName.Result) > 0 ? "Y" : "N";

                    //  var unit = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? "";
                    var unitdataset = UnitList.Result.Result;
                    var unitTable = unitdataset.Tables[0];

                    bool unitExists = false;
                    foreach (DataRow rows in unitTable.Rows)
                    {
                        if (rows["Unit_name"].ToString().Trim().Equals(unit, StringComparison.OrdinalIgnoreCase))
                        {
                            unitExists = true;
                            break;
                        }
                    }
                    if (!unitExists)
                    {
                        return StatusCode(207, "Invalid Unit: " + unit);
                    } 

                    var Groupdataset = ItemGroupList.Result.Result;
                    var GroupTable = Groupdataset.Tables[0];

                    bool GroupExists = false;
                    foreach (DataRow rows in GroupTable.Rows)
                    {
                        if (rows["Group_name"].ToString().Trim().Equals(itemGroup, StringComparison.OrdinalIgnoreCase))
                        {
                            GroupExists = true;
                            break;
                        }
                    }
                    if (!GroupExists)
                    {
                        return StatusCode(207, $"Invalid ItemGroup: {itemGroup} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                    }
                    var Categorydataset = ItemCategoryList.Result.Result;
                    var CategoryTable = Categorydataset.Tables[0];

                    bool CategoryExists = false;
                    foreach (DataRow rows in CategoryTable.Rows)
                    {
                        if (rows["Type_Item"].ToString().Trim().Equals(itemType, StringComparison.OrdinalIgnoreCase))
                        {
                            CategoryExists = true;
                            break;
                        }
                    }
                    if (!CategoryExists)
                    {
                        return StatusCode(207, $"Invalid ItemType: {itemType} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                    }
                    



                    ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : ItemNameExists;

                    int itemGCode = 1;
                    int itemCCode = 1;
                    int itemWorkCenterId = 0;
                    int itemStoreId = 0;

                    if (itemGroupCode.Result.Result != null && itemGroupCode.Result.Result.Rows.Count > 0)
                    {
                        itemGCode = (int)itemGroupCode.Result.Result.Rows[0].ItemArray[0];
                    }

                    if (itemCatCode.Result.Result != null && itemCatCode.Result.Result.Rows.Count > 0)
                    {
                        itemCCode = (int)itemCatCode.Result.Result.Rows[0].ItemArray[0];
                    }

                    if (WorkCenterId.Result.Result != null && WorkCenterId.Result.Result.Rows.Count > 0)
                    {
                        itemWorkCenterId = (int)WorkCenterId.Result.Result.Rows[0].ItemArray[0];
                    }   
                    if (StoreIdResult.Result.Result != null && StoreIdResult.Result.Result.Rows.Count > 0)
                    {
                        itemStoreId = (int)StoreIdResult.Result.Result.Rows[0].ItemArray[0];
                    }
                   
                  
                    bool WorkcenterExists = false;
                    
                    if (!string.IsNullOrWhiteSpace(workCenter))
                    {
                        var WorkCenterList = _IItemMaster.GetWorkCenterList();
                        var WorkCenterdataset = WorkCenterList.Result.Result;
                        var WorkCenterTable = WorkCenterdataset.Tables[0];

                        foreach (DataRow rows in WorkCenterTable.Rows)
                        {
                            if (rows["WorkCenterDescription"].ToString().Trim().Equals(workCenter, StringComparison.OrdinalIgnoreCase))
                            {
                                WorkcenterExists = true;
                                break;
                            }
                        }

                        if (!WorkcenterExists)
                        {
                            return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }

                   

                    bool StoreExists = false;
                    if (!string.IsNullOrWhiteSpace(Store))
                    {
                        var StoreList = _IItemMaster.GetStoreList();
                        var Storedataset = StoreList.Result.Result;
                        var StoreTable = Storedataset.Tables[0];
                        foreach (DataRow rows in StoreTable.Rows)
                        {
                            if (rows["Store_Name"].ToString().Trim().Equals(Store, StringComparison.OrdinalIgnoreCase))
                            {
                                StoreExists = true;
                                break;
                            }
                        }


                        if (!StoreExists)
                        {
                            return StatusCode(207, $"Invalid StoreName: {Store} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");

                        }

                    }

                    decimal minLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MinimumLevel"]].Value ?? 0);
                    decimal maxLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MaximumLevel"]].Value ?? 0);
                    decimal reorderLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["ReorderLevel"]].Value ?? 0);

                    if (minLevel != 0 && maxLevel != 0 && reorderLevel != 0)
                    {
                        if (minLevel > maxLevel)
                        {
                            return StatusCode(207, $"Invalid Levels at row {row}: MinimumLevel ({minLevel}) cannot be greater than MaximumLevel ({maxLevel}).");
                        }

                        if (reorderLevel < minLevel || reorderLevel > maxLevel)
                        {
                            return StatusCode(207, $"Invalid ReorderLevel at row {row}: ReorderLevel ({reorderLevel}) must be between MinimumLevel ({minLevel}) and MaximumLevel ({maxLevel}).");
                        }
                    }
                    string batchNO = worksheet.Cells[row, headersMap["BatchNO"]].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(batchNO))
                    {
                        var validBatchOptions = new List<string> { "MRNWISE", "NOOFCase", "ForEachQty" };

                        if (!validBatchOptions.Contains(batchNO, StringComparer.OrdinalIgnoreCase))
                        {
                            return StatusCode(207, $"Invalid BatchNO value at row {row}: '{batchNO}'. Valid options are MRNWISE, NOOFCase, ForEachQty.");
                        }
                    }

                    data.Add(new ImportItemViewModel
                    {
                        Item_Code = Convert.ToInt32(worksheet.Cells[row, headersMap["Item_Code"]].Value ?? 0),
                        PartCode = partCode,
                        Item_Name = worksheet.Cells[row, headersMap["Item_Name"]].Text?.Trim(),
                        ItemGroup = itemGroup,
                        ItemType = itemType,
                        TypeName= itemType,
                        Unit = unit,
                        SalePrice = Convert.ToInt32(worksheet.Cells[row, headersMap["SalePrice"]].Value ?? 0),
                        PurchasePrice = Convert.ToInt32(worksheet.Cells[row, headersMap["PurchasePrice"]].Value ?? 0),
                        CostPrice = Convert.ToInt32(worksheet.Cells[row, headersMap["CostPrice"]].Value ?? 0),
                        HSNNO = Convert.ToInt32(worksheet.Cells[row, headersMap["HSNNO"]].Value ?? 0),
                        StoreName = worksheet.Cells[row, headersMap.ContainsKey("Store") ? headersMap["Store"] : headersMap["StoreName"]].Text?.Trim(),
                        Package = worksheet.Cells[row, headersMap["Package"]].Text?.Trim(),
                        VoltageVlue = worksheet.Cells[row, headersMap["VoltageValue"]].Text?.Trim(),
                        SerialNo = worksheet.Cells[row, headersMap["SerialNo"]].Text?.Trim(),
                        OldPartCode = worksheet.Cells[row, headersMap["OldPartCode"]].Text?.Trim(),
                        ItemGroupCode = itemGCode,
                        ItemCategoryCode = itemCCode,
                        ProdInWorkcenter = itemWorkCenterId,
                        Store = itemStoreId.ToString(),
                        PartCodeExists = PartCodeExists,
                        ItemNameExists = ItemNameExists,
                        LeadTime = Convert.ToInt32(worksheet.Cells[row, headersMap["LeadTime"]].Value ?? 0),
                        WastagePercent = Convert.ToInt32(worksheet.Cells[row, headersMap["WastagePercent"]].Value ?? 0),
                        WtSingleItem = Convert.ToDecimal(worksheet.Cells[row, headersMap["WtSingleItem"]].Value ?? 0),
                        NoOfPcs = Convert.ToInt32(worksheet.Cells[row, headersMap["NoOfPcs"]].Value ?? 0),
                        QcReq = worksheet.Cells[row, headersMap["QcReq"]].Text?.Trim(),
                        ImageURL = worksheet.Cells[row, headersMap["ImageURL"]].Text?.Trim(),
                        DrawingNo = worksheet.Cells[row, headersMap["DrawingNo"]].Text?.Trim(),
                        MinimumLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MinimumLevel"]].Value ?? 0),
                        MaximumLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MaximumLevel"]].Value ?? 0),
                        ReorderLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["ReorderLevel"]].Value ?? 0),
                        YearCode = worksheet.Cells[row, headersMap["YearCode"]].Text?.Trim(),
                        AlternateUnit = worksheet.Cells[row, headersMap["AlternateUnit"]].Text?.Trim(),
                        RackID = worksheet.Cells[row, headersMap["RackID"]].Text?.Trim(),
                        BinNo = worksheet.Cells[row, headersMap["BinNo"]].Text?.Trim(),
                        ItemSize = worksheet.Cells[row, headersMap["ItemSize"]].Text?.Trim(),
                        Colour = worksheet.Cells[row, headersMap["Colour"]].Text?.Trim(),
                        NeedPO = worksheet.Cells[row, headersMap["NeedPO"]].Text?.Trim(),
                        StdPacking = Convert.ToInt32(worksheet.Cells[row, headersMap["StdPacking"]].Value ?? 0),
                        PackingType = worksheet.Cells[row, headersMap["PackingType"]].Text?.Trim(),
                        ModelNo = worksheet.Cells[row, headersMap["ModelNo"]].Text?.Trim(),
                        YearlyConsumedQty = Convert.ToInt32(worksheet.Cells[row, headersMap["YearlyConsumedQty"]].Value ?? 0),
                        DispItemName = worksheet.Cells[row, headersMap["DispItemName"]].Text?.Trim(),
                        PurchaseAccountcode = worksheet.Cells[row, headersMap["PurchaseAccountcode"]].Text?.Trim(),
                        SaleAccountcode = worksheet.Cells[row, headersMap["SaleAccountcode"]].Text?.Trim(),
                        MinLevelDays = Convert.ToInt32(worksheet.Cells[row, headersMap["MinLevelDays"]].Value ?? 0),
                        MaxLevelDays = Convert.ToInt32(worksheet.Cells[row, headersMap["MaxLevelDays"]].Value ?? 0),
                        DailyRequirment = Convert.ToInt32(worksheet.Cells[row, headersMap["DailyRequirment"]].Value ?? 0),
                        Stockable = worksheet.Cells[row, headersMap["Stockable"]].Text?.Trim(),
                        WipStockable = worksheet.Cells[row, headersMap["WipStockable"]].Text?.Trim(),
                        ProductLifeInus = worksheet.Cells[row, headersMap["ProductLifeInus"]].Text?.Trim(),
                        ItemDesc = worksheet.Cells[row, headersMap["ItemDesc"]].Text?.Trim(),
                        MaxWipStock = Convert.ToInt32(worksheet.Cells[row, headersMap["MaxWipStock"]].Value ?? 0),
                        NeedSo = worksheet.Cells[row, headersMap["NeedSo"]].Text?.Trim(),
                        BomRequired = worksheet.Cells[row, headersMap["BomRequired"]].Text?.Trim(),
                        UniversalPartCode = worksheet.Cells[row, headersMap["Universal Part Code"]].Text?.Trim(),
                        UniversalDescription = worksheet.Cells[row, headersMap["Universal Description"]].Text?.Trim(),
                        WorkCenter = worksheet.Cells[row, headersMap["WorkCenterDescription"]].Text?.Trim(),
                        ProdInhouseJW = worksheet.Cells[row, headersMap["ProdInhouseJW"]].Text?.Trim(),
                        BatchNO = worksheet.Cells[row, headersMap["BatchNO"]].Text?.Trim(),
                        IsCustJWAdjMandatory = worksheet.Cells[row, headersMap["IsCustJWAdjMandatory"]].Text?.Trim(),
                        Active = worksheet.Cells[row, headersMap["Active"]].Text?.Trim(),
                        JobWorkItem = worksheet.Cells[row, headersMap["JobWorkItem"]].Text?.Trim(),
                        ItemServAssets = worksheet.Cells[row, headersMap["ItemServAssets"]].Text?.Trim(),

                        //Item_Code = Convert.ToInt32(worksheet.Cells[row, 2].Value), // B
                        //PartCode = worksheet.Cells[row, 3].Value?.ToString(),       // C
                        //Item_Name = worksheet.Cells[row, 4].Value?.ToString(),      // D
                        //ParentCode = worksheet.Cells[row, 5].Value?.ToString(),     // E
                        //ItemGroup = worksheet.Cells[row, 6].Value?.ToString(),      // F
                        //EntryDate = worksheet.Cells[row, 7].Value?.ToString(),      // G
                        //LastUpdatedDate = worksheet.Cells[row, 8].Value?.ToString(),// H
                        // LeadTime = Convert.ToInt32(worksheet.Cells[row, headersMap["LeadTime"]].Value ?? 0),       // I
                        //LeadTime = Convert.ToInt32(worksheet.Cells[row, 9].Value?.ToString()),       // I
                        // CC = worksheet.Cells[row, 10].Value?.ToString(),            // J
                        //Unit = worksheet.Cells[row, 11].Value?.ToString(),          // K
                        //SalePrice = Convert.ToInt32(worksheet.Cells[row, 12].Value?.ToString()),     // L
                        //PurchasePrice = Convert.ToInt32(worksheet.Cells[row, 13].Value?.ToString()), // M
                        //CostPrice = Convert.ToInt32(worksheet.Cells[row, 14].Value?.ToString()),     // N
                        //WastagePercent = Convert.ToInt32(worksheet.Cells[row, 15].Value?.ToString()),// O
                        //WtSingleItem = Convert.ToDecimal(worksheet.Cells[row, 16].Value?.ToString()),  // P
                        //NoOfPcs =Convert.ToInt32 (worksheet.Cells[row, 17].Value?.ToString()),       // Q
                        //QcReq = worksheet.Cells[row, 18].Value?.ToString(),         // R
                        //TypeName = worksheet.Cells[row, 19].Value?.ToString(),      // S
                        ////ItemType = worksheet.Cells[row, 20].Value?.ToString(),      // T
                        //ImageURL = worksheet.Cells[row, 21].Value?.ToString(),      // U
                        //UID = worksheet.Cells[row, 22].Value?.ToString(),           // V
                        //DrawingNo = worksheet.Cells[row, 23].Value?.ToString(),     // W
                        //MinimumLevel = Convert.ToDecimal(worksheet.Cells[row, 24].Value ?? 0), // X
                        //MaximumLevel = Convert.ToDecimal(worksheet.Cells[row, 25].Value ?? 0), // Y
                        //ReorderLevel = Convert.ToDecimal(worksheet.Cells[row, 26].Value ?? 0), // Z
                        //YearCode = worksheet.Cells[row, 27].Value?.ToString(),      // AA
                        //AlternateUnit = worksheet.Cells[row, 28].Value?.ToString(), // AB
                        //RackID = worksheet.Cells[row, 29].Value?.ToString(),        // AC
                        //BinNo = worksheet.Cells[row, 30].Value?.ToString(),         // AD
                        //ItemSize = worksheet.Cells[row, 31].Value?.ToString(),      // AE
                        //Colour = worksheet.Cells[row, 32].Value?.ToString(),        // AF
                        //NeedPO = worksheet.Cells[row, 33].Value?.ToString(),        // AG
                        //StdPacking = Convert.ToInt32(worksheet.Cells[row, 34].Value ?? 0), // AH
                        //PackingType = worksheet.Cells[row, 35].Value?.ToString(),   // AI
                        //ModelNo = worksheet.Cells[row, 36].Value?.ToString(),       // AJ
                        //YearlyConsumedQty = Convert.ToInt32(worksheet.Cells[row, 37].Value ?? 0), // AK
                        //DispItemName = worksheet.Cells[row, 38].Value?.ToString(),  // AL
                        //PurchaseAccountcode = worksheet.Cells[row, 39].Value?.ToString(), // AM
                        //SaleAccountcode = worksheet.Cells[row, 40].Value?.ToString(), // AN
                        //MinLevelDays = Convert.ToInt32(worksheet.Cells[row, 41].Value ?? 0), // AO
                        //MaxLevelDays = Convert.ToInt32(worksheet.Cells[row, 42].Value ?? 0), // AP
                        //EmpName = worksheet.Cells[row, 43].Value?.ToString(),       // AQ
                        //DailyRequirment = Convert.ToInt32(worksheet.Cells[row, 44].Value ?? 0), // AR
                        //Stockable = worksheet.Cells[row, 45].Value?.ToString(),     // AS
                        //WipStockable = worksheet.Cells[row, 46].Value?.ToString(),  // AT
                        //Store = worksheet.Cells[row, 47].Value?.ToString(),         // AU
                        //ProductLifeInus = worksheet.Cells[row, 48].Value?.ToString(), // AV
                        //ItemDesc = worksheet.Cells[row, 49].Value?.ToString(),      // AW
                        //MaxWipStock = Convert.ToInt32(worksheet.Cells[row, 50].Value ?? 0), // AX
                        //NeedSo = worksheet.Cells[row, 51].Value?.ToString(),        // AY
                        //BomRequired = worksheet.Cells[row, 52].Value?.ToString(),   // AZ
                        //HSNNO = Convert.ToInt32(worksheet.Cells[row, 53].Value ?? 0), // BA
                        //UniversalPartCode = worksheet.Cells[row, 54].Value?.ToString(), // BB
                        //UniversalDescription = worksheet.Cells[row, 55].Value?.ToString(), // BC
                        //WorkCenter = worksheet.Cells[row, 56].Value?.ToString(),    // BD
                        //ProdInhouseJW = worksheet.Cells[row, 57].Value?.ToString(), // BE
                        //BatchNO = worksheet.Cells[row, 58].Value?.ToString(),       // BF
                        //VoltageVlue = worksheet.Cells[row, 59].Value?.ToString(),   // BG
                        //SerialNo = worksheet.Cells[row, 60].Value?.ToString(),      // BH
                        //OldPartCode = worksheet.Cells[row, 61].Value?.ToString(),   // BI
                        //Package = worksheet.Cells[row, 62].Value?.ToString(),       // BJ
                        //IsCustJWAdjMandatory = worksheet.Cells[row, 63].Value?.ToString(), // BK
                        //CreatedByName = worksheet.Cells[row, 64].Value?.ToString(), // BL
                        //CreatedOn = DateTime.TryParse(worksheet.Cells[row, 65].Value?.ToString(), out var createdOn) ? createdOn : (DateTime?)null, // BM
                        //UpdatedByName = worksheet.Cells[row, 66].Value?.ToString(), // BN
                        //UpdatedOn = DateTime.TryParse(worksheet.Cells[row, 67].Value?.ToString(), out var updatedOn) ? updatedOn : (DateTime?)null, // BO
                        //Active = worksheet.Cells[row, 68].Value?.ToString(),        // BP
                        //JobWorkItem = worksheet.Cells[row, 69].Value?.ToString(),   // BQ
                        //ItemServAssets = worksheet.Cells[row, 70].Value?.ToString(),// BR

                        //// Additional logic fields
                        //ItemGroupCode = itemGCode,
                        //ItemCategoryCode = itemCCode,
                        //ProdInWorkcenter = itemWorkCenterId,
                        //PartCodeExists = PartCodeExists,
                        //ItemNameExists = ItemNameExists,
                        SeqNo = row - 1
                    });

                }
            }

            var model = new ItemMasterModel
            {
                ImportExcelDataList = data
            };
            return PartialView("_DisplayExcelItemData", model);
        }
        catch (Exception ex)
        {

            return StatusCode(500, "Excel processing error: " + ex.Message);
        }
    }
    [HttpPost]
    public IActionResult PreviewExcelData(IFormFile excelFile, string flag)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var data = new List<ImportItemViewModel>();
        var errors = new List<string>(); // collect errors

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var cellValue = worksheet.Cells[row, 2].Value;
                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    break;

                var model = new ImportItemViewModel
                {
                    Item_Code = Convert.ToInt32(worksheet.Cells[row, 2].Value),
                    PartCode = worksheet.Cells[row, 3].Value?.ToString(),
                    Item_Name = worksheet.Cells[row, 4].Value?.ToString()
                };

                switch (flag?.ToLower())
                {
                    case "hsncode":
                        model.HSNNO = Convert.ToInt32(worksheet.Cells[row, 5].Value ?? 0);
                        break;

                    case "store":
                        var Store = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                        bool StoreExists = false;
                        if (!string.IsNullOrWhiteSpace(Store))
                        {
                            var StoreList = _IItemMaster.GetStoreList();
                            var Storedataset = StoreList.Result.Result;
                            var StoreTable = Storedataset.Tables[0];

                          
                            foreach (DataRow rows in StoreTable.Rows)
                            {
                                if (rows["Store_Name"].ToString().Trim().Equals(Store, StringComparison.OrdinalIgnoreCase))
                                {
                                    StoreExists = true;
                                    break;
                                }
                            }


                            if (!StoreExists)
                            {
                                errors.Add($"Row {row}: Invalid StoreName: {Store}");
                                continue;
                            }
                        }
                        model.StoreName = Store;
                        var StoreIdResult = _IItemMaster.GetStoreCode(Store);
                        if (StoreIdResult.Result.Result.Rows.Count > 0)
                        {
                            model.Store = StoreIdResult.Result.Result.Rows[0].ItemArray[0]?.ToString();
                        }
                        break;

                    case "workcenter":
                        var WorkCenter = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                        var WorkCenterList = _IItemMaster.GetWorkCenterList();
                        bool WorkcenterExists = false;

                        if (!string.IsNullOrWhiteSpace(WorkCenter))
                        {
                            var WorkCenterdataset = WorkCenterList.Result.Result;
                            var WorkCenterTable = WorkCenterdataset.Tables[0];

                         
                            foreach (DataRow rows in WorkCenterTable.Rows)
                            {
                                if (rows["WorkCenterDescription"].ToString().Trim().Equals(WorkCenter, StringComparison.OrdinalIgnoreCase))
                                {
                                    WorkcenterExists = true;
                                    break;
                                }
                            }

                            if (!WorkcenterExists)
                            {
                                errors.Add($"Row {row}: Invalid Workcenter: {WorkCenter}");
                                continue;
                            }
                        }
                        model.WorkCenter = WorkCenter;
                        var workCenterIdResult = _IItemMaster.GetWorkCenterId(WorkCenter);
                        if (workCenterIdResult.Result.Result.Rows.Count > 0)
                        {
                            model.ProdInWorkcenter = Convert.ToInt32(workCenterIdResult.Result.Result.Rows[0].ItemArray[0]);
                        }
                        break;

                    case "price":
                        model.SalePrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value ?? 0);
                        model.PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0);
                        model.CostPrice = Convert.ToDecimal(worksheet.Cells[row, 7].Value ?? 0);
                        break;

                    case "minmaxlevel":
                        model.MaximumLevel = Convert.ToDecimal(worksheet.Cells[row, 5].Value ?? 0);
                        model.MinimumLevel = Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0);
                        break;
                }

                data.Add(model);
            }
        }

        // ✅ Return validation errors if any
        if (errors.Any())
        {
            return BadRequest(new { errors });
        }

        var masterList = data.Select(x => new ItemMasterModel
        {
            Item_Code = x.Item_Code,
            PartCode = x.PartCode,
            Item_Name = x.Item_Name,
            HSNNO = x.HSNNO,
            Store = x.Store,
            StoreName = x.StoreName,
            SalePrice = x.SalePrice,
            PurchasePrice = x.PurchasePrice,
            CostPrice = x.CostPrice,
            MaximumLevel = x.MaximumLevel,
            MinimumLevel = x.MinimumLevel,
            ProdWorkCenterDescription = x.WorkCenter,
            ProdInWorkcenter = x.ProdInWorkcenter
        }).ToList();

        var result = new ItemMasterModel
        {
            ImportExcelDataList = data,
            MasterList = masterList
        };

        HttpContext.Session.SetString("UploadedExcelData", JsonConvert.SerializeObject(data));
        ViewData["Flag"] = flag;
        return PartialView("_DisplayExcelselectedItemData", result);
    }




    [HttpPost]
    public IActionResult UploadExcel(IFormFile excelFile)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<ItemViewModel> data = new List<ItemViewModel>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var dupeItemNameFeatureOpt = _IItemMaster.GetFeatureOption();
            var UnitList = _IItemMaster.GetUnitList();
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var cellValue = worksheet.Cells[row, 1].Value;

                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    break; // Stop when column 1 is empty
                var itemGroupCode = _IItemMaster.GetItemGroupCode(worksheet.Cells[row, 5].Value.ToString());
                var itemCatCode = _IItemMaster.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var duplicatePartCode = _IDataLogic.isDuplicate(worksheet.Cells[row, 1].Value.ToString(), "PartCode", "Item_Master");
                var duplicateItemName = _IDataLogic.isDuplicate(worksheet.Cells[row, 2].Value.ToString(), "Item_Name", "Item_Master");

                var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 0 ? "Y" : "N";
                var ItemNameExists = Convert.ToInt32(duplicateItemName.Result) > 0 ? "Y" : "N";

                var unit = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";

                var unitdataset = UnitList.Result.Result;
                var unitTable = unitdataset.Tables[0];

                bool unitExists = false;
                foreach (DataRow rows in unitTable.Rows)
                {
                    if (rows["Unit_name"].ToString().Trim().Equals(unit, StringComparison.OrdinalIgnoreCase))
                    {
                        unitExists = true;
                        break;
                    }
                }
                if (!unitExists)
                {
                    return StatusCode(207, "Invalid Unit" + unit);
                }


                ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : ItemNameExists;

                int itemGCode = 1;
                int itemCCode = 1;
                if (itemGroupCode.Result.Result != null)
                {
                    itemGCode = itemGroupCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemGroupCode.Result.Result.Rows[0].ItemArray[0];

                }
                if (itemCatCode.Result.Result != null)
                {
                    itemCCode = itemCatCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCatCode.Result.Result.Rows[0].ItemArray[0];

                }



                data.Add(new ItemViewModel()
                {
                    PartCode = worksheet.Cells[row, 1].Value.ToString(),
                    PartCodeExists = PartCodeExists,
                    ItemNameExists = ItemNameExists,
                    ItemName = worksheet.Cells[row, 2].Value.ToString(),
                    Unit = worksheet.Cells[row, 3].Value.ToString(),
                    HSNNo = (worksheet.Cells[row, 4].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 4].Value.ToString()))
        ? 0
        : Convert.ToInt32(worksheet.Cells[row, 4].Value.ToString()),
                    ItemGroup = worksheet.Cells[row, 5].Value.ToString(),
                    ItemCategory = worksheet.Cells[row, 6].Value.ToString(),
                    MinLevel = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString()),
                    MaxLevel = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString()),
                    Stockable = worksheet.Cells[row, 9].Value.ToString(),
                    WIPStockable = worksheet.Cells[row, 10].Value.ToString(),
                    NeedPO = worksheet.Cells[row, 11].Value.ToString(),
                    QcReq = worksheet.Cells[row, 12].Value.ToString(),
                    StdPkg = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString()),
                    ItemGroupCode = itemGCode,
                    ItemCategoryCode = itemCCode,
                    ItemServAssets = worksheet.Cells[row, 14].Value.ToString()
                });
            }
        }
        var model = new ItemMasterModel();
        model.ExcelDataList = data;
        return PartialView("_DisplayExcelData", model);
    }
    public async Task<IActionResult> AddItemListdata(List<ItemViewModel> model)
    {
        try
        {
            HttpContext.Session.Remove("KeyItemList");

            string ItemList = HttpContext.Session.GetString("KeyItemList");
            IList<ItemViewModel> ItemViewModel = new List<ItemViewModel>();
            if (!string.IsNullOrEmpty(ItemList))
            {
                ItemViewModel = JsonConvert.DeserializeObject<IList<ItemViewModel>>(ItemList);
            }

            var MainModel = new ItemMasterModel();
            var ItemDetailGrid = new List<ItemViewModel>();
            var ItemGrid = new List<ItemViewModel>();
            var SSGrid = new List<ItemViewModel>();

            var seqNo = 0;
            foreach (var item in model)
            {
                if (item != null)
                {
                    if (ItemViewModel == null)
                    {
                        item.SeqNo += seqNo + 1;
                        ItemGrid.Add(item);
                        seqNo++;
                    }
                    else
                    {
                        if (ItemViewModel.Where(x => x.PartCode == item.PartCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            item.SeqNo = ItemViewModel.Count + 1;
                            //ItemGrid = ItemViewModel.Where(x => x != null).ToList();
                            //SSGrid.AddRange(ItemGrid);
                            ItemGrid.Add(item);
                        }
                    }
                    MainModel.ExcelDataList = ItemGrid;

                    HttpContext.Session.SetString("KeyItemList", JsonConvert.SerializeObject(MainModel.ExcelDataList));
                }
            }

            string modelData = HttpContext.Session.GetString("KeyItemList");
            IList<ItemViewModel> ItemListt = new List<ItemViewModel>();
            if (!string.IsNullOrEmpty(modelData))
            {
                ItemListt = JsonConvert.DeserializeObject<IList<ItemViewModel>>(modelData);
            }
            var CC = HttpContext.Session.GetString("Branch");
            var EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var ItemGridList = new DataTable();
            ItemGridList = GetDetailTable(ItemListt, CC, EmpID);

            var Result = await _IItemMaster.SaveMultipleItemData(ItemGridList);

            if (Result != null)
            {
                if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                }
                if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                {
                    ViewBag.isSuccess = true;
                    TempData["202"] = "202";
                }
                if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.isSuccess = false;
                    TempData["500"] = "500";
                    _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                    return View("Error", Result);
                }
            }


            return RedirectToAction(nameof(ImportItems));
        }
        catch (Exception ex)
        {
            LogException<ItemMasterController>.WriteException(_logger, ex);

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateItemListdata(List<ImportItemViewModel> model)
    {
        try
        {
            HttpContext.Session.Remove("KeyImportItemList");

            string ItemList = HttpContext.Session.GetString("KeyImportItemList");
            IList<ImportItemViewModel> ItemViewModel = new List<ImportItemViewModel>();
            if (!string.IsNullOrEmpty(ItemList))
            {
                ItemViewModel = JsonConvert.DeserializeObject<IList<ImportItemViewModel>>(ItemList);
            }

            var MainModel = new ItemMasterModel();
            var ItemDetailGrid = new List<ImportItemViewModel>();
            var ItemGrid = new List<ImportItemViewModel>();
            var SSGrid = new List<ImportItemViewModel>();

            var seqNo = 0;
            foreach (var item in model)
            {
                if (item != null)
                {
                    if (ItemViewModel == null)
                    {
                        item.SeqNo += seqNo + 1;
                        ItemGrid.Add(item);
                        seqNo++;
                    }
                    else
                    {
                        if (ItemViewModel.Where(x => x.PartCode == item.PartCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            item.SeqNo = ItemViewModel.Count + 1;
                            //ItemGrid = ItemViewModel.Where(x => x != null).ToList();
                            //SSGrid.AddRange(ItemGrid);
                            ItemGrid.Add(item);
                        }
                    }
                    MainModel.ImportExcelDataList = ItemGrid;

                    HttpContext.Session.SetString("KeyImportItemList", JsonConvert.SerializeObject(MainModel.ImportExcelDataList));
                }
            }

            string modelData = HttpContext.Session.GetString("KeyImportItemList");
            IList<ImportItemViewModel> ItemListt = new List<ImportItemViewModel>();
            if (!string.IsNullOrEmpty(modelData))
            {
                ItemListt = JsonConvert.DeserializeObject<IList<ImportItemViewModel>>(modelData);
            }
            var CC = HttpContext.Session.GetString("Branch");
            var EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var ItemGridList = new DataTable();
            ItemGridList = GetItemDetailTable(ItemListt, CC, EmpID);

            var Result = await _IItemMaster.UpdateMultipleItemData(ItemGridList);

            if (Result != null)
            {
                if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                }
                if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                {
                    ViewBag.isSuccess = true;
                    TempData["202"] = "202";
                }
                if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.isSuccess = false;
                    TempData["500"] = "500";
                    _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                    return View("Error", Result);
                }
            }


            return RedirectToAction(nameof(ImportItems));
        }
        catch (Exception ex)
        {
            LogException<ItemMasterController>.WriteException(_logger, ex);

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateSelectedItemListdata(List<ImportItemViewModel> model,string flag)
    {
        try
        {
            HttpContext.Session.Remove("KeyImportSelectedItemList");

            string ItemList = HttpContext.Session.GetString("KeyImportSelectedItemList");
            IList<ImportItemViewModel> ItemViewModel = new List<ImportItemViewModel>();
            if (!string.IsNullOrEmpty(ItemList))
            {
                ItemViewModel = JsonConvert.DeserializeObject<IList<ImportItemViewModel>>(ItemList);
            }

            var MainModel = new ItemMasterModel();
            var ItemDetailGrid = new List<ImportItemViewModel>();
            var ItemGrid = new List<ImportItemViewModel>();
            var SSGrid = new List<ImportItemViewModel>();

            var seqNo = 0;
            foreach (var item in model)
            {
                if (item != null)
                {
                    if (ItemViewModel == null)
                    {
                        item.SeqNo += seqNo + 1;
                        ItemGrid.Add(item);
                        seqNo++;
                    }
                    else
                    {
                        if (ItemViewModel.Where(x => x.PartCode == item.PartCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            item.SeqNo = ItemViewModel.Count + 1;
                            //ItemGrid = ItemViewModel.Where(x => x != null).ToList();
                            //SSGrid.AddRange(ItemGrid);
                            ItemGrid.Add(item);
                        }
                    }
                    MainModel.ImportExcelDataList = ItemGrid;

                    HttpContext.Session.SetString("KeyImportSelectedItemList", JsonConvert.SerializeObject(MainModel.ImportExcelDataList));
                }
            }

            string modelData = HttpContext.Session.GetString("KeyImportSelectedItemList");
            IList<ImportItemViewModel> ItemListt = new List<ImportItemViewModel>();
            if (!string.IsNullOrEmpty(modelData))
            {
                ItemListt = JsonConvert.DeserializeObject<IList<ImportItemViewModel>>(modelData);
            }
            var CC = HttpContext.Session.GetString("Branch");
            var EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var ItemGridList = new DataTable();
            ItemGridList = GetItemDetailTable(ItemListt,CC,EmpID);
            var Flag = "Update" + flag;
            var Result = await _IItemMaster.UpdateSelectedItemData(ItemGridList, Flag);

            if (Result != null)
            {
                if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                }
                if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                {
                    ViewBag.isSuccess = true;
                    TempData["202"] = "202";
                }
                if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.isSuccess = false;
                    TempData["500"] = "500";
                    _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                    return View("Error", Result);
                }
            }


            return RedirectToAction(nameof(ImportItems));
        }
        catch (Exception ex)
        {
            LogException<ItemMasterController>.WriteException(_logger, ex);

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }
    }
    private static DataTable GetDetailTable(IList<ItemViewModel> DetailList, string CC, int Empid)
    {
        var MRGrid = new DataTable();

        MRGrid.Columns.Add("Item_Code", typeof(int));
        MRGrid.Columns.Add("PartCode", typeof(string));
        MRGrid.Columns.Add("Item_Name", typeof(string));
        MRGrid.Columns.Add("ParentCode", typeof(int));
        MRGrid.Columns.Add("EntryDate", typeof(string));
        MRGrid.Columns.Add("LastUpdatedDate", typeof(string));
        MRGrid.Columns.Add("LeadTime", typeof(int));
        MRGrid.Columns.Add("CC", typeof(string));
        MRGrid.Columns.Add("Unit", typeof(string));
        MRGrid.Columns.Add("SalePrice", typeof(float));
        MRGrid.Columns.Add("PurchasePrice", typeof(float));
        MRGrid.Columns.Add("CostPrice", typeof(float));
        MRGrid.Columns.Add("WastagePercent", typeof(float));
        MRGrid.Columns.Add("WtSingleItem", typeof(float));
        MRGrid.Columns.Add("NoOfPcs", typeof(float));
        MRGrid.Columns.Add("QcReq", typeof(string));
        MRGrid.Columns.Add("ItemType", typeof(int));
        MRGrid.Columns.Add("UploadItemImage", typeof(string));
        MRGrid.Columns.Add("UploadImage", typeof(string));
        MRGrid.Columns.Add("UID", typeof(int));
        MRGrid.Columns.Add("DrawingNo", typeof(string));
        MRGrid.Columns.Add("MinimumLevel", typeof(float));
        MRGrid.Columns.Add("MaximumLevel", typeof(float));
        MRGrid.Columns.Add("ReorderLevel", typeof(float));
        MRGrid.Columns.Add("YearCode", typeof(float));
        MRGrid.Columns.Add("AlternateUnit", typeof(string));
        MRGrid.Columns.Add("RackID", typeof(string));
        MRGrid.Columns.Add("BinNo", typeof(string));
        MRGrid.Columns.Add("ItemSize", typeof(string));
        MRGrid.Columns.Add("Colour", typeof(string));
        MRGrid.Columns.Add("NeedPO", typeof(string));
        MRGrid.Columns.Add("StdPacking", typeof(float));
        MRGrid.Columns.Add("PackingType", typeof(string));
        MRGrid.Columns.Add("ModelNo", typeof(string));
        MRGrid.Columns.Add("YearlyConsumedQty", typeof(float));
        MRGrid.Columns.Add("DispItemName", typeof(string));
        MRGrid.Columns.Add("PurchaseAccountcode", typeof(int));
        MRGrid.Columns.Add("SaleAccountcode", typeof(int));
        MRGrid.Columns.Add("MinLevelDays", typeof(int));
        MRGrid.Columns.Add("MaxLevelDays", typeof(int));
        MRGrid.Columns.Add("EmpName", typeof(string));
        MRGrid.Columns.Add("DailyRequirment", typeof(float));
        MRGrid.Columns.Add("Stockable", typeof(string));
        MRGrid.Columns.Add("WipStockable", typeof(string));
        MRGrid.Columns.Add("Store", typeof(string));
        MRGrid.Columns.Add("ProductLifeInus", typeof(float));
        MRGrid.Columns.Add("ItemDesc", typeof(string));
        MRGrid.Columns.Add("MaxWipStock", typeof(float));
        MRGrid.Columns.Add("NeedSo", typeof(string));
        MRGrid.Columns.Add("BomRequired", typeof(string));
        MRGrid.Columns.Add("JobWorkItem", typeof(string));
        MRGrid.Columns.Add("HsnNo", typeof(string));
        MRGrid.Columns.Add("CreatedBy", typeof(int));
        MRGrid.Columns.Add("CreatedOn", typeof(string));
        MRGrid.Columns.Add("UpdatedBy", typeof(int));
        MRGrid.Columns.Add("UpdatedOn", typeof(string));
        MRGrid.Columns.Add("Active", typeof(string));
        MRGrid.Columns.Add("ItemServAssets", typeof(string));

        foreach (var Item in DetailList)
        {
            DateTime today = DateTime.Today;
            MRGrid.Rows.Add(
                new object[]
                                    {
                                        0,                          // 1. Item_Code (int)
                                        Item.PartCode ?? "",         // 2. PartCode (string)
                                        Item.ItemName,               // 3. Item_Name (string)
                                        Item.ItemGroupCode,          // 4. ParentCode (int)
                                        today.ToString("yyyy-MM-dd").Split(" ")[0],  // 5. EntryDate (string)
                                        today.ToString("yyyy-MM-dd").Split(" ")[0],  // 6. LastUpdatedDate (string)
                                        0,                           // 7. LeadTime (int)
                                        CC,                          // 8. CC (string)
                                        Item.Unit,                   // 9. Unit (string)
                                        0,                           // 10. SalePrice (float)
                                        0,                           // 11. PurchasePrice (float)
                                        0,                           // 12. CostPrice (float)
                                        0,                           // 13. WastagePercent (float)
                                        0,                           // 14. WtSingleItem (float)
                                        0,                           // 15. NoOfPcs (float)
                                        "Y",                          // 16. QcReq (string)
                                        Item.ItemCategoryCode,       // 17. ItemType (int)
                                        "",                          // 18. UploadItemImage (string)
                                        "",                          // 19. UploadImage (string)
                                        Empid,                       // 20. UID (int)
                                        "",                          // 21. DrawingNo (string)
                                        0,                           // 22. MinimumLevel (float)
                                        0,                           // 23. MaximumLevel (float)
                                        0,                           // 24. ReorderLevel (float)
                                       "2025",                           // 25. YearCode (float)
                                        "",                          // 26. AlternateUnit (string)
                                        "",                          // 27. RackID (string)
                                        "",                          // 28. BinNo (string)
                                        "",                          // 29. ItemSize (string)
                                        "",                          // 30. Colour (string)
                                        "N",                          // 31. NeedPO (string)
                                        0,                           // 32. StdPacking (float)
                                        "",                          // 33. PackingType (string)
                                        "",                          // 34. ModelNo (string)
                                        0,                           // 35. YearlyConsumedQty (float)
                                        "",                          // 36. DispItemName (string)
                                        0,                           // 37. PurchaseAccountcode (int)
                                        0,                          // 38. SaleAccountcode (string) - MISMATCH (expected int)
                                        0,                           // 39. MinLevelDays (int)
                                        0,                           // 40. MaxLevelDays (int)
                                        "",                          // 41. EmpName (string)
                                        0,                           // 41. DailyRequirment (float)
                                        "Y",                         // 42. Stockable (string)
                                        "Y",                         // 43. WipStockable (string)
                                        "",                          // 44. Store (string)
                                        0,                           // 45. ProductLifeInus (float)
                                        "",                          // 46. ItemDesc (string)
                                        0,                           // 47. MaxWipStock (float)
                                        "",                          // 48. NeedSo (string)
                                        "",                          // 49. BomRequired (string)
                                        "",                          // 50. JobWorkItem (string)
                                        Item.HSNNo,                  // 51. HsnNo (string) - EXTRA VALUE
                                        Empid,                       // 52. CreatedBy (int) - EXTRA VALUE
                                        today.ToString("yyyy-MM-dd").Split(" ")[0], // 53. CreatedOn (string) - EXTRA VALUE
                                        0,                           // 54. UpdatedBy (int) - EXTRA VALUE
                                        today.ToString("yyyy-MM-dd").Split(" ")[0], // 55. UpdatedOn (string) - EXTRA VALUE
                                        "Y",                          // 56. Active (string) - EXTRA VALUE
                                        Item.ItemServAssets          // 57. ItemServAssets (string) - EXTRA VALUE
                                    }

                );
        }
        MRGrid.Dispose();
        return MRGrid;
    }
    private static DataTable GetItemDetailTable(IList<ImportItemViewModel> DetailList, string CC, int Empid)
    {
        var MRGrid = new DataTable();

        // === Define all columns ===
        MRGrid.Columns.Add("Item_Code", typeof(int));
        MRGrid.Columns.Add("PartCode", typeof(string));
        MRGrid.Columns.Add("Item_Name", typeof(string));
        MRGrid.Columns.Add("ParentCode", typeof(int));
        MRGrid.Columns.Add("EntryDate", typeof(string));
        MRGrid.Columns.Add("LastUpdatedDate", typeof(string));
        MRGrid.Columns.Add("LeadTime", typeof(int));
        MRGrid.Columns.Add("CC", typeof(string));
        MRGrid.Columns.Add("Unit", typeof(string));
        MRGrid.Columns.Add("SalePrice", typeof(decimal));
        MRGrid.Columns.Add("PurchasePrice", typeof(decimal));
        MRGrid.Columns.Add("CostPrice", typeof(decimal));
        MRGrid.Columns.Add("WastagePercent", typeof(decimal));
        MRGrid.Columns.Add("WtSingleItem", typeof(decimal));
        MRGrid.Columns.Add("NoOfPcs", typeof(int));
        MRGrid.Columns.Add("QcReq", typeof(string));
        MRGrid.Columns.Add("ItemType", typeof(string));
        MRGrid.Columns.Add("UploadItemImage", typeof(string));
        MRGrid.Columns.Add("UploadImage", typeof(string));
        MRGrid.Columns.Add("UID", typeof(string));
        MRGrid.Columns.Add("DrawingNo", typeof(string));
        MRGrid.Columns.Add("MinimumLevel", typeof(decimal));
        MRGrid.Columns.Add("MaximumLevel", typeof(decimal));
        MRGrid.Columns.Add("ReorderLevel", typeof(decimal));
        MRGrid.Columns.Add("YearCode", typeof(string));
        MRGrid.Columns.Add("AlternateUnit", typeof(string));
        MRGrid.Columns.Add("RackID", typeof(string));
        MRGrid.Columns.Add("BinNo", typeof(string));
        MRGrid.Columns.Add("ItemSize", typeof(string));
        MRGrid.Columns.Add("Colour", typeof(string));
        MRGrid.Columns.Add("NeedPO", typeof(string));
        MRGrid.Columns.Add("StdPacking", typeof(int));
        MRGrid.Columns.Add("PackingType", typeof(string));
        MRGrid.Columns.Add("ModelNo", typeof(string));
        MRGrid.Columns.Add("YearlyConsumedQty", typeof(int));
        MRGrid.Columns.Add("DispItemName", typeof(string));
        MRGrid.Columns.Add("PurchaseAccountcode", typeof(string));
        MRGrid.Columns.Add("SaleAccountcode", typeof(string));
        MRGrid.Columns.Add("MinLevelDays", typeof(int));
        MRGrid.Columns.Add("MaxLevelDays", typeof(int));
        MRGrid.Columns.Add("EmpName", typeof(string));
        MRGrid.Columns.Add("DailyRequirment", typeof(int));
        MRGrid.Columns.Add("Stockable", typeof(string));
        MRGrid.Columns.Add("WipStockable", typeof(string));
        MRGrid.Columns.Add("Store", typeof(string));
        MRGrid.Columns.Add("ProductLifeInus", typeof(string));
        MRGrid.Columns.Add("ItemDesc", typeof(string));
        MRGrid.Columns.Add("MaxWipStock", typeof(int));
        MRGrid.Columns.Add("NeedSo", typeof(string));
        MRGrid.Columns.Add("BomRequired", typeof(string));
        MRGrid.Columns.Add("JobWorkItem", typeof(string));
        MRGrid.Columns.Add("HsnNo", typeof(int));
        MRGrid.Columns.Add("CreatedBy", typeof(int));
        MRGrid.Columns.Add("CreatedOn", typeof(string));
        MRGrid.Columns.Add("UpdatedBy", typeof(int));
        MRGrid.Columns.Add("UpdatedOn", typeof(string));
        MRGrid.Columns.Add("Active", typeof(string));
        MRGrid.Columns.Add("ItemServAssets", typeof(string));
        MRGrid.Columns.Add("VendorBatchcodeMand", typeof(string));
        MRGrid.Columns.Add("EntryByMachineName", typeof(string));
        MRGrid.Columns.Add("UniversalPartCode", typeof(string));
        MRGrid.Columns.Add("UniversalDescription", typeof(string));
        MRGrid.Columns.Add("ProdInWorkcenter", typeof(int));
        MRGrid.Columns.Add("ProdInhouseJW", typeof(string));
        MRGrid.Columns.Add("BatchNO", typeof(string));
        MRGrid.Columns.Add("VoltageValue", typeof(string));
        MRGrid.Columns.Add("SerialNo", typeof(string));
        MRGrid.Columns.Add("OldPartCode", typeof(string));
        MRGrid.Columns.Add("Package", typeof(string));
        MRGrid.Columns.Add("IsCustJWAdjMandatory", typeof(string));

        // === Add rows ===
        foreach (var item in DetailList)
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");

            MRGrid.Rows.Add(
                item.Item_Code,
                item.PartCode ?? "",
                item.Item_Name ?? "",
                item.ItemGroupCode,
                item.EntryDate ?? today,
                item.UpdatedOn?.ToString("yyyy-MM-dd") ?? today,
                item.LeadTime,
                CC,
                item.Unit ?? "",
                item.SalePrice,
                item.PurchasePrice,
                item.CostPrice,
                item.WastagePercent,
                item.WtSingleItem,
                item.NoOfPcs,
                item.QcReq ?? "",
                item.ItemType ?? "",
                "", "", // UploadItemImage, UploadImage
                  Empid,        
                item.DrawingNo ?? "",
                item.MinimumLevel,
                item.MaximumLevel,
                item.ReorderLevel,
                item.YearCode ?? "",
                item.AlternateUnit ?? "",
                item.RackID ?? "",
                item.BinNo ?? "",
                item.ItemSize ?? "",
                item.Colour ?? "",
                item.NeedPO ?? "N",
                item.StdPacking,
                item.PackingType ?? "",
                item.ModelNo ?? "",
                item.YearlyConsumedQty,
                item.DispItemName ?? "",
                item.PurchaseAccountcode ?? "",
                item.SaleAccountcode ?? "",
                item.MinLevelDays,
                item.MaxLevelDays,
                Empid,
                item.DailyRequirment,
                item.Stockable ?? "Y",
                item.WipStockable ?? "Y",
                item.Store ?? "",
                item.ProductLifeInus ?? "",
                item.ItemDesc ?? "",
                item.MaxWipStock,
                item.NeedSo ?? "",
                item.BomRequired ?? "",
                item.JobWorkItem ?? "",
                item.HSNNO,
                Empid,
                item.CreatedOn?.ToString("yyyy-MM-dd") ?? today,
                Empid,
                item.UpdatedOn?.ToString("yyyy-MM-dd") ?? today,
                item.Active ?? "Y",
                item.ItemServAssets ?? "",
                "", "", // VendorBatchcodeMand, EntryByMachineName
                item.UniversalPartCode ?? "",
                item.UniversalDescription ?? "",
                item.ProdInWorkcenter,
                item.ProdInhouseJW ?? "",
                item.BatchNO ?? "",
                item.VoltageVlue ?? "",
                item.SerialNo ?? "",
                item.OldPartCode ?? "",
                item.Package ?? "",
                item.IsCustJWAdjMandatory ?? "N"
            );
        }

        return MRGrid;
    } 
    private static DataTable GetSelectedItemDetailTable(IList<ImportItemViewModel> DetailList)
    {
        var MRGrid = new DataTable();

        // === Define all columns ===
        MRGrid.Columns.Add("Item_Code", typeof(int));
        MRGrid.Columns.Add("PartCode", typeof(string));
        MRGrid.Columns.Add("Item_Name", typeof(string));
        MRGrid.Columns.Add("MinimumLevel", typeof(decimal));
        MRGrid.Columns.Add("MaximumLevel", typeof(decimal));
        MRGrid.Columns.Add("HsnNo", typeof(int));
        MRGrid.Columns.Add("ProdInWorkcenter", typeof(int));
        MRGrid.Columns.Add("SalePrice", typeof(decimal));
        MRGrid.Columns.Add("CostPrice", typeof(decimal));
        MRGrid.Columns.Add("PurchasePrice", typeof(decimal));
        MRGrid.Columns.Add("Store", typeof(string));
        foreach (var item in DetailList)
        {
            MRGrid.Rows.Add(
                item.Item_Code,
                item.PartCode,
                item.Item_Name ,
                item.MinimumLevel,
                item.MaximumLevel,
                item.HSNNO,
                item.ProdInWorkcenter,
                item.SalePrice ,
                item.CostPrice ,
                item.PurchasePrice ,
                item.Store
            );
        }

        return MRGrid;
    }
}