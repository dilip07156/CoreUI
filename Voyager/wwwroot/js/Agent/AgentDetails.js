function LoadAgentSection(CompanyId, CompanyName, ContactId) {

    $("#Details").click(function () {
        $('#AgentDetails').load('/Agent/NewAgentDetails', { CompanyId: CompanyId, CompanyName: CompanyName });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Details #aAgentDetails").addClass("active");
        $("#AgentDetails").show();
    });

    $("#Contacts").click(function () {
        $('#AgentContacts').load('/Agent/ContactsAndStaffDetails', { CompanyId: CompanyId, CompanyName: CompanyName, ContactId: $("#hdnContactId").val() });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Contacts #aContacts").addClass("active");
        $("#AgentContacts").show();
    });

    $("#Commercials").click(function () {
        $('#AgentCommercials').load('/Agent/CommercialsAndPaymentDetails', { CompanyId: CompanyId });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Commercials #aCommercials").addClass("active");
        $("#AgentCommercials").show();
    });

    $("#Terms").click(function () {
        $('#AgentTerms').load('/Agent/TermsAndConditionsDetails', { CompanyId: CompanyId });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Terms #aTerms").addClass("active");
        $("#AgentTerms").show();
    });

    $("#SystemOptions").click(function () {
        $('#AgentSystemOptions').load('/Agent/SystemOptionsDetails', { CompanyId: CompanyId });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#SystemOptions #aSystemOptions").addClass("active");
        $("#AgentSystemOptions").show();
    });

    $("#Products").click(function () {
        $('#SupplierProducts').load('/Supplier/Products', { CompanyId: CompanyId });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Products #aProducts").addClass("active");
        $("#SupplierProducts").show();
    });

    $("#Targets").click(function () {
        $('#AgentTargets').load('/Agent/GetCompanyTargets', { CompanyId: CompanyId, ActionType: $("#Targets").attr("actiontype") });
        $("#QuoteTabsCont").children().hide();
        $("#HeadersTab").parent().find("a").removeClass("active");
        $("#Targets #aTargets").addClass("active");
        $("#AgentTargets").show();
        $("#Targets").attr("actiontype", "");
    });
}

function showhidebackbutton() {
    var prevurl = document.referrer;
    var pattern = /AgentDetail/;
    var exists = pattern.test(prevurl);
    if (exists) {
        $("#btnBackToAgentScreen").show();
    } else {
        $("#btnBackToAgentScreen").hide();
    }
}

$('#btnBackToAgentScreen').on('click', function (evt) {
    var prevurl = document.referrer;
    var pattern = /AgentDetail/;
    var exists = pattern.test(prevurl);

    if (exists) {
        evt.preventDefault();
        history.back();
    } else {
        return false;
    }
});


