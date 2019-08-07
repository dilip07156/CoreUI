using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;
using System.ComponentModel.DataAnnotations;

namespace Voyager.App.ViewModels
{
    public class OpsProdTypeVM
    {
        public string BookingNumber { get; set; }
        public string ProductType { get; set; }
        public string PositionStatus { get; set; }
        public string Position_Id { get; set; }
        public string ProductId { get; set; }
        public string DayName { get; set; }
        public string ChargeBasis { get; set; }
        public bool IsRealSupplier { get; set; }
        public string SystemCompany_Id { get; set; }
        public OpsProdTypeDaysMenuVM OpsProdTypeDaysMenuVM { get; set; } = new OpsProdTypeDaysMenuVM();
        public ProdTypePositions ProdTypePositions { get; set; }
        //public List<ScheduleDetails> ScheduleDetailsList { get; set; } = new List<ScheduleDetails>();
        public List<ScheduleDetails> TimeSlabsList { get; set; } = new List<ScheduleDetails>();
    }

    public class OpsProdTypeDaysMenuVM
    {
        public string DayName { get; set; }
        public List<AttributeValues> DaysList { get; set; }
    }

    //public class ProdTypeModel
    //{
    //    public List<AttributeValues> DaysList { get; set; }
    //    public List<ProdTypePositions> ProdTypePositions { get; set; }
    //}

    public class ProdTypePositions
    {
        public ProdTypePositions()
        {
            SuppFOC = new OpsPosFOCViewModel();
            RoomsAndRates = new OpsPosRoomsPricesViewModel();
            Financials = new OpsPosFinancials();
        }

        public ProductsSRPViewModel ProductsSRP { get; set; }
        public OpsPositionAdditionalInfo ProductAdditionalInfo { get; set; }
        public OpsPosRoomsPricesViewModel RoomsAndRates { get; set; }
        public OpsPosFOCViewModel SuppFOC { get; set; }
        public ProductsSRPViewModel CommTrail { get; set; }
        public ProductsSRPViewModel AlternateServices { get; set; }
        public OpsPosFinancials Financials { get; set; }
    }

    public class OpsPosFOCViewModel
    {
        public OpsPosFOCViewModel()
        {
            lstBookingRooms = new List<ProductRangeDetails>();
            lstPosFOC = new List<OpsPositionFOC>();
        }

        public string Booking_Id { get; set; }
        public string BookingNo { get; set; }
        public string Position_Id { get; set; }
        public List<ProductRangeDetails> lstBookingRooms { get; set; }
        //public List<ProductRangeDetails> lstGetBookingRooms { get; set; }
        public List<OpsPositionFOC> lstPosFOC { get; set; }
    }

    public class OpsPosRoomsPricesViewModel
    {
        public OpsPosRoomsPricesViewModel()
        {
            BookingRoomsAndPrices = new List<OpsPositionRoomPrice>();
            BookingRoomsAndPricesSupp = new List<OpsPositionRoomPrice>();
            BudgetSupplements = new List<OpsBudgetSupplements>();
            RangeList = new List<ProductRangeDetails>();
            RangeListSupp = new List<ProductRangeDetails>();
            //ChargeBasisList = new List<DefChargeBasis>();
            PersonTypeList = new List<mDefPersonType>();
        }

        public string Booking_Id { get; set; }
        public string BookingNo { get; set; }
        public string Position_Id { get; set; }
        public List<ProductRangeDetails> RangeList { get; set; }
        public List<ProductRangeDetails> RangeListSupp { get; set; }
        //public List<DefChargeBasis> ChargeBasisList { get; set; }
        public List<mDefPersonType> PersonTypeList { get; set; }
        public List<ProductRangeDetails> RangeListBudgSupp { get; set; }

        public List<OpsPositionRoomPrice> BookingRoomsAndPrices { get; set; }
        public List<OpsPositionRoomPrice> BookingRoomsAndPricesSupp { get; set; }
        public List<OpsBudgetSupplements> BudgetSupplements { get; set; }

    }

    public class OpsPosFinancials
    {
        public OpsPosFinancials()
        {
            FinancialDetails = new List<FinancialDetail>();
        }
        public List<FinancialDetail> FinancialDetails { get; set; }
        public string TotalBuyPrice { get; set; }
        public string TotalBuyCurrency { get; set; }
        public string TotalSellPrice { get; set; }
        public string TotalSellCurrency { get; set; }
        public string TotalGPPercent { get; set; }
        public string TotalGPAmount { get; set; }
    }

    public class OpsPositionAdditionalInfo
    {
        #region Common Attributes

        public string ProdType { get; set; }

        [Required(ErrorMessage = "*")]
        public string FileHandlerID { get; set; }
        public string FileHandler { get; set; }
        public List<AttributeValues> FileHandlerList { get; set; } = new List<AttributeValues>();
        [Required(ErrorMessage = "*")]
        public string PositionType { get; set; }
        public List<string> PositionTypeList { get; set; } = new List<string> { "Core", "Optional", "Post", "Pre", "Supplement" };
        [Required(ErrorMessage = "*")]
        public string PaxType { get; set; }
        public List<AttributeValues> PaxTypeList { get; set; } = new List<AttributeValues> { new AttributeValues { AttributeValue_Id = "true", Value = "YES" }, new AttributeValues { AttributeValue_Id = "false", Value = "NO" } };

        [Required(ErrorMessage = "*")]
        public string StartDate { get; set; }
        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }
        //[Required(ErrorMessage = "*")]
        public string StartLocation { get; set; }
        public List<AttributeValues> LocationList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string EndDate { get; set; }
        [Required(ErrorMessage = "*")]
        public string EndTime { get; set; }
        //[Required(ErrorMessage = "*")]
        public string EndLocation { get; set; }

        //[Required(ErrorMessage = "*")]
        public string OptionDate { get; set; }
        //[Required(ErrorMessage = "*")]
        public string ConfirmDate { get; set; }
        //[Required(ErrorMessage = "*")]
        public string CancellationDate { get; set; }
        #endregion

        #region Hotel Attributes
        [Required(ErrorMessage = "*")]
        public string BoardBasis { get; set; }
        public List<AttributeValues> BoardBasisList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string BreakfastType { get; set; }
        public List<AttributeValues> BreakfastTypeList { get; set; } = new List<AttributeValues>();

        public string Porterage { get; set; }
        //public List<string> PorterageList { get; set; } = new List<string> { "YES", "NO" };

        public string VoucherNote { get; set; }
        public string CancellationPolicy { get; set; }
        public string CityTaxAdvise { get; set; }
        public string TLRemarks { get; set; }
        public string OpsRemarks { get; set; }
        #endregion

        #region Attraction Attributes
        public string TicketLocation { get; set; }
        public List<AttributeValues> TicketLocationList { get; set; } = new List<AttributeValues>();

        public string TrainNumber { get; set; }
        public bool GuidePurchaseTicket { get; set; }
        public string Itinerary { get; set; }
        //public string AttrCancellationPolicy { get; set; }
        #endregion

        #region Bus Attributes
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string DriverLanguages { get; set; }
        public string DriverLicenceNumber { get; set; }
        public string NumberofDriverRooms { get; set; }
        public string TypeofRoom { get; set; }
        public bool MealsIncluded { get; set; }
        public string VehicleRegistration { get; set; }
        public string ManufacturedDate { get; set; }
        public string SafetyCertificateDate { get; set; }
        public bool Parking { get; set; }
        public bool CityPermit { get; set; }
        public bool RoadTolls { get; set; }
        public bool AC { get; set; }
        public bool WC { get; set; }
        public bool Safety { get; set; }
        public bool GPS { get; set; }
        public bool AV { get; set; }
        //public string BusItinerary { get; set; }
        //public string BusCancellationPolicy { get; set; }
        public List<AttributeValues> TypeofRoomList { get; set; } = new List<AttributeValues>();
        #endregion

        #region Guide Attributes
        public string GuideName { get; set; }
        public string GuideContact { get; set; }
        //public string NumberofGuideRooms { get; set; }
        //public string GuideTypeofRoom { get; set; }
        //public bool GuideMeals { get; set; }
        public bool GuideTickets { get; set; }
        //public string GuideItinerary { get; set; }
        //public string GuideCancellationPolicy { get; set; }
        #endregion

        #region Meal Attributes
        public string Floor { get; set; }
        public string CoachParking { get; set; }
        public string MealStyle { get; set; }
        public string Courses { get; set; }
        public bool Tea { get; set; }
        public bool Dessert { get; set; }
        public bool Water { get; set; }
        public bool Bread { get; set; }
        public string MealMenu { get; set; }
        //public string MealCancellationPolicy { get; set; }

        public List<AttributeValues> FloorList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> MealStyleList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> MealCoursesList { get; set; } = new List<AttributeValues>();
        #endregion

        #region Flight Attributes
        public string FlightNumber { get; set; }
        #endregion
    }

    public class OpsPositionDetailsViewModel
    {
        public string BookingNumber { get; set; }
        public string PositionId { get; set; }
        public string UpdateUser { get; set; }
        public Dictionary<string, string> PositionDetails = new Dictionary<string, string>();
        //Dictionary<string, string>
    }

    //public class TimeSlabs
    //{
    //    public string PositionId { get; set; }
    //    public string ProductName { get; set; }
    //    public string StartTime { get; set; }
    //    public string EndTime { get; set; }
    //}
}
