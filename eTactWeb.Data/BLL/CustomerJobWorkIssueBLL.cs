using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CustomerJobWorkIssueBLL : ICustomerJobWorkIssue
    {
        private CustomerJobWorkIssueDAL _CustomerJobWorkIssueDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CustomerJobWorkIssueBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _CustomerJobWorkIssueDAL = new CustomerJobWorkIssueDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _CustomerJobWorkIssueDAL.GetFormRights(userID);
        }

        public async Task<ResponseResult> GetNewEntry(int yearCode)
        {
            return await _CustomerJobWorkIssueDAL.GetNewEntry(yearCode);
        }
        public async Task<ResponseResult> GetCustomers(int yearCode)
        {
            return await _CustomerJobWorkIssueDAL.GetCustomers(yearCode);
        }
        public async Task<ResponseResult> GetCustomerDetails(int AccountCode)
        {
            return await _CustomerJobWorkIssueDAL.GetCustomerDetails(AccountCode);
        }
        public async Task<ResponseResult> GetDistanceData(int AccountCode)
        {
            return await _CustomerJobWorkIssueDAL.GetDistanceData(AccountCode); 
        }
        public async Task<ResponseResult> FillSoNoDetails(string ChallanDate, int AccountCode)
        {
            return await _CustomerJobWorkIssueDAL.FillSoNoDetails(ChallanDate, AccountCode);
        }
        public async Task<ResponseResult> FillSOSchedule(string ChallanDate, int AccountCode, int SoNo, int SoNoYearCode)
        {
            return await _CustomerJobWorkIssueDAL.FillSOSchedule(ChallanDate, AccountCode, SoNo, SoNoYearCode);
        }
        public async Task<ResponseResult> FillSoNoYearCode(int AccountCode, int SoNo, string ChallanDate)
        {
            return await _CustomerJobWorkIssueDAL.FillSoNoYearCode(AccountCode, SoNo, ChallanDate);
        }
        public async Task<ResponseResult> FillCustOrderNo(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode)
        {
            return await _CustomerJobWorkIssueDAL.FillCustOrderNo(AccountCode, SoNo, ChallanDate, SoNOYearCode);
        }

        public async Task<ResponseResult> FillScheduleNoAndYear(int AccountCode, int SoNo, string ChallanDate, int SoNOYearCode)
        {
            return await _CustomerJobWorkIssueDAL.FillScheduleNoAndYear(AccountCode, SoNo, ChallanDate, SoNOYearCode);
        }

        public async Task<ResponseResult> FillPartCode(int yearCode)
        {
            return await _CustomerJobWorkIssueDAL.FillPartCodes(yearCode);
        }
        public async Task<ResponseResult> FillItemList(int yearCode)
        {
            return await _CustomerJobWorkIssueDAL.FillItemList(yearCode);
        }
        public async Task<ResponseResult> GetStoreList()
        {
            return await _CustomerJobWorkIssueDAL.GetStores();
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string ReportType)
        {
            return await _CustomerJobWorkIssueDAL.GetDashboardData( FromDate,  ToDate,  ReportType);
        }
      
        public async Task<CustJWIssQDashboard> GetDashboardDetailsData(string FromDate, string ToDate, string ReportType)
        {
            return await _CustomerJobWorkIssueDAL.GetDashboardDetailsData(FromDate, ToDate, ReportType);
        }
        
        //public async Task<ResponseResult> GetDashboardSummaryData()
        //{
        //    return await _CustomerJobWorkIssueDAL.GetDashboardSummaryData();
        //}
        public async Task<ResponseResult> SaveCustomerJWI(CustomerJobWorkIssueModel model, DataTable JWIGrid, DataTable ChallanGrid)
        {
            return await _CustomerJobWorkIssueDAL.SaveCustomerJWI(model, JWIGrid, ChallanGrid);
        } 
        public async Task<ResponseResult> DeleteByID(int CustJwIssEntryId, int CustJwIssYearCode, string EntryMachineName, int EntryById, string ActualEntryDate, int Account_Code)
        {
            return await _CustomerJobWorkIssueDAL.DeleteByID(CustJwIssEntryId, CustJwIssYearCode, EntryMachineName, EntryById, ActualEntryDate, Account_Code);
        }
        public async Task<CustomerJobWorkIssueModel> GetViewByID(int ID, int YearCode)
        {
            return await _CustomerJobWorkIssueDAL.GetViewByID(ID, YearCode);
        }

        //public async Task<ResponseResult> GetAdjustedChallan(int YearCode, string EntryDate, string ChallanDate,DataTable DTTItemGrid)
        //{
        //    return await _CustomerJobWorkIssueDAL.GetAdjustedChallan( YearCode,  EntryDate,  ChallanDate, DTTItemGrid);
        //}
        public async Task<ResponseResult> GetAdjustedChallanDetailsData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, DataTable DTTItemGrid)
        {
            return await _CustomerJobWorkIssueDAL.GetAdjustedChallanDetailsData(YearCode, EntryDate, ChallanDate, AccountCode, DTTItemGrid);
        }

        //public List<CustomerJobWorkIssueModel> GetAdjustedChallanDetailsData(List<CustomerJobWorkIssueModel> adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode)
        //{
        //    return _CustomerJobWorkIssueDAL.GetAdjustedChallanDetailsData(adjustedData, YearCode, EntryDate, ChallanDate, AccountCode);
        //}
        public async Task<ResponseResult> GetPopUpData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, string prodUnProd, string BOMINd, int RMItemCode, string Partcode)
        {
            return await _CustomerJobWorkIssueDAL.GetPopUpData(YearCode, EntryDate, ChallanDate, AccountCode, prodUnProd, BOMINd, RMItemCode, Partcode);
        }
    }
}
