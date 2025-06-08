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
    public class ControlPlanDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ControlPlanDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> SaveControlPlan(ControlPlanModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();

                if (model.Mode == "U" || model.Mode == "V")
                {

                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                   
                }

                SqlParams.Add(new SqlParameter("@CntPlanEntryId", model.CntPlanEntryId));
                SqlParams.Add(new SqlParameter("@CntPlanEntryDate", model.CntPlanEntryDate));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", model.CntPlanYearCode));
                SqlParams.Add(new SqlParameter("@ControlPlanNo", model.Control_PlanNo ?? ""));
                SqlParams.Add(new SqlParameter("@RevNo", model.RevNo ?? ""));
                SqlParams.Add(new SqlParameter("@ForInOutInprocess", model.ForInOutInprocess ?? ""));
                SqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@EngApprovedBy", model.EngApprovedBy));
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                SqlParams.Add(new SqlParameter("@Remarks", model.Remarks ?? ""));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? ""));
                SqlParams.Add(new SqlParameter("@UId", model.UId));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationDate", model.LastUpdationDate));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? ""));

                SqlParams.Add(new SqlParameter("@DTSSGrid", GIGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetNewEntryId(int Yearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", Yearcode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
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

                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
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

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetEvMeasureTech()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "EvalutionMeasurmentTechnique"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCharacteristic()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "Characteristic"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> GetDashboardData(ControlPlanModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@REportType", model.ReportType));
                SqlParams.Add(new SqlParameter("@Fromdate", model.FromDate));
                SqlParams.Add(new SqlParameter("@Todate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPQCControlPlanMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ControlPlanModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new ControlPlanModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPQCControlPlanMain", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@REportType", ReportType);
                    oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@Todate", ToDate);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (ReportType == "SUMMARY")
                    {
                        model.DTSSGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new ControlPlanDetailModel
                                                        {
                                                            CntPlanEntryId = dr["CntPlanEntryId"] != DBNull.Value ? Convert.ToInt32(dr["CntPlanEntryId"]) : 0,
                                                            CntPlanEntryDate = dr["CntPlanEntryDate"] != DBNull.Value ? Convert.ToString(dr["CntPlanEntryDate"]) : string.Empty,
                                                            CntPlanYearCode = dr["CntPlanYearCode"] != DBNull.Value ? Convert.ToInt32(dr["CntPlanYearCode"]) : 0,
                                                            Control_PlanNo = dr["ControlPlanNo"] != DBNull.Value ? Convert.ToString(dr["ControlPlanNo"]) : string.Empty,
                                                            RevNo = dr["RevNo"] != DBNull.Value ? Convert.ToString(dr["RevNo"]) : string.Empty,
                                                            ForInOutInprocess = dr["ForInOutInprocess"] != DBNull.Value ? Convert.ToString(dr["ForInOutInprocess"]) : string.Empty,
                                                            ItemCode = dr["ItemCode"] != DBNull.Value ? Convert.ToInt32(dr["ItemCode"]) : 0,
                                                            ItemName = dr["Item_Name"] != DBNull.Value ? Convert.ToString(dr["Item_Name"]) : string.Empty,
                                                            AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                                            // AccountName       = dr["Account_Name"]         != DBNull.Value ? Convert.ToString(dr["Account_Name"])         : string.Empty,
                                                            EngApprovedBy = dr["EngApprovedBy"] != DBNull.Value ? Convert.ToInt32(dr["EngApprovedBy"]) : 0,
                                                            ApprovedBy = dr["ApprovedBy"] != DBNull.Value ? Convert.ToInt32(dr["ApprovedBy"]) : 0,
                                                            Remarks = dr["Remarks"] != DBNull.Value ? Convert.ToString(dr["Remarks"]) : string.Empty,
                                                            CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,
                                                            UId = dr["UId"] != DBNull.Value ? Convert.ToInt32(dr["UId"]) : 0,
                                                            ActualEntryBy = dr["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryBy"]) : 0,
                                                            ActualEntryByName = dr["ActualEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEmployee"]) : string.Empty,
                                                            ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToString(dr["ActualEntryDate"]) : string.Empty,
                                                            LastUpdatedBy = dr["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["LastUpdatedBy"]) : 0,
                                                            LastUpdatedByName = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToString(dr["UpdatedByEmployee"]) : string.Empty,
                                                            LastUpdationDate = dr["LastUpdationDate"] != DBNull.Value ? Convert.ToString(dr["LastUpdationDate"]) : string.Empty,
                                                            EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,

                                                        }).ToList();
                    }

                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (ReportType == "DETAIL")
                    {
                        model.DTSSGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new ControlPlanDetailModel
                                                        {
                                                            
                                                        }).ToList();
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
