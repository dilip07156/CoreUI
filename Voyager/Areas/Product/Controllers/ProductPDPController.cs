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
    public class ProductPDP : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private ProductLibrary productLibrary;
        private PositionLibrary positionLibrary;
        private COCommonLibrary coCommonLibrary;
        private ProductProviders productProviders;

        /// <summary>
        /// Product Search Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ProductPDP(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            productLibrary = new ProductLibrary(_configuration);
            positionLibrary = new PositionLibrary(_configuration);
            coCommonLibrary = new COCommonLibrary(_configuration);
            productProviders = new ProductProviders(_configuration);
        }

        /// <summary>
        /// Accomodation PDP action
        /// </summary>
        /// <returns></returns>
        [Route("Accomodation")]
        public IActionResult Accomodation()
        {
            ProductsPDPViewModel model = new ProductsPDPViewModel();
            model.Products.VoyagerProduct_Id = Request.Query["ProductId"].ToString();
            return View(model);
        }

        /// <summary>
        /// returns Accomodation PDP partial view
        /// </summary>
        /// <returns></returns>
        [Route("_Accomodation")]
        public IActionResult _Accomodation(string ProductId)
        {
            ProductsPDPViewModel model = new ProductsPDPViewModel();
            //model.MenuViewModel.QRFID = Request.Query["QRFId"].ToString();
            //model.MenuViewModel.MenuName = "Accomodation";
            model.ImageInitial = _configuration.GetValue<string>("SystemSettings:CountryImageInitial");

            #region Get Costing Officer Tour Info Header By QRFId
            //NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = "255" };
            //model.COHeaderViewModel = coCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            //model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            #region Get Product Info from service
            //string ProductId = Request.Query["ProductId"].ToString();
            ProductPDPSearchRes ProductPDP = productProviders.GetProductPDPDetails(new List<string> { ProductId }, token).Result;
            if (ProductPDP.ProductDetails?.Count > 0) model.Products = ProductPDP.ProductDetails.FirstOrDefault();
            model.KeyFacilities = GetKeyFacilities();

            StaticMap._configuration = _configuration;
            //GeocoderLocation geocoder = null;
            string Landmark, mapURL = "https://maps.googleapis.com/maps/api/staticmap?zoom=10&size=640x416&maptype=roadmap&key=GoogleAPIKey";
            mapURL += "&markers=color:red|label:" + model.Products.ProductName.Substring(0, 1) + "|{0},{1}";
            mapURL = string.Format(mapURL, model.Products.Lat, model.Products.Long);
            //model.Products.InAndAround.Add(new InAndAround
            //{
            //    LandmarkName = "British Museum",
            //    LandmarkType = "Attraction",
            //    Direction = "south",
            //    Distance = "10",
            //    DistanceUnit = "km",
            //});
            foreach (var item in model.Products.InAndAround)
            {
                Landmark = item.LandmarkName + "," + model.Products.CityName + "," + model.Products.CountryName;
                //geocoder = StaticMap.Locate(Landmark);
                //if (geocoder.Latitude != 0 && geocoder.Longitude != 0)
                //{
                mapURL += "&markers=color:blue|label:" + item.LandmarkName.Substring(0, 1) + "|" + Landmark;
                //mapURL = string.Format(mapURL, geocoder.Latitude, geocoder.Longitude);
                //}
            }
            if (StaticMap.RenderImage(mapURL, model.Products.VoyagerProduct_Id + "_PDP_hotelmap.png", out string output))
            {
                model.ProductMapUrl = output;
            }

            #endregion

            #region replacing image urls from resources to ImageResources
            if (model.Products != null)
            {
                foreach (var ProdResource in model.Products.ProductResources)
                {
                    if (ProdResource.ImageSRC != null) ProdResource.ImageSRC = ProdResource.ImageSRC.Replace("resources/", "ImageResources/");
                }
            }
            #endregion
            return View(model);
        }

        private string[] GetKeyFacilities()
        {
            return new string[] {
                "Swimming Pool",
                "Mini Bar",
                "Health Club",
                "Coffee Shop",
                "Hair Dryer",
                "Parking Available",
                "Rooms",
                "Trouser Press",
                "Banquet",
                "Travel Desk",
                "Bars",
                "Breakfast Room",
                "Acceptence Credit Card",
                "Restaurants",
                "Wheelchair Available",
                "Tee & Coffee Making Facilities",
                "Elevator",
                "Games",
                "Tennis Court",
                "Outdoor Pool",
                "Rooms for disabled guests",
                "Direct Dial Telephone",
                "Indoor Pool",
                "Leisure Facilities",
                "Conference Rooms",
                "In House Movies",
                "Forex / Currency Excahnge"
            };
        }
    }
}