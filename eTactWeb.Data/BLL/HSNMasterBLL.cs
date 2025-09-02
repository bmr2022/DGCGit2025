using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class HSNMasterBLL : IHSNMaster
    {
        private readonly HSNMasterDAL _HSNMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HSNMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _HSNMasterDAL = new HSNMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<IEnumerable<HSNMasterModel>> GetDashboard()
        {
            return await _HSNMasterDAL.GetDashboard();
        }

        public async Task<HSNMasterModel> GetById(int id)
        {
            return await _HSNMasterDAL.GetById(id);
        }

        public async Task<ResponseResult> Insert(HSNMasterModel model)
        {
            model.Mode = "I";
            return await _HSNMasterDAL.SaveHSN(model);
        }

        public async Task<ResponseResult> Update(HSNMasterModel model)
        {
            model.Mode = "U";
            return await _HSNMasterDAL.SaveHSN(model);
        }

        public async Task<ResponseResult> Delete(int id)
        {
            return await _HSNMasterDAL.Delete(id);
        }

        public async Task<int> GetNewEntryId()
        {
            return await _HSNMasterDAL.GetNewEntryId();
        }
    }
}
