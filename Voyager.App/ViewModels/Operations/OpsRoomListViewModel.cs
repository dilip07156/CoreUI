using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels.Operations
{
    public class OpsRoomListViewModel
    {
        public string BookingNumber { get; set; }
        public COHeaderViewModel COHeader { get; set; } = new COHeaderViewModel();
        public List<PersonalDetailsViewModel> PassengerDetails { get; set; } = new List<PersonalDetailsViewModel>();
    }
}
