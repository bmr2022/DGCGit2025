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
    public interface ISaleBill {
        Task<ResponseResult> GetReportName();
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> GetBatchInventory();
        Task<ResponseResult> GetCustomerBasedDetails(int Code);
        Task<ResponseResult> FillCurrency(int accountCode);
        Task<ResponseResult> GetAutocompleteValue();
        Task<ResponseResult> FillJWCustomerList(string SBJobwork, int yearCode);
        Task<ResponseResult> FillCustomerList(string SBJobwork, string ShowAllCustomer);
        Task<ResponseResult> GetDistance(int accountCode);
        Task<ResponseResult> FillDocument(string ShowAllDoc);
        Task<ResponseResult> FillSONO(string billDate, string accountCode);
        Task<ResponseResult> FillConsigneeList(string showAllConsignee);
        Task<ResponseResult> FillSOYearCode(string sono,string accountCode);
        Task<ResponseResult> DisplaySODetail(string accountName, string itemName, string partCode, string sono, int soYearCode, string custOrderNo, string schNo, int schYearCode);
        Task<ResponseResult> FillItems(string showAll, string TypeItemServAssets,string sbJobWork);
        Task<ResponseResult> FillSOWiseItems(string invoiceDate, string sono, int soYearCode, int accountCode, string schNo, int schYearCode, string sbJobWork);
        Task<ResponseResult> JWItemList(string typeItemServAssets, string showAll,string bomInd, string schNo, int schYearCode);
        Task<ResponseResult> FillStore();
        Task<ResponseResult> AllowTaxPassword();
        Task<ResponseResult> FillSOPendQty(int saleBillEntryId, string saleBillNo, int saleBillYearCode,string sono, int soYearcode, string custOrderNo, int itemCode, int accountCode, string schNo, int schYearCode);
        Task<ResponseResult> SaveSaleBill(SaleBillModel model, DataTable SBGrid,DataTable TaxDetailDT,DataTable DrCrDetailDT,DataTable AdjDetailDT ,DataTable AdjChallanDetailDT);
        Task<ResponseResult> FILLSOScheduleDate(string sono, int accountCode, int soYearCode, string schNo, int schYearCode);
        Task<ResponseResult> FillSOItemRate(string sono, int soYearCode, int accountCode, string custOrderNo, int itemCode);
        Task<ResponseResult> FILLCustomerOrderAndSPDate(string billDate, int accountCode, string sono, int soYearCode);
        Task<ResponseResult> FillSOSchedule(string sono,string accountCode, int soYearCode, int ItemCode); 
        Task<ResponseResult> GetDashboardData(string summaryDetail, string partCode, string itemName, string saleBillno, string customerName, string sono, string custOrderNo, string schNo, string performaInvNo, string saleQuoteNo, string domensticExportNEPZ, string fromdate, string toDate);
        Task<SaleBillModel> GetViewByID(int ID, string Mode, int YC);
        Task<ResponseResult> DeleteByID(int ID, int YC, string machineName);

        Task<List<CustomerJobWorkIssueAdjustDetail>> GetAdjustedChallanDetailsData(DataTable adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode);
        Task<ResponseResult> GetReportData(int EntryId, int YearCode, string Type, string InvoiceNo, int AccountCode);
    }
}
