function LoadNewQuoteScript(QRFId) {
    if (QRFId > 0) {
        // ======= SubMenu Functionlaity Start ========\\
        $("#AgentInformation").click(function () {
            $(".ajaxloader").show();
            window.location.reload(true);
            //$("#QuoteTabsCont").children().hide();
            //$("#AgentInformation").parent().find("a").removeClass("active");
            //$("#aAgentInformation").addClass("active");
            //$("#QuoteAgentInformation").show();
        });

        $("#DateRange").click(function () {
            //if ($('#QuoteDateRange').html() === "") {
            $('#QuoteDateRange').load('/Quote/GetDateRange/', { QRFId: QRFId });
            //}
            $("#QuoteTabsCont").children().hide();
            $("#DateRange").parent().find("a").removeClass("active");
            $("#aDateRange").addClass("active");
            $("#QuoteDateRange").show();
        });

        $("#Routing").click(function () {
            //if ($('#QuoteRouting').html() === "") {
            $('#QuoteRouting').load('/Quote/GetRouting/', { QRFId: QRFId });
            //}
            $("#QuoteTabsCont").children().hide();
            $("#Routing").parent().find("a").removeClass("active");
            $("#aRouting").addClass("active");
            $("#QuoteRouting").show();
        });

        $('#Margins').click(function () {
            //if ($('#QuoteMargins').html() === "") {
            $('#QuoteMargins').load('/Quote/GetMargin/', { QRFId: QRFId });
            //}
            $("#QuoteTabsCont").children().hide();
            $("#Margins").parent().find("a").removeClass("active");
            $("#aMargins").addClass("active");
            $("#QuoteMargins").show();
        });

        $("#PaxRange").click(function () {
            //if ($('#QuotePaxRange').html() === "") {
            $('#QuotePaxRange').load('/Quote/GetPaxRange/', { QRFId: QRFId });
            //}
            $("#QuoteTabsCont").children().hide();
            $("#PaxRange").parent().find("a").removeClass("active");
            $("#aPaxRange").addClass("active");
            $("#QuotePaxRange").show();
        });

        $("#FOC").click(function () {
            //if ($('#QuoteFOC').html() === "") {
            $('#QuoteFOC').load('/Quote/GetFOC/', { QRFId: QRFId });
            //}
            $("#QuoteTabsCont").children().hide();
            $("#FOC").parent().find("a").removeClass("active");
            $("#aFOC").addClass("active");
            $("#QuoteFOC").show();
        });
    }
    else {
        $("#DateRange,#aDateRange,#PaxRange,#aPaxRange,#FOC,#aFOC,#Routing,#aRouting,#Margins,#aMargins").addClass("disabled");
    }

    // ======= SubMenu Functionlaity End ========\\

    $(".bindAgent").on("keyup", function (event) {
        InitializeAutoComplete('/Quote/GetAgentNameFrommCompanies', this, 3, { term: this.value }, 'agent');
    });

    // ======= Child increment Functionlaity Start ========\\

    ChildWithBedAgeNo = 0;
    ChildWithoutBedAgeNo = 0;
    $('.count-childwithbed .count-up').click(function () {
        var ChildNo = $('.txt-childwithbed-no').val();
        ChildWithBedAgeNo++;
        AddChild(parseInt(ChildNo) + 1, '1', 'WB');
    });

    $('.count-childwithbed .count-down').click(function () {
        var ChildNo = $('.txt-childwithbed-no').val();
        $('.count-child .count-up').removeClass('disabled');
        $('.count-child .count-up').removeAttr('disabled');
        if (ChildNo == 0) {
            ChildWithBedAgeNo = 0;
        }
        else {
            $('.multi-child').show();
            ChildWithBedAgeNo--;
            $('.div-childwithbed .child:last-child').remove();
        }
    });

    $('.count-childwithoutbed .count-up').click(function () {
        var ChildNo = $('.txt-childwithoutbed-no').val();
        ChildWithoutBedAgeNo++;
        AddChild(parseInt(ChildNo) + 1, '1', 'WOB');
    });

    $('.count-childwithoutbed .count-down').click(function () {
        var ChildNo = $('.txt-childwithoutbed-no').val();
        $('.count-child .count-up').removeClass('disabled');
        $('.count-child .count-up').removeAttr('disabled');
        if (ChildNo == 0) {
            ChildWithoutBedAgeNo = 0;
        }
        else {
            $('.multi-child').show();
            ChildWithoutBedAgeNo--;
            $('.div-childwithoutbed .child:last-child').remove();
        }
    });

    // ======= Child increment Functionlaity End ========\\

    // ======= AgentInfo Form submit Functionality Start ========\\

    $("#frmQuoteAgentInformation").submit(function (event) {
        var flag = CalculateAdultPaxOnRoomType();
        if (!flag)
            return flag;
        var withBedAges, withoutBedAges;
        $('.div-childwithbed').children().find("input").each(function () {
            if (withBedAges == undefined || withBedAges == "") {
                withBedAges = $(this).val();
            }
            else {
                withBedAges += "," + $(this).val();
            }
        });
        $('.div-childwithoutbed').children().find("input").each(function () {
            if (withoutBedAges == undefined || withoutBedAges == "") {
                withoutBedAges = $(this).val();
            }
            else {
                withoutBedAges += "," + $(this).val();
            }
        });
        $('#ApproxPaxChildWithBedAge').val(withBedAges);
        $('#ApproxPaxChildWithoutBedAge').val(withoutBedAges);

        var TourName = $('#TourName').val();
        var hdnTourName = $('#hdnTourName').val();
        if (TourName != undefined && TourName != null && TourName != '') {
            if ($('#TourNameValid')[0].style.display == 'none' && hdnTourName != TourName) {
                $('#TourNameValid').hide();
                $('#TourNameInvalid').show();
                event.preventDefault();
            }
        }

    });

    // ======= AgentInfo Form submit Functionality End ========\\

    $(".bootstrap-switch-container span,.bootstrap-switch-container input,.sm-toggle-btns small").click(function () {
        if ($(this).text() == 'ON') {
            $('#QuotePricingType').val('Consolidated Cost');
        }
        else if ($(this).text() == 'OFF') {
            $('#QuotePricingType').val('Itemised Costing');
        }
        else {
            if ($('#QuotePricingType').val() == 'Itemised Costing') {
                $('#QuotePricingType').val('Consolidated Cost');
            }
            else if ($('#QuotePricingType').val() == 'Consolidated Cost') {
                $('#QuotePricingType').val('Itemised Costing');
            }
        }
        //alert($('#QuotePricingType').val());
    });

    $("#ContactPerson").change(function () {
        var ContactID = $("#ContactPerson").val();
        $.ajax({
            type: "GET",
            url: "/Quote/GetContactDetailsByContactID",
            data: { ContactID: ContactID },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $("#MobileNo").val(result.mobile);
                $("#Email").val(result.mail);
            },
            error: function (error) {
                alert(error);
            }
        });
    });

    $('input.room-type-btn').click(function () {
        if ($('.room-type-opt-cont').css('display') === 'none')
            $('.room-type-opt-cont').show();
        else {
            CalculateAdultPaxOnRoomType();
            $('.room-type-opt-cont').hide();
        }
    });

    $('.room-type-opt-cont .btn-close').click(function () {
        CalculateAdultPaxOnRoomType();
        $('.room-type-opt-cont').hide();
    });

    $('.room-type-opt-cont input[name*="IsSelected"]').click(function () {

        var roomtypeTitle = $('input.room-type-btn').val();
        var roomtype = $(this).closest('li').find('input[id*="RoomTypeName"]').val();

        if (this.checked == true) {
            $(this).closest('li').find('input[id*="RoomCount"]').removeAttr('disabled');

            if (roomtypeTitle == 'None Selected')
                $('input.room-type-btn').val(roomtype);
            else
                $('input.room-type-btn').val(roomtypeTitle + ', ' + roomtype);
        }
        else {
            $(this).closest('li').find('input[id*="RoomCount"]').attr('disabled', 'disabled');
            $(this).closest('li').find('input[id*="RoomCount"]').val('');

            if (roomtypeTitle == roomtype) $('input.room-type-btn').val('None Selected');
            else {
                if (roomtypeTitle.indexOf(', ' + roomtype) > -1)
                    $('input.room-type-btn').val(roomtypeTitle.replace(', ' + roomtype, ''));
                else
                    $('input.room-type-btn').val(roomtypeTitle.replace(roomtype, ''));

                roomtypeTitle = $('input.room-type-btn').val();
                if (roomtypeTitle.startsWith(', '))
                    $('input.room-type-btn').val(roomtypeTitle.substr(2));
            }
        }
    });

    $('.ValidateTourName').blur(function () {
        var TourName = this.value;
        if (TourName != null && TourName != '') {
            $.ajax({
                type: "GET",
                url: "/Quote/CheckDuplicateQRFTourName",
                data: { TourName: TourName },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var hdnTourName = $('#hdnTourName').val();
                    if (result != undefined && result != null && result != '') {
                        if (result.toLowerCase() == 'valid') {
                            $('#TourNameValid').show();
                            $('#TourNameInvalid').hide();
                        }
                        else if (hdnTourName == TourName) {
                            $('#TourNameValid').show();
                            $('#TourNameInvalid').hide();
                        }
                        else {
                            $('#TourNameValid').hide();
                            $('#TourNameInvalid').show();
                        }
                    }
                    else {
                        $('#TourNameValid').hide();
                        $('#TourNameInvalid').show();
                    }
                },
                error: function (error) {
                    alert(error.statusText);
                }
            });
        }
        else {
            $('#TourNameValid').hide();
            $('#TourNameInvalid').hide();
        }
    });

    ShowHideRoleBasedControls('Sales', 'pricing-type-sec');
}

function CalculateAdultPaxOnRoomType() {
    var flag = true;
    var AdultCount = 0, RoomCount = 0, RoomTypeName = '', IsSelected = false;
    $('.room-type-opt-cont').find('li').each(function (iRow) {
        RoomCount = $(this).find('input[id*="__RoomCount"]').val();
        RoomTypeName = $(this).find('input[id*="__RoomTypeName"]').val();
        IsSelected = $(this).find('input[id*="__IsSelected"]').is(':checked');

        if (IsSelected && !(RoomCount != undefined && RoomCount != null && RoomCount != '' && !isNaN(RoomCount) && RoomCount != 0)) {
            alert("Please enter room count for selected room");
            //$(".room-type-opt-cont").css("display", "block");
            // event.preventDefault();
            flag = false;
        }

        if (IsSelected && (RoomCount != undefined && RoomCount != null && RoomCount != '' && !isNaN(RoomCount)) && (RoomTypeName != undefined && RoomTypeName != null && RoomTypeName != '')) {
            if (RoomTypeName.toLowerCase() == 'single')
                AdultCount += parseInt(RoomCount);
            else if (RoomTypeName.toLowerCase() == 'tsu')
                AdultCount += parseInt(RoomCount);
            else if (RoomTypeName.toLowerCase() == 'twin')
                AdultCount += (parseInt(RoomCount) * 2);
            else if (RoomTypeName.toLowerCase() == 'double')
                AdultCount += (parseInt(RoomCount) * 2);
            else if (RoomTypeName.toLowerCase() == 'triple')
                AdultCount += (parseInt(RoomCount) * 3);
            else if (RoomTypeName.toLowerCase() == 'quad')
                AdultCount += (parseInt(RoomCount) * 4);
        }
    });

    $('#ApproxPaxAdult').val(AdultCount);
    return flag;
}

function AddChild(ChildNo, ChildAgeNo, BedType) {
    $('.multi-child').show();
    if (BedType == "WB") {
        var divChild = '<div class="col-xs-12 child"><strong>C+B ' + ChildNo + ' Age</strong><div class="cust-spinner value"><div class="input-group spinner"><div class="input-group-btn"><button class="btn btn-default count-down" type="button"><i class="fa-custom-minus white"></i></button></div><input type="text" class="form-control frm-pax-control" value="' + ChildAgeNo + '"><div class="input-group-btn"><button class="btn btn-default count-up" type="button"><i class="fa-custom-plus white"></i></button></div></div></div></div>';
        $('.div-childwithbed').append(divChild);
    }
    else if (BedType == "WOB") {
        var divChild = '<div class="col-xs-12 child"><strong>C-B ' + ChildNo + ' Age</strong><div class="cust-spinner value"><div class="input-group spinner"><div class="input-group-btn"><button class="btn btn-default count-down" type="button"><i class="fa-custom-minus white"></i></button></div><input type="text" class="form-control frm-pax-control" value="' + ChildAgeNo + '"><div class="input-group-btn"><button class="btn btn-default count-up" type="button"><i class="fa-custom-plus white"></i></button></div></div></div></div>';
        $('.div-childwithoutbed').append(divChild);
    }
}

function SetSelectedQuoteRoomValues(QuoteRoomTypeArray) {
    //if (QuoteRoomTypeArray != undefined && QuoteRoomTypeArray != "") {
    //    for (var i = 0; i < QuoteRoomTypeArray.length; i++) {
    //        var value = QuoteRoomTypeArray[i];
    //        $('#QuoteRoomType').parent().find('.dropdown-menu').find('input[value="' + value + '"]').click();
    //    }
    //}
}

function SetChildControlValues(CWBCount, CWBAge, CWoBCount, CWoBAge) {
    var CWBAgeList = CWBAge.split(',');
    var CWoBAgeList = CWoBAge.split(',');

    for (var i = 0; i < CWBCount; i++) {
        AddChild(i + 1, CWBAgeList[i], "WB");
    }
    for (var i = 0; i < CWoBCount; i++) {
        AddChild(i + 1, CWoBAgeList[i], "WOB");
    }
}

function SetQuotePricingType(QuotePricingType) {
    if (QuotePricingType == 'Itemised Costing') {
        $('#QuotePricingType').val('Itemised Costing');
    }
    else {
        $(".bootstrap-switch-container span,.bootstrap-switch-container input").click();
    }
}

function SetTourInfoHeaderValues(QRFId, AgentName, TourName, TourCode, Duration) {
    if (QRFId != "0") {
        $('#lblQRFId').text(QRFId);
        $('#lblAgentName').text(AgentName);
        $('#lblTourName').text(TourName);
        $('#lblTourCode').text(TourCode);
        $('#lblDuration').append(Duration + ' N');
        $('#TourInfoHeader').show();
    }
}


function AutoCompleteChanged(thisparam) {
    BindAgentDetails();
}

function BindAgentDetails() {
    $("#MobileNo").val('');
    $("#Email").val('');
    var AgentId = $("#AgentId").val();
    if (AgentId.length > 2) {
        $.ajax({
            type: "GET",
            url: "/Quote/GetAgentContacts",
            data: { AgentId: AgentId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var $el = $("#ContactPerson");
                $el.empty();
                $el.append($("<option></option>").attr("value", '').text('Select'));
                $.each(result, function (i, list) {
                    $el.append($("<option></option>").attr("value", list.voyagerContact_Id).text(list.fullName));
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

$("#ddlProductDivision").on("change", function () {
	 
	var Id = $(this).find("option:selected").val();
	var Name = $(this).find("option:selected").text();
	$("#ProductDivision").val(Name);
});

function SetDefaultValues() {
    var PurposeOfTravel = $("#PurposeOfTravel").val();
    var Priority = $("#Priority").val();

    if (PurposeOfTravel != undefined && (PurposeOfTravel == null || PurposeOfTravel == '')) {
        $("#PurposeOfTravel option:contains(Leisure)").attr('selected', 'selected');
    }
    if (Priority != undefined && (Priority == null || Priority == '')) {
        $("#Priority").val('Medium');
    }
}

//function ShowHideRoleBasedControls() {
//    try {
//        var UserRoles = $('#hdnUserRoles').val();
//        if (UserRoles != null && UserRoles != '') {
//            UserRoles = UserRoles.split(',');
//            if ($.inArray('Sales', UserRoles) > 0) {
//                $('.pricing-type-sec').show();
//            }
//            else {
//                $('.pricing-type-sec').hide();
//            }
//        }
//    } catch (e) {
//        $('.pricing-type-sec').hide();
//    }
//}
