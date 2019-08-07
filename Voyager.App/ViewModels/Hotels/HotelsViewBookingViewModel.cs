using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class HotelsViewBookingViewModel
    {
        public string BookingNumber { get; set; }

        public Bookings Bookings { get; set; }
        //public List<mProducts_Lite> ProductSRPInfo { get; set; } = new List<mProducts_Lite>();
        public List<BookingProductsSRPInfo> SRPViewModelList { get; set; } = new List<BookingProductsSRPInfo>();
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();

        public string StatusName { get; set; }
        public List<Attributes> Status = new List<Attributes>();       
    }

    public class BookingProductsSRPInfo
    {
        public string PositionId { get; set; }
        public ProductsSRPViewModel ProductsSRPViewModel { get; set; } = new ProductsSRPViewModel();
    }
    //public class HotelsViewBookingDetails
    //{
    //    public string BookingNumber { get; set; }
    //    public string QRFID { get; set; }
    //    public DateTime? STARTDATE { get; set; }
    //    public DateTime? ENDDATE { get; set; }
    //    public string STATUS { get; set; }

    //    public BookingGuestDetails GuestDetails { get; set; }
    //    public List<Positions> Positions { get; set; }
    //}
}
