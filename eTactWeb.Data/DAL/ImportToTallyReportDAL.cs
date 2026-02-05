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
    public class ImportToTallyReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;
        private readonly ICommon _common;

        public ImportToTallyReportDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService, ICommon common)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _common = common;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }


        public async Task<ResponseResult> GetReportData(string Flag, string FromDate, string Todate,int AccountCode,string BillNo)
        {
            var fromDt = CommonFunc.ParseFormattedDate(FromDate);
            var toDt = CommonFunc.ParseFormattedDate(Todate);

            var parameters = new Dictionary<string, object>
    {

       
        {"@fromDate", fromDt},
        { "@ToDate", toDt },
        { "@AccountCode", AccountCode },
        { "@BillNo", BillNo },
       
    };

            return await _common.GetDashboardData(
                "AccSPExportToTallyFormat",
                Flag,
                parameters
            );

        }

    }
}
