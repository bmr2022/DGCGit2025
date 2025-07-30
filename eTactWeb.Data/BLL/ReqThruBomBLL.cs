using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class ReqThruBomBLL : IReqThruBom
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ReqThruBomDAL _ReqThruBomDAL;
        public ReqThruBomBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _ReqThruBomDAL = new ReqThruBomDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _ReqThruBomDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _ReqThruBomDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillItems()
        {
            return await _ReqThruBomDAL.FillItems();
        }
        public async Task<ResponseResult> AutoFillPartCode(string showallitem, string SearchItemCode, string SearchPartCode)
        {
            return await _ReqThruBomDAL.AutoFillPartCode(showallitem, SearchItemCode, SearchPartCode);
        } 
        public async Task<ResponseResult> AutoFillItemName(string showallitem, string SearchItemCode, string SearchPartCode)
        {
            return await _ReqThruBomDAL.AutoFillItemName(showallitem, SearchItemCode, SearchPartCode);
        }
        public async Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName)
        {
            return await _ReqThruBomDAL.GetNewEntry(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> FillWorkOrder()
        {
            return await _ReqThruBomDAL.FillWorkOrder();
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _ReqThruBomDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Store)
        {
            return await _ReqThruBomDAL.FillTotalStock(ItemCode, Store);
        }
        public async Task<ResponseResult> GetBomRevNo(int ItemCode)
        {
            return await _ReqThruBomDAL.GetBomRevNo(ItemCode);
        }
        public async Task<ResponseResult> GetProjectNo()
        {
            return await _ReqThruBomDAL.GetProjectNo();
        }
        public async Task<ResponseResult> GetPopUpData(int ItemCode, int BomNo)
        {
            return await _ReqThruBomDAL.GetPopUpData(ItemCode, BomNo);
        }
        public async Task<ResponseResult> SaveRequisition(RequisitionThroughBomModel model, DataTable ReqGrid)
        {
            return await _ReqThruBomDAL.SaveRequisition(model, ReqGrid);
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _ReqThruBomDAL.GetDashboardData(FromDate, Todate, Flag);
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            return await _ReqThruBomDAL.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
        }
        public async Task<RTBDashboard> GetDashboardData(string REQNo, string WCName,string WONO, string DepName, string PartCode, string ItemName,string BranchName, string FromDate, string ToDate)
        {
            return await _ReqThruBomDAL.GetDashboardData(REQNo, WCName, WONO, DepName, PartCode, ItemName,BranchName, FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _ReqThruBomDAL.DeleteByID(ID, YC);
        }
        public async Task<RequisitionThroughBomModel> GetViewByID(int ID, int YearCode)
        {
            return await _ReqThruBomDAL.GetViewByID(ID, YearCode);
        }
        public async Task<RTBDashboard> GetDetailData(string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            return await _ReqThruBomDAL.GetDetailData(REQNo, WCName, WONo, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _ReqThruBomDAL.CheckFeatureOption();
        }
    }
}
