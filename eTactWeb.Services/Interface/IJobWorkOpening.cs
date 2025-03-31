using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IJobWorkOpening
    {
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> FillEntryId(string EntryId, int YearCode, string FormTypeCustJWNRGP, string SPName);
        Task<JobWorkOpeningModel> GetViewByID(int ID, string Mode,int YC,string OpeningType);
        Task<ResponseResult> DeleteByID(int ID, int YC, string SumDetail, int Itemcode, string ChallanNo,string OpeningType,
            int ActualEntryById, string MachineName, int AccountCode, string Partcode, string ItemName);
        Task<ResponseResult> GetDashboardData(JobWorkOpeningDashboard model);
        Task<ResponseResult> GetDetailData(JobWorkOpeningDashboard model);
        Task<ResponseResult> SaveJobWorkOpening(JobWorkOpeningModel model, DataTable GIGrid);
        Task<ResponseResult> FillItemPartCode();
        Task<ResponseResult> FillBomNo(int ItemCode, int RecItemCode, string BomType);
        Task<ResponseResult> FillUnitAltUnit(int ItemCode);
        Task<ResponseResult> FillPartyName();
        Task<ResponseResult> FillProcessName();
        Task<ResponseResult> FillRecItemPartCode(string BOMIND, int ItemCode);
        Task<ResponseResult> FillScrapItemPartCode(string BOMIND, int ItemCode);
    }
}
