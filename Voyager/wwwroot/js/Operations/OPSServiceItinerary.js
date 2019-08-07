$(document).ready(function () {
    $(".ddlItineraryFilter").click(function () {
        var ddlDay = $("#ddlDays").val();
        var ddlProdType = $("#ddlServiceType").val().toUpperCase();
        var ddlStatus = $("#ddlBookingStaus").val();

        $("#tbodyOpsItinerary").find('tr').show();
        var objTrDays = $("#tbodyOpsItinerary").find('tr');
        if (ddlDay != "") {
            $("#tbodyOpsItinerary").find('tr[trDayName !="' + ddlDay + '"]').hide();
            objTrDays = $("#tbodyOpsItinerary").find('tr[trDayName ="' + ddlDay + '"]');
            objTrDays.show();
        }

        if (ddlProdType != "") {
            objTrDays.filter('tr[trprodtype !="' + ddlProdType + '"]').hide();
        }

        if (ddlStatus != "") {
            objTrDays.filter('tr[trposstatus !="' + ddlStatus + '"]').hide();
        }
        if (ddlDay != "") {
            $("#tbodyOpsItinerary").find('tr.trDayHeader[trDayName ="' + ddlDay + '"]').show();
            $("#tbodyOpsItinerary").find('tr.trDayFooter[trDayName ="' + ddlDay + '"]').show();
        }
        else {
            $("#tbodyOpsItinerary").find('tr.trDayHeader').show();
            $("#tbodyOpsItinerary").find('tr.trDayFooter').show();
        }
    });

    $(".showAddService").click(function () { 
        var currentDay = $(this).attr("data-value");
        var BookingNumber = $("#BookingNumber").val();
        $("#AddService-popup .popup-in").html('');
        $.ajax({
            type: "GET",
            url: "/Operations/OPSAddService",
            data: { BookingNumber: BookingNumber },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $("#AddService-popup .popup-in").html(response);
                $("#AddService-popup").show();
                $("#divAddPosition #PositionStartDate").val(currentDay);
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    });

	$(".showCancelPosition").click(function () {
		 
		var BookingNumber = $("#BookingNumber").val();
		var PositionId = $(this).attr("PosId");
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
});