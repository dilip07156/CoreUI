using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class OutBoundIntegartionCredentailsViewModel
    {
        public OutBoundIntegartionCredentailsViewModel()
        {
            ApplicationList = new List<Attributes>();
            ResponseStatus = new ResponseStatus();
        }

        public string Application_Id { get; set; }
        public string ConfigId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string EncryptedValue { get; set; }

        public List<Attributes> ApplicationList { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
