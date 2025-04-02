using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class DepartmentMasterBLL:IDepartmentMaster
    {
        private DepartmentMasterDAL _DepartmentMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public DepartmentMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _DepartmentMasterDAL = new DepartmentMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillDeptType()
        {
            return await _DepartmentMasterDAL.FillDeptType();
        }

        public async Task<ResponseResult> FillDeptID()
        {
            return await _DepartmentMasterDAL.FillDeptID();
        }

        public async Task<ResponseResult> SaveDeptMaster(DepartmentMasterModel model)
        {
            return await _DepartmentMasterDAL.SaveDeptMaster(model);
        }

        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _DepartmentMasterDAL.GetDashBoardData();
        }
        public async Task<DepartmentMasterModel> GetDashBoardDetailData()
        {
            return await _DepartmentMasterDAL.GetDashBoardDetailData();
        }

        public async Task<DepartmentMasterModel> GetViewByID(int ID)
        {
            return await _DepartmentMasterDAL.GetViewByID(ID);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _DepartmentMasterDAL.DeleteByID(ID);
        }

    }
}
