var form = $("#frmProdTypePos");
form.removeData('validator');
form.removeData('unobtrusiveValidation');
$.validator.unobtrusive.parse(form);
selectInitialize();
EnableDisabledConditionalControls();

function openCloseNav() {
    var sidebarWidth = $("#mySidebar").css("width");
    if (sidebarWidth !== "0px") {
        document.getElementById("mySidebar").style.width = "0";
        document.getElementById("mainOps").style.marginLeft = "0";
        //document.getElementById("mySidebar").style.backgroundColor = "#fff";
    }
    else {
        document.getElementById("mySidebar").style.width = "20%";
        document.getElementById("mainOps").style.marginLeft = "20%";
        //document.getElementById("mySidebar").style.backgroundColor = "#f8f7f5";
    }
}

/*----------------------------communication trail starts here----------------------------------------------*/
$(".communitrail").click(function () {
    var thisparam = this;
    var model = { //Passing data
        Position_Id: $("#Position_Id").val(),
        BookingNumber: $("#BookingNumber").val(),
        ModuleType: "ops"
    };
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/Operations/GetCommunicationTrail", // Controller/View
        data: model,
        success: function (response) {
            $("#modelcommunitrail-popup").html(response);
            $(".btnview:first").click();
            $("#divemailhtml > style").remove();
            var targetDiv = $(thisparam).attr("href");
            ShowMagnificPopup(targetDiv, false, false);
        },
        error: function (jqXHR, exception) {
            var msg = getAjaxErrorStatusDescription(jqXHR, exception, errorThrown);
            $(".ajaxloader").hide();
            alert(msg);
        }
    });
});
/*----------------------------communication trail ends here----------------------------------------------*/

function updatePositionDetails() {
    debugger;
    if (positionFormData['TableOpsPosFOC'] != undefined && positionFormData['TableOpsPosFOC'] != null) {
        var TableOpsPosFOC = JSON.parse(positionFormData['TableOpsPosFOC']);
        var result = TableOpsPosFOC.filter(foc => foc.BuyQuantity != '' && foc.BuyBookingRoomsId != '' && foc.GetQuantity != '' && foc.GetBookingRoomsId != '');
        positionFormData['TableOpsPosFOC'] = JSON.stringify(result);
    }
    var isValid = ValidateFocTable();
    var isFormValid = $('#frmProdTypePos').valid();
    var invalidMsg = '', htmlMsg = '';

    $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

    if (isValid && isFormValid) {
        var BookingNumber = $('#BookingNumber').val();
        var PositionId = $('#Position_Id').val();

        positionFormData['BookingNumber'] = BookingNumber;
        positionFormData['PositionId'] = PositionId;
        positionFormData['ChargeBasis'] = $('#ChargeBasis').val();

        positionFormData['Action'] = "Save";
        positionFormData['Module'] = "Position";
        positionFormData['ModuleParent'] = "Booking";
        positionFormData['saveposition'] = "saveposition";

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: positionFormData },
            dataType: "json",
            success: function (response) {
                debugger;
                if (response != undefined && response != null && response != '') {
                    if (response.status == 'Success') {
                        $("#commonMsg-popup #headerCommonMsg").html('Success!');
                        htmlMsg = '<div class="alert alert-success" role="alert">Position Updated Successfully.</div>';
                    }
                    else {
                        $("#commonMsg-popup #headerCommonMsg").html('Error!');
                        var msg = response.msg;
                        msg = msg.filter(a => a != "" && a != undefined && a != null);
                        for (var i = 0; i < msg.length; i++) {
                            htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                        }
                    }
                    SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
                    $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                    ShowMagnificPopup("#commonMsg-popup", true, true);
                    $("#commonMsg-popup").show();
                }
            },
            error: function (response) {
                //alert(response.statusText);
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                htmlMsg = '<div class="alert alert-danger" role="alert">' + response.statusText + '</div>';
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowMagnificPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
            }
        });
    }
    else {
        if (!isValid) {
            invalidMsg = 'Please enter appropriate values in FOC';
        }
        else {
            invalidMsg = "Please enter valid values!!!";
        }

        $("#commonMsg-popup #headerCommonMsg").html('Error!');
        htmlMsg = '<div class="alert alert-danger" role="alert">' + invalidMsg + '</div>';
        $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
        ShowMagnificPopup("#commonMsg-popup", true, true);
        $("#commonMsg-popup").show();
    }
}

$(".showCancelPosition").click(function () {
    var BookingNumber = $("#BookingNumber").val();
    var PositionId = $("#Position_Id").val();
    $("#CancelPosition-popup .popup-in").html('');
    $.ajax({
        type: "GET",
        url: "/Operations/CancelPosition",
        data: { BookingNumber: BookingNumber, PositionId: PositionId },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $("#CancelPosition-popup .popup-in").html(response);
            $("#CancelPosition-popup").show();
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
});

$(".showContactSupplier").click(function () {
    var BookingNumber = $("#BookingNumber").val();
    var PositionId = $("#Position_Id").val();
    $("#ContactSupplier-popup .popup-in").html('');
    $.ajax({
        type: "GET",
        url: "/Operations/ContactSupplier",
        data: { BookingNumber: BookingNumber, PositionId: PositionId },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $("#ContactSupplier-popup .popup-in").html(response);
            $("#ContactSupplier-popup").show();
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
});

function confirmPosition() {
    $(".ajaxloader").show();
    var posConfirmData = {};
    var BookingNumber = $("#BookingNumber").val();
    $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

    var PositionId = $('#Position_Id').val();
    posConfirmData['BookingNumber'] = BookingNumber;
    posConfirmData['PositionId'] = PositionId;
    posConfirmData['Action'] = "Book";
    posConfirmData['Module'] = "Position";
    posConfirmData['ModuleParent'] = "Booking";
    posConfirmData['Status'] = "K";

    $.ajax({
        type: "POST",
        url: "/Operations/SetBookingByWorkflow",
        data: { model: posConfirmData },
        dataType: "json",
        global: false,
        success: function (response) {
            var htmlMsg = "";
            console.log(response);

            if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                if (response.status.toLowerCase() !== "success") {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    var msg = response.msg;
                    msg = msg.filter(a => a != "" && a != undefined && a != null);
                    for (var i = 0; i < msg.length; i++) {
                        htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                    }
                }
                else {
                    $("#commonMsg-popup #headerCommonMsg").html('Success!');
                    htmlMsg = '<div class="alert alert-success" role="alert">SERVICE CONFIRMED WITH SUPPLIER</div>';
                }
            } else {
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred When Book the Hotel.</div>';
            }
            SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
            $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
            ShowMagnificPopup("#commonMsg-popup", true, true);
            $("#commonMsg-popup").show();
            $(".ajaxloader").hide();
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = getAjaxErrorStatusDescription(jqXHR, exception, errorThrown);

            SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
            ShowMagnificPopup("#commonMsg-popup", true, true);
            $("#commonMsg-popup #headerCommonMsg").html('Error!');
            $("#commonMsg-popup #pCommonMsg").html('<div class="alert alert-danger" role="alert">' + msg + '</div>');
            $("#commonMsg-popup").show();
            $(".ajaxloader").hide();
        }
    });

}

function raisePosVoucher() {
    var posData = {};
    var BookingNumber = $("#BookingNumber").val();
    $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

    var PositionId = $('#Position_Id').val();
    posData['BookingNumber'] = BookingNumber;
    posData['PositionId'] = PositionId;
    posData['Action'] = "RaiseVoucher";
    posData['Module'] = "Position";
    posData['ModuleParent'] = "Booking";
    posData['Status'] = "voucher";

    $.ajax({
        type: "POST",
        url: "/Operations/SetBookingByWorkflow",
        data: { model: posData },
        dataType: "json",
        success: function (response) {
            var htmlMsg = "";
            if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                if (response.status.toLowerCase() !== "success") {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    var msg = response.msg;
                    msg = msg.filter(a => a != "" && a != undefined && a != null);
                    for (var i = 0; i < msg.length; i++) {
                        htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                    }
                    $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                    ShowMagnificPopup("#commonMsg-popup", true, true);
                    $("#commonMsg-popup").show();
                    $(".ajaxloader").hide();
                }
                else {
                    if (response.docname != null && response.docname != "" && response.docname.length > 0) {
                        $(".ajaxloader").hide();
                        $("#aFilePath").attr("href", "/Operations/DownloadFile?file=" + response.docname[0]);
                        $("#aFilePath")[0].click();
                    }
                    else {
                        $("#commonMsg-popup #headerCommonMsg").html('Error!');
                        htmlMsg = '<div class="alert alert-danger" role="alert">An error occurred while downloading the voucher.</div>';
                        $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                        ShowPopup("#commonMsg-popup", true, true);
                        $("#commonMsg-popup").show();
                        $(".ajaxloader").hide();
                    }
                }
            } else {
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred when raise the voucher.</div>';
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowMagnificPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            }
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = getAjaxErrorStatusDescription(jqXHR, exception, errorThrown);
            $("#commonMsg-popup #headerCommonMsg").html('Error!');
            $("#commonMsg-popup #pCommonMsg").html('<div class="alert alert-danger" role="alert">' + msg + '</div>');
            ShowPopup("#commonMsg-popup", true, true);
            $("#commonMsg-popup").show();
            $(".ajaxloader").hide();
        }
    });
}

$("#btnConfirmBookPosition").click(function () {
    $("#headerConfirm").html("Confirm Service");
    $("#pConfirmMsg").html("Are you sure want to confirm?");
    $("#confirmOk").attr("data-value", "confirmposition");
    $("#Confirm-popup").show();
});

$("#btnRaiseVoucher").click(function () {
    $("#headerConfirm").html("Raise Voucher");
    $("#pConfirmMsg").html("Are you sure you want to Raise a Voucher?");
    $("#confirmOk").attr("data-value", "raisevoucher");
    $("#Confirm-popup").show();
});

function loadSelectedPosition(positionId) {
    var bookingNumber = $("#BookingNumber").val();
    $('#divOpsDayInfo').empty();
    $('#divOpsDayInfo').load('/Operations/ViewProdTypePosition', { BookingNumber: bookingNumber, PositionId: positionId });
}

$('#PositionType, #PaxType').change(function () {
    EnableDisabledConditionalControls();
});

function EnableDisabledConditionalControls() {
    var PositionType = $('#PositionType').val();
    var StandardPax = $('#PaxType').val();

    if (PositionType != undefined && PositionType != null && PositionType != '' && StandardPax != undefined && StandardPax != null && StandardPax != '') {
        PositionType = PositionType.toUpperCase();
        StandardPax = StandardPax.toUpperCase();

        if ((PositionType == 'CORE' || PositionType == 'SUPPLEMENT') && StandardPax == 'TRUE') {
            //enable
            $('#PaxType').prop('disabled', false);

            //disable
            $('.RebuildRooming').prop('disabled', true);
            $('.btnAddNewRoom').prop('disabled', true);
            $('.btnAddNewRoomSupp').prop('disabled', true);
            $('.btnAddNewBudgSupp').prop('disabled', true);
            $('.btnDeletePrice').prop('disabled', true);
            $('.btnDeletePriceSupp').prop('disabled', true);
            $('.btnDeleteBudgSupp').prop('disabled', true);
            $('#tblPosRoomPrice').find('.ddlBuyBookingRoomsID').prop('disabled', true);
            $('#tblPosBudgSupp').find('.bdgSuppProductRangeID').prop('disabled', true);
            $('#tblPosBudgSupp').find('.clsBudgSuppAmt').prop('disabled', true);
            $('#tblPosRoomPrice').find('input[name*=".Req_Count"]').prop('disabled', true);
            $('#tblPosRoomPrice').find('input[name*=".BudgetPrice"]').prop('disabled', true);
        }
        else if ((PositionType == 'CORE' || PositionType == 'SUPPLEMENT') && StandardPax == 'FALSE') {
            //enable
            $('.RebuildRooming').prop('disabled', false);
            $('.btnAddNewRoom').prop('disabled', false);
            $('.btnAddNewRoomSupp').prop('disabled', false);
            $('.btnAddNewBudgSupp').prop('disabled', false);
            $('.btnDeletePrice').prop('disabled', false);
            $('.btnDeletePriceSupp').prop('disabled', false);
            $('.btnDeleteBudgSupp').prop('disabled', false);
            $('#tblPosRoomPrice').find('.ddlBuyBookingRoomsID').prop('disabled', false);
            $('#tblPosBudgSupp').find('.bdgSuppProductRangeID').prop('disabled', false);
            $('#tblPosBudgSupp').find('.clsBudgSuppAmt').prop('disabled', false);
            $('#tblPosRoomPrice').find('input[name*=".Req_Count"]').prop('disabled', false);
            $('#PaxType').prop('disabled', false);

            //disable
            $('#tblPosRoomPrice').find('input[name*=".BudgetPrice"]').prop('disabled', true);
        }
        else if (PositionType == 'OPTIONAL' || PositionType == 'PRE' || PositionType == 'POST') {
            //enable
            $('.RebuildRooming').prop('disabled', false);
            $('.btnAddNewRoom').prop('disabled', false);
            $('.btnAddNewRoomSupp').prop('disabled', false);
            $('.btnAddNewBudgSupp').prop('disabled', false);
            $('.btnDeletePrice').prop('disabled', false);
            $('.btnDeletePriceSupp').prop('disabled', false);
            $('.btnDeleteBudgSupp').prop('disabled', false);
            $('#tblPosRoomPrice').find('.ddlBuyBookingRoomsID').prop('disabled', false);
            $('#tblPosBudgSupp').find('.bdgSuppProductRangeID').prop('disabled', false);
            $('#tblPosBudgSupp').find('.clsBudgSuppAmt').prop('disabled', false);
            $('#tblPosRoomPrice').find('input[name*=".Req_Count"]').prop('disabled', false);
            $('#tblPosRoomPrice').find('input[name*=".BudgetPrice"]').prop('disabled', false);

            //disable
            $('#PaxType').val('false').prop('disabled', true);
        }
    }

    var PositionStatus = $('#PositionStatus').val();
    var IsRealSupplier = $('#IsRealSupplier').val();
    var IsPlaceholder = $("#Placeholder").val();
    if (PositionStatus != undefined && PositionStatus != null && PositionStatus != '') {
        if ('B,K'.indexOf(PositionStatus) > -1) $('#btnRaiseVoucher').show();
        if ('P,O'.indexOf(PositionStatus) > -1 && IsPlaceholder == 'False') $('#btnConfirmBookPosition').show();
        if ('O,B,K,M'.indexOf(PositionStatus) > -1) $('.showCancelPosition').show();
        if ('K,M,!'.indexOf(PositionStatus) > -1) $('.btnSupplierConfirmed').show();
        if ('E,P,O,M,K,B'.indexOf(PositionStatus) > -1 && IsRealSupplier == 'True') $('.showContactSupplier').show();
    }
}

$('#divPosRoomRates').on('click', '.btnAddNewRoom', function () {
    var totalRowCnt = $('#tblPosRoomPrice').find('.trPosRoomPrice').length;
    var hiddenRowCnt = $('#tblPosRoomPrice').find('.trPosRoomPrice[style="display: none;"]').length;
    var isNewRow = $('#tblPosRoomPriceSupp').find('tr:first').find('input[id*="IsNewRow"]').val();

    if (totalRowCnt == 1 && hiddenRowCnt == 1 && isNewRow == "True") {
        $('#tblPosRoomPrice').find('.trPosRoomPrice').show();
        $('#tblPosRoomPrice').find('input[name*=".BudgetPrice"]').prop('disabled', false);
    }
    else {
        var curRowCnt = $(".trPosRoomPrice").length - 1;
        var iRowCnt = curRowCnt + 1;
        var clonedRow = $("#tblPosRoomPrice").children('tr:first').clone();
        $("#tblPosRoomPrice").children('tr:last').after(clonedRow);
        $(clonedRow).show();

        $(".trPosRoomPrice:last").find('input,span,div,small,button,select').each(function (iColCnt) {

            if (this.id != undefined && this.id != "") {
                this.id = this.id.replace(0, iRowCnt);
            }

            if (this.name != undefined && this.name != "") this.name = this.name.replace(0, iRowCnt);

            if (this.aria_describedby != undefined) this.aria_describedby = this.aria_describedby.replace(0, iRowCnt);

            if (this.attributes['data-valmsg-for'] != undefined && this.attributes['data-valmsg-for'] != "") this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(0, iRowCnt);

            if (this.id != undefined && this.id != null) {
                if (this.id.indexOf('ChargeBasis') > -1)
                    $(this).val($('#BookingRoomsAndPrices_0__ChargeBasis').val());
                else if (this.id.indexOf('BookingRooms_Id') > -1 || this.id.indexOf('PositionPricing_Id') > -1)
                    this.value = NewGuid();
                else if (this.id.indexOf('BudgetPrice') > -1)
                    $(this).val('').prop('disabled', false);
                else if (this.id.indexOf('IsNewRow') > -1)
                    $(this).val('True');
                else
                    this.value = "";
            }
            else {
                this.value = "";
            }
        });
        $('.trPosRoomPrice:last').find('input[type=checkbox]:checked').removeAttr('checked');
        $(".trPosRoomPrice:last").attr("id", "trPosRoomPrice_" + iRowCnt);

        //Rebind form validation to enable new rows validation
        var form = $("#frmProdTypePos");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    }
});

$('#divPosRoomRates').on('click', '.btnAddNewRoomSupp', function () {
    var totalRowCnt = $('#tblPosRoomPriceSupp').find('.trPosRoomPriceSupp').length;
    var hiddenRowCnt = $('#tblPosRoomPriceSupp').find('.trPosRoomPriceSupp[style="display: none;"]').length;
    var isNewRow = $('#tblPosRoomPriceSupp').find('tr:first').find('input[id*="IsNewRow"]').val();

    if (totalRowCnt == 1 && hiddenRowCnt == 1 && isNewRow == "True") {
        $('#tblPosRoomPriceSupp').find('.trPosRoomPriceSupp').show();
        $('#tblPosRoomPriceSupp').find('input[name*=".BudgetPrice"]').prop('disabled', false);
    }
    else {
        var BookingRooms_Id = $("#tblPosRoomPriceSupp #BookingRooms_Id_0").val();
        var curRowCnt = $(".trPosRoomPriceSupp").length - 1;
        var iRowCnt = curRowCnt + 1;
        var clonedRow = $("#tblPosRoomPriceSupp").children('tr:first').clone();
        $(clonedRow).show();

        $("#tblPosRoomPriceSupp").children('tr:last').after(clonedRow);
        $(".trPosRoomPriceSupp:last").find(".ui-datepicker-trigger").remove();
        $(".trPosRoomPriceSupp:last").find('input,span,div,small,button,select').each(function (iColCnt) {
            if (this.id != undefined && this.id != "") {
                this.id = this.id.replace(0, iRowCnt);
            }

            if (this.name != undefined && this.name != "") this.name = this.name.replace(0, iRowCnt);

            if (this.aria_describedby != undefined) {
                this.aria_describedby = this.aria_describedby.replace(0, iRowCnt);
            }
            if (this.attributes['data-valmsg-for'] != undefined && this.attributes['data-valmsg-for'] != "") this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(0, iRowCnt);
            if (this.id != undefined && this.id != null) {
                if (this.id.indexOf('BookingRooms_Id') > -1 || this.id.indexOf('PositionPricing_Id') > -1)
                    this.value = NewGuid();
                else if (this.id.indexOf('IsNewRow') > -1)
                    $(this).val('True');
                else
                    this.value = "";
            }
            else {
                this.value = "";
            }
        });
        $('.trPosRoomPriceSupp:last').find('input[type=checkbox]:checked').removeAttr('checked');
        $(".trPosRoomPriceSupp:last").attr("id", "trPosRoomPriceSupp_" + iRowCnt);
        $("#OneOffDate_" + iRowCnt)
            .removeClass('hasDatepicker')
            .removeData('datepicker')
            .unbind()
            .datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "both",
                dateFormat: "dd/mm/yy",
                minDate: new Date()
            });

        //Rebind form validation to enable new rows validation
        var form = $("#frmProdTypePos");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    }
});

$('#divPosRoomRates').on('click', '.btnAddNewBudgSupp', function () {
    var totalRowCnt = $('#tblPosBudgSupp').find('.trPosBudgSupp').length;
    var hiddenRowCnt = $('#tblPosBudgSupp').find('.trPosBudgSupp[style="display: none;"]').length;
    var isNewRow = $('#tblPosBudgSupp').find('tr:first').find('input[id*="IsNewRow"]').val();

    if (totalRowCnt == 1 && hiddenRowCnt == 1 && isNewRow == "True") {
        $('#tblPosBudgSupp').find('.trPosBudgSupp').show();
        //$('#tblPosBudgSupp').find('input[name*=".BudgetPrice"]').prop('disabled', false);
    }
    else {
        var curRowCnt = $(".trPosBudgSupp").length - 1;
        var iRowCnt = curRowCnt + 1;
        var clonedRow = $("#tblPosBudgSupp").children('tr:first').clone();
        $("#tblPosBudgSupp").children('tr:last').after(clonedRow);
        $(clonedRow).show();

        $(".trPosBudgSupp:last").find('input,span,div,small,button,select').each(function (iColCnt) {

            if (this.id != undefined && this.id != "") {
                this.id = this.id.replace(0, iRowCnt);
            }

            if (this.name != undefined && this.name != "") this.name = this.name.replace(0, iRowCnt);

            if (this.aria_describedby != undefined) this.aria_describedby = this.aria_describedby.replace(0, iRowCnt);

            if (this.attributes['data-valmsg-for'] != undefined && this.attributes['data-valmsg-for'] != "") this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(0, iRowCnt);

            if (this.id != undefined && this.id != null) {
                if (this.id.indexOf('BudgetSupplement_Id') > -1)
                    this.value = NewGuid();
                else if (this.id.indexOf('IsNewRow') > -1)
                    $(this).val('True');
                else
                    this.value = "";
            }
            else {
                this.value = "";
            }
        });
        $('.trPosBudgSupp:last').find('input[type=checkbox]:checked').removeAttr('checked');
        $(".trPosBudgSupp:last").attr("id", "trPosRoomPrice_" + iRowCnt);

        //Rebind form validation to enable new rows validation
        var form = $("#frmProdTypePos");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    }
});

$('.ConfirmedOnBudget').click(function () {
    $('#tblPosRoomPrice,#tblPosRoomPriceSupp').find('tr').each(function (i) {
        var BudgetPrice = $(this).find('input[id*="BudgetPrice"]').val();
        var ConfirmedReqPrice = $(this).find('input[name*="ConfirmedReqPrice"]');

        if (BudgetPrice != undefined && BudgetPrice != null && BudgetPrice != '') {
            var RequestedPrice = $(this).find('input[id*="RequestedPrice"]');
            var BuyPrice = $(this).find('input[id*="BuyPrice"]');

            if (RequestedPrice != undefined && (RequestedPrice.val() == null || RequestedPrice.val() == '')) RequestedPrice.val(BudgetPrice);
            if (BuyPrice != undefined && (BuyPrice.val() == null || BuyPrice.val() == '')) BuyPrice.val(BudgetPrice);
        }
        ConfirmedReqPrice.prop('checked', true).change();
    });
});

function ConfirmSupplier() {
    var SuppConfirmNumber = $('#SuppConfirmNumber').val();
    if (SuppConfirmNumber != undefined && SuppConfirmNumber != null && SuppConfirmNumber.trim() != '') {
        $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');
        var posConfirmData = {};
        var BookingNumber = $("#BookingNumber").val();
        var PositionStatus = $('#PositionStatus').val();

        if (PositionStatus == "K" || PositionStatus == "M") {
            PositionStatus = "B";
        }
        else if (PositionStatus == "!") {
            PositionStatus = "C";
        }

        var PositionId = $('#Position_Id').val();
        posConfirmData['BookingNumber'] = BookingNumber;
        posConfirmData['PositionId'] = PositionId;
        posConfirmData['Action'] = "Save";
        posConfirmData['Module'] = "Position";
        posConfirmData['ModuleParent'] = "Booking";
        posConfirmData['Status'] = PositionStatus;
        posConfirmData['SupplierConfirmation'] = SuppConfirmNumber;

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: posConfirmData },
            dataType: "json",
            success: function (response) {
                debugger;
                var htmlMsg = "";
                console.log(response);
                if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                    if (response.status.toLowerCase() !== "success") {
                        $("#commonMsg-popup #headerCommonMsg").html('Error!');
                        var msg = response.msg;
                        msg = msg.filter(a => a != "" && a != undefined && a != null);
                        for (var i = 0; i < msg.length; i++) {
                            htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                        }
                        $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                        ShowMagnificPopup("#commonMsg-popup", true, true);
                        $("#commonMsg-popup").show();
                    }
                    else {
                        $("#commonMsg-popup #headerCommonMsg").html('Success!');
                        htmlMsg = '<div class="alert alert-success" role="alert">SUPPLIER CONFIRMED</div>';
                        SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
                        $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                        ShowMagnificPopup("#commonMsg-popup", true, true);
                        $("#commonMsg-popup").show();
                    }
                } else {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred.</div>';
                    $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                    ShowMagnificPopup("#commonMsg-popup", true, true);
                    $("#commonMsg-popup").show();
                }
                $(".ajaxloader").hide();
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = "";
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status === 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status === 500) {
                    msg = 'Internal Server Error [500].';
                } else if (exception === 'parsererror') {
                    msg = 'Requested JSON parse failed.';
                } else if (exception === 'timeout') {
                    msg = 'Time out error.';
                } else if (exception === 'abort') {
                    msg = 'Ajax request aborted.';
                } else if (jqXHR.status === 401) {
                    msg = '401 : Session Timeout';
                } else {
                    msg = 'Uncaught Error.\n' + jqXHR.status + ' : ' + errorThrown;
                }
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                $("#commonMsg-popup #pCommonMsg").html('<div class="alert alert-danger" role="alert">' + msg + '</div>');
                ShowPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            }
        });

        $('#confirmOk').magnificPopup('close');
        $('#SuppConfirmNumber-error').hide();
    }
    else {
        $('#SuppConfirmNumber-error').show();
    }
}

$('#divPosRoomRates').on('click', '.btnDeletePrice', function () {
    var curRow = $(this).closest('tr');
    var isNewRow = curRow.find('input[id*="IsNewRow"]').val();
    var totalRowCnt = $('#tblPosRoomPrice').find('.trPosRoomPrice').length;

    if (totalRowCnt == 1 && isNewRow == "True") {
        $('#tblPosRoomPrice').find('.trPosRoomPrice').hide().find('input,select').val('');
    }
    else {
        if (isNewRow == 'True') {
            curRow.remove();
        }
        else {
            curRow.hide();
            curRow.find('input[id*="Status"]').val('X');
        }
    }

    positionFormData = addChangedValueToList($("#tblPosRoomPrice").find('tr:first'), positionFormData);
});

$('#divPosRoomRates').on('click', '.btnDeletePriceSupp', function () {
    var curRow = $(this).closest('tr');
    var isNewRow = curRow.find('input[id*="IsNewRow"]').val();
    var totalRowCnt = $('#tblPosRoomPriceSupp').find('.trPosRoomPriceSupp').length;

    if (totalRowCnt == 1 && isNewRow == "True") {
        $('#tblPosRoomPriceSupp').find('.trPosRoomPriceSupp').hide().find('input,select').val('');
    }
    else {
        if (isNewRow == 'True') {
            curRow.remove();
        }
        else {
            curRow.hide();
            curRow.find('input[id*="Status"]').val('X');
        }
    }

    positionFormData = addChangedValueToList($("#tblPosRoomPriceSupp").find('tr:first'), positionFormData);
});

$('#divPosRoomRates').on('click', '.btnDeleteBudgSupp', function () {
    debugger;
    var curRow = $(this).closest('tr');
    var isNewRow = curRow.find('input[id*="IsNewRow"]').val();
    var totalRowCnt = $('#tblPosBudgSupp').find('.trPosBudgSupp').length;

    if (totalRowCnt == 1 && isNewRow == "True") {
        $('#tblPosBudgSupp').find('.trPosBudgSupp').hide().find('input,select').val('');
    }
    else {
        if (isNewRow == 'True') {
            curRow.remove();
        }
        else {
            curRow.hide();
            curRow.find('input[id*="status"]').val('X');
        }
    }

    positionFormData = addChangedValueToList($("#tblPosBudgSupp").find('tr:first'), positionFormData);
});

$(document).off("change").on('change', '.bdgSuppProductRangeID', function (evt) {
    var currentRow = $(this).closest("tr");
    var ProductRangeID = $(this).val();

    $("#tblPosRoomPrice .trPosRoomPrice").each(function (index) {
        var rangeId = $(this).find(".ddlBuyBookingRoomsID").val();
        if (ProductRangeID === rangeId) {
            $(currentRow).find(".clsBookingRoomsId").val($(this).find(".clsBookingRoomsId").val());
            $(currentRow).find(".clsPositionPricingId").val($(this).find(".clsPositionPricingId").val());
        }
    });
    positionFormData = addChangedValueToList($("#tblPosBudgSupp").find('tr:first'), positionFormData);
});

function GetBudgetSupplement(current) {
    var currentRow = $(current).closest("tr");
    var ProductRangeID = $(currentRow).find(".ddlBuyBookingRoomsID").val();
    var BudgetPrice = parseFloat($(currentRow).find(".clBudgetPrice").val());
    var BuyPrice = parseFloat($(current).val());

    if (BuyPrice > BudgetPrice) {
        var IsRangeFound = false;
        $("#tblPosBudgSupp .trPosBudgSupp").each(function (index) {
            var rangeId = $(this).find(".bdgSuppProductRangeID").val();
            if (ProductRangeID === rangeId) {
                $(this).find(".clsBookingRoomsId").val($(currentRow).find(".clsBookingRoomsId").val());
                $(this).find(".clsPositionPricingId").val($(currentRow).find(".clsPositionPricingId").val());
                $(this).find(".clsBudgSuppAmt").val(BuyPrice - BudgetPrice);
                IsRangeFound = true;
            }
        });
        if (IsRangeFound == false) {
            $(".btnAddNewBudgSupp").click();
            $('.trPosBudgSupp:last').find(".clsBookingRoomsId").val($(currentRow).find(".clsBookingRoomsId").val());
            $('.trPosBudgSupp:last').find(".clsPositionPricingId").val($(currentRow).find(".clsPositionPricingId").val());
            $('.trPosBudgSupp:last').find(".clsBudgSuppAmt").val(BuyPrice - BudgetPrice);
            $('.trPosBudgSupp:last').find(".bdgSuppProductRangeID").val(ProductRangeID);
        }
        positionFormData = addChangedValueToList($("#tblPosBudgSupp").find('tr:first'), positionFormData);
    }
    else {
        $("#tblPosBudgSupp .trPosBudgSupp").each(function (index) {
            var rangeId = $(this).find(".bdgSuppProductRangeID").val();
            if (ProductRangeID === rangeId) {
                $(this).find(".btnDeleteBudgSupp").click();
            }
        });
    }

    BuyPrice = $(current).val();
    if (BuyPrice == undefined || BuyPrice == null || BuyPrice == '') {
        $(current).addClass('error');
    }
    else {
        $(current).removeClass('error');
    }
}

function AddRemoveRangeToBdgSupp() {
    //Add Range
    $("#tblPosRoomPrice .trPosRoomPrice").each(function (index) {
        var rangeId = $(this).find(".ddlBuyBookingRoomsID").val();
        var range = $(this).find(".ddlBuyBookingRoomsID option:selected").text();

        $("#tblPosBudgSupp .trPosBudgSupp").each(function (index) {
            var optionExists = $("#" + this.id + ' .bdgSuppProductRangeID option[value=' + rangeId + ']').length > 0;

            if (!optionExists) {
                $(this).find('.bdgSuppProductRangeID').append("<option value='" + rangeId + "'>" + range + "</option>");
            }

        });

    });

    //Remove Range
    var IsRangeFound = false;
    $("#tblPosBudgSupp .trPosBudgSupp").each(function (index) {
        debugger;
        var currentRow = this.id;
        IsRangeFound = false;
        $("#" + currentRow + " .bdgSuppProductRangeID > option").each(function () {
            //alert(this.text + ' ' + this.value);
            IsRangeFound = false;
            var rangeIdSupp = this.value;

            if (rangeIdSupp != "") {
                $("#tblPosRoomPrice .trPosRoomPrice").each(function (index) {
                    debugger;
                    var rangeId = $(this).find(".ddlBuyBookingRoomsID").val();
                    if (rangeIdSupp == rangeId) {
                        IsRangeFound = true;
                    }
                });
                if (IsRangeFound == false) {
                    $("#" + currentRow + " .bdgSuppProductRangeID  option[value=" + rangeIdSupp + "]").remove();
                }
            }
        });
    });

    positionFormData = addChangedValueToList($("#tblPosBudgSupp").find('tr:first'), positionFormData);
}

$('#btnUpdatePosition').click(function () {
    debugger;
    var PositionStatus = $('#PositionStatus').val();
    var flag = false;

    if (positionFormData.hasOwnProperty('StartDate') || positionFormData.hasOwnProperty('EndDate') || positionFormData.hasOwnProperty('StartTime') ||
        positionFormData.hasOwnProperty('EndTime') || positionFormData.hasOwnProperty('TableRoomsAndRates') ||
        positionFormData.hasOwnProperty('TableAdditionalSuppliments') || positionFormData.hasOwnProperty('TableBudgetSuppliments') ||
        positionFormData.hasOwnProperty('TableOpsPosFOC')) {
        flag = true;
    }


    if ('K,B,M,O'.indexOf(PositionStatus) > -1 && flag == true) {
        positionFormData['SendBookAmend'] = 'True';
        $('#btnUpdatePosition').addClass('popup-inline');
        $("#btnUpdatePosition").attr("href", "#Confirm-popup");
        $("#headerConfirm").html("Email to Supplier");
        $("#pConfirmMsg").html("Do you want to send email to the Supplier?");
        $("#confirmOk").attr("data-value", "updatePosition");
        $("#confirmCancel").attr("data-value", "updatePosition");
        $("#Confirm-popup").show();
    }
    else {
        updatePositionDetails();
    }
});