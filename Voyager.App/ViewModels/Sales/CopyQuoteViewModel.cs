using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class CopyQuoteViewModel
    {
        public string QRFID { get; set; }
        [Required]
        public string TourName { get; set; }
        public string AgentId { get; set; }
        [Required]
        public string AgentName { get; set; }

        [Display(Name = "Mobile")]
        public string MobileNo { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        public List<ContactProperties> ContactPersonList { get; set; }

        public List<ExisitingDepatures> ExisitingDepatures { get; set; } = new List<ExisitingDepatures>(); 

        public List<CopyQuoteDeparturesNew> CopyQuoteDepartures { get; set; } = new List<CopyQuoteDeparturesNew>();
    }

    public class CopyQuoteDeparturesNew
    {  
        public string NewDepartureDate { get; set; }
        public long DepartureId { get; set; }
    }
}
