using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class MaterialConversionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMaterialConversion _IMaterialConversion { get; }

        private readonly ILogger<MaterialConversionController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MaterialConversionController(ILogger<MaterialConversionController> logger, IDataLogic iDataLogic, IMaterialConversion iMaterialConversion, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialConversion = iMaterialConversion;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> MaterialConversion()
        {
            var model = new MaterialConversionModel();

            model.OpeningYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            model.ActualEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            model.cc = HttpContext.Session.GetString("Branch");
            //model.ActualEntryDate = DateTime.Now.ToString("dd/MM/yyyy");

            return View(model);
        }

        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<IActionResult> MaterialConversion(MaterialConversionModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                //_MemoryCache.TryGetValue("MaterialConversionGrid", out List<MaterialConversionModel> MaterialConversionGrid);
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out List<MaterialConversionModel> MaterialConversionGrid);

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                //        if (model.Mode == "U")
                //        {
                //            GIGrid = GetDetailTable(AlternateItemMasterGrid);
                //            var mainItemCodes = AlternateItemMasterGrid
                //.Select(item => item.MainItemCode)
                //.ToList();
                //            var altItemCode = AlternateItemMasterGrid
                //.Select(item => item.AlternateItemCode)
                //.ToList();
                //            foreach (var maincode in mainItemCodes)
                //            {
                //                model.MainItemCode = maincode;
                //            }
                //            foreach (var code in altItemCode)
                //            {
                //                model.AlternateItemCode = code;
                //            }
                //        }
                //        else
                //        {
                //            GIGrid = GetDetailTable(AlternateItemMasterGrid);
                //        }
                GIGrid = GetDetailTable(MaterialConversionGrid);
                var Result = await _IMaterialConversion.SaveMaterialConversion(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove("AlternateItemMasterGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(MaterialConversion));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<MaterialConversionController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<MaterialConversionModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("OriginalItemCode", typeof(long));
                GIGrid.Columns.Add("Unit", typeof(string));
                GIGrid.Columns.Add("OriginalQty", typeof(float));
                GIGrid.Columns.Add("AltItemCode", typeof(int));
                GIGrid.Columns.Add("AltUnit", typeof(string));
                GIGrid.Columns.Add("AltOriginalQty", typeof(float));
                GIGrid.Columns.Add("OriginalStoreId", typeof(long));
                GIGrid.Columns.Add("AltStoreId", typeof(long));
                GIGrid.Columns.Add("OriginalWCID", typeof(long));
                GIGrid.Columns.Add("AltWCID", typeof(long));
                GIGrid.Columns.Add("BatchNo", typeof(string));
                GIGrid.Columns.Add("UniqueBatchNo", typeof(string));
                GIGrid.Columns.Add("BatchStock", typeof(float));
                GIGrid.Columns.Add("TotalStock", typeof(float));
                GIGrid.Columns.Add("AltStock", typeof(float));
                GIGrid.Columns.Add("PlanNo", typeof(string));
                GIGrid.Columns.Add("PlanYearCode", typeof(long));
                GIGrid.Columns.Add("PlanDate", typeof(DateTime));
                GIGrid.Columns.Add("ProdSchNo", typeof(long));
                GIGrid.Columns.Add("ProdSchYearCode", typeof(long));
                GIGrid.Columns.Add("ProdSchdatetime", typeof(DateTime));
                GIGrid.Columns.Add("OrigItemRate", typeof(float));
                GIGrid.Columns.Add("Remark", typeof(string));

                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                            Item.OriginalItemCode == null ? 0 : Item.OriginalItemCode,
                            Item.Unit == null ? "" : Item.Unit,
                            Item.OriginalQty == null ? 0f : Item.OriginalQty,
                            Item.AltItemCode == null ? 0 : Item.AltItemCode,
                            Item.AltUnit == null ? "" : Item.AltUnit,
                            Item.AltOriginalQty == null ? 0f : Item.AltOriginalQty,
                            Item.OriginalStoreId == null ? 0 : Item.OriginalStoreId,
                            Item.AltStoreId == null ? 0 : Item.AltStoreId,
                            Item.WcId == null ? 0 : Item.WcId,
                            Item.AltWCID == null ? 0 : Item.AltWCID,
                            Item.BatchNo == null ? "" : Item.BatchNo,
                            Item.UniqueBatchNo == null ? "" : Item.UniqueBatchNo,
                            Item.BatchStock == null ? 0f : Item.BatchStock,
                            Item.TotalStock == null ? 0f : Item.TotalStock,
                            Item.AltStock == null ? 0f : Item.AltStock,
                            Item.PlanNo == null ? "" : Item.PlanNo,
                            Item.PlanYearCode == null ? 0 : Item.PlanYearCode,
                            Item.PlanDate =  Item.PlanDate,
                            Item.ProdSchNo == null ? 0 : Item.ProdSchNo,
                            Item.ProdSchYearCode == null ? 0 : Item.ProdSchYearCode,
                            Item.ProdSchDatetime =   Item.ProdSchDatetime,
                            Item.OrigItemRate == null ? 0f : Item.OrigItemRate,
                            Item.Remark == null ? "" : Item.Remark,

                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> FillEntryID(int YearCode)
        {
            var JSON = await _IMaterialConversion.FillEntryID(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBranch()
        {
            var JSON = await _IMaterialConversion.FillBranch();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IMaterialConversion.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName()
        {
            var JSON = await _IMaterialConversion.FillWorkCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOriginalPartCode()
        {
            var JSON = await _IMaterialConversion.GetOriginalPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IMaterialConversion.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOriginalItemName()
        {
            var JSON = await _IMaterialConversion.GetOriginalItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddToGridData(MaterialConversionModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out IList<MaterialConversionModel> MaterialConversionGrid);

                var MainModel = new MaterialConversionModel();
                var WorkOrderPGrid = new List<MaterialConversionModel>();
                var OrderGrid = new List<MaterialConversionModel>();
                var ssGrid = new List<MaterialConversionModel>();

                if (model != null)
                {
                    if (MaterialConversionGrid == null)
                    {
                        model.SrNO = 1;
                        OrderGrid.Add(model);
                    }
                    else
                    {
                        if (MaterialConversionGrid.Any(x => (x.OriginalPartCode == model.OriginalPartCode)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //count = WorkOrderProcessGrid.Count();
                            model.SrNO = MaterialConversionGrid.Count + 1;
                            OrderGrid = MaterialConversionGrid.Where(x => x != null).ToList();
                            ssGrid.AddRange(OrderGrid);
                            OrderGrid.Add(model);

                        }

                    }

                    MainModel.MaterialConversionGrid = OrderGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyMaterialConversionGrid", MainModel.MaterialConversionGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                }
                return PartialView("_MaterialConversionGrid", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult EditItemRow(int SrNO, string Mode)
        {
            IList<MaterialConversionModel> MaterialConversionModelGrid = new List<MaterialConversionModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out MaterialConversionModelGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out MaterialConversionModelGrid);
            }
            IEnumerable<MaterialConversionModel> SSBreakdownGrid = MaterialConversionModelGrid;
            if (MaterialConversionModelGrid != null)
            {
                SSBreakdownGrid = MaterialConversionModelGrid.Where(x => x.SrNO == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SrNO, string Mode)
        {
            var MainModel = new MaterialConversionModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out List<MaterialConversionModel> MaterialConversionGrid);
                int Indx = SrNO - 1;

                if (MaterialConversionGrid != null && MaterialConversionGrid.Count > 0)
                {
                    MaterialConversionGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in MaterialConversionGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.MaterialConversionGrid = MaterialConversionGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyMaterialConversionGrid", MainModel.MaterialConversionGrid, cacheEntryOptions);
                }
            }
           
            return PartialView("_MaterialConversionGrid", MainModel);
        }
        public async Task<JsonResult> GetUnitAltUnit(int ItemCode)
        {
            var JSON = await _IMaterialConversion.GetUnitAltUnit(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAltPartCode(int MainItemcode)
        {
            var JSON = await _IMaterialConversion.GetAltPartCode(MainItemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAltItemName(int MainItemcode)
        {
            var JSON = await _IMaterialConversion.GetAltItemName(MainItemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> MaterialConversionDashBoard( string ReportType, string FromDate, string ToDate)
        {
            var model = new MaterialConversionModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ReportType = "SUMMARY";
            var Result = await _IMaterialConversion.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.MaterialConversionGrid = CommonFunc.DataTableToList<MaterialConversionModel>(dt, "MaterialConversionDashboard");
                }
                //var DT = DS.Tables[0]
                    //.DefaultView.ToTable(true, "OriginalItemCode", "Unit",
                    //"OriginalQty", "AltItemCode", "AltUnit", "AltOriginalQty", "OriginalStoreId", 
                    //"AltStoreId", "OriginalWCID", "AltWCID", "BatchNo", "UniqueBatchNo", "BatchStock",
                    //"TotalStock", "AltStock", "PlanNo", "PlanYearCode", "PlanDate", "ProdSchNo",
                    //"ProdSchYearCode", "ProdSchdatetime", "OrigItemRate", "Remark");
                //model.MaterialConversionGrid = CommonFunc.DataTableToList<MaterialConversionModel>(DT, "MaterialConversionDashboard");
             

            }
            
            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate,string ToDate,string ReportType)
        {
            //model.Mode = "Search";
            var model = new MaterialConversionModel();
            model = await _IMaterialConversion.GetDashboardDetailData( FromDate,  ToDate,  ReportType);
            if (ReportType=="SUMMARY")
            {
                return PartialView("_MaterialConversionDashBoardSummaryGrid", model);
            }
            if (ReportType== "DETAIL")
            {
                return PartialView("_MaterialConversionDashBoardDetailGrid", model);
            }
            return null;
           
        }
    }
}
