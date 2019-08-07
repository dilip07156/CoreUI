$(document).ready(function () {


    //var checkedValue = $('.chkIsProdType:checked').val();

    //if (checkedValue.toLowerCase() == 'summary')
    //    $("#summarySection").show();

    //$(".ChkProductTypeList .chkIsProdType").click(function () {

    //    if ($(this).is(":checked")) {
    //        var ProductName = $(this).closest('div').find('span.ProductType');//.text().replace(' ', '-');
    //        $('.ProductTypeSection.' + ProductName).find('.chkIsProdType').prop('checked', true);
    //    }
    //    else {
    //        var ProductName = $(this).closest('div').find('span.ProductType');//.text().replace(' ', '-');
    //        $('.ProductTypeSection.' + ProductName).find('.chkIsProdType').prop('checked', false);
    //    }
    //});

    $(".chkSummary").click(function () {
        if ($('.chkSummary').prop('checked'))
            $("#summarySection").show();
        else
            $("#summarySection").hide();
    });


    $('.chkIsProdType').click(function () {
         
        var checkedValue = this.value;        
        if (checkedValue == "Hotel") {
            $("#hotelSection").toggle();
        }
        if (checkedValue == "Meal") {
            $("#mealSection").toggle();
        }
        if (checkedValue == "Overnight Ferry") {
            $("#transferSection").toggle();
        }
    });
});

