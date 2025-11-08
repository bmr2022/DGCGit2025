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
    public class HRLeaveApplicationMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        //private readonly IConfiguration configuration;
        private readonly DataSet oDataSet = new();

        private readonly DataTable oDataTable = new();
        private dynamic? _ResponseResult;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public HRLeaveApplicationMasterDAL(IConfiguration configuration, IDataLogic dataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = dataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetEmpName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetEmployeeDetail(int empid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeDetail"));
                SqlParams.Add(new SqlParameter("@Empid", empid));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetLeaveDetail(int empid, string LeaveAppEntryDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "GetBalanceLeave"));
                SqlParams.Add(new SqlParameter("@Empid", empid));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryDate",CommonFunc.ParseFormattedDate(LeaveAppEntryDate)));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetBalanceandMaxLeaveTypeWise(int empid, string LeaveAppEntryDate,int LeaveEntryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "GetBalanceandMaxLeaveTypeWise"));
                SqlParams.Add(new SqlParameter("@Empid", empid));
                SqlParams.Add(new SqlParameter("@LeaveEntryId", LeaveEntryId));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryDate", CommonFunc.ParseFormattedDate(LeaveAppEntryDate)));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetLeaveName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "LeaveName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetShiftName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "Shift"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetEmpCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "EmployeeCode"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@LeaveAppYearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveData(HRLeaveApplicationMasterModel model, DataTable DT)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@Updatedby", model.UpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUPdatedBy", DateTime.Parse(model.LastUPdatedDate).ToString("dd/MMM/yyyy")));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                SqlParams.Add(new SqlParameter("@dt", DT));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryId", model.LeaveAppEntryId));
                SqlParams.Add(new SqlParameter("@LeaveAppYearCode", model.LeaveAppYearCode));
                SqlParams.Add(new SqlParameter("@BranchCC", model.BranchCC));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryDate", DateTime.Parse(model.LeaveAppEntryDate)));
              //  SqlParams.Add(new SqlParameter("@LeaveAppEntryDate", DateTime.Parse(model.LeaveAppEntryDate).ToString("dd/MMM/yyyy")));
                SqlParams.Add(new SqlParameter("@ApplicationNo", model.ApplicationNo));
            //    SqlParams.Add(new SqlParameter("@ApplicationDate", DateTime.Parse(model.ApplicationDate).ToString("dd/MMM/yyyy")));
                SqlParams.Add(new SqlParameter("@ApplicationDate", DateTime.Parse(model.ApplicationDate)));
                SqlParams.Add(new SqlParameter("@EmpId", model.EmpId));
                SqlParams.Add(new SqlParameter("@MobileNo", model.MobileNo));
                SqlParams.Add(new SqlParameter("@PhoneNo", model.PhoneNo));
                SqlParams.Add(new SqlParameter("@desigEntryId", model.desigEntryId));
                SqlParams.Add(new SqlParameter("@CategoryId", model.CategoryId));
                SqlParams.Add(new SqlParameter("@DeptId", model.DeptId));
                SqlParams.Add(new SqlParameter("@ShiftId", model.ShiftId));
                SqlParams.Add(new SqlParameter("@TotalYearlyLeave", model.TotalYearlyLeave));
                SqlParams.Add(new SqlParameter("@TotalMonthlyLeave", model.TotalMonthlyLeave));
                SqlParams.Add(new SqlParameter("@GrossSalary", model.GrossSalary));
                SqlParams.Add(new SqlParameter("@BalanceMonthlyLeave", model.BalanceMonthlyLeave));
                SqlParams.Add(new SqlParameter("@MaxAllowedLeave", model.MaxAllowedLeave));
                SqlParams.Add(new SqlParameter("@TraineePermanent", model.TraineePermanent));
                SqlParams.Add(new SqlParameter("@BalanceAdvanceAmt", model.BalanceAdvanceAmt));
                SqlParams.Add(new SqlParameter("@DepartHeadApprovedBy", model.DepartHeadApprovedBy));
                SqlParams.Add(new SqlParameter("@HRApprovedBy", model.HRApprovedBy));
                SqlParams.Add(new SqlParameter("@DepartAppdate", DateTime.Parse(model.DepartAppdate)));
                SqlParams.Add(new SqlParameter("@HRAppDate", DateTime.Parse(model.HRAppDate)));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));
               
                SqlParams.Add(new SqlParameter("@Canceled", model.Canceled));
                SqlParams.Add(new SqlParameter("@Approved", model.Approved));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
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

                responseResult = await _IDataLogic.ExecuteDataSet("HRSPLeaveApplicationMain", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<HRLeaveApplicationDashBoard> GetDashboardDetailData(string ReportType, string FromDate, string ToDate,int Empid,int LeaveEntryId)
        {
            DataSet? oDataSet = new DataSet();
            var model = new HRLeaveApplicationDashBoard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPLeaveApplicationMain", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@Fromdate", Convert.ToDateTime(FromDate));
                    oCmd.Parameters.AddWithValue("@todate", Convert.ToDateTime(ToDate));
                    oCmd.Parameters.AddWithValue("@ReportType", ReportType);
                    oCmd.Parameters.AddWithValue("@Empid", Empid);
                    oCmd.Parameters.AddWithValue("@LeaveEntryId", LeaveEntryId);



                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "SUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.HRLeaveApplicationDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                               select new HRLeaveApplicationDashBoard
                                                               {


                                                                   LeaveAppEntryId = Convert.ToInt32(dr["LeaveAppEntryId"]),
                                                                   LeaveAppYearCode = Convert.ToInt32(dr["LeaveAppYearCode"]),
                                                                   LeaveAppEntryDate = dr["LeaveAppEntryDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["LeaveAppEntryDate"]).ToString("dd-MM-yyyy"),

                                                                   BranchCC = dr["BranchCC"].ToString(),
                                                                   ApplicationNo = dr["ApplicationNo"].ToString(),
                                                                   ApplicationDate = dr["ApplicationDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ApplicationDate"]).ToString("dd-MM-yyyy"),


                                                                   EmpName = dr["EmpName"].ToString(),
                                                                   MobileNo = dr["MobileNo"].ToString(),
                                                                   PhoneNo = dr["PhoneNo"].ToString(),
                                                                   Designation = dr["Designation"].ToString(),
                                                                   EmpCategory = dr["EmpCateg"].ToString(),
                                                                   Department = dr["DeptName"].ToString(),
                                                                   TotalYearlyLeave = Convert.ToDecimal(dr["TotalYearlyLeave"]),
                                                                   TotalMonthlyLeave = Convert.ToDecimal(dr["TotalMonthlyLeave"]),
                                                                   GrossSalary = Convert.ToDecimal(dr["GrossSalary"]),
                                                                   BalanceMonthlyLeave = Convert.ToDecimal(dr["BalanceMonthlyLeave"]),
                                                                   MaxAllowedLeave = Convert.ToDecimal(dr["MaxAllowedLeave"]),
                                                                   TraineePermanent = dr["TraineePermanent"].ToString(),
                                                                   BalanceAdvanceAmt = Convert.ToDecimal(dr["BalanceAdvanceAmt"]),
                                                                   DepartHeadApprovedBy = dr["DepartHeadApprovedBy"].ToString(),
                                                                   HRApprovedBy = dr["HRApprovedBy"].ToString(),
                                                                   DepartAppdate = dr["DepartAppdate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["DepartAppdate"]).ToString("dd-MM-yyyy"),
                                                                   HRAppDate = dr["HRAppDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["HRAppDate"]).ToString("dd-MM-yyyy"),

                                                                   ActualEntrybyEmp = dr["ActualEntrybyEmp"].ToString(),
                                                                   UpdatedbyEmp = dr["UpdatedbyEmp"].ToString(),
                                                                   LastUPdatedDate = dr["LastUPdatedBy"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["LastUPdatedBy"]).ToString("dd-MM-yyyy"),

                                                                   Canceled = dr["Canceled"].ToString(),
                                                                   Approved = dr["Approved"].ToString(),



                                                               }).ToList();
                    }
                }
                else if (ReportType == "DETAIL")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.HRLeaveApplicationDashBoardDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                               select new HRLeaveApplicationDashBoard
                                                               {


                                                                   LeaveAppEntryId = Convert.ToInt32(dr["LeaveAppEntryId"]),
                                                                   LeaveAppYearCode = Convert.ToInt32(dr["LeaveAppYearCode"]),

                                                                   EmpName = dr["EmpName"].ToString(),
                                                                   LeaveName = dr["LeaveName"].ToString(),

                                                                   SeqNo = Convert.ToInt32(dr["SeqNo"]),
                                                                   FromDate = dr["FromDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["FromDate"]).ToString("dd-MM-yyyy"),
                                                                   ToDate = dr["ToDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(dr["ToDate"]).ToString("dd-MM-yyyy"),
                                                                   Duration = Convert.ToDecimal(dr["Duration"]),
                                                                   BalanceLeaveMonthly = Convert.ToDecimal(dr["BalanceLeaveMonthly"]),
                                                                   BalanceLeaveYearly = Convert.ToDecimal(dr["BalanceLeaveYearly"]),
                                                                   MaxLeaveInMonth = Convert.ToDecimal(dr["MaxLeaveInMonth"]),

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


        private static HRLeaveApplicationMasterModel PrepareView(DataSet DS, ref HRLeaveApplicationMasterModel? model)
        {
            var HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
            DS.Tables[0].TableName = "HRLeaveApplicationdetail";
            int cnt = 0;
            model.LeaveAppEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["LeaveAppEntryId"].ToString());
            model.LeaveAppYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["LeaveAppYearCode"].ToString());
            model.BranchCC = DS.Tables[0].Rows[0]["BranchCC"].ToString();
            model.LeaveAppEntryDate = DS.Tables[0].Rows[0]["LeaveAppEntryDate"].ToString();
            model.ApplicationNo = DS.Tables[0].Rows[0]["ApplicationNo"].ToString();
            model.ApplicationDate = DS.Tables[0].Rows[0]["ApplicationDate"].ToString();
            model.EmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpId"].ToString());
            model.MobileNo = DS.Tables[0].Rows[0]["MobileNo"].ToString();
            model.PhoneNo = DS.Tables[0].Rows[0]["PhoneNo"].ToString();
            model.desigEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["desigEntryId"].ToString());
            model.CategoryId = Convert.ToInt32(DS.Tables[0].Rows[0]["CategoryId"].ToString());
            model.DeptId = Convert.ToInt32(DS.Tables[0].Rows[0]["DeptId"].ToString());
            model.ShiftId = Convert.ToInt32(DS.Tables[0].Rows[0]["ShiftId"].ToString());
            model.TotalYearlyLeave = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalYearlyLeave"].ToString());
            model.TotalMonthlyLeave = Convert.ToDecimal(DS.Tables[0].Rows[0]["TotalMonthlyLeave"].ToString());
            model.GrossSalary = Convert.ToDecimal(DS.Tables[0].Rows[0]["GrossSalary"].ToString());
            model.BalanceMonthlyLeave = Convert.ToDecimal(DS.Tables[0].Rows[0]["BalanceMonthlyLeave"].ToString());
            model.MaxAllowedLeave = Convert.ToDecimal(DS.Tables[0].Rows[0]["MaxAllowedLeave"].ToString());
            model.BalanceAdvanceAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["BalanceAdvanceAmt"].ToString());
            model.DepartHeadApprovedBy = DS.Tables[0].Rows[0]["DepartHeadApprovedBy"].ToString();
            model.HRApprovedBy =DS.Tables[0].Rows[0]["HRApprovedBy"].ToString();
            model.DepartAppdate = DS.Tables[0].Rows[0]["DepartAppdate"].ToString();
            model.HRAppDate = DS.Tables[0].Rows[0]["HRAppDate"].ToString();
            model.ActualEntryBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryBy"].ToString());
            model.Updatedby = Convert.ToInt32(DS.Tables[0].Rows[0]["Updatedby"].ToString());
            model.LastUPdatedDate = DS.Tables[0].Rows[0]["LastUPdatedBy"].ToString();
            model.Designation = DS.Tables[0].Rows[0]["Designation"].ToString();
            model.EmpCategory = DS.Tables[0].Rows[0]["EmpCateg"].ToString();
            model.Department = DS.Tables[0].Rows[0]["DeptName"].ToString();
            model.Approved = DS.Tables[0].Rows[0]["Approved"].ToString();
            model.Canceled = DS.Tables[0].Rows[0]["Canceled"].ToString();
            model.TraineePermanent = DS.Tables[0].Rows[0]["TraineePermanent"].ToString();

            if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[0].Rows)
                {
                    HRLeaveApplicationDetail.Add(new HRLeaveApplicationDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
                        LeaveEntryId = Convert.ToInt32(row["LeaveEntryId"].ToString()),
                        LeaveName = row["LeaveName"].ToString(),
                        FromDate = Convert.ToDateTime(row["FromDate"]).ToString("dd/MM/yyyy"),
                        ToDate = Convert.ToDateTime(row["ToDate"]).ToString("dd/MM/yyyy"),
                        Duration = Convert.ToDecimal(row["Duration"].ToString()),
                        BalanceLeaveMonthly = Convert.ToDecimal(row["BalanceLeaveMonthly"].ToString()),
                        BalanceLeaveYearly = Convert.ToDecimal(row["BalanceLeaveYearly"].ToString()),
                        MaxLeaveInMonth = Convert.ToDecimal(row["MaxLeaveInMonth"].ToString()),
                       

                    });
                }
                model.ItemDetailGrid = HRLeaveApplicationDetail.OrderBy(x => x.SeqNo).ToList();
            }
            return model;
        }
        public async Task<HRLeaveApplicationMasterModel> GetViewByID(int id, int year)
        {
            var model = new HRLeaveApplicationMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewByid"));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryId", id));
                SqlParams.Add(new SqlParameter("@LeaveAppYearCode", year));

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPLeaveApplicationMain", SqlParams);

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

        public async Task<ResponseResult> DeleteByID(int id, int year)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryId", id));
                SqlParams.Add(new SqlParameter("@LeaveAppYearCode", year));
               
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPLeaveApplicationMain", SqlParams);
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
