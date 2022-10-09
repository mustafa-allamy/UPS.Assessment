using System.Net.Http;
using System.Threading.Tasks;

namespace UPS.Assessment.Infrastructure.Interfaces.Services
{
    public interface IHttpClientHelper
    {
        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="method">Method name and query parameters if needed</param>
        /// <param name="token">OAuth2 token</param>
        /// <param name="baseUrl">API base url</param>
        /// <returns>Return HttpResponseMessage</returns>
        Task<HttpResponseMessage> Get(string method, string token, string baseUrl);
        Task<HttpResponseMessage> Post(string method, string body, string? token, string baseUrl);
        Task<HttpResponseMessage> Patch(string method, string id, string body, string? token, string baseUrl);
        Task<HttpResponseMessage> Delete(string method, string id, string? token, string baseUrl);
        Task<HttpResponseMessage> Get(string method, string id, string token, string baseUrl);
    }
}