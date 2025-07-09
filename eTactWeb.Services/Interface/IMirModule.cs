using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMirModule
    {
        Task<DataSet> BindBranch(string Flag);
        Task<ResponseResult> GetMRNNo(string Flag, string SPName, string FromDate, string ToDate, string MRNCustJW);
        Task<ResponseResult> AddPassWord();

        Task<ResponseResult> GetMRNData(string Flag, string SPName, string MRNNo, int MRNYearCode,int GateNo,int GateYear,int GateEntryId, string MRNCustJW);
        Task<MirModel> GetMIRMainItem(string Flag, string SPName, string MRNNo, int MRNYearCode, int GateNo, int GateYear, int GateEntryId, string MRNCustJW);
        Task<ResponseResult> GetMIRFromPend(string Flag, string SPName, string MRNNo, int MRNYearCode,string MRNCustJW);
        Task<ResponseResult> GetStore(string Flag, string SPName);
        Task<ResponseResult> GetRewStore(string Flag, string SPName);
        Task<ResponseResult> GetHoldStore(string Flag, string SPName);
        Task<ResponseResult> GetRecOkStore(int ItemCode,string Flag, string SPName);
        Task<ResponseResult> GetGateData(string Flag, string SPName,string mrnNo, string MRNYearCode, string MRNCustJW);
        Task<ResponseResult> GetEmployeeList(string Flag, string SPName);
        Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName);
        Task<ResponseResult> AllowUpdelete(int EntryId, string YearCode);
        Task<MIRQDashboard> GetSearchData(string VendorName, string MrnNo,string GateNo,string MirNo, string ItemName, string FromDate, string ToDate);
        Task<MIRQDashboard> GetDashboardDetailData(string VendorName, string MrnNo, string GateNo, string MirNo,string ItemName, string FromDate, string ToDate);
        Task<ResponseResult> SaveMIR(MirModel model, DataTable MIRGrid);
        Task<ResponseResult> CheckEditOrDelete(int EntryId, int YearCode);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> GetSearchData(MIRQDashboard model);
        Task<MirModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetOkRecStore(int ItemCode,string ShowAllStore);
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<ResponseResult> GetReportName();
    }
}
