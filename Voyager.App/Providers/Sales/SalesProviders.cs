using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class SalesProviders
    {
        private readonly IConfiguration _configuration;
        private ServiceProxy serviceProxy;

        #region Initializers

        public SalesProviders()
        {
        }

        public SalesProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Master Data

        public async Task<MasterTypeResponse> GetGenericMasterForType(MasterTypeRequest objMasterTypeRequest, string ticket)
        {
            MasterTypeResponse objMasterTypeResponse = new MasterTypeResponse();
            objMasterTypeResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetGenericMasterForType"), objMasterTypeRequest, typeof(MasterTypeResponse), ticket);
            return objMasterTypeResponse;
        }

		public async Task<QuoteAgentGetRes> GetDivisionList(QuoteSearchReq request, string ticket)
		{
			QuoteAgentGetRes response = new QuoteAgentGetRes();
			response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetDivisionList"), request, typeof(QuoteAgentGetRes), ticket);
			return response;
		}

		#endregion

		#region Quote Search
		public async Task<QuoteSearchRes> GetQRFEnquiryPipelineList(QuoteSearchReq objQuoteSearchReq, string ticket)
        {
            QuoteSearchRes objQuoteSearchRes = new QuoteSearchRes();
            objQuoteSearchRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:QRFAgentDetailsBySearchCriteria"), objQuoteSearchReq, typeof(QuoteSearchRes), ticket);
            return objQuoteSearchRes;
        }
        #endregion

        #region Agent Info
        public async Task<AgentCompanyRes> GetAgentList(AgentCompanyReq objAgentRequest, string ticket)
        {
            AgentCompanyRes objAgentResponse = new AgentCompanyRes();
            objAgentResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetAgentList"), objAgentRequest, typeof(AgentCompanyRes), ticket);
            return objAgentResponse;
        }

        public async Task<AgentCompanyRes> GetAgentListFrommCompanies(AgentCompanyReq objAgentRequest, string ticket)
        {
            AgentCompanyRes objAgentResponse = new AgentCompanyRes();
            objAgentResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetAgentListFrommCompanies"), objAgentRequest, typeof(AgentCompanyRes), ticket);
            return objAgentResponse;
        }

        public async Task<AgentCompanyRes> GetSupplierListFrommCompanies(AgentCompanyReq objAgentRequest, string ticket)
        {
            AgentCompanyRes objAgentResponse = new AgentCompanyRes();
            objAgentResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetSupplierListfrommCompanies"), objAgentRequest, typeof(AgentCompanyRes), ticket);
            return objAgentResponse;
        }

        public async Task<CompanyDetailsRes> GetCompanyDetails(string ticket)
        {
            CompanyDetailsRes objAgentResponse = new CompanyDetailsRes();
            objAgentResponse = await serviceProxy.GetData(_configuration.GetValue<string>("ServiceAgent:GetCompanyDetails"), typeof(CompanyDetailsRes), ticket);
            return objAgentResponse;
        }

        public async Task<AgentContactRes> GetContactListForAgent(AgentContactReq objAgentContactRequest, string ticket)
        {
            AgentContactRes objAgentContactResponse = new AgentContactRes();
            objAgentContactResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetContactListForAgent"), objAgentContactRequest, typeof(AgentContactRes), ticket);
            return objAgentContactResponse;
        }

        public async Task<AgentContactRes> CheckDuplicateQRFTourName(AgentContactReq objAgentContactRequest, string ticket)
        {
            AgentContactRes objAgentContactResponse = new AgentContactRes();
            objAgentContactResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:CheckDuplicateQRFTourName"), objAgentContactRequest, typeof(AgentContactRes), ticket);
            return objAgentContactResponse;
        }

        public async Task<AgentContactDetailsRes> GetContactDetailsByContactID(AgentContactDetailsReq objAgentContactDetailsReq, string ticket)
        {
            AgentContactDetailsRes objAgentContactDetailsRes = new AgentContactDetailsRes();
            objAgentContactDetailsRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetContactDetailsByContactID"), objAgentContactDetailsReq, typeof(AgentContactDetailsRes), ticket);
            return objAgentContactDetailsRes;
        }

        public async Task<CurrencyResponse> GetCurrencyList(string ticket)
        {
            CurrencyResponse objCurrencyResponse = new CurrencyResponse();
            objCurrencyResponse = await serviceProxy.GetData(_configuration.GetValue<string>("ServiceMaster:GetCurrencyList"), typeof(CurrencyResponse), ticket);
            return objCurrencyResponse;
        }

        public async Task<QuoteAgentSetRes> SetQRFAgentDetails(QUOTEAgentSetReq objQRFAgentSetReq, string ticket)
        {
            QuoteAgentSetRes objQuoteAgentSetRes = new QuoteAgentSetRes();
            objQuoteAgentSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:SetQRFAgentDetails"), objQRFAgentSetReq, typeof(QuoteAgentSetRes), ticket);
            return objQuoteAgentSetRes;
        }

        public async Task<QuoteAgentGetRes> GetQRFAgentByQRFID(QuoteAgentGetReq objQuoteAgentGetReq, string ticket)
        {
            QuoteAgentGetRes objQuoteAgentGetRes = new QuoteAgentGetRes();
            objQuoteAgentGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetQRFAgentByQRFID"), objQuoteAgentGetReq, typeof(QuoteAgentGetRes), ticket);
            return objQuoteAgentGetRes;
        }

        public async Task<ProdNationalityGetRes> GetNationalityList(string CompanyId, string ticket)
        {
            ProdNationalityGetRes objNationalityRes = new ProdNationalityGetRes();
            objNationalityRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetNationalityList"), CompanyId, typeof(ProdNationalityGetRes), ticket);
            return objNationalityRes;
        }

        #endregion      

        #region Departure Dates
        public async Task<DepartureDateGetResponse> GetDepartureDatesForQRFId(DepartureDateGetRequest objDepartureDateRequest, string ticket)
        {
            DepartureDateGetResponse objDepartureDateResponse = new DepartureDateGetResponse();
            objDepartureDateResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceDeparture:GetDepartureDatesForQRFId"), objDepartureDateRequest, typeof(DepartureDateGetResponse), ticket);
            return objDepartureDateResponse;
        }

        public async Task<DepartureDateSetResponse> SetQRFDepartureDetails(DepartureDateSetRequest objQRFDepartureSetReq, string ticket)
        {
            DepartureDateSetResponse objQuoteDepartureSetRes = new DepartureDateSetResponse();
            objQuoteDepartureSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceDeparture:SetDepartureDatesForQRFId"), objQRFDepartureSetReq, typeof(DepartureDateSetResponse), ticket);
            return objQuoteDepartureSetRes;
        }

        #endregion

        #region Pax Range
        public async Task<CoachesGetResponse> GetCoachTypesForTransport(string ticket)
        {
            CoachesGetResponse objCoachTypesResponse = new CoachesGetResponse();
            objCoachTypesResponse = await serviceProxy.GetData(_configuration.GetValue<string>("ServiceMaster:GetCoachTypesForTransport"), typeof(CoachesGetResponse), ticket);
            return objCoachTypesResponse;
        }

        public async Task<PaxGetResponse> GetQRFPaxRangeDetails(PaxGetRequest objQRFPaxGetReq, string ticket)
        {
            PaxGetResponse objPaxSetRes = new PaxGetResponse();
            objPaxSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePaxSlab:GetQRFPaxRangeDetails"), objQRFPaxGetReq, typeof(PaxGetResponse), ticket);
            return objPaxSetRes;
        }

        public async Task<PaxSetResponse> SetQRFPaxRangeDetails(PaxSetRequest objQRFPaxSetReq, string ticket)
        {
            PaxSetResponse objPaxSetRes = new PaxSetResponse();
            objPaxSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServicePaxSlab:SetQRFPaxRangeDetails"), objQRFPaxSetReq, typeof(PaxSetResponse), ticket);
            return objPaxSetRes;
        }

        #endregion        

        #region QRF Routing

        public async Task<RoutingSetRes> SetQRFRoutingDetails(RoutingSetReq routingSetReq, string ticket)
        {
            RoutingSetRes routingSetRes = new RoutingSetRes();
            routingSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceRoute:SetQRFRouteDetails"), routingSetReq, typeof(RoutingSetRes), ticket);
            return routingSetRes;
        }

        public async Task<RoutingGetRes> getQRFRoutingDetails(RoutingGetReq routingGetReq, string ticket)
        {
            RoutingGetRes routingGetRes = new RoutingGetRes();
            routingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceRoute:GetQRFRouteDetails"), routingGetReq, typeof(RoutingGetRes), ticket);
            return routingGetRes;
        }

        public async Task<CityLookupResponse> GetCityList(CityLookupRequest objCityLookupRequest, string ticket)
        {
            CityLookupResponse objCityLookupResponse = new CityLookupResponse();
            objCityLookupResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMaster:GetCityList"), objCityLookupRequest, typeof(CityLookupResponse), ticket);
            return objCityLookupResponse;
        }
        #endregion

        #region QRF Routing Days
        public async Task<RoutingDaysGetRes> GetQRFRoutingDays(RoutingDaysGetReq routingGetReq, string ticket)
        {
            RoutingDaysGetRes routingDaysGetRes = new RoutingDaysGetRes();
            routingDaysGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceRoute:GetQRFRoutingDays"), routingGetReq, typeof(RoutingDaysGetRes), ticket);
            return routingDaysGetRes;
        }
        #endregion

        #region QRF Margin
        public async Task<MarginSetRes> SetQRFMarginDetails(MarginSetReq objMarginSetReq, string ticket)
        {
            MarginSetRes objMarginSetRes = new MarginSetRes();
            objMarginSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMargin:SetQRFMarginDetails"), objMarginSetReq, typeof(MarginSetRes), ticket);
            return objMarginSetRes;
        }

        public async Task<MarginGetRes> GetQRFMarginDetailsByQRFID(MarginGetReq objMarginGetReq, string ticket)
        {
            MarginGetRes objMarginGetRes = new MarginGetRes();
            objMarginGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceMargin:GetQRFMarginDetailsByQRFID"), objMarginGetReq, typeof(MarginGetRes), ticket);
            return objMarginGetRes;
        }

        public async Task<CurrencyGetRes> GetActiveCurrencyList(CurrencyGetReq objCurrencyGetReq, string ticket)
        {
            CurrencyGetRes objCurrencyGetRes = new CurrencyGetRes();
            objCurrencyGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceDeparture:GetActiveCurrencyList"), objCurrencyGetReq, typeof(CurrencyGetRes), ticket);
            return objCurrencyGetRes;
        }
        #endregion

        #region QRF FOC
        public async Task<FOCGetResponse> GetQRFFOCDetails(PaxGetRequest objQRFFOCGetReq, string ticket)
        {
            FOCGetResponse objFOCSetRes = new FOCGetResponse();
            objFOCSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFOC:GetQRFFOCDetails"), objQRFFOCGetReq, typeof(FOCGetResponse), ticket);
            return objFOCSetRes;
        }

        public async Task<PaxSetResponse> SetQRFFOCDetails(FOCSetRequest objQRFFOCSetReq, string ticket)
        {
            PaxSetResponse objPaxSetRes = new PaxSetResponse();
            objPaxSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFOC:SetQRFFOCDetails"), objQRFFOCSetReq, typeof(PaxSetResponse), ticket);
            return objPaxSetRes;
        }
        #endregion 

        public async Task<LinkedQRFsGetRes> GetLinkedQRFs(LinkedQRFsGetReq linkedQRFsGetReq, string ticket)
        {
            LinkedQRFsGetRes linkedQRFsGetRes = new LinkedQRFsGetRes();
            linkedQRFsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAgent:GetLinkedQRFs"), linkedQRFsGetReq, typeof(LinkedQRFsGetRes), ticket);
            return linkedQRFsGetRes;
        }

        #region Follow Up
        public async Task<FollowUpMasterGetRes> GetFollowUpMasterData(FollowUpGetReq followUpGetReq, string ticket)
        {
            FollowUpMasterGetRes followUpMasterGetRes = new FollowUpMasterGetRes();
            followUpMasterGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFollowUp:GetFollowUpMasterData"), followUpGetReq, typeof(FollowUpMasterGetRes), ticket);
            return followUpMasterGetRes;
        }
        public async Task<FollowUpGetRes> GetFollowUpForQRF(FollowUpGetReq followUpGetReq, string ticket)
        {
            FollowUpGetRes followUpGetRes = new FollowUpGetRes();
            followUpGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFollowUp:GetFollowUpForQRF"), followUpGetReq, typeof(FollowUpGetRes), ticket);
            return followUpGetRes;
        }

        public async Task<FollowUpSetRes> SetFollowUpForQRF(FollowUpSetReq followUpSetReq, string ticket)
        {
            FollowUpSetRes followUpSetRes = new FollowUpSetRes();
            followUpSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFollowUp:SetFollowUpForQRF"), followUpSetReq, typeof(FollowUpSetRes), ticket);
            return followUpSetRes;
        }

        public async Task<EmailGetRes> SendQuoteFollowUpMail(EmailGetReq emailGetReq, string ticket)
        {
            EmailGetRes emailGetRes = new EmailGetRes();
            emailGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceFollowUp:SendQuoteFollowUpMail"), emailGetReq, typeof(EmailGetRes), ticket);
            return emailGetRes;
        }

        #endregion

        public async Task<CommonResponse> SetQuoteRejectOpportunity(QuoteRejectOpportunityReq request, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:SetQuoteRejectOpportunity"), request, typeof(CommonResponse), ticket);
            return response;
        }

        #region Copy Quote
        public async Task<GetQRFForCopyQuoteRes> GetQRFDataForCopyQuote(QuoteAgentGetReq request, string ticket)
        {
            GetQRFForCopyQuoteRes response = new GetQRFForCopyQuoteRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:GetQRFDataForCopyQuote"), request, typeof(GetQRFForCopyQuoteRes), ticket);
            return response;
        }

        public async Task<SetCopyQuoteRes> SetCopyQuote(SetCopyQuoteReq request, string ticket)
        {
            SetCopyQuoteRes response = new SetCopyQuoteRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:SetCopyQuote"), request, typeof(SetCopyQuoteRes), ticket);
            return response;
        }

        #endregion
        public async Task<QuoteThirdPartyGetRes> GetPartnerQuoteDetails(QuoteThirdPartyGetReq request, string ticket)
        {
            QuoteThirdPartyGetRes response = new QuoteThirdPartyGetRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:GetPartnerQuoteDetails"), request, typeof(QuoteThirdPartyGetRes), ticket);
            return response;
        }
    }
}
