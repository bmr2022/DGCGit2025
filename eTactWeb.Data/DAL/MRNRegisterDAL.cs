using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class MRNRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public MRNRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        } 
        public async Task<MRNRegisterModel> GetMRNRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MRNRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportMRN", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType.ToString().ToUpper() == "DETAIL")
                    {
                        oCmd = new SqlCommand("SPReportMRN", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SPReportMRN", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@MRNTYpe", MRNType);

                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@docname", docname);
                    oCmd.Parameters.AddWithValue("@gateno", gateno);
                    oCmd.Parameters.AddWithValue("@PONo", PONo);
                    oCmd.Parameters.AddWithValue("@Schno", Schno);
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@invoiceNo", invoiceNo);
                    oCmd.Parameters.AddWithValue("@Mrnno", MRNno);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType.ToString().ToUpper() == "DETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),

                                                       EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? "" : dr["EntryDate"].ToString(),
                                                       VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                       PONo = string.IsNullOrEmpty(dr["PONo"].ToString()) ? "" : dr["PONo"].ToString(),
                                                       POType = string.IsNullOrEmpty(dr["POTYpe"].ToString()) ? "" : dr["POTYpe"].ToString(),
                                                       PODate = string.IsNullOrEmpty(dr["PODate"].ToString()) ? "" : dr["PODate"].ToString(),
                                                       SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                       SchDate = string.IsNullOrEmpty(dr["SchDate"].ToString()) ? "" : dr["SchDate"].ToString(),
                                                       PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       FOC = string.IsNullOrEmpty(dr["FOC"].ToString()) ? "" : dr["FOC"].ToString(),

                                                       BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       ShortExcessQty = Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                       AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                       AltQty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                       Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                       ShelfLife = Convert.ToInt32(dr["ShelfLife"].ToString()),
                                                       LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEmp"].ToString()) ? "" : dr["UpdatedByEmp"].ToString(),
                                                       LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdateDate"].ToString()) ? "" : dr["LastUpdateDate"].ToString(),

                                                       ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                       ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),

                                                       EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),

                                                       Amount = Convert.ToDecimal(dr["Amount"]),
                                                       Batchno = string.IsNullOrEmpty(dr["Batchno"].ToString()) ? "" : dr["Batchno"].ToString(),
                                                       Uniquebatchno = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),
                                                       SupplierBatchNo = string.IsNullOrEmpty(dr["SupplierBatchNo"].ToString()) ? "" : dr["SupplierBatchNo"].ToString(),
                                                       QCCompleted = string.IsNullOrEmpty(dr["QCCompleted"].ToString()) ? "" : dr["QCCompleted"].ToString(),
                                                       MRNQCCompleted = string.IsNullOrEmpty(dr["MRNQCCompleted"].ToString()) ? "" : dr["MRNQCCompleted"].ToString(),
                                                       NeedTochkQC = string.IsNullOrEmpty(dr["NeedTochkQC"].ToString()) ? "" : dr["NeedTochkQC"].ToString(),
                                                       Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),

                                                       DepName = string.IsNullOrEmpty(dr["DeptName"].ToString()) ? "" : dr["DeptName"].ToString(),
                                                       RecInStore = string.IsNullOrEmpty(dr["RecInStore"].ToString()) ? "" : dr["RecInStore"].ToString(),
                                                       MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                       VendAddress = string.IsNullOrEmpty(dr["VendAddress"].ToString()) ? "" : dr["VendAddress"].ToString(),
                                                       TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                       NetAmout = Convert.ToDecimal(dr["NetAmt"].ToString()),

                                                   }).ToList();
                    }
                }

                else if (ReportType.ToString().ToUpper() == "ItemWiseConsolidated".ToUpper()) //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       BillQty = Convert.ToDecimal(dr["TotalBillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       ShortExcessQty = Convert.ToDecimal(dr["ShortExcQty"].ToString()),
                                                       TotalAmt = Convert.ToDecimal(dr["Value"].ToString()),
                                                       Rate = Convert.ToDecimal(dr["AvgRate"].ToString()),

                                                       MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                   }).ToList();
                    }
                } 

                else if (ReportType.ToString().ToUpper() == "vendorItemWiseConsolidated".ToUpper())
                {
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MRNRegisterDetail
                                                       {
                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                           PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                           ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                           BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                           ShortExcessQty = Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                                           Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                            MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                           TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                       }).ToList();
                        }
                    }
                }
                else if (ReportType.ToString().ToUpper() == "vendorItemWiseSummary".ToUpper())
                {
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MRNRegisterDetail
                                                       { 
                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                           GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                           GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                           MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                           MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),

                                                           EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? "" : dr["EntryDate"].ToString(),
                                                           InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                           InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                           DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                           PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                           ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                           BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                           ShortExcessQty = Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                                           Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                           MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                           Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                           LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEmp"].ToString()) ? "" : dr["UpdatedByEmp"].ToString(),
                                                           LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdateDate"].ToString()) ? "" : dr["LastUpdateDate"].ToString(),
                                                           ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                           ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),
                                                           EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                           FOC = string.IsNullOrEmpty(dr["FOC"].ToString()) ? "" : dr["FOC"].ToString(),
                                                           DepName = string.IsNullOrEmpty(dr["DeptName"].ToString()) ? "" : dr["DeptName"].ToString(),
                                                           RecInStore = string.IsNullOrEmpty(dr["RecInStore"].ToString()) ? "" : dr["RecInStore"].ToString(),
                                                           Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),
                                                           TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                           NetAmout = Convert.ToDecimal(dr["NetAmt"].ToString()),
                                                           Rate = Convert.ToDecimal(dr["rate"].ToString()),

                                                       }).ToList();
                        }
                    }
                }
                else if (ReportType.ToString().ToUpper() == "vendorWiseConsolidated".ToUpper())
                {
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MRNRegisterDetail
                                                       {
   //                                                          SELECT  distinct   Account_Name'VendorName',
   //isnull( sum(gm.BillQty),0)  BillQty , isnull(sum(gm.RecQty),0) 'RecQty',
   //isnull( sum(gm.BillQty),0) - isnull(sum(gm.RecQty),0) 'ShortExcessQty',
   //Round(isnull( sum(gm.BillQty),0)  * avg(Rate),2)  'TotalAmt',  MRNTypes 'MRNType'


                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                            BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                           ShortExcessQty = Convert.ToDecimal(dr["ShortExcessQty"].ToString()),
                                                            MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                           TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                       }).ToList();
                        }
                    }
                }
               else  if (ReportType.ToString().ToUpper() == "DAYWISEMRNENTRYLIST") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),

                                                        VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                        FOC = string.IsNullOrEmpty(dr["FOC"].ToString()) ? "" : dr["FOC"].ToString(),
                                                         LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEmp"].ToString()) ? "" : dr["UpdatedByEmp"].ToString(),
                                                       LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdateDate"].ToString()) ? "" : dr["LastUpdateDate"].ToString(),

                                                       ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                       ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),

                                                       EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                        MRNQCCompleted = string.IsNullOrEmpty(dr["MRNQCCompleted"].ToString()) ? "" : dr["MRNQCCompleted"].ToString(),
                                                       NeedTochkQC = string.IsNullOrEmpty(dr["NeedTochkQC"].ToString()) ? "" : dr["NeedTochkQC"].ToString(),
                                                       Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),

                                                       DepName = string.IsNullOrEmpty(dr["DeptName"].ToString()) ? "" : dr["DeptName"].ToString(),
                                                       RecInStore = string.IsNullOrEmpty(dr["RecInStore"].ToString()) ? "" : dr["RecInStore"].ToString(),
                                                       MRNType = string.IsNullOrEmpty(dr["MRNType"].ToString()) ? "" : dr["MRNType"].ToString(),
                                                       VendAddress = string.IsNullOrEmpty(dr["VendAddress"].ToString()) ? "" : dr["VendAddress"].ToString(),
                                                       TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                       NetAmout = Convert.ToDecimal(dr["NetAmt"].ToString()),
                                                       BillQty = Convert.ToDecimal(dr["TotalBillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       ShortExcessQty = Convert.ToDecimal(dr["ShortExcQty"].ToString()),
                                                   }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PENDGATEFORMRN")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),

                                                       GateYearCode = Convert.ToInt16(dr["YearCode"].ToString()),

                                                       EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? "" : dr["EntryDate"].ToString(),
                                                       VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                       Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                       PreparedByEmp = string.IsNullOrEmpty(dr["PrepByEMp"].ToString()) ? "" : dr["PrepByEMp"].ToString(),
                                                       ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                       ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),

                                                       UpdatedByEMp = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),
                                                       LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? "" : dr["LastUpdatedDate"].ToString(),

                                                       EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                       LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),


                                                   }).ToList();
                    }
                }
               else if (ReportType.ToString().ToUpper() == "PENDMRNFORQC(SUMMARY)") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                            GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                            GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                            MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                            MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                            VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                            InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                            InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                             MRNType = string.IsNullOrEmpty(dr["MRNTypes"].ToString()) ? "" : dr["MRNTypes"].ToString(),
                                                            QCCompleted = string.IsNullOrEmpty(dr["QCCompleted"].ToString()) ? "" : dr["QCCompleted"].ToString(),
                                                            TotalBillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                            RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                            ShortExcessQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                            TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                            PurchaseBillPosted = string.IsNullOrEmpty(dr["PurchaseBillPosted"].ToString()) ? "" : dr["PurchaseBillPosted"].ToString(),
                                                     }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PENDMRNFORQC(DEATIL)") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                       VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                       PartCode = string.IsNullOrEmpty(dr["partcode"].ToString()) ? "" : dr["partcode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       PONo = string.IsNullOrEmpty(dr["PONO"].ToString()) ? "" : dr["PONO"].ToString(),
                                                       PODate = string.IsNullOrEmpty(dr["PoDate"].ToString()) ? "" : dr["PoDate"].ToString(),
                                                       POType = string.IsNullOrEmpty(dr["PoType"].ToString()) ? "" : dr["PoType"].ToString(),
                                                       SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                       SchDate = string.IsNullOrEmpty(dr["Schdate"].ToString()) ? "" : dr["Schdate"].ToString(),
                                                       DepName = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),

                                                       MRNType = string.IsNullOrEmpty(dr["MRNTypes"].ToString()) ? "" : dr["MRNTypes"].ToString(),
                                                       QCCompleted = string.IsNullOrEmpty(dr["QCCompleted"].ToString()) ? "" : dr["QCCompleted"].ToString(),
                                                       Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                       TotalBillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                       TotalAmt = Convert.ToDecimal(dr["ItemAmount"].ToString()),

                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       ShortExcessQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       AltQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                       AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                       PurchaseBillPosted = string.IsNullOrEmpty(dr["PurchaseBillPosted"].ToString()) ? "" : dr["PurchaseBillPosted"].ToString(),
                                                   
                                                   }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "mrnqcdetail".ToUpper()) //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                       VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                       PartCode = string.IsNullOrEmpty(dr["partcode"].ToString()) ? "" : dr["partcode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       PONo = string.IsNullOrEmpty(dr["PONO"].ToString()) ? "" : dr["PONO"].ToString(),
                                                       PODate = string.IsNullOrEmpty(dr["PoDate"].ToString()) ? "" : dr["PoDate"].ToString(),
                                                       POType = string.IsNullOrEmpty(dr["PoType"].ToString()) ? "" : dr["PoType"].ToString(),
                                                       SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                       MRNType = string.IsNullOrEmpty(dr["MRNTypes"].ToString()) ? "" : dr["MRNTypes"].ToString(),
                                                       QCCompleted = string.IsNullOrEmpty(dr["QCCompleted"].ToString()) ? "" : dr["QCCompleted"].ToString(),
                                                       Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                       TotalBillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       ShortExcessQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       AltQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                       AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                       PurchaseBillPosted = string.IsNullOrEmpty(dr["PurchaseBillPosted"].ToString()) ? "" : dr["PurchaseBillPosted"].ToString(),
                                                       AcceptedQty = Convert.ToDecimal(dr["AcceptedQty"].ToString()),
                                                       AltAcceptedQty = Convert.ToDecimal(dr["AltAcceptedQty"].ToString()),
                                                       RejectedQty = Convert.ToDecimal(dr["RejectedQty"].ToString()),
                                                       HoldQty = Convert.ToDecimal(dr["HoldQty"].ToString()),
                                                       Reworkqty = Convert.ToDecimal(dr["Reworkqty"].ToString()),
                                                   }).ToList();
                    }

                }
                else if (ReportType.ToString().ToUpper() == "Short Excess Register".ToUpper()) //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MRNRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MRNRegisterDetail
                                                   {
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNo"].ToString()) ? "" : dr["MRNNo"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                       MRNType = string.IsNullOrEmpty(dr["MRNTypes"].ToString()) ? "" : dr["MRNTypes"].ToString(),
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       GDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? "" : Convert.ToDateTime(dr["GateDate"]).ToString("dd/MM/yyyy"),

                                                       InvoiceNo = string.IsNullOrEmpty(dr["InvNo"].ToString()) ? "" : dr["InvNo"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? "" : Convert.ToDateTime(dr["InvDate"]).ToString("dd/MM/yyyy"),

                                                       Account_Name = string.IsNullOrEmpty(dr["Account_Name"].ToString()) ? "" : dr["Account_Name"].ToString(),
                                                       PONo = string.IsNullOrEmpty(dr["pono"].ToString()) ? "" : dr["pono"].ToString(),
                                                      
                                                       SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                       PartCode = string.IsNullOrEmpty(dr["partcode"].ToString()) ? "" : dr["partcode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["item_name"].ToString()) ? "" : dr["item_name"].ToString(),
                                                       BillQty = string.IsNullOrEmpty(dr["BillQty"].ToString()) ? 0 : Convert.ToDecimal(dr["BillQty"]),
                                                       RecQty = string.IsNullOrEmpty(dr["RecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["RecQty"]),
                                                       Rate = string.IsNullOrEmpty(dr["Rate"].ToString()) ? 0 : Convert.ToDecimal(dr["Rate"]),
                                                       TotalAmt = string.IsNullOrEmpty(dr["TotalAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["TotalAmt"]),
                                                       NetAmout = string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? 0 : Convert.ToDecimal(dr["NetAmt"]),
                                                       AltQty = string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? 0 : Convert.ToDecimal(dr["AltRecQty"]),
                                                       ShortExcessQty = string.IsNullOrEmpty(dr["ShortExcessQty"].ToString()) ? 0 : Convert.ToDecimal(dr["ShortExcessQty"]),
                                                       PODate = string.IsNullOrEmpty(dr["PoDate"].ToString()) ? "" : Convert.ToDateTime(dr["PoDate"]).ToString("dd/MM/yyyy"),

                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),
                                                       DepName = string.IsNullOrEmpty(dr["DeptName"].ToString()) ? "" : dr["DeptName"].ToString(),
                                                       AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                       Batchno = string.IsNullOrEmpty(dr["Batchno"].ToString()) ? "" : dr["Batchno"].ToString(),
                                                       Uniquebatchno = string.IsNullOrEmpty(dr["Uniquebatchno"].ToString()) ? "" : dr["Uniquebatchno"].ToString(),

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
 
        public async Task<ResponseResult> FillGateNo(string FromDate,string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGateNo"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMRNNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMRNNo"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDocument(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDocument"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillVendor(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillVendor"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInvoice(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillInvoice"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemNamePartcode"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPONO"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
     public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSchNo"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRN", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

         
      
    }
}
