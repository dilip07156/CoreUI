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
    public class CruiseController : VoyagerController
    {
        #region declaration
        private readonly IConfiguration _configuration;
        PositionMapping positionMapping;
        #endregion

        public CruiseController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Cruise()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            string SaveType = Convert.ToString(Request.Query["SaveType"]);
            string PositionId = Request.Query["PositionId"];
            bool IsClone = Convert.ToBoolean(Request.Query["IsClone"]);
            CruiseViewModel model = new CruiseViewModel();
            try
            {
                List<ProductType> lst = new List<ProductType>();
                lst.Add(new ProductType { ProdType = "Overnight Ferry" }); 
                PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = QRFID, ProductType = lst, PositionId = PositionId,IsClone=IsClone };

                model = positionMapping.GetCruiseDetails(_configuration, token, objPositionGetReq);
                model.MenuViewModel.PositionId = PositionId;
                if (string.IsNullOrEmpty(PositionId))
                    if (!string.IsNullOrEmpty(SaveType) && SaveType.ToLower() == "full")
                    {
                        model.SaveType = "success";
                        return PartialView("_Cruise", model);
                    }
                    else
                        return View(model);
                else
                    return PartialView("_Cruise", model);

            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Cruise(CruiseViewModel model)
        {
            model.VoyagerUserId = ckLoginUser_Id;
            PositionSetRes objPositionSetRes = positionMapping.SetCruiseDetals(model, ckUserEmailId, token);
            if (model.SaveType == "partial")
            {
                var objPosition = objPositionSetRes.mPosition.FirstOrDefault();
                return Json(new { objPositionSetRes.ResponseStatus.Status, PositionId = objPosition.PositionId, RoomDetailsInfo = objPosition.RoomDetailsInfo });
            }
            else
            {
                if (objPositionSetRes.ResponseStatus.Status.ToLower() == "success")
                    TempData["success"] = "Cruise " + objPositionSetRes.ResponseStatus.ErrorMessage;
                else
                    TempData["error"] = objPositionSetRes.ResponseStatus.ErrorMessage;

                model.SaveType = "full";
                if (model.CruiseDetails.Count == 1)
                    return RedirectToAction("Cruise", new { QRFId = model.QRFID, model.SaveType, PositionId = model.CruiseDetails[0].PositionId, IsClone = model.MenuViewModel.IsClone });
                else
                    return RedirectToAction("Cruise", new { QRFId = model.QRFID, model.SaveType, IsClone = model.MenuViewModel.IsClone });
            }             
        }
    }
}