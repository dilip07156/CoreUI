$(document).ready(function () {
	var AgentName = $("#AgentIdUI").val();
	var AgentCode = $("#AgentCode").val();
	var CNKReferenceNo = $("#CNKReferenceNo").val();
	var AgentTour = $("#AgentTour").val();
	var BookingStatus = $("#BookingStatus").find("option:selected").val();
	var DateType = $("#DateType").find("option:selected").val();
	var From = $("#From").val();
	var To = $("#To").val();

	var oTable = $('#tblBooking').dataTable({
		processing: true,
		serverSide: true,
		info: true,
		ajax: {
			url: "/Booking/LoadData",
			type: "POST",
			data: function (d) {
				d.AgentName = AgentName;
				d.AgentCode = AgentCode;
				d.CNKReferenceNo = CNKReferenceNo;
				d.AgentTour = AgentTour;
				d.BookingStatus = BookingStatus;
				d.DateType = DateType;
				d.From = From;
				d.To = To;
			}
		},
		columns: [
			{ "data": "bookingReference", "orderable": true },
			{ "data": "agentCode", "orderable": true },
			{ "data": "agentontact", "orderable": true },
			{ "data": "bookingName", "orderable": true },
			{ "data": "startDate", "orderable": true, "sType": "date-uk" },
			{ "data": "endDate", "orderable": true },
			{ "data": "duration", "orderable": true },
			{ "data": "status", "orderable": true },
			{
				"data": function (data, type, row, meta) {
                    return '<a href="#action-popup" id="btnView" class="popup-inline btn btn-blue smallbutton btnView" onClick="ShowViewBooking(' + "'" + data.bookingReference + "'" +');">View</a>&nbsp;<a href="#documentdownload-popup" id="btnItinerary" class="popup-inline btn btn-blue smallbutton btnItinerary" onClick="ShowItinerary(' + "'" + data.bookingId + "'" + ',\'' + data.bookingReference + '\');">Itinerary</a>&nbsp;<a href="#documentdownload-popup" id="btnVouchers" class="popup-inline btn btn-blue smallbutton btnVouchers" onClick="ShowVouchers(' + "'" + data.bookingId + "'" + ',\'' + data.bookingReference + '\');">Vouchers</a>&nbsp;<a href="#documentdownload-popup" id="btnInvoice" class="popup-inline btn btn-blue smallbutton btnInvoice" onClick="ShowInvoice(' + "'" + data.bookingId + "'" + ',\'' + data.bookingReference + '\');">Invoice</a>';                     
				},
				"orderable": false
			},
		],
		"order": [[0, "asc"]],
		"columnDefs": [{
			"defaultContent": "",
			"targets": "_all",
		},
		{
			"targets": [4,5,6,7,8],
			"className": "text-center"
		}],
		"oLanguage": {
			sLengthMenu: "Show entries _MENU_",
		},
		rowCallback: function (row, data) {
			 
			if (data["status"] == "I" || data["status"] == "K" || data["status"] == "B") {
				$(row).addClass('alert-success');
			}
			if (data["status"] == "M" || data["status"] == "O") {
				$(row).addClass('alert-warning');
			}
			if (data["status"] == "J" || data["status"] == "C") {
				$(row).addClass('tr-strikthrough');
			}
			if (data["status"] == "E" || data["status"] == "P" || data["status"] == "Q") {
				$(row).addClass('alert-danger');
			}
		},
		"dom": "<'row'<'col-sm-3'l><'col-sm-1'f><'col-sm-8'p>>" + "<'row'<'col-sm-12'tr>>" + "<'row'<'col-sm-5 margin-top-bottom-20'i><'col-sm-7'p>>",
	});

	$('#tblBooking_filter').css('display', 'none');
});

function ShowViewBooking(bookingReference) {
	 
	$.ajax({
		type: "GET",
		url: "/Booking/ViewBooking",
		data: { BookingNo: bookingReference },
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$(".displayBookingPosition").html(response.replace("action-popup ", ""));
			$("#action-popup").show();
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
};

function ShowItinerary(bookingId, bookingReference) {
	 
	var type = "Itinerary";
	$.ajax({
		type: "GET",
		url: "/Booking/DocumentDownload",
		data: { Type: type, BookingId: bookingId, BookingNo: bookingReference },
		async: false,
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#documentdownload-popup").html(response);
			$("#bookingNumber").text(bookingReference);
			$("#bookingId").text(bookingId);
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
};

function ShowVouchers(bookingId, bookingReference) {
	 
	var type = "Vouchers";
	$.ajax({
		type: "GET",
		url: "/Booking/DocumentDownload",
		data: { Type: type, BookingId: bookingId, BookingNo: bookingReference },
		async: false,
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#documentdownload-popup").html(response);
			$("#bookingNumber").text(bookingReference);
			$("#bookingId").text(bookingId);
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
};

function ShowInvoice(bookingId, bookingReference) {
	 
	var type = "Invoice";
	$.ajax({
		type: "GET",
		url: "/Booking/DocumentDownload",
		data: { Type: type, BookingId: bookingId, BookingNo: bookingReference },
		async: false,
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#documentdownload-popup").html(response);
			$("#bookingNumber").text(bookingReference);
			$("#bookingId").text(bookingId);
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
};