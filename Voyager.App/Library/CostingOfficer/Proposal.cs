using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.App.Library
{
    public class ProposalLibrary
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        COProviders coProviders;
        public COCommonLibrary cOCommonLibrary;

        public ProposalLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            coProviders = new COProviders(_configuration);
            cOCommonLibrary = new COCommonLibrary(configuration);
        }
        #endregion

        public bool GetProposalPriceBreakupByQRFId(IConfiguration _configuration, string token, ref ProposalViewModel model, string QRFID)
        {
            #region Get Costsheet by QRFId
            CostsheetGetReq request = new CostsheetGetReq();
            CostsheetGetRes response = new CostsheetGetRes();
            request.QRFID = QRFID;
            //request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
            response = coProviders.GetCostsheet(request, token).Result;
            #endregion

            #region Price breakup data binding
            List<QRFPkgAndNonPkgPrice> lstDepartureDates = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstTwinPkg = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstDoublePkg = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstSinglePkg = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstPositionTypeSupplement = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstPositionTypeOptional = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstNonPkgSupplement = new List<QRFPkgAndNonPkgPrice>();
            List<QRFPkgAndNonPkgPrice> lstNonPkgOptional = new List<QRFPkgAndNonPkgPrice>();

            foreach (var date in response.QrfPackagePrice)
            {
                if (lstDepartureDates.Where(x => x.DepartureId == date.Departure_Id).Count() <= 0)
                {
                    lstDepartureDates.Add(new QRFPkgAndNonPkgPrice { DepartureDate = date.DepartureDate, DepartureId = date.Departure_Id, PaxSlabId = date.PaxSlab_Id, PaxSlab = date.PaxSlab });
                }
            }

            var twinrooms = new List<mQRFPackagePrice>();
            var doublerooms = new List<mQRFPackagePrice>();
            var singlerooms = new List<mQRFPackagePrice>();

            twinrooms = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "TWIN").ToList();
            if (twinrooms.Count() <= 0)
            {
                doublerooms = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "DOUBLE").ToList();
                if (doublerooms.Count() <= 0)
                {
                    singlerooms = response.QrfPackagePrice.Where(x => x.RoomName.ToUpper() == "SINGLE").ToList();
                }
            }

            if (twinrooms.Count() > 0)
            {
                foreach (var pkg in twinrooms)
                {
                    lstTwinPkg.Add(new QRFPkgAndNonPkgPrice { SellPrice = pkg.SellPrice, QRFCurrency = pkg.QRFCurrency.Substring(0, 3).ToUpper(), PaxSlabId = pkg.PaxSlab_Id, PaxSlab = pkg.PaxSlab, RoomName = pkg.RoomName, DepartureDate = pkg.DepartureDate, DepartureId = pkg.Departure_Id });
                }
            }

            if (doublerooms.Count() > 0)
            {
                foreach (var pkg in doublerooms)
                {
                    lstDoublePkg.Add(new QRFPkgAndNonPkgPrice { SellPrice = pkg.SellPrice, QRFCurrency = pkg.QRFCurrency.Substring(0, 3).ToUpper(), PaxSlabId = pkg.PaxSlab_Id, PaxSlab = pkg.PaxSlab, RoomName = pkg.RoomName, DepartureDate = pkg.DepartureDate, DepartureId = pkg.Departure_Id });
                }
            }

            if (singlerooms.Count() > 0)
            {
                foreach (var pkg in singlerooms)
                {
                    lstSinglePkg.Add(new QRFPkgAndNonPkgPrice { SellPrice = pkg.SellPrice, QRFCurrency = pkg.QRFCurrency.Substring(0, 3).ToUpper(), PaxSlabId = pkg.PaxSlab_Id, PaxSlab = pkg.PaxSlab, RoomName = pkg.RoomName, DepartureDate = pkg.DepartureDate, DepartureId = pkg.Departure_Id });
                }
            }

            //#region QRFNonPkgPrice Position Bindings
            //foreach (var a in response.QrfNonPackagePrice.Where(x => x.PositionKeepAs.ToUpper() == "SUPPLEMENT"))
            //{
            //    if (lstPositionTypeSupplement.Where(x => x.PositionId == a.PositionId).Count() <= 0)
            //    {
            //        lstPositionTypeSupplement.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, DepartureDate = a.DepartureDate, DepartureId = a.Departure_Id });
            //    }
            //}

            //foreach (var a in response.QrfNonPackagePrice.Where(x => x.PositionKeepAs.ToUpper() == "OPTIONAL"))
            //{
            //    if (lstPositionTypeOptional.Where(x => x.PositionId == a.PositionId).Count() <= 0)
            //    {
            //        lstPositionTypeOptional.Add(new QRFPkgAndNonPkgPrice { PaxSlabId = a.PaxSlab_Id, PaxSlab = a.PaxSlab, SellPrice = a.SellPrice, RoomName = a.RoomName, PositionKeepAs = a.PositionKeepAs, PositionType = a.PositionType, PositionId = a.PositionId, ProductName = a.ProductName, DepartureDate = a.DepartureDate, DepartureId = a.Departure_Id });
            //    }
            //}
            //#endregion

            foreach (var nonpkg in response.QrfNonPackagePrice)
            {
                if (nonpkg.RoomName.ToUpper() == "ADULT" && nonpkg.PositionKeepAs.ToUpper() == "SUPPLEMENT")
                {
                    lstNonPkgSupplement.Add(new QRFPkgAndNonPkgPrice { SellPrice = nonpkg.SellPrice, QRFCurrency = nonpkg.QRFCurrency.Substring(0, 3).ToUpper(), PaxSlabId = nonpkg.PaxSlab_Id, PaxSlab = nonpkg.PaxSlab, RoomName = nonpkg.RoomName, DepartureDate = nonpkg.DepartureDate, DepartureId = nonpkg.Departure_Id, PositionId = nonpkg.PositionId, PositionType = nonpkg.PositionType, ProductName = nonpkg.ProductName });
                }
            }

            foreach (var nonpkg in response.QrfNonPackagePrice)
            {
                if (nonpkg.RoomName.ToUpper() == "ADULT" && nonpkg.PositionKeepAs.ToUpper() == "OPTIONAL")
                {
                    lstNonPkgOptional.Add(new QRFPkgAndNonPkgPrice { SellPrice = nonpkg.SellPrice, QRFCurrency = nonpkg.QRFCurrency.Substring(0, 3).ToUpper(), PaxSlabId = nonpkg.PaxSlab_Id, PaxSlab = nonpkg.PaxSlab, RoomName = nonpkg.RoomName, DepartureDate = nonpkg.DepartureDate, DepartureId = nonpkg.Departure_Id, PositionId = nonpkg.PositionId, PositionType = nonpkg.PositionType, ProductName = nonpkg.ProductName });
                }
            }

            model.ProposalPriceBreakupViewModel.DepartureDatesList = lstDepartureDates.OrderBy(x => x.DepartureDate).ToList();
            model.ProposalPriceBreakupViewModel.QrfTwinPkgPriceList = lstTwinPkg;
            model.ProposalPriceBreakupViewModel.QrfDoublePkgPriceList = lstDoublePkg;
            model.ProposalPriceBreakupViewModel.QrfSinglePkgPriceList = lstSinglePkg;
            //model.ProposalPriceBreakupViewModel.QRFNonPkgSupplementPositions = lstPositionTypeSupplement;
            //model.ProposalPriceBreakupViewModel.QRFNonPkgOptionalPositions = lstPositionTypeOptional;
            model.ProposalPriceBreakupViewModel.QrfNonPkgPriceSupplementList = lstNonPkgSupplement;
            model.ProposalPriceBreakupViewModel.QrfNonPkgPriceOptionalList = lstNonPkgOptional;

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.ProposalPriceBreakupViewModel.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            #endregion

            #region Get Hotel Summary details from Itinerary

            ProposalGetReq req = new ProposalGetReq();
            ProposalGetRes res = new ProposalGetRes();

            req.QRFID = model.QRFID;
            res = coProviders.GetHotelSummaryByQrfId(req, token).Result;
            var hotellist = res.Hotels;
            model.ProposalPriceBreakupViewModel.HotelList = hotellist.Select(x => new Hotel { HotelName = x.HotelName, Location = string.IsNullOrWhiteSpace(x.Location) ? "" : x.Location.Split(',')[0], Stars = x.Stars, Duration = x.Duration }).ToList();

            #endregion

            #endregion

            return true;
        }
    }
}
