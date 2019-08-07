$(document).ready(function () {
	//Remarks related functionality  

	$('.close-popup').click(function (e) {
		e.preventDefault();
		$.magnificPopup.close()
	});

	$("#remarksSave").click(function () {
		 
		var QRFId = $("#QRFId").val();
		var selectedDay = $("#Days option:selected").text();
		var selectedServiceType = $("#Services option:selected").text();
		var BookingNumber = $("#BookingNumber").val();

		var data = {
			QRFId: QRFId,
			ItineraryId: $("#ItineraryId").val(),
			ItineraryDaysId: $("#ItineraryDaysId").val(),
			PositionId: $("#PositionId").val(),
			TLRemarks: $("#TLRemarks").val(),
			OPSRemarks: $("#OPSRemarks").val(),
			BookingNumber: BookingNumber
		}
		$.ajax({
			type: "POST",
			url: "/QRFSummary/SaveRemarksForPosition",
			data: data,
			dataType: "json",
			success: function (response) {
				 
				$("#SuccessMessage").html(response.responseText);
				$("#SuccessAlert").show();
				if (BookingNumber == null || BookingNumber == 'undefined' || BookingNumber == undefined || BookingNumber == "" || BookingNumber == " ") {
					GetDefaultItinerary();
				}
				else {
					 
					GetItineraryBuilderSummary();
				}
			},
			failure: function (response) {
				$("#ErrorMessage").html(response.responseText);
				$("#ErrorAlert").show();
			},
			error: function (response) {
				$("#ErrorMessage").html(response.responseText);
				$("#ErrorAlert").show();
			}
		});
	});
});
