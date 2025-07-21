using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IVendoreRatingAnalysisReport
    {
        Task<VendoreRatingAnalysisReportModel> GetVendoreRatingDetailsData(string ReportType,string RatingType, string CurrentDate, string PartCode, string ItemName, string CustomerName, int YearCode);
    }
}
