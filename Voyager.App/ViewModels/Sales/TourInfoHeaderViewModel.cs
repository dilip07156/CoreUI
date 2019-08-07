using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class TourInfoHeaderViewModel
    {
        public string QRFID { get; set; }
        public string AgentName { get; set; }
        public string TourName { get; set; }
        public string TourCode { get; set; }
        public int NoOfDays { get; set; }
        public int NoOfNights { get; set; }
        public bool IsLinkedQRFsExist { get; set; }
    }
}
