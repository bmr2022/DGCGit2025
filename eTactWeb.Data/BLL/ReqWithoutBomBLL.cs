using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class ReqWithoutBomBLL:IReqWithoutBOM
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ReqWithoutBomDAL _ReqWithoutBomDAL;
        public ReqWithoutBomBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _ReqWithoutBomDAL = new ReqWithoutBomDAL(configuration, iDataLogic,connectionStringService);
        }
        public async Task<ResponseResult> AutoFillitem(string Flag, string TF, string SearchItemCode, string SearchPartCode)
        {
            return await _ReqWithoutBomDAL.AutoFillitem(Flag, TF, SearchItemCode, SearchPartCode);
        }
        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _ReqWithoutBomDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _ReqWithoutBomDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillItems(string TF)
        {
            return await _ReqWithoutBomDAL.FillItems(TF);
        }
        public async Task<ResponseResult> FillDept()
        {
            return await _ReqWithoutBomDAL.FillDept();
        }
        public async Task<ResponseResult> FillPartCode(string TF)
        {
            return await _ReqWithoutBomDAL.FillPartCode(TF);
        }
        public async Task<ResponseResult> FillWorkOrder()
        {
            return await _ReqWithoutBomDAL.FillWorkOrder();
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _ReqWithoutBomDAL.CheckFeatureOption();
        }
        public async Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName)
        {
            return await _ReqWithoutBomDAL.GetNewEntry(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> GetProjectNo()
        {
            return await _ReqWithoutBomDAL.GetProjectNo();
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _ReqWithoutBomDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _ReqWithoutBomDAL.FillStore();
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            return await _ReqWithoutBomDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Store)
        {
            return await _ReqWithoutBomDAL.FillTotalStock(ItemCode,Store);
        }
        public async Task<ResponseResult> SaveRequisition(RequisitionWithoutBOMModel model, DataTable ReqGrid)
        {
            return await _ReqWithoutBomDAL.SaveRequisition(model,ReqGrid);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _ReqWithoutBomDAL.GetDashboardData(FromDate,Todate, Flag);
        }
        public async Task<RWBDashboard> GetDashboardData(string REQNo, string WCName,string WONO, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            return await _ReqWithoutBomDAL.GetDashboardData(REQNo, WCName,WONO, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
        }
        public async Task<RWBDashboard> GetDetailData(string REQNo, string WCName,string WONO, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            return await _ReqWithoutBomDAL.GetDetailData(REQNo, WCName,WONO, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _ReqWithoutBomDAL.DeleteByID(ID, YC);
        }

        public async Task<RequisitionWithoutBOMModel> GetViewByID(int ID, int YearCode)
        {
            return await _ReqWithoutBomDAL.GetViewByID(ID, YearCode);
        }
    }
}
