using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class CommunicationTrailProviders
    {
        #region Declaration
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;
        #endregion

        #region Initializers
        public CommunicationTrailProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region Communication Trail
        public async Task<DocumentStoreInfoGetRes> GetCommunicationTrail(DocumentStoreGetReq request, string ticket)
        {
            DocumentStoreInfoGetRes documentStoreInfoGetRes = new DocumentStoreInfoGetRes();
            documentStoreInfoGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("DocumentStore:GetCommunicationTrail"), request, typeof(DocumentStoreInfoGetRes), ticket);
            return documentStoreInfoGetRes;
        }

        public async Task<DocumentStoreInfo> GetCommunicationTrailById(DocumentStoreGetReq request, string ticket)
        {
            DocumentStoreInfo documentStoreInfo = new DocumentStoreInfo();
            documentStoreInfo = await serviceProxy.PostData(_configuration.GetValue<string>("DocumentStore:GetCommunicationTrailById"), request, typeof(DocumentStoreInfo), ticket);
            return documentStoreInfo;
        }

        //public async Task<MailSetRes> SetEmailDetails(MailSetReq request, string ticket)
        //{
        //    MailSetRes mailSetRes = new MailSetRes();
        //    mailSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceCostsheet:SetEmailDetails"), request, typeof(MailSetRes), ticket);
        //    return mailSetRes;
        //}
        #endregion
    }
}

