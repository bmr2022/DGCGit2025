using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PurchaseOrderBLL : IPurchaseOrder
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly PurchaseOrderDAL _PurchaseOrderDAL;

        public PurchaseOrderBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _PurchaseOrderDAL = new PurchaseOrderDAL(configuration, iDataLogic,connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        //public async Task<ResponseResult> GetItemServiceFORPO(string ItemSErv)
        //{
        //    return await _PurchaseOrderDAL.GetItemServiceFORPO(ItemSErv);
        //}
        public async Task<string> GetItemServiceFORPO(string ItemService)
        {
            return await _PurchaseOrderDAL.GetItemServiceFORPO(ItemService);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode,int createdBy,string entryByMachineName, string Flag)
        {
            return await _PurchaseOrderDAL.DeleteByID(ID, YearCode,createdBy,entryByMachineName, Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _PurchaseOrderDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> GetFormRightsAmm(int ID)
        {
            return await _PurchaseOrderDAL.GetFormRightsAmm(ID);
        }
        public async Task<ResponseResult> GetPendQty(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag)
        {
            return await _PurchaseOrderDAL.GetPendQty(PONo, POYearCode, ItemCode, AccountCode, SchNo, SchYearCode, Flag);
        }
        public async Task<ResponseResult> FillIndentDetail(string itemName, string partCode, int itemCode)
        {
            return await _PurchaseOrderDAL.FillIndentDetail(itemName, partCode,itemCode);
        }
        public async Task<ResponseResult> FillEntryandPONumber(int YearCode)
        {
            return await _PurchaseOrderDAL.FillEntryandPONumber(YearCode);
        }
        public async Task<ResponseResult> NewAmmEntryId(int PoAmendYearCode)
        {
            return await _PurchaseOrderDAL.NewAmmEntryId(PoAmendYearCode);
        }
        public async Task<ResponseResult> FillVendors()
        {
            return await _PurchaseOrderDAL.FillVendors();
        }
        public async Task<ResponseResult> GetAllPartyName(string CTRL)
        {
            return await _PurchaseOrderDAL.GetAllPartyName(CTRL);
        }
        public async Task<PODashBoard> GetUpdAmmData(PODashBoard model)
        {
            return await _PurchaseOrderDAL.GetUpdAmmData(model);
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            return await _PurchaseOrderDAL.GetItemCode(PartCode);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _PurchaseOrderDAL.GetReportName();
        }
        public async Task<ResponseResult> GetExchangeRate(string Currency)
        {
            return await _PurchaseOrderDAL.GetExchangeRate(Currency);
        }
        public async Task<PurchaseOrderModel> GetViewPOCcompletedByID(int ID, int YearCode, string PONO, string Flag)
        {
            return await _PurchaseOrderDAL.GetViewPOCcompletedByID(ID, YearCode, PONO, Flag);
        }
        public async Task<PODashBoard> GetAmmDashboardData()
        {
            return await _PurchaseOrderDAL.GetAmmDashboardData();
        }
        public async Task<PODashBoard> GetAmmCompletedData(string summaryDetail)
        {
            return await _PurchaseOrderDAL.GetAmmCompletedData(summaryDetail);
        }
        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
        {
            return await _PurchaseOrderDAL.FillItems(Type, ShowAllItem);
        }
        public async Task<ResponseResult> FillPONumber(int YearCode, string OrderType, string PODate)
        {
            return await _PurchaseOrderDAL.FillPONumber(YearCode, OrderType, PODate);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            return await _PurchaseOrderDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> CheckLockYear(int YearCode)
        {
            return await _PurchaseOrderDAL.CheckLockYear(YearCode);
        }
        public async Task<ResponseResult> FillCurrency(string Ctrl)
        {
            return await _PurchaseOrderDAL.FillCurrency(Ctrl);
        }
        public async Task<ResponseResult> GetGstRegister(string Flag, int Code)
        {
            return await _PurchaseOrderDAL.GetGstRegister(Flag, Code);
        }

        public async Task<PODashBoard> GetDashBoardData()
        {
            return await _PurchaseOrderDAL.GetDashBoardData();
        }

        public async Task<object?> GetMrpYear(string Flag, string MRPNo)
        {
            return await _PurchaseOrderDAL.GetMrpYear(Flag, MRPNo);
        }

        public async Task<ResponseResult> GetQuotData(string Flag, string QuotNo)
        {
            return await _PurchaseOrderDAL.GetQuotData(Flag, QuotNo);
        }

        public async Task<PODashBoard> GetSearchData(PODashBoard model)
        {
            return await _PurchaseOrderDAL.GetSearchData(model);
        }
        public async Task<PODashBoard> GetDetailData(PODashBoard model)
        {
            return await _PurchaseOrderDAL.GetDetailData(model);
        }
        public async Task<PODashBoard> GetSearchCompData(PODashBoard model)
        {
            return await _PurchaseOrderDAL.GetSearchCompData(model);
        }

        public async Task<PurchaseOrderModel> GetViewByID(int ID, int YC, string Flag)
        {
            return await _PurchaseOrderDAL.GetViewByID(ID, YC, Flag);
        }

        public async Task<ResponseResult> SavePurchaseOrder(DataTable ItemDetailDT, DataTable DelieveryScheduleDT, DataTable TaxDetailDT,DataTable IndentDetailDT, PurchaseOrderModel model)
        {
            return await _PurchaseOrderDAL.SavePurchaseOrder(ItemDetailDT, DelieveryScheduleDT, TaxDetailDT,IndentDetailDT, model);
        }


    }
}