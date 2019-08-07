$(document).ready(function () {
    $('.input-append.date').find('input.monthyearCal').datepicker({
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        showButtonPanel: true,
        dateFormat: 'MM yy',
        onClose: function (dateText, inst) {
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
            //the below off event added for DatePicker Month Year dropdown not working in modal popup, so it makes enabled
            $(document).on('focusin');
        },
        beforeShow: function (input, inst) {
            //the below off event added for DatePicker Month Year dropdown not working in modal popup, so it makes enabled
            $(document).off('focusin');
        }
    });
});

//Add/Update button click
$(document).off("click", ".btnTargetAddEdit");
$(document).on("click", ".btnTargetAddEdit", function (e) {
    $(".ajaxloader").show();
    var thisparam = this;

    var targettype = $(thisparam).attr("target-type");
    var currentTr = $(thisparam).parents("tr");
    var TargetId = currentTr.find("#TargetId").val();
    var hfCnt = parseInt(currentTr.find("#hfCnt").val());
    var txtMonth = currentTr.find("#txtMonth_" + hfCnt).val();
    var txtQRFs = currentTr.find("#txtQRFs_" + hfCnt).val();
    var txtBookings = currentTr.find("#txtBookings_" + hfCnt).val();
    var txtIncome = currentTr.find("#txtIncome_" + hfCnt).val();
    var txtPassengers = currentTr.find("#txtPassengers_" + hfCnt).val();

    var lblMonth = "";
    if (targettype == "update") {
        lblMonth = currentTr.find("#lblMonth_" + hfCnt).text();
    }
    var flag = false;
    if (txtMonth == "") {
        flag = true;
        currentTr.find("#spMonth_" + hfCnt).text("*");
        currentTr.find("#spMonth_" + hfCnt).show();
    }

    if (txtQRFs == "" || txtQRFs == "0") {
        flag = true;
        currentTr.find("#spQRFs_" + hfCnt).text("*");
        currentTr.find("#spQRFs_" + hfCnt).show();
    }

    if (txtBookings == "" || txtBookings == "0") {
        flag = true;
        currentTr.find("#spBookings_" + hfCnt).text("*");
        currentTr.find("#spBookings_" + hfCnt).show();
    }

    if (txtIncome == "" || txtIncome == "0") {
        flag = true;
        currentTr.find("#spIncome_" + hfCnt).text("*");
        currentTr.find("#spIncome_" + hfCnt).show();
    }

    if (txtPassengers == "" || txtPassengers == "0") {
        flag = true;
        currentTr.find("#spPassengers_" + hfCnt).text("*");
        currentTr.find("#spPassengers_" + hfCnt).show();
    }

    if (flag) {
        $(".ajaxloader").hide();
        return false;
    }
    else {
        $("#tbodyTargets").find("label.clsMonth").each(function () { 
            if (targettype == "update") {
                if ($(this).text() != lblMonth) {
                    if ($(this).text() == txtMonth) {
                        flag = true;
                        return true;
                    }
                }
            }
            else {
                if ($(this).text() != lblMonth) {
                    if ($(this).text() == txtMonth) {
                        flag = true;
                        return true;
                    }
                }
            }
        });

        if (flag) {
            $(".ajaxloader").hide();
            alert(txtMonth + " is already added.");
            return true;
        }

        var targets = new Array();
        var objtarget = {
            TargetId: TargetId,
            Month: txtMonth,
            QRFs: parseInt(txtQRFs),
            Bookings: parseInt(txtBookings),
            Passengers: parseInt(txtPassengers),
            Income: parseFloat(txtIncome),
            EditDate: null
        };
        var ContactId = $("#ContactId").val();
        var CompanyId = $("#CompanyId").val();

        var model = { //Passing data
            CompanyId: $("#CompanyId").val(),
            ContactId: ContactId,
            Targets: objtarget,
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            type: "POST",
            url: "/Agent/SetCompanyTargets",
            data: model,
            global: false,
            success: function (result) {
                if (result != null && result != "" && result != undefined && result.status != null && result.status != "" && result.status != undefined &&
                    result.status.toLowerCase() == "success") {
                    if (targettype == "add") {
                        var cnt = $("#tbodyTargets tr").length;
                        var target = result.targets;

                        var newTargets = "";
                        newTargets = "<tr><td><input type='hidden' id='TargetId' value='" + target.targetId + "'/><input type='hidden' id='hfCnt' value='" + cnt + "'/>";
                        newTargets += "<label id='lblMonth_" + cnt + "' class='clsMonth'>" + txtMonth + "</label></td>";
                        newTargets += "<td><label id='lblQRFs_" + cnt + "'>" + txtQRFs + "</td>";
                        newTargets += "<td><label id='lblBookings_" + cnt + "'>" + txtBookings + "</td>";
                        newTargets += "<td><label id='lblCur_" + cnt + "' class='clsbasecur'>" + $("#Currency").val() + "</label><label id='lblIncome_" + cnt + "'>" + txtIncome + "</td>";
                        newTargets += "<td><label id='lblPassengers_" + cnt + "'>" + txtPassengers + "</td>";
                        newTargets += "<td><div class='row'><div class='col-lg-12 divTargetEdit'>";
                        newTargets += "<button type='button' class='btn btn-primary btn-sm btnTargetEdit' target-type='edit' id='btnTargetEdit_" + cnt + "'>Edit</button></div></div></td></tr>";
                        $("#tbodyTargets").append(newTargets);

                        cnt += 1;
                        currentTr.find('input,span').each(function () {
                            if (this.id != undefined && this.id != "") {
                                if (this.id == 'hfCnt') { this.value = cnt; };
                                this.id = this.id.replace(hfCnt, cnt);
                            }
                            if (this.nodeName == "SPAN") {
                                this.innerHTML = "";
                            }
                            if (this.nodeName == "INPUT" && this.type != "hidden") {
                                this.value = "";
                            }
                        });
                        initializeDatepicker(hfCnt);

                        if (ContactId !== null && ContactId !== "" && ContactId !== undefined) {
                            $("#divContactTargetDetails").load('/Agent/GetCompanyTargets', { CompanyId: CompanyId, ContactId: ContactId, ActionType: "add" });
                        }
                        else {
                            $("#Targets").attr("actiontype", "add");
                            $("#Targets").click();
                        }
                       // alert("Details added successfully.");
                    }
                    else {
                        UpdateTarget(thisparam);
                       // alert("Details Updated successfully.");
                    }
                }
                else {
                    //var msg = result.msg != null && result.msg != undefined && result.msg != "" ? result.msg : "Details not saved.";

                    if (targettype == "update") {
                        UpdateTarget(thisparam);
                    }
                   // alert(msg);
                }
                $(".ajaxloader").hide();
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
                if (targettype == "update") {
                    UpdateTarget(thisparam);
                }
                if (ContactId !== null && ContactId !== "" && ContactId !== undefined) {
                    $("#divContactTargetDetails").load('/Agent/GetCompanyTargets', { CompanyId: CompanyId, ContactId: ContactId, ActionType: "add"  });
                }
                else {
                    $("#Targets").attr("actiontype", "");
                    $("#Targets").click();
                }
                //alert(msg);
                $(".ajaxloader").hide();
            }
        });
    }
});

//on edit button click
$(document).off("click", ".btnTargetEdit");
$(document).on("click", ".btnTargetEdit", function (e) {
    var parent = $(this).parents('tr');
    var hfCnt = parent.find("#hfCnt").val();
    var Month = parent.find("#lblMonth_" + hfCnt);
    var QRFs = parent.find("#lblQRFs_" + hfCnt);
    var Bookings = parent.find("#lblBookings_" + hfCnt);
    var Income = parent.find("#lblIncome_" + hfCnt);
    var Passengers = parent.find("#lblPassengers_" + hfCnt);
    var BaseCurrency = parent.find("#lblCur_" + hfCnt);
    var Contact_Id = $("#ContactId").val();

    $("#tfootTargets").find(".text-danger").text("");

    Month.hide();
    QRFs.hide();
    Bookings.hide();
    Income.hide();
    BaseCurrency.hide();
    Passengers.hide();
    parent.find(".divTargetEdit").hide();
    parent.find(".editTarget").html('');

    var monthtd = '<div class="row editTarget"><div class="col-lg-10" style="padding-right:0px;"><input type="hidden" id="hfCnt" value="' + hfCnt + '">';
    monthtd += '<div class="input-append date" data-date-format="MM yy"><input type="text" id="txtMonth_' + hfCnt + '" value="' + Month.text() + '" class="form-control monthyearCal txtMonth" maxlength="7">';
    monthtd += '<span class="add-on"></span></div></div><div class="col-lg-2"><span id="spMonth_' + hfCnt + '" class="text-danger" style=""></span></div>';
    Month.after(monthtd);

    initializeDatepicker(hfCnt);

    var QRFstd = '<div class="row editTarget"><div class="col-lg-10" style="padding-right:0px;"><input type="text" id="txtQRFs_' + hfCnt + '" value="' + QRFs.text() + '" class="form-control numericInt txtQRFs" maxlength="5">';
    QRFstd += '</div><div class="col-lg-2"><span id="spQRFs_' + hfCnt + '" class="text-danger" style=""></span></div></div>';
    QRFs.after(QRFstd);

    var Bookingstd = '<div class="row editTarget"><div class="col-lg-10" style="padding-right:0px;"><input type="text" id="txtBookings_' + hfCnt + '" value="' + Bookings.text() + '" class="form-control numericInt txtBookings" maxlength="5">';
    Bookingstd += '</div><div class="col-lg-2"><span id="spBookings_' + hfCnt + '" class="text-danger" style=""></span></div></div>';
    Bookings.after(Bookingstd);

    var Incometd = '<div class="row editTarget"><div class="col-lg-10" style="padding-right:0px;"><input type="text" id="txtIncome_' + hfCnt + '" value="' + Income.text() + '" class="form-control OnlyNumericOneDigit txtIncome" maxlength="10">';
    Incometd += '</div><div class="col-lg-2"><span id="spIncome_' + hfCnt + '" class="text-danger" style=""></span></div></div>';
    Income.after(Incometd);

    var Passengerstd = '<div class="row editTarget"><div class="col-lg-10" style="padding-right:0px;"><input type="text" id="txtPassengers_' + hfCnt + '" value="' + Passengers.text() + '" class="form-control numericInt txtPassengers" maxlength="5">';
    Passengerstd += '</div><div class="col-lg-2"><span id="spPassengers_' + hfCnt + '" class="text-danger" style=""></span></div></div>';
    Passengers.after(Passengerstd);

    var updatecancelbtn = "";

    if (Contact_Id !== null && Contact_Id !== "" && Contact_Id !== undefined) {
        updatecancelbtn = '<div class="col-lg-12 divTargetUpdateCancel"><div class="col-lg-4"><button type="button" class="btn btn-primary btn-sm btnTargetAddEdit" target-type="update">Update</button></div>';
        updatecancelbtn += '<div class="col-lg-1"></div>';
        updatecancelbtn += '<div class="col-lg-4"><button type="button" class="btn btn-primary btn-sm btnTargetUpdateCancel">Cancel</button></div>';
    }
    else {
        updatecancelbtn = '<div class="col-lg-12 divTargetUpdateCancel"><div class="col-lg-2"></div>';
        updatecancelbtn += '<div class="col-lg-4"><button type="button" class="btn btn-primary btn-sm btnTargetAddEdit" target-type="update">Update</button></div>';
        updatecancelbtn += '<div class="col-lg-4"><button type="button" class="btn btn-primary btn-sm btnTargetUpdateCancel">Cancel</button></div>';
    }
    updatecancelbtn += '</div>';

    parent.find(".divTargetEdit").after(updatecancelbtn);
    parent.find(".btnTargetUpdateCancel").bind("click", UpdateCancelTarget);
});

//on update button click
function UpdateTarget(thisparam) {
    var parent = $(thisparam).parents('tr');
    var hfCnt = parent.find("#hfCnt").val();
    var txtMonth = parent.find("#txtMonth_" + hfCnt);
    var txtQRFs = parent.find("#txtQRFs_" + hfCnt);
    var txtBookings = parent.find("#txtBookings_" + hfCnt);
    var txtIncome = parent.find("#txtIncome_" + hfCnt);
    var txtPassengers = parent.find("#txtPassengers_" + hfCnt);

    parent.find("#lblMonth_" + hfCnt).text(txtMonth.val()).show();
    parent.find("#lblQRFs_" + hfCnt).text(txtQRFs.val()).show();
    parent.find("#lblBookings_" + hfCnt).text(txtBookings.val()).show();
    parent.find("#lblIncome_" + hfCnt).text(txtIncome.val()).show();
    parent.find("#lblCur_" + hfCnt).show();
    parent.find("#lblPassengers_" + hfCnt).text(txtPassengers.val()).show();

    txtMonth.parents("div.editTarget").remove();
    txtQRFs.parents("div.editTarget").remove();
    txtBookings.parents("div.editTarget").remove();
    txtIncome.parents("div.editTarget").remove();
    txtPassengers.parents("div.editTarget").remove();
    parent.find(".divTargetUpdateCancel").remove();

    parent.find(".divTargetEdit").show();

    var Contact_Id = $("#ContactId").val();
    var Company_Id = $("#CompanyId").val();
    if (Contact_Id !== null && Contact_Id !== "" && Contact_Id !== undefined) { 
        $("#divContactTargetDetails").load('/Agent/GetCompanyTargets', { CompanyId: Company_Id, ContactId: Contact_Id, ActionType: "update"  });
    }
    else {
        $("#Targets").attr("actiontype","update");
        $("#Targets").click();
    }
}

//on cancel button click
function UpdateCancelTarget() {
    $("#tfootTargets").find(".text-danger").text("");
    var parent = $(this).parents('tr');
    var hfCnt = parent.find("#hfCnt").val();
    parent.find("#txtMonth_" + hfCnt).parents("div.editTarget").remove();
    parent.find("#txtQRFs_" + hfCnt).parents("div.editTarget").remove();
    parent.find("#txtBookings_" + hfCnt).parents("div.editTarget").remove();
    parent.find("#txtIncome_" + hfCnt).parents("div.editTarget").remove();
    parent.find("#txtPassengers_" + hfCnt).parents("div.editTarget").remove();
    parent.find(".divTargetUpdateCancel").remove();

    parent.find("#lblMonth_" + hfCnt).show();
    parent.find("#lblQRFs_" + hfCnt).show();
    parent.find("#lblBookings_" + hfCnt).show();
    parent.find("#lblIncome_" + hfCnt).show();
    parent.find("#lblCur_" + hfCnt).show();
    parent.find("#lblPassengers_" + hfCnt).show();
    parent.find(".divTargetEdit").show();
}

function initializeDatepicker(Cnt) {
    $("#txtMonth_" + Cnt)
        .removeClass('hasDatepicker')
        .removeData('datepicker')
        .unbind()
        .datepicker({
            changeMonth: true,
            changeYear: true,
            showOn: "both",
            showButtonPanel: true,
            dateFormat: 'MM yy',
            onClose: function (dateText, inst) {
                $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
                //the below off event added for DatePicker Month Year dropdown not working in modal popup, so it makes enabled
                $(document).on('focusin');
            },
            beforeShow: function (input, inst) {
                //the below off event added for DatePicker Month Year dropdown not working in modal popup, so it makes enabled
                $(document).off('focusin');
            }
        });
}