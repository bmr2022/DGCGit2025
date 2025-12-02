using System.Data;
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
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.AdvanceYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _iemployeeAdvancePayement.GetViewByID(ID,YearCode, Mode).ConfigureAwait(false);
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
        public async Task<JsonResult> FillAdvanceType()
        {
            var JSON = await _iemployeeAdvancePayement.FillAdvanceType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPreviousAdanaceLoanDetail(int empId,string requestedDate)
        {
            var JSON = await _iemployeeAdvancePayement.FillPreviousAdanaceLoanDetail(empId,requestedDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEmpName()
        {
            var JSON = await _iemployeeAdvancePayement.FillEmpName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDeptName()
        {
            var JSON = await _iemployeeAdvancePayement.FillDeptName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFinancialEmployeeNameList()
        {
            var JSON = await _iemployeeAdvancePayement.FillFinancialEmployeeNameList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        
        public async Task<JsonResult> FillMgmtEmployeeNameList()
        {
            var JSON = await _iemployeeAdvancePayement.FillMgmtEmployeeNameList();
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
                            return RedirectToAction("Index", "EmployeeAdvancePayement");
                        }
                        else
                        {
                            TempData["422"] = Result.StatusText;
                            return RedirectToAction("Index", "EmployeeAdvancePayement");
                        }
                    }

                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        var modelUpdate = new HRAdvanceModel();
                        return RedirectToAction("EmployeeAdvancePayement", modelUpdate);
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

        public async Task<IActionResult> EAPDashboard(string fromDate, string toDate)
        {
            try
            {
                var model = new HRAdvanceDashboard();
                fromDate = fromDate ?? ParseFormattedDate(HttpContext.Session.GetString("FromDate"));
                toDate = toDate ?? ParseFormattedDate(HttpContext.Session.GetString("ToDate"));

                var Result = await _iemployeeAdvancePayement.GetDashboardData(fromDate,toDate).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true,
                                                                   "AdvanceEntryId",
                                                                   "AdvanceYearCode",
                                                                 "AdvanceSlipNo",
                                                                 "EmpId",
                                                                 "EmpCode",
                                                                 "EmpName",
                                                                 "DesigName",
                                                                 "DeptId",
                                                                 "DeptName",
                                                                 "RequestDate",
                                                                 "EntryDate",
                                                                 "RequestedAmount",
                                                                 "AdvanceType",
                                                                 "Purpose",
                                                                 "DOJ",
                                                                 "BasicSalary",
                                                                 "Grosssalary",
                                                                 "NetSalary",
                                                                 "PresentDaysinCurrMonth",
                                                                 "PresentDaysInCurrYear",
                                                                 "ApprovedAmount",
                                                                 "PreviousPendAdvanceAmt",
                                                                 "PreviousPendLoanAmt",
                                                                 "MgrApprovaldate",
                                                                 "ApprovByMgr",
                                                                 "ApprovByHR",
                                                                 "HRApprovedbyEmpid",
                                                                 "HRApprovalDate",
                                                                 "ApprByFinCode",
                                                                 "ApprovByFin",
                                                                 "CanceledByEmpId",
                                                                 "FinanceApprovalDate",
                                                                 "FinanceApprovalEmpid",
                                                                 "Canceled",
                                                                 "CancelOrApprovalremarks",
                                                                 "ModeOfPayment",
                                                                 "PaymentReferenceNo",
                                                                 "PaymentVoucherNo",
                                                                 "PaymentVoucherType",
                                                                 "PaymentRemark",
                                                                 "RecoveryMethod",
                                                                 "NoofInstallment",
                                                                 "StartRecoveryFromMonth",
                                                                 "AutoDeductionFromSalaryYN",
                                                                 "FinalRevoveryDate",
                                                                 "ActualFinalRecoveryDate",
                                                                 "StatusHoldCancelApproved",
                                                                 "LastUpdatedBy",
                                                                 "LastUpdationDate",
                                                                 "RequestEntryByMachine",
                                                                 "ApprovedBYMachine",
                                                                 "CancelByMachine",
                                                                 "ActualEntryByName",
                                                                 "CC"
                                                             );


                        model.HRAdvanceDashboards = CommonFunc.DataTableToList<HRAdvanceDashboard>(DT, "HRAdvanceDashboard");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> GetSearchData(string fromDate,string toDate,string employeeName, string departmentName, string employeeCode)
        {
            var model = new HRAdvanceDashboard();
            var Result = await _iemployeeAdvancePayement.GetDashboardData(fromDate,toDate,employeeName, departmentName, employeeCode).ConfigureAwait(true);
            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null)
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true,
                                                                    "AdvanceEntryId",
                                                                    "AdvanceYearCode",
                                                                  "AdvanceSlipNo",
                                                                  "EmpId",
                                                                  "EmpCode",
                                                                  "EmpName",
                                                                  "DesigName",
                                                                  "DeptId",
                                                                  "DeptName",
                                                                  "RequestDate",
                                                                  "EntryDate",
                                                                  "RequestedAmount",
                                                                  "AdvanceType",
                                                                  "Purpose",
                                                                  "DOJ",
                                                                  "BasicSalary",
                                                                  "Grosssalary",
                                                                  "NetSalary",
                                                                  "PresentDaysinCurrMonth",
                                                                  "PresentDaysInCurrYear",
                                                                  "ApprovedAmount",
                                                                  "PreviousPendAdvanceAmt",
                                                                  "PreviousPendLoanAmt",
                                                                  "MgrApprovaldate",
                                                                  "ApprovByMgr",
                                                                  "ApprovByHR",
                                                                  "HRApprovedbyEmpid",
                                                                  "HRApprovalDate",
                                                                  "ApprByFinCode",
                                                                  "ApprovByFin",
                                                                  "CanceledByEmpId",
                                                                  "FinanceApprovalDate",
                                                                  "FinanceApprovalEmpid",
                                                                  "Canceled",
                                                                  "CancelOrApprovalremarks",
                                                                  "ModeOfPayment",
                                                                  "PaymentReferenceNo",
                                                                  "PaymentVoucherNo",
                                                                  "PaymentVoucherType",
                                                                  "PaymentRemark",
                                                                  "RecoveryMethod",
                                                                  "NoofInstallment",
                                                                  "StartRecoveryFromMonth",
                                                                  "AutoDeductionFromSalaryYN",
                                                                  "FinalRevoveryDate",
                                                                  "ActualFinalRecoveryDate",
                                                                  "StatusHoldCancelApproved",
                                                                  "LastUpdatedBy",
                                                                  "LastUpdationDate",
                                                                  "RequestEntryByMachine",
                                                                  "ApprovedBYMachine",
                                                                  "CancelByMachine",
                                                                  "ActualEntryByName",
                                                                  "CC"
                                                              );


                    model.HRAdvanceDashboards = CommonFunc.DataTableToList<HRAdvanceDashboard>(DT, "HRAdvanceDashboard");
                }
            }
            return PartialView("_EAPDashboardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int advEntryId, int advYearCode, string entryByMachineName, string entryDate, string fromDate, string toDate, string empName,string deptName,string searchBox)
        {
            var actualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            entryDate = ParseFormattedDate(entryDate);
            var Result = await _iemployeeAdvancePayement.DeleteByID(advEntryId, advYearCode, actualEntryBy, entryByMachineName, entryDate).ConfigureAwait(false);

            if (Result.StatusText == "Success" || Result.StatusText == "deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                //  TempData["423"] = "423";
                TempData["DeleteMessage"] = Result.StatusText;

            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";

            }

            return RedirectToAction("EAPDashboard");
        }
    }
}
