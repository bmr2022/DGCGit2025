using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;

namespace eTactWeb.Controllers
{
    public class VendJWRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IVendJWRegister _IVendJWRegister { get; }

        private readonly ILogger<VendJWRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;
        public VendJWRegisterController(ILogger<VendJWRegisterController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IVendJWRegister VendJWRegister, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IVendJWRegister = VendJWRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public IActionResult VendJWRegister()
        {
            var model = new VendJWRegisterModel();
            model.VendJWRegisterDetails = new List<VendJWRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetJWRegisterData(string FromDate, string ToDate,string RecChallanNo,string IssChallanNo, string PartyName, string PartCode, string ItemName, string IssueChallanType, string ReportMode, int pageNumber = 1, int pageSize = 20, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new VendJWRegisterModel();
            model = await _IVendJWRegister.GetJWRegisterData(FromDate, ToDate,RecChallanNo,IssChallanNo, PartyName,PartCode, ItemName, IssueChallanType, ReportMode);
            model.ReportMode = ReportMode;
            model.IssueChallanType = IssueChallanType;
            var modelList = model?.VendJWRegisterDetails ?? new List<VendJWRegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.VendJWRegisterDetails = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<VendJWRegisterDetail> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.VendJWRegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyVendJWList", modelList, cacheEntryOptions);
            if (IssueChallanType == "ISSUE")
            {
                return PartialView("_VenderJWIssueChallanGrid", model);
            }
            else if(IssueChallanType == "REC")
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
            else
            {
                return PartialView("_VendorRecoIssueReceiveGrid", model);
            }
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString,string IssueChallanType,  string dashboardType = "JOBWORKReceiveSUMMARY", int pageNumber = 1, int pageSize = 20)
        {
            VendJWRegisterModel model = new VendJWRegisterModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                //return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
                if (IssueChallanType == "ISSUE")
                {
                    return PartialView("_VenderJWIssueChallanGrid", new List<VendJWRegisterModel>());
                }
                else if (IssueChallanType == "REC")
                {
                    return PartialView("_VenderJWReceiveGrid", new List<VendJWRegisterModel>());
                }
                else
                {
                    return PartialView("_VendorRecoIssueReceiveGrid",  new List<VendJWRegisterModel>());
                }
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyVendJWList", out IList<VendJWRegisterDetail> vendJWRegisterDetail) || vendJWRegisterDetail == null)
            {
                if (IssueChallanType == "ISSUE")
                {
                    return PartialView("_VenderJWIssueChallanGrid", new List<VendJWRegisterModel>());
                }
                else if (IssueChallanType == "REC")
                {
                    return PartialView("_VenderJWReceiveGrid", new List<VendJWRegisterModel>());
                }
                else
                {
                    return PartialView("_VendorRecoIssueReceiveGrid", new List<VendJWRegisterModel>());
                }
            }

            List<VendJWRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = vendJWRegisterDetail.ToList();
            }
            else
            {
                filteredResults = vendJWRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = vendJWRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.VendJWRegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            //receive
            if (model.ReportMode == "JOBWORKReceiveSUMMARY")
            {
                return PartialView("_JWReceiveSummaryReport", model);
            }
            else if (model.ReportMode == "JOBWORKReceiveWithAdjustemntDETAIL")
            {
                return PartialView("_JWReceiveAdjustmentDetailReport", model);
            }
            else if (model.ReportMode == "JOBWORKReceiveItemWiseSUMM")
            {
                return PartialView("_JWReceiveItemWiseSumReport", model);
            } 
            else if (model.ReportMode == "JOBWORKReceiveDETAIL")
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
            //reco
            else if (model.ReportMode == "JOBWORKRecoDETAIL")
            {
                return PartialView("_JWRecoIssueReceiveSummaryReport", model);
            }
            else if (model.ReportMode == "JOBWORKRecoSummary")
            {
                return PartialView("_VendorRecoIssueReceiveGrid", model);
            }
            //issue
            else if (model.ReportMode == "JOBWORKISSUECHALLANITEMDETAIL")
            {
                return PartialView("_JWIssueChallanDetalGridReport", model);
            }
            else if (model.ReportMode == "JOBWORKISSUEITEMDETAIL")
            {
                return PartialView("_JWIssueItemDetailReport", model);
            }
            else if (model.ReportMode == "JOBWORKISSUEPatyITEMDETAIL")
            {
                return PartialView("_JWIssuePartyItemDetailReport", model);
            }
             else if (model.ReportMode == "JOBWORKISSUECHALLANSUMMARY")
            {
                return PartialView("_VenderJWIssueChallanGrid", model);
            }

            else
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
        }
    }
}
