$(document).ready(function () {
	$(".ddlFilter").change(function () {
		GetItineraryBuilderSummary();
	});

	$(".remarks").click(function () {

		var BookingNumber = $('#tblItineraryBuilder #QRFID').val();
		var PositionId = $(this).parents('tr').find('.clsPositionId').val();
		ViewRemarks(BookingNumber, PositionId);
	});

	$(".addEditNewItinerary").click(function () {
		var QRFId = $('#tblItineraryBuilder #QRFID').val();
		var ItineraryDetailID = $(this).parents('tr').find('.clsItineraryDetailId').val();
		var ItineraryDaysId = $(this).parents('tr').find('.clsItineraryDaysId').val();
		var PositionId = $(this).parents('tr').find('.clsPositionId').val();
		var DayNo = $(this).parents('tr').find('.clsDay').val();
		var Action = $(this).parents('tr').find('.addEditNewItinerary').data('val');

		$.ajax({
			type: "POST",
			url: "/Proposal/AddNewItineraryDetails",
			data: { QRFId: QRFId, PositionId: PositionId, ItineraryID: ItineraryDetailID, ItineraryDaysId: ItineraryDaysId, Day: DayNo, action: Action, type: "opsItinerary" },
			success: function (response) {
				$("#addNewDetails-popup").html(response);
				$("#addNewDetails-popup #Day").addClass("disabled", true);
			},
			error: function (response) {
				alert(response);
			}
		});
	});

	$(".editPosition").click(function () {

		var BookingNumber = $('#tblItineraryBuilder #QRFID').val();
		var ItineraryDetailID = $(this).parents('tr').find('.clsItineraryDetailId').val();
		var ProdDescription = $(this).parents('tr').find('.clsProductName').val();

		$.ajax({
			type: "POST",
			url: "/Operations/EditItineraryDetails",
			data: { BookingNumber: BookingNumber, ItineraryDetailID: ItineraryDetailID, ProdDescription: ProdDescription },
			success: function (response) {

				$("#EditPositionDetails-popup").html(response);
			},
			error: function (response) {
				alert(response);
			}
		});
	});

	$(".enableDisablePosition").click(function () {

		var BookingNumber = $('#tblItineraryBuilder #QRFID').val();
		var ItineraryDetailID = $(this).parents('tr').find('.clsItineraryDetailId').val();
		var IsDeleted = $(this).parents('tr').find('.clsIsDeleted').val();
		$.ajax({
			type: "POST",
			url: "/Operations/DeleteItineraryDetails",
			data: { BookingNumber: BookingNumber, ItineraryDetailID: ItineraryDetailID, IsDeleted: IsDeleted },
			success: function (response) {

				GetItineraryBuilderSummary();
			},
			error: function (response) {
				alert(response);
			}
		});
	});

	$("#btnSave").click(function () {

		var model = $("#frmItineraryBuilder").serialize();
		$.ajax({
			type: "POST",
			url: "/Operations/SaveItineraryDetails",
			data: model,
			success: function (response) {

				GetItineraryBuilderSummary();
				if (response.status == "error") {
					$("#SuccessAlert").hide();
					$("#ErrorMessage").html(response.responseText);
					$("#ErrorAlert").show();
				}
				else {
					$("#ErrorAlert").hide();
					$("#SuccessMessage").html(response.responseText);
					$("#SuccessAlert").show();
				}
			},
			error: function (response) {
				alert(response);
			}
		});
	});

	$("#btnCreateFullItinerary").click(function () {
		debugger;
		$("#headerConfirm").html("Generate Full Itinerary");
		$("#pConfirmMsg").html("Are you sure you want to generate a full itinerary?");
		$("#confirmOk").attr("data-value", "fullItinerary");
		$("#Confirm-popup").show();
	});

});

function fullItinerary() {
	var posData = {};
	var BookingNumber = $("#BookingNumber").val();
	$("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

	var PositionId = $('#Position_Id').val();
	posData['BookingNumber'] = BookingNumber;
	posData['PositionId'] = PositionId;
	posData['Action'] = "FullItinerary";
	posData['Module'] = "Position";
	posData['ModuleParent'] = "Booking";
	posData['Status'] = "FullItinerary";

	$.ajax({
		type: "POST",
		url: "/Operations/SetBookingByWorkflow",
		data: { model: posData },
		dataType: "json",
		success: function (response) {
			debugger;
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

function ViewRemarks(BookingNumber, PositionId) {
	$.ajax({
		type: "GET",
		url: "/Operations/GetRemarksForPosition",
		data: { BookingNumber: BookingNumber, PositionId: PositionId },
		async: false,
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#remarks-popup").html(response);
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
}