using eTactWeb.Data.Common;
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
    public class IssueNrgpBLL : IIssueNRGP
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly IssueNrgpDAL _IssueNRGPDAL;
        public IssueNrgpBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _IssueNRGPDAL = new IssueNrgpDAL(configuration, iDataLogic,connectionStringService);
        }
        public async Task<ResponseResult> GetItemGroup()
        {
            return await _IssueNRGPDAL.GetItemGroup();
        }

        
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _IssueNRGPDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _IssueNRGPDAL.GetReportName();
        }
        public async Task<IssueNRGPModel> selectMultipleItem(string GroupName, int StoreID, string FromDate, string ToDate, string PartCode)
        {
            return await _IssueNRGPDAL.selectMultipleItem( GroupName,  StoreID,  FromDate,  ToDate,  PartCode);
        }
        public async Task<ResponseResult> FillEntryandChallanNo(int YearCode, string RGPNRGP)
        {
            return await _IssueNRGPDAL.FillEntryandChallanNo(YearCode, RGPNRGP);
        }

        public async Task<ResponseResult> IssueChaallanTaxIsMandatory()
        {
            return await _IssueNRGPDAL.IssueChaallanTaxIsMandatory();
        }
        public async Task<ResponseResult> GetBatchInventory()
        {
            return await _IssueNRGPDAL.GetBatchInventory();
        }  
        public async Task<ResponseResult> GetVendorList()
        {
            return await _IssueNRGPDAL.GetVendorList();
        }  
        public async Task<ResponseResult> GetProcessList()
        {
            return await _IssueNRGPDAL.GetProcessList();
        }  
        public async Task<ResponseResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo, int accountcode)
        {
            return await _IssueNRGPDAL.GetItemRate(ItemCode, TillDate, YearCode, BatchNo, UniqueBatchNo,  accountcode);
        }  
        public async Task<ResponseResult> GetStoreList()
        {
            return await _IssueNRGPDAL.GetStoreList();
        }
        public async Task<ResponseResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            return await _IssueNRGPDAL.GetPrevQty(EntryId, YearCode, ItemCode, uniqueBatchNo);
        }
        public async Task<ResponseResult> GetAllowBackDate()
        {
            return await _IssueNRGPDAL.GetAllowBackDate();
        }
        public async Task<ResponseResult> GetAddressDetails(int AccountCode)
        {
            return await _IssueNRGPDAL.GetAddressDetails(AccountCode);
        }
        public async Task<ResponseResult> GetEmails(int AccountCode)
        {
            return await _IssueNRGPDAL.GetEmails(AccountCode);
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno)
        {
            return await _IssueNRGPDAL.FillCurrentBatchINStore(ItemCode, YearCode, FinStartDate, StoreName, batchno);
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, string FinStartDate, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            return await _IssueNRGPDAL.GetBatchNumber(SPName, StoreId, StoreName, FinStartDate, ItemCode, TransDate, YearCode, BatchNo);
        }
        public async Task<ResponseResult> GetStoreTotalStock(string Flag, int ItemCode, int StoreId, string TillDate)
        {
            return await _IssueNRGPDAL.GetStoreTotalStock(Flag, ItemCode, StoreId, TillDate);
        }
        public async Task<ResponseResult> GetBatchStockQty(string Flag, int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            return await _IssueNRGPDAL.GetBatchStockQty(Flag, ItemCode, StoreId, TillDate, BatchNo, UniqueBatchNo);
        }
        public async Task<ResponseResult> GetPONOByAccount(string Flag, int AccountCode, string PONO, int ItemCode, int POYear)
        {
            return await _IssueNRGPDAL.GetPONOByAccount(Flag, AccountCode, PONO, ItemCode, POYear);
        }
        public async Task<ResponseResult> FillAgainstChallanNo(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate)
        {
            return await _IssueNRGPDAL.FillAgainstChallanNo(Flag, AccountCode,ItemCode,YearCode,ChallanDate);
        }
        public async Task<ResponseResult> FillAgainstChallanYC(string Flag, int AccountCode, int ItemCode, int YearCode, string ChallanDate,string AgainstChallanNo)
        {
            return await _IssueNRGPDAL.FillAgainstChallanYC(Flag, AccountCode,ItemCode,YearCode,ChallanDate, AgainstChallanNo);
        }
        public async Task<ResponseResult> FillAgainstChallanEntryId(string Flag, int AccountCode, int ItemCode, string ChallanDate,string AgainstChallanNo,string AgainstChallanYC)
        {
            return await _IssueNRGPDAL.FillAgainstChallanEntryId(Flag, AccountCode,ItemCode,ChallanDate, AgainstChallanNo, AgainstChallanYC);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _IssueNRGPDAL.BindAllDropDowns(Flag);
        }
		//public async Task<ResponseResult> GetAllItems(string Flag)
		//{
		//    return await _IssueNRGPDAL.GetAllItems(Flag);
		//}

		//public async Task<ResponseResult> FillItemName(string Flag)
		//{
		//    return await _IssueNRGPDAL.FillItemName(Flag);
		//}

		
        public async Task<ResponseResult> AutoFillitem(string Flag, string showallitem, string SearchItemCode, string SearchPartCode)
		{
			return await _IssueNRGPDAL.AutoFillitem(Flag, showallitem, SearchItemCode, SearchPartCode);
		}

		public async Task<ResponseResult> CheckItems(string Flag)
        {
            return await _IssueNRGPDAL.CheckItems(Flag);
        }

        public async Task<ResponseResult> FillChallanType(string RGPNRGP)
        {
            return await _IssueNRGPDAL.FillChallanType(RGPNRGP);
        }
        public async Task<ResponseResult> StockableItems(string Flag, int ItemCode)
        {
            return await _IssueNRGPDAL.StockableItems(Flag, ItemCode);
        }
        public async Task<ResponseResult> GetDashboardData(INDashboard model)
        {
            return await _IssueNRGPDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC,string machineName, int actuaEntryBy)
        {
            return await _IssueNRGPDAL.DeleteByID(ID, YC,machineName,actuaEntryBy);
        }
        public async Task<IssueNRGPModel> GetViewByID(int ID, int YC, string Mode)
        {
            return await _IssueNRGPDAL.GetViewByID(ID, YC, Mode);
        }
        public async Task<ResponseResult> CheckGateEntry(int ID, int YC)
        {
            return await _IssueNRGPDAL.CheckGateEntry(ID, YC);
        }
        public async Task<ResponseResult> SaveIssueNRGP(IssueNRGPModel model, DataTable INGrid, DataTable TaxDetailDT)
        {
            return await _IssueNRGPDAL.SaveIssueNRGP(model, INGrid, TaxDetailDT);
        }
    }
}
