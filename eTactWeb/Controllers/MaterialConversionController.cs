using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static Grpc.Core.Metadata;

namespace eTactWeb.Controllers
{
    public class MaterialConversionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMaterialConversion _IMaterialConversion { get; }
        private readonly ILogger<MaterialConversionController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public MaterialConversionController(ILogger<MaterialConversionController> logger, IDataLogic iDataLogic, IMaterialConversion iMaterialConversion, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialConversion = iMaterialConversion;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public IActionResult PrintReport(int EntryId, int YC, string SlipNo)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();

            webReport.Report.Dispose();
            webReport.Report = new Report();

            //webReport.Report.Load(webRootPath + "\\MaterialConversionReport.frx"); // default report
            var ReportName = _IMaterialConversion.GetReportName();

            // webReport.Report.Load(webRootPath + "\\DirectPurchaseBillReport.frx"); // default report
            if (!string.IsNullOrWhiteSpace(Convert.ToString(ReportName.Result.Result.Rows[0].ItemArray[0])))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\MaterialConversionReport.frx"); // default report

            }
            webReport.Report.SetParameterValue("EntryIdparam", EntryId);
            webReport.Report.SetParameterValue("YearCodeparam", YC);
            webReport.Report.SetParameterValue("SlipNoparam", SlipNo);
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> MaterialConversion(string formKey, int ID, int YC, string Mode, string SlipNo,
            int StoreId, int AltStoreId, int OrginalWCID, int AltWCID, int ActualEntryByEmpid, int UpdatedByEmpId, int PlanYearCode, int ProdSchYearCode,
        decimal OriginalQty, decimal AltOriginalQty, decimal AltStock, decimal BatchStock, decimal TotalStock, decimal OrigItemRate,
        string StoreName, string OriginalItemCode, string OriginalPartCode, string OriginalItemName, string Unit, string WorkCenterName, string AltStoreName, string AltWorkCenterName, string AltPartCode, string AltItemName, string AltUnit, string BatchNo,
        string UniqueBatchNo, string Remark, string EntryByMachine, string PlanNo, string PlanDate,
        int ProdSchNo, string ProdSchDatetime, string ActualEntryDate, string UpdationDate, string FromDate, string ToDate)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            var MainModel = new MaterialConversionModel();

            MainModel.OpeningYearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));

            MainModel.ActualEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            MainModel.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString($"EmpName_{formKey}");
            MainModel.cc = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
            MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));
            HttpContext.Session.Remove($"KeyMaterialConversionGrid_{uniqueKey}");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IMaterialConversion.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.EntryId = ID;
                MainModel.OpeningYearCode = YC;
                //MainModel.StoreId = StoreId;
                //MainModel.StoreName = StoreName;
                //MainModel.OriginalItemCode = OriginalItemCode;
                //MainModel.OriginalPartCode = OriginalPartCode;
                //MainModel.OriginalItemName = OriginalItemName;
                //MainModel.OriginalQty = OriginalQty;
                //MainModel.Unit = Unit;
                //MainModel.WorkCenterName = WorkCenterName;
                //MainModel.AltStoreId = AltStoreId;
                //MainModel.AltStoreName = AltStoreName;
                //MainModel.OrginalWCID = OrginalWCID;
                //MainModel.AltWCID = AltWCID;
                //MainModel.AltWorkCenterName = AltWorkCenterName;
                //MainModel.AltPartCode = AltPartCode;
                //MainModel.AltItemName = AltItemName;
                //MainModel.AltOriginalQty = AltOriginalQty;
                //MainModel.AltUnit = AltUnit;
                //MainModel.AltStock = AltStock;
                //MainModel.BatchNo = BatchNo;
                //MainModel.UniqueBatchNo = UniqueBatchNo;
                //MainModel.BatchStock = BatchStock;
                //MainModel.TotalStock = TotalStock;
                //MainModel.OrigItemRate = OrigItemRate;
                //MainModel.Remark = Remark;
                //MainModel.ActualEntryByEmpid = ActualEntryByEmpid;
                //MainModel.ActualEntryDate = ActualEntryDate;
                //MainModel.UpdatedByEmpId = UpdatedByEmpId;
                //MainModel.UpdationDate = UpdationDate;
                //MainModel.EntryByMachine = EntryByMachine;
                //MainModel.PlanNo = PlanNo;
                //MainModel.PlanYearCode = PlanYearCode;
                //MainModel.PlanDate = PlanDate;
                //MainModel.ProdSchNo = ProdSchNo;
                //MainModel.ProdSchYearCode = ProdSchYearCode;
                //MainModel.ProdSchDatetime = ProdSchDatetime;
                //MainModel.SlipNo = "1";

                if (Mode == "U")
                {
                    MainModel.UpdatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                    MainModel.UpdatedByEmpName = HttpContext.Session.GetString($"EmpName_{formKey}");
                    MainModel.UpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.cc = HttpContext.Session.GetString($"Branch_{formKey}");
                    MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
                    MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));
                }

                string serializedGrid = JsonConvert.SerializeObject(MainModel.MaterialConversionGrid);
                HttpContext.Session.SetString($"KeyMaterialConversionGrid_{uniqueKey}", serializedGrid);
            }

            return View(MainModel);
        }
        public async Task<JsonResult> AutoFillPartCode(string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IMaterialConversion.AutoFillitem("AutoFillPartCode", SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillItemName(string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IMaterialConversion.AutoFillitem("AutoFillItemName", SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> AutoFillAlternatePartCode(int origItemcode, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IMaterialConversion.AutoFillAltitem("AutoFillAlternatePartCode", origItemcode, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillAlternateItemName(int origItemcode, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IMaterialConversion.AutoFillAltitem("AutoFillAlternateItemName", origItemcode, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await _IMaterialConversion.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<IActionResult> MaterialConversion(MaterialConversionModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                var formKey = model.formKey;
                var uniqueKey = model.uniqueKey;
                string modelJson = HttpContext.Session.GetString($"KeyMaterialConversionGrid_{uniqueKey}");
                List<MaterialConversionModel> MaterialConversionGrid = new List<MaterialConversionModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MaterialConversionGrid = JsonConvert.DeserializeObject<List<MaterialConversionModel>>(modelJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                GIGrid = GetDetailTable(MaterialConversionGrid);
                model.EntryByMachine = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                var Result = await _IMaterialConversion.SaveMaterialConversion(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove($"KeyMaterialConversionGrid_{uniqueKey}");
                        //return RedirectToAction(nameof(MaterialConversionDashBoard));
                        return Json(new
                        {
                            success = true,
                            message = "Data Save successfully",
                            redirectUrl = Url.Action("MaterialConversionDashBoard", "MaterialConversion", new { formKey = formKey })
                        });
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove($"KeyMaterialConversionGrid_{uniqueKey}");
                        //return RedirectToAction(nameof(MaterialConversionDashBoard));
                        return Json(new
                        {
                            success = true,
                            message = "Data Save successfully",
                            redirectUrl = Url.Action("MaterialConversionDashBoard", "MaterialConversion", new { formkey = formKey })
                        });
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);

                    }
                    else if (!string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        //ViewBag.isSuccess = false;
                        //TempData["ErrorMessage"] = Result.StatusText;
                        //return View(model);
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });
                    }
                    else if (Result.StatusText == "TransDate" || Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        var input = "";
                        if (Result?.Result != null)
                        {
                            if (Result.Result is string str)
                            {
                                input = str;
                            }
                            else
                            {
                                input = JsonConvert.SerializeObject(Result.Result);
                            }

                            TempData["ErrorMessage"] = input;
                        }
                        else
                        {
                            TempData["500"] = "500";
                        }


                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        //model.IsError = "true";
                        //return View("Error", Result);
                        HttpContext.Session.Remove($"KeyMaterialConversionGrid_{uniqueKey}");
                        return RedirectToAction(nameof(MaterialConversionDashBoard));
                    }
                }

                return RedirectToAction(nameof(MaterialConversionDashBoard));

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
                GIGrid.Columns.Add("OriginalQty", typeof(decimal));
                GIGrid.Columns.Add("AltItemCode", typeof(int));
                GIGrid.Columns.Add("AltUnit", typeof(string));
                GIGrid.Columns.Add("AltOriginalQty", typeof(decimal));
                GIGrid.Columns.Add("OriginalStoreId", typeof(long));
                GIGrid.Columns.Add("AltStoreId", typeof(long));
                GIGrid.Columns.Add("OriginalWCID", typeof(long));
                GIGrid.Columns.Add("AltWCID", typeof(long));
                GIGrid.Columns.Add("BatchNo", typeof(string));
                GIGrid.Columns.Add("UniqueBatchNo", typeof(string));
                GIGrid.Columns.Add("BatchStock", typeof(decimal));
                GIGrid.Columns.Add("TotalStock", typeof(decimal));
                GIGrid.Columns.Add("AltStock", typeof(decimal));
                GIGrid.Columns.Add("PlanNo", typeof(string));
                GIGrid.Columns.Add("PlanYearCode", typeof(long));
                GIGrid.Columns.Add("PlanDate", typeof(DateTime));
                GIGrid.Columns.Add("ProdSchNo", typeof(long));
                GIGrid.Columns.Add("ProdSchYearCode", typeof(long));
                GIGrid.Columns.Add("ProdSchdatetime", typeof(DateTime));
                GIGrid.Columns.Add("OrigItemRate", typeof(decimal));
                GIGrid.Columns.Add("Remark", typeof(string));
                GIGrid.Columns.Add("Seqno", typeof(long));
                GIGrid.Columns.Add("AltQty", typeof(decimal));

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
                            Item.StoreId == null ? 0 : Item.StoreId,
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
                            Item.seqno == null ? "" : Item.seqno,
                            Item.AltOriginalQty == null ? 0 : Item.AltOriginalQty,

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
        public async Task<JsonResult> FillStockBatchNo(string formKey, int ItemCode, string StoreName, string WorkCenterName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            var JSON = await _IMaterialConversion.FillStockBatchNo(ItemCode, StoreName, WorkCenterName, YearCode, batchno, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOriginalItemName()
        {
            var JSON = await _IMaterialConversion.GetOriginalItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddToGridData(MaterialConversionModel model, string uniqueKey)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString($"KeyMaterialConversionGrid_{uniqueKey}");
                List<MaterialConversionModel> MaterialConversionGrid = new List<MaterialConversionModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MaterialConversionGrid = JsonConvert.DeserializeObject<List<MaterialConversionModel>>(modelJson);
                }

                var MainModel = new MaterialConversionModel();
                var WorkOrderPGrid = new List<MaterialConversionModel>();
                var OrderGrid = new List<MaterialConversionModel>();
                var ssGrid = new List<MaterialConversionModel>();

                if (model != null)
                {
                    if (MaterialConversionGrid == null)
                    {
                        model.seqno = 1;
                        OrderGrid.Add(model);
                    }
                    else
                    {
                        if (MaterialConversionGrid.Any(x => (x.OriginalPartCode == model.OriginalPartCode && x.BatchNo == model.BatchNo && x.UniqueBatchNo == model.UniqueBatchNo)))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //count = WorkOrderProcessGrid.Count();
                            model.seqno = MaterialConversionGrid.Count + 1;
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

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.MaterialConversionGrid);
                    HttpContext.Session.SetString($"KeyMaterialConversionGrid_{uniqueKey}", serializedGrid);
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
        public IActionResult EditItemRow(int SrNO, string Mode, string uniqueKey)
        {
            IList<MaterialConversionModel> MaterialConversionModelGrid = new List<MaterialConversionModel>();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString($"KeyMaterialConversionGrid_{uniqueKey}");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MaterialConversionModelGrid = JsonConvert.DeserializeObject<List<MaterialConversionModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString($"KeyMaterialConversionGrid_{uniqueKey}");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MaterialConversionModelGrid = JsonConvert.DeserializeObject<List<MaterialConversionModel>>(modelJson);
                }
            }
            IEnumerable<MaterialConversionModel> SSBreakdownGrid = MaterialConversionModelGrid;
            if (MaterialConversionModelGrid != null)
            {
                SSBreakdownGrid = MaterialConversionModelGrid.Where(x => x.seqno == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SrNO, string Mode, string uniqueKey)
        {
            var MainModel = new MaterialConversionModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString($"KeyMaterialConversionGrid_{uniqueKey}");
                List<MaterialConversionModel> MaterialConversionGrid = new List<MaterialConversionModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MaterialConversionGrid = JsonConvert.DeserializeObject<List<MaterialConversionModel>>(modelJson);
                }

                int Indx = SrNO - 1;

                if (MaterialConversionGrid != null && MaterialConversionGrid.Count > 0)
                {
                    MaterialConversionGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in MaterialConversionGrid)
                    {
                        Indx++;
                        item.seqno = Indx;
                    }
                    MainModel.MaterialConversionGrid = MaterialConversionGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.MaterialConversionGrid);
                    HttpContext.Session.SetString($"KeyMaterialConversionGrid_{uniqueKey}", serializedGrid);
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
        public async Task<IActionResult> MaterialConversionDashBoard(string formKey, string ReportType, string FromDate, string ToDate)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            var model = new MaterialConversionModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            Dictionary<int, string> monthNames = new Dictionary<int, string>
            {
                {1, "Jan"}, {2, "Feb"}, {3, "Mar"}, {4, "Apr"}, {5, "May"}, {6, "Jun"},
                {7, "Jul"}, {8, "Aug"}, {9, "Sep"}, {10, "Oct"}, {11, "Nov"}, {12, "Dec"}
            };

            model.FromDate = $"{firstDayOfMonth.Day}/{monthNames[firstDayOfMonth.Month]}/{firstDayOfMonth.Year}";
            model.ToDate = $"{now.Day}/{monthNames[now.Month]}/{now.Year}";
            //DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            //model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            //model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ActualEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
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
        public async Task<IActionResult> GetDetailData(string formKey, string FromDate, string ToDate, string ReportType)
        {
            ViewBag.formKey = formKey;

            //model.Mode = "Search";
            var model = new MaterialConversionModel();

            model = await _IMaterialConversion.GetDashboardDetailData(FromDate, ToDate, ReportType);
            model.formKey = formKey;
            if (ReportType == "SUMMARY")
            {
                return PartialView("_MaterialConversionDashBoardSummaryGrid", model);
            }
            if (ReportType == "DETAIL")
            {
                return PartialView("_MaterialConversionDashBoardDetailGrid", model);
            }
            return null;

        }
        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var Result = await _IMaterialConversion.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);

            if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else if (!string.IsNullOrEmpty(Result.StatusText))
            {
                // If SP returned a message (like adjustment error)
                ViewBag.isSuccess = false;
                TempData["ErrorMessage"] = Result.StatusText;
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("MaterialConversionDashBoard");

        }
    }
}
