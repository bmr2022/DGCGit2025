using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public  class ScheduleCalibrationBLL:IScheduleCalibration
    {
		private readonly IDataLogic _DataLogicDAL;
		private readonly ScheduleCalibrationDAL _ScheduleCalibrationDAL;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ScheduleCalibrationBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_DataLogicDAL = iDataLogic;
			_ScheduleCalibrationDAL = new ScheduleCalibrationDAL(configuration, iDataLogic,  connectionStringService);
		}
		public async Task<PendingScheduleCalibrationModel> GetScheduleCalibrationSearchData(string PartCode, string ItemName, string ToolCode, string ToolName)
		{
			return await _ScheduleCalibrationDAL.GetScheduleCalibrationSearchData( PartCode,  ItemName,  ToolCode,  ToolName);
		}
        public async Task<ResponseResult> GetCalibrationAgency()
        {
            return await _ScheduleCalibrationDAL.GetCalibrationAgency();
        }
    }
}
