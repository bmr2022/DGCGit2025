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
    public class CancelRequitionBLL:ICancelRequition
    {
        private readonly CancelRequitionDAL _CancelRequitionDAL;
        private readonly IDataLogic _DataLogicDAL;
        public CancelRequitionBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CancelRequitionDAL = new CancelRequitionDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<ResponseResult> FillItemName(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            return await _CancelRequitionDAL.FillItemName(Fromdate, ToDate, ReportType, PendCancelReq, RequitionType, ReqNo, ItemCode);
        }
         public async Task<ResponseResult> FillPartcode(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            return await _CancelRequitionDAL.FillPartcode(Fromdate, ToDate, ReportType, PendCancelReq, RequitionType, ReqNo, ItemCode);
        }
        public async Task<ResponseResult> FillReqNo(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            return await _CancelRequitionDAL.FillReqNo(Fromdate, ToDate, ReportType, PendCancelReq, RequitionType, ReqNo, ItemCode);
        }
        public async Task<CancelRequitionModel> GetSearchData(string Fromdate, string ToDate, string ReportType, string PendCancelReq, string RequitionType, string ReqNo, int ItemCode)
        {
            return await _CancelRequitionDAL.GetSearchData(Fromdate, ToDate, ReportType, PendCancelReq, RequitionType, ReqNo, ItemCode);
        }

    }
}
