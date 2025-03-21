using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;

namespace eTactWeb.Controllers;

[Authorize]
public class AdminController : Controller, IAsyncDisposable
{
    private readonly EncryptDecrypt _EncryptDecrypt;
    private readonly IAdminModule _IAdminModule;
    private readonly IDataLogic _IDataLogic;
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private readonly IMemoryCache _MemoryCache;

    private dynamic Result;

    public AdminController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IAdminModule iAdminModule, IMemoryCache memoryCache)
    {
        _IDataLogic = iDataLogic;
        _IWebHostEnvironment = iWebHostEnvironment;
        _EncryptDecrypt = encryptDecrypt;
        _IAdminModule = iAdminModule;
        _MemoryCache = memoryCache;
    }

    public IActionResult DeleteRightByID(int ID)
    {
        Common.ResponseResult Result = _IAdminModule.DeleteRightByID(ID);

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.OK)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }

        return RedirectToAction(nameof(UserRightDashboard));
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
    }

    //public async ValueTask DisposeAsync() => GC.SuppressFinalize(this);

    #region UserMaster

    public IActionResult DeleteUserByID(int ID)
    {
        Result = _IAdminModule.DeleteUserByID(ID);

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.OK)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }

        return RedirectToAction(nameof(UserDashBoard));
    }

    public async Task<IActionResult> UserDashBoard(string Flag = "True", string Usertype = "", string EmpCode = "", string EmpName = "", string UserName = "")
    {
        UserMasterModel model = new()
        {
            //dynamic dd = new ExpandoObject();
            UserMasterList = await _IAdminModule.GetDashBoardData("", Usertype, EmpCode, EmpName, UserName)
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

    public async Task<IActionResult> UserMaster(int ID, string Mode)
    {
        UserMasterModel model = new();
        _MemoryCache.Remove("KeyUserRightsList");
        if (ID == 0)
        {
            model.UserTypeList = await _IDataLogic.GetDropDownList("UserTypeTB", "SP_GetDropDownList");
            model.EmpNameList = await _IDataLogic.GetDropDownList("EmpNameWithCode", "SP_GetDropDownList");
            model.CreatedByName = HttpContext.Session.GetString("EmpName");
            model.CreatedOn = DateTime.Now;
            return View(model);
        }
        else
        {
            model = await _IAdminModule.GetUserByID(ID);
            model.UserTypeList = await _IDataLogic.GetDropDownList("UserTypeTB", "SP_GetDropDownList");
            model.EmpNameList = await _IDataLogic.GetDropDownList("EmpNameNCode", "SP_GetDropDownList");
            model.Mode = Mode;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserMaster(UserMasterModel model)
    {
        //if (ModelState.IsValid)
        //{
        model.Mode = model.ID == 0 ? "Insert" : "Update";

        if (model.Password == model.CnfPass)
        {
            //    if (!string.IsNullOrEmpty(model.CnfPass))
            //    {
            //        model.CnfPass = _EncryptDecrypt.Encrypt(model.CnfPass);
            //    }
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            Result = await _IAdminModule.SaveUserMaster(model);

            if (Result == null)
            {
                ViewBag.isSuccess = false;
                TempData["Message"] = "Something Went Wrong, Please Try Again.";
                return RedirectToAction(nameof(UserMaster), new { ID = 0 });
            }
            else if (Result != null && Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.isSuccess = true;
                TempData["200"] = "200";
            }
            else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["202"] = "202";
            }
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["Message"] = "Password And Confirm Password aren't same.";
            return RedirectToAction(nameof(UserMaster), new { ID = 0 });
        }
        //}
        return RedirectToAction(nameof(UserDashBoard));

        //else
        //{
        //    ViewBag.isSuccess = false;
        //    TempData["Message"] = "Form Validation Error.";
        //    return RedirectToAction(nameof(UserMaster), new { ID = 0 });
        //}
    }

    #endregion UserMaster

    public async Task<JsonResult> GetList(string Flag, string Module, string MainMenu)
    {
        if (Flag == "Module")
        {
            //Module = Module == "SALES & MKT" ? "Sales" : Module;
            //Module = Module == "PROCURMENT MGMT" ? "PROCURMENT" : Module;
            //Module = Module == "FINANCIAL ACCOUNTING" ? "Financial Accounting" : Module;
            Result = await _IAdminModule.GetMenuList("MainMenu", Module, "");
        }
        //if (Flag == "SubMenu")
        //{
        //    Result = await _IAdminModule.GetMenuList("SubMenu", Module, MainMenu);
        //}
        return Json(Result);
    }
    public async Task<JsonResult> GetUserList(string ShowAll)
    {
        var Result = await _IAdminModule.GetUserList(ShowAll);
        return Json(Result);
    }
    public async Task<JsonResult> GetUserType()
    {
        string UserType = HttpContext.Session.GetString("UserType");
        return Json(UserType);
    }
    public async Task<JsonResult> CheckAdminExists()
    {
        var Result = await _IAdminModule.CheckAdminExists();
        string JsonString = JsonConvert.SerializeObject(Result);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAllUserRights()
    {
        int EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var Result = await _IAdminModule.GetAllUserRights(EmpId);
        string JsonString = JsonConvert.SerializeObject(Result);
        return Json(JsonString);
    }
    public async Task<IActionResult> UserRightDashboard()
    {
        var model = new UserRightModel();
        Result = await _IAdminModule.GetUserRightDashboard("Get");
        model.UserRights = Result;
        if (Result.Count == 0)
        {
            model.UserRights = new List<UserRightModel>();
        }
        return View(model);
    }

    public async Task<IActionResult> UserRights(int ID, string Mode, string userName = "")
    {
        UserRightModel model = new();
        if (Mode == "C")
        {
            _MemoryCache.TryGetValue("KeyUserRightsDetail", out IList<UserRightModel> UserRightModelDetail);
            model.UserRights = UserRightModelDetail;
        }
        else
        {
            _MemoryCache.Remove("KeyUserRightsDetail");
            model.UserRights = new List<UserRightModel>();

        }

        if (ID != 0 || (Mode == "V" || Mode == "U" || Mode == "C"))
        {

            // model = await _IAdminModule.GetUserRightByID(ID);
            model.Mode = Mode;
        }

        model.UserList = await _IAdminModule.GetUserList("False");
        model.ModuleList = await _IAdminModule.GetMenuList("Module", "", "");
        model.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, "");
        //model.SubMenuList = await _IAdminModule.GetMenuList("SubMenu", "", "");
        model.EmpID = ID;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserRights(UserRightModel model)
    {
        //if (ModelState.IsValid)
        //{
        model.Mode = model.ModelMode == "U" ? "Update" : "Insert";

        if (model.EmpID != 0)
        {
            //model.SubMenu ??= model.SubMenu2;
            var UserRightGrid = new DataTable();
            _MemoryCache.TryGetValue("KeyUserRightsDetail", out List<UserRightModel> UserRightDetail);

            UserRightGrid = GetDetailTable(UserRightDetail);
            Common.ResponseResult Result = await _IAdminModule.SaveUserRights(model, UserRightGrid);

            if (Result == null)
            {
                ViewBag.isSuccess = false;
                TempData["Message"] = "Something Went Wrong, Please Try Again.";
                return RedirectToAction(nameof(UserRights), new { ID = 0 });
            }
            else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
            {
                ViewBag.isSuccess = true;
                TempData["200"] = "200";
            }
            else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.Ambiguous)
            {
                ViewBag.isSuccess = false;
                TempData["300"] = "300";
            }
            else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["202"] = "202";
            }
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["Message"] = "Password And Confirm Password aren't same.";
            return RedirectToAction(nameof(UserRights), new { ID = 0 });
        }
        return RedirectToAction(nameof(UserRightDashboard), new { ID = 0 });
    }
    //else
    //{
    //    ViewBag.isSuccess = false;
    //    TempData["Message"] = "Form Validation Error.";
    //    return RedirectToAction(nameof(UserRights), new { ID = 0 });
    //}

    public async Task<IActionResult> GetSearchDataByEmpName(int EmpID, string Module, string MainMenu) // for main form
    {
        //model.Mode = "Search";
        var model = new UserRightModel();
        model.UserRights = new List<UserRightModel>();
        model = await _IAdminModule.GetSearchData(EmpID);
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };

        _MemoryCache.Set("KeyUserRightsDetail", model.UserRights, cacheEntryOptions);
        if (model.UserRights.Count > 0)
        {
            model.Mode = "U";
        }
        return PartialView("_UserRightGrid", model);
    }
    public async Task<IActionResult> GetSearchData(string EmpName, string UserName, string Module, string MainMenu) // for dashboard only
    {
        var model = new UserRightModel();
        model.UserRights = new List<UserRightModel>();
        model = await _IAdminModule.GetSearchData(EmpName, UserName, Module, MainMenu);
        model.Mode = "Summary";
        return PartialView("_SearchFieldDashboard", model);
    }
    public async Task<IActionResult> GetSearchDetailData(string EmpName, string UserName) // for detail dashboard only
    {
        var model = new UserRightModel();
        model.UserRights = new List<UserRightModel>();
        model = await _IAdminModule.GetSearchDetailData(EmpName, UserName);
        model.Mode = "Detail";
        return PartialView("_SearchFieldDashboard", model);
    }
    //public async Task<IActionResult> GetSearchDataByUser(string EmpName)
    //{
    //    //model.Mode = "Search";
    //    var model = new UserRightModel();
    //    model = await _IAdminModule.GetSearchData(EmpName);
    //    return PartialView("_UserRightsAssigned", model);
    //}
    public IActionResult AddToGrid(int EmpID, string Module, string MainMenu, string All, string Save, string Update, string Delete, string View)
    {
        try
        {
            var model = new UserRightModel
            {
                EmpID = EmpID,
                Module = Module,
                MainMenu = MainMenu,
                All = All,
                Save = Save,
                Update = Update,
                Delete = Delete,
                View = View
            };

            _MemoryCache.TryGetValue("KeyUserRightsList", out IList<UserRightModel> UserRightList);

            var MainModel = new UserRightModel();
            var UserRightGrid = new List<UserRightModel>();
            var UserGrid = new List<UserRightModel>();
            var SSGrid = new List<UserRightModel>();

            if (UserRightList == null)
            {
                MainModel.Seqno = 1;
                UserGrid.Add(model);
            }
            else
            {
                if (UserRightList.Where(x => x.EmpID == model.EmpID && x.Module == model.Module && x.MainMenu == model.MainMenu).Any())
                {
                    return StatusCode(207, "Duplicate");
                }
                else
                {
                    model.Seqno = UserRightList.Count + 1;
                    UserGrid = UserRightList.Where(x => x != null).ToList();
                    SSGrid.AddRange(UserGrid);
                    UserGrid.Add(model);
                }
            }

            MainModel.UserRights = UserGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyUserRightsList", MainModel.UserRights, cacheEntryOptions);


            return PartialView("_UserRightsAssigned", MainModel);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<IActionResult> AddUserRightDetail(UserRightModel model)
    {
        try
        {
            _MemoryCache.TryGetValue("KeyUserRightsDetail", out IList<UserRightModel> UserRightModelDetail);

            //ALL MODULES and MAINMENUS
            if (model.Module == "0" && model.MainMenu == "0")
            {
                var MainModel1 = new UserRightModel();

                if (UserRightModelDetail != null)
                {
                    if (UserRightModelDetail.Count > 0)
                    {
                        return StatusCode(203, "AlreadyExists");
                    }
                    else
                    {
                        var UserRightModel = new UserRightModel();
                        var UserRightGrid1 = new List<UserRightModel>();
                        var UserGrid1 = new List<UserRightModel>();
                        var SSGrid1 = new List<UserRightModel>();


                        UserRightModel.ModuleList = await _IAdminModule.GetMenuList("Module", "", "");
                        int SeqNo = 1;
                        foreach (var item in UserRightModel.ModuleList)
                        {
                            var newUserRightModel = new UserRightModel();

                            newUserRightModel.Module = item.Text;
                            newUserRightModel.All = model.All;
                            newUserRightModel.Save = model.Save;
                            newUserRightModel.Update = model.Update;
                            newUserRightModel.Delete = model.Delete;
                            newUserRightModel.View = model.View;

                            if (item.Text == "SALES & MKT")
                            {
                                item.Text = "Sales";
                            }
                            else if (item.Text == "PROCURMENT MGMT")
                            {
                                item.Text = "Procurment";
                            }

                            newUserRightModel.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", item.Text, "");

                            foreach (var item1 in newUserRightModel.MainMenuList)
                            {
                                _MemoryCache.TryGetValue("KeyUserRightsDetail", out IList<UserRightModel> UserRightModelDetail1);
                                var newMenuItemModel = new UserRightModel();

                                newMenuItemModel.Module = newUserRightModel.Module;
                                newMenuItemModel.All = newUserRightModel.All;
                                newMenuItemModel.Save = newUserRightModel.Save;
                                newMenuItemModel.Update = newUserRightModel.Update;
                                newMenuItemModel.Delete = newUserRightModel.Delete;
                                newMenuItemModel.View = newUserRightModel.View;
                                newMenuItemModel.MainMenu = item1.Text;
                                newMenuItemModel.EmpID = model.EmpID;
                                newMenuItemModel.EmpName = model.EmpName;
                                newMenuItemModel.ID = model.ID;

                                if (newMenuItemModel != null)
                                {
                                    if (UserRightModelDetail1 == null)
                                    {
                                        newMenuItemModel.Seqno = 1;
                                        UserGrid1.Add(newMenuItemModel);
                                    }
                                    else
                                    {
                                        newMenuItemModel.Seqno = SeqNo++;
                                        UserGrid1 = UserRightModelDetail1.Where(x => x != null).ToList();
                                        SSGrid1.AddRange(UserGrid1);
                                        UserGrid1.Add(newMenuItemModel);
                                    }

                                    MainModel1.UserRights = UserGrid1;

                                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                                    {
                                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                                        SlidingExpiration = TimeSpan.FromMinutes(55),
                                        Size = 1024,
                                    };

                                    _MemoryCache.Set("KeyUserRightsDetail", MainModel1.UserRights, cacheEntryOptions);
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
                var MainModel = new UserRightModel();
                var UserRightGrid = new List<UserRightModel>();
                var UserGrid = new List<UserRightModel>();
                var SSGrid = new List<UserRightModel>();

                MainModel.Module = model.Module;
                MainModel.All = model.All;
                MainModel.Save = model.Save;
                MainModel.Update = model.Update;
                MainModel.Delete = model.Delete;
                MainModel.View = model.View;
                if(model.MainMenu == "0")
                {
                    model.MainMenu = string.Empty;
                }
                MainModel.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, model.MainMenu);
                int SeqNo = 1;
                foreach (var item1 in MainModel.MainMenuList)
                {
                    _MemoryCache.TryGetValue("KeyUserRightsDetail", out IList<UserRightModel> UserRightModelDetail1);
                    var newMenuItemModel = new UserRightModel();

                    newMenuItemModel.Module = MainModel.Module;
                    newMenuItemModel.All = MainModel.All;
                    newMenuItemModel.Save = MainModel.Save;
                    newMenuItemModel.Update =   MainModel.Update;
                    newMenuItemModel.Delete = MainModel.Delete;
                    newMenuItemModel.View = MainModel.View;
                    newMenuItemModel.MainMenu = item1.Text;
                    newMenuItemModel.EmpID = model.EmpID;
                    newMenuItemModel.EmpName = model.EmpName;
                    newMenuItemModel.ID = model.ID;

                    if (newMenuItemModel != null)
                    {
                        if (UserRightModelDetail1 == null)
                        {
                            newMenuItemModel.Seqno = 1;
                            UserGrid.Add(model);
                        }
                        else
                        {
                            if (UserRightModelDetail1.Where(x => x.EmpName == newMenuItemModel.EmpName && x.Module == newMenuItemModel.Module && x.MainMenu == newMenuItemModel.MainMenu).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                newMenuItemModel.Seqno = SeqNo++;
                                UserGrid = UserRightModelDetail1.Where(x => x != null).ToList();
                                SSGrid.AddRange(UserGrid);
                                UserGrid.Add(newMenuItemModel);
                            }
                        }

                        MainModel.UserRights = UserGrid;
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyUserRightsDetail", MainModel.UserRights, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                }
 
                return PartialView("_UserRightGrid", MainModel);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public IActionResult DeleteItemRow(int SeqNo)
    {
        var MainModel = new UserRightModel();
        _MemoryCache.TryGetValue("KeyUserRightsDetail", out List<UserRightModel> UserRightDetail);
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (UserRightDetail != null && UserRightDetail.Count > 0)
        {
            UserRightDetail.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (var item in UserRightDetail)
            {
                Indx++;
                item.Seqno = Indx;
            }
            MainModel.UserRights = UserRightDetail;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyUserRightsDetail", MainModel.UserRights, cacheEntryOptions);
        }


        return PartialView("_UserRightGrid", MainModel);
    }

    public IActionResult EditItemRow(int SeqNo)
    {
        IList<UserRightModel> UserRightDetail = new List<UserRightModel>();

        _MemoryCache.TryGetValue("KeyUserRightsDetail", out UserRightDetail);

        IEnumerable<UserRightModel> SSGrid = UserRightDetail;
        if (UserRightDetail != null)
        {
            SSGrid = UserRightDetail.Where(x => x.Seqno == SeqNo);
        }
        string JsonString = JsonConvert.SerializeObject(SSGrid);
        return Json(JsonString);
    }

    private static DataTable GetDetailTable(IList<UserRightModel> DetailList)
    {
        var GIGrid = new DataTable();

        GIGrid.Columns.Add("UID", typeof(int));
        GIGrid.Columns.Add("EmpID", typeof(int));
        GIGrid.Columns.Add("EmpName", typeof(string));
        GIGrid.Columns.Add("Module", typeof(string));
        GIGrid.Columns.Add("MainMenu", typeof(string));
        GIGrid.Columns.Add("OptAll", typeof(string));
        GIGrid.Columns.Add("OptSave", typeof(string));
        GIGrid.Columns.Add("OptUpdate", typeof(string));
        GIGrid.Columns.Add("OptDelete", typeof(string));
        GIGrid.Columns.Add("OptView", typeof(string));
        GIGrid.Columns.Add("CreatedOn", typeof(DateTime));
        GIGrid.Columns.Add("CreatedBy", typeof(string));
        GIGrid.Columns.Add("UpdatedOn", typeof(DateTime));
        GIGrid.Columns.Add("UpdatedBy", typeof(string));
        GIGrid.Columns.Add("Active", typeof(string));


        foreach (var Item in DetailList)
        {
            GIGrid.Rows.Add(
                new object[]
                {
                    0,//UID
                    Item.EmpID,
                    Item.EmpName,
                    Item.Module,
                    Item.MainMenu,
                    Item.All,
                    Item.Save,
                    Item.Update,
                    Item.Delete,
                    Item.View,
                    DateTime.Today,//Item.CreatedOn,
                    Item.CreatedBy,
                    DateTime.Today,
                    Item.UpdatedBy,
                    "Y"
                });
        }
        GIGrid.Dispose();
        return GIGrid;
    }
    public async Task<IActionResult> CopyUserRightDetail(string EmpName, int EmpID)
    {
        var MainModel = new UserRightModel();
        _MemoryCache.TryGetValue("KeyUserRightsDetail", out IList<UserRightModel> UserRightModelDetail);
        if (UserRightModelDetail != null)
        {
            foreach (var userRight in UserRightModelDetail)
            {
                userRight.EmpID = EmpID;
                userRight.EmpName = EmpName;
            }
            MainModel.UserRights = UserRightModelDetail;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyUserRightsDetail", MainModel.UserRights, cacheEntryOptions);
        }
        return PartialView("_UserRightGrid", MainModel);
    }
}