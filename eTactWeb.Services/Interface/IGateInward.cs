using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using static eTactWeb.DOM.Models.Common;
namespace eTactWeb.Services.Interface;

public interface IGateInward
{

    Task<ResponseResult> GetPoNumberDropDownList(string Flag, string Service, string SPName, string AccountCode,int Year,int DocTypeId);

    Task<ResponseResult> FillSaleBillChallan(int AccountCode, int doctype,int ItemCode);
    Task<ResponseResult> FillChallanQty(int AccountCode, int ItemCode,string ChallanNo);
    Task<ResponseResult> FillSaleBillQty(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode);
    Task<ResponseResult> GetItems(string Flag, int doctype, string Check, int AccountCode);
    Task<ResponseResult> GetPopUpData(string Flag, int AccountCode, string PONO);
    Task<ResponseResult> AltUnitConversion(int ItemCode, int AltQty, int UnitQty);
    Task<ResponseResult> GetScheDuleByYearCodeandAccountCode(string Flag, string AccountCode, string YearCode, string poNo, int docTypeId, string InvoiceDate,string ItemService,string EntryDate);

    Task<ResponseResult> FillItems(string Flag, string accountCode, string Year, string poNo, string Type, string GateNo = "", string GateYear = "", string Check = "");

    Task<ResponseResult> SaveGateInward(GateInwardModel model, DataTable GIGrid);

    Task<ResponseResult> DeleteByID(int ID, int YC,int ActualEnteredBy,string EntryByMachineName,string gateno);
    Task<ResponseResult> FillEntryandGate(string Flag, int YearCode, string SPName);
    Task<ResponseResult> CheckEditOrDelete(string GateNo, int YearCode);
    Task<ResponseResult> FillPendQty(int ItemCode, int PartyCode, string PONO, int POYear, int Year, string SchNo, int SchYearCode, int ProcessId, int EntryId, int YearCode);

    Task<ResponseResult> GetDashboardData();

    Task<GateInwardDashboard> GetDashboardData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate,string DashboardType);
    Task<PendingGateInwardDashboard> GetPendingGateEntryDashboardData(int AccountCode, string PoNo, int PoYearCode, int ItemCode,
    string FromDate, string ToDatePartCode,string PartCode, string ItemName);
    Task<GateInwardDashboard> GetDashboardDetailData(string VendorName, string Gateno, string ItemName, string PartCode,string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate);

    Task<ResponseResult> GetSearchData(GateDashboard model);
    Task<ResponseResult> GetAccountCode(string AccountName );
    Task<ResponseResult> GetItemCode(string ItemName );

    Task<GateInwardModel> GetViewByID(int ID, int YearCode);

    Task<ResponseResult> CheckFeatureOption();
    Task<ResponseResult> CCEnableDisable();
    Task<ResponseResult> GetFormRights(int uId);
    Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType);
    Task<ResponseResult> FillSaleBillRate(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode);

}
