using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager.App.Contracts
{
    public class LoginResponse
    {
        public string Token {get; set;}
        public string Expiry { get; set; }
        public string Message { get; set; }
    }
}
