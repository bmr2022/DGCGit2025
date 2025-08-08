using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PartyItemGroupDiscountBLL:IPartyItemGroupDiscount
    {
		private PartyItemGroupDiscountDAL _PartyItemGroupDiscountDAL;
		private readonly IDataLogic _DataLogicDAL;

		public PartyItemGroupDiscountBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_PartyItemGroupDiscountDAL = new PartyItemGroupDiscountDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<ResponseResult> FillPartyName()
		{
			return await _PartyItemGroupDiscountDAL.FillPartyName();
		}
		public async Task<ResponseResult> FillEntryID(string EntryDate)
		{
			return await _PartyItemGroupDiscountDAL.FillEntryID(EntryDate);
		}

		public async Task<ResponseResult> FillCategoryName()
		{
			return await _PartyItemGroupDiscountDAL.FillCategoryName();
		}
		public async Task<ResponseResult> FillCategoryCode()
		{
			return await _PartyItemGroupDiscountDAL.FillCategoryCode();
		}
		public async Task<ResponseResult> FillGroupName()
		{
			return await _PartyItemGroupDiscountDAL.FillGroupName();
		}
		public async Task<ResponseResult> FillGroupCode()
		{
			return await _PartyItemGroupDiscountDAL.FillGroupCode();
		}
		public async Task<ResponseResult> FillPartyNameDashBoard()
		{
			return await _PartyItemGroupDiscountDAL.FillPartyNameDashBoard();
		}
		

		public async Task<ResponseResult> FillCategoryNameDashBoard()
		{
			return await _PartyItemGroupDiscountDAL.FillCategoryNameDashBoard();
		}
		public async Task<ResponseResult> FillCategoryCodeDashBoard()
		{
			return await _PartyItemGroupDiscountDAL.FillCategoryCodeDashBoard();
		}
		public async Task<ResponseResult> FillGroupNameDashBoard()
		{
			return await _PartyItemGroupDiscountDAL.FillGroupNameDashBoard();
		}
		public async Task<ResponseResult> FillGroupCodeDashBoard()
		{
			return await _PartyItemGroupDiscountDAL.FillGroupCodeDashBoard();
		}
		public async Task<ResponseResult> SavePartyItemGroupDiscount(PartyItemGroupDiscountModel model, DataTable GIGrid)
		{
			return await _PartyItemGroupDiscountDAL.SavePartyItemGroupDiscount(model, GIGrid);
		}
        public async Task<ResponseResult> GetDashboardData(PartyItemGroupDiscountModel model)
        {
            return await _PartyItemGroupDiscountDAL.GetDashboardData(model);
        }
        public async Task<PartyItemGroupDiscountModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _PartyItemGroupDiscountDAL.GetDashboardDetailData(FromDate, ToDate, ReportType);
        }

        public async Task<ResponseResult> DeleteByID(int EntryId, int AccountCode, string EntryDate)
        {
            return await _PartyItemGroupDiscountDAL.DeleteByID(EntryId, AccountCode, EntryDate);
        }
		public async Task<PartyItemGroupDiscountModel> GetViewByID(int ID)
		{
			return await _PartyItemGroupDiscountDAL.GetViewByID(ID);
		}
    }
}
