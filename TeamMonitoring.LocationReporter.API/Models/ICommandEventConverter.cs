using TeamMonitoring.LocationReporter.API.Events;

namespace TeamMonitoring.LocationReporter.API.Models
{
    public interface ICommandEventConverter
    {
        MemberLocationRecordedEvent CommandToEvent(LocationReport locationReport);
    }
}