using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class SimilarHotelsViewModel
    {
        public string BookingNumber { get; set; }
        public string PositionId { get; set; }
        public string ProductId { get; set; }
        public string HotelName { get; set; }
        public string HotelDesc { get; set; }
        public bool IsClone { get; set; }
        public string Caller { get; set; }
        public List<ProductList> SelectedHotelList { get; set; }
        public List<ProductList> BlackListedHotelList { get; set; }

        public SimilarHotelsViewModel()
        {
            SelectedHotelList = new List<ProductList>();
            BlackListedHotelList = new List<ProductList>();
        }
    }
}
