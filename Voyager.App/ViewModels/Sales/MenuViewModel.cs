using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class MenuViewModel
    {
        public string QRFID { get; set; }

        public string BookingNumber { get; set; }

        public string PositionId { get; set; } 

        public string MenuName { get; set; }

        public string EnquiryPipeline { get; set; }

        public bool IsClone { get; set; } 
    }
}
