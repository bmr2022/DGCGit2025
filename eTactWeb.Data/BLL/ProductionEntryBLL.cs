using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class ProductionEntryBLL : IProductionEntry
    {
        //private readonly PurchaseOrderDAL _PurchaseOrderDAL;
        private readonly IDataLogic _DataLogicDAL;
        private readonly ProductionEntryDAL _ProductionEntryDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductionEntryBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //_PurchaseOrderDAL = new PurchaseOrderDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
            _ProductionEntryDAL = new ProductionEntryDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<PendingProductionEntryModel> GetPendingProductionEntry(int Yearcode)
        {
            return await _ProductionEntryDAL.GetPendingProductionEntry(Yearcode);
        }
        public async Task<ResponseResult> CheckFeatureOption()
        {
            return await _ProductionEntryDAL.CheckFeatureOption();
        }
        public async Task<ResponseResult> CCEnableDisable()
        {
            return await _ProductionEntryDAL.CCEnableDisable();
        }
        public async Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType)
        {
            return await _ProductionEntryDAL.CheckDuplicateEntry(YearCode, AccountCode, InvNo, DocType);
        }
        public async Task<ResponseResult> CheckEditOrDelete(string ProdSlipNo, int ProdYearCode)
        {
            return await _ProductionEntryDAL.CheckEditOrDelete(ProdSlipNo, ProdYearCode);
        }
        public async Task<ResponseResult> GetPoNumberDropDownList(string Flag, string ServiceType, string SPName, string AccountCode, int Year, int DocTypeId)
        {
            return await _ProductionEntryDAL.GetPoNumberDropDownList(Flag, ServiceType, SPName, AccountCode, Year, DocTypeId);
        }
        public async Task<ResponseResult> FillSaleBillChallan(int AccountCode, int doctype, int ItemCode)
        {
            return await _ProductionEntryDAL.FillSaleBillChallan(AccountCode, doctype, ItemCode);
        }
        public async Task<ResponseResult> FillChallanQty(int AccountCode, int ItemCode, string ChallanNo)
        {
            return await _ProductionEntryDAL.FillChallanQty(AccountCode, ItemCode, ChallanNo);
        }
        public async Task<ResponseResult> FillSaleBillQty(int AccountCode, int ItemCode, int SaleBillNo)
        {
            return await _ProductionEntryDAL.FillSaleBillQty(AccountCode, ItemCode, SaleBillNo);
        }
        public async Task<ResponseResult> AltUnitConversion(int ItemCode, int AltQty, int UnitQty)
        {
            return await _ProductionEntryDAL.AltUnitConversion(ItemCode, AltQty, UnitQty);
        }
        public async Task<ProductionEntryModel> GetChildData(string Flag, string SPName, int WcId, int YearCode, float ProdQty, int ItemCode, string ProdDate, int BomNo)
        {
            return await _ProductionEntryDAL.GetChildData(Flag, SPName, WcId, YearCode, ProdQty, ItemCode, ProdDate, BomNo);
        }
        public async Task<ProductionEntryDashboard> GetDashboardData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            return await _ProductionEntryDAL.GetDashboardData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
        }
        public async Task<ProductionEntryDashboard> GetDashboardDetailData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            return await _ProductionEntryDAL.GetDashboardDetailData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
        }
        public async Task<ProductionEntryDashboard> GetBatchwiseDetail(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            return await _ProductionEntryDAL.GetBatchwiseDetail(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
        }
        public async Task<ProductionEntryDashboard> GetBreakdownData(string FromDate, string ToDate)
        {
            return await _ProductionEntryDAL.GetBreakdownData(FromDate, ToDate);
        }
        public async Task<ProductionEntryDashboard> GetOperationData(string FromDate, string ToDate)
        {
            return await _ProductionEntryDAL.GetOperationData(FromDate, ToDate);
        }
        public async Task<ProductionEntryDashboard> GetScrapData(string FromDate, string ToDate)
        {
            return await _ProductionEntryDAL.GetScrapData(FromDate, ToDate);
        }
        public async Task<ProductionEntryDashboard> GetProductData(string FromDate, string ToDate)
        {
            return await _ProductionEntryDAL.GetProductData(FromDate, ToDate);
        }
        public async Task<ResponseResult> GetPopUpData(string Flag, int AccountCode, string PONO)
        {
            return await _ProductionEntryDAL.GetPopUpData(Flag, AccountCode, PONO);
        }
        public async Task<ResponseResult> GetScheDuleByYearCodeandAccountCode(string Flag, string AccountCode, string YearCode, string poNo)
        {
            return await _ProductionEntryDAL.GetScheDuleByYearCodeandAccountCode(Flag, AccountCode, YearCode, poNo);
        }
        public async Task<ResponseResult> GetFeatureOption(string Flag, string SPName)
        {
            return await _ProductionEntryDAL.GetFeatureOption(Flag, SPName);
        }
        public async Task<ResponseResult> SaveProductionEntry(ProductionEntryModel model, DataTable GIGrid, DataTable OperatorGrid, DataTable BreakDownGrid, DataTable ScrapGrid, DataTable ProductGrid)
        {
            return await _ProductionEntryDAL.SaveProductionEntry(model, GIGrid, OperatorGrid, BreakDownGrid, ScrapGrid, ProductGrid);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate,int ActualEntryBy)
        {
            return await _ProductionEntryDAL.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate,ActualEntryBy);
        }
        public async Task<ResponseResult> FillEntryandGate(string Flag, int YearCode, string SPName)
        {
            return await _ProductionEntryDAL.FillEntryandGate(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> FillShift()
        {
            return await _ProductionEntryDAL.FillShift();
        }
        public async Task<ResponseResult> GetTimeDiff(string Flag, string ToTime, string DiffType, string FromTime)
        {
            return await _ProductionEntryDAL.GetTimeDiff(Flag,ToTime, DiffType, FromTime);
        }
        public async Task<ResponseResult> GetDateforBreakdown(string Flag, string DiffType, string QtyOfTime,  string FromTime)
        {
            return await _ProductionEntryDAL.GetDateforBreakdown(Flag, DiffType, QtyOfTime,FromTime);
        }
        public async Task<ResponseResult> FillShiftTime(int ShiftId)
        {
            return await _ProductionEntryDAL.FillShiftTime(ShiftId);
        }
        public async Task<ResponseResult> GetWorkCenterTotalStock(string Flag, int ItemCode, int WcId, string TillDate)
        {
            return await _ProductionEntryDAL.GetWorkCenterTotalStock(Flag, ItemCode, WcId, TillDate);
        }
        public async Task<ResponseResult> GetWorkCenterQty(string SPName, int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            return await _ProductionEntryDAL.GetWorkCenterQty(SPName, ItemCode, WcId, TillDate, BatchNo, UniqueBatchNo);
        }
        public async Task<ResponseResult> GetUnit(int RmItemCode)
        {
            return await _ProductionEntryDAL.GetUnit(RmItemCode);
        }
        public async Task<ProductionEntryModel> FillScrapData(int FGItemCode, decimal FgProdQty, string BomNo)
        {
            return await _ProductionEntryDAL.FillScrapData(FGItemCode,FgProdQty,BomNo);
        }
        public async Task<ResponseResult> GetScrapUnit(int ScrapItemCode)
        {
            return await _ProductionEntryDAL.GetScrapUnit(ScrapItemCode);
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _ProductionEntryDAL.FillStore();
        }
        public async Task<ResponseResult> CheckAllowToAddNegativeStock()
        {
            return await _ProductionEntryDAL.CheckAllowToAddNegativeStock();
        }
        public async Task<ResponseResult> FillTool()
        {
            return await _ProductionEntryDAL.FillTool();
        }
        public async Task<ResponseResult> FillMachineGroup()
        {
            return await _ProductionEntryDAL.FillMachineGroup();
        }
        public async Task<ResponseResult> FillMachineName()
        {
            return await _ProductionEntryDAL.FillMachineName();
        }
        public async Task<ResponseResult> FillSuperwiser()
        {
            return await _ProductionEntryDAL.FillSuperwiser();
        }
        public async Task<ResponseResult> FillOperator()
        {
            return await _ProductionEntryDAL.FillOperator();
        }
        public async Task<ResponseResult> FillOperatorName()
        {
            return await _ProductionEntryDAL.FillOperatorName();
        }
        public async Task<ResponseResult> FillBreakdownreason()
        {
            return await _ProductionEntryDAL.FillBreakdownreason();
        }
        public async Task<ResponseResult> FillResponsibleEmp()
        {
            return await _ProductionEntryDAL.FillResponsibleEmp();
        }
        public async Task<ResponseResult> FillScrapItems()
        {
            return await _ProductionEntryDAL.FillScrapItems();
        }
        public async Task<ResponseResult> FillResFactor()
        {
            return await _ProductionEntryDAL.FillResFactor();
        }
        public async Task<ResponseResult> FillScrapPartCode()
        {
            return await _ProductionEntryDAL.FillScrapPartCode();
        }
        public async Task<ResponseResult> FillScrapType()
        {
            return await _ProductionEntryDAL.FillScrapType();
        }
        public async Task<ResponseResult> FillProductItems(int FgItemCode, string BomNo)
        {
            return await _ProductionEntryDAL.FillProductItems(FgItemCode,BomNo);
        }
        public async Task<ResponseResult> FillProductPartCode(int FgItemCode, string BomNo)
        {
            return await _ProductionEntryDAL.FillProductPartCode(FgItemCode, BomNo);
        }
        public async Task<ResponseResult> FillProductType()
        {
            return await _ProductionEntryDAL.FillProductType();
        }
        public async Task<ResponseResult> FillProductUnit(int ProductItemCode)
        {
            return await _ProductionEntryDAL.FillProductUnit(ProductItemCode);
        }
        public async Task<ResponseResult> FillRMItemName()
        {
            return await _ProductionEntryDAL.FillRMItemName();
        }
        public async Task<ResponseResult> FillRMPartCode()
        {
            return await _ProductionEntryDAL.FillRMPartCode();
        }
        public async Task<ResponseResult> FillAltItemName()
        {
            return await _ProductionEntryDAL.FillAltItemName();
        }
        public async Task<ResponseResult> FillAltPartCode()
        {
            return await _ProductionEntryDAL.FillAltPartCode();
        }
        public async Task<ResponseResult> GetLastProddate(int YearCode)
        {
            return await _ProductionEntryDAL.GetLastProddate(YearCode);
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            return await _ProductionEntryDAL.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
        }
        public async Task<ResponseResult> DisplayRoutingDetail(int ItemCode)
        {
            return await _ProductionEntryDAL.DisplayRoutingDetail(ItemCode);
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
        {
            return await _ProductionEntryDAL.GetBatchNumber(SPName, ItemCode, YearCode, WcId, TransDate, BatchNo);
        }
        public async Task<ResponseResult> GetItems(string ProdAgainst, int YearCode)
        {
            return await _ProductionEntryDAL.GetItems(ProdAgainst, YearCode);
        }
        public async Task<ResponseResult> GetPartCode(string ProdAgainst, int YearCode)
        {
            return await _ProductionEntryDAL.GetPartCode(ProdAgainst, YearCode);
        }
        public async Task<ResponseResult> FillWorkcenter()
        {
            return await _ProductionEntryDAL.FillWorkcenter();
        }
        public async Task<ResponseResult> FillBomNo(int ItemCode)
        {
            return await _ProductionEntryDAL.FillBomNo(ItemCode);
        }
        public async Task<ResponseResult> GetProcessDetail(int ItemCode,int ProcessId,int WcId)
        {
            return await _ProductionEntryDAL.GetProcessDetail(ItemCode,ProcessId,WcId);
        }
        public async Task<ResponseResult> FillIssWorkcenter(string QcMandatory)
        {
            return await _ProductionEntryDAL.FillIssWorkcenter(QcMandatory);
        }
        public async Task<ResponseResult> FillIssWorkcenterForQcMandatory(string QcMandatory)
        {
            return await _ProductionEntryDAL.FillIssWorkcenterForQcMandatory(QcMandatory);
        }
        public async Task<ResponseResult> CheckForEditandDelete(string ProdSlipNo,int YearCode)
        {
            return await _ProductionEntryDAL.CheckForEditandDelete(ProdSlipNo,YearCode);
        }
        public async Task<ResponseResult> FillIssStore()
        {
            return await _ProductionEntryDAL.FillIssStore();
        }
        public async Task<ResponseResult> FillOperation(int ItemCode, int WcId)
        {
            return await _ProductionEntryDAL.FillOperation(ItemCode,WcId);
        }
        public async Task<ResponseResult> FillReqNo(string Fromdate, string ToDate, string ProdAgainst, int ItemCode)
        {
            return await _ProductionEntryDAL.FillReqNo(Fromdate, ToDate, ProdAgainst, ItemCode);
        }
        public async Task<ResponseResult> FillProdSchNo(string Fromdate, string ToDate, string ProdAgainst, int ItemCode)
        {
            return await _ProductionEntryDAL.FillProdSchNo(Fromdate, ToDate, ProdAgainst, ItemCode);
        }
        public async Task<ResponseResult> FillProdPlanDetail(string FromDate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdYearCode)
        {
            return await _ProductionEntryDAL.FillProdPlanDetail(FromDate, ToDate, ProdAgainst, ProdSchNo,ProdYearCode);
        }
        public async Task<ResponseResult> FillReqYear(string Fromdate, string ToDate, string ProdAgainst, string ReqNo)
        {
            return await _ProductionEntryDAL.FillReqYear(Fromdate, ToDate, ProdAgainst, ReqNo);
        }
        public async Task<ResponseResult> FillReqDate(string Fromdate, string ToDate, string ProdAgainst, string ReqNo, int ReqYearCode, int ItemCode)
        {
            return await _ProductionEntryDAL.FillReqDate(Fromdate, ToDate, ProdAgainst, ReqNo,ReqYearCode,ItemCode);
        }
        public async Task<ResponseResult> FillProdDate(string Fromdate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdPlanYearCode)
        {
            return await _ProductionEntryDAL.FillProdDate(Fromdate, ToDate, ProdAgainst, ProdSchNo,ProdPlanYearCode);
        }
        public async Task<ResponseResult> FillProdSchYear(string Fromdate, string ToDate, string ProdAgainst, string ProdSch)
        {
            return await _ProductionEntryDAL.FillProdSchYear(Fromdate, ToDate, ProdAgainst, ProdSch);
        }
        public async Task<ResponseResult> FillReqQty(string CurrentDate, string ProdAgainst, string ReqNo, int YearCode, int ItemCode, int ReqYearCode)
        {
            return await _ProductionEntryDAL.FillReqQty(CurrentDate, ProdAgainst, ReqNo, YearCode, ItemCode, ReqYearCode);
        }
        public async Task<ResponseResult> FillPendQty(string CurrentDate, string ProdAgainst, string ProdSchNo, int YearCode, int ItemCode, int ProdSchYear, int Entryid)
        {
            return await _ProductionEntryDAL.FillPendQty(CurrentDate, ProdAgainst, ProdSchNo, YearCode, ItemCode, ProdSchYear,Entryid);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _ProductionEntryDAL.GetDashboardData();
        }

        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _ProductionEntryDAL.GetFormRights(ID);
        }

        public async Task<ProductionEntryModel> GetViewByID(int ID, int YearCode)
        {
            return await _ProductionEntryDAL.GetViewByID(ID, YearCode);

        }
    }
}