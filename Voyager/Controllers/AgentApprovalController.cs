using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;
//using Microsoft.Office.Interop.Outlook;
//using OutlookApp = Microsoft.Office.Interop.Outlook.Application;

namespace Voyager.Controllers
{
	[Authorize]
	public class AgentApprovalController : VoyagerController
	{
		#region Declaration
		private readonly IConfiguration _configuration;
		public COCommonLibrary cOCommonLibrary;
		private ItineraryGetReq itineraryGetReq;
		private ItineraryGetRes itineraryGetRes;
		private COProviders coProviders;
		private AgentApprovalMapping agentApprovalMapping;
		private ItineraryViewModel model = new ItineraryViewModel();
		#endregion

		public AgentApprovalController(IConfiguration configuration) : base(configuration)
		{
			_configuration = configuration;
			cOCommonLibrary = new COCommonLibrary(configuration);
			coProviders = new COProviders(_configuration);
			agentApprovalMapping = new AgentApprovalMapping(_configuration);
		}

		#region Itinerary
		[Route("AgentApproval/Itinerary")]
		public IActionResult Itinerary(string QRFID)
		{
			ItineraryViewModel model = new ItineraryViewModel();
			model.MenuViewModel.QRFID = QRFID;
			model.MenuViewModel.MenuName = "Itinerary";

			#region Get Costing Officer Tour Info Header By QRFId
			NewQuoteViewModel modelQuote = new NewQuoteViewModel();
			modelQuote.QRFID = QRFID;
			model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
			model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
			model.AgentApprovalHeaderButtons = new AgentApprovalHeaderButtons();
			model.AgentApprovalHeaderButtons.QRFID = QRFID;
			model.AgentApprovalHeaderButtons.QRFPriceID = model.COHeaderViewModel.QRFPriceID;
			#endregion

			return View("Itinerary/Itinerary", model);
		}
		#endregion

		#region Proposal
		[Route("AgentApproval/Proposal")]
		public IActionResult Proposal(string QRFID)
		{
			ProposalViewModel model = new ProposalViewModel();
			model.MenuViewModel.QRFID = QRFID;
			model.MenuViewModel.MenuName = "Proposal";

			bool GetStatus = false;
			#region Get Costing Officer Tour Info Header By QRFId
			NewQuoteViewModel modelQuote = new NewQuoteViewModel();
			modelQuote.QRFID = QRFID;
			model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
			model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
			model.AgentApprovalHeaderButtons = new AgentApprovalHeaderButtons();
			model.AgentApprovalHeaderButtons.QRFID = QRFID;
			model.AgentApprovalHeaderButtons.QRFPriceID = model.COHeaderViewModel.QRFPriceID;
			#endregion

			ProposalLibrary proposalLibrary = new ProposalLibrary(_configuration);

			#region PriceBreakUp
			if (!string.IsNullOrEmpty(QRFID))
			{
				model.QRFID = QRFID;
				GetStatus = proposalLibrary.GetProposalPriceBreakupByQRFId(_configuration, token, ref model, QRFID);
			}

			ProposalGetReq request = new ProposalGetReq();
			ProposalGetRes response = new ProposalGetRes();
			request.QRFID = QRFID;
			response = coProviders.GetProposal(request, token).Result;
			if (response != null)
			{
				model.ProposalId = response.Proposal.ProposalId;
				model.ProposalPriceBreakupViewModel.PriceBreakUp = response.Proposal.PriceBreakup;
			}
			#endregion

			return View("Proposal/Proposal", model);
		}
		#endregion

		#region SendToClient & AgentAccept
		[HttpPost]
		public IActionResult SendToClientMail(SendToClientSetReq model)
		{
			//OutlookApp outlookApp = new OutlookApp();
			//MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);

			//mailItem.Subject = "This is the subject";
			//mailItem.HTMLBody = model.SendToClientHtml;

			////Set a high priority to the message
			//mailItem.Importance = OlImportance.olImportanceHigh;
			//mailItem.Display(false);

			//return Json(new
			//{
			//    status = "",
			//    msg = ""
			//});

			model.UserName = ckUserName;
            model.VoyagerUserId = ckLoginUser_Id;
			SendToClientSetRes objSendToClientSetRes = agentApprovalMapping.SendToClientMail(model, token);
			return Json(new
			{
				status = objSendToClientSetRes.ResponseStatus.Status,
				msg = objSendToClientSetRes.ResponseStatus.ErrorMessage
			});
		}

		[HttpPost]
		public IActionResult GetSendToClientDetails(SendToClientGetReq model)
		{
			model.UserCompanyId = ckUserCompanyId;
			SendToClientSetRes objSendToClientSetRes = agentApprovalMapping.GetSendToClientDetails(model, token);
			return PartialView("SendToClient/_SendToClient", objSendToClientSetRes);
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult GetSuggestSendToClient(string QRFID, string id, string emailid, string status)
		{
			OfflineMessageViewModel model = agentApprovalMapping.GetSuggestSendToClient(new GetSuggestionReq { QRFPriceID = id, QRFID = QRFID, MailStatus = status, Document_Id = emailid });
			model.QRFPriceID = id;
			model.QRFID = QRFID;

			return View("SendToClient/OfflineMessage", model);
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult SetSuggestSendToClient(SendToClientGetReq request)
		{
			SendToClientGetRes model = agentApprovalMapping.SetSuggestSendToClient(request);
			return Json(new
			{
				status = model.ResponseStatus.Status,
				msg = model.ResponseStatus.ErrorMessage
			});
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult AcceptSendToClient(string QRFID, string id, string emailid, string status)
		{
			//  DeleteAllCookies();
			OfflineMessageViewModel model = new OfflineMessageViewModel();
			model = agentApprovalMapping.AcceptSendToClient(new SendToClientGetReq { QRFPriceID = id, QRFID = QRFID, MailStatus = status, Document_Id = emailid });

			return View("SendToClient/OfflineMessage", model);
		}

		[HttpGet]
		public IActionResult AgentAcceptSendToClient(string QRFID, string id, string status)
		{
			OfflineMessageViewModel model = new OfflineMessageViewModel();
			ViewData.Add("agentaccept", "true");
			if (status == "agentaccept")
			{
				status = "accepted";
				model = agentApprovalMapping.AcceptSendToClient(new SendToClientGetReq { QRFPriceID = id, QRFID = QRFID, MailStatus = status, Type = "agentaccept", UserName = ckUserName, UserEmailId = ckUserEmailId, VoyagerUserId = ckLoginUser_Id });
			}
			else
			{
				model.Status = "invalid";
				model.MailStatus = "accepted";
			}
			return View("SendToClient/OfflineMessage", model);
		}
		#endregion

		#region AcceptWithoutProposal
		[HttpPost]
		public IActionResult AcceptWithoutProposal(QuoteGetReq model)
		{
			EmailGetReq emailGetReq = new EmailGetReq()
			{
				QrfId = model.QRFID,
				UserName = ckUserName,
				UserEmail = ckUserEmailId,
				PlacerUserId = ckLoginUser_Id
			};
			CommonResponse objCommonResponse = agentApprovalMapping.AcceptWithoutProposal(emailGetReq, token);
			return Json(new
			{
				status = objCommonResponse.ResponseStatus.Status,
				msg = objCommonResponse.ResponseStatus.ErrorMessage
			});
		}
		#endregion

		#region CheckProposalGenerated
		[HttpPost]
		public IActionResult CheckProposalGenerated(QuoteGetReq model)
		{
			CommonResponse objCommonResponse = new CommonResponse() { ResponseStatus = new ResponseStatus() };
			try
			{
				objCommonResponse = agentApprovalMapping.CheckProposalGenerated(model, token);
			}
			catch (System.Exception ex)
			{
				objCommonResponse.ResponseStatus.Status = "Error";
				objCommonResponse.ResponseStatus.ErrorMessage = "An Error Occurs:- " + ex.Message;
			}

			return Json(new
			{
				status = objCommonResponse.ResponseStatus.Status,
				msg = objCommonResponse.ResponseStatus.ErrorMessage
			});
		}
		#endregion

		#region ActivityList
		[Route("AgentApproval/ActivityList")]
		public IActionResult ActivityList(string QRFID)
		{
			QRFID = string.IsNullOrEmpty(QRFID) ? Request.Query["QRFId"].ToString() : QRFID;
			model = GetSalesOfficerHeraderInfo(QRFID, "ActivityList");
			model.MenuViewModel.QRFID = QRFID;
			model.AgentApprovalHeaderButtons = new AgentApprovalHeaderButtons();
			model.AgentApprovalHeaderButtons.QRFID = QRFID;
			model.AgentApprovalHeaderButtons.QRFPriceID = model.COHeaderViewModel.QRFPriceID;
			return View("ActivityList/ActivityList", model);
		}

		public IActionResult GetActivityList(string QRFID, string filterByDay = null, string filterByServiceType = null)
		{
			model.MenuViewModel.QRFID = QRFID;

			#region Get Itinerary Details by QRFID
			itineraryGetReq = new ItineraryGetReq();
			itineraryGetReq.QRFID = QRFID;
			itineraryGetRes = coProviders.GetItinerary(itineraryGetReq, token).Result;
			model.Itinerary = itineraryGetRes.Itinerary;
			model.Itinerary.ItineraryDays.ForEach(a => a.ItineraryDescription.RemoveAll(b => b.IsDeleted));
			model.Itinerary.ItineraryDays.ForEach(a => a.Hotel.RemoveAll(b => b.IsDeleted));
			model.Itinerary.ItineraryDays.ForEach(a => a.Meal.RemoveAll(b => b.IsDeleted));
			#endregion

			#region Filter Dropdown Bindings
			model.DayList = GetDays(itineraryGetRes.Itinerary.ItineraryDays);
			model.ServiceTypeList = GetProductTypes(model.Itinerary.ItineraryDays);

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
			#endregion
			return PartialView("ActivityList/_ActivityList", model);
		}

		[HttpPost]
		public IActionResult SaveActivityList(ItineraryViewModel model)
		{
			try
			{
				ItinerarySetRes response = new ItinerarySetRes();
				ItinerarySetReq request = new ItinerarySetReq();

				request.itinerary.ItineraryID = model.Itinerary.ItineraryID;
				request.itinerary.ItineraryDays = model.Itinerary.ItineraryDays;
                request.QRFId = model.MenuViewModel.QRFID;
                request.VoyagerUserId = ckLoginUser_Id;
				response = coProviders.SetItinerary(request, token).Result;

				if (response.ResponseStatus.Status.ToLower() == "success")
				{
					TempData["success"] = "Details saved successfully";
				}
				else
				{
					TempData["error"] = response.ResponseStatus.Status;
				}
				return RedirectToAction("ActivityList", new { QRFId = model.MenuViewModel.QRFID });
			}
			catch (System.Exception ex)
			{
				return View();
			}
		}
		#endregion

		#region Common Function
		public ItineraryViewModel GetSalesOfficerHeraderInfo(string QRFID, string MenuName)
		{
			model = new ItineraryViewModel();
			model.MenuViewModel.QRFID = QRFID;
			model.MenuViewModel.MenuName = MenuName;

			#region Get Costing Officer Tour Info Header By QRFId
			NewQuoteViewModel modelQuote = new NewQuoteViewModel();
			modelQuote.QRFID = QRFID;
			model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
			model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
			model.AgentApprovalHeaderButtons = new AgentApprovalHeaderButtons();
			model.AgentApprovalHeaderButtons.QRFID = QRFID;
			model.AgentApprovalHeaderButtons.QRFPriceID = model.COHeaderViewModel.QRFPriceID;
			#endregion
			return model;
		}

		public List<Attributes> GetDays(List<ItineraryDaysInfo> itineraryDays)
		{
			List<Attributes> lstDays = new List<Attributes>();
			foreach (var day in itineraryDays)
			{
				lstDays.Add(new Attributes { Attribute_Id = day.Day, AttributeName = day.Day });
			}
			return lstDays;
		}

		public List<mProductType> GetProductTypes(List<ItineraryDaysInfo> itineraryDays)
		{
			List<mProductType> lstProductTypes = new List<mProductType>();
			foreach (var m in itineraryDays)
			{
				foreach (var n in m.ItineraryDescription)
				{
					if (!string.IsNullOrEmpty(n.ProductType))
					{
						if (lstProductTypes.Where(a => a.Prodtype == n.ProductType).Count() <= 0)
						{
							lstProductTypes.Add(new mProductType { Prodtype = n.ProductType });
						}
					}
				}
			}
			return lstProductTypes.OrderBy(x => x.Prodtype).ToList();
		}

		[HttpGet]
		public IActionResult CheckProposalGenerated(string QRFID)
		{
			ResponseStatus res = new ResponseStatus();
			try
			{
				res.Status = "Success";
			}
			catch (System.Exception ex)
			{
				res.Status = "Error";
				res.ErrorMessage = "An Error Occurs:- " + ex.Message;
			}
			return Json(new
			{
				status = res.Status,
				msg = res.ErrorMessage
			});
		}
		#endregion

		[HttpPost]
		public IActionResult AmendmentQuote(string QRFID)
		{
			try
			{
				AmendmentQuoteReq request = new AmendmentQuoteReq();
				request.QRFID = QRFID;
				request.EditUser = ckUserEmailId;
                request.VoyagerUserId = ckLoginUser_Id;

                AgentApprovalProviders agentApprovalProviders = new AgentApprovalProviders(_configuration);

				CommonResponse objResponse = agentApprovalProviders.AmendmentQuote(request, token).Result;

				return Json(objResponse);
			}
			catch (System.Exception ex)
			{
				throw;
			}
		}
	}
}