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
    public class CloseJobWorkChallanBLL: ICloseJobWorkChallan
    {
        private CloseJobWorkChallanDAL _CloseJobWorkChallanDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CloseJobWorkChallanBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _CloseJobWorkChallanDAL = new CloseJobWorkChallanDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll)
        {
            return await _CloseJobWorkChallanDAL.GetSearchData( FromDate,  ToDate,  AccountCode,  ChallanNO,  ShowClsoedPendingAll);
        }
        public async Task<List<CloseJobWorkChallanModel>> ShowDetail(int ID, int YC)
        {
            return await _CloseJobWorkChallanDAL.ShowDetail(ID, YC);
        }

    }
}
