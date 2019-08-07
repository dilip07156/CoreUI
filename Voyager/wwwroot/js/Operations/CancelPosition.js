$(document).ready(function () {
	var form = $("#frmCancelPosition");
	form.removeData('validator');
	form.removeData('unobtrusiveValidation');
	$.validator.unobtrusive.parse(form);	
});

$(".btnCancel").click(function () {
	
	if ($('#frmCancelPosition').valid()) {
		 		
		$("#headerConfirm").html("Cancel Position");
		$("#pConfirmMsg").html("Are you sure you want to Cancel a position?");
		$("#confirmOk").attr("data-value", "cancelPosition");
		$("#Confirm-popup").show();
	}
	else
		return false;	
});

$(".btnMaintain").click(function () {
	if ($('#frmCancelPosition').valid()) {
		$("#headerConfirm").html("Cancel Position");
		$("#pConfirmMsg").html("Are you sure you want to Cancel a position?");
		$("#confirmOk").attr("data-value", "changeProduct");
		$("#Confirm-popup").show();
	}
	else
		return false;	
});

$(document).find("#cancelPositionSection select, #cancelPositionSection textarea, #cancelPositionSection input").on('change input', function (e) {
	positionFormData = addChangedValueToList(this, positionFormData);
});

function ChangeProduct() {
	debugger;
	showSupplier();
}

function showSupplier(){
	
		debugger;
		$(".ajaxloader").show();
		var BookingNumber = $("#BookingNumber").val();
		var PositionId = $("#Position_Id").val();
		var IsPlaceholder = $("#Placeholder").val();
		var PositionStatus = $("#PositionStatusFull").val();
		var SystemCompanyId = $("#SystemCompany_Id").val();

		$("#ChangeSupplier-popup .popup-in").html('');

		if (IsPlaceholder == 'True') {
			$.ajax({
				type: "GET",
				url: "/Operations/ChangeProduct",
				data: { BookingNumber: BookingNumber, PositionId: PositionId },
				contentType: "application/json; charset=utf-8",
				dataType: "html",
				global: false,
				success: function (response) {
					debugger;
					$("#ChangeSupplier-popup .popup-in").html(response);
					$("#ChangeSupplier-popup").show();
					ShowPopup("#ChangeSupplier-popup", true, true);
					$(".ajaxloader").hide();
				},
				error: function (response) {
					alert(response.statusText);
					$(".ajaxloader").hide();
				}
			});
		}
		else {
			$.ajax({
				type: "GET",
				url: "/Operations/ChangeSupplier",
				data: { BookingNumber: BookingNumber, PositionId: PositionId, PositionStatus: PositionStatus, SystemCompanyId: SystemCompanyId },
				contentType: "application/json; charset=utf-8",
				dataType: "html",
				success: function (response) {
					debugger;
					$("#ChangeSupplier-popup .popup-in").html(response);
					$("#ChangeSupplier-popup").show();
					ShowPopup("#ChangeSupplier-popup", true, true);
					$(".ajaxloader").hide();
				},
				error: function (response) {
					alert(response.statusText);
				}
			});
		}	
}

function CancelPosition() {
	 
	if ($('#frmCancelPosition').valid()) {
		 
		var BookingNumber = $('#BookingNumber').val();
		var PositionId = $('#Position_Id').val();	

		positionFormData['BookingNumber'] = BookingNumber;
		positionFormData['PositionId'] = PositionId;

		positionFormData['Action'] = "Cancel";
		positionFormData['Module'] = "Position";
		positionFormData['ModuleParent'] = "Booking";
		positionFormData['Status'] = "C";

		$.ajax({
			type: "POST",
			url: "/Operations/SetBookingByWorkflow",
			data: { model: positionFormData },
			dataType: "json",
			success: function (response1) {
				$("#CancelPosition-popup").show();
				ShowPopup("#CancelPosition-popup", true, true);
				if (response1.status.toLowerCase() !== "success") {
					$("#SuccessAlert").hide();
					$("#ErrorMessage").html("Error!" + response1.msg);
					$("#ErrorAlert").show();
				}
				else {
					$("#ErrorAlert").hide();
					$("#SuccessMessage").html('Success! Position Cancelled Successfully.');
					$("#SuccessAlert").show();
				}
			},
			error: function (response) {
				$("#SuccessAlert").hide();
				$("#ErrorMessage").html(response.msg);
				$("#ErrorAlert").show();
			}
		});
	}
	else {
		alert("Please fill mandatory fields!!!");
	}
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