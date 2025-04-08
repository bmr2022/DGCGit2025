using eTactWeb.DOM.Models;
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
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> FillEntryID(int YearCode);
        Task<ResponseResult> FillBranch();
     
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillWorkCenterName();
        Task<ResponseResult> GetOriginalPartCode();
        Task<ResponseResult> GetOriginalItemName();
        Task<ResponseResult> GetUnitAltUnit(int ItemCode);
        Task<ResponseResult> GetAltPartCode(int MainItemcode);
        Task<ResponseResult> GetAltItemName(int MainItemcode);
        Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName,string WorkCenterName, int YearCode, string batchno, string FinStartDate);

        Task<ResponseResult> SaveMaterialConversion(MaterialConversionModel model, DataTable GIGrid);

        Task<ResponseResult> GetDashboardData(MaterialConversionModel model);
        Task<MaterialConversionModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType);
        Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId);
        Task<MaterialConversionModel> GetViewByID(int ID, int YC,string FromDate,string TODate);


    }
}
