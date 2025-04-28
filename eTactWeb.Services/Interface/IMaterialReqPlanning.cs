using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMaterialReqPlanning
    {
        Task<ResponseResult> GetMRPNo(int YearCode);
        Task<ResponseResult> GetMonthList(int YearCode);
        Task<ResponseResult> GetPartCode();
        Task<ResponseResult> GetItemName();
        public Task<MaterialReqPlanningModel> GetDetailData(string ReportType,string mrpno, string Month, int YearCode,string FromDate,string ToDate,string ItemName,string PartCode);


    }
}
