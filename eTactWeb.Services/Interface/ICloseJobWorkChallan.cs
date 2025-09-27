using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICloseJobWorkChallan
    {
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll);

        Task<List<CloseJobWorkChallanModel>> ShowDetail(int ID, int YearCode);



    }
}
