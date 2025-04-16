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

namespace eTactWeb.Data.DAL
{
    public class GateEntryRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        public GateEntryRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<GateEntryRegisterModel> GetGateRegisterData(string ReportType, string FromDate, string ToDate, string gateno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new GateEntryRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpReportGate", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType.ToString().ToUpper() == "DETAIL")
                    {
                        oCmd = new SqlCommand("SpReportGate", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SpReportGate", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                   
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
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
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                         GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                         GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                         EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? "" : dr["EntryDate"].ToString(),
                                                         VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                         InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                         InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                         DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                         POTypeServiceItem = string.IsNullOrEmpty(dr["POTypeServItem"].ToString()) ? "" : dr["POTypeServItem"].ToString(),
                                                         PONo = string.IsNullOrEmpty(dr["PONo"].ToString()) ? "" : dr["PONo"].ToString(),
                                                         SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["Item_Name"].ToString()) ? "" : dr["Item_Name"].ToString(),
                                                         Qty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                         Amout = Convert.ToDecimal(dr["Amount"].ToString()),
                                                         AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                         POtype = string.IsNullOrEmpty(dr["POTYpe"].ToString()) ? "" : dr["POTYpe"].ToString(),
                                                         PendPOQty = Convert.ToDecimal(dr["PendPOQty"].ToString()),
                                                         AltQty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                         
                                                         SaleBillNo = string.IsNullOrEmpty(dr["SaleBillNo"].ToString()) ? "" : dr["SaleBillNo"].ToString(),
                                                         SaleBillYearCode = Convert.ToInt16(dr["SaleBillYearCode"].ToString()) ,
                                                         SaleBillQty = Convert.ToDecimal(dr["SaleBillQty"].ToString()),

                                                         AgainstChallanNo = string.IsNullOrEmpty(dr["AgainstChallanNo"].ToString()) ? "" : dr["AgainstChallanNo"].ToString(),
                                                         AgainstChallanQty = Convert.ToInt32(dr["ShelfLife"].ToString()),

                                                        PreparedByEmp = string.IsNullOrEmpty(dr["PrepByEMp"].ToString()) ? "" : dr["PrepByEMp"].ToString(),
                                                        ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEnteredBy"].ToString()) ? "" : dr["ActualEnteredBy"].ToString(),
                                                        EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                        Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                        ShelfLife = Convert.ToInt32(dr["ShelfLife"].ToString()),
                                                        LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),
                                                         GateEntryId = Convert.ToInt32(dr["GateEntryId"].ToString()),
                                                         GateYearCode = Convert.ToInt32(dr["GateYearCode"].ToString()),

                                                         // = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),

                                                         //Qty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                         Amount = Convert.ToDecimal(dr["Amount"])
                                                      }).ToList();
                    }
                }

                else if (ReportType.ToString().ToUpper() == "ITEMSUMMARY") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                          PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                          Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         Qty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                         Amout = Convert.ToDecimal(dr["Amount"].ToString()),
                                                         AltQty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                         AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                     }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PARTYITEMSUMMARY")// 
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                         VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         Qty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         AltQty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                         AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                         City = string.IsNullOrEmpty(dr["VendorCity"].ToString()) ? "" : dr["VendorCity"].ToString(),
                                                         CompanyAddress = string.IsNullOrEmpty(dr["VendAddress"].ToString()) ? "" : dr["VendAddress"].ToString(),
                                                     }).ToList();
                    }
                } 
                else if (ReportType.ToString().ToUpper() == "PARTYITEMPOSUMMARY")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                         VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                         PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                         ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                         Qty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                         Unit = string.IsNullOrEmpty(dr["unit"].ToString()) ? "" : dr["unit"].ToString(),
                                                         PONo = string.IsNullOrEmpty(dr["PONo"].ToString()) ? "" : dr["PONo"].ToString(),
                                                         SchNo = string.IsNullOrEmpty(dr["SchNo"].ToString()) ? "" : dr["SchNo"].ToString(),
                                                         AltQty = Convert.ToDecimal(dr["altqty"].ToString()),
                                                         AltUnit = string.IsNullOrEmpty(dr["altunit"].ToString()) ? "" : dr["altunit"].ToString(),
                                                         POYearCode = Convert.ToInt32 (dr["POYearCode"].ToString()),
                                                         SchYearCode = Convert.ToInt32(dr["SchYearCode"].ToString()),
                                                     }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "DAYWISEGATEENTRYLIST")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                         GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                         GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                         GateEntryId = Convert.ToInt16(dr["GateEntryId"].ToString()),
                                                         GateYearCode = Convert.ToInt16(dr["GateYearCode"].ToString()),

                                                         EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? "" : dr["EntryDate"].ToString(),
                                                         VendorName = string.IsNullOrEmpty(dr["VendorName"].ToString()) ? "" : dr["VendorName"].ToString(),
                                                         InvoiceNo = string.IsNullOrEmpty(dr["Invoiceno"].ToString()) ? "" : dr["Invoiceno"].ToString(),
                                                         InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? "" : dr["InvoiceDate"].ToString(),
                                                         DocNo = string.IsNullOrEmpty(dr["DocName"].ToString()) ? "" : dr["DocName"].ToString(),
                                                         Qty = Convert.ToDecimal(dr["TotalQty"].ToString()),
                                                         Amout = Convert.ToDecimal(dr["TotalAmount"].ToString()),
                                                          Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                         PreparedByEmp = string.IsNullOrEmpty(dr["PrepByEMp"].ToString()) ? "" : dr["PrepByEMp"].ToString(),
                                                         ActualEntryByEmp = string.IsNullOrEmpty(dr["ActualEntryByEMp"].ToString()) ? "" : dr["ActualEntryByEMp"].ToString(),
                                                         UpdatedByEMp = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),
                                                        
                                                         EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                         City = string.IsNullOrEmpty(dr["VendorCity"].ToString()) ? "" : dr["VendorCity"].ToString(),
                                                         LastUpdatedDate = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? "" : dr["LastUpdatedDate"].ToString(),
                                                         LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),


                                                     }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "PENDGATEFORMRN")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.GateEntryRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new GateEntryRegisterDetail
                                                     {
                                                         GateNo = string.IsNullOrEmpty(dr["GateNo"].ToString()) ? "" : dr["GateNo"].ToString(),
                                                         GDate = string.IsNullOrEmpty(dr["GDate"].ToString()) ? "" : dr["GDate"].ToString(),
                                                         GateEntryId = Convert.ToInt16(dr["EntryId"].ToString()),
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
                                                         City = string.IsNullOrEmpty(dr["VendorCity"].ToString()) ? "" : dr["VendorCity"].ToString(),
                                                          LastUpdatedby = string.IsNullOrEmpty(dr["UpdatedByEMp"].ToString()) ? "" : dr["UpdatedByEMp"].ToString(),


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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportGate", SqlParams);
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
