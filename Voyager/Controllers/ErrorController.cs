using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Voyager.Models;

namespace Voyager.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        public IActionResult ErrorCode(int errCode)
        {  
            ErrorViewModel model = new ErrorViewModel();
            string statusmessage = "";
            switch (errCode)
            {
                case 400:
                    statusmessage = "Bad request: The request cannot be fulfilled due to bad syntax";
                    break;
                case 403:
                    statusmessage = "Forbidden";
                    break;
                case 404:
                    statusmessage = "Page not found";
                    break;
                case 408:
                    statusmessage = "The server timed out waiting for the request";
                    break;
                case 500:
                    statusmessage = "Internal Server Error - server was unable to finish processing the request";
                    break;
                default:
                    statusmessage = "That’s odd... Something we didn't expect happened";
                    break;

            }
            model.ErrorCode = Convert.ToString(errCode);
            model.ErrorMsg = statusmessage;
            return View("ErrorCode", model);
        }
    }
}