using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMaterialConversion
    {
        Task<ResponseResult> FillEntryID(int YearCode);
        Task<ResponseResult> FillBranch();
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillWorkCenterName();
        Task<ResponseResult> GetOriginalPartCode();
        Task<ResponseResult> GetOriginalItemName();
    }
}
