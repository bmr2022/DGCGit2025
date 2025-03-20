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
    public class AutoGenerateScheduleBLL:IAutoGenerateSchedule
    {
        private AutoGenerateScheduleDAL _AutoGenerateScheduleDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AutoGenerateScheduleBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _AutoGenerateScheduleDAL = new AutoGenerateScheduleDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetMrpNo(string SchEffectivedate, string MrpYearCode)
        {
            return await _AutoGenerateScheduleDAL.GetMrpNo( SchEffectivedate,  MrpYearCode);
        }
        public async Task<AutoGenerateScheduleModel> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        {
            return await _AutoGenerateScheduleDAL.GetAutoGenSchDetailData(ReportType, SchEffectivedate, MrpNo, MrpYearCode, MrpEntryId, CreatedBy, CC,UID, MachineName);
        }
    }
}
