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
    }
}
