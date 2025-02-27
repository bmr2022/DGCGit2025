using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport;
using FastReport.Web;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;

namespace eTactWeb.Controllers
{
    public class ReqThroughBomController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReqThruBom _IReqThruBom;
        private readonly ILogger<ReqThroughBomController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public ReqThroughBomController(ILogger<ReqThroughBomController> logger, IDataLogic iDataLogic, IReqThruBom iReqThruBom, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IReqThruBom = iReqThruBom;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }


        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string Type = "")
        {
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();
            if (Type == "Detail")
            {
                webReport.Report.Load(webRootPath + "\\RequisitionThrBomDetail.frx"); // detail report
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\RequisitionThroughBOM.frx"); // summary report
            }
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\RequisitionThrBomDetail.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the HTML export
                using (HTMLExport html = new HTMLExport())
                {
                    using (FileStream st = new FileStream(webRootPath + "\\test.html", FileMode.Create))
                    {
                        webReport.Report.Export(html, st);
                        return File("App_Data/test.html", "application/octet-stream", "Test.html");
                    }
                }
            }
        }

        public IActionResult GetImage(int EntryId = 0, int YearCode = 0)
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\RequisitionThrBomDetail.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the Image export
                using (ImageExport image = new ImageExport())
                {
                    image.ImageFormat = ImageExportFormat.Jpeg;
                    image.JpegQuality = 100; // Set up the quality
                    image.Resolution = 100; // Set up a resolution 
                    image.SeparateFiles = false; // We need all pages in one big single file

                    using (MemoryStream st = new MemoryStream())// Using stream to save export
                    {
                        webReport.Report.Export(image, st);
                        return base.File(st.ToArray(), "image/jpeg");
                    }
                }
            }
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> ReqThroughBom()
        {
            ViewData["Title"] = "Requisition Through BOM Detail";
            TempData.Clear();
            _MemoryCache.Remove("KeyReqThroughBOMGrid");
            var MainModel = new RequisitionThroughBomModel();
            MainModel = await BindModel(MainModel);
            MainModel.Mode = "F";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyReqThroughBOMGrid", MainModel, cacheEntryOptions);
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReqThroughBom(RequisitionThroughBomModel model)
        {
            try
            {
                var ReqGrid = new DataTable();

                _MemoryCache.TryGetValue("KeyReqThroughBOMGrid", out List<RequisitionThruBomDetail> RequisitionDetail);

                if (RequisitionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReqThruBom", "ReqThruBom Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("ReqWithoutBom", model);
                }
                else
                {
                    //model.CreatedBy = Constants.UserID;
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    }
                    ReqGrid = GetDetailTable(RequisitionDetail);
                    var Result = await _IReqThruBom.SaveRequisition(model, ReqGrid);

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
                    var MainModel = new RequisitionThroughBomModel();
                    MainModel = await BindModel(MainModel);
                    _MemoryCache.Remove("KeyReqThroughBOMGrid");
                    return RedirectToAction(nameof(ReqThroughBom));
                }
            }
            catch (Exception ex)
            {
                LogException<ReqThroughBomController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IReqThruBom.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IReqThruBom.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private static DataTable GetDetailTable(IList<RequisitionThruBomDetail> DetailList)
        {
            try
            {
                var ReqGrid = new DataTable();

                ReqGrid.Columns.Add("EntryId", typeof(int));
                ReqGrid.Columns.Add("YearCode", typeof(int));
                ReqGrid.Columns.Add("Seqno", typeof(int));
                ReqGrid.Columns.Add("ItemCode", typeof(int));
                ReqGrid.Columns.Add("BomRevNo", typeof(int));
                ReqGrid.Columns.Add("BOMEffDate", typeof(DateTime));
                ReqGrid.Columns.Add("Unit", typeof(string));
                ReqGrid.Columns.Add("Qty", typeof(decimal));
                ReqGrid.Columns.Add("AltUnit", typeof(string));
                ReqGrid.Columns.Add("AltQty", typeof(decimal));
                ReqGrid.Columns.Add("ItemModel", typeof(string));
                ReqGrid.Columns.Add("ItemSize", typeof(string));
                ReqGrid.Columns.Add("ExpectedDate", typeof(DateTime));
                ReqGrid.Columns.Add("Remark", typeof(string));
                ReqGrid.Columns.Add("PendQty", typeof(decimal));
                ReqGrid.Columns.Add("PendAltQty", typeof(decimal));
                ReqGrid.Columns.Add("StoreId", typeof(int));
                ReqGrid.Columns.Add("TotalStock", typeof(decimal));
                ReqGrid.Columns.Add("Cancel", typeof(string));
                ReqGrid.Columns.Add("ProjectNo", typeof(string));
                ReqGrid.Columns.Add("ProjectYearCode", typeof(int));
                ReqGrid.Columns.Add("CostCenterId", typeof(int));
                ReqGrid.Columns.Add("ItemLocation", typeof(string));
                ReqGrid.Columns.Add("ItemBinRackNo", typeof(string));

                foreach (var Item in DetailList)
                {
                    ReqGrid.Rows.Add(
                        new object[]
                        {
                    Item.ID,
                    Item.YearCode,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.BomRevNo,
                    Item.BOMEffDate != null ? ParseFormattedDate(Item.BOMEffDate) : DateTime.Today,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.Qty,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.AltQty,
                    Item.ItemModel == null ? "" : Item.ItemModel,
                    Item.ItemSize == null ? "" : Item.ItemSize,
                    Item.ExpectedDate != null ? ParseFormattedDate(Item.ExpectedDate) : DateTime.Today,
                    Item.Remark == null ? "" : Item.Remark,
                    Item.PendQty,
                    Item.PendAltQty,
                    Item.StoreId,
                    Item.TotalStock,
                    Item.Cancel == null ? "" : Item.Cancel,
                    Item.ProjectNo== null ? "" : Item.ProjectNo,
                    Item.ProjectYearCode,
                    Item.CostCenterId,
                    Item.ItemLocation== null ? "" : Item.ItemLocation,
                    Item.ItemBinRackNo == null ? "" : Item.ItemBinRackNo


                        });
                }
                ReqGrid.Dispose();
                return ReqGrid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddReqThruBomDetail(RequisitionThruBomDetail model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyReqThroughBOMGrid", out IList<RequisitionThruBomDetail> GridDetail);

                var MainModel = new RequisitionThroughBomModel();
                var ReqThruBOMGrid = new List<RequisitionThruBomDetail>();
                var ReqGrid = new List<RequisitionThruBomDetail>();
                var SSGrid = new List<RequisitionThruBomDetail>();

                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        model.SeqNo = 1;
                        if (model.CostCenterName == "-Select-")
                        {
                            model.CostCenterName = "NA";
                        }
                        if (model.StoreName == "-Select-")
                        {
                            model.StoreName = "NA";
                        }
                        ReqGrid.Add(model);
                    }
                    else
                    {
                        if (GridDetail.Where(x => x.ItemCode == model.ItemCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SeqNo = GridDetail.Count + 1;
                            if (model.CostCenterName == "-Select-")
                            {
                                model.CostCenterName = "NA";
                            }
                            if (model.StoreName == "-Select-")
                            {
                                model.StoreName = "NA";
                            }
                            ReqGrid = GridDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(ReqGrid);
                            ReqGrid.Add(model);
                        }
                    }

                    MainModel.ReqDetailGrid = ReqGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyReqThroughBOMGrid", MainModel.ReqDetailGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_ReqThruBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> Dashboard(string FromDate, string ToDate, string Flag, string REQNo = "", string ItemName = "", string PartCode = "", string WCName = "", string WONo = "", string DepName = "", string DBType = "", string searchbox = "")
        {
            try
            {
                _MemoryCache.Remove("KeyReqThroughBOMGrid");
                var model = new ReqThruMainDashboard();
                model.CC = HttpContext.Session.GetString("Branch");
                var FromDt = ParseFormattedDate(FromDate.Split(" ")[0]);
                var ToDt = ParseFormattedDate(ToDate.Split(" ")[0]);
                var Result = await _IReqThruBom.GetDashboardData(FromDt, ToDt, Flag).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "REQNo", "ReqDate", "EntryDate", "WorkCenter", "WONO",
                            "BranchName", "DeptName", "Reason", "Cancel", "MachName", "WOYearcode", "EntryId", "YearCode", "TotalReqQty", "TotalPendQty", "Completed");
                        model.ReqMainDashboard = CommonFunc.DataTableToList<RTBDashboard>(DT, "ReqThruDashboard");
                    }
                }
                if (Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = ToDate;
                    model.REQNo= REQNo;
                    model.ItemName= ItemName;
                    model.PartCode= PartCode;
                    model.WorkCenter= WCName;
                    model.WONo=WONo;
                    model.DeptName= DepName;
                    model.DashboardType=DBType;
                    model.GlobalSearch=searchbox;
                }
                return View(model);
                //else
                //    return PartialView("_ReqThruBomDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteItemRow(int SeqNo)
        {
            var MainModel = new RequisitionThroughBomModel();
            _MemoryCache.TryGetValue("KeyReqThroughBOMGrid", out List<RequisitionThruBomDetail> RequisitionDetail);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (RequisitionDetail != null && RequisitionDetail.Count > 0)
            {
                RequisitionDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in RequisitionDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ReqDetailGrid = RequisitionDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (RequisitionDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyReqThroughBOMGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_ReqThruBomGrid", MainModel);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate = "", string ToDate = "", string REQNo = "", string ItemName = "", string PartCode = "", string WCName = "", string WONo = "", string DepName = "", string DBType = "", string searchbox = "")
        {
            var Result = await _IReqThruBom.DeleteByID(ID, YC);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");


            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, REQNo = REQNo, ItemName = ItemName, PartCode = PartCode, WCName = WCName, WONo = WONo, DepName = DepName, DBType = DBType, searchbox = searchbox, Flag = "False" });
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ReqThroughBom(int ID, string Mode, int YC, string ReqNo = "", string ItemName = "", string Partcode = "", string WorkCenter = "", string WONO = "", string DeptName = "", string FromDate = "", string ToDate = "", string Type = "", string GlobalSearch = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new RequisitionThroughBomModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            _MemoryCache.Remove("KeyReqThroughBOMGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IReqThruBom.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode=YC;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyReqThroughBOMGrid", MainModel.ReqDetailGrid, cacheEntryOptions);
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                //MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                // MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                // MainModel.UpdatedOn = DateTime.Now;
            }

            MainModel.REQNoBack = ReqNo;
            MainModel.ItemNameBack = ItemName;
            MainModel.PartCodeBack = Partcode;
            MainModel.WorkCenterIdBack = WorkCenter;
            MainModel.WONoBack = WONO;
            MainModel.DeptNameBack = DeptName;
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.TypeBack = Type;
            MainModel.GlobalSearchBack = GlobalSearch;

            return View(MainModel);
        }
        private async Task<RequisitionThroughBomModel> BindModel(RequisitionThroughBomModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IReqThruBom.BindAllDropDowns("BINDALLDROPDOWN");
            model.BranchList = new List<TextValue>();
            model.ProjectList = new List<TextValue>();
            model.CostCenterList = new List<TextValue>();
            model.DepartmentList = new List<TextValue>();
            model.EmployeeList = new List<TextValue>();
            model.MachineList = new List<TextValue>();
            model.StoreList = new List<TextValue>();
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
                        Value = row["projectname"].ToString(),
                        Text = row["projectname"].ToString()
                    });
                }
                model.ProjectList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DepartmentList = _List;

                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["CostCenterName"].ToString()
                    });
                }
                model.CostCenterList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()
                    });
                }
                model.EmployeeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["machinename"].ToString()
                    });
                }
                model.MachineList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[6].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["Store_Name"].ToString()
                    });
                }
                model.StoreList = _List;
            }
            return model;
        }
        public async Task<IActionResult> GetSearchData(string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RTBDashboard();
            model = await _IReqThruBom.GetDashboardData(REQNo, WCName, WONo, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
            return PartialView("_ReqThruBomDashboardGrid", model);
        }
        public async Task<JsonResult> FillItems()
        {
            var JSON = await _IReqThruBom.FillItems();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkOrder()
        {
            var JSON = await _IReqThruBom.FillWorkOrder();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IReqThruBom.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTotalStock(int ItemCode, int Store)
        {
            var JSON = await _IReqThruBom.FillTotalStock(ItemCode, Store);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomRevNo(int ItemCode)
        {
            var JSON = await _IReqThruBom.GetBomRevNo(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProjectNo()
        {
            var JSON = await _IReqThruBom.GetProjectNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPopUpData(int ItemCode, int BomNo)
        {
            var JSON = await _IReqThruBom.GetPopUpData(ItemCode, BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetNewEntry(int YearCode)
        {
            var JSON = await _IReqThruBom.GetNewEntry("GETNEWENTRY", YearCode, "SP_RequisitionThrBOM");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            var model = new RequisitionThroughBomModel();
            _MemoryCache.TryGetValue("KeyReqThroughBOMGrid", out List<RequisitionThruBomDetail> RequisitionDetail);
            string JsonString = "";
            if (RequisitionDetail != null)
            {
                var SSGrid = RequisitionDetail.Where(x => x.SeqNo == SeqNo);
                JsonString = JsonConvert.SerializeObject(SSGrid);
            }
            return Json(JsonString);
        }

        public async Task<IActionResult> GetDetailData(string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RTBDashboard();
            model = await _IReqThruBom.GetDetailData(REQNo, WCName, WONo, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
            model.Mode = "Detail";
            return PartialView("_ReqThruBomDashboardGrid", model);
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IReqThruBom.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
