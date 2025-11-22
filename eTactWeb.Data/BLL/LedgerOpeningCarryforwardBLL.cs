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
    public class LedgerOpeningCarryforwardBLL : ILedgerOpeningCarryforward
    {
        private LedgerOpeningCarryforwardDAL _LedgerOpeningCarryforwardDAL;
        private readonly IDataLogic _DataLogicDAL;

        public LedgerOpeningCarryforwardBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _LedgerOpeningCarryforwardDAL = new LedgerOpeningCarryforwardDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _LedgerOpeningCarryforwardDAL.GetDashboardData();
        }

        public async Task<ResponseResult> FILLFINYEAR()
        {
            return await _LedgerOpeningCarryforwardDAL.FILLFINYEAR();
        }
        public async Task<ResponseResult> CarryforwardLedgerbalance(LedgerOpeningCarryforwardModel model)
        {
            return await _LedgerOpeningCarryforwardDAL.CarryforwardLedgerbalance(model);
        }
    }
}
