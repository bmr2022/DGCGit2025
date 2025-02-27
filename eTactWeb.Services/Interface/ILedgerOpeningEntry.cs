using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ILedgerOpeningEntry
    {
        Task<ResponseResult> GetGroupByAccountCode(int AccountCode); 
        Task<ResponseResult> GetLedgerByAccountCode(int AccountCode, int OpeningForYear, string ActualEntryDate);
        Task<ResponseResult> GetLedgersALLGroup();
        Task<ResponseResult> GetAllGroupName();
        Task<ResponseResult> GetLedgersByGroup(string groupAccountCode);
        Task<ResponseResult> SaveWorkOrderProcess(LedgerOpeningEntryModel model);
        Task<LedgerOpeningEntryModel> GetViewByID(int AccountCode);
        Task<ResponseResult> GetDashboardData();
        Task<LedgerOpeningEntryDashBoardGridModel> GetDashboardDetailData(string GroupName, string LedgerName, float Amount, string DrCr);
        Task<ResponseResult> DeleteByID(int AC, int YC);
    }
}
