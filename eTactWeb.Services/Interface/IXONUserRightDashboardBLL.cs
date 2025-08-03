using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IXONUserRightDashboardBLL
    {
        Task<IList<TextValue>> GetUserList(string ShowAllUsers);
        Task<IList<TextValue>> GetDashboardName();
        Task<ResponseResult> GetDashboardSubScreen(string DashboardName);
        Task<ResponseResult> SaveUserRightDashboard(UserRightDashboardModel model, DataTable UserRightDashboardGrid);
        Task<UserRightDashboardModel> GetSearchData(string EmpName, string UserName, string DashboardName, string DashboardSubScreen);
    }
}
