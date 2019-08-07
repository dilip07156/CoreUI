//On CTRL F5/F5 event, keep the selected Tab(Itinerary,Acco,Meals,etc.) activated and in Days List set the 1st Day Tab Active.
$(document).on("keydown", "body", function (e) {
    if ((e.keyCode == 116 && e.ctrlKey) || (e.keyCode == 116)) {
        e.preventDefault();
        var SelectedMenu = $("#MenuName").val();
        if (SelectedMenu === "Itinerary") {
            $("#aItinerary").click();
        }
        else {
            $("#ulOpsProdType").find('li').each(function () {
                var anchorThis = $(this).find("a");
                if (anchorThis.text().trim().toLocaleLowerCase() === SelectedMenu.toLocaleLowerCase()) {
                    var bookingNumber = $("#BookingNumber").val();
                    $("#divOpsProdTypeMain").find(".sqr-tab-data").html('').hide();
                    $("#MenuName").val(anchorThis.parent("li").attr("liprodtype"));
                    var prodType = anchorThis.attr("prodtype");
                    $('#divOpsProdTypeInfo').load('/Operations/ViewProductTypeDetails', { ProductType: prodType, BookingNumber: bookingNumber });
                    $("#ulOpsProdType").find(".active").removeClass("active");
                    $("#" + anchorThis.parent("li").attr("id")).addClass("active");
                    $("#divOpsProdTypeInfo").show();
                    return;
                }
            });
        }
    }
});