using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IOrderAmendHistory
    {
        Task<OrderAmendHistoryModel> GetOrderAmendHistoryData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, string PONO, int ItemCode,string HistoryReportMode);
    }
}
