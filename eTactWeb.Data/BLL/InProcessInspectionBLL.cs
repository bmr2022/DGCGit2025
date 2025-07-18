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
    public  class InProcessInspectionBLL:IInProcessInspection
    {
        private InProcessInspectionDAL _InProcessInspectionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public InProcessInspectionBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _InProcessInspectionDAL = new InProcessInspectionDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
		public async Task<ResponseResult> FillPartCode(string InspectionType)
		{
            return await _InProcessInspectionDAL.FillPartCode( InspectionType);
		}
        public async Task<ResponseResult> FillItemName()
		{
            return await _InProcessInspectionDAL.FillItemName();
		}
        public async Task<ResponseResult> FillMachineName()
		{
            return await _InProcessInspectionDAL.FillMachineName();
		}
        public async Task<ResponseResult> FillCustomer()
		{
            return await _InProcessInspectionDAL.FillCustomer();
		}
         public async Task<ResponseResult> FillShift()
		{
            return await _InProcessInspectionDAL.FillShift();
		}

         public async Task<ResponseResult> FillColor(string PartNo)
		{
            return await _InProcessInspectionDAL.FillColor(PartNo);
		}
		public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			return await _InProcessInspectionDAL.FillEntryID(YearCode);
		}
		public async Task<InProcessInspectionModel> GetInprocessInspectionGridData(int ItemCode, int SampleSize)
		{
			return await _InProcessInspectionDAL.GetInprocessInspectionGridData( ItemCode,SampleSize);
		}
        public async Task<ResponseResult> SaveInprocessInspection(InProcessInspectionModel model, DataTable GIGrid)
        {
            return await _InProcessInspectionDAL.SaveInprocessInspection(model, GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData(InProcessInspectionModel model)
        {
            return await _InProcessInspectionDAL.GetDashboardData(model);
        }
        public async Task<InProcessInspectionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _InProcessInspectionDAL.GetDashboardDetailData(FromDate, ToDate, ReportType);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            return await _InProcessInspectionDAL.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);
        }
        public async Task<InProcessInspectionModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            return await _InProcessInspectionDAL.GetViewByID(ID, YC, FromDate, ToDate);
        }
    }
}
