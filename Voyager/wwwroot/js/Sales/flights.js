/*----------------------------------on change Product category Prod Range binds starts here----------------------*/
$(document).on('change', '.bindProductRange', function (e) {
    var thisparent = $(this).parents("tr");
    var index = thisparent.find("[id=hdnSeqNo]").val();
    var prodId = thisparent.find("[id=ProductName_" + index + "]").val();
    var hfProdCat = thisparent.find("[id^=ddlCategory_]").attr("id");
    var catId = $(this).val();
    $('[id="' + hfProdCat + '"]').val(e.target.selectedOptions[0].label);

    if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
        $.ajax({
            type: "GET",
            url: "/Flights/GetProductRange",
            data: { ProductId: prodId, CategoryId: catId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                var $elem = thisparent.find('select[name*="FlightDetails[' + index + '].PositionRoomsData.SupplementID"]');
                var $elem2 = $('#FlightDetails_' + index + '__PositionRoomsData_RoomDetails_0__ProductRangeID');
                $elem.empty(); $elem2.empty();
                $elem.append($("<option></option>").attr("value", '').text('Select'));
                $elem2.append($("<option></option>").attr("value", '').text('Select'));
                if (result != null && result != "") {
                    thisparent.find("[id^=hfSupplierID_]").val(result.supplierId);
                    thisparent.find("[id^=hfSupplierName_]").val(result.supplierName);
                    $('#SupplementID_' + index).closest('tr').find('ul li.clonedInput').each(function (iCnt) {
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

/*----------------------------------Autocomplete product names------------------------------------------------*/
$(document).on("keyup", ".bindpositionprod", function (event) {
    var key = event.which;
    var ingnore_key_codes = [60, 62];
    if ($.inArray(key, ingnore_key_codes) != -1) {
        event.preventDefault();
    } else {
        var thistr = $(this).parents('tr');
        var index = thistr.find('[id=hdnSeqNo]').val();
        var cityname = thistr.find('[id^=CityIDUI_]').val();
        var prodtype = "Domestic Flight";
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

/*----------------------------------grid add button click start here-----------------------------------*/
$('#tblFlights').on('click', 'a.icon-squre-green', function () {
    addRow(this);
    return false;
});
/*----------------------------------grid add button click ends here-----------------------------------*/

/*----------------------------------grid remove button click start here-----------------------------------*/
$('#tblFlights').on('click', 'a.icon-squre-red', function () {
    var clonedInputLen = $("#tblFlights .FlightsRow").length;
    if (clonedInputLen == 1) {
        $(this).closest('tr').find('input,select,span,textarea').each(function (iColCnt) {
            if (this.value != undefined) {
                if (this.id == 'hdnSeqNo') this.value = '0';
                else if (this.id.indexOf('ddlDayID') > -1) $("#" + this.id).val("");
                else if (this.id.indexOf('ddlCategoryID') > -1) { $("#" + this.id).empty(); $("#" + this.id).append($("<option></option>").attr("value", '').text('Select Category')); }
                else if (this.id.indexOf('ddlKeepAs') > -1) this.value = "Included";
                else if (this.id.indexOf('ddlDuration') > -1) this.value = "1";
                else if (this.id.indexOf('hfPositionSequence') > -1) this.value = 1;
                else {
                    if (this.id.indexOf('__PositionRoomsData_RoomDetails_') > -1) {
                        if (this.name.indexOf('PositionRoomsData.RoomDetails[0]') > -1) { $("#" + this.id).empty(); $("#" + this.id).append($("<option></option>").attr("value", '').text('Select')); }
                        else {
                            $(this).parent().parent().remove();
                        }
                    }
                    else if (this.id.indexOf('SupplementID_') > -1) {
                        $("#" + this.id).empty(); $("#" + this.id).append($("<option></option>").attr("value", '').text('Select'));
                    }
                    else this.value = '';
                }
            }
        });
        alert('Cannot delete this record.');
        return false;
    }

    if (confirm("Are you sure you want to delete this row?")) {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var id = $('#hfPositionId_' + index).val();

        if (id != undefined && id != null && id != '' && id.length > 30 && id.substring(0, 8) != '00000000') {
            $(this).closest('tr').hide();
            $('#hfIsDeleted_' + index).val('True');
        }
        else {
            removeRow(this, index);
        }
    }
    return false;
});
/*----------------------------------grid remove button click end here-----------------------------------*/

/*----------------------------------grid add price button click start here-----------------------------------*/
$("#tblFlights").on('click', '.clsfocprice', function () {
    //$(document).on('click', '.clsAddPrice', function (e) {
    var QRFId = $("#QRFId").val();
    var curRow = $(this).parents('tr');
    var index = curRow.find("[id=hdnSeqNo]").val();
    var flightsDetails = new Array();
    var flag = currentRowValidation(curRow);
    var dataname = $(this).attr("data-name");

    var pr = "";
    var foc = "";
    if (dataname == "price") {
        pr = "price";
    }
    else {
        foc = "foc";
    }

    if (!(RoomValidation("#frmFlights")))
        flag = false;

    if (!flag) {
        alert('Please fill the mandatory fields.');
        return false;
    }

    if (curRow.find('.field-validation-error').length <= 0) {
        $(".ajaxloader").show();
        var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;
        var RoomDetails = new Array();
        if (lastseq > 0) {
            for (var i = 0; i < lastseq; i++) {
                var roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                var roomId = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']").val();
                if ((roomId != '') || ((roomId == '') && (roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0))) {
                    RoomDetails.push({
                        RoomId: roomId,
                        RoomSequence: roomSeq,
                        ProductRangeID: curRow.find("select[name*='PositionRoomsData.RoomDetails[" + i + "].ProductRangeID']").val(),
                        ProductRange: curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].ProductRange']").val(),
                        IsSupplement: curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].IsSupplement']").val()
                    });
                }
            }
        }

        var PositionRoomsViewModel = { RoomDetails: RoomDetails };

        flightsDetails.push({
            PositionId: curRow.find("input[id^='hfPositionId_']").val(),
            DayID: curRow.find("select[id^='ddlDayID_']").val(),
            DayName: curRow.find("input[id^='ddlDay_']").val(),
            ProductType: curRow.find("input[name*='ProductType']").val(),
            ProductTypeID: curRow.find("input[name*='ProductTypeID']").val(),
            CityName: curRow.find("input[id^='CityIDUI_']").val(),
            CityID: curRow.find("input[id^='CityID_']").val(),
            StartTime: curRow.find("input[id^='StartTime_']").val(),
            ProductName: curRow.find("input[name*='ProductName']").val(),
            ProductID: curRow.find("input[name*='ProductID']").val(),
            BudgetCategory: curRow.find("select[id^='ddlCategoryID_']").val(),
            BudgetCategoryId: curRow.find("input[id^='ddlCategory_']").val(),
            SupplierID: curRow.find("input[id^='hfSupplierID_']").val(),
            SupplierName: curRow.find("input[id^='hfSupplierName_']").val(),
            KeepAs: curRow.find("select[id^='ddlKeepAs_']").val(),
            OPSRemarks: curRow.find("textarea[id^='txtOPSRemarks_']").val(),
            TLRemarks: curRow.find("textarea[id^='txtTLRemarks_']").val(),
            IsDeleted: curRow.find("input[id^='hfIsDeleted_']").val(),
            Status: "Q",
            PositionSequence: curRow.find("input[id^='hfPositionSequence_']").val(),
            NoOfPaxAdult: curRow.find("input[id*='__NoOfPaxAdult']").val(),
            NoOfPaxChild: curRow.find("input[id*='__NoOfPaxChild']").val(),
            NoOfPaxInfant: curRow.find("input[id*='__NoOfPaxInfant']").val(),
            PositionRoomsData: PositionRoomsViewModel,
            Duration: curRow.find("select[id^='ddlDuration_']").val()
        });
        var MenuViewModel = new Object();
        MenuViewModel = { IsClone: $("#IsClone").val() };

        var model = { QRFId: QRFId, SaveType: 'partial', FOC: foc, Price: pr, MenuViewModel: MenuViewModel, FlightDetails: flightsDetails, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };

        $.ajax({
            type: "POST",
            url: "/Flights/Flights",
            data: model,
            global: false,
            success: function (response) {
                var divprice = "#" + curRow.find("div[id^='divPrices_']").attr("id");
                var divfoc = "#" + curRow.find("div[id^='divFOC_']").attr("id");
                if (response.status.toLowerCase() == "success") {
                    curRow.find("input[id^='hfPositionId_']").val(response.positionId);
                    var roomDetailsInfo = response.roomDetailsInfo;
                    var roomobj, roomSeq, roomId, roomidnew = "";

                    var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;

                    if (lastseq > 0) {
                        for (var i = 0; i < lastseq; i++) {
                            roomobj = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']");
                            roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                            roomId = roomobj.val();
                            if (roomId == '' && roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0) {
                                roomidnew = roomDetailsInfo.find(a => a.roomSequence == roomSeq);
                                if (roomidnew != undefined && roomidnew != "" && roomidnew != null) {
                                    roomobj.val(roomidnew.roomId);
                                }
                            }
                        }
                        if (dataname == "price") {
                            $(divprice).show();
                            $(divprice).prev(".clsfocprice").addClass("active"); 
                            GetPositionPricesPartView(divprice, QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                        }
                        else if (dataname == "foc") {
                            $(divfoc).show();
                            $(divfoc).prev(".clsfocprice").addClass("active"); 
                            GetPositionFOCPartView(divfoc, QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                        }
                    }
                }
                else {
                    $(".ajaxloader").hide();
                    $(divprice).hide();
                    $(divfoc).hide();
                    $(".clsfocprice ").removeClass("active");
                }
            },
            error: function (response) {
                $(".ajaxloader").hide();
                alert(response.statusText);
            }
        });
    }
});
/*----------------------------------grid add price button click ends here-----------------------------------*/

/*----------------------------------Flights save button start here-----------------------------------------*/
$('.flightssave').click(function () {
    var flag = true;
    if (!(RoomValidation("#frmFlights")))
        flag = false;

    if (!$('#frmFlights').valid()) {
        flag = false;
    }
    if (flag) {
        var model = $('#frmFlights').serialize();
        model += '&FOC=foc&Price=price';

        $.ajax({
            type: "POST",
            url: "/Flights/Flights",
            data: model,
            dataType: "html",
            success: function (response) {
                if (response.indexOf('frmFlights') > 0) {
                    $('#frmFlights').replaceWith(response);
                    var form = $("#frmFlights");
                    form.removeData('validator');
                    form.removeData('unobtrusiveValidation');
                    $.validator.unobtrusive.parse(form);
                }
                else {
                    var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Flights not saved.</div>';
                    $('#frmFlights .simple-tab-cont').before(successmsg);
                }
            },
            error: function (response) {
                var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.statusText + '</div>';
                $('#frmFlights .simple-tab-cont').before(successmsg);
            }
        });
    }
    else {
        alert('Please fill mandatory fields.');
    }
});
/*----------------------------------Flights save button ends here-----------------------------------------*/

$(document).ready(function () {
    $("#tblFlights").on('change', '.keepAsChanged', function () {
        var row = $(this).closest('tr');
        if (this.value == 'Included') row.find('.NoOfPaxColumn').hide();
        else row.find('.NoOfPaxColumn').show();
    });

    $("#tblFlights").on('change', '.bindDayStartTime', function () {
        var row = $(this).closest('tr');
        row.find('input[id*="__StartTime"]').val('10:00');
    });

    $(document).on('change', '.fromcityUI', function () {
        var curRow = $(this).closest('tr');
        curRow.find("input.clsProductID").val('');
        curRow.find("input.bindpositionprod").val('');
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

    $(".keepAsChanged").change();
});

function addRow(thisparam) {
    var flag = true; 
    $("#tblFlights .FlightsRow:not([style*='display: none;'])").each(function (index) {
        flag = currentRowValidation($(this));
    });
    if (!(RoomValidation("#frmFlights")))
        flag = false;

    if (!flag) {
        alert('Please fill the mandatory fields before adding new record!');
        return false;
    }

    var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();
    var clonedRow = $(thisparam).closest('tr').clone();
    $(thisparam).closest('tr').after(clonedRow);

    var tdPrice = clonedRow.find('td.light-yellow-bg');
    if (tdPrice.length > 0) {
        tdPrice.removeClass('light-yellow-bg').find('.Prices span.msg, .Prices span.msgErr,.FOC span.msg, .FOC span.msgErr').hide();
        tdPrice.find('.clsAddPrice').text('Prices');
        tdPrice.find('.clsAddFOC').text('FOC');
        tdPrice.find('.tooltip-cont #tblFlights table tbody').empty();
    }

    $('#tblFlights tbody tr.FlightsRow').each(function (iRowCnt) {
        if (iRowCnt > index) {
            var thisRow = this;
            var curRow = $(this).find('#hdnSeqNo').val();
            this.id = this.id.replace(curRow, iRowCnt);

            $(this).find('input,select,span,textarea,td,ul,div').each(function (iColCnt) {
                if (this.id != undefined) {
                    this.id = this.id.replace(curRow, iRowCnt);
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else if (this.id.indexOf('ddlKeepAs') > -1) $(this).val(this.value); 
                    else if (this.id.indexOf('hfPositionSequence') > -1) this.value = iRowCnt + 1;
                    else {
                        if (iRowCnt == (parseInt(index) + 1)) {
                            if (this.id.indexOf('NoOfPax') > -1) { $(thisRow).find('.NoOfPaxColumn').hide(); }
                            else if (this.id.indexOf('StartTime') > -1) {
                                var fullStartTime = $('#StartTime_' + index).val();
                                var startTime = fullStartTime.substring(0, 2)
                                var startTimeM = fullStartTime.substring(3, 5)
                                if (startTime !== undefined && startTime != "" && startTime.length == 2) {
                                    startTime = parseInt(startTime);
                                    startTime = startTime + 3;
                                    startTime = (startTime > 0 && startTime <= 9) ? "0" + startTime : startTime;
                                    this.value = startTime + ":" + startTimeM;
                                }
                            }
                            else if (this.id.indexOf('__PositionRoomsData_RoomDetails_') > -1) {
                                if (this.id.indexOf('__PositionRoomsData_RoomDetails_0__RoomId') > -1) { this.value = ''; }
                                else if (this.name.indexOf('PositionRoomsData.RoomDetails[0]') > -1) { $("#" + this.id).empty(); $("#" + this.id).append($("<option></option>").attr("value", '').text('Select')); }
                                else {
                                    $(this).parent().parent().remove();
                                } 
                            }
                            else if (this.id.indexOf('ddlDuration') > -1) {
                                $(this).val("1");
                            }
                            else if (this.id.indexOf('SupplementID_') > -1 || this.id.indexOf('ddlCategoryID') > -1) {
                                $("#" + this.id).empty(); $("#" + this.id).append($("<option></option>").attr("value", '').text('Select'));
                            }
                            else this.value = '';
                        }
                    }
                }

                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });

    var form = $("#frmFlights");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}

function removeRow(thisparam, rowIndex) {
    $(thisparam).closest('tr').remove();
    $('#tblFlights .FlightsRow').each(function (iRowCnt) {
        if (iRowCnt >= rowIndex) {
            var curRow = $(this).find('#hdnSeqNo').val();
            $(this).find('input,select,span,textarea').each(function (iColCnt) {
                if (this.id != undefined) {
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else this.id = this.id.replace(curRow, iRowCnt);
                }
                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });
}

function currentRowValidation(currentRow) {
    var IsValidFlag = true;
    var index = currentRow.find("#hdnSeqNo").val();
    var ddlDay = currentRow.find("[id^=ddlDayID_]").val();
    if (ddlDay == '' || ddlDay == null) {
        currentRow.find("[id^=ddlDayID_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var ddlDuration = currentRow.find("[id^=ddlDuration_]").val();
    if (ddlDuration == '') {
        currentRow.find("[id^=ddlDuration_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var StartTime = currentRow.find("[id^=StartTime_]").val();
    if (StartTime == '') {
        currentRow.find("[id^=StartTime_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var ProductNameUI = currentRow.find("[id^=ProductNameUI_]").val();
    if (ProductNameUI == '') {
        currentRow.find("[id^=ProductNameUI_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var ddlCategory = currentRow.find("[id^=ddlCategoryID_]").val();
    if (ddlCategory == '') {
        currentRow.find("[id^=ddlCategoryID_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var CityIDUI_ = currentRow.find("[id^=CityIDUI_]").val();
    if (CityIDUI_ == '') {
        currentRow.find("[id^=CityIDUI_]").siblings("span").text("*");
        IsValidFlag = false;
    }

    var ddlKeepAs = currentRow.find("[id^=ddlKeepAs_]").val();

    if (ddlKeepAs == '') {
        currentRow.find("[id^=ddlKeepAs_]").siblings("span").text("*");
        IsValidFlag = false;
    }
    else if (ddlKeepAs != "Included") {
        if (currentRow.find("[id=FlightDetails_" + index + "__NoOfPaxAdult]").val() == "0") {
            currentRow.find("[id=FlightDetails_" + index + "__NoOfPaxAdult]").parents(".NoOfPaxColumn").find("span").text("*");
            IsValidFlag = false;
        }
    }

    return IsValidFlag;
}

function AutoCompleteChanged(thisparam) {
    var parentrow = $(thisparam).parents('.FlightsRow');
    var ddlProdCat = parentrow.find('[id^=ddlCategoryID_]').attr("id");
    $("#" + ddlProdCat).empty();
    $("#" + ddlProdCat).append($("<option></option>").attr("value", '').text('Select Category'));

    var productid = parentrow.find('[id^=ProductName_]').val(); 
    if (productid != "" && productid != null && productid != undefined) {
        $.ajax({
            type: "GET",
            url: "/Flights/GetProductCategoryByParam",
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