using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBankReconciliation
    {
        Task<ResponseResult> SaveBankReceipt(List<BankReconciliationModel> model, DataTable GIGrid);
        Task<ResponseResult> GetBankName(string DateFrom, string DateTo,string NewOrEdit);
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        public Task<BankReconciliationModel> GetDetailsData(string DateFrom, string DateTo, string chequeNo,string NewOrEdit,string Account_Code);
        Task<ResponseResult> GetFormRights(int uId);
    }
}
