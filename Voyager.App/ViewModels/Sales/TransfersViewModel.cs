using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VGER_WAPI_CLASSES;
using System;

namespace Voyager.App.ViewModels
{
    public class TransfersViewModel
    {
        public string QRFID { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<TransferDetails> TransferDetails { get; set; } 

        public TransferPopup TransferPopup { get; set; }

        public string SaveType { get; set; }

        public string Price { get; set; }

        public string FOC { get; set; }
        public string VoyagerUserId { get; set; }

        public TransfersViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "Transfers" };
            TransferDetails = new List<TransferDetails>(); 
            TransferPopup = new TransferPopup();
        }
    } 

    public class TransferPopup
    {
        public List<AttributeValues> DayList { get; set; } = new List<AttributeValues>();

        public TransferProperties TransferProperties { get; set; } 

        public string ProductType { get; set; }
        public string ProductTypeID { get; set; }
         
        public string FromCity { get; set; }
        public string FromCityID { get; set; }
         
        public string FromPickUpLoc { get; set; }
        public string FromPickUpLocID { get; set; }
         
        public string StartTime { get; set; }
         
        public string ToCity { get; set; }
        public string ToCityID { get; set; }
         
        public string ToDropOffLoc { get; set; }
        public string ToDropOffLocID { get; set; }
         
        public string EndTime { get; set; }
         
        public string ProductName { get; set; }
        public string ProductID { get; set; }
         
        public string BudgetCategory { get; set; } = "";
        public List<ProdCategoryDetails> ProdCategoryList { get; set; } = new List<ProdCategoryDetails>();
        public string BudgetCategoryId { get; set; } = "";

        public List<RoomDetailsInfo> RoomDetailsInfo { get; set; } = new List<RoomDetailsInfo>();

        public List<TransferProductRange> TransferProductRange { get; set; } = new List<TransferProductRange>();

        public int Duration { get; set; }
        public List<AttributeValues> DurationList { get; set; }
         
        public string KeepAs { get; set; }

        public string OPSRemarks { get; set; }

        public string TLRemarks { get; set; }
               
        public string SupplierID { get; set; }

        public string SupplierName { get; set; }        
    } 

    public class TransferProductRange {
        public PositionRoomsViewModel PositionRoomsData { get; set; } = new PositionRoomsViewModel();
    }
}
