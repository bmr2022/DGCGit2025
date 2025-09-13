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
    public class GateAttendanceBLL : IGateAttendance
    {
        private GateAttendanceDAL _GateAttendanceDAL;
        private readonly IDataLogic _DataLogicDAL;

        public GateAttendanceBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _GateAttendanceDAL = new GateAttendanceDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        async Task<GateAttendanceModel> IGateAttendance.GetManualAttendance(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode)
        {
            return await _GateAttendanceDAL.GetManualAttendance(DayOrMonthType, Attdate, AttMonth, YearCode);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _GateAttendanceDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillEntryId(int YearCode)
        {
            return await _GateAttendanceDAL.FillEntryId(YearCode);
        }
    }
}
