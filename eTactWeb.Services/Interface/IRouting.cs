using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IRouting
    {
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> FillItems(string Flag);
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> AlreadyExistItems(string Flag);
        Task<ResponseResult> FillSubItems(string Flag);
        Task<RoutingModel> GetAllDataItemWise(string Flag, int ItemCode);
        Task<ResponseResult> SaveRouting(RoutingModel model, DataTable JWGrid);
        Task<ResponseResult> GetDashboardData(string FromDate, string ToDate);
        Task<RoutingGridDashBoard> GetDashboardData(string SummaryDetail, string PartCode, string ItemName, string Stage, string WorkCenter, string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID);
        Task<ResponseResult> GetNewEntryId();
        Task<ResponseResult> GetFormRights(int ID);
        Task<RoutingModel> GetViewByID(int ID, string Mode);

    }
}