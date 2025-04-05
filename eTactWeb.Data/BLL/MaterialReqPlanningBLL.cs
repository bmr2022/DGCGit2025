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
    public class MaterialReqPlanningBLL:IMaterialReqPlanning
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly MaterialReqPlanningDAL _MaterialReqPlanningDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaterialReqPlanningBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _MaterialReqPlanningDAL = new MaterialReqPlanningDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<ResponseResult> GetMRPNo(int YearCode)
        {
            return await _MaterialReqPlanningDAL.GetMRPNo(YearCode);
        }
        public async Task<ResponseResult> GetMonthList(int YearCode)
        {
            return await _MaterialReqPlanningDAL.GetMonthList( YearCode);
        }
        public async Task<MaterialReqPlanningModel> GetDetailData(string mrpno, string Month, int YearCode)
        {
            return await _MaterialReqPlanningDAL.GetDetailData( mrpno,  Month,  YearCode);
        }
    }
}
