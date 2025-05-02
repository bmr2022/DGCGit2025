using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models;
using System.Data;

namespace eTactWeb.Controllers
{
    public class PendingRequisitionToIssueController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingReqToIssue _IPendReqToIssue;
        private readonly IIssueWithoutBom _IssueWithoutBom;
        private readonly ILogger<PendingRequisitionToIssueController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingRequisitionToIssueController(ILogger<PendingRequisitionToIssueController> logger, IDataLogic iDataLogic, IPendingReqToIssue IPendReqToIssue,IIssueWithoutBom IIssueWithoutBom,IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendReqToIssue = IPendReqToIssue;
            _IssueWithoutBom = IIssueWithoutBom;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
       
        public async Task<IActionResult> PendingRequisitionToIssue(string REQNo, string ItemName, string PartCode, string WorkCenter, string DashboardType, string FromDate, string ToDate, string GlobalSearch, string FromStore)
        {
           
            ViewData["Title"] = "Pending Requisition to Issue Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyPendingToIssue");
            var MainModel = new PendingRequisitionToIssue();
            var model = new IssueWithoutBomDetail();
            MainModel = await BindModel(MainModel);
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
           HttpContext.Session.SetString("KeyPendingToIssue", JsonConvert.SerializeObject(model));

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.REQNoBack = REQNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.WorkCenterBack = WorkCenter;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = GlobalSearch;
            MainModel.FromStoreBack = FromStore;

            return View(MainModel);
        }

        private async Task<PendingRequisitionToIssue> BindModel(PendingRequisitionToIssue model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            oDataSet = await _IPendReqToIssue.BindAllDropDowns("BINDDATA",YearCode).ConfigureAwait(true);
            model.ItemList = _List;
            model.ReqList = _List;
            model.PartCodeList= _List;
            model.WorkCenterList= _List;
            model.DeptList= _List;
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["REQNO"].ToString()
                    });
                }
                model.ReqList = _List;    
                
            }
            var oDataSet1 = new DataSet();
            oDataSet1 = await _IPendReqToIssue.BindAllDropDowns("BINDPARTCODE", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            if (oDataSet1.Tables.Count > 0 && oDataSet1.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet1.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.PartCodeList = _List;    
                
            }
            var oDataSet2 = new DataSet();

            oDataSet2 = await _IPendReqToIssue.BindAllDropDowns("BINDITEM", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            if (oDataSet2.Tables.Count > 0 && oDataSet2.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet2.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.ItemList = _List;    
                
            }
            var oDataSet3 = new DataSet();

            oDataSet3 = await _IPendReqToIssue.BindAllDropDowns("BINDDEPARTMENT", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            if (oDataSet3.Tables.Count > 0 && oDataSet3.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet3.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptList = _List;    
                
            }
            var oDataSet4 = new DataSet();

            oDataSet4 = await _IPendReqToIssue.BindAllDropDowns("BINDWORKCENTER", YearCode).ConfigureAwait(true);
            _List = new List<TextValue>();
            model.WorkCenterList = _List;

            if (oDataSet4.Tables.Count > 0 && oDataSet4.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet4.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["WorkCenterDescription"].ToString()
                    });
                }
                model.WorkCenterList = _List;    
                
            }
            return model;
        }

        public IActionResult AddissueWithoutBom(List<IssueWithoutBomDetail> model)
        {
            //if (model != null)
            //{
            //    foreach(var listItem in model)
            //    {
            //        int YC = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            //        var FinStartDate = ParseFormattedDate(HttpContext.Session.GetString("FromDate").Split(" ")[0]);
            //        //first get batchno's
            //        //listItem. = ParseFormattedDate(listItem.IssuedDate);
     
            //        listItem.IssuedDate = ParseFormattedDate(listItem.IssuedDate);
            //        //var Batchno = _IssueWithoutBom.FillBatchUnique(listItem.ItemCode, YC, "MAIN STORE", "", listItem.IssuedDate ?? DateTime.Today.ToString(), FinStartDate);
            //        var Batchno = _IssueWithoutBom.FillBatchUnique(listItem.ItemCode, YC, listItem.StoreName , "", listItem.IssuedDate ?? DateTime.Today.ToString(), FinStartDate);
            //        var frmStoreid = listItem.StoreId;
            //        if (Batchno.Result.Result != null)
            //        { 
            //            var i = 0;
            //            foreach (var batchList in Batchno.Result.Result.Rows)
            //            {
            //                var checkTransDate = _IPendReqToIssue.CheckTransDate(listItem.ItemCode, listItem.IssuedDate, Batchno.Result.Result.Rows[i].ItemArray[1], Batchno.Result.Result.Rows[i].ItemArray[2], YC, frmStoreid);
            //                if (checkTransDate.Result.Result.Tables[0].Rows[0].ItemArray[0] != "Successful")
            //                {
            //                    return Json(checkTransDate.Result.Result.Tables[0].Rows[0].ItemArray[0]);
            //                }
            //                i++;
            //            }
            //        }
            //    }
            //}
            try
            {
                HttpContext.Session.Remove("KeyPendingToIssue");
                string modelJson = HttpContext.Session.GetString("KeyPendingToIssue");
                IList<IssueWithoutBomDetail> IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<IList<IssueWithoutBomDetail>>(modelJson);
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IssueWithoutBomDetailGrid = JsonConvert.DeserializeObject<IList<IssueWithoutBomDetail>>(modelJson);
                }
                TempData.Clear();
                var MainModel = new IssueWithoutBom();
                var IssueWithoutBomGrid = new List<IssueWithoutBomDetail>();
                var IssueGrid = new List<IssueWithoutBomDetail>();
                var SSGrid = new List<IssueWithoutBomDetail>();
               
                var seqNo = 0;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                                if (IssueWithoutBomDetailGrid == null)
                                {
                                    item.seqno += seqNo + 1;
                                    IssueGrid.Add(item);
                                    seqNo++;
                                }
                                else
                                {
                                if (IssueWithoutBomDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    item.seqno = IssueWithoutBomDetailGrid.Count + 1;
                                            IssueGrid = IssueWithoutBomDetailGrid.Where(x => x != null).ToList();
                                            SSGrid.AddRange(IssueGrid);
                                            IssueGrid.Add(item);
                                    }
                                }
                            
                            MainModel.ItemDetailGrid = IssueGrid;

                            HttpContext.Session.SetString("KeyPendingToIssue", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                        }
                    }
                }
                string pendingToIssue = HttpContext.Session.GetString("KeyPendingToIssue");
                IList<IssueWithoutBomDetail> grid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(pendingToIssue);
                if(!string.IsNullOrEmpty(pendingToIssue))
                {
                    grid = JsonConvert.DeserializeObject<List<IssueWithoutBomDetail>>(pendingToIssue);
                }
                HttpContext.Session.SetString("KeyIssWOBom", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssWOBom", serializedGrid);


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> FillItemCode(string ReqNo,int WorkCenter, int DeptName, int YearCode, string ToDate)
        {
            var JSON = await _IPendReqToIssue.FillItemCode(ReqNo,WorkCenter,DeptName, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillWorkCenter(string ReqNo, int ItemCode, int DeptName, int YearCode, string ToDate)
        {
            var JSON = await _IPendReqToIssue.FillWorkCenter(ReqNo, ItemCode, DeptName, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDeptName(string ReqNo, int ItemCode, int WorkCenter, int YearCode, string ToDate)
        {
            var JSON = await _IPendReqToIssue.FillDeptName(ReqNo, ItemCode, WorkCenter, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRequisition(int ToDept, int ItemCode, int WorkCenter, int YearCode, string ToDate)
        {
            var JSON = await _IPendReqToIssue.FillRequisition(ToDept, ItemCode, WorkCenter, YearCode, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate,string GlobalSearch, string FromStore, int StoreId)
        {
            var JSON = await _IPendReqToIssue.ShowDetail(FromDate,ToDate,ReqNo,YearCode,ItemCode,WoNo,WorkCenter,DeptName, ReqYear, IssueDate, GlobalSearch, FromStore,StoreId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        //public async Task<IActionResult> BackMethod(string REQNo = "", string ItemName = "", string PartCode = "", string WorkCenter = "", string DashboardType = "", string FromDate = "", string ToDate = "", string GlobalSearch = "")
        //{
        //    var MainModel = new PendingRequisitionToIssue();
        //    MainModel.FromDateBack = FromDate;
        //    MainModel.ToDateBack = ToDate;
        //    MainModel.REQNoBack = REQNo;
        //    MainModel.PartCodeBack = PartCode;
        //    MainModel.ItemNameBack = ItemName;
        //    MainModel.WorkCenterBack = WorkCenter;
        //    MainModel.DashboardTypeBack = DashboardType;
        //    MainModel.GlobalSearchBack = GlobalSearch;

        //    return View(MainModel);

        //}
        public async Task<JsonResult> BindReqYear(int YearCode, string ToDate)
        {        
            var JSON = await _IPendReqToIssue.BindReqYear(YearCode,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> EnableOrDisableIssueDate()
        {        
            var JSON = await _IPendReqToIssue.EnableOrDisableIssueDate();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAlternateItemCode(int MainIC)
        {        
            var JSON = await _IPendReqToIssue.GetAlternateItemCode(MainIC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


    }
}