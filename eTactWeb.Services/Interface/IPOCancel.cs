using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{public interface IPOCancel
    {
        Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string CancelationType, string PONO, string VendorName, int EmpId, string UID);

        Task<List<POCancleDetail>> ShowPODetail(int ID, int YearCode, string PoNo, string TypeOfCancle);
    }
}
