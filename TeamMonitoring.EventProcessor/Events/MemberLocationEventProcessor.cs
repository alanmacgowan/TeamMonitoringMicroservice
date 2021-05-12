using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.EventProcessor.Queues;

namespace TeamMonitoring.EventProcessor.Events
{
    public class MemberLocationEventProcessor : BackgroundService, IEventProcessor
    {
        private ILogger _logger;
        private IEventSubscriber subscriber;
        private IEventEmitter eventEmitter;
        private ProximityDetector proximityDetector;
        private ILocationCache locationCache;

        public MemberLocationEventProcessor(
            ILogger<MemberLocationEventProcessor> logger,
            IEventSubscriber eventSubscriber,
            IEventEmitter eventEmitter,
            ILocationCache locationCache
        )
        {
            _logger = logger;
            this.subscriber = eventSubscriber;
            this.eventEmitter = eventEmitter;
            this.proximityDetector = new ProximityDetector();
            this.locationCache = locationCache;

            this.subscriber.MemberLocationRecordedEventReceived += (mlre) =>
            {
                _logger.LogInformation($"MemberLocationRecordedEvent Received: {mlre.MemberID}");

                var memberLocations = locationCache.GetMemberLocations(mlre.TeamID);
                ICollection<ProximityDetectedEvent> proximityEvents = proximityDetector.DetectProximityEvents(mlre, memberLocations, 30.0f);
                foreach (var proximityEvent in proximityEvents)
                {
                    eventEmitter.EmitProximityDetectedEvent(proximityEvent);
                }

                locationCache.Put(mlre.TeamID, new MemberLocation
                {
                    MemberID = mlre.MemberID,
                    Location = new GpsCoordinate
                    {
                        Latitude = mlre.Latitude,
                        Longitude = mlre.Longitude
                    }
                });
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
            this.subscriber.Subscribe();
        }

        public void Stop()
        {
            this.subscriber.Unsubscribe();
        }
    }
}