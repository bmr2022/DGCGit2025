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
    public class HRLeaveMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _DataLogicDAL;
        //private readonly IConfiguration configuration;
        private dynamic? _ResponseResult;

        private IDataReader? Reader;

        public HRLeaveMasterDAL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            //configuration = config;
            DBConnectionString = config.GetConnectionString("eTactDB");
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetleaveType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "leaveType"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPLeaveMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetApprovalleval()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeApproverList"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPLeaveMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetLeaveCategory()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "LeaveCategory"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPLeaveMaster", SqlParams);
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
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPLeaveMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "EmployeeCategoryList";

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
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Tax"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Purchase order"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        internal async Task<DataSet> GetDepartment()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Department"));
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPLeaveMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "DepartmentList";

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

        internal async Task<DataSet> GetLocation()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeApproverList"));
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPLeaveMaster", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "LocationList";

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

        public async Task<ResponseResult> FillLeaveId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPLeaveMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveData(HRLeaveMasterModel model, List<string> HREmpCatDT, List<string> HRDeptCatDT)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPLeaveMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@LeaveEntryId", model.LeaveId);
                   
                    // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
                    oCmd.Parameters.AddWithValue("@LeaveCode", model.LeaveCode);
                    oCmd.Parameters.AddWithValue("@LeaveName", model.LeaveName);
                    oCmd.Parameters.AddWithValue("@LeaveType", model.LeaveType);
                    oCmd.Parameters.AddWithValue("@LeaveCategory", model.LeaveCategory);
                    oCmd.Parameters.AddWithValue("@Gender", model.GenderApplicable);
                    oCmd.Parameters.AddWithValue("@MaxleavePerYear", model.MaxLeavePerYear);
                    oCmd.Parameters.AddWithValue("@MinDayForApplication", model.MinDaysForApplication);
                    oCmd.Parameters.AddWithValue("@MaxConsecutiveDaysAllowed", model.MaxConsecutiveDaysAllowed);
                    oCmd.Parameters.AddWithValue("@Enachable", model.Encashable);
                    oCmd.Parameters.AddWithValue("@CarryForwad", model.CarryForward);
                    oCmd.Parameters.AddWithValue("@MaxCarryForwardLimit", model.MaxCarryForwardLimit);
                    oCmd.Parameters.AddWithValue("@HalfDayAllowed", model.HalfDayAllowed);
                    oCmd.Parameters.AddWithValue("@LeaveApprovalRequired", model.LeaveApprovalRequired);
                    oCmd.Parameters.AddWithValue("@LeaveDeductionApplicable", model.LeaveDeductionApplicable);
                    oCmd.Parameters.AddWithValue("@CompensatoryOffRequired", model.CompensatoryOffRequired);
                    oCmd.Parameters.AddWithValue("@EligibilityAfterMonths", model.EligibilityAfterMonths);
                    oCmd.Parameters.AddWithValue("@MinWorkDaysRequired", model.MinWorkDaysRequired);
                    oCmd.Parameters.AddWithValue("@AutoApproveLimitDays", model.AutoApproveLimitDays);
                    oCmd.Parameters.AddWithValue("@ApprovalLevel1", model.ApprovalLevel1);
                    oCmd.Parameters.AddWithValue("@ApprovalLevel2", model.ApprovalLevel2);
                    oCmd.Parameters.AddWithValue("@ApprovalLevel3", model.ApprovalLevel3);
                   
                    oCmd.Parameters.AddWithValue("@EffectiveFrom",
                string.IsNullOrEmpty(model.EffectiveFrom) ? DBNull.Value : DateTime.Parse(model.EffectiveFrom).ToString("dd/MMM/yyyy"));
                    oCmd.Parameters.AddWithValue("@CreatedByEmpid", model.CreatedBy);
                   
                    oCmd.Parameters.AddWithValue("@CreationDate",
                string.IsNullOrEmpty(model.CreatedOn) ? DBNull.Value : DateTime.Parse(model.CreatedOn).ToString("dd/MMM/yyyy"));
                    
                    string empCatCsv = string.Join(",", HREmpCatDT);
                    string deptCatCsv = string.Join(",", HRDeptCatDT);

                    
                    oCmd.Parameters.AddWithValue("@CatIdList", empCatCsv);
                    oCmd.Parameters.AddWithValue("@DepartIdList", deptCatCsv);


                    if (model.Mode == "UPDATE")
                    {
                        oCmd.Parameters.AddWithValue("@UpdatedBy", model.UpdatedBy);
                        oCmd.Parameters.AddWithValue("@UpdatedOnDate", string.IsNullOrEmpty(model.UpdatedOn) ? DBNull.Value : model.UpdatedOn);


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

                responseResult = await _DataLogicDAL.ExecuteDataSet("HRSPLeaveMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRLeaveMasterModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRLeaveMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPLeaveMaster", myConnection)
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
                    model.HRLeaveDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new HRLeaveMasterModel
                                               {
                                                   LeaveId = Convert.ToInt32(dr["LeaveEntryId"]),

                                                   LeaveCode = dr["LeaveCode"].ToString(),
                                                   LeaveName = dr["LeaveName"].ToString(),
                                                   LeaveType = dr["LeaveType"].ToString(),
                                                   LeaveCategory = dr["LeaveCategory"].ToString(),
                                               
                                                   GenderApplicable = dr["Gender"].ToString(),
                                                   MaxLeavePerYear = Convert.ToInt32(dr["MaxleavePerYear"]),
                                                   MinDaysForApplication = Convert.ToInt32(dr["MinDayForApplication"]),
                                                   MaxConsecutiveDaysAllowed = Convert.ToInt32(dr["MaxConsecutiveDaysAllowed"]),
                                                  
                                                   Encashable = dr["Enachable"].ToString(),
                                                   CarryForward = dr["CarryForwad"].ToString(),
                                                   MaxCarryForwardLimit = Convert.ToInt32(dr["MaxCarryForwardLimit"]),
                                                   HalfDayAllowed = dr["HalfDayAllowed"].ToString(),
                                                   LeaveApprovalRequired = dr["LeaveApprovalRequired"].ToString(),
                                                   LeaveDeductionApplicable = dr["LeaveDeductionApplicable"].ToString(),
                                                   EligibilityAfterMonths = Convert.ToInt32(dr["EligibilityAfterMonths"]),
                                                   CompensatoryOffRequired = dr["CompensatoryOffRequired"].ToString(),
                                                 
                                                   MinWorkDaysRequired = Convert.ToInt32(dr["MinWorkDaysRequired"]),
                                                   AutoApproveLimitDays = Convert.ToInt32(dr["AutoApproveLimitDays"]),
                                                   ApplicableDepartment = dr["ApplicableDepartment"].ToString(),
                                                   ApplicableOnCategory = dr["ApplicableOnCategory"].ToString(),
                                                   ApprovalLevel1 = Convert.ToInt32(dr["ApprovalLevel1"]),

                                                   ApprovalLevel2 = Convert.ToInt32(dr["ApprovalLevel2"]),
                                                   ApprovalLevel3 = Convert.ToInt32(dr["ApprovalLevel3"]),
                                                  //CreatedBy = Convert.ToInt32(dr["CreatedByEmpid"]),
                                                   UpdatedBy = Convert.ToInt32(dr["UpdatedBy"]),
                                                   EmpApprovalLevel1 = dr["Approve1EmpName"].ToString(),
                                                   EmpApprovalLevel2 = dr["Approve2EmpName"].ToString(),
                                                   CreatedByEmpName = dr["CreatedByEmpName"].ToString(),

                                                   EmpApprovalLevel3 = dr["Approve3EmpName"].ToString(),

                                                   EffectiveFrom = dr["EffectiveFrom"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["EffectiveFrom"]).ToString("dd-MM-yyyy"),
                                                   CreatedOn = dr["CreationDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["CreationDate"]).ToString("dd-MM-yyyy"),
                                                   UpdatedOn = dr["UpdatedOnDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["UpdatedOnDate"]).ToString("dd-MM-yyyy"),
                                                   

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


        public async Task<HRLeaveMasterModel> GetViewByID(int id)
        {
            var model = new HRLeaveMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@LeaveEntryId", id));

                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPLeaveMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTTaxMasterDetail = oDataSet.Tables[0];
                    var DEmpCategDetail = oDataSet.Tables[1];
                    var DDeptWiseCategDetail = oDataSet.Tables[2];

                    if (oDataSet.Tables.Count > 0 && DTTaxMasterDetail.Rows.Count > 0)
                    {
                        model.LeaveId = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["LeaveEntryId"]);
                      
                        model.LeaveCode = DTTaxMasterDetail.Rows[0]["LeaveCode"].ToString();
                        model.LeaveName = DTTaxMasterDetail.Rows[0]["LeaveName"].ToString();
                        model.LeaveType = DTTaxMasterDetail.Rows[0]["LeaveType"].ToString();
                        model.LeaveCategory = DTTaxMasterDetail.Rows[0]["LeaveCategory"].ToString();
                        model.GenderApplicable = DTTaxMasterDetail.Rows[0]["Gender"].ToString();
                        model.MaxLeavePerYear = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["MaxleavePerYear"]);
                        model.MinDaysForApplication = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["MinDayForApplication"]);
                        model.MaxConsecutiveDaysAllowed = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["MaxConsecutiveDaysAllowed"]);
                        model.Encashable = DTTaxMasterDetail.Rows[0]["Enachable"].ToString();
                        model.CarryForward = DTTaxMasterDetail.Rows[0]["CarryForwad"].ToString();
                        model.MaxCarryForwardLimit = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["MaxCarryForwardLimit"]);
                        model.HalfDayAllowed = DTTaxMasterDetail.Rows[0]["HalfDayAllowed"].ToString();
                        model.LeaveApprovalRequired = DTTaxMasterDetail.Rows[0]["LeaveApprovalRequired"].ToString();
                        model.LeaveDeductionApplicable = DTTaxMasterDetail.Rows[0]["LeaveDeductionApplicable"].ToString();
                        model.CompensatoryOffRequired = DTTaxMasterDetail.Rows[0]["CompensatoryOffRequired"].ToString();
                        model.EligibilityAfterMonths = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["EligibilityAfterMonths"]);
                        model.MinWorkDaysRequired = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["MinWorkDaysRequired"]);
                        model.AutoApproveLimitDays = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["AutoApproveLimitDays"]);
                        model.ApprovalLevel1 = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["ApprovalLevel1"]);
                        model.ApprovalLevel2 = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["ApprovalLevel2"]);
                        model.ApprovalLevel3 = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["ApprovalLevel3"]);
                        model.EffectiveFrom = Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["EffectiveFrom"]).ToString("dd/MM/yyyy");
                        model.CreatedOn = Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["CreationDate"]).ToString("dd/MM/yyyy");



                        model.CreatedByEmpName = DTTaxMasterDetail.Rows[0]["CreatedByEmpName"].ToString();


                        model.CreatedBy = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["CreatedByEmpid"]);
                       


                        //if (!string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString()))
                        //{
                        //    model.LastUpdatedOn = DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString();

                        //    model.LastUpdatedBy = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["LastUpdatedBy"]);
                        //    model.LastUpdatedOn = string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["LastUpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["LastUpdatedOn"]);
                        //}
                    }
                    if (oDataSet.Tables.Count > 0 && DEmpCategDetail.Rows.Count > 0)
                    {
                        DEmpCategDetail.TableName = "LeaveEmpCategDetail";
                        model.EmpCategDetailList = CommonFunc.DataTableToList<LeaveEmpCategDetail>(DEmpCategDetail);
                        model.RestrictedToEmployeeCategory = model.EmpCategDetailList.Select(x => x.CategoryId).ToList();
                    }

                    if (oDataSet.Tables.Count > 0 && DDeptWiseCategDetail.Rows.Count > 0)
                    {
                        DDeptWiseCategDetail.TableName = "LeaveDeptWiseCategDetail";
                        model.DeptWiseCategDetailList = CommonFunc.DataTableToList<LeaveDeptWiseCategDetail>(DDeptWiseCategDetail);
                       
                        model.RestrictedToDepartment = model.DeptWiseCategDetailList.Select(x => x.DeptId).ToList();
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
