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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.BLL
{
    public  class CancelSaleBillrequisitionBLL:ICancelSaleBillrequisition
    {
        private CancelSaleBillrequisitionDAL _CancelSaleBillrequisitionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CancelSaleBillrequisitionBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _CancelSaleBillrequisitionDAL = new CancelSaleBillrequisitionDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillSaleBillNo(string CurrentDate,string SaleBillDate)
        {
            return await _CancelSaleBillrequisitionDAL.FillSaleBillNo(CurrentDate, SaleBillDate);
        }
        public async Task<ResponseResult> FillCanSaleBillReqEntryid(string CurrentDate,string SaleBillDate, int CanSaleBillReqYearcode)
        {
            return await _CancelSaleBillrequisitionDAL.FillCanSaleBillReqEntryid(CurrentDate, SaleBillDate, CanSaleBillReqYearcode);
        }
        public async Task<ResponseResult> FillCanRequisitionNo(string CurrentDate,string SaleBillDate, int CanSaleBillReqYearcode)
        {
            return await _CancelSaleBillrequisitionDAL.FillCanRequisitionNo(CurrentDate, SaleBillDate, CanSaleBillReqYearcode);
        }
        public async Task<ResponseResult> FillSaleBillNoYear(string CurrentDate,string SaleBillDate, string SaleBillNo)
        {
            return await _CancelSaleBillrequisitionDAL.FillSaleBillNoYear(CurrentDate, SaleBillDate, SaleBillNo);
        }
        public async Task<ResponseResult> FillCustomerName(string CurrentDate,string SaleBillDate)
        {
            return await _CancelSaleBillrequisitionDAL.FillCustomerName(CurrentDate, SaleBillDate);
        }
        public async Task<ResponseResult> FillSaleBillNoDate(string CurrentDate, string SaleBillNo, string SaleBillYearCode, string SaleBillDate)
        {
            return await _CancelSaleBillrequisitionDAL.FillSaleBillNoDate( CurrentDate,  SaleBillNo,  SaleBillYearCode,  SaleBillDate);
        }
        public async Task<ResponseResult> SaveCancelSaleBillRequisition(CancelSaleBillrequisitionModel model)
        {
            return await _CancelSaleBillrequisitionDAL.SaveCancelSaleBillRequisition(model);
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            return await _CancelSaleBillrequisitionDAL.GetDashBoardData(FromDate,  ToDate);
        }
        public async Task<CancelSaleBillrequisitionModel> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            return await _CancelSaleBillrequisitionDAL.GetDashBoardDetailData( FromDate,  ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, string CanReqNo, string MachineName, int CanSaleBillReqYC)
        {
            return await _CancelSaleBillrequisitionDAL.DeleteByID( ID,  CanReqNo,  MachineName,  CanSaleBillReqYC);

        }
        public async Task<CancelSaleBillrequisitionModel> GetViewByID(string CanRequisitionNo, int CanSaleBillReqYearcode)
        {
            return await _CancelSaleBillrequisitionDAL.GetViewByID( CanRequisitionNo,  CanSaleBillReqYearcode);
        }
    }
}
