using System;
using System.Collections.Generic;
using System.Text;
using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Contracts;
using Microsoft.Extensions.Configuration;
using System.Linq;
using VGER_WAPI_CLASSES;
using Microsoft.AspNetCore.Http;

namespace Voyager.App.Library
{
    public class SalesQuoteLibrary
    {
        MasterProviders objMasterProviders;
        private readonly IConfiguration _configuration;
        public SalesQuoteLibrary(IConfiguration configuration)
        {
            _configuration = configuration;
            objMasterProviders = new MasterProviders(_configuration);
        }

        public QuoteSearchViewModel BindMasterData(IConfiguration _configuration, string token, string AttributeName = "")
        {
            MasterTypeRequest objMasterTypeRequest = new MasterTypeRequest();
            objMasterTypeRequest.Property = "QRF Masters";
            objMasterTypeRequest.Name = string.IsNullOrEmpty(AttributeName) ? "" : AttributeName;

            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MasterTypeResponse objMasterTypeResponse = objSalesProvider.GetGenericMasterForType(objMasterTypeRequest, token).Result;

            QuoteSearchViewModel model = new QuoteSearchViewModel();
            if (objMasterTypeResponse.Status == "Success" && objMasterTypeResponse.PropertyList.Count > 0)
            {
                for (int i = 0; i < objMasterTypeResponse.PropertyList[0].Attribute.Count; i++)
                {
                    if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Pipeline Stage")
                    {
                        model.PipelineStagesList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.ToList();
                        if (model.PipelineStagesList.Count > 0)
                        {
                            model.PipelineStagesList = SequencePipelineStages(model.PipelineStagesList);
                        }

                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Quote Priority")
                    {
                        model.QuotePriorityList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Quote Result")
                    {
                        model.QuoteResultList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Quote Status")
                    {
                        model.QuoteStatusList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Quote Type")
                    {
                        model.QuoteTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Division Type")
                    {
                        model.DivisionList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Quote Room Type")
                    {
                        model.QuoteRoomTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "QRF PRODUCT")
                    {
                        model.ProductList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Purpose of Travel")
                    {
                        model.PurposeOfTravelList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "QRF Destination")
                    {
                        model.DestinationList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.ToList();
                        model.DestinationList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.Select(a => new AttributeValues
                        {
                            AttributeValue_Id = a.AttributeValue_Id,
                            CityName = a.Value.Contains("|") ? a.Value.Split("|")[0].Trim() : "",
                            Value = a.Value.Contains("|") ? a.Value.Split("|")[1].Trim() : a.Value
                        }).OrderBy(a => a.CityName).ThenBy(b => b.Value).ToList();

                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Month")
                    {
                        model.MonthList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Year")
                    {
                        model.YearList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "QRF Date Type")
                    {
                        model.DateTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "ActivityTypeMaster")
                    {
                        model.ExcursionTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "TransportCategoryMaster")
                    {
                        model.TransportTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "TaxRgStatus")
                    {
                        model.TaxRgStatusList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "TaxRgType")
                    {
                        model.TaxRgTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "PickUpDropOffLocations")
                    {
                        model.PickUpDropOffLocations = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "BreakfastType")
                    {
                        model.BreakfastTypeList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "TicketLocation")
                    {
                        model.TicketLocationList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "Floors")
                    {
                        model.FloorsList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => Convert.ToInt32(a.Value)).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "MealStyle")
                    {
                        model.MealStyleList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                    else if (objMasterTypeResponse.PropertyList[0].Attribute[i].AttributeName == "MealCourses")
                    {
                        model.MealCoursesList = objMasterTypeResponse.PropertyList[0].Attribute[i].Values.OrderBy(a => a.Value).ToList();
                    }
                }
            }
            return model;
        }

        public List<AttributeValues> SequencePipelineStages(List<AttributeValues> listPipelineStages)
        {
            for (int i = 0; i < listPipelineStages.Count; i++)
            {
                if (listPipelineStages[i].Value == "Quote Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 1;
                }
                else if (listPipelineStages[i].Value == "Agent Approval Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 2;
                }
                else if (listPipelineStages[i].Value == "Costing Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 3;
                }
                else if (listPipelineStages[i].Value == "Amendment Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 4;
                }
                else if (listPipelineStages[i].Value == "Costing Approval Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 5;
                }
                else if (listPipelineStages[i].Value == "Handover Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 6;
                }
                else if (listPipelineStages[i].Value == "Rejected Pipeline")
                {
                    listPipelineStages[i].SequenceNo = 7;
                }
                else
                {
                    listPipelineStages[i].SequenceNo = 100;
                }
                //else if (listPipelineStages[i].Value == "Itinerary Pipeline")
                //{
                //    listPipelineStages[i].SequenceNo = 2;
                //}
                //else if (listPipelineStages[i].Value == "Proposal Pipeline")
                //{
                //    listPipelineStages[i].SequenceNo = 5;
                //}
            }

            listPipelineStages = listPipelineStages.OrderBy(a => a.SequenceNo).ToList();
            return listPipelineStages;
        }

        #region Quote Agent Info

        public void LoadQuoteAgentInfo(IConfiguration _configuration, string token, ref NewQuoteViewModel model, string loggedInUserName, string CompanyId)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            CurrencyResponse objCurrencyResponse = objSalesProvider.GetCurrencyList(token).Result;
            model.mdlQuoteAgentInfoViewModel.BudgetCurrencyList = objCurrencyResponse.CurrencyList;

            ProdNationalityGetRes objNationalityRes = objSalesProvider.GetNationalityList(CompanyId, token).Result;
            model.mdlQuoteAgentInfoViewModel.NationalityList = objNationalityRes.NationalityList;
            if (string.IsNullOrEmpty(model.mdlQuoteAgentInfoViewModel.Nationality))
                model.mdlQuoteAgentInfoViewModel.Nationality = objNationalityRes.CompanyNationality;

            if (!string.IsNullOrEmpty(model.mdlQuoteAgentInfoViewModel.AgentId))
            {
                AgentContactReq objContactRequest = new AgentContactReq() { Company_Id = model.mdlQuoteAgentInfoViewModel.AgentId };
                AgentContactRes objContactResponse = objSalesProvider.GetContactListForAgent(objContactRequest, token).Result;
                model.mdlQuoteAgentInfoViewModel.ContactPersonList = objContactResponse.ContactProperties;
            }

            if (!string.IsNullOrWhiteSpace(loggedInUserName))
            {
                QuoteSearchReq request = new QuoteSearchReq() { UserName = loggedInUserName };
                QuoteAgentGetRes response = objSalesProvider.GetDivisionList(request, token).Result;
                if (response?.QuoteAgentGetProperties?.Division.Count > 0)
                {
                    if (string.IsNullOrEmpty(model.mdlQuoteAgentInfoViewModel.ProductDivisionId))
                        model.mdlQuoteAgentInfoViewModel.ProductDivisionId = response.CompanyDivision;
                    model.mdlQuoteAgentInfoViewModel.ProductDivisionList = response.QuoteAgentGetProperties.Division;
                }
            }

            QuoteSearchViewModel model2 = BindMasterData(_configuration, token);
            model.mdlQuoteAgentInfoViewModel.QuoteRoomTypeList = model2.QuoteRoomTypeList.Select(a => new QuoteRoomTypeData { RoomTypeID = a.AttributeValue_Id, RoomTypeName = a.Value }).ToList();

            QuoteRoomTypeData quoteRoom = new QuoteRoomTypeData();
            foreach (var item in model.mdlQuoteAgentInfoViewModel.QuoteRoomType)
            {
                quoteRoom = model.mdlQuoteAgentInfoViewModel.QuoteRoomTypeList.Where(a => a.RoomTypeID == item.RoomTypeID).FirstOrDefault();
                quoteRoom.IsSelected = true;
                quoteRoom.RoomCount = item.RoomCount;
            }

            model.mdlQuoteAgentInfoViewModel.ProductTypeList.AddRange(model2.QuoteTypeList);
            //model.mdlQuoteAgentInfoViewModel.ProductDivisionList.AddRange(model2.DivisionList);
            model.mdlQuoteAgentInfoViewModel.PurposeOfTravelList.AddRange(model2.PurposeOfTravelList);
            model.mdlQuoteAgentInfoViewModel.DestinationList.AddRange(model2.DestinationList);
            model.mdlQuoteAgentInfoViewModel.PriorityList.AddRange(model2.QuotePriorityList);

        }

        public QuoteAgentSetRes SetQRFAgentDetails(IConfiguration _configuration, string token, QuoteAgentInfoViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            QUOTEAgentSetReq objQRFAgentRequest = new QUOTEAgentSetReq();

            objQRFAgentRequest.QRFID = model.QRFID;
            objQRFAgentRequest.CurrentPipeline = "Quote Pipeline";
            objQRFAgentRequest.CurrentPipelineStep = "Quote";
            objQRFAgentRequest.CurrentPipelineSubStep = "AgentInformation";
            objQRFAgentRequest.Status = "NewQuote";
            objQRFAgentRequest.QuoteResult = "Success";
            objQRFAgentRequest.Remarks = "Remarks for Quote";
            objQRFAgentRequest.SalesPerson = objCookies["EmailId"] ?? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
            objQRFAgentRequest.SalesPersonUserName = objCookies["UserName"] ?? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault();
            objQRFAgentRequest.SalesPersonCompany = objCookies["CompanyName"] ?? SessionInfo.Where(a => a.AttributeId == "CompanyName").Select(b => b.Value).FirstOrDefault();
            objQRFAgentRequest.LoggedInUserContact_Id = objCookies["ContactId"] ?? SessionInfo.Where(a => a.AttributeId == "ContactId").Select(b => b.Value).FirstOrDefault();
            objQRFAgentRequest.Module = model.Module;
            objQRFAgentRequest.Application = model.Application;
            objQRFAgentRequest.PartnerEntityCode = model.PartnerEntityCode;
            objQRFAgentRequest.Operation = model.Operation;
            objQRFAgentRequest.ckLoginUser_Id = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(a => a.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();

            objQRFAgentRequest.AgentInfo = new AgentInfo
            {
                AgentID = model.AgentId,
                ContactPersonID = model.ContactPerson,
                EmailAddress = model.Email,
                MobileNo = model.MobileNo
            };

            objQRFAgentRequest.AgentProductInfo = new AgentProductInfo
            {
                BudgetAmount = model.BudgetAmount,
                BudgetCurrencyID = model.BudgetCurrency,
                CostingType = model.QuotePricingType,
                DestinationID = model.Destination,
                DivisionID = model.ProductDivisionId,
                Division = model.ProductDivision,
                Duration = model.Duration,
                Priority = model.Priority,
                ProductID = model.Nationality,
                PurposeofTravelID = model.PurposeOfTravel,
                TourCode = model.TourCode,
                TourName = model.TourName,
                TypeID = model.ProductType
            };

            objQRFAgentRequest.AgentPaymentInfo = new AgentPaymentInfo { PaymentTerms = model.PaymentTerms.ToString() };

            objQRFAgentRequest.AgentRoomInfo = new List<AgentRoomInfo>();
            objQRFAgentRequest.AgentRoomInfo.AddRange(model.QuoteRoomTypeList.Where(a => a.IsSelected == true).Select(a => new AgentRoomInfo { RoomTypeID = a.RoomTypeID, RoomTypeName = a.RoomTypeName, RoomCount = a.RoomCount }));

            objQRFAgentRequest.AgentPassengerInfo = new List<AgentPassengerInfo>();
            List<int> lstAge = new List<int> { 0 };
            objQRFAgentRequest.AgentPassengerInfo.Add(new AgentPassengerInfo() { Type = "ADULT", count = model.ApproxPaxAdult, Age = lstAge });
            objQRFAgentRequest.AgentPassengerInfo.Add(new AgentPassengerInfo() { Type = "INFANT", count = model.ApproxPaxInfant, Age = lstAge });

            lstAge = new List<int>();
            if (string.IsNullOrEmpty(model.ApproxPaxChildWithBedAge)) model.ApproxPaxChildWithBedAge = "0";
            lstAge.AddRange(model.ApproxPaxChildWithBedAge.Split(',').Select(int.Parse));
            objQRFAgentRequest.AgentPassengerInfo.Add(new AgentPassengerInfo() { Type = "CHILDWITHBED", count = model.ApproxPaxChildWithBed, Age = lstAge });

            lstAge = new List<int>();
            if (string.IsNullOrEmpty(model.ApproxPaxChildWithoutBedAge)) model.ApproxPaxChildWithoutBedAge = "0";
            lstAge.AddRange(model.ApproxPaxChildWithoutBedAge.Split(',').Select(int.Parse));
            objQRFAgentRequest.AgentPassengerInfo.Add(new AgentPassengerInfo() { Type = "CHILDWITHOUTBED", count = model.ApproxPaxChildWithoutBed, Age = lstAge });

            QuoteAgentSetRes objQRFAgentResponse = objSalesProvider.SetQRFAgentDetails(objQRFAgentRequest, token).Result;
            return objQRFAgentResponse;
        }

        public bool GetQRFAgentByQRFId(IConfiguration _configuration, string token, ref NewQuoteViewModel model)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            QuoteAgentGetReq objQRFAgentRequest = new QuoteAgentGetReq() { QRFID = model.QRFID };
            QuoteAgentGetRes objQRFAgentResponse = objSalesProvider.GetQRFAgentByQRFID(objQRFAgentRequest, token).Result;
            QuoteAgentGetProperties objResult = objQRFAgentResponse.QuoteAgentGetProperties;

            if (objResult != null)
            {
                model.mdlQuoteAgentInfoViewModel.QRFID = objResult.QRFID.ToString();
                model.mdlQuoteAgentInfoViewModel.AgentId = objResult.AgentInfo.AgentID;
                model.mdlQuoteAgentInfoViewModel.AgentName = objResult.AgentInfo.AgentName;
                model.mdlQuoteAgentInfoViewModel.ContactPerson = objResult.AgentInfo.ContactPersonID;
                model.mdlQuoteAgentInfoViewModel.Email = objResult.AgentInfo.EmailAddress;
                model.mdlQuoteAgentInfoViewModel.MobileNo = objResult.AgentInfo.MobileNo;
                model.mdlQuoteAgentInfoViewModel.BudgetAmount = Convert.ToDouble(objResult.AgentProductInfo.BudgetAmount);
                model.mdlQuoteAgentInfoViewModel.BudgetCurrency = objResult.AgentProductInfo.BudgetCurrencyCode;
                model.mdlQuoteAgentInfoViewModel.QuotePricingType = objResult.AgentProductInfo.CostingType;
                model.mdlQuoteAgentInfoViewModel.Destination = objResult.AgentProductInfo.DestinationID;
                model.mdlQuoteAgentInfoViewModel.ProductDivisionId = objResult.AgentProductInfo.DivisionID;
                model.mdlQuoteAgentInfoViewModel.ProductDivision = objResult.AgentProductInfo.Division;
                model.mdlQuoteAgentInfoViewModel.Duration = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                model.mdlQuoteAgentInfoViewModel.Priority = objResult.AgentProductInfo.Priority;
                model.mdlQuoteAgentInfoViewModel.Nationality = objResult.AgentProductInfo.ProductID;
                model.mdlQuoteAgentInfoViewModel.PurposeOfTravel = objResult.AgentProductInfo.PurposeofTravelID;
                model.mdlQuoteAgentInfoViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                model.mdlQuoteAgentInfoViewModel.TourName = objResult.AgentProductInfo.TourName;
                model.mdlQuoteAgentInfoViewModel.ProductType = objResult.AgentProductInfo.TypeID;
                model.mdlQuoteAgentInfoViewModel.PaymentTerms = objResult.AgentPaymentInfo.PaymentTerms;
                model.mdlQuoteAgentInfoViewModel.QuoteRoomType = objResult.AgentRoom.Select(a => new QuoteRoomTypeData
                { IsSelected = true, RoomTypeID = a.RoomTypeID, RoomTypeName = a.RoomTypeName, RoomCount = a.RoomCount }).ToList();

                List<AgentPassengerInfo> obj = objResult.AgentPassengerInfo.Where(m => m.Type == "ADULT").ToList();
                if (obj != null && obj.Count > 0) model.mdlQuoteAgentInfoViewModel.ApproxPaxAdult = obj[0].count;

                obj = objResult.AgentPassengerInfo.Where(m => m.Type == "INFANT").ToList();
                if (obj != null && obj.Count > 0) model.mdlQuoteAgentInfoViewModel.ApproxPaxInfant = obj[0].count;

                obj = objResult.AgentPassengerInfo.Where(m => m.Type == "CHILDWITHBED").ToList();
                if (obj != null && obj.Count > 0)
                {
                    model.mdlQuoteAgentInfoViewModel.ApproxPaxChildWithBed = obj[0].count;
                    model.mdlQuoteAgentInfoViewModel.ApproxPaxChildWithBedAge = string.Join(",", obj[0].Age);
                }

                obj = objResult.AgentPassengerInfo.Where(m => m.Type == "CHILDWITHOUTBED").ToList();
                if (obj != null && obj.Count > 0)
                {
                    model.mdlQuoteAgentInfoViewModel.ApproxPaxChildWithoutBed = obj[0].count;
                    model.mdlQuoteAgentInfoViewModel.ApproxPaxChildWithoutBedAge = string.Join(",", obj[0].Age);
                }

                model.TourInfoHeaderViewModel.QRFID = objResult.QRFID;
                model.TourInfoHeaderViewModel.AgentName = objResult.AgentInfo.AgentName;
                model.TourInfoHeaderViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                model.TourInfoHeaderViewModel.TourName = objResult.AgentProductInfo.TourName;
                model.TourInfoHeaderViewModel.NoOfNights = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                model.TourInfoHeaderViewModel.NoOfDays = Convert.ToInt32(objResult.AgentProductInfo.Duration) + 1;
                model.TourInfoHeaderViewModel.IsLinkedQRFsExist = objResult.IsLinkedQRFsExist;

                model.COHeaderViewModel.QRFID = objResult.QRFID;
                model.COHeaderViewModel.AgentName = objResult.AgentInfo.AgentName;
                model.COHeaderViewModel.TourCode = objResult.AgentProductInfo.TourCode;
                model.COHeaderViewModel.TourName = objResult.AgentProductInfo.TourName;
                model.COHeaderViewModel.NoOfNights = Convert.ToInt32(objResult.AgentProductInfo.Duration);
                model.COHeaderViewModel.NoOfDays = Convert.ToInt32(objResult.AgentProductInfo.Duration) + 1;
                model.COHeaderViewModel.SalesPerson = objResult.SalesPerson;
                model.COHeaderViewModel.ContactPerson = objResult.AgentInfo.ContactPerson;
                model.COHeaderViewModel.Destination = objResult.AgentProductInfo.Destination;
                model.COHeaderViewModel.TravelDate = objResult.DepartureDates.Count > 0 ? objResult.DepartureDates[0].Date : null;
                model.COHeaderViewModel.Version = 0;
                model.COHeaderViewModel.Pax = objResult.AgentPassengerInfo.Where(a => a.Type == "ADULT").Select(b => b.count).FirstOrDefault();
                model.COHeaderViewModel.CostingOfficer = objQRFAgentResponse.QuoteAgentGetProperties.CostingOfficer;
                model.COHeaderViewModel.ValidForTravel = objQRFAgentResponse.QuoteAgentGetProperties.ValidForTravel;
                model.COHeaderViewModel.ValidForAcceptance = objQRFAgentResponse.QuoteAgentGetProperties.ValidForAcceptance;
                model.mdlMenuViewModel.EnquiryPipeline = objResult.CurrentPipeline;

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Margin
        public MarginSetRes SetQRFMarginDetails(IConfiguration _configuration, string token, QuoteMarginViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo, bool IsCostingMargin = false)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            string UserName = "";
            objCookies.TryGetValue("UserName", out UserName);
            UserName = string.IsNullOrEmpty(UserName) ? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault() : UserName;
            MarginSetReq objMarginSetReq = new MarginSetReq
            {
                QRFID = model.QRFID,
                IsCostingMargin = IsCostingMargin,
                VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault(),
            Margins = new Margins
                {
                    SelectedMargin = model.SelectedMargin,
                    CreateDate = DateTime.Now,
                    CreateUser = UserName,
                    EditDate = DateTime.Now,
                    EditUser = UserName,
                    Package = new Package
                    {
                        MarginComputed = new MarginComputed
                        {
                            MarkupType = model.PackageMarginDetails.MarkupType,
                            TotalCost = model.PackageMarginDetails.TotalCost,
                            TotalLeadersCost = model.PackageMarginDetails.TotalLeadersCost,
                            Upgrade = model.PackageMarginDetails.Upgrade
                        }
                    },
                    Product = new Product { MarginComputed = new MarginComputed() },
                    Itemwise = new Itemwise { MarginComputed = new MarginComputed() }
                }
            };

            #region Package Margin
            List<PackageProperties> lstPackageProperties = new List<PackageProperties>();
            if (model.PackageMarginDetails.Accommodation == "Package including Accommodation")
            {
                var pckid = string.IsNullOrEmpty(model.PackageMarginDetails.IncPackageID) ? model.PackageMarginDetails.ExcPackageID : model.PackageMarginDetails.IncPackageID;
                lstPackageProperties.Add(new PackageProperties { ComponentName = model.PackageMarginDetails.Accommodation, MarginUnit = model.PackageMarginDetails.IncMarginUnit, PackageID = pckid, SellingPrice = model.PackageMarginDetails.IncSellingPrice });
            }
            else if (model.PackageMarginDetails.Accommodation == "Package not including Accommodation")
            {
                var pckid = string.IsNullOrEmpty(model.PackageMarginDetails.ExcPackageID) ? model.PackageMarginDetails.IncPackageID : model.PackageMarginDetails.ExcPackageID;
                lstPackageProperties.Add(new PackageProperties { ComponentName = model.PackageMarginDetails.Accommodation, MarginUnit = model.PackageMarginDetails.ExcMarginUnit, PackageID = pckid, SellingPrice = model.PackageMarginDetails.ExcSellingPrice });
            }

            if (model.PackageMarginDetails.SupSelected == true)
            {
                var pckid = model.PackageMarginDetails.SupPackageID;
                lstPackageProperties.Add(new PackageProperties { ComponentName = "Suppliments", MarginUnit = model.PackageMarginDetails.SupMarginUnit, PackageID = pckid, SellingPrice = model.PackageMarginDetails.SupSellingPrice });
            }
            if (model.PackageMarginDetails.OptSelected == true)
            {
                var pckid = model.PackageMarginDetails.OptPackageID;
                lstPackageProperties.Add(new PackageProperties { ComponentName = "Optionals", MarginUnit = model.PackageMarginDetails.OptMarginUnit, PackageID = pckid, SellingPrice = model.PackageMarginDetails.OptSellingPrice });
            }

            objMarginSetReq.Margins.Package.PackageProperties = lstPackageProperties;
            #endregion

            #region Product Margin
            foreach (var item in model.ProductMarginDetails.ProductProperties)
            {
                if (!item.IsProduct && item.IsProdtype)
                {
                    model.ProductMarginDetails.ProductProperties.Where(a => a.ProductMaster == item.ProductMaster).ToList().ForEach(b =>
                    {
                        b.IsProdtype = true;
                        b.SellingPrice = item.SellingPrice;
                        b.MarginUnit = item.MarginUnit;
                    });
                }
                else if (item.IsProduct && item.IsProdtype)
                {
                    VGER_WAPI_CLASSES.ProductProperties productProperties = new VGER_WAPI_CLASSES.ProductProperties();
                    productProperties.HowMany = item.HowMany;
                    productProperties.SellingPrice = item.SellingPrice;
                    productProperties.MarginUnit = item.MarginUnit;
                    productProperties.Prodtype = item.Prodtype;
                    productProperties.ProductID = item.ProductID;
                    productProperties.VoyagerProductType_Id = item.VoyagerProductType_Id;
                    objMarginSetReq.Margins.Product.ProductProperties.Add(productProperties);
                }
            }
            objMarginSetReq.Margins.Product.MarginComputed.MarkupType = model.ProductMarginDetails.MarkupType;
            objMarginSetReq.Margins.Product.MarginComputed.TotalCost = model.ProductMarginDetails.TotalCost;
            objMarginSetReq.Margins.Product.MarginComputed.TotalLeadersCost = model.ProductMarginDetails.TotalLeadersCost;
            objMarginSetReq.Margins.Product.MarginComputed.Upgrade = model.ProductMarginDetails.Upgrade;
            #endregion

            #region Itemwise Margin
            foreach (var item in model.ItemwiseMarginDetails.ProductProperties)
            {
                if (!item.IsPosition && item.IsProdtype)
                {
                    model.ItemwiseMarginDetails.ProductProperties.Where(a => a.Prodtype == item.Prodtype).ToList().ForEach(b =>
                    {
                        b.SellingPrice = item.SellingPrice;
                        b.MarginUnit = item.MarginUnit;
                    });
                }
                else if (item.IsPosition)
                {
                    objMarginSetReq.Margins.Itemwise.ItemProperties.Add(new ItemProperties
                    {
                        ItemID = item.ProductID,
                        PositionID = item.PositionID,
                        ProductName = item.ProductName,
                        HowMany = item.HowMany,
                        SellingPrice = item.SellingPrice,
                        MarginUnit = item.MarginUnit,
                        Prodtype = item.Prodtype,
                        VoyagerProductType_Id = item.VoyagerProductType_Id
                    });
                }
            }
            objMarginSetReq.Margins.Itemwise.MarginComputed.MarkupType = model.ItemwiseMarginDetails.MarkupType;
            objMarginSetReq.Margins.Itemwise.MarginComputed.TotalCost = model.ItemwiseMarginDetails.TotalCost;
            objMarginSetReq.Margins.Itemwise.MarginComputed.TotalLeadersCost = model.ItemwiseMarginDetails.TotalLeadersCost;
            objMarginSetReq.Margins.Itemwise.MarginComputed.Upgrade = model.ItemwiseMarginDetails.Upgrade;
            #endregion

            objMarginSetReq.Margins.CreateUser = objCookies["UserName"] ?? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault();
            objMarginSetReq.Margins.EditUser = objCookies["UserName"] ?? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault();
            MarginSetRes objMarginSetRes = objSalesProvider.SetQRFMarginDetails(objMarginSetReq, token).Result;
            return objMarginSetRes;
        }

        public bool GetQRFMarginDetailsByQRFID(IConfiguration _configuration, string token, ref QuoteMarginViewModel model)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            MarginGetReq objMarginGetReq = new MarginGetReq() { QRFID = model.QRFID };
            MarginGetRes objMarginGetRes = objSalesProvider.GetQRFMarginDetailsByQRFID(objMarginGetReq, token).Result;
            Margins objResult = objMarginGetRes.Margins;
            return true;
        }
        #endregion

        #region Quote Departure Dates

        public DepartureDateSetResponse SetQRFDepartureDetails(IConfiguration _configuration, string token, QuoteDateRangeViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            SalesProviders salesProviders = new SalesProviders(_configuration);
            DepartureDateSetRequest departureSetRequest = new DepartureDateSetRequest();
            departureSetRequest.QRFID = model.QRFID;
            departureSetRequest.UserEmail = objCookies["EmailId"] ?? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
            departureSetRequest.VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();
            departureSetRequest.Departures = model.QuoteSpecificDateViewModel
                .Select(a => new DepartureDates
                {
                    Departure_Id = a.DepartureId,
                    Date = Convert.ToDateTime(a.SelectedDate).AddMinutes(1),
                    NoOfDep = a.NoOfDepartures,
                    PaxPerDep = a.PaxPerDeparture,
                    Warning = a.Warning,
                    IsDeleted = a.IsDeleted,
                    CreateUser = objCookies["EmailId"] ?? SessionInfo.Where(x => x.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault(),
                    CreateDate = DateTime.Now,
                    EditUser = objCookies["EmailId"] ?? SessionInfo.Where(x => x.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault(),
                    EditDate = DateTime.Now
                }).ToList();

            DepartureDateSetResponse departureSetResponse = salesProviders.SetQRFDepartureDetails(departureSetRequest, token).Result;
            return departureSetResponse;
        }

        #endregion

        #region Quote Pax Range
        public bool GetQRFPaxSlabDetails(IConfiguration _configuration, string token, ref NewQuoteViewModel model)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            CoachesGetResponse objCoachTypesResponse = objSalesProvider.GetCoachTypesForTransport(token).Result;
            List<AttributeValues> lstCoachSizes = objCoachTypesResponse.CategoryName.Select(data => new AttributeValues { AttributeValue_Id = data, Value = data }).ToList();
            List<AttributeValues> lstTransportBrand = objCoachTypesResponse.BrandName.Select(data => new AttributeValues { AttributeValue_Id = data, Value = data }).ToList();
            QuoteSearchViewModel model2 = BindMasterData(_configuration, token, "TransportCategoryMaster");
            List<AttributeValues> lstCategory = model2.TransportTypeList;
            model.mdlQuotePaxRangeViewModel.QRFID = model.QRFID;

            if (!string.IsNullOrEmpty(model.QRFID))
            {
                PaxGetResponse paxGetResponse = objSalesProvider.GetQRFPaxRangeDetails(new PaxGetRequest { QRFID = model.QRFID }, token).Result;
                QuoteAgentGetRes objQRFAgentRes = objSalesProvider.GetQRFAgentByQRFID(new QuoteAgentGetReq { QRFID = model.QRFID }, token).Result;
                model.mdlQuotePaxRangeViewModel.HotelsOption = paxGetResponse.PaxSlabDetails.HotelFlag;
                model.mdlQuotePaxRangeViewModel.HotelCategoryList = paxGetResponse.PaxSlabDetails.HotelCategories.Select(a => new ProductAttributeDetails { Value = a.Name, AttributeId = a.VoyagerDefProductCategoryId }).ToList();
                model.mdlQuotePaxRangeViewModel.HotelChainList = paxGetResponse.PaxSlabDetails.HotelChain.Select(a => new ProductAttributeDetails { Value = a.Name, AttributeId = a.AttributeId }).ToList();
                model.mdlQuotePaxRangeViewModel.QuotePaxSlabDetails = paxGetResponse.PaxSlabDetails.PaxSlabs
                    .Select(a => new QuotePaxSlabDetails
                    {
                        PaxSlabId = a.PaxSlab_Id,
                        PaxSlabFrom = a.From,
                        PaxSlabTo = a.To,
                        PaxSlabDividedBy = a.DivideByCost,
                        TransportCategory = a.Category,
                        TransportCategoryID = a.Category_Id,
                        TransportCoachType = a.CoachType,
                        TransportCoachTypeID = a.CoachType_Id,
                        TransportBrand = a.Brand,
                        TransportBrandID = a.Brand_Id,
                        TransportCount = a.HowMany,
                        BudgetCurrency = objQRFAgentRes.QuoteAgentGetProperties.AgentProductInfo.BudgetCurrency,
                        BudgetAmount = a.BudgetAmount,
                        IsDeleted = false,
                        TransportCoachTypeList = lstCoachSizes,
                        TransportCategoryList = lstCategory,
                        TransportBrandList = lstTransportBrand
                    }).ToList();
            }
            else model.mdlQuotePaxRangeViewModel.HotelsOption = "no";

            if (model.mdlQuotePaxRangeViewModel.HotelCategoryList.Count < 1) model.mdlQuotePaxRangeViewModel.HotelCategoryList.Add(new ProductAttributeDetails());
            if (model.mdlQuotePaxRangeViewModel.HotelChainList.Count < 1) model.mdlQuotePaxRangeViewModel.HotelChainList.Add(new ProductAttributeDetails());

            return true;
        }

        public PaxSetResponse SetPaxRangeDetails(IConfiguration _configuration, string token, QuotePaxRangeViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            SalesProviders salesProviders = new SalesProviders(_configuration);
            PaxSetRequest paxSetRequest = new PaxSetRequest();
            paxSetRequest.QRFID = model.QRFID;
            paxSetRequest.PaxSlabDetails.CreateUser = objCookies["EmailId"] ?? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
            paxSetRequest.PaxSlabDetails.CreateDate = DateTime.Now;
            paxSetRequest.PaxSlabDetails.EditUser = objCookies["EmailId"] ?? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
            paxSetRequest.PaxSlabDetails.EditDate = DateTime.Now;
            paxSetRequest.UserEmail = objCookies["EmailId"] ?? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault();
            paxSetRequest.VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();
            paxSetRequest.PaxSlabDetails.HotelFlag = model.HotelsOption;
            if (model.HotelsOption == "yes")
            {
                paxSetRequest.PaxSlabDetails.HotelCategories = model.HotelCategoryList.Select(a => new HotelCategories { Name = a.Value, VoyagerDefProductCategoryId = a.AttributeId }).ToList();
                paxSetRequest.PaxSlabDetails.HotelChain = model.HotelChainList.Select(a => new HotelChain { Name = a.Value, AttributeId = a.AttributeId }).ToList();
            }
            paxSetRequest.PaxSlabDetails.PaxSlabs = model.QuotePaxSlabDetails
                .Select(a => new PaxSlabs
                {
                    PaxSlab_Id = a.PaxSlabId,
                    From = a.PaxSlabFrom,
                    To = a.PaxSlabTo,
                    HowMany = a.TransportCount,
                    Brand = a.TransportBrand,
                    Brand_Id = a.TransportBrandID,
                    Category = a.TransportCategory,
                    Category_Id = a.TransportCategoryID,
                    CoachType = a.TransportCoachType,
                    CoachType_Id = a.TransportCoachTypeID,
                    DivideByCost = a.PaxSlabDividedBy,
                    BudgetAmount = a.BudgetAmount,
                    IsDeleted = a.IsDeleted,
                    CreateDate = DateTime.Now,
                    EditDate = DateTime.Now,
                    DeleteDate = DateTime.Now,
                    DeleteUser = objCookies["EmailId"] ?? SessionInfo.Where(x => x.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault()
                }).ToList();

            PaxSetResponse departureSetResponse = salesProviders.SetQRFPaxRangeDetails(paxSetRequest, token).Result;
            return departureSetResponse;
        }

        //private List<QuotePaxSlabDetails> GetDefaultPaxSlab(List<AttributeValues> lstCoachSizes, List<AttributeValues> lstCategory, List<AttributeValues> lstTransportBrand)
        //{
        //    string defCatId = "786fab83-6a95-49c8-ab98-7aa795b3902d";
        //    string defCat = "Standard";
        //    string defCoach = "49-Seater with WC and intercom";
        //    List<QuotePaxSlabDetails> listPaxSlab = new List<QuotePaxSlabDetails>
        //    {
        //        new QuotePaxSlabDetails
        //        {
        //            PaxSlabId = 0,
        //            PaxSlabFrom = 10,
        //            PaxSlabTo = 19,
        //            PaxSlabDividedBy = 10,
        //            TransportCategory = defCat,
        //            TransportCategoryID = defCatId,
        //            TransportCoachType = defCoach,
        //            TransportCoachTypeID = defCoach,
        //            TransportBrand = "",
        //            TransportBrandID = "",
        //            TransportCount = 1,
        //            IsDeleted = false,
        //            TransportCoachTypeList = lstCoachSizes,
        //            TransportCategoryList = lstCategory,
        //            TransportBrandList = lstTransportBrand
        //        },
        //        new QuotePaxSlabDetails
        //        {
        //            PaxSlabId = 0,
        //            PaxSlabFrom = 20,
        //            PaxSlabTo = 29,
        //            PaxSlabDividedBy = 20,
        //            TransportCategory = defCat,
        //            TransportCategoryID = defCatId,
        //            TransportCoachType = defCoach,
        //            TransportCoachTypeID = defCoach,
        //            TransportBrand = "",
        //            TransportBrandID = "",
        //            TransportCount = 1,
        //            IsDeleted = false,
        //            TransportCoachTypeList = lstCoachSizes,
        //            TransportCategoryList = lstCategory,
        //            TransportBrandList = lstTransportBrand
        //        },
        //        new QuotePaxSlabDetails
        //        {
        //            PaxSlabId = 0,
        //            PaxSlabFrom = 30,
        //            PaxSlabTo = 39,
        //            PaxSlabDividedBy = 30,
        //            TransportCategory = defCat,
        //            TransportCategoryID = defCatId,
        //            TransportCoachType = defCoach,
        //            TransportCoachTypeID = defCoach,
        //            TransportBrand = "",
        //            TransportBrandID = "",
        //            TransportCount = 1,
        //            IsDeleted = false,
        //            TransportCoachTypeList = lstCoachSizes,
        //            TransportCategoryList = lstCategory,
        //            TransportBrandList = lstTransportBrand
        //        },
        //        new QuotePaxSlabDetails
        //        {
        //            PaxSlabId = 0,
        //            PaxSlabFrom = 40,
        //            PaxSlabTo = 49,
        //            PaxSlabDividedBy = 40,
        //            TransportCategory = defCat,
        //            TransportCategoryID = defCatId,
        //            TransportCoachType = defCoach,
        //            TransportCoachTypeID = defCoach,
        //            TransportBrand = "",
        //            TransportBrandID = "",
        //            TransportCount = 1,
        //            IsDeleted = false,
        //            TransportCoachTypeList = lstCoachSizes,
        //            TransportCategoryList = lstCategory,
        //            TransportBrandList = lstTransportBrand
        //        },
        //    };

        //    return listPaxSlab;
        //}
        #endregion

        #region FOC
        public bool GetFOCDetails(IConfiguration _configuration, string token, ref NewQuoteViewModel model)
        {
            SalesProviders objSalesProvider = new SalesProviders(_configuration);
            model.mdlQuoteFOCViewModel.QRFID = model.QRFID;

            if (!string.IsNullOrEmpty(model.QRFID))
            {
                PaxGetRequest paxGetRequest = new PaxGetRequest() { QRFID = model.QRFID };
                FOCGetResponse paxGetResponse = objSalesProvider.GetQRFFOCDetails(paxGetRequest, token).Result;
                if (paxGetResponse != null && paxGetResponse.FOCDetails != null && paxGetResponse.FOCDetails.Count > 0)
                {
                    model.mdlQuoteFOCViewModel.StandardFoc = paxGetResponse.StandardFOC;
                    model.mdlQuoteFOCViewModel.QuoteFOCDetails = paxGetResponse.FOCDetails
                        .Select(a => new QuoteFOCDetails
                        {
                            FOCId = a.FOC_Id,
                            DateRange = a.DateRange,
                            PaxSlab = a.PaxSlab,
                            DateRangeId = a.DateRangeId,
                            PaxSlabId = a.PaxSlabId,
                            DivideByCost = a.DivideByCost,
                            FOCSingle = a.FOCSingle,
                            FOCTwin = a.FOCTwin,
                            FOCDouble = a.FOCDouble,
                            FOCTriple = a.FOCTriple,
                            IsDeleted = false
                        }).ToList();
                }
                else
                {
                    int minDivideByCost = Convert.ToInt16(paxGetResponse.FOCDetails.Min(a => a.DivideByCost).ToString());
                    model.mdlQuoteFOCViewModel.StandardFoc = true;
                    model.mdlQuoteFOCViewModel.QuoteFOCDetails.Add(new QuoteFOCDetails
                    {
                        DateRange = "ALL",
                        PaxSlab = "ALL",
                        DivideByCost = minDivideByCost,
                        FOCSingle = 0,
                        FOCTwin = 0,
                        FOCDouble = 0,
                        FOCTriple = 0
                    });
                }
            }

            return true;
        }

        public PaxSetResponse SetFOCDetails(IConfiguration _configuration, string token, QuoteFOCViewModel model, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            SalesProviders salesProviders = new SalesProviders(_configuration);

            if (model.StandardFoc)
            {
                var res = model.QuoteFOCDetails.Where(x => x.DateRange.ToUpper() == "ALL").FirstOrDefault();
                model.QuoteFOCDetails.Where(x => x.DateRange.ToUpper() != "ALL").ToList().ForEach(y =>
                {
                    y.FOCSingle = res.FOCSingle;
                    y.FOCTwin = res.FOCTwin;
                    y.FOCDouble = res.FOCDouble;
                    y.FOCTriple = res.FOCTriple;
                });
            }
            FOCSetRequest paxSetRequest = new FOCSetRequest
            {
                QRFID = model.QRFID,
                StandardFOC = model.StandardFoc,
                VoyagerUserId = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault(),
            FOCDetails = model.QuoteFOCDetails.Where(x => x.DateRange.ToUpper() != "ALL").Select(a => new FOCDetails
                {
                    FOC_Id = a.FOCId,
                    DateRange = a.DateRange,
                    PaxSlab = a.PaxSlab,
                    DateRangeId = a.DateRangeId,
                    PaxSlabId = a.PaxSlabId,
                    DivideByCost = a.DivideByCost,
                    FOCSingle = a.FOCSingle,
                    FOCTwin = a.FOCTwin,
                    FOCDouble = a.FOCDouble,
                    FOCTriple = a.FOCTriple,
                    IsDeleted = a.IsDeleted,
                    CreateUser = objCookies["EmailId"] ?? SessionInfo.Where(x => x.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault(),
                    CreateDate = DateTime.Now,
                    EditUser = objCookies["EmailId"] ?? SessionInfo.Where(x => x.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault(),
                    EditDate = DateTime.Now,
                    
                }).ToList()
            };

            PaxSetResponse FOCSetResponse = salesProviders.SetQRFFOCDetails(paxSetRequest, token).Result;
            return FOCSetResponse;
        }
        #endregion

        public ProductRangeGetRes GetProductRangeList(string ProductId, string CategoryId, string AdditionalYN, string token, string QRFID = "", List<string> ProdIdList = null, List<string> ProdCatIdList = null)
        {
            MasterProviders objMasterProviders = new MasterProviders(_configuration);
            bool? bAdditionalYN = null;
            if (!string.IsNullOrEmpty(AdditionalYN)) bAdditionalYN = Convert.ToBoolean(AdditionalYN);
            ProductRangeGetReq prodRangeGetReq = new ProductRangeGetReq { QRFID = QRFID, ProductId = ProductId, ProductCatId = CategoryId, AdditionalYN = bAdditionalYN, ProductIdList = ProdIdList, ProductCatIdList = ProdCatIdList };
            ProductRangeGetRes prodRangeGetRes = objMasterProviders.GetProductRangeByParam(prodRangeGetReq, token).Result;
            return prodRangeGetRes;
        }

        public QuoteSetRes SetQuoteDetails(IConfiguration _configuration, string token, string QRFID, string quoteRemarks, string Officer, IRequestCookieCollection objCookies, List<ProductAttributeDetails> SessionInfo)
        {
            QRFSummaryProviders objQrfSummaryProvider = new QRFSummaryProviders(_configuration);
            QuoteSetReq objQuoteRequest = new QuoteSetReq();
            string emailId = "";
            objCookies.TryGetValue("EmailId", out emailId);
            emailId = string.IsNullOrEmpty(emailId) ? SessionInfo.Where(a => a.AttributeId == "EmailId").Select(b => b.Value).FirstOrDefault() : emailId;
            string PlacerUser = "";
            objCookies.TryGetValue("UserName", out PlacerUser);
            PlacerUser = string.IsNullOrEmpty(PlacerUser) ? SessionInfo.Where(a => a.AttributeId == "UserName").Select(b => b.Value).FirstOrDefault() : PlacerUser;

            objQuoteRequest.QRFID = QRFID;
            objQuoteRequest.Remarks = quoteRemarks;
            objQuoteRequest.PlacerUser = PlacerUser;
            objQuoteRequest.PlacerEmail = emailId;
            objQuoteRequest.CostingOfficer = Officer;
            objQuoteRequest.EnquiryPipeline = "Quote Pipeline";
            objQuoteRequest.VoyagerUserID = objCookies["VoyagerUser_Id"] ?? SessionInfo.Where(x => x.AttributeId == "VoyagerUser_Id").Select(b => b.Value).FirstOrDefault();


            QuoteSetRes objQuoteResponse = objQrfSummaryProvider.UpdateQuoteDetails(objQuoteRequest, token).Result;
            return objQuoteResponse;
        }
    }
}
