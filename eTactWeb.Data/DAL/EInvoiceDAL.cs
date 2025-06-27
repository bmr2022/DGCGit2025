using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;
using Common = eTactWeb.Data.Common;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Data;
using System.Data.SqlClient;

using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic; // For Dictionary
using System.Net.Http; // For HttpClient
using System.Threading.Tasks;
//using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;
//using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
using System.Data;
using System.Security.Cryptography.X509Certificates; // For async/await



using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Text.Json.Nodes;
using System;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace eTactWeb.Data.DAL
{

    public class EInvoiceDAL
    {
        private readonly IConfiguration _configuration;

        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString;
        private readonly ConnectionStringService _connectionStringService;
        private readonly IHttpClientFactory _httpClientFactory;

        public string barcodeVal;
        public string irnNo;
        public string invoicenumber;
        public int yearcodenumber;
        public string token;
        public string connectionString = string.Empty;

        public EInvoiceDAL(IConfiguration configuration, IDataLogic iDataLogic, IHttpClientFactory httpClientFactory, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _httpClientFactory = httpClientFactory; // Fix: Assign the IHttpClientFactory instance passed in the constructor to the field.  
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> CheckDuplicateIRN(int entryId, string invoiceNo, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@ENtryID", entryId));
                SqlParams.Add(new SqlParameter("@INVNO", invoiceNo));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("chkForDuplicateIRNNO", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        private async Task<ResponseResult> GetInvoicePrefix(int yearCode, int entryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                SqlParams.Add(new SqlParameter("@EntryId", entryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SELECT Excise_Amt_Word FROM SaleBillMain WHERE SaleBillYearCode = @YearCode AND entry_id = @EntryId", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;


        }

        //private async Task<ResponseResult> GetInvoiceData(string invoiceNo, int yearCode)
        //{
        //    var _ResponseResult = new ResponseResult();
        //    try
        //    {
        //        var SqlParams = new List<dynamic>();
        //        SqlParams.Add(new SqlParameter("@SaleBillNo1", invoiceNo));
        //        SqlParams.Add(new SqlParameter("@SaleBillNo2", invoiceNo));
        //        SqlParams.Add(new SqlParameter("@SaleSaleBillYearCode", yearCode));
        //        _ResponseResult = await _IDataLogic.ExecuteDataTable("getIRNData", SqlParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic Error = new ExpandoObject();
        //        Error.Message = ex.Message;
        //        Error.Source = ex.Source;
        //    }

        //    return _ResponseResult;


        //}
        public async Task<ResponseResult> BuildInvoiceDetails(DataTable dataTable, string invoice, string saleBillType, string customerPartCode, string token)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var invoiceDetails = new Dictionary<string, object>
        {
            { "access_token", token },
            { "user_gstin", GetSellerGstin(dataTable) },
            { "data_source", "erp" },
            { "transaction_details", GetTransactionDetails(saleBillType) },
            { "document_details", GetDocumentDetails(invoice, dataTable) },
            { "seller_details", GetSellerDetails(dataTable) },
            { "buyer_details", GetBuyerDetails(saleBillType, dataTable) },
            { "ship_details", GetShippingDetails(saleBillType, dataTable) },
            { "value_details", GetValueDetails(dataTable) },
            { "item_list", GetItemList(dataTable, saleBillType) }
        };

                _ResponseResult.Result = invoiceDetails;
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        private string GetSellerGstin(DataTable dataTable)
        {
            return dataTable.Rows[0]["sellergstin"].ToString();
        }

        private Dictionary<string, object> GetTransactionDetails(string SaleBillType)
        {
            var supplyType = SaleBillType.ToUpper() switch
            {
                "EXPORT WITH PAYMENT" => "EXPWP",
                "WITH SEZ PAYMENT" => "SEZWP",
                "SEZ WITHOUT PAYMENT" => "SEZWOP",
                "EXPORT WITHOUT PAYMENT" => "EXPWOP",
                "DEEMED EXPORT" => "DEXP",
                "BUSINESS TO BUSINESS" => "B2B",
                _ => "B2B"
            };

            //        return new Dictionary<string, object>
            //{
            //    { "supply_type", supplyType },
            //    { "charge_type", "N" },
            //    { "igst_on_intra", "N" },
            //    { "ecommerce_gstin", "" }
            //};

            var details = new Dictionary<string, object>
    {
        { "supply_type", supplyType },
        { "charge_type", "N" },
        { "igst_on_intra", "N" }
    };

            // Include ecommerce_gstin only for domestic transactions like B2B
            bool isDomestic = supplyType == "B2B" || supplyType == "DEXP";
            if (isDomestic)
            {
                details.Add("ecommerce_gstin", "");
            }

            return details;
        }

        private Dictionary<string, object> GetDocumentDetails(string invoice, DataTable dataTable)
        {
            return new Dictionary<string, object>
    {
        { "document_type", "INV" },
        { "document_number", invoice },
        { "document_date", dataTable.Rows[0]["invoice_date"].ToString() }
    };
        }

        private Dictionary<string, object> GetSellerDetails(DataTable dataTable)
        {
            var row = dataTable.Rows[0];
            return new Dictionary<string, object>
    {
        { "gstin", row["sellergstin"].ToString() },
        { "legal_name", row["sellername"].ToString() },
        { "address1", row["selleraddress"].ToString() },
        { "location", row["sellerlocation"].ToString() },
        { "pincode", row["sellerpincode"].ToString() },
        { "state_code", row["sellerstate"].ToString() }
    };
        }

        //    private Dictionary<string, object> GetBuyerDetails(string customerPartCode, DataTable dataTable)
        //    {
        //        var row = dataTable.Rows[0];
        //        var gstin = customerPartCode.ToUpper() == "EXPORT WITH PAYMENT" ? "URP" : row["buyergstin"].ToString();
        //        var stateCode = customerPartCode.ToUpper() == "EXPORT WITH PAYMENT" ? "96" : row["buyerstate"].ToString();

        //        return new Dictionary<string, object>
        //{
        //    { "gstin", gstin },
        //    { "legal_name", row["buyername"].ToString() },
        //    { "address1", row["buyeraddress"].ToString() },
        //    { "location", row["buyerlocation"].ToString() },
        //    { "pincode", row["buyerpincode"].ToString() },
        //    { "place_of_supply", row["buyerstatecode"].ToString() },
        //    { "state_code", stateCode }
        //};
        //}
        private Dictionary<string, object> GetBuyerDetails(string saleBillType, DataTable dataTable)
        {
            var row = dataTable.Rows[0];
            var upperType = saleBillType.ToUpper();

            // Fix: Correctly define the isExport variable and ensure proper syntax
            var isExport = "EXPORT WITH PAYMENT".Equals(upperType) || "EXPORT WITHOUT PAYMENT".Equals(upperType);

            var buyerDetails = new Dictionary<string, object>
            {
                { "legal_name", row["buyername"].ToString() },
                { "address1", string.IsNullOrWhiteSpace(row["buyeraddress"].ToString()) ? "NA" : row["buyeraddress"].ToString() },
                { "location", row["buyerlocation"].ToString() },
                { "pincode", row["buyerpincode"].ToString() },
                { "place_of_supply", row["buyerstatecode"].ToString() },
                { "state_code", row["buyerstate"].ToString() }
            };

            if (isExport)
            {
                buyerDetails.Add("gstin", "URP");
            }
            else
            {
                var gstin = row["buyergstin"].ToString();
                if (!string.IsNullOrWhiteSpace(gstin))
                {
                    buyerDetails.Add("gstin", gstin);
                }
                else
                {
                    throw new Exception("GSTIN is required for domestic transactions.");
                }
            }

            return buyerDetails;
        }


        private Dictionary<string, object> GetShippingDetails(string customerPartCode, DataTable dataTable)
        {
            // You can customize this similarly to GetBuyerDetails
            return new Dictionary<string, object>();
        }

        private Dictionary<string, object> GetValueDetails(DataTable dataTable)
        {
            var row = dataTable.Rows[0];
            return new Dictionary<string, object>
    {
        { "total_assessable_value", row["TOTALASSVAL"].ToString() },
        { "total_cgst_value", row["TOTALCGST"].ToString() },
        { "total_sgst_value", row["TOTALSGST"].ToString() },
        { "total_igst_value", row["TOTALIGST"].ToString() },
        { "total_invoice_value", row["TOATLINVVAL"].ToString() }
    };
        }

        private List<Dictionary<string, object>> GetItemList(DataTable dataTable, string saleBillType)
        {
            var itemList = new List<Dictionary<string, object>>();
            int ni = 1;

            foreach (DataRow row in dataTable.Rows)
            {
                var item = new Dictionary<string, object>
        {
            { "total_item_value", row["TOTALITEMVAL"].ToString() },
            { "igst_amount", row["IGSTAMT"].ToString() },
            { "gst_rate", row["GSTRATE"].ToString() },
            { "assessable_value", row["TOTALASSVAL"].ToString() },
            { "total_amount", row["TOATLINVVAL"].ToString() },
            { "unit_price", row["unitprice"].ToString() },
            { "unit", row["unitofitem"].ToString() },
            { "quantity", row["qtyofitem"].ToString() },
            { "hsn_code", row["hsnno"].ToString() },
            {
                "is_service", row["IsService"].ToString().ToUpper() == "J" ? "Y" : "N"
            },
            { "product_description", row["proddesc"].ToString() },
            { "item_serial_number", ni }
        };
                ni++;
                itemList.Add(item);
            }

            return itemList;
        }


        public async Task<ResponseResult> GetInvoiceDataAsync(string invoiceNo, int yearCode ,string flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@SaleBillNo1", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo2", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleSaleBillYearCode", yearCode));
                SqlParams.Add(new SqlParameter("@flag", flag));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPIRNEInvoiceAndEwayBillData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetInvoiceDataCancelAsync(string invoiceNo, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@SaleBillNo1", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo2", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleSaleBillYearCode", yearCode));
                SqlParams.Add(new SqlParameter("@flag", "GETIRNNO"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPIRNEInvoiceAndEwayBillData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<string> PostCancelIRNAsync(Dictionary<string, object> dictData)
        {
            var urlToPost = "https://pro.mastersindia.co/ewayBillCancel";
            using var client = new HttpClient();

            try
            {
                var jsonData = JsonConvert.SerializeObject(dictData, Formatting.Indented);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(urlToPost, content);
                response.EnsureSuccessStatusCode();

                var resString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(resString);

                var ewayBillNo = json.SelectToken("results.message.ewayBillNo")?.ToString();
                var cancelDate = json.SelectToken("results.message.cancelDate")?.ToString();

                var result = new
                {
                    EwayBillNo = ewayBillNo,
                    CancelDate = cancelDate,
                    RawJson = resString
                };

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error cancelling eway bill: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<ResponseResult> CancelEInvoiceAsync(string ewaybillno, string gstin, string SaleBillNo, int SaleBillYearCode,string  token)
        {
            var result = new ResponseResult();
            try
            {
              

                var dictData1 = new Dictionary<string, object>
                    {
                        { "access_token", token },
                        { "userGstin", gstin },
                        { "eway_bill_number", ewaybillno },
                        { "reason_of_cancel","Others" },
                        { "cancel_remarks", "Cancelled the order" },
                        { "data_source", "ERP" }
                    };
                string InvoiceStatus = await PostCancelIRNAsync(dictData1);
                var resultObj = JsonConvert.DeserializeObject<dynamic>(InvoiceStatus);
                string cancelledEwayBillNo = resultObj.EwayBillNo;
                string cancelledDate = resultObj.CancelDate;

                if (string.IsNullOrWhiteSpace(cancelledEwayBillNo) || string.IsNullOrWhiteSpace(cancelledDate))
                {
                    result.StatusText = "Error";
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Result = $"EWay Bill Cancel Failed. Raw Response: {InvoiceStatus}";
                    return result;
                }
          
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "UpdateIRNDetails"));
                SqlParams.Add(new SqlParameter("@InvoiceNo", SaleBillNo));
                SqlParams.Add(new SqlParameter("@cancelledirnno", cancelledEwayBillNo)); // Ensure accountCode is of type int
                SqlParams.Add(new SqlParameter("@cancelleddate", cancelledDate)); // Ensure accountCode is of type int
                SqlParams.Add(new SqlParameter("@yearcode", SaleBillYearCode));
                await _IDataLogic.ExecuteDataTable("SPEInvoiceIRNdetail", SqlParams);
                result.Result = "EInvoice Cancelled Successfully";
                return result;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public async Task<ResponseResult> UpdateQRCodeImageAsync(string invoiceNo, int yearCode, byte[] image)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@InvoiceNo", invoiceNo));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                SqlParams.Add(new SqlParameter("@img", image));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("getIRNData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<string> PostDataAsync(
     Dictionary<string, object> dictData,
     string invoicenumber,
     int yearcodenumber,
     string transporterName,
     string vehicleNo,
     string distanceKM,
     int EntrybyId,
     string MachineName,
     string fromname,
     string generateEway, string flag)
        {
            try
            {
                string EwbNo = "", EwbDate = "", ewbUrl = "", EwbValidTill = "", ewayStatus = "";
                var client = _httpClientFactory.CreateClient();

                string urlToPost1 = "https://pro.mastersindia.co/generateEinvoice";
                var requestContent = new StringContent(JsonConvert.SerializeObject(dictData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(urlToPost1, requestContent);
                token = dictData.ContainsKey("access_token") ? dictData["access_token"]?.ToString() : null;

                if (!response.IsSuccessStatusCode)
                    return $"Invoice API Error: {response.StatusCode}";

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                irnNo = json.SelectToken("results.message.Irn")?.ToString();
                barcodeVal = json.SelectToken("results.message.SignedQRCode")?.ToString();
                var ackNo = json.SelectToken("results.message.AckNo")?.ToString();
                var ackDate = json.SelectToken("results.message.AckDt")?.ToString();

                if (string.IsNullOrEmpty(irnNo) || string.IsNullOrEmpty(barcodeVal))
                    return "Failed to generate invoice: missing IRN or QR code.";

                var sellerDetails = dictData["seller_details"] as Dictionary<string, object>;
                var buyerDetails = dictData["buyer_details"] as Dictionary<string, object>;

                string pincodeFrom = sellerDetails?["pincode"]?.ToString() ?? "";
                string pincodeTo = buyerDetails?["pincode"]?.ToString() ?? "";

                var distanceResponse = await client.GetStringAsync($"http://pro.mastersindia.co/distance?access_token={token}&fromPincode={pincodeFrom}&toPincode={pincodeTo}");
                var distanceJson = JObject.Parse(distanceResponse);
                var distanceValue = distanceJson.SelectToken("results.distance")?.ToString();

                var response1 = await GetInvoiceDataAsync(invoicenumber, yearcodenumber, flag);
                var dataTable = response1.Result as DataTable;
                if (dataTable.Rows.Count == 0) return "Invoice data not found";

                var row = dataTable.Rows[0];
                int entryid = row.Table.Columns.Contains("entry_id") && row["entry_id"] != DBNull.Value ? Convert.ToInt32(row["entry_id"]) : 0;
                int accountCode = row.Table.Columns.Contains("account_code") && row["account_code"] != DBNull.Value ? Convert.ToInt32(row["account_code"]) : 0;

                string transporterGst = await GetTransporterGSTAsync(transporterName);

                if (generateEway == "EwayAndEInvoice")
                {
                    if (string.IsNullOrEmpty(transporterGst) && string.IsNullOrEmpty(vehicleNo))
                        return "Transporter Name and Vehicle No cannot be empty at the same time. Please provide at least one of them.";
                }

                var dictData19 = new Dictionary<string, object>
           {
               { "access_token", token },
               { "user_gstin", row["sellergstin"].ToString() },
               { "irn", irnNo },
               { "data_source", "ERP" },
               {
                   "dispatch_details", new Dictionary<string, object>
                   {
                       { "company_name", row["sellername"].ToString() },
                       { "address1", row["selleraddress"].ToString() },
                       { "address2", "" },
                       { "location", row["sellerlocation"].ToString() },
                       { "pincode", row["sellerpincode"].ToString() },
                       { "state_code", row["sellerstate"].ToString() }
                   }
               }
           };

                if (!string.IsNullOrEmpty(transporterGst))
                    dictData19.Add("transporter_id", transporterGst);

                if (!string.IsNullOrEmpty(vehicleNo))
                {
                    dictData19.Add("vehicle_number", vehicleNo);
                    dictData19.Add("transportation_mode", "1"); // Consider using constants
                    dictData19.Add("vehicle_type", "R");        // Consider using constants
                    dictData19.Add("transporter_name", transporterName);
                    dictData19.Add("distance", !string.IsNullOrEmpty(distanceKM) ? distanceKM : (distanceValue ?? "0"));
                }
                string EwayresponseString = null;
                if (generateEway == "EwayAndEInvoice")
                {
                    ewayStatus = await PostData2(dictData19);
                    if (string.IsNullOrWhiteSpace(ewayStatus)) return "E-way bill response was empty.";
                    if (ewayStatus.StartsWith("Error")) return $"E-way bill generation failed: {ewayStatus}";

                    EwbNo = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("Ewaybill No:"))?.Split("Ewaybill No:").Last()?.Trim();
                    EwbDate = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("Date:"))?.Split("Date:").Last()?.Trim();
                    EwbValidTill = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("EwbValidTill:"))?.Split("EwbValidTill:").Last()?.Trim();
                    ewbUrl = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("PDF:"))?.Split("PDF:").Last()?.Trim();
                    EwayresponseString = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("responseString:"))?.Split("responseString:").Last()?.Trim();
                }

                // Insert final IRN + EWB data
                await InsertIRNDetailAsync(
                    barcodeVal, invoicenumber, entryid, accountCode, yearcodenumber,
                    irnNo, ackNo, ackDate, token, EwbNo, EwbDate, EwbValidTill, fromname, EntrybyId, MachineName);

                // Generate QR if only invoice was created
                if (generateEway != "EwayAndEInvoice")
                {
                    var qrResult = await GenerateQRCode(barcodeVal, invoicenumber, yearcodenumber);
                    if (qrResult != "QR code generated and saved to database successfully.")
                        return "QR generation failed.";
                }
                var resultJson = new JObject
                {
                    ["ewbUrl"] = generateEway == "EwayAndEInvoice" ? ewbUrl : barcodeVal,
                    ["rawResponse"] = new JObject
                    {
                        ["eInvoiceResponse"] = responseString,
                        ["eWayBillResponse"] = EwayresponseString // or the actual raw response string if you have it
                    }
                };

                return resultJson.ToString();
              //  return generateEway == "EwayAndEInvoice" ? ewbUrl : barcodeVal;
            }
            catch (Exception ex)
            {
                return $"Exception occurred: {ex.Message}";
            }
        }
        public async Task<string> PostDataAsyncJW(
       Dictionary<string, object> dictData,
       string invoicenumber,
       int yearcodenumber,
       string transporterName,
       string vehicleNo,
       string distanceKM,
       int EntrybyId,
       string MachineName,
       string fromname,
       string generateEway,
       string flag)
        {
            try
            {
                string EwbNo = "", EwbDate = "", ewbUrl = "", EwbValidTill = "", ewayStatus = "";
                var client = _httpClientFactory.CreateClient();

                string token = dictData.ContainsKey("access_token") ? dictData["access_token"]?.ToString() : null;
                string irnNo = null, barcodeVal = null, ackNo = "", ackDate = "";

               var sellerDetails = dictData["seller_details"] as Dictionary<string, object>;
                var buyerDetails = dictData["buyer_details"] as Dictionary<string, object>;

                string pincodeFrom = sellerDetails?["pincode"]?.ToString() ?? "";
                string pincodeTo = buyerDetails?["pincode"]?.ToString() ?? "";

                var distanceResponse = await client.GetStringAsync($"http://pro.mastersindia.co/distance?access_token={token}&fromPincode={pincodeFrom}&toPincode={pincodeTo}");
                var distanceJson = JObject.Parse(distanceResponse);
                var distanceValue = distanceJson.SelectToken("results.distance")?.ToString();

                var response1 = await GetInvoiceDataAsync(invoicenumber, yearcodenumber, flag);
                var dataTable = response1.Result as DataTable;
                if (dataTable.Rows.Count == 0) return "Invoice data not found";

                var row = dataTable.Rows[0];
                int entryid = row.Table.Columns.Contains("entry_id") && row["entry_id"] != DBNull.Value ? Convert.ToInt32(row["entry_id"]) : 0;
                int accountCode = row.Table.Columns.Contains("account_code") && row["account_code"] != DBNull.Value ? Convert.ToInt32(row["account_code"]) : 0;

                string transporterGst = await GetTransporterGSTAsync(transporterName);

                if (string.IsNullOrEmpty(transporterGst) && string.IsNullOrEmpty(vehicleNo))
                    return "Transporter Name and Vehicle No cannot be empty at the same time. Please provide at least one of them.";

                var dictData19 = new Dictionary<string, object>
                {
                    { "access_token", token },
                    { "userGstin", row["sellergstin"].ToString() },
                    { "supply_type", "Outward" },
                    { "sub_supply_type", "Others" },
                    {"sub_supply_description", "sales from other country" },
                    { "document_type", "Delivery Challan" },
                    { "document_number", invoicenumber },
                    { "document_date", row["invoice_date"].ToString() },

                    { "gstin_of_consignor", row["sellergstin"].ToString() },
                    { "legal_name_of_consignor", row["sellername"].ToString() },
                    { "address1_of_consignor", row["selleraddress"].ToString() },
                    { "address2_of_consignor", "" },
                    { "place_of_consignor", row["sellerlocation"].ToString() },
                    { "pincode_of_consignor", row["sellerpincode"].ToString() },
                    { "state_of_consignor", row["sellerstate"].ToString() },
                    { "actual_from_state_name", row["sellerstate"].ToString() },

                    { "gstin_of_consignee", buyerDetails?["gstin"]?.ToString() ?? "URP" },
                    { "legal_name_of_consignee", buyerDetails?["legal_name"]?.ToString() },
                    { "address1_of_consignee", buyerDetails?["address1"]?.ToString() },
                    { "address2_of_consignee", "" },
                    { "place_of_consignee", buyerDetails?["location"]?.ToString() },
                    { "pincode_of_consignee", buyerDetails?["pincode"]?.ToString() },
                    { "state_of_supply", buyerDetails?["state_code"]?.ToString() },
                    { "actual_to_state_name", buyerDetails?["state_code"]?.ToString() },

                    { "transaction_type", 1 },
                    { "other_value", 0 },
                    { "total_invoice_value", row["TOATLINVVAL"].ToString() },
                    { "taxable_amount", row["TOTALASSVAL"].ToString() },
                    { "cgst_amount", row["cgstamt"].ToString() },
                    { "sgst_amount", row["sgstamt"].ToString() },
                    { "igst_amount", row["igstamt"].ToString() },
                    { "cess_amount", "0" },
                    { "cess_nonadvol_value", "0" },
                    { "generate_status", "1" },
                    { "user_ref", "" },
                    { "location_code", "" },
                    { "eway_bill_status", "ABC" },
                    { "auto_print", "Y" },
                    { "email", "info@bmrinfotech.com" },
                    { "itemList", dictData["item_list"] }
                };

                if (!string.IsNullOrEmpty(transporterGst))
                {
                    dictData19["transporter_id"] = transporterGst;
                }

                if (!string.IsNullOrEmpty(vehicleNo))
                {
                    dictData19["vehicle_number"] = vehicleNo;
                    dictData19["transportation_mode"] = "1";
                    dictData19["vehicle_type"] = "R";
                    dictData19["transporter_name"] = transporterName;
                    dictData19["data_source"] = "ERP";
                    dictData19["distance"] = !string.IsNullOrEmpty(distanceKM) ? distanceKM : (distanceValue ?? "0");
                }


                string urlToPost2 = "https://pro.mastersindia.co/ewayBillsGenerate";
                using var client1 = _httpClientFactory.CreateClient();

                var jsonContent = JsonConvert.SerializeObject(dictData19, Formatting.Indented);
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client1.PostAsync(urlToPost2, requestContent);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = JObject.Parse(responseString);
                    string status = json.SelectToken("results.status")?.ToString();
                    string errorMessage = json.SelectToken("results.errorMessage")?.ToString();

                    if (status == "Failed")
                    {
                        return $"Error: {errorMessage}";
                    }

                    if (status == "Success")
                    {
                        EwbNo = json.SelectToken("results.message.ewayBillNo")?.ToString();
                        EwbDate = json.SelectToken("results.message.ewayBillDate")?.ToString();
                        ewbUrl = json.SelectToken("results.message.url")?.ToString();
                        EwbValidTill = json.SelectToken("results.message.validUpto")?.ToString();
                    }
                }

                await InsertIRNDetailAsync(
                    barcodeVal, invoicenumber, entryid, accountCode, yearcodenumber,
                    irnNo, ackNo, ackDate, token, EwbNo, CommonFunc.ParseFormattedDateTime1(EwbDate), CommonFunc.ParseFormattedDateTime1(EwbValidTill), fromname, EntrybyId, MachineName
                );
                return JsonConvert.SerializeObject(new
                {
                    ewbUrl = ewbUrl,
                    rawResponse = responseString
                });

            }
            catch (Exception ex)
            {
                return $"Exception occurred: {ex.Message}";
            }
        }

        public async Task<string> PostData2(Dictionary<string, object> dictData)
        {
            try
            {
                string urlToPost2 = "https://pro.mastersindia.co/generateEwaybillByIrn";

                using var client = _httpClientFactory.CreateClient();
                var jsonContent = JsonConvert.SerializeObject(dictData, Formatting.Indented);
                var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(urlToPost2, requestContent);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = JObject.Parse(responseString);
                    string status = json.SelectToken("results.status")?.ToString();
                    string errorMessage = json.SelectToken("results.errorMessage")?.ToString();

                    if (status == "Failed")
                    {
                        return $"Error: {errorMessage}";
                    }

                    if (status == "Success")
                    {
                        string ewbNo = json.SelectToken("results.message.EwbNo")?.ToString();
                        string ewbDt = json.SelectToken("results.message.EwbDt")?.ToString();
                        string ewbUrl = json.SelectToken("results.message.EwaybillPdf")?.ToString();
                        string EwbValidTill = json.SelectToken("results.message.EwbValidTill")?.ToString();
                        string Alert = json.SelectToken("results.message.Alert")?.ToString();
                        return $"Ewaybill No: {ewbNo}, Date: {ewbDt}, PDF: {ewbUrl},EwbValidTill:{EwbValidTill},Alert:{Alert},responseString:{responseString}";
                    }
                }
                else
                {
                    return $"Error: HTTP {response.StatusCode} - {responseString}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }

            // Ensure all code paths return a value  
            return "Unexpected error occurred.";
        }

        public async Task<string> GenerateQRCode(string barcodeValue, string invoicenumber, int yearcodenumber)
        {
            try
            {
                string tempDir = Path.GetTempPath();
                string filePath = Path.Combine(tempDir, "barcode_input.txt");
                string outputFilePath = Path.Combine(tempDir, "output.png");

                await System.IO.File.WriteAllTextAsync(filePath, barcodeValue);

                string zintPath = @"C:\Program Files (x86)\Zint\zint.exe";
                if (!System.IO.File.Exists(zintPath))
                    return "Zint executable not found.";

                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = zintPath,
                        Arguments = $"-b 58 -o \"{outputFilePath}\" -i \"{filePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                })
                {
                    process.Start();
                    await process.WaitForExitAsync();

                    if (!System.IO.File.Exists(outputFilePath))
                        return "QR code image not generated.";
                }

                byte[] img = await System.IO.File.ReadAllBytesAsync(outputFilePath);



                var _ResponseResult = new ResponseResult();
                try
                {
                    var SqlParams = new List<dynamic>();
                    SqlParams.Add(new SqlParameter("@INVNO", invoicenumber));
                    SqlParams.Add(new SqlParameter("@year_code", yearcodenumber));
                    SqlParams.Add(new SqlParameter("@img", img));
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("UpdateImgToBarcode", SqlParams);
                }
                catch (Exception ex)
                {
                    dynamic Error = new ExpandoObject();
                    Error.Message = ex.Message;
                    Error.Source = ex.Source;
                }


                //using (var connection = new SqlConnection(connectionString))
                //using (var cmd = new SqlCommand("UpdateImgToBarcode", connection))
                //{
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.Add(new SqlParameter("@INVNO", SqlDbType.BigInt) { Value = invoicenumber });
                //    cmd.Parameters.Add(new SqlParameter("@year_code", SqlDbType.BigInt) { Value = yearcodenumber });
                //    cmd.Parameters.Add(new SqlParameter("@img", SqlDbType.Image) { Value = img });

                //    await connection.OpenAsync();
                //    await cmd.ExecuteNonQueryAsync();
                //}

                return "QR code generated and saved to database successfully.";
            }
            catch (Exception ex)
            {
                return "Failed to generate QR code.";
            }
        }

    
        public async Task<string> GetTransporterGSTAsync(string transporterName)
        {
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@transportername", transporterName));
                var responseResult = await _IDataLogic.ExecuteDataTable("SELECT TransporterGSTNO FROM TransporterMaster WHERE transportername = @transportername", SqlParams);

                var dataTable = responseResult.Result as DataTable;
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    return dataTable.Rows[0]["TransporterGSTNO"].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task InsertIRNDetailAsync(
           string barcodeVal,
           string invoicenumber,
           int entryId,
           int accountCode,
           int yearcodenumber,
           string irnNo,
           string ackNo,
           string ackDate,
           string token,
           string EwbNo,
           string EwbDate,
           string EwbValidTill,
           string fromname,
           int EntrybyId,
           string MachineName)
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@imagebarcode", barcodeVal));
            SqlParams.Add(new SqlParameter("@InvoiceNo", invoicenumber));
            SqlParams.Add(new SqlParameter("@entryid", entryId)); // Ensure entryId is of type int
            SqlParams.Add(new SqlParameter("@accountcode", accountCode)); // Ensure accountCode is of type int
            SqlParams.Add(new SqlParameter("@yearcode", yearcodenumber));
            SqlParams.Add(new SqlParameter("@IRnno", irnNo));
            SqlParams.Add(new SqlParameter("@ackno", ackNo));
            SqlParams.Add(new SqlParameter("@ackdate", ackDate));
            SqlParams.Add(new SqlParameter("@Tokenno", token));
            SqlParams.Add(new SqlParameter("@INVType", "B"));
            SqlParams.Add(new SqlParameter("@EwbNo", EwbNo));
            SqlParams.Add(new SqlParameter("@EwbDt", EwbDate));
            SqlParams.Add(new SqlParameter("@EwbValidTill", EwbValidTill));
            SqlParams.Add(new SqlParameter("@fromname", fromname));
            SqlParams.Add(new SqlParameter("@EntrybyId", EntrybyId));
            SqlParams.Add(new SqlParameter("@MachineName", MachineName));

            await _IDataLogic.ExecuteDataTable("SPEInvoiceIRNdetail", SqlParams);
        }
        // Add this method to EInvoiceDAL

    }

}

