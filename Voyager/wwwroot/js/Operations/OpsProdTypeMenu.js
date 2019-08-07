$(document).ready(function () {
    loadProductType();
});

function loadProductType() {
    var SelectedMenu = $("#MenuName").val();
    if (SelectedMenu === "Itinerary") {
        $("#aItinerary").click();
    }
    else { //this block will call when From BookingSummaryPage->Fix button click
       // alert("hi");
        var PositionId = $("#MenuName").attr("data-posid"); 
        $("#ulOpsProdType").find('li').each(function () {
            var anchorThis = $(this).find("a");
            if (anchorThis.text().trim().toLocaleLowerCase() === SelectedMenu.toLocaleLowerCase()) {
                SetProdTypeActive(anchorThis, PositionId);
                return;
            }
        });
    } 
}

$("#aItinerary").click(function () {
    var ddlDay = $("#ddlDays").val();
    var ddlProdType = $("#ddlServiceType").val();
    var ddlStatus = $("#ddlBookingStaus").val();
    var bookingNumber = $("#BookingNumber").val();
    $("#divOpsProdTypeMain").find(".sqr-tab-data").html('').hide();
    $("#MenuName").val($("#liItinerary").attr("liprodtype"));
    $('#divOpsItinerary').load('/Operations/ViewServiceItinerary',
        {
            BookingNumber: bookingNumber,
            BookingRemark: "noremarks",
            DayName: ddlDay,
            ProductType: ddlProdType,
            Status: ddlStatus
        });
    $("#ulOpsProdType").find("li").removeClass("active");
    $("#liItinerary").addClass("active");
    $("#divOpsItinerary").show();
});

$(".aProdType").click(function () {
    var anchorThis = $(this);
    SetProdTypeActive(anchorThis);
    $("#MenuName").attr("data-posid", "");
});

$(document).on("click", ".btnOpsRedirect", function (e) {
    var thisparam = $(this);
    var prodType = thisparam.attr("ProdType");
    var posId = thisparam.attr("PosId");

    if (prodType != "" && prodType != undefined && posId != "" && posId != undefined) {
        var UIProdType = GetProductType(prodType);
        prodType = prodType.toLocaleLowerCase();
        $("#ulOpsProdType").find('li').each(function () {
            var anchorThis = $(this).find("a");
            if (anchorThis.text().trim().toLocaleLowerCase() == UIProdType.toLocaleLowerCase()) {
                SetProdTypeActive(anchorThis, posId);
                return;
            }
        });
    }
});

function SetProdTypeActiveOld(anchorThis, posId = "") { 
    var bookingNumber = $("#BookingNumber").val();
    $("#divOpsProdTypeMain").find(".sqr-tab-data").html('').hide();
    $("#MenuName").val(anchorThis.parent("li").attr("liprodtype"));
    var prodType = anchorThis.attr("prodtype");
    $('#divOpsProdTypeInfo').load('/Operations/ViewProductTypeDetails', { ProductType: prodType, BookingNumber: bookingNumber, PositionId: posId });
    $("#ulOpsProdType").find(".active").removeClass("active");
    $("#" + anchorThis.parent("li").attr("id")).addClass("active");
    $("#divOpsProdTypeInfo").show();
}

function SetProdTypeActive(anchorThis, posId = "") { 
    var bookingNumber = $("#BookingNumber").val();
    $("#divOpsProdTypeMain").find(".sqr-tab-data").html('').hide();
    $("#MenuName").val(anchorThis.parent("li").attr("liprodtype"));
    var prodType = anchorThis.attr("prodtype"); 
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/Operations/ViewProductTypeDetails", // Controller/View
        data: { ProductType: prodType, BookingNumber: bookingNumber, PositionId: posId },
        success: function (response) {
            $("#divOpsProdTypeInfo").html(response);
            $("#ulOpsProdType").find(".active").removeClass("active");
            $("#" + anchorThis.parent("li").attr("id")).addClass("active");
            $("#divOpsProdTypeInfo").show();
            $(".ajaxloader").hide();
        },
        error: function (jqXHR, exception) {
            var msg = getAjaxErrorStatusDescription(jqXHR, exception, errorThrown); 
            $(".ajaxloader").hide();
            alert(msg);            
        }
    });
}

function GetProductType(prodType) {
    var UIProdType = "";
    switch (prodType) {
        case "HOTEL":
            UIProdType = "Accommodation";
            break;
        case "MEAL":
            UIProdType = "Meals";
            break;
        case "ATTRACTIONS":
        case "SIGHTSEEING - CITYTOUR":
            UIProdType = "Activities";
            break;
        case "LDC":
        case "COACH":
            UIProdType = "Bus";
            break;
        case "OVERNIGHT FERRY":
            UIProdType = "Cruise";
            break;
        case "TRAIN":
            UIProdType = "Rail";
            break;
        case "PRIVATE TRANSFER":
        case "SCHEDULED TRANSFER":
        case "FERRY TRANSFER":
        case "FERRY PASSENGER":
            UIProdType = "Transfers";
            break;
        case "DOMESTIC FLIGHT":
            UIProdType = "Flights";
            break;
        case "GUIDE":
        case "ASSISTANT":
            UIProdType = "Local Guide";
            break;
        case "VISA":
        case "INSURANCE":
        case "OTHER":
        case "FEE":
            UIProdType = "Others";
            break;
        default:
            UIProdType = "";
            break;
    }

    return UIProdType;
}