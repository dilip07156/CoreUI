using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class CommercialsController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        private COProviders coProviders;
        private string SessionName = "IntegrationInfo";

        public CommercialsController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            coProviders = new COProviders(_configuration);
        }
        #endregion

        [Route("CostingOfficer/Commercials")]
        public IActionResult Commercials()
        {
            if (UserRoles.Contains("Product Accountant"))
            { }
            else if (!UserRoles.Contains("Costing Officer"))
            {
                return View("Unauthorize");
            }
            string QRFID = Request.Query["QRFId"];

            CommercialsViewModel model = new CommercialsViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Commercials";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return View(model);
        }

        public IActionResult GetCommercialsData(string QRFID, string EnquiryPipeline = "", long filterByDeparture = 0, long filterByPaxSlab = 0)
        {
            CommercialsViewModel model = new CommercialsViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.EnquiryPipeline = EnquiryPipeline;
            #region Dropdown Binding
            //NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            //bool GetStatus = false;
            //SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            //if (Convert.ToInt32(QRFId) > 0)
            //{
            //    modelQuote.QRFID = QRFID;
            //    GetStatus = quoteLibrary.GetQRFPaxSlabDetails(_configuration, token, ref modelQuote);
            //}

            //SalesProviders objSalesProvider = new SalesProviders(_configuration);
            //DepartureDateGetRequest objDepartureDatesReq = new DepartureDateGetRequest() { QRF_Id = QRFId, date = (DateTime?)null };
            //DepartureDateGetResponse objDepartureDatesRes = objSalesProvider.GetDepartureDatesForQRFId(objDepartureDatesReq, token).Result;
            //model.PaxSlabList = (modelQuote.mdlQuotePaxRangeViewModel.QuotePaxSlabDetails).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlabId.ToString(), Value = a.PaxSlabFrom + "-" + a.PaxSlabTo }).ToList();
            //model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();
            ////long defaultPaxSlabId = Convert.ToInt32(model.PaxSlabList[0].AttributeValue_Id);

            QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
            QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
            QRFPaxSlabGetReq objPaxSlabReq = new QRFPaxSlabGetReq() { QRFID = QRFID };
            QRFPaxGetResponse objPaxSlabRes = coProviders.GetPaxSlabDetailsForCostingByQRF_Id(objPaxSlabReq, token).Result;
            model.PaxSlabList = (objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlab_Id.ToString(), Value = a.From + "-" + a.To }).ToList();
            model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();

            model.CurrentDate = DateTime.Now.ToString("dd MMM yyyy");

            string username = "";
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            username = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
            model.UserName = username;
            #endregion

            #region Get Commercial data
            CommercialsGetRes response = new CommercialsGetRes();
            CommercialsGetReq request = new CommercialsGetReq();
            request.QRFID = QRFID;
            request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
            request.PaxSlabId = filterByPaxSlab == 0 ? objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs[0].PaxSlab_Id : filterByPaxSlab;

            response = coProviders.GetCommercials(request, token).Result;
            model.QRFID = response.QRFID;
            model.QRFPriceId = response.QRFPriceId;
            model.PercentSoldOptional = response.PercentSoldOptional;

            model.BareBoneList = response.BareBoneList;
            model.QRFExhangeRates = response.QRFExhangeRates;

            string PositionType = "";
            foreach (var item in response.PositionIncluded)
            {
                if (item.PositionType != PositionType)
                {
                    model.PositionIncluded.Add(new mQRFPositionTotalCost
                    {
                        QRFCostForPositionID = "",
                        PositionType = item.PositionType,
                        ProductName = "",
                        TotalBuyPrice = response.PositionIncluded.Where(a => a.PositionType == item.PositionType).Sum(a => a.TotalBuyPrice),
                        TotalSellPrice = response.PositionIncluded.Where(a => a.PositionType == item.PositionType).Sum(a => a.TotalSellPrice),
                        ProfitPercentage = response.PositionIncluded.Where(a => a.PositionType == item.PositionType && a.ProfitPercentage > 0).Count() > 0 ?
                        (response.PositionIncluded.Where(a => a.PositionType == item.PositionType).Sum(a => a.ProfitPercentage)
                                            / response.PositionIncluded.Where(a => a.PositionType == item.PositionType && a.ProfitPercentage > 0).Count()) : 0,
                        ProfitAmount = response.PositionIncluded.Where(a => a.PositionType == item.PositionType).Sum(a => a.ProfitAmount),
                        BuyCurrency = item.BuyCurrency,
                        QRFCurrency = item.QRFCurrency
                    });
                }
                model.PositionIncluded.Add(new mQRFPositionTotalCost
                {
                    QRFCostForPositionID = item.QRFCostForPositionID,
                    PositionType = item.PositionType,
                    PositionId = item.PositionId,
                    ProductName = item.ProductName,
                    TotalBuyPrice = item.TotalBuyPrice,
                    TotalSellPrice = item.TotalSellPrice,
                    ProfitAmount = item.ProfitAmount,
                    ProfitPercentage = item.ProfitPercentage,
                    BuyCurrency = item.BuyCurrency,
                    QRFCurrency = item.QRFCurrency
                });
                PositionType = item.PositionType;
            }

            foreach (var item in response.PositionSupplement)
            {
                model.PositionSupplement.Add(new mQRFPositionTotalCost
                {
                    QRFCostForPositionID = item.QRFCostForPositionID,
                    PositionType = item.PositionType,
                    PositionId = item.PositionId,
                    ProductName = item.ProductName,
                    TotalBuyPrice = item.TotalBuyPrice,
                    TotalSellPrice = item.TotalSellPrice,
                    ProfitAmount = item.ProfitAmount,
                    ProfitPercentage = item.ProfitPercentage,
                    BuyCurrency = item.BuyCurrency,
                    QRFCurrency = item.QRFCurrency
                });
            }

            foreach (var item in response.PositionOptional)
            {
                model.PositionOptional.Add(new mQRFPositionTotalCost
                {
                    QRFCostForPositionID = item.QRFCostForPositionID,
                    PositionType = item.PositionType,
                    PositionId = item.PositionId,
                    ProductName = item.ProductName,
                    TotalBuyPrice = item.TotalBuyPrice,
                    TotalSellPrice = item.TotalSellPrice,
                    ProfitAmount = item.ProfitAmount,
                    ProfitPercentage = item.ProfitPercentage,
                    BuyCurrency = item.BuyCurrency,
                    QRFCurrency = item.QRFCurrency
                });
            }

            model.DepartureDate = filterByDeparture;
            model.PaxSlab = filterByPaxSlab;

            #endregion

            if (model.PositionSupplement.Count > 0)
                ViewBag.QRFCurrency = model.PositionSupplement[0].QRFCurrency;
            if (model.PositionOptional.Count > 0)
                ViewBag.QRFCurrency = model.PositionOptional[0].QRFCurrency;
            if (model.PositionIncluded.Count > 0)
                ViewBag.QRFCurrency = model.PositionIncluded[0].QRFCurrency;

            ViewBag.IsAllowToAppRej = false;
            if (model.MenuViewModel.EnquiryPipeline == "Costing Approval Pipeline")
            {
                if (UserRoles.Contains("Product Accountant"))
                {
                    ViewBag.IsAllowToAppRej = true;
                }
            }

            return PartialView("_Commercials", model);
        }

        [HttpPost]
        public JsonResult ChangePositionKeepAs(string QRFID, string ChangeType, List<string> PositionIds)
        {
            CommonResponse response  = new CommonResponse();
            ChangePositionKeepReq request = new ChangePositionKeepReq();
            request.QRFID = QRFID;
            request.ChangeType = ChangeType;
            request.PositionIds = PositionIds;
            request.EditUser = ckUserEmailId;
            response = coProviders.ChangePositionKeepAs(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        [HttpPost]
        public JsonResult SaveCommercials(string QRFPriceId, double PercentSoldOptional,string QRFID = null)
        {
            CommonResponse response = new CommonResponse();
            CommercialsSetReq request = new CommercialsSetReq();
            request.QRFPriceId = QRFPriceId;
            request.PercentSoldOptional = PercentSoldOptional;
            request.EditUser = ckUserEmailId;
            request.VoyagerUserId = ckLoginUser_Id;
            request.QrfId = QRFID;
            response = coProviders.SaveCommercials(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        [HttpPost]
        public IActionResult SubmitCommercial(string QRFID, string Remarks, string EnquiryPipeline, bool IsApproveQuote, string Officer)
        {
            try
            {
                COCommonLibrary commonLibrary = new COCommonLibrary(_configuration);
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["UserName"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;

                CommonResponse objResponse = commonLibrary.UpdateQuoteDetails(_configuration, token, QRFID, Remarks, EnquiryPipeline, IsApproveQuote, Officer, objCookies, sessionData);

                if (objResponse.ResponseStatus.Status.ToLower() == "success")
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
                return View();
            }
        }

    }
}