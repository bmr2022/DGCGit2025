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
    public class TransferMaterialReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public TransferMaterialReportDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_TransferMaterialFromWc", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillFromWorkCenter(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLFromWorkCenter"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillToWorkCenter(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLToWorkCenter"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillToStore(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLToStore"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FILLPartCode"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "FILLItemName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTransferSlipNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLTransferSlipNo"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillProcessName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLProcessName"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportTransferMaterial", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<TransferMaterialReportModel> GetTransferMaterialReport(string ReportType, string FromDate, string ToDate, string FromWorkCenter, string ToWorkCenter, string Tostore, string PartCode, string ItemName, string TransferSlipNo,string ProdSlipNo,string ProdPlanNo,string ProdSchNo,string ProcessName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new TransferMaterialReportModel();
            var TransferReport = new List<TransferMaterialReportDetail>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportTransferMaterial", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@fromdate", fromDt);
                    oCmd.Parameters.AddWithValue("@todate", toDt);
                    oCmd.Parameters.AddWithValue("@FromWorkcenterName", FromWorkCenter);
                    oCmd.Parameters.AddWithValue("@ToWorkcenterName", ToWorkCenter);
                    oCmd.Parameters.AddWithValue("@IssuedToStore", Tostore);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@TRansferSlipNo", TransferSlipNo);
                    oCmd.Parameters.AddWithValue("@ProdSlipNo", ProdSlipNo);
                    oCmd.Parameters.AddWithValue("@ProdPlanNo", ProdPlanNo);
                    oCmd.Parameters.AddWithValue("@ProdSchNo", ProdSchNo);
                    oCmd.Parameters.AddWithValue("@processName", ProcessName);
                    oCmd.Parameters.AddWithValue("@ItemCode", PartCode);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "Detail") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<TransferMaterialReportDetail>(row);
                            TransferReport.Add(poDetail);
                        }
                        model.TransferMaterialReportDetail = TransferReport;
                    }
                }
                else if (ReportType == "FromWcAndItemWiseConsolidated")//done(stocksummary,Zeroinventory,balance)
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<TransferMaterialReportDetail>(row);
                            TransferReport.Add(poDetail);
                        }
                        model.TransferMaterialReportDetail = TransferReport;
                    }
                }
                else if (ReportType == "FromWcAndItem+DateWiseConsolidated")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<TransferMaterialReportDetail>(row);
                            TransferReport.Add(poDetail);
                        }
                        model.TransferMaterialReportDetail = TransferReport;
                    }
                }
                else if (ReportType == "TransferMaterial Deatil")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<TransferMaterialReportDetail>(row);
                            TransferReport.Add(poDetail);
                        }
                        model.TransferMaterialReportDetail = TransferReport;
                    }
                }
                else if (ReportType == "TransferMaterial Date Wise Summary")//done & working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            var poDetail = CommonFunc.DataRowToClass<TransferMaterialReportDetail>(row);
                            TransferReport.Add(poDetail);
                        }
                        model.TransferMaterialReportDetail = TransferReport;
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
