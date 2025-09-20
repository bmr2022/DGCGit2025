using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class Features_OptionsDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public Features_OptionsDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@Flag", "DASHBAORD")
                };

                responseResult = await _IDataLogic.ExecuteDataSet("Sp_FeaturesOptions", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<Features_OptionsModel> GetDashboardDetailData(string Type)
        {
            DataSet? oDataSet = new DataSet();
            var model = new Features_OptionsModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("Sp_FeaturesOptions", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    //oCmd.Parameters.AddWithValue("@Flag", "DASHBAORD");
                    if (Type == "ItemDetail")
                    {

                        oCmd.Parameters.AddWithValue("@flag", "ITEMDashbaord");
                    }
                    if (Type == "PurchaseDetail")
                    {

                        oCmd.Parameters.AddWithValue("@flag", "PurchaseDashbaord");
                    }
                    if (Type == "SaleOrderDetail")
                    {

                        oCmd.Parameters.AddWithValue("@flag", "SaleOrderDashbaord");
                    }
                     if (Type == "GateEntryDetail")
                     {

                        oCmd.Parameters.AddWithValue("@flag", "GateEntryDashbaord");
                     }
                      if (Type == "MRNDetail")
                      {
                        oCmd.Parameters.AddWithValue("@flag", "MRNDashbaord");
                      }
                       if (Type == "CommonDetail")
                       {
                        oCmd.Parameters.AddWithValue("@flag", "CommonDashbaord");
                       }
                        if (Type == "RequisitionDetail")
                        {
                        oCmd.Parameters.AddWithValue("@flag", "RequisitionDashbaord");
                        }
                        if (Type == "VendJwIssRecDetail")
                        {
                          oCmd.Parameters.AddWithValue("@flag", "VendJwIssRecDashbaord");
                        }
                        if (Type == "StockAdjDetail")
                        {
                          oCmd.Parameters.AddWithValue("@flag", "StockAdjDashbaord");
                        }
                         if (Type == "IssWithBomDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "IssWithBomDashbaord");
                         }
                         if (Type == "IssRecChallanDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "IssChallanDashbaord");
                         }
                         if (Type == "JustJobWorkDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "CustJobWorkDashbaord");
                         } 
                         if (Type == "PrintReportDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "PrintReportDashbaord");
                         }
                         if (Type == "ProdEntryDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "ProdEntryDashbaord");
                         }
                         if (Type == "ProdPlanSchDetail")
                         {
                            oCmd.Parameters.AddWithValue("@flag", "ProdPlanSchDashbaord");
                         }
                         if (Type == "TransMtrFromWcRec")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "TransMtrFromWcRecDashbaord");
                         }
                         if (Type == "SaleBillDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "SaleBillDashbaord");
                         }
                         if (Type == "AccountDetail")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "AccountDashbaord");
                         }
                          if (Type == "PurchaseBill")
                         {
                          oCmd.Parameters.AddWithValue("@flag", "PurchaseBillDashbaord");
                         }
                        

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    if (Type == "ItemDetail")
                    {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {

                                                                AutoGen_PartCode = dr["AutoGen_PartCode"] != DBNull.Value ? dr["AutoGen_PartCode"].ToString() : string.Empty,
                                                                DuplicateItemName = dr["DuplicateItemName"] != DBNull.Value ? dr["DuplicateItemName"].ToString() : string.Empty,
                                                                ItemPartcodeGenerationFormat = dr["ItemPartcodeGenerationFormat"] != DBNull.Value ? dr["ItemPartcodeGenerationFormat"].ToString() : string.Empty,
                                                               
                                                            }).ToList();
                    }
                    if (Type == "PurchaseDetail")
                    {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                Po_File_Name = dr["Po_File_Name"] != DBNull.Value ? dr["Po_File_Name"].ToString() : string.Empty,
                                                                PONOEditable = dr["PONOEditable"] != DBNull.Value ? dr["PONOEditable"].ToString() : string.Empty,
                                                                PONOYearlyRenew = dr["PONOYearlyRenew"] != DBNull.Value ? dr["PONOYearlyRenew"].ToString() : string.Empty,
                                                                AutoGeneratedPUrchaseOrder = dr["AutoGeneratedPUrchaseOrder"] != DBNull.Value ? dr["AutoGeneratedPUrchaseOrder"].ToString() : string.Empty,
                                                                PurchaseOrderReport = dr["PurchaseOrderReport"] != DBNull.Value ? dr["PurchaseOrderReport"].ToString() : string.Empty,
                                                                PurchaseorderPrintReportName = dr["PurchaseorderPrintReportName"] != DBNull.Value ? dr["PurchaseorderPrintReportName"].ToString() : string.Empty,
                                                                PurchaseorderAmendPrintReportName = dr["PurchaseorderAmendPrintReportName"] != DBNull.Value ? dr["PurchaseorderAmendPrintReportName"].ToString() : string.Empty,
                                                                PurchasSchedulePrintReportName = dr["PurchasSchedulePrintReportName"] != DBNull.Value ? dr["PurchasSchedulePrintReportName"].ToString() : string.Empty,
                                                                PurchasScheduleAmendPrintReportName = dr["PurchasScheduleAmendPrintReportName"] != DBNull.Value ? dr["PurchasScheduleAmendPrintReportName"].ToString() : string.Empty,
                                                                AllowBackDatePURCHASEORDER = dr["AllowBackDatePURCHASEORDER"] != DBNull.Value ? dr["AllowBackDatePURCHASEORDER"].ToString() : string.Empty,
                                                                AllowBackDateINDENT = dr["AllowBackDateINDENT"] != DBNull.Value ? dr["AllowBackDateINDENT"].ToString() : string.Empty,
                                                                CheckPOPendFromPOonlyNotFromAmendment = dr["CheckPOPendFromPOonlyNotFromAmendment"] != DBNull.Value ? dr["CheckPOPendFromPOonlyNotFromAmendment"].ToString() : string.Empty,
                                                                PoallowtoprintWithoutApproval = dr["PoallowtoprintWithoutApproval"] != DBNull.Value ? dr["PoallowtoprintWithoutApproval"].ToString() : string.Empty,
                                                                POClosePOAlwaysAgainstIndent = dr["POClosePOAlwaysAgainstIndent"] != DBNull.Value ? dr["POClosePOAlwaysAgainstIndent"].ToString() : string.Empty,
                                                                IndentReportName = dr["IndentReportName"] != DBNull.Value ? dr["IndentReportName"].ToString() : string.Empty,
                                                                PurchaseIncrementPercentageofRMInMRP = dr["PurchaseIncrementPercentageofRMInMRP"] != DBNull.Value? Convert.ToDecimal(dr["PurchaseIncrementPercentageofRMInMRP"]): 0


                                                            }).ToList();
                    } 
                    if (Type == "PurchaseBill")
                    {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AccAllowtochangeDocumentinPurchaseBill = dr["AccAllowtochangeDocumnetinPurchaseBill"] != DBNull.Value ? dr["AccAllowtochangeDocumnetinPurchaseBill"].ToString() : string.Empty,
                                                                AccPasswordToChangeDocumentinPurchaseBill = dr["AccPasswordToChangeDocumentinPurchaseBill"] != DBNull.Value ? dr["AccPasswordToChangeDocumentinPurchaseBill"].ToString() : string.Empty,
                                                                AccAllowtochangeInvoiceNoDateinPurchaseBill = dr["AccAllowtochangeInvoiceNoDateinPurchaseBill"] != DBNull.Value ? dr["AccAllowtochangeInvoiceNoDateinPurchaseBill"].ToString() : string.Empty,
                                                                AccAllowToChangeVoucherDateInPurchBill = dr["AccAllowTochangeVoucherDateInPurchBill"] != DBNull.Value ? dr["AccAllowTochangeVoucherDateInPurchBill"].ToString() : string.Empty,
                                                                AccPurchaseVoucherPrintoutFilename = dr["AccPurchaseVoucherPrintoutfilename"] != DBNull.Value ? dr["AccPurchaseVoucherPrintoutfilename"].ToString() : string.Empty,
                                                                AccPurchaseBillInvoicePrintoutFilename = dr["AccPurchaseBillInvoicePrintoutfilename"] != DBNull.Value ? dr["AccPurchaseBillInvoicePrintoutfilename"].ToString() : string.Empty,

                                                                //AccAllowtochangeDocumnetinPurchaseBill = dr["AccAllowtochangeDocumnetinPurchaseBill"] != DBNull.Value ? dr["AccAllowtochangeDocumnetinPurchaseBill"].ToString() : string.Empty
                                                            }).ToList();
                    }

                    if (Type == "SaleOrderDetail")
                    {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                SaleInvoiceFileName = dr["SaleInvoiceFileName"] != DBNull.Value ? dr["SaleInvoiceFileName"].ToString() : string.Empty,
                                                                AutoGenItemGroupCode = dr["AutoGenItemGroupCode"] != DBNull.Value ? dr["AutoGenItemGroupCode"].ToString() : string.Empty,
                                                                AllowMultipleBuyerInSaleOrder = dr["AllowMultipleBuyerInSaleOrder"] != DBNull.Value ? dr["AllowMultipleBuyerInSaleOrder"].ToString() : string.Empty,
                                                                SaleorderPrintReportName = dr["SaleorderPrintReportName"] != DBNull.Value ? dr["SaleorderPrintReportName"].ToString() : string.Empty,
                                                                SaleAmendPrintReportName = dr["SaleAmendPrintReportName"] != DBNull.Value ? dr["SaleAmendPrintReportName"].ToString() : string.Empty,
                                                                SaleSchedulePrintReportName = dr["SaleSchedulePrintReportName"] != DBNull.Value ? dr["SaleSchedulePrintReportName"].ToString() : string.Empty,
                                                                SaleScheduleAmendPrintReportName = dr["SaleScheduleAmendPrintReportName"] != DBNull.Value ? dr["SaleScheduleAmendPrintReportName"].ToString() : string.Empty,

                                                            }).ToList();
                    }
                     if (Type == "GateEntryDetail")
                     {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                BlockGateEntry4UnAppPOAmm = dr["BlockGateEntry4UnAppPOAmm"] != DBNull.Value ? dr["BlockGateEntry4UnAppPOAmm"].ToString() : string.Empty,
                                                                AllowBackDateGAteEntry = dr["AllowBackDateGAteEntry"] != DBNull.Value ? dr["AllowBackDateGAteEntry"].ToString() : string.Empty,
                                                                ShowRateINGAteMrn = dr["ShowRateINGAteMrn"] != DBNull.Value ? dr["ShowRateINGAteMrn"].ToString() : string.Empty,
                                                                GateEntryPrintReportName = dr["GateEntryPrintReportName"] != DBNull.Value ? dr["GateEntryPrintReportName"].ToString() : string.Empty,
                                                                AllowGateRateEnabled = dr["AllowGateRateEnabled"] != DBNull.Value ? dr["AllowGateRateEnabled"].ToString() : string.Empty,


                                                            }).ToList();
                     }
                     if (Type == "MRNDetail")
                     {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackDateMRNEntry = dr["AllowBackDateMRNEntry"] != DBNull.Value ? dr["AllowBackDateMRNEntry"].ToString() : string.Empty,
                                                                AllowToChangeStoreInMRN = dr["AllowToChangeStoreInMRN"] != DBNull.Value ? dr["AllowToChangeStoreInMRN"].ToString() : string.Empty,
                                                                MRNPrintReportName = dr["MRNPrintReportName"] != DBNull.Value ? dr["MRNPrintReportName"].ToString() : string.Empty,
                                                                AllowBackDateMRIR = dr["AllowBackDateMRIR"] != DBNull.Value ? dr["AllowBackDateMRIR"].ToString() : string.Empty,

                                                            }).ToList();
                     }
                     if (Type == "CommonDetail")
                     {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                CCMandatoryInQuery = dr["CCMandatoryInQuery"] != DBNull.Value ? dr["CCMandatoryInQuery"].ToString() : string.Empty,
                                                                BatchWiseInventory = dr["BatchWiseInventory"] != DBNull.Value ? dr["BatchWiseInventory"].ToString() : string.Empty,
                                                                AllowToChangeBranch = dr["AllowToChangeBranch"] != DBNull.Value ? dr["AllowToChangeBranch"].ToString() : string.Empty,
                                                                AllowBAtchEditable = dr["AllowBAtchEditable"] != DBNull.Value ? dr["AllowBAtchEditable"].ToString() : string.Empty,
                                                                AllowBAtchEditablePAssword = dr["AllowBAtchEditablePAssword"] != DBNull.Value ? dr["AllowBAtchEditablePAssword"].ToString() : string.Empty,
                                                               // FIFOBasedBatchInventory = dr["FIFOBasedBatchInventory"] != DBNull.Value ? dr["FIFOBasedBatchInventory"].ToString() : string.Empty,
                                                                //FIFOBasedBatchInventoryInJobWorkIss = dr["FIFOBasedBatchInventoryInJobWorkIss"] != DBNull.Value ? dr["FIFOBasedBatchInventoryInJobWorkIss"].ToString() : string.Empty,


                                                            }).ToList();
                     }
                      if (Type == "RequisitionDetail")
                      {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackdateReqWOBOM = dr["AllowBackdateReqWOBOM"] != DBNull.Value ? dr["AllowBackdateReqWOBOM"].ToString() : string.Empty,
                                                                MaxDurationForReqToBePend = model.MaxDurationForReqToBePend = dr["MaxDurationForReqToBePend"] != DBNull.Value ? Convert.ToInt64(dr["MaxDurationForReqToBePend"]) : 0,
                                                                IssueWithScanner = dr["IssueWithScanner"] != DBNull.Value ? dr["IssueWithScanner"].ToString() : string.Empty,
                                                                AllowBackDateRequsitionWITHBOM = dr["AllowBackDateRequsitionWITHBOM"] != DBNull.Value ? dr["AllowBackDateRequsitionWITHBOM"].ToString() : string.Empty,
                                                              
                                                            }).ToList();
                      }
                       if (Type == "VendJwIssRecDetail")
                       {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                VendJWIssuePrintReportName = dr["VendJWIssuePrintReportName"] != DBNull.Value ? dr["VendJWIssuePrintReportName"].ToString() : string.Empty,
                                                                VendJWRecPrintReportName = dr["VendJWRecPrintReportName"] != DBNull.Value ? dr["VendJWRecPrintReportName"].ToString() : string.Empty,
                                                                AllowBackDateJOBWorkIssue = dr["AllowBackDateJOBWorkIssue"] != DBNull.Value ? dr["AllowBackDateJOBWorkIssue"].ToString() : string.Empty,
                                                                ALLOWBACKDATEJobworkRec = dr["ALLOWBACKDATEJobworkRec"] != DBNull.Value ? dr["ALLOWBACKDATEJobworkRec"].ToString() : string.Empty,
                                                                VendorJWAdjustAutoOrManual = dr["vendorJWAdjustAutoOrManual"] != DBNull.Value ? dr["vendorJWAdjustAutoOrManual"].ToString() : string.Empty,
                                                               // FIFOBasedBatchInventoryInJobWorkIss = dr["FIFOBasedBatchInventoryInJobWorkIss"] != DBNull.Value ? dr["FIFOBasedBatchInventoryInJobWorkIss"].ToString() : string.Empty,

                                                            }).ToList();
                       }
                       if (Type == "StockAdjDetail")
                       {
                        model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackDateStockAdjustment = dr["AllowBackDateStockAdjustment"] != DBNull.Value ? dr["AllowBackDateStockAdjustment"].ToString() : string.Empty,
                                                                AllowBackDateInterStoreTransfer = dr["AllowBackDateInterStoreTransfer"] != DBNull.Value ? dr["AllowBackDateInterStoreTransfer"].ToString() : string.Empty,

                                                            }).ToList();
                       }
                        if (Type == "IssWithBomDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackdateIssueWOBOM = dr["AllowBackdateIssueWOBOM"] != DBNull.Value ? dr["AllowBackdateIssueWOBOM"].ToString() : string.Empty,
                                                                AllowBackDateISSUEWITHBOM = dr["AllowBackDateISSUEWITHBOM"] != DBNull.Value ? dr["AllowBackDateISSUEWITHBOM"].ToString() : string.Empty,
                                                                IssueViaScanningBarcode = dr["IssueViaScanningBarcode"] != DBNull.Value ? dr["IssueViaScanningBarcode"].ToString() : string.Empty,
                                                                AllowBackDateIssueChallan = dr["AllowBackDateIssueChallan"] != DBNull.Value ? dr["AllowBackDateIssueChallan"].ToString() : string.Empty,
                                                                ALLOWBACKDATRECCHALLAN = dr["ALLOWBACKDATRECCHALLAN"] != DBNull.Value ? dr["ALLOWBACKDATRECCHALLAN"].ToString() : string.Empty,


                                                            }).ToList();
                        }
                         if (Type == "IssRecChallanDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                RGPChallanPrintReportName = dr["RGPChallanPrintReportName"] != DBNull.Value ? dr["RGPChallanPrintReportName"].ToString() : string.Empty,
                                                                NRGPChallanPrintReportName = dr["NRGPChallanPrintReportName"] != DBNull.Value ? dr["NRGPChallanPrintReportName"].ToString() : string.Empty,
                                                                AllowBackDateReceiveChallanEntry = dr["AllowBackDateReceiveChallanEntry"] != DBNull.Value ? dr["AllowBackDateReceiveChallanEntry"].ToString() : string.Empty,
                                                                IssueChaallanTaxIsMandatory = dr["IssueChaallanTaxIsMandatory"] != DBNull.Value ? dr["IssueChaallanTaxIsMandatory"].ToString() : string.Empty,


                                                            }).ToList();
                        }
                        if (Type == "JustJobWorkDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                CustJWIssuePrintReportName = dr["CustJWIssuePrintReportName"] != DBNull.Value ? dr["CustJWIssuePrintReportName"].ToString() : string.Empty,
                                                                CUSTJWRecPrintReportName = dr["CUSTJWRecPrintReportName"] != DBNull.Value ? dr["CUSTJWRecPrintReportName"].ToString() : string.Empty,
                                                                AllowBackDateCustomerJWIssue = dr["AllowBackDateCustomerJWIssue"] != DBNull.Value ? dr["AllowBackDateCustomerJWIssue"].ToString() : string.Empty,


                                                            }).ToList();
                        }
                        if (Type == "PrintReportDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                SaleBillPrintReportName = dr["SaleBillPrintReportName"] != DBNull.Value ? dr["SaleBillPrintReportName"].ToString() : string.Empty,
                                                                JWSaleBillPrintReportName = dr["JWSaleBillPrintReportName"] != DBNull.Value ? dr["JWSaleBillPrintReportName"].ToString() : string.Empty,
                                                                PurchaseRejPrintReportName = dr["PurchaseRejPrintReportName"] != DBNull.Value ? dr["PurchaseRejPrintReportName"].ToString() : string.Empty,
                                                                DebitNotePrintReportName = dr["DebitNotePrintReportName"] != DBNull.Value ? dr["DebitNotePrintReportName"].ToString() : string.Empty,
                                                                CreditNotePrintReportName = dr["CreditNotePrintReportName"] != DBNull.Value ? dr["CreditNotePrintReportName"].ToString() : string.Empty,
                                                                SaleRejectionPrintReportName = dr["SaleRejectionPrintReportName"] != DBNull.Value ? dr["SaleRejectionPrintReportName"].ToString() : string.Empty,
                                                                PurchaseBillPrintReportName = dr["PurchaseBillPrintReportName"] != DBNull.Value ? dr["PurchaseBillPrintReportName"].ToString() : string.Empty,
                                                                DirectPurchaseBillPrintReportName = dr["DirectPurchaseBillPrintReportName"] != DBNull.Value ? dr["DirectPurchaseBillPrintReportName"].ToString() : string.Empty,
                                                                PurchBillVoucherPrintReportName = dr["PurchBillVoucherPrintReportName"] != DBNull.Value ? dr["PurchBillVoucherPrintReportName"].ToString() : string.Empty,
                                                                DirectPurchBillVoucherPrintReportName = dr["DirectPurchBillVoucherPrintReportName"] != DBNull.Value ? dr["DirectPurchBillVoucherPrintReportName"].ToString() : string.Empty,
                                                                BankRecVoucherPrintReportName = dr["BankRecVoucherPrintReportName"] != DBNull.Value ? dr["BankRecVoucherPrintReportName"].ToString() : string.Empty,
                                                                BankPaymentVoucherPrintReportName = dr["BankPaymentVoucherPrintReportName"] != DBNull.Value ? dr["BankPaymentVoucherPrintReportName"].ToString() : string.Empty,
                                                                CashRecVoucherPrintReportName = dr["CashRecVoucherPrintReportName"] != DBNull.Value ? dr["CashRecVoucherPrintReportName"].ToString() : string.Empty,
                                                                CashPaymentVoucherPrintReportName = dr["CashPaymentVoucherPrintReportName"] != DBNull.Value ? dr["CashPaymentVoucherPrintReportName"].ToString() : string.Empty,
                                                                JournalVoucherPrintReportName = dr["JournalVoucherPrintReportName"] != DBNull.Value ? dr["JournalVoucherPrintReportName"].ToString() : string.Empty,

                                                            }).ToList();
                        } 
                        if (Type == "ProdEntryDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                PrdProdEntryAgainstWOProdSchReqManual = dr["PrdProdEntryAgainstWOProdSchReqManual"] != DBNull.Value ? dr["PrdProdEntryAgainstWOProdSchReqManual"].ToString() : string.Empty,
                                                                AllowProdWithoutBom = dr["AllowProdWithoutBom"] != DBNull.Value ? dr["AllowProdWithoutBom"].ToString() : string.Empty,
                                                                ProdSchAllowToAddManualNewItem = dr["ProdSchAllowToAddManualNewItem"] != DBNull.Value ? dr["ProdSchAllowToAddManualNewItem"].ToString() : string.Empty,
                                                                ProdEntryAllowToAddRMItem = dr["ProdEntryAllowToAddRMItem"] != DBNull.Value ? dr["ProdEntryAllowToAddRMItem"].ToString() : string.Empty,
                                                                ProdEntryAllowToAddWithoutRM = dr["ProdEntryAllowToAddWithoutRM"] != DBNull.Value ? dr["ProdEntryAllowToAddWithoutRM"].ToString() : string.Empty,
                                                                ProdEntryAllowToAddNegativeStock = dr["ProdEntryAllowToAddNegativeStock"] != DBNull.Value ? dr["ProdEntryAllowToAddNegativeStock"].ToString() : string.Empty,
                                                                BatchWiseProduction = dr["BatchWiseProduction"] != DBNull.Value ? dr["BatchWiseProduction"].ToString() : string.Empty,
                                                                ProdEntryAllowBackDate = dr["ProdEntryAllowBackDate"] != DBNull.Value ? dr["ProdEntryAllowBackDate"].ToString() : string.Empty,
                                                                AllowBackDateDAILYPRODUCTION = dr["AllowBackDateDAILYPRODUCTION"] != DBNull.Value ? dr["AllowBackDateDAILYPRODUCTION"].ToString() : string.Empty,
                                                                //ProdAllowMultiplePlanInProdSchOrNot = dr["ProdAllowMultiplePlanInProdSchOrNot"] != DBNull.Value ? dr["ProdAllowMultiplePlanInProdSchOrNot"].ToString() : string.Empty,

                                                            }).ToList();
                        }
                        if (Type == "ProdPlanSchDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackDateWorkorderEntry = dr["AllowBackDateWorkorderEntry"] != DBNull.Value ? dr["AllowBackDateWorkorderEntry"].ToString() : string.Empty,
                                                                AllowBackDateProductionScheduleEntry = dr["AllowBackDateProductionScheduleEntry"] != DBNull.Value ? dr["AllowBackDateProductionScheduleEntry"].ToString() : string.Empty,
                                                                InProdScheduleShowSumOrDetail = dr["InProdScheduleShowSumOrDetail"] != DBNull.Value ? dr["InProdScheduleShowSumOrDetail"].ToString() : string.Empty,

                                                            }).ToList();
                        }

                        if (Type == "TransMtrFromWcRec")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                                AllowBackDateTRANSFERMATERIAL = dr["AllowBackDateTRANSFERMATERIAL"] != DBNull.Value ? dr["AllowBackDateTRANSFERMATERIAL"].ToString() : string.Empty,
                                                                AllowBackDateRECEIVEINSTORE = dr["AllowBackDateRECEIVEINSTORE"] != DBNull.Value ? dr["AllowBackDateRECEIVEINSTORE"].ToString() : string.Empty,
                                                               
                                                            }).ToList();
                        }
                        if (Type == "SaleBillDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
																SaleInvoiceFileName = dr["SaleInvoiceFileName"] != DBNull.Value ? dr["SaleInvoiceFileName"].ToString() : string.Empty,
																SaleBillPrintReportName = dr["SaleBillPrintReportName"] != DBNull.Value ? dr["SaleBillPrintReportName"].ToString() : string.Empty,
																AllowMultipleBuyerInSaleOrder = dr["AllowMultipleBuyerInSaleOrder"] != DBNull.Value ? dr["AllowMultipleBuyerInSaleOrder"].ToString() : string.Empty,
																JWSaleBillPrintReportName = dr["JWSaleBillPrintReportName"] != DBNull.Value ? dr["JWSaleBillPrintReportName"].ToString() : string.Empty,
																AllowBackDateSALEBILL = dr["AllowBackDateSALEBILL"] != DBNull.Value ? dr["AllowBackDateSALEBILL"].ToString() : string.Empty,
																salebillShowItemBatchFIFOBased = dr["salebillShowItemBatchFIFOBased"] != DBNull.Value ? dr["salebillShowItemBatchFIFOBased"].ToString() : string.Empty,
																AccSaleBillManualTaxAlloweed = dr["AccSaleBillManualTaxAlloweed"] != DBNull.Value ? dr["AccSaleBillManualTaxAlloweed"].ToString() : string.Empty,
																AccPasswordToAllowManualTax = dr["AccPasswordToAllowManualTax"] != DBNull.Value ? dr["AccPasswordToAllowManualTax"].ToString() : string.Empty,
																EditableRateAndDiscountONSaleInvoice = dr["EditableRateAndDiscountONSaleInvoice"] != DBNull.Value ? dr["EditableRateAndDiscountONSaleInvoice"].ToString() : string.Empty,
																ShowHideEntryDetail = dr["ShowHideSaleBillEntryDetail"] != DBNull.Value ? dr["ShowHideSaleBillEntryDetail"].ToString() : string.Empty,
																ShowHideCustomerDetail = dr["ShowHideSaleBillCustomerDetail"] != DBNull.Value ? dr["ShowHideSaleBillCustomerDetail"].ToString() : string.Empty,
																ShowHideOtherRequiredDetail = dr["ShowHideSaleBillOtherRequiredDetail"] != DBNull.Value ? dr["ShowHideSaleBillOtherRequiredDetail"].ToString() : string.Empty,
																ShowHideCurrency = dr["ShowHideSaleBillCurrency"] != DBNull.Value ? dr["ShowHideSaleBillCurrency"].ToString() : string.Empty,
																ShowHideConsignee = dr["ShowHideSaleBillConsignee"] != DBNull.Value ? dr["ShowHideSaleBillConsignee"].ToString() : string.Empty,
																ShowHideScheduleDetail = dr["ShowHideSaleBillScheduleDetail"] != DBNull.Value ? dr["ShowHideSaleBillScheduleDetail"].ToString() : string.Empty,
																AllowToChangeStoreName = dr["AllowToChangeSaleBillStoreName"] != DBNull.Value ? dr["AllowToChangeSaleBillStoreName"].ToString() : string.Empty,
																HideOtherFieldOFDetailTable = dr["HideOtherFieldOfSaleBillDetailTable"] != DBNull.Value ? dr["HideOtherFieldOfSaleBillDetailTable"].ToString() : string.Empty,
																ApproveSOForGenerateSaleInvoiceOrNot = dr["ApproveSOForGenerateSaleInvoiceOrNot"] != DBNull.Value ? dr["ApproveSOForGenerateSaleInvoiceOrNot"].ToString() : string.Empty,


															}).ToList();
                        }
                         if (Type == "AccountDetail")
                        {
                         model.features_OptionsModelsGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                            select new Features_OptionsModel
                                                            {
                                                               
                                                                AccPasswordToAllowManualTax = dr["AccPasswordToAllowManualTax"] != DBNull.Value ? dr["AccPasswordToAllowManualTax"].ToString() : string.Empty,
                                                                AccSaleBillManualTaxAllowed = dr["AccSaleBillManualTaxAlloweed"] != DBNull.Value ? dr["AccSaleBillManualTaxAlloweed"].ToString() : string.Empty,
                                                                AccAllowBackdateVoucherEntry = dr["AccAllowBackdateVoucherEntry"] != DBNull.Value ? dr["AccAllowBackdateVoucherEntry"].ToString() : string.Empty,
                                                                AccBackdateVoucherEntryPassword = dr["AccBackdateVoucherEntryPassword"] != DBNull.Value ? dr["AccBackdateVoucherEntryPassword"].ToString() : string.Empty,
                                                               
                                                            }).ToList();
                        }

                 }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }
        public async Task<Features_OptionsModel> GetViewByID(string Type)
        {
            var model = new Features_OptionsModel();
            try
            {
                var SqlParams = new List<dynamic>();

                if (Type == "ItemDetail")
                {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDItemMDashbaord"));
                }
                if (Type == "PurchaseDetail")
                {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDPurchaseDashbaord"));
                }
                if (Type == "PurchaseBill")
                {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDPurchaseBill"));
                }

                 if (Type == "SaleOrderDetail")
                 {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDSaleOrder"));
                 }
                 if (Type == "GateEntryDetail")
                 {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDGateEntry"));
                 }
                 if (Type == "MRNDetail")
                 {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDMRN"));
                 }
                 if (Type == "CommonDetail")
                 {

                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDCommon"));
                 }
                 if (Type == "RequisitionDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDRequisition"));
                 } 
                 if (Type == "VendJwIssRecDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDVendJwIssRec"));
                 } 

                 if (Type == "StockAdjDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDStockAdj"));
                 }
                 if (Type == "IssWithBomDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDIssWithBom"));
                 }
                 if (Type == "IssRecChallanDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDIssChallan"));
                 }
                 if (Type == "JustJobWorkDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDCustJobWork"));
                 }
                 if (Type == "PrintReportDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDPrintReport"));
                 }
                 if (Type == "ProdEntryDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDProdEntry"));
                 }
                 if (Type == "TransMtrFromWcRec")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDTransMtrFromWcRec"));
                 }
                 if (Type == "SaleBillDetail")
                 {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDSaleBill"));
                 }
                  if (Type == "ProdPlanSchDetail")
                  {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDProdPlanSch"));
                  }
                   if (Type == "AccountDetail")
                  {
                    SqlParams.Add(new SqlParameter("@flag", "VIEWBYIDAccount"));
                  }

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("Sp_FeaturesOptions", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model, Type);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }
        private static Features_OptionsModel PrepareView(DataSet DS, ref Features_OptionsModel? model,string Type)
        {
            try
            {
                var ItemList = new List<Features_OptionsModel>();
                DS.Tables[0].TableName = "SSMain";
                int cnt = 0;

                if (Type == "ItemDetail")
                { 
                    model.AutoGen_PartCode = DS.Tables[0].Rows[0]["AutoGen_PartCode"].ToString();
                    model.DuplicateItemName = DS.Tables[0].Rows[0]["DuplicateItemName"].ToString();
                    model.ItemPartcodeGenerationFormat = DS.Tables[0].Rows[0]["ItemPartcodeGenerationFormat"].ToString();
                }
                if (Type == "PurchaseBill")
                {
					model.PurchaseBillPrintReportName = DS.Tables[0].Rows[0]["PurchaseBillPrintReportName"].ToString();
					model.DirectPurchaseBillPrintReportName = DS.Tables[0].Rows[0]["DirectPurchaseBillPrintReportName"].ToString();
					model.PurchBillVoucherPrintReportName = DS.Tables[0].Rows[0]["PurchBillVoucherPrintReportName"].ToString();
					model.DirectPurchBillVoucherPrintReportName = DS.Tables[0].Rows[0]["DirectPurchBillVoucherPrintReportName"].ToString();
					model.AccAllowtochangeDocumnetinPurchaseBill = DS.Tables[0].Rows[0]["AccAllowtochangeDocumnetinPurchaseBill"].ToString();
					model.AccAllowTochangeVoucherDateInPurchBill = DS.Tables[0].Rows[0]["AccAllowTochangeVoucherDateInPurchBill"].ToString();
					model.AccAllowtochangeInvoiceNoDateinPurchaseBill = DS.Tables[0].Rows[0]["AccAllowtochangeInvoiceNoDateinPurchaseBill"].ToString();
					model.AccPurchaseVoucherPrintoutFileName = DS.Tables[0].Rows[0]["AccPurchaseVoucherPrintoutFileName"].ToString();
					model.AccPurchaseBillInvoicePrintoutFileName = DS.Tables[0].Rows[0]["AccPurchaseBillInvoicePrintoutFileName"].ToString();

				}

				if (Type == "SaleOrderDetail")
                {
                    model.AllowMultipleBuyerInSaleOrder = DS.Tables[0].Rows[0]["AllowMultipleBuyerInSaleOrder"].ToString();
                    model.SaleorderPrintReportName = DS.Tables[0].Rows[0]["SaleorderPrintReportName"].ToString();
                    model.SaleAmendPrintReportName = DS.Tables[0].Rows[0]["SaleAmendPrintReportName"].ToString();
                    model.SaleSchedulePrintReportName = DS.Tables[0].Rows[0]["SaleSchedulePrintReportName"].ToString();
                    model.SaleScheduleAmendPrintReportName = DS.Tables[0].Rows[0]["SaleScheduleAmendPrintReportName"].ToString();
                    model.ShowHideSaleOrderEntryDetail = DS.Tables[0].Rows[0]["ShowHideSaleOrderEntryDetail"].ToString();
                    model.ShowHideSaleOrderOtherRequiredDetail = DS.Tables[0].Rows[0]["ShowHideSaleOrderOtherRequiredDetail"].ToString();
                    model.ShowHideSaleOrderConsignee = DS.Tables[0].Rows[0]["ShowHideSaleOrderConsignee"].ToString();
                    model.HideOtherFieldOFSaleOrderDetailTable = DS.Tables[0].Rows[0]["HideOtherFieldOFSaleOrderDetailTable"].ToString();

                } 
                if (Type == "PurchaseDetail")
                {
					model.PurchaseorderPrintReportName = DS.Tables[0].Rows[0]["PurchaseorderPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchaseorderPrintReportName"].ToString() : string.Empty;
					model.PurchaseorderAmendPrintReportName = DS.Tables[0].Rows[0]["PurchaseorderAmendPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchaseorderAmendPrintReportName"].ToString() : string.Empty;
					model.PurchasSchedulePrintReportName = DS.Tables[0].Rows[0]["PurchasSchedulePrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchasSchedulePrintReportName"].ToString() : string.Empty;
					model.PurchasScheduleAmendPrintReportName = DS.Tables[0].Rows[0]["PurchasScheduleAmendPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchasScheduleAmendPrintReportName"].ToString() : string.Empty;
					model.IndentReportName = DS.Tables[0].Rows[0]["IndentReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["IndentReportName"].ToString() : string.Empty;
					model.PoallowtoprintWithoutApproval = DS.Tables[0].Rows[0]["PoallowtoprintWithoutApproval"] != DBNull.Value ? DS.Tables[0].Rows[0]["PoallowtoprintWithoutApproval"].ToString() : string.Empty;
					model.PurchaseIncrementPercentageofRMInMRP =
		DS.Tables[0].Rows[0]["PurchaseIncrementPercentageofRMInMRP"] != DBNull.Value
		? Convert.ToDecimal(DS.Tables[0].Rows[0]["PurchaseIncrementPercentageofRMInMRP"])
		: 0;

					model.PONotAllowedWithoutPartCodePartyWiseDefinition = DS.Tables[0].Rows[0]["PONotAllowedWithoutPartCodePartyWiseDefinition"] != DBNull.Value ? DS.Tables[0].Rows[0]["PONotAllowedWithoutPartCodePartyWiseDefinition"].ToString() : string.Empty;
					model.Allow_PO_Print = DS.Tables[0].Rows[0]["Allow_PO_Print"] != DBNull.Value ? DS.Tables[0].Rows[0]["Allow_PO_Print"].ToString() : string.Empty;
					model.Po_File_Name = DS.Tables[0].Rows[0]["Po_File_Name"] != DBNull.Value ? DS.Tables[0].Rows[0]["Po_File_Name"].ToString() : string.Empty;
					model.PONOEditable = DS.Tables[0].Rows[0]["PONOEditable"] != DBNull.Value ? DS.Tables[0].Rows[0]["PONOEditable"].ToString() : string.Empty;
					model.PONOYearlyRenew = DS.Tables[0].Rows[0]["PONOYearlyRenew"] != DBNull.Value ? DS.Tables[0].Rows[0]["PONOYearlyRenew"].ToString() : string.Empty;
					model.AutoGeneratedPUrchaseOrder = DS.Tables[0].Rows[0]["AutoGeneratedPUrchaseOrder"] != DBNull.Value ? DS.Tables[0].Rows[0]["AutoGeneratedPUrchaseOrder"].ToString() : string.Empty;
					model.CheckPOPendFromPOonlyNotFromAmendment = DS.Tables[0].Rows[0]["CheckPOPendFromPOonlyNotFromAmendment"] != DBNull.Value ? DS.Tables[0].Rows[0]["CheckPOPendFromPOonlyNotFromAmendment"].ToString() : string.Empty;
					model.POClosePOAlwaysAgainstIndent = DS.Tables[0].Rows[0]["POClosePOAlwaysAgainstIndent"] != DBNull.Value ? DS.Tables[0].Rows[0]["POClosePOAlwaysAgainstIndent"].ToString() : string.Empty;

				}
				if (Type == "GateEntryDetail")
                {
                    model.BlockGateEntry4UnAppPOAmm = DS.Tables[0].Rows[0]["BlockGateEntry4UnAppPOAmm"].ToString();
                    model.AllowBackDateGAteEntry = DS.Tables[0].Rows[0]["AllowBackDateGAteEntry"].ToString();
                    model.ShowRateINGAteMrn = DS.Tables[0].Rows[0]["ShowRateINGAteMrn"].ToString();
                    model.GateEntryPrintReportName = DS.Tables[0].Rows[0]["GateEntryPrintReportName"].ToString();
                    model.AllowGateRateEnabled = DS.Tables[0].Rows[0]["AllowGateRateEnabled"].ToString();

                }
                if (Type == "MRNDetail")
                {
                    model.AllowBackDateMRNEntry = DS.Tables[0].Rows[0]["AllowBackDateMRNEntry"].ToString();
                    model.AllowToChangeStoreInMRN = DS.Tables[0].Rows[0]["AllowToChangeStoreInMRN"].ToString();
                    model.MRNPrintReportName = DS.Tables[0].Rows[0]["MRNPrintReportName"].ToString();
                    model.AllowBackDateMRIR = DS.Tables[0].Rows[0]["AllowBackDateMRIR"].ToString();


                }
                if (Type == "CommonDetail")
                {
                    model.CCMandatoryInQuery = DS.Tables[0].Rows[0]["CCMandatoryInQuery"].ToString();
                    model.BatchWiseInventory = DS.Tables[0].Rows[0]["BatchWiseInventory"].ToString();
                    model.AllowToChangeBranch = DS.Tables[0].Rows[0]["AllowToChangeBranch"].ToString();
                    model.AllowBAtchEditable = DS.Tables[0].Rows[0]["AllowBAtchEditable"].ToString();
                    model.AllowBAtchEditablePAssword = DS.Tables[0].Rows[0]["AllowBAtchEditablePAssword"].ToString();
                    //model.FIFOBasedBatchInventory = DS.Tables[0].Rows[0]["FIFOBasedBatchInventory"].ToString();
                    model.FIFOBasedBatchInventoryInJobWorkIss = DS.Tables[0].Rows[0]["FIFOBasedBatchInventoryInJobWorkIss"].ToString();

                }
                if (Type == "RequisitionDetail")
                {
                    model.AllowBackdateReqWOBOM = DS.Tables[0].Rows[0]["AllowBackdateReqWOBOM"].ToString();
                    model.MaxDurationForReqToBePend = Convert.ToInt64(DS.Tables[0].Rows[0]["MaxDurationForReqToBePend"]);
                    model.IssueWithScanner = DS.Tables[0].Rows[0]["IssueWithScanner"].ToString();
                    model.AllowBackDateRequsitionWITHBOM = DS.Tables[0].Rows[0]["AllowBackDateRequsitionWITHBOM"].ToString();
                  
                }
                if (Type == "VendJwIssRecDetail")
                {
                    model.VendJWIssuePrintReportName = DS.Tables[0].Rows[0]["VendJWIssuePrintReportName"].ToString();
                    model.VendJWRecPrintReportName = DS.Tables[0].Rows[0]["VendJWRecPrintReportName"].ToString();
                    model.AllowBackDateJOBWorkIssue = DS.Tables[0].Rows[0]["AllowBackDateJOBWorkIssue"].ToString();
                    model.ALLOWBACKDATEJobworkRec = DS.Tables[0].Rows[0]["ALLOWBACKDATEJobworkRec"].ToString();
                    model.VendorJWAdjustAutoOrManual = DS.Tables[0].Rows[0]["vendorJWAdjustAutoOrManual"].ToString();
                    model.FIFOBasedBatchInventoryInJobWorkIss = DS.Tables[0].Rows[0]["FIFOBasedBatchInventoryInJobWorkIss"].ToString();

                }
                if (Type == "StockAdjDetail")
                {
                    model.AllowBackDateStockAdjustment = DS.Tables[0].Rows[0]["AllowBackDateStockAdjustment"].ToString();
                    model.AllowBackDateInterStoreTransfer = DS.Tables[0].Rows[0]["AllowBackDateInterStoreTransfer"].ToString();

                }
                if (Type == "IssWithBomDetail")
                {
                    model.AllowBackdateIssueWOBOM = DS.Tables[0].Rows[0]["AllowBackdateIssueWOBOM"].ToString();
                    model.AllowBackDateISSUEWITHBOM = DS.Tables[0].Rows[0]["AllowBackDateISSUEWITHBOM"].ToString();
                    model.IssueViaScanningBarcode = DS.Tables[0].Rows[0]["IssueViaScanningBarcode"].ToString();
                    model.AllowBackDateIssueChallan = DS.Tables[0].Rows[0]["AllowBackDateIssueChallan"].ToString();
                    model.ALLOWBACKDATRECCHALLAN = DS.Tables[0].Rows[0]["ALLOWBACKDATRECCHALLAN"].ToString();

                }
                if (Type == "IssRecChallanDetail")
                {
                    model.RGPChallanPrintReportName = DS.Tables[0].Rows[0]["RGPChallanPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["RGPChallanPrintReportName"].ToString(): string.Empty;
                    model.NRGPChallanPrintReportName = DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"] != DBNull.Value? DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"].ToString() : string.Empty;
                    model.AllowBackDateReceiveChallanEntry = DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"] != DBNull.Value? DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"].ToString(): string.Empty;
                    model.IssueChaallanTaxIsMandatory = DS.Tables[0].Rows[0]["IssueChaallanTaxIsMandatory"] != DBNull.Value? DS.Tables[0].Rows[0]["IssueChaallanTaxIsMandatory"].ToString(): string.Empty;

                }
                if (Type == "JustJobWorkDetail")
                {
                    model.CustJWIssuePrintReportName = DS.Tables[0].Rows[0]["CustJWIssuePrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["RGPChallanPrintReportName"].ToString(): string.Empty;
                    model.CUSTJWRecPrintReportName = DS.Tables[0].Rows[0]["CUSTJWRecPrintReportName"] != DBNull.Value? DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"].ToString() : string.Empty;
                    model.AllowBackDateCustomerJWIssue = DS.Tables[0].Rows[0]["AllowBackDateCustomerJWIssue"] != DBNull.Value? DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"].ToString(): string.Empty;

                }
                if (Type == "PrintReportDetail")
                {
                    model.SaleBillPrintReportName = DS.Tables[0].Rows[0]["SaleBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["SaleBillPrintReportName"].ToString() : string.Empty;
                    model.JWSaleBillPrintReportName = DS.Tables[0].Rows[0]["JWSaleBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["JWSaleBillPrintReportName"].ToString() : string.Empty;
                    model.PurchaseRejPrintReportName = DS.Tables[0].Rows[0]["PurchaseRejPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchaseRejPrintReportName"].ToString() : string.Empty;
                    model.DebitNotePrintReportName = DS.Tables[0].Rows[0]["DebitNotePrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["DebitNotePrintReportName"].ToString() : string.Empty;
                    model.CreditNotePrintReportName = DS.Tables[0].Rows[0]["CreditNotePrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["CreditNotePrintReportName"].ToString() : string.Empty;
                    model.SaleRejectionPrintReportName = DS.Tables[0].Rows[0]["SaleRejectionPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["SaleRejectionPrintReportName"].ToString() : string.Empty;
                    model.PurchaseBillPrintReportName = DS.Tables[0].Rows[0]["PurchaseBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchaseBillPrintReportName"].ToString() : string.Empty;
                    model.DirectPurchaseBillPrintReportName = DS.Tables[0].Rows[0]["DirectPurchaseBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["DirectPurchaseBillPrintReportName"].ToString() : string.Empty;
                    model.PurchBillVoucherPrintReportName = DS.Tables[0].Rows[0]["PurchBillVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["PurchBillVoucherPrintReportName"].ToString() : string.Empty;
                    model.DirectPurchBillVoucherPrintReportName = DS.Tables[0].Rows[0]["DirectPurchBillVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["DirectPurchBillVoucherPrintReportName"].ToString() : string.Empty;
                    model.BankRecVoucherPrintReportName = DS.Tables[0].Rows[0]["BankRecVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["BankRecVoucherPrintReportName"].ToString() : string.Empty;
                    model.BankPaymentVoucherPrintReportName = DS.Tables[0].Rows[0]["BankPaymentVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["BankPaymentVoucherPrintReportName"].ToString() : string.Empty;
                    model.CashRecVoucherPrintReportName = DS.Tables[0].Rows[0]["CashRecVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["CashRecVoucherPrintReportName"].ToString() : string.Empty;
                    model.CashPaymentVoucherPrintReportName = DS.Tables[0].Rows[0]["CashPaymentVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["CashPaymentVoucherPrintReportName"].ToString() : string.Empty;
                    model.JournalVoucherPrintReportName = DS.Tables[0].Rows[0]["JournalVoucherPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["JournalVoucherPrintReportName"].ToString() : string.Empty;

                }
                if (Type == "ProdEntryDetail")
                {
                    model.PrdProdEntryAgainstWOProdSchReqManual = DS.Tables[0].Rows[0]["PrdProdEntryAgainstWOProdSchReqManual"] != DBNull.Value ? DS.Tables[0].Rows[0]["PrdProdEntryAgainstWOProdSchReqManual"].ToString() : string.Empty;
                    model.AllowProdWithoutBom = DS.Tables[0].Rows[0]["AllowProdWithoutBom"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowProdWithoutBom"].ToString() : string.Empty;
                    model.ProdSchAllowToAddManualNewItem = DS.Tables[0].Rows[0]["ProdSchAllowToAddManualNewItem"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdSchAllowToAddManualNewItem"].ToString() : string.Empty;
                    model.ProdEntryAllowToAddRMItem = DS.Tables[0].Rows[0]["ProdEntryAllowToAddRMItem"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdEntryAllowToAddRMItem"].ToString() : string.Empty;
                    model.ProdEntryAllowToAddWithoutRM = DS.Tables[0].Rows[0]["ProdEntryAllowToAddWithoutRM"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdEntryAllowToAddWithoutRM"].ToString() : string.Empty;
                    model.ProdEntryAllowToAddNegativeStock = DS.Tables[0].Rows[0]["ProdEntryAllowToAddNegativeStock"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdEntryAllowToAddNegativeStock"].ToString() : string.Empty;
                    model.BatchWiseProduction = DS.Tables[0].Rows[0]["BatchWiseProduction"] != DBNull.Value ? DS.Tables[0].Rows[0]["BatchWiseProduction"].ToString() : string.Empty;
                    model.ProdEntryAllowBackDate = DS.Tables[0].Rows[0]["ProdEntryAllowBackDate"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdEntryAllowBackDate"].ToString() : string.Empty;
                    model.AllowBackDateDAILYPRODUCTION = DS.Tables[0].Rows[0]["AllowBackDateDAILYPRODUCTION"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateDAILYPRODUCTION"].ToString() : string.Empty;
                    model.ProdAllowMultiplePlanInProdSchOrNot = DS.Tables[0].Rows[0]["ProdAllowMultiplePlanInProdSchOrNot"] != DBNull.Value ? DS.Tables[0].Rows[0]["ProdAllowMultiplePlanInProdSchOrNot"].ToString() : string.Empty;

                }
                if (Type == "ProdPlanSchDetail")
                {
                    model.AllowBackDateWorkorderEntry = DS.Tables[0].Rows[0]["AllowBackDateWorkorderEntry"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateWorkorderEntry"].ToString() : string.Empty;
                    model.AllowBackDateProductionScheduleEntry = DS.Tables[0].Rows[0]["AllowBackDateProductionScheduleEntry"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateProductionScheduleEntry"].ToString() : string.Empty;
                    model.InProdScheduleShowSumOrDetail = DS.Tables[0].Rows[0]["InProdScheduleShowSumOrDetail"] != DBNull.Value ? DS.Tables[0].Rows[0]["InProdScheduleShowSumOrDetail"].ToString() : string.Empty;

                }
                if (Type == "TransMtrFromWcRec")
                {
                    model.AllowBackDateTRANSFERMATERIAL = DS.Tables[0].Rows[0]["AllowBackDateTRANSFERMATERIAL"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateTRANSFERMATERIAL"].ToString() : string.Empty;
                    model.AllowBackDateRECEIVEINSTORE = DS.Tables[0].Rows[0]["AllowBackDateRECEIVEINSTORE"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateRECEIVEINSTORE"].ToString() : string.Empty;
                    
                }
                 if (Type == "SaleBillDetail")
                {
					model.SaleInvoiceFileName = DS.Tables[0].Rows[0]["SaleInvoiceFileName"] != DBNull.Value ? DS.Tables[0].Rows[0]["SaleInvoiceFileName"].ToString() : string.Empty;
					model.SaleBillPrintReportName = DS.Tables[0].Rows[0]["SaleBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["SaleBillPrintReportName"].ToString() : string.Empty;
					model.AllowMultipleBuyerInSaleOrder = DS.Tables[0].Rows[0]["AllowMultipleBuyerInSaleOrder"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowMultipleBuyerInSaleOrder"].ToString() : string.Empty;
					model.JWSaleBillPrintReportName = DS.Tables[0].Rows[0]["JWSaleBillPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["JWSaleBillPrintReportName"].ToString() : string.Empty;
					model.AllowBackDateSALEBILL = DS.Tables[0].Rows[0]["AllowBackDateSALEBILL"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowBackDateSALEBILL"].ToString() : string.Empty;
					model.salebillShowItemBatchFIFOBased = DS.Tables[0].Rows[0]["salebillShowItemBatchFIFOBased"] != DBNull.Value ? DS.Tables[0].Rows[0]["salebillShowItemBatchFIFOBased"].ToString() : string.Empty;
					model.AccSaleBillManualTaxAlloweed = DS.Tables[0].Rows[0]["AccSaleBillManualTaxAlloweed"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccSaleBillManualTaxAlloweed"].ToString() : string.Empty;
					model.AccPasswordToAllowManualTax = DS.Tables[0].Rows[0]["AccPasswordToAllowManualTax"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccPasswordToAllowManualTax"].ToString() : string.Empty;
					model.EditableRateAndDiscountONSaleInvoice = DS.Tables[0].Rows[0]["EditableRateAndDiscountONSaleInvoice"] != DBNull.Value ? DS.Tables[0].Rows[0]["EditableRateAndDiscountONSaleInvoice"].ToString() : string.Empty;
					model.ShowHideEntryDetail = DS.Tables[0].Rows[0]["ShowHideSaleBillEntryDetail"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillEntryDetail"].ToString() : string.Empty;
					model.ShowHideCustomerDetail = DS.Tables[0].Rows[0]["ShowHideSaleBillCustomerDetail"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillCustomerDetail"].ToString() : string.Empty;
					model.ShowHideOtherRequiredDetail = DS.Tables[0].Rows[0]["ShowHideSaleBillOtherRequiredDetail"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillOtherRequiredDetail"].ToString() : string.Empty;
					model.ShowHideCurrency = DS.Tables[0].Rows[0]["ShowHideSaleBillCurrency"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillCurrency"].ToString() : string.Empty;
					model.ShowHideConsignee = DS.Tables[0].Rows[0]["ShowHideSaleBillConsignee"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillConsignee"].ToString() : string.Empty;
					model.ShowHideScheduleDetail = DS.Tables[0].Rows[0]["ShowHideSaleBillScheduleDetail"] != DBNull.Value ? DS.Tables[0].Rows[0]["ShowHideSaleBillScheduleDetail"].ToString() : string.Empty;
					model.AllowToChangeStoreName = DS.Tables[0].Rows[0]["AllowToChangeSaleBillStoreName"] != DBNull.Value ? DS.Tables[0].Rows[0]["AllowToChangeSaleBillStoreName"].ToString() : string.Empty;
					model.HideOtherFieldOFDetailTable = DS.Tables[0].Rows[0]["HideOtherFieldOfSaleBillDetailTable"] != DBNull.Value ? DS.Tables[0].Rows[0]["HideOtherFieldOfSaleBillDetailTable"].ToString() : string.Empty;
					model.ApproveSOForGenerateSaleInvoiceOrNot = DS.Tables[0].Rows[0]["ApproveSOForGenerateSaleInvoiceOrNot"] != DBNull.Value ? DS.Tables[0].Rows[0]["ApproveSOForGenerateSaleInvoiceOrNot"].ToString() : string.Empty;
					
				}
				if (Type == "AccountDetail")
                {
                   
                    model.AccPasswordToAllowManualTax = DS.Tables[0].Rows[0]["AccPasswordToAllowManualTax"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccPasswordToAllowManualTax"].ToString() : string.Empty;
                    model.AccSaleBillManualTaxAllowed = DS.Tables[0].Rows[0]["AccSaleBillManualTaxAlloweed"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccSaleBillManualTaxAlloweed"].ToString() : string.Empty;
                    model.AccAllowBackdateVoucherEntry = DS.Tables[0].Rows[0]["AccAllowBackdateVoucherEntry"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccAllowBackdateVoucherEntry"].ToString() : string.Empty;
                    model.AccBackdateVoucherEntryPassword = DS.Tables[0].Rows[0]["AccBackdateVoucherEntryPassword"] != DBNull.Value ? DS.Tables[0].Rows[0]["AccBackdateVoucherEntryPassword"].ToString() : string.Empty;
                    
                }

                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new Features_OptionsModel
                        {
                            //SrNO = Convert.ToInt32(row["SrNO"].ToString()),
                            
                        });
                    }
                    model.features_OptionsModelsGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> SaveFeatures_Options(Features_OptionsModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                
             
                    if (model.Type == "ItemDetail")
                    {

                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEItemDashbaord"));
                        SqlParams.Add(new SqlParameter("@AutoGen_PartCode", model.AutoGen_PartCode));
                        SqlParams.Add(new SqlParameter("@DuplicateItemName", model.DuplicateItemName));
                        SqlParams.Add(new SqlParameter("@ItemPartcodeGenerationFormat", model.ItemPartcodeGenerationFormat));
                    }
                     if (model.Type == "PurchaseDetail")
                     {

                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEPurchaseDashbaord"));
					SqlParams.Add(new SqlParameter("@PurchaseorderPrintReportName", model.PurchaseorderPrintReportName));
					SqlParams.Add(new SqlParameter("@PurchaseorderAmendPrintReportName", model.PurchaseorderAmendPrintReportName));
					SqlParams.Add(new SqlParameter("@PurchasSchedulePrintReportName", model.PurchasSchedulePrintReportName));
					SqlParams.Add(new SqlParameter("@PurchasScheduleAmendPrintReportName", model.PurchasScheduleAmendPrintReportName));
					SqlParams.Add(new SqlParameter("@IndentReportName", model.IndentReportName));
					SqlParams.Add(new SqlParameter("@PoallowtoprintWithoutApproval", model.PoallowtoprintWithoutApproval));
					SqlParams.Add(new SqlParameter("@PurchaseIncrementPercentageofRMInMRP", model.PurchaseIncrementPercentageofRMInMRP));
					SqlParams.Add(new SqlParameter("@PONotAllowedWithoutPartCodePartyWiseDefinition", model.PONotAllowedWithoutPartCodePartyWiseDefinition));
					SqlParams.Add(new SqlParameter("@Allow_PO_Print", model.Allow_PO_Print));
					SqlParams.Add(new SqlParameter("@Po_File_Name", model.Po_File_Name));
					SqlParams.Add(new SqlParameter("@PONOEditable", model.PONOEditable));
					SqlParams.Add(new SqlParameter("@PONOYearlyRenew", model.PONOYearlyRenew));
					SqlParams.Add(new SqlParameter("@AutoGeneratedPUrchaseOrder", model.AutoGeneratedPUrchaseOrder));
					SqlParams.Add(new SqlParameter("@CheckPOPendFromPOonlyNotFromAmendment", model.CheckPOPendFromPOonlyNotFromAmendment));
					SqlParams.Add(new SqlParameter("@POClosePOAlwaysAgainstIndent", model.POClosePOAlwaysAgainstIndent));

				}


				if (model.Type == "PurchaseBill") 
                    {
                   
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATEPurchaseBillDashbaord"));
					SqlParams.Add(new SqlParameter("@PurchaseBillPrintReportName", model.PurchaseBillPrintReportName));
					SqlParams.Add(new SqlParameter("@DirectPurchaseBillPrintReportName", model.DirectPurchaseBillPrintReportName));
					SqlParams.Add(new SqlParameter("@PurchBillVoucherPrintReportName", model.PurchBillVoucherPrintReportName));
					SqlParams.Add(new SqlParameter("@DirectPurchBillVoucherPrintReportName", model.DirectPurchBillVoucherPrintReportName));
					SqlParams.Add(new SqlParameter("@AccAllowtochangeDocumnetinPurchaseBill", model.AccAllowtochangeDocumnetinPurchaseBill));
					SqlParams.Add(new SqlParameter("@AccAllowTochangeVoucherDateInPurchBill", model.AccAllowTochangeVoucherDateInPurchBill));
					SqlParams.Add(new SqlParameter("@AccAllowtochangeInvoiceNoDateinPurchaseBill", model.AccAllowtochangeInvoiceNoDateinPurchaseBill));
					SqlParams.Add(new SqlParameter("@AccPurchaseVoucherPrintoutFileName", model.AccPurchaseVoucherPrintoutFileName));
					SqlParams.Add(new SqlParameter("@AccPurchaseBillInvoicePrintoutFileName", model.AccPurchaseBillInvoicePrintoutFileName));

				}
				if (model.Type == "SaleOrderDetail")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATESaleOrderDashbaord"));
                        SqlParams.Add(new SqlParameter("@SaleInvoiceFileName", model.SaleInvoiceFileName));
                        SqlParams.Add(new SqlParameter("@AutoGenItemGroupCode", model.AutoGenItemGroupCode));
                        SqlParams.Add(new SqlParameter("@AllowMultipleBuyerInSaleOrder", model.AllowMultipleBuyerInSaleOrder));
                        SqlParams.Add(new SqlParameter("@SaleorderPrintReportName", model.SaleorderPrintReportName));
                        SqlParams.Add(new SqlParameter("@SaleAmendPrintReportName", model.SaleAmendPrintReportName));
                        SqlParams.Add(new SqlParameter("@SaleSchedulePrintReportName", model.SaleSchedulePrintReportName));
                        SqlParams.Add(new SqlParameter("@SaleScheduleAmendPrintReportName", model.SaleScheduleAmendPrintReportName));
                        SqlParams.Add(new SqlParameter("@ShowHideSaleOrderEntryDetail", model.ShowHideSaleOrderEntryDetail));
                        SqlParams.Add(new SqlParameter("@ShowHideSaleOrderOtherRequiredDetail", model.ShowHideSaleOrderOtherRequiredDetail));
                        SqlParams.Add(new SqlParameter("@ShowHideSaleOrderConsignee", model.ShowHideSaleOrderConsignee));
                        SqlParams.Add(new SqlParameter("@HideOtherFieldOFSaleOrderDetailTable", model.HideOtherFieldOFSaleOrderDetailTable));

                    }
                    if (model.Type == "GateEntryDetail")
                    {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEGateEntryDashbaord"));
                        SqlParams.Add(new SqlParameter("@BlockGateEntry4UnAppPOAmm", model.BlockGateEntry4UnAppPOAmm));
                        SqlParams.Add(new SqlParameter("@AllowBackDateGAteEntry", model.AllowBackDateGAteEntry));
                        SqlParams.Add(new SqlParameter("@ShowRateINGAteMrn", model.ShowRateINGAteMrn));
                        SqlParams.Add(new SqlParameter("@GateEntryPrintReportName", model.GateEntryPrintReportName));
                        SqlParams.Add(new SqlParameter("@AllowGateRateEnabled", model.AllowGateRateEnabled));


                    }
                     if (model.Type == "MRNDetail")
                     {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEMRNDashbaord"));
                        SqlParams.Add(new SqlParameter("@AllowBackDateMRNEntry", model.AllowBackDateMRNEntry));
                        SqlParams.Add(new SqlParameter("@AllowToChangeStoreInMRN", model.AllowToChangeStoreInMRN));
                        SqlParams.Add(new SqlParameter("@MRNPrintReportName", model.MRNPrintReportName));
                        SqlParams.Add(new SqlParameter("@AllowBackDateMRIR", model.AllowBackDateMRIR));

                     }
                      if (model.Type == "CommonDetail")
                     {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATECommonDashbaord"));
                        SqlParams.Add(new SqlParameter("@CCMandatoryInQuery", model.CCMandatoryInQuery));
                        SqlParams.Add(new SqlParameter("@BatchWiseInventory", model.BatchWiseInventory));
                        SqlParams.Add(new SqlParameter("@AllowToChangeBranch", model.AllowToChangeBranch));
                        SqlParams.Add(new SqlParameter("@AllowBAtchEditable", model.AllowBAtchEditable));
                        SqlParams.Add(new SqlParameter("@AllowBAtchEditablePAssword", model.AllowBAtchEditablePAssword));
                        //SqlParams.Add(new SqlParameter("@FIFOBasedBatchInventory", model.FIFOBasedBatchInventory));
                        SqlParams.Add(new SqlParameter("@FIFOBasedBatchInventoryInJobWorkIss", model.FIFOBasedBatchInventoryInJobWorkIss));


                    }
                     if (model.Type == "RequisitionDetail")
                     {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATERequisitionDashbaord"));
                        SqlParams.Add(new SqlParameter("@AllowBackdateReqWOBOM", model.AllowBackdateReqWOBOM));
                        SqlParams.Add(new SqlParameter("@MaxDurationForReqToBePend", model.MaxDurationForReqToBePend));
                        SqlParams.Add(new SqlParameter("@IssueWithScanner", model.IssueWithScanner));
                        SqlParams.Add(new SqlParameter("@AllowBackDateRequsitionWITHBOM", model.AllowBackDateRequsitionWITHBOM));
                      

                     }
                     if (model.Type == "VendJwIssRecDetail")
                     {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEVendJwIssRecDashbaord"));
                        SqlParams.Add(new SqlParameter("@VendJWIssuePrintReportName", model.VendJWIssuePrintReportName));
                        SqlParams.Add(new SqlParameter("@VendJWRecPrintReportName", model.VendJWRecPrintReportName));
                        SqlParams.Add(new SqlParameter("@AllowBackDateJOBWorkIssue", model.AllowBackDateJOBWorkIssue));
                        SqlParams.Add(new SqlParameter("@ALLOWBACKDATEJobworkRec", model.ALLOWBACKDATEJobworkRec));
                        SqlParams.Add(new SqlParameter("@vendorJWAdjustAutoOrManual", model.VendorJWAdjustAutoOrManual));
                        SqlParams.Add(new SqlParameter("@FIFOBasedBatchInventoryInJobWorkIss", model.FIFOBasedBatchInventoryInJobWorkIss));
                     }
                     if (model.Type == "StockAdjDetail")
                     {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEStockAdjDashbaord"));
                        SqlParams.Add(new SqlParameter("@AllowBackDateStockAdjustment", model.AllowBackDateStockAdjustment));
                        SqlParams.Add(new SqlParameter("@AllowBackDateInterStoreTransfer", model.AllowBackDateInterStoreTransfer));

                    }
                      if (model.Type == "IssWithBomDetail")
                      {
                        SqlParams.Add(new SqlParameter("@Flag", "UPDATEIssWithBomDashbaord"));
                        SqlParams.Add(new SqlParameter("@AllowBackdateIssueWOBOM", model.AllowBackdateIssueWOBOM));
                        SqlParams.Add(new SqlParameter("@AllowBackDateISSUEWITHBOM", model.AllowBackDateISSUEWITHBOM));
                        SqlParams.Add(new SqlParameter("@IssueViaScanningBarcode", model.IssueViaScanningBarcode));
                        SqlParams.Add(new SqlParameter("@AllowBackDateIssueChallan", model.AllowBackDateIssueChallan));
                        SqlParams.Add(new SqlParameter("@ALLOWBACKDATRECCHALLAN", model.ALLOWBACKDATRECCHALLAN));
 
                      }
                        if (model.Type == "IssRecChallanDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATEIssChallanDashbaord"));
                            SqlParams.Add(new SqlParameter("@RGPChallanPrintReportName", model.RGPChallanPrintReportName));
                            SqlParams.Add(new SqlParameter("@NRGPChallanPrintReportName", model.NRGPChallanPrintReportName));
                            SqlParams.Add(new SqlParameter("@AllowBackDateReceiveChallanEntry", model.AllowBackDateReceiveChallanEntry));
                            SqlParams.Add(new SqlParameter("@IssueChaallanTaxIsMandatory", model.IssueChaallanTaxIsMandatory));

                        }
                        if (model.Type == "JustJobWorkDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATECustJobWorkDashbaord"));
                            SqlParams.Add(new SqlParameter("@CustJWIssuePrintReportName", model.CustJWIssuePrintReportName));
                            SqlParams.Add(new SqlParameter("@CUSTJWRecPrintReportName", model.CUSTJWRecPrintReportName));
                            SqlParams.Add(new SqlParameter("@AllowBackDateCustomerJWIssue", model.AllowBackDateCustomerJWIssue));

                        }
                        if (model.Type == "PrintReportDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATEPrintReportDashbaord"));
                            SqlParams.Add(new SqlParameter("@SaleBillPrintReportName", model.SaleBillPrintReportName));
                            SqlParams.Add(new SqlParameter("@JWSaleBillPrintReportName", model.JWSaleBillPrintReportName));
                            SqlParams.Add(new SqlParameter("@PurchaseRejPrintReportName", model.PurchaseRejPrintReportName));
                            SqlParams.Add(new SqlParameter("@DebitNotePrintReportName", model.DebitNotePrintReportName));
                            SqlParams.Add(new SqlParameter("@CreditNotePrintReportName", model.CreditNotePrintReportName));
                            SqlParams.Add(new SqlParameter("@SaleRejectionPrintReportName", model.SaleRejectionPrintReportName));
                            SqlParams.Add(new SqlParameter("@PurchaseBillPrintReportName", model.PurchaseBillPrintReportName));
                            SqlParams.Add(new SqlParameter("@DirectPurchaseBillPrintReportName", model.DirectPurchaseBillPrintReportName));
                            SqlParams.Add(new SqlParameter("@PurchBillVoucherPrintReportName", model.PurchBillVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@DirectPurchBillVoucherPrintReportName", model.DirectPurchBillVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@BankRecVoucherPrintReportName", model.BankRecVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@BankPaymentVoucherPrintReportName", model.BankPaymentVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@CashRecVoucherPrintReportName", model.CashRecVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@CashPaymentVoucherPrintReportName", model.CashPaymentVoucherPrintReportName));
                            SqlParams.Add(new SqlParameter("@JournalVoucherPrintReportName", model.JournalVoucherPrintReportName));

                    }

                    if (model.Type == "ProdEntryDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATEProdEntryDashbaord"));
                            SqlParams.Add(new SqlParameter("@PrdProdEntryAgainstWOProdSchReqManual", model.PrdProdEntryAgainstWOProdSchReqManual));
                            SqlParams.Add(new SqlParameter("@AllowProdWithoutBom", model.AllowProdWithoutBom));
                            SqlParams.Add(new SqlParameter("@ProdSchAllowToAddManualNewItem", model.ProdSchAllowToAddManualNewItem));
                            SqlParams.Add(new SqlParameter("@ProdEntryAllowToAddRMItem", model.ProdEntryAllowToAddRMItem));
                            SqlParams.Add(new SqlParameter("@ProdEntryAllowToAddWithoutRM", model.ProdEntryAllowToAddWithoutRM));
                            SqlParams.Add(new SqlParameter("@ProdEntryAllowToAddNegativeStock", model.ProdEntryAllowToAddNegativeStock));
                            SqlParams.Add(new SqlParameter("@BatchWiseProduction", model.BatchWiseProduction));
                            SqlParams.Add(new SqlParameter("@ProdEntryAllowBackDate", model.ProdEntryAllowBackDate));
                            SqlParams.Add(new SqlParameter("@AllowBackDateDAILYPRODUCTION", model.AllowBackDateDAILYPRODUCTION));
                            SqlParams.Add(new SqlParameter("@ProdAllowMultiplePlanInProdSchOrNot", model.ProdAllowMultiplePlanInProdSchOrNot));

                        }
                        if (model.Type == "ProdPlanSchDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATEProdPlanSchDashbaord"));
                            SqlParams.Add(new SqlParameter("@AllowBackDateWorkorderEntry", model.AllowBackDateWorkorderEntry));
                            SqlParams.Add(new SqlParameter("@AllowBackDateProductionScheduleEntry", model.AllowBackDateProductionScheduleEntry));
                            SqlParams.Add(new SqlParameter("@InProdScheduleShowSumOrDetail", model.InProdScheduleShowSumOrDetail));

                        }

                        if (model.Type == "TransMtrFromWcRec")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATETransMtrFromWcRecDashbaord"));
                            SqlParams.Add(new SqlParameter("@AllowBackDateTRANSFERMATERIAL", model.AllowBackDateTRANSFERMATERIAL));
                            SqlParams.Add(new SqlParameter("@AllowBackDateRECEIVEINSTORE", model.AllowBackDateRECEIVEINSTORE));

                        }
                        if (model.Type == "SaleBillDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATESaleBill"));
					SqlParams.Add(new SqlParameter("@SaleInvoiceFileName", model.SaleInvoiceFileName));
					SqlParams.Add(new SqlParameter("@SaleBillPrintReportName", model.SaleBillPrintReportName));
					SqlParams.Add(new SqlParameter("@AllowMultipleBuyerInSaleOrder", model.AllowMultipleBuyerInSaleOrder));
					SqlParams.Add(new SqlParameter("@JWSaleBillPrintReportName", model.JWSaleBillPrintReportName));
					SqlParams.Add(new SqlParameter("@AllowBackDateSALEBILL", model.AllowBackDateSALEBILL));
					SqlParams.Add(new SqlParameter("@salebillShowItemBatchFIFOBased", model.salebillShowItemBatchFIFOBased));
					SqlParams.Add(new SqlParameter("@AccSaleBillManualTaxAlloweed", model.AccSaleBillManualTaxAlloweed));
					SqlParams.Add(new SqlParameter("@AccPasswordToAllowManualTax", model.AccPasswordToAllowManualTax));
					SqlParams.Add(new SqlParameter("@EditableRateAndDiscountONSaleInvoice", model.EditableRateAndDiscountONSaleInvoice));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillEntryDetail", model.ShowHideEntryDetail));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillCustomerDetail", model.ShowHideCustomerDetail));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillOtherRequiredDetail", model.ShowHideOtherRequiredDetail));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillCurrency", model.ShowHideCurrency));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillConsignee", model.ShowHideConsignee));
					SqlParams.Add(new SqlParameter("@ShowHideSaleBillScheduleDetail", model.ShowHideScheduleDetail));
					SqlParams.Add(new SqlParameter("@AllowToChangeSaleBillStoreName", model.AllowToChangeStoreName));
					SqlParams.Add(new SqlParameter("@HideOtherFieldOfSaleBillDetailTable", model.HideOtherFieldOFDetailTable));
					SqlParams.Add(new SqlParameter("@ApproveSOForGenerateSaleInvoiceOrNot", model.ApproveSOForGenerateSaleInvoiceOrNot));

				}
				if (model.Type == "AccountDetail")
                         {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATEAccount"));
                           
                            SqlParams.Add(new SqlParameter("@AccPasswordToAllowManualTax", model.AccPasswordToAllowManualTax));
                            SqlParams.Add(new SqlParameter("@AccSaleBillManualTaxAlloweed", model.AccSaleBillManualTaxAllowed));
                            SqlParams.Add(new SqlParameter("@AccAllowBackdateVoucherEntry", model.AccAllowBackdateVoucherEntry));
                            SqlParams.Add(new SqlParameter("@AccBackdateVoucherEntryPassword", model.AccBackdateVoucherEntryPassword));
                            
                         }



               // _ResponseResult = await _IDataLogic.ExecuteDataTable("Sp_FeaturesOptions", SqlParams);
                _ResponseResult = await _IDataLogic.ExecuteDataSet("Sp_FeaturesOptions", SqlParams).ConfigureAwait(false);
             

            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }
            return _ResponseResult;
        }
    }
}
