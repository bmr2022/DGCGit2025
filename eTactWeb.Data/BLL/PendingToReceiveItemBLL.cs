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
    public class PendingToReceiveItemBLL : IPendingToReceiveItem
    {
        private readonly PendingToReceiveItemDAL _PendingToReceiveItemDAL;
        private readonly IDataLogic _DataLogicDAL;
        public PendingToReceiveItemBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _PendingToReceiveItemDAL = new PendingToReceiveItemDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<DataSet> BindItem(string Flag,string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindItem(Flag,FromDate,ToDate);
        }
        public async Task<DataSet> BindPartCode(string Flag, string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindPartCode(Flag, FromDate, ToDate);
        }
        public async Task<DataSet> BindWorkCenter(string Flag, string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindWorkCenter(Flag, FromDate, ToDate);
        }
        public async Task<DataSet> BindProdSlipNo(string Flag, string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindProdSlipNo(Flag, FromDate, ToDate);
        }
        public async Task<DataSet> BindStoreName(string Flag, string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindStoreName(Flag, FromDate, ToDate);
        }
        public async Task<DataSet> BindProdType(string Flag, string FromDate, string ToDate)
        {
            return await _PendingToReceiveItemDAL.BindProdType(Flag, FromDate, ToDate);
        }
        public async Task<ResponseResult> GetDataForPendingReceiveItem(string Flag, string FromDate, string ToDate,string partcode,string itemname, string slipno)
        {
            return await _PendingToReceiveItemDAL.GetDataForPendingReceiveItem(Flag, FromDate, ToDate,partcode,itemname,slipno);
        }
        public async Task<ResponseResult> GetDataReceiveItem(DataTable DisplayPendReceiveItem)
        {
            return await _PendingToReceiveItemDAL.GetDataReceiveItem(DisplayPendReceiveItem);
        }
    }
}
