//on view details view popup
$(document).on('click', '.popupchk', function (e) {
    $("#divpopup").html('');

    var index = $(this).parents('tr').find("[id^=hfindex_]").val();
    var dayname = $(this).parents('tr').find("[id^=dayname_]").val();
    var dataaction = $(this).attr('data-action');
    var positionid = dataaction == "pct" ? $("#hfpctId_" + index).val() : dataaction == "st" ? $("#hfstId_" + index).val() :
        dataaction == "fp" ? $("#hffpId_" + index).val() : dataaction == "ft" ? $("#hfftId_" + index).val() : "";

    var hf, hfval, hfold;
    hfold = $(this).parents('td').find("[id^=hf]").attr("id");
    $('.document-list').find('td input:checkbox').each(function () {
        hf = $(this).parents('td').find("[id^=hf]");
        hfval = hf.val();
        hfid = hf.attr("id");
        if (hfold != hfid) {
            var el = $(this), el_TrPopSec = el.parents('tr').next('.tr-pop-sec'), el_ActionPop = el.parents('tr').next('.tr-pop-sec').find('.div-pop-sec');
            $(this).parents('td').removeClass('active');
            $(this).parents('td').removeClass('selected');
            el_TrPopSec.find("#divpopup").html('');
            el_ActionPop.hide();
            el_TrPopSec.hide();           
            if (hfval == "" || hfval == "0") {
                $(this).parents('td').find('input[type="checkbox"]').prop('checked', false);
            }
            else if (hfval != "" && hfval != "0" && $(this).parents('td').find('input[type="checkbox"]').is(":checked")) {
                $(this).parents('td').find("[id^=viewdetails_]").show();
            }
        }
    });

    positionid = positionid == undefined ? 0 : positionid;

    PositionPopupChange(this, $("#hfqrfid").val(), positionid, index, "transfer", dayname);

    if (this.checked == false) {
        var hfval = $(this).parents('td').find("[id^=hf]").val();
        if (hfval != "" && hfval != "0") {
            $(this).parents('td').find("[id^=viewdetails_]").hide();
        }
    }
});

$(document).on("click", ".transferSave", function (e) {
    var model = $("#frmTransfers").serialize();
    model += '&FOC=foc&Price=price';
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/Transfers/Transfers", // Controller/View
        data: model,
        success: function (result) {
            $("#divSuccessError").show();
            if (result.responseStatus.status.toLowerCase() == "success") {
                $("#divSuccessError").addClass("alert alert-success");
                $("#lblmsg").text(result.responseStatus.errorMessage);
                $("#stMsg").text("Success!");
                setTimeout(function () {
                    window.location.href = "/Transfers/Transfers?QRFId=" + $("#hfqrfid").val();
                }, 2000);                
            }
            else {
                $("#divSuccessError").addClass("alert alert-danger");
                $("#lblmsg").text(result.responseStatus.errorMessage);
                $("#stMsg").text("Error!");
            }
        },
        error: function (response) {
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
        }
    });
    var aTag = "";
    $('.document-list').find('td input:checkbox:not(:checked)').each(function () {
        aTag = $(this).parents('td').find("[id^=viewdetails_]");
        if (aTag.css('display') == 'block')
            aTag.css('display', 'none');
    });
});