$(document).ready(function () {
    var QRFId = $("#QRFId").val();
    GetQRFSummary();

    $("#btnSubmitQuoteSave").click(function () {         
        var data = {
            QRFId: $("#QRFId").val(),
            QuoteRemarks: $("#quoteRemarks").val(),
            Officer: $("#Officer option:selected").text()
        }
        $.ajax({
            type: "POST",
            url: "/QRFSummary/SubmitQuote",
            data: data,
            dataType: "json",
            success: function (response) {
				 
				if (response.status == "error")
				{
					alert(response.responseText);
				}
				else {
					$("#itinerary-quote-submitted").show();
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
    });

function GetQRFSummary() {
     
   // $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    var selectedDay = $("#Days option:selected").text();
    var selectedServiceType = $("#Services option:selected").text();
    $.ajax({
        type: "GET",
        url: "/QRFSummary/GetQRFSummaryData",
        data: { QRFId: QRFId, filterByDay: selectedDay, filterByServiceType: selectedServiceType },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $("#divQRFSummary").html(response);
            $(".ajaxloader").hide();
        },
        failure: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        },
        error: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        }
    });
}

function GetDefaultItinerary() {
     
    $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    var selectedDay = $("#Days option:selected").text();
    var selectedServiceType = $("#Services option:selected").text();
    $.ajax({
        type: "GET",
        url: "/QRFSummary/GetItineraryData",
        data: { QRFId: QRFId, filterByDay: selectedDay, filterByServiceType: selectedServiceType },
        contentType: "application/json; charset=utf-8",
		dataType: "html",
        success: function (response) {
            $("#divQRFSummary").html(response);
            $(".ajaxloader").hide();
        },
        failure: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        },
        error: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        }
    });
}

$('.close-popup').click(function (e) {
   // e.preventDefault();
    $.magnificPopup.close()
});

$('.closeOkPopup').click(function (e) {
   // e.preventDefault();
    window.location.href = "/Quote/Quote";
});