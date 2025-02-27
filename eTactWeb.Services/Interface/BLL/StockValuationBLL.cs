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

        public StockValuationBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _StockValuationDAL = new StockValuationDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillStoreName(string FromDate, string CurrentDate)
        {
            return await _StockValuationDAL.FillStoreName(FromDate, CurrentDate);
        }
        public async Task<StockValuationModel> GetStockValuationDetailsData(string FromDate, string ToDate,string StoreId,string ReportType)
        {
            return await _StockValuationDAL.GetStockValuationDetailsData(FromDate, ToDate, StoreId, ReportType);
        }
    }
}
