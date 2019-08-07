using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Mappers;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class HandoverController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        private COProviders coProviders;
        private HandoverMapping handoverMapping;
        private ItineraryViewModel model = new ItineraryViewModel();
        #endregion

        public HandoverController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            coProviders = new COProviders(_configuration);
            handoverMapping = new HandoverMapping(_configuration);
        }

        #region Itinerary
        [Route("Handover/Itinerary")]
        public IActionResult Itinerary(string QRFID)
        {
            ItineraryViewModel model = new ItineraryViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Itinerary";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            return View("Itinerary/Itinerary", model);
        }
        #endregion

        #region AttachToMaster
        [Route("Handover/AttachToMaster")]
        public IActionResult AttachToMaster(string QRFID, string type = "")
        {
            AttachToMasterViewModel model = new AttachToMasterViewModel();
            model = handoverMapping.GetGoAhead(token, new GoAheadGetReq { QRFID = QRFID, GoAheadId = "" });
            if (!string.IsNullOrEmpty(type) && type == "partial")
            {
                return PartialView("AttachToMaster/_AttachToMaster", model);
            }

            return View("AttachToMaster/AttachToMaster", model);
        }

        public IActionResult GetGoAheadDepature(string QRFID, string GoAheadId, long depatureid)
        {
            Materialisation model = new Materialisation();
            if (depatureid > 0)
            {
                model = handoverMapping.GetGoAheadDepature(token, new GoAheadGetReq { QRFID = QRFID, GoAheadId = GoAheadId, DepatureId = depatureid });
                model.GoAheadId = GoAheadId;
                model.QRFID = QRFID;
                model.DepartureId = depatureid;
            }
            return PartialView("AttachToMaster/_Materialisation", model);
        }

        [HttpPost]
        public IActionResult SetAttachToMaster(AttachToMasterViewModel model)
        {
            model.GoAheadGetRes.mGoAhead.CreateUser = ckUserName;
            model.GoAheadGetRes.mGoAhead.EditUser = ckUserName;
            string type = model.Type;

            GoAheadSetRes objGoAheadSetRes = handoverMapping.SetGoAhead(model, ckUserEmailId, token, ckLoginUser_Id);

            if (!string.IsNullOrEmpty(model.Type) && type == "partial")
            {
                return Json(new
                {
                    status = objGoAheadSetRes.ResponseStatus.Status,
                    msg = objGoAheadSetRes.ResponseStatus.ErrorMessage,
                    goaheadid = objGoAheadSetRes.mGoAhead.GoAheadId
                });
                //objGoAheadSetRes.mGoAhead.Depatures = null;
                //objGoAheadSetRes.mGoAhead.Depatures.Add(depres);
                //return PartialView("_Materialisation", objGoAheadSetRes.mGoAhead);
            }
            else
            {
                if (objGoAheadSetRes != null && objGoAheadSetRes.ResponseStatus != null && objGoAheadSetRes.ResponseStatus.Status.ToLower() == "success")
                {
                    //GoAheadSetRes objGoAheadSetResMail = handoverMapping.SendMailHandoverBooking(new GoAheadGetReq { GoAheadId = model.GoAheadGetRes.mGoAhead.GoAheadId, QRFID = model.GoAheadGetRes.mGoAhead.QRFID }, token);

                    TempData["success"] = objGoAheadSetRes.ResponseStatus.ErrorMessage;
                }
                else
                {
                    TempData["error"] = objGoAheadSetRes.ResponseStatus.ErrorMessage;
                }
                //var Status = "";
                //var Msg = "";
                //if (objGoAheadSetResMail != null && objGoAheadSetResMail.ResponseStatus != null)
                //{
                //    Status = objGoAheadSetResMail.ResponseStatus.Status;
                //    Msg = objGoAheadSetResMail.ResponseStatus.ErrorMessage;
                //}
                //else
                //{
                //    Status = "Error";
                //    Msg = "An Error Occurs.";
                //}
                return RedirectToAction("AttachToMaster", new { QRFId = model.GoAheadGetRes.mGoAhead.QRFID });
            }

        }

        [HttpPost]
        public IActionResult SetMaterialisation(Materialisation model)
        {
            model.UserName = ckUserName;
            if (model.ChildInfo != null && model.ChildInfo.Count > 0)
            {
                model.ChildInfo.RemoveAll(a => a.IsDeleted == true && (a.ChildInfoId == "0" || string.IsNullOrEmpty(a.ChildInfoId)));
            }

            SetMaterialisationRes objSetSetMaterialisationRes = handoverMapping.SetMaterialisation(model, token);
            return Json(new
            {
                status = objSetSetMaterialisationRes.ResponseStatus.Status,
                msg = objSetSetMaterialisationRes.ResponseStatus.ErrorMessage
            });
        }
        #endregion

        #region Handover
        [Route("Handover/Handover")]
        public IActionResult Handover(string QRFID)
        {
            HandoverViewModel model = new HandoverViewModel();
            model = handoverMapping.GetHandover(token, QRFID);
            model.DepatureInfo = handoverMapping.GetGoAheadDeparturesDetails(token, QRFID);

            return View("Handover/Handover", model);
        }

        [HttpPost]
        public IActionResult SetConfirmBooking(DepatureInfoGetReq request)
        {
            HandoverViewModel model = new HandoverViewModel();
            string UserName = ckUserEmailId;
            string msg = "";
            string status = "";

            ConfirmBookingSetRes confirmBookingSetRes = handoverMapping.SetGoAheadConfirmMessage(token, request.QRFID, UserName, request.DepatureId);
            if (confirmBookingSetRes?.ResponseStatus?.Status?.ToLower() == "success")
            {
                foreach (var item in request.DepatureId)
                {
                    model = handoverMapping.SetConfirmBooking(token, request.QRFID, UserName, new List<long>() { item });
                }
            }
            status = "Success";
            msg = "Booking Confirmation done successfully."; 
            return Json(new
            {
                message = msg,
                msgstatus = status
            });
        }

        public IActionResult GetGoAheadDeparturesDetails(DepatureInfoGetReq depatureInfoGetReq)
        {
            HandoverViewModel model = new HandoverViewModel();
            model.DepatureInfo = handoverMapping.GetGoAheadDeparturesDetails(token, depatureInfoGetReq.QRFID);

            if (model.DepatureInfo != null && model.DepatureInfo.Count > 0)
            {
                if (depatureInfoGetReq.DepatureId != null && depatureInfoGetReq.DepatureId.Count > 0)
                {
                    bool flag = false;
                    model.DepatureInfo.ForEach(a =>
                    {
                        if (depatureInfoGetReq.DepatureId.Contains(a.DepatureId))
                        {
                            if (flag == false && string.IsNullOrEmpty(a.ConfirmMessage))
                            {
                                flag = true;
                                a.IsShowProcessing = true;
                            }
                            else if (!string.IsNullOrEmpty(a.ConfirmMessage) && a.ConfirmMessage.ToLower() != "success")
                            {
                                a.IsShowProcessing = false;
                            }
                            else if (flag)
                            {
                                a.IsShowPending = true;
                            }
                        }
                    });
                }

                return Json(new
                {
                    depatures = model.DepatureInfo
                });
            }
            else
            {
                return Json(new
                {
                    depatures = new List<Depatures>()
                });
            }
        }
        #endregion

        #region Common Function
        public ItineraryViewModel GetSalesOfficerHeraderInfo(string QRFID, string MenuName)
        {
            model = new ItineraryViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = MenuName;

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return model;
        }

        public List<Attributes> GetDays(List<ItineraryDaysInfo> itineraryDays)
        {
            List<Attributes> lstDays = new List<Attributes>();
            foreach (var day in itineraryDays)
            {
                lstDays.Add(new Attributes { Attribute_Id = day.Day, AttributeName = day.Day });
            }
            return lstDays;
        }

        public List<mProductType> GetProductTypes(List<ItineraryDaysInfo> itineraryDays)
        {
            List<mProductType> lstProductTypes = new List<mProductType>();
            foreach (var m in itineraryDays)
            {
                foreach (var n in m.ItineraryDescription)
                {
                    if (!string.IsNullOrEmpty(n.ProductType))
                    {
                        if (lstProductTypes.Where(a => a.Prodtype == n.ProductType).Count() <= 0)
                        {
                            lstProductTypes.Add(new mProductType { Prodtype = n.ProductType });
                        }
                    }
                }
            }
            return lstProductTypes.OrderBy(x => x.Prodtype).ToList();
        }
        #endregion

        #region Add New Departures
        public IActionResult GetGoAheadNewDepature(string QRFID, string GoAheadId)
        {
            GoAheadNewDeptGetRes model = new GoAheadNewDeptGetRes();
            model = handoverMapping.GetGoAheadExistDepartures(new GoAheadGetReq() { GoAheadId = GoAheadId, QRFID = QRFID }, token);
            model.GoAheadId = GoAheadId;
            model.QRFID = QRFID;
            model.NewDepatures = new List<NewDepatures>() { new NewDepatures() { DepatureDate = DateTime.Now } };
            return PartialView("AttachToMaster/_AddDepartures", model);
        }

        [HttpPost]
        public IActionResult SetGoAheadNewDepartures(GoAheadNewDeptGetRes objGoAheadNewDeptGetRes)
        {
            GoAheadNewDeptSetRes model = new GoAheadNewDeptSetRes();
            string msg = "";
            string status = "";
            model = handoverMapping.SetGoAheadNewDepartures(new GoAheadNewDeptSetReq()
            {
                ExisitingDepatures = new ExisitingDepatures() { DepatureId = Convert.ToInt64(objGoAheadNewDeptGetRes.ExistDepartureId) },
                GoAheadId = objGoAheadNewDeptGetRes.GoAheadId,
                QRFID = objGoAheadNewDeptGetRes.QRFID,
                NewDepatures = objGoAheadNewDeptGetRes.NewDepatures,
                UserEmail = ckUserEmailId
            }, token);

            if (!string.IsNullOrEmpty(model?.ResponseStatus?.Status))
            {
                status = model.ResponseStatus.Status;
                msg = model.ResponseStatus.ErrorMessage;
                if (status.ToLower() == "error")
                {
                    TempData["error"] = msg;
                }
                else
                {
                    TempData["success"] = msg;
                }
            }
            else
            {
                status = "Error";
                msg = "New Departure dates not added.";
                TempData["error"] = msg;
            }

            return Json(new
            {
                message = msg,
                msgstatus = status
            });
        }
        #endregion
    }
}