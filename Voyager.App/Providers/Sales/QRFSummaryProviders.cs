using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class QRFSummaryProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public QRFSummaryProviders()
        {
        }

        public QRFSummaryProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        public async Task<QRFSummaryGetRes> GetQRFSummary(QRFSummaryGetReq qrfSummaryGetReq, string ticket)
        {
            QRFSummaryGetRes qrfSummaryGetRes = new QRFSummaryGetRes();
            qrfSummaryGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQRFSummary:GetQRFSummary"), qrfSummaryGetReq, typeof(QRFSummaryGetRes), ticket);
            return qrfSummaryGetRes;
        }

        public async Task<QuoteSetRes> UpdateQuoteDetails(QuoteSetReq objQuoteSetReq, string ticket)
        {
            QuoteSetRes objQuoteSetRes = new QuoteSetRes();
            objQuoteSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceQRFSummary:SubmitQuote"), objQuoteSetReq, typeof(QuoteSetRes), ticket);
            return objQuoteSetRes;
        }
    }
}
