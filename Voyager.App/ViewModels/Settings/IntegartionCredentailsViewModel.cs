using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class IntegartionCredentailsViewModel
    {
        public IntegartionCredentailsViewModel()
        {
            Integration_Search_DataList = new List<Integration_Search_Data>();
            ResponseStatus = new ResponseStatus();
        }
        //public CompanyOfficerGetRes officerGetRes { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonID { get; set; }
        public List<CompanyContacts> ContactPersonList { get; set; } = new List<CompanyContacts>();
        public string Application_Id { get; set; }
        public List<Attributes> ApplicationList { get; set; } = new List<Attributes>();
        public List<Integration_Search_Data> Integration_Search_DataList { get; set; }
        public bool isSearch { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
