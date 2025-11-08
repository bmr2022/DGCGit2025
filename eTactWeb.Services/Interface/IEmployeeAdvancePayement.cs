using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IEmployeeAdvancePayement
    {
        Task<ResponseResult> FillEntryId(int yearCode);
        Task<ResponseResult> FillEmpName();
    }
}
