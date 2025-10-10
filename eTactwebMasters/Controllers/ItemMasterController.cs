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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using DocumentFormat.OpenXml.EMMA;
using eTactWeb.Data.DAL;
using System.Data.SqlClient;

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
      //  worksheet.Cell(row, col++).Value = "Item_Code";
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
        else if(flag== "productiondetail")
        {
            worksheet.Cell(row, col++).Value = "NoOfCavity";
            worksheet.Cell(row, col++).Value = "NoOfshotsHours";
            worksheet.Cell(row, col++).Value = "ProdInMachineGroup";
            worksheet.Cell(row, col++).Value = "ProdInMachine1";
            worksheet.Cell(row, col++).Value = "ProdInMachine2";
            worksheet.Cell(row, col++).Value = "ProdInMachine3";
            worksheet.Cell(row, col).Value = "ProdInMachine4";

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
            worksheet.Cell(row, col++).Value = "MinimumLevel";
            worksheet.Cell(row, col++).Value = "NoOfCavity";
            worksheet.Cell(row, col++).Value = "NoOfshotsHours";
            worksheet.Cell(row, col++).Value = "ProdInMachineGroup";
            worksheet.Cell(row, col++).Value = "ProdInMachine1";
            worksheet.Cell(row, col++).Value = "ProdInMachine2";
            worksheet.Cell(row, col++).Value = "ProdInMachine3";
            worksheet.Cell(row, col).Value = "ProdInMachine4";
        }

        // Write data rows
        for (int i = 0; i < modelList.Count; i++)
        {
            int r = i + 2;
            int c = 1;

            worksheet.Cell(r, c++).Value = i + 1;
            //worksheet.Cell(r, c++).Value = modelList[i].Item_Code;
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
            else if (flag == "productiondetail")
            {
                worksheet.Cell(r, c++).Value = modelList[i].NoOfCavity;
                worksheet.Cell(r, c++).Value = modelList[i].NoOfshotsHours;
                worksheet.Cell(r, c++).Value = modelList[i].ProdInMachineGroupName;
                worksheet.Cell(r, c++).Value = modelList[i].ProdInMachineName1;
                worksheet.Cell(r, c++).Value = modelList[i].ProdInMachineName2;
                worksheet.Cell(r, c++).Value = modelList[i].ProdInMachineName3;
                worksheet.Cell(r, c).Value = modelList[i].ProdInMachineName4;
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
        "DispItemName", "PurchaseAccount", "SaleAccount", "MinLevelDays", "MaxLevelDays",
        "DailyRequirment", "Stockable", "WipStockable", "Store", "ProductLifeInus", "ItemDesc", "MaxWipStock",
        "NeedSo", "BomRequired", "HSNNO", "Universal Part Code", "Universal Description", "WorkCenterDescription",
        "ProdInhouseJW", "BatchNO", "VoltageValue", "SerialNo", "OldPartCode", "Package",
        "IsCustJWAdjMandatory", "Active", "JobWorkItem", "ItemServAssets","Branch","NoOfCavity","NoOfshotsHours","ChildBom",
        "ProdInMachineGroup","ProdInMachine1","ProdInMachine2","ProdInMachine3","ProdInMachine4"
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
            sheet.Cell(row, col++).Value = item.PurchaseAccountName;
            sheet.Cell(row, col++).Value = item.SaleAccountName;
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
            sheet.Cell(row, col++).Value = item.BranchName;
            sheet.Cell(row, col++).Value = item.NoOfCavity;
            sheet.Cell(row, col++).Value = item.NoOfshotsHours;
            sheet.Cell(row, col++).Value = item.ChildBom;
            sheet.Cell(row, col++).Value = item.ProdInMachineGroupName;
            sheet.Cell(row, col++).Value = item.ProdInMachineName1;
            sheet.Cell(row, col++).Value = item.ProdInMachineName2;
            sheet.Cell(row, col++).Value = item.ProdInMachineName3;
            sheet.Cell(row, col++).Value = item.ProdInMachineName4;
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
    public async Task<JsonResult> GetBranchList()
    {
        var JSON = await _IItemMaster.GetBranchList();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> ProdInMachineGroupList()
    {
        var JSON = await _IItemMaster.ProdInMachineGroupList();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> ProdInMachineList(int MachGroupId)
    {
        var JSON = await _IItemMaster.ProdInMachineList(MachGroupId);
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
            model.ChildBom = "N";
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
            string extension = Path.GetExtension(model.UploadImage.FileName)?.ToLowerInvariant();

            // Build safe parts
            string safePurchVouchNo = model.Item_Name.Replace("\\", "_").Replace("/", "_");

            string safeInvNo = model.PartCode.Replace("\\", "_").Replace("/", "_");
            ImagePath += safePurchVouchNo + "_" + model.YearCode + "_" + safeInvNo + "_" + model.EntryDate.Replace("\\", "_").Replace("/", "_") + "_" + "DrawimgImage" + "_" + Guid.NewGuid().ToString() + extension;

            //ImagePath += Guid.NewGuid().ToString() + "_" + model.UploadImage.FileName;
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
        else
        {
            model.ImageURL = model.ImageURL;
        }
        if (model.ItemImage != null)
        {
            //string ImagePath = "UploadedImages/";
            string ImagePath = "Uploads/ItemImage/";

            if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
            {
                Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
            }
            string extension = Path.GetExtension(model.UploadImage.FileName)?.ToLowerInvariant();

            // Build safe parts
            string safePurchVouchNo = model.Item_Name.Replace("\\", "_").Replace("/", "_");

            string safeInvNo = model.PartCode.Replace("\\", "_").Replace("/", "_");
            ImagePath += safePurchVouchNo + "_" + model.YearCode + "_" + safeInvNo + "_" + model.EntryDate.Replace("\\", "_").Replace("/", "_") + "_" + "ItemImage" + "_" + Guid.NewGuid().ToString() + extension;

            //   ImagePath += Guid.NewGuid().ToString() + "_" + model.ItemImage.FileName;
            model.ItemImageURL = "/" + ImagePath;
            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
            //await model.UploadImage.CopyToAsync(new FileStream(ServerPath, FileMode.Create));
            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
            {
                await model.ItemImage.CopyToAsync(FileStream);
            }

        }
        else
        {
            model.ItemImageURL = model.ItemImageURL;
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

        //model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HSNNO, UniversalPartCode, Flag);

        //HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(model.MasterList));

        return View(model);
    }
    public async Task<IActionResult> GetUpdateandImportDashBoardData()
    {
        string? Item_Name = "";
        string? PartCode = "";
        string? ParentCode = "";
        string? ItemType = "";
        string? HSNNO = "";
        string? UniversalPartCode = "";
        string? Flag = "";
         var model = new ItemMasterModel();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HSNNO, UniversalPartCode, Flag);
        HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(model.MasterList));
        return PartialView("_DisplayExcelselectedItemData", model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUploadExcel(IFormFile excelFile)
    {
        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<ImportItemViewModel> data = new List<ImportItemViewModel>();
            var errors = new List<string>(); // collect errors

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

                var ProdInMachineGroupList= _IItemMaster.ProdInMachineGroupList();
             
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
                    var ProdInMachineGroup = worksheet.Cells[row, headersMap["ProdInMachineGroup"]].Text?.Trim();
                    var ProdInMachine1 = worksheet.Cells[row, headersMap["ProdInMachine1"]].Text?.Trim();
                    var ProdInMachine2 = worksheet.Cells[row, headersMap["ProdInMachine2"]].Text?.Trim();
                    var ProdInMachine3 = worksheet.Cells[row, headersMap["ProdInMachine3"]].Text?.Trim();
                    var ProdInMachine4 = worksheet.Cells[row, headersMap["ProdInMachine4"]].Text?.Trim();
                    var Branch = worksheet.Cells[row, headersMap["Branch"]].Text?.Trim();

                    var PurchaseAccount = worksheet.Cells[row, headersMap["PurchaseAccount"]].Text?.Trim();
                    var SaleAccount = worksheet.Cells[row, headersMap["SaleAccount"]].Text?.Trim();

                    var duplicatePartCode = _IDataLogic.isDuplicate(partCode, "PartCode", "Item_Master");
                    var duplicateItemName = _IDataLogic.isDuplicate(ItemName, "Item_Name", "Item_Master");

                    var itemPurchaseAccountCode = _IItemMaster.GetAccountCode(PurchaseAccount);
                    var itemSaleAccountCode = _IItemMaster.GetAccountCode(SaleAccount);
                    var itemGroupCode = _IItemMaster.GetItemGroupCode(itemGroup);
                    var itemCatCode = _IItemMaster.GetItemCatCode(itemType);
                    var WorkCenterId = _IItemMaster.GetWorkCenterId(workCenter);
                    var StoreIdResult = _IItemMaster.GetStoreCode(Store);
                    var Branchlist = _IItemMaster.GetBranchList();
                    var ProdInMachineGroupId = _IItemMaster.ProdInMachineGroupId(ProdInMachineGroup);
                    var ProdInMachineNameId1 = _IItemMaster.ProdInMachineNameId(ProdInMachine1);
                    var ProdInMachineNameId2 = _IItemMaster.ProdInMachineNameId(ProdInMachine2);
                    var ProdInMachineNameId3 = _IItemMaster.ProdInMachineNameId(ProdInMachine3);
                    var ProdInMachineNameId4 = _IItemMaster.ProdInMachineNameId(ProdInMachine4);
                   

                    var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 1 ? "Y" : "N";
                    var ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : (Convert.ToInt32(duplicateItemName.Result) > 1 ? "Y" : "N");
                    var ItemServAssets = worksheet.Cells[row, headersMap["ItemServAssets"]].Text?.Trim();

                    var validItemServAssetsOptions = new List<string> { "Item", "Service", "Asset" };

                    if (!validItemServAssetsOptions.Contains(ItemServAssets, StringComparer.OrdinalIgnoreCase))
                    {
                        errors.Add($"Invalid ItemServAssets: {ItemServAssets} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;
                    }
                    string hsnString = worksheet.Cells[row, headersMap["HSNNO"]] .Value?.ToString().Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(hsnString))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(hsnString, @"^\d{8}$"))
                        {
                            errors.Add($"Invalid HSNNo: {hsnString} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;

                        }
                    }
                        //if (PartCodeExists == "Y")
                        //{
                        //    errors.Add($"Duplicate PartCode found: {partCode} at row {row} (ItemName: {ItemName})");
                        //    continue;
                        //}

                        //if (ItemNameExists == "Y")
                        //{
                        //    errors.Add($"Duplicate ItemName found: {ItemName} at  row {row} (PartCode: {partCode})");
                        //    continue;
                        //}

                        var ItemGroupList = _IItemMaster.GetItemGroup(ItemServAssets);
                    var ItemCategoryList = _IItemMaster.GetItemCategory(ItemServAssets);

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
                        errors.Add($"Invalid Unit: {unit} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;

                    }
                    if (!string.IsNullOrWhiteSpace(Branch))
                    {
                        var branchList = Branch?.Split(',') ?? Array.Empty<string>();

                        var Branchdataset = Branchlist.Result.Result;
                        var BranchTable = Branchdataset.Tables[0];

                        foreach (var branchName in branchList)
                        {
                            bool exists = false;

                            foreach (DataRow rows in BranchTable.Rows)
                            {
                                if (rows["BranchName"].ToString().Trim()
                                    .Equals(branchName.Trim(), StringComparison.OrdinalIgnoreCase))
                                {
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                errors.Add($"Invalid Branch: {branchName.Trim()} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
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
                        errors.Add($"Invalid ItemGroup: {itemGroup} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;
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
                        errors.Add($"Invalid ItemType: {itemType} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;

                    }

                    ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : ItemNameExists;

                    int itemGCode = 1;
                    int itemCCode = 1;
                    int itemWorkCenterId = 0;
                    int itemStoreId = 0;
                    int itemPurchaseAccCode = 0;
                    int itemSaleAccCode = 0;
                    int itemProdInmachineGroup = 0;

                    if (itemGroupCode.Result.Result != null && itemGroupCode.Result.Result.Rows.Count > 0)
                    {
                        itemGCode = (int)itemGroupCode.Result.Result.Rows[0].ItemArray[0];
                    }
                    if (itemPurchaseAccountCode.Result.Result != null && itemPurchaseAccountCode.Result.Result.Rows.Count > 0)
                    {
                        itemPurchaseAccCode = (int)itemPurchaseAccountCode.Result.Result.Rows[0].ItemArray[0];
                    }
                    if (itemSaleAccountCode.Result.Result != null && itemSaleAccountCode.Result.Result.Rows.Count > 0)
                    {
                        itemSaleAccCode = (int)itemSaleAccountCode.Result.Result.Rows[0].ItemArray[0];
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
                    if (ProdInMachineGroupId.Result.Result != null && ProdInMachineGroupId.Result.Result.Rows.Count > 0)
                    {
                        itemProdInmachineGroup = (int)ProdInMachineGroupId.Result.Result.Rows[0].ItemArray[0];
                    }
                    int itemProdInmachine1 = 0;
                    if (ProdInMachineNameId1.Result.Result != null && ProdInMachineNameId1.Result.Result.Rows.Count > 0)
                    {
                        itemProdInmachine1 = (int)ProdInMachineNameId1.Result.Result.Rows[0].ItemArray[0];
                    }
                    int itemProdInmachine2 = 0;
                    if (ProdInMachineNameId2.Result.Result != null && ProdInMachineNameId2.Result.Result.Rows.Count > 0)
                    {
                        itemProdInmachine2 = (int)ProdInMachineNameId2.Result.Result.Rows[0].ItemArray[0];
                    }   
                    int itemProdInmachine3 = 0;
                    if (ProdInMachineNameId3.Result.Result != null && ProdInMachineNameId3.Result.Result.Rows.Count > 0)
                    {
                        itemProdInmachine3 = (int)ProdInMachineNameId3.Result.Result.Rows[0].ItemArray[0];
                    }
                    int itemProdInmachine4 = 0;
                    if (ProdInMachineNameId4.Result.Result != null && ProdInMachineNameId4.Result.Result.Rows.Count > 0)
                    {
                        itemProdInmachine4 = (int)ProdInMachineNameId4.Result.Result.Rows[0].ItemArray[0];
                    }

                    bool itemProdInmachineGroupExists = false;

                    if (!string.IsNullOrWhiteSpace(ProdInMachineGroup))
                    {
                      
                        var dataset = ProdInMachineGroupList.Result.Result;
                        var Table = dataset.Tables[0];

                        foreach (DataRow rows in Table.Rows)
                        {
                            if (rows["ProdInMachineGroup"].ToString().Trim().Equals(ProdInMachineGroup, StringComparison.OrdinalIgnoreCase))
                            {
                                itemProdInmachineGroupExists = true;
                                break;
                            }
                        }

                        if (!itemProdInmachineGroupExists)
                        {
                            errors.Add($"Invalid ProdInmachineGroup: {ProdInMachineGroup} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }
                    bool itemProdInmachine1Exists = false;

                    if (!string.IsNullOrWhiteSpace(ProdInMachine1))
                    {
                        var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                        var dataset = ProdInMachineList.Result.Result;
                        var Table = dataset.Tables[0];

                        foreach (DataRow rows in Table.Rows)
                        {
                            if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine1, StringComparison.OrdinalIgnoreCase))
                            {
                                itemProdInmachine1Exists = true;
                                break;
                            }
                        }

                        if (!itemProdInmachine1Exists)
                        {
                            errors.Add($"Invalid ProdInmachine1: {ProdInMachine1} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }
                    bool itemProdInmachine2Exists = false;

                    if (!string.IsNullOrWhiteSpace(ProdInMachine2))
                    {
                       
                        var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                        var dataset = ProdInMachineList.Result.Result;
                        var Table = dataset.Tables[0];

                        foreach (DataRow rows in Table.Rows)
                        {
                            if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine2, StringComparison.OrdinalIgnoreCase))
                            {
                                itemProdInmachine2Exists = true;
                                break;
                            }
                        }

                        if (!itemProdInmachine2Exists)
                        {
                            errors.Add($"Invalid ProdInmachine2: {ProdInMachine2} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }
                    bool itemProdInmachine3Exists = false;

                    if (!string.IsNullOrWhiteSpace(ProdInMachine3))
                    {
                        var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                        var dataset = ProdInMachineList.Result.Result;
                        var Table = dataset.Tables[0];

                        foreach (DataRow rows in Table.Rows)
                        {
                            if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine3, StringComparison.OrdinalIgnoreCase))
                            {
                                itemProdInmachine3Exists = true;
                                break;
                            }
                        }

                        if (!itemProdInmachine3Exists)
                        {
                            errors.Add($"Invalid ProdInmachine3: {ProdInMachine3} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }
                    bool itemProdInmachine4Exists = false;

                    if (!string.IsNullOrWhiteSpace(ProdInMachine4))
                    {
                        var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                        var dataset = ProdInMachineList.Result.Result;
                        var Table = dataset.Tables[0];

                        foreach (DataRow rows in Table.Rows)
                        {
                            if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine4, StringComparison.OrdinalIgnoreCase))
                            {
                                itemProdInmachine4Exists = true;
                                break;
                            }
                        }

                        if (!itemProdInmachine4Exists)
                        {
                            errors.Add($"Invalid ProdInmachine4: {ProdInMachine4} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
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
                            errors.Add($"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }

                    bool PurchaseAccountNameExists = false;

                    if (!string.IsNullOrWhiteSpace(PurchaseAccount))
                    {

                        var purchaseAccountList = await _IDataLogic.GetDropDownList("Account_Head_Master_PA", "SP_GetDropDownList");

                        foreach (var item in purchaseAccountList)
                        {
                            if (item.Text.Trim().Equals(PurchaseAccount, StringComparison.OrdinalIgnoreCase))
                            {
                                PurchaseAccountNameExists = true;
                                break;
                            }
                        }

                        if (!PurchaseAccountNameExists)
                        {
                            errors.Add($"Invalid PurchaseAccount: {PurchaseAccount} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //return StatusCode(207, $"Invalid PurchaseAccountCode: {PurchaseAccount} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }


                    bool SaleAccountNameExists = false;

                    if (!string.IsNullOrWhiteSpace(SaleAccount))
                    {
                        var SaleAccountNameList = await _IDataLogic.GetDropDownList("Account_Head_Master_SA", "SP_GetDropDownList");

                        foreach (var item in SaleAccountNameList)
                        {
                            if (item.Text.Trim().Equals(SaleAccount, StringComparison.OrdinalIgnoreCase))
                            {
                                SaleAccountNameExists = true;
                                break;
                            }

                        }

                        if (!SaleAccountNameExists)
                        {
                            errors.Add($"Invalid SaleAccountCode: {SaleAccount} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                            //  return StatusCode(207, $"Invalid SaleAccountName: {SaleAccount} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        }
                    }

                    var machines = new List<string>
                    {
                           ProdInMachine1?.Trim(),
                            ProdInMachine2?.Trim(),
                            ProdInMachine3?.Trim(),
                            ProdInMachine4?.Trim()
                    }.Where(m => !string.IsNullOrWhiteSpace(m)) .ToList();
                    var duplicateMachines = machines
                     .GroupBy(m => m, StringComparer.OrdinalIgnoreCase)
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key)
                     .ToList();

                    if (duplicateMachines.Any())
                    {
                        errors.Add($"Machine names must be unique in row {row}. Duplicates: {string.Join(", ", duplicateMachines)} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;
                    }

                    bool StoreExists = false;

                    // Store is mandatory — check blank first
                    if (string.IsNullOrWhiteSpace(Store))
                    {
                        errors.Add($"Store is mandatory at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;
                    }

                    // If not blank, check if it exists in DB
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
                        errors.Add($"Invalid StoreName: {Store} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                        continue;
                    }


                    decimal minLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MinimumLevel"]].Value ?? 0);
                    decimal maxLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MaximumLevel"]].Value ?? 0);
                    decimal reorderLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["ReorderLevel"]].Value ?? 0);

                    if (maxLevel != 0)
                    {
                        if (minLevel != 0 && minLevel > maxLevel)
                        {
                            errors.Add($"Invalid Levels at row {row}: MinimumLevel ({minLevel}) cannot be greater than MaximumLevel ({maxLevel}).");
                            continue;
                            //return StatusCode(207, $"Invalid Levels at row {row}: MinimumLevel ({minLevel}) cannot be greater than MaximumLevel ({maxLevel}).");
                        }

                        if (reorderLevel != 0)
                        {
                            if ((minLevel != 0 && reorderLevel < minLevel) || reorderLevel > maxLevel)
                            {
                                errors.Add($"Invalid ReorderLevel at row {row}: ReorderLevel ({reorderLevel}) must be between {(minLevel != 0 ? minLevel : 0)} and {maxLevel}.");
                                continue;
                                //  return StatusCode(207, $"Invalid ReorderLevel at row {row}: ReorderLevel ({reorderLevel}) must be between {(minLevel != 0 ? minLevel : 0)} and {maxLevel}.");
                            }
                        }
                    }
                    string batchNO = worksheet.Cells[row, headersMap["BatchNO"]].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(batchNO))
                    {
                        var validBatchOptions = new List<string> { "MRNWISE", "NOOFCase", "ForEachQty" };

                        if (!validBatchOptions.Contains(batchNO, StringComparer.OrdinalIgnoreCase))
                        {
                            errors.Add($"Invalid BatchNO value at row {row}: '{batchNO}'. Valid options are MRNWISE, NOOFCase, ForEachQty.");
                            continue;
                            //   return StatusCode(207, $"Invalid BatchNO value at row {row}: '{batchNO}'. Valid options are MRNWISE, NOOFCase, ForEachQty.");
                        }
                    }
                    //var ItemCodeId = _IItemMaster.GetItemCode(partCode, worksheet.Cells[row, headersMap["Item_Name"]].Text?.Trim());
                    //int itemcode = 0;
                    //if (ItemCodeId.Result.Result != null && ItemCodeId.Result.Result.Rows.Count > 0)
                    //{
                    //    itemcode = (int)ItemCodeId.Result.Result.Rows[0].ItemArray[0];
                    //}
                    var ItemCodeId = _IItemMaster.GetItemCode(partCode, ItemName);
                    int itemcode = 0;
                    if (ItemCodeId.Result.Result != null && ItemCodeId.Result.Result.Tables.Count > 0)
                    {
                        var table = ItemCodeId.Result.Result.Tables[0];
                        if (table.Rows.Count > 0)
                        {
                            itemcode = Convert.ToInt32(table.Rows[0].ItemArray[0]);
                        }
                        // itemcode = (int)ItemCodeId.Result.Result.Rows[0].ItemArray[0];
                    }
                    data.Add(new ImportItemViewModel
                    {
                        Item_Code = itemcode,
                        PartCode = partCode,
                        Item_Name = worksheet.Cells[row, headersMap["Item_Name"]].Text?.Trim(),
                        ItemGroup = itemGroup,
                        ItemType = itemType,
                        TypeName = itemType,
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
                        Branch = worksheet.Cells[row, headersMap["Branch"]].Text?.Trim(),
                        ItemGroupCode = itemGCode,
                        ItemCategoryCode = itemCCode,
                        ProdInWorkcenter = itemWorkCenterId,
                        ProdInMachineGroupId= itemProdInmachineGroup,
                        ProdInMachineGroupName = ProdInMachineGroup,
                        ProdInMachineName1= ProdInMachine1,
                        ProdInMachineName2= ProdInMachine2,
                        ProdInMachineName3= ProdInMachine3,
                        ProdInMachineName4= ProdInMachine4,
                        ProdInMachine1=itemProdInmachine1,
                        ProdInMachine2=itemProdInmachine2,
                        ProdInMachine3=itemProdInmachine3,
                        ProdInMachine4=itemProdInmachine4,
                        PurchaseAccountcode = itemPurchaseAccCode.ToString(),
                        SaleAccountcode = itemSaleAccCode.ToString(),
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
                        PurchaseAccountName = worksheet.Cells[row, headersMap["PurchaseAccount"]].Text?.Trim(),
                        SaleAccountName = worksheet.Cells[row, headersMap["SaleAccount"]].Text?.Trim(),
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
                        NoOfCavity = Convert.ToInt32(worksheet.Cells[row, headersMap["NoOfCavity"]].Text?.Trim()),
                        NoOfshotsHours = Convert.ToInt32(worksheet.Cells[row, headersMap["NoOfshotsHours"]].Text?.Trim()),
                        ChildBom = worksheet.Cells[row, headersMap["ChildBom"]].Text?.Trim(),
                    
                        SeqNo = row - 1
                    });

                }
            }
            if (errors.Any())
            {
                return BadRequest(new { errors });
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
            var totalColumns = worksheet.Dimension.Columns;
            var headersMap = new Dictionary<string, int>();
            for (int col = 1; col <= totalColumns; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header) && !headersMap.ContainsKey(header))
                    headersMap[header] = col;
            }
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var cellValue = worksheet.Cells[row, 2].Value;
                var partCode = worksheet.Cells[row, headersMap["PartCode"]].Text?.Trim();
                //worksheet.Cells[row, 3].Value?.ToString().Trim();
                var ItemName = worksheet.Cells[row, headersMap["Item_Name"]].Text?.Trim();
                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    break;
                var ItemCodeId = _IItemMaster.GetItemCode(partCode, ItemName);
                int itemcode = 0;
                if (ItemCodeId.Result.Result != null && ItemCodeId.Result.Result.Tables.Count > 0)
                {
                    var table = ItemCodeId.Result.Result.Tables[0];
                    if (table.Rows.Count > 0)
                    {
                        itemcode = Convert.ToInt32(table.Rows[0].ItemArray[0]);
                    }
                   // itemcode = (int)ItemCodeId.Result.Result.Rows[0].ItemArray[0];
                }
                var model = new ImportItemViewModel
                {
                    Item_Code = itemcode,
                    PartCode = partCode,
                    Item_Name = ItemName
                };

                switch (flag?.ToLower())
                {
                   case "hsncode":
                    string hsnString = worksheet.Cells[row, headersMap["HSNCODE"]].Text?.Trim() ?? string.Empty; 
                       // worksheet.Cells[row, 5].Value?.ToString().Trim() ?? string.Empty;

                    if (!string.IsNullOrEmpty(hsnString))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(hsnString, @"^\d{8}$"))
                        {
                                errors.Add($"Invalid HSNNo: {hsnString} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                             
                        }

                       
                    }
                        model.HSNNO = Convert.ToInt32(hsnString);
                        break;


                    case "store":
                        var Store = worksheet.Cells[row, headersMap["StoreName"]].Text?.Trim() ?? "";
                            //worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                        bool StoreExists = false;
                        if (string.IsNullOrWhiteSpace(Store))
                        {
                            errors.Add($"Store is mandatory at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                        }
                       
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
                                errors.Add($"Invalid StoreName: {Store} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                            }
                        
                        model.StoreName = Store;
                        var StoreIdResult = _IItemMaster.GetStoreCode(Store);
                        if (StoreIdResult.Result.Result.Rows.Count > 0)
                        {
                            model.Store = StoreIdResult.Result.Result.Rows[0].ItemArray[0]?.ToString();
                        }
                        break;

                    case "workcenter":
                        var WorkCenter = worksheet.Cells[row, headersMap["WorkCenter"]].Text?.Trim()??"";
                        //worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
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
                                errors.Add($"Invalid Workcenter: {WorkCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
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
                        model.SalePrice = Convert.ToDecimal(worksheet.Cells[row, headersMap["SalePrice"]].Text?.Trim() ?? "0");

                        //Convert.ToDecimal(worksheet.Cells[row, 5].Value ?? 0);
                        model.PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, headersMap["PurchasePrice"]].Text?.Trim() ?? "0");
                        //Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0);
                        model.CostPrice = Convert.ToDecimal(worksheet.Cells[row, headersMap["CostPrice"]].Text?.Trim() ?? "0");
                        //Convert.ToDecimal(worksheet.Cells[row, 7].Value ?? 0);
                        break;
                    case "productiondetail":

                        //var ProdInMachineGroup = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? "";
                        //var ProdInMachine1 = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? "";
                        //var ProdInMachine2 = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? "";
                        //var ProdInMachine3 = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? "";
                        //var ProdInMachine4 = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? "";
                        var ProdInMachineGroup = worksheet.Cells[row, headersMap["ProdInMachineGroup"]].Text?.Trim() ?? "";
                        var ProdInMachine1 = worksheet.Cells[row, headersMap["ProdInMachine1"]].Text?.Trim() ?? "";
                        var ProdInMachine2 = worksheet.Cells[row, headersMap["ProdInMachine2"]].Text?.Trim() ?? "";
                        var ProdInMachine3 = worksheet.Cells[row, headersMap["ProdInMachine3"]].Text?.Trim() ?? "";
                        var ProdInMachine4 = worksheet.Cells[row, headersMap["ProdInMachine4"]].Text?.Trim() ?? "";
                     
                        var ProdInMachineGroupId = _IItemMaster.ProdInMachineGroupId(ProdInMachineGroup);
                        var ProdInMachineNameId1 = _IItemMaster.ProdInMachineNameId(ProdInMachine1);
                        var ProdInMachineNameId2 = _IItemMaster.ProdInMachineNameId(ProdInMachine2);
                        var ProdInMachineNameId3 = _IItemMaster.ProdInMachineNameId(ProdInMachine3);
                        var ProdInMachineNameId4 = _IItemMaster.ProdInMachineNameId(ProdInMachine4);
                        int itemProdInmachineGroup = 0;
                        if (ProdInMachineGroupId.Result.Result != null && ProdInMachineGroupId.Result.Result.Rows.Count > 0)
                        {
                            itemProdInmachineGroup = (int)ProdInMachineGroupId.Result.Result.Rows[0].ItemArray[0];
                        }
                        int itemProdInmachine1 = 0;
                        if (ProdInMachineNameId1.Result.Result != null && ProdInMachineNameId1.Result.Result.Rows.Count > 0)
                        {
                            itemProdInmachine1 = (int)ProdInMachineNameId1.Result.Result.Rows[0].ItemArray[0];
                        }
                        int itemProdInmachine2 = 0;
                        if (ProdInMachineNameId2.Result.Result != null && ProdInMachineNameId2.Result.Result.Rows.Count > 0)
                        {
                            itemProdInmachine2 = (int)ProdInMachineNameId2.Result.Result.Rows[0].ItemArray[0];
                        }
                        int itemProdInmachine3 = 0;
                        if (ProdInMachineNameId3.Result.Result != null && ProdInMachineNameId3.Result.Result.Rows.Count > 0)
                        {
                            itemProdInmachine3 = (int)ProdInMachineNameId3.Result.Result.Rows[0].ItemArray[0];
                        }
                        int itemProdInmachine4 = 0;
                        if (ProdInMachineNameId4.Result.Result != null && ProdInMachineNameId4.Result.Result.Rows.Count > 0)
                        {
                            itemProdInmachine4 = (int)ProdInMachineNameId4.Result.Result.Rows[0].ItemArray[0];
                        }

                        bool itemProdInmachineGroupExists = false;

                        if (!string.IsNullOrWhiteSpace(ProdInMachineGroup))
                        {
                            var ProdInMachineGroupList = _IItemMaster.ProdInMachineGroupList();

                            var dataset = ProdInMachineGroupList.Result.Result;
                            var Table = dataset.Tables[0];

                            foreach (DataRow rows in Table.Rows)
                            {
                                if (rows["ProdInMachineGroup"].ToString().Trim().Equals(ProdInMachineGroup, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemProdInmachineGroupExists = true;
                                    break;
                                }
                            }

                            if (!itemProdInmachineGroupExists)
                            {
                                errors.Add($"Invalid ProdInmachineGroup: {ProdInMachineGroup} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                                //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
                        bool itemProdInmachine1Exists = false;

                        if (!string.IsNullOrWhiteSpace(ProdInMachine1))
                        {
                            var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                            var dataset = ProdInMachineList.Result.Result;
                            var Table = dataset.Tables[0];

                            foreach (DataRow rows in Table.Rows)
                            {
                                if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine1, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemProdInmachine1Exists = true;
                                    break;
                                }
                            }

                            if (!itemProdInmachine1Exists)
                            {
                                errors.Add($"Invalid ProdInmachine1: {ProdInMachine1} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                                //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
                        bool itemProdInmachine2Exists = false;

                        if (!string.IsNullOrWhiteSpace(ProdInMachine2))
                        {

                            var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                            var dataset = ProdInMachineList.Result.Result;
                            var Table = dataset.Tables[0];

                            foreach (DataRow rows in Table.Rows)
                            {
                                if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine2, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemProdInmachine2Exists = true;
                                    break;
                                }
                            }

                            if (!itemProdInmachine2Exists)
                            {
                                errors.Add($"Invalid ProdInmachine2: {ProdInMachine2} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                                //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
                        bool itemProdInmachine3Exists = false;

                        if (!string.IsNullOrWhiteSpace(ProdInMachine3))
                        {
                            var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                            var dataset = ProdInMachineList.Result.Result;
                            var Table = dataset.Tables[0];

                            foreach (DataRow rows in Table.Rows)
                            {
                                if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine3, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemProdInmachine3Exists = true;
                                    break;
                                }
                            }

                            if (!itemProdInmachine3Exists)
                            {
                                errors.Add($"Invalid ProdInmachine3: {ProdInMachine3} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                                //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
                        bool itemProdInmachine4Exists = false;

                        if (!string.IsNullOrWhiteSpace(ProdInMachine4))
                        {
                            var ProdInMachineList = _IItemMaster.ProdInMachineList(itemProdInmachineGroup);
                            var dataset = ProdInMachineList.Result.Result;
                            var Table = dataset.Tables[0];

                            foreach (DataRow rows in Table.Rows)
                            {
                                if (rows["MachineName"].ToString().Trim().Equals(ProdInMachine4, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemProdInmachine4Exists = true;
                                    break;
                                }
                            }

                            if (!itemProdInmachine4Exists)
                            {
                                errors.Add($"Invalid ProdInmachine4: {ProdInMachine4} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                                continue;
                                //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                            }
                        }
                        var prodInMachineNames = new List<string>
                        {
                            ProdInMachine1,
                           ProdInMachine2,
                            ProdInMachine3,
                            ProdInMachine4
                        }.Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
                        var duplicateMachines = prodInMachineNames
                         .GroupBy(m => m, StringComparer.OrdinalIgnoreCase)
                         .Where(g => g.Count() > 1)
                         .Select(g => g.Key)
                         .ToList();

                        if (duplicateMachines.Any())
                        {
                            errors.Add($"Machine names must be unique in row {row}. Duplicates: {string.Join(", ", duplicateMachines)} (PartCode: {partCode}, ItemName: {ItemName})");
                            continue;
                        }
                        //model.NoOfCavity = Convert.ToInt32(worksheet.Cells[row, 5].Value ?? 0);
                        //model.NoOfshotsHours = Convert.ToInt32(worksheet.Cells[row, 6].Value ?? 0);   
                        model.NoOfCavity = Convert.ToInt32(worksheet.Cells[row, headersMap["NoOfCavity"]].Value??0);
                        model.NoOfshotsHours = Convert.ToInt32(worksheet.Cells[row, headersMap["NoOfshotsHours"]].Value??0);
                        model.ProdInMachineGroupId = itemProdInmachineGroup;
                        model.ProdInMachineGroupName = ProdInMachineGroup;
                        model.ProdInMachineName1 = ProdInMachine1;
                        model.ProdInMachineName2 = ProdInMachine2;
                        model.ProdInMachineName3 = ProdInMachine3;
                        model.ProdInMachineName4 = ProdInMachine4;
                        model.ProdInMachine1 = itemProdInmachine1;
                        model.ProdInMachine2 = itemProdInmachine2;
                        model.ProdInMachine3 = itemProdInmachine3;
                        model.ProdInMachine4 = itemProdInmachine4;
                       
                        break;
                    case "minmaxlevel":
                        //model.MaximumLevel = Convert.ToDecimal(worksheet.Cells[row, 5].Value ?? 0);
                        //model.MinimumLevel = Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0);
                        //decimal minLevel = Convert.ToDecimal(worksheet.Cells[row, 5].Value ?? 0);
                        //decimal maxLevel = Convert.ToDecimal(worksheet.Cells[row, 6].Value ?? 0); 
                        model.MaximumLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MaximumLevel"]].Value ?? 0);
                        model.MinimumLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MinimumLevel"]].Value ?? 0);
                        decimal minLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MinimumLevel"]].Value ?? 0);
                        decimal maxLevel = Convert.ToDecimal(worksheet.Cells[row, headersMap["MaximumLevel"]].Value ?? 0);

                        if (maxLevel != 0)
                        {
                            if (minLevel != 0 && minLevel > maxLevel)
                            {
                                errors.Add($"Row {row}: Invalid Levels at row {row}: MinimumLevel ({minLevel}) cannot be greater than MaximumLevel ({maxLevel})");
                                continue;
                            }
                        }
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
            ProdInWorkcenter = x.ProdInWorkcenter,
            NoOfCavity=x.NoOfCavity,
            NoOfshotsHours=x.NoOfshotsHours,
            ProdInMachineGroupName = x.ProdInMachineGroupName,
            ProdInMachineGroupId=x.ProdInMachineGroupId,
            ProdInMachine1=x.ProdInMachine1,
            ProdInMachine2=x.ProdInMachine2,
            ProdInMachine3=x.ProdInMachine3,
            ProdInMachine4=x.ProdInMachine4,
            ProdInMachineName1=x.ProdInMachineName1,
            ProdInMachineName2=x.ProdInMachineName2,
            ProdInMachineName3=x.ProdInMachineName3,
            ProdInMachineName4=x.ProdInMachineName4,
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
            Dictionary<string, int> partCodeTracker = new Dictionary<string, int>();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var cellValue = worksheet.Cells[row, 2].Value;

                if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    break; // Stop when column 1 is empty
                var itemGroupCode = _IItemMaster.GetItemGroupCode(worksheet.Cells[row, 5].Value.ToString().Trim() ?? "");
                var itemCatCode = _IItemMaster.GetItemCatCode(
                    worksheet.Cells[row, 6].Value?.ToString().Trim() ?? ""
                );
                var dupPartCode = worksheet.Cells[row, 1].Value?.ToString().Trim() ?? "";
                var duplicatePartCode = _IDataLogic.isDuplicate(dupPartCode, "PartCode", "Item_Master");
                //var duplicatePartCode = _IDataLogic.isDuplicate(worksheet.Cells[row, 1].Value.ToString().Trim() ?? "", "PartCode", "Item_Master");
                var duplicateItemName = _IDataLogic.isDuplicate(worksheet.Cells[row, 2].Value.ToString().Trim() ?? "", "Item_Name", "Item_Master");
                var workCenter = worksheet.Cells[row, 15].Value?.ToString().Trim() ?? "";
                var Store = worksheet.Cells[row, 16].Value?.ToString().Trim() ?? "";
                var RackId = worksheet.Cells[row, 17].Value?.ToString().Trim() ?? "";
                var Barcode = worksheet.Cells[row, 18].Value?.ToString().Trim() ?? "";

                //var workCenter = worksheet.Cells[row, headersMap["WorkCenterDescription"]].Text?.Trim();
                //var Store = worksheet.Cells[row, headersMap["Store"]].Text?.Trim();
                var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 0 ? "Y" : "N";
                var ItemNameExists = Convert.ToInt32(duplicateItemName.Result) > 0 ? "Y" : "N";
                var WorkCenterId = _IItemMaster.GetWorkCenterId(workCenter);
               
                var StoreIdResult = _IItemMaster.GetStoreCode(Store);
                
                var partCode = worksheet.Cells[row, 1].Value?.ToString().Trim() ?? "";

                var ItemName = worksheet.Cells[row, 2].Value.ToString().Trim();
                var featureOption = _IItemMaster.GetFeatureOption();
               
                string hsnNoValue = worksheet.Cells[row, 4].Value?.ToString().Trim() ?? string.Empty;

                if (!string.IsNullOrEmpty(hsnNoValue))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(hsnNoValue, @"^\d{8}$"))
                    {
                        return StatusCode(207, $"Invalid HSNNo: {hsnNoValue} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");

                        //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                    }
                   
                }
              
                var ItemServAssets = GetCellValue(worksheet, row, 14);
                var validItemServAssetsOptions = new List<string> { "Item", "Service", "Asset" };

                if (!validItemServAssetsOptions.Contains(ItemServAssets, StringComparer.OrdinalIgnoreCase))
                {
                    return StatusCode(207, $"Invalid ItemServAssets: {ItemServAssets} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");

                }

                var unit = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";

                var unitdataset = UnitList.Result.Result;
                var unitTable = unitdataset.Tables[0];
                int itemWorkCenterId = 0;
                int itemStoreId = 0;
                if (WorkCenterId.Result.Result != null && WorkCenterId.Result.Result.Rows.Count > 0)
                {
                    itemWorkCenterId = (int)WorkCenterId.Result.Result.Rows[0].ItemArray[0];
                }
                if (StoreIdResult.Result.Result != null && StoreIdResult.Result.Result.Rows.Count > 0)
                {
                    itemStoreId = (int)StoreIdResult.Result.Result.Rows[0].ItemArray[0];
                }
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
                        
                        //return StatusCode(207, $"Invalid Workcenter: {workCenter} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                    }
                }

                bool StoreExists = false;
                //if (string.IsNullOrWhiteSpace(Store))
                //{
                //    return StatusCode(207, $"Store is mandatory at row: {Store} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");
                //}
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

                        //  return StatusCode(207, $"Invalid StoreName: {Store} at row {row} (PartCode: {partCode}, ItemName: {ItemName})");

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

                string GetCellValue(ExcelWorksheet sheet, int r, int c) =>sheet.Cells[r, c].Value?.ToString().Trim() ?? string.Empty;

                int GetCellIntValue(ExcelWorksheet sheet, int r, int c) =>
                    int.TryParse(sheet.Cells[r, c].Value?.ToString().Trim(), out int result) ? result : 0;
               // string PcValue = GetCellValue(worksheet, row, 1);
               // string newPartCode ="";
               //if (string.IsNullOrEmpty(PcValue))
               // {
               //     var result = GetPartCode(itemGCode, itemCCode);

               //     var partCodeResult = (result.Result.Value != null) ? result.Result.Value.ToString() : string.Empty;
               //     using JsonDocument doc = JsonDocument.Parse(partCodeResult);
               //     JsonElement root = doc.RootElement;

               //     string dbCode = doc.RootElement
               //           .GetProperty("Result")
               //           .GetProperty("Table")[0]
               //           .GetProperty("partcode")
               //           .GetString();  // Example: "1ASS78589"

               //     // Split numeric suffix
               //     string numberPart = new string(dbCode.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
               //     string prefix = dbCode.Substring(0, dbCode.Length - numberPart.Length);

               //     int lastNum = string.IsNullOrEmpty(numberPart) ? 0 : int.Parse(numberPart);

               //     if (!partCodeTracker.ContainsKey(prefix))
               //     {
               //         partCodeTracker[prefix] = lastNum;
               //     }
               //     else
               //     {
               //         partCodeTracker[prefix]++;
               //     }

               //      newPartCode = prefix + partCodeTracker[prefix];
               // }
              
               // // If Excel already has a PartCode, use it; else use generated
               
               // string finalPartCode = !string.IsNullOrEmpty(PcValue) ? PcValue : newPartCode;

            
                data.Add(new ItemViewModel()
                {
                  // PartCode= finalPartCode,
                    PartCode = GetCellValue(worksheet, row, 1),
                    PartCodeExists = PartCodeExists,
                    ItemNameExists = ItemNameExists,
                    ItemName = GetCellValue(worksheet, row, 2),
                    Unit = GetCellValue(worksheet, row, 3),
                    HSNNo = GetCellIntValue(worksheet, row, 4),
                    ItemGroup = GetCellValue(worksheet, row, 5),
                    ItemCategory = GetCellValue(worksheet, row, 6),
                    MinLevel = GetCellIntValue(worksheet, row, 7),
                    MaxLevel = GetCellIntValue(worksheet, row, 8),
                    Stockable = GetCellValue(worksheet, row, 9),
                    WIPStockable = GetCellValue(worksheet, row, 10),
                    NeedPO = GetCellValue(worksheet, row, 11),
                    QcReq = GetCellValue(worksheet, row, 12),
                    StdPkg = GetCellIntValue(worksheet, row, 13),
                    RackID = GetCellValue(worksheet, row, 17),
                    Barcode = GetCellValue(worksheet, row, 18),
                    ItemGroupCode = itemGCode,
                    ItemCategoryCode = itemCCode,
                    ItemServAssets = GetCellValue(worksheet, row, 14),
                    Store = itemStoreId.ToString(),
                    StoreName = Store,
                    WorkCenter = workCenter,
                    ProdInWorkcenter = itemWorkCenterId
                });
            }
        }
        var model = new ItemMasterModel();
        model.ExcelDataList = data;
        return PartialView("_DisplayExcelData", model);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
    {
        var response = new ResponseResult();
        var flag = request.Flag;

        try
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("PartCode", typeof(string));
            dt.Columns.Add("Item_Name", typeof(string));
            dt.Columns.Add("ParentCode", typeof(string));
            dt.Columns.Add("EntryDate", typeof(DateTime));
            dt.Columns.Add("LastUpdatedDate", typeof(DateTime));
            dt.Columns.Add("LeadTime", typeof(int));
            dt.Columns.Add("CC", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("SalePrice", typeof(decimal));
            dt.Columns.Add("PurchasePrice", typeof(decimal));
            dt.Columns.Add("CostPrice", typeof(decimal));
            dt.Columns.Add("WastagePercent", typeof(decimal));
            dt.Columns.Add("WtSingleItem", typeof(decimal));
            dt.Columns.Add("NoOfPcs", typeof(int));
            dt.Columns.Add("QcReq", typeof(string));
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("UploadItemImage", typeof(string));
            dt.Columns.Add("UploadImage", typeof(string));
            dt.Columns.Add("UID", typeof(string));
            dt.Columns.Add("DrawingNo", typeof(string));
            dt.Columns.Add("MinimumLevel", typeof(int));
            dt.Columns.Add("MaximumLevel", typeof(int));
            dt.Columns.Add("ReorderLevel", typeof(int));
            dt.Columns.Add("YearCode", typeof(string));
            dt.Columns.Add("AlternateUnit", typeof(string));
            dt.Columns.Add("RackID", typeof(string));
            dt.Columns.Add("BinNo", typeof(string));
            dt.Columns.Add("ItemSize", typeof(string));
            dt.Columns.Add("Colour", typeof(string));
            dt.Columns.Add("NeedPO", typeof(string));
            dt.Columns.Add("StdPacking", typeof(string));
            dt.Columns.Add("PackingType", typeof(string));
            dt.Columns.Add("ModelNo", typeof(string));
            dt.Columns.Add("YearlyConsumedQty", typeof(decimal));
            dt.Columns.Add("DispItemName", typeof(string));
            dt.Columns.Add("PurchaseAccountcode", typeof(string));
            dt.Columns.Add("SaleAccountcode", typeof(string));
            dt.Columns.Add("MinLevelDays", typeof(int));
            dt.Columns.Add("MaxLevelDays", typeof(int));
            dt.Columns.Add("EmpName", typeof(string));
            dt.Columns.Add("DailyRequirment", typeof(decimal));
            dt.Columns.Add("Stockable", typeof(string));
            dt.Columns.Add("WipStockable", typeof(string));
            dt.Columns.Add("Store", typeof(string));
            dt.Columns.Add("ProductLifeInus", typeof(string));
            dt.Columns.Add("ItemDesc", typeof(string));
            dt.Columns.Add("MaxWipStock", typeof(decimal));
            dt.Columns.Add("NeedSo", typeof(string));
            dt.Columns.Add("BomRequired", typeof(string));
            dt.Columns.Add("JobWorkItem", typeof(string));
            dt.Columns.Add("HsnNo", typeof(string));
            dt.Columns.Add("CreatedBy", typeof(string));
            dt.Columns.Add("CreatedOn", typeof(DateTime));
            dt.Columns.Add("UpdatedBy", typeof(string));
            dt.Columns.Add("UpdatedOn", typeof(DateTime));
            dt.Columns.Add("Active", typeof(string));
            dt.Columns.Add("ItemServAssets", typeof(string));
            dt.Columns.Add("VendorBatchcodeMand", typeof(string));
            dt.Columns.Add("EntryByMachineName", typeof(string));
            dt.Columns.Add("UniversalPartCode", typeof(string));
            dt.Columns.Add("UniversalDescription", typeof(string));
            dt.Columns.Add("ProdInWorkcenter", typeof(string));
            dt.Columns.Add("ProdInhouseJW", typeof(string));
            dt.Columns.Add("BatchNO", typeof(string));
            dt.Columns.Add("VoltageValue", typeof(string));
            dt.Columns.Add("SerialNo", typeof(string));
            dt.Columns.Add("OldPartCode", typeof(string));
            dt.Columns.Add("package", typeof(string));
            dt.Columns.Add("IsCustJWAdjMandatory", typeof(string));
            dt.Columns.Add("Branch", typeof(string));
            dt.Columns.Add("NoOfCavity", typeof(int));
            dt.Columns.Add("ProdInMachineGroup", typeof(string));
            dt.Columns.Add("ProdInMachine1", typeof(string));
            dt.Columns.Add("ProdInMachine2", typeof(string));
            dt.Columns.Add("ProdInMachine3", typeof(string));
            dt.Columns.Add("NoOfshotsHours", typeof(decimal));
            dt.Columns.Add("ProdInMachine4", typeof(string));
            dt.Columns.Add("ChildBom", typeof(string));
            dt.Columns.Add("usedinMachorVehicle", typeof(string));
            dt.Columns.Add("Barcode", typeof(string));

            foreach (var excelRow in request.ExcelData)
            {
                DataRow row = dt.NewRow();

                foreach (var map in request.Mapping)
                {
                    string dbCol = map.Key;          // DB column
                    string excelCol = map.Value;     // Excel column name

                    object value = DBNull.Value;     // default

                    if (excelRow.ContainsKey(excelCol) && !string.IsNullOrEmpty(excelRow[excelCol]))
                    {
                        value = excelRow[excelCol];

                        // Convert types for numeric/boolean/date columns if needed
                        Type columnType = dt.Columns[dbCol].DataType;

                        try
                        {
                            if (dbCol == "ParentCode")  // <-- Special handling for ParentCode
                            {
                                string groupName = value.ToString().Trim();

                                int ParentCode = 0;
                                var groupCode =  _IItemMaster.GetItemGroupCode(groupName);

                                if (groupCode.Result.Result != null && groupCode.Result.Result.Rows.Count > 0)
                                {
                                    ParentCode = (int)groupCode.Result.Result.Rows[0].ItemArray[0];
                                }
                               
                                else
                                {
                                    ParentCode = 0;
                                }

                                if (ParentCode!=0)
                                    value = ParentCode;   // replace with code
                                else
                                {
                                    return Json(new
                                    {
                                        StatusCode = 200,
                                        StatusText = "Please Enter valid group"
                                       
                                    });
                                }
                                   
                            }

                            if (dbCol == "ItemType")  // <-- Special handling for ParentCode
                            {
                                string ItemCat = value.ToString().Trim();

                                int ItemType = 0;
                                var CatCode = _IItemMaster.GetItemCatCode(ItemCat);

                                if (CatCode.Result.Result != null && CatCode.Result.Result.Rows.Count > 0)
                                {
                                    ItemType = (int)CatCode.Result.Result.Rows[0].ItemArray[0];
                                }

                                else
                                {
                                    ItemType = 0;
                                }

                                if (ItemType != 0)
                                    value = ItemType;   // replace with code
                                else
                                {
                                    return Json(new
                                    {
                                        StatusCode = 200,
                                        StatusText = "Please Enter valid category"

                                    });
                                }

                            }


                            if (dbCol == "StoreId")  // <-- Special handling for ParentCode
                            {
                                string StoreId = value.ToString().Trim();

                                int StoreName = 0;
                                var storeid = _IItemMaster.GetStoreCode(StoreId);

                                if (storeid.Result.Result != null && storeid.Result.Result.Rows.Count > 0)
                                {
                                    StoreName = (int)storeid.Result.Result.Rows[0].ItemArray[0];
                                }

                                else
                                {
                                    StoreName = 0;
                                }

                                if (StoreName != 0)
                                    value = StoreName;   // replace with code
                                else
                                {
                                    return Json(new
                                    {
                                        StatusCode = 200,
                                        StatusText = "Please Enter valid Store"

                                    });
                                }

                            }



                            if (columnType == typeof(int))
                                value = int.Parse(value.ToString());
                            else if (columnType == typeof(decimal))
                                value = decimal.Parse(value.ToString());
                            else if (columnType == typeof(bool))
                            {
                                // Accept 1/0, true/false, Y/N
                                string s = value.ToString().Trim().ToLower();
                                value = (s == "1" || s == "true" || s == "y");
                            }
                            else if (columnType == typeof(DateTime))
                                value = DateTime.Parse(value.ToString());
                            else
                                value = value.ToString();
                        }
                        catch
                        {
                            value = DBNull.Value; // fallback if conversion fails
                        }
                    }
                    row[dbCol] = value;
                }

                dt.Rows.Add(row);
            }

            response = await _IItemMaster.UpdateMultipleItemDataFromExcel(dt, flag);

            if (response != null)
            {
                if ((response.StatusText == "Success" || response.StatusText == "Updated") &&
                     (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted))
                {
                    return Json(new
                    {
                        StatusCode = 200,
                        StatusText = "Data imported successfully",
                        RedirectUrl = Url.Action("ImportandUpdateItems", "ItemMaster", new { Flag = "" })
                    });
                }
                else
                {
                    return Json(new
                    {

                        StatusText = response.StatusText,
                        statusCode = 201,
                        redirectUrl = ""
                    });
                }
            }

            return Json(new
            {
                StatusCode = 500,
                StatusText = "Unknown error occurred"
            });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }

        
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
                if ((Result.StatusText == "Success" || Result.StatusText == "Updated") &&
                     (Result.StatusCode == HttpStatusCode.OK || Result.StatusCode == HttpStatusCode.Accepted))
                {
                    return Json(new
                    {
                        StatusCode = 200,
                        StatusText = "Data imported successfully",
                        RedirectUrl = Url.Action("ImportandUpdateItems", "ItemMaster", new { Flag = "" })
                    });
                }
                else
                {
                    return Json(new
                    {
                        
                        StatusText = Result.StatusText,
                        statusCode = 201,
                        redirectUrl = ""
                    });
                }
            }

            return Json(new
            {
                StatusCode = 500,
                StatusText = "Unknown error occurred"
            });
        }
            //if (Result != null)
            //{
            //    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            //    {
            //        ViewBag.isSuccess = true;
            //        TempData["200"] = "200";
            //    }
            //    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
            //    {
            //        ViewBag.isSuccess = true;
            //        TempData["202"] = "202";
            //    }
            //    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
            //    {
            //        ViewBag.isSuccess = false;
            //        TempData["500"] = "500";
            //        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
            //        return View("Error", Result);
            //    }
            //    else
            //    {
            //        ViewBag.isSuccess = false;
            //        TempData["500"] = "500";
            //        return View("Error", Result.StatusText);

        //    }
        //}


        //return RedirectToAction(nameof(ImportItems));
        
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
                if ((Result.StatusText == "Success" || Result.StatusText == "Updated") &&
                     (Result.StatusCode == HttpStatusCode.OK || Result.StatusCode == HttpStatusCode.Accepted))
                {
                    return Json(new
                    {
                        StatusCode = 200,
                        StatusText = "Data imported successfully",
                        RedirectUrl = Url.Action("ImportandUpdateItems", "ItemMaster", new { Flag = "" })
                    });
                }
                else
                {
                    return Json(new
                    {

                        StatusText = Result.StatusText,
                        statusCode = 201,
                        redirectUrl = ""
                    });
                }
            }

            return Json(new
            {
                StatusCode = 500,
                StatusText = "Unknown error occurred"
            });
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
        MRGrid.Columns.Add("ProdInWorkcenter", typeof(string));
        MRGrid.Columns.Add("Barcode", typeof(string));

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
                                        Item.RackID,                          // 27. RackID (string)
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
                                         Item.Store ?? "",                       // 44. Store (string)
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
                                        Item.ItemServAssets      ,    // 57. ItemServAssets (string) - EXTRA VALUE
                                        Item.ProdInWorkcenter   ,       // 57. ItemServAssets (string) - EXTRA VALUE,
                                        Item.Barcode
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
        MRGrid.Columns.Add("NoOfCavity", typeof(string));
        MRGrid.Columns.Add("NoOfshotsHours", typeof(string));
        MRGrid.Columns.Add("ChildBom", typeof(string));
        MRGrid.Columns.Add("ProdInMachineGroup", typeof(string));
        MRGrid.Columns.Add("ProdInMachine1", typeof(string));
        MRGrid.Columns.Add("ProdInMachine2", typeof(string));
        MRGrid.Columns.Add("ProdInMachine3", typeof(string));
        MRGrid.Columns.Add("ProdInMachine4", typeof(string));
        MRGrid.Columns.Add("Branch", typeof(string));

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
                item.IsCustJWAdjMandatory ?? "N",
                item.NoOfCavity,
                item.NoOfshotsHours,
                item.ChildBom??"N",
                item.ProdInMachineGroupId,
                item.ProdInMachine1,
                item.ProdInMachine2,
                item.ProdInMachine3,
                item.ProdInMachine4,
                item.Branch
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