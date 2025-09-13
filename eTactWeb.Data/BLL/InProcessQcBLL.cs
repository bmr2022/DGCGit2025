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
    public class InProcessQcBLL : IInProcessQc
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly InProcessQcDAL _InProcessQcDAL;

        public InProcessQcBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _InProcessQcDAL = new InProcessQcDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<InProcessQc> GetViewByID(int ID, int YearCode)
        {
            return await _InProcessQcDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName)
        {
            return await _InProcessQcDAL.FillEntryId(Flag, YearCode, SPName);
        }
        public async Task<DataSet> BindEmpList(string Flag)
        {
            return await _InProcessQcDAL.BindEmpList(Flag);
        }
        public async Task<ResponseResult> SaveInprocessQc(InProcessQc model, DataTable InProcessQcGrid)
        {
            return await _InProcessQcDAL.SaveInprocessQc(model, InProcessQcGrid);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _InProcessQcDAL.GetDashboardData();
        }
        public async Task<InProcessDashboard> GetDashboardData(string FromDate, string ToDate, string QcSlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ProdSlipNo, string DashboardType)
        {
            return await _InProcessQcDAL.GetDashboardData(FromDate, ToDate, QcSlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ProdSlipNo, DashboardType);
        }
        public async Task<InProcessDashboard> GetDashboardDetailData(string FromDate, string ToDate)
        {
            return await _InProcessQcDAL.GetDashboardDetailData(FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate)
        {
            return await _InProcessQcDAL.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate);
        }
        public async Task<ResponseResult> FillTransferToWorkCenter()
        {
            return await _InProcessQcDAL.FillTransferToWorkCenter();
        }
        public async Task<ResponseResult> FillReworkreason()
        {
            return await _InProcessQcDAL.FillReworkreason();
        }
        public async Task<ResponseResult> FillRejectionreason()
        {
            return await _InProcessQcDAL.FillRejectionreason();
        }
        public async Task<ResponseResult> FillTransferToStore()
        {
            return await _InProcessQcDAL.FillTransferToStore();
        }
    }
}
