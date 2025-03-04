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
    public class HRWeekOffMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private dynamic? _ResponseResult;

        public HRWeekOffMasterDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPWeekoffMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        
        public async Task<ResponseResult> SaveData(HRWeekOffMasterModel model)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPWeekoffMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@WeekoffEntryId", model.WeekoffEntryId);

                    // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
                    oCmd.Parameters.AddWithValue("@WeekoffYearCode", model.WeekoffYearCode);
                    oCmd.Parameters.AddWithValue("@WeekoffName", model.WeekoffName);
                    oCmd.Parameters.AddWithValue("@WeekoffTypefixRot", model.WeekoffTypefixRot);
                    //oCmd.Parameters.AddWithValue("@EmpCategoryId", model.EmpCategoryId);
                    //oCmd.Parameters.AddWithValue("@DeptId", model.DeptId);
                    oCmd.Parameters.AddWithValue("@WeekoffDays", model.WeekoffDays);
                    oCmd.Parameters.AddWithValue("@MinWorkDaysRequired", model.MinWorkDaysRequired);
                    oCmd.Parameters.AddWithValue("@halfdayFulldayOff", model.halfdayFulldayOff);
                    oCmd.Parameters.AddWithValue("@MaxWorkingDaysReqForWeekOff", model.MaxWorkingDaysReqForWeekOff);
                    oCmd.Parameters.AddWithValue("@OverrideForHolidays", model.OverrideForHolidays);
                    oCmd.Parameters.AddWithValue("@CompensatoryOffApplicable", model.CompensatoryOffApplicable);
                    oCmd.Parameters.AddWithValue("@ExtraPayApplicable", model.ExtraPayApplicable);
                    oCmd.Parameters.AddWithValue("@Remark", model.Remark);
                    oCmd.Parameters.AddWithValue("@Active", model.Active);
                    
                    oCmd.Parameters.AddWithValue("@EffectiveFrom",
               string.IsNullOrEmpty(model.EffectiveFrom) ? DBNull.Value : DateTime.Parse(model.EffectiveFrom).ToString("dd/MMM/yyyy"));

                    oCmd.Parameters.AddWithValue("@EntryByEmpId", model.EntryByEmpId);
                    oCmd.Parameters.AddWithValue("@EntryByMachine", model.EntryByMachine);
                    oCmd.Parameters.AddWithValue("@EmpCategoryId", model.EmpCategoryId);
                    oCmd.Parameters.AddWithValue("@DeptId", model.DeptId);


                    if (model.Mode == "UPDATE")
                    {
                        oCmd.Parameters.AddWithValue("@UpdatedbyId", model.UpdatedbyId);


                    }



                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusText = "Success",
                                Result = Reader["Result"].ToString()
                            };
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
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetEmpCat()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillEmployeeCategory"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPWeekoffMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDeptCat()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillDepartment"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPWeekoffMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _IDataLogic.ExecuteDataSet("HRSPWeekoffMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRWeekOffMasterModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRWeekOffMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPWeekoffMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.HRWeekOffMasterDashBoard = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new HRWeekOffMasterModel
                                               {
                                                   //SalHeadEffectiveDate = dr["SalHeadEffectiveDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["SalHeadEffectiveDate"]).ToString("dd-MM-yyyy"),
                                                   WeekoffEntryId = Convert.ToInt32(dr["WeekoffEntryId"]),

                                                   WeekoffYearCode = Convert.ToInt32(dr["WeekoffYearCode"]),
                                                   WeekoffName = dr["WeekoffName"].ToString(),
                                                   WeekoffTypefixRot = dr["WeekoffTypefixRot"].ToString(),
                                                   WeekoffDays = dr["WeekoffDays"].ToString(),
                                                   MinWorkDaysRequired = Convert.ToInt32(dr["MinWorkDaysRequired"]),
                                                   halfdayFulldayOff = dr["halfdayFulldayOff"].ToString(),
                                                   MaxWorkingDaysReqForWeekOff = Convert.ToInt32(dr["MaxWorkingDaysReqForWeekOff"]),


                                                   CompensatoryOffApplicable = dr["CompensatoryOffApplicable"].ToString(),
                                                   Remark = dr["Remark"].ToString(),
                                                   ExtraPayApplicable = dr["ExtraPayApplicable"].ToString(),
                                                  
                                                   EffectiveFrom =  dr["EffectiveFrom"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["EffectiveFrom"]).ToString("dd-MM-yyyy"),
                                                   //EntryByEmpId = Convert.ToInt32(dr["EntryByEmpId"]),
                                                   //UpdatedbyId = Convert.ToInt32(dr["UpdatedbyId"]),
                                                   Active = dr["Active"].ToString(),
                                                  

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

        public async Task<HRWeekOffMasterModel> GetViewByID(int ID)
        {
            var model = new HRWeekOffMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@WeekoffEntryId", ID));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPWeekoffMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTWeekOffMasterDetail = oDataSet.Tables[0];
                    

                    if (oDataSet.Tables.Count > 0 && DTWeekOffMasterDetail.Rows.Count > 0)
                    {
                        model.WeekoffEntryId = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["WeekoffEntryId"]);
                        model.WeekoffYearCode = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["WeekoffYearCode"]);
                        model.WeekoffName = DTWeekOffMasterDetail.Rows[0]["WeekoffName"].ToString();
                        model.WeekoffTypefixRot = DTWeekOffMasterDetail.Rows[0]["WeekoffTypefixRot"].ToString();
                        model.WeekoffDays = DTWeekOffMasterDetail.Rows[0]["WeekoffDays"].ToString();
                        model.MinWorkDaysRequired = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["MinWorkDaysRequired"]);

                        model.halfdayFulldayOff = DTWeekOffMasterDetail.Rows[0]["halfdayFulldayOff"].ToString();
                        model.MaxWorkingDaysReqForWeekOff = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["MaxWorkingDaysReqForWeekOff"]);
                        model.CompensatoryOffApplicable = DTWeekOffMasterDetail.Rows[0]["CompensatoryOffApplicable"].ToString();
                       
                        model.Remark = DTWeekOffMasterDetail.Rows[0]["Remark"].ToString();
                        model.OverrideForHolidays = DTWeekOffMasterDetail.Rows[0]["OverrideForHolidays"].ToString();
                        model.ExtraPayApplicable = DTWeekOffMasterDetail.Rows[0]["ExtraPayApplicable"].ToString();
                        model.EffectiveFrom = DTWeekOffMasterDetail.Rows[0]["EffectiveFrom"].ToString();
                        model.EntryByEmpId = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["EntryByEmpId"]);
                        model.EmpCategoryId = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["EmpCategoryId"]);
                        model.DeptId = Convert.ToInt32(DTWeekOffMasterDetail.Rows[0]["DeptId"]);
                        model.EntryByMachine = DTWeekOffMasterDetail.Rows[0]["EntryByMachine"].ToString();
                        model.EffectiveFrom = Convert.ToDateTime(DTWeekOffMasterDetail.Rows[0]["EffectiveFrom"]).ToString("dd/MM/yyyy");




                    }

                    
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

        internal async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "Delete"));
                SqlParams.Add(new SqlParameter("@WeekoffEntryId", ID));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPWeekoffMaster", SqlParams);
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
