$(document).ready(function () {
    $(".rdoExistDept:first").prop("checked", true);
    datepickerInitialize();
});

//Add new departure button
$('#tNewDepartures').on('click', '.btnAddNewDt', function () {
    var cntNewDeptDt = $("input[id^='txtnewDt").length;
    var thisparam = this;
    var curRowCnt = $(thisparam).attr("id").replace("btnAddNewDt_", "");

    var flag = false;
    var newDeptDetails = new Array();
    var existDeptDetails = new Array();
    var cntExistDeptDt = $("input[id^='lblDepatureDate_").length;
    var existDeptDt = "";

    for (var i = 0; i < cntExistDeptDt; i++) {
        existDeptDt = $("input[id='lblDepatureDate_" + i + "").attr("customdt");
        existDeptDetails.push({
            DepatureDate: existDeptDt
        });
    }

    //1)check if new dept date is blank or not
    newDeptDetails = CheckIsEmptyNewDepartureDate();
    flag = (cntNewDeptDt == newDeptDetails.length) ? false : true;

    //2)Check Departure date already exist or not
    var existDeptmsg = CheckIsExistDeparturedate(newDeptDetails, existDeptDetails);

    if (existDeptmsg != "" || flag) {
        if (existDeptmsg != "") {
            alert(existDeptmsg);
        }
        return false;
    }
    else {
        var clonedRow = $(thisparam).closest("tr").clone();
        var cntTr = $(".trNewDeptDatesAdd").length - 1;
        $(thisparam).closest("tr").after(clonedRow);
        var iRowCnt = 0;

        if (cntTr == 0) {
            cntTr = 1;
            iRowCnt = 1;
        }
        else {
            iRowCnt = cntTr + 1;
        }

        $(thisparam).closest("tr").find(".btnAddNewDt").remove();
        $(".trNewDeptDatesAdd:last").find(".ui-datepicker-trigger").remove();

        $(".trNewDeptDatesAdd:last").find('input,span,div,small,a').each(function (iColCnt) {
            if (this.id == "hdnSeqNo") this.value = iRowCnt;

            if (this.id != undefined && this.id != "") {
                this.id = this.id.replace(curRowCnt, iRowCnt);
            }

            if (this.name != undefined && this.name != "") this.name = this.name.replace(curRowCnt, iRowCnt);

            if (this.aria_describedby != undefined) {
                this.aria_describedby = this.aria_describedby.replace(curRowCnt, iRowCnt);
            }
            if (this.attributes['data-valmsg-for'] != undefined && this.attributes['data-valmsg-for'] != "") this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRowCnt, iRowCnt);
        });

        $("#txtnewDt_" + iRowCnt).val('');
        $("span[data-valmsg-for='NewDepatures[" + iRowCnt + "].DepatureDate").text("");

        $("#txtnewDt_" + iRowCnt)
            .removeClass('hasDatepicker')
            .removeData('datepicker')
            .unbind()
            .datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "both",
                dateFormat: "dd/mm/yy"
                //beforeShow: function (el, dp) {
                //    $(el).parent().append($('#ui-datepicker-div'));
                //    $('#ui-datepicker-div').hide();
                //}
            });
    }
});

//remove new departure button
$("#tNewDepartures").on('click', '.btnRemoveDt', function () {
    var removeBtnCnt = $(".btnRemoveDt").length;

    if (removeBtnCnt > 1) {
        $(this).closest('tr').remove();
        var curRow = 0;
        $("#ui-datepicker-div").remove();
        $('#tNewDepartures .trNewDeptDatesAdd').each(function (iRowCnt) {
            $(this).find('input,span,div,small,a').each(function (iColCnt) {
                if (this.id != undefined && this.id != "") {
                    if (this.id == 'hdnSeqNo') { curRow = this.value; this.value = iRowCnt; };
                    this.id = this.id.replace(curRow, iRowCnt);
                }
                if (this.name != undefined && this.name != "") this.name = this.name.replace(curRow, iRowCnt);

                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
            $("#txtnewDt_" + iRowCnt)
                .removeClass('hasDatepicker')
                .removeData('datepicker')
                .unbind()
                .datepicker({
                    changeMonth: true,
                    changeYear: true,
                    showOn: "both",
                    dateFormat: "dd/mm/yy"
                    //beforeShow: function (el, dp) {
                    //    $(el).parent().append($('#ui-datepicker-div'));
                    //    $('#ui-datepicker-div').hide();
                    //}
                }); 
        }); 
        var cnt = $("input[id^='txtnewDt").length - 1;
        $('#tNewDepartures .trNewDeptDatesAdd:last').find(".btnAddNewDt").remove();
        $('#tNewDepartures .trNewDeptDatesAdd:last').find(".divAddButton").html('<a href="#" title="Add" class="icon-squre-green btnAddNewDt" id="btnAddNewDt_' + cnt.toString() + '"><i class="fa fa-plus"></i></a>');
    }
    else {
        alert('Departure date at 1st row can not be delete.');
    }
});

//Create New departure details
$("#btnCreateDeparture").click(function () {
    var cnt = $(".trExistingDeptDates input[type=radio]:checked").length;
    var cntNewDeptDt = $("input[id^='txtnewDt").length;
    var newDeptDetails = new Array();
    var existDeptDetails = new Array();
    var cntExistDeptDt = $("input[id^='lblDepatureDate_").length;

    var msg = "";
    var existDeptDt = "";
    var flag = false;

    if (cnt <= 0) {
        msg = "Please select at least one departure date.";
    }

    for (var i = 0; i < cntExistDeptDt; i++) {
        existDeptDt = $("input[id='lblDepatureDate_" + i + "").attr("customdt");
        existDeptDetails.push({
            DepatureDate: existDeptDt
        });
    }

    //1)check if new dept date is blank or not
    newDeptDetails = CheckIsEmptyNewDepartureDate();
    flag = (cntNewDeptDt == newDeptDetails.length) ? false : true;

    //2)Check Departure date already exist or not
    var existDeptmsg = CheckIsExistDeparturedate(newDeptDetails, existDeptDetails);
    msg = (existDeptmsg != "" && msg != "") ? (msg + "\n") : msg;
    msg += existDeptmsg;

    if (msg != "" || flag) {
        if (msg != "") {
            alert(msg);
        }
        return false;
    }
    else {
        $(".ajaxloader").show();
        var dtArr = [];
        for (var i = 0; i < newDeptDetails.length; i++) {
            dtArr = newDeptDetails[i].DepatureDate.split('/');
            newDeptDetails[i].DepatureDate = dtArr[2] + "/" + dtArr[1] + "/" + dtArr[0];
        }
        var model = { //Passing data
            ExistDepartureId: $(".trExistingDeptDates input[type=radio]:checked").val(),
            GoAheadId: $("#GoAheadId").val(),
            QRFID: $("#QRFId").val(),
            NewDepatures: newDeptDetails,
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        };

        var curparent = $("#modelAddDeparture-popup");
        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/Handover/SetGoAheadNewDepartures", // Controller/View
            data: model,
            // global: false,
            dataType: "html",
            success: function (result) {
                GetGoAheadNewDepature($("#btnAddDept"));
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = "";
                $(curparent).find("#divSuccessError").show();
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
                GetGoAheadNewDepature($("#btnAddDept"));
                alert(msg);
                $(".ajaxloader").hide();
            }
        });
    }
});

//if we select on table row then checked the radio button
$('tr.trExistingDeptDates').click(function () {
    $(this).find('td input[type=radio]').prop('checked', true);
});

//modal popup close/Cancel button
$(document).off('click', '#modelAddDeparture-popup .close-adddept-popup');
$(document).on('click', '#modelAddDeparture-popup .close-adddept-popup', function (e) {
    e.preventDefault();
    $(".ajaxloader").show();
    window.location.href = "/Handover/AttachToMaster?QRFId=" + $("#QRFId").val();
    $.magnificPopup.close();
});

//below function check the new departure dates are empty/blank and return the list of New Departure List
function CheckIsEmptyNewDepartureDate() {
    var cntNewDeptDt = $("input[id^='txtnewDt").length;
    var newDeptDt = "";
    var newDeptDetails = new Array();

    for (var i = 0; i < cntNewDeptDt; i++) {
        newDeptDt = $("input[name='NewDepatures[" + i + "].DepatureDate").val();
        if (newDeptDt != "" && newDeptDt != null && newDeptDt != undefined) {
            $("span[data-valmsg-for='NewDepatures[" + i + "].DepatureDate").text("");
            newDeptDetails.push({
                DepatureDate: newDeptDt
            });
        }
        else {
            $("span[data-valmsg-for='NewDepatures[" + i + "].DepatureDate").text("*");
        }
    }
    return newDeptDetails;
}

//check the new entered Departure date is present in Existing Departures/New Depatures and returns the Error Msg
function CheckIsExistDeparturedate(newDeptDetails, existDeptDetails) {
    var newDeptDt = "";
    var existDept = "";
    var duplicateArr = new Array();
    var msg = "";
    var index = 0;
    var newDept = 0;

    for (var i = 0; i < newDeptDetails.length; i++) {
        newDeptDt = newDeptDetails[i].DepatureDate;
        if (newDeptDt != "" && newDeptDt != null && newDeptDt != undefined) {
            newDept = $.grep(newDeptDetails, function (elem) {
                return elem.DepatureDate == newDeptDt;
            }).length;

            index = duplicateArr.findIndex(a => a.DepatureDate === newDeptDt);
            if (newDept >= 2 && index == -1) {
                duplicateArr.push({
                    DepatureDate: newDeptDt
                });
                msg += "Departure already exist for " + newDeptDt + ".\n";
            }

            existDept = existDeptDetails.find(b => b.DepatureDate == newDeptDt);
            if (newDept == 1 && (existDept != undefined && existDept != "" && existDept != null)) {

                msg += "Departure already exist for " + newDeptDt + ".\n";
            }
        }
    }
    return msg;
}

