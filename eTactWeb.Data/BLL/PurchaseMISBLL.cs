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
    public class PurchaseMISBLL: IPurchaseMIS
    {
        private PurchaseMISDAL _PurchaseMISDAL;
        private readonly IDataLogic _DataLogicDAL;

        public PurchaseMISBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _PurchaseMISDAL = new PurchaseMISDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillItemName()
        {
            return await _PurchaseMISDAL.FillItemName();
        }
        public async Task<ResponseResult> FillPartCode()
        {
            return await _PurchaseMISDAL.FillPartCode();
        }
        public async Task<ResponseResult> FillAccountName()
        {
            return await _PurchaseMISDAL.FillAccountName();
        }
        public async Task<PurchaseMISModel> GetPurchaseMISDetailsData(string ReportType, string ToDate, int YearCode, int Itemcode, int AccountCode)
        {
            return await _PurchaseMISDAL.GetPurchaseMISDetailsData(ReportType, ToDate, YearCode, Itemcode, AccountCode);
        }
    }
}
