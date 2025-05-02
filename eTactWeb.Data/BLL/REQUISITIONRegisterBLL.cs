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

namespace eTactWeb.Data.BLL
{
    public class REQUISITIONRegisterBLL : IREQUISITIONRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly REQUISITIONRegisterDAL _REQUISITIONRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public REQUISITIONRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _REQUISITIONRegisterDAL = new REQUISITIONRegisterDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }
        public async Task<REQUISITIONRegistermodel> GetREQUISITIONRegisterData(string Flag, string ReQType, string fromDate, string ToDate, string REQNo, string Partcode, string ItemName, string FromstoreId, string Toworkcenter, int ReqYearcode)
        {
            return await _REQUISITIONRegisterDAL.GetREQUISITIONRegisterData( Flag,  ReQType,  fromDate,  ToDate,  REQNo,  Partcode,  ItemName,  FromstoreId,  Toworkcenter,  ReqYearcode);

        }
    }
}
