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
    public class HRAttendanceBLL : IHRAttendance
    {
        private readonly IDataLogic _DataLogicDAL;

        private readonly HRAttendanceDAL _HRAttendanceDAL;

        public HRAttendanceBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _HRAttendanceDAL = new HRAttendanceDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        async Task<HRAListDataModel> IHRAttendance.GetHRAListData(string? flag, string? AttendanceDate, string? EmpCateg, HRAListDataModel model)
        {
            throw new NotImplementedException();//return await _HRAttendanceDAL.GetHRAttendanceListData(flag, AttendanceDate, EmpCateg, model);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _HRAttendanceDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillEntryId(int YearCode)
        {
            return await _HRAttendanceDAL.FillEntryId(YearCode);
        }
    }
}
