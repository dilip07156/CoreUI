using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using VGER_WAPI_CLASSES;
using VGER_WAPI_CLASSES.Booking;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using Voyager.App.ViewModels.Operations;
using Voyager.Controllers;

namespace Voyager.Areas.Operations.Controllers
{
    [Authorize]
    [Area("Operations")]
    [Route("Operations")]
    public class OperationsController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private BookingProviders bookingProviders;
        private OperationsProviders operationsProviders;
        private SalesProviders objSalesProvider;
        private SalesQuoteLibrary salesLibrary;
        private LoginProviders loginProviders;
        private AgentProviders agentProviders;
        private MasterProviders masterProviders;
        private CommunicationTrailMapping communicationTrailMapping;
        private HotelsProviders hotelsProviders;

        public OperationsController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            bookingProviders = new BookingProviders(_configuration);
            operationsProviders = new OperationsProviders(_configuration);
            objSalesProvider = new SalesProviders(_configuration);
            salesLibrary = new SalesQuoteLibrary(_configuration);
            loginProviders = new LoginProviders(_configuration);
            agentProviders = new AgentProviders(_configuration);
            masterProviders = new MasterProviders(_configuration);
            communicationTrailMapping = new CommunicationTrailMapping(configuration);
            hotelsProviders = new HotelsProviders(_configuration);
        }

        #region SearchBooking
        [Route("SearchBooking")]
        public IActionResult SearchBooking()
        {
            OpsSearchViewModel model = new OpsSearchViewModel();

            #region Dropdown Binding
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest { Property = "QRF Masters", Name = "BookingSearchDateType" };
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            QuoteSearchViewModel model2 = salesLibrary.BindMasterData(_configuration, token);
            #endregion

            #region Get Booking Details 
            BookingSearchRes response = new BookingSearchRes();
            response = bookingProviders.GetBookingStatusList(token).Result;
            #endregion

            string[] StatusIgnoreList = new string[] { "N", "J", "I", "B", "C", "-", "^", "L", "S", "X", "T" };
            model.OpsSearchFilters.DateTypeList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList();
            model.OpsSearchFilters.BookingStatusList = response.BookingStatusList.Where(a => !StatusIgnoreList.Contains(a.Attribute_Id)).OrderBy(a => a.AttributeName).ToList();
            model.OpsSearchFilters.BusinessTypeList = model2.QuoteTypeList;
            model.OpsSearchFilters.DestinationList = model2.DestinationList.Select(a => new AttributeValues { Value = a.Value, CityName = a.CityName, AttributeValue_Id = a.AttributeValue_Id }).ToList();

            UserByRoleGetRes responseFH = new UserByRoleGetRes();
            UserByRoleGetReq request = new UserByRoleGetReq();
            request.RoleName = new List<string>() { "Groups", "Operations_Management" };
            responseFH = loginProviders.GetUsersByRole(request, token).Result;
            if (responseFH?.Users.Count > 0)
            {
                var FileHandlerList = responseFH.Users.Select(a => a.Email).Distinct().ToList();
                FileHandlerList.ForEach(a => model.OpsSearchFilters.FileHandlerList.Add(new UserDetails() { Email = a }));
            }

            //var SalesOfficeResponse = agentProviders.GetSalesOfficesByCompanyId(ckUserCompanyId, token).Result;
            //if (SalesOfficeResponse?.Branches.Count > 0)
            //    model.OpsSearchFilters.SalesOfficeList = SalesOfficeResponse.Branches;

            if (!string.IsNullOrWhiteSpace(ckUserEmailId))
            {
                QuoteSearchReq requestSO = new QuoteSearchReq() { UserName = ckUserEmailId };
                QuoteAgentGetRes responseSO = objSalesProvider.GetDivisionList(requestSO, token).Result;
                if (responseSO?.QuoteAgentGetProperties?.Division.Count > 0)
                {
                    if (!string.IsNullOrEmpty(responseSO.CompanyDivision))
                        model.OpsSearchFilters.SalesOffice = responseSO.QuoteAgentGetProperties.Division.Where(a => a.AttributeValue_Id == responseSO.CompanyDivision).Select(a => a.Value).FirstOrDefault();
                    model.OpsSearchFilters.SalesOfficeList = responseSO.QuoteAgentGetProperties.Division;
                }
            }

            model.OpsSearchFilters.DateType = "Travel Date";
            model.OpsSearchFilters.From = DateTime.Now.ToString("dd/MM/yyyy");
            model.OpsSearchFilters.To = DateTime.Now.ToString("dd/MM/yyyy");

            return View(model);
        }

        [Route("_SearchBookingResult")]
        public IActionResult _SearchBookingResult()
        {
            OpsSearchViewModel model = new OpsSearchViewModel();
            return PartialView("_SearchBookingResult", model);
        }

        [HttpPost]
        [Route("LoadData")]
        public ActionResult LoadData(OpsSearchFilters searchFilters, int draw, int start, int length)
        {
            OpsBookingSearchReq BookingReq = new OpsBookingSearchReq();
            OpsBookingSearchRes BookingRes = new OpsBookingSearchRes();
            BookingReq.AgentName = searchFilters.AgentName;
            BookingReq.BookingNumber = searchFilters.CNKReferenceNo;
            BookingReq.AgentCode = searchFilters.AgentReferenceNo;
            BookingReq.AgentTourName = searchFilters.AgentTour;
            BookingReq.Status = searchFilters.BookingStatus;
            BookingReq.DateType = searchFilters.DateType;
            if (string.IsNullOrEmpty(searchFilters.From))
                BookingReq.From = null;
            else
            {
                var strFromDT = searchFilters.From.Split("/");
                if (strFromDT?.Count() >= 3)
                {
                    DateTime fromDT = new DateTime(Convert.ToInt32(strFromDT[2]), Convert.ToInt32(strFromDT[1]), Convert.ToInt32(strFromDT[0]));
                    BookingReq.From = fromDT;
                }
                else
                {
                    BookingReq.From = null;
                }
            }
            if (string.IsNullOrEmpty(searchFilters.To))
                BookingReq.To = null;
            else
            {
                var strToDT = searchFilters.To.Split("/");
                if (strToDT?.Count() >= 3)
                {
                    DateTime toDT = new DateTime(Convert.ToInt32(strToDT[2]), Convert.ToInt32(strToDT[1]), Convert.ToInt32(strToDT[0]));
                    BookingReq.To = toDT;
                }
                else
                {
                    BookingReq.To = null;
                }
            }
            BookingReq.BusinessType = searchFilters.BusinessType;
            BookingReq.Destination = searchFilters.Destination;
            BookingReq.SalesOffice = searchFilters.SalesOffice;
            BookingReq.FileHandler = searchFilters.FileHandler;
            BookingReq.Start = start;
            if (length == 0)
                length = 10;
            BookingReq.Length = length;
            BookingRes = operationsProviders.GetOpsBookingDetails(BookingReq, token).Result;

            return Json(new
            {
                draw = draw,
                recordsTotal = BookingRes.BookingTotalCount,
                recordsFiltered = BookingRes.BookingTotalCount,
                data = BookingRes.Bookings
            });
        }
        #endregion

        #region BookingSummary
        [Route("ViewBookingSummary")]
        public IActionResult ViewBookingSummary(string BookingNumber)
        {
            OpsBookingSummaryViewModel model = new OpsBookingSummaryViewModel();

            #region Get Booking Details 
            ProductSRPHotelGetReq BookingReq = new ProductSRPHotelGetReq() { QRFID = BookingNumber };
            OpsBookingSummaryGetRes BookingRes = operationsProviders.GetOpsBookingSummary(BookingReq, token).Result;
            #endregion

            #region Get ProductType Master
            var ProdTypeList = agentProviders.GetProductTypes(token).Result;
            model.ProdTypeList = ProdTypeList.Select(a => new AttributeValues { AttributeValue_Id = a.Attribute_Id, Value = a.AttributeName }).ToList();
            #endregion

            if (BookingRes != null)
            {
                #region Get Costing Officer Tour Info Header By QRFId
                model.COHeader.QRFID = BookingRes.OpsBookingSummaryDetails.QRFID;
                model.COHeader.TourName = BookingNumber + " : " + BookingRes.OpsBookingSummaryDetails.TourName;
                model.COHeader.NoOfNights = BookingRes.OpsBookingSummaryDetails.NoOfNights;
                model.COHeader.NoOfDays = BookingRes.OpsBookingSummaryDetails.NoOfDays;
                model.COHeader.Destination = BookingRes.OpsBookingSummaryDetails.Destination;

                model.COHeader.TravelDate = BookingRes.OpsBookingSummaryDetails.StartDate;
                model.COHeader.EndDate = BookingRes.OpsBookingSummaryDetails.EndDate;
                model.COHeader.SalesPerson = BookingRes.OpsBookingSummaryDetails.SalesOfficerEmail;
                model.COHeader.CostingOfficer = BookingRes.OpsBookingSummaryDetails.CostingOfficerEmail;
                model.COHeader.FileHandler = BookingRes.OpsBookingSummaryDetails.FileHandlerEmail;
                model.COHeader.ProductAccountant = BookingRes.OpsBookingSummaryDetails.ProductAccountantEmail;
                model.COHeader.ModuleName = "ops";
                model.COHeader.BookingNumber = BookingNumber;
                #endregion

                model.OpsBookingSummary = BookingRes?.OpsBookingSummaryDetails;
                model.OpsBookingSummary.BookingNumber = BookingNumber;
            }
            return View(model);
        }

        [Route("ShiftBooking")]
        public IActionResult ShiftBooking(string BookingNumber)
        {
            try
            {
                OpsCancelBooking model = new OpsCancelBooking();
                model.BookingNumber = BookingNumber;

                List<SendRoomingListToHotelVm> ListOfHotel = new List<SendRoomingListToHotelVm>();
                PositionsFromBookingGetReq requestForBookiing = new PositionsFromBookingGetReq();
                requestForBookiing.BookingNumber = BookingNumber;
                var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBookiing, token).Result;
                if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
                {
                    foreach (var item in BookingPositions.PositionDetails)
                    {
                        SendRoomingListToHotelVm hotel = new SendRoomingListToHotelVm();
                        hotel.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
                        hotel.Location = item.City + "," + item.Country;
                        hotel.ProductName = item.Product_Name;
                        hotel.Status = item.STATUS;
                        hotel.PositionId = item.Position_Id;
                        ListOfHotel.Add(hotel);
                    }
                }
                model.HotelList = ListOfHotel.OrderBy(x => x.Location).ToList();

                return PartialView("_ShiftBooking", model);
            }
            catch (Exception ex)
            {
                return PartialView("_ShiftBooking");
            }
        }

        [Route("CancelBooking")]
        public IActionResult CancelBooking(string BookingNumber)
        {
            try
            {
                OpsCancelBooking model = new OpsCancelBooking();
                model.BookingNumber = BookingNumber;

                #region Cancellation list binding
                // Get Master Data from Operations Masters, Cancellation Reasons 
                MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
                objMasterTypeRequest.Property = "OperationsMasters";
                objMasterTypeRequest.Name = "CancellationReason";
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
                List<Attributes> list = new List<Attributes>();
                if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
                {
                    if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "CancellationReason")
                    {
                        var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
                        foreach (var a in result)
                        {
                            Attributes aa = new Attributes();
                            aa.Attribute_Id = a.AttributeValue_Id;
                            aa.AttributeName = a.Value;
                            list.Add(aa);
                        }
                    }
                    model.CancellationList = list.OrderBy(x => x.AttributeName).ToList();
                }
                #endregion

                List<SendRoomingListToHotelVm> ListOfHotel = new List<SendRoomingListToHotelVm>();
                PositionsFromBookingGetReq requestForBooking = new PositionsFromBookingGetReq();
                requestForBooking.BookingNumber = BookingNumber;
                requestForBooking.IsPlaceholder = false;
                var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBooking, token).Result;
                if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
                {
                    foreach (var item in BookingPositions.PositionDetails)
                    {
                        SendRoomingListToHotelVm hotel = new SendRoomingListToHotelVm();
                        hotel.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
                        hotel.Location = item.City + "," + item.Country;
                        hotel.ProductName = item.Product_Name;
                        hotel.Status = item.STATUS;
                        hotel.PositionId = item.Position_Id;
                        ListOfHotel.Add(hotel);
                    }
                }
                model.HotelList = ListOfHotel.OrderBy(x => x.Location).ToList();

                return PartialView("_CancelBooking", model);
            }
            catch (Exception ex)
            {
                return PartialView("_CancelBooking");
            }
        }

        [Route("GetCancelBookingSupplierList")]
        public IActionResult GetCancelBookingSupplierList(string BookingNumber)
        {
            try
            {
                OpsCancelBooking model = new OpsCancelBooking();
                model.BookingNumber = BookingNumber;
                List<SendRoomingListToHotelVm> ListOfHotel = new List<SendRoomingListToHotelVm>();
                PositionsFromBookingGetReq requestForBooking = new PositionsFromBookingGetReq();
                requestForBooking.BookingNumber = BookingNumber;
                requestForBooking.IsPlaceholder = false;
                var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBooking, token).Result;
                if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
                {
                    foreach (var item in BookingPositions.PositionDetails)
                    {
                        SendRoomingListToHotelVm hotel = new SendRoomingListToHotelVm();
                        hotel.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
                        hotel.Location = item.City + "," + item.Country;
                        hotel.ProductName = item.Product_Name;
                        hotel.Status = item.STATUS;
                        hotel.PositionId = item.Position_Id;
                        ListOfHotel.Add(hotel);
                    }
                }
                model.HotelList = ListOfHotel.OrderBy(x => x.Location).ToList();

                return PartialView("_CancelBookingSupplierList", model);
            }
            catch (Exception ex)
            {
                return PartialView("_CancelBookingSupplierList");
            }
        }
        #endregion

        #region RoomingList
        [Route("ViewRoomingListSummary")]
        public IActionResult ViewRoomingListSummary(string BookingNumber)
        {
            OpsRoomListSummaryViewModel model = new OpsRoomListSummaryViewModel();
            BookingRoomingGetRequest request = new BookingRoomingGetRequest();
            request.Booking_Number = BookingNumber;
            #region Get Booking Details 
            ProductSRPHotelGetReq BookingReq = new ProductSRPHotelGetReq() { QRFID = BookingNumber };
            OpsBookingSummaryGetRes BookingRes = operationsProviders.GetOpsBookingSummary(BookingReq, token).Result;

            #endregion

            if (BookingRes != null)
            {
                #region Get Costing Officer Tour Info Header By QRFId
                model.COHeader.QRFID = BookingRes.OpsBookingSummaryDetails.QRFID;
                model.COHeader.TourName = BookingNumber + " : " + BookingRes.OpsBookingSummaryDetails.TourName;
                model.COHeader.NoOfNights = BookingRes.OpsBookingSummaryDetails.NoOfNights;
                model.COHeader.NoOfDays = BookingRes.OpsBookingSummaryDetails.NoOfDays;
                model.COHeader.Destination = BookingRes.OpsBookingSummaryDetails.Destination;

                model.COHeader.TravelDate = BookingRes.OpsBookingSummaryDetails.StartDate;
                model.COHeader.EndDate = BookingRes.OpsBookingSummaryDetails.EndDate;
                model.COHeader.SalesPerson = BookingRes.OpsBookingSummaryDetails.SalesOfficerEmail;
                model.COHeader.CostingOfficer = BookingRes.OpsBookingSummaryDetails.CostingOfficerEmail;
                model.COHeader.FileHandler = BookingRes.OpsBookingSummaryDetails.FileHandlerEmail;
                model.COHeader.ProductAccountant = BookingRes.OpsBookingSummaryDetails.ProductAccountantEmail;
                model.COHeader.ModuleName = "ops";
                model.COHeader.BookingNumber = BookingNumber;
                model.BookingNumber = BookingNumber;
                #endregion
            }
            return View(model);
        }

        [Route("ViewRoomingData")]
        public IActionResult ViewRoomingData(string BookingNumber)
        {
            //  OpsRoomListSummaryViewModel model = new OpsRoomListSummaryViewModel();
            OpsRoomListViewModel model = new OpsRoomListViewModel();
            BookingRoomingGetRequest request = new BookingRoomingGetRequest();
            request.Booking_Number = BookingNumber;
            #region Get Rooming Details 

            BookingRoomingGetResponse bookingrooming = operationsProviders.GetRoomingDetails(request, token).Result;
            if (bookingrooming != null && bookingrooming.Passengers.Any())
            {
                foreach (var item in bookingrooming.Passengers)
                {
                    PersonalDetailsViewModel p = new PersonalDetailsViewModel();
                    p.Title = item.Title;
                    p.FirstName = item.FirstName;
                    p.LastName = item.LastName;
                    p.PassengerName_LocalLanguage = item.PassengerName_LocalLanguage;
                    p.PassportNumber = item.PassportNumber;
                    p.DateOfBirth = item.DateOfBirth?.ToString("dd/MM/yyyy");
                    p.Notes = item.Notes;
                    p.RoomAssignment = item.RoomAssignment;
                    p.Passenger_Id = item.Passenger_Id;
                    p.RoomType = item.RoomType;
                    model.PassengerDetails.Add(p);
                }

            }
            model.BookingNumber = BookingNumber;
            #endregion
            return PartialView("_RoomingListData", model);
        }

        [Route("CreateRoomingList")]
        public IActionResult CreateRoomingList(string BookingNumber)
        {
            var Bookin1gNumber = Request.Query["BookingNumber"];
            SetPassengerDetailsReq PassengerRequest = new SetPassengerDetailsReq();
            ProductSRPHotelGetReq BookingReq = new ProductSRPHotelGetReq() { QRFID = BookingNumber };
            OpsBookingSummaryGetRes BookingRes = operationsProviders.GetOpsBookingSummary(BookingReq, token).Result;
            int PassengerCounter = 1;
            int RoomCounter = 1;
            List<PassengerDetails> Passdetails = new List<PassengerDetails>();
            if (BookingRes.OpsBookingSummaryDetails.PaxRooms != null && BookingRes.OpsBookingSummaryDetails.PaxRooms.Any())
            {
                foreach (var n in BookingRes.OpsBookingSummaryDetails.PaxRooms)
                {
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:SINGLE"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            PassengerDetails p = new PassengerDetails();
                            p.FirstName = n.Type + "" + RoomCounter;
                            p.LastName = "Passenger" + PassengerCounter;
                            p.PassengerNumber = PassengerCounter;
                            p.RoomAssignment = RoomCounter;
                            p.RoomType = n.Type;
                            p.PersonType = "Adult";
                            PassengerCounter++;
                            RoomCounter++;
                            Passdetails.Add(p);
                        }
                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:CHILDWITHBED"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            PassengerDetails p = new PassengerDetails();
                            p.FirstName = n.Type + "" + RoomCounter;
                            p.LastName = "Passenger" + PassengerCounter;
                            p.PassengerNumber = PassengerCounter;
                            p.RoomAssignment = RoomCounter;
                            p.RoomType = n.Type;
                            p.Age = n.Age;
                            p.PersonType = "Adult";
                            PassengerCounter++;
                            RoomCounter++;
                            Passdetails.Add(p);
                        }
                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:CHILDWITHOUTBED"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            PassengerDetails p = new PassengerDetails();
                            p.FirstName = n.Type + "" + RoomCounter;
                            p.LastName = "Passenger" + PassengerCounter;
                            p.PassengerNumber = PassengerCounter;
                            p.RoomAssignment = RoomCounter;
                            p.RoomType = n.Type;
                            p.Age = n.Age;
                            p.PersonType = "Adult";
                            PassengerCounter++;
                            RoomCounter++;
                            Passdetails.Add(p);
                        }
                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:INFANT"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            PassengerDetails p = new PassengerDetails();
                            p.FirstName = n.Type + "" + RoomCounter;
                            p.LastName = "Passenger" + PassengerCounter;
                            p.PassengerNumber = PassengerCounter;
                            p.RoomAssignment = RoomCounter;
                            p.RoomType = n.Type;
                            p.Age = n.Age;
                            p.PersonType = "Adult";
                            PassengerCounter++;
                            RoomCounter++;
                            Passdetails.Add(p);
                        }
                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:TWIN"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            for (int j = 1; j <= 2; j++)
                            {
                                PassengerDetails p = new PassengerDetails();
                                p.FirstName = n.Type + "" + RoomCounter;
                                p.LastName = "Passenger" + PassengerCounter;
                                p.PassengerNumber = PassengerCounter;
                                p.RoomType = n.Type;
                                p.PersonType = "Adult";
                                p.RoomAssignment = RoomCounter;
                                PassengerCounter++;
                                Passdetails.Add(p);
                            }
                            RoomCounter++;
                        }

                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:DOUBLE"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            for (int j = 1; j <= 2; j++)
                            {
                                PassengerDetails p = new PassengerDetails();
                                p.FirstName = n.Type + "" + RoomCounter;
                                p.RoomType = n.Type;
                                p.PersonType = "Adult";
                                p.LastName = "Passenger" + PassengerCounter;
                                p.PassengerNumber = PassengerCounter;
                                p.RoomAssignment = RoomCounter;
                                PassengerCounter++;
                                Passdetails.Add(p);
                            }
                            RoomCounter++;
                        }

                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:TRIPLE"), true) == 0)
                    {

                        for (int i = 1; i <= n.Count; i++)
                        {
                            for (int j = 1; j <= 3; j++)
                            {
                                PassengerDetails p = new PassengerDetails();
                                p.FirstName = n.Type + "" + RoomCounter;
                                p.RoomType = n.Type;
                                p.PersonType = "Adult";
                                p.LastName = "Passenger" + PassengerCounter;
                                p.PassengerNumber = PassengerCounter;
                                p.RoomAssignment = RoomCounter;
                                PassengerCounter++;
                                Passdetails.Add(p);
                            }
                            RoomCounter++;
                        }

                    }
                    if (string.Compare(n.Type, _configuration.GetValue<string>("BookingRoomInfo:QUAD"), true) == 0)
                    {
                        for (int i = 1; i <= n.Count; i++)
                        {
                            for (int j = 1; j <= 4; j++)
                            {
                                PassengerDetails p = new PassengerDetails();
                                p.FirstName = n.Type + "" + RoomCounter;
                                p.RoomType = n.Type;
                                p.PersonType = "Adult";
                                p.LastName = "Passenger" + PassengerCounter;
                                p.PassengerNumber = PassengerCounter;
                                p.RoomAssignment = RoomCounter;
                                PassengerCounter++;
                                Passdetails.Add(p);
                            }
                            RoomCounter++;
                        }

                    }

                }
            }
            PassengerRequest.PassengerInfo = Passdetails;
            PassengerRequest.Booking_Number = BookingNumber;

            var response = operationsProviders.SetRoomingDetails(PassengerRequest, token).Result;
            return RedirectToAction("ViewRoomingData", new { BookingNumber = BookingNumber });


        }

        //[Route("SendRoomingListToHotel")]
        //public IActionResult SendRoomingListToHotel(string BookingNumber)
        //{
        //	SendRoomingListToHotelVmList ViewModel = new SendRoomingListToHotelVmList();
        //	List<SendRoomingListToHotelVm> ListOfHotel = new List<SendRoomingListToHotelVm>();
        //	PositionsFromBookingGetReq requestForBookiing = new PositionsFromBookingGetReq();
        //	requestForBookiing.BookingNumber = BookingNumber;
        //	requestForBookiing.PositionType = "Hotel";
        //	ViewModel.BookingNumber = BookingNumber;
        //	var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBookiing, token).Result;
        //	if (BookingPositions?.PositionDetails?.Count>0)
        //	{
        //		foreach (var item in BookingPositions.PositionDetails)
        //		{
        //			SendRoomingListToHotelVm hotel = new SendRoomingListToHotelVm();
        //			hotel.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
        //			hotel.Location = item.City + "," + item.Country;
        //			hotel.ProductName = item.Product_Name;
        //			ListOfHotel.Add(hotel);
        //		}
        //	}
        //	ViewModel.HotelList = ListOfHotel;

        //	return PartialView("~/Areas/Operations/Views/Operations/_SendRoomingListToHotel.cshtml", ViewModel);
        //}

        [Route("SendRoomingListToHotel")]
        public IActionResult SendRoomingListToHotel(string BookingNumber)
        {
            BookingRoomHotelsGetRes ViewModel = new BookingRoomHotelsGetRes();
            var bookingRoomHotelsGetRes = operationsProviders.GetRoomingDetailsForHotels(new BookingRoomingGetRequest { Booking_Number = BookingNumber }, token).Result;
            if (bookingRoomHotelsGetRes?.SendRoomingListToHotelVm?.Count > 0)
            {
                ViewModel.SendRoomingListToHotelVm = bookingRoomHotelsGetRes.SendRoomingListToHotelVm;
            }
            else
            {
                ViewModel.SendRoomingListToHotelVm = new List<SendRoomingListToHotelVm>();
            }
            return PartialView("~/Areas/Operations/Views/Operations/_SendRoomingListToHotel.cshtml", ViewModel);
        }

        [HttpPost]
        [Route("SendRoomingListToHotel")]
        public IActionResult SendRoomingListToHotel(SendRoomingListToHotelVmList Model)
        {
            //SendRoomingListToHotelVmList ViewModel = new SendRoomingListToHotelVmList();
            //List<SendRoomingListToHotelVm> ListOfHotel = new List<SendRoomingListToHotelVm>();
            //PositionsFromBookingGetReq requestForBookiing = new PositionsFromBookingGetReq();
            //requestForBookiing.BookingNumber = BookingNumber;
            //requestForBookiing.PositionType = "Hotel";
            //ViewModel.BookingNumber = BookingNumber;
            //var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBookiing, token).Result;
            //if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
            //{
            //    foreach (var item in BookingPositions.PositionDetails)
            //    {
            //        SendRoomingListToHotelVm hotel = new SendRoomingListToHotelVm();
            //        hotel.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
            //        hotel.Location = item.City + "," + item.Country;
            //        hotel.ProductName = item.Product_Name;
            //        ListOfHotel.Add(hotel);
            //    }
            //}
            //ViewModel.HotelList = ListOfHotel;

            return PartialView("~/Areas/Operations/Views/Operations/_SendRoomingListToHotel.cshtml", Model);
        }

        [Route("DownloadExcel")]
        public IActionResult DownloadExcel(string BookingNumber)
        {
            BookingRoomingGetRequest request = new BookingRoomingGetRequest();
            request.Booking_Number = BookingNumber;
            BookingRoomingGetResponse bookingrooming = operationsProviders.GetRoomingDetails(request, token).Result;
            List<PersonTemplateViewModel> tempateList = new List<PersonTemplateViewModel>();
            //string ExcelName = BookingNumber + bookingrooming.TourName + "RoomingList";
            string ExcelName = "BookingNo-" + BookingNumber;
            DataTable dt = new DataTable(ExcelName);

            dt.Columns.AddRange(new DataColumn[14] { new DataColumn("Pax"),
                                            new DataColumn("Room"),
                                            new DataColumn("RoomType"),
                                  new DataColumn("FullName"),new DataColumn("FirstName"),new DataColumn("LastName"),new DataColumn("Sex"),new DataColumn("Date of Birth"),new DataColumn("PP.No")
                                  ,new DataColumn("Date of Issue"),new DataColumn("Date of Expiry"),new DataColumn("Visa number")
                                  ,new DataColumn("Comment"),new DataColumn("Age")});
            if (bookingrooming != null && bookingrooming.Passengers.Any())
            {
                foreach (var item in bookingrooming.Passengers)
                {
                    PersonTemplateViewModel p = new PersonTemplateViewModel();
                    p.Pax = item.PassengerNumber;
                    p.Room = item.RoomAssignment.HasValue ? item.RoomAssignment.Value : 0;
                    p.RoomType = item.RoomType;
                    p.FullName_NonEnglish = item.PassengerName_LocalLanguage;
                    p.FirstName = item.FirstName;
                    p.LastName = item.LastName;
                    p.Sex = item.Sex;
                    p.DateOfBirth = item.DateOfBirth.HasValue ? item.DateOfBirth?.ToString("dd/MM/yyyy") : null;
                    p.PassportNumber = item.PassportNumber;
                    p.DateOfIssue = item.PassportIssued.HasValue ? item.PassportIssued?.ToString("dd/MM/yyyy") : null;
                    p.DateofExpiry = item.PassportExpiry.HasValue ? item.PassportExpiry?.ToString("dd/MM/yyyy") : null;
                    p.VisaNumber = item.VisaNumber;
                    p.Comment = item.Notes;
                    if (item.RoomType?.ToUpper() == "SINGLE" || item.RoomType?.ToUpper() == "DOUBLE" || item.RoomType?.ToUpper() == "TWIN" || item.RoomType?.ToUpper() == "TRIPLE" || item.RoomType?.ToUpper() == "TSU" || item.RoomType?.ToUpper() == "QUAD")
                    {
                        p.Age = null;
                    }
                    else {
                        p.Age = item.Age;
                    }
                    dt.Rows.Add(p.Pax, p.Room, p.RoomType, p.FullName_NonEnglish, p.FirstName, p.LastName, p.Sex, p.DateOfBirth, p.PassportNumber, p.DateOfIssue, p.DateofExpiry, p.VisaNumber, p.Comment,p.Age);
                }

            }
            using (XLWorkbook wb = new XLWorkbook())
            {

                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelName.xlsx");
                }
            }


        }

        [Route("ViewPersonalDetails")]
        public IActionResult ViewPersonalDetails(string Personid, string BookingNumber)
        {
            PersonalDetailsViewModel Pvm = new PersonalDetailsViewModel();

            BookingRoomingGetRequest _request = new BookingRoomingGetRequest();
            _request.Booking_Number = BookingNumber;
            _request.Passenger_Id = Personid;
            var personaldetails = operationsProviders.GetRoomingDetails(_request, token).Result;
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            BookingRoomingGetRequest request = new BookingRoomingGetRequest();
            request.Booking_Number = BookingNumber;
            BookingRoomingGetResponse bookingrooming = operationsProviders.GetRoomingDetails(request, token).Result;
            var RoomNumber = bookingrooming.Passengers.Where(x => x.RoomAssignment != null).OrderBy(x => x.RoomAssignment).Last().RoomAssignment;
            for (int i = 1; i <= RoomNumber; i++)
            {
                Pvm.RoomAssignmentList.Add(i);
            }
            objMasterTypeRequest.Property = "OperationsMasters";
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                for (int i = 0; i < objMasterTypeResponse.PropertyList[0].Attribute.Count; i++)
                {
                    if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "RoomType")
                    {
                        Pvm.RoomList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "DietaryRequirements")
                    {
                        Pvm.DietaryRequirementsList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();

                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "SpecialAssistance")
                    {

                        Pvm.SpecialAssistanceList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();

                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "NameTitle")
                    {

                        Pvm.NameTitle = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();

                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Sex")
                    {
                        Pvm.SexList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                }
            }
            var person = personaldetails.Passengers.FirstOrDefault();
            Pvm.FirstName = person?.FirstName;
            Pvm.LastName = person?.LastName;
            Pvm.Passenger_Id = person?.Passenger_Id;
            Pvm.PassportExpiry = person?.PassportExpiry?.ToString("dd/MM/yyyy");
            Pvm.PassportIssued = person?.PassportIssued?.ToString("dd/MM/yyyy");
            Pvm.PassportNumber = person?.PassportNumber;
            Pvm.PersonType = person?.PersonType;
            Pvm.PassengerNumber = person.PassengerNumber;
            Pvm.ISTourLeader = person?.ISTourLeader ?? false;
            Pvm.Notes = person?.Notes;
            Pvm.PassengerName_LocalLanguage = person?.PassengerName_LocalLanguage;
            Pvm.RoomAssignment = person?.RoomAssignment;
            Pvm.RoomType = person?.RoomType;
            Pvm.Age = person?.Age;
            Pvm.Sex = person?.Sex;
            Pvm.SpecialAssistanceRequirements = person?.SpecialAssistanceRequirements;
            Pvm.VisaNumber = person?.VisaNumber;
            Pvm.RoomList.Add(new AttributeValues { AttributeValue_Id = null, CityId = null, CityName = null, SequenceNo = 0, Flag = false, Value = "CHILDWITHBED" });
            Pvm.RoomList.Add(new AttributeValues { AttributeValue_Id = null, CityId = null, CityName = null, SequenceNo = 0, Flag = false, Value = "CHILDWITHOUTBED" });
            Pvm.RoomList.Add(new AttributeValues { AttributeValue_Id = null, CityId = null, CityName = null, SequenceNo = 0, Flag = false, Value = "INFANT" });
            Pvm.SpecialAssistanceRequirements = person?.SpecialAssistanceRequirements;
            if (person.DietaryRequirements.Any())
            {
                Pvm.DietaryRequirements = person?.DietaryRequirements?.First();
            }
            Pvm.DateOfBirth = person?.DateOfBirth?.ToString("dd/MM/yyyy");
            Pvm.BookingNumber = BookingNumber;
            Pvm.DateOfAnniversary = person?.DateOfAnniversary?.ToString("dd/MM/yyyy");
            Pvm.Title = person.Title;

            return PartialView("~/Areas/Operations/Views/Operations/_ViewPersonalDetails.cshtml", Pvm);
            // return PartialView("_ViewPersonalDetails", Pvm);
        }

        [HttpPost]
        [Route("SavePersonalDetail")]
        public IActionResult SavePersonalDetail(PersonalDetailsViewModel p)
        {
            SetPassengerDetailsReq PassengerRequest = new SetPassengerDetailsReq();
            List<PassengerDetails> passdetailsList = new List<PassengerDetails>();
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;

            PassengerDetails passdetails = new PassengerDetails();
            passdetails.Title = p.Title;
            if (!string.IsNullOrEmpty(p.Age) && Int32.TryParse(p.Age, out int passage))
            {
                passdetails.Age = p.Age;
            }
            passdetails.FirstName = p.FirstName;
            passdetails.LastName = p.LastName;
            passdetails.PassengerName_LocalLanguage = p.PassengerName_LocalLanguage;
            passdetails.PassengerNumber = p.PassengerNumber;
            passdetails.AuditTrail.CREA_US = objCookies["EmailId"];
            passdetails.AuditTrail.MODI_US = objCookies["EmailId"];
            if (!string.IsNullOrEmpty(p.PassportExpiry))
            {
                string[] strDT = new string[0];
                strDT = p.PassportExpiry.Split("/");
                if (strDT.Count() >= 3)
                {
                    DateTime dtnew = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                    passdetails.PassportExpiry = dtnew;
                }
            }
            passdetails.Sex = p.Sex;
            if (!string.IsNullOrEmpty(p.DateOfAnniversary))
            {
                string[] strDT = new string[0];
                strDT = p.DateOfAnniversary.Split("/");
                if (strDT.Count() >= 3)
                {
                    DateTime dtnew = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                    passdetails.DateOfAnniversary = dtnew;
                }

            }

            if (!string.IsNullOrEmpty(p.DateOfBirth))
            {
                string[] strDT = new string[0];
                strDT = p.DateOfBirth.Split("/");
                if (strDT.Count() >= 3)
                {
                    DateTime dtnew = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                    passdetails.DateOfBirth = dtnew;
                }
            }
            if (!string.IsNullOrEmpty(p.DietaryRequirements))
            {
                passdetails.DietaryRequirements.Add(p.DietaryRequirements);
            }
            if (!string.IsNullOrEmpty(p.PassportIssued))
            {
                string[] strDT = new string[0];
                strDT = p.PassportIssued.Split("/");
                if (strDT.Count() >= 3)
                {
                    DateTime dtnew = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                    passdetails.PassportIssued = dtnew;
                }

            }
            passdetails.ISTourLeader = p.ISTourLeader;
            passdetails.PersonType = p.PersonType;
            passdetails.RoomAssignment = p.RoomAssignment;
            passdetails.RoomType = p.RoomType;
            passdetails.SpecialAssistanceRequirements = p.SpecialAssistanceRequirements;
            passdetails.Passenger_Id = p.Passenger_Id;
            passdetails.Notes = p.Notes;
            passdetails.PassportNumber = p.PassportNumber;
            passdetails.VisaNumber = p.VisaNumber;
            passdetailsList.Add(passdetails);
            PassengerRequest.PassengerInfo = passdetailsList;
            PassengerRequest.Booking_Number = p.BookingNumber;
            var response = operationsProviders.SetRoomingDetails(PassengerRequest, token).Result;
            p.status = response.ResponseStatus.Status;
            p.StatusMessage = response.ResponseStatus.StatusMessage;
            return Json(new { responseText = response.ResponseStatus.StatusMessage, status = response.ResponseStatus.Status, BookingNumber = p.BookingNumber });

        }

        [HttpGet]
        [Route("UploadRoomingListFile")]
        public IActionResult UploadRoomingListFile(string BookingNumber)
        {
            UploadRoomingListDetailsVm uvm = new UploadRoomingListDetailsVm();
            uvm.BookingNumber = BookingNumber;
            return PartialView("~/Areas/Operations/Views/Operations/_UploadRoomingList.cshtml", uvm);
        }

        [HttpPost]
        [Route("UploadRoomingList")]
        public JsonResult UploadRoomingList()
        {
            ResponseStatus result = new ResponseStatus();
            DataTable dtContent = new DataTable();
            IFormFile File = Request.Form.Files[0];
            // string BookingNumber = Request.Form.First().Value.ToString();
            UploadRoomingListDetailsVm uvm = new UploadRoomingListDetailsVm();
            result = uvm.UploadUserFile(File);
            if (result.Status == "Success")
            {
                try
                {
                    dtContent = GetDataTableFromExcel(File);
                }
                catch (Exception e)
                {
                    result.Status = "Failure";
                    result.StatusMessage = "Excel Template not Valid.Template should be Same as Download RoomingList Template Excel";
                    return Json(result);
                }
                result = ValidateDataTable(dtContent);
                if (string.IsNullOrEmpty(result.Status))
                {
                    result.Status = "Success";
                    result.StatusMessage = "Excel is in valid Format and can be uploaded";
                }

            }
            return Json(result);
        }

        [HttpPost]
        [Route("SaveRoomingListPersonDetails")]
        public JsonResult SaveRoomingListPersonDetails(UploadRoomingListDetailsVm Vm)
        {
            SetPassengerDetailsReq PassengerRequest = new SetPassengerDetailsReq();
            List<PassengerDetails> passdetailsList = new List<PassengerDetails>();
            BookingRoomingSetResponse response = new BookingRoomingSetResponse();
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            if (Vm != null && Vm.PassengerDetailsFromExcel != null && Vm.PassengerDetailsFromExcel.Any())
            {
                DateTime? dt = null;
                string[] strDT = new string[0];
                foreach (var x in Vm.PassengerDetailsFromExcel)
                {
                    PassengerDetails passdetails = new PassengerDetails();
                    passdetails.PassengerNumber = Convert.ToInt32(x.Pax);
                    passdetails.RoomAssignment = Convert.ToInt32(x.Room);
                    passdetails.RoomType = x.RoomName;
                    passdetails.PassengerName_LocalLanguage = x.FullName;
                    passdetails.FirstName = x.FirstName;
                    passdetails.LastName = x.LastName;
                    passdetails.Sex = x.Sex;
                    if (!string.IsNullOrEmpty(x.Age) && Int32.TryParse(x.Age, out int passage))
                    {
                        passdetails.Age = x.Age;
                    }
                    passdetails.AuditTrail.MODI_US = objCookies["EmailId"];
                    if (!string.IsNullOrEmpty(x.Dob))
                    {
                        if (x.Dob.Contains(" "))
                        {
                            string[] strDTforSpace = new string[0];
                            strDTforSpace = x.Dob.Split(" ");
                            string sdr = strDTforSpace[0];
                            strDT = sdr.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                            passdetails.DateOfBirth = dt;
                        }
                        else
                        {
                            strDT = x.Dob.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                            passdetails.DateOfBirth = dt;
                        }
                    }
                    passdetails.PassportNumber = x.PassportNumber;
                    passdetails.Notes = x.Comments;
                    passdetails.VisaNumber = x.VisaNo;
                    if (!string.IsNullOrEmpty(x.DateofIssue))
                    {
                        if (x.DateofIssue.Contains(" "))
                        {
                            string[] strDTforSpace = new string[0];
                            strDTforSpace = x.DateofIssue.Split(" ");
                            string sdr = strDTforSpace[0];
                            strDT = sdr.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                            passdetails.PassportIssued = dt;
                        }
                        else
                        {
                            strDT = x.DateofIssue.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                            passdetails.PassportIssued = dt;
                        }
                    }
                    if (!string.IsNullOrEmpty(x.DateOfExp))
                    {
                        if (x.DateOfExp.Contains(" "))
                        {
                            string[] strDTforSpace = new string[0];
                            strDTforSpace = x.DateOfExp.Split(" ");
                            string sdr = strDTforSpace[0];
                            strDT = sdr.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                            passdetails.PassportExpiry = dt;
                        }
                        else
                        {
                            strDT = x.DateOfExp.ToString().Split("/");
                            if (strDT.Count() == 3)
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));

                            }
                            passdetails.PassportExpiry = dt;
                        }
                    }
                    passdetailsList.Add(passdetails);
                }

                PassengerRequest.PassengerInfo = passdetailsList;
                PassengerRequest.Booking_Number = Vm.BookingNumber;
                var responseforsave = operationsProviders.SaveRoomingListPersonDetails(PassengerRequest, token).Result;
                response = responseforsave;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("UpdateBookingRoomsDataAsperExcel")]
        public IActionResult UpdateBookingRoomsDataAsperExcel(Dictionary<string, string> model)
        {
            string status = "";
            List<string> msg = new List<string>();
            List<string> docname = new List<string>();
            if (model != null)
            {
                BookingPaxDetailsGetRequest requestforPax = new BookingPaxDetailsGetRequest();
                List<OpsBookingRoomsDetails> BookingDetails = new List<OpsBookingRoomsDetails>();
                List<OpsBookingPaxDetails> BookingPaxDetailsforUi = new List<OpsBookingPaxDetails>();
                List<ViewRoomingListCompare> BookingRoomsData = new List<ViewRoomingListCompare>();
                List<SupplierDetails> SuppliersData = new List<SupplierDetails>();
                List<DeterminePassCount> TotalPassForPax = new List<DeterminePassCount>();
                List<PersonMaterializationDetails> PersonalDetails = new List<PersonMaterializationDetails>();
                string BookingNumber = model["BookingNumber"];
                string Action = model["Action"];
                string Module = model["Module"];
                string ModuleParent = model["ModuleParent"];
                string PositionId = model.ContainsKey("PositionId") ? model["PositionId"] : "";
                requestforPax.BookingNumber = BookingNumber;
                var request = new OpsBookingSetReq
                {
                    BookingNumber = BookingNumber,
                    PositionIds = !string.IsNullOrEmpty(PositionId) ? new List<string>() { PositionId } : null,
                    UserEmailId = ckUserEmailId,
                    UserId = ckLoginUser_Id,
                    UserName = ckUserName,
                    OpsKeyValue = model.Select(a => new OpsKeyValue { Key = a.Key, Value = a.Value }).ToList(),
                    Action = Action,
                    Module = Module,
                    ModuleParent = ModuleParent,

                };
                var keyValueForBookingRooms = new OpsKeyValue();
                var keyValueForPositions = new OpsKeyValue();
                var KeyValueList = request.OpsKeyValue;
                keyValueForBookingRooms = KeyValueList.Where(x => x.Key == "tblBookingRoomsDetails").FirstOrDefault();
                keyValueForPositions = KeyValueList.Where(x => x.Key == "tblSuppliers").FirstOrDefault();
                BookingRoomsData = JsonConvert.DeserializeObject<List<ViewRoomingListCompare>>(keyValueForBookingRooms.Value.ToString());
                SuppliersData = JsonConvert.DeserializeObject<List<SupplierDetails>>(keyValueForPositions.Value.ToString());
                if (BookingRoomsData != null)
                {
                    foreach (var v in BookingRoomsData)
                    {
                        OpsBookingRoomsDetails op = new OpsBookingRoomsDetails();
                        op.RoomType = v.RoomName?.ToUpper() ;
                        op.NewLevel = v.ExcelSheetRoomQuantity;
                        op.Current = v.BookingRoomQuantity;
                        op.BookingRooms_Id = v.BookingRoomsid;
                        op.For = "Pax";
                        if (v.Difference == "Room Type Removed")
                        {
                            op.Status = "X";
                        }
                        if (op.RoomType == "SINGLE" || op.RoomType == "DOUBLE" || op.RoomType == "TWIN" || op.RoomType == "TRIPLE" || op.RoomType == "TSU" || op.RoomType == "QUAD")
                        {
                            op.Age = null;
                        }
                        else {
                            if (!string.IsNullOrEmpty(v.Age))
                            {
                                op.Age = Convert.ToInt16(v.Age);
                            }

                        }
                        
                        BookingDetails.Add(op);
                        TotalPassForPax.Add(new DeterminePassCount { RoomName = v.RoomName, RoomCount = v.ExcelSheetRoomQuantity.ToString() });
                    }
                }
                var BookingPaxDetails = operationsProviders.GetOpsBookingPaxDetails(requestforPax, token).Result;
                if (BookingPaxDetails != null && BookingPaxDetails.bookingPaxDetails != null && BookingPaxDetails.bookingPaxDetails.Count > 0)
                {
                    var PaxDetails = BookingPaxDetails.bookingPaxDetails.ToList();
                    foreach (var t in PaxDetails)
                    {
                        OpsBookingPaxDetails op = new OpsBookingPaxDetails();
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "CHILD + BED")
                        {
                            op.PassengerType = "CHILD + BED";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString():null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;
                        }
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "CHILD - BED")
                        {
                            op.PassengerType = "CHILD - BED";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString() : null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;
                        }
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "INFANT")
                        {
                            op.PassengerType = "INFANT";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString() : null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;
                        }
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "DRIVER")
                        {
                            op.PassengerType = "DRIVER";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString() : null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;
                        }
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "GUIDE")
                        {
                            op.PassengerType = "GUIDE";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString() : null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;
                        }
                        if (t.PERSTYPE?.Trim()?.ToUpper() == "ADULT")
                        {
                            op.PassengerType = "ADULT";
                            op.PassengerAge = t.AGE.HasValue ? t.AGE.Value.ToString() : null;
                            op.BookingPax_ID = t.BookingPax_Id;
                            op.PassengerQty = t.PERSONS;

                        }
                        BookingPaxDetailsforUi.Add(op);
                    }

                }
                int AdultCount = DeterminePassengerQuantity(TotalPassForPax);
           var AdultpaxData  = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "ADULT")?.FirstOrDefault();
                AdultpaxData.PassengerQty = AdultCount;
                foreach (var a in BookingRoomsData)
                {
                    if (a.RoomName?.Trim()?.ToUpper() == "CHILDWITHBED" || a.RoomName?.Trim()?.ToUpper() == "CHILDWITHOUTBED" || a.RoomName?.Trim()?.ToUpper() == "INFANT")
                    {
                        if (!string.IsNullOrEmpty(a.BookingRoomsid))
                        {
                            if (a.RoomName?.Trim()?.ToUpper() == "CHILDWITHBED")
                            {
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "CHILD + BED" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if(childdata != null)
                                childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                            }
                            if (a.RoomName == "CHILDWITHOUTBED")
                            {
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "CHILD - BED" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if (childdata != null)
                                    childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                            }
                            if (a.RoomName == "INFANT")
                            {
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "INFANT" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if (childdata != null)
                                    childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                            }

                        }
                        else {
                            if (a.RoomName?.Trim()?.ToUpper() == "CHILDWITHBED")
                            {  
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "CHILD + BED" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if (childdata != null)
                                {
                                    childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                                }
                                else
                                {
                                    BookingPaxDetailsforUi.Add(new OpsBookingPaxDetails
                                    {
                                        BookingPax_ID = null,
                                        PassengerAge = a.Age,
                                        PassengerQty = a.ExcelSheetRoomQuantity,
                                        PassengerType = "CHILD + BED"
                                    });
                                }
                            }
                            if (a.RoomName?.Trim()?.ToUpper() == "CHILDWITHOUTBED")
                            {
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "CHILD - BED" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if (childdata != null)
                                {
                                    childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                                }
                                else
                                {
                                    BookingPaxDetailsforUi.Add(new OpsBookingPaxDetails
                                    {
                                        BookingPax_ID = null,
                                        PassengerAge = a.Age,
                                        PassengerQty = a.ExcelSheetRoomQuantity,
                                        PassengerType = "CHILD - BED"
                                    });
                                }
                            }
                            if (a.RoomName?.Trim()?.ToUpper() == "INFANT")
                            {
                                var childdata = BookingPaxDetailsforUi.Where(x => x.PassengerType?.Trim()?.ToUpper() == "INFANT" && x.PassengerAge == a.Age)?.FirstOrDefault();
                                if (childdata != null)
                                {
                                    childdata.PassengerQty = a.ExcelSheetRoomQuantity;
                                }
                                else
                                {
                                    BookingPaxDetailsforUi.Add(new OpsBookingPaxDetails
                                    {
                                        BookingPax_ID = null,
                                        PassengerAge = a.Age,
                                        PassengerQty = a.ExcelSheetRoomQuantity,
                                        PassengerType = "INFANT"
                                    });
                                }
                            }
                        }
                    }
                }
                var Serializeddata = JsonConvert.SerializeObject(BookingDetails);
                var SerilalizedSupplierData = JsonConvert.SerializeObject(SuppliersData);
                var SerializedPaxData = JsonConvert.SerializeObject(BookingPaxDetailsforUi);
                model.Add("BookingRooms", Serializeddata);
                model.Add("BookingPax", SerializedPaxData);
                model.Add("Positions", SerilalizedSupplierData);
                model.Remove("tblBookingRoomsDetails");
                model.Remove("tblSuppliers");
                model.Remove("BookingNumber");
                model.Remove("PositionId");
                model.Remove("Action");
                model.Remove("Module");
                model.Remove("ModuleParent");
                //model.Remove("Doctype");

                var request1 = new OpsBookingSetReq
                {
                    BookingNumber = BookingNumber,
                    PositionIds = !string.IsNullOrEmpty(PositionId) ? new List<string>() { PositionId } : null,
                    UserEmailId = ckUserEmailId,
                    UserId = ckLoginUser_Id,
                    UserName = ckUserName,
                    OpsKeyValue = model.Select(a => new OpsKeyValue { Key = a.Key, Value = a.Value }).ToList(),
                    Action = Action,
                    Module = Module,
                    ModuleParent = ModuleParent,

                };

                var response = operationsProviders.SetBookingByWorkflow(request1, token).Result;
                if (response?.ResponseStatus != null)
                {
                    status = response.ResponseStatus.Status;
                    msg = response.ResponseStatus.ErrorMessage;
                    docname = response.ResponseStatus.DocumentDetails?.Select(a => a.DocumentName).ToList();
                }
                else
                {
                    status = "Failure";
                    msg.Add("An Error Occurred.");
                }

            }
            return Json(new
            {
                status = status,
                msg = msg,
                docname = docname
            });
            
        }
        [HttpPost]
        [Route("CancelRoomingListUpdate")]
        public IActionResult CancelRoomingListUpdate(string BookingNumber)
        {
            string status = "";
            List<string> msg = new List<string>();
            if (!string.IsNullOrEmpty(BookingNumber))
            {
                var response = operationsProviders.CancelRoomingListUpdate(BookingNumber, token).Result;
                if (response?.ResponseStatus != null)
                {
                    status = response.ResponseStatus.Status;
                    msg = response.ResponseStatus.ErrorMessage;
                    
                }

            }
            else {
                return Json(new
                {
                    status = "Failure",
                    msg = "Booking Number missing",
                   
                });
            }
            return Json(new
            {
                status = status,
                msg = msg
               
            });

        }

        [HttpPost]
        [Route("CompareRoomingListData")]
        public IActionResult CompareRoomingListData()
        {
            UploadRoomingListDetailsVm uvm = new UploadRoomingListDetailsVm();
            List<PersonalDetailsTobeSaved> ps = new List<PersonalDetailsTobeSaved>();
            ResponseStatus result = new ResponseStatus();
            List<OperationsBookingRoom> OperationBooking = new List<OperationsBookingRoom>();
            List<OperationsBookingRoom> OperationBookingForExcel = new List<OperationsBookingRoom>();
            List<ExcelBookingRoom> ExcelBooking = new List<ExcelBookingRoom>();
            List<ViewRoomingListCompare> DisplayRoomingData = new List<ViewRoomingListCompare>();
            // ProductSRPHotelGetReq request = new ProductSRPHotelGetReq();
            List<SupplierDetails> supdetails = new List<SupplierDetails>();
            PositionsFromBookingGetReq requestForBookiing = new PositionsFromBookingGetReq();
            BookingRoomsGetRequest request = new BookingRoomsGetRequest();
            IFormFile File = Request.Form.Files[0];
            string BookingNumber = Request.Form.First().Value.ToString();
            uvm.BookingNumber = BookingNumber;
            requestForBookiing.BookingNumber = BookingNumber;
            var dtContent = GetDataTableFromExcel(File);
            // request.QRFID = BookingNumber;
            request.BookingNumber = BookingNumber;
            // var BookingDetails = operationsProviders.GetOpsBookingSummary(request, token).Result;
            var BookingDetails = operationsProviders.GetOpsBookingRoomsDetails(request, token).Result;
            var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBookiing, token).Result;
            if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
            {
                foreach (var item in BookingPositions.PositionDetails)
                {
                    SupplierDetails sdetails = new SupplierDetails();
                    sdetails.ProductType = item.ProductType;
                    sdetails.Position_ID = item.Position_Id;
                    sdetails.ProductName = item.Product_Name;
                    sdetails.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
                    sdetails.SupplierName = item.SupplierInfo?.Name;
                    supdetails.Add(sdetails);

                }
                uvm.SuppliersDetails = supdetails;

            }
            if (BookingDetails != null && BookingDetails.BookingRoomsDetails
                != null && BookingDetails.BookingRoomsDetails.Count > 0)
            {
                var BookingRoomsDetails = BookingDetails.BookingRoomsDetails.ToList();
                foreach (var booking in BookingRoomsDetails)
                {
                    OperationsBookingRoom ob = new OperationsBookingRoom();
                    ob.RoomQuantity = booking.ROOMNO.HasValue ? booking.ROOMNO.Value.ToString() : string.Empty;
                    ob.RoomName = booking.SUBPROD;
                    ob.BookingRoomsID = booking.BookingRooms_ID;
                    ob.Age = booking.Age.HasValue ?booking.Age.Value.ToString() : null;
                    OperationBooking.Add(ob);
                }
            }
            if (dtContent != null && dtContent.Rows != null)
            {
                for (int i = 0; i < dtContent.Rows.Count; i++)
                {
                    //ExcelBookingRoom eb = new ExcelBookingRoom();
                    //eb.RoomQuantity = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[1].ToString()) ? dtContent.Rows[i].ItemArray[1].ToString() : string.Empty;
                    //eb.RoomName = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[2].ToString()) ? dtContent.Rows[i].ItemArray[2].ToString() : string.Empty;
                    //ExcelBooking.Add(eb);
                    OperationsBookingRoom eb = new OperationsBookingRoom();
                    eb.Pax = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[0].ToString()) ? dtContent.Rows[i].ItemArray[0].ToString() : string.Empty;
                    eb.RoomQuantity = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[1].ToString()) ? dtContent.Rows[i].ItemArray[1].ToString() : string.Empty;
                    eb.RoomName = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[2].ToString()) ? dtContent.Rows[i].ItemArray[2].ToString() : string.Empty;
                    eb.FullName = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[3].ToString()) ? dtContent.Rows[i].ItemArray[3].ToString() : string.Empty;
                    eb.FirstName = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[4].ToString()) ? dtContent.Rows[i].ItemArray[4].ToString() : string.Empty;
                    eb.LastName = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[5].ToString()) ? dtContent.Rows[i].ItemArray[5].ToString() : string.Empty;
                    eb.Sex = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[6].ToString()) ? dtContent.Rows[i].ItemArray[6].ToString() : string.Empty;
                    eb.Dob = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[7].ToString()) ? dtContent.Rows[i].ItemArray[7].ToString() : string.Empty;
                    eb.PassportNumber = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[8].ToString()) ? dtContent.Rows[i].ItemArray[8].ToString() : string.Empty;
                    eb.DateofIssue = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[9].ToString()) ? dtContent.Rows[i].ItemArray[9].ToString() : string.Empty;
                    eb.DateOfExp = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[10].ToString()) ? dtContent.Rows[i].ItemArray[10].ToString() : string.Empty;
                    eb.VisaNo = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[11].ToString()) ? dtContent.Rows[i].ItemArray[11].ToString() : string.Empty;
                    eb.Comments = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[12].ToString()) ? dtContent.Rows[i].ItemArray[12].ToString() : string.Empty;
                    eb.Age = !String.IsNullOrEmpty(dtContent.Rows[i].ItemArray[13].ToString()) ? dtContent.Rows[i].ItemArray[13].ToString() : null;
                    OperationBookingForExcel.Add(eb);
                }
            }
            foreach (var x in OperationBookingForExcel)
            {
                PersonalDetailsTobeSaved person = new PersonalDetailsTobeSaved();
                person.Pax = x.Pax;
                person.Room = x.RoomQuantity;
                person.RoomName = x.RoomName;
                person.FullName = x.FullName;
                person.FirstName = x.FirstName;
                person.LastName = x.LastName;
                person.Sex = x.Sex;
                person.Dob = x.Dob;
                person.PassportNumber = x.PassportNumber;
                person.DateofIssue = x.DateofIssue;
                person.DateOfExp = x.DateOfExp;
                person.VisaNo = x.VisaNo;
                person.Comments = x.Comments;
                if (!string.IsNullOrEmpty(x.Age))
                {
                    person.Age = x.Age;
                }
                ps.Add(person);
            }
            uvm.PassengerDetailsFromExcel = ps;
            // var list = ExcelBooking.Select(x => new { x.RoomQuantity, x.RoomName }).Distinct().GroupBy(x => x.RoomName);
            var OperationBookingForExcelforChild = OperationBookingForExcel.Where(x => (x.RoomName?.Trim()?.ToUpper() == "CHILDWITHBED" || x.RoomName?.Trim()?.ToUpper() == "CHILDWITHOUTBED" || x.RoomName?.Trim()?.ToUpper() == "INFANT"));
            var listforchild = OperationBookingForExcelforChild.Select(x => new { x.RoomQuantity, x.RoomName, x.Age }).GroupBy(t => new { t.RoomName, t.Age });
            OperationBookingForExcel = OperationBookingForExcel.Where(x => x.RoomName?.Trim()?.ToUpper() != "CHILDWITHBED" && x.RoomName?.Trim()?.ToUpper() != "CHILDWITHOUTBED" && x.RoomName?.Trim()?.ToUpper() != "INFANT").ToList();
            var OperationBookingForChild = OperationBooking.Where(x => (x.RoomName?.Trim()?.ToUpper() == "CHILDWITHBED" || x.RoomName?.Trim()?.ToUpper() == "CHILDWITHOUTBED" || x.RoomName?.Trim()?.ToUpper() == "INFANT")).ToList();
            OperationBooking = OperationBooking.Where(x => x.RoomName?.Trim()?.ToUpper() != "CHILDWITHBED" && x.RoomName?.Trim()?.ToUpper() != "CHILDWITHOUTBED" && x.RoomName?.Trim()?.ToUpper() != "INFANT").ToList();
            var list = OperationBookingForExcel.Select(x => new { x.RoomQuantity, x.RoomName }).Distinct().GroupBy(x => x.RoomName);
            foreach (var x in list)
            {
                string roomtype = x.Key;
                ViewRoomingListCompare rc = new ViewRoomingListCompare();
                rc.ExcelSheetRoomQuantity = x.Count();
                rc.RoomName = x.Key;


                if (OperationBooking.Where(m => m.RoomName.Trim().ToUpper() == x.Key.Trim().ToUpper()).Any())
                {
                    //OperationBooking.Where(m => m.RoomName == x.Key) != null
                    string RoomQuant = OperationBooking.Where(t => t.RoomName.Trim().ToUpper() == x.Key.Trim().ToUpper())?.FirstOrDefault()?.RoomQuantity;
                    rc.BookingRoomQuantity = Convert.ToInt32(RoomQuant);
                    rc.BookingRoomsid = OperationBooking.Where(t => t.RoomName.Trim().ToUpper() == x.Key.Trim().ToUpper())?.FirstOrDefault()?.BookingRoomsID;
                   // rc.Age = OperationBooking.Where(t => t.RoomName.Trim().ToUpper() == x.Key.Trim().ToUpper())?.FirstOrDefault()?.Age;
                }
                else
                {
                    rc.Difference = "New Room Type";

                }
                DisplayRoomingData.Add(rc);
            }
            foreach (var x in listforchild)
            {
               
                ViewRoomingListCompare rc = new ViewRoomingListCompare();
                rc.ExcelSheetRoomQuantity = x.Count();
                rc.RoomName = x.Key.RoomName;
                rc.Age = x.Key.Age;


                if (OperationBookingForChild.Where(m => m.RoomName.Trim().ToUpper() == x.Key.RoomName.Trim().ToUpper() && m.Age == x.Key.Age).Any())
                {
                    //OperationBooking.Where(m => m.RoomName == x.Key) != null
                    string RoomQuant = OperationBookingForChild.Where(t => t.RoomName.Trim().ToUpper() == x.Key.RoomName.Trim().ToUpper())?.FirstOrDefault()?.RoomQuantity;
                    rc.BookingRoomQuantity = Convert.ToInt32(RoomQuant);
                    rc.BookingRoomsid = OperationBookingForChild.Where(t => t.RoomName.Trim().ToUpper() == x.Key.RoomName.Trim().ToUpper() && t.Age == x.Key.Age )?.FirstOrDefault()?.BookingRoomsID;
                    // rc.Age = OperationBooking.Where(t => t.RoomName.Trim().ToUpper() == x.Key.Trim().ToUpper())?.FirstOrDefault()?.Age;
                }
                else
                {
                    rc.Difference = "New Room Type";

                }
                DisplayRoomingData.Add(rc);
            }
            //foreach (var t in OperationBooking)
            //{
            //    if (list.ToList().Where(c => c.Key.Trim().ToUpper() == t.RoomName.Trim().ToUpper()).Any())
            //    {
            //        ViewRoomingListCompare rc = new ViewRoomingListCompare();
            //        rc.RoomName = t.RoomName;
            //        rc.BookingRoomQuantity = Convert.ToInt32(t.RoomQuantity);
            //        rc.ExcelSheetRoomQuantity = 0 ;
            //        rc.Difference = "Room Type Removed";
            //        DisplayRoomingData.Add(rc);
            //    }

            //}
            var newList = list.ToList();
            for (int i = 0; i < OperationBooking.Count(); i++)
            {
                if (newList.Find(x => x.Key.Trim().ToUpper() == OperationBooking[i].RoomName.Trim().ToUpper()) == null)
                {
                    ViewRoomingListCompare rc = new ViewRoomingListCompare();
                    rc.RoomName = OperationBooking[i].RoomName;
                    rc.BookingRoomQuantity = Convert.ToInt32(OperationBooking[i].RoomQuantity);
                    rc.ExcelSheetRoomQuantity = 0;
                    rc.Difference = "Room Type Removed";
                    rc.BookingRoomsid = OperationBooking[i].BookingRoomsID;
                    DisplayRoomingData.Add(rc);

                }


            }
            var newListforChild = listforchild.ToList();
            for (int i = 0; i < OperationBookingForChild.Count(); i++)
            {
                if (newListforChild.Find(x => x.Key.RoomName.Trim().ToUpper() == OperationBookingForChild[i].RoomName.Trim().ToUpper() && x.Key.Age == OperationBookingForChild[i].Age) == null)
                {
                    ViewRoomingListCompare rc = new ViewRoomingListCompare();
                    rc.RoomName = OperationBookingForChild[i].RoomName;
                    rc.BookingRoomQuantity = Convert.ToInt32(OperationBookingForChild[i].RoomQuantity);
                    rc.ExcelSheetRoomQuantity = 0;
                    rc.Difference = "Room Type Removed";
                    rc.BookingRoomsid = OperationBookingForChild[i].BookingRoomsID;
                    rc.Age = OperationBookingForChild[i].Age;
                    DisplayRoomingData.Add(rc);

                }


            }
            uvm.DisplayRoomingData = DisplayRoomingData;

            return PartialView("~/Areas/Operations/Views/Operations/_CompareRoomDetails.cshtml", uvm);
        }


        private ResponseStatus ValidateDataTable(DataTable dtContent)
        {
            List<int> Elements = new List<int>() { 0, 1, 2, 4, 5, 6, 8, 10 ,13};
            List<string> Columns = new List<string>() { "Pax", "Room", "RoomType", "FullName", "FirstName", "LastName", "Sex", "Date of Birth", "PP.No", "Date of Issue", "Date of Expiry", "Visa number", "Comment" , "Age" };
            ResponseStatus response = new ResponseStatus();
            if (dtContent != null && dtContent.Columns != null)
            {
                if (dtContent.Columns.Count != 14)
                {
                    response.Status = "Failure";
                    response.StatusMessage = "Invalid Number of Columns";
                    return response;
                }
                if (Columns.Any() && dtContent.Columns.Count == 14)
                {

                    for (int i = 0; i < 13; i++)
                    {
                        if (String.Compare(dtContent.Columns[i].ToString().Trim().ToUpper(), Columns[i].ToString().Trim().ToUpper()) != 0)
                        {
                            response.Status = "Failure";
                            response.StatusMessage = "Invalid Excel Template";
                            return response;
                        }
                    }
                }
                if (dtContent.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow row in dtContent.Rows)
                    {
                        i++;
                        foreach (var index in Elements)
                        {
                            if (index != 13)
                            {
                                if (String.IsNullOrEmpty((row.ItemArray[index]).ToString()))
                                {

                                    response.Status = "Failure";
                                    response.StatusMessage = "Mandatory Data is missing on rows " + (i + 1);
                                    return response;
                                }
                            }
                            if (index == 13 && (row.ItemArray[2].ToString().ToUpper() == "CHILDWITHBED" || row.ItemArray[2].ToString().ToUpper() == "CHILDWITHOUTBED" || row.ItemArray[2].ToString().ToUpper() == "INFANT"))
                            {
                                if (String.IsNullOrEmpty((row.ItemArray[index]).ToString()))
                                {

                                    response.Status = "Failure";
                                    response.StatusMessage = "Mandatory Data  For Child/Infant is missing on rows - Age data " + (i + 1);
                                    return response;
                                }

                            }
                        }

                    }
                }
            }
            if (String.IsNullOrEmpty(response.Status))
            {
                response = checkDatatypeforEachRow(dtContent);
                if (String.IsNullOrEmpty(response.Status))
                {
                    response.Status = "Success";
                    response.StatusMessage = "Data in Excel is in proper format";

                }
                return response;
            }

            return response;
        }

        private ResponseStatus checkDatatypeforEachRow(DataTable dtContent)
        {
            List<String> RoomTypeList = new List<string> { "SINGLE", "QUAD", "DOUBLE", "TRIPLE", "TWIN","TSU","CHILDWITHBED","CHILDWITHOUTBED","INFANT" };
            List<string> SexName = new List<string> { "M", "F", "T" };
            ResponseStatus response = new ResponseStatus();
            for (int i = 0; i < dtContent.Columns.Count; i++)
            {
                for (int j = 0; j < dtContent.Rows.Count; j++)
                {
                    if (dtContent.Columns[i].ColumnName == "Pax")
                    {
                        if (!String.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()) && !Int32.TryParse(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString(), out int numValue))
                        {
                            response.Status = "Failure";
                            response.StatusMessage = "Data Format is incorrect on row " + (j + 2);
                            return response;
                        }
                    }
                    if (dtContent.Columns[i].ColumnName == "Age")
                    {
                        if (!String.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()) && !Int32.TryParse(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString(), out int numValue))
                        {
                            response.Status = "Failure";
                            response.StatusMessage = "Data Format of Age is incorrect on row " + (j + 2);
                            return response;
                        }
                    }
                    if (dtContent.Columns[i].ColumnName == "Room")
                    {
                        if (!String.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()) && !Int32.TryParse(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString(), out int numValue))
                        {
                            response.Status = "Failure";
                            response.StatusMessage = "Data Format is incorrect on row " + (j + 2);
                            return response;
                        }
                    }
                    if (dtContent.Columns[i].ColumnName == "RoomType" && !String.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()) && !(RoomTypeList.Contains(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString().ToUpper())))
                    {
                        response.Status = "Failure";
                        response.StatusMessage = "Room Type not Enumeration" + (j + 2);
                        return response;
                    }
                    if (dtContent.Columns[i].ColumnName == "Sex" && !String.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()) && !(SexName.Contains(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString().Substring(0, 1))))
                    {
                        response.Status = "Failure";
                        response.StatusMessage = "Sex Type not Enumeration" + (j + 2);
                        return response;

                    }
                    if (dtContent.Columns[i].ColumnName == "Date of Birth")
                    {
                        //CultureInfo info = CultureInfo.InstalledUICulture;
                        // var format =  info.DateTimeFormat.s;

                        if (!string.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()))
                        {

                            DateTime? dtnew = CheckForDate(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString());
                            if (dtnew == null)
                            {
                                response.Status = "Failure";
                                response.StatusMessage = " DateofBirth Format is not in dd/MM/yyyy format on row " + (j + 2);
                                return response;
                            }
                        }
                    }
                    if (dtContent.Columns[i].ColumnName == "Date of Issue")
                    {
                        if (!string.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()))
                        {
                            DateTime? dtnew = CheckForDate(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString());
                            if (dtnew == null)
                            {
                                response.Status = "Failure";
                                response.StatusMessage = " Date of Issue Format is not in dd/MM/yyyy format on row " + (j + 2);
                                return response;
                            }
                        }
                    }
                    if (dtContent.Columns[i].ColumnName == "Date of Expiry")
                    {
                        if (!string.IsNullOrEmpty(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString()))
                        {
                            DateTime? dtnew = CheckForDate(dtContent.Columns[i].Table.Rows[j].ItemArray[i].ToString());
                            if (dtnew == null)
                            {
                                response.Status = "Failure";
                                response.StatusMessage = " Date of Expiry Format is not in dd/MM/yyyy format on row" + (j + 2);
                                return response;
                            }
                        }
                    }
                }
            }
            return response;
        }

        private DateTime? CheckForDate(string str)
        {
            DateTime? dt = null;
            string[] strDT = new string[0];
            if (str.Contains(" "))
            {
                string[] strDTforSpace = new string[0];
                strDTforSpace = str.Split(" ");
                string sdr = strDTforSpace[0];
                strDT = sdr.ToString().Split("/");
                if (strDT.Count() == 3)
                {
                    if ((Convert.ToInt32(strDT[1]) > 0 && Convert.ToInt32(strDT[1]) <= 12))
                    {
                        if (Convert.ToInt32(strDT[1]) == 1 || Convert.ToInt32(strDT[1]) == 3 || Convert.ToInt32(strDT[1]) == 5 || Convert.ToInt32(strDT[1]) == 7 || Convert.ToInt32(strDT[1]) == 9 || Convert.ToInt32(strDT[1]) == 11)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 31))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                        else if (Convert.ToInt32(strDT[1]) == 4 || Convert.ToInt32(strDT[1]) == 6 || Convert.ToInt32(strDT[1]) == 8 || Convert.ToInt32(strDT[1]) == 10 | Convert.ToInt32(strDT[1]) == 12)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 30))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));

                            }
                        }
                        else if (((Convert.ToInt32(strDT[2]) % 4 == 0 && Convert.ToInt32(strDT[2]) % 100 != 0) || (Convert.ToInt32(strDT[2]) % 400 == 0)) && Convert.ToInt32(strDT[1]) == 2)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 29))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                        else
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 28))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                    }
                }
            }
            else
            {
                strDT = str.ToString().Split("/");
                if (strDT.Count() == 3)
                {
                    if ((Convert.ToInt32(strDT[1]) > 0 && Convert.ToInt32(strDT[1]) <= 12))
                    {
                        if (Convert.ToInt32(strDT[1]) == 1 || Convert.ToInt32(strDT[1]) == 3 || Convert.ToInt32(strDT[1]) == 5 || Convert.ToInt32(strDT[1]) == 7 || Convert.ToInt32(strDT[1]) == 9 || Convert.ToInt32(strDT[1]) == 11)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 31))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                        else if (Convert.ToInt32(strDT[1]) == 4 || Convert.ToInt32(strDT[1]) == 6 || Convert.ToInt32(strDT[1]) == 8 || Convert.ToInt32(strDT[1]) == 10 | Convert.ToInt32(strDT[1]) == 12)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 30))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                        else if (((Convert.ToInt32(strDT[2]) % 4 == 0 && Convert.ToInt32(strDT[2]) % 100 != 0) || (Convert.ToInt32(strDT[2]) % 400 == 0)) && Convert.ToInt32(strDT[1]) == 2)
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 29))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                        else
                        {
                            if ((Convert.ToInt32(strDT[0]) > 0 && Convert.ToInt32(strDT[0]) <= 28))
                            {
                                dt = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                            }
                        }
                    }
                }
            }
            return dt;
        }

        private static DataTable GetDataTableFromExcel(IFormFile file, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = file.OpenReadStream())
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value;
                    }
                }
                return tbl;
            }
        }
        #endregion

        #region ItineraryBuilder
        [Route("ViewItineraryBuilderSummary")]
        public IActionResult ViewItineraryBuilderSummary(string BookingNumber)
        {
            OpsItineraryBuilderSummaryViewModel model = new OpsItineraryBuilderSummaryViewModel();

            #region Get Booking Details 
            ProductSRPHotelGetReq BookingReq = new ProductSRPHotelGetReq() { QRFID = BookingNumber };
            OpsBookingSummaryGetRes BookingRes = operationsProviders.GetOpsBookingSummary(BookingReq, token).Result;
            #endregion

            if (BookingRes != null)
            {
                #region Get Costing Officer Tour Info Header By QRFId
                model.COHeader.QRFID = BookingRes.OpsBookingSummaryDetails.QRFID;
                model.COHeader.TourName = BookingNumber + " : " + BookingRes.OpsBookingSummaryDetails.TourName;
                model.COHeader.NoOfNights = BookingRes.OpsBookingSummaryDetails.NoOfNights;
                model.COHeader.NoOfDays = BookingRes.OpsBookingSummaryDetails.NoOfDays;
                model.COHeader.Destination = BookingRes.OpsBookingSummaryDetails.Destination;

                model.COHeader.TravelDate = BookingRes.OpsBookingSummaryDetails.StartDate;
                model.COHeader.EndDate = BookingRes.OpsBookingSummaryDetails.EndDate;
                model.COHeader.SalesPerson = BookingRes.OpsBookingSummaryDetails.SalesOfficerEmail;
                model.COHeader.CostingOfficer = BookingRes.OpsBookingSummaryDetails.CostingOfficerEmail;
                model.COHeader.FileHandler = BookingRes.OpsBookingSummaryDetails.FileHandlerEmail;
                model.COHeader.ProductAccountant = BookingRes.OpsBookingSummaryDetails.ProductAccountantEmail;
                model.COHeader.ModuleName = "ops";
                model.COHeader.BookingNumber = BookingNumber;
                #endregion
            }
            return View(model);
            //return PartialView("~/Views/QRFSummary/_QRFSummary.cshtml", qRFSummaryViewModel);
        }

        /// <summary>
        /// ViewServiceItinerary :-this will call on click of Itinerary Button
        /// and get the details of Itinerary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("ViewItineraryBuilder")]
        public IActionResult _ViewItineraryBuilder(OpsBookingItineraryGetReq request)
        {
            QRFSummaryViewModel qRFSummaryViewModel = new QRFSummaryViewModel();

            qRFSummaryViewModel.Days = request.DayName;
            qRFSummaryViewModel.Services = request.ProductType;
            var day = request.DayName == "Day" ? null : request.DayName;
            var prodType = request.ProductType == "Service Type" ? null : request.ProductType;

            #region Get Ops Service Itinerary Details  
            OpsBookingItineraryGetReq opsBookingGetReq = new OpsBookingItineraryGetReq() { BookingNumber = request.BookingNumber, DayName = day, ProductType = prodType };
            OpsBookingItineraryGetRes opsBookingGetRes = operationsProviders.GetItineraryBuilderDetails(opsBookingGetReq, token).Result;
            qRFSummaryViewModel.QRFID = request.BookingNumber;

            if (opsBookingGetRes != null)
            {
                qRFSummaryViewModel.DayList = opsBookingGetRes.OpsItineraryDetails.Days.Select(x => new Attributes() { Attribute_Id = x, AttributeName = x }).ToList();
                qRFSummaryViewModel.ServiceTypeList = opsBookingGetRes.OpsItineraryDetails.ServiceType.Select(x => new mProductType() { Prodtype = x }).ToList();
                qRFSummaryViewModel.Itinerary.ItineraryDays = opsBookingGetRes.OpsItineraryDetails.OpsItinenraryDays.Select(x => new ItineraryDaysInfo()
                {
                    City = x.CityNames,
                    DayOfWeek = x.OpsItineraryDayDetails.Select(y => y.STARTDayOfWeek).FirstOrDefault(),
                    Day = x.DayName,
                    Date = Convert.ToDateTime(x.OpsItineraryDayDetails.Select(y => y.STARTDATE).FirstOrDefault()),
                    ItineraryName = x.CityNames,
                    ItineraryDescription = x.OpsItineraryDayDetails.Select(y => new ItineraryDescriptionInfo()
                    {
                        PositionId = y.PositionId,
                        StartTime = y.STARTTIME,
                        EndTime = (!string.IsNullOrWhiteSpace(y.ENDTIME) && !string.IsNullOrWhiteSpace(y.UniqueIdentityValue)) ? (y.ENDTIME + "|" + y.UniqueIdentityValue) : (string.IsNullOrWhiteSpace(y.ENDTIME) && !string.IsNullOrWhiteSpace(y.UniqueIdentityValue)) ? "0:00|" + y.UniqueIdentityValue : y.ENDTIME,
                        ProductType = y.ProductType,
                        City = y.CityName,
                        ProductName = y.ProductName,
                        TLRemarks = y.TLRemarks,
                        OPSRemarks = y.OPSRemarks,
                        ProductId = y.ItineraryDetailId, //Assigning for Edit and Delete functionality
                        IsDeleted = y.IsDeleted,
                        ItineraryRemarks = y.ItineraryRemarks
                        //Supplier = y.Supplier,
                        //Allocation = y.Allocation,
                        //NumberOfPax = Convert.ToInt32(y.NoOfPax)
                    }).ToList(),
                }).ToList();
                qRFSummaryViewModel.Positions = opsBookingGetRes.OpsItineraryDetails.OpsPositions;
            }
            #endregion
            return PartialView(qRFSummaryViewModel);
        }

        [Route("GetRemarksForPosition")]
        public IActionResult GetRemarksForPosition(string BookingNumber, string PositionId)
        {
            try
            {
                OriginalItineraryDetails model = new OriginalItineraryDetails();

                #region Get Itinerary Details by QRFId
                OpsBookingItineraryGetReq itineraryGetReq = new OpsBookingItineraryGetReq() { BookingNumber = BookingNumber, PositionId = PositionId };
                OpsBookingItineraryGetRes itineraryGetRes = operationsProviders.GetItineraryBuilderPositionDetailById(itineraryGetReq, token).Result;
                #endregion

                if (itineraryGetRes != null && itineraryGetRes.OpsItineraryDetails != null)
                {
                    model.TLRemarks = itineraryGetRes.ItineraryDetails.TLRemarks;
                    model.OPSRemarks = itineraryGetRes.ItineraryDetails.OPSRemarks;
                    model.PositionId = itineraryGetRes.ItineraryDetails.Position_Id;
                    model.ItineraryId = itineraryGetRes.ItineraryDetails.ItineraryDetail_Id;
                    model.BookingNumber = BookingNumber;
                }
                return PartialView("_Remarks", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("EditItineraryDetails")]
        public IActionResult EditItineraryDetails(string BookingNumber, string ItineraryDetailID, string ProdDescription)
        {
            ExtraPosition position = new ExtraPosition();
            position.ProductName = ProdDescription;
            position.QRFID = BookingNumber;
            position.ItineraryID = ItineraryDetailID;

            return PartialView("EditItineraryDetails", position);
        }

        [Route("DeleteItineraryDetails")]
        public IActionResult DeleteItineraryDetails(string BookingNumber, string ItineraryDetailID, bool IsDeleted)
        {
            if (!string.IsNullOrWhiteSpace(BookingNumber))
            {
                OpsBookingItinerarySetReq req = new OpsBookingItinerarySetReq()
                {
                    BookingNumber = BookingNumber,
                    Type = "UpdateDeleted",
                    ItineraryDetails = new ItineraryDetails()
                    {
                        ItineraryDetail_Id = ItineraryDetailID,
                        IsDeleted = IsDeleted == true ? false : true
                    }
                };
                OpsBookingItinerarySetRes res = operationsProviders.SetRemarksForItineraryBuilderDetails(req, token).Result;
                if (res.ResponseStatus.Status.ToLower() == "success")
                {
                    return Json(new { responseText = "Success! Description updated successfully." });
                }
                else
                {
                    return Content("Error");
                }
            }
            else
            {
                return Content("Error");
            }
        }

        [HttpPost]
        [Route("SaveItineraryDetails")]
        public IActionResult SaveItineraryDetails(QRFSummaryViewModel model)
        {
            //return Content("Error");

            List<ItineraryDetails> list = new List<ItineraryDetails>();

            foreach (var a in model.Itinerary.ItineraryDays)
            {
                foreach (var d in a.ItineraryDescription)
                {
                    ItineraryDetails obj = new ItineraryDetails() { ItineraryDetail_Id = d.ProductId, ItineraryRemarks = d.ItineraryRemarks };
                    list.Add(obj);
                }
            }
            OpsBookingItinerarySetReq req = new OpsBookingItinerarySetReq() { BookingNumber = model.QRFID, lstItineraryDetails = list };
            OpsBookingItinerarySetRes res = operationsProviders.SetItineraryBuilderDetails(req, token).Result;
            if (res.ResponseStatus.Status.ToLower() == "success")
            {
                return Json(new { responseText = "Success! Details updated successfully.", status = "success" });
            }
            else
            {
                return Json(new { responseText = "Error! Details not updated successfully.", status = "error" });
            }
        }

        [Route("SaveItineraryDescriptionDetails")]
        public IActionResult SaveItineraryDescriptionDetails(string BookingNumber, string ItineraryDetailID, string ProdDescription)
        {
            if (!string.IsNullOrWhiteSpace(BookingNumber))
            {
                if (string.IsNullOrWhiteSpace(ProdDescription))
                {
                    return Json(new { responseText = "Error! Please Enter Description.", status = "Error" });
                }
                else
                {
                    OpsBookingItinerarySetReq req = new OpsBookingItinerarySetReq()
                    {
                        BookingNumber = BookingNumber,
                        Type = "UpdateDescription",
                        ItineraryDetails = new ItineraryDetails()
                        {
                            ItineraryDetail_Id = ItineraryDetailID,
                            Description = ProdDescription
                        }
                    };
                    OpsBookingItinerarySetRes res = operationsProviders.SetRemarksForItineraryBuilderDetails(req, token).Result;
                    if (res.ResponseStatus.Status.ToLower() == "success")
                    {
                        return Json(new { responseText = "Success! Description updated successfully.", status = "Success" });
                    }
                    else
                    {
                        return Json(new { responseText = "Error! Error in updating description.", status = "Error" });
                    }
                }
            }
            else
            {
                return Json(new { responseText = "Error! Booking Number not found.", status = "Success" });
            }
        }

        [Route("OPSAddService")]
        public IActionResult OPSAddService(string BookingNumber)
        {
            OpsAddPositionViewModel model = new OpsAddPositionViewModel();
            try
            {
                //#region Bind Position Type
                //MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
                //objMasterTypeRequest.Property = "QRF Masters";
                //objMasterTypeRequest.Name = "Position Type";

                //SalesProviders objSalesProvider = new SalesProviders(_configuration);
                //MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

                //if (objMasterTypeResponse?.Status == "Success" && objMasterTypeResponse?.PropertyList?.Count > 0)
                //{
                //    if (objMasterTypeResponse.PropertyList[0].Attribute[0]?.AttributeName == "Position Type")
                //    {
                //        model.PostionTypeList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values?.ToList();
                //    }
                //}
                //#endregion
                model.PositionType = "Supplement";
                model.NoOfDays = "1";
                model.StartTime = "12:00";
                model.EndTime = "03:00";
                return PartialView("_AddService", model);
            }
            catch (Exception ex)
            {
                return PartialView("_AddService", model);
            }
        }
        #endregion

        #region ViewServiceStatus
        [Route("ViewServiceStatus")]
        public IActionResult ViewServiceStatus(OpsBookingGetReq objOpsBookingGetReq)
        {
            OpsViewServiceVM model = new OpsViewServiceVM() { COHeader = new COHeaderViewModel() };

            if (TempData["posid"] != null)
            {
                objOpsBookingGetReq.PositionId = TempData["posid"].ToString();
            }
            #region Get Ops Service Header details
            OpsHeaderGetReq opsHeaderGetReq = new OpsHeaderGetReq() { BookingNumber = objOpsBookingGetReq.BookingNumber, PositionId = objOpsBookingGetReq.PositionId };
            OperationHeaderInfo operationHeaderInfo = operationsProviders.GetOperationHeaderDetails(opsHeaderGetReq, token).Result;
            if (operationHeaderInfo != null)
            {
                model.COHeader.TourName = operationHeaderInfo.BookingNumber + " : " + operationHeaderInfo.TourName;
                model.COHeader.NoOfNights = operationHeaderInfo.NoOfNights;
                model.COHeader.NoOfDays = operationHeaderInfo.NoOfDays;
                model.COHeader.Destination = operationHeaderInfo.Destination;
                model.COHeader.TravelDate = operationHeaderInfo.StartDate;
                model.COHeader.EndDate = operationHeaderInfo.EndDate;
                model.COHeader.SalesPerson = operationHeaderInfo.SalesOfficerEmail;
                model.COHeader.CostingOfficer = operationHeaderInfo.CostingOfficerEmail;
                model.COHeader.FileHandler = operationHeaderInfo.FileHandlerEmail;
                model.COHeader.ProductAccountant = operationHeaderInfo.ProductAccountantEmail;
                model.COHeader.BookingNumber = operationHeaderInfo.BookingNumber;
                model.COHeader.AgentName = operationHeaderInfo.AgentName;
                model.COHeader.ModuleName = "ops";
            }
            #endregion

            #region Get Ops Tabs
            model.MenuViewModel.BookingNumber = operationHeaderInfo.BookingNumber;
            model.MenuViewModel.PositionId = objOpsBookingGetReq.PositionId;
            if (string.IsNullOrEmpty(objOpsBookingGetReq.PositionId))
            {
                model.MenuViewModel.MenuName = "Itinerary";
            }
            else
            {
                model.MenuViewModel.MenuName = operationHeaderInfo.UIProductType;
                model.MenuViewModel.MenuName = string.IsNullOrEmpty(model.MenuViewModel.MenuName) ? "Itinerary" : model.MenuViewModel.MenuName;
            }
            #endregion

            return View(model);
        }

        [Route("GetServiceStatusParam")]
        public IActionResult GetServiceStatusParam(OpsBookingGetReq objOpsBookingGetReq)
        {
            TempData["posid"] = objOpsBookingGetReq.PositionId;
            return RedirectToAction("ViewServiceStatus", new { BookingNumber = objOpsBookingGetReq.BookingNumber });
        }

        #region Itinerary Tab
        /// <summary>
        /// ViewServiceItinerary :-this will call on click of Itinerary Button
        /// and get the details of Itinerary
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("ViewServiceItinerary")]
        public IActionResult _ViewServiceItinerary(OpsBookingItineraryGetReq request)
        {
            OpsItineraryDetails model = new OpsItineraryDetails();

            #region Get Ops Service Itinerary Details  
            OpsBookingItineraryGetRes bookingRes = operationsProviders.GetBookingItineraryDetails(request, token).Result;
            if (bookingRes != null)
            {
                model = bookingRes.OpsItineraryDetails;
            }
            #endregion

            return PartialView("_ViewServiceItinerary", model);
        }
        #endregion

        #region ProductType Tabs
        /// <summary>
        /// ViewProductTypeDetails :this will call on click of ProductType Tab as Acco,Meals,Flights,etc
        /// and gets the details of DayList details and takes single position details order by StartDateTime
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("ViewProductTypeDetails")]
        public IActionResult ViewProductTypeDetails(OpsProductTypeGetReq request)
        {
            OpsProdTypeVM model = new OpsProdTypeVM() { OpsProdTypeDaysMenuVM = new OpsProdTypeDaysMenuVM(), ProdTypePositions = new ProdTypePositions() };
            try
            {
                #region Get Ops product Type Details  
                var response = operationsProviders.GetOpsProductTypeDetails(request, token).Result;
                if (response != null && response.OpsProductTypeCommonFields != null)
                {
                    model = BindOPSProductTypeDetails(response);
                }
                else
                {
                    response = new OpsProductTypeGetRes();
                    response.Position = new Positions();
                }
                #endregion
                return View("ViewProductTypeDetails", model);
            }
            catch (Exception ex)
            {
                return View("ViewProductTypeDetails", model);
            }
        }

        /// <summary>
        /// ViewProdTypePosition:- this will call on click of Days tab
        /// and get the details of position of that Day order by StartDateTime
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("ViewProdTypePosition")]
        public IActionResult _ViewProdTypePosition(OpsProdTypePositionGetReq request)
        {
            OpsProdTypeVM model = new OpsProdTypeVM();
            try
            {
                #region Get Ops Service Itinerary Details  
                var response = operationsProviders.GetProdTypePositionByParam(request, token).Result;
                if (response != null && response.OpsProductTypeCommonFields != null)
                {
                    model = BindOPSProductTypeDetails(response);
                }
                else
                {
                    response.Position = new Positions();
                }
                #endregion
                return PartialView("_ViewProdTypePosition", model);
            }
            catch (Exception ex)
            {
                return PartialView("_ViewProdTypePosition", model);
            }
        }

        /// <summary>
        /// BindOPSProductTypeDetails :-this function will retrun the OpsProdTypeVM model 
        /// If it calls from ViewProductTypeDetails action then it will return DaysList and single position details order by StartdateTime
        /// If it calls from ViewProdTypePosition action then it will return only single position details order by StartDateTime
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public OpsProdTypeVM BindOPSProductTypeDetails(OpsProductTypeGetRes response)
        {
            OpsProdTypeVM model = new OpsProdTypeVM();
            try
            {
                model.BookingNumber = response.OpsProductTypeCommonFields.BookingNumber;
                model.DayName = model.OpsProdTypeDaysMenuVM.DayName = response.OpsProductTypeCommonFields.DayName;
                model.Position_Id = response.OpsProductTypeCommonFields.PositionId;
                model.ProductType = response.OpsProductTypeCommonFields.ProductType.ToUpper();
                model.IsRealSupplier = response.OpsProductTypeCommonFields.IsRealSupplier;
                model.OpsProdTypeDaysMenuVM.DaysList = response.DayList;
                model.TimeSlabsList = GenerateScheduleTimeSlabs(response);
                model.ProductId = response.Position.Product_Id;
                model.SystemCompany_Id = response.OpsProductTypeCommonFields.SystemCompany_Id;

                if (response.Position != null && !string.IsNullOrEmpty(response.Position.Position_Id))
                {
                    model.PositionStatus = response.Position.STATUS;
                    //var ChargeBasisList = masterProviders.GetDefChargeBasis(token).Result;
                    var PersonTypeList = masterProviders.GetPersonType(token).Result;

                    var pos = response.Position;
                    pos.BookingRoomsAndPrices.ForEach(a => a.Status = a.Status == null ? "" : a.Status);
                    pos.BookingRoomsAndPrices.RemoveAll(a => a.Req_Count != null && a.Req_Count <= 0);
                    List<string> ExcludeStatusForBRP = new List<string>() { "X", "J", "C", "-", "!" };
                    model.ChargeBasis = response.OpsProductTypeCommonFields.ChargeBasis;
                    string FOCPersonTypeByChargeBasis = model.ChargeBasis == "PU" || model.ChargeBasis == "PUPD" ? "UNIT" : "ADULT";

                    #region Get Rooms dropodown in FOC section
                    ProdCategoryRangeGetReq req = new ProdCategoryRangeGetReq() { ProductId = new List<string>() { pos.Product_Id } };
                    ProdCategoryRangeGetRes res = masterProviders.GetProductCategoryRangeByProductID(req, token).Result;
                    #endregion

                    #region Get Financials details
                    OpsFinancialsGetReq freq = new OpsFinancialsGetReq() { BookingNumber = model.BookingNumber, PositionId = pos.Position_Id };
                    OpsFinancialsGetRes fres = operationsProviders.GetOpsFinancialDetails(freq, token).Result;
                    #endregion

                    #region Get Hotel Location, Breakfast Type, Board Basis
                    var BoardBasisList = new List<AttributeValues>();
                    var DefMealTypeList = new List<AttributeValues>();
                    var modelMasterData = salesLibrary.BindMasterData(_configuration, token);
                    var objDefMealPlanGetRes = masterProviders.GetDefMealPlan(new DefMealPlanGetReq { Status = "false" }, token).Result;
                    if (objDefMealPlanGetRes?.ResponseStatus?.Status == "Success" && objDefMealPlanGetRes?.mDefMealPlan?.Count > 0)
                    {
                        BoardBasisList = objDefMealPlanGetRes.mDefMealPlan.Select(a => new AttributeValues { AttributeValue_Id = a.MealPlan, Value = a.Description }).OrderBy(a => a.Value).ToList();
                    }
                    var DefMealTypeRes = masterProviders.GetMealType(new DefMealTypeGetReq { ForBreakfast = true }, token).Result;
                    if (DefMealTypeRes?.ResponseStatus?.Status == "Success" && DefMealTypeRes?.DefMealType?.Count > 0)
                    {
                        DefMealTypeList = DefMealTypeRes.DefMealType.Select(a => new AttributeValues { AttributeValue_Id = a.MealType, Value = a.MealType_Name }).OrderBy(a => a.Value).ToList();
                    }
                    #endregion


                    #region Get OPS Users from mCompanies.ContactDetails
                    var OpsUsersFileHandler = agentProviders.GetCompanyContacts(new CompanyOfficerGetReq { CompanyId = model.SystemCompany_Id }, token).Result;
                    var FileHandlerList = OpsUsersFileHandler.ContactDetails.Where(a => string.IsNullOrWhiteSpace(a.STATUS) && (a.Roles != null && a.Roles.Count > 0) && a.Roles.Any(b => b.RoleName == "Groups" || b.RoleName == "FIT"))
                                            .Select(a => new AttributeValues { AttributeValue_Id = a.MAIL, Value = a.FIRSTNAME + " " + a.LastNAME }).ToList();
                    #endregion

                    model.ProdTypePositions = new ProdTypePositions()
                    {
                        ProductsSRP = new ProductsSRPViewModel
                        {
                            PageName = "hotel",
                            ProductSRPDetails = new List<ProductSRPDetails> { response.OpsProductTypeDetails.ProductSRPDetails }
                        },

                        SuppFOC = new OpsPosFOCViewModel()
                        {
                            Booking_Id = response.OpsProductTypeCommonFields.BookingId,
                            BookingNo = response.OpsProductTypeCommonFields.BookingNumber,
                            Position_Id = pos.Position_Id,
                            lstPosFOC = (pos != null && pos.PositionFOC != null && pos.PositionFOC.Count > 0) ? pos.PositionFOC.Select(x => new OpsPositionFOC()
                            {
                                PositionFOC_Id = x.PositionFOC_Id,
                                BuyQuantity = x.BuyQuantity,
                                BuyBookingRoomsId = x.BuyBookingRooms_ID,
                                BuyRoomShortCode = x.BuyRoomShortCode,
                                GetQuantity = x.GetQuantity,
                                GetBookingRoomsId = x.GetBookingRooms_ID,
                                GetRoomShortCode = x.GetRoomShortCode,
                                CREA_DT = x.AuditTrail.CREA_DT,
                                CREA_US = x.AuditTrail.CREA_US,
                                MODI_DT = x.AuditTrail.MODI_DT,
                                MODI_US = x.AuditTrail.MODI_US
                            }).ToList() : new List<OpsPositionFOC>() { new OpsPositionFOC() { PositionFOC_Id = Guid.NewGuid().ToString() } },

                            lstBookingRooms = res.ProdCategoryRangeDetails?.Where(a => a.AdditionalYN == false && a.PersonType?.ToUpper() == FOCPersonTypeByChargeBasis)
                            .Select(a => new ProductRangeDetails
                            {
                                VoyagerProductRange_Id = a.VoyagerProductRange_Id,
                                ProductRangeName = "(" + a.ProductCategoryName + ") " + a.ProductRangeName,
                            }).OrderBy(x => x.ProductRangeName).ToList() ?? new List<ProductRangeDetails>(),
                        },

                        RoomsAndRates = new OpsPosRoomsPricesViewModel()
                        {
                            BookingRoomsAndPrices = (pos.BookingRoomsAndPrices?.Count > 0) ? pos.BookingRoomsAndPrices.Where(a => a.Status?.ToUpper() != "X" && !a.IsAdditionalYN && !ExcludeStatusForBRP.Contains(a.Status))?.Select(x => new OpsPositionRoomPrice()
                            {
                                Booking_Id = x.Booking_Id,
                                Position_Id = x.Position_Id,
                                BookingRooms_Id = x.BookingRooms_Id,
                                PositionPricing_Id = x.PositionPricing_Id,
                                Req_Count = x.Req_Count,
                                ChargeBasis = x.ChargeBasis,
                                PersonTypeID = x.PersonType_Id,
                                Status = x.Status,
                                BudgetPrice = x.BudgetPrice,
                                RequestedPrice = x.RequestedPrice,
                                BuyPrice = x.BuyPrice,
                                ApplyMarkup = Convert.ToBoolean(x.ApplyMarkup),
                                ExcludeFromInvoice = Convert.ToBoolean(x.ExcludeFromInvoice),
                                ConfirmedReqPrice = Convert.ToBoolean(x.ConfirmedReqPrice),
                                BuyCurrency_Name = x.BuyCurrency_Name,
                                ProductRangeID = x.ProductRange_Id,
                                PersonType = x.PersonType,
                                ProductRange = "(" + x.CategoryName + ") " + x.RoomName,
                                IsNewRow = false,
                            }).ToList() : new List<OpsPositionRoomPrice>() { new OpsPositionRoomPrice() { BookingRooms_Id = Guid.NewGuid().ToString(), PositionPricing_Id = Guid.NewGuid().ToString(), BuyCurrency_Name = pos.BookingRoomsAndPrices?.Count > 0 ? pos?.BookingRoomsAndPrices[0]?.BuyCurrency_Name : "", IsNewRow = true } },

                            BookingRoomsAndPricesSupp = (pos.BookingRoomsAndPrices?.Count > 0) && pos.BookingRoomsAndPrices.Where(a => a.IsAdditionalYN && !ExcludeStatusForBRP.Contains(a.Status))?.ToList().Count > 0 ? pos.BookingRoomsAndPrices.Where(a => a.IsAdditionalYN && !ExcludeStatusForBRP.Contains(a.Status))?.Select(x => new OpsPositionRoomPrice()
                            {
                                Booking_Id = x.Booking_Id,
                                Position_Id = x.Position_Id,
                                BookingRooms_Id = x.BookingRooms_Id,
                                PositionPricing_Id = x.PositionPricing_Id,
                                Req_Count = x.Req_Count,
                                Age = x.Age,
                                ChargeBasis = x.ChargeBasis,
                                PersonTypeID = x.PersonType_Id,
                                Status = x.Status,
                                BudgetPrice = x.BudgetPrice,
                                RequestedPrice = x.RequestedPrice,
                                BuyPrice = x.BuyPrice,
                                ApplyMarkup = Convert.ToBoolean(x.ApplyMarkup),
                                ExcludeFromInvoice = Convert.ToBoolean(x.ExcludeFromInvoice),
                                ConfirmedReqPrice = Convert.ToBoolean(x.ConfirmedReqPrice),
                                BuyCurrency_Name = x.BuyCurrency_Name,
                                ProductRangeID = x.ProductRange_Id,
                                PersonType = x.PersonType,
                                OneOffDate = x.OneOffDate?.ToString("dd/MM/yyyy").Replace("-", "/"),
                                ProductRange = "(" + x.CategoryName + ") " + x.RoomName,
                                IsNewRow = false,
                            }).ToList() : new List<OpsPositionRoomPrice>() { new OpsPositionRoomPrice() { BookingRooms_Id = Guid.NewGuid().ToString(), PositionPricing_Id = Guid.NewGuid().ToString(), BuyCurrency_Name = pos.BookingRoomsAndPrices?.Count > 0 ? pos?.BookingRoomsAndPrices[0]?.BuyCurrency_Name : "", IsNewRow = true } },

                            BudgetSupplements = (pos.BudgetSupplements?.Count > 0 && pos.BudgetSupplements.Where(a => !ExcludeStatusForBRP.Contains(a.status))?.ToList().Count > 0) ? pos.BudgetSupplements.Select(x => new OpsBudgetSupplements()
                            {
                                BudgetSupplement_Id = x.BudgetSupplement_Id,
                                ProductRangeID = pos.BookingRoomsAndPrices?.Where(a => a.BookingRooms_Id == x.BookingRooms_Id).Select(a => a.ProductRange_Id).FirstOrDefault(),
                                ProductRange = x.RoomShortCode,
                                BookingRooms_Id = x.BookingRooms_Id,
                                PositionPricing_Id = x.PositionPricing_Id,
                                BudgetSupplementAmount = x.BudgetSupplementAmount,
                                BudgetSupplementReason = x.BudgetSupplementReason,
                                ApplyMarkUp = x.ApplyMarkUp,
                                AgentConfirmed = x.AgentConfirmed,
                                status= x.status,
                                IsNewRow = false
                            }).ToList() : new List<OpsBudgetSupplements>() { new OpsBudgetSupplements() { BudgetSupplement_Id = Guid.NewGuid().ToString(), IsNewRow = true } },

                            RangeList = res.ProdCategoryRangeDetails?.Where(a => Convert.ToBoolean(a.AdditionalYN) == false).ToList() ?? new List<ProductRangeDetails>(),
                            RangeListSupp = res.ProdCategoryRangeDetails?.Where(a => a.AdditionalYN == true).ToList() ?? new List<ProductRangeDetails>(),
                            //RangeListBudgSupp = res.ProdCategoryRangeDetails?.ToList() ?? new List<ProductRangeDetails>(),
                            //ChargeBasisList = ChargeBasisList?.DefChargeBasis,
                            PersonTypeList = PersonTypeList?.DefPersonType
                        },

                        Financials = new OpsPosFinancials()
                        {
                            FinancialDetails = fres.FinancialDetail,
                            TotalBuyCurrency = fres.TotalBuyCurrency,
                            TotalBuyPrice = fres.TotalBuyPrice,
                            TotalSellCurrency = fres.TotalSellCurrency,
                            TotalSellPrice = fres.TotalSellPrice,
                            TotalGPAmount = fres.TotalGPAmount,
                            TotalGPPercent = fres.TotalGPPercent
                        },

                        ProductAdditionalInfo = new OpsPositionAdditionalInfo
                        {
                            #region Common Attributes
                            ProdType = pos.ProductType.ToUpper(),
                            FileHandlerID = pos.HotelPLacer_ID,
                            FileHandler = pos.HotelPLacer_Name,
                            PositionType = pos.PositionType,
                            PaxType = pos.StandardRooming?.ToString(),
                            StartDate = pos.STARTDATE?.ToString("dd/MM/yyyy").Replace("-", "/"),
                            StartTime = pos.STARTTIME,
                            StartLocation = pos.STARTLOC,
                            EndDate = pos.ENDDATE?.ToString("dd/MM/yyyy").Replace("-", "/"),
                            EndTime = pos.ENDTIME,
                            EndLocation = pos.ENDLOC,
                            OptionDate = pos.OPTIONDATE?.ToString("dd/MM/yyyy").Replace("-", "/"),
                            ConfirmDate = pos.ConfirmDate?.ToString("dd/MM/yyyy").Replace("-", "/"),       //No field in OPS. New field to be added in Mongo 
                            CancellationDate = pos.CancellationDate?.ToString("dd/MM/yyyy").Replace("-", "/"),
                            #endregion

                            #region Hotel Attributes
                            BoardBasis = pos.HOTELMEALPLAN,
                            BreakfastType = pos.BreakFastType,
                            Porterage = pos.Porterage?.ToString(),
                            VoucherNote = pos.VoucherNote,
                            CancellationPolicy = pos.CancellationPolicy,
                            CityTaxAdvise = pos.CityTaxAdvise,
                            TLRemarks = pos.TLRemarks,
                            OpsRemarks = pos.OPSRemarks,
                            #endregion

                            #region Activities Attributes
                            TicketLocation = pos.TicketLocation,
                            TrainNumber = pos.TrainNumber,
                            GuidePurchaseTicket = pos.GuidePurchaseTicket,
                            Itinerary = pos.Itinerary,
                            //AttrCancellationPolicy = pos.CancellationPolicy,
                            #endregion

                            #region Bus Attributes
                            DriverName = pos.DriverName,
                            DriverContact = pos.DriverContactNumber,
                            DriverLanguages = pos.DriverLanguage,
                            DriverLicenceNumber = pos.DriverLicenceNumber,
                            NumberofDriverRooms = pos.NumberOfDriverRooms.ToString(),
                            TypeofRoom = pos.TypeOfRoom,
                            MealsIncluded = pos.MealsIncluded ?? false,
                            VehicleRegistration = pos.VehicleRegistration,
                            ManufacturedDate = pos.ManufacturedDate?.ToString(),
                            Parking = pos.Parking,
                            CityPermit = pos.CityPermits,
                            RoadTolls = pos.RoadTolls,
                            AC = pos.AC,
                            WC = pos.WC,
                            Safety = pos.Safety,
                            GPS = pos.GPS,
                            AV = pos.AV,
                            //BusItinerary = pos.Itinerary,
                            //BusCancellationPolicy = pos.CancellationPolicy,
                            #endregion

                            #region Guide Attributes
                            GuideName = pos.DriverName,
                            GuideContact = pos.DriverContactNumber,
                            //NumberofGuideRooms = pos.NumberOfDriverRooms.ToString(),
                            //GuideTypeofRoom = pos.TypeOfRoom,
                            //GuideMeals = pos.MealsIncluded ?? false,
                            GuideTickets = pos.TicketsIncluded ?? false,
                            //GuideItinerary = pos.Itinerary,
                            //GuideCancellationPolicy = pos.CancellationPolicy,
                            #endregion

                            #region Meal Attributes
                            Floor = pos.Floor,
                            CoachParking = pos.CoachParkingAvailable.ToString(),
                            MealStyle = pos.MealStyle,
                            Courses = pos.Course,
                            Tea = pos.Tea,
                            Dessert = pos.Dessert,
                            Water = pos.Water,
                            Bread = pos.Bread,
                            MealMenu = pos.Menu,
                            //MealCancellationPolicy = pos.CancellationPolicy,
                            #endregion

                            #region Flight Attributes
                            FlightNumber = pos.FlightNumber,
                            #endregion

                            //#region FERRY TRANSFER, FERRY PASSENGER, OVERNIGHT FERRY & SIGHTSEEING - CITYTOUR Attributes
                            //PickupPoint = pos.PickupPoint,
                            //DropOffPoint = pos.DropOffPoint,
                            //#endregion

                            #region Dropdown Lists
                            LocationList = modelMasterData?.PickUpDropOffLocations ?? new List<AttributeValues>(),
                            BoardBasisList = BoardBasisList ?? new List<AttributeValues>(),
                            BreakfastTypeList = DefMealTypeList ?? new List<AttributeValues>(),
                            TicketLocationList = modelMasterData?.TicketLocationList ?? new List<AttributeValues>(),
                            TypeofRoomList = modelMasterData?.QuoteRoomTypeList ?? new List<AttributeValues>(),
                            FloorList = modelMasterData?.FloorsList ?? new List<AttributeValues>(),
                            MealStyleList = modelMasterData?.MealStyleList ?? new List<AttributeValues>(),
                            MealCoursesList = modelMasterData?.MealCoursesList ?? new List<AttributeValues>(),
                            FileHandlerList = FileHandlerList,
                            #endregion
                        }
                    };
                    model.ProdTypePositions.RoomsAndRates.RangeListBudgSupp = res.ProdCategoryRangeDetails?.Where(a => model.ProdTypePositions.RoomsAndRates.BookingRoomsAndPrices.Any(b => b.ProductRangeID == a.VoyagerProductRange_Id)).ToList() ?? new List<ProductRangeDetails>();
                    model.ProdTypePositions.RoomsAndRates.RangeList.ForEach(a => a.ProductRangeCode = "(" + a.ProductCategoryName + ") " + a.ProductRangeName);
                    model.ProdTypePositions.RoomsAndRates.RangeListSupp.ForEach(a => a.ProductRangeCode = "(" + a.ProductCategoryName + ") " + a.ProductRangeName);
                    model.ProdTypePositions.SuppFOC.lstPosFOC.RemoveAll(x => x.GetQuantity == null && x.BuyQuantity == null);
                    model.ProdTypePositions.SuppFOC.lstPosFOC.AddRange(new List<OpsPositionFOC>() { new OpsPositionFOC() { PositionFOC_Id = Guid.NewGuid().ToString() } });
                }

            }
            catch (Exception e)
            {
            }
            return model;
        }

        private List<ScheduleDetails> GenerateScheduleTimeSlabs(OpsProductTypeGetRes response)
        {
            int ScheduleIndex = 0;
            TimeSpan SlabStartTime, SlabEndTime;
            ScheduleDetails item, Schedule, slabOG = new ScheduleDetails();
            List<ScheduleDetails> MultiSchedule = new List<ScheduleDetails>();
            List<ScheduleDetails> TimeSlabsList = new List<ScheduleDetails>();
            List<ScheduleDetails> TimeSlabsListCopy = new List<ScheduleDetails>();
            //List<ScheduleDetails> ScheduleDetailsList = response.ScheduleDetailsList;
            List<string> TimeSlabs = new List<string> { "04:00", "04:30", "05:00", "05:30", "06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30",
                    "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00", "17:30",
                    "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "23:30", "00:00", "00:30", "01:00", "01:30",
                    "02:00", "02:30", "03:00", "03:30", };

            try
            {
                var ItineraryDetails = response.SpecificDayItineraryDetails.Where(a => string.IsNullOrEmpty(a.Position_Id)).ToList();
                response.ScheduleDetailsList.AddRange(ItineraryDetails.Select(a => new ScheduleDetails
                {
                    PositionId = a.Position_Id,
                    ProductName = a.Description,
                    ProductType = a.ProductType,
                    StartTime = a.STARTTIME,
                    EndTime = a.ENDTIME,
                    StartDate = a.STARTDATE.Value
                }).ToList());

                TimeSlabsList = TimeSlabs.Select(slab => new ScheduleDetails { StartTime = slab }).ToList();
                TimeSlabsListCopy = TimeSlabsList.Select(a => new ScheduleDetails
                {
                    PositionId = a.PositionId,
                    ProductName = a.ProductName,
                    ProductType = a.ProductType,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    StartDate = a.StartDate
                }).ToList();

                foreach (var slab in TimeSlabsListCopy)
                {
                    SlabStartTime = TimeSpan.Parse(slab.StartTime);
                    SlabEndTime = TimeSpan.Parse(slab.StartTime).Add(TimeSpan.Parse("00:30"));

                    MultiSchedule = response.ScheduleDetailsList.Where(a => TimeSpan.Parse(a.StartTime) >= SlabStartTime && TimeSpan.Parse(a.StartTime) < SlabEndTime).ToList();
                    if (MultiSchedule != null && MultiSchedule.Count > 0)
                    {
                        for (int i = 0; i < MultiSchedule.Count; i++)
                        {
                            item = MultiSchedule[i];
                            Schedule = new ScheduleDetails
                            {
                                PositionId = item.PositionId,
                                ProductName = item.ProductName,
                                ProductType = item.ProductType,
                                StartTime = item.StartTime,
                                EndTime = item.EndTime,
                                StartDate = item.StartDate
                            };

                            if (i == 0)
                            {
                                slabOG = TimeSlabsList.Where(a => a.StartTime == slab.StartTime).FirstOrDefault();
                                slabOG.PositionId = Schedule.PositionId;
                                slabOG.ProductName = Schedule.ProductName;
                                slabOG.ProductType = Schedule.ProductType;
                            }
                            else
                            {
                                ScheduleIndex = TimeSlabsList.IndexOf(TimeSlabsList.Where(a => a.PositionId == slabOG.PositionId).FirstOrDefault()) + 1;
                                TimeSlabsList.Insert(ScheduleIndex, Schedule);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return TimeSlabsList;
        }

        [Route("GetPickUpDropOffLocations")]
        public JsonResult GetPickUpDropOffLocations(string term)
        {
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "PickUpDropOffLocations";
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "PickUpDropOffLocations")
                {
                    List<AttributeValues> pickUpDropOffLocationsList = new List<AttributeValues>();
                    pickUpDropOffLocationsList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.Where(p => p.Value.ToLower().Contains(term.ToLower())).ToList();
                    return Json(pickUpDropOffLocationsList);
                }
                else
                {
                    return Json("");
                }
            }
            else
            {
                return Json("");
            }
        }

        /// <summary>
        /// Get PersonType By ProductRange, Category and ProductId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("GetPersonTypeByProductRange")]
        public JsonResult GetPersonTypeByProductRange(OpsProdRangePersTypeGetReq request)
        {
            try
            {
                var response = operationsProviders.GetPersonTypeByProductRange(request, token).Result;
                if (response != null)
                {
                    return Json(response);
                }
                else
                {
                    return Json("No response received.");
                }
            }
            catch (Exception ex)
            {
                return Json("Unable to process this request.");
            }
        }
        #endregion

        #region FOC
        [Route("SetOpsPosFOCDetails")]
        public IActionResult SetOpsPosFOCDetails(OpsPosFOCViewModel model)
        {
            try
            {
                #region Set Ops Position FOC Details  
                model.lstPosFOC.RemoveAll(x => (x.BuyQuantity == null && x.GetQuantity == null) || x.IsDeleted == true);
                OpsFOCSetReq request = new OpsFOCSetReq()
                {
                    BookingNo = model.BookingNo,
                    Position_Id = model.Position_Id,
                    PositionFoc = model.lstPosFOC.Select(x => new PositionFOC
                    {
                        Booking_Id = model.Booking_Id,
                        Position_Id = model.Position_Id,
                        PositionFOC_Id = x.PositionFOC_Id ?? Guid.NewGuid().ToString(),
                        BuyQuantity = x.BuyQuantity,
                        BuyBookingRooms_ID = x.BuyBookingRoomsId,
                        BuyRoomShortCode = x.BuyRoomShortCode,
                        GetQuantity = x.GetQuantity,
                        GetBookingRooms_ID = x.GetBookingRoomsId,
                        GetRoomShortCode = x.GetRoomShortCode,
                        AuditTrail = new AuditTrail()
                        {
                            CREA_DT = string.IsNullOrEmpty(x.PositionFOC_Id) ? DateTime.Now : x.CREA_DT,
                            CREA_US = string.IsNullOrEmpty(x.PositionFOC_Id) ? ckUserEmailId : x.CREA_US,
                            MODI_DT = (!string.IsNullOrEmpty(x.PositionFOC_Id) && x.IsDeleted == true) ? DateTime.Now : x.MODI_DT,
                            MODI_US = (!string.IsNullOrEmpty(x.PositionFOC_Id) && x.IsDeleted == true) ? ckUserEmailId : x.MODI_US
                        }
                    }).ToList()
                };
                var response = operationsProviders.SetBookingPositionFOC(request, token).Result;
                #endregion

                //return PartialView("~/Views/Operations/_PositionFOC.cshtml", model.lstPosFOC);
                return Json(new { status = response.ResponseStatus, responseText = response.ResponseStatus.ErrorMessage });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("GetOpsPositionFOC")]
        public IActionResult GetOpsPositionFOC(OpsProductTypeGetReq request)
        {
            OpsProdTypeVM model = new OpsProdTypeVM();
            try
            {
                #region Get Ops Service Itinerary Details  
                var response = operationsProviders.GetOpsProductTypeDetails(request, token).Result;
                var pos = response.Position;
                model.ChargeBasis = response?.OpsProductTypeCommonFields?.ChargeBasis;
                string FOCPersonTypeByChargeBasis = model.ChargeBasis == "PU" || model.ChargeBasis == "PUPD" ? "UNIT" : "ADULT";

                #region Get Rooms dropodown in FOC section
                ProdCategoryRangeGetReq req = new ProdCategoryRangeGetReq() { ProductId = new List<string>() { pos.Product_Id } };
                ProdCategoryRangeGetRes res = masterProviders.GetProductCategoryRangeByProductID(req, token).Result;
                #endregion

                var SuppFOC = new OpsPosFOCViewModel()
                {
                    Booking_Id = response.OpsProductTypeCommonFields.BookingId,
                    BookingNo = response.OpsProductTypeCommonFields.BookingNumber,
                    Position_Id = pos.Position_Id,
                    lstPosFOC = (pos != null && pos.PositionFOC != null && pos.PositionFOC.Count > 0) ? pos.PositionFOC.Select(x => new OpsPositionFOC()
                    {
                        PositionFOC_Id = x.PositionFOC_Id,
                        BuyQuantity = x.BuyQuantity,
                        BuyBookingRoomsId = x.BuyBookingRooms_ID,
                        BuyRoomShortCode = x.BuyRoomShortCode,
                        GetQuantity = x.GetQuantity,
                        GetBookingRoomsId = x.GetBookingRooms_ID,
                        GetRoomShortCode = x.GetRoomShortCode,
                        CREA_DT = x.AuditTrail.CREA_DT,
                        CREA_US = x.AuditTrail.CREA_US,
                        MODI_DT = x.AuditTrail.MODI_DT,
                        MODI_US = x.AuditTrail.MODI_US
                    }).ToList() : new List<OpsPositionFOC>() { new OpsPositionFOC() { PositionFOC_Id = Guid.NewGuid().ToString() } },

                    lstBookingRooms = res.ProdCategoryRangeDetails?.Where(a => a.AdditionalYN == false && a.PersonType?.ToUpper() == FOCPersonTypeByChargeBasis).OrderBy(x => x.ProductRangeName).ToList() ?? new List<ProductRangeDetails>(),
                };

                SuppFOC.lstPosFOC.RemoveAll(x => x.GetQuantity == null && x.BuyQuantity == null);
                SuppFOC.lstPosFOC.AddRange(new List<OpsPositionFOC>() { new OpsPositionFOC() { PositionFOC_Id = Guid.NewGuid().ToString() } });

                #endregion
                return View("_OpsPositionFOC", SuppFOC);
            }
            catch (Exception ex)
            {
                return View("ViewServiceStatusProdType", model);
            }
        }
        #endregion

        #region Communication Trail at Position Level
        [HttpPost]
        [Route("GetCommunicationTrail")]
        public IActionResult GetCommunicationTrail(DocumentStoreGetReq model)
        {
            CommunicationTrailViewModel objCommunicationTrailViewModel = communicationTrailMapping.GetCommunicationTrail(model, token);
            objCommunicationTrailViewModel.ModuleType = model.ModuleType;
            objCommunicationTrailViewModel.DocumentStoreInfoGetRes.DocumentStoreInfo.Body = "";
            return PartialView("~/Views/Shared/CommunicationTrail/_CommunicationTrail.cshtml", objCommunicationTrailViewModel);
        }

        [HttpPost]
        [Route("GetCommunicationTrailById")]
        public IActionResult GetCommunicationTrailById(DocumentStoreGetReq model)
        {
            DocumentStoreInfo objDocumentStoreInfo = communicationTrailMapping.GetCommunicationTrailById(model, token);
            if (objDocumentStoreInfo?.ResponseStatus?.Status.ToLower() == "success")
            {
                return Json(new { model = objDocumentStoreInfo, maildt = objDocumentStoreInfo.SendDate.ToString("MMM dd yyy HH:mm") });
            }
            else
            {
                return Json(new { model = objDocumentStoreInfo, maildt = "" });
            }
        }

        [HttpGet]
        [Route("Download")]
        public ActionResult Download(string file)
        {
            //string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\documents", file);
            string filepath = Path.Combine(_configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath"), file);

            if (System.IO.File.Exists(filepath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = file;
                return response;
            }
            else
            {
                throw new FileNotFoundException(file + " File Not Found.");
            }
        }
        #endregion

        [Route("CancelPosition")]
        public IActionResult CancelPosition(string BookingNumber, string PositionId)
        {
            try
            {
                OpsCancelBooking model = new OpsCancelBooking();
                model.BookingNumber = BookingNumber;

                #region Cancellation list binding
                // Get Master Data from Operations Masters, Cancellation Reasons 
                MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
                objMasterTypeRequest.Property = "OperationsMasters";
                objMasterTypeRequest.Name = "CancellationReason";
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
                List<Attributes> list = new List<Attributes>();
                if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
                {
                    if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "CancellationReason")
                    {
                        var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
                        foreach (var a in result)
                        {
                            Attributes aa = new Attributes();
                            aa.Attribute_Id = a.AttributeValue_Id;
                            aa.AttributeName = a.Value;
                            list.Add(aa);
                        }
                    }
                    model.CancellationList = list.OrderBy(x => x.AttributeName).ToList();
                }
                #endregion

                if (!string.IsNullOrEmpty(PositionId))
                {
                    #region Get Ops Service Itinerary Details  
                    OpsBookingItineraryGetReq request = new OpsBookingItineraryGetReq() { BookingNumber = BookingNumber, PositionId = PositionId };
                    OpsBookingItineraryGetRes bookingRes = operationsProviders.GetPositionTypeByPositionId(request, token).Result;
                    if (bookingRes != null)
                    {
                        var positionType = bookingRes.OpsItineraryDetails.OpsPositions[0].PositionType;
                        model.Placeholder = bookingRes.OpsItineraryDetails.OpsPositions[0].Placeholder;
                        model.PositionStatusFull = bookingRes.OpsItineraryDetails.OpsPositions[0].PositionStatus;
                        model.SystemCompany_Id = bookingRes.OpsItineraryDetails.OpsPositions[0].systemCompanyId;
                        if (positionType?.ToLower() == "core")
                            model.PosType = "core";
                        else
                            model.PosType = "noncore";
                    }
                    #endregion
                }
                model.PositionId = PositionId;

                return PartialView("_CancelPosition", model);
            }
            catch (Exception ex)
            {
                return PartialView("_CancelPosition");
            }
        }

        [Route("RoomChange")]
        public IActionResult RoomChange(string BookingNumber)
        {
            BookingMaterializationVm bookingmaterial = new BookingMaterializationVm();
            List<SupplierDetails> supdetails = new List<SupplierDetails>();
            BookingRoomsGetRequest request = new BookingRoomsGetRequest();
            BookingPaxDetailsGetRequest requestforPax = new BookingPaxDetailsGetRequest();
            List<RoomMaterializationDetails> ListroomMat = new List<RoomMaterializationDetails>();
            PositionsFromBookingGetReq requestForBookiing = new PositionsFromBookingGetReq();
            List<PersonMaterializationDetails> ListPersonMat = new List<PersonMaterializationDetails>();
            QrfPackagePriceGetReq RequestForInCostRooms = new QrfPackagePriceGetReq();
            List<DeterminePassCount> TotalPassForPax = new List<DeterminePassCount>();
            List<string> RoomNames = new List<string>();
            request.BookingNumber = BookingNumber;
            requestforPax.BookingNumber = BookingNumber;
            bookingmaterial.bookingNumber = BookingNumber;
            requestForBookiing.BookingNumber = BookingNumber;
            var BookingPositions = operationsProviders.GetPositionsFromBooking(requestForBookiing, token).Result;
            if (BookingPositions != null && BookingPositions.PositionDetails != null && BookingPositions.PositionDetails.Any())
            {
                foreach (var item in BookingPositions.PositionDetails)
                {
                    SupplierDetails sdetails = new SupplierDetails();
                    sdetails.ProductType = item.ProductType;
                    sdetails.Position_ID = item.Position_Id;
                    sdetails.ProductName = item.Product_Name;
                    sdetails.StartDate = item.STARTDATE.HasValue ? item.STARTDATE.Value.ToString("dd MMM") : string.Empty;
                    sdetails.SupplierName = item.SupplierInfo?.Name;
                    supdetails.Add(sdetails);

                }
                bookingmaterial.SupplierDetails = supdetails;

            }
            try
            {
                var BookingDetails = operationsProviders.GetOpsBookingRoomsDetails(request, token).Result;
                string Qrfid = BookingDetails.QrfId;
                RequestForInCostRooms.QrfId = Qrfid;
                if (BookingDetails != null && BookingDetails.BookingRoomsDetails != null && BookingDetails.BookingRoomsDetails.Any())
                {

                    foreach (var item in BookingDetails.BookingRoomsDetails)
                    {
                        RoomMaterializationDetails roomMat = new RoomMaterializationDetails();
                        roomMat.RoomType = item.SUBPROD;
                        roomMat.For = "Pax";
                        roomMat.Age = item.Age;
                        roomMat.Current = item.ROOMNO.HasValue ? item.ROOMNO.Value : 0;
                        roomMat.NewLevel = item.ROOMNO.HasValue ? item.ROOMNO.Value : 0;
                        roomMat.BookingRooms_Id = item.BookingRooms_ID;
                        ListroomMat.Add(roomMat);
                        RoomNames.Add(item.SUBPROD);
                        TotalPassForPax.Add(new DeterminePassCount { RoomName = item.SUBPROD, RoomCount = roomMat.Current.ToString() });

                    }

                    bookingmaterial.roomMaterializationDetails = ListroomMat;
                }
                var BookingPaxDetails = operationsProviders.GetOpsBookingPaxDetails(requestforPax, token).Result;
                if (BookingPaxDetails != null && BookingPaxDetails.bookingPaxDetails != null && BookingPaxDetails.bookingPaxDetails.Any())
                {
                    foreach (var item in BookingPaxDetails.bookingPaxDetails)
                    {
                        PersonMaterializationDetails personMat = new PersonMaterializationDetails();
                        personMat.PassengerAge = item.AGE.HasValue ? item.AGE.Value.ToString() : string.Empty;
                        personMat.PassengerType = item.PERSTYPE?.ToUpper();
                        personMat.PassengerQty = item.PERSONS;
                        personMat.BookingPax_ID = item.BookingPax_Id;
                        ListPersonMat.Add(personMat);
                    }
                    bookingmaterial.personMaterializationDetails = ListPersonMat;
                }
                if (bookingmaterial.personMaterializationDetails != null && bookingmaterial.personMaterializationDetails.Any())
                {
                    var ForAdult = bookingmaterial.personMaterializationDetails.Where(x => x.PassengerType?.ToUpper() == "ADULT").FirstOrDefault();
                    if (ForAdult != null)
                    {
                        ForAdult.PassengerQty = DeterminePassengerQuantity(TotalPassForPax);
                    }
                }
                var InCostRoom = operationsProviders.GetQRFPackagePriceForRoomsDetails(RequestForInCostRooms, token).Result;
                if (InCostRoom != null && InCostRoom.RoomType.Any())
                {
                    var ListofExplicitRooms = InCostRoom.RoomType.Except(RoomNames);

                    bookingmaterial.RoomsInCostExceptInMaterialization = ListofExplicitRooms.ToList();
                }


                return PartialView("_RoomChange", bookingmaterial);
            }
            catch (Exception ex)
            {
                return PartialView("_RoomChange", bookingmaterial);
            }
        }
        [HttpPost]
        [Route("SaveRoomChangeDetails")]
        public IActionResult SaveRoomChangeDetails(BookingMaterializationVm vm)
        {


            return View();
        }
        private int DeterminePassengerQuantity(List<DeterminePassCount> Roomdetails)
        {
            int PassengerCount = 0;
            foreach (var Room in Roomdetails)
            {
                if (!string.IsNullOrEmpty(Room.RoomCount))
                {
                    if (Room.RoomName?.Trim()?.ToUpper() == "SINGLE")
                    {
                        PassengerCount += Convert.ToInt32(Room.RoomCount);
                    }
                    else if (Room?.RoomName?.Trim().ToUpper() == "TWIN" || Room?.RoomName?.Trim().ToUpper() == "DOUBLE")
                    {
                        PassengerCount += 2 * Convert.ToInt32(Room.RoomCount);
                    }
                    else if (Room?.RoomName?.Trim().ToUpper() == "TRIPLE")
                    {
                        PassengerCount += 3 * Convert.ToInt32(Room.RoomCount);
                    }
                    else if (Room?.RoomName?.Trim().ToUpper() == "QUAD")
                    {
                        PassengerCount += 4 * Convert.ToInt32(Room.RoomCount);
                    }
                    else if (Room?.RoomName?.Trim().ToUpper() == "TSU")
                    {
                        PassengerCount += Convert.ToInt32(Room.RoomCount);
                    }
                    else { }
                }
            }
            return PassengerCount;

        }
        [Route("ContactSupplier")]
        public IActionResult ContactSupplier(string BookingNumber, string PositionId)
        {
            OPSContactSupplier model = new OPSContactSupplier();
            try
            {
                model.BookingNumber = BookingNumber;
                model.PositionId = PositionId;
                return PartialView("_ContactSupplier", model);
            }
            catch (Exception ex)
            {
                return PartialView("_ContactSupplier", model);
            }
        }

        [Route("ChangeSupplier")]
        public IActionResult ChangeSupplier(string BookingNumber, string PositionId, string PositionStatus, string SystemCompanyId)
        {
            OpsChangeSupplierViewModel model = new OpsChangeSupplierViewModel();
            try
            {
                PositionsFromBookingGetReq positionRequest = new PositionsFromBookingGetReq();
                PositionsFromBookingGetRes positionResponse = new PositionsFromBookingGetRes();
                positionRequest.BookingNumber = BookingNumber;
                positionRequest.PositionId = PositionId;
                positionResponse = operationsProviders.GetPositionsFromBooking(positionRequest, token).Result;
                if (positionResponse?.Response?.Status?.ToLower() == "success")
                {
                    if (positionResponse.PositionDetails?.Count > 0)
                    {
                        model.BookingNumber = BookingNumber;
                        model.Position_Id = PositionId;
                        model.Supplier_Name = positionResponse.PositionDetails[0].SupplierInfo?.Name;
                        model.PositionStatus = PositionStatus;
                        model.BookingRoomsAndPrices = positionResponse.PositionDetails[0].BookingRoomsAndPrices.Where(a => a.Status?.ToUpper() != "X" && a.PersonType?.ToUpper() == "ADULT").ToList();


                        #region Supplier List
                        ProductSupplierGetRes response = new ProductSupplierGetRes();
                        ProductSupplierGetReq request = new ProductSupplierGetReq();
                        request.ProductId = positionResponse.PositionDetails[0].Product_Id;
                        request.IsContractRateRequired = true;
                        response = masterProviders.GetProductSupplierList(request, token).Result;

                        if (response?.ResponseStatus?.Status?.ToLower() == "success")
                        {
                            if (response.SupllierList?.Count > 0)
                            {
                                foreach (var resSupp in response.SupllierList.Where(a => a.SupplierId != positionResponse.PositionDetails[0].SupplierInfo?.Id && a.SupplierId != SystemCompanyId))
                                {
                                    var Supplier = new OpsSupplierDetails();
                                    Supplier.SupplierId = resSupp.SupplierId;
                                    Supplier.SupplierName = resSupp.SupplierName;
                                    Supplier.CityName = resSupp.CityName;

                                    foreach (var bookingRoom in model.BookingRoomsAndPrices)
                                    {
                                        var productRange = new OpsProductRangeContract();
                                        productRange.ProductCategoryId = bookingRoom.Category_Id;
                                        productRange.ProductCategory = bookingRoom.CategoryName;
                                        productRange.ProductRangeId = bookingRoom.ProductRange_Id;
                                        productRange.ProductRange = bookingRoom.RoomName;

                                        var lstPCInfo = response.ProductContracts.Where(a => a.ProductRangeId == bookingRoom.ProductRange_Id
                                                          && (a.FromDate <= Convert.ToDateTime(positionResponse.PositionDetails[0].ENDDATE) && Convert.ToDateTime(positionResponse.PositionDetails[0].ENDDATE) <= a.ToDate)).ToList();

                                        if (lstPCInfo != null && lstPCInfo.Count > 0)
                                        {
                                            foreach (var con in lstPCInfo)
                                            {
                                                if (con.DayComboPattern == null || con.DayComboPattern == "")
                                                    con.DayComboPattern = "1111111";
                                                char[] dayPattern = con.DayComboPattern.ToCharArray();
                                                int dayNo = (int)Convert.ToDateTime(positionResponse.PositionDetails[0].ENDDATE).DayOfWeek;

                                                if (dayNo == 0)
                                                    dayNo = 7;

                                                if (dayPattern[dayNo - 1] == '1')
                                                {
                                                    productRange.ContractId = con.ContractId;
                                                    productRange.ContractPrice = Convert.ToDouble(con.Price);
                                                    productRange.ContractCurrency = con.Currency;
                                                    break;
                                                }
                                            }
                                        }
                                        Supplier.ProductRangeContract.Add(productRange);
                                    }

                                    model.SupplierList.Add(Supplier);
                                }
                            }
                        }
                        #endregion
                    }
                }

                return PartialView("_ChangeSupplier", model);
            }
            catch (Exception ex)
            {
                return PartialView("_ChangeSupplier", model);
            }
        }

        [Route("ChangeProduct")]
        public IActionResult ChangeProduct(string BookingNumber, string PositionId)
        {
            OpsChangeProductViewModel model = new OpsChangeProductViewModel();
            try
            {
                PositionsFromBookingGetReq positionRequest = new PositionsFromBookingGetReq();
                PositionsFromBookingGetRes positionResponse = new PositionsFromBookingGetRes();
                positionRequest.BookingNumber = BookingNumber;
                positionRequest.PositionId = PositionId;
                positionResponse = operationsProviders.GetPositionsFromBooking(positionRequest, token).Result;
                if (positionResponse?.Response?.Status?.ToLower() == "success")
                {
                    if (positionResponse.PositionDetails?.Count > 0)
                    {
                        model.BookingNumber = BookingNumber;
                        model.Position_Id = PositionId;
                        model.Product_Name = positionResponse.PositionDetails[0].Product_Name;
                        model.CityName = positionResponse.PositionDetails[0].City + ", " + positionResponse.PositionDetails[0].Country;
                        model.PositionEndDate = positionResponse.PositionDetails[0].ENDDATE;
                        model.BookingRoomsAndPrices = positionResponse.PositionDetails[0].BookingRoomsAndPrices.Where(a => a.PersonType?.ToUpper() == "ADULT").ToList();
                        model.Attributes = positionResponse.PositionDetails[0].Attributes;
                    }
                }
                return PartialView("_ChangeProduct", model);
            }
            catch (Exception ex)
            {
                return PartialView("_ChangeProduct", model);
            }
        }
        #endregion

        #region CommunicationsTrail at Booking Level
        [Route("ViewCommunicationsTrailSummary")]
        public IActionResult ViewCommunicationsTrailSummary(string BookingNumber)
        {
            OpsCommunicationTrailSummaryViewModel model = new OpsCommunicationTrailSummaryViewModel();

            #region Get Booking Details 
            OpsHeaderGetReq opsHeaderGetReq = new OpsHeaderGetReq() { BookingNumber = BookingNumber };
            OperationHeaderInfo operationHeaderInfo = operationsProviders.GetOperationHeaderDetails(opsHeaderGetReq, token).Result;
            #endregion

            if (operationHeaderInfo != null)
            {
                #region Get Costing Officer Tour Info Header By QRFId
                model.COHeader.TourName = operationHeaderInfo.BookingNumber + " : " + operationHeaderInfo.TourName;
                model.COHeader.NoOfNights = operationHeaderInfo.NoOfNights;
                model.COHeader.NoOfDays = operationHeaderInfo.NoOfDays;
                model.COHeader.Destination = operationHeaderInfo.Destination;
                model.COHeader.TravelDate = operationHeaderInfo.StartDate;
                model.COHeader.EndDate = operationHeaderInfo.EndDate;
                model.COHeader.SalesPerson = operationHeaderInfo.SalesOfficerEmail;
                model.COHeader.CostingOfficer = operationHeaderInfo.CostingOfficerEmail;
                model.COHeader.FileHandler = operationHeaderInfo.FileHandlerEmail;
                model.COHeader.ProductAccountant = operationHeaderInfo.ProductAccountantEmail;
                model.COHeader.BookingNumber = operationHeaderInfo.BookingNumber;
                model.COHeader.ModuleName = "ops";
                #endregion

                CommunicationTrailViewModel objCommunicationTrailViewModel = communicationTrailMapping.GetCommunicationTrail(new DocumentStoreGetReq() { BookingNumber = BookingNumber }, token);
                objCommunicationTrailViewModel.ModuleType = "ops";
                objCommunicationTrailViewModel.DocumentStoreInfoGetRes.DocumentStoreInfo.Body = "";
                model.CommunicationTrailViewModel = objCommunicationTrailViewModel;
            }
            return View(model);
        }
        #endregion

        #region SetBookingByWorkflow
        [HttpPost]
        [Route("ManageDataforTemplatebookingRooms")]
        public IActionResult ManageDataforTemplatebookingRooms(Dictionary<string, string> model)
        {
            string status = "";
            BookingRoomsGetRequest request1 = new BookingRoomsGetRequest();
            List<FilterRemovedData> RoomsinDb1;
            List<FilterRemovedData> RoomsinMaterialization1;
            List<string> RoomsinDb;
            List<string> RoomsinMaterialization;

            List<string> msg = new List<string>();
            List<string> docname = new List<string>();
            string BookingNumber = model["BookingNumber"];
            request1.BookingNumber = BookingNumber;
            string PositionId = model.ContainsKey("PositionId") ? model["PositionId"] : "";
            string Action = model["Action"];
            string Module = model["Module"];
            string ModuleParent = model["ModuleParent"];
           

            //model.Remove("BookingNumber");
            //model.Remove("PositionId");
            //model.Remove("Action");
            //model.Remove("Module");
            //model.Remove("ModuleParent");

            var request = new OpsBookingSetReq
            {
                BookingNumber = BookingNumber,
                PositionIds = !string.IsNullOrEmpty(PositionId) ? new List<string>() { PositionId } : null,
                UserEmailId = ckUserEmailId,
                UserId = ckLoginUser_Id,
                UserName = ckUserName,
                OpsKeyValue = model.Select(a => new OpsKeyValue { Key = a.Key, Value = a.Value }).ToList(),
                Action = Action,
                Module = Module,
                ModuleParent = ModuleParent,

            };
            List<OpsBookingRoomsDetails> NewDeleted = new List<OpsBookingRoomsDetails>();
            OPSWorkflowResponseStatus response = new OPSWorkflowResponseStatus();
            List<TemplateBookingRoomsGrid> bookingRoomsData = new List<TemplateBookingRoomsGrid>();
            List<OpsBookingRoomsDetails> BookingDetails = new List<OpsBookingRoomsDetails>();
            var keyValueForBookingRooms = new OpsKeyValue();
            var KeyValueList = request.OpsKeyValue;
            keyValueForBookingRooms = KeyValueList.Where(x => x.Key == "BookingRooms").FirstOrDefault();
            BookingDetails = JsonConvert.DeserializeObject<List<OpsBookingRoomsDetails>>(keyValueForBookingRooms.Value.ToString());

            var BookingRoomListData = operationsProviders.GetOpsBookingRoomsDetails(request1, token).Result;
            if(BookingRoomListData != null && BookingRoomListData.BookingRoomsDetails.Any())
            {
                 bookingRoomsData = BookingRoomListData.BookingRoomsDetails;
                var BookingRoomListDataForNotChild = bookingRoomsData?.Where(x => x.SUBPROD != null && x.SUBPROD != "CHILDWITHBED" && x.SUBPROD != "CHILDWITHOUTBED" && x.SUBPROD != "INFANT").ToList();
                if (BookingRoomListDataForNotChild != null && BookingDetails != null && BookingDetails.Any())
                {
                    RoomsinDb = BookingRoomListDataForNotChild.Select(x => x.SUBPROD).ToList();
                    RoomsinMaterialization = BookingDetails.Where(x => x.RoomType != "CHILDWITHBED" && x.RoomType != "CHILDWITHOUTBED" && x.RoomType != "INFANT")?.Select(x => x.RoomType).ToList();
                    var RoomInDbNotinMaterialization = RoomsinDb.Except(RoomsinMaterialization);
                    if (RoomInDbNotinMaterialization != null && RoomInDbNotinMaterialization.Any())
                    {
                        foreach (var RoomToBeRemoved in RoomInDbNotinMaterialization)
                        {
                            OpsBookingRoomsDetails ops = new OpsBookingRoomsDetails();
                            ops.RoomType = RoomToBeRemoved?.ToUpper();
                            ops.Status = "X";
                            NewDeleted.Add(ops);

                        }
                    }
                }
                var BookingRoomsDataforChild = bookingRoomsData.Where(x => x.SUBPROD?.Trim()?.ToUpper() == "CHILDWITHBED" || x.SUBPROD?.Trim()?.ToUpper() == "CHILDWITHOUTBED" || x.SUBPROD?.Trim()?.ToUpper() == "INFANT").ToList();
                var RoomsDataForChild = BookingDetails.Where(x => x.RoomType?.Trim()?.ToUpper() == "CHILDWITHBED" || x.RoomType?.Trim()?.ToUpper() == "CHILDWITHOUTBED" || x.RoomType?.Trim()?.ToUpper() == "INFANT").ToList();
                if (BookingRoomsDataforChild != null && BookingRoomsDataforChild.Any() && RoomsDataForChild != null )
                {
                    RoomsinDb1 = BookingRoomsDataforChild.Select(x => new FilterRemovedData { Age = x.Age.HasValue ? x.Age.Value.ToString() : null, RoomName = x.SUBPROD }).ToList();
                    RoomsinMaterialization1 = RoomsDataForChild.Select(x => new FilterRemovedData { RoomName = x.RoomType, Age = x.Age.HasValue ? x.Age.Value.ToString() : null }).ToList();
                    //var RoomInDbNotinMaterializationForChild = RoomsinDb1.Except(RoomsinMaterialization1);
                    foreach (var room in RoomsinMaterialization1)
                    {
                        RoomsinDb1.RemoveAll(a => a.RoomName == room.RoomName && a.Age == room.Age);

                    }
                    var RoomInDbNotinMaterializationForChild = RoomsinDb1;
                    if (RoomInDbNotinMaterializationForChild != null && RoomInDbNotinMaterializationForChild.Any())
                    {
                        foreach (FilterRemovedData MissingRoomsinMaterialization in RoomInDbNotinMaterializationForChild)
                        {
                            OpsBookingRoomsDetails ops = new OpsBookingRoomsDetails();
                            ops.RoomType = MissingRoomsinMaterialization?.RoomName?.ToUpper();
                            ops.Status = "X";
                            if (!string.IsNullOrEmpty(MissingRoomsinMaterialization.Age))
                            {
                                ops.Age = Convert.ToInt32(MissingRoomsinMaterialization.Age);
                            }
                            NewDeleted.Add(ops);
                        }
                    }
                }

                BookingDetails.AddRange(NewDeleted);
                var Serializeddata = JsonConvert.SerializeObject(BookingDetails);
                model.Remove("BookingRooms");
                model.Add("BookingRooms", Serializeddata);
                
            }

          var  response2 = this.SetBookingByWorkflow(model);

            return response2;
        }
        [HttpPost]
        [Route("SetBookingByWorkflow")]
        public IActionResult SetBookingByWorkflow(Dictionary<string, string> model)
        {
            string status = "";
            List<string> msg = new List<string>();
            List<string> docname = new List<string>();
            
            try
            {
                //var roomsandrates = JsonConvert.DeserializeObject<List<OpsPositionRoomPrice>>(model["TableRoomsAndRates"]);
                string BookingNumber = model["BookingNumber"];
                string PositionId = model.ContainsKey("PositionId") ? model["PositionId"] : "";
                string Action = model["Action"];
                string Module = model["Module"];
                string ModuleParent = model["ModuleParent"];
                model.Remove("BookingNumber");
                model.Remove("PositionId");
                model.Remove("Action");
                model.Remove("Module");
                model.Remove("ModuleParent");
                //model.Remove("Doctype");

                var request = new OpsBookingSetReq
                {
                    BookingNumber = BookingNumber,
                    PositionIds = !string.IsNullOrEmpty(PositionId) ? new List<string>() { PositionId } : null,
                    UserEmailId = ckUserEmailId,
                    UserId = ckLoginUser_Id,
                    UserName = ckUserName,
                    OpsKeyValue = model.Select(a => new OpsKeyValue { Key = a.Key, Value = a.Value }).ToList(),
                    Action = Action,
                    Module = Module,
                    ModuleParent = ModuleParent,
                  
                };
                
                var response = operationsProviders.SetBookingByWorkflow(request, token).Result;
                if (response?.ResponseStatus != null)
                {
                    status = response.ResponseStatus.Status;
                    msg = response.ResponseStatus.ErrorMessage;
                    docname = response.ResponseStatus.DocumentDetails?.Select(a => a.DocumentName).ToList();
                }
                else
                {
                    status = "Failure";
                    msg.Add("An Error Occurred.");
                }
            }
            catch (Exception ex)
            {
                status = "Failure";
                msg.Add(ex.Message);
            }
            return Json(new
            {
                status = status,
                msg = msg,
                docname = docname
            });
        }
        #endregion

        #region common services
        [Route("GetCityName")]
        public JsonResult GetCityName(string term, string QRFID)
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

        [Route("GetProductList")]
        public IActionResult GetProductList(ProductWithRateSearchReq objProductSearchReq, string BookingNumber, string PositionId, DateTime? newPositionDate = null, string ServiceType = "")
        {
            OpsProductListViewModel model = new OpsProductListViewModel();
            PositionsFromBookingGetRes positionResponse = new PositionsFromBookingGetRes();
            var BookingRoomResponse = new BookingRoomGetResponse();
            DateTime PositionDate = new DateTime(); //DefaultDate is old date

            try
            {
                model.ServiceType = ServiceType;
                #region ProductTemplate
                ProductTemplatesGetRes productTemplatesGetRes = masterProviders.GetProductTemplates(new ProductTemplatesGetReq(), token).Result;
                #endregion

                if (ServiceType == "AddPosition")
                {
                    #region BookingRooms
                    BookingRoomResponse = operationsProviders.GetOpsBookingRoomsDetails(new BookingRoomsGetRequest { BookingNumber = BookingNumber }, token).Result;
                    #endregion
                    if (newPositionDate != null)
                        PositionDate = Convert.ToDateTime(newPositionDate);
                }
                else
                {
                    #region PositionData
                    PositionsFromBookingGetReq positionRequest = new PositionsFromBookingGetReq();
                    positionRequest.BookingNumber = BookingNumber;
                    positionRequest.PositionId = PositionId;
                    positionResponse = operationsProviders.GetPositionsFromBooking(positionRequest, token).Result;
                    if (positionResponse?.Response?.Status?.ToLower() == "success")
                    {
                        if (positionResponse.PositionDetails?.Count > 0)
                        {
                            PositionDate = Convert.ToDateTime(positionResponse.PositionDetails[0].ENDDATE);
                        }
                    }

                    #endregion
                }

                objProductSearchReq.Location = objProductSearchReq.Location == "Select" ? null : objProductSearchReq.Location;
                objProductSearchReq.BudgetCategory = objProductSearchReq.BudgetCategory == "Select" ? null : objProductSearchReq.BudgetCategory;

                MasterProviders objMasterProviders = new MasterProviders(_configuration);
                ProductWithRateSearchRes objProductSearchRes = objMasterProviders.GetProductWithRateBySearchCriteria(objProductSearchReq, token).Result;

                if (objProductSearchRes.ResponseStatus.Status == "Success" && objProductSearchRes.ProductWithRate.Count > 0)
                {
                    ViewData["ProductType"] = objProductSearchReq.ProdType?.ToLower();
                    foreach (var resProduct in objProductSearchRes.ProductWithRate)
                    {
                        var product = new OpsProductDetails();
                        product.ProductId = resProduct.VoyagerProduct_Id;
                        product.ProductName = resProduct.ProductName;
                        product.CityName = resProduct.CityName;
                        product.DefaultSupplierId = resProduct.DefaultSupplierId;
                        product.DefaultSupplier = resProduct.DefaultSupplier;

                        product.ProductRangeContract = new List<OpsProductRangeContract>();

                        //.Where(a => positionResponse.PositionDetails[0].BookingRoomsAndPrices.Any(b => b.CategoryName == a.ProductCategoryName))
                        foreach (var resProductCat in resProduct.ProductCategories)
                        {
                            if (ServiceType == "AddPosition")
                            {
                                if (objProductSearchReq.ProdType.ToLower() == "hotel" || objProductSearchReq.ProdType.ToLower() == "apartments" || objProductSearchReq.ProdType.ToLower() == "overnight ferry")
                                {
                                    foreach (var bookingRoom in BookingRoomResponse?.BookingRoomsDetails)
                                    {
                                        var resProductRange = resProductCat.ProductRanges.Where(a => a.AdditionalYn != true && bookingRoom.SUBPROD == a.ProductTemplateCode && a.PersonType?.ToUpper() == "ADULT").FirstOrDefault();

                                        if (resProductRange == null)
                                        {
                                            var productTemplateList = productTemplatesGetRes?.ProductTemplates?.Where(a => a.ParentSubProd == bookingRoom.SUBPROD).ToList();
                                            if (productTemplateList?.Count > 0)
                                                resProductRange = resProductCat.ProductRanges.Where(a => productTemplateList.Any(b => b.VoyagerProductTemplate_Id == a.ProductTemplate_Id)).FirstOrDefault();
                                        }

                                        if (resProductRange != null)
                                        {
                                            var productRange = new OpsProductRangeContract();
                                            productRange.ProductCategoryId = resProductCat.ProductCategory_Id;
                                            productRange.ProductCategory = resProductCat.ParentCategoryName;
                                            productRange.ProductRangeId = resProductRange.ProductRange_Id;
                                            productRange.ProductRange = resProductRange.ProductTemplateName;

                                            var lstPCInfo = resProduct.ProductContracts?.Where(a => a.ProductRangeId == resProductRange.ProductRange_Id
                                                              && (a.FromDate <= PositionDate && PositionDate <= a.ToDate)).ToList();

                                            if (lstPCInfo != null && lstPCInfo.Count > 0)
                                            {
                                                foreach (var con in lstPCInfo)
                                                {
                                                    if (con.DayComboPattern == null || con.DayComboPattern == "")
                                                        con.DayComboPattern = "1111111";
                                                    char[] dayPattern = con.DayComboPattern.ToCharArray();
                                                    int dayNo = (int)Convert.ToDateTime(PositionDate).DayOfWeek;

                                                    if (dayNo == 0)
                                                        dayNo = 7;

                                                    if (dayPattern[dayNo - 1] == '1')
                                                    {
                                                        productRange.ContractId = con.ContractId;
                                                        productRange.ContractPrice = Convert.ToDouble(con.Price);
                                                        productRange.ContractCurrency = con.Currency;
                                                    }
                                                }
                                            }
                                            product.ProductRangeContract.Add(productRange);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var resProductRange in resProductCat.ProductRanges.Where(a => a.AdditionalYn != true && a.PersonType?.ToUpper() == "ADULT"))
                                    {
                                        var productRange = new OpsProductRangeContract();
                                        productRange.ProductCategoryId = resProductCat.ProductCategory_Id;
                                        productRange.ProductCategory = resProductCat.ParentCategoryName;
                                        productRange.ProductRangeId = resProductRange.ProductRange_Id;
                                        productRange.ProductRange = resProductRange.ProductTemplateName;

                                        var lstPCInfo = resProduct.ProductContracts?.Where(a => a.ProductRangeId == resProductRange.ProductRange_Id
                                                          && (a.FromDate <= PositionDate && PositionDate <= a.ToDate)).ToList();

                                        if (lstPCInfo != null && lstPCInfo.Count > 0)
                                        {
                                            foreach (var con in lstPCInfo)
                                            {
                                                if (con.DayComboPattern == null || con.DayComboPattern == "")
                                                    con.DayComboPattern = "1111111";
                                                char[] dayPattern = con.DayComboPattern.ToCharArray();
                                                int dayNo = (int)Convert.ToDateTime(PositionDate).DayOfWeek;

                                                if (dayNo == 0)
                                                    dayNo = 7;

                                                if (dayPattern[dayNo - 1] == '1')
                                                {
                                                    productRange.ContractId = con.ContractId;
                                                    productRange.ContractPrice = Convert.ToDouble(con.Price);
                                                    productRange.ContractCurrency = con.Currency;
                                                }
                                            }
                                        }
                                        product.ProductRangeContract.Add(productRange);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var bookingRoom in positionResponse.PositionDetails[0].BookingRoomsAndPrices)
                                {
                                    var resProductRange = resProductCat.ProductRanges.Where(a => a.AdditionalYn != true && bookingRoom.RoomShortCode == a.ProductTemplateCode && bookingRoom.PersonType == a.PersonType && a.PersonType?.ToUpper() == "ADULT").FirstOrDefault();

                                    if (resProductRange == null)
                                    {
                                        var productTemplateList = productTemplatesGetRes?.ProductTemplates?.Where(a => a.ParentSubProd == bookingRoom.RoomShortCode).ToList();
                                        if (productTemplateList?.Count > 0)
                                            resProductRange = resProductCat.ProductRanges.Where(a => productTemplateList.Any(b => b.VoyagerProductTemplate_Id == a.ProductTemplate_Id)).FirstOrDefault();
                                    }

                                    if (resProductRange != null)
                                    {
                                        var productRange = new OpsProductRangeContract();
                                        productRange.ProductCategoryId = resProductCat.ProductCategory_Id;
                                        productRange.ProductCategory = resProductCat.ParentCategoryName;
                                        productRange.ProductRangeId = resProductRange.ProductRange_Id;
                                        productRange.ProductRange = resProductRange.ProductTemplateName;

                                        var lstPCInfo = resProduct.ProductContracts?.Where(a => a.ProductRangeId == resProductRange.ProductRange_Id
                                                          && (a.FromDate <= PositionDate && PositionDate <= a.ToDate)).ToList();

                                        if (lstPCInfo != null && lstPCInfo.Count > 0)
                                        {
                                            foreach (var con in lstPCInfo)
                                            {
                                                if (con.DayComboPattern == null || con.DayComboPattern == "")
                                                    con.DayComboPattern = "1111111";
                                                char[] dayPattern = con.DayComboPattern.ToCharArray();
                                                int dayNo = (int)Convert.ToDateTime(PositionDate).DayOfWeek;

                                                if (dayNo == 0)
                                                    dayNo = 7;

                                                if (dayPattern[dayNo - 1] == '1')
                                                {
                                                    productRange.ContractId = con.ContractId;
                                                    productRange.ContractPrice = Convert.ToDouble(con.Price);
                                                    productRange.ContractCurrency = con.Currency;
                                                    break;
                                                }
                                            }
                                        }
                                        product.ProductRangeContract.Add(productRange);
                                        break;
                                    }
                                }

                            }
                        }
                        model.OpsProductList.Add(product);
                    }
                    return PartialView("_ProductList", model);
                }
                else
                    return PartialView("_ProductList", model);

            }
            catch (Exception e)
            {
                return PartialView("_ProductList", model);
            }
        }

        [Route("GetContractRatesByProductID")]
        public IActionResult GetContractRatesByProductID(string ProductId, string SupplierId, DateTime? PositionDate = null)
        {
            try
            {
                ProdContractGetReq request = new ProdContractGetReq();
                ProdContractGetRes response = new ProdContractGetRes();
                request.ProductIDList = new List<string>() { ProductId };
                request.SupplierId = SupplierId;

                response = masterProviders.GetContractRatesByProductID(request, token).Result;

                if (response?.ResponseStatus?.Status?.ToLower() == "success" && response?.ProductContractInfo?.Count > 0 && PositionDate != null)
                {
                    response.ProductContractInfo = response.ProductContractInfo.Where(a => a.FromDate <= PositionDate && PositionDate <= a.ToDate).ToList();

                    if (response.ProductContractInfo?.Count > 0)
                    {
                        List<ProductContractInfo> contractList = new List<ProductContractInfo>();
                        foreach (var con in response.ProductContractInfo)
                        {
                            if (con.DayComboPattern == null || con.DayComboPattern == "")
                                con.DayComboPattern = "1111111";
                            char[] dayPattern = con.DayComboPattern.ToCharArray();
                            int dayNo = (int)Convert.ToDateTime(PositionDate).DayOfWeek;

                            if (dayNo == 0)
                                dayNo = 7;

                            if (dayPattern[dayNo - 1] == '1')
                            {
                                contractList.Add(con);
                            }
                        }
                        response.ProductContractInfo = contractList;
                    }
                }
                return Json(response);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [Route("DownloadFile")]
        [Authorize]
        [HttpGet]
        public ActionResult DownloadFile(string file)
        {
            string filepath = Path.Combine(_configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath"), file);

            if (System.IO.File.Exists(filepath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                var response = new FileContentResult(fileBytes, "application/octet-stream");
                response.FileDownloadName = file;
                return response;
            }
            else
            {
                throw new FileNotFoundException(file + " File Not Found.");
            }
        }
        #endregion
    }
}