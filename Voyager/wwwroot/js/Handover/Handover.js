setInterval(refreshDepartures, 10000);//10 sec
var depids = new Array();
refreshDepartures();

//the below function will call every 10 sec to fetch the Departuredetails from mGoAhead collection with BookingNo and its status
function refreshDepartures() {
    var depatureInfoGetReq = { //Passing data
        QRFID: $("#QRFId").val(),
        DepatureId: depids
    };
    if (($("#tbodydepartures").find("input[id^=chkIsChecked_]:not(:disabled):not(:checked)").length > 0)) {
    }
    else {
        $("#btnbooking").attr("disabled", "disabled");
    }

    $(".ajaxloader").hide();
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/Handover/GetGoAheadDeparturesDetails", // Controller/View
        data: { depatureInfoGetReq: depatureInfoGetReq },
        global: false,
        success: function (response) {
            if (response != null && response.depatures != null && response.depatures.length > 0) {
                var tbody = "";
                var dept = response.depatures;
                var dt, dtnew = "";
                var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

                var isProcessCnt = dept.filter(a => (a.isShowProcessing == true || a.isShowPending == true) && (a.opsBookingNumber == "" || a.opsBookingNumber == null));

                if (isProcessCnt.length == 0 && ($("#tbodydepartures").find("input[id^=chkIsChecked_]:not(:disabled)").length > 0)) {
                    depids = new Array();
                    $("#btnbooking").removeAttr("disabled");
                }
                for (var i = 0; i < dept.length; i++) {
                    if (jQuery.inArray(dept[i].depatureId.toString(), depids) !== -1
                        && ((dept[i].opsBookingNumber != "" && dept[i].opsBookingNumber != null && dept[i].isCreate && dept[i].confirmMessage.toLowerCase() == "success") || ((dept[i].isShowProcessing == true || dept[i].isShowPending == true) && (dept[i].opsBookingNumber == "" || dept[i].opsBookingNumber == null)))) {
                        tbody += '<tr class="trDeparture">';
                    }
                    else {
                        tbody += '<tr>';
                    }
                    tbody += '<td><input data-val="true" data-val-required="The DepatureId field is required." id="hdDepatureId_' + i + '" name="DepatureInfo[' + i + '].DepatureId" value="' + dept[i].depatureId + '" type="hidden"/>';
                    if (dept[i].opsBookingNumber != "" && dept[i].opsBookingNumber != null && dept[i].isCreate && dept[i].confirmMessage.toLowerCase() == "success") {
                        tbody += '<input value="true" data-name="' + dept[i].depatureId + '"  data-val="true" data-val-required="The IsDeleted field is required." id="chkIsChecked_' + i + '" name="DepatureInfo[' + i + '].IsDeleted" type="checkbox" disabled="disabled"/></td>';
                    }
                    else {
                        if ((dept[i].isShowProcessing == true || dept[i].isShowPending == true) && (dept[i].opsBookingNumber == "" || dept[i].opsBookingNumber == null)) {
                            tbody += '<input value="true" data-name="' + dept[i].depatureId + '" data-val="true" data-val-required="The IsDeleted field is required." id="chkIsChecked_' + i + '" name="DepatureInfo[' + i + '].IsDeleted" type="checkbox" checked="checked"/></td>';
                        }
                        else {
                            tbody += '<input value="true" data-name="' + dept[i].depatureId + '" data-val="true" data-val-required="The IsDeleted field is required." id="chkIsChecked_' + i + '" name="DepatureInfo[' + i + '].IsDeleted" type="checkbox" class="chkDeparture"/></td>';
                        }
                    }

                    dtnew = dept[i].depatureDate.substring(0, 10);
                    dt = dtnew.split('-')[2] + " " + months[parseInt(dtnew.split('-')[1]) - 1] + " " + dtnew.split('-')[0];
                    tbody += '<td><input id="DepatureDate_' + i + '" class="form-control txt-span texttolabel" name="DepatureInfo[' + i + '].DepatureDate" readonly="true" value="' + dt + '" type="text"></td>';

                    if (dept[i].isMaterialised) {
                        tbody += '<td><input data-val="true" data-val-required="The IsMaterialised field is required." id= "chkIsTemplate_' + i + '" name="DepatureInfo[' + i + '].IsMaterialised" checked="checked" type="checkbox" disabled="disabled"/> </td>';
                    }
                    else {
                        tbody += '<td><input data-val="true" data-val-required="The IsMaterialised field is required." id= "chkIsTemplate_' + i + '" name="DepatureInfo[' + i + '].IsMaterialised" type="checkbox" disabled="disabled"/> </td>';
                    }

                    if (dept[i].isShowPending == true && (dept[i].opsBookingNumber == "" || dept[i].opsBookingNumber == null)) {
                        tbody += '<td class="tdStatus"><span>Pending...</span></td>';
                    }
                    else if (dept[i].isShowProcessing == true && (dept[i].opsBookingNumber == "" || dept[i].opsBookingNumber == null)) {
                        tbody += '<td class="tdStatus"><div class="classic-loading-bar"></div></td>';
                    }
                    else if (dept[i].opsBookingNumber != "" && dept[i].opsBookingNumber != null && dept[i].isCreate && dept[i].confirmMessage.toLowerCase() == "success") {
                        tbody += '<td class="tdStatus"><span>Y</span></td>';
                    }
                    else if ((dept[i].opsBookingNumber == "" || dept[i].opsBookingNumber == null) && !dept[i].isCreate && dept[i].confirmMessage.toLowerCase() != "success") {
                        tbody += '<td class="tdStatus"><span>N</span></td>';
                    }
                    else {
                        tbody += '<td class="tdStatus"></td>';
                    }

                    tbody += '<td><input class="form-control txt-span texttolabel" id="OpsBookingNumber_' + i + '" name="DepatureInfo[' + i + '].OpsBookingNumber"'
                    tbody += ' readonly = "true" value="' + (dept[i].opsBookingNumber == null ? "" : dept[i].opsBookingNumber) + '" type="text" ></td> ';
                    tbody += '<td>';
                    if (dept[i].confirmMessage != "" && dept[i].confirmMessage != null && !dept[i].confirmMessage.startsWith("Failure"))
                        tbody += '<div id="ConfirmMessage_"' + i + ' style="overflow-wrap: break-word;">' + dept[i].confirmMessage + '</div>';                     

                    tbody += '</td></tr>';
                }
                $("#tbodydepartures").html('');
                $("#tbodydepartures").html(tbody);
            }
        },
        error: function (error) {
            var msg = "";
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
            $(".ajaxloader").hide();
        }
    });
}

//confirm booking button
$("#btnbooking").click(function () {
    var empties = $("#tbodydepartures").find("input[id^=OpsBookingNumber_]").filter(function () {
        return $.trim($(this).val()) == '';
    });
    if (empties.length == 0) {
        alert('There is no departures for booking.');
        return false;
    }
    else if ($("#tbodydepartures").find("input[id^=chkIsChecked_]:checked").length == 0) {
        alert('Please select departures for booking.');
        return false;
    }
    else {
        depids = new Array();
        if ($("#tbodydepartures").find("input[id^=chkIsChecked_]:checked").length > 0) {
            $(".ajaxloader").hide();
            var flag = true;

            $("#tbodydepartures").find("input[id^=chkIsChecked_]:checked").each(function () {
                depids.push($(this).attr("data-name"));
                if (flag) {
                    flag = false;
                    $('<div class="classic-loading-bar"></div>').appendTo($(this).parents("tr").find("td.tdStatus"));
                }
                else {
                    $('<span>Pending...</span>').appendTo($(this).parents("tr").find("td.tdStatus"));
                }
            });

            if (depids != null && depids.length > 0) {
                $("#btnbooking").attr("disabled", "disabled");
                var request = { //Passing data
                    QRFID: $("#QRFId").val(),
                    DepatureId: depids,
                    __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
                };

                $.ajax({
                    type: "POST", //HTTP POST Method
                    url: "/Handover/SetConfirmBooking", // Controller/View
                    data: request,
                    global: false,
                    success: function (result) {
                       // $("#divSuccessError").show();
                        if (result != null && result != "") {
                            if (result.msgstatus == "Success") {
                                refreshDepartures();
                                //$("#divSuccessError").addClass("alert alert-success");
                                //$("#lblmsg").text(result.message);
                                //$("#stMsg").text("Success!");
                            }
                            else {
                                //$("#divSuccessError").addClass("alert alert-danger");
                                //$("#lblmsg").text(result.message);
                                //$("#stMsg").text("Error!");
                            }
                        }
                        else {
                            //$("#divSuccessError").addClass("alert alert-danger");
                            //$("#lblmsg").text("An Error Occurs.");
                            //$("#stMsg").text("Error!");
                        }
                    },
                    error: function (jqXHR, exception, errorThrown) {
                        var msg = "";
                       // $("#divSuccessError").show();
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
                        //$("#divSuccessError").addClass("alert alert-danger");
                        //$("#lblmsg").text(msg);
                        //$("#stMsg").text("Error!");
                        // alert(msg);
                        $(".ajaxloader").hide();
                    }
                });
            }
            else {
                alert('No departures found for booking.');
                return false;
            }
        }
        else {
            alert('No departures selected for booking.');
            return false;
        }
    }
});

//Message close button
$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});

//$(".chkDeparture").click(function () {
$(document).on('click', '.chkDeparture', function (e) {
    if ($(this).is(":checked")) {
        $(this).parents("tr").addClass("trDeparture");
    }
    else {
        $(this).parents("tr").removeClass("trDeparture");
    }
});