using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class ConsumptionReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IConsumptionReport _IConsumptionReport { get; }
        private readonly ILogger<ConsumptionReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ConsumptionReportController(ILogger<ConsumptionReportController> logger, IDataLogic iDataLogic, IConsumptionReport iConsumptionReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IConsumptionReport = iConsumptionReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [HttpGet("{controller}/Index")]
        public async Task<ActionResult> ConsumptionReport()
        {
            var model = new ConsumptionReportModel();
            model.ConsumptionReportGrid = new List<ConsumptionReportModel>();
            //model.YearCode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model = await BindModel(model).ConfigureAwait(false);
            model = await BindModel1(model).ConfigureAwait(false);
            return View(model);
        }
        private async Task<ConsumptionReportModel> BindModel(ConsumptionReportModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IConsumptionReport.GetCategory().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Entry_Id"].ToString(),
                        Text = row["ItemCategory"].ToString()
                    });
                }
                model.CategList = _List;
                _List = new List<TextValue>();

            }


            return model;
        }
        private async Task<ConsumptionReportModel> BindModel1(ConsumptionReportModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IConsumptionReport.GetGroupName().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Group_Code"].ToString(),
                        Text = row["ParentGroup"].ToString()
                    });
                }
                model.GroupNameList = _List;
                _List = new List<TextValue>();

            }


            return model;
        }

        public async Task<JsonResult> FillFGItemName()
        {
            var JSON = await _IConsumptionReport.FillFGItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFGPartCode()
        {
            var JSON = await _IConsumptionReport.FillFGPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName()
        {
            var JSON = await _IConsumptionReport.FillRMItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode()
        {
            var JSON = await _IConsumptionReport.FillRMPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IConsumptionReport.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName()
        {
            var JSON = await _IConsumptionReport.FillWorkCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetConsumptionDetailsData(string fromDate, string toDate, int WorkCenterid, string ReportType, int FGItemCode, int RMItemCode, int Storeid, string GroupName, string ItemCateg)
        {
            var model = new ConsumptionReportModel();
            model = await _IConsumptionReport
                .GetConsumptionDetailsData(fromDate, toDate, WorkCenterid, ReportType, FGItemCode, RMItemCode, Storeid,  GroupName,  ItemCateg);

            if (ReportType == "ProductionConsumptionReport(SUMMARY)")
            {
                return PartialView("_ConsumptionReportSummaryGrid", model);
            }
            if (ReportType == "ProductionConsumptionReport(DETAIL)")
            {
                return PartialView("_ConsumptionReportDetailGrid", model);
            }
            if (ReportType == "ProductionConsumptionReport(CONSOLIDATED)")
            {
                return PartialView("_ConsumptionReportCONSOLIDATEDGrid", model);
            }
            if (ReportType == "COMPACT")
            {
                return PartialView("_ConsumptionReportCompact", model);
            }
            if (ReportType == "Prod Date Wise Consumption")
            {
                return PartialView("_ConsumptionReportProdDateWiseConsumption", model);
            }

            return null;
        }
    }
}
