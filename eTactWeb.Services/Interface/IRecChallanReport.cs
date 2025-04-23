using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IRecChallanReport
    {
        Task<RecChallanReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate, int EntryId, int YearCode);
    }
}
