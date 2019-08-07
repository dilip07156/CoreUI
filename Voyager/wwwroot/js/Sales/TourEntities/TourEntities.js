$(document).ready(function () {
    $("#tblTourEntitiesMain").css("width", "45%");
    var lstPositionid = "";
    $("#TETourEntity [class=chkDynamic]").prop("checked", true);
    $("#TETourEntity [class=chkDynamic]").parents("li").addClass("active");

    $("#TETourEntity [id^=chkDynamic_]:checked").each(function () {
        if ($(this).attr("data-action") != null && $(this).attr("data-action") != "" && $(this).attr("data-action") != undefined) {
            lstPositionid += $(this).attr("data-action") + ",";
            $(this).prop("checked", true);
            $(this).parents("li").addClass("active");
        }
    });
    lstPositionid = lstPositionid.slice(0, -1);

    if (lstPositionid != "" && lstPositionid != null && lstPositionid != undefined) {
        $("#TETourEntity .divdynamic").show();
        $("#trTourHeader").find("th").remove();
        $("#trTourSubHeader").find("th").remove();
        $("tr[class^=trPaxSlab]").remove();
        $("#tblTourEntitiesMain").find(".trTourHeaderPS").remove();
        AddTourSectionToTable("", lstPositionid);
    }
    else {
        $(".ajaxloader").hide();
        $("#TETourEntity .divdynamic").hide();
    }

    $(".btnsaveTourEntity").click(function (e) {
        SaveTourEntities();
    });
});

//Message close button 
$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});

/*------------Others Checkbox click event starts here------------------*/
$(document).on('click', function (e) {
    // if ($('.mlti-lst').find("li.active").length > 0)
    if ($('.mlti-lst').find("input[type=checkbox]:checked").length > 0) {
        $("#others").prop("checked", true);
    }
    else {
        $("#others").prop("checked", false);
    }
    $(".mlti-lst").hide();
    $(".other-chk").removeClass("active");
});

$('.other-chk').on('change', function (e) {
    e.stopPropagation();
    $('.mlti-lst').show();
    $('.othrs-fld').hide();
    $('.othrs-fld').val('');
});

$('#others-fld').on('change', function (e) {
    e.stopPropagation();
    $('.mlti-lst').show();
    $(".mlti-lst").animate({ scrollTop: $(document).height() }, "slow");
    if ($(this).is(":checked")) {
        $('.othrs-fld').show();
    }
    else {
        $('.othrs-fld').hide();
    }
});
/*------------Others Checkbox click event ends here------------------*/

//Static & Dynamic Tour Entities Check box types
$('#chkEntities input[type=checkbox],#chkDynamicEntities input[type=checkbox]').click(function () {
    var chkvalue = $(this).parent("label").text().trim();
    var posname = $(this).attr("data-name").trim();

    if (chkvalue != "Others") {
        if ($(this).is(":checked")) {
            $(this).parent().parent().addClass('active');
            posid = $(this).attr("data-action") != null && $(this).attr("data-action") != undefined && $(this).attr("data-action") != "" ? $(this).attr("data-action") : "";
            AddTourSectionToTable(posname, posid);
        }
        else {
            $(this).parent().parent().removeClass('active');
            RemoveTourSectionFromTable(posname);
        }
    }
});

//When CheckBox uncheck then remove it from page
function RemoveTourSectionFromTable(posname) {
    //$("#tblTourEntitiesMain").find("[data-label='" + chkvalue + "']").css("display", "none");    
    $("#tblTourEntitiesMain").find("[data-label='" + posname + "']").remove();
    if ($("#trTourHeader").find("th").length == 1) {
        $("#tblTourEntitiesMain").find(".trTourHeaderPS").remove();
        $("tr[class^=trPaxSlab]").remove();
    }
}

//When checkbox checked then add tour Type
function AddTourSectionToTable(chkvalue, PositionID = "") {
    var posname = "";
    if (chkvalue != null && chkvalue != null && chkvalue != undefined) {
        posname = chkvalue.split('_')[0];
    }
    var tourEntitiesParam = { QRFID: $("#QRFId").val(), Type: posname, PositionID: PositionID };
    $.ajax({
        type: "GET",
        url: "/TourEntities/GetTourEntities",
        data: tourEntitiesParam,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result != undefined && result != null && result != "" && result.paxSlabs != null && result.paxSlabs != "" && result.paxSlabs.length > 0) {
                var trTourHeader, trTourSubHeader, trTourSubSubHeader, trPaxSlab = "";
                var datalabel = '';
                var ddloptions = "";
                var cnt = 1;
                //plax slab starts here
                if ($("#tblTourEntitiesMain").find(".trTourHeaderPS").length == 0) {
                    var paxSlabs = result.paxSlabs;
                    trTourHeader = '<th style="width:1%;" rowspan="3" class="valign-top trTourHeaderPS"><span>Pax Slabs</span></th>';
                    $("#trTourHeader").append(trTourHeader);

                    for (var i = 0; i < paxSlabs.length; i++) {
                        var paxslab = paxSlabs[i].from + " - " + paxSlabs[i].to;

                        trPaxSlab = '<tr class="trPaxSlab_' + i + '" data-label="' + paxslab + '"><td data-label="Pax Slabs" class="trTourHeaderPS"><label class="form-control" id="lblPS_' + i + '" >' + paxslab + '</label><input type="hidden" value="' + paxSlabs[i].paxSlab_Id + '" id="hfPSID_' + i + '" /></td></tr>';
                        $(".tbodyPaxSlab").append(trPaxSlab);
                    }
                }
                //plax slab ends here
                ddloptions += '<option value="">Select</option>';
                if (result.roomList != null && result.roomList != "" && result.roomList.length > 0) {
                    for (var i = 0; i < result.roomList.length; i++) {
                        ddloptions += '<option value="' + result.roomList[i].value + '">' + result.roomList[i].label + '</option>';
                    }
                }

                if (result.tourEntity != null && result.tourEntity != "" && result.tourEntity.length > 0) { 
                    var tourentity = result.tourEntity;
                    var tourentitycnt = result.tourEntityCnt;
                    var tourresult = new Array();
                    var paxslabhm = "";
                    var paxslabstarttd = "";
                    var paxslabendtd = "</td>";
                    var howmany, stayingin, lunchincluded, dinnerincluded, removebtn = "";
                    var k = 0;
                    var curtd, curtr = "";
                    var duration = "";

                    for (var i = 0; i < tourentitycnt.length; i++) {
                        k = 0;
                        howmany = "";
                        stayingin = "";
                        lunchincluded = "";
                        dinnerincluded = "";
                        removebtn = "";
                        var datanamePos = $("input:checkbox[data-action='" + tourentitycnt[i].positionID + "']").attr("data-name");
                        $("input:checkbox[data-name='" + datanamePos + "']").prop("checked", true);
                        $("input:checkbox[data-name='" + datanamePos + "']").parents("li").addClass("active");

                        curtr = $("tr[data-label='" + tourentitycnt[i].paxSlab + "']");
                        curtd = $("td[data-label='" + datanamePos + "']");

                        datalabel = 'data-label="' + datanamePos + '"';
                        paxslabhm = '<td ' + datalabel + ' class="light-yellow-bg clshm">';
                        paxslabstarttd = '<td ' + datalabel + ' class="light-yellow-bg">';

                        if ($("th[data-label='" + datanamePos + "']").length == 0) {
                            //top header (Tour Type)
                            trTourHeader = '<th style="width: 15%;" colspan="6" ' + datalabel + '><span>' + tourentitycnt[i].type + '</span></th>';
                            $("#trTourHeader").append(trTourHeader);

                            //top Sub header (How many ,staying in)
                            trTourSubHeader = '<th rowspan="2" class="light-yellow-bg" ' + datalabel + '>How Many?</th><th rowspan="2" ' + datalabel + ' class="light-yellow-bg"> Staying in</th><th colspan="2" ' + datalabel + ' class="light-yellow-bg">Meal Included</th><th rowspan="2" colspan="2" ' + datalabel + ' class="light-yellow-bg">Action</th>';
                            $("#trTourSubHeader").append(trTourSubHeader);

                            //top Sub Subheader (All Checkbox for Lunch,Dinner)
                            trTourSubSubHeader = '<th ' + datalabel + ' class="light-yellow-bg"><label id="lunchAll_' + datanamePos + '" style="border-bottom:0px;" class="lunchall form-control no-bg"><input id="chkLunchAll_' + datanamePos + '" name="Lunch" type="checkbox"/> All</label></th>';
                            trTourSubSubHeader += '<th ' + datalabel + ' class="light-yellow-bg"><label id="dinnerAll_' + datanamePos + '" style="border-bottom:0px;" class="dinnerall form-control no-bg"><input id="chkDinnerAll_' + datanamePos + '" name="Dinner" type="checkbox"/> All</label></th>';
                            $("#trTourSubSubHeader").append(trTourSubSubHeader);
                        }

                        tourresult = $.grep(tourentity, function (element) { 
                            return (element.paxSlab == tourentitycnt[i].paxSlab && element.positionID == tourentitycnt[i].positionID);
                        });

                        if (tourresult != null && tourresult != "" && tourresult.length > 0) {

                            for (var j = 0; j < tourresult.length; j++) {
                                k = j + 1;
                                if (j > 0) {
                                    howmany += '<hr/>';
                                    stayingin += '<hr/>';
                                    dinnerincluded += '<hr/>';
                                    lunchincluded += '<hr/>';
                                    removebtn += '<hr/>';
                                }
                                var duration = $("input:checkbox[data-action='" + tourresult[j].positionId + "']").attr("data-dur");
                                duration = (duration == null || duration == "" || duration == "0" || duration == "1") ? "0" : "1";

                                //how many td starts here 
                                tourresult[j].howMany = (tourresult[j].howMany == null || tourresult[j].howMany == "") ? duration : tourresult[j].howMany;
                                howmany += '<input type="text" maxlength="3" class="form-control no-bg numeric" value="' + tourresult[j].howMany + '" id="txtHowMany_' + k + '"/>';

                                //staing in td starts here                                 
                                stayingin += '<input class="hfstatus" type="hidden" value="' + tourresult[j].isDeleted + '" id="hfStatus_' + k + '" /><input class="hfTEID" type="hidden" value="' + tourresult[j].tourEntityID + '" id="hfTourEntityID_' + k + '" /><select class="form-control no-bg" id="ddlRoomType_' + k + '" >' + ddloptions + '</select>';

                                //Meal Included dinner lunch td starts here
                                if (tourresult[j].isLunch) {
                                    lunchincluded += '<label id="lunch_' + k + '" style="border-bottom:0px;" class="lunch form-control no-bg"><input id="chkLunch_' + k + '" name="Lunch" type="checkbox" checked="true"/> Lunch</label>';
                                }
                                else {
                                    lunchincluded += '<label id="lunch_' + k + '" style="border-bottom:0px;" class="lunch form-control no-bg"><input id="chkLunch_' + k + '" name="Lunch" type="checkbox"/> Lunch</label>';
                                }

                                if (tourresult[j].isDinner) {
                                    dinnerincluded += '<label id="dinner_' + k + '" style="border-bottom:0px;" class="dinner form-control no-bg"><input id="chkDinner_' + k + '"  name="Dinner" type="checkbox" checked="true" /> Dinner</label>';
                                }
                                else {
                                    dinnerincluded += '<label id="dinner_' + k + '"  style="border-bottom:0px;" class="dinner form-control no-bg"><input id="chkDinner_' + k + '"  name="Dinner" type="checkbox" /> Dinner</label>';
                                }

                                removebtn += '<a id="aRemove_' + k + '" class="form-control icon-squre-red pointercursor"><i class="fa fa-times"></i></a>';
                            }

                            trPaxSlab = paxslabhm + howmany + paxslabendtd; //how many
                            trPaxSlab += paxslabstarttd + stayingin + paxslabendtd;//staying in                            
                            trPaxSlab += paxslabstarttd + lunchincluded + paxslabendtd;//meal included
                            trPaxSlab += paxslabstarttd + dinnerincluded + paxslabendtd;//meal included
                            trPaxSlab += paxslabstarttd + removebtn + paxslabendtd;//remove btn
                            trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><a class="form-control icon-squre-green pointercursor"><i class="fa fa-plus"></i></a></td>';

                            curtr.append(trPaxSlab);

                            for (var j = 0; j < tourresult.length; j++) {
                                k = j + 1;
                                if (tourresult[j].roomTypeID != null && tourresult[j].roomTypeID != "") {
                                    curtr.find("td[data-label='" + datanamePos + "']").find("#ddlRoomType_" + k).val(tourresult[j].roomTypeID);
                                }
                                else {
                                    var ddlval = curtr.find("td[data-label='" + datanamePos + "']").find("#ddlRoomType_" + k + " option:contains(Single)").val();
                                    curtr.find("td[data-label='" + datanamePos + "']").find("#ddlRoomType_" + k).val(ddlval);
                                }
                            }

                            /*------------------All Checkbox selection code for Lunch and Dinner starts here------------------*/
                            var totalLunchCntChk = $(".tbodyPaxSlab tr").find("td[data-label='" + datanamePos + "']").find("input[id^=chkLunch_]").length;
                            var totalLunchCntChked = $(".tbodyPaxSlab tr").find("td[data-label='" + datanamePos + "']").find("input[id^=chkLunch_]:checked").length;
                            $("#trTourSubSubHeader").find("th[data-label='" + datanamePos + "']").find("input[id^=chkLunchAll]").prop("checked", totalLunchCntChk == totalLunchCntChked);

                            var totalDinnerCntChk = $(".tbodyPaxSlab tr").find("td[data-label='" + datanamePos + "']").find("input[id^=chkDinner_]").length;
                            var totalDinnerCntChked = $(".tbodyPaxSlab tr").find("td[data-label='" + datanamePos + "']").find("input[id^=chkDinner_]:checked").length;
                            $("#trTourSubSubHeader").find("th[data-label='" + datanamePos + "']").find("input[id^=chkDinnerAll]").prop("checked", totalDinnerCntChk == totalDinnerCntChked);
                            /*------------------All Checkbox selection code for Lunch and Dinner end here------------------*/
                        }
                    }
                }
                else if (chkvalue != "" && chkvalue != null && chkvalue != undefined) {
                    AddTourSection(chkvalue, datalabel, ddloptions);
                }

                if (chkvalue == "" && result.positionNotExists != null && result.positionNotExists != "" && result.positionNotExists.length > 0) {
                    var positionnm = "";
                    for (var i = 0; i < result.positionNotExists.length; i++) {
                        if (result.positionNotExists[i] != "" && result.positionNotExists[i] != null) {
                            positionnm = $("input:checkbox[data-action='" + result.positionNotExists[i] + "']").attr("data-name");
                            datalabel = 'data-label="' + positionnm + '"';
                            AddTourSection(positionnm, datalabel, ddloptions);
                        }
                    }
                }

                if ($('.mlti-lst').find("input[type=checkbox]:checked").length > 0)
                    $("#others").prop("checked", true);
                else
                    $("#others").prop("checked", false);
            }
        },
        error: function (error) {
            alert(error);
        }
    });
}

function AddTourSection(chkvalue, datalabel, ddloptions) {
    var positionName = chkvalue.split('_')[0];

    var trTourHeader, trTourSubHeader, trPaxSlab = "";
    var datalabel = 'data-label="' + chkvalue + '"';
    //top header (Tour Type)
    trTourHeader = '<th style="width: 15%;" colspan="6" ' + datalabel + '><span>' + positionName + '</span></th>';
    $("#trTourHeader").append(trTourHeader);

    //top Sub header (How many ,staying in)
    trTourSubHeader = '<th rowspan="2" class="light-yellow-bg" ' + datalabel + '>How Many?</th><th rowspan="2" ' + datalabel + ' class="light-yellow-bg"> Staying in</th><th colspan="2" ' + datalabel + ' class="light-yellow-bg">Meal Included</th><th rowspan="2" colspan="2" ' + datalabel + ' class="light-yellow-bg">Action</th>';
    $("#trTourSubHeader").append(trTourSubHeader);

    //top Sub Subheader (All Checkbox for Lunch,Dinner)
    trTourSubSubHeader = '<th ' + datalabel + ' class="light-yellow-bg"><label id="lunchAll_' + chkvalue + '" style="border-bottom:0px;" class="lunchall form-control no-bg"><input id="chkLunchAll_' + chkvalue + '" name="Lunch" type="checkbox"/> All</label></th>';
    trTourSubSubHeader += '<th ' + datalabel + ' class="light-yellow-bg"><label id="dinnerAll_' + chkvalue + '" style="border-bottom:0px;" class="dinnerall form-control no-bg"><input id="chkDinnerAll_' + chkvalue + '" name="Dinner" type="checkbox"/> All</label></th>';
    $("#trTourSubSubHeader").append(trTourSubSubHeader);

    var duration = $("input:checkbox[data-name='" + chkvalue + "']").attr("data-dur");
    duration = (duration == null || duration == "" || duration == "0" || duration == "1") ? "0" : "1";

    //td starts here
    trPaxSlab = '<td ' + datalabel + ' class="light-yellow-bg clshm"><input type="text" maxlength="3" class="form-control no-bg numeric" value=' + duration + ' id="txtHowMany_1"/></td>';
    trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><input class="hfstatus" type="hidden" value="false" id="hfStatus_1" /><input class="hfTEID" type="hidden" value="0" id="hfTourEntityID_1" />';
    trPaxSlab += '<select class="form-control no-bg" id="ddlRoomType_1" >' + ddloptions + '</select></td >';
    trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><label id="lunch_1" style="border-bottom:0px;" class="lunch form-control no-bg"><input id="chkLunch_1" name="Lunch" type="checkbox" /> Lunch</label></td>';
    trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><label id="dinner_1" style="border-bottom:0px;" class="dinner form-control no-bg"><input id="chkDinner_1" name="Dinner" type="checkbox" /> Dinner</label></td>';
    trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><a id="aRemove_1" class="form-control icon-squre-red pointercursor"><i class="fa fa-times"></i></a></td>';
    trPaxSlab += '<td ' + datalabel + ' class="light-yellow-bg"><a class="form-control icon-squre-green pointercursor"><i class="fa fa-plus"></i></a></td>';

    $("tr[class^=trPaxSlab]").append(trPaxSlab);
    var ddlval = $("select[id^='ddlRoomType_'] option:contains(Single)").val();
    if (ddlval != null && ddlval != "" && ddlval != undefined) {
        $("td[data-label='" + chkvalue + "']").find("select[id^='ddlRoomType_']").val(ddlval);
    }
}

//Lunch Checkbox All 
$(document).off('click', 'input[id^=chkLunchAll]');
$(document).on('click', 'input[id^=chkLunchAll]', function (e) {
    var chkValue = $(this).parent("label").attr("id").replace("lunchAll_", "");
    var posName = chkValue.split('_')[0];

    if (posName != undefined && posName != "" && posName != null) {
        var chkLunch = $(".tbodyPaxSlab tr td[data-label='" + chkValue + "']").find("input[id^=chkLunch_]");
        if ($(this).is(":checked")) {
            chkLunch.prop("checked", true);
        }
        else {
            chkLunch.prop("checked", false);
        }
    }
});

//Lunch:- on Single Checkbox Event
$(document).off('click', 'input[id^=chkLunch_]');
$(document).on('click', 'input[id^=chkLunch_]', function (e) {
    var chkValue = $(this).parents("td").attr("data-label");
    var posName = chkValue.split('_')[0];
    var totalLunchCntChk = $(".tbodyPaxSlab tr").find("td[data-label='" + chkValue + "']").find("input[id^=chkLunch_]").length;
    var totalLunchCntChked = 0;

    if (posName != undefined && posName != "" && posName != null) { 

        totalLunchCntChked = $(".tbodyPaxSlab tr").find("td[data-label='" + chkValue + "']").find("input[id^=chkLunch_]:checked").length; 
        $("#trTourSubSubHeader").find("th[data-label='" + chkValue + "']").find("input[id^=chkLunchAll]").prop("checked", totalLunchCntChk == totalLunchCntChked);
    }
});

//Dinner Checkbox All 
$(document).off('click', 'input[id^=chkDinnerAll]');
$(document).on('click', 'input[id^=chkDinnerAll]', function (e) {
    var chkValue = $(this).parent("label").attr("id").replace("dinnerAll_", "");
    var posName = chkValue.split('_')[0];

    if (posName != undefined && posName != "" && posName != null) {
        if ($(this).is(":checked")) {
            $(".tbodyPaxSlab tr td[data-label='" + chkValue + "']").find("input[id^=chkDinner_]").prop("checked", true);
        }
        else {
            $(".tbodyPaxSlab tr td[data-label='" + chkValue + "']").find("input[id^=chkDinner_]").prop("checked", false);
        }
    }
});

//Dinner:- on Single Checkbox Event
$(document).off('click', 'input[id^=chkDinner_]');
$(document).on('click', 'input[id^=chkDinner_]', function (e) {
    var chkValue = $(this).parents("td").attr("data-label");
    var posName = chkValue.split('_')[0];
    var totalDinnerCntChk = $(".tbodyPaxSlab tr").find("td[data-label='" + chkValue + "']").find("input[id^=chkDinner_]").length;
    var totalDinnerCntChked = 0;

    if (posName != undefined && posName != "" && posName != null) {
        totalDinnerCntChked = $(".tbodyPaxSlab tr").find("td[data-label='" + chkValue + "']").find("input[id^=chkDinner_]:checked").length;
        $("#trTourSubSubHeader").find("th[data-label='" + chkValue + "']").find("input[id^=chkDinnerAll]").prop("checked", totalDinnerCntChk == totalDinnerCntChked);
    }
});

/*-----------------------save button starts here-----------------------------*/
function SaveTourEntities() {
    $("#divSuccessError").hide();
    var flag = false;
    $(".ajaxloader").show();
    //$("#tblTourEntitiesMain").find(".numeric:not([style*='display: none;'])").each(function () {
    //    if ($(this).val() == "" || $(this).val() == "0") {
    //        flag = true;
    //    }
    //});

    //if (flag) {
    //    $(".ajaxloader").show();
    //    alert("How Many Number Of Staff field can not be Zero / Blank.");
    //    return false;
    //}
    var tourentity = new Array();
    var paxslab, paxslabid, positionid, curtourtype, curtr, tourentityid, status, howmany, dataflag = "";
    $(".tbodyPaxSlab").find("tr").each(function () {
        curtr = $(this);
        paxslab = curtr.find("[id^=lblPS_]").text();
        paxslabid = curtr.find("[id^=hfPSID_]").val();
        positionid = "";
        curtourtype = "";
        var roomtypeid = "";

        $(this).find("td.clshm").each(function () {
            var tourtype = $(this).attr("data-label");
            positionid = $("input:checkbox[data-name='" + tourtype + "']").attr("data-action");
            dataflag = $("input:checkbox[data-name='" + tourtype + "']").attr("data-flag");
            curtourtype = curtr.find("td[data-label='" + tourtype + "']");
            for (var i = 1; i <= $(this).find(".no-bg").length; i++) {

                roomtypeid = curtourtype.find("#ddlRoomType_" + i).val();
                tourentityid = curtourtype.find("#hfTourEntityID_" + i).val();
                status = curtourtype.find("#hfStatus_" + i).val();
                positionid = (positionid == undefined || positionid == "" || positionid == null) ? "" : positionid;
                tourentityid = tourentityid == null || tourentityid == undefined || tourentityid == "" ? "0" : tourentityid;
                howmany = curtourtype.find("#txtHowMany_" + i).val();

                if ((tourentityid != "0" && (status == "false" || status == "true")) || (tourentityid == "0" && status == "false") && roomtypeid != "") {
                    tourentity.push({
                        PersonType: "",
                        PositionID: positionid,
                        Flag: dataflag,
                        IsDeleted: status,
                        RoomTypeID: roomtypeid,
                        RoomType: curtourtype.find("#ddlRoomType_" + i + " option[value='" + roomtypeid + "']").text(),
                        IsDinner: curtourtype.find("#chkDinner_" + i).is(":checked") ? "true" : "false",
                        IsLunch: curtourtype.find("#chkLunch_" + i).is(":checked") ? "true" : "false",
                        PaxSlab: paxslab,
                        PaxSlabID: paxslabid,
                        Type: tourtype.split('_')[0],
                        TourEntityID: tourentityid,
                        HowMany: howmany
                    });
                }
            }
        });
    });

    if (tourentity != null && tourentity != undefined && tourentity.length > 0 && $("#QRFId").val() > 0) {
        //console.log(tourentity);
        //var lunchdinnerte = $.grep(tourentity, function (element) {
        //    return (element.IsDinner == "false" && element.IsLunch == "false" && (element.HowMany == "0" || element.HowMany == ""));
        //});

        //console.log(lunchdinnerte);
        //if (lunchdinnerte != undefined && lunchdinnerte != null && lunchdinnerte.length > 0) {
        //    $(".ajaxloader").hide();
        //    alert("How Many Number Of Staff field can not be Zero / Blank.");
        //    return false;
        //}

        tourEntitiesParam = { QRFID: $("#QRFId").val(), TourEntities: tourentity, __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val() };

        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/TourEntities/TourEntities", // Controller/View
            data: tourEntitiesParam,
            global: false,
            success: function (result) {
                // $("#divSuccessError").show();               
                if (result != null && result != "") {
                    //if (result.responseStatus.status == "Success") {
                    //    $("#divSuccessError").addClass("alert alert-success");
                    //    $("#lblmsg").text(result.responseStatus.errorMessage);
                    //    $("#stMsg").text("Success!");
                    //}
                    //else {
                    //    $("#divSuccessError").addClass("alert alert-danger");
                    //    $("#lblmsg").text(result.responseStatus.errorMessage);
                    //    $("#stMsg").text("Error!");
                    //}
                    result.responseStatus.status = result.responseStatus.status == "Success" ? result.responseStatus.status : "Error";
                    GetTourMenus(result.responseStatus.errorMessage, result.responseStatus.status);
                }
                else {
                    //$("#divSuccessError").addClass("alert alert-danger");
                    //$("#lblmsg").text("Details Not Saved.");
                    //$("#stMsg").text("Error!");
                    GetTourMenus("Details Not Saved.", "Error");
                }
            },
            error: function (jqXHR, exception, errorThrown) {
                GetTourMenus();
                var msg = "";
                $("#divSuccessError").show();
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status == 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status == 500) {
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
                $("#divSuccessError").addClass("alert alert-danger");
                $("#lblmsg").text(msg);
                $("#stMsg").text("Error!");
                $(".ajaxloader").hide();
            }
        });
    }
    else {
        $(".ajaxloader").hide();
        alert('No details found for submission.');
    }
}
/*-----------------------save button ends here-----------------------------*/

function GetTourMenus(msg = "", status = "") {
    $("#divSuccessError").hide();
    $.ajax({
        type: "GET",
        url: "/TourEntities/GetTourEntities",
        data: { QRFId: $("#QRFId").val(), TourType: "TE" },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        global: false,
        success: function (response) {
            $('#TETourEntity').html(response);
            if (status != "" && msg != "" && status != undefined && msg != undefined) {
                $("#divSuccessError").show();
                if (status.toLocaleLowerCase() == "success") {
                    $("#divSuccessError").addClass("alert alert-success");
                    $("#lblmsg").text(msg);
                    $("#stMsg").text("Success!");
                }
                else {
                    $("#divSuccessError").addClass("alert alert-danger");
                    $("#lblmsg").text(msg);
                    $("#stMsg").text("Error!");
                }
            }
        },
        error: function (response) {
            if (status != "" && msg != "" && status != undefined && msg != undefined) {
                $("#divSuccessError").show();
                if (status.toLocaleLowerCase() != "success") {
                    $("#divSuccessError").addClass("alert alert-danger");
                    $("#lblmsg").text(msg);
                    $("#stMsg").text("Error!");
                }
            }
            $(".ajaxloader").hide();
            alert(response.statusText);
        }
    });
}

/*-----------------------on delete button starts here------------------------------------*/
$(document).off('click', '.icon-squre-red');
$(document).on('click', '.icon-squre-red', function (e) {
    //$('.icon-squre-red').click(function (e) {
    var thisparam = $(this);
    var curtr = thisparam.parents("tr");
    var curtd = thisparam.parent("td");
    if (curtd.find(".pointercursor:visible").length == 1) {
        alert('Last row can not be delete.');
    }
    else {
        var curdatalabel = curtd.attr("data-label");
        var curid = thisparam.attr("id");
        var curindex = curid.replace("aRemove_", "");
        var tddatalabel = curtr.find("td[data-label='" + curdatalabel + "']");

        var curhm = tddatalabel.find("#txtHowMany_" + curindex);
        var curtid = tddatalabel.find("#hfTourEntityID_" + curindex);
        var curroom = tddatalabel.find("#ddlRoomType_" + curindex);
        var curdinner = tddatalabel.find("#dinner_" + curindex);
        var curlunch = tddatalabel.find("#lunch_" + curindex);
        var curremove = tddatalabel.find("#aRemove_" + curindex);
        var curstatus = tddatalabel.find("#hfStatus_" + curindex);

        curhm.hide();
        curtid.hide();
        curroom.hide();
        curdinner.hide();
        curlunch.hide();
        curremove.hide();
        curstatus.val('true');
        curstatus.hide();

        curhm.next("hr").hide();
        curtid.next("hr").hide();
        curroom.next("hr").hide();
        curdinner.next("hr").hide();
        curlunch.next("hr").hide();
        curremove.next("hr").hide();
        curstatus.next("hr").hide();

        if (curtd.find(".pointercursor:visible").length == 1) {
            curhm.prevAll("hr").hide();
            curtid.prevAll("hr").hide();
            curroom.prevAll("hr").hide();
            curdinner.prevAll("hr").hide();
            curlunch.prevAll("hr").hide();
            curremove.prevAll("hr").hide();
            curstatus.prevAll("hr").hide();

            curhm.nextAll("hr").hide();
            curtid.nextAll("hr").hide();
            curroom.nextAll("hr").hide();
            curdinner.nextAll("hr").hide();
            curlunch.nextAll("hr").hide();
            curremove.nextAll("hr").hide();
            curstatus.nextAll("hr").hide();
        }
    }
});
/*-----------------------on delete button ends here------------------------------------*/

//When click on Add button 
$(document).off('click', '.icon-squre-green');
$(document).on('click', '.icon-squre-green', function (e) { 
    var hrlast = "";
    var hrfirst = "";

    //Remove Button
    var prevtdremove = $(this).parent("td").prev("td");
    hrlast = prevtdremove.find(".pointercursor:not([style*='display: none;'])").last();
    hrfirst = prevtdremove.find(".pointercursor:not([style*='display: none;'])").first();
    hrlast.next("hr").hide();
    hrfirst.prev("hr").hide();
    var remove = prevtdremove.find(".pointercursor:last").clone().get(0);
    var index = parseInt(remove.id.replace("aRemove_", ""));
    remove.id = "aRemove_" + (index + 1);
    remove.style.display = null;
    prevtdremove.append("<hr/>");
    prevtdremove.append(remove);

    //Dinner CheckBox
    var prevtddinner = $(this).parent("td").prev("td").prev("td");
    hrlast = prevtddinner.find(".dinner:not([style*='display: none;'])").last();
    hrfirst = prevtddinner.find(".dinner:not([style*='display: none;'])").first();
    hrlast.next("hr").hide();
    hrfirst.prev("hr").hide();
    var dinner = prevtddinner.find(".dinner:last").clone().get(0);
    var index = parseInt(dinner.id.replace("dinner_", ""));
    dinner.id = "dinner_" + (index + 1);
    dinner.children[0].id = "chkDinner_" + (index + 1);
    dinner.children[0].checked = false;
    dinner.style.display = null;
    prevtddinner.append("<hr/>");
    prevtddinner.append(dinner);

    //Lunch CheckBox
    var prevtdlunch = $(this).parent("td").prev("td").prev("td").prev("td");
    hrlast = prevtdlunch.find(".lunch:not([style*='display: none;'])").last();
    hrfirst = prevtdlunch.find(".lunch:not([style*='display: none;'])").first();
    hrlast.next("hr").hide();
    hrfirst.prev("hr").hide();
    var lunch = prevtdlunch.find(".lunch:last").clone().get(0);
    var index = parseInt(lunch.id.replace("lunch_", ""));
    lunch.id = "lunch_" + (index + 1);
    lunch.children[0].id = "chkLunch_" + (index + 1);
    lunch.children[0].checked = false;
    lunch.style.display = null;
    prevtdlunch.append("<hr/>");
    prevtdlunch.append(lunch);

    //Room Dropdown
    var prevtdroom = $(this).parent("td").prev("td").prev("td").prev("td").prev("td");
    hrlast = prevtdroom.find(".no-bg:not([style*='display: none;'])").last();
    hrfirst = prevtdroom.find(".no-bg:not([style*='display: none;'])").first();
    hrlast.next("hr").hide();
    hrfirst.prev("hr").hide();
    var ddl = prevtdroom.find(".no-bg:last").clone().get(0);
    var hftourentityid = prevtdroom.find(".hfTEID:last").clone().get(0);
    var hfstatus = prevtdroom.find(".hfstatus:last").clone().get(0);
    var index = parseInt(ddl.id.replace("ddlRoomType_", ""));
    ddl.id = "ddlRoomType_" + (index + 1);
    hftourentityid.id = "hfTourEntityID_" + (index + 1);
    hfstatus.id = "hfStatus_" + (index + 1);
    hftourentityid.value = "0";
    hfstatus.style.display = null;
    hfstatus.value = "false";
    hftourentityid.style.display = null;
    ddl.style.display = null;
    prevtdroom.append("<hr/>");
    prevtdroom.append(hfstatus);
    prevtdroom.append(hftourentityid);
    prevtdroom.append(ddl);

    //How Many textbox
    var prevtdhowmany = $(this).parent("td").prev("td").prev("td").prev("td").prev("td").prev("td");
    hrlast = prevtdhowmany.find(".no-bg:not([style*='display: none;'])").last();
    hrfirst = prevtdhowmany.find(".no-bg:not([style*='display: none;'])").first();
    hrlast.next("hr").hide();
    hrfirst.prev("hr").hide();
    var txt = prevtdhowmany.find(".no-bg:last").clone().get(0);
    var index = parseInt(txt.id.replace("txtHowMany_", ""));
    txt.id = "txtHowMany_" + (index + 1);
    txt.value = "0";
    txt.style.display = null;
    prevtdhowmany.append("<hr/>");
    prevtdhowmany.append(txt);
});