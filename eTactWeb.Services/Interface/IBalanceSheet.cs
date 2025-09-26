using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBalanceSheet
    {
        Task<BalanceSheetModel> GetBalanceSheetData(string FromDate, string ToDate, string ReportType, int? BalParentAccountCode);
        Task<ResponseResult> GetLiabilitiesAndAssetsData(string FromDate, string ToDate);
        Task<ResponseResult> GetParentAccountData(string FromDate, string ToDate);
        Task<ResponseResult> GetAccountData(string FromDate, string ToDate);
    }
}
