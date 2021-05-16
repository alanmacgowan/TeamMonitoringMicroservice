using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<Guid> GetTeamForMember(Guid memberId)
        {
            var response = await _httpClient.GetAsync($"/members/{memberId}/team");

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