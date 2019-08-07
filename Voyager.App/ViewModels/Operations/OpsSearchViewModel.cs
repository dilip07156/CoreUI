using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
	#region ops search models
	public class OpsSearchViewModel
	{
		public string BookingNumber { get; set; }

		public OpsSearchFilters OpsSearchFilters { get; set; } = new OpsSearchFilters();
		public List<OpsBookingsSearchResult> OpsSearchResult { get; set; } = new List<OpsBookingsSearchResult>();
	}

	public class OpsSearchFilters
	{
		public string AgentId { get; set; }
		public string AgentName { get; set; }
		public string CNKReferenceNo { get; set; }
		public string AgentReferenceNo { get; set; }
		public string AgentTour { get; set; }
		public string From { get; set; }
		public string To { get; set; }

		public string DateType { get; set; }
		public List<AttributeValues> DateTypeList { get; set; } = new List<AttributeValues>();

		public string BookingStatus { get; set; }
		public List<VGER_WAPI_CLASSES.Attributes> BookingStatusList { get; set; } = new List<VGER_WAPI_CLASSES.Attributes>();

		public string BusinessType { get; set; }
		public List<AttributeValues> BusinessTypeList { get; set; } = new List<AttributeValues>();

		public string Destination { get; set; }
		public List<AttributeValues> DestinationList { get; set; } = new List<AttributeValues>();

		//public string SalesOffice { get; set; }
		//public List<ChildrenCompanies> SalesOfficeList { get; set; } = new List<ChildrenCompanies>();

		public string SalesOffice { get; set; }
		public List<AttributeValues> SalesOfficeList { get; set; }

		public string FileHandler { get; set; }
		public List<UserDetails> FileHandlerList { get; set; } = new List<UserDetails>();
	}
	#endregion

	#region booking summary models
	public class OpsBookingSummaryViewModel
	{
		public string BookingNumber { get; set; }
		public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
		public OpsBookingSummaryDetails OpsBookingSummary { get; set; } = new OpsBookingSummaryDetails();
		public List<AttributeValues> ProdTypeList { get; set; } = new List<AttributeValues>();
	}
	#endregion

	#region RoomList 
	public class OpsRoomListSummaryViewModel
	{
		public string BookingNumber { get; set; }
		public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
		public List<PassengerDetails> PassengerDetails { get; set; } = new List<PassengerDetails>();

	}
	#endregion

	#region ItineraryBuilder 
	public class OpsItineraryBuilderSummaryViewModel
	{
		public string BookingNumber { get; set; }
		public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
		public QRFSummaryViewModel QRFSummaryViewModel { get; set; } = new QRFSummaryViewModel();
	}

	#endregion

	#region CommunicationTrail 
	public class OpsCommunicationTrailSummaryViewModel
	{
		public string BookingNumber { get; set; }
		public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
		public CommunicationTrailViewModel CommunicationTrailViewModel { get; set; } = new CommunicationTrailViewModel();
	}
	#endregion

	#region View Service Status
	public class OpsBookingServiceStatusViewModel
	{
		public string BookingNumber { get; set; }
		public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
	}

	public class OpsChangeProductViewModel
	{
		public string BookingNumber { get; set; }
		public string Position_Id { get; set; }
		public string Product_Name { get; set; }
		public string Supplier_Name { get; set; }
		public string PositionStatus { get; set; }
		public DateTime? PositionEndDate { get; set; }
		public List<BookingRoomsAndPrices> BookingRoomsAndPrices { get; set; } = new List<BookingRoomsAndPrices>();
        public HotelAdditionalInfo Attributes { get; set; } = new HotelAdditionalInfo();

        public string CityName { get; set; }
		public string CityID { get; set; }
	}

	public class OpsChangeSupplierViewModel
	{
		public string BookingNumber { get; set; }
		public string Position_Id { get; set; }
		public string Supplier_Name { get; set; }
		public string PositionStatus { get; set; }
		public string ChangeSupplierId { get; set; }
		public List<BookingRoomsAndPrices> BookingRoomsAndPrices { get; set; } = new List<BookingRoomsAndPrices>();
		public List<OpsSupplierDetails> SupplierList { get; set; } = new List<OpsSupplierDetails>();
	}

	public class OpsSupplierDetails
	{
		public string SupplierId { get; set; }
		public string SupplierName { get; set; }
		public string CityName { get; set; }
		public List<OpsProductRangeContract> ProductRangeContract { get; set; } = new List<OpsProductRangeContract>();
	}

	public class OpsProductListViewModel
	{
        public string ServiceType { get; set; }
        public List<OpsProductDetails> OpsProductList { get; set; } = new List<OpsProductDetails>();
	}

	public class OpsProductDetails
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public string CityName { get; set; }
		public string DefaultSupplierId { get; set; }
		public string DefaultSupplier { get; set; }
		public List<OpsProductRangeContract> ProductRangeContract { get; set; } = new List<OpsProductRangeContract>();
	}

	public class OpsProductRangeContract
	{
		public string ProductRangeId { get; set; }
		public string ProductRange { get; set; }
		public string ProductCategoryId { get; set; }
		public string ProductCategory { get; set; }
		public string ContractId { get; set; }
		public double ContractPrice { get; set; }
		public string ContractCurrency { get; set; }
	}

	public class OpsCancelBooking
	{
		public string BookingNumber { get; set; }
		public string PositionId { get; set; }
		[Required(ErrorMessage = "*")]
		public string CancelReason { get; set; }
		public List<Attributes> CancellationList { get; set; } = new List<Attributes>();
		public string AdditionalInfo { get; set; }
		public List<SendRoomingListToHotelVm> HotelList { get; set; } = new List<SendRoomingListToHotelVm>();
		public bool IsSelected { get; set; }
		[Required(ErrorMessage = "*")]
		public string ShiftBookingDate { get; set; }
		public string PosType { get; set; }
		public bool? Placeholder { get; set; }
		public string PositionStatusFull { get; set; }
		public string SystemCompany_Id { get; set; }
	}

	public class OPSContactSupplier
	{
		public string BookingNumber { get; set; }
		public string PositionId { get; set; }
	}

    public class OpsAddPositionViewModel
    {
        public string BookingNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public string PositionType { get; set; }
        public List<string> PositionTypeList { get; set; } = new List<string> { "Core", "Optional", "Post", "Pre", "Supplement" };

        [Required(ErrorMessage = "*")]
        public string PositionStartDate { get; set; }
        [Required(ErrorMessage = "*")]
        public string NoOfDays { get; set; }

        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string EndTime { get; set; }
    }
    #endregion
}
