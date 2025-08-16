using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
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
    public class WorkCenterMasterBLL : IWorkCenterMaster
    {
        private WorkCenterMasterDAL _WorkCenterMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public WorkCenterMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _WorkCenterMasterDAL = new WorkCenterMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<WorkCenterMasterModel> GetDashboardData(WorkCenterMasterModel model)
        {
            return await _WorkCenterMasterDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _WorkCenterMasterDAL.GetFormRights(ID);
        }

        public async Task<ResponseResult> SaveData(WorkCenterMasterModel model)
        {
            return await _WorkCenterMasterDAL.SaveData(model);
        }


        public async Task<ResponseResult> DeleteMachine(string WorkCenterDescription)
        {
            return await _WorkCenterMasterDAL.DeleteMachine(WorkCenterDescription);
        }

        public async Task<WorkCenterMasterModel> ViewByID(int id)
        {
            return await _WorkCenterMasterDAL.ViewByID(id);
        }
    }
}
