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
                if (ReportType == "Sale Order Summary")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                SODate = row["SODate"] != DBNull.Value ? row["SODate"].ToString() : string.Empty,
                                                                OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                SOType = row["SOType"] != DBNull.Value ? row["SOType"].ToString() : string.Empty,
                                                                SOFor = row["SOFor"] != DBNull.Value ? row["SOFor"].ToString() : string.Empty,
                                                                WEF = row["WEF"] != DBNull.Value ? row["WEF"].ToString() : string.Empty,
                                                                SOCloseDate = row["SOCloseDate"] != DBNull.Value ? row["SOCloseDate"].ToString() : string.Empty,
                                                                SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? row["SOAmmEffDate"].ToString() : string.Empty,
                                                                DeliveryAddress = row["DeliveryAddress"] != DBNull.Value ? row["DeliveryAddress"].ToString() : string.Empty,
                                                                ConsigneeName = row["ConsigneeName"] != DBNull.Value ? row["ConsigneeName"].ToString() : string.Empty,
                                                                ConsigneeAddress = row["ConsigneeAddress"] != DBNull.Value ? row["ConsigneeAddress"].ToString() : string.Empty,
                                                                OrderAmt = row["OrderAmt"] != DBNull.Value ? Convert.ToDecimal(row["OrderAmt"]) : 0,
                                                                OrderNetAmt = row["OrderNetAmt"] != DBNull.Value ? Convert.ToDecimal(row["OrderNetAmt"]) : 0,
                                                                SOConfirmDate = row["SOConfirmDate"] != DBNull.Value ? row["SOConfirmDate"].ToString() : string.Empty,
                                                                SoComplete = row["SoComplete"] != DBNull.Value ? row["SoComplete"].ToString() : string.Empty,
                                                                Approved = row["Approved"] != DBNull.Value ? row["Approved"].ToString() : string.Empty,
                                                                ApprovedDate = row["ApprovedDate"] != DBNull.Value ? row["ApprovedDate"].ToString() : string.Empty,
                                                                ApprovedByEmp = row["ApprovedByEmp"] != DBNull.Value ? row["ApprovedByEmp"].ToString() : string.Empty,
                                                                SalesPersonName = row["salesPersonName"] != DBNull.Value ? row["salesPersonName"].ToString() : string.Empty,
                                                                SalesEmailId = row["salesemailid"] != DBNull.Value ? row["salesemailid"].ToString() : string.Empty,
                                                                SalesMobileNo = row["salesmobileno"] != DBNull.Value ? row["salesmobileno"].ToString() : string.Empty,
                                                                FreightPaidBy = row["FreightPaidBy"] != DBNull.Value ? row["FreightPaidBy"].ToString() : string.Empty,
                                                                InsuApplicable = row["InsuApplicable"] != DBNull.Value ? row["InsuApplicable"].ToString() : string.Empty,
                                                                ModeTransport = row["ModeTransport"] != DBNull.Value ? row["ModeTransport"].ToString() : string.Empty,
                                                                Remark = row["Remark"] != DBNull.Value ? row["Remark"].ToString() : string.Empty,
                                                                DeActiveDate = row["DeActiveDate"] != DBNull.Value ? row["DeActiveDate"].ToString() : string.Empty,
                                                                DeActiveByEmp = row["DeActiveByEmp"] != DBNull.Value ? row["DeActiveByEmp"].ToString() : string.Empty,
                                                                EntryByMachineName = row["EntryByMachineName"] != DBNull.Value ? row["EntryByMachineName"].ToString() : string.Empty,
                                                                CustomerLocation = row["CustomerLocation"] != DBNull.Value ? row["CustomerLocation"].ToString() : string.Empty,
                                                                SOEntryID = row["SOEntryID"] != DBNull.Value ? Convert.ToInt32(row["SOEntryID"]) : 0,
                                                                SOYearCode = row["SOYearCode"] != DBNull.Value ? Convert.ToInt32(row["SOYearCode"]) : 0,

                                                            }).ToList();
                    }
                }
                else if (ReportType == "Sale Order Detail")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                SODate = row["SODate"] != DBNull.Value ? row["SODate"].ToString() : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                Qty = row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0,
                                                                PendQty = row["PendQty"] != DBNull.Value ? Convert.ToDecimal(row["PendQty"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                                                                DeliveryDate = row["DeliveryDate"] != DBNull.Value ? row["DeliveryDate"].ToString() : string.Empty,
                                                                OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                SOType = row["SOType"] != DBNull.Value ? row["SOType"].ToString() : string.Empty,
                                                                SOFor = row["SOFor"] != DBNull.Value ? row["SOFor"].ToString() : string.Empty,
                                                                WEF = row["WEF"] != DBNull.Value ? row["WEF"].ToString() : string.Empty,
                                                                SOCloseDate = row["SOCloseDate"] != DBNull.Value ? row["SOCloseDate"].ToString() : string.Empty,
                                                                SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? row["SOAmmEffDate"].ToString() : string.Empty,

                                                            }).ToList();
                    }
                }
                else if (ReportType == "Schedule Summary")
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

                else if (ReportType.ToString() == "Schedule Summary Detail")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                                SchDate = row["SchDate"] != DBNull.Value ? Convert.ToDateTime(row["SchDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                Qty = row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                                                                OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                SOType = row["SOType"] != DBNull.Value ? row["SOType"].ToString() : string.Empty,
                                                                SOFor = row["SOFor"] != DBNull.Value ? row["SOFor"].ToString() : string.Empty,
                                                                WEF = row["WEF"] != DBNull.Value ? Convert.ToDateTime(row["WEF"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOCloseDate = row["SOCloseDate"] != DBNull.Value ? Convert.ToDateTime(row["SOCloseDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? Convert.ToDateTime(row["SOAmmEffDate"]).ToString("yyyy-MM-dd") : string.Empty,


                                                            }).ToList();
                    }
                }
               
                else if (ReportType.ToString() == "Monthly Order+Schedule Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                SODate = row["SODate"] != DBNull.Value ? Convert.ToDateTime(row["SODate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                                SchDate = row["SchDate"] != DBNull.Value ? Convert.ToDateTime(row["SchDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                Qty = row["Qty"] != DBNull.Value ? Convert.ToDecimal(row["Qty"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                                                                OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                SOType = row["SOType"] != DBNull.Value ? row["SOType"].ToString() : string.Empty,
                                                                SOFor = row["SOFor"] != DBNull.Value ? row["SOFor"].ToString() : string.Empty,
                                                                WEF = row["WEF"] != DBNull.Value ? Convert.ToDateTime(row["WEF"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOCloseDate = row["SOCloseDate"] != DBNull.Value ? Convert.ToDateTime(row["SOCloseDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? Convert.ToDateTime(row["SOAmmEffDate"]).ToString("yyyy-MM-dd") : string.Empty,


                                                            }).ToList();
                    }
                }
                else if (ReportType.ToString() == "Day Wise Order+Schedule (Item Wise)")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,

                                                                Day1 = row["1"] != DBNull.Value ? Convert.ToDecimal(row["1"]) : 0,
                                                                Day2 = row["2"] != DBNull.Value ? Convert.ToDecimal(row["2"]) : 0,
                                                                Day3 = row["3"] != DBNull.Value ? Convert.ToDecimal(row["3"]) : 0,
                                                                Day4 = row["4"] != DBNull.Value ? Convert.ToDecimal(row["4"]) : 0,
                                                                Day5 = row["5"] != DBNull.Value ? Convert.ToDecimal(row["5"]) : 0,
                                                                Day6 = row["6"] != DBNull.Value ? Convert.ToDecimal(row["6"]) : 0,
                                                                Day7 = row["7"] != DBNull.Value ? Convert.ToDecimal(row["7"]) : 0,
                                                                Day8 = row["8"] != DBNull.Value ? Convert.ToDecimal(row["8"]) : 0,
                                                                Day9 = row["9"] != DBNull.Value ? Convert.ToDecimal(row["9"]) : 0,
                                                                Day10 = row["10"] != DBNull.Value ? Convert.ToDecimal(row["10"]) : 0,
                                                                Day11 = row["11"] != DBNull.Value ? Convert.ToDecimal(row["11"]) : 0,
                                                                Day12 = row["12"] != DBNull.Value ? Convert.ToDecimal(row["12"]) : 0,
                                                                Day13 = row["13"] != DBNull.Value ? Convert.ToDecimal(row["13"]) : 0,
                                                                Day14 = row["14"] != DBNull.Value ? Convert.ToDecimal(row["14"]) : 0,
                                                                Day15 = row["15"] != DBNull.Value ? Convert.ToDecimal(row["15"]) : 0,
                                                                Day16 = row["16"] != DBNull.Value ? Convert.ToDecimal(row["16"]) : 0,
                                                                Day17 = row["17"] != DBNull.Value ? Convert.ToDecimal(row["17"]) : 0,
                                                                Day18 = row["18"] != DBNull.Value ? Convert.ToDecimal(row["18"]) : 0,
                                                                Day19 = row["19"] != DBNull.Value ? Convert.ToDecimal(row["19"]) : 0,
                                                                Day20 = row["20"] != DBNull.Value ? Convert.ToDecimal(row["20"]) : 0,
                                                                Day21 = row["21"] != DBNull.Value ? Convert.ToDecimal(row["21"]) : 0,
                                                                Day22 = row["22"] != DBNull.Value ? Convert.ToDecimal(row["22"]) : 0,
                                                                Day23 = row["23"] != DBNull.Value ? Convert.ToDecimal(row["23"]) : 0,
                                                                Day24 = row["24"] != DBNull.Value ? Convert.ToDecimal(row["24"]) : 0,
                                                                Day25 = row["25"] != DBNull.Value ? Convert.ToDecimal(row["25"]) : 0,
                                                                Day26 = row["26"] != DBNull.Value ? Convert.ToDecimal(row["26"]) : 0,
                                                                Day27 = row["27"] != DBNull.Value ? Convert.ToDecimal(row["27"]) : 0,
                                                                Day28 = row["28"] != DBNull.Value ? Convert.ToDecimal(row["28"]) : 0,
                                                                Day29 = row["29"] != DBNull.Value ? Convert.ToDecimal(row["29"]) : 0,
                                                                Day30 = row["30"] != DBNull.Value ? Convert.ToDecimal(row["30"]) : 0,
                                                                Day31 = row["31"] != DBNull.Value ? Convert.ToDecimal(row["31"]) : 0,


                                                            }).ToList();
                    }
                }
               
                else if (ReportType.ToString() == "Monthly Order+Schedule+Pending Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,

                                                                Day1 = row["1"] != DBNull.Value ? Convert.ToDecimal(row["1"]) : 0,
                                                                Day2 = row["2"] != DBNull.Value ? Convert.ToDecimal(row["2"]) : 0,
                                                                Day3 = row["3"] != DBNull.Value ? Convert.ToDecimal(row["3"]) : 0,
                                                                Day4 = row["4"] != DBNull.Value ? Convert.ToDecimal(row["4"]) : 0,
                                                                Day5 = row["5"] != DBNull.Value ? Convert.ToDecimal(row["5"]) : 0,
                                                                Day6 = row["6"] != DBNull.Value ? Convert.ToDecimal(row["6"]) : 0,
                                                                Day7 = row["7"] != DBNull.Value ? Convert.ToDecimal(row["7"]) : 0,
                                                                Day8 = row["8"] != DBNull.Value ? Convert.ToDecimal(row["8"]) : 0,
                                                                Day9 = row["9"] != DBNull.Value ? Convert.ToDecimal(row["9"]) : 0,
                                                                Day10 = row["10"] != DBNull.Value ? Convert.ToDecimal(row["10"]) : 0,
                                                                Day11 = row["11"] != DBNull.Value ? Convert.ToDecimal(row["11"]) : 0,
                                                                Day12 = row["12"] != DBNull.Value ? Convert.ToDecimal(row["12"]) : 0,
                                                                Day13 = row["13"] != DBNull.Value ? Convert.ToDecimal(row["13"]) : 0,
                                                                Day14 = row["14"] != DBNull.Value ? Convert.ToDecimal(row["14"]) : 0,
                                                                Day15 = row["15"] != DBNull.Value ? Convert.ToDecimal(row["15"]) : 0,
                                                                Day16 = row["16"] != DBNull.Value ? Convert.ToDecimal(row["16"]) : 0,
                                                                Day17 = row["17"] != DBNull.Value ? Convert.ToDecimal(row["17"]) : 0,
                                                                Day18 = row["18"] != DBNull.Value ? Convert.ToDecimal(row["18"]) : 0,
                                                                Day19 = row["19"] != DBNull.Value ? Convert.ToDecimal(row["19"]) : 0,
                                                                Day20 = row["20"] != DBNull.Value ? Convert.ToDecimal(row["20"]) : 0,
                                                                Day21 = row["21"] != DBNull.Value ? Convert.ToDecimal(row["21"]) : 0,
                                                                Day22 = row["22"] != DBNull.Value ? Convert.ToDecimal(row["22"]) : 0,
                                                                Day23 = row["23"] != DBNull.Value ? Convert.ToDecimal(row["23"]) : 0,
                                                                Day24 = row["24"] != DBNull.Value ? Convert.ToDecimal(row["24"]) : 0,
                                                                Day25 = row["25"] != DBNull.Value ? Convert.ToDecimal(row["25"]) : 0,
                                                                Day26 = row["26"] != DBNull.Value ? Convert.ToDecimal(row["26"]) : 0,
                                                                Day27 = row["27"] != DBNull.Value ? Convert.ToDecimal(row["27"]) : 0,
                                                                Day28 = row["28"] != DBNull.Value ? Convert.ToDecimal(row["28"]) : 0,
                                                                Day29 = row["29"] != DBNull.Value ? Convert.ToDecimal(row["29"]) : 0,
                                                                Day30 = row["30"] != DBNull.Value ? Convert.ToDecimal(row["30"]) : 0,
                                                                Day31 = row["31"] != DBNull.Value ? Convert.ToDecimal(row["31"]) : 0,


                                                            }).ToList();
                    }
                }
                else if (ReportType.ToString() == "Day Wise Order+Schedule (Item + Customer Wise)")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,

                                                                Day1 = row["1"] != DBNull.Value ? Convert.ToDecimal(row["1"]) : 0,
                                                                Day2 = row["2"] != DBNull.Value ? Convert.ToDecimal(row["2"]) : 0,
                                                                Day3 = row["3"] != DBNull.Value ? Convert.ToDecimal(row["3"]) : 0,
                                                                Day4 = row["4"] != DBNull.Value ? Convert.ToDecimal(row["4"]) : 0,
                                                                Day5 = row["5"] != DBNull.Value ? Convert.ToDecimal(row["5"]) : 0,
                                                                Day6 = row["6"] != DBNull.Value ? Convert.ToDecimal(row["6"]) : 0,
                                                                Day7 = row["7"] != DBNull.Value ? Convert.ToDecimal(row["7"]) : 0,
                                                                Day8 = row["8"] != DBNull.Value ? Convert.ToDecimal(row["8"]) : 0,
                                                                Day9 = row["9"] != DBNull.Value ? Convert.ToDecimal(row["9"]) : 0,
                                                                Day10 = row["10"] != DBNull.Value ? Convert.ToDecimal(row["10"]) : 0,
                                                                Day11 = row["11"] != DBNull.Value ? Convert.ToDecimal(row["11"]) : 0,
                                                                Day12 = row["12"] != DBNull.Value ? Convert.ToDecimal(row["12"]) : 0,
                                                                Day13 = row["13"] != DBNull.Value ? Convert.ToDecimal(row["13"]) : 0,
                                                                Day14 = row["14"] != DBNull.Value ? Convert.ToDecimal(row["14"]) : 0,
                                                                Day15 = row["15"] != DBNull.Value ? Convert.ToDecimal(row["15"]) : 0,
                                                                Day16 = row["16"] != DBNull.Value ? Convert.ToDecimal(row["16"]) : 0,
                                                                Day17 = row["17"] != DBNull.Value ? Convert.ToDecimal(row["17"]) : 0,
                                                                Day18 = row["18"] != DBNull.Value ? Convert.ToDecimal(row["18"]) : 0,
                                                                Day19 = row["19"] != DBNull.Value ? Convert.ToDecimal(row["19"]) : 0,
                                                                Day20 = row["20"] != DBNull.Value ? Convert.ToDecimal(row["20"]) : 0,
                                                                Day21 = row["21"] != DBNull.Value ? Convert.ToDecimal(row["21"]) : 0,
                                                                Day22 = row["22"] != DBNull.Value ? Convert.ToDecimal(row["22"]) : 0,
                                                                Day23 = row["23"] != DBNull.Value ? Convert.ToDecimal(row["23"]) : 0,
                                                                Day24 = row["24"] != DBNull.Value ? Convert.ToDecimal(row["24"]) : 0,
                                                                Day25 = row["25"] != DBNull.Value ? Convert.ToDecimal(row["25"]) : 0,
                                                                Day26 = row["26"] != DBNull.Value ? Convert.ToDecimal(row["26"]) : 0,
                                                                Day27 = row["27"] != DBNull.Value ? Convert.ToDecimal(row["27"]) : 0,
                                                                Day28 = row["28"] != DBNull.Value ? Convert.ToDecimal(row["28"]) : 0,
                                                                Day29 = row["29"] != DBNull.Value ? Convert.ToDecimal(row["29"]) : 0,
                                                                Day30 = row["30"] != DBNull.Value ? Convert.ToDecimal(row["30"]) : 0,
                                                                Day31 = row["31"] != DBNull.Value ? Convert.ToDecimal(row["31"]) : 0,

                                                            }).ToList();
                    }
                }

               else if (ReportType.ToString() == "Monthly Order+Schedule+Pending Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.saleOrderRegisterGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new SaleOrderRegisterModel
                                                            {
                                                                CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                                                                SONo = row["SONo"] != DBNull.Value ? row["SONo"].ToString() : string.Empty,
                                                                CustOrderNo = row["CustOrderNo"] != DBNull.Value ? row["CustOrderNo"].ToString() : string.Empty,
                                                                SODate = row["SODate"] != DBNull.Value ? Convert.ToDateTime(row["SODate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                                SchDate = row["SchDate"] != DBNull.Value ? Convert.ToDateTime(row["SchDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                OrderQty = row["OrderQty"] != DBNull.Value ? Convert.ToDecimal(row["OrderQty"]) : 0,
                                                                BillQty = row["BillQty"] != DBNull.Value ? Convert.ToDecimal(row["BillQty"]) : 0,
                                                                PendQty = row["PendQty"] != DBNull.Value ? Convert.ToDecimal(row["PendQty"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                                                                OrderType = row["OrderType"] != DBNull.Value ? row["OrderType"].ToString() : string.Empty,
                                                                WEF = row["WEF"] != DBNull.Value ? Convert.ToDateTime(row["WEF"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOCloseDate = row["SOCloseDate"] != DBNull.Value ? Convert.ToDateTime(row["SOCloseDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                                SOAmmNo = row["SOAmmNo"] != DBNull.Value ? Convert.ToInt32(row["SOAmmNo"]) : 0,
                                                                SOAmmEffDate = row["SOAmmEffDate"] != DBNull.Value ? Convert.ToDateTime(row["SOAmmEffDate"]).ToString("yyyy-MM-dd") : string.Empty


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
