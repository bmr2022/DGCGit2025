using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class DashboardDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public DashboardDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> FillInventoryDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "INVENTORY VALAUTION And STOCK And Dead Inventory"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInventoryDashboardForPendingData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "INVENTORY PENDINGLIST"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTopItemByStockValue()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "TopItemByStockValue"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTopItemBelowMinLevel()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "TopItemBelowMinLevel"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetTopFastMovingItem()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "TopFastMovingItem"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInventoryByCategory()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "INVENTORYBYCATEGORY"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> NoOfItemInStock()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "NoOfItemInStock"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> StockValuation()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "StockValuation"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> PendingInventoryTask()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "PendingInventoryTask"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> DeadStockInventoryTask()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "DeadStockInventory"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FastMovingItemsListTask()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "FastMovingItemsList"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FastVsSlowMovingTask()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "INVENTORY"));
                SqlParams.Add(new SqlParameter("@FLAG", "Fast Vs Slow Moving"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PENDENCY IN PURCHASE And NoOfPO"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardDataByCategoryValue()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO Category Wise"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardDataMonthlyTrend()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO Monthly Trend"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardTop10ItemData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 Item"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardTop10VendorData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 Supplier"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseDashboardNewVendorInThisMonthData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "NEW VENDORE OF THE MONTH"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPurchaseVsConsumptionDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO vs Consumption"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillBestSupplier()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "BEST SUPPLIER"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorstSupplier()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "WORST SUPPLIER"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveNoOfPOItemsAndPending()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PENDENCY IN PURCHASE And NoOfPO"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTop10ItemForPO()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 Item"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTop10VendorForPO()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 Supplier"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SavePOMonthlyTrend()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO Monthly Trend"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SavePOCategoryWise()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO Category Wise"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SavePOVsConsumption()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "PO vs Consumption"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveNewVendorOfTheMonth()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "PURCHASE"));
                SqlParams.Add(new SqlParameter("@FLAG", "NEW VENDORE OF THE MONTH"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveBestAndWorstSupplier()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ReportType ", "PURCHASE DASHBOARD VENDOR RATING"));
                SqlParams.Add(new SqlParameter("@FLAG", ""));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportVendoreRatingAnalysisAndVendoreReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDisplaySalesHeading()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Dispaly Sales Heading"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSALEMonthlyTrend()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "SALE Monthly Trend"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTop10SALECUSTOMER()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SALE CUSTOMER"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTop10SOLDItem()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SOLD Item"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillTop10SALESPERSON()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SALES PERSON"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillNEWCustomerOFTHEMONTH()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "NEW Customer OF THE MONTH"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillMonthlyRejectionTrend()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Monthly Rejection Trend"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleOrderVsDispatch()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "SaleOrder Vs Dispatch"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPXONDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveDisplaySalesHeading()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Dispaly Sales Heading"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveNewCustomerOfTheMonth()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "NEW Customer OF THE MONTH"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTop10SaleCustomer()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SALE CUSTOMER"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTop10SoldItem()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SOLD Item"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveSaleMonthlyTrend()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "SALE Monthly Trend"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveTop10SalesPerson()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));
                SqlParams.Add(new SqlParameter("@FLAG", "Top 10 SALES PERSON"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveSaleOrderVsDispatch()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@DashboardType ", "SALE"));   
                SqlParams.Add(new SqlParameter("@FLAG", "SaleOrder Vs Dispatch"));
                SqlParams.Add(new SqlParameter("@CurrentDate", DateTime.UtcNow));
                SqlParams.Add(new SqlParameter("@CurrentYear", DateTime.UtcNow.Year));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPDashboardCalculation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
    }
}
