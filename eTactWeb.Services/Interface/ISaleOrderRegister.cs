using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public  interface ISaleOrderRegister
    {
        Task<ResponseResult> FillPartCode(string FromDate, string ToDate);
        Task<ResponseResult> FillSaleOrderNo(string FromDate, string ToDate);
        Task<ResponseResult> FillCustOrderNo(string FromDate, string ToDate);
        Task<ResponseResult> FillSchNo(string FromDate, string ToDate);
        Task<ResponseResult> FillCustomerName(string FromDate, string ToDate);
        Task<ResponseResult> FillSalesPerson(string FromDate, string ToDate);

        public Task<SaleOrderRegisterModel> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate);

    }
}
