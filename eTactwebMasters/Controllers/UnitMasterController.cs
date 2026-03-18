using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using PdfSharp.Drawing.BarCodes;

namespace eTactWeb.Controllers
{
    public class UnitMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public EncryptDecrypt EncryptDecrypt { get; }
        public IUnitMaster _IUnitMaster { get; }
        private readonly ILogger<UnitMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public UnitMasterController(ILogger<UnitMasterController> logger, IDataLogic iDataLogic, IUnitMaster iUnitMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IUnitMaster = iUnitMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            EncryptDecrypt = encryptDecrypt;
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IUnitMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/UnitMaster")]
        [HttpGet]
        public async Task<ActionResult> UnitMaster(string Unit_Name, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            //TempData.Clear();
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _IUnitMaster.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }


            var table = rights.Result.Tables[0];

            if (!string.IsNullOrEmpty(Unit_Name) && (!string.IsNullOrEmpty(Unit_Name)))
            {
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);
                string decryptedUnitName = EncryptDecrypt.Decrypt(Unit_Name);
                Mode = decryptedMode;
                Unit_Name = decryptedUnitName;


            }

            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            //else if (ID <= 0)
            //{
            //    if (!optSave)
            //    {
            //        return RedirectToAction("UnitMasterDashBoard", "UnitMaster");
            //    }
            //    //if (!(optAll || optSave))
            //    //{
            //    //    return RedirectToAction("Dashboard", "Home");
            //    //}

            //}

            var MainModel = new UnitMasterModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EntryDate = HttpContext.Session.GetString("EntryDate");

            if (!string.IsNullOrEmpty(Mode) && Unit_Name != " " && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IUnitMaster.GetViewByID(Unit_Name).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.Unit_Name = Unit_Name;

                MainModel.PrevUnitName = Unit_Name;


                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            return View(MainModel);
        }

        [Route("{controller}/UnitMaster")]
        [HttpPost]
        public async Task<IActionResult> UnitMaster(UnitMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.CC = HttpContext.Session.GetString("Branch");
                var Result = await _IUnitMaster.SaveUnitMaster(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        return Json(new
                        {
                            success = true,
                            message = "Data saved successfully",
                            redirectUrl = Url.Action(
                                    "UnitMasterDashBoard",
                                    "UnitMaster"

                            )
                        });
                        //_MemoryCache.Remove("KeyLedgerOpeningEntryGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        return Json(new
                        {
                            success = true,
                            message = "Data saved successfully",
                            redirectUrl = Url.Action(
                                        "UnitMasterDashBoard",
                                        "UnitMaster"
                                      )
                        });
                    }

                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return Json(new
                        {
                            success = false,
                            message = "An unexpected error occurred."
                        });
                        //return View("Error", Result);
                    }
                    else if (Result.StatusText == "Error" && ((int)Result.StatusCode == 423))
                    {
                        ViewBag.isSuccess = true;
                        string message = "This unit is already in use. You cannot update it.";


                        return Json(new
                        {
                            success = false,
                            message = message
                        });


                    }
                    else if (!string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });

                        //return View(model);
                    }
                    else
                    {
                        ViewBag.isSuccess = false;
                        TempData["Message"] = "Form Validation Error.";
                        return Json(new
                        {
                            success = false,
                            message = "An unexpected error occurred."
                        });

                        //  return RedirectToAction(nameof(Form), new { ID = 0 });
                    }


                }
                else
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Form Validation Error.";
                    return Json(new
                    {
                        success = false,
                        message = "An unexpected error occurred."
                    });
                }


                // return RedirectToAction(nameof(UnitMasterDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<UnitMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UnitMasterDashBoard()
        {
            try
            {

                int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                var rights = await _IUnitMaster.GetFormRights(userID);
                if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var table = rights.Result.Tables[0];

                bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
                bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
                bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
                if (!(optAll || optView || optUpdate || optDelete))
                {
                    return RedirectToAction("Dashboard", "Home");
                }


                var model = new UnitMasterModel();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IUnitMaster.GetDashBoardData(userID).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.UnitMasterDashBoardGrid = CommonFunc.DataTableToList<UnitMasterModel>(dt, "UnitMasterDashBoard");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetDashBoardDetailData()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var model = new UnitMasterModel();
            model = await _IUnitMaster.GetDashBoardDetailData(userID);
            return PartialView("_UnitMasterDashBoardGrid", model);
        }


        public async Task<IActionResult> DeleteByID(String Unit_Name)
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _IUnitMaster.DeleteByID(Unit_Name, userID);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";


                //TempData["Message"] = "Data deleted successfully.";
            }
            else if (!string.IsNullOrEmpty(Result.StatusText))
            {
                // If SP returned a message (like adjustment error)
                ViewBag.isSuccess = false;
                TempData["ErrorMessage"] = Result.StatusText;

            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("UnitMasterDashBoard");

        }
    }
}

