using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactwebMasters.Controllers
{
    public class XONUserRightDashboardController : Controller
    {
        public IActionResult XONUserRightDashboard()
        {
            return View();
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

            //model.UserList = await _IAdminModule.GetUserList("False");
            //model.ModuleList = await _IAdminModule.GetMenuList("Module", "", "");
            //model.MainMenuList = await _IAdminModule.GetMenuList("MainMenu", model.Module, "");
            //model.EmpID = ID;
            return View(model);
        }
    }
}
