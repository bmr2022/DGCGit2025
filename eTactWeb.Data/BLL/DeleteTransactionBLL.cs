using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class DeleteTransactionBLL : IDeleteTransaction
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly DeleteTransactionDAL _DeleteTransactionDAL;

        public DeleteTransactionBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _DeleteTransactionDAL = new DeleteTransactionDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> GetFormName(string Flag, string ModuleName)
        {
            return await _DeleteTransactionDAL.GetFormData(Flag, ModuleName);
        }

        public async Task<ResponseResult> GetModuleName(string Flag)
        {
            return await _DeleteTransactionDAL.GetModuleName(Flag);
        }


        public async Task<ResponseResult> GetSlipNoData(string Flag, string MainTableName)
        {
            return await _DeleteTransactionDAL.GetSlipNoData(Flag, MainTableName);
        }

        public async Task<ResponseResult> InsertAndDeleteTransaction(DeleteTransactionModel model)
        {
            return await _DeleteTransactionDAL.InsertAndDeleteTransaction(model);
        }

        public async Task<ResponseResult> UpdateExistingSlipNo(DeleteTransactionModel model)
        {
            return await _DeleteTransactionDAL.UpdateExistingSlipNo(model);
        }
    }


}
