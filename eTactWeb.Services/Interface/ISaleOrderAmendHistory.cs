using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISaleOrderAmendHistory
    {
        Task<SaleOrderAmendHistoryModel> SaleOrderAmendHistory(
            string reportType,
            string flag,
            string dashboardFlag,
            string fromDate,
            string toDate,
            int accountCode,
            string poNo,
            string partCode,
            int itemCode,
            string soNo);
        Task<ResponseResult> FillItemNamePartcode(string FromDate, string ToDate, int AccountCode, string SOno);
        Task<ResponseResult> FillSONO(string FromDate, string ToDate, int AccountCode);
        Task<ResponseResult> FillAccountCode(string FromDate, string ToDate, string partCode, string ItemCode, string SOno);
        Task<ResponseResult> FillPONO(string FromDate, string ToDate, int AccountCode);
    }


}
