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
    public class RecChallanReportBLL : IRecChallanReport
    {
        private RecChallanReportDAL _RecChallanReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public RecChallanReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _RecChallanReportDAL = new RecChallanReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<RecChallanReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate, int EntryId, int YearCode)
        {
            return await _RecChallanReportDAL.GetInventoryAgingReportDetailsData( fromDate,  toDate,  EntryId,  YearCode);
        }
    }
   
}
