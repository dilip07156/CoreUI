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
    public class RailController : VoyagerController
    {
        private readonly IConfiguration _configuration;
        PositionProviders positionProviders;
        PositionMapping positionMapping;

        public RailController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionProviders = new PositionProviders(_configuration);
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Rail()
        {
            RailViewModel model = new RailViewModel();
            try
            {
                string QRFID = Request.Query["QRFId"].ToString();
                string SaveType = Request.Query["SaveType"].ToString();
                string PositionId = Request.Query["PositionId"];
                bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);

                model.QRFID = QRFID;
                model.MenuViewModel.QRFID = QRFID;
                model.MenuViewModel.PositionId = PositionId;

                PositionGetReq positionGetReq = new PositionGetReq { QRFID = QRFID, IsClone=IsClone };
                if (!string.IsNullOrEmpty(PositionId) && PositionId.Substring(0, 8) != "00000000") positionGetReq.PositionId = PositionId;
                positionGetReq.ProductType.Add(new ProductType { ProdType = "Train" });
                PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

                positionMapping.RailGet(positionGetRes, ref model, token);
                if (string.IsNullOrEmpty(PositionId))
                    if (!string.IsNullOrEmpty(SaveType) && SaveType.ToLower() == "full")
                    {
                        model.SaveType = "success";
                        return PartialView("_Rail", model);
                    }
                    else
                        return View(model);
                else
                    return PartialView("_Rail", model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Rail(RailViewModel model)
        {   
            
            IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
            SalesQuoteLibrary salesLibrary = new SalesQuoteLibrary(_configuration);
            PositionSetReq positionSetReq = new PositionSetReq();            
            positionSetReq = positionMapping.RailSet(model, ckUserEmailId);
            positionSetReq.FOC = model.FOC;
            positionSetReq.Price = model.Price;
            positionSetReq.QRFID = model.QRFID;
            positionSetReq.VoyagerUserID = ckLoginUser_Id;
            PositionSetRes positionSetRes = positionProviders.SetPosition(positionSetReq, token).Result;

            if (!string.IsNullOrEmpty(model.SaveType) && model.SaveType.ToLower() == "partial" && positionSetRes?.mPosition != null)
            {
                if (positionSetRes.mPosition.Count > 0)
                    return Json(new { positionSetRes.ResponseStatus.Status, positionSetRes.mPosition[0].PositionId, positionSetRes.mPosition[0].RoomDetailsInfo });
                else
                    return Json(new { positionSetRes.ResponseStatus.Status });
            }
            else
            {
                if (positionSetRes?.ResponseStatus?.Status?.ToLower() == "success") TempData["success"] = "Rail details saved successfully";
                else if (!string.IsNullOrEmpty(positionSetRes?.ResponseStatus?.Status)) TempData["error"] = positionSetRes?.ResponseStatus?.Status;
                else TempData["error"] = "Error occured";

                model.SaveType = "full";
                if (model.RailDetails.Count == 1)
                    return RedirectToAction("Rail", new { model.QRFID, model.SaveType, PositionId = model.RailDetails[0].RailId, model.MenuViewModel.IsClone });
                else
                    return RedirectToAction("Rail", new { model.QRFID, model.SaveType, model.MenuViewModel.IsClone });
            }
        }
    }
}