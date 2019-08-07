$(document).ready(function () {
    $("#tblRail").on("keyup", ".bindProductName", function () {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var city = $(this).closest('tr').find('.bindCity').val();
        InitializeAutoComplete('/Activities/GetProductName', this, 3, { prodName: this.value, city: city, prodType: 'Train' }, 'category');
    });

	$('#tblRail').on('click', 'a.icon-squre-green', function () {
		var flag = true;
		if (!(RoomValidation("#frmRail")))
			flag = false;

		if (!$('#frmRail').valid())
			flag = false;

		if (flag) {
			addRow(this);
		}
		else {
			alert('Please fill the mandatory fields!!!');
		}
		return false;	
    });

    $('#tblRail').on('click', 'a.icon-squre-red', function () {
        if (confirm("Are you sure you want to delete this row?")) {
            var index = $(this).closest('tr').find('#hdnSeqNo').val();
            var RailId = $('#RailDetails_' + index + '__RailId').val();  //IsDeleted

            if (RailId != undefined && RailId != null && RailId != '' && RailId.length > 30 && RailId.substring(0, 8) != '00000000') {
                if ($('#tblRail tbody tr').length <= 1) {
                    var clonedRow = $(this).closest('tr').clone();
                    $(this).closest('tr').after(clonedRow);
                    clonedRow.find('input,select,span,textarea').each(function (iColCnt) {
                        if (this.value != undefined) {
                            if (this.id != 'hdnSeqNo') this.value = '';
                        }
                    });
                }
                $(this).closest('tr').hide();
                $('#RailDetails_' + index + '__IsDeleted').val('True');
            }
            else {
                removeRow(this, index);
            }
        }
        return false;
    });

    $("#tblRail").on('change', '.bindProductRange', function () {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var QRFId = $("#QRFId").val();
        var prodId = $('#ProductName_' + index).val();
        var catId = $('#RailDetails_' + index + '__ProductCategoryID').val();
        if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
            $.ajax({
                type: "GET",
                url: "/Activities/GetProductRangeList",
                data: { QRFId: QRFId, ProductId: prodId, CategoryId: catId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    $('#RailDetails_' + index + '__SupplierID').val(result.supplierId);
                    $('#RailDetails_' + index + '__SupplierName').val(result.supplierName);
                    if (result != undefined && result != null) {
                        $('#SupplementID_' + index).closest('tr').find('ul li.clonedInput').each(function (iCnt) {
                            if (iCnt > 0) {
                                $(this).closest('tr').find('a.removeRoom').click();
                            }
                        });

                        //bind product range dropdown
                        if (result.prodRangeList != undefined && result.prodRangeList != null) {
                            var $elem = $('#SupplementID_' + index);                                                            //Supplementary Services
                            var $elem2 = $('#RailDetails_' + index + '__PositionRoomsData_RoomDetails_0__ProductRangeID');      //Mandatory Services
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

                            //load default product ranges
                            if (result.defProdRangeList != undefined && result.defProdRangeList != null) {
                                $.each(result.defProdRangeList, function (i, list) {
                                    var tr = $elem.closest('tr');
                                    if (i > 0) tr.find('a.cloneRoom').click();
                                    var ctrl = $(tr.find('td.divRoomeDetails ul li[class="clonedInput"]')[i]).find('select.clsRoomType');
                                    if (ctrl.length > 0) {
                                        ctrl[0].value = list.value;
                                        $('input[name="' + ctrl[0].name.replace('ID', '') + '"]').val(list.label);
                                    }
                                });

                                //$('#SupplementID_' + index).closest('tr').find('ul li').each(function (iCnt) {
                                //    var i = 0;
                                //    if (this.class = "clonedInput") {
                                //        this.value = result.defProdRangeList
                                //    }
                                //});
                            }
                        }
                    }
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });

    $("#tblRail").on('change', '.keepAsChanged', function () {
        var row = $(this).closest('tr');
        if (this.value == 'Included') row.find('.NoOfPaxColumn').hide();
        else row.find('.NoOfPaxColumn').show();
    });

    $("#tblRail").on('click', '.clsfocprice', function () {
        var flag = true;
        var thisparam = this;
        var QRFId = $("#QRFId").val();
        var curRow = $(this).closest('tr');
        var RailDetails = new Array();
        var dataname = $(this).attr("data-name");
        var flag = true;

        var pr = "";
        var foc = "";
        if (dataname == "price") {
            pr = "price";
        }
        else {
            foc = "foc";
        }

		if ($('#frmRail').valid()) { }

		if (!(RoomValidation("#frmRail")))
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

            RailDetails.push({
                RailId: curRow.find("input[id*='__RailId']").val(),
                DayNo: curRow.find("select[id*='ddlDayID_']").val(),
                DayName: curRow.find("input[id^='ddlDay_']").val(),
                CityID: curRow.find("input[name*='CityID']").val(),
                CityName: curRow.find("input[name*='CityName']").val(),
                StartTime: curRow.find("input[id*='__StartTime']").val(),
                Duration: curRow.find("select[id*='ddlDuration_']").val(),
                ProductName: curRow.find("input[name*='ProductName']").val(),
                ProductID: curRow.find("input[name*='ProductID']").val(),
                ProductType: curRow.find("input[name*='ProductType']").val(),
                ProductTypeID: curRow.find("input[name*='ProductTypeID']").val(),//
                ProductCategoryID: curRow.find("select[id*='__ProductCategoryID']").val(),
                ProductCategory: curRow.find("input[id*='__ProductCategory']").val(),
                SupplierID: curRow.find("input[id*='__SupplierID']").val(),
                SupplierName: curRow.find("input[id*='__SupplierName']").val(),
                TransferDetails: curRow.find("select[id*='__TransferDetails']").val(),
                NoOfPaxAdult: curRow.find("input[id*='__NoOfPaxAdult']").val(),
                NoOfPaxChild: curRow.find("input[id*='__NoOfPaxChild']").val(),
                NoOfPaxInfant: curRow.find("input[id*='__NoOfPaxInfant']").val(),
                KeepAs: curRow.find("select[id*='__KeepAs']").val(),
                RemarksTL: curRow.find("textarea[id*='__RemarksTL']").val(),
                RemarksOPS: curRow.find("textarea[id*='__RemarksOPS']").val(),
                PositionRoomsData: PositionRoomsViewModel
            });
            var MenuViewModel = new Object();
            MenuViewModel = { IsClone: $("#IsClone").val() };

            var model = { QRFId: QRFId, SaveType: 'partial', FOC: foc, Price: pr, MenuViewModel: MenuViewModel, RailDetails: RailDetails, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };
            $.ajax({
                type: "POST",
                url: "/Rail/Rail",
                data: model,
                global: false,
                success: function (response) {
                    if (response.status.toLowerCase() == "success") {
                        curRow.find("input[id*='__RailId']").val(response.positionId);
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
                                GetPositionPricesPartView('#' + curRow.find("div[id*='divPrices']").attr('id'), QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                            }
                            else if (dataname == "foc") {
                                $(curRow.find("div[id*='divFOC']")).show();
                                $(curRow.find("div[id*='divFOC']")).prev(".clsfocprice").addClass("active");
                                GetPositionFOCPartView('#' + curRow.find("div[id*='divFOC']").attr('id'), QRFId, response.positionId, curRow.find("input[name*='ProductID']").val());
                            }
                            //GetQRFPricesPartView(thisparam, QRFId);
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
        }
    });

    $("#tblRail").on('change', '.bindDayStartTime', function () {
        var row = $(this).closest('tr');
        row.find('input[id*="__StartTime"]').val('10:00');
    });

    $(".keepAsChanged").change();

	$('.SaveRail').click(function () {
		var flag = true;
		if (!(RoomValidation("#frmRail")))
			flag = false;

		if (!$('#frmRail').valid()) {
			flag = false;
		}

		if (flag) {
            var model = $('#frmRail').serialize();
            model += '&FOC=foc&Price=price';
            $.ajax({
                type: "POST",
                url: "/Rail/Rail",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmRail') > 0) {
                        $('#frmRail').replaceWith(response);
                        var form = $("#frmRail");
                        form.removeData('validator');
                        form.removeData('unobtrusiveValidation');
                        $.validator.unobtrusive.parse(form);
                    }
                    else {
                        var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Train details not saved!!!</div>';
                        $('#frmRail .simple-tab-cont').before(successmsg);
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
});

function addRow(thisparam) {
    var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();     //Get Row Index from hidden field
    var clonedRow = $(thisparam).closest('tr').clone();                 //Clone row to append

    //Remove Proce/Foc section data before adding
    var tdPrice = clonedRow.find('td.light-yellow-bg');
    if (tdPrice.length > 0) {
        tdPrice.removeClass('light-yellow-bg').find('.Prices span.msg, .Prices span.msgErr').hide();
        tdPrice.find('.clsAddPrice').text('Prices');
        tdPrice.find('.clsAddFOC').text('FOC');
        tdPrice.find('.tooltip-cont #frmRail table tbody').empty();
    }

    //Append cloned row after current row
    $(thisparam).closest('tr').after(clonedRow);

    //Change id, name, value and other properties of all rows
    $('#tblRail tbody tr.RailRow').each(function (iRowCnt) {
        //Check if loop row index is greater than current row index
        if (iRowCnt > index) {
            var thisRow = this;
            var curRow = $(this).find('#hdnSeqNo').val();

            //Change id of all rows with new one (update seq no in row id)
            this.id = this.id.replace(curRow, iRowCnt);

            //Change id, name, value and other properties of all fields
            $(this).find('input,select,span,textarea,td,ul,div').each(function (iColCnt) {
                if (this.id != undefined) {
                    //Change id of all fields
                    this.id = this.id.replace(curRow, iRowCnt);

                    //Change value of all fields
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else if (this.id.indexOf('ddlDayID') > -1 || this.id.indexOf('KeepAs') > -1 || this.id.indexOf('TransferDetails') > -1) this.value = this.value; 
                    else {
                        //Check if the row index is current row
                        if (iRowCnt == (parseInt(index) + 1)) {
                            //Hide pax count fields
                            if (this.id.indexOf('NoOfPax') > -1) { $(thisRow).find('.NoOfPaxColumn').hide(); }
                            else if (this.id.indexOf('ddlDuration') > -1) { this.value = '1'; }
                            else if (this.id.indexOf('StartTime') > -1) {
                                //Start time field logic to add 3 hours for new row on same day
                                var fullStartTime = $('#RailDetails_' + index + '__StartTime').val();
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
                                //Remove product range dropdowns except single dropdown
                                if (this.name.indexOf('PositionRoomsData.RoomDetails[0]') > -1) {
                                    if (this.id.indexOf('__RoomId') > -1) { this.value = ''; }
                                }
                                else {
                                    $(this).parent().parent().remove();
                                }
                            } 
                            else this.value = '';           //Change all values to null

                            //Clear values of product category and range dropdowns
                            if (this.id.indexOf('ProductCategoryID') > -1 || this.id.indexOf('ProductRangeID') > -1 || this.id.indexOf('SupplementID') > -1) {
                                $(this).empty();
                                $(this).append($("<option></option>").attr("value", '').text('Select'));
                            }
                        }
                    }
                }

                //Change name of all fields
                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);

                //Change validation controls values
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });

    //Rebind form validation to enable new rows validation
    var form = $("#frmRail");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}

function removeRow(thisparam, rowIndex) {
    if ($('#tblRail tbody tr').length <= 1) {
        var clonedRow = $(thisparam).closest('tr').clone();
        $(thisparam).closest('tr').after(clonedRow);
        clonedRow.find('input,select,span,textarea').each(function (iColCnt) {
            if (this.value != undefined) {
                if (this.id != 'hdnSeqNo') this.value = '';
            }
        });
    }
    $(thisparam).closest('tr').remove();
    $('table.table tbody tr').each(function (iRowCnt) {
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
                var $elem = $(thisparam).closest('td').find('select[id*="__ProductCategory"]');
                $($elem).empty();
                //$elem.append($("<option></option>").attr("value", '').text('Select'));
                $.each(result, function (key, value) {
                    $($elem).append($("<option></option>").val(value.value).html(value.label));
                });
                $($elem).change();
            },
            error: function (error) {
                alert(error);
            }
        });
    }
}