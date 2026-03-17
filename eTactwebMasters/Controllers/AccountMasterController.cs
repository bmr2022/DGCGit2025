using ClosedXML.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    [Authorize]
    public class AccountMasterController : Controller
    {
        private readonly IAccountMaster _IAccountMaster;
        private readonly IDataLogic _IDataLogic;
        public EncryptDecrypt EncryptDecrypt { get; }

        public AccountMasterController(IDataLogic iDataLogic, IAccountMaster iAccountMaster, EncryptDecrypt encryptDecrypt)
        {
            _IDataLogic = iDataLogic;
            _IAccountMaster = iAccountMaster;
            EncryptDecrypt = encryptDecrypt;
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

        //public async Task<IActionResult> DashBoard()
        //{
        //    var model = new AccountMasterModel();
        //    model.Mode = "Dashboard";
        //    model = await _IAccountMaster.GetDashboardData(model);
        //    model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
        //    HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
        //    string jsonData = JsonConvert.SerializeObject(model);
        //    HttpContext.Session.SetString("AccountList", jsonData);
        //    return View(model);
        //   // return RedirectToAction("DashBoard", "AccountMaster");

        //}


        public async Task<IActionResult> DashBoard()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _IAccountMaster.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var table = rights.Result.Tables[0];

            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
            if (!(optAll || optView || optUpdate || optDelete))
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var model = new AccountMasterModel();

            model.Mode = "Dashboard";
            //model = await _IAccountMaster.GetDashboardData(model, userID);
            model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
            HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
            string jsonData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("AccountList", jsonData);
            return View(model);
            // return RedirectToAction("DashBoard", "AccountMaster");

        }


        public async Task<IActionResult> GetSearchData(AccountMasterModel model, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50)
        {
            var DashboardRepoType = model.DashboardType;

            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            // 🔥 Call BLL (Dynamic ResponseResult)
            var result = await _IAccountMaster.GetDashboardData(
               model, userID, DashboardRepoType);

            if (result == null || !(result.Result is DataTable dt))
            {
                model.TotalRecords = 0;
                model.Rows = new List<Dictionary<string, object>>();
                return PartialView("_AccMasterDashboard", model);
            }

            model.TotalRecords = dt.Rows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            // 🔥 Dynamic Headers
            model.Headers = dt.Columns
                .Cast<DataColumn>()
                .Select(c => new DashboardColumn
                {
                    Title = c.ColumnName,
                    Field = c.ColumnName
                })
                .ToList();

            // 🔥 Dynamic Rows with Pagination
            model.Rows = dt.AsEnumerable()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => dt.Columns
                    .Cast<DataColumn>()
                    .ToDictionary(
                        c => c.ColumnName,
                        c => r[c] == DBNull.Value ? null : r[c]
                    ))
                .ToList();
            // 🔹 Store in session if needed
            HttpContext.Session.SetString("AccountList", JsonConvert.SerializeObject(dt));


            return PartialView("_AccMasterDashboard", model);
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
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _IAccountMaster.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var table = rights.Result.Tables[0];
            //string encID = Request.Query["ID"].ToString();
            string encID = RouteData.Values["id"]?.ToString();
            if (!string.IsNullOrEmpty(encID) && encID != "0" && !string.IsNullOrEmpty(Mode))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);
                ID = decryptedID;
                Mode = decryptedMode;

            }

            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (ID <= 0)
            {
                if (!optSave)
                {
                    return RedirectToAction("DashBoard", "AccountMaster");
                }
                //if (!(optAll || optSave))
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}

            }

            var model = new AccountMasterModel();

            if (ID == 0)
            {
                //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
                //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
                model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");
                model.DatabaseList = await _IAccountMaster.GetDropDownList("GetDatabaseName");
                model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
                model.DiscountCategoryList = await _IDataLogic.GetDropDownList("GetDiscountCategory", "SP_AccountMaster");
                model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList("GetGroupDiscountCategory", "SP_AccountMaster");
                model.RegionList = await _IDataLogic.GetDropDownList("GetRegion", "SP_AccountMaster");
                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

                model.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy");
                model.CC = HttpContext.Session.GetString("Branch");
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
                model.ID = ID;
                model.CC = HttpContext.Session.GetString("Branch");

                //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
                //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
                model.DatabaseList = await _IAccountMaster.GetDropDownList("GetDatabaseName");
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
            ModelState.Clear();
            HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
            return View(model);
            // return RedirectToAction("Form", "AccountMaster");
            // return View("Views/AccountMaster/Form.cshtml");
        }


        //public async Task<IActionResult> Form(int ID, string Mode)
        //{
        //    var model = new AccountMasterModel();

        //    if (ID == 0)
        //    {
        //        //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
        //        //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
        //        model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");
        //        model.DatabaseList = await _IAccountMaster.GetDropDownList("GetDatabaseName");
        //        model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
        //        model.DiscountCategoryList = await _IDataLogic.GetDropDownList("GetDiscountCategory", "SP_AccountMaster");
        //        model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList("GetGroupDiscountCategory", "SP_AccountMaster");
        //        model.RegionList = await _IDataLogic.GetDropDownList("GetRegion", "SP_AccountMaster");
        //        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

        //        model.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
        //        //productsList.Insert(0, new SelectListItem()
        //        //{
        //        //    Text = "----Select----",
        //        //    Value = string.Empty
        //        //});
        //    }
        //    else
        //    {
        //        model = await _IAccountMaster.GetByID(ID);
        //        model.Mode = Mode;
        //        //model.StateList = _IAccountMaster.GetDropDownList("StateMaster");
        //        //model.ParentGroupList = _IAccountMaster.GetDropDownList("VPrimaryAccountHeadMaster");
        //        model.DatabaseList = await _IAccountMaster.GetDropDownList("GetDatabaseName");
        //        model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");
        //        model.ParentGroupList = await _IDataLogic.GetDropDownList("VPrimaryAccountHeadMaster", "SP_AccountMaster");
        //        model.DiscountCategoryList = await _IDataLogic.GetDropDownList("GetDiscountCategory", "SP_AccountMaster");
        //        model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList("GetGroupDiscountCategory", "SP_AccountMaster");
        //        model.RegionList = await _IDataLogic.GetDropDownList("GetRegion", "SP_AccountMaster");
        //    }
        //    if (Mode != "U")
        //    {
        //        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //        model.CreatedByName = HttpContext.Session.GetString("EmpName");
        //        model.CreatedOn = DateTime.Now;
        //    }
        //    else if (Mode == "U")
        //    {
        //        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
        //        model.UpdatedOn = DateTime.Now;
        //    }
        //    HttpContext.Session.SetString("Model", JsonConvert.SerializeObject(model));
        //      return View(model);
        //   // return RedirectToAction("Form", "AccountMaster");
        //   // return View("Views/AccountMaster/Form.cshtml");
        //}

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
            //if (ModelState.IsValid)
            //{
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            if (model.Mode == "U")   // UPDATE
            {
                model.Mode = "Update";
                // DO NOT touch CreatedBy
                model.UpdatedBy = userId;
            }
            else                     // INSERT
            {
                model.Mode = "Insert";
                model.CreatedBy = userId;
            }
            //model.CreatedBy = Constants.UserID;
            model.MachineName = HttpContext.Session.GetString("ClientMachineName");
            model.IPAddress = HttpContext.Session.GetString("ClientIP");
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
                //return RedirectToAction(nameof(Form), new { ID = 0 });
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
                //return RedirectToAction(nameof(Form), new { ID = 0 });
            }
            else if (!string.IsNullOrEmpty(Result.StatusText))
            {
                // If SP returned a message (like adjustment error)
                ViewBag.isSuccess = false;
                TempData["ErrorMessage"] = Result.StatusText;
                await BindDropdowns(model);   // VERY IMPORTANT
                model.Mode = model.Mode == "Update" ? "U" : "I";

                return View(model);
            }
            return RedirectToAction(nameof(Form), new { ID = 0 });
            //   return RedirectToAction("Form", "AccountMaster");

        }

        private async Task BindDropdowns(AccountMasterModel model)
        {
            model.StateList = await _IDataLogic.GetDropDownList("StateMaster", "SP_AccountMaster");

            model.ParentGroupList = await _IDataLogic.GetDropDownList(
                "VPrimaryAccountHeadMaster",
                "SP_AccountMaster"
            );

            model.DiscountCategoryList = await _IDataLogic.GetDropDownList(
                "GetDiscountCategory",
                "SP_AccountMaster"
            );

            model.GroupDiscountCategoryList = await _IDataLogic.GetDropDownList(
                "GetGroupDiscountCategory",
                "SP_AccountMaster"
            );

            model.RegionList = await _IDataLogic.GetDropDownList(
                "GetRegion",
                "SP_AccountMaster"
            );

            model.DatabaseList = await _IAccountMaster.GetDropDownList("GetDatabaseName");

            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Form(AccountMasterModel model)
        //{
        //    ModelState.Remove("TxPageName");
        //    if (ModelState.IsValid)
        //    {
        //        model.Mode = model.ID == 0 ? "Insert" : "Update";
        //        //model.CreatedBy = Constants.UserID;

        //        var Result = await _IAccountMaster.SaveAccountMaster(model);

        //        if (Result == null)
        //        {
        //            ViewBag.isSuccess = false;
        //            TempData["Message"] = "Something Went Wrong, Please Try Again.";
        //        }
        //        else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
        //        {
        //            ViewBag.isSuccess = true;
        //            TempData["200"] = "200";
        //            return RedirectToAction(nameof(Form), new { ID = 0 });
        //        }
        //        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.Ambiguous)
        //        {
        //            ViewBag.isSuccess = false;
        //            TempData["300"] = "300";
        //        }
        //        else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
        //        {
        //            ViewBag.isSuccess = true;
        //            TempData["202"] = "202";

        //            return RedirectToAction(nameof(Form), new { ID = 0 });
        //        }
        //        return RedirectToAction(nameof(Form), new { ID = 0 });
        //    }
        //    else
        //    {
        //        ViewBag.isSuccess = false;
        //        TempData["Message"] = "Form Validation Error.";
        //        return RedirectToAction(nameof(Form), new { ID = 0 });
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> GetParentGroupDetail(string ID)
        {
            var Result = await _IAccountMaster.GetParentGroupDetail(ID);

            string JsonString = JsonConvert.SerializeObject(Result);

            return Json(JsonString);
        }

        //public async Task<IActionResult> GetSearchData(AccountMasterModel model)
        //{
        //    model.Mode = "Search";
        //    model = await _IAccountMaster.GetDashboardData(model);
        //    string jsonData = JsonConvert.SerializeObject(model);
        //    HttpContext.Session.SetString("AccountList", jsonData);
        //    return PartialView("_AccMasterDashboard", model);
        //}
        //public async Task<IActionResult> GetDetailData(AccountMasterModel model)
        //{
        //    model.Mode = "DetailSearch";
        //    model = await _IAccountMaster.GetDetailDashboardData(model);
        //    string jsonData = JsonConvert.SerializeObject(model);
        //    HttpContext.Session.SetString("AccountList", jsonData);
        //    return PartialView("_AccMasterDashboard", model);
        //}

        [HttpGet]
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 50)
        {
            AccountMasterModel model = new AccountMasterModel();

            // 1️⃣ Get session data
            string modelJson = HttpContext.Session.GetString("AccountList");

            if (string.IsNullOrWhiteSpace(modelJson))
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_AccMasterDashboard", model);
            }

            // 2️⃣ Deserialize rows
            var allRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelJson);

            if (allRows == null || allRows.Count == 0)
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_AccMasterDashboard", model);
            }

            // 3️⃣ Dynamic search (all columns)
            var filteredRows = string.IsNullOrWhiteSpace(searchString)
                ? allRows
                : allRows.Where(row =>
                    row.Values.Any(val =>
                        val != null &&
                        val.ToString()
                           .Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                  .ToList();

            // fallback → show all
            if (filteredRows.Count == 0)
                filteredRows = allRows;

            // 4️⃣ Dynamic headers (from keys)
            model.Headers = filteredRows.First()
                .Keys
                .Select(k => new DashboardColumn
                {
                    Title = k,
                    Field = k
                })
                .ToList();

            // 5️⃣ Pagination
            model.TotalRecords = filteredRows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Rows = filteredRows
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PartialView("_AccMasterDashboard", model);
        }


        public IActionResult ExportAccountExcel(string ReportType)
        {
            string json = HttpContext.Session.GetString("AccountList");

            if (string.IsNullOrEmpty(json))
                return Content("No data available for export.");

            // Deserialize as dynamic list
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AccountMaster");

            if (data != null && data.Count > 0)
            {
                // 🔥 Add Headers
                int col = 1;
                foreach (var key in data[0].Keys)
                {
                    worksheet.Cell(1, col).Value = key;
                    worksheet.Cell(1, col).Style.Font.Bold = true;
                    col++;
                }

                // 🔥 Add Rows
                int row = 2;
                foreach (var item in data)
                {
                    col = 1;
                    foreach (var value in item.Values)
                    {
                        worksheet.Cell(row, col).Value = value?.ToString();
                        col++;
                    }
                    row++;
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AccountMaster.xlsx"
            );
        }

        public async Task<IActionResult> GetTDSPartyList()
        {
            var Result = await _IAccountMaster.GetTDSPartyList().ConfigureAwait(true);
            return Json(JsonConvert.SerializeObject(Result));
        }

        public async Task<IActionResult> GetSalePersonName()
        {
            var Result = await _IAccountMaster.GetSalePersonName().ConfigureAwait(true);
            return Json(JsonConvert.SerializeObject(Result));
        }

        public ResponseResult isDuplicate(string ColVal, string ColName)
        {
            var Result = _IDataLogic.isDuplicate(ColVal, ColName, "Account_Head_Master");
            return Result;
        }
        public async Task<IActionResult> ImportandUpdateAccount(
     string? Item_Name = "",
     string? PartCode = "",
     string? ParentCode = "",
     string? ItemType = "",
     string? HSNNO = "",
     string? UniversalPartCode = "",
     string? Flag = "")
        {
            var model = new AccountMasterModel();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            //model.MasterList = await _IItemMaster.GetDashBoardData(Item_Name, PartCode, ParentCode, ItemType, HSNNO, UniversalPartCode, Flag);

            //HttpContext.Session.SetString("KeyItemListSearch", JsonConvert.SerializeObject(model.MasterList));

            return View(model);
        }
        [HttpPost]
        //public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        //{
        //    var response = new ResponseResult();
        //    var flag = request.Flag;

        //    try
        //    {
        //        int pageSize = request.PageSize > 0 ? request.PageSize : 500;
        //        int pageNo = request.PageNo <= 0 ? 1 : request.PageNo;

        //        // Calculate total pages from incoming full dataset length if client provided full list
        //        int totalPages = request.TotalPages;
        //        // Get only the current page rows (client is sending page-wise; but if they send less rows, handle gracefully)
        //        //var paginatedData = request.ExcelData != null
        //        //    ? request.ExcelData.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList()
        //        //    : new List<Dictionary<string, string>>();

        //        var paginatedData = request.ExcelData ?? new List<Dictionary<string, string>>();
        //        DataTable dt = new DataTable();

        //        dt.Columns.Add("Account_Name", typeof(string));
        //        dt.Columns.Add("Party_Code", typeof(string));
        //        dt.Columns.Add("DisplayName", typeof(string));
        //        dt.Columns.Add("AccountType", typeof(string));
        //        dt.Columns.Add("ParentAccount", typeof(string));

        //        dt.Columns.Add("MainGroup", typeof(string));
        //        dt.Columns.Add("SubGroup", typeof(string));
        //        dt.Columns.Add("SubSubGroup", typeof(string));
        //        dt.Columns.Add("UnderGroup", typeof(string));
        //        dt.Columns.Add("ComAddress", typeof(string));
        //        dt.Columns.Add("ComAddress1", typeof(string));
        //        dt.Columns.Add("PinCode", typeof(string));
        //        dt.Columns.Add("City", typeof(string));
        //        dt.Columns.Add("State", typeof(string));
        //        dt.Columns.Add("Country", typeof(string));
        //        dt.Columns.Add("PhoneNo", typeof(string));
        //        dt.Columns.Add("MobileNo", typeof(string));
        //        dt.Columns.Add("ContactPerson", typeof(string));
        //        dt.Columns.Add("PartyType", typeof(string));
        //        dt.Columns.Add("GSTRegistered", typeof(string));
        //        dt.Columns.Add("GSTNO", typeof(string));
        //        dt.Columns.Add("GSTPartyTypes", typeof(string));
        //        dt.Columns.Add("GSTTAXTYPE", typeof(string));
        //        dt.Columns.Add("Segment", typeof(string));
        //        dt.Columns.Add("SSLNo", typeof(string));
        //        dt.Columns.Add("PANNO", typeof(string));
        //        dt.Columns.Add("TDS", typeof(string));
        //        dt.Columns.Add("TDSRate", typeof(decimal));               // decimal
        //        dt.Columns.Add("TDSPartyCategery", typeof(string));
        //        dt.Columns.Add("ResponsibleEmployee", typeof(string));
        //        dt.Columns.Add("ResponsibleEmpContactNo", typeof(string));
        //        dt.Columns.Add("SalesPersonName", typeof(string));
        //        dt.Columns.Add("SalesPersonEmailId", typeof(string));
        //        dt.Columns.Add("SalesPersonMobile", typeof(string));
        //        dt.Columns.Add("PurchPersonName", typeof(string));
        //        dt.Columns.Add("PurchasePersonEmailId", typeof(string));
        //        dt.Columns.Add("PurchMobileNo", typeof(string));
        //        dt.Columns.Add("QCPersonEmailId", typeof(string));
        //        dt.Columns.Add("WebSite_Add", typeof(string));
        //        dt.Columns.Add("EMail", typeof(string));
        //        dt.Columns.Add("RANGE", typeof(string));
        //        dt.Columns.Add("Division", typeof(string));
        //        dt.Columns.Add("Commodity", typeof(string));
        //        dt.Columns.Add("WorkingAdd1", typeof(string));
        //        dt.Columns.Add("WorkingAdd2", typeof(string));
        //        dt.Columns.Add("RateOfInt", typeof(decimal));             // decimal
        //        dt.Columns.Add("CreditLimit", typeof(decimal));           // decimal
        //        dt.Columns.Add("CreditDays", typeof(int));                // int
        //        dt.Columns.Add("SSL", typeof(string));
        //        dt.Columns.Add("BankAccount_No", typeof(string));
        //        dt.Columns.Add("BankAddress", typeof(string));
        //        dt.Columns.Add("BankIFSCCode", typeof(string));
        //        dt.Columns.Add("BankSwiftCode", typeof(string));
        //        dt.Columns.Add("InterbranchSaleBILL", typeof(string));
        //        dt.Columns.Add("OursalespersonId", typeof(int));          // int
        //        dt.Columns.Add("salesemailid", typeof(string));
        //        dt.Columns.Add("salesmobileno", typeof(string));
        //        dt.Columns.Add("Approved_By", typeof(int));               // int
        //        dt.Columns.Add("Approved", typeof(string));
        //        dt.Columns.Add("ApprovalDate", typeof(DateTime));
        //        dt.Columns.Add("BlackListed", typeof(string));
        //        dt.Columns.Add("BlackListed_By", typeof(int));            // int
        //        dt.Columns.Add("YearCode", typeof(int));                  // int
        //        dt.Columns.Add("Uid", typeof(string));
        //        dt.Columns.Add("CC", typeof(string));
        //        dt.Columns.Add("CreatedBy", typeof(int));                 // int
        //        dt.Columns.Add("CreatedOn", typeof(DateTime));
        //        dt.Columns.Add("UpdatedBy", typeof(int));                 // int
        //        dt.Columns.Add("UpdatedOn", typeof(DateTime));
        //        dt.Columns.Add("Active", typeof(string));
        //        dt.Columns.Add("BranchCompany", typeof(string));
        //        dt.Columns.Add("trailbalanceGroupid", typeof(int));       // int
        //        dt.Columns.Add("SalePersonEmpId", typeof(int));           // int
        //        dt.Columns.Add("MSMENo", typeof(string));
        //        dt.Columns.Add("MSMEType", typeof(string));
        //        dt.Columns.Add("Region", typeof(string));
        //        dt.Columns.Add("DiscountCategory", typeof(string));
        //        dt.Columns.Add("DiscCategoryEntryId", typeof(int));       // int
        //        dt.Columns.Add("GroupDiscountCategory", typeof(int));     // int
        //        dt.Columns.Add("Account_Code", typeof(long));             // bigint
        //        dt.Columns.Add("DebCredCode", typeof(string));
        //        dt.Columns.Add("Entry_Date", typeof(DateTime));
        //        dt.Columns.Add("ParentAccountCode", typeof(long));        // bigint





        //        int rowIndex = 1;
        //        foreach (var excelRow in request.ExcelData)
        //        {
        //            DataRow row = dt.NewRow();

        //            foreach (var map in request.Mapping)
        //            {
        //                string dbCol = map.Key;          // DB column
        //                string excelCol = map.Value;     // Excel column name

        //                object value = DBNull.Value;     // default

        //                if (excelRow.ContainsKey(excelCol) && !string.IsNullOrEmpty(excelRow[excelCol]))
        //                {
        //                    value = excelRow[excelCol];

        //                    // Convert types for numeric/boolean/date columns if needed
        //                    Type columnType = dt.Columns[dbCol].DataType;

        //                    try
        //                    {
        //                        if (dbCol == "State")  // <-- Special handling for ParentCode
        //                        {
        //                            string StateName = value.ToString().Trim();

        //                            string StateCode = "";
        //                            var groupCode = _IAccountMaster.GetStateCode(StateName);

        //                            if (groupCode.Result.Result != null && groupCode.Result.Result.Rows.Count > 0)
        //                            {
        //                                StateCode = groupCode.Result.Result.Rows[0].ItemArray[0];
        //                            }

        //                            else
        //                            {
        //                                StateCode = "";
        //                            }

        //                            if (StateCode != "")
        //                                value = StateCode;   // replace with code
        //                            else
        //                            {
        //                                value = "";
        //                            }

        //                        }




        //                                                        else if (dbCol == "ParentAccount")  // <-- Special handling for ParentCode
        //                        {
        //                            string ParentAccName = value.ToString().Trim();

        //                            long ParentAccCode = 0;
        //                            var groupCode = _IAccountMaster.GetAccountGroupDetail(ParentAccName);
        //                            if (groupCode.Result.Result != null && groupCode.Result.Result.Rows.Count > 0)
        //                            {
        //                                var rowData = groupCode.Result.Result.Rows[0];

        //                                ParentAccCode = Convert.ToInt64(rowData["Account_Code"]);

        //                                // Optional: use other details if needed
        //                                string mainGroup = rowData["Main_Group"].ToString();
        //                                string subGroup = rowData["SubGroup"].ToString();
        //                                string subSubGroup = rowData["SubSubGroup"].ToString();
        //                                string underGroup = rowData["UnderGroup"].ToString();
        //                                string accountType = rowData["Account_Type"].ToString();

        //                                int yearcode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        //                                int CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //                                int Uid= Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //                                string CC= HttpContext.Session.GetString("Branch");

        //                                // Example: store in your DataTable
        //                                row["MainGroup"] = mainGroup;
        //                                row["ParentAccountCode"] = ParentAccCode;
        //                                row["SubGroup"] = subGroup;
        //                                row["SubSubGroup"] = subSubGroup;
        //                                row["UnderGroup"] = underGroup;
        //                                row["AccountType"] = accountType;
        //                                row["YearCode"] = yearcode;
        //                                row["CreatedBy"] = CreatedBy;
        //                                row["Uid"] = Uid;
        //                                row["CC"] = CC;
        //                                row["Active"] = "Y";
        //                            }

        //                            else
        //                            {
        //                                ParentAccCode = 0;
        //                            }

        //                            if (ParentAccCode != 0)
        //                                value = ParentAccCode;   // replace with code
        //                            else
        //                            {
        //                                return Json(new
        //                                {
        //                                    StatusCode = 205,
        //                                    StatusText = $"Please enter a valid ParentCode at Excel row {ParentAccName}"

        //                                });
        //                            }

        //                        }

        //                        else if (columnType == typeof(long))
        //                            value = long.Parse(value.ToString());
        //                        else



        //                        if (columnType == typeof(int))
        //                            value = int.Parse(value.ToString());
        //                        else if (columnType == typeof(decimal))
        //                            value = decimal.Parse(value.ToString());
        //                        else if (columnType == typeof(bool))
        //                        {
        //                            // Accept 1/0, true/false, Y/N
        //                            string s = value.ToString().Trim().ToLower();
        //                            value = (s == "1" || s == "true" || s == "y");
        //                        }
        //                        else if (columnType == typeof(DateTime))
        //                            value = DateTime.Parse(value.ToString());
        //                        else
        //                            value = value.ToString();
        //                    }
        //                    catch
        //                    {
        //                        value = DBNull.Value; // fallback if conversion fails
        //                    }
        //                }
        //                row[dbCol] = value;
        //            }

        //            dt.Rows.Add(row);
        //        }

        //        response = await _IAccountMaster.UpdateMultipleItemDataFromExcel(dt, flag);

        //        if (response != null &&
        //        (response.StatusText == "Success" || response.StatusText == "Updated") &&
        //        (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted))
        //        {
        //            // If this was last page, return redirect url, else return next page
        //            if (request.PageNo < request.TotalPages)
        //            {
        //                return Json(new
        //                {
        //                    StatusCode = 200,
        //                    StatusText = "Page saved",
        //                    CurrentPage = request.PageNo,
        //                    TotalPages = request.TotalPages,
        //                    NextPage = request.PageNo + 1
        //                });
        //            }
        //            else
        //            {
        //                return Json(new
        //                {
        //                    StatusCode = 200,
        //                    StatusText = "All pages saved successfully",
        //                    CurrentPage = request.PageNo,
        //                    TotalPages = request.TotalPages,
        //                    NextPage = 0
        //                });
        //            }
        //        }
        //        else
        //        {
        //            // Partial/custom response from service
        //            string txt = response?.StatusText ?? "Service returned failure";
        //            return Json(new
        //            {
        //                StatusCode = 201,
        //                StatusText = txt,
        //                CurrentPage = pageNo,
        //                TotalPages = totalPages,
        //                NextPage = 0,
        //                RedirectUrl = ""
        //            });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }


        //}



        public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        {
            var response = new ResponseResult();
            List<object> errorList = new List<object>();

            try
            {
                int pageSize = request.PageSize > 0 ? request.PageSize : 500;
                int pageNo = request.PageNo <= 0 ? 1 : request.PageNo;

                DataTable dt = new DataTable();

                dt.Columns.Add("Account_Name", typeof(string));
                dt.Columns.Add("Party_Code", typeof(string));
                dt.Columns.Add("DisplayName", typeof(string));
                dt.Columns.Add("AccountType", typeof(string));
                dt.Columns.Add("ParentAccount", typeof(string));
                dt.Columns.Add("MainGroup", typeof(string));
                dt.Columns.Add("SubGroup", typeof(string));
                dt.Columns.Add("SubSubGroup", typeof(string));
                dt.Columns.Add("UnderGroup", typeof(string));
                dt.Columns.Add("ComAddress", typeof(string));
                dt.Columns.Add("ComAddress1", typeof(string));
                dt.Columns.Add("PinCode", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("State", typeof(string));
                dt.Columns.Add("Country", typeof(string));
                dt.Columns.Add("PhoneNo", typeof(string));
                dt.Columns.Add("MobileNo", typeof(string));
                dt.Columns.Add("ContactPerson", typeof(string));
                dt.Columns.Add("PartyType", typeof(string));
                dt.Columns.Add("GSTRegistered", typeof(string));
                dt.Columns.Add("GSTNO", typeof(string));
                dt.Columns.Add("GSTPartyTypes", typeof(string));
                dt.Columns.Add("GSTTAXTYPE", typeof(string));
                dt.Columns.Add("Segment", typeof(string));
                dt.Columns.Add("SSLNo", typeof(string));
                dt.Columns.Add("PANNO", typeof(string));
                dt.Columns.Add("TDS", typeof(string));
                dt.Columns.Add("TDSRate", typeof(decimal));
                dt.Columns.Add("TDSPartyCategery", typeof(string));
                dt.Columns.Add("ResponsibleEmployee", typeof(string));
                dt.Columns.Add("ResponsibleEmpContactNo", typeof(string));
                dt.Columns.Add("SalesPersonName", typeof(string));
                dt.Columns.Add("SalesPersonEmailId", typeof(string));
                dt.Columns.Add("SalesPersonMobile", typeof(string));
                dt.Columns.Add("PurchPersonName", typeof(string));
                dt.Columns.Add("PurchasePersonEmailId", typeof(string));
                dt.Columns.Add("PurchMobileNo", typeof(string));
                dt.Columns.Add("QCPersonEmailId", typeof(string));
                dt.Columns.Add("WebSite_Add", typeof(string));
                dt.Columns.Add("EMail", typeof(string));
                dt.Columns.Add("RANGE", typeof(string));
                dt.Columns.Add("Division", typeof(string));
                dt.Columns.Add("Commodity", typeof(string));
                dt.Columns.Add("WorkingAdd1", typeof(string));
                dt.Columns.Add("WorkingAdd2", typeof(string));
                dt.Columns.Add("RateOfInt", typeof(decimal));
                dt.Columns.Add("CreditLimit", typeof(decimal));
                dt.Columns.Add("CreditDays", typeof(int));
                dt.Columns.Add("SSL", typeof(string));
                dt.Columns.Add("BankAccount_No", typeof(string));
                dt.Columns.Add("BankAddress", typeof(string));
                dt.Columns.Add("BankIFSCCode", typeof(string));
                dt.Columns.Add("BankSwiftCode", typeof(string));
                dt.Columns.Add("InterbranchSaleBILL", typeof(string));
                dt.Columns.Add("OursalespersonId", typeof(int));
                dt.Columns.Add("salesemailid", typeof(string));
                dt.Columns.Add("salesmobileno", typeof(string));
                dt.Columns.Add("Approved_By", typeof(int));
                dt.Columns.Add("Approved", typeof(string));
                dt.Columns.Add("ApprovalDate", typeof(DateTime));
                dt.Columns.Add("BlackListed", typeof(string));
                dt.Columns.Add("BlackListed_By", typeof(int));
                dt.Columns.Add("YearCode", typeof(int));
                dt.Columns.Add("Uid", typeof(string));
                dt.Columns.Add("CC", typeof(string));
                dt.Columns.Add("CreatedBy", typeof(int));
                dt.Columns.Add("CreatedOn", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(int));
                dt.Columns.Add("UpdatedOn", typeof(DateTime));
                dt.Columns.Add("Active", typeof(string));
                dt.Columns.Add("BranchCompany", typeof(string));
                dt.Columns.Add("trailbalanceGroupid", typeof(int));
                dt.Columns.Add("SalePersonEmpId", typeof(int));
                dt.Columns.Add("MSMENo", typeof(string));
                dt.Columns.Add("MSMEType", typeof(string));
                dt.Columns.Add("Region", typeof(string));
                dt.Columns.Add("DiscountCategory", typeof(string));
                dt.Columns.Add("DiscCategoryEntryId", typeof(int));
                dt.Columns.Add("GroupDiscountCategory", typeof(int));
                dt.Columns.Add("Account_Code", typeof(long));
                dt.Columns.Add("DebCredCode", typeof(string));
                dt.Columns.Add("Entry_Date", typeof(DateTime));
                dt.Columns.Add("ParentAccountCode", typeof(long));

                int rowIndex = ((pageNo - 1) * pageSize) + 1;

                foreach (var excelRow in request.ExcelData)
                {
                    bool rowHasError = false;
                    DataRow row = dt.NewRow();

                    foreach (var map in request.Mapping)
                    {
                        string dbCol = map.Key;
                        string excelCol = map.Value;

                        if (!dt.Columns.Contains(dbCol))
                            continue;

                        object value = DBNull.Value;

                        if (excelRow.ContainsKey(excelCol) && !string.IsNullOrWhiteSpace(excelRow[excelCol]))
                        {
                            value = excelRow[excelCol];

                            try
                            {
                                if (dbCol == "State")
                                {
                                    var st = await _IAccountMaster.GetStateCode(value.ToString());
                                    if (st?.Result != null && st.Result.Rows.Count > 0)
                                        value = st.Result.Rows[0][0].ToString();
                                    else
                                    {
                                        errorList.Add(new { Row = rowIndex, Column = "State", Message = $"Invalid State at row '{rowIndex}' " });


                                        rowHasError = true;
                                        break;
                                    }
                                }
                                else if (dbCol == "ParentAccount")
                                {
                                    var acc = await _IAccountMaster.GetAccountGroupDetail(value.ToString());
                                    if (acc?.Result != null && acc.Result.Rows.Count > 0)
                                    {
                                        var r = acc.Result.Rows[0];
                                        row["ParentAccountCode"] = Convert.ToInt64(r["Account_Code"]);
                                        row["MainGroup"] = r["Main_Group"].ToString();
                                        row["SubGroup"] = r["SubGroup"].ToString();
                                        row["SubSubGroup"] = r["SubSubGroup"].ToString();
                                        row["UnderGroup"] = r["UnderGroup"].ToString();
                                        row["AccountType"] = r["Account_Type"].ToString();
                                        row["YearCode"] = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                        row["CreatedBy"] = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                        row["Uid"] = HttpContext.Session.GetString("UID");
                                        row["CC"] = HttpContext.Session.GetString("Branch");
                                        row["Active"] = "Y";
                                        continue;
                                    }
                                    else
                                    {
                                        errorList.Add(new { Row = rowIndex, Column = "ParentAccount", Message = $"Invalid ParentAccount at row '{rowIndex}'" });
                                        rowHasError = true;
                                        break;
                                    }
                                }

                                row[dbCol] = value;
                            }
                            catch
                            {
                                errorList.Add(new { Row = rowIndex, Column = dbCol, Message = $"Invalid value at row '{rowIndex}'" });
                                rowHasError = true;
                                break;
                            }
                        }
                    }

                    if (!rowHasError)
                        dt.Rows.Add(row);

                    rowIndex++;
                }

                if (dt.Rows.Count > 0)
                    response = await _IAccountMaster.UpdateMultipleItemDataFromExcel(dt, request.Flag);

                //return Json(new
                //{
                //    StatusCode = 200,
                //    CurrentPage = pageNo,
                //    TotalPages = request.TotalPages,
                //    NextPage = pageNo < request.TotalPages ? pageNo + 1 : 0,
                //    Errors = errorList
                //});


                // Build response
                if (response != null &&
                    (response.StatusText == "Success" || response.StatusText == "Updated") &&
                    (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted))
                {
                    // If this was last page, return redirect url, else return next page
                    if (request.PageNo < request.TotalPages)
                    {
                        return Json(new
                        {
                            StatusCode = 200,
                            StatusText = "Page saved",
                            CurrentPage = request.PageNo,
                            TotalPages = request.TotalPages,
                            NextPage = request.PageNo + 1,
                            // 🔴 CHANGE
                            failedRows = errorList

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            StatusCode = 200,
                            StatusText = "All pages saved successfully",
                            CurrentPage = request.PageNo,
                            TotalPages = request.TotalPages,
                            NextPage = 0,
                            // 🔴 CHANGE
                            failedRows = errorList
                        });
                    }
                }
                else
                {
                    // Partial/custom response from service
                    string txt = response?.StatusText ?? "Service returned failure";
                    return Json(new
                    {
                        StatusCode = 201,
                        StatusText = txt,
                        CurrentPage = pageNo,
                        TotalPages = request.TotalPages,
                        NextPage = 0,
                        RedirectUrl = "",
                        // 🔴 CHANGE
                        failedRows = errorList
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }


    }
}