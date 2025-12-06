using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MirModel;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Web;
using FastReport;
using DocumentFormat.OpenXml.EMMA;

namespace eTactWeb.Controllers
{
    public class MIRController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IMirModule _IMirModule;
        private readonly ILogger<MIRController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        public List<string> ImageArray = new List<string>();
        private readonly ConnectionStringService _connectionStringService;
        public MIRController(ILogger<MIRController> logger, IDataLogic iDataLogic, IMirModule IMirModule, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMirModule = IMirModule;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this._iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _IMirModule.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0].ToString() + ".frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\MRN.frx"); // default report
            }

            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MrnNoparam", MrnNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> MIR()
        {
            ViewData["Title"] = "Quality Module Details";
            TempData.Clear();
            //_MemoryCache.Remove("KeyMIRGrid");
            HttpContext.Session.Remove("KeyMIRGrid");
            var MainModel = new MirModel();
            MainModel = await BindModel(MainModel);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            //_MemoryCache.Set("KeyMIRGrid", MainModel, cacheEntryOptions);

            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyMIRGrid", serializedGrid);


            //_MemoryCache.TryGetValue("KeyMIRGrid", out List<MirDetail> MIRDetail);

            string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
            List<MirDetail> MIRDetail = new List<MirDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MIRDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
            }

            MainModel.DateIntact = "N";
            MainModel.FromPend = "Y";
            if (MainModel.Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
            }
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> MIR(MirModel model)
        {
            var modelFrmDate = model.FromDate;
            var modelToDate = model.ToDate;
            var qc = model.MRNJWCustJW;
            if(qc == "MRN")
            {
                model.QcType = "M";
            }
            else if(qc == "VENDOR JOBWORK")
            {
                model.QcType = "V";
            }
            else if(qc == "CUSTOMER JOBWORK")
            {
                model.QcType= "C";
            }
            try
            {
                var MIRGrid = new DataTable();
                //_MemoryCache.TryGetValue("KeyMIRGrid", out List<MirDetail> MIRDetail);

                string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
                List<MirDetail> MIRDetail = new List<MirDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MIRDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
                }

                if (MIRDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("MIR", "MIR Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("MIR", model);
                }

                else
                {
                    MIRGrid = GetDetailTable(MIRDetail);
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    }
                    model = await BindModel(model);
                    var Result = await _IMirModule.SaveMIR(model, MIRGrid);
                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            TempData["SuccessMessage"] = "Data Saved successfully!";
                        }
                        else if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            TempData["SuccessMessage"] = "Data Updated successfully!";
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                        else if (!string.IsNullOrEmpty(Result.StatusText))
                        {
                            // If SP returned a message (like adjustment error)
                            TempData["ErrorMessage"] = Result.StatusText;
                            //return View(model);
                            //return View(model);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error while  transaction.";
                        }
                    }
                    model.DateIntact = "Y";
                    model.FromDate = modelFrmDate;
                    model.ToDate = modelToDate;
                    return RedirectToAction("PendingMRNtoQc", "PendingMRNtoQC");
                }
            }
            catch (Exception ex)
            {
                LogException<MIRController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> MIR(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string ItemName = "", string MRNNO = "", string VendorName = "", string INVNo = "", string Deptname = "", string PartCode = "", string MRNJW = "")//, ILogger logger)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new MirModel();
            //_MemoryCache.Remove("KeyMIRGrid");
            HttpContext.Session.Remove("KeyMIRGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IMirModule.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode= YC;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                //_MemoryCache.Set("KeyMIRGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyMIRGrid", serializedGrid);

            }
            else
            {
                ViewData["Title"] = "Quality Module Details";
                TempData.Clear();
                //_MemoryCache.Remove("KeyMIRGrid");
                HttpContext.Session.Remove("KeyMIRGrid");
                var MainModel1 = new MirModel();
                MainModel1 = await BindModel(MainModel1);
                MainModel1.FromDateBack = FromDate;
                MainModel1.ToDateBack = ToDate;
                MainModel1.VendorNameBack = VendorName;
                MainModel1.InvNoBack = INVNo;
                MainModel1.PartCodeBack = PartCode;
                MainModel1.ItemNameBack = ItemName;
                MainModel1.DeptNameBack = Deptname;
                MainModel1.MRNJWBack = MRNJW;
                MainModel1.MRNNoBack = MRNNO;
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                //_MemoryCache.Set("KeyMIRGrid", MainModel1.ItemDetailGrid, cacheEntryOptions);
                string serializedGrid = JsonConvert.SerializeObject(MainModel1.ItemDetailGrid);
                HttpContext.Session.SetString("KeyMIRGrid", serializedGrid);


                MainModel1.DateIntact = "N";
                MainModel1.FromPend = "Y";
                if (MainModel1.Mode != "U")
                {
                    MainModel1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    MainModel1.CreatedOn = DateTime.Now;
                    MainModel1.CC = HttpContext.Session.GetString("Branch");
                   MainModel1.YearCode  =  Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                }
                //_MemoryCache.TryGetValue("KeyMIRGrid", out List<MirDetail> MIRDetail);

               string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
                List<MirDetail> MIRDetail = new List<MirDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MIRDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
                }

                return View(MainModel1);
            }
            //MainModel.DateIntact = "N";
            //MainModel.FromPend = "Y";
            if (MainModel.Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.ItemNameBack = ItemName;
            MainModel.VendorNameBack = VendorName;
            MainModel.MRNNoBack = MRNNO;

            return View(MainModel);
        }
        public async Task<IActionResult> MIRDashboard(string Flag="True",string FromDate = "", string ToDate = "", string VendorName = "", string MRNNo = "", string GateNo = "", string MIRNo = "", string ItemName = "", string PartCode = "", string DashboardType = "", string Searchbox = "")
        {
            try
            {
                //_MemoryCache.Remove("KeyMIRGrid");
                HttpContext.Session.Remove("KeyMIRGrid");
                var model = new MIRQDashboard();
                var Result = await _IMirModule.GetDashboardData().ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        //                     mm.Account_Name 'VendorName',mm.MIRNo, mm.MIRDate, mm.MRNNO, mm.MRNDate, mm.GateNo,
                        //mm.MRNJWCustJW, mm.INVNo, mm.Invdate,
                        //mm.EntryId, mm.EntryDate, mm.YearCode
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "MRNNO", "MRNDate", "VendorName", "MIRNo",
                            "MIRDate", "GateNo", "MRNJWCustJW", "INVNo", "Invdate", "EntryId", "YearCode", "EntryDate", "PurchaseBillBooked", "MaterialIssued", "EnteredBy", "UpdatedBy");
                        model.MIRQDashboard = CommonFunc.DataTableToList<MIRDashboard>(DT, "MIRDATA");
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["MRNNO"].ToString(),
                                Value = row["MRNNO"].ToString()
                            });
                        }
                        var _MRNListD = _List.DistinctBy(x => x.Value).ToList();
                        model.MRNList = _MRNListD;
                        _List = new List<TextValue>();
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["GateNo"].ToString(),
                                Value = row["GateNo"].ToString()
                            });
                        }
                        var _GateList = _List.DistinctBy(x => x.Value).ToList();
                        model.GateList = _GateList;
                        _List = new List<TextValue>();
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["MIRNo"].ToString(),
                                Value = row["MIRNo"].ToString()
                            });
                        }
                        var _MIRList = _List.DistinctBy(x => x.Value).ToList();
                        model.MIRList = _MIRList;
                        _List = new List<TextValue>();
                    }
                }
                if (Flag != "True")
                {
                    model.FromDate = FromDate;
                    model.ToDate = ToDate;
                    model.VendorName=VendorName;
                    model.ItemName=ItemName;
                    model.PartCode=PartCode;
                    model.MRNNo=MRNNo;
                    model.MIRNo=MIRNo;
                    model.Searchbox=Searchbox;
                    model.DashboardType=DashboardType;
                    return View(model);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetNewEntry()
        {
            int YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IMirModule.GetNewEntry("GETNEWENTRY", YearCode, "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AllowUpdelete(int EntryId, string YearCode)
        {
            var JSON = await _IMirModule.AllowUpdelete(EntryId, YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private static DataTable GetDetailTable(IList<MirDetail> DetailList)
        {
            var MIRGrid = new DataTable();
            MIRGrid.Columns.Add("SeqNo", typeof(int));
            MIRGrid.Columns.Add("PONo", typeof(string));
            MIRGrid.Columns.Add("POYearCode", typeof(int));
            MIRGrid.Columns.Add("SchNo", typeof(string));
            MIRGrid.Columns.Add("SchYearCode", typeof(int));
            MIRGrid.Columns.Add("ItemCode", typeof(int));
            MIRGrid.Columns.Add("Unit", typeof(string));
            MIRGrid.Columns.Add("AltUnit", typeof(string));
            MIRGrid.Columns.Add("BillQty", typeof(decimal));
            MIRGrid.Columns.Add("RecQty", typeof(decimal));
            MIRGrid.Columns.Add("AltRecQty", typeof(decimal));
            MIRGrid.Columns.Add("AcceptedQty", typeof(decimal));
            MIRGrid.Columns.Add("AltAcceptedQty", typeof(decimal));
            MIRGrid.Columns.Add("OkRecStore", typeof(int));
            MIRGrid.Columns.Add("DeviationQty", typeof(decimal));
            MIRGrid.Columns.Add("ResponsibleEmpForDeviation", typeof(int));
            MIRGrid.Columns.Add("RejectedQty", typeof(decimal));
            MIRGrid.Columns.Add("AltRejectedQty", typeof(decimal));
            MIRGrid.Columns.Add("RejRecStore", typeof(int));
            MIRGrid.Columns.Add("Remarks", typeof(string));
            MIRGrid.Columns.Add("Defaulttype", typeof(string));
            MIRGrid.Columns.Add("ApprovedByEmp", typeof(int));
            MIRGrid.Columns.Add("HoldQty", typeof(decimal));
            MIRGrid.Columns.Add("HoldStoreId", typeof(int));
            MIRGrid.Columns.Add("ProcessId", typeof(int));
            MIRGrid.Columns.Add("Reworkqty", typeof(decimal));
            MIRGrid.Columns.Add("RewokStoreId", typeof(int));
            MIRGrid.Columns.Add("Color", typeof(string));
            MIRGrid.Columns.Add("ItemSize", typeof(string));
            MIRGrid.Columns.Add("ResponsibleFactor", typeof(string));
            MIRGrid.Columns.Add("SupplierBatchno", typeof(string));
            MIRGrid.Columns.Add("shelfLife", typeof(decimal));
            MIRGrid.Columns.Add("BatchNo", typeof(string));
            MIRGrid.Columns.Add("uniqueBatchno", typeof(string));
            MIRGrid.Columns.Add("AllowDebitNote", typeof(string));
            MIRGrid.Columns.Add("Rate", typeof(decimal));
            MIRGrid.Columns.Add("rateinother", typeof(decimal));
            MIRGrid.Columns.Add("PODate", typeof(DateTime));
           // MIRGrid.Columns.Add("ItemColor", typeof(string));
            MIRGrid.Columns.Add("FilePath", typeof(string));
            MIRGrid.Columns.Add("MRNNO", typeof(string));
            MIRGrid.Columns.Add("MRNYearCode", typeof(int));
            MIRGrid.Columns.Add("MRNJWCUSTJW", typeof(string));

            foreach (var Item in DetailList)
            {
                MIRGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo == 0 ? 0 : Item.SeqNo,
                    Item.PONo ?? "",
                    Item.POYearCode== 0 ? 0 : Item.POYearCode,
                    Item.SchNo == null?"":Item.SchNo,
                    Item.SchYearCode== 0 ? 0 : Item.SchYearCode,
                    Item.ItemCode== 0 ? 0 : Item.ItemCode,
                    Item.Unit ?? "",
                    (Item.AltUnit  == null || Item.AltUnit == "undefined")?"":Item.AltUnit,
                    Item.BillQty,
                    Item.RecQty,
                    Item.ALtRecQty,
                    Item.AcceptedQty,
                    Item.AltAcceptedQty,
                    Item.OkRecStore,
                    Item.DeviationQty,
                    Item.ResponsibleEmpForDeviation,
                    Item.RejectedQty,
                    Item.AltRejectedQty,
                    Item.RejRecStore,
                    Item.Remarks ?? "",
                    Item.DefaultType ?? "",
                    Item.ApprovedByEmp,
                    Item.HoldQty,
                    Item.HoldStoreId,
                    Item.ProcessId,
                    Item.ReworkQty,
                    Item.RewokStoreId,
                    Item.Color ?? "",
                    Item.ItemSize ?? "",
                    Item.ResponsibleFactor ?? "",
                    Item.SupplierBatchNo ?? "",
                    Item.ShelfLife,
                    Item.BatchNo ?? "",
                    Item.UniqueBatchNo ?? "",
                    Item.AllowDebitNote ?? "",
                    Item.Rate == 0?0:Item.Rate,
                    Item.RateInOtherCurr == 0 ? 0 :Item.RateInOtherCurr,
                    DateTime.Today,
                   // Item.Itemcolor,
                    Item.PathOfFileURL??"",
                    Item.MRNNo ?? "",
                    Item.MRNYearCode == 0 ? 0 : Item.MRNYearCode,
                    Item.MRNJwCust ?? ""
                    });
            }
            MIRGrid.Dispose();
            return MIRGrid;
        }
        private async Task<MirModel> BindModel(MirModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IMirModule.BindBranch("BINDBRANCH");

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
            }
            return model;
        }
        public async Task<IActionResult> AddToMIRGrid2(List<IFormFile> model)
        {
            int i = 0;
            foreach (IFormFile file in model)
            {
                string ImagePath = "Uploads/MRIR/";

                if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "//" + ImagePath))
                {
                    Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "//" + ImagePath);
                }

                ImagePath += Guid.NewGuid().ToString() + "_" + file.FileName;
                ImageArray.Add(ImagePath);

                string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                {
                    await file.CopyToAsync(FileStream);
                }
                i++;
            }
            return Ok(ImageArray);
        }
        public IActionResult AddToMIRGrid(List<MirDetail> model, List<string> ImageList)
        {

            try
            {

                //_MemoryCache.TryGetValue("KeyMIRGrid", out IList<MirDetail> MirDetail);
                string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
                List<MirDetail> MirDetail = new List<MirDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MirDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
                }


                var MainModel = new MirModel();
                var MIRModuleGrid = new List<MirDetail>();
                var MIRGrid = new List<MirDetail>();
                var SSGrid = new List<MirDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var Seqno = 0;
                int i = 0;
                int k = 0;
                foreach (var item in model)
                {
                    if (item != null)
                    {
                        if (MirDetail == null)
                        {
                            item.SeqNo = Seqno + 1;
                            if (item.OkRecStoreName == "-Select-")
                            {
                                item.OkRecStoreName = "NA";
                            }
                            if (item.RejRecStoreName == "-Select-")
                            {
                                item.RejRecStoreName = "NA";
                            }
                            if (item.HoldStoreName == "-Select-")
                            {
                                item.HoldStoreName = "NA";
                            }
                            if (item.RewokStoreName == "-Select-")
                            {
                                item.RewokStoreName = "NA";
                            }
                            //for image path
                            if (item.PathOfFile != null)
                            {
                                string ImagePath = "Uploads/MRIR/";

                                string filename = Path.GetFileName(item.PathOfFile);
                                ImagePath += Guid.NewGuid().ToString() + "_" + filename;
                                if (ImageList.Count > 0)
                                {
                                    item.PathOfFileURL = "/" + ImageList[i];
                                }
                                else
                                {
                                    item.PathOfFileURL = item.PathOfFile;
                                }
                                i++;
                            }


                            MIRGrid.Add(item);
                            Seqno++;
                        }
                        else
                        {
                            if (MirDetail.Where(x => x.PartCode == item.PartCode && x.PONo == item.PONo && x.POYearCode == item.POYearCode && x.SchNo == item.SchNo && x.SchYearCode == item.SchYearCode && x.BatchNo == item.BatchNo && x.UniqueBatchNo == item.UniqueBatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                item.SeqNo = MirDetail.Count + 1;
                                if (item.OkRecStoreName == "-Select-")
                                {
                                    item.OkRecStoreName = "NA";
                                }
                                if (item.RejRecStoreName == "-Select-")
                                {
                                    item.RejRecStoreName = "NA";
                                }
                                if (item.HoldStoreName == "-Select-")
                                {
                                    item.HoldStoreName = "NA";
                                }
                                if (item.RewokStoreName == "-Select-")
                                {
                                    item.RewokStoreName = "NA";
                                }
                                MIRGrid = MirDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(MIRGrid);
                                //for image path
                                if (item.PathOfFile != null)
                                {
                                    if (ImageList.Count == 0)
                                    {
                                        //do nothing
                                        item.PathOfFileURL = item.PathOfFile;
                                    }
                                    else
                                    {
                                        string ImagePath = "Uploads/MRIR/";

                                        string filename = Path.GetFileName(item.PathOfFile);
                                        ImagePath += Guid.NewGuid().ToString() + "_" + filename;
                                        item.PathOfFileURL = "/" + ImageList[k];
                                        k++;
                                    }
                                }
                                MIRGrid.Add(item);
                            }
                        }

                        MainModel.ItemDetailGrid = MIRGrid;
                        //_MemoryCache.Set("KeyMIRGrid", MainModel.ItemDetailGrid, cacheEntryOptions);

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyMIRGrid", serializedGrid);

                    }
                }
                return PartialView("_MIRGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new MirModel();
            //_MemoryCache.TryGetValue("KeyMIRGrid", out List<MirDetail> MirDetail);

            string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
            List<MirDetail> MirDetail = new List<MirDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MirDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (MirDetail != null && MirDetail.Count > 0)
            {
                MirDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in MirDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = MirDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (MirDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyMIRGrid");
                    //_MemoryCache.Remove("KeyMIRGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_MIRGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            //_MemoryCache.TryGetValue("KeyMIRGrid", out IList<MirDetail> MirDetail);
            string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
            List<MirDetail> MirDetail = new List<MirDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MirDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
            }

            var SSGrid = MirDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteByItemCode(int ItemCode,int SeqNo)
        {
            var MainModel = new MirModel();
            //_MemoryCache.TryGetValue("KeyMIRGridOnLoad", out List<MirDetail> MirDetail);

            string modelJson = HttpContext.Session.GetString("KeyMIRGrid");
            List<MirDetail> MirDetail = new List<MirDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MirDetail = JsonConvert.DeserializeObject<List<MirDetail>>(modelJson);
            }
            List<MirDetail> SSGrid = MirDetail.Where(x => x.ItemCode == ItemCode).ToList();
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (MirDetail != null && MirDetail.Count > 0)
            {
               // foreach(var )
                MirDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in MirDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetail = MirDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (MirDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyMIRGridOnLoad");
                    //_MemoryCache.Remove("KeyMIRGridOnLoad");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_MIRDetailGrid", MainModel);
            //_MemoryCache.TryGetValue("KeyMIRGrid", out IList<MirDetail> MirDetail);
            //var SSGrid = MirDetail.Where(x => x.ItemCode == ItemCode);
            //string JsonString = JsonConvert.SerializeObject(SSGrid);
            //return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IMirModule.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOkRecStore(int ItemCode,string ShowAllStore, string GateNo)
        {
            var JSON = await _IMirModule.GetOkRecStore(ItemCode, ShowAllStore, GateNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNNo(string FromDate, string ToDate, string MRNCustJW)
        {
            var JSON = await _IMirModule.GetMRNNo("PENDINGMRNFORMIR", "SPMIR", FromDate, ToDate, MRNCustJW);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> AddPassWord()
        {
            var JSON = await _IMirModule.AddPassWord();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateData(string MRNNo, string MRNYearCode, string MRNCustJW)
        {
            var JSON = await _IMirModule.GetGateData("GateMRNData", "SPMIR", MRNNo, MRNYearCode, MRNCustJW);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNData(string MRNNo, int MRNYearCode, int GateNo, int GateYear, int GateEntryId, string MRNCustJW)
        {
            var JSON = await _IMirModule.GetMRNData("MRNMAINDATA", "SPMIR", MRNNo, MRNYearCode, GateNo, GateYear, GateEntryId, MRNCustJW);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetMIRMainItem(string MRNNo, int MRNYearCode, int GateNo, int GateYear, int GateEntryId, string MRNCustJW, int start = 0, int pageSize = 0)
        {
            //_MemoryCache.Remove("KeyMIRGridOnLoad");
            HttpContext.Session.Remove("KeyMIRGridOnLoad");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var model = await _IMirModule.GetMIRMainItem("MIRMAINITEM", "SPMIR", MRNNo, MRNYearCode, GateNo, GateYear, GateEntryId, MRNCustJW);
            //_MemoryCache.Set("KeyMIRGridOnLoad", model.ItemDetail, cacheEntryOptions);
            string serializedGrid = JsonConvert.SerializeObject(model.ItemDetail);
            HttpContext.Session.SetString("KeyMIRGridOnLoad", serializedGrid);

            if (model.ItemDetail != null)
            {
                model.ItemDetail = model.ItemDetail.ToList();
                model.MirTotalRows= model.ItemDetail.Count; 
                model.ItemDetail = model.ItemDetail
                            .AsEnumerable()
                            .Skip(start)
                            .Take(pageSize)
                            .ToList();
            }
            return PartialView("_MIRDetailGrid", model);
        }
        //public async Task<JsonResult> GetMIRMainItem(string MRNNo, int MRNYearCode, int GateNo, int GateYear, int GateEntryId,string MRNCustJW,int start=0,int pageSize=0)
        //{
        //    var JSON= await _IMirModule.GetMIRMainItem("MIRMAINITEM", "SPMIR", MRNNo, MRNYearCode, GateNo, GateYear, GateEntryId, MRNCustJW);
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetMIRMain([FromBody] List<MirDetail> mirDetail)
        //{
        //    var model = new MirModel();
        //    model.ItemDetail = mirDetail;
        //    return PartialView("_MIRDetailGrid", model);
        //}
        public async Task<IActionResult> GetSearchData(string VendorName, string MrnNo, string GateNo,string MirNo,string ItemName, string FromDate, string ToDate)
        {
            var model = new MIRQDashboard();
            model = await _IMirModule.GetSearchData(VendorName, MrnNo,GateNo,MirNo, ItemName, FromDate, ToDate);
            model.DashboardType = "Summary";
            return PartialView("_MIRDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string VendorName, string MrnNo, string GateNo, string MirNo,string ItemName, string FromDate, string ToDate)
        {
            var model = new MIRQDashboard();
            model = await _IMirModule.GetDashboardDetailData(VendorName, MrnNo,GateNo,MirNo, ItemName, FromDate, ToDate);
            model.DashboardType = "Detail";
            return PartialView("_MIRDashboardGrid", model);
        }
        public JsonResult FillGridFromMemoryCache()
        {
            try
            {
                //_MemoryCache.TryGetValue("KeyPendingMRNToQC", out IList<MIRFromPend> grid);

                string modelJson = HttpContext.Session.GetString("KeyPendingMRNToQC");
                IList<MIRFromPend> grid = new List<MIRFromPend>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    grid = JsonConvert.DeserializeObject<IList<MIRFromPend>>(modelJson);
                }


                //_MemoryCache.TryGetValue("KeyMIRGridFromMRN", out IList<MIRFromPend> grid2);

                string modelJson1 = HttpContext.Session.GetString("KeyMIRGridFromMRN");
                IList<MIRFromPend> grid2 = new List<MIRFromPend>();
                if (!string.IsNullOrEmpty(modelJson1))
                {
                    grid2 = JsonConvert.DeserializeObject<IList<MIRFromPend>>(modelJson1);
                }
                //_MemoryCache.TryGetValue("KeyIssWOBom", out IList<IssueWithoutBomDetail> IssueWithoutBomDetailGrid);

                string modelJson2 = HttpContext.Session.GetString("KeyIssWOBom");
                IList<IssueWithoutBomDetail> IssueWithoutBomDetailGrid = new List<IssueWithoutBomDetail>();
                if (!string.IsNullOrEmpty(modelJson2))
                {
                    IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<IList<IssueWithoutBomDetail>>(modelJson2);
                }
                var MainModel = new MirModel();
                var IssueGrid = new List<IssueWithoutBomDetail>();
                var SSGrid = new List<IssueWithoutBomDetail>();


                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                if (grid != null)
                {
                    for (int i = 0; i < grid.Count; i++)
                    {
                        MainModel.MRNNo = grid[i].MRNNo;
                        MainModel.MRNYearCode = grid[i].MRNYear;
                        MainModel.MRNJWCustJW = grid[i].MRNJW;
                        MainModel.FromDate = grid[i].FromDate;
                        MainModel.ToDate = grid[i].ToDate;
                        MainModel.GateNo = grid[i].GateNo;

                    }
                }
                //_MemoryCache.Remove("KeyIssWOBom");
                HttpContext.Session.Remove("KeyIssWOBom");

                return Json(MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetStore()
        {
            var JSON = await _IMirModule.GetStore("RejSTORE", "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetRewStore()
        {
            var JSON = await _IMirModule.GetRewStore("RewSTORE", "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetHoldStore()
        {
            var JSON = await _IMirModule.GetHoldStore("HoldSTORE", "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetRecOkStore(int ItemCode)
        {
            var JSON = await _IMirModule.GetRecOkStore(ItemCode,"FillOkRecStore", "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEmployeeList()
        {
            var JSON = await _IMirModule.GetEmployeeList("BINDEMP", "SPMIR");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC,string FromDate="",string ToDate="",string VendorName="",string MRNNo="",string GateNo="",string MIRNo="",string ItemName="",string PartCode="",string DashboardType="",string Searchbox="")
        {
            var Result = await _IMirModule.DeleteByID(ID, YC);

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

            return RedirectToAction("MIRDashboard", new {Flag = "False", FromDate = FromDate, ToDate = ToDate, VendorName = VendorName, ItemName = ItemName, PartCode = PartCode, MRNNo = MRNNo, GateNo = GateNo, MIRNO = MIRNo, Searchbox = Searchbox, DashboardType = DashboardType });
        }
        public async Task<JsonResult> CheckEditOrDelete(int EntryId, int YearCode)
        {
            var JSON = await _IMirModule.CheckEditOrDelete(EntryId, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpPost]

		public async Task<JsonResult> GenerateBarCodeTag(string MIRNo, int YearCode, string ItemCodes)
		{
			var JSON = await _IMirModule.GenerateBarCodeTag(MIRNo, YearCode, ItemCodes);

			// if SP saved successfully, build PrintReport URL
			if (JSON != null && JSON.Result != null && JSON.Result.Tables.Count > 0 && JSON.Result.Tables[0].Rows.Count > 0)
			{
				// Example: take EntryId from your SP result (adjust column name accordingly)
				int entryId = Convert.ToInt32(JSON.Result.Tables[0].Rows[0]["EntryId"]);

				// Build PrintReport URL
				string reportUrl = Url.Action("PrintBarcodeTag", "MIR", new
				{
					MIRNo = MIRNo,
					YearCode = YearCode,
					ItemCodes = ItemCodes
                }, protocol: Request.Scheme);

                return Json(new { url = reportUrl });// return URL to AJAX
            }

			return Json(null);
		 }
		public IActionResult PrintBarcodeTag(string MIRNo, int YearCode, string ItemCodes)
		{
			string my_connection_string;
			string contentRootPath = _IWebHostEnvironment.ContentRootPath;
			string webRootPath = _IWebHostEnvironment.WebRootPath;
			var webReport = new WebReport();
			webReport.Report.Clear();
			var ReportName = _IMirModule.GetReportName();
			webReport.Report.Dispose();
			webReport.Report = new Report();
			
				webReport.Report.Load(webRootPath + "\\MIRGenerateIncomingBarcode.frx"); 
			

            my_connection_string = _connectionStringService.GetConnectionString();
			//my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
			webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
			webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
			webReport.Report.SetParameterValue("MRINoparam", MIRNo);
			webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
			webReport.Report.SetParameterValue("Itemcodesparam", ItemCodes);
			webReport.Report.SetParameterValue("MyParameter", my_connection_string);
			webReport.Report.Refresh();
			return View(webReport);
		}
		//public async Task<JsonResult> GenerateBarCodeTag(string MIRNo, int YearCode, string ItemCodes)
		//{
		//    var JSON = await _IMirModule.GenerateBarCodeTag(MIRNo, YearCode, ItemCodes);
		//    string JsonString = JsonConvert.SerializeObject(JSON);
		//    return Json(JsonString);
		//}
	}
}
