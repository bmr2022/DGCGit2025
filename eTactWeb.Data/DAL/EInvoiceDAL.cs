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


        public async Task<ResponseResult> GetInvoiceDataAsync(string invoiceNo, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@SaleBillNo1", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleBillNo2", invoiceNo));
                SqlParams.Add(new SqlParameter("@SaleSaleBillYearCode", yearCode));
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
        public async Task<ResponseResult> GetInvoiceDataAsync(int yearCode, int entryId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                SqlParams.Add(new SqlParameter("@EntryId", entryId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SELECT Excise_Amt_Word FROM Sale_Bill_Main WHERE year_code = @YearCode AND entry_id = @EntryId", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
          
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

        public async Task<string> PostDataAsync(Dictionary<string, object> dictData, string invoicenumber, int yearcodenumber, string transporterName, string vehicleNo, string distanceKM,int EntrybyId,string MachineName,string fromname,string generateEway)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                string urlToPost1 = "https://pro.mastersindia.co/generateEinvoice";
                var requestContent = new StringContent(JsonConvert.SerializeObject(dictData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(urlToPost1, requestContent);
                token = dictData.ContainsKey("access_token") ? dictData["access_token"]?.ToString() : null;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                irnNo = json.SelectToken("results.message.Irn")?.ToString();
                barcodeVal = json.SelectToken("results.message.SignedQRCode")?.ToString();
                var ackNo = json.SelectToken("results.message.AckNo")?.ToString();
                var ackDate = json.SelectToken("results.message.AckDt")?.ToString();

                var sellerDetails = dictData.ContainsKey("seller_details") ? dictData["seller_details"] as Dictionary<string, object> : null;
                var buyerDetails = dictData.ContainsKey("buyer_details") ? dictData["buyer_details"] as Dictionary<string, object> : null;

                string pincodeFrom = sellerDetails != null && sellerDetails.ContainsKey("pincode") ? sellerDetails["pincode"].ToString() : "";
                string pincodeTo = buyerDetails != null && buyerDetails.ContainsKey("pincode") ? buyerDetails["pincode"].ToString() : "";

                var distanceResponse = await client.GetStringAsync($"http://pro.mastersindia.co/distance?access_token={token}&fromPincode={pincodeFrom}&toPincode={pincodeTo}");
                var distanceJson = JObject.Parse(distanceResponse);
                var distanceValue = distanceJson.SelectToken("results.distance")?.ToString();

                ResponseResult response1 = await GetInvoiceDataAsync(invoicenumber, yearcodenumber);
                DataTable dataTable = response1.Result as DataTable;
                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                var row = dataTable.Rows[0];
                int entryid = row.Table.Columns.Contains("entry_id") && row["entry_id"] != DBNull.Value? Convert.ToInt32(row["entry_id"]): 0;
                int accountCode = row.Table.Columns.Contains("account_code") && row["account_code"] != DBNull.Value  ? Convert.ToInt32(row["account_code"]): 0;

                var AccountCode = row["account_code"];
                string transporterGst = await GetTransporterGSTAsync(transporterName);

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

                if (!string.IsNullOrEmpty(transporterGst) && !string.IsNullOrEmpty(vehicleNo))
                {
                    dictData19.Add("transporter_id", transporterGst);
                    dictData19.Add("vehicle_number", vehicleNo);
                    dictData19.Add("transportation_mode", "1");
                    dictData19.Add("vehicle_type", "R");
                    dictData19.Add("transporter_name", transporterName);
                    dictData19.Add("distance", !string.IsNullOrEmpty(distanceKM) ? distanceKM : (distanceValue ?? "0"));
                }
                else if (!string.IsNullOrEmpty(transporterGst) && string.IsNullOrEmpty(vehicleNo))
                {
                    dictData19.Add("transporter_id", transporterGst);
                }
                else if (string.IsNullOrEmpty(transporterGst) && !string.IsNullOrEmpty(vehicleNo))
                {
                    dictData19.Add("vehicle_number", vehicleNo);
                    dictData19.Add("transportation_mode", "1");
                    dictData19.Add("vehicle_type", "R");
                    dictData19.Add("transporter_name", transporterName);
                    dictData19.Add("distance", !string.IsNullOrEmpty(distanceKM) ? distanceKM : "0");
                }
                string EwbNo = "", EwbDate = "", ewbUrl = "", EwbValidTill = "", ewayStatus="";

                if (generateEway == "true")
                {
                     ewayStatus = await PostData2(dictData19);
                    if (string.IsNullOrEmpty(ewayStatus)) return null;

                     EwbNo = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("Ewaybill No:"))?.Split("Ewaybill No:").Last()?.Trim();
                    EwbDate = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("Date:"))?.Split("Date:").Last()?.Trim();
                    EwbValidTill = ewayStatus.Split(',').FirstOrDefault(x => x.Contains("EwbValidTill:"))?.Split("EwbValidTill:").Last()?.Trim();
                }

                
                await InsertIRNDetailAsync(
                    barcodeVal, invoicenumber,entryid,accountCode, yearcodenumber,
                    irnNo, ackNo, ackDate, token, EwbNo, EwbDate, EwbValidTill, fromname, EntrybyId, MachineName);

                 ewbUrl = ewayStatus.Split(',')
                        .FirstOrDefault(x => x.Contains("PDF:"))
                        ?.Split("PDF:").Last()
                        ?.Trim();
                var qrResult = await GenerateQRCode(barcodeVal, invoicenumber, yearcodenumber);
                if (qrResult != "QR code generated and saved to database successfully.")
                {
                    return null;
                }
                if (generateEway == "true")
                {
                    return ewbUrl;
                }
                else
                {
                    return barcodeVal;
                }
                    
            }
            catch (Exception ex)
            {
                return "url not found";
            }
        }
    
        public async Task<string> GenerateQRCode(string barcodeValue,string invoicenumber,int yearcodenumber)
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
                    string ewbNo = json.SelectToken("results.message.EwbNo")?.ToString();
                    string ewbDt = json.SelectToken("results.message.EwbDt")?.ToString();
                    string ewbUrl = json.SelectToken("results.message.EwaybillPdf")?.ToString();
                    string EwbValidTill = json.SelectToken("results.message.EwbValidTill")?.ToString();
                    string Alert = json.SelectToken("results.message.Alert")?.ToString();
                    return $"Ewaybill No: {ewbNo}, Date: {ewbDt}, PDF: {ewbUrl},EwbValidTill:{EwbValidTill},Alert:{Alert}";
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

            await _IDataLogic.ExecuteDataTable("InsertIRNdetail", SqlParams);
        }
        // Add this method to EInvoiceDAL
       
    }

}

