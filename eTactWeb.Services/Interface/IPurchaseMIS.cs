using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPurchaseMIS
    {
        Task<ResponseResult> FillItemName();
        Task<ResponseResult> FillPartCode();
        Task<ResponseResult> FillAccountName();
        Task<PurchaseMISModel> GetPurchaseMISDetailsData(string ReportType, string ToDate, int YearCode, int Itemcode,int AccountCode);
    }
}
