using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using VGER_WAPI_CLASSES;
using Voyager.App.Library;
using Voyager.App.Providers;
using Voyager.App.ViewModels;

namespace Voyager.App.Mappers
{
    public class HandoverMapping
    {
        #region Declaration 
        private readonly IConfiguration _configuration;
        private HandoverProviders handoverProviders;
        public COCommonLibrary cOCommonLibrary;
        #endregion

        public HandoverMapping(IConfiguration configuration)
        {
            _configuration = configuration;
            cOCommonLibrary = new COCommonLibrary(configuration);
            handoverProviders = new HandoverProviders(_configuration);
        }

        #region AttachToMaster
        public AttachToMasterViewModel GetGoAhead(string token, GoAheadGetReq objGoAheadGetReq)
        {
            AttachToMasterViewModel model = new AttachToMasterViewModel();
            GoAheadGetRes objGoAheadGetRes = new GoAheadGetRes();
            try
            {
                model = GetSalesOfficerHeraderInfo(objGoAheadGetReq.QRFID, "AttachToMaster", token);
                objGoAheadGetRes = handoverProviders.GetGoAhead(objGoAheadGetReq, token).Result;
                if (objGoAheadGetRes != null)
                {
                    model.GoAheadGetRes = objGoAheadGetRes;
                    model.UserList = objGoAheadGetRes.UserSystemContactDetails.Select(a => new AttributeValues { AttributeValue_Id = a.VoygerContactId, Value = a.FirstName + " " + a.LastName, Flag = a.IsOperationDefault }).OrderBy(a => a.Value).ToList();
                    model.UserID = string.IsNullOrEmpty(objGoAheadGetRes.mGoAhead.OperationUserID) ? model.UserList.Where(a => a.Flag == true).FirstOrDefault().AttributeValue_Id : objGoAheadGetRes.mGoAhead.OperationUserID;
                    model.UserName = string.IsNullOrEmpty(objGoAheadGetRes.mGoAhead.OperationUserName) ? model.UserList.Where(a => a.Flag == true).FirstOrDefault().Value : objGoAheadGetRes.mGoAhead.OperationUserName;
                    CostsheetVersion objCostsheetVersion = objGoAheadGetRes.CostsheetVersion.Where(a => a.IsCurrentVersion == true).FirstOrDefault();
                    if (objCostsheetVersion != null)
                    {
                        model.VersionId = objCostsheetVersion.VersionId;
                        model.QRFPriceId = objCostsheetVersion.QRFPriceId;
                    }
                    if (objGoAheadGetRes.mGoAhead.ConfirmationDeadline != null)
                    {
                        var dt = objGoAheadGetRes.mGoAhead.ConfirmationDeadline.Value;
                        var day = dt.Day.ToString().Length == 1 ? "0" + dt.Day.ToString() : dt.Day.ToString();
                        var month = dt.Month.ToString().Length == 1 ? "0" + dt.Month.ToString() : dt.Month.ToString();
                        model.ConfirmationDT = day + "/" + month + "/" + dt.Year;
                    }
                    else
                    {
                        model.ConfirmationDT = null;
                    }
                }
                else
                {
                    objGoAheadGetRes = new GoAheadGetRes();
                    model.GoAheadGetRes = objGoAheadGetRes;
                    model.UserList = new List<AttributeValues>();
                    model.UserID = "";
                    model.UserName = "";
                    model.VersionId = 0;
                    model.QRFPriceId = "";
                    model.ConfirmationDT = null;
                }
            }
            catch (Exception ex)
            {
                objGoAheadGetRes = new GoAheadGetRes();
                model.GoAheadGetRes = objGoAheadGetRes;
                model.UserList = new List<AttributeValues>();
                model.UserID = "";
                model.UserName = "";
                model.VersionId = 0;
                model.QRFPriceId = "";
                model.ConfirmationDT = null;
            }
            return model;
        }

        public Materialisation GetGoAheadDepature(string token, GoAheadGetReq objGoAheadGetReq)
        {
            Materialisation model = new Materialisation();
            GetGoAheadDepatureRes objGetGoAheadDepatureRes = new GetGoAheadDepatureRes();
            try
            {
                objGetGoAheadDepatureRes = handoverProviders.GetGoAheadDepature(objGoAheadGetReq, token).Result;
                if (objGetGoAheadDepatureRes != null && objGetGoAheadDepatureRes.mGoAhead != null && objGetGoAheadDepatureRes.mGoAhead.Depatures != null
                    && objGetGoAheadDepatureRes.mGoAhead.Depatures.Count > 0)
                {
                    model.RoomInfo = objGetGoAheadDepatureRes.mGoAhead.Depatures[0].PassengerRoomInfo.Select(a =>
                    new ViewModels.RoomInfo
                    {
                        RoomCount = Convert.ToInt32(a.RoomCount),
                        RoomTypeID = a.RoomTypeID,
                        RoomTypeName = a.RoomTypeName,
                        PaxCount = Convert.ToInt32(a.PaxCount)
                    }).ToList();
                    int i = 0;
                    model.ChildInfo = objGetGoAheadDepatureRes.mGoAhead.Depatures[0].ChildInfo.Select(a =>
                    new ViewModels.ChildInfo
                    {
                        Age = a.Age,
                        ChildInfoId = a.ChildInfoId,
                        Number = a.Number,
                        Type = a.Type,
                        IsDeleted = a.IsDeleted,
                        SeqNo = i++
                    }).ToList();

                    if (model.ChildInfo == null || model.ChildInfo.Count == 0)
                    {
                        model.ChildInfo.Add(new ViewModels.ChildInfo { Age = 0, ChildInfoId = "", IsDeleted = false, Number = 0, SeqNo = 0, Type = "" });
                        model.ChildAgeList = new List<AttributeValues>();
                    }
                    else
                    {
                        model.ChildAgeList = model.ChildInfo.Select(a => a.Age).Distinct().Select(a => new AttributeValues { AttributeValue_Id = a.ToString(), Value = a.ToString() }).ToList();
                    }
                    model.ChildTypeList = objGetGoAheadDepatureRes.ChildTypeList;
                }
                else
                {
                    model.RoomInfo = new List<ViewModels.RoomInfo>();
                    model.ChildInfo = new List<ViewModels.ChildInfo>();
                }
            }
            catch (Exception ex)
            {

            }
            return model;
        }

        public GoAheadSetRes SetGoAhead(AttachToMasterViewModel model, string userEmail, string token, string voyagerUserId)
        {
            GoAheadSetRes objGoAheadSetRes = new GoAheadSetRes();
            GoAheadSetReq objGoAheadSetReq = new GoAheadSetReq { mGoAhead = model.GoAheadGetRes.mGoAhead, DepatureId = model.DepartureId };

            try
            {
                objGoAheadSetReq.mGoAhead.OperationUserID = model.UserID;
                objGoAheadSetReq.mGoAhead.OperationUserName = model.UserName;
                objGoAheadSetReq.mGoAhead.QRFID = model.MenuViewModel.QRFID;

                if (!string.IsNullOrEmpty(model.ConfirmationDT))
                {
                    var dt = model.ConfirmationDT.Split("/");
                    if (dt?.Count() >= 3)
                    {
                        DateTime fromDT = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]));
                        objGoAheadSetReq.mGoAhead.ConfirmationDeadline = fromDT;
                    }
                    else
                    {
                        objGoAheadSetReq.mGoAhead.ConfirmationDeadline = null;
                    }
                }
                objGoAheadSetReq.mGoAhead.QRFPriceId = model.QRFPriceId;
                objGoAheadSetReq.mGoAhead.VersionId = model.VersionId;
                objGoAheadSetReq.UserEmail = userEmail;
                objGoAheadSetReq.VoyagerUserId = voyagerUserId;

                objGoAheadSetRes = handoverProviders.SetGoAhead(objGoAheadSetReq, token).Result;
                objGoAheadSetRes = objGoAheadSetRes != null ? objGoAheadSetRes : new GoAheadSetRes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objGoAheadSetRes;
        }

        public SetMaterialisationRes SetMaterialisation(Materialisation model, string token)
        {
            SetMaterialisationRes objSetMaterialisationRes = new SetMaterialisationRes();
            SetMaterialisationReq objSetMaterialisationReq = new SetMaterialisationReq();

            try
            {
                objSetMaterialisationReq.mGoAhead = new mGoAhead();
                objSetMaterialisationReq.mGoAhead.Depatures = new List<Depatures>();
                List<VGER_WAPI_CLASSES.ChildInfo> ChildInfo = new List<VGER_WAPI_CLASSES.ChildInfo>();
                List<VGER_WAPI_CLASSES.PassengerRoomInfo> PassengerRoomInfo = new List<VGER_WAPI_CLASSES.PassengerRoomInfo>();
                if (model.ChildInfo != null && model.ChildInfo.Count > 0)
                {
                    ChildInfo = model.ChildInfo.Select(a => new VGER_WAPI_CLASSES.ChildInfo
                    {
                        Age = a.Age,
                        ChildInfoId = a.ChildInfoId,
                        IsDeleted = a.IsDeleted,
                        Number = a.Number,
                        Type = a.Type
                    }).ToList();
                }
                if (model.RoomInfo != null && model.RoomInfo.Count > 0)
                {
                    PassengerRoomInfo = model.RoomInfo.Select(a => new VGER_WAPI_CLASSES.PassengerRoomInfo
                    {
                        RoomCount = a.RoomCount,
                        RoomTypeID = a.RoomTypeID,
                        RoomTypeName = a.RoomTypeName,
                        PaxCount = a.PaxCount
                    }).ToList();
                }
                objSetMaterialisationReq.mGoAhead.Depatures.Add(new Depatures { DepatureId = model.DepartureId, PassengerRoomInfo = PassengerRoomInfo, ChildInfo = ChildInfo });
                objSetMaterialisationReq.mGoAhead.QRFID = model.QRFID;
                objSetMaterialisationReq.mGoAhead.GoAheadId = model.GoAheadId;

                objSetMaterialisationRes = handoverProviders.SetMaterialisation(objSetMaterialisationReq, token).Result;
                objSetMaterialisationRes = objSetMaterialisationRes != null ? objSetMaterialisationRes : new SetMaterialisationRes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objSetMaterialisationRes;
        }

        public GoAheadSetRes SendMailHandoverBooking(GoAheadGetReq request, string token)
        {
            GoAheadSetRes objGoAheadSetRes = new GoAheadSetRes();

            try
            {
                objGoAheadSetRes = handoverProviders.SendMailHandoverBooking(request, token).Result;
                objGoAheadSetRes = objGoAheadSetRes != null ? objGoAheadSetRes : new GoAheadSetRes();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return objGoAheadSetRes;
        }
        #endregion

        #region Handover
        public ConfirmBookingSetRes SetGoAheadConfirmMessage(string token, string QRFID, string username, List<long> DepatureId)
        {
            ConfirmBookingSetReq confirmBookingSetReq = new ConfirmBookingSetReq() { QRFID = QRFID, UserName = username, DepatureId = DepatureId };
            ConfirmBookingSetRes res = handoverProviders.SetGoAheadConfirmMessage(confirmBookingSetReq, token).Result;
            return res;
        }

        public HandoverViewModel GetHandover(string token, string QRFID)
        {
            HandoverViewModel model = new HandoverViewModel();
            model = GetSalesOfficerHeraderInfo(QRFID, token);
            return model;
        }

        public HandoverViewModel SetConfirmBooking(string token, string QRFID, string username, List<long> DepatureId = null)
        {
            HandoverViewModel model = new HandoverViewModel();
            model.MenuViewModel.QRFID = QRFID;
            ConfirmBookingGetReq confirmBookingGetReq = new ConfirmBookingGetReq() { QRFID = QRFID, UserName = username, DepatureId = DepatureId };
            ConfirmBookingGetRes res = handoverProviders.GoAheadQuotes(confirmBookingGetReq, token).Result;
            model.ResponseStatus = res != null ? res.ResponseStatus != null ? res.ResponseStatus : new ResponseStatus() : new ResponseStatus(); return model;
        }

        public List<DepatureInfo> GetGoAheadDeparturesDetails(string token, string QRFID)
        {
            List<DepatureInfo> lstDepatureInfo = new List<DepatureInfo>();
            GoAheadGetReq goAheadGetReq = new GoAheadGetReq() { QRFID = QRFID };
            HandoverGetRes res = handoverProviders.GetGoAheadDeparturesDetails(goAheadGetReq, token).Result;
            if (res.Depatures != null && res.Depatures.Count > 0)
            {
                lstDepatureInfo = res.Depatures.Select(a => new DepatureInfo
                {
                    Confirmed_Date = a.Confirmed_Date,
                    Confirmed_User = a.Confirmed_User,
                    ConfirmMessage = a.ConfirmMessage,
                    ConfirmStatus = a.ConfirmStatus,
                    DepatureDate = a.DepatureDate,
                    DepatureId = a.DepatureId,
                    IsCreate = a.IsCreate,
                    IsDeleted = a.IsDeleted,
                    IsMaterialised = a.IsMaterialised,
                    IsShowProcessing = false,
                    OpsBookingNumber = a.OpsBookingNumber
                }).ToList();
            }
            return lstDepatureInfo;
        }
        #endregion

        #region Common Function
        public AttachToMasterViewModel GetSalesOfficerHeraderInfo(string QRFID, string MenuName, string token)
        {
            AttachToMasterViewModel model = new AttachToMasterViewModel();
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

        public HandoverViewModel GetSalesOfficerHeraderInfo(string QRFID, string token)
        {
            HandoverViewModel model = new HandoverViewModel();
            model.MenuViewModel.QRFID = QRFID;
            model.MenuViewModel.MenuName = "Handover";

            #region Get Costing Officer Tour Info Header By QRFId
            NewQuoteViewModel modelQuote = new NewQuoteViewModel();
            modelQuote.QRFID = QRFID;
            model.COHeaderViewModel = cOCommonLibrary.GetCOTourInfoHeader(ref modelQuote, token);
            model.MenuViewModel.EnquiryPipeline = modelQuote.mdlMenuViewModel.EnquiryPipeline;
            #endregion
            return model;
        }
        #endregion

        #region Add New Departures
        public GoAheadNewDeptGetRes GetGoAheadExistDepartures(GoAheadGetReq objGoAheadGetReq, string token)
        {
            GoAheadNewDeptGetRes model = new GoAheadNewDeptGetRes();
            model = handoverProviders.GetGoAheadExistDepartures(objGoAheadGetReq, token).Result;
            model = model != null ? model : new GoAheadNewDeptGetRes();
            return model;
        }

        public GoAheadNewDeptSetRes SetGoAheadNewDepartures(GoAheadNewDeptSetReq objGoAheadNewDeptSetReq, string token)
        {
            GoAheadNewDeptSetRes model = new GoAheadNewDeptSetRes();
            model = handoverProviders.SetGoAheadNewDepartures(objGoAheadNewDeptSetReq, token).Result;
            return model;
        }
        #endregion
    }
}
