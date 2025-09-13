using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class CommonDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CommonDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@Date1", ParseFormattedDate(Date)));
                SqlParams.Add(new SqlParameter("@Date1Name", ""));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("ChkDateFallInFinyear", SqlParams);
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
