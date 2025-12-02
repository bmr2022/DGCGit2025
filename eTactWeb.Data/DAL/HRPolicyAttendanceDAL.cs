using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class HRPolicyAttendanceDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public HRPolicyAttendanceDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<HRPolicyAttendanceModel> GetByIdHRPolicyAttendanc(int policyId)
        {
            var model = new HRPolicyAttendanceModel();

            var SqlParams = new List<dynamic>
    {
        new SqlParameter("@Flag", "ViewById"),
        new SqlParameter("@PolicyId", policyId)
    };

            var response = await _IDataLogic.ExecuteDataSet("SPHRPolicyAttendanceDetail", SqlParams).ConfigureAwait(false);

            if (response != null && response.Result is DataSet ds && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];

                model.PolicyId = Convert.ToInt32(row["PolicyId"]);
                model.PolicyName = row["PolicyName"].ToString();
                model.IsActive = row["IsActive"].ToString();
                model.HalfDayApplicableAfterLateMin = row["HalfDayApplicableAfterLateMin"].ToString();
                model.MaxGraceMinforlate = Convert.ToInt32(row["MaxGraceMinforlate"]);
                model.FullDayAbsentIfPresentLessthenMin = Convert.ToInt32(row["FullDayAbsentIfPresentLessthenMin"]);
                model.OverTimeEligibleYN = row["OverTimeEligibleYN"].ToString();
                model.OverTimeRateXBasicRate = Convert.ToInt32(row["OverTimeRateXBasicRate"]);
                model.OverTimeApplicableAfterExtraHours = row["OverTimeApplicableAfterExtraHours"].ToString();
                model.WeekOffFixedRotate = row["WeekOffFixedRotate"].ToString();
                model.MinNoOfPresentDaysreqForWeekOff = Convert.ToInt32(row["MinNoOfPresentDaysreqForWeekOff"]);
                model.HalfDayCountIfLeftBeforeShiftTime = Convert.ToInt32(row["HalfDayCountIfLeftBeforeShiftTime"]);
                model.FlexibleInOutTimingAllowed = row["FlexibleInOutTimingAllowed"].ToString();
                model.SandwitchWeekoffPolicyApplicable = row["SandwitchWeekoffPolicyApplicable"].ToString();
                model.SandWitchHolidayPolicyApplicable = row["SandWitchHolidayPolicyApplicable"].ToString();
                model.MaxLateIncomingAllowed = Convert.ToInt32(row["MaxLateIncomingAllowed"]);
                model.MaxEarlyLeavingAllowed = Convert.ToInt32(row["MaxEarlyLeavingAllowed"]);
                model.LateComingEarlyGoingCountSame = row["LateComingEarlyGoingCountSame"].ToString();
                model.MaxNoOfHalfDayInMonth = Convert.ToInt32(row["MaxNoOfHalfDayInMonth"]);
                model.MaxNoOfhourinAWeek = Convert.ToInt32(row["MaxNoOfhourinAWeek"]);
            }


            return model;
        }

        public async Task<ResponseResult> SaveHRPolicyAttendance(HRPolicyAttendanceModel model)
        {
            var _ResponseResult = new ResponseResult();


            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", model.PolicyId == 0 ? "Insert" : "Update"));
            SqlParams.Add(new SqlParameter("@PolicyId", model.PolicyId));
            SqlParams.Add(new SqlParameter("@PolicyName", model.PolicyName));
            SqlParams.Add(new SqlParameter("@IsActive", model.IsActive));
            SqlParams.Add(new SqlParameter("@HalfDayApplicableAfterLateMin", model.HalfDayApplicableAfterLateMin));
            SqlParams.Add(new SqlParameter("@MaxGraceMinforlate", model.MaxGraceMinforlate));
            SqlParams.Add(new SqlParameter("@FullDayAbsentIfPresentLessthenMin", model.FullDayAbsentIfPresentLessthenMin));
            SqlParams.Add(new SqlParameter("@OverTimeEligibleYN", model.OverTimeEligibleYN));
            SqlParams.Add(new SqlParameter("@OverTimeRateXBasicRate", model.OverTimeRateXBasicRate));
            SqlParams.Add(new SqlParameter("@OverTimeApplicableAfterExtraHours", model.OverTimeApplicableAfterExtraHours));
            SqlParams.Add(new SqlParameter("@WeekOffFixedRotate", model.WeekOffFixedRotate));
            SqlParams.Add(new SqlParameter("@MinNoOfPresentDaysreqForWeekOff", model.MinNoOfPresentDaysreqForWeekOff));
            SqlParams.Add(new SqlParameter("@HalfDayCountIfLeftBeforeShiftTime", model.HalfDayCountIfLeftBeforeShiftTime));
            SqlParams.Add(new SqlParameter("@FlexibleInOutTimingAllowed", model.FlexibleInOutTimingAllowed));
            SqlParams.Add(new SqlParameter("@SandwitchWeekoffPolicyApplicable", model.SandwitchWeekoffPolicyApplicable));
            SqlParams.Add(new SqlParameter("@SandWitchHolidayPolicyApplicable", model.SandWitchHolidayPolicyApplicable));
            SqlParams.Add(new SqlParameter("@MaxLateIncomingAllowed", model.MaxLateIncomingAllowed));
            SqlParams.Add(new SqlParameter("@MaxEarlyLeavingAllowed", model.MaxEarlyLeavingAllowed));
            SqlParams.Add(new SqlParameter("@LateComingEarlyGoingCountSame", model.LateComingEarlyGoingCountSame));
            SqlParams.Add(new SqlParameter("@MaxNoOfHalfDayInMonth", model.MaxNoOfHalfDayInMonth));
            SqlParams.Add(new SqlParameter("@MaxNoOfhourinAWeek", model.MaxNoOfhourinAWeek));


            var result = await _IDataLogic.ExecuteDataSet("SPHRPolicyAttendanceDetail", SqlParams).ConfigureAwait(false);
            return result;
        }

    }
}
