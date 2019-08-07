$(document).ready(function () {
    var form = $("#frmOthersVisa");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);

    $("#OthersVisa").on("keyup", "#tblVisa .bindProductName", function () {
        //var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var city = $(this).closest('tr').find('.bindCity').val();
        InitializeAutoComplete('/Activities/GetProductName', this, 3, { prodName: this.value, city: city, prodType: 'VISA' }, 'category');
    });

    $('#OthersVisa').on('click', '#tblVisa a.icon-squre-green', function () {
        var flag = true;
        if (!(RoomValidation("#frmOthersVisa")))
            flag = false;

        if (!$('#frmOthersVisa').valid())
            flag = false;

        if (flag) {  
            addRowVisa(this);
        }
        else {
            alert('Please fill the mandatory fields!!!');
        }
        return false;
    });

    $('#OthersVisa').on('click', '#tblVisa a.icon-squre-red', function () {
        if (confirm("Are you sure you want to delete this row?")) {
            var index = $(this).closest('tr').find('#hdnSeqNo').val();
            var othersId = $('#OthersVisa #LocalGuideDetails_' + index + '__OthersId').val();  //IsDeleted

            if (othersId != undefined && othersId != null && othersId != '' && othersId.length > 30 && othersId.substring(0, 8) != '00000000') {
                if ($('#tblVisa tbody tr').length <= 1) {
                    var clonedRow = $(this).closest('tr').clone();
                    $(this).closest('tr').after(clonedRow);
                    clonedRow.find('input,select,span,textarea').each(function (iColCnt) {
                        if (this.value != undefined) {
                            if (this.id != 'hdnSeqNo') this.value = '';
                        }
                    });
                }
                $(this).closest('tr').hide();
                $('#OthersVisa #LocalGuideDetails_' + index + '__IsDeleted').val('True');
            }
            else {
                removeRowVisa(this, index);
            }
        }
        return false;
    });

    $("#OthersVisa").on('change', '#tblVisa .bindProductRange', function () {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var QRFId = $("#QRFId").val();
        var prodId = $('#ProductName1_' + index).val();
        var catId = $('#ProductCategoryID_' + index).val();
        if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
            $.ajax({
                type: "GET",
                url: "/Activities/GetProductRangeList",
                data: { QRFId: QRFId, ProductId: prodId, CategoryId: catId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $('#OthersVisa #LocalGuideDetails_' + index + '__SupplierID').val(result.supplierId);
                    $('#OthersVisa #LocalGuideDetails_' + index + '__SupplierName').val(result.supplierName);
                    if (result != undefined && result != null) {
                        $('#OthersVisa #SupplementID_' + index).closest('tr').find('ul li.clonedInput').each(function (iCnt) {
                            if (iCnt > 0) {
                                $(this).closest('tr').find('a.removeRoom').click();
                            }
                        });

                        //bind product range dropdown
                        if (result.prodRangeList != undefined && result.prodRangeList != null) {
                            var $elem = $('#OthersVisa #SupplementID_' + index);                                                                    //Supplementary Services
                            var $elem2 = $('#OthersVisa #LocalGuideDetails_' + index + '__PositionRoomsData_RoomDetails_0__ProductRangeID');        //Mandatory Services
                            $elem.empty(); $elem2.empty();
                            $elem.append($("<option></option>").attr("value", '').text('Select'));
                            $elem2.append($("<option></option>").attr("value", '').text('Select'));
                            $.each(result.prodRangeList, function (i, list) {
                                //check AdditionalYN and associated dropdown
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
                    alert(error.statusText);
                }
            });
        }
    });

    $("#OthersVisa").on('click', '#tblVisa .clsfocprice', function () {
        var thisparam = this;
        var QRFId = $("#QRFId").val();
        var curRow = $(this).closest('tr');
        var LocalGuideDetails = new Array();
        var dataname = $(this).attr("data-name");

        var pr = "";
        var foc = "";
        if (dataname == "price") {
            pr = "price";
        }
        else {
            foc = "foc";
        }

        var flag = true;
        if ($('#frmOthersVisa').valid()) { }

        if (!(RoomValidation("#frmOthersVisa")))
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

            LocalGuideDetails.push({
                OthersId: curRow.find("input[id*='OthersId']").val(),
                DayNo: curRow.find("select[id*='ddlDayID_']").val(),
                DayName: curRow.find("input[id*='ddlDay_']").val(),
                CityID: curRow.find("input[name*='CityID']").val(),
                CityName: curRow.find("input[name*='CityName']").val(),
                StartTime: curRow.find("input[id*='StartTime']").val(),
                Duration: curRow.find("select[id*='ddlDuration_']").val(),
                ProductName: curRow.find("input[name*='ProductName']").val(),
                ProductID: curRow.find("input[name*='ProductID']").val(),
                ProductType: curRow.find("input[name*='ProductType']").val(),
                ProductTypeID: curRow.find("input[name*='ProductTypeID']").val(),//
                ProductCategory: curRow.find("input[id*='ProductCategory_']").val(),
                ProductCategoryID: curRow.find("select[id*='ProductCategoryID_']").val(),
                SupplierID: curRow.find("input[id*='SupplierID']").val(),
                SupplierName: curRow.find("input[id*='SupplierName']").val(),
                KeepAs: curRow.find("select[id*='KeepAs']").val(),
                RemarksTL: curRow.find("textarea[id*='RemarksTL']").val(),
                RemarksOPS: curRow.find("textarea[id*='RemarksOPS']").val(),
                PositionRoomsData: PositionRoomsViewModel
            });

            var model = { QRFId: QRFId, SaveType: 'partial', FOC: foc, Price: pr, IsClone: $("#IsClone").val(), LocalGuideDetails: LocalGuideDetails, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };
            $.ajax({
                type: "POST",
                url: "/Others/LocalGuide",
                data: model,
                global: false,
                success: function (response) {
                    if (response.status.toLowerCase() == "success") {
                        //curRow.find("input[id*='__OthersId']").val(response.positionId);
                        //GetQRFPricesPartViewVisa(thisparam, QRFId);
                        curRow.find("input[id*='__OthersId']").val(response.positionId);
                        var roomDetailsInfo = response.roomDetailsInfo;
                        var roomobj, roomSeq, roomId, roomidnew = "";
                        var lastseq = curRow.find("ul[id*='clonedRoom_'] li").length;

                        if (lastseq > 0) {
                            for (var i = 0; i < lastseq; i++) {
                                roomobj = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomId']");
                                roomSeq = curRow.find("input[name*='PositionRoomsData.RoomDetails[" + i + "].RoomSequence']").val();
                                roomId = roomobj.val();
                                if (roomId == '' && (roomSeq != undefined && roomSeq != '' && !isNaN(roomSeq) && parseInt(roomSeq) > 0)) {
                                    roomidnew = roomDetailsInfo.find(a => a.roomSequence == roomSeq);
                                    roomobj.val(roomidnew.roomId);
                                }
                            }
                            if (dataname == "price") {
                                $(curRow.find("div[id*='divPrices']")).show();
                                $(curRow.find("div[id*='divPrices']")).prev(".clsfocprice").addClass("active");
                                GetPositionPricesPartView(curRow.find("div[id*='divPrices']"), QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                            }
                            else if (dataname == "foc") {
                                $(curRow.find("div[id*='divFOC']")).show();
                                $(curRow.find("div[id*='divFOC']")).prev(".clsfocprice").addClass("active");
                                GetPositionFOCPartView(curRow.find("div[id*='divFOC']"), QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                            }
                        }
                    }
                    else {
                        $(".ajaxloader").hide();
                        $(".tool-tip > a").click();
                    }
                },
                error: function (response) {
                    $(".ajaxloader").hide();
                    alert(response.statusText);
                }
            });
        }
        else {
            alert('Please fill the mandatory fields!!!');
            return false;
        }
    });

    $("#OthersVisa").on('change', '#tblVisa .bindDayStartTime', function () {
        var row = $(this).closest('tr');
        row.find('input[id*="__StartTime"]').val('10:00');
    });

    $('.SaveVisa').click(function () { 
        var flag = true;
        if (!(RoomValidation("#frmOthersVisa")))
            flag = false;

        if (!$('#frmOthersVisa').valid()) {
            flag = false;
        }

        if (flag) {
            var model = $('#frmOthersVisa').serialize();
            model += '&FOC=foc&Price=price';
            $.ajax({
                type: "POST",
                url: "/Others/LocalGuide",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmOthersVisa') > 0) {
                        $('#frmOthersVisa').replaceWith(response);
                        var form = $("#frmOthersVisa");
                        form.removeData('validator');
                        form.removeData('unobtrusiveValidation');
                        $.validator.unobtrusive.parse(form);
                    }
                    else {
                        var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                        $('#frmOthersVisa .simple-tab-cont').before(successmsg);
                    }
                },
                error: function (response) {
                    alert(response.statusText);
                }
            });
        }
        else {
            alert('Please fill mandatory fields!!!');
        }
    });

    $(document).on('change', '.fromcityUI', function () {
        var curRow = $(this).closest('tr');
        curRow.find("input.clsProductID").val('');
        curRow.find("input.bindProductName").val('');
        curRow.find("input[name*='SupplierID']").val('');
        curRow.find("input[name*='SupplierName']").val('');

        curRow.find("input[name*='ProductCategory']").val('');
        curRow.find("select[name*='ProductCategoryID']").empty().append($("<option></option>").attr("value", '').text('Select'));

        //Remove all product ranges except one before filling defaults
        curRow.find('ul li.clonedInput').each(function (iCnt) {
            if (iCnt > 0) {
                $(this).closest('tr').find('a.removeRoom').click();
            }
        });
        curRow.find("select.clsRoomType").empty().append($("<option></option>").attr("value", '').text('Select'));
        curRow.find("select.clsSupplement").empty().append($("<option></option>").attr("value", '').text('Select'));
    });

    //ShowHideRoleBasedControls('QRF', 'price-foc-sec');
});

function addRowVisa(thisparam) {
    var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();
    var clonedRow = $(thisparam).closest('tr').clone();
    $(thisparam).closest('tr').after(clonedRow);
    var tdPrice = clonedRow.find('td.light-yellow-bg');
    if (tdPrice.length > 0) {
        tdPrice.removeClass('light-yellow-bg').find('.Prices span.msg, .Prices span.msgErr').hide();
        tdPrice.find('.clsAddPrice').text('Prices');
        tdPrice.find('.clsAddFOC').text('FOC');
        tdPrice.find('.tooltip-cont #frmQRFPrices table tbody').empty();
    }

    $('#OthersVisa #tblVisa tbody tr.VisaRow').each(function (iRowCnt) {
        if (iRowCnt > index) {
            var curRow = $(this).find('#hdnSeqNo').val();
            this.id = this.id.replace(curRow, iRowCnt);
            $(this).find('input,select,span,textarea,td,ul').each(function (iColCnt) {
                if (this.id != undefined) {
                    this.id = this.id.replace(curRow, iRowCnt);
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else if (this.id.indexOf('DayNo') > -1 || this.id.indexOf('KeepAs') > -1) this.value = this.value;
                    else {
                        if (iRowCnt == (parseInt(index) + 1)) {
                            if (this.id.indexOf('NoOfPax') > -1) { }
                            else if (this.id.indexOf('StartTime') > -1) {
                                var fullStartTime = $('#OthersVisa #LocalGuideDetails_' + index + '__StartTime').val();
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
                                if (this.name.indexOf('PositionRoomsData.RoomDetails[0]') > -1) { }
                                else {
                                    $(this).parent().parent().remove();
                                }
                            }
                            else this.value = '';

                            if (this.id.indexOf('ProductCategoryID') > -1 || this.id.indexOf('ProductRangeID') > -1 || this.id.indexOf('SupplementID') > -1) {
                                $(this).empty();
                                $(this).append($("<option></option>").attr("value", '').text('Select'));
                            }
                        }
                    }
                }

                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });

    var form = $("#frmOthersVisa");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}

function removeRowVisa(thisparam, rowIndex) {
    if ($('#OthersVisa #tblVisa tbody tr').length <= 1) {
        var clonedRow = $(thisparam).closest('tr').clone();
        $(thisparam).closest('tr').after(clonedRow);
        clonedRow.find('input,select,span,textarea').each(function (iColCnt) {
            if (this.value != undefined) {
                if (this.id != 'hdnSeqNo') this.value = '';
            }
        });
    }
    $(thisparam).closest('tr').remove();
    $('#OthersVisa #tblVisa tbody tr').each(function (iRowCnt) {
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
    })
}

function AutoCompleteChanged(thisparam) {
    var hdnId = '#' + $(thisparam).attr('id').replace('UI', '');
    var productId = $(hdnId).val();
    if (productId != undefined && productId != '') {
        $.ajax({
            type: "GET",
            url: "/Activities/GetProductCategory",
            data: { ProductId: productId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            global: false,
            success: function (result) {
                var $elem = $(thisparam).closest('td').find('select[id*="ProductCategoryID"]');
                $($elem).empty();
                //$elem.append($("<option></option>").attr("value", '').text('Select'));
                $.each(result, function (key, value) {
                    $($elem).append($("<option></option>").val(value.value).html(value.label));
                });
                $($elem).change();
            },
            error: function (error) {
                alert(error.statusText);
            }
        });
    }
}