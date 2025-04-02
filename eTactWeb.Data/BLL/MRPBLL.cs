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
    public class MRPBLL : IMRP
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly MRPDAL _MRPDAL;
        public MRPBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _MRPDAL = new MRPDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> PendingMRPData(PendingMRP model)
        {
            return await _MRPDAL.PendingMRPData(model);
        }
        public async Task<ResponseResult> GetStore()
        {
            return await _MRPDAL.GetStore();
        }
        public async Task<ResponseResult> GetWorkCenter()
        {
            return await _MRPDAL.GetWorkCenter();
        }
        public async Task<ResponseResult> IsCheckMonthWiseData(int Month, int YearCode)
        {
            return await _MRPDAL.IsCheckMonthWiseData(Month, YearCode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _MRPDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> GetFormRights(int UserID)
        {
            return await _MRPDAL.GetFormRights(UserID);
        }
        public async Task<ResponseResult> GetDashboardData(MRPDashboard model)
        {
            return await _MRPDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string MRPNo, string EntryByMachineName, int CreatedByEmpId)
        {
            return await _MRPDAL.DeleteByID(ID, YC, MRPNo, EntryByMachineName, CreatedByEmpId);
        }
        public async Task<ResponseResult> SaveMRPDetail(MRPMain model, DataTable MRPGrid, DataTable MRPSOGrid, DataTable MRPFGGrid)
        {
            return await _MRPDAL.SaveMRPDetail(model, MRPGrid, MRPSOGrid, MRPFGGrid);
        }
        public async Task<MRPMain> GetMRPDetailData(PendingMRP model)
        {
            return await _MRPDAL.GetMRPDetailData(model);
        }
        public async Task<MRPMain> GetMRPFGRMDetailData(PendingMRP model)
        {
            return await _MRPDAL.GetMRPFGRMDetailData(model);
        }
        public async Task<MRPMain> GetViewByID(int ID, string Mode, int YC)
        {
            return await _MRPDAL.GetViewByID(ID, Mode, YC);
        }

    }
}

