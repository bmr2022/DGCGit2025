using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using eTactWeb.Controllers;

namespace eTactwebInventory.Controllers
{
    public class ReOfferItemController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IReofferItem _IReofferItem { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<ReOfferItemController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        public ReOfferItemController(IReofferItem iReofferItem, IConfiguration configuration, IDataLogic iDataLogic, ILogger<ReOfferItemController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _IReofferItem = iReofferItem;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReOfferItem(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "ReOfferItem";
            TempData.Clear();
            HttpContext.Session.Remove("KeyReOfferItemGrid");
            var MainModel = new ReOfferItemModel();

            MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString().Replace("-", "/");
            MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString().Replace("-", "/");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ReofferYearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                //MainModel = await _IDeassembleItem.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                

            }
          
            
            if (Mode != "U")
            {

                MainModel.EnteredByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.EnteredByEmpName = HttpContext.Session.GetString("EmpName");

            }
            else
            {

                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");

            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
            HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.SlipNoBack = SlipNo;
            MainModel.BatchNoback = BatchNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.DashboardTypeBack = SummaryDetail;
            MainModel.GlobalSearchBack = Searchbox;
            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReOfferItem(ReOfferItemModel model)
        {
            try
            {
                var ISTGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                if (ISTDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReofferItemDetail", "ReofferItem Grid Should Have Atleast 1 Item...!");
                    return View("ReOfferItem", model);
                }
                else
                {


                    ISTGrid = GetDetailTable(ISTDetail);
                    var Result = await _IReofferItem.SaveReoffer(model, ISTGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyReOfferItemGrid");
                            return RedirectToAction(nameof(ReOfferItem));
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("KeyReOfferItemGrid");
                            return RedirectToAction(nameof(ReOfferItem));
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["2627"] = "2627";
                            return RedirectToAction(nameof(ReOfferItem));
                        }
                    }
                    return RedirectToAction(nameof(ReOfferItem));
                }
            }
            catch (Exception ex)
            {
                LogException<ReOfferItemController>.WriteException(Logger, ex);


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
        private static string SafeString(string input, int maxLength)
        {
            return string.IsNullOrEmpty(input) ? "" : input.Length > maxLength ? input.Substring(0, maxLength) : input;
        }
        private static DataTable GetDetailTable(IList<ReofferItemDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("ReofferEntryId", typeof(int));          // bigint
            DTSSGrid.Columns.Add("ReofferYearcode", typeof(int));
            DTSSGrid.Columns.Add("MIREntryId", typeof(int));
            DTSSGrid.Columns.Add("MIRYearCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("PONo", typeof(string));                 // nvarchar(100)
            DTSSGrid.Columns.Add("POYearCode", typeof(int));
            DTSSGrid.Columns.Add("SchNo", typeof(string));                // nvarchar(100)
            DTSSGrid.Columns.Add("SchYearCode", typeof(int));
            DTSSGrid.Columns.Add("MRNno", typeof(string));                // nvarchar(200)
            DTSSGrid.Columns.Add("mrnYearCode", typeof(int));            // nullable, consider nullable long if needed
            DTSSGrid.Columns.Add("MRNJWCUSTJW", typeof(string));          // nullable nvarchar(100)
            DTSSGrid.Columns.Add("Itemcode", typeof(int));
            DTSSGrid.Columns.Add("Unit", typeof(string));                  // nvarchar(3)
            DTSSGrid.Columns.Add("AltUnit", typeof(string));               // nvarchar(3)
            DTSSGrid.Columns.Add("BillQty", typeof(float));               // float
            DTSSGrid.Columns.Add("RecQty", typeof(float));
            DTSSGrid.Columns.Add("AltRecQty", typeof(float));
            DTSSGrid.Columns.Add("PrevAcceptedQty", typeof(float));
            DTSSGrid.Columns.Add("AcceptedQty", typeof(float));
            DTSSGrid.Columns.Add("AltAcceptedQty", typeof(float));
            DTSSGrid.Columns.Add("OkRecStore", typeof(int));
            DTSSGrid.Columns.Add("DeviationQty", typeof(float));
            DTSSGrid.Columns.Add("ResponsibleEmpForDeviation", typeof(int));
            DTSSGrid.Columns.Add("PreviousRejectedQty", typeof(double));
            DTSSGrid.Columns.Add("RejectedQty", typeof(float));
            DTSSGrid.Columns.Add("AltRejectedQty", typeof(float));
            DTSSGrid.Columns.Add("RejRecStore", typeof(int));
            DTSSGrid.Columns.Add("Remarks", typeof(string));              // nvarchar(50)
            DTSSGrid.Columns.Add("Defaulttype", typeof(string));           // nvarchar(50)
            DTSSGrid.Columns.Add("ApprovedByEmp", typeof(int));
            DTSSGrid.Columns.Add("PreviousHoldQty", typeof(float));
            DTSSGrid.Columns.Add("HoldQty", typeof(float));
            DTSSGrid.Columns.Add("HoldStoreId", typeof(int));
            DTSSGrid.Columns.Add("ProcessId", typeof(int));
            DTSSGrid.Columns.Add("PreviousReworkqty", typeof(float));
            DTSSGrid.Columns.Add("Reworkqty", typeof(float));
            DTSSGrid.Columns.Add("RewokStoreId", typeof(int));
            DTSSGrid.Columns.Add("Color", typeof(string));                 // nvarchar(100)
            DTSSGrid.Columns.Add("ItemSize", typeof(string));              // nvarchar(100)
            DTSSGrid.Columns.Add("ResponsibleFactor", typeof(string));     // nvarchar(15)
            DTSSGrid.Columns.Add("SupplierBatchno", typeof(string));       // nvarchar(100)
            DTSSGrid.Columns.Add("shelfLife", typeof(decimal));             // decimal(10,2)
            DTSSGrid.Columns.Add("BatchNo", typeof(string));                // nvarchar(100)
            DTSSGrid.Columns.Add("uniqueBatchno", typeof(string));          // nvarchar(100)
            DTSSGrid.Columns.Add("AllowDebitNote", typeof(string));         // nvarchar(1)
            DTSSGrid.Columns.Add("Rate", typeof(float));
            DTSSGrid.Columns.Add("rateinother", typeof(float));
            DTSSGrid.Columns.Add("PODate", typeof(string));
            DTSSGrid.Columns.Add("FilePath", typeof(string));               // nvarchar(200)
            DTSSGrid.Columns.Add("schdate", typeof(string));              // nullable? If yes, handle DBNull.Value when adding rows

            int seqNo = 1;
            foreach (var Item in DetailList)
            {
                try
                {
                    string uniqueString = Guid.NewGuid().ToString();
                    DTSSGrid.Rows.Add(
                        new object[]
                        {
                 0,
            0,
            0,
            0,
            seqNo++,
            SafeString(Item.PONo, 100),
            Item.POYearCode ?? 0,
            SafeString(Item.SchNo, 100),
            Item.SchYearCode ?? 0,
            SafeString("", 200),
            0,
            SafeString("", 100),
            Item.Itemcode ?? 0,
            SafeString(Item.Unit, 3),
            SafeString(Item.AltUnit, 3),
            Item.BillQty ?? 0,
            Item.RecQty ?? 0,
            Item.AltRecQty ?? 0,
            Item.PrevAcceptedQty ?? 0,
            Item.AcceptedQty ?? 0,
            Item.AltAcceptedQty ?? 0,
            Item.OkRecStore ?? 0,
            Item.DeviationQty ?? 0,
            Item.ResponsibleEmpForDeviation ?? 0,
            Item.PreviousRejectedQty ?? 0,
            Item.RejectedQty ?? 0,
            Item.AltRejectedQty ?? 0,
            Item.RejRecStore ?? 0,
            SafeString(Item.Remarks, 50),
            SafeString(Item.Defaulttype, 50),
            Item.ApprovedByEmp ?? 0,
            Item.PreviousHoldQty ?? 0,
            Item.HoldQty ?? 0,
            Item.HoldStoreId ?? 0,
            Item.ProcessId ?? 0,
            Item.PreviousReworkqty ?? 0,
            Item.Reworkqty ?? 0,
            Item.RewokStoreId ?? 0,
            SafeString(Item.Color, 100),
            SafeString(Item.ItemSize, 100),
            SafeString(Item.ResponsibleFactor, 15),
            SafeString(Item.SupplierBatchno, 100),
            Item.shelfLife ?? 0,
            SafeString(Item.BatchNo, 100),
            SafeString(Item.uniqueBatchno, 100),
            SafeString(Item.AllowDebitNote, 1),
            Item.Rate ?? 0,
            Item.rateinother ?? 0,
            CommonFunc.ParseFormattedDate( Item.PODate ?? ""),
            SafeString(Item.FilePath, 200),
            CommonFunc.ParseFormattedDate( Item.schdate ?? ""),
                     


                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error adding row: PO No: {Item.PONo}, Itemcode: {Item.Itemcode}, Error: {ex.Message}");
                    throw; // rethrow to stop execution, or comment this out to continue and skip bad rows
                }
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }


        public async Task<JsonResult> GETNEWENTRY(int ReofferYearcode)
        {
            var JSON = await _IReofferItem.GETNEWENTRY( ReofferYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLQCTYPE()
        {
            var JSON = await _IReofferItem.FILLQCTYPE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLMIRNO(string ReofferMir)
        {
            var JSON = await _IReofferItem.FILLMIRNO(ReofferMir);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLMIRYearCode(string MIRNO)
        {
            var JSON = await _IReofferItem.FILLMIRYearCode(MIRNO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FILLMIRData(string MIRNO, int MIRYearCode)
        {
            var JSON = await _IReofferItem.FILLMIRData(MIRNO,  MIRYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BINDSTORE()
        {
            var JSON = await _IReofferItem.BINDSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetItemDeatil(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            var JSON = await _IReofferItem.GetItemDeatil( MIRNO,  MIRYearCode,  accountcode,  ReofferMir);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItemData(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            var JSON = await _IReofferItem.GetItemData(MIRNO, MIRYearCode, accountcode, ReofferMir);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetItemQty(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir, int itemcode)
        {
            var JSON = await _IReofferItem.GetItemQty(MIRNO, MIRYearCode, accountcode, ReofferMir, itemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillOkRecStore(int itemcode, string ShowAllStore)
        {
            var JSON = await _IReofferItem.FillOkRecStore( itemcode,  ShowAllStore);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> ALLOWSHOWALLSTORE()
        {
            var JSON = await _IReofferItem.ALLOWSHOWALLSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> RejSTORE()
        {
            var JSON = await _IReofferItem.RejSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> RewSTORE()
        {
            var JSON = await _IReofferItem.RewSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> HoldSTORE()
        {
            var JSON = await _IReofferItem.HoldSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BINDEMP()
        {
            var JSON = await _IReofferItem.BINDEMP();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPODetail(string MIRNO, int MIRYearCode, int accountcode, int itemcode)
        {
            var JSON = await _IReofferItem.FillPODetail( MIRNO,  MIRYearCode,  accountcode,  itemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddReOfferDetail(ReofferItemDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                var MainModel = new ReOfferItemModel();
                var ISTGrid = new List<ReofferItemDetail>();
                var ISTList = new List<ReofferItemDetail>();

                if (model != null)
                {
                    if (ISTDetail == null)
                    {
                        // model.SeqNo = 1;
                        ISTGrid.Add(model);
                    }
                    else
                    {
                        if (ISTDetail.Any(x => x.Itemcode == model.Itemcode))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {

                            ISTGrid = ISTDetail.Where(x => x != null).ToList();
                            ISTList.AddRange(ISTGrid);
                            ISTGrid.Add(model);
                        }
                    }

                    ISTGrid = ISTGrid.OrderBy(item => item.SeqNo).ToList();
                    MainModel.ReofferItemDetail = ISTGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                return PartialView("_ReOfferItemGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddReOfferDetail1(List<ReofferItemDetail> model)
        {
            try
            {
                var MainModel = new ReOfferItemModel();
                var RCGrid = new List<ReofferItemDetail>();
                var ReceiveChallanGrid = new List<ReofferItemDetail>();

                var SeqNo = 0;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                    IList<ReofferItemDetail> RCDetail = new List<ReofferItemDetail>();
                    if (modelJson != null)
                    {
                        RCDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                    }

                    if (model != null)
                    {
                        if (RCDetail == null)
                        {
                            //item.SeqNo += SeqNo + 1;
                            RCGrid.Add(item);
                        }
                        else
                        {
                            if (RCDetail.Any(x => x.Itemcode == item.Itemcode))
                            {
                                //return StatusCode(207, "Duplicate");
                                var duplicateInfo = new
                                {
                                    item.Itemcode,
                                    

                                };
                            }
                            else
                            {
                                //item.SeqNo = RCDetail.Count + 1;
                                RCGrid = RCDetail.Where(x => x != null).ToList();
                                ReceiveChallanGrid.AddRange(RCGrid);
                                RCGrid.Add(item);
                            }
                        }
                        RCGrid = RCGrid.OrderBy(item => item.SeqNo).ToList();
                        MainModel.ReofferItemDetail = RCGrid;

                        HttpContext.Session.SetString("KeyReOfferItemGrid", JsonConvert.SerializeObject(MainModel.ReofferItemDetail));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Receive Challan List Cannot Be Empty...!");
                    }
                }


                return PartialView("_ReOfferItemGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new DeassembleItemModel();
            string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
            List<ReofferItemDetail> ReofferItemDetail = new List<ReofferItemDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                ReofferItemDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
            }
            var ISTGrid = ReofferItemDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(ISTGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ReOfferItemModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;

                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.ReofferItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.ReofferItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
            }
            return PartialView("_ReOfferItemGrid", MainModel);
        }
        [HttpGet]
        [Route("ReOfferItemDashBoard")]
        public async Task<IActionResult> ReOfferItemDashBoard()
        {
            try
            {
                var model = new ReOfferItemModel();
                var result = await _IReofferItem.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.reofferdashboard = CommonFunc.DataTableToList<ReOfferItemModel>(dt, "ReOfferItem");
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate, string ReportType)
        {
            //model.Mode = "Search";
            var model = new ReOfferItemModel();
            model = await _IReofferItem.GetDashBoardDetailData(FromDate, ToDate, ReportType);
            if (ReportType == "SUMMARY")
            {
                return PartialView("_DashBoardSummaryGrid", model);
            }
            else
            {
                return PartialView("_DashBoardDetailGrid", model);
            }
            return null;

        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName, string SummaryDetail, string FromDate = "", string ToDate = "", string SlipNo = "", string PartCode = "", string ItemName = "", string BatchNo = "")
        {
            var Result = await _IReofferItem.DeleteByID(ID, YC, EntryDate, ActualEntryBy, MachineName).ConfigureAwait(false);

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
            return RedirectToAction("ReOfferItemDashBoard", new { Flag = "false", FromDate = FromDate, ToDate = ToDate, SlipNo = SlipNo, PartCode = PartCode, ItemName = ItemName, BatchNo = BatchNo, SummaryDetail = SummaryDetail });
        }
        public async Task<IActionResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var JSON = await _IReofferItem.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}
