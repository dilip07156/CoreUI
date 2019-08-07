using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class PositionFOCViewModel
    {
        public string QRFID { get; set; }
        public bool StandardFoc { get; set; }
        public List<PositionFOCData> PositionFOCData { get; set; }
        public bool IsClone { get; set; } = false;

        public PositionFOCViewModel()
        {
            PositionFOCData = new List<PositionFOCData>();
        }
    }

    public class PositionFOCData
    {
        public string PositionFOCId { get; set; }
        public string QRFID { get; set; }
        public string PositionId { get; set; }
        public long DepartureId { get; set; }
        public string Period { get; set; }
        public long PaxSlabId { get; set; }
        public string PaxSlab { get; set; }
        public string Type { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string RoomId { get; set; }
        public bool IsSupplement { get; set; }
        public string SupplierId { get; set; }
        public string Supplier { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductRangeId { get; set; }
        public string ProductRange { get; set; }
        public string ContractId { get; set; }
        public int Quantity { get; set; }
        public int FOCQty { get; set; }

        public string CreateUser { get; set; } = "";
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditUser { get; set; } = "";
        public DateTime? EditDate { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
    }
}
