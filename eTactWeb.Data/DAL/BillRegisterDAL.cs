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
                else if (ReportType== "GSTSUMMARY"||ReportType== "PURCHASESUMMARYREG")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.BillRegisterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new BillRegisterModel
                                                        {
                                                            VendorName = dr["VendorName"] == DBNull.Value ? "" : Convert.ToString(dr["VendorName"]),
                                                            InvoiceNo = dr["InvoiceNo"] == DBNull.Value ? "" : Convert.ToString(dr["InvoiceNo"]),
                                                            InvoiceDate = dr["InvoiceDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["InvoiceDate"]),
                                                            VoucherNo = dr["VoucherNo"] == DBNull.Value ? "" : Convert.ToString(dr["VoucherNo"]),
                                                            VoucherDate = dr["VoucherDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VoucherDate"]),
                                                            MRNNo = dr["MRNNo"] == DBNull.Value ? "" : Convert.ToString(dr["MRNNo"]),
                                                            MRNDate = dr["MRNDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["MRNDate"]),
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
                                                            GSTNO = dr["GSTNO"] == DBNull.Value ? "" : Convert.ToString(dr["GSTNO"]),
                                                            VendorAddress = dr["VendorAddress"] == DBNull.Value ? "" : Convert.ToString(dr["VendorAddress"]),
                                                            State = dr["State"] == DBNull.Value ? "" : Convert.ToString(dr["State"]),
                                                            DirectPBOrAgainstMRN = dr["DirectPBOrAgainstMRN"] == DBNull.Value ? "" : Convert.ToString(dr["DirectPBOrAgainstMRN"]),
                                                            EntryByMachineName = dr["EntryByMachineName"] == DBNull.Value ? "" : Convert.ToString(dr["EntryByMachineName"]),
                                                            TypeItemServAssets = dr["TypeItemServAssets"] == DBNull.Value ? "" : Convert.ToString(dr["TypeItemServAssets"]),
                                                            DomesticImport = dr["DomesticImport"] == DBNull.Value ? "" : Convert.ToString(dr["DomesticImport"]),
                                                            PaymentDays = dr["PaymentDays"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PaymentDays"]),
                                                            PurchBillEntryId = dr["PurchBillEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PurchBillEntryId"]),
                                                            PurchBillYearCode = dr["PurchBillYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PurchBillYearCode"]),
                                                            ActualEntryByEmp = dr["ActualEntryByEmp"] == DBNull.Value ? "" : Convert.ToString(dr["ActualEntryByEmp"]),
                                                            ActualEntryDate = dr["ActualEntryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                            LastUpdatedByEmp = dr["LastUpdatedByEmp"] == DBNull.Value ? "" : Convert.ToString(dr["LastUpdatedByEmp"]),
                                                            LastUpdatedDate = dr["LastUpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                            Remark = dr["Remark"] == DBNull.Value ? "" : Convert.ToString(dr["Remark"]),
                                                            Country = dr["Country"] == DBNull.Value ? "" : Convert.ToString(dr["Country"])

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
