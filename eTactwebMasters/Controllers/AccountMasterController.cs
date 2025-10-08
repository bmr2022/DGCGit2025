using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Globalization;
using System.Data;

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
        public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        {
            var response = new ResponseResult();
            var flag = request.Flag;

            try
            {
                DataTable dt = new DataTable();


                dt.Columns.Add("Account_Code", typeof(long));
                dt.Columns.Add("DebCredCode", typeof(string));
                dt.Columns.Add("Party_Code", typeof(string));
                dt.Columns.Add("Entry_Date", typeof(DateTime));
                dt.Columns.Add("Account_Name", typeof(string));
                dt.Columns.Add("DisplayName", typeof(string));
                dt.Columns.Add("ParentAccountCode", typeof(long));
                dt.Columns.Add("MainGroup", typeof(string));
                dt.Columns.Add("AccountType", typeof(string));
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
                dt.Columns.Add("TDSRate", typeof(double));
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
                dt.Columns.Add("RateOfInt", typeof(double));
                dt.Columns.Add("CreditLimit", typeof(double));
                dt.Columns.Add("CreditDays", typeof(string));
                dt.Columns.Add("SSL", typeof(string));
                dt.Columns.Add("BankAccount_No", typeof(string));
                dt.Columns.Add("BankAddress", typeof(string));
                dt.Columns.Add("BankIFSCCode", typeof(string));
                dt.Columns.Add("BankSwiftCode", typeof(string));
                dt.Columns.Add("InterbranchSaleBILL", typeof(string));
                dt.Columns.Add("OursalespersonId", typeof(string));
                dt.Columns.Add("salesemailid", typeof(string));
                dt.Columns.Add("salesmobileno", typeof(string));
                dt.Columns.Add("Approved_By", typeof(string));
                dt.Columns.Add("Approved", typeof(string));
                dt.Columns.Add("ApprovalDate", typeof(DateTime));
                dt.Columns.Add("BlackListed", typeof(string));
                dt.Columns.Add("BlackListed_By", typeof(string));
                dt.Columns.Add("YearCode", typeof(long));
                dt.Columns.Add("Uid", typeof(string));
                dt.Columns.Add("CC", typeof(string));
                dt.Columns.Add("CreatedBy", typeof(long));
                dt.Columns.Add("CreatedOn", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(long));
                dt.Columns.Add("UpdatedOn", typeof(DateTime));
                dt.Columns.Add("Active", typeof(string));
                dt.Columns.Add("BranchCompany", typeof(string));
                dt.Columns.Add("trailbalanceGroupid", typeof(long));
                dt.Columns.Add("SalePersonEmpId", typeof(long));
                dt.Columns.Add("MSMENo", typeof(string));
                dt.Columns.Add("MSMEType", typeof(string));
                dt.Columns.Add("Region", typeof(string));
                dt.Columns.Add("DiscountCategory", typeof(string));
                dt.Columns.Add("DiscCategoryEntryId", typeof(long));
                dt.Columns.Add("GroupDiscountCategory", typeof(long));

                int rowIndex = 1;
                foreach (var excelRow in request.ExcelData)
                {
                    DataRow row = dt.NewRow();

                    foreach (var map in request.Mapping)
                    {
                        string dbCol = map.Key;          // DB column
                        string excelCol = map.Value;     // Excel column name

                        object value = DBNull.Value;     // default

                        if (excelRow.ContainsKey(excelCol) && !string.IsNullOrEmpty(excelRow[excelCol]))
                        {
                            value = excelRow[excelCol];

                            // Convert types for numeric/boolean/date columns if needed
                            Type columnType = dt.Columns[dbCol].DataType;

                            try
                            {
                                if (dbCol == "State")  // <-- Special handling for ParentCode
                                {
                                    string StateName = value.ToString().Trim();

                                    string StateCode = "";
                                    var groupCode = _IAccountMaster.GetStateCode(StateName);

                                    if (groupCode.Result.Result != null && groupCode.Result.Result.Rows.Count > 0)
                                    {
                                        StateCode = groupCode.Result.Result.Rows[0].ItemArray[0];
                                    }

                                    else
                                    {
                                        StateCode = "";
                                    }

                                    if (StateCode != "")
                                        value = StateCode;   // replace with code
                                    else
                                    {
                                        value = "";
                                    }

                                }




                                                                else if (dbCol == "ParentAccountCode")  // <-- Special handling for ParentCode
                                {
                                    string ParentAccName = value.ToString().Trim();

                                    long ParentAccCode = 0;
                                    var groupCode = _IAccountMaster.GetAccountGroupDetail(ParentAccName);
                                    if (groupCode.Result.Result != null && groupCode.Result.Result.Rows.Count > 0)
                                    {
                                        var rowData = groupCode.Result.Result.Rows[0];

                                        ParentAccCode = Convert.ToInt64(rowData["Parent_Account_Code"]);

                                        // Optional: use other details if needed
                                        string mainGroup = rowData["Main_Group"].ToString();
                                        string subGroup = rowData["SubGroup"].ToString();
                                        string subSubGroup = rowData["SubSubGroup"].ToString();
                                        string underGroup = rowData["UnderGroup"].ToString();
                                        string accountType = rowData["Account_Type"].ToString();

                                        int yearcode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                        int CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                        int Uid= Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                        string CC= HttpContext.Session.GetString("Branch");

                                        // Example: store in your DataTable
                                        row["MainGroup"] = mainGroup;
                                        row["SubGroup"] = subGroup;
                                        row["SubSubGroup"] = subSubGroup;
                                        row["UnderGroup"] = underGroup;
                                        row["AccountType"] = accountType;
                                        row["YearCode"] = yearcode;
                                        row["CreatedBy"] = CreatedBy;
                                        row["Uid"] = Uid;
                                        row["CC"] = CC;
                                        row["Active"] = "Y";
                                    }

                                    else
                                    {
                                        ParentAccCode = 0;
                                    }

                                    if (ParentAccCode != 0)
                                        value = ParentAccCode;   // replace with code
                                    else
                                    {
                                        return Json(new
                                        {
                                            StatusCode = 205,
                                            StatusText = $"Please enter a valid ParentCode at Excel row {ParentAccName}"

                                        });
                                    }

                                }

                                else if (columnType == typeof(long))
                                    value = long.Parse(value.ToString());
                                else

                              

                                if (columnType == typeof(int))
                                    value = int.Parse(value.ToString());
                                else if (columnType == typeof(decimal))
                                    value = decimal.Parse(value.ToString());
                                else if (columnType == typeof(bool))
                                {
                                    // Accept 1/0, true/false, Y/N
                                    string s = value.ToString().Trim().ToLower();
                                    value = (s == "1" || s == "true" || s == "y");
                                }
                                else if (columnType == typeof(DateTime))
                                    value = DateTime.Parse(value.ToString());
                                else
                                    value = value.ToString();
                            }
                            catch
                            {
                                value = DBNull.Value; // fallback if conversion fails
                            }
                        }
                        row[dbCol] = value;
                    }

                    dt.Rows.Add(row);
                }

                response = await _IAccountMaster.UpdateMultipleItemDataFromExcel(dt, flag);

                if (response != null)
                {
                    if ((response.StatusText == "Success" || response.StatusText == "Updated") &&
                         (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted))
                    {
                        return Json(new
                        {
                            StatusCode = 200,
                            StatusText = "Data imported successfully",
                            RedirectUrl = Url.Action("ImportandUpdateAccount", "AccountMaster", new { Flag = "" })
                        });
                    }
                    else
                    {
                        return Json(new
                        {

                            StatusText = response.StatusText,
                            statusCode = 201,
                            redirectUrl = ""
                        });
                    }
                }

                return Json(new
                {
                    StatusCode = 500,
                    StatusText = "Unknown error occurred"
                });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

    }
}