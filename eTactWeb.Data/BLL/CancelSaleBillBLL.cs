using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
     
        //public async Task<ResponseResult> SaveCancelation(int EntryId, int YC, string SaleBillNo, string type, int EmpID)
        //{
        //    // Implementation for SaveCancelation
        //    // Assuming _CancelSaleBillDAL has a method to handle this functionality
        //    return await _CancelSaleBillDAL.SaveCancelation(EntryId, YC, SaleBillNo, type, EmpID);
        //}


    }
}
