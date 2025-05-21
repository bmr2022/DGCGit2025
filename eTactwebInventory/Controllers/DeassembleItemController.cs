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
    public class DeassembleItemController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IDeassembleItem _IDeassembleItem { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<DeassembleItemController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        public DeassembleItemController(IDeassembleItem iDeassembleItem, IConfiguration configuration, IDataLogic iDataLogic, ILogger<DeassembleItemController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _IDeassembleItem = iDeassembleItem;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> DeassembleItem(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "DeassembleItem";
            TempData.Clear();
            HttpContext.Session.Remove("KeyDeassembleItemGrid");
            var MainModel = new DeassembleItemModel();

            MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString().Replace("-", "/");
            MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString().Replace("-", "/");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.DeassYearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IDeassembleItem.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //  MainModel = await BindModel(MainModel);

            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }
            if (Mode != "U")
            {

                MainModel.CreatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");

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

            string serializedGrid = JsonConvert.SerializeObject(MainModel.DeassembleItemDetail);
            HttpContext.Session.SetString("KeyDeassembleItemGrid", serializedGrid);

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
        public async Task<IActionResult> DeassembleItem(DeassembleItemModel model)
        {
            try
            {
                var ISTGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyDeassembleItemGrid");
                List<DeassembleItemDetail> ISTDetail = new List<DeassembleItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<DeassembleItemDetail>>(modelJson);
                }

                if (ISTDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("DeassembleItemDetail", "DeassembleItem Grid Should Have Atleast 1 Item...!");
                    return View("DeassembleItem", model);
                }
                else
                {


                    ISTGrid = GetDetailTable(ISTDetail);
                    var Result = await _IDeassembleItem.SaveDeassemble(model, ISTGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyDeassembleItemGrid");
                            return RedirectToAction(nameof(DeassembleItem));
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("KeyDeassembleItemGrid");
                            return RedirectToAction(nameof(DeassembleItem));
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["2627"] = "2627";
                            return RedirectToAction(nameof(DeassembleItem));
                        }
                    }
                    return RedirectToAction(nameof(DeassembleItem));
                }
            }
            catch (Exception ex)
            {
                LogException<DeassembleItemController>.WriteException(Logger, ex);


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

        private static DataTable GetDetailTable(IList<DeassembleItemDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("DeassEntryID", typeof(int));
            DTSSGrid.Columns.Add("DeassYearCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("RMItemCode", typeof(int));
            DTSSGrid.Columns.Add("BomQty", typeof(float));
            DTSSGrid.Columns.Add("DeassQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("RMStoreId", typeof(int));
            DTSSGrid.Columns.Add("Remark", typeof(string));
            DTSSGrid.Columns.Add("RMBatchNo", typeof(string));
            DTSSGrid.Columns.Add("RmUniqueBatchNo", typeof(string));
            DTSSGrid.Columns.Add("IdealDeassQty", typeof(float));
            //DateTime DeliveryDt = new DateTime();
            int seqNo = 0;
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                   seqNo++,
                    Item.RMItemCode,

                    Item.BomQty,
                    Item.DeassQty,

                    Item.RMUnit ?? "",
                    Item.RMStoreId,
                    Item.Remark,
                    Item.RMBatchNo ?? "",
                    Item.RmUniqueBatchNo?? "",
                    Item.IdealDeassQty ,

                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }




        public async Task<JsonResult> NewEntryId()
        {
            var JSON = await _IDeassembleItem.NewEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> BomQty(int RMItemCode)
        {
            var JSON = await _IDeassembleItem.BomQty(RMItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IDeassembleItem.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillFGItemName()
        {
            var JSON = await _IDeassembleItem.FillFGItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFGPartCode()
        {
            var JSON = await _IDeassembleItem.FillFGPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBomNo(int FinishItemCode)
        {
            var JSON = await _IDeassembleItem.FillBomNo(FinishItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillMRNNO(int FGItemCode, int yearcode)
        {
            var JSON = await _IDeassembleItem.FillMRNNO(FGItemCode, yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillMRNYearCode(int FGItemCode, int yearcode, string MRNNO)
        {
            var JSON = await _IDeassembleItem.FillMRNYearCode(FGItemCode, yearcode, MRNNO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillMRNDetail(int yearcode, string MRNNO, int mrnyearcode)
        {
            var JSON = await _IDeassembleItem.FillMRNDetail(yearcode, MRNNO, mrnyearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName(int FinishItemCode, int BomNo)
        {
            var JSON = await _IDeassembleItem.FillRMItemName(FinishItemCode, BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode(int FinishItemCode, int BomNo)
        {
            var JSON = await _IDeassembleItem.FillRMPartCode(FinishItemCode, BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IDeassembleItem.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        public IActionResult AddDeassembleDetail(DeassembleItemDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyDeassembleItemGrid");
                List<DeassembleItemDetail> ISTDetail = new List<DeassembleItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<DeassembleItemDetail>>(modelJson);
                }

                var MainModel = new DeassembleItemModel();
                var ISTGrid = new List<DeassembleItemDetail>();
                var ISTList = new List<DeassembleItemDetail>();

                if (model != null)
                {
                    if (ISTDetail == null)
                    {
                        // model.SeqNo = 1;
                        ISTGrid.Add(model);
                    }
                    else
                    {
                        if (ISTDetail.Any(x => x.RMItemCode == model.RMItemCode && x.RMBatchNo == model.RMBatchNo))
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
                    MainModel.DeassembleItemDetail = ISTGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.DeassembleItemDetail);
                    HttpContext.Session.SetString("KeyDeassembleItemGrid", serializedGrid);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                return PartialView("_DeassembleItemGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new DeassembleItemModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;

                string modelJson = HttpContext.Session.GetString("KeyDeassembleItemGrid");
                List<DeassembleItemDetail> ISTDetail = new List<DeassembleItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<DeassembleItemDetail>>(modelJson);
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
                    MainModel.DeassembleItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.DeassembleItemDetail);
                    HttpContext.Session.SetString("KeyDeassembleItemGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyDeassembleItemGrid");
                List<DeassembleItemDetail> ISTDetail = new List<DeassembleItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<DeassembleItemDetail>>(modelJson);
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
                    MainModel.DeassembleItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.DeassembleItemDetail);
                    HttpContext.Session.SetString("KeyDeassembleItemGrid", serializedGrid);
                }
            }
            return PartialView("_DeassembleItemGrid", MainModel);
        }

        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new DeassembleItemModel();
            string modelJson = HttpContext.Session.GetString("KeyDeassembleItemGrid");
            List<DeassembleItemDetail> DeassembleItemDetail = new List<DeassembleItemDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                DeassembleItemDetail = JsonConvert.DeserializeObject<List<DeassembleItemDetail>>(modelJson);
            }
            var ISTGrid = DeassembleItemDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(ISTGrid);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName, string SummaryDetail, string FromDate = "", string ToDate = "", string SlipNo = "", string PartCode = "", string ItemName = "", string BatchNo = "")
        {
            var Result = await _IDeassembleItem.DeleteByID(ID, YC, EntryDate, ActualEntryBy, MachineName).ConfigureAwait(false);

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
            return RedirectToAction("DeassembleDashBoard", new { Flag = "false", FromDate = FromDate, ToDate = ToDate, SlipNo = SlipNo, PartCode = PartCode, ItemName = ItemName, BatchNo = BatchNo, SummaryDetail = SummaryDetail });
        }


        [HttpGet]
        [Route("DeassembleDashBoard")]
        public async Task<IActionResult> DeassembleDashBoard()
        {
            try
            {
                var model = new DeassembleItemDashBoard();
                var result = await _IDeassembleItem.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.DeassembleItemDashBoardDetail = CommonFunc.DataTableToList<DeassembleItemDashBoard>(dt, "DeassembleItem");
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
            var model = new DeassembleItemDashBoard();
            model = await _IDeassembleItem.GetDashBoardDetailData(FromDate, ToDate, ReportType);
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





    }
}
