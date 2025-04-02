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
    public  class LedgerPartyWiseOpeningBLL:ILedgerPartyWiseOpening
    {
        private LedgerPartyWiseOpeningDAL _LedgerPartyWiseOpeningDAL;
        private readonly IDataLogic _DataLogicDAL;

        public LedgerPartyWiseOpeningBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _LedgerPartyWiseOpeningDAL = new LedgerPartyWiseOpeningDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

		public async Task<ResponseResult> FillEntryId()
		{
			return await _LedgerPartyWiseOpeningDAL.FillEntryId();
		}
		public async Task<ResponseResult> FillLedgerName(int OpeningYearCode)
        {
            return await _LedgerPartyWiseOpeningDAL.FillLedgerName(OpeningYearCode);
        }
        public async Task<ResponseResult> FillAccountNameForDashBoard()
        {
            return await _LedgerPartyWiseOpeningDAL.FillAccountNameForDashBoard();
        }
        public async Task<ResponseResult> FillInvoiceForDashBoard()
        {
            return await _LedgerPartyWiseOpeningDAL.FillInvoiceForDashBoard();
        }
        public async Task<ResponseResult> GetOpeningAmt(int OpeningYearCode, int AccountCode)
        {
            return await _LedgerPartyWiseOpeningDAL.GetOpeningAmt(OpeningYearCode,  AccountCode);
        }
        public async Task<ResponseResult> FillDueDate(int AccountCode)
        {
            return await _LedgerPartyWiseOpeningDAL.FillDueDate(  AccountCode);
        }
        public async Task<ResponseResult> SaveLedgerPartyWiseOpening(LedgerPartyWiseOpeningModel model, DataTable GIGrid)
        {
            return await _LedgerPartyWiseOpeningDAL.SaveLedgerPartyWiseOpening(model,GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _LedgerPartyWiseOpeningDAL.GetDashboardData();
        }
        public async Task<LedgerPartyWiseOpeningDashBoardModel> GetDashboardDetailData(string LedgerName, string BillNo)
        {
            return await _LedgerPartyWiseOpeningDAL.GetDashboardDetailData( LedgerName,  BillNo);

        }
        public async Task<LedgerPartyWiseOpeningModel> GetViewByID(int OpeningYearCode, int LedgerOpnEntryId)
        {
            return await _LedgerPartyWiseOpeningDAL.GetViewByID( OpeningYearCode,  LedgerOpnEntryId);

        }
        public async Task<ResponseResult> DeleteByID(string EntryByMachine, int OpeningYearCode, int LedgerOpnEntryId, int AccountCode)
        {
            return await _LedgerPartyWiseOpeningDAL.DeleteByID( EntryByMachine,  OpeningYearCode,  LedgerOpnEntryId,  AccountCode);
        }
        public Task<LedgerPartyWiseOpeningModel> GetAllDataAccountCodeWise(int OpeningYearCode, int AccountCode)
        {
            return _LedgerPartyWiseOpeningDAL.GetAllDataAccountCodeWise( OpeningYearCode,  AccountCode);
        }
    }
}

