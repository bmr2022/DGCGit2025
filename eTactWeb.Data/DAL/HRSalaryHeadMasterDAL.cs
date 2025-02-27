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
    public class HRSalaryHeadMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _DataLogicDAL;
        //private readonly IConfiguration configuration;
        private dynamic? _ResponseResult;

        private IDataReader? Reader;

        public HRSalaryHeadMasterDAL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            //configuration = config;
            DBConnectionString = config.GetConnectionString("eTactDB");
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTypeofSalaryHead()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "TypeofSalaryHead"));
                
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetSalaryCalculationType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "SalaryCalculationType"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPartOf()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Partof"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetSalaryPaymentMode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "SalaryPaymentMode"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetCurrency()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Currency"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAmountPercentageOfCalculation()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "AmountPercentageOfCalculation"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> ChkForDuplicateHeadName(string SalaryHead,int SalHeadEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "CheckDuplicateHeadName"));
                SqlParams.Add(new SqlParameter("@SalaryHead", SalaryHead));
                SqlParams.Add(new SqlParameter("@SalHeadEntryId", SalHeadEntryId));
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> CheckBeforeDelete(int SalHeadEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "CheckBeforeDelete"));
               
                SqlParams.Add(new SqlParameter("@SalHeadEntryId", SalHeadEntryId));
                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetRoundOff()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "RoundOff"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetPaymentFrequency()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "PaymentFrequency"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDeductionOfTax()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DeductionOfTax"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
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
                SqlParams.Add(new SqlParameter("@flag", "FillEmployeeCategory"));
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPSalaryHeadMaster", SqlParams);
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

        internal async Task<DataSet> GetDepartment()
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillDepartment"));
                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPSalaryHeadMaster", SqlParams);
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

        //internal async Task<IList<HRSalaryHeadMasterModel>> GetDashBoardData()
        //{
        //    var DashBoard = new List<HRSalaryHeadMasterModel>();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();
        //        SqlParams.Add(new SqlParameter("@flag", "DASHBOARD"));
        //        var _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
        //        if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
        //        {
        //            DataTable DT = _ResponseResult.Result.DefaultView.ToTable(true, "SalHeadEffectiveDate", "SalaryHead", "SalaryCode", "ShortForm", "TypeOfSalary", "PartOfGrossBasic", "MinAmount", "MaxAmount", "PaymentMode", "CalculationType", "PercentageOfSalaryHeadID", "CalculationPercentage", "CalculationFormula", "RoundOffMethod", "FrequencyOfPayment", "CarryForward", "MaxAmountofCarryForward", "EmployerContribution", "ContributionSalaryHead", "ContributionPercentage", "ContributionAmount", "PFApplicable", "ESIApplicable", "IncomeTaxApplicable", "Taxpercentage", "TaxAmount", "DeductionMorY", "Currency", "ActiveStatus", "DisplayOrder", "ActualEntryby", "LastUpdatedOn", "PartOfPayslip", "EntryByMachine", "LastUpdatedBy", "Remarks", "SalHeadEntryId", "ContributionPerOfSalaryHeadId", "SalHeadEntryDate", "CurrencyId", "ApplicableOnCategory", "ApplicableOnDeparyment");
        //            DT.TableName = "HRSALARYHEADMASTER";

        //            DashBoard = CommonFunc.DataTableToList<HRSalaryHeadMasterModel>(DT);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return DashBoard;
        //}



        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _DataLogicDAL.ExecuteDataSet("HRSPSalaryHeadMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRSalaryHeadMasterModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRSalaryHeadMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPSalaryHeadMaster", myConnection)
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
                    model.HRSalaryDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                              select new HRSalaryHeadMasterModel
                                                              {
                                                                  SalHeadEffectiveDate = dr["SalHeadEffectiveDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["SalHeadEffectiveDate"]).ToString("dd-MM-yyyy"),
                                                                  SalaryHead = dr["SalaryHead"].ToString(),
                                                                
                                                                  SalaryCode = dr["SalaryCode"].ToString(),
                                                                  ShortForm = dr["ShortForm"].ToString(),
                                                                  TypeOfSalary = dr["TypeOfSalary"].ToString(),
                                                                  PartOfGrossBasic = dr["PartOfGrossBasic"].ToString(),
                                                                  MinAmount = Convert.ToInt32(dr["MinAmount"]),
                                                                  MaxAmount = Convert.ToInt32(dr["MaxAmount"]),


                                                                  SalaryPaymentMode = dr["PaymentMode"].ToString(),
                                                                  CalculationType = dr["CalculationType"].ToString(),
                                                                  PercentageOfSalaryHeadID = dr["PercentageOfSalaryHeadID"].ToString(),
                                                                  CalculationPercentage = Convert.ToInt32(dr["CalculationPercentage"]),
                                                                  CalculationFormula = dr["CalculationFormula"].ToString(),
                                                                  RoundOffMethod = dr["RoundOffMethod"].ToString(),
                                                                  FrequencyOfPayment = dr["FrequencyOfPayment"].ToString(),
                                                                  CarryForward = dr["CarryForward"].ToString(),
                                                                  MaxAmountofCarryForward = Convert.ToInt32(dr["MaxAmountofCarryForward"]),
                                                                  EmployerContribution = dr["EmployerContribution"].ToString(),
                                                                  ContributionSalaryHead = dr["ContributionSalaryHead"].ToString(),
                                                                  ContributionPercentage = Convert.ToInt32(dr["ContributionPercentage"]),
                                                                  ContributionAmount = Convert.ToInt32(dr["ContributionAmount"]),
                                                                  PFApplicable = dr["PFApplicable"].ToString(),
                                                                  ESIApplicable = dr["ESIApplicable"].ToString(),
                                                                  IncomeTaxApplicable = dr["IncomeTaxApplicable"].ToString(),
                                                                  Taxpercentage = Convert.ToInt32(dr["Taxpercentage"]),
                                                                  TaxAmount = Convert.ToInt32(dr["TaxAmount"]),
                                                                  DeductionMorY = dr["DeductionMorY"].ToString(),
                                                                  ApplicableOnCategory = dr["ApplicableOnCategory"].ToString(),
                                                                  ApplicableOnDeparyment = dr["ApplicableOnDeparyment"].ToString(),

                                                                  Currency = dr["Currency"].ToString(),
                                                                  ActiveStatus = dr["ActiveStatus"].ToString(),

                                                                  DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]),
                                                                  ActualEntryby = Convert.ToInt32(dr["ActualEntryby"]),
                                                                  LastUpdatedOn = dr["LastUpdatedOn"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["LastUpdatedOn"]).ToString("dd-MM-yyyy"),
                                                                  SalHeadEntryDate = dr["SalHeadEntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["SalHeadEntryDate"]).ToString("dd-MM-yyyy"),
                                                                  PartOfPayslip = dr["PartOfPayslip"].ToString(),
                                                                  EntryByMachine = dr["EntryByMachine"].ToString(),
                                                                  AmountOrPercentage = dr["AmountPercentage"].ToString(),
                                                                  LastUpdatedBy = Convert.ToInt32(dr["LastUpdatedBy"]),
                                                                  Remarks = dr["Remarks"].ToString(),
                                                                  SalHeadEntryId = Convert.ToInt32(dr["SalHeadEntryId"]),
                                                                  ContributionPerOfSalaryHeadId = Convert.ToInt32(dr["ContributionPerOfSalaryHeadId"]),

                                                                  CurrencyId = dr["CurrencyId"].ToString(),
                                                                  //EmpCateg = dr["CurrencyId"].ToList(),

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

        public async Task<HRSalaryHeadMasterModel> GetViewByID(int SalHeadEntryId)
        {
            var model = new HRSalaryHeadMasterModel();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@SalHeadEntryId", SalHeadEntryId));

                var _ResponseResult = await _DataLogicDAL.ExecuteDataSet("HRSPSalaryHeadMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    var oDataSet = new DataSet();
                    oDataSet = _ResponseResult.Result;
                    var DTTaxMasterDetail = oDataSet.Tables[0];
                    var DEmpCategDetail = oDataSet.Tables[1];
                    var DDeptWiseCategDetail = oDataSet.Tables[2];

                    if (oDataSet.Tables.Count > 0 && DTTaxMasterDetail.Rows.Count > 0)
                    {
                        model.SalHeadEffectiveDate = DTTaxMasterDetail.Rows[0]["SalHeadEffectiveDate"].ToString(); 
                        model.SalaryHead = DTTaxMasterDetail.Rows[0]["SalaryHead"].ToString();
                        model.SalaryCode = DTTaxMasterDetail.Rows[0]["SalaryCode"].ToString();
                        model.ShortForm = DTTaxMasterDetail.Rows[0]["ShortForm"].ToString();
                        model.TypeOfSalary = DTTaxMasterDetail.Rows[0]["TypeOfSalary"].ToString();
                        model.PartOfGrossBasic = DTTaxMasterDetail.Rows[0]["PartOfGrossBasic"].ToString();
                        model.MinAmount = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["MinAmount"]);
                        model.MaxAmount = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["MaxAmount"]);
                        model.SalaryPaymentMode = DTTaxMasterDetail.Rows[0]["PaymentMode"].ToString();
                        model.CalculationType = DTTaxMasterDetail.Rows[0]["CalculationType"].ToString();
                        model.PercentageOfSalaryHeadID = DTTaxMasterDetail.Rows[0]["PercentageOfSalaryHeadID"].ToString();
                        model.CalculationPercentage = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["CalculationPercentage"]);
                        model.CalculationFormula = DTTaxMasterDetail.Rows[0]["CalculationFormula"].ToString();
                        model.RoundOffMethod = DTTaxMasterDetail.Rows[0]["RoundOffMethod"].ToString();
                        model.FrequencyOfPayment = DTTaxMasterDetail.Rows[0]["FrequencyOfPayment"].ToString();
                        model.CarryForward = DTTaxMasterDetail.Rows[0]["CarryForward"].ToString();
                        model.MaxAmountofCarryForward = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["MaxAmountofCarryForward"]);
                        model.EmployerContribution = DTTaxMasterDetail.Rows[0]["EmployerContribution"].ToString();
                        model.ContributionSalaryHead = DTTaxMasterDetail.Rows[0]["ContributionSalaryHead"].ToString();
                        model.ContributionPercentage = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["ContributionPercentage"]);
                        model.ContributionAmount = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["ContributionAmount"]);
                        model.PFApplicable = DTTaxMasterDetail.Rows[0]["PFApplicable"].ToString();
                        model.ESIApplicable = DTTaxMasterDetail.Rows[0]["ESIApplicable"].ToString();
                        model.IncomeTaxApplicable = DTTaxMasterDetail.Rows[0]["IncomeTaxApplicable"].ToString();
                        model.Taxpercentage = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["Taxpercentage"]);
                        model.TaxAmount = Convert.ToDecimal(DTTaxMasterDetail.Rows[0]["TaxAmount"]);
                        model.DeductionMorY = DTTaxMasterDetail.Rows[0]["DeductionMorY"].ToString();
                        model.Currency = DTTaxMasterDetail.Rows[0]["Currency"].ToString();
                        model.ActiveStatus = DTTaxMasterDetail.Rows[0]["ActiveStatus"].ToString();
                        model.DisplayOrder = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["DisplayOrder"]);
                        model.PartOfPayslip = DTTaxMasterDetail.Rows[0]["PartOfPayslip"].ToString();
                        model.EntryByMachine = DTTaxMasterDetail.Rows[0]["EntryByMachine"].ToString();
                        model.Remarks = DTTaxMasterDetail.Rows[0]["Remarks"].ToString();
                        model.SalHeadEntryId = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["SalHeadEntryId"]);
                        model.ContributionPerOfSalaryHeadId = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["ContributionPerOfSalaryHeadId"]);
                        model.SalHeadEntryDate = Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["SalHeadEntryDate"]).ToString("yy/mm/dd");
                        model.CurrencyId = DTTaxMasterDetail.Rows[0]["CurrencyId"].ToString();
                        model.AmountOrPercentage = DTTaxMasterDetail.Rows[0]["AmountPercentage"].ToString();


                        //if (!string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString()))
                        //{
                        //    model.LastUpdatedOn = DTTaxMasterDetail.Rows[0]["UpdatedByName"].ToString();

                        //    model.LastUpdatedBy = Convert.ToInt32(DTTaxMasterDetail.Rows[0]["LastUpdatedBy"]);
                        //    model.LastUpdatedOn = string.IsNullOrEmpty(DTTaxMasterDetail.Rows[0]["LastUpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DTTaxMasterDetail.Rows[0]["LastUpdatedOn"]);
                        //}
                    }

                    if (oDataSet.Tables.Count > 0 && DEmpCategDetail.Rows.Count > 0)
                    {
                        DEmpCategDetail.TableName = "EmpCategDetail1";
                        model.EmpCategDetailList1 = CommonFunc.DataTableToList<EmpCategDetail1>(DEmpCategDetail);
                        model.EmpCateg = model.EmpCategDetailList1.Select(x => x.CategoryId).ToList();
                    }

                    if (oDataSet.Tables.Count > 0 && DDeptWiseCategDetail.Rows.Count > 0)
                    {
                        DDeptWiseCategDetail.TableName = "DeptWiseCategDetail1";
                        model.DeptWiseCategDetailList1 = CommonFunc.DataTableToList<DeptWiseCategDetail1>(DDeptWiseCategDetail);
                        model.DeptName = model.DeptWiseCategDetailList1.Select(x => x.DeptId).ToList();
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

        public async Task<ResponseResult> SaveData(HRSalaryHeadMasterModel model, DataTable HRSalaryMasterDT, DataTable HRSalaryMasterDeptWiseDT)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPSalaryHeadMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@SalHeadEntryId", model.SalHeadEntryId);
                    oCmd.Parameters.AddWithValue("@SalHeadEntryDate", string.IsNullOrEmpty(model.SalHeadEntryDate) ? DBNull.Value : model.SalHeadEntryDate);
                   // oCmd.Parameters.AddWithValue("@SalHeadEntryDate", model.SalHeadEntryDate);
                    oCmd.Parameters.AddWithValue("@SalHeadEffectiveDate", model.SalHeadEffectiveDate);
                    oCmd.Parameters.AddWithValue("@SalaryHead", model.SalaryHead);
                    oCmd.Parameters.AddWithValue("@SalaryCode", model.SalaryCode);
                    oCmd.Parameters.AddWithValue("@ShortForm", model.ShortForm);
                    oCmd.Parameters.AddWithValue("@TypeOfSalary", model.TypeOfSalary);
                    oCmd.Parameters.AddWithValue("@PartOfGrossBasic", model.PartOfGrossBasic);
                    oCmd.Parameters.AddWithValue("@CurrencyId", model.CurrencyId);
                    oCmd.Parameters.AddWithValue("@PaymentMode", model.SalaryPaymentMode);
                    oCmd.Parameters.AddWithValue("@CalculationType", model.CalculationType);
                    oCmd.Parameters.AddWithValue("@PercentageOfSalaryHeadID", model.PercentageOfSalaryHeadID);
                    oCmd.Parameters.AddWithValue("@CalculationPercentage", model.CalculationPercentage);
                    oCmd.Parameters.AddWithValue("@CalculationFormula", model.CalculationFormula);
                    oCmd.Parameters.AddWithValue("@RoundOffMethod", model.RoundOffMethod);
                    oCmd.Parameters.AddWithValue("@FrequencyOfPayment", model.FrequencyOfPayment);
                    oCmd.Parameters.AddWithValue("@CarryForward", model.CarryForward);
                    oCmd.Parameters.AddWithValue("@MaxAmountofCarryForward", model.MaxAmountofCarryForward);
                    oCmd.Parameters.AddWithValue("@EmployerContribution", model.EmployerContribution);
                    oCmd.Parameters.AddWithValue("@ContributionPerOfSalaryHeadId", model.ContributionPerOfSalaryHeadId);
                    oCmd.Parameters.AddWithValue("@ContributionPercentage", model.ContributionPercentage);
                    oCmd.Parameters.AddWithValue("@ContributionAmount", model.ContributionAmount);
                    oCmd.Parameters.AddWithValue("@PFApplicable", model.PFApplicable);
                    oCmd.Parameters.AddWithValue("@ESIApplicable", model.ESIApplicable);
                    oCmd.Parameters.AddWithValue("@IncomeTaxApplicable", model.IncomeTaxApplicable);
                    oCmd.Parameters.AddWithValue("@Taxpercentage", model.Taxpercentage);
                    oCmd.Parameters.AddWithValue("@TaxAmount", model.TaxAmount);
                    oCmd.Parameters.AddWithValue("@DeductionMorY", model.DeductionMorY);
                    oCmd.Parameters.AddWithValue("@ActiveStatus", model.ActiveStatus);
                    oCmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
                    oCmd.Parameters.AddWithValue("@Remarks", model.Remarks);
                    oCmd.Parameters.AddWithValue("@ActualEntryby", model.ActualEntryby);                  
                    oCmd.Parameters.AddWithValue("@EntryByMachine", model.EntryByMachine);
                    oCmd.Parameters.AddWithValue("@MinAmount", model.MinAmount);
                    oCmd.Parameters.AddWithValue("@MaxAmount", model.MaxAmount);
                    oCmd.Parameters.AddWithValue("@PartOfPayslip", model.PartOfPayslip);
                    oCmd.Parameters.AddWithValue("@AmountPercentage", model.AmountOrPercentage);
                    oCmd.Parameters.AddWithValue("@catDt", HRSalaryMasterDT);
                    oCmd.Parameters.AddWithValue("@DepDt", HRSalaryMasterDeptWiseDT);

                    if (model.Mode == "UPDATE")
                    {
                        oCmd.Parameters.AddWithValue("@LastUpdatedBy", model.LastUpdatedBy);
                        oCmd.Parameters.AddWithValue("@LastUpdatedOn", model.LastUpdatedOn);

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

        internal async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "Delete"));
                SqlParams.Add(new SqlParameter("@SalHeadEntryId", ID));

                _ResponseResult = await _DataLogicDAL.ExecuteDataTable("HRSPSalaryHeadMaster", SqlParams);
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
