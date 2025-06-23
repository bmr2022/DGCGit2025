using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CancelSaleBillBLL : ICancelSaleBill
    {
        private readonly CancelSaleBillDAL _CancelSaleBillDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CancelSaleBillBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CancelSaleBillDAL = new CancelSaleBillDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string SaleBillNo, string CustomerName, string CanRequisitionNo)
        {
            return await _CancelSaleBillDAL.GetSearchData(FromDate, ToDate, SaleBillNo, CustomerName, CanRequisitionNo);
        }
        public async Task<ResponseResult> FillCanRequisitionNo(string CurrentDate, int accountcode, string SaleBillNo)
        {
            return await _CancelSaleBillDAL.FillCanRequisitionNo(CurrentDate, accountcode, SaleBillNo);
            
        }
        public async Task<ResponseResult> FillSaleBillNo(string CurrentDate, int accountcode)
        {
            return await _CancelSaleBillDAL.FillSaleBillNo(CurrentDate, accountcode);

        }
        public async Task<ResponseResult> FillCustomerName(string CurrentDate, string SaleBillNo)
        {
            return await _CancelSaleBillDAL.FillCustomerName(CurrentDate, SaleBillNo);

        }
        public async Task<List<CancelSaleBillDetails>> ShowSaleBillDetail(int SaleBillEntryId, int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo)
        {
            return await _CancelSaleBillDAL.ShowSaleBillDetail(SaleBillEntryId,SaleBillYearCode, CanSaleBillReqYearcode, CanRequisitionNo, SaleBillNo);
        }

        public async Task<ResponseResult> SaveCancelation(int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo, int CancelBy, String Cancelreason, String Canceldate)
        {
            // Implementation for SaveCancelation
            // Assuming _CancelSaleBillDAL has a method to handle this functionality
            return await _CancelSaleBillDAL.SaveCancelation(SaleBillYearCode, CanSaleBillReqYearcode, CanRequisitionNo, SaleBillNo, CancelBy, Cancelreason, Canceldate);
        }
      
        public async Task<ResponseResult> CancelEInvoice(int SaleBillYearCode, string CanRequisitionNo, string SaleBillNo, string CustomerName)
        {
            var result = new ResponseResult();
            var dataResult = await _CancelSaleBillDAL.GetInvoiceDataAsync(SaleBillNo, SaleBillYearCode);
            if (dataResult?.Result == null || dataResult.Result.Rows.Count == 0)
            {
                return result;
            }
            string irn = dataResult.Result.Rows[0]["IRnno"]?.ToString();
            string gstin = dataResult.Result.Rows[0]["GSTNO"]?.ToString();
            result = await _CancelSaleBillDAL.CancelEInvoiceAsync(irn, gstin, SaleBillNo, SaleBillYearCode);
            return result;

        }

    }

}
