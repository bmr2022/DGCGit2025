using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.BLL
{
    public class CancelRequitionBLL:ICancelRequition
    {
        private readonly CancelRequitionDAL _CancelRequitionDAL;
        private readonly IDataLogic _DataLogicDAL;
        public CancelRequitionBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _CancelRequitionDAL = new CancelRequitionDAL(configuration, iDataLogic, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }
    }
}
