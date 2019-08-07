$(document).ready(function () {

    $("tr.trMaterialised").each(function (index) {
        var confirmationstatus = $(this).find("[id^=ConfirmStatus]").val().toLowerCase();
        if (confirmationstatus == "false") {
            if ($(this).find(".chkIsCreate").is(":checked"))
                $(this).find(".chkMaterialised").attr("disabled", false);
            else {
                $(this).find(".chkMaterialised").attr("disabled", true);
                $(this).find(".chkMaterialised").prop("checked", false);
                $(this).find(".anchorMaterialised").hide();
            }
        }
        else {
            $(this).find(".chkIsCreate").attr("disabled", "disabled");
        }
    });

    if ($("#ConfirmationDeadline").val().indexOf('0001') > -1) {
        $("#ConfirmationDeadline").val('');
    }
});

function checkValidation() {
    if ($("#ConfirmationDeadline").val() == "" && $("#ddlUserID").val() == "") {
        $("#divdt").find("span").text("*");
        $("#divUserID").find("span").text("*");
        alert("Please Enter Hotel Confirmation Deadline.\nPlease Select Operations User.");
        return false;
    }
    else if ($("#ConfirmationDeadline").val() == "") {
        $("#divdt").find("span").text("*");
        alert("Please Enter Hotel Confirmation Deadline.");
        return false;
    }
    else if ($("#ddlUserID").val() == "") {
        $("#divUserID").find("span").text("*");
        alert("Please Select Operations User.");
        return false;
    }
    else {
        $("#divdt").find("span").text("");
        $("#divUserID").find("span").text("");
        return true;
    }
}

function SetAttachToMaster(thisparam = null, departureid = null, type, isanchor) {
    $(".ajaxloader").show();
    var model = $('#frmAttachToMaster').serialize();
    if (isanchor) {
        model += '&Type=partial';
    }
    else {
        model += '&Type=partial&DepartureId=' + $(departureid).val();
    }
    $.ajax({
        type: "POST",
        url: "/Handover/SetAttachToMaster",
        data: model,
        global: false,
        success: function (result) {
            if (result != null && result != "" && result != undefined && result.status != null && result.status != "" && result.status != undefined &&
                result.status.toLowerCase() == "success") {
                $("#GoAheadId").val(result.goaheadid);
                if (departureid != null && departureid != "" && departureid != undefined && thisparam != null && thisparam != undefined && type == "materialised") {
                    GetMaterialisation($("#QRFId").val(), result.goaheadid, $(departureid).val(), thisparam);
                }
                else if (type == "adddepartures") {
                    GetGoAheadNewDepature(thisparam);
                }
            }
            else {
                var msg = result.msg != null && result.msg != undefined && result.msg != "" ? result.msg : "Details not saved.";
                var erromsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong>' + msg + '</div>';
                $('.alert-danger').remove();
                $('#frmAttachToMaster .divbuttons').before(erromsg);
                $(".ajaxloader").hide();
            }
        },
        error: function (response) {
            var erromsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.statusText + '</div>';
            $('.alert-danger').remove();
            $('#frmAttachToMaster .divbuttons').before(erromsg);
            $(".ajaxloader").hide();
        }
    });
}

$(".chkMaterialised").click(function (event) {
    var type = $(this).attr("data-type");
    var flag = false;
    var isanchor = false;
    if (type == "checkbox") {
        isanchor = false;
        flag = $(this).is(":checked");
    }
    else if (type == "anchor") {
        isanchor = true;
        flag = true;
    }

    if (!checkValidation()) {
        return false;
    }

    if (type == "checkbox" && !flag) {
        $(this).parents("td").find("[id^=viewdetails_]").hide();
    }
    else if (flag) {
        var thisparam = this;
        event.preventDefault();
        //save the GoAheadInfo and set the ID  
        var curid = $(this).attr("id");
        var index = type == "checkbox" ? curid.replace("chkIsMaterialised_", "") : curid.replace("viewdetails_", "");
        var departureid = "#DepatureId_" + index;
        SetAttachToMaster(thisparam, departureid, "materialised", isanchor);
    }
});

$(".chkIsCreate").click(function (event) {
    var curid = $(this).attr("id");
    var index = curid.replace("chkIsCreate_", "");
    $("#viewdetails_" + index).hide();
    $("#chkIsMaterialised_" + index).prop("checked", false);
    if ($(this).is(":checked")) {
        $("#chkIsMaterialised_" + index).attr("disabled", false);
    }
    else {
        $("#chkIsMaterialised_" + index).attr("disabled", true);
    }
});

$(document).off('blur', '.roomcount');
$(document).on('blur', '.roomcount', function (e) {
    var curid = $(this).attr("id");
    var index = curid.replace("RoomCount_", "");
    var RoomTypeName = "#RoomTypeName_" + index;
    var RoomCount = $("#RoomCount_" + index).val();
    var PaxCount = "#PaxCount_" + index;
    if ($(RoomTypeName).val().toLowerCase() == "single") {
        $(PaxCount).val(RoomCount * 1);
    }
    else if ($(RoomTypeName).val().toLowerCase() == "double") {
        $(PaxCount).val(RoomCount * 2);
    }
    else if ($(RoomTypeName).val().toLowerCase() == "triple") {
        $(PaxCount).val(RoomCount * 3);
    }
    else if ($(RoomTypeName).val().toLowerCase() == "quod") {
        $(PaxCount).val(RoomCount * 4);
    }
    else if ($(RoomTypeName).val().toLowerCase() == "twin") {
        $(PaxCount).val(RoomCount * 2);
    }
    else if ($(RoomTypeName).val().toLowerCase() == "tsu") {
        $(PaxCount).val(RoomCount * 5);
    }
});

function GetMaterialisation(QRFId, GoAheadId, DepartureId, thisparam) {
    $.ajax({
        type: "GET",
        url: "/Handover/GetGoAheadDepature",
        data: { QRFId: QRFId, GoAheadId: GoAheadId, depatureid: DepartureId },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $("#modelMaterialisation-popup").html(response);
            ShowPopup(thisparam);
        },
        failure: function (response) {
            $(".ajaxloader").hide();
            alert(response.responseText);
        },
        error: function (response) {
            $(".ajaxloader").hide();
            alert(response.responseText);
        }
    });
}

$(document).off('click', '#modelMaterialisation .close-popup');
$(document).on('click', '#modelMaterialisation .close-popup', function (e) {
    //$('.closeMaterialisation-popup').click(function (e) {    
    $(".ajaxloader").show();
    e.preventDefault();
    GetAttachToMaster();
    $.magnificPopup.close();
});

$(".rdoIsCurrentVersion").click(function () {
    if ($(this).is(":checked")) {
        var id = $(this).attr("id");
        var index = id.replace("rdoSelect_", "");
        $("#QRFPriceId").val($("#QRFPriceId_" + index).val());
        $("#VersionId").val($("#VersionId_" + index).text());
    }
})

/*----------------------------------Materlization grid add button click start here-----------------------------------*/
//$(document).ready(function () {
// $('#tblChild').on('click', 'a.addMaterilization', function () {
//$('.addMaterilization').click(function (e) {
$(document).off('click', '.addMaterilization');
$(document).on('click', '.addMaterilization', function (e) {
    addRow(this);
    return false;
});
//});

function addRow(thisparam) {
    var flag = true;
    var flagOld = true;
    $("#tblChild .ChildRow:not([style*='display: none;'])").each(function (index) {
        flag = currentRowValidation($(this));
        if (!flag) {
            flagOld = false;
        }
    });

    if (!flagOld) {
        alert('Please fill the mandatory fields before adding new record!');
        return false;
    }

    var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();
    var clonedRow = $(thisparam).closest('tr').clone();
    $(thisparam).closest('tr').after(clonedRow);

    $('#tblChild tbody tr.ChildRow').each(function (iRowCnt) {
        if (iRowCnt > index) {
            var curRow = $(this).find('#hdnSeqNo').val();
            this.id = this.id.replace(curRow, iRowCnt);

            $(this).find('input,select,span,td,div').each(function (iColCnt) {
                if (this.id != undefined) {
                    this.id = this.id.replace(curRow, iRowCnt);
                    if (this.id.indexOf('hdnSeqNo') > -1) this.value = iRowCnt;
                    else if (iRowCnt == (parseInt(index) + 1)) {
                        if (this.id.indexOf('ddlChildTypeListID') > -1 || this.id.indexOf('ddlAgeID') > -1) {
                            $(this).val("");
                        }
                        else this.value = '';
                        $(this).find(".collapse-container").show();
                        $(this).find(".collapse-link").removeClass("collapsed");
                    }
                }
                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });
    $(".text-danger").text("");
    var form = $("#frmAttachToMaster");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}

function currentRowValidation(currentRow) {
    var IsValidFlag = true;

    var ddlChildTypeListID = currentRow.find("[id^=ddlChildTypeListID]").val();
    var ddlChildTypeListText = currentRow.find("[id^=ddlChildTypeListID] > option[value='" + ddlChildTypeListID + "']").text();

    var ddlChildTypeList = currentRow.find('[id^=ddlChildTypeListID] > option:contains("' + ddlChildTypeListText + '")').attr("data-flag");

    var childNumber = currentRow.find("[id^=childNumber]").val();
    var childAge = currentRow.find("[id^=ddlAgeID]").val();
    if (ddlChildTypeList == "True") {
        if (ddlChildTypeListID == '' || ddlChildTypeListID == null) {
            currentRow.find("[id^=ddlChildTypeListID]").siblings("span").text("*");
            IsValidFlag = false;
        }

        if (childNumber == '') {
            currentRow.find("[id^=childNumber]").siblings("span").text("*");
            IsValidFlag = false;
        }

        if (childAge == '') {
            currentRow.find("[id^=ddlAgeID]").siblings("span").text("*");
            IsValidFlag = false;
        }
    }
    else {
        currentRow.find("[id^=ddlChildTypeListID]").siblings("span").text("");
        currentRow.find("[id^=childNumber]").siblings("span").text("");
        currentRow.find("[id^=ddlAgeID]").siblings("span").text("");
    }

    return IsValidFlag;
}

/*----------------------------------Materlization grid add button click ends here-----------------------------------*/

/*----------------------------------Materlization grid remove button click start here-----------------------------------*/
//$('#tblChild').on('click', 'a.deleteMaterilization', function () {
$(document).off('click', '.deleteMaterilization');
$(document).on('click', '.deleteMaterilization', function (e) {
    var clonedInputLen = $("#tblChild .ChildRow").length;
    if (clonedInputLen == 1) {
        $(this).closest('tr').find('input,select,span').each(function (iColCnt) {
            if (this.value != undefined) {
                if (this.id == 'hdnSeqNo') this.value = '0';
                else if (this.id.indexOf('ddlChildTypeListID') > -1) $("#" + this.id).val("");
                else {
                    this.value = '';
                }
            }
        });
        alert('Cannot delete this record.');
        return false;
    }

    if (confirm("Are you sure you want to delete this row?")) {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var id = $('#hdnChildInfoId_' + index).val();

        if (id != undefined && id != null && id != '' && id.length > 30 && id.substring(0, 8) != '00000000') {
            $(this).closest('tr').hide();
            $('#hdnIsDeleted_' + index).val('True');
        }
        else {
            removeRow(this, index);
        }
    }
    return false;
});

function removeRow(thisparam, rowIndex) {
    $(thisparam).closest('tr').remove();
    $('#tblChild .ChildRow').each(function (iRowCnt) {
        if (iRowCnt >= rowIndex) {
            var curRow = $(this).find('#hdnSeqNo').val();
            $(this).find('input,select,span').each(function (iColCnt) {
                if (this.id != undefined) {
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else this.id = this.id.replace(curRow, iRowCnt);
                }
                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });

}
/*----------------------------------Materlization grid remove button click end here-----------------------------------*/

//$(".SetMaterialisation").click(function (e) {
$(document).off('click', '.SetMaterialisation');
$(document).on('click', '.SetMaterialisation', function (e) {
    var curparent = $(this).parents("#modelMaterialisation");
    $(".text-danger").text("");
    var flag = true;
    var flagOld = true;
    $("#tblChild .ChildRow:not([style*='display: none;'])").each(function (index) {
        flag = currentRowValidation($(this));
        if (!flag) {
            flagOld = false;
        }
    });

    $("#tblAdult .adultrow").each(function (index) {
        flag = currentAdultRowValidation($(this));
        if (!flag) {
            flagOld = false;
        }
    });

    if (flagOld) {
        $(".ajaxloader").show();
        var ChildInfo = new Array();
        var ddlChildTypeListID = "";
        var childno = "";
        var childAge = "";
        var Isdeleted = "";
        var ChildInfoId = "";

        $("#tblChild .ChildRow").each(function (index) {
            ddlChildTypeListID = $(this).find("[id^=ddlChildTypeListID]").val();
            childno = $(this).find("[id^=childNumber]").val();
            childAge = $(this).find("[id^=ddlAgeID]").val();
            Isdeleted = $(this).find("[id^=hdnIsDeleted]").val();
            ChildInfoId = $(this).find("[id^=hdnChildInfoId]").val();

            if (ddlChildTypeListID != '' && ddlChildTypeListID != null && childno != "" && childAge != "") {
                ChildInfo.push({
                    ChildInfoId: ChildInfoId,
                    Type: ddlChildTypeListID,
                    Number: childno,
                    Age: childAge,
                    IsDeleted: Isdeleted
                });
            }
        });

        var AdultInfo = new Array();
        var RoomCount = "";
        var RoomTypeName = "";
        var RoomTypeID = "";
        $("#tblAdult .adultrow").each(function (index) {
            RoomCount = $(this).find("[id^=RoomCount_]").val();
            RoomTypeName = $(this).find("[id^=RoomTypeName_]").val();
            RoomTypeID = $(this).find("[id^=RoomTypeID_]").val();
            PaxCount = $(this).find("[id^=PaxCount_]").val();

            if (RoomCount != '') {
                AdultInfo.push({
                    RoomTypeID: RoomTypeID,
                    RoomTypeName: RoomTypeName,
                    RoomCount: RoomCount,
                    PaxCount: PaxCount
                });
            }
        });

        var model = { //Passing data
            DepartureId: $("#DepartureId").val(),
            GoAheadId: $("#GoAheadId").val(),
            QRFId: $("#QRFId").val(),
            RoomInfo: AdultInfo,
            ChildInfo: ChildInfo,
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/Handover/SetMaterialisation", // Controller/View
            data: model,
            // global: false,
            success: function (result) {
                $(curparent).find("#divSuccessError").show();
                if (result != null && result != "" && result.status != "" && result.status != null) {
                    if (result.status.toLowerCase() == "success") {
                        $(curparent).find("#divSuccessError").addClass("alert alert-success");
                        $(curparent).find("#lblmsg").text(result.msg);
                        $(curparent).find("#stMsg").text("Success!");
                    }
                    else {
                        $(".ajaxloader").hide();
                        $(curparent).find("#divSuccessError").addClass("alert alert-danger");
                        $(curparent).find("#lblmsg").text(result.msg);
                        $(curparent).find("#stMsg").text("Error!");
                    }
                }
                else {
                    $(".ajaxloader").hide();
                    $(curparent).find("#divSuccessError").addClass("alert alert-danger");
                    $(curparent).find("#lblmsg").text("Details Not Saved.");
                    $(curparent).find("#stMsg").text("Error!");
                }
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
                $(curparent).find("#divSuccessError").addClass("alert alert-danger");
                $(curparent).find("#lblmsg").text(msg);
                $(curparent).find("#stMsg").text("Error!");
                $(".ajaxloader").hide();
            }
        });
    }
    else {
        alert('Please fill the mandatory fields.');
    }
});

function currentAdultRowValidation(currentRow) {
    var IsValidFlag = true;

    var RoomCount = currentRow.find("[id^=RoomCount_]").val();
    if (RoomCount == '') {
        currentRow.find("[id^=RoomCount]").siblings("span").text("*");
        IsValidFlag = false;
    }
    return IsValidFlag;
}

//Message close button
$(document).off('click', '.close');
$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});

function GetAttachToMaster() {
    window.location.href = "/Handover/AttachToMaster?QRFId=" + $("#QRFId").val();
    //$.ajax({
    //    type: "GET",
    //    url: "/AgentApproval/AttachToMaster",
    //    data: { QRFId: $("#QRFId").val(), type: "partial" },
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "html",
    //    success: function (response) {
    //        $("#attachtomaster").html(response);
    //    },
    //    failure: function (response) {
    //        $(".ajaxloader").hide();
    //        alert(response.responseText);
    //    },
    //    error: function (response) {
    //        $(".ajaxloader").hide();
    //        alert(response.responseText);
    //    }
    //});
}

$(document).off('click', '.btnSave');
$(document).on('click', '.btnSave', function (e) {
    $(".text-danger").find("span").text("");

    var flag = true;
    if ($("#ConfirmationDeadline").val() == "") {
        $("#divdt").find("span").text("*");
        flag = false;
    }

    if ($("#ddlUserID").val() == "") {
        $("#divUserID").find("span").text("*");
        flag = false;
    }
    return flag;
});

/*------------------------------------------Add Departure starts here-------------------------------------------*/
$(document).on('click', '#btnAddDept', function (e) {
    if (!checkValidation()) {
        return false;
    }
    e.preventDefault();
    SetAttachToMaster(this, null, "adddepartures");
});

function GetGoAheadNewDepature(thisparam) {
    var QRFId = $("#QRFId").val();
    var GoAheadId = $("#GoAheadId").val();
    $.ajax({
        type: "GET",
        url: "/Handover/GetGoAheadNewDepature",
        data: { QRFId: QRFId, GoAheadId: GoAheadId },
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $("#modelAddDeparture-popup .popup-in").html(response);
            //$("#modelAddDeparture-popup").show();        
            ShowPopup(thisparam);
            $("#txtnewDt_0")
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
                    }
                });
        },
        failure: function (response) {
            $(".ajaxloader").hide();
            alert(response.responseText);
        },
        error: function (response) {
            $(".ajaxloader").hide();
            alert(response.responseText);
        }
    });
}
/*------------------------------------------Add Departure ends here---------------------------------------------*/

function ShowPopup(thisparam) {
    //opens the popup
    var targetDiv = $(thisparam).attr("href");
    $.magnificPopup.open({
        type: 'inline',
        items: { src: targetDiv },
        fixedContentPos: true,
        fixedBgPos: true,
        overflowY: 'auto',
        closeBtnInside: true,
        preloader: false,
        midClick: true,
        removalDelay: 500,
        mainClass: 'my-mfp-slide-bottom',
        closeOnBgClick: false,
        enableEscapeKey: false
    });
}


