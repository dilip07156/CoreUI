var indexPaxRange = 1;
var indexHotelCat = 1;
var indexHotelChain = 1;

$(document).ready(function () {
    try {
        var form = $("#frmQuotePaxRange");
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    } catch (e) {
        console.log(e.message);
    }

    $('input[name="HotelsOption"]').click(function (e) {

        if (this.value == 'yes') {
            $('.hotels-opt-cont').show();
        }
        else
            $('.hotels-opt-cont').hide();
    });

    $(".hotel-category").on("keyup", ".bindHotelCat", function () {
        InitializeAutoComplete('/Quote/GetHotelCategory', this, 3, { term: this.value });
    });

    $(".hotel-chain").on("keyup", ".bindHotelChain", function () {
        InitializeAutoComplete('/Accomodation/GetChainName', this, 3, { term: this.value });
    });

    $('#tblPaxRange').on('click', 'tbody tr td .icon-squre-green', function () {
         
        var clonedRow = $('#tblPaxRange tbody tr:first').clone();
        $('#tblPaxRange tbody').append(clonedRow);
        clonedRow.find('input,select,span').each(function (index) {
            if (this.id != undefined) {
                if (this.id === "hdnSeqNo") this.value = indexPaxRange;
                else if (this.id.indexOf('PaxSlabId') > -1) this.value = '0';
                else if (this.id.indexOf('IsDeleted') > -1) this.value = 'False';
                else if (this.id.indexOf('TransportCategory') > -1 || this.id.indexOf('TransportCoachType') > -1) { }
                else if (this.id.indexOf('TransportCount') > -1) { this.value = '1'; }
                else if (this.id.indexOf('BudgetAmount') > -1) { this.value = '0'; }
                else this.value = '';
            }
            else this.value = '';

            if (this.id != undefined) this.id = this.id.replace('0', indexPaxRange);
            if (this.name != undefined) this.name = this.name.replace('0', indexPaxRange);

            if (this.attributes['data-valmsg-for'] != undefined)
                this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace('0', indexPaxRange);

            if (this.attributes['aria-describedby'] != undefined)
                this.attributes['aria-describedby'].nodeValue = this.attributes['aria-describedby'].nodeValue.replace('0', indexPaxRange);
        });

        var form1 = $("#frmQuotePaxRange");
        form1.removeData('validator');
        form1.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form1);
        indexPaxRange++;
        return false;
    });

    $('#tblPaxRange').on('click', 'tbody tr td .icon-squre-red', function () {

        var index = $(this).closest('tr').find('#hdnSeqNo').val();
        var selectedTR = $(this).closest('tr');

        if ($("#tblPaxRange tbody").find('tr:not([style*="display: none;"])').length == 1) {
            selectedTR.find('input[id*="__PaxSlabFrom"]').val("");
            selectedTR.find('input[id*="__PaxSlabTo"]').val("");
            selectedTR.find('input[id*="__PaxSlabDividedBy"]').val("");
        }
        else {
            $(this).closest('tr').hide();
            $('#tblPaxRange #QuotePaxSlabDetails_' + index + '__IsDeleted').val('True');
        }

        return false;
    });

    $('.hotels-opt-cont').on('click', '.hotel-category a.txt-green', function () {
        var clonedRow = $('.hotel-category ul li:first').clone();
        $('.hotel-category ul').append(clonedRow);
        clonedRow.find('input,span').each(function (index) {
            this.value = '';
            this.id = this.id.replace('0', indexHotelCat);
            if (this.name != undefined) this.name = this.name.replace('0', indexHotelCat);
            if (this.id == 'cat_seq') this.innerText = this.innerText.replace('1', indexHotelCat + 1);
            this.focus();
        });
        indexHotelCat++;
        return false;
    });

    $('.hotels-opt-cont').on('click', '.hotel-category a.txt-red', function () {
        if ($('.hotel-category ul li').length > 1) {
            $(this).closest('li').remove();
            indexHotelCat--;
        }
        $('.hotel-category ul li').each(function (iRow) {
            $(this).find('input,span').each(function (iCol) {
                if (this.id == "cat_seq") this.innerText = (parseInt(iRow) + 1) + ' - ';
                else if (this.type == 'text' && this.id.indexOf('HotelCategory') > -1) {
                    this.id = 'HotelCategory_' + iRow + 'UI';
                    this.name = 'HotelCategoryList[' + iRow + '].Value';
                }
                else if (this.type == 'hidden' && this.id.indexOf('HotelCategory') > -1) {
                    this.id = 'HotelCategory_' + iRow;
                    this.name = 'HotelCategoryList[' + iRow + '].AttributeId';
                }
            })
        })
        return false;
    });

    $('.hotels-opt-cont').on('click', '.hotel-chain a.txt-green', function () {
        var clonedRow = $('.hotel-chain ul li:first').clone();
        $('.hotel-chain ul').append(clonedRow);
        clonedRow.find('input,span').each(function (index) {
            this.value = '';
            this.id = this.id.replace('0', indexHotelChain);
            if (this.name != undefined) this.name = this.name.replace('0', indexHotelChain);
            if (this.id == 'chain_seq') this.innerText = this.innerText.replace('1', indexHotelChain + 1);
            this.focus();
        });
        indexHotelChain++;
        return false;
    });

    $('.hotels-opt-cont').on('click', '.hotel-chain a.txt-red', function () {
        if ($('.hotel-chain ul li').length > 1) {
            $(this).closest('li').remove();
            indexHotelChain--;
        }
        $('.hotel-chain ul li').each(function (iRow) {
            $(this).find('input,span').each(function (iCol) {
                if (this.id == "chain_seq") this.innerText = (parseInt(iRow) + 1) + ' - ';
                else if (this.type == 'text' && this.id.indexOf('HotelChain') > -1) {
                    this.id = 'HotelChain_' + iRow + 'UI';
                    this.name = 'HotelChainList[' + iRow + '].Value';
                }
                else if (this.type == 'hidden' && this.id.indexOf('HotelChain') > -1) {
                    this.id = 'HotelChain_' + iRow;
                    this.name = 'HotelChainList[' + iRow + '].AttributeId';
                }
            })
        })
        return false;
    });

    $('#PaxSlabs').click(function () {
        $('#SlabFrom').val("");
        $('#SlabTo').val("");
        $('#SlabGap').val("");
        $('.slab-box').show();
        $('.slab-box').collapse('show');
        return false;
    });

    $('.slab-box .btn-close').click(function () {
        $('.slab-box').collapse('hide');
        setTimeout(function () { $('.slab-box').hide(); }, 300);
        return false;
    });

    $('#GenerateSlabs').click(function () {
        
        var SlabFrom = $('#SlabFrom').val(); var SlabTo = $('#SlabTo').val(); var SlabGap = $('#SlabGap').val();
        var prevRowCnt = parseInt($('#tblPaxRange tbody tr').length); var RowIndex = 0;
        var slabsArray = new Array();

        SlabFrom = parseInt(SlabFrom) - 1; SlabTo = parseInt(SlabTo); SlabGap = parseInt(SlabGap);

        if (isNaN(SlabFrom) || isNaN(SlabTo) || isNaN(SlabGap)) {
            alert('Please enter a valid number!!!');
            return false;
        }

        var isExist = false;
        var tableFrom = 0;
        var tableTo = 0;
        $('#tblPaxRange tbody tr').each(function (index, element) {
            //if (element.style[0] != "display") {
                index == 0 ? tableFrom = parseInt(element.firstElementChild.children[3].value) : 0;
                var elementTo = parseInt(element.children[1].firstElementChild.value)
                tableTo = tableTo < elementTo ? elementTo : tableTo;
                //isExist = (element.firstElementChild.children[3].value >= (SlabFrom + 1) && SlabTo <= elementTo && !isExist);
            //}
        });

        if ((SlabFrom + 1) <= tableTo) {
            $("#divSuccessError").show();
            $("#divSuccessError").addClass("alert alert-danger");
            $("#lblmsg").text("Pax Slab is already exist.");
            $("#stMsg").text("Error! ");
            setTimeout(() => { $("#divSuccessError").hide(); }, 3000);
            return false;
        }

        for (var iFrom = SlabFrom; iFrom < SlabTo; iFrom = iFrom + SlabGap) {
            //if ($('#tblPaxRange tbody tr').length == 1) {
            //    var PaxSlabFrom = $('#tblPaxRange tbody tr').find('input[id*="__PaxSlabFrom"]').val();
            //    var PaxSlabTo = $('#tblPaxRange tbody tr').find('input[id*="__PaxSlabTo"]').val();
            //    if (!isNaN(PaxSlabFrom) && parseInt(PaxSlabFrom) == 0 && !isNaN(PaxSlabTo) && parseInt(PaxSlabTo) == 0) { }
            //}
            slabsArray.push({ 'SlabFrom': iFrom + 1, 'SlabTo': (iFrom + SlabGap) > SlabTo ? SlabTo : (iFrom + SlabGap) });
            $('#tblPaxRange tbody tr:first td .icon-squre-green').click();
        }
        //CurRowCnt = $('#tblPaxRange tbody tr').length;

        $('#tblPaxRange tbody tr').each(function (iRowCnt) {
            if (iRowCnt >= prevRowCnt) {
                $(this).find('input[id*="__PaxSlabFrom"]').val(slabsArray[RowIndex].SlabFrom);
                $(this).find('input[id*="__PaxSlabTo"]').val(slabsArray[RowIndex].SlabTo);
                $(this).find('input[id*="__PaxSlabDividedBy"]').val(slabsArray[RowIndex].SlabFrom);
                RowIndex++;
            }
        });

        $('#SlabFrom').val("");
        $('#SlabTo').val("");
        $('#SlabGap').val("");
        $("#divSuccessError").hide();
        return false;
    });

    $('#tblPaxRange').on('blur', 'input[id*="__PaxSlabDividedBy"]', function (e) {
        var SlabFrom = $(this).closest('tr').find('input[id*="__PaxSlabFrom"]').val();
        var SlabTo = $(this).closest('tr').find('input[id*="__PaxSlabTo"]').val();
        if (!isNaN(SlabFrom) && !isNaN(SlabTo)) {
            SlabFrom = parseInt(SlabFrom);
            SlabTo = parseInt(SlabTo);
            if (this.value >= SlabFrom && this.value <= SlabTo) return true;
        }
        this.value = '';
    });

    $('#tblPaxRange').on('blur', 'input[id*="__PaxSlabFrom"],input[id*="__PaxSlabTo"]', function (e) {
        var SlabFrom = $(this).closest('tr').find('input[id*="__PaxSlabFrom"]').val();
        if (!isNaN(SlabFrom)) {
            SlabFrom = parseInt(SlabFrom);
            $(this).closest('tr').find('input[id*="__PaxSlabDividedBy"]').val(SlabFrom);
        }
        else {
            $(this).closest('tr').find('input[id*="__PaxSlabDividedBy"]').val('');
        }
    });

    $('#StandardFoc').click(function () {
        PopulateFocRows(this);
    });

    $('#FilterDateRange').change(function () {
        FilterFOC();
    });

    $('#FilterPaxSlab').change(function () {
        FilterFOC();
    });

    PopulateFocRows($('#StandardFoc')[0]);

    $('#frmQuotePaxRange').on('click', '.SaveQuotePaxRange', function () {
         
        if ($('#frmQuotePaxRange').valid()) {
            var model = $('#frmQuotePaxRange').serialize();
            $.ajax({
                type: "POST",
                url: "/Quote/QuotePaxRange",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmQuotePaxRange') > 0) {
                        $("#FOC").click();
                        //$('#frmQuotePaxRange').replaceWith(response);
                        //var form = $("#frmQuotePaxRange");
                        //form.removeData('validator');
                        //form.removeData('unobtrusiveValidation');
                        //$.validator.unobtrusive.parse(form);
                    }
                    else {
                        //var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                        //$('#frmQuotePaxRange .tab-spe-dates-sec').before(successmsg);
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
    
    $('#frmQuoteFOC').on('click', '.SaveQuoteFOC', function () {
         
        var flag = ValidateFOC();
        if (!flag)
            return flag;

        if ($('#frmQuoteFOC').valid()) {
             
            var model = $('#frmQuoteFOC').serialize();
            $.ajax({
                type: "POST",
                url: "/Quote/QuoteFOC",
                data: model,
                dataType: "html",
                success: function (response) {
                    if (response.indexOf('frmQuoteFOC') > 0) {
                        $("#Routing").click();
                        //$('#frmQuoteFOC').replaceWith(response);
                        //var form = $("#frmQuoteFOC");
                        //form.removeData('validator');
                        //form.removeData('unobtrusiveValidation');
                        //$.validator.unobtrusive.parse(form);
                    }
                    else {
                        //var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> Activities not saved!!!</div>';
                        //$('#frmQuoteFOC .tab-spe-dates-sec').before(successmsg);
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
});

function LoadListCounts(PaxSlabCnt, CatCnt, ChainCnt) {
    var rowCount = PaxSlabCnt;
    if (!isNaN(rowCount)) indexPaxRange = parseInt(rowCount);

    rowCount = CatCnt;
    if (!isNaN(rowCount)) indexHotelCat = parseInt(rowCount);

    rowCount = ChainCnt;
    if (!isNaN(rowCount)) indexHotelChain = parseInt(rowCount);

    if (indexHotelCat > 0 || indexHotelChain > 0) {
        if ($('#HotelCategory_0UI').val() == '' || $('#HotelCategory_0UI').val() == '') $('.hotels-opt-cont').hide();
        else $('.hotels-opt-cont').show();
    }
}

function PopulateFocRows(thisparam) {
    try {
        if (thisparam.checked == true) {
            $('.StandardFocRow').show();
            $('.FocRow').hide();
            $('#FilterDateRange').prop('disabled', true);
            $('#FilterPaxSlab').prop('disabled', true);
        }
        else if (thisparam.checked == false) {
            $('.StandardFocRow').hide();
            $('.FocRow').show();
            $('#FilterDateRange').prop('disabled', false);
            $('#FilterPaxSlab').prop('disabled', false);
            FilterFOC();
        }
    } catch (e) {
        console.log(e.message);
    }
}

function FilterFOC() {
     
    var stdFOC = $('#StandardFoc')[0].checked;
    var selDate = $('#FilterDateRange').val();
    var selSlab = $('#FilterPaxSlab').val();

    if (stdFOC == false) {
        if (selDate.toUpperCase() == 'ALL' && selSlab.toUpperCase() == 'ALL') {
            $('#tblFOC .FocRow').show();
        }
        else {
            if (selDate.toUpperCase() == 'ALL' || selSlab.toUpperCase() == 'ALL') {
                $('#tblFOC .FocRow').show();
                $('#tblFOC .FocRow').each(function (index) {
                    var dateRange = $(this).find('input[type="text"][id*="DateRange"]').val();
                    var paxSlab = $(this).find('input[type="text"][id*="PaxSlab"]').val();

                    if (selDate.toUpperCase() == 'ALL') {
                        if (paxSlab.indexOf(selSlab) > -1) $(this).show();
                        else $(this).hide();
                    }
                    else if (selSlab.toUpperCase() == 'ALL') {
                        if (dateRange.indexOf(selDate) > -1) $(this).show();
                        else $(this).hide();
                    }
                });
            }
            else {
                $('#tblFOC .FocRow').each(function (index) {
                    var dateRange = $(this).find('input[type="text"][id*="DateRange"]').val();
                    var paxSlab = $(this).find('input[type="text"][id*="PaxSlab"]').val();
                    if (dateRange.indexOf(selDate) > -1 && paxSlab.indexOf(selSlab) > -1) $(this).show();
                    else $(this).hide();
                });
            }
        }
    }
}

function ValidateFOC()
{
     
    var stdFOC = $('#StandardFoc')[0].checked;
    var flag = true;

    if (stdFOC == false) {
        $('#tblFOC .FocRow').each(function (index) {
            var DivideByCost = $(this).find('input[type="hidden"][id*="DivideByCost"]').val();
            if (DivideByCost <= 0) {
                alert("Please enter valid Departures and Pax slab details.");
                flag = false;
            }
            var FOCSingle = $(this).find('input[type="text"][id*="FOCSingle"]').val();
            var FOCTwin = $(this).find('input[type="text"][id*="FOCTwin"]').val();
            var FOCDouble = $(this).find('input[type="text"][id*="FOCDouble"]').val();
            var FOCTriple = $(this).find('input[type="text"][id*="FOCTriple"]').val();

            var FOCCount = 0;
            FOCCount += parseInt(FOCSingle);
            FOCCount += parseInt(FOCTwin);
            FOCCount += parseInt(FOCDouble);
            FOCCount += parseInt(FOCTriple);

            if (DivideByCost <= FOCCount) {
                flag = false;
            }
        });
    }
    else
    {
        var minDivideByCost = 0;
        $('#tblFOC .FocRow').each(function (index) {
            var DivideByCost = $(this).find('input[type="hidden"][id*="DivideByCost"]').val();
            if (DivideByCost <= 0) {
                alert("Please enter valid Departures and Pax slab details.");
                flag = false;
            }
            if (index == 0) {
                minDivideByCost = DivideByCost;
            }

            if (DivideByCost < minDivideByCost) {
                minDivideByCost = DivideByCost;
            }
        });
        $('#tblFOC .StandardFocRow').each(function (index) {
            var FOCSingle = $(this).find('input[type="text"][id*="FOCSingle"]').val();
            var FOCTwin = $(this).find('input[type="text"][id*="FOCTwin"]').val();
            var FOCDouble = $(this).find('input[type="text"][id*="FOCDouble"]').val();
            var FOCTriple = $(this).find('input[type="text"][id*="FOCTriple"]').val();

            var FOCCount = 0;
            FOCCount += parseInt(FOCSingle);
            FOCCount += parseInt(FOCTwin);
            FOCCount += parseInt(FOCDouble);
            FOCCount += parseInt(FOCTriple);

            if (minDivideByCost <= 0) {
                alert("Please enter valid Departures and Pax slab details.");
                flag = false;
            }
            if (minDivideByCost <= FOCCount) {
                flag = false;
            }
        });
    }
    
    if (flag == false) {
        $("#divErrorMsg").css("display", "block");
    }
    else {
        $("#divErrorMsg").css("display", "none");
    }
    return flag;
}

$(document).on('click', '.close', function (e) {
    var curdiv = $(this).parents("#divSuccessError");
    curdiv.hide();
    curdiv.removeAttr("class");
    curdiv.find("#lblmsg").text("");
    curdiv.find("#stMsg").text("");
});
