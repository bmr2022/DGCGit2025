using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using DocumentFormat.OpenXml.EMMA;
namespace eTactwebAdmin.Controllers
{
    public class SaleBillApprovalController : Controller
    {

        public IActionResult SaleBillApproval()
        {
            var MainModel = new SaleBillApprovalModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            MainModel.ActualEntryId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
            MainModel.SaleBillList = new List<SaleBillApprovalDashboard>();

            return View(MainModel);
        }
    }
}
