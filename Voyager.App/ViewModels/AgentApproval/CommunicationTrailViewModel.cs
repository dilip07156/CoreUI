using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class CommunicationTrailViewModel
    { 
        public DocumentStoreInfoGetRes DocumentStoreInfoGetRes { get; set; }
        public string ModuleType { get; set; }
    } 
}