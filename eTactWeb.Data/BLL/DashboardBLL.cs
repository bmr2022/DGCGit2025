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
    public class DashboardBLL : IDashboard
    {
        private DashboardDAL _DashboardDAL;
        private readonly IDataLogic _DataLogicDAL;

        public DashboardBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _DashboardDAL = new DashboardDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillInventoryDashboardData()
        {
            return await _DashboardDAL.FillInventoryDashboardData();
        }
        public async Task<ResponseResult> FillInventoryDashboardForPendingData()
        {
            return await _DashboardDAL.FillInventoryDashboardForPendingData();
        }
        public async Task<ResponseResult> FillInventoryByCategory()
        {
            return await _DashboardDAL.FillInventoryByCategory();
        }
        public async Task<ResponseResult> GetTopItemByStockValue()
        {
            return await _DashboardDAL.GetTopItemByStockValue();
        }
        public async Task<ResponseResult> GetTopItemBelowMinLevel()
        {
            return await _DashboardDAL.GetTopItemBelowMinLevel();
        }
        public async Task<ResponseResult> NoOfItemInStock()
        {
            return await _DashboardDAL.NoOfItemInStock();
        }
        public async Task<ResponseResult> StockValuation()
        {
            return await _DashboardDAL.StockValuation();
        }
        public async Task<ResponseResult> GetTopFastMovingItem()
        {
            return await _DashboardDAL.GetTopFastMovingItem();
        }
        public async Task<ResponseResult> PendingInventoryTask()
        {
            return await _DashboardDAL.PendingInventoryTask();
        }
    }
}
