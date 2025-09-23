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

namespace eTactWeb.Data.BLL
{
    public class BalanceSheetBLL : IBalanceSheet
    {
        private BalanceSheetDAL _BalanceSheetDAL;
        private readonly IDataLogic _DataLogicDAL;
        public BalanceSheetBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _BalanceSheetDAL = new BalanceSheetDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<BalanceSheetModel> GetBalanceSheetData(string FromDate, string ToDate, string ReportType)
        {
            return await _BalanceSheetDAL.GetBalanceSheetData(FromDate, ToDate, ReportType);
        }
    }
}
