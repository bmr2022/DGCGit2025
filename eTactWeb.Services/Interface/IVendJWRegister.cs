using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IVendJWRegister
    {
        Task<VendJWRegisterModel> GetJWRegisterData(string FromDate, string ToDate,string RecChallanNo,string IssChallanNo, string PartyName,  string PartCode, string ItemName, string IssueChallanType, string ReportMode);
    }
}
