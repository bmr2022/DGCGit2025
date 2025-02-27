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
    public class MIRRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        public MIRRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
        } 
        public async Task<MIRRegisterModel> GetRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string MIRNo, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MIRRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportMRIR", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType.ToString().ToUpper() == "DETAIL")
                    {
                        oCmd = new SqlCommand("SPReportMRIR", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SPReportMRIR", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@MRNTYpe", MRNType);

                    oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@MIRNo", MIRNo);
                    oCmd.Parameters.AddWithValue("@gateno", gateno);
                    oCmd.Parameters.AddWithValue("@PONo", PONo);
                    oCmd.Parameters.AddWithValue("@Schno", Schno);
                    oCmd.Parameters.AddWithValue("@VendorName", VendorName);
                    oCmd.Parameters.AddWithValue("@InvNo", invoiceNo);

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
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       
                                                       VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       MIRNo = string.IsNullOrEmpty(dr["MIRNo"].ToString()) ? "" : dr["MIRNo"].ToString(),
                                                       MIRDate = string.IsNullOrEmpty(dr["MIRDate"].ToString()) ? "" : dr["MIRDate"].ToString(),
                                                       EntryDate = string.IsNullOrEmpty(dr["MIREntryDate"].ToString()) ? "" : dr["MIREntryDate"].ToString(),
                                                       MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                       MRNJWCustJW = string.IsNullOrEmpty(dr["MRNJWCustJW"].ToString()) ? "" : dr["MRNJWCustJW"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["InvNo"].ToString()) ? "" : dr["InvNo"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? "" : dr["InvDate"].ToString(),
                                                       PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       AcceptedQty = Convert.ToDecimal(dr["AcceptedQty"].ToString()),
                                                       RejectedQty = Convert.ToDecimal(dr["RejectedQty"].ToString()),
                                                       HoldQty = Convert.ToDecimal(dr["HoldQty"].ToString()),
                                                       Reworkqty = Convert.ToDecimal(dr["ReworkQty"].ToString()),
                                                       BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       FromStore = string.IsNullOrEmpty(dr["FromStore"].ToString()) ? "" : dr["FromStore"].ToString(),
                                                       AcceptStore = string.IsNullOrEmpty(dr["AcceptStore"].ToString()) ? "" : dr["AcceptStore"].ToString(),
                                                       RejStore = string.IsNullOrEmpty(dr["RejStore"].ToString()) ? "" : dr["RejStore"].ToString(),
                                                       RewStore = string.IsNullOrEmpty(dr["RewStore"].ToString()) ? "" : dr["RewStore"].ToString(),
                                                       AltAcceptedQty = Convert.ToDecimal(dr["AltAcceptedQty"].ToString()),
                                                       AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                        
                                                       LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEmp"].ToString()) ? "" : dr["UpdatedByEmp"].ToString(),
                                                       LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdateDate"].ToString()) ? "" : dr["LastUpdateDate"].ToString(),

                                                       ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                       ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),

                                                       EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),

                                                    }).ToList();
                    }
                }

                else if (ReportType.ToString().ToUpper() == "ItemWiseConsolidated".ToUpper()) //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                       ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                       Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       BillQty = Convert.ToDecimal(dr["TotalBillQty"].ToString()),
                                                       RecQty = Convert.ToDecimal(dr["RecQty"].ToString()), 
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
                            model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MIRRegisterDetail
                                                       {
                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                           PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                           ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                           BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()), 
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
                            model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MIRRegisterDetail
                                                       {
                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                           PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                           ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            AcceptedQty = Convert.ToDecimal(dr["AcceptedQty"].ToString()),
                                                           RejectedQty = Convert.ToDecimal(dr["RejectedQty"].ToString()),
                                                           HoldQty = Convert.ToDecimal(dr["HoldQty"].ToString()),
                                                           Reworkqty = Convert.ToDecimal(dr["ReworkQty"].ToString()),
                                                           BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                           Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       }).ToList();
                        }
                    }
                }
                else if (ReportType.ToString().ToUpper() == "vendorWiseConsolidated".ToUpper())
                {
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                       select new MIRRegisterDetail
                                                       {
   //                                                          SELECT  distinct   Account_Name'VendorName',
   //isnull( sum(gm.BillQty),0)  BillQty , isnull(sum(gm.RecQty),0) 'RecQty',
   //isnull( sum(gm.BillQty),0) - isnull(sum(gm.RecQty),0) 'ShortExcessQty',
   //Round(isnull( sum(gm.BillQty),0)  * avg(Rate),2)  'TotalAmt',  MRNTypes 'MRNType'


                                                           VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                            BillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                           RecQty = Convert.ToDecimal(dr["RecQty"].ToString()), 
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
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                        MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                       MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),

                                                        VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                       InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                       InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                       DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
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
                                                   }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PENDGATEFORMRN")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(), 

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
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                            GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                             MRNNo = string.IsNullOrEmpty(dr["MRNNO"].ToString()) ? "" : dr["MRNNO"].ToString(),
                                                            MRNDate = string.IsNullOrEmpty(dr["MRNDate"].ToString()) ? "" : dr["MRNDate"].ToString(),
                                                            VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                            InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                            InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                             MRNType = string.IsNullOrEmpty(dr["MRNTypes"].ToString()) ? "" : dr["MRNTypes"].ToString(),
                                                            QCCompleted = string.IsNullOrEmpty(dr["QCCompleted"].ToString()) ? "" : dr["QCCompleted"].ToString(),
                                                            TotalBillQty = Convert.ToDecimal(dr["BillQty"].ToString()),
                                                            RecQty = Convert.ToDecimal(dr["RecQty"].ToString()),
                                                             TotalAmt = Convert.ToDecimal(dr["TotalAmt"].ToString()),
                                                            PurchaseBillPosted = string.IsNullOrEmpty(dr["PurchaseBillPosted"].ToString()) ? "" : dr["PurchaseBillPosted"].ToString(),
                                                     }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PENDMRNFORQC(DEATIL)") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(), 
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
                                                        Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       AltAcceptedQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
                                                       AltUnit = string.IsNullOrEmpty(dr["AltUnit"].ToString()) ? "" : dr["AltUnit"].ToString(),
                                                       PurchaseBillPosted = string.IsNullOrEmpty(dr["PurchaseBillPosted"].ToString()) ? "" : dr["PurchaseBillPosted"].ToString(),
                                                   
                                                   }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "mrnqcdetail".ToUpper()) //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.MIRRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                   select new MIRRegisterDetail
                                                   {
                                                       GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
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
                                                        Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                       AltPendQty = Convert.ToDecimal(dr["AltRecQty"].ToString()),
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMIRNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMIRNO"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemPartcode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartcode"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMRIR", SqlParams);
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
