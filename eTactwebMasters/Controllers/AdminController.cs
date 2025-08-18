using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Helpers;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers;

[Authorize]
public class AdminController : Controller, IAsyncDisposable
{
    private readonly EncryptDecrypt _EncryptDecrypt;
    private readonly IAdminModule _IAdminModule;
    private readonly IDataLogic _IDataLogic;
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private dynamic Result;

    public AdminController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IAdminModule iAdminModule)
    {
        _IDataLogic = iDataLogic;
        _IWebHostEnvironment = iWebHostEnvironment;
        _EncryptDecrypt = encryptDecrypt;
        _IAdminModule = iAdminModule;
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

    public async Task<IActionResult> DeleteUserByID(int ID)
    {
       // Result = _IAdminModule.DeleteUserByID(ID);
        var Result = await _IAdminModule.DeleteUserByID(ID).ConfigureAwait(true);

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.OK)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["403"] = "403";
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

    public void GetClearGrid()
    {
        string modelJson = HttpContext.Session.GetString("KeyUserRightsList");
        if (string.IsNullOrEmpty(modelJson))
        {
            HttpContext.Session.Remove("KeyUserRightsList");
        }
        HttpContext.Session.Remove("KeyUserRightsDetail");
    }
    public string ClearGrid()
    {
        HttpContext.Session.Remove("KeyUserRightsDetail");
        return "OK";
    }

    public async Task<IActionResult> UserMaster(int ID, string Mode)
    {
        UserMasterModel model = new();
        var encrypted = System.IO.File.ReadAllText("license.lic");
        var plain = LicenseCrypto.Decrypt(encrypted);
        var lic = System.Text.Json.JsonSerializer.Deserialize<LicenseInfo>(plain);
        var jsonResult = await GetUserCount();

        var response = jsonResult.Value as ResponseResult;
        int userCount = 0;

        if (response?.Result is DataTable dt && dt.Rows.Count > 0)
        {
            userCount = Convert.ToInt32(dt.Rows[0]["TotalUser"]);
        }
        ViewBag.CurrentUserCount = userCount;
        ViewBag.LicensedUserCount = lic.NumberOfUser;
        HttpContext.Session.Remove("KeyUserRightsList");
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
            string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
            List<UserRightModel> UserRightModelDetail = new List<UserRightModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                UserRightModelDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
            }
            model.UserRights = UserRightModelDetail;
        }
        else
        {
            HttpContext.Session.Remove("KeyUserRightsDetail");
            model.UserRights = new List<UserRightModel>();
        }

        if (ID != 0 || (Mode == "V" || Mode == "U" || Mode == "C"))
        {
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
    public IActionResult UpdateAllUserRightsInSession(string field, bool value)
    {
        string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
        var userRights = string.IsNullOrEmpty(modelJson)
            ? new List<UserRightModel>()
            : JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);

        foreach (var right in userRights)
        {
            switch (field)
            {
                case "All": right.All = value.ToString().ToLower(); break;
                case "Update": right.Update = value.ToString().ToLower(); break;
                case "Save": right.Save = value.ToString().ToLower(); break;
                case "Delete": right.Delete = value.ToString().ToLower(); break;
                case "View": right.View = value.ToString().ToLower(); break;
            }
        }

        HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(userRights));
        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult UpdateAllPermissionsInSession(bool value)
    {
        string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
        var userRights = string.IsNullOrEmpty(modelJson)
            ? new List<UserRightModel>()
            : JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);

        foreach (var right in userRights)
        {
            right.All = value.ToString().ToLower();
            right.Update = value.ToString().ToLower();
            right.Save = value.ToString().ToLower();
            right.Delete = value.ToString().ToLower();
            right.View = value.ToString().ToLower();
        }

        HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(userRights));
        return Json(new { success = true });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserRights(UserRightModel model)
    {
        model.Mode = model.ModelMode == "U" ? "Update" : "Insert";
        if (model.EmpID != 0)
        {
            var UserRightGrid = new DataTable();
            string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
            List<UserRightModel> UserRightDetail = new List<UserRightModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                UserRightDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
            }

            

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
    public async Task<IActionResult> GetSearchDataByEmpName(int EmpID, string Module, string MainMenu) // for main form
    {
        //model.Mode = "Search";
        var model = new UserRightModel();
        model.UserRights = new List<UserRightModel>();
        model = await _IAdminModule.GetSearchData(EmpID);
        HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(model.UserRights));
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

            string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
            List<UserRightModel> UserRightList = new List<UserRightModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                UserRightList = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
            }

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

            HttpContext.Session.SetString("KeyUserRightsList", JsonConvert.SerializeObject(MainModel.UserRights));    
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
            string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
            IList<UserRightModel> UserRightModelDetail = new List<UserRightModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                UserRightModelDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
            }

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

                            //if (item.Text == "SALES & MKT")
                            //{
                            //    item.Text = "Sales";
                            //}
                             if (item.Text == "PROCURMENT MGMT")
                            {
                                item.Text = "Procurment";
                            }

                            newUserRightModel.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", item.Text, "");

                            foreach (var item1 in newUserRightModel.MainMenuList)
                            {
                                string userRightsJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                                IList<UserRightModel> UserRightModelDetail1 = new List<UserRightModel>();
                                if (!string.IsNullOrEmpty(userRightsJson))
                                {
                                    UserRightModelDetail1 = JsonConvert.DeserializeObject<List<UserRightModel>>(userRightsJson);
                                }
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
                                    HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(MainModel1.UserRights));
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
                var existingListJson = HttpContext.Session.GetString("KeyUserRightsDetail");
                var existingRights = !string.IsNullOrEmpty(existingListJson)
                    ? JsonConvert.DeserializeObject<List<UserRightModel>>(existingListJson)
                    : new List<UserRightModel>();

                var duplicates = new List<string>();
                int seqNo = existingRights.Any() ? (existingRights.Max(x => x.Seqno) ?? 0) + 1 : 1;

                MainModel.Module = model.Module;
                MainModel.All = model.All;
                MainModel.Save = model.Save;
                MainModel.Update = model.Update;
                MainModel.Delete = model.Delete;
                MainModel.View = model.View;

                if (model.MainMenu == "0")
                    model.MainMenu = string.Empty;

                MainModel.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, model.MainMenu);

                foreach (var item1 in MainModel.MainMenuList)
                {
                    var newEntry = new UserRightModel
                    {
                        Module = MainModel.Module,
                        MainMenu = item1.Text,
                        All = MainModel.All,
                        Save = MainModel.Save,
                        Update = MainModel.Update,
                        Delete = MainModel.Delete,
                        View = MainModel.View,
                        EmpID = model.EmpID,
                        EmpName = model.EmpName,
                        ID = model.ID
                    };

                    bool isDuplicate = existingRights.Any(x =>
                        x.EmpName == newEntry.EmpName &&
                        x.Module == newEntry.Module &&
                        x.MainMenu == newEntry.MainMenu);

                    if (isDuplicate)
                    {
                        duplicates.Add(newEntry.MainMenu);
                        continue;
                    }

                    newEntry.Seqno = seqNo++;
                    existingRights.Add(newEntry);
                }

                HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(existingRights));
                MainModel.UserRights = existingRights;

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
    public IActionResult DeleteItemRow(int SeqNo)
    {
        var MainModel = new UserRightModel();
        string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
        List<UserRightModel> UserRightDetail = new List<UserRightModel>();
        if (!string.IsNullOrEmpty(modelJson))
        {
            UserRightDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
        }
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

            HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(MainModel.UserRights));
        }
        return PartialView("_UserRightGrid", MainModel);
    }

    public IActionResult EditItemRow(int SeqNo)
    {
        IList<UserRightModel> UserRightDetail = new List<UserRightModel>();

        string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
        if (!string.IsNullOrEmpty(modelJson))
        {
            UserRightDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
        }

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
        string modelJson = HttpContext.Session.GetString("KeyUserRightsDetail");
        List<UserRightModel> UserRightModelDetail = new List<UserRightModel>();
        if (!string.IsNullOrEmpty(modelJson))
        {
            UserRightModelDetail = JsonConvert.DeserializeObject<List<UserRightModel>>(modelJson);
        }
        if (UserRightModelDetail != null)
        {
            foreach (var userRight in UserRightModelDetail)
            {
                userRight.EmpID = EmpID;
                userRight.EmpName = EmpName;
            }
            MainModel.UserRights = UserRightModelDetail;
            HttpContext.Session.SetString("KeyUserRightsDetail", JsonConvert.SerializeObject(MainModel.UserRights));
        }
        return PartialView("_UserRightGrid", MainModel);
    }
    public async Task<JsonResult> GetUserCount()
    {
        var Result = await _IAdminModule.GetUserCount();
        return Json(Result);
    }
}