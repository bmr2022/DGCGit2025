using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class AdminModuleBLL : IAdminModule
{
    private AdminModuleDAL _AdminModuleDAL;
    private readonly IDataLogic _IDataLogic;

    public AdminModuleBLL(IConfiguration config, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        _AdminModuleDAL = new AdminModuleDAL(config,iDataLogic, connectionStringService);
        _IDataLogic = iDataLogic;
    }

    public ResponseResult DeleteRightByID(int ID)
    {
        return _AdminModuleDAL.DeleteRightByID(ID);
    }

    public Task<ResponseResult> DeleteUserByID(int ID)
    {
        return _AdminModuleDAL.DeleteUserByID(ID);
    }

    public async Task<IList<UserMasterModel>> GetDashBoardData(string Flag,string Usertype,string EmpCode,string EmpName,string UserName)
    {
        return await _AdminModuleDAL.GetDashBoardData(Flag,Usertype,EmpCode,EmpName,UserName);
    }

    public async Task<IList<TextValue>> GetMenuList(string Flag, string Module, string MainMenu)
    {
        return await _AdminModuleDAL.GetMenuList(Flag, Module, MainMenu);
    }
    public async Task<IList<TextValue>> GetUserList(string ShowAllUsers)
    {
        return await _AdminModuleDAL.GetUserList(ShowAllUsers);
    }
    public async Task<ResponseResult> CheckAdminExists()
    {
        return await _AdminModuleDAL.CheckAdminExists();
    }
    public async Task<ResponseResult> GetAllUserRights(int EmpID)
    {
        return await _AdminModuleDAL.GetAllUserRights(EmpID);
    }

    public async Task<UserMasterModel> GetUserByID(int ID)
    {
        return await _AdminModuleDAL.GetUserByID(ID);
    }

    public async Task<UserRightModel> GetUserRightByID(int ID)
    {
        return await _AdminModuleDAL.GetUserRightByID(ID);
    }
    public async Task<UserRightModel> GetSearchData(string EmpName,string UserName, string Module, string MainMenu)
    {
        return await _AdminModuleDAL.GetSearchData(EmpName,UserName, Module, MainMenu);
    }
    public async Task<UserRightModel> GetSearchData(int EmpID)
    {
        return await _AdminModuleDAL.GetSearchData(EmpID);
    }
    public async Task<UserRightModel> GetSearchDetailData(string EmpName,string UserName)
    {
        return await _AdminModuleDAL.GetSearchDetailData(EmpName,UserName);
    }

    public async Task<List<UserRightModel>> GetUserRightDashboard(string Flag)
    {
        return (await _AdminModuleDAL.GetUserRightDashboard(Flag)).ToList();
    }

    public async Task<ResponseResult> SaveUserMaster(UserMasterModel model)
    {
        return await _AdminModuleDAL.SaveUserMaster(model);
    }

    public async Task<ResponseResult> SaveUserRights(UserRightModel model,DataTable URGRid)
    {
        return await _AdminModuleDAL.SaveUserRights(model,URGRid);
    }

    public async Task<ResponseResult> GetUserCount()
    {
        return await _AdminModuleDAL.GetUserCount();
    }

}