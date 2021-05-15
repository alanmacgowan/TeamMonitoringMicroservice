using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.ProximityMonitor.Realtime;
using TeamMonitoring.ProximityMonitor.TeamService;

namespace TeamMonitoring.ProximityMonitor.Events
{
    public class ProximityDetectedEventProcessor : BackgroundService, IEventProcessor
    {
        private ILogger _logger;
        private ITeamServiceClient _teamClient;
        private IEventSubscriber<ProximityDetectedEvent> _subscriber;
        private IHubContext<TeamMonitoringHub, ITeamMonitoringHub> _monitoringHub;

        public ProximityDetectedEventProcessor(ILogger<ProximityDetectedEventProcessor> logger,
                                               IEventSubscriber<ProximityDetectedEvent> subscriber,
                                               ITeamServiceClient teamClient,
                                               IHubContext<TeamMonitoringHub, ITeamMonitoringHub> monitoringHub)
        {
            _monitoringHub = monitoringHub;
            _logger = logger;
            _subscriber = subscriber;
            _teamClient = teamClient;
            _logger.LogInformation("Created Proximity Event Processor.");

            _subscriber.EventReceived += async (pde) =>
            {
                await NotifyProximityDetectedRealtimeEvent(pde);
            };

            Start();
        }

        private async Task NotifyProximityDetectedRealtimeEvent(ProximityDetectedEvent pde)
        {
            var team = _teamClient.GetTeam(pde.TeamID);
            var sourceMember = _teamClient.GetMember(pde.TeamID, pde.SourceMemberID);
            var targetMember = _teamClient.GetMember(pde.TeamID, pde.TargetMemberID);

            var outEvent = new ProximityDetectedRealtimeEvent
            {
                TargetMemberID = pde.TargetMemberID,
                SourceMemberID = pde.SourceMemberID,
                DetectionTime = pde.DetectionTime,
                SourceMemberLocation = pde.SourceMemberLocation,
                TargetMemberLocation = pde.TargetMemberLocation,
                MemberDistance = pde.MemberDistance,
                TeamID = pde.TeamID,
                TeamName = team.Name,
                SourceMemberName = $"{sourceMember.FirstName} {sourceMember.LastName}",
                TargetMemberName = $"{targetMember.FirstName} {targetMember.LastName}"
            };
            await _monitoringHub.Clients.All.ProximityDetectedNotification(outEvent);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void Start()
        {
            _subscriber.Subscribe();
        }

        public void Stop()
        {
            _subscriber.Unsubscribe();
        }
    }
}