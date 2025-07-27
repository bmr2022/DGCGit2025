using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Composition;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using Microsoft.Extensions.Configuration;
using static Grpc.Core.Metadata;
using System.Net;
using System.Globalization;
using System.Data;
using System.Configuration;

namespace eTactWeb.Controllers
{
    public class ReqWithoutBomController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReqWithoutBOM _IReqWithoutBOM;
        private readonly ILogger<ReqWithoutBomController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;

        public ReqWithoutBomController(ILogger<ReqWithoutBomController> logger, IDataLogic iDataLogic, IReqWithoutBOM iReqWithoutBOM, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IReqWithoutBOM = iReqWithoutBOM;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
        }

        public IActionResult PrintReport(int EntryId, int YearCode = 0, string Type = "")
        {
            try
            {
                string contentRootPath = _IWebHostEnvironment.ContentRootPath;
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();
               // string reportPath = Path.Combine(webRootPath, "ReqWithoutBom.frx");
                string reportPath = Path.Combine(webRootPath, "ReqWithoutBOMF.frx");
                webReport.Report.Load(reportPath);
                string my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
                webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
                webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.SetParameterValue("MyParameter", my_connection_string);
                webReport.Report.Refresh();
                return View(webReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Report generation failed: {ex.Message}");
            }
        }

        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\requisitionWithoutBOM.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("EntryId", EntryId);
                webReport.Report.SetParameterValue("YearCode", YearCode);
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


                webReport.Report.Load(webRootPath + "\\requisitionWithoutBOM.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("EntryId", EntryId);
                webReport.Report.SetParameterValue("YearCode", YearCode);
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
        public async Task<IActionResult> ReqWithoutBom()
        {
            ViewData["Title"] = "Requisition Without BOM Detail";
            TempData.Clear();
            HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
            var MainModel = new RequisitionWithoutBOMModel();
           

            MainModel = await BindModel(MainModel);
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.Mode = "F";
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyReqWithoutBOMGrid", serializedGrid);
            return View(MainModel);
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ReqWithoutBom(int ID, string Mode, int YC, string REQNo = "",string ItemName = "",string PartCode = "", string WorkCenter = "", string WONo = "", string DeptName = "", string DashboardType = "", string FromDate = "", string ToDate = "", string GlobalSearch = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new RequisitionWithoutBOMModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")); 
            HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IReqWithoutBOM.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ReqDetailGrid);
                HttpContext.Session.SetString("KeyReqWithoutBOMGrid", serializedGrid);
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

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.REQNoBack = REQNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.WorkCenterBack = WorkCenter;
            MainModel.WorkOrderNoback = WONo;
            MainModel.DeptNameBack = DeptName;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = GlobalSearch;
            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReqWithoutBom(RequisitionWithoutBOMModel model, string ShouldPrint)
        {
            try
            {
                var ReqGrid = new DataTable();
                var mainmodel2 = model;
                string modelJson = HttpContext.Session.GetString("KeyReqWithoutBOMGrid");
                List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
                }

                mainmodel2.ReqDetailGrid = RequisitionDetail;
                if (RequisitionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReqWithoutBom", "ReqWithoutBom Grid Should Have Atleast 1 Item...!");
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
                    ReqGrid = GetDetailTable(RequisitionDetail,model.Mode);
                    var Result = await _IReqWithoutBOM.SaveRequisition(model, ReqGrid);

                    if (Result != null)
                    {
                        if ((Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)||(Result.StatusText == "Completed is Y" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            if (ShouldPrint == "true")
                            {
                                return RedirectToAction("PrintReport", new { EntryId = model.EntryId, YearCode = model.YearCode });
                            }
                            var MainModel = new RequisitionWithoutBOMModel();
                            MainModel = await BindModel(MainModel);
                            HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
                            return RedirectToAction(nameof(ReqWithoutBom));
                        }
                        if ((Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted) || (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            if (ShouldPrint == "true")
                            {
                                return RedirectToAction("PrintReport", new { EntryId = model.EntryId, YearCode = model.YearCode });
                            }
                            var MainModel = new RequisitionWithoutBOMModel();
                            MainModel = await BindModel(MainModel);
                            HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
                            return RedirectToAction(nameof(ReqWithoutBom));
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //return View("Error", Result);
                        }
                    }
                    mainmodel2 = await BindModel(mainmodel2);
                    return View(mainmodel2);
                }
            }
            catch (Exception ex)
            {
                LogException<ReqWithoutBomController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> AutoFillPartCode(string TF, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IReqWithoutBOM.AutoFillitem("AutoFillPartCode", TF, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillItemName(string TF, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IReqWithoutBOM.AutoFillitem("AutoFillItemName", TF, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IReqWithoutBOM.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProjectNo()
        {
            var JSON = await _IReqWithoutBOM.GetProjectNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> Dashboard(string FromDate, string Todate, string Flag, string REQNo = "", string WCName = "", string WONo="", string DepName="", string PartCode="", string ItemName="")
        {
            try
            {
                HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
                var model = new ReqMainDashboard();
                model.Mode = "Summary";
                model.CC = HttpContext.Session.GetString("Branch");
                var Result = await _IReqWithoutBOM.GetDashboardData(FromDate, Todate, Flag).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "REQNo", "ReqDate", "EntryDate", "WorkCenter", "WONO",
                           "BranchName", "Reason", "Cancel", "MachName", "WOYearcode", "EntryId","YearCode","TotalReqQty","TotalPendQty","Completed", "CreatedByName", "UpdatedByName");
                        model.ReqMainDashboard = CommonFunc.DataTableToList<RWBDashboard>(DT, "ReqDashboard");
                    }
                }
               if(Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;
                    model.REQNo = REQNo;
                    model.WorkCenter = WCName;
                    model.WONo = WONo;
                    model.PartCode = PartCode;
                    model.ItemName = ItemName;
                }
               // if (Flag == "True")
                    return View(model);
                //else
                //{
                //    return PartialView("_ReqWithoutBomDashboardGrid", model);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC,string FromDate, string ToDate, string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName)
        {
            var Result = await _IReqWithoutBOM.DeleteByID(ID, YC);
            var CC = HttpContext.Session.GetString("Branch");
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

            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate,ToDate = formattedToDate,Flag = "False", REQNo=REQNo, WCName=WCName, WONo=WONo, DepName=DepName, PartCode=PartCode, ItemName=ItemName, BranchName=CC});
        }
        private static DataTable GetDetailTable(IList<RequisitionDetail> DetailList, string mode)
        {
            var ReqGrid = new DataTable();

            ReqGrid.Columns.Add("EntryId", typeof(int));
            ReqGrid.Columns.Add("YearCode", typeof(int));
            ReqGrid.Columns.Add("Seqno", typeof(int));
            ReqGrid.Columns.Add("ItemCode", typeof(int));
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
                DateTime expDt = new DateTime();
                if(mode != "U")
                {
                if (Item.ExpectedDate != null)
                    expDt = DateTime.ParseExact(Item.ExpectedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                ReqGrid.Rows.Add(
                    new object[]
                    {
                    1,
                    2023,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.Unit,
                    Item.Qty,
                    Item.AltUnit,
                    Item.AltQty,
                    Item.ItemModel,
                    Item.ItemSize,
                    mode=="U"? Item.ExpectedDate: expDt.ToString("yyyy/MM/dd"),
                    Item.Remark,
                    Item.PendQty,
                    Item.PendAltQty,
                    Item.StoreId ?? 0,
                    Item.TotalStock,
                    Item.Cancle,
                    Item.ProjectNo,
                    Item.ProjectYearCode,
                    Item.CostCenterId,
                    Item.ItemLocation,
                    Item.ItemBinRackNo
                    });
            }
            ReqGrid.Dispose();
            return ReqGrid;
        }
        public async Task<IActionResult> DeleteItemRow(int SeqNo)
        {
            var MainModel = new RequisitionWithoutBOMModel();
            string modelJson = HttpContext.Session.GetString("KeyReqWithoutBOMGrid");
            List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
            }

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
                    HttpContext.Session.Remove("KeyReqWithoutBOMGrid");
                }
                HttpContext.Session.SetString("KeyReqWithoutBOMGrid", JsonConvert.SerializeObject(RequisitionDetail));
            }
            return PartialView("_ReqWithoutBomGrid", MainModel);
        }
        public IActionResult AddReqWithoutBomDetail(RequisitionDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyReqWithoutBOMGrid");
                List<RequisitionDetail> GridDetail = new List<RequisitionDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
                }

                var MainModel = new RequisitionWithoutBOMModel();
                var ReqWithoutBOMGrid = new List<RequisitionDetail>();
                var ReqGrid = new List<RequisitionDetail>();
                var SSGrid = new List<RequisitionDetail>();

                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        model.SeqNo = 1;
                        if (model.CostCenterName == "-Select-")
                        {
                            model.CostCenterName = "NA";
                        }if (model.StoreName == "-Select-")
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
                            if (model.CostCenterName == "-Select-")
                            {
                                model.CostCenterName = "NA";
                            }
                            if (model.StoreName == "-Select-")
                            {
                                model.StoreName = "NA";
                            }
                            model.SeqNo = GridDetail.Count + 1;
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
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReqDetailGrid);
                    HttpContext.Session.SetString("KeyReqWithoutBOMGrid", serializedGrid);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_ReqWithoutBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<RequisitionWithoutBOMModel> BindModel(RequisitionWithoutBOMModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IReqWithoutBOM.BindAllDropDowns("BINDALLDROPDOWN");

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
                        Text = row["yearcode"].ToString()
                    });
                }
                model.ProjectList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryId"].ToString(),
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
                    if (string.IsNullOrEmpty(model.Mode) || (model.Mode != "U" && model.Mode != "V"))
                        if (row["Store_Type"]?.ToString() == "MAIN STORE")
                    {
                       
                        model.StoreId = Convert.ToInt32(row["EntryID"]);
                    }
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
        public async Task<JsonResult> FillItems(string TF)
        {
            var JSON = await _IReqWithoutBOM.FillItems(TF);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDept()
        {
            var JSON = await _IReqWithoutBOM.FillDept();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string TF)
        {
            var JSON = await _IReqWithoutBOM.FillPartCode(TF);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkOrder()
        {
            var JSON = await _IReqWithoutBOM.FillWorkOrder();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IReqWithoutBOM.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IReqWithoutBOM.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTotalStock(int ItemCode, int Store)
        {
            var JSON = await _IReqWithoutBOM.FillTotalStock(ItemCode, Store);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSearchData(string REQNo,string WCName,string WONo,string DepName,string PartCode,string ItemName,string BranchName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RWBDashboard();
            model = await _IReqWithoutBOM.GetDashboardData(REQNo,WCName,WONo, DepName, PartCode, ItemName,BranchName, FromDate, ToDate);
            model.Mode = "Summary";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.ReqMainDashboard);
            HttpContext.Session.SetString("KeyRWBList", serializedGrid);
            return PartialView("_ReqWithoutBomDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string REQNo,string WCName,string WONo,string DepName,string PartCode,string ItemName,string BranchName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RWBDashboard();
            model = await _IReqWithoutBOM.GetDetailData(REQNo,WCName,WONo, DepName, PartCode, ItemName,BranchName, FromDate, ToDate);
            model.Mode = "Detail";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.ReqMainDashboard);
            HttpContext.Session.SetString("KeyRWBList", serializedGrid);
            return PartialView("_ReqWithoutBomDashboardGrid", model);
        }
        public async Task<JsonResult> GetNewEntry(int YearCode)
        {
            var JSON = await _IReqWithoutBOM.GetNewEntry("NewEntryId", YearCode, "SP_RequisitionWithoutBOM");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var JSON = await _IReqWithoutBOM.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            var model = new RequisitionWithoutBOMModel();
            string modelJson = HttpContext.Session.GetString("KeyReqWithoutBOMGrid");
            List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
            }
            
            var SSGrid = RequisitionDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        [HttpGet]
        public IActionResult GetReqWithoutBomDashBoardGridData()
        {
            string modelJson = HttpContext.Session.GetString("KeyRWBList");
            List<RWBDashboard> stockRegisterList = new List<RWBDashboard>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<RWBDashboard>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
