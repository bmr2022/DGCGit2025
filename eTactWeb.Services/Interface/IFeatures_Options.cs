using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
	public interface IFeatures_Options
	{
		public Task<ResponseResult> GetDashboardData();
		public Task<Features_OptionsModel> GetDashboardDetailData(string Type);
		public Task<Features_OptionsModel> GetViewByID(string Type, int ActualEntryBy, string MachineName, string IPAddress);
		public Task<ResponseResult> SaveFeatures_Options(Features_OptionsModel model);
		Task<ResponseResult> GetFormRights(int uId);
		public Task<ResponseResult> SaveFeatures_OptionsForProduction(Features_OptionsModel model);
		public Task<Features_OptionsModel> GetViewByIDForProduction(string Type, int ActualEntryBy, string MachineName, string IPAddress);
		public Task<ResponseResult> SaveFeatures_OptionsForAccounts(Features_OptionsModel model);
		public Task<Features_OptionsModel> GetViewByIDForAccounts(string Type, int ActualEntryBy, string MachineName, string IPAddress);
	}
}
