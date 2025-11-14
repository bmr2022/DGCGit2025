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
        public async Task<ResponseResult> FillEmpName()
        {
            return await _EmployeeAdvancePayementDAL.FillEmpName();
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
    }
}
