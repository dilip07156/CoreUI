using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGER_WAPI_CLASSES;
using Voyager.App.Proxy;

namespace Voyager.App.Providers
{
    public class HandoverProviders
    {
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers
        public HandoverProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }
        #endregion

        #region AttachToMaster
        public async Task<GoAheadGetRes> GetGoAhead(GoAheadGetReq goAheadGetReq, string ticket)
        {
            GoAheadGetRes goAheadGetRes = new GoAheadGetRes();
            goAheadGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:GetGoAhead"), goAheadGetReq, typeof(GoAheadGetRes), ticket);
            return goAheadGetRes;
        }

        public async Task<GetGoAheadDepatureRes> GetGoAheadDepature(GoAheadGetReq goAheadGetReq, string ticket)
        {
            GetGoAheadDepatureRes goAheadGetRes = new GetGoAheadDepatureRes();
            goAheadGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:GetGoAheadDepature"), goAheadGetReq, typeof(GetGoAheadDepatureRes), ticket);
            return goAheadGetRes;
        }

        public async Task<GoAheadSetRes> SetGoAhead(GoAheadSetReq goAheadSetReq, string ticket)
        {
            GoAheadSetRes goAheadSetRes = new GoAheadSetRes();
            goAheadSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:SetGoAhead"), goAheadSetReq, typeof(GoAheadSetRes), ticket);
            return goAheadSetRes;
        }

        public async Task<SetMaterialisationRes> SetMaterialisation(SetMaterialisationReq setMaterialisationReq, string ticket)
        {
            SetMaterialisationRes objSetMaterialisationRes = new SetMaterialisationRes();
            objSetMaterialisationRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:SetMaterialisation"), setMaterialisationReq, typeof(SetMaterialisationRes), ticket);
            return objSetMaterialisationRes;
        }

        public async Task<GoAheadSetRes> SendMailHandoverBooking(GoAheadGetReq goAheadGetReq, string ticket)
        {
            GoAheadSetRes objGoAheadSetRes = new GoAheadSetRes();
            objGoAheadSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:SendMailHandoverBooking"), goAheadGetReq, typeof(GoAheadSetRes), ticket);
            return objGoAheadSetRes;
        }
        #endregion

        #region Handover
        public async Task<ConfirmBookingSetRes> SetGoAheadConfirmMessage(ConfirmBookingSetReq confirmBookingSetReq, string ticket)
        {
            ConfirmBookingSetRes response = new ConfirmBookingSetRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:SetGoAheadConfirmMessage"), confirmBookingSetReq, typeof(ConfirmBookingSetRes), ticket);
            return response;
        }

        public async Task<ConfirmBookingGetRes> GoAheadQuotes(ConfirmBookingGetReq confirmBookingGetReq, string ticket)
        {
            ConfirmBookingGetRes response = new ConfirmBookingGetRes();
            response= await serviceProxy.PostData(_configuration.GetValue<string>("Handover:GoAheadQuotes"), confirmBookingGetReq, typeof(ConfirmBookingGetRes), ticket);
            return response;
        }

        public async Task<HandoverGetRes> GetGoAheadDeparturesDetails(GoAheadGetReq goAheadGetReq, string ticket)
        {
            HandoverGetRes response = new HandoverGetRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:GetGoAheadDeparturesDetails"), goAheadGetReq, typeof(HandoverGetRes), ticket);
            return response;
        }
        #endregion

        #region Add New Departures
        public async Task<GoAheadNewDeptGetRes> GetGoAheadExistDepartures(GoAheadGetReq goAheadGetReq, string ticket)
        {
            GoAheadNewDeptGetRes objGoAheadNewDeptGetRes = new GoAheadNewDeptGetRes();
            objGoAheadNewDeptGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:GetGoAheadExistDepartures"), goAheadGetReq, typeof(GoAheadNewDeptGetRes), ticket);
            return objGoAheadNewDeptGetRes;
        }

        public async Task<GoAheadNewDeptSetRes> SetGoAheadNewDepartures(GoAheadNewDeptSetReq goAheadNewDeptSetReq, string ticket)
        {
            GoAheadNewDeptSetRes objGoAheadNewDeptSetRes = new GoAheadNewDeptSetRes();
            objGoAheadNewDeptSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("Handover:SetGoAheadNewDepartures"), goAheadNewDeptSetReq, typeof(GoAheadNewDeptSetRes), ticket);
            return objGoAheadNewDeptSetRes;
        }
        #endregion
    }
}