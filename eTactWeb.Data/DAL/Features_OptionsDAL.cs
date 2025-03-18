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

        public Features_OptionsDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
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

                        oCmd.Parameters.AddWithValue("@flag", "UPDATEPurchaseDashbaord");
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
                                                                FIFOBasedBatchInventory = dr["FIFOBasedBatchInventory"] != DBNull.Value ? dr["FIFOBasedBatchInventory"].ToString() : string.Empty,


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
                if (Type == "PurchaseDetail")
                {
                    model.Po_File_Name = DS.Tables[0].Rows[0]["Po_File_Name"].ToString();
                    model.PONOEditable = DS.Tables[0].Rows[0]["PONOEditable"].ToString();
                    model.PONOYearlyRenew = DS.Tables[0].Rows[0]["PONOYearlyRenew"].ToString();
                    model.AutoGeneratedPUrchaseOrder = DS.Tables[0].Rows[0]["AutoGeneratedPUrchaseOrder"].ToString();
                    model.PurchaseOrderReport = DS.Tables[0].Rows[0]["PurchaseOrderReport"].ToString();
                    model.PurchaseorderPrintReportName = DS.Tables[0].Rows[0]["PurchaseorderPrintReportName"].ToString();
                    model.PurchaseorderAmendPrintReportName = DS.Tables[0].Rows[0]["PurchaseorderAmendPrintReportName"].ToString();
                    model.PurchasSchedulePrintReportName = DS.Tables[0].Rows[0]["PurchasSchedulePrintReportName"].ToString();
                    model.PurchasScheduleAmendPrintReportName = DS.Tables[0].Rows[0]["PurchasScheduleAmendPrintReportName"].ToString();
                    model.AllowBackDatePURCHASEORDER = DS.Tables[0].Rows[0]["AllowBackDatePURCHASEORDER"].ToString();
                    model.AllowBackDateINDENT = DS.Tables[0].Rows[0]["AllowBackDateINDENT"].ToString();
                    model.CheckPOPendFromPOonlyNotFromAmendment = DS.Tables[0].Rows[0]["CheckPOPendFromPOonlyNotFromAmendment"].ToString();
                }
                if (model.Type == "SaleOrderDetail")
                {
                    model.SaleInvoiceFileName = DS.Tables[0].Rows[0]["SaleInvoiceFileName"].ToString();
                    model.AutoGenItemGroupCode = DS.Tables[0].Rows[0]["AutoGenItemGroupCode"].ToString();
                    model.AllowMultipleBuyerInSaleOrder = DS.Tables[0].Rows[0]["AllowMultipleBuyerInSaleOrder"].ToString();
                    model.SaleorderPrintReportName = DS.Tables[0].Rows[0]["SaleorderPrintReportName"].ToString();
                    model.SaleAmendPrintReportName = DS.Tables[0].Rows[0]["SaleAmendPrintReportName"].ToString();
                    model.SaleSchedulePrintReportName = DS.Tables[0].Rows[0]["SaleSchedulePrintReportName"].ToString();
                    model.SaleScheduleAmendPrintReportName = DS.Tables[0].Rows[0]["SaleScheduleAmendPrintReportName"].ToString();

                } 
                if (model.Type == "GateEntryDetail")
                {
                    model.BlockGateEntry4UnAppPOAmm = DS.Tables[0].Rows[0]["BlockGateEntry4UnAppPOAmm"].ToString();
                    model.AllowBackDateGAteEntry = DS.Tables[0].Rows[0]["AllowBackDateGAteEntry"].ToString();
                    model.ShowRateINGAteMrn = DS.Tables[0].Rows[0]["ShowRateINGAteMrn"].ToString();
                    model.GateEntryPrintReportName = DS.Tables[0].Rows[0]["GateEntryPrintReportName"].ToString();
                    model.AllowGateRateEnabled = DS.Tables[0].Rows[0]["AllowGateRateEnabled"].ToString();

                }
                if (model.Type == "MRNDetail")
                {
                    model.AllowBackDateMRNEntry = DS.Tables[0].Rows[0]["AllowBackDateMRNEntry"].ToString();
                    model.AllowToChangeStoreInMRN = DS.Tables[0].Rows[0]["AllowToChangeStoreInMRN"].ToString();
                    model.MRNPrintReportName = DS.Tables[0].Rows[0]["MRNPrintReportName"].ToString();
                    model.AllowBackDateMRIR = DS.Tables[0].Rows[0]["AllowBackDateMRIR"].ToString();


                }
                if (model.Type == "CommonDetail")
                {
                    model.CCMandatoryInQuery = DS.Tables[0].Rows[0]["CCMandatoryInQuery"].ToString();
                    model.BatchWiseInventory = DS.Tables[0].Rows[0]["BatchWiseInventory"].ToString();
                    model.AllowToChangeBranch = DS.Tables[0].Rows[0]["AllowToChangeBranch"].ToString();
                    model.AllowBAtchEditable = DS.Tables[0].Rows[0]["AllowBAtchEditable"].ToString();
                    model.AllowBAtchEditablePAssword = DS.Tables[0].Rows[0]["AllowBAtchEditablePAssword"].ToString();
                    model.FIFOBasedBatchInventory = DS.Tables[0].Rows[0]["FIFOBasedBatchInventory"].ToString();

                }
                if (model.Type == "RequisitionDetail")
                {
                    model.AllowBackdateReqWOBOM = DS.Tables[0].Rows[0]["AllowBackdateReqWOBOM"].ToString();
                    model.MaxDurationForReqToBePend = Convert.ToInt64(DS.Tables[0].Rows[0]["MaxDurationForReqToBePend"]);
                    model.IssueWithScanner = DS.Tables[0].Rows[0]["IssueWithScanner"].ToString();
                    model.AllowBackDateRequsitionWITHBOM = DS.Tables[0].Rows[0]["AllowBackDateRequsitionWITHBOM"].ToString();
                  
                }
                if (model.Type == "VendJwIssRecDetail")
                {
                    model.VendJWIssuePrintReportName = DS.Tables[0].Rows[0]["VendJWIssuePrintReportName"].ToString();
                    model.VendJWRecPrintReportName = DS.Tables[0].Rows[0]["VendJWRecPrintReportName"].ToString();
                    model.AllowBackDateJOBWorkIssue = DS.Tables[0].Rows[0]["AllowBackDateJOBWorkIssue"].ToString();
                    model.ALLOWBACKDATEJobworkRec = DS.Tables[0].Rows[0]["ALLOWBACKDATEJobworkRec"].ToString();
                    model.VendorJWAdjustAutoOrManual = DS.Tables[0].Rows[0]["vendorJWAdjustAutoOrManual"].ToString();

                }
                if (model.Type == "StockAdjDetail")
                {
                    model.AllowBackDateStockAdjustment = DS.Tables[0].Rows[0]["AllowBackDateStockAdjustment"].ToString();
                    model.AllowBackDateInterStoreTransfer = DS.Tables[0].Rows[0]["AllowBackDateInterStoreTransfer"].ToString();

                }
                if (model.Type == "IssWithBomDetail")
                {
                    model.AllowBackdateIssueWOBOM = DS.Tables[0].Rows[0]["AllowBackdateIssueWOBOM"].ToString();
                    model.AllowBackDateISSUEWITHBOM = DS.Tables[0].Rows[0]["AllowBackDateISSUEWITHBOM"].ToString();
                    model.IssueViaScanningBarcode = DS.Tables[0].Rows[0]["IssueViaScanningBarcode"].ToString();
                    model.AllowBackDateIssueChallan = DS.Tables[0].Rows[0]["AllowBackDateIssueChallan"].ToString();
                    model.ALLOWBACKDATRECCHALLAN = DS.Tables[0].Rows[0]["ALLOWBACKDATRECCHALLAN"].ToString();

                }
                if (model.Type == "IssRecChallanDetail")
                {
                    model.RGPChallanPrintReportName = DS.Tables[0].Rows[0]["RGPChallanPrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["RGPChallanPrintReportName"].ToString(): string.Empty;
                    model.NRGPChallanPrintReportName = DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"] != DBNull.Value? DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"].ToString() : string.Empty;
                    model.AllowBackDateReceiveChallanEntry = DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"] != DBNull.Value? DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"].ToString(): string.Empty;

                }
                 if (model.Type == "JustJobWorkDetail")
                {
                    model.CustJWIssuePrintReportName = DS.Tables[0].Rows[0]["CustJWIssuePrintReportName"] != DBNull.Value ? DS.Tables[0].Rows[0]["RGPChallanPrintReportName"].ToString(): string.Empty;
                    model.CUSTJWRecPrintReportName = DS.Tables[0].Rows[0]["CUSTJWRecPrintReportName"] != DBNull.Value? DS.Tables[0].Rows[0]["NRGPChallanPrintReportName"].ToString() : string.Empty;
                    model.AllowBackDateCustomerJWIssue = DS.Tables[0].Rows[0]["AllowBackDateCustomerJWIssue"] != DBNull.Value? DS.Tables[0].Rows[0]["AllowBackDateReceiveChallanEntry"].ToString(): string.Empty;

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
                
                if (model.Mode == "U" || model.Mode == "V")
                {
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
                        SqlParams.Add(new SqlParameter("@Po_File_Name", model.Po_File_Name));
                        SqlParams.Add(new SqlParameter("@PONOEditable", model.PONOEditable));
                        SqlParams.Add(new SqlParameter("@PONOYearlyRenew", model.PONOYearlyRenew));
                        SqlParams.Add(new SqlParameter("@AutoGeneratedPUrchaseOrder", model.AutoGeneratedPUrchaseOrder));
                        SqlParams.Add(new SqlParameter("@PurchaseOrderReport", model.PurchaseOrderReport));
                        SqlParams.Add(new SqlParameter("@PurchaseorderPrintReportName", model.PurchaseorderPrintReportName));
                        SqlParams.Add(new SqlParameter("@PurchaseorderAmendPrintReportName", model.PurchaseorderAmendPrintReportName));
                        SqlParams.Add(new SqlParameter("@PurchasSchedulePrintReportName", model.PurchasSchedulePrintReportName));
                        SqlParams.Add(new SqlParameter("@PurchasScheduleAmendPrintReportName", model.PurchasScheduleAmendPrintReportName));
                        SqlParams.Add(new SqlParameter("@AllowBackDatePURCHASEORDER", model.AllowBackDatePURCHASEORDER));
                        SqlParams.Add(new SqlParameter("@AllowBackDateINDENT", model.AllowBackDateINDENT));
                        SqlParams.Add(new SqlParameter("@CheckPOPendFromPOonlyNotFromAmendment", model.CheckPOPendFromPOonlyNotFromAmendment));
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
                        SqlParams.Add(new SqlParameter("@FIFOBasedBatchInventory", model.FIFOBasedBatchInventory));


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

                        }
                        if (model.Type == "JustJobWorkDetail")
                        {
                            SqlParams.Add(new SqlParameter("@Flag", "UPDATECustJobWorkDashbaord"));
                            SqlParams.Add(new SqlParameter("@CustJWIssuePrintReportName", model.CustJWIssuePrintReportName));
                            SqlParams.Add(new SqlParameter("@CUSTJWRecPrintReportName", model.CUSTJWRecPrintReportName));
                            SqlParams.Add(new SqlParameter("@AllowBackDateCustomerJWIssue", model.AllowBackDateCustomerJWIssue));

                        }

                }


                _ResponseResult = await _IDataLogic.ExecuteDataTable("Sp_FeaturesOptions", SqlParams);


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
