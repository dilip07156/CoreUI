using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
	[Authorize]
	public class ProposalController : VoyagerController
	{
		#region Declaration
		private readonly IConfiguration _configuration;
		public COCommonLibrary cOCommonLibrary;
		private ItineraryGetReq itineraryGetReq;
		private ItineraryGetRes itineraryGetRes;
		private ProposalGetReq proposalGetReq;
		private ProposalGetRes proposalGetRes;
		private LoginProviders loginProviders;
		private PositionProviders positionProviders;
		private AgentProviders agentProviders;
		private COProviders coProviders;
		private ItineraryMapper itineraryMapper;
		private SalesProviders objSalesProvider;
		private OperationsProviders operationsProviders;

		public ProposalController(IConfiguration configuration) : base(configuration)
		{
			_configuration = configuration;
			cOCommonLibrary = new COCommonLibrary(configuration);
			coProviders = new COProviders(_configuration);
			positionProviders = new PositionProviders(_configuration);
			itineraryMapper = new ItineraryMapper(_configuration);
			loginProviders = new LoginProviders(_configuration);
			agentProviders = new AgentProviders(_configuration);
			objSalesProvider = new SalesProviders(_configuration);
			operationsProviders = new OperationsProviders(_configuration);
		}
		#endregion

		#region Methods

		[Route("CostingOfficer/Proposal")]
		public IActionResult Proposal()
		{
			try
			{
				if (!UserRoles.Contains("Costing Officer"))
				{
					return View("Unauthorize");
				}

				string QRFID = Request.Query["QRFId"];
				bool GetStatus = false;
				ProposalViewModel model = new ProposalViewModel();
				model.MenuViewModel.QRFID = QRFID;
				model.MenuViewModel.MenuName = "Proposal";
				model.CurrentDate = DateTime.Now.ToString("dd MMM yyyy");
				IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
				objCookies.TryGetValue("UserName", out string username);
				username = string.IsNullOrEmpty(username) ? ckUserName : username;

				model.UserName = username;

				#region Get Costing Officer Tour Info Header By QRFId
				NewQuoteViewModel modelQuote = new NewQuoteViewModel();
				modelQuote.QRFID = QRFID;
				model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
				model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
				#endregion

				CompanyOfficerGetRes officerGetRes = new CompanyOfficerGetRes();
				CompanyOfficerGetReq officerGetReq = new CompanyOfficerGetReq();
				officerGetReq.CompanyId = ckUserCompanyId;
				officerGetReq.UserRole = "Product Accountant";
				officerGetRes = agentProviders.GetCompanyOfficers(officerGetReq, token).Result;

				model.OfficerList = officerGetRes.ContactDetails;
				var salespipelineres = agentProviders.GetAutomatedSalesPipelineRoles(new SettingsAutomatedGetReq { CompanyId = ckUserCompanyId, QRFId = QRFID, UserRole = officerGetReq.UserRole }, token).Result;
				if (!string.IsNullOrEmpty(salespipelineres?.ResponseStatus?.Status))
				{
					model.Officer = salespipelineres.UserEmailId;
				}

				ProposalLibrary proposalLibrary = new ProposalLibrary(_configuration);
				#region PriceBreakUp
				if (!string.IsNullOrEmpty(QRFID))
				{
					model.QRFID = QRFID;
					GetStatus = proposalLibrary.GetProposalPriceBreakupByQRFId(_configuration, token, ref model, QRFID);
				}

				//ProposalGetReq request = new ProposalGetReq();
				//ProposalGetRes response = new ProposalGetRes();
				//request.QRFID = QRFID;
				//response = coProviders.GetProposal(request, token).Result;
				//if (response != null)
				//{
				//    model.ProposalId = response.Proposal.ProposalId;
				//    if (!string.IsNullOrEmpty(response.Proposal.PriceBreakup))
				//        model.ProposalPriceBreakupViewModel.PriceBreakUp = response.Proposal.PriceBreakup;
				//} 
				#endregion

				return View(model);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public IActionResult CostingOfficerHeader()
		{
			string QRFID = Request.Query["QRFId"];
			ProposalViewModel model = new ProposalViewModel();

			#region Get Costing Officer Tour Info Header By QRFId
			NewQuoteViewModel modelQuote = new NewQuoteViewModel();
			modelQuote.QRFID = QRFID;
			model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
			ViewData.Add("EnquiryPipeline", modelQuote.mdlMenuViewModel.EnquiryPipeline);
			#endregion

			return PartialView("_CostingOfficerHeader", model.COHeaderViewModel);
		}

		#region Price Breakup
		public IActionResult ProposalPriceBreakup(string QRFID)
		{
			ProposalViewModel model = new ProposalViewModel();
			ProposalLibrary proposalLibrary = new ProposalLibrary(_configuration);
			#region Get Proposal Details by QRFId
			//proposalGetReq = new ProposalGetReq();
			//proposalGetReq.QRFID = QRFID;
			//proposalGetRes = coProviders.GetProposal(proposalGetReq, token).Result;
			//model.PriceBreakUp = proposalGetRes.Proposal.PriceBreakup;

			if (!string.IsNullOrEmpty(QRFID))
			{
				bool GetStatus = false;
				model.QRFID = QRFID;
				GetStatus = proposalLibrary.GetProposalPriceBreakupByQRFId(_configuration, token, ref model, QRFID);
			}
			#endregion

			return PartialView("_ProposalPriceBreakup", model.ProposalPriceBreakupViewModel);
		}

		[HttpPost]
		public IActionResult SaveProposalPriceBreakup(string QRFID, string ProposalId, string txtPriceBreakup)
		{
			string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
			#region Set Proposal by Proposal Id
			ProposalSetReq request = new ProposalSetReq();
			ProposalSetRes response = new ProposalSetRes();
			request.Proposal.QRFID = QRFID;
			request.Proposal.ProposalId = ProposalId;
			request.Proposal.PriceBreakup = txtPriceBreakup == null ? "" : txtPriceBreakup;
			request.Proposal.EditUser = UserName;
			request.Proposal.CreateUser = UserName;
			response = coProviders.SetProposal(request, token).Result;
			#endregion

			if (response.ResponseStatus.Status.ToLower() == "success")
			{
				return Json(new { responseText = "success" });
			}
			else
			{
				return Content("Error");
			}
		}
		#endregion

		#region Suggested Itinerary

		public IActionResult ProposalSuggestedItinerary(string QRFID)
		{
			try
			{
				ProposalViewModel proposalViewModel = new ProposalViewModel();
				proposalViewModel.QRFID = QRFID;

				#region Get Itinerary Details by QRFId
				itineraryGetReq = new ItineraryGetReq();
				itineraryGetReq.QRFID = QRFID;
				itineraryGetRes = coProviders.GetItinerary(itineraryGetReq, token).Result;
				proposalViewModel.Itinerary = itineraryGetRes.Itinerary;
				proposalViewModel.Itinerary.ItineraryDays.ForEach(a => a.ItineraryDescription.RemoveAll(b => b.IsDeleted));
				#endregion

				return PartialView("_ProposalSuggestedItinerary", proposalViewModel);
			}
			catch (Exception ex)
			{
				throw;
				return View();
			}
		}

		[HttpPost]
		public IActionResult SetSuggestedItinerary(ProposalViewModel model)
		{
			string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
			model.Itinerary.EditUser = ckUserEmailId;
			ItinerarySetRes response = new ItinerarySetRes();
			ItinerarySetReq request = new ItinerarySetReq();
			request.itinerary = model.Itinerary;
			request.IsNewVersion = model.IsNewVersion;
			request.itinerary.EditUser = UserName;
			response = coProviders.SetItinerary(request, token).Result;

			if (response.ResponseStatus.Status.ToLower() == "success")
			{
				return Json(new { responseText = "success" });
			}
			else
			{
				return Content("Error");
			}
		}

		public IActionResult AddNewItineraryDetails(string QRFID, string PositionId, string ItineraryID, string ItineraryDaysId, string Day, string action, string type = "")
		{
			ExtraPosition position = new ExtraPosition();
			position.Day = Day;
			position.Type = type;
			if (type == "opsItinerary")
			{
				#region Get dropdown values for Ops Service Itinerary Details  
				OpsBookingItineraryGetReq opsBookingGetReq = new OpsBookingItineraryGetReq() { BookingNumber = QRFID };
				OpsBookingItineraryGetRes opsBookingGetRes = operationsProviders.GetItineraryBuilderDetails(opsBookingGetReq, token).Result;
				position.DayList = opsBookingGetRes.OpsItineraryDetails.Days.Select(x => new Attributes() { Attribute_Id = x, AttributeName = x }).ToList();
				position.CityList = opsBookingGetRes.OpsItineraryDetails.Cities.Select(x => new Attributes() { Attribute_Id = x, AttributeName = x }).ToList();
				position.CityList.RemoveAll(x => string.IsNullOrWhiteSpace(x.AttributeName));
				#endregion

				#region Get Position Detail
				OpsBookingItineraryGetReq opsBookingGetReq1 = new OpsBookingItineraryGetReq() { BookingNumber = QRFID, ItineraryDetailId = ItineraryID };
				OpsBookingItineraryGetRes opsBookingGetRes2 = operationsProviders.GetItineraryBuilderPositionDetailById(opsBookingGetReq1, token).Result;
				position.StartTime = opsBookingGetRes2.ItineraryDetails.STARTTIME;
				position.EndTime = opsBookingGetRes2.ItineraryDetails.ENDTIME;
				position.ProductName = opsBookingGetRes2.ItineraryDetails.Description;
				position.City = opsBookingGetRes2.ItineraryDetails.CityName;
				position.PositionId = opsBookingGetRes2.ItineraryDetails.Position_Id;
				position.Day = "Day " + opsBookingGetRes2.ItineraryDetails.DayNo;
				#endregion

				if(action == "EditDesc") position.flag = true; else position.flag = false;
			}
			else
			{
				#region Get Itinerary Details by QRFId
				itineraryGetReq = new ItineraryGetReq();
				itineraryGetReq.QRFID = QRFID;
				itineraryGetRes = coProviders.GetItinerary(itineraryGetReq, token).Result;
				#endregion

				if (action == "Edit") position.flag = true; else position.flag = false;
				position.QRFID = QRFID;
				position.ItineraryID = ItineraryID;
				position.DayList = GetDays(itineraryGetRes.Itinerary.ItineraryDays);
				//position.CityList = GetCity(itineraryGetRes.Itinerary.ItineraryDays);			

				RoutingGetReq routingGetReq = new RoutingGetReq();
				routingGetReq.QRFID = QRFID;
				RoutingGetRes routingGetRes = objSalesProvider.getQRFRoutingDetails(routingGetReq, token).Result;

				position.CityList = GetCity(routingGetRes.RoutingInfo);

				//position.CityList = routingGetRes.RoutingInfo.Select(x => new Attributes { Attribute_Id = x.FromCityID,
				//	AttributeName = (!string.IsNullOrWhiteSpace(x.FromCityName) && x.FromCityName.Contains(',')) ? x.FromCityName.Split(',')[0] : x.FromCityName }).ToList();

				position.ItineraryDaysId = string.IsNullOrEmpty(ItineraryDaysId) ? position.DayList != null && position.DayList.Count > 0 ? position.DayList[0].Attribute_Id : "" : ItineraryDaysId;
				position.City = ItineraryDaysId;
				foreach (var k in itineraryGetRes.Itinerary.ItineraryDays)
				{
					if (k.ItineraryDaysId == ItineraryDaysId)
					{
						foreach (var p in k.ItineraryDescription)
						{
							if (p.PositionId == PositionId)
							{
								//var city = itineraryGetRes.Itinerary.ItineraryDays.Where(x => x.City == p.City).Select(y => y.ItineraryDaysId).FirstOrDefault();
								position.StartTime = p.StartTime;
								position.EndTime = p.EndTime;
								position.ProductName = p.ProductName;
								position.Day = k.ItineraryDaysId;
								position.City = p.City;
								position.PositionId = p.PositionId;
							}
						}
					}
					else
					{
						position.Day = ItineraryDaysId;
						//position.City = ItineraryDaysId;
					}
				}
			}
			return PartialView("_AddNewItineraryDetails", position);
		}

		public IActionResult SaveNewItineraryElement(ExtraPosition position)
		{
			//if (ModelState.IsValid)
			//{
			ItinerarySetRes response = new ItinerarySetRes();
			ItinerarySetReq request = new ItinerarySetReq();
			OpsBookingItinerarySetReq req = new OpsBookingItinerarySetReq();
			OpsBookingItinerarySetRes res = new OpsBookingItinerarySetRes();


			if (position.flag == true)
			{
				string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;

				if (string.IsNullOrEmpty(position.PositionId))
					request.IsExtraItineraryElement = true;
				else
					request.IsExtraItineraryElement = false;

				if(string.IsNullOrWhiteSpace(position.ProductName))
					return Json(new { responseText = "Please Enter Description", status="error" });

				request.itinerary.QRFID = position.QRFID;
				request.itinerary.ItineraryID = position.ItineraryID;
				request.itinerary.ItineraryDays.Add(new ItineraryDaysInfo
				{
					ItineraryDaysId = position.ItineraryDaysId
				});

				request.itinerary.ItineraryDays[0].ItineraryDescription.Add(new ItineraryDescriptionInfo
				{
					ProductType = "",
					ProductName = position.ProductName,
					PositionId = position.PositionId
				});
				request.itinerary.EditUser = UserName;
				request.itinerary.CreateUser = UserName;
				response = coProviders.SetItinerary(request, token).Result;


				if (response.ResponseStatus.Status.ToLower() == "success")
				{
					return Json(new { responseText = "Record Saved Successfully" });
				}
				else
				{
					return Content("Error");
				}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(position.ItineraryDaysId))
				{
					string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
					req.BookingNumber = position.QRFID;
					req.Type = "NewElement";
					req.ItineraryDetails = new ItineraryDetails()
					{
						ItineraryDetail_Id = position.ItineraryID,
						ProductType = "",
						CityName = position.City,
						STARTTIME = position.StartTime,
						ENDTIME = position.EndTime,
						Description = position.ProductName,
						DayNo = Convert.ToInt32(position.Day.Split(" ")[1])
					};
					res = operationsProviders.SetRemarksForItineraryBuilderDetails(req, token).Result;

					if (res.ResponseStatus.Status.ToLower() == "success")
					{
						return Json(new { responseText = "Record Saved Successfully", ItineraryDetailId = res.ItineraryDetailId });
					}
					else
					{
						return Content("Error");
					}
				}
				else
				{
					string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;

					if (string.IsNullOrEmpty(position.PositionId))
						request.IsExtraItineraryElement = true;
					else
						request.IsExtraItineraryElement = false;

					request.itinerary.QRFID = position.QRFID;
					request.itinerary.ItineraryID = position.ItineraryID;
					request.itinerary.ItineraryDays.Add(new ItineraryDaysInfo
					{
						ItineraryDaysId = position.ItineraryDaysId
					});

					request.itinerary.ItineraryDays[0].ItineraryDescription.Add(new ItineraryDescriptionInfo
					{
						ProductType = "",
						City = position.City,
						StartTime = position.StartTime,
						EndTime = position.EndTime,
						Type = "Extra",
						ProductName = position.ProductName,
						PositionId = position.PositionId
					});
					request.itinerary.EditUser = UserName;
					request.itinerary.CreateUser = UserName;
					response = coProviders.SetItinerary(request, token).Result;


					if (response.ResponseStatus.Status.ToLower() == "success")
					{
						return Json(new { responseText = "Record Saved Successfully" });
					}
					else
					{
						return Content("Error");
					}
				}
			}
			//}
		}
		#endregion

		#region Routing

		public IActionResult ProposalRouting()
		{
			return PartialView("_ProposalRouting");
		}

		#endregion

		#region Inclusions/Exclusions

		public IActionResult ProposalInclusionsExclusions(string QRFID)
		{
			ProposalInclusionsExclusionsViewModel model = new ProposalInclusionsExclusionsViewModel();
			List<App.ViewModels.TermsAndConditions> lstInclusions = new List<App.ViewModels.TermsAndConditions>();
			List<App.ViewModels.TermsAndConditions> lstExclusions = new List<App.ViewModels.TermsAndConditions>();

			#region Get Proposal Inclusions by QRFId
			proposalGetReq = new ProposalGetReq();
			proposalGetReq.QRFID = QRFID;
			proposalGetReq.Section = "Inclusions";
			proposalGetRes = coProviders.GetProposal(proposalGetReq, token).Result;

			if (!string.IsNullOrEmpty(proposalGetRes.Proposal.Inclusions))
			{
				model.Inclusions = proposalGetRes.Proposal.Inclusions;
			}
			else
			{
				foreach (var p in proposalGetRes.TermsAndConditions)
				{
					lstInclusions.Add(new App.ViewModels.TermsAndConditions { Section = p.Section, Tcs = p.Tcs, OrderNumber = p.OrderNr });
				}
			}
			#endregion

			#region Get Proposal Exclusions by QRFId
			proposalGetReq = new ProposalGetReq();
			proposalGetReq.QRFID = QRFID;
			proposalGetReq.Section = "Exclusions";
			proposalGetRes = coProviders.GetProposal(proposalGetReq, token).Result;

			if (!string.IsNullOrEmpty(proposalGetRes.Proposal.Inclusions))
			{
				model.Exclusions = proposalGetRes.Proposal.Exclusions;
			}
			else
			{
				foreach (var p in proposalGetRes.TermsAndConditions)
				{
					lstExclusions.Add(new App.ViewModels.TermsAndConditions { Section = p.Section, Tcs = p.Tcs, OrderNumber = p.OrderNr });
				}
			}
			#endregion

			#region Inclusion/Exclusion binding      
			model.InclusionList = lstInclusions;
			model.ExclusionList = lstExclusions;
			#endregion

			return PartialView("_ProposalInclusionsExclusions", model);
		}

		#endregion

		[HttpPost]
		public IActionResult SaveProposalInclusionExclusions(string QRFID, string ProposalId, string txtInclusionExclusions)
		{
			string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
			#region Set Proposal by Proposal Id
			ProposalSetReq request = new ProposalSetReq();
			ProposalSetRes response = new ProposalSetRes();
			request.Proposal.QRFID = QRFID;
			request.Proposal.ProposalId = ProposalId;

			if (txtInclusionExclusions != null && !txtInclusionExclusions.Contains("<hr />") && txtInclusionExclusions.Contains("INCLUSIONS"))
			{
				request.Proposal.Inclusions = txtInclusionExclusions;
				request.Proposal.Exclusions = "";
			}
			else if (txtInclusionExclusions != null && !txtInclusionExclusions.Contains("<hr />") && txtInclusionExclusions.Contains("EXCLUSIONS"))
			{
				request.Proposal.Inclusions = "";
				request.Proposal.Exclusions = txtInclusionExclusions;
			}
			else if (txtInclusionExclusions != null && txtInclusionExclusions.Contains("<hr />"))
			{
				request.Proposal.Inclusions = txtInclusionExclusions.Split("<hr />")[0].ToString() == null ? "" : txtInclusionExclusions.Split("<hr />")[0].ToString();
				request.Proposal.Exclusions = txtInclusionExclusions.Split("<hr />")[1].ToString() == null ? "" : txtInclusionExclusions.Split("<hr />")[1].ToString();
			}
			else
			{
				request.Proposal.Inclusions = "";
				request.Proposal.Exclusions = "";
			}
			request.Proposal.EditUser = UserName;
			request.Proposal.CreateUser = UserName;
			response = coProviders.SetProposal(request, token).Result;
			#endregion

			if (response.ResponseStatus.Status.ToLower() == "success")
			{
				return Json(new { responseText = "success" });
			}
			else
			{
				return Content("Error");
			}
		}

		#endregion

		#region Terms

		public IActionResult ProposalTerms(string QRFID)
		{
			ProposalTermsViewModel model = new ProposalTermsViewModel();
			List<App.ViewModels.TermsAndConditions> lstTerms = new List<App.ViewModels.TermsAndConditions>();

			#region Get Proposal Terms by QRFId
			proposalGetReq = new ProposalGetReq();
			proposalGetReq.QRFID = QRFID;
			proposalGetReq.DocType = "QUOTATION-NEW";
			proposalGetReq.Section = "General";
			proposalGetRes = coProviders.GetProposal(proposalGetReq, token).Result;
			model.Terms = proposalGetRes.Proposal.Terms;
			model.ProposalIncludeRegions = proposalGetRes.Proposal.ProposalIncludeRegions ?? new ProposalIncludeRegions();
			#endregion

			#region Inclusion/Exclusion binding
			foreach (var p in proposalGetRes.TermsAndConditions)
			{
				lstTerms.Add(new App.ViewModels.TermsAndConditions { Section = p.Section, Tcs = p.Tcs, OrderNumber = p.OrderNr });
			}
			model.TermsList = lstTerms;
			#endregion

			return PartialView("_ProposalTerms", model);
		}

		[HttpPost]
		public IActionResult SaveProposalTerms(string QRFID, string ProposalId, string txtTerms, ProposalIncludeRegions ProposalIncludeRegions)
		{
			string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
			#region Set Proposal by Proposal Id
			ProposalSetReq request = new ProposalSetReq();
			ProposalSetRes response = new ProposalSetRes();
			request.Proposal.QRFID = QRFID;
			request.Proposal.ProposalId = ProposalId;
			request.Proposal.Terms = txtTerms ?? "";
			request.Proposal.ProposalIncludeRegions = ProposalIncludeRegions;
			request.Proposal.EditUser = UserName;
			request.Proposal.CreateUser = UserName;
			response = coProviders.SetProposal(request, token).Result;
			#endregion

			if (response.ResponseStatus.Status.ToLower() == "success")
			{
				return Json(new { responseText = "success" });
			}
			else
			{
				return Content("Error");
			}
		}

		#endregion

		#region Covering Note

		public IActionResult ProposalCoveringNote(string QRFID)
		{
			ProposalCoveringNoteViewModel model = new ProposalCoveringNoteViewModel();
			#region Get Proposal Details by QRFId
			proposalGetReq = new ProposalGetReq();
			proposalGetReq.QRFID = QRFID;
			proposalGetRes = coProviders.GetProposal(proposalGetReq, token).Result;
			//model.CoveringNote = proposalGetRes.Proposal.CoveringNote;
			model.ProposalIncludeRegions = proposalGetRes.Proposal.ProposalIncludeRegions;
			model.CoveringNote = model.CoveringNote != null ? ReplaceCoveringNoteTableOfContents(proposalGetRes.Proposal.CoveringNote, proposalGetRes.Proposal.ProposalIncludeRegions) : null;
			#endregion

			#region Get Quote Info By QRFId
			SalesProviders objSalesProvider = new SalesProviders(_configuration);
			QuoteAgentGetReq objQRFAgentRequest = new QuoteAgentGetReq() { QRFID = QRFID };
			QuoteAgentGetRes objQRFAgentResponse = objSalesProvider.GetQRFAgentByQRFID(objQRFAgentRequest, token).Result;
			QuoteAgentGetProperties objResult = objQRFAgentResponse.QuoteAgentGetProperties;

			model.QRFID = QRFID;
			model.CompanyName = !string.IsNullOrEmpty(objQRFAgentResponse.QuoteAgentGetProperties.SalesPersonCompany) ? objQRFAgentResponse.QuoteAgentGetProperties.SalesPersonCompany : "";
			model.Destination = !string.IsNullOrEmpty(objResult.AgentProductInfo.Destination) && objResult.AgentProductInfo.Destination.Contains('|') ? objResult.AgentProductInfo.Destination.Split('|')[1] : "";
			model.SalesPersonUserName = !string.IsNullOrEmpty(objQRFAgentResponse.QuoteAgentGetProperties.SalesPersonUserName) ? objQRFAgentResponse.QuoteAgentGetProperties.SalesPersonUserName : "";
			#endregion

			#region Agent Person Binding
			var agentContactPersonId = objResult.AgentInfo.ContactPersonID;
			AgentContactDetailsReq req = new AgentContactDetailsReq() { VoyagerContact_Id = agentContactPersonId };
			AgentContactDetailsRes res = objSalesProvider.GetContactDetailsByContactID(req, token).Result;

			model.AgentTitle = res.AgentContactDetailsProperties != null && !string.IsNullOrEmpty(res.AgentContactDetailsProperties.CommonTitle) ? res.AgentContactDetailsProperties.CommonTitle : "";
			model.AgentFirstName = res.AgentContactDetailsProperties != null && !string.IsNullOrEmpty(res.AgentContactDetailsProperties.FirstName) ? res.AgentContactDetailsProperties.FirstName : "";
			model.AgentLastName = res.AgentContactDetailsProperties != null && !string.IsNullOrEmpty(res.AgentContactDetailsProperties.LastName) ? res.AgentContactDetailsProperties.LastName : "";
			#endregion

			#region Sales Person Details Binding
			var salesPersonContactEmail = objQRFAgentResponse.QuoteAgentGetProperties.SalesPerson;
			ContactDetailsRequest req1 = new ContactDetailsRequest() { Email = salesPersonContactEmail };
			ContactDetailsResponse res1 = loginProviders.GetContactDetails(req1, token).Result;

			model.SalesPersonCommonTitle = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.CommonTitle) ? res1.Contacts.CommonTitle : "";
			model.SalesPersonTitle = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.TITLE) ? res1.Contacts.TITLE : "";
			model.SalesPersonFullName = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.FIRSTNAME) && !string.IsNullOrEmpty(res1.Contacts.LastNAME) ? res1.Contacts.FIRSTNAME + " " + res1.Contacts.LastNAME : objQRFAgentResponse.QuoteAgentGetProperties.SalesPersonUserName;
			model.SalesPersonEmail = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.MAIL) ? res1.Contacts.MAIL : salesPersonContactEmail;
			model.SalesPersonFax = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.FAX) ? res1.Contacts.FAX : "";
			model.SalesPersonPhone = res1.Contacts != null && !string.IsNullOrEmpty(res1.Contacts.MOBILE) ? res1.Contacts.MOBILE : "";

			#endregion

			return PartialView("_ProposalCoveringNote", model);
		}

		private string ReplaceCoveringNoteTableOfContents(string CoveringNote, ProposalIncludeRegions Regions)
		{
			StringBuilder builder = new StringBuilder();
			string response, txtReplace, txtSearch = "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"table\" style=\"width:773px\">";
			string tdFormat = "\n\t\t\t<td><img alt=\"Blue Correct Icon\" src=\"/resources/images/proposal-report/correct-blue-icon.jpg\" /></td>\n\t\t\t<td>{0}</td>";
			int startIndex = CoveringNote.IndexOf(txtSearch);
			if (startIndex > -1)
			{
				txtReplace = CoveringNote.Substring(startIndex, (CoveringNote.IndexOf("</table>", startIndex) - startIndex + 8));

				builder.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"table\" style=\"width:773px\">\n\t<tbody>\n\t\t<tr>");
				builder.AppendFormat(tdFormat, "Departure&nbsp;Dates&nbsp;/&nbsp;Costs");
				builder.AppendFormat(tdFormat, "Inclusions&nbsp;/&nbsp;Exclusions&nbsp;/&nbsp;Notes");
				if (Regions.RegionMap)
				{
					builder.AppendFormat(tdFormat, "Map&nbsp;of&nbsp;the&nbsp;Region");
				}
				builder.Append("\n\t\t</tr>\n\t\t<tr>");
				if (Regions.SuggestedHotels)
				{
					builder.AppendFormat(tdFormat, "Information&nbsp;on&nbsp;the&nbsp;suggested&nbsp;hotel&nbsp;list");
				}
				if (Regions.ItineraryBrief)
				{
					builder.AppendFormat(tdFormat, "Your&nbsp;Itinerary&nbsp;in&nbsp;Brief");
				}
				if (Regions.ItineraryDetail)
				{
					builder.AppendFormat(tdFormat, "Your&nbsp;Itinerary&nbsp;in&nbsp;Detail");
				}
				builder.Append("\n\t\t</tr>\n\t</tbody>\n</table>");

				response = CoveringNote.Replace(txtReplace, builder.ToString());
			}
			else
			{
				response = CoveringNote;
			}
			return response;
		}

		[HttpPost]
		public IActionResult SaveProposalCoveringNote(string QRFID, string ProposalId, string txtCoveringNote)
		{
			string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
			#region Set Proposal by Proposal Id
			ProposalSetReq request = new ProposalSetReq();
			ProposalSetRes response = new ProposalSetRes();
			request.Proposal.QRFID = QRFID;
			request.Proposal.ProposalId = ProposalId;
			request.Proposal.CoveringNote = txtCoveringNote ?? "";
			request.Proposal.EditUser = UserName;
			request.Proposal.CreateUser = UserName;
            request.QRFID = QRFID;
            request.VoyagerUserId = ckLoginUser_Id;
			response = coProviders.SetProposal(request, token).Result;
			#endregion

			if (response.ResponseStatus.Status.ToLower() == "success")
			{
				return Json(new { responseText = "success" });
			}
			else
			{
				return Content("Error");
			}
		}

		#endregion

		#region Review

		public IActionResult ProposalReview(string QRFID)
		{
			NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = QRFID };
			string VirtualPath = _configuration.GetValue<string>("SystemSettings:ProposalFilesVirtualPath");
			cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
			string pdfPath = "/" + VirtualPath + "/" + ProposalDocumentController.FormatFileName(modelQuote.COHeaderViewModel.TourName) + ".pdf";
			return PartialView("_ProposalReview", pdfPath);
		}

		#endregion

		#region Helper Methods
		public List<Attributes> GetDays(List<ItineraryDaysInfo> itineraryDays)
		{
			List<Attributes> lstDays = new List<Attributes>();
			foreach (var day in itineraryDays)
			{
				lstDays.Add(new Attributes { Attribute_Id = day.ItineraryDaysId, AttributeName = day.Day });
			}
			return lstDays;
		}

		//public List<Attributes> GetCity(List<ItineraryDaysInfo> itineraryDays)
		//{
		//	List<Attributes> lstCities = new List<Attributes>();
		//	foreach (var day in itineraryDays)
		//	{
		//		if (lstCities.Where(x => x.AttributeName == day.City).Count() <= 0)
		//		{
		//			lstCities.Add(new Attributes { Attribute_Id = day.ItineraryDaysId, AttributeName = day.City });
		//		}
		//	}
		//	return lstCities;
		//}

		public List<Attributes> GetCity(List<RoutingInfo> routingInfo)
		{
			List<Attributes> lstCities = new List<Attributes>();

			//Get all From city name
			foreach (var day in routingInfo)
			{
				day.FromCityName = (!string.IsNullOrWhiteSpace(day.FromCityName) && day.FromCityName.Contains(',')) ? day.FromCityName.Split(',')[0] : day.FromCityName;
				if (lstCities.Where(x => x.AttributeName == day.FromCityName).Count() <= 0)
				{
					lstCities.Add(new Attributes { Attribute_Id = day.FromCityID, AttributeName = day.FromCityName });
				}
			}

			//Get all To city name
			foreach (var day in routingInfo)
			{
				day.ToCityName = (!string.IsNullOrWhiteSpace(day.ToCityName) && day.ToCityName.Contains(',')) ? day.ToCityName.Split(',')[0] : day.ToCityName;
				if (lstCities.Where(x => x.AttributeName == day.ToCityName).Count() <= 0)
				{
					lstCities.Add(new Attributes { Attribute_Id = day.ToCityID, AttributeName = day.ToCityName });
				}
			}

			return lstCities.OrderBy(x => x.AttributeName).ToList();
		}
		#endregion
	}
}