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
    public class ImportBillsFromExcelBLL:IImportBillsFromExcel
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ImportBillsFromExcelDAL _ImportBillsFromExcelDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommon _common;
        public ImportBillsFromExcelBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService, ICommon common)
        {
            _DataLogicDAL = iDataLogic;
            _ImportBillsFromExcelDAL = new ImportBillsFromExcelDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService, common);
            _common = common;
        }

        public async Task<ResponseResult> ImportBills(string ReportType, int ForFinYear, int CreatedBy)
        {
            return await _ImportBillsFromExcelDAL.ImportBills( ReportType,  ForFinYear, CreatedBy);
        }
    }
}
