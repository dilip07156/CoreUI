using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using Voyager.Controllers;

namespace Voyager.Areas.Hotels.Controllers
{
	[Authorize]
	[Area("Hotels")]
	[Route("Hotels")]
	public class BookingController : VoyagerController
	{
		private readonly IConfiguration _configuration;
		private COCommonLibrary coCommonLibrary;
		private BookingProviders bookingProviders;
		private HotelsProviders hotelsProviders;
		private SalesProviders objSalesProvider;
		private CommunicationTrailMapping communicationTrailMapping;
		private SupplierProviders supplierProviders;

		/// <summary>
		/// Hotels Search Constructor
		/// </summary>
		/// <param name="configuration"></param>
		public BookingController(IConfiguration configuration) : base(configuration)
		{
			_configuration = configuration;
			coCommonLibrary = new COCommonLibrary(_configuration);
			bookingProviders = new BookingProviders(_configuration);
			objSalesProvider = new SalesProviders(_configuration);
			hotelsProviders = new HotelsProviders(_configuration);
			communicationTrailMapping = new CommunicationTrailMapping(configuration);
			supplierProviders = new SupplierProviders(_configuration);
		}

		#region Search Hotel Page
		[Route("SearchHotels")]
		public IActionResult SearchHotels()
		{
			HotelsDeptViewModel model = new HotelsDeptViewModel();

			#region Dropdown Binding
			MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest { Property = "QRF Masters", Name = "BookingSearchDateType" };
			MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
			#endregion

			#region Get Booking Details 
			BookingSearchRes response = new BookingSearchRes();
			response = bookingProviders.GetBookingStatusList(token).Result;
			#endregion

			string[] StatusIgnoreList = new string[] { "N", "J", "I", "B", "C", "-", "^", "L", "S", "X", "T" };
			model.HotelsDeptSearchFilters.DateTypeList = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.OrderBy(a => a.Value).ToList();
			model.HotelsDeptSearchFilters.BookingStatusList = response.BookingStatusList.Where(a => !StatusIgnoreList.Contains(a.Attribute_Id)).OrderBy(a => a.AttributeName).ToList();
			return View(model);
		}

		[Route("_HotelsResult")]
		public IActionResult _HotelsResult(HotelsDeptSearchFilters searchFilters)
		{
			HotelsDeptViewModel model = new HotelsDeptViewModel();
			return PartialView(model);
		}

		[HttpPost]
		[Route("LoadData")]
		public ActionResult LoadData(HotelsDeptSearchFilters searchFilters, int draw, int start, int length)
		{
			BookingSearchReq BookingReq = new BookingSearchReq();
			HotelsDeptSearchRes BookingRes = new HotelsDeptSearchRes();
			BookingReq.AgentName = searchFilters.AgentName;
			BookingReq.AgentCode = searchFilters.AgentCode;
			BookingReq.BookingNumber = searchFilters.CNKReferenceNo;
			BookingReq.BookingName = searchFilters.AgentTour;
			BookingReq.Status = searchFilters.BookingStatus;
			BookingReq.DateType = searchFilters.DateType;
			BookingReq.From =  searchFilters.From;
			BookingReq.To =  searchFilters.To;

			BookingReq.Start = start;
			if (length == 0)
				length = 10;
			BookingReq.Length = length;
			BookingRes = hotelsProviders.GetHotelsByBookingDetails(BookingReq, token).Result;

			//Duration Details
			for (int i = 0; i < BookingRes.BookingsDetails.Count; i++)
			{
				int duration = Convert.ToInt32(BookingRes.BookingsDetails[i].Duration);
				BookingRes.BookingsDetails[i].Duration = duration + "N/" + (duration + 1) + "D";
			}

			//Rooming Details
			var rooms = "";
			for (int i = 0; i < BookingRes.BookingsDetails.Count; i++)
			{
				if (BookingRes.BookingsDetails[i].BookingRooms != null && BookingRes.BookingsDetails[i].BookingRooms.Count > 0)
				{
					for (int j = 0; j < BookingRes.BookingsDetails[i].BookingRooms.Count; j++)
					{
						if (j > 0)
						{
							rooms = rooms + " , " + BookingRes.BookingsDetails[i].BookingRooms[j].ROOMNO + " x " + BookingRes.BookingsDetails[i].BookingRooms[j].SUBPROD;
						}
						else
						{
							rooms = BookingRes.BookingsDetails[i].BookingRooms[j].ROOMNO + " x " + BookingRes.BookingsDetails[i].BookingRooms[j].SUBPROD;
						}

						BookingRes.BookingsDetails[i].StrBookingRooms = rooms;
					}
				}
			}

			return Json(new
			{
				draw = draw,
				recordsTotal = BookingRes.HotelsTotalCount,
				recordsFiltered = BookingRes.HotelsTotalCount,
				data = BookingRes.BookingsDetails
			});
		}

		#endregion

		#region View Hotels by booking number
		[Route("ViewHotelsByBooking")]
		public IActionResult ViewHotelsByBooking()
		{
			HotelsViewBookingViewModel model = new HotelsViewBookingViewModel();
			try
			{
				string BookingNo = Request.Query["BookingNo"].ToString();

				#region Get Hotel Booking details by booking number
				HotelsByBookingGetRes response = hotelsProviders.GetProductHotelDetails(new ProductSRPHotelGetReq() { QRFID = BookingNo }, token).Result;
				model.Bookings = response.Bookings;

				model.Bookings.Positions.ForEach(b =>
				{
					//b.AlternateServices = b.AlternateServices.OrderBy(a => a.Product_Name).ToList();
					b.BookingRoomsAndPrices = b.BookingRoomsAndPrices.Where(a => a.Req_Count > 0).OrderBy(a =>
						a.RoomName.Contains("SINGLE") ? "A" : a.RoomName.Contains("DOUBLE") ? "B" :
						a.RoomName.Contains("TWIN") ? "C" : a.RoomName.Contains("TRIPLE") ? "D" :
						a.RoomName.Contains("QUAD") ? "E" : a.RoomName.Contains("TSU") ? "F" :
						a.PersonType.ToLower().Contains("child + bed") ? "G" : a.PersonType.ToLower().Contains("child - bed") ? "H" :
						a.RoomName.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.RoomName).ThenBy(a => a.PersonType).ThenBy(a => a.Req_Count).ToList();
				});

				//Sorting by availablity status with order available, waitlist, pending, unavailable and twin room rate ascending
				model.Bookings.Positions.ForEach(b =>
				{
					if (b.AlternateServices != null && b.AlternateServices.Count > 0)
					{
						b.AlternateServices.ForEach(c => c.Availability_Status = (c.Availability_Status == null ? "" : c.Availability_Status));

						b.AlternateServices = b.AlternateServices.OrderBy(a =>
						  a.Availability_Status.ToUpper() == "AVAILABLE" ? "A" : a.Availability_Status.ToUpper() == "WAITLIST" ? "D" :
						  a.Availability_Status.ToUpper() == "PENDING" ? "F" : a.Availability_Status.ToUpper() == "UNAVAILABLE" ? "I" : "M")
						.ThenBy(a => a.Availability_Status).ThenBy(a => a.PPTwin_Rate).ToList();
					}
					else
					{
						b.AlternateServices = new List<AlternateServices>();
					}
				});

				List<BookingProductsSRPInfo> list = new List<BookingProductsSRPInfo>();
				model.SRPViewModelList.AddRange(response.Bookings.Positions.Select(a => new BookingProductsSRPInfo
				{
					PositionId = a.Position_Id,
					ProductsSRPViewModel = new ProductsSRPViewModel
					{
						PageName = "hotel",
						ProductSRPDetails = response.ProductSRPDetails.Where(b => b.VoyagerProduct_Id == a.Product_Id).ToList()
					}
				}).ToList());

				#endregion

				#region Get Costing Officer Tour Info Header By QRFId
				NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = response.Bookings.QRFID };
				model.COHeaderViewModel = coCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
                model.COHeaderViewModel.BookingNumber = BookingNo;
                #endregion

                #region Status dropdownlist binding

                var name = new List<string>();
				var attr = new List<Attributes>();
				foreach (var p in model.Bookings.Positions)
				{
					var altservices = model.Bookings.Positions.Where(a => a.Position_Id == p.Position_Id).FirstOrDefault().AlternateServices.Select(x => new Attributes { Attribute_Id = p.Position_Id, AttributeName = x.Availability_Status }).Distinct().ToList();
					name = altservices.Select(x => x.AttributeName).Distinct().ToList();
					foreach (var alt in name)
					{
						attr.Add(new Attributes { AttributeName = alt, Attribute_Id = p.Position_Id });
					}
				}
				attr.RemoveAll(x => string.IsNullOrWhiteSpace(x.AttributeName));
				model.Status = attr.OrderBy(x => x.AttributeName == "AVAILABLE" ? "A" : x.AttributeName == "WAITLIST" ? "D" :
				x.AttributeName == "PENDING" ? "F" : x.AttributeName == "UNAVAILABLE" ? "I" : "M").ToList();

				#endregion

				if (string.IsNullOrEmpty(model.COHeaderViewModel.QRFID))
				{
					model.COHeaderViewModel.TourName = model.Bookings.BookingNumber + " : " + model.Bookings.CustRef;
					model.COHeaderViewModel.NoOfNights = Convert.ToInt16(model.Bookings.Duration);
					model.COHeaderViewModel.NoOfDays = model.COHeaderViewModel.NoOfNights + 1;
					model.COHeaderViewModel.TravelDate = model.Bookings.STARTDATE;
					model.COHeaderViewModel.SalesPerson = model.Bookings.StaffDetails.Staff_SalesUser_Name;
					model.COHeaderViewModel.CostingOfficer = model.Bookings.StaffDetails.Staff_SalesUser_Name;
					model.COHeaderViewModel.Pax = model.Bookings.BookingPax.Where(a => a.PERSTYPE == "ADULT").Select(b => b.PERSONS).FirstOrDefault();
				}
				else
				{
					model.COHeaderViewModel.TourName = model.Bookings.BookingNumber + " : " + model.COHeaderViewModel.TourName;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			#region replacing image urls from resources to ImageResources
			foreach (var SRPViewModel in model.SRPViewModelList)
			{
				if (SRPViewModel.ProductsSRPViewModel != null)
				{
					foreach (var ProductSRP in SRPViewModel.ProductsSRPViewModel.ProductSRPDetails)
					{
						if (ProductSRP.HotelImageURL != null) ProductSRP.HotelImageURL = ProductSRP.HotelImageURL.Replace("resources/", "ImageResources/");
					}
				}
			}
			#endregion
			return View(model);
		}

		[Route("GetSimilarHotels")]
		public ActionResult GetSimilarHotels(string BookingNumber, string PositionId)
		{
			try
			{
				HotelAlternateServicesGetRes response = hotelsProviders.GetAlternateServicesByBooking(new HotelAlternateServicesGetReq() { BookingNumber = BookingNumber, PositionId = PositionId }, token).Result;
				SimilarHotelsViewModel model = new SimilarHotelsViewModel() { Caller = "bookings", BookingNumber = BookingNumber };

				if (response.AlternateServices.Count > 0)
				{
					model.SelectedHotelList = response.AlternateServices.Where(a => a.IsBlackListed == null || a.IsBlackListed == false)
						.Select(b => new ProductList
						{
							Category = b.Attributes != null ? b.Attributes.BdgPriceCategory : "",
							Location = b.Attributes != null ? b.Attributes.Location : "",
							StarRating = b.Attributes != null ? b.Attributes.StarRating : "",
							VoyagerProductId = b.Product_Id,
							Name = b.Product_Name,
							Supplier = b.SupplierInfo?.Name,
							SupplierId = b.SupplierInfo?.Id,
							LocationInfo = new ProductLocation { CityName = b.City, CountryName = b.Country }
						}).OrderBy(a => a.Name).ToList();

					model.BlackListedHotelList = response.AlternateServices.Where(a => a.IsBlackListed == true)
						.Select(b => new ProductList
						{
							Category = b.Attributes.BdgPriceCategory,
							Location = b.Attributes.Location,
							StarRating = b.Attributes.StarRating,
							VoyagerProductId = b.Product_Id,
							Name = b.Product_Name,
							Supplier = b.SupplierInfo?.Name,
							SupplierId = b.SupplierInfo?.Id,
							LocationInfo = new ProductLocation { CityName = b.City, CountryName = b.Country }
						}).OrderBy(a => a.Name).ToList();

					foreach (var hotel in model.SelectedHotelList)
					{
						hotel.VoyagerProductId = hotel.VoyagerProductId + "|" + hotel.SupplierId;
						hotel.Name = hotel.Name + " (";
						if (!string.IsNullOrEmpty(hotel.LocationInfo.CityName))
							hotel.Name = hotel.Name + hotel.LocationInfo.CityName;
						if (!string.IsNullOrEmpty(hotel.StarRating))
							hotel.Name = hotel.Name + ", " + hotel.StarRating;
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
							hotel.Name = hotel.Name + ", " + hotel.StarRating;
						if (!string.IsNullOrEmpty(hotel.Category))
							hotel.Name = hotel.Name + "/" + hotel.Category;
						if (!string.IsNullOrEmpty(hotel.Location))
							hotel.Name = hotel.Name + "/" + hotel.Location;
						if (!string.IsNullOrEmpty(hotel.Supplier))
							hotel.Name = hotel.Name + "/" + hotel.Supplier;
						hotel.Name = hotel.Name + ")";
					}
				}
				return PartialView("~/Views/Accomodation/_SimilarHotels.cshtml", model);
			}
			catch (Exception ex)
			{
				return PartialView("~/Views/Accomodation/_SimilarHotels.cshtml");
			}
		}		
		#endregion

		#region Send booking email to suppliers
		[Route("SendReservationRequestEmail")]
		public JsonResult SendReservationRequestEmail(string BookingNumber, string PositionId, string SendType)
		{
			try
			{
				#region Get Hotel Booking details by booking number
				//HotelsViewBookingViewModel model = new HotelsViewBookingViewModel();
				HotelReservationRequestEmail requestEmail = new HotelReservationRequestEmail()
				{
					BookingNumber = BookingNumber,
					PositionId = PositionId,
					SendType = SendType,
					PlacerEmail = Request.Cookies["EmailId"],
					PlacerUserId = Request.Cookies["VoyagerUser_Id"],
					WebURLInitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value
				};
				HotelReservationEmailRes response = hotelsProviders.SendHotelReservationRequestEmail(requestEmail, token).Result;
				#endregion

				if (response != null && response?.ResponseStatus?.Status != null && response?.ResponseStatus?.Status?.ToLower() == "success")
					return Json(new { status = "success", msg = response.ResponseStatus.ErrorMessage });
				else
					return Json(new { status = "failure", msg = response.ResponseStatus.ErrorMessage });
			}
			catch (Exception ex)
			{
				return Json(new { status = "failure", msg = ex.Message });
			}
		}
		#endregion

		#region View and update Hotel availability requests
		[AllowAnonymous]
		[HttpGet]
		[Route("HotelAvailabilityRequest")]
		public ActionResult HotelAvailabilityRequest(string bn, string pid, string asid, string st, string avl, string om, string clr = "email", string pagename = "")
		{
			UpdateAvailabilityViewModel model = new UpdateAvailabilityViewModel();
			try
			{
				AvailabilityRequestDetailsGetReq requestAvailability = new AvailabilityRequestDetailsGetReq()
				{
					BookingNumber = bn,
					PositionId = pid,
					AltSvcId = asid,
					MailStatus = st,
					MailType = "",
					QRFID = ""
				};
				AvailabilityRequestDetailsGetRes response = hotelsProviders.GetHotelAvailabilityRequestDetails(requestAvailability).Result;

				#region Set Costing Officer Tour Info Header
				model.COHeaderViewModel.QRFID = response.CostingGetProperties.QRFID;
				model.COHeaderViewModel.AgentName = response.CostingGetProperties.AgentInfo.AgentName;
				model.COHeaderViewModel.TourCode = response.CostingGetProperties.AgentProductInfo.TourCode;
				model.COHeaderViewModel.TourName = response.CostingGetProperties.AgentProductInfo.TourName;
				model.COHeaderViewModel.NoOfNights = Convert.ToInt32(response.CostingGetProperties.AgentProductInfo.Duration);
				model.COHeaderViewModel.NoOfDays = Convert.ToInt32(response.CostingGetProperties.AgentProductInfo.Duration) + 1;
				model.COHeaderViewModel.SalesPerson = response.CostingGetProperties.SalesOfficer;
				model.COHeaderViewModel.ContactPerson = response.CostingGetProperties.AgentInfo.ContactPerson;
				model.COHeaderViewModel.Destination = response.CostingGetProperties.AgentProductInfo.Destination;
				model.COHeaderViewModel.TravelDate = response.CostingGetProperties.DepartureDates.Count > 0 ? response.CostingGetProperties.DepartureDates[0].Date : null;
				model.COHeaderViewModel.Version = response.CostingGetProperties.VersionId;
				model.COHeaderViewModel.SystemPhone = response.CostingGetProperties.AgentInfo.MobileNo;
                model.COHeaderViewModel.BookingNumber = bn;
                #endregion

                #region Set SRP, Request Details & etc
                model.ProductsSRPViewModel = new ProductsSRPViewModel
				{
					PageName = "hotel",
					ProductSRPDetails = response.ProductSRPDetails
				};

				model.SupplierId = response.UpdateReqDetails.SupplierId;
				model.BookingNumber = bn;
				model.Status = st;
				model.OpenMode = om?.ToLower();
				model.Availability = !string.IsNullOrEmpty(avl) ? avl : response.UpdateReqDetails.Availability;
				model.PositionId = pid;
				model.AltSvcId = asid;
				model.BudgetCurrencyList = response.CurrencyList;
				model.UpdateReqDetails = response.UpdateReqDetails;
				model.ReservationRequestDetails = response.ReservationRequestDetails;
				model.RoomRateDetails = response.AltSvcRoomsAndPrices;
				model.BudgetSupplements = response.BudgetSupplements;

				if (response.ResponseStatus.Status == "pending" || st == "pending")
					model.Status = "pending";
				else if (response.ResponseStatus.Status == "done")
					model.Status = "done";

				#endregion

				ViewBag.Caller = clr;

				if (pagename == "view")
				{
					#region GetCommunicationTrail
					DocumentStoreGetReq req = new DocumentStoreGetReq();
					req.BookingNumber = bn;
					req.AlternateService_Id = asid;
					CommunicationTrailViewModel objCommunicationTrailViewModel = communicationTrailMapping.GetCommunicationTrail(req, token);
					model.CommunicationTrailViewModel = objCommunicationTrailViewModel;
					#endregion
				}



				ViewBag.Page = pagename;

				return View(model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("LoadCommunicationTrail")]
		public IActionResult LoadCommunicationTrail(string bn, string asid)
		{
			CommunicationTrailViewModel model = new CommunicationTrailViewModel();
			try
			{
				#region GetCommunicationTrail
				DocumentStoreGetReq req = new DocumentStoreGetReq();
				req.BookingNumber = bn;
				req.AlternateService_Id = asid;
				CommunicationTrailViewModel objCommunicationTrailViewModel = communicationTrailMapping.GetCommunicationTrail(req, token);
				model = objCommunicationTrailViewModel;
				#endregion

				return PartialView("~/Views/Shared/CommunicationTrail/_CommunicationTrail.cshtml", objCommunicationTrailViewModel);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("UpdateAvailabilityRequest")]
		public ActionResult UpdateAvailabilityRequest(UpdateAvailabilityViewModel model, string command, string clr = "email")
		{
			try
			{
				string[] commands = command.Split("~");
				AvailabilityRequestDetailsSetReq requestAvailability = new AvailabilityRequestDetailsSetReq()
				{
					BookingNumber = model.BookingNumber,
					PositionId = model.PositionId,
					AltSvcId = model.AltSvcId,
					QRFID = model.COHeaderViewModel.QRFID,
					AltSvcRoomsAndPrices = model.RoomRateDetails,
					UpdateReqDetails = model.UpdateReqDetails,
					Status = commands[0],
					UserEmailId = ckUserEmailId,
					Caller = commands[1],
					PlacerUserId = ckLoginUser_Id,
					BudgetSupplements = model.BudgetSupplements
				};
				HotelReservationEmailRes response = hotelsProviders.UpdateHotelAvailabilityRequest(requestAvailability).Result;

				if (response.ResponseStatus.Status == "Success") model.Status = "success";
				else { model.Status = "error"; model.StatusMessage = response.ResponseStatus.ErrorMessage; }

				if (commands[1] == "ui")
					return RedirectToAction("ViewHotelsByBooking", new { BookingNo = model.BookingNumber });
				else
					return View("HotelAvailabilityRequest", model);
				//return RedirectToAction("HotelAvailabilityRequest", new { bn = model.BookingNumber, pid = model.PositionId, asid = model.AltSvcId, st = model.Status, avl = command });
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		[Route("GetBudgetSupplement")]
		public ActionResult GetBudgetSupplement(UpdateAvailabilityViewModel model, List<BudgetSupplements> BudgetSupplementsOld)
		{
			BudgetSupplementGetReq budgetSupplementGetReq = new BudgetSupplementGetReq()
			{
				BookingNumber = model.BookingNumber,
				PositionId = model.PositionId,
				AltSvcId = model.AltSvcId,
				QRFID = model.COHeaderViewModel.QRFID,
				AltSvcRoomsAndPrices = model.RoomRateDetails,
				UserEmailId = ckUserEmailId
			};
			BudgetSupplementGetRes response = hotelsProviders.GetBudgetSupplement(budgetSupplementGetReq, token).Result;

			if (response != null && response.ResponseStatus.Status == "Success")
			{
				if (BudgetSupplementsOld?.Count > 0 && response.BudgetSupplements?.Count > 0)
				{
					foreach (var supp in response.BudgetSupplements)
					{
						var oldSupp = BudgetSupplementsOld.Where(a => a.BookingRooms_Id == supp.BookingRooms_Id).FirstOrDefault();
						if (oldSupp != null)
						{
							supp.BudgetSupplementReason = oldSupp.BudgetSupplementReason;
							supp.ApplyMarkUp = oldSupp.ApplyMarkUp;
							supp.AgentConfirmed = oldSupp.AgentConfirmed;
						}
					}
				}
				model.BudgetSupplements = response.BudgetSupplements;
				ViewBag.IsHideSaveButton = true;//Just to hide Save button
				return PartialView("_BudgetSupplement", model);
			}
			return Json("failure");
		}


		[HttpPost]
		[Route("SetBudgetSupplement")]
		public ActionResult SetBudgetSupplement(UpdateAvailabilityViewModel model)
		{
			BudgetSupplementSetReq budgetSupplementSetReq = new BudgetSupplementSetReq()
			{
				BookingNumber = model.BookingNumber,
				PositionId = model.PositionId,
				AltSvcId = model.AltSvcId,
				QRFID = model.COHeaderViewModel.QRFID,
				BudgetSupplements = model.BudgetSupplements,
				UserEmailId = ckUserEmailId
			};
			CommonResponse response = hotelsProviders.SetBudgetSupplement(budgetSupplementSetReq, token).Result;

			if (response != null)
			{
				return Json(response.ResponseStatus);
			}
			return Json("failure");
		}
		#endregion

		#region Remind email functionality
		[Route("RemindSupplier")]
		public JsonResult RemindSupplier(string BookingNumber, string PositionId, string AltSvcId)
		{
			try
			{
				#region Send RemindSupplier Mail
				HotelReservationRequestEmail requestEmail = new HotelReservationRequestEmail()
				{
					BookingNumber = BookingNumber,
					PositionId = PositionId,
					AltSvcId = AltSvcId,
					SendType = "remind",
					PlacerEmail = ckUserEmailId,
					PlacerUserId = ckLoginUser_Id,
					WebURLInitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value
				};
				HotelReservationEmailRes response = hotelsProviders.SendHotelReservationRequestEmail(requestEmail, token).Result;
				if (response != null && response.ResponseStatus != null)
				{
					if (response.ResponseStatus.Status == "Success")
					{
						return Json(new { status = "success", msg = response.ResponseStatus.ErrorMessage });
					}
					else
					{
						return Json(new { status = "failure", msg = response.ResponseStatus.ErrorMessage });
					}
				}
				else
				{
					return Json(new { status = "failure", msg = "An Error Occured." });
				}
				#endregion
			}
			catch (Exception ex)
			{
				return Json(new { status = "failure", msg = ex.Message });
			}
		}
		#endregion

		#region Switch Hotel functionality
		[HttpGet]
		[Route("_ActivateHotelRequest")]
		public ActionResult _ActivateHotelRequest(string bn, string pid, string asid)
		{
			ActivateHotelViewModel model = new ActivateHotelViewModel();
			try
			{
				AvailabilityRequestDetailsGetReq requestAvailability = new AvailabilityRequestDetailsGetReq()
				{
					BookingNumber = bn,
					PositionId = pid,
					AltSvcId = asid
				};
				ActivateHotelDetailsGetRes response = hotelsProviders.GetHotelActivationDetails(requestAvailability, token).Result;

				#region Set model details from service response

				model.BookingNumber = bn;
				model.PositionId = pid;
				model.AltSvcId = asid;
				model.PositionProductDetails = response.PositionProductDetails;
				model.ReservationRequestDetails = response.ReservationRequestDetails;
				model.UpdateReqDetails = response.UpdateReqDetails;
				model.RoomRateDetails = response.AltSvcRoomsAndPrices;
				model.PosProductSRPViewModel = new ProductsSRPViewModel
				{
					PageName = "hotel",
					ProductSRPDetails = response.PosProductSRPDetails
				};
				model.AltProductSRPViewModel = new ProductsSRPViewModel
				{
					PageName = "hotel",
					ProductSRPDetails = response.AltProductSRPDetails
				};
				#endregion
				return View(model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		[Route("SwitchProduct")]
		public JsonResult SwitchProduct(string BookingNumber, string PositionId, string AltSvcId, string SwitchType, string OptionDate, string CancellationDeadline)
		{
			ActivateHotelViewModel model = new ActivateHotelViewModel();
			try
			{
				AvailabilityRequestDetailsGetReq requestAvailability = new AvailabilityRequestDetailsGetReq()
				{
					BookingNumber = BookingNumber,
					PositionId = PositionId,
					AltSvcId = AltSvcId,
					MailType = SwitchType,
					UserEmailId = ckUserEmailId,
					PlacerUserId = ckLoginUser_Id,
					PageType = "hotels",
					OptionDate = OptionDate,
					CancellationDeadline = CancellationDeadline
				};
				HotelReservationEmailRes response = hotelsProviders.UpdateHotelActivationDetails(requestAvailability, token).Result;

				if (response != null && response.ResponseStatus != null)
				{
					return Json(new { status = response.ResponseStatus.Status.ToLower(), message = response.ResponseStatus.ErrorMessage });
				}
				else
				{
					return Json(new { status = "error", message = "No response received" });
				}
			}
			catch (Exception ex)
			{
				return Json("error");
			}
		}
		#endregion

		#region Send Emails on View Hotels requests
		[HttpGet]
		[Route("GetHotelEmailContent")]
		public ActionResult _HotelRequestSendEmail(string BookingNo, string PositionId, string AltSvcId, string doctype, string SupplierId)
		{
			EmailTemplateGetRes model = new EmailTemplateGetRes();
			try
			{
				if (doctype == "GE") doctype = DocType.FOLLOWUP;
				else if (doctype == "RQ") doctype = DocType.FOLLOWUPEXPENSIVE;
				else if (doctype == "EO") doctype = DocType.BOOKOPTEXT;
				else if (doctype == "PROV") doctype = DocType.BOOKPROV;
				else if (doctype == "KK") doctype = DocType.BOOKKK;

				EmailGetReq emailGetReq = new EmailGetReq()
				{
					BookingNo = BookingNo,
					PositionId = PositionId,
					AlternateServiceId = AltSvcId,
					DocumentType = doctype,
					PlacerUserId = ckLoginUser_Id,
					UserEmail = ckUserEmailId,
					IsSendEmail = false,
					SupplierId = SupplierId
				};
				EmailGetRes response = hotelsProviders.GenerateAndSendEmail(emailGetReq, token).Result;
				model = response.EmailTemplateGetRes.Count > 0 ? response.EmailTemplateGetRes[0] : null;
				return PartialView(model);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		[Route("SendMailToSupplier")]
		public JsonResult SendMailToSupplier(string From, string To, string CC, string Subject, string Body, string BookingNo, string PositionId, string AltSvcId, string doctype, string SupplierId)
		{
			EmailTemplateGetRes model = new EmailTemplateGetRes();
			try
			{
				if (doctype == "GE") doctype = DocType.FOLLOWUP;
				else if (doctype == "RQ") doctype = DocType.FOLLOWUPEXPENSIVE;
				else if (doctype == "EO") doctype = DocType.BOOKOPTEXT;
				else if (doctype == "PROV") doctype = DocType.BOOKPROV;
				else if (doctype == "KK") doctype = DocType.BOOKKK;

				EmailTemplateGetRes emailGetReq = new EmailTemplateGetRes();
				emailGetReq.From = From;
				emailGetReq.To = To;
				emailGetReq.CC = CC;
				emailGetReq.Subject = Subject;
				emailGetReq.Body = Body;
				emailGetReq.UserEmail = ckUserEmailId;
				emailGetReq.EmailGetReq.IsSaveDocStore = true;
				emailGetReq.EmailGetReq.BookingNo = BookingNo;
				emailGetReq.EmailGetReq.PositionId = PositionId;
				emailGetReq.EmailGetReq.AlternateServiceId = AltSvcId;
				emailGetReq.EmailGetReq.DocumentType = doctype;
				emailGetReq.EmailGetReq.PlacerUserId = ckLoginUser_Id;
				emailGetReq.EmailGetReq.UserEmail = ckUserEmailId;
				emailGetReq.EmailGetReq.SupplierId = SupplierId;

				EmailGetRes response = hotelsProviders.SendEmail(emailGetReq, token).Result;
				if (response != null && response.ResponseStatus != null)
				{
					return Json(new { status = response.ResponseStatus.Status.ToLower(), message = response.ResponseStatus.ErrorMessage });
				}
				else
				{
					return Json(new { status = "error", message = "No response received" });
				}
			}
			catch (Exception ex)
			{
				return Json("error");
			}
		}

		#endregion

		[HttpPost]
		[Route("GetCommunicationTrailById")]
		public JsonResult GetCommunicationTrailById(DocumentStoreGetReq model)
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

	}
}