$(document).ready(function () {

	$('.close-popup').click(function (e) {
		e.preventDefault();
		$.magnificPopup.close()
	});

	$('#docItinerary').on('click', '.btnGenerate', function () {
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide();


		var bookingId = $('#bookingId').text();
		var bookingno = $('#bookingNumber').text();
		var type = "Itinerary";
		$.ajax({
			type: "GET",
			url: "/Booking/GenerateBookingDocument",
			data: { BookingNo: bookingno, BookingId: bookingId, type: type },
			dataType: "json",
			success: function (response) {
				if (response.message == "error") {
					$("#SuccessAlert").hide();
					$("#ErrorAlert").html(response.responseText);
					$("#ErrorAlert").show();
				}
				else {

					$("#ErrorAlert").hide();
					$("#itineraryDocPath").text(response.path);
					$("#itineraryDocDate").text(response.date);
					$("#itineraryDocId").text(response.documentId);
					$("#SuccessAlert").html(response.responseText);
					$("#SuccessAlert").show();
				}
			},
			failure: function (response) {
				alert(response.responseText);
			},
			error: function (response) {
				alert(response.responseText);
			}
		});
	});

	$('#docItinerary').on('click', '.btnGenerateFinal', function () {
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide();

		var bookingId = $('#bookingId').text();
		var bookingno = $('#bookingNumber').text();
		var type = "FinalItinerary";
		$.ajax({
			type: "GET",
			url: "/Booking/GenerateBookingDocument",
			data: { BookingNo: bookingno, BookingId: bookingId, type: type },
			dataType: "json",
			success: function (response) {
				if (response.message == "error") {
					$("#SuccessAlert").hide();
					$("#ErrorAlert").html(response.responseText);
					$("#ErrorAlert").show();
				}
				else {
					$("#ErrorAlert").hide();
					$("#finalItineraryDocPath").text(response.path);
					$("#finalItineraryDocDate").text(response.date);
					$("#finalItineraryDocId").text(response.documentId);
					$("#SuccessAlert").html(response.responseText);
					$("#SuccessAlert").show();
				}
			},
			failure: function (response) {
				alert(response.responseText);
			},
			error: function (response) {
				alert(response.responseText);
			}
		});
	});

	$('#docVoucher').on('click', '.btnGenerate', function () {
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide();

		var bookingId = $('#bookingId').text();
		var bookingno = $('#bookingNumber').text();
		var type = "Vouchers";
		$.ajax({
			type: "GET",
			url: "/Booking/GenerateBookingDocument",
			data: { BookingNo: bookingno, BookingId: bookingId, type: type },
			dataType: "json",
			success: function (response) {
				if (response.message == "error") {
					$("#SuccessAlert").hide();
					$("#ErrorAlert").html(response.responseText);
					$("#ErrorAlert").show();
				}
				else {
					$("#ErrorAlert").hide();
					$("#voucherfullpath").text(response.fullPath);
					$("#voucherDocPath").text(response.path);
					$("#voucherDocDate").text(response.date);
					$("#SuccessAlert").html(response.responseText);
					$("#SuccessAlert").show();

					var fullpath = $("#voucherfullpath").text();
					//window.location.href = "/Booking/DownloadBookingDocument?generateddocId=null&bookingId=" + bookingId + "&type=Vouchers&fullDocPath=" + fullpath;

					try {						
						$.ajax({
							type: "GET",
							url: "/Booking/DownloadBookingDocument",
							data: { generateddocId: null, bookingId: bookingId, type: "Vouchers", fullDocPath: fullpath },
							dataType: "json",
							success: function (response) {
								if (response.message == "error") {
									$("#SuccessAlert").hide();
									$("#ErrorAlert").html(response.responseText);
									$("#ErrorAlert").show();
								}
								else {
									$("#ErrorAlert").hide();
								}
							},
							failure: function (response) {
								console.log(response.responseText);
							},
							error: function (response) {
								console.log(response.responseText);
							}
						});

					} catch (e) {
						$("#ErrorAlert").text("Error while downloading document");
						$("#ErrorAlert").show();
						return false;
					}

				}
			},
			failure: function (response) {
				alert(response.responseText);
			},
			error: function (response) {
				alert(response.responseText);
			}
		});
	});

	$('#docInvoice').on('click', '.btnGenerate', function () {
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide();

		var bookingId = $('#bookingId').text();
		var bookingno = $('#bookingNumber').text();
		var type = "Invoice";
		$.ajax({
			type: "GET",
			url: "/Booking/GenerateBookingDocument",
			data: { BookingNo: bookingno, BookingId: bookingId, type: type },
			dataType: "json",
			success: function (response) {
				if (response.message == "error") {
					$("#SuccessAlert").hide();
					$("#ErrorAlert").html(response.responseText);
					$("#ErrorAlert").show();
				}
				else {
					$("#ErrorAlert").hide();
					$("#invoicefullpath").text(response.fullPath);
					$("#invoiceDocPath").text(response.path);
					$("#invoiceDocDate").text(response.date);
					$("#SuccessAlert").html(response.responseText);
					$("#SuccessAlert").show();

					var fullpath = $("#invoicefullpath").text();
					//window.location.href = "/Booking/DownloadBookingDocument?generateddocId=null&bookingId=" + bookingId + "&type=Invoice&fullDocPath=" + fullpath;

					try {
						$.ajax({
							type: "GET",
							url: "/Booking/DownloadBookingDocument",
							data: { generateddocId: null, bookingId: bookingId, type: "Invoice", fullDocPath: fullpath },
							dataType: "json",
							success: function (response) {
								if (response.message == "error") {
									$("#SuccessAlert").hide();
									$("#ErrorAlert").html(response.responseText);
									$("#ErrorAlert").show();
								}
								else {
									$("#ErrorAlert").hide();
								}
							},
							failure: function (response) {
								console.log(response.responseText);
							},
							error: function (response) {
								console.log(response.responseText);
							}
						});

					} catch (e) {
						$("#ErrorAlert").text("Error while downloading document");
						$("#ErrorAlert").show();
						return false;
					}
				}
			},
			failure: function (response) {
				alert(response.responseText);
			},
			error: function (response) {
				alert(response.responseText);
			}
		});
	});

	$('#docItinerary').on('click', '.btnDownload', function () {
		 
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide(); 
		var id = $("#docItinerary #itineraryDocId").text() || $(this).parents('tr').find('#docId').val();
		if (id == "" || id == null || id == "undefined") {
			$("#ErrorAlert").text("Please generate document first");
			$("#ErrorAlert").show();
			return false;
		}
		else {
			try {
				//var lnk = "/Booking/DownloadBookingDocument?generateddocId=" + id + "&bookingId=null&type=Itinerary";
				//var result = $(this).attr("href", lnk);		

				$.ajax({
					type: "GET",
					url: "/Booking/DownloadBookingDocument",
					data: { generateddocId: id, bookingId: null, type: "Itinerary" },
					dataType: "json",
					success: function (response) {
						 
						if (response.message == "error") {
							$("#SuccessAlert").hide();
							$("#ErrorAlert").html(response.responseText);
							$("#ErrorAlert").show();
						}
						else {
							$("#ErrorAlert").hide();
						}
					},
					failure: function (response) {
						console.log(response.responseText);
					},
					error: function (response) {
						console.log(response.responseText);
					}
				});

			} catch (e) {
				$("#ErrorAlert").text("Error while downloading document");
				$("#ErrorAlert").show();
				return false;
			}
		}

	});

	$('#docItinerary').on('click', '.btnDownloadFinal', function () {
		$("#SuccessAlert").hide();
		$("#ErrorAlert").hide();

		var id = $("#docItinerary #finalItineraryDocId").text() || $(this).parents('tr').find('#finaldocId').val();

		if (id == "" || id == null || id == "undefined") {
			$("#ErrorAlert").text("Please generate document first");
			$("#ErrorAlert").show();
			return false;
		}
		else {
			try {
				//var result = $(this).attr("href", "/Booking/DownloadBookingDocument?generateddocId=" + id + "&bookingId=null&type=FinalItinerary");		

				$.ajax({
					type: "GET",
					url: "/Booking/DownloadBookingDocument",
					data: { generateddocId: id, bookingId: null, type: "FinalItinerary" },
					dataType: "json",
					success: function (response) {
						if (response.message == "error") {
							$("#SuccessAlert").hide();
							$("#ErrorAlert").html(response.responseText);
							$("#ErrorAlert").show();
						}
						else {
							$("#ErrorAlert").hide();
						}
					},
					failure: function (response) {
						console.log(response.responseText);
					},
					error: function (response) {
						console.log(response.responseText);
					}
				});

			} catch (e) {
				$("#ErrorAlert").text("Error while downloading document");
				$("#ErrorAlert").show();
				return false;
			}
		}
	});
});