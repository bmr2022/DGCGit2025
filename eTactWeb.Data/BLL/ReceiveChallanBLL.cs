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
    public class ReceiveChallanBLL : IReceiveChallan
    {
        public ReceiveChallanBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _ReceiveChallanDAL = new ReceiveChallanDAL(configuration, iDataLogic,connectionStringService);
        }


        private ReceiveChallanDAL? _ReceiveChallanDAL { get; }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _ReceiveChallanDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _ReceiveChallanDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> GetMRNNo(int yearcode,string FromDate, string ToDate)
        {
            return await _ReceiveChallanDAL.GetMRNNo(yearcode,FromDate,ToDate);
        }
        public async Task<ResponseResult> GetGateNo(int yearcode, string FromDate, string ToDate)
        {
            return await _ReceiveChallanDAL.GetGateNo(yearcode,FromDate,ToDate);
        }
        public async Task<ResponseResult> GetMRNDetail(int EntryId)
        {
            return await _ReceiveChallanDAL.GetMRNDetail(EntryId);
        }
        public async Task<ResponseResult> GetGateDetail(string GateNo, int GateYc,int GateEntryId, string FromDate, string ToDate, string Flag)
        {
            return await _ReceiveChallanDAL.GetGateDetail(GateNo,GateYc,GateEntryId,FromDate,ToDate,Flag);
        }
        public async Task<ResponseResult> GetMRNYear(string MRNNO)
        {
            return await _ReceiveChallanDAL.GetMRNYear(MRNNO);
        }
        public async Task<ResponseResult> GetMRNDate(string MRNNO,int MRNYC)
        {
            return await _ReceiveChallanDAL.GetMRNDate(MRNNO,MRNYC);
        }
        public async Task<ResponseResult> GetGateYear(string Gateno,string FromDate,string ToDate, int yearcode)
        {
            return await _ReceiveChallanDAL.GetGateYear(Gateno,FromDate,ToDate, yearcode);
        }
        public async Task<ResponseResult> GetGateDate(string Gateno,int GateYC)
        {
            return await _ReceiveChallanDAL.GetGateDate(Gateno,GateYC);
        }
        public async Task<ResponseResult> FillAlltDetail(string Gateno,int GateYC)
        {
            return await _ReceiveChallanDAL.FillAlltDetail(Gateno,GateYC);
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _ReceiveChallanDAL.FillStore();
        }
        public async Task<ResponseResult> SaveReceiveChallan(ReceiveChallanModel model,DataTable RCGrid)
        {
            return await _ReceiveChallanDAL.SaveReceiveChallan(model,RCGrid);
        }
        public async Task<ResponseResult> GetDashboardData(RCDashboard model)
        {
            return await _ReceiveChallanDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> DeleteByID(int ID,int YC)
        {
            return await _ReceiveChallanDAL.DeleteByID(ID,YC);
        }
        public async Task<ReceiveChallanModel> GetViewByID(int ID,int YC,string Mode)
        {
            return await _ReceiveChallanDAL.GetViewByID(ID,YC,Mode);
        }
        public async Task<ResponseResult> GetFeatureOption()
        {
            return await _ReceiveChallanDAL.GetFeatureOption();
        }
    }
}
