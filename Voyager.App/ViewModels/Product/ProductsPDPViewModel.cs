using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class ProductsPDPViewModel
    {
        public MenuViewModel MenuViewModel { get; set; } = new MenuViewModel();
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();
        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; } = new TourInfoHeaderViewModel();
        public Products Products { get; set; } = new Products();
        public string[] KeyFacilities { get; set; }
        public string ImageInitial { get; set; }
        public string ProductMapUrl { get; set; }
    }
}
