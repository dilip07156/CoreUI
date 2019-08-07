$(document).ready(function () {
    $("#chkMargin li").removeAttr('class');
    var chkid = "#chk" + $("#SelectedMargin").val();

    $(chkid).parent().parent().addClass('active');

    $('#chkMargin li :checkbox').prop('checked', false);
    $(chkid).prop('checked', true);

    if (chkid == "#chkPackage") {
        $('#tblPackage').show();
        $('#tblProduct,#divProductMargin,#tblItemwise,#divItemwiseMargin').hide();
    }
    else if (chkid == "#chkProduct") {
    //show - margin - positions
        $('#tblProduct,#divProductMargin').show();
        $('#tblPackage,#tblItemwise,#divItemwiseMargin').hide();
    }
    else if (chkid == "#chkServiceItem") {
        $('#tblItemwise,#divItemwiseMargin').show();
        $('#tblPackage,#tblProduct,#divProductMargin').hide();
    }
    else {
        $('#tblPackage,#tblProduct,#divProductMargin,#tblItemwise,#divItemwiseMargin').hide();
    }


    if ($('#rdoExcAccommodation').is(":checked")) {
        $("#txtExcMarginUnit").prop('disabled', false);
        $("#txtExcSellingPrice").prop('disabled', false);
        $("#txtIncMarginUnit").prop('disabled', true);
        $("#txtIncSellingPrice").prop('disabled', true);
    }
    else if ($('#rdoIncAccommodation').is(":checked")) {
        $("#txtExcMarginUnit").prop('disabled', true);
        $("#txtExcSellingPrice").prop('disabled', true);
        $("#txtIncMarginUnit").prop('disabled', false);
        $("#txtIncSellingPrice").prop('disabled', false);
    }


    $("#rdoSupAccommodation").closest('tbody').find('input[type="text"]').attr('disabled', 'disabled');

    if ($('#rdoSupAccommodation').is(":checked")) {
        $("#txtSupSellingPrice").prop('disabled', false);
        $("#txtSupMarginUnit").prop('disabled', false);
    }
    else {
        $("#txtSupSellingPrice").prop('disabled', true);
        $("#txtSupMarginUnit").prop('disabled', true);
    }

    if ($('#rdoOptAccommodation').is(":checked")) {
        $("#txtOptSellingPrice").prop('disabled', false);
        $("#txtOptMarginUnit").prop('disabled', false);
    }
    else {
        $("#txtOptSellingPrice").prop('disabled', true);
        $("#txtOptMarginUnit").prop('disabled', true);
    }


    $("#tblItemwise,#tblProduct").find('input[type="text"]').attr('disabled', 'disabled');
    $('#tblItemwise .chkIsProdType:checked,#tblProduct .chkIsProdType:checked').closest('tr').find('input[type="text"]').removeAttr('disabled');


    $("#Margin-popup .marginClose").click(function () {
        GetCommercialsData();
    });

    $("#Margin-popup .btnCloseMargin").click(function () {
        $('#Margin-popup .marginClose').click();
    });
});

$(".bindCurrency").on("keyup keypress blur", function (event) {
    $(this).val($(this).val().replace(/[^a-zA-Z%]+/g, ''));
    var key = event.which;
    if (!((key == 8 || key == 13 || key == 27 || key == 0) || (key >= 97 && key <= 122) || (key == 37 || this.value.indexOf('%') != -1))) {
        event.preventDefault();
    }
    else if (this.value == "%") {
        getCurrencyNames("/Quote/GetActiveCurrency", this, 1, { term: this.value });
        // InitializeAutoComplete('GetActiveCurrency', this, 1, { term: this.value },"");
    }
    else {
        getCurrencyNames("/Quote/GetActiveCurrency", this, 2, { term: this.value });
        //InitializeAutoComplete('GetActiveCurrency', this, 2, { term: this.value },"");
    }

});

function getCurrencyNames(actionUrl, thisparam, minlen, param, status = "") {

    if (actionUrl != undefined && actionUrl != "") {
        var controlId = "#" + $(thisparam).attr('id');
        var hdnId = controlId.replace('UI', '');
        $(controlId).autocomplete({
            source: actionUrl,
            function(request, response) {
                $.getJSON(
                    actionUrl,
                    param,
                    response
                );
            },
            minLength: minlen,
            select: function (e, ui) {
                if (ui.item) {
                    $(controlId).val(ui.item.label);
                    $(hdnId).val(ui.item.value);
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                    e.preventDefault();
                } else {//if list null then hide the list 
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                }
            },
            focus: function (e, ui) { //on key down/up ID shows in textbox to handle this focus event used 
                $(controlId).val(ui.item.label);
                $(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
                e.preventDefault();

            },
            change: function (e, ui) { //if text is not present in list then blank the textbox      
                $(".ui-widget.ui-widget-content").css("border", "none");
                $(".ui-menu-item").hide();
                $(".ui-menu").css("border", "none");
                $(".ui-menu").css("width", "0px");//add for handling width on browsers 

                if (!ui.item) {
                    $(this).val('');
                }
                else if ($(this).val().length >= minlen && (e.which == 0 || e.which === 13) && status != '') {
                    AutoCompleteChanged(this);
                }
            }
        }).keyup(function (e) {
            if ($(controlId).val().length >= minlen) {
                $(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
            }
            else if ($(controlId).val().length < minlen) {
                $(".ui-menu-item").hide();
                $(".ui-menu").css("border", "none");
                $(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
            }
            if (e.which === 13 || e.which === 27) {//on enter or escape key hide list
                $(".ui-menu-item").hide();
                $(".ui-menu").css("border", "none");
                $(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
            }
        }).blur(function (e) { //if char in textbox is 1 then hide the list     
            if (e.which === 8 || e.which === 0) {//on backspace and tab key
                if ($(controlId).val().length < minlen) {
                    $(".ui-autocomplete").css("width", "0px"); //add for handling width on browsers
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                }
            }
        });
    }
}

//Apply Margin checkbox
$('#chkMargin input[type=checkbox]').click(function () {

    $("#chkMargin li").removeAttr('class');
    var chkid = $(this).attr('id');
    $("#SelectedMargin").val(chkid.replace('chk', ''));

    if ($(this).is(":checked")) {
        $(this).parent().parent().addClass('active');
    }

    $('#chkMargin li :checkbox').not(this).prop('checked', false);

    if (chkid == "chkPackage" && $(this).is(":checked")) {
        $('#tblPackage').show();
        $('#tblProduct,#divProductMargin,#tblItemwise,#divItemwiseMargin').hide();
    }
    else if (chkid == "chkProduct" && $(this).is(":checked")) {
        $('#tblProduct,#divProductMargin').show();
        $('#tblPackage,#tblItemwise,#divItemwiseMargin').hide();
    }
    else if (chkid == "chkServiceItem" && $(this).is(":checked")) {
        $('#tblItemwise,#divItemwiseMargin').show();
        $('#tblPackage,#tblProduct,#divProductMargin').hide();
    }
    else {
        $('#tblPackage,#tblProduct,#divProductMargin,#tblItemwise,#divItemwiseMargin').hide();
    }
});

$(".SaveMargin").click(function (e) {
    var flag = false;
    if ($('#chkPackage').is(":checked")) {
        flag = ChkPackage(e);
    }
    else if ($('#chkProduct').is(":checked")) {
        $("#spanPkgComponent").text("");
        if ($("#tblProduct .chkIsProdType:checked").length > 0) {
            $("#tblProduct .chkIsProdType:checked").each(function () {

                var index = $(this).attr('id').split('_')[1];
                var txtSellingPrice = "#txtSellingPrice_" + index;
                var txtMarginUnit = "#txtMarginUnit_" + index;
                var lblMarginUnit = "#lblMarginUnit_" + index;
                var lblSellingPrice = "#lblSellingPrice_" + index;

                if ($(txtSellingPrice).val() == '' && $(txtMarginUnit).val() == '') {
                    $(lblMarginUnit).text('*');
                    $(lblSellingPrice).text('*');
                    flag = true;
                    // e.preventDefault();
                }
                else if ($(txtMarginUnit).val() == '') {
                    $(lblMarginUnit).text('*');
                    $(lblSellingPrice).text('');
                    flag = true;
                    // e.preventDefault();
                }
                else if ($(txtSellingPrice).val() == '') {
                    $(lblSellingPrice).text('*');
                    $(lblMarginUnit).text('');
                    flag = true;
                    // e.preventDefault();
                }
                else {
                    $(lblSellingPrice).text('');
                    $(lblMarginUnit).text('');
                }
            });
        }
        else {
            $("#spanPkgComponent").text("Please select atleast one component.");
            $("#spanPkgComponent").addClass("text-danger");
            flag = true;
            // e.preventDefault();
        }
    }
    else if ($('#chkServiceItem').is(":checked")) {
        $('#tblItemwise .chkIsProdType:checked').each(function (index) {
            var row = $(this).closest('tr');
            var cur = row.find('.bindCurrency').val();
            var price = row.find('.txtProdPrice').val();
            if (cur == undefined || cur == null || cur == '') {
                row.find('label[id*="lblItemMarginUnit"]').text('*');
                flag = true;
                //  e.preventDefault();
            }
            if (price == undefined || price == null || price == '') {
                row.find('label[id*="lblItemSellingPrice"]').text('*');
                flag = true;
                // e.preventDefault();
            }
        })
    }

    if (flag) {
        e.preventDefault();
    }
    else {
        SaveMargin();
    }
});

function ChkPackage(e) {
    var flag = false;
    if (!$('#rdoIncAccommodation').is(":checked") && !$('#rdoExcAccommodation').is(":checked")) {
        $("#spanPkgComponent").text("Please select atleast one component.");
        $("#spanPkgComponent").addClass("text-danger");
        flag = true;
        //  e.preventDefault();
    }
    else {
        flag = false;
        $("#spanPkgComponent").text(" ");
    }

    if ($('#rdoIncAccommodation').is(":checked")) {
        if (($('#txtIncSellingPrice').val() == "") && $('#txtIncMarginUnit').val() == "") {
            $('#spanIncSellingPrice').show();
            $('#spanIncMarginUnit').show();
            //  e.preventDefault();
            flag = true;
        }
        else if (($('#txtIncSellingPrice').val() == "") && $('#txtIncMarginUnit').val() != "") {
            $('#spanIncSellingPrice').show();
            $('#spanIncMarginUnit').hide();
            // e.preventDefault();
            flag = true;
        }
        else if ($('#txtIncMarginUnit').val() == "" && $('#txtIncSellingPrice').val() != "") {
            $('#spanIncSellingPrice').hide();
            $('#spanIncMarginUnit').show();
            //  e.preventDefault();
            flag = true;
        }
        else {
            flag = false;
            $('#spanIncSellingPrice').hide();
            $('#spanIncMarginUnit').hide();
        }
    }
    else if ($('#rdoExcAccommodation').is(":checked")) {
        if (($('#txtExcSellingPrice').val() == "") && $('#txtExcMarginUnit').val() == "") {
            $('#spanExcSellingPrice').show();
            $('#spanExcMarginUnit').show();
            //  e.preventDefault();
            flag = true;
        }
        else if (($('#txtExcSellingPrice').val() == "" ) && $('#txtExcMarginUnit').val() != "") {
            $('#spanExcSellingPrice').show();
            $('#spanExcMarginUnit').hide();
            // e.preventDefault();
            flag = true;
        }
        else if ($('#txtExcMarginUnit').val() == "" && $('#txtExcSellingPrice').val() != "" ) {
            $('#spanExcSellingPrice').hide();
            $('#spanExcMarginUnit').show();
            //  e.preventDefault();
            flag = true;
        } else {
            flag = false;
            $('#spanExcSellingPrice').hide();
            $('#spanExcMarginUnit').hide();
        }
    }

    if ($('#rdoSupAccommodation').is(":checked")) {
        var SupPrice = $('#txtSupSellingPrice').val();
        var SupUnit = $('#txtSupMarginUnit').val();

        if (SupPrice != null && !isNaN(SupPrice) && parseInt(SupPrice) > 0) { }
        else {
            $('#spanSupSellingPrice').show(); flag = true;//e.preventDefault(); 
        }

        if (SupUnit != null && SupUnit != '') { }
        else {
            $('#spanSupMarginUnit').show(); flag = true;//e.preventDefault(); 
        }
    }
    if ($('#rdoOptAccommodation').is(":checked")) {
        var OptPrice = $('#txtOptSellingPrice').val();
        var OptUnit = $('#txtOptMarginUnit').val();

        if (OptPrice != null && !isNaN(OptPrice) && parseInt(OptPrice) > 0) { }
        else {
            $('#spanOptSellingPrice').show(); flag = true;//e.preventDefault(); 
        }

        if (OptUnit != null && OptUnit != '') { }
        else {
            $('#spanOptMarginUnit').show(); flag = true;//e.preventDefault();
        }
    }

    if (flag) {
        return true;
    }
    else {
        return false;
    }
}

function SaveMargin() {
     
    var model = $('#frmQuoteMarginInformation').serialize();
    var IsCostingMargin = $('#IsCostingMargin').val();
    if (model != "" && model != null) {
        $.ajax({
            type: "POST",
            url: "/Quote/QuoteMarginInformation",
            data: model + "&IsCostingMargin=" + IsCostingMargin,
            dataType: "html",
            success: function (response) {
                if (response.indexOf('frmQuoteMarginInformation') > 0) {
                    $('#frmQuoteMarginInformation').replaceWith(response);
                    //var form = $("#frmQuoteMarginInformation");
                    //form.removeData('validator');
                    //form.removeData('unobtrusiveValidation');
                    //$.validator.unobtrusive.parse(form);
                }
                else {
                    var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Margin Details not saved.</div>';
                    $('.simple-tab-cont').before(successmsg);
                }
            },
            error: function (response) {
                var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.statusText + '</div>';
                $('.simple-tab-cont').before(successmsg);
            }
        });
    }
}

$('input[name="PackageMarginDetails.Accommodation"]').click(function () {
    if (this.checked == true) {
        $("#spanPkgComponent").text(" ");
        $(this).closest('tbody').find('input[type="text"]').val('').attr('disabled', 'disabled');
        $(this).closest('tr').find('input[type="text"]').removeAttr('disabled');
        $(this).closest('tr').find('span.text-danger').hide();
    }
});

$('#rdoSupAccommodation,#rdoOptAccommodation').click(function () {

    $(this).closest('tr').find('span.text-danger').hide();
    if (this.checked == true) {
        $(this).closest('tr').find('input[type="text"]').removeAttr('disabled');
    }
    else {
        $(this).closest('tr').find('input[type="text"]').val('').attr('disabled', 'disabled');
    }
});

$('.chkIsProdType').click(function () {
    if (!$(this).is(":checked")) {
        var index = this.id.split('_')[1];
        var lblMarginUnit = "#lblMarginUnit_" + index;
        var lblSellingPrice = "#lblSellingPrice_" + index;
        $(lblSellingPrice).text('');
        $(lblMarginUnit).text('');

        var txtSellingPrice = "#txtSellingPrice_" + index;
        $(txtSellingPrice).val('');

        var txtExcMarginUnit = "#txtMarginUnit_" + index;
        $(txtExcMarginUnit).val('');
        $(this).closest('tr').find('input[type="text"]').attr('disabled', 'disabled').val('');
    }
    else {
        $(this).closest('tr').find('input[type="text"]').removeAttr('disabled');
    }
});

$('.rdoMargin').click(function () {
    var controlId = "#" + $(this).attr('id');

    if ($(controlId).attr('checked')) {
        $(controlId).removeAttr('checked')
    } else {
        $(controlId).attr('checked', true)
    }
});

$('.StandardRow .show-margin-positions').click(function () {

    var ProductName = $(this).closest('td').find('.ProductName').text();
    if (ProductName != null && ProductName != '') {
        ProductName = ProductName.replace(' ', '-');
        $('.' + ProductName).toggle();
        $(this).toggleClass('fa-chevron-circle-up fa-chevron-circle-down');
    }
});

$('.StandardRow .show-margin-products').click(function () {

    var ProductMaster = $(this).closest('td').find('.ProductMaster').text();
    if (ProductMaster != null && ProductMaster != '') {
        ProductMaster = ProductMaster.replace(' ', '-');
        $('.' + ProductMaster).toggle();
        $(this).toggleClass('fa-chevron-circle-up fa-chevron-circle-down');
    }
});



//function commented() {
    //$('.StandardRow .chkIsProdType').click(function () {
    //    if ($(this).is(":checked")) {
    //        var ProductName = $(this).closest('td').find('span.ProductName').text().replace(' ', '-');
    //        $('.ProductRow.' + ProductName).find('.chkIsProdType').prop('checked', true);
    //    }
    //    else {
    //        var ProductName = $(this).closest('td').find('span.ProductName').text().replace(' ', '-');
    //        $('.ProductRow.' + ProductName).find('.chkIsProdType').prop('checked', false);
    //    }
    //});
    //$('.ProductRow .chkIsProdType').click(function () {
    //     
    //    if (!$(this).is(":checked")) {
    //        var ProdType = $(this).closest('td').find('input[id*="ItemProdtype_Id_"]').val();
    //        var ProdSpan = $('span.ProductName:contains("' + ProdType + '")');
    //        $(ProdSpan).closest('td').find('.chkIsProdType').prop('checked', false);
    //    }
    //});
//}

