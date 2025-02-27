using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PurchaseBillBLL : IPurchaseBill
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly PurchaseBillDAL _PurchaseBillDAL;

        public PurchaseBillBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _PurchaseBillDAL = new PurchaseBillDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<string> GetItemServiceFORPO(string ItemService)
        {
            return await _PurchaseBillDAL.GetItemServiceFORPO(ItemService);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string PurchVoucherNo, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate)
        {
            return await _PurchaseBillDAL.DeleteByID(ID, YearCode, Flag, PurchVoucherNo, InvNo, EntryBy, EntryByMachineName, EntryDate);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _PurchaseBillDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> fillEntryandVouchNo(int YearCode, string VODate)
        {
            return await _PurchaseBillDAL.fillEntryandVouchNo(YearCode, VODate);
        }
        public async Task<ResponseResult> GetAllowDocName()
        {
            return await _PurchaseBillDAL.GetAllowDocName();
        }
        public async Task<ResponseResult> GetAllowInvoiceNo()
        {
            return await _PurchaseBillDAL.GetAllowInvoiceNo();
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            return await _PurchaseBillDAL.GetItemCode(PartCode);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _PurchaseBillDAL.GetReportName();
        }
        public async Task<ResponseResult> GetExchangeRate(string Currency)
        {
            return await _PurchaseBillDAL.GetExchangeRate(Currency);
        }

        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
        {
            return await _PurchaseBillDAL.FillItems(Type, ShowAllItem);
        }
        public async Task<ResponseResult> GetItems(string ShowAll)
        {
            return await _PurchaseBillDAL.GetItems(ShowAll);
        }
        public async Task<ResponseResult> FillPONumber(int YearCode, string OrderType, string PODate)
        {
            return await _PurchaseBillDAL.FillPONumber(YearCode, OrderType, PODate);
        }
        public async Task<ResponseResult> GetPOData(string BillDate, int? AccountCode, int? ItemCode)
        {
            return await _PurchaseBillDAL.GetPOData(BillDate, AccountCode, ItemCode);
        }
        public async Task<ResponseResult> GetScheduleData(string PONo, int? POYear, string BillDate, int? AccountCode, int? ItemCode)
        {
            return await _PurchaseBillDAL.GetScheduleData(PONo, BillDate, POYear, AccountCode, ItemCode);
        }
        public async Task<ResponseResult> FillVouchNumber(int YearCode, string VODate)
        {
            return await _PurchaseBillDAL.FillVouchNumber(YearCode, VODate);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            return await _PurchaseBillDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> CheckLockYear(int YearCode)
        {
            return await _PurchaseBillDAL.CheckLockYear(YearCode);
        }
        public async Task<ResponseResult> FillCurrency(int? AccountCode)
        {
            return await _PurchaseBillDAL.FillCurrency(AccountCode);
        }
        public async Task<ResponseResult> FillDocName(string ShowAll)
        {
            return await _PurchaseBillDAL.FillDocName(ShowAll);
        }
        public async Task<ResponseResult> GetGstRegister(string Flag, int Code)
        {
            return await _PurchaseBillDAL.GetGstRegister(Flag, Code);
        }
        public async Task<ResponseResult> GetStateGST(string Flag, int Code)
        {
            return await _PurchaseBillDAL.GetStateGST(Flag, Code);
        }
        Task<ResponseResult> IPurchaseBill.NewAmmEntryId()
        {
            throw new NotImplementedException();
        }

        Task<PurchaseBillModel> IPurchaseBill.GetViewPOCcompletedByID(int iD, int yC, string PONO, string v)
        {
            throw new NotImplementedException();
        }

        async Task<PurchaseBillModel> IPurchaseBill.GetViewByID(int ID, int YearCode, string Flag)
        {
            return await _PurchaseBillDAL.GetViewByID(ID, YearCode, Flag);
        }


        public async Task<ResponseResult> SavePurchaseBILL(DataTable itemDetailDt, DataTable taxDetailDt, DataTable TDSDetailDT, PurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT)
        {
            return await _PurchaseBillDAL.SavePurchaseBILL(itemDetailDt, taxDetailDt, TDSDetailDT, model, DrCrDetailDT, AdjDetailDT);
        }

        async Task<PBDashBoard> IPurchaseBill.GetDashBoardData()
        {
            return await _PurchaseBillDAL.GetDashBoardData();
        }

        async Task<PBDashBoard> IPurchaseBill.GetSummaryData(PBDashBoard model)
        {
            return await _PurchaseBillDAL.GetSummaryData(model);
        }

        async Task<PBDashBoard> IPurchaseBill.GetDetailData(PBDashBoard model)
        {
            return await _PurchaseBillDAL.GetDetailData(model);
        }

        Task<PBDashBoard> IPurchaseBill.GetSearchCompData(PBDashBoard model)
        {
            throw new NotImplementedException();
        }
        async Task<PBListDataModel> IPurchaseBill.GetPurchaseBillListData(string? flag, string? MRNType, string? dashboardtype, DateTime? firstdate, DateTime? todate, PBListDataModel model)
        {
            return await _PurchaseBillDAL.GetPurchaseBillListData(flag, MRNType, dashboardtype, firstdate, todate, model);
        }
        async Task<PurchaseBillModel> IPurchaseBill.GetPurchaseBillItemData(string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode)
        {
            return await _PurchaseBillDAL.GetPurchaseBillItemData(flag, FlagMRNJWCHALLAN, Mrnno, mrnyearcode, accountcode);
        }
    }
}