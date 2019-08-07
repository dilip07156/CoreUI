using System;
using System.Collections.Generic;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels
{
    public class FollowUpViewModel
    {
        public string QRFID { get; set; }

        public string FollowUpType { get; set; }

        public List<AttributeValues> FollowUpTaskList { get; set; } = new List<AttributeValues>();

        public string FollowUpTask { get; set; }

        public List<CompanyContacts> InternalUserList { get; set; } = new List<CompanyContacts>();

        public string InternalUser { get; set; }

        public string InternalUserName { get; set; }

        public List<CompanyContacts> ExternalUserList { get; set; } = new List<CompanyContacts>();

        public string ExternalUser { get; set; }

        public string ExternalUserName { get; set; }

        public string Notes { get; set; }
        
        public DateTime FollowUpDate { get; set; }

        public string FollowUpTime { get; set; }

        public List<FollowUp> FollowUp { get; set; } = new List<FollowUp>();
    }
}
