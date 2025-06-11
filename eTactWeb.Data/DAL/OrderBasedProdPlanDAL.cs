using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public  class OrderBasedProdPlanDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public OrderBasedProdPlanDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        //public async Task<ResponseResult> FillSONO_OrderNO_SchNo(string FromDate, string ToDate)
        //{
        //    var _ResponseResult = new ResponseResult();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();
        //        SqlParams.Add(new SqlParameter("@flag", "FILLONCONTROL"));
        //        //SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
        //        //SqlParams.Add(new SqlParameter("@StoreId", Storeid));
        //        _ResponseResult = await _IDataLogic.ExecuteDataTable("SPSaleOrderBasedPlanAndDispatchPlanReportOnGgrid", SqlParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return _ResponseResult;
        //}
        public async Task<OrderBasedProdPlanModel> FillSONO_OrderNO_SchNo(string FromDate, string ToDate)
        {
            var result = new OrderBasedProdPlanModel();

            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SPSaleOrderBasedPlanAndDispatchPlanReportOnGgrid", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "FILLONCONTROL");
                   

                    myConnection.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // 1st result: PartCode + ItemCode
                        while (dr.Read())
                        {
                            result.PartCodes.Add(new SelectListItem
                            {
                                Text = dr["PartCode"].ToString(),
                                Value = dr["Itemcode"].ToString()
                            });
                        }

                        dr.NextResult();

                        // 2nd result: ItemName + ItemCode
                        while (dr.Read())
                        {
                            result.ItemNames.Add(new SelectListItem
                            {
                                Text = dr["Item_Name"].ToString(),
                                Value = dr["Itemcode"].ToString()
                            });
                        }

                        dr.NextResult();
                        // 3rd result: AccountName + AccountCode
                        while (dr.Read())
                        {
                            result.AccountNames.Add(new SelectListItem
                            {
                                Text = dr["Account_Name"].ToString(),
                                Value = dr["Account_Code"].ToString()
                            });
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            result.SONOs.Add(new SelectListItem
                            {
                                Text = dr["SONO"].ToString(),
                                Value = dr["SONO"].ToString()
                            });
                        }
                        dr.NextResult();
                        while (dr.Read())
                        {
                            result.CustOrderNos.Add(new SelectListItem
                            {
                                Text = dr["CustOrderNo"].ToString(),
                                Value = dr["CustOrderNo"].ToString()
                            });
                        }
                        dr.NextResult();
                        // 5rd result: AccountName + AccountCode
                        while (dr.Read())
                        {
                            result.ScheduleNos.Add(new SelectListItem
                            {
                                Text = dr["ScheduleNo"].ToString(),
                                Value = dr["ScheduleNo"].ToString()
                            });
                        }
                        

                        
                    }
                }
            }

            return result;
        }
        public async Task<OrderBasedProdPlanModel> GetOrderBasedProdPlanData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, int ItemCode)
        {
            DataSet? oDataSet = new DataSet();
            var model = new OrderBasedProdPlanModel();
           
            var _ResponseResult = new ResponseResult();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPSaleOrderBasedPlanAndDispatchPlanReportOnGgrid", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", "DISPLAYPRODUCTIONPLANREPORT");
                    oCmd.Parameters.AddWithValue("@ReportType", ReportType);
                    oCmd.Parameters.AddWithValue("@fromdatedate", fromDt);
                    oCmd.Parameters.AddWithValue("@Todate", toDt);
                    oCmd.Parameters.AddWithValue("@Partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@itemcode", ItemCode);
                    oCmd.Parameters.AddWithValue("@Accountcode", AccountCode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }

                    if (ReportType == "Item+WC wise Plan")
                    {

                        
                            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                            {
                               model.OrderBasedProdPlanList = (from DataRow row in oDataSet.Tables[0].Rows
                                                             select new OrderBasedProdPlanModel
                                                             {
                                                                 PartCode = row["PartCode"] != DBNull.Value ? Convert.ToString(row["PartCode"]) : string.Empty,
                                                                 ItemName = row["ItemName"] != DBNull.Value ? Convert.ToString(row["ItemName"]) : string.Empty,
                                                                 WCName = row["WCName"] != DBNull.Value ? Convert.ToString(row["WCName"]) : string.Empty,

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
