
using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class COHeaderViewModel
    {
        public string QRFID { get; set; }
        public string QRFPriceID { get; set; }
        public string AgentID { get; set; }
        public string AgentName { get; set; }
        public string TourName { get; set; }
        public string TourCode { get; set; }

        public string Destination { get; set; }
        public DateTime? TravelDate { get; set; }
        public string SalesPerson { get; set; }
        public string SalesPersonMobile { get; set; }
        public string ContactPersonID { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public int Pax { get; set; }
        public int Version { get; set; }

        public string CostingOfficer { get; set; }
        public string ProductAccountant { get; set; }
        public string ValidForTravel { get; set; }
        public string ValidForAcceptance { get; set; }

        public string SystemPhone { get; set; }
        public string SystemEmail { get; set; }
        public string SystemWebsite { get; set; }
        public string URLinitial { get; set; }

        public bool IsLinkedQRFsExist { get; set; }

        public string FollowUpCostingOfficer { get; set; }
        public string WithClient { get; set; }

        #region ops booking summary header fields
        public string BookingNumber { get; set; }
        public int NoOfDays { get; set; }
        public int NoOfNights { get; set; }
        public DateTime? EndDate { get; set; }
        public string FileHandler { get; set; }
        public string ModuleName { get; set; }
        #endregion
    }
}
