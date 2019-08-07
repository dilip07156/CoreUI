
$(document).ready(function () {
    var cloneIndex = $(".clonedInput").length - 1;
    var regex = /^(.+?)(\d+)$/i;
    var regexForName = /^(.+?)(\d+)(.+?)$/i;
    bindRoutingCitiesLable();
    EnableDisStarRating();

    $("a.clone").on("click", clone);
    $("a.remove").on("click", remove);
    $(".bindCity").on("keyup", getCityNames);

    $(document).on("focusout", ".noNight", function (event) {

        var thisparam = $(this);
        var nights = thisparam.val();
        if (nights > 0)
            thisparam.parents("tr").find(".clsStarRating").css("display", "block");
        else
            thisparam.parents("tr").find(".clsStarRating").css("display", "none");
    });

    $('.move-up').click(function () {
        if ($(this).prev()) {
            var t = $(this);
            t.parents(".clonedInput").animate({ top: '-20px' }, 0, function () {
                t.parents(".clonedInput").prev().animate({ top: '20px' }, 0, function () {
                    t.parents(".clonedInput").css('top', '0px');
                    t.parents(".clonedInput").prev().css('top', '0px');
                    t.parents(".clonedInput").insertBefore(t.parents(".clonedInput").prev());
                });
            });
        }
        bindRowSequence();
    });

    $('.move-down').click(function () {
        if ($(this).next()) {
            var t = $(this);
            t.parents(".clonedInput").animate({ top: '20px' }, 0, function () {
                t.parents(".clonedInput").next().animate({ top: '-20px' }, 0, function () {
                    t.parents(".clonedInput").css('top', '0px');
                    t.parents(".clonedInput").next().css('top', '0px');
                    t.parents(".clonedInput").insertAfter(t.parents(".clonedInput").next());
                });
            });
        }
        bindRowSequence();
    });

    $(".AutoPopNights").on("focusout", AutoPopNights);

    function AutoPopNights() {
        var days = $(this).val();
        if (days == '' || days == 0) {
            $(this).siblings("input[type=text]").val('');
        }
        else if (days == 1) {
            $(this).siblings("input[type=text]").val(days);
        }
        else if (days != 1) {
            days = parseInt(days) - 1;
            $(this).siblings("input[type=text]").val(days);
        }
    }

    function clone() {

        var emptyfields = validateRoutingTable();

        if (!emptyfields) {
            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");//info
            $("#lblmsg").text("Please fill the routing details.");
            $("#stMsg").text("Error! ");
            return false;
        }


        var currentRow = "#" + $(this).parents(".clonedInput").attr('id');
        $("#clonedInput_00").clone(true, true)
            .insertAfter(currentRow)
            .attr("id", "clonedInput_" + cloneIndex)
            .slideDown()
            .find("*")
            .each(function () {
                var id = this.id || "";
                var match = id.match(regex) || [];
                if (match.length == 3) {
                    this.id = match[1] + cloneIndex;
                    if (match[1] == "IsLocalGuide_") {
                        $(this).removeAttr("Checked");
                        $(this).attr("value", "true");
                    }
                    if (match[1] == "spanValidationMsg_") {
                        var dataValMsgFor = $(this).attr("data-valmsg-for");
                        $(this).attr("data-valmsg-for", dataValMsgFor.replace('0', cloneIndex));
                    }
                }

                var name = this.name || "";
                var match = name.match(regexForName) || [];
                if (match.length == 4) {
                    this.name = match[1] + cloneIndex + match[3];
                }
            })
            .find('input').not("input[type=checkbox]").val('')
            .on('click', 'a.clone', clone)
            .on('click', 'a.remove', remove)
            .on('keypress', '.bindCity', getCityNames);

        jQuery("#clonedInput_" + cloneIndex).find('select').val('');

        jQuery("#clonedInput_" + cloneIndex + " td").each(function (index) {
            jQuery(this).css("display", "none");
        });

        $("#clonedInput_" + cloneIndex + " td").slideDown();

        cloneIndex++;
        var form = $("#frmQuoteRouting");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        bindRowSequence();
    }

    function remove() {

        $("#divSuccessError").hide();
        if ($('#tblRouting tbody tr.clonedInput').size() == 1) {
            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");//info
            $("#lblmsg").text("Cannot delete, at least one record is needed.");
            $("#stMsg").text("Error! ");
            return false;
        }

        var sequenceId = $(this).parents(".clonedInput").find("input[type=hidden]")[0].id;
        $("#" + sequenceId).val("0");

        var cloneId = "#" + $(this).parents(".clonedInput")[0].id;
        $(cloneId + " td").slideUp();

        $(this).parents(".clonedInput").removeClass("clonedInput");
        bindRowSequence();
    }

    function bindRowSequence() {
        jQuery(".tblRoutingData .clonedInput").each(function (index) {
            var seqId = "#" + jQuery(this).find("input[type=hidden]")[0].id;
            $(seqId).val(index + 1);
        });
        bindRoutingCitiesLable();
    }

    function bindRoutingCitiesLable() {
        var cloneIndex = $(".clonedInput").length - 1;
        var routingCities = "";
        jQuery(".tblRoutingData .clonedInput").each(function (index) {
            var routingCitiesId = "#" + jQuery(this).find("input[type=hidden]")[4].id;
            if ($(routingCitiesId).val() != '') {
                var cityName = $(routingCitiesId).val().split(',');
                routingCities = routingCities + cityName[0] + ", ";
            }
        });
        routingCities = routingCities.substr(0, routingCities.length - 2)
        $("#lblRoutingCities").text(routingCities);
        hideArrowOfFirstAndLastRow();
    }

    function hideArrowOfFirstAndLastRow() {
        var lastSeqValue = 0;
        jQuery(".tblRoutingData .clonedInput").each(function (index) {
            var seqId = "#" + jQuery(this).find("input[type=hidden]")[0].id;
            var seqIdValue = $(seqId).val();

            if (seqIdValue == 1)
                jQuery(this).find(".move-up").hide();
            else
                jQuery(this).find(".move-up").show();

            if (seqIdValue > 0)
                lastSeqValue = seqIdValue;
        });

        jQuery(".tblRoutingData .clonedInput").each(function (index) {
            var seqId = "#" + jQuery(this).find("input[type=hidden]")[0].id;
            var seqIdValue = $(seqId).val();

            if (seqIdValue == lastSeqValue)
                jQuery(this).find(".move-down").hide();
            else
                jQuery(this).find(".move-down").show();
        });
    }

    function getCityNames() {

        var actionUrl = "GetCityName";
        var controlId = "#" + $(this).attr('id');
        var hdnId = "#hdn" + $(this).attr('id');
        var minlen = 3;

        if (actionUrl != undefined && actionUrl != "") {
            var id = controlId;
            id = id.replace('UI', '').replace('#', '');
            $(controlId).autocomplete({
                source: actionUrl,
                minLength: 3,
                select: function (e, ui) {
                    if (ui.item) {
                        $(controlId).val(ui.item.label);
                        $(hdnId).val(ui.item.label);
                        $("#" + id).val(ui.item.value);
                        if (ui.item.type != undefined && ui.item.type != '') $(hdnId + 'Type').val(ui.item.type);
                        if (ui.item.typeId != undefined && ui.item.typeId != '') $(hdnId + 'TypeID').val(ui.item.typeId);
                        if (ui.item.code != undefined && ui.item.code != '') $(hdnId + 'Code').val(ui.item.code);

                        ui.item.placeholder = (ui.item.placeholder == true && ui.item.placeholder != null) ? "true" : "false";
                        if ($(hdnId + 'PlaceHolder') != undefined) $(hdnId + 'PlaceHolder').val(ui.item.placeholder);

                        $(".ui-menu-item").hide();
                        $(".ui-menu").css("border", "none");
                        e.preventDefault();
                    } else {//if list null then hide the list 
                        $(".ui-menu-item").hide();
                        $(".ui-menu").css("border", "none");
                    }
                },
                focus: function (e, ui) { //on key down/up ID shows in textbox to handle this focus event used 
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
                        $(hdnId).val('');
                    }
                    if ((e.which == 0 || e.which === 13)) {
                        bindRoutingCitiesLable();
                    }
                }
            }).unbind('keypress').bind('keypress', function (e) {
                if ($(controlId).val().length >= minlen) {
                    $(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
                }
                else if ($(controlId).val().length < minlen) {
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                    $(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
                }
                if (e.which === 13 || e.which === 27 || e.which === 0) {//on enter or escape or tab key hide list
                    $(".ui-menu-item").hide();
                    $(".ui-menu").css("border", "none");
                    $(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
                }
            }).bind('blur', function (e) { //if char in textbox is 1 then hide the list     
                if (e.which === 8 || e.which === 0) {//on backspace and tab key
                    if ($(controlId).val().length < minlen) {
                        $(".ui-autocomplete").css("width", "0px"); //add for handling width on browsers
                        $(".ui-menu-item").hide();
                        $(".ui-menu").css("border", "none");
                    }
                }
            });
        }

        //if (actionUrl != undefined && actionUrl != "") {
        //    var id = controlId;
        //    id = id.replace('UI', '').replace('#', '');
        //    $(controlId).autocomplete({
        //        source: actionUrl,
        //        minLength: 3,
        //        select: function (event, ui) {
        //            if (ui != null) {
        //                $(controlId).val(ui.item.label);
        //                $(hdncontrolId).val(ui.item.label);
        //                $("#" + id).val(ui.item.value);
        //                $(".ui-menu-item").hide();
        //                $(".ui-menu").css("border", "none");
        //                bindRoutingCitiesLable();
        //                event.preventDefault();
        //            }
        //        }
        //    }).keyup(function (e) {
        //        if (e.which === 13 || e.which === 27) {
        //            $(".ui-menu-item").hide();
        //            $(".ui-menu").css("border", "none");
        //        }
        //        else if (e.which === 38 || e.which === 40) {
        //        }
        //    });
        //}
    }

    $('#IsSetPrefHotels').click(function () {
        EnableDisStarRating();
    });



    $('.routingSave').click(function () {

        var IsSetPrefHotels = $('#IsSetPrefHotels').is(":checked");
        if (IsSetPrefHotels == true) {
            var IsRoutingExist = $('#IsRoutingExist').val();

            if (IsRoutingExist == "True") {
                var res = confirm("Do you want to overwrite existing position with new preferred hotels?");
                $('#IsOverwriteExtPos').val(res);
            }
            else
                $('#IsOverwriteExtPos').val(true);
        }

        if ($('#frmQuoteRouting').valid() && validateRoutingTable()) {
            var model = $('#frmQuoteRouting').serialize();
            $.ajax({
                type: "POST",
                url: "/Quote/QuoteRouting",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmQuoteRouting') > 0) {
                        var sum = 0;
                        $("#tblRouting tr:not([style*='display:none'])").find(".noNight").each(function () { 
                            sum += parseInt(this.value) || 0;
                        });

                        $("#spNoOfDays").text(sum + 1);
                        $("#spNights").text(sum);
                        $('#Margins').click();
                        //$('#frmQuoteRouting').replaceWith(response);
                        //var form = $("#frmQuoteRouting");
                        //form.removeData('validator');
                        //form.removeData('unobtrusiveValidation');
                        //$.validator.unobtrusive.parse(form);
                    }
                    else {
                        var errormsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Routing Details not saved.</div>';
                        $('.simple-tab-cont').before(errormsg);
                    }
                },
                error: function (response) {
                    var errormsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.statusText + '</div>';
                    $('.simple-tab-cont').before(errormsg);
                }
            });
        }
        else {
            //alert('Please fill the mandatory fields.');
            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");//info
            $("#lblmsg").text("Please fill the routing details.");
            $("#stMsg").text("Error! ");
        }

    });
});

$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});

function validateRoutingTable() {

    var emptyfields = true;
    $("#divSuccessError").hide();

    $('#tblRouting tbody tr').each(function (/*index, element*/) {

        $(this).find(".fromCity").siblings("span").text("");
        $(this).find(".toCity").siblings("span").text("");
        $(this).find(".noDay").siblings("span").text("");
        $(this).find(".noNight").siblings("span").text("");

        if (($(this).find('td:not([style*="display: none;"])').length) > 0) {
            if ($(this).find('.fromCity').val() == "" || $(this).find('.fromCity').val() == undefined) {
                $(this).find(".fromCity").siblings("span").text("*")
                emptyfields = false;
            }
            if ($(this).find('.toCity').val() == "" || $(this).find('.toCity').val() == undefined) {
                $(this).find(".toCity").siblings("span").text("*")
                emptyfields = false;
            }
            if ($(this).find('.noDays').val() == "" || $(this).find('.noDay').val() == undefined || $(this).find('.noNight').val() == "" || $(this).find('.noNight').val() == undefined) {
                $(this).find('.noDays').val() == "" || $(this).find('.noDay').val() == undefined ? $(this).find(".noDay").siblings("span").text("*") : $(this).find(".noDay").siblings("span").text("");
                $(this).find('.noNight').val() == "" || $(this).find('.noNight').val() == undefined ? $(this).find(".noNight").siblings("span").text("*") : $(this).find(".noNight").siblings("span").text("");
                emptyfields = false;
            }
        }
    });

    return emptyfields;
}

function EnableDisStarRating() {
    var IsSetPrefHotels = $('#IsSetPrefHotels').is(":checked");
    if (IsSetPrefHotels == true) {
        $('.clsStarRating').prop('disabled', false);
    }
    else {
        $('.clsStarRating').val('');
        $('.clsStarRating').prop('disabled', true);
    }
}
