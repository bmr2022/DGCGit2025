using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPartyItemGroupDiscount
    {
		Task<ResponseResult> FillPartyName();
		Task<ResponseResult> FillCategoryName();
		Task<ResponseResult> FillCategoryCode();
		Task<ResponseResult> FillGroupName();
		Task<ResponseResult> FillGroupCode();
			Task<ResponseResult> FillPartyNameDashBoard();
		Task<ResponseResult> FillCategoryNameDashBoard();
		Task<ResponseResult> FillCategoryCodeDashBoard();
		Task<ResponseResult> FillGroupNameDashBoard();
		Task<ResponseResult> FillGroupCodeDashBoard();

		Task<ResponseResult> FillEntryID(string EntryDate);
		Task<ResponseResult> SavePartyItemGroupDiscount(PartyItemGroupDiscountModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData(PartyItemGroupDiscountModel model);
        Task<PartyItemGroupDiscountModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType, int AccountCode, int CategoryId, string GroupName);
        Task<ResponseResult> DeleteByID(int EntryId, int AccountCode, string EntryDate);
		Task<PartyItemGroupDiscountModel> GetViewByID(int ID);
	}
}
