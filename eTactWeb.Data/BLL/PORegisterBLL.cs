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
    public class PORegisterBLL : IPORegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly PORegisterDAL _PORegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PORegisterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _PORegisterDAL = new PORegisterDAL(configuration, iDataLogic, _httpContextAccessor, connectionStringService);
        }

        public async Task<PORegisterModel> GetPORegisterData(string FromDate, string ToDate, string ReportType, int YearCode, string Partyname, string partcode, string itemName, string POno, string SchNo, string OrderType, string POFor, string ItemType, string ItemGroup, string showOnlyCompletedPO, string showClosedPO, string showOnlyActivePO)
        {
            return await _PORegisterDAL.GetPORegisterData(FromDate, ToDate, ReportType, YearCode, Partyname, partcode, itemName, POno, SchNo, OrderType, POFor, ItemType, ItemGroup, showOnlyCompletedPO,  showClosedPO,  showOnlyActivePO);
        }
    }
}
