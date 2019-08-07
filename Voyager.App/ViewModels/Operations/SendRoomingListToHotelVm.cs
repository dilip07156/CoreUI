using System.Collections.Generic;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels.Operations
{
    public class SendRoomingListToHotelVmList
    {
        public SendRoomingListToHotelVmList()
        {
            HotelList = new List<SendRoomingListToHotelVm>();
            response = new ResponseStatus();

        }
        public string BookingNumber { get; set; }
        public List<SendRoomingListToHotelVm> HotelList { get; set; }
        public ResponseStatus response { get; set; }

    } 
}
