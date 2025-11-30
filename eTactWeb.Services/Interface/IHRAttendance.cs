using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRAttendance
    {
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> FillEntryId(int YearCode);
        Task<HRAListDataModel> GetHRAListData(string? flag, string? AttandanceDate, string? EmpCareg, HRAListDataModel model);
    }
}
