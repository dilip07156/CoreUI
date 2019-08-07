
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Contracts;
using Voyager.App.Library;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VGER_WAPI_CLASSES;
using Voyager.App.Mappers;

namespace Voyager.Controllers
{
    [Authorize]
    public class ActivitiesController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        PositionProviders positionProviders;
        PositionMapping positionMapping;

        public ActivitiesController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionProviders = new PositionProviders(_configuration);
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Activities()
        {
            ActivitiesViewModel model = new ActivitiesViewModel();
            try
            {
                string QRFID = Request.Query["QRFId"].ToString();
                string SaveType = Request.Query["SaveType"].ToString();
                string PositionId = Request.Query["PositionId"];
                bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);
                model.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;
                model.MenuViewModel.PositionId = PositionId;
                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID };
                if (!string.IsNullOrEmpty(PositionId) && PositionId.Substring(0, 8) != "00000000") positionGetReq.PositionId = PositionId;
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Attractions" });
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Sightseeing - CityTour" });
                positionGetReq.IsClone = IsClone;

                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
                positionMapping.ActivitiesGet(positionGetRes, null, ref model, token);

                if (string.IsNullOrEmpty(PositionId))
                    if (!string.IsNullOrEmpty(SaveType) && SaveType.ToLower() == "full")
                    {
                        model.SaveType = "success";
                        return PartialView("_Activities", model);
                    }
                    else
                        return View(model);
                else
                    return PartialView("_Activities", model);
            }
            catch (Exception ex)
            {
                throw;
                Console.WriteLine(ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Activities(ActivitiesViewModel model)
        {
            string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            SalesQuoteLibrary salesLibrary = new SalesQuoteLibrary(_configuration);
            PositionSetReq positionSetReq = new PositionSetReq();
            bool isDuplicate = CheckDuplicateActivity(model);
            if (isDuplicate)
            {
                TempData["error"] = "Can not save same Activity for same day and time!!!";
                model.SaveType = "full";
                //return PartialView("_Activities", model);
                return RedirectToAction("Activities", new { model.QRFID, model.SaveType, model.MenuViewModel.IsClone });
            }
            else
            {
                positionSetReq = positionMapping.ActivitiesSet(model, ckUserEmailId);
                positionSetReq.QRFID = model.QRFID;
                positionSetReq.VoyagerUserID = ckLoginUser_Id;
                positionSetReq.FOC = model.FOC;
                positionSetReq.Price = model.Price;
                PositionSetRes positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;

                if (!string.IsNullOrEmpty(model.SaveType) && model.SaveType.ToLower() == "partial")
                {
                    if (positionSetRes.mPosition.Count > 0)
                        return Json(new { positionSetRes.ResponseStatus.Status, positionSetRes.mPosition[0].PositionId, positionSetRes.mPosition[0].RoomDetailsInfo });
                    else
                        return Json(new { positionSetRes.ResponseStatus.Status });
                }
                else
                {
                    if (positionSetRes.ResponseStatus.Status.ToLower() == "success") TempData["success"] = "Activities saved successfully";
                    else TempData["error"] = positionSetRes.ResponseStatus.Status;

                    model.SaveType = "full";
                    if (model.ActivityDetails.Count == 1)
                        return RedirectToAction("Activities", new { model.QRFID, model.SaveType, PositionId = model.ActivityDetails[0].ActivityId, IsClone = model.MenuViewModel.IsClone });
                    else
                        return RedirectToAction("Activities", new { model.QRFID, model.SaveType, IsClone = model.MenuViewModel.IsClone });
                }
            }
        }

        private bool CheckDuplicateActivity(ActivitiesViewModel model)
        {
            var dayList = model.ActivityDetails.Select(a => a.ActivityDayNo).ToList();
            bool isDuplicate = false; int activityCnt = 0;
            foreach (var day in dayList)
            {
                var timeList = model.ActivityDetails.Where(a => a.ActivityDayNo == day).ToList();
                foreach (var time in timeList)
                {
                    activityCnt = model.ActivityDetails.Where(a => a.ActivityDayNo == day && a.StartTime == time.StartTime && a.ProductID == time.ProductID).Count();
                    if (activityCnt > 1)
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                if (isDuplicate) break;
            }

            return isDuplicate;
        }

        public IActionResult GetQuickPickActivities()
        {
            ActivitiesViewModel model = new ActivitiesViewModel();
            try
            {
                string QRFID = Request.Query["QRFId"].ToString();
                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID };
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Attractions" });
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Sightseeing - CityTour" });
                PosQuicePickGetRes QuickPickGetRes = positionProviders.GetQuickPickActivities(positionGetReq, token).Result;
                positionMapping.ActivitiesGet(null, QuickPickGetRes, ref model, token);
            }
            catch (Exception)
            { throw; }
            return PartialView("_ActivitiesQuickPickList", model);
        }

        [HttpPost]
        public IActionResult SaveQuickPickActivities(ActivitiesViewModel model)
        {
            PositionSetReq positionSetReq = new PositionSetReq();
            string UserName = HttpContext.Request.Cookies["UserName"] ?? ckUserName;
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            SalesProviders salesProviders = new SalesProviders(_configuration);
            RoutingDaysGetRes routingDaysRes = salesProviders.GetQRFRoutingDays(new RoutingDaysGetReq { QRFID = model.QRFID }, token).Result;
            PositionGetReq positionGetReq = new PositionGetReq { QRFID = model.QRFID };
            positionGetReq.ProductType.Add(new ProductType { ProdType = "Attractions" });
            positionGetReq.ProductType.Add(new ProductType { ProdType = "Sightseeing - CityTour" });
            PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;
            positionMapping.ActivitiesGet(positionGetRes, null, ref model, token);

            positionSetReq = positionMapping.QuickPickActivitiesSet(model, routingDaysRes.RoutingDays, ckUserEmailId, positionGetRes);
            positionSetReq.QRFID = model.QRFID;
            positionSetReq.VoyagerUserID = ckLoginUser_Id;
            PositionSetRes positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;

            model.SaveType = "full";
            return RedirectToAction("Activities", new { model.QRFID, model.SaveType, IsClone = model.MenuViewModel.IsClone });
        }

        public JsonResult GetProductName(string prodName, string city, string prodType)
        {
            string[] City = { "", "" };
            if (!string.IsNullOrEmpty(city) && city.Contains(',')) City = city.Split(',');
            ProductSearchReq objProductSearchReq = new ProductSearchReq
            {
                ProdName = prodName,
                CityName = City[0].Trim(),
                CountryName = City[1].Trim(),
                ProdType = string.IsNullOrEmpty(prodType) ? "".Split(',').ToList() : prodType.Split(",").ToList()
            };

            //  if (objProductSearchReq.ProdName.Length >= 3 && objProductSearchReq.ProdName.Substring(0, 3) == "###") objProductSearchReq.ProdName = "";

            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProductSearchRes objProductSearchRes = objMasterProviders.GetProductDetailsBySearchCriteria(objProductSearchReq, token).Result;

            if (objProductSearchRes.ResponseStatus.Status == "Success" && objProductSearchRes.ProductSearchDetails.Count > 0)
            {
                List<AutoCompleteTextBox> ProductList = new List<AutoCompleteTextBox>();
                ProductList = objProductSearchRes.ProductSearchDetails.Select(data => new AutoCompleteTextBox { value = data.VoyagerProduct_Id, label = data.ProdName, type = data.ProdType, typeId = data.ProdTypeId }).ToList();
                return Json(ProductList);
            }
            else
                return Json("");
        }

        public JsonResult GetProductCategory(string ProductId)
        {
            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            ProductCatGetReq objProdCatGetReq = new ProductCatGetReq { ProductId = ProductId };
            ProductCatGetRes objMasterTypeResponse = objMasterProviders.GetProductCategoryByParam(objProdCatGetReq, token).Result;

            if (objMasterTypeResponse.ResponseStatus.Status == "Success" && objMasterTypeResponse.ProdCategoryDetails.Count > 0)
            {
                List<AutoCompleteTextBox> CategoryList = new List<AutoCompleteTextBox>();
                CategoryList = objMasterTypeResponse.ProdCategoryDetails.Select(data => new AutoCompleteTextBox { value = data.ProductCategoryId, label = data.ProductCategoryName }).ToList();
                return Json(CategoryList);
            }
            else
                return Json("");
        }

        public JsonResult GetProductRangeList(string QRFId, string ProductId, string CategoryId, string AdditionalYN)
        {
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            ProductRangeGetRes prodRangeGetRes = quoteLibrary.GetProductRangeList(ProductId, CategoryId, AdditionalYN, token, string.IsNullOrEmpty(QRFId) ? "" : QRFId);

            List<AutoCompleteTextBox> prodRangeList = new List<AutoCompleteTextBox>();
            List<AutoCompleteTextBox> defProdRangeList = new List<AutoCompleteTextBox>();
            if (prodRangeGetRes.ResponseStatus.Status.ToLower() == "success" && prodRangeGetRes.ProductRangeDetails.Count > 0)
            {
                prodRangeList = prodRangeGetRes.ProductRangeDetails.Select(a => new AutoCompleteTextBox { value = a.VoyagerProductRange_Id, label = a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ")", type = a.AdditionalYN.ToString() }).OrderBy(a => a.type).ThenBy(a => a.label).ToList();
                if (prodRangeGetRes.DefProdRangelist != null && prodRangeGetRes.DefProdRangelist.Count > 0)
                {
                    defProdRangeList = prodRangeGetRes.DefProdRangelist.Select(a => new AutoCompleteTextBox { value = a.VoyagerProductRange_Id, label = a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ")", type = a.AdditionalYN.ToString() }).OrderBy(a => a.type).ThenBy(a => a.label).ToList();
                }
                return Json(new { prodRangeGetRes.SupplierId, prodRangeGetRes.SupplierName, prodRangeList, defProdRangeList });
            }
            else
                return Json("");
        }
    }
}