using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IGateAttendance
    {
        Task<GateAttendanceModel> GetManualAttendance(string DayOrMonthType, DateTime Attdate, int AttMonth, int YearCode);
        GateAttendanceModel GetHolidayList(int EmpCatId, DateTime Attdate, int YearCode);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> FillEntryId(int YearCode);
        Task<ResponseResult> SaveGateAtt(GateAttendanceModel model, DataTable itemgrid);
        Task<GateAttDashBoard> GetDashBoardData();
    }
}
