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
        public Task<ResponseResult> GetMrpNo(string SchEffectivedate, int YearCode);
        public Task<ResponseResult> GetMrpId(string SchEffectivedate, int YearCode,int MrpNo,int MrpYearCode);
        public Task<ResponseResult> GetMrpYearCode(string SchEffectivedate, int YearCode,int MrpNo);
        public  Task<AutoGenerateScheduleModel> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy,string CC,int UID,string MachineName);
    }
}
