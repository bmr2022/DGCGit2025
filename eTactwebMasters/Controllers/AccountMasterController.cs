using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Globalization;

namespace eTactWeb.Controllers
{
    [Authorize]
    public class AccountMasterController : Controller
    {
        private readonly IAccountMaster _IAccountMaster;
        private readonly IDataLogic _IDataLogic;

        public AccountMasterController(IDataLogic iDataLogic, IAccountMaster iAccountMaster)
        {
            _IDataLogic = iDataLogic;
            _IAccountMaster = iAccountMaster;
        }

        [HttpPost]
        public JsonResult AutoComplete(string ColumnName, string prefix)
        {
            var iList = _IDataLogic.AutoComplete("Account_Head_Master", ColumnName,"","",0,0);
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
            var model = new AccountMasterModel();
            model.Mode = "Dashboard";
            model = await _IAccountMaster.GetDashboardData(model);
            model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
            HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
            return View(model);
           // return RedirectToAction("DashBoard", "AccountMaster");

        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            var IsDelete = _IDataLogic.IsDelete(ID, "AccountMaster");

            if (IsDelete == 0)
            {
                var Result = await _IAccountMaster.DeleteByID(ID);

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
            var JSON = await _IAccountMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> Form(int ID, string Mode)
        {
            var model = new AccountMasterModel();

            if (ID == 0)
            {
                //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
                //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
                model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");
                model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
                model.DiscountCategoryList = await _IDataLogic.GetDropDownList("GetDiscountCategory", "SP_AccountMaster");
                model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList("GetGroupDiscountCategory", "SP_AccountMaster");
                model.RegionList = await _IDataLogic.GetDropDownList("GetRegion", "SP_AccountMaster");
                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

                model.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
                //productsList.Insert(0, new SelectListItem()
                //{
                //    Text = "----Select----",
                //    Value = string.Empty
                //});
            }
            else
            {
                model = await _IAccountMaster.GetByID(ID);
                model.Mode = Mode;
                //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
                //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
                model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");
                model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
                model.DiscountCategoryList = await _IDataLogic.GetDropDownList("GetDiscountCategory", "SP_AccountMaster");
                model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList("GetGroupDiscountCategory", "SP_AccountMaster");
                model.RegionList = await _IDataLogic.GetDropDownList("GetRegion", "SP_AccountMaster");
            }
            if (Mode != "U")
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.CreatedByName = HttpContext.Session.GetString("EmpName");
                model.CreatedOn = DateTime.Now;
            }
            else if (Mode == "U")
            {
                model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                model.UpdatedOn = DateTime.Now;
            }
            HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
              return View(model);
           // return RedirectToAction("Form", "AccountMaster");
           // return View("Views/AccountMaster/Form.cshtml");
        }
       
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(AccountMasterModel model)
        {
            ModelState.Remove("TxPageName");
            if (ModelState.IsValid)
            {
                model.Mode = model.ID == 0 ? "Insert" : "Update";
                //model.CreatedBy = Constants.UserID;

                var Result = await _IAccountMaster.SaveAccountMaster(model);

                if (Result == null)
                {
                    ViewBag.isSuccess = false;
                    TempData["Message"] = "Something Went Wrong, Please Try Again.";
                }
                else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.isSuccess = true;
                    TempData["200"] = "200";
                    return RedirectToAction("Dashboard", "AccountMaster");
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

        [HttpPost]
        public async Task<JsonResult> GetParentGroupDetail(string ID)
        {
            var Result = await _IAccountMaster.GetParentGroupDetail(ID);

            string JsonString = JsonConvert.SerializeObject(Result);

            return Json(JsonString);
        }

        public async Task<IActionResult> GetSearchData(AccountMasterModel model)
        {
            model.Mode = "Search";
            model = await _IAccountMaster.GetDashboardData(model);
            return PartialView("_AccMasterDashboard", model);
        }
        public async Task<IActionResult> GetDetailData(AccountMasterModel model)
        {
            model.Mode = "DetailSearch";
            model = await _IAccountMaster.GetDetailDashboardData(model);
            return PartialView("_AccMasterDashboard", model);
        }

        public async Task<IActionResult> GetTDSPartyList()
        {
            var Result = await _IAccountMaster.GetTDSPartyList().ConfigureAwait(true);
            return Json(JsonConvert.SerializeObject(Result));
        }

        public ResponseResult isDuplicate(string ColVal, string ColName)
        {
            var Result = _IDataLogic.isDuplicate(ColVal, ColName, "Account_Head_Master");
            return Result;
        }
    }
}