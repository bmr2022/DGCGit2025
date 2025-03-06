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
    public class AccGroupLedgerBLL:IAccGroupLedger
    {
        private AccGroupLedgerDAL _AccGroupLedgerDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AccGroupLedgerBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _AccGroupLedgerDAL = new AccGroupLedgerDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillGroupName(string FromDate, string ToDate)
        {
            return await _AccGroupLedgerDAL.FillGroupName( FromDate,  ToDate);
        }
        public async Task<AccGroupLedgerModel> GetGroupLedgerDetailsData(string FromDate, string ToDate, int GroupCode, string ReportType)
        {
            return await _AccGroupLedgerDAL.GetGroupLedgerDetailsData(FromDate, ToDate, GroupCode, ReportType);
        }
    }
}
