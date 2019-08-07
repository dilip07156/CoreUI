using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    /// <summary>
    /// ViewModel for Proposal Document
    /// </summary>
    public class ProposalDocumentViewModel
    {
        /// <summary>
        /// QRF ID
        /// </summary>
        public string QRFID { get; set; }

        /// <summary>
        /// URL Initials like http://localhost/
        /// </summary>
        public string URLinitial { get; set; }

        /// <summary>
        /// Country image initial like http://localhost/
        /// </summary>
        public string CountryImageInitial { get; set; }

        /// <summary>
        /// Small Map URL for Map of region
        /// </summary>
        public string SmallMapURL { get; set; }

        /// <summary>
        /// full cities Map URL for Map of region
        /// </summary>
        public string BigMapURL { get; set; }

        /// <summary>
        /// QRF Form Details
        /// </summary>
        public mQRFPrice QRFQuote { get; set; } = new mQRFPrice();

        /// <summary>
        /// Proposal Details
        /// </summary>
        public mProposal Proposal { get; set; } = new mProposal();

        /// <summary>
        /// Itenerary Details
        /// </summary>
        public mItinerary Itinerary { get; set; } = new mItinerary();

        /// <summary>
        /// View Model for Menu
        /// </summary>
        public MenuViewModel MenuViewModel { get; set; } = new MenuViewModel();

        /// <summary>
        /// Routing Cities from QRF
        /// </summary>
        public List<string> RoutingCities { get; set; } = new List<string>();

        /// <summary>
        /// Product Images for Countries etc
        /// </summary>
        public List<Images> ProductImages { get; set; } = new List<Images>();

        /// <summary>
        /// Product and Hotel images from Generic Images Collection
        /// </summary>
        public List<mGenericImages> GenericImages { get; set; }

        /// <summary>
        /// Departure dates list
        /// </summary>
        public List<ProposalDepartDate> DatesList { get; set; } = new List<ProposalDepartDate>();

        /// <summary>
        /// ViewModel for Header and Footer
        /// </summary>
        public COHeaderViewModel COHeaderViewModel { get; set; } = new COHeaderViewModel();

        /// <summary>
        /// Hotel Summary List
        /// </summary>
        public List<Hotel> HotelList { get; set; } = new List<Hotel>();
    }
    
    /// <summary>
    /// Departure Date details
    /// </summary>
    public class ProposalDepartDate
    {
        /// <summary>
        /// Departure Date Day
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Departure Date month
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Departure Date Year
        /// </summary>
        public string Year { get; set; }
    }

}
