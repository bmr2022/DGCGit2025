using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IHRAttendance
    {
        Task<HRAListDataModel> GetHRAListData(string? flag, string? AttandanceDate, string? EmpCareg, HRAListDataModel model);
    }
}
