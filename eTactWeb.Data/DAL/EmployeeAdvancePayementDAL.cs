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
        public async Task<ResponseResult> FillPreviousAdanaceLoanDetail(int empId, string requestedDate)
		{
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PreviousAdanaceLoanDetail"));
                SqlParams.Add(new SqlParameter("@Empid", empId));
                SqlParams.Add(new SqlParameter("@RequestDate", requestedDate));
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
        public async Task<ResponseResult> FillAdvanceType(int empId, string requestedDate)
		{
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PreviousAdanaceLoanDetail"));
                SqlParams.Add(new SqlParameter("@Empid", empId));
                SqlParams.Add(new SqlParameter("@RequestDate", requestedDate));
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
        public async Task<ResponseResult> FillFinancialEmployeeNameList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FinancialEmployeeNameList"));
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
        public async Task<ResponseResult> FillMgmtEmployeeNameList()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "MgmtEmployeeNameList"));
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
                var doj = CommonFunc.ParseFormattedDate(model.DOJ);
                var reqDt = CommonFunc.ParseFormattedDate(model.RequestDate);
                var entryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
                var mgrAppDt = CommonFunc.ParseFormattedDate(model.MgrApprovaldate);
                var hrAppDt = CommonFunc.ParseFormattedDate(model.HRApprovalDate);
                var finAppDt = CommonFunc.ParseFormattedDate(model.FinanceApprovalDate);
                var finRecoveryDt = CommonFunc.ParseFormattedDate(model.FinalRevoveryDate);
                var actualFinRecoveryDt = CommonFunc.ParseFormattedDate(model.ActualFinalRecoveryDate);

                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    //SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                    //SqlParams.Add(new SqlParameter("@lastupdationdate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                var currentDate = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());

                SqlParams.Add(new SqlParameter("@AdvanceEntryId", model.AdvanceEntryId));
                SqlParams.Add(new SqlParameter("@AdvanceYearCode", model.AdvanceYearCode));
                SqlParams.Add(new SqlParameter("@AdvanceSlipNo", model.AdvanceSlipNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EmpId", model.EmpId));
                SqlParams.Add(new SqlParameter("@DesigId", model.DesigId));
                SqlParams.Add(new SqlParameter("@DepId", model.DepId));
                SqlParams.Add(new SqlParameter("@DOJ", doj == default ? string.Empty : doj));
                SqlParams.Add(new SqlParameter("@BasicSalary", model.BasicSalary));
                SqlParams.Add(new SqlParameter("@NetSalary", model.NetSalary));
                SqlParams.Add(new SqlParameter("@PresentDaysinCurrMonth", model.PresentDaysinCurrMonth));
                SqlParams.Add(new SqlParameter("@PresentDaysInCurrYear", model.PresentDaysInCurrYear));
                SqlParams.Add(new SqlParameter("@CategoryId", model.CategoryId));
                SqlParams.Add(new SqlParameter("@RequestDate", reqDt == default ? string.Empty : reqDt));
                SqlParams.Add(new SqlParameter("@EntryDate", entryDt == default ? string.Empty : entryDt));
                SqlParams.Add(new SqlParameter("@Purpose", model.Purpose ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RequestedAmount", model.RequestedAmount));
                SqlParams.Add(new SqlParameter("@ApprovedAmount", model.ApprovedAmount)); 
                SqlParams.Add(new SqlParameter("@PreviousPendAdvanceAmt", model.PreviousPendAdvanceAmt));
                SqlParams.Add(new SqlParameter("@PreviousPendLoanAmt", model.PreviousPendLoanAmt));
                SqlParams.Add(new SqlParameter("@MgrApprovaldate", mgrAppDt == default ? string.Empty : mgrAppDt));
                SqlParams.Add(new SqlParameter("@MgrApprovedbyEmpid", model.MgrApprovedbyEmpid));
                SqlParams.Add(new SqlParameter("@HRApprovedbyEmpid", model.HRApprovedbyEmpid));
                SqlParams.Add(new SqlParameter("@HRApprovalDate", hrAppDt == default ? string.Empty : hrAppDt));
                SqlParams.Add(new SqlParameter("@FinanceApprovalEmpid", model.FinanceApprovalEmpid));
                SqlParams.Add(new SqlParameter("@FinanceApprovalDate", finAppDt == default ? string.Empty : finAppDt)); 
                SqlParams.Add(new SqlParameter("@CanceledByEmpId", model.CanceledByEmpId)); 
                SqlParams.Add(new SqlParameter("@Canceled", model.Canceled ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CancelOrApprovalremarks", model.CancelOrApprovalremarks ?? string.Empty)); 
                SqlParams.Add(new SqlParameter("@ModeOfPayment", model.ModeOfPayment ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentReferenceNo", model.PaymentReferenceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentVoucherNo", model.PaymentVoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentVoucherType", model.PaymentVoucherType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentRemark", model.PaymentRemark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RecoveryMethod", model.RecoveryMethod ?? string.Empty));
                SqlParams.Add(new SqlParameter("@NoofInstallment", model.NoofInstallment));
                SqlParams.Add(new SqlParameter("@StartRecoveryFromMonth", model.StartRecoveryFromMonth ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AutoDeductionFromSalaryYN", model.AutoDeductionFromSalaryYN ?? string.Empty));
                SqlParams.Add(new SqlParameter("@FinalRevoveryDate", finRecoveryDt == default ? string.Empty : finRecoveryDt));
                SqlParams.Add(new SqlParameter("@ActualFinalRecoveryDate", actualFinRecoveryDt == default ? string.Empty : actualFinRecoveryDt));
                SqlParams.Add(new SqlParameter("@StatusHoldCancelApproved", model.StatusHoldCancelApproved ?? string.Empty)); 
                SqlParams.Add(new SqlParameter("@RequestEntryByMachine", model.RequestEntryByMachine ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ApprovedBYMachine", model.ApprovedBYMachine ?? string.Empty)); 
                SqlParams.Add(new SqlParameter("@CancelByMachine", model.CancelByMachine ?? string.Empty)); 
                SqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@UID", model.UID));

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

    }
}
