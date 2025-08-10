using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using System.Net;
using static eTactWeb.DOM.Models.Common;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.SqlClient;
using System.Dynamic;

namespace eTactwebMasters.Controllers
{
    public class XONUserRightDashboardController : Controller
    {
        private readonly IXONUserRightDashboardBLL _IXONUserRightDashboardBLL;
        private readonly IDataLogic _IDataLogic;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ILogger<XONUserRightDashboardController> _logger;
        private dynamic Result;
        public XONUserRightDashboardController(ILogger<XONUserRightDashboardController> logger, IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, IXONUserRightDashboardBLL IXONUserRightDashboardBLL)
        {
            _IDataLogic = iDataLogic;
            _logger = logger;
            _IWebHostEnvironment = iWebHostEnvironment;
            _IXONUserRightDashboardBLL = IXONUserRightDashboardBLL;
        }
        public async Task<IActionResult> XONUserRightDashboard(int ID, string Mode, string userName = "")
        {
            UserRightDashboardModel model = new();
            if (Mode == "C")
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                List<UserRightDashboardModel> UserRightDashboardModelDetail = new List<UserRightDashboardModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardModelDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }
                model.UserRightsDashboard = UserRightDashboardModelDetail;
            }
            else
            {
                HttpContext.Session.Remove("KeyUserRightsDetail");
                model.CreatedById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.CreatedOn = DateTime.Now;
                model.UserRightsDashboard = new List<UserRightDashboardModel>();
            }

            if (ID != 0 || (Mode == "V" || Mode == "U" || Mode == "C"))
            {
                model.Mode = Mode;
                model.UpdatedById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.UpdatedOn = DateTime.Now;
            }

            model.UserList = await _IXONUserRightDashboardBLL.GetUserList("False");
            model.DashboardNameList = await _IXONUserRightDashboardBLL.GetDashboardName();
            //model.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, "");
            //model.EmpID = ID;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XONUserRightDashboard(UserRightDashboardModel model)
        {
            try
            {
                var UserRightDashboardGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                List<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                List<UserRightDashboardModel> UserRightDashboardDetailEdit = new List<UserRightDashboardModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    UserRightDashboardDetailEdit = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelEditJson);
                }

                if (UserRightDashboardDetail == null && UserRightDashboardDetailEdit == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("UserRightDashboardDetail", "User Right Dashboard Grid Should Have Atleast 1 Item...!");
                    return View("XONUserRightDashboard", model);
                }

                else
                {
                    model.CreatedById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        UserRightDashboardGrid = GetDetailTable(UserRightDashboardDetailEdit);
                    }
                    else
                    {
                        UserRightDashboardGrid = GetDetailTable(UserRightDashboardDetail);
                    }

                    var Result = await _IXONUserRightDashboardBLL.SaveUserRightDashboard(model, UserRightDashboardGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyUserRightDashboardDetail");
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Duplicate")
                        {
                            string gateNo = string.Empty;
                            gateNo = Result.Result.Rows[0]["Result"].ToString();
                            ViewBag.isSuccess = false;
                            TempData["409"] = "409";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                            return View("Error", Result);
                        }
                    }
                }
                return RedirectToAction(nameof(XONUserRightDashboard));
            }
            catch (Exception ex)
            {
                LogException<XONUserRightDashboardController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetUserList(string ShowAll)
        {
            var Result = await _IXONUserRightDashboardBLL.GetUserList(ShowAll);
            return Json(Result);
        }
        public async Task<JsonResult> GetDashboardName()
        {
            var Result = await _IXONUserRightDashboardBLL.GetDashboardName();
            return Json(Result);
        }
        public async Task<JsonResult> GetDashboardSubScreen(string DashboardName)
        {
            var Result = await _IXONUserRightDashboardBLL.GetDashboardSubScreen(DashboardName);
            string JsonString = JsonConvert.SerializeObject(Result);
            return Json(JsonString);
        }
        public IActionResult AddUserRightsDashboardDetail(UserRightDashboardModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                    List<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                    }

                    var MainModel = new UserRightDashboardModel();
                    var UserRightDashboardModelGrid = new List<UserRightDashboardModel>();
                    var UserRightDashboardGrid = new List<UserRightDashboardModel>();
                    var SSGrid = new List<UserRightDashboardModel>();

                    if (model != null)
                    {
                        if (UserRightDashboardDetail == null || UserRightDashboardDetail.Count() == 0)
                        {
                            model.SeqNo = 1;
                            UserRightDashboardGrid.Add(model);
                        }
                        else
                        {
                            if (UserRightDashboardDetail.Any(x=>x.UserId == model.UserId && x.DashboardName == model.DashboardName && x.DashboardSubScreen == model.DashboardSubScreen))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = UserRightDashboardDetail.Count + 1;
                                UserRightDashboardGrid = UserRightDashboardDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(UserRightDashboardGrid);
                                UserRightDashboardGrid.Add(model);
                            }
                        }

                        MainModel.UserRightsDashboard = UserRightDashboardGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.UserRightsDashboard);
                        HttpContext.Session.SetString("KeyUserRightDashboardDetail", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_XONUserRightDashboardGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                    List<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                    }

                    var MainModel = new UserRightDashboardModel();
                    var UserRightDashboardModelGrid = new List<UserRightDashboardModel>();
                    var UserRightDashboardGrid = new List<UserRightDashboardModel>();
                    var SSGrid = new List<UserRightDashboardModel>();

                    if (model != null)
                    {
                        if (UserRightDashboardDetail == null || UserRightDashboardDetail.Count() == 0)
                        {
                            model.SeqNo = 1;
                            UserRightDashboardGrid.Add(model);
                        }
                        else
                        {
                            if (UserRightDashboardDetail.Any(x => x.UserId == model.UserId && x.DashboardName == model.DashboardName && x.DashboardSubScreen == model.DashboardSubScreen))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = UserRightDashboardDetail.Count + 1;
                                UserRightDashboardGrid = UserRightDashboardDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(UserRightDashboardGrid);
                                UserRightDashboardGrid.Add(model);
                            }

                        }

                        MainModel.UserRightsDashboard = UserRightDashboardGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.UserRightsDashboard);
                        HttpContext.Session.SetString("KeyUserRightDashboardDetail", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_XONUserRightDashboardGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult ClearUserRightDashboardGrid()
        {
            HttpContext.Session.Remove("KeyUserRightDashboardDetail");
            return Json("Ok");
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
            if (Mode == "U" || Mode == "V")
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }
            }
            IEnumerable<UserRightDashboardModel> SSGrid = UserRightDashboardDetail;
            if (UserRightDashboardDetail != null)
            {
                SSGrid = UserRightDashboardDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new UserRightDashboardModel();
            if (Mode == "U" || Mode == "V")
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                List<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (UserRightDashboardDetail != null && UserRightDashboardDetail.Count > 0)
                {
                    UserRightDashboardDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in UserRightDashboardDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.UserRightsDashboard = UserRightDashboardDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.UserRightsDashboard);
                    HttpContext.Session.SetString("KeyUserRightDashboardDetail", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightDashboardDetail");
                List<UserRightDashboardModel> UserRightDashboardDetail = new List<UserRightDashboardModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardDetail = JsonConvert.DeserializeObject<List<UserRightDashboardModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (UserRightDashboardDetail != null && UserRightDashboardDetail.Count > 0)
                {
                    UserRightDashboardDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in UserRightDashboardDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.UserRightsDashboard = UserRightDashboardDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.UserRightsDashboard);
                    HttpContext.Session.SetString("KeyUserRightDashboardDetail", serializedGrid);
                }
            }

            return PartialView("_XONUserRightDashboardGrid", MainModel);
        }
        private static DataTable GetDetailTable(IList<UserRightDashboardModel> DetailList)
        {
            try
            {
                var UserRightDashboardGrid = new DataTable();

                UserRightDashboardGrid.Columns.Add("UID", typeof(int));
                UserRightDashboardGrid.Columns.Add("EmpID", typeof(int));
                UserRightDashboardGrid.Columns.Add("DashboardName", typeof(string));
                UserRightDashboardGrid.Columns.Add("DashboardSubscreen", typeof(string));
                UserRightDashboardGrid.Columns.Add("SeqNo", typeof(int));
                UserRightDashboardGrid.Columns.Add("OptView", typeof(bool));
                
                foreach (var Item in DetailList)
                {

                    UserRightDashboardGrid.Rows.Add(
                        new object[]
                        {
                    Item.UserId,
                    Item.EmpId,
                    Item.DashboardName??"",
                    Item.DashboardSubScreen??"",
                    Item.SeqNo,
                    Item.IsView
                        });
                }
                UserRightDashboardGrid.Dispose();
                return UserRightDashboardGrid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> XONUserRightDashboardData()
        {
            var model = new UserRightDashboardModel();
            Result = await _IXONUserRightDashboardBLL.GetUserRightDashboard("Get");
            model.UserRightsDashboard = Result;
            if (Result.Count == 0)
            {
                model.UserRightsDashboard = new List<UserRightDashboardModel>();
            }
            return View(model);
        }
        public async Task<IActionResult> UserRightDashBoard(string Flag = "True", string Usertype = "", string EmpCode = "", string EmpName = "", string UserName = "")
        {
            UserRightDashboardModel model = new()
            {
                //dynamic dd = new ExpandoObject();
                UserRightsDashboard = await _IXONUserRightDashboardBLL.GetDashBoardData("", Usertype, EmpCode, EmpName, UserName)
            };
            if (Flag == "False")
            {
                return PartialView("_UserGrid", model);
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> GetSearchData(string EmpName, string UserName, string DashboardName, string DashboardSubScreen) 
        {
            var model = new UserRightDashboardModel();
            model.UserRightsDashboard = new List<UserRightDashboardModel>();
            model = await _IXONUserRightDashboardBLL.GetSearchData(EmpName, UserName, DashboardName, DashboardSubScreen);
            model.Mode = "Summary";
            return PartialView("_SearchFieldDashboard", model);
        }
        public async Task<IActionResult> GetSearchDetailData(string EmpName, string UserName) // for detail dashboard only
        {
            var model = new UserRightDashboardModel();
            model.UserRightsDashboard = new List<UserRightDashboardModel>();
            model = await _IXONUserRightDashboardBLL.GetSearchDetailData(EmpName, UserName);
            model.Mode = "Detail";
            return PartialView("_SearchFieldDashboard", model);
        }
    }
}
