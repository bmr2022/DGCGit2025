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
    public class RCRegisterBLL : IRCRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly RCRegisterDAL _RCRegisterDAL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RCRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _RCRegisterDAL = new RCRegisterDAL(configuration, iDataLogic, _httpContextAccessor);
        }
        public async Task<RCRegisterModel> GetRCRegisterData(string FromDate, string ToDate, string Partyname, string IssueChallanNo, string RecChallanNo, string PartCode, string ItemName, string IssueChallanType, string RGPNRGP, string ReportMode)
        {
            return await _RCRegisterDAL.GetRCRegisterData(FromDate, ToDate, Partyname, IssueChallanNo,RecChallanNo,PartCode, ItemName, IssueChallanType, RGPNRGP, ReportMode);
        }
    }
}
