using eTactWeb.DOM.Models;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAdminModule
    {
        ResponseResult DeleteRightByID(int ID);

        ResponseResult DeleteUserByID(int ID);

        Task<IList<UserMasterModel>> GetDashBoardData(string Flag,string Usertype,string EmpCode, string EmpName,string UserName);

        Task<IList<TextValue>> GetMenuList(string Flag, string Module, string MainMenu);
        Task<IList<TextValue>> GetUserList(string ShowAllUsers);
        Task<ResponseResult> CheckAdminExists();
        Task<ResponseResult> GetAllUserRights(int EmpID);
        Task<UserMasterModel> GetUserByID(int ID);

        Task<UserRightModel> GetUserRightByID(int ID);

        Task<List<UserRightModel>> GetUserRightDashboard(string Flag);

        Task<ResponseResult> SaveUserMaster(UserMasterModel model);

        Task<ResponseResult> SaveUserRights(UserRightModel model, DataTable URGrid);
        Task<UserRightModel> GetSearchData(string EmpID,string UserName, string Module, string MainMenu);
        Task<UserRightModel> GetSearchData(int EmpID);
        Task<UserRightModel> GetSearchDetailData(string EmpName,string UserName);
        Task<ResponseResult> GetUserCount();
    }
}