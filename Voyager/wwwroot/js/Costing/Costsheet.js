
$(document).ready(function () {


    //$("#DepartureDate").change(function () {
    //    GetConsolidatedSummary();
    //});
    
    //$("#ddlCostSheet").change(function () {
    //    var QRFId = $("#QRFId").val();
    //    var selectedVal = $("#ddlCostSheet option:selected").val();
    //    if (selectedVal == 1)
    //        $("#divConsolidatedSummary").load("/Costsheet/GetConsolidatedSummary", { QRFId: QRFId });
    //    else if (selectedVal == 2)
    //        $("#divConsolidatedSummary").load("/Costsheet/GetDetailedInfo", { QRFId: QRFId });
    //    else
    //        return false;
    //});

    //$(".showVersion").click(function () {
         
    //    var QRFId = $("#QRFId").val();
    //    $.ajax({
    //        type: "GET",
    //        url: "/Costsheet/GetCostsheetVersions",
    //        data: { QRFId: QRFId },
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "html",
    //        success: function (response) {
                 
    //            $("#VersionList-popup .popup-in").html(response);
    //            $("#VersionList-popup").show();
    //        },
    //        error: function (response) {
    //            alert(response.statusText);
    //        }
    //    });
    //});
});

//function changeCurrentVersion() {
//     
//    $(".ajaxloader").show();
//    var QRFId = $("#QRFId").val();
//    var QRFPriceId = $("#SelectedVersion:checked").val();
    
//    $.ajax({
//        type: "POST",
//        url: "/Costsheet/UpdateCostsheetVersion",
//        data: { QRFId: QRFId, QRFPriceId: QRFPriceId },
//        global: false,
//        //contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (response) {
//            $('#VersionList-popup .close-popup').click();
//            window.location.href = "/CostingOfficer/Costsheet?QRFId=" + QRFId;
//            //if (response.status == 'Success') {
//            //     
//            //    window.location.href = "/CostingOfficer/Costsheet?QRFId=" + QRFId;              
//            //    //GetConsolidatedSummary(response.errorMessage, "");
//            //}
//            //else {
//            //     
//            //    GetConsolidatedSummary("", response.errorMessage);
//            //}
//        },
//        failure: function (response) {
//            $(".ajaxloader").hide();
//            alert(response.responseText);
//        },
//        error: function (response) {
//            $(".ajaxloader").hide();
//            alert(response.responseText);
//        }
//    });
//}