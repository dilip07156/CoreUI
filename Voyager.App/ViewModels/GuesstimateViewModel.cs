using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class GuesstimateViewModel
    {
        public COHeaderViewModel COHeaderViewModel { get; set; }

        public MenuViewModel MenuViewModel { get; set; }

        public List<SummaryDetails> SummaryDetails { get; set; }

        public string Day { get; set; }
        public List<Attributes> DayList { get; set; }

        public string Service { get; set; }
        public List<mProductType> ServiceTypeList { get; set; }

        public string KeepAs { get; set; }
        public List<AttributeValues> KeepAsList { get; set; }
        
        public string DepartureDate { get; set; }
        public List<AttributeValues> DepartureDatesList { get; set; }

        public string PaxSlab { get; set; }
        public List<AttributeValues> PaxSlabList { get; set; }

        public List<AttributeValues> RangesList { get; set; }
        public List<AttributeValues> RangeAccoList { get; set; }
        public List<AttributeValues> RangeServicesList { get; set; }
        public List<AttributeValues> RangeUnitList { get; set; }

        public mGuesstimate Guesstimate { get; set; }

        public int NextVersionId { get; set; }

        public bool IsNewVersion { get; set; }

        public bool IsStandardPrice { get; set; }

        public List<AttributeValues> SupplierList { get; set; } = new List<AttributeValues>();

        public GuesstimateViewModel()
        {
            MenuViewModel = new MenuViewModel();
            COHeaderViewModel = new COHeaderViewModel();
            SummaryDetails = new List<SummaryDetails>();
            Guesstimate = new mGuesstimate();
        }

    }
}

