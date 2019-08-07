using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;


namespace Voyager.App.Library
{
    public class COCommonLibrary
    {
        #region Declaration
        private SalesQuoteLibrary salesQuoteLibrary;
        private COProviders coProviders;
        private readonly IConfiguration _configuration;
        public COCommonLibrary()
        {
        }
        public COCommonLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            salesQuoteLibrary = new SalesQuoteLibrary(_configuration);
            coProviders = new COProviders(_configuration);
        }
        #endregion

        public COHeaderViewModel GetCOTourInfoHeader(ref NewQuoteViewModel model, string token)
        {
            CostingGetReq objCostingRequest = new CostingGetReq() { QRFID = model.QRFID };
            CostingGetRes objCostingResponse = coProviders.GetCostingDetailsByQRFID(objCostingRequest, token).Result;
            CostingGetProperties objResult = objCostingResponse.CostingGetProperties;

            if (objResult != null)
            {
                model.COHeaderViewModel.QRFID = objResult.QRFID;
                model.COHeaderViewModel.AgentID = objResult.AgentInfo.AgentID;
                model.COHeaderViewModel.AgentName = objResult.AgentInfo.AgentName;
                model.COHeaderViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                model.COHeaderViewModel.TourName = objResult.AgentProductInfo.TourName;
                model.COHeaderViewModel.NoOfNights = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                model.COHeaderViewModel.NoOfDays = Convert.ToInt32(objResult.AgentProductInfo.Duration) + 1;
                model.COHeaderViewModel.SalesPerson = objResult.SalesOfficer;
                model.COHeaderViewModel.ContactPersonID = objResult.AgentInfo.ContactPersonID;
                model.COHeaderViewModel.ContactPerson = objResult.AgentInfo.ContactPerson;
                model.COHeaderViewModel.EmailAddress = objResult.AgentInfo.EmailAddress;
                model.COHeaderViewModel.MobileNo = objResult.AgentInfo.MobileNo;
                model.COHeaderViewModel.Destination = objResult.AgentProductInfo.Destination;
                model.COHeaderViewModel.TravelDate = objResult.DepartureDates.Count > 0 ? objResult.DepartureDates[0].Date : null;
                model.COHeaderViewModel.Version = objResult.VersionId;
                model.COHeaderViewModel.Pax = objResult.AgentPassengerInfo.Where(a => a.Type == "ADULT").Select(b => b.count).FirstOrDefault();
                model.COHeaderViewModel.CostingOfficer = objResult.CostingOfficer;
                model.COHeaderViewModel.ProductAccountant = objResult.ProductAccountant;
                model.COHeaderViewModel.ValidForTravel = objResult.ValidForTravel;
                model.COHeaderViewModel.ValidForAcceptance = objResult.ValidForAcceptance;
                model.mdlMenuViewModel.EnquiryPipeline = objCostingResponse.EnquiryPipeline;
                model.COHeaderViewModel.IsLinkedQRFsExist = objCostingResponse.IsLinkedQRFsExist;
                model.COHeaderViewModel.QRFPriceID = objResult.QRFPriceID;
                model.COHeaderViewModel.FollowUpCostingOfficer = objResult.FollowUpCostingOfficer != null ? (Convert.ToDateTime(objResult.FollowUpCostingOfficer)).ToString("dd/MMM/yyyy").Replace('-', '/') : "";
                model.COHeaderViewModel.WithClient = objResult.FollowUpWithClient != null ? (Convert.ToDateTime(objResult.FollowUpWithClient)).ToString("dd/MMM/yyyy").Replace('-', '/') : "";
                model.COHeaderViewModel.SalesPersonMobile = objResult.SalesOfficerMobile;
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
                model.COHeaderViewModel.IsLinkedQRFsExist = false;
                model.COHeaderViewModel.QRFPriceID = "";
            }

            return model.COHeaderViewModel;
        }

        public CommonResponse UpdateQuoteDetails(IConfiguration _configuration, string token, string QRFID, string remarks, string enquiryPipeline, bool IsApproveQuote, string Officer, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            QuoteSetReq objQuoteRequest = new QuoteSetReq();
            string emailId = "";
            objCookies.TryGetValue("EmailId", out emailId);
            emailId = string.IsNullOrEmpty(emailId) ? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault() : emailId;
            string PlacerUser = "";
            objCookies.TryGetValue("UserName", out PlacerUser);
            PlacerUser = string.IsNullOrEmpty(PlacerUser) ? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault() : PlacerUser;
            string userId = "";
            objCookies.TryGetValue("VoyagerUser_Id", out userId);
            userId = string.IsNullOrEmpty(userId) ? SessionInfo.Where(a => a.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault() : userId;
            objQuoteRequest.QRFID = QRFID;
            objQuoteRequest.Remarks = remarks;
            objQuoteRequest.PlacerEmail = emailId;
            objQuoteRequest.PlacerUser = PlacerUser;
            objQuoteRequest.EnquiryPipeline = enquiryPipeline;
            objQuoteRequest.IsApproveQuote = IsApproveQuote;
            objQuoteRequest.PlacerUserId = userId;
            objQuoteRequest.CostingOfficer = Officer;
            objQuoteRequest.VoyagerUserID = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();

            CommonResponse objQuoteResponse = coProviders.UpdateQuoteDetails(objQuoteRequest, token).Result;
            return objQuoteResponse;
        }

        public COHeaderViewModel GetProposalDocumentHeaderDetails(string QRFID, HttpRequest request, HttpResponse response, string token)
        {
            COHeaderViewModel model = new COHeaderViewModel();
            //if (string.IsNullOrEmpty(request.Cookies["TourName"]))
            //{
            QuoteAgentGetReq objQRFAgentRequest = new QuoteAgentGetReq() { QRFID = QRFID };
            COProviders objCOProvider = new COProviders(_configuration);
            ProposalDocumentGetRes objProposalRes = objCOProvider.GetProposalDocumentHeaderDetails(objQRFAgentRequest, token).Result;
            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = QRFID };
            model = GetCOTourInfoHeader(ref modelQuote, token);
            //model.SystemEmail = objProposalRes.SystemEmail;
            //model.SystemPhone = objProposalRes.SystemPhone;
            model.SystemEmail = model.SalesPerson ?? "";
            model.SystemPhone = model.SalesPersonMobile ?? "";
            model.SystemWebsite = objProposalRes.SystemWebsite ?? "";

            //response.Cookies.Append("SystemEmail", objProposalRes.SystemEmail);
            //response.Cookies.Append("SystemPhone", objProposalRes.SystemPhone);
            //response.Cookies.Append("SystemWebsite", objProposalRes.SystemWebsite);
            //response.Cookies.Append("TravelDate", model.TravelDate.ToString());
            //response.Cookies.Append("TourName", model.TourName);
            //response.Cookies.Append("ContactPerson", model.ContactPerson);
            //response.Cookies.Append("AgentName", model.AgentName);
            //response.Cookies.Append("PaxCount", model.Pax.ToString());
            //}
            //else
            //{
            //    model.SystemEmail = request.Cookies["SystemEmail"];
            //    model.SystemPhone = request.Cookies["SystemPhone"];
            //    model.SystemWebsite = request.Cookies["SystemWebsite"];
            //    model.TravelDate = Convert.ToDateTime(request.Cookies["TravelDate"]);
            //    model.TourName = request.Cookies["TourName"];
            //    model.ContactPerson = request.Cookies["ContactPerson"];
            //    model.AgentName = request.Cookies["AgentName"];
            //    model.Pax = Convert.ToInt32(request.Cookies["PaxCount"]);
            //}
            return model;
        }

        #region Update ValidForAcceptance field in mQuote and mQRFPrice collection
        public ResponseStatus UpdateValidForAcceptance(IConfiguration _configuration, string token, string QRFID, string UserName)
        {
            QuoteGetReq objQuoteGetReq = new QuoteGetReq() { QRFID = QRFID, UserName = UserName };
            ResponseStatus objResponseStatus = coProviders.UpdateValidForAcceptance(objQuoteGetReq, token).Result;
            return objResponseStatus;
        }
        #endregion
    }
}
