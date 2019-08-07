using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using System.Linq;
using System.IO;

namespace Voyager.App.Mappers
{
    public class AgentApprovalMapping
    {
        #region Declaration 
        private readonly IConfiguration _configuration;
        AgentApprovalProviders agentApprovalProviders;
        public COCommonLibrary cOCommonLibrary;
        #endregion

        public AgentApprovalMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            agentApprovalProviders = new AgentApprovalProviders(_configuration);
        }

        #region Send To Client
        public SendToClientSetRes SendToClientMail(SendToClientSetReq request, string token)
        {
            SendToClientSetRes objSendToClientSetRes = new SendToClientSetRes() { ResponseStatus=new ResponseStatus()};
            try
            {
                objSendToClientSetRes = agentApprovalProviders.SendToClientMail(request, token).Result;
                objSendToClientSetRes = objSendToClientSetRes != null ? objSendToClientSetRes : new SendToClientSetRes();
            }
            catch (Exception ex)
            {
                objSendToClientSetRes.ResponseStatus.Status = "Failure";
                objSendToClientSetRes.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objSendToClientSetRes;
        }

        public SendToClientSetRes GetSendToClientDetails(SendToClientGetReq request, string token)
        {
            SendToClientSetRes objSendToClientSetRes = new SendToClientSetRes();

            try
            {
                objSendToClientSetRes = agentApprovalProviders.GetSendToClientDetails(request, token).Result;
                objSendToClientSetRes = objSendToClientSetRes != null ? objSendToClientSetRes : new SendToClientSetRes();
            }
            catch (Exception ex)
            {
                objSendToClientSetRes.ResponseStatus.Status = "Failure";
                objSendToClientSetRes.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objSendToClientSetRes;
        }

        public SendToClientSetRes GetSuggestSendToClient(SendToClientGetReq request, string token)
        {
            SendToClientSetRes objSendToClientSetRes = new SendToClientSetRes();

            try
            {
                objSendToClientSetRes = agentApprovalProviders.GetSendToClientDetails(request, token).Result;
                objSendToClientSetRes = objSendToClientSetRes != null ? objSendToClientSetRes : new SendToClientSetRes();
            }
            catch (Exception ex)
            {
                objSendToClientSetRes.ResponseStatus.Status = "Failure";
                objSendToClientSetRes.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objSendToClientSetRes;
        }

        public SendToClientGetRes SetSuggestSendToClient(SendToClientGetReq request)
        {
            SendToClientGetRes objSendToClientGetRes = new SendToClientGetRes();

            try
            {
                objSendToClientGetRes = agentApprovalProviders.SetSuggestSendToClient(request).Result;
                objSendToClientGetRes = objSendToClientGetRes != null ? objSendToClientGetRes : new SendToClientGetRes();
            }
            catch (Exception ex)
            {
                objSendToClientGetRes.ResponseStatus.Status = "Failure";
                objSendToClientGetRes.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objSendToClientGetRes;
        }

        public OfflineMessageViewModel GetSuggestSendToClient(GetSuggestionReq request)
        {
            OfflineMessageViewModel model = new OfflineMessageViewModel();
            model.COHeaderViewModel = new COHeaderViewModel();

            try
            {
                CostingGetRes objCostingResponse = agentApprovalProviders.GetSuggestSendToClient(request).Result;
                if (objCostingResponse != null && objCostingResponse.CostingGetProperties != null)
                {
                    CostingGetProperties objResult = objCostingResponse.CostingGetProperties;
                    model.COHeaderViewModel.QRFID = objResult.QRFID;
                    model.COHeaderViewModel.AgentName = objResult.AgentInfo.AgentName;
                    model.COHeaderViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                    model.COHeaderViewModel.TourName = objResult.AgentProductInfo.TourName;
                    model.COHeaderViewModel.NoOfNights = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                    model.COHeaderViewModel.NoOfDays = Convert.ToInt32(objResult.AgentProductInfo.Duration) + 1;
                    model.COHeaderViewModel.SalesPerson = objResult.SalesOfficer;
                    model.COHeaderViewModel.ContactPerson = objResult.AgentInfo.ContactPerson;
                    model.COHeaderViewModel.Destination = objResult.AgentProductInfo.Destination;
                    model.COHeaderViewModel.TravelDate = objResult.DepartureDates.Count > 0 ? objResult.DepartureDates[0].Date : null;
                    model.COHeaderViewModel.Version = objResult.VersionId;
                    model.SalesOfficerName = objCostingResponse.SalesOfficerName;
                    model.SalesOfficerPhone = objCostingResponse.SalesOfficerPhone;
                    model.MailStatus = objCostingResponse.MailStatus;
                    model.Status = objCostingResponse.Status;
                    model.Document_Id = objResult.Document_Id;

                    // model.COHeaderViewModel.Pax = objResult.AgentPassengerInfo.Where(a => a.Type == "ADULT").Select(b => b.count).FirstOrDefault();
                    //  model.COHeaderViewModel.CostingOfficer = objResult.CostingOfficer;
                    //  model.COHeaderViewModel.ProductAccountant = objResult.ProductAccountant;
                    //  model.COHeaderViewModel.ValidForTravel = objResult.ValidForTravel;
                    //  model.COHeaderViewModel.ValidForAcceptance = objResult.ValidForAcceptance;
                }
                else
                {
                    model.COHeaderViewModel.QRFID = "";
                    model.COHeaderViewModel.AgentName = "";
                    model.COHeaderViewModel.TourCode = "";
                    model.COHeaderViewModel.TourName = "";
                    model.COHeaderViewModel.NoOfNights = 0;
                    model.COHeaderViewModel.NoOfDays = 0;
                    model.COHeaderViewModel.SalesPerson = "";
                    model.COHeaderViewModel.ContactPerson = "";
                    model.COHeaderViewModel.Destination = "";
                    model.COHeaderViewModel.TravelDate = DateTime.MinValue;
                    model.COHeaderViewModel.Version = 0;
                    model.COHeaderViewModel.Pax = 0;
                    model.COHeaderViewModel.CostingOfficer = "";
                    model.COHeaderViewModel.ValidForTravel = "";
                    model.COHeaderViewModel.ValidForAcceptance = "";
                    model.SalesOfficerName = objCostingResponse != null ? objCostingResponse.SalesOfficerName : "";
                    model.SalesOfficerPhone = objCostingResponse != null ? objCostingResponse.SalesOfficerPhone : "";
                    model.MailStatus = objCostingResponse != null ? objCostingResponse.MailStatus : "";
                    model.Status = objCostingResponse != null ? objCostingResponse.Status : "";
                }
            }
            catch (Exception ex)
            {
                model.Status = "Failure";
                model.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return model;
        }

        public OfflineMessageViewModel AcceptSendToClient(SendToClientGetReq request)
        {
            OfflineMessageViewModel model = new OfflineMessageViewModel();
            model.COHeaderViewModel = new COHeaderViewModel();
            try
            {
                AcceptSendToClientSetRes objAcceptSendToClientSetRes = agentApprovalProviders.AcceptSendToClient(request).Result;

                if (objAcceptSendToClientSetRes != null)
                {
                    model.QRFPriceID = request.QRFPriceID;
                    model.QRFID = request.QRFID;
                    model.SalesOfficerName = objAcceptSendToClientSetRes.SalesOfficerName;
                    model.SalesOfficerPhone = objAcceptSendToClientSetRes.SalesOfficerPhone;
                    model.Status = objAcceptSendToClientSetRes.Status;
                    model.MailStatus = objAcceptSendToClientSetRes.MailStatus;
                    model.ErrorMessage = objAcceptSendToClientSetRes.ResponseStatus?.ErrorMessage;

                    if (objAcceptSendToClientSetRes.CostingGetRes != null && objAcceptSendToClientSetRes.CostingGetRes.CostingGetProperties != null)
                    {
                        CostingGetProperties objResult = objAcceptSendToClientSetRes.CostingGetRes.CostingGetProperties;
                        model.COHeaderViewModel.QRFID = objResult.QRFID;
                        model.COHeaderViewModel.AgentName = objResult.AgentInfo.AgentName;
                        model.COHeaderViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                        model.COHeaderViewModel.TourName = objResult.AgentProductInfo.TourName;
                        model.COHeaderViewModel.NoOfNights = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                        model.COHeaderViewModel.NoOfDays = Convert.ToInt32(objResult.AgentProductInfo.Duration) + 1;
                        model.COHeaderViewModel.SalesPerson = objResult.SalesOfficer;
                        model.COHeaderViewModel.ContactPerson = objResult.AgentInfo.ContactPerson;
                        model.COHeaderViewModel.Destination = objResult.AgentProductInfo.Destination;
                        model.COHeaderViewModel.TravelDate = objResult.DepartureDates.Count > 0 ? objResult.DepartureDates[0].Date : null;
                        model.COHeaderViewModel.Version = objResult.VersionId;
                        model.COHeaderViewModel.SystemPhone = objResult.AgentInfo.MobileNo;

                        // model.COHeaderViewModel.Pax = objResult.AgentPassengerInfo.Where(a => a.Type == "ADULT").Select(b => b.count).FirstOrDefault();
                        // model.COHeaderViewModel.CostingOfficer = objResult.CostingOfficer;
                        //  model.COHeaderViewModel.ProductAccountant = objResult.ProductAccountant;
                        //  model.COHeaderViewModel.ValidForTravel = objResult.ValidForTravel;
                        //  model.COHeaderViewModel.ValidForAcceptance = objResult.ValidForAcceptance;
                    }
                    else
                    {
                        model.COHeaderViewModel.QRFID = "";
                        model.COHeaderViewModel.AgentName = "";
                        model.COHeaderViewModel.TourCode = "";
                        model.COHeaderViewModel.TourName = "";
                        model.COHeaderViewModel.NoOfNights = 0;
                        model.COHeaderViewModel.NoOfDays = 0;
                        model.COHeaderViewModel.SalesPerson = "";
                        model.COHeaderViewModel.ContactPerson = "";
                        model.COHeaderViewModel.Destination = "";
                        model.COHeaderViewModel.TravelDate = DateTime.MinValue;
                        model.COHeaderViewModel.Version = 0;
                        model.COHeaderViewModel.Pax = 0;
                        model.COHeaderViewModel.CostingOfficer = "";
                        model.COHeaderViewModel.ValidForTravel = "";
                        model.COHeaderViewModel.ValidForAcceptance = "";
                    }
                }
            }
            catch (Exception ex)
            {
                model.Status = "Failure";
                model.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return model;
        }
        #endregion

        #region AcceptWithoutProposal
        public CommonResponse AcceptWithoutProposal(EmailGetReq request, string token)
        {
            CommonResponse objCommonResponse = new CommonResponse();
            try
            {
                objCommonResponse = agentApprovalProviders.AcceptWithoutProposal(request, token).Result;
                objCommonResponse = objCommonResponse != null ? objCommonResponse : new CommonResponse();
            }
            catch (Exception ex)
            {
                objCommonResponse.ResponseStatus.Status = "Failure";
                objCommonResponse.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objCommonResponse;
        }
        #endregion

        #region CheckProposalGenerated
        public CommonResponse CheckProposalGenerated(QuoteGetReq request, string token)
        {
            CommonResponse objCommonResponse = new CommonResponse();
            try
            {
                objCommonResponse = agentApprovalProviders.CheckProposalGenerated(request, token).Result;
                objCommonResponse = objCommonResponse != null ? objCommonResponse : new CommonResponse();
            }
            catch (Exception ex)
            {
                objCommonResponse.ResponseStatus.Status = "Failure";
                objCommonResponse.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
            }
            return objCommonResponse;
        }
        #endregion

        #region Common Function
        public AttachToMasterViewModel GetSalesOfficerHeraderInfo(string QRFID, string MenuName, string token)
        {
            AttachToMasterViewModel model = new AttachToMasterViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = MenuName;

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return model;
        }
        #endregion
    }
}