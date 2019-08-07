using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class UpdateAvailabilityViewModel
    {
		public string SupplierId { get; set; }
		public string BookingNumber { get; set; }
        public string PositionId { get; set; }
        public string AltSvcId { get; set; }
        public string Availability { get; set; }
        public string Status { get; set; }
        public string LinkStatus { get; set; }
        public string OpenMode { get; set; }
        public string StatusMessage { get; set; }
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();
        public ProductsSRPViewModel ProductsSRPViewModel { get; set; } = new ProductsSRPViewModel();
        public ReservationRequestDetails ReservationRequestDetails { get; set; } = new ReservationRequestDetails();
        public UpdateAvailabilityReqDetails UpdateReqDetails { get; set; } = new UpdateAvailabilityReqDetails();
        public List<AltSvcRoomsAndPrices> RoomRateDetails { get; set; } = new List<AltSvcRoomsAndPrices>();
        public List<BudgetSupplements> BudgetSupplements { get; set; } = new List<BudgetSupplements>();
        public List<Currency> BudgetCurrencyList { get; set; } = new List<Currency>();
		//public List<AltSvcRoomsAndPrices> AltSvcRoomsAndPrices { get; set; } = new List<AltSvcRoomsAndPrices>(); 
		public CommunicationTrailViewModel CommunicationTrailViewModel { get; set; } = new CommunicationTrailViewModel();
	}

    public class ActivateHotelViewModel
    {
        public string BookingNumber { get; set; }
        public string PositionId { get; set; }
        public string AltSvcId { get; set; }
        //public string Availability { get; set; }
        public PositionProductDetails PositionProductDetails { get; set; } = new PositionProductDetails();
        public ReservationRequestDetails ReservationRequestDetails { get; set; } = new ReservationRequestDetails();
        public UpdateAvailabilityReqDetails UpdateReqDetails { get; set; } = new UpdateAvailabilityReqDetails();
        public List<AltSvcRoomsAndPrices> RoomRateDetails { get; set; } = new List<AltSvcRoomsAndPrices>();
        public ProductsSRPViewModel PosProductSRPViewModel { get; set; } = new ProductsSRPViewModel();
        public ProductsSRPViewModel AltProductSRPViewModel { get; set; } = new ProductsSRPViewModel();
        public EmailTemplateGetRes EmailTemplateModel { get; set; } = new EmailTemplateGetRes();
    }
}
