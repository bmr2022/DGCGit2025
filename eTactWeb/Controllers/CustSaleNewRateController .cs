using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eTactWeb.Controllers
{
    public class CustSaleNewRateController : Controller
    {
        private readonly ICustSaleNewRate _custSaleNewRate;

        public CustSaleNewRateController(ICustSaleNewRate custSaleNewRate)
        {
            _custSaleNewRate = custSaleNewRate;
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _custSaleNewRate.GetCustomerListAsync();
            var partCodes = await _custSaleNewRate.GetPartCodeListAsync(true);

            ViewBag.CustomerList = customers.Tables[0]
                .AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["Account_Code"].ToString(), 
                    Text = row["Account_Name"].ToString()
                }).ToList();

            ViewBag.PartCodeList = partCodes.Tables[0]
                .AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["ItemCode"].ToString(), 
                    Text = row["PartCode"].ToString()
                }).ToList();

            return View();
        }

    }
}
