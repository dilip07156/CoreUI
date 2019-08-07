using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;
using System;

namespace Voyager.App.ViewModels
{
    public class RailViewModel
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public string ProductAttributeType { get; set; } = "Oneway";

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<RailDetails> RailDetails { get; set; }
        

        public RailViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "Rail" };
            RailDetails = new List<RailDetails>();
        }
    }

    public class RailDetails
    {
        public Guid RailId { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayNo { get; set; }
        public List<AttributeValues> DaysList { get; set; } = new List<AttributeValues>(); 
        public string DayName { get; set; }

        [Required(ErrorMessage = "*")]
        public string CityID { get; set; }
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "*")]
        public int Duration { get; set; }
        public List<AttributeValues> DurationList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeID { get; set; }

        [Required(ErrorMessage = "*")]
        public string ProductCategoryID { get; set; }
        public string ProductCategory { get; set; }
        public List<AttributeValues> ProductCategoryList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string TransferDetails { get; set; }

        [Required(ErrorMessage = "*")]
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();


        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int NoOfPaxAdult { get; set; }
        public int NoOfPaxChild { get; set; }
        public int NoOfPaxInfant { get; set; }
        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; } = new List<AttributeValues>();

        public string RemarksTL { get; set; }
        public string RemarksOPS { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
