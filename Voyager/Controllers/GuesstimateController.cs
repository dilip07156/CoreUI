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
    public class GuesstimateController : VoyagerController
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        public COCommonLibrary cOCommonLibrary;
        private QRFSummaryProviders qrfSummaryProviders;
        private COProviders coProviders;
        private MasterProviders masterProviders;
        private QRFSummaryMapping qrfSummaryMapping;

        public GuesstimateController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            qrfSummaryProviders = new QRFSummaryProviders(_configuration);
            qrfSummaryMapping = new QRFSummaryMapping(_configuration);
            coProviders = new COProviders(_configuration);
            masterProviders = new MasterProviders(_configuration);
        }
        #endregion

        [Route("CostingOfficer/Guesstimate")]
        public IActionResult Guesstimate()
        {
            if (UserRoles.Contains("Product Accountant"))
            { }
            else if (!UserRoles.Contains("Costing Officer"))
            {
                return View("Unauthorize");
            }
            string QRFID = Request.Query["QRFId"];

            GuesstimateViewModel model = new GuesstimateViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Guesstimate";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion

            return View(model);
        }

        public IActionResult GetGuesstimateData(string QRFID, string filterByDay = null, string filterByServiceType = null, string filterByPackageType = null, long filterByDeparture = 0, long filterByPaxSlab = 0, string SuccessMessage = null, string ErrorMessage = null, string CalculateFor = null)
        {
            try
            {
                if (!(string.IsNullOrEmpty(SuccessMessage)))
                    TempData["success"] = SuccessMessage;

                if (!(string.IsNullOrEmpty(ErrorMessage)))
                    TempData["error"] = ErrorMessage;

                GuesstimateViewModel model = new GuesstimateViewModel();

                //#region Get Summary Detailsby QRFId
                //qrfSummaryGetReq = new QRFSummaryGetReq();
                //qrfSummaryGetReq.QRFID = QRFID;
                //qrfSummaryGetRes = qrfSummaryProviders.GetQRFSummary(qrfSummaryGetReq, token).Result;
                //model.SummaryDetails = qrfSummaryMapping.GetQRFSummaryDetails(qrfSummaryGetRes, token);
                //#endregion

                #region Filter Dropdown Bindings

                var objKeepAs = new List<AttributeValues>();

                MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
                objMasterTypeRequest.Property = "QRF Masters";
                objMasterTypeRequest.Name = "Position Type";

                SalesProviders objSalesProvider = new SalesProviders(_configuration);
                MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

                if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
                {
                    if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "Position Type")
                    {
                        objKeepAs = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
                    }
                }

                QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
                QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
                QRFPaxSlabGetReq objPaxSlabReq = new QRFPaxSlabGetReq() { QRFID = QRFID };
                QRFPaxGetResponse objPaxSlabRes = coProviders.GetPaxSlabDetailsForCostingByQRF_Id(objPaxSlabReq, token).Result;
                model.PaxSlabList = (objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs).Select(a => new AttributeValues { AttributeValue_Id = a.PaxSlab_Id.ToString(), Value = a.From + "-" + a.To }).ToList();
                model.DepartureDatesList = (objDepartureDatesRes.DepartureDates).Select(a => new AttributeValues { AttributeValue_Id = a.Departure_Id.ToString(), Value = Convert.ToDateTime(a.Date).ToString("dd MMM yy") }).ToList();

                #endregion

                #region Get Guesstimate by QRFId
                GuesstimateGetRes response = new GuesstimateGetRes();
                GuesstimateGetReq request = new GuesstimateGetReq();
                request.QRFID = QRFID;
                request.DepartureId = filterByDeparture == 0 ? objDepartureDatesRes.DepartureDates[0].Departure_Id : filterByDeparture;
                request.PaxSlabId = filterByPaxSlab == 0 ? objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs[0].PaxSlab_Id : filterByPaxSlab;
                request.CalculateFor = CalculateFor;
                request.LoginUserId = ckUserEmailId;
                response = coProviders.GetGuesstimate(request, token).Result;
                GetDistinctRanges(response.Guesstimate, ref model);
                model.Guesstimate = response.Guesstimate;
                model.Guesstimate.VersionId = model.Guesstimate.VersionId;
                ViewBag.VersionName = model.Guesstimate.VersionName;
                ViewBag.VersionDescription = model.Guesstimate.VersionDescription;
                model.Guesstimate.VersionName = "";
                model.Guesstimate.VersionDescription = "";
                model.NextVersionId = response.LastVersionId + 1;
                model.IsStandardPrice = response.IsStandardPrice;
                #endregion

                #region Filter Logic

                model.DayList = GetDays(model.Guesstimate.GuesstimatePosition);
                model.ServiceTypeList = GetProductTypes(model.Guesstimate.GuesstimatePosition);
                model.KeepAsList = objKeepAs;

                if (filterByDay == "All")
                    filterByDay = null;

                if (filterByServiceType == "All")
                    filterByServiceType = null;

                if (filterByPackageType == "All")
                    filterByPackageType = null;

                if (!string.IsNullOrEmpty(filterByDay))
                {
                    model.Guesstimate.GuesstimatePosition = model.Guesstimate.GuesstimatePosition.Where(x => x.Day == filterByDay).ToList();
                }

                if (!string.IsNullOrEmpty(filterByServiceType))
                {
                    model.Guesstimate.GuesstimatePosition = model.Guesstimate.GuesstimatePosition.Where(x => x.ProductType == filterByServiceType).ToList();
                }

                if (!string.IsNullOrEmpty(filterByPackageType))
                {
                    model.Guesstimate.GuesstimatePosition = model.Guesstimate.GuesstimatePosition.Where(x => x.KeepAs == filterByPackageType).ToList();
                }

                //model.Guesstimate.GuesstimateDetails = model.Guesstimate.GuesstimateDetails.Where(x => model.SummaryDetails.Any(a => a.OriginalItineraryDetails.Any(b => b.PositionId == x.PositionId))).ToList();

                model.Day = filterByDay;
                model.Service = filterByServiceType;
                model.KeepAs = filterByPackageType;
                model.DepartureDate = filterByDeparture.ToString();
                model.PaxSlab = filterByPaxSlab.ToString();

                #endregion

                return PartialView("_Guesstimate", model);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        [HttpPost]
        public JsonResult SetGuesstimatePrices(GuesstimateViewModel model)
        {
            model.Guesstimate.CreateUser = ckUserEmailId;
            model.Guesstimate.EditUser = ckUserEmailId;
            model.Guesstimate.VersionId = model.NextVersionId;

            GuesstimateSetRes response = new GuesstimateSetRes();
            GuesstimateSetReq request = new GuesstimateSetReq();
            request.Guesstimate = model.Guesstimate;
            request.IsNewVersion = model.IsNewVersion;
            request.DepartureId = Convert.ToInt64(model.DepartureDate);
            request.PaxSlabId = Convert.ToInt64(model.PaxSlab);
            request.Qrfid = model.Guesstimate.QRFID;
            request.VoyagerUserId = ckLoginUser_Id;
            response = coProviders.SetGuesstimate(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        public IActionResult GetGuesstimateVersions(string QRFID)
        {
            try
            {
                GuesstimateVersionGetRes response = new GuesstimateVersionGetRes();
                GuesstimateGetReq request = new GuesstimateGetReq();
                request.QRFID = QRFID;
                response = coProviders.GetGuesstimateVersions(request, token).Result;
                return PartialView("_GuesstimateVersion", response.GuesstimateVersions);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        [HttpPost]
        public JsonResult UpdateGuesstimateVersion(string QRFID, string GuesstimateId)
        {
            GuesstimateSetRes response = new GuesstimateSetRes();
            GuesstimateVersionSetReq request = new GuesstimateVersionSetReq();
            request.QRFID = QRFID;
            request.GuesstimateId = GuesstimateId;
            request.EditUser = ckUserEmailId;
            response = coProviders.UpdateGuesstimateVersion(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }

        public IActionResult GetProductSupplierList(string ProductId)
        {
            try
            {
                ProductSupplierGetRes response = new ProductSupplierGetRes();
                ProductSupplierGetReq request = new ProductSupplierGetReq();
                request.ProductId = ProductId;
                response = masterProviders.GetProductSupplierList(request, token).Result;
                return Json(response);
            }
            catch (Exception ex)
            {
                throw;
                return View();
            }
        }

        public IActionResult GetSupplierPrice(string QRFID, long DepartureId = 0, long PaxSlabId = 0, string PositionId = null, string SupplierId = null)
        {
            try
            {
                #region Get Guesstimate by QRFId
                GuesstimateGetRes response = new GuesstimateGetRes();
                GuesstimateGetReq request = new GuesstimateGetReq();
                request.QRFID = QRFID;
                request.DepartureId = DepartureId;
                request.PaxSlabId = PaxSlabId;
                request.PositionId = PositionId;
                request.SupplierId = SupplierId;
                request.LoginUserId = ckUserEmailId;

                if (DepartureId == 0)
                {
                    QRFDepartureDateGetReq objDepartureDatesReq = new QRFDepartureDateGetReq() { QRFID = QRFID, date = (DateTime?)null };
                    QRFDepartureDateGetRes objDepartureDatesRes = coProviders.GetDepartureDatesForCostingByQRF_Id(objDepartureDatesReq, token).Result;
                    request.DepartureId = objDepartureDatesRes.DepartureDates[0].Departure_Id;
                }
                if (PaxSlabId == 0)
                {
                    QRFPaxSlabGetReq objPaxSlabReq = new QRFPaxSlabGetReq() { QRFID = QRFID };
                    QRFPaxGetResponse objPaxSlabRes = coProviders.GetPaxSlabDetailsForCostingByQRF_Id(objPaxSlabReq, token).Result;
                    request.PaxSlabId = objPaxSlabRes.PaxSlabDetails.QRFPaxSlabs[0].PaxSlab_Id;
                }

                response = coProviders.GetSupplierPrice(request, token).Result;
                #endregion
                return Json(response);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult SetGuesstimateChangeRule(string GuesstimateId, string ChangeRule, double? ChangeRulePercent)
        {
            GuesstimateSetRes response = new GuesstimateSetRes();
            GuesstimateChangeRuleSetReq request = new GuesstimateChangeRuleSetReq();
            request.GuesstimateId = GuesstimateId;
            request.ChangeRule = ChangeRule;
            request.ChangeRulePercent = ChangeRulePercent;
            request.EditUser = ckUserEmailId;
            response = coProviders.SetGuesstimateChangeRule(request, token).Result;

            if (response != null)
            {
                return Json(response.ResponseStatus);
            }
            return Json("failure");
        }


        #region Helper Methods
        public List<Attributes> GetDays(List<GuesstimatePosition> pos)
        {
            List<Attributes> lstDays = new List<Attributes>();
            foreach (var day in pos)
            {
                if (lstDays.Where(a => a.Attribute_Id == day.Day).Count() <= 0)
                {
                    lstDays.Add(new Attributes { Attribute_Id = day.Day, AttributeName = day.Day });
                }
            }
            return lstDays;
        }

        public List<mProductType> GetProductTypes(List<GuesstimatePosition> pos)
        {
            List<mProductType> lstProductTypes = new List<mProductType>();
            foreach (var m in pos)
            {
                if (lstProductTypes.Where(a => a.Prodtype == m.ProductType).Count() <= 0)
                {
                    lstProductTypes.Add(new mProductType { Prodtype = m.ProductType });
                }
            }
            return lstProductTypes;

        }

        public bool GetDistinctRanges(mGuesstimate prices, ref GuesstimateViewModel model)
        {
            List<AttributeValues> lstRangesAcco = new List<AttributeValues>();
            foreach (var p in prices.GuesstimatePosition)
            {
                if (p.ProductType != null)
                {
                    if (p.ProductType.ToLower() == "hotel" || p.ProductType.ToLower() == "apartments" || p.ProductType.ToLower() == "overnight ferry")
                    {
                        foreach (var pr in p.GuesstimatePrice)
                        {
                            if (pr.ProductRange != null)
                            {
                                if (lstRangesAcco.Where(a => a.Value == pr.ProductRange.Replace("ADULT", "A").Replace("GUIDE", "G").Replace("DRIVER", "D")).Count() <= 0)
                                {
                                    lstRangesAcco.Add(new AttributeValues { AttributeValue_Id = pr.ProductRangeId, Value = pr.ProductRange.Replace("ADULT", "A").Replace("GUIDE", "G").Replace("DRIVER", "D") });
                                }
                            }
                        }
                    }
                }
            }

            #region Sort
            for (int i = 0; i < lstRangesAcco.Count; i++)
            {
                if (lstRangesAcco[i].Value.ToLower() == "single (a)")
                {
                    lstRangesAcco[i].SequenceNo = 1;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "double (a)")
                {
                    lstRangesAcco[i].SequenceNo = 2;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "twin (a)")
                {
                    lstRangesAcco[i].SequenceNo = 3;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "triple (a)")
                {
                    lstRangesAcco[i].SequenceNo = 4;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "quad (a)")
                {
                    lstRangesAcco[i].SequenceNo = 5;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "single (d)")
                {
                    lstRangesAcco[i].SequenceNo = 6;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "double (d)")
                {
                    lstRangesAcco[i].SequenceNo = 7;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "twin (d)")
                {
                    lstRangesAcco[i].SequenceNo = 8;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "triple (d)")
                {
                    lstRangesAcco[i].SequenceNo = 9;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "quad (d)")
                {
                    lstRangesAcco[i].SequenceNo = 10;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "single (g)")
                {
                    lstRangesAcco[i].SequenceNo = 11;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "double (g)")
                {
                    lstRangesAcco[i].SequenceNo = 12;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "twin (g)")
                {
                    lstRangesAcco[i].SequenceNo = 13;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "triple (g")
                {
                    lstRangesAcco[i].SequenceNo = 14;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "quad (g)")
                {
                    lstRangesAcco[i].SequenceNo = 15;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "child")
                {
                    lstRangesAcco[i].SequenceNo = 16;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "child + bed")
                {
                    lstRangesAcco[i].SequenceNo = 17;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "child - bed")
                {
                    lstRangesAcco[i].SequenceNo = 18;
                }
                else if (lstRangesAcco[i].Value.ToLower() == "infant")
                {
                    lstRangesAcco[i].SequenceNo = 19;
                }
                else
                {
                    lstRangesAcco[i].SequenceNo = 100;
                }
            }
            lstRangesAcco = lstRangesAcco.OrderBy(a => a.SequenceNo).ToList();
            model.RangeAccoList = lstRangesAcco;
            #endregion

            List<AttributeValues> lstRangesServ = new List<AttributeValues>();
            foreach (var p in prices.GuesstimatePosition)
            {
                if (p.ProductType != null)
                {
                    if (p.ProductType.ToLower() == "meal" || p.ProductType.ToLower() == "attractions" || p.ProductType.ToLower() == "sightseeing - citytour" || p.ProductType.ToLower() == "visa" ||
                    p.ProductType.ToLower() == "insurance" || p.ProductType.ToLower() == "ferry passenger" || p.ProductType.ToLower() == "scheduled transfer" || p.ProductType.ToLower() == "train"
                    || p.ProductType.ToLower() == "domestic flight"
                    )
                    {
                        foreach (var pr in p.GuesstimatePrice)
                        {
                            if (pr.IsSupplement)
                            {
                                if (pr.ProductRange != null)
                                {
                                    if (lstRangesServ.Where(a => a.Value == pr.ProductRange.Replace("ADULT", "A").Replace("CHILD", "C")).Count() <= 0)
                                    {
                                        lstRangesServ.Add(new AttributeValues { AttributeValue_Id = pr.ProductRangeId, Value = pr.ProductRange.Replace("ADULT", "A").Replace("CHILD", "C") });
                                    }
                                }
                            }
                            else
                            {
                                if (pr.Type != null)
                                {
                                    if (lstRangesServ.Where(a => a.Value == pr.Type).Count() <= 0)
                                    {
                                        lstRangesServ.Add(new AttributeValues { AttributeValue_Id = pr.ProductRangeId, Value = pr.Type });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #region Sort
            for (int i = 0; i < lstRangesServ.Count; i++)
            {
                if (lstRangesServ[i].Value.ToLower() == "adult")
                {
                    lstRangesServ[i].SequenceNo = 1;
                }
                else if (lstRangesServ[i].Value.ToLower() == "child")
                {
                    lstRangesServ[i].SequenceNo = 2;
                }
                else
                {
                    lstRangesServ[i].SequenceNo = 100;
                }
            }
            lstRangesServ = lstRangesServ.OrderBy(a => a.SequenceNo).ToList();
            model.RangeServicesList = lstRangesServ;
            #endregion

            List<AttributeValues> lstRangesUnit = new List<AttributeValues>();
            foreach (var p in prices.GuesstimatePosition)
            {
                if (p.ProductType != null)
                {
                    if (p.ProductType.ToLower() == "coach" || p.ProductType.ToLower() == "ferry transfer" || p.ProductType.ToLower() == "guide" || p.ProductType.ToLower() == "private transfer" ||
                    p.ProductType.ToLower() == "ldc" || p.ProductType.ToLower() == "assistant" || p.ProductType.ToLower() == "fee" || p.ProductType.ToLower() == "other")
                    {
                        foreach (var pr in p.GuesstimatePrice)
                        {
                            if (pr.ProductRangeCode != null)
                            {
                                if (lstRangesUnit.Where(a => a.Value == pr.ProductRangeCode).Count() <= 0)
                                {
                                    lstRangesUnit.Add(new AttributeValues { AttributeValue_Id = pr.ProductRangeId, Value = pr.ProductRangeCode });
                                }
                            }
                        }
                    }
                }
            }
            model.RangeUnitList = lstRangesUnit;

            model.RangesList = new List<AttributeValues>();
            model.RangesList.AddRange(lstRangesAcco);
            model.RangesList.AddRange(lstRangesServ);
            model.RangesList.AddRange(lstRangesUnit);
            return true;
        }

        #endregion
    }
}