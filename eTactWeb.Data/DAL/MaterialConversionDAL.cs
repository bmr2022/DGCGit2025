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
    public class MaterialConversionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public MaterialConversionDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> SaveMaterialConversion(MaterialConversionModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();


            try
            {
                var sqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    //sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    //sqlParams.Add(new SqlParameter("@EffectiveDate", model.EffectiveDate));
                    //sqlParams.Add(new SqlParameter("@ActualEntryByEmp", model.ActualEntryByEmp));
                    //sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                    //sqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? Environment.MachineName));
                    //sqlParams.Add(new SqlParameter("@MainItemcode", GetMainItemCodeFromGIGrid(GIGrid))); 
                    //sqlParams.Add(new SqlParameter("@AlternateItemCode", model.AlternateItemCode)); 
                    //sqlParams.Add(new SqlParameter("@UpdatedByEmp", model.UpdatedByEmp));
                    //sqlParams.Add(new SqlParameter("@UpdationDate", DateTime.Now));
                    //sqlParams.Add(new SqlParameter("@ProcGrid", GIGrid));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    sqlParams.Add(new SqlParameter("@MatConvEntryId", model.EntryId));
                    sqlParams.Add(new SqlParameter("@YearCode", model.OpeningYearCode));
                    sqlParams.Add(new SqlParameter("@MatConvSlipNo", model.SlipNo));
                    sqlParams.Add(new SqlParameter("@MatConvSlipDate", model.SlipDate));
                    sqlParams.Add(new SqlParameter("@StoreWorkcenter", model.IssueToStoreWC));
                    sqlParams.Add(new SqlParameter("@Remarks", model.Remarks));
                    sqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                    sqlParams.Add(new SqlParameter("@Uid", model.Uid));
                    sqlParams.Add(new SqlParameter("@CC", model.cc));
                    sqlParams.Add(new SqlParameter("@ActualEntryByEmpid", model.ActualEntryByEmpid));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
                    sqlParams.Add(new SqlParameter("@UpdationDate", model.UpdationDate));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));

                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
                }

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", sqlParams);

            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEntryID(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillBranch()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "LoadBranch"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> GetDashboardData(MaterialConversionModel model)
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

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpMaterialConversionMainDetail", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "LoadStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<MaterialConversionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MaterialConversionModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpMaterialConversionMainDetail", myConnection)
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
                    if(ReportType== "SUMMARY")
                    {
                        model.MaterialConversionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new MaterialConversionModel
                                                        {
                                                            EntryId = dr["MatConvEntryId"] != DBNull.Value ? Convert.ToInt32(dr["MatConvEntryId"]) : 0,
                                                            SlipNo = dr["MatConvSlipNo"] != DBNull.Value ? Convert.ToString(dr["MatConvSlipNo"]) : string.Empty,
                                                            SlipDate = dr["MatConvSlipDate"] != DBNull.Value ? Convert.ToDateTime(dr["MatConvSlipDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            IssueToStoreWC = dr["StoreWorkcenter"] != DBNull.Value ? Convert.ToString(dr["StoreWorkcenter"]) : string.Empty,
                                                            Remark = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                            ApprovedByEmpName = dr["ActualEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEmployee"]) : string.Empty,

                                                            ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd/MM/yyyy"): string.Empty,

                                                           // UpdationDate = dr["UpdationDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdationDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            
                                                            UpdatedByEmpId = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedByEmployee"]) : 0,
                                                            
                                                            EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                                            OpeningYearCode = dr["MatConvYearCode"] != DBNull.Value ? Convert.ToInt32(dr["MatConvYearCode"]) : 0

                                                        }).ToList();
                    }
                   
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if(ReportType== "DETAIL")
                    {
                        model.MaterialConversionGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new MaterialConversionModel
                                                        {
                                                            EntryId = dr["MatConvEntryId"] != DBNull.Value ? Convert.ToInt32(dr["MatConvEntryId"]) : 0,
                                                            SlipNo = dr["MatConvSlipNo"] != DBNull.Value ? Convert.ToString(dr["MatConvSlipNo"]) : string.Empty,
                                                            SlipDate = dr["MatConvSlipDate"] != DBNull.Value ? Convert.ToDateTime(dr["MatConvSlipDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            IssueToStoreWC = dr["StoreWorkcenter"] != DBNull.Value ? Convert.ToString(dr["StoreWorkcenter"]) : string.Empty,

                                                            OriginalPartCode = dr["OrigPartCode"] != DBNull.Value ? Convert.ToString(dr["OrigPartCode"]) : string.Empty,
                                                            OriginalItemName = dr["OrigItemName"] != DBNull.Value ? Convert.ToString(dr["OrigItemName"]) : string.Empty,
                                                            OriginalQty = dr["OriginalQty"] != DBNull.Value ? Convert.ToDecimal(dr["OriginalQty"]) : 0,
                                                            Unit = dr["Unit"] != DBNull.Value ? Convert.ToString(dr["Unit"]) : string.Empty,
                                                            StoreName = dr["OrigStoreName"] != DBNull.Value ? Convert.ToString(dr["OrigStoreName"]) : string.Empty,
                                                            WorkCenterName = dr["origWorkcenetr"] != DBNull.Value ? Convert.ToString(dr["origWorkcenetr"]) : string.Empty,

                                                            AltPartCode = dr["AltPartCode"] != DBNull.Value ? Convert.ToString(dr["AltPartCode"]) : string.Empty,
                                                            AltItemName = dr["AltItemName"] != DBNull.Value ? Convert.ToString(dr["AltItemName"]) : string.Empty,
                                                            AltOriginalQty = dr["AltOriginalQty"] != DBNull.Value ? Convert.ToDecimal(dr["AltOriginalQty"]) : 0,
                                                            AltUnit = dr["AltUnit"] != DBNull.Value ? Convert.ToString(dr["AltUnit"]) : string.Empty,
                                                            AltStoreName = dr["AltStoreName"] != DBNull.Value ? Convert.ToString(dr["AltStoreName"]) : string.Empty,
                                                           // AltWorkCenterName = dr["AltWorkcenetr"] != DBNull.Value ? Convert.ToString(dr["AltWorkcenetr"]) : string.Empty,

                                                            BatchNo = dr["BatchNo"] != DBNull.Value ? Convert.ToString(dr["BatchNo"]) : string.Empty,
                                                            UniqueBatchNo = dr["Uniquebatchno"] != DBNull.Value ? Convert.ToString(dr["Uniquebatchno"]) : string.Empty,
                                                            BatchStock = dr["BatchStock"] != DBNull.Value ? Convert.ToDecimal(dr["BatchStock"]) : 0,
                                                            TotalStock = dr["TotalStock"] != DBNull.Value ? Convert.ToDecimal(dr["TotalStock"]) : 0,
                                                            //AltStock = dr["AltStock"] != DBNull.Value ? Convert.ToDecimal(dr["AltStock"]) : 0,

                                                            OrigItemRate = dr["OrigItemRate"] != DBNull.Value ? Convert.ToDecimal(dr["OrigItemRate"]) : 0,
                                                            Remark = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
                                                            ApprovedByEmpName = dr["ActualEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEmployee"]) : string.Empty,

                                                            
                                                            ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            UpdatedByEmpId = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedByEmployee"]) : 0,
                                                           // UpdationDate = dr["UpdationDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdationDate"]).ToString("dd/MM/yyyy") : string.Empty,

                                                            EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                                            OpeningYearCode = dr["MatConvYearCode"] != DBNull.Value ? Convert.ToInt32(dr["MatConvYearCode"]) : 0

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
        public async Task<ResponseResult> FillWorkCenterName()
         {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "LoadWorkcenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
         }
         public async Task<ResponseResult> GetOriginalItemName()
         {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
         }
         public async Task<ResponseResult> GetOriginalPartCode()
         {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
         }
        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                var Date = DateTime.Now;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                SqlParams.Add(new SqlParameter("@transDate", Date));
                SqlParams.Add(new SqlParameter("@batchno", batchno));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("FillCurrentBatchINStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetUnitAltUnit(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "Getunit"));
                SqlParams.Add(new SqlParameter("@origItemcode", ItemCode));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAltPartCode(int MainItemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillAlternatePartCode"));
                SqlParams.Add(new SqlParameter("@origItemcode", MainItemcode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAltItemName(int MainItemcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillAlternateItemName"));
                SqlParams.Add(new SqlParameter("@origItemcode", MainItemcode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpMaterialConversionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
    }
}
