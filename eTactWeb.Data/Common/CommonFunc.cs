using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using eTactWeb.Services.Interface;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Http;

namespace eTactWeb.Data.Common;

public static class CommonFunc
{
    public static List<T> ConvertDataTable<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    public static List<T> ConvertDataTable2<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem2<T>(row);
            data.Add(item);
        }
        return data;
    }
    public static DataTable ConvertListToTable<T>(IList<T> list)
    {
        Type entityType = typeof(T);
        DataTable table = new DataTable();
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

        foreach (PropertyDescriptor prop in properties)
        {
            //if (prop.Name != "PartCode" && prop.Name != "SeqNo" && prop.Name != "Active" && prop.Name != "Browser" && prop.Name != "CreatedBy" && prop.Name != "CreatedOn"
            //    && prop.Name != "ID" && prop.Name != "IPAddress" && prop.Name != "Mode" && prop.Name != "UpdatedBy" && prop.Name != "UpdatedOn")
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (T item in list)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }
        return table;
    }
    public static string ConvertToDesiredFormat(string inputDate)
    {
        string[] dateFormats = {
            "dd/MM/yyyy",
            "MM-dd-yyyy",
            "yyyy.MM.dd",
            "MMMM d, yyyy",
             "dd.MM.yy","dd.MM.yyyy", "dd/M/yy", "d.M.yy", "d/M/yy", "yyyy-MM-dd", "MM/dd/yyyy","dd/MMM/yyyy",
             "dd-MM-yyyy HH:mm:ss"
        };

        if (DateTime.TryParseExact(inputDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            string outputDateString = parsedDate.ToString("yyyy/MM/dd");
            return outputDateString;
        }
        else
        {
            return "Invalid Date";
        }
    }
    //public static string ParseFormattedDate(string dateString)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dateString))
    //        {
    //            return string.Empty;
    //        }
    //        DateTime date;
    //        string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy", "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "dd/MMM/yyyy", "dd/MMM/yyyy HH:mm:ss", "HH:mm:ss", "" };
    //        DateTime parsedDate = DateTime.ParseExact(dateString, "dd/MM/yyyy", null);
    //        string formattedDate = parsedDate.ToString("yyyy-MM-dd");
    //        if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
    //        {
    //            return formattedDate;
    //        }
    //        else
    //        {
    //            return string.Empty;
    //        }
    //    }catch(Exception ex)
    //    {
    //        throw;
    //    }
    //}
    public static DateTime ParseDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }

        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        else
        {
            return DateTime.Parse(dateString);
        }
    }

    public static  string FormatDateOrEmpty(string? date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return string.Empty;

        if (DateTime.TryParse(date, out DateTime parsedDate))
        {
            if (parsedDate.Date == new DateTime(1900, 1, 1) || parsedDate.Date == DateTime.MinValue)
                return string.Empty;

            return ParseFormattedDate(date);
        }

        return string.Empty; // In case parsing fails
    }


    public static string ParseFormattedDateForView(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return string.Empty; // Return empty if the input is null or whitespace
        }

        try
        {
            DateTime date;
            string[] formats =
            {
                           "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy", "dd-MMM-yyyy",
    "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "MMM dd, yyyy", "dd-MMM-yyyy HH:mm",
    "dd/MMM/yyyy", "dd/MMM/yyyy HH:mm:ss", "dd/MMM/yyyy hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss",
    "MM/dd/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "d-M-yyyy hh:mm:ss tt", "d-M-yyyy",
    "dd/M/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss tt", "dd-MMM-yyyy hh:mm:ss", "d/M/yyyy h:mm:ss tt",
    "dd/MM/yyyy HH:mm", "MM-dd-yyyy hh:mm:ss a", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
    "yyyy-MM-ddTHH:mm:ss.SSS", "yyyy-MM-ddTHH:mm:ss.SSSZ", "yyyy-MM-ddTHH:mm:ss+HH:mm",
    "EEEE, MMMM dd, yyyy", "EEE, dd MMM yyyy", "EEEE dd/MM/yyyy", "EEEE dd-MMM-yyyy",
    "hh:mm:ss a", "dd-MMM-yyyy hh:mm:ss tt", "HH:mm", "hh:mm a", "dd/MMM/yyyy hh:mm:ss",
    "dd/MM/yyyy hh:mm:ss tt", "dd-MM-yyyy hh:mm:ss", "dd-MM-yyyy HH:mm:ss", "d-M-yy hh:mm:ss",
    "d-M-yy HH:mm:ss", "dd-M-yy hh:mm:ss", "dd-M-yy HH:mm:ss", "dd/MMM/yyyy HH.mm.ss tt",
    "dd/MMM/yyyy hh.mm.ss tt", "dd/MMM/yyyy h.mm.ss tt", "MM/dd/yyyy HH:mm:ss tt",
    "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "M/dd/yyyy",
     "yyyy/MM/dd",                       // 2025/07/07
    "yyyy.MM.dd",                       // 2025.07.07
    "dd.MM.yyyy",                       // 07.07.2025
    "dd.MM.yyyy HH:mm:ss",             // 07.07.2025 14:00:00
    "yyyy.MM.dd HH:mm:ss",             // 2025.07.07 14:00:00 
    "dd-MM-yyyyTHH:mm:ss",             // e.g., 07-07-2025T14:00:00
    "yyyy/MM/dd hh:mm:ss tt",          // 2025/07/07 03:00:00 PM   
    "yyyy-MM-dd hh:mm:ss tt",          // 2025-07-07 03:15:00 PM
    "yyyy-MM-dd hh:mm tt",             // 2025-07-07 03:15 PM
    "yyyy-MM-dd HH:mm",                // 2025-07-07 15:15
    "dd-MM-yyyy hh:mm tt",             // 07-07-2025 03:15 PM
    "dd/MM/yyyy hh:mm tt",             // 07/07/2025 03:15 PM
    "MM/dd/yy hh.mm.ss tt",
    "M/d/yy hh:mm:ss tt",
"M/dd/yyyy hh:mm:ss tt",
"MM/d/yyyy hh:mm:ss tt"
        };

            // Try parsing the date string against all the formats
            if (DateTime.TryParseExact(dateString, formats,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out date))
            {
                // Successfully parsed; return the date in "yyyy-MM-dd" format
                return date.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex)
        {
            // Log the exception if needed (e.g., to a file, database, or logging framework)
            Console.WriteLine($"Error parsing date: {ex.Message}");
        }

        // Return empty string if parsing failed
        return dateString.ToString();
    }



    public static string ParseFormattedDate(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return string.Empty; // Return empty if the input is null or whitespace
        }

        try
        {
            DateTime date;
            string[] formats =
            {
                          "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy", "dd-MMM-yyyy",
    "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "MMM dd, yyyy", "dd-MMM-yyyy HH:mm",
    "dd/MMM/yyyy", "dd/MMM/yyyy HH:mm:ss", "dd/MMM/yyyy hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss",
    "MM/dd/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "d-M-yyyy hh:mm:ss tt", "d-M-yyyy",
    "dd/M/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss tt", "dd-MMM-yyyy hh:mm:ss", "d/M/yyyy h:mm:ss tt",
    "dd/MM/yyyy HH:mm", "MM-dd-yyyy hh:mm:ss a", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
    "yyyy-MM-ddTHH:mm:ss.SSS", "yyyy-MM-ddTHH:mm:ss.SSSZ", "yyyy-MM-ddTHH:mm:ss+HH:mm",
    "EEEE, MMMM dd, yyyy", "EEE, dd MMM yyyy", "EEEE dd/MM/yyyy", "EEEE dd-MMM-yyyy",
    "hh:mm:ss a", "dd-MMM-yyyy hh:mm:ss tt", "HH:mm", "hh:mm a", "dd/MMM/yyyy hh:mm:ss",
    "dd/MM/yyyy hh:mm:ss tt", "dd-MM-yyyy hh:mm:ss", "dd-MM-yyyy HH:mm:ss", "d-M-yy hh:mm:ss",
    "d-M-yy HH:mm:ss", "dd-M-yy hh:mm:ss", "dd-M-yy HH:mm:ss", "dd/MMM/yyyy HH.mm.ss tt",
    "dd/MMM/yyyy hh.mm.ss tt", "dd/MMM/yyyy h.mm.ss tt", "MM/dd/yyyy HH:mm:ss tt",
    "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "M/dd/yyyy",
     "yyyy/MM/dd",                       // 2025/07/07
    "yyyy.MM.dd",                       // 2025.07.07
    "dd.MM.yyyy",                       // 07.07.2025
    "dd.MM.yyyy HH:mm:ss",             // 07.07.2025 14:00:00
    "yyyy.MM.dd HH:mm:ss",             // 2025.07.07 14:00:00 
    "dd-MM-yyyyTHH:mm:ss",             // e.g., 07-07-2025T14:00:00
    "yyyy/MM/dd hh:mm:ss tt",          // 2025/07/07 03:00:00 PM   
    "yyyy-MM-dd hh:mm:ss tt",          // 2025-07-07 03:15:00 PM
    "yyyy-MM-dd hh:mm tt",             // 2025-07-07 03:15 PM
    "yyyy-MM-dd HH:mm",                // 2025-07-07 15:15
    "dd-MM-yyyy hh:mm tt",             // 07-07-2025 03:15 PM
    "dd/MM/yyyy hh:mm tt",             // 07/07/2025 03:15 PM
    "MM/dd/yy hh.mm.ss tt" ,
    "M/d/yyyy h:mm:ss tt",     // for "7/1/2025 12:00:00 AM"
"M/d/yy h:mm:ss tt",       // for "7/1/25 12:00:00 AM"
"MM/dd/yyyy h:mm tt",      // for "07/01/2025 12:00 AM"
"M/d/yyyy h:mm tt",        // for "7/1/2025 12:00 AM"
"M/d/yy h:mm tt",          // for "7/1/25 12:00 AM"
"yyyy-MM-dd HH:mm:ss.fff"
        };

            // Try parsing the date string against all the formats
            if (DateTime.TryParseExact(dateString, formats,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out date))
            {
                // Successfully parsed; return the date in "yyyy-MM-dd" format
                return date.ToString("dd/MMM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return date.ToString("dd/MMM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex)
        {
            // Log the exception if needed (e.g., to a file, database, or logging framework)
            return dateString.ToString();
            Console.WriteLine($"Error parsing date: {ex.Message}");
        }
      
        // Return empty string if parsing failed

    }
    public static string FormatToDDMMYYYY(string inputDate)
    {
        if (string.IsNullOrWhiteSpace(inputDate))
            return "";

        DateTime parsedDate;

        // Add all common and known date formats here
        string[] formats = {
        "d/M/yyyy", "dd/MM/yyyy",
        "M/d/yyyy", "MM/dd/yyyy",
        "yyyy-M-d", "yyyy-MM-dd", "yyyy/MM/dd",
        "d-M-yyyy", "dd-MM-yyyy",
        "d.MM.yyyy", "dd.MM.yyyy",
        "M.d.yyyy", "MM.dd.yyyy",
        "d/M/yy", "dd/MM/yy",
        "M/d/yy", "MM/dd/yy",
        "yyyyMMdd",
        "dd MMM yyyy", "d MMM yyyy",
        "dd MMMM yyyy", "d MMMM yyyy",
        "dd/MMM/yyyy", "d/MMM/yyyy",         // Added format: 08/Apr/2025
        "dd-MMM-yyyy", "d-MMM-yyyy"  ,        // Also allow: 08-Apr-2025

    };

        // Try parsing with specific formats
        bool parsed = DateTime.TryParseExact(
            inputDate,
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate
        );

        // Fallback to flexible parse if no exact match
        if (!parsed)
        {
            parsed = DateTime.TryParse(inputDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }

        if (!parsed)
            return ""; // or return inputDate

        return parsedDate.ToString("dd/MM/yyyy");
    }
    public static string ParseFormattedDateTime(string dateString)
    {
        string[] formats = {"dd/MM/yyyy hh:mm:ss tt",         // 12-hour with AM/PM
        "dd/MM/yyyy hh:mm:ss.fff tt",
        "dd/MM/yyyy HH:mm:ss",
        "dd-MM-yyyy HH:mm:ss",// 24-hour
        "dd/MM/yyyy HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss.fff"};
        DateTime parsedDate;

        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        throw new FormatException("Date string was not recognized as a valid DateTime.");
    }
    public static string ParseFormattedDateTime1(string dateString)
    {
        dateString = dateString.Trim();

        bool hasAmPm = dateString.EndsWith("AM", StringComparison.OrdinalIgnoreCase) ||
                       dateString.EndsWith("PM", StringComparison.OrdinalIgnoreCase);

        if (hasAmPm)
        {
            // Extract the time part
            string[] parts = dateString.Split(' ');
            if (parts.Length >= 2 && TimeSpan.TryParse(parts[1], out TimeSpan time))
            {
                if (time.Hours > 12)
                {
                    // 24-hour time with AM/PM is invalid, clean it up
                    dateString = dateString.Replace("AM", "", StringComparison.OrdinalIgnoreCase)
                                           .Replace("PM", "", StringComparison.OrdinalIgnoreCase).Trim();
                    hasAmPm = false; // treat as 24-hour now
                }
            }
        }

        if (!hasAmPm)
        {
            // Remove any AM/PM if it sneaked in
            dateString = dateString.Replace("AM", "", StringComparison.OrdinalIgnoreCase)
                                   .Replace("PM", "", StringComparison.OrdinalIgnoreCase).Trim();
        }

        string[] formats = {
        "dd/MM/yyyy hh:mm:ss tt",
        "dd/MM/yyyy hh:mm:ss.fff tt",
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss.fff"
    };

        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
        }


        throw new FormatException("Date string was not recognized as a valid DateTime.");
    }

    public static string ParseFormattedTime(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return string.Empty;
        }
        DateTime date;
        string[] formats = { "HH:mm:ss", "" };

        if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
        {
            return date.ToString("HH:mm:ss");
        }
        else
        {
            return string.Empty;
        }
    }
    public static List<T> DataTableToList<T>(this DataTable table, string Tbname = "") where T : new()
    {
        List<T> list = new List<T>();
        var typeProperties = typeof(T).GetProperties().Select(propertyInfo => new
        {
            PropertyInfo = propertyInfo,
            Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
        }).ToList();

        foreach (var row in table.Rows.Cast<DataRow>())
        {
            T obj = new T();
            foreach (var typeProperty in typeProperties)
            {
                if (table.TableName == "SODASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmNo" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList"
                    && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "BranchList" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "AmendmentDate"
                     && typeProperty.PropertyInfo.Name != "ConsigneeAccountName" && typeProperty.PropertyInfo.Name != "SODelivery" && typeProperty.PropertyInfo.Name != "ResposibleSalesPersonID"
                     && typeProperty.PropertyInfo.Name != "AmendmentReason" && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "Rejper" && typeProperty.PropertyInfo.Name != "ProjQty1"
                     && typeProperty.PropertyInfo.Name != "ProjQty2" && typeProperty.PropertyInfo.Name != "Excessper" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "Qty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Rate"
                     && typeProperty.PropertyInfo.Name != "HSNNO" && typeProperty.PropertyInfo.Name != "OtherRateCurr" && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "TolLimit"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "Remark" && typeProperty.PropertyInfo.Name != "StockQty" && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "Description"
                     && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "DiscPer")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "AMMDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmNo" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList"
                    && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "BranchList" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "AmendmentDate"
                     && typeProperty.PropertyInfo.Name != "ConsigneeAccountName" && typeProperty.PropertyInfo.Name != "SODelivery" && typeProperty.PropertyInfo.Name != "ResposibleSalesPersonID"
                     && typeProperty.PropertyInfo.Name != "AmendmentReason" && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "Rejper" && typeProperty.PropertyInfo.Name != "ProjQty1"
                     && typeProperty.PropertyInfo.Name != "ProjQty2" && typeProperty.PropertyInfo.Name != "Excessper" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "Qty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Rate"
                     && typeProperty.PropertyInfo.Name != "HSNNO" && typeProperty.PropertyInfo.Name != "OtherRateCurr" && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "TolLimit"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "Remark" && typeProperty.PropertyInfo.Name != "StockQty" && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "Description"
                     && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "DiscPer"
                     && typeProperty.PropertyInfo.Name != "EntryDate" && typeProperty.PropertyInfo.Name != "EntryTime" && typeProperty.PropertyInfo.Name != "QuotNo"
                     && typeProperty.PropertyInfo.Name != "QDate" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "Address"
                     && typeProperty.PropertyInfo.Name != "ConsigneeAccountName" && typeProperty.PropertyInfo.Name != "FreightPaidBy" && typeProperty.PropertyInfo.Name != "InsuApplicable"
                     && typeProperty.PropertyInfo.Name != "ModeTransport" && typeProperty.PropertyInfo.Name != "DeliverySch" && typeProperty.PropertyInfo.Name != "PackingChgApplicable"
                     && typeProperty.PropertyInfo.Name != "DeliveryTerms" && typeProperty.PropertyInfo.Name != "PreparedBy" && typeProperty.PropertyInfo.Name != "TotalDiscount" && typeProperty.PropertyInfo.Name != "SODelivery"
                     && typeProperty.PropertyInfo.Name != "TotalDisPercent" && typeProperty.PropertyInfo.Name != "TotalDiscAmt" && typeProperty.PropertyInfo.Name != "DespatchAdviseComplete" && typeProperty.PropertyInfo.Name != "PortToLoading"
                     && typeProperty.PropertyInfo.Name != "PortOfDischarge" && typeProperty.PropertyInfo.Name != "ResposibleSalesPersonID" && typeProperty.PropertyInfo.Name != "CustContactPerson" && typeProperty.PropertyInfo.Name != "SaleDocType"
                     && typeProperty.PropertyInfo.Name != "OtherDetail" && typeProperty.PropertyInfo.Name != "OrderDelayReason" && typeProperty.PropertyInfo.Name != "UID" && typeProperty.PropertyInfo.Name != "RoundOff"
                     && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "EntryByMachineName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "CreditNoteSummTable" || Tbname == "CreditNoteSummTable")
                {
                    if ( typeProperty.PropertyInfo.Name != "CheckBoxNo"
                        && typeProperty.PropertyInfo.Name != "PaymentCreditDay" && typeProperty.PropertyInfo.Name != "GstRegUnreg"
                        && typeProperty.PropertyInfo.Name != "ActualEnteredBy" && typeProperty.PropertyInfo.Name != "ActualEnteredByName"
                        && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "LastUpdatedByName"
                        && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "FinToDate"
                        && typeProperty.PropertyInfo.Name != "YesNoList" && typeProperty.PropertyInfo.Name != "AccCreditNoteDetails"
                        && typeProperty.PropertyInfo.Name != "AccCreditNoteAgainstBillDetails" && typeProperty.PropertyInfo.Name != "ItemDetailGrid"
                        && typeProperty.PropertyInfo.Name != "AgainstSaleBillBillNo" && typeProperty.PropertyInfo.Name != "AgainstSaleBillEntryId"
                         && typeProperty.PropertyInfo.Name != "AgainstSaleBillYearCode" && typeProperty.PropertyInfo.Name != "AgainstSaleBillDate"
                         && typeProperty.PropertyInfo.Name != "AgainstSaleBillEntryId" && typeProperty.PropertyInfo.Name != "AgainstSaleBillVoucherNo"
                         && typeProperty.PropertyInfo.Name != "SaleBillType" && typeProperty.PropertyInfo.Name != "AgainstPurchaseBillBillNo"
                         && typeProperty.PropertyInfo.Name != "AgainstPurchaseBillYearCode" && typeProperty.PropertyInfo.Name != "AgainstPurchaseBillDate"
                         && typeProperty.PropertyInfo.Name != "AgainstPurchaseBillEntryId" && typeProperty.PropertyInfo.Name != "AgainstPurchaseVoucherNo"
                         && typeProperty.PropertyInfo.Name != "PurchaseBillType" && typeProperty.PropertyInfo.Name != " ItemCode"
                         && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ItemName"
                         && typeProperty.PropertyInfo.Name != "BillQty" && typeProperty.PropertyInfo.Name != "Unit"
                         && typeProperty.PropertyInfo.Name != "BillRate" && typeProperty.PropertyInfo.Name != " DiscountPer"
                         && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "ItemSize"
                         && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PONO"
                         && typeProperty.PropertyInfo.Name != "PODate" && typeProperty.PropertyInfo.Name != "POEntryId"
                         && typeProperty.PropertyInfo.Name != "POYearCode" && typeProperty.PropertyInfo.Name != "PoRate"
                         && typeProperty.PropertyInfo.Name != "PoAmmNo" && typeProperty.PropertyInfo.Name != "SONO"
                         && typeProperty.PropertyInfo.Name != "SOYearCode" && typeProperty.PropertyInfo.Name != "SODate"
                         && typeProperty.PropertyInfo.Name != "CustOrderNo" && typeProperty.PropertyInfo.Name != "SOEntryId"
                         && typeProperty.PropertyInfo.Name != "BatchNo" && typeProperty.PropertyInfo.Name != "UniqueBatchNo"
                         && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                         && typeProperty.PropertyInfo.Name != "ItemNetAmount" && typeProperty.PropertyInfo.Name != "NetTotal"
                         && typeProperty.PropertyInfo.Name != "TotalAmtAftrDiscount" && typeProperty.PropertyInfo.Name != "TotalDiscountPercentage"
                         && typeProperty.PropertyInfo.Name != "TotalRoundOff" && typeProperty.PropertyInfo.Name != "AgainstSaleBillYearCode"
                         && typeProperty.PropertyInfo.Name != "TotalRoundOffAmt" && typeProperty.PropertyInfo.Name != "adjustmentModel" && typeProperty.PropertyInfo.Name != "SeqNo"
                             && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "ItemSA"
                             && typeProperty.PropertyInfo.Name != "DA" && typeProperty.PropertyInfo.Name != "ShowAllBill"
                             && typeProperty.PropertyInfo.Name != "HSNNo" && typeProperty.PropertyInfo.Name != "BillQty"
                             && typeProperty.PropertyInfo.Name != "RejectedQty" && typeProperty.PropertyInfo.Name != "Unit"
                             && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                             && typeProperty.PropertyInfo.Name != "CRNRate" && typeProperty.PropertyInfo.Name != "BillRate"
                             && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "AltRate"
                             && typeProperty.PropertyInfo.Name != "NoOfCase" && typeProperty.PropertyInfo.Name != "CostCenterId"
                             && typeProperty.PropertyInfo.Name != "DocAccountCode" && typeProperty.PropertyInfo.Name != "DocAccountName"
                             && typeProperty.PropertyInfo.Name != "ItemAmount" && typeProperty.PropertyInfo.Name != "DiscountPer"
                             && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "StoreId"
                             && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "ItemSize"
                             && typeProperty.PropertyInfo.Name != "ItemDescription" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "CreditNoteDashboard"
                             && typeProperty.PropertyInfo.Name != "ToDate1" && typeProperty.PropertyInfo.Name != "FromDate1" && typeProperty.PropertyInfo.Name != "Searchbox"
                             && typeProperty.PropertyInfo.Name != "TaxDetailGridd"
                             && typeProperty.PropertyInfo.Name != "TaxList" && typeProperty.PropertyInfo.Name != "TotalTaxAmt"
                            && typeProperty.PropertyInfo.Name != "TxAccountCode" && typeProperty.PropertyInfo.Name != "TxAccountName"
                            && typeProperty.PropertyInfo.Name != "TxAdInTxable" && typeProperty.PropertyInfo.Name != "TxAmount"
                            && typeProperty.PropertyInfo.Name != "TxItemCode" && typeProperty.PropertyInfo.Name != "TxItemName"
                            && typeProperty.PropertyInfo.Name != "TxOnExp" && typeProperty.PropertyInfo.Name != "TxPartCode"
                            && typeProperty.PropertyInfo.Name != "TxPartName" && typeProperty.PropertyInfo.Name != "TxPercentg"
                            && typeProperty.PropertyInfo.Name != "TxRefundable" && typeProperty.PropertyInfo.Name != "TxRemark"
                            && typeProperty.PropertyInfo.Name != "TxRoundOff" && typeProperty.PropertyInfo.Name != "TxSeqNo"
                            && typeProperty.PropertyInfo.Name != "TxTaxType" && typeProperty.PropertyInfo.Name != "TxTaxTypeName"
                            && typeProperty.PropertyInfo.Name != "TxType" && typeProperty.PropertyInfo.Name != "YesNo"
                             && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "CreatedBy"
                                 && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "EID"
                                 && typeProperty.PropertyInfo.Name != "ID" && typeProperty.PropertyInfo.Name != "Mode"
                                 && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "UpdatedBy"
                                 && typeProperty.PropertyInfo.Name != "UpdatedOn" && typeProperty.PropertyInfo.Name != "DRCRGrid"
                                  && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "TotalRecords"
                                   && typeProperty.PropertyInfo.Name != "PageSize"
                                    )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "SODETAILDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmNo" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode"
                     && typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList"
                    && typeProperty.PropertyInfo.Name != "BranchList" && typeProperty.PropertyInfo.Name != "SummaryDetail"
                    && typeProperty.PropertyInfo.Name != "eMailFromCC1" && typeProperty.PropertyInfo.Name != "eMailFromCC2" 
                    && typeProperty.PropertyInfo.Name != "eMailFromCC3"
                    && typeProperty.PropertyInfo.Name != "pendingAmt"
                    && typeProperty.PropertyInfo.Name != "SalesPersonEmailId"
                    && typeProperty.PropertyInfo.Name != "ConsigneeAccountName" && typeProperty.PropertyInfo.Name != "SODelivery" && typeProperty.PropertyInfo.Name != "ResposibleSalesPersonID")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "SOCOMPLETED")
                {
                    if (typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate"
                        && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "AmmApproved" && typeProperty.PropertyInfo.Name != "SODashboard"
                        && typeProperty.PropertyInfo.Name != "SONoList" && typeProperty.PropertyInfo.Name != "BranchList" && typeProperty.PropertyInfo.Name != "AmmEffDate"
                        && typeProperty.PropertyInfo.Name != "AmendmentDate" && typeProperty.PropertyInfo.Name != "AmendmentReason" && typeProperty.PropertyInfo.Name != "Color"
                        && typeProperty.PropertyInfo.Name != "RejPer" && typeProperty.PropertyInfo.Name != "ProjQty1" && typeProperty.PropertyInfo.Name != "ProjQty2" && typeProperty.PropertyInfo.Name != "Excessper" && typeProperty.PropertyInfo.Name != "AmmApproved"
                        && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "EntryDate"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Rate"
                     && typeProperty.PropertyInfo.Name != "HSNNO" && typeProperty.PropertyInfo.Name != "OtherRateCurr" && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "TolLimit"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "Remark" && typeProperty.PropertyInfo.Name != "StockQty" && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "Description"
                     && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "Rejper" && typeProperty.PropertyInfo.Name != "EntryTime" && typeProperty.PropertyInfo.Name != "QuotNo"
                     && typeProperty.PropertyInfo.Name != "QDate" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "Address"
                     && typeProperty.PropertyInfo.Name != "ConsigneeAccountName" && typeProperty.PropertyInfo.Name != "FreightPaidBy" && typeProperty.PropertyInfo.Name != "InsuApplicable"
                     && typeProperty.PropertyInfo.Name != "ModeTransport" && typeProperty.PropertyInfo.Name != "DeliverySch" && typeProperty.PropertyInfo.Name != "PackingChgApplicable"
                     && typeProperty.PropertyInfo.Name != "DeliveryTerms" && typeProperty.PropertyInfo.Name != "PreparedBy" && typeProperty.PropertyInfo.Name != "TotalDiscount" && typeProperty.PropertyInfo.Name != "SODelivery"
                     && typeProperty.PropertyInfo.Name != "TotalDisPercent" && typeProperty.PropertyInfo.Name != "TotalDiscAmt" && typeProperty.PropertyInfo.Name != "DespatchAdviseComplete" && typeProperty.PropertyInfo.Name != "PortToLoading"
                     && typeProperty.PropertyInfo.Name != "PortOfDischarge" && typeProperty.PropertyInfo.Name != "ResposibleSalesPersonID" && typeProperty.PropertyInfo.Name != "CustContactPerson" && typeProperty.PropertyInfo.Name != "SaleDocType"
                     && typeProperty.PropertyInfo.Name != "OtherDetail" && typeProperty.PropertyInfo.Name != "OrderDelayReason" && typeProperty.PropertyInfo.Name != "UID" && typeProperty.PropertyInfo.Name != "RoundOff"
                     && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "EntryByMachineName" & typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "BOMDataForConstraint")
                {
                    if (typeProperty.PropertyInfo.Name == "BomNo" || typeProperty.PropertyInfo.Name == "FinishItemCode" || typeProperty.PropertyInfo.Name == "ItemCode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "HSNDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "EmpCategDetail1")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "DeptWiseCategDetail1")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "LeaveEmpCategDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "HRWeekEmpCategDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (table.TableName == "LeaveDeptWiseCategDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "TAXMASTERDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AddInTaxableList" && typeProperty.PropertyInfo.Name != "RefundableList" && typeProperty.PropertyInfo.Name != "TaxCategoryList"
                        && typeProperty.PropertyInfo.Name != "TaxTypeList" && typeProperty.PropertyInfo.Name != "TMDashboard")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "RCDashboard" || Tbname == "RCDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "RCDashboardList" && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "ISTDashboard" || Tbname == "ISTDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "ISTDashboardGrid" && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RoutingTable")
                {
                    if (typeProperty.PropertyInfo.Name != "RoutingGrid" && typeProperty.PropertyInfo.Name != "SummaryDetail"
                    && typeProperty.PropertyInfo.Name != "StoreID" && typeProperty.PropertyInfo.Name != "StoreName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "PODASHBOARD" || Tbname == "PODASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode"
              && typeProperty.PropertyInfo.Name != "ItemName" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard"
              //&& typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate"
               //&& typeProperty.PropertyInfo.Name != "Approved"
               && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "POQty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted"
                      //&& typeProperty.PropertyInfo.Name != "EnteredBy" 
                      && typeProperty.PropertyInfo.Name != "AmmType"
                      && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "ItemCode"
                      && typeProperty.PropertyInfo.Name != "HSNNo" && typeProperty.PropertyInfo.Name != "TolLimitPercentage"
                      && typeProperty.PropertyInfo.Name != "TolLimitQty" && typeProperty.PropertyInfo.Name != "AdditionalRate"
                      && typeProperty.PropertyInfo.Name != "OldRate" && typeProperty.PropertyInfo.Name != "Remark"
                       && typeProperty.PropertyInfo.Name != "Description" && typeProperty.PropertyInfo.Name != "PendQty"
                      && typeProperty.PropertyInfo.Name != "PendAltQty" && typeProperty.PropertyInfo.Name != "Process"
                      && typeProperty.PropertyInfo.Name != "PkgStd" && typeProperty.PropertyInfo.Name != "AmmendmentNo"
                      && typeProperty.PropertyInfo.Name != "AmmendmentNo" && typeProperty.PropertyInfo.Name != "AmmendmentDate"
                      && typeProperty.PropertyInfo.Name != "AmmendmentReason" && typeProperty.PropertyInfo.Name != "FirstMonthTentQty"
                      && typeProperty.PropertyInfo.Name != "SecMonthTentQty" && typeProperty.PropertyInfo.Name != "SizeDetail"
                      && typeProperty.PropertyInfo.Name != "Colour" && typeProperty.PropertyInfo.Name != "CostCenter"
                      && typeProperty.PropertyInfo.Name != "Active"
                      && typeProperty.PropertyInfo.Name != "RateApplicableOnUnit" && typeProperty.PropertyInfo.Name != "EntryDate"
                      && typeProperty.PropertyInfo.Name != "Branch"
                      && typeProperty.PropertyInfo.Name != "ActualEntryBy"
                      && typeProperty.PropertyInfo.Name != "AmmEffDate"
                     // && typeProperty.PropertyInfo.Name != "ApproveddByEmp"
                     && typeProperty.PropertyInfo.Name != "DeliveryTerm"
                     // && typeProperty.PropertyInfo.Name != "UpdateddByEmp"
                     && typeProperty.PropertyInfo.Name != "UpdatedOn"
                     && typeProperty.PropertyInfo.Name != "PoallowtoprintWithoutApproval"
                      && typeProperty.PropertyInfo.Name != "ShowOnlyAmendItem"
                      && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                      && typeProperty.PropertyInfo.Name != "OrderNo"
                      )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (table.TableName == "POCompletedDetail" || Tbname == "POCompletedDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "ApproveAmm" && typeProperty.PropertyInfo.Name != "Approval1Levelapproved"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" //&& typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" && typeProperty.PropertyInfo.Name != "SchQty" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "RateInOtherCurr"
                      && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType"
                      && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "ItemCode"
                      && typeProperty.PropertyInfo.Name != "TolLimitPercentage" && typeProperty.PropertyInfo.Name != "CretaedByName" && typeProperty.PropertyInfo.Name != "UpdatedByName"
                    && typeProperty.PropertyInfo.Name != "Remark" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "PendQty"
                      && typeProperty.PropertyInfo.Name != "Process" 
                      && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                      && typeProperty.PropertyInfo.Name != "ShowOnlyAmendItem"
                      && typeProperty.PropertyInfo.Name != "PoallowtoprintWithoutApproval"
                      && typeProperty.PropertyInfo.Name != "PkgStd" && typeProperty.PropertyInfo.Name != "AmmendmentNo"
                      && typeProperty.PropertyInfo.Name != "AmmendmentNo" && typeProperty.PropertyInfo.Name != "AmmendmentDate"
                      && typeProperty.PropertyInfo.Name != "AmmendmentReason" && typeProperty.PropertyInfo.Name != "FirstMonthTentQty" && typeProperty.PropertyInfo.Name != "CreatedBy"
                      && typeProperty.PropertyInfo.Name != "SecMonthTentQty" && typeProperty.PropertyInfo.Name != "SizeDetail" && typeProperty.PropertyInfo.Name != "DeliveryTerm"
                      && typeProperty.PropertyInfo.Name != "Colour" && typeProperty.PropertyInfo.Name != "CostCenter" && typeProperty.PropertyInfo.Name != "FOC"
                      && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "RateApplicableOnUnit" && typeProperty.PropertyInfo.Name != "CC")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "CostCenterMaster" || Tbname == "CostCenterMaster")
                {
                    if (typeProperty.PropertyInfo.Name != "CostcenetrGroupName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (table.TableName == "POCompletedSummary" || Tbname == "POCompletedSummary")
                {
                    if (typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "Approval1Levelapproved"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" && typeProperty.PropertyInfo.Name != "SchQty" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr"
                      && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType"
                      && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "ItemCode"
                      && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "TolLimitPercentage" && typeProperty.PropertyInfo.Name != "CretaedByName" && typeProperty.PropertyInfo.Name != "UpdatedByName"
                    && typeProperty.PropertyInfo.Name != "Remark" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "PendQty"
                      && typeProperty.PropertyInfo.Name != "Process" && typeProperty.PropertyInfo.Name != "PkgStd" && typeProperty.PropertyInfo.Name != "AmmendmentNo"
                      && typeProperty.PropertyInfo.Name != "AmmendmentNo" && typeProperty.PropertyInfo.Name != "AmmendmentDate"
                      && typeProperty.PropertyInfo.Name != "AmmendmentReason" && typeProperty.PropertyInfo.Name != "FirstMonthTentQty" && typeProperty.PropertyInfo.Name != "CreatedBy"
                      && typeProperty.PropertyInfo.Name != "SecMonthTentQty" && typeProperty.PropertyInfo.Name != "SizeDetail" && typeProperty.PropertyInfo.Name != "DeliveryTerm"
                      && typeProperty.PropertyInfo.Name != "Colour" && typeProperty.PropertyInfo.Name != "CostCenter" && typeProperty.PropertyInfo.Name != "FOC"
                      && typeProperty.PropertyInfo.Name != "CostcenetrGroupName"
                      && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "RateApplicableOnUnit" && typeProperty.PropertyInfo.Name != "CC"
                      && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "rate" && typeProperty.PropertyInfo.Name != "OldRate" && typeProperty.PropertyInfo.Name != "POQty"
                      && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "Description" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "PoallowtoprintWithoutApproval"
                       && typeProperty.PropertyInfo.Name != "ShowOnlyAmendItem" &&
                       typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                      && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "AdditionalRate"
                      && typeProperty.PropertyInfo.Name != "OldRate" && typeProperty.PropertyInfo.Name != "Remark"
                      && typeProperty.PropertyInfo.Name != "Description" && typeProperty.PropertyInfo.Name != "PendQty"
                      && typeProperty.PropertyInfo.Name != "PendAltQty" && typeProperty.PropertyInfo.Name != "Process"
                      && typeProperty.PropertyInfo.Name != "PkgStd" && typeProperty.PropertyInfo.Name != "AmmendmentNo"
                      && typeProperty.PropertyInfo.Name != "AmmendmentNo" && typeProperty.PropertyInfo.Name != "AmmendmentDate"
                      && typeProperty.PropertyInfo.Name != "AmmendmentReason" && typeProperty.PropertyInfo.Name != "FirstMonthTentQty"
                      && typeProperty.PropertyInfo.Name != "SecMonthTentQty" && typeProperty.PropertyInfo.Name != "SizeDetail" && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                      && typeProperty.PropertyInfo.Name != "Colour" && typeProperty.PropertyInfo.Name != "CostCenter" && typeProperty.PropertyInfo.Name != "TolLimitQty"
                      && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "RateApplicableOnUnit" && typeProperty.PropertyInfo.Name != "HSNNo")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (table.TableName == "PODASHBOARDMain" || Tbname == "PODASHBOARDMain")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ItemName" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "POQty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType"
                      && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "SummaryDetail"
                      && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "ItemCode"
                      && typeProperty.PropertyInfo.Name != "HSNNo" && typeProperty.PropertyInfo.Name != "POQty"
                      && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty"
                      && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "TolLimitPercentage"
                      && typeProperty.PropertyInfo.Name != "TolLimitQty" && typeProperty.PropertyInfo.Name != "UnitRate"
                      && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr"
                      && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                      && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "AdditionalRate"
                      && typeProperty.PropertyInfo.Name != "OldRate" && typeProperty.PropertyInfo.Name != "Remark"
                      && typeProperty.PropertyInfo.Name != "Description" && typeProperty.PropertyInfo.Name != "PendQty"
                      && typeProperty.PropertyInfo.Name != "PendAltQty" && typeProperty.PropertyInfo.Name != "Process"
                      && typeProperty.PropertyInfo.Name != "PkgStd" && typeProperty.PropertyInfo.Name != "AmmendmentNo"
                      && typeProperty.PropertyInfo.Name != "AmmendmentNo" && typeProperty.PropertyInfo.Name != "AmmendmentDate"
                      && typeProperty.PropertyInfo.Name != "AmmendmentReason" && typeProperty.PropertyInfo.Name != "FirstMonthTentQty"
                      && typeProperty.PropertyInfo.Name != "SecMonthTentQty" && typeProperty.PropertyInfo.Name != "SizeDetail"
                      && typeProperty.PropertyInfo.Name != "Colour" && typeProperty.PropertyInfo.Name != "CostCenter"
                      && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "RateApplicableOnUnit")

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "PODETAILDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" &&
               typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "POQty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                      && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "ApprovedBy" && typeProperty.PropertyInfo.Name != "DashboardType")

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (table.TableName == "PODASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ItemName" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "POQty"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                     && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                     && typeProperty.PropertyInfo.Name != "UnitRate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType" && typeProperty.PropertyInfo.Name != "UpdatedByName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                //else if (table.TableName == "POSCHEDULEDASHBOARD")
                else if (Tbname == "POSCHEDULEDETAILDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" 
              //&& typeProperty.PropertyInfo.Name != "ItemName" &&
              //typeProperty.PropertyInfo.Name != "PartCode"
              && typeProperty.PropertyInfo.Name != "TxPageName"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "PODashboard" && typeProperty.PropertyInfo.Name != "PONoList"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" 
               //&& typeProperty.PropertyInfo.Name != "SchQty"
                     //&& typeProperty.PropertyInfo.Name != "Rate" 
                     && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "DashboardType"
                     && typeProperty.PropertyInfo.Name != "QuotNo" && typeProperty.PropertyInfo.Name != "QuotYear" && typeProperty.PropertyInfo.Name != "BasicAmount"
                     && typeProperty.PropertyInfo.Name != "NetAmount" && typeProperty.PropertyInfo.Name != "HsnNo" 
                     //&& typeProperty.PropertyInfo.Name != "POQty"
                     //&& typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltPOQty" && typeProperty.PropertyInfo.Name != "AltUnit"
                     //&& typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "DiscPer" && typeProperty.PropertyInfo.Name != "DiscRs"
                     && typeProperty.PropertyInfo.Name != "Amount" 
                     //&& typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                     //&& typeProperty.PropertyInfo.Name != "UnitRate" 
                     //&& typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                     // && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType"
                      && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType" && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "ApprovedBy")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "POSCHEDULEDASHBOARD")
                {
                    if (typeProperty.PropertyInfo.Name != "AmmEffDate" && typeProperty.PropertyInfo.Name != "ApprovedBy"
              && typeProperty.PropertyInfo.Name != "FromDate" && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "schAmendDate" &&
              typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
              && typeProperty.PropertyInfo.Name != "CreatedByEmpName"
              && typeProperty.PropertyInfo.Name != "SchAmendApprovedByEmp"
              && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "Branch"
              && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "POComplete"
               && typeProperty.PropertyInfo.Name != "ApprovedDate" && typeProperty.PropertyInfo.Name != "Approved" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "ApprovedBy"
                   && typeProperty.PropertyInfo.Name != "UserName" && typeProperty.PropertyInfo.Name != "ItemAmendNo"
                     && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "AltSchQty"
                  && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "AltPendQty" && typeProperty.PropertyInfo.Name != "Canceled"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "EnteredBy" && typeProperty.PropertyInfo.Name != "AmmType" && typeProperty.PropertyInfo.Name != "UpdatedByName"
                       && typeProperty.PropertyInfo.Name != "schAmendNo"
                         && typeProperty.PropertyInfo.Name != "MRPNO" && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                         && typeProperty.PropertyInfo.Name != "Active"
                         && typeProperty.PropertyInfo.Name != "Mode"
                         && typeProperty.PropertyInfo.Name != "PartCode"
                         && typeProperty.PropertyInfo.Name != "ItemName"
                         && typeProperty.PropertyInfo.Name != "Rate"
                         && typeProperty.PropertyInfo.Name != "SchQty"
                         && typeProperty.PropertyInfo.Name != "PendQty"
                         && typeProperty.PropertyInfo.Name != "Unit"
                         && typeProperty.PropertyInfo.Name != "AltUnit"
                         && typeProperty.PropertyInfo.Name != "AltSchQty"
                         && typeProperty.PropertyInfo.Name != "AltPendQty"
                         && typeProperty.PropertyInfo.Name != "ItemAmendNo"
                         && typeProperty.PropertyInfo.Name != "SchApproved"
                         && typeProperty.PropertyInfo.Name != "DeliveryDate"
                         && typeProperty.PropertyInfo.Name != "Canceled"
                         && typeProperty.PropertyInfo.Name != "SchCompleted"
                      )

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "StockAdjustment" || table.TableName == "StockAdjustment")
                {
                    if (typeProperty.PropertyInfo.Name != "SADashboard" && typeProperty.PropertyInfo.Name != "PartCode" &&
                        typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "Storeid" && typeProperty.PropertyInfo.Name != "wcid" && typeProperty.PropertyInfo.Name != "ToDate"
                    && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "WorkCenter" && typeProperty.PropertyInfo.Name != "altUnit"
                     && typeProperty.PropertyInfo.Name != "TotalStock" && typeProperty.PropertyInfo.Name != "LotStock" && typeProperty.PropertyInfo.Name != "ItemCode"
                     && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "ActuleStockQty"
                     && typeProperty.PropertyInfo.Name != "AdjType" && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "AdjQty"
                     && typeProperty.PropertyInfo.Name != "batchno" && typeProperty.PropertyInfo.Name != "uniquebatchno" && typeProperty.PropertyInfo.Name != "reasonOfAdjutment")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "CustomerSummary" || table.TableName == "CustomerSummary")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "BOMNO"
                        && typeProperty.PropertyInfo.Name != "BOMDate" && typeProperty.PropertyInfo.Name != "UID"
                        && typeProperty.PropertyInfo.Name != "RGPNRGP" && typeProperty.PropertyInfo.Name != "ChallanType"
                        && typeProperty.PropertyInfo.Name != "ClosedChallan" && typeProperty.PropertyInfo.Name != "CloseChallandate"
                        && typeProperty.PropertyInfo.Name != "Emp_id" && typeProperty.PropertyInfo.Name != "Empname" && typeProperty.PropertyInfo.Name != "IssChalanEntryId")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "jobWorkopening" || table.TableName == "jobWorkopening")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "UID"
                        && typeProperty.PropertyInfo.Name != "ClosedChallan" && typeProperty.PropertyInfo.Name != "CloseChallandate"
                        && typeProperty.PropertyInfo.Name != "ChallanType" && typeProperty.PropertyInfo.Name != "RGPNRGP"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RGPChallan" || table.TableName == "RGPChallan")
                {
                    if (
                        typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "Emp_id" && typeProperty.PropertyInfo.Name != "Empname" &&
                        typeProperty.PropertyInfo.Name != "Closed" && typeProperty.PropertyInfo.Name != "IssChalanEntryId" && typeProperty.PropertyInfo.Name != "LastUpdatedByEmp" && typeProperty.PropertyInfo.Name != "EnteredByMachine"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (Tbname == "customerJobWorkOpeningDetail" || table.TableName == "customerJobWorkOpeningDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "Emp_id" && typeProperty.PropertyInfo.Name != "Empname"
                     && typeProperty.PropertyInfo.Name != "IssChalanEntryId" && typeProperty.PropertyInfo.Name != "LastUpdatedByEmp" && typeProperty.PropertyInfo.Name != "ItemCode"
                     && typeProperty.PropertyInfo.Name != "OpnIssQty" && typeProperty.PropertyInfo.Name != "ScrapQty" && typeProperty.PropertyInfo.Name != "PendScrapToRec"
                     && typeProperty.PropertyInfo.Name != "ChallanQty" && typeProperty.PropertyInfo.Name != "IssChalanEntryId"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (Tbname == "jobWorkopeningDetail" || table.TableName == "jobWorkopeningDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "uid" && typeProperty.PropertyInfo.Name != "BOMNO" && typeProperty.PropertyInfo.Name != "BOMDate"
                     && typeProperty.PropertyInfo.Name != "Empname"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RGPChallanDetail" || table.TableName == "RGPChallanDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "Mode"
                     && typeProperty.PropertyInfo.Name != "Empname" && typeProperty.PropertyInfo.Name != "RecItemName" && typeProperty.PropertyInfo.Name != "RecPartCode"
                     && typeProperty.PropertyInfo.Name != "BomStatus" && typeProperty.PropertyInfo.Name != "BOMNO" && typeProperty.PropertyInfo.Name != "BOMDate"
                     && typeProperty.PropertyInfo.Name != "RemainingPendQty" && typeProperty.PropertyInfo.Name != "ProcessName" && typeProperty.PropertyInfo.Name != "EnteredByMachine"
                     && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "OpnIssQty" && typeProperty.PropertyInfo.Name != "Emp_id"
                     && typeProperty.PropertyInfo.Name != "ScrapItemName" && typeProperty.PropertyInfo.Name != "ScrapPartCode" && typeProperty.PropertyInfo.Name != "ScrapQty" && typeProperty.PropertyInfo.Name != "PendScrapToRec"
                     && typeProperty.PropertyInfo.Name != "IssChalanEntryId" && typeProperty.PropertyInfo.Name != "LastUpdatedByEmp" && typeProperty.PropertyInfo.Name != "Closed"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "StockAdjustmentDetail")
                {

                    if (typeProperty.PropertyInfo.Name != "SADashboard" && typeProperty.PropertyInfo.Name != "FromDate"
                     && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IssueNRGP" || table.TableName == "IssueNRGP")
                {
                    if (typeProperty.PropertyInfo.Name != "INDasboard" && typeProperty.PropertyInfo.Name != "INNDashboard" && typeProperty.PropertyInfo.Name != "ActualEnteredEMpBy"
                        && typeProperty.PropertyInfo.Name != "ChallanTypeList" && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "AccLedgerOpening" || table.TableName == "AccLedgerOpening")
                {
                    if (typeProperty.PropertyInfo.Name != "CreatedBy" &&
                        typeProperty.PropertyInfo.Name != "SrNO"
                        && typeProperty.PropertyInfo.Name != "GroupAccountCode"
                         && typeProperty.PropertyInfo.Name != "ParentAccountCode"
                         && typeProperty.PropertyInfo.Name != "OpeningForYear"
                         && typeProperty.PropertyInfo.Name != "Mode"
                         && typeProperty.PropertyInfo.Name != "PreviousAmount"
                         && typeProperty.PropertyInfo.Name != "UpdatedByEmpId"
                         && typeProperty.PropertyInfo.Name != "UpdatedByEmpId"
                         && typeProperty.PropertyInfo.Name != "Account_Name"
                         && typeProperty.PropertyInfo.Name != "FinToDate"
                         && typeProperty.PropertyInfo.Name != "FinFromDate"
                         && typeProperty.PropertyInfo.Name != "LedgerOpeningEntryDashBoardGrid"
                         && typeProperty.PropertyInfo.Name != "Active"
                         && typeProperty.PropertyInfo.Name != "CreatedOn"
                         && typeProperty.PropertyInfo.Name != "EID"
                         && typeProperty.PropertyInfo.Name != "ID"
                         )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }

                }
                else if (Tbname == "IssueNRGPDetail" || table.TableName == "IssueNRGPDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "INDasboard" && typeProperty.PropertyInfo.Name != "INNDashboard"
                        && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "partcode"
                        && typeProperty.PropertyInfo.Name != "ItemNamePartCode" && typeProperty.PropertyInfo.Name != "ItemCode"
                        && typeProperty.PropertyInfo.Name != "HSNNO" && typeProperty.PropertyInfo.Name != "Store" && typeProperty.PropertyInfo.Name != "BatchNo"
                        && typeProperty.PropertyInfo.Name != "uniquebatchno" && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "TotalStock"
                        && typeProperty.PropertyInfo.Name != "BatchStock" && typeProperty.PropertyInfo.Name != "ProcessId" && typeProperty.PropertyInfo.Name != "unit"
                        && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PurchasePrice"
                        && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "altUnit" && typeProperty.PropertyInfo.Name != "StageDescription"
                        && typeProperty.PropertyInfo.Name != "PONo" && typeProperty.PropertyInfo.Name != "PoYear" && typeProperty.PropertyInfo.Name != "PODate"
                        && typeProperty.PropertyInfo.Name != "POAmmendNo" && typeProperty.PropertyInfo.Name != "discper" && typeProperty.PropertyInfo.Name != "discamt"
                        && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "AgainstChallanNoEntryId" && typeProperty.PropertyInfo.Name != "AgainstChallanNo"
                        && typeProperty.PropertyInfo.Name != "AgainstChallanYearCode" && typeProperty.PropertyInfo.Name != "AgainstChallanType" && typeProperty.PropertyInfo.Name != "closed"
                        && typeProperty.PropertyInfo.Name != "ItemColor" && typeProperty.PropertyInfo.Name != "ItemSize" && typeProperty.PropertyInfo.Name != "ItemModel"
                        && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "PendAltQty"
                        && typeProperty.PropertyInfo.Name != "ActualEnteredEMpBy")

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleSchedule" || table.TableName == "SaleSchedule")
                {
                    if (typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "SSDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "PartCode"
                     && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "SchQty" && typeProperty.PropertyInfo.Name != "PendQty"
                     && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "AltPendQty" && typeProperty.PropertyInfo.Name != "RateInOthCurr" && typeProperty.PropertyInfo.Name != "DelivaeryDate"
                     && typeProperty.PropertyInfo.Name != "ItemSize" && typeProperty.PropertyInfo.Name != "ItemColor" && typeProperty.PropertyInfo.Name != "OtherDetail" && typeProperty.PropertyInfo.Name != "Remarks"
                     && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleScheduleDetail" || table.TableName == "SaleScheduleDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "SSDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Mode"
                     && typeProperty.PropertyInfo.Name != "SummaryDetail")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "MRP" || table.TableName == "MRP")
                {
                    if (typeProperty.PropertyInfo.Name != "MRPDahboardGrid" && typeProperty.PropertyInfo.Name != "FromDate"
                            && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "ActualEnteredByName"
                            && typeProperty.PropertyInfo.Name != "LastUpdatedByName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "PendingMRP" || table.TableName == "PendingMRP")
                {
                    if (typeProperty.PropertyInfo.Name != "PendingMRPGrid" && typeProperty.PropertyInfo.Name != "Month"
                        && typeProperty.PropertyInfo.Name != "YearCode" && typeProperty.PropertyInfo.Name != "MRPGenDate"
                        && typeProperty.PropertyInfo.Name != "MRPEntryId" && typeProperty.PropertyInfo.Name != "MRPNo"
                        && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "StoreId"
                        && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "WcId"
                        && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID" && typeProperty.PropertyInfo.Name != "ForMonthYear"
                        && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "TxPageName"
                        && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "UpdatedOn" && typeProperty.PropertyInfo.Name != "SeqNo"
                        && typeProperty.PropertyInfo.Name != "IncludeProjection")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "Indent" || table.TableName == "Indent")
                {
                    if (typeProperty.PropertyInfo.Name != "IndentDashboardGrid" && typeProperty.PropertyInfo.Name != "FromDate"
                            && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "LastUpdationDate"
                            && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "LastUpdatedByName"
                            && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedByName"
                            && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "PartCode"
                            && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PendReqNo" && typeProperty.PropertyInfo.Name != "ReqYearCode"
                            && typeProperty.PropertyInfo.Name != "ReqDate" && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "Unit"
                            && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "TotalStock" && typeProperty.PropertyInfo.Name != "AltUnit"
                            && typeProperty.PropertyInfo.Name != "Model" && typeProperty.PropertyInfo.Name != "Size" && typeProperty.PropertyInfo.Name != "Account_Name"
                            && typeProperty.PropertyInfo.Name != "Account_Name2" && typeProperty.PropertyInfo.Name != "ItemDescription" && typeProperty.PropertyInfo.Name != "ItemRemark"
                            && typeProperty.PropertyInfo.Name != "Specification" && typeProperty.PropertyInfo.Name != "PendQtyForPO" && typeProperty.PropertyInfo.Name != "PendAltQtyForPO"
                            && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "ApproValue" && typeProperty.PropertyInfo.Name != "SummaryDetail"
                            && typeProperty.PropertyInfo.Name != "Searchbox"
                            && typeProperty.PropertyInfo.Name != "AltQty")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IndentDetail" || table.TableName == "IndentDetail")
                {
                    if (typeProperty.PropertyInfo.Name != "IndentDashboardGrid" && typeProperty.PropertyInfo.Name != "FromDate"
                            && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "LastUpdationDate"
                            && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "LastUpdatedByName"
                            && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedByName"
                            && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "SummaryDetail"
                            && typeProperty.PropertyInfo.Name != "Searchbox")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleScheduleComp" || table.TableName == "SaleScheduleComp")
                {
                    if (typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "SSDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "PartCode"
                     && typeProperty.PropertyInfo.Name != "SchClosed" && typeProperty.PropertyInfo.Name != "SchAmendApprove" && typeProperty.PropertyInfo.Name != "EntryDate"
                     && typeProperty.PropertyInfo.Name != "ModeOfTransport" && typeProperty.PropertyInfo.Name != "OrderPriority" && typeProperty.PropertyInfo.Name != "CC"
                     && typeProperty.PropertyInfo.Name != "UpdatedByName" && typeProperty.PropertyInfo.Name != "UpdatedOn" && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                     && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "AltPendQty" && typeProperty.PropertyInfo.Name != "Rate"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "RateInOthCurr" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "DeliveryDate"
                     && typeProperty.PropertyInfo.Name != "ItemSize" && typeProperty.PropertyInfo.Name != "ItemColor" && typeProperty.PropertyInfo.Name != "OtherDetail"
                     && typeProperty.PropertyInfo.Name != "Remarks" && typeProperty.PropertyInfo.Name != "SummaryDetail")

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleScheduleComp" || table.TableName == "SaleScheduleComp")
                {
                    if (typeProperty.PropertyInfo.Name != "SODashboard" && typeProperty.PropertyInfo.Name != "SONoList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "SSDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "SchClosed"
                     && typeProperty.PropertyInfo.Name != "EntryDate")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }

                else if (Tbname == "PurchaseSchedule")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "PONoList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "PSDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "DeliveryDate" && typeProperty.PropertyInfo.Name != "SchQty"
                     && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "SchApproved" && typeProperty.PropertyInfo.Name != "SchAmendApprove"
                      && typeProperty.PropertyInfo.Name != "SchCompleted" &&
                      typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "DashboardType"
                      && typeProperty.PropertyInfo.Name != "ApprovedBy" && typeProperty.PropertyInfo.Name != "UserName"
                      && typeProperty.PropertyInfo.Name != "CreatedBy"
                      //&& typeProperty.PropertyInfo.Name != "SchApprovedByEmp"
                      //&& typeProperty.PropertyInfo.Name != "SchAmendApprovedByEmp" && typeProperty.PropertyInfo.Name != "SchCompleted"


                      )
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "GateInward" || table.TableName == "GateInward")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "GateNOList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "GateDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "DashboardType"
                    && typeProperty.PropertyInfo.Name != "TareWeight" && typeProperty.PropertyInfo.Name != "GrossWeight" && typeProperty.PropertyInfo.Name != "NetWeight"
                    && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "AltQty"
                    && typeProperty.PropertyInfo.Name != "PendPOQty" && typeProperty.PropertyInfo.Name != "AltPendQty" && typeProperty.PropertyInfo.Name != "ShowPoTillDate"
                    && typeProperty.PropertyInfo.Name != "CC" && typeProperty.PropertyInfo.Name != "PoNo" && typeProperty.PropertyInfo.Name != "PoYearCode"
                    && typeProperty.PropertyInfo.Name != "SchYearCode" && typeProperty.PropertyInfo.Name != "POtype" && typeProperty.PropertyInfo.Name != "SchNo"
                    && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Size"
                    && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ItemName"
                    && typeProperty.PropertyInfo.Name != "EntryByMachineName" && typeProperty.PropertyInfo.Name != "SaleBillNo"
                    && typeProperty.PropertyInfo.Name != "SaleBillQty" && typeProperty.PropertyInfo.Name != "SaleBillYearCode"
                    && typeProperty.PropertyInfo.Name != "AgainstChallanNo" && typeProperty.PropertyInfo.Name != "ChallanQty"
                    && typeProperty.PropertyInfo.Name != "SupplierBatchNo" && typeProperty.PropertyInfo.Name != "ShelfLife"
                    && typeProperty.PropertyInfo.Name != "Remarks" && typeProperty.PropertyInfo.Name != "ProcessName"
                    && typeProperty.PropertyInfo.Name != "Summary" && typeProperty.PropertyInfo.Name != "ProcessId"
                    && typeProperty.PropertyInfo.Name != "AgainstChallanYearCode" && typeProperty.PropertyInfo.Name != "SeqNo"
                    && typeProperty.PropertyInfo.Name != "Types" && typeProperty.PropertyInfo.Name != "OtherDetail"
                    && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "NoOfBoxes" && typeProperty.PropertyInfo.Name != "Searchbox"
                    && typeProperty.PropertyInfo.Name != "TotalRecords" && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "PageSize")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ProductionEntry" || table.TableName == "ProductionEntry")
                {
                    if (typeProperty.PropertyInfo.Name != "ProductionDashboard" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "DashboardType"
                    && typeProperty.PropertyInfo.Name != "ConsumedRMQTY" && typeProperty.PropertyInfo.Name != "ConsumedRMUnit"
                    && typeProperty.PropertyInfo.Name != "MainRMQTY" && typeProperty.PropertyInfo.Name != "MainRMUnit"
                    && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "RMPartCode"
                    && typeProperty.PropertyInfo.Name != "RMunit"
                    && typeProperty.PropertyInfo.Name != "TotalReqRMQty" && typeProperty.PropertyInfo.Name != "TotalStock"
                    && typeProperty.PropertyInfo.Name != "BatchStock" && typeProperty.PropertyInfo.Name != "AltRMQty"
                    && typeProperty.PropertyInfo.Name != "AltRMUnit" && typeProperty.PropertyInfo.Name != "RMNetWt"
                    && typeProperty.PropertyInfo.Name != "GrossWt" && typeProperty.PropertyInfo.Name != "Batchwise"
                    && typeProperty.PropertyInfo.Name != "BOMRevNO" && typeProperty.PropertyInfo.Name != "BOMRevDate"
                    && typeProperty.PropertyInfo.Name != "ManualAutoEntry" && typeProperty.PropertyInfo.Name != "Proddate"
                    && typeProperty.PropertyInfo.Name != "BreakfromTime" && typeProperty.PropertyInfo.Name != "BreaktoTime"
                    && typeProperty.PropertyInfo.Name != "ResonDetail" && typeProperty.PropertyInfo.Name != "BreakTimeMin"
                    && typeProperty.PropertyInfo.Name != "ResEmpName" && typeProperty.PropertyInfo.Name != "ResFactor"
                    && typeProperty.PropertyInfo.Name != "WorkCenter" && typeProperty.PropertyInfo.Name != "OperatorName"
                    && typeProperty.PropertyInfo.Name != "Fromtime" && typeProperty.PropertyInfo.Name != "totime"
                    && typeProperty.PropertyInfo.Name != "TotalHrs" && typeProperty.PropertyInfo.Name != "OverTimeHrs"
                    && typeProperty.PropertyInfo.Name != "MachineCharges" && typeProperty.PropertyInfo.Name != "ScrapType"
                    && typeProperty.PropertyInfo.Name != "ScrapQty" && typeProperty.PropertyInfo.Name != "Scrapunit"
                    && typeProperty.PropertyInfo.Name != "ProdQty" && typeProperty.PropertyInfo.Name != "EntryId"
                    && typeProperty.PropertyInfo.Name != "Yearcode" && typeProperty.PropertyInfo.Name != "ReqYearcode"
                    && typeProperty.PropertyInfo.Name != "QCCheckedDate"
                    && typeProperty.PropertyInfo.Name != "StoreWC" && typeProperty.PropertyInfo.Name != "RMItemName" && typeProperty.PropertyInfo.Name != "Searchbox"
                    && typeProperty.PropertyInfo.Name != "TotalRecords" && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "PageSize"
                    && typeProperty.PropertyInfo.Name != "ProductQty" && typeProperty.PropertyInfo.Name != "ProductUnit")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "InProcessQC" || table.TableName == "InProcessQC")
                {
                    if (typeProperty.PropertyInfo.Name != "InProcessDashboard" && typeProperty.PropertyInfo.Name != "ItemName" &&
                        typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ProdEntryid" &&
                        typeProperty.PropertyInfo.Name != "ProdYearCode" && typeProperty.PropertyInfo.Name != "ProdSlipNo" &&
                        typeProperty.PropertyInfo.Name != "ProdEntryDate" && typeProperty.PropertyInfo.Name != "MaterialIstransfered" &&
                        typeProperty.PropertyInfo.Name != "ProdSchNo" && typeProperty.PropertyInfo.Name != "ProdSchYearCode" &&
                        typeProperty.PropertyInfo.Name != "ProdSchdate" && typeProperty.PropertyInfo.Name != "ProdPlanNo" &&
                        typeProperty.PropertyInfo.Name != "ProdPlanYearCode" && typeProperty.PropertyInfo.Name != "ProdPlanDate" &&
                        typeProperty.PropertyInfo.Name != "ReqNo" && typeProperty.PropertyInfo.Name != "ReqDate" &&
                        typeProperty.PropertyInfo.Name != "ReqYearCode" && typeProperty.PropertyInfo.Name != "ProdQty" &&
                        typeProperty.PropertyInfo.Name != "ProdOkQty" && typeProperty.PropertyInfo.Name != "OkQty" &&
                        typeProperty.PropertyInfo.Name != "RejQty" && typeProperty.PropertyInfo.Name != "RewQty" &&
                        typeProperty.PropertyInfo.Name != "RejReason" && typeProperty.PropertyInfo.Name != "RewRemark" &&
                        typeProperty.PropertyInfo.Name != "otherdetail" && typeProperty.PropertyInfo.Name != "Batchno" &&
                        typeProperty.PropertyInfo.Name != "Uniquebatchno" && typeProperty.PropertyInfo.Name != "TransferedQty" &&
                        typeProperty.PropertyInfo.Name != "PendForTransfQty" && typeProperty.PropertyInfo.Name != "TransfertoWorkCenter" &&
                        typeProperty.PropertyInfo.Name != "TransfertoStoreName" && typeProperty.PropertyInfo.Name != "ProcessName" &&
                        typeProperty.PropertyInfo.Name != "WorkCenter" && typeProperty.PropertyInfo.Name != "Sampleqtytested" &&
                        typeProperty.PropertyInfo.Name != "TestingMethod" && typeProperty.PropertyInfo.Name != "QcClearedEmpId"
                        && typeProperty.PropertyInfo.Name != "QcClearedEmp" && typeProperty.PropertyInfo.Name != "ActualEntryById"
                        && typeProperty.PropertyInfo.Name != "ActualEmpName" && typeProperty.PropertyInfo.Name != "LastUpdatedBy"
                        && typeProperty.PropertyInfo.Name != "LastUpdatedByEmp" && typeProperty.PropertyInfo.Name != "ToWcId"
                         && typeProperty.PropertyInfo.Name != "ToStoreId" && typeProperty.PropertyInfo.Name != "WcId"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ReceiveMaterialInStore" || table.TableName == "ReceiveMaterialInStore")
                {
                    if (typeProperty.PropertyInfo.Name != "ReceiveItemDashboard" && typeProperty.PropertyInfo.Name != "MaterialType" &&
                        typeProperty.PropertyInfo.Name != "FromDepWorkCenter" && typeProperty.PropertyInfo.Name != "ItemName" &&
                        typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ActualRecQtyInStr" &&
                        typeProperty.PropertyInfo.Name != "ActualTransferQtyFrmWC" && typeProperty.PropertyInfo.Name != "Unit" &&
                        typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" &&
                        typeProperty.PropertyInfo.Name != "QCOkQty" && typeProperty.PropertyInfo.Name != "Remark" &&
                        typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "Prodentryid" &&
                        typeProperty.PropertyInfo.Name != "ProdyearCode" && typeProperty.PropertyInfo.Name != "ProdDateAndTime" &&
                        typeProperty.PropertyInfo.Name != "PlanNoEntryId" && typeProperty.PropertyInfo.Name != "ProdPlanNo" &&
                        typeProperty.PropertyInfo.Name != "ProdPlanYearCode" && typeProperty.PropertyInfo.Name != "ProdSchEntryId" &&
                        typeProperty.PropertyInfo.Name != "ProdSchNo" && typeProperty.PropertyInfo.Name != "ProdSchYearCode" &&
                        typeProperty.PropertyInfo.Name != "InProcQCSlipNo" && typeProperty.PropertyInfo.Name != "InProcQCEntryId" &&
                        typeProperty.PropertyInfo.Name != "InProcQCYearCode" && typeProperty.PropertyInfo.Name != "ProdQty" &&
                        typeProperty.PropertyInfo.Name != "RejQty" && typeProperty.PropertyInfo.Name != "Batchno" &&
                        typeProperty.PropertyInfo.Name != "UniqueBatchno" && typeProperty.PropertyInfo.Name != "TransferMatEntryId" &&
                        typeProperty.PropertyInfo.Name != "TransferMatYearCode" && typeProperty.PropertyInfo.Name != "TransferMatSlipNo" &&
                        typeProperty.PropertyInfo.Name != "UID" && typeProperty.PropertyInfo.Name != "ReceiveDate" &&
                        typeProperty.PropertyInfo.Name != "UpdatedByEmpId")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "TransferMaterialFromWc" || table.TableName == "TransferMaterialFromWc")
                {
                    if (typeProperty.PropertyInfo.Name != "TransferFromWorkCenterDashboard" && typeProperty.PropertyInfo.Name != "PartCode"
                        && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "TransferQty"
                        && typeProperty.PropertyInfo.Name != "QCOkQty" && typeProperty.PropertyInfo.Name != "ProdQty"
                        && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltTransferQty"
                        && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "TotalStock"
                        && typeProperty.PropertyInfo.Name != "BatchNo" && typeProperty.PropertyInfo.Name != "uniquebatchno"
                        && typeProperty.PropertyInfo.Name != "BatchStock" && typeProperty.PropertyInfo.Name != "Rate"
                        && typeProperty.PropertyInfo.Name != "ItemWeight" && typeProperty.PropertyInfo.Name != "ItemSize"
                        && typeProperty.PropertyInfo.Name != "ItemColor" && typeProperty.PropertyInfo.Name != "ItemRecCompleted"
                        && typeProperty.PropertyInfo.Name != "PendingQtyToAcknowledge" && typeProperty.PropertyInfo.Name != "ItemRemark"
                        && typeProperty.PropertyInfo.Name != "ProdEntryId" && typeProperty.PropertyInfo.Name != "ProdSlipNo"
                        && typeProperty.PropertyInfo.Name != "ProdYearCode" && typeProperty.PropertyInfo.Name != "ProdEntryDate"
                        && typeProperty.PropertyInfo.Name != "ProdPlanNo" && typeProperty.PropertyInfo.Name != "ProdPlanYearCode"
                        && typeProperty.PropertyInfo.Name != "ProdSchNo" && typeProperty.PropertyInfo.Name != "ProdSchYearCode"

                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IssueAgainstProdSchedule" || table.TableName == "IssueAgainstProdSchedule")
                {
                    if (typeProperty.PropertyInfo.Name != "IssueAgainstProdScheduleDashboard"
                        && typeProperty.PropertyInfo.Name != "TotalStock"
                        && typeProperty.PropertyInfo.Name != "BatchStock" &&
                        typeProperty.PropertyInfo.Name != "WIPStock"
                        && typeProperty.PropertyInfo.Name != "StdPkg" &&
                        typeProperty.PropertyInfo.Name != "IssuedFromStoreName" && typeProperty.PropertyInfo.Name != "MaxIssueQtyAllowed" &&
                        typeProperty.PropertyInfo.Name != "WorkCenter" && typeProperty.PropertyInfo.Name != "IssueQty" &&
                        typeProperty.PropertyInfo.Name != "ProdPlanNo" && typeProperty.PropertyInfo.Name != "Unit" &&
                        typeProperty.PropertyInfo.Name != "PlanNoYearCode" && typeProperty.PropertyInfo.Name != "AltIssueQty" &&
                        typeProperty.PropertyInfo.Name != "PlanNoEntryId" && typeProperty.PropertyInfo.Name != "Altunit" &&
                        typeProperty.PropertyInfo.Name != "PlanDate" && typeProperty.PropertyInfo.Name != "PrevissueQty" &&
                        typeProperty.PropertyInfo.Name != "FGItemName" && typeProperty.PropertyInfo.Name != "PreIssueAltQty" &&
                        typeProperty.PropertyInfo.Name != "FGPartCode" && typeProperty.PropertyInfo.Name != "BOMNo" &&
                        typeProperty.PropertyInfo.Name != "ProdSchNo" && typeProperty.PropertyInfo.Name != "BomQty" &&
                        typeProperty.PropertyInfo.Name != "ProdSchEntryId" && typeProperty.PropertyInfo.Name != "WIPMaxQty" &&
                        typeProperty.PropertyInfo.Name != "ProdSchYearCode" && typeProperty.PropertyInfo.Name != "InProcessQCEntryId" &&
                        typeProperty.PropertyInfo.Name != "ProdSchDate" && typeProperty.PropertyInfo.Name != "InprocessQCSlipNo" &&
                        typeProperty.PropertyInfo.Name != "ParentProdSchNo" && typeProperty.PropertyInfo.Name != "InProcessQCYearCode" &&
                        typeProperty.PropertyInfo.Name != "ParentProdSchEntryId" && typeProperty.PropertyInfo.Name != "InProcessQCDate" &&
                        typeProperty.PropertyInfo.Name != "ParentProdSchYearCode" && typeProperty.PropertyInfo.Name != "ItemHasSubBom" &&
                        typeProperty.PropertyInfo.Name != "ParentProdSchDate" && typeProperty.PropertyInfo.Name != "MaterialIsIssuedDirectlyFrmWC" &&
                        typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "PRODPlanFGItemCode" &&
                        typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PRODSCHFGItemCode" &&
                        typeProperty.PropertyInfo.Name != "BatchNo" && typeProperty.PropertyInfo.Name != "IssueItemCode" &&
                        typeProperty.PropertyInfo.Name != "UniqueBatchno" && typeProperty.PropertyInfo.Name != "UpdatedByEmpName"

                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "MRN" || table.TableName == "MRN")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "GateNOList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "MRNQDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "MRNList" && typeProperty.PropertyInfo.Name != "PartCode"
                    && typeProperty.PropertyInfo.Name != "PONO" && typeProperty.PropertyInfo.Name != "PoYearCode"
                    && typeProperty.PropertyInfo.Name != "SchNo" && typeProperty.PropertyInfo.Name != "SchYearCode"
                    && typeProperty.PropertyInfo.Name != "PoType" && typeProperty.PropertyInfo.Name != "POAmendNo"
                    && typeProperty.PropertyInfo.Name != "PODate" && typeProperty.PropertyInfo.Name != "Unit"
                    && typeProperty.PropertyInfo.Name != "RateUnit" && typeProperty.PropertyInfo.Name != "AltUnit"
                    && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "AltQty"
                    && typeProperty.PropertyInfo.Name != "NoOfCase" && typeProperty.PropertyInfo.Name != "BillQty"
                    && typeProperty.PropertyInfo.Name != "RecQty" && typeProperty.PropertyInfo.Name != "AltRecQty"
                    && typeProperty.PropertyInfo.Name != "ShortExcessQty" && typeProperty.PropertyInfo.Name != "Rate"
                    && typeProperty.PropertyInfo.Name != "RateinOther" && typeProperty.PropertyInfo.Name != "Amount"
                    && typeProperty.PropertyInfo.Name != "PendPOQty" && typeProperty.PropertyInfo.Name != "QCCompleted"
                    && typeProperty.PropertyInfo.Name != "RetChallanPendQty" && typeProperty.PropertyInfo.Name != "BatchWise"
                    && typeProperty.PropertyInfo.Name != "SaleBillNo" && typeProperty.PropertyInfo.Name != "SaleBillYearCode"
                    && typeProperty.PropertyInfo.Name != "AgainstChallanNo" && typeProperty.PropertyInfo.Name != "BatchNo"
                    && typeProperty.PropertyInfo.Name != "UniqueBatchNo" && typeProperty.PropertyInfo.Name != "SupplierBatchNo"
                    && typeProperty.PropertyInfo.Name != "ShelfLife" && typeProperty.PropertyInfo.Name != "ItemSize"
                    && typeProperty.PropertyInfo.Name != "ItemColor" && typeProperty.PropertyInfo.Name != "TotalRecords"
                    && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "PageSize"
                    && typeProperty.PropertyInfo.Name != "FromMRNNo" && typeProperty.PropertyInfo.Name != "ToMRNNo"
                    )

                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "CustomerJWR")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "CustomerJWRQDashboard" && typeProperty.PropertyInfo.Name != "EID"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "MRNList" && typeProperty.PropertyInfo.Name != "PartCode"
                    && typeProperty.PropertyInfo.Name != "RecPartCode" && typeProperty.PropertyInfo.Name != "RecItemName" && typeProperty.PropertyInfo.Name != "Billqty" && typeProperty.PropertyInfo.Name != "RecQty"
                    && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "RecAltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Rate"
                    && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "ShortExcessQty" && typeProperty.PropertyInfo.Name != "ItemRemark" && typeProperty.PropertyInfo.Name != "Purpose"
                    && typeProperty.PropertyInfo.Name != "FinishedItemCode" && typeProperty.PropertyInfo.Name != "FinishedQty" && typeProperty.PropertyInfo.Name != "PendQty" && typeProperty.PropertyInfo.Name != "RecScrap"
                    && typeProperty.PropertyInfo.Name != "AllowedRejPer" && typeProperty.PropertyInfo.Name != "ProcessId" && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "ItemQcCompleted"
                    && typeProperty.PropertyInfo.Name != "CustBatchno" && typeProperty.PropertyInfo.Name != "batchno" && typeProperty.PropertyInfo.Name != "UniqueBatchNo" && typeProperty.PropertyInfo.Name != "SoNo"
                    && typeProperty.PropertyInfo.Name != "SoYearCode" && typeProperty.PropertyInfo.Name != "CustOrderno" && typeProperty.PropertyInfo.Name != "SOSchNo" && typeProperty.PropertyInfo.Name != "SOSchYearCode"
                    && typeProperty.PropertyInfo.Name != "SODate" && typeProperty.PropertyInfo.Name != "SchDate" && typeProperty.PropertyInfo.Name != "bomno" && typeProperty.PropertyInfo.Name != "BomName" && typeProperty.PropertyInfo.Name != "BomDate"
                    && typeProperty.PropertyInfo.Name != "INDBOM" && typeProperty.PropertyInfo.Name != "FGITEMCODE" && typeProperty.PropertyInfo.Name != "FGPartCode" && typeProperty.PropertyInfo.Name != "FGItemName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "CustomerJWI")
                {
                    if (row.Table.Columns.Contains(typeProperty.PropertyInfo.Name))
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ISSVendJW")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "ChallanList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "JWIssQDashboard"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ChallanList"
                    && typeProperty.PropertyInfo.Name != "Types" && typeProperty.PropertyInfo.Name != "TimeOfRemoval" && typeProperty.PropertyInfo.Name != "JobWorkNewRework"
                    && typeProperty.PropertyInfo.Name != "TransporterName" && typeProperty.PropertyInfo.Name != "VehicleNo" && typeProperty.PropertyInfo.Name != "DispatchTo"
                    && typeProperty.PropertyInfo.Name != "HsnNo" && typeProperty.PropertyInfo.Name != "IssQty" && typeProperty.PropertyInfo.Name != "Unit"
                    && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "PurchasePrice"
                    && typeProperty.PropertyInfo.Name != "StageDescription" && typeProperty.PropertyInfo.Name != "Store" && typeProperty.PropertyInfo.Name != "PendQty"
                    && typeProperty.PropertyInfo.Name != "ScrapPartCode" && typeProperty.PropertyInfo.Name != "ScrapItemName" && typeProperty.PropertyInfo.Name != "RecScrapQty"
                    && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "CC"
                    && typeProperty.PropertyInfo.Name != "PendAltQty" && typeProperty.PropertyInfo.Name != "UpdatedDate" && typeProperty.PropertyInfo.Name != "EntryByMachineName"
                    && typeProperty.PropertyInfo.Name != "Process" && typeProperty.PropertyInfo.Name != "RecQty"
                    && typeProperty.PropertyInfo.Name != "RecItemName" && typeProperty.PropertyInfo.Name != "ItemDetailGrid")

                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RoutingTable")
                {
                    if (typeProperty.PropertyInfo.Name != "RoutingGrid")
                    //&& typeProperty.PropertyInfo.Name != "VendorName")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ProductionSchTable")
                {
                    if (typeProperty.PropertyInfo.Name != "productionScheduleDashboards" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "SearchBox" 
                        && typeProperty.PropertyInfo.Name != "EffectiveTill" && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "FinToDate"
                        && typeProperty.PropertyInfo.Name != "FinToDate" && typeProperty.PropertyInfo.Name != "ProductionScheduleDetails" && typeProperty.PropertyInfo.Name != "ProdSchNoBack"
                        && typeProperty.PropertyInfo.Name != "FromDateBack" && typeProperty.PropertyInfo.Name != "ToDateBack" && typeProperty.PropertyInfo.Name != "PartCodeBack" && typeProperty.PropertyInfo.Name != "ItemNameBack"
                        && typeProperty.PropertyInfo.Name != "AccountNameBack" && typeProperty.PropertyInfo.Name != "WONOBack" && typeProperty.PropertyInfo.Name != "SummaryDetailBack" && typeProperty.PropertyInfo.Name != "SearchBoxBack"
                        && typeProperty.PropertyInfo.Name != "prodPlanDetails" && typeProperty.PropertyInfo.Name != "StartFromTime" && typeProperty.PropertyInfo.Name != "ToTime"
                        && typeProperty.PropertyInfo.Name != "WOEntryId" && typeProperty.PropertyInfo.Name != "SOEntryId"
                    && typeProperty.PropertyInfo.Name != "Active" && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "Mode"
                    && typeProperty.PropertyInfo.Name != "UpdatedOn" && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "CreatedOn"
                    && typeProperty.PropertyInfo.Name != "ID" && typeProperty.PropertyInfo.Name != "BomDatamodel" && typeProperty.PropertyInfo.Name != "WONo" && typeProperty.PropertyInfo.Name != "WOYearCode" && typeProperty.PropertyInfo.Name != "WODate" && typeProperty.PropertyInfo.Name != "NoOfCavity" && typeProperty.PropertyInfo.Name != "NoOfshotsHours")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "VendorUserDashboard" || table.TableName == "VendorUserDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "VendorUserDashboards" && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "FinToDate"
                        && typeProperty.PropertyInfo.Name != "FinToDate" && typeProperty.PropertyInfo.Name != "Active"
                                && typeProperty.PropertyInfo.Name != "CreatedBy"
                                && typeProperty.PropertyInfo.Name != "CreatedOn"
                                && typeProperty.PropertyInfo.Name != "EID"
                                && typeProperty.PropertyInfo.Name != "ID"
                                && typeProperty.PropertyInfo.Name != "Mode"
                                && typeProperty.PropertyInfo.Name != "TxPageName"
                                && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "ConfirmPassword"
                                && typeProperty.PropertyInfo.Name != "UpdatedOn")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleBillSummTable")
                {
                    if (typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "saleBillDashboard" && typeProperty.PropertyInfo.Name != "StateCode"
                        && typeProperty.PropertyInfo.Name != "saleBillDetails" && typeProperty.PropertyInfo.Name != "ItemDetailGrid" && typeProperty.PropertyInfo.Name != "_YesNo" && typeProperty.PropertyInfo.Name != "YesNoList"
                        && typeProperty.PropertyInfo.Name != "TransporterId" && typeProperty.PropertyInfo.Name != "ConsigneeAccountcode" && typeProperty.PropertyInfo.Name != "PNConsingee"
                        && typeProperty.PropertyInfo.Name != "InvPrefix" && typeProperty.PropertyInfo.Name != "PN1" && typeProperty.PropertyInfo.Name != "DocTypeAccountCode"
                        && typeProperty.PropertyInfo.Name != "DocTypeAccountCode" && typeProperty.PropertyInfo.Name != "DocTypeAccountName" && typeProperty.PropertyInfo.Name != "TaxbaleAmtInWord"
                         && typeProperty.PropertyInfo.Name != "BillAmtWord" && typeProperty.PropertyInfo.Name != "TaxableAmt" && typeProperty.PropertyInfo.Name != "RoundTypea"
                        && typeProperty.PropertyInfo.Name != "DiscountPercent" && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "NetAmtInWords" && typeProperty.PropertyInfo.Name != "PermitNo"
                        && typeProperty.PropertyInfo.Name != "CashDisPer" && typeProperty.PropertyInfo.Name != "CashDisRs" && typeProperty.PropertyInfo.Name != "SoDelTime" && typeProperty.PropertyInfo.Name != "TypeJob"
                        && typeProperty.PropertyInfo.Name != "CurrencyId" && typeProperty.PropertyInfo.Name != "Shippingdate" && typeProperty.PropertyInfo.Name != "Cancelreason"
                        && typeProperty.PropertyInfo.Name != "BankName"
                        && typeProperty.PropertyInfo.Name != "SOType" && typeProperty.PropertyInfo.Name != "EntryByempId" && typeProperty.PropertyInfo.Name != "ActualEnteredBy" && typeProperty.PropertyInfo.Name != "LastUpdatedByName"
                        && typeProperty.PropertyInfo.Name != "ChallanNo" && typeProperty.PropertyInfo.Name != "ChallanDate" && typeProperty.PropertyInfo.Name != "ChallanEntryid" && typeProperty.PropertyInfo.Name != "ChallanYearCode"
                        && typeProperty.PropertyInfo.Name != "SaleQuotEntryID" && typeProperty.PropertyInfo.Name != "TotalRoundOff" && typeProperty.PropertyInfo.Name != "TotalDiscountPercentage" && typeProperty.PropertyInfo.Name != "FinToDate"
                        && typeProperty.PropertyInfo.Name != "ItemSA" && typeProperty.PropertyInfo.Name != "dispatchLocation" && typeProperty.PropertyInfo.Name != "NetTotal" && typeProperty.PropertyInfo.Name != "BomNo"
                        && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "TotalAmtAftrDiscount" && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "SONO" && typeProperty.PropertyInfo.Name != "CustOrderNo"
                        && typeProperty.PropertyInfo.Name != "SOYearCode" && typeProperty.PropertyInfo.Name != "SODate" && typeProperty.PropertyInfo.Name != "SchNo" && typeProperty.PropertyInfo.Name != "Schdate" && typeProperty.PropertyInfo.Name != "SaleSchYearCode"
                        && typeProperty.PropertyInfo.Name != "SOAmendNo" && typeProperty.PropertyInfo.Name != "SOAmendDate" && typeProperty.PropertyInfo.Name != "SchAmendNo" && typeProperty.PropertyInfo.Name != "SchAmendDate" && typeProperty.PropertyInfo.Name != "ItemCode"
                        && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "HSNNo" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "NoofCase" && typeProperty.PropertyInfo.Name != "Qty"
                        && typeProperty.PropertyInfo.Name != "UnitOfRate" && typeProperty.PropertyInfo.Name != "RateInOtherCurr" && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "AltQty"
                        && typeProperty.PropertyInfo.Name != "ItemWeight" && typeProperty.PropertyInfo.Name != "NoofPcs" && typeProperty.PropertyInfo.Name != "CustomerPartCode" && typeProperty.PropertyInfo.Name != "MRP" && typeProperty.PropertyInfo.Name != "OriginalMRP" && typeProperty.PropertyInfo.Name != "SOPendQty"
                        && typeProperty.PropertyInfo.Name != "AltSOPendQty" && typeProperty.PropertyInfo.Name != "DiscountPer" && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "ItemSize" && typeProperty.PropertyInfo.Name != "Itemcolor" && typeProperty.PropertyInfo.Name != "StoreId"
                        && typeProperty.PropertyInfo.Name != "StoreName" && typeProperty.PropertyInfo.Name != "ItemAmount" && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "AdviceNo" && typeProperty.PropertyInfo.Name != "AdviseEntryId" && typeProperty.PropertyInfo.Name != "AdviceYearCode"
                        && typeProperty.PropertyInfo.Name != "AdviseDate" && typeProperty.PropertyInfo.Name != "ProcessId" && typeProperty.PropertyInfo.Name != "Batchno" && typeProperty.PropertyInfo.Name != "Uniquebatchno" && typeProperty.PropertyInfo.Name != "LotStock"
                        && typeProperty.PropertyInfo.Name != "TotalStock" && typeProperty.PropertyInfo.Name != "AgainstProdPlanNo" && typeProperty.PropertyInfo.Name != "AgainstProdPlanYearCode" && typeProperty.PropertyInfo.Name != "AgaisntProdPlanDate" && typeProperty.PropertyInfo.Name != "GSTPer" && typeProperty.PropertyInfo.Name != "GSTType"
                        && typeProperty.PropertyInfo.Name != "PacketsDetail" && typeProperty.PropertyInfo.Name != "OtherDetail" && typeProperty.PropertyInfo.Name != "ItemRemark" && typeProperty.PropertyInfo.Name != "SH" && typeProperty.PropertyInfo.Name != "ProdSchno"
                        && typeProperty.PropertyInfo.Name != "CostCenterId" && typeProperty.PropertyInfo.Name != "ProdSchYearcode" && typeProperty.PropertyInfo.Name != "ProdSchEntryId" && typeProperty.PropertyInfo.Name != "ProdSchDate"
                        && typeProperty.PropertyInfo.Name != "SchdeliveryDate" && typeProperty.PropertyInfo.Name != "ItemNetAmount" && typeProperty.PropertyInfo.Name != "currExchangeRate" && typeProperty.PropertyInfo.Name != "currencyId"

                       && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "AgstInvNo" && typeProperty.PropertyInfo.Name != "AgstInvDate"
                       && typeProperty.PropertyInfo.Name != "SaleQuotyearCode" && typeProperty.PropertyInfo.Name != "TotalRoundOffAmt" && typeProperty.PropertyInfo.Name != "AgstInvYearCode"
                       && typeProperty.PropertyInfo.Name != "TaxDetailGridd" && typeProperty.PropertyInfo.Name != "TaxList" && typeProperty.PropertyInfo.Name != "DT" && typeProperty.PropertyInfo.Name != "Group_Code"

                       && typeProperty.PropertyInfo.Name != "ToDate1" && typeProperty.PropertyInfo.Name != "FromDate1"
                       && typeProperty.PropertyInfo.Name != "FromDateBack" && typeProperty.PropertyInfo.Name != "ToDateBack" && typeProperty.PropertyInfo.Name != "PartCodeBack" && typeProperty.PropertyInfo.Name != "ItemNameBack" && typeProperty.PropertyInfo.Name != "SaleBillNoBack" && typeProperty.PropertyInfo.Name != "CustNameBack" && typeProperty.PropertyInfo.Name != "SonoBack" && typeProperty.PropertyInfo.Name != "CustOrderNoBack"
                        && typeProperty.PropertyInfo.Name != "SchNoBack" && typeProperty.PropertyInfo.Name != "Searchbox"
                        && typeProperty.PropertyInfo.Name != "PerformaInvNoBack" && typeProperty.PropertyInfo.Name != "CustomerJobWorkChallanAdj"
                        && typeProperty.PropertyInfo.Name != "SaleQuoteNoBack" && typeProperty.PropertyInfo.Name != "SaleBillDataDashboard"
                        && typeProperty.PropertyInfo.Name != "DomesticExportNEPZBack"
                        && typeProperty.PropertyInfo.Name != "SearchBoxBack" && typeProperty.PropertyInfo.Name != "SummaryDetailBack"
                        && typeProperty.PropertyInfo.Name != "MaxSaleInvoiceEntryDate" && typeProperty.PropertyInfo.Name != "MaxSaleInvoiceEntryDate"
                        && typeProperty.PropertyInfo.Name != "AllowBackDateSALEBILL" && typeProperty.PropertyInfo.Name != "AllowBackDateSALEBILL"

                       && typeProperty.PropertyInfo.Name != "TotalTaxAmt" && typeProperty.PropertyInfo.Name != "Active"
                        && typeProperty.PropertyInfo.Name != "TxAccountCode" && typeProperty.PropertyInfo.Name != "DPBItemDetails"
                        && typeProperty.PropertyInfo.Name != "TxAccountName" && typeProperty.PropertyInfo.Name != "adjustmentModel"
                        && typeProperty.PropertyInfo.Name != "TxAdInTxable" && typeProperty.PropertyInfo.Name != "ProcessName"
                        && typeProperty.PropertyInfo.Name != "TxAmount" && typeProperty.PropertyInfo.Name != "CostCenterName"
                        && typeProperty.PropertyInfo.Name != "TxItemCode" && typeProperty.PropertyInfo.Name != "VendJwAdjustmentMandatory" && typeProperty.PropertyInfo.Name != "StockableNonStockable"
                        && typeProperty.PropertyInfo.Name != "TxItemName" && typeProperty.PropertyInfo.Name != "CustJwAdjustmentMandatory"
                        && typeProperty.PropertyInfo.Name != "TxOnExp"
                        && typeProperty.PropertyInfo.Name != "TxPartCode"
                        && typeProperty.PropertyInfo.Name != "TxPartName"
                        && typeProperty.PropertyInfo.Name != "TxPercentg"
                        && typeProperty.PropertyInfo.Name != "TxRefundable"
                        && typeProperty.PropertyInfo.Name != "TxRemark"
                        && typeProperty.PropertyInfo.Name != "TxRoundOff"
                        && typeProperty.PropertyInfo.Name != "TxSeqNo"
                        && typeProperty.PropertyInfo.Name != "RackID"
                        && typeProperty.PropertyInfo.Name != "AdditionalDiscount"
                        && typeProperty.PropertyInfo.Name != "AllowToChangeSaleBillStoreName"
                        && typeProperty.PropertyInfo.Name != "PackingCharges"
                        && typeProperty.PropertyInfo.Name != "ForwardingCharges"
                        && typeProperty.PropertyInfo.Name != "CourieerCharges"
                        && typeProperty.PropertyInfo.Name != "GST"
                        && typeProperty.PropertyInfo.Name != "PrivateMark"
                        && typeProperty.PropertyInfo.Name != "GRNo"
                        && typeProperty.PropertyInfo.Name != "GRDate"
                        && typeProperty.PropertyInfo.Name != "BillNo"
                        && typeProperty.PropertyInfo.Name != "BillDate"
                        && typeProperty.PropertyInfo.Name != "CustomerName"
                        && typeProperty.PropertyInfo.Name != "AllowToAdjZeroAmt"
                        && typeProperty.PropertyInfo.Name != "TxTaxType" && typeProperty.PropertyInfo.Name != "TotalRecords"
                        && typeProperty.PropertyInfo.Name != "TxTaxTypeName" && typeProperty.PropertyInfo.Name != "BOMInd" && typeProperty.PropertyInfo.Name != "ProducedUnprod"
                        && typeProperty.PropertyInfo.Name != "TxType" && typeProperty.PropertyInfo.Name != "DashboardTypeBack"
                        && typeProperty.PropertyInfo.Name != "YesNo" && typeProperty.PropertyInfo.Name != "AttachmentFile1" && typeProperty.PropertyInfo.Name != "AttachmentFile2" && typeProperty.PropertyInfo.Name != "AttachmentFile3"
                        && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID" && typeProperty.PropertyInfo.Name != "Mode"
                        && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "UpdatedOn"
                        && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "SummaryDetailBack" && typeProperty.PropertyInfo.Name != "PageSize" && typeProperty.PropertyInfo.Name != "TotalRecords" && typeProperty.PropertyInfo.Name != "Group_name"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleBillDetailTable")
                {
                    if (typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "saleBillDashboard" && typeProperty.PropertyInfo.Name != "StateCode"
                    && typeProperty.PropertyInfo.Name != "saleBillDetails" && typeProperty.PropertyInfo.Name != "ItemDetailGrid" && typeProperty.PropertyInfo.Name != "_YesNo" && typeProperty.PropertyInfo.Name != "YesNoList"
                    && typeProperty.PropertyInfo.Name != "TransporterId" && typeProperty.PropertyInfo.Name != "ConsigneeAccountcode" && typeProperty.PropertyInfo.Name != "PNConsingee" && typeProperty.PropertyInfo.Name != "SOType"
                    && typeProperty.PropertyInfo.Name != "InvPrefix" && typeProperty.PropertyInfo.Name != "PN1" && typeProperty.PropertyInfo.Name != "DocTypeAccountCode"
                     && typeProperty.PropertyInfo.Name != "MaxSaleInvoiceEntryDate" && typeProperty.PropertyInfo.Name != "MaxSaleInvoiceEntryDate"
                      && typeProperty.PropertyInfo.Name != "AllowBackDateSALEBILL" && typeProperty.PropertyInfo.Name != "AllowBackDateSALEBILL"

                    && typeProperty.PropertyInfo.Name != "DocTypeAccountName" && typeProperty.PropertyInfo.Name != "TaxbaleAmtInWord" && typeProperty.PropertyInfo.Name != "SaleSchYearCode"
                    && typeProperty.PropertyInfo.Name != "BillAmtWord" && typeProperty.PropertyInfo.Name != "RoundTypea"
                    && typeProperty.PropertyInfo.Name != "DiscountPercent" && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "NetAmtInWords" && typeProperty.PropertyInfo.Name != "PermitNo"
                    && typeProperty.PropertyInfo.Name != "CashDisPer" && typeProperty.PropertyInfo.Name != "CashDisRs" && typeProperty.PropertyInfo.Name != "SoDelTime" && typeProperty.PropertyInfo.Name != "TypeJob"
                    && typeProperty.PropertyInfo.Name != "currencyId" && typeProperty.PropertyInfo.Name != "ChallanNo" && typeProperty.PropertyInfo.Name != "ChallanDate" && typeProperty.PropertyInfo.Name != "ChallanEntryid" && typeProperty.PropertyInfo.Name != "ChallanYearCode"
                    && typeProperty.PropertyInfo.Name != "SaleQuotEntryID" && typeProperty.PropertyInfo.Name != "TotalRoundOff" && typeProperty.PropertyInfo.Name != "TotalDiscountPercentage" && typeProperty.PropertyInfo.Name != "FinToDate"
                    && typeProperty.PropertyInfo.Name != "ItemSA" && typeProperty.PropertyInfo.Name != "dispatchLocation" && typeProperty.PropertyInfo.Name != "NetTotal"
                    && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "TotalAmtAftrDiscount" && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "Schdate" && typeProperty.PropertyInfo.Name != "SOYearCode"
                    && typeProperty.PropertyInfo.Name != "SOAmendNo" && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "PartCodeBack" && typeProperty.PropertyInfo.Name != "ItemNameBack"
                    && typeProperty.PropertyInfo.Name != "SaleBillNoBack" && typeProperty.PropertyInfo.Name != "CustNameBack" && typeProperty.PropertyInfo.Name != "SonoBack"
                    && typeProperty.PropertyInfo.Name != "CustOrderNoBack" && typeProperty.PropertyInfo.Name != "SchNoBack" && typeProperty.PropertyInfo.Name != "Searchbox"
                    && typeProperty.PropertyInfo.Name != "PerformaInvNoBack" && typeProperty.PropertyInfo.Name != "SaleQuoteNoBack" && typeProperty.PropertyInfo.Name != "DomesticExportNEPZBack"
                    && typeProperty.PropertyInfo.Name != "SearchBoxBack" && typeProperty.PropertyInfo.Name != "TotalTaxAmt" && typeProperty.PropertyInfo.Name != "Active"
                    && typeProperty.PropertyInfo.Name != "TxAccountCode" && typeProperty.PropertyInfo.Name != "DPBItemDetails" && typeProperty.PropertyInfo.Name != "AgstInvDate" && typeProperty.PropertyInfo.Name != "AgstInvYearCode"
                    && typeProperty.PropertyInfo.Name != "TxAccountName" && typeProperty.PropertyInfo.Name != "adjustmentModel" && typeProperty.PropertyInfo.Name != "AgstInvNo"
                    && typeProperty.PropertyInfo.Name != "TxAdInTxable" && typeProperty.PropertyInfo.Name != "ProcessName" && typeProperty.PropertyInfo.Name != "BILLAgainstWarrenty"
                    && typeProperty.PropertyInfo.Name != "TxAmount" && typeProperty.PropertyInfo.Name != "CostCenterName" && typeProperty.PropertyInfo.Name != "RemovalDate" && typeProperty.PropertyInfo.Name != "RemovalTime"
                    && typeProperty.PropertyInfo.Name != "TxItemCode" && typeProperty.PropertyInfo.Name != "TxItemName" && typeProperty.PropertyInfo.Name != "PN1" && typeProperty.PropertyInfo.Name != "DT"
                    && typeProperty.PropertyInfo.Name != "TxOnExp" && typeProperty.PropertyInfo.Name != "TxPartCode" && typeProperty.PropertyInfo.Name != "AccountCode"
                    && typeProperty.PropertyInfo.Name != "TxPartName" && typeProperty.PropertyInfo.Name != "TxPercentg" && typeProperty.PropertyInfo.Name != "CustomerJobWorkChallanAdj"
                    && typeProperty.PropertyInfo.Name != "TxRefundable" && typeProperty.PropertyInfo.Name != "TxRemark" && typeProperty.PropertyInfo.Name != "currExchangeRate"
                    && typeProperty.PropertyInfo.Name != "TxRoundOff" && typeProperty.PropertyInfo.Name != "TxSeqNo" && typeProperty.PropertyInfo.Name != "EntryByempId"
                    && typeProperty.PropertyInfo.Name != "TxTaxType" && typeProperty.PropertyInfo.Name != "TxTaxTypeName" && typeProperty.PropertyInfo.Name != "BomNo"
                    && typeProperty.PropertyInfo.Name != "TxType" && typeProperty.PropertyInfo.Name != "YesNo" && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "LastUpdationDate"
                    && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedOn" && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID" && typeProperty.PropertyInfo.Name != "Mode"
                    && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "UpdatedOn"
                    && typeProperty.PropertyInfo.Name != "ToDate1" && typeProperty.PropertyInfo.Name != "FromDate1" && typeProperty.PropertyInfo.Name != "TypeItemServAssets"
                     && typeProperty.PropertyInfo.Name != "PerformaInvNo" && typeProperty.PropertyInfo.Name != "PerformaInvDate"
                    && typeProperty.PropertyInfo.Name != "PerformaInvYearCode" && typeProperty.PropertyInfo.Name != "BILLAgainstWarrenty" && typeProperty.PropertyInfo.Name != "ExportInvoiceNo"
                    && typeProperty.PropertyInfo.Name != "InvoiceTime" && typeProperty.PropertyInfo.Name != "RemovalDate" && typeProperty.PropertyInfo.Name != "SaleQuotDate"
                    && typeProperty.PropertyInfo.Name != "BalanceSheetClosed" && typeProperty.PropertyInfo.Name != "ActualEnteredBy"
                    && typeProperty.PropertyInfo.Name != "SaleQuotNo" && typeProperty.PropertyInfo.Name != "EntryFreezToAccounts"
                      && typeProperty.PropertyInfo.Name != "SaleQuotyearCode" && typeProperty.PropertyInfo.Name != "Amount"
                          && typeProperty.PropertyInfo.Name != "SaleQuotDate" && typeProperty.PropertyInfo.Name != "SOAmendDate" && typeProperty.PropertyInfo.Name != "MRP" && typeProperty.PropertyInfo.Name != "OriginalMRP"
                          && typeProperty.PropertyInfo.Name != "FinFromDate" && typeProperty.PropertyInfo.Name != "ProcessName"
                          && typeProperty.PropertyInfo.Name != "FinToDate" && typeProperty.PropertyInfo.Name != "UnitOfRate" && typeProperty.PropertyInfo.Name != "ItemWeight"
                          && typeProperty.PropertyInfo.Name != "NetTotal" && typeProperty.PropertyInfo.Name != "NoofPcs" && typeProperty.PropertyInfo.Name != "SH"
                          && typeProperty.PropertyInfo.Name != "TotalAmtAftrDiscount" && typeProperty.PropertyInfo.Name != "CostCenterName"
                          && typeProperty.PropertyInfo.Name != "TotalDiscountPercentage" && typeProperty.PropertyInfo.Name != "ItemNetAmount"
                          && typeProperty.PropertyInfo.Name != "TotalRoundOff" && typeProperty.PropertyInfo.Name != "TaxDetailGridd" && typeProperty.PropertyInfo.Name != "DashboardTypeBack"
                          && typeProperty.PropertyInfo.Name != "TotalRoundOffAmt" && typeProperty.PropertyInfo.Name != "FromDateBack" && typeProperty.PropertyInfo.Name != "ToDateBack" && typeProperty.PropertyInfo.Name != "PartCodeBack" && typeProperty.PropertyInfo.Name != "ItemNameBack" && typeProperty.PropertyInfo.Name != "SaleBillNoBack" && typeProperty.PropertyInfo.Name != "CustNameBack" && typeProperty.PropertyInfo.Name != "SonoBack" && typeProperty.PropertyInfo.Name != "CustOrderNoBack"
                        && typeProperty.PropertyInfo.Name != "SchNoBack" && typeProperty.PropertyInfo.Name != "Searchbox"
                        && typeProperty.PropertyInfo.Name != "PerformaInvNoBack" && typeProperty.PropertyInfo.Name != "TaxList" && typeProperty.PropertyInfo.Name != "DT"
                        && typeProperty.PropertyInfo.Name != "SaleQuoteNoBack" && typeProperty.PropertyInfo.Name != "DashboardTypeBack" && typeProperty.PropertyInfo.Name != "BOMInd"
                        && typeProperty.PropertyInfo.Name != "DomesticExportNEPZBack" && typeProperty.PropertyInfo.Name != "AttachmentFile1" && typeProperty.PropertyInfo.Name != "AttachmentFile2" && typeProperty.PropertyInfo.Name != "AttachmentFile3"
                        && typeProperty.PropertyInfo.Name != "SearchBoxBack" && typeProperty.PropertyInfo.Name != "SummaryDetailBack" && typeProperty.PropertyInfo.Name != "SaleBillDataDashboard"
                        && typeProperty.PropertyInfo.Name != "PageNumber" && typeProperty.PropertyInfo.Name != "SummaryDetailBack" && typeProperty.PropertyInfo.Name != "PageSize" && typeProperty.PropertyInfo.Name != "TotalRecords" && typeProperty.PropertyInfo.Name != "ProducedUnprod" && typeProperty.PropertyInfo.Name != "CustJwAdjustmentMandatory" && typeProperty.PropertyInfo.Name != "StockableNonStockable" && typeProperty.PropertyInfo.Name != "VendJwAdjustmentMandatory" && typeProperty.PropertyInfo.Name != "Group_Code" && typeProperty.PropertyInfo.Name != "Group_name"
                        && typeProperty.PropertyInfo.Name != "ConsigneeAccountName"
                        && typeProperty.PropertyInfo.Name != "RackID"
                        && typeProperty.PropertyInfo.Name != "AdditionalDiscount"
                        && typeProperty.PropertyInfo.Name != "AllowToChangeSaleBillStoreName"
                        && typeProperty.PropertyInfo.Name != "PackingCharges"
                        && typeProperty.PropertyInfo.Name != "ForwardingCharges"
                        && typeProperty.PropertyInfo.Name != "CourieerCharges"
                        && typeProperty.PropertyInfo.Name != "GST"
                         && typeProperty.PropertyInfo.Name != "PrivateMark"
                        && typeProperty.PropertyInfo.Name != "GRNo"
                        && typeProperty.PropertyInfo.Name != "GRDate"
                        && typeProperty.PropertyInfo.Name != "AllowToAdjZeroAmt"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleRejectionSummTable")
                {
                    if (typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "SH"
                        && typeProperty.PropertyInfo.Name != "saleRejectionDashboard" && typeProperty.PropertyInfo.Name != "RejRate"
                         && typeProperty.PropertyInfo.Name != "PN1" && typeProperty.PropertyInfo.Name != "AltQty"
                        && typeProperty.PropertyInfo.Name != "DT" && typeProperty.PropertyInfo.Name != "MRNEntryId"
                          && typeProperty.PropertyInfo.Name != "CurrencyId" && typeProperty.PropertyInfo.Name != "SearchBox"
                        && typeProperty.PropertyInfo.Name != "EntryByempId" && typeProperty.PropertyInfo.Name != "LastUpdatedBy" && typeProperty.PropertyInfo.Name != "FinFromDate"
                        && typeProperty.PropertyInfo.Name != "FinToDate" && typeProperty.PropertyInfo.Name != "NetTotal" && typeProperty.PropertyInfo.Name != "TotalAmtAftrDiscount"
                        && typeProperty.PropertyInfo.Name != "TotalDiscountPercentage" && typeProperty.PropertyInfo.Name != "TotalRoundOff" && typeProperty.PropertyInfo.Name != "TotalRoundOffAmt"
                        && typeProperty.PropertyInfo.Name != "SaleRejectionInputGrid" && typeProperty.PropertyInfo.Name != "SaleRejectionDetails"
                        && typeProperty.PropertyInfo.Name != "ItemDetailGrid" && typeProperty.PropertyInfo.Name != "DPBItemDetails" && typeProperty.PropertyInfo.Name != "adjustmentModel"
                         && typeProperty.PropertyInfo.Name != "AgainstBillTypeJWSALE" && typeProperty.PropertyInfo.Name != "AgainstBillNo"
                         && typeProperty.PropertyInfo.Name != "AgainstBillYearCode" && typeProperty.PropertyInfo.Name != "AgainstBillEntryId"
                         && typeProperty.PropertyInfo.Name != "AgainstOpnOrBill" && typeProperty.PropertyInfo.Name != "DocTypeAccountCode"
                         && typeProperty.PropertyInfo.Name != "ItemCode" && typeProperty.PropertyInfo.Name != "ItemName"
                         && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "ItemSA"
                         && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "HSNNo" && typeProperty.PropertyInfo.Name != "NoOfCase"
                         && typeProperty.PropertyInfo.Name != "SaleBillQty" && typeProperty.PropertyInfo.Name != "RejQty"
                         && typeProperty.PropertyInfo.Name != "RecQty" && typeProperty.PropertyInfo.Name != "Rate" && typeProperty.PropertyInfo.Name != "DiscountPer"
                         && typeProperty.PropertyInfo.Name != "DiscountAmt" && typeProperty.PropertyInfo.Name != "SONO"
                         && typeProperty.PropertyInfo.Name != "SOyearcode" && typeProperty.PropertyInfo.Name != "SODate"
                         && typeProperty.PropertyInfo.Name != "CustOrderNo" && typeProperty.PropertyInfo.Name != "SOAmmNo" && typeProperty.PropertyInfo.Name != "Itemsize"
                         && typeProperty.PropertyInfo.Name != "RecStoreId" && typeProperty.PropertyInfo.Name != "RecStoreName"
                         && typeProperty.PropertyInfo.Name != "OtherDetail" && typeProperty.PropertyInfo.Name != "Amount" && typeProperty.PropertyInfo.Name != "RejectionReason"
                         && typeProperty.PropertyInfo.Name != "SaleorderRemark" && typeProperty.PropertyInfo.Name != "SaleBillremark"
                         && typeProperty.PropertyInfo.Name != "ItemNetAmount"
                        // && typeProperty.PropertyInfo.Name != "RoundOffAmt"
                        //&& typeProperty.PropertyInfo.Name != "Ewaybillno" && typeProperty.PropertyInfo.Name != "EInvNo" && typeProperty.PropertyInfo.Name != "Ewaybillno"
                        //&& typeProperty.PropertyInfo.Name != "EinvGenerated" && typeProperty.PropertyInfo.Name != "ActualEntryDate"
                        // && typeProperty.PropertyInfo.Name != "LastUpdatedByName" && typeProperty.PropertyInfo.Name != "LastUpdationDate"
                        // && typeProperty.PropertyInfo.Name != "TypeItemServAssets" && typeProperty.PropertyInfo.Name != "SaleBillJobwork"
                        //&& typeProperty.PropertyInfo.Name != "PerformaInvNo" && typeProperty.PropertyInfo.Name != "PerformaInvDate" && typeProperty.PropertyInfo.Name != "PerformaInvYearCode"
                        //&& typeProperty.PropertyInfo.Name != "BILLAgainstWarrenty" && typeProperty.PropertyInfo.Name != "ExportInvoiceNo" 
                        //&& typeProperty.PropertyInfo.Name != "InvoiceTime" && typeProperty.PropertyInfo.Name != "RemovalDate" 
                        //Manadatory
                        && typeProperty.PropertyInfo.Name != "YesNoList" && typeProperty.PropertyInfo.Name != "TaxDetailGridd" && typeProperty.PropertyInfo.Name != "TaxList"
                        && typeProperty.PropertyInfo.Name != "TotalTaxAmt" && typeProperty.PropertyInfo.Name != "TxAccountCode" && typeProperty.PropertyInfo.Name != "TxAccountName" && typeProperty.PropertyInfo.Name != "TxAdInTxable" && typeProperty.PropertyInfo.Name != "TxAmount"
                         && typeProperty.PropertyInfo.Name != "TxItemCode" && typeProperty.PropertyInfo.Name != "TxItemName" && typeProperty.PropertyInfo.Name != "TxOnExp"
                         && typeProperty.PropertyInfo.Name != "TxPartCode" && typeProperty.PropertyInfo.Name != "TxPartName" && typeProperty.PropertyInfo.Name != "TxPercentg" && typeProperty.PropertyInfo.Name != "TxRefundable"
                         && typeProperty.PropertyInfo.Name != "TxRemark" && typeProperty.PropertyInfo.Name != "TxRoundOff" && typeProperty.PropertyInfo.Name != "TxSeqNo"
                         && typeProperty.PropertyInfo.Name != "TxTaxType" && typeProperty.PropertyInfo.Name != "TxTaxTypeName"
                         && typeProperty.PropertyInfo.Name != "TxType" && typeProperty.PropertyInfo.Name != "YesNo" && typeProperty.PropertyInfo.Name != "Active"
                         && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedOn"
                        && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "Mode"
                        && typeProperty.PropertyInfo.Name != "TxPageName" && typeProperty.PropertyInfo.Name != "UpdatedBy"
                        && typeProperty.PropertyInfo.Name != "UpdatedOn" && typeProperty.PropertyInfo.Name != "ID"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "SaleBillTable")
                {
                    if (typeProperty.PropertyInfo.Name != "saleBillDashboard" && typeProperty.PropertyInfo.Name != "SummaryDetail" && typeProperty.PropertyInfo.Name != "ToDate1"
                        && typeProperty.PropertyInfo.Name != "FromDate1" && typeProperty.PropertyInfo.Name != "ToDate1" && typeProperty.PropertyInfo.Name != "Searchbox"
                        && typeProperty.PropertyInfo.Name != "FromDateBack"
                        && typeProperty.PropertyInfo.Name != "ToDateBack"
                        && typeProperty.PropertyInfo.Name != "PartCodeBack"
                        && typeProperty.PropertyInfo.Name != "ItemNameBack"
                        && typeProperty.PropertyInfo.Name != "SaleBillNoBack"
                        && typeProperty.PropertyInfo.Name != "CustNameBack"
                        && typeProperty.PropertyInfo.Name != "SonoBack"
                        && typeProperty.PropertyInfo.Name != "CustOrderNoBack"
                        && typeProperty.PropertyInfo.Name != "SchNoBack"
                        && typeProperty.PropertyInfo.Name != "PerformaInvNoBack"
                        && typeProperty.PropertyInfo.Name != "SaleQuoteNoBack"
                        && typeProperty.PropertyInfo.Name != "DomesticExportNEPZBack"
                        && typeProperty.PropertyInfo.Name != "SearchBoxBack"
                        )
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RECVendJW")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "JWRecQDashboard"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "SearchMode"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "Unit"
                    && typeProperty.PropertyInfo.Name != "BillQty" && typeProperty.PropertyInfo.Name != "RecQty" && typeProperty.PropertyInfo.Name != "Amount"
                    && typeProperty.PropertyInfo.Name != "ProducedUnProd" && typeProperty.PropertyInfo.Name != "BomRevNo" && typeProperty.PropertyInfo.Name != "PONo"
                    && typeProperty.PropertyInfo.Name != "POYearCode" && typeProperty.PropertyInfo.Name != "PODate" && typeProperty.PropertyInfo.Name != "BatchNo"
                    && typeProperty.PropertyInfo.Name != "UniqueBatchNo")
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ReqDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "ReqMainDashboard"
                        && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "PendQty"
                        && typeProperty.PropertyInfo.Name != "ItemLocation" && typeProperty.PropertyInfo.Name != "ItemBinRackNo" && typeProperty.PropertyInfo.Name != "Location"
                        && typeProperty.PropertyInfo.Name != "BinNo" && typeProperty.PropertyInfo.Name != "SearchBox" && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "UpdatedBy")
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "RDMDashboard")
                {
                    if (row.Table.Columns.Contains(typeProperty.PropertyInfo.Name))
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ReqThruDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "ReqMainDashboard"
                        && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "PartCode" && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltUnit" && typeProperty.PropertyInfo.Name != "Location" && typeProperty.PropertyInfo.Name != "BinNo" && typeProperty.PropertyInfo.Name != "Qty" && typeProperty.PropertyInfo.Name != "AltQty" && typeProperty.PropertyInfo.Name != "PendQty"
                        && typeProperty.PropertyInfo.Name != "GlobalSearch" && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "FromDept")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "PartCodePartyWise" || table.TableName == "PartCodePartyWise")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "DashboardType"
                        && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "PartCodeList" &&
                        typeProperty.PropertyInfo.Name != "ItemNameList" && typeProperty.PropertyInfo.Name != "DataTable"
                      && typeProperty.PropertyInfo.Name != "DTDashboard" && typeProperty.PropertyInfo.Name != "ItemCode"
                      && typeProperty.PropertyInfo.Name != "EntryByMachineName" && typeProperty.PropertyInfo.Name != "Active"
                      && typeProperty.PropertyInfo.Name != "CreatedByName" && typeProperty.PropertyInfo.Name != "UpdatedByName"
                      && typeProperty.PropertyInfo.Name != "SummaryDetailBack" && typeProperty.PropertyInfo.Name != "GlobalSearchBack"
                      && typeProperty.PropertyInfo.Name != "ActualEnteredBy" && typeProperty.PropertyInfo.Name != "UpdatedByName"

                      && typeProperty.PropertyInfo.Name != "UpdatedBy" && typeProperty.PropertyInfo.Name != "UpdatedOn"
                      && typeProperty.PropertyInfo.Name != "CreatedBy" && typeProperty.PropertyInfo.Name != "CreatedOn"
                      && typeProperty.PropertyInfo.Name != "AccountName" && typeProperty.PropertyInfo.Name != "ActualEnteredByName"
                      && typeProperty.PropertyInfo.Name != "SeqNo" && typeProperty.PropertyInfo.Name != "Searchbox"
                      //&& typeProperty.PropertyInfo.Name != "PartCodePartyDashboard" && typeProperty.PropertyInfo.Name != "CreatedOn"
                      && typeProperty.PropertyInfo.Name != "EID" && typeProperty.PropertyInfo.Name != "ID"
                       && typeProperty.PropertyInfo.Name != "TxPageName"

                        && typeProperty.PropertyInfo.Name != "PartCodePartyDashboard" && typeProperty.PropertyInfo.Name != "GlobalSearch")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "ReqThru1Dashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "ReqThruMainDashboard"
                        && typeProperty.PropertyInfo.Name != "Mode")
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IssueWODashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "IssueWOBOMDashboard" && typeProperty.PropertyInfo.Name != "Mode"
                        && typeProperty.PropertyInfo.Name != "DashboardType" && typeProperty.PropertyInfo.Name != "ID"
                        )
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IssueThrDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "IssueThrBOMDashboard" && typeProperty.PropertyInfo.Name != "Mode")
                    {

                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "IssueThrSUMMDashboard")
                {
                    if (typeProperty.PropertyInfo.Name != "IssueThrBOMDashboard" &&
                        typeProperty.PropertyInfo.Name != "RecEmpCode" &&
                        typeProperty.PropertyInfo.Name != "RecEmpByCodeName" &&
                        typeProperty.PropertyInfo.Name != "Mode" &&
                        typeProperty.PropertyInfo.Name != "FGItemName" &&
                        typeProperty.PropertyInfo.Name != "FGPartCode" &&
                        typeProperty.PropertyInfo.Name != "IssueSlipno" &&
                        typeProperty.PropertyInfo.Name != "RMItemName" &&
                        typeProperty.PropertyInfo.Name != "RMPartCode" &&
                        typeProperty.PropertyInfo.Name != "IssueQty" &&
                        typeProperty.PropertyInfo.Name != "rmUnit" &&
                        typeProperty.PropertyInfo.Name != "PendQty" &&
                        typeProperty.PropertyInfo.Name != "AltReqQty" &&
                        typeProperty.PropertyInfo.Name != "AltIssueQty" &&
                        typeProperty.PropertyInfo.Name != "AltUnit" &&
                        typeProperty.PropertyInfo.Name != "BatchNo" &&
                        typeProperty.PropertyInfo.Name != "uniquebatchNo" &&
                        typeProperty.PropertyInfo.Name != "lotStock" &&
                        typeProperty.PropertyInfo.Name != "TotalStock" &&
                        typeProperty.PropertyInfo.Name != "IssuedAlternateItem" &&
                        typeProperty.PropertyInfo.Name != "WorkCenter" &&
                        typeProperty.PropertyInfo.Name != "OrginalItemName" &&
                        typeProperty.PropertyInfo.Name != "RMRemark" &&
                        typeProperty.PropertyInfo.Name != "itemsize" &&
                        typeProperty.PropertyInfo.Name != "OriginalPartCode" &&
                        typeProperty.PropertyInfo.Name != "itemcolor" &&
                        typeProperty.PropertyInfo.Name != "ReqQty" &&
                        typeProperty.PropertyInfo.Name != "IssueByEmpCode")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
                else if (Tbname == "MIRDATA")
                {
                    if (typeProperty.PropertyInfo.Name != "Dashboard" && typeProperty.PropertyInfo.Name != "ChallanList" && typeProperty.PropertyInfo.Name != "FromDate"
                    && typeProperty.PropertyInfo.Name != "ToDate" && typeProperty.PropertyInfo.Name != "MIRQDashboard"
                    && typeProperty.PropertyInfo.Name != "ItemName" && typeProperty.PropertyInfo.Name != "Mode" && typeProperty.PropertyInfo.Name != "MRNList"
                    && typeProperty.PropertyInfo.Name != "InvList" && typeProperty.PropertyInfo.Name != "PartCode"
                    && typeProperty.PropertyInfo.Name != "partcode" && typeProperty.PropertyInfo.Name != "item_name"
                    && typeProperty.PropertyInfo.Name != "BillQty" && typeProperty.PropertyInfo.Name != "RecQty"
                    && typeProperty.PropertyInfo.Name != "AcceptedQty" && typeProperty.PropertyInfo.Name != "AltAcceptedQty"
                    && typeProperty.PropertyInfo.Name != "OkRecStore" && typeProperty.PropertyInfo.Name != "RejectedQty"
                    && typeProperty.PropertyInfo.Name != "AltRejectedQty" && typeProperty.PropertyInfo.Name != "RejRecStore"
                    && typeProperty.PropertyInfo.Name != "HoldQty" && typeProperty.PropertyInfo.Name != "Reworkqty"
                    && typeProperty.PropertyInfo.Name != "DeviationQty" && typeProperty.PropertyInfo.Name != "ResponsibleEmpForDeviation"
                    && typeProperty.PropertyInfo.Name != "PONo" && typeProperty.PropertyInfo.Name != "POYearCode"
                    && typeProperty.PropertyInfo.Name != "SchNo" && typeProperty.PropertyInfo.Name != "SchYearCode"
                    && typeProperty.PropertyInfo.Name != "Unit" && typeProperty.PropertyInfo.Name != "AltUnit"
                    && typeProperty.PropertyInfo.Name != "AltRecQty" && typeProperty.PropertyInfo.Name != "Remarks"
                    && typeProperty.PropertyInfo.Name != "Defaulttype" && typeProperty.PropertyInfo.Name != "ApprovedByEmp"
                    && typeProperty.PropertyInfo.Name != "Color" && typeProperty.PropertyInfo.Name != "ItemSize"
                    && typeProperty.PropertyInfo.Name != "ResponsibleFactor" && typeProperty.PropertyInfo.Name != "SupplierBatchno"
                    && typeProperty.PropertyInfo.Name != "shelfLife" && typeProperty.PropertyInfo.Name != "BatchNo"
                    && typeProperty.PropertyInfo.Name != "uniqueBatchno" && typeProperty.PropertyInfo.Name != "AllowDebitNote"
                    && typeProperty.PropertyInfo.Name != "FilePath" && typeProperty.PropertyInfo.Name != "Rate"
                    && typeProperty.PropertyInfo.Name != "FilePath")
                    {
                        object value = row[typeProperty.PropertyInfo.Name];
                        object? safeValue = value == null || DBNull.Value.Equals(value) ? null : Convert.ChangeType(value, typeProperty.Type);
                        typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                    }
                }
            }
            list.Add(obj);
        }
        return list;
    }
    public static DataTable ToDataTable<T>(List<T> items)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);

        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names
            if (prop.Name.ToLower() == "birthdate" || prop.Name.ToLower() == "appointmentdate")
            {
                dataTable.Columns.Add(prop.Name, typeof(DateTime));
            }
            else
            {
                dataTable.Columns.Add(prop.Name);
            }
        }
        foreach (T item in items)
        {
            object?[] values = new object?[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null);
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }
    public static List<T> ToListof<T>(this DataTable dt)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        var columnNames = dt.Columns.Cast<DataColumn>()
            .Select(c => c.ColumnName)
            .ToList();
        var objectProperties = typeof(T).GetProperties(flags);
        var targetList = dt.AsEnumerable().Select(dataRow =>
        {
            var instanceOfT = Activator.CreateInstance<T>();

            foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
            {
                properties.SetValue(instanceOfT, dataRow[properties.Name], null);
            }
            return instanceOfT;
        }).ToList();

        return targetList;
    }
    public static T DataRowToClass<T>(this DataRow dataRow) where T : new()
    {
        T item = new T();

        IEnumerable<PropertyInfo> properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                             .Where(x => x.CanWrite);

        foreach (DataColumn column in dataRow.Table.Columns)
        {
            if (dataRow[column] == DBNull.Value)
            {
                continue;
            }

            PropertyInfo property = properties.FirstOrDefault(x => column.ColumnName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                continue;
            }

            try
            {
                Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                object safeValue = (dataRow[column] == null) ? null : Convert.ChangeType(dataRow[column], t);

                property.SetValue(item, safeValue, null);
            }
            catch
            {
                throw new Exception($"The value '{dataRow[column]}' cannot be mapped to the property '{property.Name}'!");
            }

        }

        return item;
    }
    public static T DataRowToClassSafe<T>(this DataRow row) where T : new()
    {
        T item = new T();
        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             .Where(p => p.CanWrite && p.PropertyType != typeof(IFormFile)); // Ignore IFormFile

        foreach (var prop in props)
        {
            if (!row.Table.Columns.Contains(prop.Name) || row[prop.Name] == DBNull.Value)
                continue;

            try
            {
                object value = Convert.ChangeType(row[prop.Name], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                prop.SetValue(item, value);
            }
            catch (Exception)
            {
                // Log or handle type mismatch if needed
            }
        }

        return item;
    }

    public static bool IsDeliveryDateInRange(DateTime delDate, DateTime effFrom, DateTime effTill, out string errorMsg, int row)
    {
        errorMsg = string.Empty;

        // Compare by Year first
        if (delDate.Year < effFrom.Year || delDate.Year > effTill.Year)
        {
            errorMsg = $"Row : {row} has an Out of Range Delivery Year.";
            return false;
        }
        // If year matches, compare by Month
        if ((delDate.Year == effFrom.Year && delDate.Month < effFrom.Month) ||
            (delDate.Year == effTill.Year && delDate.Month > effTill.Month))
        {
            errorMsg = $"Row : {row} has an Out of Range Delivery Month.";
            return false;
        }
        // If month matches, compare by Date
        if ((delDate.Year == effFrom.Year && delDate.Month == effFrom.Month && delDate.Day < effFrom.Day) ||
            (delDate.Year == effTill.Year && delDate.Month == effTill.Month && delDate.Day > effTill.Day))
        {
            errorMsg = $"Row : {row} has an Out of Range Delivery Date.";
            return false;
        }

        return true;
    }
    private static T GetItem<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                if (pro.Name == column.ColumnName)
                {
                    pro.SetValue(obj, dr[column.ColumnName], null);
                }
                else
                {
                    continue;
                }
            }
        }
        return obj;
    }
    private static T GetItem2<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                //in case you have a enum/GUID datatype in your model
                //We will check field's dataType, and convert the value in it.
                if (pro.Name == column.ColumnName)
                {
                    try
                    {
                        var convertedValue = GetValueByDataType(pro.PropertyType, dr[column.ColumnName]);
                        pro.SetValue(obj, convertedValue, null);
                    }
                    catch (Exception e)
                    {
                        //ex handle code
                        throw;
                    }
                    //pro.SetValue(obj, dr[column.ColumnName], null);
                }
                else
                    continue;
            }
        }
        return obj;
    }
    private static object GetValueByDataType(Type propertyType, object o)
    {
        if (o.ToString() == "null")
        {
            return null;
        }
        if (propertyType == (typeof(Guid)) || propertyType == typeof(Guid?))
        {
            return Guid.Parse(o.ToString());
        }
        else if (propertyType == typeof(int) || propertyType.IsEnum)
        {
            return Convert.ToInt32(o);
        }
        else if (propertyType == typeof(decimal))
        {
            return Convert.ToDecimal(o);
        }
        else if (propertyType == typeof(long))
        {
            return Convert.ToInt64(o);
        }
        else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return Convert.ToBoolean(o);
        }
        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            return Convert.ToDateTime(o);
        }
        return o.ToString();
    }
    public static DateTime ParseSafeDate(string inputDate)
    {
        string[] formats = {
        "dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "MM/dd/yyyy HH:mm:ss",
        "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy"
    };
        DateTime minSqlDate = new DateTime(1753, 1, 1);
        if (!string.IsNullOrWhiteSpace(inputDate) &&
            DateTime.TryParseExact(inputDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result < minSqlDate ? minSqlDate : result;
        }
        return minSqlDate;
    }
    public static TimeOnly? ParseSafeTime(string inputTime, bool? MinValueIfNull = false)
    {
        string[] formats = {
        "HH:mm:ss",     // 23:59:59
        "HH:mm",        // 23:59
        "hh:mm tt",     // 11:59 PM
        "hh:mm:ss tt"   // 11:59:59 PM
    };
        if (!string.IsNullOrWhiteSpace(inputTime))
        {
            foreach (var fmt in formats)
            {
                if (TimeOnly.TryParseExact(inputTime, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly parsed))
                    return parsed;
            }

            // fallback try general parse
            if (TimeOnly.TryParse(inputTime, CultureInfo.InvariantCulture, out TimeOnly fallback))
                return fallback;
        }
        if(MinValueIfNull != null && Convert.ToBoolean(MinValueIfNull))
        {
            return new TimeOnly(0, 0, 0);
        }
        return null;
    }
    public class LogException<T> where T : class
    {
        public LogException()
        {
        }
        public static void LogInfo(ILogger logger, string Message)
        {
            logger.Log(LogLevel.Information, "\n \n ********** Log ********** \n " + JsonConvert.SerializeObject(logger) + "\n");
            logger.Log(LogLevel.Information, "\n \n ********** Context ********** \n " + JsonConvert.SerializeObject(Message) + "\n");
        }
        public static void WriteException(ILogger<T> logger, Exception ex)
        {
            logger.Log(LogLevel.Information, "\n \n ********** Log ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogInformation("\n \n ********** LogInformation ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogCritical("\n \n ********** LogCritical ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogDebug("\n \n ********** LogDebug ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogTrace("\n \n ********** LogTrace ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
            logger.LogWarning("\n \n ********** LogWarning ********** \n " + JsonConvert.SerializeObject(ex) + "\n");
        }
        public static void WriteLog(ILogger logger, ActionExecutingContext context)
        {
            logger.Log(LogLevel.Information, "\n \n ********** Log ********** \n " + JsonConvert.SerializeObject(logger) + "\n");
            logger.Log(LogLevel.Information, "\n \n ********** Context ********** \n " + JsonConvert.SerializeObject(context) + "\n");
        }
    }
}