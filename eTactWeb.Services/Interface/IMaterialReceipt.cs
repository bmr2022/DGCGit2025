using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMaterialReceipt
    {
        Task<ResponseResult> GetGateNo(string Flag,string SPName,string FromDate, string ToDate);
        Task<ResponseResult> GetReportName();
        Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateyearCode, int GateEntryId);

        Task<ResponseResult> GetGateItemData(string Flag, string SPName, string GateNo, string GateyearCode, int GateEntryId);

        Task<ResponseResult> GetDeptAndEmp(string Flag, string SPName, int Deptid,  int resEmp);

        Task<ResponseResult> SaveMaterialReceipt(MaterialReceiptModel model, DataTable MRGrid, DataTable BatchGrid);

        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);
        Task<ResponseResult> DeleteByID(int ID, int YC);
        Task<ResponseResult> FillEntryandMRN(string Flag,int YearCode, string SPName);
        Task<IList<TextValue>> GetEmployeeList();

        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> BindDept(string flag,string SpName);

        Task<MRNQDashboard> GetDashboardData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate);
        Task<MRNQDashboard> GetDetailDashboardData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate);
        Task<ResponseResult> GetSearchData(MRNQDashboard model);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode);
        Task<ResponseResult> CheckBeforeInsert(string GateNo, int GateYearCode);

        Task<MaterialReceiptModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> CheckFeatureOption();
    }
}
