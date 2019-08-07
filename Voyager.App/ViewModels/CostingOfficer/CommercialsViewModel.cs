using System.Collections.Generic;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class CommercialsViewModel
    {
        public string Remarks { get; set; }

        public string CurrentDate { get; set; }

        public string UserName { get; set; }

        public COHeaderViewModel COHeaderViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }
        
        public long PaxSlab { get; set; }
        public List<AttributeValues> PaxSlabList { get; set; }

        public long DepartureDate { get; set; }
        public List<AttributeValues> DepartureDatesList { get; set; }

        public List<CommercialsData> BareBoneList { get; set; }

        public List<mQRFPositionTotalCost> PositionIncluded { get; set; }
        public List<mQRFPositionTotalCost> PositionSupplement { get; set; }
        public List<mQRFPositionTotalCost> PositionOptional { get; set; }
        public List<QRFExchangeRates> QRFExhangeRates { get; set; }

        public string QRFID { get; set; }
        public string QRFPriceId { get; set; }
        public double PercentSoldOptional { get; set; }

        public string SalesOfficer { get; set; }
        public string CostingOfficer { get; set; }
        public string ProductAccountant { get; set; }

        public CommercialsViewModel()
        {
            MenuViewModel = new MenuViewModel();
            COHeaderViewModel = new COHeaderViewModel();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            BareBoneList = new List<CommercialsData>();
            PositionIncluded = new List<mQRFPositionTotalCost>();
            PositionSupplement = new List<mQRFPositionTotalCost>();
            PositionOptional = new List<mQRFPositionTotalCost>();
            QRFExhangeRates = new List<QRFExchangeRates>();
        }

    }
}
