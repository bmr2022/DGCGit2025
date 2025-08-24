using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITrailBalance
    {
        public Task<TrailBalanceModel> GetTrailBalanceDetailsData(string FromDate, string ToDate, int? TrailBalanceGroupCode, string ReportType);
        Task<ResponseResult> FillGroupList(string FromDate, string ToDate);
        Task<ResponseResult> FillParentGroupList(string FromDate, string ToDate, int? GroupCode);
        Task<ResponseResult> FillAccountList(string FromDate, string ToDate, int? GroupCode, int? ParentAccountCode);
    }
}
