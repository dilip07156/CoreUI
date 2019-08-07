using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels.Operations
{
    public class SupplierDetails
    {
        public string ProductType { get; set; }
        public string StartDate { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string Position_ID { get; set; }
        public bool IsSelected { get; set; }
        
    }
    public class DeterminePassCount
    {
        public string RoomName { get; set; }
        public string RoomCount { get; set; }

    }
}
