
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PendingSaleRejectionBLL : IPendingSaleRejection
    {
        private readonly PendingSaleRejectionDAL _PendingSaleRejectionDAL;
        private readonly IDataLogic _DataLogicDAL;
        public PendingSaleRejectionBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _PendingSaleRejectionDAL = new PendingSaleRejectionDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
        public async Task<ResponseResult> PendingMRNForSaleRejection(string fromDate, string toDate, string mrnNo, string gateNo,string customerName)
        {
            return await _PendingSaleRejectionDAL.PendingMRNForSaleRejection(fromDate, toDate,mrnNo,gateNo,customerName);
        }
        public async Task<ResponseResult> FillMRNNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            return await _PendingSaleRejectionDAL.FillMRNNO(fromDate, toDate, mrnNo, gateNo);
        }
        public async Task<ResponseResult> FillGateNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            return await _PendingSaleRejectionDAL.FillGateNO(fromDate, toDate, mrnNo, gateNo);
        }
        public async Task<ResponseResult> FillCustomerInvNO(string fromDate, string toDate, string mrnNo, string gateNo)
        {
            return await _PendingSaleRejectionDAL.FillCustomerInvNO(fromDate, toDate, mrnNo, gateNo);
        }
        public async Task<ResponseResult> FillPartyName(string fromDate, string toDate)
        {
            return await _PendingSaleRejectionDAL.FillPartyName( fromDate,  toDate);
        }
    }
}
