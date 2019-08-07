using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Voyager.Controllers
{
	[Authorize]
	public class QRFSummaryController : VoyagerController
	{
		#region Declaration

		QRFSummaryMapping qrfSummaryMapping;
		QRFSummaryProviders qrfSummaryProviders;
		//QRFSummaryGetReq qrfSummaryGetReq;
		//QRFSummaryGetRes qrfSummaryGetRes;
		//PositionGetReq positionGetReq;
		//PositionGetRes positionGetRes;
		//PositionSetReq positionSetReq;
		//PositionSetRes positionSetRes;
		PositionProviders positionProviders;
		//ProdTypeGetReq prodTypeGetReq;
		//ProdTypeGetRes prodTypeGetRes;
		MasterProviders masterProviders;
		ItineraryGetReq itineraryGetReq;
		ItineraryGetRes itineraryGetRes;
		COProviders coProviders;
		AgentProviders agentProviders;
		private OperationsProviders operationsProviders;
		private string SessionName = "IntegrationInfo";

		private readonly IConfiguration _configuration;

		public QRFSummaryController(IConfiguration configuration) : base(configuration)
		{
			_configuration = configuration;
			qrfSummaryProviders = new QRFSummaryProviders(_configuration);
			qrfSummaryMapping = new QRFSummaryMapping(_configuration);
			positionProviders = new PositionProviders(_configuration);
			masterProviders = new MasterProviders(_configuration);
			coProviders = new COProviders(_configuration);
			agentProviders = new AgentProviders(_configuration);
			operationsProviders = new OperationsProviders(_configuration);
		}

		#endregion

		public IActionResult QRFSummary(string filterByDay = null, string filterByServiceType = null)
		{
			try
			{
				string QRFID = Request.Query["QRFId"];
				QRFSummaryViewModel model = new QRFSummaryViewModel();
				model.QRFID = QRFID;
				model.MenuViewModel.QRFID = QRFID;
				model.MenuViewModel.MenuName = "Summary";
				model.CurrentDate = DateTime.Now.ToString("dd MMM yyyy");
				bool GetStatus = false;
				string username = "";
				IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
				objCookies.TryGetValue("UserName", out username);
				username = string.IsNullOrEmpty(username) ? ckUserName : username;
				model.UserName = username;
				#region Get Quote Info By QRFId
				NewQuoteViewModel modelQuote = new NewQuoteViewModel();
				modelQuote.QRFID = QRFID;
				SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
				GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
				if (GetStatus)
				{
					model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
				}

				CompanyOfficerGetRes officerGetRes = new CompanyOfficerGetRes();
				CompanyOfficerGetReq officerGetReq = new CompanyOfficerGetReq();
				officerGetReq.CompanyId = ckUserCompanyId;
				officerGetReq.UserRole = "Costing Officer";
				officerGetRes = agentProviders.GetCompanyOfficers(officerGetReq, token).Result;

				model.OfficerList = officerGetRes.ContactDetails;
				var salespipelineres = agentProviders.GetAutomatedSalesPipelineRoles(new SettingsAutomatedGetReq { CompanyId = ckUserCompanyId, QRFId = QRFID, UserRole = officerGetReq.UserRole }, token).Result;
				if (!string.IsNullOrEmpty(salespipelineres?.ResponseStatus?.Status))
				{
					model.Officer = salespipelineres.UserEmailId;
				}
				#endregion
				return View(model);
			}
			catch (Exception ex)
			{
				throw;
			}

		}

		public IActionResult GetQRFSummaryData(string QRFID, string filterByDay = null, string filterByServiceType = null, string PageName = "QRFSummary")
		{
			try
			{
				QRFSummaryViewModel model = new QRFSummaryViewModel();
				model.PageName = PageName;

				#region Get Quote Info By QRFId
				NewQuoteViewModel modelQuote = new NewQuoteViewModel();
				modelQuote.QRFID = QRFID;
				SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
				bool GetStatus = false;
				GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
				if (GetStatus)
				{
					model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
				}
				#endregion

				//#region Get Summary Detailsby QRFId
				//qrfSummaryGetReq = new QRFSummaryGetReq();
				//qrfSummaryGetReq.QRFID = QRFID;
				//qrfSummaryGetRes = qrfSummaryProviders.GetQRFSummary(qrfSummaryGetReq, token).Result;
				//model.SummaryDetails = qrfSummaryMapping.GetQRFSummaryDetails(qrfSummaryGetRes, token);
				//model.QRFID = QRFID;
				//#endregion

				#region Get Itinerary Details by QRFId
				itineraryGetReq = new ItineraryGetReq();
				itineraryGetReq.QRFID = QRFID;
				itineraryGetReq.Page = PageName;
				itineraryGetReq.editUser = ckUserEmailId;
				itineraryGetRes = coProviders.GetItineraryDetails(itineraryGetReq, token).Result;
				model.Itinerary = itineraryGetRes.Itinerary;
				#endregion

				#region Filter Dropdown Bindings
				model.DayList = GetDays(model.Itinerary.ItineraryDays);
				//model.ServiceTypeList = GetProductTypes();
				model.ServiceTypeList = GetProductTypes(model.Itinerary.ItineraryDays);
				#endregion

				if (filterByDay == "Day")
					filterByDay = null;

				if (filterByServiceType == "Service Type")
					filterByServiceType = null;

				if (!string.IsNullOrEmpty(filterByDay))
				{
					model.Itinerary.ItineraryDays = model.Itinerary.ItineraryDays.Where(x => x.Day == filterByDay).ToList();
				}

				if (!string.IsNullOrEmpty(filterByServiceType))
				{
					for (int i = 0; i < model.Itinerary.ItineraryDays.Count; i++)
					{
						model.Itinerary.ItineraryDays[i].ItineraryDescription = model.Itinerary.ItineraryDays[i].ItineraryDescription.Where(x => x.ProductType == filterByServiceType).ToList();
					}
				}
				model.Days = filterByDay;
				model.Services = filterByServiceType;
				return PartialView("_QRFSummary", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult GetItineraryData(string QRFID, string filterByDay = null, string filterByServiceType = null, string PageName = "QRFSummary")
		{
			try
			{
				QRFSummaryViewModel model = new QRFSummaryViewModel();
				model.PageName = PageName;

				#region Get Quote Info By QRFId
				NewQuoteViewModel modelQuote = new NewQuoteViewModel();
				modelQuote.QRFID = QRFID;
				SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
				bool GetStatus = false;
				GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
				if (GetStatus)
				{
					model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
				}
				#endregion

				#region Get Itinerary Details by QRFId
				itineraryGetReq = new ItineraryGetReq();
				itineraryGetReq.QRFID = QRFID;
				itineraryGetRes = coProviders.GetItinerary(itineraryGetReq, token).Result;
				model.Itinerary = itineraryGetRes.Itinerary;
				#endregion

				#region Filter Dropdown Bindings
				model.DayList = GetDays(model.Itinerary.ItineraryDays);
				model.ServiceTypeList = GetProductTypes(model.Itinerary.ItineraryDays);
				#endregion

				if (filterByDay == "Day")
					filterByDay = null;

				if (filterByServiceType == "Service Type")
					filterByServiceType = null;

				if (!string.IsNullOrEmpty(filterByDay))
				{
					model.Itinerary.ItineraryDays = model.Itinerary.ItineraryDays.Where(x => x.Day == filterByDay).ToList();
				}

				if (!string.IsNullOrEmpty(filterByServiceType))
				{
					for (int i = 0; i < model.Itinerary.ItineraryDays.Count; i++)
					{
						model.Itinerary.ItineraryDays[i].ItineraryDescription = model.Itinerary.ItineraryDays[i].ItineraryDescription.Where(x => x.ProductType == filterByServiceType).ToList();
					}
				}
				model.Days = filterByDay;
				model.Services = filterByServiceType;
				return PartialView("_QRFSummary", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult GetRemarksForPosition(string QRFID, string PositionId, string ItineraryId, string ItineraryDaysId)
		{
			try
			{
				//positionGetReq = new PositionGetReq();
				//positionGetReq.QRFID = QRFID;
				//positionGetReq.PositionId = PositionId;
				//positionGetReq.Type = "all";

				//positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
				OriginalItineraryDetails model = new OriginalItineraryDetails();

				#region Get Itinerary Details by QRFId
				itineraryGetReq = new ItineraryGetReq();
				itineraryGetReq.QRFID = QRFID;
				itineraryGetRes = coProviders.GetItinerary(itineraryGetReq, token).Result;
				#endregion

				if (itineraryGetRes != null && itineraryGetRes.Itinerary != null && !string.IsNullOrEmpty(itineraryGetRes.Itinerary.ItineraryID))
				{
					var day = itineraryGetRes.Itinerary.ItineraryDays.Where(x => x.ItineraryDaysId == ItineraryDaysId).FirstOrDefault();
					var pos = day.ItineraryDescription.Where(x => x.PositionId == PositionId).FirstOrDefault();

					model.TLRemarks = pos.TLRemarks;
					model.OPSRemarks = pos.OPSRemarks;
					model.PositionId = pos.PositionId;
					model.ItineraryId = itineraryGetRes.Itinerary.ItineraryID;
					model.ItineraryDaysId = day.ItineraryDaysId;
				}
				//model.TLRemarks = positionGetRes.mPosition[0].TLRemarks;
				//model.OPSRemarks = positionGetRes.mPosition[0].OPSRemarks;
				//model.PositionId = positionGetRes.mPosition[0].PositionId;

				return PartialView("_Remarks", model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult EditPosition(string QRFID, string PositionId, string ItineraryID, string ItineraryDaysId, string ProductType = null, string Type = null)
		{
			try
			{
				if (!string.IsNullOrEmpty(Type) && Type.ToUpper() == "EXTRA")
				{
					if (!string.IsNullOrEmpty(PositionId) && !string.IsNullOrEmpty(ItineraryID) && !string.IsNullOrEmpty(ItineraryDaysId))
					{
						return RedirectToAction("EditNewItineraryElement", "Proposal", new { QRFID, PositionId, ItineraryID, ItineraryDaysId });
					}
					else
					{
						return Content("No Content");
					}
				}
				else
				{
					//bool IsClone = true;
					string prodType = ProductType.ToUpper();
					switch (prodType)
					{
						case "MEAL":
							return RedirectToAction("GetMealsvenue", "Meals", new { QRFID, PositionId });

						case "HOTEL":
							return RedirectToAction("Accomodation", "Accomodation", new { QRFID, PositionId });

						case "ATTRACTIONS":
						case "SIGHTSEEING - CITYTOUR":
							return RedirectToAction("Activities", "Activities", new { QRFID, PositionId });

						case "COACH":
						case "LDC":
							return RedirectToAction("Bus", "Bus", new { QRFID, PositionId });

						case "OVERNIGHT FERRY":
							return RedirectToAction("Cruise", "Cruise", new { QRFID, PositionId });

						case "PRIVATE TRANSFER":
						case "SCHEDULED TRANSFER":
						case "FERRY TRANSFER":
						case "FERRY PASSENGER":
							return RedirectToAction("GetTransfersType", "Transfers", new { QRFID, PositionId });

						case "GUIDE":
						case "ASSISTANT":
							ProductType = "guide";
							return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType });
						case "VISA":
						case "INSURANCE":
							return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType });
						case "OTHER":
						case "FEE":
							ProductType = "other";
							return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType });

						case "DOMESTIC FLIGHT":
							return RedirectToAction("Flights", "Flights", new { QRFID, PositionId });

						case "TRAIN":
							return RedirectToAction("Rail", "Rail", new { QRFID, PositionId });

						default:
							return Content("No Content");
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpPost]
		public IActionResult SaveRemarksForPosition(string QRFID, string PositionId, string ItineraryId, string ItineraryDaysId, string TLRemarks, string OPSRemarks, string BookingNumber)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(BookingNumber))
				{
					OpsBookingItinerarySetReq req = new OpsBookingItinerarySetReq()
					{
						BookingNumber = BookingNumber,
						ItineraryDetails = new ItineraryDetails()
						{
							TLRemarks = TLRemarks,
							OPSRemarks = OPSRemarks,
							ItineraryDetail_Id = ItineraryId
						}
					};
					OpsBookingItinerarySetRes res = operationsProviders.SetRemarksForItineraryBuilderDetails(req, token).Result;
					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						return Json(new { responseText = "Success! Remarks updated successfully." });
					}
					else
					{
						return Content("Error");
					}
				}
				else
				{
					ItinerarySetRes response = new ItinerarySetRes();
					ItinerarySetReq request = new ItinerarySetReq();

					request.QRFId = QRFID;
					request.ItineraryId = ItineraryId;
					request.ItineraryDaysId = ItineraryDaysId;
					request.PositionId = PositionId;
					request.TLRemarks = TLRemarks;
					request.OPSRemarks = OPSRemarks;
					response = coProviders.SaveRemarks(request, token).Result;

					if (response.ResponseStatus.Status.ToLower() == "success")
					{
						return Json(new { responseText = "Success! Remarks updated successfully." });
					}
					else
					{
						return Content("Error");
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult EnableDisablePosition(string QRFID, string PositionId, string ItineraryId, string ItineraryDaysId, bool IsDeleted)
		{
			try
			{
				ItinerarySetRes response = new ItinerarySetRes();
				ItinerarySetReq request = new ItinerarySetReq();

				request.QRFId = QRFID;
				request.ItineraryId = ItineraryId;
				request.ItineraryDaysId = ItineraryDaysId;
				request.PositionId = PositionId;

				if (IsDeleted == false)
					IsDeleted = true;
				else
					IsDeleted = false;

				request.IsDeleted = IsDeleted;
				response = coProviders.EnableDisablePosition(request, token).Result;

				if (response.ResponseStatus.Status.ToLower() == "success")
				{
					return Json(new { responseText = "success" });
				}
				else
				{
					return Content("Error");
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			//try
			//{
			//    positionGetReq = new PositionGetReq();
			//    positionGetReq.QRFID = QRFID;
			//    positionGetReq.PositionId = PositionId;
			//    positionGetReq.Type = "all";

			//    positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

			//    if (positionGetRes.mPosition.Count == 1 && positionGetRes.mPosition[0].PositionId == PositionId)
			//    {
			//        positionSetReq = new PositionSetReq();
			//        positionSetReq.SaveType = "Complete";
			//        positionSetReq.mPosition = new List<mPosition>();
			//        positionSetReq.mPosition.Add(positionGetRes.mPosition[0]);

			//        if (IsDeleted == false)
			//            positionSetReq.mPosition[0].IsDeleted = true;
			//        else
			//            positionSetReq.mPosition[0].IsDeleted = false;

			//        positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;

			//        if (positionSetRes.ResponseStatus.Status.ToLower() == "success")
			//        {
			//            return Json("success");
			//        }
			//        else
			//        {
			//            return Content("Error");
			//        }
			//    }
			//    return View();
			//}
			//catch (Exception ex)
			//{
			//    throw;
			//}
		}

		[HttpPost]
		public IActionResult SubmitQuote(string QRFID, string QuoteRemarks, string Officer)
		{
			try
			{
				SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
				IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
				var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;

				QuoteSetRes objResponse = quoteLibrary.SetQuoteDetails(_configuration, token, QRFID, QuoteRemarks, Officer, objCookies, sessionData);

				if (objResponse?.ResponseStatus?.Status?.ToLower() == "success")
				{
					return Json(new { responseText = "success" });
				}
				else
				{
					return Json(new { responseText = objResponse?.ResponseStatus?.ErrorMessage , status= "error" });
				}
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		#region Helper Methods
		public List<Attributes> GetDays(List<ItineraryDaysInfo> summaryDetails)
		{
			List<Attributes> lstDays = new List<Attributes>();
			foreach (var day in summaryDetails)
			{
				lstDays.Add(new Attributes { Attribute_Id = day.Day, AttributeName = day.Day });
			}
			return lstDays;
		}

		//public List<mProductType> GetProductTypes1()
		//{
		//    List<mProductType> lstProductTypes = new List<mProductType>();
		//    ProdTypeGetReq prodTypeGetReq = new ProdTypeGetReq();
		//    prodTypeGetRes = masterProviders.GetProdTypeByProdType(prodTypeGetReq, token).Result;
		//    lstProductTypes = prodTypeGetRes.ProductTypeList;
		//    return lstProductTypes;
		//}

		public List<mProductType> GetProductTypes(List<ItineraryDaysInfo> summaryDetails)
		{
			List<mProductType> lstProductTypes = new List<mProductType>();
			foreach (var m in summaryDetails)
			{
				foreach (var n in m.ItineraryDescription)
				{
					if (lstProductTypes.Where(a => a.Prodtype == n.ProductType).Count() <= 0)
					{
						if (!string.IsNullOrEmpty(n.ProductType))
							lstProductTypes.Add(new mProductType { Prodtype = n.ProductType });
					}
				}
			}
			return lstProductTypes.OrderBy(x => x.Prodtype).ToList();

		}

		#endregion

	}
}