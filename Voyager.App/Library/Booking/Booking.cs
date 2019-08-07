using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Contracts;
using Microsoft.Extensions.Configuration;
using System.Linq;
using VGER_WAPI_CLASSES;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Voyager.App.Library
{
    public class BookingLibrary
    {
        MasterProviders objMasterProviders;
        private readonly IConfiguration _configuration;
        public BookingLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            objMasterProviders = new MasterProviders(_configuration);
        }

        public List<BookingPositionViewModel> BindHotelList(BookingDetailRes response, BookingSearchRes res)
        {
            List<BookingPositionViewModel> lstHotelposition = new List<BookingPositionViewModel>();
            if (response.Booking.Services != null && res.BookingRooms != null)
            {
                var hotelpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "HOTEL").ToList();
                var positionIds = hotelpositions.Select(x => x.Position_Id).ToList();
                var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId) && x.PersonType == "ADULT").Select(y => new { y.PositionId, y.RoomNo, y.ProductTemplate, y.PersonType }).ToList();

                foreach (var hotel in hotelpositions)
                {
                    var pos = new BookingPositionViewModel();
                    pos.StartDate = hotel.StartDate;
                    pos.Country = hotel.Country;
                    pos.City = hotel.City;
                    pos.ProductName = hotel.ProductName;
                    pos.Board = hotel.MealPlan;
                    pos.Status = hotel.Status;
                    pos.Single = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "SINGLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
                    pos.Triple = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "TRIPLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
                    pos.Double = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "DOUBLE").Select(x => x.RoomNo).FirstOrDefault() ?? "";
                    pos.Twin = rooms.Where(x => x.PositionId == hotel.Position_Id && x.ProductTemplate == "TWIN").Select(x => x.RoomNo).FirstOrDefault() ?? "";

                    lstHotelposition.Add(pos);
                }
                return lstHotelposition;
               // model.HotelPositions = lstHotelposition;
            }
            else
            {
                return null;
                //model.HotelPositions = new List<BookingPositionViewModel>();
            }
        }

        public List<BookingPositionViewModel> BindMealList(BookingDetailRes response, BookingSearchRes res, ProductRangeGetRes prodRangeGetRes, ProductCatGetRes prodCatGetRes)
        {
            List<BookingPositionViewModel> lstMealposition = new List<BookingPositionViewModel>();

            //ProductRangeGetReq prodRangeGetReq = new ProductRangeGetReq();
            //ProductRangeGetRes prodRangeGetRes = new ProductRangeGetRes();
            //ProductCatGetReq prodCatGetReq = new ProductCatGetReq();
            //ProductCatGetRes prodCatGetRes = new ProductCatGetRes();

            if (response.Booking.Services != null && res.BookingRooms != null)
            {
                var mealpositions = response.Booking.Services.Where(a => a.ProductType.ToUpper() == "MEAL").ToList(); //BookingPositions
                var positionIds = mealpositions.Select(x => x.Position_Id).ToList();
                var rooms = res.BookingRooms.Where(x => positionIds.Contains(x.PositionId) && x.PersonType == "ADULT").Select(y => new { y.ProductRangeId, y.ProductTemplate, y.PositionId }).ToList(); //BookingRooms

                #region Get Product Range by Product Id list
                //var productRangeIds = rooms.Select(x => x.ProductRangeId).ToList();
                //prodRangeGetReq.ProductRangeIdList = productRangeIds;
                //prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
                var lstrange = prodRangeGetRes.ProductRangeDetails.Select(y => new { y.ProductCategoryId, y.ProductMenu, y.VoyagerProductRange_Id });
                #endregion

                #region Get Product Category by Product Category Id list
                //var productCategoryIds = lstrange.Select(x => x.ProductCategoryId).ToList();
                //prodCatGetReq.ProdCatIdList = productCategoryIds;
                //prodCatGetRes = objMasterProviders.GetProductCategoryByParam(prodCatGetReq, token).Result;
                var lstCategory = prodCatGetRes.ProdCategoryDetails.Select(y => new { y.ProductCategoryId, y.ProductCategoryName });
                #endregion


                foreach (var meal in mealpositions)
                {
                    var prodRangeId = rooms.Where(x => x.PositionId == meal.Position_Id).Select(y => y.ProductRangeId).FirstOrDefault() ?? "";
                    var prodCatId = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductCategoryId).FirstOrDefault() ?? "";
                    var pos = new BookingPositionViewModel();
                    pos.StartDate = meal.StartDate;
                    pos.Country = meal.Country;
                    pos.City = meal.City;
                    pos.ProductName = meal.ProductName;
                    pos.Status = meal.Status;
                    pos.RestaurantType = lstCategory.Where(x => x.ProductCategoryId == prodCatId).Select(y => y.ProductCategoryName).FirstOrDefault() ?? "";
                    pos.Meal = rooms.Where(x => x.PositionId == meal.Position_Id).Select(y => y.ProductTemplate).FirstOrDefault() ?? "";
                    pos.Menu = lstrange.Where(x => x.VoyagerProductRange_Id == prodRangeId).Select(y => y.ProductMenu).FirstOrDefault() ?? "";

                    lstMealposition.Add(pos);
                }
                return lstMealposition;
                //model.MealPositions = lstMealposition;
            }
            else
            {
                return null;
                //model.MealPositions = new List<BookingPositionViewModel>();
            }
        }
    }
}
