using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICancelSaleBill
    {
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string SaleBillNo, string CustomerName, string CanRequisitionNo);
        Task<ResponseResult> FillCanRequisitionNo(string CurrentDate, int accountcode, string SaleBillNo);
        Task<ResponseResult> FillSaleBillNo(string CurrentDate, int accountcode);
        Task<ResponseResult> FillCustomerName(string CurrentDate, string SaleBillNo);

        Task<List<CancelSaleBillDetails>> ShowSaleBillDetail(int SaleBillEntryId, int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo);

        Task<ResponseResult> SaveCancelation( int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo,int CancelBy,String Cancelreason,String Canceldate); Task<ResponseResult> CancelEInvoice(int SaleBillYearCode, string CanRequisitionNo, string SaleBillNo, string CustomerName);
    }
}
