using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class SaleBillBLL : ISaleBill
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly SaleBillDAL _SaleBillDAL;
        public async Task<ResponseResult> ShowPendingSaleorderforBill(string Flag, int CurrentYear, string FromDate, string Todate, string InvoiceDate, int BillFromStoreId, int accountCode, string SONo, string PartCode)
        {
            return await _SaleBillDAL.ShowPendingSaleorderforBill( Flag,  CurrentYear,  FromDate,  Todate,  InvoiceDate,  BillFromStoreId,  accountCode,  SONo,  PartCode);
        }

        public async Task<ResponseResult> FILLPendingSONO(string Flag, int CurrentYear, string FromDate, string Todate, string InvoiceDate, int BillFromStoreId, int accountCode)
        {
            return await _SaleBillDAL.FILLPendingSONO(Flag, CurrentYear, FromDate, Todate, InvoiceDate, BillFromStoreId, accountCode);
        }
        public async Task<ResponseResult> FillPendingPartCOde(string Flag, int CurrentYear, string FromDate, string Todate, string InvoiceDate, int BillFromStoreId, int accountCode)
        {
            return await _SaleBillDAL.FillPendingPartCOde(Flag, CurrentYear, FromDate, Todate, InvoiceDate, BillFromStoreId, accountCode);
        }

        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _SaleBillDAL.GetFormRights(ID);
        }
        public async Task<SaleBillModel> ShowGroupWiseItems(int Group_Code, int AccountCode, int storeid, string GroupName, string ToDate, string FromDate, string PartCode)
        {
            return await _SaleBillDAL.ShowGroupWiseItems( Group_Code,  AccountCode,  storeid,  GroupName,  ToDate,  FromDate,  PartCode);
        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _SaleBillDAL.GetReportName();
        }
        public SaleBillBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _SaleBillDAL = new SaleBillDAL(configuration, iDataLogic,connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _SaleBillDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> getdiscCategoryName(int Group_Code, int AccountCode)
        {
            return await _SaleBillDAL.getdiscCategoryName(Group_Code, AccountCode);
        }
        public async Task<ResponseResult> AutoFillStore( string SearchStoreName)
        {
            return await _SaleBillDAL.AutoFillStore(SearchStoreName);
        }
        public async Task<ResponseResult> GetItemGroup()
        {
            return await _SaleBillDAL.GetItemGroup();
        }

        public async Task<ResponseResult> GETGROUPWISEITEM(int Group_Code)
        {
            return await _SaleBillDAL.GETGROUPWISEITEM(Group_Code);
        }
        public async Task<ResponseResult> GetBatchInventory()
        {
            return await _SaleBillDAL.GetBatchInventory();
        }
        public async Task<ResponseResult> GetCustomerBasedDetails(int Code)
        {
            return await _SaleBillDAL.GetCustomerBasedDetails(Code);
        }
        public async Task<ResponseResult> FillCurrency(int accountCode)
        {
            return await _SaleBillDAL.FillCurrency(accountCode);
        }
        public async Task<ResponseResult> GetAutocompleteValue()
        {
            return await _SaleBillDAL.GetAutocompleteValue();
        }
        public async Task<ResponseResult> FillCustomerList(string SBJobwork, string ShowAllCustomer)
        {
            return await _SaleBillDAL.FillCustomerList(SBJobwork,ShowAllCustomer);
        }
        public async Task<ResponseResult> FillJWCustomerList(string SBJobwork, int yearCode)
        {
            return await _SaleBillDAL.FillJWCustomerList(SBJobwork,yearCode);
        }
        public async Task<ResponseResult> GetDistance(int accountCode)
        {
            return await _SaleBillDAL.GetDistance(accountCode);
        }
        public async Task<ResponseResult> FillDocument(string ShowAllDoc)
        {
            return await _SaleBillDAL.FillDocument(ShowAllDoc);
        }
        public async Task<ResponseResult> FillSONO(string billDate, string accountCode, string billType)
        {
            return await _SaleBillDAL.FillSONO(billDate,accountCode,billType);
        }
        public async Task<ResponseResult> FillConsigneeList(string showAllConsignee)
        {
            return await _SaleBillDAL.FillConsigneeList(showAllConsignee);
        }     
        public async Task<ResponseResult> FillSOYearCode(string sono,string accountCode)
        {
            return await _SaleBillDAL.FillSOYearCode(sono,accountCode);
        }     
        public async Task<ResponseResult> FillSOSchedule(string sono, string accountCode, int soYearCode, int ItemCode)
        {
            return await _SaleBillDAL.FillSOSchedule(sono,accountCode,soYearCode,ItemCode);
        }    
        public async Task<ResponseResult> FillSOItemRate(string sono, int soYearCode, int accountCode, string custOrderNo, int itemCode)
        {
            return await _SaleBillDAL.FillSOItemRate(sono,soYearCode,accountCode,custOrderNo,itemCode);
        }       
        public async Task<ResponseResult> FillItems( string showAll, string typeItemServAssets,string sbJobWork)
        {
            return await _SaleBillDAL.FillItems(showAll,typeItemServAssets,sbJobWork);
        }       
        public async Task<ResponseResult> FillSOWiseItems(string invoiceDate, string sono, int soYearCode, int accountCode, string schNo, int schYearCode, string sbJobWork)
        {
            return await _SaleBillDAL.FillSOWiseItems(invoiceDate,sono,soYearCode,accountCode,schNo,schYearCode,sbJobWork);
        }       
        public async Task<ResponseResult> JWItemList(string typeItemServAssets, string showAll,string bomInd, string schNo, int schYearCode)
        {
            return await _SaleBillDAL.JWItemList(typeItemServAssets,showAll,bomInd,schNo,schYearCode);
        } 
        public async Task<ResponseResult> FillStore()
        {
            return await _SaleBillDAL.FillStore();
        }
        public async Task<ResponseResult> FillTransporter()
        {
            return await _SaleBillDAL.FillTransporter();
        }  
        public async Task<ResponseResult> DisplaySODetail(string accountName, string itemName, string partCode, string sono, int soYearCode, string custOrderNo, string schNo, int schYearCode)
        {
            return await _SaleBillDAL.DisplaySODetail(accountName, itemName, partCode, sono, soYearCode, custOrderNo, schNo, schYearCode);
        }  
        public async Task<ResponseResult> GetDashboardData(string summaryDetail, string partCode, string itemName, string saleBillno, string customerName, string sono, string custOrderNo, string schNo, string performaInvNo, string saleQuoteNo, string domensticExportNEPZ, string fromdate, string toDate)
        {
            return await _SaleBillDAL.GetDashboardData(summaryDetail, partCode, itemName, saleBillno,customerName,sono,custOrderNo,schNo,performaInvNo,saleQuoteNo,domensticExportNEPZ,fromdate,toDate);
        }  
        public async Task<ResponseResult> FILLSOScheduleDate(string sono, int accountCode, int soYearCode, string schNo, int schYearCode)
        {
            return await _SaleBillDAL.FILLSOScheduleDate(sono,accountCode,soYearCode,schNo,schYearCode);
        }  
        public async Task<ResponseResult> SaveSaleBill(SaleBillModel model, DataTable SBGrid, DataTable TaxDetailDT,DataTable DrCrDetailDT,DataTable AdjDetailDT,DataTable AdjChallanDetailDT)
        {
            return await _SaleBillDAL.SaveSaleBill(model,SBGrid,TaxDetailDT, DrCrDetailDT, AdjDetailDT, AdjChallanDetailDT);
        }  
        public async Task<ResponseResult> FILLCustomerOrderAndSPDate(string billDate, int accountCode, string sono, int soYearCode)
        {
            return await _SaleBillDAL.FILLCustomerOrderAndSPDate(billDate,accountCode,sono,soYearCode);
        }     
        public async Task<ResponseResult> AllowTaxPassword()
        {
            return await _SaleBillDAL.AllowTaxPassword();
        }     
        public async Task<ResponseResult> FillSOPendQty(int saleBillEntryId, string saleBillNo, int saleBillYearCode,string sono, int soYearcode, string custOrderNo, int itemCode, int accountCode, string schNo, int schYearCode)
        {
            return await _SaleBillDAL.FillSOPendQty(saleBillEntryId,saleBillNo,saleBillYearCode,sono, soYearcode, custOrderNo, itemCode, accountCode, schNo, schYearCode);
        }          
        public async Task<SaleBillModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _SaleBillDAL.GetViewByID(ID, YC, Mode);
        }          
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string machineName)
        {
            return await _SaleBillDAL.DeleteByID(ID, YC,machineName);
        }          
        public  async Task<AdjChallanDetail> GetAdjustedChallanDetailsData(DataTable adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode)
        {
            return await  _SaleBillDAL.GetAdjustedChallanDetailsData(adjustedData,  YearCode,  EntryDate, ChallanDate, AccountCode);
        }
        public async Task<ResponseResult> GetReportData(int EntryId, int YearCode, string Type,string InvoiceNo,int AccountCode)
        {
            return await _SaleBillDAL.GetReportData(EntryId, YearCode, Type,InvoiceNo,AccountCode);
        }
        public async Task<ResponseResult> GetMaxSaleInvoiceEntryDate(int YearCode)
        {
            return await _SaleBillDAL.GetMaxSaleInvoiceEntryDate(YearCode);
        }
        public async Task<ResponseResult> GetFeatureOption()
        {
            return await _SaleBillDAL.GetFeatureOption();
        }
        public async Task<ResponseResult> FillStoreList()
        {
            return await _SaleBillDAL.FillStoreList();
        }
        public async Task<ResponseResult> FillCustomerListForPending()
        {
            return await _SaleBillDAL.FillCustomerListForPending();
        }



    }
}
