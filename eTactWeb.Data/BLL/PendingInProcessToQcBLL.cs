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
    public class PendingInProcessToQcBLL : IPendingInProcessToQc
    {
        private readonly PendingInProcessToQcDAL _PendInProcesstoQcDAL;
        private readonly IDataLogic _DataLogicDAL;
        public PendingInProcessToQcBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _PendInProcesstoQcDAL = new PendingInProcessToQcDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<DataSet> BindItem(string Flag)
        { 
            return await _PendInProcesstoQcDAL.BindItem(Flag);
        }
        public async Task<DataSet> BindProdSlip(string Flag)
        {
            return await _PendInProcesstoQcDAL.BindProdSlip(Flag);
        }
        public async Task<DataSet> BindWorkCenter(string Flag)
        {
            return await _PendInProcesstoQcDAL.BindWorkCenter(Flag);
        }
        public async Task<ResponseResult> GetDataForPendingInProcess(string Flag, string FromDate, string ToDate, string PartCode, string ItemName, string ProdSlipNo, string WorkCenter, string GlobalSearch)
        {
            return await _PendInProcesstoQcDAL.GetDataForPendingMRN(Flag, FromDate, ToDate,PartCode,ItemName,ProdSlipNo,WorkCenter,GlobalSearch);
        }
        public async Task<ResponseResult> GetDataforQc(DataTable DisplayPendForQC,string Flag, string FromDate, string ToDate)
        {
            return await _PendInProcesstoQcDAL.GetDataforQc(DisplayPendForQC,Flag, FromDate, ToDate);
        }
    }
}
