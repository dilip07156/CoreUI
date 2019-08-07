using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;
using Voyager.App.Models;

namespace Voyager.App.ViewModels
{
    public class TourEntitiesViewModel
    {
        public string QRFID { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public TourInfoHeaderViewModel TourInfoHeaderViewModel { get; set; }

        public List<DynamicTourEntity> DynamicTourEntity { get; set; }

        public List<TourEntities> TourEntities { get; set; }

        public List<string> PositionNotExists { get; set; }

        public List<AutoCompleteTextBox> AutoCompleteTextBox { get; set; }

        public PaxSlabDetails PaxSlabDetails { get; set; }

        public ResponseStatus ResponseStatus { get; set; }

        public TourEntitiesViewModel()
        {
            MenuViewModel = new MenuViewModel { MenuName = "TourEntities" };
            DynamicTourEntity = new List<DynamicTourEntity>();
            TourEntities = new List<TourEntities>();
            PositionNotExists = new List<string>();
            TourInfoHeaderViewModel = new TourInfoHeaderViewModel();
            PaxSlabDetails = new PaxSlabDetails();
            PaxSlabDetails.PaxSlabs = new List<PaxSlabs>();
            AutoCompleteTextBox = new List<AutoCompleteTextBox>();
        }
    }

    public class TourEntitiesParam
    {
        public string QRFID { get; set; }
        public string Type { get; set; }
        public string TourType { get; set; }
        public string PositionID { get; set; }
        public List<TourEntities> TourEntities { get; set; }
    }
}