using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface  IDiscountCustomerCategoryMaster
    {
		Task<ResponseResult> FillEntryID(int YearCode);
	}
}
