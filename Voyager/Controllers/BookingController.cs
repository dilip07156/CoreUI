using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class BookingController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private BookingProviders bookingProviders;
        private SalesProviders objSalesProvider;
        private MasterProviders objMasterProviders;

        public BookingController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            bookingProviders = new BookingProviders(_configuration);
            objSalesProvider = new SalesProviders(_configuration);
            objMasterProviders = new MasterProviders(_configuration);
        }

        public IActionResult Booking()
        {
            BookingSearchViewModel model = new BookingSearchViewModel();
            ViewBag.SearchResult = false;

            #region Dropdown Binding
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "BookingSearchDateType";
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            #endregion

            #region Get Booking Details 
            BookingSearchRes response = new BookingSearchRes();
            response = bookingProviders.GetBookingStatusList(token).Result;
            #endregion

            model.DateTypeList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList();
            model.BookingStatusList = response.BookingStatusList;
            return View(model);
        }

        public IActionResult _BookingPipeline()
        {
            var model = new BookingPipelineViewModel();
            return PartialView("_BookingPipeline", model);
        }

        [HttpPost]
        public ActionResult LoadData(BookingSearchViewModel searchFilters, int draw, int start, int length)
        {
            #region Get Booking Details 
            BookingSearchReq request = new BookingSearchReq();
            BookingSearchRes response = new BookingSearchRes();
            request.AgentName = searchFilters.AgentName;
            request.AgentCode = searchFilters.AgentCode;
            request.BookingNumber = Convert.ToString(searchFilters.CNKReferenceNo);
            request.BookingName = searchFilters.AgentTour;
            request.Status = searchFilters.BookingStatus;
            request.DateType = searchFilters.DateType;
            request.From =searchFilters.From;
            request.To = searchFilters.To;
            request.UserId = ckLoginUser_Id;
            request.Start = start;
            if (length == 0)
                length = 10;
            request.Length = length;
            response = bookingProviders.GetSearchBookingDetails(request, token).Result;
            #endregion

            return Json(new
            {
                draw = draw,
                recordsTotal = response.BookingTotalCount,
                recordsFiltered = response.BookingTotalCount,
                data = response.Bookings
            });
        }

        [HttpPost]
        public IActionResult Booking(BookingSearchViewModel model)
        {
            //#region Get Booking Details 
            //BookingSearchReq request = new BookingSearchReq();
            //BookingSearchRes response = new BookingSearchRes();
            //request.AgentName = model.AgentName;
            //request.AgentCode = model.AgentCode;
            //request.BookingNumber = Convert.ToString(model.CNKReferenceNo);
            //request.BookingName = model.AgentTour;
            //request.Status = model.BookingStatus;
            //request.DateType = model.DateType;
            //request.From = Convert.ToString(model.From);
            //request.To = Convert.ToString(model.To);
            //response = bookingProviders.GetSearchBookingDetails(request, token).Result;
            //#endregion

            #region Dropdown Binding
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "BookingSearchDateType";
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            #endregion

            #region Get Booking Details 
            BookingSearchRes response1 = new BookingSearchRes();
            response1 = bookingProviders.GetBookingStatusList(token).Result;
            #endregion

            model.DateTypeList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList(); 
            model.BookingStatusList = response1.BookingStatusList;

            //model.BookingPipeline.BookingList = response.Bookings;
            //ViewBag.SearchResult = true;
            return View(model);

        }

        //public IActionResult ViewBooking(string BookingNo, string BookingName)
        //{
        //	BookingPipelineViewModel model = new BookingPipelineViewModel();

        //	#region Get Booking Details by Booking Number
        //	BookingDetailReq request = new BookingDetailReq();
        //	BookingDetailRes response = new BookingDetailRes();
        //	request.BookingReference = BookingNo;
        //	response = bookingProviders.GetAllBookingData(request, token).Result;
        //	#endregion

        //	#region Get Booking Room Details by Booking Number
        //	BookingSearchReq req = new BookingSearchReq();
        //	BookingSearchRes res = new BookingSearchRes();
        //	req.BookingNumber = BookingNo;
        //	res = bookingProviders.GetBookingRoomDetails(req, token).Result;
        //	#endregion

        //	#region Header binding

        //	var header = BookingNo + " - " + BookingName;
        //	model.Header = header;

        //	#endregion

        //	#region Hotel binding

        //	List<BookingPositionViewModel> lstHotelposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var hotelpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "HOTEL" || a.ProductType.ToUpper() == "OVERNIGHT FERRY").ToList();
        //		var positionIds = hotelpositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId) && x.PersonType == "ADULT").Select(y => new { y.PositionId, y.RoomNo, y.ProductTemplate, y.PersonType }).ToList();

        //		foreach (var hotel in hotelpositions)
        //		{
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = hotel.StartDate;
        //			pos.Country = hotel.Country;
        //			pos.City = hotel.City;
        //			pos.ProductName = hotel.ProductName;
        //			pos.Board = hotel.MealPlan;
        //			pos.Status = hotel.Status;
        //			pos.Single = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "SINGLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
        //			pos.Triple = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "TRIPLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
        //			pos.Double = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "DOUBLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
        //			pos.Twin = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "TWIN").Select(x => x.RoomNo).FirstOrDefault() ?? "";

        //			lstHotelposition.Add(pos);
        //		}
        //		model.HotelPositions = lstHotelposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.HotelPositions = new List<BookingPositionViewModel>();
        //	}
        //	#endregion

        //	ProductRangeGetReq prodRangeGetReq = new ProductRangeGetReq();
        //	ProductRangeGetRes prodRangeGetRes = new ProductRangeGetRes();
        //	ProductCatGetReq prodCatGetReq = new ProductCatGetReq();
        //	ProductCatGetRes prodCatGetRes = new ProductCatGetRes();

        //	#region Restaurant binding
        //	List<BookingPositionViewModel> lstMealposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var mealpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "MEAL").ToList(); //BookingPositions
        //		var positionIds = mealpositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId) && x.PersonType == "ADULT").Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId }).ToList(); //BookingRooms

        //		#region Get Product Range by Product Id list
        //		var productRangeIds = rooms.Select(x => x.ProductRangeId).ToList();
        //		prodRangeGetReq.ProductRangeIdList = productRangeIds;
        //		prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
        //		var lstrange = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.ProductCategoryId, y.ProductMenu, y.VoyagerProductRange_Id, y.ProductRangeName });
        //		#endregion

        //		#region Get Product Category by Product Category Id list
        //		var productCategoryIds = lstrange.Select(x => x.ProductCategoryId).ToList();
        //		prodCatGetReq.ProdCatIdList = productCategoryIds;
        //		prodCatGetRes = objMasterProviders.GetProductCategoryByParam(prodCatGetReq, token).Result;
        //		var lstCategory = prodCatGetRes.ProdCategoryDetails.Select(y => new { y.ProductCategoryId, y.ProductCategoryName });
        //		#endregion

        //		foreach (var meal in mealpositions)
        //		{
        //			var prodRangeId = rooms.Where(x => x.PositionId == meal.Position_Id).Select(y => y.ProductRangeId).FirstOrDefault() ?? "";
        //			var prodCatId = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductCategoryId).FirstOrDefault() ?? "";
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = meal.StartDate;
        //			pos.Country = meal.Country;
        //			pos.City = meal.City;
        //			pos.ProductName = meal.ProductName;
        //			pos.Status = meal.Status;
        //			pos.RestaurantType = lstCategory.Where(x => x.ProductCategoryId == prodCatId).Select(y => y.ProductCategoryName).FirstOrDefault() ?? "";
        //			pos.Meal = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
        //			pos.Menu = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductMenu).FirstOrDefault() ?? "";

        //			lstMealposition.Add(pos);
        //		}
        //		model.MealPositions = lstMealposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.MealPositions = new List<BookingPositionViewModel>();
        //	}

        //	#endregion

        //	#region Attractions binding
        //	List<BookingPositionViewModel> lstAttractionposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var attractionpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "ATTRACTIONS" || a.ProductType.ToUpper() == "SIGHTSEEING - CITYTOUR").ToList();
        //		var positionIds = attractionpositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId)).Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId }).ToList(); //BookingRooms

        //		#region Get Product Range by Product Id list
        //		var productRangeIds = rooms.Select(x => x.ProductRangeId).ToList();
        //		prodRangeGetReq.ProductRangeIdList = productRangeIds;
        //		prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
        //		var lstrange = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
        //		#endregion

        //		foreach (var attr in attractionpositions)
        //		{
        //			var prodRangeId = rooms.Where(x => x.PositionId == attr.Position_Id).Select(y => y.ProductRangeId).FirstOrDefault() ?? "";
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = attr.StartDate;
        //			pos.Country = attr.Country;
        //			pos.City = attr.City;
        //			pos.ProductName = attr.ProductName;
        //			pos.Status = attr.Status;
        //			pos.Option = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";

        //			lstAttractionposition.Add(pos);
        //		}
        //		model.AttractionPositions = lstAttractionposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.AttractionPositions = new List<BookingPositionViewModel>();
        //	}
        //	#endregion

        //	#region Transfer binding

        //	List<BookingPositionViewModel> lstTransferposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var transferpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "LDC" || a.ProductType.ToUpper() == "COACH" || a.ProductType.ToUpper() == "PRIVATE TRANSFER" || a.ProductType.ToUpper() == "SCHEDULED TRANSFER" || a.ProductType.ToUpper() == "FERRY TRANSFER" || a.ProductType.ToUpper() == "FERRY PASSENGER" || a.ProductType.ToUpper() == "TRAIN" || a.ProductType.ToUpper() == "DOMESTIC FLIGHT").ToList();
        //		var positionIds = transferpositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId)).Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId, y.BookingRoomId }).ToList(); //BookingRooms
        //		var bookingRoomIds = rooms.Select(x => x.BookingRoomId).ToList();

        //		#region Get Booking Position Pricing by Booking number
        //		BookingSearchReq reqbook = new BookingSearchReq();
        //		BookingSearchRes resbook = new BookingSearchRes();
        //		reqbook.BookingNumber = BookingNo;
        //		reqbook.BookingRoomId = bookingRoomIds;
        //		resbook = bookingProviders.GetBookingPositionPricingDetails(reqbook, token).Result;
        //		var lstprice = resbook.BookingPositionPricing.Select(y => new { y.BookingRoomId, y.PositionId, y.ProductTemplate });
        //		#endregion

        //		foreach (var trans in transferpositions)
        //		{
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = trans.StartDate;
        //			pos.EndDate = trans.EndDate;
        //			pos.ProductType = trans.ProductType;
        //			pos.ProductName = trans.ProductName;
        //			pos.City = trans.City;
        //			//Add ToCity
        //			pos.Unit = lstprice.Where(x => x.PositionId == trans.Position_Id).Select(y => y.ProductTemplate).FirstOrDefault() ?? "";
        //			pos.PickUpLoc = trans.PickupPoint;
        //			pos.DropOffLoc = trans.DropOffPoint;
        //			pos.Driver = trans.DriverName + " " + trans.DriverContact;
        //			pos.Status = trans.Status;

        //			lstTransferposition.Add(pos);
        //		}
        //		model.TransferPositions = lstTransferposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.TransferPositions = new List<BookingPositionViewModel>();
        //	}
        //	#endregion

        //	#region Guide and Assistant binding
        //	List<BookingPositionViewModel> lstGuideposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var guidepositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "GUIDE" || a.ProductType.ToUpper() == "ASSISTANT").ToList();
        //		var positionIds = guidepositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId)).Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId }).ToList(); //BookingRooms

        //		#region Get Product Range by Product Id list
        //		var productRangeIds = rooms.Select(x => x.ProductRangeId).ToList();
        //		prodRangeGetReq.ProductRangeIdList = productRangeIds;
        //		prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
        //		var lstrange = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
        //		#endregion

        //		foreach (var guide in guidepositions)
        //		{
        //			var prodRangeId = rooms.Where(x => x.PositionId == guide.Position_Id).Select(y => y.ProductRangeId).FirstOrDefault() ?? "";
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = guide.StartDate;
        //			pos.Country = guide.Country;
        //			pos.City = guide.City;
        //			pos.ProductName = guide.ProductName;
        //			pos.Option = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
        //			pos.StartTime = guide.StartTime;
        //			pos.EndTime = guide.EndTime;
        //			pos.Driver = guide.DriverName + " " + guide.DriverContact;
        //			pos.Status = guide.Status;

        //			lstGuideposition.Add(pos);
        //		}
        //		model.GuidePositions = lstGuideposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.GuidePositions = new List<BookingPositionViewModel>();
        //	}
        //	#endregion

        //	#region Others binding
        //	List<BookingPositionViewModel> lstOtherposition = new List<BookingPositionViewModel>();
        //	if (response != null && response.Booking != null && response.Booking.Services != null && res != null && res.BookingRooms != null)
        //	{
        //		var otherpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "VISA" || a.ProductType.ToUpper() == "INSURANCE" || a.ProductType.ToUpper() == "OTHER").ToList();
        //		var positionIds = otherpositions.Select(x => x.Position_Id).ToList();
        //		var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId)).Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId }).ToList(); //BookingRooms

        //		#region Get Product Range by Product Id list
        //		var productRangeIds = rooms.Select(x => x.ProductRangeId).ToList();
        //		prodRangeGetReq.ProductRangeIdList = productRangeIds;
        //		prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
        //		var lstrange = prodRangeGetRes.ProductRangeDetails.Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
        //		#endregion

        //		foreach (var other in otherpositions)
        //		{
        //			var prodRangeId = rooms.Where(x => x.PositionId == other.Position_Id).Select(y => y.ProductRangeId).FirstOrDefault() ?? "";
        //			var pos = new BookingPositionViewModel();
        //			pos.StartDate = other.StartDate;
        //			pos.Country = other.Country;
        //			pos.City = other.City;
        //			pos.ProductName = other.ProductName;
        //			pos.Option = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
        //			pos.Status = other.Status;

        //			lstOtherposition.Add(pos);
        //		}
        //		model.OtherPositions = lstOtherposition.OrderBy(x => x.StartDate).ToList();
        //	}
        //	else
        //	{
        //		model.OtherPositions = new List<BookingPositionViewModel>();
        //	}
        //	#endregion

        //	return PartialView("_ViewBooking", model);
        //}

        public IActionResult ViewBooking(string BookingNo)
        {
            BookingPipelineViewModel model = new BookingPipelineViewModel();

            if (!string.IsNullOrEmpty(BookingNo))
            {
                #region Get Booking Details by Booking Number
                BookingDetailReq request = new BookingDetailReq();
                BookingInfoRes response = new BookingInfoRes();
                request.BookingNumber = BookingNo;
                request.UserName = ckUserEmailId;
                request.VoygerCompany_Id = ckUserCompanyId;
                response = bookingProviders.GetBookingDetailsByParam(request, token).Result;
                #endregion

                #region Header binding
                var header = BookingNo + " - " + response.Bookings.CustRef;
                model.Header = header;
                #endregion

                if (response.Bookings?.Positions?.Count > 0 && response.Bookings?.BookingRooms?.Count > 0)
                {
                    ProductRangeGetRes prodRangeGetRes = new ProductRangeGetRes();
                    ProductCatGetReq prodCatGetReq = new ProductCatGetReq();
                    ProductCatGetRes prodCatGetRes = new ProductCatGetRes();

                    #region Hotel binding
                    List<BookingPositionViewModel> lstHotelposition = new List<BookingPositionViewModel>();
                    var hotelpositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "HOTEL" || a.ProductType.ToUpper() == "OVERNIGHT FERRY").ToList();
                    foreach (var hotel in hotelpositions)
                    {
                        var pos = new BookingPositionViewModel();
                        var adultRooms = hotel.BookingRoomsAndPrices.Where(a => a.PersonType == "ADULT").ToList();
                        pos.StartDate = hotel.STARTDATE != null ? hotel.STARTDATE.ToString() : "";
                        pos.Country = hotel.Country;
                        pos.City = hotel.City;
                        pos.ProductName = hotel.Product_Name;
                        pos.Board = hotel.HOTELMEALPLAN;
                        pos.Status = hotel.STATUS;
                        pos.Single = adultRooms.Where(x => x.RoomName == "SINGLE").Select(x => Convert.ToString(x.Req_Count)).FirstOrDefault() ?? "";
                        pos.Triple = adultRooms.Where(x => x.RoomName == "TRIPLE").Select(x => Convert.ToString(x.Req_Count)).FirstOrDefault() ?? "";
                        pos.Double = adultRooms.Where(x => x.RoomName == "DOUBLE").Select(x => Convert.ToString(x.Req_Count)).FirstOrDefault() ?? "";
                        pos.Twin = adultRooms.Where(x => x.RoomName == "TWIN").Select(x => Convert.ToString(x.Req_Count)).FirstOrDefault() ?? "";

                        lstHotelposition.Add(pos);
                    }
                    model.HotelPositions = lstHotelposition.OrderBy(x => x.StartDate).ToList();
                    #endregion

                    #region Restaurant binding
                    List<BookingPositionViewModel> lstMealposition = new List<BookingPositionViewModel>();
                    var mealpositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "MEAL").ToList(); //BookingPositions 

                    #region Get Product Range by Product Id list
                    ProductRangeGetReq prodRangeGetReqRestaurant = new ProductRangeGetReq() { ProductRangeIdList=new List<string>()};
                    mealpositions.SelectMany(a => a.BookingRoomsAndPrices).ToList().ForEach(a => prodRangeGetReqRestaurant.ProductRangeIdList.Add(a.ProductRange_Id));
                    if (prodRangeGetReqRestaurant.ProductRangeIdList?.Count > 0)
                    {
                        prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReqRestaurant, token).Result;
                    }
                    else
                    {
                        prodRangeGetRes = new ProductRangeGetRes();
                    }
                    var lstrangeMeal = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.ProductCategoryId, y.ProductMenu, y.VoyagerProductRange_Id, y.ProductRangeName });
                    #endregion

                    #region Get Product Category by Product Category Id list
                    var productCategoryIds = lstrangeMeal.Select(x => x.ProductCategoryId).ToList();
                    if (productCategoryIds?.Count > 0)
                    {
                        prodCatGetReq.ProdCatIdList = productCategoryIds;
                        prodCatGetRes = objMasterProviders.GetProductCategoryByParam(prodCatGetReq, token).Result;
                    }
                    else
                    {
                        prodCatGetRes = new ProductCatGetRes();
                    }
                    var lstCategory = prodCatGetRes.ProdCategoryDetails.Select(y => new { y.ProductCategoryId, y.ProductCategoryName });
                    #endregion

                    foreach (var meal in mealpositions)
                    {
                        var mealrooms = meal.BookingRoomsAndPrices.Where(a => a.PersonType == "ADULT").ToList();
                        var prodRangeId = mealrooms.Select(y => y.ProductRange_Id).FirstOrDefault() ?? "";
                        var prodCatId = lstrangeMeal.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductCategoryId).FirstOrDefault() ?? "";
                        var pos = new BookingPositionViewModel();
                        pos.StartDate = meal.STARTDATE != null ? meal.STARTDATE.ToString() : "";
                        pos.Country = meal.Country;
                        pos.City = meal.City;
                        pos.ProductName = meal.Product_Name;
                        pos.Status = meal.STATUS;
                        pos.RestaurantType = lstCategory.Where(x => x.ProductCategoryId == prodCatId).Select(y => y.ProductCategoryName).FirstOrDefault() ?? "";
                        pos.Meal = lstrangeMeal.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
                        pos.Menu = lstrangeMeal.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductMenu).FirstOrDefault() ?? "";

                        lstMealposition.Add(pos);
                    }
                    model.MealPositions = lstMealposition.OrderBy(x => x.StartDate).ToList();
                    #endregion

                    #region Attractions binding
                    List<BookingPositionViewModel> lstAttractionposition = new List<BookingPositionViewModel>();
                    var attractionpositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "ATTRACTIONS" || a.ProductType.ToUpper() == "SIGHTSEEING - CITYTOUR").ToList();

                    #region Get Product Range by Product Id list 
                    ProductRangeGetReq prodRangeGetReqAttractions = new ProductRangeGetReq() { ProductRangeIdList = new List<string>() };
                    attractionpositions.SelectMany(a => a.BookingRoomsAndPrices).ToList().ForEach(a => prodRangeGetReqAttractions.ProductRangeIdList.Add(a.ProductRange_Id));
                    if (prodRangeGetReqAttractions.ProductRangeIdList?.Count > 0)
                    {
                        prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReqAttractions, token).Result;
                    }
                    else
                    {
                        prodRangeGetRes = new ProductRangeGetRes();
                    }
                    var lstrangeAttraction = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
                    #endregion

                    foreach (var attr in attractionpositions)
                    {
                        var prodRangeId = attr.BookingRoomsAndPrices.Select(y => y.ProductRange_Id).FirstOrDefault() ?? "";
                        var pos = new BookingPositionViewModel();
                        pos.StartDate = attr.STARTDATE != null ? attr.STARTDATE.ToString() : "";
                        pos.Country = attr.Country;
                        pos.City = attr.City;
                        pos.ProductName = attr.Product_Name;
                        pos.Status = attr.STATUS;                         
                        pos.Option = lstrangeAttraction.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
                        lstAttractionposition.Add(pos);
                    }
                    model.AttractionPositions = lstAttractionposition.OrderBy(x => x.StartDate).ToList();
                    #endregion

                    #region Transfer binding
                    List<BookingPositionViewModel> lstTransferposition = new List<BookingPositionViewModel>();
                    var transferpositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "LDC" || a.ProductType.ToUpper() == "COACH" || a.ProductType.ToUpper() == "PRIVATE TRANSFER" || a.ProductType.ToUpper() == "SCHEDULED TRANSFER" || a.ProductType.ToUpper() == "FERRY TRANSFER" || a.ProductType.ToUpper() == "FERRY PASSENGER" || a.ProductType.ToUpper() == "TRAIN" || a.ProductType.ToUpper() == "DOMESTIC FLIGHT").ToList();
                    var rooms = transferpositions.SelectMany(a => a.BookingRoomsAndPrices).Select(y => new { y.RoomName, y.ProductRange_Id, y.ProductTemplate_Id, y.BookingRooms_Id }).ToList(); //BookingRooms
                    var bookingRoomIds = rooms.Select(x => x.BookingRooms_Id).ToList();

                    #region Get Booking Position Pricing by Booking number
                    BookingSearchReq reqbook = new BookingSearchReq();
                    BookingSearchRes resbook = new BookingSearchRes();
                    reqbook.BookingNumber = BookingNo;
                    reqbook.BookingRoomId = bookingRoomIds;
                    if (bookingRoomIds?.Count > 0)
                    {
                        resbook = bookingProviders.GetBookingPositionPricingDetails(reqbook, token).Result;
                    }
                    else
                    {
                        resbook = new BookingSearchRes() { BookingPositionPricing = new List<BookingList>() };
                    }
                    var lstprice = resbook.BookingPositionPricing.Select(y => new { y.BookingRoomId, y.PositionId, y.ProductTemplate });
                    #endregion

                    foreach (var trans in transferpositions)
                    {
                        var pos = new BookingPositionViewModel();
                        pos.StartDate = trans.STARTDATE != null ? trans.STARTDATE.ToString() : "";
                        pos.EndDate = trans.STARTDATE != null ? trans.STARTDATE.ToString() : "";
                        pos.ProductType = trans.ProductType;
                        pos.ProductName = trans.Product_Name;
                        pos.City = trans.City;
                        //Add ToCity
                        pos.Unit = lstprice.Where(x => x.PositionId == trans.Position_Id).Select(y => y.ProductTemplate).FirstOrDefault() ?? "";
                        pos.PickUpLoc = trans.STARTLOC;
                        pos.DropOffLoc = trans.ENDLOC;
                        pos.Driver = trans.DriverName + " " + trans.DriverContactNumber;
                        pos.Status = trans.STATUS;
                        lstTransferposition.Add(pos);
                    }
                    model.TransferPositions = lstTransferposition.OrderBy(x => x.StartDate).ToList();
                    #endregion

                    #region Guide and Assistant binding
                    List<BookingPositionViewModel> lstGuideposition = new List<BookingPositionViewModel>();
                    var guidepositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "GUIDE" || a.ProductType.ToUpper() == "ASSISTANT").ToList();

                    #region Get Product Range by Product Id list
                    ProductRangeGetReq prodRangeGetReqGuide = new ProductRangeGetReq() { ProductRangeIdList = new List<string>() };
                    guidepositions.SelectMany(a => a.BookingRoomsAndPrices).ToList().ForEach(a => prodRangeGetReqGuide.ProductRangeIdList.Add(a.ProductRange_Id));
                    if (prodRangeGetReqGuide.ProductRangeIdList?.Count > 0)
                    {
                        prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReqGuide, token).Result;
                    }
                    else
                    {
                        prodRangeGetRes = new ProductRangeGetRes();
                    }
                    var lstrangeGuide = prodRangeGetRes.ProductRangeDetails.Where(x => x.AdditionalYN == false).Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
                    #endregion

                    foreach (var guide in guidepositions)
                    {
                        var prodRangeId = guide.BookingRoomsAndPrices.Select(y => y.ProductRange_Id).FirstOrDefault() ?? "";
                        var pos = new BookingPositionViewModel();
                        pos.StartDate = guide.STARTDATE != null ? guide.STARTDATE.ToString() : "";
                        pos.Country = guide.Country;
                        pos.City = guide.City;
                        pos.ProductName = guide.Product_Name;
                        pos.Option = lstrangeGuide.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
                        pos.StartTime = guide.STARTTIME;
                        pos.EndTime = guide.ENDTIME;
                        pos.Driver = guide.DriverName + " " + guide.DriverContactNumber;
                        pos.Status = guide.STATUS;
                        lstGuideposition.Add(pos);
                    }
                    model.GuidePositions = lstGuideposition.OrderBy(x => x.StartDate).ToList();
                    #endregion

                    #region Others binding
                    List<BookingPositionViewModel> lstOtherposition = new List<BookingPositionViewModel>();
                    var otherpositions = response.Bookings.Positions.Where(a => a.ProductType.ToUpper() == "VISA" || a.ProductType.ToUpper() == "INSURANCE" || a.ProductType.ToUpper() == "OTHER"
                                         || a.ProductType.ToUpper() == "FEE").ToList();

                    #region Get Product Range by Product Id list
                    ProductRangeGetReq prodRangeGetReqOthers = new ProductRangeGetReq() { ProductRangeIdList = new List<string>() };
                    otherpositions.SelectMany(a => a.BookingRoomsAndPrices).ToList().ForEach(a => prodRangeGetReqOthers.ProductRangeIdList.Add(a.ProductRange_Id));
                    if (prodRangeGetReqOthers.ProductRangeIdList?.Count > 0)
                    {
                        prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReqOthers, token).Result;
                    }
                    else
                    {
                        prodRangeGetRes = new ProductRangeGetRes();
                    }
                    var lstrangeOthers = prodRangeGetRes.ProductRangeDetails.Select(y => new { y.VoyagerProductRange_Id, y.ProductRangeName });
                    #endregion

                    foreach (var other in otherpositions)
                    {
                        var prodRangeId = other.BookingRoomsAndPrices.Select(y => y.ProductRange_Id).FirstOrDefault() ?? "";
                        var pos = new BookingPositionViewModel();
                        pos.StartDate = other.STARTDATE != null ? other.STARTDATE.ToString() : "";
                        pos.Country = other.Country;
                        pos.City = other.City;
                        pos.ProductName = other.Product_Name;
                        pos.Option = lstrangeOthers.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductRangeName).FirstOrDefault() ?? "";
                        pos.Status = other.STATUS;
                        lstOtherposition.Add(pos);
                    }
                    model.OtherPositions = lstOtherposition.OrderBy(x => x.StartDate).ToList();
                    #endregion
                }
                else
                {
                    model.HotelPositions = new List<BookingPositionViewModel>();
                    model.MealPositions = new List<BookingPositionViewModel>();
                    model.AttractionPositions = new List<BookingPositionViewModel>();
                    model.TransferPositions = new List<BookingPositionViewModel>();
                    model.GuidePositions = new List<BookingPositionViewModel>();
                    model.OtherPositions = new List<BookingPositionViewModel>();
                }
            }
            else
            {
                model.HotelPositions = new List<BookingPositionViewModel>();
                model.MealPositions = new List<BookingPositionViewModel>();
                model.AttractionPositions = new List<BookingPositionViewModel>();
                model.TransferPositions = new List<BookingPositionViewModel>();
                model.GuidePositions = new List<BookingPositionViewModel>();
                model.OtherPositions = new List<BookingPositionViewModel>();
            }

            return PartialView("_ViewBooking", model);
        }

        public IActionResult DocumentDownload(BookingDocDownloadViewModel model)
        {
            BookingDocumentGetReq request = new BookingDocumentGetReq();
            BookingDocumentGetRes response = new BookingDocumentGetRes();

            #region Get Booking Document Details by Type Itinerary
            request.Type = model.Type;
            request.Booking_Id = model.BookingId;
            response = bookingProviders.GetBookingDocumentDetails(request, token).Result;
            if (response != null)
            {
                model.DocumentId = response.Document_Id;
                model.FilePath = Path.GetFileName(response.File_Path);
                model.FileCreationDate = response.FileCreationDate;
            }
            #endregion

            if (model.Type == "Itinerary")
            {
                #region Get Booking Document Details by Type Final Itinerary
                request.Type = "FinalItinerary";
                request.Booking_Id = model.BookingId;
                var res = bookingProviders.GetBookingDocumentDetails(request, token).Result;
                if (res != null)
                {
                    model.FinalItineraryDocumentId = res.Document_Id;
                    model.FinalItineraryFilePath = Path.GetFileName(res.File_Path);
                    model.FinalItineraryFileCreationDate = res.FileCreationDate;
                }
                #endregion
            }

            return PartialView("_DocumentDownload", model);
        }

        public IActionResult GenerateBookingDocument(string BookingNo, string BookingId, string type)
        {
            try
            {
                #region Get Booking Document Details by Type
                BookingDocumentGetReq request = new BookingDocumentGetReq();
                BookingDocumentGetRes response = new BookingDocumentGetRes();
                BookingDocumentSetReq req = new BookingDocumentSetReq();
                BookingDocumentSetRes res = new BookingDocumentSetRes();
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                string userid = "";
                string username = "";
                userid = HttpContext.Request.Cookies["VoyagerUser_Id"] ?? ckLoginUser_Id;
                username = HttpContext.Request.Cookies["UserName"] ?? ckUserName;

                request.Booking_Id = BookingId;
                request.User = "apiuser";
                //request.User = userid;

                if (type == "Itinerary")
                {
                    response = bookingProviders.GenerateItinerary(request).Result;
                    if (response != null)
                    {
                        req.documents.BookingId = BookingId;
                        req.documents.BookingNumber = BookingNo;
                        req.documents.Type = type;
                        req.documents.DocumentId = response.Document_Id;
                        req.documents.FilePath = response.File_Path;
                        req.documents.CreateUser = username;
                    }
                    else
                        return Json(new { responseText = "File not found", message = "error" });

                    var result = bookingProviders.SetBookingDocumentDetails(req, token).Result;
                    if (result != null)
                    {
                        var documentId = response.Document_Id;
                        var path = string.IsNullOrEmpty(response.File_Path) ? "" : response.File_Path;
                        var date = DateTime.Now.ToString("dd MMM yyyy");
                        return Json(new { responseText = "Skeleton Itinerary Generated Successfully", documentId = documentId, path = path, date = date });
                    }
                    else
                        return Json(new { responseText = "Error in Skeleton Itinerary Generation", message = "error" });
                }
                else if (type == "FinalItinerary")
                {
                    response = bookingProviders.GenerateFinalItinerary(request).Result;
                    if (response != null)
                    {
                        req.documents.BookingId = BookingId;
                        req.documents.BookingNumber = BookingNo;
                        req.documents.Type = type;
                        req.documents.DocumentId = response.Document_Id;
                        req.documents.FilePath = response.File_Path;
                        req.documents.CreateUser = username;
                    }
                    else
                        return Json(new { responseText = "File not found", message = "error" });

                    var result = bookingProviders.SetBookingDocumentDetails(req, token).Result;
                    if (result != null)
                    {
                        var documentId = response.Document_Id;
                        var path = string.IsNullOrEmpty(response.File_Path) ? "" : response.File_Path;
                        var date = DateTime.Now.ToString("dd MMM yyyy");
                        return Json(new { responseText = "Final Itinerary Generated Successfully", documentId = documentId, path = path, date = date });
                    }
                    else
                        return Json(new { responseText = "Error in Final Itinerary Generation", message = "error" });
                }
                else if (type == "Vouchers")
                {
                    response = bookingProviders.GenerateVoucher(request).Result;

                    if (response != null)
                    {
                        req.documents.BookingId = BookingId;
                        req.documents.BookingNumber = BookingNo;
                        req.documents.Type = type;
                        req.documents.CreateUser = username;
                        req.documents.DocumentId = response.Document_Id;
                        req.documents.FilePath = response.File_Path;
                    }
                    else
                        return Json(new { responseText = "File not found", message = "error" });

                    var result = bookingProviders.SetBookingDocumentDetails(req, token).Result;
                    if (result != null)
                    {
                        var fullpath = string.IsNullOrEmpty(response.File_Path) ? "" : response.File_Path;
                        var path = string.IsNullOrEmpty(response.File_Path) ? "" : Path.GetFileName(response.File_Path);
                        var date = DateTime.Now.ToString("dd MMM yyyy");
                        return Json(new { responseText = "Voucher Generated Successfully", fullPath = fullpath, path = path, date = date });
                    }
                    else
                        return Json(new { responseText = "Error in Voucher Generation", message = "error" });
                }
                else if (type == "Invoice")
                {
                    response = bookingProviders.GenerateInvoice(request).Result;
                    if (response != null)
                    {
                        req.documents.BookingId = BookingId;
                        req.documents.BookingNumber = BookingNo;
                        req.documents.Type = type;
                        req.documents.DocumentId = response.Document_Id;
                        req.documents.FilePath = response.File_Path;
                        req.documents.CreateUser = username;
                    }
                    else
                        return Json(new { responseText = "File not found", message = "error" });

                    var result = bookingProviders.SetBookingDocumentDetails(req, token).Result;
                    if (result != null)
                    {
                        if (!response.File_Path.Contains("http"))
                            return Json(new { responseText = response.File_Path, message = "error" });
                        else
                        {
                            if (response.File_Path != null)
                            {
                                {
                                    string rootPath = _configuration.GetValue<string>("SystemSettings:BookingDocFilePath");
                                    string PDFPath = response.File_Path;
                                    string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), rootPath, Path.GetFileName(response.File_Path));
                                    if (!System.IO.File.Exists(outputFilePath))
                                    {
                                        if (!Directory.Exists(Path.GetDirectoryName(outputFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                                        WebRequest req1 = WebRequest.Create(PDFPath);
                                        try
                                        {
                                            using (WebResponse resp = req1.GetResponse())
                                            {
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            return Json(new { responseText = "Please contact Accounts Department to generate the Invoice Document", message = "error" });
                                        }

                                    }
                                }
                            }
                            var documentId = response.Document_Id;
                            var fullpath = string.IsNullOrEmpty(response.File_Path) ? "" : response.File_Path;
                            var path = string.IsNullOrEmpty(response.File_Path) ? "" : Path.GetFileName(response.File_Path);
                            var date = DateTime.Now.ToString("dd MMM yyyy");
                            return Json(new { responseText = "Invoice Generated Successfully", documentId = documentId, fullPath = fullpath, path = path, date = date });
                        }
                    }
                    else
                        return Json(new { responseText = "Error in Invoice Generation", message = "error" });


                }
                else
                    return Json(new { responseText = "failure", message = "error" });
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> DownloadBookingDocument(string generateddocId, string bookingId, string type, string fullDocPath)
        {
            try
            {
                #region Get Booking Document Details
                BookingDocumentGetReq request = new BookingDocumentGetReq();
                BookingDocumentGetRes response = new BookingDocumentGetRes();
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                string userid = "";
                userid = HttpContext.Request.Cookies["VoyagerUser_Id"] ?? ckLoginUser_Id;

                if (type == "Itinerary" || type == "FinalItinerary")
                    request.Document_Id = generateddocId;
                else
                    request.Booking_Id = bookingId;

                request.User = "apiuser";
                //request.User = userid;

                if (type == "Itinerary")
                    response = bookingProviders.DownloadItinerary(request).Result;
                else if (type == "FinalItinerary")
                    response = bookingProviders.DownloadFinalItinerary(request).Result;
                else if (type == "Vouchers")
                    response.File_Path = fullDocPath;
                else if (type == "Invoice")
                    response.File_Path = fullDocPath;
                else
                    return Json(new { responseText = "failure", message = "error" });
                #endregion

                if (response != null && response.File_Path != null)
                {
                    string rootPath = _configuration.GetValue<string>("SystemSettings:BookingDocFilePath");
                    byte[] buffer;
                    string PDFPath = response.File_Path;
                    string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), rootPath, Path.GetFileName(response.File_Path));
                    if (!System.IO.File.Exists(outputFilePath))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(outputFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
                        WebRequest req = WebRequest.Create(PDFPath);
                        using (WebResponse resp = req.GetResponse())
                        {
                            using (Stream stream = resp.GetResponseStream())
                            {
                                using (BinaryReader br = new BinaryReader(stream))
                                {
                                    int len = (int)(resp.ContentLength);
                                    buffer = br.ReadBytes(len);
                                    br.Close();

                                    using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                                    {
                                        using (BinaryWriter w = new BinaryWriter(fs))
                                        {
                                            try
                                            {
                                                w.Write(buffer);
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(outputFilePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, PdfConvert.GetContentType(PDFPath), Path.GetFileName(PDFPath));
                }
                else
                {
                    return Json(new { responseText = "File not found", message = "error" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { responseText = "File not generated", message = "error" });
            }
        }
    }
}