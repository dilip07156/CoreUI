
$(document).ready(function () {
    $('#Password').keyup(function () {
        $('#result').html(checkStrength($('#Password').val()));
    });

    function checkStrength(password) {

        var strength = 0
        if (password.length < 6) {
            $('#result').removeClass()
            $('#result').addClass('regshort')
            return 'Too short'
        }
        if (password.length > 7) strength += 1
        // If password contains both lower and uppercase characters, increase strength value.
        if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1
        // If it has numbers and characters, increase strength value.
        if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) strength += 1
        // If it has one special character, increase strength value.
        if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
        // If it has two special characters, increase strength value.
        if (password.match(/(.*[!,%,&,@,#,$,^,*,?,_,~].*[!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
        // Calculated strength value, we can return messages
        // If value is less than 2
        if (strength < 2) {
            $('#result').removeClass()
            $('#result').addClass('regweak')
            return 'Weak'
        } else if (strength == 2) {
            $('#result').removeClass()
            $('#result').addClass('reggood')
            return 'Good'
        } else {
            $('#result').removeClass()
            $('#result').addClass('regstrong')
            return 'Strong'
        }
    }

    $("#ddlStartPage").on("change", function () {

        var ddlStartPageId = $(this).find("option:selected").val();
        var startpage = $(this).find("option:selected").text();
        if (ddlStartPageId == null)
            $("#StartPage").val("");
        else
            $("#StartPage").val(startpage);
    });

    $(".btnSaveHome").click(function () {
        var pattern = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var web = /^(?:http(s) ?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;

        var IsValidFlag = true;

        var TITLE = $("#TITLE").val();
        if (TITLE == '') {
            $("#spanTitle").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var Telephone = $("#Telephone").val();
        if (Telephone == '') {
            $("#spanTelephone").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var FIRSTNAME = $("#FIRSTNAME").val();
        if (FIRSTNAME == '') {
            $("#spanFIRSTNAME").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var LastNAME = $("#LastNAME").val();
        if (LastNAME == '') {
            $("#spanLastNAME").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");


        var MAIL = $("#MAIL").val();
        var span = $("#spanMAIL").text();
        if (!pattern.test(MAIL)) {
            $("#spanMAIL").text("Enter valid Email Address");
            IsValidFlag = false;
        }
        else if (MAIL == '') {
            $("#spanMAIL").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");


        var WEB = $("#WEB").val();
        if ((WEB != "" && WEB != null && WEB != undefined) && !web.test(WEB)) {
            $("#spanWEB").text("Enter valid Web Address");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var Status = $("#ddlStatus").find("option:selected").val();
        if (Status == '') {
            $("#spanStatus").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        //var StartPage = $("#ddlStartPage").find("option:selected").val();
        //if (StartPage == '') {
        //    $("#spanStartPage").text("*");
        //    IsValidFlag = false;
        //}
        //else
        //    $(this).siblings("span").text("");

        if (!IsValidFlag)
            return false;

        $(".ajaxloader").show();
        //var cid = $("#Contact_Id").val();
        var model = $("#frmHome").serialize();
        $.ajax({
            type: "POST",
            url: "/Agent/SaveNewContact",
            data: model,
            global: false,
            success: function (response) {
                //$("#frmHome #Contact_Id").val(response.contactId);
                //$("#frmSystem #Contact_Id").val(response.contactId);
                //$("#frmSystem #UserName").val(response.username);
                $("#Contacts").click();

                if (response.status == "error") {
                    $("#frmHome #Contact_Id").val(response.contactId);
                    $("#frmSystem #Contact_Id").val(response.contactId);
                    $("#frmSystem #UserName").val(response.username);
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! ' + response.responseText + ' </strong></div>';
                    $("#frmHome #message").html(successmsg);
                }
                else {
                    if (response.status == "error") {
                        var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! ' + response.responseText + ' </strong></div>';
                        $("#frmHome #message").html(successmsg);
                    }
                    else {
                        var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! Details saved successfully.</strong></div>';
                        $("#frmHome #message").html(successmsg);
                    }
                    $("#frmHome #Contact_Id").val(response.contactId);
                    $("#frmSystem #Contact_Id").val(response.contactId);
					$("#frmSystem #UserName").val(response.username);
					$("#spanTitle").text("");
					$("#spanTelephone").text("");
                    $("#spanFIRSTNAME").text("");
                    $("#spanLastNAME").text("");
                    $("#spanMAIL").text("");
                    $("#spanWEB").text("");
                    $("#spanStatus").text("");
                    //$("#spanStartPage").text("");
                    $(".btnCreateUser,.btnSendReset,.btnRemoveUser,.btnSaveSystem").removeAttr("disabled");
                }
                $(".ajaxloader").hide();
            },
            error: function () {
                $(".ajaxloader").hide();
            }
        });
    });

    $(".btnSaveSystem").click(function () {


        var pwd = $(".clsPassword").val();
        var conpwd = $(".clsConfirmPassword").val();

        if ((pwd != null || pwd != undefined || pwd != "") && (conpwd != null || conpwd != undefined || conpwd != "") && (pwd != conpwd)) {
            var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! Password and Confirm Password does not match</strong></div>';
            $("#frmSystem #message").html(successmsg);
            return false;
        }

        $(".ajaxloader").show();
        var model = $("#frmSystem").serialize();
        $.ajax({
            type: "POST",
            url: "/Agent/SaveNewContact",
            data: model,
            global: false,
            success: function (response) {
                $("#Contacts").click();
                if (response.status == "error") {
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! ' + response.responseText + '</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                else if (response.status == "error1") {
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! ' + response.responseText + ' </strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                else {
                    var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! ' + response.responseText + '</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                $(".ajaxloader").hide();
            },
            error: function () {
                $(".ajaxloader").hide();
            }
        });
    });

    $(".btnCreateUser").click(function () {
        var IsValidFlag = true;

        var UserName = $("#UserName").val();
        if (UserName == '') {
            $("#spanUserName").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var Password = $("#Password").val();
        if (Password == '') {
            $("#spanPwd").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");

        var ConfirmPassword = $("#ConfirmPassword").val();
        if (ConfirmPassword == '') {
            $("#spanConfirmPwd").text("*");
            IsValidFlag = false;
        }
        else
            $(this).siblings("span").text("");


        if (Password != null && Password != undefined && Password != "") {

            var pwdStrengthVal = checkStrength($('#Password').val())
            if (pwdStrengthVal == "Strong")
                IsValidFlag = true;
            else {
                var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! Please enter valid Password. </strong></div>';
                $("#frmSystem #message").html(successmsg);
                IsValidFlag = false;
            }
        }

        if ((Password != null || Password != undefined || Password != "") && (ConfirmPassword != null || ConfirmPassword != undefined || ConfirmPassword != "") && (Password != ConfirmPassword)) {
            var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! Password and Confirm Password does not match</strong></div>';
            $("#frmSystem #message").html(successmsg);
            return false;
        }

        if (!IsValidFlag)
            return false;

        //var companyId = $("#Company_Id").val();
        //var contactId = $("#Contact_Id").val();

        var model = $("#frmSystem").serialize();
        $.ajax({
            type: "POST",
            url: "/Agent/CreateUser",
            //data: {CompanyId: companyId, ContactId: contactId },
            data: model,
            success: function (response) {

                $("#frmSystem #User_Id").val(response.userId);
                $("#Contacts").click();

                if (response.status == "error") {
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!' + response.responseText + '</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                else {

                    var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! User is created.</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                    $("#spanUserName").text("");
                    $("#spanPwd").text("");
                    $("#spanConfirmPwd").text("");
                    $(".btnCreateUser").hide();
                    $("#IsUserExists").val(true);
                    $(".frmSystembtn .btnSendReset").show();
                    $(".frmSystembtn .btnRemoveUser").show();

                    //$(".frmSystembtn .btnSendReset").css('display', true);
                    //$(".btnSendReset").show();
                }
                $(".ajaxloader").hide();
            },
            error: function () {
                $(".ajaxloader").hide();
            }
        });
    });

    $(".btnRemoveUser").click(function () {
        var model = $("#frmSystem").serialize();
        $.ajax({
            type: "POST",
            url: "/Agent/RemoveUser",
            //data: {CompanyId: companyId, ContactId: contactId },
            data: model,
            success: function (response) {

                $("#frmSystem #User_Id").val(response.userId);
                $("#Contacts").click();

                if (response.status == "error") {
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error! </strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                else {
                    var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! User is deactivated successfully.</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                $(".ajaxloader").hide();
            },
            error: function () {
                $(".ajaxloader").hide();
            }
        });
    });

    $(".btnSendReset").click(function () {
        var UserName = $("#UserName").val();
        $.ajax({
            type: "POST",
            url: "/Agent/SendResetUserPassword",
            //data: {CompanyId: companyId, ContactId: contactId },
            data: { UserName: UserName },
            success: function (response) {

                $("#frmHome #User_Id").val(response.userId);
                $("#Contacts").click();

                if (response.status == "error") {
                    var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!' + response.msg + '</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                else {
                    var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! Password Reset and sent via email.</strong></div>';
                    $("#frmSystem #message").html(successmsg);
                }
                $(".ajaxloader").hide();
            },
            error: function () {
                $(".ajaxloader").hide();
            }
        });
    });

    $("#aTargets").click(function (e) {
        var Company_Id = $("#Company_Id").val();
        var Contact_Id = $("#Contact_Id").val();
        $("#ulContactsTab").find("a").removeClass("active");
        $("#ulContactsTab").find("li").removeClass("active");
        $(".tab-content").find(".active").removeClass("active in");
        $("#aTargets").parent("li").addClass("active");
        $("#divTargets").addClass("active in");
        $("#divContactTargetDetails").load('/Agent/GetCompanyTargets', { CompanyId: Company_Id, ContactId: Contact_Id });
    });
});
