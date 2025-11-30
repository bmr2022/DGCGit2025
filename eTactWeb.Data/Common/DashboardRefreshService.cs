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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var today930 = now.Date.AddHours(9).AddMinutes(30);
                var today630 = now.Date.AddHours(18).AddMinutes(30);

                // First run
                DateTime nextRun;

                if (now < today930)
                    nextRun = today930;
                else if (now < today630)
                    nextRun = today630;
                else
                    nextRun = today930.AddDays(1);

                var delay = nextRun - DateTime.Now;

                _logger.LogInformation($"Dashboard refresh scheduled at: {nextRun}");

                await Task.Delay(delay, stoppingToken);

                await RefreshDashboardData();
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
