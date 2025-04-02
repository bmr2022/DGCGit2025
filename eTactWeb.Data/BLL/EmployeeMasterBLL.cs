using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class EmployeeMasterBLL : IEmployeeMaster
    {
        private EmployeeMasterDAL _EmployeeMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public EmployeeMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _EmployeeMasterDAL = new EmployeeMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<DOM.Models.Common.ResponseResult> DeleteByID(int ID, string EmpName)
        {
            return await _EmployeeMasterDAL.DeleteByID(ID, EmpName);
        }

        public async Task<EmployeeMasterModel> GetByID(int ID)
        {
            return await _EmployeeMasterDAL.GetByID(ID);
        }

        public async Task<EmployeeMasterModel> GetDashboardData(EmployeeMasterModel model)
        {
            return await _EmployeeMasterDAL.GetDashboardData(model);
        }
        public Task<DOM.Models.Common.ResponseResult> GetFormRights(int uId)
        {
            return _EmployeeMasterDAL.GetFormRights(uId);
        }

        public async Task<EmployeeMasterModel> GetSearchData(EmployeeMasterModel model, string EmpCode)
        {
            return await _EmployeeMasterDAL.GetSearchData(model, EmpCode);
        }
        public async Task<ResponseResult> GetEmpIdandEmpCode(string designation, string department)
        {
            return await _EmployeeMasterDAL.GetEmpIdandEmpCode(designation, department);
        }

        public async Task<DOM.Models.Common.ResponseResult> SaveEmployeeMaster(EmployeeMasterModel model)
        {
            return await _EmployeeMasterDAL.SaveEmployeeMaster(model);
        }
    }
}