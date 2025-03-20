using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAutoGenerateSchedule
    {
        public Task<ResponseResult> GetMrpNo(string SchEffectivedate, string MrpYearCode);
        public  Task<AutoGenerateScheduleModel> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy,string CC,int UID,string MachineName);
    }
}
