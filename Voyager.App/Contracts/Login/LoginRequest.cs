using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voyager.App.Contracts
{
    public class LoginRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
