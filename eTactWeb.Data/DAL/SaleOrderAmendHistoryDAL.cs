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
    public class SaleOrderAmendHistoryDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        public SaleOrderAmendHistoryDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate, int AccountCode, string SOno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "SOAmendmentHistoryReport"),
            new SqlParameter("@Dashboardflag", "FillSOPARTCODE"),
            new SqlParameter("@SOno", SOno),
            new SqlParameter("@AccountCode", AccountCode),
            new SqlParameter("@FromDate", FromDate),
            new SqlParameter("@ToDate", ToDate)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching Sale Order Amendment History.", ex);
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillAccountCode(string FromDate, string ToDate, string partCode, string ItemCode, string SOno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "SOAmendmentHistoryReport"),
            new SqlParameter("@Dashboardflag", "FillSOCustName"),
            new SqlParameter("@partCode", partCode),
            new SqlParameter("@ItemCode", ItemCode),
            new SqlParameter("@SOno", SOno),
            new SqlParameter("@FromDate", FromDate),
            new SqlParameter("@ToDate", ToDate)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching Sale Order Amendment History.", ex);
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillSONO(string FromDate, string ToDate, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "SOAmendmentHistoryReport"),
            new SqlParameter("@Dashboardflag", "FillSONO"),
            new SqlParameter("@AccountCode", AccountCode),
            new SqlParameter("@FromDate", FromDate),
            new SqlParameter("@ToDate", ToDate)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching Sale Order Amendment History.", ex);
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate, int AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "POAmendmentHistoryReport"),
            new SqlParameter("@Dashboardflag", "FillPONO"),
            new SqlParameter("@AccountCode", AccountCode),
            new SqlParameter("@FromDate", FromDate),
            new SqlParameter("@ToDate", ToDate)
        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching Sale Order Amendment History.", ex);
            }

            return _ResponseResult;
        }

        public async Task<SaleOrderAmendHistoryModel> SaleOrderAmendHistory(
         string reportType,
         string flag,
         string dashboardFlag,
         string fromDate,
         string toDate,
         int accountCode,
         string poNo,
         string partCode,
         int itemCode,
         string soNo)
        {
            var model = new SaleOrderAmendHistoryModel();
            var oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                using (SqlCommand oCmd = new SqlCommand("SpReportOrderAmendHistoryOnGrid", myConnection))
                {
                    oCmd.CommandType = CommandType.StoredProcedure;

                    var fromDt = CommonFunc.ParseFormattedDate(fromDate);
                    var toDt = CommonFunc.ParseFormattedDate(toDate);

                    oCmd.Parameters.AddWithValue("@reportType", reportType ?? "Sale Order Detail");
                    oCmd.Parameters.AddWithValue("@flag", flag ?? "SOAmendmentHistoryReport");
                    oCmd.Parameters.AddWithValue("@Dashboardflag", dashboardFlag ?? "");
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@Accountcode", accountCode);
                    oCmd.Parameters.AddWithValue("@POno", poNo ?? "");
                    oCmd.Parameters.AddWithValue("@partCode", partCode ?? "");
                    oCmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    oCmd.Parameters.AddWithValue("@SOno", soNo ?? "");

                    await myConnection.OpenAsync();

                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                    if (reportType.ToString()== "Sale Order Detail") //done&working
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            {
                                model.SaleOrderAmendHistoryDetail = oDataSet.Tables[0].AsEnumerable()
                                    .Select(dr => new SaleOrderAmendHistoryDetail
                                    {
                                        VendorName = dr["VendorName"]?.ToString(),
                                        SONO = dr["SONO"]?.ToString(),
                                        SODate = dr.Field<DateTime?>("SODate"),
                                        WEF = dr.Field<DateTime?>("WEF"),
                                        SOClosedate = dr.Field<DateTime?>("SOClosedate"),
                                        OrderType = dr["ordertype"]?.ToString(),
                                        SOType = dr["SOtype"]?.ToString(),
                                        SOFor = dr["SOFor"]?.ToString(),
                                        AmmNo = dr["AmmNo"]?.ToString(),
                                        AmmEffDate = dr.Field<DateTime?>("AmmEffDate"),
                                        PartCode = dr["PartCode"]?.ToString(),
                                        ItemName = dr["ItemName"]?.ToString(),
                                        HSNNo = dr["HSNNo"]?.ToString(),
                                        SOQty = dr.Field<decimal?>("SOQty"),
                                        Unit = dr["Unit"]?.ToString(),
                                        Rate = dr.Field<decimal?>("Rate"),
                                        DiscPer = dr.Field<decimal?>("DiscPer"),
                                        DiscRs = dr.Field<decimal?>("DiscRs"),
                                        Amount = dr.Field<decimal?>("Amount"),
                                        OldRate = dr.Field<decimal?>("OldRate"),
                                        Remark = dr["Remark"]?.ToString(),
                                        AmmendmentReason = dr["AmmendmentReason"]?.ToString(),
                                        RateInOtherCurr = dr.Field<decimal?>("RateInOtherCurr"),
                                        RateApplicableOnUnit = dr["RateApplicableOnUnit"]?.ToString(),
                                        AltSOQty = dr.Field<decimal?>("AltSOQty"),
                                        AltUnit = dr["AltUnit"]?.ToString(),
                                        ShippingAddress = dr["ShippingAddress"]?.ToString(),
                                        BasicAmount = dr.Field<decimal?>("BasicAmount"),
                                        NetAmount = dr.Field<decimal?>("NetAmount"),
                                        SOAmendEntryID = Convert.ToInt32(dr["SOAmendEntryID"].ToString()),
                                        SOAmendYearCode = Convert.ToInt32(dr["SOAmendYearCode"].ToString()),
                                        AmendSO = dr["AmendSO"]?.ToString(),
                                        AmendSOSeq = Convert.ToInt32(dr["AmendSOSeq"].ToString()),
                                        SOEntryId = Convert.ToInt32(dr["SOEntryId"].ToString()),
                                        SOYearCode = Convert.ToInt32(dr["SOYearCode"].ToString()),
                                        SOCanceled =  dr["SOCanceled"]?.ToString(),
                                        SOComplete = dr["SOComplete"]?.ToString(),
                                        Active = dr["Active"]?.ToString(),
                                        AccountCode = Convert.ToInt32(dr["AccountCode"].ToString()),
                                    }).ToList();
                            }
                        }
                    }
//                    else if (reportType.ToString() == "Sale Order Summary") //done&working{
//                    {
//                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
//                        {
//                            model.SaleOrderAmendHistoryDetail = oDataSet.Tables[0].AsEnumerable()
//                                    .Select(dr => new SaleOrderAmendHistoryDetail
                                    
//                                    {
//                                        VendorName = dr["VendorName"]?.ToString() ?? "",
//                                        SONO = dr["SOno"]?.ToString() ?? "",
//                                        SODate = Convert.ToDateTime(dr["SODate"]),
//                                        AmmNo = dr["AmmNo"]?.ToString() ?? "",
//                                        AmmEffDate = Convert.ToDateTime(dr["AmmEffDate"]),
//                                        WEF = Convert.ToDateTime(dr["WEF"]),
//                                        SOClosedate = Convert.ToDateTime(dr["SOClosedate"]),
//                                        OrderType = dr["OrderType"]?.ToString() ?? "",
//                                        SOType = dr["SOType"]?.ToString() ?? "",
//                                        SOFor = dr["SOFor"]?.ToString() ?? "",
//                                        BasicAmount = Convert.ToDecimal(dr["BasicAmount"]),
//                                        NetAmount = Convert.ToDecimal(dr["NetAmount"]),
//                                        SOCanceled = Convert.ToBoolean(dr["SOCanceled"]),
//                                        SOComplete = Convert.ToBoolean(dr["SOComplete"]),
//                                        Active = Convert.ToBoolean(dr["Active"]),
//                                        ShippingAddress = dr["ShippingAddress"]?.ToString(),
//                                        SOAmendYearCode = dr.Field<int>("SOAmendYearCode"),
//                                        AmendSOSeq = dr.Field<int?>("AmendSOSeq"),
//                                        SOEntryId = dr.Field<int>("SOEntryId"),
//                                        SOYearCode = dr.Field<int>("SOYearCode")
                                      
//                                    }).ToList();
//}
//                    }
                        
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while fetching Sale Order Amendment History.", ex);
            }
            finally
            {
                oDataSet.Dispose();
            }

            return model;
        }


    }

}