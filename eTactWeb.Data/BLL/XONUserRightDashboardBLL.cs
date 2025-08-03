using eTactWeb.Data.Common;
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
    public class XONUserRightDashboardBLL : IXONUserRightDashboardBLL
    {
        private XONUserRightDashboardDAL _XONUserRightDashboardDAL;
        private readonly IDataLogic _IDataLogic;

        public XONUserRightDashboardBLL(IConfiguration config, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _XONUserRightDashboardDAL = new XONUserRightDashboardDAL(config, iDataLogic,connectionStringService);
            _IDataLogic = iDataLogic;
        }
        public async Task<IList<TextValue>> GetUserList(string ShowAllUsers)
        {
            return await _XONUserRightDashboardDAL.GetUserList(ShowAllUsers);
        }
        public async Task<IList<TextValue>> GetDashboardName()
        {
            return await _XONUserRightDashboardDAL.GetDashboardName();
        }
        public async Task<ResponseResult> GetDashboardSubScreen(string DashboardName)
        {
            return await _XONUserRightDashboardDAL.GetDashboardSubScreen(DashboardName);
        }
        public async Task<ResponseResult> SaveUserRightDashboard(UserRightDashboardModel model, DataTable UserRightDashboardGrid)
        {
            return await _XONUserRightDashboardDAL.SaveUserRightDashboard(model, UserRightDashboardGrid);
        }
        public async Task<UserRightDashboardModel> GetSearchData(string EmpName, string UserName, string DashboardName, string DashboardSubScreen)
        {
            return await _XONUserRightDashboardDAL.GetSearchData(EmpName, UserName, DashboardName, DashboardSubScreen);
        }
    }
}
