using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITransferFromWorkCenter
    {
        Task<ResponseResult> GetReportName();
        Task<ResponseResult> GetFormRights(int userID);
        Task<TransferFromWorkCenterModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> FillEntryandGate(string Flag, int YearCode, string SPName);
        Task<ResponseResult> BindEmpList();
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> FillItemName(int TransferMatYearCode);
        Task<ResponseResult> FillPartCode(int TransferMatYearCode);
        Task<ResponseResult> GetBatchNumber(string SPName, int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo);
        Task<ResponseResult> GetWorkCenterTotalStock(string Flag, int ItemCode, int WcId, string TillDate);
        Task<ResponseResult> GetWorkCenterQty(string SPName, int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo);
        Task<ResponseResult> GetUnit(int ItemCode);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> FillProcessName();
        Task<ResponseResult> CheckEditOrDelete(int TransferEntryId, int TransferYearCode);
        Task<TransferFromWorkCenterDashboard> GetDashboardDetailData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType);
        Task<TransferFromWorkCenterDashboard> GetDashboardData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType);
        Task<ResponseResult> SaveTransferFromWorkCenter(TransferFromWorkCenterModel model, DataTable TransferGrid);
        Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate,int EmpID);
        Task<ResponseResult> ChkWIPStockBeforeSaving(int WcId, string TransferMatEntryDate, int TransferMatYearCode,int TransferMatEntryId, DataTable TransferGrid,string Mode);
    }
}
