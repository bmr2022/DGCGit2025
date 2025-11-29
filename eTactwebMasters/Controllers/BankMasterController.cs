using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    //[Authorize]
    public class BankMasterController : Controller
    {
        private readonly IBankMaster _IBankMaster;
        private readonly IDataLogic _IDataLogic;

        public BankMasterController(IDataLogic iDataLogic, IBankMaster iBankMaster)
        {
            _IDataLogic = iDataLogic;
            _IBankMaster = iBankMaster;
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
            var model = new BankMasterModel();
            model.Mode = "BankDashboard";
            model = await _IBankMaster.GetDashboardData(model);
            //if (model.ParentGroupList != null) {
            model.ParentGroupList = model.ParentGroupList.DistinctBy(x => x.Value).ToList();

            //}
             return View(model);
            //HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
            //return RedirectToAction("DashBoard", "Bank");

        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            var IsDelete = _IDataLogic.IsDelete(ID, "ACCOUNT");

            if (IsDelete == 0)
            {
                var Result = await _IBankMaster.DeleteByID(ID);

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
        [HttpPost]
        [Route("api/validateEmail")]
        public IActionResult ValidateEmail([FromBody] string email)
        {
            // Email validation regex
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            bool isValid = Regex.IsMatch(email, emailPattern);

            if (isValid)
            {
                return Ok(new { message = "Valid email address" });
            }
            else
            {
                return BadRequest(new { message = "Invalid email address" });
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IBankMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<IActionResult> BankMaster(int ID, string Mode)
        {
            var model = new BankMasterModel();

            if (ID == 0)
            {
                //model.StateList = _IBankMaster.GetDropDownList("StateMaster");
                //model.ParentGroupList = _IBankMaster.GetDropDownList("VPrimaryAccountHeadMaster");
                model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_GetDropDownList");
                model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMasterForBank", "SP_AccountMaster");
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
                model = await _IBankMaster.GetByID(ID);
                model.Mode = Mode;
                 model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_GetDropDownList");
                model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMasterForBank", "SP_AccountMaster");
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
           // HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
            //return RedirectToAction("BankMaster", "Bank");
           return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Form(BankMasterModel model)
        {
            ModelState.Remove("TxPageName");
            if (ModelState.IsValid)
            {
                model.Mode = model.Mode != "U" ? "Insert" : "Update";                //model.CreatedBy = Constants.UserID;

                var Result = await _IBankMaster.SaveAccountMaster(model);

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
            var Result = await _IBankMaster.GetParentGroupDetail(ID);

            string JsonString = JsonConvert.SerializeObject(Result);

            return Json(JsonString);
        }

        public async Task<IActionResult> GetSearchData(BankMasterModel model)
        {
            model.Mode = "BANKSearch";
            model = await _IBankMaster.GetDashboardData(model);
            return PartialView("_BankMasterDashboard", model);
        }
        public async Task<IActionResult> GetDetailData(BankMasterModel model)
        {
            model.Mode = "DetailSearch";
            model = await _IBankMaster.GetDetailDashboardData(model);
            return PartialView("_BankMasterDashboard", model);
        } 

        public ResponseResult isDuplicate(string ColVal, string ColName)
        {
            var Result = _IDataLogic.isDuplicate(ColVal, ColName, "Account_Head_Master");
            return Result;
        }
    }
}