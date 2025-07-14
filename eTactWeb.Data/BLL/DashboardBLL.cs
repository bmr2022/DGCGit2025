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
        public async Task<ResponseResult> DeadStockInventoryTask()
        {
            return await _DashboardDAL.DeadStockInventoryTask();
        }
        public async Task<ResponseResult> FastMovingItemsListTask()
        {
            return await _DashboardDAL.FastMovingItemsListTask();
        }
        public async Task<ResponseResult> FastVsSlowMovingTask()
        {
            return await _DashboardDAL.FastVsSlowMovingTask();
        }
        public async Task<ResponseResult> FillPurchaseDashboardData()
        {
            return await _DashboardDAL.FillPurchaseDashboardData();
        }
        public async Task<ResponseResult> FillPurchaseDashboardDataByCategoryValue()
        {
            return await _DashboardDAL.FillPurchaseDashboardDataByCategoryValue();
        }
        public async Task<ResponseResult> FillPurchaseDashboardTop10ItemData()
        {
            return await _DashboardDAL.FillPurchaseDashboardTop10ItemData();
        }
        public async Task<ResponseResult> FillPurchaseDashboardTop10VendorData()
        {
            return await _DashboardDAL.FillPurchaseDashboardTop10VendorData();
        }
        public async Task<ResponseResult> FillPurchaseDashboardDataMonthlyTrend()
        {
            return await _DashboardDAL.FillPurchaseDashboardDataMonthlyTrend();
        }
        public async Task<ResponseResult> SaveNoOfPOItemsAndPending()
        {
            return await _DashboardDAL.SaveNoOfPOItemsAndPending();
        }
        public async Task<ResponseResult> SaveTop10ItemForPO()
        {
            return await _DashboardDAL.SaveTop10ItemForPO();
        }
        public async Task<ResponseResult> SaveTop10VendorForPO()
        {
            return await _DashboardDAL.SaveTop10VendorForPO();
        }
    }
}
