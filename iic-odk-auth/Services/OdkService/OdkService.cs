using System;
using System.Security.Cryptography;
using System.Text;
using iic_odk_auth.Models;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace iic_odk_auth.Services.OdkService
{
    public class OdkService : IOdkService
    {
        private readonly HttpClient httpClient;
        
        private readonly String odkAppCredentials;
        private readonly String odkHost;
        private readonly String odkProjectId;
        private readonly ILogger<OdkService> log;

        public OdkService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<OdkService> log)
        {
            httpClient = httpClientFactory.CreateClient();
            
            odkAppCredentials = config.GetValue<String>("OdkAppCredentials")!;
            odkHost = config.GetValue<String>("odkHost")!;
            odkProjectId = config.GetValue<String>("odkProjectId")!;
            this.log = log;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            var data = new
            {
                email = user.Email,
                password = user.Password,
                displayName = user.Name
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{odkHost}/v1/users");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            request.Headers.Add("Authorization", $"Basic {odkAppCredentials}");

            var response = await httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            dynamic responseJson = JsonConvert.DeserializeObject(responseBody);

            return responseJson.id;
        }

        public async Task<int> GetUserOdkIdAsync(User user)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{odkHost}/v1/users?q={user.Email}");

            request.Headers.Add("Authorization", $"Basic {odkAppCredentials}");

            var response = await httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && !responseBody.Equals("[]"))
            {
                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);

                var emailFromOdk = responseJson.First.email.Value;

                if (user.Email.Equals(emailFromOdk))
                {
                    return responseJson.First.id;
                }
            }

            return default;
        }

        public async Task AddToProjectAsync(User user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{odkHost}/v1/projects/{odkProjectId}/assignments/8/{user.OdkId}");

            request.Headers.Add("Authorization", $"Basic {odkAppCredentials}");

            var response = await httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();
        }

        public async Task<StringValues> Login(User user)
        {
            var data = new
            {
                email = user.Email,
                password = user.Password
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{odkHost}/v1/sessions");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            log.LogDebug("Session response body" + responseBody);

            var cookies = response.Headers.GetValues("Set-Cookie");

            return new StringValues(cookies.ToArray());
        }

    }
}

