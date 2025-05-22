using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
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
    public class BOMReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public BOMReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetBOMTree()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BOMTREE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDirectBOMTree()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DirectBOM"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBOMStockTree()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BOMSTOCK"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> FillFinishPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFINISHPARTCODE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFinishItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFINISHITEMNAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMTEMNAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> FillRMPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMPARTCODE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenterName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLWorkCenterName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSTORENAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPBOMReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<BOMReportModel> GetBomTreeDetailsData(string fromDate, string toDate, int Yearcode, string ReportType, string FGPartCode, string RMPartCode, int Storeid,float calculateQty)
        {
            var resultList = new BOMReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                var currentDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPBOMReport", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType == "BOMSTOCK"|| ReportType == "BOMSTOCK(WITH SUB BOM)"|| ReportType == "BOMSTOCK(DIRECT CHILD)")
                    {
                        command.Parameters.AddWithValue("@Flag", ReportType);
                        command.Parameters.AddWithValue("@CurrentDate", currentDt);
                        command.Parameters.AddWithValue("@Storeid", Storeid);
                        command.Parameters.AddWithValue("@Yearcode", Yearcode);
                        command.Parameters.AddWithValue("@CalForQty", calculateQty);
                        command.Parameters.AddWithValue("@FGPartcode", FGPartCode ?? string.Empty);
                        command.Parameters.AddWithValue("@RMpartcode", RMPartCode ?? string.Empty);
                    }
                    else
                    {


                        // Add SQL parameters
                        command.Parameters.AddWithValue("@Flag", ReportType);
                        //command.Parameters.AddWithValue("@FGItemcode", fgItemcode);
                        //command.Parameters.AddWithValue("@RMItemcode", rmItemcode);
                        //command.Parameters.AddWithValue("@FGName", FGName ?? string.Empty);
                        command.Parameters.AddWithValue("@FGPartcode", FGPartCode ?? string.Empty);
                        //command.Parameters.AddWithValue("@RMName", RMName ?? string.Empty);
                        command.Parameters.AddWithValue("@RMpartcode", RMPartCode ?? string.Empty);
                        //command.Parameters.AddWithValue("@RMpartcode", RMPartCode ?? string.Empty);
                        //command.Parameters.AddWithValue("@RMpartcode", RMPartCode ?? string.Empty);
                    }

                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType.ToString().ToUpper() == "BOMTREE")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.BOMReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new BOMReportModel
                                                    {
                                                        MainPartCode = row["Main PartCode"].ToString(),
                                                        MainItemName = row["Main ItemName"].ToString(),
                                                        FGPartCode = row["FGPartCode"].ToString(),
                                                        FGItemName = row["FGItemName"].ToString(),
                                                        RMPartCode = row["RMPartCode"].ToString(),
                                                        RMItemName = row["RMItemName"].ToString(),
                                                        NetReqQty = Convert.ToDecimal(row["NetReqQty"]),
                                                        RMQty = Convert.ToDecimal(row["RMQty"]),
                                                        BomRevNo = Convert.ToInt32(row["BomRevNo"].ToString()),
                                                        ItemLvl = Convert.ToInt32(row["ItemLvl"]),
                                                        BOMItemCode = Convert.ToInt32(row["BOMITEMCODE"].ToString()),
                                                        FGItemCode = Convert.ToInt32(row["Item_Code"].ToString()),
                                                        RMItemCode = Convert.ToInt32(row["Item_Code"].ToString()),
                                                        SubBOM = row["SubBOM"].ToString()
                                                    }).ToList();
                    }
                }
                else if (ReportType.ToString() == "DirectBOM")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.BOMReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new BOMReportModel
                                                    {
                                                        //FGPartCode = row["FGPartCode"].ToString(),
                                                        //FGItemName = row["FGItemName"].ToString(),
                                                        //RMPartCode = row["RMPartCode"].ToString(),
                                                        //RMItemName = row["RMItemName"].ToString(),
                                                        //BomRevNo = Convert.ToInt32(row["BomRevNo"].ToString()),
                                                        NetReqQty = Convert.ToDecimal(row["NetReqQty"]),
                                                        //RMQty = Convert.ToInt32(row["RMQty"]),
                                                        //FGItemCode = Convert.ToInt32(row["FgItemCode"]),
                                                        //RMItemCode = Convert.ToInt32(row["RMItemCode"])
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        //ItemLvl = Convert.ToInt32(row["ItemLvl"]),
                                                        BomRevNo = row["BomRevNo"] != DBNull.Value ? Convert.ToInt32(row["BomRevNo"]) : 0,
                                                        FGItemCode = Convert.ToInt32(row["FGItemCode"].ToString()),
                                                        RMItemCode = Convert.ToInt32(row["RMItemCode"].ToString()),
                                                        // FgItemCode = row["FgItemCode"] != DBNull.Value ? Convert.ToInt32(row["FgItemCode"]) : 0,
                                                        // RMItemCode = row["RMItemCode"] != DBNull.Value ? Convert.ToInt32(row["RMItemCode"]) : 0
                                                    }).ToList();
                    }
                }
                else if (ReportType.ToString() == "BOMSTOCK")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.BOMReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new BOMReportModel
                                                    {
                                                        BOMItemName = row["BOMItemName"] != DBNull.Value ? row["BOMItemName"].ToString() : string.Empty,
                                                        BOMPartCode = row["BOMPartCode"] != DBNull.Value ? row["BOMPartCode"].ToString() : string.Empty,
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        FGUnit = row["FGUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMUnit = row["RMUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        NetReqQty = row["NetReqQty"] != DBNull.Value ? Convert.ToDecimal(row["NetReqQty"]) : 0,
                                                        //RMQty = Convert.ToInt32(row["Qty"]),
                                                        SubBOM = row["SubBOM"]?.ToString(),
                                                        BomRevNo = row["BomRevNo"] != DBNull.Value ? Convert.ToInt32(row["BomRevNo"]) : 0,
                                                       // StoreId = row["StoreId"] != DBNull.Value ? Convert.ToInt32(row["StoreId"]) : 0,
                                                       //WCID = row["WCID"] != DBNull.Value ? Convert.ToInt32(row["WCID"]) : 0,
                                                        //Yearcode = row["Yearcode"] != DBNull.Value ? Convert.ToInt32(row["Yearcode"]) : 0,
                                                        StoreStock = row["StoreStock"] != DBNull.Value ? Convert.ToDouble(row["StoreStock"]) : 0.0,
                                                        WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToDouble(row["WIPStock"]) : 0.0,
                                                        ForTheStore = row["ForTheStore"]?.ToString(),
                                                        ForWorkCeneter = row["ForWorkCeneter"]?.ToString()
                                                    }).ToList();
                    }
                }
                 else if (ReportType=="BOMSTOCK(WITH SUB BOM)")
                 {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.BOMReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new BOMReportModel
                                                    {
                                                        BOMItemName = row["BOMItemName"] != DBNull.Value ? row["BOMItemName"].ToString() : string.Empty,
                                                        BOMPartCode = row["BOMPartCode"] != DBNull.Value ? row["BOMPartCode"].ToString() : string.Empty,
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        FGUnit = row["FGUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMUnit = row["RMUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        NetReqQty = row["NetReqQty"] != DBNull.Value ? Convert.ToDecimal(row["NetReqQty"]) : 0,
                                                        SubBOM = row["SubBOM"]?.ToString(),
                                                        BomRevNo = row["BomRevNo"] != DBNull.Value ? Convert.ToInt32(row["BomRevNo"]) : 0,
                                                        StoreStock = row["StoreStock"] != DBNull.Value ? Convert.ToDouble(row["StoreStock"]) : 0.0,
                                                        WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToDouble(row["WIPStock"]) : 0.0,
                                                        Rate = row["Rate"] != DBNull.Value ? Convert.ToDouble(row["Rate"]) : 0.0,
                                                        Amount = row["Amount"] != DBNull.Value ? Convert.ToDouble(row["Amount"]) : 0.0,
                                                        TotalReqQty = row["TotalReqQty"] != DBNull.Value ? Convert.ToDouble(row["TotalReqQty"]) : 0.0,
                                                        ShortExcess = row["ShortExcess"] != DBNull.Value ? Convert.ToDouble(row["ShortExcess"]) : 0.0,
                                                        ForTheStore = row["ForTheStore"]?.ToString(),
                                                        ForWorkCeneter = row["ForWorkCeneter"]?.ToString()
                                                    }).ToList();
                    }
                 }
                 else if (ReportType== "BOMSTOCK(DIRECT CHILD)")
                 {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.BOMReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new BOMReportModel
                                                    {
                                                       
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        FGUnit = row["FGUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMUnit = row["RMUnit"] != DBNull.Value ? row["FGUnit"].ToString() : string.Empty,
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        StoreStock = row["StoreStock"] != DBNull.Value ? Convert.ToDouble(row["StoreStock"]) : 0.0,
                                                        WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToDouble(row["WIPStock"]) : 0.0,
                                                        TotalStock = row["TotalStock"] != DBNull.Value ? Convert.ToDouble(row["TotalStock"]) : 0.0,
                                                        Rate = row["Rate"] != DBNull.Value ? Convert.ToDouble(row["Rate"]) : 0.0,
                                                        Amount = row["Amount"] != DBNull.Value ? Convert.ToDouble(row["Amount"]) : 0.0,
                                                        ReqQty = row["ReqQty"] != DBNull.Value ? Convert.ToDouble(row["ReqQty"]) : 0.0,
                                                        TotalReqQty = row["TotalReqQty"] != DBNull.Value ? Convert.ToDouble(row["TotalReqQty"]) : 0.0,
                                                        ShortExcess = row["ShortExcess"] != DBNull.Value ? Convert.ToDouble(row["ShortExcess"]) : 0.0,
                                                        BomRevNo = row["BomRevNo"] != DBNull.Value ? Convert.ToInt32(row["BomRevNo"]) : 0,
                                                        ForTheStore = row["ForTheStore"]?.ToString(),
                                                        ForWorkCeneter = row["ForWorkCeneter"]?.ToString()
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
