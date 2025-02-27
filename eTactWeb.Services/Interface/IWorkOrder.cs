using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IWorkOrder
    {
        Task<ResponseResult> GetSaleOrderData(string Flag, string SPName, int yearCode, string WODate,string EffFrom,string EffTill);
        Task<ResponseResult> GetStoreList(string Flag, string SPName);
        Task<ResponseResult> GetTotalStockList(int store, int Itemcode);
        Task<ResponseResult> GetBomNo(int ItemCode);
        Task<ResponseResult> DisplayBomDetail(int ItemCode,float WOQty,int BomRevNo);
        Task<ResponseResult> GetBomName(int BomNo,int FinishedItemCode);
        Task<ResponseResult> GetEffDate(int BomNo,int FinishedItemCode);
        Task<ResponseResult> GetMachineGroupName();
        Task<ResponseResult> GetMachineName(int MachGroupId);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<WorkOrderModel> GetViewByID(int ID, int YC, string Mode);
        Task<ResponseResult> SaveWorkOrder(WorkOrderModel model, DataTable WOGrid);
        Task<ResponseResult> GetDashboardData(string FromDate, string ToDate);
        Task<WorkOrderGridDashboard> GetDashboardData(string SummaryDetail, string WONO, string CC, string SONO, string SchNo, string AccountName, string PartCode, string ItemName, string FromDate, string ToDate);

    }

}
