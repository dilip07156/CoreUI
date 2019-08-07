$(document).ready(function () {
    var form = $("#frmCopyQuote");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    $('.input-append.date').find('input.span3').datepicker({
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        dateFormat: "dd/mm/yy",
        beforeShow: function (el, dp) {
            $(el).parent().append($('#ui-datepicker-div'));
            $('#ui-datepicker-div').hide();
        },
        minDate: new Date()
    });

    $(".bindAgent").on("keyup", function (event) {
        InitializeAutoComplete('/Quote/GetAgentNameFrommCompanies', this, 3, { term: this.value }, 'agent');
    });

    $("#ContactPerson").change(function () {
        var ContactID = $("#ContactPerson").val();
        $.ajax({
            type: "GET",
            url: "/Quote/GetContactDetailsByContactID",
            data: { ContactID: ContactID },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $("#MobileNo").val(result.mobile);
                $("#Email").val(result.mail);
            },
            error: function (error) {
                alert(error);
            }
        });
    });

    $(document).on("change", ".ddlDeparture", function (event) {

        var thisparam = $(this);
        var Rate = thisparam.find('option:selected').attr("Rate");
        var Currency = thisparam.find('option:selected').attr("Currency");
        if (Currency != undefined)
            thisparam.parents("tr").find(".lblPrice").text(Currency + ' ' + Rate);
    });

    //Add new departure button
    $('#tNewDepartures').on('click', '.btnAddNewDt', function () {

        var cntNewDeptDt = $("input[id^='txtnewDt").length;
        var thisparam = this;
        var curRowCnt = $(thisparam).attr("id").replace("btnAddNewDt_", "");
        var flag = false;
        var msg = "";
        var newDeptDetails = new Array();

        //1)check if new dept date is blank or not
        newDeptDetails = CheckIsEmptyNewDepartureDate();
        flag = (cntNewDeptDt == newDeptDetails.length) ? false : true;

        //2)Check Departure date already exist or not
        var existDeptmsg = CheckIsExistDeparturedate(newDeptDetails);
        msg = (existDeptmsg != "" && msg != "") ? (msg + "\n") : msg;
        msg += existDeptmsg;

        if (msg != "" || flag) {
            if (msg != "") {
                alert(msg);
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

            $(".trNewDeptDatesAdd:last").find('input,span,div,small,button,select').each(function (iColCnt) {
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
                    dateFormat: "dd/mm/yy",
                    beforeShow: function (el, dp) {
                        $(el).parent().append($('#ui-datepicker-div'));
                        $('#ui-datepicker-div').hide();
                    },
                    minDate: new Date()
                });
            var form = $("#frmCopyQuote");
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        }
    });

    //remove new departure button
    $("#tNewDepartures").on('click', '.btnRemove', function () {
        var removeBtnCnt = $(".btnRemove").length;

        if (removeBtnCnt > 1) {
            $(this).closest('tr').remove();
            var curRow = 0;

            $('#tNewDepartures .trNewDeptDatesAdd').each(function (iRowCnt) {
                $(this).find('input,span,div,small,button,select').each(function (iColCnt) {

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
                        showOn: "both",
                        dateFormat: "mm/dd/yy",
                        minDate: new Date()
                    });
            });
            var cnt = $("input[id^='txtnewDt").length - 1;
            $('#tNewDepartures .trNewDeptDatesAdd:last').find(".btnAddNewDt").remove();
            $('#tNewDepartures .trNewDeptDatesAdd:last').find(".divAddButton").html('<a href="#" title="Add" class="icon-squre-green btnAddNewDt" id="btnAddNewDt_' + cnt.toString() + '"><i class="fa fa-plus"></i></a>');
        }
        else {
            alert('Departure date at 1st row can not be delete.');
        }
        var form = $("#frmCopyQuote");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    });

    $(document).on("focusout", ".clsNewDepartureDate", function (event) {
        var msg = "";
        var newDt = $(this).val();
        var from = newDt.split("/");
        if (ValidateDatemmddyyyy(newDt)) {
            var fromDt = from[2] + '/' + from[0] + '/' + from[1];
            var date = new Date();
            var curDate = date.getFullYear() + "/" + (date.getMonth() + 1) + "/" + date.getDate();
            if (Date.parse(curDate) > Date.parse(fromDt)) {
                $(this).val('');
                msg = "Departure date should not be back dated.";
                alert(msg);
                return false;
            }
        }
        else {
            $(this).val('');
        }
        return true;
    });
});

//below function check the new departure dates are empty/blank and return the list of New Departure List
function CheckIsEmptyNewDepartureDate() {
    var cntNewDeptDt = $("input[id^='txtnewDt").length;
    var newDeptDt = "";
    var newDeptDetails = new Array();

    for (var i = 0; i < cntNewDeptDt; i++) {
        newDeptDt = $("input[name='CopyQuoteDepartures[" + i + "].NewDepartureDate").val();
        if (newDeptDt != "" && newDeptDt != null && newDeptDt != undefined) {
            $("span[data-valmsg-for='CopyQuoteDepartures[" + i + "].NewDepartureDate").text("");
            newDeptDetails.push({
                DepatureDate: newDeptDt
            });
        }
        else {
            $("span[data-valmsg-for='CopyQuoteDepartures[" + i + "].NewDepartureDate").text("*");
        }
    }
    return newDeptDetails;
}

//check the new entered Departure date is present in Existing Departures/New Depatures and returns the Error Msg
function CheckIsExistDeparturedate(newDeptDetails, existDeptDetails) {
    var newDeptDt = "";
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
        }
    }
    return msg;
}

function SetCopyQuote() {

    var isvalid = $("#frmCopyQuote").valid();
    if (!isvalid) {
        return false;
    }

    var model = $("#frmCopyQuote").serialize();

    $.ajax({
        type: "POST",
        url: "/Quote/SetCopyQuote",
        data: model,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $("#copy-quote-main").html('');
            $("#copy-quote-submitted").show();
            if (response.responseStatus.status == 'Success') {
                $("#spnNewQRFID").text(response.qrfid);
            }
            else {
                $("#spnNewQRFID").text(response.responseStatus.errorMessage);
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

function AutoCompleteChanged(thisparam) {
    BindAgentDetails();
}

function BindAgentDetails() {
    $("#MobileNo").val('');
    $("#Email").val('');
    var AgentId = $("#AgentId").val();
    if (AgentId.length > 2) {
        $.ajax({
            type: "GET",
            url: "/Quote/GetAgentContacts",
            data: { AgentId: AgentId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var $el = $("#ContactPerson");
                $el.empty();
                $el.append($("<option></option>").attr("value", '').text('Select'));
                $.each(result, function (i, list) {
                    $el.append($("<option></option>").attr("value", list.voyagerContact_Id).text(list.fullName));
                });
            },
            failure: function (error) {
                alert(error);
            },
            error: function (error) {
                alert(error);
            }
        });
    }
} 