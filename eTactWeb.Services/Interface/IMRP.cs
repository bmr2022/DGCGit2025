using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMRP
    {
        public Task<ResponseResult> PendingMRPData(PendingMRP model);
        public Task<ResponseResult> GetStore();
        public Task<ResponseResult> GetWorkCenter();
        public Task<ResponseResult> IsCheckMonthWiseData(int Month, int Year);
        public Task<ResponseResult> NewEntryId(int YearCode);
        public Task<ResponseResult> GetFormRights(int UserID);
        public Task<ResponseResult> GetDashboardData(MRPDashboard model);
        public Task<ResponseResult> DeleteByID(int ID, int YC, string MRPNo);
        public Task<MRPMain> GetMRPDetailData(PendingMRP model);
        public Task<MRPMain> GetMRPFGRMDetailData(PendingMRP model);
        Task<ResponseResult> SaveMRPDetail(MRPMain model, DataTable MRPGrid, DataTable MRPSOGrid, DataTable MRPFGGrid);
        Task<MRPMain> GetViewByID(int ID, string Mode, int YC);

    }
}