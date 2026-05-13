using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IImportBillsFromExcel
    {
        Task<ResponseResult> ImportBills(string ReportType, int ForFinYear, int CreatedBy);

    }
}
