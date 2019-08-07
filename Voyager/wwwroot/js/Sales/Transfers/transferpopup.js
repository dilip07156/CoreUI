//$(document).ready(function () {
//    ShowHideRoleBasedControls('QRF', 'price-foc-sec');
//});

function AutoCompleteChanged(thisparam) {
    var parentrow = $(thisparam).parents('.TransferRow');
    var ddlProdCat = parentrow.find('[id=ddlCategoryID]').attr("id");
    $("#" + ddlProdCat).empty();
    $("#" + ddlProdCat).append($("<option></option>").attr("value", '').text('Select Category'));

    var productid = parentrow.find('[id=ProductName]').val();
    if (productid != "" && productid != null && productid != undefined) {
        $.ajax({
            type: "GET",
            url: "/Transfers/GetProductCategoryByParam",
            data: { ProductId: productid, ProductName: null },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $("#" + ddlProdCat).empty();
                $("#" + ddlProdCat).append($("<option></option>").attr("value", '').text('Select Category'));
                $.each(result, function (key, value) {
                    $("#" + ddlProdCat).append($("<option></option>").val(value.value).html(value.label));
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

/*----------------------------------Autocomplete product names------------------------------------------------*/
//$(".bindpositionprod").unbind("keyup");
//$(document).on("keyup", ".bindpositionprod", function (event) {
$(".bindpositionprod").on("keyup", function (event) {
    var key = event.which;
    var ingnore_key_codes = [60, 62];
    if ($.inArray(key, ingnore_key_codes) != -1) {
        event.preventDefault();
    } else {
        var thistr = $(this).parents(".div-pop-sec");
        var cityname = thistr.find('#FromCityUI').val();
        prodtype = thistr.find(".headertextposition").text();

        if (cityname != "" && cityname != undefined && cityname != null && prodtype != "" && prodtype != null && prodtype != undefined) {
            var ProdType = JSON.stringify(prodtype);
            var arr = cityname.split(",");
            var citynm = arr != null && arr[0] != null ? arr[0].trim() : "";
            var countrynm = arr != null && arr[1] != null ? arr[1].trim() : "";
            var data = { ProdName: $(this).val(), ProdType: ProdType, CityName: citynm, CountryName: countrynm };
            InitializeAutoComplete('GetProductNameList', this, 3, data, 'prodcat');
        }
    }
});
/*----------------------------------Autocomplete product names------------------------------------------------*/

/*----------------------------------on change Product category Prod Range binds starts here----------------------*/
//$(document).on('change', '.bindProductRange', function (e) {
$('.bindProductRange').on('change', function (e) {
    var thisparent = $(this).parents("tr");
    var prodId = thisparent.find("[id=ProductName]").val();
    var hfProdCat = thisparent.find("[id=ddlCategory]").attr("id");
    var catId = $(this).val();
    $('[id="' + hfProdCat + '"]').val(e.target.selectedOptions[0].label);

    if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
        $.ajax({
            type: "GET",
            url: "/Transfers/GetProductRange",
            data: { ProductId: prodId, CategoryId: catId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var $elem = thisparent.find('select[name*="TransferProductRange[0].PositionRoomsData.SupplementID"]');
                var $elem2 = $('#TransferProductRange_0__PositionRoomsData_RoomDetails_0__ProductRangeID');
                $elem.empty(); $elem2.empty();
                $elem.append($("<option></option>").attr("value", '').text('Select'));
                $elem2.append($("<option></option>").attr("value", '').text('Select'));

                if (result != null && result != "") {
                    thisparent.find("[id^=hfSupplierID]").val(result.supplierId);
                    thisparent.find("[id^=hfSupplierName]").val(result.supplierName);

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
/*----------------------------------on change Product category Prod Range binds ends here----------------------*/

$(".transferContinue").unbind("click");
//popup continue button
$(".transferContinue").on('click', function (e) {
    CheckSaveTransferDetails(this);
});

//popup add price button 
$('.clsfocprice').on('click', function (e) {
    CheckSaveTransferDetails(this, "focprice");
});

function CheckSaveTransferDetails(thisparam, status = "") {
    var thisparent = $(thisparam).parents(".div-pop-sec");
    var dataname = $(thisparam).attr("data-name");

    var flag = true;
    if (!(RoomValidation("#frmTransfers")))
        flag = false;

    if (thisparent.find("[id=FromCityUI]").val() == "") {
        flag = false;
        thisparent.find("#lblFromCity").text("*");
    }
    if (thisparent.find("#ddlDuration").val() == "") {
        flag = false;
        thisparent.find("#lblDurationList").text("*");
    }
    if (thisparent.find("#FromPickUpLocUI").val() == "") {
        flag = false;
        thisparent.find("#lblFromPickUpLoc").text("*");
    }
    if (thisparent.find("#FromPickUpTimeUI").val() == "") {
        flag = false;
        thisparent.find("#lblFromPickUpTime").text("*");
    }
    if (thisparent.find("#ToCityUI").val() == "") {
        flag = false;
        thisparent.find("#lblToCity").text("*");
    }
    if (thisparent.find("#ToDropOffLocUI").val() == "") {
        flag = false;
        thisparent.find("#lblToDropOffLoc").text("*");
    }
    if (thisparent.find("#ToDropOffTimeUI").val() == "") {
        flag = false;
        thisparent.find("#lblToDropOffTime").text("*");
    }
    if (thisparent.find("#ProductNameUI").val() == "") {
        flag = false;
        thisparent.find("#lblProductName").text("*");
    }
    if (thisparent.find("#ddlCategoryID").val() == "") {
        flag = false;
        thisparent.find("#lblBudgetCategory").text("*");
    }

    if (flag) {
        thisparent.find("#lblFromCity").text("");
        thisparent.find("#lblDurationList").text("");
        thisparent.find("#lblFromPickUpLoc").text("");
        thisparent.find("#lblFromPickUpTime").text("");
        thisparent.find("#lblToCity").text("");
        thisparent.find("#lblToDropOffLoc").text("");
        thisparent.find("#lblToDropOffTime").text("");
        thisparent.find("#lblProductName").text("");
        thisparent.find("#lblBudgetCategory").text("");
        SetTransferDetails(thisparam, dataname);
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

function SetTransferDetails(thisparam, flag) {
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
    var prodid = thisparent.find("[name=ProductID]").val();
    var prodtype = thisparent.find("input[name='ProductType']").val()

    var RoomDetails = new Array();
    if (lastseq > 0) {
        for (var i = 0; i < lastseq; i++) {
            var roomSeq = thisparent.find("input[name='TransferProductRange[0].PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
            RoomDetails.push({
                RoomId: thisparent.find("input[name='TransferProductRange[0].PositionRoomsData.RoomDetails[" + i + "].RoomId']").val(),
                RoomSequence: roomSeq,
                ProductCategoryId: thisparent.find("#ddlCategoryID").val(),
                ProductCategory: thisparent.find("#ddlCategoryID option[value='" + thisparent.find("#ddlCategoryID").val() + "']").text(),
                ProductRangeId: thisparent.find("select[name='TransferProductRange[0].PositionRoomsData.RoomDetails[" + i + "].ProductRangeID']").val(),
                ProductRange: thisparent.find("input[name='TransferProductRange[0].PositionRoomsData.RoomDetails[" + i + "].ProductRange']").val(),
                IsSupplement: thisparent.find("input[name='TransferProductRange[0].PositionRoomsData.RoomDetails[" + i + "].IsSupplement']").val(),
                IsDeleted: roomSeq == 0 ? true : false
            });
        }
    }
    var PositionType = thisparent.find('.headertextposition').text().trim();

    PositionType = PositionType == "scheduled transfer" ? "Scheduled Transfer" : PositionType == "private transfer" ? "Private Transfer" :
        PositionType == "ferry passenger" ? "Ferry Passenger" : PositionType == "ferry transfer" ? "Ferry Transfer" : PositionType;

    var TransferProperties = {
        PositionId: positionid, QRFId: qrfid, DayID: thisparent.find("[id=hfDayID]").val(),
        PositionSequence: thisparent.find("[id=hfPositionSeq]").val(), RoutingDaysID: thisparent.find("[id=hfRoutingDaysID]").val()
    };

    var TransferPopup = { //Passing data
        ProductType: prodtype,
        ProductTypeId: thisparent.find("input[name*='ProductTypeID']").val(),
        FromCity: thisparent.find("[id=FromCityUI]").val().trim(),
        FromCityID: thisparent.find("[id=FromCity]").val(),
        FromPickUpLoc: thisparent.find("[id=FromPickUpLocUI]").val(),
        FromPickUpLocID: thisparent.find("[id=FromPickUpLoc]").val(),
        StartTime: thisparent.find("[id=FromPickUpTimeUI]").val(),
        ToCity: thisparent.find("[id=ToCityUI]").val(),
        ToCityID: thisparent.find("[id=ToCity]").val(),
        ToDropOffLoc: thisparent.find("[id=ToDropOffLocUI]").val(),
        ToDropOffLocID: thisparent.find("[id=ToDropOffLoc]").val(),
        EndTime: thisparent.find("[id=ToDropOffTimeUI]").val(),
        ProductId: prodid,
        ProductName: thisparent.find("[name^=ProductName]").val(),
        BudgetCategory: thisparent.find("#ddlCategoryID option[value='" + thisparent.find("#ddlCategoryID").val() + "']").text(),
        BudgetCategoryId: thisparent.find("#ddlCategoryID").val(),
        RoomDetailsInfo: RoomDetails,
        Duration: thisparent.find("#ddlDuration").val(),
        KeepAs: thisparent.find("#ddlKeepAs").val(),
        TLRemarks: thisparent.find("#txtTLRemarks").val(),
        OPSRemarks: thisparent.find("#txtOPSRemarks").val(),
        SupplierId: thisparent.find("#hfSupplierID").val(),
        SupplierName: thisparent.find("#hfSupplierName").val(),
        TransferProperties: TransferProperties
    };
    var MenuViewModel = new Object();
    MenuViewModel = { IsClone: $("#IsClone").val() };

    var model = { FOC: foc, Price: pr, MenuViewModel: MenuViewModel, TransferPopup: TransferPopup, SaveType: "partial", __RequestVerificationToken: $(':input[name="__RequestVerificationToken"]').val() };

    var datasec = thisparent.find(".div-pop-sec").attr("data-sec");
    var hfPositionId = thisparent.prev('tr').find('td.active').find("[datasechf^=" + datasec + "]").attr("id");
    $.ajax({
        type: "POST", //HTTP POST Method
        url: "/Transfers/Transfers", // Controller/View
        data: model,
        global: false,
        success: function (result) {
            thisparent.find("#divSuccessError").show();
            if (result != null && result != "") {
                thisparent.find("#hfQRFId").val(result.qrfid);
                thisparent.find("#hfPositionId").val(result.positionId);
                if (result.responseStatus.status.toLowerCase() == "success") {
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
                        GetPositionFOCPartView("#divFOC", qrfid, result.positionId, prodid);
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
        }
    });
}

$(document).on('change', 'input.fromcityUI', function () {
    var curRow = $(this).closest('tr');
    curRow.find("input.bindpositionprod").val('');
    curRow.find("input[name*='ProductID']").val('');
    curRow.find("input[name*='SupplierID']").val('');
    curRow.find("input[name*='SupplierName']").val('');

    curRow.find("input[name*='BudgetCategoryId']").val('');
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