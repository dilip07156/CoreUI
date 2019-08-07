using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;
using System;

namespace Voyager.App.ViewModels
{
    public class BusViewModel
    {
        public string QRFID { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<BusDetails> BusDetails { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public string LocationId { get; set; }
        public string Location { get; set; }
        public string VoyagerUserId { get; set; }

        public BusViewModel()
        {
            //QRFId = 0;
            MenuViewModel = new MenuViewModel { MenuName = "Bus" };
            BusDetails = new List<BusDetails>();
        }
    }

    public class BusDetails
    {
        public string BusID { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayID { get; set; }
        public string DayName { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string ProductType { get; set; }
        public string ProductTypeID { get; set; }

        [Required(ErrorMessage = "*")]
        public string FromCity { get; set; }
        public string FromCityID { get; set; }

        [Required(ErrorMessage = "*")]
        public string FromPickUpLoc { get; set; }
        public string FromPickUpLocID { get; set; }

        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string ToCity { get; set; }
        public string ToCityID { get; set; }

        [Required(ErrorMessage = "*")]
        public string ToDropOffLoc { get; set; }
        public string ToDropOffLocID { get; set; }

        [Required(ErrorMessage = "*")]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string ProductName { get; set; }
        public string ProductID { get; set; }

        [Required(ErrorMessage = "*")]
        public string BudgetCategory { get; set; } = "";
        public List<ProdCategoryDetails> ProdCategoryList { get; set; } = new List<ProdCategoryDetails>();
        public string BudgetCategoryId { get; set; } = "";

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();

        [Required(ErrorMessage = "*")]
        public int Duration { get; set; }
        public List<AttributeValues> DurationList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string KeepAs { get; set; }

        public bool IsCityPermit { get; set; }

        public bool IsParkingCharges { get; set; }

        public bool IsRoadTolls { get; set; }

        public string OPSRemarks { get; set; }
        public string TLRemarks { get; set; }

        public bool IsDeleted { get; set; }
        public string CreateUser { get; set; }
        public string EditUser { get; set; }
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int BusSequence { get; set; }
        public string ForPositionId { get; set; }
    }

    public class PositionGetParam
    {
        public string QRFID { get; set; }

        public string PositionId { get; set; }

        public string Day { get; set; }
    }

}
