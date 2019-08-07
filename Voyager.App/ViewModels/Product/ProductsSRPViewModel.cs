using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class ProductsSRPViewModel
    {
        public ProductSRPFilters ProductSRPFilters = new ProductSRPFilters();
        public List<ProductSRPDetails> ProductSRPDetails = new List<ProductSRPDetails>();
        //public ProductSRPDetails ProductSRPDetails = new ProductSRPDetails();
        public string PageName { get; set; }
        public int Counter { get; set; }
        public string IsHotelDefSupplier { get; set; }
    }

    public class ProductSRPFilters
    {
        public string ProdName { get; set; }
        public string ProdCode { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public string ProductTypeID { get; set; }
        public string ProductType { get; set; }
        public string LocationID { get; set; }
        public string Location { get; set; }
        public string StarRating { get; set; }
        public string BudgetCategoryID { get; set; }
        public string BudgetCategory { get; set; }
        public string ChainID { get; set; }
        public string Chain { get; set; }
        public string Status { get; set; }
        public List<AttributeValues> ProductTypeList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> StarRatingList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> BudgetCategoryList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> StatusList { get; set; } = new List<AttributeValues>();
        public List<AttributeValues> LocationList { get; set; } = new List<AttributeValues>();
        public List<Def_Facilities> ProductFacilities { get; set; } = new List<Def_Facilities>();
    }

    //public class ProductSRPDetails
    //{
    //    public string VoyagerProduct_Id { get; set; }
    //    public string ProductType_Id { get; set; }
    //    public bool? Placeholder { get; set; }
    //    public string ProdName { get; set; }
    //    public string ProductCode { get; set; }
    //    public string DefaultSupplier { get; set; }
    //    public string CountryName { get; set; }
    //    public string CityName { get; set; }
    //    public string ProductType { get; set; }
    //    public string Address { get; set; }
    //    public string Street { get; set; }
    //    public string PostCode { get; set; }
    //    public string Status { get; set; }
    //    public string ProdDesc { get; set; }
    //    public string Location { get; set; }
    //    public string StarRating { get; set; }
    //    public string BdgPriceCategory { get; set; }
    //    public string Chain { get; set; }
    //    public string HotelType { get; set; }
    //    public string HotelImageURL { get; set; }
    //    public List<Def_Facilities> ProductFacilities { get; set; }
    //    public List<ProductRoomTypes> Rooms { get; set; }
    //    public string CreateUser { get; set; }
    //    public DateTime? CreateDate { get; set; }
    //    public string EditUser { get; set; }
    //    public DateTime? EditDate { get; set; }
    //}
}
