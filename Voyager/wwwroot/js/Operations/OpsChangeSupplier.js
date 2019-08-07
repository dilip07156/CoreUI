$(document).ready(function () {
    $(".showSwitchSuppPopup").click(function () {
        var SupplierId = $("#ChangeSupplierId:checked").val();
        if (SupplierId == 'undefined' || SupplierId == null) {
            alert("Please select Supplier from List");
            return false;
        }
        $("#headerConfirm").html("Switch Supplier");
        $("#pConfirmMsg").html("Are you sure you want to change supplier?");
        $("#confirmOk").attr("data-value", "switchSupplier");


        $("#Confirm-popup").show();
    });

    $("#confirmOk").unbind("click");
    $("#confirmOk").click(function () {
        //$.magnificPopup.close();
        $('#confirmOk').magnificPopup('close');

        // 500, time must be bigger, as the delay from the first popup. By me the delay is 300
        var value = $("#confirmOk").attr("data-value");
        if (value === "switchSupplier") {
            //confirmPosition();
            window.setTimeout(switchSupplier, 400);
        }
    });
    
    $(".close-popup").unbind("click");
    $(".close-popup").click(function () {
        if ($(this).text() == "Ok") {
            var PositionId = $('#Position_Id').val();
            SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
        }
    });

    $('.rdoChangeSupplier').click(function (e) {
        $(".divChangeSuppAlrMsg").css('display', 'block');
    });

    function switchSupplier() {
        $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');
        var switchSupplierData = {};
        var SupplierId = $("#ChangeSupplierId:checked").val();
        var BookingNumber = $("#BookingNumber").val();
        var PositionId = $('#Position_Id').val();
        switchSupplierData['TableChangeSuppReal'] = true;
        switchSupplierData['BookingNumber'] = BookingNumber;
        switchSupplierData['PositionId'] = PositionId;
        switchSupplierData['SupplierId'] = SupplierId;

        switchSupplierData['Action'] = "ChangeSupplier_Real";
        switchSupplierData['Module'] = "Supplier";
        switchSupplierData['ModuleParent'] = "Position";

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: switchSupplierData },
            dataType: "json",
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
                        htmlMsg = '<div class="alert alert-success" role="alert">Supplier Switched Successfully</div>';
                    }
                } else {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred.</div>';
                }
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowMagnificPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
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
    }
});