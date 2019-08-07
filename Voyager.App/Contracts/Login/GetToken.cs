using System;
using System.Collections.Generic;
using System.Text;

namespace Voyager.App.Contract
{
    public class TokenRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string Expiry { get; set; }
        public string Message { get; set; }
    }
}
