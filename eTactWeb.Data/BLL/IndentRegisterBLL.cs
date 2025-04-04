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
    public class IndentRegisterBLL: IIndentRegister
    {
        private IndentRegisterDAL _IndentRegisterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public IndentRegisterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _IndentRegisterDAL = new IndentRegisterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetItemName(string FromDate, string ToDate)
        {
            return await _IndentRegisterDAL.GetItemName(FromDate,ToDate);
        }

        public async Task<ResponseResult> GetPartCode(string FromDate, string ToDate)
        {
            return await _IndentRegisterDAL.GetPartCode(FromDate, ToDate);
        }

        public async Task<ResponseResult> GetIndentNo(string FromDate, string ToDate)
        {
            return await _IndentRegisterDAL.GetIndentNo(FromDate, ToDate);
        }
        public async Task<IndentRegisterModel> GetDetailsData(string FromDate, string ToDate, string ItemName, string PartCode, string IndentNo,string ReportType)
        {
            return await _IndentRegisterDAL.GetDetailsData( FromDate,  ToDate,  ItemName,  PartCode,  IndentNo, ReportType);
        }
        
    }
}
