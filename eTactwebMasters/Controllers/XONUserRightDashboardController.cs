using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            return Json(Result);
        }
    }
}
