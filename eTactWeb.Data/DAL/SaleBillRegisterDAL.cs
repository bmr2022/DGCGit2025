using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class SaleBillRegisterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public SaleBillRegisterDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _httpContextAccessor = httpContextAccessor;
        }
        //3
        public async Task<SaleBillRegisterModel> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, string PartCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO)
        {
            DataSet? oDataSet = new DataSet();
            var model = new SaleBillRegisterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPReportSalebillRegister", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    if (ReportType.ToString().ToUpper() == "SaleSummary".ToUpper())
                    {
                        oCmd = new SqlCommand("SPReportSalebillRegister", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    else
                    {
                        oCmd = new SqlCommand("SPReportSalebillRegister", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                    }
                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    oCmd.Parameters.AddWithValue("@Flag", ReportType);
                    //oCmd.Parameters.AddWithValue("@FromDate", fromDt.ToString("yyyy/MM/dd"));
                    //oCmd.Parameters.AddWithValue("@ToDate", toDt.ToString("yyyy/MM/dd"));

                    oCmd.Parameters.AddWithValue("@FromDate", fromDt);
                    oCmd.Parameters.AddWithValue("@ToDate", toDt);
                    oCmd.Parameters.AddWithValue("@PartCode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@Docname", docname);
                    oCmd.Parameters.AddWithValue("@SONo", SONo);
                    oCmd.Parameters.AddWithValue("@Schno", Schno);
                    oCmd.Parameters.AddWithValue("@InvNo", SaleBillNo);
                    oCmd.Parameters.AddWithValue("@GSTNo", GSTNO);
                    oCmd.Parameters.AddWithValue("@HSNNO", HSNNO);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType.ToString().ToUpper() == "SaleSummary".ToUpper())
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            Description = Convert.ToString(dr["Description"].ToString()),
                                                            forTheDuration = Convert.ToDecimal(dr["forTheDuration"].ToString()),
                                                            ForFinYear = Convert.ToDecimal(dr["ForFinYear"].ToString())
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "SALESUMMARYREG" || ReportType.ToString().ToUpper() == "GSTSALESUMMARY") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            SaleBillNo = string.IsNullOrEmpty(dr["InvoiceNo"].ToString()) ? "" : dr["InvoiceNo"].ToString(),
                                                            SaleBillDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? "" : dr["InvDate"].ToString(),
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            GSTNO = string.IsNullOrEmpty(dr["GSTNO"].ToString()) ? "" : dr["GSTNO"].ToString(),
                                                            CustAddress = string.IsNullOrEmpty(dr["CustAddress"].ToString()) ? "" : dr["CustAddress"].ToString(),
                                                            State = string.IsNullOrEmpty(dr["State"].ToString()) ? "" : dr["State"].ToString(),
                                                            StateCode = string.IsNullOrEmpty(dr["StateCode"].ToString()) ? "" : dr["StateCode"].ToString(),
                                                            Country = string.IsNullOrEmpty(dr["Country"].ToString()) ? "" : dr["Country"].ToString(),
                                                            ConsigneeName = string.IsNullOrEmpty(dr["ConsigneeName"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            ConsigneeAddress = string.IsNullOrEmpty(dr["ConsigneeAddress"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            BillAmt = Convert.ToDecimal(dr["BillAmt"].ToString()),
                                                            TaxableAmt = Convert.ToDecimal(dr["TaxableAmt"].ToString()),
                                                            GSTAmount = Convert.ToDecimal(dr["GSTAmount"].ToString()),
                                                            Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),
                                                            TotalBillQty = Convert.ToDecimal(dr["TotalBillQty"].ToString()),
                                                            TotalDisAmt = Convert.ToDecimal(dr["TotalDisAmt"].ToString()),
                                                            TotalItemAmt = Convert.ToDecimal(dr["TotalItemAmt"].ToString()),
                                                            CGSTPer = Convert.ToDecimal(dr["CGSTPer"].ToString()),
                                                            CGSTAmt = Convert.ToDecimal(dr["CGSTAmt"].ToString()),
                                                            SGSTPer = Convert.ToDecimal(dr["SGSTPer"].ToString()),
                                                            SGSTAmt = Convert.ToDecimal(dr["SGSTAmt"].ToString()),
                                                            IGSTPer = Convert.ToDecimal(dr["IGSTPer"].ToString()),
                                                            IGSTAmt = Convert.ToDecimal(dr["IGSTAmt"].ToString()),

                                                            ExpenseAmt = Convert.ToDecimal(dr["ExpenseAmt"].ToString()),
                                                            InvAmt = Convert.ToDecimal(dr["InvAmt"].ToString()),
                                                            TypeItemServAssets = string.IsNullOrEmpty(dr["TypeItemServAssets"].ToString()) ? "" : dr["TypeItemServAssets"].ToString(),
                                                            DomesticExportNEPZ = string.IsNullOrEmpty(dr["DomesticExportNEPZ"].ToString()) ? "" : dr["DomesticExportNEPZ"].ToString(),

                                                            SupplyType = string.IsNullOrEmpty(dr["SupplyType"].ToString()) ? "" : dr["SupplyType"].ToString(),
                                                            ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),


                                                            LastUpdatedByEmp = string.IsNullOrEmpty(dr["LastUpdatedByEmp"].ToString()) ? "" : dr["LastUpdatedByEmp"].ToString(),

                                                            LastUpdationDate = string.IsNullOrEmpty(dr["LastUpdationDate"].ToString()) ? "" : dr["LastUpdationDate"].ToString(),

                                                            EntryId = Convert.ToInt16(dr["salebillentryId"].ToString()),
                                                            YearCode = Convert.ToInt16(dr["salebillYearCode"].ToString()),
                                                            EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                            Remark = string.IsNullOrEmpty(dr["Remark"].ToString()) ? "" : dr["Remark"].ToString(),
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "SALEDETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            SaleBillNo = string.IsNullOrEmpty(dr["InvoiceNo"].ToString()) ? "" : dr["InvoiceNo"].ToString(),
                                                            SaleBillDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? "" : dr["InvDate"].ToString(),
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            GSTNO = string.IsNullOrEmpty(dr["GSTNO"].ToString()) ? "" : dr["GSTNO"].ToString(),
                                                            CustAddress = string.IsNullOrEmpty(dr["CustAddress"].ToString()) ? "" : dr["CustAddress"].ToString(),
                                                            State = string.IsNullOrEmpty(dr["State"].ToString()) ? "" : dr["State"].ToString(),
                                                            StateCode = string.IsNullOrEmpty(dr["StateCode"].ToString()) ? "" : dr["StateCode"].ToString(),
                                                            Country = string.IsNullOrEmpty(dr["Country"].ToString()) ? "" : dr["Country"].ToString(),
                                                            ConsigneeName = string.IsNullOrEmpty(dr["ConsigneeName"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            ConsigneeAddress = string.IsNullOrEmpty(dr["ConsigneeAddress"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            HSNNO = string.IsNullOrEmpty(dr["HSNNO"].ToString()) ? "" : dr["HSNNO"].ToString(),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                            Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                            TotalBillQty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                            TotalDisAmt = Convert.ToDecimal(dr["DisAmt"].ToString()),
                                                            Amount = Convert.ToDecimal(dr["Amount"].ToString()),
                                                            CGSTPer = Convert.ToDecimal(dr["CGSTPer"].ToString()),
                                                            CGSTAmt = Convert.ToDecimal(dr["CGSTAmt"].ToString()),
                                                            SGSTPer = Convert.ToDecimal(dr["SGSTPer"].ToString()),
                                                            SGSTAmt = Convert.ToDecimal(dr["SGSTAmt"].ToString()),
                                                            IGSTPer = Convert.ToDecimal(dr["IGSTPer"].ToString()),
                                                            IGSTAmt = Convert.ToDecimal(dr["IGSTAmt"].ToString()),
                                                            ExpenseAmt = Convert.ToDecimal(dr["ExpenseAmt"].ToString()),
                                                            BillAmt = Convert.ToDecimal(dr["BillAmt"].ToString()),
                                                            TaxableAmt = Convert.ToDecimal(dr["TaxableAmt"].ToString()),
                                                            GSTAmount = Convert.ToDecimal(dr["GSTAmount"].ToString()),
                                                            Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),
                                                            InvAmt = Convert.ToDecimal(dr["InvAmt"].ToString()),
                                                            Ewaybillno = string.IsNullOrEmpty(dr["Ewaybillno"].ToString()) ? "" : dr["Ewaybillno"].ToString(),
                                                            EInvNo = string.IsNullOrEmpty(dr["EInvNo"].ToString()) ? "" : dr["EInvNo"].ToString(),
                                                            CancelBill = string.IsNullOrEmpty(dr["CancelBill"].ToString()) ? "" : dr["CancelBill"].ToString(),
                                                            CancelReason = string.IsNullOrEmpty(dr["CancelReason"].ToString()) ? "" : dr["CancelReason"].ToString(),
                                                            Canceldate = string.IsNullOrEmpty(dr["Canceldate"].ToString()) ? "" : dr["Canceldate"].ToString(),

                                                            TypeItemServAssets = string.IsNullOrEmpty(dr["TypeItemServAssets"].ToString()) ? "" : dr["TypeItemServAssets"].ToString(),
                                                            DomesticExportNEPZ = string.IsNullOrEmpty(dr["DomesticExportNEPZ"].ToString()) ? "" : dr["DomesticExportNEPZ"].ToString(),
                                                            SupplyType = string.IsNullOrEmpty(dr["SupplyType"].ToString()) ? "" : dr["SupplyType"].ToString(),
                                                            EntryId = Convert.ToInt16(dr["salebillentryId"].ToString()),
                                                            YearCode = Convert.ToInt16(dr["salebillYearCode"].ToString()),
                                                            EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                            ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),
                                                            LastUpdatedByEmp = string.IsNullOrEmpty(dr["LastUpdatedByEmp"].ToString()) ? "" : dr["LastUpdatedByEmp"].ToString(),
                                                            LastUpdationDate = string.IsNullOrEmpty(dr["LastUpdationDate"].ToString()) ? "" : dr["LastUpdationDate"].ToString(),
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "SALECOMPLETEDETAIL") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            SaleBillNo = string.IsNullOrEmpty(dr["InvoiceNo"].ToString()) ? "" : dr["InvoiceNo"].ToString(),
                                                            SaleBillDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? "" : dr["InvDate"].ToString(),
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            GSTNO = string.IsNullOrEmpty(dr["GSTNO"].ToString()) ? "" : dr["GSTNO"].ToString(),
                                                            CustAddress = string.IsNullOrEmpty(dr["CustAddress"].ToString()) ? "" : dr["CustAddress"].ToString(),
                                                            State = string.IsNullOrEmpty(dr["State"].ToString()) ? "" : dr["State"].ToString(),
                                                            StateCode = string.IsNullOrEmpty(dr["StateCode"].ToString()) ? "" : dr["StateCode"].ToString(),
                                                            Country = string.IsNullOrEmpty(dr["Country"].ToString()) ? "" : dr["Country"].ToString(),
                                                            ConsigneeName = string.IsNullOrEmpty(dr["ConsigneeName"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            ConsigneeAddress = string.IsNullOrEmpty(dr["ConsigneeAddress"].ToString()) ? "" : dr["ConsigneeName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            HSNNO = string.IsNullOrEmpty(dr["HSNNO"].ToString()) ? "" : dr["HSNNO"].ToString(),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),
                                                            Rate = Convert.ToDecimal(dr["Rate"].ToString()),
                                                            TotalBillQty = Convert.ToDecimal(dr["Qty"].ToString()),
                                                            TotalDisAmt = Convert.ToDecimal(dr["DisAmt"].ToString()),
                                                            Amount = Convert.ToDecimal(dr["Amount"].ToString()),
                                                            BatchNo = string.IsNullOrEmpty(dr["batchno"].ToString()) ? "" : dr["batchno"].ToString(),
                                                            UniqueBatchno = string.IsNullOrEmpty(dr["uniquebatchno"].ToString()) ? "" : dr["uniquebatchno"].ToString(),
                                                            CustomerPartCode = string.IsNullOrEmpty(dr["CustomerPartCode"].ToString()) ? "" : dr["CustomerPartCode"].ToString(),
                                                            FromStore = string.IsNullOrEmpty(dr["FromStore"].ToString()) ? "" : dr["FromStore"].ToString(),
                                                            CostcenterName = string.IsNullOrEmpty(dr["CostCenterName"].ToString()) ? "" : dr["CostCenterName"].ToString(),
                                                            SONo = string.IsNullOrEmpty(dr["SONO"].ToString()) ? "" : dr["SONO"].ToString(),
                                                            CustOrderNo = string.IsNullOrEmpty(dr["CustOrderNo"].ToString()) ? "" : dr["CustOrderNo"].ToString(),
                                                            SODate = string.IsNullOrEmpty(dr["SODate"].ToString()) ? "" : dr["SODate"].ToString(),
                                                            ItemRemark = string.IsNullOrEmpty(dr["ItemRemark"].ToString()) ? "" : dr["ItemRemark"].ToString(),
                                                            TotStk = Convert.ToDecimal(dr["TotalStock"].ToString()),
                                                            //done till here 
                                                            CGSTPer = Convert.ToDecimal(dr["CGSTPer"].ToString()),
                                                            CGSTAmt = Convert.ToDecimal(dr["CGSTAmt"].ToString()),
                                                            SGSTPer = Convert.ToDecimal(dr["SGSTPer"].ToString()),
                                                            SGSTAmt = Convert.ToDecimal(dr["SGSTAmt"].ToString()),
                                                            IGSTPer = Convert.ToDecimal(dr["IGSTPer"].ToString()),
                                                            IGSTAmt = Convert.ToDecimal(dr["IGSTAmt"].ToString()),
                                                            ExpenseAmt = Convert.ToDecimal(dr["ExpenseAmt"].ToString()),
                                                            BillAmt = Convert.ToDecimal(dr["BillAmt"].ToString()),
                                                            TaxableAmt = Convert.ToDecimal(dr["TaxableAmt"].ToString()),
                                                            GSTAmount = Convert.ToDecimal(dr["GSTAmount"].ToString()),
                                                            CancelReason = string.IsNullOrEmpty(dr["CancelReason"].ToString()) ? "" : dr["CancelReason"].ToString(),
                                                            Currency = string.IsNullOrEmpty(dr["Currency"].ToString()) ? "" : dr["Currency"].ToString(),
                                                            InvAmt = Convert.ToDecimal(dr["InvAmt"].ToString()),
                                                            EinvGenerated = string.IsNullOrEmpty(dr["EinvGenerated"].ToString()) ? "" : dr["EinvGenerated"].ToString(),
                                                            Ewaybillno = string.IsNullOrEmpty(dr["Ewaybillno"].ToString()) ? "" : dr["Ewaybillno"].ToString(),
                                                            EInvNo = string.IsNullOrEmpty(dr["EInvNo"].ToString()) ? "" : dr["EInvNo"].ToString(),
                                                            TypeItemServAssets = string.IsNullOrEmpty(dr["TypeItemServAssets"].ToString()) ? "" : dr["TypeItemServAssets"].ToString(),
                                                            DomesticExportNEPZ = string.IsNullOrEmpty(dr["DomesticExportNEPZ"].ToString()) ? "" : dr["DomesticExportNEPZ"].ToString(),
                                                            SupplyType = string.IsNullOrEmpty(dr["SupplyType"].ToString()) ? "" : dr["SupplyType"].ToString(),
                                                            Approved = string.IsNullOrEmpty(dr["Approved"].ToString()) ? "" : dr["Approved"].ToString(),
                                                            BankName = string.IsNullOrEmpty(dr["BankName"].ToString()) ? "" : dr["BankName"].ToString(),
                                                            Commodity = string.IsNullOrEmpty(dr["Commodity"].ToString()) ? "" : dr["Commodity"].ToString(),
                                                            CountryOfSupply = string.IsNullOrEmpty(dr["CountryOfSupply"].ToString()) ? "" : dr["CountryOfSupply"].ToString(),
                                                            DispatchDelayReason = string.IsNullOrEmpty(dr["DispatchDelayReason"].ToString()) ? "" : dr["DispatchDelayReason"].ToString(),
                                                            DispatchThrough = string.IsNullOrEmpty(dr["DispatchThrough"].ToString()) ? "" : dr["DispatchThrough"].ToString(),
                                                            DistanceKM = Convert.ToDecimal(dr["DistanceKM"].ToString()),
                                                            EntryId = Convert.ToInt16(dr["salebillentryId"].ToString()),
                                                            YearCode = Convert.ToInt16(dr["salebillYearCode"].ToString()),
                                                            EntryByMachineName = string.IsNullOrEmpty(dr["EntryByMachineName"].ToString()) ? "" : dr["EntryByMachineName"].ToString(),
                                                            ActualEntryDate = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? "" : dr["ActualEntryDate"].ToString(),
                                                            LastUpdatedByEmp = string.IsNullOrEmpty(dr["LastUpdatedByEmp"].ToString()) ? "" : dr["LastUpdatedByEmp"].ToString(),
                                                            LastUpdationDate = string.IsNullOrEmpty(dr["LastUpdationDate"].ToString()) ? "" : dr["LastUpdationDate"].ToString(),
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "ITEMWISEMONTHLYTREND") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            AprAmt = Convert.ToDecimal(dr["AprAmt"].ToString()),
                                                            MayAmt = Convert.ToDecimal(dr["MayAmt"].ToString()),
                                                            JunAmt = Convert.ToDecimal(dr["JunAmt"].ToString()),
                                                            JulAmt = Convert.ToDecimal(dr["JulAmt"].ToString()),
                                                            AugAmt = Convert.ToDecimal(dr["AugAmt"].ToString()),
                                                            SepAmt = Convert.ToDecimal(dr["SepAmt"].ToString()),
                                                            OctAmt = Convert.ToDecimal(dr["OctAmt"].ToString()),
                                                            NovAmt = Convert.ToDecimal(dr["NovAmt"].ToString()),
                                                            DecAmt = Convert.ToDecimal(dr["DecAmt"].ToString()),
                                                            JanAmt = Convert.ToDecimal(dr["JanAmt"].ToString()),
                                                            FebAmt = Convert.ToDecimal(dr["FebAmt"].ToString()),
                                                            MarAmt = Convert.ToDecimal(dr["MarAmt"].ToString()),
                                                            AprQty = Convert.ToDecimal(dr["AprQty"].ToString()),
                                                            MayQty = Convert.ToDecimal(dr["MayQty"].ToString()),
                                                            JunQty = Convert.ToDecimal(dr["JunQty"].ToString()),
                                                            JulQty = Convert.ToDecimal(dr["JulQty"].ToString()),
                                                            AugQty = Convert.ToDecimal(dr["AugQty"].ToString()),
                                                            SepQty = Convert.ToDecimal(dr["SepQty"].ToString()),
                                                            OctQty = Convert.ToDecimal(dr["OctQty"].ToString()),
                                                            NovQty = Convert.ToDecimal(dr["NovQty"].ToString()),
                                                            DecQty = Convert.ToDecimal(dr["DecQty"].ToString()),
                                                            JanQty = Convert.ToDecimal(dr["JanQty"].ToString()),
                                                            FebQty = Convert.ToDecimal(dr["FebQty"].ToString()),
                                                            MarQty = Convert.ToDecimal(dr["MarQty"].ToString()),


                                                        }).ToList();
                    }
                }

                else if (ReportType.ToString().ToUpper() == "CUSTOMERWISEMONTHLYTREND") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            AprAmt = Convert.ToDecimal(dr["AprAmt"].ToString()),
                                                            MayAmt = Convert.ToDecimal(dr["MayAmt"].ToString()),
                                                            JunAmt = Convert.ToDecimal(dr["JunAmt"].ToString()),
                                                            JulAmt = Convert.ToDecimal(dr["JulAmt"].ToString()),
                                                            AugAmt = Convert.ToDecimal(dr["AugAmt"].ToString()),
                                                            SepAmt = Convert.ToDecimal(dr["SepAmt"].ToString()),
                                                            OctAmt = Convert.ToDecimal(dr["OctAmt"].ToString()),
                                                            NovAmt = Convert.ToDecimal(dr["NovAmt"].ToString()),
                                                            DecAmt = Convert.ToDecimal(dr["DecAmt"].ToString()),
                                                            JanAmt = Convert.ToDecimal(dr["JanAmt"].ToString()),
                                                            FebAmt = Convert.ToDecimal(dr["FebAmt"].ToString()),
                                                            MarAmt = Convert.ToDecimal(dr["MarAmt"].ToString()),
                                                            AprQty = Convert.ToDecimal(dr["AprQty"].ToString()),
                                                            MayQty = Convert.ToDecimal(dr["MayQty"].ToString()),
                                                            JunQty = Convert.ToDecimal(dr["JunQty"].ToString()),
                                                            JulQty = Convert.ToDecimal(dr["JulQty"].ToString()),
                                                            AugQty = Convert.ToDecimal(dr["AugQty"].ToString()),
                                                            SepQty = Convert.ToDecimal(dr["SepQty"].ToString()),
                                                            OctQty = Convert.ToDecimal(dr["OctQty"].ToString()),
                                                            NovQty = Convert.ToDecimal(dr["NovQty"].ToString()),
                                                            DecQty = Convert.ToDecimal(dr["DecQty"].ToString()),
                                                            JanQty = Convert.ToDecimal(dr["JanQty"].ToString()),
                                                            FebQty = Convert.ToDecimal(dr["FebQty"].ToString()),
                                                            MarQty = Convert.ToDecimal(dr["MarQty"].ToString()),
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "CUSTOMERSALESPERSONWISEMONTHLYTREND") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            SalesPersonName = string.IsNullOrEmpty(dr["SalesPersonName"].ToString()) ? "" : dr["SalesPersonName"].ToString(),

                                                            AprAmt = Convert.ToDecimal(dr["AprAmt"].ToString()),
                                                            MayAmt = Convert.ToDecimal(dr["MayAmt"].ToString()),
                                                            JunAmt = Convert.ToDecimal(dr["JunAmt"].ToString()),
                                                            JulAmt = Convert.ToDecimal(dr["JulAmt"].ToString()),
                                                            AugAmt = Convert.ToDecimal(dr["AugAmt"].ToString()),
                                                            SepAmt = Convert.ToDecimal(dr["SepAmt"].ToString()),
                                                            OctAmt = Convert.ToDecimal(dr["OctAmt"].ToString()),
                                                            NovAmt = Convert.ToDecimal(dr["NovAmt"].ToString()),
                                                            DecAmt = Convert.ToDecimal(dr["DecAmt"].ToString()),
                                                            JanAmt = Convert.ToDecimal(dr["JanAmt"].ToString()),
                                                            FebAmt = Convert.ToDecimal(dr["FebAmt"].ToString()),
                                                            MarAmt = Convert.ToDecimal(dr["MarAmt"].ToString()),
                                                            AprQty = Convert.ToDecimal(dr["AprQty"].ToString()),
                                                            MayQty = Convert.ToDecimal(dr["MayQty"].ToString()),
                                                            JunQty = Convert.ToDecimal(dr["JunQty"].ToString()),
                                                            JulQty = Convert.ToDecimal(dr["JulQty"].ToString()),
                                                            AugQty = Convert.ToDecimal(dr["AugQty"].ToString()),
                                                            SepQty = Convert.ToDecimal(dr["SepQty"].ToString()),
                                                            OctQty = Convert.ToDecimal(dr["OctQty"].ToString()),
                                                            NovQty = Convert.ToDecimal(dr["NovQty"].ToString()),
                                                            DecQty = Convert.ToDecimal(dr["DecQty"].ToString()),
                                                            JanQty = Convert.ToDecimal(dr["JanQty"].ToString()),
                                                            FebQty = Convert.ToDecimal(dr["FebQty"].ToString()),
                                                            MarQty = Convert.ToDecimal(dr["MarQty"].ToString()),
                                                        }).ToList();
                    }
                }
                else if (ReportType.ToString().ToUpper() == "CUSTOMERITEMWISEMONTHLYDATA") //done&working
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.SaleBillRegisterDetail = (from DataRow dr in oDataSet.Tables[0].Rows
                                                        select new SaleBillRegisterDetail
                                                        {
                                                            CustomerName = string.IsNullOrEmpty(dr["CustomerName"].ToString()) ? "" : dr["CustomerName"].ToString(),
                                                            PartCode = string.IsNullOrEmpty(dr["PartCode"].ToString()) ? "" : dr["PartCode"].ToString(),
                                                            ItemName = string.IsNullOrEmpty(dr["ItemName"].ToString()) ? "" : dr["ItemName"].ToString(),
                                                            Unit = string.IsNullOrEmpty(dr["Unit"].ToString()) ? "" : dr["Unit"].ToString(),

                                                            MonthName = string.IsNullOrEmpty(dr["MonthName"].ToString()) ? "" : dr["MonthName"].ToString(),
                                                            TotalItemAmt = Convert.ToDecimal(dr["TotalAmount"].ToString()),
                                                            TotalBillQty = Convert.ToDecimal(dr["TotalQty"].ToString()),

                                                        }).ToList();
                    }
                }
                else
                {
                    try
                    {
                        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = oDataSet.Tables[0];

                            List<SaleBillRegisterDetail> list = new List<SaleBillRegisterDetail>();

                            foreach (DataRow dr in dt.Rows)
                            {
                                SaleBillRegisterDetail obj = new SaleBillRegisterDetail();
                                obj.DynamicColumns = new Dictionary<string, string>();

                                foreach (DataColumn col in dt.Columns)
                                {
                                    string colName = col.ColumnName;
                                    string colValue = dr[col].ToString();

                                    // Populate dynamic dictionary
                                    obj.DynamicColumns[colName] = colValue;
                                }

                                list.Add(obj);
                            }

                            model.SaleBillRegisterDetail = list;
                            model.DynamicHeaders = dt.Columns
                                                     .Cast<DataColumn>()
                                                     .Select(c => c.ColumnName)
                                                     .ToList();
                        }
                    }
                    catch (Exception ex) { throw ex; }
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

        public async Task<ResponseResult> FillDocumentList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDocumentList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSaleBillList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSaleBillList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemNamePartcodeList(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillItemNamePartcodeList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSONO(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSONOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSchNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSchNOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillHSNNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillHSNNOList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillGSTNo(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGSTList"));
                SqlParams.Add(new SqlParameter("@fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@todate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportSalebillRegister", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
    }
}
