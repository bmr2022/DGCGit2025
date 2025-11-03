using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace eTactWeb.DOM.Models;

[Serializable()]
public class Common
{
    public Common(string? Browser)
    {
        this.Browser = Browser;
    }

    public string? Browser { get; set; }

    public class DeliveryModel
    {
        public int DAltQty { get; set; }
        public string? DDate { get; set; }
        public int DDays { get; set; }
        public int DItemCode { get; set; }
        public int DPartCode { get; set; }
        public int DQty { get; set; }
        public string? DRemarks { get; set; }
        public int DSRNo { get; set; }
        public decimal DTotalQty { get; set; }
        public int DTQty { get; set; }
        public bool IsListEmpty { get; set; }
    }

    public class HSNTAX
    {
        public int AC { get; set; }
        public int HSNNo { get; set; }
    }

    public class HSNTAXInfo
    {
        public string? AddInTaxable { get; set; }
        public int CGSTAccountCode { get; set; }
        public string? CGSTTaxName { get; set; }
        public int ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? LocalCen { get; set; }
        public string? PartName { get; set; }
        public string? Refundable { get; set; }
        public int SGSTAccountCode { get; set; }
        public string? SGSTTaxName { get; set; }
        public int TaxPercent { get; set; }
        public string? TaxType { get; set; }
    }

    public class ResponseResult
    {
        public dynamic? Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? StatusText { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public object Rows { get; set; }
	}

    public class TaxModel : TimeStamp
    {
        public IList<TaxModel>? TaxDetailGridd { get; set; }

        public IList<SelectListItem> TaxList { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "TAX", Text = "TAX" },
            new() { Value = "EXPENSES", Text = "EXPENSES" },
        };

        [Column(TypeName = "decimal(18, 6)")]
        public decimal TotalTaxAmt { get; set; }

        public int TxAccountCode { get; set; }
        public string? TxAccountName { get; set; }
        public string? TxAdInTxable { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal TxAmount { get; set; }

        public int TxItemCode { get; set; }
        public string? TxItemName { get; set; }
        public decimal TxOnExp { get; set; }
        public int TxPartCode { get; set; }
        public string? TxPartName { get; set; }
        public decimal TxPercentg { get; set; }
        public string? TxRefundable { get; set; }
        public string? TxRemark { get; set; }
        public string? TxRoundOff { get; set; }
        public int TxSeqNo { get; set; }
        public int TxTaxType { get; set; }
        public string? TxTaxTypeName { get; set; }
        public string? TxType { get; set; }
        public string ? Message { get; set; }
        public IList<SelectListItem> YesNo { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No", Selected = true  },
        };
    }
    public class TDSModel : TimeStamp
    {
        public IList<TDSModel>? TDSDetailGridd { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal TotalTDSAmt { get; set; }
        public int TDSAccountCode { get; set; }
        public string? TDSAccountName { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TDSAmount { get; set; }
        public decimal TDSPercentg { get; set; }
        public string? TDSRemark { get; set; }
        public string? TDSRoundOff { get; set; }
        public decimal? TDSRoundOffAmt { get; set; }
        public int TDSSeqNo { get; set; }
        public int TDSTaxType { get; set; }
        public string? TDSTaxTypeName { get; set; }
        public IList<SelectListItem> TdsYesNo { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No", Selected = true  },
        };
    }
    public interface ITDSModel
    {
        IList<TDSModel>? TDSDetailGridd { get; set; }
        decimal TotalTDSAmt { get; set; }
        int TDSAccountCode { get; set; }
        string? TDSAccountName { get; set; }
        decimal TDSAmount { get; set; }
        decimal TDSPercentg { get; set; }
        string? TDSRemark { get; set; }
        string? TDSRoundOff { get; set; }
        decimal? TDSRoundOffAmt { get; set; }
        int TDSSeqNo { get; set; }
        int TDSTaxType { get; set; }
        string? TDSTaxTypeName { get; set; }
        IList<SelectListItem> TdsYesNo { get; set; }

        string? Active { get; set; }
        int CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        string? EID { get; set; }
        int ID { get; set; }
        string? Mode { get; set; }
        string TxPageName { get; set; }
        int? UpdatedBy { get; set; }
      
        DateTime? UpdatedOn { get; set; }
    }
    public class TextValue
    {
        public string? Text { get; set; }
        public string? Value { get; set; }
        public int EmpId { get; set; }
    }

    public class TimeStamp
    {
        [Newtonsoft.Json.JsonIgnore]
        public string? Active { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? EID { get; set; }

        public int ID { get; set; }

        public string? Mode { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string TxPageName { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }

    public class UserResponse<T> where T : class
    {
        public enum ResponseMessages
        {
            Success = 200,
            Error = 500,
            NotFound = 404,
            Duplicate = 303
        }

        public dynamic Data { get; set; }
        public int id { get; set; }
        public string Message { get; set; }
        public int RecordCount { get; set; }
        public int StatusCode { get; set; }

        public static UserResponse<T> SendResponse(int recordCount, IEnumerable<T> viewModel)
        {
            UserResponse<T> response = new UserResponse<T>();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.RecordCount = recordCount;
            response.Message = ResponseMessages.Success.ToString();
            response.Data = viewModel;
            return response;
        }

        public static UserResponse<T> SendResponse(T viewModel, int? id = null)
        {
            UserResponse<T> response = new UserResponse<T>();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.RecordCount = 0;
            response.Message = ResponseMessages.Success.ToString();
            response.Data = viewModel;
            response.id = id ?? default(int);
            return response;
        }

        public static UserResponse<T> SendResponse(int recordCount, List<IEnumerable<T>> viewModel)
        {
            UserResponse<T> response = new UserResponse<T>();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.RecordCount = recordCount;
            response.Message = ResponseMessages.Success.ToString();
            response.Data = viewModel;
            return response;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // public static UserResponse<T> SendResponse(string viewModel) { UserResponse<T> response =
        // new UserResponse<T>(); response.StatusCode = (int)HttpStatusCode.OK; response.RecordCount
        // = 0; response.Message = ResponseMessages.Success.ToString(); response.Data = viewModel;
        // return response; }
    }

    public class DbCrModel 
    {
        public IList<DbCrModel>? DbCrDetailGrid { get; set; }
        public int AccEntryId { get; set; }
        public int AccYearCode { get; set; }
        public int SeqNo { get; set; }
        public string? InvoiceNo { get; set; }
        public string? VoucherNo { get; set; }
        public string? AgainstInvNo { get; set; }
        public int? AginstVoucherYearcode { get; set; }
        public int? AccountCode { get; set; }
        public int? DocTypeID { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public int? BillQty { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public int? Rate { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public int? AccountAmount { get; set; }
        public string? DRCR { get; set; }
        public string? AccountName { get; set; }
        public float? DrAmt { get; set; }
        public float? CrAmt { get; set; }
    }
    public class AdjustmentModel : TimeStamp
    {
        public List<AdjustmentModel>? AdjAdjustmentDetailGrid { get; set; }
        public IList<SelectListItem> AdjModeOfAdjustmentList { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "NewRef", Text = "New Ref", Selected = true  },
            new() { Value = "AgainstRef", Text = "Against Ref" },
            new() { Value = "Advance", Text = "Advance" },
        };
        public IList<SelectListItem> AdjDrCrList { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "DR", Text = "DR" },
            new() { Value = "CR", Text = "CR", Selected = true },
        };
        public IList<SelectListItem> AdjPurchOrderNoList { get; set; } = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" },
        };
        public int? AdjSeqNo { get; set; }
        public string? AdjModeOfAdjstment { get; set; }
        public string? AdjModeOfAdjstmentName { get; set; }
        public string? AdjNewRefNo { get; set; }
        public string? AdjDescription { get; set; }
        public string? AdjDrCrName { get; set; }
        public string? AdjDrCr { get; set; }
        //[Column(TypeName = "decimal(18, 6)")]
        public decimal? AdjPendAmt { get; set; }
        public float? AdjAdjstedAmt { get; set; }
        public float? AdjTotalAmt { get; set; }
        public float? AdjRemainingAmt { get; set; }
        public int? AdjOpenEntryID { get; set; }
        public int? AdjOpeningYearCode { get; set; }
        public DateTime? AdjDueDate { get; set; }
        public string? DueDate { get; set; }
        public string? AdjPurchOrderNo { get; set; }
        public int? AdjPOYear { get; set; }
        public DateTime? AdjPODate { get; set; }
        public string? AdjPageName { get; set; }
        public int? AdjAgnstAccEntryID { get; set; }
        public int? AdjAgnstAccYearCode { get; set; }
        public string? AdjAgnstModeOfAdjstment { get; set; }
        public string? AdjAgnstModeOfAdjstmentName { get; set; }
        public string? AdjAgnstNewRefNo { get; set; }
        public string? AdjAgnstVouchNo { get; set; }
        public string? AdjAgnstVouchType { get; set; }
        public string? AdjAgnstDrCrName { get; set; }
        public string? AdjAgnstDrCr { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public float? AdjAgnstPendAmt { get; set; }
        public float? AdjAgnstAdjstedAmt { get; set; }
        public float? AdjAgnstTotalAmt { get; set; }
        public float? AdjAgnstRemainingAmt { get; set; }
        public int? AdjAgnstOpenEntryID { get; set; }
        public int? AdjAgnstOpeningYearCode { get; set; }
        public DateTime? AdjAgnstVouchDate { get; set; }
        public string? AdjAgnstTransType { get; set; }
        public float TotalBillAmount {  get; set; }
        public float TotalRemainingAmount {  get; set; }
        public float TotalAdjustAmount { get; set; }
        public float? PendingToAdjustAmount { get; set; }
    }
}
