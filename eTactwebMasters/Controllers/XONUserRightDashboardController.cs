using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebMasters.Controllers
{
    public class XONUserRightDashboardController : Controller
    {
        private readonly EncryptDecrypt _EncryptDecrypt;
        private readonly IXONUserRightDashboardBLL _IXONUserRightDashboardBLL;
        private readonly IDataLogic _IDataLogic;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private dynamic Result;
        public XONUserRightDashboardController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IXONUserRightDashboardBLL IXONUserRightDashboardBLL)
        {
            _IDataLogic = iDataLogic;
            _IWebHostEnvironment = iWebHostEnvironment;
            _EncryptDecrypt = encryptDecrypt;
            _IXONUserRightDashboardBLL = IXONUserRightDashboardBLL;
        }
        public async Task<IActionResult> XONUserRightDashboard(int ID, string Mode, string userName = "")
        {
            UserRightDashboard model = new();
            if (Mode == "C")
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                List<UserRightDashboard> UserRightDashboardModelDetail = new List<UserRightDashboard>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardModelDetail = JsonConvert.DeserializeObject<List<UserRightDashboard>>(modelJson);
                }
                model.UserRightsDashboard = UserRightDashboardModelDetail;
            }
            else
            {
                HttpContext.Session.Remove("KeyUserRightsDetail");
                model.UserRightsDashboard = new List<UserRightDashboard>();
            }

            if (ID != 0 || (Mode == "V" || Mode == "U" || Mode == "C"))
            {
                model.Mode = Mode;
            }

            model.UserList = await _IXONUserRightDashboardBLL.GetUserList("False");
            model.DashboardNameList = await _IXONUserRightDashboardBLL.GetDashboardName();
            //model.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, "");
            //model.EmpID = ID;
            return View(model);
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
        public async Task<IActionResult> AddUserRightsDashboardDetail(UserRightDashboard model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                IList<UserRightDashboard> UserRightDashboardModelDetail = new List<UserRightDashboard>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    UserRightDashboardModelDetail = JsonConvert.DeserializeObject<List<UserRightDashboard>>(modelJson);
                }

                //ALL MODULES and MAINMENUS
                if (model.DashboardName == "0" && model.DashboardName == "0")
                {
                    var MainModel1 = new UserRightDashboard();

                    if (UserRightDashboardModelDetail != null)
                    {
                        if (UserRightDashboardModelDetail.Count > 0)
                        {
                            return StatusCode(203, "AlreadyExists");
                        }
                        else
                        {
                            var UserRightDashboardModel = new UserRightDashboard();
                            var UserRightDashboardGrid1 = new List<UserRightDashboard>();
                            var UserGrid1 = new List<UserRightDashboard>();
                            var SSGrid1 = new List<UserRightDashboard>();


                            UserRightDashboardModel.DashboardNameList ??= new List<TextValue>();

                            UserRightDashboardModel.DashboardNameList.Add(new TextValue
                            {
                                Text = "Sales Dashboard",
                                Value = "Sales"
                            });
                            int SeqNo = 1;
                            foreach (var item in UserRightDashboardModel.UserRightsDashboard)
                            {
                                var newUserRightDashboardModel = new UserRightDashboard();

                                newUserRightDashboardModel.DashboardName = item.DashboardName;
                                newUserRightDashboardModel.IsView = model.IsView;
                                newUserRightDashboardModel.DashboardSubScreen = model.DashboardSubScreen;

                                foreach (var item1 in newUserRightDashboardModel.DashboardName)
                                {
                                    string userRightsJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                                    IList<UserRightDashboard> UserRightDashboardModelDetail1 = new List<UserRightDashboard>();
                                    if (!string.IsNullOrEmpty(userRightsJson))
                                    {
                                        UserRightDashboardModelDetail1 = JsonConvert.DeserializeObject<List<UserRightDashboard>>(userRightsJson);
                                    }
                                    var newMenuItemModel = new UserRightDashboard();

                                    newMenuItemModel.DashboardName = newUserRightDashboardModel.DashboardName;

                                    if (newMenuItemModel != null)
                                    {
                                        if (UserRightDashboardModelDetail1 == null)
                                        {
                                            newMenuItemModel.SeqNo = 1;
                                            UserGrid1.Add(newMenuItemModel);
                                        }
                                        else
                                        {
                                            newMenuItemModel.SeqNo = SeqNo++;
                                            UserGrid1 = UserRightDashboardModelDetail1.Where(x => x != null).ToList();
                                            SSGrid1.AddRange(UserGrid1);
                                            UserGrid1.Add(newMenuItemModel);
                                        }

                                        MainModel1.UserRightsDashboard = UserGrid1;
                                        HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(MainModel1.UserRightsDashboard));
                                    }
                                    else
                                    {
                                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                                    }
                                }
                            }
                        }
                    }
                    return PartialView("_UserRightGrid", MainModel1);
                }
                else
                {
                    var MainModel = new UserRightDashboard();
                    var existingListJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                    var existingRights = !string.IsNullOrEmpty(existingListJson)
                        ? JsonConvert.DeserializeObject<List<UserRightDashboard>>(existingListJson)
                        : new List<UserRightDashboard>();

                    var duplicates = new List<string>();
                    int seqNo = 0;

                    MainModel.DashboardName = model.DashboardName;
                    

                    if (model.DashboardName == "0")
                        model.DashboardName = string.Empty;

                    MainModel.DashboardNameList ??= new List<TextValue>();

                    MainModel.DashboardNameList.Add(new TextValue
                    {
                        Text = "Sales Dashboard",
                        Value = "Sales"
                    });

                    foreach (var item1 in MainModel.DashboardName)
                    {
                        var newEntry = new UserRightDashboard
                        {
                            DashboardName = MainModel.DashboardName,
                        };

                        bool isDuplicate = existingRights.Any(x =>
                            x.EmpName == newEntry.EmpName &&
                            x.DashboardName == newEntry.DashboardName &&
                            x.DashboardSubScreen == newEntry.DashboardSubScreen);

                        if (isDuplicate)
                        {
                            duplicates.Add(newEntry.DashboardName);
                            continue;
                        }

                        newEntry.SeqNo = seqNo++;
                        existingRights.Add(newEntry);
                    }

                    HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(existingRights));
                    MainModel.UserRightsDashboard = existingRights;

                    if (duplicates.Any())
                    {
                        TempData["Duplicate"] = "Duplicate rights not allowed for: " + string.Join(", ", duplicates);
                    }

                    return PartialView("_UserRightGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
