using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIssueNRGP
    {
        public Task<ResponseResult> FillEntryandChallanNo(int YearCode, string RGPNRGP);
        public Task<ResponseResult> IssueChaallanTaxIsMandatory();
        Task<ResponseResult> GetReportName();
        Task<ResponseResult> GetFormRights(int userID);
        public Task<ResponseResult> GetBatchInventory();
        public Task<ResponseResult> GetVendorList();
        public Task<ResponseResult> GetProcessList();
        public   Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo,int accountcode);
        public Task<ResponseResult> GetStoreList();
        public Task<ResponseResult> GetAllowBackDate();
        public Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo);
        public Task<ResponseResult> GetAddressDetails(int AccountCode);

        public Task<ResponseResult> GetEmails(int AccountCode);
        public Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno);
        public Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, string FinStartDate, int ItemCode, string TransDate, int YearCode, string BatchNo);
        public Task<ResponseResult> GetStoreTotalStock(string Flag, int ItemCode, int StoreId, string TillDate);
        public Task<ResponseResult> GetBatchStockQty(string Flag, int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo);
        public Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int ItemCode, int POYear);
        public Task<ResponseResult> FillAgainstChallanNo(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate);
        public Task<ResponseResult> FillAgainstChallanYC(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate, string AgainstChallanNo);
        public Task<ResponseResult> FillAgainstChallanEntryId(string Flag, int AccountCode, int ItemCode, string ChallanDate, string AgainstChallanNo,string AgainstChallanYC);
        //public Task<ResponseResult> GetAllItems(string Flag);
        public Task<ResponseResult> AutoFillitem(string Flag,string showallitem, string SearchItemCode, string SearchPartCode);
        public Task<ResponseResult> CheckItems(string Flag);
        public Task<ResponseResult> FillChallanType(string RGPNRGP);
        public Task<ResponseResult> StockableItems(string Flag, int ItemCode);
        public Task<ResponseResult> GetDashboardData(INDashboard model);
        public Task<ResponseResult> DeleteByID(int ID, int YC, string machineName, int actuaEntryBy);
        public Task<IssueNRGPModel> GetViewByID(int ID, int YC, string Mode);
        public Task<ResponseResult> CheckGateEntry(int ID, int YC);
        public Task<ResponseResult> SaveIssueNRGP(IssueNRGPModel model, DataTable INGrid, DataTable TaxDetailDT);
        public Task<DataSet> BindAllDropDowns(string Flag);

        Task<ResponseResult> GetItemGroup();
        Task<IssueNRGPModel> selectMultipleItem(string GroupName, int StoreID,string FromDate,string ToDate, string PartCode);
        Task<ResponseResult> GetFeatureOption();

    }
}