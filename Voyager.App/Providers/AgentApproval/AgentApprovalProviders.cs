using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class AgentApprovalProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        public AgentApprovalProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region Send To Client Mail      
        public async Task<SendToClientSetRes> SendToClientMail(SendToClientSetReq sendToClientSetReq, string ticket)
        {
            SendToClientSetRes sendToClientSetRes = new SendToClientSetRes();
            sendToClientSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:SendToClientMail"), sendToClientSetReq, typeof(SendToClientSetRes), ticket);
            return sendToClientSetRes;
        }

        public async Task<SendToClientSetRes> GetSendToClientDetails(SendToClientGetReq sendToClientGetReq, string ticket)
        {
            SendToClientSetRes sendToClientSetRes = new SendToClientSetRes();
            sendToClientSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:GetSendToClientDetails"), sendToClientGetReq, typeof(SendToClientSetRes), ticket);
            return sendToClientSetRes;
        }

        public async Task<SendToClientGetRes> SetSuggestSendToClient(SendToClientGetReq sendToClientGetReq)
        {
            SendToClientGetRes sendToClientGetRes = new SendToClientGetRes();
            sendToClientGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:SetSuggestSendToClient"), sendToClientGetReq, typeof(SendToClientGetRes));
            return sendToClientGetRes;
        }

        public async Task<CostingGetRes> GetSuggestSendToClient(GetSuggestionReq getSuggestionReq)
        {
            CostingGetRes costingGetRes = new CostingGetRes();
            costingGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:GetSuggestSendToClient"), getSuggestionReq, typeof(CostingGetRes));
            return costingGetRes;
        }

        public async Task<AcceptSendToClientSetRes> AcceptSendToClient(SendToClientGetReq sendToClientGetReq)
        {
            AcceptSendToClientSetRes acceptSendToClientSetRes = new AcceptSendToClientSetRes();
            acceptSendToClientSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:AcceptSendToClient"), sendToClientGetReq, typeof(AcceptSendToClientSetRes));
            return acceptSendToClientSetRes;
        }
        #endregion

        public async Task<CommonResponse> AcceptWithoutProposal(EmailGetReq reqEmailGetReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:AcceptWithoutProposal"), reqEmailGetReq, typeof(CommonResponse), ticket);
            return response;
        }

        public async Task<CommonResponse> CheckProposalGenerated(QuoteGetReq reqQuoteGetReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:CheckProposalGenerated"), reqQuoteGetReq, typeof(CommonResponse), ticket);
            return response;
        }

        public async Task<CommonResponse> AmendmentQuote(AmendmentQuoteReq amendmentQuoteReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("AgentApproval:AmendmentQuote"), amendmentQuoteReq, typeof(CommonResponse), ticket);
            return response;
        }
    }
}
