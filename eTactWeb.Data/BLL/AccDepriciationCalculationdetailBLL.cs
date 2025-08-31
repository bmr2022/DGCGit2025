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

namespace eTactWeb.Data.BLL
{
    public class AccDepriciationCalculationdetailBLL: IAccDepriciationCalculationdetail
    {
		private AccDepriciationCalculationdetailDAL _AccDepriciationCalculationdetailDAL;
		private readonly IDataLogic _DataLogicDAL;

		public AccDepriciationCalculationdetailBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_AccDepriciationCalculationdetailDAL = new AccDepriciationCalculationdetailDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<AccDepriciationCalculationdetailModel> GetAssets()
		{
			return await _AccDepriciationCalculationdetailDAL.GetAssets();
		}
	}
}
