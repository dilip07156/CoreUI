using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class IntegrationApplicationMappingViewModel
    {
        public IntegrationApplicationMappingViewModel()
        {
            ApplicationList = new List<Attributes>();

            ResponseStatus = new ResponseStatus();
        }

        public string Application { get; set; }
        public string ApplicationName { get; set; }

        public List<Attributes> ApplicationList { get; set; }

        //Search on Entity level
        public string IntegrationApplicationMapping_Id { get; set; }
        public string Type { get; set; }
        public string Entity { get; set; }



        //Search on item level
        public string IntegrationApplicationMappingItem_Id { get; set; }
        public string PartnerEntityName { get; set; }//3rd party Name
        public string PartnerEntityCode { get; set; }//3rd party Value/Code
        public string SystemEntityName { get; set; }//System Name
        public string SystemEntityCode { get; set; }//System value/Code

        //Save Application Mapping Items


        public ResponseStatus ResponseStatus { get; set; }
    }
}
