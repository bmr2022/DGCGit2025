using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingMRNToQC
    {
        Task<DataSet> BindData(string Flag);

        Task<ResponseResult> GetDataForPendingMRN(string Flag,string MRNJW, int YearCode, string FromDate, string ToDate, int AccountCode, string MrnNo, int ItemCode, string InvoiceNo,int DeptId);
        Task<ResponseResult> GetDeptForUser(int Empid);


    }   
}
