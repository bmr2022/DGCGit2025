using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface ITrailBalance
    {
        public Task<TrailBalanceModel> GetTrailBalanceDetailsData(string FromDate, string ToDate, int TrailBalanceGroupCode, string ReportType);
    }
}
