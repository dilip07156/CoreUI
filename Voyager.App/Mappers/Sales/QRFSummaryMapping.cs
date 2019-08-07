using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;
using Voyager.App.ViewModels;

namespace Voyager.App.Mappers
{
    public class QRFSummaryMapping
    {
        #region Declaration
     
        private readonly IConfiguration _configuration;

        public QRFSummaryMapping(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
                
        public List<SummaryDetails> GetQRFSummaryDetails(QRFSummaryGetRes qrfSummaryGetRes, string token)
        {
            List<SummaryDetails> summaryDetails = new List<SummaryDetails>();
            try
            {
                var totalSummaryDetails = qrfSummaryGetRes.SummaryDetailsInfo.Count;
                if (qrfSummaryGetRes.ResponseStatus.Status == "Success" && totalSummaryDetails > 0)
                {
                    int counter = 0;
                    foreach (var summary in qrfSummaryGetRes.SummaryDetailsInfo)
                    {
                        var objSummary = new SummaryDetails();
                        objSummary.Day = summary.Day;
                        objSummary.OriginalItineraryDate = DateTime.Parse(summary.OriginalItineraryDate.ToString()).ToString("dd MMM yy");
                        objSummary.OriginalItineraryDay = summary.OriginalItineraryDay;
                        objSummary.PlaceOfService = summary.PlaceOfService;
                        objSummary.OriginalItineraryName = summary.OriginalItineraryName;
                        //if (counter == 0)
                        //{
                        //    objSummary.OriginalItineraryName = "ARRIVE AT " + summary.OriginalItineraryName.Split('-')[0];
                        //}
                        //else if (counter == totalSummaryDetails - 1)
                        //{
                        //    objSummary.OriginalItineraryName = "DEPART FROM " + summary.OriginalItineraryName.Split('-')[0];
                        //}
                        //else
                        //{
                        //    objSummary.OriginalItineraryName = summary.OriginalItineraryName;
                        //}

                        foreach (var itinerary in summary.OriginalItineraryDetails)
                        {
                            var objOriginal = new OriginalItineraryDetails();
                            objOriginal.PositionId = itinerary.PositionId;
                            objOriginal.Allocation = itinerary.Allocation;
                            objOriginal.NumberOfPax = itinerary.NumberOfPax;
                            objOriginal.Supplier = itinerary.Supplier;
                            objOriginal.OriginalItineraryDescription = itinerary.OriginalItineraryDescription;
                            objOriginal.ProductType = itinerary.ProductType;
                            objOriginal.TLRemarks = itinerary.TLRemarks;
                            objOriginal.OPSRemarks = itinerary.OPSRemarks;
                            objOriginal.IsDeleted = itinerary.IsDeleted;
                            objOriginal.KeepAs = itinerary.KeepAs;
                            objOriginal.ProductCategoryId = itinerary.ProductCategoryId;
                            objOriginal.ProductCategory = itinerary.ProductCategory;
                            objOriginal.ProductTypeChargeBasis = itinerary.ProductTypeChargeBasis;
                            objOriginal.BuyCurrency = itinerary.BuyCurrency;
                            objOriginal.StartTime = itinerary.StartTime;
                            objOriginal.EndTime = itinerary.EndTime;
                            objSummary.OriginalItineraryDetails.Add(objOriginal);                            
                        }                       
                        summaryDetails.Add(objSummary);
                        counter++;
                    }
                    //model.SummaryDetails = summaryDetails;
                    
                }
            }
            catch (Exception ex)
            {
            }

            return summaryDetails;
        }
    }
}
