using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class ProposalViewModel
    {
        public string QRFID { get; set; }

        public string Remarks { get; set; }

        public string CurrentDate { get; set; }

        public string UserName { get; set; }

        public string ProposalId { get; set; }

        public bool IsNewVersion { get; set; } = false;

        public COHeaderViewModel COHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public ProposalPriceBreakupViewModel ProposalPriceBreakupViewModel { get; set; }

        public ProposalInclusionsExclusionsViewModel ProposalInclusionsExclusionsViewModel { get; set; }

        public ProposalTermsViewModel ProposalTermsViewModel { get; set; }

        public ProposalCoveringNoteViewModel ProposalCoveringNoteViewModel { get; set; }

        public mItinerary Itinerary { get; set; }

        public AgentApprovalHeaderButtons AgentApprovalHeaderButtons { get; set; }

        public string Officer { get; set; }
        public List<CompanyContacts> OfficerList { get; set; }

        public ProposalViewModel()
        {
            MenuViewModel = new MenuViewModel();
            COHeaderViewModel = new COHeaderViewModel();
            ProposalPriceBreakupViewModel = new ProposalPriceBreakupViewModel();
            ProposalInclusionsExclusionsViewModel = new ProposalInclusionsExclusionsViewModel();
            ProposalTermsViewModel = new ProposalTermsViewModel();
            ProposalCoveringNoteViewModel = new ProposalCoveringNoteViewModel();
            AgentApprovalHeaderButtons = new AgentApprovalHeaderButtons();
        }
    }

    public class ProposalPriceBreakupViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }
        public string PriceBreakUp { get; set; }        
        public List<QRFPkgAndNonPkgPrice> DepartureDatesList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QrfTwinPkgPriceList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QrfDoublePkgPriceList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QrfSinglePkgPriceList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgSupplementPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgOptionalPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> QrfNonPkgPriceSupplementList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QrfNonPkgPriceOptionalList { get; set; }
        public List<Hotel> HotelList { get; set; }

        public ProposalPriceBreakupViewModel()
        {
            COHeaderViewModel = new COHeaderViewModel();
            DepartureDatesList = new List<QRFPkgAndNonPkgPrice>();
            QrfTwinPkgPriceList = new List<QRFPkgAndNonPkgPrice>();
            QrfDoublePkgPriceList = new List<QRFPkgAndNonPkgPrice>();
            QrfSinglePkgPriceList = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgSupplementPositions = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgOptionalPositions = new List<QRFPkgAndNonPkgPrice>();
            QrfNonPkgPriceSupplementList = new List<QRFPkgAndNonPkgPrice>();
            QrfNonPkgPriceOptionalList = new List<QRFPkgAndNonPkgPrice>();
            HotelList = new List<Hotel>();
        }
    }

    public class ExtraPosition
    {
        public string QRFID { get; set; }
        public string ItineraryID { get; set; }
        public string ItineraryDaysId { get; set; }
        public string PositionId { get; set; }
        public string Day { get; set; }
        public List<Attributes> DayList { get; set; }    
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }
        public string City { get; set; }
        public List<Attributes> CityList { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Type { get; set; }
		public bool flag { get; set; } = false;
	}
        
    public class ProposalRouting
    {

    }

    public class ProposalInclusionsExclusionsViewModel
    {       
        public List<TermsAndConditions> InclusionList { get; set; }
        public string Inclusions { get; set; }
        public List<TermsAndConditions> ExclusionList { get; set; }
        public string Exclusions { get; set; }
    }

    public class ProposalTermsViewModel
    {
        public string Terms { get; set; }
        public List<TermsAndConditions> TermsList { get; set; }
        public ProposalIncludeRegions ProposalIncludeRegions { get; set; } = new ProposalIncludeRegions();
    }

    public class ProposalCoveringNoteViewModel
    {
        public string QRFID { get; set; }
        public string SalesPersonUserName { get; set; }
        public string AgentTitle { get; set; }
        public string AgentFirstName { get; set; }
        public string AgentLastName { get; set; }
        public string CompanyName { get; set; }
        public string Destination { get; set; }
        public string SalesPersonCommonTitle { get; set; }
        public string SalesPersonTitle { get; set; }
        public string SalesPersonPhone { get; set; }
        public string SalesPersonFax { get; set; }
        public string SalesPersonEmail { get; set; }
        public string SalesPersonFullName { get; set; }
        public string CoveringNote { get; set; }
        public ProposalIncludeRegions ProposalIncludeRegions { get; set; } = new ProposalIncludeRegions();
    }

    public class ProposalReview
    {

    }

    public class TermsAndConditions
    {
        public string Section { get; set; }
        public string Tcs { get; set; }
        public int? OrderNumber { get; set; }
    }

}
