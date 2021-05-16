using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamMonitoring.ProximityMonitor.TeamService
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

        public async Task<Team> GetTeam(Guid teamId)
        {
            var response = await _httpClient.GetAsync($"/teams/{teamId}");

            Team teamResponse = null;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                teamResponse = JsonConvert.DeserializeObject<Team>(json);
            }
            return teamResponse;
        }

        public async Task<Member> GetMember(Guid teamId, Guid memberId)
        {
            var response = await _httpClient.GetAsync($"/teams/{teamId}/members/{memberId}");

            Member memberResponse = null;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                memberResponse = JsonConvert.DeserializeObject<Member>(json);
            }
            return memberResponse;
        }
    }
}