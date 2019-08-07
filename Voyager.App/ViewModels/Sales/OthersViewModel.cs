using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;
using System;

namespace Voyager.App.ViewModels
{
    public class OthersViewModel
    {
        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public OthersLocalGuide OthersLocalGuide { get; set; }

        public OthersVisa OthersVisa { get; set; }

        public OthersMiscellaneous OthersMiscellaneous { get; set; }

        public OthersViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "Others" };
            OthersLocalGuide = new OthersLocalGuide();
            OthersVisa = new OthersVisa();
            OthersMiscellaneous = new OthersMiscellaneous();
        }
    }

    public class OthersLocalGuide
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public List<LocalGuideDetails> LocalGuideDetails { get; set; } = new List<LocalGuideDetails>();

        public bool IsClone { get; set; }
    }

    public class LocalGuideDetails
    {
        public Guid OthersId { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayNo { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>(); 
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
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();

        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; } = new List<AttributeValues>();

        public string RemarksTL { get; set; }
        public string RemarksOPS { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
    }
    
    public class OthersVisa
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public List<VisaDetails> VisaDetails = new List<VisaDetails>();
    }

    public class VisaDetails
    {
        public long OthersId { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayNo { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string CityID { get; set; }
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        public string VisaType { get; set; }
        
        [Required(ErrorMessage = "*")]
        public string Durations { get; set; }
        public string DurationsDays { get; set; }

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
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Adult count is required")]
        [Display(Name = "Adult")]
        public int ApproxPaxAdult { get; set; }
        public int ApproxPaxChild { get; set; }
        

        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; } = new List<AttributeValues>();

        public string RemarksTL { get; set; }
        public string RemarksOPS { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class OthersMiscellaneous
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public List<MiscellaneousDetails> MiscellaneousDetails = new List<MiscellaneousDetails>();
    }

    public class MiscellaneousDetails
    {
        public long OthersId { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayNo { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string CityID { get; set; }
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        public string VisaType { get; set; }

        [Required(ErrorMessage = "*")]
        public string Durations { get; set; }
        public string DurationsDays { get; set; }

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
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Adult count is required")]
        [Display(Name = "Adult")]
        public int ApproxPaxAdult { get; set; }
        public int ApproxPaxChild { get; set; }


        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; } = new List<AttributeValues>();

        public string RemarksTL { get; set; }
        public string RemarksOPS { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class TourGuideDefaultRows {
        public string DayNo { get; set; }
        public string DayName { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
    }
}
