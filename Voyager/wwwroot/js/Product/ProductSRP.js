$(document).ready(function () {
});

/*----------------------------------Autocomplete City starts here------------------------------------------------*/
$('.fliter-sec').on('keyup', '.bindCity', function (e) {
    var param = { term: this.value };
    InitializeAutoComplete('/Product/GetCityName', this, 3, param, "");
});
/*----------------------------------Autocomplete City ends here------------------------------------------------*/

/*----------------------------------Autocomplete Chain starts here------------------------------------------------*/
$('.fliter-sec').on('keyup', '.bindProdChain', function (e) {
    var param = { term: this.value };
    InitializeAutoComplete('/Product/GetChainName', this, 3, param, "", "#HotelChain");
});
/*----------------------------------Autocomplete Chain ends here------------------------------------------------*/

/*----------------------------------Search Products starts here------------------------------------------------*/
$('.fliter-sec').on('click', '.SearchProducts', function (e) {

    var model = {
        ProductType: $('#ProductType').val(),
        ProdName: $('#ProdName').val(),
        ProdCode: $('#ProdCode').val(),
        CityName: $('#CityIDUI').val(),
        Location: $('#LocationID').val(),
        BudgetCategory: $('#BudgetCategoryID').val(),
        Chain: $('#ChainIDUI').val(),
        StarRating: $('#StarRating').val(),
        Status: $('#Status').val()
    }

    $.ajax({
        type: "POST",
        url: "/Product/ProductSRP",
        data: { filters: model },
        dataType: "html",
        success: function (response) {
            $('#ProductSearchResults').html(response);
            $('.chkSRPList').css("display", "block");
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
});

$(".searchProduct").click(function () {
    var data = {
        BookingNumber: $("#BookingNumber").val(),
        PositionId: $("#Position_Id").val(),
        ProdType: $('#ProductFilterView #ProductType').val(),
        ProdName: $('#ProdName').val(),
        ProdCode: $('#ProdCode').val(),
        CityName: $('#CityIDUI').val(),
        Location: $('#LocationID').val(),
        BudgetCategory: $('#BudgetCategoryID').val(),
        Chain: $('#ChainIDUI').val(),
        StarRating: $('#StarRating').val(),
        Status: $('#Status').val(),
        ServiceType: $('#ServiceType').val()
    };

$.ajax({
    type: "GET",
    url: "/Operations/GetProductList",
    data: data,
    contentType: "application/json; charset=utf-8",
    dataType: "html",
    success: function (response) {
        $("#divProductList").html(response);
    },
    error: function (response) {
        alert(response.statusText);
    }
});
});
/*----------------------------------Search Products ends here------------------------------------------------*/
