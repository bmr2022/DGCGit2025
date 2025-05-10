using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Net;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using DataTable = System.Data.DataTable;

namespace eTactWeb.Controllers;

[Authorize]
public class BomController : Controller
{
    private readonly IBomModule _IBom;
    private readonly IDataLogic _IDataLogic;
    public BomController(IDataLogic iDataLogic, IBomModule iBom)
    {
        _IDataLogic = iDataLogic;
        _IBom = iBom;
    }

    public async Task<IActionResult> BindBomData(string FIC, int BMNo)
    {
        BomModel model = await _IBom.EditBomDetail(FIC, BMNo - 1, "EditBomDetail");

        model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");

        model.Mode = null;

        //HttpContext.Session.Remove("BomList");

        if (model.BomList != null)
        {
            HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
        }

        //var _View = new PartialViewResult
        //{
        //    ViewName = "_BomGrid",
        //    ViewData = ViewData,
        //};
        //return Json(new { model, _View });

        //if (HttpContext.Session.GetString("BomList") == null)
        //{
        //var _List = new List<BomModel>();
        //_List = model.BomList.Select(item => new BomModel()
        //{
        //    SeqNo = model.SeqNo,
        //    FinishItemCode = model.FinishItemCode,
        //    FinishedItemName = model.FinishedItemName,
        //    BOMName = model.BOMName,
        //    BomNo = model.BomNo,
        //    BomQty = model.BomQty,
        //    EntryDate = model.EntryDate,
        //    EffectiveDate = model.EffectiveDate,
        //    ItemCode = model.ItemCode,
        //    ICName = model.ICName,
        //    ItemName = model.ItemName,
        //    Qty = model.Qty,
        //    Unit = model.Unit,
        //    AltItemCode1 = model.AltItemCode1,
        //    AICName1 = model.AICName1,
        //    AltItemName1 = model.AltItemName1,
        //    AltQty1 = model.AltQty1,
        //    UsedStageId = model.UsedStageId,
        //    AltItemCode2 = model.AltItemCode2,
        //    AICName2 = model.AICName2,
        //    AltItemName2 = model.AltItemName2,
        //    AltQty2 = model.AltQty2,
        //    IssueToJOBwork = model.IssueToJOBwork,
        //    DirectProcess = model.DirectProcess,
        //    RecFrmCustJobwork = model.RecFrmCustJobwork,
        //    PkgItem = model.PkgItem,
        //    Remark = model.Remark,
        //    RunnerItemCode = model.RunnerItemCode,
        //    RunnerQty = model.RunnerQty,
        //    GrossWt = model.GrossWt,
        //    NetWt = model.NetWt,
        //    Scrap = model.Scrap,
        //    BurnQty = model.BurnQty
        //}).ToList();
        //}

        return PartialView("_BomForm", model);
    }

    // GET: BomController
    public async Task<IActionResult> BomForm(int ID, string Mode)
    {
        HttpContext.Session.Remove("KeyBomList");
        BomModel model = new BomModel
        {
            FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList"),
            FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),

            CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList"),
            NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),

            UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList"),

            ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList"),

            PkgItem = "N",
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")),
            EntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/"),
            EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/")
        };
        if (Mode != "U")
        {
            model.UID = HttpContext.Session.GetString("UID");
            model.CC = HttpContext.Session.GetString("Branch");
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.CreatedByName = HttpContext.Session.GetString("EmpName");
            model.CreatedOn = DateTime.Now;
        }
        else if (Mode == "U")
        {
            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.UpdatedByName = HttpContext.Session.GetString("EmpName");
            model.UpdatedOn = DateTime.Now;
        }
        HttpContext.Session.Remove("BomList");
        HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
        //  return View(model);
        // return RedirectToAction("BomForm", "BOMStage");
        return View(model);
    }
    public JsonResult AutoComplete(string ColumnName, string prefix)
    {
        var iList = _IDataLogic.AutoComplete("Bom", ColumnName, "", "", 0, 0);
        var Result = (from item in iList
                      where item.Text.Contains(prefix)
                      select new
                      {
                          item.Text
                      }).Distinct().ToList();

        return Json(Result);
    }


    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> BomForm(BomModel model)
    {
        var BomStatus = 0;
        var BomList = new List<BomModel>();

        model.Mode = model.Mode == "U" ? "Update" : "Insert";

        if (model.Mode != "Update")
        {
            BomStatus = _IBom.GetBomStatus(model.FinishItemCode, model.BomNo);
        }

        if (model.Mode == "U" && model.BomList == null && HttpContext.Session.GetString("BomList") == "[]")
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (HttpContext.Session.GetString("BomList") == null && (model.Mode == "U" || model.BomList == null))
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (model.Mode == null && HttpContext.Session.GetString("BomList") == null)
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (model.BomList == null && (HttpContext.Session.GetString("BomList") == null || HttpContext.Session.GetString("BomList") == "[]"))
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else
        {
            if (BomStatus == 0)
            {
                BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));

                DataTable _Table = new DataTable();

                _Table.Columns.Add("SeqNo", typeof(int));
                _Table.Columns.Add("ItemCode", typeof(string));
                _Table.Columns.Add("Qty", typeof(decimal));
                _Table.Columns.Add("Unit", typeof(string));
                _Table.Columns.Add("Location", typeof(string));
                _Table.Columns.Add("MPNNo", typeof(string));
                _Table.Columns.Add("UsedStageId", typeof(string));
                _Table.Columns.Add("AltItemCode1", typeof(int));
                _Table.Columns.Add("AltQty1", typeof(decimal));
                _Table.Columns.Add("AltItemCode2", typeof(int));
                _Table.Columns.Add("AltQty2", typeof(decimal));
                _Table.Columns.Add("IssueToJoBwork", typeof(string));
                _Table.Columns.Add("DirectProcess", typeof(string));
                _Table.Columns.Add("RecFrmCustJobWork", typeof(string));
                _Table.Columns.Add("PkgItem", typeof(string));
                _Table.Columns.Add("Remark", typeof(string));
                _Table.Columns.Add("GrossWt", typeof(decimal));
                _Table.Columns.Add("NetWt", typeof(decimal));
                _Table.Columns.Add("Scrap", typeof(decimal));
                _Table.Columns.Add("RunnerItemCode", typeof(int));
                _Table.Columns.Add("RunnerQty", typeof(decimal));
                _Table.Columns.Add("BurnQty", typeof(decimal));
                _Table.Columns.Add("CustJWmandatory", typeof(string));

                //_Table = Repository.Common.ToDataTable(BomList);

                foreach (BomModel _Item in BomList)
                {
                    _Table.Rows.Add(new object[]
                    {
                    _Item.SeqNo,
                    _Item.ItemCode,
                    _Item.Qty,
                    _Item.Unit,
                    _Item.Location,
                    _Item.MPNNo,
                    _Item.UsedStageId,
                    _Item.AltItemCode1,
                    _Item.AltQty1,
                    _Item.AltItemCode2,
                    _Item.AltQty2,
                    _Item.IssueToJOBwork,
                    _Item.DirectProcess,
                    _Item.RecFrmCustJobwork,
                    _Item.PkgItem,
                    _Item.Remark,
                    _Item.GrossWt,
                    _Item.NetWt,
                    _Item.Scrap,
                    _Item.RunnerItemCode,
                    _Item.RunnerQty,
                    _Item.BurnQty,
                    _Item.CustJwAdjustmentMandatory ?? ""
                    });
                }

                if (_Table.Rows.Count > 0)
                {
                    //model.CreatedBy = Constants.UserID;
                    model.EntryDate = ParseFormattedDate(model.EntryDate);
                    model.EffectiveDate = ParseFormattedDate(model.EffectiveDate);

                    if (model.Mode == "Update")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    }
                    Common.ResponseResult Result = await _IBom.SaveBomData(_Table, model);

                    if (Result != null && Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                    }
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                }
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("Error", "Cannot Save Duplicate Bom...!");
            }
        }

        model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");

        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

        return View(model);
    }

    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await _IBom.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public IActionResult BomGrid(BomModel model)
    {
        List<BomModel> _List = new List<BomModel>();

        if (HttpContext.Session.GetString("BomList") == null)
        {
            _List.Add(new BomModel
            {
                SeqNo = 1,
                FinishItemCode = model.FinishItemCode,
                FinishedItemName = model.FinishedItemName,
                BOMName = model.BOMName,
                BomNo = model.BomNo,
                BomQty = model.BomQty,
                EntryDate = model.EntryDate,
                EffectiveDate = model.EffectiveDate,
                ItemCode = model.ItemCode,
                ICName = model.ICName,
                ItemName = model.ItemName,
                Qty = model.Qty,
                Unit = model.Unit,
                Location = model.Location,
                MPNNo = model.MPNNo,
                AltItemCode1 = model.AltItemCode1,
                AICName1 = model.AICName1,
                AltItemName1 = model.AltItemName1,
                AltQty1 = model.AltQty1,
                UsedStageId = model.UsedStageId,
                AltItemCode2 = model.AltItemCode2,
                AICName2 = model.AICName2,
                AltItemName2 = model.AltItemName2,
                AltQty2 = model.AltQty2,
                IssueToJOBwork = model.IssueToJOBwork,
                DirectProcess = model.DirectProcess,
                RecFrmCustJobwork = model.RecFrmCustJobwork,
                PkgItem = model.PkgItem,
                Remark = model.Remark,
                RunnerItemCode = model.RunnerItemCode,
                RunnerQty = model.RunnerQty,
                GrossWt = model.GrossWt,
                NetWt = model.NetWt,
                Scrap = model.Scrap,
                BurnQty = model.BurnQty,
                CustJwAdjustmentMandatory = model.CustJwAdjustmentMandatory
            });
            model.BomList = _List;
            HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
        }
        else
        {
            model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));

            bool TF = model.BomList.Any(x => x.ItemCode == model.ItemCode);

            if (TF == false)
            {
                model.BomList.Add(new BomModel
                {
                    SeqNo = model.BomList.Count + 1,
                    FinishItemCode = model.FinishItemCode,
                    FinishedItemName = model.FinishedItemName,
                    BOMName = model.BOMName,
                    BomNo = model.BomNo,
                    BomQty = model.BomQty,
                    EntryDate = model.EntryDate,
                    EffectiveDate = model.EffectiveDate,
                    ItemCode = model.ItemCode,
                    ICName = model.ICName,
                    ItemName = model.ItemName,
                    Qty = model.Qty,
                    Unit = model.Unit,
                    Location = model.Location,
                    MPNNo = model.MPNNo,
                    AltItemCode1 = model.AltItemCode1,
                    AICName1 = model.AICName1,
                    AltItemName1 = model.AltItemName1,
                    AltQty1 = model.AltQty1,
                    UsedStageId = model.UsedStageId,
                    AltItemCode2 = model.AltItemCode2,
                    AICName2 = model.AICName2,
                    AltItemName2 = model.AltItemName2,
                    AltQty2 = model.AltQty2,
                    IssueToJOBwork = model.IssueToJOBwork,
                    DirectProcess = model.DirectProcess,
                    RecFrmCustJobwork = model.RecFrmCustJobwork,
                    PkgItem = model.PkgItem,
                    Remark = model.Remark,
                    RunnerItemCode = model.RunnerItemCode,
                    RunnerQty = model.RunnerQty,
                    GrossWt = model.GrossWt,
                    NetWt = model.NetWt,
                    Scrap = model.Scrap,
                    BurnQty = model.BurnQty,
                    CustJwAdjustmentMandatory = model.CustJwAdjustmentMandatory
                });
                HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
            }
            else
            {
                return StatusCode(207, "Duplicate");
            }
        }

        return PartialView("_BomGrid", model);
    }

    public async Task<IActionResult> Dashboard(string FGPartCode = "", string FGItemName = "", string RMPartCode = "", string RMItemName = "", string BomRevNo = "", string DashboardType = "", string GlobalSearch = "", int pageNumber = 1, int pageSize = 50)
    {
        //BomDashboard model = new BomDashboard
        //{
        //    FGPartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList"),
        //    FGItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList"),

        // RMPartCodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST",
        // "SP_GetDropDownList"), RMItemNameList = await
        // _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),        //    DTDashboard = await _IBom.GetBomDashboard("Dashboard"),
        //};

        var model = new BomDashboard();
        var oDataSet = await _IBom.GetBomDashboard("Dashboard").ConfigureAwait(false);

        if (oDataSet.Tables.Count != 0)
        {
            model.DTDashboard = oDataSet.Tables[0];

            model.FGPartCodeList = (from DataRow dr in oDataSet.Tables[0].Rows.Cast<DataRow>()
                                    select new TextValue()
                                    {
                                        Text = dr["FGPartCode"].ToString(),
                                        Value = dr["FGPartCode"].ToString(),
                                    }).DistinctBy(x => x.Value).ToList();

            model.FGItemNameList = (from DataRow dr in oDataSet.Tables[0].Rows
                                    select new TextValue()
                                    {
                                        Text = dr["FGItem"].ToString(),
                                        Value = dr["FGItem"].ToString(),
                                    }).DistinctBy(x => x.Value).ToList();

            model.RMPartCodeList = (from DataRow dr in oDataSet.Tables[0].Rows.Cast<DataRow>()
                                    select new TextValue()
                                    {
                                        Text = dr["RMPartCode"].ToString(),
                                        Value = dr["RMPartCode"].ToString(),
                                    }).DistinctBy(x => x.Value).ToList();

            model.RMItemNameList = (from DataRow dr in oDataSet.Tables[0].Rows
                                    select new TextValue()
                                    {
                                        Text = dr["RMItemName"].ToString(),
                                        Value = dr["RMItemName"].ToString(),
                                    }).DistinctBy(x => x.Value).ToList();
        }
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };

        //model.FGPartCode = FGPartCode;
        //model.FGItemName = FGItemName;
        //model.RMPartCode = RMPartCode;
        //model.RMItemName = RMItemName;
        //model.BomRevNo = BomRevNo;
        //model.DashboardType = DashboardType;

        model.DTDashboard = model.DTDashboard == null ? new System.Data.DataTable() : model.DTDashboard;
        var DTDashboardPage = model.DTDashboard;
        HttpContext.Session.SetString("KeyBomList", JsonConvert.SerializeObject(DTDashboardPage));

        model.TotalRecords = model.DTDashboard.Rows.Count;
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;

        var pagedRows = model.DTDashboard.AsEnumerable()
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize);

        model.DTDashboard = pagedRows.Any() ? pagedRows.CopyToDataTable() : model.DTDashboard.Clone();

        return View(model);
    }
   
    //public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 10)
    //{
    //    BomDashboard model = new BomDashboard();
    //    DataTable bomViewModel = new DataTable();
    //    string bomData = HttpContext.Session.GetString("KeyBomList");
    //    if (string.IsNullOrWhiteSpace(searchString))
    //    {
    //        return PartialView("_BomDashboardGrid", model); // return empty model (or paginated full list)
    //    }

    //    if (bomViewModel == null)
    //    {
    //        return PartialView("_BomDashboardGrid", model); // no data in memory
    //    }

    //    // Clone the structure of the original DataTable
    //    DataTable filteredTable = bomViewModel.Clone();

    //    // Filter rows based on the search string (case-insensitive)
    //    foreach (DataRow row in bomViewModel.Rows)
    //    {
    //        foreach (DataColumn column in bomViewModel.Columns)
    //        {
    //            if (column.DataType == typeof(string))
    //            {
    //                var cellValue = row[column]?.ToString();
    //                if (!string.IsNullOrEmpty(cellValue) &&
    //                    cellValue.Contains(searchString, StringComparison.OrdinalIgnoreCase))
    //                {
    //                    filteredTable.ImportRow(row);
    //                    break; // match found in this row, no need to check other columns
    //                }
    //            }
    //        }
    //    }

    //    // Total records after search
    //    model.TotalRecords = filteredTable.Rows.Count;
    //    model.PageNumber = pageNumber;
    //    model.PageSize = pageSize;

    //    // Paging the filtered rows
    //    var pagedRows = filteredTable.AsEnumerable()
    //                                 .Skip((pageNumber - 1) * pageSize)
    //                                 .Take(pageSize);

    //    model.DTDashboard = pagedRows.Any() ? pagedRows.CopyToDataTable() : filteredTable.Clone();

    //    return PartialView("_BomDashboardGrid", model);
    //}


	public static DataTable ToDataTable<T>(List<T> items)
	{
		DataTable table = new DataTable(typeof(T).Name);

		var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

		foreach (var prop in props)
		{
			table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
		}

		foreach (var item in items)
		{
			var values = new object[props.Length];
			for (int i = 0; i < props.Length; i++)
			{
				values[i] = props[i].GetValue(item, null);
			}
			table.Rows.Add(values);
		}

		return table;
	}


	[HttpGet]

	public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 500)
  {
        BomDashboard model = new BomDashboard();
        //DataTable bomViewModel = new DataTable();
        //string bomData = HttpContext.Session.GetString("KeyBomList");
        //WIPStockRegisterModel model = new WIPStockRegisterModel();
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return PartialView("_BomDashboardGrid", model); // return empty model (or paginated full list)
        }

        string modelJson = HttpContext.Session.GetString("KeyBomList");
        List<BomModel> BomDashboard = new List<BomModel>();
        if (!string.IsNullOrEmpty(modelJson))
        {
            BomDashboard = JsonConvert.DeserializeObject<List<BomModel>>(modelJson);
        }
        if (BomDashboard == null)
        {
            return PartialView("_BomDashboardGrid", new List<BomModel>());
        }

        List<BomModel> filteredResults;

        if (string.IsNullOrWhiteSpace(searchString))
        {
            filteredResults = BomDashboard.ToList();
        }
        else
        {
            filteredResults = BomDashboard
                .Where(i => i.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.GetValue(i)?.ToString())
                    .Any(value => !string.IsNullOrEmpty(value) &&
                                  value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            if (filteredResults.Count == 0)
            {
                filteredResults = BomDashboard.ToList();
            }
        }

        model.TotalRecords = filteredResults.Count;
		//model.DTDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		var pagedList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		model.BomList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;

        return PartialView("_BomDashboardGrid", model.BomList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(BomForm));
        }
        catch (Exception error)
        {
            //return View();
            throw;
        }
    }

    public PartialViewResult DeleteBomRow(string SeqNo)
    {
        BomModel model = new BomModel();
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (HttpContext.Session.GetString("BomList") != null)
        {
            model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));
            model.BomList.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (BomModel item in model.BomList)
            {
                Indx++;
                item.SeqNo = Indx;
            }

            HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
        }
        return PartialView("_BomGrid", model);
    }
    public async Task<IActionResult> DeleteByID(string FIC, int BMNo, string FGPartCode, string FGItemName, string RMPartCode, string RMItemName, string BomRevNo, string DashboardType, string GlobalSearch)
    {
        Common.ResponseResult Result = await _IBom.DeleteByID(FIC, BMNo, "DeleteByID");

        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else if (Result.StatusText == "UnSuccess")
        {
            ViewBag.isSuccess = false;
            var input = "";
            input = Result.Result;
            TempData["ErrorMessage"] = input;
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction("Dashboard", new { FGPartCode = FGPartCode, FGItemName = FGItemName, RMPartCode = RMPartCode, RMItemName = RMItemName, BomRevNo = BomRevNo, DashboardType = DashboardType, GlobalSearch = GlobalSearch });
    }

    public async Task<IActionResult> EditBomDetail(string FIC, int BMNo, string Mode, string FGPartCode = "", string FGItemName = "", string RMPartCode = "", string RMItemName = "", string BomRevNo = "", string SummaryDetail = "", string GlobalSearch = "")
    {
        BomModel model = await _IBom.EditBomDetail(FIC, BMNo, "EditBomDetail");

        model.Mode = Mode;

        model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");

        HttpContext.Session.Remove("BomList");

        HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));

        model.FGPartCodeBack = FGPartCode;
        model.FGItemNameBack = FGItemName;
        model.RMItemNameBack = RMItemName;
        model.RMPartCodeBack = RMPartCode;
        model.BomRevNoBack = BomRevNo;
        model.SummaryDetailBack = SummaryDetail;
        model.GlobalSearchBack = GlobalSearch;

        return View("BomForm", model);
    }

    public async Task<IActionResult> EditBomSeq(BomModel model)
    {
        object Result = string.Empty;

        int Indx = Convert.ToInt32(model.SeqNo) - 1;

        if (HttpContext.Session.GetString("BomList") != null)
        {
            model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));
            Result = model.BomList.Where(m => m.SeqNo == model.SeqNo).ToList();

            model.BomList.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (BomModel item in model.BomList)
            {
                Indx++;
                item.SeqNo = Indx;
            }

            if (model.BomList.Count > 0)
            {
                HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
            }
            else
            {
                HttpContext.Session.Remove("BomList");
            }
        }
        return Json(JsonConvert.SerializeObject(Result));
    }

    public async Task<PartialViewResult> FetchAllItem(string TF, string CtrlID)
    {
        BomModel model = new BomModel();
        string _PartialView = CtrlID == "FG1" ? "_FGItem1" : CtrlID == "FG2" ? "_FGItem2" : CtrlID == "FG3" ? "_FGItem3" : "";

        if (TF == "true")
        {
            if (CtrlID == "FG1")
            {
                model.FG1CodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
                model.FG1NameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");
            }
            if (CtrlID == "FG2" || CtrlID == "FG3")
            {
                model.CodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
                model.NameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");
            }
        }
        else
        {
            if (CtrlID == "FG1")
            {
                model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
                model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
            }
            if (CtrlID == "FG2" || CtrlID == "FG3")
            {
                model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
                model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
            }
        }
        return PartialView(_PartialView, model);
    }

    public IActionResult GetBomDetail(string FGC, int BMNo)
    {
        BomDashboard model = new BomDashboard
        {
            DTDashboard = _IBom.GetBomDetail(FGC, BMNo, "GetBomDetail")
        };
        return PartialView("_BomDetail", model);
    }

    public JsonResult GetBomNo(int GBN)
    {
        GBN = _IBom.GetBomNo(GBN, "GetBomNo");
        return Json(new { GBN = GBN });
    }

    public async Task<IActionResult> GetSearchData(BomDashboard model)
    {
        model = await _IBom.GetSearchData(model);

        model.FGPartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
        model.FGItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");

        model.RMPartCodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.RMItemNameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
        //var Result = System.Text.Json.JsonSerializer.Serialize(model);
        return PartialView("_BomDashboardGrid", model);
    }

    public async Task<IActionResult> GetDetailSearchData(BomDashboard model)
    {
        model = await _IBom.GetDetailSearchData(model);

        //var Result = System.Text.Json.JsonSerializer.Serialize(model);
        if (model.DashboardType == "Summary")
        {
            return PartialView("_BomDashboardGrid", model);
        }
        if (model.DashboardType == "Detail")
        {
            return PartialView("_BomDashboardDetailGrid", model);
        }
        return null;
    }


    public JsonResult GetUnit(string IC)
    {
        IC = _IBom.GetUnit(IC, "GetUnit");
        return Json(new { IC = IC });
    }
    public IActionResult GetGridData(int IC, int BMNo)
    {
        var model = new BomModel();
        model = _IBom.GetGridData(IC, BMNo);
        HttpContext.Session.SetString("KeyBomList", JsonConvert.SerializeObject(model.BomList));
        return PartialView("_BomGrid", model);
    }
    public ActionResult ImportBom()
    {
        BomModel model = new BomModel();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        return View(model);
    }
    [HttpPost]
    public IActionResult UploadExcel(IFormFile excelFile)
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        List<BomViewModel> data = new List<BomViewModel>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            List<ImportBomData> importDataList = new();
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var FGPartCode = worksheet.Cells[row, 1].Value.ToString();
                var RMPartCode = worksheet.Cells[row, 3].Value.ToString();
                var AltPartCode1 = worksheet.Cells[row, 8].Value == null ? "" : worksheet.Cells[row, 8].Value.ToString();
                var AltPartCode2 = worksheet.Cells[row, 9].Value == null ? "" : worksheet.Cells[row, 9].Value.ToString();
                var FGItemCode = 0; var RmItemCode = 0; var AltItemCode1 = 0; var AltItemCode2 = 0;
                var FGItemName = ""; var RMItemName = "";
                var FGIC = _IBom.GetItemCode(FGPartCode, RMPartCode);

                var bomPartCodeData = new ImportBomData()
                {
                    FGPartCode = worksheet.Cells[row, 1].Value?.ToString(),
                    RMPartCode = worksheet.Cells[row, 3].Value.ToString()
                };

                importDataList.Add(bomPartCodeData);

                if (!string.IsNullOrEmpty(AltPartCode1))
                {
                    var AltPC = _IBom.GetAltItemCode(AltPartCode1);
                    if (AltPC != null)
                    {
                        var resultDataSet = AltPC.Result.Result;
                        if (resultDataSet != null)
                        {
                            var firstTable = resultDataSet.Tables[0];
                            foreach (DataRow row1 in firstTable.Rows)
                            {
                                AltItemCode1 = Convert.ToInt32(row1["AltItemCode"]);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(AltPartCode2))
                {
                    var AltPC = _IBom.GetAltItemCode(AltPartCode2);
                    if (AltPC != null)
                    {
                        var resultDataSet = AltPC.Result.Result;
                        if (resultDataSet != null)
                        {
                            var firstTable = resultDataSet.Tables[0];
                            foreach (DataRow row1 in firstTable.Rows)
                            {
                                AltItemCode2 = Convert.ToInt32(row1["AltItemCode"]);
                            }
                        }
                    }
                }

                if (FGIC != null)
                {
                    var resultDataSet = FGIC.Result.Result;
                    if (resultDataSet != null)
                    {
                        var firstTable = resultDataSet.Tables[0];
                        foreach (DataRow row1 in firstTable.Rows)
                        {
                            FGItemCode = Convert.ToInt32(row1["FGItemCode"]);
                            RmItemCode = Convert.ToInt32(row1["RMItemCode"]);
                            FGItemName = row1["FGItemName"].ToString();
                            RMItemName = row1["RMItemName"].ToString();
                        }
                    }
                }

                var BomData = _IBom.CheckDupeConstraint();
                var BomRevNo = _IBom.GetBomNo(FGItemCode, "GetBomNo");
                var BomRevNoChck = _IBom.GetBomNo(FGItemCode, "GetCheckBomNo");

                var duplicateBom = "";

                if (BomData != null)
                {
                    var oDT = BomData.Result.DefaultView.ToTable(true, "FinishItemCode", "BomNo", "ItemCode");
                    oDT.TableName = "BOMDataForConstraint";

                    //DashBoardData.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);

                    var bomData = CommonFunc.DataTableToList<BomModel>(oDT);

                    var checkConstraint = bomData.Where(x => x.FinishItemCode == FGItemCode && x.ItemCode == RmItemCode && x.BomNo == BomRevNo).ToList();

                    if (checkConstraint.Count > 0)
                    {
                        duplicateBom = "true";
                    }
                }
                data.Add(new BomViewModel()
                {
                    FGPartCode = worksheet.Cells[row, 1].Value == null ? "" : worksheet.Cells[row, 1].Value.ToString(),
                    FGItemName = FGItemName,
                    FGItemCode = FGItemCode,
                    RMItemCode = RmItemCode,
                    RMPartCode = worksheet.Cells[row, 3].Value == null ? "" : worksheet.Cells[row, 3].Value.ToString(),
                    RMItemName = RMItemName,
                    BomName = worksheet.Cells[row, 5].Value == null ? "" : worksheet.Cells[row, 5].Value.ToString(),
                    RMQty = worksheet.Cells[row, 6].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 6].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString())),
                    RMUnit = worksheet.Cells[row, 7].Value == null ? "" : worksheet.Cells[row, 7].Value.ToString(),
                    Location = worksheet.Cells[row, 8].Value == null ? "" : worksheet.Cells[row, 8].Value.ToString(),
                    MPNNumber = worksheet.Cells[row, 9].Value == null ? "" : worksheet.Cells[row, 9].Value.ToString(),
                    AltPartCode1 = worksheet.Cells[row, 10].Value == null ? "" : worksheet.Cells[row, 10].Value.ToString(),
                    AltItemCode1 = AltItemCode1,
                    AltItemCode2 = AltItemCode2,
                    AltPartCode2 = worksheet.Cells[row, 11].Value == null ? "" : worksheet.Cells[row, 11].Value.ToString(),
                    AltQty1 = worksheet.Cells[row, 12].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 12].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 12].Value.ToString())),
                    AltQty2 = worksheet.Cells[row, 13].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 13].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 13].Value.ToString())),
                    Scrap = worksheet.Cells[row, 14].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 14].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString())),
                    GrossWeight = worksheet.Cells[row, 15].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 15].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 15].Value.ToString())),
                    NetWeight = worksheet.Cells[row, 16].Value == null ? 0 : (string.IsNullOrEmpty(worksheet.Cells[row, 16].Value.ToString()) ? 0 : Convert.ToDecimal(worksheet.Cells[row, 16].Value.ToString())),
                    Remark = worksheet.Cells[row, 17].Value == null ? "" : worksheet.Cells[row, 17].Value.ToString(),
                    BomNo = BomRevNo,
                    ConstraintExists = duplicateBom,
                });
            }

            // duplicate items
            var duplicateParts = importDataList
        .GroupBy(x => new { x.FGPartCode, x.RMPartCode })
        .Where(g => g.Count() > 1)
        .Select(g => new
        {
            FGPartCode = g.Key.FGPartCode,
            RMPartCode = g.Key.RMPartCode,
            Count = g.Count()
        })
        .ToList();

            if (duplicateParts.Any())
            {
                foreach (var item in duplicateParts)
                {
                    var errorMsg = "Duplicate: FGPartCode = {item.FGPartCode}, RMPartCode = {item.RMPartCode}, Count = {item.Count}";
                    return StatusCode(207, errorMsg);
                }
            }

            // Get Bom Detail
            var bomDataTable = GetBomDetailTable(importDataList);
            var isValidPartCodes = _IBom.VerifyPartCode(bomDataTable);
            var extractedData = JsonConvert.DeserializeObject<List<dynamic>>(isValidPartCodes.Result);

            var simplifiedResponse = extractedData.Select(x => new
            {
                FGPartCode = x.FGPartCode,
                RMPartCode = x.RMPartCode
            }).ToList();

            var response = new
            {
                Message = "Some part codes are invalid. Please check the details.",
                InvalidPartCodes = simplifiedResponse
            };

            string jsonResponse = JsonConvert.SerializeObject(response);

            if (simplifiedResponse.Count > 0)
            {
                return StatusCode(207, jsonResponse);
            }
        }

        var model = new BomModel();
        model.ExcelDataList = data;
        return PartialView("_DisplayExcelData", model);
    }
    public async Task<IActionResult> AddBomListData(List<BomViewModel> model)
    {
        try
        {
            HttpContext.Session.Remove("KeyBomList");
            string jsonString = HttpContext.Session.GetString("KeyBomList");
            IList<BomViewModel> ItemViewModel = new List<BomViewModel> ();
            
            var MainModel = new BomModel();
            var ItemDetailGrid = new List<BomViewModel>();
            var ItemGrid = new List<BomViewModel>();
            var SSGrid = new List<BomViewModel>();

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
                        item.SeqNo = ItemViewModel.Count + 1;
                        ItemGrid = ItemViewModel.Where(x => x != null).ToList();
                        SSGrid.AddRange(ItemGrid);
                        ItemGrid.Add(item);
                    }
                    MainModel.ExcelDataList = ItemGrid;

                    HttpContext.Session.SetString("KeyBomList", JsonConvert.SerializeObject(MainModel.ExcelDataList));
                }
            }
            string modelJson = HttpContext.Session.GetString("KeyBomList");
            List<BomViewModel> ItemListt = new List<BomViewModel>();
            if(!string.IsNullOrEmpty(modelJson))
            {
                ItemListt = JsonConvert.DeserializeObject<List<BomViewModel>>(modelJson);
            }
            var CC = HttpContext.Session.GetString("Branch");
            var EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var ItemGridList = new DataTable();
            ItemGridList = GetDetailTable(ItemListt, CC, EmpID, yearCode);

            var Result = await _IBom.SaveMultipleBomData(ItemGridList);

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
                    return View("Error", Result);
                }
            }


            return RedirectToAction(nameof(ImportBom));
        }
        catch (Exception ex)
        {
            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };

            return View("Error", ResponseResult);
        }
    }
    private static System.Data.DataTable GetBomDetailTable(List<ImportBomData> DetailList)
    {
        var BOMGrid = new System.Data.DataTable();

        BOMGrid.Columns.Add("SeqNo", typeof(int));
        BOMGrid.Columns.Add("FGPartCode", typeof(string));
        BOMGrid.Columns.Add("RMPartCode", typeof(string));
        BOMGrid.Columns.Add("BOMQty", typeof(float));
        BOMGrid.Columns.Add("ScrapPartCode", typeof(string));
        BOMGrid.Columns.Add("ByProdPartCode", typeof(string));

        foreach (var Item in DetailList)
        {
            DateTime today = DateTime.Today;
            BOMGrid.Rows.Add(
                new object[]
                {
                    Item.SeqNo,
                    Item.FGPartCode ?? string.Empty,
                    Item.RMPartCode ?? string.Empty,
                    Item.BomQty,
                    Item.ScrapPartCode ?? string.Empty,
                    Item.ByProdPartCode ?? string.Empty
                });
        }
        BOMGrid.Dispose();
        return BOMGrid;
    }
    private static DataTable GetDetailTable(IList<BomViewModel> DetailList, string CC, int Empid, int YearCode)
    {
        var MRGrid = new DataTable();

        MRGrid.Columns.Add("FinishItemCode", typeof(int));
        MRGrid.Columns.Add("BOMName", typeof(string));
        MRGrid.Columns.Add("BomNo", typeof(int));
        MRGrid.Columns.Add("BomQty", typeof(float));
        MRGrid.Columns.Add("EntryDate", typeof(string));
        MRGrid.Columns.Add("EffectiveDate", typeof(string));
        MRGrid.Columns.Add("SeqNo", typeof(int));
        MRGrid.Columns.Add("ItemCode", typeof(int));
        MRGrid.Columns.Add("Qty", typeof(float));
        MRGrid.Columns.Add("Unit", typeof(string));
        MRGrid.Columns.Add("UsedStageId", typeof(string));
        MRGrid.Columns.Add("AltItemCode1", typeof(int));
        MRGrid.Columns.Add("AltQty1", typeof(float));
        MRGrid.Columns.Add("AltItemCode2", typeof(int));
        MRGrid.Columns.Add("AltQty2", typeof(float));
        MRGrid.Columns.Add("Location", typeof(string));
        MRGrid.Columns.Add("IssueToJoBWork", typeof(string));
        MRGrid.Columns.Add("DirectProcess", typeof(string));
        MRGrid.Columns.Add("RecFrmCustJobWork", typeof(string));
        MRGrid.Columns.Add("PkgItem", typeof(string));
        MRGrid.Columns.Add("Remark", typeof(string));
        MRGrid.Columns.Add("GrossWt", typeof(float));
        MRGrid.Columns.Add("NetWt", typeof(float));
        MRGrid.Columns.Add("Scrap", typeof(float));
        MRGrid.Columns.Add("RunnerItemCode", typeof(int));
        MRGrid.Columns.Add("RunnerQty", typeof(float));
        MRGrid.Columns.Add("BurnQty", typeof(float));
        MRGrid.Columns.Add("UID", typeof(int));
        MRGrid.Columns.Add("CC", typeof(string));
        MRGrid.Columns.Add("YearCode", typeof(int));
        MRGrid.Columns.Add("CreatedBy", typeof(int));
        MRGrid.Columns.Add("CreatedOn", typeof(string)); // datetime
        MRGrid.Columns.Add("UpdatedBy", typeof(int));
        MRGrid.Columns.Add("UpdatedOn", typeof(string)); // datetime
        MRGrid.Columns.Add("Active", typeof(string));
        MRGrid.Columns.Add("EntryByMachineName", typeof(string));
        MRGrid.Columns.Add("MPNNo", typeof(string));
        MRGrid.Columns.Add("CustJWmandatory", typeof(string));

        foreach (var Item in DetailList)
        {
            DateTime today = DateTime.Today;
            MRGrid.Rows.Add(
                new object[]
                {
                    Item.FGItemCode,
                    Item.BomName,
                    Item.BomNo,//bomno
                    0,
                    "",
                    "",
                    Item.SeqNo,
                    Item.RMItemCode,
                    Item.RMQty,
                    Item.RMUnit,
                    "",Item.AltItemCode1,Item.AltQty1,Item.AltItemCode2,Item.AltQty2,
                    Item.Location,
                    "","","","",Item.Remark,Item.GrossWeight,Item.NetWeight,Item.Scrap,0,0,0,Empid,
                    CC,YearCode,Empid,
                    ParseFormattedDate(DateTime.UtcNow.ToString()),
                    0,
                    ParseFormattedDate(DateTime.UtcNow.ToString()),"Y",
                    Environment.MachineName,
                    Item.CustJWmandatory,

                });
        }
        MRGrid.Dispose();
        return MRGrid;
    }
}