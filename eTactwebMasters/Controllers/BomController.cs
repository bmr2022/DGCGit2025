using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using PdfSharp.Drawing.BarCodes;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Caching;
using ZXing;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using DataTable = System.Data.DataTable;

namespace eTactWeb.Controllers;

[Authorize]
public class BomController : Controller
{
    private readonly IBomModule _IBom;
    private readonly IDataLogic _IDataLogic;
    private readonly IMemoryCache _MemoryCache;
    public EncryptDecrypt EncryptDecrypt { get; }

    public BomController(IDataLogic iDataLogic, IBomModule iBom, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt)
    {
        _IDataLogic = iDataLogic;
        _IBom = iBom;
        _MemoryCache = iMemoryCache;
        EncryptDecrypt = encryptDecrypt;
    }
    private string GetBomSessionKey()
    {
        var sessionKey = HttpContext.Session.GetString("SessionKey");

        if (string.IsNullOrEmpty(sessionKey))
        {
            sessionKey = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("SessionKey", sessionKey);
        }

        return sessionKey;
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
            var sessionKey = GetBomSessionKey();
            HttpContext.Session.SetString($"BomList_{sessionKey}", JsonConvert.SerializeObject(model.BomList));
            //   HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
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
    public async Task<IActionResult> BomForm(string FIC, int BMNo, string Mode, string formKey)
    {
        ViewBag.formKey = formKey;

        int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
        var rights = await _IBom.GetFormRights(userID);

        if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });

        var table = rights.Result.Tables[0];

        string encFIC = Request.Query["FIC"].ToString();
        string encBMNo = Request.Query["BMNo"].ToString();
        string encBMNo1 = RouteData.Values["BMNo"]?.ToString();
        string encBMNo2 = RouteData.Values["bmno"]?.ToString();
        if (!string.IsNullOrEmpty(encFIC) || !string.IsNullOrEmpty(encBMNo))
        {
            string decryptedFIC = EncryptDecrypt.Decrypt(encFIC);
            int decryptedBMNo = EncryptDecrypt.DecodeID(encBMNo);
            string decryptedMode = EncryptDecrypt.Decrypt(Mode);

            FIC = decryptedFIC;
            Mode = decryptedMode;
            BMNo = decryptedBMNo;
        }

        // permission check
        if (Mode == "U" && !Convert.ToBoolean(table.Rows[0]["OptUpdate"]))
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });

        if (Mode == "V" && !Convert.ToBoolean(table.Rows[0]["OptView"]))
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });

        if (Mode != "U" && !Convert.ToBoolean(table.Rows[0]["OptSave"]))
            return RedirectToAction("Dashboard", "Bom", new { formKey = formKey });

        BomModel model;

        // 🔥 EDIT MODE
        if (!string.IsNullOrEmpty(FIC))
        {
            model = await _IBom.EditBomDetail(FIC, BMNo, "EditBomDetail");
            model.Mode = Mode;
            model.BMNo = BMNo;
        }
        else
        {
            model = new BomModel
            {
                PkgItem = "N",
                YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}")),
                EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
                EffectiveDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/")
            };
        }

        var flagResponse = await _IBom.ChangeBomRMQtySameAsGrossWeight();

        if (flagResponse != null && flagResponse.Result != null &&
            flagResponse.Result.Rows.Count > 0)
        {
            model.AutoFillRMQtyFlag =
                flagResponse.Result.Rows[0]["ChangeBomRMQtySameAsGrossWeight"]?.ToString();
        }
        else
        {
            model.AutoFillRMQtyFlag = "N";
        }
        model.FG1CodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
        model.FG1NameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");
        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");
        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

        var sessionKey = GetBomSessionKey();
        // audit
        if (Mode == "U")
        {
            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            model.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
            model.UpdatedOn = DateTime.Now;


            HttpContext.Session.SetString($"BomList_{sessionKey}", JsonConvert.SerializeObject(model.BomList));
            //var sessionKey = GetBomSessionKey();

            var bomListJson = HttpContext.Session.GetString($"BomList_{sessionKey}");
            //HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
            //  var bomListJson = HttpContext.Session.GetString("BomList");
            var bomList = JsonConvert.DeserializeObject<List<BomModel>>(bomListJson);

        }
        else
        {
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            model.CreatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
            model.CreatedOn = DateTime.Now;
            HttpContext.Session.Remove($"BomList_{sessionKey}");

        }
        var industryData = await _IBom.GetIndustryType();

        if (industryData != null &&
            industryData.Result != null &&
            industryData.Result.Tables.Count > 0 &&
            industryData.Result.Tables[0].Rows.Count > 0)
        {
            ViewBag.IndustryType =
                industryData.Result.Tables[0].Rows[0]["IndustryType"]?.ToString();
        }
        else
        {
            ViewBag.IndustryType = "";
        }
        //HttpContext.Session.Remove("BomList");
        return View(model);
    }

    // GET: BomController
    //public async Task<IActionResult> BomForm(int ID, string Mode)
    //{

    //    int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
    //    var rights = await _IBom.GetFormRights(userID);
    //    if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
    //    {
    //                        return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
    //    }
    //    var table = rights.Result.Tables[0];
    //    string encID = Request.Query["ID"].ToString();

    //    if (!string.IsNullOrEmpty(encID))
    //    {
    //        int decryptedID = EncryptDecrypt.DecodeID(encID);
    //        string decryptedMode = EncryptDecrypt.Decrypt(Mode);
    //        ID = decryptedID;
    //        Mode = decryptedMode;
    //    }

    //    bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
    //    bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
    //    bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
    //    bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


    //    if (Mode == "U")
    //    {
    //        if (!(optUpdate))
    //        {
    //                            return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
    //        }
    //    }
    //    else if (Mode == "V")
    //    {
    //        if (!(optView))
    //        {
    //                            return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
    //        }
    //    }
    //    else if (ID <= 0)
    //    {
    //        if (!optSave)
    //        {
    //            return RedirectToAction("Dashboard", "Bom");
    //        }
    //        //if (!(optAll || optSave))
    //        //{
    //        //                    return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
    //        //}

    //    }

    //    HttpContext.Session.Remove("KeyBomList");
    //    BomModel model = new BomModel

    //    {
    //        FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList"),
    //        FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),

    //        CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList"),
    //        NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),

    //        UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList"),

    //        ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList"),

    //        PkgItem = "N",
    //        YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}")),
    //        EntryDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/"),
    //        EffectiveDate = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/")
    //    };
    //    model.ID = ID;
    //    model.Mode = Mode;
    //    if (Mode != "U")
    //    {
    //        model.UID = HttpContext.Session.GetString($"UID_{formKey}");
    //        model.CC = HttpContext.Session.GetString($"Branch_{formKey}");
    //        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
    //        model.CreatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
    //        model.CreatedOn = DateTime.Now;
    //    }
    //    else if (Mode == "U")
    //    {
    //        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
    //        model.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
    //        model.UpdatedOn = DateTime.Now;
    //    }
    //    HttpContext.Session.Remove("BomList");
    //    HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
    //    //  return View(model);
    //    // return RedirectToAction("BomForm", "BOMStage");
    //    return View(model);
    //}
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

    public async Task<JsonResult> FillItems(string Flag, String SearchString)
    {
        var JSON = await _IBom.FillItems(Flag, SearchString);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BomForm(BomModel model)
    {
        var BomStatus = 0;

        var BomList = new List<BomModel>();
        string formKey = model.formKey;
        model.Mode = model.Mode == "U" ? "U" : "Insert";
        model.EntryByMachineName = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
        model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
        if (model.Mode != "U")
        {
            BomStatus = _IBom.GetBomStatus(model.FinishItemCode, model.BomNo);
        }
        var sessionKey = GetBomSessionKey();

        var bomListJson = HttpContext.Session.GetString($"BomList_{sessionKey}");

        if (model.Mode == "U" && model.BomList == null && bomListJson == "[]")
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (bomListJson == null && (model.Mode == "U" || model.BomList == null))
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (model.Mode == null && bomListJson == null)
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else if (model.BomList == null && (bomListJson == null || bomListJson == "[]"))
        {
            ModelState.Clear();
            ModelState.AddModelError("Error", "Bom Grid Should Atleast Have a Row.");
        }
        else
        {
            if (BomStatus == 0)
            {
                BomList = JsonConvert.DeserializeObject<List<BomModel>>(bomListJson);

                // var bomListJson = HttpContext.Session.GetString("BomList");
                var bomList = JsonConvert.DeserializeObject<List<BomModel>>(bomListJson);

                DataTable _Table = BuildBomDataTable(bomList);

                var sessionBomList = bomListJson;

                if (!string.IsNullOrEmpty(sessionBomList))
                {
                    model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(sessionBomList);
                }
                if (_Table.Rows.Count > 0)
                {
                    //model.CreatedBy = Constants.UserID;
                    model.EntryDate = ParseFormattedDate(model.EntryDate);
                    model.EffectiveDate = ParseFormattedDate(model.EffectiveDate);

                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                    }
                    Common.ResponseResult Result = await _IBom.SaveBomData(_Table, model);
                    //Console.WriteLine((int)Result.StatusCode);
                    //Console.WriteLine(Result.StatusCode);
                    //Console.WriteLine(Result.StatusText);
                    if (Result != null && Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        return RedirectToAction(nameof(BomForm), new { ID = 0, formKey = formKey });

                    }
                    else if (Result != null && Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        return RedirectToAction(nameof(Dashboard), new { formKey = formKey });

                    }
                    else if (Result != null && Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        // _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        //return View("Error", Result);
                    }
                    else if (Result != null && !string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        ViewBag.isSuccess = false;
                        TempData["ErrorMessage"] = Result.StatusText;

                        model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
                        model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
                        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");
                        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
                        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
                        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

                        return View(model);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error while deleting transaction.";
                    }
                }
                //return RedirectToAction(nameof(BomForm), new { ID = 0 });
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
        return RedirectToAction(nameof(BomForm), new { ID = 0, formKey = formKey });
        //   return View(model);
    }
    private DataTable BuildBomDataTable(List<BomModel> bomList)
    {
        DataTable table = new DataTable();

        table.Columns.Add("SeqNo", typeof(int));
        table.Columns.Add("ItemCode", typeof(string));
        table.Columns.Add("Qty", typeof(decimal));
        table.Columns.Add("Unit", typeof(string));
        table.Columns.Add("Location", typeof(string));
        table.Columns.Add("MPNNo", typeof(string));
        table.Columns.Add("UsedStageId", typeof(string));
        table.Columns.Add("AltItemCode1", typeof(int));
        table.Columns.Add("AltQty1", typeof(decimal));
        table.Columns.Add("AltItemCode2", typeof(int));
        table.Columns.Add("AltQty2", typeof(decimal));
        table.Columns.Add("AltItemCode3", typeof(int));
        table.Columns.Add("AltQty3", typeof(decimal));
        table.Columns.Add("AltItemCode4", typeof(int));
        table.Columns.Add("AltQty4", typeof(decimal));
        table.Columns.Add("AltItemCode5", typeof(int));
        table.Columns.Add("AltQty5", typeof(decimal));
        table.Columns.Add("IssueToJoBwork", typeof(string));
        table.Columns.Add("DirectProcess", typeof(string));
        table.Columns.Add("RecFrmCustJobWork", typeof(string));
        table.Columns.Add("PkgItem", typeof(string));
        table.Columns.Add("Remark", typeof(string));
        table.Columns.Add("GrossWt", typeof(decimal));
        table.Columns.Add("NetWt", typeof(decimal));
        table.Columns.Add("Scrap", typeof(decimal));
        table.Columns.Add("RunnerItemCode", typeof(int));
        table.Columns.Add("RunnerQty", typeof(decimal));
        table.Columns.Add("BurnQty", typeof(decimal));
        table.Columns.Add("CustJWmandatory", typeof(string));
        table.Columns.Add("ByprodItemcode1", typeof(int));
        table.Columns.Add("ByprodItemcode2", typeof(int));
        table.Columns.Add("ByprodItemcQty1", typeof(decimal));
        table.Columns.Add("ByprodItemcQty2", typeof(decimal));
        table.Columns.Add("Dia", typeof(string));
        table.Columns.Add("grade", typeof(string));

        table.Columns.Add("thickness", typeof(decimal));
        table.Columns.Add("width", typeof(decimal));
        table.Columns.Add("length", typeof(decimal));
        table.Columns.Add("VendJwAdjustmentMandatory", typeof(string));
        table.Columns.Add("CustJwAdjustmentMandatory", typeof(string));
        table.Columns.Add("TotalRmQtyForTotBomQty", typeof(decimal));
        table.Columns.Add("Rate", typeof(decimal));
        table.Columns.Add("Amount", typeof(decimal));

        if (bomList == null || !bomList.Any())
            return table;

        foreach (var item in bomList)
        {
            table.Rows.Add(
                item.SeqNo,
                item.ItemCode,
                item.Qty,
                item.Unit,
                item.Location,
                item.MPNNo,
                item.UsedStageId,
                item.AltItemCode1,
                item.AltQty1,
                item.AltItemCode2,
                item.AltQty2,
                item.AltItemCode3,
                item.AltQty3,
                item.AltItemCode4,
                item.AltQty4,
                item.AltItemCode5,
                item.AltQty5,
                item.IssueToJOBwork,
                item.DirectProcess,
                item.RecFrmCustJobwork,
                item.PkgItem,
                item.Remark,
                item.GrossWt,
                item.NetWt,
                item.Scrap,
                item.RunnerItemCode,
                item.RunnerQty,
                item.BurnQty,
                item.CustJWmandatory ?? "",
                item.ByprodItemCode1,
                item.ByprodItemCode2,
                item.ByProdQty1,
                item.ByProdQty2,
                item.Dia ?? "",
                item.grade ?? "",

                item.thickness ?? 0,
                item.width ?? 0,
                item.length ?? 0,
                item.VendJwAdjustmentMandatory ?? "",
                item.CustJwAdjustmentMandatory ?? "",
                 item.TotalRmQtyForTotBomQty ?? 0,
                 item.Rate ?? 0,
                 item.Amount ?? 0
            );
        }

        return table;
    }

    public async Task<JsonResult> GetRMPartCodeList(string SearchText, string CTRL)
    {
        var JSON = await _IBom.GetRMPartCodeList(SearchText, CTRL);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetRMItemNameList(string SearchText, string CTRL)
    {
        var JSON = await _IBom.GetRMItemNameList(SearchText, CTRL);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetFGItemNameList(string SearchText, string CTRL)
    {
        var JSON = await _IBom.GetFGItemNameList(SearchText, CTRL);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetFGPartCodeList(string SearchText, string CTRL)
    {
        var JSON = await _IBom.GetFGPartCodeList(SearchText, CTRL);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetFormRights(string formKey)
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
        var JSON = await _IBom.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetIndustryType()
    {

        var JSON = await _IBom.GetIndustryType();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetBomMultiLevelGrid()
    {
        var JSON = await _IBom.GetBomMultiLevelGrid();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public IActionResult BomGrid(BomModel model)
    {
        var MainModel = new BomModel();
        List<BomModel> bomList = new();
        var _List = new List<BomModel>();
        var SSGrid = new List<BomModel>();
        var sessionKey = GetBomSessionKey();

        var modelJson = HttpContext.Session.GetString($"BomList_{sessionKey}");

        //string modelJson = HttpContext.Session.GetString("BomList");
        if (!string.IsNullOrEmpty(modelJson))
        {
            bomList = JsonConvert.DeserializeObject<List<BomModel>>(modelJson);
        }
        int newSeqNo = 0;

        if (bomList == null || bomList.Count == 0)
        {
            // No items → start with 1
            newSeqNo = 1;
        }
        else
        {
            // If user did not pass SeqNo → auto-increment
            if (model.SeqNo == 0 || model.SeqNo == null)
            {
                newSeqNo = bomList.Max(x => x.SeqNo) + 1;
            }
            else
            {
                // Use existing SeqNo
                newSeqNo = model.SeqNo;
            }
        }
        if (model != null)
        {
            if (bomList == null)
            {
                model.SeqNo = newSeqNo;
                _List.Add(model);
            }
            else
            {
                if (bomList.Any(x => x.ItemCode == model.ItemCode))
                {
                    return StatusCode(207, "Duplicate");
                }
                else
                {
                    if (model.SeqNo == 0)
                    {
                        model.SeqNo = newSeqNo;
                    }

                    _List = bomList.Where(x => x != null).ToList();
                    SSGrid.AddRange(_List);
                    _List.Add(model);
                }
            }
            MainModel.BomList = _List.OrderBy(x => x.SeqNo).ToList();

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var sessionKey1 = GetBomSessionKey();

            HttpContext.Session.SetString($"BomList_{sessionKey1}", JsonConvert.SerializeObject(MainModel.BomList));

            //_MemoryCache.Set("ItemList", MainModel.ItemDetailGrid, cacheEntryOptions);
            // HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(MainModel.BomList));
        }
        else
        {
            ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
        }
        //   if (model == null)
        //   {
        //       ModelState.TryAddModelError("Error", "BOM item cannot be null.");
        //       return PartialView("_BomGrid", MainModel);
        //   }

        //   // Duplicate check only for ADD
        //   if (model.SeqNo == 0 && bomList.Any(x => x.ItemCode == model.ItemCode))
        //   {
        //       return StatusCode(207, "Duplicate");
        //   }

        //   // EDIT
        //   int existingIndex = bomList.FindIndex(x => x.SeqNo == model.SeqNo);
        //   if (existingIndex >= 0)
        //   {
        //       bomList[existingIndex] = model; // keep SeqNo
        //   }
        //   else
        //   {
        //       // ADD
        //       model.SeqNo = bomList.Count == 0 ? 1  : model.SeqNo;

        //       bomList.Add(model);
        //   }

        //   HttpContext.Session.SetString(   "BomList", JsonConvert.SerializeObject(bomList));

        ////   MainModel.BomList = bomList;
        //   MainModel.BomList = bomList.OrderBy(x => x.SeqNo).ToList();

        return PartialView("_BomGrid", MainModel);
    }

    //public IActionResult BomGrid(BomModel model)
    //{
    //    List<BomModel> _List = new List<BomModel>();

    //    if (HttpContext.Session.GetString("BomList") == null)
    //    {
    //        _List.Add(new BomModel
    //        {
    //            SeqNo = 1,
    //            FinishItemCode = model.FinishItemCode,
    //            FinishedItemName = model.FinishedItemName,
    //            ByprodItemCode1 = model.ByprodItemCode1,
    //            ByprodItemName1 = model.ByprodItemName1,
    //            ByprodItemCode2 = model.ByprodItemCode2,
    //            ByprodItemName2 = model.ByprodItemName2,
    //            ByProdQty1 = model.ByProdQty1,
    //            ByProdQty2 = model.ByProdQty2,
    //            Dia = model.Dia,
    //            grade = model.grade,
    //            thickness = model.thickness,
    //            width = model.width,
    //            length = model.length,

    //            BOMName = model.BOMName,
    //            BomNo = model.BomNo,
    //            BomQty = model.BomQty,
    //            EntryDate = model.EntryDate,
    //            EffectiveDate = model.EffectiveDate,
    //            ItemCode = model.ItemCode,
    //            ICName = model.ICName,
    //            ItemName = model.ItemName,
    //            Qty = model.Qty,
    //            Unit = model.Unit,
    //            Location = model.Location,
    //            MPNNo = model.MPNNo,
    //            AltItemCode1 = model.AltItemCode1,
    //            AICName1 = model.AICName1,
    //            AltItemName1 = model.AltItemName1,
    //            AltQty1 = model.AltQty1,
    //            UsedStageId = model.UsedStageId,
    //            AltItemCode2 = model.AltItemCode2,
    //            AICName2 = model.AICName2,
    //            AltItemName2 = model.AltItemName2,
    //            AltQty2 = model.AltQty2,
    //            IssueToJOBwork = model.IssueToJOBwork,
    //            DirectProcess = model.DirectProcess,
    //            RecFrmCustJobwork = model.RecFrmCustJobwork,
    //            PkgItem = model.PkgItem,
    //            Remark = model.Remark,
    //            RunnerItemCode = model.RunnerItemCode,
    //            RunnerQty = model.RunnerQty,
    //            GrossWt = model.GrossWt,
    //            NetWt = model.NetWt,
    //            Scrap = model.Scrap,
    //            BurnQty = model.BurnQty,
    //            CustJwAdjustmentMandatory = model.CustJwAdjustmentMandatory

    //        });
    //        model.BomList = _List;
    //        HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
    //    }
    //    else
    //    {
    //        model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));

    //        bool TF = model.BomList.Any(x => x.ItemCode == model.ItemCode);

    //        if (TF == false)
    //        {
    //            model.BomList.Add(new BomModel
    //            {
    //                SeqNo = model.SeqNo,
    //                FinishItemCode = model.FinishItemCode,
    //                FinishedItemName = model.FinishedItemName,
    //                ByprodItemCode1 = model.ByprodItemCode1,
    //                ByprodItemName1 = model.ByprodItemName1,
    //                ByprodItemCode2 = model.ByprodItemCode2,
    //                ByprodItemName2 = model.ByprodItemName2,
    //                ByProdQty1 = model.ByProdQty1,
    //                ByProdQty2 = model.ByProdQty2,
    //                Dia = model.Dia,
    //                grade = model.grade,
    //                thickness = model.thickness,
    //                width = model.width,
    //                length = model.length,

    //                BOMName = model.BOMName,
    //                BomNo = model.BomNo,
    //                BomQty = model.BomQty,
    //                EntryDate = model.EntryDate,
    //                EffectiveDate = model.EffectiveDate,
    //                ItemCode = model.ItemCode,
    //                ICName = model.ICName,
    //                ItemName = model.ItemName,
    //                Qty = model.Qty,
    //                Unit = model.Unit,
    //                Location = model.Location,
    //                MPNNo = model.MPNNo,
    //                AltItemCode1 = model.AltItemCode1,
    //                AICName1 = model.AICName1,
    //                AltItemName1 = model.AltItemName1,
    //                AltQty1 = model.AltQty1,
    //                UsedStageId = model.UsedStageId,
    //                AltItemCode2 = model.AltItemCode2,
    //                AICName2 = model.AICName2,
    //                AltItemName2 = model.AltItemName2,
    //                AltQty2 = model.AltQty2,
    //                IssueToJOBwork = model.IssueToJOBwork,
    //                DirectProcess = model.DirectProcess,
    //                RecFrmCustJobwork = model.RecFrmCustJobwork,
    //                PkgItem = model.PkgItem,
    //                Remark = model.Remark,
    //                RunnerItemCode = model.RunnerItemCode,
    //                RunnerQty = model.RunnerQty,
    //                GrossWt = model.GrossWt,
    //                NetWt = model.NetWt,
    //                Scrap = model.Scrap,
    //                BurnQty = model.BurnQty,
    //                CustJwAdjustmentMandatory = model.CustJwAdjustmentMandatory
    //            });
    //            HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
    //        }
    //        else
    //        {
    //            return StatusCode(207, "Duplicate");
    //        }
    //    }

    //    return PartialView("_BomGrid", model);
    //}
    ////BOMDashboard--
    public async Task<IActionResult> Dashboard(string formKey)
    {
        ViewBag.formKey = formKey;

        int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
        var rights = await _IBom.GetFormRights(userID);
        if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
        {
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
        }
        var table = rights.Result.Tables[0];

        bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
        bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
        bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
        bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
        if (!(optAll || optView || optUpdate || optDelete))
        {
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
        }
        var model = new BomDashboard();

        // Dropdowns only
        model.FGPartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
        model.FGItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");

        model.RMPartCodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.RMItemNameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        return View(model);
    }

    //public async Task<IActionResult> Dashboard(string FGPartCode = "", string FGItemName = "", string RMPartCode = "", string RMItemName = "", string BomRevNo = "", string DashboardType = "", string Search = "", int pageNumber = 1, int pageSize = 50)
    //{
    //    //BomDashboard model = new BomDashboard
    //    //{
    //    //    FGPartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList"),
    //    //    FGItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList"),

    //    // RMPartCodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST",
    //    // "SP_GetDropDownList"), RMItemNameList = await
    //    // _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList"),        //    DTDashboard = await _IBom.GetBomDashboard("Dashboard"),
    //    //};

    //    var model = new BomDashboard();
    //    var oDataSet = await _IBom.GetBomDashboard("Dashboard").ConfigureAwait(false);

    //    if (oDataSet.Tables.Count != 0)
    //    {
    //        model.DTDashboard = oDataSet.Tables[0];

    //        model.FGPartCodeList = (from DataRow dr in oDataSet.Tables[0].Rows.Cast<DataRow>()
    //                                select new TextValue()
    //                                {
    //                                    Text = dr["FGPartCode"].ToString(),
    //                                    Value = dr["FGPartCode"].ToString(),
    //                                }).DistinctBy(x => x.Value).ToList();

    //        model.FGItemNameList = (from DataRow dr in oDataSet.Tables[0].Rows
    //                                select new TextValue()
    //                                {
    //                                    Text = dr["FGItem"].ToString(),
    //                                    Value = dr["FGItem"].ToString(),
    //                                }).DistinctBy(x => x.Value).ToList();

    //        model.RMPartCodeList = (from DataRow dr in oDataSet.Tables[0].Rows.Cast<DataRow>()
    //                                select new TextValue()
    //                                {
    //                                    Text = dr["RMPartCode"].ToString(),
    //                                    Value = dr["RMPartCode"].ToString(),
    //                                }).DistinctBy(x => x.Value).ToList();

    //        model.RMItemNameList = (from DataRow dr in oDataSet.Tables[0].Rows
    //                                select new TextValue()
    //                                {
    //                                    Text = dr["RMItemName"].ToString(),
    //                                    Value = dr["RMItemName"].ToString(),
    //                                }).DistinctBy(x => x.Value).ToList();
    //    }
    //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
    //    {
    //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
    //        SlidingExpiration = TimeSpan.FromMinutes(55),
    //        Size = 1024,
    //    };

    //    //model.FGPartCode = FGPartCode;
    //    //model.FGItemName = FGItemName;
    //    //model.RMPartCode = RMPartCode;
    //    //model.RMItemName = RMItemName;
    //    //model.BomRevNo = BomRevNo;
    //    //model.DashboardType = DashboardType;

    //    //model.DTDashboard = model.DTDashboard == null ? new System.Data.DataTable() : model.DTDashboard;
    //    //var DTDashboardPage = model.DTDashboard;
    //    //HttpContext.Session.SetString("KeyBomList", JsonConvert.SerializeObject(DTDashboardPage));
    //    //_MemoryCache.Set("KeyBOMList_Summary", JsonConvert.SerializeObject(DTDashboardPage), cacheEntryOptions);
    //    //model.TotalRecords = model.DTDashboard.Rows.Count;
    //    //model.PageNumber = pageNumber;
    //    //model.PageSize = pageSize;

    //    //var pagedRows = model.DTDashboard.AsEnumerable()
    //    //                         .Skip((pageNumber - 1) * pageSize)
    //    //                         .Take(pageSize);

    //    //model.DTDashboard = pagedRows.Any() ? pagedRows.CopyToDataTable() : model.DTDashboard.Clone();

    //    return View(model);
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




    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection, string formKey)
    {
        try
        {
            return RedirectToAction(nameof(BomForm), new { formKey = formKey });
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
        var sessionKey = GetBomSessionKey();

        var bomListJson = HttpContext.Session.GetString($"BomList_{sessionKey}");

        if (bomListJson != null)
        {
            model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(bomListJson);
            model.BomList.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (BomModel item in model.BomList)
            {
                Indx++;
                item.SeqNo = Indx;
            }

            HttpContext.Session.SetString($"BomList_{sessionKey}", JsonConvert.SerializeObject(model.BomList));

            //HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
        }
        return PartialView("_BomGrid", model);
    }
    public async Task<IActionResult> DeleteByID(string formKey, string FIC, int BMNo, string FGPartCode, string FGItemName, string RMPartCode, string RMItemName, string BomRevNo, string DashboardType, string GlobalSearch)
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

        return RedirectToAction("Dashboard", new { formKey = formKey, FGPartCode = FGPartCode, FGItemName = FGItemName, RMPartCode = RMPartCode, RMItemName = RMItemName, BomRevNo = BomRevNo, DashboardType = DashboardType, GlobalSearch = GlobalSearch });
    }

    public async Task<IActionResult> EditBomDetail(string formKey, string FIC, int BMNo, string Mode, string FGPartCode = "", string FGItemName = "", string RMPartCode = "", string RMItemName = "", string BomRevNo = "", string SummaryDetail = "", string GlobalSearch = "")
    {
        ViewBag.formKey = formKey;

        int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
        var rights = await _IBom.GetFormRights(userID);
        if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
        {
            return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
        }


        var table = rights.Result.Tables[0];
        string encFIC = Request.Query["FIC"].ToString();
        string encBMNo = Request.Query["BMNo"].ToString();

        if (!string.IsNullOrEmpty(encFIC) || !string.IsNullOrEmpty(encBMNo))
        {
            string decryptedFIC = EncryptDecrypt.Decrypt(encFIC);
            int decryptedBMNo = EncryptDecrypt.DecodeID(encBMNo);
            string decryptedMode = EncryptDecrypt.Decrypt(Mode);

            FIC = decryptedFIC;
            Mode = decryptedMode;
            BMNo = decryptedBMNo;
        }

        bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
        bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
        bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
        bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


        if (Mode == "U")
        {
            if (!(optUpdate))
            {
                return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
            }
        }
        else if (Mode == "V")
        {
            if (!(optView))
            {
                return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
            }
        }
        else if (Mode != "U")
        {
            if (!optSave)
            {
                return RedirectToAction("Dashboard", "Bom", new { formKey = formKey });
            }
            //if (!(optAll || optSave))
            //{
            //                    return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
            //}

        }

        BomModel model = await _IBom.EditBomDetail(FIC, BMNo, "EditBomDetail");

        model.Mode = Mode;
        model.BMNo = BMNo;
        model.FG1CodeList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.FG1NameList = await _IDataLogic.GetDropDownList("FINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.CodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
        model.NameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");

        model.ApprovedByList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");

        model.UsedStageList = await _IDataLogic.GetDropDownList("UsedStageList", "SP_GetDropDownList");
        var sessionKey = GetBomSessionKey();
        HttpContext.Session.Remove($"BomList_{sessionKey}");
        HttpContext.Session.SetString($"BomList_{sessionKey}", JsonConvert.SerializeObject(model.BomList));

        //HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));

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

        //int Indx = Convert.ToInt32(model.SeqNo) - 1;
        int seq = Convert.ToInt32(model.SeqNo);

        var sessionKey = GetBomSessionKey();
        var bomListJson = HttpContext.Session.GetString($"BomList_{sessionKey}");

        if (bomListJson != null)
        {
            model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(bomListJson);
            //Result = model.BomList.Where(m => m.SeqNo == model.SeqNo).ToList();
            Result = model.BomList.Where(m => m.SeqNo == model.SeqNo).ToList();
            var removebom = model.BomList.FirstOrDefault(x => x.SeqNo == seq);
            if (removebom != null)
            {
                model.BomList.Remove(removebom);
            }

            if (model.BomList.Count > 0)
            {
                HttpContext.Session.SetString($"BomList_{sessionKey}", JsonConvert.SerializeObject(model.BomList));
                //  HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
            }
            else
            {
                HttpContext.Session.Remove($"BomList_{sessionKey}");

                //  HttpContext.Session.Remove("BomList");
            }
        }
        return Json(JsonConvert.SerializeObject(Result));
        //int seq = Convert.ToInt32(model.SeqNo);
        //if (HttpContext.Session.GetString("BomList") != null)
        //{
        //    model.BomList = JsonConvert.DeserializeObject<List<BomModel>>(HttpContext.Session.GetString("BomList"));
        //    Result = model.BomList.Where(m => m.SeqNo == model.SeqNo).ToList();

        //    model.BomList.RemoveAt(Convert.ToInt32(Indx));

        //    Indx = 0;

        //    foreach (BomModel item in model.BomList)
        //    {
        //        Indx++;
        //        item.SeqNo = Indx;
        //    }

        //    if (model.BomList.Count > 0)
        //    {
        //        HttpContext.Session.SetString("BomList", JsonConvert.SerializeObject(model.BomList));
        //    }
        //    else
        //    {
        //        HttpContext.Session.Remove("BomList");
        //    }
        //}
        //return Json(JsonConvert.SerializeObject(Result));
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
        //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //{
        //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //    SlidingExpiration = TimeSpan.FromMinutes(55),
        //    Size = 1024,
        //};

        //_MemoryCache.Set("KeyBOMList_BomDetail", model, cacheEntryOptions);

        return PartialView("_BomDetail", model);
    }

    public JsonResult GetBomNo(int GBN)
    {
        GBN = _IBom.GetBomNo(GBN, "GetBomNo");
        return Json(new { GBN = GBN });
    }

    public async Task<IActionResult> GetSearchData(BomDashboard model, string formKey, int pageNumber = 1, int pageSize = 15, string SearchBox = "")

    {
        try
        {
            ViewBag.formKey = formKey;

            int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));

            var result = await _IBom.GetSearchData(model, userID);
            if (result == null || !(result.Result is DataTable dt))
            {
                return PartialView("_BomDashboardDetailGrid", model);
            }

            // ✅ SEARCH (on DataTable)
            var filteredRows = string.IsNullOrWhiteSpace(SearchBox)
                ? dt.AsEnumerable()
                : dt.AsEnumerable().Where(r =>
                    dt.Columns.Cast<DataColumn>()
                        .Any(c => r[c] != DBNull.Value &&
                                  r[c].ToString()
                                      .Contains(SearchBox, StringComparison.OrdinalIgnoreCase)));

            var sessionRows = filteredRows
    .Select(r => dt.Columns
        .Cast<DataColumn>()
        .ToDictionary(
            c => c.ColumnName,
            c => r[c] == DBNull.Value ? null : r[c]
        ))
    .ToList();
            HttpContext.Session.SetString(
"KeyBOMList_Detail",
JsonConvert.SerializeObject(sessionRows)
);
            model.Headers = dt.Columns
                .Cast<DataColumn>()
                .Select(c => new DashboardColumn
                {
                    Title = c.ColumnName,
                    Field = c.ColumnName
                })
                .ToList();
            // ✅ TOTAL RECORDS (before pagination)
            model.TotalRecords = filteredRows.Count();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Rows = filteredRows
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(r => dt.Columns
                     .Cast<DataColumn>()
                     .ToDictionary(
                         c => c.ColumnName,
                         c => r[c] == DBNull.Value ? null : r[c]
                     ))
                 .ToList();


            return PartialView("_BomDashboardDetailGrid", model);
        }
        catch
        {
            throw;
        }
    }


    //public async Task<IActionResult> GetSearchData(BomDashboard model, int pageNumber = 1, int pageSize = 500)
    //{
    //    int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));

    //    model = await _IBom.GetSearchData(model, userID);
    //    if (model?.DTDashboard != null)
    //    {

    //        var list = ConvertDataTableToList<BomDashboard>(model.DTDashboard);

    //        model.TotalRecords = list.Count;

    //        var paginatedList = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //        model.DTDashboard = ToDataTable(paginatedList);

    //        model.PageNumber = pageNumber;
    //        model.PageSize = pageSize;
    //    }
    //    model.FGPartCodeList = await _IDataLogic.GetDropDownList("ALLGOODS", "CODELIST", "SP_GetDropDownList");
    //    model.FGItemNameList = await _IDataLogic.GetDropDownList("ALLGOODS", "NAMELIST", "SP_GetDropDownList");

    //    model.RMPartCodeList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "CODELIST", "SP_GetDropDownList");
    //    model.RMItemNameList = await _IDataLogic.GetDropDownList("UNFINISHEDGOODS", "NAMELIST", "SP_GetDropDownList");
    //    //var Result = System.Text.Json.JsonSerializer.Serialize(model);
    //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
    //    {
    //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
    //        SlidingExpiration = TimeSpan.FromMinutes(55),
    //        Size = 1024,
    //    };

    //   _MemoryCache.Set("KeyBOMList_Summary", model, cacheEntryOptions);
    //    return PartialView("_BomDashboardGrid", model);
    //}

    [HttpGet]
    public IActionResult GlobalSearch(string ReportType, string searchString, string formKey, int pageNumber = 1, int pageSize = 10)
    {
        BomDashboard model = new BomDashboard();
        ViewBag.formKey = formKey;
        // 1️⃣ Get session data
        string modelJson = HttpContext.Session.GetString("KeyBOMList_Detail");

        if (string.IsNullOrWhiteSpace(modelJson))
        {
            model.Rows = new List<Dictionary<string, object>>();
            model.Headers = new List<DashboardColumn>();
            model.TotalRecords = 0;
            return PartialView("_BOMDashboardDetailGrid", model);
        }


        // 2️⃣ Deserialize rows
        var allRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelJson);

        if (allRows == null || allRows.Count == 0)
        {
            model.Rows = new List<Dictionary<string, object>>();
            model.Headers = new List<DashboardColumn>();
            model.TotalRecords = 0;
            return PartialView("_BOMDashboardDetailGrid", model);
        }

        // 3️⃣ Dynamic search (all columns)
        var filteredRows = string.IsNullOrWhiteSpace(searchString)
            ? allRows
            : allRows.Where(row =>
                row.Values.Any(val =>
                    val != null &&
                    val.ToString()
                       .Contains(searchString, StringComparison.OrdinalIgnoreCase)))
              .ToList();

        // fallback → show all
        if (filteredRows.Count == 0)
            filteredRows = allRows;

        // 4️⃣ Dynamic headers (from keys)
        model.Headers = filteredRows.First()
            .Keys
            .Select(k => new DashboardColumn
            {
                Title = k,
                Field = k
            })
            .ToList();
        HttpContext.Session.SetString(
"KeyBOMList_Detail",
JsonConvert.SerializeObject(filteredRows)
);
        // 5️⃣ Pagination
        model.TotalRecords = filteredRows.Count;
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;

        model.Rows = filteredRows
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return PartialView("_BOMDashboardDetailGrid", model);
    }


    //[HttpGet]

    //public IActionResult GlobalSearch(string ReportType, string searchString, int pageNumber = 1, int pageSize = 500 )
    //{
    //    BomDashboard model = new BomDashboard();
    //    //DataTable bomViewModel = new DataTable();
    //    //string bomData = HttpContext.Session.GetString("KeyBomList");
    //    //WIPStockRegisterModel model = new WIPStockRegisterModel();

    //    if (string.IsNullOrWhiteSpace(searchString))
    //    {
    //        return PartialView("_BomDashboardGrid", model); // return empty model (or paginated full list)
    //    }

    //    string cacheKey = $"KeyBOMList_{ReportType}";
    //    //if (!_MemoryCache.TryGetValue(cacheKey, out IList<BomDashboard> bomDashboard) || bomDashboard == null)
    //    //{
    //    //    return PartialView("_BomDashboardGrid", new List<BomDashboard>());
    //    //}

    //    if (!_MemoryCache.TryGetValue(cacheKey, out BomDashboard dashboardModel) || dashboardModel == null || dashboardModel.DTDashboard == null)
    //    {
    //        return PartialView("_BomDashboardGrid", new List<BomDashboard>());
    //    }

    //    List<BomDashboard> bomDashboard = ConvertDataTableToList<BomDashboard>(dashboardModel.DTDashboard);

    //    //string modelJson = HttpContext.Session.GetString("KeyBomList");
    //    List<BomDashboard> filteredResults;
    //    if (string.IsNullOrWhiteSpace(searchString))
    //    {
    //        filteredResults = bomDashboard.ToList();
    //    }
    //    else
    //    {
    //        filteredResults = bomDashboard
    //            .Where(i => i.GetType().GetProperties()
    //                .Where(p => p.PropertyType == typeof(string))
    //                .Select(p => p.GetValue(i)?.ToString())
    //                .Any(value => !string.IsNullOrEmpty(value) &&
    //                              value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
    //            .ToList();


    //        if (filteredResults.Count == 0)
    //        {
    //            filteredResults = bomDashboard.ToList();
    //        }
    //    }

    //    model.TotalRecords = filteredResults.Count;
    //    model.DTDashboard = ToDataTable(filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
    //    model.PageNumber = pageNumber;
    //    model.PageSize = pageSize;
    //    model.TotalRecords = filteredResults.Count;
    //    //model.DTDashboard = ToDataTable(filteredResults);
    //    //model.DTDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //    //var pagedList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //    //model.BomList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //    //model.PageNumber = pageNumber;
    //    //model.PageSize = pageSize;
    //    var viewName = string.Equals(ReportType?.Trim(), "detail", StringComparison.OrdinalIgnoreCase)
    //? "_BomDashboardDetailGrid"
    //: "_BomDashboardGrid";

    //    return PartialView(viewName, model);

    //  //  return PartialView("_BomDashboardGrid", model);
    //}
    public static List<T> ConvertDataTableToList<T>(DataTable dt) where T : new()
    {
        var dataList = new List<T>();

        foreach (DataRow row in dt.Rows)
        {
            T item = new T();

            foreach (DataColumn column in dt.Columns)
            {
                PropertyInfo prop = typeof(T).GetProperty(column.ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && row[column] != DBNull.Value)
                {
                    try
                    {
                        object value = Convert.ChangeType(row[column], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        prop.SetValue(item, value, null);
                    }
                    catch
                    {
                        // Skip setting this property if conversion fails
                        continue;
                    }
                }
            }

            dataList.Add(item);
        }

        return dataList;
    }

    public JsonResult GetUnit(string IC)
    {
        IC = _IBom.GetUnit(IC, "GetUnit");
        return Json(new { IC = IC });
    }
    public string RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;

        using (var sw = new StringWriter())
        {
            var viewResult = HttpContext.RequestServices
                .GetService<ICompositeViewEngine>()
                .FindView(ControllerContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).Wait();
            return sw.GetStringBuilder().ToString();
        }
    }
    public IActionResult GetGridData(int IC, int BMNo)
    {
        var model = new BomModel();
        model = _IBom.GetGridData(IC, BMNo);
        var sessionKey = GetBomSessionKey();

        HttpContext.Session.SetString($"KeyBomList{sessionKey}", JsonConvert.SerializeObject(model.BomList));

        //HttpContext.Session.SetString("KeyBomList", JsonConvert.SerializeObject(model.BomList));
        bool shouldClearGrid = model.BomList?.Any() == true;

        string html = RenderPartialViewToString("_BomGrid", model);

        return Json(new
        {
            shouldClearGrid,
            html
        });
        // return PartialView("_BomGrid", model);
    }
    public ActionResult ImportBom(string formKey)
    {
        ViewBag.formKey = formKey;

        BomModel model = new BomModel();
        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
        return View(model);
    }
    [HttpPost]

    public IActionResult UploadExcel(IFormFile excelFile)
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        List<BomViewModel> data = new();
        List<BOMExcelRowError> errorList = new();

        var UpdateRevNo = Request.Form.Where(x => x.Key == "UpdateRevNo").FirstOrDefault().Value;

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var uniqueBomKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                try
                {
                    var FGPartCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(FGPartCode))
                        break;

                    var RMPartCode = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                    var RunnerPartCode = worksheet.Cells[row, 26].Value?.ToString()?.Trim();
                    var AltPartCode1 = worksheet.Cells[row, 10].Value?.ToString()?.Trim();
                    var AltPartCode2 = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
                    var AltPartCode3 = worksheet.Cells[row, 12].Value?.ToString()?.Trim();
                    var AltPartCode4 = worksheet.Cells[row, 13].Value?.ToString()?.Trim();
                    var AltPartCode5 = worksheet.Cells[row, 14].Value?.ToString()?.Trim();

                    if (string.IsNullOrWhiteSpace(RMPartCode))
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "RMPartCode is empty"
                        });
                        continue;
                    }

                    string bomKey = $"{FGPartCode}|{RMPartCode}";

                    if (!uniqueBomKeys.Add(bomKey))
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = $"Duplicate FGPartCode + RMPartCode found: {FGPartCode}, {RMPartCode}"
                        });
                        continue;
                    }

                    var duplicateResult = _IBom.IsBomExists(FGPartCode, RMPartCode, 1);
                    int BomRevNo = 0;
                    if (duplicateResult?.Result?.Result?.Tables.Count > 0)
                    //if (duplicateResult.Result != null &&
                    //duplicateResult.Result.Tables.Count > 0 &&
                    //duplicateResult.Result.Tables[0].Rows.Count > 0)
                    {
                        var dt = duplicateResult.Result.Result.Tables[0].Rows[0];
                        if (Convert.ToInt32(dt["IsDuplicate"]) == 1)
                        {
                            if (UpdateRevNo == "Y")
                            {
                                BomRevNo = Convert.ToInt32(dt["BomNo"]);
                            }
                            else
                            {
                                errorList.Add(new BOMExcelRowError
                                {
                                    RowNo = row,
                                    Message = $"BOM already exists for FG PartCode : {FGPartCode}, RM PartCode : {RMPartCode}"
                                });

                                continue;
                            }
                        }
                    }
                    var FGIC = _IBom.GetItemCode(FGPartCode, RMPartCode, RunnerPartCode, AltPartCode1, AltPartCode2, AltPartCode3, AltPartCode4, AltPartCode5);
                    int FGItemCode = 0, RMItemCode = 0, RunnerItemCode = 0, AltItemCode1 = 0, AltItemCode2 = 0, AltItemCode3 = 0, AltItemCode4 = 0, AltItemCode5 = 0;
                    string FGItemName = "", RMItemName = "", RunnerItemName = "";

                    if (FGIC?.Result?.Result?.Tables.Count > 0)
                    {
                        var r = FGIC.Result.Result.Tables[0].Rows[0];

                        FGItemCode = Convert.IsDBNull(r["FGItemCode"]) ? 0 : Convert.ToInt32(r["FGItemCode"]);
                        RMItemCode = Convert.IsDBNull(r["RMItemCode"]) ? 0 : Convert.ToInt32(r["RMItemCode"]);
                        RunnerItemCode = Convert.IsDBNull(r["RunnerItemCode"]) ? 0 : Convert.ToInt32(r["RunnerItemCode"]);
                        AltItemCode1 = Convert.IsDBNull(r["AltItemCode1"]) ? 0 : Convert.ToInt32(r["AltItemCode1"]);
                        AltItemCode2 = Convert.IsDBNull(r["AltItemCode2"]) ? 0 : Convert.ToInt32(r["AltItemCode2"]);
                        AltItemCode3 = Convert.IsDBNull(r["AltItemCode3"]) ? 0 : Convert.ToInt32(r["AltItemCode3"]);
                        AltItemCode4 = Convert.IsDBNull(r["AltItemCode4"]) ? 0 : Convert.ToInt32(r["AltItemCode4"]);
                        AltItemCode5 = Convert.IsDBNull(r["AltItemCode5"]) ? 0 : Convert.ToInt32(r["AltItemCode5"]);
                        FGItemName = Convert.IsDBNull(r["FGItemName"]) ? "" : r["FGItemName"].ToString();
                        RMItemName = Convert.IsDBNull(r["RMItemName"]) ? "" : r["RMItemName"].ToString();
                        RunnerItemName = Convert.IsDBNull(r["RunnerItemName"]) ? "" : r["RunnerItemName"].ToString();
                    }

                    if (FGItemCode == 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = $"Invalid FGPartCode: {FGPartCode}"
                        });
                        continue;
                    }

                    if (RMItemCode == 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = $"Invalid RMPartCode: {RMPartCode}"
                        });
                        continue;
                    }

                    if (FGItemCode == RMItemCode)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "FGItemCode and RMItemCode cannot be same"
                        });
                        continue;
                    }
                    var RMUnit = _IBom.GetUnit(RMItemCode.ToString(), "GetUnit");
                    if (RMUnit == null)
                    {
                        errorList.Add(
                            new BOMExcelRowError
                            {
                                RowNo = row,
                                Message = $"Unit not found for RMItemCode: {RMItemCode}"
                            });
                        continue;
                    }
                    else
                    {
                        RMUnit = RMUnit.Trim();
                    }
                    var RMQty = worksheet.Cells[row, 6].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 6].Value);
                    var RMGrossWt = worksheet.Cells[row, 23].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 17].Value);
                    var RMNetWt = worksheet.Cells[row, 24].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 18].Value);
                    var RMScrapWt = worksheet.Cells[row, 22].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 16].Value);
                    var RMMPNNo = worksheet.Cells[row, 9].Value == null ? 0 :
                                (worksheet.Cells[row, 9].Value);
                    var Location = worksheet.Cells[row, 8].Value == null ? "" :
                                (worksheet.Cells[row, 8].Value).ToString();
                    var AltQty1 = worksheet.Cells[row, 15].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 15].Value);
                    var AltQty2 = worksheet.Cells[row, 16].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 16].Value);
                    var AltQty3 = worksheet.Cells[row, 17].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 17].Value);
                    var AltQty4 = worksheet.Cells[row, 18].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 18].Value);
                    var AltQty5 = worksheet.Cells[row, 19].Value == null ? 0 :
                                Convert.ToDecimal(worksheet.Cells[row, 19].Value);
                    if (RMQty == 0)
                    {
                        errorList.Add(
                            new BOMExcelRowError
                            {
                                RowNo = row,
                                Message = $"RMQty cannot be zero for RMItemCode: {RMItemCode}"
                            });
                        continue;
                    }

                    var itemCodes = new List<int>
{
    FGItemCode,
    RMItemCode
};

                    if (AltItemCode1 > 0) itemCodes.Add(AltItemCode1);
                    if (AltItemCode2 > 0) itemCodes.Add(AltItemCode2);
                    if (AltItemCode3 > 0) itemCodes.Add(AltItemCode3);
                    if (AltItemCode4 > 0) itemCodes.Add(AltItemCode4);
                    if (AltItemCode5 > 0) itemCodes.Add(AltItemCode5);

                    if (itemCodes.Count != itemCodes.Distinct().Count())
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "FG, RM and Alternate Item Codes cannot be the same."
                        });
                        continue;
                    }
                    if (AltItemCode1 > 0 && AltQty1 <= 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "Alternate Item 1 Qty must be greater than 0."
                        });
                        continue;
                    }

                    if (AltItemCode2 > 0 && AltQty2 <= 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "Alternate Item 2 Qty must be greater than 0."
                        });
                        continue;
                    }

                    if (AltItemCode3 > 0 && AltQty3 <= 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "Alternate Item 3 Qty must be greater than 0."
                        });
                        continue;
                    }

                    if (AltItemCode4 > 0 && AltQty4 <= 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "Alternate Item 4 Qty must be greater than 0."
                        });
                        continue;
                    }

                    if (AltItemCode5 > 0 && AltQty5 <= 0)
                    {
                        errorList.Add(new BOMExcelRowError
                        {
                            RowNo = row,
                            Message = "Alternate Item 5 Qty must be greater than 0."
                        });
                        continue;
                    }
                    if (BomRevNo == 0)
                    {
                        BomRevNo = 1;
                    }
                    // ✅ VALID ROW — SAME AS YOUR CURRENT LOGIC
                    data.Add(new BomViewModel
                    {
                        FGPartCode = FGPartCode,
                        BomName = FGPartCode,
                        FGItemCode = FGItemCode,
                        FGItemName = FGItemName,
                        RMItemCode = RMItemCode,
                        RMPartCode = RMPartCode,
                        RMItemName = RMItemName,
                        BomNo = BomRevNo,
                        RMQty = RMQty,
                        RMUnit = RMUnit,
                        GrossWeight = RMGrossWt,
                        NetWeight = RMNetWt,
                        Scrap = RMScrapWt,
                        MPNNumber = RMMPNNo.ToString(),
                        Location = Location.ToString(),
                        RunnerItemCode = RunnerItemCode,
                        AltItemCode1 = AltItemCode1,
                        AltItemCode2 = AltItemCode2,
                        AltItemCode3 = AltItemCode3,
                        AltItemCode4 = AltItemCode4,
                        AltItemCode5 = AltItemCode5,
                        AltPartCode1 = AltPartCode1,
                        AltPartCode2 = AltPartCode2,
                        AltPartCode3 = AltPartCode3,
                        AltPartCode4 = AltPartCode4,
                        AltPartCode5 = AltPartCode5,
                        AltQty1 = AltQty1,
                        AltQty2 = AltQty2,
                        AltQty3 = AltQty3,
                        AltQty4 = AltQty4,
                        AltQty5 = AltQty5,

                        RunnerPartCode = RunnerPartCode

                    });
                }
                catch (Exception ex)
                {
                    errorList.Add(new BOMExcelRowError
                    {
                        RowNo = row,
                        Message = ex.Message
                    });
                }
            }
        }

        HttpContext.Session.SetString(
      "BOM_EXCEL_DATA",
      Newtonsoft.Json.JsonConvert.SerializeObject(data)
  );


        ViewBag.ExcelErrors = errorList;

        var model = new BomModel
        {
            ExcelDataList = data
        };


        return PartialView("_DisplayExcelData", model);
    }

    //public IActionResult UploadExcel(IFormFile excelFile)
    //{
    //    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    //    List<BomViewModel> data = new List<BomViewModel>();

    //    using (var stream = excelFile.OpenReadStream())
    //    using (var package = new ExcelPackage(stream))
    //    {
    //        var worksheet = package.Workbook.Worksheets[0];
    //        List<ImportBomData> importDataList = new();
    //        var BomData = _IBom.CheckDupeConstraint();

    //        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
    //        {
    //            var cellValue = worksheet.Cells[row, 1].Value;

    //            if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
    //                break; // Stop when column 1 is empty
    //            var FGPartCode = worksheet.Cells[row, 1].Value.ToString();



    //            var RMPartCode = worksheet.Cells[row, 3].Value.ToString();
    //            var AltPartCode1 = worksheet.Cells[row, 8].Value == null ? "" : worksheet.Cells[row, 8].Value.ToString();
    //            var AltPartCode2 = worksheet.Cells[row, 9].Value == null ? "" : worksheet.Cells[row, 9].Value.ToString();
    //            var FGItemCode = 0; var RmItemCode = 0; var AltItemCode1 = 0; var AltItemCode2 = 0;
    //            var FGItemName = ""; var RMItemName = "";
    //            var FGIC = _IBom.GetItemCode(FGPartCode, RMPartCode);

    //            var bomPartCodeData = new ImportBomData()
    //            {
    //                FGPartCode = FGPartCode,
    //                RMPartCode = RMPartCode
    //            };
    //            var RMQty = Convert.ToDouble(worksheet.Cells[row, 6].Value.ToString());

    //            importDataList.Add(bomPartCodeData);

    //            if (!string.IsNullOrEmpty(AltPartCode1) && AltPartCode1 != "0")
    //            {
    //                var AltPC = _IBom.GetAltItemCode(AltPartCode1);
    //                if (AltPC != null)
    //                {
    //                    var resultDataSet = AltPC.Result.Result;
    //                    if (resultDataSet != null)
    //                    {
    //                        var firstTable = resultDataSet.Tables[0];
    //                        foreach (DataRow row1 in firstTable.Rows)
    //                        {
    //                            AltItemCode1 = Convert.ToInt32(row1["AltItemCode"]);
    //                        }
    //                    }
    //                }
    //            }
    //            if (!string.IsNullOrEmpty(AltPartCode2) && AltPartCode2 != "0")
    //            {
    //                var AltPC = _IBom.GetAltItemCode(AltPartCode2);
    //                if (AltPC != null)
    //                {
    //                    var resultDataSet = AltPC.Result.Result;
    //                    if (resultDataSet != null)
    //                    {
    //                        var firstTable = resultDataSet.Tables[0];
    //                        foreach (DataRow row1 in firstTable.Rows)
    //                        {
    //                            AltItemCode2 = Convert.ToInt32(row1["AltItemCode"]);
    //                        }
    //                    }
    //                }
    //            }

    //            if (FGIC != null)
    //            {
    //                var resultDataSet = FGIC.Result.Result;
    //                if (resultDataSet != null)
    //                {
    //                    var firstTable = resultDataSet.Tables[0];
    //                    var row1 = firstTable.Rows[0];

    //                    FGItemCode = !Convert.IsDBNull(row1["FGItemCode"]) ? Convert.ToInt32(row1["FGItemCode"]) : 0;
    //                    RmItemCode = !Convert.IsDBNull(row1["RMItemCode"]) ? Convert.ToInt32(row1["RMItemCode"]) : 0;
    //                    FGItemName = !Convert.IsDBNull(row1["FGItemName"]) ? row1["FGItemName"].ToString() : "";
    //                    RMItemName = !Convert.IsDBNull(row1["RMItemName"]) ? row1["RMItemName"].ToString() : "";
    //                }
    //            }

    //            if (FGItemCode == 0 )
    //{
    //	return StatusCode(207, "Invalid FGPartCode   " + FGPartCode);
    //}
    //if (RmItemCode == 0)
    //{
    //	return StatusCode(207, "Invalid  RMPartCode  " + RMPartCode);
    //}
    //            if (RmItemCode == FGItemCode)
    //            {
    //                return StatusCode(207, $"Error: FGItemCode and RmItemCode cannot be the same. Row: {row}, FGItemCode: {RmItemCode}");
    //            }
    //            if (!string.IsNullOrWhiteSpace(FGItemName) && !string.IsNullOrWhiteSpace(RMItemName) && FGItemName == RMItemName)
    //            {
    //                return StatusCode(207, $"Error: FGItemName and RMItemName cannot be the same. Row: {row}, FGItemName: {FGItemName}");
    //            }


    //            var BomRevNo = _IBom.GetBomNo(FGItemCode, "GetBomNo");
    //            //var BomRevNoChck = _IBom.GetBomNo(FGItemCode, "GetCheckBomNo");

    //            var duplicateBom = "";

    //            if (BomData != null)
    //            {
    //                var oDT = BomData.Result.DefaultView.ToTable(true, "FinishItemCode", "BomNo", "ItemCode");
    //                oDT.TableName = "BOMDataForConstraint";

    //                //DashBoardData.PODashboard = CommonFunc.DataTableToList<PODashBoard>(oDT);

    //                var bomData = CommonFunc.DataTableToList<BomModel>(oDT);

    //                var checkConstraint = bomData.Where(x => x.FinishItemCode == FGItemCode && x.ItemCode == RmItemCode && x.BomNo == BomRevNo).ToList();

    //                if (checkConstraint.Count > 0)
    //                {
    //                    duplicateBom = "true";
    //                }
    //            }

    //            data.Add(new BomViewModel()
    //            {
    //                FGPartCode = FGPartCode,
    //                FGItemName = FGItemName,
    //                FGItemCode = FGItemCode,
    //                RMItemCode = RmItemCode,
    //                RMPartCode = RMPartCode,
    //                RMItemName = RMItemName,

    //                BomName = worksheet.Cells[row, 5].Value?.ToString() ?? "",

    //                RMQty = worksheet.Cells[row, 6].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 6].Value),

    //                RMUnit = worksheet.Cells[row, 7].Value?.ToString() ?? "",
    //                Location = worksheet.Cells[row, 8].Value?.ToString() ?? "",
    //                MPNNumber = worksheet.Cells[row, 9].Value?.ToString() ?? "",

    //                AltPartCode1 = worksheet.Cells[row, 10].Value?.ToString() ?? "",
    //                AltItemCode1 = AltItemCode1,
    //                AltItemCode2 = AltItemCode2,
    //                AltPartCode2 = worksheet.Cells[row, 11].Value?.ToString() ?? "",

    //                AltQty1 = worksheet.Cells[row, 12].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 12].Value),

    //                AltQty2 = worksheet.Cells[row, 13].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 13].Value),

    //                Scrap = worksheet.Cells[row, 14].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 14].Value),

    //                GrossWeight = worksheet.Cells[row, 15].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 15].Value),

    //                NetWeight = worksheet.Cells[row, 16].Value == null
    //            ? 0
    //            : Convert.ToDecimal(worksheet.Cells[row, 16].Value),

    //                Remark = worksheet.Cells[row, 17].Value?.ToString() ?? "",

    //                BomNo = BomRevNo,
    //                ConstraintExists = duplicateBom
    //            });

    //        }

    //        // duplicate items
    //        var duplicateParts = importDataList
    //    .GroupBy(x => new { x.FGPartCode, x.RMPartCode })
    //    .Where(g => g.Count() > 1)
    //    .Select(g => new
    //    {
    //        FGPartCode = g.Key.FGPartCode,
    //        RMPartCode = g.Key.RMPartCode,
    //        Count = g.Count()
    //    })
    //    .ToList();

    //        if (duplicateParts.Any())
    //        {
    //            foreach (var item in duplicateParts)
    //            {
    //                var errorMsg = "Duplicate: FGPartCode = {item.FGPartCode}, RMPartCode = {item.RMPartCode}, Count = {item.Count}";
    //                return StatusCode(207, errorMsg);
    //            }
    //        }

    //        // Get Bom Detail
    //        var bomDataTable = GetBomDetailTable(importDataList);
    //        //var isValidPartCodes = _IBom.VerifyPartCode(bomDataTable);
    //        //var extractedData = JsonConvert.DeserializeObject<List<dynamic>>(isValidPartCodes.Result);

    //        //var simplifiedResponse = extractedData.Select(x => new
    //        //{
    //        //    FGPartCode = x.FGPartCode,
    //        //    RMPartCode = x.RMPartCode
    //        //}).ToList();

    //        //var response = new
    //        //{
    //        //    Message = "Some part codes are invalid. Please check the details.",
    //        //    InvalidPartCodes = simplifiedResponse
    //        //};

    //        //string jsonResponse = JsonConvert.SerializeObject(response);

    //        //if (simplifiedResponse.Count > 0)
    //        //{
    //        //    return StatusCode(207, jsonResponse);
    //        //}
    //    }

    //    var model = new BomModel();
    //    model.ExcelDataList = data;
    //    return PartialView("_DisplayExcelData", model);
    //}
    //[HttpPost]
    //[Consumes("application/json")]
    //public async Task<IActionResult> AddBomListData([FromBody] List<BomViewModel> model)
    //{
    //    if (model == null)
    //        return BadRequest("Model is NULL");

    //    if (model.Count == 0)
    //        return BadRequest("Model count is ZERO");

    //    int seq = 1;
    //    foreach (var item in model)
    //        item.SeqNo = seq++;

    //    var CC = HttpContext.Session.GetString($"Branch_{formKey}");
    //    var EmpID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
    //    var yearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));

    //    var dt = GetDetailTable(model, CC, EmpID, yearCode);
    //    var result = await _IBom.SaveMultipleBomData(dt);

    //    return Ok(new { Count = model.Count });
    //}

    [HttpPost]
    public async Task<IActionResult> AddBomListData(string formKey)
    {
        var json = HttpContext.Session.GetString("BOM_EXCEL_DATA");

        if (string.IsNullOrEmpty(json))
            return BadRequest("Session expired or no data found");

        var model = Newtonsoft.Json.JsonConvert
            .DeserializeObject<List<BomViewModel>>(json);

        int seq = 1;
        foreach (var item in model)
            item.SeqNo = seq++;



        var CC = HttpContext.Session.GetString($"Branch_{formKey}");
        var EmpID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
        var yearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));

        var dt = GetDetailTable(model, CC, EmpID, yearCode);
        var Result = await _IBom.SaveMultipleBomData(dt);

        HttpContext.Session.Remove("BOM_EXCEL_DATA"); // cleanup
        if (Result != null)
        {
            if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            {
                return Json(new
                {
                    success = true,
                    message = Result.StatusText
                });
            }

            return Json(new
            {
                success = false,
                message = Result.StatusText
            });
        }


        return BadRequest(new
        {
            StatusText = "No response from database.",
            StatusCode = 500
        });
        //return Ok(new { Count = model.Count });
    }


    private static System.Data.DataTable GetBomDetailTable(List<ImportBomData> DetailList)
    {
        var BOMGrid = new System.Data.DataTable();

        BOMGrid.Columns.Add("SeqNo", typeof(int));
        BOMGrid.Columns.Add("FGPartCode", typeof(string));
        BOMGrid.Columns.Add("RMPartCode", typeof(string));
        BOMGrid.Columns.Add("BOMQty", typeof(decimal));
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
        MRGrid.Columns.Add("BomQty", typeof(decimal));
        MRGrid.Columns.Add("EntryDate", typeof(string));
        MRGrid.Columns.Add("EffectiveDate", typeof(string));
        MRGrid.Columns.Add("SeqNo", typeof(int));
        MRGrid.Columns.Add("ItemCode", typeof(int));
        MRGrid.Columns.Add("Qty", typeof(decimal));
        MRGrid.Columns.Add("Unit", typeof(string));
        MRGrid.Columns.Add("UsedStageId", typeof(string));
        MRGrid.Columns.Add("AltItemCode1", typeof(int));
        MRGrid.Columns.Add("AltQty1", typeof(decimal));
        MRGrid.Columns.Add("AltItemCode2", typeof(int));
        MRGrid.Columns.Add("AltQty2", typeof(decimal));
        MRGrid.Columns.Add("AltItemCode3", typeof(int));
        MRGrid.Columns.Add("AltQty3", typeof(decimal));
        MRGrid.Columns.Add("AltItemCode4", typeof(int));
        MRGrid.Columns.Add("AltQty4", typeof(decimal));
        MRGrid.Columns.Add("AltItemCode5", typeof(int));
        MRGrid.Columns.Add("AltQty5", typeof(decimal));
        MRGrid.Columns.Add("Location", typeof(string));
        MRGrid.Columns.Add("IssueToJoBWork", typeof(string));
        MRGrid.Columns.Add("DirectProcess", typeof(string));
        MRGrid.Columns.Add("RecFrmCustJobWork", typeof(string));
        MRGrid.Columns.Add("PkgItem", typeof(string));
        MRGrid.Columns.Add("Remark", typeof(string));
        MRGrid.Columns.Add("GrossWt", typeof(decimal));
        MRGrid.Columns.Add("NetWt", typeof(decimal));
        MRGrid.Columns.Add("Scrap", typeof(decimal));
        MRGrid.Columns.Add("RunnerItemCode", typeof(int));
        MRGrid.Columns.Add("RunnerQty", typeof(decimal));
        MRGrid.Columns.Add("BurnQty", typeof(decimal));
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
        MRGrid.Columns.Add("TotalRmQtyForTotBomQty", typeof(decimal));

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
                   today.ToString("yyyy-MM-dd").Split(" ")[0],
                   today.ToString("yyyy-MM-dd").Split(" ")[0],
                    Item.SeqNo,
                    Item.RMItemCode,
                    Item.RMQty,
                    Item.RMUnit,
                    "",Item.AltItemCode1
                    ,Item.AltQty1
                    ,Item.AltItemCode2
                    ,Item.AltQty2
                    ,Item.AltItemCode3
                    ,Item.AltQty3
                    ,Item.AltItemCode4
                    ,Item.AltQty4
                    ,Item.AltItemCode5
                    ,Item.AltQty5
                    ,Item.Location,
                    "","","","",
                    Item.Remark,Item.GrossWeight,Item.NetWeight
                    ,Item.Scrap
                    ,Item.RunnerItemCode
                    ,0,Item.BurnQty,Empid,
                    CC,YearCode,Empid,
                    ParseFormattedDate(DateTime.UtcNow.ToString()),
                    0,
                    ParseFormattedDate(DateTime.UtcNow.ToString()),"Y",
                    Environment.MachineName,
                    Item.MPNNumber,
                    Item.CustJWmandatory,
                    0,

                });
        }
        MRGrid.Dispose();
        return MRGrid;
    }
    public async Task<JsonResult> GetByProdItemName(int MainItemcode)
    {
        var JSON = await _IBom.GetByProdItemName(MainItemcode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpGet]
    //public IActionResult ExportBomDashboardToExcel()
    //{
    //    string modelJson = HttpContext.Session.GetString("KeyBOMList_Detail");

    //    List<BomDashboard> bomList = new();

    //    if (!string.IsNullOrEmpty(modelJson))
    //    {
    //        bomList = JsonConvert.DeserializeObject<List<BomDashboard>>(modelJson);
    //    }

    //    if (bomList == null || bomList.Count == 0)
    //        return NotFound("No data available to export.");

    //    using var workbook = new XLWorkbook();
    //    var worksheet = workbook.Worksheets.Add("BOM Dashboard");

    //    // ✅ Headers EXACTLY like table
    //    string[] headers = {"Sr#","BomNo","FG Part Code","FG Item Name","RM Part Code","RM Item Name","Bom Name","Bom ","EntryDate",
    //    "Effective Date","FG Item Code","Location"};

    //    // Write headers
    //    for (int i = 0; i < headers.Length; i++)
    //        worksheet.Cell(1, i + 1).Value = headers[i];

    //    int row = 2;
    //    int sr = 1;

    //    foreach (var item in bomList)
    //    {
    //        worksheet.Cell(row, 1).Value = sr++;
    //        worksheet.Cell(row, 2).Value = item.BomNo;
    //        worksheet.Cell(row, 3).Value = item.FGPartCode;
    //        worksheet.Cell(row, 4).Value = item.FGItem;
    //        worksheet.Cell(row, 5).Value = item.RMPartCode;
    //        worksheet.Cell(row, 6).Value = item.RMItemName;
    //        worksheet.Cell(row, 7).Value = item.BomName;
    //        worksheet.Cell(row, 8).Value = item.BomQty;
    //        worksheet.Cell(row, 9).Value = SafeDate(item.EntryDate);
    //        worksheet.Cell(row, 10).Value = SafeDate(item.EffectiveDate);
    //        worksheet.Cell(row, 11).Value = item.FGItemCode;
    //        worksheet.Cell(row, 12).Value = item.grade;

    //        row++;
    //    }

    //    worksheet.Columns().AdjustToContents();

    //    using var stream = new MemoryStream();
    //    workbook.SaveAs(stream);
    //    stream.Position = 0;

    //    return File(
    //        stream.ToArray(),
    //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //        "BomDashboardReport.xlsx"
    //    );
    //}
    public IActionResult ExportBomDashboardToExcel()
    {
        // Get Session Data
        string sessionData = HttpContext.Session.GetString("KeyBOMList_Detail");

        if (string.IsNullOrEmpty(sessionData))
        {
            return NotFound("No data available to export.");
        }

        // Deserialize dynamic rows
        var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(sessionData);

        if (rows == null || rows.Count == 0)
        {
            return NotFound("No data available to export.");
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("BOM Dashboard");

        // Dynamic Headers
        var headers = rows.First().Keys.ToList();

        // Write Headers
        for (int col = 0; col < headers.Count; col++)
        {
            worksheet.Cell(1, col + 1).Value = headers[col];
        }

        // Header Style
        var headerRange = worksheet.Range(1, 1, 1, headers.Count);

        headerRange.Style.Fill.BackgroundColor = XLColor.Yellow;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Freeze Header Row
        worksheet.SheetView.FreezeRows(1);

        // Write Data
        int rowIndex = 2;

        foreach (var row in rows)
        {
            for (int col = 0; col < headers.Count; col++)
            {
                var value = row[headers[col]];

                worksheet.Cell(rowIndex, col + 1).Value = value?.ToString();

                // Optional Date Formatting
                //if (value != null &&
                //    DateTime.TryParse(value.ToString(), out DateTime dt))
                //{
                //    worksheet.Cell(rowIndex, col + 1).Value = dt;
                //    worksheet.Cell(rowIndex, col + 1)
                //        .Style.DateFormat.Format = "dd-MMM-yyyy";
                //}
                if (value != null &&
    DateTime.TryParseExact(
        value.ToString(),
        new[] { "dd/MM/yyyy", "dd-MM-yyyy", "dd-MMM-yyyy" },
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None,
        out DateTime dt))
                {
                    worksheet.Cell(rowIndex, col + 1).Value = dt;

                    worksheet.Cell(rowIndex, col + 1)
                        .Style.DateFormat.Format = "dd-MMM-yyyy";
                }
            }

            rowIndex++;
        }

        // Auto Fit Columns
        worksheet.Columns().AdjustToContents();

        // Export
        using var stream = new MemoryStream();

        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "BomDashboardReport.xlsx"
        );
    }


    private string SafeDate(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        return input.Contains(" ") ? input.Split(" ")[0] : input;
    }

    public async Task<JsonResult> ChangeBomRMQtySameAsGrossWeight()
    {

        var JSON = await _IBom.ChangeBomRMQtySameAsGrossWeight();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> AllowChangeFGBomQty()
    {

        var JSON = await _IBom.AllowChangeFGBomQty();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpPost]
    public async Task<IActionResult> GetItemData(string FGPartCode, string FGItemName, string RMPartCode, string RMItemName,
int page = 1,
int pageSize = 5000,
string searchText = ""
)
    {
        try
        {
            BomModel model = await _IBom.GetItemData(FGPartCode, FGItemName, RMPartCode, RMItemName);

            if (model == null)
            {
                model = new BomModel();
            }

            if (model.ALLDATAForUpdate == null)
            {
                model.ALLDATAForUpdate = new List<BomViewModel>();
            }

            IQueryable<BomViewModel> query =
                model.ALLDATAForUpdate.AsQueryable();

            // GLOBAL SEARCH
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();

                query = query.Where(x =>

                    (!string.IsNullOrEmpty(x.FGPartCode) &&
                     x.FGPartCode.ToLower().Contains(searchText))

                    ||

                    (!string.IsNullOrEmpty(x.FGItemName) &&
                     x.FGItemName.ToLower().Contains(searchText))

                    ||

                    (!string.IsNullOrEmpty(x.RMPartCode) &&
                     x.RMPartCode.ToLower().Contains(searchText))

                    ||

                    (!string.IsNullOrEmpty(x.RMItemName) &&
                     x.RMItemName.ToLower().Contains(searchText))
                );
            }

            //// ITEM GROUP FILTER
            //if (ItemGroupId > 0)
            //{
            //    query = query.Where(x => x.ParentCode == ItemGroupId);
            //}

            int totalRecords = query.Count();

            int totalPages =
                (int)Math.Ceiling((double)totalRecords / pageSize);

            List<BomViewModel> pagedData = query
                .OrderBy(x => x.FGItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = pagedData
            };

            return Json(result);
        }
        catch (Exception ex)
        {
            return Json(new
            {
                StatusCode = 500,
                Message = ex.Message
            });
        }
    }


    [HttpPost]
    public async Task<JsonResult> SaveMiltiBomData([FromBody] List<BomViewModel> allData, string formKey, int oldItemCode, string ApplyType)
    {
        try
        {
            if (allData == null || allData.Count == 0)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "No data found to save."
                });
            }

            // ============================
            // SESSION VALUES
            // ============================
            int empId = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            int YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            string branch = HttpContext.Session.GetString($"Branch_{formKey}");

            // ============================
            // CONVERT LIST TO DATATABLE
            // ============================
            DataTable Table = GetDetailTable(allData, branch, empId, YearCode);

            if (Table.Rows.Count == 0)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "DataTable is empty."
                });
            }

            foreach (DataRow row in Table.Rows)
            {
                foreach (DataColumn col in Table.Columns)
                {
                    // CHECK ONLY BIGINT / INT TYPE COLUMNS
                    if (
                        col.DataType == typeof(long) ||
                        col.DataType == typeof(int) ||
                        col.DataType == typeof(Int64)
                    )
                    {
                        var value = row[col.ColumnName]?.ToString();

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            bool isValid = long.TryParse(value, out _);

                            if (!isValid)
                            {
                                return Json(new
                                {
                                    statusCode = 500,
                                    message = $"Invalid bigint value in Column : {col.ColumnName} , Value : {value}"
                                });
                            }
                        }
                    }
                }
            }
            // ============================
            // SAVE DATA
            // ============================
            ResponseResult result = await _IBom.UpdateMultipleBOMData(Table, oldItemCode, ApplyType);

            // ============================
            // RESPONSE
            // ============================
            if (result != null)
            {
                if (result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return Json(new
                    {
                        statusCode = 500,
                        message = result.StatusText
                    });
                }

                return Json(new
                {
                    statusCode = 200,
                    message = "BOM data saved successfully."
                });
            }

            return Json(new
            {
                statusCode = 500,
                message = "No response from database."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                statusCode = 500,
                message = ex.Message
            });
        }
    }

}