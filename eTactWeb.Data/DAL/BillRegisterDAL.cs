using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class BillRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public BillRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseResult> FillVendoreList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillVendorList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDocList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDocumentList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPONOList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPONOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSchNOList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSchNOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillHsnNOList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillHSNNOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillGstNOList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGSTList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInvoiceNOList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPurchaseInvoiceList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
         public async Task<ResponseResult> FillItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemNamePartcodeList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
         public async Task<ResponseResult> FillPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemNamePartcodeList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchaseBillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<BillRegisterModel> GetRegisterData(string ReportType, string FromDate, string ToDate, string DocumentName, string PONO, string SchNo, string HsnNo, string GstNo, string InvoiceNo, string PurchaseBill, string PurchaseRejection, string DebitNote, string CreditNote, string SaleRejection, string PartCode, string ItemName,string VendorName,int  ForFinYear)
        {
            DataSet? oDataSet = new DataSet();
            var model = new BillRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportPurchaseBillRegister", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@VendorName",
                    (VendorName == "0" || string.IsNullOrWhiteSpace(VendorName)) ? "" : VendorName);

                    oCmd.Parameters.AddWithValue("@Docname",
                        (DocumentName == "0" || string.IsNullOrWhiteSpace(DocumentName)) ? "" : DocumentName);

                    oCmd.Parameters.AddWithValue("@PartCode",
                        (PartCode == "0" || string.IsNullOrWhiteSpace(PartCode)) ? "" : PartCode);

                    oCmd.Parameters.AddWithValue("@ItemName",
                        (ItemName == "0" || string.IsNullOrWhiteSpace(ItemName)) ? "" : ItemName);

                    oCmd.Parameters.AddWithValue("@SONo",
                        (PONO == "0" || string.IsNullOrWhiteSpace(PONO)) ? "" : PONO);

                    oCmd.Parameters.AddWithValue("@InvNo",
                        (InvoiceNo == "0" || string.IsNullOrWhiteSpace(InvoiceNo)) ? "" : InvoiceNo);

                    oCmd.Parameters.AddWithValue("@GSTNo",
                        (GstNo == "0" || string.IsNullOrWhiteSpace(GstNo)) ? "" : GstNo);

                    oCmd.Parameters.AddWithValue("@SchNo",
                        (SchNo == "0" || string.IsNullOrWhiteSpace(SchNo)) ? "" : SchNo);

                    oCmd.Parameters.AddWithValue("@HSNNO",
                        (HsnNo == "0" || string.IsNullOrWhiteSpace(HsnNo)) ? "" : HsnNo);

                    oCmd.Parameters.AddWithValue("@yearCode",  ForFinYear);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "PurchaseSummary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      Description = Convert.ToString(dr["Description"].ToString()),
                                                      ForTheDuration = Convert.ToString(dr["forTheDuration"].ToString()),
                                                      ForFinYear = Convert.ToString(dr["ForFinYear"].ToString())
                                                  }).ToList();
                    }
                }
                else if (ReportType == "PurchaseBillDetail")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new BillRegisterModel
                                                        {
                                                            VendorName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                            GstNo = dr["GSTNO"] != DBNull.Value ? Convert.ToString(dr["GSTNO"]) : string.Empty,
                                                            VendorAddress = dr["VendorAddress"] != DBNull.Value ? Convert.ToString(dr["VendorAddress"]) : string.Empty,
                                                            State = dr["State"] != DBNull.Value ? Convert.ToString(dr["State"]) : string.Empty,
                                                            StateCode = dr["State"] != DBNull.Value ? Convert.ToString(dr["State"]) : string.Empty,
                                                            InvoiceNo = dr["InvoiceNo"] != DBNull.Value ? Convert.ToString(dr["InvoiceNo"]) : string.Empty,
                                                            InvDate = dr["InvDate"] != DBNull.Value ? Convert.ToDateTime(dr["InvDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            VoucherNo = dr["VoucherNo"] != DBNull.Value ? Convert.ToString(dr["VoucherNo"]) : string.Empty,
                                                            VoucherDate = dr["VoucherDate"] != DBNull.Value ? Convert.ToString(dr["VoucherDate"]) : string.Empty,
                                                            MRNNo = dr["MRNNo"] != DBNull.Value ? Convert.ToString(dr["MRNNo"]) : string.Empty,
                                                            MRNDate = dr["MRNDate"] != DBNull.Value ? Convert.ToDateTime(dr["MRNDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            GateNo = dr["GateNo"] != DBNull.Value ? Convert.ToString(dr["GateNo"]) : string.Empty,
                                                            GateDate = dr["GateDate"] != DBNull.Value ? Convert.ToDateTime(dr["GateDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                                            ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                            HsnNo = dr["HSNNO"] != DBNull.Value ? Convert.ToString(dr["HSNNO"]) : string.Empty,
                                                            BillQty = dr["BillQty"] != DBNull.Value ? Convert.ToDecimal(dr["BillQty"]) : 0,
                                                            RecQty = dr["RecQty"] != DBNull.Value ? Convert.ToDecimal(dr["RecQty"]) : 0,
                                                            RejectedQty = dr["RejectedQty"] != DBNull.Value ? Convert.ToDecimal(dr["RejectedQty"]) : 0,
                                                            Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                            BillRate = dr["BillRate"] != DBNull.Value ? Convert.ToDecimal(dr["BillRate"]) : 0,
                                                            PoRate = dr["PoRate"] != DBNull.Value ? Convert.ToDecimal(dr["PoRate"]) : 0,
                                                            DiscountPer = dr["DiscountPer"] != DBNull.Value ? Convert.ToDecimal(dr["DiscountPer"]) : 0,
                                                            DisAmt = dr["DisAmt"] != DBNull.Value ? Convert.ToDecimal(dr["DisAmt"]) : 0,
                                                            ItemAmount = dr["ItemAmount"] != DBNull.Value ? Convert.ToDecimal(dr["ItemAmount"]) : 0,
                                                            CGSTPer = dr["CGSTPer"] != DBNull.Value ? Convert.ToDecimal(dr["CGSTPer"]) : 0,
                                                            CGSTAmt = dr["CGSTAmt"] != DBNull.Value ? Convert.ToDecimal(dr["CGSTAmt"]) : 0,
                                                            SGSTPer = dr["SGSTPer"] != DBNull.Value ? Convert.ToDecimal(dr["SGSTPer"]) : 0,
                                                            SGSTAmt = dr["SGSTAmt"] != DBNull.Value ? Convert.ToDecimal(dr["SGSTAmt"]) : 0,
                                                            IGSTPer = dr["IGSTPer"] != DBNull.Value ? Convert.ToDecimal(dr["IGSTPer"]) : 0,
                                                            IGSTAmt = dr["IGSTAmt"] != DBNull.Value ? Convert.ToDecimal(dr["IGSTAmt"]) : 0,
                                                            ExpenseAmt = dr["ExpenseAmt"] != DBNull.Value ? Convert.ToDecimal(dr["ExpenseAmt"]) : 0,
                                                            TotalBillAmt = dr["TotalBillAmt"] != DBNull.Value ? Convert.ToDecimal(dr["TotalBillAmt"]) : 0,
                                                            TotaltaxableAmt = dr["TotaltaxableAmt"] != DBNull.Value ? Convert.ToDecimal(dr["TotaltaxableAmt"]) : 0,
                                                            GSTAmount = dr["GSTAmount"] != DBNull.Value ? Convert.ToDecimal(dr["GSTAmount"]) : 0,
                                                            Currency = dr["Currency"] != DBNull.Value ? Convert.ToString(dr["Currency"]) : string.Empty,
                                                            InvAmt = dr["InvAmt"] != DBNull.Value ? Convert.ToDecimal(dr["InvAmt"]) : 0,
                                                            MIRNO = dr["MIRNO"] != DBNull.Value ? Convert.ToString(dr["MIRNO"]) : string.Empty,
                                                            MIRDate = dr["MIRDate"] != DBNull.Value ? Convert.ToDateTime(dr["MIRDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            ItemParentGroup = dr["ItemParentGroup"] != DBNull.Value ? Convert.ToString(dr["ItemParentGroup"]) : string.Empty,
                                                            ItemCategory = dr["ItemCategory"] != DBNull.Value ? Convert.ToString(dr["ItemCategory"]) : string.Empty,
                                                            TypeItemServAssets = dr["TypeItemServAssets"] != DBNull.Value ? Convert.ToString(dr["TypeItemServAssets"]) : string.Empty,
                                                            DomesticImport = dr["DomesticImport"] != DBNull.Value ? Convert.ToString(dr["DomesticImport"]) : string.Empty,
                                                            PurchaseBillDirectPB = dr["PurchaseBillDirectPB"] != DBNull.Value ? Convert.ToString(dr["PurchaseBillDirectPB"]) : string.Empty,
                                                            PurchaseBillTypeMRNJWChallan = dr["PurchaseBillTypeMRNJWChallan"] != DBNull.Value ? Convert.ToString(dr["PurchaseBillTypeMRNJWChallan"]) : string.Empty,
                                                            PaymentDays = dr["PaymentDays"] != DBNull.Value ? Convert.ToInt32(dr["PaymentDays"]) : 0,
                                                            PaymentTerm = dr["PaymentTerm"] != DBNull.Value ? Convert.ToString(dr["PaymentTerm"]) : string.Empty,
                                                            Approved = dr["Approved"] != DBNull.Value ? Convert.ToString(dr["Approved"]) : string.Empty,
                                                            ApprovedDate = dr["ApprovedDate"] != DBNull.Value ? Convert.ToDateTime(dr["ApprovedDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            EntryByMachineName = dr["EntryByMachineName"] != DBNull.Value ? Convert.ToString(dr["EntryByMachineName"]) : string.Empty,
                                                            PurchBillEntryId = dr["PurchBillEntryId"] != DBNull.Value ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                            PurchBillYearCode = dr["PurchBillYearCode"] != DBNull.Value ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                            EntryByEmployee = dr["EntryByEmployee"] != DBNull.Value ? Convert.ToString(dr["EntryByEmployee"]) : string.Empty,
                                                            ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            LastUpdatedByEmp = dr["LastUpdatedByEmp"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedByEmp"]) : string.Empty,
                                                            LastUpdatedDate = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["LastUpdatedDate"]).ToString("dd/MM/yyyy") : string.Empty,

                                                        }).ToList();
                    }
                }
                

                else if (ReportType== "GSTSUMMARY"||ReportType== "PURCHASESUMMARYREG")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new BillRegisterModel
                                                        {
                                                            VendorName = dr["VendorName"] == DBNull.Value ? "" : Convert.ToString(dr["VendorName"]),
                                                            InvoiceNo = dr["InvoiceNo"] == DBNull.Value ? "" : Convert.ToString(dr["InvoiceNo"]),
                                                            InvoiceDate = dr["InvoiceDate"] == DBNull.Value ? "" : Convert.ToString(dr["InvoiceDate"]),
                              
                                                            VoucherNo = dr["VoucherNo"] == DBNull.Value ? "" : Convert.ToString(dr["VoucherNo"]),
                                                            VoucherDate = dr["VoucherDate"] == DBNull.Value ? "" : Convert.ToString(dr["VoucherDate"]),
                       
                                                            MRNNo = dr["MRNNo"] == DBNull.Value ? "" : Convert.ToString(dr["MRNNo"]),
                                                            MRNDate = dr["MRNDate"] == DBNull.Value ? "" : Convert.ToString(dr["MRNDate"]),
                                                            BillAmt = dr["BillAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["BillAmt"]),
                                                            TaxableAmt = dr["TaxableAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TaxableAmt"]),
                                                            GSTAmount = dr["GSTAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["GSTAmount"]),
                                                            Currency = dr["Currency"] == DBNull.Value ? "" : Convert.ToString(dr["Currency"]),
                                                            TotalBillQty = dr["TotalBillQty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalBillQty"]),
                                                            TotalDisAmt = dr["TotalDisAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalDisAmt"]),
                                                            TotalItemAmt = dr["TotalItemAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalItemAmt"]),
                                                            CGSTPer = dr["CGSTPer"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CGSTPer"]),
                                                            CGSTAmt = dr["CGSTAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CGSTAmt"]),
                                                            SGSTPer = dr["SGSTPer"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SGSTPer"]),
                                                            SGSTAmt = dr["SGSTAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SGSTAmt"]),
                                                            IGSTPer = dr["IGSTPer"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IGSTPer"]),
                                                            IGSTAmt = dr["IGSTAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["IGSTAmt"]),
                                                            ExpenseAmt = dr["ExpenseAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ExpenseAmt"]),
                                                            InvAmt = dr["InvAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["InvAmt"]),
                                                            GSTType = dr["GSTType"] == DBNull.Value ? "" : Convert.ToString(dr["GSTType"]),
                                                            GstNo = dr["GSTNO"] == DBNull.Value ? "" : Convert.ToString(dr["GSTNO"]),
                                                            VendorAddress = dr["VendorAddress"] == DBNull.Value ? "" : Convert.ToString(dr["VendorAddress"]),
                                                            State = dr["State"] == DBNull.Value ? "" : Convert.ToString(dr["State"]),
                                                            DirectPBOrAgainstMRN = dr["DirectPBOrAgainstMRN"] == DBNull.Value ? "" : Convert.ToString(dr["DirectPBOrAgainstMRN"]),
                                                            EntryByMachineName = dr["EntryByMachineName"] == DBNull.Value ? "" : Convert.ToString(dr["EntryByMachineName"]),
                                                            TypeItemServAssets = dr["TypeItemServAssets"] == DBNull.Value ? "" : Convert.ToString(dr["TypeItemServAssets"]),
                                                            DomesticImport = dr["DomesticImport"] == DBNull.Value ? "" : Convert.ToString(dr["DomesticImport"]),
                                                            PaymentDays = dr["PaymentDays"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PaymentDays"]),
                                                            PurchBillEntryId = dr["PurchBillEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PurchBillEntryId"]),
                                                            PurchBillYearCode = dr["PurchBillYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PurchBillYearCode"]),
                                                            ActualEntryByEmp = dr["ActualEntryByEmp"] == DBNull.Value ? "" : Convert.ToString(dr["ActualEntryDate"]),
                                                            ActualEntryDate = dr["ActualEntryDate"] == DBNull.Value ? "" : Convert.ToString(dr["ActualEntryByEmp"]),
                                                            LastUpdatedByEmp = dr["LastUpdatedByEmp"] == DBNull.Value ? "" : Convert.ToString(dr["LastUpdatedByEmp"]),
                                                            LastUpdatedDate = dr["LastUpdatedDate"] == DBNull.Value ? "" : Convert.ToString(dr["LastUpdatedDate"]),
                                                            Remark = dr["Remark"] == DBNull.Value ? "" : Convert.ToString(dr["Remark"]),
                                                            Country = dr["Country"] == DBNull.Value ? "" : Convert.ToString(dr["Country"])

                                                        }).ToList();
                    }
                }
                else if (ReportType == "VendorItemWiseTrend")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      VendorName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                      PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                                      ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                      HsnNo = dr["HSNNO"] != DBNull.Value ? Convert.ToString(dr["HSNNO"]) : string.Empty,
                                                      TotalQty = dr["TotalQty"] != DBNull.Value ? Convert.ToDecimal(dr["TotalQty"]) : 0,
                                                      Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                      TotalAmount = dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0,

                                                  }).ToList();
                    }
                }
                
                else if (ReportType == "VendorWiseMonthlyTrend")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      VendorName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                      AprAmt = dr["AprAmt"] != DBNull.Value ? Convert.ToDouble(dr["AprAmt"]) : 0,
                                                      MayAmt = dr["MayAmt"] != DBNull.Value ? Convert.ToDouble(dr["MayAmt"]) : 0,
                                                      JunAmt = dr["JunAmt"] != DBNull.Value ? Convert.ToDouble(dr["JunAmt"]) : 0,
                                                      JulAmt = dr["JulAmt"] != DBNull.Value ? Convert.ToDouble(dr["JulAmt"]) : 0,
                                                      AugAmt = dr["AugAmt"] != DBNull.Value ? Convert.ToDouble(dr["AugAmt"]) : 0,
                                                      SepAmt = dr["SepAmt"] != DBNull.Value ? Convert.ToDouble(dr["SepAmt"]) : 0,
                                                      OctAmt = dr["OctAmt"] != DBNull.Value ? Convert.ToDouble(dr["OctAmt"]) : 0,
                                                      NovAmt = dr["NovAmt"] != DBNull.Value ? Convert.ToDouble(dr["NovAmt"]) : 0,
                                                      DecAmt = dr["DecAmt"] != DBNull.Value ? Convert.ToDouble(dr["DecAmt"]) : 0,
                                                      JanAmt = dr["JanAmt"] != DBNull.Value ? Convert.ToDouble(dr["JanAmt"]) : 0,
                                                      FebAmt = dr["FebAmt"] != DBNull.Value ? Convert.ToDouble(dr["FebAmt"]) : 0,
                                                      MarAmt = dr["MarAmt"] != DBNull.Value ? Convert.ToDouble(dr["MarAmt"]) : 0,

                                                      AprQty = dr["AprQty"] != DBNull.Value ? Convert.ToDouble(dr["AprQty"]) : 0,
                                                      MayQty = dr["MayQty"] != DBNull.Value ? Convert.ToDouble(dr["MayQty"]) : 0,
                                                      JunQty = dr["JunQty"] != DBNull.Value ? Convert.ToDouble(dr["JunQty"]) : 0,
                                                      JulQty = dr["JulQty"] != DBNull.Value ? Convert.ToDouble(dr["JulQty"]) : 0,
                                                      AugQty = dr["AugQty"] != DBNull.Value ? Convert.ToDouble(dr["AugQty"]) : 0,
                                                      SepQty = dr["SepQty"] != DBNull.Value ? Convert.ToDouble(dr["SepQty"]) : 0,
                                                      OctQty = dr["OctQty"] != DBNull.Value ? Convert.ToDouble(dr["OctQty"]) : 0,
                                                      NovQty = dr["NovQty"] != DBNull.Value ? Convert.ToDouble(dr["NovQty"]) : 0,
                                                      DecQty = dr["DecQty"] != DBNull.Value ? Convert.ToDouble(dr["DecQty"]) : 0,
                                                      JanQty = dr["JanQty"] != DBNull.Value ? Convert.ToDouble(dr["JanQty"]) : 0,
                                                      FebQty = dr["FebQty"] != DBNull.Value ? Convert.ToDouble(dr["FebQty"]) : 0,
                                                      MarQty = dr["MarQty"] != DBNull.Value ? Convert.ToDouble(dr["MarQty"]) : 0

                                                  }).ToList();
                    }
                }
               else if (ReportType == "VendorItemWiseMonthlyData")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      VendorName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                      PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,
                                                      ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                      Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                      MonthName = dr["MonthName"] != DBNull.Value ? Convert.ToString(dr["MonthName"]) : string.Empty,
                                                      TotalQty = dr["TotalQty"] != DBNull.Value ? Convert.ToDecimal(dr["TotalQty"]) : 0,
                                                      TotalAmount = dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0

                                                  }).ToList();
                    }
                }
               else  if (ReportType == "ItemWiseMonthlyTrend")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      ItemName = dr["ItemName"] != DBNull.Value ? Convert.ToString(dr["ItemName"]) : string.Empty,
                                                      PartCode = dr["PartCode"] != DBNull.Value ? Convert.ToString(dr["PartCode"]) : string.Empty,

                                                      AprAmt = dr["AprAmt"] != DBNull.Value ? Convert.ToDouble(dr["AprAmt"]) : 0,
                                                      MayAmt = dr["MayAmt"] != DBNull.Value ? Convert.ToDouble(dr["MayAmt"]) : 0,
                                                      JunAmt = dr["JunAmt"] != DBNull.Value ? Convert.ToDouble(dr["JunAmt"]) : 0,
                                                      JulAmt = dr["JulAmt"] != DBNull.Value ? Convert.ToDouble(dr["JulAmt"]) : 0,
                                                      AugAmt = dr["AugAmt"] != DBNull.Value ? Convert.ToDouble(dr["AugAmt"]) : 0,
                                                      SepAmt = dr["SepAmt"] != DBNull.Value ? Convert.ToDouble(dr["SepAmt"]) : 0,
                                                      OctAmt = dr["OctAmt"] != DBNull.Value ? Convert.ToDouble(dr["OctAmt"]) : 0,
                                                      NovAmt = dr["NovAmt"] != DBNull.Value ? Convert.ToDouble(dr["NovAmt"]) : 0,
                                                      DecAmt = dr["DecAmt"] != DBNull.Value ? Convert.ToDouble(dr["DecAmt"]) : 0,
                                                      JanAmt = dr["JanAmt"] != DBNull.Value ? Convert.ToDouble(dr["JanAmt"]) : 0,
                                                      FebAmt = dr["FebAmt"] != DBNull.Value ? Convert.ToDouble(dr["FebAmt"]) : 0,
                                                      MarAmt = dr["MarAmt"] != DBNull.Value ? Convert.ToDouble(dr["MarAmt"]) : 0,

                                                      AprQty = dr["AprQty"] != DBNull.Value ? Convert.ToDouble(dr["AprQty"]) : 0,
                                                      MayQty = dr["MayQty"] != DBNull.Value ? Convert.ToDouble(dr["MayQty"]) : 0,
                                                      JunQty = dr["JunQty"] != DBNull.Value ? Convert.ToDouble(dr["JunQty"]) : 0,
                                                      JulQty = dr["JulQty"] != DBNull.Value ? Convert.ToDouble(dr["JulQty"]) : 0,
                                                      AugQty = dr["AugQty"] != DBNull.Value ? Convert.ToDouble(dr["AugQty"]) : 0,
                                                      SepQty = dr["SepQty"] != DBNull.Value ? Convert.ToDouble(dr["SepQty"]) : 0,
                                                      OctQty = dr["OctQty"] != DBNull.Value ? Convert.ToDouble(dr["OctQty"]) : 0,
                                                      NovQty = dr["NovQty"] != DBNull.Value ? Convert.ToDouble(dr["NovQty"]) : 0,
                                                      DecQty = dr["DecQty"] != DBNull.Value ? Convert.ToDouble(dr["DecQty"]) : 0,
                                                      JanQty = dr["JanQty"] != DBNull.Value ? Convert.ToDouble(dr["JanQty"]) : 0,
                                                      FebQty = dr["FebQty"] != DBNull.Value ? Convert.ToDouble(dr["FebQty"]) : 0,
                                                      MarQty = dr["MarQty"] != DBNull.Value ? Convert.ToDouble(dr["MarQty"]) : 0

                                                  }).ToList();
                    }
                }
                else if (ReportType == "VendorWiseMonthlyValueTrend")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new BillRegisterModel
                                                  {
                                                      VendorName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                      AprAmt = dr["AprAmt"] != DBNull.Value ? Convert.ToDouble(dr["AprAmt"]) : 0,
                                                      MayAmt = dr["MayAmt"] != DBNull.Value ? Convert.ToDouble(dr["MayAmt"]) : 0,
                                                      JunAmt = dr["JunAmt"] != DBNull.Value ? Convert.ToDouble(dr["JunAmt"]) : 0,
                                                      JulAmt = dr["JulAmt"] != DBNull.Value ? Convert.ToDouble(dr["JulAmt"]) : 0,
                                                      AugAmt = dr["AugAmt"] != DBNull.Value ? Convert.ToDouble(dr["AugAmt"]) : 0,
                                                      SepAmt = dr["SepAmt"] != DBNull.Value ? Convert.ToDouble(dr["SepAmt"]) : 0,
                                                      OctAmt = dr["OctAmt"] != DBNull.Value ? Convert.ToDouble(dr["OctAmt"]) : 0,
                                                      NovAmt = dr["NovAmt"] != DBNull.Value ? Convert.ToDouble(dr["NovAmt"]) : 0,
                                                      DecAmt = dr["DecAmt"] != DBNull.Value ? Convert.ToDouble(dr["DecAmt"]) : 0,
                                                      JanAmt = dr["JanAmt"] != DBNull.Value ? Convert.ToDouble(dr["JanAmt"]) : 0,
                                                      FebAmt = dr["FebAmt"] != DBNull.Value ? Convert.ToDouble(dr["FebAmt"]) : 0,
                                                      MarAmt = dr["MarAmt"] != DBNull.Value ? Convert.ToDouble(dr["MarAmt"]) : 0,

                                                  }).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }

    }
}
