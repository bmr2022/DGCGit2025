using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IREQUISITIONRegister
    {
        Task<REQUISITIONRegistermodel> GetREQUISITIONRegisterData(string Flag, string ReQType, string fromDate, string ToDate, string REQNo, string Partcode, string ItemName, string FromstoreId, string Toworkcenter, int ReqYearcode);
    }
}
