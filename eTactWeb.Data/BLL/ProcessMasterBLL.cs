using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;
public class ProcessMasterBLL : IProcessMaster
{
    private readonly ProcessMasterDAL _ProcessMasterDAL;
    private readonly IDataLogic _DataLogicDAL;

    public ProcessMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
    {
        _ProcessMasterDAL = new ProcessMasterDAL(config, dataLogicDAL);
        _DataLogicDAL = dataLogicDAL;
    }
    public Task<ResponseResult> CheckBeforeUpdate(int Type)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseResult> DeleteByID(int ID)
    {
        return await _ProcessMasterDAL.DeleteByID(ID);
    }

    public Task<ProcessMasterModel> GetByID(int ID)
    {
        return _ProcessMasterDAL.GetByID(ID);
    }

    public Task<ProcessMasterModel> GetDashboardData(ProcessMasterModel model)
    {
        return _ProcessMasterDAL.GetDashboardData(model);
    }

    public Task<IList<DOM.Models.Common.TextValue>> GetDropDownList(string Flag)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseResult> GetFormRights(int userId)
    {
        return await _ProcessMasterDAL.GetFormRights(userId);
    }

    public async Task<ProcessMasterModel> GetSearchData(ProcessMasterModel model, string ProcessShortName, string ProcessCode)
    {
        return await _ProcessMasterDAL.GetSearchData(model, ProcessShortName, ProcessCode);
    }

    public Task<ResponseResult> SaveProcessMasterMaster(ProcessMasterModel model)
    {
        return _ProcessMasterDAL.SaveProcessMaster(model);
    }
}
