using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IInProcessInspection
    {
		Task<ResponseResult> FillPartCode(string InspectionType);
		Task<ResponseResult> FillItemName();
		Task<ResponseResult> FillMachineName();
		Task<ResponseResult> FillCustomer();
		Task<ResponseResult> FillColor(string PartNo);
	}
}
