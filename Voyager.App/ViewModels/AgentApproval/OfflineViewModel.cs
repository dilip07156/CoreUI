using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class OfflineMessageViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public string UserName { get; set; }

        public string QRFID { get; set; }

        public string QRFPriceID { get; set; }

        public string Document_Id { get; set; }

        public string Status { get; set; } 

        public string SalesOfficerName { get; set; }

        public string SalesOfficerPhone { get; set; }           

        public string Remarks { get; set; }

        public string MailStatus { get; set; }

        public string ErrorMessage { get; set; }
    }
}