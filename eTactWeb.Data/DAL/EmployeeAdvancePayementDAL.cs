using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class EmployeeAdvancePayementDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;

        public EmployeeAdvancePayementDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> FillEntryId(int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@AdvanceYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPAdvanceMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEmpName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "EmployeeName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPAdvanceMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEmpCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "EmployeeCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPAdvanceMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillEmployeeDetail(int empId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "EmployeeDetail"));
                SqlParams.Add(new SqlParameter("@Empid", empId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPAdvanceMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> SaveEmployeeAdvancePayment(HRAdvanceModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var upDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    //SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                    //SqlParams.Add(new SqlParameter("@LastUpdationdate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                var currentDate = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());

                SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationdate", currentDate == default ? string.Empty : currentDate));

                SqlParams.Add(new SqlParameter("@AdvanceEntryId", model.AdvanceEntryId));
                SqlParams.Add(new SqlParameter("@AdvanceYearCode", model.AdvanceYearCode));
                SqlParams.Add(new SqlParameter("@AdvanceSlipNo", model.AdvanceSlipNo));
                SqlParams.Add(new SqlParameter("@EmpId", model.EmpId));
                SqlParams.Add(new SqlParameter("@DesigId", model.DesigId));
                SqlParams.Add(new SqlParameter("@DepId", model.DepId));
                SqlParams.Add(new SqlParameter("@DOJ", model.DOJ));
                SqlParams.Add(new SqlParameter("@BasicSalary", model.BasicSalary));
                SqlParams.Add(new SqlParameter("@NetSalary", model.NetSalary));
                SqlParams.Add(new SqlParameter("@PresentDaysinCurrMonth", model.PresentDaysinCurrMonth));
                SqlParams.Add(new SqlParameter("@PresentDaysInCurrYear", model.PresentDaysInCurrYear));
                SqlParams.Add(new SqlParameter("@CategoryId", model.CategoryId));
                SqlParams.Add(new SqlParameter("@RequestDate", model.RequestDate));
                SqlParams.Add(new SqlParameter("@EntryDate", model.EntryDate));
                SqlParams.Add(new SqlParameter("@Purpose", model.Purpose));
                SqlParams.Add(new SqlParameter("@RequestedAmount", model.RequestedAmount));
                SqlParams.Add(new SqlParameter("@ApprovedAmount", 0)); // constant
                SqlParams.Add(new SqlParameter("@PreviousPendAdvanceAmt", model.PreviousPendAdvanceAmt));
                SqlParams.Add(new SqlParameter("@PreviousPendLoanAmt", model.PreviousPendLoanAmt));
                SqlParams.Add(new SqlParameter("@MgrApprovaldate", model.MgrApprovaldate));
                SqlParams.Add(new SqlParameter("@MgrApprovedbyEmpid", model.MgrApprovedbyEmpid));
                SqlParams.Add(new SqlParameter("@HRApprovedbyEmpid", model.HRApprovedbyEmpid));
                SqlParams.Add(new SqlParameter("@HRApprovalDate", model.HRApprovalDate));
                SqlParams.Add(new SqlParameter("@FinanceApprovalEmpid", model.FinanceApprovalEmpid));
                SqlParams.Add(new SqlParameter("@FinanceApprovalDate", DBNull.Value)); // null
                SqlParams.Add(new SqlParameter("@CanceledByEmpId", 0)); // constant
                SqlParams.Add(new SqlParameter("@Canceled", "N")); // constant
                SqlParams.Add(new SqlParameter("@CancelOrApprovalremarks", "")); // constant
                SqlParams.Add(new SqlParameter("@ModeOfPayment", model.ModeOfPayment));
                SqlParams.Add(new SqlParameter("@PaymentReferenceNo", model.PaymentReferenceNo));
                SqlParams.Add(new SqlParameter("@PaymentVoucherNo", model.PaymentVoucherNo));
                SqlParams.Add(new SqlParameter("@PaymentVoucherType", model.PaymentVoucherType));
                SqlParams.Add(new SqlParameter("@PaymentRemark", model.PaymentRemark));
                SqlParams.Add(new SqlParameter("@RecoveryMethod", model.RecoveryMethod));
                SqlParams.Add(new SqlParameter("@NoofInstallment", model.NoofInstallment));
                SqlParams.Add(new SqlParameter("@StartRecoveryFromMonth", model.StartRecoveryFromMonth));
                SqlParams.Add(new SqlParameter("@AutoDeductionFromSalaryYN", model.AutoDeductionFromSalaryYN));
                SqlParams.Add(new SqlParameter("@FinalRevoveryDate", model.FinalRevoveryDate));
                SqlParams.Add(new SqlParameter("@ActualFinalRecoveryDate", model.ActualFinalRecoveryDate));
                SqlParams.Add(new SqlParameter("@StatusHoldCancelApproved", "")); // constant
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", DBNull.Value)); // null
                SqlParams.Add(new SqlParameter("@LastUpdationDate", DBNull.Value)); // null
                SqlParams.Add(new SqlParameter("@RequestEntryByMachine", model.RequestEntryByMachine));
                SqlParams.Add(new SqlParameter("@ApprovedBYMachine", "")); // constant
                SqlParams.Add(new SqlParameter("@CancelByMachine", DBNull.Value)); // null
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
                SqlParams.Add(new SqlParameter("@UID", model.UID));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("VPSpUserMasterForVendorPortal", SqlParams);

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
