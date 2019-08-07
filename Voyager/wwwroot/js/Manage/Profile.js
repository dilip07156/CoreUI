$(document).ready(function () {
    $("#btnChange").on("click", function () {
        $(".pwd").removeAttr("readonly");
        $("#hdnIsPasswordEnabled").val("true");
    });

    $(".updateButton").on("click", function (e) {
        var flag = false;
        var isIsPasswordEnabled = $("#hdnIsPasswordEnabled").val();

        $("#spanPassword").html("");
        $("#spanConfirmPassword").html("");
        if (isIsPasswordEnabled == "true") {
            var txtpassword = $("#Password").val();
            var txtConfimPassword = $("#ConfirmPassword").val();

            if (txtpassword === '' || txtpassword == null) {
                $('#spanPassword').show();

                $("#spanPassword").html("Password is required.")
                flag = true;
            }

            if (txtConfimPassword === '' || txtConfimPassword == null) {
                $('#spanConfirmPassword').show();
                $("#spanConfirmPassword").html("Password is required.")
                flag = true;

            }

            if (txtpassword !== txtConfimPassword) {
                $('#spanConfirmPassword').show();
                $("#spanConfirmPassword").html("Password not matched.")
                flag = true;

            }


        }



        if (flag) {
            e.preventDefault();
        }
        else {
            //SaveMargin();
        }

    });

    $("#btnUpload").on("click", function (e) {
        var _validFileExtensions = ["jpg", "jpeg", "bmp", "png"];

        var ext = $('#files').val().split('.').pop().toLowerCase();
        var sFileName = $('#files').val();
        if (sFileName !== "") {
            if ($.inArray(ext, _validFileExtensions) == -1) {
                alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                return false;
            }
        }
        else {
            alert("Please select the file to Upload.");
            return false;
        }
        return true;
    });


});