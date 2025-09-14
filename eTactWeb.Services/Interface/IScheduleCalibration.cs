using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IScheduleCalibration
    {
		Task<PendingScheduleCalibrationModel> GetScheduleCalibrationSearchData(string PartCode, string ItemName,string ToolCode,string ToolName);
	}
}
