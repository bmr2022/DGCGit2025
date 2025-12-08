using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Principal;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MirModel;
using eTactWeb.DOM.Models;
using System.Data;

namespace eTactWeb.Controllers
{
    public class PendingMRNtoQcController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingMRNToQC _IPendMRNToQC;
        private readonly ILogger<PendingMRNtoQcController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingMRNtoQcController(ILogger<PendingMRNtoQcController> logger, IDataLogic iDataLogic, IPendingMRNToQC IPendMRNToQC, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendMRNToQC = IPendMRNToQC;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        public async Task<IActionResult> PendingMRNtoQc()
        {
            ViewData["Title"] = "Pending MRN to QC Details";
            ViewBag.YearCode = HttpContext.Session.GetString("YearCode");
            //TempData.Clear();
            //_MemoryCache.Remove("KeyPendingMRNToQC");
            HttpContext.Session.Remove("KeyPendingMRNToQC");
            var MainModel = new PendingMRNToQC();
            var model = new IssueWithoutBomDetail();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.UserType = HttpContext.Session.GetString("UserType");
            MainModel = await BindModel(MainModel);
            //MainModel.CC = HttpContext.Session.GetString("Branch");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            //_MemoryCache.Set("KeyPendingMRNToQC", model, cacheEntryOptions);
            string serializedGrid = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("KeyPendingMRNToQC", serializedGrid);


            return View(MainModel);
        }

        private async Task<PendingMRNToQC> BindModel(PendingMRNToQC model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IPendMRNToQC.BindData("BindData");
            model.AccountList = new List<TextValue>();
            model.InvNoList = new List<TextValue>();
            model.MRNNoList = new List<TextValue>();
            model.ItemNameList = new List<TextValue>();
            model.PartCodeList = new List<TextValue>();
            model.DeptList = new List<TextValue>();
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Account_Code"].ToString(),
                        Text = row["Account_Name"].ToString()
                    });
                }
                model.AccountList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[1].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["InvNo"].ToString(),
                        Text = row["InvNo"].ToString()
                    });
                }
                model.InvNoList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["MRNNo"].ToString(),
                        Text = row["MRNNo"].ToString()
                    });
                }
                model.MRNNoList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.PartCodeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.ItemNameList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryId"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptList = _List; 
            }
            return model;
        }

        public async Task<JsonResult> GetDataForPendingMRN(string Flag,string MRNJW, int YearCode, string FromDate, string ToDate, int AccountCode,string MrnNo,int ItemCode,string InvoiceNo,int DeptId)
        {
            var JSON = await _IPendMRNToQC.GetDataForPendingMRN(Flag,MRNJW, YearCode, FromDate, ToDate, AccountCode,MrnNo,ItemCode,InvoiceNo,DeptId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDeptForUser()
        {
            int Empid = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IPendMRNToQC.GetDeptForUser(Empid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddPendingMRNToQc(MIRFromPend model, string FromDate = "", string ToDate = "", string ItemName = "", string MRNNo = "", string VendorName = "",string INVNo="",string DeptName="",string PartCode="",string MRNJW="")
        {
            try
            {
                _MemoryCache.TryGetValue("KeyPendingMRNToQC", out IList<MIRFromPend> MirModel);
                //string modelJson = HttpContext.Session.GetString("KeyPendingMRNToQC");
                //List<MIRFromPend> MirModel = new List<MIRFromPend>();
                //if (!string.IsNullOrEmpty(modelJson))
                //{
                //    MirModel = JsonConvert.DeserializeObject<List<MIRFromPend>>(modelJson);
                //}

                var MainModel = new MirModel();
                var MIRGrid = new List<MIRFromPend>();
                var mirGrid = new MIRFromPend();
                var SSGrid = new List<MIRFromPend>();
                MemoryCacheEntryOptions cacheEntryOptions1 = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                if (model != null)
                {
                    if (MirModel == null)
                    {
                        MIRGrid.Add(model);
                    }
                    else
                    {
                        MIRGrid = MirModel.Where(x => x != null).ToList();
                            SSGrid.AddRange(MIRGrid);
                        MIRGrid.Add(model);
                    }

                    MainModel.MIRFromPendDetail = MIRGrid;
                    
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    //_MemoryCache.Set("KeyPendingMRNToQC", MainModel.MIRFromPendDetail, cacheEntryOptions);

                    string serializedGrid2 = JsonConvert.SerializeObject(MainModel.MIRFromPendDetail);
                    HttpContext.Session.SetString("KeyPendingMRNToQC", serializedGrid2);


                    if (MirModel == null)
    MirModel = new List<MIRFromPend>();

MirModel.Add(model);

// Serialize as LIST always
string serialized = JsonConvert.SerializeObject(MirModel);
HttpContext.Session.SetString("KeyPendingMRNToQC", serialized);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                //_MemoryCache.TryGetValue("KeyPendingMRNToQC", out IList<MIRFromPend> grid);
                string modelJson1 = HttpContext.Session.GetString("KeyMIRGrid");
                List<MIRFromPend> grid = new List<MIRFromPend>();
                if (!string.IsNullOrEmpty(modelJson1))
                {
                    grid = JsonConvert.DeserializeObject<List<MIRFromPend>>(modelJson1);
                }

                //_MemoryCache.Set("KeyMIRGridFromMRN", MainModel.MIRFromPendDetail, cacheEntryOptions1);

                string serializedGrid1 = JsonConvert.SerializeObject(MainModel.MIRFromPendDetail);
                HttpContext.Session.SetString("KeyMIRGridFromMRN", serializedGrid1);

                
                MainModel.FromDateBack = FromDate;
                MainModel.ToDateBack= ToDate;
                MainModel.VendorNameBack= VendorName;
                MainModel.InvNoBack = INVNo;
                MainModel.PartCodeBack= PartCode;
                MainModel.ItemNameBack = ItemName;
                MainModel.DeptNameBack= DeptName;
                MainModel.MRNJWBack= MRNJW;
                MainModel.MRNNoBack = MRNNo;
                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
