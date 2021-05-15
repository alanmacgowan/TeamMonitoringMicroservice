using Newtonsoft.Json;
using System;
using TeamMonitoring.Common.Queues;

namespace TeamMonitoring.LocationReporter.API.Events
{
    public class MemberLocationRecordedEvent : IEvent<MemberLocationRecordedEvent>
    {
        public String Origin { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid MemberID { get; set; }
        public long RecordedTime { get; set; }
        public Guid ReportID { get; set; }
        public Guid TeamID { get; set; }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public MemberLocationRecordedEvent FromJson(string jsonBody)
        {
            return JsonConvert.DeserializeObject<MemberLocationRecordedEvent>(jsonBody);
        }
    }
}
