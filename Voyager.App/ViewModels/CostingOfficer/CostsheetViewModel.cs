using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class CostsheetViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public ConsolidatedSummaryViewModel ConsolidatedSummaryViewModel { get; set; }

        public string Type { get; set; }

        public CostsheetViewModel()
        {
            COHeaderViewModel = new COHeaderViewModel();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            MenuViewModel = new MenuViewModel();
            ConsolidatedSummaryViewModel = new ConsolidatedSummaryViewModel();
        }
    }

    public class ConsolidatedSummaryViewModel
    {
        public string DepartureDate { get; set; }
        public CostsheetVersion CostsheetVersion { get; set; }
        public List<AttributeValues> DepartureDatesList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFPackagePriceList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFPackagePriceDataList { get; set; }
        public List<QRFPkgAndNonPkgPrice> HeaderList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPackagePriceSupplementList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPackagePriceOptionalList { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgSupplementPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgOptionalPositions { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgPriceSupplementPaxSlabs { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgPriceOptionalPaxSlabs { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFPkgPositionTotalCost { get; set; }
        public List<QRFPkgAndNonPkgPrice> QRFNonPkgPositionTotalCost { get; set; }

        public ConsolidatedSummaryViewModel()
        {
            QRFPackagePriceDataList = new List<QRFPkgAndNonPkgPrice>();
            QRFPackagePriceList = new List<QRFPkgAndNonPkgPrice>();
            HeaderList = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPackagePriceSupplementList = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPackagePriceOptionalList = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgSupplementPositions = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgOptionalPositions = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgPriceSupplementPaxSlabs = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgPriceOptionalPaxSlabs = new List<QRFPkgAndNonPkgPrice>();
            QRFPkgPositionTotalCost = new List<QRFPkgAndNonPkgPrice>();
            QRFNonPkgPositionTotalCost = new List<QRFPkgAndNonPkgPrice>();
        }
    }

    public class QRFPkgAndNonPkgPrice
    {
        public int SequenceNo { get; set; }
        public int Age { get; set; }
        public long PaxSlabId { get; set; }
        public string PaxSlab { get; set; }
        public string RoomName { get; set; }
        public string DisplayRoomName { get; set; }
        public string ProductName { get; set; }
        public string QRFCurrency { get; set; }
        public double SellPrice { get; set; }
        public string PositionId { get; set; }
        public string PositionKeepAs { get; set; }
        public string PositionType { get; set; }
        public string Type { get; set; }
        public long DepartureId { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string focValue { get; set; }
    }
    
    public class DetailedInfoViewModel
    {
        public string DepartureDate { get; set; }
        public List<AttributeValues> DepartureDatesList { get; set; }

        public string PaxSlab { get; set; }
        public List<AttributeValues> PaxSlabList { get; set; }

        public string ProductName { get; set; }
        public List<AttributeValues> ProductNameList { get; set; }

        public string Supplier { get; set; }
        public List<AttributeValues> SupplierList { get; set; }
               
        public List<ProductDetails> ProductTypeList { get; set; }
                
        public DetailedInfoViewModel()
        {
            ProductTypeList = new List<ProductDetails>();
        }
    }

    public class ProductDetails
    {
        //public string ProductID { get; set; }
        //public string PositionID { get; set; }
        //public string ProductName { get; set; }
        //public string VoyagerProductType_Id { get; set; }
        public string Prodtype { get; set; }
        public bool IsProdtype { get; set; }
        //public string MarginUnit { get; set; }
        //public double SellingPrice { get; set; }
        //public string HowMany { get; set; }
        //public bool IsPosition { get; set; }
    }
}
