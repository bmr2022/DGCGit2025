using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
       
    }
}
