using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class JobWorkIssueBLL : IJobWorkIssue
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly JobWorkIssueDAL _JobWorkIssueDAL;
        public JobWorkIssueBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _JobWorkIssueDAL = new JobWorkIssueDAL(configuration, iDataLogic);
        }

        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _JobWorkIssueDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _JobWorkIssueDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _JobWorkIssueDAL.CheckFeatureOption();
        }
        public async Task<ResponseResult> GetAddressDetails(int AccountCode)
        {
            return await _JobWorkIssueDAL.GetAddressDetails(AccountCode);
        }
        public async Task<ResponseResult> FillVendors()
        {
            return await _JobWorkIssueDAL.FillVendors();
        }
        public async Task<ResponseResult> FillPartCode(string bomStatus)
        {
            return await _JobWorkIssueDAL.FillPartCode(bomStatus);
        }
        public async Task<ResponseResult> FillItem(string bomStatus)
        {
            return await _JobWorkIssueDAL.FillItem(bomStatus);
        }
        public async Task<ResponseResult> FillEmployee()
        {
            return await _JobWorkIssueDAL.FillEmployee();
        }
        public async Task<ResponseResult> FillProcess()
        {
            return await _JobWorkIssueDAL.FillProcess();
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _JobWorkIssueDAL.FillStore();
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo,string FinFromDate)
        {
            return await _JobWorkIssueDAL.GetBatchNumber(SPName, StoreId,StoreName,ItemCode,TransDate, YearCode,BatchNo,FinFromDate);
        }
        public async Task<ResponseResult> GetStockQty(string SPName, int ItemCode, int StockId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            return await _JobWorkIssueDAL.GetStockQty(SPName, ItemCode, StockId,TillDate, BatchNo,UniqueBatchNo);
        }       
        public async Task<ResponseResult> FillItemsBom(string Flag,string BomStatus,string Types)
        {
            return await _JobWorkIssueDAL.FillItemsBom(Flag,BomStatus,Types);
        }
        public async Task<ResponseResult> FillEntryandJWNo(int yearCode)
        {
            return await _JobWorkIssueDAL.FillEntryandJWNo(yearCode);
        }
        public async Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            return await _JobWorkIssueDAL.GetPrevQty(EntryId, YearCode, ItemCode, uniqueBatchNo);
        }
        public async Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo)
        {
            return await _JobWorkIssueDAL.GetItemRate(ItemCode,TillDate,YearCode,BatchNo,UniqueBatchNo);
        }
        public async Task<ResponseResult> FillAdditionalFields(string Flag, int AccountCode)
        {
            return await _JobWorkIssueDAL.FillAdditionalFields(Flag, AccountCode);
        }
        public async Task<ResponseResult> GetAllItems(string Flag, string SH,string Types)
        {
            return await _JobWorkIssueDAL.GetAllItems(Flag, SH,Types);
        }
        public async Task<ResponseResult> GetStoreTotalStock(string Flag, int ItemCode, int StoreId, string TillDate)
        {
            return await _JobWorkIssueDAL.GetStoreTotalStock(Flag, ItemCode,StoreId,TillDate);
        }
        public async Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int POYear, int ItemCode, string ChallanDate)
        {
            return await _JobWorkIssueDAL.GetPONOByAccount(Flag, AccountCode, PONO,POYear,ItemCode,ChallanDate);
        }
        public async Task<ResponseResult> SaveJobWorkIssue(JobWorkIssueModel model, DataTable JWGrid, DataTable TaxDetailGridd)
        {
            return await _JobWorkIssueDAL.SaveJobWorkIssue(model, JWGrid, TaxDetailGridd);
        }

        public Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return  _JobWorkIssueDAL.DeleteByID(ID, YC);

        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _JobWorkIssueDAL.GetDashboardData();

            //throw new NotImplementedException();
        } 

        public async Task<JobWorkIssueModel> GetViewByID(int ID, int YearCode)
        {
            return await _JobWorkIssueDAL.GetViewByID(ID, YearCode);

        }

        public async Task<ResponseResult> GetSearchData(JWIssQDashboard model)
        {
            return await _JobWorkIssueDAL.GetSearchData(model);
        }
        public async Task<JWIssQDashboard> GetDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            return await _JobWorkIssueDAL.GetDashboardData(VendorName, ChallanNo, ItemName, PartCode, FromDate, ToDate);
        }
        public async Task<JWIssQDashboard> GetDetailDashboardData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            return await _JobWorkIssueDAL.GetDetailDashboardData(VendorName, ChallanNo, ItemName, PartCode, FromDate, ToDate);
        }
    }
}
