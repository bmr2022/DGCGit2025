using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAccDepriciationCalculationdetail
    {
		Task<AccDepriciationCalculationdetailModel> GetAssets(int DepriciationYearCode,string AssetsName,string DepreciationMethod,string AssetsCategoryName);
		Task<ResponseResult> FillEntryID(string EntryDate,int YearCode);
		Task<ResponseResult> FillEssetsName();
		Task<ResponseResult> SaveDepriciationCalculationdetail(AccDepriciationCalculationdetailModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData(AccDepriciationCalculationdetailModel model);
        Task<AccDepriciationCalculationdetailModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType,string AssetsName);
		Task<ResponseResult> DeleteByID(int EntryId, int YearCode);
		Task<AccDepriciationCalculationdetailModel> GetViewByID(int ID,int YC);
	}
}
