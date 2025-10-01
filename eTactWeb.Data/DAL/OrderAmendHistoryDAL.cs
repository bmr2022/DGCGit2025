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
    public class OrderAmendHistoryDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public OrderAmendHistoryDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> FillPONO(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
            //    SqlParams.Add(new SqlParameter("@flag", "POAmendmentHistoryReport"));
                SqlParams.Add(new SqlParameter("@reportType", ""));
                SqlParams.Add(new SqlParameter("@Dashboardflag", "FillPONO"));
                SqlParams.Add(new SqlParameter("@FromDate", CommonFunc.ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate", CommonFunc.ParseFormattedDate(ToDate)));
                //SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                //SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillVendorName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
              //  SqlParams.Add(new SqlParameter("@flag", "POAmendmentHistoryReport"));
            
                SqlParams.Add(new SqlParameter("@Dashboardflag", "FillPOVendorName"));
                SqlParams.Add(new SqlParameter("@FromDate",CommonFunc.ParseFormattedDate( FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate",CommonFunc.ParseFormattedDate( ToDate)));
                //SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                //SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
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
               // SqlParams.Add(new SqlParameter("@flag", "POAmendmentHistoryReport"));
                SqlParams.Add(new SqlParameter("@reportType", ""));
                SqlParams.Add(new SqlParameter("@Dashboardflag", "FillPOPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", CommonFunc.ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate", CommonFunc.ParseFormattedDate(ToDate)));
                //SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                //SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
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
            //    SqlParams.Add(new SqlParameter("@flag", "POAmendmentHistoryReport"));
                SqlParams.Add(new SqlParameter("@reportType", ""));
                SqlParams.Add(new SqlParameter("@Dashboardflag", "FillPOPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", CommonFunc.ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate",CommonFunc.ParseFormattedDate(  ToDate)));
                //SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                //SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportOrderAmendHistoryOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
         }

        public async Task<OrderAmendHistoryModel> GetOrderAmendHistoryData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, string PONO, int ItemCode,string HistoryReportMode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new OrderAmendHistoryModel();
            var _PODetail = new List<OrderAmendHistoryModel>();
            var _ResponseResult = new ResponseResult();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpReportOrderAmendHistoryOnGrid", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@flag", HistoryReportMode);
                    oCmd.Parameters.AddWithValue("@reportType", ReportType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@Accountcode", AccountCode);
                    oCmd.Parameters.AddWithValue("@POno", PONO );
                    //oCmd.Parameters.AddWithValue("@partCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemCode", ItemCode );
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }

                    if (HistoryReportMode == "POAmendmentHistoryReport")
                    {

                        if (ReportType == "POSummary")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var poDetail = CommonFunc.DataRowToClass<OrderAmendHistoryModel>(row);
                                    _PODetail.Add(poDetail);
                                }
                                model.OrderAmendHistoryGrid = _PODetail;
                            }
                        }

                        if (ReportType == "PODetail") 
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var poDetail = CommonFunc.DataRowToClass<OrderAmendHistoryModel>(row);
                                    _PODetail.Add(poDetail);
                                }
                                model.OrderAmendHistoryGrid = _PODetail;
                            }
                        }
                    }
                    else if (HistoryReportMode == "POSCHAMENDMENTHISTORY")
                    {

                        if (ReportType == "POSummary")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var poDetail = CommonFunc.DataRowToClass<OrderAmendHistoryModel>(row);
                                    _PODetail.Add(poDetail);
                                }
                                model.OrderAmendHistoryGrid = _PODetail;
                            }
                        }

                        if (ReportType == "PODetail")
                        {
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in oDataSet.Tables[0].Rows)
                                {
                                    var poDetail = CommonFunc.DataRowToClass<OrderAmendHistoryModel>(row);
                                    _PODetail.Add(poDetail);
                                }
                                model.OrderAmendHistoryGrid = _PODetail;
                            }
                        }
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
