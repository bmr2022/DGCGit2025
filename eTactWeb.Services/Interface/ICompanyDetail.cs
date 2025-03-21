using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICompanyDetail
    {
        public Task<ResponseResult> GetDashboardData();
        public Task<CompanyDetailModel> GetDashboardDetailData();
        Task<ResponseResult> SaveCompanyDetail(CompanyDetailModel model);
    }
}
