using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class DeassembleItemModel: DeassembleItemDetail
    {

        public int? DeassEntryID { get; set; }
        public string? DeassEntryDate { get; set; }
        public int? DeassYearcode { get; set; }
        public string? DeassSlipNo { get; set; }
        public int? BomNo { get; set; }
        public int? FGStoreId { get; set; }
        public string? FGStoreName { get; set; }
        public int? FinishItemCode { get; set; }
        public string? FinishItemName { get; set; }
        public string? FinishPartCode { get; set; }
        public string? FGBatchNo { get; set; }
        public string? FGUniqueBatchNo { get; set; }
        public decimal? TotalStock { get; set; }
        public decimal? FGQty { get; set; }
        public string? Unit { get; set; }
        public decimal? FGConvQty { get; set; }
        public int? CreatedByEmp { get; set; }
        public string? CreatedByEmpName { get; set; }
        public string? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public int? UpdatedOn { get; set; }
        public string? EntryByMachine { get; set; }
        public string? CC { get; set; }
        public string? MRNO { get; set; }
        public int? MRNYearCode { get; set; }
        public string? MRNDate { get; set; }
        public int? MRNEntryID { get; set; }
        public int? ProdSlipNO { get; set; }
        public string? ProdDate { get; set; }
        public int? ProdYearCode { get; set; }
        public int? ProdEntryId { get; set; }
        public string? MaterialRecFrom { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string SlipNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string BatchNoback { get; set; }
        public string DashboardTypeBack { get; set; }
        public string GlobalSearchBack { get; set; }

        public IList<DeassembleItemDetail> DeassembleItemDetail { get; set; }
        


    }

    public class DeassembleItemDetail : TimeStamp
    {
        public int? SeqNo { get; set; }
        public int? RMItemCode { get; set; }
        public int? RMStoreId { get; set; }
        public string? RMStoreName { get; set; }
        public string? RMItemName { get; set; }
        public string? RmPartCode { get; set; }
        public string? RMUnit { get; set; }
        public string? Remark { get; set; }
        public string? RMBatchNo { get; set; }
        public string? RmUniqueBatchNo { get; set; }
        public decimal? BomQty { get; set; }
        public decimal? DeassQty { get; set; }
        public decimal? IdealDeassQty { get; set; }
    }

    public class DeassembleItemDashBoard : TimeStamp
    {
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; }
        public string? Searchbox { get; set; }

        public int? DeassEntryID { get; set; }                  // DM.DeassEntryID
        public string? DeassEntryDate { get; set; }           // DM.DeassEntryDate
        public int? DeassYearcode { get; set; }              // DM.DeassYearcode
        public string? DeassSlipNo { get; set; }                // DM.DeassSlipNo
        public int?     FGStoreId { get; set; }                     // DM.FGStoreId
        public string? FGStoreName { get; set; }                // FGStoreName

        public int? FinishItemCode { get; set; }             // DM.FinishItemCode
        public string? FinishItemName { get; set; }             // FinishItemName
        public string? FinishPartCode { get; set; }             // FinishPartCode
        public string? FGBatchNo { get; set; }                  // DM.FGBatchNo
        public string? FGUniqueBatchNo { get; set; }            // DM.FGUniqueBatchNo

        public decimal? TotalStock { get; set; }                // DM.TotalStock
        public decimal? FGQty { get; set; }                     // DM.FGQty
        public string? FGUnit { get; set; }                     // FGUnit
        public decimal? FGConvQty { get; set; }                 // DM.FGConvQty

        public int? CreatedByEmp { get; set; }                  // DM.CreatedByEmp
        public string? CreatedByEmpName { get; set; }           // CreatedByEmpName
        public string? CreatedOn { get; set; }                // DM.CreatedOn

        public int? UpdatedBy { get; set; }                     // DM.UpdatedBy
        public string? UpdatedByEmpName { get; set; }           // UpdatedByEmpName
        public string? UpdatedOn { get; set; }               // DM.UpdatedOn (nullable in case it's not updated yet)
        public IList<DeassembleItemDashBoard>? DeassembleItemDashBoardDetail { get; set; }
    }
}
