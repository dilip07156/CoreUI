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
    [Area("Amendment")]
    [Route("Amendment")]
    public class AccomodationAmendment : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private ProductLibrary productLibrary;
        private PositionLibrary positionLibrary;
        private COCommonLibrary coCommonLibrary;

        /// <summary>
        /// Product Search Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public AccomodationAmendment(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            productLibrary = new ProductLibrary(_configuration);
            positionLibrary = new PositionLibrary(_configuration);
            coCommonLibrary = new COCommonLibrary(_configuration);
        }

        /// <summary>
        /// Accomodation PDP
        /// </summary>
        /// <returns></returns>
        [Route("Accomodation")]
        public IActionResult Accomodation()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            AccomodationAmendmentVM model = new AccomodationAmendmentVM();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Accomodation";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = QRFID };
            model.COHeaderViewModel = coCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            //model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            ProductProviders productProviders = new ProductProviders(_configuration);
            ProductSRPHotelGetRes response = productProviders.GetProductSRPHotelDetails(new ProductSRPHotelGetReq() {QRFID= QRFID }, token).Result;
            model.ProductSRPRouteInfo = response.ProductSRPRouteInfo;
            return View(model);
        }
        
    }
}