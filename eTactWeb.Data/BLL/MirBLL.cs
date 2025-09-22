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
    public class MirBLL :IMirModule
    {
        private readonly MirDAL _MIRDal;
        private readonly IDataLogic _DataLogicDAL;
        public MirBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _MIRDal = new MirDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<DataSet> BindBranch(string Flag)
        {
            return await _MIRDal.BindBranch(Flag);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _MIRDal.GetFormRights(ID);
        }
        public async Task<ResponseResult> GetOkRecStore(int ItemCode, string ShowAllStore,string GateNo)
        {
            return await _MIRDal.GetOkRecStore(ItemCode, ShowAllStore, GateNo);
        }
        public async Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName)
        {
            return await _MIRDal.GetNewEntry(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> AllowUpdelete(int EntryId, string YearCode)
        {
            return await _MIRDal.AllowUpdelete(EntryId, YearCode);
        }
        public async Task<ResponseResult> CheckEditOrDelete(int EntryId, int YearCode)
        {
            return await _MIRDal.CheckEditOrDelete(EntryId, YearCode);
        }
        public async Task<ResponseResult> GetMRNNo(string Flag, string SPName, string FromDate, string ToDate, string MRNCustJW)
        {
            return await _MIRDal.GetMRNNo(Flag, SPName, FromDate, ToDate, MRNCustJW);
        }

        public async Task<ResponseResult> AddPassWord()
        {
            return await _MIRDal.AddPassWord();
        }
        public async Task<ResponseResult> GetMRNData(string Flag, string SPName, string MRNNO, int MRNYearCode,int GateNo,int GateYear,int GateEntryId, string MRNCustJW)
        {
            return await _MIRDal.GetMRNData(Flag, SPName, MRNNO, MRNYearCode,GateNo,GateYear,GateEntryId,MRNCustJW);
        }
        public async Task<MirModel> GetMIRMainItem(string Flag, string SPName, string MRNNO, int MRNYearCode, int GateNo, int GateYear, int GateEntryId, string MRNNCustJW) 
        {
            return await _MIRDal.GetMIRMainItem(Flag, SPName, MRNNO, MRNYearCode, GateNo, GateYear, GateEntryId,MRNNCustJW);
        }
        public async Task<ResponseResult> GetMIRFromPend(string Flag, string SPName, string MRNNO, int MRNYearCode, string MRNNCustJW) 
        {
            return await _MIRDal.GetMIRFromPend(Flag, SPName, MRNNO, MRNYearCode, MRNNCustJW);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC)
        {
            return await _MIRDal.DeleteByID(ID, YC);
        }
        public async Task<ResponseResult> GetStore(string Flag, string SPName)
        {
            return await _MIRDal.GetStore(Flag, SPName);
        }
        public async Task<ResponseResult> GetRewStore(string Flag, string SPName)
        {
            return await _MIRDal.GetRewStore(Flag, SPName);
        }
        public async Task<ResponseResult> GetHoldStore(string Flag, string SPName)
        {
            return await _MIRDal.GetHoldStore(Flag, SPName);
        }
        public async Task<ResponseResult> GetRecOkStore(int ItemCode,string Flag, string SPName)
        {
            return await _MIRDal.GetRecOkStore(ItemCode,Flag, SPName);
        }
        public async Task<ResponseResult> GetGateData(string Flag, string SPName,string MRNNo,string MRNYearCode,string MRNCustJW)
        {
            return await _MIRDal.GetGateData(Flag, SPName,MRNNo,MRNYearCode, MRNCustJW);
        }

        public async Task<ResponseResult> GetEmployeeList(string Flag, string SPName)
        {
            return await _MIRDal.GetEmployeeList(Flag, SPName);
        }
        public async Task<ResponseResult> SaveMIR(MirModel model, DataTable MIRGRid)
        {
            return await _MIRDal.SaveMIR(model, MIRGRid);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _MIRDal.GetDashboardData();

        }
        public async Task<MIRQDashboard> GetSearchData(string VendorName, string MrnNo,string GateNo,string MirNo, string ItemName, string FromDate, string ToDate)
        {
            return await _MIRDal.GetSearchData(VendorName,MrnNo,GateNo, MirNo, ItemName,FromDate,ToDate);

        }
        public async Task<MIRQDashboard> GetDashboardDetailData(string VendorName, string MrnNo,string GateNo,string MirNo, string ItemName, string FromDate, string ToDate)
        {
            return await _MIRDal.GetDashboardDetailData(VendorName, MrnNo,GateNo,MirNo, ItemName, FromDate, ToDate);

        }
        public async Task<ResponseResult> GetSearchData(MIRQDashboard model)
        {
            return await _MIRDal.GetSearchData(model);
        }
        public async Task<MirModel> GetViewByID(int ID, int YearCode)
        {
            return await _MIRDal.GetViewByID(ID, YearCode);

        }
        public async Task<ResponseResult> GetReportName()
        {
            return await _MIRDal.GetReportName();

        }
        public async Task<ResponseResult> GenerateBarCodeTag(string MIRNo, int YearCode, string ItemCodes)
        {
            return await _MIRDal.GenerateBarCodeTag(MIRNo, YearCode, ItemCodes);
        }
    }
}
