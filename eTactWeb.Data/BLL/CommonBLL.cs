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
    public class CommonBLL : ICommon
    {
        private CommonDAL _CommonDAL;
        private readonly IDataLogic _IDataLogic;

        public CommonBLL(IConfiguration config, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CommonDAL = new CommonDAL(config, iDataLogic, connectionStringService);
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName)
        {
            return await _CommonDAL.CheckFinYearBeforeSave(YearCode, Date, DateName);
        }
    }
}
