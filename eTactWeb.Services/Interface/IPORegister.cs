using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPORegister
    {
        Task<PORegisterModel> GetPORegisterData(string FromDate, string ToDate, string ReportType, int YearCode, string Partyname, string partcode, string itemName, string POno, string SchNo, string OrderType, string POFor, string ItemType, string ItemGroup);
    }
}
