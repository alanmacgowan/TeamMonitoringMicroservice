using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

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

        public Team GetTeam(Guid teamId)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = _httpClient.GetAsync(String.Format("/teams/{0}", teamId)).Result;

            Team teamResponse = null;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                teamResponse = JsonConvert.DeserializeObject<Team>(json);
            }
            return teamResponse;
        }

        public Member GetMember(Guid teamId, Guid memberId)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = _httpClient.GetAsync(String.Format("/teams/{0}/members/{1}", teamId, memberId)).Result;

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