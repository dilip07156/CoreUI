/*----------------------------Amendment Quote starts here----------------------------------------------*/
$(document).ready(function () {
    $(".btnAmendmentQuote").click(function () {
        var data = {
            QRFId: $("#QRFId").val()
        };
        $.ajax({
            type: "POST",
            url: "/AgentApproval/AmendmentQuote",
            data: data,
            dataType: "json",
            success: function (response) {
                $("#spnNewQRFID").text(response.qrfid);
                $("#amendment-quote-submitted").show();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
});
/*----------------------------Amendment Quote starts here----------------------------------------------*/

/*----------------------------send to client starts here----------------------------------------------*/
//$(document).off('click', '.sendtoclient');
$(document).on('click', '.sendtoclient', function (e) {
    $(".ajaxloader").show();
    var thisparam = this;
    var model = { //Passing data 
        QRFID: $("#QRFId").val(),
        __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
    };

    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/AgentApproval/CheckProposalGenerated", // Controller/View
        data: model,
        global: false,
        success: function (response) {
            if (response != null && response != undefined && response != "" && response.status != undefined) {
                if (response.status.toLowerCase() == "success") {
                    $.ajax({
                        type: "POST", //HTTP POST Method
                        url: "/AgentApproval/GetSendToClientDetails", // Controller/View 
                        data: model,
                        success: function (response) {
                            $(".ajaxloader").hide();
                            $("#modelSendToClient-popup").html(response);
                            ShowPopup(thisparam);
                        },
                        error: function (jqXHR, exception, errorThrown) {
                            var msg = "";
                            if (jqXHR.status === 0) {
                                msg = 'Not connect.\n Verify Network.';
                            } else if (jqXHR.status === 404) {
                                msg = 'Requested page not found. [404]';
                            } else if (jqXHR.status === 500) {
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
                            $(".ajaxloader").hide();
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
                }
                else {
                    var msg = response.msg != undefined && response.msg != "" && response.msg != null ? response.msg : "An Error Occurs.";
                    $("#pErrorMsg").text(msg);
                    $(".ajaxloader").hide();
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
            }
            else {
                var msg = "An Error Occurs.";
                $("#pErrorMsg").text(msg);
                $(".ajaxloader").hide();
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
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = "";
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status === 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status === 500) {
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
/*----------------------------send to client ends here----------------------------------------------*/

/*----------------------------AgentAccept starts here----------------------------------------------*/
//$(document).off('click', '.agentaccept');
$(document).on('click', '.agentaccept', function (e) {
    e.preventDefault();
    var thisparam = this;
    var model = { //Passing data 
        QRFID: $("#QRFId").val(),
        __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
    };

    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/AgentApproval/CheckProposalGenerated", // Controller/View
        data: model,
        success: function (response) { 
            if (response != null && response != undefined && response != "" && response.status != undefined) {
                if (response.status.toLowerCase() == "success") {
                    window.location.href = "/AgentApproval/AgentAcceptSendToClient/" + $("#QRFPriceID").val() + "?QRFID=" + $("#QRFId").val() + "&status=agentaccept";
                }
                else {
                    var msg = response.msg != undefined && response.msg != "" && response.msg != null ? response.msg : "An Error Occurs.";
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
            }
            else {
                var msg = msg != undefined && msg != "" && msg != null ? msg : "An Error Occurs.";
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
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = "";
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status === 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status === 500) {
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

            $(".ajaxloader").hide();
        }
    });
});
/*----------------------------AgentAccept ends here----------------------------------------------*/

/*----------------------------communication trail starts here----------------------------------------------*/
$(document).on('click', '.communitrail', function (e) {
    var thisparam = this;
    var model = { //Passing data 
        QRFID: $("#QRFId").val(),
        __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
    };

    $.ajax({
        type: "POST", //HTTP POST Method
        url: "GetCommunicationTrail", // Controller/View
        data: model,
        success: function (response) {
            $("#modelcommunitrail-popup").html(response);
            $("#divemailhtml > style").remove();
            ShowPopup(thisparam);
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = "";
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status === 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status === 500) {
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
});
/*----------------------------communication trail ends here----------------------------------------------*/

/*----------------------------Accept Without Proposal starts here----------------------------------------------*/
$(document).on('click', '#btnAcceptWithoutProposalOk', function (e) {
    var thisparam = this;
    var model = { //Passing data 
        QRFID: $("#QRFId").val(),
        CurrentPipelineStep: $("ul.sqr-tab-list").find("li.active").text().trim(),
        __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
    };

    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/AgentApproval/AcceptWithoutProposal", // Controller/View
        data: model,
        dataType: "json",
        success: function (response) {
            if (response != null && response != undefined && response != "") {
                if (response.status != "" && response.status != undefined && response.status != null) {
                    if (response.status.toLowerCase() == "success") {
                        $("#modelAcceptWithoutProposal").hide();
                        $("#modelAcceptWithoutProposalOk").show();
                    }
                    else {
                        $(".ajaxloader").hide(); 
                        $("#pErrorMsg").text(response.msg);
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
                }
                else {
                    $(".ajaxloader").hide(); 
                    $("#pErrorMsg").text("An Error Occurs.");
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
            }
        },
        error: function (jqXHR, exception, errorThrown) {
            $("#modelAcceptWithoutProposal").hide();
            var msg = "";
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status === 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status === 500) {
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

$('.closeOkPopup').click(function (e) {
    var QRFId = $("#QRFId").val();
    window.location.href = "/Quote/Quote";
});
/*----------------------------Accept Without Proposal ends here----------------------------------------------*/

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
        removalDelay: 600,
        mainClass: 'my-mfp-slide-bottom'
    });
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