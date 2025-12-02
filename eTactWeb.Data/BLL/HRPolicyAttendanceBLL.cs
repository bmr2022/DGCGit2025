using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class HRPolicyAttendanceBLL : IHRPolicyAttendanceBLL
    {
        private HRPolicyAttendanceDAL _hrPolicyAttendanceDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRPolicyAttendanceBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _hrPolicyAttendanceDAL = new HRPolicyAttendanceDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> SaveHRPolicyAttendance(HRPolicyAttendanceModel model)
        {
            return await _hrPolicyAttendanceDAL.SaveHRPolicyAttendance(model);
        }  public async Task<HRPolicyAttendanceModel> GetByIdHRPolicyAttendanc(int policyId)
        {
            return await _hrPolicyAttendanceDAL.GetByIdHRPolicyAttendanc(policyId);
        }
    }
}
