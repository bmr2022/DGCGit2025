using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISalesPersonTransfer
    {
		Task<ResponseResult> FillNewSalesEmpName(string ShowAllEmp);
		Task<ResponseResult> FillOldSalesEmpName(string ShowAllEmp);
        Task<SalesPersonTransferModel> FillCustomerList(string ShowAllCust);
        Task<ResponseResult> FillEntryID(int YearCode, string EntryDate);
		Task<ResponseResult> SaveSalesPersonTransfer(SalesPersonTransferModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData(SalesPersonTransferModel model);
        Task<SalesPersonTransferModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType);
        Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId);
        Task<SalesPersonTransferModel> GetViewByID(int ID, int YC, string FromDate, string TODate);
    }
}
