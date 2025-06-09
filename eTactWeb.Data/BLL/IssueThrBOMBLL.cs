using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.BLL
{
    public class IssueThrBOMBLL: IIssueThrBOM
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly IssueThrBOMDAL _IssueThrBOMDAL;
       
        public IssueThrBOMBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _IssueThrBOMDAL = new IssueThrBOMDAL(configuration, iDataLogic);
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _IssueThrBOMDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            return await _IssueThrBOMDAL.GetNewEntry(YearCode);
        }

        public async Task<DataSet> FillEmployee(string Flag)
        {
            return await _IssueThrBOMDAL.FillEmployee(Flag);
        }

        public async Task<ResponseResult> FillProjectNo()
        {
            return await _IssueThrBOMDAL.FillProjectNo();
        }
        public async Task<ResponseResult> GetReqByName(string reqno, int yearcode)
        {
            return await _IssueThrBOMDAL.GetReqByName( reqno,  yearcode);
        }
        public async Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IssueThrBOMDAL.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
        }

        public async Task<ResponseResult> GETWIPSTOCKBATCHWISE(int ItemCode, int WCID, int LAST_YEAR, string BatchNo, string UniqBatchNo)
        {
            return await _IssueThrBOMDAL.GETWIPSTOCKBATCHWISE(ItemCode, WCID, LAST_YEAR, BatchNo, UniqBatchNo);
        }

        public async Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IssueThrBOMDAL.CheckStockBeforeSaving(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
        }

        public async Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, int ReqyearCode, int ItemCode)
        {
            return await _IssueThrBOMDAL.CheckRequisitionBeforeSaving(ReqNo, ReqyearCode, ItemCode);
        }
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            return await _IssueThrBOMDAL.GetIsStockable(ItemCode);
        }
        public async Task<ResponseResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate)
        {
            return await _IssueThrBOMDAL.FillBatchUnique(ItemCode, YearCode, StoreName, BatchNo, IssuedDate, FinStartDate);
        }
        public async Task<ResponseResult> GetAllowBatch()
        {
            return await _IssueThrBOMDAL.GetAllowBatch();
        }
        public async Task<ResponseResult> FillFGDataList(string Reqno, int ReqYC)
        {
            return await _IssueThrBOMDAL.FillFGDataList(Reqno,ReqYC);
        }
        public async Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate)
        {
            return await _IssueThrBOMDAL.GetItemDetailFromUniqBatch(UniqBatchNo,YearCode,TransDate);
        }
        public async Task<ResponseResult> GetReqQtyForScan(string ReqNo, int ReqYearCode, string ReqDate, int ItemCode)
        {
            return await _IssueThrBOMDAL.GetReqQtyForScan(ReqNo,ReqYearCode,ReqDate,ItemCode);
        }
        public async Task<ResponseResult> SaveIssueThrBom(IssueThrBom model, DataTable RMGrid,DataTable FGGrid)
        {
            return await _IssueThrBOMDAL.SaveIssueThrBom(model, RMGrid,FGGrid);
        }

        public async Task<IList<TextValue>> GetEmployeeList()
        {
            return await _IssueThrBOMDAL.GetEmployeeList();
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            return await _IssueThrBOMDAL.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _IssueThrBOMDAL.GetDashboardData(FromDate, Todate, Flag);
        }
        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate, string DashboardType, string IssueSlipNo, string ReqNo)
        {
            return await _IssueThrBOMDAL.GetDashboardData(FromDate, ToDate, DashboardType, IssueSlipNo, ReqNo);
        }
        public async Task<IssueThrBomDashboard> GetSearchData(string DashboardType, string FromDate, string ToDate, string IssueSlipNo, string ReqNo, string WCName, string ItemName, string PartCode)
        {
            return await _IssueThrBOMDAL.GetSearchData(DashboardType, FromDate, ToDate, IssueSlipNo, ReqNo, WCName, ItemName, PartCode);
        }
        public async Task<IssueThrBom> GetViewByID(int ID, int YearCode)
        {
            return await _IssueThrBOMDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _IssueThrBOMDAL.DeleteByID(ID, YC);
        }
        public async Task<IssueThrBomMainDashboard> FGDetailData(string FromDate, string Todate, string Flag = "", string DashboardType = "FGSUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "")
        {
            return await _IssueThrBOMDAL.FGDetailData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo, FGPartCode, FGItemName);
        } 
        public async Task<IssueThrBomMainDashboard> RMDetailData(string FromDate, string Todate, string WCName, string PartCode,string ItemName, string Flag = "", string DashboardType = "RMDETAIL", string IssueSlipNo = "", string ReqNo = "", string GlobalSearch = "", string FGPartCode = "", string FGItemName = "")
        {
            return await _IssueThrBOMDAL.RMDetailData(FromDate, Todate, WCName, PartCode, ItemName, Flag, DashboardType, IssueSlipNo, ReqNo, GlobalSearch, FGPartCode, FGItemName);
        }
        public async Task<IssueThrBomMainDashboard> SummaryData(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "")
        {
            return await _IssueThrBOMDAL.SummaryData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo);
        }
    }
}
