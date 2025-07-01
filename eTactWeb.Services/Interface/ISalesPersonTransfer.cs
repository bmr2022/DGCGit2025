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
        Task<ResponseResult> FillEntryID(int YearCode);
		Task<ResponseResult> SaveSalesPersonTransfer(SalesPersonTransferModel model, DataTable GIGrid);
	}
}
