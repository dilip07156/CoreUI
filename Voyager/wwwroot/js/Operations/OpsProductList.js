$(document).ready(function () {
    $('.dataTables').DataTable({
        "ordering": false,
        "aLengthMenu": [10, 25, 50, 100],
        "iDisplayLength": 10,
        "searching": true,
        "lengthChange": true,
        "oLanguage": {
            sLengthMenu: "Show entries _MENU_",
        },
        "dom": "<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>" + "<'row'<'col-sm-12'tr>>" + "<'row'<'col-sm-5 margin-top-bottom-20'i><'col-sm-7'p>>",
    });
    
    $(document).off("change").on('change', '.ddlSupplier', function (evt) {
        var currentRow = $(this.closest('tr'));
        var ProductId = $(currentRow).find(".clsProductId").val();
        var SupplierId = $(this).val();
        var PositionDate = $("#PositionEndDate").val();

        $.ajax({
            type: "GET",
            url: "/Operations/GetContractRatesByProductID",
            data: { ProductId: ProductId, SupplierId: SupplierId, PositionDate: PositionDate },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {

                $(currentRow).find(".divContractPrice").each(function (index) {
                    var RangeId = $(this).find(".clsRangeId").val();
                    var ContractPrice = $(this).find(".spnContractPrice");
                    $(ContractPrice).text("No Contract");

                    $.each(result.productContractInfo, function (key, value) {
                        if (value.productRangeId == RangeId) {
                            $(ContractPrice).text(value.currency + " " + value.price);
                        }
                    });
                });
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });

    $("#confirmOk").unbind("click");
    $("#confirmOk").click(function () {

        //$.magnificPopup.close();
        $('#confirmOk').magnificPopup('close');

        // 500, time must be bigger, as the delay from the first popup. By me the delay is 300
        var value = $("#confirmOk").attr("data-value");
        if (value === "switchProduct") {
            window.setTimeout(switchProduct, 400);
        }
        else if (value === "addPosition") {
            window.setTimeout(AddPosition, 400);
        }
    });

    $(".close-popup").unbind("click");
    $(".close-popup").click(function () {
        debugger;
        if ($(this).text() == "Ok") {
            var value = $(this).attr("data-value");
            if (value === "addPosition") {
                location.reload();
            }
            else {
                var PositionId = $('#Position_Id').val();
                SetProdTypeActive($("#ulOpsProdType li.active a"), PositionId);
            }
        }
    });

    function switchProduct() {
        var switchProductData = {};
        $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

        var BookingNumber = $("#BookingNumber").val();
        var PositionId = $('#Position_Id').val();
        switchProductData['TableChangeSuppPlace'] = true;
        switchProductData['BookingNumber'] = BookingNumber;
        switchProductData['PositionId'] = PositionId;

        var switchCurrentRow = $("#hdnswitchCurrentRow").val();
        var switchCurrenttd = $("#hdnswitchCurrenttd").val();

        var ProductId = $("#" + switchCurrentRow).find(".clsProductId").val();
        var SupplierId = $("#" + switchCurrentRow).find(".ddlSupplier option:selected").val();
        var ProductCategoryId = $("#" + switchCurrentRow).find("#" + switchCurrenttd).find(".clsProductCategoryId").val();
        var ProductRangeId = $("#" + switchCurrentRow).find("#" + switchCurrenttd).find(".clsProductRangeId").val();
        switchProductData['ProductId'] = ProductId;
        switchProductData['SupplierId'] = SupplierId;
        switchProductData['ProductCategoryId'] = ProductCategoryId;
        switchProductData['ProductRangeId'] = ProductRangeId;

        switchProductData['Action'] = "ChangeSupplier_PlaceHolder";
        switchProductData['Module'] = "Supplier";
        switchProductData['ModuleParent'] = "Position";

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: switchProductData },
            dataType: "json",
            success: function (response) {
                var htmlMsg = "";
                console.log(response);
                if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                    if (response.status.toLowerCase() !== "success") {
                        $("#commonMsg-popup #headerCommonMsg").html('Error!');
                        var msg = response.msg;
                        msg = msg.filter(a => a != "" && a != undefined && a != null);
                        for (var i = 0; i < msg.length; i++) {
                            htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                        }
                    }
                    else {
                        $("#commonMsg-popup #headerCommonMsg").html('Success!');
                        htmlMsg = '<div class="alert alert-success" role="alert">Product Switched Successfully</div>';
                    }
                } else {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred.</div>';
                }
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowMagnificPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = "";
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status === 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status === 500) {
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
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                $("#commonMsg-popup #pCommonMsg").html('<div class="alert alert-danger" role="alert">' + msg + '</div>');
                ShowPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            }
        });
    }

    function AddPosition() {
        var addPositionData = {};
        $("#commonMsg-popup #headerCommonMsg,#commonMsg-popup #pCommonMsg").html('');

        var BookingNumber = $("#BookingNumber").val();
        addPositionData['TableAddPosition'] = true;
        addPositionData['BookingNumber'] = BookingNumber;
        addPositionData['PositionId'] = NewGuid();

        var PositionType = $("#divAddPosition #PositionType option:selected").val();
        var PositionStartDate = $("#divAddPosition #PositionStartDate").val();
        var NoOfDays = $("#divAddPosition #NoOfDays").val();
        var StartTime = $("#divAddPosition #StartTime").val();
        var EndTime = $("#divAddPosition #EndTime").val();
        addPositionData['PositionType'] = PositionType;
        addPositionData['PositionStartDate'] = PositionStartDate;
        addPositionData['NoOfDays'] = NoOfDays;
        addPositionData['StartTime'] = StartTime;
        addPositionData['EndTime'] = EndTime;
        
        var switchCurrentRow = $("#hdnswitchCurrentRow").val();
        var switchCurrenttd = $("#hdnswitchCurrenttd").val();

        var ProductId = $("#" + switchCurrentRow).find(".clsProductId").val();
        var SupplierId = $("#" + switchCurrentRow).find(".ddlSupplier option:selected").val();
        var ProductCategoryId = $("#" + switchCurrentRow).find("#" + switchCurrenttd).find(".clsProductCategoryId").val();
        var ProductRangeId = $("#" + switchCurrentRow).find("#" + switchCurrenttd).find(".clsProductRangeId").val();
        addPositionData['ProductId'] = ProductId;
        addPositionData['SupplierId'] = SupplierId;
        addPositionData['ProductCategoryId'] = ProductCategoryId;
        addPositionData['ProductRangeId'] = ProductRangeId;

        addPositionData['Action'] = "AddPosition";
        addPositionData['Module'] = "Position";
        addPositionData['ModuleParent'] = "Booking";

        $.ajax({
            type: "POST",
            url: "/Operations/SetBookingByWorkflow",
            data: { model: addPositionData },
            dataType: "json",
            success: function (response) {
                var htmlMsg = "";
                console.log(response);
                if (response !== null && response !== undefined && response.status != null && response.status != undefined && response.status != "") {
                    if (response.status.toLowerCase() !== "success") {
                        $("#commonMsg-popup #headerCommonMsg").html('Error!');
                        var msg = response.msg;
                        msg = msg.filter(a => a != "" && a != undefined && a != null);
                        for (var i = 0; i < msg.length; i++) {
                            htmlMsg += '<div class="alert alert-danger" role="alert"> ' + msg[i] + '</div>';
                        }
                    }
                    else {
                        $("#commonMsg-popup #headerCommonMsg").html('Success!');
                        htmlMsg = '<div class="alert alert-success" role="alert">Position Added Successfully</div>';
                    }
                } else {
                    $("#commonMsg-popup #headerCommonMsg").html('Error!');
                    htmlMsg = '<div class="alert alert-danger" role="alert">An Error Occurred.</div>';
                }
                $("#commonMsg-popup #pCommonMsg").html(htmlMsg);
                ShowMagnificPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            },
            error: function (jqXHR, exception, errorThrown) {
                var msg = "";
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status === 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status === 500) {
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
                $("#commonMsg-popup #headerCommonMsg").html('Error!');
                $("#commonMsg-popup #pCommonMsg").html('<div class="alert alert-danger" role="alert">' + msg + '</div>');
                ShowPopup("#commonMsg-popup", true, true);
                $("#commonMsg-popup").show();
                $(".ajaxloader").hide();
            }
        });
    }

    $(".btnAddPosition").click(function () {
        var current = $(this);
        var isvalid = $("#frmAddPosition").valid();
        if (!isvalid) {
            alert("Please enter required data!!!");
            return false;
        }
        else {
            $("#headerConfirm").html("Add Position");
            $("#pConfirmMsg").html("Are you sure you want to Add Position?");
            $("#confirmOk").attr("data-value", "addPosition");
            $(".close-popup").attr("data-value", "addPosition");

            var currentRow = $(current.closest('tr')).attr("id");
            $("#hdnswitchCurrentRow").val(currentRow);
            var currenttd = $(current.closest('span')).attr("id");
            $("#hdnswitchCurrenttd").val(currenttd);

            $("#Confirm-popup").show();
        }
    });

});

function showSwitchProdPopup(current) {
    $("#headerConfirm").html("Switch Product");
    $("#pConfirmMsg").html("Are you sure you want to switch product with placeholder?");
    $("#confirmOk").attr("data-value", "switchProduct");

    var currentRow = $(current.closest('tr')).attr("id");
    $("#hdnswitchCurrentRow").val(currentRow);
    var currenttd = $(current.closest('span')).attr("id");
    $("#hdnswitchCurrenttd").val(currenttd);

    $("#Confirm-popup").show();
}

function GetProductSupplier(current) {
    var supplierId = $(current).val();
    var ddlSupplier = $(current);
    var supplierCount = $(ddlSupplier).children('option').length;
    if (supplierCount <= 1) {
        var ProductId = $(current.closest('td')).find('.clsProductId').val();
        $.ajax({
            type: "GET",
            url: "/Guesstimate/GetProductSupplierList",
            data: { ProductId: ProductId },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                $(ddlSupplier).empty();
                if (result.supllierList.length > 0) {
                    $.each(result.supllierList, function (key, value) {
                        $(ddlSupplier).append($('<option cur="' + value.currency + '" curid="' + value.currencyId + '"></option>').val(value.supplierId).html(value.supplierName));
                    });
                }
                $(ddlSupplier).val(supplierId);
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    }
}
