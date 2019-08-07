using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class ActivitiesViewModel
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public MenuViewModel MenuViewModel { get; set; } = new MenuViewModel { MenuName = "Activities" };

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<ActivityDetails> ActivityDetails { get; set; } = new List<ActivityDetails>();

        public List<QuickPickActivities> QuickPickActivities { get; set; } = new List<QuickPickActivities>();
    }

    public class ActivityDetails
    {
        public Guid ActivityId { get; set; }

        [Required(ErrorMessage = "*")]
        public string ActivityDayNo { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>(); 
        public string DayName { get; set; }

        [Required(ErrorMessage = "*")]
        public string CityID { get; set; }
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string EndTime { get; set; }

        //[Required(ErrorMessage = "*")]
        public string TypeOfExcursionID { get; set; }
        public string TypeOfExcursion { get; set; }
        public List<AttributeValues> TypeOfExcursionList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeID { get; set; }

        [Required(ErrorMessage = "*")]
        public string TourTypeID { get; set; }
        public string TourType { get; set; }
        public List<AttributeValues> TourTypeList { get; set; } = new List<AttributeValues>();

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

    public class ProdTransferDetails
    {
        public string TransferDetails { get; set; } = "";
        public int Index { get; set; } = 0;
    }

    //public class QuickPickActivities
    //{
    //    public string CityName { get; set; }
    //    public List<QuickPickProductDetails> QuickPickProductDetails { get; set; } = new List<QuickPickProductDetails>();
    //}

    public class QuickPickActivities
    {
        public int QPSeqNo { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public bool IsSelected { get; set; }
        public string PositionID { get; set; }
        public string ProdID { get; set; }
        public string ProdCode { get; set; }
        public string ProdName { get; set; }
        public string ProdType { get; set; }
        public string ProdTypeID { get; set; }
        public string ProdCategory { get; set; }
        public string ProdCategoryID { get; set; }
        public string StartTime { get; set; }
        public string DayName { get; set; }
        public string ActivityDayNo { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public List<AttributeValues> ProdCategoryList { get; set; } = new List<AttributeValues>();
        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();
    }
}
