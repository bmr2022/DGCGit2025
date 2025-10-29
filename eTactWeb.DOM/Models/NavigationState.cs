public class NavigationState
{
    public string ControllerName { get; set; }
    public string ActionName { get; set; }

    // Filters
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string ReportType { get; set; }
    public string GroupOrLedger { get; set; }
    public string ParentLedger { get; set; }
    public string VoucherType { get; set; }
    public string VoucherNo { get; set; }
    public string INVNo { get; set; }
    public string Narration { get; set; }
    public string Amount { get; set; }
    public string Dr { get; set; }
    public string Cr { get; set; }
    public string GlobalSearch { get; set; }

    // DrillDown context
    public int AccountCode { get; set; }
    public int YearCode { get; set; }
    public int ID { get; set; }
    public string Mode { get; set; }

    // Optional: check equality to prevent duplicate push
    public override bool Equals(object obj)
    {
        if (obj is not NavigationState other) return false;
        return ControllerName == other.ControllerName &&
               ActionName == other.ActionName &&
               AccountCode == other.AccountCode &&
               YearCode == other.YearCode &&
               ID == other.ID &&
               Mode == other.Mode;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ControllerName, ActionName, AccountCode, YearCode, ID, Mode);
    }
}
