using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager.App.Contracts
{
    public class UserDetailsResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public string ContactDisplayMessage { get; set; }
        public string Currency { get; set; }
        public string BalanceAmount { get; set; }
        public string CreditAmount { get; set; }
    }
}
