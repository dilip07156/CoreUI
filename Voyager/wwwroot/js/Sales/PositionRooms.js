
$(document).ready(function () {
    var regex = /^(.+?)(\d+)$/i;
    var regexForNameRoom = /^(.+?)(\d+)(.+?)(\d+)(.+?)$/i;

    $(document).on('click', 'a.cloneRoom', { cloneType: "Room" }, cloneRoom);
    $(document).on('click', 'a.cloneSupplement', { cloneType: "Supplement" }, cloneRoom);
    $(document).on('click', 'a.removeRoom', removeRoom);

    function cloneRoom(event) {
        var cloneType = '';
        cloneType = event.data.cloneType;
        var cloneIndexRoomAll = $(".clsRoomSequenceId").length - 1;
        var currentTDId = $(this).parents("tr").find(".divRoomeDetails");
        var currentRowId = $(this).closest("tr");

        if (cloneType == 'Supplement') {
            var supplementValue = $(currentRowId).find(".clsSupplement").val();
            if (supplementValue == '') {
                alert('Please select Supplement.');
                return false;
            }
        }

        var cloneRoomIndex = currentTDId.find("li").length;
        var currentRoomRow = currentTDId.find("ul");
        var currentHotelArr = currentRowId.attr('id').match(regex) || [];
        var currentHotel = currentHotelArr[2];

        $("#clonedInput_000").clone(true, true)
            .appendTo(currentRoomRow)
            .attr("id", "clonedInput_" + cloneIndexRoomAll)
            .show()
            .find("*")
            .each(function () {
                var id = this.id || "";
                var match = id.match(regex) || [];
                if (match.length == 3) {
                    this.id = match[1] + cloneIndexRoomAll;
                    if (match[1] == "spanValidationMsg_") {
                        var dataValMsgFor = $(this).attr("data-valmsg-for");
                        var match = dataValMsgFor.match(regexForNameRoom) || [];
                        if (match.length == 6) {
                            $(this).attr("data-valmsg-for", match[1] + currentHotel + match[3] + cloneRoomIndex + match[5]);
                        }
                    }
                }

                var name = this.name || "";
                var match = name.match(regexForNameRoom) || [];
                if (match.length == 6) {
                    this.name = match[1] + currentHotel + match[3] + cloneRoomIndex + match[5];
                }

            })
            .find('input').val('');

        if (cloneType == 'Room') {
            var lastRoomTypeId = currentTDId.find(".clsRoomType").first();
            var newRoomTypeId = currentTDId.find("#clonedInput_" + cloneIndexRoomAll).find(".clsRoomType");
            $(newRoomTypeId).html($(lastRoomTypeId).html());
            $(newRoomTypeId).val('');
            currentTDId.find("#clonedInput_" + cloneIndexRoomAll).find(".clsIsSupplement").val('false');
        }
        else if (cloneType == 'Supplement') {
            var supplementValue = $(currentRowId).find(".clsSupplement").val();
            var supplementId = $(currentRowId).find(".clsSupplement");
            //var supplementText = $('#' + supplementId.attr('id') + " option:selected").text();
            var supplementText = $(currentRowId).find(".clsSupplement option:selected").text();
            var newRoomTypeId = currentTDId.find("#clonedInput_" + cloneIndexRoomAll).find(".clsRoomType");
            $(newRoomTypeId).html('');
            $(newRoomTypeId).append($("<option></option>").val(supplementValue).html(supplementText));
            //$('#' + newRoomTypeId.attr('id').replace('ID', '')).val(supplementText);
            $(newRoomTypeId[0].previousElementSibling).val(supplementText);
            currentTDId.find("#clonedInput_" + cloneIndexRoomAll).find(".clsIsSupplement").val('true');
        }

        $(currentTDId).find("#clonedInput_" + cloneIndexRoomAll + " span").each(function (index) {
            $(this).text("");
        });

        $(currentTDId).find("#clonedInput_" + cloneIndexRoomAll + " div").each(function (index) {
            $(this).css("display", "none");
        });

        $(currentTDId).find("#clonedInput_" + cloneIndexRoomAll + " div").slideDown();

        var form = $("#frmAccomodation");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        bindRoomRowSequence(currentRoomRow);
    }

    function removeRoom() {
        var currentTDId = $(this).parents("tr").find(".divRoomeDetails");
        var cloneRoomIndex = $(currentTDId).find(".clonedInput").length;
        if (cloneRoomIndex == 1) {
            alert('Cannot delete this record.');
            return false;
        }

        var currentRoomRow = $(this).parents(".divAddRemoveRoom").siblings("ul");
        var cloneId = $(currentRoomRow).find(".clonedInput").last();
        $(cloneId).hide().find(".clsRoomSequenceId").val("0");
        $(cloneId).removeClass("clonedInput").find(".clsRoomType").removeClass("clsRoomType");
        bindRoomRowSequence(currentRoomRow);
    }

    function bindRoomRowSequence(current) {
        $(current).find(".clonedInput").each(function (index) {
            var seqId = $(this).find(".clsRoomSequenceId").val(index + 1);
        });
    }

});

function RoomValidation(formName) {
    var flag = true;
    if (formName == "frmAccomodation") {
        $(formName + " tr.clonedHotel .clsRoomType").each(function (index) {
            var RoomTypeVal = $(this).val();
            if (RoomTypeVal == '') {
                $(this).siblings("span").text("*");
                flag = false;
            }
            else
                $(this).siblings("span").text("");
        });
    }
    else if (formName == "#frmQuickPickActivities") {
        if ($(formName).find("tr.QuickPickRow:not(.ui-state-disabled) .chkQuickPickSelected:checked ").length < 1) {
            //alert('No attractions selected.');
            return false;
        }
        $(formName + " tr .chkQuickPickSelected:checked ").each(function (index) {
            var RoomType = $(this).closest('tr.QuickPickRow:not(.ui-state-disabled)').find('.clsRoomType');
            var RoomTypeVal = $(RoomType).val();
            if (RoomTypeVal == '') {
                $(RoomType).siblings("span").text("*");
                flag = false;
            }
            else
                $(RoomType).siblings("span").text("");
        });
    }
    else {
        $(formName + " tr:not([style*='display: none;']) .clsRoomType").each(function (index) {
            var RoomTypeVal = $(this).val();
            if (RoomTypeVal == '') {
                $(this).siblings("span").text("*");
                flag = false;
            }
            else
                $(this).siblings("span").text("");
        });
    }
    return flag;
}
