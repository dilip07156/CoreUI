using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class UserDetailsViewModel
    {
        public string SalesOfficer { get; set; }
        public string CostingOfficer { get; set; }
        public string ProductAccountant { get; set; }
        public string FileHandler { get; set; }

        public List<UserDetails> SalesOfficerList { get; set; } = new List<UserDetails>();
        public List<UserDetails> CostingOfficerList { get; set; } = new List<UserDetails>();
        public List<UserDetails> ProductAccountantList { get; set; } = new List<UserDetails>();
        public List<UserDetails> FileHandlerList { get; set; } = new List<UserDetails>();

        public string ContactPerson { get; set; }
        public List<CompanyContacts> ContactPersonList { get; set; } = new List<CompanyContacts>();

        public string ContactPersonID { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
    }
}
