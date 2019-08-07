$(document).ready(function () {
	 
	if ($("#Flag").val() == "True" || $("#Flag").val() == true)
		$(".display").css('display', 'none');

	$("#Day").change(function () {
		var selectedDay = $("#Day option:selected").val();
		$("#City").val(selectedDay);
		$("#ItineraryDaysId").val(selectedDay);
	});

	$('.close-popup').click(function (e) {
		e.preventDefault();
		$.magnificPopup.close()
	});

	$("#btnExtraItinerarySave").click(function () {
		 
		var flag = false;
		var startTime = $("#StartTime").val();
		var endTime = $("#EndTime").val();
		var prodName = $("#ProductName").val();

		if ($("#Flag").val() != "True" && $("#Flag").val() != true) {
			if ((startTime != "" || startTime != null) && (endTime != "" || endTime != null)) {
				if (startTime > endTime) {
					$("#spanEndTime").text("End Time should be greater than Start Time");
					flag = true;
				}
			}

			if (prodName == "" || prodName == null) {
				$("#spanProductName").text("Description is required");
				flag = true;
			}
			else
				$("#spanProductName").text("");

			if (startTime == "" || startTime == null) {
				$("#spanStartTime").text("Start Time is required");
				flag = true;
			}

			if (endTime == "" || endTime == null) {
				$("#spanEndTime").text("End Time is required");
				flag = true;
			}
		}
		if (!flag) {
			$(".ajaxloader").show();
			var data = {
				QRFId: $("#QRFId").val(),
				ItineraryID: $(this).parents('#frmAddNewItinerary').find('.clsItineraryID').val(), //$("#ItineraryID").val(),
				ItineraryDaysId: $(this).parents('#frmAddNewItinerary').find('.clsItineraryDaysId').val(),
				PositionId: $(this).parents('#frmAddNewItinerary').find('.clsPositionId').val(), //$("#PositionId").val(),
				Day: $("#Day option:selected").text(),
				StartTime: $("#StartTime").val(),
				EndTime: $("#EndTime").val(),
				City: $("#City option:selected").text(),
				ProductName: $("#ProductName").val(),
				flag: $("#Flag").val()
			};
			$.ajax({
				type: "POST",
				url: "/Proposal/SaveNewItineraryElement",
				data: data,
				global: false,
				success: function (response) {
					 
					if ($("#Type").val() != null && $("#Type").val() != "") {
						if ($("#Type").val() == "proposal") {
							GetProposalData();
							$.magnificPopup.close();
						}
						else if ($("#Type").val() == "summary") {
							if (response.status == "error") {
								$("#SuccessAlert").hide();
								$("#ErrorMessage").html(response.responseText);
								$("#ErrorAlert").show();
							}
							else {
								GetDefaultItinerary();
								$("#ErrorAlert").hide();
								$("#SuccessMessage").html(response.responseText);
								$("#SuccessAlert").show();
							}
						}
						else if ($("#Type").val() == "opsItinerary") {
							$('.clsItineraryID').val(response.itineraryDetailId);
							GetItineraryBuilderSummary();
							$("#SuccessMessage").html(response.responseText);
							$("#SuccessAlert").show();							
						}
						else {							
							if (response.status == "error") {
								$("#SuccessAlert").hide();
								$("#ErrorMessage").html(response.responseText);
								$("#ErrorAlert").show();
							}
							else {
								GetItineraryData();
								$("#ErrorAlert").hide();
								$("#SuccessMessage").html(response.responseText);
								$("#SuccessAlert").show();
							}
						}
					}
					$(".ajaxloader").hide();
				},
				error: function () {
					$(".ajaxloader").hide();
				}
			});
		}
		else
		{ return false; }
	});
});

function GetProposalData() {
	var QRFId = $("#QRFId").val();
	$.ajax({
		type: "GET",
		url: "/Proposal/ProposalSuggestedItinerary",
		data: { QRFId: QRFId },
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#divItinerary").html(response);
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
}
