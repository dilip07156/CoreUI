using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using VGER_WAPI_CLASSES.BRIDGE.Company;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class AgentProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        public AgentProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region Agent Details

        public async Task<AgentGetRes> GetAgentDetails(AgentGetReq agentGetReq, string ticket)
        {
            AgentGetRes agentGetRes = new AgentGetRes();
            agentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetAgentDetails"), agentGetReq, typeof(AgentGetRes), ticket);
            return agentGetRes;
        }

        public async Task<AgentGetRes> GetAgentDetailedInfo(AgentGetReq agentGetReq, string ticket)
        {
            AgentGetRes agentGetRes = new AgentGetRes();
            agentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetAgentDetailedInfo"), agentGetReq, typeof(AgentGetRes), ticket);
            return agentGetRes;
        }

        public async Task<AgentThirdPartyGetRes> GetPartnerAgentDetailedInfo(AgentThirdPartyGetReq agentGetReq, string ticket)
        {
            AgentThirdPartyGetRes agentGetRes = new AgentThirdPartyGetRes();
            agentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetPartnerAgentDetailedInfo"), agentGetReq, typeof(AgentThirdPartyGetRes), ticket);
            return agentGetRes;
        }

        public async Task<AgentThirdPartyGetRes> GetPartnerAgentContactDetails(AgentThirdPartyGetReq agentGetReq, string ticket)
        {
            AgentThirdPartyGetRes agentGetRes = new AgentThirdPartyGetRes();
            agentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetPartnerAgentContactDetails"), agentGetReq, typeof(AgentThirdPartyGetRes), ticket);
            return agentGetRes;
        }

        public async Task<List<mDefStartPage>> GetStartPageForAgents(string ticket)
        {
            List<mDefStartPage> list = new List<mDefStartPage>();
            list = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetStartPageForAgents"), null, typeof(List<mDefStartPage>), ticket);
            return list;
        }

		public async Task<List<mStatus>> GetStatusForAgents(string ticket)
		{
			List<mStatus> list = new List<mStatus>();
			list = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetStatusForAgents"), null, typeof(List<mStatus>), ticket);
			return list;
		}

		public async Task<List<Attributes>> GetDefDocumentTypes(string ticket)
		{
			List<Attributes> list = new List<Attributes>();
			list = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetDefDocumentTypes"), null, typeof(List<Attributes>), ticket);
			return list;
		}

		public async Task<List<Attributes>> GetProductTypes(string ticket)
		{
			List<Attributes> list = new List<Attributes>();
			list = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetProductTypes"), null, typeof(List<Attributes>), ticket);
			return list;
		}

		public async Task<AgentSetRes> SetAgentDetailedInfo(AgentSetReq agentSetReq, string ticket)
        {
            AgentSetRes agentSetRes = new AgentSetRes();
            agentSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:SetAgentDetailedInfo"), agentSetReq, typeof(AgentSetRes), ticket);
            return agentSetRes;
        }

        public async Task<AgentGetRes> GetUserDetailsByContactId(AgentGetReq agentGetReq, string ticket)
        {
            AgentGetRes agentGetRes = new AgentGetRes();
            agentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetUserDetailsByContactId"), agentGetReq, typeof(AgentGetRes), ticket);
            return agentGetRes;
        }

        public async Task<CompanyOfficerGetRes> GetCompanyOfficers(CompanyOfficerGetReq companyOfficerGetReq, string ticket)
        {
            CompanyOfficerGetRes companyOfficerGetRes = new CompanyOfficerGetRes();
            companyOfficerGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetCompanyOfficers"), companyOfficerGetReq, typeof(CompanyOfficerGetRes), ticket);
            return companyOfficerGetRes;
        }

        public async Task<CompanyOfficerGetRes> GetCompanyContacts(CompanyOfficerGetReq companyOfficerGetReq, string ticket)
        {
            CompanyOfficerGetRes companyOfficerGetRes = new CompanyOfficerGetRes();
            companyOfficerGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetCompanyContacts"), companyOfficerGetReq, typeof(CompanyOfficerGetRes), ticket);
            return companyOfficerGetRes;
        }

		public async Task<CompanyOfficerGetRes> GetSalesOfficesOfSystemCompany(string ticket)
		{
			CompanyOfficerGetRes companyOfficerGetRes = new CompanyOfficerGetRes();
			companyOfficerGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetSalesOfficesOfSystemCompany"), null, typeof(CompanyOfficerGetRes), ticket);
			return companyOfficerGetRes;
		}

        public async Task<CompanyOfficerGetRes> GetSalesOfficesByCompanyId(string CompanyId, string ticket)
        {
            CompanyOfficerGetRes companyOfficerGetRes = new CompanyOfficerGetRes();
            companyOfficerGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetSalesOfficesByCompanyId"), CompanyId, typeof(CompanyOfficerGetRes), ticket);
            return companyOfficerGetRes;
        }

        public async Task<Response> GetSystemCompanyDetails(string UserContactId ,string ticket)
        {
            Response response = new Response();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetSystemCompany"), UserContactId, typeof(Response), ticket);
            return response;
        }

        public async Task<TargetGetRes> GetCompanyTargets(AgentGetReq agentGetReq, string ticket)
        {
            TargetGetRes response = new TargetGetRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetCompanyTargets"), agentGetReq, typeof(TargetGetRes), ticket);
            return response;
        }
        #endregion

        #region Bridge Services
        public async Task<GetCompany_RS> GetLatestSQLReferenceNumber(GetCompany_RQ request, string ticket)
        {
            GetCompany_RS response = new GetCompany_RS();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:GetLatestSQLReferenceNumber"), request, typeof(GetCompany_RS), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompany(SetCompany_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompany"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyKeyInfo(SetCompanyKeyInfo_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyKeyInfo"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyBranches(SetCompanyKeyInfo_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyBranches"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyContact(SetCompanyContact_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyContact"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanySharedContact(SetCompanyContact_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanySharedContact"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyAccountsAndMarkup(SetCompanyKeyInfo_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyAccountsAndMarkup"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyPaymentTerms(SetCompanyPaymentTerms_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyPaymentTerms"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }
        public async Task<ResponseStatus> SetCompanyTaxRegDetails(SetCompanyTaxReg_RQ request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyTaxRegDetails"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetTermsAndCondition(TermsAndConditionsSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetTermsAndCondition"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetCompanyPayment(CompanyPaymentSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetCompanyPayment"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetEmergencyContact(EmergencyContactSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetEmergencyContact"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> DelEmergencyContact(EmergencyContactSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:DelEmergencyContact"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetUserRoles(UserRolesSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetUserRoles"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }

        public async Task<ResponseStatus> SetUserPassword(UserRolesSetReq request, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("BRIDGEServiceCompany:SetUserPassword"), request, typeof(ResponseStatus), ticket, "Bridge");
            return response;
        }
        #endregion

        public async Task<SettingsAutomatedGetRes> GetAutomatedSalesPipelineRoles(SettingsAutomatedGetReq request, string ticket)
        {
            SettingsAutomatedGetRes objSettingsAutomatedGetRes = new SettingsAutomatedGetRes();
            objSettingsAutomatedGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentService:GetAutomatedSalesPipelineRoles"), request, typeof(SettingsAutomatedGetRes), ticket);
            return objSettingsAutomatedGetRes;
        }
    }
}
