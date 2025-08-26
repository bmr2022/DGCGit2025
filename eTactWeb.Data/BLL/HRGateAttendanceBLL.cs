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
    public class HRGateAttendanceBLL : IHRGateAttendance
    {
        private HRGateAttendanceDAL _HRGateAttendanceDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRGateAttendanceBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _HRGateAttendanceDAL = new HRGateAttendanceDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
    }
}
