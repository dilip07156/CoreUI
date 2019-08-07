using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class COProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        
        public COProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Common Methods

        public async Task<CostingGetRes> GetCostingDetailsByQRFID(CostingGetReq objCostingRequest, string ticket)
        {
            CostingGetRes objCostingResponse = new CostingGetRes();
            objCostingResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommon:GetCostingDetailsByQRFID"), objCostingRequest, typeof(CostingGetRes), ticket);
            return objCostingResponse;
        }

        public async Task<QRFDepartureDateGetRes> GetDepartureDatesForCostingByQRF_Id(QRFDepartureDateGetReq objDepartureDateRequest, string ticket)
        {
            QRFDepartureDateGetRes objDepartureDateResponse = new QRFDepartureDateGetRes();
            objDepartureDateResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommon:GetDepartureDates"), objDepartureDateRequest, typeof(QRFDepartureDateGetRes), ticket);
            return objDepartureDateResponse;
        }

        public async Task<QRFPaxGetResponse> GetPaxSlabDetailsForCostingByQRF_Id(QRFPaxSlabGetReq objQRFPaxGetReq, string ticket)
        {
            QRFPaxGetResponse objPaxSetRes = new QRFPaxGetResponse();
            objPaxSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommon:GetPaxSlabs"), objQRFPaxGetReq, typeof(QRFPaxGetResponse), ticket);
            return objPaxSetRes;
        }       
        #endregion

        #region Guesstimate
        public async Task<GuesstimateGetRes> GetGuesstimate(GuesstimateGetReq guesstimateGetReq, string ticket)
        {
            GuesstimateGetRes guesstimateGetRes = new GuesstimateGetRes();
            guesstimateGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:GetGuesstimate"), guesstimateGetReq, typeof(GuesstimateGetRes), ticket);
            return guesstimateGetRes;
        }
        
        public async Task<GuesstimateSetRes> SetGuesstimate(GuesstimateSetReq guesstimateSetReq, string ticket)
        {
            GuesstimateSetRes guesstimateSetRes = new GuesstimateSetRes();
            guesstimateSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:SetGuesstimate"), guesstimateSetReq, typeof(GuesstimateSetRes), ticket);
            return guesstimateSetRes;
        }

        public async Task<GuesstimateVersionGetRes> GetGuesstimateVersions(GuesstimateGetReq guesstimateGetReq, string ticket)
        {
            GuesstimateVersionGetRes guesstimateGetRes = new GuesstimateVersionGetRes();
            guesstimateGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:GetGuesstimateVersions"), guesstimateGetReq, typeof(GuesstimateVersionGetRes), ticket);
            return guesstimateGetRes;
        }
        
        public async Task<GuesstimateSetRes> UpdateGuesstimateVersion(GuesstimateVersionSetReq guesstimateSetReq, string ticket)
        {
            GuesstimateSetRes guesstimateSetRes = new GuesstimateSetRes();
            guesstimateSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:UpdateGuesstimateVersion"), guesstimateSetReq, typeof(GuesstimateSetRes), ticket);
            return guesstimateSetRes;
        }

        public async Task<GuesstimateGetRes> GetSupplierPrice(GuesstimateGetReq guesstimateGetReq, string ticket)
        {
            GuesstimateGetRes guesstimateGetRes = new GuesstimateGetRes();
            guesstimateGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:GetSupplierPrice"), guesstimateGetReq, typeof(GuesstimateGetRes), ticket);
            return guesstimateGetRes;
        }

        public async Task<GuesstimateSetRes> SetGuesstimateChangeRule(GuesstimateChangeRuleSetReq guesstimateCRSetReq, string ticket)
        {
            GuesstimateSetRes guesstimateSetRes = new GuesstimateSetRes();
            guesstimateSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGuesstimate:SetGuesstimateChangeRule"), guesstimateCRSetReq, typeof(GuesstimateSetRes), ticket);
            return guesstimateSetRes;
        }
        #endregion

        #region Itinerary
        public async Task<ItineraryGetRes> GetItinerary(ItineraryGetReq itineraryGetReq, string ticket)
        {
            ItineraryGetRes itineraryGetRes = new ItineraryGetRes();
            itineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:GetItinerary"), itineraryGetReq, typeof(ItineraryGetRes), ticket);
            return itineraryGetRes;
        }

        public async Task<ItineraryGetRes> GetItineraryDetails(ItineraryGetReq itineraryGetReq, string ticket)
        {
            ItineraryGetRes itineraryGetRes = new ItineraryGetRes();
            itineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:GetItineraryDetails"), itineraryGetReq, typeof(ItineraryGetRes), ticket);
            return itineraryGetRes;
        }

        public async Task<ItinerarySetRes> SetItinerary(ItinerarySetReq itinerarySetReq, string ticket)
        {
            ItinerarySetRes itinerarySetRes = new ItinerarySetRes();
            itinerarySetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:SetItinerary"), itinerarySetReq, typeof(ItinerarySetRes), ticket);
            return itinerarySetRes;
        }

        public async Task<ItinerarySetRes> EnableDisablePosition(ItinerarySetReq itinerarySetReq, string ticket)
        {
            ItinerarySetRes itinerarySetRes = new ItinerarySetRes();
            itinerarySetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:EnableDisablePosition"), itinerarySetReq, typeof(ItinerarySetRes), ticket);
            return itinerarySetRes;
        }

        public async Task<ItinerarySetRes> SaveRemarks(ItinerarySetReq itinerarySetReq, string ticket)
        {
            ItinerarySetRes itinerarySetRes = new ItinerarySetRes();
            itinerarySetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:SaveRemarks"), itinerarySetReq, typeof(ItinerarySetRes), ticket);
            return itinerarySetRes;
        }

        public async Task<ItineraryGetRes> GetQRFPosition(ItineraryGetReq itineraryGetReq, string ticket)
        {
            ItineraryGetRes itineraryGetRes = new ItineraryGetRes();
            itineraryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceItinerary:GetQRFPosition"), itineraryGetReq, typeof(ItineraryGetRes), ticket);
            return itineraryGetRes;
        }
        #endregion

        #region Costsheet
        public async Task<CostsheetGetRes> GetCostsheet(CostsheetGetReq costsheetGetReq, string ticket)
        {
            CostsheetGetRes costsheetGetRes = new CostsheetGetRes();
            costsheetGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:GetCostsheet"), costsheetGetReq, typeof(CostsheetGetRes), ticket);
            return costsheetGetRes;
        }

        public async Task<CostsheetVerSetRes> SetCostsheetNewVersion(CostsheetVerSetReq xostsheetSetReq, string ticket)
        {
            CostsheetVerSetRes costsheetSetRes = new CostsheetVerSetRes();
            costsheetSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:SetCostsheetNewVersion"), xostsheetSetReq, typeof(CostsheetVerSetRes), ticket);
            return costsheetSetRes;
        }

        public async Task<CostsheetVersionGetRes> GetCostsheetVersions(CostsheetGetReq costsheetGetReq, string ticket)
        {
            CostsheetVersionGetRes costsheetGetRes = new CostsheetVersionGetRes();
            costsheetGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:GetCostsheetVersions"), costsheetGetReq, typeof(CostsheetVersionGetRes), ticket);
            return costsheetGetRes;
        }

        public async Task<CostsheetVerSetRes> UpdateCostsheetVersion(CostsheetVerSetReq costsheetVerSetReq , string ticket)
        {
            CostsheetVerSetRes costsheetVerSetRes = new CostsheetVerSetRes();
            costsheetVerSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:UpdateCostsheetVersion"), costsheetVerSetReq, typeof(CostsheetVerSetRes), ticket);
            return costsheetVerSetRes;
        }

        public async Task<ResponseStatus> CheckActiveCostsheetPrice(CostsheetGetReq costsheetGetReq, string ticket)
        {
            ResponseStatus responseStatus = new ResponseStatus();
            responseStatus = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:CheckActiveCostsheetPrice"), costsheetGetReq, typeof(ResponseStatus), ticket);
            return responseStatus;
        }
        #endregion

        #region Proposal
        public async Task<ProposalGetRes> GetProposal(ProposalGetReq proposalGetReq, string ticket)
        {
            ProposalGetRes proposalGetRes = new ProposalGetRes();
            proposalGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProposal:GetProposal"), proposalGetReq, typeof(ProposalGetRes), ticket);
            return proposalGetRes;
        }

        public async Task<ProposalSetRes> SetProposal(ProposalSetReq proposalSetReq, string ticket)
        {
            ProposalSetRes proposalSetRes = new ProposalSetRes();
            proposalSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProposal:SetProposal"), proposalSetReq, typeof(ProposalSetRes), ticket);
            return proposalSetRes;
        }

        public async Task<ProposalDocumentGetRes> GetProposalDocumentDetailsByQRFID(QuoteAgentGetReq proposalGetReq, string ticket)
        {
            ProposalDocumentGetRes proposalGetRes = new ProposalDocumentGetRes();
            proposalGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProposal:GetProposalDocumentDetailsByQRFID"), proposalGetReq, typeof(ProposalDocumentGetRes), ticket);
            return proposalGetRes;
        }

        public async Task<ProposalDocumentGetRes> GetProposalDocumentHeaderDetails(QuoteAgentGetReq proposalGetReq, string ticket)
        {
            ProposalDocumentGetRes proposalGetRes = new ProposalDocumentGetRes();
            proposalGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProposal:GetProposalDocumentHeaderDetails"), proposalGetReq, typeof(ProposalDocumentGetRes), ticket);
            return proposalGetRes;
        }

        public async Task<ProposalGetRes> GetHotelSummaryByQrfId(ProposalGetReq proposalGetReq, string ticket)
        {
            ProposalGetRes proposalGetRes = new ProposalGetRes();
            proposalGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceProposal:GetHotelSummaryByQrfId"), proposalGetReq, typeof(ProposalGetRes), ticket);
            return proposalGetRes;
        }

        #endregion

        #region Commercials
        public async Task<CommercialsGetRes> GetCommercials(CommercialsGetReq commercialsGetReq, string ticket)
        {
            CommercialsGetRes commercialsGetRes = new CommercialsGetRes();
            commercialsGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommercials:GetCommercials"), commercialsGetReq, typeof(CommercialsGetRes), ticket);
            return commercialsGetRes;
        }
        
        public async Task<CommonResponse> ChangePositionKeepAs(ChangePositionKeepReq changePositionKeepReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommercials:ChangePositionKeepAs"), changePositionKeepReq, typeof(CommonResponse), ticket);
            return response;
        }

        public async Task<CommonResponse> SaveCommercials(CommercialsSetReq commercialsSetReq, string ticket)
        {
            CommonResponse response = new CommonResponse();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommercials:SaveCommercials"), commercialsSetReq, typeof(CommonResponse), ticket);
            return response;
        }

        public async Task<CommonResponse> UpdateQuoteDetails(QuoteSetReq objQuoteRequest, string ticket)
        {
            CommonResponse objCommonResponse = new CommonResponse();
            objCommonResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCommercials:SetQuoteDetails"), objQuoteRequest, typeof(CommonResponse), ticket);
            return objCommonResponse;
        }
        #endregion

        #region Update ValidForAcceptance field in mQuote and mQRFPrice collection
        public async Task<ResponseStatus> UpdateValidForAcceptance(QuoteGetReq quoteGetReq, string ticket)
        {
            ResponseStatus response = new ResponseStatus();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQuote:UpdateValidForAcceptance"), quoteGetReq, typeof(ResponseStatus), ticket);
            return response;
        }
        #endregion
    }
}

