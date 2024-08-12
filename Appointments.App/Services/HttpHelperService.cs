using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Text.Json.JsonSerializer;

namespace Appointments.App.Services
{
    public class HttpHelperService : IHttpHelperService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpHelperService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<T> GetAsync<T>(string client, string method)
        {
            var httpClient = _httpClientFactory.CreateClient(client);

            AddHeaders(httpClient);

            var response = await httpClient.GetStringAsync(method);

            return JsonConvert.DeserializeObject<T>(response);
        }

        public async Task<T> PostAsync<T>(string client, string method, object body)
        {

            using (StringContent jsonContent = new StringContent(Serialize(body), System.Text.Encoding.UTF8, "application/json"))
            {

                var httpClient = _httpClientFactory.CreateClient(client);

                AddHeaders(httpClient);

                HttpResponseMessage res = await httpClient.PostAsync(method, jsonContent);

                var response = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(response);
            }
        }

        public async Task<T> PutAsync<T>(string client, string method, object body)
        {
            using (StringContent jsonContent = new StringContent(Serialize(body), System.Text.Encoding.UTF8, "application/json"))
            {

                var httpClient = _httpClientFactory.CreateClient(client);

                AddHeaders(httpClient);

                HttpResponseMessage res = await httpClient.PutAsync(method, jsonContent);

                var response = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(response);
            }
        } 

        private static void AddHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("x-timestamp", DateTime.Now.ToString("YYYYMMddHHmmss"));
            httpClient.DefaultRequestHeaders.Add("x-trackingId", Guid.NewGuid().ToString());
            httpClient.DefaultRequestHeaders.Add("x-appId", "1");
        }
    }
}
