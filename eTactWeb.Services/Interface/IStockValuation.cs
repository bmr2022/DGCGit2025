using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IStockValuation
    {
        public Task<StockValuationModel> GetStockValuationDetailsData(string FromDate, string ToDate,string StoreId,string ReportType);
        public Task<ResponseResult> FillStoreName(string FromDate, string CurrentDate);

    }
}
