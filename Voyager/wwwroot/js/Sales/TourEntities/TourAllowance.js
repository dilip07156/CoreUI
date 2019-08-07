$(document).off('click', 'a.cloneRoom');
$(document).off('click', 'a.cloneSupplement');
$(document).off('click', 'a.removeRoom');

$(document).ready(function () {

    //$('a[data-toggle="collapse"]').on('click', function () {
    //     
    //    var objectID = $(this).attr('href');
    //    console.log(objectID);
    //    if ($(objectID).hasClass('in')) {
    //        $(objectID).collapse('hide');
    //    }
    //    else {
    //        $('div[id^=' + objectID + ']').collapse('show');
    //    }
    //});
    $("#TourPriceServices").hide();
    var flag = false;
    $("#TEAllowances [id^=chkDynamic_]").each(function () { 
        if ($(this).attr("data-action") != null && $(this).attr("data-action") != "" && $(this).attr("data-action") != undefined) {           
            flag = true;
            $(this).prop("checked", true);
            $(this).parents("li").addClass("active");
            ShowTourAllowanceToTable($(this).parent("label").text().trim(), $(this).attr("data-action"), $(this).attr("data-prodid"));
        }
    });
    if (flag) {
        $("#TourPriceServices").show();
        $("#TEAllowances .divdynamic").show();
    }
    else {
        $("#TEAllowances .divdynamic").hide();
    }
});

//$(".SavePositionServices").click(function () {
$(document).off('click', '.SavePositionServices');
$(document).on("click", '.SavePositionServices', function (e) {
    $(".ajaxloader").show();
    var cnt = $(this).attr("id").replace("services_", "");
    var posid = $(this).attr("data-positionid");
    var prodid = $(this).attr("data-productid");
    var qrfid = $(this).attr("data-qrfid");

    var curRow = $(this).parents('td');

    var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;
    var RoomDetailsInfo = new Array();
    if (lastseq > 0) {
        for (var i = 0; i < lastseq; i++) {
            var roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
            var roomId = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']").val();
            if ((roomId != '') || ((roomId == '') && (roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0))) {
                RoomDetailsInfo.push({
                    RoomId: roomId,
                    RoomSequence: parseInt(roomSeq),
                    ProductRangeId: curRow.find("select[name*='PositionRoomsData.RoomDetails[" + i + "].ProductRangeID']").val(),
                    ProductRange: curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].ProductRange']").val(),
                    IsSupplement: curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].IsSupplement']").val().toLowerCase() == "true" ? true : false
                });
            }
        }
        var request = { PositionId: posid, ProductId: prodid, QRFId: qrfid, RoomDetailsInfo: RoomDetailsInfo, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };

        $.ajax({
            type: "POST",
            url: "/Position/SetPositionRoomDetails",
            data: request,
            global: false,
            success: function (result) {
                if (result != undefined && result != null && result != "" && result.status != undefined && result.status == "success") {
                    var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;
                    var roomDetailsInfo = result.roomDetailsInfo;
                    var roomobj, roomSeq, roomId, roomidnew = "";

                    if (lastseq > 0 && roomDetailsInfo != null && roomDetailsInfo != undefined && roomDetailsInfo.length > 0) {
                        for (var i = 0; i < lastseq; i++) {
                            roomobj = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']");
                            roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                            roomId = roomobj.val();
                            if (roomId == '' && roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0) {
                                roomidnew = roomDetailsInfo.find(a => a.roomSequence == roomSeq);
                                if (roomidnew != undefined && roomidnew != "" && roomidnew != null) {
                                    roomobj.val(roomidnew.roomId);
                                }
                            }
                        }
                    }
                }
                GetPositionPricesPartView($("#trprice_" + cnt), qrfid, posid, prodid);
            },
            error: function (response) {
                $(".ajaxloader").hide();
            }
        });
    }
});

$(document).on("click", 'a[data-toggle="collapse"]', function (e) {
    var objectID = $(this).attr('href');
    // console.log(objectID);
    // var id = objectID.split("_")[0];

    if ($(objectID).hasClass('in')) {
        $(objectID).collapse('hide');
    }
    else {
        $(objectID).collapse('show');
    }
});

//Static & Dynamic Tour Entities Check box types
$('#chkEntities input[type=checkbox],#chkDynamicEntities input[type=checkbox]').click(function () {
    var chkvalue = $(this).parent("label").text().trim();
    if (chkvalue != "Others") {
        if ($(this).is(":checked")) {
            $(this).parent().parent().addClass('active');
            if ($(this).attr("data-action") != "" && $(this).attr("data-prodid") != "") {
                ShowTourAllowanceToTable(chkvalue, $(this).attr("data-action"), $(this).attr("data-prodid"));
            }
            else {
                $(this).prop("checked", false);
                $(this).parents("li").removeClass("active");
                alert("No details found.");
            }
        }
        else {
            $(this).parent().parent().removeClass('active');
            HideTourAllowanceFromTable(chkvalue, $(this).attr("data-action"));
        }
    }
});

//When checkbox checked then add tour allowances
function ShowTourAllowanceToTable(chkvalue, PositionID, ProductID) { 
    $("#TourPriceServices").show();
    if (PositionID != null && PositionID != undefined && PositionID != "" && ProductID != null && ProductID != undefined && ProductID != "") {
        $(".ajaxloader").show();
        var divService = $("div[data-posid='" + PositionID + "']");
        var cnt = 0;
        if (divService != undefined && divService != null && divService != "" && divService.length == 1) {
            $(divService).show();
            cnt = divService.find("div[class^='panel-body']").attr("class").replace("panel-body_", "");
            if (!$("#collapse_" + cnt).hasClass('in')) {
                $("#collapse_" + cnt).addClass("in");
                $("#collapse_" + cnt).collapse('show');
                $("#collapse_" + cnt).attr('aria-expanded', true);
                $("#collapse_" + cnt).removeAttr('style');
            }
            GetPositionPricesPartView($("#trprice_" + cnt), $("#QRFId").val(), PositionID, ProductID);
            GetPositionRoomDetails(PositionID, ProductID, $("#QRFId").val(), "#trservices_" + cnt, 0)
        }
        else {
            cnt = $("div[class^='panel-body']").length + 1;
            var accor = '<div class="panel panel-default" id="panel_' + cnt + '" data-label="' + chkvalue + '" data-posid="' + PositionID + '">';
            accor += '<div class="panel-heading"><a data-toggle="collapse" data-parent="#accordion" href="#collapse_' + cnt + '"><h4 class="panel-title">' + chkvalue + '</h4></a></div>';
            accor += '<div id="collapse_' + cnt + '" class="panel-collapse collapse in panelaccord"><div class="panel-body_' + cnt + '" >';
            accor += '<table class="table" style="margin-bottom:0px;"><tbody><tr id="tr_' + cnt + '"><td id="trservices_' + cnt + '"></td><td id="trprice_' + cnt + '" class="col-lg-9 trprice"></td></tr></tbody><table></div></div></div>';
            var accord = $("#accordion").append(accor);
            if (accord != null && accord != undefined) {
                GetPositionPricesPartView($("#trprice_" + cnt), $("#QRFId").val(), PositionID, ProductID);
                GetPositionRoomDetails(PositionID, ProductID, $("#QRFId").val(), "#trservices_" + cnt, 0)
            }
        }
    }
}

function HideTourAllowanceFromTable(chkvalue, PositionID) {
    if (PositionID != null && PositionID != undefined && PositionID != "") {
        var divService = $("div[data-posid='" + PositionID + "']");
        $(divService).hide();
    }
}

function GetPositionRoomDetails(PositionID, ProductID, QRFId, divServices, RowNo) {
    $.ajax({
        type: "GET",
        url: "/Position/GetPositionRoomDetails",
        data: { QRFId: QRFId, PositionId: PositionID, ProductId: ProductID, RowNo: RowNo, PositionType: "TourDetails[" },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            //  $(divServices).html(response);
            $(divServices).replaceWith(response);
            var cnt = divServices.replace("#trservices_", "");
            $("#tr_" + cnt + " td.divRoomeDetails:first-child").addClass(divServices.replace("#trservices_", "trservices_"));
            //var buttonText = '<input type="button" id="services_' + cnt + '" class="btn btn-md btn-blue btn-save SavePositionServices"';
            //buttonText.appendTo(".trservices_1");
            if ($('#services_' + cnt).length == 0) {
                $('</br>').appendTo('.trservices_' + cnt)
                $('<input>').attr({ 'data-positionid': PositionID, 'data-productid': ProductID, 'data-qrfid': QRFId, type: 'button', id: 'services_' + cnt, class: 'btn btn-md btn-blue btn-save SavePositionServices', value: 'Save Services' }).appendTo('.trservices_' + cnt);
            }            
        },
        error: function (response) {
            alert(response.statusText);
        }
    });
} 