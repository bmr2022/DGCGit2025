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
	public class Features_OptionsBLL : IFeatures_Options
	{
		private Features_OptionsDAL _Features_OptionsDAL;
		private readonly IDataLogic _DataLogicDAL;

		public Features_OptionsBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_Features_OptionsDAL = new Features_OptionsDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<ResponseResult> GetDashboardData()
		{
			return await _Features_OptionsDAL.GetDashboardData();
		}
		public async Task<Features_OptionsModel> GetDashboardDetailData(string Type)
		{
			return await _Features_OptionsDAL.GetDashboardDetailData(Type);
		}
		public async Task<Features_OptionsModel> GetViewByID(string Type, int ActualEntryBy, string MachineName, string IPAddress)
		{
			return await _Features_OptionsDAL.GetViewByID(Type, ActualEntryBy, MachineName, IPAddress);
		}
		public async Task<ResponseResult> SaveFeatures_Options(Features_OptionsModel model)
		{
			return await _Features_OptionsDAL.SaveFeatures_Options(model);
		}
		public async Task<ResponseResult> GetFormRights(int ID)
		{
			return await _Features_OptionsDAL.GetFormRights(ID);
		}

		public async Task<ResponseResult> SaveFeatures_OptionsForProduction(Features_OptionsModel model)
		{
			return await _Features_OptionsDAL.SaveFeatures_OptionsForProduction(model);
		}
		public async Task<Features_OptionsModel> GetViewByIDForProduction(string Type, int ActualEntryBy, string MachineName, string IPAddress)
		{
			return await _Features_OptionsDAL.GetViewByIDForProduction(Type, ActualEntryBy, MachineName, IPAddress);
		}
		public async Task<ResponseResult> SaveFeatures_OptionsForAccounts(Features_OptionsModel model)
		{
			return await _Features_OptionsDAL.SaveFeatures_OptionsForAccounts(model);
		}
		public async Task<Features_OptionsModel> GetViewByIDForAccounts(string Type, int ActualEntryBy, string MachineName, string IPAddress)
		{
			return await _Features_OptionsDAL.GetViewByIDForAccounts(Type, ActualEntryBy, MachineName, IPAddress);
		}
	}
}
