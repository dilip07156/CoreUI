
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.Controllers
{
    [Authorize]
    public class PositionController : VoyagerController
    {
        #region Declaration
        private PositionProviders positionProviders;
        private readonly IConfiguration _configuration;

        public PositionController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            positionProviders = new PositionProviders(_configuration);
        }
        #endregion

        #region Prices 
        public ActionResult GetPositionPricesPartView(PositionPriceGetReq request)
        {
            PositionPriceGetRes response = new PositionPriceGetRes();
            request.LoginUser = ckUserEmailId;
            response = positionProviders.GetPostionPrices(request, token).Result;

            var contractId = response.PositionPrice.Where(a => !string.IsNullOrEmpty(a.ContractId)).Select(b => b.ContractId).FirstOrDefault();
            if (!string.IsNullOrEmpty(contractId))
            {
                ViewBag.IsContractExist = true;
            }

            var dateRangeCount = response.PositionPrice.Select(b => b.DepartureId).Distinct().ToList().Count();
            if (dateRangeCount > 1)
            {
                ViewBag.IsMultipleDateExist = dateRangeCount;
            }

            #region commented
            //foreach (var ProdRange in ProdRangeList)
            //{
            //    model.PositionPricesData.Add(new PositionPricesData
            //    {
            //        QRFID = request.QRFID,
            //        PositionId = request.PositionId,
            //        DepartureId = request.DepartureId,
            //        Period = "ALL",
            //        PaxSlab = "ALL",
            //        Type = "ALL",
            //        ProductCategory = "ALL",
            //        BudgetPrice = ProdRange.BudgetPrice,
            //        ContractPrice = ProdRange.ContractPrice,
            //        BuyCurrency = ProdRange.BuyCurrency,
            //        ProductRange = ProdRange.ProductRange,
            //        ProductRangeId = ProdRange.ProductRangeId,
            //        ProductRangeCode = ProdRange.ProductRangeCode
            //    });
            //}

            //foreach (var objModel in response.PositionPrice)
            //{
            //    //if (model.StandardPrice && model.PositionPricesData.Count > 0 && model.PositionPricesData[0].BudgetPrice == 0.00)
            //    //    model.PositionPricesData[0].BudgetPrice = objModel.BudgetPrice;

            //    var obj = new PositionPricesData
            //    {
            //        PositionPriceId = objModel.PositionPriceId,
            //        QRFID = objModel.QRFID,
            //        PositionId = objModel.PositionId,
            //        DepartureId = objModel.DepartureId,
            //        Period = objModel.Period.ToString(),
            //        PaxSlabId = objModel.PaxSlabId,
            //        PaxSlab = objModel.PaxSlab,
            //        Type = objModel.Type,
            //        RoomId = objModel.RoomId,
            //        IsSupplement = objModel.IsSupplement,
            //        SupplierId = objModel.SupplierId,
            //        Supplier = objModel.Supplier,
            //        ProductCategoryId = objModel.ProductCategoryId,
            //        ProductCategory = objModel.ProductCategory,
            //        ProductRangeId = objModel.ProductRangeId,
            //        ProductRange = objModel.ProductRange,
            //        ProductRangeCode = objModel.ProductRangeCode,
            //        ContractPrice = objModel.ContractPrice,
            //        ContractId = objModel.ContractId,
            //        BuyCurrencyId = objModel.BuyCurrencyId,
            //        BuyCurrency = objModel.BuyCurrency,
            //        BudgetPrice = objModel.BudgetPrice,
            //        BuyPrice = objModel.BuyPrice,
            //        MarkupAmount = objModel.MarkupAmount,
            //        SellCurrencyId = objModel.SellCurrencyId,
            //        SellCurrency = objModel.SellCurrency,
            //        SellNetPrice = objModel.SellNetPrice,
            //        TaxAmount = objModel.TaxAmount,
            //        SellPrice = objModel.SellPrice,
            //        ExchangeRateId = objModel.ExchangeRateId,
            //        ExchangeRatio = objModel.ExchangeRatio
            //    };

            //    model.PositionPricesData.Add(obj);
            //}
            #endregion

            PositionPricesViewModel model = new PositionPricesViewModel { StandardPrice = (response.StandardPrice == null || response.StandardPrice == true) ? true : false };
            
            var ProdRangeList = response.PositionPrice.Select(a => new { a.ProductRange, a.ProductRangeId }).Distinct().ToList();

            foreach (var ProdRange in ProdRangeList)
            {
                var IsExist = model.PositionPricesData.Where(a => a.ProductRangeId == ProdRange.ProductRangeId).FirstOrDefault();
                if (IsExist == null)
                {
                    var PriceList = response.PositionPrice.Where(a => a.ProductRangeId == ProdRange.ProductRangeId).Select(a => new { a.ProductRange, a.ProductRangeId, a.BudgetPrice, a.ContractPrice, a.ContractId, a.BuyCurrency, a.ProductRangeCode }).Distinct().ToList();
                    if (PriceList?.Count == 1)
                    {
                        model.PositionPricesData.AddRange(PriceList.Select(a => new PositionPricesData
                        {
                            QRFID = request.QRFID,
                            PositionId = request.PositionId,
                            DepartureId = request.DepartureId,
                            Period = "ALL",
                            PaxSlab = "ALL",
                            Type = "ALL",
                            ProductCategory = "ALL",
                            BudgetPrice = a.BudgetPrice,
                            ContractPrice = a.ContractPrice,
                            IsContractExist = string.IsNullOrEmpty(a.ContractId) ? false : true,
                            BuyCurrency = a.BuyCurrency,
                            ProductRange = a.ProductRange,
                            ProductRangeId = a.ProductRangeId,
                            ProductRangeCode = a.ProductRangeCode
                        }));
                    }
                    else
                    {
                        model.PositionPricesData.Add(new PositionPricesData
                        {
                            QRFID = request.QRFID,
                            PositionId = request.PositionId,
                            DepartureId = request.DepartureId,
                            Period = "ALL",
                            PaxSlab = "ALL",
                            Type = "ALL",
                            ProductCategory = "ALL",
                            BudgetPrice = 0,
                            ContractPrice = 0,
                            IsContractExist = string.IsNullOrEmpty(PriceList[0].ContractId) ? false : true,
                            BuyCurrency = PriceList[0].BuyCurrency,
                            ProductRange = PriceList[0].ProductRange,
                            ProductRangeId = PriceList[0].ProductRangeId,
                            ProductRangeCode = PriceList[0].ProductRangeCode
                        });
                    }
                }

            }

            model.PositionPricesData.AddRange(response.PositionPrice.Select(a => new PositionPricesData
            {
                PositionPriceId = a.PositionPriceId,
                QRFID = a.QRFID,
                PositionId = a.PositionId,
                DepartureId = a.DepartureId,
                Period = a.Period.ToString(),
                PaxSlabId = a.PaxSlabId,
                PaxSlab = a.PaxSlab,
                Type = a.Type,
                RoomId = a.RoomId,
                IsSupplement = a.IsSupplement,
                SupplierId = a.SupplierId,
                Supplier = a.Supplier,
                ProductCategoryId = a.ProductCategoryId,
                ProductCategory = a.ProductCategory,
                ProductRangeId = a.ProductRangeId,
                ProductRange = a.ProductRange,
                ProductRangeCode = a.ProductRangeCode,
                ContractPrice = a.ContractPrice,
                IsContractExist = string.IsNullOrEmpty(a.ContractId) ? false : true,
                ContractId = a.ContractId,
                BuyCurrencyId = a.BuyCurrencyId,
                BuyCurrency = a.BuyCurrency,
                BudgetPrice = a.BudgetPrice,
                BuyPrice = a.BuyPrice,
                MarkupAmount = a.MarkupAmount,
                SellCurrencyId = a.SellCurrencyId,
                SellCurrency = a.SellCurrency,
                SellNetPrice = a.SellNetPrice,
                TaxAmount = a.TaxAmount,
                SellPrice = a.SellPrice,
                ExchangeRateId = a.ExchangeRateId,
                ExchangeRatio = a.ExchangeRatio
            }));
            model.IsSalesOfficeUser = response.IsSalesOfficeUser;
            return PartialView("_PositionPrices", model);
        }

        [HttpPost]
        public JsonResult SetPositionPrices(PositionPricesViewModel model)
        {
            PositionPriceSetReq request = new PositionPriceSetReq();
            PositionPriceSetRes response = new PositionPriceSetRes();

            if (model.StandardPrice)
            {
                var ProdRangeList = model.PositionPricesData.Select(a => new { a.ProductRangeId }).Distinct().ToList();
                foreach (var item in ProdRangeList)
                {
                    var data = model.PositionPricesData.Where(a => a.Period.ToUpper() == "ALL" && a.ProductRangeId == item.ProductRangeId).FirstOrDefault();
                    model.PositionPricesData.Where(a => a.Period.ToUpper() != "ALL" && a.ProductRangeId == item.ProductRangeId).ToList()
                        .ForEach(y => y.BudgetPrice = data.BudgetPrice);
                }
                //var res = model.PositionPricesData.Where(x => x.Period.ToUpper() == "ALL").FirstOrDefault();
                //model.PositionPricesData.Where(x => x.Period.ToUpper() != "ALL").ToList().ForEach(y => y.BudgetPrice = res.BudgetPrice);
            }

            List<mPositionPrice> lstPositionPrices = new List<mPositionPrice>();
            foreach (var objModel in model.PositionPricesData.Where(x => x.Period.ToUpper() != "ALL"))
            {
                var obj = new mPositionPrice
                {
                    PositionPriceId = objModel.PositionPriceId,
                    BudgetPrice = objModel.BudgetPrice,
                    PositionId = objModel.PositionId,
                    CreateUser = ckUserEmailId,
                    EditUser = ckUserEmailId
                };

                if (objModel.Period == null || Convert.ToDateTime(objModel.Period) == DateTime.MinValue) obj.Period = null;
                else obj.Period = Convert.ToDateTime(objModel.Period);

                lstPositionPrices.Add(obj);
            }

            request.StandardPrice = model.StandardPrice;
            request.PositionPrice = lstPositionPrices;
            request.IsClone = model.IsClone;
            request.QRFID = model.QRFID;
            request.VoyagerUserID = ckLoginUser_Id;
            response = positionProviders.SetPostionPrices(request, token).Result;

            if (response != null)
            {
                if (response.ResponseStatus.Status.ToLower() == "success")
                    return Json(new { status = "success", positionid = response.PositionId, productid = response.ProductId, positionname = response.PositionName });
            }
            return Json(new { status = "failure", positionid = "", productid = "", positionname = "" });
        }
        #endregion

        #region FOC 
        public ActionResult GetPositionFOCPartView(PositionFOCGetReq request)
        {
            PositionFOCGetRes response = new PositionFOCGetRes();
            response = positionProviders.GetPostionFOC(request, token).Result;

            PositionFOCViewModel model = new PositionFOCViewModel { StandardFoc = (response.StandardFOC == null || response.StandardFOC == true) ? true : false };
            
            var ProdRangeList = response.PositionFOC.Select(a => new { a.ProductRange, a.ProductRangeId }).Distinct().ToList();

            foreach(var ProdRange in ProdRangeList)
            {
                var IsExist = model.PositionFOCData.Where(a => a.ProductRangeId == ProdRange.ProductRangeId).FirstOrDefault();
                if(IsExist == null)
                {
                    var FOCList = response.PositionFOC.Where(a=>a.ProductRangeId==ProdRange.ProductRangeId).Select(a => new { a.ProductRange, a.ProductRangeId, a.FOCQty }).Distinct().ToList();
                    if(FOCList?.Count == 1)
                    {
                        model.PositionFOCData.AddRange(FOCList.Select(a => new PositionFOCData
                        {
                            QRFID = request.QRFID,
                            PositionId = request.PositionId,
                            Period = "ALL",
                            CityName = "ALL",
                            ProductName = "ALL",
                            PaxSlab = "ALL",
                            Type = "ALL",
                            ProductCategory = "ALL",
                            FOCQty = a.FOCQty,
                            ProductRange = a.ProductRange,
                            ProductRangeId = a.ProductRangeId
                        }));
                    }
                    else
                    {
                        model.PositionFOCData.Add(new PositionFOCData
                        {
                            QRFID = request.QRFID,
                            PositionId = request.PositionId,
                            Period = "ALL",
                            CityName = "ALL",
                            ProductName = "ALL",
                            PaxSlab = "ALL",
                            Type = "ALL",
                            ProductCategory = "ALL",
                            FOCQty = 0,
                            ProductRange = FOCList[0].ProductRange,
                            ProductRangeId = FOCList[0].ProductRangeId
                        });
                    }
                }

            }
            
            model.PositionFOCData.AddRange(response.PositionFOC.Select(a => new PositionFOCData
            {
                PositionFOCId = a.PositionFOCId,
                QRFID = a.QRFID,
                PositionId = a.PositionId,
                DepartureId = a.DepartureId,
                Period = a.Period.ToString(),
                PaxSlabId = a.PaxSlabId,
                PaxSlab = a.PaxSlab,
                Type = a.Type,
                RoomId = a.RoomId,
                IsSupplement = a.IsSupplement,
                SupplierId = a.SupplierId,
                Supplier = a.Supplier,
                ProductCategoryId = a.ProductCategoryId,
                ProductCategory = a.ProductCategory,
                ProductRangeId = a.ProductRangeId,
                ProductRange = a.ProductRange,
                ContractId = a.ContractId,
                CityId = a.CityId,
                CityName = a.CityName,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Quantity = a.Quantity,
                FOCQty = a.FOCQty
            }));
            
            return PartialView("_PositionFOC", model);
        }

        [HttpPost]
        public JsonResult SetPositionFOC(PositionFOCViewModel model)
        {
            PositionFOCSetReq request = new PositionFOCSetReq();
            PositionFOCSetRes response = new PositionFOCSetRes();

            if (model.StandardFoc)
            {
                var ProdRangeList = model.PositionFOCData.Select(a => new { a.ProductRangeId }).Distinct().ToList();
                foreach (var item in ProdRangeList)
                {
                    var data = model.PositionFOCData.Where(a => a.Period.ToUpper() == "ALL" && a.ProductRangeId == item.ProductRangeId).FirstOrDefault();
                    model.PositionFOCData.Where(a => a.Period.ToUpper() != "ALL" && a.ProductRangeId == item.ProductRangeId).ToList()
                        .ForEach(y => y.FOCQty = data.FOCQty);
                }
                //var res = model.PositionFOCData.Where(x => x.Period.ToUpper() == "ALL").FirstOrDefault();
                //model.PositionFOCData.Where(x => x.Period.ToUpper() != "ALL").ToList().ForEach(y => y.FOCQty = res.FOCQty);
            }

            List<mPositionFOC> lstPositionFOC = new List<mPositionFOC>();
            foreach (var objModel in model.PositionFOCData.Where(x => x.Period.ToUpper() != "ALL"))
            {
                var obj = new mPositionFOC
                {
                    PositionFOCId = objModel.PositionFOCId,
                    PositionId = objModel.PositionId,
                    FOCQty = objModel.FOCQty,
                    CreateUser = ckUserEmailId,
                    EditUser = ckUserEmailId
                };

                if (objModel.Period == null || Convert.ToDateTime(objModel.Period) == DateTime.MinValue) obj.Period = null;
                else obj.Period = Convert.ToDateTime(objModel.Period);

                lstPositionFOC.Add(obj);
            }

            request.StandardFOC = model.StandardFoc;
            request.PositionFOC = lstPositionFOC;
            request.IsClone = model.IsClone;
            response = positionProviders.SetPostionFOC(request, token).Result;

            if (response != null)
            {
                if (response.ResponseStatus.Status.ToLower() == "success")
                    return Json("success");
            }
            return Json("failure");
        }
        #endregion

        #region PositionRoomDetails
        public ActionResult GetPositionRoomDetails(PositionRoomsGetReq request)
        {
            PositionRoomsViewModel model = new PositionRoomsViewModel();
            List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
            List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();

            model.RowNo = request.RowNo;
            model.PositionType = request.PositionType;
            model.PositionId = request.PositionId;
            PositionRoomsGetRes response = positionProviders.GetPositionRoomDetails(request, token).Result;

            if (response != null && response.ResponseStatus != null && response.ResponseStatus.Status == "Success")
            {
                if (response.ProductRangeDetails != null && response.ProductRangeDetails.Count > 0)
                {
                    listRoomType = response.ProductRangeDetails.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                    listSupplements = response.ProductRangeDetails.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                }
                else
                {
                    listRoomType = new List<ProductAttributeDetails>();
                    listSupplements = new List<ProductAttributeDetails>();
                }
            }
            else
            {
                listRoomType = new List<ProductAttributeDetails>();
                listSupplements = new List<ProductAttributeDetails>();
            }
            model.RoomDetails = (response.RoomDetailsInfo == null || response.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                              response.RoomDetailsInfo.Select(b => new RoomDetails
                              {
                                  RoomId = b.RoomId,
                                  RoomSequence = b.RoomSequence,
                                  ProductRange = b.ProductRange,
                                  ProductRangeID = b.ProductRangeId,
                                  IsSupplement = b.IsSupplement,
                                  RoomTypeList = b.IsSupplement ?
                                      new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                              }).ToList();
            model.SupplementList = listSupplements;

            return PartialView("_PositionRoomDetails", model);
        }

        [HttpPost]
        public JsonResult SetPositionRoomDetails(PositionRoomsSetReq request)
        {
            request.EditUser = ckUserEmailId;
            request.RoomDetailsInfo.ForEach(a =>
            {
                a.IsDeleted = a.RoomSequence == 0 ? true : false;
                a.CreateUser = request.EditUser;
                a.CreateDate = DateTime.Now;
                a.EditUser = request.EditUser;
                a.EditDate = DateTime.Now;
            });
            PositionRoomsSetRes response = positionProviders.SetPositionRoomDetails(request, token).Result;

            if (response != null && response.ResponseStatus != null && response.ResponseStatus.Status == "Success")
            {
                return Json(new { status = "success", positionid = request.PositionId, productid = request.PositionId, RoomDetailsInfo = response.RoomDetailsInfo });
            }
            else
            {
                return Json(new { status = "failure", positionid = request.PositionId, productid = request.PositionId, RoomDetailsInfo = new List<RoomDetailsInfo>() });
            }
        }
        #endregion
    }
}