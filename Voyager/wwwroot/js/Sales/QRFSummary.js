$(document).ready(function () {  
     //Enable and Disable position functionality
    $(".enableDisablePosition").click(function () {
         
       $(".ajaxloader").show();
        var isDeleted = $(this).parents('tr').find('.clsIsDeleted').val();
        var QRFId = $("#QRFId").val();
        var PositionId = $(this).parents('tr').find('.clsPositionId').val();
        var ItineraryId = $(this).parents('tr').find('.clsItineraryId').val();
        var ItineraryDaysId = $(this).closest('tr').find('.clsItineraryDaysId').val();
        $.ajax({
            type: "POST",
            url: "/QRFSummary/EnableDisablePosition",
            data: { QRFId: QRFId, PositionId: PositionId, ItineraryId: ItineraryId, ItineraryDaysId: ItineraryDaysId, IsDeleted: isDeleted },
			dataType: "json",
			global: false,
            success: function (response) {                 
                GetQRFSummary();
                //$(".ajaxloader").hide();
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
    });

    $(".closeWindow").click(function () {
        GetQRFSummary();
    });
    
    //Edit position functionality 
    $(".editPosition").click(function () {        
        var currentRow = $(this).parents('tr');
        var QRFId = $('#QRFId').val();
        var PositionId = currentRow.find("#PositionId").val();    
        var ProductType = currentRow.find("#ProductType").val();   
        GetPositionPartView(QRFId, PositionId, ProductType);
    });

    function GetPositionPartView(QRFId, PositionId, ProductType) {
        $.ajax({
            type: "GET",
            url: "/QRFSummary/EditPosition",
            data: { QRFId: QRFId, PositionId: PositionId, ProductType: ProductType },
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $(".displayPosition").html(response.replace("action-popup ", ""));
                $("#action-popup").show();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    //Remarks related functionality  
    $(".remarks").click(function () { 
        var QRFId = $('#QRFId').val();
        var PositionId = $(this).parents('tr').find('.clsPositionId').val();
        var ItineraryId = $(this).parents('tr').find('.clsItineraryId').val();
        var ItineraryDaysId = $(this).closest('tr').find('.clsItineraryDaysId').val();
        GetRemarksPartView(QRFId, PositionId, ItineraryId, ItineraryDaysId);
    });

    function GetRemarksPartView(QRFId, PositionId, ItineraryId, ItineraryDaysId) { 
        $.ajax({
            type: "GET",
            url: "/QRFSummary/GetRemarksForPosition",
            data: { QRFId: QRFId, PositionId: PositionId, ItineraryId: ItineraryId, ItineraryDaysId: ItineraryDaysId },
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

    //Dropdown related functionality
	$("#Days").change(function () {
		 
        GetDefaultItinerary();           
    });

    $("#Services").change(function () {
        GetDefaultItinerary();        
    }); 

	$(".addEditNewItinerary").click(function () {
		 
        var QRFId = $("#QRFId").val();
        var ItineraryID = $(this).parents('tr').find('.clsItineraryId').val();
        var ItineraryDaysId = $(this).parents('tr').find('.clsItineraryDaysId').val();
		var PositionId = $(this).parents('tr').find('.clsPositionId').val();
		var Action = $(this).parents('tr').find('.addEditNewItinerary').data('val');

        $.ajax({
            type: "POST",
            url: "/Proposal/AddNewItineraryDetails",
			data: { QRFId: QRFId, PositionId: PositionId, ItineraryID: ItineraryID, ItineraryDaysId: ItineraryDaysId, action: Action, type: "summary" },
            success: function (response) {
                $("#addNewDetails-popup").html(response);
                $("#addNewDetails-popup #Day").addClass("disabled", true);
            },
            error: function (response) {
                alert(response);
            }
        });
	});

}); 

