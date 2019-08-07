$(document).ready(function () {
    $(".filter").change(function () {
        GetCommercialsData();
    });

    $('.StandardRow .show-positions').click(function () {
        var PositionType = $(this).closest('td').find('.PositionType').text();
        if (PositionType != null && PositionType != '') {
            PositionType = PositionType.replace(' ', '-');
            $('.' + PositionType).toggle();
            $(this).toggleClass('fa-chevron-circle-up fa-chevron-circle-down');
        }
    });

    $('.chkStandard').click(function () {
        var flag = $(this).is(":checked");
        var PositionType = $(this).closest('td').find('.PositionType').text();
        if (PositionType != null && PositionType != '') {
            PositionType = PositionType.replace(' ', '-');

            $('.' + PositionType).find('.chkPosition').each(function (index) {
                if (flag) {
                    $(this).val(true);
                    $(this).prop("checked", "checked");
                }
                else {
                    $(this).val(false);
                    $(this).prop("checked", "");
                }
            });
        }
    });

    $(".showMargin").click(function () {
        $("#Margin-popup .popup-in").html('');
        var QRFId = $("#QRFId").val();
        $.ajax({
            type: "GET",
            url: "/Quote/GetMargin",
            data: { QRFId: QRFId, IsCostingMargin : true },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $("#Margin-popup .popup-in").html(response);
                $("#Margin-popup").show();
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    });

    $("#btnSubmitCommercialSave").click(function () {
        var data = {
            QRFId: $("#QRFId").val(),
            Remarks: $("#remarks").val(),
            EnquiryPipeline: $("#EnquiryPipeline").val(),
            IsApproveQuote: true
        }
        $.ajax({
            type: "POST",
            url: "/Commercials/SubmitCommercial",
            data: data,
            dataType: "json",
            success: function (response) {
                $("#commercial-approval-submitted").show();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });   

    $("#btnSubmitCommercialReject").click(function () {
        var data = {
            QRFId: $("#QRFId").val(),
            Remarks: $("#remarks").val(),
            EnquiryPipeline: $("#EnquiryPipeline").val(),
            IsApproveQuote:false
        }
        $.ajax({
            type: "POST",
            url: "/Commercials/SubmitCommercial",
            data: data,
            dataType: "json",
            success: function (response) {
                $("#commercial-reject-submitted").show();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });   

    $('.close-popup').click(function (e) {
        $.magnificPopup.close()
    });

    $('.closeOkPopup').click(function (e) {
        var QRFId = $("#QRFId").val();
        window.location.href = "/Quote/Quote";
    });

    $("#btnApprovalCA").click(function (e) {
        $(".ajaxloader").show();
        var thisparam = this;
        var data = {
            QRFId: $("#QRFId").val(),
            EnquiryPipeline: "Costing Approval Pipeline"
        };

        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/Costsheet/CheckActiveCostsheetPrice", // Controller/View
            data: data,
            global: false,
            success: function (result) {
                if (result != null && result != undefined && result != "") {
                    if (result.status != null && result.status == "Failure") {
                        $(".ajaxloader").hide();
                        $("#pErrorMsg").text(result.errorMessage);
                        $.magnificPopup.open({
                            type: 'inline',
                            items: { src: "#modelErrorMsg" },
                            fixedContentPos: true,
                            fixedBgPos: true,
                            overflowY: 'auto',
                            closeBtnInside: true,
                            preloader: false,
                            midClick: true,
                            removalDelay: 600,
                            mainClass: 'my-mfp-slide-bottom'
                        });
                        $("#modelErrorMsg").show();
                    }
                    else { 
                        $(".ajaxloader").hide();
                        ShowPopup(thisparam); 
                    }
                }
            },
            error: function (jqXHR, exception, errorThrown) {
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
                $("#pErrorMsg").text(msg);
                $.magnificPopup.open({
                    type: 'inline',
                    items: { src: "#modelErrorMsg" },
                    fixedContentPos: true,
                    fixedBgPos: true,
                    overflowY: 'auto',
                    closeBtnInside: true,
                    preloader: false,
                    midClick: true,
                    removalDelay: 600,
                    mainClass: 'my-mfp-slide-bottom'
                });
                $("#modelErrorMsg").show();
            }
        });
    });
});

function changePosition(con, ChangeType) {
    $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    var mainTable = $(con).closest('.table-responsive');
    var PositionIdList = [];
    $(mainTable).find('.ProductRow .chkPosition').each(function (index) {
        var chkPosition = $(this).is(":checked");
        var PositionId = $(this).closest('tr').find('#PositionId').val();
        if (chkPosition) {
            PositionIdList.push(PositionId);
        }
    });
    $.ajax({
        type: "POST",
        url: "/Commercials/ChangePositionKeepAs",
        data: { QRFId: QRFId, ChangeType: ChangeType, PositionIds: PositionIdList },
        dataType: "json",
        global: false,
        success: function (response) {
            GetCommercialsData();
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

function saveCommercials(con, ChangeType) {
    $(".ajaxloader").show();
    var QRFPriceId = $("#QRFPriceId").val();
    var PercentSoldOptional = $("#PercentSoldOptional").val();
    var Qrfid = $("#QRFId").val();
    $.ajax({
        type: "POST",
        url: "/Commercials/SaveCommercials",
        data: { QRFPriceId: QRFPriceId, PercentSoldOptional: PercentSoldOptional, QRFID: Qrfid },
        dataType: "json",
        success: function (response) {
           // GetCommercialsData();
            $('.alert').remove();
            var successmsg = '<div class="alert alert-success"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success!</strong> Data saved Successfully.</div>';
            $('form').before(successmsg);
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
        closeOnBgClick: true,
        enableEscapeKey: true
    });
}