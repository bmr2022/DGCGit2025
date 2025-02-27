using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using System.Data;

namespace eTactwebMasters.Controllers
{
    [Authorize]
    public class HRLeaveMasterController : Controller
    {
          private readonly EncryptDecrypt _EncryptDecrypt;
            private readonly IDataLogic _IDataLogic;
            private readonly IHRLeaveMaster _IHRLeaveMaster;
            private readonly IWebHostEnvironment _IWebHostEnvironment;
            private readonly IMemoryCache _MemoryCache;
            private readonly ILogger<HRLeaveMasterController> _logger;



            public HRLeaveMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IHRLeaveMaster iHRLeaveMaster, IMemoryCache iMemoryCache, ILogger<HRLeaveMasterController> logger)
            {
                _IDataLogic = iDataLogic;
                _IHRLeaveMaster = iHRLeaveMaster;
                _IWebHostEnvironment = iWebHostEnvironment;
                _EncryptDecrypt = encryptDecrypt;
                _MemoryCache = iMemoryCache;
                _logger = logger;
            }

        public async Task<ActionResult> HRLeaveMaster(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRLeaveMasterModel();
            
            MainModel.LeaveId = ID;
            MainModel.Mode = Mode;
            ////MainModel.SalHeadEntryDate = SalHeadEntryDate;
            //if (Mode != "U")
            //{
            //    MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));



            //}
            //if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            //{

            //    //Retrieve the old data by AccountCode and populate the model with existing values
            //    MainModel = await _ISalaryHeadMaster.GetViewByID(ID).ConfigureAwait(false);
            //    MainModel.Mode = Mode; // Set Mode to Update
            //    MainModel.SalHeadEntryId = ID;


            //    if (Mode == "U")
            //    {
            //        MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            //        MainModel.LastUpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");

            //    }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };
            //    _MemoryCache.Set("HRSalaryDashboard", MainModel.HRSalaryDashboard, cacheEntryOptions);
            //}

            //// If not in "Update" mode, bind new model data
            //else
            //{
            //    // MainModel = await BindModels(MainModel);
            //}



            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            MainModel = await BindModel1(MainModel).ConfigureAwait(false);
            MainModel = await BindModel2(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        private async Task<HRLeaveMasterModel> BindModel(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetEmployeeCategory().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["CategoryId"].ToString(),
                        Text = row["EmpCateg"].ToString()
                    });
                }
                model.EmpCategList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        private async Task<HRLeaveMasterModel> BindModel1(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetDepartment().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["DeptId"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptWiseCategList = _List;
                _List = new List<TextValue>();




            }


            return model;
        }

        private async Task<HRLeaveMasterModel> BindModel2(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetLocation().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["emp_id"].ToString(),
                        Text = row["EmployeeName"].ToString()
                    });
                }
                model.LocationList = _List;
                _List = new List<TextValue>();




            }


            return model;
        }

        public async Task<JsonResult> GetleaveType()
        {
            var JSON = await _IHRLeaveMaster.GetleaveType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLeaveCategory()
        {
            var JSON = await _IHRLeaveMaster.GetLeaveCategory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillLeaveId()
        {
            var JSON = await _IHRLeaveMaster.FillLeaveId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }






    }
}
