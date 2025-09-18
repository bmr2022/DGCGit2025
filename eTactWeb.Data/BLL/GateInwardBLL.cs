using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class GateInwardBLL : IGateInward
    {
        //private readonly PurchaseOrderDAL _PurchaseOrderDAL;
        private readonly IDataLogic _DataLogicDAL;
        private readonly GateInwardDAL _GateInwardDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GateInwardBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //_PurchaseOrderDAL = new PurchaseOrderDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
            _GateInwardDAL = new GateInwardDAL(configuration, iDataLogic,_httpContextAccessor, connectionStringService);
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _GateInwardDAL.CheckFeatureOption();
        }
        public async Task<ResponseResult> CCEnableDisable()
        {
            return await _GateInwardDAL.CCEnableDisable();
        }
        public async Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType)
        {
            return await _GateInwardDAL.CheckDuplicateEntry(YearCode,AccountCode,InvNo,DocType);
        }
        public async Task<ResponseResult> CheckEditOrDelete(string GateNo,int YearCode)
        {
            return await _GateInwardDAL.CheckEditOrDelete(GateNo, YearCode);
        }

        public async Task<ResponseResult> GetPoNumberDropDownList(string Flag, string ServiceType, string SPName,string AccountCode, int Year, int DocTypeId)
        {
            return await _GateInwardDAL.GetPoNumberDropDownList(Flag, ServiceType, SPName, AccountCode,Year, DocTypeId);
        }
        public async Task<ResponseResult> FillSaleBillChallan(int AccountCode, int doctype,int ItemCode)
        {
            return await _GateInwardDAL.FillSaleBillChallan(AccountCode, doctype, ItemCode);
        }
        public async Task<ResponseResult> FillChallanQty(int AccountCode, int ItemCode,string ChallanNo)
        {
            return await _GateInwardDAL.FillChallanQty(AccountCode, ItemCode,ChallanNo);
        }
        public async Task<ResponseResult> FillSaleBillQty(int AccountCode, int ItemCode, string SaleBillNo,int SaleBillYearCode)
        {
            return await _GateInwardDAL.FillSaleBillQty(AccountCode, ItemCode, SaleBillNo,SaleBillYearCode);
        }
        public async Task<ResponseResult> GetItems(string Flag, int doctype, string Check,int AccountCode)
        {
            return await _GateInwardDAL.GetItems(Flag,doctype, Check,AccountCode);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, int AltQty, int UnitQty)
        {
            return await _GateInwardDAL.AltUnitConversion(ItemCode,AltQty,UnitQty);
        }
        public async Task<ResponseResult> FillPendQty(int ItemCode, int PartyCode, string PONO, int POYear, int Year, string SchNo, int SchYearCode, int ProcessId, int EntryId, int YearCode)
        {
            return await _GateInwardDAL.FillPendQty(ItemCode, PartyCode, PONO, POYear, Year, SchNo, SchYearCode, ProcessId, EntryId, YearCode);
        }
        
        public async Task<GateInwardDashboard> GetDashboardData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate,string DashboardType)
        {
            return await _GateInwardDAL.GetDashboardData(VendorName, Gateno, ItemName, PartCode, DocName, PONO, ScheduleNo, FromDate, ToDate,DashboardType);
        } 
        public async Task<PendingGateInwardDashboard> GetPendingGateEntryDashboardData(int AccountCode, string PoNo, int PoYearCode, int ItemCode,
   string FromDate, string ToDate,string PartCode,string ItemName)
        {
            return await _GateInwardDAL.GetPendingGateEntryDashboardData(AccountCode, PoNo, PoYearCode, ItemCode, FromDate, ToDate,PartCode,ItemName);
        }
    
        public async Task<GateInwardDashboard> GetDashboardDetailData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate)
        {
            return await _GateInwardDAL.GetDashboardDetailData(VendorName, Gateno, ItemName, PartCode, DocName, PONO, ScheduleNo, FromDate, ToDate);
        }
        public async Task<ResponseResult> GetPopUpData(string Flag, int AccountCode,string PONO)
        {
            return await _GateInwardDAL.GetPopUpData(Flag, AccountCode,PONO);
        }
        public async Task<ResponseResult> GetScheDuleByYearCodeandAccountCode(string Flag,string AccountCode, string YearCode, string poNo, int docTypeId, string InvoiceDate, string ItemService, string EntryDate)
        {
            return await _GateInwardDAL.GetScheDuleByYearCodeandAccountCode(Flag,AccountCode,YearCode,poNo, docTypeId,  InvoiceDate, ItemService,EntryDate);
        }

        public async Task<ResponseResult> FillItems(string Flag, string accountCode, string Year, string poNo,string Type, string scheduleNO = "", string scheduleYear = "",string Check="")
        {
            return await _GateInwardDAL.FillItems(Flag,accountCode,Year,poNo,Type, scheduleNO,scheduleYear,Check);
        }
        public async Task<ResponseResult> SaveGateInward(GateInwardModel model, DataTable GIGrid)
        {
            return await _GateInwardDAL.SaveGateInward(model, GIGrid);
        }
        public async Task<GateInwardModel> GetEwayBillDataforPo(GateInwardModel model, DataTable GIGrid)
        {
            return await _GateInwardDAL.GetEwayBillDataforPo(model, GIGrid);
        }

        public async Task<ResponseResult> DeleteByID(int ID, int YC,int ActualEnteredBy, string EntryByMachineName, string gateno)
        {
            return await _GateInwardDAL.DeleteByID(ID, YC, ActualEnteredBy,  EntryByMachineName,  gateno);
        }
        public async Task<ResponseResult> FillEntryandGate(string Flag, int YearCode, string SPName)
        {
            return await _GateInwardDAL.FillEntryandGate(Flag, YearCode,SPName);
        }
        //public async Task<ResponseResult> GetDashboardData()
        //public async Task<ResponseResult> GetDashboardData()
        //{
        //    return await _PurchaseScheduleDAL.GetDashboardData();
        //}
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _GateInwardDAL.GetDashboardData();
        } 
       

        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _GateInwardDAL.GetFormRights(ID);
        }

        public async Task<GateInwardModel> GetViewByID(int ID, int YearCode)
        {
            return await _GateInwardDAL.GetViewByID(ID, YearCode);

        } 
        public async Task<ResponseResult> GetAccountCode(string AccountName)
        {
            return await _GateInwardDAL.GetAccountCode(AccountName);

        }
        public async Task<ResponseResult> GetItemCode(string ItemName)
        {
            return await _GateInwardDAL.GetItemCode(ItemName);

        }
        public async Task<ResponseResult> FillSaleBillRate(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode)
        {
            return await _GateInwardDAL.FillSaleBillRate(AccountCode, ItemCode,SaleBillNo,SaleBillYearCode);

        }
        public async Task<ResponseResult> GetSearchData(GateDashboard model)
        {
            return await _GateInwardDAL.GetSearchData(model);
        }
    }
}