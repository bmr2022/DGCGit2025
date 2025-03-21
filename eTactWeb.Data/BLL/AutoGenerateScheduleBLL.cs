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
        public async Task<ResponseResult> GetMrpNo(string SchEffectivedate, int YearCode)
        {
            return await _AutoGenerateScheduleDAL.GetMrpNo( SchEffectivedate, YearCode);
        }
         public async Task<ResponseResult> GetMrpId(string SchEffectivedate, int YearCode, int MrpNo, int MrpYearCode)
        {
            return await _AutoGenerateScheduleDAL.GetMrpId( SchEffectivedate, YearCode, MrpNo, MrpYearCode);
        }
         public async Task<ResponseResult> GetMrpYearCode(string SchEffectivedate, int YearCode, int MrpNo)
        {
            return await _AutoGenerateScheduleDAL.GetMrpYearCode( SchEffectivedate, YearCode, MrpNo);
        }
        //public async Task<ResponseResult> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        //{
        //    return await _AutoGenerateScheduleDAL.GetAutoGenSchDetailData(ReportType, SchEffectivedate, MrpNo, MrpYearCode, MrpEntryId, CreatedBy, CC, UID, MachineName);
        //}

        public async Task<AutoGenerateScheduleModel> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        {
            return await _AutoGenerateScheduleDAL.GetAutoGenSchDetailData(ReportType, SchEffectivedate, MrpNo, MrpYearCode, MrpEntryId, CreatedBy, CC, UID, MachineName);
        }
    }
}
