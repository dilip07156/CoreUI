using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class SupplierViewModel
	{
        public string CompanyId { get; set; }
        public List<SupplierProducts> SupplierProducts { get; set; }

        public SupplierViewModel()
        {
            SupplierProducts = new List<SupplierProducts>();
        }
    }

    public class SupplierProducts
    {
        public string ProductSupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Default { get; set; }
        public string Preferred { get; set; }
        public int Usage { get; set; }
        public string ProductId { get; set; }
        public string ProductType { get; set; }
    }

    public class ProductViewModel
    {
        public string CompanyId { get; set; }
        public string ProductSupplierId { get; set; }
        public string Status { get; set; }
        public List<Attributes> StatusList { get; set; }

        public string Currency_Id { get; set; }
        public string Currency { get; set; }
        public List<Currency> CurrencyList { get; set; }

        public bool IsPreferred { get; set; }
        public bool IsDefault { get; set; }
        public string ActiveFrom { get; set; }
        public string ActiveTo { get; set; }

        public string ContactSalesId { get; set; }
        public string ContactSalesName { get; set; }
        public string ContactSalesEmail { get; set; }
        public List<Contact> lstSales { get; set; }

        public string ContactFitId { get; set; }
        public string ContactFitName { get; set; }
        public string ContactFitEmail { get; set; }
        public List<Contact> lstFit { get; set; }

        public string ContactGroupId { get; set; }
        public string ContactGroupName { get; set; }
        public string ContactGroupEmail { get; set; }
        public List<Contact> lstGroup { get; set; }

        public string ContactFinanceId { get; set; }
        public string ContactFinanceName { get; set; }
        public string ContactFinanceEmail { get; set; }
        public List<Contact> lstFinance { get; set; }

        public string ContactEmergencyId { get; set; }
        public string ContactEmergencyName { get; set; }
        public string ContactEmergencyEmail { get; set; }
        public List<Contact> lstEmergency { get; set; }

        public string ContactComplaintId { get; set; }
        public string ContactComplaintName { get; set; }
        public string ContactComplaintEmail { get; set; }
        public List<Contact> lstComplaint { get; set; }

        public ProductsSRPViewModel ProductSRPViewModel { get; set; }

        public bool IsAllSalesMkts { get; set; }
        public List<ProductSupplierSalesMarket> SelectedSalesMarket { get; set; }
        public List<ProductSupplierSalesMarket> selectedlstSalesMarket { get; set; }
        public List<ProductSupplierSalesMarket> lstSalesMarket { get; set; }

        public bool IsAllOpsMkts { get; set; }
        public List<ProductSupplierOperatingMarket> SelectedOperatingMarket { get; set; }
        public List<ProductSupplierOperatingMarket> selectedlstOperatingMarket { get; set; }
        public List<ProductSupplierOperatingMarket> lstOperatingMarket { get; set; }

        public List<ProductSupplierSalesAgent> lstSalesAgent { get; set; }

        public List<string> TeamIds { get; set; }
               
        public MultiSelectList Teams { get; set; }

        public ProductViewModel()
        {
            ProductSRPViewModel = new ProductsSRPViewModel();
            lstSales = new List<Contact>();
            lstFit = new List<Contact>();
            lstGroup = new List<Contact>();
            lstFinance = new List<Contact>();
            lstEmergency = new List<Contact>();
            lstComplaint = new List<Contact>();
            lstSalesMarket = new List<ProductSupplierSalesMarket>();
            lstOperatingMarket = new List<ProductSupplierOperatingMarket>();
            lstSalesAgent = new List<ProductSupplierSalesAgent>();
            selectedlstSalesMarket = new List<ProductSupplierSalesMarket>();
            selectedlstOperatingMarket = new List<ProductSupplierOperatingMarket>();
            SelectedSalesMarket = new List<ProductSupplierSalesMarket>();
            SelectedOperatingMarket = new List<ProductSupplierOperatingMarket>();
        }
    }

    public class Contact
    {
        public string Contact_Id { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Mail { get; set; }
    }
}
