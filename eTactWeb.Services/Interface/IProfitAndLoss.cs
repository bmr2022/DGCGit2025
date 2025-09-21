using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IProfitAndLoss
    {
        Task<ProfitAndLossModel> GetProfitAndLossData(string FromDate, string ToDate, string Flag, string ReportType, string ShowOpening, string ShowRecordWithZeroAmt, int? ParentAccountCode);
        Task<ResponseResult> GetGroupData(string FromDate, string ToDate, string ReportType, string ShowOpening, string ShowRecordWithZeroAmt);
    }
}
