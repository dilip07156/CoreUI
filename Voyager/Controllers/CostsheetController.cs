using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class CostsheetController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        private COProviders coProviders;

        public CostsheetController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            coProviders = new COProviders(_configuration);
        }
        #endregion

        [Route("CostingOfficer/Costsheet")]
        public IActionResult Costsheet()
        {
            if (UserRoles.Contains("Product Accountant"))
            { }
            else if (!UserRoles.Contains("Costing Officer"))
            {
                return View("Unauthorize");
            }

            string QRFID = Convert.ToString(Request.Query["QRFId"]);

            CostsheetViewModel model = new CostsheetViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Costsheet";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            #region Dropdown Binding
            QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
            QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
            model.ConsolidatedSummaryViewModel.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
            #endregion

            #region Get Costsheet by QRFId
            CostsheetGetReq request = new CostsheetGetReq();
            CostsheetGetRes response = new CostsheetGetRes();
            request.QRFID = QRFID;
            response = coProviders.GetCostsheet(request, token).Result;
            model.ConsolidatedSummaryViewModel.CostsheetVersion = response.CostsheetVersion;
            #endregion

            return View(model);
        }

        #region Consolidated Summary
        public IActionResult GetConsolidatedSummary(string QRFID, long filterByDeparture = 0, string SuccessMessage = null, string ErrorMessage = null)
        {
            if (!(string.IsNullOrEmpty(SuccessMessage)))
                TempData["success"] = SuccessMessage;

            if (!(string.IsNullOrEmpty(ErrorMessage)))
                TempData["error"] = ErrorMessage;

            ConsolidatedSummaryViewModel model = new ConsolidatedSummaryViewModel();

            #region Dropdown Binding
            QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
            QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
            model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
            #endregion

            #region Get Costsheet by QRFId
            CostsheetGetReq request = new CostsheetGetReq();
            CostsheetGetRes response = new CostsheetGetRes();
            request.QRFID = QRFID;
            request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
            response = coProviders.GetCostsheet(request, token).Result;
            model.CostsheetVersion = response.CostsheetVersion;

            #region Get currency list
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
            List<Currency> CurrencyList = objCurrencyResponse.CurrencyList;
            #endregion
            #endregion

            #region Header Binding
            List<QRFPkgAndNonPkgPrice> lstHeaders = new List<QRFPkgAndNonPkgPrice>();
            foreach (var a in response.QrfPackagePrice.OrderBy(x => x.Age))
            {
                if (lstHeaders.Where(x => x.RoomName == a.RoomName && x.Age == a.Age).Count() <= 0)
                {
                    var qrfCurrency = CurrencyList.Where(x => x.CurrencyId == a.QRFCurrency_Id).Select(x => x.CurrencyCode).FirstOrDefault();
                    if (!string.IsNullOrEmpty(qrfCurrency)) { qrfCurrency = qrfCurrency.ToUpper(); }
                    lstHeaders.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, DisplayRoomName = a.RoomName, RoomName = a.RoomName, Age = a.Age, QRFCurrency = qrfCurrency, DepartureDate = a.DepartureDate, DepartureId = a.Departure_Id });
                }
            }
            if (lstHeaders.Count == 0)
            {
                foreach (var a in response.QrfNonPackagePrice.OrderBy(x => x.Age))
                {
                    if (lstHeaders.Where(x => x.RoomName == a.RoomName && x.Age == a.Age).Count() <= 0)
                    {
                        var qrfCurrency = CurrencyList.Where(x => x.CurrencyId == a.QRFCurrency_Id).Select(x => x.CurrencyCode).FirstOrDefault().ToUpper();
                        if (!string.IsNullOrEmpty(qrfCurrency)) { qrfCurrency = qrfCurrency.ToUpper(); }
                        lstHeaders.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, DisplayRoomName = a.RoomName, RoomName = a.RoomName, Age = a.Age, QRFCurrency = qrfCurrency, DepartureDate = a.DepartureDate, DepartureId = a.Departure_Id, Type = a.RoomName });
                    }
                }
            }

            foreach (var k in lstHeaders)
            {
                if (k.DisplayRoomName == "SINGLE")
                    k.DisplayRoomName = "SGL SUPPLEMENT";
                if (k.DisplayRoomName == "TRIPLE")
                    k.DisplayRoomName = "TPL REDUCTION";
                if (k.DisplayRoomName.Contains("CHILDWITHBED"))
                {
                    string roomname = k.DisplayRoomName;
                    k.DisplayRoomName = Regex.Replace(roomname, "CHILDWITHBED", "C + B") + " Age : " + k.Age;
                }
                if (k.DisplayRoomName.Contains("CHILDWITHOUTBED"))
                {
                    string roomname = k.DisplayRoomName;
                    k.DisplayRoomName = Regex.Replace(roomname, "CHILDWITHOUTBED", "C - B") + " Age : " + k.Age;
                }
            }

            #region Sequencewise sorting
            foreach (var l in lstHeaders)
            {
                if (l.RoomName.ToUpper() == "TWIN")
                {
                    l.SequenceNo = 1;
                    l.Type = "ADULT";
                }
                else if (l.RoomName.ToUpper() == "DOUBLE")
                {
                    l.SequenceNo = 2;
                    l.Type = "ADULT";
                }
                else if (l.RoomName.ToUpper() == "SINGLE")
                {
                    l.SequenceNo = 3;
                    l.Type = "ADULT";
                }
                else if (l.RoomName.ToUpper() == "TRIPLE")
                {
                    l.SequenceNo = 4;
                    l.Type = "ADULT";
                }
                else if (l.RoomName.ToUpper() == "CHILDWITHBED")
                {
                    l.SequenceNo = 5;
                }
                else if (l.RoomName.ToUpper() == "CHILDWITHOUTBED")
                {
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

            model.HeaderList = lstHeaders.OrderBy(x => x.SequenceNo).ToList();
            #endregion

            #region Data binding
            List<QRFPkgAndNonPkgPrice> lstPrice = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstPriceData = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstSupplements = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstOptionals = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstPositionTypeSupplement = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstPositionTypeOptional = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstSupplementPaxSlabs = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstOptionalPaxSlabs = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstQRFPkgPricePositionTotalCost = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstNonQRFPkgPricePositionTotalCost = new List<QRFPkgAndNonPkgPrice>();

            #region QRFPkgPrice Postion Bindings

            #region Paxslab and FOC binding (1st/2nd column)
            foreach (var a in response.QrfPackagePrice)
            {

                if (lstPrice.Where(x => x.PaxSlabId == a.PaxSlab_Id).Count() <= 0)
                {
                    lstPrice.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName });
                }
            }

            foreach (var pax in lstPrice)
            {
                var focPrice = new QRFPkgAndNonPkgPrice();

                foreach (var f in response.QRFSalesFOC)
                {
                    if (pax.PaxSlabId == f.PaxSlabId)
                    {
                        string foc1;
                        StringBuilder str = new StringBuilder();
                        if (f.FOCSingle > 0)
                        {
                            str.Append(f.FOCSingle + " SINGLE");
                        }
                        if (f.FOCDouble > 0)
                        {
                            if (f.FOCSingle > 0)
                            {
                                str.Append(" + ");
                            }
                            str.Append(f.FOCDouble + " DOUBLE");
                        }
                        if (f.FOCTwin > 0)
                        {
                            if (f.FOCDouble > 0 || f.FOCSingle > 0)
                            {
                                str.Append(" + ");
                            }
                            str.Append(f.FOCTwin + " TWIN");
                        }
                        if (f.FOCTriple > 0)
                        {
                            if (f.FOCTwin > 0 || f.FOCDouble > 0 || f.FOCSingle > 0)
                            {
                                str.Append(" + ");
                            }
                            str.Append(f.FOCTriple + " TRIPLE");
                        }
                        foc1 = str.ToString();
                        focPrice.focValue = foc1;
                        focPrice.PaxSlabId = f.PaxSlabId;
                        lstQRFPkgPricePositionTotalCost.Add(focPrice);
                        lstNonQRFPkgPricePositionTotalCost.Add(focPrice);
                    }
                }
            }

            #endregion

            #region Roomdetails binding
            foreach (var a in response.QrfPackagePrice)
            {
                foreach (var k in lstPrice)
                {
                    double singleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "SINGLE" && x.PaxSlab_Id == k.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double doubleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "DOUBLE" && x.PaxSlab_Id == k.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double twinSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "TWIN" && x.PaxSlab_Id == k.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();
                    double tripleSellPrice = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "TRIPLE" && x.PaxSlab_Id == k.PaxSlabId).Select(y => y.SellPrice).FirstOrDefault();

                    if (k.PaxSlabId == a.PaxSlab_Id && a.RoomName.ToUpper() == "SINGLE")
                    {
                        var price = twinSellPrice != 0 ? twinSellPrice : doubleSellPrice;
                        a.SellPrice = singleSellPrice != 0 ? singleSellPrice - price : singleSellPrice;
                    }

                    if (k.PaxSlabId == a.PaxSlab_Id && a.RoomName.ToUpper() == "TRIPLE")
                    {
                        var price = twinSellPrice != 0 ? twinSellPrice : doubleSellPrice;
                        a.SellPrice = tripleSellPrice != 0 ? price - tripleSellPrice : tripleSellPrice;
                    }
                }

                lstPriceData.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age });
            }

            model.QRFPackagePriceList = lstPrice;
            model.QRFPkgPositionTotalCost = lstQRFPkgPricePositionTotalCost;
            model.QRFPackagePriceDataList = lstPriceData;

            #endregion

            #endregion

            #region QRFNonPkgPrice Position Bindings
            foreach (var a in response.QrfNonPackagePrice.Where(x => x.PositionKeepAs.ToUpper() == "SUPPLEMENT"))
            {
                if (lstPositionTypeSupplement.Where(x => x.PositionId == a.PositionId).Count() <= 0)
                {
                    lstPositionTypeSupplement.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName });
                }
            }

            foreach (var a in response.QrfNonPackagePrice.Where(x => x.PositionKeepAs.ToUpper() == "OPTIONAL"))
            {
                if (lstPositionTypeOptional.Where(x => x.PositionId == a.PositionId).Count() <= 0)
                {
                    lstPositionTypeOptional.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName });
                }
            }

            model.QRFNonPkgSupplementPositions = lstPositionTypeSupplement;
            model.QRFNonPkgOptionalPositions = lstPositionTypeOptional;
            #endregion

            #region QRFNonPkgPrice Paxslab & FOC binding (1st/2nd column)

            foreach (var a in response.QrfNonPackagePrice)
            {
                if (a.PositionKeepAs.ToUpper() == "SUPPLEMENT")
                {
                    if (lstSupplementPaxSlabs.Where(x => x.PaxSlabId == a.PaxSlab_Id).Count() <= 0)
                    {
                        lstSupplementPaxSlabs.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, ProductName = a.ProductName });
                    }
                }
            }

            foreach (var a in response.QrfNonPackagePrice)
            {
                if (a.PositionKeepAs.ToUpper() == "OPTIONAL")
                {
                    if (lstOptionalPaxSlabs.Where(x => x.PaxSlabId == a.PaxSlab_Id).Count() <= 0)
                    {
                        lstOptionalPaxSlabs.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, ProductName = a.ProductName });
                    }
                }
            }

            //foreach (var p in response.QRFPositionTotalCost)
            //{
            //lstNonQRFPkgPricePositionTotalCost.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = p.PaxSlab_Id, PaxSlab = p.PaxSlab, TotalSellPriceForFOC = p.FOCInBuyCurrency, PositionId = p.PositionId });
            //}


            model.QRFNonPkgPositionTotalCost = lstNonQRFPkgPricePositionTotalCost;
            model.QRFNonPkgPriceSupplementPaxSlabs = lstSupplementPaxSlabs;
            model.QRFNonPkgPriceOptionalPaxSlabs = lstOptionalPaxSlabs;
            #endregion

            #region Roomdetails binding

            foreach (var a in response.QrfNonPackagePrice)
            {
                if (a.PositionKeepAs.ToUpper() == "SUPPLEMENT")
                {
                    lstSupplements.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName });

                }
            }

            foreach (var a in response.QrfNonPackagePrice)
            {
                if (a.PositionKeepAs.ToUpper() == "OPTIONAL")
                {
                    lstOptionals.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, Age = a.Age, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName });
                }
            }

            model.QRFNonPackagePriceSupplementList = lstSupplements;
            model.QRFNonPackagePriceOptionalList = lstOptionals;

            #endregion

            model.DepartureDate = filterByDeparture.ToString();
            #endregion

            return PartialView("_ConsolidatedSummary", model);
        }
        #endregion

        #region Detailed Info
        public IActionResult GetDetailedInfo(string QRFID, long filterByDeparture = 0)
        {
            DetailedInfoViewModel model = new DetailedInfoViewModel();

            #region Dropdown Binding
            QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
            QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
            QRFPaxSlabGetReq objPaxSlabReq = new QRFPaxSlabGetReq() { QRFID = QRFID };
            QRFPaxGetResponse objPaxSlabRes = coProviders.GetPaxSlabDetailsForCostingByQRF_Id(objPaxSlabReq, token).Result;
            GuesstimateGetReq request = new GuesstimateGetReq() { QRFID = QRFID };
            GuesstimateGetRes response = coProviders.GetGuesstimate(request, token).Result;

            model.ProductNameList = GetProductNamesFromGuesstimate(response.Guesstimate.GuesstimatePosition);
            model.SupplierList = GetSuppliersFromGuesstimate(response.Guesstimate.GuesstimatePosition);
            model.ProductTypeList = GetProductTypesFromGuesstimate(response.Guesstimate.GuesstimatePosition);
            model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
            model.PaxSlabList = (objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlab_Id.ToString(), Value = a.From + "-" + a.To }).ToList();
            #endregion

            return PartialView("_DetailedInfo", model);
        }
        #endregion

        [HttpPost]
        public JsonResult SetCostsheetNewVersion(string QRFID, string Pipeline)
        {
            CostsheetVerSetRes response = new CostsheetVerSetRes();
            CostsheetVerSetReq request = new CostsheetVerSetReq();
            request.QRFID = QRFID;
            request.Pipeline = Pipeline;
            request.Create_User = ckUserEmailId;
            response = coProviders.SetCostsheetNewVersion(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        public IActionResult GetCostsheetVersions(string QRFID)
        {
            try
            {
                CostsheetVersionGetRes response = new CostsheetVersionGetRes();
                CostsheetGetReq request = new CostsheetGetReq();
                request.QRFID = QRFID;
                response = coProviders.GetCostsheetVersions(request, token).Result;
                return PartialView("_CostsheetVersion", response.CostsheetVersions);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        [HttpPost]
        public JsonResult UpdateCostsheetVersion(string QRFID, string QRFPriceId)
        {
            CostsheetVerSetRes response = new CostsheetVerSetRes();
            CostsheetVerSetReq request = new CostsheetVerSetReq();
            request.QRFID = QRFID;
            request.QRFPriceId = QRFPriceId;
            request.Create_User = ckUserEmailId;
            response = coProviders.UpdateCostsheetVersion(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        [HttpPost]
        public JsonResult CheckActiveCostsheetPrice(string QRFID,string EnquiryPipeline)
        {
            ResponseStatus response = new ResponseStatus();
            CostsheetGetReq request = new CostsheetGetReq();
            request.QRFID = QRFID;
            request.EnquiryPipeline = EnquiryPipeline;
            response = coProviders.CheckActiveCostsheetPrice(request, token).Result;

            if (response != null)
            {
                return Json(response);
            }
            return Json(response = new ResponseStatus() { Status = "Failure", ErrorMessage = "An Error Occurs." });
        }

        #region Helper Methods

        public List<App.ViewModels.ProductDetails> GetProductTypesFromGuesstimate(List<GuesstimatePosition> pos)
        {
            List<Voyager.App.ViewModels.ProductDetails> lstProductTypes = new List<Voyager.App.ViewModels.ProductDetails>();
            foreach (var m in pos)
            {
                if (lstProductTypes.Where(a => a.Prodtype == m.ProductType).Count() <= 0)
                {
                    lstProductTypes.Add(new Voyager.App.ViewModels.ProductDetails { Prodtype = m.ProductType });
                }
            }
            return lstProductTypes.OrderBy(x => x.Prodtype).ToList();
        }

        public List<AttributeValues> GetProductNamesFromGuesstimate(List<GuesstimatePosition> pos)
        {
            List<AttributeValues> lstProductTypes = new List<AttributeValues>();
            foreach (var m in pos)
            {
                if (lstProductTypes.Where(a => a.Value == m.OriginalItineraryDescription).Count() <= 0)
                {
                    lstProductTypes.Add(new AttributeValues { AttributeValue_Id = m.ProductType, Value = m.OriginalItineraryDescription });
                }
            }
            return lstProductTypes.OrderBy(x => x.Value).ToList();
        }

        public List<AttributeValues> GetSuppliersFromGuesstimate(List<GuesstimatePosition> pos)
        {
            List<AttributeValues> lstSuppliers = new List<AttributeValues>();
            foreach (var m in pos)
            {
                if (!string.IsNullOrEmpty(m.ActiveSupplier))
                {
                    if (lstSuppliers.Where(a => a.Value == m.ActiveSupplier).Count() <= 0)
                    {
                        lstSuppliers.Add(new AttributeValues { AttributeValue_Id = m.ActiveSupplierId, Value = m.ActiveSupplier });
                    }
                }
            }
            return lstSuppliers.OrderBy(x => x.Value).ToList();

        }
        #endregion
    }
}