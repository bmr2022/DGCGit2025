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
    public class HRPFESIMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _DataLogicDAL;
        //private readonly IConfiguration configuration;
        private dynamic? _ResponseResult;

        private IDataReader? Reader;

        public HRPFESIMasterDAL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            //configuration = config;
            DBConnectionString = config.GetConnectionString("eTactDB");
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetESIDispensary()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "ESIBenifitCoverd"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPPFESIMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPPFESIMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        internal async Task<DataSet> GetExemptedCategories()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "ExemptedCategories"));
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPPFESIMaster", SqlParams);
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

        public async Task<ResponseResult> SaveData(HRPFESIMasterModel model)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPPFESIMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@PFESIEntryId", model.EntryId);

                    // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
                    oCmd.Parameters.AddWithValue("@SchemeType", model.SchemeType);

                    oCmd.Parameters.AddWithValue("@EffectiveFrom",
                string.IsNullOrEmpty(model.EffectiveFrom) ? DBNull.Value : DateTime.Parse(model.EffectiveFrom).ToString("dd/MMM/yyyy"));
                    oCmd.Parameters.AddWithValue("@EmployeePFContributionPercent", model.EmployeePFDeducionPer);
                    oCmd.Parameters.AddWithValue("@EmployerPFContributionPercent", model.EmployerPFContributionPer);
                    oCmd.Parameters.AddWithValue("@EmployerEPSContributionPercent", model.EmployerEPSContributionPer);
                    oCmd.Parameters.AddWithValue("@PFWageLimit", model.PFWageLimit);
                    oCmd.Parameters.AddWithValue("@MinServicePeriodForService", model.MinServicePeriodForPF);
                    oCmd.Parameters.AddWithValue("@PFAccountnumberFormat", model.PFAccountNumberFormat);
                   
                    oCmd.Parameters.AddWithValue("@EmployeeESIContributionPercent", model.EmployeeESIContributionPer);
                    oCmd.Parameters.AddWithValue("@EmployerESIContributionPercent", model.EmployerESIContributionPer);
                    oCmd.Parameters.AddWithValue("@ESIWageLimit", model.ESIWageLimit);
                    oCmd.Parameters.AddWithValue("@ESICode", model.ESICode);
                    oCmd.Parameters.AddWithValue("@ESIDispensary", model.ESIDispensary);
                    oCmd.Parameters.AddWithValue("@TaxDeductionApplicable", model.TaxDeductionApplicable);
                    oCmd.Parameters.AddWithValue("@VoluntaryPFAllowed", model.VoluntaryPFAllowed);
                    oCmd.Parameters.AddWithValue("@PFWithdrawLockPeriod", model.PFWithdrawalLockPeriod);
                    oCmd.Parameters.AddWithValue("@ESIBenifitsCovered", model.ESIBenefitsCovered);
                    oCmd.Parameters.AddWithValue("@ActualEntryBy", model.CreatedBy);
                    oCmd.Parameters.AddWithValue("@MachineName", model.EntryByMachine);
                    oCmd.Parameters.AddWithValue("@MaxCompanyLimitAmt", model.MaxCompanyLimitAmt);
                    oCmd.Parameters.AddWithValue("@PFBankName", model.PFBankName);
                    oCmd.Parameters.AddWithValue("@PFBankAddress", model.PFBankAddress);
                    oCmd.Parameters.AddWithValue("@PFRegistationNo", model.PFRegistationNo);


                    oCmd.Parameters.AddWithValue("@ActualEntryDate",
                string.IsNullOrEmpty(model.CreatedOn) ? DBNull.Value : DateTime.Parse(model.CreatedOn).ToString("dd/MMM/yyyy"));
                  
                
                    if (model.Mode == "UPDATE")
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

                responseResult = await _DataLogicDAL.ExecuteDataSet("HRSPPFESIMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRPFESIMasterModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRPFESIMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPPFESIMaster", myConnection)
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
                    model.HRPFESIDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new HRPFESIMasterModel
                                              {
                                                  EntryId = Convert.ToInt32(dr["PFESIEntryId"]),
                                                  SchemeType = dr["SchemeType"].ToString(),
                                                  EffectiveFrom = dr["EffectiveFrom"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["EffectiveFrom"]).ToString("dd-MM-yyyy"),
                                                  EmployeePFDeducionPer = Convert.ToDecimal(dr["EmployeePFContributionPercent"]),
                                                  EmployerPFContributionPer = Convert.ToDecimal(dr["EmployerPFContributionPercent"]),
                                                  EmployerEPSContributionPer = Convert.ToDecimal(dr["EmployerEPSContributionPercent"]),
                                                  PFWageLimit = Convert.ToDecimal(dr["PFWageLimit"]),
                                                  MinServicePeriodForPF = Convert.ToInt32(dr["MinServicePeriodForService"]),
                                                  PFAccountNumberFormat = dr["PFAccountnumberFormat"].ToString(),
                                                  EmployeeESIContributionPer = Convert.ToInt32(dr["EmployeeESIContributionPercent"]),
                                                  EmployerESIContributionPer = Convert.ToInt32(dr["EmployerESIContributionPercent"]),
                                                  ESIWageLimit = Convert.ToInt32(dr["ESIWageLimit"]),
                                                  ESICode = dr["ESICode"].ToString(),
                                                  ESIDispensary = dr["ESIDispensary"].ToString(),
                                                  TaxDeductionApplicable = dr["TaxDeductionApplicable"].ToString(),                                                 
                                                  VoluntaryPFAllowed = dr["VoluntaryPFAllowed"].ToString(),
                                                  PFWithdrawalLockPeriod = Convert.ToInt32(dr["PFWithdrawLockPeriod"]),
                                                  ESIBenefitsCovered = dr["ESIBenifitsCovered"].ToString(),                                                
                                                  CreatedBy = Convert.ToInt32(dr["ActualEntryBy"]),                                               
                                                  UpdatedBy = Convert.ToInt32(dr["LastupdatedBy"]),                                          
                                                  CreatedOn = dr["ActualEntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ActualEntryDate"]).ToString("dd-MM-yyyy"),
                                                  UpdatedOn = dr["UpdatationDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["UpdatationDate"]).ToString("dd-MM-yyyy"),
                                                  EntryByMachine = dr["MachineName"].ToString(),
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

        public async Task<HRPFESIMasterModel> GetViewByID(int id)
        {
            var model = new HRPFESIMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@PFESIEntryId", id));

                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPPFESIMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTPFESIMasterDetail = oDataSet.Tables[0];
                    //var DEmpCategDetail = oDataSet.Tables[1];
                    //var DDeptWiseCategDetail = oDataSet.Tables[2];

                    if (oDataSet.Tables.Count > 0 && DTPFESIMasterDetail.Rows.Count > 0)
                    {
                        model.EntryId = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["PFESIEntryId"]);
                        model.SchemeType = DTPFESIMasterDetail.Rows[0]["SchemeType"].ToString();
                        model.EffectiveFrom = Convert.ToDateTime(DTPFESIMasterDetail.Rows[0]["EffectiveFrom"]).ToString("dd/MM/yyyy");
                        model.EmployeePFDeducionPer = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["EmployeePFContributionPercent"]);
                        model.EmployerPFContributionPer = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["EmployerPFContributionPercent"]);
                        model.EmployerEPSContributionPer = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["EmployerEPSContributionPercent"]);
                        model.PFWageLimit = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["PFWageLimit"]);
                        model.MinServicePeriodForPF = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["MinServicePeriodForService"]);
                        model.PFAccountNumberFormat = DTPFESIMasterDetail.Rows[0]["PFAccountnumberFormat"].ToString();
                        model.EmployeeESIContributionPer = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["EmployeeESIContributionPercent"]);
                        model.EmployerESIContributionPer = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["EmployerESIContributionPercent"]);
                        model.ESIWageLimit = Convert.ToDecimal(DTPFESIMasterDetail.Rows[0]["ESIWageLimit"]);
                        model.ESICode = DTPFESIMasterDetail.Rows[0]["ESICode"].ToString();
                        model.ESIDispensary = DTPFESIMasterDetail.Rows[0]["ESIDispensary"].ToString();
                        model.TaxDeductionApplicable = DTPFESIMasterDetail.Rows[0]["TaxDeductionApplicable"].ToString();
                        model.VoluntaryPFAllowed = DTPFESIMasterDetail.Rows[0]["VoluntaryPFAllowed"].ToString();
                        model.PFWithdrawalLockPeriod = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["PFWithdrawLockPeriod"]);
                        model.EntryByMachine = DTPFESIMasterDetail.Rows[0]["MachineName"].ToString();
                        model.ESIBenefitsCovered = DTPFESIMasterDetail.Rows[0]["ESIBenifitsCovered"].ToString();
                        model.MaxCompanyLimitAmt = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["MaxCompanyLimitAmt"]);
                        model.PFBankName = DTPFESIMasterDetail.Rows[0]["ESIBenifitsCovered"].ToString();
                        model.PFBankAddress = DTPFESIMasterDetail.Rows[0]["ESIBenifitsCovered"].ToString();
                        model.PFRegistationNo = DTPFESIMasterDetail.Rows[0]["ESIBenifitsCovered"].ToString();
                        model.CreatedBy = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["ActualEntryBy"]);         
                        //model.UpdatedBy = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["LastupdatedBy"]);
                        //model.UpdatedOn = Convert.ToDateTime(DTPFESIMasterDetail.Rows[0]["UpdatationDate"]).ToString("dd/MM/yyyy");
                        model.CreatedOn = Convert.ToDateTime(DTPFESIMasterDetail.Rows[0]["ActualEntryDate"]).ToString("dd/MM/yyyy");



                       


                        


                        //if (!string.IsNullOrEmpty(DTPFESIMasterDetail.Rows[0]["UpdatedByName"].ToString()))
                        //{
                        //    model.LastUpdatedOn = DTPFESIMasterDetail.Rows[0]["UpdatedByName"].ToString();

                        //    model.LastUpdatedBy = Convert.ToInt32(DTPFESIMasterDetail.Rows[0]["LastUpdatedBy"]);
                        //    model.LastUpdatedOn = string.IsNullOrEmpty(DTPFESIMasterDetail.Rows[0]["LastUpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DTPFESIMasterDetail.Rows[0]["LastUpdatedOn"]);
                        //}
                    }
                    //if (oDataSet.Tables.Count > 0 && DEmpCategDetail.Rows.Count > 0)
                    //{
                    //    DEmpCategDetail.TableName = "LeaveEmpCategDetail";
                    //    model.EmpCategDetailList = CommonFunc.DataTableToList<LeaveEmpCategDetail>(DEmpCategDetail);
                    //    model.RestrictedToEmployeeCategory = model.EmpCategDetailList.Select(x => x.CategoryId).ToList();
                    //}

                    //if (oDataSet.Tables.Count > 0 && DDeptWiseCategDetail.Rows.Count > 0)
                    //{
                    //    DDeptWiseCategDetail.TableName = "LeaveDeptWiseCategDetail";
                    //    model.DeptWiseCategDetailList = CommonFunc.DataTableToList<LeaveDeptWiseCategDetail>(DDeptWiseCategDetail);

                    //    model.RestrictedToDepartment = model.DeptWiseCategDetailList.Select(x => x.DeptId).ToList();
                    //}


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

        internal async Task<ResponseResult> DeleteByID(int ID,string machineName)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "Delete"));
                SqlParams.Add(new SqlParameter("@PFESIEntryId", ID));
                SqlParams.Add(new SqlParameter("@MachineName", machineName));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPPFESIMaster", SqlParams);
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
