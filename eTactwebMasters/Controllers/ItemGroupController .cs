using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace eTactWeb.Controllers
{
    [Authorize]
    public class ItemGroupController : Controller
    {
        private readonly IItemGroup _IItemGroup;
        private readonly IDataLogic _IDataLogic;

        public ItemGroupController(IDataLogic iDataLogic, IItemGroup iItemGroup)
        {
            _IDataLogic = iDataLogic;
            _IItemGroup = iItemGroup;
        }

        [HttpPost]
        public JsonResult AutoComplete(string ColumnName, string prefix)
        {
            var iList = _IDataLogic.AutoComplete("Item_group_master", ColumnName, "", "", 0, 0);
            var Result = (from item in iList
                          where item.Text.Contains(prefix)
                          select new
                          {
                              item.Text
                          }).Distinct().ToList();

            return Json(Result);
        }

        public async Task<IActionResult> DashBoard()
        {
            var model = new ItemGroupModel();
            model.Mode = "Dashboard";
            var opts = GetFormRights();
            var jsonString = "";
            if (opts != null)
            {
                jsonString = opts.Result.Value.ToString();
            }
            JObject json = JObject.Parse(jsonString);
            JArray table = (JArray)json["Result"]["Table"];
            if (table.Count > 0)
            {
                string optAll = table[0]["OptAll"].ToString();
                if (optAll == "False")
                {
                    string optUpdate = table[0]["OptUpdate"].ToString();
                    string optDelete = table[0]["OptDelete"].ToString();
                    string optView = table[0]["OptView"].ToString();
                    model.OptDelete = optDelete;
                    model.OptUpdate = optUpdate;
                    model.OptView = optView;

                }
                else
                {
                    model.OptView = model.OptView ?? "True";
                    model.OptDelete = model.OptDelete ?? "True";
                    model.OptUpdate = model.OptUpdate ?? "True";
                }
            }
            else
            {
                model.OptView = model.OptView ?? "True";
                model.OptDelete = model.OptDelete ?? "True";
                model.OptUpdate = model.OptUpdate ?? "True";
            }

            model = await _IItemGroup.GetDashboardData(model).ConfigureAwait(false);
            //model.ItemCatList = model.ItemCatList.DistinctBy(x => x.Entry_id).ToList();
            model.ItemGroupList = model.ItemGroupList.DistinctBy(x => x.Group_Code).ToList();
            return View(model);
        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            // need to chk
            // var IsDelete = _IDataLogic.IsDelete(ID, "Entry_id");
            var IsDelete = _IDataLogic.IsDelete(ID, "Group_id");

            if (IsDelete == 0)
            {
                var Result = await _IItemGroup.DeleteByID(ID);

                if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
                {
                    ViewBag.isSuccess = true;
                    TempData["410"] = "410";
                }
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["423"] = "423";
            }

            return RedirectToAction(nameof(DashBoard));
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IItemGroup.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnderCategory(string Mode, string Type)
        {
            var JSON = await _IItemGroup.GetUnderCategory(Mode, Type);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckBeforeUpdate(int GroupCode)
        {
            var JSON = await _IItemGroup.CheckBeforeUpdate(GroupCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetAllItemGroup()
        {
            var JSON = await _IItemGroup.GetAllItemGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> Form(int ID, string Mode)
        {
            var model = new ItemGroupModel();
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            if (ID == 0)
            {

                model.ItemCateList = await _IDataLogic.GetDropDownList("Itemgroup_master", "SP_GetDropDownList").ConfigureAwait(true);
                //model.Entry_date = DateTime.Today;
            }
            else
            {
                model = await _IItemGroup.GetByID(ID).ConfigureAwait(false);
                //model.ItemCateList = await _IDataLogic.GetDropDownList("Item_Master_Type", "SP_GetDropDownList").ConfigureAwait(true);
                model.ItemServAssets = (model.ItemServAssets == "Assets") ? "Asset" : (model.ItemServAssets == "Services") ? "Service" : "Item";

                model.Mode = Mode;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(ItemGroupModel model)
        {
            ModelState.Remove("TxPageName");
            if (ModelState.IsValid)
            {
                model.Mode = model.ID == 0 ? "Insert" : "Update";
                model.CreatedBy = Constants.UserID;
                if (model.Mode == "Insert")
                {
                    model.Group_Code = 0;
                };
                var Result = await _IItemGroup.SaveItemGroup(model).ConfigureAwait(true);

                if (Result == null)
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Something Went Wrong, Please Try Again.";
                }
                else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                    return RedirectToAction(nameof(DashBoard));
                }
                else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.Ambiguous)
                {
                    ViewBag.isSuccess = false;
                    TempData["300"] = Result.Result;
                }
                else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                {
                    ViewBag.isSuccess = true;
                    TempData["202"] = "202";
                    return RedirectToAction(nameof(DashBoard));
                }
                return RedirectToAction(nameof(Form), new { ID = 0 });
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["Message"] = "Form Validation Error.";
                return RedirectToAction(nameof(Form), new { ID = 0 });
            }
        }

        public async Task<IActionResult> GetSearchData(ItemGroupModel model)
        {
            model.Mode = "Search";
            model = await _IItemGroup.GetDashboardData(model);
            return PartialView("_ItemGroupDashboard", model);
        }



        public ResponseResult isDuplicate(string ColVal, string ColName)
        {
            var Result = _IDataLogic.isDuplicate(ColVal, ColName, "Itemgroup_master");
            return Result;
        }
    }
}
