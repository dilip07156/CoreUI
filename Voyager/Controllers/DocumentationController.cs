using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Voyager.Controllers
{
    [Authorize]
    public class DocumentationController : VoyagerController
    {
        private readonly IConfiguration _configuration;


        public DocumentationController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;

        }
        public IActionResult GetDocumentation()
        {
            if (UserRoles.Contains("Agent_Manuals"))
            {
                ViewBag.DocLoc = _configuration.GetValue<string>("Documentation");
                return View();
            }
            else
            {

                string page = _configuration.GetValue<string>("Pages:UnauthorizePage");
                return View(page);
            }

        }
    }
}