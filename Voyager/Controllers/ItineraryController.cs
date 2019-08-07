using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class ItineraryController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        private MasterProviders objMasterProviders;
        private ItineraryGetReq itineraryGetReq;
        private ItineraryGetRes itineraryGetRes;
        private COProviders coProviders;

        public ItineraryController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            coProviders = new COProviders(_configuration);
            objMasterProviders = new MasterProviders(_configuration);
        }
        #endregion

        [Route("CostingOfficer/Itinerary")]
        public IActionResult Itinerary(string QRFID)
        {
            if (UserRoles.Contains("Product Accountant"))
            { }
            else if (!UserRoles.Contains("Costing Officer"))
            {
                return View("Unauthorize");
            }

            ItineraryViewModel model = new ItineraryViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Itinerary";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return View(model);
        }

        public IActionResult GetPricingSummary(string QRFID, long filterByDeparture = 0, long filterByPaxSlab = 0)
        {
            try
            {
                ItineraryViewModel model = new ItineraryViewModel();
                long defaultPaxSlabId = 0;
                #region Dropdown Binding
                QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
                QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
                QRFPaxSlabGetReq objPaxSlabReq = new QRFPaxSlabGetReq() { QRFID = QRFID };
                QRFPaxGetResponse objPaxSlabRes = coProviders.GetPaxSlabDetailsForCostingByQRF_Id(objPaxSlabReq, token).Result;
                model.PaxSlabList = (objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlab_Id.ToString(), Value = a.From + "-" + a.To }).ToList();
                model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
                if (model.PaxSlabList != null && model.PaxSlabList.Count > 0) { defaultPaxSlabId = Convert.ToInt32(model.PaxSlabList[0].AttributeValue_Id); }
                #endregion

                #region Get Costsheet by QRFId
                CostsheetGetReq request = new CostsheetGetReq();
                CostsheetGetRes response = new CostsheetGetRes();
                request.QRFID = QRFID;
                request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
                request.PaxSlabId = filterByPaxSlab == 0 ? defaultPaxSlabId : filterByPaxSlab;
                response = coProviders.GetCostsheet(request, token).Result;
                #endregion

                #region Main Tour Bindings
                List<QRFPkgAndNonPkgPrice> lstQrfPkg = new List<QRFPkgAndNonPkgPrice>();
                foreach (var a in response.QrfPackagePrice.OrderBy(x => x.Age))
                {
                    if (lstQrfPkg.Where(x => x.RoomName == a.RoomName && x.Age == a.Age).Count() <= 0)
                    {
                        lstQrfPkg.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, DisplayRoomName = a.RoomName, RoomName = a.RoomName, SellPrice = a.SellPrice, Age = a.Age, QRFCurrency = string.IsNullOrEmpty(a.QRFCurrency) ? "" : a.QRFCurrency.Substring(0, 3).ToUpper() });
                    }
                }

                #region Sequencewise sorting and Sell Price Calculation
                foreach (var l in lstQrfPkg.OrderBy(x => x.Age))
                {
                    double singleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "SINGLE" && x.PaxSlab_Id == l.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double doubleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "DOUBLE" && x.PaxSlab_Id == l.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double twinSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "TWIN" && x.PaxSlab_Id == l.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double tripleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "TRIPLE" && x.PaxSlab_Id == l.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();

                    if (l.RoomName.ToUpper() == "TWIN")
                    {
                        l.SequenceNo = 1;
                    }
                    else if (l.RoomName.ToUpper() == "DOUBLE")
                    {
                        l.SequenceNo = 2;
                    }
                    else if (l.RoomName.ToUpper() == "SINGLE")
                    {
                        if (l.DisplayRoomName == "SINGLE")
                            l.DisplayRoomName = "SGL SUPPLEMENT";

                        var price = twinSellPrice != 0 ? twinSellPrice : doubleSellPrice;
                        l.SellPrice = singleSellPrice != 0 ? singleSellPrice - price : singleSellPrice;

                        l.SequenceNo = 3;
                    }
                    else if (l.RoomName.ToUpper() == "TRIPLE")
                    {
                        if (l.DisplayRoomName == "TRIPLE")
                            l.DisplayRoomName = "TPL REDUCTION";

                        var price = twinSellPrice != 0 ? twinSellPrice : doubleSellPrice;
                        l.SellPrice = tripleSellPrice != 0 ? price - tripleSellPrice : tripleSellPrice;

                        l.SequenceNo = 4;
                    }
                    else if (l.RoomName.ToUpper() == "CHILDWITHBED")
                    {
                        if (l.DisplayRoomName.Contains("CHILDWITHBED"))
                        {
                            string roomname = l.DisplayRoomName;
                            l.DisplayRoomName = Regex.Replace(roomname, "CHILDWITHBED", "C + B") + " Age : " + l.Age;
                        }
                        l.SequenceNo = 5;
                    }
                    else if (l.RoomName.ToUpper() == "CHILDWITHOUTBED")
                    {
                        if (l.DisplayRoomName.Contains("CHILDWITHOUTBED"))
                        {
                            string roomname = l.DisplayRoomName;
                            l.DisplayRoomName = Regex.Replace(roomname, "CHILDWITHOUTBED", "C - B") + " Age : " + l.Age;
                        }
                        l.SequenceNo = 6;
                    }
                    else if (l.RoomName.ToUpper() == "INFANT")
                    {
                        l.SequenceNo = 100;
                    }
                    else
                        l.SequenceNo = 80;
                }
                #endregion
                #endregion

                #region Supplement Bindings
                List<QRFPkgAndNonPkgPrice> lstSupplementHeaders = new List<QRFPkgAndNonPkgPrice>();
                List<QRFPkgAndNonPkgPrice> lstSupplements = new List<QRFPkgAndNonPkgPrice>();

                foreach (var a in response.QrfNonPackagePrice)
                {
                    if (a.PositionKeepAs.ToUpper() == "SUPPLEMENT")
                    {
                        if (lstSupplementHeaders.Where(x => x.PositionId == a.PositionId).Count() <= 0)
                        {
                            lstSupplementHeaders.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, QRFCurrency = string.IsNullOrEmpty(a.QRFCurrency) ? "" : a.QRFCurrency.Substring(0, 3).ToUpper() });
                        }
                    }
                }

                foreach (var a in response.QrfNonPackagePrice)
                {
                    if (a.PositionKeepAs.ToUpper() == "SUPPLEMENT")
                    {
                        lstSupplements.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, QRFCurrency = string.IsNullOrEmpty(a.QRFCurrency) ? "" : a.QRFCurrency.Substring(0, 3).ToUpper() });
                    }
                }

                model.SupplementProductList = GetProductsForSupplement(lstSupplementHeaders);
                #endregion

                #region Optional Bindings
                List<QRFPkgAndNonPkgPrice> lstOptionalHeaders = new List<QRFPkgAndNonPkgPrice>();
                List<QRFPkgAndNonPkgPrice> lstOptionals = new List<QRFPkgAndNonPkgPrice>();

                foreach (var a in response.QrfNonPackagePrice)
                {
                    if (a.PositionKeepAs.ToUpper() == "OPTIONAL")
                    {
                        if (lstOptionalHeaders.Where(x => x.PositionId == a.PositionId).Count() <= 0)
                        {
                            lstOptionalHeaders.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, QRFCurrency = a.QRFCurrency.Substring(0, 3).ToUpper() });
                        }
                    }
                }

                foreach (var a in response.QrfNonPackagePrice)
                {
                    if (a.PositionKeepAs.ToUpper() == "OPTIONAL")
                    {
                        lstOptionals.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, QRFCurrency = a.QRFCurrency.Substring(0, 3).ToUpper() });
                    }
                }
                model.OptionalProductList = GetProductsForOptional(lstOptionalHeaders);
                #endregion

                model.MainTourList = lstQrfPkg.OrderBy(x => x.SequenceNo).ToList();

                model.QRFNonPkgSupplementPositions = lstSupplementHeaders;
                model.SupplementList = lstSupplements;

                model.QRFNonPkgOptionalPositions = lstOptionalHeaders;
                model.OptionalList = lstOptionals;

                model.DepartureDate = filterByDeparture;
                model.PaxSlab = filterByPaxSlab;

                return PartialView("_PricingSummary", model);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        public IActionResult GetItinerary(string QRFID, string filterByDay = null, string filterByServiceType = null, long filterByDeparture = 0, long filterByPaxSlab = 0)
        {
            try
            {
                ItineraryViewModel model = new ItineraryViewModel();

                #region Get Quote Info By QRFId
                NewQuoteViewModel modelQuote = new NewQuoteViewModel();
                modelQuote.QRFID = QRFID;
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                bool GetStatus = false;
                GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus)
                {
                    model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
                    model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
                }
                #endregion 

                #region Get Itinerary Details by QRFId
                itineraryGetReq = new ItineraryGetReq();
                itineraryGetReq.QRFID = QRFID;
                itineraryGetReq.editUser = ckUserEmailId;
                itineraryGetRes = coProviders.GetItineraryDetails(itineraryGetReq, token).Result;
                model.Itinerary = itineraryGetRes.Itinerary;
                #endregion

                #region Get Costsheet by QRFId
                NewQuoteViewModel modelQuote1 = new NewQuoteViewModel();
                bool GetStatus1 = false;
                SalesQuoteLibrary quoteLibrary1 = new SalesQuoteLibrary(_configuration);
                if (!string.IsNullOrEmpty(QRFID))
                {
                    modelQuote1.QRFID = QRFID;
                    GetStatus1 = quoteLibrary1.GetQRFPaxSlabDetails(_configuration, token, ref modelQuote1);
                }
                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                DepartureDateGetRequest objDepartureDatesReq = new DepartureDateGetRequest() { QRFID = QRFID, date = (DateTime?)null };
                DepartureDateGetResponse objDepartureDatesRes = objSalesProvider.GetDepartureDatesForQRFId(objDepartureDatesReq, token).Result;
                model.PaxSlabList = (modelQuote1.mdlQuotePaxRangeViewModel.QuotePaxSlabDetails).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlabId.ToString(), Value = a.PaxSlabFrom + "-" + a.PaxSlabTo }).ToList();
                model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
                long defaultPaxSlabId = Convert.ToInt32(model.PaxSlabList[0].AttributeValue_Id);
                CostsheetGetReq request = new CostsheetGetReq();
                CostsheetGetRes response = new CostsheetGetRes();
                request.QRFID = QRFID;
                request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
                request.PaxSlabId = filterByPaxSlab == 0 ? defaultPaxSlabId : filterByPaxSlab;
                response = coProviders.GetCostsheet(request, token).Result;
                #endregion

                #region Price Per Person Binding
                List<ItineraryDaysInfo> lstDays = itineraryGetRes.Itinerary.ItineraryDays.ToList();
                List<QRFPkgAndNonPkgPrice> lstPositions = new List<QRFPkgAndNonPkgPrice>();

                foreach (var a in lstDays)
                {
                    List<ItineraryDescriptionInfo> lstdes = a.ItineraryDescription.Where(x => x.KeepAs.ToUpper() != "INCLUDED").ToList();
                    foreach (var k in response.QrfNonPackagePrice.Where(y => y.RoomName.ToUpper() == "ADULT"))
                    {
                        foreach (var l in lstdes)
                        {
                            if (l.PositionId == k.PositionId)
                            {
                                lstPositions.Add(new QRFPkgAndNonPkgPrice { QRFCurrency = string.IsNullOrEmpty(k.QRFCurrency) ? "" : k.QRFCurrency.Substring(0, 3).ToUpper(), SellPrice = k.SellPrice, PositionId = k.PositionId, PositionKeepAs = k.PositionKeepAs, PaxSlabId = k.PaxSlab_Id, ProductName = k.ProductName, RoomName = k.RoomName });
                            }
                        }
                    }
                }

                model.ListNonQrfPkgPositions = lstPositions;
                #endregion

                #region Filter Dropdown Bindings
                model.DayList = GetDays(itineraryGetRes.Itinerary.ItineraryDays);
                model.ServiceTypeList = GetProductTypes(model.Itinerary.ItineraryDays);
                #endregion

                #region Date Binding
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
                model.DepartureDate = filterByDeparture;
                model.PaxSlab = filterByPaxSlab;
                #endregion

                return PartialView("_Itinerary", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult EditQRFPosition(string QRFID, string PositionId, string ItineraryID, string ItineraryDaysId, string ProductType = null, string Type = null)
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
                    bool IsClone = true;
                    string prodType = ProductType.ToUpper();
                    switch (prodType)
                    {
                        case "MEAL":
                            return RedirectToAction("GetMealsvenue", "Meals", new { QRFID, PositionId, IsClone });

                        case "HOTEL":
                            return RedirectToAction("Accomodation", "Accomodation", new { QRFID, PositionId, IsClone });

                        case "ATTRACTIONS":
                            return RedirectToAction("Activities", "Activities", new { QRFID, PositionId, IsClone });

                        case "COACH":
                        case "LDC":
                            return RedirectToAction("Bus", "Bus", new { QRFID, PositionId, IsClone });

                        case "OVERNIGHT FERRY":
                            return RedirectToAction("Cruise", "Cruise", new { QRFID, PositionId, IsClone });

                        case "PRIVATE TRANSFER":
                        case "SCHEDULED TRANSFER":
                        case "FERRY TRANSFER":
                        case "FERRY PASSENGER":
                            return RedirectToAction("GetTransfersType", "Transfers", new { QRFID, PositionId, IsClone });

                        case "GUIDE":
                        case "ASSISTANT":
                            ProductType = "guide";
                            return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType, IsClone });
                        case "VISA":
                        case "INSURANCE":
                            return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType, IsClone });
                        case "OTHER":
                        case "FEE":
                            ProductType = "other";
                            return RedirectToAction("Others", "Others", new { QRFID, PositionId, ProductType, IsClone }); 
                       
                        case "DOMESTIC FLIGHT":
                            return RedirectToAction("Flights", "Flights", new { QRFID, PositionId, IsClone });

                        case "TRAIN":
                            return RedirectToAction("Rail", "Rail", new { QRFID, PositionId, IsClone });

                        default:
                            return Content("No Content");
                    }
                }
            }
            catch (Exception ex)
            { 
                return View();
            }
        }

        public IActionResult EnableDisableItineraryDetails(string QRFID, string PositionId, string ItineraryId, string ItineraryDaysId, bool IsDeleted)
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
        }

        #region Helper Methods
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

        public List<Attributes> GetProductsForSupplement(List<QRFPkgAndNonPkgPrice> lstSupplementHeaders)
        {
            List<Attributes> lstProducts = new List<Attributes>();

            foreach (var k in lstSupplementHeaders)
            {
                if (lstProducts.Where(a => a.AttributeName == k.PositionType).Count() <= 0)
                {
                    lstProducts.Add(new Attributes { Attribute_Id = k.PositionType, AttributeName = k.PositionType });
                }
            }
            return lstProducts.OrderBy(x => x.AttributeName).ToList();
        }

        public List<Attributes> GetProductsForOptional(List<QRFPkgAndNonPkgPrice> lstOptionalHeaders)
        {
            List<Attributes> lstProducts = new List<Attributes>();

            foreach (var k in lstOptionalHeaders)
            {
                if (lstProducts.Where(a => a.AttributeName == k.PositionType).Count() <= 0)
                {
                    lstProducts.Add(new Attributes { Attribute_Id = k.PositionType, AttributeName = k.PositionType });
                }
            }
            return lstProducts.OrderBy(x => x.AttributeName).ToList();
        }

        #endregion

    }
}