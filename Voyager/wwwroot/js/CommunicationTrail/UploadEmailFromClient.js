$(document).ready(function () {
    CKEDITOR.replace("editorEmailHtml", {
        height: '300px'
    });
    $('.input-append.date').find('input.span2').datepicker({
        changeMonth: true,
        changeYear: true,
        showOn: "both",
        firstDay: 0, // Start with Monday
        dayNamesMin: "Sun Mon Tue Wed Thu Fri Sat".split(" "),
        dateFormat: "dd/M/yy",
        beforeShow: function (el, dp) {
            $(el).parent().append($('#ui-datepicker-div'));
            $('#ui-datepicker-div').hide();
        }
    });
});

$("#savefromclient").click(function () {
    $("#divSuccessError").hide();
    var pattern = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    var flag = true;
    $("#sptxtTo").text("");
    $("#sptxtFrom").text("");
    $("#sptxtCC").text("");
    $("#sptxtBCC").text("");
    $("#sptxtSubject").text("");
    $("#sptxtMailType").text("");
    $("#sptxtMailStatus").text("");
    $("#sptxtRemarks").text("");
    $("#sptxtmailsenton").text("");
    $("#sptxtMailSentBy").text("");
    $("#sptxtTime").text("");
    $("#speditorEmailHtml").text("");

    var from = $("#txtFrom").val();
    if (from == "" || !pattern.test(from)) {
        flag = false;
        $("#sptxtFrom").text("*");
    }

    var to = $("#txtTo").val();
    if (to == "" || !pattern.test(to)) {
        flag = false;
        $("#sptxtTo").text("*");
    }

    var cc = $("#txtCC").val();
    if (cc == "") {
        flag = false;
        $("#sptxtCC").text("*");
    }
    else {
        var ccarray = cc.split(";");
        $.each(ccarray, function (i) {
            if (!pattern.test(ccarray[i])) {
                flag = false;
                $("#sptxtCC").text("*");
                return;
            }
        });
    }

    var bcc = $("#txtBCC").val();
    if (bcc == "") {
        flag = false;
        $("#sptxtBCC").text("*");
    }
    else {
        var bccarray = bcc.split(";");
        $.each(bccarray, function (i) {
            if (!pattern.test(bccarray[i])) {
                flag = false;
                $("#sptxtBCC").text("*");
                return;
            }
        });
    }

    var subject = $("#txtSubject").val();
    if (subject == "") {
        flag = false;
        $("#sptxtSubject").text("*");
    }

    //var mailtype = $("#txtMailType").val();
    //if (mailtype == "") {
    //    flag = false;
    //    $("#sptxtMailType").text("*");
    //}

    var mailstatus = $("#txtMailStatus").val();
    if (mailstatus == "") {
        flag = false;
        $("#sptxtMailStatus").text("*");
    }

    var mailsenton = $("#txtmailsenton").val();
    if (mailsenton == "") {
        flag = false;
        $("#sptxtmailsenton").text("*");
    }
    else if (!ValidateDate('#txtmailsenton')) {
        flag = false;
        $("#sptxtmailsenton").text("*");
    }

    var time = $("#txtTime").val();
    if (time == "") {
        flag = false;
        $("#sptxtTime").text("*");
    }

    var mailSentBy = $("#txtMailSentBy").val();
    if (mailSentBy == "") {
        flag = false;
        $("#sptxtMailSentBy").text("*");
    }

    var editorEmailHtml = CKEDITOR.instances.editorEmailHtml.document.getBody().getChild(0).getText(); 
    if (editorEmailHtml == '' || editorEmailHtml == null || editorEmailHtml == undefined || editorEmailHtml == "\n") {
        flag = false;
        $("#speditorEmailHtml").text("*");
    } 

    if (!flag) {
        alert("Please fill the madatory fields.");
    }
    else {
        var model = { //Passing data
            QRFID: $("#QRFId").val(),
            QRFPriceID: $("#QRFPriceID").val(),
            AgentTourName: $("#AgentTourName").val(),
            From: $("#txtFrom").val(),
            To: $("#txtTo").val(),
            CC: $("#txtCC").val(),
            BCC: $("#txtBCC").val(),
            Subject: $("#txtSubject").val(),
            MailSent: $("#chkIsMailSent").is(":checked") ? "1" : "0",
            MailType: "User Reply",//$("#txtMailType").val(),
            MailSentBy: $("#txtMailSentBy").val(),
            MailSentOn: $("#txtmailsenton").val(),
            Time: $("#txtTime").val(),
            Remarks: $("#txtRemarks").val(),
            EmailHtml: CKEDITOR.instances['editorEmailHtml'].getData(),
            MailStatus: $("#txtMailStatus").val(),
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        }; 
        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/AgentApproval/SetUploadEmailClient", // Controller/View
            data: model,
            success: function (response) {
                $("#divSuccessError").show();
                if (response != null && response != undefined && response != "" && response.status != null && response.status != undefined
                    && response.status.toLowerCase() == "success") { 
                    $("#divSuccessError").addClass("alert alert-success");
                    $("#lblmsg").text(response.msg);
                    $("#stMsg").text(response.status + "! ");
                }
                else { 
                    $("#divSuccessError").addClass("alert alert-danger");
                    $("#lblmsg").text(response.msg);
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
                $("#divSuccessError").show();
                $("#divSuccessError").addClass("alert alert-danger");
                $("#lblmsg").text(msg);
                $("#stMsg").text("Error!");
            }
        });
    }
}); 