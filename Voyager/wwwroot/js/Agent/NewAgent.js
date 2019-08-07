$(document).ready(function () {

	$("#ddlCountry").on("change", function () {
		var countryId = $(this).find("option:selected").val();
		var countryName = $(this).find("option:selected").text();
		$("#ddlCountryName").val(countryName);
		$.ajax({
			type: "POST",
			url: "/Agent/PopulateCitiesByCountryId",
			data: { countryId: countryId },
			success: function (result) {
				$("#ddlCity").empty();
				$("#ddlCity").append($("<option></option>").val(null).html("Select"));
				if (result.response.length > 0) {
					$.each(result.response, function (key, value) {
						$("#ddlCity").append($("<option></option>").val
							(value.attribute_Id).html(value.attributeName));
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

	$("#ddlCity").on("change", function () {
		var cityId = $(this).find("option:selected").val();
		var cityName = $(this).find("option:selected").text();
		if (cityId == null)
			$("#ddlCityName").val("");
		else
			$("#ddlCityName").val(cityName);
	});

	$(".btnAdd").click(function () {

		var pattern = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

		var IsValidFlag = true;

		var Title = $("#CTitle").val();
		if (Title == '') {
			$("#spanCommonTitle").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanCommonTitle").text("");

		var AgentName = $("#AgentName").val();
		if (AgentName == '') {
			$("#spanAgentName").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanAgentName").text("");

		var Street = $("#Street").val();
		if (Street == '') {
			$("#spanStreet").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanStreet").text("");

		var Zipcode = $("#Zipcode").val();
		if (Zipcode == '') {
			$("#spanZipCode").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanZipCode").text("");

		var Country = $("#ddlCountry").find("option:selected").val();
		if (Country == '') {
			$("#spanCountry").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanCountry").text("");

		var City = $("#ddlCity").find("option:selected").val();
		if (City == '') {
			$("#spanCity").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanCity").text("");

		var FIRSTNAME = $("#FIRSTNAME").val();
		if (FIRSTNAME == '') {
			$("#spanFirstName").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanFirstName").text("");

		var LastNAME = $("#LastNAME").val();
		if (LastNAME == '') {
			$("#spanLastName").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanLastName").text("");

		var TEL = $("#TEL").val();
		if (TEL == "") {
			$("#spanTel").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanTel").text("");

		var MAIL = $("#MAIL").val();
		var span = $("#spanMAIL").text();
		if (!pattern.test(MAIL)) {
			$("#spanMAIL").text("Enter valid Email Address");
			IsValidFlag = false;
		}
		else if (MAIL == '') {
			$("#spanMAIL").text("*");
			IsValidFlag = false;
		}
		else
			$("#spanMAIL").text("");

		if (!IsValidFlag)
			return false;

		$(".ajaxloader").show();
		var model = $("#frmNewAgent").serialize();
		$.ajax({
			type: "POST",
			url: "/Agent/SaveNewAgent",
			data: model,
			global: false,
			success: function (response) {
				 
				$("#frmNewAgent #Company_Id").val(response.companyId);
				$("#frmNewAgent #Contact_Id").val(response.contactId);
				$("#frmNewAgent #Company_Code").val(response.companyCode);
				if (response.status == "error") {
					var successmsg = '<div class="alert alert-danger" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong> Error! ' + response.responseText + '</strong></div>';
					$("#frmNewAgent #message").html(successmsg);
				}
				else {
					var successmsg = '<div class="alert alert-success" id="errmsg"><a href="#" class="close" data-dismiss="alert" aria-label="close" title="close">×</a><strong> Success! ' + response.responseText + '</strong></div>';
					$("#frmNewAgent #message").html(successmsg);
					if (response.type == "Agent") {
						window.location.href = "/Agent/AgentDetail?CompanyId=" + response.companyId;
					}
				}
				$(".ajaxloader").hide();
			},
			error: function () {
				$(".ajaxloader").hide();
			}
		});
	});
});