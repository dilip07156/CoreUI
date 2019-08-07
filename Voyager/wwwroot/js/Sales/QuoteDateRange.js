var dates = new Array();
var datesCopy = new Array();
var indexInMain = 0;
var count = 0;

$(document).ready(function () {
    $('.deleteRow').remove();
    travelDateTypeSelection();

    $('#TravelDateType').click(function () {
        travelDateTypeSelection();
    });
    
    $('#SpecificDateRange').on('click', '.icon-squre-cont .icon-squre-red', function () {
        var date = $(this).closest('tr').find('input[name*="SelectedDate"]')[0].value;
        addOrRemoveDate(date);
        return false;
    });

    $('#frmQuoteDateRange').on('click', '.SaveQuoteDateRange', function () {

        $("#divSuccessError").hide();
        if ($("#SpecificDateRange tbody").find('tr:not([style*="display: none;"])').length == 0) {
            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");//info
            $("#lblmsg").text("Please select date range.");
            $("#stMsg").text("Error! ");
            return false;
        }
         
        if ($('#frmQuoteDateRange').valid()) {
            var model = $('#frmQuoteDateRange').serialize();
            $.ajax({
                type: "POST",
                url: "/Quote/QuoteDateRange",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmQuoteDateRange') > 0) {
                        $("#PaxRange").click();
                        //$('#frmQuoteDateRange').replaceWith(response);
                        //var form = $("#frmQuoteDateRange");
                        //form.removeData('validator');
                        //form.removeData('unobtrusiveValidation');
                        //$.validator.unobtrusive.parse(form);
                    }
                    else {
                        //var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                        //$('#frmQuoteDateRange .tab-spe-dates-sec').before(successmsg);
                    }
                },
                error: function (response) {
                    alert(response.statusText);
                }
            });
        }
        else {
            alert('Please fill mandatory fields!!!');
        }
    });
});

function addDatesOnLoad(dateArray) {
    if (dateArray.length > 0) {
        for (var i = 0; i < dateArray.length; i++) {
            if (dateArray[i].departureId >= 0) {
                var date = dateArray[i].selectedDate;
                dates.push(date);
                datesCopy.push(date);
                count += 1;
            }
        }
    }
    loadDatePicker();
}

function loadDatePicker() {
    $("#datepicker").datepicker({
        numberOfMonths: 3,
        dateFormat: 'dd M yy',
        onSelect: function (dateText, inst) {
            addOrRemoveDate(dateText);
        },
        beforeShowDay: function (date) {
            var year = date.getFullYear();
            var month = date.toLocaleString("en-us", { month: "short" });       //padNumber(date.getMonth() + 1);
            var day = padNumber(date.getDate());
            var dateString = day + " " + month + " " + year;
            var gotDate = jQuery.inArray(dateString, dates);
            if (gotDate >= 0) {
                return [true, "multi-calendar-selected-date"];                  // Enable date so it can be deselected. Set style to be highlighted
            }
            return [true, ""];
        }
    });
}

function addDate(date) {
    if (jQuery.inArray(date, dates) < 0) {
        dates.push(date);
        if (indexInMain < 0) {
            datesCopy.push(date);
            addRowInGrid(date);
            count += 1;
        }
        else {
            addRowInGrid(date);
        }
        //count += 1;
    }
}

function removeDate(date, index) {
    var arrDate = date.split(' ');
    dates.splice(index, 1);
    $('#SpecificDateRange #QuoteSpecificDateViewModel_' + indexInMain + '__IsDeleted').val('True');
    $('#SpecificDateRange tbody').find('input[value="' + date + '"]').closest('tr').hide();
    $('#datepicker').find('span:contains(' + arrDate[1] + ')').closest('.ui-datepicker-group').find('a:contains(' + arrDate[0] + ')').parent().removeClass('multi-calendar-selected-date');
}

function addRowInGrid(dateText) {
    if (indexInMain > -1) {
        $('#SpecificDateRange #QuoteSpecificDateViewModel_' + indexInMain + '__IsDeleted').val('False');
        $('#SpecificDateRange tbody').find('input[value="' + dateText + '"]').closest('tr').show();
    }
    else {
        var clonedRow = $('#TableToClone tbody tr:first').clone();
        $('#SpecificDateRange tbody').append(clonedRow);
        clonedRow.find('input').each(function (index) { this.id = this.id.replace('0', count); this.name = this.name.replace('0', count); });
        $('#SpecificDateRange #QuoteSpecificDateViewModel_' + count + '__SelectedDate').attr('value', dateText);
        $('#SpecificDateRange #QuoteSpecificDateViewModel_' + count + '__DepartureId').val('0');
        var currIndex = count; 
        $.ajax({
            type: "GET",
            url: "/Quote/GetDepartureDatesForQRFId",
            data: { QRFId: '0', SelectedDate: dateText },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) { 
                if (result && result.length) {
                    var warningMsg = result[0].warning;
                    if (warningMsg !== "") {
                        $('#SpecificDateRange #QuoteSpecificDateViewModel_' + currIndex + '__Warning').val(warningMsg);
                        $('#SpecificDateRange tbody tr:last .icon-squre-cont').show();
                    }
                    else {
                        $('#SpecificDateRange #QuoteSpecificDateViewModel_' + currIndex + '__Warning').val('');
                        $('#SpecificDateRange tbody tr:last .icon-squre-cont').hide();
                    }
                }
            },
            failure: function (error) {
                alert('failure' + error);
            },
            error: function (error) {
                alert('error' + error);
            }
        });
    }
}

// Adds a date if we don't have it yet, else remove it
function addOrRemoveDate(date) {
    var index = jQuery.inArray(date, dates);
    indexInMain = jQuery.inArray(date, datesCopy);
    if (index >= 0) {
        removeDate(date, index);
    }
    else {
        addDate(date);
    }
}

// Takes a 1-digit number and inserts a zero before it
function padNumber(number) {
    var ret = new String(number);
    if (ret.length === 1)
        ret = "0" + ret;
    return ret;
}

function travelDateTypeSelection() {
    //if ($('#TravelDateType[value="SpecificDates"]').is(':checked') || $('#TravelDateType[value="DateRange"]').is(':checked')) {
    if ($('#TravelDateType:checked').length) {
        $('.tab-spe-dates-sec').show();
        //$(".radio-list-bar.js-tab  li > *").click();
    }
}

$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});