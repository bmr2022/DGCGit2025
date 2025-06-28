using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    }
}
