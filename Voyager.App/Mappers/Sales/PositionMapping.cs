using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.App.Mappers
{
    public class PositionMapping
    {
        #region Declaration
        private PositionLibrary accomLibrary;
        private RoomDetails objRoom;
        private readonly IConfiguration _configuration;
        private MasterProviders objMasterProviders;
        private PositionProviders positionProviders;
        #endregion

        public PositionMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            accomLibrary = new PositionLibrary(_configuration);
            objRoom = new RoomDetails();
            objMasterProviders = new MasterProviders(_configuration);
            positionProviders = new PositionProviders(_configuration);
        }

        #region Accomodation
        public AccomodationViewModel AccomodationGet(PositionGetRes positionGetRes, ref AccomodationViewModel model, string token)
        {
            List<AttributeValues> listNON = new List<AttributeValues>();
            List<TourGuideDefaultRows> DefaultList = new List<TourGuideDefaultRows>();

            if (positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
            {
                for (int i = 1; i <= (positionGetRes.DaysList.Count - 1); i++)
                {
                    listNON.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                }
            }

            if (positionGetRes.RoutingInfo != null && positionGetRes.RoutingInfo.Count > 0 && string.IsNullOrEmpty(model.MenuViewModel.PositionId))
            {
                int day = 0;
                int prevnight = 0;
                foreach (var Route in positionGetRes.RoutingInfo.Where(a => a.Nights > 0))
                {
                    if (day == 0) { day = 1; }
                    else { day = prevnight + day; }
                    //else day += 1; 

                    DefaultList.Add(new TourGuideDefaultRows
                    {
                        DayNo = positionGetRes.DaysList.Where(a => a.Value == "Day " + day.ToString()).FirstOrDefault().AttributeValue_Id,
                        DayName = "Day " + day.ToString(),
                        Duration = Route.Nights,
                        CityID = Route.ToCityID,
                        CityName = Route.ToCityName,
                    });
                    // day += Route.Days - 1;
                    prevnight = Route.Nights;
                }
            }

            if (positionGetRes.ResponseStatus.Status == "Success" && positionGetRes.mPosition.Count > 0 && positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
            {
                List<string> ProductIds = new List<string>();
                positionGetRes.mPosition.ForEach(a => ProductIds.Add(a.ProductID));

                var result = accomLibrary.GetRoomTypeAndSupplementList(ProductIds.Distinct().ToList(), model.MenuViewModel.QRFID, token);

                foreach (var objModel in positionGetRes.mPosition)
                {
                    var obj = new AccomodationData();

                    obj.AccomodationId = objModel.PositionId;
                    obj.AccomodationSequence = objModel.PositionSequence;
                    obj.CityName = objModel.CityName + ", " + objModel.CountryName;
                    obj.CityID = objModel.CityID;
                    obj.StartingFrom = objModel.RoutingDaysID;
                    obj.StartingFromList = positionGetRes.DaysList;
                    obj.NoOfNightList = listNON;
                    obj.DayName = positionGetRes.DaysList.Where(r => r.AttributeValue_Id == objModel.RoutingDaysID).FirstOrDefault() != null ? positionGetRes.DaysList.Where(p => p.AttributeValue_Id == objModel.RoutingDaysID).FirstOrDefault().Value : "";
                    obj.DayNo = objModel.DayNo;
                    obj.NoOfNight = objModel.Duration;
                    obj.Category = objModel.BudgetCategoryId + "|" + objModel.BudgetCategory;
                    obj.StarRating = objModel.StarRating;
                    obj.Location = objModel.Location;
                    obj.ChainName = objModel.ChainName;
                    obj.ChainID = objModel.ChainID;
                    obj.HotelName = objModel.ProductName;
                    obj.HotelID = objModel.ProductID;
                    obj.ProductType = objModel.ProductType;
                    obj.ProductTypeId = objModel.ProductTypeId;
                    obj.SupplierId = objModel.SupplierId;
                    obj.SupplierName = objModel.SupplierName;
                    //Requirements
                    obj.MealPlan = objModel.MealPlan;
                    if (objModel.EarlyCheckInDate!=null)
                    {
                        var from = objModel.EarlyCheckInDate.Value;
                        var day = from.Day.ToString().Length == 1 ? "0" + from.Day.ToString() : from.Day.ToString();
                        var month = from.Month.ToString().Length == 1 ? "0" + from.Month.ToString() : from.Month.ToString();
                        obj.EarlyCheckInDate = day + "/" + month + "/" + from.Year; 
                    } 
                    obj.EarlyCheckInTime = objModel.EarlyCheckInTime;
                    //obj.NumberOfEarlyCheckInRooms = objModel.NumberOfEarlyCheckInRooms;

                    obj.NumberofInterConnectingRooms = objModel.InterConnectingRooms;
                    obj.NumberOfWashChangeRooms = objModel.WashChangeRooms;

                    if (objModel.LateCheckOutDate != null)
                    {
                        var to = objModel.LateCheckOutDate.Value;
                        var day = to.Day.ToString().Length == 1 ? "0" + to.Day.ToString() : to.Day.ToString();
                        var month = to.Month.ToString().Length == 1 ? "0" + to.Month.ToString() : to.Month.ToString();
                        obj.LateCheckOutDate = day + "/" + month + "/" + to.Year;                         
                    } 
                    obj.LateCheckOutTime = objModel.LateCheckOutTime;
                    //obj.NumberOfLateCheckOutRooms = objModel.NumberOfLateCheckOutRooms;

                    //obj.SupplementID = objModel.SupplementID;
                    //obj.Supplement = objModel.Supplement;
                    //Requirements End

                    obj.KeepAs = objModel.KeepAs;
                    obj.RemarksForTL = objModel.TLRemarks;
                    obj.RemarksForOPS = objModel.OPSRemarks;

                    obj.StartTime = objModel.StartTime;
                    obj.EndTime = objModel.EndTime;

                    obj.PositionRoomsData.RoomDetails = new List<RoomDetails>();

                    if (result != null)
                    {
                        if (objModel.RoomDetailsInfo != null && objModel.RoomDetailsInfo.Count > 0)
                        {
                            foreach (var objRoomModel in objModel.RoomDetailsInfo)
                            {
                                objRoom = new RoomDetails();
                                objRoom.RoomId = objRoomModel.RoomId;
                                objRoom.RoomSequence = objRoomModel.RoomSequence;
                                objRoom.ProductRange = "(" + objRoomModel.ProductCategory + ") " + objRoomModel.ProductRange;
                                objRoom.ProductRangeID = objRoomModel.ProductCategoryId + "|" + objRoomModel.ProductRangeId;
                                objRoom.IsSupplement = objRoomModel.IsSupplement;
                                if (objRoom.IsSupplement)
                                {
                                    ProductAttributeDetails productAttributeDetails = new ProductAttributeDetails();
                                    productAttributeDetails.AttributeId = objRoom.ProductRangeID;
                                    productAttributeDetails.Value = "(" + objRoomModel.ProductCategory + ") " + objRoomModel.ProductRange;
                                    objRoom.RoomTypeList.Add(productAttributeDetails);
                                }
                                else
                                {
                                    if (result.RoomTypeList.Count > 0)
                                    {
                                        objRoom.RoomTypeList = (result.RoomTypeList).Where(a => a.AdditionalYN != true && a.ProductId == obj.HotelID).
                                            Select(a => new ProductAttributeDetails { AttributeId = a.ProductCategoryId + '|' + a.VoyagerProductRange_Id, Value = "(" + a.ProductCategoryName + ") " + a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") " }).
                                            OrderBy(a => a.Value.Contains("SINGLE") ? "A" : a.Value.Contains("DOUBLE") ? "B" : a.Value.Contains("TWIN") ? "C" : a.Value.Contains("TRIPLE") ? "D" :
                                            a.Value.Contains("QUAD") ? "E" : a.Value.Contains("TSU") ? "F" :
                                            a.Value.ToLower().Contains("child + bed") ? "G" : a.Value.ToLower().Contains("child - bed") ? "H" :
                                            a.Value.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.Value).ToList();
                                    }
                                }
                                obj.PositionRoomsData.RoomDetails.Add(objRoom);
                            }
                        }
                        else
                        {
                            objRoom.RoomTypeList = (result.RoomTypeList).Where(a => a.AdditionalYN != true && a.ProductId == obj.HotelID).
                                              Select(a => new ProductAttributeDetails { AttributeId = a.ProductCategoryId + '|' + a.VoyagerProductRange_Id, Value = "(" + a.ProductCategoryName + ") " + a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") " }).
                                              OrderBy(a => a.Value.Contains("SINGLE") ? "A" : a.Value.Contains("DOUBLE") ? "B" : a.Value.Contains("TWIN") ? "C" : a.Value.Contains("TRIPLE") ? "D" :
                                              a.Value.Contains("QUAD") ? "E" : a.Value.Contains("TSU") ? "F" :
                                              a.Value.ToLower().Contains("child + bed") ? "G" : a.Value.ToLower().Contains("child - bed") ? "H" :
                                              a.Value.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.Value).ToList();

                            obj.PositionRoomsData.RoomDetails.Add(objRoom);
                        }
                        if (result.RoomTypeList.Count > 0)
                        {
                            obj.PositionRoomsData.SupplementList = (result.RoomTypeList).Where(a => a.AdditionalYN == true && a.ProductId == obj.HotelID).OrderBy(a => a.ProductRangeName).Select(a => new ProductAttributeDetails { AttributeId = a.ProductCategoryId + '|' + a.VoyagerProductRange_Id, Value = "(" + a.ProductCategoryName + ") " + a.ProductRangeCode + " (" + a.PersonType + (a.AgeRange == null ? "" : " | " + a.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                        }
                    }
                    else
                    {
                        obj.PositionRoomsData = new PositionRoomsViewModel { RoomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } } };
                    }
                    model.AccomodationData.Add(obj);
                }

                if (DefaultList.Count > 0)
                {
                    //foreach (var defItem in DefaultList)
                    //{
                    //    if (model.AccomodationData.Where(a => a.StartingFrom == defItem.DayNo).Count() < 1)
                    //    {
                    //        model.AccomodationData.Add(new AccomodationData
                    //        {
                    //            CityID = defItem.CityID,
                    //            CityName = defItem.CityName,
                    //            DayName = defItem.DayName,
                    //            AccomodationSequence = 1,
                    //            MealPlan = "BB",
                    //            KeepAs = "Included",
                    //            StartingFrom = defItem.DayNo,
                    //            NoOfNight = defItem.Duration,
                    //            NoOfNightList = listNON,
                    //            StartingFromList = positionGetRes.DaysList,
                    //            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } } }
                    //        });
                    //    }
                    //}
                    //model.AccomodationData = model.AccomodationData.OrderBy(a => a.DayName).ToList();
                    var Acco = new AccomodationData();
                    int dayno = 0;
                    foreach (var defItem in DefaultList)
                    {
                        Acco = model.AccomodationData.Where(a => a.StartingFrom == defItem.DayNo).FirstOrDefault();
                        dayno = dayno == 0 ? Convert.ToInt32(defItem.DayName.Replace("Day ", "")) : dayno;
                        if (Acco == null && Convert.ToInt32(defItem.DayName.Replace("Day ", "")) >= dayno)
                        {
                            model.AccomodationData.Add(new AccomodationData
                            {
                                CityID = defItem.CityID,
                                CityName = defItem.CityName,
                                DayName = defItem.DayName,
                                DayNo = dayno,
                                AccomodationSequence = 1,
                                MealPlan = "BB",
                                KeepAs = "Included",
                                StartingFrom = defItem.DayNo,
                                StartTime = "20:00",
                                EndTime = "08:30",
                                NoOfNight = defItem.Duration,
                                NoOfNightList = listNON,
                                StartingFromList = positionGetRes.DaysList,
                                PositionRoomsData = new PositionRoomsViewModel { RoomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } } }
                            });
                        }
                        else if (Acco != null)
                        {
                            dayno = Convert.ToInt32(Acco.DayName.Replace("Day ", "")) + Acco.NoOfNight;
                        }
                    }
                }
                //model.AccomodationData = model.AccomodationData.OrderBy(a => a.DayName).ToList();
                int i = 1;
                model.AccomodationData.ForEach(a => a.AccomodationSequence = i++);
                accomLibrary.BindAccomodationMasterData(_configuration, token, ref model, model.AccomodationData.Count);
            }
            else
            {
                if (DefaultList.Count > 0)
                {
                    model.AccomodationData.AddRange(DefaultList.Select(a => new AccomodationData
                    {
                        CityID = a.CityID,
                        CityName = a.CityName,
                        DayName = a.DayName,
                        DayNo = Convert.ToInt16(a.DayName?.Replace("Day ", "").Trim()),
                        AccomodationSequence = 1,
                        MealPlan = "BB",
                        KeepAs = "Included",
                        StartingFrom = a.DayNo,
                        StartTime = "20:00",
                        EndTime = "08:30",
                        NoOfNight = a.Duration,
                        NoOfNightList = listNON,
                        StartingFromList = positionGetRes.DaysList,
                        PositionRoomsData = new PositionRoomsViewModel { RoomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } } }
                    }));
                }

                if (model.AccomodationData.Count < 1)
                {
                    var obj = new AccomodationData();
                    obj.AccomodationSequence = 1;
                    obj.MealPlan = "BB";
                    obj.KeepAs = "Included";
                    obj.StartingFromList = positionGetRes.DaysList;
                    obj.StartingFrom = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id;
                    obj.DayName = "Day 1";
                    obj.StartTime = "20:00";
                    obj.EndTime = "08:30";
                    obj.NoOfNightList = listNON;
                    obj.PositionRoomsData.RoomDetails.Add(new RoomDetails { RoomSequence = 1, IsSupplement = false });
                    model.AccomodationData.Add(obj);
                }
                accomLibrary.BindAccomodationMasterData(_configuration, token, ref model, model.AccomodationData.Count);
            }
            
            model.AccomodationData = model.AccomodationData.OrderBy(a => a.DayNo).ToList();

            return model;
        }

        public PositionSetReq AccomodationSet(AccomodationViewModel model, string CurrentUser, string token)
        {
            PositionSetReq positionSetReq = new PositionSetReq();
            positionSetReq.SaveType = model.SaveType;
            positionSetReq.IsClone = model.MenuViewModel.IsClone;
            positionSetReq.mPosition = new List<mPosition>();

            List<string> ProductIds = new List<string>();
            model.AccomodationData.ForEach(a => ProductIds.Add(a.HotelID)); 
            var resultRooms = accomLibrary.GetRoomTypeAndSupplementList(ProductIds.Distinct().ToList(), model.MenuViewModel.QRFID, token)?.RoomTypeList;

            if (resultRooms?.Count>0)
            {
                foreach (var objModel in model.AccomodationData)
                {
                    var obj = new mPosition();

                    obj.QRFID = model.MenuViewModel.QRFID;
                    obj.PositionId = objModel.AccomodationId;

                    obj.PositionSequence = objModel.AccomodationSequence;
                    obj.ProductTypeId = objModel.ProductTypeId;
                    obj.ProductType = objModel.ProductType;
                    obj.CityName = objModel.CityName.Split(",")[0].Trim();
                    obj.CountryName = objModel.CityName.Split(",")[1].Trim();
                    obj.CityID = objModel.CityID;

                    obj.RoutingDaysID = objModel.StartingFrom;
                    obj.DayNo = Convert.ToInt16(objModel.DayName.Replace("Day ", ""));
                    obj.StartingFrom = objModel.DayName;

                    obj.Duration = objModel.NoOfNight;
                    if (!(string.IsNullOrEmpty(objModel.Category)))
                    {
                        obj.BudgetCategoryId = objModel.Category.Split("|")[0];
                        obj.BudgetCategory = objModel.Category.Split("|")[1];
                    }
                    obj.StarRating = objModel.StarRating;
                    obj.Location = objModel.Location;
                    obj.ChainName = objModel.ChainName;
                    obj.ChainID = objModel.ChainID;
                    obj.ProductName = objModel.HotelName;
                    obj.ProductID = objModel.HotelID;
                    obj.ProductType = objModel.ProductType;
                    obj.ProductTypeId = objModel.ProductTypeId;
                    obj.SupplierId = objModel.SupplierId;
                    obj.SupplierName = objModel.SupplierName;

                //Requirements
                obj.MealPlan = objModel.MealPlan;
                if (!string.IsNullOrEmpty(objModel.EarlyCheckInDate))
                {
                    var strFromDT = objModel.EarlyCheckInDate.Split("/");
                    if (strFromDT?.Count() >= 3)
                    {
                        DateTime fromDT = new DateTime(Convert.ToInt32(strFromDT[2]), Convert.ToInt32(strFromDT[1]), Convert.ToInt32(strFromDT[0]));
                        obj.EarlyCheckInDate = fromDT;
                    }
                    else
                    {
                        obj.EarlyCheckInDate = null;
                    }
                } 
                obj.EarlyCheckInTime = objModel.EarlyCheckInTime;
                obj.StartTime = objModel.StartTime;
                obj.EndTime = objModel.EndTime;
                //obj.NumberOfEarlyCheckInRooms = objModel.NumberOfEarlyCheckInRooms;

                    obj.InterConnectingRooms = objModel.NumberofInterConnectingRooms;
                    obj.WashChangeRooms = objModel.NumberOfWashChangeRooms;

                if (!string.IsNullOrEmpty(objModel.EarlyCheckInDate))
                {
                    var strFromDT = objModel.LateCheckOutDate.Split("/");
                    if (strFromDT?.Count() >= 3)
                    {
                        DateTime fromDT = new DateTime(Convert.ToInt32(strFromDT[2]), Convert.ToInt32(strFromDT[1]), Convert.ToInt32(strFromDT[0]));
                        obj.LateCheckOutDate = fromDT;
                    }
                    else
                    {
                        obj.LateCheckOutDate = null;
                    }
                } 
                obj.LateCheckOutTime = objModel.LateCheckOutTime;
                //obj.NumberOfLateCheckOutRooms = objModel.NumberOfLateCheckOutRooms;

                    //obj.Supplement = objModel.Supplement;
                    //obj.SupplementID = objModel.SupplementID;
                    //Requirements End

                    obj.KeepAs = objModel.KeepAs;
                    obj.TLRemarks = objModel.RemarksForTL;
                    obj.OPSRemarks = objModel.RemarksForOPS;

                    obj.CreateUser = CurrentUser;
                    obj.CreateDate = DateTime.Now;
                    obj.EditUser = CurrentUser;
                    obj.EditDate = DateTime.Now;

                    if (objModel.AccomodationSequence == 0)
                        obj.IsDeleted = true;

                    obj.RoomDetailsInfo = new List<RoomDetailsInfo>();

                    var roomsuppliments = new List<RoomDetails>();
                    var roomservices = new List<RoomDetails>();

                    roomservices = objModel.PositionRoomsData.RoomDetails.Where(a => !string.IsNullOrEmpty(a.ProductRange) && a.IsSupplement == false).
                                        OrderBy(a => a.ProductRange.Contains("SINGLE") ? "A" : a.ProductRange.Contains("DOUBLE") ? "B" : a.ProductRange.Contains("TWIN") ? "C" :
                                        a.ProductRange.Contains("TRIPLE") ? "D" : a.ProductRange.Contains("QUAD") ? "E" : a.ProductRange.Contains("TSU") ? "F" :
                                        a.ProductRange.ToLower().Contains("child + bed") ? "G" : a.ProductRange.ToLower().Contains("child - bed") ? "H" :
                                        a.ProductRange.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.ProductRange).ToList();

                    roomsuppliments = objModel.PositionRoomsData.RoomDetails.Where(a => !string.IsNullOrEmpty(a.ProductRange) && a.IsSupplement).OrderBy(a => a.ProductRange).ToList();
                    objModel.PositionRoomsData.RoomDetails = new List<RoomDetails>();
                    objModel.PositionRoomsData.RoomDetails.AddRange(roomservices);
                    objModel.PositionRoomsData.RoomDetails.AddRange(roomsuppliments);

                    foreach (var objRoomModel in objModel.PositionRoomsData.RoomDetails)
                    {
                        if (objRoomModel.RoomSequence > 0)
                        {
                            var objRoom = new RoomDetailsInfo(); 
                            objRoom.RoomId = objRoomModel.RoomId;
                            objRoom.RoomSequence = objRoomModel.RoomSequence;
                            if (!(string.IsNullOrEmpty(objRoomModel.ProductRange)))
                            {
                                objRoom.ProductCategoryId = objRoomModel.ProductRangeID.Split("|")[0];
                                objRoom.ProductRangeId = objRoomModel.ProductRangeID.Split("|")[1];

                                var prodRange = resultRooms.Where(a => a.VoyagerProductRange_Id == objRoom.ProductRangeId && a.ProductCategoryId == objRoom.ProductCategoryId).FirstOrDefault();
                                if (prodRange!=null)
                                {
                                    objRoom.ProductCategory = prodRange.ProductCategoryName;
                                    objRoom.ProductRange = prodRange.ProductRangeCode + " (" + prodRange.PersonType + (prodRange.AgeRange == null ? "" : " | " + prodRange.AgeRange) + ")";
                                }                               
                            }
                            objRoom.IsSupplement = objRoomModel.IsSupplement;
                            objRoom.CreateUser = CurrentUser;
                            objRoom.CreateDate = DateTime.Now;
                            objRoom.EditUser = CurrentUser;
                            objRoom.EditDate = DateTime.Now;
                            if (objRoomModel.RoomSequence == 0)
                                objRoom.IsDeleted = true;

                            obj.RoomDetailsInfo.Add(objRoom);
                        }

                        //if (objRoomModel.RoomSequence > 0)
                        //{
                        //    var objRoom = new RoomDetailsInfo();
                        //    string[] RoomType;

                        //    objRoom.RoomId = objRoomModel.RoomId;
                        //    objRoom.RoomSequence = objRoomModel.RoomSequence;                            
                        //    if (!(string.IsNullOrEmpty(objRoomModel.ProductRange)))
                        //    {
                        //        RoomType = objRoomModel.ProductRange.Split(')');
                        //        objRoom.ProductCategory = RoomType[0].Replace("(", "").Trim();
                        //        if (RoomType.Length > 2)
                        //            objRoom.ProductRange = RoomType[1].Trim() + ")";
                        //        else
                        //            objRoom.ProductRange = RoomType[1].Trim();

                        //        objRoom.ProductCategoryId = objRoomModel.ProductRangeID.Split("|")[0];
                        //        objRoom.ProductRangeId = objRoomModel.ProductRangeID.Split("|")[1];
                        //    }
                        //    objRoom.IsSupplement = objRoomModel.IsSupplement;
                        //    objRoom.CreateUser = CurrentUser;
                        //    objRoom.CreateDate = DateTime.Now;
                        //    objRoom.EditUser = CurrentUser;
                        //    objRoom.EditDate = DateTime.Now;
                        //    if (objRoomModel.RoomSequence == 0)
                        //        objRoom.IsDeleted = true;

                        //    obj.RoomDetailsInfo.Add(objRoom);
                        //}
                    }

                    positionSetReq.mPosition.Add(obj);
                }
            }
         
            return positionSetReq;
        }

        public bool ValidateAccomodationWithRouting(AccomodationViewModel model, string token)
        {
            bool flag = false;
            List<AttributeValues> listNON = new List<AttributeValues>();
            List<TourGuideDefaultRows> DefaultList = new List<TourGuideDefaultRows>();
            PositionGetReq positionGetReq = new PositionGetReq();
            PositionGetRes positionGetRes = new PositionGetRes();
            positionGetReq.QRFID = model.MenuViewModel.QRFID;
            //positionGetReq.PositionId = PositionId;
            positionGetReq.ProductType.Add(new ProductType { ProdType = "Hotel" });

            positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

            if (positionGetRes.RoutingInfo != null && positionGetRes.RoutingInfo.Count > 0)
            {
                int day = 0;
                int prevnight = 0;
                foreach (var Route in positionGetRes.RoutingInfo.Where(a => a.Nights > 0))
                {
                    if (day == 0) { day = 1; }
                    else { day = prevnight + day; }
                    //else day += 1;

                    DefaultList.Add(new TourGuideDefaultRows
                    {
                        DayNo = positionGetRes.DaysList.Where(a => a.Value == "Day " + day.ToString()).FirstOrDefault().AttributeValue_Id,
                        DayName = "Day " + day.ToString(),
                        Duration = Route.Nights,
                        CityID = Route.ToCityID,
                        CityName = Route.ToCityName,
                    });
                    //day += Route.Days - 1;
                    prevnight = Route.Nights;
                }
            }

            if (positionGetRes.ResponseStatus.Status == "Success" && positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
            {
                if (DefaultList.Count > 0)
                {
                    int count = 0;
                    foreach (var acco in model.AccomodationData.Where(a => a.AccomodationSequence > 0).ToList())
                    {
                        count = DefaultList.Where(a => a.CityID == acco.CityID && a.DayNo == acco.StartingFrom).Count();
                        if (count < 1)
                        {
                            flag = false; break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
            }

            return flag;
        }
         
        #endregion

        #region Bus
        public BusViewModel GetBusDetails(IConfiguration _configuration, string token, PositionGetReq objPositionGetReq)
        {
            BusViewModel model = new BusViewModel();
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model.MenuViewModel.QRFID = objPositionGetReq.QRFID;
            model.QRFID = objPositionGetReq.QRFID;

            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = objPositionGetReq.QRFID };
            bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

            string dropofflocnm = "";
            string dropofflocid = "";
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "PickUpDropOffLocations";
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "PickUpDropOffLocations")
                {
                    AutoCompleteTextBox pickUpDropOffLocations = new AutoCompleteTextBox();
                    var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList().Where(p => p.Value.ToLower().Contains("the hotel"));
                    pickUpDropOffLocations = result.Select(data => new AutoCompleteTextBox { value = data.AttributeValue_Id, label = data.Value }).FirstOrDefault();
                    dropofflocnm = pickUpDropOffLocations.label;
                    dropofflocid = pickUpDropOffLocations.value;
                }
            }

            PositionGetRes objPositionGetRes = positionProviders.GetPosition(objPositionGetReq, token).Result;
            List<AttributeValues> DurationList = new List<AttributeValues>();

            if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
            {
                var duration = objPositionGetRes.DaysList.Count;
                if (duration > 0)
                {
                    for (int i = 1; i <= duration; i++)
                    {
                        DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                    }
                }

                if (objPositionGetRes.mPosition != null && objPositionGetRes.mPosition.Count > 0)
                {
                    List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                    List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                    List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                    List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                    var ProdIdList = objPositionGetRes.mPosition.Select(a => a.ProductID).ToList();
                    var ProdcatIdList = objPositionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                    var ProdRange = new List<ProductRangeDetails>();

                    if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                    {
                        ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                    }

                    foreach (var item in objPositionGetRes.mPosition)
                    {
                        if (!string.IsNullOrEmpty(item.BudgetCategoryId))
                        {
                            listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                            if (listRange != null && listRange.Count > 0)
                            {
                                //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                                listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                            }
                            else
                            {
                                listRoomType = new List<ProductAttributeDetails>();
                                listRange = new List<ProductRangeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }
                        }
                        else
                        {
                            listRoomType = new List<ProductAttributeDetails>();
                            listRange = new List<ProductRangeDetails>();
                            listSupplements = new List<ProductAttributeDetails>();
                        }
                        listPositionRooms.Add(new PositionRoomsViewModel
                        {
                            PositionId = item.PositionId,
                            RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                               item.RoomDetailsInfo.Select(b => new RoomDetails
                               {
                                   RoomId = b.RoomId,
                                   RoomSequence = b.RoomSequence,
                                   ProductRange = b.ProductRange,
                                   ProductRangeID = b.ProductRangeId,
                                   IsSupplement = b.IsSupplement,
                                   RoomTypeList = b.IsSupplement ?
                                       new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                               }).ToList(),
                            SupplementList = listSupplements
                        });
                    }

                    model.BusDetails = objPositionGetRes.mPosition.Select(p => new BusDetails
                    {
                        BusID = p.PositionId,
                        DayID = p.RoutingDaysID,
                        DayName = objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault() != null ? objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault().Value : "",
                        DayList = objPositionGetRes.DaysList,
                        ProductType = p.ProductType,
                        ProductTypeID = p.ProductTypeId,
                        BudgetCategoryId = p.BudgetCategory,
                        BudgetCategory = p.BudgetCategoryId,
                        Duration = p.Duration,
                        PositionRoomsData = listPositionRooms.Where(b => b.PositionId == p.PositionId).FirstOrDefault(),
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        TLRemarks = p.TLRemarks,
                        OPSRemarks = p.OPSRemarks,
                        FromCity = !string.IsNullOrEmpty(p.CountryName) ? (p.CityName + ", " + p.CountryName.Trim()) : "",
                        FromCityID = p.CityID,
                        IsDeleted = p.IsDeleted,
                        KeepAs = p.KeepAs,
                        FromPickUpLoc = p.FromPickUpLoc,
                        FromPickUpLocID = p.FromPickUpLocID,
                        StartTime = p.StartTime,
                        ProdCategoryList = !string.IsNullOrEmpty(p.ProductID) ? GetProdCategoryDetails(p.ProductID, token) : new List<ProdCategoryDetails>(),
                        ToCity = !string.IsNullOrEmpty(p.ToCountryName) ? (p.ToCityName + ", " + p.ToCountryName.Trim()) : "",
                        ToCityID = p.ToCityID,
                        ToDropOffLoc = p.ToDropOffLoc,
                        ToDropOffLocID = p.ToDropOffLocID,
                        EndTime = p.EndTime,
                        DurationList = DurationList,
                        BusSequence = p.PositionSequence,
                        SupplierID = p.SupplierId,
                        SupplierName = p.SupplierName,
                        ForPositionId = p.ForPositionId,
                        IsCityPermit = p.IsCityPermit,
                        IsParkingCharges = p.IsParkingCharges,
                        IsRoadTolls = p.IsRoadTolls
                    }).ToList();
                }
                else
                {
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                    var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();
                    var deftocity = objPositionGetRes.DaysList.Where(a => a.Value == "Day 2").FirstOrDefault();

                    model.BusDetails.Add(new BusDetails
                    {
                        DayList = objPositionGetRes.DaysList,
                        DayName = "Day 1",
                        DayID = objPositionGetRes.DaysList.Count == 0 ? "" : objPositionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                        PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                        IsDeleted = false,
                        DurationList = DurationList,
                        Duration = 0,
                        BusSequence = 1,
                        FromCity = deffromcity != null ? deffromcity.CityName : "",
                        FromCityID = deffromcity != null ? deffromcity.CityId : "",
                        ToCity = deftocity != null ? deftocity.CityName : "",
                        ToCityID = deftocity != null ? deftocity.CityId : "",
                        ToDropOffLoc = dropofflocnm,
                        ToDropOffLocID = dropofflocid
                    });
                }
            }
            else
            {
                model.BusDetails.Add(new BusDetails());
            }
            model.Location = dropofflocnm;
            model.LocationId = dropofflocid;
            return model;
        }

        public List<ProdCategoryDetails> GetProdCategoryDetails(string ProductId, string token)
        {
            var lstProdCat = new List<ProdCategoryDetails>();
            ProductCatGetReq objProductCatGetReq = new ProductCatGetReq();
            objProductCatGetReq.ProductId = ProductId;
            ProductCatGetRes productCatGetRes = objMasterProviders.GetProductCategoryByParam(objProductCatGetReq, token).Result;

            if (productCatGetRes.ResponseStatus.Status.ToLower() == "success" && productCatGetRes.ProdCategoryDetails.Count > 0)
            {
                lstProdCat = productCatGetRes.ProdCategoryDetails;
            }
            return lstProdCat;
        }

        public PositionSetRes SetBusDetals(BusViewModel model, string CurrentUser, string token)
        {
            PositionSetRes objPositionSetRes = new PositionSetRes();
            PositionSetReq objPositionSetReq = new PositionSetReq { FOC = model.FOC, Price = model.Price, SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone,QRFID = model.QRFID,VoyagerUserID = model.VoyagerUserId };
            try
            {
                model.BusDetails.RemoveAll(a => string.IsNullOrWhiteSpace(a.BusID) && string.IsNullOrWhiteSpace(a.FromCity));

                objPositionSetReq.mPosition = model.BusDetails.Select(a => new mPosition
                {
                    PositionSequence = a.BusSequence,
                    QRFID = model.QRFID,
                    PositionId = a.BusID,
                    RoutingDaysID = a.DayID,
                    DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : Convert.ToInt32(a.DayName.Replace("Day ", "")),
                    StartingFrom = a.DayName,
                    ProductType = a.ProductType,
                    ProductTypeId = a.ProductTypeID,
                    CityID = a.FromCityID,
                    CityName = a.FromCity.Split(",")[0].Trim(),
                    ToCityID = a.ToCityID,
                    ToCityName = a.ToCity.Split(",")[0].Trim(),
                    CountryName = a.FromCity.Split(",")[1].Trim(),
                    ToCountryName = a.FromCity.Split(",")[1].Trim(),
                    Duration = a.Duration,
                    FromPickUpLoc = a.FromPickUpLoc,
                    FromPickUpLocID = a.FromPickUpLocID,
                    StartTime = a.StartTime,
                    ToDropOffLoc = a.ToDropOffLoc,
                    ToDropOffLocID = a.ToDropOffLocID,
                    EndTime = a.EndTime,
                    ProductID = a.ProductID,
                    ProductName = a.ProductName,
                    BudgetCategory = a.BudgetCategoryId,
                    BudgetCategoryId = a.BudgetCategory,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    KeepAs = a.KeepAs,
                    OPSRemarks = a.OPSRemarks,
                    TLRemarks = a.TLRemarks,
                    CreateUser = CurrentUser,
                    EditUser = CurrentUser,
                    IsDeleted = a.IsDeleted,
                    Status = "Q",
                    ForPositionId = a.ForPositionId,
                    IsRoadTolls = a.IsRoadTolls,
                    IsParkingCharges = a.IsParkingCharges,
                    IsCityPermit = a.IsCityPermit,
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductCategoryId = a.BudgetCategory,
                        ProductCategory = a.BudgetCategoryId,
                        ProductRangeId = b.ProductRangeID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();

                objPositionSetRes = positionProviders.SetPosition(objPositionSetReq, token).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objPositionSetRes;

        }
        #endregion

        #region Meals
        public MealsViewModel GetMealDetails(IConfiguration _configuration, string token, PositionGetReq objPositionGetReq, IRequestCookieCollection objCookies, string Day, List<ProductAttributeDetails> SessionInfo)
        {
            MealsViewModel model = new MealsViewModel();
            model.MenuViewModel.QRFID = objPositionGetReq.QRFID;

            PositionProviders objPositionProviders = new PositionProviders(_configuration);
            PositionGetRes objPositionGetRes = objPositionProviders.GetPosition(objPositionGetReq, token).Result;
            string username = "";
            objCookies.TryGetValue("UserName", out username);
            username = string.IsNullOrEmpty(username) ? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault() : username;
            var VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(objPositionGetReq.PositionId))
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = objPositionGetReq.QRFID };
                bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
                List<MealDetails> lstMealDetails = new List<MealDetails>();

                if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.RoutingDays != null && objPositionGetRes.RoutingDays.Count > 0)
                {
                    model.MealDetails = objPositionGetRes.MealDetails.OrderBy(m => m.DayNo).ToList();
                    SetMealInQuote(model.MealDetails, objPositionGetReq.QRFID, username, "full", token, null, objPositionGetRes.mPosition, VoyagerUserId);
                }
                else
                {
                    model.MealDetails = lstMealDetails;
                }
            }
            else if (!string.IsNullOrEmpty(objPositionGetReq.PositionId))
            {
                model = GetMealVenueById(objPositionGetRes, token, Day, objPositionGetReq.Type);
            }
            return model;
        }

        public MealsViewModel GetMealVenueById(PositionGetRes objPositionGetRes, string token, string Day, string mealtype)
        {
            MealsViewModel model = new MealsViewModel();
            VenueDetails objVenueDetails = new VenueDetails();
            if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.mPosition != null && objPositionGetRes.mPosition.Count > 0)
            {
                objVenueDetails.PositionId = objPositionGetRes.mPosition[0].PositionId;
                objVenueDetails.KeepAs = objPositionGetRes.mPosition[0].KeepAs;
                objVenueDetails.OPSRemarks = objPositionGetRes.mPosition[0].OPSRemarks;
                objVenueDetails.ProductId = objPositionGetRes.mPosition[0].ProductID;
                objVenueDetails.ProductName = objPositionGetRes.mPosition[0].ProductName;
                objVenueDetails.QRFID = objPositionGetRes.mPosition[0].QRFID;
                objVenueDetails.TLRemarks = objPositionGetRes.mPosition[0].TLRemarks;
                objVenueDetails.TransferDetails = objPositionGetRes.mPosition[0].TransferDetails;
                objVenueDetails.MealType = objPositionGetRes.mPosition[0].MealType;
                objVenueDetails.ProductType = objPositionGetRes.mPosition[0].ProductType;
                objVenueDetails.ProductTypeId = objPositionGetRes.mPosition[0].ProductTypeId;
                objVenueDetails.SupplierId = objPositionGetRes.mPosition[0].SupplierId;
                objVenueDetails.SupplierName = objPositionGetRes.mPosition[0].SupplierName;
                objVenueDetails.CityID = objPositionGetRes.mPosition[0].CityID;
                objVenueDetails.CityName = objPositionGetRes.mPosition[0].CityName.Trim() + ", " + objPositionGetRes.mPosition[0].CountryName.Trim();
                objVenueDetails.PositionSequence = objPositionGetRes.mPosition[0].PositionSequence;
                objVenueDetails.DayID = objPositionGetRes.mPosition[0].DayNo.ToString();
                objVenueDetails.RoutingDaysID = objPositionGetRes.mPosition[0].RoutingDaysID;
                objVenueDetails.ApplyAcrossDays = objPositionGetRes.mPosition[0].ApplyAcrossDays;
                objVenueDetails.PlaceHolder = objPositionGetRes.IsPlaceHolder;
                objVenueDetails.StartTime = objPositionGetRes.mPosition[0].StartTime;
                objVenueDetails.EndTime = objPositionGetRes.mPosition[0].EndTime;

                if (!string.IsNullOrEmpty(objVenueDetails.ProductName))
                {
                    SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                    ProductCatGetReq objProductCatGetReq = new ProductCatGetReq();
                    objProductCatGetReq.ProductId = objVenueDetails.ProductId;
                    ProductCatGetRes productCatGetRes = objMasterProviders.GetProductCategoryByParam(objProductCatGetReq, token).Result;

                    if (productCatGetRes.ResponseStatus.Status.ToLower() == "success" && productCatGetRes.ProdCategoryDetails.Count > 0)
                    {
                        objVenueDetails.ProdCategoryList = productCatGetRes.ProdCategoryDetails;

                        PositionRoomsViewModel objPositionRooms = new PositionRoomsViewModel();
                        List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                        List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                        List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();
                        List<MealProductRange> lstMealProductRange = new List<MealProductRange>();
                        MealProductRange objMealProductRange = new MealProductRange();

                        var catid = objPositionGetRes.mPosition[0].BudgetCategoryId;
                        var catname = objPositionGetRes.mPosition[0].BudgetCategory;
                        if (!string.IsNullOrEmpty(catid))
                        {
                            listRange = quoteLibrary.GetProductRangeList(objPositionGetRes.mPosition[0].ProductID, catid, null, token, "").ProductRangeDetails;
                            listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                            listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                        }
                        else
                        {
                            listRange = new List<ProductRangeDetails>();
                            listRoomType = new List<ProductAttributeDetails>();
                            listSupplements = new List<ProductAttributeDetails>();
                        }
                        objPositionRooms.PositionId = objPositionGetRes.mPosition[0].PositionId;
                        objPositionRooms.RoomDetails = (objPositionGetRes.mPosition[0].RoomDetailsInfo != null && objPositionGetRes.mPosition[0].RoomDetailsInfo.Count > 0) ?
                                        objPositionGetRes.mPosition[0].RoomDetailsInfo.Select(b => new RoomDetails
                                        {
                                            RoomId = b.RoomId,
                                            RoomSequence = b.RoomSequence,
                                            ProductRange = b.ProductRange,
                                            ProductRangeID = b.ProductRangeId,
                                            IsSupplement = b.IsSupplement,
                                            RoomTypeList = b.IsSupplement ? new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                        }).ToList() : new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } };

                        objPositionRooms.SupplementList = listSupplements;
                        objMealProductRange.PositionRoomsData = objPositionRooms;
                        lstMealProductRange.Add(objMealProductRange);

                        objVenueDetails.MealProductRange = lstMealProductRange;
                        objVenueDetails.BudgetCategory = catid;
                        objVenueDetails.BudgetCategoryId = catid;
                    }
                    else
                    {
                        objVenueDetails.ProdCategoryList = new List<ProdCategoryDetails>();
                        List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                        objVenueDetails.MealProductRange.Add(new MealProductRange { PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails } });
                    }
                }
                else
                {
                    objVenueDetails.ProdCategoryList = new List<ProdCategoryDetails>();
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                    objVenueDetails.MealProductRange.Add(new MealProductRange { PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails } });
                }
            }
            else
            {
                objVenueDetails.ProdCategoryList = new List<ProdCategoryDetails>();
                List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                objVenueDetails.MealProductRange.Add(new MealProductRange { PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails } });
                if (objPositionGetRes != null && objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
                {
                    var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == Day).FirstOrDefault();
                    if (deffromcity != null)
                    {
                        objVenueDetails.CityID = deffromcity.CityId;
                        objVenueDetails.CityName = deffromcity.CityName;
                    }
                }
                objVenueDetails.StartTime = mealtype == "earlymorningtea" ? MealTime.MorningTeaStartTime : mealtype == "breakfast" ? MealTime.BreakFastStartTime : mealtype == "brunch" ? MealTime.BrunchStartTime : mealtype == "lunch" ? MealTime.LunchStartTime : mealtype == "tea" ? MealTime.TeaStartTime : mealtype == "dinner" ? MealTime.DinnerStartTime : "00:00";
                objVenueDetails.EndTime = mealtype == "earlymorningtea" ? MealTime.MorningTeaEndTime : mealtype == "breakfast" ? MealTime.BreakFastEndTime : mealtype == "brunch" ? MealTime.BrunchEndTime : mealtype == "lunch" ? MealTime.LunchEndTime : mealtype == "tea" ? MealTime.TeaEndTime : mealtype == "dinner" ? MealTime.DinnerEndTime : "00:00";
            }
            objVenueDetails.QuoteRoomPassanger = objPositionGetRes != null && objPositionGetRes.AgentPassengerInfo != null && objPositionGetRes.AgentPassengerInfo.Count > 0
                   ? String.Join(",", objPositionGetRes.AgentPassengerInfo.Select(a => (a.Type == "CHILDWITHBED" || a.Type == "CHILDWITHOUTBED") ? "CHILD" : a.Type).Distinct().ToList()) : "";

            model.VenueDetails = objVenueDetails;
            return model;
        }

        public VenueDetails SetMealDetails(IConfiguration _configuration, string token, VenueDetails model)
        {
            PositionProviders objPositionProviders = new PositionProviders(_configuration);
            mPosition objmPosition = new mPosition();
            objmPosition.CityID = model.CityID;
            objmPosition.CityName = !string.IsNullOrEmpty(model.CityName) ? model.CityName.Split(',')[0] : "";
            objmPosition.CountryName = !string.IsNullOrEmpty(model.CityName) ? model.CityName.Split(',')[1] : "";
            objmPosition.CreateUser = model.UserName;
            objmPosition.DayNo = Convert.ToInt32(model.DayID.Replace("Day ", "").Trim());
            objmPosition.EditUser = model.UserName;
            objmPosition.IsDeleted = false;
            objmPosition.KeepAs = model.KeepAs;
            objmPosition.MealType = model.MealType;
            objmPosition.PositionId = model.PositionId;
            objmPosition.PositionSequence = model.PositionSequence;
            objmPosition.BudgetCategory = model.BudgetCategory;
            objmPosition.BudgetCategoryId = model.BudgetCategoryId;
            objmPosition.ProductID = model.ProductId;
            objmPosition.ProductName = model.ProductName;
            objmPosition.SupplierId = model.SupplierId;
            objmPosition.SupplierName = model.SupplierName;
            objmPosition.QRFID = model.QRFID;
            objmPosition.TLRemarks = model.TLRemarks;
            objmPosition.TransferDetails = model.TransferDetails;
            objmPosition.OPSRemarks = model.OPSRemarks;
            objmPosition.ProductType = model.ProductType;
            objmPosition.ProductTypeId = model.ProductTypeId;
            objmPosition.Status = "Q";
            objmPosition.SupplierId = model.SupplierId;
            model.RoomDetailsInfo.ForEach(p => { p.CreateUser = model.UserName; p.EditUser = model.UserName; });
            objmPosition.RoomDetailsInfo = model.RoomDetailsInfo;
            objmPosition.RoutingDaysID = model.RoutingDaysID;
            objmPosition.StartingFrom = "Day " + model.DayID;
            objmPosition.ApplyAcrossDays = model.ApplyAcrossDays;
            objmPosition.StartTime = model.StartTime;
            objmPosition.EndTime = model.EndTime;

            List<mPosition> lstPosition = new List<mPosition>();
            lstPosition.Add(objmPosition);
            PositionSetReq positionSetReq = new PositionSetReq { Price = model.Price, FOC = model.FOC, SaveType = "partial", mPosition = lstPosition, IsClone = model.IsClone ,QRFID = model.QRFID,VoyagerUserID = model.VoyagerUserID };
            PositionSetRes objPositionSetRes = objPositionProviders.SetPosition(positionSetReq, token).Result;
            model.ResponseStatus = objPositionSetRes.ResponseStatus;

            if (objPositionSetRes != null && objPositionSetRes.mPosition != null && objPositionSetRes.mPosition.Count > 0)
            {
                model.PositionId = objPositionSetRes.mPosition.FirstOrDefault().PositionId;
                model.QRFID = objPositionSetRes.mPosition.FirstOrDefault().QRFID;
                model.RoomDetailsInfo = objPositionSetRes.mPosition.FirstOrDefault().RoomDetailsInfo;
            }

            #region SetMealInQuote
            model.PositionDetails = objPositionSetRes.PositionDetails;
            if (objPositionSetRes.PositionDetails?.Count == 0)
            {
                var pos = objPositionSetRes.mPosition.FirstOrDefault();
                model.PositionDetails.Add(new PositionDetails
                {
                    RoutingDaysID = pos.RoutingDaysID,
                    Days = pos.StartingFrom,
                    PositionID = pos.PositionId,
                    ProductID=pos.ProductID
                });
            }
            if (model.PositionDetails?.Count > 0)
            {
                List<MealDays> lstMealDays = new List<MealDays>();
                var MealDayInfo = new List<MealDayInfo>();
                foreach (var item in model.PositionDetails)
                {
                    MealDayInfo = new List<MealDayInfo>();
                    MealDayInfo.Add(new MealDayInfo { MealType = model.MealType, PositionID = item.PositionID, StartTime = model.StartTime,ProductID= item.ProductID });
                    lstMealDays.Add(new MealDays { DayName = item.Days, RoutingDaysID = item.RoutingDaysID, MealDayInfo = MealDayInfo });
                }

                SetMealInQuote(null, model.QRFID, model.UserName, "partial", token, lstMealDays, null);
            }
            #endregion
            return model;
        }

        public PositionSetRes SetAllMealDetails(IConfiguration _configuration, string token, MealsViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            PositionProviders objPositionProviders = new PositionProviders(_configuration);
            List<mPosition> lstPosition = new List<mPosition>();
            PositionSetRes objPositionSetRes = new PositionSetRes();
            string username = "";
            objCookies.TryGetValue("EmailId", out username);
            username = string.IsNullOrEmpty(username) ? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault() : username;

            foreach (var item in model.MealDetails)
            {
                if (!string.IsNullOrEmpty(item.DinnerId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.DinnerId, IsDeleted = (item.IsDinner == true ? false : true), ProductType = "Meal", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.LunchId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.LunchId, IsDeleted = (item.IsLunch == true ? false : true), ProductType = "Meal", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.TeaId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.TeaId, IsDeleted = (item.IsTea == true ? false : true), ProductType = "Meal", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.EarlyMorningTeaId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.EarlyMorningTeaId, IsDeleted = (item.IsEarlyMorningTea == true ? false : true), ProductType = "Meal", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.BrunchId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.BrunchId, IsDeleted = (item.IsBrunch == true ? false : true), ProductType = "Meal", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.BreakfastId))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.BreakfastId, IsDeleted = (item.IsBreakfast == true ? false : true), ProductType = "Meal", EditUser = username });
                }
            }
            var positionSetRes = new PositionSetRes();

            var VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();

            if (lstPosition != null && lstPosition.Count > 0)
            {
                PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = lstPosition, Price = model.Price, FOC = model.FOC, IsClone = model.MenuViewModel.IsClone };
                positionSetReq.QRFID = model.MenuViewModel.QRFID;
                positionSetReq.VoyagerUserID = VoyagerUserId;
                positionSetRes = objPositionProviders.SetPosition(positionSetReq, token).Result;
                objPositionSetRes.mPosition = positionSetRes.mPosition;
                objPositionSetRes.PositionId = positionSetRes.PositionId;
                objPositionSetRes.ResponseStatus = positionSetRes.ResponseStatus;
            }
            else
            {
                objPositionSetRes.ResponseStatus = new ResponseStatus();
                objPositionSetRes.ResponseStatus.ErrorMessage = "Not checked anything";
                objPositionSetRes.ResponseStatus.Status = "Success";
            }
            SetMealInQuote(model.MealDetails, model.MenuViewModel.QRFID, username, "full", token, null, positionSetRes.mPosition,  VoyagerUserId);
            return objPositionSetRes;
        }

        public void SetMealInQuote(List<MealDetails> mealDetails, string QRFID, string username, string flag, string token, List<MealDays> model = null, List<mPosition> lstPos = null, string VoyagerUserId = null)
        {
           
            List<MealDays> lstMealDays = new List<MealDays>();
            var MealDayInfo = new List<MealDayInfo>();
            var st = "";
            mPosition pos = new mPosition();

            if (flag == "full")
            {
                foreach (var item in mealDetails)
                {
                    MealDayInfo = new List<MealDayInfo>();
                    if ((item.IsEarlyMorningTea && !string.IsNullOrEmpty(item.EarlyMorningTeaId)) || (!item.IsEarlyMorningTea && !string.IsNullOrEmpty(item.EarlyMorningTea) && item.EarlyMorningTea.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.EarlyMorningTeaId).FirstOrDefault();
                        
                        st = pos != null ? pos.StartTime : MealTime.MorningTeaStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Early Morning Tea", PositionID = item.EarlyMorningTeaId, StartTime = st });
                    }
                    if ((item.IsBreakfast && !string.IsNullOrEmpty(item.BreakfastId)) || (!item.IsBreakfast && !string.IsNullOrEmpty(item.Breakfast) && item.Breakfast.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.BreakfastId).FirstOrDefault();
                        st = pos != null ? pos.StartTime : MealTime.BreakFastStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Breakfast", PositionID = item.BreakfastId, StartTime = st });
                    }
                    if ((item.IsBrunch && !string.IsNullOrEmpty(item.BrunchId)) || (!item.IsBrunch && !string.IsNullOrEmpty(item.Brunch) && item.Brunch.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.BrunchId).FirstOrDefault();
                        st = pos != null ? pos.StartTime : MealTime.BrunchStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Brunch", PositionID = item.BrunchId, StartTime = st });
                    }
                    if ((item.IsLunch && !string.IsNullOrEmpty(item.LunchId)) || (!item.IsLunch && !string.IsNullOrEmpty(item.Lunch) && item.Lunch.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.LunchId).FirstOrDefault();
                        st = pos != null ? pos.StartTime : MealTime.LunchStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Lunch", PositionID = item.LunchId, StartTime = st });
                    }
                    if ((item.IsTea && !string.IsNullOrEmpty(item.TeaId)) || (!item.IsTea && !string.IsNullOrEmpty(item.Tea) && item.Tea.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.TeaId).FirstOrDefault();
                        st = pos != null ? pos.StartTime : MealTime.TeaStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Tea", PositionID = item.TeaId, StartTime = st });
                    }
                    if ((item.IsDinner && !string.IsNullOrEmpty(item.DinnerId)) || (!item.IsDinner && !string.IsNullOrEmpty(item.Dinner) && item.Dinner.ToLower() == "included in hotel"))
                    {
                        pos = lstPos.Where(a => a.PositionId == item.DinnerId).FirstOrDefault();
                        st = pos != null ? pos.StartTime : item.Dinner.ToLower() == "included in hotel"? MealTime.DinnerStartTimeInHotel :MealTime.DinnerStartTime;
                        MealDayInfo.Add(new MealDayInfo { MealType = "Dinner", PositionID = item.DinnerId, StartTime = st });
                    }
                    if (MealDayInfo.Count > 0)
                    {
                        lstMealDays.Add(new MealDays { DayName = item.DayID, RoutingDaysID = item.RoutingDaysID, MealDayInfo = MealDayInfo });
                    }
                }
            }
            else if (model?.Count > 0)
            {
                lstMealDays = model;
            }

            MealSetReq req = new MealSetReq() { MealDays = lstMealDays, QRFID = QRFID, UserName = username, Flag = flag, VoyagerUserId = VoyagerUserId };
            MealSetRes res = positionProviders.SetMeals(req, token).Result;
        }
        #endregion

        #region Activities
        public bool ActivitiesGet(PositionGetRes positionGetRes, PosQuicePickGetRes QuickPickGetRes, ref ActivitiesViewModel model, string token)
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                QuoteSearchViewModel modelMaster = new QuoteSearchViewModel();
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = model.QRFID };
                List<AttributeValues> TourList = new List<AttributeValues>();
                List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };

                #region bind position data from response to model
                if (positionGetRes != null && positionGetRes.ResponseStatus.Status.ToLower() == "success" && positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
                {
                    bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                    if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

                    modelMaster = quoteLibrary.BindMasterData(_configuration, token, "ActivityTypeMaster");
                    //TourList = positionGetRes.mPosition.Select(a => new AttributeValues { AttributeValue_Id = a.BudgetCategoryId, Value = a.BudgetCategory }).ToList();

                    if (positionGetRes.mPosition != null && positionGetRes.mPosition.Count > 0)
                    {
                        List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                        List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                        List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                        List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                        var ProdIdList = positionGetRes.mPosition.Select(a => a.ProductID).ToList();
                        var ProdcatIdList = positionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                        var ProdRange = new List<ProductRangeDetails>();

                        if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                        {
                            ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                        }

                        foreach (var item in positionGetRes.mPosition)
                        {
                            if (!string.IsNullOrEmpty(item.BudgetCategoryId))
                            {
                                //listRoomType = quoteLibrary.GetTicketTypeList(item.ProductID, item.BudgetCategoryId, "false", token).ProductRangeDetails.Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeName + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).ToList();
                                //listSupplements = quoteLibrary.GetTicketTypeList(item.ProductID, item.BudgetCategoryId, "true", token).ProductRangeDetails.Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeName }).ToList();

                                // listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;                               
                                listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                                if (listRange != null && listRange.Count > 0)
                                {
                                    //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                    listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                                    listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                                }
                                else
                                {
                                    listRoomType = new List<ProductAttributeDetails>();
                                    listRange = new List<ProductRangeDetails>();
                                    listSupplements = new List<ProductAttributeDetails>();
                                }

                                ProductCatGetReq objProdCatGetReq = new ProductCatGetReq { ProductId = item.ProductID };
                                ProductCatGetRes objMasterTypeResponse = objMasterProviders.GetProductCategoryByParam(objProdCatGetReq, token).Result;

                                if (objMasterTypeResponse.ResponseStatus.Status == "Success" && objMasterTypeResponse.ProdCategoryDetails.Count > 0)
                                {
                                    List<AttributeValues> CategoryList = new List<AttributeValues>();
                                    CategoryList = objMasterTypeResponse.ProdCategoryDetails.Select(data => new AttributeValues { AttributeValue_Id = data.ProductCategoryId, Value = data.ProductCategoryName, CityId = item.PositionId }).ToList();
                                    TourList.AddRange(CategoryList);
                                }
                            }
                            else
                            {
                                listRoomType = new List<ProductAttributeDetails>();
                                listRange = new List<ProductRangeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }
                            listPositionRooms.Add(new PositionRoomsViewModel
                            {
                                PositionId = item.PositionId,
                                RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                                    item.RoomDetailsInfo.Select(b => new RoomDetails
                                    {
                                        RoomId = b.RoomId,
                                        RoomSequence = b.RoomSequence,
                                        ProductRange = b.ProductRange,
                                        ProductRangeID = b.ProductRangeId,
                                        IsSupplement = b.IsSupplement,
                                        RoomTypeList = b.IsSupplement ?
                                            new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                    }).ToList(),
                                SupplementList = listSupplements
                            });
                        }

                        model.ActivityDetails = positionGetRes.mPosition.OrderBy(a => a.DayNo).ThenBy(a => a.StartTime).Select(a => new ActivityDetails
                        {
                            ActivityId = Guid.Parse(a.PositionId),
                            CityID = a.CityID,
                            CityName = a.CityName + ", " + a.CountryName,
                            StartTime = a.StartTime,
                            EndTime = a.EndTime,
                            RemarksOPS = a.OPSRemarks,
                            RemarksTL = a.TLRemarks,
                            KeepAs = string.IsNullOrEmpty(a.KeepAs) ? "Included" : a.KeepAs,
                            NoOfPaxAdult = a.NoOfPaxAdult == null ? 0 : Convert.ToInt32(a.NoOfPaxAdult),
                            NoOfPaxChild = a.NoOfPaxChild == null ? 0 : Convert.ToInt32(a.NoOfPaxChild),
                            NoOfPaxInfant = a.NoOfPaxInfant == null ? 0 : Convert.ToInt32(a.NoOfPaxInfant),
                            ProductID = a.ProductID,
                            ProductName = a.ProductName,
                            ProductType = a.ProductType,
                            ProductTypeID = a.ProductTypeId,
                            SupplierID = a.SupplierId,
                            SupplierName = a.SupplierName,
                            TourType = a.BudgetCategory,
                            TourTypeID = a.BudgetCategoryId,
                            TypeOfExcursion = a.TypeOfExcursion,
                            TypeOfExcursionID = a.TypeOfExcursion_Id,
                            TransferDetails = a.TransferDetails,
                            IsDeleted = false,
                            Status = a.Status,
                            ActivityDayNo = a.RoutingDaysID,
                            DayList = positionGetRes.DaysList,
                            DayName = positionGetRes.DaysList.Where(r => r.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault() != null ? positionGetRes.DaysList.Where(p => p.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault().Value : "",
                            TourTypeList = TourList.Where(b => b.CityId == a.PositionId).ToList(),
                            TypeOfExcursionList = modelMaster.ExcursionTypeList,
                            PositionRoomsData = listPositionRooms.Where(b => b.PositionId == a.PositionId).FirstOrDefault()
                        }).ToList();
                    }
                    else
                    {
                        var deffromcity = positionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                        model.ActivityDetails.Add(new ActivityDetails
                        {
                            CityName = deffromcity != null ? deffromcity.CityName : "",
                            CityID = deffromcity != null ? deffromcity.CityId : "",
                            StartTime = "10:00",
                            DayList = positionGetRes.DaysList,
                            DayName = "Day 1",
                            ActivityDayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                            TypeOfExcursionList = modelMaster.ExcursionTypeList,
                            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                            IsDeleted = false
                        });
                    }
                }
                else
                {
                    model.ActivityDetails.Add(new ActivityDetails());
                }
                #endregion

                #region Assign Quice Pick data from response to model
                if (QuickPickGetRes != null && QuickPickGetRes.ResponseStatus.Status.ToLower() == "success" && QuickPickGetRes.PosQuickPickList != null && QuickPickGetRes.PosQuickPickList.Count > 0)
                {
                    model.QRFID = QuickPickGetRes.QRFID;
                    foreach (var item in QuickPickGetRes.PosQuickPickList)
                    {
                        model.QuickPickActivities.AddRange(item.PosQuickPickProductList.Select(a => new QuickPickActivities
                        {
                            CityID = item.CityID,
                            CityName = item.CityName,
                            IsSelected = a.IsSelected,
                            PositionID = a.PositionId,
                            ProdID = a.ProdId,
                            ProdName = a.ProdName,
                            ProdCode = a.ProdCode,
                            ProdType = a.ProdType,
                            ProdTypeID = a.ProdTypeId,
                            StartTime = string.IsNullOrEmpty(a.StartTime) ? "10:00" : a.StartTime,
                            ActivityDayNo = a.ActivityDayNo,
                            DayName = a.DayName,
                            SupplierID = a.SupplierID,
                            SupplierName = a.SupplierName,
                            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails }
                        }).ToList());
                    }
                    model.QuickPickActivities = model.QuickPickActivities.OrderByDescending(a => a.IsSelected).ThenBy(a => a.DayName).ThenBy(a => a.QPSeqNo).ToList();
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PositionSetReq ActivitiesSet(ActivitiesViewModel model, string CurrentUser)
        {
            PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone };

            try
            {
                positionSetReq.mPosition = model.ActivityDetails.Select(a => new mPosition
                {
                    QRFID = model.QRFID,
                    PositionId = a.ActivityId.ToString(),
                    StartingFrom = a.DayName,
                    DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : (string.IsNullOrEmpty(a.DayName.Split(' ')[1]) ? 0 : Convert.ToInt32(a.DayName.Split(' ')[1])),
                    RoutingDaysID = a.ActivityDayNo,
                    CityID = a.CityID,
                    CityName = a.CityName.Split(",")[0].Trim(),
                    CountryName = a.CityName.Split(",")[1].Trim(),
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    //EndTime = (a.StartTime.Length >= 5 && a.StartTime.Contains(':')) ? (Convert.ToInt32(a.StartTime.Split(':')[0]) + 2).ToString() + ":" + a.StartTime.Split(':')[1] : a.StartTime,
                    ProductID = a.ProductID,
                    ProductName = a.ProductName,
                    ProductType = a.ProductType,
                    ProductTypeId = a.ProductTypeID,
                    BudgetCategory = a.TourType,
                    BudgetCategoryId = a.TourTypeID,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    TypeOfExcursion = a.TypeOfExcursion,
                    TypeOfExcursion_Id = a.TypeOfExcursionID,
                    TransferDetails = a.TransferDetails,
                    KeepAs = a.KeepAs,
                    NoOfPaxAdult = a.NoOfPaxAdult,
                    NoOfPaxChild = a.NoOfPaxChild,
                    NoOfPaxInfant = a.NoOfPaxInfant,
                    OPSRemarks = a.RemarksOPS,
                    TLRemarks = a.RemarksTL,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser,
                    EditDate = DateTime.Now,
                    EditUser = CurrentUser,
                    IsDeleted = a.IsDeleted,
                    Status = "Q",
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductRangeId = b.ProductRangeID,
                        ProductCategory = a.TourType,
                        ProductCategoryId = a.TourTypeID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return positionSetReq;
        }

        public PositionSetReq QuickPickActivitiesSet(ActivitiesViewModel ActivityModel, List<RoutingDays> RoutingDays, string CurrentUser, PositionGetRes positionGetRes)
        {
            PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = new List<mPosition>(), IsClone = ActivityModel.MenuViewModel.IsClone };
            try
            {
                int time, DayCount = 0;
                string NextDayId, NextDayName, NextDayNo;
                QuickPickActivities nextday, currentday;
                List<QuickPickActivities> sortList;

                //PositionGetReq positionGetReq = new PositionGetReq { QRFID = ActivityModel.QRFID };
                //positionGetReq.ProductType.Add(new ProductType { ProdType = "Attractions" });
                //positionGetReq.ProductType.Add(new ProductType { ProdType = "Sightseeing - CityTour" });
                //PositionGetRes positionGetRes = positionProviders.GetPosition(positionGetReq, token).Result;

                var daylist = ActivityModel.QuickPickActivities.Where(b => b.IsSelected == true).Select(a => new { a.DayName, a.CityName }).Distinct().ToList();
                foreach (var dayWise in daylist)
                {
                    if ((DayCount == 0) || (DayCount > 0 && daylist[DayCount].CityName != daylist[DayCount - 1].CityName))
                    {
                        //fetch start time of the last activity for the day already saved in positions
                        currentday = ActivityModel.QuickPickActivities.Where(b => b.IsSelected == true && b.DayName == dayWise.DayName && !string.IsNullOrEmpty(b.PositionID))
                            .OrderByDescending(a => a.StartTime).FirstOrDefault();


                        sortList = ActivityModel.QuickPickActivities.Where(b => b.IsSelected == true && b.DayName == dayWise.DayName && string.IsNullOrEmpty(b.PositionID)).OrderBy(a => a.DayName).ThenBy(c => c.QPSeqNo).ToList();

                        //if no activities are already saved for the day then initialize it as 10AM
                        //if (string.IsNullOrEmpty(currentday.StartTime) || currentday.StartTime == "10:00")
                        if (currentday == null)
                        {
                            if (currentday == null && sortList != null && sortList.Count > 0)
                                currentday = sortList.FirstOrDefault();

                            time = 10;
                            NextDayId = currentday.ActivityDayNo;
                            NextDayName = currentday.DayName;
                        }
                        else
                        {
                            //or else add 3 hours in the last activity start time for the next activity
                            if (!string.IsNullOrEmpty(currentday.StartTime)) time = Convert.ToInt32(currentday.StartTime?.Split(':')[0]);
                            else time = 10;

                            if (time < 1)
                            {
                                time = 10;
                                NextDayId = currentday.ActivityDayNo;
                                NextDayName = currentday.DayName;
                            }
                            else if (time < 22)
                            {
                                time += 3;
                                NextDayId = currentday.ActivityDayNo;
                                NextDayName = currentday.DayName;
                            }
                            else
                            {
                                NextDayNo = currentday.DayName.Split(" ")[1];
                                nextday = ActivityModel.QuickPickActivities.Where(a => a.IsSelected == true && a.DayName == ("Day " + (Convert.ToInt32(NextDayNo) + 1)) && a.CityName == currentday.CityName && !string.IsNullOrEmpty(a.PositionID)).OrderByDescending(a => a.StartTime).FirstOrDefault();
                                if (nextday != null)
                                {
                                    //time = Convert.ToInt32(nextday.StartTime.Split(':')[0]);
                                    NextDayId = nextday.ActivityDayNo;
                                    NextDayName = nextday.DayName;

                                    if (!string.IsNullOrEmpty(nextday.StartTime) && nextday.StartTime?.Split(':').Length > 1)
                                    {
                                        time = Convert.ToInt32(nextday.StartTime?.Split(':')[0]);
                                        if (time < 1) time = 10;
                                        else if (time < 22) time += 3;
                                        //else time = 24;
                                    }
                                    else
                                    {
                                        time = 10;
                                    }
                                }
                                else
                                {
                                    var nextrouting = RoutingDays.Where(a => a.DayNo == (Convert.ToInt32(NextDayNo) + 1) && a.FromCityName == currentday.CityName).FirstOrDefault();

                                    if (nextrouting != null)
                                    {
                                        time = 10;
                                        NextDayId = nextrouting.RoutingDaysID;
                                        NextDayName = nextrouting.Days;
                                    }
                                    else
                                    {
                                        //time = time;
                                        NextDayId = currentday.ActivityDayNo;
                                        NextDayName = currentday.DayName;
                                        //NextDayId = nextrouting.RoutingDaysID;
                                        //NextDayName = nextrouting.Days;
                                    }
                                }
                            }
                        }

                        foreach (var item in sortList)
                        {//10:00
                         //if last activity start time exists then add 3 hours in the last activity start time for the next activity
                            if (!string.IsNullOrEmpty(item.StartTime) && item.StartTime?.Split(':').Length > 1)
                            {
                                bool bWhile = true;
                                while (bWhile)
                                {
                                    if (time < 22 && positionGetRes.mPosition.Where(a => a.DayNo == Convert.ToInt32(item.DayName?.Split(" ")[1]) && a.StartTime == (time).ToString() + ":" + item.StartTime?.Split(':')[1]).Count() > 0)
                                    {
                                        time += 3;
                                    }
                                    else
                                    {
                                        bWhile = false;
                                    }
                                }
                                item.StartTime = (time).ToString() + ":" + item.StartTime?.Split(':')[1];
                                item.ActivityDayNo = NextDayId;
                                item.DayName = NextDayName;

                                if (time < 22)
                                {
                                    time += 3;
                                }
                                else
                                {
                                    NextDayNo = currentday.DayName.Split(" ")[1];
                                    nextday = sortList.Where(a => a.DayName == ("Day " + (Convert.ToInt32(NextDayNo) + 1)) && a.CityName == item.CityName).OrderByDescending(a => a.StartTime).FirstOrDefault();
                                    if (nextday != null)
                                    {
                                        time = Convert.ToInt32(nextday.StartTime?.Split(':')[0]);
                                        NextDayId = nextday.ActivityDayNo;
                                        NextDayName = nextday.DayName;

                                        if (string.IsNullOrEmpty(nextday.StartTime) || nextday.StartTime == "10:00") time = 10;
                                        else
                                        {
                                            time = Convert.ToInt32(nextday.StartTime?.Split(':')[0]);
                                            if (time < 1) time = 10;
                                            else if (time < 22) time += 3;
                                            else time = 24;
                                        }
                                    }
                                    else
                                    {
                                        var nextrouting = RoutingDays.Where(a => a.DayNo == (Convert.ToInt32(NextDayNo) + 1) && a.FromCityName == item.CityName).FirstOrDefault();

                                        if (nextrouting != null)
                                        {
                                            time = 10;
                                            NextDayId = nextrouting.RoutingDaysID;
                                            NextDayName = nextrouting.Days;
                                        }
                                    }
                                }
                                //time = Convert.ToInt32(item.StartTime.Split(':')[0]);
                            }
                            else
                            {
                                //or else initialize it as 10AM
                                item.StartTime = "10:00";
                            }
                            //ActivityModel.QuickPickActivities.Where(a=>a.QPSeqNo == item.QPSeqNo).FirstOrDefault
                        }
                    }
                    DayCount += 1;
                }

                positionSetReq.mPosition = ActivityModel.QuickPickActivities.Where(b => b.IsSelected == true && string.IsNullOrEmpty(b.PositionID)).Select(a => new mPosition
                {
                    QRFID = ActivityModel.QRFID,
                    PositionId = Guid.Empty.ToString(),
                    StartingFrom = a.DayName,
                    DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : (string.IsNullOrEmpty(a.DayName.Split(' ')[1]) ? 0 : Convert.ToInt32(a.DayName.Split(' ')[1])),
                    RoutingDaysID = a.ActivityDayNo,
                    CityID = a.CityID,
                    CityName = a.CityName.Split(",")[0].Trim(),
                    CountryName = a.CityName.Split(",")[1].Trim(),
                    StartTime = a.StartTime,
                    EndTime = (a.StartTime.Length >= 5 && a.StartTime.Contains(':')) ? (Convert.ToInt32(a.StartTime.Split(':')[0]) + 2).ToString() + ":" + a.StartTime.Split(':')[1] : a.StartTime,
                    ProductID = a.ProdID,
                    ProductName = a.ProdName,
                    ProductType = a.ProdType,
                    ProductTypeId = a.ProdTypeID,
                    BudgetCategory = a.ProdCategory,
                    BudgetCategoryId = a.ProdCategoryID,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    TransferDetails = "Through Coach",
                    KeepAs = "Included",
                    NoOfPaxAdult = 0,
                    NoOfPaxChild = 0,
                    NoOfPaxInfant = 0,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser,
                    EditDate = DateTime.Now,
                    EditUser = CurrentUser,
                    IsDeleted = false,
                    Status = "Q",
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductRangeId = b.ProductRangeID,
                        ProductCategory = a.ProdCategory,
                        ProductCategoryId = a.ProdCategoryID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return positionSetReq;
        }
        #endregion

        #region Cruise
        public CruiseViewModel GetCruiseDetails(IConfiguration _configuration, string token, PositionGetReq objPositionGetReq)
        {
            CruiseViewModel model = new CruiseViewModel();
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model.MenuViewModel.QRFID = objPositionGetReq.QRFID;
            model.QRFID = objPositionGetReq.QRFID;

            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = objPositionGetReq.QRFID };

            bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

            PositionGetRes objPositionGetRes = positionProviders.GetPosition(objPositionGetReq, token).Result;
            List<AttributeValues> DurationList = new List<AttributeValues>();

            if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
            {
                var duration = objPositionGetRes.DaysList.Count;
                if (duration > 0)
                {
                    for (int i = 1; i <= duration; i++)
                    {
                        DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                    }
                }

                if (objPositionGetRes.mPosition != null && objPositionGetRes.mPosition.Count > 0)
                {
                    List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                    List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                    List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                    List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                    var ProdIdList = objPositionGetRes.mPosition.Select(a => a.ProductID).ToList();
                    var ProdcatIdList = objPositionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                    var ProdRange = new List<ProductRangeDetails>();

                    if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                    {
                        ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                    }

                    foreach (var item in objPositionGetRes.mPosition)
                    {
                        if (!string.IsNullOrEmpty(item.BudgetCategoryId))
                        {
                            listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                            if (listRange != null && listRange.Count > 0)
                            {
                                //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).ToList().
                                       Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).
                                       OrderBy(a => a.Value.Contains("SINGLE") ? "A" : a.Value.Contains("DOUBLE") ? "B" : a.Value.Contains("TWIN") ? "C" : a.Value.Contains("TRIPLE") ? "D" :
                                                          a.Value.Contains("QUAD") ? "E" : a.Value.Contains("TSU") ? "F" :
                                                          a.Value.ToLower().Contains("child + bed") ? "G" : a.Value.ToLower().Contains("child - bed") ? "H" :
                                                          a.Value.ToLower().Contains("infant") ? "I" : "J").ThenBy(a => a.Value).ToList();

                                listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                            }
                            else
                            {
                                listRoomType = new List<ProductAttributeDetails>();
                                listRange = new List<ProductRangeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }
                        }
                        else
                        {
                            listRange = new List<ProductRangeDetails>();
                            listRoomType = new List<ProductAttributeDetails>();
                            listSupplements = new List<ProductAttributeDetails>();
                        }

                        listPositionRooms.Add(new PositionRoomsViewModel
                        {
                            PositionId = item.PositionId,
                            RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                            item.RoomDetailsInfo.Select(b => new RoomDetails
                            {
                                RoomId = b.RoomId,
                                RoomSequence = b.RoomSequence,
                                ProductRange = b.ProductRange,
                                ProductRangeID = b.ProductRangeId,
                                IsSupplement = b.IsSupplement,
                                RoomTypeList = b.IsSupplement ?
                                    new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                            }).ToList(),
                            SupplementList = listSupplements
                        });
                    }
                    int i = 1;
                    model.CruiseDetails = objPositionGetRes.mPosition.OrderBy(a => a.DayNo).Select(p => new CruiseDetails
                    {
                        PositionId = p.PositionId,
                        ProductType = p.ProductType,
                        ProductTypeID = p.ProductTypeId,
                        BudgetCategoryId = p.BudgetCategory,
                        BudgetCategory = p.BudgetCategoryId,
                        TransferDetails = p.TransferDetails,
                        DayID = p.RoutingDaysID,
                        DayName = objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault() != null ? objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault().Value : "",
                        DayList = objPositionGetRes.DaysList,
                        Duration = p.Duration,
                        CityID = p.CityID,
                        CityName = p.CityName + ", " + p.CountryName.Trim(),
                        PositionSequence = i++,
                        KeepAs = string.IsNullOrEmpty(p.KeepAs) ? "Included" : p.KeepAs,
                        NoOfPaxAdult = p.NoOfPaxAdult == null ? 0 : Convert.ToInt32(p.NoOfPaxAdult),
                        NoOfPaxChild = p.NoOfPaxChild == null ? 0 : Convert.ToInt32(p.NoOfPaxChild),
                        NoOfPaxInfant = p.NoOfPaxInfant == null ? 0 : Convert.ToInt32(p.NoOfPaxInfant),
                        PositionRoomsData = listPositionRooms.Where(b => b.PositionId == p.PositionId).FirstOrDefault(),
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        TLRemarks = p.TLRemarks,
                        OPSRemarks = p.OPSRemarks,
                        IsDeleted = p.IsDeleted,
                        StartTime = p.StartTime,
                        ProdCategoryList = GetProdCategoryDetails(p.ProductID, token),
                        DurationList = DurationList,
                        SupplierID = p.SupplierId,
                        SupplierName = p.SupplierName
                    }).ToList();
                }
                else
                {
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                    var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                    model.CruiseDetails.Add(new CruiseDetails
                    {
                        CityName = deffromcity != null ? deffromcity.CityName : "",
                        CityID = deffromcity != null ? deffromcity.CityId : "",
                        DayList = objPositionGetRes.DaysList,
                        DayName = "Day 1",
                        DayID = objPositionGetRes.DaysList.Count == 0 ? "" : objPositionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                        PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                        IsDeleted = false,
                        DurationList = DurationList,
                        Duration = 0,
                        PositionSequence = 1,
                        ProdCategoryList = new List<ProdCategoryDetails>(),
                        NoOfPaxAdult = 0,
                        NoOfPaxChild = 0,
                        NoOfPaxInfant = 0
                    });
                }
            }
            else
            {
                model.CruiseDetails.Add(new CruiseDetails());
            }
            return model;
        }

        public PositionSetRes SetCruiseDetals(CruiseViewModel model, string CurrentUser, string token)
        {
            PositionSetRes objPositionSetRes = new PositionSetRes();
            PositionSetReq objPositionSetReq = new PositionSetReq { FOC = model.FOC, Price = model.Price, SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone,QRFID = model.QRFID ,VoyagerUserID = model.VoyagerUserId};
            try
            {
                mPosition objposition = new mPosition();
                List<mPosition> lstposition = new List<mPosition>();
                foreach (var a in model.CruiseDetails)
                {
					objposition = new mPosition();
					objposition.PositionSequence = a.PositionSequence;
                    objposition.QRFID = model.QRFID;
                    objposition.PositionId = a.PositionId;
                    objposition.DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : Convert.ToInt32(a.DayName.Replace("Day ", ""));
                    objposition.RoutingDaysID = a.DayID;
                    objposition.StartingFrom = a.DayName;
                    objposition.ProductType = a.ProductType;
                    objposition.ProductTypeId = a.ProductTypeID;
                    objposition.TransferDetails = a.TransferDetails;
                    objposition.CityID = a.CityID;
                    objposition.CityName = a.CityName.Split(",")[0].Trim();
                    objposition.CountryName = a.CityName.Split(",")[1].Trim();
                    objposition.Duration = a.Duration;
                    objposition.StartTime = a.StartTime;
                    objposition.ProductID = a.ProductID;
                    objposition.ProductName = a.ProductName;
                    objposition.BudgetCategory = a.BudgetCategoryId;
                    objposition.BudgetCategoryId = a.BudgetCategory;
                    objposition.SupplierId = a.SupplierID;
                    objposition.SupplierName = a.SupplierName;
                    objposition.KeepAs = a.KeepAs;
                    objposition.OPSRemarks = a.OPSRemarks;
                    objposition.TLRemarks = a.TLRemarks;
                    objposition.CreateUser = CurrentUser;
                    objposition.EditUser = CurrentUser;
                    objposition.IsDeleted = a.IsDeleted;
                    objposition.Status = "Q";
                    objposition.NoOfPaxAdult = a.NoOfPaxAdult;
                    objposition.NoOfPaxChild = a.NoOfPaxChild;
                    objposition.NoOfPaxInfant = a.NoOfPaxInfant;
                    objposition.RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductCategoryId = a.BudgetCategory,
                        ProductCategory = a.BudgetCategoryId,
                        ProductRangeId = b.ProductRangeID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList();

                    var roomsuppliment = new List<RoomDetailsInfo>();
                    var roomservice = new List<RoomDetailsInfo>();

                    roomservice = objposition.RoomDetailsInfo.Where(b => b.IsSupplement == false).ToList().
                         OrderBy(b => b.ProductRange.Contains("SINGLE") ? "A" : b.ProductRange.Contains("DOUBLE") ? "B" : b.ProductRange.Contains("TWIN") ? "C" : b.ProductRange.Contains("TRIPLE") ? "D" :
                                            b.ProductRange.Contains("QUAD") ? "E" : b.ProductRange.Contains("TSU") ? "F" :
                                            b.ProductRange.ToLower().Contains("child + bed") ? "G" : b.ProductRange.ToLower().Contains("child - bed") ? "H" :
                                            b.ProductRange.ToLower().Contains("infant") ? "I" : "J").ThenBy(b => b.ProductRange).ToList();

                    roomsuppliment = objposition.RoomDetailsInfo.Where(b => b.IsSupplement == true).OrderBy(b => b.ProductRange).ToList();
                    roomservice.AddRange(roomsuppliment);
                    objposition.RoomDetailsInfo = roomservice;
                    lstposition.Add(objposition);
                }
                objPositionSetReq.mPosition = lstposition;
                objPositionSetRes = positionProviders.SetPosition(objPositionSetReq, token).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objPositionSetRes;

        }
        #endregion

        #region Rail
        public bool RailGet(PositionGetRes positionGetRes, ref RailViewModel model, string token)
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = model.MenuViewModel.QRFID };
                List<AttributeValues> CategoryList = new List<AttributeValues>();
                List<AttributeValues> DurationList = new List<AttributeValues>();

                bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

                if (positionGetRes != null && positionGetRes.ResponseStatus.Status.ToLower() == "success" && positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
                {
                    int duration = positionGetRes.DaysList.Count;
                    CategoryList = positionGetRes.mPosition.Select(a => new AttributeValues { AttributeValue_Id = a.BudgetCategoryId, Value = a.BudgetCategory }).ToList();

                    if (duration > 0)
                    {
                        for (int i = 1; i <= duration; i++)
                        {
                            DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                        }
                    }

                    if (positionGetRes.mPosition != null && positionGetRes.mPosition.Count > 0)
                    {
                        List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                        List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                        List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                        List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                        var ProdIdList = positionGetRes.mPosition.Select(a => a.ProductID).ToList();
                        var ProdcatIdList = positionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                        var ProdRange = new List<ProductRangeDetails>();

                        if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                        {
                            ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                        }

                        foreach (var item in positionGetRes.mPosition)
                        {
                            if (!string.IsNullOrEmpty(item.BudgetCategoryId))
                            {
                                listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                                if (listRange != null && listRange.Count > 0)
                                {
                                    //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                    listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                                    listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                                }
                                else
                                {
                                    listRoomType = new List<ProductAttributeDetails>();
                                    listRange = new List<ProductRangeDetails>();
                                    listSupplements = new List<ProductAttributeDetails>();
                                }
                            }
                            else
                            {
                                listRange = new List<ProductRangeDetails>();
                                listRoomType = new List<ProductAttributeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }

                            listPositionRooms.Add(new PositionRoomsViewModel
                            {
                                PositionId = item.PositionId,
                                RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                                item.RoomDetailsInfo.Select(b => new RoomDetails
                                {
                                    RoomId = b.RoomId,
                                    RoomSequence = b.RoomSequence,
                                    ProductRange = b.ProductRange,
                                    ProductRangeID = b.ProductRangeId,
                                    IsSupplement = b.IsSupplement,
                                    RoomTypeList = b.IsSupplement ?
                                        new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                }).ToList(),
                                SupplementList = listSupplements
                            });
                        }

                        model.ProductAttributeType = positionGetRes.mPosition.Select(a => a.ProductAttributeType).FirstOrDefault();
                        model.RailDetails = positionGetRes.mPosition.Select(a => new RailDetails
                        {
                            RailId = Guid.Parse(a.PositionId),
                            DayNo = a.RoutingDaysID,
                            DayName = positionGetRes.DaysList.Where(p => p.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault() != null ? positionGetRes.DaysList.Where(p => p.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault().Value : "",
                            DaysList = positionGetRes.DaysList,
                            CityID = a.CityID,
                            CityName = a.CityName + ", " + a.CountryName,
                            StartTime = a.StartTime,
                            Duration = a.Duration,
                            RemarksOPS = a.OPSRemarks,
                            RemarksTL = a.TLRemarks,
                            KeepAs = string.IsNullOrEmpty(a.KeepAs) ? "Included" : a.KeepAs,
                            ProductID = a.ProductID,
                            ProductName = a.ProductName,
                            ProductType = a.ProductType,
                            ProductTypeID = a.ProductTypeId,
                            SupplierID = a.SupplierId,
                            SupplierName = a.SupplierName,
                            ProductCategory = a.BudgetCategory,
                            ProductCategoryID = a.BudgetCategoryId,
                            TransferDetails = a.TransferDetails,
                            IsDeleted = false,
                            Status = a.Status,
                            DurationList = DurationList,
                            ProductCategoryList = CategoryList.Where(b => b.AttributeValue_Id == a.BudgetCategoryId).ToList(),
                            PositionRoomsData = listPositionRooms.Where(b => b.PositionId == a.PositionId).FirstOrDefault()
                        }).ToList();
                    }
                    else
                    {
                        List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                        var deffromcity = positionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                        if (model.RailDetails.Count < 1)
                        {
                            model.ProductAttributeType = "Oneway";
                            model.RailDetails.Add(new RailDetails
                            {
                                CityName = deffromcity != null ? deffromcity.CityName : "",
                                CityID = deffromcity != null ? deffromcity.CityId : "",
                                DaysList = positionGetRes.DaysList,
                                DayName = "Day 1",
                                DayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                                StartTime = "10:00",
                                DurationList = DurationList,
                                PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                                IsDeleted = false
                            });
                        }
                    }
                }
                else
                {
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                    var deffromcity = positionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                    if (model.RailDetails.Count < 1)
                    {
                        model.ProductAttributeType = "Oneway";
                        model.RailDetails.Add(new RailDetails
                        {
                            CityName = deffromcity != null ? deffromcity.CityName : "",
                            CityID = deffromcity != null ? deffromcity.CityId : "",
                            DaysList = positionGetRes.DaysList,
                            DayName = "Day 1",
                            DayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                            StartTime = "10:00",
                            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                            IsDeleted = false
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public PositionSetReq RailSet(RailViewModel model, string CurrentUser)
        {
            PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone };

            try
            {
                positionSetReq.mPosition = model.RailDetails.Select(a => new mPosition
                {
                    QRFID = model.QRFID,
                    PositionId = a.RailId.ToString(),
                    StartingFrom = a.DayName,
                    DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : (string.IsNullOrEmpty(a.DayName.Split(' ')[1]) ? 0 : Convert.ToInt32(a.DayName.Split(' ')[1])),
                    RoutingDaysID = a.DayNo,
                    CityID = a.CityID,
                    CityName = a.CityName.Split(",")[0].Trim(),
                    CountryName = a.CityName.Split(",")[1].Trim(),
                    StartTime = a.StartTime,
                    Duration = a.Duration,
                    ProductID = a.ProductID,
                    ProductName = a.ProductName,
                    ProductType = a.ProductType,
                    ProductTypeId = a.ProductTypeID,
                    ProductAttributeType = model.ProductAttributeType,
                    BudgetCategory = a.ProductCategory,
                    BudgetCategoryId = a.ProductCategoryID,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    TransferDetails = a.TransferDetails,
                    KeepAs = a.KeepAs,
                    OPSRemarks = a.RemarksOPS,
                    TLRemarks = a.RemarksTL,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser,
                    EditDate = DateTime.Now,
                    EditUser = CurrentUser,
                    IsDeleted = a.IsDeleted,
                    Status = "Q",
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductRangeId = b.ProductRangeID,
                        ProductCategory = a.ProductCategory,
                        ProductCategoryId = a.ProductCategoryID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return positionSetReq;
        }
        #endregion

        #region Transfers
        public List<ProductType> GetProductType(string strProdType)
        {
            List<ProductType> lst = new List<ProductType>();
            if (strProdType == "transfer")
            {
                lst.Add(new ProductType { ProdType = "Scheduled Transfer" });
                lst.Add(new ProductType { ProdType = "Private Transfer" });
                lst.Add(new ProductType { ProdType = "Ferry Passenger" });
                lst.Add(new ProductType { ProdType = "Ferry Transfer" });
            }
            return lst;
        }

        public TransfersViewModel GetTransfersDetails(IConfiguration _configuration, string token, PositionGetReq objPositionGetReq, string Day)
        {
            TransfersViewModel model = new TransfersViewModel();
            model.MenuViewModel.QRFID = objPositionGetReq.QRFID;
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = objPositionGetReq.QRFID };
            bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

            PositionProviders objPositionProviders = new PositionProviders(_configuration);
            PositionGetRes objPositionGetRes = objPositionProviders.GetPosition(objPositionGetReq, token).Result;
            if (string.IsNullOrEmpty(objPositionGetReq.PositionId))
            {
                List<TransferDetails> lstTransferDetails = new List<TransferDetails>();
                if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.RoutingDays != null && objPositionGetRes.RoutingDays.Count > 0)
                {
                    model.TransferDetails = objPositionGetRes.TransferDetails.OrderBy(m => m.TransferProperties.DayID).ToList();
                }
                else
                {
                    model.TransferDetails = lstTransferDetails;
                }
            }
            else if (!string.IsNullOrEmpty(objPositionGetReq.PositionId))
            {
                model.TransferPopup = new TransferPopup();
                model.TransferPopup.DurationList = new List<AttributeValues>();
                model.TransferPopup.ProdCategoryList = new List<ProdCategoryDetails>();
                model.TransferPopup.TransferProperties = new TransferProperties();
                model.TransferPopup.DayList = new List<AttributeValues>();

                List<AttributeValues> DurationList = new List<AttributeValues>();

                if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success")
                {
                    var duration = objPositionGetRes.DaysList.Count;
                    if (duration > 0)
                    {
                        for (int i = 1; i <= duration; i++)
                        {
                            DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                        }
                    }
                    model.TransferPopup.DayList = objPositionGetRes.DaysList;
                    if (objPositionGetRes.mPosition != null && objPositionGetRes.mPosition.Count > 0)
                    {
                        List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                        List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                        List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                        List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                        mPosition objPosition = objPositionGetRes.mPosition[0];
                        if (!string.IsNullOrEmpty(objPosition.BudgetCategoryId))
                        {
                            listRange = quoteLibrary.GetProductRangeList(objPosition.ProductID, objPosition.BudgetCategoryId, null, token).ProductRangeDetails;
                            listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                            listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                        }
                        else
                        {
                            listRange = new List<ProductRangeDetails>();
                            listRoomType = new List<ProductAttributeDetails>();
                            listSupplements = new List<ProductAttributeDetails>();
                        }

                        listPositionRooms.Add(new PositionRoomsViewModel
                        {
                            PositionId = objPosition.PositionId,
                            RoomDetails = (objPosition.RoomDetailsInfo == null || objPosition.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                                objPosition.RoomDetailsInfo.Select(b => new RoomDetails
                                {
                                    RoomId = b.RoomId,
                                    RoomSequence = b.RoomSequence,
                                    ProductRange = b.ProductRange,
                                    ProductRangeID = b.ProductRangeId,
                                    IsSupplement = b.IsSupplement,
                                    RoomTypeList = b.IsSupplement ?
                                        new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                }).ToList(),
                            SupplementList = listSupplements
                        });

                        model.TransferPopup.TransferProperties.PositionId = objPosition.PositionId;
                        model.TransferPopup.TransferProperties.QRFID = objPosition.QRFID;
                        model.TransferPopup.TransferProperties.DayID = objPosition.DayNo;
                        model.TransferPopup.TransferProperties.PositionSequence = objPosition.PositionSequence;
                        model.TransferPopup.TransferProperties.RoutingDaysID = objPosition.RoutingDaysID;

                        model.TransferPopup.ProductType = objPosition.ProductType;
                        model.TransferPopup.ProductTypeID = objPosition.ProductTypeId;
                        model.TransferPopup.BudgetCategoryId = objPosition.BudgetCategory;
                        model.TransferPopup.BudgetCategory = objPosition.BudgetCategoryId;
                        model.TransferPopup.TransferProperties.DayName = objPosition.StartingFrom;
                        model.TransferPopup.Duration = objPosition.Duration;
                        model.TransferPopup.TransferProductRange = new List<TransferProductRange>();
                        model.TransferPopup.TransferProductRange.Add(new TransferProductRange { PositionRoomsData = listPositionRooms.Where(b => b.PositionId == objPosition.PositionId).FirstOrDefault() });
                        model.TransferPopup.ProductID = objPosition.ProductID;
                        model.TransferPopup.ProductName = objPosition.ProductName;
                        model.TransferPopup.TLRemarks = objPosition.TLRemarks;
                        model.TransferPopup.OPSRemarks = objPosition.OPSRemarks;
                        model.TransferPopup.FromCity = objPosition.CityName + ", " + objPosition.CountryName.Trim();
                        model.TransferPopup.FromCityID = objPosition.CityID;
                        model.TransferPopup.KeepAs = objPosition.KeepAs;
                        model.TransferPopup.FromPickUpLoc = objPosition.FromPickUpLoc;
                        model.TransferPopup.FromPickUpLocID = objPosition.FromPickUpLocID;
                        model.TransferPopup.StartTime = objPosition.StartTime;
                        model.TransferPopup.ProdCategoryList = GetProdCategoryDetails(objPosition.ProductID, token);
                        model.TransferPopup.ToCity = objPosition.ToCityName + ", " + objPosition.ToCountryName.Trim();
                        model.TransferPopup.ToCityID = objPosition.ToCityID;
                        model.TransferPopup.ToDropOffLoc = objPosition.ToDropOffLoc;
                        model.TransferPopup.ToDropOffLocID = objPosition.ToDropOffLocID;
                        model.TransferPopup.EndTime = objPosition.EndTime;
                        model.TransferPopup.DurationList = DurationList;
                        model.TransferPopup.TransferProperties.PositionSequence = objPosition.PositionSequence;
                        model.TransferPopup.SupplierID = objPosition.SupplierId;
                        model.TransferPopup.SupplierName = objPosition.SupplierName;
                    }
                    else
                    {
                        List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                        model.TransferPopup.DurationList = DurationList;
                        model.TransferPopup.TransferProductRange = new List<TransferProductRange>();
                        model.TransferPopup.TransferProductRange.Add(new TransferProductRange { PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails } });
                        model.TransferPopup.Duration = 0;
                        model.TransferPopup.TransferProperties = new TransferProperties();
                        model.TransferPopup.TransferProperties.DayName = Day;
                        if (objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
                        {
                            var dayno = Convert.ToInt32(Day.Replace("Day", "").Trim());
                            var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == "Day " + dayno).FirstOrDefault();
                            var deftocity = objPositionGetRes.DaysList.Where(a => a.Value == "Day " + (dayno + 1)).FirstOrDefault();
                            model.TransferPopup.TransferProperties.RoutingDaysID = deffromcity.AttributeValue_Id;
                            if (deffromcity != null)
                            {
                                model.TransferPopup.FromCityID = deffromcity.CityId;
                                model.TransferPopup.FromCity = deffromcity.CityName;
                            }
                            if (deftocity != null)
                            {
                                model.TransferPopup.ToCityID = deftocity.CityId;
                                model.TransferPopup.ToCity = deftocity.CityName;
                            }
                        }
                    }
                }
                else
                {
                    model.TransferPopup = new TransferPopup();
                    model.TransferPopup.TransferProperties = new TransferProperties();
                    model.TransferPopup.TransferProperties.DayName = Day;

                    if (objPositionGetRes != null && objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
                    {
                        model.TransferPopup.DayList = objPositionGetRes.DaysList;

                        var dayno = Convert.ToInt32(Day.Replace("Day", "").Trim());
                        var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == "Day " + dayno).FirstOrDefault();
                        var deftocity = objPositionGetRes.DaysList.Where(a => a.Value == "Day " + (dayno + 1)).FirstOrDefault();
                        model.TransferPopup.TransferProperties.RoutingDaysID = deffromcity.AttributeValue_Id;
                        if (deffromcity != null)
                        {
                            model.TransferPopup.FromCityID = deffromcity.CityId;
                            model.TransferPopup.FromCity = deffromcity.CityName;
                        }
                        if (deftocity != null)
                        {
                            model.TransferPopup.ToCityID = deftocity.CityId;
                            model.TransferPopup.ToCity = deftocity.CityName;
                        }
                    }
                }
            }
            return model;
        }

        public PositionSetRes SetTransfersDetails(TransfersViewModel model, string CurrentUser, string token)
        {
            PositionSetRes objPositionSetRes = new PositionSetRes();
            PositionSetReq objPositionSetReq = new PositionSetReq { SaveType = "partial", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone };
            try
            {
                mPosition objmPosition = new mPosition
                {
                    PositionSequence = model.TransferPopup.TransferProperties.PositionSequence,
                    QRFID = model.TransferPopup.TransferProperties.QRFID,
                    PositionId = model.TransferPopup.TransferProperties.PositionId,
                    DayNo = model.TransferPopup.TransferProperties.DayID,
                    StartingFrom = "Day " + model.TransferPopup.TransferProperties.DayID,
                    RoutingDaysID = model.TransferPopup.TransferProperties.RoutingDaysID,
                    ProductType = model.TransferPopup.ProductType,
                    ProductTypeId = model.TransferPopup.ProductTypeID,
                    CityID = model.TransferPopup.FromCityID,
                    CityName = model.TransferPopup.FromCity.Split(",")[0].Trim(),
                    ToCityID = model.TransferPopup.ToCityID,
                    ToCityName = model.TransferPopup.ToCity.Split(",")[0].Trim(),
                    CountryName = model.TransferPopup.FromCity.Split(",")[1].Trim(),
                    ToCountryName = model.TransferPopup.FromCity.Split(",")[1].Trim(),
                    Duration = model.TransferPopup.Duration,
                    FromPickUpLoc = model.TransferPopup.FromPickUpLoc,
                    FromPickUpLocID = model.TransferPopup.FromPickUpLocID,
                    StartTime = model.TransferPopup.StartTime,
                    ToDropOffLoc = model.TransferPopup.ToDropOffLoc,
                    ToDropOffLocID = model.TransferPopup.ToDropOffLocID,
                    EndTime = model.TransferPopup.EndTime,
                    ProductID = model.TransferPopup.ProductID,
                    ProductName = model.TransferPopup.ProductName,
                    BudgetCategory = model.TransferPopup.BudgetCategory,
                    BudgetCategoryId = model.TransferPopup.BudgetCategoryId,
                    SupplierId = model.TransferPopup.SupplierID,
                    SupplierName = model.TransferPopup.SupplierName,
                    KeepAs = model.TransferPopup.KeepAs,
                    OPSRemarks = model.TransferPopup.OPSRemarks,
                    TLRemarks = model.TransferPopup.TLRemarks,
                    CreateUser = CurrentUser,
                    EditUser = CurrentUser,
                    IsDeleted = false,
                    Status = "Q",
                    RoomDetailsInfo = model.TransferPopup.RoomDetailsInfo.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductCategoryId = model.TransferPopup.BudgetCategoryId,
                        ProductCategory = model.TransferPopup.BudgetCategory,
                        ProductRangeId = b.ProductRangeId,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                };
                List<mPosition> lstPosition = new List<mPosition>();
                lstPosition.Add(objmPosition);
                objPositionSetReq.mPosition = lstPosition;
                objPositionSetReq.FOC = model.FOC;
                objPositionSetReq.Price = model.Price;
                objPositionSetRes = positionProviders.SetPosition(objPositionSetReq, token).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objPositionSetRes;

        }

        public PositionSetRes SetAllTransfersDetails(IConfiguration _configuration, string token, TransfersViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            PositionProviders objPositionProviders = new PositionProviders(_configuration);
            List<mPosition> lstPosition = new List<mPosition>();
            PositionSetRes objPositionSetRes = new PositionSetRes();
            string username = "";
            objCookies.TryGetValue("UserName", out username);
            username = string.IsNullOrEmpty(username) ? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault() : username;

            foreach (var item in model.TransferDetails)
            {
                if (!string.IsNullOrEmpty(item.PCTID))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.PCTID, IsDeleted = (item.IsPCT == true ? false : true), ProductType = "Private Transfer", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.STID))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.STID, IsDeleted = (item.IsST == true ? false : true), ProductType = "Scheduled Transfer", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.FTID))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.FTID, IsDeleted = (item.IsFT == true ? false : true), ProductType = "Ferry Transfer", EditUser = username });
                }

                if (!string.IsNullOrEmpty(item.FPID))
                {
                    lstPosition.Add(new mPosition { QRFID = model.MenuViewModel.QRFID, Status = "isactive", PositionId = item.FPID, IsDeleted = (item.IsFP == true ? false : true), ProductType = "Ferry Passenger", EditUser = username });
                }
            }

            if (lstPosition != null && lstPosition.Count > 0)
            {
                PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = lstPosition, Price = model.Price, FOC = model.FOC, IsClone = model.MenuViewModel.IsClone,QRFID = model.QRFID,VoyagerUserID = model.VoyagerUserId };
                var positionSetRes = objPositionProviders.SetPosition(positionSetReq, token).Result;
                objPositionSetRes.mPosition = positionSetRes.mPosition;
                objPositionSetRes.PositionId = positionSetRes.PositionId;
                objPositionSetRes.ResponseStatus = positionSetRes.ResponseStatus;
            }
            else
            {
                objPositionSetRes.ResponseStatus = new ResponseStatus();
                objPositionSetRes.ResponseStatus.ErrorMessage = "You have not checked anything";
                objPositionSetRes.ResponseStatus.Status = "Success";
            }
            return objPositionSetRes;
        }
        #endregion

        #region Flights
        public FlightsViewModel GetFlightsDetails(IConfiguration _configuration, string token, PositionGetReq objPositionGetReq)
        {
            FlightsViewModel model = new FlightsViewModel();
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model.MenuViewModel.QRFID = objPositionGetReq.QRFID;
            model.QRFID = objPositionGetReq.QRFID;

            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = objPositionGetReq.QRFID };

            bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

            PositionGetRes objPositionGetRes = positionProviders.GetPosition(objPositionGetReq, token).Result;
            List<AttributeValues> DurationList = new List<AttributeValues>();

            if (objPositionGetRes != null && objPositionGetRes.ResponseStatus.Status.ToLower() == "success" && objPositionGetRes.DaysList != null && objPositionGetRes.DaysList.Count > 0)
            {
                var duration = objPositionGetRes.DaysList.Count;
                if (duration > 0)
                {
                    for (int i = 1; i <= duration; i++)
                    {
                        DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                    }
                }

                if (objPositionGetRes.mPosition != null && objPositionGetRes.mPosition.Count > 0)
                {
                    List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                    List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                    List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                    List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                    var ProdIdList = objPositionGetRes.mPosition.Select(a => a.ProductID).ToList();
                    var ProdcatIdList = objPositionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                    var ProdRange = new List<ProductRangeDetails>();

                    if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                    {
                        ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                    }

                    foreach (var item in objPositionGetRes.mPosition)
                    {
                        if (!string.IsNullOrEmpty(item.BudgetCategory))
                        {
                            listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                            if (listRange != null && listRange.Count > 0)
                            {
                                //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                                listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                            }
                            else
                            {
                                listRoomType = new List<ProductAttributeDetails>();
                                listRange = new List<ProductRangeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }
                        }
                        else
                        {
                            listRange = new List<ProductRangeDetails>();
                            listRoomType = new List<ProductAttributeDetails>();
                            listSupplements = new List<ProductAttributeDetails>();
                        }
                        listPositionRooms.Add(new PositionRoomsViewModel
                        {
                            PositionId = item.PositionId,
                            RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                                item.RoomDetailsInfo.Select(b => new RoomDetails
                                {
                                    RoomId = b.RoomId,
                                    RoomSequence = b.RoomSequence,
                                    ProductRange = b.ProductRange,
                                    ProductRangeID = b.ProductRangeId,
                                    IsSupplement = b.IsSupplement,
                                    RoomTypeList = b.IsSupplement ?
                                        new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                }).ToList(),
                            SupplementList = listSupplements
                        });
                    }
                    int i = 1;
                    model.FlightDetails = objPositionGetRes.mPosition.OrderBy(a => a.DayNo).Select(p => new FlightDetails
                    {
                        PositionId = p.PositionId,
                        ProductType = p.ProductType,
                        ProductTypeID = p.ProductTypeId,
                        BudgetCategoryId = p.BudgetCategory,
                        BudgetCategory = p.BudgetCategoryId,
                        DayID = p.RoutingDaysID,
                        DayName = objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault() != null ? objPositionGetRes.DaysList.Where(a => a.AttributeValue_Id == p.RoutingDaysID).FirstOrDefault().Value : "",
                        DayList = objPositionGetRes.DaysList,
                        Duration = p.Duration,
                        CityID = p.CityID,
                        CityName = p.CityName + ", " + p.CountryName.Trim(),
                        PositionSequence = i++,
                        KeepAs = string.IsNullOrEmpty(p.KeepAs) ? "Included" : p.KeepAs,
                        NoOfPaxAdult = p.NoOfPaxAdult == null ? 0 : Convert.ToInt32(p.NoOfPaxAdult),
                        NoOfPaxChild = p.NoOfPaxChild == null ? 0 : Convert.ToInt32(p.NoOfPaxChild),
                        NoOfPaxInfant = p.NoOfPaxInfant == null ? 0 : Convert.ToInt32(p.NoOfPaxInfant),
                        PositionRoomsData = listPositionRooms.Where(b => b.PositionId == p.PositionId).FirstOrDefault(),
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        TLRemarks = p.TLRemarks,
                        OPSRemarks = p.OPSRemarks,
                        IsDeleted = p.IsDeleted,
                        StartTime = p.StartTime,
                        ProdCategoryList = GetProdCategoryDetails(p.ProductID, token),
                        DurationList = DurationList,
                        SupplierID = p.SupplierId,
                        SupplierName = p.SupplierName
                    }).ToList();
                }
                else
                {
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };
                    var deffromcity = objPositionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                    model.FlightDetails.Add(new FlightDetails
                    {
                        CityName = deffromcity != null ? deffromcity.CityName : "",
                        CityID = deffromcity != null ? deffromcity.CityId : "",
                        DayList = objPositionGetRes.DaysList,
                        DayName = "Day 1",
                        DayID = objPositionGetRes.DaysList.Count == 0 ? "" : objPositionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                        PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                        IsDeleted = false,
                        DurationList = DurationList,
                        Duration = 0,
                        PositionSequence = 1,
                        ProdCategoryList = new List<ProdCategoryDetails>(),
                        NoOfPaxAdult = 0,
                        NoOfPaxChild = 0,
                        NoOfPaxInfant = 0
                    });
                }
            }
            else
            {
                model.FlightDetails.Add(new FlightDetails());
            }
            return model;
        }

        public PositionSetRes SetFlightsDetals(FlightsViewModel model, string CurrentUser, string token)
        {
            PositionSetRes objPositionSetRes = new PositionSetRes();
            PositionSetReq objPositionSetReq = new PositionSetReq { FOC = model.FOC, Price = model.Price, SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone,QRFID = model.QRFID,VoyagerUserID = model.VoyagerUserID };
            try
            {
                objPositionSetReq.mPosition = model.FlightDetails.Select(a => new mPosition
                {
                    PositionSequence = a.PositionSequence,
                    QRFID = model.QRFID,
                    PositionId = a.PositionId,
                    DayNo = Convert.ToInt32(a.DayName.Replace("Day ", "")),
                    StartingFrom = a.DayName,
                    RoutingDaysID = a.DayID,
                    ProductType = a.ProductType,
                    ProductTypeId = a.ProductTypeID,
                    CityID = a.CityID,
                    CityName = a.CityName.Split(",")[0].Trim(),
                    CountryName = a.CityName.Split(",")[1].Trim(),
                    Duration = a.Duration,
                    StartTime = a.StartTime,
                    ProductID = a.ProductID,
                    ProductName = a.ProductName,
                    BudgetCategory = a.BudgetCategoryId,
                    BudgetCategoryId = a.BudgetCategory,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    KeepAs = a.KeepAs,
                    OPSRemarks = a.OPSRemarks,
                    TLRemarks = a.TLRemarks,
                    CreateUser = CurrentUser,
                    EditUser = CurrentUser,
                    IsDeleted = a.IsDeleted,
                    Status = "Q",
                    NoOfPaxAdult = a.NoOfPaxAdult,
                    NoOfPaxChild = a.NoOfPaxChild,
                    NoOfPaxInfant = a.NoOfPaxInfant,
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductCategoryId = a.BudgetCategory,
                        ProductCategory = a.BudgetCategoryId,
                        ProductRangeId = b.ProductRangeID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();

                objPositionSetRes = positionProviders.SetPosition(objPositionSetReq, token).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objPositionSetRes;

        }
        #endregion

        #region OthersLocalGuide
        public bool OthersLocalGuideGet(PositionGetRes positionGetRes, ref OthersViewModel model, string token, string GetType = "GUIDE")
        {
            try
            {
                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = model.MenuViewModel.QRFID };
                List<AttributeValues> CategoryList = new List<AttributeValues>();

                List<AttributeValues> DurationList = new List<AttributeValues>();
                List<TourGuideDefaultRows> DefaultList = new List<TourGuideDefaultRows>();

                bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

                if (positionGetRes != null && positionGetRes.ResponseStatus.Status.ToLower() == "success" && positionGetRes.DaysList != null && positionGetRes.DaysList.Count > 0)
                {
                    int duration = positionGetRes.DaysList.Count;
                    CategoryList = positionGetRes.mPosition.Select(a => new AttributeValues { AttributeValue_Id = a.BudgetCategoryId, Value = a.BudgetCategory }).ToList();

                    int day = 0;
					int prevnight = 0;

					foreach (var Route in positionGetRes.RoutingInfo)
                    {
                        if (day == 0) day = 1;
						else { day = prevnight + day; }
						//else day += 1;

						if (Route.IsLocalGuide && string.IsNullOrEmpty(model.MenuViewModel.PositionId))
                        {
                            DefaultList.Add(new TourGuideDefaultRows
                            {
                                DayNo = positionGetRes.DaysList.Where(a => a.Value == "Day " + day.ToString()).FirstOrDefault().AttributeValue_Id,
                                DayName = "Day " + day.ToString(),
                                Duration = Route.Days,
                                CityID = Route.FromCityID,
                                CityName = Route.FromCityName,
                                StartTime = "10:00"
                            });
                        }
						//day += Route.Days - 1;
						prevnight = Route.Nights;
					}

                    if (duration > 0)
                    {
                        for (int i = 1; i <= duration; i++)
                        {
                            DurationList.Add(new AttributeValues { AttributeValue_Id = i.ToString(), Value = i.ToString() });
                        }
                    }

                    if (positionGetRes.mPosition != null && positionGetRes.mPosition.Count > 0)
                    {
                        List<PositionRoomsViewModel> listPositionRooms = new List<PositionRoomsViewModel>();
                        List<ProductAttributeDetails> listRoomType = new List<ProductAttributeDetails>();
                        List<ProductAttributeDetails> listSupplements = new List<ProductAttributeDetails>();
                        List<ProductRangeDetails> listRange = new List<ProductRangeDetails>();

                        var ProdIdList = positionGetRes.mPosition.Select(a => a.ProductID).ToList();
                        var ProdcatIdList = positionGetRes.mPosition.Select(a => a.BudgetCategoryId).ToList();
                        var ProdRange = new List<ProductRangeDetails>();

                        if (ProdIdList != null && ProdIdList.Count > 0 && ProdcatIdList != null && ProdcatIdList.Count > 0)
                        {
                            ProdRange = quoteLibrary.GetProductRangeList("", "", null, token, "", ProdIdList, ProdcatIdList).ProductRangeDetails;
                        }

                        foreach (var item in positionGetRes.mPosition)
                        {
                            if (!string.IsNullOrEmpty(item.BudgetCategoryId))
                            {
                                listRange = ProdRange.Where(a => a.ProductCategoryId == item.BudgetCategoryId && a.ProductId == item.ProductID).ToList();
                                if (listRange != null && listRange.Count > 0)
                                {
                                    //listRange = quoteLibrary.GetProductRangeList(item.ProductID, item.BudgetCategoryId, null, token).ProductRangeDetails;
                                    listRoomType = listRange.Where(a => a.AdditionalYN == false || a.AdditionalYN == null).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode + " (" + c.PersonType + (c.AgeRange == null ? "" : " | " + c.AgeRange) + ") " }).OrderBy(a => a.Value).ToList();
                                    listSupplements = listRange.Where(a => a.AdditionalYN == true).Select(c => new ProductAttributeDetails { AttributeId = c.VoyagerProductRange_Id, Value = c.ProductRangeCode }).OrderBy(a => a.Value).ToList();
                                }
                                else
                                {
                                    listRoomType = new List<ProductAttributeDetails>();
                                    listRange = new List<ProductRangeDetails>();
                                    listSupplements = new List<ProductAttributeDetails>();
                                }
                            }
                            else
                            {
                                listRange = new List<ProductRangeDetails>();
                                listRoomType = new List<ProductAttributeDetails>();
                                listSupplements = new List<ProductAttributeDetails>();
                            }

                            listPositionRooms.Add(new PositionRoomsViewModel
                            {
                                PositionId = item.PositionId,
                                RoomDetails = (item.RoomDetailsInfo == null || item.RoomDetailsInfo.Count == 0) ? new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false, RoomTypeList = listRoomType } } :
                                    item.RoomDetailsInfo.Select(b => new RoomDetails
                                    {
                                        RoomId = b.RoomId,
                                        RoomSequence = b.RoomSequence,
                                        ProductRange = b.ProductRange,
                                        ProductRangeID = b.ProductRangeId,
                                        IsSupplement = b.IsSupplement,
                                        RoomTypeList = b.IsSupplement ?
                                            new List<ProductAttributeDetails> { new ProductAttributeDetails { AttributeId = b.ProductRangeId, Value = b.ProductRange } } : listRoomType
                                    }).ToList(),
                                SupplementList = listSupplements
                            });
                        }


                        model.OthersLocalGuide.LocalGuideDetails = positionGetRes.mPosition.Select(a => new LocalGuideDetails
                        {
                            OthersId = Guid.Parse(a.PositionId),
                            DayNo = a.RoutingDaysID,
                            DayName = positionGetRes.DaysList.Where(p => p.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault() != null ? positionGetRes.DaysList.Where(p => p.AttributeValue_Id == a.RoutingDaysID).FirstOrDefault().Value : "",
                            DayList = positionGetRes.DaysList,
                            CityID = a.CityID,
                            CityName = a.CityName + ", " + a.CountryName,
                            StartTime = a.StartTime,
                            Duration = a.Duration,
                            RemarksOPS = a.OPSRemarks,
                            RemarksTL = a.TLRemarks,
                            KeepAs = string.IsNullOrEmpty(a.KeepAs) ? "Included" : a.KeepAs,
                            ProductID = a.ProductID,
                            ProductName = a.ProductName,
                            ProductType = a.ProductType,
                            ProductTypeID = a.ProductTypeId,
                            SupplierID = a.SupplierId,
                            SupplierName = a.SupplierName,
                            ProductCategory = a.BudgetCategory,
                            ProductCategoryID = a.BudgetCategoryId,
                            IsDeleted = false,
                            Status = a.Status,
                            DurationList = DurationList,
                            ProductCategoryList = CategoryList.Where(b => b.AttributeValue_Id == a.BudgetCategoryId).ToList(),
                            PositionRoomsData = listPositionRooms.Where(b => b.PositionId == a.PositionId).FirstOrDefault()
                        }).ToList();


                        if (DefaultList.Count > 0)
                        {
                            foreach (var defItem in DefaultList)
                            {
                                if (model.OthersLocalGuide.LocalGuideDetails.Where(a => a.DayNo == defItem.DayNo).Count() < 1)
                                {
                                    model.OthersLocalGuide.LocalGuideDetails.Add(new LocalGuideDetails
                                    {
                                        CityID = defItem.CityID,
                                        CityName = defItem.CityName,
                                        DayName = defItem.DayName,
                                        StartTime = defItem.StartTime,
                                        DayNo = defItem.DayNo,
                                        Duration = defItem.Duration,
                                        IsDeleted = false,
                                        DurationList = DurationList,
                                        DayList = positionGetRes.DaysList,
                                        PositionRoomsData = new PositionRoomsViewModel { RoomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } } },
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };

                        if (GetType == "GUIDE")
                        {
                            model.OthersLocalGuide.LocalGuideDetails.AddRange(DefaultList.Select(a => new LocalGuideDetails
                            {
                                StartTime = a.StartTime,
                                CityID = a.CityID,
                                CityName = a.CityName,
                                DayName = a.DayName,
                                DayNo = a.DayNo, //positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                                Duration = a.Duration,
                                DurationList = DurationList,
                                DayList = positionGetRes.DaysList,
                                PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                                IsDeleted = false
                            }));
                        }

                        if (model.OthersLocalGuide.LocalGuideDetails.Count < 1)
                        {
                            var deffromcity = positionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                            model.OthersLocalGuide.LocalGuideDetails.Add(new LocalGuideDetails
                            {
                                CityName = deffromcity != null ? deffromcity.CityName : "",
                                CityID = deffromcity != null ? deffromcity.CityId : "",
                                StartTime = "10:00",
                                DurationList = DurationList,
                                DayList = positionGetRes.DaysList,
                                DayName = "Day 1",
                                DayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                                PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                                IsDeleted = false
                            });
                        }
                    }
                }
                else
                {
                    List<RoomDetails> roomDetails = new List<RoomDetails> { new RoomDetails { RoomSequence = 1, IsSupplement = false } };

                    if (GetType == "GUIDE")
                    {
                        model.OthersLocalGuide.LocalGuideDetails.AddRange(DefaultList.Select(a => new LocalGuideDetails
                        {
                            StartTime = a.StartTime,
                            CityID = a.CityID,
                            CityName = a.CityName,
                            DayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                            DayName = "Day 1",
                            Duration = a.Duration,
                            DurationList = DurationList,
                            DayList = positionGetRes.DaysList,
                            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                            IsDeleted = false
                        }));
                    }

                    if (model.OthersLocalGuide.LocalGuideDetails.Count < 1)
                    {
                        var deffromcity = positionGetRes.DaysList.Where(a => a.Value == "Day 1").FirstOrDefault();

                        model.OthersLocalGuide.LocalGuideDetails.Add(new LocalGuideDetails
                        {
                            CityName = deffromcity != null ? deffromcity.CityName : "",
                            CityID = deffromcity != null ? deffromcity.CityId : "",
                            StartTime = "10:00",
                            DayList = positionGetRes.DaysList,
                            DayName = "Day 1",
                            DayNo = positionGetRes.DaysList.Count == 0 ? "" : positionGetRes.DaysList.FirstOrDefault().AttributeValue_Id,
                            PositionRoomsData = new PositionRoomsViewModel { RoomDetails = roomDetails },
                            IsDeleted = false
                        });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public PositionSetReq OthersLocalGuideSet(OthersViewModel model, string CurrentUser)
        {
            PositionSetReq positionSetReq = new PositionSetReq { SaveType = "complete", mPosition = new List<mPosition>(), IsClone = model.MenuViewModel.IsClone };

            try
            {
                positionSetReq.mPosition = model.OthersLocalGuide.LocalGuideDetails.Select(a => new mPosition
                {
                    QRFID = model.OthersLocalGuide.QRFID,
                    PositionId = a.OthersId.ToString(),
                    RoutingDaysID = a.DayNo,
                    StartingFrom = a.DayName,
                    DayNo = string.IsNullOrEmpty(a.DayName) ? 0 : (string.IsNullOrEmpty(a.DayName.Split(' ')[1]) ? 0 : Convert.ToInt32(a.DayName.Split(' ')[1])),
                    CityID = a.CityID,
                    CityName = a.CityName.Split(",")[0].Trim(),
                    CountryName = a.CityName.Split(",")[1].Trim(),
                    StartTime = a.StartTime,
                    Duration = a.Duration,
                    ProductID = a.ProductID,
                    ProductName = a.ProductName,
                    ProductType = a.ProductType,
                    ProductTypeId = a.ProductTypeID,
                    BudgetCategory = a.ProductCategory,
                    BudgetCategoryId = a.ProductCategoryID,
                    SupplierId = a.SupplierID,
                    SupplierName = a.SupplierName,
                    KeepAs = a.KeepAs,
                    OPSRemarks = a.RemarksOPS,
                    TLRemarks = a.RemarksTL,
                    CreateDate = DateTime.Now,
                    CreateUser = CurrentUser,
                    EditDate = DateTime.Now,
                    EditUser = CurrentUser,
                    IsDeleted = a.IsDeleted,
                    Status = "Q",
                    RoomDetailsInfo = a.PositionRoomsData.RoomDetails.Select(b => new RoomDetailsInfo
                    {
                        RoomId = b.RoomId,
                        RoomSequence = b.RoomSequence,
                        ProductRange = b.ProductRange,
                        ProductRangeId = b.ProductRangeID,
                        ProductCategory = a.ProductCategory,
                        ProductCategoryId = a.ProductCategoryID,
                        IsSupplement = b.IsSupplement,
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        EditUser = CurrentUser,
                        EditDate = DateTime.Now,
                        IsDeleted = b.RoomSequence == 0 ? true : false
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return positionSetReq;
        }
        #endregion

        #region Tour Entities
        public TourEntitiesViewModel GetDynamicTourEntities(IConfiguration _configuration, string token, TourEntitiesGetReq tourEntitiesGetReq)
        {
            TourEntitiesViewModel model = new TourEntitiesViewModel();
            SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
            model.MenuViewModel.QRFID = tourEntitiesGetReq.QRFID;
            model.QRFID = tourEntitiesGetReq.QRFID;

            NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = tourEntitiesGetReq.QRFID };

            bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
            if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;

            TourEntitiesGetRes response = positionProviders.GetDynamicTourEntities(tourEntitiesGetReq, token).Result;

            if (response != null && response.ResponseStatus.Status.ToLower() == "success" && response.DynamicTourEntity.Count > 0)
            {
                model.DynamicTourEntity = response.DynamicTourEntity;
            }
            else
            {
                model.DynamicTourEntity = new List<DynamicTourEntity>();
            }
            return model;
        }

        public TourEntitiesViewModel GetTourEntities(IConfiguration _configuration, string token, TourEntitiesGetReq tourEntitiesGetReq)
        {
            TourEntitiesViewModel model = new TourEntitiesViewModel();
            model.MenuViewModel.QRFID = tourEntitiesGetReq.QRFID;
            model.QRFID = tourEntitiesGetReq.QRFID;

            TourEntitiesGetRes response = positionProviders.GetTourEntities(tourEntitiesGetReq, token).Result;
            if (response != null && response.ResponseStatus.Status.ToLower() == "success" && response.PaxSlabDetails != null && response.PaxSlabDetails.PaxSlabs.Count > 0)
            {
                model.PaxSlabDetails.PaxSlabs = response.PaxSlabDetails.PaxSlabs;

                if (!string.IsNullOrEmpty(tourEntitiesGetReq.PositionID))
                {
                    var lst = tourEntitiesGetReq.PositionID.Split(",");
                    List<string> list = new List<string>(lst);
                    var positionexists = response.TourEntities.Where(a => list.Contains(a.PositionID)).Select(a => a.PositionID).Distinct().ToList();
                    var positionnotexists = positionexists != null && positionexists.Count > 0 ? list.Where(a => !positionexists.Contains(a)).ToList() : list;
                    model.PositionNotExists = positionnotexists != null && positionnotexists.Count() > 0 ? positionnotexists : new List<string>();
                }
                else
                {
                    model.PositionNotExists = new List<string>();
                }
                var objTourEntity = new TourEntities();
                var lstTourEntity = new List<TourEntities>();

                var paxnotexists = response.PaxSlabDetails.PaxSlabs.Select(b => (b.From.ToString() + " - " + b.To.ToString())).ToList()
                        .FindAll(c => !response.TourEntities.Select(b => b.PaxSlab).Contains(c));//30-40

                var postourent = response.TourEntities.Select(a => new { a.Type, a.PositionID, a.TourEntityID, a.PaxSlab }).Distinct().ToList();//10-20

                var posnotinpaxslab = postourent.FindAll(a => !paxnotexists.Contains(a.PaxSlab));
                if (paxnotexists?.Count > 0)
                {
                    foreach (var item in posnotinpaxslab.Select(a => new { a.Type, a.PositionID }).Distinct().ToList())
                    {
                        foreach (var pax in paxnotexists)
                        {
                            objTourEntity = new TourEntities();
                            objTourEntity.PaxSlab = pax;
                            objTourEntity.PositionID = item.PositionID;
                            objTourEntity.TourEntityID = "0";
                            objTourEntity.Type = item.Type;
                            lstTourEntity.Add(objTourEntity);
                        }
                    }
                }

                if (lstTourEntity != null && lstTourEntity.Count > 0)
                {
                    response.TourEntities.AddRange(lstTourEntity);
                }
                model.TourEntities = response.TourEntities;
            }
            else
            {
                model.PositionNotExists = !string.IsNullOrEmpty(tourEntitiesGetReq.PositionID) ? new List<string>(tourEntitiesGetReq.PositionID.Split(",")) : new List<string>();
                model.PaxSlabDetails = new PaxSlabDetails();
                model.TourEntities = new List<TourEntities>();
            }

            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = "Quote Room Type";
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;
            model.AutoCompleteTextBox = new List<AutoCompleteTextBox>();
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                if (objMasterTypeResponse.PropertyList[0].Attribute[0].AttributeName == "Quote Room Type")
                {
                    List<AutoCompleteTextBox> roomTypeList = new List<AutoCompleteTextBox>();
                    var result = objMasterTypeResponse.PropertyList[0].Attribute[0].Values.ToList();
                    roomTypeList = result.Select(data => new AutoCompleteTextBox { value = data.AttributeValue_Id, label = data.Value }).ToList();
                    model.AutoCompleteTextBox = roomTypeList;
                }
            }

            model.ResponseStatus = response.ResponseStatus;
            return model;
        }

        public TourEntitiesSetRes SetTourEntities(IConfiguration _configuration, string token, TourEntitiesSetReq tourEntitiesSetReq)
        {
            TourEntitiesSetRes response = positionProviders.SetTourEntities(tourEntitiesSetReq, token).Result;
            if (response == null || response.ResponseStatus.Status.ToLower() != "success")
            {
                response = new TourEntitiesSetRes();
                response.ResponseStatus = new ResponseStatus();
                response.TourEntities = new List<TourEntities>();
            }
            return response;
        }
        #endregion
    }
}
