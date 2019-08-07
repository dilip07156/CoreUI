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
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Voyager.App.Library
{
    public class PositionLibrary
    {
        #region Declaration
        MasterProviders objMasterProviders;
        PositionProviders objPositionProviders;
        SalesQuoteLibrary salesQuoteLibrary;
        private readonly IConfiguration _configuration;
        public PositionLibrary()
        {
        }
        public PositionLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            objMasterProviders = new MasterProviders(_configuration);
            objPositionProviders = new PositionProviders(_configuration);
            salesQuoteLibrary = new SalesQuoteLibrary(_configuration);
        }
        #endregion

        #region Accomodation

        public bool BindAccomodationMasterData(IConfiguration _configuration, string token, ref AccomodationViewModel model, int totalRecord)
        {
            ProductAttributeReq objProductAttributeReq;
            ProductAttributeRes objProductAttributeRes;
            //bool GetStatus = false;

            //#region Get Quote Info By QRFId
            //NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            //modelQuote.QRFID = model.MenuViewModel.QRFID;
            //SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            //GetStatus = salesQuoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            ////if (GetStatus)
            ////{
            ////    model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
            ////}
            //#endregion

            // #region Bind StartingFrom and NoOfNights
            //var objAccomodationData = new AccomodationData();
            //  List<AttributeValues> listSF = new List<AttributeValues>();
            //  List<AttributeValues> listNON = new List<AttributeValues>();

            //if (modelQuote.mdlQuoteAgentInfoViewModel.Duration > 0)
            //{
            //    for (int i = 1; i <= modelQuote.mdlQuoteAgentInfoViewModel.Duration; i++)
            //    {
            //       // listSF.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = "Day " + i });
            //        listNON.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
            //    }
            // //   listSF.Add(new AttributeValues { AttributeValue_Id = (modelQuote.mdlQuoteAgentInfoViewModel.Duration + 1).ToString(), Value = "Day " + (modelQuote.mdlQuoteAgentInfoViewModel.Duration + 1) });
            //}
            //else
            //{
            //   // listSF.Add(new AttributeValues { AttributeValue_Id = "1", Value = "Day 1" });
            //    listNON.Add(new AttributeValues { AttributeValue_Id = "1", Value = "1" });
            //}

            //   #endregion

            #region Bind Hotel category
            var objCategory = new List<ProdCatDefProperties>();

            objProductAttributeReq = new ProductAttributeReq();
            objProductAttributeReq.AttributeName = "Budgetprice Category";
            objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                objCategory = objProductAttributeRes.ProductAttributeDetails.Select(a => new ProdCatDefProperties
                {
                    VoyagerDefProductCategoryId = a.AttributeId + "|" + a.Value,
                    Name = a.Value
                }).ToList();
            }

            //ProdCatDefGetReq objProdCatDefGetReq = new ProdCatDefGetReq(); 
            //ProdCatDefGetRes objProdCatDefGetRes = objMasterProviders.GetProdCatDefByName(objProdCatDefGetReq, token).Result;

            //if (objProdCatDefGetRes.ResponseStatus.Status == "Success" && objProdCatDefGetRes.ProdCatDefProperties.Count > 0)
            //{
            //    //objCategory = objProdCatDefGetRes.ProdCatDefProperties;
            //    objCategory = objProdCatDefGetRes.ProdCatDefProperties.Select(a => new ProdCatDefProperties
            //    {
            //        VoyagerDefProductCategoryId = a.VoyagerDefProductCategoryId + "|" + a.Name,
            //        Name = a.Name
            //    }).ToList();
            //}
            #endregion

            #region Bind Hotel Star Rating
            var objStarRating = new List<ProductAttributeDetails>();

            objProductAttributeReq = new ProductAttributeReq();
            objProductAttributeReq.AttributeName = "StarRating";
            objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                objStarRating = objProductAttributeRes.ProductAttributeDetails;
            }
            #endregion

            #region Bind Hotel Location
            var objLocation = new List<ProductAttributeDetails>();

            objProductAttributeReq = new ProductAttributeReq();
            objProductAttributeReq.AttributeName = "Location";
            objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                objLocation = objProductAttributeRes.ProductAttributeDetails;
            }
            #endregion

            #region Meal Plan
            //var objMealPlan = new List<ProductAttributeDetails>();

            //objProductAttributeReq = new ProductAttributeReq();
            //objProductAttributeReq.AttributeName = "Board";
            //objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            //if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            //{
            //    objMealPlan = objProductAttributeRes.ProductAttributeDetails;
            //}

            var objMealPlan = new List<ProductAttributeDetails>();

            var defMealPlanGetReq = new DefMealPlanGetReq();
            defMealPlanGetReq.Status = "false";
            DefMealPlanGetRes objDefMealPlanGetRes = objMasterProviders.GetDefMealPlan(defMealPlanGetReq, token).Result;

            if (objDefMealPlanGetRes.ResponseStatus.Status == "Success" && objDefMealPlanGetRes.mDefMealPlan.Count > 0)
            {
                objMealPlan = objDefMealPlanGetRes.mDefMealPlan.Select(a => new ProductAttributeDetails { AttributeId = a.MealPlan, Value = a.Description }).ToList();
            }
            #endregion

            #region Bind Keep As
            var objKeepAs = new List<AttributeValues>();

            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "Position Type";

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "Position Type")
                {
                    objKeepAs = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
                }
            }
            #endregion

            for (int i = 0; i < totalRecord; i++)
            {
                //  model.AccomodationData[i].StartingFromList = listSF;
                // model.AccomodationData[i].NoOfNightList = listNON;
                model.AccomodationData[i].CategoryList = objCategory;
                model.AccomodationData[i].StarRatingList = objStarRating;
                model.AccomodationData[i].LocationList = objLocation;
                model.AccomodationData[i].MealPlanList = objMealPlan;
                model.AccomodationData[i].KeepAsList = objKeepAs;
            }

            return true;
        }

        public TourInfoHeaderViewModel GetSalesTourInfoHeader(ref NewQuoteViewModel modelQuote, string token)
        {
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            salesQuoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            return modelQuote.TourInfoHeaderViewModel;
        }

        public List<AutoCompleteTextBox> GetCityList(string term, string token)
        {
            CityLookupRequest objCityLookupRequest = new CityLookupRequest();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objCityLookupRequest.CityName = term;

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            CityLookupResponse objCityLookupResponse = objSalesProvider.GetCityList(objCityLookupRequest, token).Result;
            List<AutoCompleteTextBox> cityList = new List<AutoCompleteTextBox>();

            if (objCityLookupResponse.CityLookupProperties == null)
                return cityList;
            else
            {
                cityList = objCityLookupResponse.CityLookupProperties.Select(data => new AutoCompleteTextBox { value = data.Voyager_Resort_Id, label = data.Lookup }).ToList();
                return cityList;
            }
        }

        public List<AutoCompleteTextBox> GetChainList(string term, string token)
        {
            ProductAttributeReq objProductAttributeReq = new ProductAttributeReq();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objProductAttributeReq.AttributeName = "Chain";
            objProductAttributeReq.Attributevalue = term;
            objProductAttributeReq.Status = "chain";
            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProductAttributeRes objProductAttributeRes = objMasterProviders.GetProdAttributeDetailsByNameOrVal(objProductAttributeReq, token).Result;

            List<AutoCompleteTextBox> chainList = new List<AutoCompleteTextBox>();
            if (objProductAttributeRes.ResponseStatus.Status == "Success" && objProductAttributeRes.ProductAttributeDetails.Count > 0)
            {
                chainList = objProductAttributeRes.ProductAttributeDetails.Select(data => new AutoCompleteTextBox { value = data.AttributeId, label = data.Value }).ToList();
                return chainList;
            }
            else
                return chainList;
        }

        public List<AutoCompleteTextBox> GetHotelList(ProductSearchReq objProductSearchReq, string token)
        {
            //the below condition commented coz its handled in WAPI level- Manisha
            //  if (objProductSearchReq.ProdName.Length >= 3 && objProductSearchReq.ProdName.Substring(0, 3) == "###") objProductSearchReq.ProdName = "";

            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProductSearchRes objProductSearchRes = objMasterProviders.GetProductDetailsBySearchCriteria(objProductSearchReq, token).Result;

            List<AutoCompleteTextBox> hotelNameList = new List<AutoCompleteTextBox>();
            if (objProductSearchRes.ResponseStatus.Status == "Success" && objProductSearchRes.ProductSearchDetails.Count > 0)
            {
                hotelNameList = objProductSearchRes.ProductSearchDetails.Select(data => new AutoCompleteTextBox
                {
                    value = data.VoyagerProduct_Id,
                    label = data.ProdName,
                    type = data.ProdType,
                    typeId = data.ProdTypeId,
                    categoryid = data.CategoryId,
                    location = data.Location,
                    starrating = data.StarRating,
                    category = data.CategoryName,
                    chain = data.Chain,
                    chainid = data.ChainId
                }).ToList();
                return hotelNameList;
            }
            else
                return hotelNameList;
        }

        public PositionGetRoomTypeAndSuppRes GetRoomTypeAndSupplementList(List<string> HotelId, string QRFID, string token)
        {
            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProdCategoryRangeGetReq prodCategoryRangeGetReq = new ProdCategoryRangeGetReq();
            prodCategoryRangeGetReq.ProductId = HotelId;
            prodCategoryRangeGetReq.QRFID = QRFID;
            prodCategoryRangeGetReq.AdditionalYn = false;
            PositionGetRoomTypeAndSuppRes objAcc = objPositionProviders.GetRoomTypeAndSupplement(prodCategoryRangeGetReq, token).Result;

            //List<AutoCompleteTextBox> RoomTypeList = new List<AutoCompleteTextBox>();
            //List<AutoCompleteTextBox> SupplementList = new List<AutoCompleteTextBox>();
            ////if (objAcc.ResponseStatus.Status == "Success")
            //{
            //    if (objAcc.RoomTypeList.Count > 0)
            //    {
            //        RoomTypeList = objAcc.RoomTypeList.Select(data => new AutoCompleteTextBox { value = data.ProductCategoryId + '|' + data.VoyagerProductRange_Id, label = "(" + data.ProductCategoryName + ") " + data.ProductRangeCode }).ToList();
            //    }
            //    if (objAcc.SupplementList.Count > 0)
            //    {
            //        SupplementList = objAcc.RoomTypeList.Select(data => new AutoCompleteTextBox { value = data.ProductCategoryId + '|' + data.VoyagerProductRange_Id, label = "(" + data.ProductCategoryName + ") " + data.ProductRangeCode }).ToList();
            //    }
            //    return objAcc;
            //}
            //else
            return objAcc;
        }

        //public List<AutoCompleteTextBox> GetSupplementList(string HotelId, string token)
        //{
        //    MasterProviders objMasterProviders = new MasterProviders(_configuration);
        //    ProdCategoryRangeGetReq prodCategoryRangeGetReq = new ProdCategoryRangeGetReq();
        //    prodCategoryRangeGetReq.ProductId = HotelId;
        //    prodCategoryRangeGetReq.AdditionalYn = true;
        //    ProdCategoryRangeGetRes prodCategoryRangeGetRes = objMasterProviders.GetProductCategoryRangeByProductID(prodCategoryRangeGetReq, token).Result;

        //    List<AutoCompleteTextBox> SupplementList = new List<AutoCompleteTextBox>();
        //    if (prodCategoryRangeGetRes.ResponseStatus.Status == "Success" && prodCategoryRangeGetRes.ProdCategoryRangeDetails.Count > 0)
        //    {
        //        SupplementList = prodCategoryRangeGetRes.ProdCategoryRangeDetails.Select(data => new AutoCompleteTextBox { value = data.ProductCategoryId + '|' + data.VoyagerProductRange_Id, label = "(" + data.ProductCategoryName + ") " + data.ProductRangeCode }).ToList();
        //        return SupplementList;
        //    }
        //    else
        //        return SupplementList;
        //}

        #endregion
    }
}
