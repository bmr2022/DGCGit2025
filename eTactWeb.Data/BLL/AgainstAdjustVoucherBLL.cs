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
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _AgainstAdjustVoucherDAL.FillSubVoucherName(VoucherType);
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

