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
    public class AgainstAdjustVoucherBLL : IAgainstAdjustVoucher
    {
        private AgainstAdjustVoucherDAL _AgainstAdjustVoucherDAL;
        private readonly IDataLogic _DataLogicDAL;
        public AgainstAdjustVoucherBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _AgainstAdjustVoucherDAL = new AgainstAdjustVoucherDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _AgainstAdjustVoucherDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _AgainstAdjustVoucherDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillVoucherType(int Yearcode)
        {
            return await _AgainstAdjustVoucherDAL.FillVoucherType(Yearcode);
        }
        public async Task<ResponseResult> FillInvoiceNo(int YearCode, string VoucherType, string FromDate, string ToDate, string VoucherNo, int AccountCode)
        {
            return await _AgainstAdjustVoucherDAL.FillInvoiceNo(YearCode, VoucherType,FromDate,ToDate,VoucherNo,AccountCode);
        }
        public async Task<ResponseResult> GetAccEntryId(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo)
        {
            return await _AgainstAdjustVoucherDAL.GetAccEntryId(YearCode, VoucherType,VoucherNo,AccountCode,InvoiceNo);
        } 
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _AgainstAdjustVoucherDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
        }
        //public async Task<AgainstAdjustVoucherModel> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId)
        //{
        //    return await _AgainstAdjustVoucherDAL.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
        //}
        public async Task<List<AgainstAdjustVoucherModel>> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId)
        {
            return await _AgainstAdjustVoucherDAL.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
        }
        public async Task<ResponseResult> FillVoucherNo(int YearCode, string VoucherType, string FromDate, string ToDate, int AccountCode)
        {
            return await _AgainstAdjustVoucherDAL.FillVoucherNo(YearCode, VoucherType,FromDate,ToDate,AccountCode);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _AgainstAdjustVoucherDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _AgainstAdjustVoucherDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _AgainstAdjustVoucherDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _AgainstAdjustVoucherDAL.FillCurrency();
        }
        public async Task<AgainstAdjustVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _AgainstAdjustVoucherDAL.GetViewByID(ID, YearCode, VoucherNo);
        }
        //public async Task<ResponseResult> SaveAgainstAdjustVoucher(AgainstAdjustVoucherModel model, DataTable GIGrid)
        //{
        //    return await _AgainstAdjustVoucherDAL.SaveAgainstAdjustVoucher(model, GIGrid);
        //}
    }
}

