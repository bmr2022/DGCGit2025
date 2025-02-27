using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIssueThrBOM
    {
        Task<ResponseResult> GetNewEntry(int YearCode);
        Task<ResponseResult> FillProjectNo();
        Task<ResponseResult> GetIsStockable(int ItemCode);
        Task<ResponseResult> GetAllowBatch();
        Task<ResponseResult> FillFGDataList(string Reqno, int ReqYC);
        Task<DataSet> FillEmployee(string Flag);
        Task<ResponseResult> GetReqQtyForScan(string ReqNo, int ReqYearCode, string ReqDate, int ItemCode);
        Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo);
        Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate);
        Task<ResponseResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate);
        Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);

        Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);

        Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, int ReqyearCode, int ItemCode);
        Task<ResponseResult> SaveIssueThrBom(IssueThrBom model, DataTable RMGrid,DataTable FGGrid);
        Task<ResponseResult> GetDashboardData(string Fromdate, string ToDate, string Flag);
        Task<IList<TextValue>> GetEmployeeList();
        Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string DashboardType, string IssueSlipNo, string ReqNo);
        Task<IssueThrBom> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> DeleteByID(int ID, int YearCode);
        Task<IssueThrBomMainDashboard> FGDetailData(string FromDate, string Todate, string Flag = "", string DashboardType = "FGSUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "");
        Task<IssueThrBomMainDashboard> RMDetailData(string FromDate, string Todate, string WCName,string PartCode,string ItemName, string Flag = "", string DashboardType = "RMDETAIL", string IssueSlipNo = "", string ReqNo = "", string GlobalSearch = "", string FGPartCode = "", string FGItemName = "");
        Task<IssueThrBomMainDashboard> SummaryData(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "");

    }
}
