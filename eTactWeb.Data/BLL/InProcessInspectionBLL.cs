using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.BLL
{
    public  class InProcessInspectionBLL:IInProcessInspection
    {
        private InProcessInspectionDAL _InProcessInspectionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public InProcessInspectionBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _InProcessInspectionDAL = new InProcessInspectionDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
    }
}
