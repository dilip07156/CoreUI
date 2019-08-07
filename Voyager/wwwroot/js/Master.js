//Add common functions in this js file to use all over the application

var positionFormData = {};

$(document).ready(function () {
    ShowHideRoleBasedControls('QRF', 'price-foc-sec');
    EnableDisableRoleBasedControls('QRF', 'editorPriceBreakup');
    var d1 = $.cookie('IsCollaps');

    if ($.cookie('IsCollaps')) {
        //$("#collapseExample").removeAttr('class');
        //$("#collapseExample").addClass($.cookie('setCssClass'));
        var d = $.cookie('IsCollaps');
        var c = $("#collapseExample");

        if (d == "true") {
            c.hide();
            //if (c.hasClass("collapse") && c.hasClass("in")) {
            //    $(".wrapper").css({ 'padding-top': ($("header").outerHeight()) });
            //    $("body.booking-flow").css({ 'padding-top': ($("header").outerHeight()) });
            //    $("#btnCollpasToggle").find('span').toggleClass("fa fa-angle-double-up fa fa-angle-double-down");
            //} else {
            $(".wrapper").css({ 'padding-top': '35px' });
            $("body.booking-flow").css({ 'padding-top': '35px' });
            $("#btnCollpasToggle").find('span').removeAttr('class').addClass("fa fa-angle-double-down");
        }
        else if (d == "false") {
            c.show();
            $(".wrapper").css({ 'padding-top': ($("header").outerHeight()) });
            $("body.booking-flow").css({ 'padding-top': ($("header").outerHeight()) });
            $("#btnCollpasToggle").find('span').removeAttr('class').addClass("fa fa-angle-double-up");

        }
        //}
    }

    $("#btnCollpasToggle").on("click", function () {
        //alert("collapsToggle click");

        var data1 = "false";

        if ($.cookie('IsCollaps')) {
            data1 = $.cookie('IsCollaps');
        }

        if (data1.toLowerCase() == "true") {
            data1 = "false";
            $("#collapseExample").show();
            $(".wrapper").css({ 'padding-top': ($("header").outerHeight()) });
            $("body.booking-flow").css({ 'padding-top': ($("header").outerHeight()) });
        }
        else if (data1.toLowerCase() == "false") {
            data1 = "true";
            $("#collapseExample").hide();
            $(".wrapper").css({ 'padding-top': '35px' });
            $("body.booking-flow").css({ 'padding-top': '35px' });
        }

        // $.cookie('setCssClass', data1);
        //fa fa - angle - double - up
        var data = {
            isCollaps: data1
        }
        $.ajax({
            type: "POST",
            url: "/Account/CookieSet",
            data: data,
            // data: data,
            //contentType: "application/json; charset=utf-8",
            dataType: "json",
            global: false,
            success: function (response) {
                //alert(response)
            },
            error: function (response) {

            }

        });


        $(this).find('span').toggleClass("fa fa-angle-double-up fa fa-angle-double-down");
        //$("#hdnCollapsCheck").val("true");



        //$(this).text($(this).text() == '<span class="fa fa-angle-double-up"></span>' ? '<span class="fa fa-angle-double-down"></span>' : '<span class="fa fa-angle-double-down"></span>');
        //collapsToggle();
        //alert($.cookie("setCssClass"));



    });
    //ShowHideRoleBasedControls('Agent_Manuals', 'menu-document-sec');

    loadPostMainJSEvents();
});

function ShowHideRoleBasedControls(userRole, controlClass) {
    if (controlClass.indexOf('.') > -1) {
        controlClass.replace('.', '');
    }
    try {
        var UserRoles = $('#hdnUserRoles').val();
        if (UserRoles != null && UserRoles != '') {
            UserRoles = UserRoles.toLowerCase().split(',');
            if (userRole == 'QRF') userRole = 'Sales Manager';
            if ($.inArray(userRole.toLowerCase(), UserRoles) > 0) {
                $('.' + controlClass).show();
            }
            else {
                $('.' + controlClass).hide();
            }
        }
    } catch (e) {
        $('.' + controlClass).hide();
    }
}

function EnableDisableRoleBasedControls(userRole, controlClass) {

    if (controlClass.indexOf('.') > -1) {
        controlClass.replace('.', '');
    }
    try {
        var UserRoles = $('#hdnUserRoles').val();
        if (UserRoles != null && UserRoles != '') {
            UserRoles = UserRoles.toLowerCase().split(',');
            if (userRole == 'QRF') userRole = 'Sales Officer';
            if ($.inArray(userRole.toLowerCase(), UserRoles) > 0) {
                $('.' + controlClass).attr('disabled', false);
                // $('.cke_toolgroup, .cke_toolbar').css('opacity', '').css('pointer-events', '');
            }
            else {
                $('.' + controlClass).attr('disabled', true);
                // $('.cke_toolgroup, .cke_toolbar').css('opacity', '0.8').css('pointer-events', 'none');
            }
        }
    } catch (e) {
        $('.' + controlClass).attr('disabled', true);
        //  $('.cke_toolgroup, .cke_toolbar').css('opacity', '0.8').css('pointer-events', 'none');
    }
}

function ValidateDate(datefieldid) {
    var dtValue = $(datefieldid).val();
    var dtRegex = new RegExp("^([0]?[1-9]|[1-2]\\d|3[0-1])/(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)/[1-2]\\d{3}$", 'i');
    return dtRegex.test(dtValue);
}

function collapsToggle() {
    var c = $("#collapseExample");
    c.toggle();

    if (c.hasClass("collapse") && c.hasClass("in")) {
        $(".wrapper").css({ 'padding-top': ($("header").outerHeight()) });
        $("body.booking-flow").css({ 'padding-top': ($("header").outerHeight()) });
    } else {
        $(".wrapper").css({ 'padding-top': '35px' });
        $("body.booking-flow").css({ 'padding-top': '35px' });
    }
}

function QuoteRejectOpprtunity(QRFID) {
    var result = confirm("Are you sure you want to Reject this quote? This action is not reversible.");
    if (result) {
        $.ajax({
            type: "POST",
            url: "/Quote/SetQuoteRejectOpportunity",
            data: { QRFID: QRFID },
            dataType: "json",
            success: function (response) {
                if (response.status == 'Success') {
                    alert("Quote Rejected successfully!!!");
                    window.location.href = "/Quote/Quote";
                }
                else {
                    alert("Error in Rejecting Quote : " + response.errorMessage);
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
}

function CopyQuote() {
    var QRFId = $("#QRFID").val();
    $.ajax({
        type: "GET",
        url: "/Quote/GetCopyQuoteData",
        data: { QRFId: QRFId },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        //headers: { 'IsAjaxRequest': 'true' },
        success: function (response) {
            $("#CopyQuote-popup .popup-in").html(response);
            $("#CopyQuote-popup").show();
            var form = $("#frmCopyQuote");
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

/**
 * This function will add the input/select/textarea/table NAME (as key) and VALUE (as value) in the dataList object passed as the parameter.
 * It will return the same list passed in parameter with new values added.
 * @param {any} input 'this' param object of control
 * @param {any} dataList list object of data where data needs to be added
 */
function addChangedValueToList(input, dataList) {
    var key = input.name;
    var value = input.value;

    var table = $(input).closest('table')[0];
    if (table != undefined && table != null) {
        key = $(table).attr('name');
        value = convertTableToJsonString(table);
    }
    dataList[key] = value;
    return dataList;
}

/**
 * This function will take the table and convert its data in Json Array format.
 * It will returt the Json Array in String format.
 * @param {any} oTable table object to be converted in Json string
 */
function convertTableToJsonString(oTable) {
    var jsonArray = [];
    $(oTable).find("tbody tr").each(function (iRowCnt) {
        var jsonObject = {};
        $(this).find('input,select,textarea').each(function (iColCnt) {
            var key = this.name;
            var dotIndex = parseInt(key.lastIndexOf(".")) + 1;
            key = key.substring(dotIndex);
            jsonObject[key] = this.value;
            if (this.type === 'checkbox') jsonObject[key] = this.checked;
        });
        jsonArray.push(jsonObject);
    });
    return JSON.stringify(jsonArray);
}

function loadPostMainJSEvents() {
    $(document).on('change input', '#mainOps select, #mainOps textarea, #mainOps input', function (e) {
        positionFormData = addChangedValueToList(this, positionFormData);
    });
}


function ConfirmOkOnClick() {
    debugger;
    //$.magnificPopup.close();
    $('#confirmOk').magnificPopup('close');

    // 500, time must be bigger, as the delay from the first popup. By me the delay is 300
    var value = $("#confirmOk").attr("data-value");
    if (value === "confirmposition") {
        window.setTimeout(confirmPosition, 400);
    }
    else if (value === "raisevoucher") {
        window.setTimeout(raisePosVoucher, 400);
    }
    else if (value === "confirmbooking") {
        //confirmPosition();
        window.setTimeout(confirmBooking, 400);
    }
    else if (value === "dwdvoucher") {
        window.setTimeout(downloadPosVoucher, 400);
	}
	else if (value === "cancelPosition") {
		window.setTimeout(CancelPosition, 400);
	}
	else if (value === "changeProduct") {
		window.setTimeout(ChangeProduct, 400);
	}	
	else if (value === "fullItinerary") {
		window.setTimeout(fullItinerary, 400);
	}
    else if (value === "updatePosition") {
	    positionFormData['ConfirmBookAmend'] = 'True';
	    window.setTimeout(updatePositionDetails, 400);
	}
}

function getAjaxErrorStatusDescription(jqXHR, exception, errorThrown) {
    var msg = "";
    if (jqXHR.status === 0) {
        msg = 'Not connect.\n Verify Network.';
    } else if (jqXHR.status === 404) {
        msg = 'Requested page not found. [404]';
    } else if (jqXHR.status === 500) {
        msg = 'Internal Server Error [500].';
    } else if (exception === 'parsererror') {
        msg = 'Requested JSON parse failed.';
    } else if (exception === 'timeout') {
        msg = 'Time out error.';
    } else if (exception === 'abort') {
        msg = 'Ajax request aborted.';
    } else if (jqXHR.status === 401) {
        msg = '401 : Session Timeout';
    } else {
        msg = 'Uncaught Error.\n' + jqXHR.status + ' : ' + errorThrown;
    }
    return msg;
}

function ConfirmCancelOnClick() {
    debugger;
    //$.magnificPopup.close();
    $('#confirmCancel').magnificPopup('close');

    // 500, time must be bigger, as the delay from the first popup. By me the delay is 300
    var value = $("#confirmCancel").attr("data-value");
    if (value === "updatePosition") {
        positionFormData['ConfirmBookAmend'] = 'False';
        window.setTimeout(updatePositionDetails, 400);
    }
}