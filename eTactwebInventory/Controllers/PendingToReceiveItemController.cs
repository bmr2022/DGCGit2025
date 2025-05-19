using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
//using NuGet.Packaging;
using System.Data;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class PendingToReceiveItemController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingToReceiveItem _IPendingToReceiveItem;
        private readonly ILogger<PendingToReceiveItemController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingToReceiveItemController(ILogger<PendingToReceiveItemController> logger, IDataLogic iDataLogic, IPendingToReceiveItem IPendingToReceiveItem, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendingToReceiveItem = IPendingToReceiveItem;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        public async Task<IActionResult> PendingToReceiveItem()
        {
            ViewData["Title"] = "Pending Receive Item to QC Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyPendingReceiveItem");
            var MainModel = new PendingToReceiveItemModel();
            var model = new PendingToReceiveItemModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel = await BindItem(MainModel);
            MainModel= await BindPartCode(MainModel);
            MainModel= await BindWorkCenter(MainModel);
            MainModel= await BindProdSlipNo(MainModel);
            MainModel= await BindStoreName(MainModel);
            MainModel= await BindProdType(MainModel);

            HttpContext.Session.SetString("KeyPendingReceiveItem", JsonConvert.SerializeObject(model));
            return View(MainModel);
        }
        private async Task<PendingToReceiveItemModel> BindItem(PendingToReceiveItemModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            oDataSet = await _IPendingToReceiveItem.BindItem("PENDINGITEMNAME",model.FromDate,model.ToDate);
            model.ItemNameList = new List<TextValue>();
            model.PartCodeList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["ItemName"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.ItemNameList = _List.Where(x=>x.Value != null).ToList();
                _List = new List<TextValue>();

            }
            return model;
        }
        private async Task<PendingToReceiveItemModel> BindPartCode(PendingToReceiveItemModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            oDataSet = await _IPendingToReceiveItem.BindPartCode("PENDINGPartCode", model.FromDate, model.ToDate);
            model.FromWorkCenterList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["PartCode"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.PartCodeList = _List.Where(x => x.Value != null).ToList();
                _List = new List<TextValue>();
            }
            return model;
        }
        private async Task<PendingToReceiveItemModel> BindWorkCenter(PendingToReceiveItemModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            oDataSet = await _IPendingToReceiveItem.BindItem("FROMWORKCENTER", model.FromDate, model.ToDate);
            model.FromWorkCenterList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[1].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["FromWorkcenter"].ToString(),
                        Text = row["FromWorkcenter"].ToString()
                    });
                }
                model.FromWorkCenterList = _List.Where(x => x.Value != null).ToList();
                _List = new List<TextValue>();
            }
            return model;
        }
        private async Task<PendingToReceiveItemModel> BindProdSlipNo(PendingToReceiveItemModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            oDataSet = await _IPendingToReceiveItem.BindProdSlipNo("PRODSLIPNO", model.FromDate, model.ToDate);
            model.ProdSlipNoList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["prodslipno"].ToString(),
                        Text = row["prodslipno"].ToString()
                    });
                }
                model.ProdSlipNoList = _List.Where(x => x.Value != null).ToList();
                _List = new List<TextValue>();
            }
            return model;
        }
        private async Task<PendingToReceiveItemModel> BindStoreName(PendingToReceiveItemModel model)
        {
            try
            {
                var oDataSet = new DataSet();
                var _List = new List<TextValue>();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                oDataSet = await _IPendingToReceiveItem.BindStoreName("TOSTORE", model.FromDate, model.ToDate);
                model.ToStoreNameList = new List<TextValue>();
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[1].Rows)
                    {
                        _List.Add(new TextValue
                        {
                            Value = row["ToStore"].ToString(),
                            Text = row["ToStore"].ToString()
                        });
                    }
                    model.ToStoreNameList = _List.Where(x=>x.Value != null).ToList();
                    _List = new List<TextValue>();
                }
                return model;
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
        private async Task<PendingToReceiveItemModel> BindProdType(PendingToReceiveItemModel model)
        {
            try
            {
                var oDataSet = new DataSet();
                var _List = new List<TextValue>();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                oDataSet = await _IPendingToReceiveItem.BindProdType("PRODTYPE", model.FromDate, model.ToDate);
                model.ProdTypeList = new List<TextValue>();
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[0].Rows)
                    {
                        _List.Add(new TextValue
                        {
                            Value = row["PRODSTATUSProdUnProdRej"].ToString(),
                            Text = row["PRODSTATUSProdUnProdRej"].ToString()
                        });
                    }
                    model.ProdTypeList = _List
                .DistinctBy(x => x.Value).Where(x=>x.Value != null)// Use DistinctBy to get distinct values directly
                .ToList();
                    _List = new List<TextValue>();
                }
                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<JsonResult> GetDataForPendingReceiveItem(string Flag, string FromDate, string ToDate,string partcode,string itemname)
        {
            var JSON = await _IPendingToReceiveItem.GetDataForPendingReceiveItem(Flag, FromDate, ToDate,partcode,itemname);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDataReceiveItem(string DisplayPendReceiveItem)
        {
            try
            {
                var DisplayGrid = new DataTable();
                List<DisplayPendToReceive> Displaydetails = JsonConvert.DeserializeObject<List<DisplayPendToReceive>>(DisplayPendReceiveItem);
                DisplayGrid = GetDisplayPendForReceiveItem(Displaydetails);
                var JSON = await _IPendingToReceiveItem.GetDataReceiveItem(DisplayGrid);
                string JsonString = JsonConvert.SerializeObject(JSON);
                DataTable dataTable = JSON.Result.Tables[0];
                List<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                foreach (DataRow row in dataTable.Rows)
                {
                    ReceiveItemDetail process = new ReceiveItemDetail
                    {
                        MaterialType = row["MaterialType"].ToString(),
                        FromDepWorkCenter = row["FromDepWorkCenter"].ToString(),
                        DepID = Convert.ToInt32(row["DepID"]),
                        FromWorkcenter = row["FromWorkcenter"].ToString(),
                        WCID = Convert.ToInt32(row["WCID"]),
                        PartCode = row["PartCode"].ToString(),
                        ItemName = row["ItemName"].ToString(),
                        ItemCode=Convert.ToInt32(row["ItemCode"]),
                        Qty = Convert.ToDecimal(row["Qty"]),
                        Unit = row["Unit"].ToString(),
                        AltTransferQty = Convert.ToDecimal(row["AltTransferQty"]),
                        AltUnit = row["AltUnit"].ToString(),
                        Itemremark=row["Itemremark"].ToString(),
                        RecStoreId = Convert.ToInt16(row["RecStoreId"]),
                        RecInStore = row["RecInStore"].ToString(),
                        ProdEntryId = Convert.ToInt32(row["ProdEntryId"]),
                        ProdYearCode = Convert.ToInt32(row["ProdYearCode"]),
                        ProdEntryDate = row["ProdEntryDate"].ToString(),
                        ProdPlanNo=row["ProdPlanNo"].ToString(),
                        ProdPlanYearCode = Convert.ToInt32(row["ProdPlanYearCode"]),
                        ProdSchNo=row["ProdSchNo"].ToString(),
                        ProdSchYearCode = Convert.ToInt32(row["ProdSchYearCode"]),
                        InProcQCSlipNo=row["InProcQCSlipNo"].ToString(),
                        InProcQCYearCode = Convert.ToInt32(row["InProcQCYearCode"]),
                        ProdQty = Convert.ToDecimal(row["ProdQty"]),
                        QCOkQty=Convert.ToDecimal(row["QCOkQty"]),
                        RejQty = Convert.ToDecimal(row["RejQty"]),
                        BatchNo=row["BatchNo"].ToString(),
                        uniquebatchno=row["uniquebatchno"].ToString(),
                        TransferMatEntryId = Convert.ToInt32(row["TransferMatEntryId"]),
                        TransferMatYearCode = Convert.ToInt32(row["TransferMatYearCode"]),
                        TransferMatSlipNo=row["TransferMatSlipNo"].ToString(),
                    };

                    ReceiveItemDetail.Add(process);
                }
                var dataresult = AddPendingInProcessToQc(ReceiveItemDetail);
                return Json(JsonString);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult AddPendingInProcessToQc(List<ReceiveItemDetail> ReceiveItemDetail)
        {
            try
            {
                HttpContext.Session.Remove("KeyReceiveItemGrid");
                string modelJson = HttpContext.Session.GetString("KeyReceiveItemGrid");
                List<ReceiveItemDetail> ReceiveItemDetailGrid = new List<ReceiveItemDetail>();
                if (modelJson != null)
                {
                    ReceiveItemDetailGrid = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }

                TempData.Clear();

                var MainModel = new ReceiveItemModel();
                var ReceiveItemDetails = new List<ReceiveItemDetail>();
               
                var seqNo = 0;
                if (ReceiveItemDetail != null)
                {
                    ReceiveItemDetails.AddRange(ReceiveItemDetail);
                }
                MainModel.ItemDetailGrid = ReceiveItemDetails;
                string ReceiveData = HttpContext.Session.GetString("KeyReceiveItemGrid");
                IList<ReceiveItemDetail> grid = new List<ReceiveItemDetail>();
                if(!string.IsNullOrEmpty(ReceiveData))
                {
                    grid = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(ReceiveData);
                }
                HttpContext.Session.SetString("KeyReceiveItem", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetDisplayPendForReceiveItem(List<DisplayPendToReceive> DisplayPendToReceive)
        {
            var DisplayGrid = new DataTable();
            DisplayGrid.Columns.Add("TransferMatEntryId", typeof(int));
            DisplayGrid.Columns.Add("TransferMatYearCode", typeof(int));
            DisplayGrid.Columns.Add("ItemCode", typeof(int));
            DisplayGrid.Columns.Add("IssueToStoreWC", typeof(string));
            DisplayGrid.Columns.Add("BatchNo", typeof(string));
            DisplayGrid.Columns.Add("Uniquebatchno", typeof(string));

            foreach (var Item in DisplayPendToReceive)
            {
                DisplayGrid.Rows.Add(
                    new object[]
                    {
                    Item.TransferMatEntryId,
                    Item.TransferMatYearCode,
                    Item.ItemCode,
                    Item.IssueToStoreWC,

                    Item.BatchNo,   
                    Item.Uniquebatchno
                    });
            }
            DisplayGrid.Dispose();
            return DisplayGrid;
        }
    }
}
