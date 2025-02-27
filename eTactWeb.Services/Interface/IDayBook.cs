using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDayBook
    {
        public Task<DayBookModel> GetDayBookDetailsData(string FromDate, string ToDate, string Ledger, string VoucherType, string CrAmt, string DrAmt);
        public Task<ResponseResult> FillLedgerName(string FromDate, string ToDate);
        public Task<ResponseResult> FillVoucherName(string FromDate, string ToDate);
    }
}
