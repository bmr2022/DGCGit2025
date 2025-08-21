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
    public class SalepersonWiseRateMasterBLL:ISalepersonWiseRateMaster
    {
        private SalepersonWiseRateMasterDAL _SalepersonWiseRateMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public SalepersonWiseRateMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _SalepersonWiseRateMasterDAL = new SalepersonWiseRateMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _SalepersonWiseRateMasterDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> FillSalePerson()
        {
            return await _SalepersonWiseRateMasterDAL.FillSalePerson();
        }
        public async Task<SalepersonWiseRateMasterModel> GetItemData(int ItemGroupId)
        {
            return await _SalepersonWiseRateMasterDAL.GetItemData(ItemGroupId);
        }
        public async Task<ResponseResult> SaveSalePersonWiseRate(DataTable DTItemGrid, SalepersonWiseRateMasterModel model)
        {
            return await _SalepersonWiseRateMasterDAL.SaveSalePersonWiseRate(DTItemGrid, model);
        }
    }
}
