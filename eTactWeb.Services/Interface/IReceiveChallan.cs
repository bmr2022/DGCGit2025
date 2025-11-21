using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IReceiveChallan
    {
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> GetMRNNo(int yearcode,string FromDate,string ToDate);
        Task<ResponseResult> GetGateNo(int yearcode, string FromDate, string ToDate);
        Task<ResponseResult> GetMRNDetail(int EntryId);
        Task<ResponseResult> ListOfPendingChallaFromOtherBranch(int AccountCode);
        Task<ResponseResult> PendingChallaItemDetailFromOtherBranch(int AccountCode, int EntryId, int YearCode,string SourceDB);

        Task<ResponseResult> GetGateDetail(string GateNo, int GateYc,int GateEntryId, string FromDate, string ToDate, string Flag);
        Task<ResponseResult> GetMRNYear(string MRNNO);
        Task<ResponseResult> GetMRNDate(string MRNNO,int MRNYC);
        Task<ResponseResult> GetGateYear(string Gateno, string FromDate, string ToDate,int yearcode);
        Task<ResponseResult> GetGateDate(string Gateno,int GateYC);
        Task<ResponseResult> FillAlltDetail(string Gateno,int GateYC);
        Task<ResponseResult> FillStore();
        Task<ResponseResult> SaveReceiveChallan(ReceiveChallanModel model, DataTable RCGrid);
        Task<ReceiveChallanModel> GetViewByID(int ID, int YearCode, string Mode);
        Task<ResponseResult> GetDashboardData(RCDashboard model);
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<ResponseResult> GetFeatureOption();
    }
}
