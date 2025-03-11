using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRLeaveApplicationMaster
    {
        Task<ResponseResult> GetEmpName();
        Task<ResponseResult> GetLeaveName();
        Task<ResponseResult> GetShiftName();
        Task<ResponseResult> GetEmpCode();
        Task<ResponseResult> FillEntryId(int YearCode);

        Task<ResponseResult> GetEmployeeDetail(int empid);

        Task<ResponseResult> SaveData(HRLeaveApplicationMasterModel model, DataTable DT);
    }
}
