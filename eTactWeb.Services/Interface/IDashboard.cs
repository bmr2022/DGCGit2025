using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDashboard
    {
        Task<ResponseResult> FillInventoryDashboardData();
        Task<ResponseResult> FillInventoryDashboardForPendingData();
        Task<ResponseResult> GetTopItemByStockValue();
        Task<ResponseResult> FillInventoryByCategory();
        Task<ResponseResult> NoOfItemInStock();
        Task<ResponseResult> StockValuation();
        Task<ResponseResult> GetTopItemBelowMinLevel();
        Task<ResponseResult> GetTopFastMovingItem();
        Task<ResponseResult> PendingInventoryTask();
        Task<ResponseResult> FillPurchaseDashboardData();
        Task<ResponseResult> FillPurchaseDashboardTop10ItemData();
        Task<ResponseResult> DeadStockInventoryTask();
        Task<ResponseResult> FastMovingItemsListTask();
        Task<ResponseResult> FastVsSlowMovingTask();
        Task<ResponseResult> SaveNoOfPOItemsAndPending();
        Task<ResponseResult> SaveTop10ItemForPO();
    }
}
