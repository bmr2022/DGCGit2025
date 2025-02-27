using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;

namespace eTactWeb.Services.Interface
{
    public interface IJobWorkReceive
    {
        Task<ResponseResult> FillNewEntry(int YearCode);
        Task<DataSet> BindBranch(string Flag);
        Task<ResponseResult> GetGateNo(string Flag, string SPName, string FromDate, string ToDate);
        Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateyearCode, int GateEntryId);
        Task<ResponseResult> GetGateItemData(string Flag, string SPName, string GateNo, string GateyearCode, int GateEntryId);
        Task<ResponseResult> GetEmployeeList(string Flag, string SPName);
        Task<ResponseResult> GetProcessList(string Flag, string SPName);
        Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode);
        Task<ResponseResult> GetFeatureOption(string Flag, string SPName);
        Task<ResponseResult> GetProcessUnit(string Flag, string SPName);
        Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo);
        Task<ResponseResult> GetPopUpChallanData(int AccountCode, int YearCode, string FromDate, string ToDate, int RecItemCode, int BomRevNo, string BomRevDate, string BOMIND, string BillChallanDate, string JobType, string ProdUnProd);
        Task<ResponseResult> GetPopUpData(string Flag,int AccountCode, int IssYear, string FinYearFromDate, string billchallandate, string prodUnProd, string BOMINd, int RMItemCode, string RMPartcode, string RMItemNAme, string ACCOUNTNAME, int Processid);
        Task<ResponseResult> GetAdjustedChallan(int AccountCode, int YearCode, string FinYearFromDate, string billchallandate, string GateNo, int GateYearCode, DataTable DTTItemGrid);
        Task<ResponseResult> GetBomValidated(int RecItemCode, int BomRevNo, string BomRevDate, int RecQty);
        Task<JWReceiveDashboard> GetDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName,string BranchName, string InvNo, string Fromdate,string Todate);
        Task<JWReceiveDashboard> GetDetailDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName,string BranchName, string InvNo, string Fromdate,string Todate);
        Task<ResponseResult> GetBomRevNo(int Itemcode);
        Task<ResponseResult> ViewDetailSection(int yearCode, int entryId);
        Task<ResponseResult> SaveJobReceive(JobWorkReceiveModel model, DataTable JWRGrid, DataTable ChallanGrid);
        Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag);
        Task<JobWorkReceiveModel> GetViewByID(int ID, int YearCode);

        Task<ResponseResult> DeleteByID(int ID, int YearCode);
        Task<ResponseResult> GetFormRights(int uId);
    }
}
