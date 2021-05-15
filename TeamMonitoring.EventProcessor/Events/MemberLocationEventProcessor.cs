using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.EventProcessor.Location;

namespace TeamMonitoring.EventProcessor.Events
{
    public class MemberLocationEventProcessor : BackgroundService, IEventProcessor
    {
        protected readonly ILogger _logger;
        protected readonly IEventSubscriber<MemberLocationRecordedEvent> _eventSubscriber;
        protected readonly IEventEmitter<ProximityDetectedEvent> _eventEmitter;
        protected readonly ProximityDetector _proximityDetector;
        protected readonly ILocationCache _locationCache;

        public MemberLocationEventProcessor(ILogger<MemberLocationEventProcessor> logger,
                                            IEventSubscriber<MemberLocationRecordedEvent> eventSubscriber,
                                            IEventEmitter<ProximityDetectedEvent> eventEmitter,
                                            ILocationCache locationCache
        )
        {
            _logger = logger;
            _eventSubscriber = eventSubscriber;
            _eventEmitter = eventEmitter;
            _proximityDetector = new ProximityDetector();
            _locationCache = locationCache;

            _eventSubscriber.EventReceived += (mlre) =>
            {
                _logger.LogInformation($"MemberLocationRecordedEvent Received: {mlre.MemberID}");

                var memberLocations = _locationCache.GetMemberLocations(mlre.TeamID);
                ICollection<ProximityDetectedEvent> proximityEvents = _proximityDetector.DetectProximityEvents(mlre, memberLocations, 30.0f);
                foreach (var proximityEvent in proximityEvents)
                {
                    _eventEmitter.EmitEvent(proximityEvent);
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
            _eventSubscriber.Subscribe();
        }

        public void Stop()
        {
            _eventSubscriber.Unsubscribe();
        }
    }
}