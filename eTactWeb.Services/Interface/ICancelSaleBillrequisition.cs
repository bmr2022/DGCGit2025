using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface  ICancelSaleBillrequisition
    {
        Task<ResponseResult> FillSaleBillNo(string  CurrentDate,string SaleBillDate);
        Task<ResponseResult> FillCanSaleBillReqEntryid(string  CurrentDate,string SaleBillDate,int CanSaleBillReqYearcode);
        Task<ResponseResult> FillCanRequisitionNo(string  CurrentDate,string SaleBillDate,int CanSaleBillReqYearcode);
        Task<ResponseResult> FillSaleBillNoYear(string  CurrentDate,string SaleBillDate,string SaleBillNo );
        Task<ResponseResult> FillCustomerName(string  CurrentDate,string SaleBillDate );
        Task<ResponseResult> FillSaleBillNoDate(string  CurrentDate,string SaleBillNo,string SaleBillYearCode, string SaleBillDate);
        Task<ResponseResult> SaveCancelSaleBillRequisition(CancelSaleBillrequisitionModel model);
        Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<CancelSaleBillrequisitionModel> GetDashBoardDetailData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID, string CanReqNo, string MachineName, int CanSaleBillReqYC);
        Task<CancelSaleBillrequisitionModel> GetViewByID(string CanRequisitionNo, int CanSaleBillReqYearcode);
    }
}
