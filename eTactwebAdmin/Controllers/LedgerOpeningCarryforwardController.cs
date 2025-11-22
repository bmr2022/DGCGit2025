using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebAdmin.Controllers
{
    public class LedgerOpeningCarryforwardController : Controller
    {

        private readonly IDataLogic _IDataLogic;
        public ILedgerOpeningCarryforward _ILedgerOpeningCarryforward { get; }
        private readonly ILogger<LedgerOpeningCarryforwardController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public LedgerOpeningCarryforwardController(ILogger<LedgerOpeningCarryforwardController> logger, IDataLogic iDataLogic, ILedgerOpeningCarryforward iLedgerOpeningCarryforward, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ILedgerOpeningCarryforward = iLedgerOpeningCarryforward;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [HttpGet]
        public async Task<IActionResult> LedgerOpeningDashboard()
        {
            try
            {
                var model = new LedgerOpeningCarryforwardModel();
                model.LoggedInFinYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                //model.DashboardType = "SUMMARY";
                var Result = await _ILedgerOpeningCarryforward.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;


                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryByEmpId",
                            "GroupName", "LedgerName", "ClosingYearCode", "AccountCode", "DrCr",
                            "Amount", "CC", "EntryByEmployee", "ActualEntryDate",
                            "UpdatedByEmployee", "GroupAccountCode", "Updationdate", "EntryByMachine");

                        model.LedgerOpeningCarryforwardDashBoardGrid = new List<LedgerOpeningCarryforwardDashBoardGridModel>();
                        foreach (DataRow row in DT.Rows)
                        {
                            model.LedgerOpeningCarryforwardDashBoardGrid.Add(new LedgerOpeningCarryforwardDashBoardGridModel
                            {
                                EntryByEmpId = Convert.ToInt32(row["EntryByEmpId"]),
                                GroupName = row["GroupName"].ToString(),
                                LedgerName = row["LedgerName"].ToString(),
                                ClosingYearCode = row["ClosingYearCode"] == DBNull.Value ? null : Convert.ToInt32(row["ClosingYearCode"]),
                                AccountCode = Convert.ToInt32(row["AccountCode"]),
                                DrCr = row["DrCr"].ToString(),
                                Amount = row["Amount"] == DBNull.Value ? 0 : Convert.ToSingle(row["Amount"]),
                                CC = row["CC"].ToString(),
                                EntryByEmployee = row["EntryByEmployee"].ToString(),
                                ActualEntryDate = row["ActualEntryDate"].ToString(),
                                UpdatedByEmployee = row["UpdatedByEmployee"].ToString(),
                                GroupAccountCode = Convert.ToInt32(row["GroupAccountCode"]),
                                Updationdate = row["Updationdate"].ToString(),
                                EntryByMachine = row["EntryByMachine"].ToString()
                            });
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> FILLFINYEAR()
        {
            try
            {
                var result = await _ILedgerOpeningCarryforward.FILLFINYEAR();

                List<object> yearList = new List<object>();

                if (result != null && result.Result != null && result.Result.Tables.Count > 0)
                {
                    DataTable dt = result.Result.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        yearList.Add(new
                        {
                            Year = dt.Rows[i]["Year"].ToString()
                        });
                    }
                }

                return Json(new { Result = yearList });
            }
            catch (Exception ex)
            {
                return Json(new { Result = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CarryForwardLedgerBalance(int CarryForwardYear)
        {
            try
            {
                var model = new LedgerOpeningCarryforwardModel
                {
                    CC = HttpContext.Session.GetString("Branch") ?? "",
                    ActualTransferBy = Convert.ToInt32(HttpContext.Session.GetString("UID") ?? "0"),
                    EntryByMachine = Environment.MachineName,
                    CarryForwardClosing = CarryForwardYear,
                    ActualTransferDate = DateTime.Now
                };

                var result = await _ILedgerOpeningCarryforward.CarryforwardLedgerbalance(model);

                string message = "Unknown error!";
                bool success = false;

                if (result != null && result.Result != null && result.Result.Tables.Count > 0)
                {
                    var dt = result.Result.Tables[1];
                    if (dt.Rows.Count > 0)
                    {
                        var status = dt.Rows[0]["status"].ToString();
                        if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        {
                            message = "Carry Forward Successful!";
                            success = true;
                        }
                        else
                        {
                            message = status; // message from SP
                        }
                    }
                    else
                    {
                        message = "No result returned from SP!";
                    }
                }
                else
                {
                    message = "Error calling SP!";
                }

                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CarryForwardLedgerBalance");
                return Json(new { success = false, message = "Unexpected Error!" });
            }
        }


    }
}
