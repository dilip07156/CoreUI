using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Voyager.App.Contracts;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Voyager.App.Proxy;
using VGER_WAPI_CLASSES;
using VGER_WAPI_CLASSES.User;

namespace Voyager.App.Providers
{
	public class LoginProviders
	{
		private readonly IConfiguration _configuration;
		ServiceProxy serviceProxy;

		public LoginProviders()
		{
		}

		public LoginProviders(IConfiguration configuration)
		{
			_configuration = configuration;
			serviceProxy = new ServiceProxy(_configuration);
		}

		//public async Task<LoginResponse> getToken(LoginRequest objTokenRequest)
		//{
		//    var json = JsonConvert.SerializeObject(objTokenRequest);

		//    client = new HttpClient();

		//    string url = _configuration.GetValue<string>("ServiceGetToken");

		//    var content = new StringContent(json, Encoding.UTF8, "application/json");

		//    var response = await client.PostAsync(url, content);
		//    HttpResponseMessage responseMessage = await client.PostAsync(url, content);

		//    var responseJsonString = await responseMessage.Content.ReadAsStringAsync();

		//    LoginResponse objLoginResponse = new LoginResponse();
		//    try
		//    {
		//        objLoginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJsonString);
		//    }
		//    catch (Exception ex)
		//    {
		//    }
		//    return objLoginResponse;
		//}

		//public async Task<UserDetailsResponse> getUserDetails(UserDetailsRequest objUserDetailsRequest,string ticket)
		//{
		//    var json = JsonConvert.SerializeObject(objUserDetailsRequest);

		//    client = new HttpClient();

		//    string url = _configuration.GetValue<string>("ServiceGetUserDetails");

		//    var content = new StringContent(json, Encoding.UTF8, "application/json");

		//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ticket);

		//    var response = await client.PostAsync(url, content);
		//    HttpResponseMessage responseMessage = await client.PostAsync(url, content);

		//    var responseJsonString = await responseMessage.Content.ReadAsStringAsync();

		//    UserDetailsResponse objUserDetailsResponse = new UserDetailsResponse();
		//    try
		//    {
		//        objUserDetailsResponse = JsonConvert.DeserializeObject<UserDetailsResponse>(responseJsonString);
		//    }
		//    catch (Exception ex)
		//    {
		//    }
		//    return objUserDetailsResponse;
		//}

		public async Task<LoginResponse> GetToken(LoginRequest objTokenRequest)
		{
			LoginResponse objLoginResponse = new LoginResponse();
			objLoginResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGetToken"), objTokenRequest, typeof(LoginResponse));
			return objLoginResponse;
		}

        public async Task<IntegrationLoginResponse> GetIntegrationToken(IntegrationLoginRequest objTokenRequest)
        {
            IntegrationLoginResponse objLoginResponse = new IntegrationLoginResponse();
            objLoginResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceGetIntegrationToken"), objTokenRequest, typeof(IntegrationLoginResponse));
            return objLoginResponse;
        }


        #region 3rd party get Redirection request

        public async Task<List<IntegrationConfigurationInfo>> GetIntegrationRedirection(IntegrationLoginRequest objRedirectionRequest, string ticket)
        {
            List<IntegrationConfigurationInfo> objIntegrationConfig = new List<IntegrationConfigurationInfo>();
            objIntegrationConfig = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSettings:GetIntegrationRedirection"), objRedirectionRequest, typeof(List<IntegrationConfigurationInfo>), ticket);
            return objIntegrationConfig;
        }

        #endregion

        public async Task<UserDetailsResponse> GetUserDetails(UserDetailsRequest objUserDetailsRequest, string ticket)
		{
			UserDetailsResponse objUserDetailsResponse = new UserDetailsResponse();
			objUserDetailsResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetUserDetails"), objUserDetailsRequest, typeof(UserDetailsResponse), ticket);
			return objUserDetailsResponse;
		}

		public async Task<ContactDetailsResponse> GetContactDetails(ContactDetailsRequest objUserDetailsRequest, string ticket)
		{
			ContactDetailsResponse objContactDetailsResponse = new ContactDetailsResponse();
			objContactDetailsResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetContactDetails"), objUserDetailsRequest, typeof(ContactDetailsResponse), ticket);
			return objContactDetailsResponse;
		}



		public async Task<ContactDetailsResponse> UpdateContactDetails(ContactDetailsRequest objUserDetailsRequest, string ticket)
		{
			ContactDetailsResponse objContactDetailsResponse = new ContactDetailsResponse();
			objContactDetailsResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSetUserContactDetails"), objUserDetailsRequest, typeof(ContactDetailsResponse), ticket);
			return objContactDetailsResponse;
		}


		public async Task<ContactDetailsResponse> UpdateUserPassword(ContactDetailsRequest objUserDetailsRequest, string ticket)
		{
			ContactDetailsResponse objContactDetailsResponse = new ContactDetailsResponse();
			objContactDetailsResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSetUserPassword"), objUserDetailsRequest, typeof(ContactDetailsResponse), ticket);
			return objContactDetailsResponse;
		}


		public async Task<ContactDetailsResponse> UpdateUserDetails(ContactDetailsRequest objUserDetailsRequest, string ticket)
		{
			ContactDetailsResponse objContactDetailsResponse = new ContactDetailsResponse();
			objContactDetailsResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceSetUserDetails"), objUserDetailsRequest, typeof(ContactDetailsResponse), ticket);
			return objContactDetailsResponse;
		}

		public async Task<bool> UserPasswordRecover(LoginRequest objTokenRequest)
		{
			bool IsUserExist = false;
			IsUserExist = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUserPasswordRecover"), objTokenRequest, typeof(bool));
			return IsUserExist;
		}




		public async Task<UserByRoleGetRes> GetUsersByRole(UserByRoleGetReq userByRoleGetReq, string ticket)
		{
			UserByRoleGetRes userByRoleGetRes = new UserByRoleGetRes();
			userByRoleGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetUsersByRole"), userByRoleGetReq, typeof(UserByRoleGetRes), ticket);
			return userByRoleGetRes;
		}

		public async Task<UserByRoleGetRes> GetRoleIdByRoleName(UserByRoleGetReq userByRoleGetReq, string ticket)
		{
			UserByRoleGetRes userByRoleGetRes = new UserByRoleGetRes();
			userByRoleGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetRoleIdByRoleName"), userByRoleGetReq, typeof(UserByRoleGetRes), ticket);
			return userByRoleGetRes;
		}

		public async Task<UserByRoleGetRes> GetUserDetailsByRole(UserByRoleGetReq userByRoleGetReq, string ticket)
		{
			UserByRoleGetRes userByRoleGetRes = new UserByRoleGetRes();
			userByRoleGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetUserDetailsByRole"), userByRoleGetReq, typeof(UserByRoleGetRes), ticket);
			return userByRoleGetRes;
		}
		public async Task<CommonResponse> UpdateUserForQuote(UpdateUserGetReq updateUserGetReq, string ticket)
		{
			CommonResponse commonResponse = new CommonResponse();
			commonResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:UpdateUserForQuote"), updateUserGetReq, typeof(CommonResponse), ticket);
			return commonResponse;
		}

		public async Task<CommonResponse> UpdateFollowup(UpdateUserGetReq updateUserGetReq, string ticket)
		{
			CommonResponse commonResponse = new CommonResponse();
			commonResponse = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:UpdateFollowup"), updateUserGetReq, typeof(CommonResponse), ticket);
			return commonResponse;
		}

		public async Task<UserRoleGetRes> GetUserRoleDetails(UserRoleGetReq userRoleGetReq, string ticket)
		{
			UserRoleGetRes userRoleGetRes = new UserRoleGetRes();
			userRoleGetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetUserRoleDetails"), userRoleGetReq, typeof(UserRoleGetRes), ticket);
			return userRoleGetRes;
		}

		public async Task<UserRoleSetRes> SetUserRoleDetails(UserRoleSetReq userRoleSetReq, string ticket)
		{
			UserRoleSetRes userRoleSetRes = new UserRoleSetRes();
			userRoleSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:SetUserRoleDetails"), userRoleSetReq, typeof(UserRoleSetRes), ticket);
			return userRoleSetRes;
		}

		public async Task<UserSetRes> CreateUser(UserSetReq userSetReq, string ticket)
		{
			UserSetRes userSetRes = new UserSetRes();
			userSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:CreateUser"), userSetReq, typeof(UserSetRes), ticket);
			return userSetRes;
		}

		public async Task<UserSetRes> EnableDisableUser(UserSetReq userSetReq, string ticket)
		{
			UserSetRes userSetRes = new UserSetRes();
			userSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:EnableDisableUser"), userSetReq, typeof(UserSetRes), ticket);
			return userSetRes;
		}

		public async Task<UserSetRes> UpdateUser(UserSetReq userSetReq, string ticket)
		{
			UserSetRes userSetRes = new UserSetRes();
			userSetRes = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:UpdateUser"), userSetReq, typeof(UserSetRes), ticket);
			return userSetRes;
		}

		public async Task<bool> CheckExistingEmail(string emailId, string ticket)
		{
			bool result = false;
			result = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:CheckExistingEmail"), emailId, typeof(bool), ticket);
			return result;
		}
        public async Task<RoleGetRes> GetRoles(string ticket)
        {
            RoleGetRes result = new RoleGetRes();
            result = await serviceProxy.PostData(_configuration.GetValue<string>("ServiceUser:GetRoles"),null, typeof(RoleGetRes), ticket);
            return result;


        }
	}
}
