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
    public class SaleRejectionBLL : ISaleRejection
    {
        private readonly SaleRejectionDAL _SaleRejectionDAL;
        private readonly IDataLogic _DataLogicDAL;
        private readonly ConnectionStringService _connectionStringService;
        public SaleRejectionBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _SaleRejectionDAL = new SaleRejectionDAL(configuration, iDataLogic,connectionStringService);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<SaleRejectionModel> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC,int yearCode)
        {
            return await _SaleRejectionDAL.FillSaleRejectionGrid(mrnNo, mrnEntryId, mrnYC,yearCode);
        }
        public async Task<ResponseResult> GetTotalAmount(SaleRejectionFilter model)
        {
            return await _SaleRejectionDAL.GetTotalAmount(model);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _SaleRejectionDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillCustomerName(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillCustomerName(fromDate, toDate);
        }
        public async Task<ResponseResult> FillItemName(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillItemName(fromDate, toDate);
        }
        public async Task<ResponseResult> FillPartCode(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillPartCode(fromDate, toDate);
        }
        public async Task<ResponseResult> FillInvoiceNo(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillInvoiceNo(fromDate, toDate);
        } 
        public async Task<ResponseResult> FillMRNNo(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillMRNNo(fromDate, toDate);
        }
        public async Task<ResponseResult> FillGateNo(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillGateNo(fromDate, toDate);
        }
        public async Task<ResponseResult> FillVoucherNo(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillVoucherNo(fromDate, toDate);
        }
        public async Task<ResponseResult> FillDocument(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillDocument(fromDate, toDate);
        }
        public async Task<ResponseResult> FillAgainstSaleBillNo(string fromDate, string toDate)
        {
            return await _SaleRejectionDAL.FillAgainstSaleBillNo(fromDate, toDate);
        }
        public async Task<ResponseResult> SaveSaleRejection(SaleRejectionModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT)
        {
            return await _SaleRejectionDAL.SaveSaleRejection(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT);
        }
        public async Task<ResponseResult> GetDashboardData(string summaryDetail, string custInvoiceNo, string custName, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string docName, string voucherNo, string fromdate, string toDate)
        {
            return await _SaleRejectionDAL.GetDashboardData(summaryDetail, custInvoiceNo, custName, mrnNo, gateNo, partCode, itemName, againstBillNo, docName, voucherNo, fromdate, toDate);
        }
        public async Task<SaleRejectionModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _SaleRejectionDAL.GetViewByID(ID, YC, Mode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _SaleRejectionDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC,int accountCode, int createdBy, string machineName,string cc)
        {
            return await _SaleRejectionDAL.DeleteByID(ID, YC,accountCode, createdBy, machineName, cc);
        }
    }
}
