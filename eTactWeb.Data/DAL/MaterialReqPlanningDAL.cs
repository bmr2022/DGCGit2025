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
    public class MaterialReqPlanningDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;

        private IDataReader? Reader;

        public MaterialReqPlanningDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetMRPNo(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "MRPNO"));
                SqlParams.Add(new SqlParameter("@YEaR_CODE", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<ResponseResult> GetMonthList(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "MONTHLIST"));
                SqlParams.Add(new SqlParameter("@year_code", YearCode));
              



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
         public async Task<ResponseResult> GetPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
         public async Task<ResponseResult> GetItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItem"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }


        public async Task<MaterialReqPlanningModel> GetDetailData(string ReportType,string mrpno, string Month, int YearCode, string FromDate, string ToDate)
        {
            var resultList = new MaterialReqPlanningModel();
            DataSet oDataSet = new DataSet();
            //oDataSet.Tables[0].TableName = "PurchaseScheduleMain";
            //oDataSet.Tables[1].TableName = "PurchaseScheduleDetail";
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {

                    SqlCommand command = new SqlCommand("SPMRPREport", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 300
                    };

                    command.Parameters.AddWithValue("@flag", ReportType);
                    command.Parameters.Add(new SqlParameter("@mrpno", mrpno));
                    command.Parameters.Add(new SqlParameter("@months", Month));
                    command.Parameters.AddWithValue("@year_code", YearCode);
                    command.Parameters.AddWithValue("@FromDate", FromDate);
                    command.Parameters.AddWithValue("@ToDate", ToDate);
                   

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (ReportType == "DAYWISEMRPDATA")
                    {
                        resultList.DayWiseMRPDataGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new DayWiseMRPData
                                                               {
                                                                   Party_Name = row["Party Name"] == DBNull.Value ? string.Empty : row["Party Name"].ToString(),
                                                                   Part_code = row["Part code"] == DBNull.Value ? string.Empty : row["Part code"].ToString(),
                                                                   Item_Name = row["Item Name"] == DBNull.Value ? string.Empty : row["Item Name"].ToString(),
                                                                   Req_Qty = row["Req Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Req Qty"]),
                                                                   Current_Stock = row["Current Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Current Stock"]),
                                                                   WIP_Qty = row["WIP Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["WIP Qty"]),
                                                                   Order_Qty = row["Order Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Order Qty"]),
                                                                   d1 = row["1"] == DBNull.Value ? 0 : Convert.ToDecimal(row["1"]),
                                                                   d2 = row["2"] == DBNull.Value ? 0 : Convert.ToDecimal(row["2"]),
                                                                   d3 = row["3"] == DBNull.Value ? 0 : Convert.ToDecimal(row["3"]),
                                                                   d4 = row["4"] == DBNull.Value ? 0 : Convert.ToDecimal(row["4"]),
                                                                   d5 = row["5"] == DBNull.Value ? 0 : Convert.ToDecimal(row["5"]),
                                                                   d6 = row["6"] == DBNull.Value ? 0 : Convert.ToDecimal(row["6"]),
                                                                   d7 = row["7"] == DBNull.Value ? 0 : Convert.ToDecimal(row["7"]),
                                                                   d8 = row["8"] == DBNull.Value ? 0 : Convert.ToDecimal(row["8"]),
                                                                   d9 = row["9"] == DBNull.Value ? 0 : Convert.ToDecimal(row["9"]),
                                                                   d10 = row["10"] == DBNull.Value ? 0 : Convert.ToDecimal(row["10"]),
                                                                   d11 = row["11"] == DBNull.Value ? 0 : Convert.ToDecimal(row["11"]),
                                                                   d12 = row["12"] == DBNull.Value ? 0 : Convert.ToDecimal(row["12"]),
                                                                   d13 = row["13"] == DBNull.Value ? 0 : Convert.ToDecimal(row["13"]),
                                                                   d14 = row["14"] == DBNull.Value ? 0 : Convert.ToDecimal(row["14"]),
                                                                   d15 = row["15"] == DBNull.Value ? 0 : Convert.ToDecimal(row["15"]),
                                                                   d16 = row["16"] == DBNull.Value ? 0 : Convert.ToDecimal(row["16"]),
                                                                   d17 = row["17"] == DBNull.Value ? 0 : Convert.ToDecimal(row["17"]),
                                                                   d18 = row["18"] == DBNull.Value ? 0 : Convert.ToDecimal(row["18"]),
                                                                   d19 = row["19"] == DBNull.Value ? 0 : Convert.ToDecimal(row["19"]),
                                                                   d20 = row["20"] == DBNull.Value ? 0 : Convert.ToDecimal(row["20"]),
                                                                   d21 = row["21"] == DBNull.Value ? 0 : Convert.ToDecimal(row["21"]),
                                                                   d22 = row["22"] == DBNull.Value ? 0 : Convert.ToDecimal(row["22"]),
                                                                   d23 = row["23"] == DBNull.Value ? 0 : Convert.ToDecimal(row["23"]),
                                                                   d24 = row["24"] == DBNull.Value ? 0 : Convert.ToDecimal(row["24"]),
                                                                   d25 = row["25"] == DBNull.Value ? 0 : Convert.ToDecimal(row["25"]),
                                                                   d26 = row["26"] == DBNull.Value ? 0 : Convert.ToDecimal(row["26"]),
                                                                   d27 = row["27"] == DBNull.Value ? 0 : Convert.ToDecimal(row["27"]),
                                                                   d28 = row["28"] == DBNull.Value ? 0 : Convert.ToDecimal(row["28"]),
                                                                   d29 = row["29"] == DBNull.Value ? 0 : Convert.ToDecimal(row["29"]),
                                                                   d30 = row["30"] == DBNull.Value ? 0 : Convert.ToDecimal(row["30"]),
                                                                   d31 = row["31"] == DBNull.Value ? 0 : Convert.ToDecimal(row["31"]),
                                                                   Total_Qty = row["Total Qty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Total Qty"]),
                                                                   Year_Code = row["Year Code"] == DBNull.Value ? 0 : Convert.ToInt32(row["Year Code"]),
                                                                   item_code = row["item_code"] == DBNull.Value ? 0 : Convert.ToInt32(row["item_code"]),
                                                                   Sch_No = row["Sch No"] == DBNull.Value ? string.Empty : row["Sch No"].ToString(),
                                                                   MRP_No = row["MRP No"] == DBNull.Value ? string.Empty : row["MRP No"].ToString(),



                                                               }).ToList();

                    }
                    else if (ReportType == "MRPCONSOLIDATED")
                    {
                        resultList.DayWiseMRPDataGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new DayWiseMRPData
                                                               {
                                                                   Part_code = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                                   Item_Name = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                                   Req_Qty = row["MRPReqQty"] == DBNull.Value ? 0: Convert.ToDecimal(row["MRPReqQty"]),
                                                                   Current_Stock = row["CurrentStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrentStock"]),
                                                                   WIPStock = row["MRPOrderQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MRPOrderQty"]),
                                                                   Order_Qty = row["WIPStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["WIPStock"]),
                                                                   SChOrderQty = row["SChOrderQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SChOrderQty"]),
                                                                  
                                                                   MRP_No = row["MRPNO"] == DBNull.Value ? string.Empty : row["MRPNO"].ToString(),

                                                                   item_code = row["item_code"] == DBNull.Value ? 0 : Convert.ToInt32(row["item_code"]),
                                                                  
                                                               }).ToList();
                    
                    }
                    else if (ReportType == "MRPDataonly")
                    {
                        resultList.DayWiseMRPDataGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new DayWiseMRPData
                                                               {
                                                                   Part_code = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                                   Item_Name = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),
                                                                   Req_Qty = row["ReqQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ReqQty"]),
                                                                   StoreStock = row["StoreStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["StoreStock"]),
                                                                   WIPStock = row["WIPStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["WIPStock"]),
                                                                   TotalStock = row["TotalStock"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalStock"]),
                                                                   MinLevel = row["MinLevel"] == DBNull.Value ? 0 : Convert.ToDecimal(row["MinLevel"]),
                                                                   OrderQty = row["OrderQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OrderQty"]),
                                                                   unit = row["unit"] == DBNull.Value ? string.Empty : row["unit"].ToString(),

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
