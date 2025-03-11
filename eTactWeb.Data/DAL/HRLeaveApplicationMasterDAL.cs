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

        public HRLeaveApplicationMasterDAL(IConfiguration configuration, IDataLogic dataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = dataLogic;
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
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
                SqlParams.Add(new SqlParameter("@dt", DT));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryId", model.LeaveAppEntryId));
                SqlParams.Add(new SqlParameter("@LeaveAppYearCode", model.LeaveAppYearCode));
                SqlParams.Add(new SqlParameter("@BranchCC", model.BranchCC));
                SqlParams.Add(new SqlParameter("@LeaveAppEntryDate", model.LeaveAppEntryDate));
                SqlParams.Add(new SqlParameter("@ApplicationNo", model.ApplicationNo));
                SqlParams.Add(new SqlParameter("@ApplicationDate", model.ApplicationDate));
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
                SqlParams.Add(new SqlParameter("@BalanceMonthlyLeave", model.BalanceLeaveMonthly));
                SqlParams.Add(new SqlParameter("@MaxAllowedLeave", model.MaxAllowedLeave));
                SqlParams.Add(new SqlParameter("@TraineePermanent", model.TraineePermanent));
                SqlParams.Add(new SqlParameter("@BalanceAdvanceAmt", model.BalanceAdvanceAmt));
                SqlParams.Add(new SqlParameter("@DepartHeadApprovedBy", model.DepartHeadApprovedBy));
                SqlParams.Add(new SqlParameter("@HRApprovedBy", model.HRApprovedBy));
                SqlParams.Add(new SqlParameter("@DepartAppdate", model.DepartAppdate));
                SqlParams.Add(new SqlParameter("@HRAppDate", model.HRAppDate));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@Updatedby", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUPdatedBy", model.LastUPdatedDate));
                SqlParams.Add(new SqlParameter("@Canceled", model.Canceled));
                SqlParams.Add(new SqlParameter("@Approved", model.@Approved));

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
