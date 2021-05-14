using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamMonitoring.ProximityMonitor.Queues;
using TeamMonitoring.ProximityMonitor.Realtime;
//using TeamMonitoring.ProximityMonitor.Realtime;
using TeamMonitoring.ProximityMonitor.TeamService;

namespace TeamMonitoring.ProximityMonitor.Events
{
    public class ProximityDetectedEventProcessor : BackgroundService, IEventProcessor
    {
        private ILogger _logger;
        //private IRealtimePublisher publisher;
        private ITeamServiceClient _teamClient;
        private IEventSubscriber _subscriber;
        private IHubContext<TeamMonitoringHub, ITeamMonitoringHub> _monitoringHub;
        //private PubnubOptions pubnubOptions;

        public ProximityDetectedEventProcessor(
            ILogger<ProximityDetectedEventProcessor> logger,
            //IRealtimePublisher publisher,
            IEventSubscriber subscriber,
            ITeamServiceClient teamClient,
            IHubContext<TeamMonitoringHub, ITeamMonitoringHub> monitoringHub)
        //,IOptions<PubnubOptions> pubnubOptions)
        {
            _monitoringHub = monitoringHub;
            _logger = logger;
            //this.pubnubOptions = pubnubOptions.Value;
            //this.publisher = publisher;
            _subscriber = subscriber;
            _teamClient = teamClient;
            _logger.LogInformation("Created Proximity Event Processor.");

            _subscriber.ProximityDetectedEventReceived += async (pde) =>
            {
                Team t = _teamClient.GetTeam(pde.TeamID);
                Member sourceMember = _teamClient.GetMember(pde.TeamID, pde.SourceMemberID);
                Member targetMember = _teamClient.GetMember(pde.TeamID, pde.TargetMemberID);

                ProximityDetectedRealtimeEvent outEvent = new ProximityDetectedRealtimeEvent
                {
                    TargetMemberID = pde.TargetMemberID,
                    SourceMemberID = pde.SourceMemberID,
                    DetectionTime = pde.DetectionTime,
                    SourceMemberLocation = pde.SourceMemberLocation,
                    TargetMemberLocation = pde.TargetMemberLocation,
                    MemberDistance = pde.MemberDistance,
                    TeamID = pde.TeamID,
                    TeamName = t.Name,
                    SourceMemberName = $"{sourceMember.FirstName} {sourceMember.LastName}",
                    TargetMemberName = $"{targetMember.FirstName} {targetMember.LastName}"
                };
                await _monitoringHub.Clients.All.ProximityDetectedNotification(outEvent);
            };

            Start();
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