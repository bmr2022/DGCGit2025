using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingToReceiveItem
    {
        Task<DataSet> BindItem(string Flag,string FromDate,string ToDate);
        Task<DataSet> BindPartCode(string Flag, string FromDate, string ToDate);
        Task<DataSet> BindWorkCenter(string Flag, string FromDate, string ToDate);
        Task<DataSet> BindProdSlipNo(string Flag, string FromDate, string ToDate);
        Task<DataSet> BindStoreName(string Flag, string FromDate, string ToDate);
        Task<DataSet> BindProdType(string Flag, string FromDate, string ToDate);
        Task<ResponseResult> GetDataForPendingReceiveItem(string Flag, string FromDate, string ToDate);
        Task<ResponseResult> GetDataReceiveItem(DataTable DisplayPendReceiveItem);
    }
}
