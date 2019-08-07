using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class MealsController : VoyagerController
    {
        #region declaration
        private readonly IConfiguration _configuration;
        PositionMapping positionMapping;
        private string SessionName = "IntegrationInfo";
        #endregion

        public MealsController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Meals()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;

            MealsViewModel model = new MealsViewModel();
            List<ProductType> lst = new List<ProductType>();
            lst.Add(new ProductType { ProdType = "Meal" });
            PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = QRFID, ProductType = lst, PositionId = null, Type = "meal" };
            model = positionMapping.GetMealDetails(_configuration, token, objPositionGetReq, objCookies, "", sessionData);

            return View(model);
        }

        [HttpPost]
        public IActionResult Meals(MealsViewModel model)
        {
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
            PositionSetRes objPositionSetRes = positionMapping.SetAllMealDetails(_configuration, token, model, objCookies, sessionData);
            //if (objPositionSetRes != null && objPositionSetRes.ResponseStatus.Status.ToLower() == "success")
            //{
            //    TempData["success"] = objPositionSetRes.ResponseStatus.ErrorMessage;
            //}
            //else
            //{
            //    TempData["error"] = objPositionSetRes.ResponseStatus.ErrorMessage;
            //}
            //return RedirectToAction("Meals", new { model.MenuViewModel.QRFID });
            objPositionSetRes.ResponseStatus.ErrorMessage = "Meal " + objPositionSetRes.ResponseStatus.ErrorMessage;
            return Json(new { objPositionSetRes.ResponseStatus });
        }

        public IActionResult GetMealsvenue(VenueGetParam objVenueGetParam, bool IsClone = false)
        {
            VenueDetails modelVenueDetails = new VenueDetails();
            modelVenueDetails.MealType = "";
            try
            {
                objVenueGetParam.PositionId = string.IsNullOrEmpty(objVenueGetParam.PositionId) ? "0" : objVenueGetParam.PositionId;
                List<ProductType> lst = new List<ProductType>();
                lst.Add(new ProductType { ProdType = "Meal" });
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = objVenueGetParam.QRFID, ProductType = lst, PositionId = objVenueGetParam.PositionId, IsClone = IsClone, Type = objVenueGetParam.MealType };
                modelVenueDetails = positionMapping.GetMealDetails(_configuration, token, objPositionGetReq, objCookies, objVenueGetParam.Day, sessionData).VenueDetails;
                modelVenueDetails.QRFID = objVenueGetParam.QRFID;
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_VenueDetails", modelVenueDetails);
        }

        [HttpPost]
        public ActionResult VenueMealsDetails(VenueDetails venueDetails)
        { 
            venueDetails.UserName = ckUserEmailId;
           // venueDetails.VoyagerUserID = ckLoginUser_Id;
            venueDetails = positionMapping.SetMealDetails(_configuration, token, venueDetails);
            venueDetails.ResponseStatus.ErrorMessage = "Meal " + venueDetails.ResponseStatus.ErrorMessage;
            return Json(venueDetails);
        }
    }
}