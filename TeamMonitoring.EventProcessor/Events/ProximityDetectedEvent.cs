using Newtonsoft.Json;
using System;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.EventProcessor.Location;

namespace TeamMonitoring.EventProcessor.Events
{
    public class ProximityDetectedEvent : IEvent<ProximityDetectedEvent>
    {
        public Guid SourceMemberID { get; set; }
        public Guid TargetMemberID { get; set; }
        public long DetectionTime { get; set; }
        public Guid TeamID { get; set; }
        public GpsCoordinate SourceMemberLocation { get; set; }
        public GpsCoordinate TargetMemberLocation { get; set; }
        public double MemberDistance { get; set; }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public ProximityDetectedEvent FromJson(string jsonBody)
        {
            return JsonConvert.DeserializeObject<ProximityDetectedEvent>(jsonBody);
        }
    }
}