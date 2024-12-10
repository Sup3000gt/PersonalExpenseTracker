using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PersonalExpenseTrackerUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "49630d64dd954e64b06992eade60a44e");
        }

        public async Task<HttpResponseMessage> RegisterUserAsync(object user)
        {
            string json = JsonSerializer.Serialize(user);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync("https://expenseuserserviceapi.azure-api.net/api/Users/register", content);
        }

        public async Task<HttpResponseMessage> LoginUserAsync(object loginDetails)
        {
            string json = JsonSerializer.Serialize(loginDetails);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync("https://expenseuserserviceapi.azure-api.net/api/Users/login", content);
        }

        public async Task<HttpResponseMessage> GetProfileAsync(string username)
        {
            return await _httpClient.GetAsync($"https://expenseuserserviceapi.azure-api.net/api/Users/profile?username={username}");
        }
    }
}
