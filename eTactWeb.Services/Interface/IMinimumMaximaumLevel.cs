using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMinimumMaximaumLevel
    {
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillPartCode();
        Task<ResponseResult> FillItemName();
        Task<MinimumMaximaumLevelModel> GetStandardDetailsData(string fromDate, string toDate,  string ReportType, string PartCode, string StoreName, int Yearcode, string CurrentDate,string ShowItem);

    }
}
