using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class EInvoiceItemModel
    {
        public int EntryId { get; set; }
        public string InvoiceNo { get; set; }
        public int YearCode { get; set; }
        public string saleBillType { get; set; }
        public string customerPartCode { get; set; }
        public string transporterName { get; set; }
        public string distanceKM { get; set; } // Add this property to fix the error  
        public string vehicleNo { get; set; }
        public int EntrybyId { get; set; }
        public string MachineName { get; set; }
        public string generateEway { get; set; }

    }

    //new code from ghere
    public class EwayBillResponse
    {
        public Results results { get; set; }
    }

    public class Results
    {
        public Message message { get; set; }
        public string status { get; set; }
        public int code { get; set; }
    }

    public class Message
    {
        public long eway_bill_number { get; set; }
        public string eway_bill_date { get; set; }
        public string generate_mode { get; set; }
        public string userGstin { get; set; }
        public string supply_type { get; set; }
        public string sub_supply_type { get; set; }
        public string document_type { get; set; }
        public string document_number { get; set; }
        public string document_date { get; set; }
        public string gstin_of_consignor { get; set; }
        public string legal_name_of_consignor { get; set; }
        public string address1_of_consignor { get; set; }
        public string address2_of_consignor { get; set; }
        public string place_of_consignor { get; set; }
        public int pincode_of_consignor { get; set; }
        public string state_of_consignor { get; set; }
        public string gstin_of_consignee { get; set; }
        public string legal_name_of_consignee { get; set; }
        public string address1_of_consignee { get; set; }
        public string address2_of_consignee { get; set; }
        public string place_of_consignee { get; set; }
        public int pincode_of_consignee { get; set; }
        public string state_of_supply { get; set; }
        public decimal taxable_amount { get; set; }
        public decimal total_invoice_value { get; set; }
        public decimal cgst_amount { get; set; }
        public decimal sgst_amount { get; set; }
        public decimal igst_amount { get; set; }
        public decimal cess_amount { get; set; }
        public string transporter_id { get; set; }
        public string transporter_name { get; set; }
        public string eway_bill_status { get; set; }
        public int transportation_distance { get; set; }
        public int number_of_valid_days { get; set; }
        public string eway_bill_valid_date { get; set; }
        public int extended_times { get; set; }
        public string reject_status { get; set; }
        public string vehicle_type { get; set; }
        public string actual_from_state_name { get; set; }
        public string actual_to_state_name { get; set; }
        public string transaction_type { get; set; }
        public decimal other_value { get; set; }
        public decimal cess_nonadvol_value { get; set; }
        public List<Itemewaybill> itemList { get; set; }
        public List<VehicleDetail> VehiclListDetails { get; set; }
    }

    public class Itemewaybill
    {
        public int item_number { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_description { get; set; }
        public int hsn_code { get; set; }
        public decimal quantity { get; set; }
        public string unit_of_product { get; set; }
        public decimal cgst_rate { get; set; }
        public decimal sgst_rate { get; set; }
        public decimal igst_rate { get; set; }
        public decimal cess_rate { get; set; }
        public decimal cessNonAdvol { get; set; }
        public decimal taxable_amount { get; set; }
    }

    public class VehicleDetail
    {
        public string update_mode { get; set; }
        public string vehicle_number { get; set; }
        public string place_of_consignor { get; set; }
        public string state_of_consignor { get; set; }
        public int tripshtNo { get; set; }
        public string userGstin { get; set; }
        public string vehicle_number_update_date { get; set; }
        public string transportation_mode { get; set; }
        public string transporter_document_number { get; set; }
        public object transporter_document_date { get; set; }
        public string group_number { get; set; }
    }
}
