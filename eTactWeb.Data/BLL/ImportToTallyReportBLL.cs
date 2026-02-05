using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    public class ImportToTallyReportBLL: IImportToTallyReport
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ImportToTallyReportDAL _ImportToTallyReportDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommon _common;
        public ImportToTallyReportBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService, ICommon common)
        {
            _DataLogicDAL = iDataLogic;
            _ImportToTallyReportDAL = new ImportToTallyReportDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService, common);
            _common = common;
        }
        public async Task<ResponseResult> GetReportData(string Flag, string FromDate, string Todate ,int AccountCode,string BillNo)
        {
            return await _ImportToTallyReportDAL.GetReportData(Flag, FromDate, Todate, AccountCode, BillNo);
        }
    }
}
