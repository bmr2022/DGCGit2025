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
    public class SalesPersonTransferBLL:ISalesPersonTransfer
    {
		private SalesPersonTransferDAL _SalesPersonTransferDAL;
		private readonly IDataLogic _DataLogicDAL;

		public SalesPersonTransferBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_SalesPersonTransferDAL = new SalesPersonTransferDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<ResponseResult> FillNewSalesEmpName(string ShowAllEmp)
		{
			return await _SalesPersonTransferDAL.FillNewSalesEmpName(ShowAllEmp);
		}
		public async Task<ResponseResult> FillOldSalesEmpName(string ShowAllEmp)
		{
			return await _SalesPersonTransferDAL.FillOldSalesEmpName(ShowAllEmp);
		}
        public async Task<SalesPersonTransferModel> FillCustomerList(string ShowAllCust)
        {
            return await _SalesPersonTransferDAL.FillCustomerList( ShowAllCust);
        }
    }
}
