using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IScheduleCalibration
    {
		Task<PendingScheduleCalibrationModel> GetScheduleCalibrationSearchData(string PartCode, string ItemName,string ToolCode,string ToolName);
        Task<ResponseResult> GetCalibrationAgency();
        Task<ResponseResult> GetFormRights(int uId);

    }
}
