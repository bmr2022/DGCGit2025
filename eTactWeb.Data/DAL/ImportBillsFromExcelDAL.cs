using eTactWeb.Data.Common;
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

namespace eTactWeb.Data.DAL
{
    public class ImportBillsFromExcelDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConnectionStringService _connectionStringService;
        private IDataReader? Reader;
        private readonly ICommon _common;

        public ImportBillsFromExcelDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService, ICommon common)
        {
            _IDataLogic = iDataLogic;
            _httpContextAccessor = httpContextAccessor;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _common = common;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        internal async Task<ResponseResult> ImportBills(string ReportType,int ForFinYear,int CreatedBy)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                
                
                    
                    SqlParams.Add(new SqlParameter("@ForFinYear", ForFinYear));
                    SqlParams.Add(new SqlParameter("@CreatedBy", CreatedBy));



                if (ReportType == "SALEREPORT")
                {
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("AccImportBillsdatafromExcelToTables", SqlParams);
                }
                if (ReportType == "PURCHASEREPORT")
                {
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("AccImportPurchaseBillsdatafromExcelToTables", SqlParams);
                }

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