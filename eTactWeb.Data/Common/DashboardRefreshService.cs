using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using eTactWeb.Data.DAL;

public class DashboardRefreshService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DashboardRefreshService> _logger;

    public DashboardRefreshService(IServiceProvider serviceProvider, ILogger<DashboardRefreshService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Dashboard Refresh Service started (TEST MODE - every 1 minute)");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Run the dashboard refresh
                await RefreshDashboardData();

                _logger.LogInformation("Dashboard data refresh completed at: " + DateTime.Now);

                // WAIT for 1 minute
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in dashboard refresh background service");
            }
        }
    }

    private async Task RefreshDashboardData()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dal = scope.ServiceProvider.GetRequiredService<DashboardDAL>();

            await dal.FillInventoryDashboardData();
            await dal.FillInventoryDashboardForPendingData();
            await dal.GetTopItemByStockValue();
            await dal.GetTopItemBelowMinLevel();
            await dal.GetTopFastMovingItem();
            await dal.FillInventoryByCategory();
            await dal.NoOfItemInStock();
            await dal.StockValuation();
            await dal.PendingInventoryTask();
            await dal.DeadStockInventoryTask();
            await dal.FastMovingItemsListTask();
            await dal.FastVsSlowMovingTask();

            // Purchase
            await dal.FillPurchaseDashboardData();
            await dal.FillPurchaseDashboardDataByCategoryValue();
            await dal.FillPurchaseDashboardDataMonthlyTrend();
            await dal.FillPurchaseDashboardTop10ItemData();
            await dal.FillPurchaseDashboardTop10VendorData();
            await dal.FillPurchaseDashboardNewVendorInThisMonthData();
            await dal.FillPurchaseVsConsumptionDashboardData();
            await dal.FillBestSupplier();
            await dal.FillWorstSupplier();

            // Sales
            await dal.FillDisplaySalesHeading();
            await dal.FillSALEMonthlyTrend();
            await dal.FillTop10SALECUSTOMER();
            await dal.FillTop10SOLDItem();
            await dal.FillTop10SALESPERSON();
            await dal.FillNEWCustomerOFTHEMONTH();
            await dal.FillMonthlyRejectionTrend();
            await dal.FillSaleOrderVsDispatch();
        }
    }
}
