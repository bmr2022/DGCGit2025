using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class DirectPurchaseBillBLL : IDirectPurchaseBill
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly DirectPurchaseBillDAL _DirectPurchaseBillDAL;

        public DirectPurchaseBillBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DirectPurchaseBillDAL = new DirectPurchaseBillDAL(configuration, iDataLogic,  connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        //public async Task<ResponseResult> GetItemServiceFORPO(string ItemSErv)
        //{
        //    return await _DirectPurchaseBillDAL.GetItemServiceFORPO(ItemSErv);
        //}
        public async Task<string> GetItemServiceFORPO(string ItemService)
        {
            return await _DirectPurchaseBillDAL.GetItemServiceFORPO(ItemService);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string PurchVoucherNo, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate)
        {
            return await _DirectPurchaseBillDAL.DeleteByID(ID, YearCode, Flag, PurchVoucherNo, InvNo, EntryBy, EntryByMachineName, EntryDate);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _DirectPurchaseBillDAL.GetFormRights(ID);
        } 
        public async Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int EntryId)
        {
            return await _DirectPurchaseBillDAL.CheckDuplicateEntry(YearCode,AccountCode, InvNo, EntryId);
        }
        public async Task<ResponseResult> FillEntryandVouchNoNumber(int YearCode, string VODate)
        {
            return await _DirectPurchaseBillDAL.FillEntryandVouchNoNumber(YearCode, VODate);
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            return await _DirectPurchaseBillDAL.GetItemCode(PartCode);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _DirectPurchaseBillDAL.GetReportName();
        }
        public async Task<ResponseResult> GetExchangeRate(string Currency)
        {
            return await _DirectPurchaseBillDAL.GetExchangeRate(Currency);
        }

        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
        {
            return await _DirectPurchaseBillDAL.FillItems(Type, ShowAllItem);
        }
        public async Task<ResponseResult> FILLDocumentList(string ShowAll)
        {
            return await _DirectPurchaseBillDAL.FILLDocumentList(ShowAll);
        }
        public async Task<ResponseResult> FillPONumber(int YearCode, string OrderType, string PODate)
        {
            return await _DirectPurchaseBillDAL.FillPONumber(YearCode, OrderType, PODate);
        }
        public async Task<ResponseResult> GetPOData(string BillDate, int? AccountCode, int? ItemCode)
        {
            return await _DirectPurchaseBillDAL.GetPOData(BillDate, AccountCode, ItemCode);
        }
        public async Task<ResponseResult> GetScheduleData(string PONo, int? POYear, string BillDate, int? AccountCode, int? ItemCode)
        {
            return await _DirectPurchaseBillDAL.GetScheduleData(PONo, BillDate, POYear, AccountCode, ItemCode);
        }
        public async Task<ResponseResult> FillVouchNumber(int YearCode, string VODate)
        {
            return await _DirectPurchaseBillDAL.FillVouchNumber(YearCode, VODate);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            return await _DirectPurchaseBillDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> CheckLockYear(int YearCode)
        {
            return await _DirectPurchaseBillDAL.CheckLockYear(YearCode);
        } 
        public async Task<ResponseResult> CheckEditOrDelete(int ID, int YearCode)
        {
            return await _DirectPurchaseBillDAL.CheckEditOrDelete(ID, YearCode);
        }
        public async Task<ResponseResult> FillCurrency(string Ctrl)
        {
            return await _DirectPurchaseBillDAL.FillCurrency(Ctrl);
        }
        public async Task<ResponseResult> GetGstRegister(string Flag, int Code)
        {
            return await _DirectPurchaseBillDAL.GetGstRegister(Flag, Code);
        }
        public async Task<ResponseResult> GetStateGST(string Flag, int Code)
        {
            return await _DirectPurchaseBillDAL.GetStateGST(Flag, Code);
        }
        Task<ResponseResult> IDirectPurchaseBill.NewAmmEntryId()
        {
            throw new NotImplementedException();
        }
        public async Task<ResponseResult> GetFeatureOption()
        {
            return await _DirectPurchaseBillDAL.GetFeatureOption();
        }
        Task<DirectPurchaseBillModel> IDirectPurchaseBill.GetViewPOCcompletedByID(int iD, int yC, string PONO, string v)
        {
            throw new NotImplementedException();
        }

        async Task<DirectPurchaseBillModel> IDirectPurchaseBill.GetViewByID(int ID, int YearCode, string Flag)
        {
            return await _DirectPurchaseBillDAL.GetViewByID(ID, YearCode, Flag);
        }


        public async Task<ResponseResult> SaveDirectPurchaseBILL(DataTable itemDetailDt, DataTable taxDetailDt, DataTable TDSDetailDT, DirectPurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT)
        {
            return await _DirectPurchaseBillDAL.SaveDirectPurchaseBILL(itemDetailDt, taxDetailDt, TDSDetailDT, model, DrCrDetailDT, AdjDetailDT);
        }

        async Task<DPBDashBoard> IDirectPurchaseBill.GetDashBoardData()
        {
            return await _DirectPurchaseBillDAL.GetDashBoardData();
        }

        async Task<DPBDashBoard> IDirectPurchaseBill.GetSummaryData(DPBDashBoard model)
        {
            return await _DirectPurchaseBillDAL.GetSummaryData(model);
        }

        async Task<DPBDashBoard> IDirectPurchaseBill.GetDetailData(DPBDashBoard model)
        {
            return await _DirectPurchaseBillDAL.GetDetailData(model);
        }

        Task<DPBDashBoard> IDirectPurchaseBill.GetSearchCompData(DPBDashBoard model)
        {
            throw new NotImplementedException();
        }

    }
}