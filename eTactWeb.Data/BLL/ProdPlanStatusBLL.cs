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
    public class ProdPlanStatusBLL:IProdPlanStatus
    {
        private ProdPlanStatusDAL _ProdPlanStatusDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ProdPlanStatusBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ProdPlanStatusDAL = new ProdPlanStatusDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ProdPlanStatusModel> GetProdPlanStatus()
        {
            return await _ProdPlanStatusDAL.GetProdPlanStatus();
        }
    }
}
