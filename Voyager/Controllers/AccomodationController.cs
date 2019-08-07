
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class AccomodationController : VoyagerController
    {
        #region Declaration
        private AccomodationViewModel model;
        private PositionLibrary posLibrary;
        private PositionProviders positionProviders;
        private PositionMapping accomoMapping;
        private PositionGetReq positionGetReq;
        private PositionGetRes positionGetRes;
        private PositionSetReq positionSetReq;
        private PositionSetRes positionSetRes;
        private MasterProviders masterProviders;
        private readonly IConfiguration _configuration;

        public AccomodationController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            posLibrary = new PositionLibrary(_configuration);
            positionProviders = new PositionProviders(_configuration);
            accomoMapping = new PositionMapping(_configuration);
            masterProviders = new MasterProviders(_configuration);
        }
        #endregion
        //[Route("Quote/Accomodation")]
        public IActionResult Accomodation()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            string PositionId = Request.Query["PositionId"].ToString();
            bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);
            string SuccessMessage = Request.Query["SuccessMessage"].ToString();
            string ErrorMessage = Request.Query["ErrorMessage"].ToString();

            ViewBag.MenuName = "Accomodation";
            model = new AccomodationViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.PositionId = PositionId;

            #region Get Quote Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = model.MenuViewModel.QRFID;
            model.TourInfoHeaderViewModel = posLibrary.GetSalesTourInfoHeader(ref modelQuote, token);
            #endregion

            positionGetReq = new PositionGetReq();
            positionGetReq.QRFID = QRFID;
            positionGetReq.PositionId = PositionId;
            positionGetReq.ProductType.Add(new ProductType { ProdType = "Hotel" });
            positionGetReq.IsClone = IsClone;

            positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

            model = accomoMapping.AccomodationGet(positionGetRes, ref model, token);

            if (!(string.IsNullOrEmpty(SuccessMessage)))
                TempData["success"] = SuccessMessage;

            if (!(string.IsNullOrEmpty(ErrorMessage)))
                TempData["error"] = ErrorMessage;

            if (string.IsNullOrEmpty(PositionId))
            {
                if (string.IsNullOrEmpty(SuccessMessage) && string.IsNullOrEmpty(ErrorMessage))
                    return View(model);
                else
                    return PartialView("_Accomodation", model);
            }
            else
                return PartialView("_Accomodation", model);
        }
        //public IActionResult GetAccomodationData()
        //{
        //    string QRFID = Convert.ToInt64(Request.Query["QRFId"].ToString());
        //    string PositionId = Request.Query["PositionId"].ToString();
        //    string SuccessMessage = Request.Query["SuccessMessage"].ToString();
        //    string ErrorMessage = Request.Query["ErrorMessage"].ToString();

        //    ViewBag.MenuName = "Accomodation";
        //    model = new AccomodationViewModel();
        //    model.MenuViewModel.QRFID = Convert.ToInt64(QRFID);
        //    model.MenuViewModel.PositionId = PositionId;

        //    positionGetReq = new PositionGetReq();
        //    positionGetReq.QRFID = Convert.ToInt64(QRFId);
        //    positionGetReq.PositionId = PositionId;
        //    positionGetReq.ProductType.Add(new ProductType { ProdType = "Hotel" });

        //    positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

        //    model = accomoMapping.AccomodationGet(positionGetRes, ref model, token);

        //    if(!(string.IsNullOrEmpty(SuccessMessage)))
        //    TempData["success"] = SuccessMessage;

        //    if (!(string.IsNullOrEmpty(ErrorMessage)))
        //        TempData["error"] = ErrorMessage;

        //    return PartialView("_Accomodation", model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Accomodation(AccomodationViewModel model)
        //{
        //    if (model.SaveType == "partial")
        //    {
        //        model.AccomodationData.RemoveAll(x => (x.AccomodationSequence == 0));
        //    }

        //    positionSetReq = new PositionSetReq();
        //    positionSetReq = accomoMapping.AccomodationSet(model, ckUserEmailId);

        //    positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;

        //    if (positionSetRes.ResponseStatus.Status.ToLower() == "success")
        //    {
        //        if (model.SaveType == "partial")
        //        {
        //            return Json(positionSetRes.mPosition[0]);
        //        }
        //        else
        //        {
        //            TempData["success"] = "Accomodation data saved successfully";
        //            return RedirectToAction("Accomodation", new { QRFId = model.MenuViewModel.QRFID });
        //        }
        //    }
        //    else
        //    {
        //        if (model.SaveType == "partial")
        //        {
        //            return Json(positionSetRes.mPosition[0]);
        //        }
        //        else
        //        {
        //            TempData["error"] = positionSetRes.ResponseStatus.ErrorMessage;
        //            return RedirectToAction("Accomodation", new { QRFId = model.MenuViewModel.QRFID });
        //        }
        //    }
        //}

        [HttpPost]
        public JsonResult Accomodation(AccomodationViewModel model)
        {
            positionSetReq = new PositionSetReq();
            

            if (model.SaveType == "partial")
            {
                model.AccomodationData.RemoveAll(x => (x.AccomodationSequence == 0));
            }
            else if (model.SaveType == "full")
            {
                model.AccomodationData.RemoveAll(x => (x.AccomodationSequence == 0 && string.IsNullOrEmpty(x.AccomodationId)));
            }
            bool IsValid = accomoMapping.ValidateAccomodationWithRouting(model, token);
            if (IsValid)
            {
                positionSetReq = accomoMapping.AccomodationSet(model, ckUserEmailId,token);
                positionSetReq.FOC = model.FOC;
                positionSetReq.Price = model.Price;
                positionSetReq.QRFID = model.MenuViewModel.QRFID;
                positionSetReq.VoyagerUserID = ckLoginUser_Id;
                positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;
                if (positionSetRes.ResponseStatus.Status.ToLower() == "success")
                {
                    if (model.SaveType == "partial")
                    {
                        return Json(new { RoomDetailsInfo = positionSetRes.mPosition[0].RoomDetailsInfo, PositionId = positionSetRes.mPosition[0].PositionId });
                    }
                    else
                    {
                        return Json(positionSetRes.ResponseStatus);
                    }
                }
                else
                {
                    if (model.SaveType == "partial")
                    {
                        return Json(new { RoomDetailsInfo = positionSetRes.mPosition[0].RoomDetailsInfo, PositionId = positionSetRes.mPosition[0].PositionId });
                    }
                    else
                    {
                        return Json(positionSetRes.ResponseStatus);
                    }
                }

            }
            else
            {
                return Json(new { status = "Invalid" });
            }

        }

        [HttpPost]
        public JsonResult SetDefaultMealPlan(PositionDefMealSetReq model)
        {
            PositionDefMealSetRes response = new PositionDefMealSetRes();
            model.UserName = ckUserEmailId;
            response = positionProviders.SetDefaultMealPlan(model, token).Result;
            if (response != null && response.ResponseStatus != null)
            {
                return Json(new { status = response.ResponseStatus.Status, msg = response.ResponseStatus.ErrorMessage });
            }
            else
            {
                return Json(new { status = "Error", msg = "Details not updated" });
            }
        }

        //public JsonResult GetCityName(string term)
        //{
        //    List<AutoCompleteTextBox> cityList = new List<AutoCompleteTextBox>();
        //    cityList = accomLibrary.GetCityList(term, token);
        //    return Json(cityList);
        //}

        public JsonResult GetChainName(string term)
        {
            List<AutoCompleteTextBox> chainList = new List<AutoCompleteTextBox>();
            chainList = posLibrary.GetChainList(term, token);
            return Json(chainList);
        }

        public JsonResult GetHotelName(ProductSearchReq objProductSearchReq)
        {
            string strProdType = objProductSearchReq.ProdType[0];
            objProductSearchReq.ProdType = new List<string>();
            objProductSearchReq.ProdType.AddRange(strProdType.Replace("\"", "").Split("|"));

            List<AutoCompleteTextBox> hotelNameList = new List<AutoCompleteTextBox>();
            hotelNameList = posLibrary.GetHotelList(objProductSearchReq, token);
            return Json(hotelNameList);
        }

        public JsonResult GetRoomType(string HotelId, string QRFID)
        {
            PositionGetRoomTypeAndSuppRes roomTypeList = new PositionGetRoomTypeAndSuppRes();
            List<string> HotelIds = new List<string>();
            HotelIds.Add(HotelId);
            roomTypeList = posLibrary.GetRoomTypeAndSupplementList(HotelIds, QRFID, token);
            var roomsuppliments = new List<ProdCategoryRangeDetails>();
            var roomservices = new List<ProdCategoryRangeDetails>();

            roomservices = roomTypeList.DefaultRoomslist.
                OrderBy(a => a.ProductRangeCode.Contains("SINGLE") ? "A" : a.ProductRangeCode.Contains("DOUBLE") ? "B" : a.ProductRangeCode.Contains("TWIN") ? "C" : a.ProductRangeCode.Contains("TRIPLE") ? "D" :
                                   a.ProductRangeCode.Contains("QUAD") ? "E" : a.ProductRangeCode.Contains("TSU") ? "F" :
                                   a.ProductRangeCode.ToLower().Contains("child + bed") ? "G" : a.ProductRangeCode.ToLower().Contains("child - bed") ? "H" :
                                   a.ProductRangeCode.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.ProductRangeCode).ToList();

            roomTypeList.DefaultRoomslist = roomservices;

            var roomsuppliment = new List<ProductRangeDetails>();
            var roomservice = new List<ProductRangeDetails>();

            roomservice = roomTypeList.RoomTypeList.Where(a => a.AdditionalYN == false).ToList().
                 OrderBy(a => a.ProductRangeCode.Contains("SINGLE") ? "A" : a.ProductRangeCode.Contains("DOUBLE") ? "B" : a.ProductRangeCode.Contains("TWIN") ? "C" : a.ProductRangeCode.Contains("TRIPLE") ? "D" :
                                    a.ProductRangeCode.Contains("QUAD") ? "E" : a.ProductRangeCode.Contains("TSU") ? "F" :
                                    a.PersonType.ToLower().Contains("child + bed") ? "G" : a.PersonType.ToLower().Contains("child - bed") ? "H" :
                                    a.ProductRangeCode.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.ProductRangeCode).ToList();

            roomsuppliment = roomTypeList.RoomTypeList.Where(a => a.AdditionalYN == true).OrderBy(a => a.ProductRangeCode).ToList();
            roomservice.AddRange(roomsuppliment);
            roomTypeList.RoomTypeList = roomservice;

            return Json(roomTypeList);
        }

        public ActionResult GetSimilarHotels(SimilarHotelsGetReq request, string HotelName, string CityName, string CountryName, string StarRating, string BudgetCategory, string Location, string EnquiryPipeline)
        {
            SimilarHotelsViewModel model = new SimilarHotelsViewModel();
            SimilarHotelsGetRes response = new SimilarHotelsGetRes();
            response = masterProviders.GetSimilarHotels(request, token).Result;

            model.HotelName = HotelName;
            model.PositionId = request.PositionId;
            model.ProductId = request.ProductId;
            ViewBag.EnquiryPipeline = EnquiryPipeline;
            model.IsClone = request.IsClone;

            if (string.IsNullOrEmpty(CityName) || string.IsNullOrEmpty(CountryName))
                model.HotelDesc = CityName + CountryName;
            else
                model.HotelDesc = CityName + ", " + CountryName;

            if (!string.IsNullOrEmpty(StarRating))
                model.HotelDesc = model.HotelDesc + " - " + StarRating;
            if (!string.IsNullOrEmpty(BudgetCategory))
                model.HotelDesc = model.HotelDesc + " - " + BudgetCategory;
            if (!string.IsNullOrEmpty(Location))
                model.HotelDesc = model.HotelDesc + " - " + Location;

            if (response.ResponseStatus.Status.ToLower() == "success")
            {
                model.SelectedHotelList = response.SelectedHotelList;
                model.BlackListedHotelList = response.BlackListedHotelList;

                foreach (var hotel in model.SelectedHotelList)
                {
                    hotel.VoyagerProductId = hotel.VoyagerProductId + "|" + hotel.SupplierId;
                    hotel.Name = hotel.Name + " (";
                    if (!string.IsNullOrEmpty(hotel.LocationInfo.CityName))
                        hotel.Name = hotel.Name + hotel.LocationInfo.CityName;
                    if (!string.IsNullOrEmpty(hotel.StarRating))
                        hotel.Name = hotel.Name + " ," + hotel.StarRating;
                    if (!string.IsNullOrEmpty(hotel.Category))
                        hotel.Name = hotel.Name + "/" + hotel.Category;
                    if (!string.IsNullOrEmpty(hotel.Location))
                        hotel.Name = hotel.Name + "/" + hotel.Location;
                    if (!string.IsNullOrEmpty(hotel.Supplier))
                        hotel.Name = hotel.Name + "/" + hotel.Supplier;
                    hotel.Name = hotel.Name + ")";
                }
                foreach (var hotel in model.BlackListedHotelList)
                {
                    hotel.VoyagerProductId = hotel.VoyagerProductId + "|" + hotel.SupplierId;
                    hotel.Name = hotel.Name + " (";
                    if (!string.IsNullOrEmpty(hotel.LocationInfo.CityName))
                        hotel.Name = hotel.Name + hotel.LocationInfo.CityName;
                    if (!string.IsNullOrEmpty(hotel.StarRating))
                        hotel.Name = hotel.Name + " ," + hotel.StarRating;
                    if (!string.IsNullOrEmpty(hotel.Category))
                        hotel.Name = hotel.Name + "/" + hotel.Category;
                    if (!string.IsNullOrEmpty(hotel.Location))
                        hotel.Name = hotel.Name + "/" + hotel.Location;
                    if (!string.IsNullOrEmpty(hotel.Supplier))
                        hotel.Name = hotel.Name + "/" + hotel.Supplier;
                    hotel.Name = hotel.Name + ")";
                }
            }
            return PartialView("_SimilarHotels", model);
        }

        [HttpPost]
        public JsonResult SaveSimilarHotels(SimilarHotelsViewModel model)
        {
            SimilarHotelsSetRes response = new SimilarHotelsSetRes();
            SimilarHotelsSetReq request = new SimilarHotelsSetReq();

            request.PositionId = model.PositionId;
            request.ProductId = model.ProductId;
            request.SelectedHotelList = model.SelectedHotelList;
            request.BlacklistedHotelList = model.BlackListedHotelList;
            request.EditUser = ckUserEmailId;
            //request.Caller = "";
            request.IsClone = model.IsClone;
            request.Caller = model.Caller;
            request.BookingNumber = model.BookingNumber;

            response = masterProviders.SetSimilarHotels(request, token).Result;
            if (response != null && response.ResponseStatus != null)
            {
                return Json(new { status = response.ResponseStatus.Status, msg = response.ResponseStatus.ErrorMessage });
            }
            else
            {
                return Json(new { status = "Error", msg = "Details not updated" });
            }
        }
        //public JsonResult GetSupplement(string HotelId)
        //{
        //    List<AutoCompleteTextBox> supplementList = new List<AutoCompleteTextBox>();
        //    supplementList = accomLibrary.GetSupplementList(HotelId, token);
        //    return Json(supplementList);
        //}

        //[AcceptVerbs("Get", "HttpPost")]
        //public IActionResult ValidateEarlyCheckInDate()
        //{
        //    string[] a = Request.QueryString.Value.Split("EarlyCheckInDate=");
        //    if (!(string.IsNullOrEmpty(a[1])))
        //    {
        //        return Json(data: $"*");
        //    }

        //    return Json(data: true);
        //}
    }
}