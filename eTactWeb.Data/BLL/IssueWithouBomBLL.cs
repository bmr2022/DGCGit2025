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
    public class IssueWithouBomBLL : IIssueWithoutBom
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly IssueWithoutBomDAL _IssuewithoutBomDAL;
        public IssueWithouBomBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _IssuewithoutBomDAL = new IssueWithoutBomDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> FillBranch()
        {
            return await _IssuewithoutBomDAL.FillBranch();
        }
        public async Task<ResponseResult> GETDepartMent(string ReqNo, int ReqYearCode)
        {
            return await _IssuewithoutBomDAL.GETDepartMent( ReqNo,  ReqYearCode);
        }
        public async Task<ResponseResult> SaveIssueWithoutBom(IssueWithoutBom model, DataTable MRGRid)
        {
            return await _IssuewithoutBomDAL.SaveIssueWithoutBom(model, MRGRid);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _IssuewithoutBomDAL.GetDashboardData(FromDate, Todate, Flag);
        }
        public async Task<IssueWOBomMainDashboard> GetSearchData(string REQNo, string ReqDate, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate)
        {
            return await _IssuewithoutBomDAL.GetSearchData(REQNo, ReqDate, IssueSlipNo, IssueDate, WorkCenter, YearCode, ReqYearCode, FromDate, ToDate);
        }

        public async Task<IssueWOBomMainDashboard> GetDetailData(string REQNo, string ReqDate, string PartCode, string Item_Name, string IssueSlipNo, string IssueDate, string WorkCenter, string YearCode, string ReqYearCode, string FromDate, string ToDate)
        {
            return await _IssuewithoutBomDAL.GetDetailData(REQNo, ReqDate, PartCode, Item_Name, IssueSlipNo, IssueDate, WorkCenter, YearCode, ReqYearCode, FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC,int ActualEntryBy,string EntryByMachine)
        {
            return await _IssuewithoutBomDAL.DeleteByID(ID, YC,ActualEntryBy,EntryByMachine);
        }
        public async Task<ResponseResult> GetDataForDelete(int ID, int YC)
        {
            return await _IssuewithoutBomDAL.GetDataForDelete(ID, YC);
        }
        public async Task<ResponseResult> CheckLastTransDate(long ItemCode, string BatchNO, string UniqBatchno)
        {
            return await _IssuewithoutBomDAL.CheckLastTransDate(ItemCode,BatchNO,UniqBatchno);
        }
        public async Task<ResponseResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate,string FinStartDate)
        {
            return await _IssuewithoutBomDAL.FillBatchUnique(ItemCode,YearCode,StoreName,BatchNo,IssuedDate, FinStartDate);
        }
        public async Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IssuewithoutBomDAL.FillLotandTotalStock(ItemCode,StoreId,TillDate,BatchNo,UniqBatchNo);
        }
        public async Task<ResponseResult> GetReqQtyForScan(string ReqNo, int ReqYearCode, string ReqDate, int ItemCode)
        {
            return await _IssuewithoutBomDAL.GetReqQtyForScan(ReqNo,ReqYearCode,ReqDate,ItemCode);
        }
        public async Task<ResponseResult> GetNewEntry(int YearCode)
        {
            return await _IssuewithoutBomDAL.GetNewEntry(YearCode);
        }
        public async Task<ResponseResult> GetAllowBatch()
        {
            return await _IssuewithoutBomDAL.GetAllowBatch();
        }
        public async Task<ResponseResult> FillProjectNo()
        {
            return await _IssuewithoutBomDAL.FillProjectNo();
        }

        public async Task<ResponseResult> FillStoreName()
        {
            return await _IssuewithoutBomDAL.FillStoreName();
        }
        public async Task<DataSet> FillEmployee(string Flag)
        {
            return await _IssuewithoutBomDAL.FillEmployee(Flag);
        }

        public async Task<ResponseResult> FillDept()
        {
            return await _IssuewithoutBomDAL.FillDept();
        }
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            return await _IssuewithoutBomDAL.GetIsStockable(ItemCode);
        }
        public async Task<ResponseResult> GetIssueScanFeature()
        {
            return await _IssuewithoutBomDAL.GetIssueScanFeature();
        }
        public async Task<ResponseResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IssuewithoutBomDAL.CheckStockBeforeSaving(ItemCode,StoreId,TillDate,BatchNo,UniqBatchNo);
        }
        public async Task<ResponseResult> CheckRequisitionBeforeSaving(string ReqNo, string ReqDate,int ItemCode)
        {
            return await _IssuewithoutBomDAL.CheckRequisitionBeforeSaving(ReqNo,ReqDate,ItemCode);
        }
        public async Task<ResponseResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode,string TransDate)
        {
            return await _IssuewithoutBomDAL.GetItemDetailFromUniqBatch(UniqBatchNo, YearCode);
         }
        public async Task<IssueWithoutBom> GetViewByID(int ID, int YearCode)
        {
            return await _IssuewithoutBomDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            return await _IssuewithoutBomDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
    }
}
