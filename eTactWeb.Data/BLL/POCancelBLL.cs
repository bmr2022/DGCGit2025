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
    public class POCancelBLL:IPOCancel
    {
        private readonly POCancelDAL _POCancelDAL;
        private readonly IDataLogic _DataLogicDAL;
        public POCancelBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _POCancelDAL = new POCancelDAL(configuration, iDataLogic);
            _DataLogicDAL = iDataLogic;
        }
    }
}
