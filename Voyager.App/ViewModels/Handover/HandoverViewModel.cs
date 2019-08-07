using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class HandoverViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();

        public MenuViewModel MenuViewModel { get; set; } = new MenuViewModel();

        public ResponseStatus ResponseStatus { get; set; } = new ResponseStatus();

        public List<DepatureInfo> DepatureInfo { get; set; }
    }

    public class DepatureInfo
    {
        public long DepatureId { get; set; }
        public DateTime? DepatureDate { get; set; }
        public bool IsCreate { get; set; }
        public bool IsMaterialised { get; set; }
        public bool IsShowProcessing { get; set; }
        public bool IsShowPending { get; set; }
        public bool IsDeleted { get; set; }
        public bool ConfirmStatus { get; set; }
        public string ConfirmMessage { get; set; }
        public string OpsBookingNumber { get; set; }
        public DateTime? Confirmed_Date { get; set; }
        public string Confirmed_User { get; set; }
    }

    public class DepatureInfoGetReq
    {
        public string QRFID { get; set; }
        public List<long> DepatureId { get; set; }
    }
}
