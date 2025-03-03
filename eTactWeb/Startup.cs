using eTactWeb.Data.BLL;
using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Http.Features;
using eTactWeb.Services.Interface;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;

namespace eTactWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

          

            loggerFactory.AddFile("Logs/eTactWeb-.log");
       
            app.UseHttpLogging();
            app.UseSession();
            app.UseOutputCache();
           
            app.UseStaticFiles();
             
            app.UseFastReport();

         
            app.Use(async (context, next) =>
            {
                await next.Invoke();
            });

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages();

          

            app.UseEndpoints
            (
                endpoints =>
                {
                    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Login}/{id?}");
                    endpoints.MapRazorPages();
                }
            );
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddLogging();
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddOutputCache();
            FastReport.Utils.RegisteredObjects.AddConnection(typeof(FastReport.Data.MsSqlDataConnection));
            services.AddControllersWithViews();

            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); });

            services.AddMvc(options =>
            {
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit=int.MaxValue;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/Home/Login/";
                options.LoginPath = "/Home/Login/";
                options.LogoutPath = "/Home/Login/";
                options.ReturnUrlParameter = "/Home/Login/";
            });

           
            services.AddScoped<DirectPurchaseBillBLL>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<EncryptDecrypt, EncryptDecrypt>();
            services.TryAddSingleton<LoggerInfo, LoggerInfo>();
            services.TryAddScoped<ITaxModule, TaxModuleBLL>();
            services.TryAddScoped<ITDSModule, TDSModuleBLL>();
            services.TryAddScoped<IDataLogic, DataLogicBLL>();
            services.TryAddScoped<IItemMaster, ItemMasterBLL>();
            services.TryAddScoped<IHRSalaryHeadMaster, HRSalaryHeadMasterBLL>();
            services.TryAddScoped<IHRWeekOffMaster, HRWeekOffMasterBLL>();
            services.TryAddScoped<IHRLeaveMaster, HRLeaveMasterBLL>();
            services.TryAddScoped<IHRPFESIMaster, HRPFESIMasterBLL>();
            services.TryAddScoped<IItemCategory,ItemCategoryBLL>();
            services.TryAddScoped<IItemGroup, ItemGroupBLL>();
            services.TryAddScoped<ILedgerOpeningEntry, LedgerOpeningEntryBLL>();
            services.TryAddScoped<ISubVoucher, SubVoucherBLL>();
            services.TryAddScoped<IPrimaryAccountGroupMaster, PrimaryAccountGroupMasterBLL>();
            services.TryAddScoped<IAlternateItemMaster, AlternateItemMasterBLL>();
            services.TryAddScoped<IBOMReport, BOMReportBLL>();
            services.TryAddScoped<IMinimumMaximaumLevel, MinimumMaximaumLevelBLL>();
            services.TryAddScoped<IUserRightReport, UserRightReportBLL>();
            services.TryAddScoped<IDayBook, DayBookBLL>();
            services.TryAddScoped<IStockValuation, StockValuationBLL>();
            services.TryAddScoped<ICancelSaleBillrequisition, CancelSaleBillrequisitionBLL>();
            services.TryAddScoped<IStoreMaster, StoreMasterBLL>();
            services.TryAddScoped<ICurrencyMaster, CurrencyMasterBLL>();
            services.TryAddScoped<IDepartmentMaster, DepartmentMasterBLL>();
            services.TryAddScoped<IUnitMaster, UnitMasterBLL>();
            services.TryAddScoped<ICostCenterMaster, CostCenterMasterBLL>();
            services.TryAddScoped<ILedgerPartyWiseOpening, LedgerPartyWiseOpeningBLL>();
            services.TryAddScoped<IBankReceipt, BankReceiptBLL>();
            services.TryAddScoped<IProdPlanStatus, ProdPlanStatusBLL>();
            services.TryAddScoped<IHRShiftMaster, HRShiftMasterBLL>();
            services.TryAddScoped<IIssueAgainstProdSchedule, IssueAgainstProdScheduleBLL>();
            services.TryAddScoped<IPendingProductionSchedule, PendingProductionScheduleBLL>();
            services.TryAddScoped<IAdminModule, AdminModuleBLL>();
            services.TryAddScoped<IAccountMaster, AccountMasterBLL>();
            services.TryAddScoped<IBankMaster, BankMasterBLL>();
            services.TryAddScoped<IBomModule, BomModuleBLL>();
            services.TryAddScoped<ISaleOrder, SaleOrderBLL>();
            services.TryAddScoped<IProcessMaster, ProcessMasterBLL>();
            services.TryAddScoped<IEmployeeMaster, EmployeeMasterBLL>();
            services.TryAddScoped<IRetFromDepartmentMain, RetFromDepartmentMainBLL>();
            services.TryAddScoped<ICustomerJobWorkIssue, CustomerJobWorkIssueBLL>();
            services.TryAddScoped<ICloseProductionPlan, CloseProductionPlanBLL>();
            services.TryAddScoped<ITransactionLedger, TransactionLedgerBLL>();
            services.TryAddScoped<IOutStanding, OutStandingBLL>();
            services.TryAddScoped<IIndentRegister, IndentRegisterBLL>();


            services.TryAddTransient<ISaleSchedule, SaleScheduleBLL>();
            services.TryAddTransient<ITaxMaster, TaxMasterBLL>();
            services.TryAddTransient<IPurchaseOrder, PurchaseOrderBLL>();
            services.TryAddTransient<IPurchaseSchedule, PurchaseScheduleBLL>();
            services.TryAddTransient<IGateInward, GateInwardBLL>();
            services.TryAddTransient<IMaterialReceipt, MaterialReceiptBLL>();
            services.TryAddTransient<IMirModule, MirBLL>();
            services.TryAddTransient<IJobWorkIssue, JobWorkIssueBLL>();
            services.TryAddTransient<IJobWorkReceive, JobWorkReceiveBLL>();
            services.TryAddTransient<IReqWithoutBOM,ReqWithoutBomBLL>();
            services.TryAddTransient<IReqThruBom,ReqThruBomBLL>();
            services.TryAddTransient<IPendingReqToIssue,PendingReqToIssueBLL>();
            services.TryAddTransient<IIssueWithoutBom,IssueWithouBomBLL>();
            services.TryAddTransient<IPendingMRNToQC,PendingMRNtoQcBLL>();
            services.TryAddTransient<IRouting,RoutingBLL>();
            services.TryAddTransient<IWorkOrder,WorkOrderBLL>();
            services.TryAddTransient<IPOApproval,POApprovalBLL>();
            services.TryAddTransient<ISOApproval,SOApprovalBLL>();
            services.TryAddTransient<IPSApproval,PSApprovalBLL>();
            services.TryAddTransient<ISSApproval,SSApprovalBLL>();
            services.TryAddTransient<IStockAdjustment, StockAdjustmentBLL>();
            services.TryAddTransient<IIssueNRGP, IssueNrgpBLL>();
            services.TryAddTransient<IPendingMaterialToIssueThrBOM, PendingMaterialToIssueThrBOMBLL>();
            services.TryAddTransient<IIssueThrBOM, IssueThrBOMBLL>();
            services.TryAddTransient<IMRP, MRPBLL>();
            services.TryAddTransient<IIndent, IndentBLL>();
            services.TryAddTransient<IStockRegister, StockRegisterBLL>();
            services.TryAddTransient<ICustomerJWR, CustomerJWRBLL>();
            services.TryAddTransient<IInterStoreTransfer, InterStoreTransferBLL>();
            services.TryAddTransient<IReceiveChallan, ReceiveChallanBLL>();
            services.TryAddTransient<IWIPStockRegister, WIPStockRegisterBLL>();
            services.TryAddTransient<IProductionEntryReport, ProductionEntryReportBLL>();
            services.TryAddTransient<ITransferMaterialReport, TransferMaterialReportBLL>();
            services.TryAddTransient<IPORegister, PORegisterBLL>();
            services.TryAddTransient<IRCRegister, RCRegisterBLL>();
            services.TryAddTransient<IVendJWRegister, VendJWRegisterBLL>();
            services.TryAddTransient<IProductionSchedule, ProductionScheduleBLL>();
            services.TryAddTransient<IREQUISITIONRegister, REQUISITIONRegisterBLL>();
            services.TryAddTransient<IProductionEntry, ProductionEntryBLL>();
            services.TryAddTransient<IDirectPurchaseBill, DirectPurchaseBillBLL>();
            services.TryAddTransient<IJobWorkOpening, JobWorkOpeningBLL>();
            services.TryAddTransient<IPendingInProcessToQc, PendingInProcessToQcBLL>();
            services.TryAddTransient<IPendingToReceiveItem, PendingToReceiveItemBLL>();
            services.TryAddTransient<IInProcessQc, InProcessQcBLL>();
            services.TryAddTransient<IReceiveItem, ReceiveItemBLL>();
            services.TryAddTransient<ITransferFromWorkCenter, TransferFromWorkCenterBLL>();
            services.TryAddTransient<ISaleBill, SaleBillBLL>();
            services.TryAddTransient<IGateEntryRegister, GateEntryRegisterBLL>();
            services.TryAddTransient<ISaleBillRegister, SaleBillRegisterBLL>();
            services.TryAddTransient<IPartCodePartyWise, PartCodePartyWiseBLL>();
            services.TryAddScoped<IMemoryCacheService, MemoryCacheService>();
            services.TryAddTransient<IPurchaseBill, PurchaseBillBLL>();
            services.TryAddTransient<IPendingSaleRejection, PendingSaleRejectionBLL>();
            services.TryAddTransient<ISaleRejection, SaleRejectionBLL>();
            services.TryAddTransient<IWorkCenterMaster, WorkCenterMasterBLL>();
            services.TryAddTransient<IMachineMaster, MachineMasterBLL>();
            services.TryAddTransient<IMRNRegister, MRNRegisterBLL>();
            services.TryAddTransient<IMIRRegister, MIRRegisterBLL>();
            services.TryAddTransient<ICreditNote, CreditNoteBLL>();
            services.TryAddTransient<IBankReconciliation, BankReconciliationBLL>();
            services.TryAddTransient<IPurchaseRejection, PurchaseRejectionBLL>();

            services.TryAddTransient<IConnectionStringHelper, ConnectionStringHelper>();
            services.AddScoped<UserContextService>();
        }
    }
}