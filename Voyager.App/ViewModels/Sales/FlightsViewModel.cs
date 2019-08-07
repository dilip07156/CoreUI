using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class FlightsViewModel
    {
        public string QRFID { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<FlightDetails> FlightDetails { get; set; }
        public string VoyagerUserID { get; set; }

        public FlightsViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "Flights" };
            FlightDetails = new List<FlightDetails>();
        }
    }

    public class FlightDetails
    {
        public string PositionId { get; set; }

        [Required(ErrorMessage = "*")]
        public string DayID { get; set; }
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>();
        public string DayName { get; set; }

        [Required(ErrorMessage = "*")]
        public int Duration { get; set; }
        public List<AttributeValues> DurationList { get; set; } = new List<AttributeValues>();

        [Required(ErrorMessage = "*")]
        public string CityID { get; set; }
        [Required(ErrorMessage = "*")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "*")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string ProductName { get; set; }
        public string ProductID { get; set; }

        [Required(ErrorMessage = "*")]
        public string BudgetCategory { get; set; } = "";
        public List<ProdCategoryDetails> ProdCategoryList { get; set; } = new List<ProdCategoryDetails>();
        public string BudgetCategoryId { get; set; } = "";

        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();

        public string ProductType { get; set; }
        public string ProductTypeID { get; set; }

        public string SupplierID { get; set; }
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public int? NoOfPaxAdult { get; set; }
        public int? NoOfPaxChild { get; set; }
        public int? NoOfPaxInfant { get; set; }
        public string KeepAs { get; set; }

        public string OPSRemarks { get; set; }
        public string TLRemarks { get; set; }

        public bool IsDeleted { get; set; }
        public string CreateUser { get; set; }
        public string EditUser { get; set; }
        public int PositionSequence { get; set; }
    }
}
