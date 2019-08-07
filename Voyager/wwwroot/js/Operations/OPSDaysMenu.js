$(document).ready(function () {
    //bydefault li active event will trigger
    $('#ulOpsBookingDaysMenu').find("li.active").click();

    SetNextPrevToDaysMenu();

    //on click of DayName
    $("#ulOpsBookingDaysMenu li").click(function (e) {
        e.preventDefault();
        $("#ulOpsBookingDaysMenu").find("li").removeClass("active");
        $(this).addClass("active");

        //call the Position View service for binding partial views as ProdSRP,FOC,Romms Rate,etc
        var dayname = $(this).find("#spDayName").text();
        var bookingNumber = $("#BookingNumber").val();
        var prodType = $("#ulOpsProdType li.active").find(".aProdType").attr("prodtype");
        $('#divOpsDayInfo').empty();
        $('#divOpsDayInfo').load('/Operations/ViewProdTypePosition', { ProductType: prodType, BookingNumber: bookingNumber, DayName: dayname });
    });

    //PREV arrow click event
    $(".scroller-left").click(
        function () {
            var navwidth = $("#ulOpsBookingDaysMenu");
            navwidth.animate({ scrollLeft: '-=200' }, 900);
        }
    );

    //NEXT arrow click event
    $(".arrow:nth-of-type(2)").click(
        function () {
            var navwidth = $("#ulOpsBookingDaysMenu");
            navwidth.animate({ scrollLeft: '+=200' }, 900);
        }
    );

    var moretext = ' more info <i class="fa-chevron-circle-right"></i>';
    var lesstext = ' less info <i class="fa-chevron-circle-left"></i>';
    $(".morelink").off('click').on('click', function () {

        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(moretext);
        } else {
            $(this).addClass("less");
            $(this).html(lesstext);
        }
        $(this).parent().prev().toggle();
        $(this).prev().toggle();
        return false;
    });
});

function SetNextPrevToDaysMenu() {
    //if Days menu is > 10 then show the next prev buttons
    if ($("#ulOpsBookingDaysMenu").find("li").length > 10) {
        $(".arrow").show();
        $("#ulOpsBookingDaysMenu").css("overflow", "hidden");
        $("#wrapper").addClass("divwrapper");
        var liActiveDay = $('#ulOpsBookingDaysMenu').find("li.active").attr("data-id");
        if (liActiveDay > 10) {
            var liDayscnt = $("#ulOpsBookingDaysMenu").find("li").length;
            var ulWidth = $("#ulOpsBookingDaysMenu").width();

            $("#ulOpsBookingDaysMenu").animate({ scrollLeft: ulWidth / liActiveDay }, 900);
        }
    }
    else {
        $(".arrow").hide();
    }
}