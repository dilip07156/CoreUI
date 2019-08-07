using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Voyager.App.Proxy
{
    public class ServiceProxy
    {
        HttpClient client;
        private readonly IConfiguration _configuration;

        public ServiceProxy(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ServiceBaseUrl
        {
            get
            {
                return _configuration.GetValue<string>("ServiceBaseUrl");
            }
        }

        public string DistributionServiceBaseUrl
        {
            get
            {
                return _configuration.GetValue<string>("DistributionServiceBaseUrl");
            }
        }

        public string BookingServiceBaseUrl
        {
            get
            {
                return _configuration.GetValue<string>("BookingServiceBaseUrl");
            }
        }

        public string BridgeServiceBaseUrl
        {
            get
            {
                return _configuration.GetValue<string>("BridgeServiceBaseUrl");
            }
        }

        public async Task<dynamic> GetData(string URL, Type ResponseType, string ticket)
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 20, 0, 0);

            string url = ServiceBaseUrl + URL;

            var content = new StringContent("", Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ticket);

            //var response = await client.PostAsync(url, content);
            HttpResponseMessage responseMessage = await client.PostAsync(url, content);

            var responseJsonString = await responseMessage.Content.ReadAsStringAsync();

            try
            {
                return (dynamic)JsonConvert.DeserializeObject(responseJsonString, ResponseType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> PostData(string URL, object Param, Type ResponseType)
        {
            var json = JsonConvert.SerializeObject(Param);

            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 20, 0, 0);
            string url = ServiceBaseUrl + URL;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = await client.PostAsync(url, content);
            HttpResponseMessage responseMessage = await client.PostAsync(url, content);

            var responseJsonString = await responseMessage.Content.ReadAsStringAsync();

            try
            {
                return (dynamic)JsonConvert.DeserializeObject(responseJsonString, ResponseType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetServiceData(string URL, Type ResponseType, string ticket = null, string Source = null)
        {
            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 20, 0, 0);
            string curBookingServiceBaseUrl = BookingServiceBaseUrl;

            if ((Source ?? "") == "BridgeGet")
                curBookingServiceBaseUrl = curBookingServiceBaseUrl.Replace("/api/", "/");
            string url = curBookingServiceBaseUrl + URL;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ticket);
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                var responseJsonString = await responseMessage.Content.ReadAsStringAsync();
                return (dynamic)JsonConvert.DeserializeObject(responseJsonString, ResponseType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> PostData(string URL, object Param, Type ResponseType, string ticket, string ServiceType = null)
        {
            string url = "";
            var json = JsonConvert.SerializeObject(Param);

            client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 20, 0, 0);

            if (ServiceType == "Distribution")
                url = DistributionServiceBaseUrl + URL;
            else if (ServiceType == "Bridge")
                url = BridgeServiceBaseUrl + URL;
            else
                url = ServiceBaseUrl + URL;

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ticket);

            try
            {
                HttpResponseMessage responseMessage = await client.PostAsync(url, content);
                var responseJsonString = await responseMessage.Content.ReadAsStringAsync();
                return (dynamic)JsonConvert.DeserializeObject(responseJsonString, ResponseType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
