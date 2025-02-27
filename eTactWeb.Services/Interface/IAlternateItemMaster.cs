using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAlternateItemMaster
    {
        Task<ResponseResult> GetMainItem();
        Task<ResponseResult> GetMainPartCode();
        Task<ResponseResult> GetAltPartCode(int MainItemcode);
        Task<ResponseResult> GetAltItemName(int MainItemcode);
        Task<ResponseResult> SaveAlternetItemMaster(AlternateItemMasterModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData();
        Task<AlternateItemMasterDashBoardModel> GetDashboardDetailData();
        Task<AlternateItemMasterModel> GetViewByID(int MainItemcode,int AlternateItemCode);
        Task<ResponseResult> DeleteByID(int MainItemCode, int AlternateItemCode, string MachineName, int EntryByempId);
    }
}
