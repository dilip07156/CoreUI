
$(document).ready(function () {
    BindPaxSlabLabel();
    $("#frmGuesstimate tbody .guessPrice").each(function (index) {
        var price = $(this).text();
        if (price.trim() == '-')
            $(this).css("background-color", "#F3F3F3");
    });
    $(".filter").change(function () {
        GetGuesstimateData("", "");
    });

    $(".saveVersion").click(function () {
        //var versionId = $("#VersionId").val();
        //$("#Version-popup").find("#newVersionId").text(parseInt(versionId) + 1);
        $("#Version-popup").show();
    });
    $(".saveNewVersion").click(function () {
        $("#IsNewVersion").val('true');
        saveGuesstimate();
        $('#Version-popup .close-popup').click();
    });

    $('.ReCalcGuesstimate').click(function (e) {
        var CalculateFor = $("#CalculateFor:checked").val();
        GetGuesstimateData("", "", CalculateFor);
    });

    $('.showRecalculatePUP').click(function (e) {
        $("#ftrChangeRule").hide();
        $("#ftrSFDatabse").show();
        $("#costs-guesstimate-recalculate-costs").show();
    });

    $('.ChangeRuleSave').click(function (e) {
        var GuesstimateId = $("#GuesstimateId").val();
        var ChangeRule = $("#ChangeRule:checked").val();
        var ChangeRulePercent = $("#ChangeRulePercent").val();

        if (ChangeRule != 'LPP')
            ChangeRulePercent = null;
        $.ajax({
            type: "POST",
            url: "/Guesstimate/SetGuesstimateChangeRule",
            data: { GuesstimateId: GuesstimateId, ChangeRule: ChangeRule, ChangeRulePercent: ChangeRulePercent },
            dataType: "json",
            success: function (response) {
                $("#ftrChangeRule").show();
                $("#ftrSFDatabse").hide();
                $("#costs-guesstimate-recalculate-costs").show();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
    
    $(".showVersion").click(function () {
        var QRFId = $("#QRFId").val();
        $.ajax({
            type: "GET",
            url: "/Guesstimate/GetGuesstimateVersions",
            data: { QRFId: QRFId },
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            //headers: { 'IsAjaxRequest': 'true' },
            success: function (response) {

                $("#VersionList-popup .popup-in").html(response);
                $("#VersionList-popup").show();
            },
            error: function (response) {
                alert(response.statusText);
            }
        });
    });
    
    $('.ddlSupplier').click(function (e) {
        var supplierId = $(this).val();
        var ddlSupplier = $(this);
        var supplierCount = $(ddlSupplier).children('option').length;
        if (supplierCount <= 1) {
            var ProductId = $(this.closest('td')).find('#ProductId').val();
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
                            $(ddlSupplier).append($('<option cur="' + value.currency + '" curid="' + value.currencyId+'"></option>').val(value.supplierId).html(value.supplierName));
                        });
                    }
                    $(ddlSupplier).val(supplierId);
                },
                error: function (response) {
                    alert(response.statusText);
                }
            });
        }
    });
    
    $(".KeepZero").click(function () {
        var KeepZeroFlag = $(this).is(':checked');
        var currentRow = $(this.closest('tr'));

        if (KeepZeroFlag) {
            $(currentRow).find(".clsBudgetPrice").each(function (index) {
                $(this).val(0);
                $(this).prop("disabled","disabled");
            });
        }
        else
        {
            $(currentRow).find(".ddlSupplier").change();
            $(currentRow).find(".clsBudgetPrice").each(function (index) {
                $(this).prop("disabled", "");
            });
        }
    });

});

function saveGuesstimate(IsCostsheetSave) {
    $(".ajaxloader").show();
    var model = $("#frmGuesstimate").serialize();
    var NextVersionId = $("#NextVersionId").val();
    var VersionName = $("#VersionName").val();
    var VersionDescription = $("#VersionDescription").val();
    var selectedDepartureDate = $("#DepartureDate option:selected").val();
    var selectedPaxSlab = $("#PaxSlab option:selected").val();
    model += '&NextVersionId=' + NextVersionId + '&Guesstimate.VersionName=' + VersionName + '&Guesstimate.VersionDescription=' + VersionDescription
        + '&Guesstimate.DepartureDate=' + selectedDepartureDate + '&Guesstimate.PaxSlab=' + selectedPaxSlab;

    $.ajax({
        type: "POST",
        url: "/Guesstimate/SetGuesstimatePrices",
        data: model,
        global: false,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
           
            if (response.status == 'Success') {
                if (IsCostsheetSave == "YES")
                    createCostsheet();
                else {
                    GetGuesstimateData(response.errorMessage, "","",true);
                }
            }
            else {
                GetGuesstimateData("", response.errorMessage);
            }
        },
        failure: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        },
        error: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        }
    });
}

function changeCurrentVersion() {
    $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    var GuesstimateId = $("#SelectedVersion:checked").val();

    $.ajax({
        type: "POST",
        url: "/Guesstimate/UpdateGuesstimateVersion",
        data: { QRFId: QRFId, GuesstimateId: GuesstimateId },
        global: false,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $('#VersionList-popup .close-popup').click();
            if (response.status == 'Success') {
                GetGuesstimateData(response.errorMessage, "","",true);
            }
            else {
                GetGuesstimateData("", response.errorMessage);
            }
        },
        failure: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        },
        error: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        }
    });
}

function createCostsheet() {
    $(".ajaxloader").show();
    var QRFId = $("#QRFId").val();
    $.ajax({
        type: "POST",
        url: "/Costsheet/SetCostsheetNewVersion",
        data: { QRFId: QRFId, Pipeline: "Guesstimate" },
        global: false,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.status == 'Success') {
                GetGuesstimateData(response.errorMessage, "");
            }
            else {
                GetGuesstimateData("", response.errorMessage);
            }
        },
        failure: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        },
        error: function (response) {
            alert(response.responseText);
            $(".ajaxloader").hide();
        }
    });
}

function BindPaxSlabLabel() {
    var selectedPaxSlab = $("#PaxSlab option:selected").text();
    var selectedDep = $("#DepartureDate option:selected").text();
    $("#lblPaxSlab").text(selectedDep + ' -- ' + selectedPaxSlab);
}