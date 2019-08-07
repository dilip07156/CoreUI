using System;
using System.Collections.Generic;
using System.Text;
using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Contracts;
using Microsoft.Extensions.Configuration;
using System.Linq;
using VGER_WAPI_CLASSES;
using Microsoft.AspNetCore.Http;

namespace Voyager.App.Library
{
    /// <summary>
    /// Library for Product Module
    /// </summary>
    public class ProductLibrary
    {
        private MasterProviders objMasterProviders;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// ProductLibrary constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ProductLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            objMasterProviders = new MasterProviders(_configuration);
        }

        /// <summary>
        /// Bind Master data for filters of product search
        /// </summary>
        /// <param name="productsSRPViewModel"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool BindProdSearchFilterData(ref ProductsSRPViewModel productsSRPViewModel, string token)
        {
            //ProductProviders productProviders = new ProductProviders(_configuration);
            //ProductSRPSearchRes objProposalRes = productProviders.GetProductSRPDetails(new ProductSRPSearchReq(), token).Result;

            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            //Get Product Type list for filters dropdown
            ProdTypeGetRes objProdTypeGetRes = objMasterProviders.GetProdTypeByProdType(new ProdTypeGetReq(), token).Result;
            if (objProdTypeGetRes.ResponseStatus.Status == "Success" && objProdTypeGetRes.ProductTypeList.Count > 0)
            {
                productsSRPViewModel.ProductSRPFilters.ProductTypeList = objProdTypeGetRes.ProductTypeList.Select(a => new AttributeValues
                {
                    AttributeValue_Id = a.VoyagerProductType_Id,
                    Value = a.Prodtype
                }).ToList();
            }

            //Get Product Budget Category list for filters dropdown
            ProdCatDefGetRes objProdCatDefGetRes = objMasterProviders.GetProdCatDef( token).Result;
            if (objProdCatDefGetRes.ResponseStatus.Status == "Success" && objProdCatDefGetRes.ProdCatDefProperties.Count > 0)
            {
                productsSRPViewModel.ProductSRPFilters.BudgetCategoryList = objProdCatDefGetRes.ProdCatDefProperties.Select(a => new AttributeValues
                {
                    AttributeValue_Id = a.VoyagerDefProductCategoryId,
                    Value = a.Name
                }).ToList();
            }

            //Get Product Location list for filters dropdown
            ProductAttributeRes objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(new ProductAttributeReq() { AttributeName = "Location" }, token).Result;
            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                productsSRPViewModel.ProductSRPFilters.LocationList = objProductAttributeRes.ProductAttributeDetails.Select(a => new AttributeValues
                {
                    AttributeValue_Id = a.AttributeId,
                    Value = a.Value
                }).ToList();
            }

            //Populate Star Rating list for filters dropdown
            List<AttributeValues> starRating = new List<AttributeValues>
            {
                new AttributeValues { Value = "1" },
                new AttributeValues { Value = "2" },
                new AttributeValues { Value = "3" },
                new AttributeValues { Value = "4" },
                new AttributeValues { Value = "5" }
            };
            productsSRPViewModel.ProductSRPFilters.StarRatingList = starRating;

            //Populate Product Status list for filters dropdown
            List<AttributeValues> statusList = new List<AttributeValues>
            {
                new AttributeValues { Value = "Active" },
                new AttributeValues { Value = "Inactive" }
            };
            productsSRPViewModel.ProductSRPFilters.StatusList = statusList;

            return true;
        }
    }
}
