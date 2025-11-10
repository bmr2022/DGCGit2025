using System.Net;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebHR.Controllers
{
    public class EmployeeAdvancePayementController : Controller
    {
        private readonly IEmployeeAdvancePayement _iemployeeAdvancePayement;
        private readonly ILogger<EmployeeAdvancePayementController> _logger;
        public EmployeeAdvancePayementController(ILogger<EmployeeAdvancePayementController> logger, IEmployeeAdvancePayement IEmployeeAdvancePayement)
        {
            _logger = logger;
            _iemployeeAdvancePayement = IEmployeeAdvancePayement;
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> EmployeeAdvancePayment(int ID, int YearCode, string Mode)
        {
            HttpContext.Session.Remove("EmployeeAdvancePayement");
            var MainModel = new HRAdvanceModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.AdvanceYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel.Mode = Mode;
                MainModel.ID = ID;
               
                string serializedGrid = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("EmployeeAdvancePayement", serializedGrid);
            }
            

            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedOn = DateTime.Now;
            }

            string serializedGateAttendance = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("EmployeeAdvancePayement", serializedGateAttendance);

            return View(MainModel);
        }

        public async Task<JsonResult> FillEntryId()
        {
            var advanceYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _iemployeeAdvancePayement.FillEntryId(advanceYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEmpName()
        {
            var JSON = await _iemployeeAdvancePayement.FillEmpName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        
        public async Task<JsonResult> FillEmployeeCode()
        {
            var JSON = await _iemployeeAdvancePayement.FillEmpCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillEmployeeDetail(int empId)
        {
            var JSON = await _iemployeeAdvancePayement.FillEmployeeDetail(empId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> EmployeeAdvancePayment(HRAdvanceModel model)
        {
            try
            {
                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                if (model.Mode == "U")
                {
                    model.LastUpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                    model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                }
                var Result = await _iemployeeAdvancePayement.SaveEmployeeAdvancePayment(model);

                if (Result != null)
                {
                    if (Result.StatusCode == HttpStatusCode.OK)
                    {
                        if (Result.StatusText == "Success")
                        {
                            TempData["Message"] = "Vendor created successfully!";
                            return RedirectToAction("Index", "VendorUser");
                        }
                        else
                        {
                            TempData["422"] = Result.StatusText;
                            return RedirectToAction("Index", "VendorUser");
                        }
                    }

                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        var modelUpdate = new VendorUserModel();
                        return RedirectToAction("VendorUser", modelUpdate);
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        var input = "";
                        if (Result != null)
                        {
                            input = Result.Result.ToString();
                            int index = input.IndexOf("#ERROR_MESSAGE");

                            if (index != -1)
                            {
                                string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                TempData["ErrorMessage"] = errorMessage;
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }
                        }
                        else
                        {
                            TempData["500"] = "500";
                        }

                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

                        return View(model);
                    }

                    ModelState.Clear();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<EmployeeAdvancePayementController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };


                return View("Error", ResponseResult);
            }

            return View(model);
        }
    }
}
