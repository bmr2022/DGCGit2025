using eTactWeb.Services.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data;

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
            if (model.ItemGroupList != null)
            {
                model.ItemGroupList = model.ItemGroupList.DistinctBy(x => x.Group_Code).ToList();
            }
            //model.ItemGroupList = model.ItemGroupList.DistinctBy(x => x.Group_Code).ToList();
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
        public async Task<IActionResult> ImportandUpdateItemGroup()
        {
            var model = new ItemGroupModel();
            return View(model);
        }
        public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        {
            var response = new ResponseResult();
            var flag = request.Flag;

            try
            {
                DataTable dt = new DataTable();

                // Define columns based on SQL table
                dt.Columns.Add("Group_Code", typeof(long));
                dt.Columns.Add("Group_name", typeof(string));
                dt.Columns.Add("Under_GroupCode", typeof(long));
                dt.Columns.Add("Entry_date", typeof(DateTime));
                dt.Columns.Add("GroupPrefix", typeof(string));
                dt.Columns.Add("UnderCategoryId", typeof(int));
                dt.Columns.Add("seqNo", typeof(long));
                dt.Columns.Add("ItemAssetsService", typeof(string));
                dt.Columns.Add("CategoryPrefix", typeof(string));
                dt.Columns.Add("ItemPrefix", typeof(string));

                int rowIndex = 1;

                foreach (var excelRow in request.ExcelData)
                {
                    DataRow row = dt.NewRow();

                    foreach (var map in request.Mapping)
                    {
                        string dbCol = map.Key;      // DB column name
                        string excelCol = map.Value; // Excel column name

                        object value = DBNull.Value;

                        if (excelRow.ContainsKey(excelCol) && !string.IsNullOrEmpty(excelRow[excelCol]))
                        {
                            value = excelRow[excelCol];
                            Type columnType = dt.Columns[dbCol].DataType;

                            try
                            {
                                if (dbCol == "UnderCategoryId")
                                {
                                    string ItemCat = value.ToString().Trim();
                                    int ItemType = 0;
                                    var CatCode = _IItemGroup.GetItemCatCode(ItemCat);

                                    if (CatCode.Result.Result != null && CatCode.Result.Result.Rows.Count > 0)
                                    {
                                        ItemType = (int)CatCode.Result.Result.Rows[0].ItemArray[0];
                                    }

                                    if (ItemType != 0)
                                        value = ItemType;
                                    else
                                        return Json(new
                                        {
                                            StatusCode = 201,
                                            StatusText = $"Please Enter valid UnderCategoryId at Row {rowIndex}"
                                        });
                                }
                                if (dbCol == "ItemAssetsService")
                                {
                                    string itemType = value?.ToString().Trim() ?? string.Empty;
                                    if (string.IsNullOrEmpty(itemType) ||
                                        !(itemType.Equals("Item", StringComparison.OrdinalIgnoreCase) ||
                                          itemType.Equals("Service", StringComparison.OrdinalIgnoreCase) ||
                                          itemType.Equals("Asset", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        return Json(new
                                        {
                                            StatusCode = 201,
                                            StatusText = $"Invalid Item/Service/Asset type at Row {rowIndex}"
                                        });
                                    }
                                }



                                if (columnType == typeof(int))
                                    value = int.Parse(value.ToString());
                                else if (columnType == typeof(decimal))
                                    value = decimal.Parse(value.ToString());
                                else if (columnType == typeof(bool))
                                {
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
                                value = DBNull.Value; // Conversion failed
                            }
                        }

                        row[dbCol] = value;
                    }

                    dt.Rows.Add(row);
                    rowIndex++;
                }

                // Pass to repository/service layer
                response = await _IItemGroup.UpdateMultipleItemDataFromExcel(dt, flag);

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
                            StatusCode = 201,
                            RedirectUrl = ""
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
