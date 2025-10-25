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
        public string GetBranchConnectionString(string branchDatabaseName)
        {
            // Base template (from appsettings)
            string baseConnection = _connectionStringService.GetConnectionString(); // e.g., "Data Source=ServerName;Initial Catalog=CompanyInfoDetailWEB;User Id=sa;Password=786nazhuss;"

            // Replace Initial Catalog with selected branch database
            var builder = new SqlConnectionStringBuilder(baseConnection)
            {
                InitialCatalog = branchDatabaseName
            };

            return builder.ConnectionString;
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
                        _EmployeeMasterModel.Gender = dr["Gender"].ToString();
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

        public async Task<EmployeeMasterModel> GetSearchData(EmployeeMasterModel model, string EmpCode, string ReportType)
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
                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    oCmd.Parameters.AddWithValue("@EmpCode", EmpCode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if(ReportType== "DashBoardSummary")
                    {
                        model.EmployeeMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                    select new EmployeeMasterModel
                                                    {
                                                        EmpId = dr["Emp_Id"] != DBNull.Value ? Convert.ToInt32(dr["Emp_Id"]) : 0,
                                                        EmpCode = dr["Emp_Code"]?.ToString() ?? string.Empty,
                                                        Name = dr["Emp_Name"]?.ToString() ?? string.Empty,
                                                        Branch = dr["branchCC"]?.ToString() ?? string.Empty,
                                                        Designation = dr["Designation"]?.ToString() ?? string.Empty,
                                                        Department = dr["DeptName"]?.ToString() ?? string.Empty,
                                                        Category = dr["Category"]?.ToString() ?? string.Empty,
                                                        DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        DateOfJoining = dr["DateOfJoining"] != DBNull.Value ? Convert.ToDateTime(dr["DateOfJoining"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        ResignationDate = dr["ResignationDate"] != DBNull.Value ? Convert.ToDateTime(dr["ResignationDate"]).ToString("yyyy-MM-dd") : string.Empty,

                                                        Shift = dr["ShiftName"]?.ToString() ?? string.Empty,
                                                        NatureOfDuties = dr["NatureOfDuties"]?.ToString() ?? string.Empty,
                                                        Active = dr["Active"]?.ToString() ?? string.Empty,

                                                        EntryDate = dr["Entry_Date"] != DBNull.Value ? Convert.ToDateTime(dr["Entry_Date"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        Gender = dr["Gender"]?.ToString() ?? string.Empty,
                                                        Nationality = dr["Nationality"]?.ToString() ?? string.Empty,
                                                        MaritalStatus = dr["MaritalStatus"]?.ToString() ?? string.Empty,
                                                        BloodGroup = dr["BloodGroup"]?.ToString() ?? string.Empty,
                                                        MobileNo = dr["MobileNo1"]?.ToString() ?? string.Empty,
                                                        MobileNo2 = dr["MobileNo2"]?.ToString() ?? string.Empty,
                                                        EmailId = dr["EmailId"]?.ToString() ?? string.Empty,
                                                        CurrentAddress = dr["CurrentAddress"]?.ToString() ?? string.Empty,
                                                        permanentAddress = dr["PermanentAddress"]?.ToString() ?? string.Empty,
                                                        EmergencyContact = dr["EmergancyContactNo"]?.ToString() ?? string.Empty,
                                                        EmergencyContactRelation = dr["EmergancyContactRelation"]?.ToString() ?? string.Empty,

                                                    }).ToList();
                    }

                
                    if(ReportType== "DashBoardDetail")
                    {
                        model.EmployeeMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                                    select new EmployeeMasterModel
                                                    {
                                                        EmpId = dr["Emp_Id"] != DBNull.Value ? Convert.ToInt32(dr["Emp_Id"]) : 0,
                                                        EmpCode = dr["Emp_Code"]?.ToString() ?? string.Empty,
                                                        Name = dr["Emp_Name"]?.ToString() ?? string.Empty,
                                                        Branch = dr["branchCC"]?.ToString() ?? string.Empty,
                                                        Designation = dr["Designation"]?.ToString() ?? string.Empty,
                                                        Department = dr["DeptName"]?.ToString() ?? string.Empty,
                                                        Category = dr["Category"]?.ToString() ?? string.Empty,
                                                        DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        DateOfJoining = dr["DateOfJoining"] != DBNull.Value ? Convert.ToDateTime(dr["DateOfJoining"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        ResignationDate = dr["ResignationDate"] != DBNull.Value ? Convert.ToDateTime(dr["ResignationDate"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        Shift = dr["ShiftName"]?.ToString() ?? string.Empty,
                                                        NatureOfDuties = dr["NatureOfDuties"]?.ToString() ?? string.Empty,
                                                        Active = dr["Active"]?.ToString() ?? string.Empty,
                                                        EntryDate = dr["Entry_Date"] != DBNull.Value ? Convert.ToDateTime(dr["Entry_Date"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        Gender = dr["Gender"]?.ToString() ?? string.Empty,
                                                        Nationality = dr["Nationality"]?.ToString() ?? string.Empty,
                                                        MaritalStatus = dr["MaritalStatus"]?.ToString() ?? string.Empty,
                                                        BloodGroup = dr["BloodGroup"]?.ToString() ?? string.Empty,
                                                        MobileNo = dr["MobileNo1"]?.ToString() ?? string.Empty,
                                                        MobileNo2 = dr["MobileNo2"]?.ToString() ?? string.Empty,
                                                        EmailId = dr["EmailId"]?.ToString() ?? string.Empty,
                                                        CurrentAddress = dr["CurrentAddress"]?.ToString() ?? string.Empty,
                                                        permanentAddress = dr["PermanentAddress"]?.ToString() ?? string.Empty,
                                                        EmergencyContact = dr["EmergancyContactNo"]?.ToString() ?? string.Empty,
                                                        EmergencyContactRelation = dr["EmergancyContactRelation"]?.ToString() ?? string.Empty,
                                                        BankName = dr["BankName"]?.ToString() ?? string.Empty,
                                                        AccountNo = dr["BankAccountNo"]?.ToString() ?? string.Empty,
                                                        AdharNo = dr["AadharCardNoCountryCardNo"]?.ToString() ?? string.Empty,
                                                        PANNo = dr["PANNOTaxIdentificationNo"]?.ToString() ?? string.Empty,
                                                        SwiftCode = dr["IBANSwiftCode"]?.ToString() ?? string.Empty,
                                                        PaymentMode = dr["PaymentMode"]?.ToString() ?? string.Empty,
                                                        PFNo = dr["PFNo"]?.ToString() ?? string.Empty,
                                                        ESINo = dr["ESINo"]?.ToString() ?? string.Empty,
                                                        GrossSalary = dr["GrossSalary"] != DBNull.Value ? Convert.ToDecimal(dr["GrossSalary"]) : 0,
                                                        BasicSalary = dr["BasicSalary"] != DBNull.Value ? Convert.ToDecimal(dr["BasicSalary"]) : 0,
                                                        CalculatePfOn = dr["CTC"]?.ToString() ?? string.Empty,
                                                        PFApplicable = dr["PFApplicable"]?.ToString() ?? string.Empty,
                                                        ESIApplicable = dr["ESIApplicable"]?.ToString() ?? string.Empty,
                                                        ApplyPFFonmAmt = dr["ApplyPFonAmt"] != DBNull.Value ? Convert.ToDecimal(dr["ApplyPFonAmt"]) : 0,
                                                        ApplyESIFonmAmt = dr["ApplyESIonAmt"] != DBNull.Value ? Convert.ToDecimal(dr["ApplyESIonAmt"]) : 0,
                                                        SalaryCalculation = dr["SalaryCalculationBasisOn"]?.ToString() ?? string.Empty,
                                                       
                                                        SalaryBasisHr = dr["SalaryBasisHrs"] != DBNull.Value ? Convert.ToDecimal(dr["SalaryBasisHrs"]) : 0,
                                                        OTApplicable = dr["OTApplicable"]?.ToString() ?? string.Empty,
                                                        LeaveApplicable = dr["LeaveApplicable"]?.ToString() ?? string.Empty,
                                                        LateMarkingCalculationApplicable = dr["LateMarkingApplicable"]?.ToString() ?? string.Empty,
                                                        FixSalaryAmt = dr["FixSalaryAmount"] != DBNull.Value ? Convert.ToDecimal(dr["FixSalaryAmount"]) : 0,
                                                        JobDepartmentId = dr["DeptId"] != DBNull.Value ? Convert.ToInt32(dr["DeptId"]) : 0,
                                                        JobDesignationId = dr["DesigId"] != DBNull.Value ? Convert.ToInt32(dr["DesigId"]) : 0,
                                                        ReportingMg = dr["ReportingManager"]?.ToString() ?? string.Empty,
                                                        EmployeeType = dr["EmpType"]?.ToString() ?? string.Empty,
                                                        JObShiftId = dr["ShiftId"] != DBNull.Value ? Convert.ToInt32(dr["ShiftId"]) : 0,
                                                        EmpGrade = dr["GradeId"]?.ToString() ?? string.Empty,
                                                        ProbationPeriod = dr["ProbationPeriod"] != DBNull.Value ? Convert.ToInt32(dr["ProbationPeriod"]) : 0,
                                                   
                                                        DateOfConfirmation = dr["DateOfConfirm"] != DBNull.Value ? Convert.ToDateTime(dr["DateOfConfirm"]).ToString("yyyy-MM-dd") : string.Empty,
                                                        JobReference1 = dr["Referance1"]?.ToString() ?? string.Empty,
                                                        JobReference2 = dr["Referencetwo"]?.ToString() ?? string.Empty,
                                                        JoiningThrough = dr["Through"]?.ToString() ?? string.Empty,
                                                        PassportNo = dr["NationalIdPassport"]?.ToString() ?? string.Empty,
                                                        WorkPeritVisa = dr["WorkPermitVisa"]?.ToString() ?? string.Empty,
                                                        DrivingLicenseNo = dr["DrivingLicence"]?.ToString() ?? string.Empty,
                                                        MedicalInsuranceDetail = dr["MedicalInsuranceDetail"]?.ToString() ?? string.Empty,
                                                        ThumbUnPress = dr["ThumbPath"]?.ToString() ?? string.Empty,
                                                        NoticPeriod = dr["NoticePeriod"] != DBNull.Value ? Convert.ToInt32(dr["NoticePeriod"]) : 0,

                                                        GratutyEligibility = dr["GratutyEligibility"]?.ToString() ?? string.Empty,

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

        public async Task<ResponseResult> DeleteByID(int ID, string EmpName)
        {
            dynamic _ResponseResult = null;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    //SqlCommand oCmd = new SqlCommand("HREmployeeMaster", myConnection)
                    //{
                    //    CommandType = CommandType.StoredProcedure
                    //};
                    var SqlParams = new List<dynamic>();
                    SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                    SqlParams.Add(new SqlParameter("@empid", ID));
                    SqlParams.Add(new SqlParameter("@EmpName", EmpName));
                   
                    
                    //Reader = await oCmd.ExecuteReaderAsync();
                    //if (Reader != null)
                    //{
                    //    while (Reader.Read())
                    //    {
                    //        _ResponseResult = new ResponseResult()
                    //        {
                    //            StatusCode = Convert.ToInt32(Reader["StatusCode"].ToString()) == 410
                    //                ? HttpStatusCode.Gone
                    //                : HttpStatusCode.BadRequest,
                    //            StatusText = "Success",
                    //            Result = Reader["Result"].ToString() ?? string.Empty
                    //        };
                    //    }
                    //}
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("HREmployeeMaster", SqlParams);
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
        } public async Task<ResponseResult> GetEmployeeType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ReportingMg"));
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
        public async Task<ResponseResult> GetReportingMg()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ReportingMg"));
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
                                                    Gender = dr["Gender"].ToString(),
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
                DateTime? DateOfProbation = new DateTime();
                DateTime? DateOfConfirm = new DateTime();

                entDt = ParseDate(model.EntryDate);
                dojDt = ParseDate(model.DateOfJoining);
                dorDt = ParseDate(model.DateOfResignation);
                dobDt = ParseDate(model.DOB);
                DateOfProbation = ParseDate(model.ProbationStartDate);
                DateOfConfirm = ParseDate(model.DateOfConfirmation);

                string branchlist = string.Join(",", model.Branches ?? new List<string>());
                if (!string.IsNullOrWhiteSpace(branchlist))
                {
                    foreach (var branchDb in model.Branches ?? new List<string>())
                    {
                        // Generate dynamic connection string for this branch
                        string branchConnectionString = GetBranchConnectionString(branchDb);
                        using (SqlConnection conn = new SqlConnection(branchConnectionString))
                        {
                            SqlCommand oCmd = new SqlCommand("HREmployeeMaster", conn)
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
                                oCmd.Parameters.AddWithValue("@Gender", model.Gender);
                                oCmd.Parameters.AddWithValue("@NatureOfDuties", model.NatureOfDuties);
                                oCmd.Parameters.AddWithValue("@branchlist", branchlist);
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
                                oCmd.Parameters.AddWithValue("@branchlist", branchlist);

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
                                oCmd.Parameters.AddWithValue("@DateOfProbation", DateOfProbation);
                                //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.ProbationEndDate);
                                oCmd.Parameters.AddWithValue("@DateOfConfirm", DateOfConfirm);
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


                            conn.Open();
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
                }

                else
                { //Single Branch


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
                            oCmd.Parameters.AddWithValue("@branchlist", branchlist);
                            oCmd.Parameters.AddWithValue("@Gender", model.Gender);
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
                            oCmd.Parameters.AddWithValue("@branchlist", branchlist);
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
                            oCmd.Parameters.AddWithValue("@DateOfProbation", DateOfProbation);
                            //oCmd.Parameters.AddWithValue("@FixSalaryAmount", model.ProbationEndDate);
                            oCmd.Parameters.AddWithValue("@DateOfConfirm", DateOfConfirm);
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
