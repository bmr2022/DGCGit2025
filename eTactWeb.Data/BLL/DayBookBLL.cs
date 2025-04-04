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
    public class DayBookBLL:IDayBook
    {
        private DayBookDAL _DayBookDAL;
        private readonly IDataLogic _DataLogicDAL;

        public DayBookBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _DayBookDAL = new DayBookDAL(config, dataLogicDAL,connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string FromDate, string ToDate)
        {
            return await _DayBookDAL.FillLedgerName( FromDate,  ToDate);
        }
        public async Task<ResponseResult> FillVoucherName(string FromDate, string ToDate)
        {
            return await _DayBookDAL.FillVoucherName( FromDate,  ToDate);
        }
        public async Task<DayBookModel> GetDayBookDetailsData(string FromDate, string ToDate, string Ledger, string VoucherType, string CrAmt, string DrAmt)
        {
            return await _DayBookDAL.GetDayBookDetailsData(FromDate, ToDate,  Ledger,  VoucherType,  CrAmt,  DrAmt);
        }
    }
}
