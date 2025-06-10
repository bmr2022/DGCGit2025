using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IOrderBasedProdPlan
    {
        Task<OrderBasedProdPlanModel> FillSONO_OrderNO_SchNo(string FromDate, string ToDate);
        Task<OrderBasedProdPlanModel> GetOrderBasedProdPlanData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, int ItemCode);
    }
}
