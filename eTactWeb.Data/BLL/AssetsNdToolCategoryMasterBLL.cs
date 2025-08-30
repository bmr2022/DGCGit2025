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
    public class AssetsNdToolCategoryMasterBLL : IAssetsNdToolCategoryMaster
    {
        private AssetsNdToolCategoryMasterDAL _AssetsNdToolCategoryMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AssetsNdToolCategoryMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _AssetsNdToolCategoryMasterDAL = new AssetsNdToolCategoryMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }


        public async Task<AssetsNdToolCategoryMasterModel> GetNewEntryId()
        {
            return await _AssetsNdToolCategoryMasterDAL.GetNewEntryId();
        }

        // Unified SaveAsync → handles Insert / Update
        public async Task<ResponseResult> SaveAsync(AssetsNdToolCategoryMasterModel model)
        {
            return await _AssetsNdToolCategoryMasterDAL.SaveAsync(model);
        }

        public async Task<ResponseResult> DeleteAsync(long id, string categoryName)
        {
            return await _AssetsNdToolCategoryMasterDAL.DeleteAsync(id, categoryName);
        }

        public async Task<List<AssetsNdToolCategoryMasterModel>> GetDashboardAsync()
        {
            return await _AssetsNdToolCategoryMasterDAL.GetDashboardAsync();
        }

        public async Task<AssetsNdToolCategoryMasterModel?> ViewByIdAsync(long id, string categoryName)
        {
            return await _AssetsNdToolCategoryMasterDAL.ViewByIdAsync(id, categoryName);
        }
    }
}



