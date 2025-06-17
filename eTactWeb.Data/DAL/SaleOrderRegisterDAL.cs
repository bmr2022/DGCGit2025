using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public  class SaleOrderRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public SaleOrderRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }

        public async Task<ResponseResult> FillPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillPartcode"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleOrderNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillSaleorder"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustOrderNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillcustomerOrder"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
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
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillScheduleNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillCustomer"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSalesPerson(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillData"));
                SqlParams.Add(new SqlParameter("@ReportType", "FillSalesPerson"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpReportSaleorderSaleSchOnGrid", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<SaleOrderRegisterModel> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate)
        {
            var resultList = new SaleOrderRegisterModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SpReportSaleorderSaleSchOnGrid", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    command.Parameters.AddWithValue("@flag", "ReportDisplay");
                    command.Parameters.AddWithValue("@FromDate",fromDt);
                    command.Parameters.AddWithValue("@ToDate", toDt);
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@SaleorderOrSchData", OrderSchedule);
                    //command.Parameters.AddWithValue("@WCID", WorkCenterid);
                    // command.Parameters.AddWithValue("@RMItemcode", RMItemCode);


                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "Schedule Summary")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new SaleOrderRegisterModel
                                                               {
                                                                   CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                   SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                                   SchDate = row["SchDate"] != DBNull.Value ? Convert.ToDateTime(row["SchDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SchEffectFrom = row["SchEffectFrom"] != DBNull.Value ? Convert.ToDateTime(row["SchEffectFrom"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SchEffTill = row["SchEffTill"] != DBNull.Value ? Convert.ToDateTime(row["SchEffTill"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                   CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                   SODate = row["SODate"] != DBNull.Value ? Convert.ToDateTime(row["SODate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                   TentConfirm = row["TentConfirm"] != DBNull.Value ? row["TentConfirm"].ToString() : string.Empty,
                                                                   SchCompleted = row["SchCompleted"] != DBNull.Value ? row["SchCompleted"].ToString() : string.Empty,
                                                                   DeliveryAddress = row["DeliveryAddress"] != DBNull.Value ? row["DeliveryAddress"].ToString() : string.Empty,
                                                                   ConsigneeName = row["ConsigneeName"] != DBNull.Value ? row["ConsigneeName"].ToString() : string.Empty,
                                                                   ConsigneeAddress = row["ConsigneeAddress"] != DBNull.Value ? row["ConsigneeAddress"].ToString() : string.Empty,
                                                                   OrderAmt = row["OrderAmt"] != DBNull.Value ? Convert.ToDecimal(row["OrderAmt"]) : 0,
                                                                   OrderNetAmt = row["OrderNetAmt"] != DBNull.Value ? Convert.ToDecimal(row["OrderNetAmt"]) : 0,
                                                                   SchAmndNo = row["SchAmndNo"] != DBNull.Value ? Convert.ToInt64(row["SchAmndNo"]) : 0,
                                                                   WEF = row["WEF"] != DBNull.Value ? Convert.ToDateTime(row["WEF"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SOCloseDate = row["SOCloseDate"] != DBNull.Value ? Convert.ToDateTime(row["SOCloseDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                   SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? Convert.ToDateTime(row["SOAmmEffDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                   SalesPersonName = row["SalesPersonName"] != DBNull.Value ? row["SalesPersonName"].ToString() : string.Empty,
                                                                   SalesEmailId = row["SalesEmailId"] != DBNull.Value ? row["SalesEmailId"].ToString() : string.Empty,
                                                                   SalesMobileNo = row["SalesMobileNo"] != DBNull.Value ? row["SalesMobileNo"].ToString() : string.Empty,
                                                                   ApprovedByEmp = row["ApprovedByEmp"] != DBNull.Value ? row["ApprovedByEmp"].ToString() : string.Empty,
                                                                   ModeTransport = row["ModeTransport"] != DBNull.Value ? row["ModeTransport"].ToString() : string.Empty,
                                                                   Remark = row["Remark"] != DBNull.Value ? row["Remark"].ToString() : string.Empty,
                                                                   EntryByMachineName = row["EntryByMachineName"] != DBNull.Value ? row["EntryByMachineName"].ToString() : string.Empty,
                                                                   CustomerLocation = row["CustomerLocation"] != DBNull.Value ? row["CustomerLocation"].ToString() : string.Empty

                                                               }).ToList();
                    }
                }
                else if (ReportType.ToString() == "Sale Order Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new SaleOrderRegisterModel
                                                               {
                                                                   

                                                               }).ToList();
                    }
                }


            }
            catch (Exception ex)
            {
                // Handle exception (log it or rethrow)
                throw new Exception("Error fetching BOM tree data.", ex);
            }

            return resultList;
        }
    }
}
