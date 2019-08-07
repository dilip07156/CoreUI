using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Voyager.App.ViewModels;
using Voyager.Models;

namespace Voyager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 

        [HttpGet]
        public ActionResult Quote()
        {
            QuoteViewModel quote = new QuoteViewModel();
            quote.ApproxPaxAdult = 1;
            quote.Duration = 1;
            return View(quote);
        }

        [HttpPost]
        public ActionResult Quote(QuoteViewModel quote)
        {
            return View();
        }
    }
}
