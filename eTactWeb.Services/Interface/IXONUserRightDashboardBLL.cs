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
        Task<List<UserRightDashboardModel>> GetUserRightDashboard(string Flag);
        Task<IList<UserRightDashboardModel>> GetDashBoardData(string Flag, string Usertype, string EmpCode, string EmpName, string UserName);
        Task<UserRightDashboardModel> GetSearchData(string EmpName, string UserName, string DashboardName, string DashboardSubScreen);
    }
}
