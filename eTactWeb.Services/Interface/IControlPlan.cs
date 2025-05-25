using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IControlPlan
    {
        Task<ResponseResult> GetNewEntryId(int Yearcode);
        Task<ResponseResult> GetItemName();
        Task<ResponseResult> GetPartCode();
        Task<ResponseResult> GetEvMeasureTech();
        Task<ResponseResult> GetCharacteristic();
       // Task<ResponseResult> SaveControlPlan(ControlPlanModel model, DataTable GIGrid);
    }
}
