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