using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ILedgerOpeningCarryforward
    {
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> FILLFINYEAR();
        Task<ResponseResult> CarryforwardLedgerbalance(LedgerOpeningCarryforwardModel model);

        
    }
}
