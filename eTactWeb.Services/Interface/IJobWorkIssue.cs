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
    public interface IJobWorkIssue
    {
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName,int ItemCode,string TransDate, int YearCode, string BatchNo, string FinFromDate);
        Task<ResponseResult> GetStockQty(string Flag, int ItemCode, int StockId, string TillDate, string BatchNo, string UniqueBatchNo);
        Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo);
        Task<ResponseResult> FillItemsBom(string Flag, string BomStatus,string Types);
        Task<ResponseResult> GetStoreTotalStock(string Flag,int ItemCode, int StoreId, string TillDate);
        Task<ResponseResult> GetAddressDetails(int AccountCode);
        Task<ResponseResult> FillVendors();
        Task<ResponseResult> FillPartCode(string bomStatus);
        Task<ResponseResult> FillItem(string bomStatus);
        Task<ResponseResult> FillEmployee();
        Task<ResponseResult> FillProcess();
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillAdditionalFields(string Flag, int AccountCode);
        Task<ResponseResult> CheckFeatureOption();
        Task<ResponseResult> FillEntryandJWNo(int YearCode);
        Task<ResponseResult> GetAllItems(string Flag, string TF,string Types);
        Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int POYear, int ItemCode, string ChallanDate);
        Task<ResponseResult> SaveJobWorkIssue(JobWorkIssueModel model, DataTable JWGrid,DataTable TaxDetailGridd);
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<ResponseResult> GetDashboardData();
        Task<JWIssQDashboard> GetDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate);
        Task<JWIssQDashboard> GetDetailDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate);

        Task<ResponseResult> GetSearchData(JWIssQDashboard model);

        Task<JobWorkIssueModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> GetFormRights(int uId);
        public Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo);



    }
}
