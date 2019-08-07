$(document).ready(function () {
    $(".SaveSuggestedItinerary").click(function () { 
        var model = $("#frmProposalSuggestedItinerary").serialize();
        $.ajax({
            type: "POST",
            url: "/Proposal/SetSuggestedItinerary",
            data: model,
            datatype: "html",
            success: function (response) {
                $("#InclusionsExclusions").click();
            },
            error: function (response) {
                alert(response);               
            }
        });
    });

    $(".AddNew").click(function () {         
        var QRFId = $("#QRFId").val();
        var ItineraryID = $("#ItineraryID").val();        
        $.ajax({
            type: "POST",
            url: "/Proposal/AddNewItineraryDetails",
            data: { QRFId: QRFId, ItineraryID: ItineraryID, type:"proposal" },
            success: function (response) {
                $("#addNewDetails-popup").html(response);
            },
            error: function (response) {
                alert(response);
            }
        });
    });
});
