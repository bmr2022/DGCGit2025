using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Convert;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkIssueModel;

namespace eTactWeb.Controllers;

[Authorize]
public class TaxController : Controller
{
    public TaxController(IMemoryCache imemoryCache, ILogger<TaxController> logger, IDataLogic iDataLogic, ITaxModule iTaxModule)
    {
        _MemoryCache = imemoryCache;
        Logger = logger;
        IDataLogic = iDataLogic;
        ITaxModule = iTaxModule;
    }

    public IDataLogic IDataLogic { get; }
    public ITaxModule ITaxModule { get; }
    public ILogger<TaxController> Logger { get; }
    private IMemoryCache _MemoryCache { get; }

    public IList<TaxModel> Add2List(TaxModel model, IList<TaxModel> TaxGrid, DataTable CgstSgst)
    {
        var _List = new List<TaxModel>();
        int accountCodeCallCount = 0;
        int rowIndex = 0;
        if (CgstSgst != null && CgstSgst.Rows != null && CgstSgst.Rows.Count > 0)
        {
            rowIndex = accountCodeCallCount % CgstSgst.Rows.Count;
        }
        else
        {
            rowIndex = 0;
        }
        accountCodeCallCount++;
        _List.Add(new TaxModel
        {
            TxSeqNo = TaxGrid == null ? 1 : TaxGrid.Count + 1,
            TxType = model.TxType,
            TxPartCode = model.TxPartCode,
            TxPartName = model.TxPartName,
            TxItemCode = model.TxItemCode,
            TxItemName = model.TxItemName,
            TxTaxType = model.TxTaxType,
            TxTaxTypeName = model.TxTaxTypeName,
            //TxAccountCode = ToInt32(CgstSgst.Rows[rowIndex]["Account_Code"]),
            //TxAccountName = CgstSgst.Rows[rowIndex]["Tax_Name"].ToString(),
            TxAccountCode = CgstSgst != null && CgstSgst.Rows.Count > 0 ? ToInt32(CgstSgst.Rows[0]["Account_Code"]) : model.TxAccountCode,
            TxAccountName = CgstSgst != null && CgstSgst.Rows.Count > 0 ? CgstSgst.Rows[0]["Tax_Name"].ToString() : model.TxAccountName,
            TxPercentg = model.TxPercentg,
            TxAdInTxable = model.TxAdInTxable,
            TxRoundOff = model.TxRoundOff,
            TxAmount = model.TxAmount,
            TxRefundable = model.TxRefundable,
            TxOnExp = model.TxOnExp,
            TxRemark = model.TxRemark,
        });

        return _List;
    }
    public IActionResult ClearAdjGrid()
    {
        _MemoryCache.Remove("KeyAdjGrid");
        HttpContext.Session.Remove("KeyAdjGrid");
        return Json("Ok");
    }
    
    public IList<IssueNRGPTaxDetail> Add2IssueList(IssueNRGPTaxDetail model, IList<IssueNRGPTaxDetail> TaxGrid, DataTable CgstSgst)
    {
        var _List = new List<IssueNRGPTaxDetail>();
        //int accountCodeCallCount = 0;
        //var rowIndex = accountCodeCallCount % CgstSgst.Rows.Count; // 0 first time, 1 second time
        //accountCodeCallCount++;

        _List.Add(new IssueNRGPTaxDetail
        {
            SeqNo = TaxGrid == null ? 1 : TaxGrid.Count + 1,
            TxType = model.TxType,
            TxPartCode = model.TxPartCode,
            TxItemCode = model.TxItemCode,
            TxItemName = model.TxItemName,
            TxTaxType = model.TxTaxType,
            TxTaxTypeName = model.TxTaxTypeName,
            //TxAccountCode = ToInt32(CgstSgst.Rows[rowIndex]["Account_Code"]),
            //TxAccountName = CgstSgst.Rows[rowIndex]["Tax_Name"].ToString(),
            TxAccountCode = CgstSgst != null && CgstSgst.Rows.Count > 0 ? ToInt32(CgstSgst.Rows[0]["Account_Code"]) : model.TxAccountCode,
            TxAccountName = CgstSgst != null && CgstSgst.Rows.Count > 0 ? CgstSgst.Rows[0]["Tax_Name"].ToString() : model.TxAccountName,
            TxPercentg = model.TxPercentg,
            TxAdInTxable = model.TxAdInTxable,
            TxRoundOff = model.TxRoundOff,
            TxAmount = model.TxAmount,
            TxRefundable = model.TxRefundable,
            TxOnExp = model.TxOnExp,
            TxRemark = model.TxRemark,
        });

        return _List;
    }

    public IActionResult AddTaxDetail(TaxModel model)
    {
        var isDuplicate = false;
        var isExpAdded = isDuplicate;
        var taxFound = isExpAdded;

        var _List = new List<TaxModel>();

        dynamic MainModel = new SaleOrderModel();

        if (model.TxPageName == "ItemList")
        {
            MainModel = new SaleOrderModel();
        }
        else if (model.TxPageName == "PurchaseOrder")
        {
            MainModel = new PurchaseOrderModel();
        }
        else if (model.TxPageName == "DirectPurchaseBill")
        {
            MainModel = new DirectPurchaseBillModel();
        }
        else if (model.TxPageName == "PurchaseBill")
        {
            MainModel = new PurchaseBillModel();
        }
        else if (model.TxPageName == "SaleInvoice")
        {
            MainModel = new SaleBillModel();
        }
        else if (model.TxPageName == "SaleRejection")
        {
            MainModel = new SaleRejectionModel();
        }
        else if (model.TxPageName == "CreditNote")
        {
            MainModel = new AccCreditNoteModel();
        }
        else if (model.TxPageName == "PurchaseRejection")
        {
            MainModel = new AccPurchaseRejectionModel();
        }
        else if (model.TxPageName == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }

        var CgstSgst = ITaxModule.SgstCgst(model.TxAccountCode);
        IList<TaxModel> TaxGrid = new List<TaxModel>();
        //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid);
        string modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
        if (!string.IsNullOrEmpty(modelTxGridJson))
        {
            TaxGrid = JsonConvert.DeserializeObject<IList<TaxModel>>(modelTxGridJson);
        }

        if (HttpContext.Session.GetString(model.TxPageName) != null)
        {
            if (model.TxPageName == "ItemList")
            {
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(model.TxPageName));
            }
            else if (model.TxPageName == "PurchaseOrder")
            {
                _MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
                //MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(model.TxPageName));
            }
            else if (model.TxPageName == "DirectPurchaseBill")
            {
                _MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
                string modelJson = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJson);
                }
            }
            else if (model.TxPageName == "PurchaseBill")
            {
                //_MemoryCache.TryGetValue("PurchaseBill", out MainModel);
                string modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }
            }
            else if (model.TxPageName == "SaleInvoice")
            {
                //_MemoryCache.TryGetValue("KeySaleBillGrid", out IList<SaleBillDetail> saleBillDetail);
                string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                List<SaleBillDetail> saleBillDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    saleBillDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }

                //var saleBillModel = new SaleBillModel();
                //saleBillModel.saleBillDetails = saleBillDetail.ToList();
                MainModel.saleBillDetails = saleBillDetail;
            }
            else if (model.TxPageName == "CreditNote")
            {
                //_MemoryCache.TryGetValue("KeyCreditNoteGrid", out IList<AccCreditNoteDetail> creditNoteDetail);
                string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }

                MainModel.AccCreditNoteDetails = creditNoteDetail;
                MainModel.ItemDetailGrid = creditNoteDetail;
            }
            else if (model.TxPageName == "PurchaseRejection")
            {
                _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out IList<AccPurchaseRejectionDetail> purchaseRejectionDetail);
                string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
                if (!string.IsNullOrEmpty(modelPRGridJson))
                {
                    purchaseRejectionDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
                }
                var purchaseRejectionModel = new AccPurchaseRejectionModel();
                purchaseRejectionModel.AccPurchaseRejectionDetails = purchaseRejectionDetail.ToList();
                MainModel = purchaseRejectionModel;
            }
            else if (model.TxPageName == "SaleRejection")
            {
                string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                List<SaleRejectionDetail> SaleRejectionDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    SaleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
                }

                //var saleBillModel = new SaleBillModel();
                //saleBillModel.saleBillDetails = saleBillDetail.ToList();
                MainModel.SaleRejectionDetails = SaleRejectionDetail;

                //_MemoryCache.TryGetValue("KeySaleRejectionGrid", out IList<SaleRejectionDetail> SaleRejectionDetail);
                //string modelPRGridJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                //if (!string.IsNullOrEmpty(modelPRGridJson))
                //{
                //    SaleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelPRGridJson);
                //}
                //var SaleRejectionDetailModel = new SaleRejectionModel();
                //SaleRejectionDetailModel.SaleRejectionDetails = SaleRejectionDetail.ToList();
                //MainModel = SaleRejectionDetailModel;

                //_MemoryCache.TryGetValue("KeySaleRejectionGrid", out IList<SaleRejectionDetail> saleRejectionDetail);
                //var saleRejectionModel = new SaleRejectionModel();
                //saleRejectionModel.SaleRejectionDetails = saleRejectionDetail.ToList();
                //MainModel = saleRejectionModel;
            }
            else if (model.TxPageName == "JobWorkIssue")
            {
                //_MemoryCache.TryGetValue("JobWorkIssue", out MainModel);

                string modelTaxJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                MainModel = new JobWorkIssueModel();
                if (!string.IsNullOrEmpty(modelTaxJson))
                {
                    MainModel.JobDetailGrid = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelTaxJson);
                }

                string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                HttpContext.Session.SetString("JobWorkIssue", serializedGrid);
                //MainModel = jobmodel;

                //MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(model.TxPageName));
            }
            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                MainModel.TaxDetailGridd = TaxGrid;

                isExpAdded = TaxGrid.Any(a => a.TxType == "EXPENSES");
                taxFound = TaxGrid.Any(m => m.TxType == "TAX");

                if (model.TxType != null)
                {
                    if (model.TxType == "EXPENSES")
                    {
                        isDuplicate = TaxGrid.Any(a => a.TxType.Equals("EXPENSES") && a.TxAccountName.Equals(model.TxAccountName));

                        if (taxFound && model.TxAdInTxable == "Y")
                        {
                            return StatusCode(400);
                        }
                    }
                    if (model.TxType == "TAX")
                    {
                        isDuplicate = TaxGrid.Any(a => a.TxType.Equals("TAX") && a.TxPartCode.Equals(model.TxPartCode));
                    }
                }
            }

            if (!isDuplicate)
            {
                if (TaxGrid != null && TaxGrid.Count > 0)
                {
                    _List.AddRange(MainModel.TaxDetailGridd);
                    _List.AddRange(Add2List(model, _List, null));
                }
                else
                {
                    var TaxModelList = Add2List(model, _List, null);
                    _List.AddRange(TaxModelList);
                }
                if (CgstSgst.Rows.Count > 0 && model.TxAccountName.ToUpper().Contains("CGST"))
                {
                    var pp = Add2List(model, _List, CgstSgst);
                    _List.AddRange(pp);
                }
            }
            else
            {
                return StatusCode(200);
            }

            MainModel.TaxDetailGridd = _List;

            //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            //    SlidingExpiration = TimeSpan.FromMinutes(55),
            //    Size = 1024,
            //};

            //CacheExtensions.Set(_MemoryCache, "KeyTaxGrid", MainModel.TaxDetailGridd, cacheEntryOptions);

            StoreInCache("KeyTaxGrid", MainModel.TaxDetailGridd);

            string serializedData = JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
            HttpContext.Session.SetString("KeyTaxGrid", serializedData);
        }
        else
        {
            return StatusCode(501, "Please Add Data In Item Detail Grid");
        }



        return PartialView("_TaxGrid", MainModel);
    }
    public IActionResult AddNRGPTaxDetail(IssueNRGPTaxDetail model)
    {
        var isDuplicate = false;
        var isExpAdded = isDuplicate;
        var taxFound = isExpAdded;

        var _List = new List<IssueNRGPTaxDetail>();

        dynamic MainModel = new IssueNRGPModel();

        if (model.TxPageName == "IssueNRGP")
        {
            MainModel = new IssueNRGPModel();
        }

        var CgstSgst = ITaxModule.SgstCgst(model.TxAccountCode);

        //_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out IList<IssueNRGPTaxDetail> TaxGrid);
        var jsondata = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
        List<IssueNRGPTaxDetail> TaxGrid = new List<IssueNRGPTaxDetail>();
        if (jsondata != null)
        {
            TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(jsondata);
        }

        if (HttpContext.Session.GetString(model.TxPageName) != null)
        {
            if (model.TxPageName == "IssueNRGP")
            {
                var jsonMainModeldata = HttpContext.Session.GetString("IssueNRGP");
                if (jsonMainModeldata != null)
                {
                    MainModel = JsonConvert.DeserializeObject<IssueNRGPModel>(jsonMainModeldata);
                }

                //_MemoryCache.TryGetValue("IssueNRGP", out MainModel);

                //var jsondata = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                //if (jsondata != null)
                //{
                //    MainModel = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(jsondata);
                //}
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(model.TxPageName));
            }

            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                MainModel.IssueNRGPTaxGrid = TaxGrid;

                isExpAdded = TaxGrid.Any(a => a.TxType == "EXPENSES");
                taxFound = TaxGrid.Any(m => m.TxType == "TAX");

                if (model.TxType != null)
                {
                    if (model.TxType == "EXPENSES")
                    {
                        isDuplicate = TaxGrid.Any(a => a.TxType.Equals("EXPENSES") && a.TxAccountName.Equals(model.TxAccountName));

                        if (taxFound && model.TxAdInTxable == "Y")
                        {
                            return StatusCode(400);
                        }
                    }
                    if (model.TxType == "TAX")
                    {
                        isDuplicate = TaxGrid.Any(a => a.TxType.Equals("TAX") && a.TxPartCode.Equals(model.TxPartCode));
                    }
                }
            }

            if (!isDuplicate)
            {
                if (TaxGrid != null && TaxGrid.Count > 0)
                {
                    _List.AddRange(MainModel.IssueNRGPTaxGrid);
                    _List.AddRange(Add2IssueList(model, _List, null));

                }
                else
                {
                    var TaxModelList = Add2IssueList(model, _List, null);
                    _List.AddRange(TaxModelList);
                }
                if (CgstSgst.Rows.Count > 0 && model.TxAccountName.ToUpper().Contains("CGST"))
                {
                    var pp = Add2IssueList(model, _List, CgstSgst);
                    _List.AddRange(pp);
                }
            }
            else
            {
                return StatusCode(200);
            }

            MainModel.IssueNRGPTaxGrid = _List;

            string serializedData = JsonConvert.SerializeObject(MainModel.IssueNRGPTaxGrid);
            HttpContext.Session.SetString("KeyIssueNRGPTaxGrid", serializedData);

            StoreInCache("KeyIssueNRGPTaxGrid", MainModel.IssueNRGPTaxGrid);
        }
        else
        {
            return StatusCode(501, "Please Add Data In Item Detail Grid");
        }

        return PartialView("_TaxNRGPGrid", MainModel);
    }

    public IActionResult ApplyTax2All(TaxModel TxModel)
    {
        int ItemCnt = 0;
        decimal BasicTotal = 0;
        decimal ItemAmount = 0;
        decimal TaxAmount = 0;
        decimal ExpTaxAmt = 0;
        decimal TaxOnExp = 0;
        bool isExp = false;
        bool exists = false;
        decimal TotalTaxAmt = 0;

        dynamic MainModel = null;
        dynamic ItemDetailGrid = null;

        var PartCode = 0;
        var PartName = "";
        var ItemCode = 0;
        var ItemText = "";

        var _List = new List<TaxModel>();

        var CgstSgst = ITaxModule.SgstCgst(TxModel.TxAccountCode);

        CultureInfo CI = new CultureInfo("en-IN");
        CI.NumberFormat.NumberDecimalDigits = 4;

        if (TxModel.TxPageName == "ItemList")
        {
            MainModel = new SaleOrderModel();
        }
        else if (TxModel.TxPageName == "PurchaseOrder")
        {
            MainModel = new PurchaseOrderModel();
        }
        else if (TxModel.TxPageName == "DirectPurchaseBill")
        {
            MainModel = new DirectPurchaseBillModel();
        }
        else if (TxModel.TxPageName == "PurchaseBill")
        {
            MainModel = new PurchaseBillModel();
        }
        else if (TxModel.TxPageName == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }
        else if (TxModel.TxPageName == "SaleInvoice")
        {
            MainModel = new SaleBillModel();
        }
        else if (TxModel.TxPageName == "CreditNote")
        {
            MainModel = new AccCreditNoteModel();
        }
        else if (TxModel.TxPageName == "PurchaseRejection")
        {
            MainModel = new AccPurchaseRejectionModel();
        }
        else if (TxModel.TxPageName == "SaleRejection")
        {
            MainModel = new SaleRejectionModel();
        }
        if (HttpContext.Session.GetString(TxModel.TxPageName) != null)
        {
            if (TxModel.TxPageName == "ItemList")
            {
                ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TxModel.TxPageName) ?? string.Empty);




               
                var _ItemGrid = new List<ItemDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new ItemDetail
                 {
                     Amount = group.Sum(item => item.Amount),
                     ItemCode = group.Key,
                     PartText = group.Select(x => x.PartText).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                     ItemText = group.Select(x => x.ItemText).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;



            }
            else if (TxModel.TxPageName == "PurchaseOrder")
            {
                //_MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
                ItemDetailGrid = MainModel.ItemDetailGrid;
            }
            else if (TxModel.TxPageName == "SaleInvoice")
            {
                //_MemoryCache.TryGetValue("KeySaleBillGrid", out IList<SaleBillDetail> saleBillDetail);
                string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                List<SaleBillDetail> saleBillDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    saleBillDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }

                ItemDetailGrid = saleBillDetail;
            }
            else if (TxModel.TxPageName == "CreditNote")
            {
                //_MemoryCache.TryGetValue("KeyCreditNoteGrid", out IList<AccCreditNoteDetail> creditNoteDetail);
                string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }

                ItemDetailGrid = creditNoteDetail;
            }
            else if (TxModel.TxPageName == "PurchaseRejection")
            {
                _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionDetail);
                string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
                List<AccPurchaseRejectionDetail> PurchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
                if (!string.IsNullOrEmpty(modelPRGridJson) && purchaseRejectionDetail == null)
                {
                    purchaseRejectionDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
                }
                ItemDetailGrid = purchaseRejectionDetail;
                var _ItemGrid = new List<AccPurchaseRejectionDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new AccPurchaseRejectionDetail
                 {
                     Amount = group.Sum(item => item.Amount),
                     ItemCode = group.Key,
                     PartCode = group.Select(x => x.PartCode).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                     ItemName = group.Select(x => x.ItemName).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }
            else if (TxModel.TxPageName == "SaleRejection")
            {
                string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                List<SaleRejectionDetail> SaleRejectionDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    SaleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
                }
                ItemDetailGrid = SaleRejectionDetail;

            }
            else if (TxModel.TxPageName == "DirectPurchaseBill")
            {
                //_MemoryCache.TryGetValue("KeyDirectPurchaseBill", out List<DPBItemDetail> DirectPurchaseBill);
                //string modelPRGridJson = HttpContext.Session.GetString("KeyDirectPurchaseBill");
                //List<DPBItemDetail> DirectPurchaseBill1 = new List<DPBItemDetail>();
                //if (!string.IsNullOrEmpty(modelPRGridJson) && DirectPurchaseBill == null)
                //{
                //    DirectPurchaseBill1 = JsonConvert.DeserializeObject<List<DPBItemDetail>>(modelPRGridJson);
                //}
                string modelJson = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJson);
                }

                //ItemDetailGrid = MainModel;
                ItemDetailGrid = MainModel.ItemDetailGrid;

                //_MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
                //ItemDetailGrid = MainModel.ItemDetailGrid;
                var _ItemGrid = new List<DPBItemDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new DPBItemDetail
                 {
                     Amount = group.Sum(item => item.Amount),
                     ItemCode = group.Key,
                     PartCode = group.First().PartCode,
                     ItemText = group.First().ItemText,
                     PartText = group.First().PartText,
                     
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }
            else if (TxModel.TxPageName == "PurchaseBill")
            {
                //_MemoryCache.TryGetValue("PurchaseBill", out MainModel);

                string modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }

                ItemDetailGrid =  MainModel.ItemDetailGridd;
                var _ItemGrid = new List<PBItemDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new PBItemDetail
                 {
                     //MRP = group.Sum(item => item.MRP),
                     Amount = Convert.ToDecimal(group.Sum(item => item.Amount)),
                     ItemCode = group.Key,
                     PartCode = group.Select(x => x.PartCode).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                     ItemText = group.Select(x => x.ItemText).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                     PartText = group.Select(x => x.PartText).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                     Item_Name = group.Select(x => x.Item_Name).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }
            else if (TxModel.TxPageName == "JobWorkIssue")
            {
                //MainModel= HttpContext.Session.GetString("KeyJobWorkIssue");
                //if (MainModel == null)
                //{
                //    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out MainModel);
                //    MainModel = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
                //}

                string? sessionData = HttpContext.Session.GetString("KeyJobWorkIssue");

                if (string.IsNullOrEmpty(sessionData))
                {
                    sessionData = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
                }

                // Deserialize JSON into list of JobWorkGridDetail
                List<JobWorkGridDetail> ItemDetailGrid1 = !string.IsNullOrEmpty(sessionData)
                    ? JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(sessionData)
                    : new List<JobWorkGridDetail>();





                ItemDetailGrid = ItemDetailGrid1;
                var _ItemGrid = new List<JobWorkGridDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new JobWorkGridDetail
                 {
                     Amount = group.Sum(item => item.PurchasePrice * item.IssQty),
                     ItemCode = group.Key,
                     PartCode = group.First().PartCode,
                     ItemName = group.First().ItemName
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }

            if (ItemDetailGrid != null && TxModel.TxType != "EXPENSES")
            {
                IList<TaxModel> TaxGrid = new List<TaxModel>();
                //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid);
                string modeltxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
                List<TaxModel> txgrid = new List<TaxModel>();
                if (!string.IsNullOrEmpty(modeltxGridJson))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modeltxGridJson);
                }
                 isExp = TaxGrid.Any(m => m.TxType == "EXPENSES");
                foreach (var item in ItemDetailGrid)
                {
                    //Basic Total Amount
                    BasicTotal = item.Amount == null ? 0 : (BasicTotal + item.Amount);
                }

                List<string> partCodeArray = new List<string>();
                var taxGrid22 = new List<int>();

                foreach (var item in ItemDetailGrid)
                {
                    List<JobWorkGridDetail> taxGrid1 = new List<JobWorkGridDetail>();
                    _MemoryCache.TryGetValue("KeyJobWorkIssue", out taxGrid1);
                    if (taxGrid1 == null)
                        _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out taxGrid1);

                    if (taxGrid1 == null)
                    {
                        taxGrid1 = new List<JobWorkGridDetail>();
                    }

                    var checkContains = false;
                    if (TxModel.TxPageName == "JobWorkIssue" || TxModel.TxPageName == "SaleInvoice" || TxModel.TxPageName == "PurchaseBill" || TxModel.TxPageName == "SaleRejection" || TxModel.TxPageName == "CreditNote" || TxModel.TxPageName == "PurchaseRejection")
                        checkContains = partCodeArray.Contains(item.PartCode);
                    else
                        checkContains = partCodeArray.Contains(item.PartText);

                    if (taxGrid22.Contains(item.ItemCode))
                    {
                        ItemAmount += item.Amount;
                    }
                    else
                    {
                        ItemAmount = item.Amount == null ? 0 : item.Amount;
                    }
                    taxGrid22.Add(item.ItemCode);

                    //if (checkContains)
                    //{
                    //    //do nothing
                    //}
                    //else
                    //{
                    decimal ItemAmount1 = 0;
                    ItemCnt = ItemCnt + 1;
                    ItemAmount = item.Amount == null ? 0 : item.Amount;

                    if (isExp)
                    {
                        ExpTaxAmt = TaxGrid.Where(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y").Sum(x => x.TxAmount);
                        //TaxOnExp = ((ItemAmount / BasicTotal) * ExpTaxAmt);
                        TaxOnExp = ItemAmount == 0 && BasicTotal == 0 ? 0 : Math.Round((ExpTaxAmt / BasicTotal) * ItemAmount, 6, MidpointRounding.AwayFromZero);

                        ItemAmount = Math.Round(ItemAmount + TaxOnExp, 6, MidpointRounding.AwayFromZero);
                    }

                    TaxAmount = ItemAmount * TxModel.TxPercentg / 100;

                    if (TxModel.TxPageName == "JobWorkIssue" || TxModel.TxPageName == "SaleInvoice" || TxModel.TxPageName == "SaleRejection" || TxModel.TxPageName == "CreditNote" || TxModel.TxPageName == "PurchaseRejection")
                    {
                        PartCode = item.ItemCode;
                        PartName = item.PartCode;
                        ItemCode = item.ItemCode;
                        ItemText = item.ItemName;
                    }
                    else if (TxModel.TxPageName == "PurchaseBill")
                    {
                        PartCode = item.ItemCode;
                        PartName = item.PartCode;
                        ItemCode = item.ItemCode;
                        ItemText = item.Item_Name;
                    }
                    else
                    {
                        PartCode = item.PartCode;
                        PartName = item.PartText;
                        ItemCode = item.ItemCode;
                        ItemText = item.ItemText;
                    }
                    var intialCount = TaxGrid == null ? 0 : TaxGrid!.Count;
                    var existsTaxType = TaxGrid != null ? TaxGrid!.Any(x => x.TxType == "EXPENSES") : false;
                    _List.Add(new TaxModel
                    {
                       // TxSeqNo = _List.Count + 1,
                        TxSeqNo = !existsTaxType ? (_List.Count + 1) : (intialCount + (_List.Count + 1)),
                        TxType = TxModel.TxType,
                        TxPartCode = PartCode,
                        TxPartName = PartName,
                        TxItemCode = ItemCode,
                        TxItemName = ItemText,
                        TxTaxType = TxModel.TxTaxType,
                        TxTaxTypeName = TxModel.TxTaxTypeName,
                        TxAccountCode = TxModel.TxAccountCode,
                        TxAccountName = TxModel.TxAccountName,
                        TxPercentg = TxModel.TxPercentg,
                        TxAdInTxable = TxModel.TxAdInTxable,
                        TxRoundOff = TxModel.TxRoundOff,
                        TxAmount = TxModel.TxRoundOff == "Y"
    ? Math.Round(TaxAmount, MidpointRounding.AwayFromZero)
    : Math.Round(TaxAmount, 6),
                        TxRefundable = TxModel.TxRefundable,
                        TxOnExp = TaxOnExp,
                        TxRemark = TxModel.TxRemark,
                    });

                    if (CgstSgst.Rows.Count > 0 && TxModel.TxAccountName.ToUpper().Contains("CGST"))
                    {
                        int accountCodeIndex = 0;
                        var rowIndex = accountCodeIndex % CgstSgst.Rows.Count; // 0 first time, 1 second time
                        accountCodeIndex++;

                        _List.Add(new TaxModel
                        {

                            //TxSeqNo = _List.Count + 1,
                            TxSeqNo = !existsTaxType ? (_List.Count + 1) : (intialCount + (_List.Count + 1)),
                            TxType = TxModel.TxType,
                            TxPartCode = PartCode,
                            TxPartName = PartName,
                            TxItemCode = ItemCode,
                            TxItemName = ItemText,
                            TxTaxType = TxModel.TxTaxType,
                            TxTaxTypeName = TxModel.TxTaxTypeName,
                            TxAccountCode = ToInt32(CgstSgst.Rows[rowIndex]["Account_Code"], CI),
                            TxAccountName = CgstSgst.Rows[rowIndex]["Tax_Name"].ToString() ,
                            TxPercentg = TxModel.TxPercentg,
                            TxAdInTxable = TxModel.TxAdInTxable,
                            TxRoundOff = TxModel.TxRoundOff,
                            TxAmount = TxModel.TxRoundOff == "Y" ? Math.Round(TaxAmount) : Math.Round(TaxAmount, 6),
                            TxRefundable = TxModel.TxRefundable,
                            TxOnExp = TaxOnExp,
                            TxRemark = TxModel.TxRemark,
                        });
                    }
                    if (TxModel.TxPageName == "JobWorkIssue" || TxModel.TxPageName == "SaleInvoice" || TxModel.TxPageName == "PurchaseBill" || TxModel.TxPageName == "SaleRejection" || TxModel.TxPageName == "CreditNote" || TxModel.TxPageName == "PurchaseRejection")
                        partCodeArray.Add(item.PartCode);
                    else
                        partCodeArray.Add(item.PartText);

                }
                if (TxModel.TxPageName == "JobWorkIssue")
                {
                    MainModel = new JobWorkIssueModel();
                }
                var expenseEntries = TaxGrid.Where(m => m.TxType == "EXPENSES").ToList();

                var existingExpenseKeys = new HashSet<string>(_List
    .Where(t => t.TxType == "EXPENSES")
    .Select(t => $"{t.TxAccountCode}_{t.TxPartCode}_{t.TxAmount}"));

                // Only keep those EXPENSES that are not already in the new list
                var filteredExpenses = expenseEntries
                    .Where(e => !existingExpenseKeys.Contains($"{e.TxAccountCode}_{e.TxPartCode}_{e.TxAmount}"))
                    .ToList();

                // Merge unique EXPENSES + new taxes
                _List = filteredExpenses.Concat(_List).ToList();
               // _List = expenseEntries.Concat(_List).ToList();

                MainModel!.TaxDetailGridd = _List;
                TaxGrid = MainModel.TaxDetailGridd;

                if (HttpContext.Session.GetString(TxModel.TxPageName) != null)
                {
                    if (TxModel.TxPageName == "ItemList")
                    {
                        MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TxModel.TxPageName) ?? string.Empty);
                    }
                    if (TxModel.TxPageName == "PurchaseOrder")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "DirectPurchaseBill")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "PurchaseBill")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "JobWorkIssue")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "SaleInvoice")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "CreditNote")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "PurchaseRejection")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                    if (TxModel.TxPageName == "SaleRejection")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                }

                if (TaxGrid != null)
                {
                    TotalTaxAmt = TaxGrid.Sum(x => x.TxAmount);
                }
                TotalTaxAmt = TotalTaxAmt + (MainModel.ItemNetAmount == null ? 0 : MainModel.ItemNetAmount);
                MainModel.TotalTaxAmt = TotalTaxAmt;

                var pagename = TxModel.TxPageName;
                ClearTax(pagename);
                StoreInCache("KeyTaxGrid", TaxGrid);
                string serializedData = JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
                HttpContext.Session.SetString("KeyTaxGrid", serializedData);
            }
        }
        return PartialView("_TaxGrid", MainModel);


        /*
        int ItemCnt = 0;
        decimal BasicTotal, ItemAmount, TaxAmount, ExpTaxAmt, TaxOnExp = 0M;
        bool isExp = false;
        bool exists = false;
        var MainModel = new SaleOrderModel();
        var _List = new List<TaxModel>();
        var CgstSgst = ITaxModule.SgstCgst(TxModel.TxAccountCode);

        CultureInfo CI = new CultureInfo("en-IN");
        //CI.NumberFormat.CurrencySymbol = "Rs.";
        CI.NumberFormat.NumberDecimalDigits = 2;

        if (HttpContext.Session.GetString("ItemList") != null)
        {
            var ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);

            if (ItemDetailGrid != null && TxModel.TxType != "EXPENSES")
            {
                _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

                if (TaxGrid != null)
                {
                    isExp = TaxGrid.Any(m => m.TxType == "EXPENSES");
                    _List = TaxGrid;
                    exists = TaxGrid.Select(s1 => s1.TxPartCode).ToList().Intersect(ItemDetailGrid.Select(s2 => s2.PartCode).ToList()).Any();
                }

                if (exists)
                {
                    return BadRequest();
                }
                else
                {
                    foreach (var item in ItemDetailGrid)
                    {
                        ItemCnt = ItemCnt + 1;
                        BasicTotal = ItemDetailGrid.Sum(m => m.Amount);
                        ItemAmount = item.Amount;

                        if (isExp)
                        {
                            ExpTaxAmt = TaxGrid.Where(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y").Sum(x => x.TxAmount);
                            TaxOnExp = ((ItemAmount / BasicTotal) * ExpTaxAmt);
                            ItemAmount = TaxOnExp + ItemAmount;
                        }

                        TaxAmount = ItemAmount * TxModel.TxPercentg / 100;

                        _List.Add(new TaxModel
                        {
                            TxSeqNo = _List.Count + 1,//ItemCnt,
                            TxType = TxModel.TxType,
                            TxPartCode = item.PartCode,
                            TxPartName = item.PartText,
                            TxItemCode = item.ItemCode,
                            TxItemName = item.ItemText,
                            TxTaxType = TxModel.TxTaxType,
                            TxTaxTypeName = TxModel.TxTaxTypeName,
                            TxAccountCode = TxModel.TxAccountCode,
                            TxAccountName = TxModel.TxAccountName,
                            TxPercentg = TxModel.TxPercentg,
                            TxAdInTxable = TxModel.TxAdInTxable,
                            TxRoundOff = TxModel.TxRoundOff,
                            TxAmount = TxModel.TxRoundOff == "Y" ? Math.Floor(TaxAmount) : Math.Round(TaxAmount, 2),
                            TxRefundable = TxModel.TxRefundable,
                            TxOnExp = TaxOnExp,//(float)TaxOnExp,
                            TxRemark = TxModel.TxRemark,
                        });

                        if (CgstSgst.Rows.Count > 0)
                        {
                            _List.Add(new TaxModel
                            {
                                TxSeqNo = _List.Count + 1,
                                TxType = TxModel.TxType,
                                TxPartCode = item.PartCode,
                                TxPartName = item.PartText,
                                TxItemCode = item.ItemCode,
                                TxItemName = item.ItemText,
                                TxTaxType = TxModel.TxTaxType,
                                TxTaxTypeName = TxModel.TxTaxTypeName,
                                TxAccountCode = ToInt32(CgstSgst.Rows[0]["Account_Code"], CI),
                                TxAccountName = CgstSgst.Rows[0]["Tax_Name"].ToString(),
                                TxPercentg = TxModel.TxPercentg,
                                TxAdInTxable = TxModel.TxAdInTxable,
                                TxRoundOff = TxModel.TxRoundOff,
                                TxAmount = TxModel.TxRoundOff == "Y" ? Math.Floor(TaxAmount) : Math.Round(TaxAmount, 2),
                                TxRefundable = TxModel.TxRefundable,
                                TxOnExp = TaxOnExp,
                                TxRemark = TxModel.TxRemark,
                            });
                        }
                    }

                    MainModel.TaxDetailGridd = _List;
                    if (HttpContext.Session.GetString("ItemList") != null)
                    {
                        MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);
                    }
                    MainModel.TotalTaxAmt = MainModel.TaxDetailGridd.Sum(x => x.TxAmount + MainModel.ItemNetAmount);

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                        SlidingExpiration = TimeSpan.FromMinutes(60),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyTaxGrid", MainModel.TaxDetailGridd, cacheEntryOptions);
                }
            }
        }
        return PartialView("_TaxGrid", MainModel);
        */
    }


    public IActionResult ApplyIssueNRGPTax2All(IssueNRGPTaxDetail TxModel)
    {
        int ItemCnt = 0;
        decimal BasicTotal = 0;
        decimal ItemAmount = 0;
        decimal TaxAmount = 0;
        decimal ExpTaxAmt = 0;
        decimal TaxOnExp = 0;
        bool isExp = false;
        bool exists = false;
        decimal TotalTaxAmt = 0;

        dynamic MainModel = null;
        dynamic ItemDetailGrid = null;

        var PartCode = 0;
        var PartName = "";
        var ItemCode = 0;
        var ItemText = "";

        var _List = new List<IssueNRGPTaxDetail>();

        var CgstSgst = ITaxModule.SgstCgst(TxModel.TxAccountCode);

        CultureInfo CI = new CultureInfo("en-IN");
        CI.NumberFormat.NumberDecimalDigits = 4;

        if (TxModel.TxPageName == "IssueNRGP")
        {
            MainModel = new IssueNRGPModel();
        }

        if (HttpContext.Session.GetString(TxModel.TxPageName) != null)
        {
            if (TxModel.TxPageName == "IssueNRGP")
            {
                //_MemoryCache.TryGetValue("IssueNRGP", out MainModel);
                var jsonMainModeldata = HttpContext.Session.GetString("IssueNRGP");
                if (jsonMainModeldata != null)
                {
                    MainModel = JsonConvert.DeserializeObject<IssueNRGPModel>(jsonMainModeldata);
                }

                ItemDetailGrid = MainModel.IssueNRGPDetailGrid;
                var _ItemGrid = new List<IssueNRGPDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new IssueNRGPDetail
                 {
                     Amount = group.Sum(item => item.Amount),
                     ItemCode = group.Key,
                     PartCode = group.First().PartCode,
                     ItemName = group.First().ItemName
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }

            if (ItemDetailGrid != null && TxModel.TxType != "EXPENSES")
            {
                //_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out List<IssueNRGPTaxDetail> TaxGrid);

                var jsondata = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                List<IssueNRGPTaxDetail> TaxGrid = new List<IssueNRGPTaxDetail>();
                if (jsondata != null)
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(jsondata);
                }


                if (TaxGrid != null)
                {
                    isExp = TaxGrid.Any(m => m.TxType == "EXPENSES");
                    _List = TaxGrid;
                    //exists = TaxGrid.Select(s1 => s1.TxPartCode).ToList().Intersect(ItemDetailGrid.Select(s2 => s2.PartCode).ToList()).Any();

                    foreach (var TxItem in TaxGrid)
                    {
                        foreach (var item in ItemDetailGrid)
                        {
                            if (TxModel.TxPageName == "IssueNRGP")
                            {
                                if (TxItem.TxItemCode == item.ItemCode)
                                    exists = true;
                                break;
                            }

                        }
                    }
                }

                if (exists)
                {
                    return BadRequest();
                }

                foreach (var item in ItemDetailGrid)
                {
                    //Basic Total Amount
                    BasicTotal = BasicTotal + item.Amount;
                }
                List<string> partCodeArray = new List<string>();

                var taxGrid22 = new List<int>();
                foreach (var item in ItemDetailGrid)
                {
                    //List<IssueNRGPDetail> taxGrid1 = new List<IssueNRGPDetail>();
                    //_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out taxGrid1);

                    var jsonTaxGrid1data = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                    List<IssueNRGPDetail> taxGrid1 = new List<IssueNRGPDetail>();
                    if (jsonTaxGrid1data != null)
                    {
                        taxGrid1 = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(jsonTaxGrid1data);
                    }

                    if (taxGrid1 == null)
                    {
                        taxGrid1 = new List<IssueNRGPDetail>();
                    }
                    var checkContains = false;
                    if (TxModel.TxPageName == "IssueNRGP")
                        checkContains = partCodeArray.Contains(item.PartCode);

                    if (taxGrid22.Contains(item.ItemCode))
                    {
                        ItemAmount += item.Amount;
                    }
                    else
                    {
                        ItemAmount = item.Amount;
                    }
                    taxGrid22.Add(item.ItemCode);
                    //if (checkContains)
                    //{
                    //    //do nothing
                    //}
                    //else
                    //{
                    decimal ItemAmount1 = 0;
                    ItemCnt = ItemCnt + 1;


                    if (isExp)
                    {
                        ExpTaxAmt = TaxGrid.Where(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y").Sum(x => (decimal)x.TxAmount);
                        //TaxOnExp = ((ItemAmount / BasicTotal) * ExpTaxAmt);
                        TaxOnExp = ItemAmount == 0 && BasicTotal == 0 ? 0 : ExpTaxAmt / BasicTotal * ItemAmount;
                        ItemAmount = TaxOnExp + ItemAmount;
                    }

                    TaxAmount = ItemAmount * (decimal)TxModel.TxPercentg / 100;

                    if (TxModel.TxPageName == "IssueNRGP")
                    {
                        PartCode = item.ItemCode;
                        PartName = item.PartCode;
                        ItemCode = item.ItemCode;
                        ItemText = item.ItemName;
                    }

                    _List.Add(new IssueNRGPTaxDetail
                    {
                        SeqNo = _List.Count + 1,
                        TxType = TxModel.TxType,
                        TxPartCode = PartName,
                        TxItemCode = ItemCode,
                        TxItemName = ItemText,
                        TxTaxType = TxModel.TxTaxType,
                        TxTaxTypeName = TxModel.TxTaxTypeName,
                        TxAccountCode = TxModel.TxAccountCode,
                        TxAccountName = TxModel.TxAccountName,
                        TxPercentg = TxModel.TxPercentg,
                        TxAdInTxable = TxModel.TxAdInTxable,
                        TxRoundOff = TxModel.TxRoundOff,
                        TxAmount = TxModel.TxRoundOff == "Y" ? (float)Math.Floor(TaxAmount) : (float)Math.Round(TaxAmount, 2),
                        TxRefundable = TxModel.TxRefundable,
                        TxOnExp = (float)TaxOnExp,
                        TxRemark = TxModel.TxRemark,
                    });

                    if (CgstSgst.Rows.Count > 0 && TxModel.TxAccountName.ToUpper().Contains("CGST"))
                    {
                        int accountCodeCallCount = 0;
                        var rowIndex = accountCodeCallCount % CgstSgst.Rows.Count; // 0 first time, 1 second time
                        accountCodeCallCount++;
                        _List.Add(new IssueNRGPTaxDetail
                        {
                            SeqNo = _List.Count + 1,
                            TxType = TxModel.TxType,
                            TxPartCode = PartName,
                            TxItemCode = ItemCode,
                            TxItemName = ItemText,
                            TxTaxType = TxModel.TxTaxType,
                            TxTaxTypeName = TxModel.TxTaxTypeName,
                            TxAccountCode = ToInt32(CgstSgst.Rows[rowIndex]["Account_Code"], CI),
                            TxAccountName = CgstSgst.Rows[rowIndex]["Tax_Name"].ToString(),
                            //TxAccountCode = ToInt32(CgstSgst.Rows[1]["Account_Code"], CI),
                            //TxAccountName = CgstSgst.Rows[0]["Tax_Name"].ToString().Contains("SGST") ? CgstSgst.Rows[0]["Tax_Name"].ToString() : CgstSgst.Rows[1]["Tax_Name"].ToString(),
                            TxPercentg = TxModel.TxPercentg,
                            TxAdInTxable = TxModel.TxAdInTxable,
                            TxRoundOff = TxModel.TxRoundOff,
                            TxAmount = TxModel.TxRoundOff == "Y" ? (float)Math.Floor(TaxAmount) : (float)Math.Round(TaxAmount, 2),
                            TxRefundable = TxModel.TxRefundable,
                            TxOnExp = (float)TaxOnExp,
                            TxRemark = TxModel.TxRemark,
                        });
                    }

                    if (TxModel.TxPageName == "IssueNRGP")
                        partCodeArray.Add(item.PartCode);

                    //}
                }

                //for loop ends

                if (TxModel.TxPageName == "IssueNRGP")
                {
                    MainModel = new IssueNRGPModel();
                }

                MainModel!.IssueNRGPTaxGrid = _List;
                TaxGrid = MainModel.IssueNRGPTaxGrid;

                if (HttpContext.Session.GetString(TxModel.TxPageName) != null)
                {
                    if (TxModel.TxPageName == "IssueNRGP")
                    {
                        HttpContext.Session.Get(TxModel.TxPageName);
                        HttpContext.Session.GetString(TxModel.TxPageName);
                    }
                }

                if (TaxGrid != null)
                {
                    TotalTaxAmt = TaxGrid.Sum(x => (decimal)x.TxAmount);
                }
                TotalTaxAmt = TotalTaxAmt + (decimal)MainModel.NetAmount;
                MainModel.TotalTaxAmt = TotalTaxAmt;

                StoreInCache("KeyIssueNRGPTaxGrid", TaxGrid);
                string serializedGrid = JsonConvert.SerializeObject(TaxGrid);
                HttpContext.Session.SetString("KeyIssueNRGPTaxGrid", serializedGrid);
            }
        }
        return PartialView("_TaxNRGPGrid", MainModel);
    }
    public JsonResult CalculateTCS(string PC, string TP, string SN)
    {
        decimal Amt = 0;
        dynamic ExpTaxAmt = 0;
        decimal TaxOnExp = 0;
        decimal BasicTotal = 0;
        dynamic ListOfItems = null;

       if (SN == "SaleInvoice")
        {
            ListOfItems = new SaleBillModel();
        }
      else if(SN== "SaleRejection")
        {
            ListOfItems = new SaleRejectionModel();
        }

        if (HttpContext.Session.GetString(SN) != null)
        {
            if (SN == "SaleInvoice")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                //_MemoryCache.TryGetValue("SaleBillModel", out SaleBillModel MainModel);
                SaleBillModel MainModel = new();
                string modelJsonSale = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJsonSale))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJsonSale);
                }

                ListOfItems = MainModel.saleBillDetails;
            }

            else if (SN == "SaleRejection")
            {

                SaleRejectionModel MainModel = new();
                string modelJson1 = HttpContext.Session.GetString("SaleRejectionModel");
                if (!string.IsNullOrEmpty(modelJson1))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson1);
                }

                ListOfItems = MainModel.ItemDetailGrid;

            }
            dynamic TaxGrid = new List<TaxModel>();
            string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTaxJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
            }
            try
            {
                if (ListOfItems != null)
                {
                    foreach (var item in ListOfItems)
                    {
                      

                        //Item Amount
                        if (SN == "SaleInvoice" || SN == "SaleRejection")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += (item.Amount == null ? 0 : item.Amount);
                            }
                        }
                    
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
         foreach (var item in ListOfItems)
            {
                 if (SN == "SaleInvoice")
                {
                    Amt += item.Amount;
                }
                
            }

            string modelJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TdsGrid = new List<TaxModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                TdsGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelJson);
            }

            Amt = Amt * ToDecimal(TP) / 100;
        }

        return Json(new { Amt, TaxOnExp });

     
    }


    public JsonResult CalculateTax(string PC, string TP, string SN)
    {
        decimal Amt = 0;
        dynamic ExpTaxAmt = 0;
        decimal TaxOnExp = 0;
        decimal BasicTotal = 0;
        dynamic ListOfItems = null;

        if (SN == "ItemList")
        {
            ListOfItems = new ItemDetail();
        }
        else if (SN == "PurchaseOrder")
        {
            ListOfItems = new PurchaseOrderModel();
        }
        else if (SN == "DirectPurchaseBill")
        {
            ListOfItems = new DirectPurchaseBillModel();
        }
        else if (SN == "PurchaseBill")
        {
            ListOfItems = new PurchaseBillModel();
        }
        else if (SN == "IssueNRGP")
        {
            ListOfItems = new IssueNRGPModel();
        }
        else if (SN == "SaleInvoice")
        {
            ListOfItems = new SaleBillModel();
        }
        else if (SN == "CreditNote")
        {
            ListOfItems = new AccCreditNoteModel();
        }
        else if (SN == "PurchaseRejection")
        {
            ListOfItems = new AccPurchaseRejectionModel();
        }
        else if (SN == "SaleRejection")
        {
            ListOfItems = new SaleRejectionModel();
        }
        else
        {
            ListOfItems = new JobWorkIssueModel();
        }

        if (HttpContext.Session.GetString(SN) != null)
        {
            if (SN == "ItemList")
            {
                ListOfItems = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
            }
            else if (SN == "PurchaseOrder")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                //monika
                //_MemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
                PurchaseOrderModel MainModel = new();
                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "DirectPurchaseBill")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
                string modelJson = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJson);
                }
                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "PurchaseBill")
            {
                _MemoryCache.TryGetValue("PurchaseBill", out PurchaseBillModel MainModel);
                string modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }
                ListOfItems = MainModel.ItemDetailGridd ?? MainModel.ItemDetailGrid;
            }
            else if (SN == "SaleInvoice")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                //_MemoryCache.TryGetValue("SaleBillModel", out SaleBillModel MainModel);
                SaleBillModel MainModel = new();
                string modelJson = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJson);
                }

                ListOfItems = MainModel.saleBillDetails;
            }
            else if (SN == "CreditNote")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                //_MemoryCache.TryGetValue("CreditNoteModel", out AccCreditNoteModel MainModel);
                AccCreditNoteModel MainModel = new();
                string modelJson = HttpContext.Session.GetString("CreditNoteModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccCreditNoteModel>(modelJson);
                }
                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "PurchaseRejection")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("PurchaseRejectionModel", out AccPurchaseRejectionModel MainModel);

                string modelPRJson = HttpContext.Session.GetString("PurchaseRejectionModel");
                AccPurchaseRejectionModel purchaseRejectionModel = new AccPurchaseRejectionModel();
                if (!string.IsNullOrEmpty(modelPRJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelPRJson);
                }

                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "SaleRejection")
            {

                SaleRejectionModel MainModel = new();
                string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
                }

                ListOfItems = MainModel.ItemDetailGrid;
                
            }
            else if (SN == "IssueNRGP")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("IssueNRGP", out IssueNRGPModel MainModel);


                string modelPRJson = HttpContext.Session.GetString("IssueNRGP");
                IssueNRGPModel MainModel1 = new IssueNRGPModel();
                if (!string.IsNullOrEmpty(modelPRJson))
                {
                    MainModel = JsonConvert.DeserializeObject<IssueNRGPModel>(modelPRJson);
                }


                ListOfItems = MainModel.IssueNRGPDetailGrid;
            }
            else
            {
                var JobGrid = new List<JobWorkGridDetail>();
                _MemoryCache.TryGetValue("KeyJobWorkIssue", out List<JobWorkGridDetail> MainModel);


                //string modelJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                var jobdetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(HttpContext.Session.GetString("KeyJobWorkIssue") ?? string.Empty);


                if (jobdetail == null)
                {
                    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out List<JobWorkGridDetail> MainModel1);


                    var jobdetail1 = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(HttpContext.Session.GetString("KeyJobWorkIssueEdit") ?? string.Empty);

                    ListOfItems = jobdetail1;

                }
                else
                {
                    ListOfItems = jobdetail;
                }
            }
            try
            {
                if (ListOfItems != null)
                {
                    foreach (var item in ListOfItems)
                    {
                        if (SN != "CreditNote" && SN != "PurchaseRejection")
                        {
                            //Basic Total Amount
                            BasicTotal = BasicTotal + (item.Amount == null ? 0 : item.Amount);
                        }

                        //Item Amount
                        if (SN == "IssueNRGP" || SN == "DirectPurchaseBill" || SN == "SaleInvoice" || SN == "SaleRejection")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += (item.Amount == null ? 0 : item.Amount);
                            }
                        }
                        else if (SN == "CreditNote")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += item.ItemAmount;
                                BasicTotal = BasicTotal + (item.ItemAmount == null ? 0 : item.ItemAmount);
                            }
                        }
                        else if (SN == "PurchaseRejection")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += item.ItemAmount;
                            }
                            BasicTotal = BasicTotal + (item.ItemAmount == null ? 0 : item.ItemAmount);
                        }
                        else if (SN == "PurchaseBill")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += item.Amount;
                            }
                        }
                        else if (SN == "JobWorkIssue")
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt += item.IssQty * item.PurchasePrice;
                            }
                        }
                        else
                        {
                            if (item.ItemCode == ToInt32(PC))
                            {
                                Amt = (item.Amount == null ? 0 : item.Amount);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            IList<TaxModel> TaxGrid = new List<TaxModel>();
            //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid);
            string modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTxGridJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);
            }
            if (TaxGrid != null)
            {
                if (ListOfItems != null)
                {
                    bool isExp = TaxGrid.Any(m => m.TxType == "EXPENSES");
                    if (isExp)
                    {
                        //Total Expense Amount
                        ExpTaxAmt = TaxGrid.Where(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y").Sum(x => x.TxAmount);// .Select(x => x.TxAmount).FirstOrDefault();

                        //Tax On Expense Amount

                        //TaxOnExp = ((Amt / BasicTotal) * ExpTaxAmt);

                        //TaxOnExp = (Amt == 0 && BasicTotal == 0) ? 0 : (Amt / BasicTotal * ExpTaxAmt); //28/09/2022

                        /*
                         Formula To Calculate GST on Freight  = (Freight amt /basic amt ) * Item wise basic amt * Gst%
                         Example : (3000/130000)  * 8000 (item amt) * gst%
                        */

                        TaxOnExp = Amt == 0 && BasicTotal == 0 ? 0 : ExpTaxAmt / BasicTotal * Amt;

                        //Tax Amount
                        Amt = TaxOnExp + Amt;

                        //Amt = Amt * Convert.ToDecimal(TP) / 100;
                    }
                    else
                    {
                        Amt = Amt * Convert.ToDecimal(TP) / 100;
                    }
                }
            }
            //else
            //{
            //    Amt = Amt * Convert.ToDecimal(TP) / 100;
            //}

           // Amt = Amt * ToDecimal(TP) / 100;
        }

        return Json(new { Amt, TaxOnExp });

        /*
        //decimal Amt = 0;
        //dynamic ExpTaxAmt = 0;
        //decimal TaxOnExp = 0;

        //if (HttpContext.Session.GetString("ItemList") != null)
        //{
        //    var Result = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList")??string.Empty);

        // //Basic Total Amount decimal BasicTotal = Result.Sum(m => m.Amount);

        // //Item Amount Amt = Result.Where(m => m.PartCode == Convert.ToInt32(PC)).Select(x => x.Amount).FirstOrDefault();

        // if (_MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid)) { var MainModel = TaxGrid;

        // if (MainModel != null) { bool isExp = MainModel.Any(m => m.TxType == "EXPENSES"); if
        // (isExp) { //Total Expense Amount ExpTaxAmt = MainModel.Where(m => m.TxType == "EXPENSES"
        // && m.TxAdInTxable == "Y").Sum(x => x.TxAmount);// .Select(x => x.TxAmount).FirstOrDefault();

        // //Tax On Expense Amount

        // //TaxOnExp = ((Amt / BasicTotal) * ExpTaxAmt);

        // //TaxOnExp = (Amt == 0 && BasicTotal == 0) ? 0 : (Amt / BasicTotal * ExpTaxAmt); //28/09/2022

        // Formula To Calculate GST on Freight = (Freight amt /basic amt ) * Item wise basic amt
        // Gst% Example : (3000/130000) * 8000 (item amt) * gst%

        // TaxOnExp = (Amt == 0 && BasicTotal == 0) ? 0 : ((ExpTaxAmt / BasicTotal) * Amt);

        // //Tax Amount Amt = TaxOnExp + Amt;

        // //Amt = Amt * Convert.ToDecimal(TP) / 100; } //else //{ // Amt = Amt *
        // Convert.ToDecimal(TP) / 100; //} } } //else //{ // Amt = Amt * Convert.ToDecimal(TP) /
        // 100; //}

        //    Amt = Amt * Convert.ToDecimal(TP) / 100;
        //}

        //return Json(new { Amt, TaxOnExp });
        */
    }

    public IActionResult ClearTax(string SN)
    {
        dynamic MainModel = null;
        if (SN == "IssueNRGP")
        {
            MainModel = new IssueNRGPModel();
            //_MemoryCache.Remove("KeyIssueNRGPTaxGrid");
            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
            return PartialView("_TaxNRGPGrid", MainModel);
        }
        else
        {
            MainModel = SN == "DirectPurchaseBill" ? new DirectPurchaseBillModel() : (SN == "ItemList" ? new SaleOrderModel() : (SN == "PurchaseBill" ? new PurchaseBillModel() : new PurchaseOrderModel()));
            MainModel = SN == "JobWorkIssue" ? new JobWorkIssueModel() : "";
            MainModel = SN == "ItemList" ? new SaleOrderModel() : "";
            MainModel = SN == "PurchaseRejection" ? new AccPurchaseRejectionModel() : "";
            MainModel = SN == "SaleRejection" ? new SaleRejectionModel() : "";
            MainModel = SN == "CreditNote" ? new AccCreditNoteModel() : "";
            //_MemoryCache.Remove("KeyTaxGrid");
            HttpContext.Session.Remove("KeyTaxGrid");
            return PartialView("_TaxGrid", MainModel);
        }

        //var TxModel = new List<TaxModel>();
        //MainModel.TaxDetailGridd = TxModel;
    }

    public IActionResult DeleteTaxRow(string SeqNo, string SN)
    {
        decimal TotalTaxAmt = 0;
        dynamic MainModel = null;
        if (SN == "ItemList")
        {
            MainModel = new SaleOrderModel();
        }
        else if (SN == "PurchaseOrder")
        {
            MainModel = new PurchaseOrderModel();
        }
        else if (SN == "DirectPurchaseBill")
        {
            MainModel = new DirectPurchaseBillModel();
        }
        else if (SN == "PurchaseBill")
        {
            MainModel = new PurchaseBillModel();
        }
        else if (SN == "SaleInvoice")
        {
            MainModel = new SaleBillModel();
        }
        else if (SN == "CreditNote")
        {
            MainModel = new AccCreditNoteModel();
        }
        else if (SN == "PurchaseRejection")
        {
            MainModel = new AccPurchaseRejectionModel();
        }
        else if (SN == "SaleRejection")
        {
            MainModel = new SaleRejectionModel();
        }
        else if (SN == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }
        //MainModel = SN == "ItemList" ? new SaleOrderModel() : new PurchaseOrderModel();
        IList<TaxModel> TaxGrid = new List<TaxModel>();
        string modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
        if (!string.IsNullOrEmpty(modelTxGridJson))
        {
            TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);
        }
        //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid)
        if (TaxGrid != null)
        {
            bool canDelete = true;

            MainModel.TaxDetailGridd = TaxGrid;
            int Indx = ToInt32(SeqNo) - 1;

            string DelType = TaxGrid.Where(m => m.TxSeqNo == ToInt32(SeqNo)).Select(m => m.TxType).FirstOrDefault();

            if (DelType != null)
            {
                if (DelType == "EXPENSES")
                {
                    canDelete = TaxGrid.Where(m => m.TxSeqNo == ToInt32(SeqNo)).Select(m => m.TxAdInTxable == "N").FirstOrDefault();

                    if (!canDelete)
                    {
                        bool taxFound = TaxGrid.Any(m => m.TxType == "TAX");
                        bool Expensefound = TaxGrid.Any(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y");

                        if (taxFound == false && Expensefound)
                        {
                            canDelete = true;
                        }
                        if (taxFound && Expensefound)
                        {
                            return StatusCode(400);
                        }
                    }
                }
            }

            if (canDelete)
            {
                var Remove = TaxGrid.Where(s => s.TxSeqNo == ToInt32(SeqNo)).ToList();

                foreach (var item in Remove)
                {
                    MainModel.TaxDetailGridd.Remove(item);
                }

                Indx = 0;
                foreach (TaxModel item in MainModel.TaxDetailGridd)
                {
                    Indx++;
                    item.TxSeqNo = Indx;
                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                    SlidingExpiration = TimeSpan.FromMinutes(60),
                    Size = 1024,
                };

                //if (HttpContext.Session.GetString(SN) != null)
                //{
                //    if (SN == "ItemList")
                //    {
                //        MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(SN));
                //    }
                //    if (SN == "PurchaseOrder")
                //    {
                //        MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN));
                //    }
                //    if (SN == "JobWorkIssue")
                //    {
                //        MainModel.JobDetailGrid = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(HttpContext.Session.GetString(SN));
                //    }
                //}

                if (TaxGrid != null)
                {
                    TotalTaxAmt = TaxGrid.Sum(x => x.TxAmount);
                }
                TotalTaxAmt = TotalTaxAmt + MainModel.ItemNetAmount ?? 0;
                MainModel.TotalTaxAmt = TotalTaxAmt;

                //_MemoryCache.Set("KeyTaxGrid", TaxGrid, cacheEntryOptions);

                string serializedGrid = JsonConvert.SerializeObject(TaxGrid);
                HttpContext.Session.SetString("KeyTaxGrid", serializedGrid);
            }
            else
            {
                return StatusCode(statusCode: 300);
            }
        }
        return PartialView("_TaxGrid", MainModel);

        /*
        if (_MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid))
        {
            bool canDelete = true;
            MainModel.TaxDetailGridd = TaxGrid;
            int Indx = ToInt32(SeqNo) - 1;

            string DelType = MainModel.TaxDetailGridd.Where(m => m.TxSeqNo == ToInt32(SeqNo)).Select(m => m.TxType).FirstOrDefault();

            if (DelType != null)
            {
                if (DelType == "EXPENSES")
                {
                    canDelete = MainModel.TaxDetailGridd.Where(m => m.TxSeqNo == ToInt32(SeqNo)).Select(m => m.TxAdInTxable == "N").FirstOrDefault();

                    if (!canDelete)
                    {
                        bool taxFound = MainModel.TaxDetailGridd.Any(m => m.TxType == "TAX");
                        bool Expensefound = MainModel.TaxDetailGridd.Any(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y");

                        if (taxFound == false && Expensefound == true)
                        {
                            canDelete = true;
                        }
                        if (taxFound && Expensefound)
                        {
                            return StatusCode(400);
                        }
                    }
                }
            }

            if (canDelete)
            {
                //MainModel.TaxDetailGridd.RemoveAt(Indx);

                var Remove = MainModel.TaxDetailGridd.Where(s => s.TxSeqNo == ToInt32(SeqNo)).ToList();
                foreach (var item in Remove)
                {
                    MainModel.TaxDetailGridd.Remove(item);
                }

                Indx = 0;
                foreach (TaxModel item in MainModel.TaxDetailGridd)
                {
                    Indx++;
                    item.TxSeqNo = Indx;
                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                    SlidingExpiration = TimeSpan.FromMinutes(60),
                    Size = 1024,
                };

                if (HttpContext.Session.GetString("ItemList") != null)
                {
                    MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString("ItemList") ?? string.Empty);
                }

                MainModel.TotalTaxAmt = MainModel.TaxDetailGridd.Sum(x => x.TxAmount + MainModel.ItemNetAmount);

                _MemoryCache.Set("KeyTaxGrid", MainModel.TaxDetailGridd, cacheEntryOptions);
            }
            else
            {
                return StatusCode(statusCode: 300);
            }
        }
        return PartialView("_TaxGrid", MainModel);
        */
    }

    public IActionResult DeleteIssueTaxRow(string SeqNo, string SN)
    {
        decimal TotalTaxAmt = 0;
        dynamic MainModel = null;
        if (SN == "IssueNRGP")
        {
            MainModel = new IssueNRGPModel();
            MainModel.IssueNRGPTaxGrid = new List<IssueNRGPTaxDetail>();
        }

        if (_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out List<IssueNRGPTaxDetail> TaxGrid))
        {
            bool canDelete = true;

            MainModel.IssueNRGPTaxGrid = TaxGrid;
            int Indx = ToInt32(SeqNo) - 1;

            string DelType = TaxGrid.Where(m => m.SeqNo == ToInt32(SeqNo)).Select(m => m.TxType).FirstOrDefault();

            if (DelType != null)
            {
                if (DelType == "EXPENSES")
                {
                    canDelete = TaxGrid.Where(m => m.SeqNo == ToInt32(SeqNo)).Select(m => m.TxAdInTxable == "N").FirstOrDefault();

                    if (!canDelete)
                    {
                        bool taxFound = TaxGrid.Any(m => m.TxType == "TAX");
                        bool Expensefound = TaxGrid.Any(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y");

                        if (taxFound == false && Expensefound)
                        {
                            canDelete = true;
                        }
                        if (taxFound && Expensefound)
                        {
                            return StatusCode(400);
                        }
                    }
                }
            }

            if (canDelete)
            {
                var Remove = TaxGrid.Where(s => s.SeqNo == ToInt32(SeqNo)).ToList();
                foreach (var item in Remove)
                {
                    MainModel.IssueNRGPTaxGrid.Remove(item);
                }

                Indx = 0;
                foreach (IssueNRGPTaxDetail item in MainModel.IssueNRGPTaxGrid)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                    SlidingExpiration = TimeSpan.FromMinutes(60),
                    Size = 1024,
                };

                if (TaxGrid != null)
                {
                    TotalTaxAmt = TaxGrid.Sum(x => (decimal)x.TxAmount);
                }
                TotalTaxAmt = TotalTaxAmt + (decimal)MainModel.NetAmount;
                MainModel.TotalTaxAmt = TotalTaxAmt;

                _MemoryCache.Set("KeyIssueNRGPTaxGrid", TaxGrid, cacheEntryOptions);
            }
            else
            {
                return StatusCode(statusCode: 300);
            }
        }
        return PartialView("_IssueTaxGrid", MainModel);
    }

    public async Task<IActionResult> GetHsnTaxInfo(int AC, string TxPageName, string RF)
    {
        var isSuccess = string.Empty;

        dynamic MainModel = null;

        var TaxGrid = new List<TaxModel>();
        var IssueTaxGrid = new List<IssueNRGPTaxDetail>();

        switch (TxPageName)
        {
            case "ItemList":
                HttpContext.Session.Get(TxPageName);
                MainModel = new SaleOrderModel();
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TxPageName));
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                MainModel.TxRoundOff = RF;
                isSuccess = ValidateHsnTax(MainModel);
                if (isSuccess != "SuccessFull")
                {
                    return Content(isSuccess);
                }
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;

            case "PurchaseOrder":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "DirectPurchaseBill":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "PurchaseBill":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("PurchaseBill", out MainModel);
                 modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "SaleInvoice":
                HttpContext.Session.Get(TxPageName);
                //_MemoryCache.TryGetValue("SaleBillModel", out MainModel);
                string modelSaleInvoiceJson = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelSaleInvoiceJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelSaleInvoiceJson);
                }

                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "CreditNote":
                HttpContext.Session.Get(TxPageName);
                //_MemoryCache.TryGetValue("CreditNoteModel", out MainModel);
                string modelCreditNoteJson = HttpContext.Session.GetString("CreditNoteModel");
                if (!string.IsNullOrEmpty(modelCreditNoteJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccCreditNoteModel>(modelCreditNoteJson);
                }

                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "PurchaseRejection":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("PurchaseRejectionModel", out MainModel);
                string modelPRJson = HttpContext.Session.GetString("PurchaseRejectionModel");
                AccPurchaseRejectionModel purchaseRejectionModel = new AccPurchaseRejectionModel();
                if (!string.IsNullOrEmpty(modelPRJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelPRJson);
                }
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;
            case "SaleRejection":
                HttpContext.Session.Get(TxPageName);
                //_MemoryCache.TryGetValue("SaleBillModel", out MainModel);
                string modelSaleInvoiceJson1 = HttpContext.Session.GetString("SaleRejectionModel");
                if (!string.IsNullOrEmpty(modelSaleInvoiceJson1))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelSaleInvoiceJson1);
                }

                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);

                //HttpContext.Session.Get(TxPageName);
                //_MemoryCache.TryGetValue("SaleRejectionModel", out MainModel);
                //MainModel.AccountCode = AC;
                //MainModel.TxPageName = TxPageName;
                //TaxGrid = await GetHSNTaxList(MainModel);
                break;
            case "IssueNRGP":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("IssueNRGP", out MainModel);

                string modelICJson = HttpContext.Session.GetString("IssueNRGP");
                if (!string.IsNullOrEmpty(modelICJson))
                {
                    MainModel = JsonConvert.DeserializeObject<dynamic>(modelICJson);
                }
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                IssueTaxGrid = await GetHSNIssueTaxList(MainModel);

                break;
            case "JobWorkIssue":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("JobWorkIssue", out MainModel);
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TaxGrid = await GetHSNTaxList(MainModel);
                if (TaxGrid.Count == 1 && !string.IsNullOrEmpty(TaxGrid[0].Message))
                    return Content(TaxGrid[0].Message);
                break;

            default:
                Console.WriteLine("No Page Defined.");
                break;
        }
        if (TxPageName == "IssueNRGP")
        {
            MainModel.IssueNRGPTaxGrid = IssueTaxGrid;
            //StoreInCache("KeyIssueNRGPTaxGrid", MainModel.IssueNRGPTaxGrid);
            //return PartialView("_IssueTaxGrid", MainModel);

            string serializedGrid = JsonConvert.SerializeObject(MainModel.IssueNRGPTaxGrid);
            HttpContext.Session.SetString("KeyTaxGrid", serializedGrid);

            return PartialView("_IssueTaxGrid", MainModel);

        }
        else
        {
            MainModel.TaxDetailGridd = TaxGrid;
            StoreInCache("KeyTaxGrid", MainModel.TaxDetailGridd);

            string serializedGrid = JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
            HttpContext.Session.SetString("KeyTaxGrid", serializedGrid);
        }

        return PartialView("_TaxGrid", MainModel);
    }

    public async Task<IActionResult> GetTaxByType(string TxType)
    {
        IList<TextValue> Result = await IDataLogic.GetDropDownList("TAXBYTYPE", TxType, "SP_GetDropDownList");
        return Json(Result);
    }

    public IActionResult GetTaxInfo()
    {
        bool isTax = false;
        bool isExp = false;
       IList<TaxModel> TaxGrid = new List<TaxModel>();
        //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid);
        string modelJson = HttpContext.Session.GetString("KeyTaxGrid");
        if (!string.IsNullOrEmpty(modelJson))
        {
            TaxGrid = JsonConvert.DeserializeObject<IList<TaxModel>>(modelJson);
        }
        if (TaxGrid != null && TaxGrid.Count > 0)
            isTax = true;
        if (isTax)
        {
            isExp = TaxGrid.Any(m => m.TxType == "EXPENSES");
            isTax = TaxGrid.Any(m => m.TxType == "TAX");
        }

        return Ok(new { isTax });
    }
    public IActionResult GetIssueTaxInfo()
    {
        bool isTax = false;
        bool isExp = false;
        //_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out IList<IssueNRGPTaxDetail> TaxGrid);
        List<IssueNRGPTaxDetail> taxGrid = new();
        string modelJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
        if (!string.IsNullOrEmpty(modelJson) && taxGrid == null)
        {
            taxGrid = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(modelJson);
        }
        if (taxGrid != null && taxGrid.Count > 0)
            isTax = true;
        if (isTax)
        {
            isExp = taxGrid.Any(m => m.TxType == "EXPENSES");
            isTax = taxGrid.Any(m => m.TxType == "TAX");
        }

        return Ok(new { isTax });
    }

    [HttpGet]
    public JsonResult GetTaxPartItem(string SessionName)
    {
        List<TextValue> PartCode = new();
        List<TextValue> ItemCode = new();
        dynamic ItemModel = null;

        if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionName)))
        {
            if (SessionName == "ItemList")
            {
                ItemModel = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(SessionName) ?? string.Empty);
                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartText,
                            Value = item.PartCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemText,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }

            if (SessionName == "DirectPurchaseBill")
            {
                _MemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
                string modelJson = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJson);
                }
                ItemModel = MainModel.ItemDetailGrid;
                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartText,
                            Value = item.PartCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemText,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }

            if (SessionName == "PurchaseBill")
            {
                _MemoryCache.TryGetValue("PurchaseBill", out PurchaseBillModel MainModel);
                var modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }
                ItemModel = MainModel.ItemDetailGridd;
                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        if (PartCode != null && (!PartCode.Any() || !PartCode.Where(a => a.Text == item.Item_Name).ToList().Any()))
                        {
                            PartCode.Add(new TextValue
                            {
                                Text = item.PartCode.ToString(),
                                Value = item.ItemCode.ToString()
                            });
                        }
                        if (ItemCode != null && (!ItemCode.Any() || !ItemCode.Where(a => a.Text == item.Item_Name).ToList().Any()))
                        {
                            ItemCode.Add(new TextValue
                            {
                                Text = item.Item_Name.ToString(),
                                Value = item.ItemCode.ToString()
                            });
                        }
                    }
                }
            }

            if (SessionName == "SaleInvoice")
            {
                //_MemoryCache.TryGetValue("KeySaleBillGrid", out IList<SaleBillDetail> saleBillDetail);
                //ItemModel = saleBillDetail;


                var modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ItemModel = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }
                //ItemModel = MainModel.ItemDetailGridd;

                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }

            if (SessionName == "CreditNote")
            {
                var modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteDetail = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }
                //_MemoryCache.TryGetValue("KeyCreditNoteGrid", out IList<AccCreditNoteDetail> creditNoteDetail);
                ItemModel = creditNoteDetail;

                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }
            if (SessionName == "PurchaseRejection")
            {
                //_MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out IList<AccPurchaseRejectionDetail> purchaseRejectionDetail);
                string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
                List<AccPurchaseRejectionDetail> purchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
                if (!string.IsNullOrEmpty(modelPRGridJson))
                {
                    purchaseRejectionDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
                }
                ItemModel = purchaseRejectionDetail;

                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }

            if (SessionName == "SaleRejection")
            {
                string modelPRGridJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                List<SaleRejectionDetail> SaleRejectionDetail = new List<SaleRejectionDetail>();
                if (!string.IsNullOrEmpty(modelPRGridJson))
                {
                    SaleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelPRGridJson);
                }
                ItemModel = SaleRejectionDetail;

                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
                //_MemoryCache.TryGetValue("KeySaleRejectionGrid", out IList<SaleRejectionDetail> saleRejectionDetail);
                //ItemModel = saleRejectionDetail;

                //if (ItemModel != null && ItemModel?.Count > 0)
                //{
                //    foreach (var item in ItemModel)
                //    {
                //        PartCode.Add(new TextValue
                //        {
                //            Text = item.PartCode,
                //            Value = item.ItemCode.ToString()
                //        });

                //        ItemCode.Add(new TextValue
                //        {
                //            Text = item.ItemName,
                //            Value = item.ItemCode.ToString()
                //        });
                //    }
                //}
            }
            if (SessionName == "PurchaseOrder")
            {
                //_MemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);

                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                PurchaseOrderModel MainModel = new PurchaseOrderModel();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
                ItemModel = MainModel.ItemDetailGrid;
                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartText,
                            Value = item.PartCode.ToString()
                        });

                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemText,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }
            //if (SessionName == "JobWorkIssue")
            //{
            //    _MemoryCache.TryGetValue("KeyJobWorkIssue", out List<JobWorkGridDetail> JobWorkGrid);
            //    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out List<JobWorkGridDetail> JobWorkGridEdit);

            //    ItemModel = JobWorkGrid ?? JobWorkGridEdit;
            //    if (ItemModel != null && ItemModel?.Count > 0)
            //    {
            //        foreach (var item in ItemModel)
            //        {
            //            PartCode.Add(new TextValue
            //            {
            //                Text = item.PartCode,
            //                Value = item.ItemCode.ToString()
            //            });
            //            ItemCode.Add(new TextValue
            //            {
            //                Text = item.ItemName,
            //                Value = item.ItemCode.ToString()
            //            });
            //        }
            //    }
            //}

            if (SessionName == "JobWorkIssue")
            {
                List<JobWorkGridDetail> JobWorkGrid = new();
                List<JobWorkGridDetail> JobWorkGridEdit = new();

                string jobWorkGridJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                if (!string.IsNullOrEmpty(jobWorkGridJson))
                {
                    JobWorkGrid = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(jobWorkGridJson);
                }

                string jobWorkGridEditJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
                if (!string.IsNullOrEmpty(jobWorkGridEditJson))
                {
                    JobWorkGridEdit = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(jobWorkGridEditJson);
                }

                ItemModel = JobWorkGrid?.Count > 0 ? JobWorkGrid : JobWorkGridEdit;

                if (ItemModel != null && ItemModel.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });
                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }

            if (SessionName == "IssueNRGP")
            {
                _MemoryCache.TryGetValue("KeyIssueNRGPGrid", out IList<IssueNRGPDetail> GridDetail);

                string jsondata = HttpContext.Session.GetString("KeyIssueNRGPGrid");
                if (!string.IsNullOrEmpty(jsondata))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(jsondata);
                }
                ItemModel = GridDetail;
                if (ItemModel != null && ItemModel?.Count > 0)
                {
                    foreach (var item in ItemModel)
                    {
                        PartCode.Add(new TextValue
                        {
                            Text = item.PartCode,
                            Value = item.ItemCode.ToString()
                        });
                        ItemCode.Add(new TextValue
                        {
                            Text = item.ItemName,
                            Value = item.ItemCode.ToString()
                        });
                    }
                }
            }
        }
        return Json(new { PartCode, ItemCode });
    }

    public string GetTaxPercentage(string TxCode)
    {
        string Result = ITaxModule.GetTaxPercentage("TXCODE", TxCode);
        return Result;
    }
    public async Task<IActionResult> FillTaxType(string Flag)
    {
        DataSet oDataSet = await IDataLogic.GetDropDownList(Flag);

        List<TextValue> TxType = new List<TextValue>();
        if (Flag == "FillTaxType")
        {
            if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[0].Rows.Count > 0)
            {
                TxType = (
                    from DataRow dr in oDataSet.Tables[0].Rows
                    select new TextValue
                    {
                        Text = dr["TaxType"].ToString(),
                        Value = dr["TaxID"].ToString(),
                    }).ToList();
            }
        }
        return Json(new { TxType });
    }
    public async Task<IActionResult> GetTaxAccountName(string Flag, int AccountCode, string TaxType, string vendorStateCode = "")
    {
        DataSet oDataSet = await IDataLogic.GetDropDownList(Flag, AccountCode, TaxType, vendorStateCode);

        List<TextValue> TxName = new List<TextValue>();
        if (Flag == "TAX")
        {
            if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[0].Rows.Count > 0)
            {
                TxName = (
                    from DataRow dr in oDataSet.Tables[0].Rows
                    select new TextValue
                    {
                        Text = dr["Tax_Name"].ToString(),
                        Value = dr["Account_Code"].ToString(),
                    }).ToList();
            }
        }
        return Json(new { TxName });
    }

    public async Task<IActionResult> GetTaxTypeName(string Flag)
    {
        DataSet oDataSet = await IDataLogic.GetDropDownList(Flag);

        List<TextValue> TxType = new List<TextValue>();
        List<TextValue> TxName = new List<TextValue>();

        //if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[0].Rows.Count > 0)
        //{
        //    PartCode = CommonFunc.ConvertDataTable<TextValue>(oDataSet.Tables[0]);
        //}

        if (Flag == "EXPENSES")
        {
            if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[0].Rows.Count > 0)
            {
                TxType = (
                    from DataRow dr in oDataSet.Tables[0].Rows
                    select new TextValue
                    {
                        Text = dr["TaxType"].ToString(),
                        Value = dr["TaxID"].ToString(),
                    }).ToList();
            }

            if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[1].Rows.Count > 0)
            {
                TxName = (
                    from DataRow dr in oDataSet.Tables[1].Rows
                    select new TextValue
                    {
                        Text = dr["Account_Name"].ToString(),
                        Value = dr["Account_Code"].ToString(),
                    }).ToList();
            }
        }

        //if (Flag == "TAX")
        //{
        //    if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[1].Rows.Count > 0)
        //    {
        //        TxName = (
        //            from DataRow dr in oDataSet.Tables[1].Rows
        //            select new TextValue
        //            {
        //                Text = dr["Tax_Name"].ToString(),
        //                Value = dr["Account_Code"].ToString(),
        //            }).ToList();
        //    }
        //}

        return Json(new { TxType, TxName });
    }

    private async Task<List<TaxModel>> GetHSNTaxList(dynamic MainModel)
    {
        try
        {
            var HSNTAXParam = new HSNTAX();
            var TaxGrid = new List<TaxModel>();
            var HSNTaxDetail = new HSNTAXInfo();
            //_MemoryCache.TryGetValue("KeyTaxGrid", out TaxGrid);
            string modelJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelJson);
            }

            if (TaxGrid == null)
                TaxGrid = new List<TaxModel>();

            var grid = MainModel;
            if (MainModel.TxPageName == "JobWorkIssue")
            {
                var JobGrid = new List<JobWorkGridDetail>();
                _MemoryCache.TryGetValue("KeyJobWorkIssue", out JobGrid);
                if (JobGrid == null)
                    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out JobGrid);

                grid = JobGrid;
            }
            else if (MainModel.TxPageName == "PurchaseBill")
            {
                grid = MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0 ? MainModel.ItemDetailGrid : MainModel.ItemDetailGridd;
            }
            else
            {
                grid = MainModel.ItemDetailGrid;
            }

            List<string> partCodeCheck = new List<string>();
            foreach (var item in grid)
            {
                HSNTAXParam.HSNNo = (MainModel != null && MainModel.TxPageName == "PurchaseBill") ? item.HSNNO : (item.HSNNo != null && !string.IsNullOrEmpty(item.HSNNo.ToString()) ? Convert.ToInt32(item.HSNNo) : 0);
                HSNTAXParam.AC = MainModel.AccountCode;
                HSNTAXParam.ItemCode = item.ItemCode;


				if (MainModel.TxPageName == "PurchaseBill")
                {
                    MainModel.HSNNO = item.HSNNO;
                }
                else
                {
                    MainModel.HSNNo = item.HSNNo;
                }

                string partCode = "";
                if (MainModel.TxPageName == "JobWorkIssue" || MainModel.TxPageName == "SaleInvoice" || MainModel.TxPageName == "CreditNote" || MainModel.TxPageName == "PurchaseRejection" || MainModel.TxPageName == "PurchaseBill" || MainModel.TxPageName == "SaleRejection")
                    partCode = item.PartCode;
                else
                    partCode = item.PartText;

                if (partCodeCheck.Contains(partCode))
                {
                    //do nothing
                }
                else
                {
                    //if(TaxGrid.TxPartName == item.PartCode)

                    var paramResult = await ITaxModule.GetHSNTaxInfo(HSNTAXParam);
                    
                    if ( paramResult.Result != null && paramResult.Result?.Rows.Count > 0)
                    {
                        if (paramResult.Result.Rows[0]["Status"] != "SuccessFull")
                       return new List<TaxModel> { new TaxModel { Message = $"Error: {paramResult.Result.Rows[0]["Status"]}" } };

                        foreach (DataRow dataRow in paramResult.Result.Rows)
                        {
                            //HSNTaxDetail.AddInTaxable = dataRow["AddInTaxable"].ToString();
                            HSNTaxDetail.AddInTaxable = "N";
                            HSNTaxDetail.LocalCen = dataRow["LocalCen"].ToString();
                            HSNTaxDetail.Refundable = dataRow["Refundable"].ToString();
                            HSNTaxDetail.SGSTAccountCode = ToInt32(dataRow["SGSTAccountCode"]);
                            HSNTaxDetail.SGSTTaxName = dataRow["SGSTTaxName"].ToString();
                            HSNTaxDetail.CGSTAccountCode = ToInt32(dataRow["CGSTAccountCode"]);
                            HSNTaxDetail.CGSTTaxName = dataRow["CGSTTaxName"].ToString();
                            HSNTaxDetail.TaxType = dataRow["TaxType"].ToString();
                            HSNTaxDetail.TaxPercent = ToInt32(dataRow["TaxPercent"]);
                            if (MainModel.TxPageName == "JobWorkIssue" || MainModel.TxPageName == "SaleInvoice" || MainModel.TxPageName == "SaleRejection" || MainModel.TxPageName == "CreditNote" || MainModel.TxPageName == "PurchaseRejection")
                            {
                                HSNTaxDetail.ItemCode = item.ItemCode;
                                HSNTaxDetail.ItemName = item.ItemName;
                                HSNTaxDetail.PartName = item.PartCode;
                            }
                            else if (MainModel.TxPageName == "PurchaseBill")
                            {
                                HSNTaxDetail.ItemCode = item.ItemCode;
                                HSNTaxDetail.ItemName = item.Item_Name;
                                HSNTaxDetail.PartName = item.PartCode;
                            }
                            else
                            {
                                HSNTaxDetail.ItemCode = item.ItemCode;
                                HSNTaxDetail.ItemName = item.ItemText;
                                HSNTaxDetail.PartName = item.PartText;
                            }
                        }

                        if (paramResult.Result.Rows[0]["LocalCen"] == "CGST" || paramResult.Result.Rows[0]["LocalCen"] == "SGST")
                        {
                            PrepareTaxList(HSNTaxDetail, ref TaxGrid, "CGST", MainModel);
                            PrepareTaxList(HSNTaxDetail, ref TaxGrid, "SGST", MainModel);
                        }
                        else
                        {
                            PrepareTaxList(HSNTaxDetail, ref TaxGrid, "IGST", MainModel);
                        }
                    }
                }
                partCodeCheck.Add(partCode);
            }

            return TaxGrid;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<List<IssueNRGPTaxDetail>> GetHSNIssueTaxList(dynamic MainModel)
    {
        var HSNTAXParam = new HSNTAX();
        var IssueTaxGrid = new List<IssueNRGPTaxDetail>();
        var HSNTaxDetail = new HSNTAXInfo();
        //_MemoryCache.TryGetValue("KeyIssueNRGPTaxGrid", out IssueTaxGrid);

        string modelICJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
        if (!string.IsNullOrEmpty(modelICJson))
        {
            MainModel = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(modelICJson);
        }

        if (IssueTaxGrid == null)
            IssueTaxGrid = new List<IssueNRGPTaxDetail>();

        var grid = MainModel;
        if (MainModel.TxPageName == "IssueNRGP")
        {
            var IssueGrid = new List<IssueNRGPDetail>();
            //_MemoryCache.TryGetValue("KeyIssueNRGPGrid", out IssueGrid);
            string modelICDetailJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
             
            if (!string.IsNullOrEmpty(modelICDetailJson))
            {
                IssueGrid = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelICDetailJson);
            }
            grid = IssueGrid;
        }


        List<string> partCodeCheck = new List<string>();
        foreach (var item in grid)
        {
            HSNTAXParam.HSNNo = item.HSNNo;
            HSNTAXParam.AC = MainModel.AccountCode;
            MainModel.HSNNo = item.HSNNo;
            string partCode = "";
            if (MainModel.TxPageName == "IssueNRGP")
                partCode = item.PartCode;


            if (partCodeCheck.Contains(partCode))
            {
                //do nothing
            }
            else
            {
                //if(TaxGrid.TxPartName == item.PartCode)

                var paramResult = await ITaxModule.GetHSNTaxInfo(HSNTAXParam);

                if (paramResult.StatusText == "Success" && paramResult.Result != null && paramResult.Result?.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in paramResult.Result.Rows)
                    {
                        HSNTaxDetail.AddInTaxable = dataRow["AddInTaxable"].ToString();
                        HSNTaxDetail.LocalCen = dataRow["LocalCen"].ToString();
                        HSNTaxDetail.Refundable = dataRow["Refundable"].ToString();
                        HSNTaxDetail.SGSTAccountCode = ToInt32(dataRow["SGSTAccountCode"]);
                        HSNTaxDetail.SGSTTaxName = dataRow["SGSTTaxName"].ToString();
                        HSNTaxDetail.CGSTAccountCode = ToInt32(dataRow["CGSTAccountCode"]);
                        HSNTaxDetail.CGSTTaxName = dataRow["CGSTTaxName"].ToString();
                        HSNTaxDetail.TaxType = dataRow["TaxType"].ToString();
                        HSNTaxDetail.TaxPercent = ToInt32(dataRow["TaxPercent"]);
                        if (MainModel.TxPageName == "IssueNRGP")
                        {
                            HSNTaxDetail.ItemCode = item.ItemCode;
                            HSNTaxDetail.ItemName = item.ItemName;
                            HSNTaxDetail.PartName = item.PartCode;
                        }

                    }

                    if (paramResult.Result.Rows[0]["LocalCen"] == "CGST" || paramResult.Result.Rows[0]["LocalCen"] == "SGST")
                    {
                        PrepareIssueTaxList(HSNTaxDetail, ref IssueTaxGrid, "CGST", MainModel);
                        PrepareIssueTaxList(HSNTaxDetail, ref IssueTaxGrid, "SGST", MainModel);
                    }
                    else
                    {
                        PrepareIssueTaxList(HSNTaxDetail, ref IssueTaxGrid, "IGST", MainModel);
                    }
                }
            }
            partCodeCheck.Add(partCode);
        }

        return IssueTaxGrid;
    }
    private void PrepareIssueTaxList(HSNTAXInfo HSNTaxDetail, ref List<IssueNRGPTaxDetail> ITaxGrid, string CGSTSGST, dynamic MainModel)
    {
        var TaxValue = CalculateTax(HSNTaxDetail.ItemCode.ToString(), HSNTaxDetail.TaxPercent.ToString(), MainModel.TxPageName);
        var TaxAmt = JToken.Parse(JsonConvert.SerializeObject(TaxValue.Value));

        if (CGSTSGST == "CGST" || CGSTSGST == "SGST")
        {
            ITaxGrid.Add(new IssueNRGPTaxDetail
            {
                SeqNo = ITaxGrid == null ? 1 : ITaxGrid.Count + 1,
                TxType = "TAX",
                TxPartCode = HSNTaxDetail.PartName,
                TxItemCode = HSNTaxDetail.ItemCode,
                TxItemName = HSNTaxDetail.ItemName,
                TxTaxType = 25,
                TxTaxTypeName = HSNTaxDetail.TaxType,
                TxAccountCode = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTAccountCode : HSNTaxDetail.SGSTAccountCode,
                TxAccountName = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTTaxName : HSNTaxDetail.SGSTTaxName,
                TxPercentg = HSNTaxDetail.TaxPercent,
                TxAdInTxable = HSNTaxDetail.AddInTaxable,
                TxRoundOff = MainModel.TxRoundOff == "YES" ? "Y" : "N",
                TxAmount = MainModel.TxRoundOff == "YES" ? Math.Round(TaxAmt["Amt"]) : TaxAmt["Amt"],
                TxRefundable = HSNTaxDetail.Refundable,
                TxOnExp = TaxAmt["TaxOnExp"],
                TxRemark = "Tax Addedd From HSN # " + MainModel.HSNNo,
            });
        }
        else
        {
            ITaxGrid.Add(new IssueNRGPTaxDetail
            {
                SeqNo = ITaxGrid == null ? 1 : ITaxGrid.Count + 1,
                TxType = "TAX",
                TxPartCode = HSNTaxDetail.PartName,
                TxItemCode = HSNTaxDetail.ItemCode,
                TxItemName = HSNTaxDetail.ItemName,
                TxTaxType = 25,
                TxTaxTypeName = HSNTaxDetail.TaxType,
                TxAccountCode = HSNTaxDetail.CGSTAccountCode,
                TxAccountName = HSNTaxDetail.CGSTTaxName,
                TxPercentg = HSNTaxDetail.TaxPercent,
                TxAdInTxable = HSNTaxDetail.AddInTaxable,
                TxRoundOff = MainModel.TxRoundOff == "YES" ? "Y" : "N",
                TxAmount = MainModel.TxRoundOff == "YES" ? Math.Round(TaxAmt["Amt"]) : TaxAmt["Amt"],
                TxRefundable = HSNTaxDetail.Refundable,
                TxOnExp = TaxAmt["TaxOnExp"],
                TxRemark = "Tax Addedd From HSN # " + MainModel.HSNNo,
            });
        }
    }
    private void PrepareTaxList(HSNTAXInfo HSNTaxDetail, ref List<TaxModel> TaxGrid, string CGSTSGST, dynamic MainModel)
    {
        var TaxValue = CalculateTax(HSNTaxDetail.ItemCode.ToString(), HSNTaxDetail.TaxPercent.ToString(), MainModel.TxPageName);
        var TaxAmt = JToken.Parse(JsonConvert.SerializeObject(TaxValue.Value));

        if (CGSTSGST == "CGST" || CGSTSGST == "SGST")
        {
            TaxGrid.Add(new TaxModel
            {
                TxSeqNo = TaxGrid == null ? 1 : TaxGrid.Count + 1,
                TxType = "TAX",
                TxPartCode = HSNTaxDetail.ItemCode,
                TxPartName = HSNTaxDetail.PartName,
                TxItemCode = HSNTaxDetail.ItemCode,
                TxItemName = HSNTaxDetail.ItemName,
                TxTaxType = 25,
                TxTaxTypeName = HSNTaxDetail.TaxType,
                TxAccountCode = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTAccountCode : HSNTaxDetail.SGSTAccountCode,
                TxAccountName = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTTaxName : HSNTaxDetail.SGSTTaxName,
                TxPercentg = HSNTaxDetail.TaxPercent,
                TxAdInTxable = HSNTaxDetail.AddInTaxable,
                TxRoundOff = MainModel.TxRoundOff == "YES" ? "Y" : "N",
                TxAmount = MainModel.TxRoundOff == "YES" ? Math.Round((decimal)TaxAmt["Amt"]) : (decimal)TaxAmt["Amt"],
                TxRefundable = HSNTaxDetail.Refundable,
                TxOnExp = (decimal)TaxAmt["TaxOnExp"],
                TxRemark = "Tax Addedd From HSN # " + ((MainModel.TxPageName == "PurchaseBill") ? MainModel.HSNNO : MainModel.HSNNo),
            });
        }
        else
        {
            TaxGrid.Add(new TaxModel
            {
                TxSeqNo = TaxGrid == null ? 1 : TaxGrid.Count + 1,
                TxType = "TAX",
                TxPartCode = HSNTaxDetail.ItemCode,
                TxPartName = HSNTaxDetail.PartName,
                TxItemCode = HSNTaxDetail.ItemCode,
                TxItemName = HSNTaxDetail.ItemName,
                TxTaxType = 25,
                TxTaxTypeName = HSNTaxDetail.TaxType,
                TxAccountCode = HSNTaxDetail.CGSTAccountCode,
                TxAccountName = HSNTaxDetail.CGSTTaxName,
                TxPercentg = HSNTaxDetail.TaxPercent,
                TxAdInTxable = HSNTaxDetail.AddInTaxable,
                TxRoundOff = MainModel.TxRoundOff == "YES" ? "Y" : "N",
                TxAmount = MainModel.TxRoundOff == "YES" ? Math.Round((decimal)TaxAmt["Amt"]) : (decimal)TaxAmt["Amt"],
                TxRefundable = HSNTaxDetail.Refundable,
                TxOnExp = (decimal)TaxAmt["TaxOnExp"],
                TxRemark = "Tax Addedd From HSN # " + ((MainModel.TxPageName == "PurchaseBill") ? MainModel.HSNNO : MainModel.HSNNo),
            });
        }
    }

    private void StoreInCache(string CacheKey, object CacheObject)
    {
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };

        _MemoryCache.Set(CacheKey, CacheObject, cacheEntryOptions);
    }

    private string ValidateHsnTax(dynamic MainModel)
    {
        var ErrMsg = string.Empty;

        foreach (var item in MainModel.ItemDetailGrid)
        {
            var HSNTAXParam = new HSNTAX();
            HSNTAXParam.HSNNo = ((MainModel.TxPageName == "PurchaseBill") ? item.HSNNO : item.HSNNo);
            HSNTAXParam.AC = MainModel.AccountCode;
            if (MainModel.TxPageName == "PurchaseBill")
            {
                MainModel.HSNNO = item.HSNNO;
            }
            else
            {
                MainModel.HSNNo = item.HSNNo;
            }

            var paramResult = Task.FromResult(ITaxModule.GetHSNTaxInfo(HSNTAXParam)).Result;
            ErrMsg = paramResult.Result.Result?.Rows[0]["Status"].ToString();
            if (ErrMsg != "SuccessFull")
                return ErrMsg;
        }

        return ErrMsg;
    }

    public IActionResult DeleteTaxRowByItemCode(string SN, int ItemCode)
    {
        decimal TotalTaxAmt = 0;
        dynamic MainModel = null;

        // Initialize model based on SN
        if (SN == "ItemList") MainModel = new SaleOrderModel();
        else if (SN == "PurchaseOrder") MainModel = new PurchaseOrderModel();
        else if (SN == "DirectPurchaseBill") MainModel = new DirectPurchaseBillModel();
        else if (SN == "PurchaseBill") MainModel = new PurchaseBillModel();
        else if (SN == "SaleInvoice") MainModel = new SaleBillModel();
        else if (SN == "CreditNote") MainModel = new AccCreditNoteModel();
        else if (SN == "PurchaseRejection") MainModel = new AccPurchaseRejectionModel();
        else if (SN == "SaleRejection") MainModel = new SaleRejectionModel();
        else if (SN == "JobWorkIssue") MainModel = new JobWorkIssueModel();

        // Get tax grid from session
        IList<TaxModel> TaxGrid = new List<TaxModel>();
        string modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");

        if (!string.IsNullOrEmpty(modelTxGridJson))
        {
            TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);
        }

        if (TaxGrid != null)
        {
            MainModel.TaxDetailGridd = TaxGrid;

            // Cast to List<TaxModel> for LINQ use
            var taxList = MainModel.TaxDetailGridd as List<TaxModel>;

            if (taxList != null)
            {
                // Remove all rows for the given ItemCode
                var removeList = taxList.Where(x => x.TxItemCode == ItemCode).ToList();

                foreach (var item in removeList)
                {
                    taxList.Remove(item);
                }

                // Reindex TxSeqNo for that ItemCode
                int index = 0;
                foreach (var item in taxList.Where(x => x.TxItemCode == ItemCode))
                {
                    index++;
                    item.TxSeqNo = index;
                }

                // Recalculate total tax amount
                TotalTaxAmt = taxList.Sum(x => x.TxAmount);
                TotalTaxAmt += MainModel.ItemNetAmount ?? 0;
                MainModel.TotalTaxAmt = TotalTaxAmt;

                // Save updated tax list back to session
                string serializedGrid = JsonConvert.SerializeObject(taxList);
                HttpContext.Session.SetString("KeyTaxGrid", serializedGrid);
            }
        }

        return PartialView("_TaxGrid", MainModel);
    }
}