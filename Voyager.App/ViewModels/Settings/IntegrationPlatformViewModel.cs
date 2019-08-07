using System;
using System.Collections.Generic;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class IntegrationPlatformViewModel
    {
        public IntegrationPlatformViewModel()
        {
            ApplicationList = new List<Attributes>();
            ModuleList = new List<AttributeValues>();
            ActionList = new List<AttributeValues>();
            AppFieldList = new List<AttributeValues>();
            SystemFieldList = new List<AttributeValues>();
            Configurations = new List<IntegrationConfigurationInfo>();
            Type = new List<string>();
            ResponseStatus = new ResponseStatus();
        }

        public string Application_Id { get; set; }
        public string ApplicationName { get; set; }
        public string Module { get; set; }
        public string ModuleName { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string SystemFieldId { get; set; }
        public string SystemFieldName { get; set; }
        public string BoundType { get; set; }

        public string URL { get; set; }
        public string ApplicationFieldName { get; set; }
        public List<IntegrationConfigurationInfo> Configurations { get; set; }

        public List<Attributes> ApplicationList { get; set; }
        public List<AttributeValues> ModuleList { get; set; }
        public List<AttributeValues> ActionList { get; set; }
        public List<AttributeValues> AppFieldList { get; set; }
        public List<AttributeValues> SystemFieldList { get; set; }
        public List<string> Type { get; set; }
        public string TypeName { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
