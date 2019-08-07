/*----------------------------------Autocomplete City starts here------------------------------------------------*/
$(document).on('keyup', '.bindCity', function (e) { 
    var QRFId = $("#frmAccomodation #QRFId").val();
    var param = { term: this.value };
    if (QRFId != undefined && QRFId != null && !isNaN(QRFId)) {
        param = { term: this.value, QRFID: QRFId };
    }
    InitializeAutoComplete('GetCityName', this, 3, param, "");
});
/*----------------------------------Autocomplete City ends here------------------------------------------------*/

function GetPositionPricesPartView(divPrices, QRFId, PositionId, ProductID) { 
    var IsClone1 = $("#hdnIsClone").val();

    $.ajax({
        type: "GET",
        url: "/Position/GetPositionPricesPartView",
        data: { QRFId: QRFId, PositionId: PositionId, ProductID: ProductID, IsClone: IsClone1 },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $(divPrices).html(response);
            PopulatePriceRows($(divPrices).find('.StandardPriceFOC')[0]);
            if (IsClone1 == "True") {
                if ($("#EnquiryPipeline").val() != "Amendment Pipeline") {
                    $('input[type="text"]').prop('disabled', 'disabled');
                    $('.btn-cancel').hide();
                    $('.btn-save').hide();
                }
                $('<input>').attr({ type: 'hidden', id: 'IsClone', name: 'IsClone', value: true }).appendTo('form');
            }
        },
        error: function (response) {
            $(".ajaxloader").hide();
            alert(response.statusText);
        }
    });
}

function GetPositionFOCPartView(divPrices, QRFId, PositionId, ProductID) {
    var hdnIsClone = $("#hdnIsClone").val();
    $.ajax({
        type: "GET",
        url: "/Position/GetPositionFOCPartView",
        data: { QRFId: QRFId, PositionId: PositionId, ProductID: ProductID, IsClone: hdnIsClone },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $(divPrices).html(response);
            PopulatePriceRows($(divPrices).find('.StandardPriceFOC')[0]);
            if (hdnIsClone == "True") {
                if ($("#EnquiryPipeline").val() != "Amendment Pipeline") {
                    $('input[type="text"]').prop('disabled', 'disabled');
                    $('.btn-cancel').hide();
                    $('.btn-save').hide();
                }
                $('<input>').attr({ type: 'hidden', id: 'IsClone', name: 'IsClone', value: true }).appendTo('form');
            }
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

function GetAccomodationData(QRFId, PositionId, SuccessMessage, ErrorMessage,IsClone) {
    $.ajax({
        type: "GET",
        url: "/Accomodation/Accomodation",
        data: { QRFId: QRFId, PositionId: PositionId, SuccessMessage: SuccessMessage, ErrorMessage: ErrorMessage, IsClone: IsClone },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $(".displayPosition").html(response);
            $(".displayAccomodation").html(response);
            if (IsClone == true)
            $('<input>').attr({ type: 'hidden', id: 'IsClone', name: 'MenuViewModel.IsClone', value: true }).appendTo('form');
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
}

function PositionPopupChange(thisparam, QRFID, positionid, index, flag, dayname, dataaction="") {
    var el = $(thisparam);
    // $("#divpopup").html('');
    if ($(thisparam).is(":checked") || $(thisparam).attr("data-type") == "anchor") {
        var trpopup = el.parents('tr').next('.tr-pop-sec');
        var el_divpopup = trpopup.find('#divpopup');

        if (flag == "transfer") {
            var objGetParam = { QRFId: QRFID, PositionId: positionid, Day: dayname };
            el_divpopup.load('/Transfers/GetTransfersType/', objGetParam, function (event) {
                PositionTypePopup(thisparam);
                trpopup.find("#hfProductType").val($(thisparam).parents("td").attr("data-label"));
                trpopup.find("#hfPositionSeq").val($(thisparam).parents("tr").find("[id^=hfPositionSeq_]").val());
                trpopup.find("#hfDayID").val($(thisparam).parents("tr").find("[id^=hfDayID_]").val());
                trpopup.find("#hfRoutingDaysID").val($(thisparam).parents("tr").find("[id^=hfRoutingDaysID_]").val());
                trpopup.find(".btn-pop-closepopup").show();
            });
        }
        else if (flag == "mealvenue") {
            var objVenueGetParam = { QRFId: QRFID, PositionId: positionid, Day: dayname, MealType: dataaction  };

            el_divpopup.load('/Meals/GetMealsvenue/', objVenueGetParam, function (event) {
                PositionTypePopup(thisparam);
                if (positionid == 0 || positionid == undefined) {
                    trpopup.find("#rdoEnroute").prop("checked", "true");
                }
                trpopup.find("#txtVenueNameUI").attr("id", "txtVenueNameUI_" + index);
                trpopup.find("#txtVenueName").attr("id", "txtVenueName_" + index);
                trpopup.find("#txtVenueName_Type").attr("id", "txtVenueName_" + index + "Type");
                trpopup.find("#txtVenueName_TypeID").attr("id", "txtVenueName_" + index + "TypeID");
                trpopup.find("#txtVenueName_PlaceHolder").attr("id", "txtVenueName_" + index + "PlaceHolder");
                trpopup.find("#hfindex").val(index);
                trpopup.find("#hfPositionSeq").val($(thisparam).parents("tr").find("[id^=hfPositionSeq_]").val());
                trpopup.find("#hfRoutingDaysID").val($(thisparam).parents("tr").find("[id^=hfRoutingDaysID_]").val());

                trpopup.find("#hfDayID").val($(thisparam).parents("tr").find("[id^=DayID_]").val().replace("Day ", ""));
                trpopup.find(".btn-pop-closepopup").show();
                if (trpopup.find("[id^=txtVenueNameUI_]").val() != "" && trpopup.find("[name=PlaceHolder]").val() != undefined && trpopup.find("[name=PlaceHolder]").val().toLowerCase() == "true") {
                    trpopup.find("#lblApplyAcrossDays").show();
                }
                else {
                    trpopup.find("#lblApplyAcrossDays").hide();
                    trpopup.find("#chkApplyAcrossDays").prop("checked", false);
                }
                //trpopup.find(".divRoomeDetails div").first().addClass("col-sm-12");
            });
        }
    } else {
        var el_TrPopSec = el.parents('tr').next('.tr-pop-sec'),
            el_ActionPop = el_TrPopSec.find('.' + el.attr('data-action') + '-popup'),
            el_ServicesTypeCol = el.parents('tr').find('.services-type'),
            el_SqrTabData = el.parents('.sqr-tab-data');

        //Row - Hide
        if (el_TrPopSec.css('display') == 'block' || el_TrPopSec.css('display') == 'table-row')
            el_TrPopSec.css({ display: 'none' });

        //Action Pop - Hide
        if (el_ActionPop.css('display') == 'block') {
            el_TrPopSec.find('.action-popup').hide();
        }

        el.parents('tr').children().removeClass('active');
        el.parents('td').removeClass('selected');

        //services Type - Hide
        if (el_ServicesTypeCol.length > 0)
            el_ServicesTypeCol.find('i').hide();

        if ($('.required-chk:checked').length == 0)
            el_SqrTabData.find('.btn-save').addClass('disabled');

    }
}

function PositionTypePopup(thisparam) {
    // if ($(thisparam).is(":checked")) {
    var chkValue = $(thisparam).attr('data-name').replace('-type', '');
    chkValuePopup = chkValue + "-popup";

    var el = $(thisparam), el_TrPopSec = el.parents('tr').next('.tr-pop-sec');
    el_TrPopSec.find('.action-popup').addClass(chkValuePopup);
    el_TrPopSec.find('.action-popup').attr("data-sec", chkValue);

    chkValue = $(thisparam).parents("td").attr("data-label");

    el_TrPopSec.find('.headertextposition').text(chkValue);

    var el_ActionPop = el_TrPopSec.find('.' + el.attr('data-action') + '-popup'),
        el_ServicesTypeCol = el.parents('tr').find('.services-type'),
        el_SqrTabData = el.parents('.sqr-tab-data');

    //Row - Show
    if (el_TrPopSec.css('display') == 'none')
        el_TrPopSec.show();

    //Action Pop - Show
    if (el_ActionPop.css('display') == 'none') {
        el_TrPopSec.find('.action-popup').hide();
        el_ActionPop.show();
    }

    //services Type - Show
    if (el_ServicesTypeCol.find('.' + el.attr('data-action') + '-icon').css('display') == 'none' && el_ServicesTypeCol.length > 0) {
        el_ServicesTypeCol.find('i').hide();
        el_ServicesTypeCol.find('.' + el.attr('data-action') + '-icon').css({ display: 'inline-block' });
    }
    el.parents('tr').children().removeClass('active');
    el.parents('tr').children().removeClass('selected');

    el.parents('td').addClass('selected active');
    el_SqrTabData.find('.btn-save').removeClass('disabled');
    //  } 
}

function FilterPriceFOC(thisparam) {

    var stdFOC = false;
    var selDate = $(thisparam).closest('form').find('.FilterDateRange').val();
    var selSlab = $(thisparam).closest('form').find('.FilterPaxSlab').val();

    if ($(thisparam).closest('form').find('.StandardPriceFOC').length > 0) {
        stdFOC = $(thisparam).closest('form').find('.StandardPriceFOC')[0].checked;
    }

    if (stdFOC == false) {
        if (selDate != null && selDate.toUpperCase() == 'ALL' && selSlab.toUpperCase() == 'ALL') {
            $(thisparam).closest('form').find('table .DetailedRow').show();
        }
        else {
            if (selDate != null && selDate.toUpperCase() == 'ALL' || selSlab.toUpperCase() == 'ALL') {
                $(thisparam).closest('form').find('table .DetailedRow').show();
                $(thisparam).closest('form').find('table .DetailedRow').each(function (index) {
                    var dateRange = $(this).find('span.spanDateRange').text();
                    var paxSlab = $(this).find('input.PaxSlab').val();

                    if (selDate.toUpperCase() == 'ALL') {
                        if (paxSlab.indexOf(selSlab) > -1) $(this).show();
                        else $(this).hide();
                    }
                    else if (selSlab.toUpperCase() == 'ALL') {
                        if (dateRange.indexOf(selDate) > -1) $(this).show();
                        else $(this).hide();
                    }
                });
            }
            else {
                $(thisparam).closest('form').find('table .DetailedRow').each(function (index) {

                    var dateRange = $(this).find('span.spanDateRange').text();
                    var paxSlab = $(this).find('input.PaxSlab').val();
                    if (dateRange.indexOf(selDate) > -1 && paxSlab.indexOf(selSlab) > -1) $(this).show();
                    else $(this).hide();
                });
            }
        }
    }
}

function PopulatePriceRows(thisparam) {
    try {
        if (thisparam.checked == true) {
            $(thisparam).closest('form').find('.StandardRow').show();
            $(thisparam).closest('form').find('.DetailedRow').hide();
            $(thisparam).closest('form').find('.FilterDateRange').prop('disabled', true);
            $(thisparam).closest('form').find('.FilterPaxSlab').prop('disabled', true);
        }
        else if (thisparam.checked == false) {
            $(thisparam).closest('form').find('.StandardRow').hide();
            $(thisparam).closest('form').find('.DetailedRow').show();
            $(thisparam).closest('form').find('.FilterDateRange').prop('disabled', false);
            $(thisparam).closest('form').find('.FilterPaxSlab').prop('disabled', false);
            FilterPriceFOC(thisparam);
        }
    } catch (e) {
        console.log(e.message);
    }
}
/*-------------------------------------Grid Nested popup starts starts here-------------------------------*/
//popup close button
$(document).on("click", ".div-pop-sec .btn-pop-closepopup", function (e) {
    e.preventDefault();
    var el = $(this), el_ActionPop = el.parents('.action-popup'), el_TrPopSec = el.parents('.tr-pop-sec');

    $("#divSuccessError").hide();
    $("#divSuccessError").removeAttr("class");
    $("#lblmsg").text("");
    $("#stMsg").text("");

    var datasec = $(this).parents('div').attr("data-sec");

    var hfPositionId = el_TrPopSec.prev('tr').find('td.active').find("[datasechf^=" + datasec + "]").attr("id");
    // var hfmealid = el_TrPopSec.prev('tr').find('td.active').find("[id^=hfPositionId_]").attr("id");

    var PositionId = el_TrPopSec.find("#hfPositionId").val();
    if (PositionId != 0 && PositionId != undefined) {
        $("#" + hfPositionId).val(PositionId);
        el_TrPopSec.prev('tr').find('td.active').find("[id^=viewdetails]").css("display", "block");
        el_TrPopSec.prev('tr').find('td.active').find('input[type="checkbox"][data-name = "' + el_ActionPop.attr('data-sec') + '-type"]').prop('checked', true);
    }
    else {
        el_TrPopSec.prev('tr').find('td.active').find('input[type="checkbox"][data-name = "' + el_ActionPop.attr('data-sec') + '-type"]').prop('checked', false);
        el_TrPopSec.prev('tr').find('td.active').find("[id^=viewdetails]").css("display", "none");
    }
    //  $("#" + hfmealid).val(el_TrPopSec.find("#hfMealID").val());

    el_TrPopSec.prev('tr').find('td.active').removeClass('selected');
    el_TrPopSec.prev('tr').children().removeClass('active');
    el_ActionPop.hide();
    el_TrPopSec.hide();
    $("#divpopup").html('');
});

//Message close button
$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});
/*-------------------------------------Grid Nested popup ends here-------------------------------*/

/*----------------------------------Autocomplete pick/Dropup loc------------------------------------------------*/
$(document).on('keyup', '.bindPickUpLoc', function (e) {
    InitializeAutoComplete('GetPickUpDropOffLocations', this, 3, { term: this.value }, "");
});
/*----------------------------------Autocomplete pick/Dropup loc------------------------------------------------*/

$(document).on('change', '.FilterDateRange', function () {
    FilterPriceFOC(this);
});

$(document).on('change', '.FilterPaxSlab', function () {
    FilterPriceFOC(this);
});

$(document).on('click', '.StandardPriceFOC', function () {
    PopulatePriceRows(this);
});

/*-------------------------------------populating city automatically on positions on Day Dropdown starts here------------------------------------*/
$(document).on("change", ".ddlday", function (event) {
    var thisparam = $(this);
    var citynm = thisparam.find('option:selected').attr("cityname");
    var cityid = thisparam.find('option:selected').attr("cityid");
    thisparam.parents("tr").find(".fromcityUI").val(citynm);
    thisparam.parents("tr").find(".fromcity").val(cityid);

    if (thisparam.parents("tr").find(".fromcityUI").length > 0) {
        var curdur = thisparam.parents("tr").find(".ddlDuration").val();
        var curday = thisparam.find('option:selected').text().replace("Day ", "");
        var dayno = parseInt(curdur) + parseInt(curday);
        var day = "Day " + dayno;

        var tocitynm = thisparam.find('option:contains("' + day + '")').attr("cityname");
        var tocityid = thisparam.find('option:contains("' + day + '")').attr("cityid");

        thisparam.parents("tr").find(".tocityUI").val(tocitynm);
        thisparam.parents("tr").find(".tocity").val(tocityid);
		thisparam.parents("tr").find(".fromcityUI").change();
    }
});

$(document).on("change", ".ddlDuration", function (event) {
     
    var thisparam = $(this);
    if (thisparam.parents("tr").find(".fromcityUI").length > 0) {
        var dayid = thisparam.parents("tr").find(".ddlday").val(); 
        var dayname = thisparam.parents("tr").find(".ddlday option[value='" + dayid + "']").text().replace("Day ", "").trim(); 
        var dayno = parseInt(thisparam.val()) + parseInt(dayname);
        var day = "Day " + dayno; 

        var citynm = thisparam.parents("tr").find('.ddlday > option:contains("' + day + '")').attr("cityname");
        var cityid = thisparam.parents("tr").find('.ddlday > option:contains("' + day + '")').attr("cityid");
        if (citynm != null && citynm != "" && citynm != undefined && cityid != null && cityid != "" && cityid != undefined) {
            thisparam.parents("tr").find(".tocityUI").val(citynm);
            thisparam.parents("tr").find(".tocity").val(cityid);
        }
        else {
            thisparam.parents("tr").find(".tocityUI").val("");
            thisparam.parents("tr").find(".tocity").val("");
        }
    }
});
 
/*-------------------------------------populating city automatically on positions on Day Dropdown ends here------------------------------------*/