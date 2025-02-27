using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Convert;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkIssueModel;
using eTactWeb.DOM.Models;
namespace eTactWeb.Controllers;

[Authorize]
public class TDSController : Controller
{
    public TDSController(IMemoryCache imemoryCache, ILogger<SaleOrderController> logger, IDataLogic iDataLogic, ITDSModule iTDSModule)
    {
        _MemoryCache = imemoryCache;
        Logger = logger;
        IDataLogic = iDataLogic;
        ITDSModule = iTDSModule;
    }

    public IDataLogic IDataLogic { get; }
    public ITDSModule ITDSModule { get; }
    public ILogger<SaleOrderController> Logger { get; }
    private IMemoryCache _MemoryCache { get; }

    public IList<TDSModel> Add2List(TDSModel model, IList<TDSModel> TdsGrid, DataTable CgstSgst)
    {
        var _List = new List<TDSModel>();

        _List.Add(new TDSModel
        {
            TDSSeqNo = TdsGrid == null ? 1 : TdsGrid.Count + 1,
            TDSTaxType = model.TDSTaxType,
            TDSTaxTypeName = model.TDSTaxTypeName,
            TDSAccountCode = CgstSgst != null && CgstSgst.Rows.Count > 0 ? ToInt32(CgstSgst.Rows[1]["Account_Code"]) : model.TDSAccountCode,
            TDSAccountName = CgstSgst != null && CgstSgst.Rows.Count > 0 ? CgstSgst.Rows[1]["TDS_Name"].ToString() : model.TDSAccountName,
            TDSPercentg = model.TDSPercentg,
            TDSRoundOff = model.TDSRoundOff,
            TDSAmount = model.TDSAmount,
            TDSRemark = model.TDSRemark,
        });

        return _List;
    }
    
    public IActionResult AddTDSDetail(TDSModel model)
    {
        var isDuplicate = false;
        var isExpAdded = isDuplicate;
        var taxFound = isExpAdded;

        var _List = new List<TDSModel>();

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
        else if (model.TxPageName == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }

        var CgstSgst = ITDSModule.SgstCgst(model.TDSAccountCode);

        _MemoryCache.TryGetValue("KeyTDSGrid", out IList<TDSModel> TdsGrid);

        if (HttpContext.Session.GetString(model.TxPageName) != null)
        {
            if (model.TxPageName == "ItemList")
            {
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(model.TxPageName));
            }
            else if (model.TxPageName == "PurchaseOrder")
            {
                _MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
            }
            else if (model.TxPageName == "DirectPurchaseBill")
            {
                _MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
            }
            else if (model.TxPageName == "PurchaseBill")
            {
                _MemoryCache.TryGetValue("PurchaseBill", out MainModel);
            }
            else if (model.TxPageName == "JobWorkIssue")
            {
                _MemoryCache.TryGetValue("JobWorkIssue", out MainModel);
            }
            if (TdsGrid != null && TdsGrid.Count > 0)
            {
                MainModel.TDSDetailGridd = TdsGrid;
                
                isDuplicate = TdsGrid.Any(a => a.TDSAccountName.Equals(model.TDSAccountName));
            }

            if (!isDuplicate)
            {
                if (TdsGrid != null && TdsGrid.Count > 0)
                {
                    _List.AddRange(MainModel.TDSDetailGridd);
                    _List.AddRange(Add2List(model, _List, null));
                }
                else
                {
                    var TDSModelList = Add2List(model, _List, null);
                    _List.AddRange(TDSModelList);
                }
                if (CgstSgst.Rows.Count > 0 && model.TDSAccountName.Contains("CGST"))
                {
                    var pp = Add2List(model, _List, CgstSgst);
                    _List.AddRange(pp);
                }
            }
            else
            {
                return StatusCode(200);
            }

            MainModel.TDSDetailGridd = _List;

            //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            //    SlidingExpiration = TimeSpan.FromMinutes(55),
            //    Size = 1024,
            //};

            //CacheExtensions.Set(_MemoryCache, "KeyTaxGrid", MainModel.TaxDetailGridd, cacheEntryOptions);

            StoreInCache("KeyTDSGrid", MainModel.TDSDetailGridd);
        }
        else
        {
            return StatusCode(501, "Please Add Data In Item Detail Grid");
        }

        return PartialView("_TDSGrid", MainModel.TDSModel);
    }

    public IActionResult ApplyTDS2All(TDSModel TDSModel)
    {
        int ItemCnt = 0;
        decimal BasicTotal = 0;
        decimal ItemAmount = 0;
        decimal TDSAmount = 0;
        decimal ExpTDSAmt = 0;
        decimal TaxOnExp = 0;
        bool isExp = false;
        bool exists = false;
        decimal TotalTDSAmt = 0;

        dynamic MainModel = null;
        dynamic ItemDetailGrid = null;

        var PartCode = 0;
        var PartName = "";
        var ItemCode = 0;
        var ItemText = "";

        var _List = new List<TDSModel>();

        var CgstSgst = ITDSModule.SgstCgst(TDSModel.TDSAccountCode);

        CultureInfo CI = new CultureInfo("en-IN");
        CI.NumberFormat.NumberDecimalDigits = 4;

        if (TDSModel.TxPageName == "ItemList")
        {
            MainModel = new SaleOrderModel();
        }
        else if (TDSModel.TxPageName == "PurchaseOrder")
        {
            MainModel = new PurchaseOrderModel();
        }
        else if (TDSModel.TxPageName == "DirectPurchaseBill")
        {
            MainModel = new DirectPurchaseBillModel();
        }
        else if (TDSModel.TxPageName == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }
        if (HttpContext.Session.GetString(TDSModel.TxPageName) != null)
        {
            if (TDSModel.TxPageName == "ItemList")
            {
                ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TDSModel.TxPageName) ?? string.Empty);
            }
            else if (TDSModel.TxPageName == "PurchaseOrder")
            {
                _MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
                ItemDetailGrid = MainModel.ItemDetailGrid;
            }
            else if (TDSModel.TxPageName == "DirectPurchaseBill")
            {
                _MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
                ItemDetailGrid = MainModel.ItemDetailGrid;
            }
            else if (TDSModel.TxPageName == "JobWorkIssue")
            {
                _MemoryCache.TryGetValue("KeyJobWorkIssue", out MainModel);
                if (MainModel == null)
                    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out MainModel);
                ItemDetailGrid = MainModel;
                var _ItemGrid = new List<JobWorkGridDetail>();
                _ItemGrid = ItemDetailGrid;
                var Amount = 0.0;
                var itemCodeArray = new List<int>();
                _ItemGrid = _ItemGrid
                 .GroupBy(item => item.ItemCode)
                 .Select(group => new JobWorkGridDetail
                 {
                     Amount = group.Sum(item => item.PurchasePrice *item.IssQty),
                     ItemCode = group.Key,
                     PartCode = group.First().PartCode,
                     ItemName = group.First().ItemName
                 })
                 .ToList();

                ItemDetailGrid = _ItemGrid;
            }

            if (ItemDetailGrid != null)
            {
                _MemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid);

                if (TdsGrid != null)
                {
                    _List = TdsGrid;
                    //exists = TaxGrid.Select(s1 => s1.TxPartCode).ToList().Intersect(ItemDetailGrid.Select(s2 => s2.PartCode).ToList()).Any();

                    //foreach (var TdsItem in TdsGrid)
                    //{
                    //    foreach (var item in ItemDetailGrid)
                    //    {
                    //        if (TDSModel.TxPageName == "JobWorkIssue")
                    //        {
                    //            if (TdsItem.TxPartCode == item.ItemCode)
                    //                exists = true;
                    //            break;
                    //        }
                    //        if (TxItem.TxPartCode == item.PartCode)
                    //            exists = true;
                    //        break;
                    //    }
                    //}
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

                    List<JobWorkGridDetail> tdsGrid1 = new List<JobWorkGridDetail>();
                    _MemoryCache.TryGetValue("KeyJobWorkIssue", out tdsGrid1);
                    if (tdsGrid1 == null)
                        _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out tdsGrid1);

                    if (tdsGrid1 == null)
                    {
                        tdsGrid1 = new List<JobWorkGridDetail>();
                    }

                    var checkContains = false;
                    if (TDSModel.TxPageName == "JobWorkIssue")
                        checkContains = partCodeArray.Contains(item.PartCode);
                    else
                        checkContains = partCodeArray.Contains(item.PartText);

                    taxGrid22.Add(item.ItemCode);
                    if (taxGrid22.Contains(item.ItemCode))
                    {
                        ItemAmount += item.Amount;
                    }
                    else
                    {
                        ItemAmount = item.Amount;
                    }

                    //if (checkContains)
                    //{
                    //    //do nothing
                    //}
                    //else
                    //{
                    decimal ItemAmount1 = 0;
                    ItemCnt = ItemCnt + 1;
                        ItemAmount = item.Amount;

                        //if (isExp)
                        //{
                        //    ExpTaxAmt = TaxGrid.Where(m => m.TxType == "EXPENSES" && m.TxAdInTxable == "Y").Sum(x => x.TxAmount);
                        //    //TaxOnExp = ((ItemAmount / BasicTotal) * ExpTaxAmt);
                        //    TaxOnExp = ItemAmount == 0 && BasicTotal == 0 ? 0 : ExpTaxAmt / BasicTotal * ItemAmount;
                        //    ItemAmount = TaxOnExp + ItemAmount;
                        //}

                        TDSAmount = ItemAmount * TDSModel.TDSPercentg / 100;

                        if (TDSModel.TxPageName == "JobWorkIssue")
                        {
                            PartCode = item.ItemCode;
                            PartName = item.PartCode;
                            ItemCode = item.ItemCode;
                            ItemText = item.ItemName;
                        }
                        else
                        {
                            PartCode = item.PartCode;
                            PartName = item.PartText;
                            ItemCode = item.ItemCode;
                            ItemText = item.ItemText;
                        }
                        _List.Add(new TDSModel
                        {
                            TDSSeqNo = _List.Count + 1,
                            TDSTaxType = TDSModel.TDSTaxType,
                            TDSTaxTypeName = TDSModel.TDSTaxTypeName,
                            TDSAccountCode = TDSModel.TDSAccountCode,
                            TDSAccountName = TDSModel.TDSAccountName,
                            TDSPercentg = TDSModel.TDSPercentg,
                            TDSRoundOff = TDSModel.TDSRoundOff,
                            TDSAmount = TDSModel.TDSRoundOff == "Y" ? Math.Floor(TDSAmount) : Math.Round(TDSAmount, 2),
                            TDSRemark = TDSModel.TDSRemark,
                        });

                        if (CgstSgst.Rows.Count > 0 && TDSModel.TDSAccountName.Contains("CGST"))
                        {
                            _List.Add(new TDSModel
                            {
                                TDSSeqNo = _List.Count + 1,
                                TDSTaxType = TDSModel.TDSTaxType,
                                TDSTaxTypeName = TDSModel.TDSTaxTypeName,
                                TDSAccountCode = ToInt32(CgstSgst.Rows[1]["Account_Code"], CI),
                                TDSAccountName = CgstSgst.Rows[0]["Tax_Name"].ToString().Contains("SGST") ? CgstSgst.Rows[0]["Tax_Name"].ToString() : CgstSgst.Rows[1]["Tax_Name"].ToString(),
                                TDSPercentg = TDSModel.TDSPercentg,
                                TDSRoundOff = TDSModel.TDSRoundOff,
                                TDSAmount = TDSModel.TDSRoundOff == "Y" ? Math.Floor(TDSAmount) : Math.Round(TDSAmount, 2),
                                TDSRemark = TDSModel.TDSRemark,
                            });
                        }
                        if (TDSModel.TxPageName == "JobWorkIssue")
                            partCodeArray.Add(item.PartCode);
                        else
                            partCodeArray.Add(item.PartText);
                    
                }
                if (TDSModel.TxPageName == "JobWorkIssue")
                {
                    MainModel = new JobWorkIssueModel();
                }
                MainModel!.TDSDetailGridd = _List;
                TdsGrid = MainModel.TDSDetailGridd;

                if (HttpContext.Session.GetString(TDSModel.TxPageName) != null)
                {
                    if (TDSModel.TxPageName == "ItemList")
                    {
                        MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TDSModel.TxPageName) ?? string.Empty);
                    }
                    if (TDSModel.TxPageName == "PurchaseOrder")
                    {
                        HttpContext.Session.Get(TDSModel.TxPageName);
                        HttpContext.Session.GetString(TDSModel.TxPageName);
                    }
                    if (TDSModel.TxPageName == "DirectPurchaseBill")
                    {
                        HttpContext.Session.Get(TDSModel.TxPageName);
                        HttpContext.Session.GetString(TDSModel.TxPageName);
                    }
                    if (TDSModel.TxPageName == "JobWorkIssue")
                    {
                        HttpContext.Session.Get(TDSModel.TxPageName);
                        HttpContext.Session.GetString(TDSModel.TxPageName);
                    }
                }

                if (TdsGrid != null)
                {
                    TotalTDSAmt = TdsGrid.Sum(x => x.TDSAmount);
                }
                TotalTDSAmt = TotalTDSAmt + MainModel.ItemNetAmount;
                MainModel.TotalTDSAmt = TotalTDSAmt;

                //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                //    SlidingExpiration = TimeSpan.FromMinutes(60),
                //    Size = 1024,
                //};

                //_MemoryCache.Set("KeyTaxGrid", TaxGrid, cacheEntryOptions);

                StoreInCache("KeyTDSGrid", TdsGrid);
            }
        }
        return PartialView("_TDSGrid", MainModel);
    }

    public JsonResult CalculateTDS(string PC, string TP, string SN)
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
                _MemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "DirectPurchaseBill")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
                ListOfItems = MainModel.ItemDetailGrid;
            }
            else if (SN == "PurchaseBill")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("PurchaseBill", out PurchaseBillModel MainModel);
                ListOfItems = MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0 ? MainModel.ItemDetailGrid : MainModel.ItemDetailGridd ;
            }
            else if (SN == "IssueNRGP")
            {
                //MainModel = JsonConvert.DeserializeObject<List<POItemDetail>>(HttpContext.Session.GetString(SN) ?? string.Empty);
                _MemoryCache.TryGetValue("IssueNRGP", out IssueNRGPModel MainModel);
                ListOfItems = MainModel.IssueNRGPDetailGrid;
            }
            else
            {
                var JobGrid = new List<JobWorkGridDetail>();
                _MemoryCache.TryGetValue("KeyJobWorkIssue", out List<JobWorkGridDetail> MainModel);


                if (MainModel == null)
                {
                    _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out List<JobWorkGridDetail> MainModel1);
                    ListOfItems = MainModel1;

                }
                else
                {
                    ListOfItems = MainModel;
                }
            }

            foreach (var item in ListOfItems)
            {
                if (SN == "PurchaseBill")
                {
                    BasicTotal = BasicTotal + item.Amount;
                }
                else
                {
                    //Basic Total Amount
                    BasicTotal = BasicTotal + item.Amount;
                }

                if (SN == "JobWorkIssue")
                {
                    Amt += item.PurchasePrice * item.IssQty;
                }
                else if (SN == "PurchaseBill")
                {
                    Amt += item.Amount;
                }
                else
                {
                    Amt += item.Amount;
                }
            }

            _MemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid);
            
            Amt = Amt * ToDecimal(TP) / 100;
        }

        return Json(new { Amt });
    }

    public IActionResult ClearTDS(string SN)
    {
        dynamic MainModel = null;

        MainModel = new TDSModel() ;
        _MemoryCache.Remove("KeyTDSGrid");
        return PartialView("_TDSGrid", MainModel);
        
    }

    public IActionResult DeleteTDSRow(string SeqNo, string SN)
    {
        decimal TotalTDSAmt = 0;
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
        else if (SN == "JobWorkIssue")
        {
            MainModel = new JobWorkIssueModel();
        }
        //MainModel = SN == "ItemList" ? new SaleOrderModel() : new PurchaseOrderModel();

        if (_MemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid))
        {
            bool canDelete = true;

            MainModel.TDSDetailGridd = TdsGrid;
            int Indx = ToInt32(SeqNo) - 1;


            if (canDelete)
            {
                var Remove = TdsGrid.Where(s => s.TDSSeqNo == ToInt32(SeqNo)).ToList();

                foreach (var item in Remove)
                {
                    MainModel.TDSDetailGridd.Remove(item);
                }

                Indx = 0;
                foreach (TDSModel item in MainModel.TDSDetailGridd)
                {
                    Indx++;
                    item.TDSSeqNo = Indx;
                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                    SlidingExpiration = TimeSpan.FromMinutes(60),
                    Size = 1024,
                };

                if (TdsGrid != null)
                {
                    TotalTDSAmt = TdsGrid.Sum(x => x.TDSAmount);
                }
                TotalTDSAmt = TotalTDSAmt + MainModel.ItemNetAmount;
                MainModel.TotalTDSAmt = TotalTDSAmt;

                _MemoryCache.Set("KeyTDSGrid", TdsGrid, cacheEntryOptions);
            }
            else
            {
                return StatusCode(statusCode: 300);
            }
        }
        if (SN == "PurchaseBill")
        {
            return PartialView("_TDSGrid", MainModel.TDSModel);
        }
        return PartialView("_TDSGrid", MainModel);

    }

    
    public async Task<IActionResult> GetHsnTDSInfo(int AC, string TxPageName, string RF)
    {
        var isSuccess = string.Empty;

        dynamic MainModel = null;

        var TdsGrid = new List<TDSModel>();
        var IssueTaxGrid = new List<IssueNRGPTaxDetail>();

        switch (TxPageName)
        {
            case "ItemList":
                HttpContext.Session.Get(TxPageName);
                MainModel = new SaleOrderModel();
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(TxPageName));
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                MainModel.TDSRoundOff = RF;
                isSuccess = ValidateHsnTax(MainModel);
                if (isSuccess != "SuccessFull")
                {
                    return Content(isSuccess);
                }
                TdsGrid = await GetHSNTaxList(MainModel);
                break;

            case "PurchaseOrder":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("PurchaseOrder", out MainModel);
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TdsGrid = await GetHSNTaxList(MainModel);
                break;
            case "DirectPurchaseBill":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TdsGrid = await GetHSNTaxList(MainModel);
                break;
            //case "IssueNRGP":
            //    HttpContext.Session.Get(TxPageName);
            //    _MemoryCache.TryGetValue("IssueNRGP", out MainModel);
            //    MainModel.AccountCode = AC;
            //    MainModel.TxPageName = TxPageName;
            //    IssueTaxGrid = await GetHSNIssueTaxList(MainModel);
            //    break;
            case "JobWorkIssue":
                HttpContext.Session.Get(TxPageName);
                _MemoryCache.TryGetValue("JobWorkIssue", out MainModel);
                MainModel.AccountCode = AC;
                MainModel.TxPageName = TxPageName;
                TdsGrid = await GetHSNTaxList(MainModel);
                break;

            default:
                Console.WriteLine("No Page Defined.");
                break;
        }
        //if (TxPageName == "IssueNRGP")
        //{
        //    MainModel.IssueNRGPTaxGrid = IssueTaxGrid;
        //    StoreInCache("KeyIssueNRGPTaxGrid", MainModel.IssueNRGPTaxGrid);
        //    return PartialView("_IssueTaxGrid", MainModel);
        //}
        //else
        //{
            MainModel.TDSDetailGridd = TdsGrid;
            StoreInCache("KeyTDSGrid", MainModel.TDSDetailGridd);
        //}


        return PartialView("_TDSGrid", MainModel);
    }

    public async Task<IActionResult> GetTaxByType(string TxType)
    {
        IList<TextValue> Result = await IDataLogic.GetDropDownList("TAXBYTYPE", TxType, "SP_GetDropDownList");
        return Json(Result);
    }

    public IActionResult GetTDSInfo()
    {
        bool isTDS = false;
        bool isExp = false;
        _MemoryCache.TryGetValue("KeyTDSGrid", out IList<TDSModel> TdsGrid);
        if (TdsGrid != null && TdsGrid.Count > 0)
            isTDS = true;

        return Ok(new { isTDS });
    }
    
    public string GetTDSPercentage(string TDSCode)
    {
        string Result = ITDSModule.GetTDSPercentage("TDSTAXPercentage", TDSCode);
        return Result;
    }
    
    public async Task<IActionResult> FillTDSTaxType(string Flag)
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
                    }).Where(a=> a.Text == "TDS").ToList();
            }
        }
        return Json(new { TxType });
    }
    public async Task<IActionResult> GetTDSAccountName(string Flag, int AccountCode, string TDSTaxType)
    {
        DataSet oDataSet = await IDataLogic.GetTDSAccountList(Flag, AccountCode, TDSTaxType);

        List<TextValue> TDSName = new List<TextValue>();
        if (Flag == "TDSTAX")
        {
            if (oDataSet.Tables.Count > 0 && oDataSet != null && oDataSet.Tables[0].Rows.Count > 0)
            {
                TDSName = (
                    from DataRow dr in oDataSet.Tables[0].Rows
                    select new TextValue
                    {
                        Text = dr["Tax_Name"].ToString(),
                        Value = dr["Account_Code"].ToString(),
                    }).ToList();
            }
        }
        return Json(new { TDSName });
    }

    public async Task<IActionResult> GetTaxTypeName(string Flag)
    {
        DataSet oDataSet = await IDataLogic.GetDropDownList(Flag);

        List<TextValue> TxType = new List<TextValue>();
        List<TextValue> TxName = new List<TextValue>();

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

        return Json(new { TxType, TxName });
    }

    private async Task<List<TDSModel>> GetHSNTaxList(dynamic MainModel)
    {
        var HSNTAXParam = new HSNTAX();
        var TdsGrid = new List<TDSModel>();
        var HSNTaxDetail = new HSNTAXInfo();
        _MemoryCache.TryGetValue("KeyTDSGrid", out TdsGrid);


        if (TdsGrid == null)
            TdsGrid = new List<TDSModel>();

        var grid = MainModel;
        if (MainModel.TxPageName == "JobWorkIssue")
        {
            var JobGrid = new List<JobWorkGridDetail>();
            _MemoryCache.TryGetValue("KeyJobWorkIssue", out JobGrid);
            if(JobGrid==null)
                _MemoryCache.TryGetValue("KeyJobWorkIssueEdit", out JobGrid);

            grid = JobGrid;
        }
        else
        {
            grid = MainModel.ItemDetailGrid;
        }

        List<string> partCodeCheck = new List<string>();
        foreach (var item in grid)
        {
            HSNTAXParam.HSNNo = item.HSNNo;
            HSNTAXParam.AC = MainModel.AccountCode;
            MainModel.HSNNo = item.HSNNo;
            string partCode = "";
            if (MainModel.TxPageName == "JobWorkIssue")
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

                var paramResult = await ITDSModule.GetHSNTaxInfo(HSNTAXParam);

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
                        if (MainModel.TxPageName == "JobWorkIssue")
                        {
                            HSNTaxDetail.ItemCode = item.ItemCode;
                            HSNTaxDetail.ItemName = item.ItemName;
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
                        PrepareTDSList(HSNTaxDetail, ref TdsGrid, "CGST", MainModel);
                        PrepareTDSList(HSNTaxDetail, ref TdsGrid, "SGST", MainModel);
                    }
                    else
                    {
                        PrepareTDSList(HSNTaxDetail, ref TdsGrid, "IGST", MainModel);
                    }
                }
            }
            partCodeCheck.Add(partCode);
        }

        return TdsGrid;
    }

    private void PrepareTDSList(HSNTAXInfo HSNTaxDetail, ref List<TDSModel> TdsGrid, string CGSTSGST, dynamic MainModel)
    {
        var TDSValue = CalculateTDS(HSNTaxDetail.ItemCode.ToString(), HSNTaxDetail.TaxPercent.ToString(), MainModel.TxPageName);
        var TDSAmt = JToken.Parse(JsonConvert.SerializeObject(TDSValue.Value));

        if (CGSTSGST == "CGST" || CGSTSGST == "SGST")
        {
            TdsGrid.Add(new TDSModel
            {
                TDSSeqNo = TdsGrid == null ? 1 : TdsGrid.Count + 1,
                TDSTaxType = 25,
                TDSTaxTypeName = HSNTaxDetail.TaxType,
                TDSAccountCode = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTAccountCode : HSNTaxDetail.SGSTAccountCode,
                TDSAccountName = !string.IsNullOrEmpty(CGSTSGST) && CGSTSGST == "CGST" ? HSNTaxDetail.CGSTTaxName : HSNTaxDetail.SGSTTaxName,
                TDSPercentg = HSNTaxDetail.TaxPercent,
                TDSRoundOff = MainModel.TDSRoundOff == "YES" ? "Y" : "N",
                TDSAmount = MainModel.TDSRoundOff == "YES" ? Math.Round((decimal)TDSAmt["Amt"]) : (decimal)TDSAmt["Amt"],
                TDSRemark = "Tax Addedd From HSN # " + MainModel.HSNNo,
            });
        }
        else
        {
            TdsGrid.Add(new TDSModel
            {
                TDSSeqNo = TdsGrid == null ? 1 : TdsGrid.Count + 1,
                TDSTaxType = 25,
                TDSTaxTypeName = HSNTaxDetail.TaxType,
                TDSAccountCode = HSNTaxDetail.CGSTAccountCode,
                TDSAccountName = HSNTaxDetail.CGSTTaxName,
                TDSPercentg = HSNTaxDetail.TaxPercent,
                TDSRoundOff = MainModel.TxRoundOff == "YES" ? "Y" : "N",
                TDSAmount = MainModel.TxRoundOff == "YES" ? Math.Round((decimal)TDSAmt["Amt"]) : (decimal)TDSAmt["Amt"],
                TDSRemark = "Tax Addedd From HSN # " + MainModel.HSNNo,
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
            HSNTAXParam.HSNNo = item.HSNNo;
            HSNTAXParam.AC = MainModel.AccountCode;
            MainModel.HSNNo = item.HSNNo;

            var paramResult = Task.FromResult(ITDSModule.GetHSNTaxInfo(HSNTAXParam)).Result;
            ErrMsg = paramResult.Result.Result?.Rows[0]["Status"].ToString();
            if (ErrMsg != "SuccessFull")
                return ErrMsg;
        }

        return ErrMsg;
    }
}