$(document).ready(function () {    
    $("#divemailhtml").find(".container").removeClass("container");
});

$(".btnview").click(function () {
    $("#divSuccessError").hide();
    var moduleType = $("#ModuleType").val();
    var url = "";

    if (moduleType !== "ops") {
        url = "GetCommunicationTrailById";
        var model = { //Passing data 
            DocumentId: $(this).attr("data-id"),
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        };
    }
    else {
        url = "/Operations/GetCommunicationTrailById";
        model = { //Passing data 
            DocumentId: $(this).attr("data-id")
        };
    } 

    $.ajax({
        type: "POST", //HTTP POST Method
        url: url, // Controller/View
        data: model,
        success: function (response) {
            if (response !== null && response != undefined && response.model != null && response.model != undefined && response.model.responseStatus.status.toLowerCase() == "success") {
                if (response.model.body != "" && response.model.body != null && response.model.body != undefined) {
                    $("#divemailhtml").html(response.model.body);
                }
                else {
                    $("#divemailhtml").html("Details not available");
                }
                
                $("#spfrom").text(response.model.from);
                $("#spto").text(response.model.to);

                if (response.model.sendStatus.toLowerCase() != "sent") {
                    if ($('#spmailsenton').is('span')) {
                        $("#spmailsenton").text("Not Sent");
                    } 
                    else{
                        $("#spmailsenton").val("Not Sent");
                    }
                }
                else {
                    if ($('#spmailsenton').is('span')) {
                        $("#spmailsenton").text(response.maildt);
                    }
                    else {
                        $("#spmailsenton").val(response.maildt);
                    }                    
                }
                $("#spsubject").text(response.model.subject);
                if (response.model.documentPath !== "" && response.model.documentPath !== null && response.model.documentPath !== undefined) {
                    $("#trAttachment").show();
                    $("#aFilePath").show();
                    $("#attachment").show();
                    $("#aFilePath").removeAttr("href");
                    $("#aFilePath").attr("href", "Download?file=" + response.model.documentPath);
                }
                else {
                    $("#trAttachment").hide();
                    $("#aFilePath").hide();
                    $("#attachment").hide();
                }
                $("#divemailhtml").find(".container").removeClass("container");
                $("#divemailhtml > style").remove();      
            }
            else {
                $("#divemailhtml").text("");
                $("#spfrom").text("");
                $("#spto").text("");
                $("#spmailsenton").text("");
                $("#spsubject").text("");
                $("#aFilePath").removeAttr("href");

                $("#divEmailInfo").hide();
                $("#divSuccessError").show();
                $("#divSuccessError").addClass("alert alert-danger");

                if (response.model.responseStatus.errorMessage !== "" && response.model.responseStatus.errorMessage != undefined && response.model.responseStatus.errorMessage != null) {
                    $("#lblmsg").text(response.model.responseStatus.errorMessage);
                }
                else {
                    $("#lblmsg").text("An error Occurs");
                }
                $("#stMsg").text("Error!");
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

            $("#divemailhtml").text("");
            $("#spfrom").text("");
            $("#spto").text("");
            $("#spmailsenton").text("");
            $("#spsubject").text("");
            $("#aFilePath").removeAttr("href");
            $("#divEmailInfo").hide();

            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");
            $("#lblmsg").text(msg);
            $("#stMsg").text("Error!");
        }
    });
});

$("#btnUpload").click(function () {
    // $.magnificPopup.close();
    var thisparam = this;
    var model = { //Passing data
        QRFID: $("#QRFId").val(),
        QRFPriceID: $("#QRFPriceID").val(),
        AgentTourName: $("#AgentTourName").val(),
        __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
    }; 
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/AgentApproval/GetUploadEmailClient", // Controller/View
        data: model,
        success: function (response) {
            $("#modelcommunitrail-popup").html(response);
            ShowPopup(thisparam);
        },
        error: function (jqXHR, exception, errorThrown) {
            var msg = "";
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
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