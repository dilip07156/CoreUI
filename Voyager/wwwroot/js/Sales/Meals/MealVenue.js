//$(document).ready(function () {
//    ShowHideRoleBasedControls('QRF', 'price-foc-sec');
//});

/*-------------------------------------autocomplete starts here-----------------------------------*/
//$(document).on("keypress", ".bindprodmeals", function (event) {
$(".bindprodmeals").on("keyup", function (event) {
    var key = event.which;
    var ingnore_key_codes = [60, 62];
    if ($.inArray(key, ingnore_key_codes) != -1) {
        event.preventDefault();
    } else {
        var thisparent = $(this).parents(".div-pop-sec");
        var cityname = thisparent.find("#txtCityNameUI").val();
        thisparent.find("#lblApplyAcrossDays").hide();
        thisparent.find("#chkApplyAcrossDays").prop("checked", false);

        if (cityname != "" && cityname != undefined && cityname != null) {
            var ProdType = JSON.stringify("Meal");
            var arr = cityname.split(",");
            var citynm = arr != null && arr[0] != null ? arr[0].trim() : "";
            var countrynm = arr != null && arr[1] != null ? arr[1].trim() : "";
            var data = { ProdName: $(this).val(), ProdType: ProdType, CityName: citynm, CountryName: countrynm };
            InitializeAutoComplete('GetProductNameList', this, 3, data, 'venue');
            if (thisparent.find("[id^=txtVenueName_]").val() != "" && thisparent.find("[name=PlaceHolder]").val() == "true") {
                thisparent.find("#lblApplyAcrossDays").show();
            }
        }
    }
});

function AutoCompleteChanged(thisparam) {
    var thisparent = $(thisparam).parents(".div-pop-sec");
    var ddlmealType = thisparent.find('#ddlMealType').attr("id");
    thisparent.find('#ddlMealType').empty();
    thisparent.find('#ddlMealType').append($("<option></option>").attr("value", '').text('Select Meal Type'));
    var productid = thisparent.find("[name=ProductId]").val();
    thisparent.find("#lblApplyAcrossDays").hide();
    if (productid != "" && productid != null && productid != undefined) {
        if (thisparent.find("[name=PlaceHolder]").val() == "true") {
            thisparent.find("#lblApplyAcrossDays").show();
        }

        $.ajax({
            type: "GET",
            url: "/Meals/GetProductCategoryByParam",
            data: { ProductId: productid, ProductName: null },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                thisparent.find('#ddlMealType').empty();
                thisparent.find('#ddlMealType').append($("<option></option>").attr("value", '').text('Select Meal Type'));
                $.each(result, function (key, value) {
                    thisparent.find('#ddlMealType').append($("<option></option>").val(value.value).html(value.label));
                });
            },
            failure: function (error) {
                alert(error);
            },
            error: function (error) {
                alert(error);
            }
        });
    }
}
/*-------------------------------------autocomplete end here--------------------------------------*/

//meal product category change event
//$(document).on('change', '#ddlMealType', function (e) {
$('#ddlMealType').on('change', function (e) {
    var thisparent = $(this).parents(".div-pop-sec");
    var prodId = thisparent.find("[name=ProductId]").val();
    var catId = $(this).val();
    if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
        $.ajax({
            type: "GET",
            url: "/Meals/GetProductRange",
            data: { ProductId: prodId, CategoryId: catId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var $elem = $('select[name*="MealProductRange[0].PositionRoomsData.SupplementID"]');
                var $elem2 = $('#MealProductRange_0__PositionRoomsData_RoomDetails_0__ProductRangeID');
                $elem.empty(); $elem2.empty();
                $elem.append($("<option></option>").attr("value", '').text('Select'));
                $elem2.append($("<option></option>").attr("value", '').text('Select'));
                if (result != null && result != "") {
                    $("#hfSupplierId").val(result.supplierId);
                    $("#hfSupplierName").val(result.supplierName);

                    $('#SupplementID_0').closest('tr').find('ul li.clonedInput').each(function (iCnt) {
                        if (iCnt > 0) {
                            $(this).closest('tr').find('a.removeRoom').click();
                        }
                    });
                    if (result.prodRangeList != null && result.prodRangeList != undefined) {
                        $.each(result.prodRangeList, function (i, list) {
                            if (list.type.toLowerCase() == 'true') {
                                $elem.append($("<option></option>").attr("value", list.value).text(list.label));
                            }
                            else {
                                $elem2.append($("<option></option>").attr("value", list.value).text(list.label));
                            }
                        });
                    }
                }
            },
            error: function (error) {
                alert(error);
            }
        });
    }
});

$(".VenueContinue").unbind("click");
//popup continue button
//$(document).on('click', '.VenueContinue', function (e) {
$('.VenueContinue').on('click', function (e) {
    CheckSaveVenueDetails(this, "");
});

function SetMealVenueDetails(thisparam, flag) {
    $(".ajaxloader").show();
    var pr = "";
    var foc = "";
    if (flag == "price") {
        pr = "price";
    }
    else if (flag == "foc") {
        foc = "foc";
    }
    else {
        pr = "price";
        foc = "foc";
    }
    var thisparentTr = $(thisparam).parents(".tr-pop-sec");

    var thisparent = $(thisparam).parents(".div-pop-sec");
    thisparent.find("#divSuccessError").hide();
    thisparent.find("#divSuccessError").removeAttr("class");
    thisparent.find("#lblmsg").text("");
    thisparent.find("#stMsg").text("");
    var lastseq = $("ul#clonedRoom_0 li").length;

    var qrfid = thisparent.find("[id^=hfQRFId]").val();
    var positionid = thisparent.find("[id^=hfPositionId]").val();
    var prodid = thisparent.find("[name=ProductId]").val();
    var prodtype = thisparent.find("input[name*='ProductType']").val();
    var hfQuoteRmPasList = thisparent.find("#hfQuoteRmPasList").val();
    var roomSeq = 0;
    var prodrangenm = "";
    var strarray = hfQuoteRmPasList.split(',');
    var notincludedroompass = "";

    var RoomDetails = new Array();
    if (lastseq > 0) {
        for (var i = 0; i < lastseq; i++) {
            roomSeq = thisparent.find("input[name='MealProductRange[0].PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
            prodrangenm = thisparent.find("input[name='MealProductRange[0].PositionRoomsData.RoomDetails[" + i + "].ProductRange']").val();
            RoomDetails.push({
                RoomId: thisparent.find("input[name='MealProductRange[0].PositionRoomsData.RoomDetails[" + i + "].RoomId']").val(),
                RoomSequence: roomSeq,
                ProductCategoryId: thisparent.find("#ddlMealType").val(),
                ProductCategory: thisparent.find("#ddlMealType option[value='" + thisparent.find("#ddlMealType").val() + "']").text(),
                ProductRangeId: thisparent.find("select[name='MealProductRange[0].PositionRoomsData.RoomDetails[" + i + "].ProductRangeID']").val(),
                ProductRange: prodrangenm,
                IsSupplement: thisparent.find("input[name='MealProductRange[0].PositionRoomsData.RoomDetails[" + i + "].IsSupplement']").val(),
                IsDeleted: roomSeq == 0 ? true : false
            });
        }
    }
    var objRm = null;

    for (var j = 0; j < strarray.length; j++) { 

        objRm = RoomDetails.find(a => a.IsDeleted == false && a.ProductRange.toLowerCase().indexOf(strarray[j].toLowerCase()) >= 0);
        if (objRm == null || objRm == undefined) {
            notincludedroompass += strarray[j] + ", ";
        }
    }

    if (notincludedroompass != "" && notincludedroompass != null && notincludedroompass != undefined) {
        $(".ajaxloader").hide();
        notincludedroompass = notincludedroompass.trim().slice(0, -1);
        var ermsg = "Please select the " + notincludedroompass + " Services.";
        //alert(ermsg);
        thisparent.find("#divSuccessError").show();
        thisparent.find("#divSuccessError").addClass("alert alert-danger");
        thisparent.find("#lblmsg").text(ermsg);
        thisparent.find("#stMsg").text("Error!");
    }
    else {
        var mealtype = thisparent.find('.headertextposition').text().trim();

        mealtype = mealtype == "lunch" ? "Lunch" : mealtype == "dinner" ? "Dinner" : mealtype == "tea" ? "Tea" : mealtype == "brunch" ? "Brunch" :
            mealtype == "breakfast" ? "Breakfast" : mealtype == "early morining tea" ? "Early Morning Tea" : mealtype;

        var chkApply = thisparent.find("#chkApplyAcrossDays").is(":checked") ? true : false;
        var venueDetails = { //Passing data
            QRFId: qrfid,
            PositionId: positionid,
            DayID: thisparent.find("[id=hfDayID]").val(),
            RoutingDaysID: thisparent.find("[id=hfRoutingDaysID]").val(),
            PositionSequence: thisparent.find("[id=hfPositionSeq]").val(),
            MealType: mealtype,
            CityID: thisparent.find("[id=txtCityName]").val(),
            CityName: thisparent.find("[id=txtCityNameUI]").val(),
            ProductType: prodtype,
            ProductTypeId: thisparent.find("input[name*='ProductTypeId']").val(),
            ProductId: prodid,
            ProductName: thisparent.find("[name^=ProductName]").val(),
            BudgetCategory: thisparent.find("#ddlMealType option[value='" + thisparent.find("#ddlMealType").val() + "']").text(),
            BudgetCategoryId: thisparent.find("#ddlMealType").val(),
            TransferDetails: thisparent.find("#TransferDetails").val(),
            KeepAs: thisparent.find("#ddlKeepAs").val(),
            TLRemarks: thisparent.find("#txtVTLRemarks").val(),
            OPSRemarks: thisparent.find("#txtVOPSRemarks").val(),
            RoomDetailsInfo: RoomDetails,
            SupplierId: thisparent.find("#hfSupplierId").val(),
            SupplierName: thisparent.find("#hfSupplierName").val(),
            ApplyAcrossDays: thisparent.find("#chkApplyAcrossDays").is(":checked") ? true : false,
            FOC: foc, Price: pr,
            EndTime: thisparent.find("#EndTimeUI").val(),
            StartTime: thisparent.find("#StartTimeUI").val(),
            IsClone: thisparent.find("#IsClone").val(),
            __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val()
        };

        var datasec = thisparent.find(".div-pop-sec").attr("data-sec");
        var hfPositionId = thisparent.prev('tr').find('td.active').find("[datasechf^=" + datasec + "]").attr("id");
        $.ajax({
            type: "POST", //HTTP POST Method
            url: "/Meals/VenueMealsDetails", // Controller/View
            data: venueDetails,
            global: false,
            // global: chkApply ? true : false,
            success: function (result) {
                thisparent.find("#divSuccessError").show();
                if (result != null && result != "") {
                    thisparent.find("#hfQRFId").val(result.qrfid);
                    thisparent.find("#hfPositionId").val(result.positionId);

                    if (result.responseStatus.status == "Success") {
                        if (chkApply == false)
                            thisparentTr.prev('tr').find('td.active').find("[id^=viewdetails]").css("display", "block");

                        if (hfPositionId != undefined && hfPositionId != "" && hfPositionId != null) {
                            $("#" + hfPositionId).val(result.positionId);
                        }
                        var roomDetailsInfo = result.roomDetailsInfo;
                        var roomobj, roomSeq, roomId, roomidnew = "";
                        var lastseq = thisparent.find("ul[id*='clonedRoom_'] li").length;
                        if (lastseq > 0) {
                            for (var i = 0; i < lastseq; i++) {
                                roomobj = thisparent.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']");
                                roomSeq = thisparent.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                                roomId = roomobj.val();
                                if (roomId == '' && roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0) {
                                    roomidnew = roomDetailsInfo.find(a => a.roomSequence == roomSeq);
                                    if (roomidnew != undefined && roomidnew != "" && roomidnew != null) {
                                        roomobj.val(roomidnew.roomId);
                                    }
                                }
                            }
                        }
                        if (flag == "price") {
                            thisparent.find("#divSuccessError").hide();
                            $("#divPrices").show();
                            $("#divPrices").prev(".clsfocprice").addClass("active");
                            GetPositionPricesPartView("#divPrices", qrfid, result.positionId, prodid);
                        }
                        else if (flag == "foc") {
                            thisparent.find("#divSuccessError").hide();
                            $("#divFOC").show();
                            $("#divFOC").prev(".clsfocprice").addClass("active");
                            GetPositionFOCPartView("#divFOC", qrfid, positionid, prodid);
                        }
                        else {
                            $(".ajaxloader").hide();
                            thisparent.find("#divSuccessError").addClass("alert alert-success");
                            thisparent.find("#lblmsg").text(result.responseStatus.errorMessage);
                            thisparent.find("#stMsg").text("Success!");
                        }
                    }
                    else {
                        $(".ajaxloader").hide();
                        thisparent.find("#divSuccessError").addClass("alert alert-danger");
                        thisparent.find("#lblmsg").text(result.responseStatus.errorMessage);
                        thisparent.find("#stMsg").text("Error!");
                    }
                }
                else {
                    $(".ajaxloader").hide();
                    thisparent.find("#divSuccessError").addClass("alert alert-danger");
                    thisparent.find("#lblmsg").text("Details Not Saved.");
                    thisparent.find("#stMsg").text("Error!");
                }
                if (chkApply && result.positionDetails != null && result.positionDetails != "" && result.positionDetails.length > 0) {
                    //window.location.href = "/Meals/Meals?QRFId=" + $("#hfQRFId").val();
                    $('.meal-list').find('td[data-label="' + mealtype + '"]').each(function () {
                        var res = result.positionDetails.find(a => a.days == $(this).parents("tr").find("[id^=DayID_]").val());
                        if (res != null && res != "" && res != undefined) {
                            $(this).find('input[type="checkbox"]').prop('checked', true);
                            $(this).find("[id^=viewdetails_]").removeAttr('style');
                            $(this).find("[id^=viewdetails_]").attr('style', 'cursor: pointer; display: block;');
                            $(this).find("[id^=hf]").val(res.positionID);
                        }
                    });
                }
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = "";
                thisparent.find("#divSuccessError").show();
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
                thisparent.find("#divSuccessError").addClass("alert alert-danger");
                thisparent.find("#lblmsg").text(msg);
                thisparent.find("#stMsg").text("Error!");
                // alert(msg);
                $(".ajaxloader").hide();
                if (chkApply) {
                    window.location.href = "/Meals/Meals?QRFId=" + $("#hfQRFId").val();
                }
            }
        });
    }
}

//popup add price button
//$(".clsAddPrice").unbind("click");
//$(".clsAddPrice").click(function () {
//$(document).on('click', '.clsfocprice', function (e) {
$('.clsfocprice').on('click', function (e) {
    CheckSaveVenueDetails(this, "focprice");
});

function CheckSaveVenueDetails(thisparam, status) {
    var thisparent = $(thisparam).parents(".div-pop-sec");
    var dataname = $(thisparam).attr("data-name");

    var flag = true;

    var StartTimeUI = $("#StartTimeUI").val();
    var EndTimeUI = $("#EndTimeUI").val();

    if (StartTimeUI != "" && EndTimeUI != "" && EndTimeUI != "00:00" && StartTimeUI != "00:00") {
        //convert both time into timestamp
        var st = new Date("November 13, 2013 " + StartTimeUI);
        st = st.getTime();
        var et = new Date("November 13, 2013 " + EndTimeUI);
        et = et.getTime();

        if (st > et) {
            alert('Start Time should be less than End Time.');
            return false;
        }
        else if (st == et) {
            alert('Start Time and End Time can not be same.');
            return false;
        }
    }
    else {
        if (StartTimeUI == "" || StartTimeUI == "00:00") {
            flag = false;
            thisparent.find("#lblStartTime").text("*");
        }
        if (EndTimeUI == "" || EndTimeUI == "00:00") {
            flag = false;
            thisparent.find("#lblEndTime").text("*");
        }
    }

    if (!(RoomValidation("#frmMeals")))
        flag = false;

    if (thisparent.find("[id^=txtCityNameUI]").val() == "") {
        flag = false;
        thisparent.find("#lblcitynm").text("*");
    }

    if (thisparent.find("[id^=txtVenueName_]").val() == "") {
        flag = false;
        thisparent.find("#lblveneuenm").text("*");
    }
    if (thisparent.find("#ddlMealType").val() == "") {
        flag = false;
        thisparent.find("#lblmealtype").text("*");
    }

    if (flag) {
        thisparent.find("#lblveneuenm").text("");
        thisparent.find("#lblmealtype").text("");
        thisparent.find("#lblcitynm").text("");
        SetMealVenueDetails(thisparam, dataname);
    }
    else {
        if (status == "focprice") {
            var divPrices = '#' + $(thisparam).siblings("div").attr('id');
            $(divPrices).hide();
            if (dataname == "foc") {
                $(".clsAddFOC").removeClass("active");
            }
            else {
                $(".clsAddPrice").removeClass("active");
            }
        }
        alert('Please fill the mandatory fields!');
        return false;
    }
}

function CheckRoomInfoFromQuote(QRFID) {

}

$(document).on('change', 'input.bindCity', function () {
    var curRow = $(this).closest('tr');
    curRow.find("input.clsProductID").val('');
    curRow.find("input.bindprodmeals").val('');
    curRow.find("input[name*='SupplierID']").val('');
    curRow.find("input[name*='SupplierName']").val('');

    curRow.find("select[name*='BudgetCategory']").empty().append($("<option></option>").attr("value", '').text('Select'));

    //Remove all product ranges except one before filling defaults
    curRow.find('ul li.clonedInput').each(function (iCnt) {
        if (iCnt > 0) {
            $(this).closest('tr').find('a.removeRoom').click();
        }
    });
    curRow.find("select.clsRoomType").empty().append($("<option></option>").attr("value", '').text('Select'));
    curRow.find("select.clsSupplement").empty().append($("<option></option>").attr("value", '').text('Select'));
});