$(document).ready(function () {

});

$(document).on("change", ".frm1ddlRole", function () {
	 
	var hdnSeqId = $(this).closest('tr').find('#hdnSeqNo').val();
	var selectedRoleName = $(this).find("option:selected").text();
	$.ajax({
		type: "POST",
		url: "/UserAndRoleMgmt/GetUsersByRole",
		data: { selectedRoleName: selectedRoleName },
		success: function (result) {
			 
			$("#ddlUserID_" + hdnSeqId).empty();
			$("#ddlUserID_" + hdnSeqId).append($("<option></option>").val(null).html("Select"));
			if (result.usersList.length > 0) {
				$.each(result.usersList, function (key, value) {
					$("#ddlUserID_" + hdnSeqId).append($("<option></option>").val
						(value.attributeValue_Id).html(value.value));
				});
			}
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
});

$('#frmtabDestination').on('click', '.frm1Add', function () {
	 

	if ($('#frmtabDestination').valid()) {
		addNewRow1(this);
	}
	//else {
	//	alert('Please fill the mandatory fields!!!');
	//}
	return false;
});

$('#frmtabDestination').on('click', '.frm1Delete', function () {
	 
	if (confirm("Are you sure you want to delete this row?")) {
		var index = $(this).closest('tr').find('#hdnSeqNo').val();
		var tid = $('#hfTypeId_' + index).val();
		var rid = $('#hfRoleId_' + index).val();
		var uid = $('#hfUserId_' + index).val();

		if (tid != undefined && tid != null && tid != '' && tid.length > 30 && tid.substring(0, 8) != '00000000' &&
			rid != undefined && rid != null && rid != '' && rid.length > 30 && rid.substring(0, 8) != '00000000' &&
			uid != undefined && uid != null && uid != '' && uid.length > 30 && uid.substring(0, 8) != '00000000') {
			$(this).closest('tr').hide();
			$('#hfIsDeleted_' + index).val('X');
		}
		//else {
		//	removeNRow(this, index);
		//}
	}
	return false;
});

$('#frm1Save').click(function () {
	 
	var flag = true;

	$('#tblDestination > tbody > tr').each(function () {
		 

		var hdn = $(this).find('#hdnSeqNo').val();

		if ($("#ddlDestinationID_" + hdn).val() == "" && $("#ddlRoleID_" + hdn).val() == "" && $("#ddlUserID_" + hdn).val() == "") {
			flag = true;
		}
		else {
			if ($("#ddlDestinationID_" + hdn).val() == "") {
				$("#ddlDestinationID_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlDestinationID_" + hdn).siblings("span").text("");
			}
			if ($("#ddlRoleID_" + hdn).val() == "") {
				$("#ddlRoleID_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlRoleID_" + hdn).siblings("span").text("");
			}
			if ($("#ddlUserID_" + hdn).val() == "") {
				$("#ddlUserID_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlUserID_" + hdn).siblings("span").text("");
			}
		}
	});

	if (flag) {
		$(".ajaxloader").show();
		var model = $('#frmtabDestination').serialize();
		$.ajax({
			type: "POST",
			url: "/UserAndRoleMgmt/SaveRoleByDestination",
			data: model,
			global: false,
			success: function (response) {
				 
				if (response.status == "Failure") {
					//var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.responseText + '</div>';
					$("#frmtabDestination .success1").hide();
					$("#frmtabDestination #errmsg1").text(response.responseText);
					$("#frmtabDestination .error1").show();
					$(".ajaxloader").hide();
				}
				else {
					ViewSalesPipelineRoles(response.responseText, "destination");
					//var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! ' + response.responseText + ' </strong></div>';
					//$("#frmtabDestination #message").html(successmsg);
				}
			},
			error: function (response) {
				var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.responseText + '</div>';
				$("#frmtabDestination #message").before(successmsg);
				$(".ajaxloader").hide();
			}
		});
	}
});

$(document).on("change", ".frm2ddlRole", function () {
	 
	var hdnSeqId = $(this).closest('tr').find('#hdnSeqNo1').val();
	var selectedRoleName = $(this).find("option:selected").text();
	$.ajax({
		type: "POST",
		url: "/UserAndRoleMgmt/GetUsersByRole",
		data: { selectedRoleName: selectedRoleName },
		success: function (result) {
			 
			$("#ddlUserID1_" + hdnSeqId).empty();
			$("#ddlUserID1_" + hdnSeqId).append($("<option></option>").val(null).html("Select"));
			if (result.usersList.length > 0) {
				$.each(result.usersList, function (key, value) {
					$("#ddlUserID1_" + hdnSeqId).append($("<option></option>").val
						(value.attributeValue_Id).html(value.value));
				});
			}
		},
		failure: function (response) {
			alert(response.responseText);
		},
		error: function (response) {
			alert(response.responseText);
		}
	});
});

$('#frmtabSalesOffice').on('click', '.frm2Add', function () {
	 

	if ($('#frmtabSalesOffice').valid()) {
		addNewRow2(this);
	}
	//else {
	//	alert('Please fill the mandatory fields!!!');
	//}
	return false;
});

$('#frmtabSalesOffice').on('click', '.frm2Delete', function () {
	 
	if (confirm("Are you sure you want to delete this row?")) {
		var index = $(this).closest('tr').find('#hdnSeqNo1').val();
		var tid = $('#hfTypeId1_' + index).val();
		var rid = $('#hfRoleId1_' + index).val();
		var uid = $('#hfUserId1_' + index).val();

		if (tid != undefined && tid != null && tid != '' && tid.length > 30 && tid.substring(0, 8) != '00000000' &&
			rid != undefined && rid != null && rid != '' && rid.length > 30 && rid.substring(0, 8) != '00000000' &&
			uid != undefined && uid != null && uid != '' && uid.length > 30 && uid.substring(0, 8) != '00000000') {
			$(this).closest('tr').hide();
			$('#hfIsDeleted1_' + index).val('X');
		}
		//else {
		//	removeNRow(this, index);
		//}
	}
	return false;
});

$('#frm2Save').click(function () {
	 
	var flag = true;

	$('#tblSalesOffice > tbody > tr').each(function () {
		 

		var hdn = $(this).find('#hdnSeqNo1').val();

		if ($("#ddlSalesOfficeID_" + hdn).val() == "" && $("#ddlRoleID1_" + hdn).val() == "" && $("#ddlUserID1_" + hdn).val() == "") {
			flag = true;
		}
		else {

			if ($("#ddlSalesOfficeID_" + hdn).val() == "") {
				$("#ddlSalesOfficeID_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlSalesOfficeID_" + hdn).siblings("span").text("");
			}
			if ($("#ddlRoleID1_" + hdn).val() == "") {
				$("#ddlRoleID1_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlRoleID1_" + hdn).siblings("span").text("");
			}
			if ($("#ddlUserID1_" + hdn).val() == "") {
				$("#ddlUserID1_" + hdn).siblings("span").text("*");
				flag = false;
			}
			else {
				$("#ddlUserID1_" + hdn).siblings("span").text("");
			}
		}
	});


	if (flag) {
		$(".ajaxloader").show();
		var model = $('#frmtabSalesOffice').serialize();
		$.ajax({
			type: "POST",
			url: "/UserAndRoleMgmt/SaveRoleBySalesOffice",
			data: model,
			global: false,
			success: function (response) {
				 

				if (response.status == "Failure") {
					//var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.responseText + '</div>';
					//$("#frmtabSalesOffice #message1").before(successmsg);
					$("#frmtabSalesOffice .success2").hide();
					$("#frmtabSalesOffice #errmsg2").text(response.responseText);
					$("#frmtabSalesOffice .error2").show();
					$(".ajaxloader").hide();
				}
				else {
					ViewSalesPipelineRoles(response.responseText, "sales");
					//var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Success! ' + response.responseText + ' </strong></div>';
					//$("#frmtabSalesOffice #message").html(successmsg);
				}
			},
			error: function (response) {
				var successmsg = '<div class="alert alert-danger"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong>Error!</strong> ' + response.responseText + '</div>';
				$("#frmtabSalesOffice #message").before(successmsg);
				$(".ajaxloader").hide();
			}
		});
	}
	//else {
	//	alert('Please fill mandatory fields.');
	//}
});

function addNewRow1(thisparam) {
	 

	var currentRow = $(thisparam).closest('tr');
	var flag = true;

	$('#tblDestination > tbody > tr').each(function () {
		 
		var hdn = $(this).find('#hdnSeqNo').val();

		if ($("#ddlDestinationID_" + hdn).val() == "") {
			$("#ddlDestinationID_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlDestinationID_" + hdn).siblings("span").text("");
		}
		if ($("#ddlRoleID_" + hdn).val() == "") {
			$("#ddlRoleID_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlRoleID_" + hdn).siblings("span").text("");
		}
		if ($("#ddlUserID_" + hdn).val() == "") {
			$("#ddlUserID_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlUserID_" + hdn).siblings("span").text("");
		}

		//var dName = currentRow.find(".frm1ddlDestination").val();
		//if (dName == '') {
		//	currentRow.find(".frm1ddlDestination").siblings("span").text("*");
		//	IsValidFlag = false;
		//}
		//else {
		//	currentRow.find(".frm1ddlDestination").siblings("span").text("");
		//}
		//var roleName = currentRow.find(".frm1ddlRole").val();
		//if (roleName == '') {
		//	currentRow.find(".frm1ddlRole").siblings("span").text("*");
		//	IsValidFlag = false;
		//}
		//else {
		//	currentRow.find(".frm1ddlRole").siblings("span").text("");
		//}
		//var userName = currentRow.find(".frm1ddlUsers").val();
		//if (userName == '') {
		//	currentRow.find(".frm1ddlUsers").siblings("span").text("*");
		//	IsValidFlag = false;
		//}
		//else {
		//	currentRow.find(".frm1ddlUsers").siblings("span").text("");
		//}
	});

	if (!flag) {
		return false;
	}

	var index = $(thisparam).closest('tr').find('#hdnSeqNo').val();     //Get Row Index from hidden field
	var clonedRow = $(thisparam).closest('tr').clone();                 //Clone row to append

	//Append cloned row after current row
	$(thisparam).closest('tr').after(clonedRow);

	$('#tblDestination > tbody > tr').each(function (iRowCnt) {
		 
		if (iRowCnt > index) {
			var thisRow = this;
			var curRow = $(this).find('#hdnSeqNo').val();
			this.id = this.id.replace(curRow, iRowCnt);
			$(this).find('input,select,span,textarea,td,ul,div').each(function (iColCnt) {
				 
				if (this.id != undefined) {
					this.id = this.id.replace(curRow, iRowCnt);
					if (this.id == 'hdnSeqNo') this.value = iRowCnt;
				}
				if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
				if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
			});
		}
	});

	//Rebind form validation to enable new rows validation
	var form = $("#frmtabDestination");
	form.removeData('validator');
	form.removeData('unobtrusiveValidation');
	$.validator.unobtrusive.parse(form);
}

function addNewRow2(thisparam) {
	 

	var currentRow = $(thisparam).closest('tr');
	var flag = true;

	$('#tblSalesOffice > tbody > tr').each(function () {

		var hdn = $(this).find('#hdnSeqNo1').val();

		if ($("#ddlSalesOfficeID_" + hdn).val() == "") {
			$("#ddlSalesOfficeID_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlSalesOfficeID_" + hdn).siblings("span").text("");
		}
		if ($("#ddlRoleID1_" + hdn).val() == "") {
			$("#ddlRoleID1_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlRoleID1_" + hdn).siblings("span").text("");
		}
		if ($("#ddlUserID1_" + hdn).val() == "") {
			$("#ddlUserID1_" + hdn).siblings("span").text("*");
			flag = false;
		}
		else {
			$("#ddlUserID1_" + hdn).siblings("span").text("");
		}
	});

	if (!flag) {
		return false;
	}

	//var currentRow = $(thisparam).closest('tr');
	//var IsValidFlag = true;
	//var dName = currentRow.find(".frm2ddlSalesOffice").val();
	//if (dName == '') {
	//	currentRow.find(".frm2ddlSalesOffice").siblings("span").text("*");
	//	IsValidFlag = false;
	//}
	//var roleName = currentRow.find(".frm2ddlRole").val();
	//if (roleName == '') {
	//	currentRow.find(".frm2ddlRole").siblings("span").text("*");
	//	IsValidFlag = false;
	//}
	//var userName = currentRow.find(".frm2ddlUsers").val();
	//if (userName == '') {
	//	currentRow.find(".frm2ddlUsers").siblings("span").text("*");
	//	IsValidFlag = false;
	//}

	//if (!IsValidFlag) {
	//	return false;
	//}

	var index = $(thisparam).closest('tr').find('#hdnSeqNo1').val();     //Get Row Index from hidden field
	var clonedRow = $(thisparam).closest('tr').clone();                 //Clone row to append

	//Append cloned row after current row
	$(thisparam).closest('tr').after(clonedRow);

	$('#tblSalesOffice > tbody > tr').each(function (iRowCnt) {
		 
		if (iRowCnt > index) {
			var thisRow = this;
			var curRow = $(this).find('#hdnSeqNo1').val();
			this.id = this.id.replace(curRow, iRowCnt);
			$(this).find('input,select,span,textarea,td,ul,div').each(function (iColCnt) {
				 
				if (this.id != undefined) {
					this.id = this.id.replace(curRow, iRowCnt);
					if (this.id == 'hdnSeqNo1') this.value = iRowCnt;
				}
				if (this.name != undefined) this.name = this.name.replace(curRow, iRowCnt);
				if (this.attributes['data-valmsg-for'] != undefined) this.attributes['data-valmsg-for'].nodeValue = this.attributes['data-valmsg-for'].nodeValue.replace(curRow, iRowCnt);
			});
		}
	});

	//Rebind form validation to enable new rows validation
	var form = $("#frmtabSalesOffice");
	form.removeData('validator');
	form.removeData('unobtrusiveValidation');
	$.validator.unobtrusive.parse(form);
}
