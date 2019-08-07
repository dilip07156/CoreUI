using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Contracts;
using Voyager.App.Models;

namespace Voyager.App.ViewModels
{
    public class QuoteSearchViewModel
    {
        [Required(ErrorMessage = "The Pipeline Stages field is required.")]
        public string PipelineStages { get; set; } = "Quote Pipeline";
        public List<AttributeValues> PipelineStagesList { get; set; }

        public string AgentName { get; set; }
        public string CNKReferenceNo { get; set; }
        public string AgentReferenceNo { get; set; }
        public string AgentTour { get; set; }
        //public DateTime? From { get; set; }
        //public DateTime? To { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        public string QuotePriority { get; set; }
        public List<AttributeValues> QuotePriorityList { get; set; }

        public string DateType { get; set; }
        public List<AttributeValues> DateTypeList { get; set; }

        public string ExcursionType { get; set; }
        public List<AttributeValues> ExcursionTypeList { get; set; }

        public string TransportType { get; set; }
        public List<AttributeValues> TransportTypeList { get; set; }

        public List<AttributeValues> TaxRgStatusList { get; set; }

        public List<AttributeValues> TaxRgTypeList { get; set; }
        public List<AttributeValues> PickUpDropOffLocations { get; set; }
        public List<AttributeValues> BreakfastTypeList { get; set; }
        public List<AttributeValues> TicketLocationList { get; set; }
        public List<AttributeValues> FloorsList { get; set; }
        public List<AttributeValues> MealStyleList { get; set; }
        public List<AttributeValues> MealCoursesList { get; set; }

        public string QuoteResult { get; set; }
        public List<AttributeValues> QuoteResultList { get; set; }

        public string Month { get; set; }
        public List<AttributeValues> MonthList { get; set; }

        public int? Year { get; set; }
        public List<AttributeValues> YearList { get; set; }

        public string QuoteStatus { get; set; }
        public List<AttributeValues> QuoteStatusList { get; set; }

        public List<AttributeValues> QuoteTypeList { get; set; }
        public string Division { get; set; }
        public List<AttributeValues> DivisionList { get; set; }
        public List<AttributeValues> QuoteRoomTypeList { get; set; }
        public List<AttributeValues> ProductList { get; set; }
        public List<AttributeValues> PurposeOfTravelList { get; set; }

        public string Destination { get; set; }
        public List<AttributeValues> DestinationList { get; set; }

        public EnquiryPipelineViewModel EnquiryPipeline { get; set; }
    }

    public class EnquiryPipelineViewModel
    {
        public List<QuoteSearchDetails> QuoteSearchDetails { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
    }
}
