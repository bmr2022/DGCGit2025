using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.BLL
{
    public class VendJWRegisterBLL : IVendJWRegister
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly VendJWRegisterDal _VendJWRegisterDal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VendJWRegisterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _VendJWRegisterDal = new VendJWRegisterDal(configuration, iDataLogic, _httpContextAccessor);
        }
        public async Task<VendJWRegisterModel> GetJWRegisterData(string FromDate, string ToDate,string RecChallanNo,string IssChallanNo, string PartyName ,string PartCode, string ItemName, string IssueChallanType,string ReportMode)
        {
            return await _VendJWRegisterDal.GetJWRegisterData(FromDate, ToDate,RecChallanNo,IssChallanNo, PartyName, PartCode, ItemName, IssueChallanType, ReportMode);

        }

        //public async Task<RCRegisterModel> GetRCRegisterData(string FromDate, string ToDate, string Partyname, string IssueChallanNo, string RecChallanNo, string PartCode, string ItemName, string IssueChallanType, string RGPNRGP, string ReportMode)
        //{
        //    return await _RCRegisterDAL.GetRCRegisterData(FromDate, ToDate, Partyname, IssueChallanNo, RecChallanNo, PartCode, ItemName, IssueChallanType, RGPNRGP, ReportMode);
        //} 
    }
}
