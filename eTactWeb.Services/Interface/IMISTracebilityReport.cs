using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMISTracebilityReport
    {
        Task<MISTracebilityReportModel> GetMISTracebilityReportData(string FromDate, string ToDate, string SaleBillNo);
        Task<ResponseResult> FillSaleBillNoList(string FromDate, string ToDate);
    }
}
