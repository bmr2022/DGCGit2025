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
    public class HRWeekOffMasterBLL: IHRWeekOffMaster
    {
        private HRWeekOffMasterDAL _HRWeekOffMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRWeekOffMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _HRWeekOffMasterDAL = new HRWeekOffMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
    }
}
