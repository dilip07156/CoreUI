using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class OthersController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        private PositionProviders positionProviders;
        private PositionMapping positionMapping;

        public OthersController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionProviders = new PositionProviders(_configuration);
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Others()
        {
            OthersViewModel model = new OthersViewModel();
            try
            {
                string ReturnView = "";
                string QRFID = Request.Query["QRFId"].ToString();
                string SaveType = Request.Query["SaveType"].ToString();
                string PositionId = Request.Query["PositionId"];
                string ProductType = Request.Query["ProductType"];
                bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);
                model.OthersLocalGuide.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;
                model.MenuViewModel.PositionId = PositionId;
                if (ProductType.ToLower() == "guide")
                {
                    model.MenuViewModel.MenuName = "LocalGuide";
                }
                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID, IsClone = IsClone };
                if (!string.IsNullOrEmpty(PositionId) && PositionId.Substring(0, 8) != "00000000") positionGetReq.PositionId = PositionId;
                else
                {
                    if (string.IsNullOrEmpty(ProductType) || ProductType.ToLower() == "guide")
                    {
                        positionGetReq.ProductType.Add(new ProductType { ProdType = "Guide" });
                        positionGetReq.ProductType.Add(new ProductType { ProdType = "Assistant" });
                    }
                    else if (!string.IsNullOrWhiteSpace(ProductType) && (ProductType.ToLower() == "other" || ProductType.ToLower() == "fee"))
                    {
                        positionGetReq.ProductType.Add(new ProductType { ProdType = "Other" });
                        positionGetReq.ProductType.Add(new ProductType { ProdType = "Fee" });
                    }
                    else
                    {
                        positionGetReq.ProductType.Add(new ProductType { ProdType = ProductType });
                    }
                }

                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = model.MenuViewModel.QRFID };
                bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

                positionMapping.OthersLocalGuideGet(positionGetRes, ref model, token);

                switch (string.IsNullOrEmpty(ProductType) ? "" : ProductType.ToLower())
                {
                    case "guide": ReturnView = "_OthersLocalGuide"; break;
                    case "visa": ReturnView = "_OthersVisa"; break;
                    case "insurance": ReturnView = "_OthersInsurance"; break;
                    case "other": ReturnView = "_OthersMiscellaneous"; break;
                    default: ReturnView = "_OthersLocalGuide"; break;
                }

                if (string.IsNullOrEmpty(PositionId) || PositionId.Substring(0, 8) == "00000000")
                    if (!string.IsNullOrEmpty(SaveType) && SaveType.ToLower() == "full")
                    {
                        model.OthersLocalGuide.SaveType = "success";
                        return PartialView(ReturnView, model.OthersLocalGuide);
                    }
                    else
                        return View(model);
                else
                    return PartialView(ReturnView, model.OthersLocalGuide);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Others(OthersViewModel model)
        {
            return RedirectToAction("Others", new { model.OthersVisa.QRFID });
        }

        #region Loading Submenus
        public IActionResult GetVisa(string QRFID)
        {
            OthersViewModel model = new OthersViewModel();
            try
            {
                model.OthersLocalGuide.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;

                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID };
                positionGetReq.ProductType.Add(new ProductType { ProdType = "VISA" });
                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
                positionMapping.OthersLocalGuideGet(positionGetRes, ref model, token, "VISA");

                return PartialView("_OthersVisa", model.OthersLocalGuide);
            }
            catch (Exception ex)
            {
                return PartialView("_OthersVisa", model.OthersLocalGuide);
            }
        }

        public IActionResult GetInsurance(string QRFID)
        {
            OthersViewModel model = new OthersViewModel();
            try
            {
                model.OthersLocalGuide.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;

                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID };
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Insurance" });
                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
                positionMapping.OthersLocalGuideGet(positionGetRes, ref model, token, "Insurance");

                return PartialView("_OthersInsurance", model.OthersLocalGuide);
            }
            catch (Exception ex)
            {
                return PartialView("_OthersInsurance", model.OthersLocalGuide);
            }
        }

        public IActionResult GetMiscellaneous(string QRFID)
        {
            OthersViewModel model = new OthersViewModel();
            try
            {
                model.OthersMiscellaneous.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;

                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID };
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Other" });
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Fee" });
                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
                positionMapping.OthersLocalGuideGet(positionGetRes, ref model, token, "QRF_Id");

                return PartialView("_OthersMiscellaneous", model.OthersLocalGuide);
            }
            catch (Exception ex)
            {
                return PartialView("_OthersMiscellaneous", model.OthersLocalGuide);
            }
        }

        public IActionResult GetGift(string QRFId)
        {
            OthersViewModel model = new OthersViewModel();
            try
            {
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView("_OthersGift");
        }
        #endregion

        #region Saving Submenus
        [HttpPost]
        public IActionResult LocalGuide(OthersLocalGuide model)
        {
            string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            SalesQuoteLibrary salesLibrary = new SalesQuoteLibrary(_configuration);
            PositionSetRes positionSetRes = new PositionSetRes();
            PositionSetReq positionSetReq = new PositionSetReq();
            OthersViewModel othersViewModel = new OthersViewModel { OthersLocalGuide = model, MenuViewModel = new MenuViewModel { IsClone = model.IsClone } };

            if (model.LocalGuideDetails.Count > 0)
            {
                positionSetReq = positionMapping.OthersLocalGuideSet(othersViewModel, ckUserEmailId);
                positionSetReq.FOC = model.FOC;
                positionSetReq.Price = model.Price;
                positionSetReq.QRFID = model.QRFID;
                positionSetReq.VoyagerUserID = ckLoginUser_Id;
                positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;
            }

            if (!string.IsNullOrEmpty(model.SaveType) && model.SaveType.ToLower() == "partial")
            {
                if (positionSetRes.mPosition.Count > 0)
                    return Json(new { positionSetRes.ResponseStatus.Status, positionSetRes.mPosition[0].PositionId, positionSetRes.mPosition[0].RoomDetailsInfo });
                else
                    return Json(new { positionSetRes.ResponseStatus.Status });
            }
            else
            {
                if (positionSetRes.ResponseStatus.Status.ToLower() == "success") TempData["success"] = "Data saved successfully";
                else TempData["error"] = positionSetRes.ResponseStatus.Status;

                model.SaveType = "full";
                if (model.LocalGuideDetails.Count == 1)
                {
                    var prodtype = model.LocalGuideDetails[0].ProductType == "Assistant" ? "Guide" : model.LocalGuideDetails[0].ProductType == "Fee" ? "Other" : model.LocalGuideDetails[0].ProductType;
                    return RedirectToAction("Others", new { model.QRFID, model.SaveType, PositionId = model.LocalGuideDetails[0].OthersId, ProductType = prodtype });
                }
                else
                {
                    if (model.LocalGuideDetails.Count > 0)
                    {
                        var prodtype = model.LocalGuideDetails[0].ProductType == "Assistant" ? "Guide" : model.LocalGuideDetails[0].ProductType == "Fee" ? "Other" : model.LocalGuideDetails[0].ProductType;
                        return RedirectToAction("Others", new { model.QRFID, model.SaveType, ProductType = prodtype });
                    }
                    else
                        return RedirectToAction("Others", new { model.QRFID, model.SaveType });
                }
            }
        }
        #endregion
    }
}