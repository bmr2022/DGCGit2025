using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace eTactWeb.Controllers
{
    public class ItemCategoryController : Controller
    {
        private readonly IItemCategory _IItemCategory;
        private readonly IDataLogic _IDataLogic;
        public ItemCategoryController(IDataLogic iDataLogic, IItemCategory iItemCategory)
        {
            _IDataLogic = iDataLogic;
            _IItemCategory = iItemCategory;
        }
        public async Task<IActionResult> DashBoard()
        {
            var model = new ItemCategoryModel();
            model.Mode = "Dashboard";
            model = await _IItemCategory.GetDashboardData(model).ConfigureAwait(true);
            model.ItemCatList = model.ItemCatList.DistinctBy(x => x.Entry_id).ToList();
            return View(model);
        }
        public async Task<IActionResult> GetSearchData(string CategoryName, string TypeItem)
        {
            var model = new ItemCategoryModel();
            model.Mode = "Search";
            model = await _IItemCategory.GetSearchData(model, CategoryName, TypeItem).ConfigureAwait(true);
            model.ItemCatList = model.ItemCatList.DistinctBy(x => x.Entry_id).ToList();
            return PartialView("_ItemCatDashboard", model);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IItemCategory.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckBeforeUpdate(int Type)
        {
            var JSON = await _IItemCategory.CheckBeforeUpdate(Type);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetAllItemCategory()
        {
            var JSON = await _IItemCategory.GetAllItemCategory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var IsDelete = _IDataLogic.IsDelete(ID, "Entry_id");

            if (IsDelete == 0)
            {
                var Result = await _IItemCategory.DeleteByID(ID).ConfigureAwait(true);

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
        public async Task<IActionResult> Form(int ID, string Mode)
        {
            var model = new ItemCategoryModel();
            if (ID == 0)
            {
                model.Year_code = Constants.FinincialYear;
                model.Entry_Date = DateTime.Today;
            }
            else
            {
                model = await _IItemCategory.GetByID(ID).ConfigureAwait(true);
                model.Mode = Mode;
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(ItemCategoryModel model)
        {
            ModelState.Remove("TxPageName");
            if (1 == 1) //(ModelState.IsValid) 
            {
                model.Mode = model.ID == 0 ? "Insert" : "Update";
                model.CreatedBy = Constants.UserID;
                //in case of save mode
                if (model.ID == 0)
                {
                    model.Year_code = Constants.FinincialYear;
                    model.Entry_Date = DateTime.Today;
                }

                //  var Result = await _IItemCategory.SaveItemCategoryMaster(model).ConfigureAwait(false);
                var Result = await _IItemCategory.SaveItemCategoryMaster(model).ConfigureAwait(true);

                if (Result == null)
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Something Went Wrong, Please Try Again while saveing Item Category.";
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
                    TempData["300"] = "300";
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
    }
}

