using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data;

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


        public async Task<IActionResult> ImportandUpdateItemCategory()
        { 
            var model = new ItemCategoryModel();
           

            return View(model);
        }
        public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        {
            var response = new ResponseResult();
            var flag = request.Flag;

            try
            {
                DataTable dt = new DataTable();


                dt.Columns.Add("Entry_id", typeof(long));
                dt.Columns.Add("Entry_Date", typeof(DateTime));
                dt.Columns.Add("Year_code", typeof(long));
                dt.Columns.Add("Type_Item", typeof(string));
                dt.Columns.Add("Main_Category_Type", typeof(string));
                dt.Columns.Add("CC", typeof(string));
                dt.Columns.Add("Uid", typeof(long));
                dt.Columns.Add("Category_Code", typeof(string));
               

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




                              
                                        int yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                       
                                        int Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                        string CC = HttpContext.Session.GetString("Branch");

                                       
                                      
                                        row["Year_code"] = yearcode;
                                       
                                        row["Uid"] = Uid;
                                        row["CC"] = CC;



                                if (dbCol == "Main_Category_Type")
                                {
                                    string itemCat = value?.ToString().Trim() ?? string.Empty;

                                    // ✅ Fetch category dataset
                                    var catCodeResponse = await _IItemCategory.GetAllItemCategory();
                                    var categoryDataSet = catCodeResponse?.Result as DataSet;

                                    // ✅ Validate that dataset and table exist
                                    if (categoryDataSet == null || categoryDataSet.Tables.Count == 0)
                                    {
                                        return Json(new
                                        {
                                            StatusCode = 500,
                                            StatusText = "Error: Could not fetch main category list from database."
                                        });
                                    }

                                    var categoryTable = categoryDataSet.Tables[0];

                                    // ✅ Extract allowed category names
                                    var allowedTypes = categoryTable.AsEnumerable()
                                        .Select(r => r["Main_Category_Type"].ToString().Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToList();

                                    // ✅ Check if input matches any allowed type
                                    bool isValid = allowedTypes.Any(t =>
                                        t.Equals(itemCat, StringComparison.OrdinalIgnoreCase));

                                    if (!isValid)
                                    {
                                        return Json(new
                                        {
                                            StatusCode = 201,
                                            StatusText = $"Invalid Main_Category_Type '{itemCat}' at Row {rowIndex}. " +
                                                         $"Allowed types: {string.Join(", ", allowedTypes)}"
                                        });
                                    }
                                }

                                if (columnType == typeof(long))
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

                response = await _IItemCategory.UpdateMultipleItemDataFromExcel(dt, flag);

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

