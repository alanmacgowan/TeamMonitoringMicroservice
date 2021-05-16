using System;
using System.Collections.Generic;
using System.Linq;
using TeamMonitoring.EventProcessor.Location;
using TeamMonitoring.Events;

namespace TeamMonitoring.EventProcessor.Processor
{
    public class ProximityDetector
    {
        public ICollection<ProximityDetectedEvent> DetectProximityEvents(MemberLocationRecordedEvent memberLocationEvent,
                                                                         ICollection<MemberLocation> memberLocations,
                                                                         double distanceThreshold)
        {
            var gpsUtility = new GpsUtility();
            var sourceCoordinate = new GpsCoordinate()
            {
                Latitude = memberLocationEvent.Latitude,
                Longitude = memberLocationEvent.Longitude
            };

            return memberLocations.Where(
                      ml => ml.MemberID != memberLocationEvent.MemberID &&
                      gpsUtility.DistanceBetweenPoints(sourceCoordinate, ml.Location) < distanceThreshold)
                .Select(ml =>
                {
                    return new ProximityDetectedEvent()
                    {
                        SourceMemberID = memberLocationEvent.MemberID,
                        TargetMemberID = ml.MemberID,
                        TeamID = memberLocationEvent.TeamID,
                        DetectionTime = DateTime.UtcNow.Ticks,
                        SourceMemberLocation = sourceCoordinate,
                        TargetMemberLocation = ml.Location,
                        MemberDistance = gpsUtility.DistanceBetweenPoints(sourceCoordinate, ml.Location)
                    };
                }).ToList();
        }
    }
}