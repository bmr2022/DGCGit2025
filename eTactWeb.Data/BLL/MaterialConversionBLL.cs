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
    public class MaterialConversionBLL: IMaterialConversion
    {
        private MaterialConversionDAL _MaterialConversionDAL;
        private readonly IDataLogic _DataLogicDAL;

        public MaterialConversionBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _MaterialConversionDAL = new MaterialConversionDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
    }
}
