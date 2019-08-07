using System.Collections.Generic;
using VGER_WAPI_CLASSES;

namespace Voyager.App.ViewModels.Operations
{
    public class PersonalDetailsViewModel
    {
        public PersonalDetailsViewModel()
        {
            RoomAssignmentList = new List<int>();
            RoomList = new List<AttributeValues>();
            DietaryRequirementsList = new List<AttributeValues>();
            NameTitle = new List<AttributeValues>();
            SpecialAssistanceList = new List<AttributeValues>();
            SexList = new List<AttributeValues>();

        }
        public string Passenger_Id { get; set; }
        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string PassengerName_LocalLanguage { get; set; }

        public string DateOfBirth { get; set; }
        public string DateOfAnniversary { get; set; }
        public string Notes { get; set; }

        public string PassportNumber { get; set; }

        public string PassportIssued { get; set; }

        public string PassportExpiry { get; set; }

        public string VisaNumber { get; set; }
        public string Sex { get; set; }
        public bool ISTourLeader { get; set; }
        public string DietaryRequirements { get; set; }
        public List<string> SpecialAssistanceRequirements { get; set; }
        public string RoomType { get; set; }
        public string PersonType { get; set; }
        public int? RoomAssignment { get; set; }
        public string CreatedDate { get; set; }
        public string EditedDate { get; set; }
        public string CreatedUser { get; set; }
        public string EditedUser { get; set; }
        public int PassengerNumber { get; set; }
        public string BookingNumber { get; set; }
        public string status { get; set; }
        public string StatusMessage { get; set; }
        public string Age { get; set; }
        public List<AttributeValues> RoomList { get; set; }
        public List<AttributeValues> DietaryRequirementsList { get; set; }
        public List<AttributeValues> NameTitle { get; set; }
        public List<AttributeValues> SpecialAssistanceList { get; set; }
        public List<AttributeValues> SexList { get; set; }
        public List<int> RoomAssignmentList { get; set; }

    }
}
