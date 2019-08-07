using System.Collections.Generic; 
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class ItineraryViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public mItinerary Itinerary { get; set; }

        public List<QRFPkgAndNonPkgPrice> ListNonQrfPkgPositions { get; set; } = new List<QRFPkgAndNonPkgPrice>();

        public bool IsClone { get; set; } = true;

        public long PaxSlab { get; set; }
        public List<AttributeValues> PaxSlabList { get; set; }

        public long DepartureDate { get; set; }
        public List<AttributeValues> DepartureDatesList { get; set; }

        public long SupplementProduct { get; set; }
        public List<Attributes> SupplementProductList { get; set; }

        public long OptionalProduct { get; set; }
        public List<Attributes> OptionalProductList { get; set; }

        public string Days { get; set; }
        public List<Attributes> DayList { get; set; }

        public string Services { get; set; }
        public List<mProductType> ServiceTypeList { get; set; }

        public List<QRFPkgAndNonPkgPrice> MainTourList { get; set; }

        public List<QRFPkgAndNonPkgPrice> QRFNonPkgSupplementPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> SupplementList { get; set; }

        public List<QRFPkgAndNonPkgPrice> QRFNonPkgOptionalPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> OptionalList { get; set; }

        public AgentApprovalHeaderButtons AgentApprovalHeaderButtons { get; set; }         

        public ItineraryViewModel()
        {
            MenuViewModel = new MenuViewModel();
            COHeaderViewModel = new COHeaderViewModel();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            ListNonQrfPkgPositions = new List<QRFPkgAndNonPkgPrice>();
        } 
    }
}
