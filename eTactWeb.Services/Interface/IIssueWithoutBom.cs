using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIssueWithoutBom
    {
        Task<ResponseResult> FillBranch();
        Task<ResponseResult> GetIsStockable(int ItemCode);
        Task<ResponseResult> SaveIssueWithoutBom(IssueWithoutBom model, DataTable MRGrid);

        Task<ResponseResult> DeleteByID(int ID, int YearCode);
        Task<ResponseResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo,string IssuedDate,string FinStartDate);
        Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);
        Task<ResponseResult> GetReqQtyForScan(string ReqNo, int ReqYearCode, string ReqDate,int ItemCode);

        Task<ResponseResult> GetDashboardData(string Fromdate, string ToDate, string Flag);
        Task<ResponseResult> GetDataForDelete(int ID,int YC);
        Task<ResponseResult> CheckLastTransDate(long itemCode,string batchno, string uniqbatchno);
        Task<IssueWOBomMainDashboard> GetSearchData(string REQNo, string ReqDate, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate);
        Task<IssueWOBomMainDashboard> GetDetailData(string REQNo, string ReqDate, string PartCode, string Item_Name, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate);
        Task<IssueWithoutBom> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> GetNewEntry(int YearCode);
        Task<ResponseResult> GetAllowBatch();
        Task<ResponseResult> FillProjectNo();
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillDept();
        Task<DataSet> FillEmployee(string Flag);
        Task<ResponseResult> GetIssueScanFeature();
        Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo);
        Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, string ReqDate, int ItemCode);
        Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate);
        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);

    }
}
