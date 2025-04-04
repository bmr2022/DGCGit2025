using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;

namespace eTactWeb.Data.BLL
{
    public class JobWorkReceiveBLL : IJobWorkReceive
    {
        private readonly JobWorkReceiveDAL _JobWorkReceiveDAL;
        private readonly IDataLogic _DataLogicDAL;
        public JobWorkReceiveBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _JobWorkReceiveDAL = new JobWorkReceiveDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<DataSet> BindBranch(string Flag)
        {
            return await _JobWorkReceiveDAL.BindBranch(Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _JobWorkReceiveDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> GetGateNo(string Flag, string SPName, string FromDate, string ToDate)
        {
            return await _JobWorkReceiveDAL.GetGateNo(Flag, SPName, FromDate, ToDate);
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            return await _JobWorkReceiveDAL.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
        }
        public async Task<ResponseResult> GetEmployeeList(string Flag, string SPName)
        {
            return await _JobWorkReceiveDAL.GetEmployeeList(Flag, SPName);
        }
        public async Task<ResponseResult> GetProcessList(string Flag, string SPName)
        {
            return await _JobWorkReceiveDAL.GetProcessList(Flag, SPName);
        }
        public async Task<ResponseResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            return await _JobWorkReceiveDAL.CheckEditOrDelete(MRNNo, YearCode);
        }
        public async Task<ResponseResult> GetFeatureOption(string Flag, string SPName)
        {
            return await _JobWorkReceiveDAL.GetFeatureOption(Flag, SPName);
        }
        public async Task<ResponseResult> GetProcessUnit(string Flag, string SPName)
        {
            return await _JobWorkReceiveDAL.GetProcessList(Flag, SPName);
        }

        public async Task<ResponseResult> GetGateMainData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            return await _JobWorkReceiveDAL.GetGateMainData(Flag, SPName, GateNo, GateYearCode, GateEntryId);
        }
        public async Task<ResponseResult> GetGateItemData(string Flag, string SPName, string GateNo, string GateYearCode, int GateEntryId)
        {
            return await _JobWorkReceiveDAL.GetGateItemData(Flag, SPName, GateNo, GateYearCode, GateEntryId);
        }

        public async Task<ResponseResult> GetPopUpChallanData(int AccountCode, int YearCode, string FromDate, string ToDate, int RecItemCode, int BomRevNo, string BomRevDate, string BOMIND, string BillChallanDate, string JobType, string ProdUnProd)
        {
            return await _JobWorkReceiveDAL.GetPopUpChallanData(AccountCode, YearCode, FromDate, ToDate, RecItemCode, BomRevNo, BomRevDate, BOMIND, BillChallanDate, JobType,ProdUnProd);
        }
        public async Task<ResponseResult> GetPopUpData(string Flag,int AccountCode, int IssYear, string FinYearFromDate, string billchallandate, string prodUnProd, string BOMINd, int RMItemCode, string RMPartcode, string RMItemNAme, string ACCOUNTNAME, int Processid)
        {
            return await _JobWorkReceiveDAL.GetPopUpData(Flag, AccountCode, IssYear,FinYearFromDate,billchallandate,prodUnProd,BOMINd,RMItemCode,RMPartcode,RMItemNAme,ACCOUNTNAME,Processid);
        }
        public async Task<ResponseResult> GetAdjustedChallan(int AccountCode, int YearCode, string FinYearFromDate, string billchallandate, string GateNo, int GateYearCode, DataTable DTTItemGrid)
        {
            return await _JobWorkReceiveDAL.GetAdjustedChallan(AccountCode, YearCode, FinYearFromDate, billchallandate, GateNo, GateYearCode, DTTItemGrid);
        }
        public async Task<ResponseResult> SaveJobReceive(JobWorkReceiveModel model, DataTable JWRGrid, DataTable ChallanGrid)
        {
            return await _JobWorkReceiveDAL.SaveJobReceive(model, JWRGrid, ChallanGrid);
        }
        public async Task<ResponseResult> GetBomRevNo(int ItemCode)
        {
            return await _JobWorkReceiveDAL.GetBomRevNo(ItemCode);
        }
        public async Task<ResponseResult> ViewDetailSection(int yearCode, int entryId)
        {
            return await _JobWorkReceiveDAL.ViewDetailSection(yearCode,entryId);
        }
        public async Task<ResponseResult> GetBomValidated(int RecItemCode, int BomRevNo, string BomRevDate, int RecQty)
        {
            return await _JobWorkReceiveDAL.GetBomValidated(RecItemCode, BomRevNo, BomRevDate, RecQty);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string Todate, string Flag)
        {
            return await _JobWorkReceiveDAL.GetDashboardData(FromDate, Todate, Flag);

            //throw new NotImplementedException();
        }

        public async Task<JWReceiveDashboard> GetDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName,string BranchName, string InvNo, string Fromdate, string Todate)
        {
            return await _JobWorkReceiveDAL.GetDashboardData(VendorName, MRNNo, GateNo, PartCode, ItemName,BranchName, InvNo,Fromdate,Todate);
        }
        public async Task<JWReceiveDashboard> GetDetailDashboardData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName,string BranchName, string InvNo, string Fromdate, string Todate)
        {
            return await _JobWorkReceiveDAL.GetDetailDashboardData(VendorName, MRNNo, GateNo, PartCode, ItemName,BranchName, InvNo,Fromdate,Todate);
        }

        public async Task<JobWorkReceiveModel> GetViewByID(int ID, int YearCode)
        {
            return await _JobWorkReceiveDAL.GetViewByID(ID, YearCode);

        }

        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _JobWorkReceiveDAL.DeleteByID(ID, YC);
        }

        public async Task<ResponseResult> FillNewEntry(int YearCode)
        {
            return await _JobWorkReceiveDAL.FillNewEntry(YearCode);
        }
    }
}
