using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class MaterialReqPlanningDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;

        private IDataReader? Reader;

        public MaterialReqPlanningDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        public async Task<ResponseResult> GetMRPNo(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "MRPNO"));
                SqlParams.Add(new SqlParameter("@YEaR_CODE", YearCode));
                


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<ResponseResult> GetMonthList( )
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "MONTHLIST"));
              



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMRPREport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
    }
}
