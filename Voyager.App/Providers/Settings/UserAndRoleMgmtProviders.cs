using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Voyager.App.Contracts;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Providers
{
    public class UserAndRoleMgmtProviders
	{
        private readonly IConfiguration _configuration;
        ServiceProxy serviceProxy;

        #region Initializers

        public UserAndRoleMgmtProviders()
        {
        }

        public UserAndRoleMgmtProviders(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceProxy = new ServiceProxy(_configuration);
        }

        #endregion

        #region Sales Pipeline Roles

        public async Task<SettingsGetRes> GetSalesPipelineRoles(SettingsGetReq request, string ticket)
        {
			SettingsGetRes response = new SettingsGetRes();
			response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetSalesPipelineRoles"), request, typeof(SettingsGetRes), ticket);
            return response;
        }

		public async Task<SettingsSetRes> SetSalesPipelineRoles(SettingsSetReq request, string ticket)
		{
			SettingsSetRes response = new SettingsSetRes();
			response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SetSalesPipelineRoles"), request, typeof(SettingsSetRes), ticket);
			return response;
		}
        #endregion

        #region Integration Credentials

        public async Task<Integration_Search_Response> GetIntegrationCredentials(Integration_Search_Request request, string ticket)
        {
            Integration_Search_Response response = new Integration_Search_Response();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetIntegrationCredentials"), request, typeof(Integration_Search_Response), ticket);
            return response;
        }

        public async Task<Integration_Search_Response> GetApplicationAttributes(Integration_Search_Request request, string ticket)
        {
            Integration_Search_Response response = new Integration_Search_Response();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetApplicationAttributes"), request, typeof(Integration_Search_Response), ticket);
            return response;
        }

        public async Task<bool> CheckCredentialsExit(Integration_Search_Request request, string ticket)
        {
            bool response = false;
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:CheckCredentialsExit"), request, typeof(bool), ticket);
            return response;
        }

        public async Task<Integration_Search_Response> SaveIntegrationCredentials(Integration_Search_Request request, string ticket)
        {
            Integration_Search_Response response = new Integration_Search_Response();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SaveIntegrationCredentials"), request, typeof(Integration_Search_Response), ticket);
            return response;
        }

        public async Task<Integration_Search_Response> DeleteIntegrationCredentials(Integration_Search_Request request, string ticket)
        {
            Integration_Search_Response response = new Integration_Search_Response();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:DeleteIntegrationCredentials"), request, typeof(Integration_Search_Response), ticket);
            return response;
        }

        #endregion

        #region Out Bound Integration Credentials

        public async Task<bool> CheckOutBoundConfigExit(OutBoundIntegrationCredentialsReq request, string ticket)
        {
            bool response = false;
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:CheckOutBoundConfigExit"), request, typeof(bool), ticket);
            return response;
        }

        public async Task<OutBoundIntegrationCredentialsRes> SaveOutBoundIntegrationCredentials(OutBoundIntegrationCredentialsReq request, string ticket)
        {
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SaveOutBoundIntegrationCredentials"), request, typeof(OutBoundIntegrationCredentialsRes), ticket);
            return response;
        }

        public async Task<OutBoundIntegrationCredentialsRes> GetOutBoundIntegrationCredentialsList(OutBoundIntegrationCredentialsReq request, string ticket)
        {
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetOutBoundIntegrationCredentialsList"), request, typeof(OutBoundIntegrationCredentialsRes), ticket);
            return response;
        }

        public async Task<OutBoundIntegrationCredentialsRes> DeleteOutBoundIntegrationCredentials(OutBoundIntegrationCredentialsReq request, string ticket)
        {
            OutBoundIntegrationCredentialsRes response = new OutBoundIntegrationCredentialsRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:DeleteOutBoundIntegrationCredentials"), request, typeof(OutBoundIntegrationCredentialsRes), ticket);
            return response;
        }

        #endregion

        #region Integration Application Mapping Data

        public async Task<bool> CheckApplicationMappingExists(IntegrationMappingDataReq request, string ticket)
        {
            bool response = false;
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:CheckApplicationMappingExists"), request, typeof(bool), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> SaveIntegrationApplicationMappingInfo(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SaveIntegrationApplicationMappingInfo"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> GetApplicationMappingList(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetApplicationMappingList"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> GetApplicationMappingDataList(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetApplicationMappingDataList"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        public async Task<bool> CheckApplicationMappingDataExists(IntegrationMappingDataReq request, string ticket)
        {
            bool response = false;
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:CheckApplicationMappingDataExists"), request, typeof(bool), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> SaveIntegrationApplicationMappingDataInfo(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SaveIntegrationApplicationMappingDataInfo"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> DeleteIntegrationApplicationMappingInfo(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:DeleteIntegrationApplicationMappingInfo"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        public async Task<IntegrationMappingDataRes> DeleteIntegrationApplicationMappingItemsInfo(IntegrationMappingDataReq request, string ticket)
        {
            IntegrationMappingDataRes response = new IntegrationMappingDataRes();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:DeleteIntegrationApplicationMappingItemsInfo"), request, typeof(IntegrationMappingDataRes), ticket);
            return response;
        }

        #endregion

        #region Integration Platform

        public async Task<bool> CheckPlatformExit(IntegartionPlatform_Req request, string ticket)
        {
            bool response = false;
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:CheckPlatformExit"), request, typeof(bool), ticket);
            return response;
        }

        public async Task<IntegartionPlatform_Res> SaveIntegrationPlatform(IntegartionPlatform_Req request, string ticket)
        {
            IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:SaveIntegrationPlatform"), request, typeof(IntegartionPlatform_Res), ticket);
            return response;
        }

        public async Task<IntegartionPlatform_Res> GetIntegrationPlatformList(IntegartionPlatform_Req request, string ticket)
        {
            IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetIntegrationPlatform"), request, typeof(IntegartionPlatform_Res), ticket);
            return response;
        }

        public async Task<IntegartionPlatform_Req> GetIntegrationPlatformConfigInfo(IntegartionPlatform_Req request, string ticket)
        {
            request = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetIntegrationPlatformConfigInfo"), request, typeof(IntegartionPlatform_Req), ticket);
            return request;
        }

        public async Task<IntegartionPlatform_Res> DeleteIntegrationPlatform(IntegartionPlatform_Req request, string ticket)
        {
            IntegartionPlatform_Res response = new IntegartionPlatform_Res();
            response = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:DeleteIntegrationPlatform"), request, typeof(IntegartionPlatform_Res), ticket);
            return response;
        }

        #endregion

    }
}
