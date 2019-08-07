using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class QRFFocViewModel
    {
        public string QRFID { get; set; }
        public string RowId { get; set; }
        public string Type { get; set; }
        public List<QRFFocData> QRFFocData { get; set; }

        public QRFFocViewModel()
        {
            QRFFocData = new List<QRFFocData>();
        }
    }

    public class QRFFocData
    {
        public string QRFFocId { get; set; }
        public string Period { get; set; }
        public string PaxSlab { get; set; }
        public string Type { get; set; }
        public string City { get; set; }
        public string ProductName { get; set; }
        public string Service { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductRangeId { get; set; }

        public double FOCQty { get; set; }
        public string CreateUser { get; set; } = "";
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditUser { get; set; } = "";
        public DateTime? EditDate { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
    }
}
