using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels
{
    public class PositionPricesViewModel
    {
        public string QRFID { get; set; }
        public string RowId { get; set; }
        public string Type { get; set; }
        public bool StandardPrice { get; set; }
        public List<PositionPricesData> PositionPricesData { get; set; }
        public bool IsClone { get; set; } = false;
        public bool IsSalesOfficeUser { get; set; }

        public PositionPricesViewModel()
        {
            PositionPricesData = new List<PositionPricesData>();
        }
    }

    public class PositionPricesData
    {
        public string PositionPriceId { get; set; }
        public string QRFID { get; set; }
        public string PositionId { get; set; }
        public long DepartureId { get; set; }
        public string Period { get; set; }
        public long PaxSlabId { get; set; }
        public string PaxSlab { get; set; }
        public string Type { get; set; }
        public string RoomId { get; set; }
        public bool IsSupplement { get; set; }
        public string SupplierId { get; set; }
        public string Supplier { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductRangeId { get; set; }
        public string ProductRange { get; set; }
        public string ProductRangeCode { get; set; }
        public string ProductType { get; set; }
        public string BuyCurrencyId { get; set; }
        public string BuyCurrency { get; set; }
        public double ContractPrice { get; set; }
        public string ContractId { get; set; }
        public double BudgetPrice { get; set; }
        public double BuyPrice { get; set; }
        public double MarkupAmount { get; set; }
        public double BuyNetPrice { get; set; }
        public string SellCurrencyId { get; set; }
        public string SellCurrency { get; set; }
        public double SellNetPrice { get; set; }
        public double TaxAmount { get; set; }
        public double SellPrice { get; set; }
        public string ExchangeRateId { get; set; }
        public double ExchangeRatio { get; set; }

        public string CreateUser { get; set; } = "";
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string EditUser { get; set; } = "";
        public DateTime? EditDate { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public bool IsContractExist { get; set; }
    }
}
