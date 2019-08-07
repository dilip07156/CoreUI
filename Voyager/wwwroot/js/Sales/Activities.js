$(document).ready(function () {
    $("#tblActivities").on("keyup", ".bindProductName", function () {
        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var city = $(this).closest('tr').find('.bindCity').val();
        InitializeAutoComplete('/Activities/GetProductName', this, 3, { prodName: this.value, city: city, prodType: 'Attractions,Sightseeing - CityTour' }, 'category');
    });

	$('#tblActivities').on('click', 'a.icon-squre-green', function () {
		var flag = true;
		if (!(RoomValidation("#frmActivities")))
			flag = false;

		if (!$('#frmActivities').valid())
			flag = false;

		if (flag) {
			addRow(this);
		}
		else {
			alert('Please fill the mandatory fields!!!');
		}
		return false;		
    });

    $('#tblActivities').on('click', 'a.icon-squre-red', function () {
        if (confirm("Are you sure you want to delete this row?")) {
            var index = $(this).closest('tr').find('#hdnSeqNo').val();
            var activityId = $('#ActivityDetails_' + index + '__ActivityId').val();  //IsDeleted

            if (activityId != undefined && activityId != null && activityId != '' && activityId.length > 30 && activityId.substring(0, 8) != '00000000') {
                if ($('#tblActivities tbody tr').length <= 1) {
                    var clonedRow = $(this).closest('tr').clone();
                    $(this).closest('tr').after(clonedRow);
                    clonedRow.find('input,select,span,textarea').each(function (iColCnt) {
                        if (this.value != undefined) {
                            if (this.id != 'hdnSeqNo') this.value = '';
                        }
                    });
                }
                $(this).closest('tr').hide();
                $('#ActivityDetails_' + index + '__IsDeleted').val('True');
            }
            else {
                removeRow(this, index);
            }
        }
        return false;
    });

    $(document).on('change', '.bindProductRange', function () {

        var index, QRFId, prodId, catId, curRow;
        curRow = $(this).closest('tr');
        QRFId = $("#QRFId").val();

        if (this.id.indexOf('QuickPickActivities') > -1) {
            index = -1;
            prodId = curRow.find('input[type="hidden"][id*="__ProdID"]').val();
            catId = curRow.find('select[id*="__ProdCategoryID"]').val();
        }
        else {
            index = curRow.find('#hdnSeqNo').val();
            prodId = $('#ProductName_' + index).val();
            catId = $('#ActivityDetails_' + index + '__TourTypeID').val();
        }
        if (catId !== undefined && prodId !== undefined && catId !== '' && prodId !== '') {
            $.ajax({
                type: "GET",
                url: "/Activities/GetProductRangeList",
                data: { QRFId: QRFId, ProductId: prodId, CategoryId: catId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result != undefined && result != null) {
                        var $elem, $elem2;
                        if (index > -1) {
                            $('#ActivityDetails_' + index + '__SupplierID').val(result.supplierId);
                            $('#ActivityDetails_' + index + '__SupplierName').val(result.supplierName);

                            //Remove all product ranges except one before filling defaults
                            $('#SupplementID_' + index).closest('tr').find('ul li.clonedInput').each(function (iCnt) {
                                if (iCnt > 0) {
                                    $(this).closest('tr').find('a.removeRoom').click();
                                }
                            });
                            $elem = $('#SupplementID_' + index);                                                            //Supplementary Services
                            $elem2 = $('#ActivityDetails_' + index + '__PositionRoomsData_RoomDetails_0__ProductRangeID');  //Mandatory Services
                        }
                        else {
                            $elem = curRow.find('select.clsSupplement');                                                    //Supplementary Services
                            $elem2 = curRow.find('select.clsRoomType');                                                     //Mandatory Services
                            curRow.find('input[type="hidden"][id*="__SupplierID"]').val(result.supplierId);
                            curRow.find('input[type="hidden"][id*="__SupplierName"]').val(result.supplierName);
                        }
                        //bind product range dropdown
                        if (result.prodRangeList != undefined && result.prodRangeList != null) {
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
                                    if (list.type.toLowerCase() == "false") {
                                        if (i > 0) tr.find('a.cloneRoom').click();
                                        var ctrl = $(tr.find('td.divRoomeDetails ul li[class="clonedInput"]')[i]).find('select.clsRoomType');
                                        if (ctrl.length > 0) {
                                            ctrl[0].value = list.value;
                                            $('input[name="' + ctrl[0].name.replace('ID', '') + '"]').val(list.label);
                                        }
                                    }
                                    else {
                                        var ctrl = $(tr.find('td.divRoomeDetails')).find('select.clsSupplement');
                                        if (ctrl.length > 0) {
                                            ctrl[0].value = list.value;
                                            $('input[name="' + ctrl[0].name.replace('ID', '') + '"]').val(list.label);
                                            tr.find('a.cloneSupplement').click();
                                            ctrl[0].value = '';
                                            $('input[name="' + ctrl[0].name.replace('ID', '') + '"]').val('');
                                        }
                                    }
                                });
                            }
                        }
                    }
                },
                error: function (error) {
                    alert(error.statusText);
                }
            });
        }
    });

    $("#tblActivities").on('change', '.keepAsChanged', function () {
        var row = $(this).closest('tr');
        if (this.value == 'Included') row.find('.NoOfPaxColumn').hide();
        else row.find('.NoOfPaxColumn').show();
    });

    $("#tblActivities").on('click', '.clsfocprice', function () {
        var flag = true;
        var thisparam = this;
        var QRFId = $("#QRFId").val();
        var curRow = $(this).closest('tr');
        var ActivityDetails = new Array();
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

        if ($('#frmActivities').valid()) { }

		if (!(RoomValidation("#frmActivities")))
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

            ActivityDetails.push({
                ActivityId: curRow.find("input[id*='__ActivityId']").val(),
                ActivityDayNo: curRow.find("select[id*='ddlDayID_']").val(),
                DayName: curRow.find("input[id*='ddlDay_']").val(),
                CityID: curRow.find("input[name*='CityID']").val(),
                CityName: curRow.find("input[name*='CityName']").val(),
                StartTime: curRow.find("input[id*='__StartTime']").val(),
                //TypeOfExcursionID: curRow.find("select[id*='__TypeOfExcursionID']").val(),
                //TypeOfExcursion: curRow.find("input[id*='__TypeOfExcursion']").val(),
                ProductName: curRow.find("input[name*='ProductName']").val(),
                ProductID: curRow.find("input[name*='ProductID']").val(),
                ProductType: curRow.find("input[name*='ProductType']").val(),
                ProductTypeID: curRow.find("input[name*='ProductTypeID']").val(),//
                TourTypeID: curRow.find("select[id*='__TourTypeID']").val(),
                TourType: curRow.find("input[id*='__TourType']").val(),
                TransferDetails: curRow.find("select[id*='__TransferDetails']").val(),
                SupplierID: curRow.find("input[id*='__SupplierID']").val(),
                SupplierName: curRow.find("input[id*='__SupplierName']").val(),
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

            var model = { QRFId: QRFId, SaveType: 'partial', FOC: foc, MenuViewModel: MenuViewModel, Price: pr, ActivityDetails: ActivityDetails, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() };
            $.ajax({
                type: "POST",
                url: "/Activities/Activities",
                data: model,
                global: false,
                success: function (response) {
                    if (response.status.toLowerCase() == "success") {
                        curRow.find("input[id*='__ActivityId']").val(response.positionId);
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

    $("#tblActivities").on('change', '.ddlday', function () {
        var row = $(this).closest('tr');
        row.find('input[id*="__StartTime"]').val('10:00');
    });

    $(".keepAsChanged").change();

	$('.SaveActivities').click(function () {
		var flag = true;
		if (!(RoomValidation("#frmActivities")))
			flag = false;

		if (!$('#frmActivities').valid()) {
			flag = false;
		}

		if (flag) {
            var model = $('#frmActivities').serialize();
            model += '&FOC=foc&Price=price';
            $.ajax({
                type: "POST",
                url: "/Activities/Activities",
                data: model,
                dataType: "html",
                async: false,
                success: function (response) {
                    if (response.indexOf('frmActivities') > 0) {
                        $('#frmActivities').replaceWith(response);
                        var form = $("#frmActivities");
                        form.removeData('validator');
                        form.removeData('unobtrusiveValidation');
                        $.validator.unobtrusive.parse(form);
                    }
                    else {
                        var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                        $('#frmActivities .simple-tab-cont').before(successmsg);
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

	$('.QuickPick').click(function () {
		 
        //On quick pick click first save current activities entered
        $('.SaveActivities')[0].click();

        //Then load the quick pick activities

        var QRFId = $("#QRFId").val();
        var prevIndex = -1, newIndex = -1;
        $.ajax({
            type: "GET",
            url: "/Activities/GetQuickPickActivities",
            data: { QRFId: QRFId },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            async: false,
            success: function (result) {
                if (result != undefined && result != null && result.indexOf('frmQuickPickActivities') > 0) {
                    //$('#activities-quice-pick').empty();
                    $('#activities-quice-pick-cont').html(result);
                    var form = $("#frmQuickPickActivities");
                    form.removeData('validator');
                    form.removeData('unobtrusiveValidation');
                    $.validator.unobtrusive.parse(form);
                    InitializeSortable();

                    $(".SaveQuickPickActivities").unbind("click");
                    $('.SaveQuickPickActivities').bind('click', SaveQuickPickActivities);
                }
                else {
                    //$('#activities-quice-pick-cont').html(result);
                    var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                    $('#frmActivities .simple-tab-cont').before(successmsg);
                }
            },
            error: function (error) {
                alert(error.statusText);
            }
        });
    });

    $(document).on('click', '.disable-quick-pick', function (e) {
        e.preventDefault();
    });

    $(document).on('click', '.chkQuickPickSelected', function () {
        var row = $(this).closest('tr');
        if ($(this).is(":checked")) {
            if (row.find('select.bindProductRange > option').length < 1) {
                AutoCompleteChanged(row.find('input[type="hidden"][id*="__ProdID"]'));
            }
            row.find('td[data-label="ProductCategory"] > div').show();
            row.find('td.divRoomeDetails > div.lnk-dotted').show();
        }
        else {
            row.find('td[data-label="ProductCategory"] > div').hide();
            row.find('td.divRoomeDetails > div.lnk-dotted').hide();
        }
    });

    $(document).on('change', '.fromcityUI', function () {
        var curRow = $(this).closest('tr');
        curRow.find("input.clsProductID").val('');
        curRow.find("input.bindProductName").val('');
        curRow.find("input[name*='SupplierID']").val('');
        curRow.find("input[name*='SupplierName']").val('');

        curRow.find("input[name*='TourType']").val('');
        curRow.find("select[name*='TourTypeID']").empty().append($("<option></option>").attr("value", '').text('Select'));
        
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

function InitializeSortable() {
    $("#sortable").sortable({
        placeholder: "ui-state-highlight",
        items: "tr:not(.ui-state-disabled)",
        start: function () {
            prevIndex = arguments[1].item[0].rowIndex;
        },
        stop: function () {
            newIndex = arguments[1].item[0].rowIndex;
            $('#sortable > tr.QuickPickRow').each(function (idx) {
                var rowidx = this.rowIndex;
                $(this).find('#QPSeqNo').val(rowidx);
            });
        }
    });
}

function addRow(thisparam) {

    var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();     //Get Row Index from hidden field
    var clonedRow = $(thisparam).closest('tr').clone();                 //Clone row to append

    //Remove Proce/Foc section data before adding
    var tdPrice = clonedRow.find('td.light-yellow-bg');
    if (tdPrice.length > 0) {
        tdPrice.removeClass('light-yellow-bg').find('.Prices span.msg, .Prices span.msgErr').hide();
        tdPrice.find('.clsAddPrice').text('Prices');
        tdPrice.find('.clsAddFOC').text('FOC');
        tdPrice.find('.tooltip-cont #frmActivities table tbody').empty();
    }

    //Append cloned row after current row
    $(thisparam).closest('tr').after(clonedRow);

    $('#tblActivities > tbody > tr.ActivitiesRow').each(function (iRowCnt) {
        if (iRowCnt > index) {
            var thisRow = this;
            var curRow = $(this).find('#hdnSeqNo').val();
            this.id = this.id.replace(curRow, iRowCnt);
            $(this).find('input,select,span,textarea,td,ul,div').each(function (iColCnt) {
                if (this.id != undefined) {
                    this.id = this.id.replace(curRow, iRowCnt);
                    if (this.id == 'hdnSeqNo') this.value = iRowCnt;
                    else if (this.id.indexOf('ddlDay_') > -1 || this.id.indexOf('KeepAs') > -1 || this.id.indexOf('CityID') > -1 || this.id.indexOf('ddlDayID_') > -1) this.value = this.value;
                    //else if (this.id.indexOf('ddlDayID_') > -1) {
                    //     
                    //    this.value = $('#ddlDayID_' + (parseInt(curRow) + 1)).val();
                    //}
                    else {
                        if (iRowCnt == (parseInt(index) + 1)) {
                            if (this.id.indexOf('NoOfPax') > -1) { $(thisRow).find('.NoOfPaxColumn').hide(); }
                            else if (this.id.indexOf('StartTime') > -1) {
                                var fullStartTime = $('#ActivityDetails_' + index + '__StartTime').val();
                                var startTime = fullStartTime.substring(0, 2)
                                var startTimeM = fullStartTime.substring(3, 5)
                                if (startTime !== undefined && startTime != "" && startTime.length == 2) {
                                    startTime = parseInt(startTime);
                                    if (startTime < 22) {
                                        startTime = startTime + 3;
                                    }
                                    else {
                                        try {
                                            var ddlDay = $(this).closest('td').find('select.ddlday')[0];
                                            var thiscity = ddlDay.selectedOptions[0].attributes['cityname'].nodeValue;
                                            var nextcity = ddlDay.selectedOptions[0].nextElementSibling.attributes['cityname'].nodeValue;
                                            var nextvalue = ddlDay.selectedOptions[0].nextElementSibling.value;//selectedIndex + 1;
                                            if (thiscity === nextcity) {
                                                $('#' + ddlDay.id).val(nextvalue);
                                                //ddlDay.selectedIndex = nextvalue;
                                                $(ddlDay).change();
                                                startTime = 10;
                                                //add code to remove and add selected attribute
                                            }
                                        } catch (e) {

                                        }
                                    }
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
                            else if (this.id.indexOf('__TransferDetails') > -1) {
                                this.value = "Through Coach";
                            }
                            else this.value = '';

                            if (this.id.indexOf('TourTypeID') > -1 || this.id.indexOf('ProductRangeID') > -1 || this.id.indexOf('SupplementID') > -1) {
                                $(this).empty();
                                $(this).append($("<option></option>").attr("value", '').text('Select'));
                            }
                        }
                    }
                }
                // && this.name.indexOf('rdoTransfer') < 0
                if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
                if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
            });
        }
    });

    var form = $("#frmActivities");
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
}

function removeRow(thisparam, rowIndex) {
    if ($('#tblActivities tbody tr').length <= 1) {
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
                //result = $.parseJSON(result);
                if (hdnId.indexOf('QuickPickActivities') > -1) {
                    var $elem = $(thisparam).closest('tr').find('select[id*="__ProdCategoryID"]');
                    $($elem).empty();
                    $.each(result, function (key, value) {
                        $($elem).append($("<option></option>").val(value.value).html(value.label));
                    });
                    $($elem).show();
                    $($elem).change();
                }
                else {
                    var $elem = $(thisparam).closest('td').find('select[id*="__TourType"]');
                    $($elem).empty();
                    $.each(result, function (key, value) {
                        $($elem).append($("<option></option>").val(value.value).html(value.label));
                    });
                    $($elem).change();
                }
            },
            error: function (error) {
                alert(error.statusText);
            }
        });
    }
}

function SaveQuickPickActivities() {
	var flag = true;
	if (!(RoomValidation("#frmQuickPickActivities")))
		flag = false;

	if (!$('#frmQuickPickActivities').valid()) {
		flag = false;
	}

	if (flag) {
        $(this).prop('disabled', true);
        var model = $('#frmQuickPickActivities').serialize();
        $.ajax({
            type: "POST",
            url: "/Activities/SaveQuickPickActivities",
            data: model,
            dataType: "html",
            success: function (response) {
                $('.close-popup').click();
                if (response.indexOf('frmActivities') > 0) {
                    $('#frmActivities').replaceWith(response);
                    var form = $("#frmActivities");
                    form.removeData('validator');
                    form.removeData('unobtrusiveValidation');
                    $.validator.unobtrusive.parse(form);
                }
                else {
                    var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                    $('#frmActivities .simple-tab-cont').before(successmsg);
                }
                $(this).prop('disabled', false);
            },
            error: function (response) {
                //$('.close-popup').click();
                alert(response.statusText);
                $(this).prop('disabled', false);
            }
        });
    }
    else {
        alert('Please fill mandatory fields!!!');
    }
}

$("#tblActivities").on('focusout', '.clsStartTime', function () {
    var startTime = $(this).val();
    startTime = startTime.replace(/\_/g, '0');
    var endTime = addMinutesToTime(startTime, '90');
    $(this).parents('tr').find(".clsEndTime").val(endTime);
});


//$("#sortable > tr").mousedown(function (e) {
//    if (e.target.className != 'chkQuickPickSelected') {
//        $(this).css("background-color", "#a5f6ca").css("cursor", " n-resize");
//    }
//}).mouseup(function () {
//    $(this).css("background-color", "transparent").css("cursor", "default");
//});
