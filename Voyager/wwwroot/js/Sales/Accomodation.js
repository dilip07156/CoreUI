$(document).ready(function () { 
    $(".accomdefmealplan").click(function () {
        $("#divSuccessError").hide();
        $("#divSuccessError").find("#lblmsg").text("");
        $("#divSuccessError").find("#stMsg").text("");
        $("#divSuccessError").find("#divSuccessError").removeAttr("class");
        $("#ddlAccomMealPlan").val("BB");
        $("#accomdefmealplan-popup").show();
    });

    $(".saveaccomdefmealplan").click(function () {
        var mealplan = $(this).parents("#accomdefmealplan-popup").find(".clsAccomMealPlan").val();
        var model = { QRFId: $("#QRFId").val(), ProductType: 'Hotel', MealType: mealplan, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };

        $.ajax({
            type: "POST",
            url: "/Accomodation/SetDefaultMealPlan",
            data: model,
            success: function (response) {
                $("#divSuccessError").show();
                if (response != null && response != undefined && response.status != "" && response.status != null
                    && response.msg != "" && response.msg != null) {
                    $("#divSuccessError").find("#stMsg").text(response.status + "!");
                    $("#divSuccessError").find("#lblmsg").text("Default Meal Plan " + response.msg);

                    if (response.status.toLowerCase() == "success") {
                        $("#divSuccessError").addClass("alert alert-success");
                        $(".clsAccomMealPlan").val(mealplan);
                    }
                    else if (response.status.toLowerCase() == "error") {
                        $("#divSuccessError").addClass("alert alert-danger");
                    }
                    else if (response.status.toLowerCase() == "failure") {
                        $("#divSuccessError").addClass("alert alert-danger");
                    }
                }
                else {
                    $("#divSuccessError").find("#stMsg").text("Error!");
                    $("#divSuccessError").find("#lblmsg").text("Not Updated");
                }
            },
            error: function (response) {
                alert(response.statusText);
            }
        });

    });

    $("clonedHotel_00 .clsRoomType").html('');

    var regexForNameRoom = /^(.+?)(\d+)(.+?)(\d+)(.+?)$/i;

    var cloneIndexHotel = $(".clonedHotel").length - 1;
    var regex = /^(.+?)(\d+)$/i;
    var regexForNameHotel = /^(.+?)(\d+)(.+?)$/i;
    //$(".bindCity").on("keyup", getCityNames);
    //$('.bindAccoCity').on('keyup', function (e) {
    //    var QRFId = $("#QRFId").val();
    //    InitializeAutoComplete('GetCityName', this, 3, { term: this.value, QRFID: QRFId }, "");
    //});

    //$(".bindChain").on("keypress", getChainNames);

    $('.bindChain').on("keyup", function (e) {
        var param = { term: this.value };
        InitializeAutoComplete('GetChainName', this, 3, param, "");
    });

    $(".bindHotelName").on("keypress", getHotelNames);
    // $(".bindHotelName").on("keyup", rsetHotelField);
    $("a.cloneHotel").on("click", cloneHotel);
    $("a.removeHotel").on("click", removeHotel);


    $(".SaveAccomodation").click(function () {
        var flag = true;
        var result = true;

        $(".tblAccomodationData .clonedHotel").each(function (index) {
            result = currentRowValidation($(this));
            if (result == false)
                flag = result;
        });
        if (!flag) {
            alert('Please fill the mandatory fields before adding new record!');
            return false;
        }

        $(".ajaxloader").show();
        var model = $("#frmAccomodation").serialize();
        model += '&FOC=foc&Price=price';

        $.ajax({
            type: "POST",
            url: "/Accomodation/Accomodation",
            data: model,
            global: false,
            //contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.status == 'Success') {
                    var QRFId = $("#QRFId").val();
                    var PositionId = $("#PositionId").val();
                    GetAccomodationData(QRFId, PositionId, response.errorMessage, "", $("#IsClone").val());

                }
                else if (response.status.toLowerCase() == 'invalid') {
                    $(".ajaxloader").hide();
                    alert('Accomodation cities not matching with routing!!!');
                }
                else {
                    var QRFId = $("#QRFId").val();
                    GetAccomodationData(QRFId, "", "", response.errorMessage);
                }
            },
            error: function (response) {
                alert(response.statusText);
                $(".ajaxloader").hide();
            }
        });

    });

    function cloneHotel() {
        var flag = true;
        var result = true;
        $(".tblAccomodationData .clonedHotel").each(function (index) {
            result = currentRowValidation($(this));
            if (result == false)
                flag = result;
        });
        if (!flag) {
            alert('Please fill the mandatory fields before adding new record!');
            return false;
        }
        //if (!$('#frmAccomodation').valid()) {
        //    alert('Please fill the mandatory fields before adding new record!');
        //    return false;
        //}
        var currentRow = "#" + $(this).parents(".clonedHotel").attr('id');
        $("#clonedHotel_00").clone(true, true)
            .insertAfter(currentRow)
            .attr("id", "clonedHotel_" + cloneIndexHotel)
            .slideDown()
            .find("*")
            .each(function () {
                var id = this.id || "";
                var match = id.match(regex) || [];
                if (match.length == 3) {
                    //if (!(match[1] == "clonedInput_" || match[1] == "CitySequence_" || match[1] == "FromCity_" || match[1] == "FromCityUI_" || match[1] == "spanValidationMsg_"))
                    this.id = match[1] + cloneIndexHotel;
                    if (match[1] == "IsLocalGuide_")
                        $(this).removeAttr("Checked");
                    if (match[1] == "spanValidationMsg_") {
                        var dataValMsgFor = $(this).attr("data-valmsg-for");
                        $(this).attr("data-valmsg-for", dataValMsgFor.replace('AccomodationData[0]', 'AccomodationData[' + cloneIndexHotel + ']'));
                    }
                }

                var name = this.name || "";
                var match = name.match(regexForNameHotel) || [];
                if (match.length == 4) {
                    this.name = match[1] + cloneIndexHotel + match[3];
                }
            })
            .find('input').val('')
            .find('select').val('Select');

        jQuery("#clonedHotel_" + cloneIndexHotel + " div").each(function (index) {
            jQuery(this).css("display", "none");
        });

        $("#clonedHotel_" + cloneIndexHotel + " div").slideDown();

        var sequenceId = $("#clonedHotel_" + cloneIndexHotel + " .clonedInput").find("input[type=hidden]")[0].id;
        $("#" + sequenceId).val("1");

        $("#clonedHotel_" + cloneIndexHotel + " .collapse-container").css("display", "none");
        $("#clonedHotel_" + cloneIndexHotel + " .clsInterConnectingInfo").css("display", "none");
        $("#clonedHotel_" + cloneIndexHotel + " .tooltip-cont").css("display", "none");

        $("#clonedHotel_" + cloneIndexHotel).find(".clsRoomType").html('');
        $("#clonedHotel_" + cloneIndexHotel).find(".clsRoomType").append($("<option></option>").val(null).html("Select"));
        $("#clonedHotel_" + cloneIndexHotel).find(".clsSupplement").html('');
        $("#clonedHotel_" + cloneIndexHotel).find(".clsSupplement").append($("<option></option>").val(null).html("Select"));
        $("#clonedHotel_" + cloneIndexHotel).find(".clsIsSupplement").val('false');

        $("#clonedHotel_" + cloneIndexHotel).find(".clsKeepAs").val('Included');
        $("#clonedHotel_" + cloneIndexHotel).find(".clsMealPlan").val('BB');
        $("#clonedHotel_" + cloneIndexHotel).find(".DayName").val('Day 1');
        $("#clonedHotel_" + cloneIndexHotel).find(".clsStartTime").val('20:00');
        $("#clonedHotel_" + cloneIndexHotel).find(".clsEndTime").val('08:30');

        $("#clonedHotel_" + cloneIndexHotel + " .clsDate")
            .removeClass('hasDatepicker')
            .removeData('datepicker')
            .unbind()
            .datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "both", 
                dateFormat: "dd/mm/yy"
            });

        cloneIndexHotel++;
        var form = $("#frmAccomodation");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        bindHotelRowSequence();
    }

    function removeHotel() {
        var clonedInputLen = $(".tblAccomodationData .clonedHotel").length;
        if (clonedInputLen == 1) {
            alert('Cannot delete this record.');
            return false;
        }

        var sequenceId = $(this).parents(".clonedHotel").find("input[type=hidden]")[0].id;
        $("#" + sequenceId).val("0");

        var cloneId = "#" + $(this).parents(".clonedHotel")[0].id;
        $(cloneId + " div").slideUp();
        $(cloneId + " td").slideUp();

        $(this).parents(".clonedHotel").removeClass("clonedHotel");
        bindHotelRowSequence();
    }

    function bindHotelRowSequence() {
        jQuery(".tblAccomodationData .clonedHotel").each(function (index) {
            var seqId = "#" + jQuery(this).find("input[type=hidden]")[0].id;
            $(seqId).val(index + 1);
        });
    }

    function getChainNames() {
        var actionUrl = "GetChainName";
        var controlId = "#" + $(this).attr('id');
        var hdncontrolId = "#hdn" + $(this).attr('id');

        if (actionUrl != undefined && actionUrl != "") {
            var id = controlId;
            id = id.replace('UI', '').replace('#', '');
            $(controlId).autocomplete({
                source: actionUrl,
                minLength: 3,
                select: function (event, ui) {
                    if (ui != null) {
                        $(controlId).val(ui.item.label);
                        $(hdncontrolId).val(ui.item.label);
                        $("#" + id).val(ui.item.value);
                        $(".ui-menu-item").hide();
                        $(".ui-menu").css("border", "none");
                        event.preventDefault();
                    }
                },
                focus: function (event, ui) {
                    event.preventDefault();
                    $(controlId).val(ui.item.label);
                }
            }).keyup(function (e) {
                if (e.which === 13 || e.which === 27) {
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                }
                else if (e.which === 38 || e.which === 40) {
                }
            });
        }
    }

    function getHotelNames() {

        var actionUrl = "GetHotelName";
        var controlId = "#" + $(this).attr('id');
        var hdncontrolId = "#hdn" + $(this).attr('id');
        var currentRow = "#" + $(this).parents(".clonedHotel").attr('id');
        var ProductTypeId = "#" + $(this).parents(".clonedHotel").find(".clsProductTypeId").attr('id');
        var ProductType = "#" + $(this).parents(".clonedHotel").find(".clsProductType").attr('id');

        var StarRatingDef = "#" + $(this).parents(".clonedHotel").find(".clsStarRatingDef").attr('id');
        var CategoryIdDef = "#" + $(this).parents(".clonedHotel").find(".clsCategoryIdDef").attr('id');
        var CategoryDef = "#" + $(this).parents(".clonedHotel").find(".clsCategoryDef").attr('id');
        var LocationDef = "#" + $(this).parents(".clonedHotel").find(".clsLocationDef").attr('id');
        var ChainIdDef = "#" + $(this).parents(".clonedHotel").find(".clsChainIdDef").attr('id');
        var ChainDef = "#" + $(this).parents(".clonedHotel").find(".clsChainDef").attr('id');

        var locvalue = $(this).parents(".clonedHotel").find(".clsLocation").val();
        var catvalue = $(this).parents(".clonedHotel").find(".clsCategory").val();
        var starratingvalue = $(this).parents(".clonedHotel").find(".clsStarRating").val();
        var chainvalue = $(this).parents(".clonedHotel").find(".clsChain").val();

        if ($(controlId).val().length >= 2) {
            var obj = {};
            obj.ProdType = JSON.stringify("Hotel");
            obj.Location = locvalue;
            //obj.Chain = $(this).parents(".clonedHotel").find(".clsChain").val();
            obj.Chain = chainvalue;
            var prodcatidval = catvalue.split('|');
            obj.ProductCategoryID = prodcatidval[0];
            obj.ProductCategoryName = prodcatidval[1];
            obj.StarRating = starratingvalue;

            var CityName = $(this).parents(".clonedHotel").find(".clsCity").val();
            if (CityName != '') {
                var CityNameArr = CityName.split(',');
                obj.CityName = CityNameArr[0].trim();
                obj.CountryName = CityNameArr[1].trim();
            } else {
                obj.CityName = '';
                obj.CountryName = '';
            }
        }

        var id = controlId;
        id = id.replace('UI', '').replace('#', '');
        $(controlId).autocomplete({
            //source: actionUrl,
            source: function (request, response) {
                $.getJSON(
                    actionUrl,
                    { ProdName: request.term, ProdType: obj.ProdType, Location: obj.Location, Chain: obj.Chain, ProductCategoryName: obj.ProductCategoryName, ProductCategoryID: obj.ProductCategoryID, StarRating: obj.StarRating, CityName: obj.CityName, CountryName: obj.CountryName },
                    response
                );
            },
            minLength: 3,
            select: function (event, ui) {
                if (ui != null) {
                    $(controlId).val(ui.item.label);
                    //$(hdncontrolId).val(ui.item.label);
                    $("#" + id).val(ui.item.value);
                    $(ProductTypeId).val(ui.item.typeId);
                    $(ProductType).val(ui.item.type);
                    $(ChainIdDef).val(ui.item.chainid);
                    $(ChainDef).val(ui.item.chain);
                    $(LocationDef).val(ui.item.location);
                    $(StarRatingDef).val(ui.item.starrating);
                    $(CategoryIdDef).val(ui.item.categoryid);
                    $(CategoryDef).val(ui.item.category);

                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                    bindRoomType(ui.item.value, currentRow);

                    if (chainvalue == "" || chainvalue == undefined) {
                        $(this).parents(".clonedHotel").find(".hfChain").val($(ChainIdDef).val());
                        $(this).parents(".clonedHotel").find(".clsChain").val($(ChainDef).val());
                    }
                    if (starratingvalue == "" || starratingvalue == undefined) {
                        var ddlSR = $(this).parents('.clonedHotel').find('.clsStarRating');
                        if ($("#" + ddlSR.attr('id') + " option[value='" + $(StarRatingDef).val() + "']").length > 0)
                            ddlSR.val($(StarRatingDef).val());
                    }
                    if (locvalue == "" || locvalue == undefined) {
                        var ddlLOC = $(this).parents('.clonedHotel').find('.clsLocation');
                        if ($("#" + ddlLOC.attr('id') + " option[value='" + $(LocationDef).val() + "']").length > 0)
                            ddlLOC.val($(LocationDef).val());
                    }
                    if (catvalue == "" || catvalue == undefined) {
                        var ddlCAT = $(this).parents('.clonedHotel').find('.clsCategory');
                        if ($("#" + ddlCAT.attr('id') + " option:contains('" + $(CategoryDef).val() + "') ").length > 0)
                            ddlCAT.val($("#" + ddlCAT.attr('id') + " option:contains('" + $(CategoryDef).val() + "') ").val());
                    }

                    //bindSupplement(ui.item.value, currentRow);
                    event.preventDefault(ui.item.value);
                } else {//if list null then hide the list 
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                }
            },
            focus: function (e, ui) {
                $(controlId).val(ui.item.label);
                $(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
                e.preventDefault();
            },
            change: function (e, ui) { //if text is not present in list then blank the textbox   
                $(".ui-widget.ui-widget-content").css("border", "none");
                $(".ui-menu-item").hide();
                $(".ui-menu").css("border", "none");
                $(".ui-menu").css("width", "0px");//add for handling width on browsers 

                if (!ui.item) {
                    $(this).val('');
                    $("#" + id).val('');
                }
            }
        }).keyup(function (e) {
            if (e.which === 13 || e.which === 27) {
                $(".ui-menu-item").hide();
                $(".ui-menu").css("border", "none");
            }
        });
    }

    function bindRoomType(HotelId, currentRow) {
        var QRFId = $("#QRFId").val();
        $.ajax({
            type: "GET",
            url: "/Accomodation/GetRoomType",
            data: { HotelId: HotelId, QRFId: QRFId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $(currentRow).find(".clsSupplierId").val(result.supplierId);
                $(currentRow).find(".clsSupplierName").val(result.supplierName);

                if (result.defaultRoomslist.length > 1) {
                    var defaultRoomsLen = result.defaultRoomslist.length;
                    var roomAvailable = $(currentRow + " .clonedInput .clsRoomType").length;

                    for (var i = 0; i < defaultRoomsLen; i++) {
                        if (i >= roomAvailable) {
                            $(currentRow).find("a.cloneRoom").click();
                        }
                    }

                    $(currentRow + " .clonedInput .clsRoomType").each(function (index) {
                        if (index != 0) {
                            if (index >= defaultRoomsLen)
                                $(currentRow).find("a.removeRoom").click();
                        }
                    });
                }
                else {
                    $(currentRow + " .clonedInput .clsRoomType").each(function (index) {
                        if (index != 0)
                            $(currentRow).find("a.removeRoom").click();
                    });
                } 

                $(currentRow + " .clonedInput .clsRoomType").each(function (index) {
                    var roomtype = $(this);
                    $(roomtype).empty();
                    $(roomtype).append($("<option></option>").val
                        (null).html("Select"));
                    if (result.roomTypeList.length > 0) {
                        $.each(result.roomTypeList, function (key, value) {
                            if (value.additionalYN != true) {
                                $(roomtype).append($("<option></option>").val
                                    (value.productCategoryId + '|' + value.voyagerProductRange_Id).html("(" + value.productCategoryName + ") " + value.productRangeCode + " (" + value.personType + (value.ageRange == null ? "" : " | " + value.ageRange) + ") "));
                            }
                        });
                    }
                    else {
                        alert("No rooms found!");
                    }
                });

                if (result.defaultRoomslist.length > 0) {
                    $(currentRow + " .clonedInput .clsRoomType").each(function (index) {
                        $(this).siblings("input[type=hidden]").val(
                            "(" + result.defaultRoomslist[index].productCategoryName + ") " + result.defaultRoomslist[index].productRangeCode +
                            " (" + result.defaultRoomslist[index].personType + (result.defaultRoomslist[index].ageRange == null ? "" : " | " + result.defaultRoomslist[index].ageRange) + ") ");
                        $(this).val(result.defaultRoomslist[index].productCategoryId + '|' + result.defaultRoomslist[index].voyagerProductRange_Id);
                    });
                }

                var supplement = $(currentRow + " .clsSupplement");
                $(supplement).empty();
                $(supplement).append($("<option></option>").val
                    (null).html("Select"));
                if (result.roomTypeList.length > 0) {
                    $.each(result.roomTypeList, function (key, value) {
                        if (value.additionalYN == true) {
                            $(supplement).append($("<option></option>").val
                                (value.productCategoryId + '|' + value.voyagerProductRange_Id).html("(" + value.productCategoryName + ") " + value.productRangeCode + " (" + value.personType + (value.ageRange == null ? "" : " | " + value.ageRange) + ") "));
                        }
                    });
                }
            },
            failure: function (error) {
                alert(error);
            },
            error: function (error) {
                alert(error);
            }
        });
    }

    function currentRowValidation(currentRow) {
        var IsValidFlag = true;
        var HotelName = currentRow.find(".bindHotelName").val();
        if (HotelName == '') {
            currentRow.find(".bindHotelName").siblings("span").text("*");
            IsValidFlag = false;
        }
        else
            currentRow.find(".bindHotelName").siblings("span").text("");

        var CityName = currentRow.find(".clsCity").val();
        if (CityName == '') {
            currentRow.find(".clsCity").siblings("span").text("*");
            IsValidFlag = false;
        }

        var StartTime = currentRow.find(".clsStartTime").val();
        if (StartTime == '') {
            currentRow.find(".clsStartTime").siblings("span").text("*");
            IsValidFlag = false;
        }

        var EndTime = currentRow.find(".clsEndTime").val();
        if (EndTime == '') {
            currentRow.find(".clsEndTime").siblings("span").text("*");
            IsValidFlag = false;
        }

        currentRow.find(".clsRoomType").each(function (index) {
            RoomTypeVal = $(this).val();
            if (RoomTypeVal == '') {
                $(this).siblings("span").text("*");
                IsValidFlag = false;
            }
            else
                $(this).siblings("span").text("");
        });

        var EarlyCheckInDateVal = currentRow.find(".clsEarlyCheckInDate").val();
        if (EarlyCheckInDateVal != '') {
            var EarlyCheckInTime = currentRow.find(".clsEarlyCheckInTime").attr('name');
            var EarlyCheckInTimeVal = $('[name="' + EarlyCheckInTime + '"]').val();
            if (EarlyCheckInTimeVal == '') {
                currentRow.find('[name="' + EarlyCheckInTime + '"]').siblings("span").text("*");
                IsValidFlag = false;
            }
            else
                currentRow.find('[name="' + EarlyCheckInTime + '"]').siblings("span").text("");
        }
        else {
            var EarlyCheckInTime = currentRow.find(".clsEarlyCheckInTime").attr('name');
            currentRow.find('[name="' + EarlyCheckInTime + '"]').siblings("span").text("");
        }

        var LateCheckOutDateVal = currentRow.find(".clsLateCheckOutDate").val();
        if (LateCheckOutDateVal != '') {
            var LateCheckOutTime = currentRow.find(".clsLateCheckOutTime").attr('name');
            var LateCheckOutTimeVal = $('[name="' + LateCheckOutTime + '"]').val();
            if (LateCheckOutTimeVal == '') {
                currentRow.find('[name="' + LateCheckOutTime + '"]').siblings("span").text("*");
                IsValidFlag = false;
            }
            else
                currentRow.find('[name="' + LateCheckOutTime + '"]').siblings("span").text("");
        }
        else {
            var LateCheckOutTime = currentRow.find(".clsLateCheckOutTime").attr('name');
            currentRow.find('[name="' + LateCheckOutTime + '"]').siblings("span").text("");
        }
        return IsValidFlag;
    }

    function rsetHotelField() {
        var controlId = "#" + $(this).attr('id');

        if ($(controlId).val().length < 3) {
            $(this).parents(".clonedHotel").find(".clsStarRating").val("");
            $(this).parents(".clonedHotel").find(".clsCategory").val("");
            $(this).parents(".clonedHotel").find(".clsStarRating").val("");
            $(this).parents(".clonedHotel").find(".clsChain").val("");
            var locvalue = $(this).parents(".clonedHotel").find(".clsLocation").val("");
        }
    }

    $(".clsfocprice").click(function () {
        $("#SimilarHotels-popup").hide();
        var dataname = $(this).attr("data-name");
        var IsValidFlag = true;
        var curRow = $(this).parents("tr");
        IsValidFlag = currentRowValidation(curRow);
        var currenrRow = this;
        if (IsValidFlag == false)
            return false;

        $(".ajaxloader").show();
        var divFOCPrices = '#' + $(this).siblings("div").attr('id');
        //$(divPrices).html('<img src="/images/loading.gif" alt="" width="50%">');

        var token = $('#frmAccomodation :input[name="__RequestVerificationToken"]').val();
        var QRFId = $("#QRFId").val();
        var HotelId = $(this).parents(".clonedHotel").find(".clsHotelID").val();
        var ProductType = $(this).parents(".clonedHotel").find(".clsProductType").val();

        var AccomodationId = "#" + $(this).parents(".clonedHotel").find("input[type=hidden]")[1].id;// "5a65ff69ffdbda0e2048e381";
        var AccomodationIdVal = $(AccomodationId).val();

        var currentRowId = "#" + $(this).parents("tr").attr('id');
        var pr = "";
        var foc = "";
        if (dataname == "price") {
            $(currentRowId + " .divPrices").html('');
            pr = "price";
        }
        else {
            $(currentRowId + " .divFOC").html('');
            foc = "foc";
        }

        var model = $(currentRowId + " :input,select,textarea").serialize();
        model += '&FOC=' + foc + '&Price=' + pr + '&SaveType=partial&MenuViewModel.QRFID=' + QRFId + '&MenuViewModel.IsClone=' + $("#IsClone").val() + '&__RequestVerificationToken=' + token;

        $.ajax({
            type: "POST",
            url: "/Accomodation/Accomodation",
            data: model,
            dataType: "json",
            global: false,
            success: function (response) {
                if (response != null && response != undefined && response.status != null && response.status.toLowerCase() == 'invalid') {
                    $(".ajaxloader").hide();
                    $(divFOCPrices).hide();
                    $(divFOCPrices).prev(".clsfocprice").removeClass("active");
                    alert('Accomodation cities not matching with routing!!!');
                }
                else if (response != null && response != undefined && response.roomDetailsInfo != null && response.roomDetailsInfo != undefined) {
                    $(divFOCPrices).show();
                    $(divFOCPrices).prev(".clsfocprice").addClass("active");

                    $(AccomodationId).val(response.positionId);
                    var roomDetailsInfo = response.roomDetailsInfo;
                    var roomobj, roomSeq, roomId, roomidnew = "";
                    // var lastseq = curRow.find("ul[id*='clonedRoom_'] li:not([style*='display: none;'])").length;
                    var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;

                    if (lastseq > 0 && roomDetailsInfo != null && roomDetailsInfo != undefined && roomDetailsInfo.length > 0) {
                        //&& lastseq == roomDetailsInfo.length) {
                        //curRow.find("ul[id*='clonedRoom_'] li:not([style*='display: none;'])").each(function (i) {
                        //    console.log(roomDetailsInfo[i]);
                        //    $(this).find("input[name*='RoomSequence']").val(roomDetailsInfo[i].roomSequence);
                        //    $(this).find("input[name*='RoomId']").val(roomDetailsInfo[i].roomId);
                        //    $(this).find("input[name*='IsSupplement']").val(roomDetailsInfo[i].isSupplement);
                        //    $(this).find("input[name*='ProductRange']").val("(" + roomDetailsInfo[i].productCategory + ") " + roomDetailsInfo[i].productRange);
                        //    $(this).find("select[name*='ProductRangeID']").val(roomDetailsInfo[i].productCategoryId + "|" + roomDetailsInfo[i].productRangeId);
                        //}); 
                        for (var i = 0; i < lastseq; i++) {
                            roomobj = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']");
                            roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                            roomId = roomobj.val();
                            if (roomId == '' && (roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0)) {
                                roomidnew = roomDetailsInfo.find(a => a.roomSequence == roomSeq);
                                roomobj.val(roomidnew.roomId);
                            }
                        }
                        if (dataname == "similarhotel") {
                            GetSImilarHotel(currenrRow);
                        }
                        else if (dataname == "price") {
                            GetPositionPricesPartView(divFOCPrices, QRFId, response.positionId, HotelId);
                        }
                        else {
                            GetPositionFOCPartView(divFOCPrices, QRFId, response.positionId, HotelId);
                        }
                    }
                }
            },
            failure: function (response) {
                $(".ajaxloader").hide();
                alert(response);
            },
            error: function (response) {
                $(".ajaxloader").hide();
                alert(response);
            }
        });
    });

    function GetSImilarHotel(currentRow) {
        var ProductId = $(currentRow).parents(".clonedHotel").find(".clsHotelID").val();
        var HotelName = $(currentRow).parents(".clonedHotel").find(".bindHotelName").val();
        var PositionId = $(currentRow).parents(".clonedHotel").find(".clsAccomodationId").val();
        var Location = $(currentRow).parents(".clonedHotel").find(".clsLocation").val();
        var BudgetCategory = $(currentRow).parents(".clonedHotel").find(".clsCategory").val().split('|')[1];
        var StarRating = $(currentRow).parents(".clonedHotel").find(".clsStarRating").val();
        var City = $(currentRow).parents(".clonedHotel").find(".clsCity").val();
        var IsClone = $("#IsClone").val();
        var EnquiryPipeline = $("#EnquiryPipeline").val();
        if (City != '') {
            var CityNameArr = City.split(',');
            CityName = CityNameArr[0].trim();
            CountryName = CityNameArr[1].trim();
        } else {
            CityName = '';
            CountryName = '';
        }
        $.ajax({
            type: "GET",
            url: "/Accomodation/GetSimilarHotels",
            data: { IsClone: IsClone, EnquiryPipeline: EnquiryPipeline, ProductId: ProductId, StarRating: StarRating, BudgetCategory: BudgetCategory, Location: Location, CityName: CityName, CountryName: CountryName, HotelName: HotelName, PositionId: PositionId },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {

                $("#SimilarHotels-popup .popup-in").html(response);
                $("#SimilarHotels-popup").show();
            },
            error: function (response) {
                $(".ajaxloader").hide();
                alert(response.statusText);
            }
        });
    }

    $(document).on('change', '.clsCity', function () {
        var curRow = $(this).closest('tr');
        curRow.find("input.clsHotelID").val('');
        curRow.find("input.bindHotelName").val('');
        curRow.find("input[name*='SupplierID']").val('');
        curRow.find("input[name*='SupplierName']").val('');

        curRow.find("select.clsStarRating").val('');
        curRow.find("select.clsCategory").val('');
        curRow.find("select.clsLocation").val('');

        //Remove all product ranges except one before filling defaults
        curRow.find('ul li.clonedInput').each(function (iCnt) {
            if (iCnt > 0) {
                $(this).closest('tr').find('a.removeRoom').click();
            }
        });
        curRow.find("select.clsRoomType").empty().append($("<option></option>").attr("value", '').text('Select'));
        curRow.find("select.clsSupplement").empty().append($("<option></option>").attr("value", '').text('Select'));
    });
});