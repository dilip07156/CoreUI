function LoadProposalSection(QRFId) {
    if (QRFId != '') {
        $("#PriceBreakup").click(function () {
            $('#ProposalPriceBreakup').load('/Proposal/ProposalPriceBreakup/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderPriceBreakUp").show();
            $("#ProposalPriceBreakup").show();
        });

        $("#SuggestedItinerary").click(function () {
            $('#ProposalSuggestedItinerary').load('/Proposal/ProposalSuggestedItinerary/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderSuggestedItinerary").show();
            $("#ProposalSuggestedItinerary").show();
        });

        $("#Routing").click(function () {
            $('#ProposalRouting').load('/Proposal/ProposalRouting/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("completed");
            $("#Routing").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderRouting").show();
            $("#ProposalRouting").show();
        });

        $("#InclusionsExclusions").click(function () {
            $('#ProposalInclusionsExclusions').load('/Proposal/ProposalInclusionsExclusions/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("completed");
            $("#Routing").addClass("completed");
            $("#InclusionsExclusions").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderInclusionsExclusions").show();
            $("#ProposalInclusionsExclusions").show();
        });

        $("#Terms").click(function () {
            $('#ProposalTerms').load('/Proposal/ProposalTerms/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("completed");
            $("#Routing").addClass("completed");
            $("#InclusionsExclusions").addClass("completed");
            $("#Terms").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderTerms").show();
            $("#ProposalTerms").show();
        });

        $("#CoveringNote").click(function () {

            $('#ProposalCoveringNote').load('/Proposal/ProposalCoveringNote/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("completed");
            $("#Routing").addClass("completed");
            $("#InclusionsExclusions").addClass("completed");
            $("#Terms").addClass("completed");
            $("#CoveringNote").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderCoveringNote").show();
            $("#ProposalCoveringNote").show();
        });

        $("#Review").click(function () {
            $('#ProposalReview').load('/Proposal/ProposalReview/', { QRFId: QRFId });
            $("#ProposalTabsCont").children().hide();
            $("#ProposalTabs").parent().find("li").removeClass("active");
            $("#PriceBreakup").addClass("completed");
            $("#SuggestedItinerary").addClass("completed");
            $("#Routing").addClass("completed");
            $("#InclusionsExclusions").addClass("completed");
            $("#Terms").addClass("completed");
            $("#CoveringNote").addClass("completed");
            $("#Review").addClass("active");
            $("#HeadersTab").children().hide();
            $("#HeadersTab #HeaderReview").show();
            $("#ProposalReview").show();
        });
    }
}

$(document).ready(function () {
    $("#btnSubmitCommercialSave").click(function () {
        var data = {
            QRFId: $("#QRFId").val(),
            Remarks: $("#remarks").val(),
            EnquiryPipeline: $("#EnquiryPipeline").val(),
            IsApproveQuote: true,
            Officer: $("#Officer option:selected").text()
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

    $('.closeOkPopup').click(function (e) {
        var QRFId = $("#QRFId").val();
        window.location.href = "/Quote/Quote";
    });

    $("#btnApprovalCP").click(function (e) {
        $(".ajaxloader").show();
        var thisparam = this;
        var data = {
            QRFId: $("#QRFId").val(),
            EnquiryPipeline: "Costing Pipeline"
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
                        //$("#Submitapproval-popup").show();
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