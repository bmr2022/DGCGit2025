using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IIndentRegister
    {
        Task<ResponseResult> GetItemName(string FromDate, string ToDate);
        Task<ResponseResult> GetPartCode(string FromDate, string ToDate);

        Task<ResponseResult> GetIndentNo(string FromDate, string ToDate);

        public Task<IndentRegisterModel> GetDetailsData(string FromDate, string ToDate, string ItemName, string PartCode,string IndentNo,string ReportType);
    }
}
