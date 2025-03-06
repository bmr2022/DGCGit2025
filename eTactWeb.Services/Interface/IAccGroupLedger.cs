using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAccGroupLedger
    {
        Task<ResponseResult> FillGroupName(string FromDate,string ToDate);
        public Task<AccGroupLedgerModel> GetGroupLedgerDetailsData(string FromDate, string ToDate, int GroupCode, string ReportType);


    }
}
