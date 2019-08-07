using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Voyager.App.Models;
using Voyager.App.Contracts;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class QuoteViewModel
    {
        public string QuoteType { get; set; }

        //Product Information
        [Required]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; }

        [Required]
        [Display(Name = "Tour Code")]
        public string TourCode { get; set; }

        public int ApproxPaxAdult { get; set; }
        public int ApproxPaxChild { get; set; }
        public int ApproxPaxInfant { get; set; }
        public int Duration { get; set; }

        [Required]
        [Display(Name = "Budget")]
        public double BudgetAmount { get; set; }
    }

    public class NewQuoteViewModel
    {
        public string QRFID { get; set; }

        public MenuViewModel mdlMenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public COHeaderViewModel COHeaderViewModel { get; set; }

        public QuoteAgentInfoViewModel mdlQuoteAgentInfoViewModel { get; set; }

        public QuoteDateRangeViewModel mdlQuoteDateRangeViewModel { get; set; }

        public QuotePaxRangeViewModel mdlQuotePaxRangeViewModel { get; set; }

        public QuoteFOCViewModel mdlQuoteFOCViewModel { get; set; }

        public QuoteRoutingViewModel mdlQuoteRoutingViewModel { get; set; }

        public QuoteMarginViewModel mdlQuoteMarginViewModel { get; set; }

        public NewQuoteViewModel()
        {
            //QRFId = 0;
            mdlMenuViewModel = new MenuViewModel { MenuName = "NewQuote" };
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            COHeaderViewModel = new COHeaderViewModel();
            mdlQuoteAgentInfoViewModel = new QuoteAgentInfoViewModel();
            mdlQuoteDateRangeViewModel = new QuoteDateRangeViewModel();
            mdlQuotePaxRangeViewModel = new QuotePaxRangeViewModel();
            mdlQuoteRoutingViewModel = new QuoteRoutingViewModel();
            mdlQuoteMarginViewModel = new QuoteMarginViewModel();
            mdlQuoteFOCViewModel = new QuoteFOCViewModel();
        }
    }

    public class QuoteAgentInfoViewModel
    {
        public string QRFID { get; set; }

        [Required]
        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }
        public string AgentId { get; set; }
        public List<AgentProperties> AgentNameList { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        public List<ContactProperties> ContactPersonList { get; set; }

        [Display(Name = "Mobile")]
        public string MobileNo { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Product Type")]
        public string ProductType { get; set; }
        public List<AttributeValues> ProductTypeList { get; set; }

        [Required]
        [Display(Name = "Product Division")]
        public string ProductDivisionId { get; set; }
        public string ProductDivision { get; set; }
        public List<AttributeValues> ProductDivisionList { get; set; }

        [Required]
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }
        public List<AttributeValues> NationalityList { get; set; }

        public string PurposeOfTravel { get; set; }
        public List<AttributeValues> PurposeOfTravelList { get; set; }

        public string Destination { get; set; }
        public List<AttributeValues> DestinationList { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; }
        public List<AttributeValues> PriorityList { get; set; }

        [Required]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; }

        [Required]
        [Display(Name = "Tour Code")]
        public string TourCode { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Adult count is required")]
        [Display(Name = "Adult")]
        public int ApproxPaxAdult { get; set; }
        public int ApproxPaxChildWithBed { get; set; }
        public int ApproxPaxChildWithoutBed { get; set; }
        public int ApproxPaxInfant { get; set; }
        public string ApproxPaxChildWithBedAge { get; set; }
        public string ApproxPaxChildWithoutBedAge { get; set; }
        public string Module { get; set; }
        public string PartnerEntityCode { get; set; }
        public string Application { get; set; }
        public string Operation { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        public string BudgetCurrency { get; set; }
        public List<Currency> BudgetCurrencyList { get; set; }
        public double BudgetAmount { get; set; }

        [Required(ErrorMessage = "Quote Me For is required")]
        [Display(Name = "Quote Me For")]
        public List<QuoteRoomTypeData> QuoteRoomType { get; set; }
        public List<QuoteRoomTypeData> QuoteRoomTypeList { get; set; }

        [Required]
        [Display(Name = "How would you like the Quote")]
        public string QuotePricingType { get; set; }

        [Required]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = "As Per Invoice";

        public QuoteAgentInfoViewModel()
        {
            // QRFId = "0";
            AgentNameList = new List<AgentProperties>();
            ContactPersonList = new List<ContactProperties>();
            ProductTypeList = new List<AttributeValues>();
            ProductDivisionList = new List<AttributeValues>();
            NationalityList = new List<AttributeValues>();
            PurposeOfTravelList = new List<AttributeValues>();
            DestinationList = new List<AttributeValues>();
            PriorityList = new List<AttributeValues>();
            BudgetCurrencyList = new List<Currency>();
            QuoteRoomType = new List<QuoteRoomTypeData>();
            QuoteRoomTypeList = new List<QuoteRoomTypeData>();
        }
    }

    public class QuoteRoomTypeData
    {
        public bool IsSelected { get; set; }
        public string RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public int? RoomCount { get; set; }
    }

    public class QuoteDateRangeViewModel
    {
        [Required(ErrorMessage = "The QRF Id is required.")]
        public string QRFID { get; set; }

        [Required(ErrorMessage = "The Travel Date Type is required.")]
        public string TravelDateType { get; set; }

        public List<QuoteSpecificDateViewModel> QuoteSpecificDateViewModel { get; set; }

        public QuoteDateRangeViewModel()
        {
            QuoteSpecificDateViewModel = new List<QuoteSpecificDateViewModel>();
        }
    }

    public class QuoteSpecificDateViewModel
    {
        public long DepartureId { get; set; }

        [Required(ErrorMessage = "*")]
        public string SelectedDate { get; set; }

        [Required(ErrorMessage = "*")]
        public int NoOfDepartures { get; set; }

        [Required(ErrorMessage = "*")]
        public int PaxPerDeparture { get; set; }

        public string Warning { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class QuotePaxRangeViewModel
    {
        [Required(ErrorMessage = "The QRF Id is required.")]
        public string QRFID { get; set; }

        [Required(ErrorMessage = "The Hotels is required.")]
        public string HotelsOption { get; set; }

        public List<ProductAttributeDetails> HotelCategoryList { get; set; }

        public List<ProductAttributeDetails> HotelChainList { get; set; }

        public List<QuotePaxSlabDetails> QuotePaxSlabDetails { get; set; }

        public QuotePaxRangeViewModel()
        {
            HotelCategoryList = new List<ProductAttributeDetails>();
            HotelChainList = new List<ProductAttributeDetails>();
            QuotePaxSlabDetails = new List<QuotePaxSlabDetails>();
        }
    }

    public class QuotePaxSlabDetails
    {
        public long PaxSlabId { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int PaxSlabFrom { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int PaxSlabTo { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int PaxSlabDividedBy { get; set; }

        [Required(ErrorMessage = "*")]
        public string TransportCategoryID { get; set; }
        public string TransportCategory { get; set; }
        public List<AttributeValues> TransportCategoryList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string TransportCoachTypeID { get; set; }
        public string TransportCoachType { get; set; }
        public List<AttributeValues> TransportCoachTypeList { get; set; } = new List<AttributeValues>();

        public string TransportBrandID { get; set; }
        public string TransportBrand { get; set; }
        public List<AttributeValues> TransportBrandList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int TransportCount { get; set; }
        
        //[Required(ErrorMessage = "*")]
        //[Range(0, int.MaxValue, ErrorMessage = "*")]
        public double? BudgetAmount { get; set; }
        public string BudgetCurrency { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class QuoteRoutingViewModel
    {
        public string QRFID { get; set; }
        public List<QuoteRoutingData> QuoteRoutingData { get; set; } 

        public string SubStep { get; set; }

        public string Status { get; set; }

        public bool IsRoutingExist { get; set; }

        public bool IsOverwriteExtPos { get; set; }

        public bool IsSetPrefHotels { get; set; }

        public List<ProductAttributeDetails> StarRatingList { get; set; }

        public QuoteRoutingViewModel()
        {
           // QRFId = 0;
            QuoteRoutingData = new List<QuoteRoutingData>();
        }
    }

    public class QuoteRoutingData
    {
        public long RouteID { get; set; }

        public int RouteSequence { get; set; }

        [Required(ErrorMessage = "*")]
        public string FromCityName { get; set; }
        public string FromCityID { get; set; }

        [Required(ErrorMessage = "*")]
        public string ToCityName { get; set; }
        public string ToCityID { get; set; }

        [Required(ErrorMessage = "*")]
        public int? Days { get; set; }

        [Required(ErrorMessage = "*")]
        public int? Nights { get; set; }

        [Required(ErrorMessage = "*")]
        public bool IsLocalGuide { get; set; }

        public string ErroMessage { get; set; }

        public string PrefStarRating { get; set; }
    }

    public class QuoteMarginViewModel
    {
        public string QRFID { get; set; }

        public string SelectedMargin { get; set; } = "Package"; 

        public string SubStep { get; set; }

        public string Status { get; set; }

        public PackageMarginDetails PackageMarginDetails { get; set; }

        public ProductMarginDetails ProductMarginDetails { get; set; }

        public ItemwiseMarginDetails ItemwiseMarginDetails { get; set; }

        public QuoteMarginViewModel()
        {
           // QRFId = 0;
            PackageMarginDetails = new PackageMarginDetails();
            ProductMarginDetails = new ProductMarginDetails();
            ItemwiseMarginDetails = new ItemwiseMarginDetails();
        }
    }

    public class PackageMarginDetails
    {
        public string Accommodation { get; set; }

        //public string AdditionalAccommodation { get; set; }

        public string IncPackageID { get; set; }

        public double IncSellingPrice { get; set; }

        public string IncMarginUnit { get; set; }

        public string ExcPackageID { get; set; }

        public double ExcSellingPrice { get; set; }

        public string ExcMarginUnit { get; set; }


        public bool SupSelected { get; set; }

        public string SupPackageID { get; set; }

        public double SupSellingPrice { get; set; }

        public string SupMarginUnit { get; set; }

        public bool OptSelected { get; set; }

        public string OptPackageID { get; set; }

        public double OptSellingPrice { get; set; }

        public string OptMarginUnit { get; set; }


        public string TotalCost { get; set; }

        public string TotalLeadersCost { get; set; }

        public string Upgrade { get; set; }

        public string MarkupType { get; set; }
    }

    public class ProductMarginDetails
    {
        public ProductMarginDetails()
        {
            ProductProperties = new List<ProductProperties>();
        }

        public List<ProductProperties> ProductProperties { get; set; }

        public string TotalCost { get; set; }

        public string TotalLeadersCost { get; set; }

        public string Upgrade { get; set; }

        public string MarkupType { get; set; }
    }

    public class ItemwiseMarginDetails
    {
        public ItemwiseMarginDetails()
        {
            ProductProperties = new List<ProductProperties>();
        }

        public List<ProductProperties> ProductProperties { get; set; }

        public string TotalCost { get; set; }

        public string TotalLeadersCost { get; set; }

        public string Upgrade { get; set; }

        public string MarkupType { get; set; }
    }

    public class ProductProperties
    {
        public string ProductID { get; set; }
        public string PositionID { get; set; }
        public string ProductName { get; set; }
        public string VoyagerProductType_Id { get; set; }
        public string Prodtype { get; set; }
        public bool IsProdtype { get; set; }
        public string MarginUnit { get; set; }

        public double SellingPrice { get; set; }

        public string HowMany { get; set; }
        public bool IsPosition { get; set; }
        public string ProductMaster { get; set; }
        public bool IsProduct { get; set; }
    }

    public class QuoteFOCViewModel
    {
        [Required(ErrorMessage = "The QRF Id is required.")]
        public string QRFID { get; set; }

        public bool StandardFoc { get; set; }

        public List<QuoteFOCDetails> QuoteFOCDetails { get; set; } = new List<QuoteFOCDetails>();
    }

    public class QuoteFOCDetails
    {
        public long FOCId { get; set; }
        public string DateRange { get; set; }
        public long DateRangeId { get; set; }
        public string PaxSlab { get; set; }
        public long PaxSlabId { get; set; }
        public int DivideByCost { get; set; }

        [Required(ErrorMessage = "*")]
        public int FOCSingle { get; set; }

        [Required(ErrorMessage = "*")]
        public int FOCTwin { get; set; }

        [Required(ErrorMessage = "*")]
        public int FOCDouble { get; set; }

        [Required(ErrorMessage = "*")]
        public int FOCTriple { get; set; }

        public bool IsDeleted { get; set; }
    }
}
