using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class OrderBasedProdPlanBLL : IOrderBasedProdPlan
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly OrderBasedProdPlanDAL _OrderBasedProdPlanDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderBasedProdPlanBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _OrderBasedProdPlanDAL = new OrderBasedProdPlanDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<OrderBasedProdPlanModel> FillSONO_OrderNO_SchNo(string FromDate, string ToDate)
        {
            return await _OrderBasedProdPlanDAL.FillSONO_OrderNO_SchNo(FromDate, ToDate);
        }
        public async Task<OrderBasedProdPlanModel> GetOrderBasedProdPlanData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, int ItemCode)
        { 
            return await _OrderBasedProdPlanDAL.GetOrderBasedProdPlanData(FromDate, ToDate, ReportType, AccountCode, PartCode, ItemName, ItemCode);
        }
    }
}
