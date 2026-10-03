using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface ITallyService
    {
        Task<TallyResponse> TestConnectionAsync();
        Task<TallyResponse> PostPurchaseVoucherAsync(PurchaseBillModel model);
        Task<TallyResponse> PostDirectPurchaseVoucherAsync(DirectPurchaseBillModel model);
        Task<TallyResponse> PostSalesVoucherAsync(SaleBillModel model);
        Task<TallyResponse> PostPurchaseRejectionVoucherAsync(AccPurchaseRejectionModel model);
        Task<TallyResponse> PostSupplSaleBillVoucherAsync(AccSupplSaleBillModel model);
        Task<TallyResponse> PostSaleRejectionVoucherAsync(SaleRejectionModel model);
        Task<TallyResponse> PostCreditNoteVoucherAsync(AccCreditNoteModel model);
    }
}
