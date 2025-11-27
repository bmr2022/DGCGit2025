using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using DocumentFormat.OpenXml.EMMA;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using PdfSharp.Drawing.BarCodes;
using static eTactWeb.DOM.Models.Common;
using System.Data;
namespace eTactwebAdmin.Controllers
{
    public class SaleBillApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleBillApproval _ISaleBillApproval { get; }
        private readonly ILogger<SaleBillApprovalController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SaleBillApprovalController(ILogger<SaleBillApprovalController> logger, IDataLogic iDataLogic, ISaleBillApproval iSaleBillApproval, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleBillApproval = iSaleBillApproval;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        public async Task<IActionResult> SaleBillApproval()
        {
            var model = new SaleBillApprovalModel();

            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            model.ActualEntryId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            model.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

            var result = await _ISaleBillApproval
                .GetPendingSaleBillSummary(model.FromDate, model.ToDate, "EntryFromCounter");

            if (result?.Result != null)
            {
                // Directly cast the Result to the correct type
                var list = result.Result as List<SaleBillApprovalDashboard>;

                if (list != null)
                {
                    model.SaleBillList = list
                        .GroupBy(x => x.SaleBillEntryId)
                        .Select(g => g.First())
                        .ToList();
                }
            }

            return View(model);
        }


    }
}
