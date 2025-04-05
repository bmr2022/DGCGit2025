using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class StockValuationBLL:IStockValuation
    {
        private StockValuationDAL _StockValuationDAL;
        private readonly IDataLogic _DataLogicDAL;

        public StockValuationBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _StockValuationDAL = new StockValuationDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillStoreName(string FromDate, string CurrentDate)
        {
            return await _StockValuationDAL.FillStoreName(FromDate, CurrentDate);
        }
        public async Task<ResponseResult> FillItemName(string FromDate, string CurrentDate)
        {
            return await _StockValuationDAL.FillItemName(FromDate, CurrentDate);
        }
        public async Task<ResponseResult> FillPartCode(string FromDate, string CurrentDate)
        {
            return await _StockValuationDAL.FillPartCode(FromDate, CurrentDate);
        }
        public async Task<StockValuationModel> GetStockValuationDetailsData(string FromDate, string ToDate,string StoreId,string ReportType, int ItemCode)
        {
            return await _StockValuationDAL.GetStockValuationDetailsData(FromDate, ToDate, StoreId, ReportType, ItemCode);
        }
    }
}
