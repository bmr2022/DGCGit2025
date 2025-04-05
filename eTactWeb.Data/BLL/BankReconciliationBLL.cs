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
    public class BankReconciliationBLL: IBankReconciliation
    {
        private BankReconciliationDAL _BackReconciliationDAL;
        private readonly IDataLogic _DataLogicDAL;

        public BankReconciliationBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _BackReconciliationDAL = new BankReconciliationDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetBankName(string DateFrom, string DateTo, string NewOrEdit)
        {
            return await _BackReconciliationDAL.GetBankName( DateFrom,  DateTo,  NewOrEdit);
        }

        public async Task<BankReconciliationModel> GetDetailsData(string DateFrom, string DateTo, string chequeNo, string NewOrEdit,string BankAccountCode)
        {
            return await _BackReconciliationDAL.GetDetailsData( DateFrom,  DateTo,  chequeNo,  NewOrEdit, BankAccountCode);
        }
    }
}
