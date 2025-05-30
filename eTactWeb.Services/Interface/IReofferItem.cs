using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IReofferItem
    {
        Task<ResponseResult> GETNEWENTRY(int ReofferYearcode);
        Task<ResponseResult> FILLQCTYPE();
        Task<ResponseResult> FILLMIRNO();
    }
}
