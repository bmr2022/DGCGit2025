using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class InProcessInspectionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public InProcessInspectionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
		public async Task<ResponseResult> FillPartCode(string InspectionType)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
				SqlParams.Add(new SqlParameter("@InprocessOutIncoming", InspectionType));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillItemName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillMachineName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillMachineName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCustomer()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCustomer"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillColor(string PartNo)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillColor"));
				SqlParams.Add(new SqlParameter("@Itemcode", PartNo));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}

	}
}
