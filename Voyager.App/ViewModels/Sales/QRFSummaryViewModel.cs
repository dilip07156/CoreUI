using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class QRFSummaryViewModel
    {
        public string QRFID { get; set; }

        public string PageName { get; set; }

        public string QuoteRemarks { get; set; }

        public string CurrentDate { get; set; }

        public string UserName { get; set; }

        public string Days { get; set; }
        public List<Attributes> DayList { get; set; }

        public string Officer { get; set; }
        public List<CompanyContacts> OfficerList { get; set; }

        public string Services { get; set; }
        public List<mProductType> ServiceTypeList { get; set; }
        
        public List<SummaryDetails> SummaryDetails { get; set; }       

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public mItinerary Itinerary { get; set; }

		public List<OpsPositionDetails> Positions { get; set; }

        public QRFSummaryViewModel()
        {
            MenuViewModel = new MenuViewModel();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();            
            SummaryDetails = new List<SummaryDetails>();
            Itinerary = new mItinerary();
			Positions = new List<OpsPositionDetails>();
        }
    }

    public class SummaryDetails
    {
        public string Day { get; set; }

        public string PlaceOfService { get; set; }

        public string OriginalItineraryDate { get; set; }

        public string OriginalItineraryDay { get; set; }

        public string OriginalItineraryName { get; set; }

        public List<OriginalItineraryDetails> OriginalItineraryDetails { get; set; }

        public SummaryDetails()
        {
            OriginalItineraryDetails = new List<OriginalItineraryDetails>();
        }
    }

    public class OriginalItineraryDetails
    {
        public string ItineraryId { get; set; }

        public string ItineraryDaysId { get; set; }

        public string PositionId { get; set; }

        public string OriginalItineraryDescription { get; set; }

        public string SupplierId { get; set; }

        public string Supplier { get; set; }

        public string Allocation { get; set; }

        public int? NumberOfPax { get; set; }

        public string ProductType { get; set; }

        public string TLRemarks { get; set; }

        public string OPSRemarks { get; set; }

        public bool IsDeleted { get; set; }

        public string KeepAs { get; set; }

        public string ProductCategoryId { get; set; }

        public string ProductCategory { get; set; }

        public string ProductTypeChargeBasis { get; set; }

        public string BuyCurrency { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

		public string BookingNumber { get; set; }
	}
    
}
