using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Mappers;
using Voyager.App.Models;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class TourEntitiesController : VoyagerController
    {
        #region declaration
        private readonly IConfiguration _configuration;
        private PositionMapping positionMapping;
        private TourEntitiesViewModel model = new TourEntitiesViewModel();
        #endregion

        public TourEntitiesController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionMapping = new PositionMapping(_configuration);
        }

        public IActionResult TourEntities()
        {
            model = new TourEntitiesViewModel();
            try
            {
                TourEntitiesParam tourEntitiesParam = new TourEntitiesParam();
                tourEntitiesParam.QRFID = Request.Query["QRFId"];
                model = GetDynamicTourEntities(tourEntitiesParam);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(model);
        }

        public IActionResult GetTourEntities(TourEntitiesParam tourEntitiesParam)
        {
            model = new TourEntitiesViewModel();

            if (!string.IsNullOrEmpty(tourEntitiesParam.TourType) && tourEntitiesParam.TourType == "TE")
            {
                model = GetDynamicTourEntities(tourEntitiesParam);
                model = model != null ? model : new TourEntitiesViewModel();
                model.DynamicTourEntity = model.DynamicTourEntity != null && model.DynamicTourEntity.Count > 0 ? model.DynamicTourEntity : new List<DynamicTourEntity>();
                return PartialView("_TourEntities", model);
            }
            else
            {
                TourEntitiesGetReq tourEntitiesGetReq = new TourEntitiesGetReq() { QRFID = tourEntitiesParam.QRFID, Type = tourEntitiesParam.Type, PositionID = tourEntitiesParam.PositionID };
                model = positionMapping.GetTourEntities(_configuration, token, tourEntitiesGetReq);

                if (model != null && model.PaxSlabDetails != null && model.PaxSlabDetails.PaxSlabs.Count > 0)
                {
                    var tourentitycnt = model.TourEntities.GroupBy(a => new { a.Type, a.PaxSlab, a.PositionID }).Select(m => new { m.Key.Type, m.Key.PaxSlab, m.Key.PositionID, TECount = m.Count() });

                    return Json(new { PositionNotExists = model.PositionNotExists, PaxSlabs = model.PaxSlabDetails.PaxSlabs, TourEntity = model.TourEntities, QRFId = model.QRFID, TourEntityCnt = tourentitycnt, ResponseStatus = model.ResponseStatus, RoomList = model.AutoCompleteTextBox });
                }
                else
                {
                    return Json(new { PositionNotExists = new List<string>(), PaxSlabs = new PaxSlabs(), TourEntities = new TourEntities(), QRFId = model.QRFID, TourEntityCnt = "", ResponseStatus = model.ResponseStatus, RoomList = new List<AutoCompleteTextBox>() });
                }
            }
        }

        [HttpPost]
        public IActionResult TourEntities(TourEntitiesParam tourEntitiesParam)
        {
            string UserName = ckUserEmailId;
            TourEntitiesSetRes response = new TourEntitiesSetRes() { TourEntities = tourEntitiesParam.TourEntities };
            try
            {
                var tourentities = tourEntitiesParam.TourEntities.Where(a => !string.IsNullOrEmpty(a.RoomTypeID)).ToList();

                TourEntitiesSetReq tourEntitiesSetReq = new TourEntitiesSetReq() { UserName = UserName, TourEntities = tourentities, QRFID = tourEntitiesParam.QRFID,VoyagerUserID = ckLoginUser_Id };
                response = positionMapping.SetTourEntities(_configuration, token, tourEntitiesSetReq);
            }
            catch (Exception ex)
            {
                throw;
            }
            if (response != null && response.ResponseStatus.Status.ToLower() == "success" && response.TourEntities != null && response.TourEntities.Count > 0)
            {
                return Json(new { PaxSlabs = response.TourEntities, TourEntity = response.TourEntities, QRFId = tourEntitiesParam.QRFID, ResponseStatus = response.ResponseStatus });
            }
            else
            {
                return Json(new { PaxSlabs = new PaxSlabs(), TourEntities = new TourEntities(), QRFId = tourEntitiesParam.QRFID, ResponseStatus = response.ResponseStatus });
            }
        }

        public IActionResult GetTourEntitiesAllowances(TourEntitiesParam tourEntitiesParam)
        {
            model = new TourEntitiesViewModel();
            try
            {
                model = GetDynamicTourEntities(tourEntitiesParam);
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_TourEntities_Allowances", model);
        }

        public TourEntitiesViewModel GetDynamicTourEntities(TourEntitiesParam tourEntitiesParam)
        {
            model = new TourEntitiesViewModel();
            try
            {
                TourEntitiesGetReq tourEntitiesGetReq = new TourEntitiesGetReq() { QRFID = tourEntitiesParam.QRFID, GetType = tourEntitiesParam.TourType };
                model = positionMapping.GetDynamicTourEntities(_configuration, token, tourEntitiesGetReq);
            }
            catch (Exception ex)
            {
                throw;
            }
            return model;
        }
    }
}