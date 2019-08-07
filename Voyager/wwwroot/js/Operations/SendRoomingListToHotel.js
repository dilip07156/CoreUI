var positionFormData = {};

$(document).ready(function () {
    $('#CheckAll').click(function () {
        if ($(this).hasClass('allChecked')) {
            $('input[type="checkbox"]:not(:disabled)', '.divIsRoomChecked').prop('checked', false);
            $("#spCheckAll").text('Select All');
        } else {
            $('input[type="checkbox"]:not(:disabled)', '.divIsRoomChecked').prop('checked', true);
            $("#spCheckAll").text('Unselect All');
        }
        $(this).toggleClass('allChecked');
    });

    $(".close-alert").click(function (e) {
        e.preventDefault();
        $(this).parent("div").hide();
    });
});

$(document).find("#divSendRoomingListToHotel input").on('change input', function (e) {
    positionFormData = addChangedValueToList(this, positionFormData);
});

function sendRoominListToHotels() {
     
    $("#divError").hide();
    $("#divSuccess").hide();

    var tblRoomsingListToHotel = positionFormData['TableRoomsingListToHotel'];
    if ((!$("#TableRoomsingListToHotel input:checkbox:checked:not(:disabled)").length > 0) || tblRoomsingListToHotel == "" || tblRoomsingListToHotel == null || tblRoomsingListToHotel == undefined) {
        $("#divError").show();
        $("#spanErrorMsg").html('Please Select Atleast One Hotel.');
        return true;
    }
    else {
        var BookingNumber = $("#BookingNumber").val();

        positionFormData['BookingNumber'] = BookingNumber;
        positionFormData['Action'] = "SendRoomingListToHotel";
        positionFormData['Module'] = "Position";
        positionFormData['ModuleParent'] = "Booking";
        positionFormData['Status'] = "rooming";

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: positionFormData },
            dataType: "json",
            success: function (response) {
                var htmlMsg = "";
                console.log(response);
                if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                    if (response.status.toLowerCase() !== "success") {
                        var msg = response.msg;
                        msg = msg.filter(a => a != "" && a != undefined && a != null);
                        for (var i = 0; i < msg.length; i++) {
                            htmlMsg += msg[i];
                            if (msg.length != (i + 1)) { 
                                htmlMsg += '<br/>';
                            }
                        }
                        $("#divError").show();
                        $("#spanErrorMsg").html('There was a problem sending the Rooming List');

                        $('input[type="checkbox"]:not(:disabled)', '.divIsRoomChecked').prop('checked', false);
                        $("#spCheckAll").text('Select All');
                    }
                    else {
                        $("#divSuccess").show();
                        $("#spanSuccessMsg").html('Rooming List Sent to Hotel.');
                    }
                } else {
                    $("#divError").show();
                    $("#spanErrorMsg").html('There was a problem sending the Rooming List.');
                }
                $(".ajaxloader").hide();
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = getAjaxErrorStatusDescription(jqXHR, exception, errorThrown);
                $("#divError").show();
                $("#spanErrorMsg").html(msg);
                $(".ajaxloader").hide();
            }
        });
    }
}
