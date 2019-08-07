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
            url: "/Itinerary/EnableDisableItineraryDetails",
            data: { QRFId: QRFId, PositionId: PositionId, IsDeleted: isDeleted, ItineraryId: ItineraryId, ItineraryDaysId: ItineraryDaysId },
            dataType: "json",
            global: false,
            success: function (response) {
                GetItineraryData();
            
                //$("#tblItinerary").load('/Itinerary/GetItinerary/', { QRFId: QRFId });
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

    //Edit position functionality 
    $(".editPosition").click(function () {
        var currentRow = $(this).parents('tr');
        var QRFId = $('#QRFId').val();
        var PositionId = currentRow.find("#PositionId").val();
        var ProductType = currentRow.find("#ProductType").val();
        var Type = currentRow.find("#Type").val();
        var ItineraryID = $(this).parents('tr').find('.clsItineraryId').val();
        var ItineraryDaysId = $(this).parents('tr').find('.clsItineraryDaysId').val();
        GetPositionPartView(QRFId, PositionId, ProductType, Type, ItineraryID, ItineraryDaysId);
    });

    function GetPositionPartView(QRFId, PositionId, ProductType, Type, ItineraryID, ItineraryDaysId) {
        $.ajax({
            type: "GET",
            url: "/Itinerary/EditQRFPosition",
            data: { QRFId: QRFId, PositionId: PositionId, ProductType: ProductType, Type: Type, ItineraryID: ItineraryID, ItineraryDaysId: ItineraryDaysId },
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                 
                $(".displayPosition").html(response.replace("action-popup ", ""));
                $("#action-popup").show();
                $('<input>').attr({ type: 'hidden', id: 'IsClone', name: 'MenuViewModel.IsClone', value: true }).appendTo('form');
                $('<input>').attr({ type: 'hidden', id: 'IsClone', name: 'MenuViewModel.IsClone', value: true }).appendTo('.div-pop-sec');
                
                if ($("#EnquiryPipeline").val() != "Amendment Pipeline") {
                    $('.displayPosition input[type="text"],.displayPosition select,.displayPosition textarea,.displayPosition button').prop('disabled', 'disabled');
                    $(".displayPosition button,:radio,:checkbox").prop('disabled', 'disabled');
                    $('.displayPosition input[type="button"]').hide();
                    $('.cloneRoom,.removeRoom,.cloneSupplement,#aContinue,.accomdefmealplan').css('display', 'none');
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }

    // Add/Edit new itinerary element functionality
    $(".addEditNewItinerary").click(function () {
        var QRFId = $("#QRFId").val();
        var ItineraryID = $(this).parents('tr').find('.clsItineraryId').val();
        var ItineraryDaysId = $(this).parents('tr').find('.clsItineraryDaysId').val();
		var PositionId = $(this).parents('tr').find('.clsPositionId').val();
		var Action = $(this).parents('tr').find('.addEditNewItinerary').data('val');
        $.ajax({
            type: "POST",
            url: "/Proposal/AddNewItineraryDetails",
			data: { QRFId: QRFId, PositionId: PositionId, ItineraryID: ItineraryID, ItineraryDaysId: ItineraryDaysId, action: Action, type: "itinerary" },
            success: function (response) {
                $("#addNewDetails-popup").html(response);
                $("#addNewDetails-popup #Day").addClass("disabled", true);
            },
            error: function (response) {
                alert(response);
            }
        });
    });

    //Dropdown related functionality
    $("#Days").change(function () {
        GetItineraryData();
    });

    $("#Services").change(function () {
        GetItineraryData();
    });


}); 


function createCostsheet() {
    $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    $.ajax({
        type: "POST",
        url: "/Costsheet/SetCostsheetNewVersion",
        data: { QRFId: QRFId, Pipeline: "Amendment" },
        global: false,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.status == 'Success') {
                GetItineraryData(response.errorMessage,'');
                //GetGuesstimateData(response.errorMessage, "");
            }
            else {
                GetItineraryData('', response.errorMessage);
                //GetGuesstimateData("", response.errorMessage);
            }
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
