using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IReqThruBom
    {
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> GetBomRevNo(int Itemcode);
        Task<ResponseResult> GetProjectNo();
        Task<ResponseResult> GetPopUpData(int Itemcode,int BomNo);
        Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName);
        Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo);
        Task<ResponseResult> FillItems();
        Task<ResponseResult> FillWorkOrder();
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> FillTotalStock(int ItemCode, int Store);
        Task<RTBDashboard> GetDashboardData(string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName,string BranchName, string FromDate, string Todate);
        Task<ResponseResult> SaveRequisition(RequisitionThroughBomModel model, DataTable ReqGrid);
        Task<ResponseResult> GetDashboardData(string Fromdate, string ToDate, string Flag);
        Task<ResponseResult> DeleteByID(int ID, int YearCode);
        Task<RequisitionThroughBomModel> GetViewByID(int ID, int YearCode);
        Task<RTBDashboard> GetDetailData(string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> CheckFeatureOption();

    }
}
