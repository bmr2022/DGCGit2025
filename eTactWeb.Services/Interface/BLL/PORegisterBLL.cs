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

        public PORegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _PORegisterDAL = new PORegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }

        public async Task<PORegisterModel> GetPORegisterData(string FromDate, string ToDate, string ReportType, int YearCode, string Partyname, string partcode, string itemName, string POno, string SchNo, string OrderType, string POFor, string ItemType, string ItemGroup)
        {
            return await _PORegisterDAL.GetPORegisterData(FromDate, ToDate, ReportType, YearCode, Partyname, partcode, itemName, POno, SchNo, OrderType, POFor, ItemType, ItemGroup);
        }
    }
}
