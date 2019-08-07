using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class ProposalDocumentController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IViewRenderService _viewRenderService;
        public COCommonLibrary COCommonLibrary;

        public ProposalDocumentController(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IViewRenderService viewRenderService) : base(configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            COCommonLibrary = new COCommonLibrary(configuration);
            _viewRenderService = viewRenderService;
        }
        #endregion

        #region Proposal Document Actions
        public IActionResult ProposalDocument()
        {
            return View(ProposalDocumentData());
        }

        public IActionResult _ProposalDocumentHeader()
        {
            string QRFID = Request.Query["QRFId"];
            COHeaderViewModel model = new COHeaderViewModel();
            if (!string.IsNullOrEmpty(QRFID))
            {
                model = COCommonLibrary.GetProposalDocumentHeaderDetails(QRFID, Request, Response, token);
                model.URLinitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            }
            return View(model);
        }

        public IActionResult _ProposalDocumentFooter()
        {
            string QRFID = Request.Query["QRFId"];
            COHeaderViewModel model = new COHeaderViewModel();
            if (!string.IsNullOrEmpty(QRFID))
            {
                model = COCommonLibrary.GetProposalDocumentHeaderDetails(QRFID, Request, Response, token);
                model.URLinitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            }
            return View(model);
        }

        public IActionResult GenerateProposal()
        {
            try
            {
                string QRFID = Request.Query["QRFId"];
                string urlInitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
                string PageURL = urlInitial + "/ProposalDocument/ProposalDocument?QRFId=" + QRFID;
                string HeaderURL = urlInitial + "/ProposalDocument/_ProposalDocumentHeader?QRFId=" + QRFID;
                string FooterUrl = urlInitial + "/ProposalDocument/_ProposalDocumentFooter?QRFId=" + QRFID;
                if (!string.IsNullOrEmpty(QRFID))
                {
                    NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = QRFID };
                    COCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);

                    string filename = FormatFileName(modelQuote.COHeaderViewModel.TourName) + ".pdf";
                    string PDFPath = _configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath");
                    string FullPDFPath = Path.Combine(PDFPath, filename);
                    if (!Directory.Exists(PDFPath)) Directory.CreateDirectory(PDFPath);

                    PdfConvert._configuration = _configuration;
                    PdfConvert.GenerateDocument(PageURL, HeaderURL, FooterUrl, FullPDFPath, token, HttpContext.Request.Cookies);

                    if (filename == null)
                        return Content("filename not present");

                    #region Update ValidForAcceptance field in mQuote and mQRFPrice collection
                    ResponseStatus objResponseStatus = COCommonLibrary.UpdateValidForAcceptance(_configuration, token, QRFID, ckUserEmailId);
                    #endregion

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(FullPDFPath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;
                    return File(memory, PdfConvert.GetContentType(FullPDFPath), Path.GetFileName(FullPDFPath));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
                return null;
            }
        }

        public ProposalDocumentViewModel ProposalDocumentData()
        {
            string QRFID = Request.Query["QRFId"];
            ProposalDocumentViewModel model = new ProposalDocumentViewModel();
            if (!string.IsNullOrEmpty(QRFID))
            {
                try
                {
                    #region fetching all data from service
                    COProviders objCOProvider = new COProviders(_configuration);
                    QuoteAgentGetReq objQRFAgentRequest = new QuoteAgentGetReq() { QRFID = QRFID };
                    ProposalDocumentGetRes objProposalRes = objCOProvider.GetProposalDocumentDetailsByQRFID(objQRFAgentRequest, token).Result;
                    model.COHeaderViewModel = COCommonLibrary.GetProposalDocumentHeaderDetails(QRFID, Request, Response, token);
                    model.Itinerary = objProposalRes.Itinerary;
                    model.Proposal = objProposalRes.Proposal;
                    model.QRFQuote = objProposalRes.QRFQuote;
                    model.ProductImages = objProposalRes.ProductImages;
                    model.GenericImages = objProposalRes.GenericImages;
                    model.CountryImageInitial = _configuration.GetValue<string>("SystemSettings:CountryImageInitial");
                    model.URLinitial = model.COHeaderViewModel.URLinitial = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
                    model.RoutingCities.AddRange(objProposalRes.Itinerary.ItineraryDays.Select(a => a.City + "," + a.Country));
                    model.RoutingCities.AddRange(objProposalRes.Itinerary.ItineraryDays.Select(a => a.ToCityName + "," + a.ToCountryName));
                    model.RoutingCities = model.RoutingCities.Distinct().ToList();
                    model.RoutingCities.RemoveAll(a => string.IsNullOrEmpty(a) || a == ",");
                    model.Itinerary.ItineraryDays.ForEach(a => a.Hotel = a.Hotel.Where(b => b.IsDeleted == false).ToList());
					model.Itinerary.ItineraryDays.ForEach(a => a.Meal = a.Meal.Where(b => b.IsDeleted == false).ToList());

					if (model.Proposal != null && model.Proposal.ProposalIncludeRegions == null)
                        model.Proposal.ProposalIncludeRegions = new ProposalIncludeRegions();
                    #endregion

                    #region creating dates list for date range
                    DateTime date; string month;
                    var list = new List<ProposalDepartDate>();
                    foreach (var item in model.QRFQuote.Departures.OrderBy(a => a.Date))
                    {
                        date = Convert.ToDateTime(item.Date);
                        month = date.ToString("MMMM");
                        list.Add(new ProposalDepartDate { Day = date.Day.ToString(), Month = month, Year = date.Year.ToString() });
                    }
                    var list2 = new List<ProposalDepartDate>();
                    foreach (var item in list)
                    {
                        month = item.Month + " " + item.Year;
                        if (list2.Where(a => a.Month == month).Count() > 0)
                        {
                            list2.Where(a => a.Month == month).FirstOrDefault().Day += (", " + item.Day);
                        }
                        else
                        {
                            list2.Add(new ProposalDepartDate { Month = month, Day = item.Day });
                        }
                    }
                    model.DatesList = list2;
                    #endregion

                    #region google maps section
                    StaticMap._configuration = _configuration;
                    //GeocoderLocation geocoder = StaticMap.Locate(model.QRFQuote.AgentProductInfo.Destination.Split('|')[1]);
                    string mapURL = "https://maps.googleapis.com/maps/api/staticmap?center={0}&zoom=3&size=233x206&maptype=roadmap&key=GoogleAPIKey&markers=color:red|label:AA|{0}";
                    mapURL = string.Format(mapURL, model.QRFQuote.AgentProductInfo.Destination.Split('|')[1]);
                    if (StaticMap.RenderImage(mapURL, QRFID + "_ProposalDocument_smallmap.png", out string output))
                    {
                        model.SmallMapURL = output;
                    }

                    mapURL = "https://maps.googleapis.com/maps/api/staticmap?size=773x682&maptype=roadmap&key=GoogleAPIKey";
                    foreach (string city in model.RoutingCities)
                    {
                        //geocoder = StaticMap.Locate(city);
                        mapURL += "&markers=color:red|label:AA|{0}";
                        mapURL = string.Format(mapURL, city);
                    }
                    if (StaticMap.RenderImage(mapURL, QRFID + "_ProposalDocument_bigmap.png", out output))
                    {
                        model.BigMapURL = output;
                    }

                    string ProdId = "";
                    for (int i = 0; i < model.Itinerary.ItineraryDays.Count; i++)
                    {
                        for (int j = 0; j < model.Itinerary.ItineraryDays[i].Hotel.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(model.Itinerary.ItineraryDays[i].Hotel[j].Lat) && !string.IsNullOrEmpty(model.Itinerary.ItineraryDays[i].Hotel[j].Long)
                                && model.Itinerary.ItineraryDays[i].Hotel[j].IsDeleted == false)
                            {
                                ProdId = !string.IsNullOrEmpty(model.Itinerary.ItineraryDays[i].Hotel[j].HotelId) ? model.Itinerary.ItineraryDays[i].Hotel[j].HotelId : model.Itinerary.ItineraryDays[i].Hotel[j].PositionId;
                                mapURL = "https://maps.googleapis.com/maps/api/staticmap?zoom=10&size=284x191&maptype=roadmap&key=GoogleAPIKey";
                                mapURL += "&markers=color:red|label:AA|{0},{1}";
                                mapURL = string.Format(mapURL, model.Itinerary.ItineraryDays[i].Hotel[j].Lat, model.Itinerary.ItineraryDays[i].Hotel[j].Long);
                                if (StaticMap.RenderImage(mapURL, ProdId + "_ProposalDocument_hotelmap.png", out output))
                                {
                                    model.Itinerary.ItineraryDays[i].Hotel[j].HotelMapURL = output;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Proposal Terms
                    string templatePath = _configuration.GetValue<string>("Pages:ProposalTermsTemplate");
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), templatePath);
                    var builder = new StringBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.Append(SourceReader.ReadToEnd());
                        model.Proposal.Terms += Environment.NewLine + builder.ToString();
                    }
                    #endregion

                    #region fetching hotel images from expedia

                    string HotelMapImage;
                    List<ArrProductResources> HotelRes = new List<ArrProductResources>();
                    foreach (var days in model.Itinerary.ItineraryDays)
                    {
                        foreach (var hotel in days.Hotel)
                        {
                            if (hotel.ProdResources != null)
                            {
                                HotelRes = hotel.ProdResources.Where(a => a.ResourceType == "Image").OrderBy(a => a.OrderNr).ToList();
                            }
                            if (hotel.ProdResources == null || HotelRes == null || HotelRes.Count < 1 || (HotelRes.Count > 0 && string.IsNullOrEmpty(HotelRes[0].ImageSRC)))
                            {
                                if (StaticMap.RenderExpediaImage(hotel.HotelCode, ProdId + "_ProposalDocument_hotelimage.png", out output))
                                {
                                    HotelMapImage = output;
                                }
                            }
                        }
                    }

                    #endregion

                    #region replacing image urls from resources to ImageResources

                    foreach (var ItineraryDay in model.Itinerary.ItineraryDays)
                    {
                        if (ItineraryDay.Hotel != null)
                        {
                            foreach (var Hotel in ItineraryDay.Hotel)
                            {
                                if (Hotel.ProdResources != null)
                                {
                                    foreach (var ProdResource in Hotel.ProdResources)
                                    {
                                        if (ProdResource.ImageSRC != null) ProdResource.ImageSRC = ProdResource.ImageSRC.Replace("resources/", "ImageResources/");
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Get Hotel Summary details from Itinerary

                    ProposalGetReq req = new ProposalGetReq();
                    ProposalGetRes res = new ProposalGetRes();
                    req.QRFID = QRFID;
                    res = objCOProvider.GetHotelSummaryByQrfId(req, token).Result;
                    var hotellist = res.Hotels;
                    model.HotelList = hotellist.Select(x => new Hotel { HotelName = x.HotelName, Location = string.IsNullOrWhiteSpace(x.Location) ? "" : x.Location.Split(',')[0], Stars = x.Stars, Duration = x.Duration }).ToList();

                    #endregion
                }
                catch (Exception ex)
                {
                    throw;
                    //Console.WriteLine(ex.Message);
                }
            }
            return model;
        }
        #endregion

        #region Action to generate html for pages/ Not in use as of now/ Do not delete
        public IActionResult ProposalDocumenthtml()
        {
            var result = _viewRenderService.RenderToStringAsync("ProposalDocument/ProposalDocument", ProposalDocumentData());
            return Content(result.Result);
        }

        public IActionResult ProposalDocumentHeader()
        {
            var result = _viewRenderService.RenderToStringAsync("ProposalDocument/_ProposalDocumentHeader", ProposalDocumentData().COHeaderViewModel);
            //Response.Headers.Add("content-disposition", "attachment; filename=~/htmltemplates/Proposal/ProposalIndex.html");
            //Response.ContentType = "application/html";
            return Content(result.Result);
        }

        public IActionResult ProposalDocumentFooter()
        {
            var result = _viewRenderService.RenderToStringAsync("ProposalDocument/_ProposalDocumentFooter", ProposalDocumentData().COHeaderViewModel);
            return Content(result.Result);
        }
        #endregion

        #region Helper Methods
        public static string FormatFileName(string fileName)
        {
            return fileName.Replace("/", "").Replace("\\", "").Replace(":", "").Replace("*", "")
                        .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
        }
        #endregion
    }


}