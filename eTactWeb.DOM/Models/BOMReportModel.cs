using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class BOMReportModel: TimeStamp
    {
        // Fields for 'BOMTREE' flag
        public string MainPartCode { get; set; }
        public string BOMPartCode { get; set; }         
        public string MainItemName { get; set; }        
        public string BOMItemName { get; set; }        
        public string FGPartCode { get; set; }           
          
        public string FGItemName { get; set; }          
        public string RMPartCode { get; set; }           
        public string RMItemName { get; set; }          
        public double? NetReqQty { get; set; }           
        public double? ReqQty { get; set; }           
        public double? RMQty { get; set; }               
        public int? BomRevNo { get; set; }             
        public int? ItemLvl { get; set; }               
        public int? BOMItemCode { get; set; }          
        public int? FGItemcode { get; set; }          
        public int? FgItemcode { get; set; }          
        public int? RMItemcode { get; set; }          
        public int? ItemCode { get; set; }          
        public int? PartCode { get; set; }          
        public int? FGItemCode { get; set; }            
        public int? FgItemCode { get; set; }            
        public int? RMItemCode { get; set; }            
        public int? Storeid { get; set; }            
        public int? Yearcode { get; set; }            
        public string SubBOM { get; set; }              
        public string FGName { get; set; }              
        public string FGPartcode { get; set; }              
        public string RMName { get; set; }              
        public string RMpartcode { get; set; }              

        // Fields for 'DirectBOM' flag
        public int? BomNo { get; set; }                 
        public double Qty { get; set; }                  

        // Fields for 'BOMSTOCK' flag
        public double? StoreRecQty { get; set; }        
        public double? StoreIssueQty { get; set; }        
        public double? StoreStock { get; set; }         
        public long? StoreId { get; set; }               
        public float? WIPRecQty { get; set; }            
        public float? WIPIssueQty { get; set; }       
        public double? Rate { get; set; }       
        public double? Amount { get; set; }       
        public double? WIPStock { get; set; }            
        public double? TotalStock { get; set; }            
        public double? TotalReqQty { get; set; }            
        public double? ShortExcess { get; set; }            
        public long? WCID { get; set; }    
        public string? FGUnit { get; set; }    
        public string? RMUnit { get; set; }    
        
        //other
        public string FromDate { get; set; }                 
        public string ToDate { get; set; }                 
        public int YearCode { get; set; }                 
        public string ActualEntryDate { get; set; }
        public string ReportType { get; set; }
        [Required(ErrorMessage = "Store Name is required.")]
        public string StoreName { get; set; }
        public string WorkCenterName { get; set; }
        public string ForTheStore { get; set; }
        public string ForWorkCeneter { get; set; }
        public IList<BOMReportModel>? BOMReportGrid { get; set; }
    }

}
