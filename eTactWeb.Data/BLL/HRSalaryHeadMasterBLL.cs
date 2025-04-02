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

    public class HRSalaryHeadMasterBLL: IHRSalaryHeadMaster
    {
        private HRSalaryHeadMasterDAL _SalaryHeadMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        //private readonly IConfiguration configuration;

        public HRSalaryHeadMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            _SalaryHeadMasterDAL = new HRSalaryHeadMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> FillEntryId()
        {
            return await _SalaryHeadMasterDAL.FillEntryId();
        }
        public async Task<ResponseResult> GetTypeofSalaryHead()
        {
            return await _SalaryHeadMasterDAL.GetTypeofSalaryHead();
        }
        public async Task<ResponseResult> GetSalaryCalculationType()
        {
            return await _SalaryHeadMasterDAL.GetSalaryCalculationType();
        }
        public async Task<ResponseResult> GetPartOf()
        {
            return await _SalaryHeadMasterDAL.GetPartOf();
        }
        public async Task<ResponseResult> GetSalaryPaymentMode()
        {
            return await _SalaryHeadMasterDAL.GetSalaryPaymentMode();
        }

        public async Task<ResponseResult> GetCurrency()
        {
            return await _SalaryHeadMasterDAL.GetCurrency();
        }
        public async Task<ResponseResult> GetAmountPercentageOfCalculation()
        {
            return await _SalaryHeadMasterDAL.GetAmountPercentageOfCalculation();
        }
        public async Task<ResponseResult> GetYesOrNo()
        {
            return await _SalaryHeadMasterDAL.GetYesOrNo();
        }
        public async Task<ResponseResult> GetRoundOff()
        {
            return await _SalaryHeadMasterDAL.GetRoundOff();
        }
        public async Task<ResponseResult> GetPaymentFrequency()
        {
            return await _SalaryHeadMasterDAL.GetPaymentFrequency();
        }
        public async Task<ResponseResult> GetDeductionOfTax()
        {
            return await _SalaryHeadMasterDAL.GetDeductionOfTax();
        }

        public async Task<DataSet> GetEmployeeCategory()
        {
            return await _SalaryHeadMasterDAL.GetEmployeeCategory();
        }
        public async Task<DataSet> GetDepartment()
        {
            return await _SalaryHeadMasterDAL.GetDepartment();
        }
        public async Task<ResponseResult> GetFormRights(int uId)
        {
            return await _SalaryHeadMasterDAL.GetFormRights(uId);
        }
        public async Task<ResponseResult> SaveData(HRSalaryHeadMasterModel model, DataTable HRSalaryMasterDT, DataTable HRSalaryMasterDeptWiseDT)
        {
            return await _SalaryHeadMasterDAL.SaveData(model, HRSalaryMasterDT,  HRSalaryMasterDeptWiseDT);
        }

       

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _SalaryHeadMasterDAL.GetDashboardData();
        }
        public async Task<HRSalaryHeadMasterModel> GetDashboardDetailData()
        {
            return await _SalaryHeadMasterDAL.GetDashboardDetailData();
        }
        public async Task<HRSalaryHeadMasterModel> GetViewByID(int SalHeadEntryId)
        {
            return await _SalaryHeadMasterDAL.GetViewByID(SalHeadEntryId);
        }
        public async Task<ResponseResult> ChkForDuplicateHeadName(string SalaryHead, int SalHeadEntryId)
        {
            return await _SalaryHeadMasterDAL.ChkForDuplicateHeadName(SalaryHead,  SalHeadEntryId);
        }
        public async Task<ResponseResult> CheckBeforeDelete(int SalHeadEntryId)
        {
            return await _SalaryHeadMasterDAL.CheckBeforeDelete( SalHeadEntryId);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _SalaryHeadMasterDAL.DeleteByID(ID);
        }
    }
}
