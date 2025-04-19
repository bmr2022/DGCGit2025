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
    public class MaterialConversionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public MaterialConversionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "Material Conversion"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Sale Order"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
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
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@MatConvEntryId", model.EntryId));
                    sqlParams.Add(new SqlParameter("@YearCode", model.OpeningYearCode));
                    sqlParams.Add(new SqlParameter("@MatConvSlipNo", model.SlipNo));
                    sqlParams.Add(new SqlParameter("@MatConvSlipDate", model.SlipDate));
                    sqlParams.Add(new SqlParameter("@StoreWorkcenter", model.IssueToStoreWC));
                    sqlParams.Add(new SqlParameter("@Remarks", model.Remarks));
                    sqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                    sqlParams.Add(new SqlParameter("@Uid", model.Uid));
                    sqlParams.Add(new SqlParameter("@cc", model.cc));
                    sqlParams.Add(new SqlParameter("@ActualEntryByEmpid", model.ActualEntryByEmpid));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
                    sqlParams.Add(new SqlParameter("@UpdationDate", model.UpdationDate));
                    sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));

                    sqlParams.Add(new SqlParameter("@dt", GIGrid));
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
                                                            //seqno = dr["seqno"] != DBNull.Value ? Convert.ToInt32(dr["seqno"]) : 0,
                                                            SlipNo = dr["MatConvSlipNo"] != DBNull.Value ? Convert.ToString(dr["MatConvSlipNo"]) : string.Empty,
                                                            SlipDate = dr["MatConvSlipDate"] != DBNull.Value ? Convert.ToDateTime(dr["MatConvSlipDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                            IssueToStoreWC = dr["StoreWorkcenter"] != DBNull.Value ? Convert.ToString(dr["StoreWorkcenter"]) : string.Empty,
                                                            //Remark = dr["Remark"] != DBNull.Value ? Convert.ToString(dr["Remark"]) : string.Empty,
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
                                                            seqno = dr["seqno"] != DBNull.Value ? Convert.ToInt32(dr["seqno"]) : 0,
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
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@MatConvEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                //SqlParams.Add(new SqlParameter("@ActualEntryDate", EntryDate));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", DateTime.ParseExact(EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MMM/yyyy")));


                SqlParams.Add(new SqlParameter("@ActualEntryByEmpid", EntryByempId));
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
        public async Task<MaterialConversionModel> GetViewByID(int ID, int YC,string FromDate,string ToDate)
        {
            var model = new MaterialConversionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@MatConvEntryId", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SpMaterialConversionMainDetail", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }
        private static MaterialConversionModel PrepareView(DataSet DS, ref MaterialConversionModel? model)
        {
            try
            {
                var ItemList = new List<MaterialConversionModel>();
                var DetailList = new List<MaterialConversionModel>();
                DS.Tables[0].TableName = "MaterialConversion";
                DS.Tables[1].TableName = "MaterialConversionDetail";
                int cnt = 0;

                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MatConvEntryId"].ToString());
                model.OpeningYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MatConvYearCode"].ToString());
                model.SlipNo = DS.Tables[0].Rows[0]["MatConvSlipNo"].ToString();
              
                model.IssueToStoreWC = DS.Tables[0].Rows[0]["StoreWorkcenter"].ToString();
                model.Remark = DS.Tables[0].Rows[0]["Remark"].ToString();
                model.ApprovedByEmpName = DS.Tables[0].Rows[0]["ActualEmployee"].ToString();
                
                model.UpdatedByEmpName = DS.Tables[0].Rows[0]["UpdatedByEmployee"].ToString();
                
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                model.SlipDate = DS.Tables[0].Rows[0]["MatConvSlipDate"] != DBNull.Value? Convert.ToDateTime(DS.Tables[0].Rows[0]["MatConvSlipDate"]).ToString("dd/MM/yyyy"): string.Empty;
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]).ToString("dd/MM/yyyy"): string.Empty;
                //model.UpdationDate = DS.Tables[0].Rows[0]["UpdationDate"] != DBNull.Value? Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdationDate"]).ToString("dd/MM/yyyy"): string.Empty;

                if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ItemList.Add(new MaterialConversionModel
                        {
                            //EntryId = Convert.ToInt32(DS.Tables[1].Rows[1]["MatConvEntryId"].ToString()),
                            //OpeningYearCode = Convert.ToInt32(DS.Tables[1].Rows[1]["MatConvYearCode"].ToString()),
                            //StoreId = Convert.ToInt32(DS.Tables[1].Rows[1]["OriginalStoreId"].ToString()),
                            //StoreName = DS.Tables[1].Rows[1]["OrigStoreName"].ToString(),
                            //OriginalItemCode = DS.Tables[1].Rows[1]["OriginalItemCode"].ToString(),
                            //OriginalPartCode = DS.Tables[1].Rows[1]["OrigPartCode"].ToString(),
                            //OriginalItemName = DS.Tables[1].Rows[1]["OrigItemName"].ToString(),
                            //OriginalQty = Convert.ToDecimal(DS.Tables[1].Rows[1]["OriginalQty"].ToString()),
                            //Unit = DS.Tables[1].Rows[1]["Unit"].ToString(),
                            //WorkCenterName = DS.Tables[1].Rows[1]["OrigWorkcenetr"].ToString(),

                            //AltStoreId = Convert.ToInt32(DS.Tables[1].Rows[1]["AltStoreId"].ToString()),
                            //AltStoreName = DS.Tables[1].Rows[1]["AltStoreName"].ToString(),
                            //OrginalWCID = Convert.ToInt32(DS.Tables[1].Rows[1]["OrginalWCID"].ToString()),
                            //AltWCID = Convert.ToInt32(DS.Tables[1].Rows[1]["AltWCID"].ToString()),
                            //AltWorkCenterName = DS.Tables[1].Rows[1]["AltWorkcenetr"].ToString(),
                            //AltPartCode = DS.Tables[1].Rows[1]["AltPartCode"].ToString(),
                            //AltItemName = DS.Tables[1].Rows[1]["AltItemName"].ToString(),
                            //AltOriginalQty = Convert.ToDecimal(DS.Tables[1].Rows[1]["AltOriginalQty"].ToString()),
                            //AltUnit = DS.Tables[1].Rows[1]["AltUnit"].ToString(),
                            //AltStock = Convert.ToDecimal(DS.Tables[1].Rows[1]["AltStock"].ToString()),

                            //BatchNo = DS.Tables[0].Rows[0]["BatchNo"].ToString(),
                            //UniqueBatchNo = DS.Tables[0].Rows[0]["Uniquebatchno"].ToString(),
                            //BatchStock = Convert.ToDecimal(DS.Tables[0].Rows[0]["BatchStock"].ToString()),
                            //TotalStock = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalStock"].ToString()),

                            //OrigItemRate = Convert.ToDecimal(DS.Tables[1].Rows[1]["OrigItemRate"].ToString()),
                            //Remark = DS.Tables[1].Rows[1]["Remark"].ToString(),
                            //ApprovedByEmpName =DS.Tables[1].Rows[1]["ActualEmployee"].ToString(),
                            //ActualEntryDate = DS.Tables[1].Rows[1]["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                            //UpdatedByEmpName =DS.Tables[1].Rows[1]["UpdatedByEmployee"].ToString(),
                            //UpdationDate = DS.Tables[1].Rows[1]["UpdationDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdationDate"]).ToString("dd/MM/yyyy") : string.Empty,
                            //EntryByMachine = DS.Tables[1].Rows[1]["EntryByMachine"].ToString(),

                            //PlanNo =DS.Tables[1].Rows[1]["PlanNo"].ToString(),
                            //PlanYearCode = Convert.ToInt32(DS.Tables[1].Rows[1]["PlanYearCode"].ToString()),
                            //PlanDate = DS.Tables[1].Rows[1]["PlanDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["PlanDate"]).ToString("dd/MM/yyyy") : string.Empty,
                            //ProdSchNo = DS.Tables[1].Rows[1]["ProdSchNo"].ToString(),
                            //ProdSchYearCode = Convert.ToInt32(DS.Tables[1].Rows[1]["ProdSchYearCode"].ToString()),
                            //ProdSchDatetime = DS.Tables[1].Rows[1]["ProdSchDatetime"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ProdSchDatetime"]).ToString("dd/MM/yyyy") : string.Empty,
                           
                            EntryId = Convert.ToInt32(row["MatConvEntryId"].ToString()),
                            OpeningYearCode = Convert.ToInt32(row["MatConvYearCode"].ToString()),
                            StoreId = Convert.ToInt32(row["OriginalStoreId"].ToString()),
                            seqno = Convert.ToInt32(row["seqno"].ToString()),
                            StoreName = row["OrigStoreName"].ToString(),
                            OriginalItemCode = row["OriginalItemCode"].ToString(),
                            OriginalPartCode = row["OrigPartCode"].ToString(),
                            OriginalItemName = row["OrigItemName"].ToString(),
                            OriginalQty = Convert.ToDecimal(row["OriginalQty"].ToString()),
                            Unit = row["Unit"].ToString(),
                            WorkCenterName = row["OrigWorkcenetr"].ToString(),

                            AltStoreId = Convert.ToInt32(row["AltStoreId"].ToString()),
                            AltStoreName = row["AltStoreName"].ToString(),
                            OrginalWCID = Convert.ToInt32(row["OrginalWCID"].ToString()),
                            AltWCID = Convert.ToInt32(row["AltWCID"].ToString()),
                            AltWorkCenterName = row["AltWorkcenetr"].ToString(),
                            AltPartCode = row["AltPartCode"].ToString(),
                            AltItemName = row["AltItemName"].ToString(),
                            AltOriginalQty = Convert.ToDecimal(row["AltOriginalQty"].ToString()),
                            AltUnit = row["AltUnit"].ToString(),
                            AltStock = Convert.ToDecimal(row["AltStock"].ToString()),

                            BatchNo = row["BatchNo"].ToString(),
                            UniqueBatchNo = row["Uniquebatchno"].ToString(),
                            BatchStock = Convert.ToDecimal(row["BatchStock"].ToString()),
                            TotalStock = Convert.ToDecimal(row["TotalStock"].ToString()),

                            OrigItemRate = Convert.ToDecimal(row["OrigItemRate"].ToString()),
                            Remark = row["Remark"].ToString(),
                            ApprovedByEmpName = row["ActualEmployee"].ToString(),

                            ActualEntryDate = row["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["ActualEntryDate"]).ToString("dd/MMM/yyyy") : string.Empty,
                            UpdatedByEmpName = row["UpdatedByEmployee"].ToString(),
                            UpdationDate = row["UpdationDate"] != DBNull.Value ? Convert.ToDateTime(row["UpdationDate"]).ToString("dd/MMM/yyyy") : string.Empty,

                            EntryByMachine = row["EntryByMachine"].ToString(),

                            PlanNo = row["PlanNo"].ToString(),
                            PlanYearCode = Convert.ToInt32(row["PlanYearCode"].ToString()),
                            PlanDate = row["PlanDate"] != DBNull.Value ? Convert.ToDateTime(row["PlanDate"]).ToString("dd/MMM/yyyy") : string.Empty,
                            ProdSchNo = row["ProdSchNo"].ToString(),
                            ProdSchYearCode = Convert.ToInt32(row["ProdSchYearCode"].ToString()),
                            ProdSchDatetime = row["ProdSchDatetime"] != DBNull.Value ? Convert.ToDateTime(row["ProdSchDatetime"]).ToString("dd/MMM/yyyy") : string.Empty,

                        });
                    }
                    model.MaterialConversionGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
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
        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName,string WorkCenterName, int YearCode, string batchno, string FinStartDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                var Date = DateTime.Now;
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                //SqlParams.Add(new SqlParameter("@WorkCenterName", WorkCenterName));
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
