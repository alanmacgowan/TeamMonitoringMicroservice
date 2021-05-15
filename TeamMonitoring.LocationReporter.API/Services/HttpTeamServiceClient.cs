using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using TeamMonitoring.LocationReporter.API.Models;

namespace TeamMonitoring.LocationReporter.API.Services
{
    public class HttpTeamServiceClient : ITeamServiceClient
    {
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;

        public HttpTeamServiceClient(ILogger<HttpTeamServiceClient> logger,
                                     IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("TeamAPI");

            _logger.LogInformation("Team Service HTTP client using URL {0}", _httpClient.BaseAddress.AbsoluteUri);
        }

        public Guid GetTeamForMember(Guid memberId)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = _httpClient.GetAsync(String.Format("/members/{0}/team", memberId)).Result;

            TeamIDResponse teamIdResponse;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                teamIdResponse = JsonConvert.DeserializeObject<TeamIDResponse>(json);
                return teamIdResponse.TeamID;
            }
            else
            {
                return Guid.Empty;
            }
        }
    }

    public class TeamIDResponse
    {
        public Guid TeamID { get; set; }
    }
}