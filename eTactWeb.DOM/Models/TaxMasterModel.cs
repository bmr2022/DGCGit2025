using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

[Serializable()]
public class TaxMasterDashboard
{
    public int AccountCode { get; set; }
    public string? AddInTaxable { get; set; }
    public IList<TextValue>? AddInTaxableList { get; set; }
    public string? DisplayName { get; set; }
    public string? Refundable { get; set; }
    
    
    public IList<TextValue>? RefundableList { get; set; }
    public string? TaxCategory { get; set; }
    public IList<TextValue>? TaxCategoryList { get; set; }
    public string? TaxName { get; set; }
    public string? TaxPercent { get; set; }
    public string? TaxType { get; set; }
    public string? HSNNo { get; set; }
    public IList<TextValue>? TaxTypeList { get; set; }
    public IList<TaxMasterDashboard>? TMDashboard { get; set; }
}

[Serializable()]
public class TaxMasterModel : TimeStamp
{
    private IList<SelectListItem> _Refundable = new List<SelectListItem>()
    {
        new() { Value = "Y", Text = "YES" },
        new() { Value = "N", Text = "NO" }
    };

    private IList<SelectListItem> _TaxApplicable = new List<SelectListItem>()
    {
        new() { Value = "EXPORT", Text = "EXPORT" },
        new() { Value = "DOMESTIC", Text = "DOMESTIC" }
    };

    private IList<SelectListItem> _TaxAppliedOn = new List<SelectListItem>()
    {
        new() { Value = "BASIC", Text = "BASIC" },
        new() { Value = "TAXABLE AMOUNT", Text = "TAXABLE AMOUNT" },
        new() { Value = "NET AMOUNT", Text = "NET AMOUNT" },
        new() { Value = "OTHER TAX", Text = "OTHER TAX" },
        new() { Value = "BASIC + EXPENSE", Text = "BASIC + EXPENSE" }
    };

    private IList<SelectListItem> _TaxCategory = new List<SelectListItem>()
    {
        new() { Value = "CGST", Text = "CGST" },
        new() { Value = "SGST", Text = "SGST" },
        new() { Value = "IGST", Text = "IGST" }
    };

    public string? AccountType { get; set; }

    public string? AddInTaxable { get; set; }

    public IList<SelectListItem> AddInTaxableList
    {
        get => _Refundable;
        set => _Refundable = value;
    }

    public string? DisplayName { get; set; }

    public string? EffectiveDate { get; set; }

    public int EntryID { get; set; }

    public IList<string>? HSN { get; set; }
    public IList<HSNDetail>? HSNDetailList { get; set; }
    public IList<TextValue>? HSNList { get; set; }

    public string? MainGroup { get; set; }

    public string? ParentGroup { get; set; }

    public IList<TextValue>? ParentGroupList { get; set; }

    public string? Refundable { get; set; }
    public string? HSNNo { get; set; }

    public IList<SelectListItem> RefundableList
    {
        get => _Refundable;
        set => _Refundable = value;
    }

    public string? SGSTHead { get; set; }

    public IList<TextValue>? SGSTHeadList { get; set; }

    public string? SubGroup { get; set; }

    public string? SubSubGroup { get; set; }

    public string? TaxApplicable { get; set; }

    public IList<SelectListItem> TaxApplicableList
    {
        get => _TaxApplicable;
        set => _TaxApplicable = value;
    }

    public string? TaxAppliedOn { get; set; }

    public IList<SelectListItem> TaxAppliedOnList
    {
        get => _TaxAppliedOn;
        set => _TaxAppliedOn = value;
    }

    public string? TaxCategory { get; set; }

    public IList<SelectListItem> TaxCategoryList
    {
        get => _TaxCategory;
        set => _TaxCategory = value;
    }

    public string? TaxName { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TaxPercent { get; set; }

    public int TaxType { get; set; }

    public IList<TextValue>? TaxTypeList { get; set; }
    public string? TaxTypeName { get; set; }
    public string? UnderGroup { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

[Serializable()]
public class HSNDetail
{
    public int AccountCode { get; set; }
    public string? HSNNO { get; set; }
    public string? TaxCategory { get; set; }
    public decimal TaxPercent { get; set; }
}