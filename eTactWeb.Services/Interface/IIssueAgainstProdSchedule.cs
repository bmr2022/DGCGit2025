using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIssueAgainstProdSchedule
    {
        Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName);
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> FillIssueByEmp();
        Task<ResponseResult> FillRecByEmp();
        Task<ResponseResult> GetIsStockable(int ItemCode);
        Task<ResponseResult> DisplayRoutingDetail(int ItemCode);
        Task<ResponseResult> DeleteByID(int ID, int YC, string ProdSchSlipNo);
        Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty,int BomNo);
        Task<IssueAgainstProdSchedule> GetViewByID(int ID, int YearCode, string ProdSlipNo);
        Task<ResponseResult> GetSummaryDetail(string FromDate, string ToDate, string IssueFromStore, string ItemName, string PartCode ,  string ProdPlanNo , string ProdSchNo , string Searchbox , string DashboardType );
        Task<IssueAgainstProdScheduleDashboard> GetSearchData(string FromDate, string IssAgtProdSchSlipNo, string ToDate, string IssueFromStore, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string DashboardType);
        Task<ResponseResult> SaveIssueAgainstProductionSchedule(IssueAgainstProdSchedule model, DataTable IssueAgainstProductionScheduleGrid);
        Task<ResponseResult> FillBatchNo(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate);
        Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);
    }
}
