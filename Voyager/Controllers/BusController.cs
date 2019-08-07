using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Library;
using Microsoft.Extensions.Configuration;
using VGER_WAPI_CLASSES;
using Voyager.App.Mappers;

namespace Voyager.Controllers
{
    [Authorize]
    public class BusController : VoyagerController
    {
        #region declaration
        private readonly IConfiguration _configuration;
        PositionMapping positionMapping;
        #endregion

        public BusController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Bus()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            string SaveType = Convert.ToString(Request.Query["SaveType"]);
            string PositionId = Request.Query["PositionId"];
            bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);
            BusViewModel model = new BusViewModel();
            try
            {
                List<ProductType> lst = new List<ProductType>();
                lst.Add(new ProductType { ProdType = "LDC" });
                lst.Add(new ProductType { ProdType = "Coach" });
                PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = QRFID, ProductType = lst, PositionId = PositionId, Type = "bus", IsClone = IsClone };

                model = positionMapping.GetBusDetails(_configuration, token, objPositionGetReq);

                model.MenuViewModel.PositionId = PositionId;

                if (string.IsNullOrEmpty(PositionId))
                    if (!string.IsNullOrEmpty(SaveType) && SaveType.ToLower() == "full")
                    {
                        model.SaveType = "success";
                        return PartialView("_Bus", model);
                    }
                    else
                        return View(model);
                else
                    return PartialView("_Bus", model);

            }
            catch (Exception ex)
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Bus(BusViewModel model)
        {
            model.VoyagerUserId = ckLoginUser_Id;
            PositionSetRes objPositionSetRes = positionMapping.SetBusDetals(model, ckUserEmailId, token);
            if (model.SaveType == "partial")
            {
                var objPosition = objPositionSetRes.mPosition.FirstOrDefault();
                if (objPosition != null)
                {
                    return Json(new { objPositionSetRes.ResponseStatus.Status, PositionId = objPosition.PositionId, RoomDetailsInfo = objPosition.RoomDetailsInfo });
                }
                else
                {
                    return Json(new { objPositionSetRes.ResponseStatus.Status, PositionId = "", RoomDetailsInfo = "" });
                }
            }
            else
            {
                if (objPositionSetRes.ResponseStatus.Status.ToLower() == "success")
                    TempData["success"] = "Bus " + objPositionSetRes.ResponseStatus.ErrorMessage;
                else
                    TempData["error"] = objPositionSetRes.ResponseStatus.ErrorMessage;

                if (model.SaveType == "full")
                {
                    return RedirectToAction("Bus", new { QRFId = model.QRFID, model.SaveType }); 
                }
                else
                {
                    model.SaveType = "full";
                    if (model.BusDetails.Count == 1)
                        return RedirectToAction("Bus", new { QRFId = model.QRFID, model.SaveType, PositionId = model.BusDetails[0].BusID, IsClone=model.MenuViewModel.IsClone });
                    else
                        return RedirectToAction("Bus", new { QRFId = model.QRFID, model.SaveType, IsClone = model.MenuViewModel.IsClone });
                }
            }
        }
    }
}