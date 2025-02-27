using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingInProcessToQc
    {
        Task<DataSet> BindItem(string Flag);
        Task<DataSet> BindProdSlip(string Flag);
        Task<DataSet> BindWorkCenter(string Flag);
        Task<ResponseResult> GetDataForPendingInProcess(string Flag, string FromDate, string ToDate, string PartCode, string ItemName, string ProdSlipNo, string WorkCenter, string GlobalSearch);
        Task<ResponseResult> GetDataforQc(DataTable DisplayPendForQC, string Flag, string FromDate, string ToDate);
    }
}
