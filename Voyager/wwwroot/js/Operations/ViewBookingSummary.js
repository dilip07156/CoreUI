$(document).ready(function () {
    $('.showFollowUp').click(function () {
        GetFollowup('792', "", "");
    });

    $(".showCancelBooking").click(function () {
        var BookingNumber = $("#BookingNumber").val();
        $("#CancelBooking-popup .popup-in").html('');
        $.ajax({
            type: "GET",
            url: "/Operations/CancelBooking",
            data: { BookingNumber: BookingNumber },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $("#CancelBooking-popup .popup-in").html(response);
                $("#CancelBooking-popup").show();
            },
            error: function (response) {
                alert(response.statusText);
            }
        });


    });

    $(".showShiftBooking").click(function () {
        var BookingNumber = $("#BookingNumber").val();
        $("#ShiftBooking-popup .popup-in").html('');
        $.ajax({
            type: "GET",
            url: "/Operations/ShiftBooking",
            data: { BookingNumber: BookingNumber },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $("#ShiftBooking-popup .popup-in").html(response);
                $("#ShiftBooking-popup").show();
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    });

    $("#showHotelConfirmation").click(function () {
        $("#headerConfirm").html("Send Hotel Confirmation");
        $("#pConfirmMsg").html("Are you sure you want to send the hotel confirmation to the Sales Officer?");
        $("#confirmOk").attr("data-value", "confirmbooking");
        $("#Confirm-popup").show();
    });

    $("#showDownloadVouchers").click(function () {
        $("#headerConfirm").html("Download Voucher");
        $("#pConfirmMsg").html("Are you sure you want to the Download Voucher?");
        $("#confirmOk").attr("data-value", "dwdvoucher");
        $("#Confirm-popup").show();
    }); 

    $(".showRoomChange").click(function () {
        var BookingNumber = $("#BookingNumber").val();
        $("#RoomChange-popup .popup-in").html('');
        $.ajax({
            type: "GET",
            url: "/Operations/RoomChange",
            data: { BookingNumber: BookingNumber },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $("#RoomChange-popup .popup-in").html(response);
                $("#RoomChange-popup").show();
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    });

    $('#BookingTable').DataTable({
        "ordering": false,
        "aLengthMenu": [10, 25, 50, 100],
        "iDisplayLength": 10,
        "searching": true,
        "lengthChange": true,
        "oLanguage": {
            sLengthMenu: "Show entries _MENU_",
        },
        "dom": "<'row'<'col-sm-3'l><'col-sm-6'p>>" + "<'row'<'col-sm-12'tr>>" + "<'row'<'col-sm-5 margin-top-bottom-20'i><'col-sm-7'p>>",
        initComplete: function () {
            this.api().columns(0).every(function () {
                var column = this;
                var select =
                    $("#FilterProdType").on('change', function () {

                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );
                        column
                            .search(val ? '^' + val + '$' : '', true, false)
                            .draw();
                    });
            });
        }
    });
});

function confirmBooking() {   
    var posData = {};
    var BookingNumber = $("#BookingNumber").val();
    $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

    posData['BookingNumber'] = BookingNumber;
    posData['Action'] = "HotelConfirm";
    posData['Module'] = "Booking";
    posData['ModuleParent'] = "Booking";
    posData['Status'] = "K";

    $.ajax({
        type: "POST",
        url: "/Operations/SetBookingByWorkflow",
        data: { model: posData },
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
                    htmlMsg = '<div class="alert alert-success" role="alert">Hotel Confirmation Sent.</div>';
                }
            } else {
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                htmlMsg = '<div class="alert alert-danger" role="alert">An error occurred when sending the Hotel Confirmation.</div>';
            }

            $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
            ShowPopup("#commonMsg-popup", true, true);
            $("#commonMsg-popup").show(); 
            $(".ajaxloader").hide();
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

function downloadPosVoucher() {
    var posData = {};

    var BookingNumber = $("#BookingNumber").val();
    $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

    posData['BookingNumber'] = BookingNumber;
    posData['Action'] = "DownloadVoucher";
    posData['Module'] = "Booking";
    posData['ModuleParent'] = "Booking";
    posData['Status'] = "VOUCHER";
    posData['BookingPositionDwdVoucher'] = "VOUCHER";

    $.ajax({
        type: "POST",
        url: "/Operations/SetBookingByWorkflow",
        data: { model: posData },
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
                    $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                    ShowPopup("#commonMsg-popup", true, true);
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
                htmlMsg = '<div class="alert alert-danger" role="alert">An error occurred while downloading the voucher.</div>';
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowPopup("#commonMsg-popup", true, true);
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

function GetFollowup(QRFID, SuccessMessage, ErrorMessage) {
    $("#FollowUpList-popup .popup-in").html("");
    $.ajax({
        type: "GET",
        url: "/Quote/GetQuoteFollowUp",
        data: { QRFID: QRFID },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {

            $("#FollowUpList-popup .popup-in").html(response);
            $("#FollowUpList-popup").show();
            if (SuccessMessage != "") {
                $("#divSuccessError").addClass("alert alert-success");
                $("#lblmsg").text(SuccessMessage);
                $("#stMsg").text("Success!");
                $("#divSuccessError").css("display", "block");
            }
            else if (ErrorMessage != "") {
                $("#divSuccessError").addClass("alert alert-danger");
                $("#lblmsg").text(ErrorMessage);
                $("#stMsg").text("Error!");
                $("#divSuccessError").css("display", "block");
            }
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

function ShowPopup(targetDiv, closeOnBgClick, enableEscapeKey) {
    //opens the popup 
    $.magnificPopup.open({
        type: 'inline',
        items: { src: targetDiv },
        fixedContentPos: true,
        fixedBgPos: true,
        overflowY: 'auto',
        closeBtnInside: true,
        preloader: false,
        midClick: true,
        removalDelay: 500,
        mainClass: 'my-mfp-slide-bottom',
        closeOnBgClick: closeOnBgClick,
        enableEscapeKey: enableEscapeKey
    });
}