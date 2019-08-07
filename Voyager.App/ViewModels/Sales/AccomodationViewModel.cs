using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class AccomodationViewModel
    {
        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<AccomodationData> AccomodationData { get; set; }

        public string SaveType { get; set; } = "full";

        public string Price { get; set; }

        public string FOC { get; set; }

        public AccomodationViewModel()
        {
            MenuViewModel = new MenuViewModel();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            AccomodationData = new List<AccomodationData>();
            MenuViewModel.MenuName = "Accomodation";
        }
    }

    public class AccomodationData
    {
        public string AccomodationId { get; set; }
        public int AccomodationSequence { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeId { get; set; }

        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }
        public string CityID { get; set; }

        public int DayNo { get; set; }
        public string DayName { get; set; }
        public string StartingFrom { get; set; }
        public List<AttributeValues> StartingFromList { get; set; }

        public int NoOfNight { get; set; }
        public List<AttributeValues> NoOfNightList { get; set; }

        public string Category { get; set; }
        public List<ProdCatDefProperties> CategoryList { get; set; }

        public string StarRating { get; set; }
        public List<ProductAttributeDetails> StarRatingList { get; set; }

        public string Location { get; set; }
        public List<ProductAttributeDetails> LocationList { get; set; }

        public string ChainName { get; set; }
        public string ChainID { get; set; }

        public string HotelName { get; set; }
        public string HotelID { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        //public List<RoomDetails> RoomDetails { get; set; }

        public PositionRoomsViewModel PositionRoomsData { get; set; }

        //Requirements
        public string MealPlan { get; set; }
        public List<ProductAttributeDetails> MealPlanList { get; set; }

        public string EarlyCheckInDate { get; set; }

        //[RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string EarlyCheckInTime { get; set; }

        public int? NumberOfEarlyCheckInRooms { get; set; }

        public int? NumberofInterConnectingRooms { get; set; }
        public int? NumberOfWashChangeRooms { get; set; }

        public string LateCheckOutDate { get; set; }

        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid Time.")]
        public string LateCheckOutTime { get; set; }

        public int? NumberOfLateCheckOutRooms { get; set; }

        //Requirements End

        [Required(ErrorMessage = "*")]
        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; }

        public string RemarksForTL { get; set; }

        public string RemarksForOPS { get; set; }

        public string StarRatingDef { get; set; }
        public string StarRatingIdDef { get; set; }
        public string LocationIdDef { get; set; }
        public string LocationDef { get; set; }
        public string CategoryIdDef { get; set; }
        public string CategoryDef { get; set; }
        public string ChainIdDef { get; set; }
        public string ChainDef { get; set; }

        public string EndTime { get; set; }
        public string StartTime { get; set; }

        public AccomodationData()
        {
            StartingFromList = new List<AttributeValues>();
            NoOfNightList = new List<AttributeValues>();
            CategoryList = new List<ProdCatDefProperties>();
            StarRatingList = new List<ProductAttributeDetails>();
            LocationList = new List<ProductAttributeDetails>();
            //RoomDetails = new List<RoomDetails>();
            MealPlanList = new List<ProductAttributeDetails>();
            KeepAsList = new List<AttributeValues>();
            PositionRoomsData = new PositionRoomsViewModel();
        }
    }

    public class PositionRoomsViewModel
    {
        public string PositionId { get; set; }
        public int RowNo { get; set; }
        public string PositionType { get; set; }
        public List<RoomDetails> RoomDetails { get; set; }

        public string SupplementID { get; set; }
        public string Supplement { get; set; }
        public List<ProductAttributeDetails> SupplementList { get; set; }

        public PositionRoomsViewModel()
        {
            RoomDetails = new List<RoomDetails>();
            SupplementList = new List<ProductAttributeDetails>();
        }
    }

    public class RoomDetails
    {
        public string RoomId { get; set; }
        public int RoomSequence { get; set; }

        [Required(ErrorMessage = "*")]
        public string ProductRangeID { get; set; }
        public string ProductRange { get; set; }
        public List<ProductAttributeDetails> RoomTypeList { get; set; }

        public bool IsSupplement { get; set; }

        public RoomDetails()
        {
            RoomTypeList = new List<ProductAttributeDetails>();
        }

    } 
}
