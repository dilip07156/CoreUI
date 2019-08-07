using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voyager.App.ViewModels;
using Microsoft.Extensions.Configuration;
using Voyager.App.Library;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Microsoft.AspNetCore.Http;

namespace Voyager.Controllers
{
    public class AmendmentController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        COProviders coProviders;
        AgentProviders agentProviders;

        public AmendmentController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            coProviders = new COProviders(_configuration);
            agentProviders = new AgentProviders(_configuration);
        }
        #endregion

        public IActionResult Itinerary(string QRFID)
        {
            ItineraryViewModel model = new ItineraryViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Itinerary";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return View(model);
        }

        public IActionResult Guesstimate(string QRFID)
        {
            GuesstimateViewModel model = new GuesstimateViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Guesstimate";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            return View(model);
        }

        public IActionResult Commercials()
        {
            string QRFID = Request.Query["QRFId"];

            CommercialsViewModel model = new CommercialsViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Commercials";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            return View(model);
        }

        public IActionResult Costsheet()
        {
            string QRFID = Convert.ToString(Request.Query["QRFId"]);

            CostsheetViewModel model = new CostsheetViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Costsheet";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            #region Dropdown Binding
            QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
            QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
            model.ConsolidatedSummaryViewModel.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
            #endregion

            #region Get Costsheet by QRFId
            CostsheetGetReq request = new CostsheetGetReq();
            CostsheetGetRes response = new CostsheetGetRes();
            request.QRFID = QRFID;
            response = coProviders.GetCostsheet(request, token).Result;
            model.ConsolidatedSummaryViewModel.CostsheetVersion = response.CostsheetVersion;
            #endregion

            return View(model);
        }

        public IActionResult Proposal()
        {
            string QRFID = Request.Query["QRFId"];
            bool GetStatus = false;

            ProposalViewModel model = new ProposalViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Proposal";
            model.CurrentDate = DateTime.Now.ToString("dd MMM yyyy");
            string username = "";
            username = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
            model.UserName = username;

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            ProposalLibrary proposalLibrary = new ProposalLibrary(_configuration);

            CompanyOfficerGetRes officerGetRes = new CompanyOfficerGetRes();
            CompanyOfficerGetReq officerGetReq = new CompanyOfficerGetReq();
            officerGetReq.CompanyId = ckUserCompanyId;
            officerGetReq.UserRole = "Product Accountant";
            officerGetRes = agentProviders.GetCompanyOfficers(officerGetReq, token).Result;

            model.OfficerList = officerGetRes.ContactDetails;
            var salespipelineres = agentProviders.GetAutomatedSalesPipelineRoles(new SettingsAutomatedGetReq { CompanyId = ckUserCompanyId, QRFId = QRFID, UserRole = officerGetReq.UserRole }, token).Result;
            if (!string.IsNullOrEmpty(salespipelineres?.ResponseStatus?.Status))
            {
                model.Officer = salespipelineres.UserEmailId;
            }
            #region PriceBreakUp
            if (!string.IsNullOrEmpty(QRFID))
            {
                model.QRFID = QRFID;
                GetStatus = proposalLibrary.GetProposalPriceBreakupByQRFId(_configuration, token, ref model, QRFID);
            }

            ProposalGetReq request = new ProposalGetReq();
            ProposalGetRes response = new ProposalGetRes();
            request.QRFID = QRFID;
            response = coProviders.GetProposal(request, token).Result;
            if (response != null)
            {
                model.ProposalId = response.Proposal.ProposalId;
                model.ProposalPriceBreakupViewModel.PriceBreakUp = response.Proposal.PriceBreakup;
            }

            #endregion
            return View(model);
        }
    }
}