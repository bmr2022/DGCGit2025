using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class EmployeeAdvancePayementBLL : IEmployeeAdvancePayement
    {
        private EmployeeAdvancePayementDAL _EmployeeAdvancePayementDAL;
        private readonly IDataLogic _DataLogicDAL;

        public EmployeeAdvancePayementBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _EmployeeAdvancePayementDAL = new EmployeeAdvancePayementDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillEntryId(int yearCode)
        {
            return await _EmployeeAdvancePayementDAL.FillEntryId(yearCode);
        }
        public async Task<HRAdvanceModel> GetViewByID(int ID,int yearCode, string mode)
        {
            return await _EmployeeAdvancePayementDAL.GetViewByID(ID,yearCode, mode);
        }
        public async Task<ResponseResult> FillAdvanceType()
        {
            return await _EmployeeAdvancePayementDAL.FillAdvanceType();
        }
        public async Task<ResponseResult> FillPreviousAdanaceLoanDetail(int empId, string requestedDate)
		{
            return await _EmployeeAdvancePayementDAL.FillPreviousAdanaceLoanDetail(empId,requestedDate);
        }
        public async Task<ResponseResult> FillEmpName()
        {
            return await _EmployeeAdvancePayementDAL.FillEmpName();
        }
        public async Task<ResponseResult> FillDeptName()
        {
            return await _EmployeeAdvancePayementDAL.FillDeptName();
        }
        public async Task<ResponseResult> FillFinancialEmployeeNameList()
        {
            return await _EmployeeAdvancePayementDAL.FillFinancialEmployeeNameList();
        }
        public async Task<ResponseResult> FillMgmtEmployeeNameList()
        {
            return await _EmployeeAdvancePayementDAL.FillMgmtEmployeeNameList();
        }
        public async Task<ResponseResult> FillEmpCode()
        {
            return await _EmployeeAdvancePayementDAL.FillEmpCode();
        }
        public async Task<ResponseResult> FillEmployeeDetail(int empId)
        {
            return await _EmployeeAdvancePayementDAL.FillEmployeeDetail(empId);
        }
        public async Task<ResponseResult> SaveEmployeeAdvancePayment(HRAdvanceModel model)
        {
            return await _EmployeeAdvancePayementDAL.SaveEmployeeAdvancePayment(model);
        }
        public async Task<ResponseResult> GetDashboardData(string fromDate, string toDate, string employeeName ="", string deptName = "", string empCode = "")
        {
            return await _EmployeeAdvancePayementDAL.GetDashboardData(fromDate, toDate,employeeName, deptName, empCode);
        }
        public async Task<ResponseResult> DeleteByID(int advEntryId, int advYearCode, int actualEntryBy, string entryByMachineName, string entryDate)
        {
            return await _EmployeeAdvancePayementDAL.DeleteByID(advEntryId, advYearCode,actualEntryBy,entryByMachineName,entryDate);
        }
    }
}
