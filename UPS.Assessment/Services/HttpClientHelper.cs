using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UPS.Assessment.Infrastructure.Interfaces.Services;

namespace UPS.Assessment.Services
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpClientHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HttpResponseMessage> Get(string method, string token, string baseUrl)
        {
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            var response = await httpClient.GetAsync($"/{method}");
            return response;
        }

        public async Task<HttpResponseMessage> Get(string method,string id, string token, string baseUrl)
        {
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            var response = await httpClient.GetAsync($"/{method}/{id}");
            return response;
        }
        public async Task<HttpResponseMessage> Post(string method, string body, string? token, string baseUrl)
        {
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            var response = await httpClient.PostAsync($"/{method}",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string method,string id, string body, string? token, string baseUrl)
        {
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            var response = await httpClient.PatchAsync($"/{method}/{id}",
                new StringContent(body, Encoding.UTF8, "application/json"));
            return response;
        }

        public async Task<HttpResponseMessage> Delete(string method, string id, string? token, string baseUrl)
        {
            HttpClient httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            var response = await httpClient.DeleteAsync($"/{method}/{id}");
            return response;
        }

    }
}