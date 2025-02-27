using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using static eTactWeb.DOM.Models.MirModel;

namespace eTactWeb.Controllers
{
    public class PendingInProcesstoQcController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingInProcessToQc _IPendProcessToQC;
        private readonly ILogger<PendingInProcesstoQcController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingInProcesstoQcController(ILogger<PendingInProcesstoQcController> logger, IDataLogic iDataLogic, IPendingInProcessToQc IPendProcessToQC, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendProcessToQC = IPendProcessToQC;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        public async Task<IActionResult> PendingInProcesstoQc()
        {
            ViewData["Title"] = "Pending In Process to QC Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyPendingProcessToQC");
            var MainModel = new PendingInProcessToQc();
            var model = new ProductionEntryModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel = await BindItem(MainModel);
            MainModel = await BindProdSlip(MainModel);
            MainModel = await BindWorkCenter(MainModel);
            //MainModel.CC = HttpContext.Session.GetString("Branch");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyPendingProcessToQC", model, cacheEntryOptions);
            return View(MainModel);
        }
        private async Task<PendingInProcessToQc> BindItem(PendingInProcessToQc model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IPendProcessToQC.BindItem("BindItem");
            model.ItemNameList = new List<TextValue>();
            model.PartCodeList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.PartCodeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.ItemNameList = _List;
                _List = new List<TextValue>();

            }
            return model;
        }
        private async Task<PendingInProcessToQc> BindProdSlip(PendingInProcessToQc model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IPendProcessToQC.BindProdSlip("BindProdSlip");
            model.ProdSlipList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["ProdSlipNo"].ToString(),
                        Text = row["ProdSlipNo"].ToString()
                    });
                }
                model.ProdSlipList = _List;
                _List = new List<TextValue>();

            }
            return model;
        }
        private async Task<PendingInProcessToQc> BindWorkCenter(PendingInProcessToQc model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IPendProcessToQC.BindWorkCenter("BindWorkCenter");
            model.WorkCenterList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["WorkCenterDescription"].ToString(),
                        Text = row["WorkCenterDescription"].ToString()
                    });
                }
                model.WorkCenterList = _List;
                _List = new List<TextValue>();

            }
            return model;
        }
        public async Task<JsonResult> GetDataForPendingInProcess(string Flag, string FromDate, string ToDate,string PartCode,string ItemName,string ProdSlipNo,string WorkCenter,string GlobalSearch)
        {
           if(ItemName != null)
            {
                string ItemNameFormatted = ItemName.Split("--->")[0];
                ItemName= ItemNameFormatted;
            }
            var JSON = await _IPendProcessToQC.GetDataForPendingInProcess(Flag, FromDate, ToDate, PartCode, ItemName, ProdSlipNo, WorkCenter, GlobalSearch);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDataforQc(string DisplayPendForQC, string Flag, string FromDate, string ToDate)
        {
            try
            {
                var DisplayGrid = new DataTable();
                List<DisplayPendForQCGrid> Displaydetails = JsonConvert.DeserializeObject<List<DisplayPendForQCGrid>>(DisplayPendForQC);
                DisplayGrid = GetDisplayPendForQC(Displaydetails);
                var JSON = await _IPendProcessToQC.GetDataforQc(DisplayGrid, Flag, FromDate, ToDate);
                string JsonString = JsonConvert.SerializeObject(JSON);
                DataTable dataTable = JSON.Result.Tables[0];
                List<InProcessQcDetail> InProcessQcDetail = new List<InProcessQcDetail>();
                foreach (DataRow row in dataTable.Rows)
                {
                    InProcessQcDetail process = new InProcessQcDetail
                    {
                        ProdSlipNo = row["ProdSlipNo"].ToString(),
                        ProdYearcode = Convert.ToInt32(row["ProdYearcode"]),
                        ProdDate = row["ProdDate"].ToString(),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        ProdPlanSchNo = row["ProdPlanSchNo"].ToString(),
                        ProdPlanSchDate = row["ProdPlanSchDate"].ToString(),
                        ProdPlanNo = row["ProdPlanNo"].ToString(),
                        ProdPlanYearCode = Convert.ToInt32(row["ProdPlanYearCode"]),
                        ProdPlanDate=row["ProdPlanDate"].ToString(),
                        Reqno = row["Reqno"].ToString(),
                        ReqYearCode = Convert.ToInt32(row["ReqThrBOMYearCode"]),
                        ReqDate = row["ReqDate"].ToString(),
                        TotProdQty = Convert.ToDecimal(row["TotProdQty"]),
                        FGRejQty = Convert.ToDecimal(row["ProdRejQty"]),
                        WcId = Convert.ToInt32(row["WcId"]),
                        WorkCenter = row["WorkCenter"].ToString(),
                        ToStoreId = Convert.ToInt32(row["ToStoreId"]),
                        ToStoreName = row["ToStoreName"].ToString(),
                        ToStoreOrWc = row["ToStoreOrWc"].ToString(),
                        ToWorkCenter=row["ToWorkCenter"].ToString(),
                        ToWcId = Convert.ToInt32(row["ToWcId"]),
                        BatchNo=row["BatchNo"].ToString(),
                        UniqueBatchNo=row["UniqueBatchNo"].ToString(),
                        ProdPlanSchYearCode=Convert.ToInt32(row["ProdPlanSchYearCode"]),
                        NewProdRework=row["NewProdRework"].ToString(),
                        ShiftName=row["ShiftName"].ToString(),
                        OperatorName=row["OperatorName"].ToString(),
                        MachineName=row["MachineName"].ToString(),
                        SchQty = Convert.ToDecimal(row["SchQty"]),
                        OKProdQty=Convert.ToDecimal(row["OKProdQty"]),
                        QCOKQty = Convert.ToDecimal(row["QCOKQty"]),
                        QCRejQty=Convert.ToDecimal(row["QCRejQty"]),
                        ItemCode = Convert.ToInt16(row["FGItemCode"]),
                        ProdEntryId = Convert.ToInt32(row["ProdEntryId"]),
                    };

                    InProcessQcDetail.Add(process);
                }
                var dataresult = AddPendingInProcessToQc(InProcessQcDetail);
                return Json(JsonString);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult AddPendingInProcessToQc(List<InProcessQcDetail> ProcessDetail)
        {
            try
            {
                _MemoryCache.Remove("KeyPendingProcessToQC");
                _MemoryCache.TryGetValue("KeyPendingProcessToQC", out IList<InProcessQcDetail> InProcessQcDetail);
                TempData.Clear();

                var MainModel = new InProcessQc();
                var InProcessQcDetails = new List<InProcessQcDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                if (ProcessDetail != null)
                {
                    InProcessQcDetails.AddRange(ProcessDetail);
                }
                MainModel.ItemDetailGrid = InProcessQcDetails;
                _MemoryCache.TryGetValue("KeyPendingProcessToQC", out IList<InProcessQcDetail> grid);
                _MemoryCache.Set("KeyInProcess", MainModel.ItemDetailGrid, cacheEntryOptions);


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetDisplayPendForQC(List<DisplayPendForQCGrid> DisplayPendForQCGrid)
        {
            var DisplayGrid = new DataTable();
            DisplayGrid.Columns.Add("ProdSlipNo", typeof(string));
            DisplayGrid.Columns.Add("ProdEntryId", typeof(int));
            DisplayGrid.Columns.Add("Prodyearcode", typeof(int));
            DisplayGrid.Columns.Add("FGItemCode", typeof(int));

            foreach (var Item in DisplayPendForQCGrid)
            {
                DisplayGrid.Rows.Add(
                    new object[]
                    {
                    Item.ProdSlipNo,
                    Item.ProdEntryId,
                    Item.Prodyearcode,
                    Item.FGItemCode,
                    });
            }
            DisplayGrid.Dispose();
            return DisplayGrid;
        }
    }
}
