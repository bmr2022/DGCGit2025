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
    public class ProductionEntryReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ProductionEntryReportDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> FillFGPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFGItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFGItemName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMPARTCODE"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMItemName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLRMItemName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdSlipNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdPlanNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProdSchNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillReqNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLReqNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenter(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLWorkCenter"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMachinName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLMachinName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillOperatorName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLOperatorName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProcess(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProcess"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillShiftName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillShiftName"));
                
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPreportProductionEntry", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ProductionEntryReportModel> GetProductionEntryReport(string ReportType,string FromDate, string ToDate, string FGPartCode, string FGItemName,string RMPartCode,string RMItemName, string ProdSlipNo, string ProdPlanNo,string ProdSchNo, string ReqNo, string WorkCenter,string MachineName,string OperatorName,string Process,string ShiftName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new ProductionEntryReportModel();
            var ProductionReport = new List<ProductionEntryReportDetail>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPreportProductionEntry", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@ReportType", ReportType);
                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@PartCode", FGPartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", FGItemName);
                    oCmd.Parameters.AddWithValue("@RMPartCode", RMPartCode);
                    oCmd.Parameters.AddWithValue("@RMItemName", RMItemName);
                    oCmd.Parameters.AddWithValue("@ProdSlipNo", ProdSlipNo);
                    oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                    oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                    oCmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    oCmd.Parameters.AddWithValue("@WorkcenterName", WorkCenter);
                    oCmd.Parameters.AddWithValue("@processName", Process);
                    oCmd.Parameters.AddWithValue("@Operator", OperatorName);
                    oCmd.Parameters.AddWithValue("@ShiftName", ShiftName);
                    //oCmd.Parameters.AddWithValue("@machineName", MachineName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "DETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Date Wise FG Consolidated")//done(stocksummary,Zeroinventory,balance)
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Machine Wise FG Production SUmmary")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "RM Wise Consumption Summary")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "RM Wise Consumption Detail") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "RM Total Consumption") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "BreakDown Detail") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Machine Wise BreakDown Summary") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Operator Detail Report") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Operator Summary Report") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                else if (ReportType == "Production QC Deatil") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
                    }
                }
                 else if (ReportType == "Production Summary") // done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<ProductionEntryReportDetail>(row);
                            ProductionReport.Add(poDetail);
                        }
                        model.ProductionEntryReportDetail = ProductionReport;
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
