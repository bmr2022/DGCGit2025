using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class MRPController : Controller
    {
        public IDataLogic IDataLogic { get; }
        public IMemoryCache IMemoryCache { get; }
        public IMRP IMRP { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<MRPController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        public MRPController(IMRP _IMRP, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<MRPController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            IMRP = _IMRP;
            IDataLogic = iDataLogic;
            IMemoryCache = iMemoryCache;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> MRP(int ID, string Mode, int YC)
        {
            //RoutingModel model = new RoutingModel();
            ViewData["Title"] = "MRP Details";
            TempData.Clear();
            IMemoryCache.Remove("KeyMRPDetail");
            IMemoryCache.Remove("KeyMRPFGDetail");
            IMemoryCache.Remove("KeyMRPSODetail");
            //IMemoryCache.Remove("keyAddedMRPGrid");
            var MainModel = new MRPMain();

            //MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            //MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            if (Mode != "U")
            {
                MainModel.UID = HttpContext.Session.GetString("UID");
                MainModel.CreatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IMRP.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //MainModel = await BindModel(MainModel);

            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeyMRPDetail", MainModel.MRPGrid, cacheEntryOptions);
            IMemoryCache.Set("KeyMRPFGDetail", MainModel.MRPFGRMGrid, cacheEntryOptions);
            IMemoryCache.Set("KeyMRPSODetail", MainModel.MRPSOGrid, cacheEntryOptions);
            //IMemoryCache.Set("keyAddedMRPGrid", MainModel.MRPGrid, cacheEntryOptions);
            //IMemoryCache.Set("KeyStockAdjustGrid", model, cacheEntryOptions);
            return View(MainModel);
        }


        public async Task<IActionResult> DeleteByID(int ID, int YC, string MRPNo, string EntryByMachineName, int CreatedByEmpId)
        {
            var Result = await IMRP.DeleteByID(ID, YC, MRPNo,  EntryByMachineName,  CreatedByEmpId).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var dt = time.ToString(format);
                return Json(formattedDate);
                //string apiUrl = "https://worldtimeapi.org/api/ip";

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log any other unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> MRP(MRPMain model)
        {
            try
            {
                var MRPGrid = new DataTable();
                var MRPSOGrid = new DataTable();
                var MRPFGGrid = new DataTable();

                //IMemoryCache.TryGetValue("keyAddedMRPGrid", out IList<MRPDetail> MRPDetail);
                IMemoryCache.TryGetValue("KeyMRPDetail", out IList<MRPDetail> MRPDetail);
                IMemoryCache.TryGetValue("KeyMRPSODetail", out IList<MRPSaleOrderDetail> MRPSODetail);
                IMemoryCache.TryGetValue("KeyMRPFGDetail", out IList<MRPFDRMDetail> MRPFGDetail);

                if (MRPDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("MRPDetail", "MRP Grid Should Have Atleast 1 Item...!");
                    return View("MRP", model);
                }
                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.UID = HttpContext.Session.GetString("UID");
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                        MRPGrid = GetDetailTable(MRPDetail);
                        MRPSOGrid = GetSODetailTable(MRPSODetail);
                        //MRPFGGrid = GetFGDetailTable(MRPFGDetail);
                    }
                    else
                    {
                        model.CreatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.CreatedByEmpName = HttpContext.Session.GetString("EmpName");
                        model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        MRPGrid = GetDetailTable(MRPDetail);
                        MRPSOGrid = GetSODetailTable(MRPSODetail);
                        //MRPFGGrid = GetFGDetailTable(MRPFGDetail);
                    }

                    var Result = await IMRP.SaveMRPDetail(model, MRPGrid, MRPSOGrid, MRPFGGrid=null);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            IMemoryCache.Remove(MRPGrid);
                            IMemoryCache.Remove(MRPSOGrid);
                            var model1 = new MRPMain();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("keyAddedMRPGrid");
                            return View(model1);
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new MRPMain();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("keyAddedMRPGrid");
                            return View(model1);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModel(null);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.CC = HttpContext.Session.GetString("Branch");
                                model2.CreatedByEmpName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View(model2);
                            }

                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            // return View("Error", Result);
                            return View(model);
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<MRPController>.WriteException(Logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
                //return View(model);
            }
        }

        public async Task<IActionResult> Dashboard()
         {
            IMemoryCache.Remove("keyAddedMRPGrid");
            var model = new MRPDashboard();
            DateTime now = DateTime.Now;

            model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

            var Result = await IMRP.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryID", "Yearcode",
                    "Entrydate", "MRPNo", "MRPDate", "MRPRevNo", "CreatedByEmpId", "MRPREvDate",
                                "ForMonth", "EffectiveFromDate", "MrpFirstDate", "MrpComplete",
                                "UID", "CC", "ActualEntryDate", "ActualEnteredBy", "EntryByMachineName",
                                "LastUpdatedBy", "LastUpdatedDate");
                model.MRPDahboardGrid = CommonFunc.DataTableToList<MRPDashboard>(DT, "MRP");

            }

            return View(model);
        }

        public async Task<IActionResult> GetSearchData(MRPDashboard model)
        {
            var Result = await IMRP.GetDashboardData(model);
            DataSet DS = Result.Result;
            var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryID", "Yearcode",
                    "Entrydate", "MRPNo", "MRPDate", "MRPRevNo", "CreatedByEmpId", "MRPREvDate",
                                "ForMonth", "EffectiveFromDate", "MrpFirstDate", "MrpComplete",
                                "UID", "CC", "ActualEntryDate", "ActualEnteredBy", "EntryByMachineName",
                                "LastUpdatedBy", "LastUpdatedDate");

            model.MRPDahboardGrid = CommonFunc.DataTableToList<MRPDashboard>(DT, "MRP");

            return PartialView("_MRPDashboardGrid", model);
        }

        private async Task<MRPMain> BindModel(MRPMain model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            //oDataSet = await IStockAdjust.BindAllDropDowns("BINDALLDROPDOWN");

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {

                //foreach (DataRow row in oDataSet.Tables[1].Rows)
                //{
                //    _List.Add(new TextValue
                //    {
                //        Value = row["entryid"].ToString(),
                //        Text = row["StageDescription"].ToString()
                //    });
                //}
                //model.StageList = _List;
                //_List = new List<TextValue>();


            }
            return model;
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

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }

        private static DataTable GetDetailTable(IList<MRPDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("Entryid", typeof(int));
            DTSSGrid.Columns.Add("Yearcode", typeof(int));
            DTSSGrid.Columns.Add("FGItemCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("RMItemCode", typeof(int));
            DTSSGrid.Columns.Add("CurrMonthOrderQty", typeof(float));
            DTSSGrid.Columns.Add("OrderQtyInclPrevPOQty", typeof(float));
            DTSSGrid.Columns.Add("IIndMonthQty", typeof(float));
            DTSSGrid.Columns.Add("IIIrdMonthQty", typeof(float));
            DTSSGrid.Columns.Add("StoreStock", typeof(float));
            DTSSGrid.Columns.Add("WIPStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("MinLevel", typeof(float));
            DTSSGrid.Columns.Add("ReorderLvl", typeof(float));
            DTSSGrid.Columns.Add("PrevOrderQty", typeof(float));
            DTSSGrid.Columns.Add("ReqQty", typeof(float));
            DTSSGrid.Columns.Add("IIndMonthActualReqQty", typeof(float));
            DTSSGrid.Columns.Add("IIIrdMonthActualReqQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("AllocatedQty", typeof(float));
            DTSSGrid.Columns.Add("ConsumedQty", typeof(float));
            DTSSGrid.Columns.Add("PORate", typeof(float));
            DTSSGrid.Columns.Add("LeadTime", typeof(int));
            DTSSGrid.Columns.Add("BOMExist", typeof(string));
            DTSSGrid.Columns.Add("POExist", typeof(string));
            DTSSGrid.Columns.Add("MaxPORate", typeof(float));
            DTSSGrid.Columns.Add("AccountCode", typeof(string));
            DTSSGrid.Columns.Add("AccountName", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.FGItemCode,
                    Item.SeqNo,
                    Item.RMItemCode,
                    Item.CurrMonthQty,
                    Item.OrderQtyInclPrevPOQty == null ? 0 : Item.OrderQtyInclPrevPOQty,
                    Item.IIndMonthQty,
                    Item.IIrdMonthQty,
                    Item.StoreStock,
                    Item.WIPStock,
                    Item.TotalStock,
                    Item.MinLevel,
                    Item.RecorderLvl,
                    Item.PrevOrderQty,
                    Item.ReqQty,
                    Item.IIndMonthActualReqQty,
                    Item.IIrdMonthActualReqQty,
                    Item.Unit, // this is uniqbatchno
                    Item.AltUnit ?? "",
                    Item.AllocatedQty ,
                    Item.ConsumedQty,
                    Item.PORate,
                    Item.LeadTime,
                    Item.BOMExist ?? "",
                    Item.POExist ?? "",
                    Item.MaxPORate == 0 ? 0.00 : Item.MaxPORate,
                    Item.AccountCode,
                    Item.AccountName ?? ""
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }


        private static DataTable GetSODetailTable(IList<MRPSaleOrderDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("EntryId", typeof(int));
            DTSSGrid.Columns.Add("YearCode", typeof(int));
            DTSSGrid.Columns.Add("SONo", typeof(string));
            DTSSGrid.Columns.Add("SOYearCode", typeof(int));
            DTSSGrid.Columns.Add("SODAte", typeof(string));
            DTSSGrid.Columns.Add("ScheduleNo", typeof(string));
            DTSSGrid.Columns.Add("SchYearCode", typeof(int));
            DTSSGrid.Columns.Add("ProjNo", typeof(int));
            DTSSGrid.Columns.Add("ProjYearCode", typeof(int));
            DTSSGrid.Columns.Add("AccountCode", typeof(int));
            DTSSGrid.Columns.Add("Months", typeof(string));
            DTSSGrid.Columns.Add("MonthYear", typeof(int));
            DTSSGrid.Columns.Add("BOMExist", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                var SODate = ParseDate(Item.SODAte);
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.SONo ?? "",
                    Item.SOYearCode,
                    SODate.ToString("yyyy/MM/dd") ?? "",
                    Item.ScheduleNo ?? "",
                    Item.SchYearCode,
                    Item.ProjNo,
                    Item.ProjYearCode,
                    Item.AccountCode,
                    Item.Months ?? "",
                    Item.MonthYear,
                    Item.BOMExist ?? "",
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        private static DataTable GetFGDetailTable(IList<MRPFDRMDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("Entryid", typeof(int));
            DTSSGrid.Columns.Add("Yearcode", typeof(int));
            DTSSGrid.Columns.Add("FGItemCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("RMItemCode", typeof(int));
            DTSSGrid.Columns.Add("BOMNo", typeof(int));
            DTSSGrid.Columns.Add("CurrMonthOrderQty", typeof(float));
            DTSSGrid.Columns.Add("OrderQtyInclPrevPOQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("AllocatedQty", typeof(float));
            DTSSGrid.Columns.Add("FGQty", typeof(float));
            DTSSGrid.Columns.Add("BOMQty", typeof(float));
            DTSSGrid.Columns.Add("BOMEffDate", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                var BomEffDt = ParseDate(Item.BOMEffDate);
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.FGItemCode,
                    Item.SeqNo,
                    Item.RMItemCode,
                    Item.BOMNo,
                    Item.CurrMonthOrderQty,
                    Item.OrderQtyInclPrevPOQty,
                    Item.AltUnit ?? "",
                    Item.AllocatedQty,
                    Item.FGQty,
                    Item.BOMQty,
                    BomEffDt.ToString("yyyy/MM/dd") ?? "",
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new MRPMain();
            IMemoryCache.TryGetValue("keyAddedMRPGrid", out List<MRPDetail> MRPGrid);
            var MRPDetail = MRPGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(MRPDetail);
            return Json(JsonString);
        }

        //public IActionResult DeleteItemRow(int SeqNo, string Mode)
        //{
        //    var MainModel = new MRPMain();
        //    if (Mode == "U")
        //    {
        //        int Indx = Convert.ToInt32(SeqNo) - 1;
        //        IMemoryCache.TryGetValue("keyAddedMRPGrid", out List<MRPDetail> MRPDetail);

        //        if (MRPDetail != null && MRPDetail.Count > 0)
        //        {
        //            MRPDetail.RemoveAt(Convert.ToInt32(Indx));

        //            Indx = 0;

        //            foreach (var item in MRPDetail)
        //            {
        //                Indx++;
        //                item.SeqNo = Indx;
        //            }
        //            MainModel.MRPGrid = MRPDetail;

        //            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //                SlidingExpiration = TimeSpan.FromMinutes(55),
        //                Size = 1024,
        //            };

        //            IMemoryCache.Set("keyAddedMRPGrid", MainModel.MRPGrid, cacheEntryOptions);
        //        }
        //    }
        //    else
        //    {
        //        IMemoryCache.TryGetValue("keyAddedMRPGrid", out List<MRPDetail> MRPDetail);
        //        int Indx = Convert.ToInt32(SeqNo) - 1;

        //        if (MRPDetail != null && MRPDetail.Count > 0)
        //        {
        //            MRPDetail.RemoveAt(Convert.ToInt32(Indx));

        //            Indx = 0;

        //            foreach (var item in MRPDetail)
        //            {
        //                Indx++;
        //                item.SeqNo = Indx;
        //            }
        //            MainModel.MRPGrid = MRPDetail;

        //            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //                SlidingExpiration = TimeSpan.FromMinutes(55),
        //                Size = 1024,
        //            };

        //            IMemoryCache.Set("keyAddedMRPGrid", MainModel.MRPGrid, cacheEntryOptions);
        //        }
        //    }
        //    return PartialView("_MRPGrid", MainModel);
        //}

        public IActionResult FillMRPGridDetail()
        {
            try
            {
                IMemoryCache.TryGetValue("KeyPendingMRPToMRPDetail", out IList<MRPDetail> PendingMRPDetail);
                var model = new MRPDetail();
                var MainModel = new MRPMain();
                var MRPGrid = new List<MRPDetail>();
                var MRPdetailGrid = new List<MRPDetail>();


                foreach (var item in PendingMRPDetail)
                {
                    IMemoryCache.TryGetValue("KeyMRPDetail", out IList<MRPDetail> MRPDetail);

                    if (MRPDetail == null)
                    {
                        model.SeqNo = 1;
                        MRPGrid.Add(item);
                    }
                    else
                    {
                        model.SeqNo = MRPDetail.Count + 1;
                        //   MRPGrid = PendingMRPDetail.Where(x => x != null).ToList();
                        MRPdetailGrid.AddRange(MRPGrid);
                        MRPGrid.Add(item);

                    }

                    MainModel.MRPGrid = MRPGrid;
                    MainModel.EntryID = item.MRPEntryId;
                    MainModel.MRPNo = item.MRPNo;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyMRPDetail", MainModel.MRPGrid, cacheEntryOptions);
                }
                return PartialView("_MRPGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult FillFGMRPDetailGrid()
        {
            try
            {
                IMemoryCache.TryGetValue("KeyPendingFGRMMRP", out IList<MRPFDRMDetail> PendingMRPFGDetail);
                var model = new MRPFDRMDetail();
                var MainModel = new MRPMain();
                var MRPGrid = new List<MRPFDRMDetail>();
                var MRPdetailGrid = new List<MRPFDRMDetail>();



                foreach (var item in PendingMRPFGDetail)
                {
                    IMemoryCache.TryGetValue("KeyMRPFGDetail", out IList<MRPFDRMDetail> MRPDetail);

                    if (MRPDetail == null)
                    {
                        model.SeqNo = 1;
                        MRPGrid.Add(item);
                    }
                    else
                    {
                        model.SeqNo = MRPDetail.Count + 1;
                        //   MRPGrid = PendingMRPDetail.Where(x => x != null).ToList();
                        MRPdetailGrid.AddRange(MRPGrid);
                        MRPGrid.Add(item);

                    }

                    MainModel.MRPFGRMGrid = MRPGrid;
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyMRPFGDetail", MainModel.MRPFGRMGrid, cacheEntryOptions);
                }
                return PartialView("_MRPFGGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult FillSaleOrderMRPGrid(string Month, int ForMonthYear)
        {
            try
            {
                IMemoryCache.TryGetValue("KeyMRPSaleOrderDetail", out IList<MRPSaleOrderDetail> MRPSODetail);
                var model = new MRPSaleOrderDetail();
                var MainModel = new MRPMain();
                var MRPGrid = new List<MRPSaleOrderDetail>();
                var MRPSOdetailGrid = new List<MRPSaleOrderDetail>();



                foreach (var item in MRPSODetail)
                {
                    IMemoryCache.TryGetValue("KeyMRPSODetail", out IList<MRPDetail> MRPDetail);
                    item.Months = Month;
                    item.MonthYear = ForMonthYear;

                    if (MRPDetail == null)
                    {
                        model.SeqNo = 1;
                        MRPGrid.Add(item);
                    }
                    else
                    {
                        model.SeqNo = MRPDetail.Count + 1;
                        //   MRPGrid = PendingMRPDetail.Where(x => x != null).ToList();
                        MRPSOdetailGrid.AddRange(MRPGrid);
                        MRPGrid.Add(item);

                    }

                    MainModel.MRPSOGrid = MRPGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyMRPSODetail", MainModel.MRPSOGrid, cacheEntryOptions);
                }
                return PartialView("_MRPSOGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await IMRP.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await IMRP.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}

