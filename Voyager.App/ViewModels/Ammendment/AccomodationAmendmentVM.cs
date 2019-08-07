using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class AccomodationAmendmentVM
    {
        public MenuViewModel MenuViewModel { get; set; } = new MenuViewModel();
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();
        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; } = new TourInfoHeaderViewModel();
        public List<ProductSRPRouteInfo> ProductSRPRouteInfo { get; set; } = new List<ProductSRPRouteInfo>();
        public ProductsSRPViewModel ProductsSRPViewModel { get; set; } = new ProductsSRPViewModel();
    }
}
