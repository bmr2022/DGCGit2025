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
    public class AssetsMasterBLL:IAssetsMaster
    {
        private AssetsMasterDAL _AssetsMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AssetsMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _AssetsMasterDAL = new AssetsMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
		public async Task<ResponseResult> FillItemName()
		{
			return await _AssetsMasterDAL.FillItemName();
		}
		public async Task<ResponseResult> FillEntryId(int YearCode, string EntryDate)
		{
			return await _AssetsMasterDAL.FillEntryId( YearCode,  EntryDate);
		}
        public async Task<ResponseResult> FillCostCenterName()
		{
			return await _AssetsMasterDAL.FillCostCenterName();
		}
        public async Task<ResponseResult> FillDepartmentName()
		{
			return await _AssetsMasterDAL.FillDepartmentName();
		}
		public async Task<ResponseResult> FillParentAccountName()
		{
			return await _AssetsMasterDAL.FillParentAccountName();
		}
        public async Task<ResponseResult> FillCategoryName()
		{
			return await _AssetsMasterDAL.FillCategoryName();
		}
		public async Task<ResponseResult> FillCustoidianEmpName()
		{
			return await _AssetsMasterDAL.FillCustoidianEmpName();
		}

		public async Task<ResponseResult> FillParentGoupDetail(int ParentAccountCode)
		{
			return await _AssetsMasterDAL.FillParentGoupDetail(ParentAccountCode);
		}
		public async Task<ResponseResult> SaveAssetsMaster(AssetsMasterModel model)
		{
			return await _AssetsMasterDAL.SaveAssetsMaster(model);
		}
        public async Task<ResponseResult> GetDashboardData(AssetsMasterModel model)
        {
            return await _AssetsMasterDAL.GetDashboardData(model);
        }
        public async Task<AssetsMasterModel> GetDashboardDetailData(string FromDate, string ToDate,string AssetsName)
        {
            return await _AssetsMasterDAL.GetDashboardDetailData(FromDate, ToDate, AssetsName);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int ActualEntryBy)
        {
            return await _AssetsMasterDAL.DeleteByID(EntryId, YearCode, EntryDate, ActualEntryBy);
        }
        public async Task<AssetsMasterModel> GetViewByID(int ID, int YC)
        {
            return await _AssetsMasterDAL.GetViewByID(ID, YC);
        }
    }
}
