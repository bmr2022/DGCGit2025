$(document).ready(function () {
     
    $('#VoucherType').select2();
    $('#Intrument').select2();
    $('#LedgerName').select2();
    $('#ModeOfAdjustment').select2();
    $('#CostCenterName').select2();
    $('#SubVoucherName').select2();
    $('#Currency').select2();
    //FillCostCenterName();
    //FillSubVoucherName();
    //FillIntrument();
    //FillCurrency();
    //BindModeOfAdjust();
    FillLedgerName();
    //checkModeOfAdjustment();

    $('#CostCenterName').change(function () {
        $('#CostCenterId').val($('#CostCenterName').val());
    });

    $('#LedgerName').rules("remove", "required");
    $('#DRCR').rules("remove", "required");
    $('#Intrument').rules("remove", "required");
    $('#InsNo').rules("remove", "required");
    $('#InsDate').rules("remove", "required");
    $('#ModeOfAdjustment').rules("remove", "required");
    $('#AdjustmentAmt').rules("remove", "required");
    $('#AdjustmentAmtOthCur').rules("remove", "required");
    $('#CostCenterName').rules("remove", "required");
    $('#PartyWiseNaration').rules("remove", "required");
    $('#SoDate').rules("remove", "required");
    $('#SOYear').rules("remove", "required");
    $('#SeqNo').rules("remove", "required");
    $('#CustOrderNo').rules("remove", "required");
    $('#AgainstVoucherNo').rules("remove", "required");
    $('#AgainstVoucherDate').rules("remove", "required");
    $('#AgainstVoucheryearCode').rules("remove", "required");
    $('#AgainstVoucherType').rules("remove", "required");
    $('#AgainstVoucherEntryId').rules("remove", "required");
    $('#VoucherBillAmt').rules("remove", "required");
    $('#PendBillAmt').rules("remove", "required");
    $('#Branch').rules("remove", "required");
    $('#EntryId').rules("remove", "required");
    $('#EntryDate').rules("remove", "required");
    $('#YearCode').rules("remove", "required");
    $('#SubVoucherName').rules("remove", "required");
    $('#VoucherNo').rules("remove", "required");
    $('#VoucherDate').rules("remove", "required");
    $('#Currency').rules("remove", "required");
    $('#ExRate').rules("remove", "required");
    $('#BankRECO').rules("remove", "required");
    $('#Balance').rules("remove", "required");
    $('#InsDate').rules("remove", "required");
    $('#AdjustmentAmt').rules("remove", "required");
    $('#AdjustmentAmtOthCur').rules("remove", "required");
    $('#SoDate').rules("remove", "required");
    $('#SOYear').rules("remove", "required");
    $('#AgainstVoucherNo').rules("remove", "required");
    $('#AgainstVoucherDate').rules("remove", "required");
    $('#AgainstVoucheryearCode').rules("remove", "required");
    $('#AgainstVoucherEntryId').rules("remove", "required");
    $('#VoucherBillAmt').rules("remove", "required");
    $('#PendBillAmt').rules("remove", "required");
    $('#PRODSTATUSProdUnProdRej').rules("remove", "required");
    $('#PRODSTATUSProdUnProdRej').rules("remove", "required");
    var date = $('#EntryDate').datepicker({ dateFormat: 'dd/mm/yy' }).datepicker("setDate", 'now');
    var currentDate = new Date();

    var minDate = new Date();
    minDate.setMonth(minDate.getMonth() - 3);

    var maxDate = new Date();
    maxDate.setMonth(maxDate.getMonth() + 3);

    $('#InsDate').datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: minDate,
        maxDate: maxDate
    }).datepicker("setDate", currentDate);
    $('#ActualEntryDate').datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: currentDate,
        maxDate: currentDate
    }).datepicker("setDate", currentDate);
    var VoucherDate = $('#VoucherDate').datepicker({ dateFormat: 'dd/mm/yy' }).datepicker("setDate", 'now');
    var AgainstVoucherDate = $('#AgainstVoucherDate').datepicker({ dateFormat: 'dd/mm/yy' }).datepicker("setDate", 'now');
    var soDate = $('#SoDate').datepicker({ dateFormat: 'dd/mm/yy' }).datepicker("setDate", 'now');

    //$('#BankRECO').datepicker({ dateFormat: 'dd/mm/yy' });
    var BankRecoDate = $('#BankRECO').val();
    if (BankRecoDate === "01/01/1900") {
        $('#BankRECO').val("");
    }

    if ($('#Mode').val() == "U" || $('#Mode').val() == "V") {

    }
    $('#AdjustmentAmt').on('input', function () {

        let adjustmentAmt = $(this).val();
        $('#AdjustmentAmtOthCur').val(adjustmentAmt);
    });
    $("#AdjustmentAmtOthCur").on("input", function () {
        var value = $(this).val();

        if (!/^\d+$/.test(value)) {
            $.notify("Adjustment amount must be a positive number.", "error");

            $(this).val('');
        }
    });

    if ($('#Mode').val() != "U" && $('#Mode').val() != "V") {
        FillEntryID();
    }

});
function checkModeOfAdjustment() {

    const modeOfAdjustment = $("#ModeOfAdjustment").val(); // Assuming this is the ID of the field
    // Show/Hide Advance Fields
    if (modeOfAdjustment === "Advance") {
        $("#advanceFieldsSOYear").show();
        $("#advanceFieldsSeqNo").show();
        $("#advanceFieldsCustOrderNo").show();
        $("#againstFieldsVoucherNo").hide();
        $("#againstFieldsVoucherDate").hide();
        $("#againstFieldsVoucheryearCode").hide();
        $("#againstFieldsVoucherType").hide();
        $("#againstFieldsVoucherEntryId").hide();
        $("#againstFieldsVoucherBillAmt").hide();
        $("#againstFieldsVoucherPendAmt").hide();
        $('#CustOrderNo').prop('disabled', false);
        $('#SoNo').prop('disabled', false);
        $('#SOYear').prop('disabled', false);
    }
    // Show/Hide Against Fields
    else if (modeOfAdjustment === "Against Ref") {
        if ($('#BankType').val() == "Bank" && $('#ModeOfAdjustment').find(":selected").text() == "Against Ref") {
            $('#DisplayRoutingDetailModal').modal('hide');
        }
        $("#againstFieldsVoucherNo").show();
        $("#againstFieldsVoucherDate").show();
        $("#againstFieldsVoucheryearCode").show();
        $("#againstFieldsVoucherType").show();
        $("#againstFieldsVoucherEntryId").show();
        $("#againstFieldsVoucherBillAmt").show();
        $("#againstFieldsVoucherPendAmt").show();
        $("#advanceFieldsSOYear").hide();
        $("#advanceFieldsSeqNo").hide();
        $("#advanceFieldsCustOrderNo").hide();
        $('#CustOrderNo').prop('disabled', true);
        $('#SoNo').prop('disabled', true);
        $('#SOYear').prop('disabled', true);
        if ($('#LedgerName').prop('selectedIndex') == 0) {
            $.notify("Please select LedgerName First..!", "error");
        }
        else {
            PopUpForPendingVouchers("AddMode");
        }
    }
    // Hide all fields for other values
    else {
        $("#advanceFieldsSOYear").hide();
        $("#advanceFieldsSeqNo").hide();
        $("#advanceFieldsCustOrderNo").hide();
        $("#againstFieldsVoucherNo").hide();
        $("#againstFieldsVoucherDate").hide();
        $("#againstFieldsVoucheryearCode").hide();
        $("#againstFieldsVoucherType").hide();
        $("#againstFieldsVoucherEntryId").hide();
        $("#againstFieldsVoucherBillAmt").hide();
        $("#againstFieldsVoucherPendAmt").hide();
        $('#CustOrderNo').prop('disabled', true);
        $('#SoNo').prop('disabled', true);
        $('#SOYear').prop('disabled', true);
    }
    if ($("#AccountCode").val() != "" && $("#ModeOfAdjustment").val() == "Advance") {

        FillSONO();
    }
}
$("#SONo").on("change", function () {
    GetSOYear();
    GetSODate();
});
$("#ModeOfAdjustment").on("change", function () {
    checkModeOfAdjustment();
});

function FillLedgerName() {
     
    var Type = $('#Type').val();
    $.ajax({
        url: '/JournalVoucher/FillLedgerName', // Hardcoded URL to avoid Razor issues
        type: 'GET',
        data: {
            Type: Type
        },
        async: false,
        success: function (response, status, error) {

            var LedgerNameDropdown = $('#LedgerName');
            var obj = $.parseJSON(response);
            LedgerNameDropdown.empty();
            LedgerNameDropdown.append('<option value="">--Select--</option>');
            $.each(obj.Result, function (index, LedgerName) {
                LedgerNameDropdown.append('<option value="' + LedgerName.Account_Code + '">' + LedgerName.Account_Name + '</option>');
            });

            if ($('#Mode').val() == "U" || $('#Mode').val() == "V") {

                var LedgerName = '@Model.AccountCode';
                $('#LedgerName').val(LedgerName).trigger("change");
                $('#LedgerName').val(LedgerName);
            }
        },
        error: function (xhr, status, error) {
            alert('Failed to load ledger names.');
        }
    });
}
function Validate_Submit() {

    var Err = 0;
    var DrAmt = parseFloat($('#DrAmt').val()) || 0;
    var CrAmt = parseFloat($('#CrAmt').val()) || 0;

    if (DrAmt != CrAmt) {
        $.notify("DrAmt and CrAmt must be equal to proceed.", "error");
        Err = 1;
    }
    var Row = $('#divBankReceiptDetail tr').length;
    if ($('#divBankReceiptDetail tr').text().trim() == "NO DATA ADDED") {
        $.notify("Grid should have at least one item.", "error");
        Err = 1;
    }
    var BankTypeData = [];
    for (let i = 1; i <= Row; i++) {
        BankTypeData.push({
            "BankType": $('#BankType-' + i).val()
        })
    }
    var hasBank = BankTypeData.some(item => item.BankType === "Bank");
    if (!hasBank) {
        $.notify("There should be at least one bank in grid.", "error");
        Err = 1;
    }

    for (let i = 1; i <= Row; i++) {
        var ModeOfAdjust = $('#ModeOfAdjustment-' + i).text();
        if (ModeOfAdjust == "Against Ref") {
            CheckAmountBeforeSave(i);
        }
    }
    Err == 1 ? ShowErrorFld('H_SAGrid') : HideErrorFld('H_SAGrid');
    if (Err == 1) return false;
    return true;
}