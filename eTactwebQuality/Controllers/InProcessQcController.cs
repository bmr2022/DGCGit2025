using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class InProcessQcController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IInProcessQc _IInProcessQc;
        private readonly ILogger<InProcessQcController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public InProcessQcController(ILogger<InProcessQcController> logger, IDataLogic iDataLogic, IInProcessQc IInProcessQc, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IInProcessQc = IInProcessQc;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> InProcessQc()
        {
            ViewData["Title"] = "In Process Qc Detail";
            TempData.Clear();
            _MemoryCache.Remove("KeyInProcessQcGrid");
            _MemoryCache.Remove("KeyInProcessQcDetail");
            var MainModel = new InProcessQc();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel = await BindEmpList(MainModel);
            MainModel.InProcQCYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntrydate = HttpContext.Session.GetString("EntryDate");
            //MainModel = await BindModel(MainModel);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyInProcessQcGrid", MainModel, cacheEntryOptions);
            //MainModel.DateIntact = "N";
            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> InProcessQc(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string QcSlipNo = "", string PartCode = "", string ItemName = "", string ProdSlipNo = "", string WorkCenter = "",string ProcessName = "",string ProdSchNo="",string ProdPlanNo="", string DashboardType = "",  string SearchBox = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new InProcessQc();
            _MemoryCache.Remove("KeyInProcessQcDetail");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.InProcQCYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.UID=Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel = await BindEmpList(MainModel);
            _MemoryCache.Remove("KeyInProcessQcGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IInProcessQc.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindEmpList(MainModel);
                //MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyInProcessQcGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntrydate = HttpContext.Session.GetString("EntryDate");
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = HttpContext.Session.GetString("LastUpdatedDate");
            }
            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.WorkCenterBack = WorkCenter;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = SearchBox;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> InProcessQc(InProcessQc model)
        {
            try
            {
                var InProcessQcGrid = new DataTable();

                _MemoryCache.TryGetValue("KeyInProcessQcDetail", out List<InProcessQcDetail> InProcessQcDetail);
                _MemoryCache.TryGetValue("KeyInProcessQcGrid", out List<InProcessQcDetail> InProcessQcDetailEdit);

                if (InProcessQcDetail == null && InProcessQcDetailEdit == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("InProcessQcDetail", "In Process Qc Detail Should Have Atleast 1 Item...!");
                    model = await BindEmpList(model);
                    return View("InProcessQc", model);
                }

                else
                {
                    model.UID=Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        InProcessQcGrid = GetDetailTable(InProcessQcDetailEdit);
                    }
                    else
                    {
                        InProcessQcGrid = GetDetailTable(InProcessQcDetail);
                    }
                    model.EnteredbyMachineName = HttpContext.Session.GetString("ClientMachineName");
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    var Result = await _IInProcessQc.SaveInprocessQc(model, InProcessQcGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove(InProcessQcDetail);
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
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
                    return RedirectToAction("PendingInProcesstoQc" , "PendingInProcesstoQc");
                }
            }
            catch (Exception ex)
            {
                LogException<InProcessQcController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<InProcessQcDetail> InProcessQcList)
        {
            try
            {
                var InProcessGrid = new DataTable();

                InProcessGrid.Columns.Add("SeqNo", typeof(int));
                InProcessGrid.Columns.Add("InProcQCEntryId", typeof(int));
                InProcessGrid.Columns.Add("InProcQCYearCode", typeof(int));
                InProcessGrid.Columns.Add("ItemCode", typeof(int));
                InProcessGrid.Columns.Add("ProdEntryid", typeof(int));
                InProcessGrid.Columns.Add("ProdSlipNo", typeof(string));
                InProcessGrid.Columns.Add("ProdYearCode", typeof(int));
                InProcessGrid.Columns.Add("ProdEntryDate", typeof(string));
                InProcessGrid.Columns.Add("ProdSchNo", typeof(string));
                InProcessGrid.Columns.Add("ProdSchYearCode", typeof(int));
                InProcessGrid.Columns.Add("ProdSchdate", typeof(string));
                InProcessGrid.Columns.Add("ProdPlanNo", typeof(string));
                InProcessGrid.Columns.Add("ProdPlanYearCode", typeof(int));
                InProcessGrid.Columns.Add("ProdPlanDate", typeof(string));
                InProcessGrid.Columns.Add("ReqNo", typeof(string));
                InProcessGrid.Columns.Add("ReqDate", typeof(string));
                InProcessGrid.Columns.Add("ReqYearCode", typeof(int));
                InProcessGrid.Columns.Add("ProdQty", typeof(float));
                InProcessGrid.Columns.Add("ProdOkQty", typeof(float));
                InProcessGrid.Columns.Add("OkQty", typeof(float));
                InProcessGrid.Columns.Add("RejQty", typeof(float));
                InProcessGrid.Columns.Add("RewQty", typeof(float));
                InProcessGrid.Columns.Add("RejReason", typeof(string));
                InProcessGrid.Columns.Add("RewRemark", typeof(string));
                InProcessGrid.Columns.Add("otherdetail", typeof(string));
                InProcessGrid.Columns.Add("Batchno", typeof(string));
                InProcessGrid.Columns.Add("Uniquebatchno", typeof(string));
                InProcessGrid.Columns.Add("QcClearByEMPId", typeof(int));
                InProcessGrid.Columns.Add("ApprovedByEmpId", typeof(int));
                InProcessGrid.Columns.Add("TransferedQty", typeof(float));
                InProcessGrid.Columns.Add("PendForTransfQty", typeof(float));
                InProcessGrid.Columns.Add("TransferToWcOrStore", typeof(string));
                InProcessGrid.Columns.Add("TransferToWC", typeof(int));
                InProcessGrid.Columns.Add("TransferToStore", typeof(int));
                InProcessGrid.Columns.Add("ProcessId", typeof(int));
                InProcessGrid.Columns.Add("ProdInWC", typeof(int));
                InProcessGrid.Columns.Add("MaterialIstransfered", typeof(string));
                InProcessGrid.Columns.Add("Sampleqtytested", typeof(float));
                InProcessGrid.Columns.Add("TestingMethod", typeof(string));
                foreach (var Item in InProcessQcList)
                {
                    InProcessGrid.Rows.Add(
                        new object[]
                        {

                    Item.SeqNo == 0 ? 0 : Item.SeqNo,
                    Item.InProcessEntryId== 0 ? 0 : Item.InProcessEntryId,
                    Item.InProcQCYearCode== 0 ? 0 : Item.InProcQCYearCode,
                    Item.ItemCode == 0 ? 0:Item.ItemCode,
                    Item.ProdEntryId== 0 ? 0:Item.ProdEntryId,
                    Item.ProdSlipNo==null ? "" : Item.ProdSlipNo,
                    Item.ProdYearcode== 0 ? 0:Item.ProdYearcode,
                    Item.ProdDate==null ? string.Empty : ParseFormattedDate(Item.ProdDate.Split(" ")[0]),
                    Item.ProdPlanSchNo== null ? "":Item.ProdPlanSchNo,
                    Item.ProdPlanSchYearCode== 0 ? 0:Item.ProdPlanSchYearCode,
                    Item.ProdPlanSchDate == null ? string.Empty : ParseFormattedDate(Item.ProdPlanSchDate.Split(" ")[0]),
                    Item.ProdPlanNo == null ? "" : Item.ProdPlanNo,
                    Item.ProdPlanYearCode== 0 ? 0:Item.ProdPlanYearCode,
                    Item.ProdPlanDate==null ? string.Empty : ParseFormattedDate(Item.ProdPlanDate.Split(" ")[0]),
                    Item.Reqno == null ? "" : Item.Reqno,
                    Item.ReqDate==null ? string.Empty : ParseFormattedDate(Item.ReqDate.Split(" ")[0]),
                    Item.ReqYearCode== 0 ? 0:Item.ReqYearCode,
                    Item.TotProdQty== 0 ? 0:Item.TotProdQty,
                    Item.OKProdQty == 0 ? 0 : Item.OKProdQty,
                    Item.QCOKQty == 0 ? 0 : Item.QCOKQty ,
                    Item.QCRejQty == 0 ? 0 : Item.QCRejQty,
                    Item.RewQty == 0 ? 0 : Item.RewQty,
                    Item.RejReason == null ? "" : Item.RejReason,
                    Item.RewRemark == null ? "" : Item.RewRemark,
                    Item.otherdetail == null ? "" : Item.otherdetail,
                    Item.BatchNo == null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo == null ? "" : Item.UniqueBatchNo,
                    Item.QcClearByEMPId==0?0:Item.QcClearByEMPId,
                    Item.ApprovedByEmpId==0?0:Item.ApprovedByEmpId,
                    Item.TransferedQty==0?0:Item.TransferedQty,
                    Item.PendForTransfQty == 0 ? 0 : Item.PendForTransfQty,
                    Item.ToStoreOrWc == null ? "" : Item.ToStoreOrWc,
                    Item.ToWcId==0?0:Item.ToWcId,
                    Item.ToStoreId==0?0:Item.ToStoreId,
                    Item.ProcessId==0?0:Item.ProcessId,
                    Item.WcId==0?0:Item.WcId,
                    Item.MaterialIstransfered==null?"":Item.MaterialIstransfered,
                    Item.Sampleqtytested==0?0:Item.Sampleqtytested,
                    Item.TestingMethod==null?"":Item.TestingMethod
                    });
                }
                InProcessGrid.Dispose();
                return InProcessGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> FillItems(string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IInProcessQc.FillItems(SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillInProcQCSlipNo()
        {
            var JSON = await _IInProcessQc.FillInProcQCSlipNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> BindProdSlip()
        {
            var JSON = await _IInProcessQc.BindProdSlip();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryId(int YearCode)
        {
            var JSON = await _IInProcessQc.FillEntryId("NewEntryId", YearCode, "SP_InProcessQC");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private async Task<InProcessQc> BindEmpList(InProcessQc model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IInProcessQc.BindEmpList("BindEmpList");
            model.EmpNameList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["Emp_Name"].ToString()
                    });
                }
                model.EmpNameList = _List;
                _List = new List<TextValue>();

            }
            return model;
        }
        public IActionResult FillGridFromMemoryCache()
        {
            try
            {
                _MemoryCache.TryGetValue("KeyInProcess", out IList<InProcessQcDetail> InProcessDetailGrid);
                var MainModel = new InProcessQc();
                var InProcessGrid = new List<InProcessQcDetail>();
                var SSGrid = new List<InProcessQcDetail>();
                MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                MainModel.ToDate = HttpContext.Session.GetString("ToDate");

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (InProcessDetailGrid != null)
                {
                    for (int i = 0; i < InProcessDetailGrid.Count; i++)
                    {
                        if (InProcessDetailGrid[i] != null)
                        {
                            InProcessDetailGrid[i].SeqNo = seqNo++;
                            SSGrid.AddRange(InProcessGrid);
                            InProcessGrid.Add(InProcessDetailGrid[i]);

                            MainModel.ItemDetailGrid = InProcessGrid;

                            _MemoryCache.Set("KeyInProcess", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }

                return PartialView("_InProcessQcGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult FillGridFromGridMemoryCache()
        {
            try
            {
                _MemoryCache.TryGetValue("KeyInProcessQcGrid", out IList<InProcessQcDetail> InProcessDetailGrid);
                var MainModel = new InProcessQc();
                var InProcessGrid = new List<InProcessQcDetail>();
                var SSGrid = new List<InProcessQcDetail>();
                MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                MainModel.ToDate = HttpContext.Session.GetString("ToDate");

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (InProcessDetailGrid != null)
                {
                    for (int i = 0; i < InProcessDetailGrid.Count; i++)
                    {
                        if (InProcessDetailGrid[i] != null)
                        {
                            InProcessDetailGrid[i].SeqNo = seqNo++;
                            SSGrid.AddRange(InProcessGrid);
                            InProcessGrid.Add(InProcessDetailGrid[i]);

                            MainModel.ItemDetailGrid = InProcessGrid;

                            _MemoryCache.Set("KeyInProcess", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }

                return PartialView("_InProcessQcGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new InProcessQc();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyInProcessQcGrid", out List<InProcessQcDetail> InProcessQcDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (InProcessQcDetail != null && InProcessQcDetail.Count > 0)
                {
                    InProcessQcDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in InProcessQcDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = InProcessQcDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyInProcessQcGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyInProcessQcDetail", out List<InProcessQcDetail> InProcessQcDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (InProcessQcDetail != null && InProcessQcDetail.Count > 0)
                {
                    InProcessQcDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in InProcessQcDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = InProcessQcDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyInProcessQcDetail", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_InProcessQcGridDetail", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<InProcessQcDetail> InProcessQcDetail = new List<InProcessQcDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyInProcessQcGrid", out InProcessQcDetail);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyInProcessQcDetail", out InProcessQcDetail);
            }
            IEnumerable<InProcessQcDetail> SSGrid = InProcessQcDetail;
            if (InProcessQcDetail != null)
            {
                SSGrid = InProcessQcDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTransferToWorkCenter()
        {
            var JSON = await _IInProcessQc.FillTransferToWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTransferToStore()
        {
            var JSON = await _IInProcessQc.FillTransferToStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillReworkreason()
        {
            var JSON = await _IInProcessQc.FillReworkreason();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRejectionreason()
        {
            var JSON = await _IInProcessQc.FillRejectionreason();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddInProcessQcGridEntry(List<InProcessQcDetail> model , string Mode)
        {
            try
            {
                var MainModel = new InProcessQc();
                var InProcessQcDetailGrid = new List<InProcessQcDetail>();
                var SSGrid = new List<InProcessQcDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                if (Mode == "U") 
                {
                    _MemoryCache.TryGetValue("KeyInProcessQcGrid", out IList<InProcessQcDetail> InProcessQcDetail);
                    var seqNo = 0;
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (InProcessQcDetail == null)
                            {
                                item.SeqNo=seqNo + 1;
                                InProcessQcDetailGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                item.SeqNo = InProcessQcDetail.Count + 1;
                                InProcessQcDetailGrid = InProcessQcDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(InProcessQcDetailGrid);
                                InProcessQcDetailGrid.Add(item);
                            }
                            MainModel.ItemDetailGrid = InProcessQcDetailGrid;
                            _MemoryCache.Set("KeyInProcessQcGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                else
                {
                    _MemoryCache.TryGetValue("KeyInProcessQcDetail", out IList<InProcessQcDetail> InProcessQcDetail);
                    var seqNo = 0;
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (InProcessQcDetail == null)
                            {
                                item.SeqNo=seqNo + 1;
                                InProcessQcDetailGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                item.SeqNo = InProcessQcDetail.Count + 1;
                                InProcessQcDetailGrid = InProcessQcDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(InProcessQcDetailGrid);
                                InProcessQcDetailGrid.Add(item);
                            }
                            MainModel.ItemDetailGrid = InProcessQcDetailGrid;
                            _MemoryCache.Set("KeyInProcessQcDetail", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                return PartialView("_InProcessQcGridDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> InProcessQcDashboard(string FromDate = "", string ToDate = "", string Flag = "True")
        {
            try
            {
                _MemoryCache.Remove("KeyInProcessQcDetail");

                var model = new InProcessDashboard();
                var Result = await _IInProcessQc.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "InProcQCEntryId", "InProcQCYearCode",
                            "InProcQCSlipNo",  "QCClearedByEmpId", "QCClearingDate", "EnteredByMachine",
                            "ActualEntryById", "ActualEntryDate", "LastUpdatedDate", "LastUpdatedBy", "CC",
                             "MaterialIstransfered");
                        model.InProcessDashboard = CommonFunc.DataTableToList<InProcessQcDashboard>(DT, "InProcessQC");

                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        return View(model);


                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetSearchData(string FromDate, string ToDate, string QcSlipNo, string ItemName, string PartCode, string ProdSlipNo, string WorkCenter, string ProcessName,string ProdSchNo,string ProdPlanNo, string DashboardType)
        {
            var model = new InProcessDashboard();
            model = await _IInProcessQc.GetDashboardData(FromDate, ToDate, QcSlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ProdSlipNo, DashboardType);
            model.DashboardType = "Summary";
            return PartialView("_InProcessQcDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string InProcQcSlipNo, string ItemName, string PartCode, string ProdSlipNo, string WorkCenter, string ProcessName, string ProdSchNo, string ProdPlanNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new InProcessDashboard();
            model = await _IInProcessQc.GetDashboardDetailData(FromDate, ToDate, InProcQcSlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ProdSlipNo, DashboardType);
            model.DashboardType = "Detail";
            return PartialView("_InProcessQcDashboardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate)
        {
            var Result = await _IInProcessQc.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate);

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

            return RedirectToAction("InProcessQcDashboard", new { EntryDate = EntryDate, Flag = "False", CC = CC, EntryByMachineName = EntryByMachineName });
        }
    }
}
