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

namespace eTactWeb.Data.BLL
{
    public class OrderAmendHistoryBLL:IOrderAmendHistory
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly OrderAmendHistoryDAL _OrderAmendHistoryDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderAmendHistoryBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _OrderAmendHistoryDAL = new OrderAmendHistoryDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }

        public async Task<OrderAmendHistoryModel> GetOrderAmendHistoryData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, string PONO, int ItemCode,string HistoryReportMode)
        {
            return await _OrderAmendHistoryDAL.GetOrderAmendHistoryData( FromDate,  ToDate,  ReportType,  AccountCode,  PartCode,  ItemName,  PONO,  ItemCode, HistoryReportMode);
        }
    }
}
