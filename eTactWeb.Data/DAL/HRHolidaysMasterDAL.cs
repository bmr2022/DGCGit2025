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
    public class HRHolidaysMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private dynamic? _ResponseResult;

        public HRHolidaysMasterDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> GetHolidayType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "HolidayType"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHolidayMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetHolidayCountry()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Country"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHolidayMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetHolidayState()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "StateName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHolidayMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<DataSet> GetEmployeeCategory()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeCategory"));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHolidayMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ExemptedCategoriesList";

                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }

        internal async Task<DataSet> GetDepartment()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Department"));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHolidayMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ExemptedCategoriesList";

                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }

        public async Task<ResponseResult> FillEntryId(int yearcode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@HolidayYear", yearcode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHolidayMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveData(HRHolidaysMasterModel model, List<string> HREmployeeDT, List<string> HRDepartmentDT)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPHolidayMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@HolidayEntryId", model.HolidayId);

                    // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
                    oCmd.Parameters.AddWithValue("@BranchCC", model.Branch);

                    oCmd.Parameters.AddWithValue("@HolidayEffFrom",
                string.IsNullOrEmpty(model.EffectiveFrom) ? DBNull.Value : DateTime.Parse(model.EffectiveFrom).ToString("dd/MMM/yyyy"));
                    oCmd.Parameters.AddWithValue("@HolidayEffTill",
               string.IsNullOrEmpty(model.HolidayDate) ? DBNull.Value : DateTime.Parse(model.HolidayDate).ToString("dd/MMM/yyyy"));
                    oCmd.Parameters.AddWithValue("@Country", model.Country);
                    oCmd.Parameters.AddWithValue("@StateId", model.StateId);
                    oCmd.Parameters.AddWithValue("@StateName", model.State);
                    oCmd.Parameters.AddWithValue("@HolidayYear", model.HolidayYear);
                    oCmd.Parameters.AddWithValue("@HolidayName", model.HolidayName);
                    oCmd.Parameters.AddWithValue("@HolidayType", model.HolidayType);

                    oCmd.Parameters.AddWithValue("@HalfDayFullDay", model.HalfDayFullDay);
                    oCmd.Parameters.AddWithValue("@OverrideWeekOff", model.OverrideWeekOff);
                    oCmd.Parameters.AddWithValue("@CompaensatoryofAllowed", model.CompensatoryOffAllowed);
                    oCmd.Parameters.AddWithValue("@PaidHoliday", model.PaidHoliday);
                    oCmd.Parameters.AddWithValue("@Remark", model.Remark);
                    oCmd.Parameters.AddWithValue("@Active", model.Active);
                    oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    oCmd.Parameters.AddWithValue("@EntryByMachine", model.EntryByMachine);
                    oCmd.Parameters.AddWithValue("@ActualEntryOn",
                    string.IsNullOrEmpty(model.CreatedOn) ? DBNull.Value : DateTime.Parse(model.CreatedOn).ToString("dd/MMM/yyyy"));
                    string Empcat = string.Join(",", HREmployeeDT);
                    oCmd.Parameters.AddWithValue("@CategoryList", Empcat);

                    string department = string.Join(",", HRDepartmentDT);
                    oCmd.Parameters.AddWithValue("@DepartmentList", department);



                    if (model.Mode == "Update")
                    {
                        oCmd.Parameters.AddWithValue("@LastupdatedBy", model.UpdatedBy);
                        oCmd.Parameters.AddWithValue("@UpdatationDate",
                        string.IsNullOrEmpty(model.UpdatedOn) ? DBNull.Value : DateTime.Parse(model.UpdatedOn).ToString("dd/MMM/yyyy"));

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

        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _IDataLogic.ExecuteDataSet("HRSPHolidayMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRHolidaysMasterModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRHolidaysMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPHolidayMaster", myConnection)
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
                    model.HRHolidayDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new HRHolidaysMasterModel
                                              {
                                                 
                                                  Branch = dr["BranchCC"].ToString(),
                                                  HolidayYear = Convert.ToInt32(dr["HolidayYear"]),
                                                  EffectiveFrom = dr["HolidayEffFrom"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["HolidayEffFrom"]).ToString("dd-MM-yyyy"),
                                                  HolidayDate = dr["HolidayEffTill"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["HolidayEffTill"]).ToString("dd-MM-yyyy"),

                                                  HolidayName = dr["HolidayName"].ToString(),


                                                  HolidayType = dr["HolidayType"].ToString(),
                                                  HalfDayFullDay = dr["HalfDayFullDay"].ToString(),
                                                  Country = dr["Country"].ToString(),
                                                  State = dr["StateName"].ToString(),
                                                  OverrideWeekOff = dr["OverrideWeekOff"].ToString(),
                                                  CompensatoryOffAllowed = dr["CompaensatoryofAllowed"].ToString(),
                                                  PaidHoliday = dr["PaidHoliday"].ToString(),

                                                  ApplicableOnDepartment = dr["ApplicableOnDepartment"].ToString(),
                                                  ApplicableOnCategory = dr["ApplicableOnCategory"].ToString(),
                                                  //CreatedBy = Convert.ToInt32(dr["ActualEntryBy"]),
                                                  //UpdatedBy = Convert.ToInt32(dr["LastupdatedBy"]),
                                                  //CreatedOn = dr["ActualEntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd-MM-yyyy"),
                                                  //UpdatedOn = dr["UpdatationDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["UpdatationDate"]).ToString("dd-MM-yyyy"),
                                                  EntryByMachine = dr["EntryByMachine"].ToString(),
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

        public async Task<HRHolidaysMasterModel> GetViewByID(int id)
        {
            var model = new HRHolidaysMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@HolidayEntryId", id));
                SqlParams.Add(new SqlParameter("@HolidayYear", model.HolidayYear));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPHolidayMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTHolidaysMasterDetail = oDataSet.Tables[0];
                    var DEmployeeDetail = oDataSet.Tables[1];
                    var DDepartmentDetail = oDataSet.Tables[2];

                    if (oDataSet.Tables.Count > 0 && DTHolidaysMasterDetail.Rows.Count > 0)
                    {
                        model.HolidayId = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["HolidayEntryId"]);
                        model.Branch = DTHolidaysMasterDetail.Rows[0]["BranchCC"].ToString();
                        model.EffectiveFrom = Convert.ToDateTime(DTHolidaysMasterDetail.Rows[0]["HolidayEffFrom"]).ToString("dd/MM/yyyy");
                        model.HolidayDate = Convert.ToDateTime(DTHolidaysMasterDetail.Rows[0]["HolidayEffTill"]).ToString("dd/MM/yyyy");
                        model.EffectiveFrom = Convert.ToDateTime(DTHolidaysMasterDetail.Rows[0]["HolidayEffFrom"]).ToString("dd/MM/yyyy");

                        model.StateId = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["StateId"]);
                        model.Country = DTHolidaysMasterDetail.Rows[0]["Country"].ToString();
                        model.State = DTHolidaysMasterDetail.Rows[0]["StateName"].ToString();
                        model.HolidayYear = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["HolidayYear"]);
                        model.HolidayName = DTHolidaysMasterDetail.Rows[0]["HolidayName"].ToString();

                        model.HolidayType = DTHolidaysMasterDetail.Rows[0]["HolidayType"].ToString();
                        model.HalfDayFullDay = DTHolidaysMasterDetail.Rows[0]["HalfDayFullDay"].ToString();
                        model.OverrideWeekOff = DTHolidaysMasterDetail.Rows[0]["OverrideWeekOff"].ToString();
                        model.CompensatoryOffAllowed = DTHolidaysMasterDetail.Rows[0]["CompaensatoryofAllowed"].ToString();
                        model.PaidHoliday = DTHolidaysMasterDetail.Rows[0]["PaidHoliday"].ToString();
                        model.Remark = DTHolidaysMasterDetail.Rows[0]["Remark"].ToString();
                        model.Active = DTHolidaysMasterDetail.Rows[0]["Active"].ToString();
                        model.EntryByMachine = DTHolidaysMasterDetail.Rows[0]["EntryByMachine"].ToString(); 

                        model.CreatedBy = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["CreatedBy"]);
                        model.UpdatedBy = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["UpdatedBy"]);
                        model.Remark = DTHolidaysMasterDetail.Rows[0]["Remark"].ToString();



                       
                    }
                    if (oDataSet.Tables.Count > 0 && DEmployeeDetail.Rows.Count > 0)
                    {
                        DEmployeeDetail.TableName = "LeaveEmpCategDetail";
                        model.EmployeeCategoryDetailList = CommonFunc.DataTableToList<HolidayEmployeeCategoryDetail>(DEmployeeDetail);
                        model.EmployeeCategory = model.EmployeeCategoryDetailList.Select(x => x.CategoryId).ToList();
                    }

                    if (oDataSet.Tables.Count > 0 && DDepartmentDetail.Rows.Count > 0)
                    {
                        DDepartmentDetail.TableName = "LeaveDeptWiseCategDetail";
                        model.DepartmentDetailList = CommonFunc.DataTableToList<HoliDayDepartmentDetail>(DDepartmentDetail);

                        model.Department = model.DepartmentDetailList.Select(x => x.DeptId).ToList();
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

    }
}
