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
using eTactWeb.Data.Common;

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
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            //DBConnectionString = _connectionStringService.GetConnectionString();
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
        public async Task<ResponseResult> FillAdvanceType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "AdvanceType"));
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
        public async Task<ResponseResult> GetDashboardData(string fromDate,string toDate, string employeeName = "", string deptName = "")
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                fromDate = CommonFunc.ParseFormattedDate(fromDate);
                toDate = CommonFunc.ParseFormattedDate(toDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromdate", fromDate));
                SqlParams.Add(new SqlParameter("@todate", toDate));
                SqlParams.Add(new SqlParameter("@EmpName", employeeName));
                SqlParams.Add(new SqlParameter("@DeptName", deptName));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPAdvanceMain", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> DeleteByID(int advEntryId, int advYearCode, int actualEntryBy, string entryByMachineName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@AdvanceEntryId", advEntryId));
                SqlParams.Add(new SqlParameter("@AdvanceYearCode", advYearCode));
                SqlParams.Add(new SqlParameter("@ActualEntryBy", actualEntryBy));
                SqlParams.Add(new SqlParameter("@RequestEntryByMachine", entryByMachineName));

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
                SqlParams.Add(new SqlParameter("@DepId", model.DeptId));
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
                SqlParams.Add(new SqlParameter("@AdvanceType", model.AdvanceType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GrossSalary", model.GrossSalary));
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

        internal async Task<HRAdvanceModel> GetViewByID(int ID,int yearCode, string mode)
        {
            var model = new HRAdvanceModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@AdvanceEntryId", ID));
                SqlParams.Add(new SqlParameter("@AdvanceYearCode", yearCode));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPAdvanceMain", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareView(ResponseResult.Result, ref model, mode);
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

        private static HRAdvanceModel PrepareView(DataSet DS, ref HRAdvanceModel? model, string mode)
        {
            try
            {
                int cnt = 0;

				var row = DS.Tables[0].Rows[0];

				model.AdvanceSlipNo = row["AdvanceSlipNo"]?.ToString();
				model.EmpCode = row["EmpCode"]?.ToString();
				model.EmpName = row["EmployeeName"]?.ToString();     // emp_name ---> emp_code
				model.DesigName = row["Designation"]?.ToString();
				model.DeptName = row["Department"]?.ToString();

				model.RequestDate = row["RequestDate"]?.ToString();
				model.EntryDate = row["EntryDate"]?.ToString();
				model.RequestedAmount = row["RequestedAmount"] != DBNull.Value ? Convert.ToDecimal(row["RequestedAmount"]) : 0;
				model.AdvanceType = row["AdvanceType"]?.ToString();
				model.Purpose = row["Purpose"]?.ToString();

				model.DOJ = row["DOJ"]?.ToString();
				model.BasicSalary = row["BasicSalary"] != DBNull.Value ? Convert.ToDecimal(row["BasicSalary"]) : 0;
				model.GrossSalary = row["GrossSalary"] != DBNull.Value ? Convert.ToDecimal(row["GrossSalary"]) : 0;
				model.NetSalary = row["NetSalary"] != DBNull.Value ? Convert.ToDecimal(row["NetSalary"]) : 0;

				model.PresentDaysinCurrMonth =
					row["PresentDaysinCurrMonth"] != DBNull.Value ? Convert.ToDecimal(row["PresentDaysinCurrMonth"]) : 0;

				model.PresentDaysInCurrYear =
					row["PresentDaysInCurrYear"] != DBNull.Value ? Convert.ToDecimal(row["PresentDaysInCurrYear"]) : 0;

				model.ApprovedAmount =
					row["ApprovedAmount"] != DBNull.Value ? Convert.ToDecimal(row["ApprovedAmount"]) : 0;

				model.PreviousPendAdvanceAmt =
					row["PreviousPendAdvanceAmt"] != DBNull.Value ? Convert.ToDecimal(row["PreviousPendAdvanceAmt"]) : 0;

				model.PreviousPendLoanAmt =
					row["PreviousPendLoanAmt"] != DBNull.Value ? Convert.ToDecimal(row["PreviousPendLoanAmt"]) : 0;

				/* ------------------- Manager Approval ------------------- */

				model.MgrApprovaldate = row["MgrApprovaldate"]?.ToString();
				model.MgrApprovedbyEmpName = row["ApprovByMgr"]?.ToString();
				model.MgrApprovedbyEmpid =
					row["MgrEmpId"] != DBNull.Value ? Convert.ToInt32(row["MgrEmpId"]) : null;

				/* ------------------- HR Approval ------------------------- */

				model.HRApprovedbyEmpName = row["ApprovByHR"]?.ToString();
				model.HRApprovedbyEmpid =
					row["HREmpid"] != DBNull.Value ? Convert.ToInt32(row["HREmpid"]) : null;

				model.HRApprovalDate = row["HRApprovalDate"]?.ToString();

				/* ------------------- Finance Approval -------------------- */

				model.FinanceApprovalEmpid =
					row["FinEmpId"] != DBNull.Value ? Convert.ToInt32(row["FinEmpId"]) : null;

				model.FinanceApprovalEmpCode = row["ApprByFinCode"]?.ToString();
				model.FinanceApprovalEmpName = row["ApprovByFin"]?.ToString();
				model.FinanceApprovalDate = row["FinanceApprovalDate"]?.ToString();

				/* ------------------- Cancellation ------------------------ */

				model.Canceled = row["Canceled"]?.ToString();
				model.CancelOrApprovalremarks = row["CancelOrApprovalremarks"]?.ToString();
				model.CanceledByEmpId =
					row["CanceledByEmpId"] != DBNull.Value ? Convert.ToInt32(row["CanceledByEmpId"]) : null;

				/* ------------------- Payment Details --------------------- */

				model.ModeOfPayment = row["ModeOfPayment"]?.ToString();
				model.PaymentReferenceNo = row["PaymentReferenceNo"]?.ToString();
				model.PaymentVoucherNo = row["PaymentVoucherNo"]?.ToString();
				model.PaymentVoucherType = row["PaymentVoucherType"]?.ToString();
				model.PaymentRemark = row["PaymentRemark"]?.ToString();

				/* ------------------- Recovery Details -------------------- */

				model.RecoveryMethod = row["RecoveryMethod"]?.ToString();
				model.NoofInstallment =
					row["NoofInstallment"] != DBNull.Value ? Convert.ToInt32(row["NoofInstallment"]) : 0;

				model.StartRecoveryFromMonth = row["StartRecoveryFromMonth"]?.ToString();
				model.AutoDeductionFromSalaryYN = row["AutoDeductionFromSalaryYN"]?.ToString();
				model.FinalRevoveryDate = row["FinalRevoveryDate"]?.ToString();
				model.ActualFinalRecoveryDate = row["ActualFinalRecoveryDate"]?.ToString();

				model.StatusHoldCancelApproved = row["StatusHoldCancelApproved"]?.ToString();

				/* ------------------- Machine ----------------------------- */

				model.RequestEntryByMachine = row["RequestEntryByMachine"]?.ToString();
				model.ApprovedBYMachine = row["ApprovedBYMachine"]?.ToString();
				model.CancelByMachine = row["CancelByMachine"]?.ToString();

				/* ------------------- Actual Entry ------------------------ */

				model.ActualEntryBy =
					row["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(row["ActualEntryBy"]) : null;

				model.ActualEntryByName = row["ActualEntryBy"]?.ToString();     // actualEntryBy emp_name--->code

				/* ------------------- Common Fields ----------------------- */

				model.CC = row["CC"]?.ToString();
				model.UID = row["UID"] != DBNull.Value ? Convert.ToInt32(row["UID"]) : 0;

				/* ------------------- IDs from SELECT --------------------- */

				model.EmpId = row["EmpId"] != DBNull.Value ? Convert.ToInt32(row["EmpId"]) : 0;
				model.DesigId = row["DesigId"] != DBNull.Value ? Convert.ToInt32(row["DesigId"]) : 0;
				model.DeptId = row["DepId"] != DBNull.Value ? Convert.ToInt32(row["DepId"]) : 0;
				model.CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : 0;

				model.AdvanceEntryId = row["AdvanceEntryId"] != DBNull.Value ? Convert.ToInt32(row["AdvanceEntryId"]) : 0;
				model.AdvanceYearCode = row["AdvanceYearCode"] != DBNull.Value ? Convert.ToInt32(row["AdvanceYearCode"]) : 0;


				if (mode == "U" || mode == "V")
                {
                    if (DS.Tables[0].Rows[0]["UpdatedByEMp"].ToString() != "")
                    {
                        model.LastUpdatedBy = DS.Tables[0].Rows[0]["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]) : null;
                        model.LastUpdatedByEmpName = DS.Tables[0].Rows[0]["UpdatedByEMp"]?.ToString();
                        model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationdate"]?.ToString();
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
