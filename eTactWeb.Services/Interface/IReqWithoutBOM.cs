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
    public interface IReqWithoutBOM
    {
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> FillItems(string TF);
        Task<ResponseResult> FillPartCode(string TF);
        public Task<ResponseResult> AutoFillitem(string Flag, string TF, string SearchItemCode, string SearchPartCode);

        Task<ResponseResult> GetProjectNo();
        Task<ResponseResult> FillDept();

        Task<ResponseResult> FillWorkOrder();
        Task<ResponseResult> CheckFeatureOption();
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> FillStore();
        Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName);
        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);
        Task<ResponseResult> FillTotalStock(int ItemCode, int Store);
        Task<RWBDashboard> GetDashboardData(string REQNo, string WCName,string Wono, string DepName, string PartCode, string ItemName,string BranchName, string FromDate, string Todate);
        Task<RWBDashboard> GetDetailData(string REQNo, string WCName,string Wono, string DepName, string PartCode, string ItemName,string BranchName, string FromDate, string Todate);
        Task<ResponseResult> SaveRequisition(RequisitionWithoutBOMModel model, DataTable ReqGrid);
        Task<ResponseResult> GetDashboardData(string Fromdate, string ToDate, string Flag);
        Task<ResponseResult> DeleteByID(int ID, int YearCode);
        Task<RequisitionWithoutBOMModel> GetViewByID(int ID, int YearCode);
        Task<ResponseResult> GetFormRights(int uId);

    }
}
