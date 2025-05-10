using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    public class DeassembleItemBLL:IDeassembleItem
    {
        private DeassembleItemDAL? _IDeassembleItemDAL { get; }
        public DeassembleItemBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDeassembleItemDAL = new DeassembleItemDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> NewEntryId()
        {
            return await _IDeassembleItemDAL.NewEntryId();
        }

        public async Task<ResponseResult> FillStore()
        {
            return await _IDeassembleItemDAL.FillStore();
        }

    }
}
