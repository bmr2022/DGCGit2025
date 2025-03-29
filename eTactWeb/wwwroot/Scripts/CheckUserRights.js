
function disableLinks(selector) {
    $(selector)
        .addClass("disabled-link")
        .prop("disabled", true) // Disables buttons if applied
        .off("click")  // Removes previous click events
        .on("click", function (e) {
            e.preventDefault(); // Prevents navigation
        });
}
function GetFormRightsSO() {

    $.ajax({
        url: "/SaleOrder/GetFormRights",
        type: "POST",
        
        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    $("#SaleOrder")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                    $("#SaleOrderDashBoard")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            $("#SaleOrder")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            $("#SaleOrderDashBoard")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


function GetFormRightsSOAmm() {

    $.ajax({
        url: "/SaleOrder/GetFormRightsAmm",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    $("#SaleOrderAmendment")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                    $("#SaleOrderAmendmentDashBoard")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            $("#SaleOrderAmendment")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            $("#SaleOrderAmendmentDashBoard")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


function GetFormRightsSaleSch() {

    $.ajax({
        url: "/SaleSchedule/GetFormRights",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    $("#SaleSchedule")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                    $("#SaleScheduleDashBoard")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            $("#SaleSchedule")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            $("#SaleScheduleDashBoard")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}

function GetFormRightsSaleSchAmm() {

    $.ajax({
        url: "/SaleSchedule/GetFormRightsAmen",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    $("#SaleScheduleAmendment")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                    $("#SaleScheduleAmendmentDash")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            $("#SaleScheduleAmendment")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            $("#SaleScheduleAmendmentDash")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


function GetFormRightsPO() {

    $.ajax({
        url: "/PurchaseOrder/GetFormRights",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    $("#PO")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                    $("#PODashBoard")
                        .addClass("disabled-link")
                        .on("click", function (e) {
                            e.preventDefault(); // Prevents clicking the link
                        });
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            $("#PO")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            $("#PODashBoard")
                                .addClass("disabled-link")
                                .on("click", function (e) {
                                    e.preventDefault(); // Prevents clicking the link
                                });
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


function GetFormRightsPOSch() {

    $.ajax({
        url: "/PurchaseSchedule/GetFormRights",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    disableLinks("#PurchaseSchedule, #PurchaseScheduleDashBoard");
                    
                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            disableLinks("#PurchaseSchedule");
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            disableLinks("#PurchaseScheduleDashBoard");
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}

function GetFormRightsPOAmm() {

    $.ajax({
        url: "/PurchaseOrder/GetFormRightsAmm",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    disableLinks("#PurchaseOrderAmendment, #PurchaseOrderAmendmentdash");

                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            disableLinks("#PurchaseOrderAmendment");
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            disableLinks("#PurchaseOrderAmendmentdash");
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


function GetFormRightsPOSchAmm() {

    $.ajax({
        url: "/PurchaseSchedule/GetFormRightsAmm",
        type: "POST",

        success: function (data) {

            if (data != null) {
                var obj = $.parseJSON(data);
                if (obj.Result.Table.length == 0) {
                    disableLinks("#PurchaseScheduleAmendment, #PurchaseScheduleAmendmentdash");

                } else {
                    console.log(obj);
                    var optAll = obj.Result.Table[0].OptAll;
                    var optSave = obj.Result.Table[0].OptSave;
                    var optUpdate = obj.Result.Table[0].OptUpdate;
                    var optDelete = obj.Result.Table[0].OptDelete;
                    var optView = obj.Result.Table[0].OptView;
                    if (!optAll) {
                        //$("#hidesoForm").hide();
                        if (!optSave) {
                            disableLinks("#PurchaseScheduleAmendment");
                        }
                        if (!(optUpdate || optDelete || optView)) {
                            disableLinks("#PurchaseScheduleAmendmentdash");
                        }
                    }
                }
            }
        },
        error: function (errMsg) {
            $.notify(errMsg, { position: "top center", className: "error" });
        }
    });
}


$(document).ready(function () {

    GetFormRightsSO();
    GetFormRightsSOAmm();
    GetFormRightsSaleSch();
    GetFormRightsSaleSchAmm();
    GetFormRightsPO();
    GetFormRightsPOSch();
    GetFormRightsPOAmm();
    GetFormRightsPOSchAmm();

});













