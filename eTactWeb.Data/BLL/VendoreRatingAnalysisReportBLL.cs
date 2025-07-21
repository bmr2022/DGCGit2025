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

namespace eTactWeb.Data.BLL
{
    public class VendoreRatingAnalysisReportBLL:IVendoreRatingAnalysisReport
    {
        private VendoreRatingAnalysisReportDAL _VendoreRatingAnalysisReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public VendoreRatingAnalysisReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _VendoreRatingAnalysisReportDAL = new VendoreRatingAnalysisReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<VendoreRatingAnalysisReportModel> GetVendoreRatingDetailsData(string ReportType, string RatingType, string CurrentDate, string PartCode, string ItemName, string CustomerName, int YearCode)
        {
            return await _VendoreRatingAnalysisReportDAL.GetVendoreRatingDetailsData(ReportType, RatingType, CurrentDate, PartCode, ItemName, CustomerName, YearCode);
        }
    }
}
