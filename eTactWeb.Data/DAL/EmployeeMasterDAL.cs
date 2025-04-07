using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class EmployeeMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public EmployeeMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Employee Master"));

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

        public async Task<EmployeeMasterModel> GetByID(int ID)
        {
            EmployeeMasterModel? _EmployeeMasterModel = new EmployeeMasterModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@empid", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "ViewByID");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }

                if (oDataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in oDataTable.Rows)
                    {
                        _EmployeeMasterModel.EmpId = Convert.ToInt32(dr["Emp_Id"].ToString());
                        _EmployeeMasterModel.Branch = dr["CC"].ToString();
                        _EmployeeMasterModel.EntryDate = dr["Entry_Date"].ToString();
                        _EmployeeMasterModel.EmpCode= dr["Emp_Code"].ToString();
                        _EmployeeMasterModel.Name = dr["Emp_Name"].ToString();
                        _EmployeeMasterModel.Designation = dr["DesigId"]?.ToString();
                        _EmployeeMasterModel.Department = dr["DeptId"]?.ToString();
                        _EmployeeMasterModel.Category = dr["CategoryId"]?.ToString();
                        _EmployeeMasterModel.DateOfJoining = dr["DateOfJoining"].ToString();
                        _EmployeeMasterModel.DOB = dr["DOB"].ToString();
                        _EmployeeMasterModel.Shift = dr["ShiftId"].ToString();
                        _EmployeeMasterModel.Active = dr["Active"].ToString();
                        _EmployeeMasterModel.DateOfResignation = dr["ResignationDate"].ToString();
                        _EmployeeMasterModel.NatureOfDuties = dr["NatureOfDuties"].ToString();
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
                oDataTable.Dispose();
            }

            return _EmployeeMasterModel;
        }

        public async Task<EmployeeMasterModel> GetSearchData(EmployeeMasterModel model, string EmpCode)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@EmpCode", EmpCode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.EmployeeMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new EmployeeMasterModel
                                               {
                                                   EmpId = Convert.ToInt32(dr["Emp_Id"]),
                                                   EmpCode = dr["Emp_Code"].ToString(),
                                                   Name = dr["Emp_Name"].ToString(),
                                                   Department = dr["DeptName"].ToString(),
                                                   Shift = dr["ShiftName"].ToString(),
                                                   Designation = dr["Designation"].ToString(),
                                                   Category = dr["Category"].ToString(),
                                                   NatureOfDuties = dr["NatureOfDuties"].ToString(),
                                                   DateOfJoining = dr["DateOfJoining"].ToString(),
                                                   EntryDate = dr["Entry_Date"].ToString(),
                                                   DateOfResignation = dr["ResignationDate"].ToString(),
                                                   DOB = dr["DOB"].ToString(),
                                                   Active = dr["active"].ToString()
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

        public async Task<ResponseResult> DeleteByID(int ID, string EmpName)
        {
            dynamic _ResponseResult = null;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@empid", ID);
                    oCmd.Parameters.AddWithValue("@EmpName", EmpName);
                    oCmd.Parameters.AddWithValue("@Flag", "DELETE");
                    await myConnection.OpenAsync();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = Convert.ToInt32(Reader["StatusCode"].ToString()) == 410
                                    ? HttpStatusCode.Gone
                                    : HttpStatusCode.BadRequest,
                                StatusText = "Success",
                                Result = Reader["Result"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Exception";
                _ResponseResult.Result = ex.Message.ToString();
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
        public async Task<ResponseResult> GetEmpIdandEmpCode(string designation, string department)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEmpCode"));
                SqlParams.Add(new SqlParameter("@desigEntryid", designation));
                SqlParams.Add(new SqlParameter("@Deptid", department));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetSalaryHead()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SalaryHead"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetSalaryMode(int SalaryHeadId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "SalaryHeadDeatil"));
                SqlParams.Add(new SqlParameter("@SalHeadEntryId", SalaryHeadId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetJobDepartMent()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "JobDepartment"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetJobDesignation()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "JobDesignation"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetJobShift()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "JobShift"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<EmployeeMasterModel> GetDashboardData(EmployeeMasterModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@Deptid", string.IsNullOrEmpty(model.Department) ? (object)DBNull.Value : model.Department);
                    oCmd.Parameters.AddWithValue("@ShiftId", string.IsNullOrEmpty(model.Shift) ? (object)DBNull.Value : model.Shift);
                    oCmd.Parameters.AddWithValue("@DesignationId", string.IsNullOrEmpty(model.Designation) ? (object)DBNull.Value : model.Designation);
                    oCmd.Parameters.AddWithValue("@CategoryId", string.IsNullOrEmpty(model.Category) ? (object)DBNull.Value : model.Category);
                    oCmd.Parameters.AddWithValue("@EmpCode", string.IsNullOrEmpty(model.EmpCode) ? null : model.EmpCode.Trim());
                    oCmd.Parameters.AddWithValue("@empid", model.EmpId);
                    oCmd.Parameters.AddWithValue("@EmpName", string.IsNullOrEmpty(model.Name) ? null : model.Name.Trim());
                    oCmd.Parameters.AddWithValue("@branchCC", string.IsNullOrEmpty(model.Branch) ? null : model.Branch.Trim());
                    oCmd.Parameters.AddWithValue("@Entrydate", model.EntryDate ?? (object)DBNull.Value);
                    oCmd.Parameters.AddWithValue("@DOJ", model.DateOfJoining ?? (object)DBNull.Value);
                    oCmd.Parameters.AddWithValue("@DOB", model.DOB ?? (object)DBNull.Value);
                    oCmd.Parameters.AddWithValue("@active", model.Active);
                    oCmd.Parameters.AddWithValue("@DOR", model.DateOfResignation ?? (object)DBNull.Value);
                    oCmd.Parameters.AddWithValue("@NatureOfDuties", string.IsNullOrEmpty(model.NatureOfDuties) ? null : model.NatureOfDuties.Trim());
                   
                    await myConnection.OpenAsync();

                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.EmployeeMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                select new EmployeeMasterModel
                                                {
                                                    EmpId = Convert.ToInt32(dr["Emp_Id"]),
                                                    EmpCode = dr["Emp_Code"].ToString(),
                                                    Name = dr["Emp_Name"].ToString(),
                                                    Department = dr["DeptName"].ToString(),
                                                    Branch = dr["branchCC"].ToString(),
                                                    Shift = dr["ShiftName"].ToString(),
                                                    Designation = dr["Designation"].ToString(),
                                                    Category = dr["Category"].ToString(),
                                                    NatureOfDuties = dr["NatureOfDuties"].ToString(),
                                                    DateOfJoining = dr["DateOfJoining"].ToString(),
                                                    EntryDate = dr["Entry_Date"].ToString(),
                                                    DateOfResignation = dr["ResignationDate"].ToString(),
                                                    DOB = dr["DOB"].ToString(),
                                                    Active = dr["Active"].ToString()
                                                }).ToList();
                }
                else
                {
                    model.EmployeeMasterList = new List<EmployeeMasterModel>();
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

        public async Task<ResponseResult> SaveEmployeeMaster(EmployeeMasterModel model)
        {
            dynamic _ResponseResult = null;

            try
            {
                DateTime? entDt = new DateTime();
                DateTime? dojDt = new DateTime();
                DateTime? dorDt = new DateTime();
                DateTime? dobDt = new DateTime();

                entDt = ParseDate(model.EntryDate);
                dojDt = ParseDate(model.DateOfJoining);
                dorDt = ParseDate(model.DateOfResignation);
                dobDt = ParseDate(model.DOB);


                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (model.Mode == "UPDATE")
                    {
                        oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                        oCmd.Parameters.AddWithValue("@empid", model.EmpId);
                        oCmd.Parameters.AddWithValue("@desigEntryid", model.Designation);
                        oCmd.Parameters.AddWithValue("@EmpCode", model.EmpCode);
                        oCmd.Parameters.AddWithValue("@branchCC", model.Branch);
                        oCmd.Parameters.AddWithValue("@EmpName", model.Name);
                        oCmd.Parameters.AddWithValue("@EntryDate", entDt == null ? DBNull.Value : entDt);
                        oCmd.Parameters.AddWithValue("@DOB", dobDt == null ? DBNull.Value : dobDt);
                        oCmd.Parameters.AddWithValue("@DOJ", dojDt == null ? DBNull.Value : dojDt);
                        oCmd.Parameters.AddWithValue("@DateOfJoining", dojDt == null ? DBNull.Value : dojDt);
                        oCmd.Parameters.AddWithValue("@DOR", dorDt == null ? DBNull.Value : dorDt);
                        oCmd.Parameters.AddWithValue("@DesignationId", model.Designation);
                        oCmd.Parameters.AddWithValue("@shiftId", model.Shift);
                        oCmd.Parameters.AddWithValue("@Deptid", model.Department);
                        oCmd.Parameters.AddWithValue("@CategoryId", model.Category);
                        oCmd.Parameters.AddWithValue("@NatureOfDuties", model.NatureOfDuties);
                        oCmd.Parameters.AddWithValue("@active", "Y");
                        oCmd.Parameters.AddWithValue("@ResignationDate", dorDt);
                    }
                    else
                    {
                        oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                        oCmd.Parameters.AddWithValue("@empid", model.EmpId);
                        oCmd.Parameters.AddWithValue("@desigEntryid", model.Designation);
                        oCmd.Parameters.AddWithValue("@EmpCode", model.EmpCode);
                        oCmd.Parameters.AddWithValue("@CC", model.Branch);
                        oCmd.Parameters.AddWithValue("@EmpName", model.Name);
                        oCmd.Parameters.AddWithValue("@EntryDate", entDt == null ? DBNull.Value : entDt);
                        oCmd.Parameters.AddWithValue("@DOB", dobDt == null ? DBNull.Value : dobDt);
                        oCmd.Parameters.AddWithValue("@DOJ", dojDt == null ? DBNull.Value : dojDt);
                        oCmd.Parameters.AddWithValue("@DateOfJoining", dojDt == null ? DBNull.Value : dojDt);
                        oCmd.Parameters.AddWithValue("@DOR", dorDt == null ? DBNull.Value : dorDt);
                        oCmd.Parameters.AddWithValue("@DesigId", model.Designation);
                        oCmd.Parameters.AddWithValue("@shiftId", model.Shift);
                        oCmd.Parameters.AddWithValue("@Deptid", model.Department);
                        oCmd.Parameters.AddWithValue("@CategoryId", model.Category);
                        oCmd.Parameters.AddWithValue("@NatureOfDuties", model.NatureOfDuties);
                        oCmd.Parameters.AddWithValue("@Active", "Y");
                        oCmd.Parameters.AddWithValue("@ResignationDate", dorDt);
                        oCmd.Parameters.AddWithValue("@Gender", model.Gender);
                        oCmd.Parameters.AddWithValue("@Nationality", model.Nationality);
                        oCmd.Parameters.AddWithValue("@MaritalStatus", model.MaritalStatus);
                        oCmd.Parameters.AddWithValue("@BloodGroup", model.BloodGroup);

                        //Contact
                        oCmd.Parameters.AddWithValue("@MobileNo1", model.MobileNo);
                        oCmd.Parameters.AddWithValue("@MobileNo2", model.MobileNo2);
                        oCmd.Parameters.AddWithValue("@EmailId", model.EmailId);
                        oCmd.Parameters.AddWithValue("@CurrentAddress", model.CurrentAddress);
                        oCmd.Parameters.AddWithValue("@PermanentAddress", model.permanentAddress);
                        oCmd.Parameters.AddWithValue("@EmergancyContactNo", model.EmergencyContact);
                        oCmd.Parameters.AddWithValue("@EmergancyContactRelation", model.EmergencyContactRelation);

                        //salary
                        oCmd.Parameters.AddWithValue("@BankName", model.BankName);
                        oCmd.Parameters.AddWithValue("@BankAccountNo", model.AccountNo);
                        oCmd.Parameters.AddWithValue("@PANNOTaxIdentificationNo", model.PANNo);
                        oCmd.Parameters.AddWithValue("@AadharCardNoCountryCardNo", model.AdharNo);
                        oCmd.Parameters.AddWithValue("@IBANSwiftCode", model.SwiftCode);
                        oCmd.Parameters.AddWithValue("@PaymentMode", model.PaymentMode);
                        oCmd.Parameters.AddWithValue("@PFNO", model.PFNo);
                        oCmd.Parameters.AddWithValue("@ESINo", model.ESINo);
                        oCmd.Parameters.AddWithValue("@GrossSalary", model.GrossSalary);
                        oCmd.Parameters.AddWithValue("@BasicSalary", model.BasicSalary);
                        oCmd.Parameters.AddWithValue("@CTC", model.CalculatePfOn);
                        oCmd.Parameters.AddWithValue("@SalaryBasisHrs", model.SalaryBasis);
                        oCmd.Parameters.AddWithValue("@SalaryCalculationBasisOn", model.SalaryCalculation);
                        oCmd.Parameters.AddWithValue("@PFApplicable", model.PFApplicable);
                        oCmd.Parameters.AddWithValue("@ApplyPFonAmt", model.ApplyPFFonmAmt);
                        oCmd.Parameters.AddWithValue("@ApplyESIonAmt", model.ApplyESIFonmAmt);
                        oCmd.Parameters.AddWithValue("@OTApplicable", model.OTApplicable);
                        oCmd.Parameters.AddWithValue("@LeaveApplicable", model.LeaveApplicable);
                        oCmd.Parameters.AddWithValue("@ESIApplicable", model.ESIApplicable);
                        oCmd.Parameters.AddWithValue("@LateMarkingApplicable", model.LateMarkingCalculationApplicable);
                        oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.FixSalaryAmt);

                        //Allowance-Deduction
                        //oCmd.Parameters.AddWithValue("@SalaryHeadId", model.SalaryHeadId);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.AllowanceMode);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.Percent);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.AllowanceAmount);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.AllowanceType);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.PartyPay);

                        //Job&Work Detail
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.DateOfJoining);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.JobDepartment);
                        oCmd.Parameters.AddWithValue("@ReportingDesignationId", model.JobDesignation);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.ReportingMg);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.EmployeeType);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.WorkLocation);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.JObShift);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.EmpGrade);
                        oCmd.Parameters.AddWithValue("@ProbationPeriod", model.JobProbationPeriod);
                        oCmd.Parameters.AddWithValue("@DateOfProbation", model.ProbationStartDate);
                        //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.ProbationEndDate);
                        oCmd.Parameters.AddWithValue("@DateOfConfirm", model.DateOfConfirmation);
                        oCmd.Parameters.AddWithValue("@Referance1", model.JobReference1);
                        oCmd.Parameters.AddWithValue("@Referencetwo", model.JobReference2);
                        oCmd.Parameters.AddWithValue("@Through", model.JoiningThrough);

                        //Document-IDproof
                        oCmd.Parameters.AddWithValue("@NationalIdPassport", model.PassportNo);
                        oCmd.Parameters.AddWithValue("@WorkPermitVisa", model.WorkPeritVisa);
                        oCmd.Parameters.AddWithValue("@DrivingLicence", model.DrivingLicenseNo);
                        oCmd.Parameters.AddWithValue("@MedicalInsuranceDetail", model.MedicalInsuranceDetail);
                        //oCmd.Parameters.AddWithValue("@PANNOTaxIdentificationNo", model.PANNo);

                        //UploadSection
                        oCmd.Parameters.AddWithValue("@ThumbPath", model.ThumbUnPress);
                        //oCmd.Parameters.AddWithValue("@AadharCardNoCountryCardNo", model.AdharNo);
                        //oCmd.Parameters.AddWithValue("@AadharCardNoCountryCardNo", model.fileUpload);


                        //Educational
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Qualification);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Univercity_Sch);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Per);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.InYear);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Remark);


                        //Experiance
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.CompanyName);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.CFromDate);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.CToDate);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Designation);
                        //oCmd.Parameters.AddWithValue("@DateOfConfirm", model.Salary);


                        //ExitDetail
                        oCmd.Parameters.AddWithValue("@NoticePeriod", model.NoticPeriod);
                        oCmd.Parameters.AddWithValue("@GratutyEligibility", model.GratutyEligibility);

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
                                StatusText = Reader["StatusText"].ToString(),
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
                Error.StackTrace = ex.StackTrace;
                _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error occurred while saving employee data.",
                    Result = ex.Message
                };
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

        public static DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            throw new FormatException($"Invalid date format: {dateString}. Expected format: dd/MM/yyyy");
        }
    }
}
