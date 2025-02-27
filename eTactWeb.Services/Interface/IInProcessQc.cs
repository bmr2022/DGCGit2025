using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IInProcessQc
    {
        Task<InProcessQc> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName);
        Task<DataSet> BindEmpList(string Flag);
        Task<ResponseResult> SaveInprocessQc(InProcessQc model, DataTable InProcessQcGrid);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> FillTransferToWorkCenter();
        Task<ResponseResult> FillTransferToStore();
        Task<ResponseResult> FillReworkreason();
        Task<ResponseResult> FillRejectionreason();
        Task<InProcessDashboard> GetDashboardData(string FromDate, string ToDate, string QcSlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ProdSlipNo, string DashboardType);
        Task<InProcessDashboard> GetDashboardDetailData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate);
    }
}
