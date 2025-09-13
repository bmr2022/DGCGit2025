using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
//using static Grpc.Core.Metadata;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Web;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Runtime.Caching;
using DocumentFormat.OpenXml.Bibliography;
using System.Drawing.Printing;

namespace eTactWeb.Controllers
{
    public class IssueWithoutBomController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IIssueWithoutBom _IIssueWOBOM;
        private readonly ILogger<IssueWithoutBomController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        private readonly ConnectionStringService _connectionStringService;
        public IssueWithoutBomController(ILogger<IssueWithoutBomController> logger, IConfiguration iconfiguration, IDataLogic iDataLogic, IIssueWithoutBom IIssueWOBOM, IWebHostEnvironment iWebHostEnvironment, IMemoryCache iMemoryCache, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueWOBOM = IIssueWOBOM;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
            _connectionStringService = connectionStringService;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueWithoutBom()
        {
            ViewData["Title"] = "Issue Without BOM Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyIssWOBomGrid");
            var MainModel = new IssueWithoutBom();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyIssWOBomGrid", serializedGrid);
            return View(MainModel);
        }
      
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IIssueWOBOM.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> IssueWithoutBom(int ID, string Mode, int YC, string REQNo = "", string ItemName = "", string PartCode = "", string WorkCenter = "", string DeptName = "", string DashboardType = "", string FromDate = "", string ToDate = "", string GlobalSearch = "",int FromStoreBack=0)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new IssueWithoutBom();
            MainModel = await BindModel(MainModel);
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.MachineCode = HttpContext.Session.GetString("MachineName");

            HttpContext.Session.Remove("KeyIssWOBomGrid");
            HttpContext.Session.Remove("KeyIssWOBomScannedGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IIssueWOBOM.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel);

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssWOBomGrid", serializedGrid);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntrydate = DateTime.Now;
                MainModel.IssuedByEmpCode = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.IssuedByEmpName = HttpContext.Session.GetString("EmpName");
                MainModel.RecByEmpCode = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.RecDepCode = Convert.ToInt32(HttpContext.Session.GetString("DeptId"));
                MainModel.RecDept = HttpContext.Session.GetString("DeptName");
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
            MainModel.DeptNameBack = DeptName;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = GlobalSearch;
            MainModel.FromStoreBack = FromStoreBack;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueWithoutBom(IssueWithoutBom model, string ShouldPrint)
        {
            try
            {
                var MRGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyIssWOBomGrid");
                List<IssueWithoutBomDetail> IssueGrid = new List<IssueWithoutBomDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IssueGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
                }

                if (IssueGrid == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("Issue Without BOM", "Issue Without BOM Grid Should Have At least 1 Item...!");                  
                    return View("IssueWithoutBom", model);
                }
                else
                {
                    var userID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    model.CreatedBy = userID;
                    if (model.Mode == "U")
                        model.LastupdatedBy = userID;

                    MRGrid = GetDetailTable(IssueGrid);

                    var Result = await _IIssueWOBOM.SaveIssueWithoutBom(model, MRGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }
                            HttpContext.Session.Remove("KeyIssWOBomGrid");
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("KeyIssWOBomGrid");
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                    //return RedirectToAction("PendingRequisitionToIssue", "PendingRequisitionToIssue");
                    return Json(new { status = "Success" });
                }
            }
            catch (Exception ex)
            {
                LogException<IssueWithoutBomController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\IssWithOutBOM.frx"); // default report
            my_connection_string = _connectionStringService.GetConnectionString();
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);


        }
        public IActionResult FillGridFromMemoryCache()
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyIssWOBom");
                List<IssueWithoutBomDetail> IssueWithoutBomDetailGrid = new List<IssueWithoutBomDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
                }

                var MainModel = new IssueWithoutBom();
                var IssueGrid = new List<IssueWithoutBomDetail>();
                var SSGrid = new List<IssueWithoutBomDetail>();
                MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                MainModel.ToDate = HttpContext.Session.GetString("ToDate");

                var seqNo = 1;
                if (IssueWithoutBomDetailGrid != null)
                {
                    for (int i = 0; i < IssueWithoutBomDetailGrid.Count; i++)
                    {
                        

                        if (IssueWithoutBomDetailGrid[i] != null)
                        {
                            IssueWithoutBomDetailGrid[i].seqno = seqNo++;
                            SSGrid.AddRange(IssueGrid);
                            IssueGrid.Add(IssueWithoutBomDetailGrid[i]);

                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssWOBom", serializedGrid);
                        }
                    }
                }

                return PartialView("_IssueWOMainBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<IssueWithoutBom> BindModel(IssueWithoutBom model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IIssueWOBOM.FillEmployee("BINDRecByEmployee");

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()

                    });
                }
                model.EmployeeList = _List;
                _List = new List<TextValue>();
            }
            return model;
        }
        public async Task<JsonResult> FillBranch()
        {
            var JSON = await _IIssueWOBOM.FillBranch();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddtoIssueWOBomGrid(List<IssueWithoutBomDetail> model)
        {
            try
            {
                var MainModel = new IssueWithoutBom();
                var IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
                var IssueGrid = new List<IssueWithoutBomDetail>();
                var SSGrid = new List<IssueWithoutBomDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {

                        var isStockable = _IIssueWOBOM.GetIsStockable(item.ItemCode);
                        var stockable = isStockable.Result.Result.Rows[0].ItemArray[0];
                        string modelJson = HttpContext.Session.GetString("KeyIssWOBomGrid");
                        List<IssueWithoutBomDetail> IssueWithoutBomDetailGrid = new List<IssueWithoutBomDetail>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
                        }
                        if (item != null)
                        {
                            if (IssueWithoutBomDetailGrid == null)
                            {
                                if (stockable == "Y")
                                {
                                    if (item.LotStock <= 0 || item.TotalStock <= 0)
                                    {
                                        return StatusCode(203, "Stock can't be zero");
                                    }
                                }
                                item.seqno += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (stockable == "Y")
                                {
                                    if (item.LotStock <= 0 || item.TotalStock <= 0)
                                    {
                                        return StatusCode(203, "Stock can't be zero");
                                    }
                                }
                                if (IssueWithoutBomDetailGrid.Where(x => x.ItemCode == item.ItemCode && x.BatchNo == item.BatchNo && x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    item.seqno = IssueWithoutBomDetailGrid.Count + 1;
                                    IssueGrid = IssueWithoutBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssWOBomGrid", serializedGrid);
                        }
                    }
                }
                HttpContext.Session.Remove("KeyIssWOBom");
                return PartialView("_IssueWithoutBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new IssueWithoutBom();
            string modelJson = HttpContext.Session.GetString("KeyIssWOBomGrid");
            List<IssueWithoutBomDetail> IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueWithoutBomGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueWithoutBomGrid != null && IssueWithoutBomGrid.Count > 0)
            {
                IssueWithoutBomGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueWithoutBomGrid)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueWithoutBomGrid;

                if (IssueWithoutBomGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyIssWOBomGrid");
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssWOBomGrid", serializedGrid);
            }
            return PartialView("_IssueWithoutBomGrid", MainModel);
        }
        [HttpPost]
        public IActionResult DeleteFromZeroStockMemoryGrid(bool deleteZeroStockOnly, int? seqNo = null)
        {
            var MainModel = new IssueWithoutBom();
            string modelJson = HttpContext.Session.GetString("KeyIssWOBom");
            List<IssueWithoutBomDetail> IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueWithoutBomGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
            }

            if (deleteZeroStockOnly)
            {
                var deletedPartCodes = new List<string>();
                IssueWithoutBomGrid.RemoveAll(x =>
                {
                    bool toDelete = (x.BatchNo == "" || x.BatchNo == null);

                    if (toDelete)
                        deletedPartCodes.Add(x.PartCode);
                    return toDelete;
                });

                ViewBag.DeletedPartCodes = string.Join(", ", deletedPartCodes);
            }
            else if (seqNo != null)
            {
                var itemToRemove = IssueWithoutBomGrid.FirstOrDefault(x => x.seqno == seqNo);
                if (itemToRemove != null)
                {
                    IssueWithoutBomGrid.Remove(itemToRemove);
                }
            }

            int newSeq = 1;
            foreach (var item in IssueWithoutBomGrid)
            {
                item.seqno = newSeq++;
            }


            MainModel.ItemDetailGrid = IssueWithoutBomGrid;


            if (IssueWithoutBomGrid.Count == 0)
            {
                HttpContext.Session.Remove("KeyIssWOBom");
            }
            else
            {
                string updatedJson = JsonConvert.SerializeObject(IssueWithoutBomGrid);
                HttpContext.Session.SetString("KeyIssWOBom", updatedJson);
            }

            return PartialView("_IssueWOMainBomGrid", MainModel);
        }

        public IActionResult DeleteFromMemoryGrid(int SeqNo)
        {
            var MainModel = new IssueWithoutBom();
            string modelJson = HttpContext.Session.GetString("KeyIssWOBom");
            List<IssueWithoutBomDetail> IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueWithoutBomGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueWithoutBomGrid != null && IssueWithoutBomGrid.Count > 0)
            {
                IssueWithoutBomGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueWithoutBomGrid)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueWithoutBomGrid;

                if (IssueWithoutBomGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyIssWOBom");
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssWOBom", serializedGrid);
            }
            return PartialView("_IssueWOMainBomGrid", MainModel);
        }
        public IActionResult DeleteScannedItemRow(int SeqNo)
        {
            var MainModel = new IssueWithoutBom();
            string modelJson = HttpContext.Session.GetString("KeyIssWOBomScannedGrid");
            List<IssueWithoutBomDetail> IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueWithoutBomGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueWithoutBomGrid != null && IssueWithoutBomGrid.Count > 0)
            {
                IssueWithoutBomGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueWithoutBomGrid)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueWithoutBomGrid;

                if (IssueWithoutBomGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyIssWOBomScannedGrid");
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssWOBomScannedGrid", serializedGrid);
            }
            return PartialView("_IssueByScanningGrid", MainModel);
        }
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
        }
        private static DataTable GetDetailTable(IList<IssueWithoutBomDetail> DetailList)
        {
            try
            {
                var MRGrid = new DataTable();

                MRGrid.Columns.Add("EntryId", typeof(int));
                MRGrid.Columns.Add("YearCode", typeof(int));
                MRGrid.Columns.Add("seqno", typeof(int));
                MRGrid.Columns.Add("ItemCode", typeof(int));
                MRGrid.Columns.Add("ReqQty", typeof(decimal));
                MRGrid.Columns.Add("AltReqQty", typeof(decimal));
                MRGrid.Columns.Add("StoreId", typeof(int));
                MRGrid.Columns.Add("BatchNo", typeof(string));
                MRGrid.Columns.Add("uniqueBatchNo", typeof(string));
                MRGrid.Columns.Add("IssueQty", typeof(decimal));
                MRGrid.Columns.Add("AltIssueQty", typeof(decimal));
                MRGrid.Columns.Add("Unit", typeof(string));
                MRGrid.Columns.Add("AltUnit", typeof(string));
                MRGrid.Columns.Add("LotStock", typeof(decimal));
                MRGrid.Columns.Add("TotalStock", typeof(decimal));
                MRGrid.Columns.Add("AltQty", typeof(decimal));
                MRGrid.Columns.Add("Rate", typeof(decimal));
                MRGrid.Columns.Add("WCId", typeof(int));
                MRGrid.Columns.Add("IssuedAlternateItem", typeof(string));
                MRGrid.Columns.Add("OriginalitemCode", typeof(int));
                MRGrid.Columns.Add("AltItemCode", typeof(int));
                MRGrid.Columns.Add("Remark", typeof(string));
                MRGrid.Columns.Add("CostCenterId", typeof(int));
                MRGrid.Columns.Add("ItemSize", typeof(string));
                MRGrid.Columns.Add("ItemColor", typeof(string));
                MRGrid.Columns.Add("ProjectNo", typeof(string));
                MRGrid.Columns.Add("ProjectYearcode", typeof(int));
                MRGrid.Columns.Add("StdPacking", typeof(float));
                MRGrid.Columns.Add("ReqNo", typeof(string));
                MRGrid.Columns.Add("ReqYearCode", typeof(string));
                MRGrid.Columns.Add("ReqDate", typeof(string));
                MRGrid.Columns.Add("ReqEntryId", typeof(int));
                MRGrid.Columns.Add("CancelReq", typeof(string));

                foreach (var Item in DetailList)
                {
                    //DateTime ReqDate = new DateTime();
                    //ReqDate = ParseDate(Item.ReqDate);
                    if (Item.AltUnit == "null")
                        Item.AltUnit = "";
                    if (Item.uniqueBatchNo == "null")
                        Item.uniqueBatchNo = "";
                    if (Item.BatchNo == "null")
                        Item.BatchNo = "";
                    MRGrid.Rows.Add(
                        new object[]
                        {
                    Item.EntryId == 0 ? 0:Item.EntryId,
                    Item.YearCode == 0 ? 0:Item.YearCode,
                    Item.seqno == 0 ? 0 : Item.seqno,
                    Item.ItemCode==0?0:Item.ItemCode,
                    Item.ReqQty == 0 ? 0 : Item.ReqQty,
                    Item.ReqQty == 0 ? 0 : Item.ReqQty, // altrecqty
                    Item.StoreId == 0?0:Item.StoreId,
                    Item.BatchNo == null ? "" : Item.BatchNo,
                    Item.uniqueBatchNo == null ? "" : Item.uniqueBatchNo,
                    Item.IssueQty == 0 ? 0 : Item.IssueQty,
                    Item.IssueQty == 0 ? 0 : Item.IssueQty, // altissueqty
                    Item.Unit == null? "":Item.Unit,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.LotStock == 0 ? 0 : Item.LotStock,
                    Item.TotalStock == 0? 0:Item.TotalStock,
                    Item.AltQty == 0 ? 0 : Item.AltQty,
                    Item.Rate == 0? 0:Item.Rate,
                    Item.WCId == 0 ? 0 : Item.WCId,
                    Item.IssuedAlternateItem == null? "":Item.IssuedAlternateItem,//issuedalternateitem
                    Item.OriginalItemCode == 0 ? 0 : Item.OriginalItemCode,//originalitemcode
                    Item.AltItemCode == 0? 0 : Item.AltItemCode,
                    Item.Remark == null ? "" : Item.Remark,
                    Item.CostCenterId == 0 ? 0 : Item.CostCenterId,
                    Item.ItemSize == null ? "" : Item.ItemSize,
                    Item.ItemColor == null?"":Item.ItemColor,
                    Item.ProjectNo == null ? "" : Item.ProjectNo,
                    Item.ProjectYearCode == 0 ? 0 : Item.ProjectYearCode,
                    Item.StdPacking == 0 ? 0:Item.StdPacking,
                    Item.ReqNo1 == null ? "" : Item.ReqNo1,
                    Item.ReqyearCode1== null?"":Item.ReqyearCode1,
                    Item.ReqDate1 == null ? "" : Item.ReqDate1,
                    Item.ReqEntryId == 0?0:Item.ReqEntryId,
                     Item.ReqItemCancel == null ? "" : Item.ReqItemCancel,
                        });
                }

                MRGrid.Dispose();
                return MRGrid;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IActionResult> Dashboard(string FromDate="", string Todate="", string Flag="", string REQNo = "", string WCName = "", string WONo = "", string DepName = "", string PartCode = "", string ItemName = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyIssWOBomGrid");
                var model = new IssueWithoutBomDashboard();
                var Result = await _IIssueWOBOM.GetDashboardData(FromDate, Todate, Flag).ConfigureAwait(true);

                //if (Result != null)
                //{
                //    var _List = new List<TextValue>();
                //    DataSet DS = Result.Result;
                //    if (DS != null)
                //    {
                //        var DT = DS.Tables[0].DefaultView.ToTable(false, "ReqNo", "ReqDate", "IssueSlipNo", "IssueDate", "ActualEnteredBy", "MachineCode" ,"Fromdepartment", "RecDepartment","ReqYearCode", "Item_Name", "PartCode", "EntryId", "YearCode", "WorkCenterDescription");
                //        model.IssueWOBOMDashboard = CommonFunc.DataTableToList<IssueWOBomMainDashboard>(DT, "IssueWODashboard");
                //    }
                //}

                if (Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;
                    model.ReqNo = REQNo;
                    model.WorkCenterDescription = WCName;
                    model.PartCode = PartCode;
                    model.Item_Name = ItemName;
                }
                    return View(model);

                //return PartialView("_IssueWithoutBomDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetSearchData(string REQNo, string ReqDate, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new IssueWOBomMainDashboard();
            model = await _IIssueWOBOM.GetSearchData(REQNo, ReqDate, IssueSlipNo, IssueDate, WorkCenter, YearCode, ReqYearCode, FromDate, ToDate);
            var modelList = model?.IssueWOBOMDashboard ?? new List<IssueWOBomMainDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.IssueWOBOMDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<IssueWOBomMainDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.IssueWOBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssWithOutBOMList_Summary", modelList, cacheEntryOptions);
            return PartialView("_IssueWithoutBomDashboardGrid", model);
        }

        public async Task<IActionResult> GetDetailData(string REQNo, string ReqDate, string PartCode, string Item_Name, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new IssueWOBomMainDashboard();
            model = await _IIssueWOBOM.GetDetailData(REQNo, ReqDate, PartCode, Item_Name, IssueSlipNo, IssueDate, WorkCenter, YearCode, ReqYearCode, FromDate, ToDate);
            model.DashboardType = "Detail";
            var modelList = model?.IssueWOBOMDashboard ?? new List<IssueWOBomMainDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.IssueWOBOMDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<IssueWOBomMainDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.IssueWOBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssWithOutBOMList_Detail", modelList, cacheEntryOptions);
            return PartialView("_IssueWithoutBomDashboardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            IssueWOBomMainDashboard model = new IssueWOBomMainDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_IssueWithoutBomDashboardGrid", new List<IssueWOBomMainDashboard>());
            }
            string cacheKey = $"KeyIssWithOutBOMList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<IssueWOBomMainDashboard> IssueWithOutBOMDashboard) || IssueWithOutBOMDashboard == null)
            {
                return PartialView("_IssueWithoutBomDashboardGrid", new List<IssueWOBomMainDashboard>());
            }

            List<IssueWOBomMainDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = IssueWithOutBOMDashboard.ToList();
            }
            else
            {
                filteredResults = IssueWithOutBOMDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = IssueWithOutBOMDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.IssueWOBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_IssueWithoutBomDashboardGrid", model);
        }
        public async Task<IActionResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo,string IssuedDate)
        {
            IssuedDate = ParseFormattedDate(IssuedDate);
            var FinStartDate = ParseFormattedDate(HttpContext.Session.GetString("FromDate"));
             var JSON = await _IIssueWOBOM.FillBatchUnique(ItemCode,YearCode,StoreName,BatchNo, IssuedDate,FinStartDate);
             string JsonString = JsonConvert.SerializeObject(JSON);
             return Json(JsonString);
        }
        public async Task<IActionResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
             var JSON = await _IIssueWOBOM.FillLotandTotalStock(ItemCode,StoreId,TillDate,BatchNo,UniqBatchNo);
             string JsonString = JsonConvert.SerializeObject(JSON);
             return Json(JsonString);
        }
        
        public async Task<IActionResult> DeleteByID(int ID, int YC,int ActualEntryBy,string EntryByMachine, string FromDate, string ToDate, string REQNo, string WCName, string PartCode, string ItemName)
        {
            var getData = _IIssueWOBOM.GetDataForDelete(ID,YC);
            EntryByMachine = Environment.MachineName;

            long[] ICArray = new long[getData.Result.Result.Rows.Count];
            string[] batchNoArray = new string[getData.Result.Result.Rows.Count];
            string[] uniqBatchArray = new string[getData.Result.Result.Rows.Count];

            if (getData.Result.Result != null)
            {
                for(int i=0;i<getData.Result.Result.Rows.Count;i++)
                {
                    ICArray[i]=getData.Result.Result.Rows[i].ItemArray[0];
                    batchNoArray[i]=getData.Result.Result.Rows[i].ItemArray[1];
                    uniqBatchArray[i]=getData.Result.Result.Rows[i].ItemArray[2];
                    var checkLasTransDate = _IIssueWOBOM.CheckLastTransDate(ICArray[i], batchNoArray[i], uniqBatchArray[i]);
                    if(checkLasTransDate.Result.Result.Rows[0].ItemArray[0]!= "Successful")
                    {
                        ViewBag.isSuccess = true;
                        TempData["423"] = "423";
                        return RedirectToAction("Dashboard", new { FromDate = "", Todate ="", Flag = "True" });
                    }
                }
            }
            ActualEntryBy= Convert.ToInt32(HttpContext.Session.GetString("UID"));

            var Result = await _IIssueWOBOM.DeleteByID(ID, YC,ActualEntryBy,EntryByMachine);

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
            
            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", REQNo = REQNo, WCName = WCName, PartCode = PartCode, ItemName = ItemName});

        }
        public async Task<JsonResult> GetNewEntry()
        {
            int YC = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IIssueWOBOM.GetNewEntry(YC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GETDepartMent(string ReqNo, int ReqYearCode)
        {
            var JSON = await _IIssueWOBOM.GETDepartMent(ReqNo, ReqYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> PassForCloseReq()
        {
            var JSON = await _IIssueWOBOM.PassForCloseReq();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> PassForCloseRequisitionItem()
        {
            var JSON = await _IIssueWOBOM.PassForCloseRequisitionItem();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowBatch()
        {
            var JSON = await _IIssueWOBOM.GetAllowBatch();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetIssueScanFeature()
        {
            var JSON = await _IIssueWOBOM.GetIssueScanFeature();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckStockBeforeSaving(int ItemCode,int StoreId,string TillDate, string BatchNo,string UniqBatchNo)
        {
            var JSON = await _IIssueWOBOM.CheckStockBeforeSaving(ItemCode,StoreId,TillDate,BatchNo,UniqBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckRequisitionBeforeSaving(string ReqNo, string ReqDate,int ItemCode)
        {
            var JSON = await _IIssueWOBOM.CheckRequisitionBeforeSaving(ReqNo,ReqDate,ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate, string ReqNo, int ReqYearCode, string ReqDate)
        {
                var MainModel = new IssueWithoutBom();
            try
            {
                ResponseResult StockData = new ResponseResult();
                var ItemDetailData = await _IIssueWOBOM.GetItemDetailFromUniqBatch(UniqBatchNo, YearCode, TransDate);
              
                ResponseResult ReqQty = await _IIssueWOBOM.GetReqQtyForScan(ReqNo,ReqYearCode,ReqDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));
                ResponseResult ReqStoreId = await _IIssueWOBOM.GetStoreIdReqForScan(ReqNo, ReqYearCode, ReqDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));

                decimal ReqQuantity = 0;
                
                if (ReqQty.Result.Rows.Count != 0) 
                {
                    ReqQuantity = Convert.ToDecimal(ReqQty.Result.Rows[0].ItemArray[0]);
                }
                else
                {
                    return StatusCode(203, "Invalid barcode this item " + ItemDetailData.Result.Rows[0].ItemArray[0] + " do not exist in this requisition");
                }

                if (ItemDetailData.Result != null)
                {
                    if (ItemDetailData.Result.Rows.Count != 0)
                    {
                        
                        StockData = await _IIssueWOBOM.FillLotandTotalStock(Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), Convert.ToInt32(ReqStoreId.Result.Rows[0].ItemArray[0]), TransDate, ItemDetailData.Result.Rows[0].ItemArray[2], UniqBatchNo);
                    }
                    else
                    {
                        return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                    }
                }
                else
                {
                    return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                }
              
                //var ItemList = new List<IssueWithoutBomDetail>();

                var lotStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[0]);
                var totStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[1]);

                var stock = lotStock <= totStock ? lotStock : totStock;

                var issueQty = stock <= ReqQuantity ? stock : ReqQuantity;
                //var JSON = await _IIssueWOBOM.ShowDetail(ReqDate, ReqDate, ReqNo, YearCode, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), "", 0, 0, ReqYearCode, ReqDate,"" , "", Convert.ToInt32(ReqStoreId.Result.Rows[0].ItemArray[0]));
                //string JsonString = JsonConvert.SerializeObject(JSON.Result.Table);


                //// Deserialize into list of IssueWithoutBomDetail
                //var ItemList = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(JsonString) ?? new List<IssueWithoutBomDetail>();

                //ItemList.Add(new IssueWithoutBomDetail
                //{
                //    ItemName = ItemDetailData.Result.Rows[0].ItemArray[0],
                //    PartCode = ItemDetailData.Result.Rows[0].ItemArray[1],
                //    ItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),
                //    BatchNo = ItemDetailData.Result.Rows[0].ItemArray[2],
                //    uniqueBatchNo = UniqBatchNo,
                //    Unit = ItemDetailData.Result.Rows[0].ItemArray[3],
                //    LotStock = lotStock,
                //    TotalStock = totStock,
                //    IssueQty = issueQty,
                //    ReqQty = ReqQuantity
                //});
                var JSON = await _IIssueWOBOM.ShowDetail(ReqDate, ReqDate, ReqNo, YearCode,Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),"", 0, 0, ReqYearCode, ReqDate, "", "",Convert.ToInt32(ReqStoreId.Result.Rows[0].ItemArray[0]));

                var ItemList = new List<IssueWithoutBomDetail>();

                if (JSON?.Result != null && JSON.Result.Tables.Count > 0)
                {
                    var table = JSON.Result.Tables[0];
                    foreach (DataRow row in table.Rows)
                    {
                        var item = new IssueWithoutBomDetail
                        {
                            ItemName = ItemDetailData.Result.Rows[0].ItemArray[0],
                            PartCode = ItemDetailData.Result.Rows[0].ItemArray[1],
                            ItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),
                            BatchNo = ItemDetailData.Result.Rows[0].ItemArray[2],
                            uniqueBatchNo = UniqBatchNo,
                            Unit = ItemDetailData.Result.Rows[0].ItemArray[3],
                            LotStock = lotStock,
                            TotalStock = totStock,
                            IssueQty = issueQty,
                            ReqQty = ReqQuantity,
                            StdPacking = row["StdPacking"] != DBNull.Value ? Convert.ToSingle(row["StdPacking"]) : 0,
                            StoreName = row["StoreName"]?.ToString(),
                            AltQty = row["AltQty"] != DBNull.Value ? Convert.ToDecimal(row["AltQty"]) : 0,
                            AltUnit = row["AltUnit"]?.ToString(),
                            Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                            Remark = row["Remark"]?.ToString(),
                            AltItemCode = row["AltItemCode"] != DBNull.Value ? Convert.ToInt32(row["AltItemCode"]) : 0,
                            CostCenterId = row["CostCenterId"] != DBNull.Value ? Convert.ToInt32(row["CostCenterId"]) : 0,
                            ItemSize = row["ItemSize"]?.ToString(),
                            ItemColor = row["ItemColor"]?.ToString(),
                            StoreId = row["storeid"] != DBNull.Value ? Convert.ToInt32(row["storeid"]) : 0,
                            ReqDept = row["ReqDepartment"]?.ToString(),
                            ReqDepartmentID = row["ReqDepartmentID"] != DBNull.Value ? Convert.ToInt32(row["ReqDepartmentID"]) : 0,
                            WCId = row["workcenterId"] != DBNull.Value ? Convert.ToInt32(row["workcenterId"]) : 0,
                            WorkCenter = row["WorkCenterName"]?.ToString(),
                            TransactionDate = row["TransactionDate"]?.ToString()
                        };

                        ItemList.Add(item);
                    }
                }

                var model = ItemList;

                var IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
                var IssueGrid = new List<IssueWithoutBomDetail>();
                var SSGrid = new List<IssueWithoutBomDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        string modelJson = HttpContext.Session.GetString("KeyIssWOBomScannedGrid");
                        List<IssueWithoutBomDetail> IssueWithoutBomDetailGrid = new List<IssueWithoutBomDetail>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
                        }
                        
                        if (item != null)
                        {
                            if (IssueWithoutBomDetailGrid == null)
                            {
                                if (item.LotStock <= 0 || item.TotalStock <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                item.seqno += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (IssueWithoutBomDetailGrid.Where(x => x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                if (item.LotStock <= 0 || item.TotalStock <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                else
                                {
                                    item.seqno = IssueWithoutBomDetailGrid.Count + 1;
                                    IssueGrid = IssueWithoutBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssWOBomScannedGrid", serializedGrid);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return PartialView("_IssueByScanningGrid",MainModel);
        }

        public IActionResult EditItemRow(int SeqNo)
        {
            string modelJson = HttpContext.Session.GetString("KeyIssWOBomGrid");
            List<IssueWithoutBomDetail> IssueGrid = new List<IssueWithoutBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueGrid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(modelJson);
            }

            var SSGrid = IssueGrid.Where(x => x.seqno == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {            
            var JSON = await _IIssueWOBOM.AltUnitConversion(ItemCode,AltQty,UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillProjectNo()
        {            
            var JSON = await _IIssueWOBOM.FillProjectNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IIssueWOBOM.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillDept()
        {
            var JSON = await _IIssueWOBOM.FillDept();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        //public async Task<JsonResult> FillEmployee()
        //{
        //    var JSON = await _IIssueWOBOM.FillEmployee();
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}
        public async Task<JsonResult> GetIsStockable(int ItemCode)
        {            
            var JSON = await _IIssueWOBOM.GetIsStockable(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}


