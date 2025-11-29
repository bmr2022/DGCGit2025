using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISaleBillRegister
    { 
        //3--//1
        Task<SaleBillRegisterModel> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate,  string docname, string SONo, string Schno, string PartCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO ,string GSTNO);
      
        Task<ResponseResult> FillCustomerList(string FromDate, string ToDate);//4
        Task<ResponseResult> FillDocumentList(string FromDate, string ToDate);//5
        Task<ResponseResult> FillSaleBillList(string FromDate, string ToDate);//8
        Task<ResponseResult> FillItemNamePartcodeList(string FromDate, string ToDate);//1
        Task<ResponseResult> FillSONO(string FromDate, string ToDate);//9
        Task<ResponseResult> FillSchNo(string FromDate, string ToDate);//2
        Task<ResponseResult> FillHSNNo(string FromDate, string ToDate);//7
        Task<ResponseResult> FillGSTNo(string FromDate, string ToDate);//6
    }
}
