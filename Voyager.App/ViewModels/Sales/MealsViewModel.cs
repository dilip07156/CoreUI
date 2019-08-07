using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class MealsViewModel
    {
        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<MealDetails> MealDetails { get; set; }

        public VenueDetails VenueDetails { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public MealsViewModel()
        {
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            MenuViewModel = new MenuViewModel();
            MealDetails = new List<MealDetails>();
            MenuViewModel.MenuName = "Meals";
        }
    }   

    public class VenueDetails
    {
        public string PositionId { get; set; } = "";

        public string QRFID { get; set; }

        public string DayID { get; set; } = "";

        public int PositionSequence { get; set; }

        public string MealType { get; set; } = "";

        public string CityID { get; set; } = "";

        public string CityName { get; set; } = "";

        public string ProductType { get; set; } = "";

        public string ProductTypeId { get; set; } = "";

        public bool? PlaceHolder { get; set; }

        public string ProductId { get; set; } = "";

        public string ProductName { get; set; } = "";

        [Required(ErrorMessage = "*")]
        public string TransferDetails { get; set; } = "";

        public string BudgetCategory { get; set; } = "";
        public List<ProdCategoryDetails> ProdCategoryList { get; set; } = new List<ProdCategoryDetails>();
        public string BudgetCategoryId { get; set; } = "";

        public List<MealProductRange> MealProductRange { get; set; } = new List<MealProductRange>();

        public List<RoomDetailsInfo> RoomDetailsInfo { get; set; } = new List<RoomDetailsInfo>();

        public string KeepAs { get; set; } = "";

        public string TLRemarks { get; set; } = "";

        public string OPSRemarks { get; set; } = "";

        public string UserName { get; set; } = "";

        public bool IsDeleted { get; set; }

        public string SupplierId { get; set; } = "";

        public string SupplierName { get; set; } = "";

        public ResponseStatus ResponseStatus { get; set; }

        public string RoutingDaysID { get; set; }

        public bool ApplyAcrossDays { get; set; }

        public List<PositionDetails> PositionDetails { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public string QuoteRoomPassanger { get; set; }

        public bool IsClone { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string VoyagerUserID { get; set; }
    }

    public class MealProductRange
    {
        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();
    }

    public class VenueGetParam
    {
        public string QRFID { get; set; }

        public string PositionId { get; set; }

        public string Day { get; set; }

        public string MealType { get; set; }
    }

}
