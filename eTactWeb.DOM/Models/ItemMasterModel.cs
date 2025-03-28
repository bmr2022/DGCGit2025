using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    [Serializable()]
    public class FeatureOption
    {
        public bool AllowPartCode { get; set; }
        public bool DuplicateItemName { get; set; }
    }

    [Serializable()]
    public class ItemMasterList
    {
        public IList<TextValue>? AltUnitList { get; set; }
        public IList<TextValue>? EmpList { get; set; }
        public IList<ItemMasterModel>? ItemMaster { get; set; }
        public IList<TextValue>? ItemTypeList { get; set; }
        public IList<TextValue>? ParentGroupList { get; set; }
        public IList<TextValue>? PurchaseAccList { get; set; }
        public IList<TextValue>? SaleAccList { get; set; }
        public IList<TextValue>? StoreList { get; set; }
        public IList<TextValue>? UnitList { get; set; }
    }

    [Serializable()]
    public class ItemMasterModel : TimeStamp
    {
        
        private IList<SelectListItem> _Test = new List<SelectListItem>()
        {
            new() { Value = "0", Text = "Select" },
            new() { Value = "Test1", Text = "Test1" },
            new() { Value = "Test2", Text = "Test2" },
        };

        private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };
        public int TotalRecords {  get; set; }
        public int PageNumber {  get; set; }
        public int PageSize { get; set; }
        public string? EntryByMachineName { get;set; }
        public string? Act { get; set; }
        public string? VoltageVlue { get; set; }
        public string? SerialNo { get; set; }
        public string? OldPartCode { get; set; }
        public string? Package { get; set; }
        public string? Active { get; set; }

        [Display(Name = "Alt. Unit")]
        public string? AlternateUnit { get; set; }
        public string SearchQuery {  get; set; }
        public string? SwitchAll { get; set; }
        public IList<TextValue>? AltUnitList { get; set; }
        public string? BinNo { get; set; }
        public string? BomRequired { get; set; }
        public string? CC { get; set; }
        public string? Colour { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal CostPrice { get; set; }

        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public decimal DailyRequirment { get; set; }
        public string? DispItemName { get; set; }
        public string? DrawingNo { get; set; }
        public IList<TextValue>? EmpList { get; set; }
        public string? EmpName { get; set; }

        [Display(Name = "Entry Date")]
        public string? EntryDate { get; set; }

        public FeatureOption? FeatureOption { get; set; }

        [NotMapped]
        public int HSNNO { get; set; }

        public string? ImageURL { get; set; }
        public int IsDelete { get; set; }

        [Display(Name = "Item Code")]
        public int Item_Code { get; set; }

        [Display(Name = "Item Name")]
        public string? Item_Name { get; set; }
        public string? ItemDesc { get; set; }
        public string? ItemGroup { get; set; }
        public string? ItemSize { get; set; }
        public int ItemType { get; set; }
        public string? ItemTypeName { get; set; }
        public string? POServType { get; set; }
        public IList<TextValue>? ItemTypeList { get; set; }
        public string? JobWorkItem { get; set; }
        public string? LastUpdatedDate { get; set; }
        public decimal LeadTime { get; set; }
        public IList<ItemMasterModel>? MasterList { get; set; }

        [Display(Name = "Max. Level")]
        public decimal MaximumLevel { get; set; }

        public decimal MaxLevelDays { get; set; }
        public decimal MaxWipStock { get; set; }

        [Display(Name = "Min. Level")]
        public decimal MinimumLevel { get; set; }

        public decimal MinLevelDays { get; set; }
        public string? Mode { get; set; }
        public string? ModelNo { get; set; }
        public string? NeedPO { get; set; }
        public string? NeedSo { get; set; }
        public decimal NoOfPcs { get; set; }
        public string? PackingType { get; set; }
        public decimal ParentCode { get; set; }
        public string? ParentName { get; set; }
        public IList<TextValue>? ParentGroupList { get; set; }

        [Display(Name = "Part Code")]
        public string? PartCode { get; set; }

        public decimal ProductLifeInus { get; set; }

        public IList<TextValue>? PurchaseAccList { get; set; }

        public string? PurchaseAccountcode { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal PurchasePrice { get; set; }

        public string? QcReq { get; set; }

        public string? RackID { get; set; }

        public decimal ReorderLevel { get; set; }

        public IList<TextValue>? SaleAccList { get; set; }

        public string? SaleAccountcode { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal SalePrice { get; set; }

        [Display(Name = "Std. Packing")]
        [Column(TypeName = "decimal(10, 4)")]
        public decimal StdPacking { get; set; }

        public string? Stockable { get; set; }
        public string? UniversalPartCode { get; set; }
        public string? UniversalDescription { get; set; }
        public string? Store { get; set; }
        public string? VendorBatchNoMandatory { get; set; }
        public string? MaterialType { get; set; }
        public string? MaterialIssueinStdPkgOnly { get; set; }

        public IList<TextValue>? StoreList { get; set; }

        public IList<SelectListItem> Test
        {
            get => _Test;
            set => _Test = value;
        }

        public string? TypeName { get; set; }

        public string? UID { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]{3}")]
        public string? Unit
        {
            get; set;
        }

        public IList<TextValue>? UnitList { get; set; }

        [NotMapped]
        public IFormFile? UploadImage { get; set; }
        public IFormFile? ItemImage { get; set; }
        public string? ItemImageURL { get; set; }
        public decimal WastagePercent { get; set; }
        public string? WipStockable { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal WtSingleItem { get; set; }

        public int YearCode { get; set; }
        public int YearlyConsumedQty { get; set; }

        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? ItemServAssets { get; set; }
        public int? ProdInWorkcenter { get; set; }
        public string? ProdWorkCenterDescription { get; set; }
        public string? ProdInhouseJW { get; set; }
        public string? BatchNO { get; set; }

        public IList<SelectListItem> YesNo
        {
            get => _YesNo;
            set => _YesNo = value;
        }
        public List<ItemViewModel>? ExcelDataList { get; set; }
    }

    public class ItemViewModel // Excel data
    {
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? PartCodeExists {  get; set; }
        public string? ItemNameExists {  get; set; }
        public string? Unit { get; set; }
        public int HSNNo { get; set; }
        public string? ItemGroup { get; set; }
        public int ItemGroupCode { get; set; }
        public int ItemCategoryCode { get; set; }
        public string? ItemCategory { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public string? Stockable { get; set; }
        public string? WIPStockable { get; set; }
        public string? NeedPO { get; set; }
        public string? QcReq { get; set; }
        public string? ItemServAssets { get; set; }
        public int StdPkg { get; set; }
        public int SeqNo { get; set; }
    }
}