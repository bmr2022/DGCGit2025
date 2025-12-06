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
        async Task<GateAttendanceModel> IGateAttendance.GetManualAttendance(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode, int EmpCatCode, int EmpId, bool IsManual = false)
        {
            return await _GateAttendanceDAL.GetManualAttendance(DayOrMonthType, Attdate, AttMonth, YearCode, EmpCatCode, EmpId, IsManual);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _GateAttendanceDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillEntryId(int YearCode)
        {
            return await _GateAttendanceDAL.FillEntryId(YearCode);
        }
        GateAttendanceModel IGateAttendance.GetHolidayList(int EmpCatId, DateTime Attdate, int YearCode)
        {
            return _GateAttendanceDAL.GetHolidayList(EmpCatId, Attdate, YearCode);
        }
        Task<ResponseResult> IGateAttendance.SaveGateAtt(GateAttendanceModel model, DataTable itemgrid)
        {
            return _GateAttendanceDAL.SaveGateAtt(model, itemgrid);
        }
        async Task<GateAttDashBoard> IGateAttendance.GetDashBoardData(GateAttDashBoard model)
        {
            return await _GateAttendanceDAL.GetDashBoardData(model);
        }
        async Task<GateAttendanceModel> IGateAttendance.GetViewByID(int ID, int YearCode)
        {
            return await _GateAttendanceDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, int EntryBy, string EntryByMachineName, string cc, DateTime EntryDate)
        {
            return await _GateAttendanceDAL.DeleteByID(ID, YearCode, Flag, EntryBy, EntryByMachineName, cc, EntryDate);
        }
    }
}
