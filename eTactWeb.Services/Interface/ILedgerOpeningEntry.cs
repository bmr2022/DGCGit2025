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
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetAccountCodeandParentAccountCode(string AccountName);
        Task<ResponseResult> UpdateMultipleDataFromExcel(DataTable ItemDetailGrid, string flag, int CloseingYearCode, string MachineName, string IPAddress, string CC, int EntryByEmpId);

        Task<ResponseResult> GetGroupByAccountCode(int AccountCode);
        Task<ResponseResult> GetAmountAndType(int AccountCode, int OpeningForYear, string ActualEntryDate);
        Task<ResponseResult> GetLedgersALLGroup();
        Task<ResponseResult> GetAllGroupName();
        Task<ResponseResult> GetLedgersByGroup(string groupAccountCode);
        Task<ResponseResult> SaveWorkOrderProcess(LedgerOpeningEntryModel model);
        Task<LedgerOpeningEntryModel> GetViewByID(int AccountCode);
        Task<ResponseResult> GetDashboardData(int userID);
        Task<LedgerOpeningEntryDashBoardGridModel> GetDashboardDetailData(string GroupName, int userID, string LedgerName, float Amount, string DrCr);
        Task<ResponseResult> DeleteByID(int AC, int YC, int userID);
    }
}
