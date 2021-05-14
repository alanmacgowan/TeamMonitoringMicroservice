using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.EventProcessor.Queues;

namespace TeamMonitoring.EventProcessor.Events
{
    public class MemberLocationEventProcessor : BackgroundService, IEventProcessor
    {
        private ILogger _logger;
        private IEventSubscriber _subscriber;
        private IEventEmitter _eventEmitter;
        private ProximityDetector _proximityDetector;
        private ILocationCache _locationCache;

        public MemberLocationEventProcessor(
            ILogger<MemberLocationEventProcessor> logger,
            IEventSubscriber eventSubscriber,
            IEventEmitter eventEmitter,
            ILocationCache locationCache
        )
        {
            _logger = logger;
            _subscriber = eventSubscriber;
            _eventEmitter = eventEmitter;
            _proximityDetector = new ProximityDetector();
            _locationCache = locationCache;

            _subscriber.MemberLocationRecordedEventReceived += (mlre) =>
            {
                _logger.LogInformation($"MemberLocationRecordedEvent Received: {mlre.MemberID}");

                var memberLocations = _locationCache.GetMemberLocations(mlre.TeamID);
                ICollection<ProximityDetectedEvent> proximityEvents = _proximityDetector.DetectProximityEvents(mlre, memberLocations, 30.0f);
                foreach (var proximityEvent in proximityEvents)
                {
                    _eventEmitter.EmitProximityDetectedEvent(proximityEvent);
                }

                _locationCache.Put(mlre.TeamID, new MemberLocation
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
            _subscriber.Subscribe();
        }

        public void Stop()
        {
            _subscriber.Unsubscribe();
        }
    }
}