using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class MISModuleViewModel
    {
        public string ActiveMenu { get; set; }
        public string ActiveMenuUrl { get; set; }
        public List<MISMappingDetails> MISMappingList = new List<MISMappingDetails>();
    }

    #region Sales Dashboard
    public class SalesDashboardFilters
    {
        public string SalesOfficeID { get; set; }
        public string SalesOffice { get; set; }
        public List<ChildrenCompanies> SalesOfficeList { get; set; } = new List<ChildrenCompanies>();

        public string SalesPersonID { get; set; }
        public string SalesPerson { get; set; }
        public List<AttributeValues> SalesPersonList { get; set; } = new List<AttributeValues>();

        public string DestinationID { get; set; }
        public string Destination { get; set; }
        public List<AttributeValues> DestinationList { get; set; } = new List<AttributeValues>();

        public string AgentID { get; set; }
        public string Agent { get; set; }
        public List<AttributeValues> AgentNameList { get; set; } = new List<AttributeValues>();

        public MISMappingRes MISMapping { get; set; } = new MISMappingRes();
    }

    public class SalesDashboardViewModel
    {
        public SalesDashboardFilters SalesDashboardFilters { get; set; } = new SalesDashboardFilters();
        public SalesDashboardSummary SalesDashboardSummary { get; set; } = new SalesDashboardSummary();
    }
    #endregion

    #region Bookings Dashboard
    //public class BookingsDashboardViewModel
    //{
    //    public List<string> FinancialYearMonths { get; set; }
    //    public List<PassengerForecastGraph> BookingVolumeGraph { get; set; }
    //    public List<PassengerForecastGrid> BookingVolumeGrid { get; set; }
    //    //public List<PassengerForecastGraph> PassengerVolumeGraph { get; set; }
    //    //public List<PassengerForecastGrid> PassengerVolumeGrid { get; set; }
    //    //public List<PassengerForecastGraph> BookingRevenueGraph { get; set; }
    //    //public List<PassengerForecastGrid> BookingRevenueGrid { get; set; }
    //}
    #endregion
}
