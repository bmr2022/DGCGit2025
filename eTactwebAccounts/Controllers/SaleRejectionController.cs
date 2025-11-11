using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NuGet.Packaging;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Identity.Client;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.EMMA;

namespace eTactWeb.Controllers
{
    public class SaleRejectionController : Controller
    {
        private readonly ISaleRejection _saleRejection;
        private readonly ILogger<SaleRejectionController> _logger;
        public SaleRejectionController(ISaleRejection saleRejection, ILogger<SaleRejectionController> logger)
        {
            _saleRejection = saleRejection;
            _logger = logger;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> SaleRejection(string mrnNo, int mrnEntryId, int mrnYC)
        {
            SaleRejectionModel model = new SaleRejectionModel();
            ViewData["Title"] = "Sale Rejection";
            TempData.Clear();
            HttpContext.Session.Remove("KeySaleRejectionGrid");
            HttpContext.Session.Remove("SaleRejectionModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyTaxGrid");


            //model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //model.ActualEnteredByName = GetEmpByMachineName();
            //model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.MrnNo = mrnNo;
            model.Mrnyearcode = mrnYC;
           // model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            model = await _saleRejection.FillSaleRejectionGrid(mrnNo, mrnEntryId, mrnYC, model.SaleRejYearCode);

            model.adjustmentModel = new AdjustmentModel();

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
            HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
            HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
            HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
            model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.ActualEnteredByName = GetEmpByMachineName();
            model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
            model.MachineName = GetEmpByMachineName();
            return View(model);
        }
        //public async Task<JsonResult> EditItemRows(int SeqNo)
        //{
        //    var MainModel = new SaleBillModel();
        //    _MemoryCache.TryGetValue("KeySaleRejectionGrid", out List<SaleRejectionDetail> saleRejectionGrid);
        //    var SAGrid = saleRejectionGrid.Where(x => x.SeqNo == SeqNo);
        //    string JsonString = JsonConvert.SerializeObject(SAGrid);
        //    return Json(JsonString);
        //}
     
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _saleRejection.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public string GetEmpByMachineName()
        {
            try
            {
                string empname = string.Empty;
                empname = HttpContext.Session.GetString("EmpName").ToString();
                if (string.IsNullOrEmpty(empname)) { empname = Environment.UserDomainName; }
                return empname;
            }
            catch
            {
                return "";
            }
        }
        [HttpGet]
        public async Task<IActionResult> SaleRejectionEdit(int ID, string Mode, int YearCode)
        {
            SaleRejectionModel model = new SaleRejectionModel();
            ViewData["Title"] = "Sale Rejection";
            TempData.Clear();
            HttpContext.Session.Remove("KeySaleRejectionGrid");
            HttpContext.Session.Remove("SaleRejectionModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyTaxGrid");
            if (model.Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = GetEmpByMachineName();
                model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                model.MachineName = GetEmpByMachineName();
            }
            else
            {
                model.ActualEnteredByName = GetEmpByMachineName();
                model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.LastUpdatedByName = GetEmpByMachineName();
                model.LastUpdationDate = HttpContext.Session.GetString("LastUpdatedDate");
                model.UpdatedOn = ParseSafeDate(HttpContext.Session.GetString("LastUpdatedDate"));
                model.MachineName = GetEmpByMachineName();
            }
            //model.adjustmentModel = new AdjustmentModel();

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _saleRejection.GetViewByID(ID, Mode, YearCode);
                model.Mode = Mode;
                model.ID = ID;
            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _saleRejection.GetViewByID(ID, Mode, YearCode);
                model.Mode = Mode;
                model.ID = ID;
              
                model.FinFromDate = model.FinFromDate ?? HttpContext.Session.GetString("FromDate");
                model.FinToDate = model.FinToDate ?? HttpContext.Session.GetString("ToDate");
                model.SaleRejYearCode = model.SaleRejYearCode != null && model.SaleRejYearCode > 0 ? model.SaleRejYearCode : Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = model.CC ?? HttpContext.Session.GetString("Branch");
            }
            else
            {
                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                model.FinToDate = HttpContext.Session.GetString("ToDate");
                model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = HttpContext.Session.GetString("Branch");
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");

            HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
          
            string serializedKeyAdjGrid = JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel);
            string serializedKeyTaxGrid = JsonConvert.SerializeObject(model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd);
            HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
           // HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
            HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
            HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
            string serializedKeyDbCrGrid = JsonConvert.SerializeObject(model.DbCrGrid == null ? new List<DbCrModel>() : model.DbCrGrid);
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                HttpContext.Session.SetString("KeyDrCrGrid", serializedKeyDbCrGrid);
            }
            HttpContext.Session.SetString("KeyTaxGrid", serializedKeyTaxGrid);

            //HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
            HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
            //HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
            //HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
            return View("SaleRejection", model);
        }
        //public IActionResult DeleteItemRow(int SeqNo)
        //{
        //    try
        //    {
        //        var MainModel = new SaleRejectionModel();

        //        // ✅ Get existing list from session
        //        string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
        //        List<SaleRejectionDetail> SaleRejectionGrid = new List<SaleRejectionDetail>();

        //        if (!string.IsNullOrEmpty(modelJson))
        //        {
        //            SaleRejectionGrid = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
        //        }
        //        if (SaleRejectionGrid != null && SaleRejectionGrid.Count > 0)
        //        {
        //            int index = SeqNo - 1;
        //            if (index >= 0 && index < SaleRejectionGrid.Count)
        //            {
        //                SaleRejectionGrid.RemoveAt(index);
        //            }
        //            int newSeq = 1;
        //            foreach (var item in SaleRejectionGrid)
        //            {
        //                item.SeqNo = newSeq;
        //                newSeq++;
        //            }
        //            MainModel.SaleRejectionDetails = SaleRejectionGrid;
        //            MainModel.ItemDetailGrid = SaleRejectionGrid;

        //            if (SaleRejectionGrid.Count == 0)
        //            {

        //                HttpContext.Session.Remove("KeySaleRejectionGrid");
        //            }
        //            else
        //            {
        //                string serializedMainModel = JsonConvert.SerializeObject(SaleRejectionGrid);
        //                HttpContext.Session.SetString("KeySaleRejectionGrid", serializedMainModel);

        //                //HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(SaleRejectionGrid));
        //                //string modelJson1 = HttpContext.Session.GetString("KeySaleRejectionGrid",);
        //                //HttpContext.Session.SetString("KeySalesRejectionGrid", JsonConvert.SerializeObject(SaleRejectionGrid));
        //            }
        //        }
        //        return PartialView("_AddSaleRejectionGrid", MainModel);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = "error", message = ex.Message });
        //    }
        //}
        //public IActionResult EditItemRow(int SeqNo)
        //{
        //    string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
        //    List<SaleRejectionDetail> MaterialGrid = new List<SaleRejectionDetail>();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        MaterialGrid = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
        //    }

        //    var SSGrid = MaterialGrid.Where(x => x.SeqNo == SeqNo);
        //    string JsonString = JsonConvert.SerializeObject(SSGrid);
        //    return Json(JsonString);
        //}
        [HttpPost]
        public async Task<IActionResult> UpdateRejectionItem(List<SaleRejectionDetail> model)
        {
            try
            {
                // 🔹 Get the existing grid data from session
                string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                var SaleBillDetail = new List<SaleRejectionDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    SaleBillDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
                }

                // 🔹 Get the main sale rejection model from session
                string SaleBillModelJson = HttpContext.Session.GetString("SaleRejectionModel");
                var saleBillModel = new SaleRejectionModel();
                if (!string.IsNullOrEmpty(SaleBillModelJson))
                {
                    saleBillModel = JsonConvert.DeserializeObject<SaleRejectionModel>(SaleBillModelJson);
                }

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        var existing = SaleBillDetail.FirstOrDefault(x =>
                            x.ItemCode == item.ItemCode &&
                            x.AgainstBillEntryId == item.AgainstBillEntryId &&
                            x.AgainstBillYearCode == item.AgainstBillYearCode);

                        if (existing != null)
                        {
                            // ✅ Correctly update properties
                            existing.RejRate = item.RejRate;
                            existing.Amount = item.Amount;
                            existing.ItemNetAmount = item.Amount;
                        }
                    }

                    // ✅ Sort & update the same model object
                    SaleBillDetail = SaleBillDetail.OrderBy(x => x.SeqNo).ToList();
                    saleBillModel.SaleRejectionDetails = SaleBillDetail;
                    saleBillModel.ItemDetailGrid = SaleBillDetail;

                    // ✅ Overwrite session ONCE cleanly
                    HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(SaleBillDetail));
                    HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(saleBillModel));

                    await HttpContext.Session.CommitAsync();
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Sale Rejection List cannot be empty...!");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public IActionResult UpdateRejectionItem(List<SaleRejectionDetail> model)
        //{
        //    try
        //    {
        //        string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
        //        if (string.IsNullOrEmpty(modelJson))
        //            return Json(new { success = false, message = "Session expired or empty." });

        //        var saleRejectionList = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
        //        var MainModel = new SaleRejectionModel();
        //        foreach (var item in model)
        //        {
        //            var existing = saleRejectionList.FirstOrDefault(x =>
        //                x.ItemCode == item.ItemCode &&
        //                x.AgainstBillEntryId == item.AgainstBillEntryId &&
        //                x.AgainstBillYearCode == item.AgainstBillYearCode);

        //            if (existing != null)
        //            {
        //                existing.RejRate = item.RejRate;
        //                existing.Amount = item.Amount;
        //                existing.ItemNetAmount = item.Amount;
        //            }
        //        }
        //        HttpContext.Session.Remove("KeySaleRejectionGrid");
        //        HttpContext.Session.Remove("SaleRejectionModel");
        //        HttpContext.Session.SetString("KeySaleRejectionGrid",
        //            JsonConvert.SerializeObject(saleRejectionList));
        //        MainModel = BindItem4Grid(MainModel);
        //        MainModel.SaleRejectionDetails = saleRejectionList;
        //        MainModel.ItemDetailGrid = saleRejectionList;
        //        HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(MainModel));
        //        string modelJson2 = HttpContext.Session.GetString("SaleRejectionModel");
        //        string modelJson1 = HttpContext.Session.GetString("KeySaleRejectionGrid");
        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //public IActionResult AddSaleRejectionDetail(List<SaleRejectionDetail> model)
        //{
        //    try
        //    {
        //        string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
        //        List<SaleRejectionDetail> SaleRejectionDetail = new List<SaleRejectionDetail>();
        //        if (!string.IsNullOrEmpty(modelJson))
        //        {
        //            SaleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
        //        }
        //        string saleRejectionModelJson = HttpContext.Session.GetString("SaleRejectionModel");
        //        SaleRejectionModel saleRejectionnModel = new SaleRejectionModel();
        //        if (!string.IsNullOrEmpty(saleRejectionModelJson))
        //        {
        //            saleRejectionnModel = JsonConvert.DeserializeObject<SaleRejectionModel>(saleRejectionModelJson);
        //        }

        //        var MainModel = new SaleRejectionModel();
        //        var saleRejectionDetail = new List<SaleRejectionDetail>();
        //        var rangeSaleRejectionGrid = new List<SaleRejectionDetail>();

        //        if (model != null)
        //        {
        //            foreach (var item in model) {
        //                if (SaleRejectionDetail == null)
        //                {
        //                    item.SeqNo = 1; 
        //                    //model.SeqNo = 1;
        //                    saleRejectionDetail.Add(item);
        //                }
        //                else
        //                {
        //                    if (SaleRejectionDetail.Any(x => x.ItemCode == item.ItemCode))
        //                    {
        //                        return Json("Duplicate");
        //                    }
        //                    int nextSeqNo = SaleRejectionDetail.Count > 0 ? SaleRejectionDetail.Max(x => x.SeqNo) + 1 : 1;
        //                    item.SeqNo = nextSeqNo;


        //                    //model.SeqNo = SaleRejectionDetail.Count + 1;
        //                    saleRejectionDetail = SaleRejectionDetail.Where(x => x != null).ToList();
        //                    rangeSaleRejectionGrid.AddRange(saleRejectionDetail);
        //                    saleRejectionDetail.Add(item);
        //                }
        //            }
        //                //MainModel = BindItem4Grid(model);
        //                MainModel.SaleRejectionDetails = saleRejectionDetail;
        //                MainModel.ItemDetailGrid = saleRejectionDetail;

        //            HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(MainModel.SaleRejectionDetails));

        //            MainModel = BindItem4Grid(MainModel);
        //            MainModel.SaleRejectionDetails = saleRejectionDetail;
        //            MainModel.ItemDetailGrid = saleRejectionDetail;
        //            HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(MainModel));
        //        }

        //        return PartialView("_AddSaleRejectionGrid", MainModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        [HttpGet]
        [Route("{controller}/Dashboard")]
        //public async Task<IActionResult> SRDashboard(string summaryDetail, string custInvoiceNo, string custName, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string docName, string voucherNo, string fromdate, string toDate)
        public async Task<IActionResult> SRDashboard(string fromDate,string toDate,string custInvoiceNo,string accountName,string mrnNo,string gateNo,string partCode,string itemName,string againstBillNo,string voucherNo,string summaryDetail,string searchBox,string Flag="True")
        {
            try
            {
                HttpContext.Session.Remove("KeySaleRejectionGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                var model = new SaleRejectionDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                DateTime ToDate = DateTime.Today;
                model.FinToDate = ToDate.ToString();

                model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.SummaryDetail = "Summary";
                var Result = await _saleRejection.GetDashboardData(model.SummaryDetail, custInvoiceNo, accountName, mrnNo, gateNo, partCode, itemName, againstBillNo, accountName/*docName*/, voucherNo, ParseFormattedDate(model.FinFromDate.Split(" ")[0]), ParseFormattedDate(model.FinToDate.Split(" ")[0])).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataTable resultDT = Result.Result;
                    if (resultDT != null)
                    {
                        var DT = resultDT.DefaultView.ToTable(true,
                                      "Account_Name", "GSTNO", "DomesticExportNEPZ", "CustInvoiceNo", "CustInvoiceDate", "SalerejCreditNoteVoucherNo", "VoucherNo"
                                    , "SaleRejEntryDate", "GateNo", "GateDate", "MrnNo", "MRNDate", "BillAmt", "InvNetAmt", "SubVoucherName", "CustInvoiceTime"
                                    , "PaymentTerm", "Transporter", "Vehicleno", "ActualEnteredBy", "AccountCode"
                                    , "RoundOffAmt", "RoundoffType", "Taxableamt", "ToatlDiscountPercent", "TotalDiscountAmount", "MachineName", "SalerejRemark", "CC", "Uid"
                                     , "ActualEnteredByName", "ActualEntryDate", "LastUpdatedBy", "LastUpdatedByName"
                                    , "LastUpdationDate", "BalanceSheetClosed", "SaleRejEntryId", "SaleRejYearCode", "Gateyearcode", "Mrnyearcode", "AgainstVoucherNo"
                            );
                        model.saleRejectionDashboard = CommonFunc.DataTableToList<SaleRejectionDashboard>(DT, "SaleRejectionSummTable");
                    }
                    //fromDate = fromDate,toDate = toDate,custInvoiceNo = custInvoiceNo,accountName = accountName
                    //,mrnNo = mrnNo,gateNo = gateNo,partCode = partCode,itemName = itemName,againstBillNo = againstBillNo,voucherNo = voucherNo

                    //model.FinFromDate = fromDate;
                    //model.FinToDate = toDate;
                    model.PartCode = partCode;
                    model.ItemName = itemName;
                    model.MrnNo = mrnNo;
                    model.GateNo = gateNo;
                    model.CustInvoiceNo = custInvoiceNo;
                    model.Account_Name = accountName;
                    model.AgainstBillNo = againstBillNo;
                    model.VoucherNo = voucherNo;
                    model.SummaryDetail = summaryDetail;
                    model.SearchBox = searchBox;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> GetSearchData(string summaryDetail, string custInvoiceNo, string account_Name, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string documentName, string voucherNo, string fromdate, string toDate)
        {
            try
            {
                var model = new SaleRejectionDashboard();
                model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                var Result = await _saleRejection.GetDashboardData(summaryDetail, custInvoiceNo, account_Name, mrnNo, gateNo, partCode, itemName, againstBillNo, documentName, voucherNo, ParseFormattedDate((fromdate).Split(" ")[0]), ParseFormattedDate(toDate.Split(" ")[0])).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataTable resultDT = Result.Result;
                    if (resultDT != null)
                    {
                        var DT = resultDT.DefaultView.ToTable(true, "Account_Name", "GSTNO", "DomesticExportNEPZ", "CustInvoiceNo", "CustInvoiceDate", "SalerejCreditNoteVoucherNo", "VoucherNo"
                                    , "SaleRejEntryDate", "GateNo", "GateDate", "MrnNo", "MRNDate", "BillAmt", "InvNetAmt", "SubVoucherName", "CustInvoiceTime"
                                    , "PaymentTerm", "Transporter", "Vehicleno", "ActualEnteredBy", "AccountCode"
                                    , "RoundOffAmt", "RoundoffType", "Taxableamt", "ToatlDiscountPercent", "TotalDiscountAmount", "MachineName", "SalerejRemark", "CC", "Uid"
                                     , "ActualEnteredByName", "ActualEntryDate", "LastUpdatedBy", "LastUpdatedByName"
                                    , "LastUpdationDate", "BalanceSheetClosed", "SaleRejEntryId", "SaleRejYearCode", "Gateyearcode", "Mrnyearcode", "AgainstVoucherNo"
                                );
                        model.saleRejectionDashboard = CommonFunc.DataTableToList<SaleRejectionDashboard>(DT, "SaleRejectionSummTable");
                        if (summaryDetail == "Summary")
                        {
                            model.saleRejectionDashboard = model.saleRejectionDashboard
                                .GroupBy(psd => psd.SaleRejEntryId)
                                .Select(group => group.First())
                                .ToList();
                        }

                    }
                }
                model.SummaryDetail = summaryDetail;
                return PartialView("_SRDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private SaleRejectionModel BindItem4Grid(SaleRejectionModel model)
        {
            var _List = new List<DPBItemDetail>();

            string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
            SaleRejectionModel MainModel = new SaleRejectionModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
            }

            _List.Add(
                new DPBItemDetail
                {
                    SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                    docTypeId = 1, //model.docTypeId,
                    DocTypeText = string.Empty, //model.DocTypeText,
                    BillQty = Convert.ToDecimal(model.RejQty),

                    Amount = model.Amount,
                    Description = string.Empty, //model.Description,
                    DiscPer = Convert.ToDecimal(model.DiscountPer),
                    DiscRs = model.DiscountAmt,

                    HSNNo = model.HSNNo,
                    ItemCode = model.ItemCode,
                    ItemText = model.ItemName,

                    //OtherRateCurr = Convert.ToDecimal(model.Rate),
                    PartCode = model.ItemCode,
                    PartText = model.PartCode,

                    DPBQty = Convert.ToDecimal(model.RejQty),
                    //Process = model.ProcessId,
                    //ProcessName = model.ProcessName,
                    //CostCenter = model.CostCenterId,
                    //CostCenterName = model.CostCenterName,
                    Rate = Convert.ToDecimal(model.Rate),

                    PONo = string.Empty,//model.pono,
                    POYear = 0,//model.POYear,
                    PODate = string.Empty,//model.PODate,
                    //ScheduleNo = model.schNo,
                    //ScheduleYear = model.SaleSchYearCode,
                    //ScheduleDate = model.Schdate,

                    Unit = model.Unit,
                });

            if (MainModel.DPBItemDetails == null)
                MainModel.DPBItemDetails = _List;
            else
                MainModel.DPBItemDetails.AddRange(_List);

            MainModel.ItemNetAmount = decimal.Parse(MainModel.DPBItemDetails.Sum(x => x.Amount).ToString("#.#0"));

            return MainModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> SaleRejection(SaleRejectionModel model)
        {
            try
            {
                var SBGrid = new DataTable();
                DataTable TaxDetailDT = null;
                DataTable AdjDetailDT = null;
                DataTable DrCrDetailDT = null;
                string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
                SaleRejectionModel MainModel = new SaleRejectionModel();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
                }
                string saleRejectionModelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
                List<SaleRejectionDetail> saleRejectionDetail = new List<SaleRejectionDetail>();
                if (!string.IsNullOrEmpty(saleRejectionModelJson))
                {
                    saleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(saleRejectionModelJson);
                }
                string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
                List<TaxModel> TaxGrid = new List<TaxModel>();
                if (!string.IsNullOrEmpty(taxGridJson))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);
                }
                string drCrGridJson = HttpContext.Session.GetString("KeyDrCrGrid");
                List<DbCrModel> DrCrGrid = new List<DbCrModel>();
                if (!string.IsNullOrEmpty(drCrGridJson))
                {
                    DrCrGrid = JsonConvert.DeserializeObject<List<DbCrModel>>(drCrGridJson);
                }
              
                if (saleRejectionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("SaleRejectionDetail", "Sale Rejection Grid Should Have Atleast 1 Item...!");
                    return View("StockADjustment", model);
                }
                else if (saleRejectionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
                    return View("StockADjustment", model);
                }
                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                        SBGrid = GetDetailTable(saleRejectionDetail);
                    }
                    else
                    {
                        model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        SBGrid = GetDetailTable(saleRejectionDetail);
                    }

                    if (TaxGrid != null && TaxGrid.Count > 0)
                    {
                        TaxDetailDT = GetTaxDetailTable(TaxGrid);
                    }
                    if (DrCrGrid != null && DrCrGrid.Count > 0)
                    {
                        DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
                    }
                    string serializedGrid = HttpContext.Session.GetString("KeyAdjGrid");
                    var adjustmentModel = new AdjustmentModel();
                    if (!string.IsNullOrEmpty(serializedGrid))
                    {
                        adjustmentModel = JsonConvert.DeserializeObject<AdjustmentModel>(serializedGrid);
                        // Use adjustmentModel as needed
                    }

                    if (adjustmentModel.AdjAdjustmentDetailGrid != null && adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                    {
                        AdjDetailDT = CommonController.GetAdjDetailTable(adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.SaleRejEntryId, model.SaleRejYearCode, model.AccountCode);
                    }
               
                    //if (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                    //{
                    //    AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.SaleRejEntryId, model.SaleRejYearCode, model.AccountCode);
                    //}

                    var Result = await _saleRejection.SaveSaleRejection(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var model1 = new SaleRejectionModel();
                            model1.adjustmentModel = model1.adjustmentModel ?? new AdjustmentModel();

                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            var yearCodeStr = HttpContext.Session.GetString("YearCode");
                            model1.SaleRejYearCode = !string.IsNullOrEmpty(yearCodeStr) ? Convert.ToInt32(yearCodeStr) : 0;
                            model1.CC = HttpContext.Session.GetString("Branch");
                            var uidStr = HttpContext.Session.GetString("UID");
                            model1.CreatedBy = !string.IsNullOrEmpty(uidStr) ? Convert.ToInt32(uidStr) : 0;
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeySaleRejectionGrid");
                            HttpContext.Session.Remove("SaleRejectionModel");
                            return RedirectToAction("PendingSaleRejection", "PendingSaleRejection", new { id = 0 });
                        }

                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new SaleBillModel();
                            model1.adjustmentModel = new AdjustmentModel();
                            model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeySaleRejectionGrid");
                            HttpContext.Session.Remove("SaleRejectionModel");
                            return RedirectToAction("PendingSaleRejection", "PendingSaleRejection", new { id = 0 });
                        }

                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

                                return View(model);
                            }

                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            // return View("Error", Result);
                            return View(model);
                        }
                        HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
                        //}
                        return View(model);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                //  _logger.LogException<SaleRejectionController>.WriteException(_logger, ex);

                var _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", _ResponseResult);
                //return View(model);
            }
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int accountCode, int createdBy, string machineName, string cc,string fromDate,string toDate,string custInvoiceNo,string accountName,string mrnNo,string gateNo,string partCode,string itemName,string againstBillNo,string voucherNo)
        {
            var Result = await _saleRejection.DeleteByID(ID, YearCode, accountCode, createdBy, machineName, cc).ConfigureAwait(false);

            if (Result.StatusText == "Success" || Result.StatusText == "deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
                TempData["DeleteMessage"] = Result.StatusText;

            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";

            }
            //if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone || Result.StatusText == "Success")
            //{
            //    ViewBag.isSuccess = true;
            //    TempData["410"] = "410";
            //}
            //else
            //{
            //    ViewBag.isSuccess = false;
            //    TempData["500"] = "500";
            //}
            return RedirectToAction("SRDashboard",new {fromDate = fromDate,toDate = toDate,custInvoiceNo = custInvoiceNo,accountName = accountName,mrnNo = mrnNo,gateNo = gateNo,partCode = partCode,itemName = itemName,againstBillNo = againstBillNo,voucherNo = voucherNo});
        }
        private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("TxSeqNo", typeof(int));
            Table.Columns.Add("TxType", typeof(string));
            Table.Columns.Add("TxItemCode", typeof(int));
            Table.Columns.Add("TxTaxType", typeof(int));
            Table.Columns.Add("TxAccountCode", typeof(int));
            Table.Columns.Add("TxPercentg", typeof(float));
            Table.Columns.Add("TxAdInTxable", typeof(string));
            Table.Columns.Add("TxRoundOff", typeof(string));
            Table.Columns.Add("TxAmount", typeof(float));
            Table.Columns.Add("TxRefundable", typeof(string));
            Table.Columns.Add("TxOnExp", typeof(float));
            Table.Columns.Add("TxRemark", typeof(string));

            foreach (TaxModel Item in TaxDetailList)
            {
                Table.Rows.Add(
                    new object[]
                    {
                    Item.TxSeqNo,
                    Item.TxType,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    Item.TxAdInTxable,
                    Item.TxRoundOff,
                    Item.TxAmount,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark,
                    });
            }

            return Table;
        }
        private static DataTable GetDetailTable(IList<SaleRejectionDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("SaleRejEntryId", typeof(int));
            DTSSGrid.Columns.Add("SaleRejYearCode", typeof(int));
            DTSSGrid.Columns.Add("AccountCode", typeof(int));
            DTSSGrid.Columns.Add("AgainstBillTypeJWSALE", typeof(string));
            DTSSGrid.Columns.Add("AgainstBillNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstBillYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgainstBillEntryId", typeof(int));
            DTSSGrid.Columns.Add("AgainstOpenBillEntryId", typeof(int));
            DTSSGrid.Columns.Add("AgainstOpenOrBill", typeof(string));
            DTSSGrid.Columns.Add("AgainstOpenBillYearCode", typeof(int));
            DTSSGrid.Columns.Add("DocTypeAccountCode", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("HSNNo", typeof(int));
            DTSSGrid.Columns.Add("NoOfCase", typeof(float));
            DTSSGrid.Columns.Add("SaleBillQty", typeof(float));
            DTSSGrid.Columns.Add("RejQty", typeof(float));
            DTSSGrid.Columns.Add("MRNRecQty", typeof(float));
            DTSSGrid.Columns.Add("RejRate", typeof(float));
            DTSSGrid.Columns.Add("SaleBillRate", typeof(float));
            DTSSGrid.Columns.Add("DiscountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("SONO", typeof(string));
            DTSSGrid.Columns.Add("SOyearcode", typeof(int));
            DTSSGrid.Columns.Add("SODate", typeof(string));
            DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
            DTSSGrid.Columns.Add("SOAmmNo", typeof(string));
            DTSSGrid.Columns.Add("Itemsize", typeof(string));
            DTSSGrid.Columns.Add("RecStoreId", typeof(int));
            DTSSGrid.Columns.Add("OtherDetail", typeof(string));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("RejectionReason", typeof(string));
            DTSSGrid.Columns.Add("SaleorderRemark", typeof(string));
            DTSSGrid.Columns.Add("SaleBillRemark", typeof(string));
            DTSSGrid.Columns.Add("ItemCGSTAmt", typeof(float));
            DTSSGrid.Columns.Add("ItemSGSTAmt", typeof(float));
            DTSSGrid.Columns.Add("ItemIGSTAmt", typeof(float));
            DTSSGrid.Columns.Add("ItemExpense", typeof(float));

            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                    1,
                    2024,
                    1,//Item.AccountCode,
                    Item.AgainstBillTypeJWSALE ?? string.Empty,
                    Item.AgainstBillNo ?? string.Empty,
                    Item.AgainstBillYearCode,
                    Item.AgainstBillEntryId,
                    1,//Item.AgainstOpenEntryId,
                    Item.AgainstOpnOrBill ?? string.Empty,
                    Item.AgainstBillYearCode, //openBillYearCode
                    0,
                    Item.ItemCode,
                    Item.Unit ?? string.Empty,
                    Item.HSNNo,
                    Item.NoOfCase,
                    Item.SaleBillQty,
                    Item.RejQty,
                    Item.RecQty, //MRNRecQty
                    Item.RejRate, //RejRate
                    Item.Rate, //SaleBillRate
                    Item.DiscountPer,
                    Item.DiscountAmt,
                    Item.SONO ?? string.Empty,
                    Item.SOyearcode,
                    Item.SODate == null ? string.Empty : ParseFormattedDate(Item.SODate.Split(" ")[0]) ,
                    Item.CustOrderNo ?? string.Empty,
                    Item.SOAmmNo ?? string.Empty,
                    Item.Itemsize ?? string.Empty,
                    Item.RecStoreId,
                    Item.OtherDetail ?? string.Empty,
                    Item.Amount,
                    Item.RejectionReason ?? string.Empty,
                    Item.SaleorderRemark ?? string.Empty,
                    Item.SaleBillremark ?? string.Empty,
                    Item.Amount, // ItemCGSTAmt
                    Item.Amount, //ItemSGSTAmt
                    Item.Amount, //ItemIGSTAmt
                    Item.Amount//ItemExpense
                  });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _saleRejection.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC, int yearCode)
        {
            var JSON = await _saleRejection.FillSaleRejectionGrid(mrnNo, mrnEntryId, mrnYC, yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerName(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillCustomerName(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillItemName(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillPartCode(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillMRNNo(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillMRNNo(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillGateNo(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillGateNo(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoiceNo(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillInvoiceNo(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNo(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillVoucherNo(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillDocument(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstSaleBillNo(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate(fromDate);
            toDate = ParseFormattedDate(toDate);
            var JSON = await _saleRejection.FillAgainstSaleBillNo(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
