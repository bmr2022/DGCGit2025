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
                var entDate = CommonFunc.ParseFormattedDate(model.Entry_Date);
                var ActentDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                var upDate = CommonFunc.ParseFormattedDate(model.LastUpdationDate);
                SqlParams.Add(new SqlParameter("@CntPlanEntryId", model.CntPlanEntryId));
                SqlParams.Add(new SqlParameter("@CntPlanEntryDate", entDate));
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
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ActentDate));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationDate", upDate));
                SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? ""));
                SqlParams.Add(new SqlParameter("@ItemimagePath", model.ItemImageURL ?? ""));
                SqlParams.Add(new SqlParameter("@DrawingNo", model.DrawingNo ?? ""));
                SqlParams.Add(new SqlParameter("@DrawingNoImagePath", model.ImageURL ?? ""));

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
                    oCmd.Parameters.AddWithValue("@ReportType", "SUMMARY");
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
                var entrydate = CommonFunc.ParseFormattedDate(EntryDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@CntPlanEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", YearCode));
                //SqlParams.Add(new SqlParameter("@ActualEntryDate", EntryDate));
                //SqlParams.Add(new SqlParameter("@ActualEntryDate", DateTime.ParseExact(EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MMM/yyyy")));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entrydate));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", EntryByempId));
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
        public async Task<ControlPlanModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            var model = new ControlPlanModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@CntPlanEntryId", ID));
                SqlParams.Add(new SqlParameter("@CntPlanYearCode", YC));
                //SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                //SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPQCControlPlanMain", SqlParams);

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
        private static ControlPlanModel PrepareView(DataSet DS, ref ControlPlanModel? model)
        {
            try
            {
                var ItemList = new List<ControlPlanDetailModel>();
                var DetailList = new List<ControlPlanModel>();
                DS.Tables[0].TableName = "ControlPlan";
                DS.Tables[1].TableName = "ControlPlanDetail";
                int cnt = 0;

                model.CntPlanEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["CntPlanEntryId"].ToString());
                model.Yearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["CntPlanYearCode"].ToString());
                model.Control_PlanNo = DS.Tables[0].Rows[0]["ControlPlanNo"].ToString();
                model.Entry_Date = DS.Tables[0].Rows[0]["CntPlanEntryDate"].ToString();

                model.RevNo = DS.Tables[0].Rows[0]["RevNo"].ToString();
                model.EffectiveDate = DS.Tables[0].Rows[0]["CntPlanEntryDate"].ToString();
                //model.PartCode = DS.Tables[0].Rows[0]["Item_Name"].ToString();
                //model.ItemName = DS.Tables[0].Rows[0]["Item_Name"].ToString();

                model.ItemName = DS.Tables[0].Rows[0]["Item_Name"].ToString();
                model.ItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCode"].ToString());

                model.ImageURL = DS.Tables[0].Rows[0]["DrawingNoImagePath"].ToString();
                model.ItemImageURL = DS.Tables[0].Rows[0]["ItemimagePath"].ToString();
                model.DrawingNo = DS.Tables[0].Rows[0]["DrawingNo"].ToString();

                model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEmployee"].ToString();
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["ActualEntryDate"]).ToString("dd/MM/yyyy") : string.Empty;
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();

                model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmployee"].ToString();
                model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"] != DBNull.Value ? Convert.ToDateTime(DS.Tables[0].Rows[0]["LastUpdationDate"]).ToString("dd/MM/yyyy") : string.Empty;
                //model.UpdationDate = DS.Tables[0].Rows[0]["UpdationDate"] != DBNull.Value? Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdationDate"]).ToString("dd/MM/yyyy"): string.Empty;

                if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        ItemList.Add(new ControlPlanDetailModel
                        {
                            CntPlanEntryId = Convert.ToInt32(row["CntPlanEntryId"].ToString()),
                            CntPlanYearCode = Convert.ToInt32(row["CntPlanYearCode"].ToString()),
                            SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                            Characteristic = row["Characteristic"].ToString(),
                            EvalutionMeasurmentTechnique = row["EvalutionMeasurmentTechnique"].ToString(),
                            SpecificationFrom = row["SpecificationFrom"].ToString(),
                            Operator = row["Operator"].ToString(),
                            SpecificationTo = row["SpecificationTo"].ToString(),
                            FrequencyofTesting = row["FrequencyofTesting"].ToString(),
                            InspectionBy = row["InspectionBy"].ToString(),
                            ControlMethod = row["ControlMethod"].ToString(),
                            RejectionPlan = row["RejectionPlan"].ToString(),
                            Remarks = row["Remarks"].ToString(),
                            //ItemImageURL = row["ItemimagePath"].ToString(),
                            //DrawingNo = row["DrawingNo"].ToString(),
                            //ImageURL = row["DrawingNoImagePath"].ToString(),
                         


                        });
                    }
                    model.DTSSGrid = ItemList;
                    model.ImageURL = DS.Tables[0].Rows[0]["DrawingNoImagePath"].ToString();
                    model.ItemImageURL = DS.Tables[0].Rows[0]["ItemimagePath"].ToString();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ControlPlanModel> GetByItemOrPartCode(int ItemCode)
        {
            var model = new ControlPlanModel();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "VIEWBYDASHBOARD")
        };

                if (ItemCode > 0)
                    SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

               

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPQCControlPlanMain", SqlParams);

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

    }
}
