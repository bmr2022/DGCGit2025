using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRPolicyAttendanceBLL
    {
        Task<ResponseResult> SaveHRPolicyAttendance(HRPolicyAttendanceModel model);
        Task<HRPolicyAttendanceModel> GetByIdHRPolicyAttendanc(int policyId);
    }
}
