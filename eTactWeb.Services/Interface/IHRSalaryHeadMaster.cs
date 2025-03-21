using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRSalaryHeadMaster
    {
        Task<ResponseResult> FillEntryId();
        Task<ResponseResult> GetTypeofSalaryHead();
        Task<ResponseResult> GetSalaryCalculationType();
        Task<ResponseResult> GetPartOf();
        Task<ResponseResult> GetSalaryPaymentMode();
        Task<ResponseResult> GetCurrency();
        Task<ResponseResult> GetAmountPercentageOfCalculation();
        Task<ResponseResult> GetYesOrNo();
        Task<ResponseResult> GetRoundOff();
        Task<ResponseResult> GetPaymentFrequency();
        Task<ResponseResult> GetDeductionOfTax();
        Task<DataSet> GetEmployeeCategory();
        Task<DataSet> GetDepartment();
        Task<ResponseResult> GetFormRights(int uId);
        
        Task<ResponseResult> GetDashboardData();
        Task<HRSalaryHeadMasterModel> GetDashboardDetailData();

        Task<HRSalaryHeadMasterModel> GetViewByID(int SalHeadEntryId);
        //Task<IList<HRSalaryHeadMasterModel>> GetSearchData(string SalaryHead, string SalaryCode, string TypeOfSalary);


        Task<ResponseResult> SaveData(HRSalaryHeadMasterModel model, DataTable HRSalaryMasterDT, DataTable HRSalaryMasterDeptWiseDT);

        Task<ResponseResult> ChkForDuplicateHeadName(string SalaryHead,int SalHeadEntryId);
        Task<ResponseResult> CheckBeforeDelete(int SalHeadEntryId);
        Task<ResponseResult> DeleteByID(int ID);
    }
}
