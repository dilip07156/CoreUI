using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using System.Threading.Tasks;

namespace Voyager.App.Providers
{
    public class AccomodationProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public AccomodationProviders()
        {
        }

        public AccomodationProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion
        
        //public async Task<AccomodationGetRes> GetAccomodationByQRFID(AccomodationGetReq accomodationGetReq, string ticket)
        //{
        //    AccomodationGetRes accomodationGetRes = new AccomodationGetRes();
        //    accomodationGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAccomodation:ServiceGetAccomodationByQRFID"), accomodationGetReq, typeof(AccomodationGetRes), ticket);
        //    return accomodationGetRes;
        //}

        //public async Task<AccomodationSetRes> InsertUpdateAccomodation(AccomodationSetReq accomodationSetReq, string ticket)
        //{
        //    AccomodationSetRes accomodationSetRes = new AccomodationSetRes();
        //    accomodationSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAccomodation:ServiceInsertUpdateAccomodation"), accomodationSetReq, typeof(AccomodationSetRes), ticket);
        //    return accomodationSetRes;
        //}

        //public async Task<AccomodationGetRoomTypeAndSuppRes> GetRoomTypeAndSupplement(ProdCategoryRangeGetReq prodCategoryRangeGetReq, string ticket)
        //{
        //    AccomodationGetRoomTypeAndSuppRes accomodationGetRoomTypeAndSuppRes = new AccomodationGetRoomTypeAndSuppRes();
        //    accomodationGetRoomTypeAndSuppRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceAccomodation:GetAccomodationRoomAndSupplement"), prodCategoryRangeGetReq, typeof(AccomodationGetRoomTypeAndSuppRes), ticket);
        //    return accomodationGetRoomTypeAndSuppRes;
        //}
    }
}
