using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using static eTactWeb.DOM.Models.Common;
using Common = eTactWeb.Data.Common;
using System.Net.Http;
using Microsoft.VisualBasic;

namespace eTactWeb.Data.BLL
{
    public class EInvoiceBLL : IEinvoiceService
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly EInvoiceDAL _EInvoiceDAL;
        private readonly IHttpClientFactory _httpClientFactory;

        public EInvoiceBLL(IConfiguration configuration, IDataLogic iDataLogic, IHttpClientFactory httpClientFactory, ConnectionStringService connectionStringService)
        {
            _httpClientFactory = httpClientFactory;
            _EInvoiceDAL = new EInvoiceDAL(configuration, iDataLogic, _httpClientFactory, connectionStringService);
            _DataLogicDAL = iDataLogic;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = new HttpClient();
            var data = new Dictionary<string, object>
                {
                    { "username", "bmr_client@outlook.com" },
                    { "password", "Autotax@0214" },
                    { "client_id", "QAVEfAAvibaQdjjXPE" },
                    { "client_secret", "vXIP4fz9sZsr3Zw4wfqhtdQh" },
                    { "grant_type", "password" }
                };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://pro.mastersindia.co/oauth/access_token", content);
            var result = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(result)?.SelectToken("access_token")?.ToString();
            return token;
        }
        public async Task<ResponseResult> CheckDuplicateIRN(int entryId, string invoiceNo, int yearCode)
        {
            return await _EInvoiceDAL.CheckDuplicateIRN(entryId, invoiceNo, yearCode);
        }
       
        public async Task<ResponseResult> CreateIRNAsync(string token, int manEntryId, string manInvoiceNo, int manYearCode, string saleBillType, string customerPartCode ,string transporterName, string vehicleNo, string distanceKM,int EntrybyId, string MachineName, string fromname,string generateEway,string flag)
        {
            var result = new ResponseResult();

            try
            {
                var invoice = manInvoiceNo;

                var dataResult = await _EInvoiceDAL.GetInvoiceDataAsync(manInvoiceNo, manYearCode,flag);
                if (dataResult?.Result == null || dataResult.Result.Rows.Count == 0)
                {
                    return result;
                }

                var dataTable = dataResult.Result;
                var buildResult = await _EInvoiceDAL.BuildInvoiceDetails(dataTable, invoice, saleBillType, customerPartCode, token);
                if (buildResult?.Result == null)
                {
                    return result;
                }

                var invoiceDetails = buildResult.Result;

                string ewbUrl = await _EInvoiceDAL.PostDataAsync(invoiceDetails, invoice, manYearCode, transporterName,vehicleNo,distanceKM, EntrybyId,MachineName,fromname, generateEway,flag);
                if (!string.IsNullOrEmpty(ewbUrl))
                {
                    result.Result = ewbUrl;
                }

               
            }
            catch (Exception ex)
            {
               
            }

            return result;
        }

        public async Task<bool> GenerateQRCodeAsync(string barcodeValue, string invoiceNo, int yearCode)
        {
            try
            {
                string filePath = @"D:\pdf.txt";
                await File.WriteAllTextAsync(filePath, barcodeValue);

                string zintPath = @"C:\Program Files (x86)\Zint\zint.exe";
                string outputFilePath = @"D:\output.png";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = zintPath,
                        Arguments = $"-b 58 -o {outputFilePath} -i {filePath}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                var imgBytes = await File.ReadAllBytesAsync(outputFilePath);
                await _EInvoiceDAL.UpdateQRCodeImageAsync(invoiceNo, yearCode, imgBytes);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ResponseResult> GenerateInvoiceAsync(EInvoiceItemModel input)
        {
            // Implement logic for generating invoice
            return new ResponseResult
            {
                StatusCode = HttpStatusCode.OK,
                StatusText = "Invoice generated successfully"
            };
        }

        public async Task<ResponseResult> GenerateEwayBillAsync(string token, string irnNo, int invoiceNo, int yearCode)
        {
            // Implement logic for generating e-way bill
            return new ResponseResult
            {
                StatusCode = HttpStatusCode.OK,
                StatusText = "E-way bill generated successfully"
            };
        }

        async Task<ResponseResult> IEinvoiceService.GenerateQRCodeAsync(string barcodeValue, int invoiceNo, int yearCode)
        {
            // Implement logic for generating QR code
            return new ResponseResult
            {
                StatusCode = HttpStatusCode.OK,
                StatusText = "QR code generated successfully"
            };
        }

      
    }
}
