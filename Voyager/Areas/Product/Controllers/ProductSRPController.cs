using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Voyager.Controllers;
using Voyager.App.ViewModels;
using Voyager.App.Providers;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Models;
using Microsoft.AspNetCore.Authorization;

namespace Voyager.Areas.Product.Controllers
{
	/// <summary>
	/// Product Search controller
	/// </summary>
	[Authorize]
	[Area("Product")]
	[Route("Product")]
	public class ProductSRP : VoyagerController
	{
		private readonly IConfiguration _configuration;
		private ProductLibrary productLibrary;
		private PositionLibrary positionLibrary;

        /// <summary>
        /// Product Search Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ProductSRP(IConfiguration configuration) : base(configuration)
		{
			_configuration = configuration;
			productLibrary = new ProductLibrary(_configuration);
			positionLibrary = new PositionLibrary(_configuration);
		}

		/// <summary>
		/// Product Search page action
		/// </summary>
		/// <returns></returns>
		[Route("ProductSearch")]
		public IActionResult ProductSearch()
		{
			//ProductsSRPViewModel productsSRPViewModel = new ProductsSRPViewModel();
			//productLibrary.BindProdSearchFilterData(ref productsSRPViewModel, token);
			return View();
		}

        /// <summary>
        /// returns Product Search page partial view
        /// </summary>
        /// <returns></returns>
        [Route("_ProductSearchView")]
        public IActionResult _ProductSearchPage(string PageName=null, ProductSRPFilters productSRPFilters = null)
        {
            ProductsSRPViewModel productsSRPViewModel = new ProductsSRPViewModel();
            productsSRPViewModel.PageName = PageName; ;
            productLibrary.BindProdSearchFilterData(ref productsSRPViewModel, token);
            productsSRPViewModel.IsHotelDefSupplier = _configuration.GetValue<string>("IsHotelDefSupplier");

            if (productSRPFilters != null)
            {
                productsSRPViewModel.ProductSRPFilters.ProductTypeID = productSRPFilters.ProductTypeID;
                productsSRPViewModel.ProductSRPFilters.ProductType = productsSRPViewModel.ProductSRPFilters.ProductTypeList.Where(a => a.AttributeValue_Id == productSRPFilters.ProductTypeID).FirstOrDefault()?.Value;
                productsSRPViewModel.ProductSRPFilters.CityName = productSRPFilters.CityName;
                productsSRPViewModel.ProductSRPFilters.LocationID = productSRPFilters.LocationID;
                productsSRPViewModel.ProductSRPFilters.BudgetCategoryID = productSRPFilters.BudgetCategoryID;
                productsSRPViewModel.ProductSRPFilters.StarRating = productSRPFilters.StarRating;
                productsSRPViewModel.ProductSRPFilters.Chain = productSRPFilters.Chain;
            }
            return View(productsSRPViewModel);
        }


		/// <summary>
		/// Product Search result action
		/// </summary>
		/// <returns></returns>
		[Route("ProductSRP")]
		public IActionResult ProductSearchResultPage(ProductSRPFilters filters)
		{
			ProductsSRPViewModel productsSRPViewModel = new ProductsSRPViewModel();
			ProductProviders productProviders = new ProductProviders(_configuration);
			ProductSRPSearchReq objQRFAgentRequest = new ProductSRPSearchReq()
			{
				ProdType = filters.ProductType,
				ProdName = filters.ProdName,
				ProdCode = filters.ProdCode,
				CityName = filters.CityName,
				Location = filters.Location == "Select" ? null : filters.Location,
				BudgetCategory = filters.BudgetCategory == "Select" ? null : filters.BudgetCategory,
				Chain = filters.Chain,
				StarRating = filters.StarRating,
				Status = filters.Status
			};
			ProductSRPSearchRes objProposalRes = productProviders.GetProductSRPDetails(objQRFAgentRequest, token).Result;
			productsSRPViewModel.ProductSRPDetails = objProposalRes.ProductSearchDetails.Select(a => new ProductSRPDetails
			{
				Address = a.Address,
				BdgPriceCategory = a.BdgPriceCategory,
				Chain = a.Chain,
				CityName = a.CityName,
				CountryName = a.CountryName,
				DefaultSupplier = a.DefaultSupplier,
				DefaultSupplierId = a.DefaultSupplierId,
				HotelImageURL = a.HotelImageURL != null ? a.HotelImageURL.Replace("resources/", "ImageResources/") : a.HotelImageURL,
				HotelType = a.HotelType,
				Location = a.Location,
				PostCode = a.PostCode,
				ProdDesc = a.ProdDesc,
				ProdName = a.ProdName,
				ProductCode = a.ProductCode,
				ProductType = a.ProductType,
				StarRating = a.StarRating,
				Street = a.Street,
				VoyagerProduct_Id = a.VoyagerProduct_Id,
				ProductType_Id = a.ProductType_Id
			}).ToList();

            var userRoles = HttpContext.Request.Cookies["UserRoles"] ?? string.Join(",", UserRoles);
            if (userRoles != null && userRoles.Contains("Administrator"))
			{
				productsSRPViewModel.PageName = "PrdSupplierMapping";
			}

			return PartialView("_ProductInfo", productsSRPViewModel);
		}

		/// <summary>
		/// Product Search filter action for HotelChain autocomplete control
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		[Route("GetChainName")]
		public JsonResult GetChainName(string term)
		{
			List<AutoCompleteTextBox> chainList = new List<AutoCompleteTextBox>();
			chainList = positionLibrary.GetChainList(term, token);
			return Json(chainList);
		}

		/// <summary>
		/// redirects to appropriate action of the product type provided
		/// </summary>
		/// <returns></returns>
		[Route("ViewFullProductInfo")]
		public IActionResult ViewFullProductInfo()
		{
			string ProductId = Request.Query["ProductId"].ToString();
			string ProductType = Request.Query["ProductType"].ToString();
			string Action = "", Controller = "Product";

			switch (ProductType)
			{
				case "Hotel":
					Action = "Accomodation";
					break;
				default:
					break;
			}

			if (!string.IsNullOrEmpty(Action))
			{
				return RedirectToAction(Action, Controller, new { ProductId });
			}
			else
			{
				return Json("OK");
			}
		}


		[Route("GetCityName")]
		public JsonResult GetCityNamefilter(string term, string QRFID)
		{
			CityLookupRequest objCityLookupRequest = new CityLookupRequest();
			if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
			objCityLookupRequest.CityName = term;
			objCityLookupRequest.QRFID = QRFID;

			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			CityLookupResponse objCityLookupResponse = objSalesProvider.GetCityList(objCityLookupRequest, token).Result;
			if (objCityLookupResponse.CityLookupProperties == null)
				return Json("");
			else
			{
				List<AutoCompleteTextBox> cityList = new List<AutoCompleteTextBox>();
				cityList = objCityLookupResponse.CityLookupProperties.Select(data => new AutoCompleteTextBox { value = data.Voyager_Resort_Id, label = data.Lookup }).ToList();
				return Json(cityList);
			}
		}

	}
}