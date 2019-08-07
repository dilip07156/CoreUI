using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.App.Mappers
{
    public class SalesQRFMapping
    {
        #region Declaration
        private MasterProviders objMasterProviders;
        private readonly IConfiguration _configuration;

        public SalesQRFMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            objMasterProviders = new MasterProviders(_configuration);
        }
        #endregion

        public QuoteRoutingViewModel QRFRoutingGet(RoutingGetRes routingGetRes, string token)
        {
            QuoteRoutingViewModel quoteRoutingViewModel = new QuoteRoutingViewModel();

            #region Bind Hotel Star Rating
            var objStarRating = new List<ProductAttributeDetails>();

            ProductAttributeReq objProductAttributeReq = new ProductAttributeReq();
            objProductAttributeReq.AttributeName = "StarRating";
            ProductAttributeRes objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                objStarRating = objProductAttributeRes.ProductAttributeDetails.Where(a => a.Value.Contains("STD")).ToList();
            }
            quoteRoutingViewModel.StarRatingList = objStarRating;
            #endregion

            if (routingGetRes.ResponseStatus.Status.ToLower() == "success" && routingGetRes.RoutingInfo.Count > 0)
            {
                quoteRoutingViewModel.QRFID = routingGetRes.QRFID;
                quoteRoutingViewModel.IsRoutingExist = true;
                if (routingGetRes.RoutingInfo.Where(a => !string.IsNullOrEmpty(a.PrefStarRating)).Count() > 0)
                    quoteRoutingViewModel.IsSetPrefHotels = true;

                quoteRoutingViewModel.QuoteRoutingData = new List<QuoteRoutingData>();

                foreach (var objRes in routingGetRes.RoutingInfo)
                {
                    var objModel = new QuoteRoutingData();

                    objModel.RouteSequence = objRes.RouteSequence;
                    objModel.RouteID = objRes.RouteID;
                    objModel.FromCityName = objRes.FromCityName;
                    objModel.FromCityID = objRes.FromCityID;
                    objModel.ToCityName = objRes.ToCityName;
                    objModel.ToCityID = objRes.ToCityID;
                    objModel.Days = Convert.ToInt16(objRes.Days);
                    objModel.Nights = Convert.ToInt16(objRes.Nights);
                    objModel.IsLocalGuide = Convert.ToBoolean(objRes.IsLocalGuide);
                    objModel.PrefStarRating = objRes.PrefStarRating;

                    quoteRoutingViewModel.QuoteRoutingData.Add(objModel);
                }
            }
            else
            {
                quoteRoutingViewModel.QRFID = routingGetRes.QRFID;
                quoteRoutingViewModel.QuoteRoutingData = new List<QuoteRoutingData>();
                var objModel = new QuoteRoutingData();

                objModel.RouteSequence = 1;
                objModel.RouteID = 0;
                objModel.FromCityName = "";
                objModel.FromCityID = "";
                objModel.ToCityName = "";
                objModel.ToCityID = "";
                objModel.Days = null;
                objModel.Nights = null;
                objModel.IsLocalGuide = false;

                quoteRoutingViewModel.QuoteRoutingData.Add(objModel);
            }


            return quoteRoutingViewModel;
        }

        public RoutingSetReq QRFRoutingSet(QuoteRoutingViewModel model, string CurrentUser)
        {
            RoutingSetReq routingSetReq = new RoutingSetReq();
            routingSetReq.QRFID = model.QRFID;
            routingSetReq.IsOverwriteExtPos = model.IsOverwriteExtPos;
            routingSetReq.IsSetPrefHotels = model.IsSetPrefHotels;
            routingSetReq.RoutingInfo = new List<RoutingInfo>();
            model.QuoteRoutingData = model.QuoteRoutingData.Where(a => !string.IsNullOrEmpty(a.FromCityID) && !string.IsNullOrEmpty(a.ToCityID)).ToList();
            foreach (var objModel in model.QuoteRoutingData)
            {
                var obj = new RoutingInfo();

                obj.RouteSequence = objModel.RouteSequence;
                obj.RouteID = objModel.RouteID;
                obj.FromCityID = objModel.FromCityID;
                obj.ToCityID = objModel.ToCityID;
                obj.Days = Convert.ToInt16(objModel.Days);
                obj.Nights = Convert.ToInt16(objModel.Nights);
                obj.IsLocalGuide = objModel.IsLocalGuide;
                obj.PrefStarRating = objModel.PrefStarRating;

                obj.CreateUser = CurrentUser;
                obj.CreateDate = DateTime.Now;
                obj.EditUser = CurrentUser;
                obj.EditDate = DateTime.Now;

                routingSetReq.RoutingInfo.Add(obj);
            }

            return routingSetReq;
        }
    }
}
