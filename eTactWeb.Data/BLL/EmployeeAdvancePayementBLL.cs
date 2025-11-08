using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class EmployeeAdvancePayementBLL : IEmployeeAdvancePayement
    {
        private EmployeeAdvancePayementDAL _EmployeeAdvancePayementDAL;
        private readonly IDataLogic _DataLogicDAL;

        public EmployeeAdvancePayementBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _EmployeeAdvancePayementDAL = new EmployeeAdvancePayementDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillEntryId(int yearCode)
        {
            return await _EmployeeAdvancePayementDAL.FillEntryId(yearCode);
        }
    }
}
