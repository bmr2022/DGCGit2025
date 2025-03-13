using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CompanyDetailBLL:ICompanyDetail
    {
        private CompanyDetailDAL _CompanyDetailDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CompanyDetailBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _CompanyDetailDAL = new CompanyDetailDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> SaveCompanyDetail(CompanyDetailModel model)
        {
            return await _CompanyDetailDAL.SaveCompanyDetail(model);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _CompanyDetailDAL.GetDashboardData();
        }

        public async Task<CompanyDetailModel> GetDashboardDetailData()
        {
            return await _CompanyDetailDAL.GetDashboardDetailData();
        }
    }
}
