using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingSaleRejection
    {
        Task<ResponseResult> PendingMRNForSaleRejection(string fromDate, string toDate, string mrnNo, string gateNo, string customerName);
        Task<ResponseResult> FillMRNNO(string fromDate, string toDate, string mrnNo, string gateNo);
        Task<ResponseResult> FillPartyName(string fromDate, string toDate);
        Task<ResponseResult> FillGateNO(string fromDate, string toDate, string mrnNo, string gateNo);
        Task<ResponseResult> FillCustomerInvNO(string fromDate, string toDate, string mrnNo, string gateNo);
    }
}
