using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHSNMaster
    {
        Task<IEnumerable<HSNMasterModel>> GetDashboard();
        Task<HSNMasterModel> GetById(int id);
        Task<ResponseResult> Insert(HSNMasterModel model);
        Task<ResponseResult> Update(HSNMasterModel model);
        Task<ResponseResult> Delete(int id);
        Task<int> GetNewEntryId();
    }
}
