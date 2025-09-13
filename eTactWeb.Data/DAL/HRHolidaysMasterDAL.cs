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
        private readonly ConnectionStringService _connectionStringService;
        public HRHolidaysMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
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
        //public async Task<ResponseResult> SaveData(HRHolidaysMasterModel model, List<string> HREmployeeDT, List<string> HRDepartmentDT)
        //{
        //    try
        //    {
        //        using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
        //        {
        //            SqlCommand oCmd = new SqlCommand("HRSPHolidayMaster", myConnection)
        //            {
        //                CommandType = CommandType.StoredProcedure
        //            };

        //            oCmd.Parameters.AddWithValue("@flag", model.Mode);
        //            oCmd.Parameters.AddWithValue("@HolidayEntryId", model.HolidayId);

        //            // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
        //            oCmd.Parameters.AddWithValue("@BranchCC", model.Branch);

        //            oCmd.Parameters.AddWithValue("@HolidayEffFrom",
        //        string.IsNullOrEmpty(model.EffectiveFrom) ? DBNull.Value : DateTime.Parse(model.EffectiveFrom).ToString("dd/MMM/yyyy"));
        //            oCmd.Parameters.AddWithValue("@HolidayEffTill",
        //       string.IsNullOrEmpty(model.HolidayEffTill) ? DBNull.Value : DateTime.Parse(model.HolidayEffTill).ToString("dd/MMM/yyyy"));
        //            oCmd.Parameters.AddWithValue("@Country", model.Country);
        //            oCmd.Parameters.AddWithValue("@StateId", model.StateId);
        //            oCmd.Parameters.AddWithValue("@StateName", model.StateName);
        //            oCmd.Parameters.AddWithValue("@HolidayYear", model.HolidayYear);
        //            oCmd.Parameters.AddWithValue("@HolidayName", model.HolidayName);
        //            oCmd.Parameters.AddWithValue("@HolidayType", model.HolidayType);

        //            oCmd.Parameters.AddWithValue("@HalfDayFullDay", model.HalfDayFullDay);
        //            oCmd.Parameters.AddWithValue("@OverrideWeekOff", model.OverrideWeekOff);
        //            oCmd.Parameters.AddWithValue("@CompaensatoryofAllowed", model.CompensatoryOffAllowed);
        //            oCmd.Parameters.AddWithValue("@PaidHoliday", model.PaidHoliday);
        //            oCmd.Parameters.AddWithValue("@Remark", model.Remark);
        //            oCmd.Parameters.AddWithValue("@Active", model.Active);
        //            oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
        //            oCmd.Parameters.AddWithValue("@EntryByMachine", model.EntryByMachine);
        //            oCmd.Parameters.AddWithValue("@ActualEntryOn",
        //            string.IsNullOrEmpty(model.CreatedOn) ? DBNull.Value : DateTime.Parse(model.CreatedOn).ToString("dd/MMM/yyyy"));
        //            oCmd.Parameters.AddWithValue("@EntryDate",
        //           string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : DateTime.Parse(model.EntryDate).ToString("dd/MMM/yyyy"));
        //            string Empcat = string.Join(",", HREmployeeDT);
        //            oCmd.Parameters.AddWithValue("@CategoryList", Empcat);

        //            string department = string.Join(",", HRDepartmentDT);
        //            oCmd.Parameters.AddWithValue("@DepartmentList", department);



        //            if (model.Mode == "update")
        //            {
        //                oCmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);
        //                oCmd.Parameters.AddWithValue("@UpdatedOn",model.UpdatedOn);

        //            }

        //            myConnection.Open();
        //            Reader = await oCmd.ExecuteReaderAsync();
        //            if (Reader != null)
        //            {
        //                while (Reader.Read())
        //                {
        //                    _ResponseResult = new ResponseResult()
        //                    {
        //                        StatusCode = (HttpStatusCode)Reader["StatusCode"],
        //                        StatusText = "Success",
        //                        Result = Reader["Result"].ToString()
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }
        //    finally
        //    {
        //        if (Reader != null)
        //        {
        //            Reader.Close();
        //            Reader.Dispose();
        //        }
        //    }

        //    return _ResponseResult;
        //}


        public async Task<ResponseResult> SaveData(HRHolidaysMasterModel model, List<string> HREmployeeDT, List<string> HRDepartmentDT)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var sqlParams = new List<dynamic>();

                
                if (model.Mode == "update" )
                {
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@HolidayEntryId", model.HolidayId));
                    sqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                    sqlParams.Add(new SqlParameter("@UpdatedOn", model.UpdatedOn ?? (object)DBNull.Value));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    sqlParams.Add(new SqlParameter("@HolidayEntryId", model.HolidayId));
                }

              
                sqlParams.Add(new SqlParameter("@BranchCC", model.Branch));

                sqlParams.Add(new SqlParameter("@HolidayEffFrom", model.EffectiveFrom));

                sqlParams.Add(new SqlParameter("@HolidayEffTill",model.HolidayEffTill));

                sqlParams.Add(new SqlParameter("@Country", model.Country));
                sqlParams.Add(new SqlParameter("@StateId", model.StateId));
                sqlParams.Add(new SqlParameter("@StateName", model.StateName));
                sqlParams.Add(new SqlParameter("@HolidayYear", model.HolidayYear));
                sqlParams.Add(new SqlParameter("@HolidayName", model.HolidayName));
                sqlParams.Add(new SqlParameter("@HolidayType", model.HolidayType));
                sqlParams.Add(new SqlParameter("@HalfDayFullDay", model.HalfDayFullDay));
                sqlParams.Add(new SqlParameter("@OverrideWeekOff", model.OverrideWeekOff));
                sqlParams.Add(new SqlParameter("@CompaensatoryofAllowed", model.CompensatoryOffAllowed));
                sqlParams.Add(new SqlParameter("@PaidHoliday", model.PaidHoliday));
                sqlParams.Add(new SqlParameter("@Remark", model.Remark ?? (object)DBNull.Value));
                sqlParams.Add(new SqlParameter("@Active", model.Active));
                sqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));
                sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));

                sqlParams.Add(new SqlParameter("@ActualEntryOn",
                    string.IsNullOrEmpty(model.CreatedOn) ? (object)DBNull.Value : DateTime.Parse(model.CreatedOn)));

                sqlParams.Add(new SqlParameter("@EntryDate",
                    string.IsNullOrEmpty(model.EntryDate) ? (object)DBNull.Value : DateTime.Parse(model.EntryDate)));

            
                string Empcat = string.Join(",", HREmployeeDT);
                sqlParams.Add(new SqlParameter("@CategoryList", Empcat));

                string department = string.Join(",", HRDepartmentDT);
                sqlParams.Add(new SqlParameter("@DepartmentList", department));

               
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPHolidayMaster", sqlParams);
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
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
        public async Task<HRHolidaysMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
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
                    oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@todate", ToDate);
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
                                                  HolidayId = Convert.ToInt32(dr["HolidayEntryId"]),
                                                  EffectiveFrom = dr["HolidayEffFrom"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["HolidayEffFrom"]).ToString("dd-MM-yyyy"),
                                                  HolidayEffTill = dr["HolidayEffTill"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["HolidayEffTill"]).ToString("dd-MM-yyyy"),

                                                  HolidayName = dr["HolidayName"].ToString(),


                                                  HolidayType = dr["HolidayType"].ToString(),
                                                  HalfDayFullDay = dr["HalfDayFullDay"].ToString(),
                                                  Country = dr["Country"].ToString(),
                                                  StateName = dr["StateName"].ToString(),
                                                  OverrideWeekOff = dr["OverrideWeekOff"].ToString(),
                                                  CompensatoryOffAllowed = dr["CompaensatoryofAllowed"].ToString(),
                                                  PaidHoliday = dr["PaidHoliday"].ToString(),

                                                  ApplicableOnDepartment = dr["ApplicableOnDepartment"].ToString(),
                                                  ApplicableOnCategory = dr["ApplicableOnCategory"].ToString(),
                                                  CreatedByEmp = dr["CreatedByEmpName"].ToString(),
                                                  UpdatedByEmp = dr["UpdatedByEmpName"].ToString(),
                                                  Remark = dr["Remark"].ToString(),
                                                  Active = dr["Active"].ToString(),
                                                  CreatedOn = dr["ActualEntryOn"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ActualEntryOn"]).ToString("dd-MM-yyyy"),
                                                  UpdatedOn = dr["UpdatedOn"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["UpdatedOn"]).ToString("dd-MM-yyyy"),
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

        public async Task<HRHolidaysMasterModel> GetViewByID(int id,int year)
        {
            var model = new HRHolidaysMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@HolidayEntryId", id));
                SqlParams.Add(new SqlParameter("@HolidayYear", year));

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
                        model.HolidayEffTill = Convert.ToDateTime(DTHolidaysMasterDetail.Rows[0]["HolidayEffTill"]).ToString("dd/MM/yyyy");
                        model.CreatedOn = Convert.ToDateTime(DTHolidaysMasterDetail.Rows[0]["ActualEntryOn"]).ToString("dd/MM/yyyy");

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
                        //model.UpdatedBy = Convert.ToInt32(DTHolidaysMasterDetail.Rows[0]["UpdatedBy"]);
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

        internal async Task<ResponseResult> DeleteByID(int ID, int year)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "Delete"));
                SqlParams.Add(new SqlParameter("@HolidayEntryId", ID));
                SqlParams.Add(new SqlParameter("@HolidayYear", year));

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

    }
}
