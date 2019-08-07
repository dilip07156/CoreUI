using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class QuoteController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private SalesProviders salesProviders;
        private AgentProviders agentProviders;
        private string SessionName = "IntegrationInfo";

        public QuoteController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            salesProviders = new SalesProviders(_configuration);
            agentProviders = new AgentProviders(_configuration);
        }

        public IActionResult Quote()
        {
            QuoteSearchViewModel model = new QuoteSearchViewModel();
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model = quoteLibrary.BindMasterData(_configuration, token);
            ViewBag.SearchResult = false;
            return View(model);
        }

        #region Agent
        [HttpPost]
        public IActionResult Quote(QuoteSearchViewModel model)
        {
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            QuoteSearchViewModel modelMaster = new QuoteSearchViewModel();
            modelMaster = quoteLibrary.BindMasterData(_configuration, token);
            model.PipelineStagesList = modelMaster.PipelineStagesList;
            model.QuotePriorityList = modelMaster.QuotePriorityList;
            model.QuoteResultList = modelMaster.QuoteResultList;
            model.QuoteStatusList = modelMaster.QuoteStatusList;
            model.DateTypeList = modelMaster.DateTypeList;
            model.MonthList = modelMaster.MonthList;
            model.YearList = modelMaster.YearList;
            model.DivisionList = modelMaster.DivisionList;
            model.DestinationList = modelMaster.DestinationList;

            QuoteSearchReq objQuoteSearchReq = new QuoteSearchReq();
            objQuoteSearchReq.CurrentPipeline = model.PipelineStages;
            objQuoteSearchReq.AgentName = model.AgentName;
            objQuoteSearchReq.QRFID = model.CNKReferenceNo;
            objQuoteSearchReq.TourCode = model.AgentReferenceNo;
            objQuoteSearchReq.TourName = model.AgentTour;
            objQuoteSearchReq.QRFStatus = model.QuoteStatus;
            objQuoteSearchReq.QuoteResult = model.QuoteResult;
            objQuoteSearchReq.Priority = model.QuotePriority;
            objQuoteSearchReq.Date = model.DateType;
            objQuoteSearchReq.Division = model.Division;
            objQuoteSearchReq.Destination = model.Destination;
            objQuoteSearchReq.UserId = ckLoginUser_Id;

            if (string.IsNullOrEmpty(model.From))
                objQuoteSearchReq.From = null;
            else
            {
                var strFromDT = model.From.Split("/");
                if (strFromDT?.Count() >= 3)
                {
                    DateTime fromDT = new DateTime(Convert.ToInt32(strFromDT[2]), Convert.ToInt32(strFromDT[1]), Convert.ToInt32(strFromDT[0]));
                    objQuoteSearchReq.From = fromDT;
                }
                else
                {
                    objQuoteSearchReq.From = null;
                }
            }
            if (string.IsNullOrEmpty(model.To))
                objQuoteSearchReq.To = null;
            else
            {
                var strToDT = model.To.Split("/");
                if (strToDT?.Count() >= 3)
                {
                    DateTime toDT = new DateTime(Convert.ToInt32(strToDT[2]), Convert.ToInt32(strToDT[1]), Convert.ToInt32(strToDT[0]));
                    objQuoteSearchReq.To = toDT;
                }
                else
                {
                    objQuoteSearchReq.To = null;
                } 
            }

            objQuoteSearchReq.Month = model.Month;
            objQuoteSearchReq.Year = Convert.ToInt16(model.Year);

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            QuoteSearchRes objQuoteSearchRes = objSalesProvider.GetQRFEnquiryPipelineList(objQuoteSearchReq, token).Result;

            model.EnquiryPipeline = new EnquiryPipelineViewModel();
            model.EnquiryPipeline.QuoteSearchDetails = objQuoteSearchRes.QuoteSearchDetails;
            if (model.EnquiryPipeline.QuoteSearchDetails != null)
            {
                foreach (var i in model.EnquiryPipeline.QuoteSearchDetails)
                {
                    if (i.AgentPassengerInfo == null)
                        i.AgentPassengerInfo = new AgentPassengerInfo();
                    if (i.FollowUpItem == null)
                        i.FollowUpItem = new FollowUpItem();
                }
            }
            model.EnquiryPipeline.Status = objQuoteSearchRes.ResponseStatus.Status;
            model.EnquiryPipeline.TotalCount = objQuoteSearchRes.TotalCount;
            ViewBag.PipelineStages = model.PipelineStages;
            ViewBag.SearchResult = true;
            ViewBag.IsAllowToEdit = false;

            if (model.PipelineStages == "Quote Pipeline" || model.PipelineStages == "Agent Approval Pipeline" || model.PipelineStages == "Amendment Pipeline" || model.PipelineStages == "Handover Pipeline")
            {
                if (UserRoles.Contains("Sales Officer"))
                {
                    ViewBag.IsAllowToEdit = true;
                }
            }
            else if (model.PipelineStages == "Costing Pipeline")
            {
                if (UserRoles.Contains("Costing Officer"))
                {
                    ViewBag.IsAllowToEdit = true;
                }
            }
            else if (model.PipelineStages == "Costing Approval Pipeline")
            {
                if (UserRoles.Contains("Product Accountant"))
                {
                    ViewBag.IsAllowToEdit = true;
                }
            }

            return View(model);
        }

        public IActionResult NewQuote()
        {

            try
            {
                string QRFID = Request.Query["QRFId"];
                string SubStep = Convert.ToString(Request.Query["SubStep"]);
                string Module = Request.Query["Module"];
                string PartnerEntityCode = Request.Query["PartnerEntityCode"];
                string Application = Request.Query["Application"];
                string Operation = Request.Query["Operation"];
                string CompanyId = Request.Query["CompanyId"];
                string ContactId = Request.Query["ContactId"];

                if (!string.IsNullOrEmpty(SubStep) && SubStep == "Routing")
                {
                    NewQuoteViewModel model = GetRoutingDetails(QRFID);
                    return PartialView("_QuoteRouting", model.mdlQuoteRoutingViewModel);
                }
                else if (!string.IsNullOrEmpty(SubStep) && SubStep == "Margins")
                {
                    bool IsCostingMargin = Convert.ToBoolean(Request.Query["IsCostingMargin"]);
                    NewQuoteViewModel model = GetMarginDetails(QRFID, IsCostingMargin);
                    TempData["IsCostingMargin"] = IsCostingMargin;
                    return PartialView("_QuoteMargins", model.mdlQuoteMarginViewModel);
                }
                else
                {
                    NewQuoteViewModel model = new NewQuoteViewModel();
                    bool GetStatus = false;
                    model.mdlMenuViewModel.QRFID = QRFID;
                    model.mdlQuoteAgentInfoViewModel.Module = Module;
                    model.mdlQuoteAgentInfoViewModel.PartnerEntityCode = PartnerEntityCode;
                    model.mdlQuoteAgentInfoViewModel.Application = Application;
                    model.mdlQuoteAgentInfoViewModel.Operation = Operation;
                    SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                    #region Agent Info
                    if (!string.IsNullOrEmpty(QRFID))
                    {
                        QuoteThirdPartyGetReq modelreq = new QuoteThirdPartyGetReq();
                        model.QRFID = QRFID;
                        modelreq.QrfID = QRFID;
                        model.mdlQuoteDateRangeViewModel.QRFID = QRFID;
                        GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref model);

                        QuoteThirdPartyGetRes response = salesProviders.GetPartnerQuoteDetails(modelreq, token).Result;
                        if (response != null)
                        {
                            model.mdlQuoteAgentInfoViewModel.Module = response?.PartnerEntityName;
                            model.mdlQuoteAgentInfoViewModel.PartnerEntityCode = response?.PartnerEntityCode;
                            model.mdlQuoteAgentInfoViewModel.Application = response?.ApplicationName?.ToLower();
                        }
                    }
                    else if (!string.IsNullOrEmpty(ContactId) && !string.IsNullOrEmpty(CompanyId))
                    {
                        model.mdlQuoteAgentInfoViewModel.AgentId = CompanyId;
                        var agentInfo = agentProviders.GetAgentDetailedInfo(new AgentGetReq() { CompanyId = CompanyId }, token).Result;
                        model.mdlQuoteAgentInfoViewModel.AgentName = agentInfo.AgentDetails.Name;
                        model.mdlQuoteAgentInfoViewModel.ContactPerson = ContactId;
                        var contactInfo = agentInfo.AgentDetails.ContactDetails.Where(a => a.Contact_Id == ContactId).FirstOrDefault();
                        model.mdlQuoteAgentInfoViewModel.Email = contactInfo.MAIL;
                        model.mdlQuoteAgentInfoViewModel.MobileNo = contactInfo.MOBILE;
                    }

                    quoteLibrary.LoadQuoteAgentInfo(_configuration, token, ref model, ckUserEmailId, ckUserCompanyId);
                    #endregion

                    if (model.mdlQuoteAgentInfoViewModel.ContactPersonList.Count == 0)
                    {
                        model.mdlQuoteAgentInfoViewModel.MobileNo = "";
                        model.mdlQuoteAgentInfoViewModel.Email = "";
                    }

                    return View(model);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult NewQuote(NewQuoteViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult QuoteAgentInformation(QuoteAgentInfoViewModel model)
        {
            try
            {
                //string SubStep = "";
                NewQuoteViewModel modelQuote = new NewQuoteViewModel();
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                if (ModelState.IsValid)
                {
                    IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                    var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                    QuoteAgentSetRes objAgentResponse = quoteLibrary.SetQRFAgentDetails(_configuration, token, model, objCookies, sessionData);
                    if (objAgentResponse.ResponseStatus.Status.ToLower() == "success")
                    {
                        modelQuote.QRFID = objAgentResponse.QRFID;
                        TempData["success"] = "QRF details saved successfully";
                        //SubStep = "DateRange";
                        //if (!string.IsNullOrEmpty(SubStep) && SubStep == "DateRange")
                        TempData["QuoteSaved"] = true;
                    }
                    else
                    {
                        TempData["error"] = objAgentResponse.ResponseStatus.Status;
                        if (string.IsNullOrEmpty(modelQuote.QRFID)) modelQuote.QRFID = model.QRFID;
                    }
                }
                return RedirectToAction("NewQuote", new { modelQuote.QRFID });
            }
            catch (Exception ex)
            {
                return RedirectToAction("NewQuote", new { model.QRFID });
            }
        }

        public JsonResult GetAgentName(string term)
        {
            AgentCompanyReq objAgentRequest = new AgentCompanyReq();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objAgentRequest.AgentName = term;
            objAgentRequest.UserId = ckLoginUser_Id;
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            AgentCompanyRes objAgentResponse = objSalesProvider.GetAgentList(objAgentRequest, token).Result;
            List<AutoCompleteTextBox> agentList = objAgentResponse.AgentProperties.Select(data => new AutoCompleteTextBox { value = data.VoyagerCompany_Id, label = data.Name }).ToList();
            return Json(agentList);
        }

        public JsonResult GetAgentNameFrommCompanies(string term, string companyId, bool issupplier)
        {
            AgentCompanyReq objAgentRequest = new AgentCompanyReq();
            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            objAgentRequest.AgentName = term;
            objAgentRequest.CompanyId = companyId;
            objAgentRequest.UserId = ckLoginUser_Id;
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            AgentCompanyRes objAgentResponse = null;
            if (issupplier == true)
                objAgentResponse = objSalesProvider.GetSupplierListFrommCompanies(objAgentRequest, token).Result;
            else
                objAgentResponse = objSalesProvider.GetAgentListFrommCompanies(objAgentRequest, token).Result;

            List<AutoCompleteTextBox> agentList = objAgentResponse.AgentProperties.Select(data => new AutoCompleteTextBox { value = data.VoyagerCompany_Id, label = data.Name, code = data.Code }).ToList();
            return Json(agentList);
        }

        public JsonResult GetAgentContacts(string AgentId)
        {
            AgentContactReq objContactRequest = new AgentContactReq() { Company_Id = AgentId };
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            AgentContactRes objContactResponse = objSalesProvider.GetContactListForAgent(objContactRequest, token).Result;
            return Json(objContactResponse.ContactProperties);
        }

        public JsonResult CheckDuplicateQRFTourName(string TourName)
        {
            try
            {
                AgentContactReq objContactRequest = new AgentContactReq() { Company_Id = TourName };
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                AgentContactRes objContactResponse = objSalesProvider.CheckDuplicateQRFTourName(objContactRequest, token).Result;
                return Json(objContactResponse.TourNameFlag);
            }
            catch (Exception)
            {
                throw;
                return Json("invalid");
            }
        }

        public JsonResult GetContactDetailsByContactID(string ContactID)
        {
            AgentContactDetailsReq objContactRequest = new AgentContactDetailsReq() { VoyagerContact_Id = ContactID };
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            AgentContactDetailsRes objContactResponse = objSalesProvider.GetContactDetailsByContactID(objContactRequest, token).Result;
            return Json(objContactResponse.AgentContactDetailsProperties);
        }
        #endregion

        #region Pax Range
        [HttpPost]
        public IActionResult QuotePaxRange(QuotePaxRangeViewModel model)
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                //if (ModelState.IsValid)
                //{
                model.QuotePaxSlabDetails = model.QuotePaxSlabDetails.Where(a => a.PaxSlabId > 0 || (a.PaxSlabId == 0 && a.IsDeleted == false)).ToList();
                if (model.HotelsOption != "yes") { model.HotelCategoryList = null; model.HotelChainList = null; }

                PaxSetResponse departureSetResponse = quoteLibrary.SetPaxRangeDetails(_configuration, token, model, objCookies, sessionData);
                if (departureSetResponse.Status.ToLower() == "success") TempData["success"] = "Pax details saved successfully";
                else TempData["error"] = departureSetResponse.Status;
                //}
                return RedirectToAction("GetPaxRange", new { model.QRFID });
            }
            catch (Exception ex)
            {
                throw;
                TempData["error"] = ex.Message;
                return RedirectToAction("GetPaxRange", new { model.QRFID });
            }
        }

        public IActionResult GetPaxRange(string QRFID)
        {
            NewQuoteViewModel model = new NewQuoteViewModel();
            try
            {
                bool GetStatus = false;
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                if (!string.IsNullOrEmpty(QRFID))
                {
                    model.QRFID = QRFID;
                    model.mdlQuoteDateRangeViewModel.QRFID = QRFID;
                    GetStatus = quoteLibrary.GetQRFPaxSlabDetails(_configuration, token, ref model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_QuotePaxRange", model.mdlQuotePaxRangeViewModel);
        }

        public JsonResult GetHotelCategory(string term)
        {
            //List<AgentName> agentList = objAgentResponse.AgentProperties.Select(data => new AgentName { value = data.VoyagerCompany_Id, label = data.Name }).ToList();

            if (term.Length >= 3 && term.Substring(0, 3) == "###") term = "";
            ProdCatDefGetReq objProdCatDefGetReq = new ProdCatDefGetReq { Name = term };
            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProdCatDefGetRes objMasterTypeResponse = objMasterProviders.GetProdCatDefByName(objProdCatDefGetReq, token).Result;
            List<AutoCompleteTextBox> CatList = objMasterTypeResponse.ProdCatDefProperties.Select(data => new AutoCompleteTextBox { value = data.VoyagerDefProductCategoryId, label = data.Name }).ToList();
            return Json(CatList);
        }
        #endregion      

        #region DateRange
        public JsonResult GetDepartureDatesForQRFId(string QRFID, string SelectedDate)
        {
            //if (SelectedDate != null) SelectedDate = "04 Sep 2012";
            DepartureDateGetRequest objDepartureDatesReq = new DepartureDateGetRequest() { QRFID = QRFID, date = string.IsNullOrEmpty(SelectedDate) ? (DateTime?)null : Convert.ToDateTime(SelectedDate) };
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            DepartureDateGetResponse objDepartureDatesRes = objSalesProvider.GetDepartureDatesForQRFId(objDepartureDatesReq, token).Result;
            return Json(objDepartureDatesRes.DepartureDates);
        }

        [HttpPost]
        public IActionResult QuoteDateRange(QuoteDateRangeViewModel model)
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                if (ModelState.IsValid)
                {
                    IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                    var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                    model.QuoteSpecificDateViewModel = model.QuoteSpecificDateViewModel.Where(a => a.DepartureId > 0 || (a.DepartureId == 0 && a.IsDeleted == false)).ToList();
                    DepartureDateSetResponse departureSetResponse = quoteLibrary.SetQRFDepartureDetails(_configuration, token, model, objCookies, sessionData);
                    if (departureSetResponse.Status.ToLower() == "success") TempData["success"] = "Departure Dates saved successfully";
                    else TempData["error"] = departureSetResponse.Status;
                }
                return RedirectToAction("GetDateRange", new { model.QRFID });
            }
            catch (Exception ex)
            {
                throw;
                return RedirectToAction("GetDateRange", new { model.QRFID });
            }
        }

        public IActionResult GetDateRange(string QRFId)
        {
            NewQuoteViewModel model = new NewQuoteViewModel();
            try
            {
                #region Departure Dates
                model.mdlQuoteDateRangeViewModel.TravelDateType = "SpecificDates";
                model.mdlQuoteDateRangeViewModel.QuoteSpecificDateViewModel.Add(new QuoteSpecificDateViewModel() { DepartureId = -1, SelectedDate = DateTime.Today.ToString("dd MMM yyyy"), NoOfDepartures = 1, PaxPerDeparture = 1, Warning = "", IsDeleted = false });
                JsonResult json = GetDepartureDatesForQRFId(QRFId, null);
                DepartureDateGetResponse departureDateRes = new DepartureDateGetResponse { DepartureDates = (List<DepartureDates>)json.Value };
                if (departureDateRes != null && departureDateRes.DepartureDates != null && departureDateRes.DepartureDates.Count > 0)
                {
                    //model.mdlQuoteDateRangeViewModel.QuoteSpecificDateViewModel.Clear();
                    model.mdlQuoteDateRangeViewModel.QuoteSpecificDateViewModel.AddRange(departureDateRes.DepartureDates
                        .Select(a => new QuoteSpecificDateViewModel
                        {
                            DepartureId = a.Departure_Id,
                            IsDeleted = a.IsDeleted,
                            NoOfDepartures = a.NoOfDep,
                            PaxPerDeparture = a.PaxPerDep,
                            SelectedDate = a.Date == null ? "" : Convert.ToDateTime(a.Date).ToString("dd MMM yyyy"),
                            Warning = a.Warning
                        }));
                }
                #endregion

                model.QRFID = model.QRFID;
                model.mdlQuoteDateRangeViewModel.QuoteSpecificDateViewModel = model.mdlQuoteDateRangeViewModel.QuoteSpecificDateViewModel;
                model.mdlQuoteDateRangeViewModel.TravelDateType = model.mdlQuoteDateRangeViewModel.TravelDateType;

            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_QuoteDateRange", model.mdlQuoteDateRangeViewModel);
        }
        #endregion

        #region Routing 
        [HttpPost]
        public IActionResult QuoteRouting(QuoteRoutingViewModel model)
        {
            SalesQRFMapping salesQRFMapping = new SalesQRFMapping(_configuration);

            RoutingSetReq routingSetReq = new RoutingSetReq();
            model.SubStep = "Routing";
            routingSetReq = salesQRFMapping.QRFRoutingSet(model, ckUserEmailId);
            routingSetReq.VoyagerUserId = ckLoginUser_Id;

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            RoutingSetRes objQuoteSearchRes = objSalesProvider.SetQRFRoutingDetails(routingSetReq, token).Result;

            if (objQuoteSearchRes.ResponseStatus.Status.ToLower() == "success") TempData["success"] = "Routing details saved successfully";
            else TempData["error"] = objQuoteSearchRes.ResponseStatus.ErrorMessage;
            return RedirectToAction("NewQuote", new { QRFId = model.QRFID, SubStep = model.SubStep });
        }

        public IActionResult GetRouting(string QRFId)
        {
            NewQuoteViewModel model = GetRoutingDetails(QRFId);
            return PartialView("_QuoteRouting", model.mdlQuoteRoutingViewModel);
        }

        public NewQuoteViewModel GetRoutingDetails(string QRFID)
        {
            NewQuoteViewModel model = new NewQuoteViewModel();
            try
            {
                model.mdlQuoteRoutingViewModel = new QuoteRoutingViewModel();
                model.mdlQuoteRoutingViewModel.QRFID = QRFID;

                SalesQRFMapping salesQRFMapping = new SalesQRFMapping(_configuration);

                RoutingGetReq routingGetReq = new RoutingGetReq();
                routingGetReq.QRFID = QRFID;

                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                RoutingGetRes routingGetRes = objSalesProvider.getQRFRoutingDetails(routingGetReq, token).Result;

                model.mdlQuoteRoutingViewModel = salesQRFMapping.QRFRoutingGet(routingGetRes, token);

            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }
        #endregion

        #region Margin
        [HttpPost]
        public IActionResult QuoteMarginInformation(QuoteMarginViewModel model, bool IsCostingMargin = false)
        {
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model.SubStep = "Margins";
            MarginSetRes objMarginSetRes = quoteLibrary.SetQRFMarginDetails(_configuration, token, model, objCookies, sessionData, IsCostingMargin);

            if (objMarginSetRes.ResponseStatus.Status.ToLower() == "success")
            {
                TempData["success"] = objMarginSetRes.ResponseStatus.ErrorMessage;
            }
            else TempData["error"] = objMarginSetRes.ResponseStatus.ErrorMessage;

            return RedirectToAction("NewQuote", new { model.QRFID, SubStep = model.SubStep, IsCostingMargin = IsCostingMargin });
        }

        public JsonResult GetActiveCurrency(string term)
        {
            List<AutoCompleteTextBox> autoCompleteTextBox = new List<AutoCompleteTextBox>();
            if (term == "%")
            {
                autoCompleteTextBox.Add(new AutoCompleteTextBox { label = "%", value = "%" });
            }
            else
            {
                CurrencyGetReq objCurrencyGetReq = new CurrencyGetReq();
                if (term.Length >= 2 && term.Substring(0, 2) == "##") term = "";
                objCurrencyGetReq.CurrencyUnit = term;
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                CurrencyGetRes objCurrencyGetRes = objSalesProvider.GetActiveCurrencyList(objCurrencyGetReq, token).Result;
                autoCompleteTextBox = objCurrencyGetRes.Currency.Select(c => new AutoCompleteTextBox { value = c.CurrencyCode, label = c.CurrencyCode }).ToList();
            }

            if (autoCompleteTextBox != null && autoCompleteTextBox.Count > 0)
            {
                return Json(autoCompleteTextBox);
            }
            else
            {
                return Json("");
            }
        }

        public IActionResult GetMargin(string QRFID, bool IsCostingMargin = false)
        {
            NewQuoteViewModel model = GetMarginDetails(QRFID, IsCostingMargin);
            TempData["IsCostingMargin"] = IsCostingMargin;
            return PartialView("_QuoteMargins", model.mdlQuoteMarginViewModel);
        }

        public NewQuoteViewModel GetMarginDetails(string QRFID, bool IsCostingMargin = false)
        {
            NewQuoteViewModel model = new NewQuoteViewModel();
            try
            {
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                model.mdlQuoteMarginViewModel = new QuoteMarginViewModel();
                MarginGetRes marginGetRes = objSalesProvider.GetQRFMarginDetailsByQRFID(new MarginGetReq { QRFID = QRFID, IsCostingMargin = IsCostingMargin }, token).Result;

                SalesMarginMapping salesMarginMapping = new SalesMarginMapping();
                model.mdlQuoteMarginViewModel = salesMarginMapping.QRFMarginGet(marginGetRes);
                model.QRFID = QRFID;
            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }
        #endregion

        #region FOC tab
        [HttpPost]
        public IActionResult QuoteFOC(QuoteFOCViewModel model)
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                PaxSetResponse focSetResponse = quoteLibrary.SetFOCDetails(_configuration, token, model, objCookies, sessionData);
                if (focSetResponse.Status.ToLower() == "success") TempData["success"] = "FOC details saved successfully";
                else TempData["error"] = focSetResponse.Status;
                return RedirectToAction("GetFOC", new { model.QRFID });
            }
            catch (Exception ex)
            {
                throw;
                TempData["error"] = ex.Message;
                return RedirectToAction("GetFOC", new { model.QRFID });
            }
        }

        public IActionResult GetFOC(string QRFID)
        {
            NewQuoteViewModel model = new NewQuoteViewModel();
            try
            {
                bool GetStatus = false;
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                if (!string.IsNullOrEmpty(QRFID))
                {
                    model.QRFID = QRFID;
                    model.mdlQuoteFOCViewModel.QRFID = QRFID;
                    model.mdlQuoteFOCViewModel.StandardFoc = true;
                    GetStatus = quoteLibrary.GetFOCDetails(_configuration, token, ref model);
                    if (model.mdlQuoteFOCViewModel.QuoteFOCDetails != null && model.mdlQuoteFOCViewModel.QuoteFOCDetails.Count < 1)
                    {
                        model.mdlQuoteFOCViewModel.QuoteFOCDetails.Add(new QuoteFOCDetails { DateRange = "ALL", PaxSlab = "ALL", FOCSingle = 0, FOCTwin = 0, FOCDouble = 0, FOCTriple = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_QuoteFOC", model.mdlQuoteFOCViewModel);
        }
        #endregion

        public IActionResult GetLinkedQRFs(string QRFID)
        {
            try
            {
                LinkedQRFsGetRes response = new LinkedQRFsGetRes();
                LinkedQRFsGetReq request = new LinkedQRFsGetReq();
                request.QRFID = QRFID;
                response = salesProviders.GetLinkedQRFs(request, token).Result;
                ViewBag.CuttentQRFID = QRFID;
                ViewBag.UserRoles = UserRoles;
                return PartialView("_LinkedQRFs", response.LinkedQRFsDataList);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        #region Follow Up

        public IActionResult GetQuoteFollowUp(string QRFID)
        {
            try
            {
                FollowUpViewModel model = new FollowUpViewModel();
                FollowUpGetRes response = new FollowUpGetRes();
                FollowUpGetReq request = new FollowUpGetReq();
                request.QRFID = QRFID;
                response = salesProviders.GetFollowUpForQRF(request, token).Result;
                model.FollowUp = response.FollowUp;

                FollowUpMasterGetRes masterResponse = new FollowUpMasterGetRes();
                request.CompanyId = ckUserCompanyId;
                masterResponse = salesProviders.GetFollowUpMasterData(request, token).Result;

                model.FollowUpTaskList = masterResponse.FollowUpTaskList;
                model.InternalUserList = masterResponse.InternalUserList;
                model.ExternalUserList = masterResponse.ExternalUserList;

                model.InternalUserList.ForEach(a => { a.FIRSTNAME = a.CommonTitle + " " + a.FIRSTNAME + " " + a.LastNAME; a.Contact_Id = a.Contact_Id + "|" + a.MAIL; });
                model.ExternalUserList.ForEach(a => { a.FIRSTNAME = a.CommonTitle + " " + a.FIRSTNAME + " " + a.LastNAME; a.Contact_Id = a.Contact_Id + "|" + a.MAIL; });

                model.FollowUpDate = DateTime.Now;
                model.QRFID = QRFID;

                return PartialView("_FollowUp", model);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        [HttpPost]
        public JsonResult SetQuoteFollowUp(FollowUpViewModel model)
        {
            FollowUpSetRes response = new FollowUpSetRes();
            FollowUpSetReq request = new FollowUpSetReq();
            request.QRFID = model.QRFID;

            if (!string.IsNullOrEmpty(model.FollowUpTime))
            {
                var time = model.FollowUpTime.Split(":");
                model.FollowUpDate = model.FollowUpDate.AddHours(Convert.ToDouble(time[0])).AddMinutes(Convert.ToDouble(time[1]));
            }

            FollowUpTask task = new FollowUpTask();
            task.Task = model.FollowUpTask;
            task.FollowUpType = model.FollowUpType;
            task.FollowUpDateTime = model.FollowUpDate;

            task.FromName = ckUserName;
            task.FromContact_Id = ckUserContactId;
            task.FromEmail = ckUserEmailId;

            if (model.FollowUpType == "Internal")
            {
                task.ToName = model.InternalUserName;
                task.ToContact_Id = model.InternalUser.Split("|")[0];
                task.ToEmail = model.InternalUser.Split("|")[1];
            }
            else
            {
                task.ToName = model.ExternalUserName;
                task.ToContact_Id = model.ExternalUser.Split("|")[0];
                task.ToEmail = model.ExternalUser.Split("|")[1];
            }

            task.Status = "Requested";
            task.Notes = model.Notes;

            var FollowUpTaskList = new List<FollowUpTask>();
            FollowUpTaskList.Add(task);

            request.FollowUp.Add(new FollowUp
            {
                FollowUp_Id = Guid.NewGuid().ToString(),
                FollowUpTask = FollowUpTaskList,
                CreateUser = ckUserEmailId,
                CreateDate = DateTime.Now
            });

            response = salesProviders.SetFollowUpForQRF(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        [HttpPost]
        public JsonResult SendQuoteFollowUpMail(string QRFID, string FollowupId)
        {
            EmailGetRes response = new EmailGetRes();
            EmailGetReq request = new EmailGetReq();
            request.QrfId = QRFID;
            request.FollowUpId = FollowupId;
            request.PlacerUserId = ckLoginUser_Id;
            request.UserEmail = ckUserEmailId;

            response = salesProviders.SendQuoteFollowUpMail(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }
        #endregion

        [HttpPost]
        public JsonResult SetQuoteRejectOpportunity(string QRFID)
        {
            CommonResponse response = new CommonResponse();
            QuoteRejectOpportunityReq request = new QuoteRejectOpportunityReq();
            request.QRFID = QRFID;
            request.EditUser = ckUserEmailId;
            request.VoyagerUserId = ckLoginUser_Id;

            response = salesProviders.SetQuoteRejectOpportunity(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        #region Copy Quote
        public IActionResult GetCopyQuoteData(string QRFID)
        {
            try
            {
                CopyQuoteViewModel model = new CopyQuoteViewModel();
                GetQRFForCopyQuoteRes response = new GetQRFForCopyQuoteRes();
                QuoteAgentGetReq request = new QuoteAgentGetReq();
                request.QRFID = QRFID;
                response = salesProviders.GetQRFDataForCopyQuote(request, token).Result;

                if (response == null || response?.ResponseStatus?.Status == "Error")
                {
                    return PartialView("_CopyQuote", model);
                }

                model.QRFID = QRFID;
                model.TourName = response.TourName;
                model.AgentId = response.AgentInfo?.AgentID;
                model.AgentName = response.AgentInfo?.AgentName;
                model.ExisitingDepatures = response.ExisitingDepatures;
                model.CopyQuoteDepartures.Add(new CopyQuoteDeparturesNew() { NewDepartureDate = DateTime.Now.ToString("dd/MM/yyyy") });

                AgentContactReq objContactRequest = new AgentContactReq() { Company_Id = response.AgentInfo?.AgentID };
                AgentContactRes objContactResponse = salesProviders.GetContactListForAgent(objContactRequest, token).Result;
                model.ContactPersonList = objContactResponse.ContactProperties;
                model.ContactPerson = response.AgentInfo?.ContactPersonID;
                model.MobileNo = response.AgentInfo?.MobileNo;
                model.Email = response.AgentInfo?.EmailAddress;


                return PartialView("_CopyQuote", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public JsonResult SetCopyQuote(CopyQuoteViewModel model)
        {
            SetCopyQuoteRes response = new SetCopyQuoteRes();
            SetCopyQuoteReq request = new SetCopyQuoteReq();
            request.QRFID = model.QRFID;
            request.TourName = model.TourName;
            request.AgentId = model.AgentId;
            request.AgentName = model.AgentName;
            request.MobileNo = model.MobileNo;
            request.Email = model.Email;
            request.ContactPerson = model.ContactPerson;
            string[] strDT = new string[0];
            foreach (var item in model.CopyQuoteDepartures)
            {
                if (!string.IsNullOrEmpty(item.NewDepartureDate))
                {
                    strDT = item.NewDepartureDate.Split("/");
                    if (strDT.Count()>=3)
                    {
                        DateTime dtnew = new DateTime(Convert.ToInt32(strDT[2]), Convert.ToInt32(strDT[1]), Convert.ToInt32(strDT[0]));
                        request.CopyQuoteDepartures.Add(new CopyQuoteDepartures()
                        {
                            DepartureId = item.DepartureId,
                            NewDepartureDate = dtnew
                        });
                    }
                }                
            } 
            request.CreateUser = ckUserEmailId;
            request.VoyagerUserId = ckLoginUser_Id;

            response = salesProviders.SetCopyQuote(request, token).Result;

            if (response != null)
            {
                return Json(response);
            }
            return Json("failure");
        }
        #endregion

        #region 3rd party Search Quote Details

        public IActionResult GetPartnerQuoteDetails(QuoteThirdPartyGetReq model)
        {
            QuoteThirdPartyGetRes response = new QuoteThirdPartyGetRes();

            response = salesProviders.GetPartnerQuoteDetails(model, token).Result;

            if (response != null && !string.IsNullOrEmpty(response.QRFID) && !string.IsNullOrEmpty(response.CurrentPipeline))
            {
                if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:Quote"), true) == 0)
                {
                    return RedirectToAction("NewQuote", new { QRFID = response.QRFID, PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }
                else if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:Costing"), true) == 0)
                {
                    return RedirectToAction("Itinerary", "Itinerary", new { QRFID = response.QRFID , PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }
                else if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:CostingApproval"), true) == 0)
                {
                    return RedirectToAction("Itinerary", "Itinerary", new { QRFID = response.QRFID , PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }
                else if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:Amendment"), true) == 0)
                {
                    return RedirectToAction("Itinerary", "Amendment", new { QRFID = response.QRFID, PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }
                else if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:AgentApproval"), true) == 0)
                {
                    return RedirectToAction("Itinerary", "AgentApproval", new { QRFID = response.QRFID, PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }
                else if (string.Compare(response.CurrentPipeline, _configuration.GetValue<string>("PipeLines:Handover"), true) == 0)
                {
                    return RedirectToAction("Itinerary", "Handover", new { QRFID = response.QRFID , PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation });
                }

            }
            else
            {
                AgentThirdPartyGetRes responseNew = new AgentThirdPartyGetRes();
                responseNew = agentProviders.GetPartnerAgentContactDetails(new AgentThirdPartyGetReq() { Application = model.Application, PartnerEntityCode = model.PartnerEntityContactCode, PartnerEntityName = "" }, token).Result;
                if (responseNew != null)
                {
                    return RedirectToAction("NewQuote", new { PartnerEntityCode = model.PartnerEntityCode, Application = model.Application, Module = model.Module, Operation = model.Operation, ContactId = responseNew.ContactId, CompanyId = responseNew.CompanyId });
                }
                return View("IntegrationError");
            }

            return View("IntegrationError");

        }
        //public IActionResult SavePartnerQuoteDetails(QuoteThirdPartyGetReq model)
        //{

        //    if (!string.IsNullOrEmpty(model.PartnerEntityCode) && !string.IsNullOrEmpty(model.Application))
        //    {
        //        var Qrfid = salesProviders.GetPartnerQuoteDetails(model, token);
        //        return RedirectToAction("NewQuote", new { PartnerEntityCode = model.PartnerEntityCode, Application = model.Application,Module=model.Module ,Operation = model.Operation, QRFId = Qrfid });
        //    }
        //    return View("IntegrationError");

        //}

        #endregion
    }
}