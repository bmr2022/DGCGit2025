using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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

	}
}
