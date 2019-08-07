using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Voyager.App.ViewModels;
using Voyager.App.Models;
using Voyager.App.Providers;
using Voyager.App.Contracts;
using Voyager.App.Library;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VGER_WAPI_CLASSES;
using Voyager.App.Mappers;

namespace Voyager.Controllers
{
    [Authorize]
    public class IntercityTransferController : VoyagerController
    {
        private readonly IConfiguration _configuration;

        public IntercityTransferController(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IActionResult IntercityTransfer()
        {
            try
            {
                string QRFID = Request.Query["QRFId"].ToString();
                IntercityTransferViewModel model = new IntercityTransferViewModel { QRFID = QRFID };
                model.MenuViewModel.QRFID = QRFID;

                SalesQuoteLibrary quoteLibrary = new SalesQuoteLibrary(_configuration);
                NewQuoteViewModel modelQuote = new NewQuoteViewModel { QRFID = model.MenuViewModel.QRFID };
                bool GetStatus = quoteLibrary.GetQRFAgentByQRFId(_configuration, token, ref modelQuote);
                if (GetStatus) model.TourInfoHeaderViewModel = modelQuote.TourInfoHeaderViewModel;
                
                return View(model);
            }
            catch (Exception)
            {
                throw;
                return View();
            }
        }

        [HttpPost]
        public IActionResult IntercityTransfer(IntercityTransferViewModel model)
        {
            return RedirectToAction("IntercityTransfer", new { model.QRFID });
        }
    }
}