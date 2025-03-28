using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ILedgerPartyWiseOpening
    {
		Task<ResponseResult> FillEntryId();
		Task<ResponseResult> FillLedgerName(int OpeningYearCode);
        Task<ResponseResult> FillAccountNameForDashBoard();
        Task<ResponseResult> FillInvoiceForDashBoard();
        Task<ResponseResult> GetOpeningAmt(int OpeningYearCode, int AccountCode);
        Task<ResponseResult> FillDueDate( int AccountCode);
        Task<ResponseResult> SaveLedgerPartyWiseOpening(LedgerPartyWiseOpeningModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData();
        Task<LedgerPartyWiseOpeningDashBoardModel> GetDashboardDetailData(string LedgerName, string BillNo);
        Task<LedgerPartyWiseOpeningModel> GetViewByID(int OpeningYearCode, int LedgerOpnEntryId);
        Task<ResponseResult> DeleteByID(string EntryByMachine,int OpeningYearCode,int LedgerOpnEntryId,int AccountCode);
        Task<LedgerPartyWiseOpeningModel> GetAllDataAccountCodeWise(int OpeningYearCode, int AccountCode);
    }
}
