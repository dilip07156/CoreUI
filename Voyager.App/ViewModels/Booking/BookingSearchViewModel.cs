using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class BookingSearchViewModel
    {
        public string BookingNumber { get; set; }

        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public int? CNKReferenceNo { get; set; }
        public string AgentCode { get; set; }
        public string AgentTour { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public string DateType { get; set; }
        public List<AttributeValues> DateTypeList { get; set; }

        public string BookingStatus { get; set; }
        public List<Attributes> BookingStatusList { get; set; }

        public BookingPipelineViewModel BookingPipeline { get; set; }

        public BookingSearchViewModel()
        {
            DateTypeList = new List<AttributeValues>();
            BookingStatusList = new List<Attributes>();
            BookingPipeline = new BookingPipelineViewModel();
        }
    }

    public class BookingPipelineViewModel
    {
        public List<BookingList> BookingList { get; set; }
        public List<BookingPositionViewModel> HotelPositions { get; set; }
        public List<BookingPositionViewModel> MealPositions { get; set; }
        public List<BookingPositionViewModel> AttractionPositions { get; set; }
        public List<BookingPositionViewModel> TransferPositions { get; set; }
        public List<BookingPositionViewModel> GuidePositions { get; set; }
        public List<BookingPositionViewModel> OtherPositions { get; set; }
        public string Header { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }

        public BookingPipelineViewModel()
        {
            BookingList = new List<BookingList>();
            HotelPositions = new List<BookingPositionViewModel>();
            MealPositions = new List<BookingPositionViewModel>();
            AttractionPositions = new List<BookingPositionViewModel>();
            TransferPositions = new List<BookingPositionViewModel>();
            GuidePositions = new List<BookingPositionViewModel>();
            OtherPositions = new List<BookingPositionViewModel>();
        }
    }

    public class BookingPositionViewModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ProductName { get; set; }
        public string Board { get; set; }
        public string Single { get; set; }
        public string Double { get; set; }
        public string Triple { get; set; }
        public string Twin { get; set; }
        public string Status { get; set; }
        public string RestaurantType { get; set; }
        public string Meal { get; set; }
        public string Menu { get; set; }
        public string ProdRangeId { get; set; }
        public string ProductType { get; set; }
        public string Option { get; set; }
        public string Unit { get; set; }
        public string PickUpLoc { get; set; }
        public string DropOffLoc { get; set; }
        public string Driver { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class BookingDocDownloadViewModel
    {
        public BookingDocDownloadViewModel()
        {
        }

        public string Type { get; set; }       
        public string BookingId { get; set; }
        public string BookingNo { get; set; }

        public string DocumentId { get; set; }
        public string FilePath { get; set; }
        public string FileCreationDate { get; set; }

        public string FinalItineraryDocumentId { get; set; }
        public string FinalItineraryFilePath { get; set; }
        public string FinalItineraryFileCreationDate { get; set; }
    }
}
