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
    }
}
