using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IRetFromDepartmentMain
    {
        Task<ResponseResult> GetDashboardData(string Fromdate, string ToDate, string Flag);
        Task<ResponseResult> FillItems(string returnByEmpName, string showAllItems);
        Task<ResponseResult> FillPartCode(string returnByEmpName);
        Task<ResponseResult> FillBatchNo(int itemCode, int receiveByEmp, DateTime? retFromDepEntrydate, int retFromDepYearCode);
        Task<ResponseResult> GetNewEntry(int yearCode);
        Task<ResponseResult> SaveRetFromDeptMain(ReturnFromDepartmentMainModel model, DataTable ReqGrid);
        Task<ReturnFromDepartmentMainModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> DeleteByID(int ID, int YC);
    }
}
