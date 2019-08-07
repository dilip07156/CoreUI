var positionFormData = {};

$(document).ready(function () {

	$('input[type="checkbox"]', '.chkHotelRow', '.btnSelectAll').prop('checked', true);
		
	var form = $("#frmCancelBooking");
	form.removeData('validator');
	form.removeData('unobtrusiveValidation');
	$.validator.unobtrusive.parse(form);
});

$('body').unbind().on('click', '#btnSelectAll', function () {
	if ($(this).hasClass('allChecked')) {
		$('input[type="checkbox"]', '.chkHotelRow').prop('checked', false);
	} else {
		$('input[type="checkbox"]', '.chkHotelRow').prop('checked', true);
	}
	$(this).toggleClass('allChecked');
})


$(document).find("#cancelBookingSection select, #cancelBookingSection textarea, #cancelBookingSection input").on('change input', function (e) {

	positionFormData = addChangedValueToList(this, positionFormData);
});

function updateCancelPositionDetails() {
	if ($('input[name*="IsSelected"]:checked').length > 0) {

		positionFormData = addChangedValueToList($("#TableCancelBooking").find('tr:first'), positionFormData);

		if ($('#frmCancelBooking').valid()) {
			var BookingNumber = $('#BookingNumber').val();
			var PositionId = $('#Position_Id').val();

			positionFormData['BookingNumber'] = BookingNumber;
			positionFormData['PositionId'] = PositionId;

			positionFormData['Action'] = "Cancel";
			positionFormData['Module'] = "Position";
			positionFormData['ModuleParent'] = "Booking";
			positionFormData['Status'] = "C";


			var val = positionFormData['TableCancelBooking'];

            if (val == "" || val == null || val == undefined || val == true || val == false) {
				$("#SuccessAlert").hide();
				$("#ErrorMessage").html('Error! Please select atleast one supplier.');
				$("#ErrorAlert").show();
				return false;
			}

			//if ($.inArray('IsSelected:true', val) > -1) {
			//	alert(val);

			//}

			$.ajax({
				type: "POST",
				url: "/Operations/SetBookingByWorkflow",
				data: { model: positionFormData },
				dataType: "json",
				success: function (response) {
					console.log(response);
					if (response.status.toLowerCase() !== "success") {
						$("#SuccessAlert").hide();
						$("#ErrorMessage").html("Error!" + response.msg);
						$("#ErrorAlert").show();
					}
					else {
						GetCancelBookingSupplierList();
						$("#ErrorAlert").hide();
						$("#SuccessMessage").html('Success! Booking Cancelled Successfully.');
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
	else {
		alert("Please select atleast one Supplier");
	}
}

function GetCancelBookingSupplierList() {

	var bn = $("#BookingNumber").val();
	$.ajax({
		type: "GET",
		url: "/Operations/GetCancelBookingSupplierList",
		data: { BookingNumber: bn },
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$('#divSupplierList').html(response);
		},
		error: function (response) {
			alert(response.statusText);
		}
	});
}