using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using VGER_WAPI_CLASSES;
using Voyager.App.ViewModels;

namespace Voyager.App.Mappers
{
    public class ItineraryMapper
    {
        #region Declaration

        private readonly IConfiguration _configuration;

        public ItineraryMapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        public List<ItineraryDaysInfo> GetSuggestedItineraryDetails(ItineraryGetRes itineraryGetRes, string token)
        {
            mItinerary model = new mItinerary();
            List<ItineraryDaysInfo> itineraryDays = new List<ItineraryDaysInfo>();
            try
            {
               
                var totalItineraryDays = itineraryGetRes.Itinerary.ItineraryDays.Count;
                if (itineraryGetRes.ResponseStatus.Status == "Success" && totalItineraryDays > 0)
                {
                    int counter = 0;
                    foreach (var itinerary in itineraryGetRes.Itinerary.ItineraryDays)
                    {                       
                        var obj = new ItineraryDaysInfo();
                        obj.ItineraryDaysId = itinerary.ItineraryDaysId;
                        obj.Day = itinerary.Day;
                        obj.Date = itinerary.Date;//.ToString("dd MMM yy");
                        obj.DayOfWeek = itinerary.DayOfWeek;
                        //obj.City = itinerary.City;                      
                        
                        if (counter == 0)
                        {
                            obj.ItineraryName = "ARRIVE AT " + itinerary.ItineraryName.Split('-')[0];
                        }
                        else if (counter == totalItineraryDays - 1)
                        {
                            obj.ItineraryName = "DEPART FROM " + itinerary.ItineraryName.Split('-')[0];
                        }
                        else
                        {
                            obj.ItineraryName = itinerary.ItineraryName;
                        }

                        foreach (var description in itinerary.ItineraryDescription)
                        {
                            var desc = new ItineraryDescriptionInfo();
                            desc.PositionId = description.PositionId;
                            desc.City = description.City;
                            desc.ProductName = description.ProductName;
                            desc.ProductType = description.ProductType;
                            desc.Type = description.Type;
                            desc.IsDeleted = description.IsDeleted;                           
                            desc.StartTime = description.StartTime;
                            desc.EndTime = description.EndTime;
                            obj.ItineraryDescription.Add(desc);
                        }
                        itineraryDays.Add(obj);
                        counter++;
                    }
                }                
            }
            catch (Exception ex)
            {
            }

            return itineraryDays;
        }
    }
}
