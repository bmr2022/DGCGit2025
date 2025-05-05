using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDeassembleItem
    {
        Task<ResponseResult> NewEntryId();
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillFGItemName();
        Task<ResponseResult> FillFGPartCode();
        Task<ResponseResult> FillBomNo(int FinishItemCode);
    }
}
