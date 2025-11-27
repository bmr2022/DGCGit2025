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
    public class SaleBillApprovalBLL:ISaleBillApproval
    {
        private SaleBillApprovalDAL _SaleBillApprovalDAL;
        private readonly IDataLogic _DataLogicDAL;

        public SaleBillApprovalBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _SaleBillApprovalDAL = new SaleBillApprovalDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetPendingSaleBillSummary(string fromDate, string toDate, string saleBillEntryFrom)
        {
            return await _SaleBillApprovalDAL.GetPendingSaleBillSummary(fromDate, toDate, saleBillEntryFrom);
        }
    }
}
