using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IEmployeeAdvancePayement
    {
        Task<ResponseResult> FillEntryId(int yearCode);
        Task<ResponseResult> FillEmpName();
        Task<ResponseResult> FillPreviousAdanaceLoanDetail(int empId, string requestedDate);
        Task<ResponseResult> FillFinancialEmployeeNameList();
        Task<ResponseResult> FillMgmtEmployeeNameList();
        Task<ResponseResult> FillEmpCode();
        Task<ResponseResult> FillEmployeeDetail(int empId);
        Task<ResponseResult> SaveEmployeeAdvancePayment(HRAdvanceModel model);
    }
}
