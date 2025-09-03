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
    public class AccDepriciationCalculationdetailBLL: IAccDepriciationCalculationdetail
    {
		private AccDepriciationCalculationdetailDAL _AccDepriciationCalculationdetailDAL;
		private readonly IDataLogic _DataLogicDAL;

		public AccDepriciationCalculationdetailBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_AccDepriciationCalculationdetailDAL = new AccDepriciationCalculationdetailDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<AccDepriciationCalculationdetailModel> GetAssets(int DepriciationYearCode, string AssetsName, string DepreciationMethod, string AssetsCategoryName)
		{
			return await _AccDepriciationCalculationdetailDAL.GetAssets(DepriciationYearCode,  AssetsName,  DepreciationMethod,  AssetsCategoryName);
		}
		public async Task<ResponseResult> SaveDepriciationCalculationdetail(AccDepriciationCalculationdetailModel model, DataTable GIGrid)
		{
			return await _AccDepriciationCalculationdetailDAL.SaveDepriciationCalculationdetail(model, GIGrid);
		}
		public async Task<ResponseResult> FillEntryID(string EntryDate, int YearCode)
		{
			return await _AccDepriciationCalculationdetailDAL.FillEntryID(EntryDate,YearCode);
		}
        public async Task<ResponseResult> GetDashboardData(AccDepriciationCalculationdetailModel model)
        {
            return await _AccDepriciationCalculationdetailDAL.GetDashboardData(model);
        }
        public async Task<AccDepriciationCalculationdetailModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _AccDepriciationCalculationdetailDAL.GetDashboardDetailData(FromDate, ToDate, ReportType);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode)
        {
            return await _AccDepriciationCalculationdetailDAL.DeleteByID(EntryId, YearCode);
        }
        public async Task<AccDepriciationCalculationdetailModel> GetViewByID(int ID, int YC)
        {
            return await _AccDepriciationCalculationdetailDAL.GetViewByID(ID,YC);
        }
    }
}
