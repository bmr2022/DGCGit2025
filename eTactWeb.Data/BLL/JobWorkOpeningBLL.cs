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
    public class JobWorkOpeningBLL : IJobWorkOpening
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly JobWorkOpeningDAL _JobWorkOpeningDAL;


        public JobWorkOpeningBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _JobWorkOpeningDAL = new JobWorkOpeningDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _JobWorkOpeningDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            return await _JobWorkOpeningDAL.GetItemCode(PartCode);
        }

        public async Task<ResponseResult> FillEntryId(string Flag, int YearCode,string FormTypeCustJWNRGP, string SPName)
        {
            return await _JobWorkOpeningDAL.FillEntryId(Flag, YearCode, FormTypeCustJWNRGP, SPName);
        }
        public async Task<JobWorkOpeningModel> GetViewByID(int ID, string Mode, int YC, string OpeningType)
        {
            return await _JobWorkOpeningDAL.GetViewByID(ID, Mode, YC, OpeningType);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string SumDetail, int Itemcode, string ChallanNo, string OpeningType, int ActualEntryById, string MachineName, int AccountCode, string Partcode, string ItemName)
        {
            return await _JobWorkOpeningDAL.DeleteByID(ID, YC, SumDetail, Itemcode, ChallanNo,OpeningType, ActualEntryById, MachineName, AccountCode,Partcode,ItemName);
        }
        public async Task<ResponseResult> FillItemPartCode()
        {
            return await _JobWorkOpeningDAL.FillItemPartCode();
        }
        public async Task<ResponseResult> FillBomNo(int ItemCode, int RecItemCode, string BomType)
        {
            return await _JobWorkOpeningDAL.FillBomNo(ItemCode, RecItemCode, BomType);
        }
        public async Task<ResponseResult> SaveJobWorkOpening(JobWorkOpeningModel model, DataTable GIGrid)
        {
            return await _JobWorkOpeningDAL.SaveJobWorkOpening(model, GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData(JobWorkOpeningDashboard model)
        {
            return await _JobWorkOpeningDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> GetDetailData(JobWorkOpeningDashboard model)
        {
            return await _JobWorkOpeningDAL.GetDetailData(model);
        }
        public async Task<ResponseResult> FillPartyName()
        {
            return await _JobWorkOpeningDAL.FillPartyName();
        }
        public async Task<ResponseResult> FillUnitAltUnit(int ItemCode)
        {
            return await _JobWorkOpeningDAL.FillUnitAltUnit(ItemCode);
        }
        public async Task<ResponseResult> FillProcessName()
        {
            return await _JobWorkOpeningDAL.FillProcessName();
        }
        public async Task<ResponseResult> FillRecItemPartCode(string BOMIND, int ItemCode)
        {
            return await _JobWorkOpeningDAL.FillRecItemPartCode(BOMIND, ItemCode);
        }
        public async Task<ResponseResult> FillScrapItemPartCode(string BOMIND, int ItemCode)
        {
            return await _JobWorkOpeningDAL.FillScrapItemPartCode(BOMIND, ItemCode);
        }
    }
}
