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
    public class LedgerOpeningEntryBLL:ILedgerOpeningEntry
    {
        private LedgerOpeningEntryDAL _LedgerOpeningEntryDAL;
        private readonly IDataLogic _DataLogicDAL;

        public LedgerOpeningEntryBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _LedgerOpeningEntryDAL = new LedgerOpeningEntryDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetLedgersByGroup(string groupAccountCode)
        {
            return await _LedgerOpeningEntryDAL.GetLedgersByGroup( groupAccountCode);
        }
        public async Task<ResponseResult> GetGroupByAccountCode(int AccountCode)
        {
            return await _LedgerOpeningEntryDAL.GetGroupByAccountCode(AccountCode);
        } 
        public async Task<ResponseResult> GetLedgerByAccountCode(int AccountCode, int OpeningForYear, string ActualEntryDate)
        {
            return await _LedgerOpeningEntryDAL.GetLedgerByAccountCode( AccountCode,  OpeningForYear,  ActualEntryDate);
        }
        public async Task<ResponseResult> GetLedgersALLGroup()
        {
            return await _LedgerOpeningEntryDAL.GetLedgersALLGroup();
        }
        public async Task<ResponseResult> GetAllGroupName()
        {
            return await _LedgerOpeningEntryDAL.GetAllGroupName();
        }
        public async Task<ResponseResult> SaveWorkOrderProcess(LedgerOpeningEntryModel model)
        {
            return await _LedgerOpeningEntryDAL.SaveWorkOrderProcess(model);
        }
        public async Task<LedgerOpeningEntryModel> GetViewByID(int AccountCode)
        {
            return await _LedgerOpeningEntryDAL.GetViewByID(AccountCode);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _LedgerOpeningEntryDAL.GetDashboardData();
        }
        public async Task<LedgerOpeningEntryDashBoardGridModel> GetDashboardDetailData(string GroupName, string LedgerName, float PreviousAmount, string DrCr)
        {
            return await _LedgerOpeningEntryDAL.GetDashboardDetailData( GroupName,  LedgerName, PreviousAmount,  DrCr);
        }
        public async Task<ResponseResult> DeleteByID(int AC, int YC)
        {
            return await _LedgerOpeningEntryDAL.DeleteByID(AC, YC);
        }
    }
}
