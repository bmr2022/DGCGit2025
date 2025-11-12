using System.Globalization;
using System.Net;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebAdmin.Controllers
{
    public class DeleteTransactionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IDeleteTransaction _IDeleteTransaction;
        private readonly ILogger<DeleteTransactionController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;

        public DeleteTransactionController(ILogger<DeleteTransactionController> logger, IDataLogic iDataLogic, IDeleteTransaction iDeleteTransaction, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IDeleteTransaction = iDeleteTransaction;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
        }
        public IActionResult DeleteTransaction()
        {
            var model = new DeleteTransactionModel();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetFormName()
        {
            try
            {
                var result = await _IDeleteTransaction.GetFormName("GetFormName");
                string jsonString = JsonConvert.SerializeObject(result);
                return Json(jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while Get formname list.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSlipNoData(string MainTableName)
        {
            try
            {
                var result = await _IDeleteTransaction.GetSlipNoData("GetSlipNoData", MainTableName);
                // Manually serialize the result
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling slip number date.");
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTranscation(DeleteTransactionModel model)
        {
            try
            {
                // 🔹 Session and system info
                model.CC = HttpContext.Session.GetString("Branch");
                model.ActionByEmpId = Convert.ToInt64(HttpContext.Session.GetString("UID") ?? "0");
                model.MachineName = Environment.MachineName;
                model.IPAddress = HttpContext.Session.GetString("ClientIP");
                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

                // 🔹 Step 1: Perform DELETE
                model.Flag = "DELETE";
                var deleteResult = await _IDeleteTransaction.InsertAndDeleteTransaction(model);

                if (deleteResult != null)
                {
                    // 🔹 If DELETE SP returns success
                    if ((deleteResult.StatusText?.ToLower() == "deleted" || deleteResult.StatusText?.ToLower() == "success") &&
                        deleteResult.StatusCode == HttpStatusCode.OK)
                    {
                        // Step 2: Log INSERT
                        model.Flag = "Insert";
                        var insertResult = await _IDeleteTransaction.InsertAndDeleteTransaction(model);

                        if (insertResult != null &&
                            (insertResult.StatusText?.ToLower() == "success" || insertResult.StatusCode == HttpStatusCode.OK))
                        {
                            TempData["SuccessMessage"] = "Transaction deleted successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Transaction deleted, but failed to insert log entry.";
                        }
                    }
                    else if (!string.IsNullOrEmpty(deleteResult.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        TempData["ErrorMessage"] = deleteResult.StatusText;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error while deleting transaction.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No response from database.";
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("DeleteTransaction");
        }





    }

}
