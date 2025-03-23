using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class MaterialConversionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMaterialConversion _IMaterialConversion { get; }

        private readonly ILogger<MaterialConversionController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MaterialConversionController(ILogger<MaterialConversionController> logger, IDataLogic iDataLogic, IMaterialConversion iMaterialConversion, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialConversion = iMaterialConversion;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> MaterialConversion()
        {
            var model = new MaterialConversionModel();
          
            model.OpeningYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
           
            model.ActualEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ApprovedBy = HttpContext.Session.GetString("EmpName");
            model.CC = HttpContext.Session.GetString("Branch");
            model.ActualEntryDate = DateTime.Now.ToString();
            return View(model);
        }
        public async Task<JsonResult> FillEntryID(int YearCode)
        {
            var JSON = await _IMaterialConversion.FillEntryID(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillBranch()
        {
            var JSON = await _IMaterialConversion.FillBranch();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IMaterialConversion.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName()
        {
            var JSON = await _IMaterialConversion.FillWorkCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> GetOriginalPartCode()
        {
            var JSON = await _IMaterialConversion.GetOriginalPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IMaterialConversion.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOriginalItemName()
         {
            var JSON = await _IMaterialConversion.GetOriginalItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
         }
        public IActionResult AddToGridData(MaterialConversionModel model)
        {
            try
            {
                    _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out IList<MaterialConversionModel> MaterialConversionGrid);

                    var MainModel = new MaterialConversionModel();
                    var WorkOrderPGrid = new List<MaterialConversionModel>();
                    var OrderGrid = new List<MaterialConversionModel>();
                    var ssGrid = new List<MaterialConversionModel>();

                    if (model != null)
                    {
                        if (MaterialConversionGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (MaterialConversionGrid.Any(x => (x.OriginalPartCode == model.OriginalPartCode)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SrNO = MaterialConversionGrid.Count + 1;
                                OrderGrid = MaterialConversionGrid.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.MaterialConversionGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyMaterialConversionGrid", MainModel.MaterialConversionGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                    }
                    return PartialView("_MaterialConversionGrid", MainModel);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SrNO, string Mode)
        {
            IList<MaterialConversionModel> MaterialConversionModelGrid = new List<MaterialConversionModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out MaterialConversionModelGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out MaterialConversionModelGrid);
            }
            IEnumerable<MaterialConversionModel> SSBreakdownGrid = MaterialConversionModelGrid;
            if (MaterialConversionModelGrid != null)
            {
                SSBreakdownGrid = MaterialConversionModelGrid.Where(x => x.SrNO == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }
       
        public IActionResult DeleteItemRow(int SrNO, string Mode)
        {
            var MainModel = new MaterialConversionModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out List<MaterialConversionModel> MaterialConversionGrid);
                int Indx = SrNO - 1;

                if (MaterialConversionGrid != null && MaterialConversionGrid.Count > 0)
                {
                    MaterialConversionGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in MaterialConversionGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.MaterialConversionGrid = MaterialConversionGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyMaterialConversionGrid", MainModel.MaterialConversionGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyMaterialConversionGrid", out List<MaterialConversionModel> MaterialConversionGrid);
                int Indx = SrNO;

                if (MaterialConversionGrid != null && MaterialConversionGrid.Count > 0)
                {
                    MaterialConversionGrid.RemoveAt(Indx);

                    Indx = 0;

                    foreach (var item in MaterialConversionGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.MaterialConversionGrid = MaterialConversionGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyMaterialConversionGrid", MainModel.MaterialConversionGrid, cacheEntryOptions);
                }
            }

            return PartialView("MaterialConversion", MainModel);
        }
    }
}
