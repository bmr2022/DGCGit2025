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
    public class MinimumMaximaumLevelBLL:IMinimumMaximaumLevel
    {
        private MinimumMaximaumLevelDAL _MinimumMaximaumLevelDAL;
        private readonly IDataLogic _DataLogicDAL;

        public MinimumMaximaumLevelBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _MinimumMaximaumLevelDAL = new MinimumMaximaumLevelDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillStoreName()
        {
            return await _MinimumMaximaumLevelDAL.FillStoreName();
        } 
        public async Task<ResponseResult> FillPartCode()
        {
            return await _MinimumMaximaumLevelDAL.FillPartCode();
        }
        public async Task<ResponseResult> FillItemName()
        {
            return await _MinimumMaximaumLevelDAL.FillItemName();
        }
        public async Task<MinimumMaximaumLevelModel> GetStandardDetailsData(string fromDate, string toDate, string ReportType, string PartCode, string StoreName, int Yearcode, string CurrentDate,string ShowItem)
        {
            return await _MinimumMaximaumLevelDAL.GetStandardDetailsData( fromDate,  toDate,  ReportType,  PartCode, StoreName,  Yearcode,  CurrentDate, ShowItem);
        }
    }
}
