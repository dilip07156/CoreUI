using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class HotelsDeptViewModel
    {
        public string BookingNumber { get; set; }

        public HotelsDeptSearchFilters HotelsDeptSearchFilters { get; set; } = new HotelsDeptSearchFilters();
        public List<HotelSearchResult> HotelSearchResults { get; set; } = new List<HotelSearchResult>();
    }

    public class HotelsDeptSearchFilters
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string CNKReferenceNo { get; set; }
        public string AgentTour { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string DateType { get; set; }
        public List<AttributeValues> DateTypeList { get; set; } = new List<AttributeValues>();

        public string BookingStatus { get; set; }
        public List<Attributes> BookingStatusList { get; set; } = new List<Attributes>();
    }

    //public class HotelSearchResult
    //{
    //    public string BookingNumber { get; set; }
    //    public string AgentName { get; set; }
    //    public string ContactName { get; set; }
    //    public string BookingName { get; set; }
    //    public string StartDate { get; set; }
    //    public string EndDate { get; set; }
    //    public string Duration { get; set; }
    //    public string Status { get; set; }
    //    public List<TemplateBookingRoomsGrid> BookingRooms { get; set; }
    //}
}
