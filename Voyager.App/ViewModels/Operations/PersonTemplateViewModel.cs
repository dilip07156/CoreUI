using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.ViewModels.Operations
{
  public class PersonTemplateViewModel
    {
        public int Pax { get; set; }
        public int Room { get; set; }
        public string RoomType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName_NonEnglish { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string PassportNumber { get; set; }
        public string DateOfIssue { get; set; }
        public string DateofExpiry { get; set; }
        public string VisaNumber { get; set; }
        public string Comment { get; set; }
        public string Age { get; set; }
    }
}
