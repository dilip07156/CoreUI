using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.ViewModels.Operations;

namespace Voyager.App.ViewModels
{
   public class BookingMaterializationVm
    {
        public BookingMaterializationVm()
        {
            roomMaterializationDetails = new List<RoomMaterializationDetails>();
            personMaterializationDetails = new List<PersonMaterializationDetails>();
            Response = new ResponseStatus();
            RoomsInCostExceptInMaterialization = new List<string>();
            SupplierDetails = new List<SupplierDetails>();
        }
        public string bookingNumber { get; set; }
        public List<RoomMaterializationDetails> roomMaterializationDetails { get; set; }
        public List<PersonMaterializationDetails> personMaterializationDetails { get; set; }
        public ResponseStatus Response { get; set; }
        public List<String> RoomsInCostExceptInMaterialization { get; set; }
        public List<SupplierDetails> SupplierDetails { get; set; }
    }
    public class RoomMaterializationDetails
    {
        public string RoomType { get; set; }
        public string For { get; set; }
        public int? Age { get; set; }
        public int Current { get; set; }
        public int? NewLevel { get; set; }
        public string BookingRooms_Id { get; set; }
        
    }
    public class PersonMaterializationDetails
    {
        public string PassengerType { get; set; }
        public string PassengerAge { get; set; }
        public int? PassengerQty { get; set; }
        public string BookingPax_ID { get; set; }
    }
}
