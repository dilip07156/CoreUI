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
    public class TransfersController : VoyagerController
    {
        #region declaration
        private readonly IConfiguration _configuration;
        PositionMapping positionMapping;
        private string SessionName = "IntegrationInfo";
        #endregion

        public TransfersController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult Transfers()
        {
            string QRFID = Request.Query["QRFId"].ToString();
            TransfersViewModel model = new TransfersViewModel { QRFID = QRFID };
            try
            {
                List<ProductType> lst = positionMapping.GetProductType("transfer");
                PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = QRFID, ProductType = lst, PositionId = null,Type="transfer" };

                model = positionMapping.GetTransfersDetails(_configuration, token, objPositionGetReq,"");
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Transfers(TransfersViewModel model)
        { 
            if (model.SaveType == "partial")
            {
                model.VoyagerUserId = ckLoginUser_Id;
                PositionSetRes objPositionSetRes = positionMapping.SetTransfersDetails(model, ckUserName, token);
                var objPosition = objPositionSetRes.mPosition.FirstOrDefault();
                if (objPosition != null)
                {
                    return Json(new { objPositionSetRes.ResponseStatus, QRFID = objPosition.QRFID, PositionId = objPosition.PositionId, RoomDetailsInfo = objPosition.RoomDetailsInfo });
                }
                else
                {
                    return Json(new { objPositionSetRes.ResponseStatus, QRFID = objPosition.QRFID, PositionId = objPosition.PositionId, RoomDetailsInfo = new List<RoomDetailsInfo>() });
                }
            }
            else
            {
                model.VoyagerUserId = ckLoginUser_Id;
                IRequestCookieCollection objCookies = HttpContext.Request.Cookies;
                var sessionData = string.IsNullOrEmpty(HttpContext.Request.Cookies["VoyagerUser_Id"]) ? HttpContext.Session.GetComplexData<List<ProductAttributeDetails>>(SessionName) : null;
                PositionSetRes objPositionSetRes = positionMapping.SetAllTransfersDetails(_configuration, token, model, objCookies, sessionData);
                objPositionSetRes.ResponseStatus.ErrorMessage = "Transfer Details " + objPositionSetRes.ResponseStatus.ErrorMessage;
                return Json(new { objPositionSetRes.ResponseStatus });
            }
        }

        public IActionResult GetTransfersType(PositionGetParam objGetParam,bool IsClone=false)
        {
            TransferPopup transferPopup = new TransferPopup();
            transferPopup.TransferProperties = new TransferProperties();
            try
            {
                objGetParam.PositionId = string.IsNullOrEmpty(objGetParam.PositionId) ? "0" : objGetParam.PositionId;

                List<ProductType> lst = positionMapping.GetProductType("transfer");
                PositionGetReq objPositionGetReq = new PositionGetReq() { QRFID = objGetParam.QRFID, ProductType = lst, PositionId = objGetParam.PositionId,IsClone=IsClone };
                transferPopup = positionMapping.GetTransfersDetails(_configuration, token, objPositionGetReq, objGetParam.Day).TransferPopup;
                transferPopup.TransferProperties.QRFID = objGetParam.QRFID;
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_TransferDetails", transferPopup);
        }
    }
}