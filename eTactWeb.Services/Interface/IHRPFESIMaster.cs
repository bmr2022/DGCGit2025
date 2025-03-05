using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRPFESIMaster
    {
        Task<ResponseResult> GetESIDispensary();
        Task<ResponseResult> FillEntryId();
        Task<DataSet> GetExemptedCategories();
        Task<ResponseResult> SaveData(HRPFESIMasterModel model);
        Task<ResponseResult> GetDashboardData();
        Task<HRPFESIMasterModel> GetDashboardDetailData();
        Task<HRPFESIMasterModel> GetViewByID(int Id);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> DeleteByID(int ID,string machineName);
    }
}
