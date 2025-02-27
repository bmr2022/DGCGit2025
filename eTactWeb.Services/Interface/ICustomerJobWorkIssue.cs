using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICustomerJobWorkIssue
    {
        Task<ResponseResult> GetNewEntry(int yearCode);
        Task<ResponseResult> GetCustomers(int yearCode);
        Task<ResponseResult> GetCustomerDetails(int AccountCode);
        Task<ResponseResult> GetDistanceData(int AccountCode);
        Task<ResponseResult> FillSoNoDetails(string ChallanDate,int AccountCode);
        Task<ResponseResult> FillSOSchedule(string ChallanDate,int AccountCode, int SoNo, int SoNoYearCode);
        Task<ResponseResult> FillSoNoYearCode(int AccountCode, int SoNo, string ChallanDate);
        Task<ResponseResult> FillCustOrderNo(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode);
        Task<ResponseResult> FillScheduleNoAndYear(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode);
        Task<ResponseResult> FillPartCode(int yearCode);
        Task<ResponseResult> FillItemList(int yearCode);
        Task<ResponseResult> GetStoreList();
        Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string ReportType);
        Task<CustJWIssQDashboard> GetDashboardDetailsData(string FromDate, string ToDate, string ReportType);
   
        //Task<ResponseResult> GetDashboardSummaryData();
        Task<ResponseResult> SaveCustomerJWI(CustomerJobWorkIssueModel model, DataTable JWIGrid);
        Task<CustomerJobWorkIssueModel> GetViewByID(int ID,int YearCode);
        Task<ResponseResult> DeleteByID(int CustJwIssEntryId, int CustJwIssYearCode, string EntryMachineName, int EntryById, string ActualEntryDate, int Account_Code);
        //Task<ResponseResult> GetAdjustedChallan(int YearCode, string EntryDate, string ChallanDate,DataTable DTTItemGrid);
        Task<ResponseResult> GetPopUpData(int YearCode, string EntryDate, string ChallanDate,int AccountCode, string prodUnProd,string BOMINd, int RMItemCode,string Partcode);
        List<CustomerJobWorkIssueModel> GetAdjustedChallanDetailsData(List<CustomerJobWorkIssueModel> adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode);

        //Task<ResponseResult> GetAdjustedChallanDetailsData(int YearCode, string EntryDate, string ChallanDate, DataTable DTTItemGrid);
    }
}
