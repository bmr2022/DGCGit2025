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
    public class CurrencyMasterBLL:ICurrencyMaster
    {
        private CurrencyMasterDAL _CurrencyMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CurrencyMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _CurrencyMasterDAL = new CurrencyMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> SaveCurrencyMaster(CurrencyMasterModel model)
        {
            return await _CurrencyMasterDAL.SaveCurrencyMaster(model);
        }

        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _CurrencyMasterDAL.GetDashBoardData();
        }

        public async Task<CurrencyMasterModel> GetDashBoardDetailData()
        {
            return await _CurrencyMasterDAL.GetDashBoardDetailData();
        }

        public async Task<CurrencyMasterModel> GetViewByID(int ID)
        {
            return await _CurrencyMasterDAL.GetViewByID(ID);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _CurrencyMasterDAL.DeleteByID(ID);
        }
    }
}
