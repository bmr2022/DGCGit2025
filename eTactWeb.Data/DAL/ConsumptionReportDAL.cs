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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class ConsumptionReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ConsumptionReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> FillFGItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FGtemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;  
            }

            return _ResponseResult;
        }
         public async Task<ResponseResult> FillFGPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FGPARTCODE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "RMItemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "RMPARTCODE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FillWorkcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPRMConsumptionReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ConsumptionReportModel> GetConsumptionDetailsData(string fromDate, string toDate, int WorkCenterid, string ReportType, int FGItemCode, int RMItemCode, int Storeid)
        {
            var resultList = new ConsumptionReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPRMConsumptionReport", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    var fromDtt = CommonFunc.ParseFormattedDate(fromDate);
                    var toDtt = CommonFunc.ParseFormattedDate(toDate);

                    command.Parameters.AddWithValue("@flag", ReportType);
                        command.Parameters.AddWithValue("@fromdate", fromDtt);
                        command.Parameters.AddWithValue("@todate", toDtt);
                        command.Parameters.AddWithValue("@Storeid", Storeid);
                        command.Parameters.AddWithValue("@WCID", WorkCenterid);
                        command.Parameters.AddWithValue("@FGItemcode", FGItemCode );
                        command.Parameters.AddWithValue("@RMItemcode", RMItemCode );
                    

                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "ProductionConsumptionReport(SUMMARY)")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.ConsumptionReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new ConsumptionReportModel
                                                    {
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        OpeningQty = row["OpeningQty"] != DBNull.Value ? Convert.ToDecimal(row["OpeningQty"]) : 0,
                                                        OpeningWIP = row["OpeningWIP"] != DBNull.Value ? Convert.ToDecimal(row["OpeningWIP"]) : 0,
                                                        Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                        POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                        AltPOQty = row["AltPOQty"] != DBNull.Value ? Convert.ToDecimal(row["AltPOQty"]) : 0,
                                                        AltUnit = row["AltUnit"] != DBNull.Value ? row["AltUnit"].ToString() : string.Empty,
                                                        RecFromMRN = row["RecFromMRN"] != DBNull.Value ? Convert.ToDecimal(row["RecFromMRN"]) : 0,
                                                        RecFromJW = row["RecFromJW"] != DBNull.Value ? Convert.ToDecimal(row["RecFromJW"]) : 0,
                                                        RecMaterialConv = row["RecMaterialConv"] != DBNull.Value ? Convert.ToDecimal(row["RecMaterialConv"]) : 0,
                                                        InterStoreTransfer = row["InterStoreTRansfer"] != DBNull.Value ? Convert.ToDecimal(row["InterStoreTRansfer"]) : 0,
                                                        OtherRec = row["OtherRec"] != DBNull.Value ? Convert.ToDecimal(row["OtherRec"]) : 0,
                                                        TotalRec = row["TotalRecQty"] != DBNull.Value ? Convert.ToDecimal(row["TotalRecQty"]) : 0,

                                                        
                                                        IssuedToShopFloorAgstPlan = row["IssuedToShopFloorAgstPlan"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstPlan"]) : 0,
                                                        IssuedToShopFloorAgstBOMReq = row["IssuedToShopFloorAgstBOMReq"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstBOMReq"]) : 0,
                                                        IssuedToShopFloorAgstIndReq = row["IssuedToShopFloorAgstIndReq"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstIndReq"]) : 0,
                                                        IssuedViaChallan = row["IssuedViaChallan"] != DBNull.Value ? Convert.ToDecimal(row["IssuedViaChallan"]) : 0,
                                                        ReturnViaRejection = row["ReturnViaRejection"] != DBNull.Value ? Convert.ToDecimal(row["ReturnViaRejection"]) : 0,
                                                        IssFromInterStoreTransf = row["IssFromInterStoreTransf"] != DBNull.Value ? Convert.ToDecimal(row["IssFromInterStoreTransf"]) : 0,
                                                        IssViaMaterialConv = row["IssViaMaterialConv"] != DBNull.Value ? Convert.ToDecimal(row["IssViaMaterialConv"]) : 0,
                                                        IssStockAdjustment = row["IssStockAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["IssStockAdjustment"]) : 0,
                                                        IssueOther = row["IssueOther"] != DBNull.Value ? Convert.ToDecimal(row["IssueOther"]) : 0,
                                                        TotalIssue = row["TotalIssue"] != DBNull.Value ? Convert.ToDecimal(row["TotalIssue"]) : 0,

                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        ConsumedRMQty = row["ConsumedRMQty"] != DBNull.Value ? Convert.ToDecimal(row["ConsumedRMQty"]) : 0,
                                                        ConsumedRMUnit = row["ConsumedRMUnit"] != DBNull.Value ? row["ConsumedRMUnit"].ToString() : string.Empty,
                                                        FGOKQty = row["FGOKQty"] != DBNull.Value ? Convert.ToDecimal(row["FGOKQty"]) : 0,
                                                        FGProdQty = row["FGProdQty"] != DBNull.Value ? Convert.ToDecimal(row["FGProdQty"]) : 0,
                                                        ClosingStock = row["ClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["ClosingStock"]) : 0,
                                                        WIPClosingStock = row["WIPClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["WIPClosingStock"]) : 0,
                                                        RMItemCode = row["RMItemcode"] != DBNull.Value ? Convert.ToInt32(row["RMItemcode"]) : 0


                                                    }).ToList();
                    }
                }
                else if (ReportType.ToString() == "ProductionConsumptionReport(DETAIL)")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.ConsumptionReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new ConsumptionReportModel
                                                    {
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        OpeningQty = row["OpeningQty"] != DBNull.Value ? Convert.ToDecimal(row["OpeningQty"]) : 0,
                                                        OpeningWIP = row["OpeningWIP"] != DBNull.Value ? Convert.ToDecimal(row["OpeningWIP"]) : 0,
                                                        Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                        PONo = row["PONo"] != DBNull.Value ? row["PONo"].ToString() : string.Empty,
                                                        PODate = row["PODate"] != DBNull.Value ? Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                        POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                        AltPOQty = row["AltPOQty"] != DBNull.Value ? Convert.ToDecimal(row["AltPOQty"]) : 0,
                                                       // AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,
                                                        VendorName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                        AltUnit = row["AltUnit"] != DBNull.Value ? row["AltUnit"].ToString() : string.Empty,
                                                        SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                        SchDate = row["SchDate"] != DBNull.Value ? Convert.ToDateTime(row["SchDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                        RecFromMRN = row["RecFromMRN"] != DBNull.Value ? Convert.ToDecimal(row["RecFromMRN"]) : 0,
                                                        RecFromJW = row["RecFromJW"] != DBNull.Value ? Convert.ToDecimal(row["RecFromJW"]) : 0,
                                                        OtherRec = row["OtherRec"] != DBNull.Value ? Convert.ToDecimal(row["OtherRec"]) : 0,
                                                        TotalRec = row["TotalRec"] != DBNull.Value ? Convert.ToDecimal(row["TotalRec"]) : 0,
                                                        IssuedToShopFloorAgstPlan = row["IssuedToShopFloorAgstPlan"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstPlan"]) : 0,
                                                        IssuedToShopFloorAgstBOMReq = row["IssuedToShopFloorAgstBOMReq"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstBOMReq"]) : 0,
                                                        IssuedToShopFloorAgstIndReq = row["IssuedToShopFloorAgstIndReq"] != DBNull.Value ? Convert.ToDecimal(row["IssuedToShopFloorAgstIndReq"]) : 0,
                                                        IssuedViaChallan = row["IssuedViaChallan"] != DBNull.Value ? Convert.ToDecimal(row["IssuedViaChallan"]) : 0,
                                                        ReturnViaRejection = row["ReturnViaRejection"] != DBNull.Value ? Convert.ToDecimal(row["ReturnViaRejection"]) : 0,
                                                        IssFromInterStoreTransf = row["IssFromInterStoreTransf"] != DBNull.Value ? Convert.ToDecimal(row["IssFromInterStoreTransf"]) : 0,
                                                        IssViaMaterialConv = row["IssViaMaterialConv"] != DBNull.Value ? Convert.ToDecimal(row["IssViaMaterialConv"]) : 0,
                                                        IssStockAdjustment = row["IssStockAdjustment"] != DBNull.Value ? Convert.ToDecimal(row["IssStockAdjustment"]) : 0,
                                                        IssueOther = row["IssueOther"] != DBNull.Value ? Convert.ToDecimal(row["IssueOther"]) : 0,
                                                        TotalIssue = row["TotalIssue"] != DBNull.Value ? Convert.ToDecimal(row["TotalIssue"]) : 0,
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        ConsumedRMQty = row["ConsumedRMQty"] != DBNull.Value ? Convert.ToDecimal(row["ConsumedRMQty"]) : 0,
                                                        ConsumedRMUnit = row["ConsumedRMUnit"] != DBNull.Value ? row["ConsumedRMUnit"].ToString() : string.Empty,
                                                        FGOKQty = row["FGOKQty"] != DBNull.Value ? Convert.ToDecimal(row["FGOKQty"]) : 0,
                                                        FGProdQty = row["FGProdQty"] != DBNull.Value ? Convert.ToDecimal(row["FGProdQty"]) : 0,
                                                        ClosingStock = row["ClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["ClosingStock"]) : 0,
                                                        WIPClosingStock = row["WIPClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["WIPClosingStock"]) : 0,
                                                        RMItemCode = row["RMItemcode"] != DBNull.Value ? Convert.ToInt32(row["RMItemcode"]) : 0

                                                    }).ToList();
                    }
                }
               else if (ReportType.ToString() == "ProductionConsumptionReport(CONSOLIDATED)")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.ConsumptionReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new ConsumptionReportModel
                                                    {
                                                        RMPartCode = row["RMPartCode"] != DBNull.Value ? row["RMPartCode"].ToString() : string.Empty,
                                                        RMItemName = row["RMItemName"] != DBNull.Value ? row["RMItemName"].ToString() : string.Empty,
                                                        OpeningQty = row["OpeningQty"] != DBNull.Value ? Convert.ToDecimal(row["OpeningQty"]) : 0,
                                                        OpeningWIP = row["OpeningWIP"] != DBNull.Value ? Convert.ToDecimal(row["OpeningWIP"]) : 0,
                                                        Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                        POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                        TotalRec = row["TotalRecQty"] != DBNull.Value ? Convert.ToDecimal(row["TotalRecQty"]) : 0,
                                                        TotalIssue = row["TotalIssue"] != DBNull.Value ? Convert.ToDecimal(row["TotalIssue"]) : 0,
                                                        ConsumedRMQty = row["ConsumedRMQty"] != DBNull.Value ? Convert.ToDecimal(row["ConsumedRMQty"]) : 0,
                                                        ConsumedRMUnit = row["ConsumedRMUnit"] != DBNull.Value ? row["ConsumedRMUnit"].ToString() : string.Empty,
                                                        FGPartCode = row["FGPartCode"] != DBNull.Value ? row["FGPartCode"].ToString() : string.Empty,
                                                        FGItemName = row["FGItemName"] != DBNull.Value ? row["FGItemName"].ToString() : string.Empty,
                                                        FGOKQty = row["FGOKQty"] != DBNull.Value ? Convert.ToDecimal(row["FGOKQty"]) : 0,
                                                        FGProdQty = row["FGProdQty"] != DBNull.Value ? Convert.ToDecimal(row["FGProdQty"]) : 0,
                                                        ClosingStock = row["ClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["ClosingStock"]) : 0,
                                                        WIPClosingStock = row["WIPClosingStock"] != DBNull.Value ? Convert.ToDecimal(row["WIPClosingStock"]) : 0,
                                                        RMItemCode = row["RMItemCode"] != DBNull.Value ? Convert.ToInt32(row["RMItemCode"]) : 0

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
