$(document).ready(function () {

    $("#MainTour").show();

    $("#DepartureDate").change(function () {
        GetPricingSummaryData();
        GetItineraryData();
    });

    $("#PaxSlab").change(function () {
        GetPricingSummaryData();
        GetItineraryData();
    });

    $("#ddlSupplement").change(function () {
        var selProd = this.value.replace(' ', '-');
        $('.SupplementTable > thead,.SupplementTable > tbody').hide();
        $('.' + selProd).show();
    });

    $("#ddlOptional").change(function () {
        var selProd = this.value.replace(' ', '-');
        $('.OptionalTable > thead,.OptionalTable > tbody').hide();
        $('.' + selProd).show();
    });

    $("#HeaderMainTour").click(function () {
        $("#MainTour").show();
        $("#Supplement").hide();
        $("#Optional").hide();
        $("#HeadersTab").parent().find("li").removeClass("active");
        $("#HeaderMainTour").addClass("active");
    });

    $("#HeaderSupplement").click(function () {
        $("#MainTour").hide();
        $("#Optional").hide();
        $("#Supplement").show();
        $("#HeadersTab").parent().find("li").removeClass("active");
        $("#HeaderSupplement").addClass("active");
    });

    $("#HeaderOptional").click(function () {
        $("#MainTour").hide();
        $("#Supplement").hide();
        $("#Optional").show();
        $("#HeadersTab").parent().find("li").removeClass("active");
        $("#HeaderOptional").addClass("active");
    });

});