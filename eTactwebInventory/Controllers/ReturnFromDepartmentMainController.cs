using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Globalization;
using System.Data;

namespace eTactWeb.Controllers
{
    public class ReturnFromDepartmentMainController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ILogger<ReturnFromDepartmentMainController> _logger;
        private readonly IRetFromDepartmentMain _IRetFromDepartmentMain;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;

        public ReturnFromDepartmentMainController(IDataLogic iDataLogic, ILogger<ReturnFromDepartmentMainController> logger, IMemoryCache memoryCache, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, IRetFromDepartmentMain iRetFromDepartmentMain)
        {
            _IDataLogic = iDataLogic;
            _logger = logger;
            _MemoryCache = memoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
            _IRetFromDepartmentMain = iRetFromDepartmentMain;
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string Type = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            //webReport.Report.Load(webRootPath + "\\returnFromDeptMain.frx"); // TODO
            webReport.Report.SetParameterValue("RetFromDepEntryId", EntryId);
            webReport.Report.SetParameterValue("RetFromDepYearCode", YearCode);
            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            return View(webReport);
        }
        public async Task<IActionResult> Dashboard(string FromDate, string Todate, string Flag, string REQNo = "", string WCName = "", string WONo = "", string DepName = "", string PartCode = "", string ItemName = "")
        {
            try
            {
                _MemoryCache.Remove("KeyRetFromDeptMainGrid");

                var model = new RetFromDepMainDashboard
                {
                    Mode = "Summary",
                    CC = HttpContext.Session.GetString("Branch")
                };

                var Result = await _IRetFromDepartmentMain.GetDashboardData(FromDate, Todate, Flag).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "RetFromDepEntryId", "RetFromDepEntrydate", "RetFromDepSlipNo", "RetFromDepActualReturnDate", "RetFromDepYearCode", "EntryByMachineName", "ActualEntryDate");
                        model.RetFromDeptMainDashboard = CommonFunc.DataTableToList<RDMDashboard>(DT, "RDMDashboard");
                    }
                }

                if (Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;
                    model.REQNo = REQNo;
                    model.WorkCenter = WCName;
                    model.WONo = WONo;
                    model.PartCode = PartCode;
                    model.ItemName = ItemName;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                _logger.LogError(ex, "Error occurred while fetching dashboard data");
                throw;
            }
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> ReturnFromDepartmentMain(int ID, string Mode, int YC, string REQNo = "", string ItemName = "", string PartCode = "", string WorkCenter = "", string WONo = "", string DeptName = "", string DashboardType = "", string FromDate = "", string ToDate = "", string GlobalSearch = "")
        {
            ViewData["Title"] = "Return From Department Main Detail";
            TempData.Clear();
            var MainModel = new ReturnFromDepartmentMainModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            _MemoryCache.Remove("KeyRetFromDeptMainGrid");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IRetFromDepartmentMain.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyRetFromDeptMainGrid", MainModel.ReturnDetailGrid, cacheEntryOptions);
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.CreatedBy = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
            }
            return View(MainModel);
        }

        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReturnFromDepartmentMain(ReturnFromDepartmentMainModel model)
        {
            try
            {
                var ReqGrid = new DataTable();
                var mainmodel2 = model;
                _MemoryCache.TryGetValue("KeyRetFromDeptMainGrid", out List<ReturnFromDepartmentDetail> RequisitionDetail);
                mainmodel2.ReqDetailGrid = RequisitionDetail;
                if (RequisitionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReturnFromDepartmentMain", "ReturnFromDepartmentMain Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("ReturnFromDepartmentMain", model);
                }
                else
                {
                    //model.CreatedBy = Constants.UserID;
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    }
                    ReqGrid = GetDetailTable(RequisitionDetail, model.Mode);
                    var Result = await _IRetFromDepartmentMain.SaveRetFromDeptMain(model, ReqGrid);

                    if (Result != null)
                    {
                        if ((Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK) || (Result.StatusText == "Completed is Y" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var MainModel = new ReturnFromDepartmentMainModel();
                            MainModel = await BindModel(MainModel);
                            _MemoryCache.Remove("KeyRetFromDeptMainGrid");
                            return RedirectToAction(nameof(ReturnFromDepartmentMain));
                        }
                        if ((Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted) || (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var MainModel = new ReturnFromDepartmentMainModel();
                            MainModel = await BindModel(MainModel);
                            _MemoryCache.Remove("KeyRetFromDeptMainGrid");
                            return RedirectToAction(nameof(ReturnFromDepartmentMain));
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
                LogException<ReturnFromDepartmentMainController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        private static DataTable GetDetailTable(IList<ReturnFromDepartmentDetail> DetailList, string mode)
        {
            var ReqGrid = new DataTable();

            ReqGrid.Columns.Add("SeqNo", typeof(long));
            ReqGrid.Columns.Add("RetFromDepEntryId", typeof(long));
            ReqGrid.Columns.Add("RetFromDepYearCode", typeof(long));
            ReqGrid.Columns.Add("ItemCode", typeof(int));
            ReqGrid.Columns.Add("Qty", typeof(float));
            ReqGrid.Columns.Add("ReturnDate", typeof(DateTime));
            ReqGrid.Columns.Add("BatchNo", typeof(string));
            ReqGrid.Columns.Add("UniqueBatchNo", typeof(string));
            ReqGrid.Columns.Add("ProductValue", typeof(float));
            ReqGrid.Columns.Add("ItemIdentMark", typeof(string));
            ReqGrid.Columns.Add("ReasonOfReturn", typeof(string));
            ReqGrid.Columns.Add("IsItemDamaged", typeof(string));
            ReqGrid.Columns.Add("DamageDetail", typeof(string));
            ReqGrid.Columns.Add("ReasonOfDamage", typeof(string));
            ReqGrid.Columns.Add("ItemRemark", typeof(string));
            ReqGrid.Columns.Add("ImageOfProduct1", typeof(string));
            ReqGrid.Columns.Add("ImageOfProduct2", typeof(string));
            ReqGrid.Columns.Add("ImageOfProduct3", typeof(string));
            ReqGrid.Columns.Add("ImageOfProduct4", typeof(string));
            ReqGrid.Columns.Add("ReceivedByEmpId", typeof(long));
            ReqGrid.Columns.Add("Approve", typeof(string));
            ReqGrid.Columns.Add("ApprovalDate", typeof(DateTime));
            ReqGrid.Columns.Add("PurchRate", typeof(float));
            ReqGrid.Columns.Add("CurrentRate", typeof(float));

            foreach (var Item in DetailList)
            {
                string returnnDate = (mode != "U" && !string.IsNullOrEmpty(Item.ReturnnDate))
                    ? DateTime.ParseExact(Item.ReturnnDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd")
                    : Item.ReturnnDate;

                ReqGrid.Rows.Add(
                    new object[]
                    {
                        Item.SeqNo,
                        Item.EntryId,
                        Item.YearCode,
                        Item.ItemCode,
                        Item.Qty,
                        returnnDate,
                        Item.BatchNo,
                        Item.UniqueBatchNo,
                        Item.ProdValue,
                        "tet",
                        Item.ReasonOfReturn,
                        Item.Damaged,
                        Item.DamageDetail,
                        Item.DamageDetail,
                        Item.IDMark,
                        Item.Pic1,
                        Item.Pic2,
                        Item.Pic3,
                        "image1.jpg",
                        1,
                        "Yes",
                        returnnDate,
                        1,
                        1,
                    });
            }

            return ReqGrid;
        }

        private async Task<ReturnFromDepartmentMainModel> BindModel(ReturnFromDepartmentMainModel model)
        {
            model.DepartmentList = await _IDataLogic.GetDropDownList("FillDepartment", "SP_ReturnFromDepartmentMainDetail");
            model.EmployeeList = await _IDataLogic.GetDropDownList("FillEmployeeName", "SP_ReturnFromDepartmentMainDetail");
            model.AssetPartCodeList = await _IDataLogic.GetDropDownList("FillPartCode", "SP_ReturnFromDepartmentMainDetail");
            model.AssetItemsList = await _IDataLogic.GetDropDownList("FillItem", "SP_ReturnFromDepartmentMainDetail");
            model.BatchNoList = await _IDataLogic.GetDropDownList("FillBatch", "SP_ReturnFromDepartmentMainDetail");
            model.UniqueBatchNoList = await _IDataLogic.GetDropDownList("FillUniqueBatchNo", "SP_ReturnFromDepartmentMainDetail");
            return model;
        }

        public IActionResult AddRetFromDeptMainDetail(ReturnFromDepartmentDetail model)
        {
            try
            {
                // Retrieve the existing grid data from memory cache
                _MemoryCache.TryGetValue("KeyRetFromDeptMainGrid", out IList<ReturnFromDepartmentDetail> GridDetail);

                var MainModel = new ReturnFromDepartmentMainModel();
                var SSGrid = new List<ReturnFromDepartmentDetail>();

                // If no data is found in the cache, initialize with the new item
                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        model.SeqNo = 1;
                        if (model.PartCode == "-Select-")
                        {
                            model.PartCode = "NA";
                        }
                        if (model.ItemName == "-Select-")
                        {
                            model.ItemName = "NA";
                        }
                        SSGrid.Add(model);  // Add the new item to the grid
                    }
                    else
                    {
                        // If data exists, check for duplicates
                        if (GridDetail.Any(x => x.SeqNo == model.SeqNo))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            // Update item values
                            if (model.PartCode == "-Select-")
                            {
                                model.PartCode = "NA";
                            }
                            if (model.ItemName == "-Select-")
                            {
                                model.ItemName = "NA";
                            }
                            model.SeqNo = GridDetail.Count + 1;

                            // Add existing grid data and new item to SSGrid
                            SSGrid.AddRange(GridDetail);  // Keep existing items
                            SSGrid.Add(model);            // Add the new item
                        }
                    }

                    // Set the updated grid data to the MainModel
                    MainModel.ReturnDetailGrid = SSGrid;

                    // Update memory cache with the new grid data
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    _MemoryCache.Set("KeyRetFromDeptMainGrid", SSGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                // Return the updated grid view
                return PartialView("_ReturnFromDepartmentMainGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> DeleteItemRow(int SeqNo)
        {
            var MainModel = new ReturnFromDepartmentMainModel();
            _MemoryCache.TryGetValue("KeyRetFromDeptMainGrid", out List<ReturnFromDepartmentDetail> RequisitionDetail);
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
                    _MemoryCache.Remove("KeyRetFromDeptMainGrid");
                }
            }
            return PartialView("_ReturnFromDepartmentMainGrid", MainModel);
        }

        public IActionResult EditItemRow(int SeqNo)
        {
            var model = new ReturnFromDepartmentMainModel();
            _MemoryCache.TryGetValue("KeyRetFromDeptMainGrid", out List<ReturnFromDepartmentDetail> ReturnFromDepartmentDetail);
            var SSGrid = ReturnFromDepartmentDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillItems(string returnByEmpName, string showAllItem)
        {
            var JSON = await _IRetFromDepartmentMain.FillItems(returnByEmpName, showAllItem);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string returnByEmpName)
        {
            var JSON = await _IRetFromDepartmentMain.FillPartCode(returnByEmpName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillBatchNo(int itemCode, int receiveByEmp, DateTime? retFromDepEntrydate, int retFromDepYearCode)
        {
            if (!retFromDepEntrydate.HasValue)
            {
                retFromDepEntrydate = DateTime.Now;
            }
            var JSON = _IRetFromDepartmentMain.FillBatchNo(itemCode, receiveByEmp, retFromDepEntrydate, retFromDepYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetNewEntry(int yearCode)
        {
            var JSON = await _IRetFromDepartmentMain.GetNewEntry(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate, string ToDate)
        {
            var Result = await _IRetFromDepartmentMain.DeleteByID(ID, YC);
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


            return RedirectToAction("Dashboard", new { FromDate, ToDate, Flag = "True" });
        }
    }
}
